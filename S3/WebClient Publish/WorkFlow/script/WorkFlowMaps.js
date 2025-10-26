app.controller("WorkFlowMapsController", ["$scope", "$http", "$rootScope", "ngDialog", "$EntityIntellisenseFactory", "$NavigateToFileService", "$timeout", "$filter", function ($scope, $http, $rootScope, ngDialog, $EntityIntellisenseFactory, $NavigateToFileService, $timeout, $filter) {

    //#region Variables

    $scope.lstValueSource = [' ', 'Constant', 'Object Field', 'Parameter'];
    $scope.isDirty = false;
    $rootScope.IsLoading = true;
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.objProcessDetails;
    $scope.objActivity = {};
    $scope.objActivityForm = {};
    $scope.objWorkFlowFormParams = {};
    $scope.objWorkFlowFormParams.AddParamGridRow = [];
    $scope.objBusinessObject = {};
    $scope.objBusinessObject.ObjTree = undefined;
    $scope.objWorkFlowDetail = {};
    $scope.selectedDesignSource = false;
    $scope.objWorkFlow = {};
    $scope.showRefID = true;
    $scope.IsActivity = true;
    $scope.lstActivityFormName = [];
    $scope.lstBusinessObject = [];



    //#region Object Tree variable

    // Business object tree properties
    $scope.objWorkFlow.SelecetdField = undefined;
    $scope.objBusinessObjectTree = {};
    $scope.objBusinessObjectTree.lstmultipleselectedfields = [];
    $scope.objBusinessObjectTree.lstCurrentBusinessObjectProperties = [];
    $scope.objBusinessObjectTree.lstdisplaybusinessobject = [];
    //#endregion


    //#endregion

    //#region OnLoad
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveWorkflowMapModel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receiveWorkflowMapModel = function (data) {

        $scope.$apply(function () {
            $scope.WorkflowMapModel = data;
            $scope.objFormExtraFields = [];
            $scope.SelectedActivityClick($scope.WorkflowMapModel.Elements[0]);

            for (var i = 0; i < $scope.WorkflowMapModel.Elements.length; i++) {

                if ($scope.WorkflowMapModel.Elements[i].Name == 'ProcessVariables') {
                    $scope.objProcessDetails = $scope.WorkflowMapModel.Elements[i];
                }
                //else if ($scope.WorkflowMapModel.Elements[i].Name == 'activity') {
                //    $scope.objActivityType = $scope.WorkflowMapModel.Elements[i];
                //    for (var j = 0; j < $scope.WorkflowMapModel.Elements[i].Elements.length; j++) {

                //        if ($scope.WorkflowMapModel.Elements[i].Elements[j].Name == 'form') {
                //            if ($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName) {
                //                if ($scope.lstActivityFormName.length == 0) {
                //                    $scope.lstActivityFormName.push($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName);
                //                }
                //                else if (!$scope.lstActivityFormName.some(function (x) { return x == $scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName; }))
                //                    $scope.lstActivityFormName.push($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName);
                //            }
                //        }

                //    }

                //}
            }
            if (!$scope.objProcessDetails) {

                $scope.objProcessDetails = $scope.CreateNewObject("ProcessVariables", $scope.WorkflowMapModel);
            }
        });
        //#region getting Remote Object list
        $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
            $scope.receiveRemoteObjectCollection(data);
        });

        $scope.GetActivityForms = function () {
            var lstActivityFormName = [];
            for (var i = 0; i < $scope.WorkflowMapModel.Elements.length; i++) {

                if ($scope.WorkflowMapModel.Elements[i].Name == 'activity') {
                    for (var j = 0; j < $scope.WorkflowMapModel.Elements[i].Elements.length; j++) {

                        if ($scope.WorkflowMapModel.Elements[i].Elements[j].Name == 'form') {
                            if ($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName) {
                                if (lstActivityFormName.length == 0) {
                                    lstActivityFormName.push($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName);
                                }
                                else if (!lstActivityFormName.some(function (x) { return x == $scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName; }))
                                    lstActivityFormName.push($scope.WorkflowMapModel.Elements[i].Elements[j].dictAttributes.sfwFormName);
                            }
                        }
                    }
                }
            }
            return lstActivityFormName;
        }

        $scope.receiveRemoteObjectCollection = function (data) {
            if (data) {
                $scope.$evalAsync(function () {
                    $scope.objRemoteObjectCollection = data;
                    for (var i = 0; i < $scope.WorkflowMapModel.Elements.length; i++) {
                        if ($scope.WorkflowMapModel.Elements[i].Name == 'activity') {
                            $scope.SelectedActivityClick($scope.WorkflowMapModel.Elements[i]);
                            break;
                        }
                    }
                });
            }
        };
        //#endregion

        //#region Populate correspondence Templates
        $scope.objWorkFlowDetail.PopulateCorrespondenceTemplate();
        //#endregion


        $scope.objWorkflowExtraFields = $filter('filter')($scope.WorkflowMapModel.Elements, {
            Name: 'ExtraFields'
        });
        if ($scope.objWorkflowExtraFields.length > 0) {
            $scope.objWorkflowExtraFields = $scope.objWorkflowExtraFields[0];
            //$scope.removeExtraFieldsDataInToMainModel();
        }

        if ($scope.objWorkflowExtraFields.length == 0) {
            $scope.objWorkflowExtraFields = {
                Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
            };
        }
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });

        $scope.ActiveTab('Activities');


    };

    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };


    //#endregion

    //#region Xml source Start
    $scope.showSource = false;


    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {

            var xmlstring = $("#" + $rootScope.currentopenfile.file.FileName).find("#code-html").val();

            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;

                var txtarea = $("#" + $rootScope.currentopenfile.file.FileName).find("#code-html");
                var lineno = getLineNumber(txtarea);

                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $rootScope.currentopenfile.file, lineno);
                }
                else {

                    var lineNumber = [];
                    if (lineno > 0) {
                        lineNumber[0] = lineno;
                    }
                    else {
                        lineNumber[0] = 1;
                    }
                    var strpacket = "";
                    var lstDataPackets = [];
                    var count = 0;
                    for (var i = 0; i < xmlstring.length; i++) {
                        count++;
                        strpacket = strpacket + xmlstring[i];
                        if (count == 32000) {
                            count = 0;
                            lstDataPackets.push(strpacket);
                            strpacket = "";
                        }
                    }
                    if (count != 0) {
                        lstDataPackets.push(strpacket);
                    }

                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Source-Design", lineNumber);
                }

                $scope.receivedesignxmlobject = function (data, path) {

                    if ($scope.isSourceDirty) {
                        // $scope.constantfilemodel = data;
                        // $scope.getconstants($scope.constantfilemodel);
                        //constantFactory.setConstantsList($scope.constantfilemodel);
                        $scope.isSourceDirty = false;
                    }



                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;

                        if (path != null && path != "") {
                            var items = [];
                            var objHierarchy;
                            if (path.contains("-") || path.contains(",")) {
                                objHierarchy = $scope.WorkflowMapModel;
                                for (var i = 0; i < path.split(',').length; i++) {
                                    objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                                    items.push(objHierarchy);
                                }
                            }

                            if (items && items.length > 0 && items[0] && items[0].Name === 'activity') {
                                $scope.SelectedActivityClick(items[0]);
                            }
                        }
                    });
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;

                        $scope.selectedDesignSource = false;
                    });
                    //$timeout(function () {
                    //    var elem = document.getElementsByClassName("selected")[0];
                    //    if (elem) {
                    //        $('.page-sidebar').scrollTo(elem, null, null);
                    //    }
                    //});
                };
            }
        }
    };
    $scope.isSourceDirty;
    $scope.sourceChanged = function () {
        $scope.isSourceDirty = true;
        $scope.isDirty = true;
        $scope.SearchSource.IsSearchCriteriaChange = true;
    };
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {

            $scope.selectedDesignSource = true;
            var sObj;
            var indexPath = [];
            var pathString;

            if ($scope.WorkflowMapModel) {
                $rootScope.IsLoading = true;
                var objreturn1 = GetBaseModel($scope.WorkflowMapModel);
                var strobj = JSON.stringify(objreturn1);

                var nodeId = [];
                if ($scope.WorkflowMapModel && $scope.selectedCurrentActivityRecord) {


                    var pathToObject = [];
                    var selectedItem = undefined;
                    selectedItem = $scope.selectedCurrentActivityRecord;
                    if ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == 'User' || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == 'Wait') {
                        if ($scope.showRefID == true && $scope.selectedNoRefParamter) {
                            selectedItem = $scope.selectedNoRefParamter;
                        }
                        else if ($scope.showRefID == false && $scope.selectedWithRefParamter) {
                            selectedItem = $scope.selectedWithRefParamter;
                        }
                    }
                    else if ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == 'Code') {
                        selectedItem = $scope.selectedCustomModelParameter;
                    }
                    else if ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == 'GenerateCommunication' || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == 'SendCommunication') {
                        selectedItem = $scope.selectedCustomModelParameter;
                    }
                    sObj = $scope.FindDeepNode($scope.WorkflowMapModel, selectedItem, pathToObject);
                    pathString = $scope.getPathSource($scope.WorkflowMapModel, pathToObject, indexPath);
                    angular.copy(pathString, nodeId);
                }

                if (strobj.length < 32000) {
                    hubMain.server.getSourceXmlObject(strobj, $rootScope.currentopenfile.file, nodeId);
                }
                else {
                    var strpacket = "";
                    var lstDataPackets = [];
                    var count = 0;
                    for (var i = 0; i < strobj.length; i++) {
                        count++;
                        strpacket = strpacket + strobj[i];
                        if (count == 32000) {
                            count = 0;
                            lstDataPackets.push(strpacket);
                            strpacket = "";
                        }
                    }
                    if (count != 0) {
                        lstDataPackets.push(strpacket);
                    }
                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Design-Source", nodeId);
                }
                $scope.receivesourcexml = function (xmlstring, lineno) {
                    $scope.$apply(function () {
                        $scope.xmlSource = xmlstring;
                        $scope.SearchSource = {};
                        $scope.SearchSource.txtarea = $("#" + $rootScope.currentopenfile.file.FileName).find("#code-html");
                        // enabling search shortcuts
                        $scope.SearchSource.txtarea[0].onkeydown = function (event) {
                            if (event.ctrlKey && event.keyCode == 70) {
                                $scope.$evalAsync(function () {
                                    $scope.SearchSource.IsEnable = true;
                                });
                                event.preventDefault();
                            }
                        };
                        $scope.SearchSource.txtarea[0].value = xmlstring;
                        $timeout(function () {
                            selectTextareaLine($scope.SearchSource.txtarea, lineno);
                        });
                        $timeout(function () {
                            scrollTextArea($scope.SearchSource.txtarea, lineno);
                        });
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                        });
                        if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });
                        }
                    });
                };
            }
        }
    };

    $scope.FindNodeHierarchy = function (objParentElements, index) {
        if (objParentElements && objParentElements.Elements) {
            var newObj = objParentElements.Elements[index];
            if (newObj == undefined) {
                newObj = objParentElements.Elements[index - 1];
            }
            return newObj;
        }
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                var isNodeInPath = $scope.isValidObject(item, selectedItem);
                if (isNodeInPath) {
                    pathToObject.push(item);
                }
                if (item == selectedItem) {
                    return selectedItem;
                }
                else if (item.Elements && item.Elements.length > 0) {
                    selectedItem = $scope.FindDeepNode(item, selectedItem, pathToObject);
                    return selectedItem;
                }
            });
        }
        return selectedItem;
    };
    $scope.getPathSource = function (objModel, pathToObject, indexPath) {
        for (var i = 0; i < pathToObject.length; i++) {
            if (i == 0) {
                var indx = objModel.Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
            else {
                var indx = pathToObject[i - 1].Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
        }
        return indexPath;
    };
    $scope.isValidObject = function (objParentElements, selectedItem) {
        var result;
        if (objParentElements == selectedItem) {
            result = true;
            return result;
        }

        for (var ele in objParentElements.Elements) {
            if (objParentElements.Elements[ele] == selectedItem) {
                result = true;
                return result;
            }
            if (objParentElements.Elements[ele].Elements && objParentElements.Elements[ele].Elements.length > 0) {
                for (iele in objParentElements.Elements[ele].Elements) {
                    result = $scope.isValidObject(objParentElements.Elements[ele].Elements[iele], selectedItem);
                    if (result == true) {
                        return result;
                    }
                }
            }
        }
        return result;
    };
    //#endregion

    //#region Extra Fields


    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objWorkflowExtraFields) {
            var index = $scope.WorkflowMapModel.Elements.indexOf($scope.objWorkflowExtraFields);
            if (index == -1) {
                $scope.WorkflowMapModel.Elements.push($scope.objWorkflowExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };
    //#endregion

    //#region Selected Activity Click

    $scope.SelectedActivityClick = function (obj) {
        if (obj) {
            if ($scope.selectedDesignSource && $scope.showRefID == true) {
                $scope.changeTab('noRef');
                $scope.selectedWithRefParamter = undefined;
            }
            else if ($scope.selectedDesignSource && $scope.showRefID == false) {
                $scope.changeTab('withRef');
                $scope.selectedNoRefParamter = undefined;
            }
            else {
                $scope.changeTab('noRef');
                $scope.selectedWithRefParamter = undefined;
            }

            $rootScope.IsLoading = true;
            $scope.selectedCurrentActivityRow = undefined;
            $scope.selectedCurrentActivityRecord = obj;
            $scope.objWithNoRef = undefined;
            $scope.objWithRef = undefined;
            var dummyParams = [];
            var dummyParamsWithRef = [];
            for (var i = 0; i < $scope.selectedCurrentActivityRecord.Elements.length; i++) {
                if ($scope.selectedCurrentActivityRecord.Elements[i].Name == 'form') {
                    if ($scope.selectedCurrentActivityRecord.Elements[i].dictAttributes.sfwMode) {
                        if ($scope.selectedCurrentActivityRecord.Elements[i].dictAttributes.sfwMode == 'new') {
                            $scope.objWithNoRef = $scope.selectedCurrentActivityRecord.Elements[i];
                        }
                        else if ($scope.selectedCurrentActivityRecord.Elements[i].dictAttributes.sfwMode == 'update') {
                            $scope.objWithRef = $scope.selectedCurrentActivityRecord.Elements[i];
                        }
                    } else {
                        $scope.selectedCurrentActivityRecord.Elements.splice(i, 1);
                        i--;
                    }
                }

                else if ($scope.selectedCurrentActivityRecord.Elements[i].Name == 'communication') {

                    $scope.selectedCommunicationRecord = $scope.selectedCurrentActivityRecord.Elements[i];


                }
                else if ($scope.selectedCurrentActivityRecord.Elements[i].Name == 'custommethod') {
                    $scope.customMethodsModel = $scope.selectedCurrentActivityRecord.Elements[i];
                }

            }

            if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "User" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "Wait")) {
                if (!$scope.objWithNoRef) {
                    $scope.objWithNoRef = {
                        Name: 'form', Value: '', dictAttributes: { sfwMode: 'new' }, Elements: [], Children: []
                    };
                    $scope.selectedCurrentActivityRecord.Elements.push($scope.objWithNoRef);
                }

                if ($scope.objWithNoRef && $scope.objWithNoRef.Elements.length > 0) {
                    dummyParams = $scope.CreateDummyParamsList($scope.objWithNoRef.Elements);
                }

                if (!$scope.objWithRef) {
                    $scope.objWithRef = {
                        Name: 'form', Value: '', dictAttributes: { sfwMode: 'update' }, Elements: [], Children: []
                    };
                    $scope.selectedCurrentActivityRecord.Elements.push($scope.objWithRef);
                }

                if ($scope.objWithRef && $scope.objWithRef.Elements.length > 0) {
                    dummyParamsWithRef = $scope.CreateDummyParamsList($scope.objWithRef.Elements);
                }
            }

            if ($scope.selectedCurrentActivityRecord && $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "Code") {
                if (!$scope.customMethodsModel) {
                    $scope.customMethodsModel = {
                        Name: 'custommethod', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                }

                if ($scope.customMethodsModel && $scope.customMethodsModel.Elements.length > 0) {
                    dummyParams = $scope.CreateDummyParamsList($scope.customMethodsModel.Elements);
                }
            }

            if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "SendCommunication" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "GenerateCommunication")) {
                if ($scope.selectedCommunicationRecord && $scope.selectedCommunicationRecord.Elements.length > 0) {
                    dummyParams = $scope.CreateDummyParamsList($scope.selectedCommunicationRecord.Elements);
                }
            }

            $scope.onRemoteObjectChangedInWorkFlow();

            $scope.RetainParamValuesByActivity(dummyParams, dummyParamsWithRef);

            $rootScope.IsLoading = false;
        }
    };

    $scope.CreateDummyParamsList = function (elements) {
        var dummyList = [];
        for (var i = 0; i < elements.length; i++) {
            dummyList.push(elements[i]);
        }
        return dummyList;
    }

    $scope.RetainParamValuesByActivity = function (dummyParams, dummyParamsWithRef) {
        if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "User" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "Wait")) {

            if ($scope.objWithNoRef && $scope.objWithNoRef.Elements.length > 0) {
                $scope.RetainParamValues(dummyParams, $scope.objWithNoRef.Elements);
            }

            if ($scope.objWithRef && $scope.objWithRef.Elements.length > 0) {
                $scope.RetainParamValues(dummyParamsWithRef, $scope.objWithRef.Elements);
            }
        }

        else if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "SendCommunication" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "GenerateCommunication")) {

            if ($scope.selectedCommunicationRecord && $scope.selectedCommunicationRecord.Elements.length > 0) {
                $scope.RetainParamValues(dummyParams, $scope.selectedCommunicationRecord.Elements);
            }
        }
        else {
            if ($scope.customMethodsModel && $scope.customMethodsModel.Elements.length > 0) {
                $scope.RetainParamValues(dummyParams, $scope.customMethodsModel.Elements);
            }
        }
    }

    $scope.RetainParamValues = function (dummyEle, elements) {
        for (var i = 0; i < elements.length; i++) {
            var lst = dummyEle.filter(function (x) { return x.dictAttributes.sfwParamaterName === elements[i].dictAttributes.sfwParamaterName });
            if (lst && lst.length > 0) {
                elements[i].dictAttributes.sfwValueSource = lst[0].dictAttributes.sfwValueSource;
                elements[i].dictAttributes.sfwParameterValue = lst[0].dictAttributes.sfwParameterValue;
            }
        }
    }
    //#endregion

    //#region change of Method Mode Selection

    $scope.methodModeChange = function (methodMode) {
        var strActiveForm = '';


        if ($scope.showRefID) {
            if ($scope.objWithNoRef) {
                $scope.objWithNoRef.Elements = [];
                strActiveForm = $scope.objWithNoRef.dictAttributes.sfwFormName;

            }
        }
        else if (!$scope.showRefID) {
            if ($scope.objWithRef) {
                $scope.objWithRef.Elements = [];
                if ($scope.objWithRef.dictAttributes) {
                    strActiveForm = $scope.objWithRef.dictAttributes.sfwFormName;
                }
            }
        }
        if (strActiveForm) {

            $.connection.hubForm.server.getNewFormModel(strActiveForm).done(function (data) {
                $scope.receivenewformmodel(data);
            });

        }

        $scope.receivenewformmodel = function (data) {
            if (data) {
                $scope.$apply(function () {
                    $rootScope.IsLoading = false;
                    var objSfxForm = data;

                    var istrValue = objSfxForm.dictAttributes.sfwType;
                    var blnIsLookup = istrValue.toUpperCase().trim() == "LOOKUP";
                    var objParamsList = {
                        Name: "parameter", Value: '', dictAttributes: {}, Elements: []
                    };

                    if (blnIsLookup) {
                        var sfxCriteriaPanel = GetCriteriaPanel(objSfxForm.Elements[1]);
                        if (sfxCriteriaPanel) {
                            angular.forEach(sfxCriteriaPanel.Elements, function (sfxRow) {
                                angular.forEach(sfxRow.Elements, function (sfxCell) {
                                    if (sfxCell) {
                                        angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                                            if ("sfwDataField" in sfxCtrl.dictAttributes) {
                                                var strFieldName = sfxCtrl.dictAttributes.sfwDataField;
                                                var dataType = sfxCtrl.dictAttributes.sfwDataType;

                                                if (strFieldName != "" && strFieldName != undefined) {
                                                    var objParamsList = {
                                                        Name: "parameter", Value: '', dictAttributes: { sfwParameterSource: "Criteria Field", sfwParamaterName: strFieldName, sfwDataType: dataType, sfwFieldName: strFieldName, sfwParameterValue: '', sfwValueSource: '' }, Elements: []
                                                    };
                                                    if ($scope.showRefID) {
                                                        $rootScope.PushItem(objParamsList, $scope.objWithNoRef.Elements);
                                                    }
                                                    else {
                                                        $rootScope.PushItem(objParamsList, $scope.objWithRef.Elements);
                                                    }
                                                }
                                            }
                                        });
                                    }
                                });
                            });
                        }
                    }
                    else {

                        var InitialLoadVM;
                        var lst = objSfxForm.Elements.filter(function (x) { return x.Name == "initialload"; });
                        if (lst && lst.length > 0) {
                            InitialLoadVM = lst[0];
                        }

                        if (InitialLoadVM) {
                            /*code commented coz session fields are not coming. */
                            for (var i = 0; i < InitialLoadVM.Elements.length; i++) {
                                //var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || (x.dictAttributes.sfwMode && x.dictAttributes.sfwMode.toLowerCase() == methodMode)); });
                                //if (lst && lst.length) {
                                //  strMethod = lst[0].dictAttributes.sfwMethodName;
                                //}

                                if (InitialLoadVM.Elements[i].Name == "callmethods") {

                                    if (!InitialLoadVM.Elements[i].dictAttributes.sfwMode || (InitialLoadVM.Elements[i].dictAttributes.sfwMode && InitialLoadVM.Elements[i].dictAttributes.sfwMode.toLowerCase() == methodMode)) {
                                        if (InitialLoadVM.Elements[i].dictAttributes.sfwMethodName) {

                                            var strMethod = InitialLoadVM.Elements[i].dictAttributes.sfwMethodName;
                                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                            var objData = entityIntellisenseList.filter(function (x) { return x.EntityName == objSfxForm.dictAttributes.sfwEntity; });
                                            if (objData && objData.length > 0) {
                                                $scope.objBusinessObjectName = objData[0].BusinessObjectName;//filter(function (x) { return x.BusinessObjectName == "BusinessObjectName" });
                                            }
                                            if (objSfxForm.dictAttributes.sfwRemoteObject != undefined && objSfxForm.dictAttributes.sfwRemoteObject != "") {
                                                var objServerObject = GetServerMethodObject(objSfxForm.dictAttributes.sfwRemoteObject, $scope.objRemoteObjectCollection);
                                                var paramerters = GetSrvMethodParameters(objServerObject, strMethod);
                                                if (paramerters) {
                                                    for (j = 0; j < paramerters.length; j++) {
                                                        var objParamsList = {
                                                            Name: "parameter", Value: '', dictAttributes: { sfwParameterSource: strMethod, sfwParamaterName: paramerters[j].dictAttributes.ID, sfwDataType: paramerters[j].dictAttributes.sfwDataType, sfwFieldName: paramerters[j].dictAttributes.ID, sfwParameterValue: '', sfwValueSource: '' }, Elements: []
                                                        };
                                                        if ($scope.showRefID) {
                                                            $rootScope.PushItem(objParamsList, $scope.objWithNoRef.Elements);

                                                        }
                                                        else {

                                                            $rootScope.PushItem(objParamsList, $scope.objWithRef.Elements);
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                var paramerters = GetEntityXMLMethodParameters(entityIntellisenseList, objSfxForm.dictAttributes.sfwEntity, strMethod);
                                                if (paramerters) {
                                                    angular.forEach(paramerters, function (objParam) {
                                                        var objParamsList = {
                                                            Name: "parameter", Value: '', dictAttributes: { sfwParameterSource: strMethod, sfwParamaterName: objParam.ID, sfwDataType: objParam.DataType, sfwFieldName: objParam.ID, sfwParameterValue: '', sfwValueSource: '' }, Elements: []
                                                        };
                                                        if ($scope.showRefID) {
                                                            $rootScope.PushItem(objParamsList, $scope.objWithNoRef.Elements);
                                                        }
                                                        else {
                                                            $rootScope.PushItem(objParamsList, $scope.objWithRef.Elements);
                                                        }
                                                    });
                                                }
                                            }

                                        }

                                    }

                                }
                                else if (InitialLoadVM.Elements[i].Name == "session") {
                                    angular.forEach(InitialLoadVM.Elements[i].Elements, function (objSessionField) {
                                        var objParamsList = {
                                            Name: "parameter", Value: '', dictAttributes: { sfwParameterSource: "Session Field", sfwParamaterName: objSessionField.dictAttributes.ID, sfwDataType: objSessionField.dictAttributes.sfwDataType, sfwFieldName: objSessionField.dictAttributes.ID, sfwParameterValue: '', sfwValueSource: '' }, Elements: []
                                        };
                                        if ($scope.showRefID) {
                                            $rootScope.PushItem(objParamsList, $scope.objWithNoRef.Elements);
                                        }
                                        else {
                                            $rootScope.PushItem(objParamsList, $scope.objWithRef.Elements);
                                        }


                                    });


                                }
                            }

                        }
                    }

                });
            }
        };
    };
    //#endregion

    //#region Tab Change

    $scope.changeTab = function (obj) {
        if (obj && obj == 'noRef') {
            $scope.showRefID = true;

        }
        else if (obj && obj == 'withRef') {
            $scope.showRefID = false;

        }
    };


    //#endregion

    //#region Tab change for Activites & Process Variable
    $scope.ActiveTab = function (obj) {
        $scope.lstBusinessObject = ['busActivityInstance'];
        if (obj && obj == 'Activities') {

            $scope.objBusinessObjectTree.busObjectName = 'busActivityInstance';
            $scope.IsActivity = true;
            $scope.changeTab('noRef');

        }
        else if (obj && obj == 'ProcessVariable') {
            var lstActivityFormName = undefined;
            $scope.lstBusinessObject.push('busProcessInstance');
            lstActivityFormName = $scope.GetActivityForms();
            if (lstActivityFormName.length > 0) {

                $.connection.hubWorkflowMap.server.getRemoteObjectList(lstActivityFormName).done(function (remoteObjectList) {
                    $scope.$evalAsync(function () {
                        if (remoteObjectList) {
                            for (var i = 0; i < remoteObjectList.length; i++) {
                                $scope.lstBusinessObject.push(remoteObjectList[i]);

                            }
                        }
                    });
                });
            }
            $scope.IsActivity = false;

        }
    };


    //#endregion

    //#region Active Form Change

    $scope.OnActiveFormValueChangeInWorkFlow = function () {
        var strActiveForm = "";

        if ($scope.showRefID) {
            if ($scope.objWithNoRef && $scope.objWithNoRef.dictAttributes)
                $scope.methodModeChange($scope.objWithNoRef.dictAttributes.sfwFormMode);
        }

        else if (!$scope.showRefID) {

            $scope.methodModeChange('update');
        }

        // $scope.methodModeChange('update');

    };
    //#endregion

    //#region Population Method on changing Remote object

    $scope.onRemoteObjectChangedInWorkFlow = function () {
        if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "SendCommunication" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "GenerateCommunication")) {

            if ($scope.selectedCommunicationRecord && $scope.selectedCommunicationRecord.dictAttributes && $scope.selectedCommunicationRecord.dictAttributes.sfwRemoteObject) {
                if ($scope.objRemoteObjectCollection) {
                    var lst = $scope.objRemoteObjectCollection.filter(function (itm) {
                        return itm.dictAttributes.ID == $scope.selectedCommunicationRecord.dictAttributes.sfwRemoteObject;
                    });
                    if (lst && lst.length > 0) {
                        $scope.SrvMethodCollection = lst[0].Elements;
                        $scope.SrvMethodCollection.splice(0, 0, { dictAttributes: { ID: '' } });
                    }

                    $scope.getParametersonMethodChange();
                }
            }
        }
        else {
            if ($scope.customMethodsModel && $scope.customMethodsModel.dictAttributes && $scope.customMethodsModel.dictAttributes.sfwRemoteObject) {
                if ($scope.objRemoteObjectCollection) {
                    var lst = $scope.objRemoteObjectCollection.filter(function (itm) {
                        return itm.dictAttributes.ID == $scope.customMethodsModel.dictAttributes.sfwRemoteObject;
                    });
                    if (lst && lst.length > 0) {
                        $scope.SrvMethodCollection = lst[0].Elements;
                        $scope.SrvMethodCollection.splice(0, 0, { dictAttributes: { ID: '' } });
                    }

                    $scope.getParametersonMethodChange();
                }
            }
        }
    };
    //#endregion

    //#region get Parameters on Method change

    $scope.getParametersonMethodChange = function () {

        if ($scope.selectedCurrentActivityRecord && ($scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "SendCommunication" || $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "GenerateCommunication")) {
            $scope.AddParameterToActivity($scope.selectedCommunicationRecord);
        }
        else if ($scope.selectedCurrentActivityRecord && $scope.selectedCurrentActivityRecord.dictAttributes.sfwActivityType == "Code") {
            $scope.AddParameterToActivity($scope.customMethodsModel);
        }
    };

    $scope.AddParameterToActivity = function (activityChildModel) {

        if (activityChildModel && activityChildModel.dictAttributes.sfwRemoteObject && activityChildModel.dictAttributes.sfwMethodName) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(activityChildModel.Elements, activityChildModel, "Elements", []);
            var objServerObject = GetServerMethodObject(activityChildModel.dictAttributes.sfwRemoteObject, $scope.objRemoteObjectCollection);

            var paramerters = GetSrvMethodParameters(objServerObject, activityChildModel.dictAttributes.sfwMethodName);
            if (paramerters) {
                for (j = 0; j < paramerters.length; j++) {


                    var objParamsList = {
                        Name: "parameter", Value: '', dictAttributes: { sfwParamaterName: paramerters[j].dictAttributes.ID, sfwDataType: paramerters[j].dictAttributes.sfwDataType, sfwFieldName: paramerters[j].dictAttributes.ID, sfwParameterValue: '', sfwValueSource: '' }, Elements: []
                    };

                    $rootScope.PushItem(objParamsList, activityChildModel.Elements);
                }
            }
            $rootScope.UndRedoBulkOp("End");

        }

    };

    //#endregion

    //#region Populate Correspondence Template

    $scope.objWorkFlowDetail.PopulateCorrespondenceTemplate = function () {
        $rootScope.IsLoading = true;
        $.connection.hubCreateNewObject.server.loadCorrespondenceTemplate(true).done(function (data) {
            $scope.objWorkFlowDetail.receiveCorrespondenceTemplate(data);
        });
    };

    $scope.objWorkFlowDetail.receiveCorrespondenceTemplate = function (data) {
        $scope.$apply(function () {
            $scope.objWorkFlowDetail.CorTemplateCollection = data;
            $rootScope.IsLoading = false;
        });

    };

    //#endregion

    //#region Navigate to Associate Form Click
    $scope.selectAssociateFormClick = function (activeFromID) {
        if (activeFromID) {
            hubMain.server.navigateToFile(activeFromID, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
    };
    //#endregion

    //#region Click on Process Variable

    $scope.selectProcessVariable = function (obj) {
        if (obj) {
            $scope.selectedProcessVariable = obj;
        }
    };
    //#endregion

    //#region Add process Variable

    $scope.addProcessVariable = function () {
        var processVariable = {
            Name: 'ProcessVariable', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        $rootScope.PushItem(processVariable, $scope.objProcessDetails.Elements, "selectProcessVariable");
    };
    //#endregion

    //#region Delete  Process Variable

    $scope.deleteProcessVariable = function () {
        var Fieldindex = -1;
        if ($scope.selectedProcessVariable) {
            Fieldindex = $scope.objProcessDetails.Elements.indexOf($scope.selectedProcessVariable);
        }
        $rootScope.DeleteItem($scope.selectedProcessVariable, $scope.objProcessDetails.Elements);
        //$scope.SessionFields.Elements.splice(Fieldindex, 1);
        $scope.selectedProcessVariable = undefined;

        if (Fieldindex < $scope.objProcessDetails.Elements.length) {

            $scope.selectedProcessVariable = $scope.objProcessDetails.Elements[Fieldindex];
        }
        else if ($scope.objProcessDetails.Elements.length > 0) {
            $scope.selectedProcessVariable = $scope.objProcessDetails.Elements[Fieldindex - 1];
        }

    };

    // disable if there is no element for SFW row
    $scope.canDeleteRow = function () {
        if ($scope.selectedProcessVariable) {
            return true;
        }
        else {
            return false;
        }
    };
    //#endregion

    //#region No Reference

    //#region click on No Reference Row

    $scope.selectNoRefParamter = function (obj) {
        if (obj) {
            $scope.selectedNoRefParamter = obj
        }
    };
    //#endregion
    //#endregion

    //#region With Reference

    //#region click on With Reference Row

    $scope.selectWithRefParamter = function (obj) {
        if (obj) {
            $scope.selectedWithRefParamter = obj;
        }
    };
    //#endregion
    //#endregion

    //#region Workflow Details Window

    $scope.onDetailClick = function () {

        var newScope = $scope.$new();
        newScope.objWorkflowExtraFields = $scope.objWorkflowExtraFields;
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Workflow";

        if (newScope.objDirFunctions.getExtraFieldData) {
            newScope.objWorkflowExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
        }
        newScope.dialog = $rootScope.showDialog(newScope, "WorkflowMap Details", "WorkFlow/views/WorkflowMapDetails.html", { width: 600, height: 600 });
        //newScope.OnSelectDetailTab = function (tabName) {
        //  newScope.selectedDetailTab = tabName;

        //  if (tabName == 'ExtraFields') {

        //      if (!newScope.IsExtraFieldTabSelected) {
        //          newScope.IsExtraFieldTabSelected = true;
        //      }
        //  }
        //  };

        newScope.OkClick = function () {
            if (newScope.dialog) {
                if (newScope.objDirFunctions.prepareExtraFieldData) {
                    newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                }

                newScope.dialog.close();
            }
        };



    };

    //#endregion

    //#region click on  Custom Model parameters

    $scope.selectCustomModelParamter = function (obj) {
        if (obj) {
            $scope.selectedCustomModelParameter = obj;
        }
    };
    //#endregion

    //#region On Change business Object
    $scope.onChangeBusinessObject = function (busObject) {
        if (busObject) {
            $scope.objBusinessObjectTree.busObjectName = busObject;
        }
    };
    //#endregion

    //#region Refresh Map

    $scope.RefreshWorkflowMap = function () {

        $rootScope.isLoading = true;
        $.connection.hubWorkflowMap.server.refreshMap($scope.WorkflowMapModel.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    var alNewActivities = data;
                    for (var iActivity = 0; iActivity < alNewActivities.length; iActivity++) {
                        var sfxNewWorkflowActivity = alNewActivities[iActivity];
                        var strActivityType = sfxNewWorkflowActivity.dictAttributes.sfwActivityType;

                        angular.forEach($scope.WorkflowMapModel.Elements, function (sfxOldWorkflowActivity) {
                            if (sfxNewWorkflowActivity.dictAttributes.sfwDisplayName == sfxOldWorkflowActivity.dictAttributes.sfwDisplayName) {
                                sfxOldWorkflowActivity.dictAttributes.sfwActivityType = strActivityType;
                                alNewActivities[iActivity] = sfxOldWorkflowActivity;

                            }

                        });

                    }
                    var lst = $scope.WorkflowMapModel.Elements.filter(function (itm) { return itm.Name == "activity"; });
                    while (lst.length > 0) {
                        var index = lst.indexOf(lst[0]);
                        $scope.WorkflowMapModel.Elements.splice(index, 1);
                        lst.splice(0, 1);
                    }

                    for (var i = 0; i < alNewActivities.length; i++) {
                        $scope.WorkflowMapModel.Elements.push(alNewActivities[i]);

                    }
                }

                $rootScope.isLoading = false;
                toastr.success("Map Refreshed.");

            });
        });

    };
    //#endregion  


    $scope.ResetValues = function (obj) {
        if (obj) {
            obj.dictAttributes.sfwParameterValue = '';
        }

    };
}]);