app.controller("NavParamGridSearchController", ["$scope", "$rootScope", "$filter", "ParameterFactory", "$EntityIntellisenseFactory", "$ValidationService", function ($scope, $rootScope, $filter, ParameterFactory, $EntityIntellisenseFactory, $ValidationService) {

    $scope.SelectedObject.IsShowAllControl = false;
    $scope.FieldCollection = [];

    $scope.Init = function () {
        if ($scope.formobject.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.formobject.dictAttributes.sfwType == "FormLinkLookup" || $scope.formobject.dictAttributes.sfwType == "FormLinkWizard") {

            var objItems;
            var arrNavParams;
            var mainItem = { ID: "Main", IsExpanded: false, IsSelected: false, Elements: [] };
            var strNavParam = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;

            if (strNavParam) {
                arrNavParams = strNavParam.split(';');
            }
            if ($scope.formobject) {
                for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                    if ($scope.formobject.Elements[i].Name == "items") {
                        objItems = $scope.formobject.Elements[i];
                        break;
                    }
                }
            }

            if (objItems) {
                $scope.PopulateAvailableFieldsFormLink(arrNavParams, objItems, mainItem);
                if (mainItem.Elements.length > 0) {
                    $scope.FieldCollection.push(mainItem);
                }
            }
            $scope.isFormLink = true;
        }
        else {
            $scope.PopulateAvailableFields();
            $scope.isFormLink = false;
        }
    };

    //#region Common Methods
    $scope.PopulateAvailableFields = function () {
        $scope.FieldCollection = [];
        var arrNavParams;
        var mainItem = { ID: "Main", IsExpanded: false, IsSelected: false, Elements: [] };
        var strNavParam = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
        if (strNavParam) {
            arrNavParams = strNavParam.split(';');
        }
        if ($scope.SelectedObject.IsShowAllControl == true) {
            var table;
            for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                if ($scope.formobject.Elements[i].Name == "sfwTable") {
                    table = $scope.formobject.Elements[i];
                    break;
                }
            }

            $scope.PopulateControls(table, mainItem, arrNavParams);
        }
        else {
            if ($scope.currentTable) {
                $scope.PopulateControls($scope.currentTable, mainItem, arrNavParams);
            }
        }
        if (mainItem.Elements.length > 0) {
            $scope.FieldCollection.push(mainItem);
        }

    };

    $scope.PopulateControls = function (sfxTable, mainItem, arrNavParams) {
        var strTreeCaption = "";
        function iterateWizardSteps(sfxWizardStep) {
            strTreeCaption = sfxWizardStep.dictAttributes.Title;
            if (strTreeCaption == "" || strTreeCaption == undefined)
                strTreeCaption = sfxWizardStep.dictAttributes.ID;
            var tnChild = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
            if (sfxWizardStep.Elements.length > 0) {
                $scope.PopulateControls(sfxWizardStep.Elements[0], tnChild, arrNavParams);
                if (tnChild.Elements.length > 0) {
                    mainItem.Elements.push(tnChild);
                    mainItem.IsExpanded = true;
                }
            }
        }
        function iterateWizardItems(objWizard) {
            if (objWizard.Name == "HeaderTemplate") {
                strTreeCaption = "HeaderTemplate";
                var tnChild = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
                if (objWizard.Elements.length > 0) {
                    $scope.PopulateControls(objWizard.Elements[0], tnChild, arrNavParams);
                }
                if (tnChild.Elements.length > 0) {
                    mainItem.Elements.push(tnChild);
                    mainItem.IsExpanded = true;
                }
            }
            else {

                angular.forEach(objWizard.Elements, iterateWizardSteps);
            }
        }
        function iterateTabSheet(sfxTabSheet) {
            strTreeCaption = sfxTabSheet.dictAttributes.HeaderText;
            if (strTreeCaption == undefined || strTreeCaption == "")
                strTreeCaption = sfxTabSheet.dictAttributes.ID;
            var tnChild = { ID: strTreeCaption, IsExpanded: false, IsSelected: false, Elements: [] };

            if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "sfwTable") {
                $scope.PopulateControls(sfxTabSheet.Elements[0], tnChild, arrNavParams);
                tnChild.IsExpanded = true;
            }
            if (tnChild.Elements.length > 0) {
                mainItem.Elements.push(tnChild);
                mainItem.IsExpanded = true;
            }
        }
        function iterateCell(sfxCtrl) {

            if (sfxCtrl.Name == "sfwPanel") {
                strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                if (strTreeCaption == undefined || strTreeCaption == "")
                    strTreeCaption = sfxCtrl.dictAttributes.ID;
                var tnChild = { ID: strTreeCaption, IsExpanded: false, IsSelected: false, Elements: [] };
                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "sfwTable") {
                    $scope.PopulateControls(sfxCtrl.Elements[0], tnChild, arrNavParams);
                    tnChild.IsExpanded = true;
                }
                if (tnChild.Elements.length > 0) {
                    mainItem.Elements.push(tnChild);
                    mainItem.IsExpanded = true;
                }
            }
            else if (sfxCtrl.Name == "sfwTabContainer") {
                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Tabs") {
                    var sfxTabs = sfxCtrl.Elements[0];

                    angular.forEach(sfxTabs.Elements, iterateTabSheet);
                }
            }
            else if (sfxCtrl.Name == "sfwWizard") {

                angular.forEach(sfxCtrl.Elements, iterateWizardItems);

            }
            else if (sfxCtrl.Name == "sfwTextBox" || sfxCtrl.Name == "sfwDropDownList" || sfxCtrl.Name == "sfwCascadingDropDownList") {
                var strId = sfxCtrl.dictAttributes.ID;
                if (strId != undefined && strId != "") {
                    var tnControl = { ID: strId, IsExpanded: false, IsSelected: false, IsCheckBoxVisible: true, Elements: [] };
                    if (arrNavParams) {
                        angular.forEach(arrNavParams, function (str) {
                            if (str == strId) {
                                tnControl.IsSelected = true;
                            }
                        });
                    }
                    mainItem.Elements.push(tnControl);
                }
            }
        }

        function iterateTable(sfxRow) {
            for (var iCol = 0; iCol < sfxRow.Elements.length; iCol++) {
                var sfxCell = sfxRow.Elements[iCol];
                if (sfxCell) {
                    angular.forEach(sfxCell.Elements, iterateCell);
                }
            }
        }
        if (sfxTable) {

            angular.forEach(sfxTable.Elements, iterateTable);
        }
    };

    //#endregion

    //#region Common Events
    $scope.showAllControlChange = function (event) {
        $scope.PopulateAvailableFields();
    };

    $scope.ExpandCollapsedControl = function (field, event) {
        field.IsExpanded = !field.IsExpanded;
    };

    $scope.SelectFieldClick = function (field, event) {
        $scope.SelectedField = field;
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.onOkClick = function () {

        var strRetriveParems = $scope.RetrieveNavParams();
        $rootScope.UndRedoBulkOp("Start");
        if ($scope.SelectedObject.dictAttributes.sfwNavigationParameter) {
            var lstParam = $scope.SelectedObject.dictAttributes.sfwNavigationParameter.split(';');
            var lstNewParam = strRetriveParems.split(";");
            if (lstParam && lstParam.length > 0) {
                for (j = 0; j < lstParam.length; j++) {
                    if (!lstNewParam.some(function (itm) { return itm == lstParam[j]; })) {
                        var model = FindControlByID($scope.formobject, lstParam[j]);
                        if (model) {
                            $scope.CheckOtherFilterGridButton(model);
                            //if (!model.IsShowDataField) {
                            //    $rootScope.EditPropertyValue(model.dictAttributes.sfwDataField, model.dictAttributes, "sfwDataField", "");
                            //    model.IsShowDataField = false;
                            //}
                        }
                    }
                }
            }

            if (lstNewParam && lstNewParam.length > 0) {
                for (j = 0; j < lstNewParam.length; j++) {
                    if (!lstParam.some(function (itm) { return itm == lstNewParam[j]; })) {
                        var model = FindControlByID($scope.formobject, lstNewParam[j]);
                        if (model) {
                            $scope.CheckOtherFilterGridButton(model);
                            if (!model.IsShowDataField) {
                                $rootScope.EditPropertyValue(model.dictAttributes.sfwEntityField, model.dictAttributes, "sfwEntityField", "");
                                $ValidationService.checkValidListValue([], model, undefined, 'sfwEntityField', "sfwEntityField", "", undefined);

                                model.IsShowDataField = true;
                            }
                        }
                    }
                }
            }
        }

        $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strRetriveParems);
        $rootScope.UndRedoBulkOp("End");
        //$scope.SelectedObject.dictAttributes.sfwNavigationParameter = strRetriveParems;
        $scope.onCancelClick();
    };

    $scope.CheckOtherFilterGridButton = function (model) {
        //$rootScope.EditPropertyValue(model.IsShowDataField, model, "IsShowDataField", false);
        var lst = [];
        FindControlListByNames($scope.formobject, ['sfwButton', 'sfwLinkButton', 'sfwImageButton'], lst);
        if (lst && lst.length > 0) {
            lst = lst.filter(function (itm) { return itm.dictAttributes.sfwMethodName == "btnGridSearch_Click"; });
            if (lst && lst.length > 0) {
                for (var i = 0; i < lst.length; i++) {
                    if (lst[i] != $scope.SelectedObject && lst[i].dictAttributes.sfwNavigationParameter) {
                        var lstParam = lst[i].dictAttributes.sfwNavigationParameter.split(';');
                        if (lstParam && lstParam.length > 0) {
                            for (j = 0; j < lstParam.length; j++) {
                                if (model && model.dictAttributes && model.dictAttributes.ID == lstParam[j]) {
                                    //model.IsShowDataField = true;
                                    $rootScope.EditPropertyValue(model.IsShowDataField, model, "IsShowDataField", true);
                                }
                            }
                        }
                    }
                }
            }
        }
    };

    $scope.RetrieveNavParams = function () {
        var strNavParam = "";
        function iterator(tn) {
            if (tn.Elements.length > 0)
                strNavParam += $scope.RetrieveNavParamsForChild(tn);
        }
        angular.forEach($scope.FieldCollection, iterator);

        strNavParam = strNavParam.trim();
        if (strNavParam.length > 0) {
            if (strNavParam[strNavParam.length - 1] == ';')
                strNavParam = strNavParam.substring(0, strNavParam.length - 1);
        }
        return strNavParam;
    };

    $scope.RetrieveNavParamsForChild = function (tnThis) {
        var strNavParam = "";
        function iterator(tn) {
            if (tn.Elements.length > 0) {
                strNavParam += $scope.RetrieveNavParamsForChild(tn);
            }
            else if ((tn.IsSelected)) {
                strNavParam += tn.ID + ";";
            }
        }
        angular.forEach(tnThis.Elements, iterator);

        return strNavParam;
    };


    $scope.onCancelClick = function () {
        if ($scope.dialog) {
            $scope.dialog.close();
        }
    };

    //#endregion


    //#region Form Link
    $scope.PopulateAvailableFieldsFormLink = function (arrNavParams, objItems, tnNode) {
        var strTreeCaption = "";
        function iterateWizardSteps(sfxWizardStep) {
            strTreeCaption = sfxWizardStep.dictAttributes.Title;
            if (strTreeCaption == "" || strTreeCaption == undefined)
                strTreeCaption = sfxWizardStep.dictAttributes.ID;
            var tnChild = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
            if (sfxWizardStep.Elements.length > 0) {
                $scope.PopulateControls(sfxWizardStep.Elements[0], tnChild, arrNavParams);
                if (tnChild.Elements.length > 0) {
                    tnNode.Elements.push(tnChild);
                    tnNode.IsExpanded = true;
                }
            }
        }
        function iterateWizardItems(objWizard) {
            if (objWizard.Name == "HeaderTemplate") {
                strTreeCaption = "HeaderTemplate";
                var tnChild = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
                if (objWizard.Elements.length > 0) {
                    $scope.PopulateControls(objWizard.Elements[0], tnChild, arrNavParams);
                }
                if (tnChild.Elements.length > 0) {
                    tnNode.Elements.push(tnChild);
                    tnNode.IsExpanded = true;
                }
            }
            else {
                angular.forEach(objWizard.Elements, iterateWizardSteps);
            }
        }

        function iterateTabSheet(sfxTabSheet) {
            strTreeCaption = sfxTabSheet.dictAttributes.HeaderText;
            if (strTreeCaption == undefined || strTreeCaption == "")
                strTreeCaption = sfxTabSheet.dictAttributes.ID;
            var tnChild = { ID: strTreeCaption, IsExpanded: false, IsSelected: false, Elements: [] };
            if (sfxTabSheet.Elements.length > 0) {
                $scope.PopulateAvailableFieldsFormLink(arrNavParams, sfxTabSheet.Elements[0], tnChild);
                tnChild.IsExpanded = true;
            }
            if (tnChild.Elements.length > 0) {
                tnNode.Elements.push(tnChild);
                tnNode.IsExpanded = true;
            }
        }

        function iterateItems(sfxCtrl) {
            if (sfxCtrl.Name == "items") {
                $scope.PopulateAvailableFieldsFormLink(arrNavParams, sfxCtrl, tnNode);
            }
            else if (sfxCtrl.Name == "sfwPanel" || sfxCtrl.Name == "sfwDialogPanel") {
                strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                if (strTreeCaption == undefined || strTreeCaption == "")
                    strTreeCaption = sfxCtrl.dictAttributes.ID;

                var tnChild = { ID: strTreeCaption, IsExpanded: false, IsSelected: false, Elements: [] };
                if (sfxCtrl.Elements.length > 0) {
                    $scope.PopulateAvailableFieldsFormLink(arrNavParams, sfxCtrl.Elements[0], tnChild);
                    tnChild.IsExpanded = true;
                }
                if (tnChild.Elements.length > 0) {
                    tnNode.Elements.push(tnChild);
                    tnNode.IsExpanded = true;
                }
            }
            else if (sfxCtrl.Name == "sfwTabContainer") {
                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Tabs") {
                    var sfxTabs = sfxCtrl.Elements[0];

                    angular.forEach(sfxTabs.Elements, iterateTabSheet);
                }
            }
            else if (sfxCtrl.Name == "sfwWizard") {

                angular.forEach(sfxCtrl.Elements, iterateWizardItems);

            }
            else if (sfxCtrl.Name == "sfwTextBox" || sfxCtrl.Name == "sfwDropDownList") {
                var strId = sfxCtrl.dictAttributes.ID;
                if (strId != undefined && strId != "") {
                    var tnControl = { ID: strId, IsExpanded: false, IsSelected: false, IsCheckBoxVisible: true, Elements: [] };
                    if (arrNavParams) {
                        angular.forEach(arrNavParams, function (str) {
                            if (str == strId) {
                                tnControl.IsSelected = true;
                            }
                        });
                    }
                    tnNode.Elements.push(tnControl);
                }
            }
        }

        angular.forEach(objItems.Elements, iterateItems);
    };
    //#endregion

    //#region Call Init Methods
    $scope.Init();
    //#endregion

}]);