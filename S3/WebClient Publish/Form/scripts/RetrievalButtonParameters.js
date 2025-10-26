app.controller("RetrievalButtonParameterController", ["$scope", "$rootScope", "$filter", "ngDialog", function ($scope, $rootScope, $filter, ngDialog) {
    $scope.ShowAll = {};
    $scope.ShowAll.isShowAll = false;
    if ($scope.$parent.formmodel && $scope.$parent.formmodel.dictAttributes) {
        $scope.sourceForm = $scope.$parent.formmodel.dictAttributes.ID;
    }
    if ($scope.targetFormModel && $scope.targetFormModel.dictAttributes.sfwType == "Lookup") {
        $scope.targetForm = $scope.targetFormModel.dictAttributes.ID;
        var criteriaPanel = GetCriteriaPanel($scope.targetFormModel);
    }
    else if ($scope.targetFormModel && $scope.targetFormModel.dictAttributes.sfwType == "FormLinkLookup") {
        $scope.targetForm = $scope.targetFormModel.dictAttributes.ID;
        var criteriaPanel = $scope.targetFormModel.Elements.filter(function (itm) { return itm.Name == "items"; });
        if (criteriaPanel && criteriaPanel.length > 0) {
            criteriaPanel = criteriaPanel[0];
        }
    }

    $scope.lstTargetFields = [];
    // var abc = "sfwRetrievalParameters";
    if (criteriaPanel) {
        GetRetrievalTargetControls(criteriaPanel, $scope.lstTargetFields);
    }
    $scope.lstSourceFields = [];
    var panelObject = [];
    if ($scope.isFormLink && $scope.isFormLink == true) {
        panelObject = GetVM("items", $scope.model);
    }
    else {
        if ($scope.$parent.formmodel && $scope.$parent.formmodel.dictAttributes && $scope.$parent.formmodel.dictAttributes.sfwType == "Wizard") {
            panelObject = GetVM("sfwWizardStep", $scope.model);
        }
        else if ($scope.$parent.formmodel && $scope.$parent.formmodel.dictAttributes &&
            ($scope.$parent.formmodel.dictAttributes.sfwType == "Report" || $scope.$parent.formmodel.dictAttributes.sfwType == "Correspondence" || $scope.$parent.formmodel.dictAttributes.sfwType == "UserControl" )) {
            panelObject = GetVM("sfwTable", $scope.model);
        }
        else {
            panelObject = GetVM("sfwPanel", $scope.model);
        }
    }
    if (panelObject) {
        GetRetrievalSourceControls(panelObject, $scope.lstSourceFields);
    }
    $scope.ShowAllControls = function () {
        $scope.lstSourceFields = [];
        if ($scope.ShowAll.isShowAll) {
            GetRetrievalSourceControls($scope.formmodel, $scope.lstSourceFields);
        }
        else {
            GetRetrievalSourceControls(panelObject, $scope.lstSourceFields);
        }
    };
    if ($scope.model.dictAttributes.sfwRetrievalParameters && $scope.model.dictAttributes.sfwRetrievalParameters != "") {
        var tempparameters = $scope.model.dictAttributes.sfwRetrievalParameters.split(";");
        for (var i = 0; i < tempparameters.length; i++) {
            var tempparameter = tempparameters[i].split("=");
            for (var j = 0; j < $scope.lstTargetFields.length; j++) {
                if ($scope.lstTargetFields[j].ID == tempparameter[0]) {
                    if (tempparameter[1].contains("#")) {
                        var sourceControl = tempparameter[1].replace("#", "");
                        $scope.lstTargetFields[j].isConstant = true;
                        $scope.lstTargetFields[j].sourceControl = sourceControl;
                    }
                    else {
                        $scope.lstTargetFields[j].sourceControl = tempparameter[1];
                    }

                }
            }
        }
    }

    $scope.okClick = function () {
        var retrievalparameter = "";
        for (var i = 0; i < $scope.lstTargetFields.length; i++) {
            if ($scope.lstTargetFields[i].sourceControl && $scope.lstTargetFields[i].sourceControl != "") {
                if (retrievalparameter == "") {
                    if ($scope.lstTargetFields[i].isConstant) {
                        retrievalparameter = $scope.lstTargetFields[i].ID + "=#" + $scope.lstTargetFields[i].sourceControl;
                    }
                    else {
                        retrievalparameter = $scope.lstTargetFields[i].ID + "=" + $scope.lstTargetFields[i].sourceControl;
                    }
                }
                else {
                    if ($scope.lstTargetFields[i].isConstant) {
                        retrievalparameter += ";" + $scope.lstTargetFields[i].ID + "=#" + $scope.lstTargetFields[i].sourceControl;
                    }
                    else {
                        retrievalparameter += ";" + $scope.lstTargetFields[i].ID + "=" + $scope.lstTargetFields[i].sourceControl;
                    }
                }
            }
        }
        $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwRetrievalParameters, $scope.model.dictAttributes, "sfwRetrievalParameters", retrievalparameter);
        //$scope.model.dictAttributes.sfwRetrievalParameters = retrievalparameter;
        $scope.RetrievalcloseClick();
    };
    $scope.RetrievalcloseClick = function () {
        //ngDialog.close($scope.RetrievalButtonParameterDialog.ID);
        $scope.RetrievalButtonParaDialog.close();
    };
    $scope.selectControlID = function (controlID) {
        $scope.selectedControlID = controlID;
    };
    $scope.ExpandCollapsedControl = function (ObjControl) {
        ObjControl.IsExpanded = !ObjControl.IsExpanded;
    };
}]);