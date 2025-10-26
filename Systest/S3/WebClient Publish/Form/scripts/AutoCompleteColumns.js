app.controller("AutoCompleteColumnsController", ["$scope", "$rootScope", "$filter", "ngDialog", "$Entityintellisenseservice", "$timeout", "$SgMessagesService", function ($scope, $rootScope, $filter, ngDialog, $Entityintellisenseservice, $timeout, $SgMessagesService) {
    $scope.lstColumns = [];
    $scope.lstAddedColumns = [];
    $scope.lstOperators = ["", "=", "!=", "<", "<=", ">", ">=", "like", "contains"];
    $scope.init = function () {
        if ($scope.strSelectedAutoColumns) {
            var lsttempColumnsString = $scope.strSelectedAutoColumns.split(";");
            for (var i = 0; i < lsttempColumnsString.length; i++) {
                var lsttempcolumns = lsttempColumnsString[i].split(",");
                var obj = { ColumnName: lsttempcolumns[0], Header: lsttempcolumns[1], Operator: lsttempcolumns[2], DataType: lsttempcolumns[3] };
                $scope.lstAddedColumns.push(obj);
            }
        }
        if ($scope.model.autocompleteType === "Query" && $scope.model.dictAttributes.sfwAutoQuery) {
            if ($scope.IsAutoComplete) {
                $.connection.hubForm.server.getEntityQueryColumns($scope.model.dictAttributes.sfwAutoQuery, "AutoCompleteColumnsController").done(function (data) {
                    $scope.receiveQueryColumns(data);
                });
            }
        }
        else if ($scope.model.autocompleteType === "Method" && $scope.model.dictAttributes.sfwAutoMethod) {
            var llstObjectMethods = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, null, null, true, false, true, false, false, false);
            llstObjectMethods = llstObjectMethods.filter(function (aobjMethod) { return aobjMethod.ID.toLowerCase() === $scope.model.dictAttributes.sfwAutoMethod.toLowerCase(); });
            if (llstObjectMethods && llstObjectMethods.length) {
                if (llstObjectMethods[0].Entity) {
                    var llstAttributes = $Entityintellisenseservice.GetIntellisenseData(llstObjectMethods[0].Entity, null, null, true, true, false, false, false, false);
                    llstAttributes = llstAttributes.map(function (attr) { return { CodeID: attr.ID, Description: attr.ID, DataType: attr.DataType }; });
                    $scope.receiveQueryColumns(llstAttributes);
                }
            }
        }
    }

    $scope.receiveQueryColumns = function (data) {
        $scope.$evalAsync(function () {
            for (var i = 0; i < data.length; i++) {
                data[i].isChecked = false;
            }
            $scope.lstColumns = data;
        });
    };

    $scope.AddColumns = function () {
        if ($scope.lstColumns.length > 0) {
            for (var i = 0; i < $scope.lstColumns.length; i++) {
                if ($scope.lstColumns[i].isChecked) {
                    var flag = true;
                    for (var j = 0; j < $scope.lstAddedColumns.length; j++) {
                        if ($scope.lstAddedColumns[j].ColumnName == $scope.lstColumns[i].CodeID) {
                            flag = false;
                            $SgMessagesService.Message('Message', $scope.lstColumns[i].CodeID + " is already added in Collection, please Check.");
                            break;
                        }
                    }
                    if (flag == true) {
                        var datatype = $scope.lstColumns[i].DataType;
                        if ($scope.lstColumns[i].DataType == "Decimal" || ($scope.lstColumns[i].DataType.indexOf("Int") == 0)) {
                            datatype = "Numeric";
                        }
                        var obj = { ColumnName: $scope.lstColumns[i].CodeID, Header: $scope.lstColumns[i].CodeID.split("_").join(" "), Operator: "", DataType: datatype };
                        $scope.lstAddedColumns.push(obj);
                    }
                }
            }
        }
    };

    $scope.okClick = function () {
        $scope.strSelectedAutoColumns = "";
        if ($scope.lstAddedColumns.length > 0) {
            for (var i = 0; i < $scope.lstAddedColumns.length; i++) {
                var obj = $scope.lstAddedColumns[i];
                if ($scope.strSelectedAutoColumns == "") {
                    $scope.strSelectedAutoColumns = obj.ColumnName + "," + obj.Header + "," + obj.Operator + "," + obj.DataType;
                }
                else {
                    $scope.strSelectedAutoColumns += ";" + obj.ColumnName + "," + obj.Header + "," + obj.Operator + "," + obj.DataType;
                }
            }
        }
        $scope.$emit("onAutoCompletelColumnsClick", $scope.strSelectedAutoColumns);
        $scope.closeClick();
    };
    $scope.closeClick = function () {
        //ngDialog.close($scope.AutoCompleteColumnsDialog.id);

        $scope.AutoCompleteColumnDialog.close();
    };

    $scope.selectColumn = function (column) {
        $scope.selectedColumn = column;
    };
    $scope.isMoveUp = function () {
        if ($scope.selectedColumn == undefined) {
            return true;
        }
        else {
            var index = $scope.lstAddedColumns.indexOf($scope.selectedColumn);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.isMoveDown = function () {
        if ($scope.selectedColumn == undefined) {
            return true;
        }
        else {
            var index = $scope.lstAddedColumns.indexOf($scope.selectedColumn);
            if (index != $scope.lstAddedColumns.length - 1 && index != -1) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.MoveUp = function () {
        var index = $scope.lstAddedColumns.indexOf($scope.selectedColumn);
        var tempObj = $scope.lstAddedColumns[index - 1];
        $scope.lstAddedColumns[index - 1] = $scope.lstAddedColumns[index];
        $scope.lstAddedColumns[index] = tempObj;
        $scope.scrollToSelectedField("#form-autocomplete-column-param", ".selected");
    };
    $scope.MoveDown = function () {
        var index = $scope.lstAddedColumns.indexOf($scope.selectedColumn);
        var tempObj = $scope.lstAddedColumns[index + 1];
        $scope.lstAddedColumns[index + 1] = $scope.lstAddedColumns[index];
        $scope.lstAddedColumns[index] = tempObj;
        $scope.scrollToSelectedField("#form-autocomplete-column-param", ".selected");
    };
    $scope.deleteColumns = function () {
        if ($scope.lstAddedColumns.length > 1) {
            var index = $scope.lstAddedColumns.indexOf($scope.selectedColumn);
            $scope.lstAddedColumns.splice(index, 1);
            $scope.selectedColumn = undefined;
        }
    };
    $scope.scrollToSelectedField = function (parentDiv, selectedElement) {
        $timeout(function () {
            var $divDom = $(parentDiv).find("table tbody");
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 200, offsetLeft: 0 }, null);
                return false;
            }

        });
    }
    $scope.init();
}]);