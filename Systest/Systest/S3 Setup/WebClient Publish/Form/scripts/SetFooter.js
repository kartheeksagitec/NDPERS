app.controller("SetFooterController", ["$scope", "$rootScope", "ngDialog", function ($scope, $rootScope, ngDialog) {
    $scope.IsMVVM = false;
    $scope.lstOperations = [{ isSelect: false, operation: "Count", caption: "" },
        { isSelect: false, operation: "Sum", caption: "" },
        { isSelect: false, operation: "Min", caption: "" },
        { isSelect: false, operation: "Max", caption: "" },
        { isSelect: false, operation: "Avg", caption: "" },
        { isSelect: false, operation: "StaticText", caption: "" }];
    $scope.Footertemplate = {};
    for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
        if ($scope.TemplateFieldModel.Elements[i].Name == "FooterTemplate") {
            angular.copy($scope.TemplateFieldModel.Elements[i], $scope.Footertemplate);
            break;
        }
    }
    if ($scope.Footertemplate.Elements && $scope.Footertemplate.Elements.length > 0) {
        if (!$scope.Footertemplate.Elements[0].dictAttributes.sfwEntityField) {
            $scope.footerTemplateLabel = $scope.Footertemplate.Elements[0];
        }
        else {
            $scope.footerTemplateLabelIsNotMVVM = $scope.Footertemplate.Elements[0];
        }

    }
    if ($scope.Footertemplate.Elements && $scope.Footertemplate.Elements.length > 0 && !$scope.Footertemplate.Elements[0].dictAttributes.sfwEntityField) {
        $scope.IsMVVM = true;
        var operations = [];
        if ($scope.footerTemplateLabel.dictAttributes.sfwFooterType) {
            operations = $scope.footerTemplateLabel.dictAttributes.sfwFooterType.split(',');
        }
        var captions = [];
        if ($scope.footerTemplateLabel.dictAttributes.sfwText) {
            captions = $scope.footerTemplateLabel.dictAttributes.sfwText.split(';');
        }
        if (operations.length == captions.length) {
            for (var i = 0; i < operations.length; i++) {
                var caption = "";
                if (captions[i].contains(":")) {
                    caption = captions[i].split(':')[0];
                }
                for (var j = 0; j < $scope.lstOperations.length; j++) {
                    if ($scope.lstOperations[j].operation == operations[i]) {
                        $scope.lstOperations[j].isSelect = true;
                        $scope.lstOperations[j].caption = caption;
                        break;
                    }
                }
            }

        }
    }
    $scope.FooterDetailsChange = function () {
        var isSelectedArray = [];
        var isNotSelectedArray = [];
        for (var i = 0; i < $scope.lstOperations.length; i++) {
            if ($scope.lstOperations[i].isSelect == true) {
                isSelectedArray.push($scope.lstOperations[i]);
            }
            else {
                $scope.lstOperations[i].caption = "";
                isNotSelectedArray.push($scope.lstOperations[i]);
            }
        }
        $scope.lstOperations = [];
        if (isSelectedArray.length > 0 && $scope.footerTemplateLabel == undefined) {
            var objFooter = { dictAttributes: {}, Elements: [{ dictAttributes: { sfwText: "", sfwFooterType: "" }, Elements: [], Children: [], Name: "sfwLabel", Value: "", prefix: "swc" }], Children: [], Name: "FooterTemplate", Value: "", prefix: "asp" };
            $scope.Footertemplate = objFooter;
            $scope.footerTemplateLabel = $scope.Footertemplate.Elements[0];
        }

        if ($scope.footerTemplateLabel) {
            $scope.footerTemplateLabel.dictAttributes.sfwFooterType = "";
            $scope.footerTemplateLabel.dictAttributes.sfwText = "";
        }

        for (var i = 0; i < isSelectedArray.length; i++) {
            if ($scope.footerTemplateLabel != undefined) {
                $scope.footerTemplateLabel.dictAttributes.sfwFooterType += isSelectedArray[i].operation;

                if (isSelectedArray[i].caption != "") {
                    $scope.footerTemplateLabel.dictAttributes.sfwText += isSelectedArray[i].caption + ":{" + i + "}";
                }
                else {
                    $scope.footerTemplateLabel.dictAttributes.sfwText += "{" + i + "}";
                }
                if (i < isSelectedArray.length - 1) {
                    $scope.footerTemplateLabel.dictAttributes.sfwFooterType += ",";
                    $scope.footerTemplateLabel.dictAttributes.sfwText += ";";
                }
            }
            $scope.lstOperations.push(isSelectedArray[i]);
        }
        for (var i = 0; i < isNotSelectedArray.length; i++) {
            $scope.lstOperations.push(isNotSelectedArray[i]);
        }
    };
    $scope.clearFooterText = function (entField) {
        if (!entField && $scope.footerTemplateLabelIsNotMVVM) {
            $scope.footerTemplateLabelIsNotMVVM.dictAttributes.sfwText = "";
        }
    };
    $scope.onSetFooterSaveClick = function () {
        if ($scope.IsMVVM) {
            $scope.footerTemplateLabelIsNotMVVM = undefined;
            if ($scope.footerTemplateLabel != undefined) {
                var flag = false;
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "FooterTemplate") {

                        if (($scope.footerTemplateLabel.dictAttributes.sfwFooterType || $scope.footerTemplateLabel.dictAttributes.sfwText) && $scope.TemplateFieldModel.Elements[i].Elements.length > 0) {
                            $rootScope.UndRedoBulkOp("Start");
                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwEntityField, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwEntityField", "");

                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwFooterType, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwFooterType", $scope.footerTemplateLabel.dictAttributes.sfwFooterType);
                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwText, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwText", $scope.footerTemplateLabel.dictAttributes.sfwText);

                            $rootScope.UndRedoBulkOp("End");
                        } else {
                            $rootScope.DeleteItem($scope.TemplateFieldModel.Elements[i], $scope.TemplateFieldModel.Elements);
                        }
                        flag = true;
                        break;
                    }
                }
                if (flag == false) {
                    $rootScope.PushItem($scope.Footertemplate, $scope.TemplateFieldModel.Elements);
                }
            }
            else {
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "FooterTemplate") {
                        $rootScope.DeleteItem($scope.TemplateFieldModel.Elements[i], $scope.TemplateFieldModel.Elements);
                    }
                }
            }
        }
        else {
            $scope.footerTemplateLabel = undefined;
            if ($scope.footerTemplateLabelIsNotMVVM != undefined) {
                var flag = false;
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "FooterTemplate") {
                     
                        if ($scope.footerTemplateLabelIsNotMVVM.dictAttributes.sfwEntityField && $scope.TemplateFieldModel.Elements[i].Elements.length > 0) {
                            $rootScope.UndRedoBulkOp("Start");
                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwEntityField, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwEntityField",$scope.footerTemplateLabelIsNotMVVM.dictAttributes.sfwEntityField);
                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwText, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwText", $scope.footerTemplateLabelIsNotMVVM.dictAttributes.sfwText);
                            $rootScope.EditPropertyValue($scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes.sfwFooterType, $scope.TemplateFieldModel.Elements[i].Elements[0].dictAttributes, "sfwFooterType", "");
                            $rootScope.UndRedoBulkOp("End");
                        } else {
                            $rootScope.DeleteItem($scope.TemplateFieldModel.Elements[i], $scope.TemplateFieldModel.Elements);
                        }

                        flag = true;
                        break;
                    }
                }
                if (flag == false) {
                    var objFooter = { dictAttributes: {}, Elements: [{ dictAttributes: { sfwText: "", sfwEntityField: "" }, Elements: [], Children: [], Name: "sfwLabel", Value: "", prefix: "swc" }], Children: [], Name: "FooterTemplate", Value: "", prefix: "asp" };
                    $scope.Footertemplate = objFooter;
                    $scope.Footertemplate.Elements[0].dictAttributes = $scope.footerTemplateLabelIsNotMVVM.dictAttributes;
                    $rootScope.PushItem($scope.Footertemplate, $scope.TemplateFieldModel.Elements);
                }
            }
            else {
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "FooterTemplate") {
                        $rootScope.DeleteItem($scope.TemplateFieldModel.Elements[i], $scope.TemplateFieldModel.Elements);
                    }
                }
            }


        }
        $scope.onCancelClick();
    };
    $scope.onCancelClick = function () {
        $scope.SetFoterDialog.close();
    };
    $scope.setRowSelection = function (condition) {
        $scope.selectedOperation = condition;
    };
    $scope.isMoveUp = function () {
        if ($scope.selectedOperation == undefined) {
            return true;
        }
        else {
            var index = $scope.lstOperations.indexOf($scope.selectedOperation);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.isMoveDown = function () {
        if ($scope.selectedOperation == undefined) {
            return true;
        }
        else {
            var index = $scope.lstOperations.indexOf($scope.selectedOperation);
            if (index != $scope.lstOperations.length - 1 && index != -1 && $scope.lstOperations[index + 1].isSelect == true) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.MoveUp = function () {
        var index = $scope.lstOperations.indexOf($scope.selectedOperation);
        var tempObj = $scope.lstOperations[index - 1];
        $scope.lstOperations[index - 1] = $scope.lstOperations[index];
        $scope.lstOperations[index] = tempObj;
        $scope.FooterDetailsChange();
    };
    $scope.MoveDown = function () {
        var index = $scope.lstOperations.indexOf($scope.selectedOperation);
        var tempObj = $scope.lstOperations[index + 1];
        $scope.lstOperations[index + 1] = $scope.lstOperations[index];
        $scope.lstOperations[index] = tempObj;
        $scope.FooterDetailsChange();
    };
}]);