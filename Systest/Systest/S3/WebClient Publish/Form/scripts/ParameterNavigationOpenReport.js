app.controller("ParameterNavigationOpenReportController", ["$scope", "$rootScope", function ($scope, $rootScope) {
    
    $scope.iobjReportName = { istrReportName: "", iblnConstant: false };
    $scope.iobjReportTemplate = { istrReportTemplate: "", iblnConstant: false };
   
    $scope.Init = function () {
        if ($scope.model.dictAttributes.sfwNavigationParameter) {
            var lstParam = $scope.model.dictAttributes.sfwNavigationParameter.split(";");
            for (var i = 0, len = lstParam.length; i < len; i++) {
                if (lstParam[i]) {
                    var lst = lstParam[i].split("=");
                    if (lst[0] == "ReportName") {
                        if (lst[1].indexOf("#") > -1) {
                            $scope.iobjReportName.iblnConstant = true;
                            $scope.iobjReportName.istrReportName = lst[1].substring(1, lst[1].length);
                        }
                        else {
                            $scope.iobjReportName.istrReportName = lst[1];
                        }
                    }
                    else {
                        if (lst[1].indexOf("#") > -1) {
                            $scope.iobjReportTemplate.iblnConstant = true;
                            $scope.iobjReportTemplate.istrReportTemplate = lst[1].substring(1, lst[1].length);
                        }
                        else {
                            $scope.iobjReportTemplate.istrReportTemplate = lst[1];
                        }
                    }
                }
            }
        }
        $.connection.hubCreateNewObject.server.loadReportData().done(function (data) {
            $scope.$apply(function () {
                $scope.lstReportData = data;
            });
        });
    };
    
    $scope.onOkClick = function () {
        var lstrNavigationParameter = "";
        if ($scope.iobjReportName.istrReportName) {
            lstrNavigationParameter = "ReportName=";
            if ($scope.iobjReportName.iblnConstant) {
                lstrNavigationParameter += "#" + $scope.iobjReportName.istrReportName;
            }
            else {
                lstrNavigationParameter += $scope.iobjReportName.istrReportName;
            }
        }
        if ($scope.iobjReportTemplate.istrReportTemplate) {
            lstrNavigationParameter += lstrNavigationParameter == "" ? "ReportTemplate=" : ";ReportTemplate=";
            if ($scope.iobjReportTemplate.iblnConstant) {
                lstrNavigationParameter += "#" + $scope.iobjReportTemplate.istrReportTemplate;
            }
            else {
                lstrNavigationParameter += $scope.iobjReportTemplate.istrReportTemplate;
            }
        }
        if ($scope.model.dictAttributes.sfwNavigationParameter != lstrNavigationParameter) {
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwNavigationParameter, $scope.model.dictAttributes, "sfwNavigationParameter", lstrNavigationParameter);
        }
        $scope.onCancelClick();

    };
    $scope.onCancelClick = function () {
        $scope.ParameterNavigationOpenReportDialog.close();
    };

    $scope.Init();
}]);