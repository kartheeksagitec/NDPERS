app.controller("GroupExpressionController", ["$scope", "ngDialog","$rootScope", function ($scope, ngDialog, $rootScope) {
    $scope.lstColumnName = [];
    $scope.lstGridColumns = [];
    $scope.lstDefualtOrder = ["", "asc", "desc"];
    $scope.lstGroupExpression = [];
    $scope.lstGroupExpression.push("");
    $scope.Init = function () {
        PopulateGridDataField($scope.model, $scope.lstColumnName);
        for (var i = 0; i < $scope.lstColumnName.length; i++) {
            $scope.lstGroupExpression.push(i + 1);
        }
        for (var i = 0; i < $scope.lstColumnName.length; i++) {
            $scope.lstGridColumns.push({ FieldName: $scope.lstColumnName[i] });
        }
        if ($scope.model.dictAttributes.sfwGroupExpression) {
            var GroupExpression = $scope.model.dictAttributes.sfwGroupExpression.split(',');
            for (i = 0; i < GroupExpression.length; i++) {
                var filedNameAndOrder = GroupExpression[i].split(" ");
                for (var j = 0; j < $scope.lstGridColumns.length ; j++) {
                    if ($scope.lstGridColumns[j].FieldName === filedNameAndOrder[0]) {
                        $scope.lstGridColumns[j].GroupExpression = i + 1;
                    }
                }
            }
        }
    };
    
    $scope.okClick = function () {
        var groupExpression = "";
        var dataKeys = "";
        for (var i = 0; i < $scope.lstGridColumns.length; i++) {
           
            if ($scope.lstGridColumns[i].GroupExpression) {


                var strGroupSeq = $scope.lstGridColumns[i].GroupExpression;
                var iGroupSeq = strGroupSeq;
                if (iGroupSeq > 0) {
                    if (!groupExpression) {
                        groupExpression += strGroupSeq + ";" + $scope.lstGridColumns[i].FieldName;
                    }
                    else {
                        groupExpression += "," + strGroupSeq + ";" + $scope.lstGridColumns[i].FieldName;
                    }
                   
                }
            }
        }

        var slFieldSeq = [];
        var lstGroupExpression = groupExpression.split(',');
        for (var i = 0; i < lstGroupExpression.length; i++) {
            var strIndex = lstGroupExpression[i].substring(0, lstGroupExpression[i].indexOf(';')).trim();
            var strGroupField = lstGroupExpression[i].substring(lstGroupExpression[i].indexOf(';') + 1).trim();
            slFieldSeq.splice(strIndex - 1, 0, strGroupField);
        }
        groupExpression = "";

        angular.forEach(slFieldSeq, function (itm) {
            if (groupExpression.length === 0)
                groupExpression = itm;
            else
                groupExpression += "," + itm;
        });

        $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwGroupExpression, $scope.model.dictAttributes, "sfwGroupExpression", groupExpression);

        $scope.closeClick();
    };
    $scope.closeClick = function () {
        $scope.GroupExpressionDialog.close();
    };

    $scope.Init();

}]);