app.filter('cut', function () {
    return function (value, wordwise, max, tail) {
        if (!value) return '';

        max = parseInt(max, 10);
        if (!max) return value;
        if (value.length <= max) return value;

        value = value.substr(0, max);
        if (wordwise) {
            var lastspace = value.lastIndexOf(' ');
            if (lastspace != -1) {
                //Also remove . and , so its gives a cleaner result.
                if (value.charAt(lastspace-1) == '.' || value.charAt(lastspace-1) == ',') {
                    lastspace = lastspace - 1;
                }
                value = value.substr(0, lastspace);
            }
        }

        return value + (tail || ' …');
    };
});


app.filter('entitybyTable', function () {

    return function (items, tablename, isSubQueryOnly) {
        var filtered = [];

        if (!tablename || !items.length) {
            return items;
        }

        items.forEach(function (itemElement, itemIndex) {
            itemElement.Queries.forEach(function (QElement, locationIndex) {
                if (QElement.SqlQuery) {
                    if (isSubQueryOnly && QElement.QueryType && QElement.QueryType == "SubSelectQuery") {
                        var testarray = QElement.SqlQuery.toLowerCase().split(/[()\r\n\s,]+/);
                        var testindex = testarray.indexOf(tablename.toLowerCase());
                        if (testindex >= 0) {
                            filtered.push({ ID: itemElement.EntityName + "." + QElement.ID, SqlQuery: QElement.SqlQuery, Parameters: QElement.Parameters ? QElement.Parameters : undefined });
                            return false;
                        }
                    }
                    else if (!isSubQueryOnly) {
                        var testarray = QElement.SqlQuery.toLowerCase().split(/[()\r\n\s,]+/);
                        var testindex = testarray.indexOf(tablename.toLowerCase());
                        if (testindex >= 0) {
                            filtered.push({ ID: itemElement.EntityName + "." + QElement.ID, SqlQuery: QElement.SqlQuery, Parameters: QElement.Parameters ? QElement.Parameters : undefined });
                            return false;
                        }
                    }
                }              
            });
        });

        return filtered;
    };
});

app.filter('filesize', function () {
    var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'];

    return function (bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) {
            return '?';
        }

        var unit = 0;

        while (bytes >= 1024) {
            bytes /= 1024;
            unit++;
        }

        return bytes.toFixed(+precision) + ' ' + units[unit];
    };
});

app.filter('range', function () {
    return function (input, min, max) {
        min = parseInt(min);
        max = parseInt(max);
        for (var i = min; i <= max; i++)
            input.push(i.toString());
        return input;
    };
});

app.filter('controlbyFormtype', function () {
    return function (controlList, visibleOption, ctrlType, formModel) {
        var filteredControlList = controlList,
            sfwType = formModel ? (formModel.hasOwnProperty("dictAttributes") ? formModel.dictAttributes.sfwType : "") : "";
            
        function checkCustomcondition(ctrl, formmodel) {
            var isValid = true;
            // for custom condition
            switch (ctrl.Name) {
                case "Label" : 
                    if (sfwType === 'Lookup' && (formModel.IsLookupCriteriaEnabled == true || formModel.IsPrototypeLookupCriteriaEnabled)) {
                        isValid = false;
                    }
                    break;
                default: break;
            }            
            return isValid;
        }
        if (ctrlType && sfwType && visibleOption && controlList.length) {
            filteredControlList = controlList.filter(function (ctrl) {
                if (ctrl && ctrl.hasOwnProperty("type") && angular.isArray(ctrl.excludeFormTypes) && angular.isArray(ctrl.optionEnabled)) {
                    if (ctrl.type === ctrlType && (ctrl.excludeFormTypes.indexOf(sfwType) < 0) && (ctrl.optionEnabled.indexOf(visibleOption) >= 0)) {
                        // custom condition go here
                        if (checkCustomcondition(ctrl, formModel)) {
                            return ctrl;
                        }
                    }
                }
            });
        }
        return filteredControlList;
    };
});