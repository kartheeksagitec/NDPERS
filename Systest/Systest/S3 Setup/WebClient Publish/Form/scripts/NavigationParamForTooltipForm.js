app.controller("TooltipNavigationParameterController", ["$scope", "$rootScope", "$filter", "$EntityIntellisenseFactory", function ($scope, $rootScope, $filter, $EntityIntellisenseFactory) {

    $scope.SelectedObject.IsShowAllControl = false;
    $scope.currentPanel;
    $scope.ParameterCollection = [];
    $scope.MethodParameterCollection = [];
    $scope.XmlParameterCollection = [];
    $scope.EntityFieldCollection = [];
    $scope.thisQuery;
    $scope.TargetFormCaption = "Target Form :";
    $scope.TargetForm = "";

    //#region Init Methods
    $scope.Init = function () {
        var curscope = getCurrentFileScope();
        if (curscope.CurrPanel) {
            $scope.currentPanel = curscope.CurrPanel;
        }
        else if (curscope.MainTable) {
            $scope.currentPanel = curscope.MainTable;
        }
        else if (curscope.objQueryForm) {
            var lstTable = curscope.objQueryForm.Elements.filter(function (x) { return x.Name == "sfwTable"; });
            if (lstTable && lstTable.length > 0) {
                $scope.currentPanel = lstTable[0];
            }
        }

        $scope.ParameterCollection = [];

        if ($scope.SelectedObject.IsChildOfGrid) {
            $scope.TargetForm = $scope.SelectedObject.dictAttributes.sfwActiveForm;
        }
        //Active Form

        if ($scope.TargetForm) {
            $scope.TargetFormCaption = "Target Form :";
            $.connection.hubMain.server.getSingleFileDetail($scope.TargetForm).done(function (filedata) {
                $scope.receiveSingleFileDetail(filedata);
            });
        }

        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
            $scope.$apply(function () {
                $scope.objGlobleParameters = data;
                $scope.PopulateGlobalParameters();
            });
        });
        if ($scope.formobject && $scope.formobject.dictAttributes) {
            if ($scope.formobject.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.formobject.dictAttributes.sfwType == "FormLinkLookup" || $scope.formobject.dictAttributes.sfwType == "FormLinkWizard") {
                $scope.PopulateAvailableFieldsForFormLink(undefined);
            }
            else {
                $scope.PopulateAvailableFields();
            }
        }
    };
    $scope.receiveSingleFileDetail = function (data) {
        $scope.sigleFileDetail = data;
        $scope.$apply(function () {
            $scope.newFormModel = data;
            if ($scope.newFormModel) {
                var methodName;
                var lstInitialLoad = $scope.newFormModel.Elements.filter(function (itm) { return itm.Name == "initialload"; });
                if (lstInitialLoad && lstInitialLoad.length > 0) {
                    var objInitialLoad = lstInitialLoad[0];
                    var lstMethod = objInitialLoad.Elements.filter(function (itm) { return itm.Name == "callmethods"; });
                    if (lstMethod && lstMethod.length > 0) {
                        methodName = lstMethod[0].dictAttributes.sfwMethodName;
                    }
                }
                if (methodName) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var EntityObject = entityIntellisenseList.filter(function (x) { return x.ID == $scope.newFormModel.dictAttributes.sfwEntity; })[0];
                    var vrParCollection = [];
                    var strParamField = "";
                    var strParamValue = "";
                    var blnConstant = false;
                    if (EntityObject.XmlMethods && EntityObject.XmlMethods.length > 0 && methodName) {

                        for (i = 0; i < EntityObject.XmlMethods.length; i++) {
                            if (EntityObject.XmlMethods[i].ID == methodName) {
                                for (j = 0; j < EntityObject.XmlMethods[i].Parameters.length; j++) {
                                    var objParameter = { ParameterField: EntityObject.XmlMethods[i].Parameters[j].ID, ParameterValue: EntityObject.XmlMethods[i].Parameters[j].Value, Constants: false };
                                    $scope.XmlParameterCollection.push(objParameter);
                                }
                            }
                        }
                    }
                    $scope.PopulateParamValues($scope.XmlParameterCollection);
                }
            }
        });

        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });

    };

    $scope.PopulateGlobalParameters = function () {
        function iAddInobjGlobalParam(itm) {
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
        if ($scope.objGlobleParameters && $scope.objGlobleParameters.Elements.length > 0) {
            var objGlobalParam = { ID: "Global Parameters", IsExpanded: false, IsSelected: false, Elements: [] };
            var globalParameters = [];

            angular.forEach($scope.objGlobleParameters.Elements, iAddInobjGlobalParam);
            if (objGlobalParam.Elements.length > 0) {
                $scope.EntityFieldCollection.push(objGlobalParam);
            }
            console.log("PopulateGlobalParameters ", $scope.EntityFieldCollection);
        }
    };
    $scope.PopulateParamValues = function (ParameterCollection) {
        var istrParameters = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
        if (istrParameters) {
            var alParams = istrParameters.split(';');
            angular.forEach(alParams, function (strParam) {
                var strParamField = strParam;
                var strParamsValue = strParam;
                var blnConstant = false;
                if (strParam.contains("=")) {
                    strParamField = strParam.substring(0, strParam.indexOf('='));
                    strParamsValue = strParam.substring(strParam.indexOf('=') + 1);
                    if (strParamsValue.match("^#")) {
                        strParamsValue = strParamsValue.substring(1);
                        blnConstant = true;
                    }
                }
                angular.forEach(ParameterCollection, function (objParameter) {
                    if (objParameter.ParameterField && strParamField && objParameter.ParameterField.toLowerCase() == strParamField.toLowerCase()) {
                        objParameter.ParameterValue = strParamsValue;
                        objParameter.Constants = blnConstant;
                    }
                });
            });
        }
    };


    //#endregion

    //#region Populate Entity Fields In Left section
    $scope.PopulateAvailableFields = function () {
        $scope.EntityFieldCollection = [];
        var mainItem = { ID: "Main", IsExpanded: false, IsSelected: false, Elements: [] };
        $scope.IsLookup = $scope.formobject.dictAttributes.sfwType == 'Lookup' ? true : false;
        $scope.blnIsReport = $scope.formobject.dictAttributes.sfwType == 'Report' ? true : false;
        $scope.blnIsCorr = $scope.formobject.dictAttributes.sfwType == 'Correspondence' ? true : false;
        var strProperty;
        if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click") {
            strProperty = "ID";
        }
        else if ($scope.IsLookup)
            strProperty = "sfwDataField";
        else if ($scope.blnIsReport || $scope.blnIsCorr) {
            strProperty = "ID";
        }
        else
            strProperty = "sfwEntityField";

        if ($scope.SelectedObject.Name == "sfwOpenDetail") {
            if ($scope.SelectedObject.dictAttributes.sfwQueryID) {
                var lst = $scope.SelectedObject.dictAttributes.sfwQueryID.split('.');
                if (lst && lst.length == 2) {
                    var entityName = lst[0];
                    var strQueryID = lst[1];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var lstEntity = $filter('filter')(entityIntellisenseList, { ID: entityName }, true);
                    if (lstEntity && lstEntity.length > 0) {
                        var objEntity = lstEntity[0];
                        var lstQuery = objEntity.Queries.filter(function (x) { return x.ID == strQueryID; });
                        if (lstQuery && lstQuery.length > 0) {
                            var objQuery = lstQuery[0];
                            $.connection.hubForm.server.getEntityQueryColumns($scope.SelectedObject.dictAttributes.sfwQueryID, "TooltipNavigationParameterController").done(function (data) {
                                $scope.receiveQueryColumns(data);
                            });
                        }
                    }
                }
            }
        }
        else {
            if ($scope.SelectedObject.IsShowAllControl == true) {

                var table;
                for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                    if ($scope.formobject.Elements[i].Name == "sfwTable") {
                        table = $scope.formobject.Elements[i];
                        break;
                    }
                }

                $scope.PopulateControls(table, mainItem, strProperty);
            }
            else {
                if ($scope.blnIsReport || $scope.blnIsCorr) {
                    $scope.PopulateControls($scope.currentPanel, mainItem, strProperty);
                }
                else {
                    $scope.PopulateControls($scope.currentPanel.Elements[0], mainItem, strProperty);
                }
            }
            $scope.EntityFieldCollection.push(mainItem);
        }

    };

    $scope.receiveQueryColumns = function (data) {
        $scope.$evalAsync(function () {
            var mainItem = { ID: $scope.SelectedObject.dictAttributes.sfwQueryID, IsSelected: false, Elements: [] };
            for (var i = 0; i < data.length; i++) {
                var tnChild = { ID: data[i].CodeID, IsSelected: false, Elements: [] };
                mainItem.Elements.push(tnChild);
            }
            $scope.EntityFieldCollection.push(mainItem);
        });
    };

    $scope.PopulateControls = function (asfxTable, tnNode, strProperty) {
        var strTreeCaption = "";

        if (asfxTable) {
            angular.forEach(asfxTable.Elements, function (sfxRow) {
                for (var iCol = 0; iCol < sfxRow.Elements.length; iCol++) {
                    var sfxCell = sfxRow.Elements[iCol];
                    if (sfxCell) {
                        angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                            if (sfxCtrl.Name == "sfwPanel") {

                                strTreeCaption = sfxCtrl.dictAttributes.ID;
                                if (strTreeCaption == "" || strTreeCaption == undefined)
                                    strTreeCaption = sfxCtrl.Name;
                                var tnChild = { ID: strTreeCaption, IsSelected: false, Elements: [] };

                                tnChild.IsExpanded = false;

                                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "sfwTable") {
                                    $scope.PopulateControls(sfxCtrl.Elements[0], tnChild, strProperty);
                                }
                                tnNode.Elements.push(tnChild);
                            }
                            else if (sfxCtrl.Name == "sfwTabContainer") {
                                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Tabs") {
                                    var sfxTabs = sfxCtrl.Elements[0];
                                    angular.forEach(sfxTabs.Elements, function (sfxTabSheet) {
                                        strTreeCaption = sfxTabSheet.dictAttributes.HeaderText;
                                        if (strTreeCaption == "")
                                            strTreeCaption = sfxTabSheet.dictAttributes.ID;
                                        var tnChild = { ID: strTreeCaption, IsSelected: false, Elements: [] };

                                        if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "sfwTable") {
                                            $scope.PopulateControls(sfxTabSheet.Elements[0], tnChild, strProperty);
                                        }
                                        if (tnChild.Elements.length > 0) {
                                            tnNode.Elements.push(tnChild);
                                        }

                                    });
                                }
                            }
                            else if (sfxCtrl.Name == "sfwWizard") {
                                angular.forEach(sfxCtrl.Elements, function (objWizard) {
                                    if (objWizard.Name == "HeaderTemplate") {
                                        strTreeCaption = "HeaderTemplate";
                                        var tnChild = { ID: strTreeCaption, IsSelected: false, Elements: [] };
                                        if (objWizard.Elements.length > 0 && objWizard.Elements[0].Name == "sfwTable") {
                                            $scope.PopulateControls(objWizard.Elements[0], tnChild, strProperty);
                                        }
                                        if (tnChild.Elements.length > 0) {
                                            tnNode.Elements.push(tnChild);
                                        }
                                    }
                                    else {
                                        angular.forEach(objWizard.Elements, function (sfxWizardStep) {
                                            strTreeCaption = sfxWizardStep.dictAttributes.Title;
                                            if (strTreeCaption == "")
                                                strTreeCaption = sfxWizardStep.dictAttributes.ID;
                                            var tnChild = { ID: strTreeCaption, IsSelected: false, Elements: [] };
                                            if (sfxWizardStep.Elements.length > 0 && sfxWizardStep.Elements[0].Name == "sfwTable") {
                                                $scope.PopulateControls(sfxWizardStep.Elements[0], tnChild, strProperty);
                                                if (tnChild.Elements.length > 0) {
                                                    tnNode.Elements.push(tnChild);
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                            else if (sfxCtrl.Elements.length > 0 && sfxCtrl.Name == "sfwGridView") {

                                var strGridId = sfxCtrl.dictAttributes.ID;

                                var objControl = { ID: strGridId + " (Data Keys)", IsSelected: false, Elements: [] };
                                tnNode.Elements.push(objControl);

                                if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                                    for (var j = 0; j < sfxCtrl.Elements[0].Elements.length; j++) {
                                        var objTempField = sfxCtrl.Elements[0].Elements[j];
                                        if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                            var objItemTempField = objTempField.Elements[0];
                                            angular.forEach(objItemTempField.Elements, function (sfxControl) {
                                                if ("sfwEntityField" in sfxControl.dictAttributes) {
                                                    var strFieldName = "";

                                                    strFieldName = sfxControl.dictAttributes.sfwEntityField;
                                                    if (strFieldName != "" && strFieldName != undefined) {
                                                        objControl.Elements.push({ ID: strFieldName, IsSelected: false, Elements: [] });
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                            else {
                                if (strProperty != undefined && strProperty != "") {
                                    if (!$scope.blnIsReport && !$scope.blnIsCorr) {
                                        if ($scope.IsLookup && !IsCriteriaField(sfxCtrl)) {
                                            strProperty = 'sfwEntityField';
                                        }
                                        if ("sfwDataField" in sfxCtrl.dictAttributes) {
                                            strTreeCaption = sfxCtrl.dictAttributes[strProperty];
                                        }

                                        else if ("sfwEntityField" in sfxCtrl.dictAttributes) {
                                            strTreeCaption = sfxCtrl.dictAttributes[strProperty];
                                        }
                                    }
                                    else {
                                        strTreeCaption = sfxCtrl.dictAttributes[strProperty];
                                    }
                                }
                                else {
                                    strTreeCaption = sfxCtrl.dictAttributes.ID;
                                }
                                if (strTreeCaption != "" && strTreeCaption != undefined) {
                                    //strTreeCaption = sfxCtrl.Name;
                                    if (!tnNode.Elements.some(function (itm) { return itm.ID == strTreeCaption; })) {
                                        var tnControl = { ID: strTreeCaption, Elements: [] };
                                        tnNode.Elements.push(tnControl);
                                    }
                                }
                            }

                        });
                    }
                }
            });
        }
    };

    //#endregion


    //#region Form Link
    $scope.PopulateAvailableFieldsForFormLink = function (strPropID) {
        $scope.EntityFieldCollection = [];
        if ($scope.formobject) {
            var arrIDs = [];
            var istrValue = $scope.formobject.dictAttributes.sfwType;
            var blnIsLookup = istrValue.toUpperCase().trim() == "FORMLINKLOOKUP";

            $scope.SourceForm = $scope.formobject.dictAttributes.ID;

            var strProperty;
            if ($scope.IsFormCodeGroup || ($scope.SelectedObject.dictAttributes.sfwMethodName && $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click") || $scope.SelectedObject.Name == "sfwCascadingDropDownList" || $scope.SelectedObject.Name == "sfwSeries") {
                strProperty = "ID";
            }
            else if (blnIsLookup)
                strProperty = "sfwDataField";
            else
                strProperty = "sfwEntityField";

            var objControl = { ID: "Main", IsExpanded: false, IsSelected: false, Elements: [] };

            var objItems = $scope.GetItemsModel();
            if (objItems) {
                if (strPropID != undefined && strPropID != "") {
                    $scope.PopulateChildIDsFormLink(strPropID, objItems, arrIDs);
                }
                $scope.PopulateAvailableFieldsFormLink(strProperty, objItems, objControl.Elements, arrIDs, strPropID);
            }

            if (objControl.Elements.length > 0)
                $scope.EntityFieldCollection.push(objControl);
        }
    };


    $scope.GetItemsModel = function () {
        var retVal;

        retVal = $scope.formobject.Elements.filter(function (item) {
            return item.Name == "items";
        });

        if (retVal && retVal.length > 0) {
            return retVal[0];
        }
        return retVal;
    };

    $scope.PopulateChildIDsFormLink = function (strPropID, sfxTable, arrID) {
        function iterator(sfxCtrl) {
            if (sfxCtrl._dictAttributes.hasOwnProperty("sfwParentControl")) {
                var strID = sfxCtrl.dictAttributes.ID;
                var strParentID = sfxCtrl.dictAttributes.sfwParentControl;
                if (strParentID == strPropID) {
                    arrID.push(strID);
                    PopulateChildIDsFormLink(strID, sfxCtrl, arrID);
                }
            }
        }
        if (sfxTable) {

            angular.foreach(sfxTable.Elements, iterator);
        }
    };

    $scope.PopulateAvailableFieldsFormLink = function (strProperty, sfxTable, list, arrIDs, strPropID) {
        var i = 0;
        function iteratorarrIDs(strID) {
            if (strID == strCtrlID) {
                blnAdd = false;
                return;
            }
        }

        function iterateWizardSteps(sfxWizardStep) {
            strTreeCaption = sfxWizardStep.dictAttributes.Title;
            if (strTreeCaption == "" || strTreeCaption == undefined)
                strTreeCaption = sfxWizardStep.dictAttributes.ID;
            list.push({ ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false });
            if (sfxWizardStep.Elements.length > 0) {
                $scope.PopulateAvailableFieldsFormLink(strProperty, sfxWizardStep.Elements[0], list[i].Elements, arrIDs, strPropID);
                if (list[i].Elements.length == 0) {
                    list.splice(i, 1);
                }
                else {
                    i++;
                }
            }
        }
        function iterateWizardItems(objWizard) {
            if (objWizard.Name == "HeaderTemplate") {
                strTreeCaption = "HeaderTemplate";
                list.push({ ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false });
                if (objWizard.Elements.length > 0) {
                    $scope.PopulateAvailableFieldsFormLink(strProperty, objWizard.Elements[0], list[i].Elements, arrIDs, strPropID);
                }
                if (list[i].Elements.length == 0) {
                    list.splice(i, 1);
                }
                else {
                    i++;
                }

            }
            else {
                angular.forEach(objWizard.Elements, iterateWizardSteps);
            }
        }
        function iterateTabSheet(sfxTabSheet) {
            strTreeCaption = sfxTabSheet.dictAttributes.HeaderText;
            if (strTreeCaption == "" || strTreeCaption == undefined)
                strTreeCaption = sfxTabSheet.dictAttributes.ID;
            list.push({ ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false });
            if (sfxTabSheet.Elements.length > 0) {
                $scope.PopulateAvailableFieldsFormLink(strProperty, sfxTabSheet.Elements[0], list[i].Elements, arrIDs, strPropID);
            }
            if (list[i].Elements.length == 0) {
                list.splice(i, 1);
            }
            else {
                i++;
            }
        }


        function iterator(sfxCtrl) {
            {
                var strTreeCaption = "";

                if (sfxCtrl.Name == "items") {
                    $scope.PopulateAvailableFieldsFormLink(strProperty, sfxCtrl, list[i].Elements, arrIDs, strPropID);
                }
                else if (sfxCtrl.Name == "sfwPanel" || sfxCtrl.Name == "sfwDialogPanel") {
                    strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                    if (strTreeCaption == "" || strTreeCaption == undefined)
                        strTreeCaption = sfxCtrl.dictAttributes.ID;
                    list.push({ ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false });
                    if (sfxCtrl.Elements.length > 0) {
                        $scope.PopulateAvailableFieldsFormLink(strProperty, sfxCtrl.Elements[0], list[i].Elements, arrIDs, strPropID);
                    }
                    if (list[i].Elements.length == 0) {
                        list.splice(i, 1);
                    }
                    else {
                        i++;
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
                else if (sfxCtrl.Name == "sfwGridView" && ($scope.SelectedObject.Name != "sfwTextBox")) {
                    var strGridId = sfxCtrl.dictAttributes.ID;

                    var objControl = { ID: strGridId + " (Data Keys)", Elements: [], IsSelected: false, IsExpanded: false };
                    list.push(objControl);

                    var strDataKeys = sfxCtrl.dictAttributes.sfwDataKeyNames;
                    if (strDataKeys)
                        var strDataKeyNames = strDataKeys.split(",");

                    if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                        for (var j = 0; j < sfxCtrl.Elements[0].Elements.length; j++) {
                            var objTempField = sfxCtrl.Elements[0].Elements[j];
                            if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                var objItemTempField = objTempField.Elements[0];
                                angular.forEach(objItemTempField.Elements, function (sfxControl) {
                                    if ("sfwEntityField" in sfxControl.dictAttributes) {
                                        var strFieldName = "";
                                        if (strPropID != "" && strPropID != undefined) {
                                            strFieldName = sfxCtrl.dictAttributes.ID;
                                        }
                                        else {
                                            strFieldName = sfxControl.dictAttributes.sfwEntityField;
                                        }
                                        //strFieldName = sfxControl.dictAttributes.sfwDataField;
                                        if (strFieldName != undefined && strFieldName != "") {
                                            objControl.Elements.push({ ID: strFieldName, Elements: [], IsSelected: false, IsExpanded: false });
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
                else if (strProperty in sfxCtrl.dictAttributes) {
                    var blnAdd = true;
                    var strCtrlID = sfxCtrl.dictAttributes.ID;

                    angular.forEach(arrIDs, iteratorarrIDs);
                    if (blnAdd) {
                        var strFieldName = "";
                        if (strPropID != undefined && strPropID != "") {
                            strFieldName = sfxCtrl.dictAttributes.ID;
                        }
                        else {
                            strFieldName = sfxCtrl.dictAttributes[strProperty];
                        }
                        if (strFieldName != undefined && strFieldName != "" && sfxCtrl.Name != "sfwButton") {
                            list.push({ ID: strFieldName, Elements: [], IsSelected: false, IsExpanded: false });
                            i++;
                        }
                    }
                }
            }
        }

        if (sfxTable != null) {
            angular.forEach(sfxTable.Elements, iterator);
        }
    };

    //#endregion

    //#region Populate Lookup Panel Criteria When Selected ActiveFormType is Lookup, In Approve Button
    var vrLookupField = [];
    $scope.PanelCriteria;

    $scope.XmlParameterCollection = [];

    //#endregion

    //#region Common Functionality
    $scope.showAllControlChange = function (event) {
        $scope.PopulateAvailableFields();
        $scope.PopulateGlobalParameters();
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

    $scope.SetFieldClass = function (obj) {
        if (obj == $scope.SelectedField) {
            return "selected";
        }
    };
    //#endregion

    //#region When dialog close on Ok and Cancel button
    $scope.strSelectedParameters;
    $scope.onOkClick = function () {
        var strCustomAttribute = "";
        if ($scope.XmlParameterCollection.length > 0) {
            strCustomAttribute = $scope.GetSavedString($scope.XmlParameterCollection);
        }

        if (strCustomAttribute != undefined) {
            $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute);
        }
        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        $scope.NavigationParameterDialog.close();
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

    $scope.Init();
}]);
