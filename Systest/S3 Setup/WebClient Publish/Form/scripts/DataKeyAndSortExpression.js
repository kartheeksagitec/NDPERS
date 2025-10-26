app.controller("DataKeysController", ["$scope", "ngDialog", "$rootScope", function ($scope, ngDialog, $rootScope) {
    $scope.lstColumnName = [];
    $scope.lstGridColumns = [];
    $scope.lstDefualtOrder = ["", "asc", "desc"];
    $scope.lstSortExpression = [];
    $scope.lstSortExpression.push("");
    PopulateGridDataField($scope.model, $scope.lstColumnName);
    for (var i = 0; i < $scope.lstColumnName.length; i++) {
        $scope.lstSortExpression.push(i + 1);
    }
    for (var i = 0; i < $scope.lstColumnName.length; i++) {
        $scope.lstGridColumns.push({ FieldName: $scope.lstColumnName[i] });
    }
    if ($scope.model.dictAttributes.sfwSortExpression) {
        var SortExpression = $scope.model.dictAttributes.sfwSortExpression.split(',');
        for (i = 0; i < SortExpression.length; i++) {
            var filedNameAndOrder = SortExpression[i].split(" ");
            for (var j = 0; j < $scope.lstGridColumns.length ; j++) {
                if ($scope.lstGridColumns[j].FieldName == filedNameAndOrder[0]) {
                    if (filedNameAndOrder.length > 1) {
                        $scope.lstGridColumns[j].DefaultOrder = filedNameAndOrder[1];
                    }
                    $scope.lstGridColumns[j].SortExpression = i + 1;
                }
            }
        }
    }
    if ($scope.model.dictAttributes.sfwDataKeyNames) {
        var dataKeys = $scope.model.dictAttributes.sfwDataKeyNames.split(",");
        for (i = 0; i < dataKeys.length; i++) {
            for (var j = 0; j < $scope.lstGridColumns.length ; j++) {
                if ($scope.lstGridColumns[j].FieldName == dataKeys[i]) {
                    $scope.lstGridColumns[j].DataKey = true;
                }
            }
        }
    }
    $scope.okClick = function () {
        var sortExpression = "";
        var dataKeys = "";
        for (var i = 0; i < $scope.lstGridColumns.length; i++) {
            if ($scope.lstGridColumns[i].DataKey) {
                if (dataKeys == "") {
                    dataKeys = $scope.lstGridColumns[i].FieldName;
                }
                else {
                    dataKeys += "," + $scope.lstGridColumns[i].FieldName;
                }
            }
            if ($scope.lstGridColumns[i].SortExpression) {


                var strSortSeq = $scope.lstGridColumns[i].SortExpression;
                var iSortSeq = strSortSeq;
                if (iSortSeq > 0) {
                    if (!sortExpression) {
                        sortExpression += strSortSeq + ";" + $scope.lstGridColumns[i].FieldName;
                    }
                    else {
                        sortExpression += "," + strSortSeq + ";" + $scope.lstGridColumns[i].FieldName;
                    }
                    if ($scope.lstGridColumns[i].DefaultOrder) {
                        sortExpression += " " + $scope.lstGridColumns[i].DefaultOrder;
                    }
                }
            }
        }

        var slFieldSeq = [];
        var lstSortExpression = sortExpression.split(',');
        for (var i = 0; i < lstSortExpression.length; i++) {
            var strIndex = lstSortExpression[i].substring(0, lstSortExpression[i].indexOf(';')).trim();
            var strSortField = lstSortExpression[i].substring(lstSortExpression[i].indexOf(';') + 1).trim();
            slFieldSeq.splice(strIndex - 1, 0, strSortField);
        }
        sortExpression = "";

        angular.forEach(slFieldSeq, function (itm) {
            if (sortExpression.length === 0)
                sortExpression = itm;
            else
                sortExpression += "," + itm;
        });

        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwSortExpression, $scope.model.dictAttributes, "sfwSortExpression", sortExpression);
        $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwDataKeyNames, $scope.model.dictAttributes, "sfwDataKeyNames", dataKeys);
        $rootScope.UndRedoBulkOp("End");

        $scope.closeClick();
    };
    $scope.closeClick = function () {
        $scope.DataKeyAndSortExpressionDialog.close();
    };

    $scope.ChangeSortExpression = function (selectedcolumn) {
        if (selectedcolumn && !selectedcolumn.SortExpression) {
            selectedcolumn.DefaultOrder = "";
        }
    };
}]);