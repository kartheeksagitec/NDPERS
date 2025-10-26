app.controller("NavigationParameterOpenWordController", ["$scope", "$rootScope", function ($scope, $rootScope) {

    $scope.Init = function () {
        if ($scope.model && $scope.model.dictAttributes.sfwNavigationParameter) {
            if ($scope.model.dictAttributes.sfwNavigationParameter.contains(';')) {
                var alParams = $scope.model.dictAttributes.sfwNavigationParameter.split(';');
                for (var i = 0; i < alParams.length; i++) {
                    if (alParams[i].contains("=")) {
                        var strParamField = '';
                        strParamField = alParams[i].substring(0, alParams[i].indexOf('='));
                        if (strParamField == "TemplateName") {
                            $scope.TemplateName = alParams[i].substring(alParams[i].indexOf('=') + 1);
                        }
                        else if (strParamField == "TrackingID") {
                            $scope.TrackingID = alParams[i].substring(alParams[i].indexOf('=') + 1);
                        }
                    }
                }
            }
            else if ($scope.model.dictAttributes.sfwNavigationParameter.contains('=')) {
                var strParamField = '';
                strParamField = $scope.model.dictAttributes.sfwNavigationParameter.substring(0, $scope.model.dictAttributes.sfwNavigationParameter.indexOf('='));
                if (strParamField == "TemplateName") {
                    $scope.TemplateName = $scope.model.dictAttributes.sfwNavigationParameter.substring($scope.model.dictAttributes.sfwNavigationParameter.indexOf('=') + 1);
                }
                else if (strParamField == "TrackingID") {
                    $scope.TrackingID = $scope.model.dictAttributes.sfwNavigationParameter.substring($scope.model.dictAttributes.sfwNavigationParameter.indexOf('=') + 1);
                }
            }
        }
        $scope.PopulateCorrespondenceTemplate();
    };

    //#region When dialog close on Ok and Cancel button
    $scope.onOkClick = function () {
        var strCustomAttribute = "";

        var objTemplateName = { ParameterField: "TemplateName", ParameterValue: $scope.TemplateName };
        var objTrackingID = { ParameterField: "TrackingID", ParameterValue: $scope.TrackingID };
        var collection = [];
        collection.push(objTemplateName);
        collection.push(objTrackingID);

        if (collection.length > 0) {
            strCustomAttribute = $scope.GetSavedString(collection);
        }
        if (strCustomAttribute != undefined) {
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwNavigationParameter, $scope.model.dictAttributes, "sfwNavigationParameter", strCustomAttribute);
        }

        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        $scope.NavigationParameterOpenWordDialog.close();
    };

    $scope.GetSavedString = function (ParameterCollection) {
        var strReturn = "";
        angular.forEach(ParameterCollection, function (objParams) {
            var strParamField = objParams.ParameterField;
            var strParamValue = objParams.ParameterValue;
            if (strParamValue != "" && strParamField != undefined) {
                if ((strParamValue != undefined && strParamValue != "") && (strParamField != undefined && strParamField != "")) {
                    var blnConstatnt = objParams.Constants;

                    if (blnConstatnt) {
                        strParamValue = "#" + strParamValue;
                    }

                    var strParam = strParamValue;

                    if (strParamValue.toLowerCase() != strParamField.toLowerCase()) {
                        strParam = strParamField + '=' + strParamValue;
                    }

                    if (strReturn == "") {
                        strReturn = strParam;
                    }
                    else {
                        strReturn += ';' + strParam;
                    }
                }
            }
        });
        return strReturn;
    };
    //#endregion

    $scope.PopulateCorrespondenceTemplate = function () {
        $.connection.hubCreateNewObject.server.loadCorrespondenceTemplate(false).done(function (data) {
            $scope.receiveCorrespondenceTemplateForForm(data);
        });
    };
    $scope.receiveCorrespondenceTemplateForForm = function (data) {
        $scope.$apply(function () {
            $scope.lstCorrTemplates = data;
        });
    };


    $scope.Init();
}]);