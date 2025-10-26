app.controller("AutoCompleteParameterController", ["$scope", "$rootScope", "$filter", "ngDialog", function ($scope, $rootScope, $filter, ngDialog) {
    $scope.ShowAll = {};
    $scope.ShowAll.isShowAll = false;

    $scope.lstParameters = [];
    var panelObject;
    $scope.Init = function () {
        var obj = { ID: "Main", Name: 'sfwPanel', Elements: [], IsExpanded: false };
        $scope.lstParameters.push(obj);
        panelObject = GetVM("sfwPanel", $scope.model);
        if ($scope.formobject.dictAttributes.sfwType == "UserControl") {
            panelObject = GetVM("sfwTable", $scope.model);
        }
        var isWizard = $scope.formobject.dictAttributes.sfwType == "Wizard";
        if (isWizard) {
            panelObject = GetVM("sfwWizardStep", $scope.model);
        }
        if ($scope.IsAutoComplete) {
            if ($scope.FormType == "FormLinkLookup" || $scope.FormType == "FormLinkMaintenance" || $scope.FormType == "FormLinkWizard") {
                if ($scope.formobject && $scope.formobject.Elements) {
                    for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                        if ($scope.formobject.Elements[i].Name == "items") {
                            $scope.PopulateAutoCompleteParametersForHtml($scope.formobject, $scope.lstParameters[0].Elements, $scope.FormType);
                        }
                    }
                }
            }
            else {
                if (panelObject) {
                    PopulateRetrievalOrAutoCompleteParameters(panelObject, $scope.lstParameters[0].Elements, $scope.FormType, false);
                }
            }
        }

        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
            $scope.$apply(function () {
                $scope.objGlobleParameters = data;
                $scope.PopulateGlobalParameters();
            });
        });
    };
    $scope.PopulateGlobalParameters = function () {
        function AddInobjGlobalParam(itm) {
            if (itm.dictAttributes && itm.dictAttributes.ID) {
                var strFieldName = itm.dictAttributes.ID;
                if (!globalParameters.filter(function (itm) { return itm == strFieldName.trim(); })) {
                    globalParameters.push(strFieldName.trim());
                }
                // objGlobalParam.Children.Add(new clsAvailableControl { ControlName = strFieldName });
                var mainItem = { ID: "~" + strFieldName, IsExpanded: false, IsSelected: false, Elements: [] };
                objGlobalParam.Elements.push(mainItem);
            }
        }

        if ($scope.objGlobleParameters) {
            var strFormType = $scope.formobject.dictAttributes.sfwType;
            if (($scope.model.Name == "sfwTextBox" || $scope.model.Name == "sfwButton" ||
                $scope.model.Name == "sfwCascadingDropDownList" || $scope.model.Name == "sfwDropDownList" || $scope.model.Name == "sfwMultiSelectDropDownList")
                && strFormType != "Report" && strFormType != "Correspondence") {
                if ($scope.model.Name == "sfwButton") {
                    if ($scope.model.dictAttributes.sfwMethodName != "btnOpen_Click" && $scope.model.dictAttributes.sfwMethodName != "btnNew_Click")
                        return;
                }
                var globalParameters = [];

                if ($scope.objGlobleParameters.Elements.length > 0) {
                    var objGlobalParam = { ID: "Global Parameters", IsExpanded: false, IsSelected: false, Elements: [] };

                    angular.forEach($scope.objGlobleParameters.Elements, AddInobjGlobalParam);
                    if (objGlobalParam.Elements.length > 0) {
                        $scope.lstParameters.push(objGlobalParam);
                    }
                }

            }
        }
    };


    $scope.ShowAllControls = function () {
        $scope.lstParameters = [];
        var obj = { ID: "Main", Name: 'sfwPanel', Elements: [], IsExpanded: false };
        $scope.lstParameters.push(obj);
        if ($scope.ShowAll.isShowAll) {
            if ($scope.formobject && $scope.formobject.Elements) {
                for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                    if ($scope.formobject.Elements[i].Name == "sfwTable") {
                        PopulateRetrievalOrAutoCompleteParameters($scope.formobject.Elements[i], $scope.lstParameters[0].Elements, $scope.FormType, false);
                        break;
                    }
                }
            }
        }
        else {
            if (panelObject) {
                PopulateRetrievalOrAutoCompleteParameters(panelObject, $scope.lstParameters[0].Elements, $scope.FormType, false);
            }
        }
        $scope.PopulateGlobalParameters();
    };
    $scope.ExpandCollapsedControl = function (lstparameter, event) {
        lstparameter.IsExpanded = !lstparameter.IsExpanded;
    };
    $scope.selectParameter = function (parameter, event) {
        $scope.selectedParameter = parameter;
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.bindParameterValue = function () {
        if ($scope.model.autocompleteType === "Query" && $scope.selectedCurrentQuery) {
            $scope.ParameterCollection = [];
            angular.forEach($scope.selectedCurrentQuery.Parameters, function (strParam) {
                ParameterField = strParam.ID;
                if (ParameterField.contains("@")) {
                    strParamField = ParameterField.substring(ParameterField.indexOf('@') + 1, ParameterField.length);
                }
                else {
                    strParamField = ParameterField;
                }

                var objParameter = { ParameterField: strParamField, ParameterValue: "" };
                $scope.ParameterCollection.push(objParameter);
            });
        }
        else if ($scope.model.autocompleteType === "Method" && $scope.selectedMethod) {
            $scope.ParameterCollection = [];
            for (var i = 0, len = $scope.selectedMethod.Parameters.length; i < len; i++) {
                var objParameter = { ParameterField: $scope.selectedMethod.Parameters[i].ID, ParameterValue: "" };
                $scope.ParameterCollection.push(objParameter);
            }
        }
        if ($scope.ParameterCollection != undefined && $scope.ParameterCollection.length > 0 && $scope.strSelectedParameters != undefined) {
            var lst = $scope.strSelectedParameters.split(";");
            for (var i = 0; i < lst.length; i++) {
                var lstControlsID = lst[i].split("=");
                for (var j = 0; j < $scope.ParameterCollection.length; j++) {
                    if ($scope.ParameterCollection[j].ParameterField == lstControlsID[0]) {
                        $scope.ParameterCollection[j].ParameterValue = lstControlsID[1];
                    }
                }
            }
        }
    };
    $scope.bindParameterValue();

    $scope.okClick = function () {
        $scope.strSelectedParameters = "";
        if ($scope.ParameterCollection != undefined) {
            for (var i = 0; i < $scope.ParameterCollection.length; i++) {
                if ($scope.ParameterCollection[i].ParameterValue != "") {
                    if ($scope.strSelectedParameters != undefined && $scope.strSelectedParameters != "") {
                        $scope.strSelectedParameters += ";" + $scope.ParameterCollection[i].ParameterField + "=" + $scope.ParameterCollection[i].ParameterValue;
                    }
                    else if ($scope.strSelectedParameters == undefined || $scope.strSelectedParameters == "") {
                        $scope.strSelectedParameters = $scope.ParameterCollection[i].ParameterField + "=" + $scope.ParameterCollection[i].ParameterValue;
                    }
                }
            }
        }
        if ($scope.IsAutoComplete) {
            $scope.$emit("onAutoCompleteParameterClick", $scope.strSelectedParameters);
        }
        $scope.closeClick();
    };
    $scope.closeClick = function () {

        $scope.objNewDialog.close();
    };

    //#region HTML Methods
    $scope.PopulateAutoCompleteParametersForHtml = function (panel, lstParameters, formType) {
        if (panel) {
            angular.forEach(panel.Elements, function (ctrl) {
                if (ctrl.Name != "sfwPanel" && ctrl.Name != "sfwWizardStep" && ctrl.Name != "sfwGridView") {
                    if (formType == "FormLinkLookup" && ctrl.dictAttributes.sfwDataField && ctrl.dictAttributes.ID) {
                        var obj = { ID: ctrl.dictAttributes.ID, ControlID: "", Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                    }
                    else if ((formType == "FormLinkMaintenance" || formType == "FormLinkWizard") && ctrl.dictAttributes.sfwEntityField && ctrl.dictAttributes.ID) {
                        var obj = { ID: ctrl.dictAttributes.ID, ControlID: "", Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                    }
                }
                if (ctrl.Elements.length > 0) {
                    var lsttempParameters = lstParameters;
                    if (ctrl.Name == "sfwPanel") {
                        var id = ctrl.dictAttributes.sfwCaption;
                        if (id == undefined || id == "") {
                            id = ctrl.dictAttributes.ID;
                        }
                        var obj = { ID: id, Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                        lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    }
                    if (ctrl.Name == "sfwWizardStep") {
                        var id = ctrl.dictAttributes.Title;
                        if (id == undefined || id == "") {
                            id = ctrl.dictAttributes.ID;
                        }
                        var obj = { ID: id, Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                        lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    }
                    //if (ctrl.Name == "sfwGridView") {
                    //    var obj = { ID: ctrl.dictAttributes.ID + "(Data Keys)", Elements: [], IsExpanded: false }
                    //    lstParameters.push(obj);
                    //    lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    //    PopulateGridEntityField(ctrl, lsttempParameters);
                    //}
                    $scope.PopulateAutoCompleteParametersForHtml(ctrl, lsttempParameters, formType);
                }
            });
        }
    };
    //#endregion

    //#region Call Init Method
    $scope.Init();
    //#endregion

}]);

