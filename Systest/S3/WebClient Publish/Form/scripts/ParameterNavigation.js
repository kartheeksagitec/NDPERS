app.controller("NavigationParameterController", ["$scope", "$rootScope", "ngDialog", "$filter", "ParameterFactory", "$EntityIntellisenseFactory", "$Entityintellisenseservice", "$GetEntityFieldObjectService", function ($scope, $rootScope, ngDialog, $filter, ParameterFactory, $EntityIntellisenseFactory, $Entityintellisenseservice, $GetEntityFieldObjectService) {

    $scope.SelectedObject.IsShowAllControl = false;
    $scope.currentPanel;
    $scope.ParameterCollection = [];
    $scope.MethodParameterCollection = [];
    $scope.LookupParameterCollection = [];
    $scope.XmlParameterCollection = [];
    $scope.thisQuery;
    $scope.activeFormEntity;
    $scope.TargetFormCaption = "Target Form :";
    $scope.TargetForm = "";
    $scope.IsParentAsCollection = false;
    if ($scope.ParentModel && $scope.ParentModel.dictAttributes.sfwEntityField) {
        $scope.IsParentAsCollection = true;
        var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, $scope.ParentModel.dictAttributes.sfwEntityField);
        if (entObject) {
            $scope.entityName = entObject.Entity;
        }

    }
    if ($scope.formobject && $scope.formobject.dictAttributes.sfwEntity && $scope.formobject.dictAttributes.sfwType == 'Correspondence') {
        $scope.entityName = $scope.formobject.dictAttributes.sfwEntity;
    }
    //#region Init Methods
    $scope.Init = function () {
        var curscope = getCurrentFileScope();
        if ($scope.SelectedObject.dictAttributes.sfwMethodName === "btnNew_Click") {
            if ($scope.formobject) {
                var larrPanels = getDescendents($scope.formobject, "sfwPanel");
                if (larrPanels && larrPanels.length > 0) {
                    $scope.currentPanel = larrPanels.filter(function (pnl) {return pnl.dictAttributes.ID === "pnlMain" || pnl.dictAttributes.ID === "pnlCriteria" })[0];
                }
            }
        }

        if (!$scope.currentPanel) {
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
            if (curscope && curscope.currentfile && curscope.currentfile.FileType == "UserControl") {
                $scope.currentPanel = curscope.FormModel;
            }
        }

        $scope.ParameterCollection = [];

        $scope.TargetForm = $scope.SelectedObject.dictAttributes.sfwActiveForm;
        //Active Form

        if ($scope.SelectedObject.Name == "sfwCascadingDropDownList" || $scope.SelectedObject.Name == "sfwDropDownList" || $scope.SelectedObject.Name == "sfwCheckBoxList" || $scope.SelectedObject.Name == "sfwRadioButtonList" || $scope.SelectedObject.Name == "sfwSourceList" || $scope.SelectedObject.Name == "sfwListPicker" || $scope.SelectedObject.Name == "sfwMultiSelectDropDownList" || $scope.SelectedObject.Name == "sfwListBox") {
            $scope.PopulateNavigationParameters();
            $scope.EntityFieldCollection = [];
        }
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnSave_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNoChangesSave_Click"
            || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnSaveAndNext_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnForceSave_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardSaveAndNext_Click"
            || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnSaveIgnoreReadOnly_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardSaveAndPrevious_Click") {
            $scope.TargetFormCaption = "Target Form:";
            if ($scope.formobject) {
                $scope.TargetForm = $scope.formobject.dictAttributes.ID;
                var methodName = "";
                var lstInitialLoad = $scope.formobject.Elements.filter(function (itm) { return itm.Name == "initialload"; });
                if (lstInitialLoad && lstInitialLoad.length > 0) {
                    var objInitialLoad = lstInitialLoad[0];
                    var lstMethod = objInitialLoad.Elements.filter(function (itm) { return itm.Name == "callmethods" && itm.dictAttributes.sfwMode != "New"; });
                    if (lstMethod && lstMethod.length > 0) {
                        methodName = lstMethod[0].dictAttributes.sfwMethodName;
                    }
                }
                if (methodName) {
                    var vrParCollection = [];
                    var strParamField = "";
                    var strParamValue = "";
                    var blnConstant = false;

                    if ($scope.formobject.dictAttributes.sfwRemoteObject != undefined && $scope.formobject.dictAttributes.sfwRemoteObject != "") {
                        var objServerObject = GetServerMethodObject($scope.formobject.dictAttributes.sfwRemoteObject, $scope.formobject.RemoteObjectCollection);
                        var paramerters = GetSrvMethodParameters(objServerObject, methodName);
                        if (paramerters) {
                            for (j = 0; j < paramerters.length; j++) {
                                var objParameter = { ParameterField: paramerters[j].dictAttributes.ID, ParameterValue: "", Constants: false };
                                $scope.XmlParameterCollection.push(objParameter);
                            }
                        }
                    }
                    else {
                        var xmlMethodParameters = $EntityIntellisenseFactory.getXmlMethodParameters($scope.formobject.dictAttributes.sfwEntity, methodName, true);
                        if (xmlMethodParameters && xmlMethodParameters.length) {
                            for (j = 0, len = xmlMethodParameters.length; j < len; j++) {
                                var objParameter = { ParameterField: xmlMethodParameters[j].ID, EntityField: xmlMethodParameters[j].Value, ParameterValue: xmlMethodParameters[j].Value, Constants: false };
                                $scope.XmlParameterCollection.push(objParameter);
                            }
                        }
                    }

                    $scope.PopulateParamValues($scope.XmlParameterCollection);
                }
            }
        }
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnSaveNew_Click") {
            $scope.TargetFormCaption = "Target Form:";
            $scope.TargetForm = $scope.formobject.dictAttributes.ID;
            if ($scope.formobject) {
                var methodName = "";
                var lstInitialLoad = $scope.formobject.Elements.filter(function (itm) { return itm.Name == "initialload"; });
                if (lstInitialLoad && lstInitialLoad.length > 0) {
                    var objInitialLoad = lstInitialLoad[0];
                    var lstMethod = objInitialLoad.Elements.filter(function (itm) { return itm.Name == "callmethods" && itm.dictAttributes.sfwMode != "Update"; });
                    if (lstMethod && lstMethod.length > 0) {
                        methodName = lstMethod[0].dictAttributes.sfwMethodName;
                    }
                }
                if (methodName) {
                    var vrParCollection = [];
                    var strParamField = "";
                    var strParamValue = "";
                    var blnConstant = false;

                    if ($scope.formobject.dictAttributes.sfwRemoteObject != undefined && $scope.formobject.dictAttributes.sfwRemoteObject != "") {
                        var objServerObject = GetServerMethodObject($scope.formobject.dictAttributes.sfwRemoteObject, $scope.formobject.RemoteObjectCollection);
                        var paramerters = GetSrvMethodParameters(objServerObject, methodName);
                        if (paramerters) {
                            for (j = 0; j < paramerters.length; j++) {
                                var objParameter = {
                                    ParameterField: paramerters[j].dictAttributes.ID, ParameterValue: "", Constants: false
                                };
                                $scope.XmlParameterCollection.push(objParameter);
                            }
                        }
                    }
                    else {
                        var xmlMethodParameters = $EntityIntellisenseFactory.getXmlMethodParameters($scope.formobject.dictAttributes.sfwEntity, methodName, true);
                        if (xmlMethodParameters && xmlMethodParameters.length) {
                            for (j = 0, len = xmlMethodParameters.length; j < len; j++) {
                                var objParameter = { ParameterField: xmlMethodParameters[j].ID, EntityField: xmlMethodParameters[j].Value, ParameterValue: xmlMethodParameters[j].Value, Constants: false };
                                $scope.XmlParameterCollection.push(objParameter);
                            }
                        }

                    }

                    $scope.PopulateParamValues($scope.XmlParameterCollection);
                }
            }
        }
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnWorkflowExecuteMethod_Click") {
            $scope.TargetFormCaption = "";
            var objParameter = {
                ParameterField: "aintActivityInstanceId", ParameterValue: "", Constants: false
            };
            $scope.XmlParameterCollection.push(objParameter);
            $scope.PopulateParamValues($scope.XmlParameterCollection);
        }
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnValidateExecuteBusinessMethod_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click"
            || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnExecuteBusinessMethod_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnCopyRecord_Click"
            || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardCancel_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnCompleteWorkflowActivities_Click") {
            if ($scope.SelectedObject.dictAttributes.sfwObjectMethod != undefined && $scope.SelectedObject.dictAttributes.sfwObjectMethod != "") {
                $scope.TargetFormCaption = "Object Method:";
                var lstData = [];

                if ($scope.SelectedObject.dictAttributes.hasOwnProperty("sfwExecuteMethodType") && $scope.SelectedObject.dictAttributes.sfwExecuteMethodType == "Rule") {
                    $scope.TargetFormCaption = "Rule:";
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, false, true, false, false);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formobject.dictAttributes.sfwEntity, "", "", true, false, false, true, false, false);
                    }
                }
                else if ($scope.SelectedObject.dictAttributes.hasOwnProperty("sfwExecuteMethodType") && $scope.SelectedObject.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                    $scope.TargetFormCaption = "Object Method:";
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, true, false, false, false);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formobject.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                    }
                }
                else if ($scope.SelectedObject.dictAttributes.hasOwnProperty("sfwExecuteMethodType") && $scope.SelectedObject.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                    $scope.TargetFormCaption = "Xml Method:";
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, false, false, false, true);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formobject.dictAttributes.sfwEntity, "", "", true, false, false, false, false, true);
                    }
                }
                else {
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, true, false, false, false);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formobject.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                    }
                }
                var strObjectMethod = $scope.SelectedObject.dictAttributes.sfwObjectMethod.trim();
                $scope.TargetForm = strObjectMethod;
                //var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();


                var lsttempData = [];
                var objMethod;
                if (lstData) {
                    angular.forEach(lstData, function (item) {
                        if (!objMethod) {
                            if (item.ID == strObjectMethod) {
                                objMethod = item;
                            }
                        }
                    });
                }

                if (objMethod) {
                    var paramerters = objMethod.Parameters;
                    //var paramerters = GetObjectMethodParameters(entityIntellisenseList, $scope.formobject.dictAttributes.sfwEntity, strObjectMethod);
                    if (paramerters) {
                        angular.forEach(paramerters, function (objParam) {
                            if (objMethod.RuleType && ["LogicalRule", "DecisionTable", "ExcelMatrix"].indexOf(objMethod.RuleType) > -1) {
                                if (objParam.Direction == "In") {
                                    if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
                                        if (["Object", "Collection", "List"].indexOf(objParam.DataType) > -1) {
                                            var objParameter = {
                                                ParameterField: objParam.ID, ParameterValue: null, IsObject: true
                                            };
                                            $scope.MethodParameterCollection.push(objParameter);
                                        }
                                        else {
                                            var objParameter = {
                                                ParameterField: objParam.ID, ParameterValue: null, IsObject: false
                                            };
                                            $scope.MethodParameterCollection.push(objParameter);
                                        }

                                    }
                                    else {
                                        var objParameter = {
                                            ParameterField: objParam.ID, ParameterValue: null, IsObject: false
                                        };
                                        $scope.MethodParameterCollection.push(objParameter);
                                    }

                                }
                            }
                            else {
                                var objParameter = {
                                    ParameterField: objParam.ID, ParameterValue: null,
                                    IsObject: false
                                };
                                $scope.MethodParameterCollection.push(objParameter);
                            }
                        });
                        $scope.PopulateParamValues($scope.MethodParameterCollection);
                    }
                }
            }

        }
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnExecuteServerMethod_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnDownload_Click") {
            $scope.TargetFormCaption = "Server Method:";
            var astrParamValue = $scope.SelectedObject.dictAttributes.sfwObjectMethod;
            if (astrParamValue == undefined || astrParamValue == "") { return; }

            var strObjectMethod = astrParamValue.trim();
            $scope.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;

            var RemoteObjectName = "srvCommon";
            if ($scope.formobject && $scope.formobject.dictAttributes.sfwRemoteObject) {
                RemoteObjectName = $scope.formobject.dictAttributes.sfwRemoteObject;
            }

            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var objServerObject = GetServerMethodObject(RemoteObjectName, $scope.formobject.RemoteObjectCollection);
            var paramerters = GetSrvMethodParameters(objServerObject, strObjectMethod);
            if (paramerters) {
                angular.forEach(paramerters, function (objParam) {
                    var objParameter = {
                        ParameterField: objParam.dictAttributes.ID
                    };
                    $scope.MethodParameterCollection.push(objParameter);
                });
                $scope.PopulateParamValues($scope.MethodParameterCollection);
            }
        }

        else if ($scope.SelectedObject.dictAttributes.sfwActiveForm || $scope.SelectedObject.dictAttributes.sfwXmlDocument) {
            //($scope.SelectedObject.isPlaceHolder && $scope.SelectedObject.placeHolder) || 

            var strActiveForm = $scope.SelectedObject.dictAttributes.sfwActiveForm;

            //if (!strActiveForm && $scope.SelectedObject.isPlaceHolder) {
            //    strActiveForm = $scope.SelectedObject.placeHolder;
            //}

            if (!strActiveForm && $scope.SelectedObject.Name === "sfwXMLPanel") {
                strActiveForm = $scope.SelectedObject.dictAttributes.sfwXmlDocument;
                var alForms = $scope.SelectedObject.dictAttributes.sfwXmlDocument.split(';');
            }
            else {
                var alForms = $scope.SelectedObject.dictAttributes.sfwActiveForm.split(';');
            }

            if (alForms.length > 0) {
                var tempActiveForm = "";
                for (var i = 0; i < alForms.length; i++) {
                    if (alForms[i] && alForms[i].contains("=")) {
                        if (tempActiveForm == "") {
                            tempActiveForm = alForms[i].substring(alForms[i].indexOf('=') + 1);
                        }
                        else {
                            tempActiveForm += "," + alForms[i].substring(alForms[i].indexOf('=') + 1);
                        }
                    }
                }
                if (tempActiveForm != "") {
                    strActiveForm = tempActiveForm;
                }
            }

            //    strActiveForm = alForms[0];


            $scope.TargetFormCaption = "Target Form :";
            $scope.TargetForm = strActiveForm;
            if (alForms && alForms.length > 0 && alForms[0].contains("=")) {
                strActiveForm = alForms[0].substring(alForms[0].indexOf('=') + 1);
            }
            if (strActiveForm) {
                $.connection.hubMain.server.getSingleFileDetail(strActiveForm).done(function (filedata) {
                    $scope.receiveSingleFileDetail(filedata);
                });
            }
        }
        //Add Defualt Navigation parameter for btnOpen_click i.e "aintPrimaryKey" if active form is not set
        else if ($scope.SelectedObject.dictAttributes.sfwMethodName && ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnOpen_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardFinish_Click")) {
            var objParameter = { ParameterField: "aintPrimaryKey", ParameterValue: "", Constants: false };
            if (!$scope.XmlParameterCollection.some(function (x) { return x.ParameterField == objParameter.ParameterField; })) {
                $scope.XmlParameterCollection.push(objParameter);
            }
            $scope.PopulateParamValues($scope.XmlParameterCollection);
        }
        if (curscope.currentfile.FileType == "FormLinkMaintenance" || curscope.currentfile.FileType == "FormLinkLookup" || curscope.currentfile.FileType == "FormLinkWizard") {
            $scope.PopulateAvailableFieldsForFormLink(undefined);
            $scope.isFormLink = true;
        }
        else {
            $scope.PopulateAvailableFields();
            $scope.isFormLink = false;
        }

        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
            $scope.$apply(function () {
                $scope.objGlobleParameters = data;
                $scope.PopulateGlobalParameters();
            });
        });
    };

    $scope.PopulateParamValues = function (ParameterCollection) {
        var istrParameters = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
        if (istrParameters != undefined && istrParameters != "") {


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


                    angular.forEach(ParameterCollection, function (objParameter) {
                        if (objParameter.ParameterField && strParamField && objParameter.ParameterField.toLowerCase() == strParamField.toLowerCase()) {
                            objParameter.ParameterValue = strParamsValue;
                            objParameter.Constants = blnConstant;
                        }
                    });
                }
            });
        }
    };


    //#endregion


    $scope.receiveSingleFileDetail = function (data) {
        $scope.sigleFileDetail = data;
        $scope.$apply(function () {
            $scope.newFormModel = data;
            if ($scope.newFormModel != null && $scope.newFormModel != undefined) {
                if (($scope.newFormModel.dictAttributes.sfwType == "Maintenance" || $scope.newFormModel.dictAttributes.sfwType == "Wizard" ||
                    $scope.newFormModel.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.newFormModel.dictAttributes.sfwType == "FormLinkWizard") && $scope.newFormModel.dictAttributes.sfwEntity != undefined && $scope.newFormModel.dictAttributes.sfwEntity != "") {
                    var blnNewButton = $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click"
                        || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnUpdate_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnSaveNew_Click";
                    $scope.activeFormEntity = $scope.newFormModel.dictAttributes.sfwEntity;
                    var method = "";
                    for (var i = 0; i < $scope.newFormModel.Elements.length; i++) {
                        if ($scope.newFormModel.Elements[i].Name == "initialload") {
                            for (var j = 0; j < $scope.newFormModel.Elements[i].Elements.length; j++) {
                                if (!blnNewButton) {
                                    if ($scope.newFormModel.Elements[i].Elements[j].Name == 'callmethods' && (!$scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMode || $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMode == 'Update')) {
                                        if ($scope.newFormModel.dictAttributes.sfwRemoteObject != undefined && $scope.newFormModel.dictAttributes.sfwRemoteObject != "") {
                                            method = $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMethodName;
                                            break;
                                        }

                                        else {
                                            method = $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMethodName;
                                            break;
                                        }
                                    }
                                }
                                else {
                                    if ($scope.newFormModel.Elements[i].Elements[j].Name == 'callmethods' && (!$scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMode || $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMode == 'New')) {
                                        if ($scope.newFormModel.dictAttributes.sfwRemoteObject != undefined && $scope.newFormModel.dictAttributes.sfwRemoteObject != "") {
                                            method = $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMethodName;
                                            break;
                                        }

                                        else {
                                            method = $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMethodName;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var vrParCollection = [];
                    var strParamField = "";
                    var strParamValue = "";
                    var blnConstant = false;

                    if ($scope.newFormModel.dictAttributes.sfwRemoteObject != undefined && $scope.newFormModel.dictAttributes.sfwRemoteObject != "") {
                        var objServerObject = GetServerMethodObject($scope.newFormModel.dictAttributes.sfwRemoteObject, $scope.formobject.RemoteObjectCollection);
                        var paramerters = GetSrvMethodParameters(objServerObject, method);
                        if (paramerters) {
                            for (j = 0; j < paramerters.length; j++) {
                                var objParameter = { ParameterField: paramerters[j].dictAttributes.ID, ParameterValue: "", Constants: false };
                                vrParCollection.push(objParameter);
                            }
                        }
                    }
                    else {
                        if (method) {
                            var xmlMethodParameters = $EntityIntellisenseFactory.getXmlMethodParameters($scope.activeFormEntity, method, true);
                            if (xmlMethodParameters && xmlMethodParameters.length) {
                                for (j = 0, len = xmlMethodParameters.length; j < len; j++) {
                                    var objParameter = { ParameterField: xmlMethodParameters[j].ID, EntityField: xmlMethodParameters[j].Value, ParameterValue: xmlMethodParameters[j].Value, Constants: false };
                                    vrParCollection.push(objParameter);
                                }
                            }
                        }
                    }
                    if ($scope.SelectedObject && $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click") {
                        for (var i = 0; i < $scope.newFormModel.Elements.length; i++) {
                            if ($scope.newFormModel.Elements[i].Name == "initialload") {
                                for (var j = 0; j < $scope.newFormModel.Elements[i].Elements.length; j++) {
                                    if ($scope.newFormModel.Elements[i].Elements[j].Name == "session") {
                                        var objSession = $scope.newFormModel.Elements[i].Elements[j];
                                        if (objSession.Elements && objSession.Elements.length > 0) {
                                            for (k = 0; k < objSession.Elements.length; k++) {
                                                var objParameter = { ParameterField: objSession.Elements[k].dictAttributes.ID, ParameterValue: "", Constants: false };
                                                vrParCollection.push(objParameter);
                                            }
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }

                    //Add Defualt Navigation parameter for btnOpen_click i.e "aintPrimaryKey" if active form has no xml methods
                    if ($scope.SelectedObject && ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnOpen_Click" || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnWizardFinish_Click") && $scope.newFormModel.Elements) {
                        var blnAddDefaultParameter = true;
                        var objIntialLoad = $scope.newFormModel.Elements.filter(function (aElement) { return aElement.Name && aElement.Name == "initialload"; });
                        if (objIntialLoad.length > 0 && objIntialLoad[0].Elements) {
                            var objCallMethds = objIntialLoad[0].Elements.filter(function (aCallMethod) { return aCallMethod.Name && aCallMethod.Name == "callmethods"; });
                            if (objCallMethds.length > 0) {
                                blnAddDefaultParameter = false;
                            }
                        }
                        if (blnAddDefaultParameter) {
                            var objParameter = { ParameterField: "aintPrimaryKey", ParameterValue: "", Constants: false };
                            vrParCollection.push(objParameter);
                        }
                    }
                    var vrXmlParCollection = [];
                    for (i = 0; i < vrParCollection.length; i++) {
                        if (!$scope.XmlParameterCollection.some(function (x) { return x.ParameterField == vrParCollection[i].ParameterField; })) {

                            var objParameter = { ParameterField: vrParCollection[i].ParameterField, EntityField: vrParCollection[i].EntityField, ParameterValue: "", Constants: false };
                            vrXmlParCollection.push(objParameter);
                        }
                    }
                    angular.forEach(vrXmlParCollection, function (x) {
                        $scope.XmlParameterCollection.push(x);
                    });


                    var customAttribute = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
                    if (customAttribute != undefined && customAttribute != "") {
                        $scope.PopulateParameters(customAttribute, "Maintenance");
                    }

                }
                else if ($scope.newFormModel.dictAttributes.sfwType == "Lookup") {
                    for (var i = 0; i < $scope.newFormModel.Elements.length; i++) {
                        if ($scope.newFormModel.Elements[i].Name == "sfwTable") {
                            $scope.isPanelCriteria = false;
                            $scope.PopulateLookupPanel($scope.newFormModel.Elements[i]);

                            var customAttribute = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
                            if (customAttribute != undefined && customAttribute != "") {
                                $scope.PopulateXmlParameters(customAttribute, "Lookup");

                                $scope.IsNewActiveForm = true;
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.IsNewActiveForm = false;
                                        }
                                    }
                                }
                            }
                            if ((customAttribute != undefined && customAttribute != "") && $scope.LookupParameterCollection.length != vrLookupField.length && !$scope.IsNewActiveForm) {
                                var vrLookupParCollection = [];
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField != vrLookupField[i].ParameterField) {
                                            var objParameter = { ControlId: vrLookupField[i].ControlId, ParameterField: vrLookupField[i].ParameterField, ParameterValue: "", Constants: false };
                                            var isParameterFieldPresent = false;
                                            var isParameterFieldPresentExist = false;
                                            if (vrLookupParCollection.length > 0) {
                                                for (var k = 0; k < vrLookupParCollection.length; k++) {
                                                    if (vrLookupParCollection[k].ParameterField == objParameter.ParameterField) {
                                                        isParameterFieldPresent = true;
                                                    }
                                                }
                                            }
                                            if ($scope.LookupParameterCollection.length > 0) {
                                                for (var k = 0; k < $scope.LookupParameterCollection.length; k++) {
                                                    if ($scope.LookupParameterCollection[k].ParameterField == objParameter.ParameterField) {
                                                        isParameterFieldPresentExist = true;
                                                    }
                                                }
                                            }

                                            if (!isParameterFieldPresent && !isParameterFieldPresentExist && objParameter && objParameter.ParameterField) {
                                                vrLookupParCollection.push(objParameter);
                                            }
                                        }
                                        else if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.LookupParameterCollection[j].ControlId = vrLookupField[i].ControlId;
                                        }
                                    }
                                }

                                angular.forEach(vrLookupParCollection, function (objParameter) {
                                    //var isParameterFieldPresent = $scope.LookupParameterCollection.filter(function (LookupCollection) { return x == LookupCollection.ParameterField; });
                                    if (objParameter && objParameter.ParameterField) {
                                        var isParameterFieldPresent = $scope.LookupParameterCollection.filter(function (item) { return item.ParameterField && item.ParameterField == objParameter.ParameterField; });
                                        if (isParameterFieldPresent.length == 0) {
                                            $scope.LookupParameterCollection.push(objParameter);
                                        }
                                    }
                                });
                            }
                            else if ($scope.LookupParameterCollection.length == vrLookupField.length && !$scope.IsNewActiveForm) {
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.LookupParameterCollection[j].ControlId = vrLookupField[i].ControlId;
                                        }
                                    }
                                }
                            }
                            else {
                                $scope.LookupParameterCollection = [];
                                if (vrLookupField.length > 0) {
                                    angular.forEach(vrLookupField, function (item) {
                                        if (item && item.ParameterField) {
                                            var isParameterFieldPresent = $scope.LookupParameterCollection.filter(function (x) { return x.ParameterField && x.ParameterField == item.ParameterField; });
                                            if (isParameterFieldPresent.length == 0) {
                                                $scope.LookupParameterCollection.push(item);
                                            }
                                        }
                                    });
                                }
                            }

                            break;
                        }
                    }
                }
            }
        });

        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });

    };

    //#region Receive FormModel from server for Active Form
    $scope.receivenewformmodel = function (data) {
        $scope.$apply(function () {
            $scope.newFormModel = data;
            if ($scope.newFormModel != null && $scope.newFormModel != undefined) {
                if ($scope.ActiveFormType == "Maintenance" && $scope.newFormModel.dictAttributes.sfwEntity != undefined && $scope.newFormModel.dictAttributes.sfwEntity != "") {
                    $scope.activeFormEntity = $scope.newFormModel.dictAttributes.sfwEntity;
                    var method = "";
                    for (var i = 0; i < $scope.newFormModel.Elements.length; i++) {
                        if ($scope.newFormModel.Elements[i].Name == "initialload") {
                            for (var j = 0; j < $scope.newFormModel.Elements[i].Elements.length; j++) {
                                if ($scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMode) {
                                    var method = $scope.newFormModel.Elements[i].Elements[j].dictAttributes.sfwMethodName;
                                }
                            }
                        }
                    }
                    var vrParCollection = [];
                    var strParamField = "";
                    var strParamValue = "";
                    var blnConstant = false;

                    if (method) {
                        var xmlMethodParameters = $EntityIntellisenseFactory.getXmlMethodParameters($scope.activeFormEntity, method, true);
                        if (xmlMethodParameters && xmlMethodParameters.length) {
                            for (j = 0, len = xmlMethodParameters.length; j < len; j++) {
                                var objParameter = { ParameterField: xmlMethodParameters[j].ID, EntityField: xmlMethodParameters[j].Value, ParameterValue: xmlMethodParameters[j].Value, Constants: false };
                                vrParCollection.push(objParameter);
                            }
                            var customAttribute = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
                            if (customAttribute != undefined && customAttribute != "") {
                                $scope.PopulateXmlParameters(customAttribute, "Maintenance");

                                $scope.IsNewActiveForm = true;
                                for (i = 0; i < vrParCollection.length; i++) {
                                    for (j = 0; j < $scope.XmlParameterCollection.length; j++) {
                                        if ($scope.XmlParameterCollection[j].ParameterField == vrParCollection[i].ParameterField) {
                                            $scope.IsNewActiveForm = false;
                                        }
                                    }
                                }
                            }
                            if ((customAttribute != undefined && customAttribute != "") && $scope.XmlParameterCollection.length != vrParCollection.length && !$scope.IsNewActiveForm) {
                                var vrXmlParCollection = [];
                                for (i = 0; i < vrParCollection.length; i++) {
                                    for (j = 0; j < $scope.XmlParameterCollection.length; j++) {
                                        if ($scope.XmlParameterCollection[j].ParameterField != vrParCollection[i].ParameterField) {
                                            var objParameter = { ParameterField: vrParCollection[i].ParameterField, EntityField: vrParCollection[i].EntityField, ParameterValue: "", Constants: false };
                                            vrXmlParCollection.push(objParameter);
                                        }
                                    }
                                }
                                angular.forEach(vrXmlParCollection, function (x) {
                                    $scope.XmlParameterCollection.push(x);
                                });
                            }
                            else if ($scope.XmlParameterCollection.length == vrParCollection.length && !$scope.IsNewActiveForm) {
                                /*var vrXmlParCollection = [];
                                for (i = 0; i < vrParCollection.length; i++) {
                                    for (j = 0; j < $scope.XmlParameterCollection.length; j++) {
                                        if ($scope.XmlParameterCollection[j].ParameterField != vrParCollection[i].ParameterField) {
                                            var objParameter = { ParameterField: vrParCollection[i].ParameterField, ParameterValue: "", Constants: false };
                                            vrXmlParCollection.push(objParameter);
                                        }
                                    }
                                }
                                angular.forEach(vrXmlParCollection, function (x) {
                                    $scope.XmlParameterCollection.push(x);
                                });*/
                            }
                            else {
                                $scope.XmlParameterCollection = [];
                                angular.forEach(vrParCollection, function (x) {
                                    $scope.XmlParameterCollection.push(x);
                                });
                            }
                        }
                    }


                    var vrSessionParCollection = [];
                    var vrNewParCollection = [];
                    var lstInitialLoad = $scope.formobject.Elements.filter(function (itm) { return itm.Name == "initialload"; });
                    //Add Session Field as Navigation Parameter 
                    if ($scope.SelectedObject && $scope.SelectedObject.Name == "sfwButton" && $scope.SelectedObject.dictAttributes.sfwMethodName && $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click") {
                        if (lstInitialLoad && lstInitialLoad.length > 0) {
                            var objInitialLoad = lstInitialLoad[0];
                            var objSession = objInitialLoad.Elements.filter(function (itm) { return itm.Name == "session"; });
                            if (objSession && objSession.Elements && objSession.Elements.length > 0) {
                                for (k = 0; k < objSession.Elements.length; k++) {
                                    var objParameter = { ParameterField: objSession.Elements[k].dictAttributes.ID, ParameterValue: "", Constants: false };
                                    vrNewParCollection.push(objParameter);
                                }
                            }
                        }
                    }
                    for (i = 0; i < vrNewParCollection.length; i++) {
                        if (!$scope.XmlParameterCollection.some(function (x) { return x.ParameterField == vrNewParCollection[i].ParameterField; })) {

                            var objParameter = { ParameterField: vrNewParCollection[i].ParameterField, ParameterValue: "", Constants: false };
                            vrSessionParCollection.push(objParameter);
                        }
                    }

                    angular.forEach(vrSessionParCollection, function (x) {
                        $scope.XmlParameterCollection.push(x);
                    });
                }
                else if ($scope.ActiveFormType == "Lookup") {
                    for (var i = 0; i < $scope.newFormModel.Elements.length; i++) {
                        if ($scope.newFormModel.Elements[i].Name == "sfwTable") {
                            $scope.isPanelCriteria = false;
                            $scope.PopulateLookupPanel($scope.newFormModel.Elements[i]);

                            var customAttribute = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;
                            if (customAttribute != undefined && customAttribute != "") {
                                $scope.PopulateXmlParameters(customAttribute, "Lookup");

                                $scope.IsNewActiveForm = true;
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.IsNewActiveForm = false;
                                        }
                                    }
                                }
                            }
                            if ((customAttribute != undefined && customAttribute != "") && $scope.LookupParameterCollection.length != vrLookupField.length && !$scope.IsNewActiveForm) {
                                var vrLookupParCollection = [];
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField != vrLookupField[i].ParameterField) {
                                            var objParameter = { ControlId: vrLookupField[i].ControlId, ParameterField: vrLookupField[i].ParameterField, ParameterValue: "", Constants: false };
                                            var isParameterFieldPresent = false;
                                            var isParameterFieldPresentExist = false;
                                            if (vrLookupParCollection.length > 0) {
                                                for (var k = 0; k < vrLookupParCollection.length; k++) {
                                                    if (vrLookupParCollection[k].ParameterField == objParameter.ParameterField) {
                                                        isParameterFieldPresent = true;
                                                    }
                                                }
                                            }
                                            if ($scope.LookupParameterCollection.length > 0) {
                                                for (var k = 0; k < $scope.LookupParameterCollection.length; k++) {
                                                    if ($scope.LookupParameterCollection[k].ParameterField == objParameter.ParameterField) {
                                                        isParameterFieldPresentExist = true;
                                                    }
                                                }
                                            }

                                            if (!isParameterFieldPresent && !isParameterFieldPresentExist) {
                                                vrLookupParCollection.push(objParameter);
                                            }
                                        }
                                        else if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.LookupParameterCollection[j].ControlId = vrLookupField[i].ControlId;
                                        }
                                    }
                                }

                                angular.forEach(vrLookupParCollection, function (x) {
                                    var isParameterFieldPresent = $scope.LookupParameterCollection.filter(function (LookupCollection) { return x == LookupCollection.ParameterField; });

                                    $scope.LookupParameterCollection.push(x);
                                });
                            }
                            else if ($scope.LookupParameterCollection.length == vrLookupField.length && !$scope.IsNewActiveForm) {
                                for (i = 0; i < vrLookupField.length; i++) {
                                    for (j = 0; j < $scope.LookupParameterCollection.length; j++) {
                                        if ($scope.LookupParameterCollection[j].ParameterField == vrLookupField[i].ParameterField) {
                                            $scope.LookupParameterCollection[j].ControlId = vrLookupField[i].ControlId;
                                        }
                                    }
                                }
                            }
                            else {
                                //if (vrLookupField.length > 0) {
                                //    angular.forEach(vrLookupField, function (x) {
                                //        $scope.LookupParameterCollection.push(x);
                                //    });
                                //}
                                $scope.LookupParameterCollection = [];
                                if (vrLookupField.length > 0) {
                                    angular.forEach(vrLookupField, function (x) {
                                        $scope.LookupParameterCollection.push(x);
                                    });
                                }
                            }

                            break;
                        }
                    }
                }
            }
            //$scope.PopulateAvailableFields();
        });

        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
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
        if ($scope.IsFormCodeGroup || $scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.SelectedObject.Name == "sfwCascadingDropDownList" || $scope.SelectedObject.Name == "sfwSeries") {
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
                            $.connection.hubForm.server.getEntityQueryColumns($scope.SelectedObject.dictAttributes.sfwQueryID, "NavigationParameterController").done(function (data) {
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
        $scope.$apply(function () {
            var mainItem = { ID: $scope.SelectedObject.dictAttributes.sfwQueryID, IsSelected: false, Elements: [] };
            //$scope.EntityFieldCollection.push(mainItem);
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
                            if (sfxCtrl.Name == "sfwPanel" || sfxCtrl.Name == "sfwDialogPanel" || sfxCtrl.Name == "sfwListView") {

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
                                                //in grid always it will be entity field in lookup and maintenance
                                                if ("sfwEntityField" in sfxControl.dictAttributes) {
                                                    var strFieldName = "";

                                                    strFieldName = sfxControl.dictAttributes["sfwEntityField"];
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
                                if (strTreeCaption != "" && strTreeCaption != undefined && (sfxCtrl.Name != "sfwButton" && sfxCtrl.Name != "sfwLinkButton" && sfxCtrl.Name != "sfwImageButton")) {
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

    $scope.PopulateEntityFields = function (asfxTable, newItem) {

        for (var i = 0; i < asfxTable.Elements.length; i++) {
            if (asfxTable.Elements[i].dictAttributes.sfwEntityField != undefined) {
                newItem.Elements.push({ ID: asfxTable.Elements[i].dictAttributes.sfwEntityField, Elements: [] });
            }

            if (asfxTable.Elements[i].Elements.length > 0) {
                $scope.PopulateEntityFields(asfxTable.Elements[i], newItem);
            }
        }
    };

    $scope.PopulateGlobalParameters = function () {
        $scope.GlobalParamCollection = [];
        function AddInobjGlobalParam(itm) {
            if (itm.dictAttributes && itm.dictAttributes.ID) {
                var strFieldName = itm.dictAttributes.ID;
                if (!globalParameters.filter(function (itm) { return itm == strFieldName.trim(); })) {
                    globalParameters.push(strFieldName.trim());
                }
                // objGlobalParam.Children.Add(new clsAvailableControl { ControlName = strFieldName });
                var mainItem = {
                    ID: "~" + strFieldName, IsExpanded: false, IsSelected: false, Elements: []
                };
                objGlobalParam.Elements.push(mainItem);
            }
        }
        if ($scope.objGlobleParameters) {
            var strFormType = $scope.formobject.dictAttributes.sfwType;
            if (($scope.SelectedObject.Name == "sfwTextBox" || $scope.SelectedObject.Name == "sfwButton" ||
                $scope.SelectedObject.Name == "sfwCascadingDropDownList" || $scope.SelectedObject.Name == "sfwDropDownList" || $scope.SelectedObject.Name == "sfwMultiSelectDropDownList" ||
                $scope.SelectedObject.Name == "sfwSeries" || $scope.SelectedObject.Name == "sfwListBox"
                || $scope.SelectedObject.Name == "sfwCheckBoxList" || $scope.SelectedObject.Name == "sfwRadioButtonList" || $scope.SelectedObject.Name == "sfwMultiCorrespondence")
                /*&& strFormType != "Correspondence"*/) {
                if ($scope.SelectedObject.Name == "sfwButton") {
                    if ($scope.SelectedObject.dictAttributes.sfwMethodName != "btnOpen_Click" && $scope.SelectedObject.dictAttributes.sfwMethodName != "btnNew_Click")
                        return;
                }
                var globalParameters = [];

                if ($scope.objGlobleParameters.Elements.length > 0) {
                    var objGlobalParam = { ID: "Global Parameters", IsExpanded: false, IsSelected: false, Elements: [], IsVisible: true };

                    angular.forEach($scope.objGlobleParameters.Elements, AddInobjGlobalParam);
                    if (objGlobalParam.Elements.length > 0) {
                        $scope.EntityFieldCollection.push(objGlobalParam);
                        if (strFormType == "Correspondence") {
                            $scope.GlobalParamCollection.push(objGlobalParam);
                        }
                    }
                }

            }
        }
    };
    //#endregion

    //#region Populate navigation Parameter on Click and Query Change

    $scope.GetQueryFromEntity = function () {
        var objQuery;
        if ($scope.SelectedObject.dictAttributes.sfwLoadSource && $scope.SelectedObject.dictAttributes.sfwLoadType == "Query") {
            var lst = $scope.SelectedObject.dictAttributes.sfwLoadSource.split('.');
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var strQueryID = lst[1];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lstEntity = $filter('filter')(entityIntellisenseList, { ID: entityName }, true);
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) { return x.ID == strQueryID; });
                    if (lstQuery && lstQuery.length > 0) {
                        objQuery = lstQuery[0];
                    }
                }
            }
        }
        return objQuery;
    };

    $scope.PopulateNavigationParameters = function () {
        if ($scope.SelectedObject.Name === "sfwCascadingDropDownList" || ($scope.SelectedObject.Name === "sfwMultiSelectDropDownList" && $scope.SelectedObject.dictAttributes.sfwLoadType === "CodeGroup")) {
            $scope.strSelectedParameters = $scope.SelectedObject.dictAttributes.sfwParameters;
        }
        else {
            $scope.strSelectedParameters = $scope.SelectedObject.dictAttributes.sfwNavigationParameter;

        }

        if ($scope.SelectedObject.dictAttributes.sfwLoadSource && $scope.SelectedObject.dictAttributes.sfwLoadType == "Query") {
            $scope.thisQuery = $scope.GetQueryFromEntity();

            if ($scope.thisQuery != undefined && $scope.thisQuery != "") {
                $scope.ParameterCollection = [];
                angular.forEach($scope.thisQuery.Parameters, function (strParam) {
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
        }
        else if ($scope.SelectedObject.dictAttributes.sfwLoadSource && $scope.SelectedObject.dictAttributes.sfwLoadType == "ChildMethod") {
            var strObjectMethod = $scope.SelectedObject.dictAttributes.sfwLoadSource.trim();
            $scope.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;

            $scope.TargetForm = strObjectMethod;
            var objParent = FindParent($scope.SelectedObject, "sfwGridView");
            if (objParent == null) {

                objParent = FindParent($scope.SelectedObject, "sfwListView");
            }
            if (objParent) {
                var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, objParent.dictAttributes.sfwEntityField);
                if (objParentField && objParentField.Entity) {
                    var lstData = $Entityintellisenseservice.GetIntellisenseData(objParentField.Entity, "", "", true, false, true, false, false, false);
                    var lsttempData = [];
                    var objMethod;
                    if (lstData) {
                        angular.forEach(lstData, function (item) {
                            if (!objMethod) {
                                if (item.ID == strObjectMethod) {
                                    objMethod = item;
                                }
                            }
                        });
                    }

                    if (objMethod) {
                        var paramerters = objMethod.Parameters;

                        if (paramerters) {
                            angular.forEach(paramerters, function (objParam) {
                                var objParameter = { ParameterField: objParam.ID, ParameterValue: "" };
                                $scope.ParameterCollection.push(objParameter);
                            });
                        }
                    }
                }
            }


        }

        else if ($scope.SelectedObject.dictAttributes.sfwLoadSource && $scope.SelectedObject.dictAttributes.sfwLoadType == "Method") {
            var strObjectMethod = $scope.SelectedObject.dictAttributes.sfwLoadSource.trim();
            $scope.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;

            $scope.TargetForm = strObjectMethod;

            var lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formobject.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
            var lsttempData = [];
            var objMethod;
            if (lstData) {
                angular.forEach(lstData, function (item) {
                    if (!objMethod) {
                        if (item.ID == strObjectMethod) {
                            objMethod = item;
                        }
                    }
                });
            }

            if (objMethod) {
                var paramerters = objMethod.Parameters;
                //var paramerters = GetObjectMethodParameters(entityIntellisenseList, $scope.formobject.dictAttributes.sfwEntity, strObjectMethod);
                if (paramerters) {
                    angular.forEach(paramerters, function (objParam) {
                        var objParameter = { ParameterField: objParam.ID, ParameterValue: "" };
                        $scope.ParameterCollection.push(objParameter);
                    });
                }
            }

            //  var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

            // var paramerters = GetObjectMethodParameters(entityIntellisenseList, $scope.formobject.dictAttributes.sfwEntity, strObjectMethod);

        }
        else if ($scope.SelectedObject && $scope.SelectedObject.dictAttributes.sfwLoadSource && $scope.SelectedObject.dictAttributes.sfwLoadType == "ServerMethod") {
            var strObjectMethod = $scope.SelectedObject.dictAttributes.sfwLoadSource.trim();
            $scope.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;

            $scope.TargetForm = strObjectMethod;

            var RemoteObjectName = "srvCommon";
            if ($scope.formobject && $scope.formobject.dictAttributes.sfwRemoteObject) {
                RemoteObjectName = $scope.formobject.dictAttributes.sfwRemoteObject;
            }

            if (RemoteObjectName) {
                var objServerObject = GetServerMethodObject(RemoteObjectName, $scope.formobject.RemoteObjectCollection);
                var paramerters = GetSrvMethodParameters(objServerObject, strObjectMethod);
                if (paramerters) {
                    for (j = 0; j < paramerters.length; j++) {
                        var objParameter = { ParameterField: paramerters[j].dictAttributes.ID, ParameterValue: "", Constants: false };
                        $scope.XmlParameterCollection.push(objParameter);
                    }
                }
            }
        }
        // set param for codegroup only for cascading dropdownlist - BALA
        else if (($scope.SelectedObject.Name == "sfwCascadingDropDownList" || $scope.SelectedObject.Name == "sfwMultiSelectDropDownList") && (($scope.SelectedObject.dictAttributes.sfwLoadSource || $scope.SelectedObject.placeHolder) && $scope.SelectedObject.dictAttributes.sfwLoadType == "CodeGroup")) {
            $scope.ParameterCollection.push({ ParameterField: "data1", ParameterValue: "" });
            $scope.ParameterCollection.push({ ParameterField: "data2", ParameterValue: "" });
            $scope.ParameterCollection.push({ ParameterField: "data3", ParameterValue: "" });
        }
        if ($scope.ParameterCollection != undefined && $scope.ParameterCollection.length > 0 && $scope.strSelectedParameters != undefined) {
            var lst = $scope.strSelectedParameters.split(";");
            for (var i = 0; i < lst.length; i++) {
                var strParam = lst[i];
                //var lstControlsID = lst[i].split("=");
                var strControlID = strParam;
                var strParamsValue = strParam;
                var blnConstant = false;

                if (strParam.contains("=")) {
                    strControlID = strParam.substring(0, strParam.indexOf('='));
                    strParamsValue = strParam.substring(strParam.indexOf('=') + 1);

                    if (strParamsValue.match("^#")) {
                        strParamsValue = strParamsValue.substring(1);
                        blnConstant = true;
                    }
                }
                for (var j = 0; j < $scope.ParameterCollection.length; j++) {
                    if ($scope.ParameterCollection[j].ParameterField == strControlID) {
                        $scope.ParameterCollection[j].ParameterValue = strParamsValue;
                        $scope.ParameterCollection[j].Constants = blnConstant;
                    }
                }
            }
        }
    };

    function fnUpdateNavParams() {
        if ($scope.thisQuery) {
            angular.forEach($scope.thisQuery.Parameters, function (strParam) {
                ParameterField = strParam.ID;
                if (ParameterField.contains("@")) {
                    strParamField = ParameterField.substring(ParameterField.indexOf('@') + 1, ParameterField.length);
                }

                var objParameter = { ParameterField: strParamField, ParameterValue: "" };
                $scope.ParameterCollection.push(objParameter);
            });
        }
    }
    //#endregion

    //#region Populate Lookup Panel Criteria When Selected ActiveFormType is Lookup, In Approve Button
    var vrLookupField = [];
    $scope.PanelCriteria;
    $scope.PopulateLookupPanel = function (asfxTable) {
        if (asfxTable) {
            var desc = getDescendents(asfxTable);
            if (desc && desc.length > 0) {
                var desc = desc.filter(function (item) { return item.Name == "sfwPanel"; });
                if (desc && desc.length > 0) {
                    for (var index = 0; index < desc.length; index++) {
                        var panelDesc = getDescendents(desc[index]);
                        if (panelDesc && panelDesc.length > 0) {
                            if (panelDesc.some(function (item) { return item.Name == "sfwTable" && item.dictAttributes.ID == "tblCriteria"; })) {
                                $scope.PanelCriteria = desc[index];
                                var controls = getDescendents($scope.PanelCriteria);;//.filter(function (item) { return item.Name == "sfwTextBox" || item.Name == "sfwDropDownList"; });
                                if (controls && controls.length > 0) {
                                    for (var idx = 0; idx < controls.length; idx++) {
                                        if (controls[idx] && controls[idx].dictAttributes.sfwDataField) {
                                            var objParameter = { ControlId: controls[idx].dictAttributes.ID, ParameterField: controls[idx].dictAttributes.sfwDataField, ParameterValue: "", Constants: false };
                                            vrLookupField.push(objParameter);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    };

    $scope.XmlParameterCollection = [];
    $scope.PopulateXmlParameters = function (customAttribute, formType) {

        var alParams = customAttribute.split(';');

        angular.forEach(alParams, function (strParam) {
            if (strParam == undefined || strParam == "") {

            }
            else {
                var strParamField = strParam;
                var strParamValue = strParam;
                var blnConstant = false;

                if (strParam.contains("=")) {
                    strParamField = strParam.substring(0, strParam.indexOf('='));
                    strParamValue = strParam.substring(strParam.indexOf('=') + 1);

                    if (strParamValue.match("^#")) {
                        strParamValue = strParamValue.substring(1);
                        blnConstant = true;
                    }
                }

                if (formType == "Maintenance") {
                    var objParameter = { ParameterField: strParamField, EntityField: strParamValue, ParameterValue: strParamValue, Constants: blnConstant };
                    $scope.XmlParameterCollection.push(objParameter);
                }
                else if (formType == "Lookup") {
                    var objParameter = { ControlId: "", ParameterField: strParamField, ParameterValue: strParamValue, Constants: blnConstant };
                    $scope.LookupParameterCollection.push(objParameter);
                }
                else {
                    var objParameter = {
                        ParameterField: strParamField, ParameterValue: strParamValue, Constants: blnConstant
                    };
                    $scope.MethodParameterCollection.push(objParameter);
                }
            }
        });

        //if ($scope.XmlParameterCollection.length != $scope.thisQuery.Parameters.length) {
        //    var vrParCollection = [];
        //    for (i = 0; i < $scope.thisQuery.Parameters.length; i++) {
        //        for (j = 0; j < $scope.ParameterCollection.length; j++) {
        //            if ($scope.ParameterCollection[j].ParameterField != $scope.thisQuery.Parameters[i].ID.substring($scope.thisQuery.Parameters[i].ID.indexOf('@') + 1, $scope.thisQuery.Parameters[i].ID.length)) {
        //                var objParameter = { ParameterField: $scope.thisQuery.Parameters[i].ID.substring($scope.thisQuery.Parameters[i].ID.indexOf('@') + 1, $scope.thisQuery.Parameters[i].ID.length), ParameterValue: "" };
        //                vrParCollection.push(objParameter);
        //            }
        //        }
        //    }
        //    angular.forEach(vrParCollection, function (x) {
        //        $scope.ParameterCollection.push(x);
        //    });
        //}
    };


    $scope.PopulateParameters = function (customAttribute, formType) {
        var alParams = customAttribute.split(';');

        angular.forEach(alParams, function (strParam) {
            if (strParam == undefined || strParam == "") {

            }
            else {
                var strParamField = strParam;
                var strParamValue = strParam;
                var blnConstant = false;

                if (strParam.contains("=")) {
                    strParamField = strParam.substring(0, strParam.indexOf('='));
                    strParamValue = strParam.substring(strParam.indexOf('=') + 1);

                    if (strParamValue.match("^#")) {
                        strParamValue = strParamValue.substring(1);
                        blnConstant = true;
                    }
                }

                if (formType == "Maintenance") {
                    var lst = $scope.XmlParameterCollection.filter(function (x) { return x.ParameterField == strParamField; });
                    if (lst && lst.length > 0) {
                        lst[0].ParameterValue = strParamValue;
                        lst[0].Constants = blnConstant;
                    }
                }
                else if (formType == "Lookup") {
                    var lst = $scope.LookupParameterCollection.filter(function (x) { return x.ParameterField == strParamField; });
                    if (lst && lst.length > 0) {
                        lst[0].ParameterValue = strParamValue;
                        lst[0].Constants = blnConstant;
                    }
                }
                else {
                    var lst = $scope.MethodParameterCollection.filter(function (x) { return x.ParameterField == strParamField; });
                    if (lst && lst.length > 0) {
                        lst[0].ParameterValue = strParamValue;
                        lst[0].Constants = blnConstant;
                    }
                }
            }
        });
    };
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
        if ($scope.ParameterCollection.length > 0) {
            strCustomAttribute = $scope.GetSavedString($scope.ParameterCollection);

            //$scope.SelectedObject.dictAttributes.sfwParameters = strCustomAttribute;
        }
        else if ($scope.LookupParameterCollection.length > 0) {
            strCustomAttribute = $scope.GetSavedString($scope.LookupParameterCollection);
        }
        else if ($scope.XmlParameterCollection.length > 0) {
            strCustomAttribute = $scope.GetSavedString($scope.XmlParameterCollection);
        }
        else if ($scope.MethodParameterCollection.length > 0) {
            strCustomAttribute = $scope.GetSavedString($scope.MethodParameterCollection);
        }

        if (strCustomAttribute != undefined) {
            if ($scope.SelectedObject.Name == "sfwCascadingDropDownList" || ($scope.SelectedObject.Name === "sfwMultiSelectDropDownList" && $scope.SelectedObject.dictAttributes.sfwLoadType === "CodeGroup")) {
                $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwParameters, $scope.SelectedObject.dictAttributes, "sfwParameters", strCustomAttribute);
            }
            else {
                if ($scope.SelectedObject.dictAttributes.sfwActiveForm && $scope.SelectedObject.dictAttributes.sfwActiveForm.trim().length > 0) {
                    if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnNew_Click") {
                        var relatedControl = $scope.SelectedObject.dictAttributes.sfwActiveForm.contains("=") ? $scope.SelectedObject.dictAttributes.sfwRelatedControl : "";
                        if (strCustomAttribute == "") {
                            $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", relatedControl);

                        }
                        else {
                            if (relatedControl && relatedControl != "") {
                                $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute + ";" + relatedControl);

                            }
                            else {
                                $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute);
                            }
                        }
                    }
                    else if ($scope.SelectedObject.dictAttributes.sfwMethodName == "btnOpen_Click") {
                        var entityField = $scope.SelectedObject.dictAttributes.sfwActiveForm.contains("=") ? $scope.SelectedObject.dictAttributes.sfwEntityField : "";
                        if (strCustomAttribute == "") {
                            $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", entityField);

                        }
                        else {
                            if (entityField) {
                                $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute + ";" + entityField);

                            }
                            else {
                                $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute);

                            }
                        }
                    }
                    else {
                        $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute);

                    }
                }
                else {
                    $rootScope.EditPropertyValue($scope.SelectedObject.dictAttributes.sfwNavigationParameter, $scope.SelectedObject.dictAttributes, "sfwNavigationParameter", strCustomAttribute);

                }
            }
        }

        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        //ngDialog.close($scope.NavigationParameterDialog.id);

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

                    if (strParamValue != undefined && strParamValue != "" && strParamValue.toLowerCase() != strParamField.toLowerCase()) {
                        strParam = strParamField + '=' + strParamValue;
                    }
                    if (strParam != undefined && strParam != "") {
                        if (strReturn == "") {
                            strReturn = strParam;
                        }
                        else {
                            strReturn += ';' + strParam;
                        }
                    }
                }
            }
        });
        return strReturn;
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
                                $.connection.hubForm.server.getEntityQueryColumns($scope.SelectedObject.dictAttributes.sfwQueryID, "NavigationParameterController").done(function (data) {
                                    $scope.receiveQueryColumns(data);
                                });
                            }
                        }
                    }
                }
            }
            else {
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
        }
    };

    $scope.GetSelectedWizardStep = function (aCntrlModel) {
        var retVal = null;
        var parent = aCntrlModel.ParentVM;
        while (null != parent) {
            if (parent.Name == "sfwWizardStep") {
                retVal = parent;
                break;
            }
            parent = parent.ParentVM;
        }
        return retVal;
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
                else if (sfxCtrl.Name == "sfwListView") {
                    strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                    if (!strTreeCaption)
                        strTreeCaption = sfxCtrl.dictAttributes.ID;
                    var obj = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
                    if (sfxCtrl.Elements.length > 0) {
                        $scope.PopulateAvailableFieldsFormLink(strProperty, sfxCtrl, obj.Elements, arrIDs, strPropID);
                    }
                    list.push(obj);
                }

                else if (sfxCtrl.Name == "sfwPanel" || sfxCtrl.Name == "sfwDialogPanel") {
                    strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                    if (!strTreeCaption)
                        strTreeCaption = sfxCtrl.dictAttributes.ID;
                    var obj = { ID: strTreeCaption, Elements: [], IsExpanded: false, IsSelected: false };
                    if (sfxCtrl.Elements.length > 0) {
                        $scope.PopulateAvailableFieldsFormLink(strProperty, sfxCtrl.Elements[0], obj.Elements, arrIDs, strPropID);
                    }

                    list.push(obj);
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
                        if (strFieldName && sfxCtrl.Name != "sfwButton") {
                            list.push({ ID: strFieldName, Elements: [], IsSelected: false, IsExpanded: false });

                        }
                        if (sfxCtrl.Elements.length > 0) {
                            $scope.PopulateAvailableFieldsFormLink(strProperty, sfxCtrl.Elements[0], list[i].Elements, arrIDs, strPropID);
                        }
                        i++;
                    }

                }
            }
        }

        if (sfxTable != null) {
            angular.forEach(sfxTable.Elements, iterator);
        }
    };

    //#endregion

    $scope.Init();
}]);

app.directive("parameterdraggable", [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.draggable = true;

            el.addEventListener('dragstart', onDragStart, false);

            function onDragStart(e) {
                e.stopPropagation();
                if (scope.dragdata != undefined && scope.dragdata != "") {
                    dragDropData = scope.dragdata;
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                }
            }
        }
    };
}]);

app.directive("parameterdroppable", [function () {
    return {
        restrict: 'A',
        scope: {
            dropdata: '=',
        },
        link: function (scope, element, attributes) {
            var el = element[0];

            el.addEventListener("dragover", function (e) {
                e.dataTransfer.dropEffect = 'copy';
                if (e.preventDefault) {
                    e.preventDefault();
                }
            });

            el.addEventListener("drop", function (e) {
                e.preventDefault();

                var strData = e.dataTransfer.getData("Text");
                if (strData == "" && lstEntityTreeFieldData != null) {
                    var obj = lstEntityTreeFieldData;
                    var Id = obj[0];
                    var DisplayName = obj[1];
                    var DataType = obj[2];
                    var data = obj[3];//JSON.parse(obj[3]);
                    var isparentTypeCollection = obj[4];
                    var fullpath = Id;
                    if (DisplayName != "") {
                        fullpath = DisplayName + "." + Id;
                    }
                    if (isparentTypeCollection == "false" && DataType !== "Collection" && DataType !== "List") {
                        if (data != undefined && data != "") {
                            scope.$apply(function () {
                                scope.dropdata = fullpath;
                            });
                        }
                    }
                    lstEntityTreeFieldData = null;
                } else {
                    lstEntityTreeFieldData = null;
                    if (dragDropData) {
                        scope.$apply(function () {
                            scope.dropdata = dragDropData;
                            dragDropData = null;
                        });
                    }
                }


                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }, false);
        }
    };
}]);