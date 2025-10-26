app.controller("CorrespondenceController", ["$scope", "$http", "$rootScope", "ngDialog", "$EntityIntellisenseFactory", "$NavigateToFileService", "CONSTANTS", "$ValidationService", "$Errors", "$filter", "$timeout", "$GetEntityFieldObjectService", "$Entityintellisenseservice", "ConfigurationFactory", function ($scope, $http, $rootScope, ngDialog, $EntityIntellisenseFactory, $NavigateToFileService, CONST, $ValidationService, $Errors, $filter, $timeout, $GetEntityFieldObjectService, $Entityintellisenseservice, ConfigurationFactory) {

    $scope.UserDataFielddatatypes = [{ key: '', value: '' },
    { key: 'int', value: 'Int' },
    { key: 'boolean', value: 'Boolean' },
    { key: 'decimal', value: 'Decimal' },
    { key: 'datetime', value: 'Datetime' },
    { key: 'sfwhtmltext', value: 'Html Text' }];
    $scope.datatypes = CONST.CORRESPONDENCEDATATYPES;
    $scope.dataformats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];
    $scope.ValueSeparators = ['', 'nl'];

    //  $rootScope.IsLoading = true;
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.SelectedCorrespondenceTab = "DataFields";
    $scope.DataFieldIsItemSelected = true;
    $scope.entityTreeName = "";
    $scope.lstTemplates = [];
    $scope.objCorrespondenceDetails = {};
    $scope.SelectedEntityObjectTreeFields = [];
    $scope.objCorrespondenceDetails.LstDisplayedEntities = [];
    $scope.objCorrespondenceDetails.SelecetdField = [];
    $scope.templatetypes = [];
    $scope.templatetypes.push("Entity", "Collection");
    $scope.CorTemplateCollection = [];
    $scope.parameterList = [];
    $scope.lstControl = CONST.FORM.CONTROL_TYPES;
    $scope.ilstXmlMethods = [];
    $scope.ilstAssociatedForms = [];

    //$scope.objCorrespondenceDetails.SelectedControl = [];
    // sending server call to get Correspondence details
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveCorrespondencemodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }
    });

    // used for find in design - does not support excel matrix
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        if (items[0] && items[0].Name) {
            if (items[0].Name == "sfwFieldBookMarks") {
                if (items[items.length - 1].Name == "sfwFieldBookMark") {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
            }
            if (items[0].Name == "sfwTableBookMarks") {
                if (items[items.length - 1].Name == "sfwTableBookMark") {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
                if (items[items.length - 1].Name == "sfwColumn") {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
            }
            if (items[0].Name == "sfwQueryBookMarks" || items[0].Name == "sfwQueryForm") {
                for (var i = 0; i < items.length; i++) {
                    if (i == items.length - 1 && (items[items.length - 1].Name != "sfwQueryBookMarks" && items[items.length - 1].Name != "sfwQueryForm" && items[items.length - 1].Name != "sfwRow" && items[items.length - 1].Name != "sfwTable")) {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    }
                }
            }
            if (items[0].Name == "sfwChildTemplateBookmarks") {
                if (items[items.length - 1].Name == "sfwChildTemplateBookmark") {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
            }
            if (items[0].Name == "sfwRuleBookmarks") {
                if (items[items.length - 1].Name == "sfwRuleBookmark") {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
            }
        }
    };
    $scope.traverseMainModel = function (path) {
        var items = [], objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.objCorrespondenceDetails;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                items.push(objHierarchy);
            }
        }
        return items;
    };
    $scope.selectElement = function (path) {
        if (path != null && path != "form" && path != "") {
            var items = $scope.traverseMainModel(path);
            var idSelectedDiv = ""; // for scrolling the selected item to view
            if (items.length > 0) {
                $scope.ActiveTabForCorr = 'Entity';
                if (items[0].Name == "sfwFieldBookMarks") {
                    $scope.selectDataFieldsTab();
                    if (items[items.length - 1].Name == "sfwFieldBookMark") {
                        $scope.SelectedFieldBookMarkClick(items[items.length - 1]);
                    }
                    idSelectedDiv = "correspondence-FieldBookMark-tab-content";
                }
                if (items[0].Name == "sfwTableBookMarks") {
                    $scope.objTableBookMarks.Elements.forEach(function (objTL) {
                        objTL.IsExpanded = false;
                    });
                    $scope.selectTablesTab();
                    if (items[items.length - 1].Name == "sfwTableBookMark") {
                        $scope.SelectedTableBookMarksClick(items[items.length - 1]);
                    }
                    if (items[items.length - 1].Name == "sfwColumn") {
                        $scope.SelectedTableBookMarksClick(items[items.length - 2]);
                        $scope.selectedColumnClick(items[items.length - 2].Elements.indexOf(items[items.length - 1]), items[items.length - 1]);
                    }
                    idSelectedDiv = "correspondence-TableBookMark-tab-content";
                }
                if (items[0].Name == "sfwRepeaters") {
                    $scope.objRepeaterBookMarks.Elements.forEach(function (objTL) {
                        objTL.IsExpanded = false;
                    });
                    $scope.selectRepeaterControlTab();
                    if (items[items.length - 1].Name == "sfwRepeater") {
                        $scope.SelectedRepeaterBookMarksClick(items[items.length - 1]);
                    }
                    if (items[items.length - 1].Name == "sfwFieldBookMark") {
                        $scope.SelectedRepeaterBookMarksClick(items[items.length - 2]);
                        $scope.selectedColumnClick(items[items.length - 2].Elements.indexOf(items[items.length - 1]), items[items.length - 1]);
                    }
                    idSelectedDiv = "correspondence-RepeaterControlBookMark-tab-content";
                }
                if (items[0].Name == "sfwQueryBookMarks" || items[0].Name == "sfwQueryForm") {
                    $scope.ActiveTabForCorr = 'Items';
                    $scope.selectUserFieldsTab();
                    for (var i = 0; i < items.length; i++) {
                        if (i == items.length - 1 && (items[items.length - 1].Name != "sfwQueryBookMarks" && items[items.length - 1].Name != "sfwQueryForm" && items[items.length - 1].Name != "sfwRow" && items[items.length - 1].Name != "sfwTable")) {
                            SetFormSelectedControl($scope.objCorrespondenceDetails, items[items.length - 1], window.event);
                            if (items[items.length - 1].Name == "sfwQueryBookMark") {
                                $scope.selectedQueryBookMark = items[items.length - 1];
                            }
                        }
                    }
                    idSelectedDiv = "correspondence-QueryBookMark-tab-content";
                }
                if (items[0].Name == "sfwChildTemplateBookmarks") {
                    $scope.selectTemplatesTab();
                    if (items[items.length - 1].Name == "sfwChildTemplateBookmark") {
                        $scope.SelectedTemplateClick(items[items.length - 1]);
                    }
                    idSelectedDiv = "correspondence-TemplateBookMark-tab-content";
                }

                if (items[0].Name == "sfwRuleBookmarks") {
                    $scope.selectRuleTab();
                    if (items[items.length - 1].Name == "sfwRuleBookmark") {
                        $scope.SelectedRuleBookMarkClick(items[items.length - 1]);
                    }
                    idSelectedDiv = "correspondence-Rule-tab-content";
                }
                // for scrolling to the selected node.
                // for attributes
                if (items.length > 1) {
                    setTimeout(function () {
                        if (idSelectedDiv) {
                            var elem = $("#" + $scope.currentfile.FileName + " #" + idSelectedDiv).find(".selected");
                            if (elem && path != null && path != "file" && path != "") {
                                if (elem.length > 0) {
                                    elem[0].scrollIntoView();
                                }
                            }
                        }
                    }, 500);
                }
            }
        } else if (path && path == "form") {
            $scope.ActiveTabForCorr = 'Entity';
        }
        $rootScope.IsLoading = false;
    };
    //#region Xml source Start
    $scope.selectedDesignSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            //$scope.selectedDesignSource = false;
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                //$scope.removeExtraFieldsDataInToMainModel();
                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;

                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $scope.currentfile, lineno);
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

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Source-Design", lineNumber);
                }
                $scope.receiveformcorrdesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receiveCorrespondencemodel(data);
                        $scope.isSourceDirty = false;
                    }
                    // navigate and highlight the node (path)
                    $scope.$evalAsync($scope.selectElement(path));
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
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeID) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeID);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            var sObj;
            var indexPath = [];
            var pathString;

            if ($scope.objCorrespondenceDetails != null && $scope.objCorrespondenceDetails != undefined) {
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();
                var objreturn1 = GetBaseModel($scope.objCorrespondenceDetails);
                if (objreturn1 != "") {

                    var strobj = JSON.stringify(objreturn1);
                    var nodeId = [];
                    var selectedCorrespondenceItem;

                    if ($scope.selectedFieldBookMark && $scope.SelectedCorrespondenceTab == 'DataFields') {
                        selectedCorrespondenceItem = $scope.selectedFieldBookMark;
                    }
                    else if ($scope.selectedTableBookMark && $scope.SelectedCorrespondenceTab == 'Tables') {
                        if ($scope.selectedSfwColumn == undefined) {
                            selectedCorrespondenceItem = $scope.selectedTableBookMark;
                        }
                        else {
                            selectedCorrespondenceItem = $scope.selectedSfwColumn;
                        }
                    }
                    else if ($scope.selectedRepeaterBookMark && $scope.SelectedCorrespondenceTab == 'RepeaterControl') {
                        if ($scope.selectedSfwColumn == undefined) {
                            selectedCorrespondenceItem = $scope.selectedRepeaterBookMark;
                        }
                        else {
                            selectedCorrespondenceItem = $scope.selectedSfwColumn;
                        }
                    }
                    else if ($scope.SelectedCorrespondenceTab == 'UserFields' && $scope.objCorrespondenceDetails.SelectedControl != undefined && $scope.objCorrespondenceDetails.SelectedControl.IsSelected) {
                        selectedCorrespondenceItem = $scope.objCorrespondenceDetails.SelectedControl;
                    }
                    else if ($scope.selectedTemplateBookMark && $scope.SelectedCorrespondenceTab == 'Templates' && $scope.ActiveTabForCorr == 'Items') {
                        selectedCorrespondenceItem = $scope.selectedTemplateBookMark;
                    }
                    else if ($scope.selectedTemplateBookMark && $scope.SelectedCorrespondenceTab == 'VisibleRule') {
                        selectedCorrespondenceItem = $scope.selectedRuleBookMark;
                    }
                    if (selectedCorrespondenceItem) {
                        //sObj = FindDeepNode($scope.objCorrespondenceDetails, selectedCorrespondenceItem);
                        //pathString = getPathSource(sObj, indexPath);
                        //angular.copy(pathString.reverse(), nodeId);

                        var pathToObject = [];

                        sObj = $scope.FindDeepNode($scope.objCorrespondenceDetails, selectedCorrespondenceItem, pathToObject);
                        pathString = $scope.getPathSource($scope.objCorrespondenceDetails, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }

                    if (strobj.length < 32000) {
                        hubMain.server.getSourceXmlObject(strobj, $scope.currentfile, nodeId);
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
                        SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Design-Source", nodeId);
                    }
                    $scope.receivesourcexml = function (xmlstring, lineno) {
                        $scope.$apply(function () {
                            $scope.xmlSource = xmlstring;
                            var ID = $scope.currentfile.FileName;
                            setDataToEditor($scope, xmlstring, lineno, ID);
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                                //$scope.selectedFieldBookMark = undefined;
                                //$scope.selectedTableBookMark = undefined;
                                //$scope.selectedSfwColumn = undefined;
                                // $scope.selectedColumn = undefined;
                                $scope.SelectedCorrespondenceTab = 'DataFields';
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
    //#endregion xml Source End

    //#region recieve model

    $scope.receiveCorrespondencemodel = function (data) {
        $scope.$apply(function () {
            $scope.objCorrespondenceExtraFields = [];
            $scope.objCorrespondenceDetails = data;
            $scope.objCorrespondenceDetails.RemoteObjectCollection = [];
            $scope.headerID = $scope.objCorrespondenceDetails.dictAttributes.ID;
            if ($scope.objCorrespondenceDetails.dictAttributes.sfwEntity) {
                $scope.PopulateXmlMethodInCors($scope.objCorrespondenceDetails.dictAttributes.sfwEntity);
                $scope.populateXmlMethods();
            }
            $scope.entityTreeName = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
            for (var i = 0; i < $scope.objCorrespondenceDetails.Elements.length; i++) {
                if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwAssociatedForms') {
                    $scope.objAssociatedForms = $scope.objCorrespondenceDetails.Elements[i];
                    if ($scope.objAssociatedForms.Elements.length > 0) {
                        $scope.blnAssociatedFormPresent = true;
                    }
                    else {
                        $scope.blnAssociatedFormPresent = false;
                    }

                }

                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwFieldBookMarks') {
                    $scope.objFieldBookMarks = $scope.objCorrespondenceDetails.Elements[i];

                    if ($scope.objFieldBookMarks.Elements.length > 0) {
                        $scope.SelectedFieldBookMarkClick($scope.objFieldBookMarks.Elements[0]);
                    }

                }

                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwQueryBookMarks') {
                    $scope.objQueryBookMarks = $scope.objCorrespondenceDetails.Elements[i];

                    $scope.SelectedQueryClick($scope.objQueryBookMarks.Elements[0]);

                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwTableBookMarks') {
                    $scope.objTableBookMarks = $scope.objCorrespondenceDetails.Elements[i];
                    if ($scope.objTableBookMarks.Elements.length > 0) {
                        $scope.SelectedTableBookMarksClick($scope.objTableBookMarks.Elements[0]);
                    }

                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwRepeaters') {
                    $scope.objRepeaterBookMarks = $scope.objCorrespondenceDetails.Elements[i];
                    if ($scope.objRepeaterBookMarks.Elements.length > 0) {
                        $scope.SelectedRepeaterBookMarksClick($scope.objRepeaterBookMarks.Elements[0]);
                    }

                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwQueryForm') {
                    $scope.objQueryForm = $scope.objCorrespondenceDetails.Elements[i];

                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwChildTemplateBookmarks') {
                    $scope.objChildTemplateBookmarks = $scope.objCorrespondenceDetails.Elements[i];
                    if ($scope.objChildTemplateBookmarks.Elements.length > 0) {
                        $scope.SelectedTemplateClick($scope.objChildTemplateBookmarks.Elements[0]);
                    }
                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwCorrespondenceTests') {
                    $scope.objCorrespondenceTests = $scope.objCorrespondenceDetails.Elements[i];
                    $scope.setParentVM($scope.objCorrespondenceDetails.Elements[i]);


                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'sfwRuleBookmarks') {
                    $scope.objRuleBookmarks = $scope.objCorrespondenceDetails.Elements[i];
                    $scope.setParentVM($scope.objCorrespondenceDetails.Elements[i]);


                }
                else if ($scope.objCorrespondenceDetails.Elements[i].Name == 'ExtraFields') {
                    $scope.objCorrespondenceExtraFields = $scope.objCorrespondenceDetails.Elements[i];
                }

            }
            $scope.populateAssociatedForms();
            //$scope.removeExtraFieldsDataInToMainModel();

            //========================= adding elements when there is no element in Entity
            if ($scope.objAssociatedForms == undefined) {
                $scope.objAssociatedForms = $scope.CreateNewObject("sfwAssociatedForms", $scope.objCorrespondenceDetails);
            }
            if ($scope.objFieldBookMarks == undefined) {
                $scope.objFieldBookMarks = $scope.CreateNewObject("sfwFieldBookMarks", $scope.objCorrespondenceDetails);
            }
            if ($scope.objQueryBookMarks == undefined) {
                $scope.objQueryBookMarks = $scope.CreateNewObject("sfwQueryBookMarks", $scope.objCorrespondenceDetails);
            }
            if ($scope.objTableBookMarks == undefined) {
                $scope.objTableBookMarks = $scope.CreateNewObject("sfwTableBookMarks", $scope.objCorrespondenceDetails);
            }
            if ($scope.objRepeaterBookMarks == undefined) {
                $scope.objRepeaterBookMarks = $scope.CreateNewObject("sfwRepeaters", $scope.objCorrespondenceDetails);
            }
            if ($scope.objQueryForm == undefined) {
                $scope.objQueryForm = $scope.CreateNewObject("sfwQueryForm", $scope.objCorrespondenceDetails);
            }
            if ($scope.objRuleBookmarks == undefined) {
                $scope.objRuleBookmarks = $scope.CreateNewObject("sfwRuleBookmarks", $scope.objCorrespondenceDetails);
            }
            if ($scope.objCorrespondenceTests == undefined) {
                $scope.objCorrespondenceTests = $scope.CreateNewObject("sfwCorrespondenceTests", $scope.objCorrespondenceDetails);
            }
            if ($scope.objCorrespondenceExtraFields.length == 0) {
                $scope.objCorrespondenceExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                };
            }
            else {
                if ($scope.objQueryForm.Elements && $scope.objQueryForm.Elements.length > 0) {
                    $scope.objCorrespondenceDetails.SelectedControl = $scope.objQueryForm.Elements[0];
                }
            }

            if ($scope.objChildTemplateBookmarks == undefined) {
                $scope.objChildTemplateBookmarks = $scope.CreateNewObject("sfwChildTemplateBookmarks", $scope.objCorrespondenceDetails);
            }

            // store validation object in current scope
            $scope.validationErrorList = [];
            if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
                var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName });
                if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: $scope.currentfile.FileName, errorList: [] });
                var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];
                $scope.validationErrorList = fileErrObj.errorList = [];

            }

            $rootScope.IsLoading = false;

            $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                $scope.$evalAsync(function () {
                    if (data) {
                        $scope.objCorrespondenceDetails.RemoteObjectCollection = data;
                        if ($scope.objCorrespondenceDetails.RemoteObjectCollection && $scope.objCorrespondenceDetails.RemoteObjectCollection.length > 0) {
                            $scope.objCorrespondenceDetails.RemoteObjectCollection.splice(0, 0, {
                                dictAttributes: {
                                    ID: ""
                                }
                            });
                        }
                    }
                });
            });
        });

    };

    //#endregion
    $scope.setParentVM = function (parent) {
        if (parent) {
            angular.forEach(parent.Elements, function (currentelement) {
                currentelement.ParentVM = [];
                currentelement.ParentVM = parent;
                $scope.setParentVM(currentelement);
            });
        }
    };
    var createValidationData = function () {
        $scope.lstEntityTemplate = {};
        $scope.parameterList = [];
        $scope.lstEntityTemplate[$scope.objCorrespondenceDetails.dictAttributes.sfwEntity] = $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate;
        traverseModelAndGetEntities($scope.objChildTemplateBookmarks, $scope.objCorrespondenceDetails.dictAttributes.sfwEntity);
        if ($scope.CorTemplateCollection && $scope.CorTemplateCollection.length <= 0) {
            $scope.PopulateCorrespondenceTemplate();
        }
        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.objGlobleParametersList(data);
                }
            });
        });
        $.connection.hubMain.server.getCorrespondenceValidationData($scope.lstEntityTemplate, $scope.objCorrespondenceDetails.dictAttributes.sfwEntity).done(function (data) {
            //  console.log(data);
            $scope.validationData = data;
            $scope.resourceList = $ValidationService.getListByPropertyName($scope.validationData.Resources, "ResourceID", false);
            $scope.codeGroupList = $scope.validationData.CodeGroup;
            createRuledata($scope.validationData[$scope.objCorrespondenceDetails.dictAttributes.sfwEntity + "_rule"]);
            $scope.objEntityExtraData.lstHardErrors = $scope.createValidationRuleList($scope.validationData[$scope.objCorrespondenceDetails.dictAttributes.sfwEntity + "_rule"], false, false);
            //start validattion
            if ($scope.objCorrespondenceDetails) {
                if ($scope.objCorrespondenceDetails.dictAttributes.hasOwnProperty('sfwEntity')) {
                    $ValidationService.validateEntity($scope.objCorrespondenceDetails, undefined);
                }

                if ($scope.objCorrespondenceDetails.dictAttributes.hasOwnProperty('sfwResource') && $scope.objCorrespondenceDetails.dictAttributes.sfwResource && $scope.objCorrespondenceDetails.dictAttributes.sfwResource != 0) {
                    $ValidationService.checkValidListValue($scope.resourceList, $scope.objCorrespondenceDetails, $scope.objCorrespondenceDetails.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, $scope.validationErrorList);
                }
                if ($scope.objCorrespondenceDetails.dictAttributes.hasOwnProperty("sfwFooterTemplate") && $scope.objCorrespondenceDetails.dictAttributes.sfwFooterTemplate) {
                    $ValidationService.checkValidListValue($scope.CorTemplateCollection, $scope.objCorrespondenceDetails, $scope.objCorrespondenceDetails.dictAttributes.sfwFooterTemplate, "sfwFooterTemplate", "sfwFooterTemplate", CONST.VALIDATION.INVALID_TEMPLATE, $scope.validationErrorList);
                }
                if ($scope.objCorrespondenceDetails.dictAttributes.hasOwnProperty("sfwHeaderTemplate") && $scope.objCorrespondenceDetails.dictAttributes.sfwHeaderTemplate) {
                    $ValidationService.checkValidListValue($scope.CorTemplateCollection, $scope.objCorrespondenceDetails, $scope.objCorrespondenceDetails.dictAttributes.sfwHeaderTemplate, "sfwHeaderTemplate", "sfwHeaderTemplate", CONST.VALIDATION.INVALID_TEMPLATE, $scope.validationErrorList);
                }
                if ($scope.objCorrespondenceDetails.dictAttributes.hasOwnProperty("sfwConditionalRule") && $scope.objCorrespondenceDetails.dictAttributes.sfwConditionalRule) {
                    $ValidationService.checkValidListValue($scope.objEntityExtraData.lstRules, $scope.objCorrespondenceDetails, $scope.objCorrespondenceDetails.dictAttributes.sfwConditionalRule, "sfwConditionalRule", "sfwConditionalRule", CONST.VALIDATION.CONDITIONAL_RULE_NOT_EXISTS, $scope.validationErrorList);
                }
            }
            if ($scope.objQueryForm) {
                $scope.iterateModel($scope.objQueryForm);
            }
            if ($scope.objCorrespondenceDetails) {
                validateForm($scope.objCorrespondenceDetails, $scope.objCorrespondenceDetails.dictAttributes.sfwEntity, $scope.objEntityExtraData);
            }

            $timeout(function () {
                $("#validation-btn").prop("disabled", false);
            });
            $rootScope.IsLoading = false;
            // end validation
        });

    };
    var traverseModelAndGetEntities = function (model, entityid) {
        angular.forEach(model.Elements, function (obj) {
            var entity = getEntityName(obj.dictAttributes.sfwEntityField, entityid, obj);
            if (entity && !$scope.lstEntityTemplate.hasOwnProperty(entity)) {
                var template = obj.dictAttributes.sfwTemplate ? obj.dictAttributes.sfwTemplate : $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate;
                $scope.lstEntityTemplate[entity] = template;
            }
            if (obj.Elements && obj.Elements.length > 0) {
                traverseModelAndGetEntities(obj, entityid);
            }
        });
    };
    $scope.objGlobleParametersList = function (data) {
        angular.forEach(data.Elements, function (paramObj) {
            if (paramObj && paramObj.dictAttributes.ID) {
                var item = "~" + paramObj.dictAttributes.ID;
                $scope.parameterList.push(item);
            }
        });
    };
    $scope.createValidationRuleList = function (objExtraData, isWizard, hardErrGroup) {
        var list = [];
        if (objExtraData && objExtraData.lstHardErrorList) {
            var hardErrorModel = objExtraData.lstHardErrorList[0];
            if (hardErrorModel && hardErrorModel.Elements.length > 0) {

                angular.forEach(hardErrorModel.Elements, function (item) {
                    if (item != undefined && item.dictAttributes.ID != "") {
                        list.push(item.dictAttributes.ID);
                    }
                });
            }
        }
        return list;
    };

    var getMethodList = function (entityid, showonlycollection, showonlyobject, isXmlMethod) {
        var lst = [];
        if (isXmlMethod) {
            lst = $Entityintellisenseservice.GetIntellisenseData(entityid, "", "", true, false, false, false, false, true);
        } else {
            lst = $Entityintellisenseservice.GetIntellisenseData(entityid, "", "", true, false, true, false, false, false);
        }
        var resultList = [];
        if (lst && lst.length > 0) {
            if (showonlycollection) {
                lst = $filter("filter")(lst, { ReturnType: "Collection" });
            }
            else if (showonlyobject) {
                lst = $filter("filter")(lst, { ReturnType: "Object" });
            }
            if (lst && lst.length > 0) {
                resultList = $ValidationService.getListByPropertyName(lst, "ID", false);
            }
        }
        return resultList;
    };

    $scope.validateFileData = function () {

        var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];

        $scope.validationErrorList = fileErrObj.errorList = [];
        $rootScope.IsLoading = true;
        createValidationData();
    };

    $scope.iterateModel = function (model) {
        angular.forEach(model.Elements, function (obj) {
            if (CONST.FORM.IGNORE_NODES.indexOf(obj.Name) <= -1) {
                $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
                $ValidationService.checkDuplicateId(obj, $scope.objQueryForm, $scope.validationErrorList, false, CONST.FORM.NODES);
            }
            if (obj.Elements && obj.Elements.length > 0 && (CONST.FORM.NODES.indexOf(obj.Name) > -1)) {
                $scope.iterateModel(obj);
            }
        });
    };
    $scope.objEntityExtraData = {};
    var createRuledata = function (entExtraData) {
        $scope.objEntityExtraData.lstRules = PopulateEntityRules(entExtraData, false, false);
    };

    var validateForm = function (mainModel, entityid, extraData) {
        angular.forEach(mainModel.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty('sfwVisibleRule') && obj.dictAttributes.sfwVisibleRule) {
                $ValidationService.checkValidListValue(extraData.lstRules, obj, undefined, 'sfwVisibleRule', "invalid_visible_rule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwEnableRule') && obj.dictAttributes.sfwEnableRule) {
                $ValidationService.checkValidListValue(extraData.lstRules, obj, undefined, 'sfwEnableRule', "invalid_enable_rule", CONST.VALIDATION.ENABLE_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwReadOnlyRule') && obj.dictAttributes.sfwReadOnlyRule) {
                $ValidationService.checkValidListValue(extraData.lstRules, obj, undefined, 'sfwReadOnlyRule', "invalid_readonly_rule", CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.Name == 'sfwTableBookMark' && obj.dictAttributes.hasOwnProperty('sfwEntityField') && obj.dictAttributes.sfwEntityField) {
                var attrType = 'Object,Collection,List';// { "one2one": true, "one2many": true, "columns": false, "cdocollection": false, "expression": false };
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityid, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwColumnHeader') && obj.Name == "sfwColumn") {
                var attrType = '';//{ "one2one": false, "one2many": false, "columns": false, "cdocollection": false, "expression": false };
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityid, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, true, attrType);
            }
            if (obj.Name == "sfwFieldBookMark" && obj.dictAttributes.hasOwnProperty('sfwEntityField') && obj.dictAttributes.sfwEntityField) {
                var attrType = '';
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityid, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
            }
            if (obj.Name == 'sfwChildTemplateBookmark' && obj.dictAttributes.hasOwnProperty('sfwEntityField') && obj.dictAttributes.sfwEntityField) {
                var attrType = '';
                if (obj.dictAttributes.sfwChildTemplateType == "Entity") attrType = 'Object';//{ "one2one": true, "one2many": false, "columns": false, "cdocollection": false, "expression": false };
                else if (obj.dictAttributes.sfwChildTemplateType == "Collection") attrType = 'Collection,List';// { "one2one": true, "one2many": true, "columns": false, "cdocollection": false, "expression": false };
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityid, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwXmlMethod') && obj.dictAttributes.sfwXmlMethod) {
                var methodList = getMethodList(entityid, false, false, true);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwXmlMethod, 'sfwXmlMethod', "invalid_method_name", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalQuery') && obj.dictAttributes.sfwRetrievalQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwRetrievalQuery, undefined, "sfwRetrievalQuery", "sfwRetrievalQuery", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalMethod') && obj.dictAttributes.sfwRetrievalMethod) {
                var methodList = getMethodList(entityid, false, true);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwRetrievalMethod, "sfwRetrievalMethod", "invalid_retrieval_method", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwResource') && obj.dictAttributes.sfwResource && obj.dictAttributes.sfwResource != 0) {
                $ValidationService.checkValidListValue($scope.resourceList, obj, obj.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwValidationRules') && obj.dictAttributes.sfwValidationRules) {
                $ValidationService.checkMultipleValueWithList(extraData.lstHardErrors, obj, obj.dictAttributes.sfwValidationRules, ";", 'sfwValidationRules', "invalid_validation_rule", CONST.VALIDATION.VALIDATION_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwTemplateName') && obj.dictAttributes.sfwTemplateName) {
                $scope.validateTemplateName(obj, "onload");
            }
            if (obj.dictAttributes.hasOwnProperty('sfwLoadSource') && obj.dictAttributes.sfwLoadSource) {
                if (obj.dictAttributes.sfwLoadType == "CodeGroup") {
                    var list = $ValidationService.getListByPropertyName($scope.codeGroupList, "CodeID");
                    list.push("0");
                    $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, $scope.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "Query") {
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwLoadSource, undefined, "sfwLoadSource", "sfwLoadSource", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "Method") {
                    var methodList = getMethodList(entityid, true, false);
                    $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_method", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "ServerMethod") {
                    validateServerMethod(obj);
                }
            }

            if (obj.dictAttributes.hasOwnProperty("sfwQueryID") && obj.dictAttributes.sfwQueryID) {
                var list = getQueryBookMarksID($scope.objCorrespondenceDetails);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwQueryID, "sfwQueryID", "invalid_query_id", CONST.VALIDATION.INVALID_QUERY_ID, $scope.validationErrorList);
            }

            if (obj.dictAttributes.hasOwnProperty("sfwParameters") && obj.dictAttributes.sfwParameters) {
                $scope.validateParameters(obj, obj.dictAttributes.sfwParameters, "sfwParameters");
            }
            if (obj.dictAttributes.hasOwnProperty("sfwAutoParameters") && obj.dictAttributes.sfwAutoParameters) {
                $scope.validateParameters(obj, obj.dictAttributes.sfwAutoParameters, "sfwAutoParameters");
            }
            if (obj.dictAttributes.hasOwnProperty("sfwCascadingRetrievalParameters") && obj.dictAttributes.sfwCascadingRetrievalParameters) {
                $scope.validateParameters(obj, obj.dictAttributes.sfwCascadingRetrievalParameters, "sfwCascadingRetrievalParameters");
            }
            if (obj.Elements && obj.Elements.length > 0) {
                if (obj.Name == "sfwTableBookMark") {
                    var entity = getEntityName(obj.dictAttributes.sfwEntityField, entityid, obj);

                    if (entity) validateForm(obj, entity, extraData);
                    else validateForm(obj, entityid, extraData);
                } else {
                    validateForm(obj, entityid, extraData);
                }
            }
        });
    };
    var getAllControlsId = function (mainObj) {
        angular.forEach(mainObj.Elements, function (obj) {
            if (obj && obj.dictAttributes.ID) {
                $scope.parameterList.push(obj.dictAttributes.ID);
            }
            if (obj.Elements.length > 0) {
                getAllControlsId(obj);
            }
        });
    };
    $scope.validateParameters = function (obj, params, prop) {
        getAllControlsId($scope.objQueryForm);
        var prefix = "prop-";
        if (prop == "sfwAutoParameters") {
            prefix = "autoprop-";
        } else if (prop == "sfwCascadingRetrievalParameters") {
            prefix = "cprop-";
        }
        //"PERSON_ID=txtFirstName;BENEFIT_TYPE=txtSsn";
        var param = params.split(";");
        for (var i = 0; i < param.length; i++) {
            var str1 = param[i].split("=");
            var strId = str1[str1.length - 1];
            $ValidationService.checkValidListValue($scope.parameterList, obj, strId, prop, prefix + strId, "parameter value(" + strId + ") does not exists", $scope.validationErrorList);
        }
    };
    var validateServerMethod = function (obj) {
        var RemoteObjectName = "srvCommon";
        var lstRemoteObj = [];

        if ($scope.validationData && $scope.validationData.hasOwnProperty("RemoteObject")) {
            lstRemoteObj = $scope.validationData["RemoteObject"];
        }
        var objServerObject = GetServerMethodObject(RemoteObjectName, lstRemoteObj);
        var list = PopulateServerMethod([], obj, objServerObject, true);
        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "sfwLoadSource", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
    };

    var getEntityName = function (entField, entityid, obj) {
        var entity;
        var data = $rootScope.getEntityAttributeIntellisense(entityid, true, false, false, false, false);
        if (entField && entField.contains('.')) {
            var lst = obj.dictAttributes.sfwEntityField.split(".");
            for (var i = 0; i < lst.length; i++) {
                if (data) {
                    var item = data.filter(function (x) { return x.ID == lst[i]; })[0];
                    if (item && item.Entity) {
                        entity = item.Entity;
                        data = $rootScope.getEntityAttributeIntellisense(entity, true, false, false, false, false);
                    }
                }
            }
        } else {
            entity = obj.dictAttributes.sfwEntityField;
            var item = data.filter(function (x) { return x.ID == entField; })[0];
            if (item && item.Entity) entity = item.Entity;
        }
        return entity;
    };
    //====================== create new object when there is no element for that object===============================
    //#region Create New Object
    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };
    //#endregion

    //#region Selection Methods
    // on click on Selected Field BookMark
    $scope.SelectedFieldBookMarkClick = function (obj) {
        $scope.selectedFieldBookMark = obj;
    };


    //// on click on Selected Rule BookMark
    $scope.SelectedRuleBookMarkClick = function (obj) {
        $scope.selectedRuleBookMark = obj;
    };

    // on click on selected table Bookmark
    $scope.SelectedTableBookMarksClick = function (obj) {
        if (obj) {
            obj.IsExpanded = true;
            $scope.selectedTableBookMark = obj;
            $scope.SelectedTableEntityID = "";
            if ($scope.selectedTableBookMark.dictAttributes && $scope.selectedTableBookMark.dictAttributes.sfwEntityField) {
                $scope.setEntityIDForSelectedTable($scope.selectedTableBookMark.dictAttributes.sfwEntityField);
            }
            $scope.selectedSfwColumn = undefined;
            $scope.selectedColumn = undefined;
        }
    };

    $scope.SelectedRepeaterBookMarksClick = function (obj) {
        if (obj) {
            obj.IsExpanded = true;
            $scope.selectedRepeaterBookMark = obj;
            $scope.SelectedTableEntityID = "";
            if ($scope.selectedRepeaterBookMark.dictAttributes && $scope.selectedRepeaterBookMark.dictAttributes.sfwEntityField) {
                $scope.setEntityIDForSelectedTable($scope.selectedRepeaterBookMark.dictAttributes.sfwEntityField);
            }
        }
    };


    $scope.setEntityIDForSelectedTable = function (entityField) {
        if (entityField) {
            var EntityID = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
            var data = [];
            $scope.SelectedTableEntityID = EntityID;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList;
            var entity = entities.filter(function (x) {
                return x.ID == EntityID;
            });
            if (entity.length > 0) {
                data = entity[0].Attributes;
            }
            var arrText = entityField.split(".");
            if (arrText.length > 0) {
                for (var index = 0; index < arrText.length; index++) {
                    var item = data.filter(function (x) { return x.ID == arrText[index]; });
                    if (item.length > 0) {
                        if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined" && item[0].ID == arrText[index] && arrText[index] != "") {
                            data = [];
                            entityname = item[0].Entity;
                            $scope.SelectedTableEntityID = entityname;
                            entity = entities.filter(function (x) { return x.ID == entityname; });
                            if (entity.length > 0) {
                                data = entity[0].Attributes;
                            }
                        }
                    }
                }
            }
        }

    };


    // on click on selected column
    $scope.selectedColumnClick = function (index, obj) {
        $scope.selectedColumn = index;
        $scope.selectedSfwColumn = obj;
    };

    // on click on selected Query
    $scope.SelectedQueryClick = function (obj) {
        if (obj) {
            $scope.selectedQueryBookMark = obj;
        }

    };
    // delete selected column details
    $scope.deleteSelectedColumn = function () {
        var Fieldindex = -1;
        if ($scope.selectedSfwColumn) {
            Fieldindex = $scope.selectedTableBookMark.Elements.indexOf($scope.selectedSfwColumn);
            $rootScope.DeleteItem($scope.selectedTableBookMark.Elements[Fieldindex], $scope.selectedTableBookMark.Elements);
        }

        $scope.selectedSfwColumn = undefined;

        if (Fieldindex < $scope.selectedTableBookMark.Elements.length && Fieldindex > -1) {

            $scope.selectedSfwColumn = $scope.selectedTableBookMark.Elements[Fieldindex];
            $scope.selectedColumn = Fieldindex;
        }
        else if ($scope.selectedTableBookMark.Elements.length > 0) {
            $scope.selectedSfwColumn = $scope.selectedTableBookMark.Elements[Fieldindex - 1];
            $scope.selectedColumn = Fieldindex - 1;
        }
        $scope.isDirty = true;
    };

    //#endregion

    //#region user field Methods
    // disable if there is no element for SFW column
    $scope.canDeleteColumn = function () {
        if ($scope.selectedSfwColumn) {
            return true;
        }
        else {
            return false;
        }
    };

    // Move up functionality for Column(Tables Tab)
    $scope.moveSelectedColumnUp = function () {
        if ($scope.selectedSfwColumn) {
            var index = $scope.selectedTableBookMark.Elements.indexOf($scope.selectedSfwColumn);
            var item = $scope.selectedTableBookMark.Elements[index - 1];
            if (item) {
                $scope.selectedTableBookMark.Elements[index - 1] = $scope.selectedSfwColumn;
                $scope.selectedTableBookMark.Elements[index] = item;
                $scope.selectedColumn = index - 1;
            }
        }
    };

    // disable the move up button if there is no element to move up
    $scope.canmoveSelectedColumnUp = function () {
        var flag = false;
        if ($scope.selectedSfwColumn != undefined) {
            for (var i = 0; i < $scope.selectedTableBookMark.Elements.length; i++) {
                if ($scope.selectedTableBookMark.Elements[i] == $scope.selectedSfwColumn) {
                    if (i > 0) {
                        flag = true;
                    }
                }
            }

        }

        return flag;
    };

    // Move Down function for Column(Tables Tab)

    $scope.moveSelectedColumnDown = function () {
        if ($scope.selectedSfwColumn) {
            var index = $scope.selectedTableBookMark.Elements.indexOf($scope.selectedSfwColumn);
            var item = $scope.selectedTableBookMark.Elements[index + 1];
            if (item) {
                $scope.selectedTableBookMark.Elements[index + 1] = $scope.selectedSfwColumn;
                $scope.selectedTableBookMark.Elements[index] = item;
                $scope.selectedColumn = index + 1;
            }
        }
    };


    // disable move down when there is no element to move down
    $scope.canmoveSelectedColumnDown = function () {
        var flag = false;
        if ($scope.selectedSfwColumn != undefined) {
            for (var i = 0; i < $scope.selectedTableBookMark.Elements.length; i++) {
                if ($scope.selectedTableBookMark.Elements[i] == $scope.selectedSfwColumn) {
                    if (i < $scope.selectedTableBookMark.Elements.length - 1) {
                        flag = true;
                    }
                }
            }

        }

        return flag;
    };

    //#endregion
    //#region Populate XML methods
    $scope.PopulateXmlMethodInCors = function (entityID) {
        $scope.lstXmlMethods = [];
        if (entityID) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (entityIntellisenseList) {
                var lst = entityIntellisenseList.filter(function (x) { return x.ID == entityID; });
                if (lst && lst.length > 0) {
                    angular.forEach(lst, function (method) {
                        angular.forEach(method.ObjectMethods, function (item) {
                            $scope.lstXmlMethods.push(item);

                        });
                    });
                }
            }
        }
    };
    //#endregion
    //#region for selection of Selected Template Click

    $scope.SelectedTemplateClick = function (obj) {

        $scope.selectedTemplateBookMark = obj;
        $scope.getTemplateList();
    };

    $scope.onchangeEnitityTableBookmark = function (entity) {
        if (entity) {
            $scope.entityTreeName = entity;
        }
        else {
            $scope.entityTreeName = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
        }
    };

    //#endregion

    //#region  Template Details
    $scope.onDetailClick = function () {
        var newScope = $scope.$new();
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Correspondence";
        newScope.lstAssociatedForms = [];
        newScope.objCorrespondenceDetails = {};

        newScope.LoadAssociatedForms = function () {
            if (newScope.objAssociatedForms && newScope.objAssociatedForms.Elements.length > 0) {
                for (var i = 0; i < newScope.objAssociatedForms.Elements.length; i++) {
                    newScope.lstAssociatedForms.push(newScope.objAssociatedForms.Elements[i]);
                }
            }
            if (newScope.blnAssociatedFormPresent) {
                angular.forEach(newScope.objAssociatedForms.Elements, function (itm) {
                    $.connection.hubCorrespondence.server.getFormEntityName(itm.dictAttributes.ID).done(function (data) {
                        newScope.receiveFormEntityName(data, itm.dictAttributes.ID);
                    });
                });
            }
            else {
                angular.forEach(newScope.lstAssociatedForms, function (itm) {
                    $.connection.hubCorrespondence.server.getFormEntityName(itm.FormName).done(function (data) {
                        newScope.receiveFormEntityName(data, itm.FormName);
                    });
                });
            }
        }

        newScope.Init = function () {
            newScope.objCorrespondenceDetails = {
                dictAttributes: {}, errors: {}
            };
            angular.forEach($scope.objCorrespondenceDetails.dictAttributes, function (val, key) {
                newScope.objCorrespondenceDetails.dictAttributes[key] = val;
            });

            angular.forEach($scope.objCorrespondenceDetails.errors, function (val, key) {
                newScope.objCorrespondenceDetails.errors[key] = val;
            });
            newScope.blnAssociatedFormPresent = $scope.blnAssociatedFormPresent;
            newScope.objAssociatedForms = $scope.objAssociatedForms;
            newScope.LoadAssociatedForms();

            $scope.PopulateCorrespondenceTemplate();

            $.connection.hubEntityModel.server.getInitialLoadRules($scope.objCorrespondenceDetails.dictAttributes.sfwEntity).done(function (data) {
                newScope.conditionalRules = data;
            });
        };

        newScope.validateCorsDetails = function () {
            var retFlag = false;
            newScope.CorDetailsErrorMessage = "";

            if (newScope.objCorrespondenceDetails.dictAttributes.sfwEntity) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

                if (newScope.lstAssociatedForms && newScope.lstAssociatedForms.length > 0) {
                    for (var i = 0, len = newScope.lstAssociatedForms.length; i < len; i++) {
                        if (newScope.objCorrespondenceDetails.dictAttributes.sfwEntity != newScope.lstAssociatedForms[i].FormEntity) {
                            if (!newScope.lstAssociatedForms[i].dictAttributes.sfwEntityField) {
                                newScope.CorDetailsErrorMessage = "Please select a Target Entity for the form.";
                                retFlag = true;
                                break;
                            }
                            var entTempList = [];
                            if (entityIntellisenseList && entityIntellisenseList.length > 0) {
                                entTempList = $filter('filter')(entityIntellisenseList, { ID: newScope.lstAssociatedForms[i].FormEntity }, true);
                            }
                            var objAttr = [];
                            if (entTempList && entTempList.length > 0 && entTempList[0].Attributes && entTempList[0].Attributes.length > 0) {
                                var associatedEntity = newScope.lstAssociatedForms[i].dictAttributes.sfwEntityField.split('.');
                                var lastField = associatedEntity[associatedEntity.length - 1];
                                for (var j = 0; j < associatedEntity.length; j++) {
                                    objAttr = entTempList[0].Attributes.filter(function (x) { return x.ID == associatedEntity[j] && x.Type == "Object"; });
                                    if (objAttr && objAttr.length > 0) {
                                        entTempList = $filter('filter')(entityIntellisenseList, { ID: objAttr[0] && objAttr[0].Entity }, true);
                                    }
                                }
                                objAttr = objAttr.filter(function (x) {
                                    return x.ID == lastField && x.Entity == newScope.objCorrespondenceDetails.dictAttributes.sfwEntity && x.Type == "Object";
                                });
                                if (objAttr && objAttr.length > 0) {
                                    retFlag = false;
                                } else {
                                    newScope.CorDetailsErrorMessage = "The Target Entity must be of type main Entity.";
                                    retFlag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (retFlag) return retFlag;

            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            retFlag = validateExtraFields(newScope);
            newScope.CorDetailsErrorMessage = newScope.FormDetailsErrorMessage;
            return retFlag;
        };

        newScope.refreshAssociatedForms = function () {
            $.connection.hubCorrespondence.server.loadAssociatedForms(newScope.objCorrespondenceDetails.dictAttributes.sfwTemplate, newScope.objCorrespondenceDetails.dictAttributes.sfwType).done(function (data) {
                newScope.$apply(function () {
                    $rootScope.IsLoading = true;
                    var lstDummyForms = [];

                    for (var i = 0; i < newScope.lstAssociatedForms.length; i++) {
                        if (lstDummyForms.length > 0 && !lstDummyForms.some(function (x) { return x.dictAttributes.ID == newScope.lstAssociatedForms[i] })) {
                            lstDummyForms.push(newScope.lstAssociatedForms[i]);
                        }
                        else {
                            lstDummyForms.push(newScope.lstAssociatedForms[i]);
                        }
                    }

                    if (data) {
                        newScope.lstAssociatedForms = [];
                        for (var i = 0; i < data.length; i++) {
                            var lst = lstDummyForms.filter(function (itm) {
                                return itm.dictAttributes.ID == data[i].FormName;
                            });
                            var objForm = { Name: 'sfwAssociatedForm', Value: '', dictAttributes: {}, Elements: [] };
                            objForm.dictAttributes.ID = data[i].FormName;
                            objForm.FormEntity = data[i].FormEntity;
                            if (lst && lst.length > 0) {
                                objForm.dictAttributes.sfwEntityField = lst[0].dictAttributes.sfwEntity;
                            }
                            newScope.lstAssociatedForms.push(objForm);

                        }
                    }

                    $rootScope.IsLoading = false;
                });
            });
        };

        newScope.receiveFormEntityName = function (data, formName) {
            newScope.$apply(function () {
                angular.forEach(newScope.lstAssociatedForms, function (itm) {
                    if (itm.dictAttributes.ID == formName) {
                        itm.FormEntity = data;
                    }
                });
            });
        };

        newScope.onClickAssociatedForm = function (index, obj) {
            newScope.selectCurrentAF = index;
            newScope.selectedAssociatedFormRow = obj;
        };

        newScope.onClickWithoutAssociatedForm = function (index, obj) {
            newScope.selectCurrentWithoutAF = index;
            newScope.selectedWithoutAssociatedFormRow = obj;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lst = entityIntellisenseList.filter(function (x) { return x.BusinessObjectName == newScope.selectedWithoutAssociatedFormRow.FormEntity; });

            if (lst.length > 0) {
                newScope.selectedWithoutAssociatedFormRow.EntityName = lst[0].ID;
            }
        };

        newScope.openAssociatedFormsDialog = function (index, obj) {
            var newAssociatedFormScope = newScope.$new();
            if (newScope) {
                if (newScope.blnAssociatedFormPresent) {
                    newScope.selectCurrentAF = index;
                    newScope.selectedAssociatedFormRow = obj;
                } else {

                    newScope.selectCurrentWithoutAF = index;
                    newScope.selectedWithoutAssociatedFormRow = obj;
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var lst = entityIntellisenseList.filter(function (x) { return x.BusinessObjectName == newScope.selectedWithoutAssociatedFormRow.FormEntity; });

                    if (lst.length > 0) {
                        newScope.selectedWithoutAssociatedFormRow.EntityName = lst[0].ID;
                    }
                }
            }
            newAssociatedFormScope.onAssociatedFormOKClick = function () {
                if (newScope.blnAssociatedFormPresent) {
                    var fieldName = "";
                    var displayEntity = getDisplayedEntity(newScope.selectedAssociatedFormRow.LstDisplayedEntities);
                    var displayName = displayEntity.strDisplayName;
                    if (newScope.selectedAssociatedFormRow && newScope.selectedAssociatedFormRow.SelectedField) {
                        fieldName = newScope.selectedAssociatedFormRow.SelectedField.ID;
                        if (displayName != "") {
                            fieldName = displayName + "." + newScope.selectedAssociatedFormRow.SelectedField.ID;
                        }
                    }
                    newScope.selectedAssociatedFormRow.dictAttributes.sfwEntityField = fieldName;//newScope.selectedAssociatedFormRow.SelectedField.ID;
                }
                else {
                    var displayEntity = getDisplayedEntity(newScope.selectedWithoutAssociatedFormRow.LstDisplayedEntities);
                    var displayName = displayEntity.strDisplayName;
                    if (newScope.selectedWithoutAssociatedFormRow.Field) {
                        fieldName = newScope.selectedWithoutAssociatedFormRow.Field.ID;
                        if (displayName != "") {
                            fieldName = displayName + "." + newScope.selectedWithoutAssociatedFormRow.Field.ID;
                        }
                    }
                    newScope.selectedWithoutAssociatedFormRow.TargetEntity = fieldName;//newScope.selectedWithoutAssociatedFormRow.Field.ID;
                    var obj = { Name: 'sfwAssociatedForm', value: '', dictAttributes: { ID: newScope.selectedWithoutAssociatedFormRow.FormName, sfwEntityField: fieldName, FormEntity: newScope.selectedWithoutAssociatedFormRow.EntityName }, Elements: [] };
                    $rootScope.PushItem(obj, newScope.objAssociatedForms.Elements, "onClickWithoutAssociatedForm");
                    newScope.blnAssociatedFormPresent = true;
                }
                newAssociatedFormScope.onAssociatedFormCloseClick();
            };

            newAssociatedFormScope.onAssociatedFormCloseClick = function () {
                newAssociatedFormScope.openAssociatedFormDialog.close();
            };

            newAssociatedFormScope.openAssociatedFormDialog = $rootScope.showDialog(newAssociatedFormScope, "Associated Form", "Correspondence/views/CorrespondenceAssociatedForms.html");
        };

        newScope.NavigateToEntity = function (aEntityID, error) {
            if (aEntityID && aEntityID != "" && !error) {
                //objNewDialog.close();
                newScope.onOkClick();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };

        newScope.onOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");


            angular.forEach(newScope.objCorrespondenceDetails.dictAttributes, function (val, key) {
                $rootScope.EditPropertyValue($scope.objCorrespondenceDetails.dictAttributes[key], $scope.objCorrespondenceDetails.dictAttributes, key, val);
            });

            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }

            if (newScope.lstAssociatedForms && newScope.lstAssociatedForms.length > 0) {
                if ($scope.objAssociatedForms && $scope.objAssociatedForms.Elements.length > 0) {
                    $rootScope.EditPropertyValue($scope.objAssociatedForms.Elements, $scope.objAssociatedForms, "Elements", []);
                    //$scope.objAssociatedForms.Elements = [];
                }
                for (var i = 0; i < newScope.lstAssociatedForms.length; i++) {
                    $rootScope.PushItem(newScope.lstAssociatedForms[i], $scope.objAssociatedForms.Elements);
                    //$scope.objAssociatedForms.Elements.push($scope.lstAssociatedForms[i]);
                }
            }

            if ($scope.objAssociatedForms.Elements.length > 0) {
                $rootScope.EditPropertyValue($scope.blnAssociatedFormPresent, $scope, "blnAssociatedFormPresent", true);
                //$scope.blnAssociatedFormPresent = true;
            }
            else {
                $rootScope.EditPropertyValue($scope.blnAssociatedFormPresent, $scope, "blnAssociatedFormPresent", false);
                //$scope.blnAssociatedFormPresent = false;
            }

            $rootScope.EditPropertyValue($scope.entityTreeName, $scope, "entityTreeName", $scope.objCorrespondenceDetails.dictAttributes.sfwEntity);
            //$scope.entityTreeName = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;

            $rootScope.UndRedoBulkOp("End");
            $scope.objCorrespondenceDetails.errors = {};
            angular.forEach(newScope.objCorrespondenceDetails.errors, function (val, key) {
                $scope.objCorrespondenceDetails.errors[key] = val;
            });
            if ($.isEmptyObject($scope.objCorrespondenceDetails.errors)) {
                $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.objCorrespondenceDetails);
            }
            newScope.closeDetails();
        };

        newScope.closeDetails = function () {
            $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.objCorrespondenceDetails);
            newScope.TemplateDetailsDialog.close();
        };
        newScope.onConditionalRuleKeyDown = function (event) {
            var visibleRules = [];
            var input = $(event.target);
            if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                //visibleRules = newScope.conditionalRules;
                //if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                //    $(input).autocomplete("search", $(input).val());
                //    event.preventDefault();
                //} else {
                //    setSingleLevelAutoComplete($(event.target), visibleRules);
                //}
                newScope.setCondtionalRuleData(input);
            }
        };
        newScope.showConditionalRuleIntellisense = function (event) {
            var input = $(event.target).prevAll("input[type='text']");
            var visibleRules = [];
            //   visibleRules = newScope.conditionalRules;
            newScope.setCondtionalRuleData(input);
            newScope.validateConditionalRule(event);
        };
        newScope.setCondtionalRuleData = function (input) {
            if (newScope.objCorrespondenceDetails && !newScope.objCorrespondenceDetails.errors) {
                newScope.objCorrespondenceDetails.errors = {};
            }
            if (newScope.objCorrespondenceDetails && newScope.objCorrespondenceDetails.dictAttributes.sfwEntity && (newScope.objCorrespondenceDetails.errors && !newScope.objCorrespondenceDetails.errors.invalid_entity)) {
                $.connection.hubEntityModel.server.getInitialLoadRules(newScope.objCorrespondenceDetails.dictAttributes.sfwEntity).done(function (data) {
                    newScope.conditionalRules = data;
                    input.focus();
                    setSingleLevelAutoComplete(input, newScope.conditionalRules);
                    if ($(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    }
                });
            } else {
                setSingleLevelAutoComplete(input, []);
            }
        };
        newScope.validateConditionalRule = function (event) {
            var input = $(event.target);
            var list = newScope.conditionalRules ? newScope.conditionalRules : [];
            $ValidationService.checkValidListValue(list, newScope.objCorrespondenceDetails, $(input).val(), "sfwConditionalRule", "sfwConditionalRule", CONST.VALIDATION.CONDITIONAL_RULE_NOT_EXISTS, $scope.validationErrorList);
        };
        newScope.navigateToConditionalRule = function (aVisibleRuleID) {
            var entity = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
            if (aVisibleRuleID && aVisibleRuleID != "") {
                newScope.onOkClick();
                $NavigateToFileService.NavigateToFile(entity, "initialload", aVisibleRuleID);
            }
        };

        newScope.TemplateDetailsDialog = $rootScope.showDialog(newScope, "Details", "Correspondence/views/CorrespondenceTemplateDetails.html", { width: 700, height: 560 });
        newScope.Init();
    };
    //#endregion

    //#region Synchronise Bookmarks
    $scope.onSynchroniseClick = function () {
        $rootScope.IsLoading = true;
        $.connection.hubCorrespondence.server.synchronizeBookmarks($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate, $scope.objCorrespondenceDetails.dictAttributes.ID, $scope.objCorrespondenceDetails.dictAttributes.sfwType);
    };

    $scope.receieveSyncronizeBookMarkData = function (data) {
        var alBookmarks = JSON.parse(data);
        $rootScope.UndRedoBulkOp("Start");
        $scope.synchronizeBookmarksValues(alBookmarks);
        $scope.SyncronizeCorTestStep(alBookmarks);
        $rootScope.UndRedoBulkOp("End");
        if ($scope.objFieldBookMarks.Elements.length == 0) {
            $scope.selectedFieldBookMark = undefined;
        } else if ($scope.objFieldBookMarks.Elements.length > 0) {
            $scope.selectedFieldBookMark = $scope.objFieldBookMarks.Elements[0];
        }
        if ($scope.objTableBookMarks.Elements.length == 0) {
            $scope.selectedTableBookMark = undefined;
        } else if ($scope.objTableBookMarks.Elements.length > 0) {
            $scope.selectedTableBookMark = $scope.objTableBookMarks.Elements[0];
        }
        if ($scope.objRepeaterBookMarks.Elements.length == 0) {
            $scope.selectedRepeaterBookMark = undefined;
        } else if ($scope.objRepeaterBookMarks.Elements.length > 0) {
            $scope.selectedRepeaterBookMark = $scope.objRepeaterBookMarks.Elements[0];
        }

        if ($scope.objChildTemplateBookmarks.Elements.length == 0) {
            $scope.selectedTemplateBookMark = undefined;
        } else if ($scope.objChildTemplateBookmarks.Elements.length > 0) {
            $scope.selectedTemplateBookMark = $scope.objChildTemplateBookmarks.Elements[0];
        }
        if ($scope.objRuleBookmarks.Elements.length == 0) {
            $scope.selectedRuleBookMark = undefined;
        } else if ($scope.objRuleBookmarks.Elements.length > 0) {
            $scope.selectedRuleBookMark = $scope.objRuleBookmarks.Elements[0];
        }

        $scope.getTemplateList();

        $scope.isDirty = true;
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });


        $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
    };


    $scope.SyncronizeCorTestStep = function (alBookmarks) {

        if ($scope.objCorrespondenceTests) {
            //this loop through all tests

            //before load bookmarks,need to update bookmark values with old onces
            $.connection.hubCorrespondence.server.retrieveBookmarksOfTemplate($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate).done(function (data) {
                var lstAllBookMarks = data;
                if (lstAllBookMarks && lstAllBookMarks.length > 0) {
                    $scope.$evalAsync(function () {
                        for (var i = 0; i < $scope.objCorrespondenceTests.Elements.length; i++) {

                            //keep old bookmark value list before removing elements
                            var test = $scope.objCorrespondenceTests.Elements[i];
                            var fields = [];
                            for (var j = test.Elements.length - 1; j >= 0; j--) {
                                if (test.Elements[j].Name !== "parameter") {
                                    fields.push(test.Elements[j]);
                                }
                            }

                            //remove existing test children
                            for (var j = test.Elements.length - 1; j >= 0; j--) {
                                if (test.Elements[j].Name !== "parameter") {
                                    test.Elements.splice(j, 1);
                                }
                            }
                            //load updated test children again

                            $scope.LoadBookmarks(lstAllBookMarks, test, fields);

                        }
                    });
                }
            });
        }
    }
    $scope.synchronizeBookmarksValues = function (alBookmarks) {
        for (var i = 0; i < alBookmarks.length; i++) {
            var corBookmark = alBookmarks[i];
            if (!$scope.FindBookmark(corBookmark)) {
                if ($scope.IsFieldBookMark(corBookmark)) {
                    var standardBookmark = { Name: 'sfwFieldBookMark', Value: '', dictAttributes: { ID: corBookmark.Name }, Elements: [] };
                    $rootScope.PushItem(standardBookmark, $scope.objFieldBookMarks.Elements);
                }
                else if ($scope.IsTableBookMark(corBookmark)) {
                    var standardBookmark = { Name: 'sfwTableBookMark', Value: '', dictAttributes: { ID: corBookmark.Name }, Elements: [] };
                    $rootScope.PushItem(standardBookmark, $scope.objTableBookMarks.Elements);
                }
                else if ($scope.IsQueryBookMark(corBookmark)) {
                    var standardBookmark = { Name: 'sfwQueryBookMark', Value: '', dictAttributes: { ID: corBookmark.Name }, Elements: [] };
                    $rootScope.PushItem(standardBookmark, $scope.objQueryBookMarks.Elements);
                }
                else if ($scope.IsTemplateBookMark(corBookmark)) {

                    var standardBookmark = { Name: 'sfwChildTemplateBookmark', Value: '', dictAttributes: { ID: corBookmark.Name, sfwChildTemplateType: 'Entity' }, Elements: [] };
                    $rootScope.PushItem(standardBookmark, $scope.objChildTemplateBookmarks.Elements);
                }
                else if ($scope.IsRuleBookMark(corBookmark)) {

                    var standardBookmark = { Name: 'sfwRuleBookmark', Value: '', dictAttributes: { ID: corBookmark.Name }, Elements: [] };
                    $rootScope.PushItem(standardBookmark, $scope.objRuleBookmarks.Elements);
                }
                else if ($scope.IsRepeaterBookMark(corBookmark)) {

                    var standardBookmark = { Name: 'sfwRepeater', Value: '', dictAttributes: { ID: corBookmark.Name }, Elements: [] };

                    if (corBookmark.icolRepeaterCells.length > 0) {
                        angular.forEach(corBookmark.icolRepeaterCells, function (cell) {
                            var fieldBookmark = { Name: 'sfwFieldBookMark', Value: '', dictAttributes: { ID: cell.Name }, Elements: [] };
                            standardBookmark.Elements.push(fieldBookmark);
                        });
                    }

                    $rootScope.PushItem(standardBookmark, $scope.objRepeaterBookMarks.Elements);

                }
                blnFound = true;
            }
        }

        blnFound = false;
        var strBookmarkName;

        //Deleting old field bookmarks
        if ($scope.objFieldBookMarks) {
            for (var i = $scope.objFieldBookMarks.Elements.length - 1; i >= 0; i--) {
                var objFieldBookMark = $scope.objFieldBookMarks.Elements[i];
                strBookmarkName = objFieldBookMark.dictAttributes.ID;
                blnFound = false;
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        if ($scope.IsFieldBookMark(objBookmark)) {
                            blnFound = true;
                            break;
                        }
                    }
                }

                if (!blnFound) {
                    $rootScope.DeleteItem($scope.objFieldBookMarks.Elements[i], $scope.objFieldBookMarks.Elements);
                }
            }
        }

        //Deleting old table bookmarks
        if ($scope.objTableBookMarks) {
            for (var i = $scope.objTableBookMarks.Elements.length - 1; i >= 0; i--) {
                var objTableBookMark = $scope.objTableBookMarks.Elements[i];
                strBookmarkName = objTableBookMark.dictAttributes.ID;
                blnFound = false;
                var strColHeadings = "";
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        if ($scope.IsTableBookMark(objBookmark)) {
                            if (objBookmark.strColHeadings != undefined && objBookmark.strColHeadings != "")
                                strColHeadings = objBookmark.strColHeadings;

                            blnFound = true;
                            break;
                        }
                    }
                }

                if (blnFound) {
                    var lstNewItems = [];


                    if (strColHeadings != undefined && strColHeadings.length > 0) {
                        var strArrHeaders = strColHeadings.split(';');
                        for (var m = 0; m < strArrHeaders.length; m++) {
                            var blnFound = false;
                            for (var k = 0; k < objTableBookMark.Elements.length; k++) {
                                if (objTableBookMark.Elements[k].dictAttributes.sfwColumnHeader == strArrHeaders[m]) {
                                    lstNewItems.push(objTableBookMark.Elements[k]);
                                    blnFound = true;
                                    break;
                                }
                            }
                            if (!blnFound) {
                                var corTableColumn = { Name: 'sfwColumn', Value: '', dictAttributes: { sfwColumnHeader: strArrHeaders[m] }, Elements: [] };
                                lstNewItems.push(corTableColumn);
                            }
                        }
                        objTableBookMark.Elements = lstNewItems;
                    }
                }
                else {
                    $rootScope.DeleteItem($scope.objTableBookMarks.Elements[i], $scope.objTableBookMarks.Elements);
                }
            }
        }

        //Deleting old repeater bookmarks
        if ($scope.objRepeaterBookMarks) {
            for (var i = $scope.objRepeaterBookMarks.Elements.length - 1; i >= 0; i--) {
                var objRepeaterBookMark = $scope.objRepeaterBookMarks.Elements[i];
                strBookmarkName = objRepeaterBookMark.dictAttributes.ID;
                blnFound = false;
                var arrRepeaterCells = "";
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        if ($scope.IsRepeaterBookMark(objBookmark)) {
                            if (objBookmark.icolRepeaterCells)
                                arrRepeaterCells = objBookmark.icolRepeaterCells;

                            blnFound = true;
                            break;
                        }
                    }
                }

                if (blnFound) {
                    var lstNewBkmks = [];
                    for (var k = 0; k < arrRepeaterCells.length; k++) {
                        var objfieldBookmark = arrRepeaterCells[k];
                        var blnFieldBkmkFound = false;
                        for (l = 0; l < objRepeaterBookMark.Elements.length; l++) {
                            if (objRepeaterBookMark.Elements[l].dictAttributes.ID == objfieldBookmark.Name) {
                                blnFieldBkmkFound = true;
                                $rootScope.PushItem(objRepeaterBookMark.Elements[l], lstNewBkmks);

                                break;
                            }
                        }

                        if (!blnFieldBkmkFound) {
                            var corTableColumn = { Name: 'sfwFieldBookMark', Value: '', dictAttributes: { ID: objfieldBookmark.Name }, Elements: [] };

                            $rootScope.PushItem(corTableColumn, lstNewBkmks);
                        }
                    }

                    objRepeaterBookMark.Elements = lstNewBkmks;

                }
                else {
                    $rootScope.DeleteItem($scope.objRepeaterBookMarks.Elements[i], $scope.objRepeaterBookMarks.Elements);
                }
            }
        }


        //Deleting old query bookmarks
        if ($scope.objQueryBookMarks != null) {
            for (var i = $scope.objQueryBookMarks.Elements.length - 1; i >= 0; i--) {
                var objQueryBookMark = $scope.objQueryBookMarks.Elements[i];
                strBookmarkName = objQueryBookMark.dictAttributes.ID;
                blnFound = false;
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        if ($scope.IsQueryBookMark(objBookmark)) {
                            blnFound = true;
                            break;
                        }

                    }
                }
                if (!blnFound) {
                    $rootScope.DeleteItem($scope.objQueryBookMarks.Elements[i], $scope.objQueryBookMarks.Elements);
                }
            }
        }

        //Deleting old template bookmarks
        if ($scope.objChildTemplateBookmarks != null) {
            for (var i = $scope.objChildTemplateBookmarks.Elements.length - 1; i >= 0; i--) {
                var templateBookmarkVM = $scope.objChildTemplateBookmarks.Elements[i];
                strBookmarkName = templateBookmarkVM.dictAttributes.ID;

                blnFound = false;
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        if ($scope.IsTemplateBookMark(objBookmark)) {
                            blnFound = true;
                            break;
                        }
                    }
                }

                if (!blnFound) {
                    $rootScope.DeleteItem(templateBookmarkVM, $scope.objChildTemplateBookmarks.Elements);
                }
            }
        }

        //Deleting old rule bookmarks
        if ($scope.objRuleBookmarks) {
            for (var i = $scope.objRuleBookmarks.Elements.length - 1; i >= 0; i--) {
                var templateBookmarkVM = $scope.objRuleBookmarks.Elements[i];
                strBookmarkName = templateBookmarkVM.dictAttributes.ID;

                blnFound = false;
                for (var j = 0; j < alBookmarks.length; j++) {
                    var objBookmark = alBookmarks[j];
                    if (objBookmark.Name == strBookmarkName) {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound) {
                    $rootScope.DeleteItem(templateBookmarkVM, $scope.objRuleBookmarks.Elements);
                }
            }
        }
    };

    $scope.FindBookmark = function (corBookmark) {
        var alBookmarks = [];
        if ($scope.IsTableBookMark(corBookmark)) {
            if ($scope.objTableBookMarks != null) {
                alBookmarks = $scope.objTableBookMarks.Elements;
            }
        }
        else if ((corBookmark.Type == "QueryUser") ||
            (corBookmark.Type == "QueryConditionalBlock")) {
            if ($scope.objQueryBookMarks) {
                alBookmarks = $scope.objQueryBookMarks.Elements;
            }
        }
        else if ($scope.IsRepeaterBookMark(corBookmark)) {
            if ($scope.objRepeaterBookMarks) {
                alBookmarks = $scope.objRepeaterBookMarks.Elements;
            }
        }
        else if (corBookmark.Type == "Template") {
            if ($scope.objChildTemplateBookmarks) {
                alBookmarks = $scope.objChildTemplateBookmarks.Elements;
            }
        }
        else if (corBookmark.Type == "RuleConditionalBlock") {
            if ($scope.objRuleBookmarks) {
                alBookmarks = $scope.objRuleBookmarks.Elements;
            }
        }
        else {
            if ($scope.objFieldBookMarks) {
                alBookmarks = $scope.objFieldBookMarks.Elements;
            }
        }

        if (alBookmarks) {

            var lst = alBookmarks.filter(function (itm) {
                return itm.dictAttributes.ID == corBookmark.Name;
            });
            if (lst && lst.length > 0) {
                return true;

            }
            return false;
        }
    };

    $scope.IsFieldBookMark = function (corBookmark) {
        if (($scope.IsTableBookMark(corBookmark)) || ($scope.IsQueryBookMark(corBookmark)) || ($scope.IsTemplateBookMark(corBookmark)) || ($scope.IsRuleBookMark(corBookmark)) || ($scope.IsRepeaterBookMark(corBookmark))) {
            return false;
        }

        return true;
    };

    $scope.IsTableBookMark = function (corBookmark) {
        if (corBookmark.Type == "NormalTable") {
            return true;
        }

        return false;
    };

    $scope.IsQueryBookMark = function (corBookmark) {
        if ((corBookmark.Type == "QueryUser") ||
            (corBookmark.Type == "QueryConditionalBlock")) {
            return true;
        }

        return false;
    };

    $scope.IsTemplateBookMark = function (corBookmark) {
        if ((corBookmark.Type == "Template")) {
            return true;
        }

        return false;
    };
    $scope.IsRuleBookMark = function (corBookmark) {
        if ((corBookmark.Type == "RuleConditionalBlock")) {
            return true;
        }

        return false;
    };
    $scope.IsRepeaterBookMark = function (corBookmark) {
        if ((corBookmark.Type == "Repeater")) {
            return true;
        }

        return false;
    };


    //#endregion

    //#region open Word Document

    $scope.openWordTemplateClick = function () {
        $rootScope.IsLoading = true;
        $.connection.hubCorrespondence.server.openWordDocument($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate, $scope.objCorrespondenceDetails.dictAttributes.ID);
    };

    // dummy recieve call from open word docs( jus to stop a spinner)

    $scope.receieveDummyCall = function () {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };
    //#endregion

    //#region Entity tree Tab Selection



    // change on Radio Button Selection

    $scope.IsGroupChange = function (obj) {
        if (obj) {
            if (obj == 'Entity' || obj == 'Collection') {
                $scope.selectedTemplateBookMark.dictAttributes.sfwEntityField = '';
                $scope.selectedTemplateBookMark.dictAttributes.sfwTemplateName = '';
            }

        }

    };

    //#endregion

    //#region Template Intellisense
    $scope.getTemplateList = function () {
        if ($scope.selectedTemplateBookMark) {

            if (!$scope.objCorrespondenceDetails.dictAttributes.sfwTemplate) {
                $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate = "";
            }
            var entityname = "";
            if ($scope.selectedTemplateBookMark.dictAttributes.sfwEntityField) {
                var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.objCorrespondenceDetails.dictAttributes.sfwEntity, $scope.selectedTemplateBookMark.dictAttributes.sfwEntityField);
                if (objField) {
                    entityname = objField.Entity;
                }
            }

            if (entityname) {
                $.connection.hubCorrespondence.server.getListOfTemplates(entityname, $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate).done(function (data) {
                    $scope.receieveTemplatesList(data);
                });
            }
            else {
                $.connection.hubCorrespondence.server.getListOfTemplates($scope.objCorrespondenceDetails.dictAttributes.sfwEntity, $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate).done(function (data) {
                    $scope.receieveTemplatesList(data);
                });
            }

        }

    };


    $scope.receieveTemplatesList = function (data) {

        $scope.$apply(function () {
            $scope.lstTemplates = data;
        });
    };

    $scope.onTemplateEntityChange = function () {
        $scope.getTemplateList();
        if ($scope.selectedTemplateBookMark) {
            $scope.validateTemplateName($scope.selectedTemplateBookMark, "edit");
        }
    };

    //#endregion

    //#region User Field Right panel 



    //#region Add Row Context Menu

    $scope.menuOptionsForUserControl = [
        ['Add Row', function ($itemScope) {
            $scope.AddRowCommand();
        }], null,

    ];
    $scope.AddRowCommand = function () {
        $rootScope.UndRedoBulkOp("Start");
        var lstTable = $scope.objQueryForm.Elements.filter(function (x) { return x.Name == "sfwTable"; });
        if (lstTable == null && lstTable.length == 0) {
            lstTable = { Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };//.SetName(ApplicationConstants.XMLFacade.SFWTABLE, ApplicationConstants.XMLFacade.PREFIX_SWC);

            lstTable.dictAttributes.ID = "";
            $rootScope.PushItem(lstTable, $scope.objQueryForm.Elements);

        }

        if (lstTable && lstTable.length > 0) {

            var lstRow = lstTable[0].Elements.filter(function (x) { return x.Name == 'sfwRow'; });

            var sfxRowModel = { Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };//.SetName(ApplicationConstants.XMLFacade.SFWROW, ApplicationConstants.XMLFacade.PREFIX_SWC);

            var colCnt = GetMaxColCount(lstRow[0], lstTable);
            if (colCnt < 1) {
                colCnt = 1;
            }
            for (var colIndex = 0; colIndex < colCnt; colIndex++) {

                var sfxCellModel = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

                $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);
            }
            $rootScope.PushItem(sfxRowModel, lstTable[0].Elements);


        }
        $rootScope.UndRedoBulkOp("End");
    };
    //#endregion

    //#endregion

    //#region Add Control With Item
    $scope.OnAddControlClick = function (cntrlClassName) {
        var lstselectedfields = [];
        var controlVM;
        if (cntrlClassName) {
            if ($scope.objQueryBookMarks != null && $scope.objQueryBookMarks.Elements.length > 0) {
                //var controlVM = $scope.GetSelectedControl();
                var ObjSfxTableVM = $scope.objQueryForm.Elements.filter(function (x) { return x.Name == "sfwTable"; });
                if (ObjSfxTableVM == undefined || ObjSfxTableVM == "") {
                    $rootScope.UndRedoBulkOp("Start");
                    var newSfxTableModel = { Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxTableModel.ParentVM = $scope.objQueryForm;
                    // newSfxTableModel.dictAttributes.ID = "";
                    newSfxTableModel.dictAttributes.ID = "tblMain";// Bug 8462:In correspondence ,for Sfwtable in User field ,the ID is blank.
                    $rootScope.PushItem(newSfxTableModel, $scope.objQueryForm.Elements);
                    ObjSfxTableVM.push(newSfxTableModel);
                }

                if (ObjSfxTableVM && ObjSfxTableVM.length > 0) {
                    var lstRow = ObjSfxTableVM[0].Elements.filter(function (x) { return x.Name == 'sfwRow'; });
                    angular.forEach($scope.objQueryBookMarks.Elements, function (objQueryBookmark) {
                        if (objQueryBookmark.IsSelected) {
                            lstselectedfields.push(objQueryBookmark);
                            var RowCount = ObjSfxTableVM[0].Elements.length;
                            var ColCount = 0;
                            if (lstRow && lstRow.length > 0) {
                                ColCount = GetMaxColCount(lstRow[0], ObjSfxTableVM);
                            }
                            if (RowCount == 0 && ColCount == 0) {
                                ColCount = 2;
                            }

                            objQueryBookmark.IsSelected = false;
                        }
                    });
                    if (lstselectedfields && lstselectedfields.length > 0) { //Bug 8479:In Correspondence-on double clicking on any control ,from items tab, row is getting added.
                        $scope.AddControls(ObjSfxTableVM[0], controlVM, cntrlClassName, lstselectedfields, false);
                    }
                }
            }
            else {

            }

        }

    };



    $scope.AddControls = function (tableVM, selectedCntrlVM, astrControlClass, alst, isListView) {

        if (astrControlClass == "sfwHyperLink") {
            astrControlClass = "sfwLinkButton";
        }

        var dRowMultiplier = 0.4;

        var totalControlCount = 1;
        totalControlCount = alst.length * 2;

        var intRows = 1;
        var ColCount = 2;
        if (tableVM && tableVM.Elements && tableVM.Elements.length > 0) {
            ColCount = GetMaxColCount(tableVM.Elements[0], tableVM);
        }
        if (ColCount == 0) {
            var sfxCellModel1 = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
            tableVM.Elements[0].Elements.push(sfxCellModel1);
            var sfxCellModel2 = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
            tableVM.Elements[0].Elements.push(sfxCellModel2);
            ColCount = 2;
        }
        if (totalControlCount == ColCount) {
            intRows = 1;
        }
        else {
            intRows = (totalControlCount / ColCount) + dRowMultiplier;
        }

        intRows = Math.round(intRows);
        if (intRows <= 0)//atleast one row should be added
            intRows = 1;

        var cellVM = GetVM("sfwColumn", selectedCntrlVM);

        var intCurRowInd;
        if (cellVM) {
            var rownvM = cellVM.ParentVM;
            var rowindex = rownvM.ParentVM.Elements.indexOf(rownvM);
            intCurRowInd = rowindex;
        }
        else {
            var RowCount = tableVM.Elements.length;
            intCurRowInd = RowCount - 1;
        }


        var cellLst = [];

        var cellInd = 0;
        for (rowInd = 1; rowInd <= intRows; rowInd++) {
            var prefix = "swc";

            var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
            sfxRowModel.ParentVM = tableVM;

            for (colInd = 0; colInd < ColCount; colInd++) {
                var sfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                sfxCellModel.ParentVM = sfxRowModel;

                $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);
            }

            $rootScope.InsertItem(sfxRowModel, tableVM.Elements, rowInd + intCurRowInd);

            angular.forEach(tableVM.Elements[rowInd + intCurRowInd].Elements, function (vm) {
                var cellitem = {};
                cellitem.key = cellInd;
                cellitem.value = vm;
                cellLst.push(cellitem);
                cellInd++;
            });

        }


        cellInd = 0;
        angular.forEach(alst, function (field) {
            cellInd = $scope.AddControlsGrid(cellLst, astrControlClass, field, cellInd, isListView);
        });

    };

    $scope.AddControlsGrid = function (acellLst, astrControlClass, afield, cellInd, isListViewControl) {
        var aintCellInd = cellInd;
        var strControlID = CreateControlID($scope.objCorrespondenceDetails, afield.dictAttributes.ID, astrControlClass);

        var cellVM;
        var newControl;
        var prefix = "swc";
        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;

                aintCellInd++;

                newControl = { Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: { sfwIsCaption: 'True' }, Elements: [], Children: [] };
                newControl.ParentVM = cellVM;

                var strLabelID = CreateControlID($scope.objCorrespondenceDetails, afield.dictAttributes.ID, "sfwLabel", true);
                newControl.dictAttributes.ID = strLabelID;
                newControl.dictAttributes.Text = GetCaptionFromFieldName(afield.dictAttributes.ID) + " : ";

                if (!isListViewControl) {
                    newControl.dictAttributes.AssociatedControlID = strControlID;
                }

                $rootScope.PushItem(newControl, cellVM.Elements);
                break;
            }
        }


        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;
                aintCellInd++;
                newControl = { Name: astrControlClass, value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newControl.ParentVM = cellVM;
                if (astrControlClass == "sfwLabel") {
                    strControlID = CreateControlID($scope.objCorrespondenceDetails, afield.dictAttributes.ID, astrControlClass, false);
                }
                else {
                    strControlID = CreateControlID($scope.objCorrespondenceDetails, afield.dictAttributes.ID, astrControlClass, true);
                }


                newControl.dictAttributes.ID = strControlID;

                newControl.dictAttributes.sfwQueryID = afield.dictAttributes.ID;

                if (astrControlClass == "sfwLinkButton") {
                    newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
                }
                else if (astrControlClass == "sfwDropDownList" || astrControlClass == "sfwMultiSelectDropDownList" || astrControlClass == "sfwCascadingDropDownList" || astrControlClass == "sfwCheckBoxList" || astrControlClass == "sfwRadioButtonList") {
                    if (endsWith(afield.Value, "_value")) {

                        var entityname = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var strCodeGroup = GetCodeID(entityname, afield.dictAttributes.ID, entityIntellisenseList);
                        if (!strCodeGroup) {
                            strCodeGroup = "0";
                        }
                        newControl.dictAttributes.sfwLoadSource = strCodeGroup;
                    }
                }
                else if (astrControlClass == "sfwRadioButton") {
                    var strIbusName = afield.Value.substring(afield.Value.indexOf(".") + 1);
                    newControl.dictAttributes.GroupName = strIbusName;
                }
                else {
                    SetDefultValuesBasedOnDataType(afield, astrControlClass, newControl);
                }
                $rootScope.PushItem(newControl, cellVM.Elements);

                break;
            }
        }


        return aintCellInd;


    };

    //#endregion

    //#region for tabs selection

    $scope.selectDataFieldsTab = function () {

        $scope.SelectedCorrespondenceTab = 'DataFields';
        $scope.ActiveTabForCorr = 'Entity';

        if ($scope.objFieldBookMarks && $scope.objFieldBookMarks.Elements.length > 0) {
            $scope.SelectedFieldBookMarkClick($scope.objFieldBookMarks.Elements[0]);
        }
    };

    $scope.selectTablesTab = function () {

        $scope.SelectedCorrespondenceTab = 'Tables';
        $scope.ActiveTabForCorr = 'Entity';

        //if ($scope.objTableBookMarks && $scope.objTableBookMarks.Elements.length > 0) {
        //    $scope.SelectedTableBookMarksClick($scope.objTableBookMarks.Elements[0]);
        //}

    };
    $scope.selectUserFieldsTab = function () {

        $scope.SelectedCorrespondenceTab = 'UserFields';
        $scope.ActiveTabForCorr = 'Items';
        $scope.objCorrespondenceDetails.IsCommonPropertiesOpen = true;

        if ($scope.objQueryBookMarks && $scope.objQueryBookMarks.Elements.length > 0) {
            $scope.SelectedQueryClick($scope.objQueryBookMarks.Elements[0]);
        }

    };
    $scope.selectTemplatesTab = function () {
        $scope.SelectedCorrespondenceTab = 'Templates';
        $scope.ActiveTabForCorr = 'Entity';
        if ($scope.objChildTemplateBookmarks && $scope.objChildTemplateBookmarks.Elements.length > 0) {
            $scope.SelectedTemplateClick($scope.objChildTemplateBookmarks.Elements[0]);
            if (!$scope.selectedTemplateBookMark.dictAttributes.sfwChildTemplateType) {
                $scope.selectedTemplateBookMark.dictAttributes.sfwChildTemplateType = 'Entity';
            }
        }
    };

    $scope.selectRepeaterControlTab = function () {
        $scope.SelectedCorrespondenceTab = 'RepeaterControl';
        $scope.ActiveTabForCorr = 'Entity';

    };

    $scope.selectRuleTab = function () {
        $scope.SelectedCorrespondenceTab = 'VisibleRule';
        $scope.ActiveTabForCorr = 'Entity';
        if ($scope.objRuleBookmarks && $scope.objRuleBookmarks.Elements.length > 0) {
            $scope.SelectedRuleBookMarkClick($scope.objRuleBookmarks.Elements[0]);

        }
    };


    //#endregion

    //#region UserFieldContextMenu
    $scope.UserFieldMenuOptions = [
        ["Add Row", function () {
            $rootScope.UndRedoBulkOp("Start");
            if ($scope.objQueryForm) {
                var objSfxTableVM;
                if ($scope.objQueryForm.Elements && $scope.objQueryForm.Elements.length > 0) {
                    objSfxTableVM = $scope.objQueryForm.Elements[0];
                }

                if (objSfxTableVM) {
                    var objRow = { Name: 'sfwRow', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    objRow.ParentVM = objSfxTableVM;
                    $rootScope.PushItem(objRow, objSfxTableVM.Elements);
                    var colCnt = GetMaxColCount(objRow, objSfxTableVM);
                    if (colCnt == 0) {
                        var sfxCellModel = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                        sfxCellModel.ParentVM = objRow;
                        $rootScope.PushItem(sfxCellModel, objRow.Elements);
                    }
                    else {
                        for (var colIndex = 0; colIndex < colCnt; colIndex++) {
                            var sfxCellModel = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                            sfxCellModel.ParentVM = objRow;
                            $rootScope.PushItem(sfxCellModel, objRow.Elements);
                        }
                    }
                }

                if (!objSfxTableVM) {
                    objSfxTableVM = { Name: 'sfwTable', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    objSfxTableVM.ParentVM = $scope.objQueryForm;
                    objSfxTableVM.dictAttributes.ID = "tblMain"; // Bug 8462:In correspondence ,for Sfwtable in User field ,the ID is blank.
                    $rootScope.PushItem(objSfxTableVM, $scope.objQueryForm.Elements);

                    var objRow = { Name: 'sfwRow', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    objRow.ParentVM = objSfxTableVM;
                    $rootScope.PushItem(objRow, objSfxTableVM.Elements);

                    var sfxCellModel = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                    sfxCellModel.ParentVM = objRow;
                    $rootScope.PushItem(sfxCellModel, objRow.Elements);
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }, null]
    ];
    //#endregion


    //#region Navigate To File
    $scope.NavigateToCorrespondence = function (aCorrespondenceID) {
        if (aCorrespondenceID && aCorrespondenceID != "") {
            hubMain.server.navigateToFile(aCorrespondenceID, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
    };
    $scope.selectMethodClick = function (aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile($scope.objCorrespondenceDetails.dictAttributes.sfwEntity, "objectmethods", aMethodID);
        }
    };

    //#endregion

    //#region Entity Intellisense
    $scope.onActionKeyDown = function (eargs) {
        var input = eargs.target;
        var entityName = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;

        var data = [];

        var attributes = $rootScope.getEntityAttributeIntellisense(entityName, true);

        data = data.concat(attributes);
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entities = entityIntellisenseList;
        var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));

        if (arrText.length > 0) {
            for (var index = 0; index < arrText.length; index++) {
                var item = data.filter(function (x) { return x.ID == arrText[index]; });
                if (item.length > 0) {
                    if (item[0].Type == "Constant" && item[0].ID == arrText[index]) {
                        // data = $rootScope.getConstants(arrText.join("."));
                        break;
                    }
                    else if (item[0].ID == "RFunc" && arrText[index] == "RFunc") {
                        data = $rootScope.rFuncMethods;
                    }
                    else {
                        if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined" && item[0].ID == arrText[index] && index < arrText.length - 1) {
                            parententityName = item[0].Entity;
                            data = [];
                            while (parententityName != "") {
                                data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, false));
                                var entity = entities.filter(function (x) {
                                    return x.ID == parententityName;
                                });
                                if (entity.length > 0) {
                                    parententityName = entity[0].ParentId;
                                } else {
                                    parententityName = "";
                                }
                            }
                        }

                        else if (item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && item[0].DataType != "AliasObject") {
                            data = [];
                        }
                        else {
                            data = item;
                        }
                    }
                }
            }
        }

        // filter expression
        var item = [];
        if (arrText.length > 0) {
            for (var index = 0; index < arrText.length; index++) {
                item = data.filter(function (x) { if (x.ID) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); } });
            }
            data = item;
        }
        setRuleIntellisense($(input), data);

        if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            eargs.preventDefault();
        }
    };

    //#endregion    

    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objCorrespondenceExtraFields) {
    //        var index = $scope.objCorrespondenceDetails.Elements.indexOf($scope.objCorrespondenceExtraFields);
    //        if (index > -1) {
    //            $scope.objCorrespondenceDetails.Elements.splice(index, 1);
    //        }
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objCorrespondenceExtraFields) {
            var index = $scope.objCorrespondenceDetails.Elements.indexOf($scope.objCorrespondenceExtraFields);
            if (index == -1) {
                $scope.objCorrespondenceDetails.Elements.push($scope.objCorrespondenceExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };

    $scope.ClearSelectFields = function () {
        lstEntityTreeFieldData = null;
    };

    $scope.NavigateToTemplate = function (templateName) {
        if (templateName && templateName.trim() != "") {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = true;
            });
            hubMain.server.navigateToTemplate(templateName).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
                if (data) {
                    $.connection.hubMain.server.navigateToFile(data, "").done(function (objfile) {
                        $rootScope.openFile(objfile, null);
                    });
                }
                else {
                    toastr.error("File not exist.");
                }
            });
        }
    };
    $scope.findParentAndChildObject = function (selectedItem) {
        var findList = [];
        $scope.FindDeepNode($scope.objCorrespondenceDetails, selectedItem, findList);

        $scope.$evalAsync(function () {
            var path = $ValidationService.createFullPath($scope.objCorrespondenceDetails, findList);
            $scope.selectElement(path);
        });
    };
    $scope.validateTemplateName = function (obj, action) {
        var list = [];
        if (action == "onload") {
            list = $scope.validationData[$scope.objCorrespondenceDetails.dictAttributes.sfwEntity];
            if (obj.Name == "sfwChildTemplateBookmark") {
                var entity = "";
                if (obj.dictAttributes.sfwEntityField) {
                    entity = getEntityName(obj.dictAttributes.sfwEntityField, $scope.objCorrespondenceDetails.dictAttributes.sfwEntity, obj);
                } else {
                    entity = $scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
                }
                if ($scope.validationData && $scope.validationData.hasOwnProperty(entity)) {
                    list = $scope.validationData[entity];
                }
            }
        } else list = $scope.lstTemplates;
        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwTemplateName, "sfwTemplateName", "sfwTemplateName", CONST.VALIDATION.INVALID_TEMPLATE, $scope.validationErrorList);
    };
    $scope.OnCutControlClick = function (model) {
        var arr = [];
        arr.push(model);
        $scope.CopyToClipBoard(arr, 'Control', true);
    };

    $scope.OnCopyControlClick = function (model) {
        var arr = [];
        arr.push(model);
        $scope.CopyToClipBoard(arr, 'Control', false);
    };

    $scope.OnPasteControl = function (cellVM) {
        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach($scope.ClipboardData, function (obj) {
                    var model = GetBaseModel(obj);

                    if ($scope.IsCutOper) {
                        $scope.OnDeleteControlClick(obj);
                    }
                    else {
                        model.dictAttributes.ID = GetControlID($scope.objCorrespondenceDetails, obj.Name);
                    }
                    if (obj.Name == "udc") {
                        model.UcChild = [];
                        model.UcChild.push(obj.UcChild[0]);
                    }

                    $rootScope.PushItem(model, cellVM.Elements);
                    model.ParentVM = cellVM;
                });
                if ($scope.IsCutOper) {
                    $scope.ClipboardData = [];
                    $scope.ClipboardDataOpeType = "";
                    $scope.IsCutOper = false;
                }
                $rootScope.UndRedoBulkOp("End");
            });
        }
    };
    //#region Select Control Methods
    $scope.selectControlOnDoubleClick = function (objChild, event) {
        $scope.selectControl(objChild, event);
        $scope.ActiveTabForCorr = 'Properties';
    };


    $scope.selectControl = function (objChild, event) {
        if (objChild) {
            if (!objChild.isLoaded) {
                objChild.isLoaded = true;
            }

            if ($scope.objCorrespondenceDetails.SelectedControl && ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwPanel" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwDialogPanel" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwListView")) {
                $scope.objCorrespondenceDetails.SelectedControl.IsVisible = false;
            }

            SetFormSelectedControl($scope.objCorrespondenceDetails, objChild, event);

            if (objChild.Name == "sfwTabSheet") {
                objChild.ParentVM.SelectedTabSheet = objChild;
                objChild.ParentVM.SelectedTabSheet.IsSelected = true;
            }

            if (objChild.Name == "sfwPanel" || objChild.Name == "sfwDialogPanel" || objChild.Name == "sfwListView") {
                //objChild.initialvisibilty = !objChild.initialvisibilty;
                objChild.IsVisible = true;
            }
        }
        if ($scope.setDisplayNoneToTable) {
            $scope.setDisplayNoneToTable();
        }
    };
    $scope.OnDeleteControlClick = function (aParam) {
        if (aParam) {
            $rootScope.DeleteItem(aParam, aParam.ParentVM.Elements);
            if (aParam.ParentVM) {
                $scope.selectControl(aParam.ParentVM, null);
            }
        }
    };

    $scope.CopyToClipBoard = function (data, opetype, iscutoper) {
        $scope.ClipboardData = [];
        $scope.ClipboardDataOpeType = opetype;
        $scope.IsCutOper = iscutoper;

        angular.forEach(data, function (obj) {
            //var model = GetBaseModel(obj);
            $scope.ClipboardData.push(obj);
        });
    };

    //#region Cut/Copy/Paste Cell

    $scope.OnCutCell = function (cellVM) {
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', true);
    };

    $scope.OnCopyCell = function (cellVM) {
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', false);
    };

    $scope.OnPasteCell = function (cellVM) {
        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach($scope.ClipboardData, function (obj) {
                    if ($scope.IsCutOper) {
                        $scope.ClearCell(obj.ParentVM);
                    }
                    var model = GetBaseModel(obj);
                    if (!$scope.IsCutOper) {
                        model.dictAttributes.ID = GetControlID($scope.objCorrespondenceDetails, obj.Name);
                    }
                    $rootScope.PushItem(model, cellVM.Elements);
                    model.ParentVM = cellVM;
                });
                if ($scope.IsCutOper) {
                    $scope.ClipboardData = [];
                    $scope.ClipboardDataOpeType = "";
                    $scope.IsCutOper = false;
                }
                $rootScope.UndRedoBulkOp("End");
            });
        }
    };
    $scope.ClearCell = function (cellVM) {
        while (cellVM.Elements.length > 0) {
            $rootScope.DeleteItem(cellVM.Elements[0], cellVM.Elements);
        }
    };
    //#endregion

    $scope.OnCopyCutPasteControlClick = function (operation) {
        //cut
        if (operation == "cut") {
            if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl && $scope.objCorrespondenceDetails.SelectedControl.Name != "sfwColumn") {
                $scope.OnCutControlClick($scope.objCorrespondenceDetails.SelectedControl);
            }
            else if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl && ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnCutCell($scope.objCorrespondenceDetails.SelectedControl);
            }
        }
        //copy
        else if (operation == "copy") {
            if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl && $scope.objCorrespondenceDetails.SelectedControl.Name != "sfwColumn") {
                $scope.OnCopyControlClick($scope.objCorrespondenceDetails.SelectedControl);
            }
            else if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl && ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnCopyCell($scope.objCorrespondenceDetails.SelectedControl);
            }
        }
        //paste
        else if (operation == "paste") {
            if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Control" && $scope.objCorrespondenceDetails.SelectedControl && ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnPasteControl($scope.objCorrespondenceDetails.SelectedControl);
            }
            else if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Cell" && $scope.objCorrespondenceDetails.SelectedControl && ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn" || $scope.objCorrespondenceDetails.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnPasteCell($scope.objCorrespondenceDetails.SelectedControl);
            }
        }
    };


    //intellisense for templates
    $scope.onActionKeyDownForTemplates = function (args) {
        var input = args.target;
        if (args.ctrlKey && args.keyCode == $.ui.keyCode.SPACE) {
            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
            args.preventDefault();
        }
        else {
            inputvalue = $(input).val();
            var data = $scope.getIntellisenseDataForTemplatesList(inputvalue);
            setSingleLevelAutoComplete($(input), data);
        }
    };

    $scope.showTemplateNameList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        var data = $scope.getIntellisenseDataForTemplatesList($(input).val());

        if (input && data) {
            input.focus();
            setSingleLevelAutoComplete($(input), data);
            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
        }
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.getIntellisenseDataForTemplatesList = function (value) {
        var data = [];
        if ($scope.lstTemplates) {
            data = $scope.lstTemplates;
            data = data.sort();
            var lstFilterData = [];
            for (var i = 0; i < data.length; i++) {
                if (data[i] && data[i].toLowerCase().indexOf(value) > -1) {
                    lstFilterData.push(data[i]);
                }
            }

        }
        return lstFilterData;
    };

    $scope.OnRowColumnInsertMoveClick = function (operation) {
        if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl) {
            var cellVM = null;
            var tableVM = null;
            if ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn") {
                cellVM = $scope.objCorrespondenceDetails.SelectedControl;
            }
            else {
                cellVM = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwColumn");
            }
            tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;
            if (tableVM && cellVM) {
                $scope.$evalAsync(function () {
                    if (operation == "InsertRowAbove") {
                        var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

                        var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
                        var index = GetIndexToInsert(false, iRowIndex);

                        $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
                    }
                    else if (operation == "InsertRowBelow") {
                        var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

                        var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
                        var index = GetIndexToInsert(true, iRowIndex);
                        $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
                    }
                    else if (operation == "InsertColumnLeft") {
                        var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
                        $scope.InsertColumn(cellVM, false, iColumnIndex, tableVM);
                    }
                    else if (operation == "InsertColumnRight") {
                        var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
                        $scope.InsertColumn(cellVM, true, iColumnIndex, tableVM);
                    }
                    else if (operation == "MoveRowUp") {
                        $scope.MoveRowUp(cellVM, tableVM)
                    }
                    else if (operation == "MoveRowDown") {
                        $scope.MoveRowDown(cellVM, tableVM);
                    }
                    else if (operation == "MoveColumnLeft") {
                        $scope.MoveColumnLeft(cellVM, tableVM);
                    }
                    else if (operation == "MoveColumnRight") {
                        $scope.MoveColumnRight(cellVM, tableVM);
                    }
                });
            }
        }
    };

    //#region Insert Column Left Or Right
    $scope.InsertColumn = function (aParam, isRight, curColIndex, tableVM) {
        function iAddColumn(rowVM) {
            var cellVM;
            for (var i = 0; i < rowVM.Elements.length; i++) {
                var acellvm = rowVM.Elements[i];
                if (rowVM.Elements.indexOf(acellvm) == curColIndex) {
                    cellVM = acellvm;
                    break;
                }
            }

            if (!cellVM) {
                $scope.CheckAndAddColumnsToRow(rowVM, curColIndex, curColIndex);
            }

            if (cellVM) {
                var index = rowVM.Elements.indexOf(cellVM);

                var prefix = "swc";
                var sfxCellModel = {
                    Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {
                    }, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = rowVM;

                if (index < rowVM.Elements.length) {
                    $rootScope.InsertItem(sfxCellModel, rowVM.Elements, index);
                }
                else {
                    $rootScope.PushItem(sfxCellModel, rowVM.Elements);
                }
            }
        }
        function iAddcolumnToRight(rowVM) {
            $scope.CheckAndAddColumnsToRow(rowVM, nextColIndex, curColIndex);
        }
        if (aParam) {

            if (isRight)//inserting next to current column
            {

                var nextColIndex = $scope.GetNextColIndex(aParam);
                $rootScope.UndRedoBulkOp("Start");


                angular.forEach(tableVM.Elements, iAddcolumnToRight);

                $rootScope.UndRedoBulkOp("End");

            }
            else //inserting before to current column
            {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iAddColumn);

                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    $scope.CheckAndAddColumnsToRow = function (arowVM, aintnextColIndex, aintCurColIndex) {
        var colIndex = 0;
        var isAdded = false;
        while (colIndex <= aintnextColIndex) {
            var obj = undefined;
            for (var i = 0; i < arowVM.Elements.length; i++) {
                var cellvm = arowVM.Elements[i];
                if (arowVM.Elements.indexOf(cellvm) == colIndex) {
                    obj = cellvm;
                    break;
                }
            }

            if (obj) {
                colIndex = $scope.GetNextColIndex(obj);
            }

            else {
                var prefix = "swc";
                var sfxCellModel = {
                    Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = arowVM;

                $rootScope.PushItem(sfxCellModel, arowVM.Elements);


                isAdded = true;
                colIndex++;
            }
        }

        if (!isAdded) {
            var prefix = "swc";
            var sfxCellModel = {
                Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
            };
            sfxCellModel.ParentVM = arowVM;


            if (aintnextColIndex < arowVM.Elements.length) {
                $rootScope.InsertItem(sfxCellModel, arowVM.Elements, aintCurColIndex + 1);
            }
            else {
                $rootScope.PushItem(sfxCellModel, arowVM.Elements);
            }
        }
    };

    $scope.GetNextColIndex = function (aCellVM) {

        var nextColIndex = aCellVM.ParentVM.Elements.indexOf(aCellVM);

        var colSpan = $scope.getColspan(aCellVM);
        if (colSpan > 0) {
            nextColIndex = aCellVM.ParentVM.Elements.indexOf(aCellVM) + colSpan;
        }
        else {
            nextColIndex++;
        }

        return nextColIndex;
    };

    $scope.getColspan = function (item) {
        if (item && item.dictAttributes.ColumnSpan && parseInt(item.dictAttributes.ColumnSpan)) {
            return item.dictAttributes.ColumnSpan;
        }
        else {
            return 1;
        }
    };


    //#endregion

    //#region Move Row/Column Up/Down

    $scope.MoveRowUp = function (aParam, tableVM) {
        if (aParam) {
            var cellVM = aParam;
            var RowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);
            if (RowIndex > 0) {
                //Removing
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(cellVM.ParentVM, tableVM.Elements);

                //Adding
                $rootScope.InsertItem(cellVM.ParentVM, tableVM.Elements, RowIndex - 1);
                $rootScope.UndRedoBulkOp("End");

            }
        }
    };

    $scope.MoveRowDown = function (aParam, tableVM) {
        if (aParam) {
            {
                var cellVM = aParam;

                var RowCount = tableVM.Elements.length;
                var RowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);
                if (RowIndex < RowCount - 1) {

                    //Removing
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(cellVM.ParentVM, tableVM.Elements);


                    //Adding
                    $rootScope.InsertItem(cellVM.ParentVM, tableVM.Elements, RowIndex + 1);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
    };

    $scope.MoveColumnLeft = function (aParam, tableVM) {
        function iMoveColumnLeft(rowVM) {
            var model = rowVM.Elements[ColIndex];
            if (model) {
                //Removing
                $rootScope.DeleteItem(rowVM.Elements[ColIndex], rowVM.Elements);

                //Adding
                $rootScope.InsertItem(model, rowVM.Elements, ColIndex - 1);
            }
        }

        if (aParam) {
            var ColIndex = aParam.ParentVM.Elements.indexOf(aParam);
            if (ColIndex > 0) {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iMoveColumnLeft);

                $rootScope.UndRedoBulkOp("End");

            }
        }
    };

    $scope.MoveColumnRight = function (aParam, tableVM) {
        function iMoveColumnRight(rowVM) {
            var model = rowVM.Elements[ColIndex];
            if (model) {
                //Removing
                $rootScope.DeleteItem(rowVM.Elements[ColIndex], rowVM.Elements);

                //Adding
                $rootScope.InsertItem(model, rowVM.Elements, ColIndex + 1);
            }
        }

        if (aParam) {
            var ColCount = GetMaxColCount(aParam.ParentVM, tableVM);
            var ColIndex = aParam.ParentVM.Elements.indexOf(aParam);
            if (ColIndex < ColCount - 1) {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iMoveColumnRight);

                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    //#endregion

    //#region Functions for Clear Panel, Delete Row and Delete Column.

    $scope.clearCell = function (cell) {
        while (cell.Elements.length > 0) {
            $rootScope.DeleteItem(cell.Elements[0], cell.Elements);
        }
    }
    $scope.clearPanel = function () {
        if ($scope.objCorrespondenceDetails.SelectedControl) {
            var table = null;
            if ($scope.objCorrespondenceDetails.SelectedControl.Name === "sfwPanel") {
                table = $scope.objCorrespondenceDetails.SelectedControl.Elements[0];
            }
            else {
                table = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwTable", true);
            }

            if (table) {
                $rootScope.UndRedoBulkOp("Start");
                for (var rowIndex = 0; rowIndex < table.Elements.length; rowIndex++) {
                    var row = table.Elements[rowIndex];
                    if (row) {
                        for (var cellIndex = 0; cellIndex < row.Elements.length; cellIndex++) {
                            $scope.clearCell(row.Elements[cellIndex]);
                        }
                    }
                }
                if ($scope.objCorrespondenceDetails.SelectedControl.Name !== "sfwPanel" && $scope.objCorrespondenceDetails.SelectedControl.Name !== "sfwTable" && $scope.objCorrespondenceDetails.SelectedControl.Name !== "sfwColumn") {
                    var parentCell = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwColumn", true);
                    if (parentCell) {
                        $scope.selectControl(parentCell);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    }
    $scope.deleteRow = function () {
        if ($scope.objCorrespondenceDetails.SelectedControl) {
            var row = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwRow", true);
            if (row && row.ParentVM && row.ParentVM.Elements) {
                if (row.ParentVM.Elements.length > 1) {

                    var rowIndex = row.ParentVM.Elements.indexOf(row);
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(row, row.ParentVM.Elements);

                    if (rowIndex === row.ParentVM.Elements.length) {
                        rowIndex--;
                    }

                    //selecting the first cell of next row.
                    if (rowIndex > -1 && row.ParentVM.Elements.length > 0 && row.ParentVM.Elements[rowIndex].Elements.length > 0) {
                        $scope.selectControl(row.ParentVM.Elements[rowIndex].Elements[0]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    alert("Atleast one row should be present");
                }
            }
        }
    }
    $scope.deleteColumn = function () {
        if ($scope.objCorrespondenceDetails.SelectedControl) {
            var cell = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwColumn", true);
            if (cell && cell.ParentVM && cell.ParentVM.ParentVM) {
                var table = cell.ParentVM.ParentVM;
                var ColCount = GetMaxColCount(cell.ParentVM, table);
                if (ColCount > 1) {
                    var colIndex = cell.ParentVM.Elements.indexOf(cell);

                    var deleteCellFromRow = function (row) {
                        if (row.Elements.length > colIndex) {
                            $rootScope.DeleteItem(row.Elements[colIndex], row.Elements);
                            if (row.Elements && row.Elements.length == 0 && row.ParentVM) {
                                $rootScope.DeleteItem(row, row.ParentVM.Elements);
                            }
                        }
                    };

                    $rootScope.UndRedoBulkOp("Start");

                    angular.forEach(table.Elements, deleteCellFromRow);

                    if (colIndex === cell.ParentVM.Elements.length) {
                        colIndex--;
                    }

                    //selecting the cell just after the selected cell.
                    if (cell.ParentVM.Elements.length > colIndex) {
                        $scope.selectControl(cell.ParentVM.Elements[colIndex]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    alert("Atleast one column should be present");
                }
            }
        }
    }

    //#endregion

    //#region Insert Cell / Delete Cell

    $scope.InsertCell = function (arowVM, acellVM, isRight) {
        var icurColIndex = -1;
        if (isRight) {
            icurColIndex = arowVM.Elements.indexOf(acellVM) + 1;
        }
        else {
            icurColIndex = arowVM.Elements.indexOf(acellVM);

            if (icurColIndex > 0) {
                icurColIndex -= icurColIndex;
            }
        }
        if (icurColIndex > -1) {
            var prefix = "swc";
            var sfxCellModel = {
                Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {
                }, Elements: [], Children: []
            };
            sfxCellModel.ParentVM = arowVM;

            if (icurColIndex < arowVM.Elements.length) {
                $rootScope.InsertItem(sfxCellModel, arowVM.Elements, icurColIndex);
            }
            else {
                $rootScope.PushItem(sfxCellModel, arowVM.Elements);
            }
        }
    };

    $scope.deleteCellClick = function () {
        if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl) {
            if ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwPanel" && $scope.objCorrespondenceDetails.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                //var objGridView = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwGridView");
                if ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.objCorrespondenceDetails.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwColumn");
                }

                $scope.deleteCell(cellVM);
            }
        }
    };

    $scope.deleteCell = function (aCellVM) {
        if (aCellVM.ParentVM && aCellVM.ParentVM.ParentVM && aCellVM.ParentVM.ParentVM.Name == 'sfwTable' && aCellVM.ParentVM.ParentVM.Elements.length == 1 && aCellVM.ParentVM.Elements.length == 1) {
            alert("Cannot delete as only one cell present.")
        }
        else {
            $rootScope.DeleteItem(aCellVM, aCellVM.ParentVM.Elements);
            if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl && $scope.objCorrespondenceDetails.SelectedControl.ParentVM) {
                if ($scope.objCorrespondenceDetails.SelectedControl.ParentVM.Elements.length == 0 && $scope.objCorrespondenceDetails.SelectedControl.ParentVM.Name == "sfwRow") {
                    $rootScope.DeleteItem($scope.objCorrespondenceDetails.SelectedControl.ParentVM, $scope.objCorrespondenceDetails.SelectedControl.ParentVM.ParentVM.Elements);
                    $scope.selectControl($scope.objCorrespondenceDetails.SelectedControl.ParentVM.ParentVM, null);
                } else {
                    $scope.selectControl($scope.objCorrespondenceDetails.SelectedControl.ParentVM, null);
                }
            }
        }
    };

    $scope.OnInsertCellClick = function (operation) {
        if ($scope.objCorrespondenceDetails && $scope.objCorrespondenceDetails.SelectedControl) {
            if ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwPanel" && $scope.objCorrespondenceDetails.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                var tableVM = null;
                //var objGridView = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwGridView");
                if ($scope.objCorrespondenceDetails.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.objCorrespondenceDetails.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.objCorrespondenceDetails.SelectedControl, "sfwColumn");
                }
                tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;
                if (tableVM && cellVM) {
                    $scope.$evalAsync(function () {
                        if (operation == "InsertCellLeft") {
                            $scope.InsertCell(cellVM.ParentVM, cellVM, false);
                        }
                        else if (operation == "InsertCellRight") {
                            $scope.InsertCell(cellVM.ParentVM, cellVM, true);
                        }
                    });
                }
            }
        }
    };

    //#endregion


    $scope.PopulateCorrespondenceTemplate = function () {
        $rootScope.IsLoading = true;

        $.connection.hubCorrespondence.server.getListOfAllTemplates().done(function (data) {
            $scope.$apply(function () {
                $scope.CorTemplateCollection = data;
                $rootScope.IsLoading = false;
            });

        });
    };
    //#region Implementation of  Tests Tab

    //#region Test Tab click
    $scope.selectTestsTab = function () {
        $scope.SelectedCorrespondenceTab = 'Tests';
        $scope.ActiveTabForCorr = 'SubTests';
    }
    //#endregion

    //#region Test Tab click
    $scope.selectTestsTab = function () {
        if (!(ConfigurationFactory.getLastProjectDetails() && ConfigurationFactory.getLastProjectDetails().ScenarioXmlFileLocation)) {
            toastr.warning("Check the Scenario XML File Location in settings file.");
        }
        $scope.SelectedCorrespondenceTab = 'Tests';
        if ($scope.objCorrespondenceTests.Elements && $scope.objCorrespondenceTests.Elements.length > 0) {
            $scope.onTestClick($scope.objCorrespondenceTests.Elements[0]);
        }
        $scope.ActiveTabForCorr = 'SubTests';
    };
    //#endregion

    //#region Add Test
    $scope.AddCorrespondenceTest = function () {
        if (ConfigurationFactory.getLastProjectDetails() && ConfigurationFactory.getLastProjectDetails().ScenarioXmlFileLocation) {
            var fields = undefined;
            var newScope = $scope.$new();
            newScope.openNewAddTest = $rootScope.showDialog(newScope, "New Correspondence Test", "Correspondence/views/AddNewTest.html");

            newScope.addTest = function () {
                $rootScope.IsLoading = true;
                if ($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate) {
                    $rootScope.UndRedoBulkOp("Start");

                    var obj = { Name: 'sfwCorrespondenceTest', value: '', dictAttributes: { ID: newScope.newTest, sfwIsObjectBased: newScope.isObjectBased }, Elements: [] };
                    if (newScope.isObjectBased) {
                        obj.dictAttributes.sfwLoadSource = "form";
                        obj.dictAttributes.sfwAssociatedFormName = $scope.ilstAssociatedForms.length > 0 ? $scope.ilstAssociatedForms[0] : "";
                    }
                    newScope.retriveBookmarks(obj);
                }
                else {
                    alert("Template name is not there.");
                }
            };
            newScope.retriveBookmarks = function (obj) {
                $.connection.hubCorrespondence.server.retrieveBookmarksOfTemplate($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate).done(function (data) {
                    var lstAllBookMarks = data;
                    if (lstAllBookMarks && lstAllBookMarks.length > 0) {
                        $scope.LoadBookmarks(lstAllBookMarks, obj, fields);

                    }
                    $scope.$evalAsync(function () {
                        $rootScope.PushItem(obj, $scope.objCorrespondenceTests.Elements);
                    });
                    $scope.onTestClick(obj);
                    if (newScope.isObjectBased) {
                        $scope.updateTestParameters();
                    }
                });
                $rootScope.UndRedoBulkOp("End");

                newScope.closeTest();
                $rootScope.IsLoading = false;
            };
            newScope.closeTest = function () {
                newScope.openNewAddTest.close();
            };
            newScope.validateTest = function () {
                newScope.testErrorMessageForDisplay = "";
                var retValue = false;
                if (!newScope.newTest) {
                    newScope.testErrorMessageForDisplay = "Error: Test name is mandatory."
                    retValue = true;
                }
                else if (newScope.newTest) {
                    if ($scope.objCorrespondenceTests.Elements.length > 0) {
                        if ($scope.objCorrespondenceTests.Elements.some(function (x) { return x.dictAttributes.ID == newScope.newTest })) {
                            newScope.testErrorMessageForDisplay = "Error: Duplicate name is not allowed."
                            retValue = true;
                        }
                    }

                }
                return retValue;
            };
        }
        else {
            toastr.warning("Check the Scenario XML File Location in settings file.");
        }
    };
    //#endregion

    //#region Load BookMarks
    $scope.LoadBookmarks = function (bookmarks, obj, fields) {
        $scope.$evalAsync(function () {
            if (obj.dictAttributes.sfwIsObjectBased !== 'True') {
                var simpleBookmarks = bookmarks.filter(function (x) { return x.Type === BookmarkType.Simple || x.Type == BookmarkType.ConditionalBlock });
                if (simpleBookmarks && simpleBookmarks.length > 0)
                    $scope.CreateTestFieldForFieldBookmarks(simpleBookmarks, obj, fields);
            }

            if ($scope.objQueryBookMarks) {
                var elements = $scope.objQueryBookMarks.Elements;
                $scope.CreateTestFields(elements, "Query", obj, fields);

                $scope.getContorlFromQueryOnLoad(obj);

            }
            if ($scope.objTableBookMarks && obj.dictAttributes.sfwIsObjectBased !== 'True') {
                var elements = $scope.objTableBookMarks.Elements;
                $scope.CreateTestFields(elements, "Table", obj, fields);
            }
            if ($scope.objChildTemplateBookmarks && obj.dictAttributes.sfwIsObjectBased !== 'True') {
                var elements = $scope.objChildTemplateBookmarks.Elements;
                $scope.CreateTestFields(elements, "Template", obj, fields);
            }

            if ($scope.objRuleBookmarks && obj.dictAttributes.sfwIsObjectBased !== 'True') {
                var elements = $scope.objRuleBookmarks.Elements;
                $scope.CreateTestFields(elements, "Rule", obj, fields);
            }

            if ($scope.objRepeaterBookMarks && obj.dictAttributes.sfwIsObjectBased !== 'True') {
                var elements = $scope.objRepeaterBookMarks.Elements;
                $scope.CreateTestFields(elements, "sfwRepeater", obj, fields);
            }
        });
    };

    //#endregion

    //#region Create Test Fields For Field Bookmarks
    $scope.CreateTestFieldForFieldBookmarks = function (simpleBookmarks, obj, oldFields) {

        angular.forEach(simpleBookmarks, function (item) {
            var field = { Name: 'sfwCorrespondenceTestField', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            field.dictAttributes.ID = item.Name;
            field.dictAttributes.sfwBookmarkType = "Field";
            obj.Elements.push(field);

            if (oldFields) {
                var oldBookMark = oldFields.filter(function (x) { return x.dictAttributes.ID == item.Name });
                if (oldBookMark && oldBookMark.length > 0) {
                    field.dictAttributes.Value = oldBookMark[0].dictAttributes.Value;
                }
            }
        });

    }
    //#endregion

    //#region Create Test Fields

    $scope.CreateTestFields = function (elements, bookMarkType, obj, oldFields) {
        angular.forEach(elements, function (item) {
            var field = { Name: 'sfwCorrespondenceTestField', value: '', dictAttributes: {}, Elements: [] };
            field.dictAttributes.ID = item.dictAttributes.ID;
            field.dictAttributes.sfwBookmarkType = bookMarkType;
            if (item.Name && (item.Name.indexOf("sfwTableBookMark") > -1 || (item.Name.indexOf("sfwRepeater") > -1))) {
                field.dictAttributes.sfwDataType = "Collection";
            }
            else if (field.dictAttributes.sfwDataType) {
                field.dictAttributes.sfwDataType = item.dictAttributes.sfwDataType;
            }
            else if (bookMarkType == "Template" && item.dictAttributes.sfwChildTemplateType) {
                field.dictAttributes.sfwDataType = item.dictAttributes.sfwChildTemplateType;
            }
            else if (bookMarkType == "Rule" && item.Name && item.Name.indexOf("sfwRuleBookmark") > -1) {
                field.dictAttributes.sfwRuleID = item.dictAttributes.sfwRuleID;
            }
            obj.Elements.push(field);

            //on Synchronize old bookmark values are retained
            if (oldFields) {
                var oldBookMark = oldFields.filter(function (x) { return x.dictAttributes.ID == item.dictAttributes.ID });
                if (oldBookMark && oldBookMark.length > 0) {
                    field.dictAttributes.Value = oldBookMark[0].dictAttributes.Value;

                    var tableVM = obj.Elements[obj.Elements.length - 1];
                    if (item.Name) {
                        if (item.Name.indexOf("sfwTableBookMark") > -1) {
                            $scope.SetCollectionValuesOnSynchronize(field, oldBookMark[0], tableVM, item);
                        }
                        else if (item.Name.indexOf("sfwRepeater") > -1) {
                            $scope.SetCollectionValuesOnSynchronize(field, oldBookMark[0], tableVM, item);
                        }
                        else if (item.Name.indexOf("sfwChildTemplateBookmark") > -1) {
                            if (item.dictAttributes.sfwChildTemplateType && item.dictAttributes.sfwChildTemplateType.indexOf("Entity") > -1) {
                                angular.forEach(oldBookMark[0].Elements, function (templateBookmark) {                                       //add sfwCorrespondenceTestField  

                                    var tableColumnField = { Name: 'sfwCorrespondenceTestField', value: '', dictAttributes: {}, Elements: [] };
                                    tableColumnField.dictAttributes.ID = templateBookmark.dictAttributes.ID;
                                    tableColumnField.dictAttributes.sfwDataType = templateBookmark.dictAttributes.sfwDataType;
                                    tableColumnField.dictAttributes.Value = templateBookmark.dictAttributes.Value;
                                    if (templateBookmark.dictAttributes.sfwBookmarkType) {
                                        tableColumnField.dictAttributes.sfwBookmarkType = templateBookmark.dictAttributes.sfwBookmarkType;
                                    }
                                    tableVM.Elements.push(tableColumnField);

                                    //retain Table data of Object Template
                                    var objectElement = tableVM.Elements[tableVM.Elements.length - 1];

                                    $scope.setChildTempleteEntityOnSync(templateBookmark, objectElement);

                                });
                            }
                            else if (item.dictAttributes.sfwChildTemplateType && item.dictAttributes.sfwChildTemplateType.indexOf("Collection") > -1) {
                                $scope.SetCollectionValuesOnSynchronize(field, oldBookMark[0], tableVM, item);
                            }
                        }
                    }
                }
            }
        });

    }


    $scope.setChildTempleteEntityOnSync = function (templateBookmark, objectElement) {
        if (templateBookmark.Elements.length > 0) {
            angular.forEach(templateBookmark.Elements, function (objectTableRow) {
                if (objectTableRow.Name === "sfwCorrespondenceTestFields") {
                    var row = { Name: 'sfwCorrespondenceTestFields', value: '', dictAttributes: {}, Elements: [] };
                    row.dictAttributes["indexNumber"] = objectTableRow["indexNumber"];
                    objectElement.Elements.push(row);

                    angular.forEach(objectTableRow.Elements, function (column) {
                        var tableColField = { Name: 'sfwCorrespondenceTestField', value: '', dictAttributes: {}, Elements: [] };
                        tableColField.dictAttributes.ID = column.dictAttributes.ID;
                        tableColField.dictAttributes.Value = column.dictAttributes.Value;
                        if (column.dictAttributes.sfwBookmarkType) {
                            tableColField.dictAttributes.sfwBookmarkType = column.dictAttributes.sfwBookmarkType;
                        }
                        if (column.Elements && column.Elements.length > 0) {
                            $scope.setChildTempleteEntityOnSync(column, tableColField);
                        }
                        row.Elements.push(tableColField);
                    });
                }
                else {
                    var tableColField = { Name: 'sfwCorrespondenceTestField', value: '', dictAttributes: {}, Elements: [] };
                    tableColField.dictAttributes.ID = objectTableRow.dictAttributes.ID;
                    tableColField.dictAttributes.Value = objectTableRow.dictAttributes.Value;
                    if (objectTableRow.dictAttributes.sfwBookmarkType) {
                        tableColField.dictAttributes.sfwBookmarkType = objectTableRow.dictAttributes.sfwBookmarkType;
                    }
                    if (objectTableRow.Elements && objectTableRow.Elements.length > 0) {
                        $scope.setChildTempleteEntityOnSync(objectTableRow, tableColField);
                    }
                    objectElement.Elements.push(tableColField);
                }
            });
        }
    }
    //#endregion

    //#region Set Collection Value for Syncronise

    $scope.SetCollectionValuesOnSynchronize = function (field, oldBookMark, tableVM, repeateritem) {

        var iaddedrows = oldBookMark.Elements.length;
        for (var i = 0; i < iaddedrows; i++) {
            var tableRowField = { Name: 'sfwCorrespondenceTestFields', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            tableRowField.dictAttributes["indexNumber"] = i;
            tableVM.Elements.push(tableRowField);
            angular.forEach(repeateritem.Elements, function (rptrheader) {
                if (tableVM.dictAttributes.sfwBookmarkType == "Table") {
                    if (rptrheader.dictAttributes.sfwColumnHeader) {
                        var tableColumnField = { Name: 'sfwCorrespondenceTestField', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                        tableColumnField.dictAttributes.sfwColumnHeader = rptrheader.dictAttributes.sfwColumnHeader;
                        tableColumnField.dictAttributes.sfwDataType = rptrheader.dictAttributes.sfwDataType;
                        for (var j = 0; j < oldBookMark.Elements.length; j++) {
                            if (oldBookMark.Elements[j].dictAttributes.indexNumber == i) {
                                for (var k = 0; k < oldBookMark.Elements[j].Elements.length; k++) {
                                    if (oldBookMark.Elements[j].Elements[k].dictAttributes.sfwColumnHeader == rptrheader.dictAttributes.sfwColumnHeader) {
                                        tableColumnField.dictAttributes.Value = oldBookMark.Elements[j].Elements[k].dictAttributes.Value;

                                        break;
                                    }
                                }
                                break;

                            }
                        }
                        tableRowField.Elements.push(tableColumnField);
                    }

                }
                else {
                    if (rptrheader.dictAttributes.ID) {
                        var tableColumnField = { Name: 'sfwCorrespondenceTestField', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                        tableColumnField.dictAttributes.ID = rptrheader.dictAttributes.ID;
                        tableColumnField.dictAttributes.sfwDataType = rptrheader.dictAttributes.sfwDataType;
                        for (var j = 0; j < oldBookMark.Elements.length; j++) {
                            if (oldBookMark.Elements[j].dictAttributes.indexNumber == i) {
                                for (var k = 0; k < oldBookMark.Elements[j].Elements.length; k++) {
                                    if (oldBookMark.Elements[j].Elements[k].dictAttributes.ID == rptrheader.dictAttributes.ID) {
                                        tableColumnField.dictAttributes.Value = oldBookMark.Elements[j].Elements[k].dictAttributes.Value;

                                        break;
                                    }
                                }
                                break;

                            }
                        }
                        tableRowField.Elements.push(tableColumnField);
                    }
                }
            });
        }



    }
    //#endregion

    //#region Delete Test

    $scope.DeleteCorrespondenceTest = function () {
        var result = confirm("Do you want to remove the selected test '" + $scope.SetSelectedTest.dictAttributes.ID + "'from this section ?");
        if (result) {
            var index = $scope.objCorrespondenceTests.Elements.indexOf($scope.SetSelectedTest);
            if (index > -1) {
                $rootScope.DeleteItem($scope.SetSelectedTest, $scope.objCorrespondenceTests.Elements, "onTestClick");
                if (index < $scope.objCorrespondenceTests.Elements.length) {
                    $scope.onTestClick($scope.objCorrespondenceTests.Elements[index]);
                }
                else if ($scope.objCorrespondenceTests.Elements.length > 0) {
                    $scope.onTestClick($scope.objCorrespondenceTests.Elements[index - 1]);
                }
                else {
                    $scope.onTestClick($scope.objCorrespondenceTests);
                }

            }
            if ($scope.objCorrespondenceTests.Elements.length == 0) {
                $scope.SetSelectedTest = undefined;
            }
        }

    }

    //#endregion

    //#region On Test Click (Left Panel)
    $scope.onTestClick = function (obj) {
        $rootScope.IsLoading = true;
        $scope.SetSelectedTest = obj;
        $scope.getContorlFromQueryOnLoad(obj);
        $rootScope.IsLoading = false;


    }
    //#endregion


    //#region On Input Click (Right Panel)
    $scope.onInputClick = function (obj) {
        $scope.SetSelectedInput = obj;

    }
    //#endregion


    //#region on Add Item while data Type is collection
    $scope.onAddItems = function (obj) {
        $scope.SetSelectedInput = obj;
        OpenPopup(obj, $scope, $scope.objCorrespondenceDetails);

    }

    //#endregion


    //#region Get all control from User Field

    $scope.getContorlFromQueryOnLoad = function (obj) {
        var controlType = [];
        var data = [];

        var elements = $scope.objQueryBookMarks.Elements;
        if (elements && elements.length > 0) {
            data = $scope.getControlForQuery(elements);
            if (obj && obj.Elements.length > 0 && data.length > 0) {
                var lstQuery = obj.Elements.filter(function (x) {
                    return x.dictAttributes.sfwBookmarkType === "Query"
                });
                if (lstQuery && lstQuery.length > 0 && data.length > 0) {
                    angular.forEach(lstQuery, function (query) {
                        controlType = data.filter(function (x) {
                            return x.dictAttributes.sfwQueryID == query.dictAttributes.ID
                        });
                        if (controlType && controlType.length > 0) {
                            query.controlName = controlType[0].Name;
                            if (query.controlName === "sfwDropDownList" || query.controlName === "sfwCheckBoxList" || query.controlName === "sfwRadioButtonList" || query.controlName === "sfwMultiSelectDropDownList") {
                                if (!controlType[0].dictAttributes.sfwLoadType) {
                                    query.LoadType = "Items";
                                }
                                else {
                                    query.LoadType = controlType[0].dictAttributes.sfwLoadType;
                                }
                                query.lstItems = [];

                                if (controlType[0].Elements.length > 0) {
                                    angular.forEach(controlType[0].Elements, function (itm) {
                                        if (itm.Name == 'ListItem') {
                                            query.lstItems.push(itm);

                                        }

                                    });
                                }
                                else if (controlType[0].dictAttributes.sfwLoadType == "CodeGroup" && controlType[0].dictAttributes.sfwLoadSource != "0") {
                                    $.connection.hubMain.server.getCodeValuesForDropDown(controlType[0].dictAttributes.sfwLoadSource).done(function (data) {
                                        $scope.$evalAsync(function () {
                                            if (data && data.length > 0) {
                                                angular.forEach(data, function (itm) {
                                                    query.lstItems.push({ dictAttributes: { Text: itm.CodeValueDescription, Value: itm.CodeValue } });

                                                });
                                            }
                                        })
                                    });
                                }
                            }


                        }

                    });

                }
            }
        }
    }

    $scope.getControlForQuery = function (element) {
        var lst = [];
        angular.forEach($scope.objQueryForm.Elements, function (tbl) {

            if (tbl) {
                angular.forEach(tbl.Elements, function (row) {
                    if (row) {
                        angular.forEach(row.Elements, function (col) {

                            if (col) {
                                angular.forEach(element, function (ele) {

                                    lst = lst.concat(col.Elements.filter(function (x) { return x.dictAttributes.sfwQueryID == ele.dictAttributes.ID }));
                                });

                            }

                        });
                    }
                });
            }
        });
        return lst;
    }
    //#endregion

    $scope.validateObjectBasedTest = function () {
        $scope.istrObjectBasedTestError = "";
        if ($scope.SetSelectedTest.dictAttributes.sfwIsObjectBased === "True") {
            if ($scope.SetSelectedTest.dictAttributes.sfwLoadSource === "form" && !$scope.SetSelectedTest.dictAttributes.sfwAssociatedFormName) {
                $scope.istrObjectBasedTestError = "Please select an associated form.";
            }
            else if ($scope.SetSelectedTest.dictAttributes.sfwLoadSource === "xmlmethod" && !$scope.SetSelectedTest.dictAttributes.sfwXmlMethod) {
                $scope.istrObjectBasedTestError = "Please select an Xml Method.";
            }
        }
    };
    $scope.populateAssociatedForms = function () {
        $scope.ilstAssociatedForms = [];
        $.connection.hubCorrespondence.server.loadAssociatedForms($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate, $scope.objCorrespondenceDetails.dictAttributes.sfwType).done(function (data) {
            $scope.$apply(function () {
                if (data && data.length) {
                    $scope.ilstAssociatedForms = data.map(function (itm) { return itm.FormName; });
                }
            });
        });
    };
    $scope.populateXmlMethods = function () {
        $scope.ilstXmlMethods = [];
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        if (entityIntellisenseList) {
            var lobjEntity = entityIntellisenseList.filter(function (x) { return x.ID === $scope.objCorrespondenceDetails.dictAttributes.sfwEntity; })[0];
            if (lobjEntity) {
                angular.forEach(lobjEntity.XmlMethods, function (item) {
                    if (item.MethodType.toLowerCase() === "load" && (item.Mode.toLowerCase() === "update" || item.Mode.toLowerCase() === "all")) {
                        $scope.ilstXmlMethods.push(item);
                    }
                });
            }
        }
    };
    $scope.updateXmlMethodParameters = function () {
        var lobjXmlMethod = $scope.ilstXmlMethods.filter(function (method) { return method.ID === $scope.SetSelectedTest.dictAttributes.sfwXmlMethod; })[0];
        $rootScope.UndRedoBulkOp("Start");

        //Get Existing Parameters
        var existingParams = $scope.SetSelectedTest.Elements.filter(function (itm) { return itm.Name === "parameter"; });

        if (lobjXmlMethod && lobjXmlMethod.Parameters) {
            for (var idx = 0; idx < lobjXmlMethod.Parameters.length; idx++) {
                //Check if a new parameter already exists, if not, then add it.
                if (!existingParams.some(function (itm) { return itm.dictAttributes.ID === lobjXmlMethod.Parameters[idx].ID; })) {
                    var objParam = {
                        Name: "parameter", dictAttributes: { ID: lobjXmlMethod.Parameters[idx].ID }, Elements: []
                    };
                    $rootScope.PushItem(objParam, $scope.SetSelectedTest.Elements);
                }
            }
        }

        //Check if any of the existing parameters doesn't contain in new parameters, if not, then remove it.
        for (idx = 0; idx < existingParams.length; idx++) {
            if (!(lobjXmlMethod && lobjXmlMethod.Parameters && lobjXmlMethod.Parameters.some(function (itm) { return itm.ID === existingParams[idx].dictAttributes.ID; }))) {
                $rootScope.DeleteItem(existingParams[idx], $scope.SetSelectedTest.Elements);
            }
        }
        $rootScope.UndRedoBulkOp("End");
        $rootScope.IsLoading = false;
    };
    $scope.updateTestParameters = function () {
        $rootScope.IsLoading = true;
        $scope.validateObjectBasedTest();
        if ($scope.SetSelectedTest.dictAttributes.sfwLoadSource === "xmlmethod") {
            $scope.updateXmlMethodParameters();
        }
        else {
            $.connection.hubForm.server.getFormParameters($scope.SetSelectedTest.dictAttributes.sfwAssociatedFormName, "Update").done(function (lstparams) {
                $scope.$apply(function () {
                    if (lstparams) {
                        $rootScope.UndRedoBulkOp("Start");
                        //Get Existing Parameters
                        var existingParams = $scope.SetSelectedTest.Elements.filter(function (itm) { return itm.Name === "parameter"; });

                        for (var idx = 0; idx < lstparams.length; idx++) {
                            //Check if a new parameter already exists, if not, then add it.
                            if (!existingParams.some(function (itm) { return itm.dictAttributes.ID === lstparams[idx].ParmeterField; })) {
                                var objParam = {
                                    Name: "parameter", dictAttributes: { ID: lstparams[idx].ParmeterField }, Elements: []
                                };
                                $rootScope.PushItem(objParam, $scope.SetSelectedTest.Elements);
                            }
                        }

                        //Check if any of the existing parameters doesn't contain in new parameters, if not, then remove it.
                        for (idx = 0; idx < existingParams.length; idx++) {
                            if (!lstparams.some(function (itm) { return itm.ParmeterField === existingParams[idx].dictAttributes.ID; })) {
                                $rootScope.DeleteItem(existingParams[idx], $scope.SetSelectedTest.Elements);
                            }
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                    $rootScope.IsLoading = false;
                });
            });
        }
    };
    //#endregion


    //#region Run Logic

    $scope.onRunClick = function () {
        $rootScope.IsLoading = true;
        var obj = GetBaseModel($scope.objCorrespondenceDetails);
        var selectedTest = GetBaseModel($scope.SetSelectedTest);
        var objTest = { mainModel: obj, selectedTestStep: selectedTest, templateName: $scope.objCorrespondenceDetails.dictAttributes.sfwTemplate };
        if (objTest != null) {
            var strobj = JSON.stringify(objTest);
            if (strobj.length < 32000) {
                $.connection.hubCorrespondence.server.onOpenCorrTestWord($scope.objCorrespondenceDetails.dictAttributes.sfwTemplate, selectedTest, obj);

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


                SendDataPacketsToServerTest(lstDataPackets);
            }
        }

    }

    var SendDataPacketsToServerTest = function (lstpackets) {

        for (var i = 0; i < lstpackets.length; i++) {
            $.connection.hubCorrespondence.server.receiveDataPacketsForTestCorrespondence(lstpackets[i], lstpackets.length, i);
        }

        //#endregion
    };
}]);

app.directive("collectiondirectivefortest", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function (modelobject, objselectedstep) {

        var template = '<td valign="top" ng-repeat="item in items.Elements" ng-model="SelectedTestField">';
        template += '  <table><tr>';
        template += ' <td><input type="text" class="form-control form-filter input-sm min-width-80" title="{{item.dictAttributes.Value}}" ng-model="item.dictAttributes.Value" ng-hide="item.dictAttributes.sfwBookmarkType == \'Template\' || item.dictAttributes.sfwBookmarkType == \'Table\'"/> </td>';
        template += '  <td> <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopup(item)" ng-show="item.dictAttributes.sfwDataType == \'Collection\' && item.dictAttributes.sfwBookmarkType == \'Template\'"/></a> </td>';
        template += '  <td> <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopup(item)" ng-show="item.dictAttributes.sfwBookmarkType == \'Table\'"/></a> </td>';
        template += '  <td> <a><img src="images/Common/icon_add_dark.svg"  ng-click="OpenPopup(item)" ng-show="item.dictAttributes.sfwDataType == \'Entity\' && item.dictAttributes.sfwBookmarkType == \'Template\'"/></a></td></tr> ';
        template += '</table></td>';


        return template;
    };

    return {
        restrict: "A",
        replace: true,
        scope: {
            items: '=',
            model: '=',
            mainmodel: '=',
            objselectedstep: "=",

        },
        link: function (scope, element, attrs) {

            element.html(getTemplate(scope.testfieldmodel, scope.objselectedstep));
            $compile(element.contents())(scope);

            //#region Methods For Open popups  for collection and Object

            scope.OpenPopup = function (objField) {
                OpenPopup(objField, scope, scope.mainmodel);

            };


        }
    };
}]);

//#region Drop for EntityField for Data Fields

function onEntityFieldDropForDataFields(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
        var dataType = scp.$parent.objCorrespondenceDetails.SelecetdField.DataType;
        var strDataFormat = null;

        while (field.objParent && !field.objParent.IsMainEntity) {
            field = field.objParent;
            value = [field.ID, value].join(".");
        }
        if (dataType && dataType.toLowerCase() == "datetime") {
            strDataFormat = "{0:d}";
        }
        else if (dataType && dataType.toLowerCase() == "decimal" && (field.ID.toLowerCase().indexOf("_amt") > -1 || field.ID.toLowerCase().indexOf("_amount") > -1)) {
            strDataFormat = "{0:C}";
        }
        else if (dataType && dataType.toLowerCase().contains("ssn")) {
            strDataFormat = "{0:000-##-####}";
        }
        else if (field.ID.toLowerCase().contains("phone") || field.ID.toLowerCase().contains("fax")) {
            strDataFormat = "{0:(###)###-####}";
        }
        scp.$parent.$apply(function () {
            scp.$parent.selectedFieldBookMark.dictAttributes.sfwEntityField = value;
            if (dataType) {
                scp.$parent.selectedFieldBookMark.dictAttributes.sfwDataType = dataType.toLowerCase();
            }
            if (strDataFormat) {
                scp.$parent.selectedFieldBookMark.dictAttributes.sfwDataFormat = strDataFormat;
            }
        });
    }
}
//#endregion

//#region Drop for EntityField for Table

function onEntityFieldDropForTable(e) {
    var scp = angular.element($(e.target)).scope();
    if (scp && scp.objCorrespondenceDetails.SelecetdField) {
        var field = scp.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Collection') {
            var value = scp.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Collection') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$apply(function () {
                scp.selectedTableBookMark.dictAttributes.sfwEntityField = value;
            });
        }
        else {
            alert('Only attribute of type Collection/List can be dropped.');
        }
    }
    lstEntityTreeFieldData = null;
}
//#endregion

//#region drop for Field in Table Fields

function onEntityFieldDropForTableFields(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp && scp.objCorrespondenceDetails.SelecetdField) {
        var field = scp.objCorrespondenceDetails.SelecetdField;
        var strDataFormat = "";
        if (scp.selectedTableBookMark.dictAttributes.sfwEntityField) {
            var value = scp.objCorrespondenceDetails.SelecetdField.ID;
            var DisplayedEntity = getDisplayedEntity(scp.objCorrespondenceDetails.LstDisplayedEntities);
            var itempath = field.ID;
            if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                itempath = DisplayedEntity.strDisplayName + "." + field.ID;
            }
            var entityFullPath = itempath; //GetItemPathForEntityObject(field);
            if (field && entityFullPath && entityFullPath.contains(scp.selectedTableBookMark.dictAttributes.sfwEntityField)) {

                if (field.DataType && field.DataType.toLowerCase() == "datetime") {
                    strDataFormat = "{0:d}";
                }
                else if ((field.DataType && field.DataType.toLowerCase() == "decimal") && (field.Value && (field.Value.toLowerCase().indexOf("_amt") > -1 || field.Value.toLowerCase().indexOf("_amount") > -1))) {
                    strDataFormat = "{0:C}";
                }
                else if (field.Value && field.Value.toLowerCase().contains("ssn")) {
                    strDataFormat = "{0:000-##-####}";
                }
                else if (field.Value && (field.Value.toLowerCase().contains("phone") || field.Value.toLowerCase().contains("fax"))) {
                    strDataFormat = "{0:(###)###-####}";
                }

                if (strDataFormat != "") {
                    strDataFormat = strDataFormat;
                }
                scp.$apply(function () {
                    scp.objTbl.dictAttributes.sfwDataField = entityFullPath.substring(scp.selectedTableBookMark.dictAttributes.sfwEntityField.length + 1, entityFullPath.length);
                    scp.objTbl.dictAttributes.sfwDataFormat = strDataFormat;


                });

            }
            else {
                alert('The Field Name should belong to the collection object associated with the Table.');
            }
        }
    }
    lstEntityTreeFieldData = null;
}
//#endregion


//#region drop for Field in Template Object Fields
function onEntityFieldDropForTemplateObjectBased(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Object') {
            var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Object') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$parent.$apply(function () {
                scp.$parent.selectedTemplateBookMark.dictAttributes.sfwEntityField = value;


            });

            scp.$parent.getTemplateList();
        }
        else {
            alert("Field Type has to be 'Object'");
        }
    }
}
//#endregion



//#region  drop for Field in Template Collection Fields
function onEntityFieldDropForTemplateForCollection(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Collection') {
            var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Collection') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$parent.$apply(function () {
                scp.$parent.selectedTemplateBookMark.dictAttributes.sfwEntityField = value;

            });
            scp.$parent.getTemplateList();
        }
        else {
            alert("Field Type has to be 'Collection' or 'List'");
        }
    }
}
//#endregion

//#region on Add Item 

GetMatchTemplate = function (objChildTemplate, objField) {
    var matchTemplates = [];
    var Parent = objField.ParentVM;
    while (Parent && Parent.Name !== "sfwCorrespondenceTestField" && Parent.Name !== "sfwCorrespondenceTest") {
        Parent = Parent.ParentVM;
    }

    if (Parent) {
        matchTemplates = objChildTemplate.Elements.filter(function (x) { return x.dictAttributes.ID == Parent.dictAttributes.ID });
    }

    return matchTemplates;
}
OpenPopup = function (objField, currentScope, mainmodel) {
    //if BookmarkType is Table then new BaseDialog is created
    if (objField.dictAttributes.sfwBookmarkType == "Table" || objField.dictAttributes.sfwBookmarkType == "sfwRepeater") {
        var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
        if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
            var matchedTemplate = GetMatchTemplate(objChildTemplateBookmarks[0], objField);
            if (!matchedTemplate || (matchedTemplate && matchedTemplate.length == 0)) {
                var parentVM = objField.ParentVM;
                while (parentVM != null) {
                    if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                        var templateBookamrks = getDescendents(objChildTemplateBookmarks);
                        //if template id is there in ChildTemplateBookmarks then only break;
                        if (templateBookamrks && templateBookamrks.length > 0) {
                            var templateBookamrk = templateBookamrks[0].filter(function (x) { return x.dictAttributes.ID == ParentVM.dictAttributes.ID });
                            if (templateBookamrk && templateBookamrk.length > 0)
                                break;
                        }
                    }
                    parentVM = parentVM.ParentVM;
                }

                if (parentVM) {
                    var lstAllBookMarks = [];
                    var fileModel = null;
                    var tempDescendent = getDescendents(mainmodel);
                    if (tempDescendent && tempDescendent.length > 0) {
                        var collectionTemplate = tempDescendent.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });//x => x[ApplicationConstants.XMLFacade.ID] == parentVM[ApplicationConstants.XMLFacade.ID]);
                        if (collectionTemplate && collectionTemplate.length > 0) {
                            if (collectionTemplate[0].dictAttributes.sfwTemplateName) {
                                $.connection.hubCorrespondence.server.getAllBookMarks(collectionTemplate[0].dictAttributes.sfwTemplateName).done(function (data) {
                                    if (data.length == 2) {
                                        lstAllBookMarks = data[0];
                                        fileModel = data[1];
                                        if (fileModel) {
                                            if (objField.ParentVM.dictAttributes.ID) {
                                                var objDescendent = getDescendents(fileModel);
                                                if (objDescendent && objDescendent.length > 0) {
                                                    var childTemplateModel = objDescendent.filter(function (x) { return x.dictAttributes.ID == objField.ParentVM.dictAttributes.ID });//.Descendants().FirstOrDefault(x => x[ApplicationConstants.XMLFacade.ID] == this.ParentVM[ApplicationConstants.XMLFacade.ID]);

                                                    if (childTemplateModel && childTemplateModel.length > 0) {

                                                        OpenChildTemplateWindow(lstAllBookMarks, childTemplateModel[0], objField, currentScope, mainmodel);
                                                    }
                                                }
                                            }
                                            else {

                                                var parent = objField.ParentVM;
                                                var collectionTemplateID = undefined;
                                                var templateBookmark = null;
                                                while (parentVM != null) {
                                                    if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                                                        var templateBookamrks = getDescendents(objChildTemplateBookmarks);
                                                        //if template id is there in ChildTemplateBookmarks then only break;
                                                        if (templateBookamrks && templateBookamrks.length > 0) {
                                                            var templateBookamrk = templateBookamrks[0].filter(function (x) { return x.dictAttributes.ID == ParentVM.dictAttributes.ID });
                                                            if (templateBookamrk && templateBookamrk.length > 0)
                                                                break;
                                                        }

                                                    }
                                                    parentVM = parentVM.ParentVM;
                                                }

                                                if (fileModel) {
                                                    OpenChildTemplateWindow(lstAllBookMarks, fileModel, objField, currentScope, mainmodel);
                                                }
                                            }


                                        }
                                    }
                                });
                            }

                        }
                        else {
                            GetAllBookmarksAndFileModelForTable(objField, mainmodel.dictAttributes.ID, currentScope, mainmodel);

                        }
                    }
                }
                else {

                    GetAllBookmarksAndFileModelForTable(objField, mainmodel.dictAttributes.ID, currentScope, mainmodel);
                }
            }
            else {
                // if child template is Object type which has table then below code is executed
                if (matchedTemplate) {

                    GetAllBookmarksAndFileModelForTable(objField, matchedTemplate[0].dictAttributes.sfwTemplateName, currentScope, mainmodel);

                }
            }

        }

    }

    // BookmarkType is Template then new BaseDialog is created
    else if (objField.dictAttributes.sfwBookmarkType == "Template") {
        var lstAllBookMarks = [];
        var fileModel = null;
        var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
        if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
            var objtemplate = objChildTemplateBookmarks[0].Elements.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });
            if (objtemplate && objtemplate.length > 0 && objtemplate[0].dictAttributes.sfwTemplateName) {
                $.connection.hubCorrespondence.server.getAllBookMarks(objtemplate[0].dictAttributes.sfwTemplateName).done(function (data) {
                    if (data.length == 2) {
                        lstAllBookMarks = data[0];
                        fileModel = data[1];
                        if (fileModel) {
                            OpenChildTemplateWindow(lstAllBookMarks, fileModel, objField, currentScope, mainmodel);
                        }
                        else {
                            // if child template has child template again then following code is executed.
                            var parentVM = objField.ParentVM;
                            while (parentVM) {
                                if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                                    //if template id is there in ChildTemplateBookmarks then only break;
                                    var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
                                    if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
                                        var templateBookamrks = getDescendents(objChildTemplateBookmarks[0]);
                                        if (templateBookamrks && templateBookamrks.length > 0) {
                                            var templateBookamrk = templateBookamrks.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });
                                            if (templateBookamrk && templateBookamrk.length > 0)
                                                break;
                                        }
                                    }

                                }
                                parentVM = parentVM.ParentVM;
                            }

                            if (parentVM) {
                                var lstAllBookMarks = [];
                                var fileModel = null;
                                var tempDescendent = getDescendents(mainmodel);
                                if (tempDescendent && tempDescendent.length > 0) {
                                    var nestedTemplate = tempDescendent.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });// (x => x[ApplicationConstants.XMLFacade.ID] == parentVM[ApplicationConstants.XMLFacade.ID]);
                                    if (nestedTemplate && nestedTemplate.length > 0 && nestedTemplate[0].dictAttributes.sfwTemplateName) {
                                        $.connection.hubCorrespondence.server.getAllBookMarks(nestedTemplate[0].dictAttributes.sfwTemplateName).done(function (data) {

                                            if (data.length == 2) {
                                                lstAllBookMarks = data[0];
                                                fileModel = data[1];

                                                var testmodel = getDescendents(fileModel);
                                                if (testmodel && testmodel.length > 0) {
                                                    var childTemplateElementInModel = testmodel.filter(function (x) { return x.dictAttributes.ID == objtemplate[0].dictAttributes.ID });  //x => x[ApplicationConstants.XMLFacade.ID] == templateControl[ApplicationConstants.XMLFacade.ID]);

                                                    if (childTemplateElementInModel && childTemplateElementInModel.length > 0) {
                                                        var objchildTemplateElementInModel = getDescendents(childTemplateElementInModel);
                                                        if (objchildTemplateElementInModel && objchildTemplateElementInModel.length > 0) {
                                                            var childTemplateElementInModel = objchildTemplateElementInModel.filter(function (x) { return x.dictAttributes.ID == objtemplate[0].dictAttributes.ID });//x => x[ApplicationConstants.XMLFacade.ID] == templateControl[ApplicationConstants.XMLFacade.ID]);

                                                            if (childTemplateElementInModel && childTemplateElementInModel.length > 0) {
                                                                if (childTemplateElementInModel[0].dictAttributes.sfwChildTemplateType == "Entity" || childTemplateElementInModel[0].dictAttributes == "Collection") {
                                                                    var lstAllBookMarksForChild = [];
                                                                    var fileModelForChild = null;
                                                                    if (childTemplateElementInModel[0].dictAttributes.sfwTemplateName) {
                                                                        $.connection.hubCorrespondence.server.getAllBookMarks(childTemplateElementInModel[0].dictAttributes.sfwTemplateName).done(function (data) {
                                                                            var lstAllBookMarksForChild = data[0];
                                                                            var fileModelForChild = data[1];
                                                                            if (fileModelForChild) {

                                                                                OpenChildTemplateWindow(lstAllBookMarksForChild, fileModelForChild, childTemplateElementInModel[0], currentScope, mainmodel);
                                                                            }
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        });
                                    }
                                }

                            }
                        }
                    }
                });
            }


        };
    }
}
//#endregion

//#region Get Bookmarks and Model for Collection
GetAllBookmarksAndFileModelForTable = function (objField, templateName, currentScope, mainmodel) {
    var newScope = currentScope.$new(true);
    newScope.lstData = [];
    var lstAllBookMarks = [];
    var fileModel = null;
    newScope.testfieldmodel = objField;
    newScope.correspondencemainmodel = mainmodel;
    if (templateName) {
        $.connection.hubCorrespondence.server.getAllBookMarks(templateName).done(function (data) {

            if (data.length == 2) {
                lstAllBookMarks = data[0];
                fileModel = data[1];
            }

            if (objField.dictAttributes.sfwBookmarkType && objField.dictAttributes.sfwBookmarkType.indexOf("Template") > -1) {
                newScope.$evalAsync(function () {

                    newScope.lstData = CreateColumnsForChildTemplate(lstAllBookMarks);
                });
            }
            else {
                //for collection
                //if table belongs to Child Template
                if (fileModel) {
                    var objDescendent = getDescendents(fileModel);
                    if (objDescendent && objDescendent.length > 0) {
                        var table = objDescendent.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });// fileModel.Descendants().FirstOrDefault(x => x[ApplicationConstants.XMLFacade.ID] == FieldVM[ApplicationConstants.XMLFacade.ID]);
                        if (table && table.length > 0) {
                            angular.forEach(table[0].Elements, function (item) {
                                if (table[0].Name == "sfwRepeater") {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["ID"]);
                                    });
                                }
                                else {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["sfwColumnHeader"]);
                                    });
                                }
                            });

                        }
                    }

                }
                else {

                    var objTableBookMarks = mainmodel.Elements.filter(function (x) { x.Name == "sfwTableBookMarks" });
                    if (objTableBookMarks && objTableBookMarks.length > 0) {
                        var table = objTableBookMarks[0].Elements.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });//=> x["ID"] == FieldVM["ID"]);
                        if (table && table.length > 0) {
                            angular.forEach(table[0].Elements, function (item) {
                                if (table[0].Name == "sfwRepeater") {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["ID"]);
                                    });
                                }
                                else {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["sfwColumnHeader"]);
                                    });
                                }
                            });
                        }
                    }

                }
            }
        });

        newScope.openAddItemToListForCollection = currentScope.$root.showDialog(newScope, "Add Items", "Correspondence/views/AddItemToListForCollection.html");

        newScope.AddCollectionRecord = function () {
            var count = 0;
            var tempModel = undefined;
            var fields = {
                Name: "sfwCorrespondenceTestFields", Value: '', dictAttributes: {}, Elements: []
            };
            if (newScope.model) {
                count = newScope.model.Elements.length;
                tempModel = newScope.model;
            }
            else if (newScope.testfieldmodel) {
                count = newScope.testfieldmodel.Elements.length;
                tempModel = newScope.testfieldmodel;
            }
            fields.dictAttributes.indexNumber = count;
            fields.ParentVM = newScope.testfieldmodel;

            currentScope.$root.PushItem(fields, newScope.testfieldmodel.Elements);

            if (newScope.lstData && newScope.lstData.length > 0) {
                angular.forEach(newScope.lstData, function (item) {

                    var field = {
                        Name: "sfwCorrespondenceTestField", Value: '', dictAttributes: {}, Elements: []
                    };
                    field.ParentVM = fields;
                    field.dictAttributes.ID = item;
                    if (lstAllBookMarks && lstAllBookMarks.length > 0) {
                        if (tempModel && tempModel.dictAttributes.sfwBookmarkType != "Table") {
                            var corBasicBkmrk = lstAllBookMarks.filter(function (x) { return x.Name == item });
                            if (corBasicBkmrk && corBasicBkmrk.length > 0) {
                                if (corBasicBkmrk.Type == BookmarkType.QueryUser ||
                                    corBasicBkmrk.Type == BookmarkType.QueryConditionalBlock) {

                                }
                                SetTemplateBookmarkTypeInModel(corBasicBkmrk, field, tempModel);
                            }
                        }
                    }

                    currentScope.$root.PushItem(field, fields.Elements);
                });
                newScope.selectedCollectionRow = newScope.testfieldmodel.Elements[newScope.testfieldmodel.Elements.length - 1];
            }
        }

        newScope.SelectedCollectionField = function (obj) {
            newScope.selectedCollectionRow = obj;
        }

        newScope.DeleteCollectionRecord = function () {
            var index = newScope.testfieldmodel.Elements.indexOf(newScope.selectedCollectionRow);
            if (index > -1) {
                currentScope.$root.DeleteItem(newScope.selectedCollectionRow, newScope.testfieldmodel.Elements, "");
                if (index < newScope.testfieldmodel.Elements.length) {
                    newScope.SelectedCollectionField(newScope.testfieldmodel.Elements[index]);
                }
                else if (newScope.testfieldmodel.Elements.length > 0) {
                    newScope.SelectedCollectionField(newScope.testfieldmodel.Elements[index - 1]);
                }
                else {
                    newScope.SelectedCollectionField(newScope.testfieldmodel);
                }

            }
            if (newScope.testfieldmodel.Elements.length == 0) {
                newScope.selectedCollectionRow = undefined;
            }
        }

    }
}


SetTemplateBookmarkTypeInModel = function (item, field, fileModel) {
    var objDescendent = getDescendents(fileModel);
    if (objDescendent && objDescendent.length > 0) {
        var fieldModel = objDescendent.filter(function (x) { return x.dictAttributes.ID == field.dictAttributes.ID });//(x => x[ApplicationConstants.XMLFacade.ID] == field[ApplicationConstants.XMLFacade.ID]);
        if (fieldModel && fieldModel.length > 0) {
            field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwDataType;
            switch (item[0].Type) {
                case BookmarkType.ConditionalBlock:
                case BookmarkType.ElseBlock:
                case BookmarkType.EndBlock:
                case BookmarkType.Simple:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
                case BookmarkType.NormalTable:
                case BookmarkType.StaticTable:
                    field.dictAttributes.sfwBookmarkType = "Table";
                    break;
                case BookmarkType.Template:
                    field.dictAttributes.sfwBookmarkType = "Template";
                    field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwChildTemplateType;
                    break;
                case BookmarkType.RuleConditionalBlock:
                    field.dictAttributes.sfwBookmarkType = "Rule";

                    break;
                default:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
            }
        }
    }
}
CreateColumnsForChildTemplate = function (bookmarks) {
    var lstData = [];
    if (bookmarks && bookmarks.length > 0) {
        //find all bookmarks and add it as column header
        angular.forEach(bookmarks, function (item) {
            lstData.push(item.Name);
        });

    }
    return lstData;
}

//#endregion

//#region open child window for Template Bookmark(for Entity & Collection Datatype)
OpenChildTemplateWindow = function (allBookmarks, fileModel, objField, currentScope, mainmodel) {
    //if DataType is Object
    if (objField.dictAttributes.sfwDataType == "Entity") {

        GetAllBookmarksAndFileModelForEntity(objField, fileModel.dictAttributes.ID, currentScope, mainmodel);

    }
    //if DataType is Collection
    else if (objField.dictAttributes.sfwDataType == "Collection") {
        GetAllBookmarksAndFileModelForTable(objField, fileModel.dictAttributes.ID, currentScope, mainmodel);

    }
    //}

}
//#endregion

//#region Get All Bookmarks And FileModel For Entity DataType
GetAllBookmarksAndFileModelForEntity = function (objField, templateName, currentScope, mainmodel) {

    var newScope = currentScope.$new();
    newScope.lstData = [];
    var lstAllBookMarks = [];
    var fileModel = null;
    if (templateName) {
        $.connection.hubCorrespondence.server.getAllBookMarks(templateName).done(function (data) {

            if (data.length == 2) {
                lstAllBookMarks = data[0];
                fileModel = data[1];
            }

            if (objField.Elements.length > 0) {

                //removing old bookmarks which is not present in new bookmarks list
                for (var j = objField.Elements.length - 1; j >= 0; j--) {
                    var bookmarkFound = lstAllBookMarks.filter(function (x) { return x.Name == objField.Elements[j].dictAttributes.ID });
                    if (!bookmarkFound || (bookmarkFound && bookmarkFound.length == 0)) {
                        objField.Elements.splice(j, 1);

                    };
                }


                angular.forEach(lstAllBookMarks, function (item) {
                    var bookmarkFound = objField.Elements.filter(function (x) { return x.dictAttributes.ID == item.Name }); //x => x[ApplicationConstants.XMLFacade.ID] == item.Name);
                    if (!bookmarkFound || (bookmarkFound && bookmarkFound.length == 0)) {
                        AddTemplateBookmarkInModel(item, fileModel, objField, currentScope);
                    }

                });

            }
            else {
                angular.forEach(lstAllBookMarks, function (item) {
                    AddTemplateBookmarkInModel(item, fileModel, objField, currentScope);
                });
            }
            newScope.testfieldmodel = objField;
            newScope.correspondencemainmodel = mainmodel;
            newScope.openAddItemToListForEntity = currentScope.$root.showDialog(newScope, "Add Items", "Correspondence/views/AddItemToListForEntity.html");
            newScope.OnItemClick = function (obj) {
                newScope.selectedItem = obj;
            }

            newScope.$evalAsync(function () {
                newScope.Elements = objField.Elements;
            });
            newScope.OpenPopup = function (objField) {
                OpenPopup(objField, newScope, fileModel);
            }

        });

    }

}

AddTemplateBookmarkInModel = function (item, fileModel, objField, currentScope) {
    if (item.Type == BookmarkType.QueryUser ||
        item.Type == BookmarkType.QueryConditionalBlock) {
        return;
    }

    var field = {
        Name: "sfwCorrespondenceTestField", Value: '', dictAttributes: {}, Elements: []
    };


    field.dictAttributes.ID = item.Name;
    var objDescendent = getDescendents(fileModel);
    if (objDescendent && objDescendent.length > 0) {

        var fieldModel = objDescendent.filter(function (x) { return x.dictAttributes.ID && x.dictAttributes.ID.toLowerCase() == item.Name.toLowerCase() });
        if (fieldModel && fieldModel.length > 0) {
            field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwDataType;
            switch (item.Type) {
                case BookmarkType.ConditionalBlock:
                case BookmarkType.ElseBlock:
                case BookmarkType.EndBlock:
                case BookmarkType.Simple:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
                case BookmarkType.NormalTable:
                case BookmarkType.StaticTable:
                    field.dictAttributes.sfwBookmarkType = "Table";
                    break;
                case BookmarkType.Template:
                    field.dictAttributes.sfwBookmarkType = "Template";
                    field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwChildTemplateType;
                    break;
                case BookmarkType.RuleConditionalBlock:
                    field.dictAttributes.sfwBookmarkType = "Rule";

                    break;
                default:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
            }
        }
    }
    currentScope.$root.PushItem(field, objField.Elements);
}


//#endregion


//#region enum Bookmarks Types
var BookmarkType =
    {
        Undefined: 0,
        Simple: 1,
        Constant: 2,
        ConditionalBlock: 3,
        QueryConditionalBlock: 4,
        EndBlock: 5,
        Barcode: 6,
        QueryUser: 7,
        NormalTable: 8,
        StaticTable: 9,
        InsertFile: 10,
        ElseBlock: 11,
        Template: 12,
        RuleConditionalBlock: 13

    };

//#endregion
app.directive('correspondencetabletemplate', ['$compile', "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", "$NavigateToFileService", "$rootScope", "$GetEntityFieldObjectService", function ($compile, $EntityIntellisenseFactory, $ValidationService, CONST, $NavigateToFileService, $rootScope, $GetEntityFieldObjectService) {
    var getTemplate = function (content1) {

        var template = '';
        template += '<div class="panel" ng-click="loadEntityTree(true, $event)">',
            template += '<div class="panel-heading"  ng-click="items.IsExpanded=!items.IsExpanded">',
            template += '<span ng-bind="items.dictAttributes.ID"></span>',
            template += '<div class="correspondence-panel-arrow">',
            template += '<i ng-class="!items.IsExpanded?\'fa fa-chevron-down\':\'fa fa-chevron-up\'">',
            template += '</i>',
            template += '</div>',
            template += '</div>',
            template += '<div class="panel-body" ng-if="items.IsExpanded">',
            template += '<div class="form-group no-bottom-margin">',
            template += '<label class="control-label corr-tables-caption" ng-if="!items.dictAttributes.sfwEntityField.trim()">Entity Field</label><label class="control-label corr-tables-caption" ng-if="items.dictAttributes.sfwEntityField.trim()"><a ng-click="NavigateToEntityField(items.dictAttributes.sfwEntityField,objcorrespondence.dictAttributes.sfwEntity)">Entity Field</a></label>',
            template += '<entityfieldelementintellisense onchangecallback="clearEntityFieldforTable()" modebinding="items.dictAttributes.sfwEntityField"  class="corr-tables-input" model="items" onblurcallback="setEntityIDForSelectedTable(items)" isshowonetoone=true isshowonetomany=true entityid="objcorrespondence.dictAttributes.sfwEntity" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="items.errors.sfwEntityField" validate="true"/>',
            template += '</div>',
            template += '<div class="contact-table-manage">',
            template += '<table class="sws-table sws-bordered-table auto-width" ng-click="loadEntityTree(false, $event)">',
            template += '<thead>',
            template += '<tr>',
            template += '<th></th>',
            template += '<th ng-repeat="objTbl in items.Elements"  ng-class="{\'selected\':objTbl==selectedSfwColumn, \'bckgGrey\' : objTbl.isAdvanceSearched, \'bckgGreen\' : (objTbl.isAdvanceSearched && objTbl==selectedSfwColumn)}" >{{objTbl.dictAttributes.sfwColumnHeader}}</th>',
            template += '</tr>',
            template += '</thead>',
            template += '<tbody>',
            template += '<tr>',
            template += '<td class="sws-bordered-table-left-td">Field Name</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            template += '<entityfieldelementintellisense setcolumndatatype="true" candrop="true" modebinding="objTbl.dictAttributes.sfwEntityField" model="objTbl" entityid="SelectedTableEntityID" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="objTbl.errors.sfwEntityField" validate="true" isempty="true"/>',
            template += '</td>',
            template += '</tr>',
            template += '<tr >',
            template += '<td class="sws-bordered-table-left-td">Data Format</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            //  template += '<select class="form-control form-filter input-sm" ng-options="x for x in dataformats" title="{{objTbl.dictAttributes.sfwDataFormat}}" ng-model="objTbl.dictAttributes.sfwDataFormat"></select>',
            template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            template += '</td>',
            template += '</tr>',
            template += '<tr>',
            template += '<td class="sws-bordered-table-left-td">Data Type</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwColumnDataType}}" ng-model="objTbl.dictAttributes.sfwColumnDataType"></select>',
            template += '</td>',
            template += '</tr>',
            template += '</tbody>',
            template += '</table>';
        template += '</div>',
            template += '</div>',
            template += '</div>';
        return template;
    };

    return {
        restrict: "E",
        replace: true,
        scope: {
            items: '=',
            objcorrespondence: '=',
            entitytreename: '=entityTreeName',
            changeCallback: '&',
            selectedSfwColumn: '='
        },
        link: function (scope, element, attrs) {


            scope.datatypes = CONST.CORRESPONDENCEDATATYPES;
            scope.dataformats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];

            scope.loadEntityTree = function (isParent, e) {
                if (!isParent) {
                    if (scope.items.dictAttributes.sfwEntityField && scope.objcorrespondence.dictAttributes.sfwEntity) {
                        var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.objcorrespondence.dictAttributes.sfwEntity, scope.items.dictAttributes.sfwEntityField);
                        if (objAttribute && (objAttribute.DataType == 'Collection' || objAttribute.DataType == 'List')) {
                            if (scope.changeCallback) {
                                scope.changeCallback({ entity: objAttribute.Entity });
                            }
                        }
                    }
                }
                else {
                    if (scope.changeCallback) {
                        scope.changeCallback();
                    }
                }
                if (e) {
                    e.stopPropagation();
                }
            };

            scope.clearEntityFieldforTable = function () {
                scope.items.Elements.forEach(function (objTL) {
                    objTL.dictAttributes.sfwEntityField = "";
                    $ValidationService.checkValidListValue([], objTL, objTL.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                    //  $ValidationService.checkValidListValueForMultilevel([], objTL, objTL.dictAttributes.sfwEntityField, scope.items.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                });
            };

            scope.setEntityIDForSelectedTable = function (selectedTableBookMark) {
                scope.SelectedTableEntityID = undefined;
                entityField = selectedTableBookMark.dictAttributes.sfwEntityField;
                var EntityID = scope.objcorrespondence.dictAttributes.sfwEntity;
                if (entityField && EntityID) {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(EntityID, entityField);
                    if (objField) {
                        scope.SelectedTableEntityID = objField.Entity;
                    }
                }
            };

            if (scope.items) {
                element.html(getTemplate(scope.items));
                $compile(element.contents())(scope);
                scope.setEntityIDForSelectedTable(scope.items);
            }
            scope.NavigateToEntityField = function (astrEntityField, astrEntityName) {
                if (astrEntityField && astrEntityName) {
                    var arrText = astrEntityField.split('.');
                    var data = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = "";

                    if (astrEntityName) {
                        parententityName = astrEntityName;
                    }
                    while (parententityName) {
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            var attributes = entity[0].Attributes;
                            data = data.concat(attributes);
                            parententityName = entity[0].ParentId;
                        }
                        else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length - 1; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    var parententityName = item[0].Entity;
                                    if (index < arrText.length - 2) {
                                        data = [];
                                    }
                                    while (parententityName) {
                                        var entity = entities.filter(function (x) {
                                            return x.ID == parententityName;
                                        });
                                        if (entity.length > 0) {
                                            var attributes = entity[0].Attributes;
                                            data = data.concat(attributes);
                                            parententityName = entity[0].ParentId;
                                        }
                                        else {
                                            parententityName = "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (data && data.length > 0) {
                        var tempField = data.filter(function (x) { return x.ID == arrText[arrText.length - 1]; });
                        if (tempField && tempField.length > 0) {
                            var attributeID = arrText[arrText.length - 1];
                            if (tempField[0].Type && tempField[0].Type == "Description" && arrText[arrText.length - 1] && arrText[arrText.length - 1].toLowerCase().endsWith("description")) {
                                attributeID = arrText[arrText.length - 1].substr(0, (arrText[arrText.length - 1].length - 11)) + "Value";
                            }
                            if (arrText.length == 1) {
                                var EntityName = "";
                                if (astrEntityName) {
                                    EntityName = astrEntityName;
                                }
                                if (EntityName) {
                                    $rootScope.IsLoading = true;
                                    $NavigateToFileService.NavigateToFile(EntityName, "attributes", attributeID);
                                }
                            }
                            else if (arrText.length > 1) {
                                var item = data.filter(function (x) { return x.ID == arrText[arrText.length - 2]; });
                                if (item && item != "" && item[0].Entity) {
                                    $rootScope.IsLoading = true;
                                    $NavigateToFileService.NavigateToFile(item[0].Entity, "attributes", attributeID);
                                }
                            }
                        }

                    }
                }
            };
        }
    };
}]);

app.directive('correspondencerepeatercontroltemplate', ['$compile', "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", "$NavigateToFileService", "$rootScope", "$GetEntityFieldObjectService", function ($compile, $EntityIntellisenseFactory, $ValidationService, CONST, $NavigateToFileService, $rootScope, $GetEntityFieldObjectService) {
    var getTemplate = function (content1) {

        var template = '';
        template += '<div class="panel" ng-click="loadEntityTree(true, $event)">',
            template += '<div class="panel-heading"  ng-click="items.IsExpanded=!items.IsExpanded">',
            template += '<span ng-bind="items.dictAttributes.ID"></span>',
            template += '<div class="correspondence-panel-arrow">',
            template += '<i ng-class="!items.IsExpanded?\'fa fa-chevron-down\':\'fa fa-chevron-up\'">',
            template += '</i>',
            template += '</div>',
            template += '</div>',
            template += '<div class="panel-body" ng-if="items.IsExpanded">',
            template += '<Grid>',
            template += '    <Grid.ColumnDefinitions>',
            template += '        <ColumnDefinition Width="auto" />',
            template += '        <ColumnDefinition Width="*" />',
            template += '        <ColumnDefinition Width="auto" />',
            template += '        <ColumnDefinition Width="*" />',
            template += '  </Grid.ColumnDefinitions>',
            template += '    <Grid.RowDefinitions>',
            template += '        <RowDefinition Height="*" />',
            template += '</Grid.RowDefinitions>',
            template += '<label Grid.Column="0" class="control-label " style="vertical-align:10px" ng-if="!items.dictAttributes.sfwEntityField.trim()">Entity Field</label><label class="control-label corr-tables-caption" ng-if="items.dictAttributes.sfwEntityField.trim()"><a ng-click="NavigateToEntityField(items.dictAttributes.sfwEntityField,objcorrespondence.dictAttributes.sfwEntity)">Entity Field</a></label>',
            template += '<entityfieldelementintellisense Grid.Column="1" onchangecallback="clearEntityFieldforTable()" modebinding="items.dictAttributes.sfwEntityField" class="corr-tables-input" model="items" onblurcallback="setEntityIDForSelectedTable(items)" isshowonetoone=true isshowonetomany=true entityid="objcorrespondence.dictAttributes.sfwEntity" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="items.errors.sfwEntityField" validate="true" />',

            template += '<label Grid.Column="2" style="vertical-align:10px" class="control-label marg-left-10">Show First Line Header</label>',
            template += '<input class="marg-left-10" type="checkbox" style="vertical-align:10px" ng-model="items.dictAttributes.sfwFirstLineHeader" ng-checked="items.dictAttributes.sfwFirstLineHeader" undoredodirective model="items" ng-true-value="\'True\'" ng-false-value="\'False\'" />',

            template += '</Grid>',
            template += '<div class="contact-table-manage">',
            template += '<table class="sws-table sws-bordered-table auto-width" ng-click="loadEntityTree(false, $event)">',
            template += '<thead>',
            template += '<tr>',
            template += '<th>ID</th>',
            template += '<th>Data Type</th>',
            template += '<th>Data Format</th>',
            template += '<th>Entity Field</th>',
            template += '</tr>',
            template += '</thead>',
            template += '<tbody>',
            template += '<tr ng-repeat="objTbl in items.Elements">',
            template += '<td>',
            template += '<input type="text" class="form-control input-sm" ng-model="objTbl.dictAttributes.ID"  undoredodirective model="objTbl" />',
            template += '</td>',
            template += '<td>',
            template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwDataType}}" ng-model="objTbl.dictAttributes.sfwDataType"></select>',
            template += '</td>',
            template += '<td>',
            template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            template += '</td>',
            template += '<td>',
            template += '<entityfieldelementintellisense setcolumndatatype="true" candrop="true" modebinding="objTbl.dictAttributes.sfwEntityField" model="objTbl" entityid="SelectedTableEntityID" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="objTbl.errors.sfwEntityField" validate="true" isempty="true"/>',
            template += '</td>',
            template += '</tr>',
            //template += '<tr >',
            //template += '<td class="sws-bordered-table-left-td">Data Format</td>',
            //template += '<td ng-repeat="objTbl in items.Elements">',
            ////  template += '<select class="form-control form-filter input-sm" ng-options="x for x in dataformats" title="{{objTbl.dictAttributes.sfwDataFormat}}" ng-model="objTbl.dictAttributes.sfwDataFormat"></select>',
            //template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            //template += '</td>',
            //template += '</tr>',
            //template += '<tr>',
            //template += '<td class="sws-bordered-table-left-td">Data Type</td>',
            //template += '<td ng-repeat="objTbl in items.Elements">',
            //template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwColumnDataType}}" ng-model="objTbl.dictAttributes.sfwColumnDataType"></select>',
            //template += '</td>',
            //template += '</tr>',
            template += '</tbody>',
            template += '</table>';
        template += '</div>',
            template += '</div>',
            template += '</div>';
        return template;
    };

    return {
        restrict: "E",
        replace: true,
        scope: {
            items: '=',
            objcorrespondence: '=',
            entitytreename: '=entityTreeName',
            changeCallback: '&',
            selectedSfwColumn: '='
        },
        link: function (scope, element, attrs) {


            scope.datatypes = CONST.CORRESPONDENCEDATATYPES;
            scope.dataformats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];

            scope.loadEntityTree = function (isParent, e) {
                if (!isParent) {
                    if (scope.items.dictAttributes.sfwEntityField && scope.objcorrespondence.dictAttributes.sfwEntity) {
                        var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.objcorrespondence.dictAttributes.sfwEntity, scope.items.dictAttributes.sfwEntityField);
                        if (objAttribute && (objAttribute.DataType == 'Collection' || objAttribute.DataType == 'List')) {
                            if (scope.changeCallback) {
                                scope.changeCallback({ entity: objAttribute.Entity });
                            }
                        }
                    }
                }
                else {
                    if (scope.changeCallback) {
                        scope.changeCallback();
                    }
                }
                if (e) {
                    e.stopPropagation();
                }
            };

            scope.clearEntityFieldforTable = function () {
                scope.items.Elements.forEach(function (objTL) {
                    objTL.dictAttributes.sfwEntityField = "";
                    $ValidationService.checkValidListValue([], objTL, objTL.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                    //  $ValidationService.checkValidListValueForMultilevel([], objTL, objTL.dictAttributes.sfwEntityField, scope.items.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                });
            };

            scope.setEntityIDForSelectedTable = function (selectedTableBookMark) {
                scope.SelectedTableEntityID = undefined;
                entityField = selectedTableBookMark.dictAttributes.sfwEntityField;
                var EntityID = scope.objcorrespondence.dictAttributes.sfwEntity;
                if (entityField && EntityID) {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(EntityID, entityField);
                    if (objField) {
                        scope.SelectedTableEntityID = objField.Entity;
                    }
                }
            };

            if (scope.items) {
                element.html(getTemplate(scope.items));
                $compile(element.contents())(scope);
                scope.setEntityIDForSelectedTable(scope.items);
            }
            scope.NavigateToEntityField = function (astrEntityField, astrEntityName) {
                if (astrEntityField && astrEntityName) {
                    var arrText = astrEntityField.split('.');
                    var data = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = "";

                    if (astrEntityName) {
                        parententityName = astrEntityName;
                    }
                    while (parententityName) {
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            var attributes = entity[0].Attributes;
                            data = data.concat(attributes);
                            parententityName = entity[0].ParentId;
                        }
                        else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length - 1; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    var parententityName = item[0].Entity;
                                    if (index < arrText.length - 2) {
                                        data = [];
                                    }
                                    while (parententityName) {
                                        var entity = entities.filter(function (x) {
                                            return x.ID == parententityName;
                                        });
                                        if (entity.length > 0) {
                                            var attributes = entity[0].Attributes;
                                            data = data.concat(attributes);
                                            parententityName = entity[0].ParentId;
                                        }
                                        else {
                                            parententityName = "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (data && data.length > 0) {
                        var tempField = data.filter(function (x) { return x.ID == arrText[arrText.length - 1]; });
                        if (tempField && tempField.length > 0) {
                            var attributeID = arrText[arrText.length - 1];
                            if (tempField[0].Type && tempField[0].Type == "Description" && arrText[arrText.length - 1] && arrText[arrText.length - 1].toLowerCase().endsWith("description")) {
                                attributeID = arrText[arrText.length - 1].substr(0, (arrText[arrText.length - 1].length - 11)) + "Value";
                            }
                            if (arrText.length == 1) {
                                var EntityName = "";
                                if (astrEntityName) {
                                    EntityName = astrEntityName;
                                }
                                if (EntityName) {
                                    $rootScope.IsLoading = true;
                                    $NavigateToFileService.NavigateToFile(EntityName, "attributes", attributeID);
                                }
                            }
                            else if (arrText.length > 1) {
                                var item = data.filter(function (x) { return x.ID == arrText[arrText.length - 2]; });
                                if (item && item != "" && item[0].Entity) {
                                    $rootScope.IsLoading = true;
                                    $NavigateToFileService.NavigateToFile(item[0].Entity, "attributes", attributeID);
                                }
                            }
                        }

                    }
                }
            };
        }
    };
}]);