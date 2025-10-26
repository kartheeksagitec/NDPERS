app.controller('PageLayoutController', ["$scope", "$http", "$rootScope", "$Errors", "$DashboardFactory", function ($scope, $http, $rootScope, $Errors, $DashboardFactory) {
    $scope.isstartpagevisible = false;
    $scope.isConfigureSettingsVisible = false;

    //#region Page Header Actions

    $scope.closeFile = function (file) {
        $rootScope.CloseCurrentFiles(file);
    };

    $scope.toggleView = function (view) {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.ShowDesign) {
                if (view == "Design") {
                    if (scope.isSourceDirty) {
                        $rootScope.ClearUndoRedoListByFileName(scope.currentfile.FileName);
                    }
                    scope.ShowDesign();
                }
                else if (view == "Source") {
                    scope.ShowSource();
                }
            }
        }
    };
    $scope.selectDesignSource = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileName) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            $scope.isHideUndoredobuttons = false;
            if (scope) {
                $scope.isHideUndoredobuttons = scope.selectedDesignSource;
                return scope.selectedDesignSource;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    };

    $scope.getFileTitle = function () {
        $scope.currentEntityName = "";
        $scope.isBpmMapLocked = false;
        var title = "";
        var scope = {};
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            if ($rootScope.currentopenfile.file.FileType) {
                scope = getCurrentFileScope();
                if (scope) {
                    switch ($rootScope.currentopenfile.file.FileType) {
                        case "BPMN":
                        case "BPMTemplate":
                            if (scope.BPMNModel && scope.BPMNModel.Title && scope.BPMNModel.Title.trim().length > 0) title = scope.BPMNModel.Title;
                            if ($rootScope.currentopenfile.file.FileType == "BPMN") {
                                if (!scope.IsMapEditable) {
                                    $scope.isBpmMapLocked = true;
                                }
                            }
                            break;
                        case "Lookup":
                        case "Maintenance":
                        case "Wizard":
                        case "UserControl":
                        case "Tooltip":
                            if (scope.FormModel && scope.FormModel.dictAttributes) {
                                $scope.currentEntityName = scope.FormModel.dictAttributes.sfwEntity;
                                if (scope.FormModel.dictAttributes.sfwTitle && scope.FormModel.dictAttributes.sfwTitle.trim().length > 0) title = scope.FormModel.dictAttributes.sfwTitle;
                            }
                            break;
                        case "LogicalRule":
                        case "ExcelMatrix":
                            if (scope.objLogicalRule && scope.objLogicalRule.dictAttributes)
                                $scope.currentEntityName = scope.objLogicalRule.dictAttributes.sfwEntity;
                            break;
                        case "DecisionTable":
                            if (scope.objLogicalRule)
                                $scope.currentEntityName = scope.objLogicalRule.Entity;
                            break;
                        case "Correspondence":
                        case "PDFCorrespondence":
                            if (scope.objCorrespondenceDetails && scope.objCorrespondenceDetails.dictAttributes)
                                $scope.currentEntityName = scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
                            break;
                        case "FormLinkMaintenance":
                        case "FormLinkLookup":
                        case "FormLinkWizard":
                            if (scope.FormLinkModel && scope.FormLinkModel.dictAttributes)
                                $scope.currentEntityName = scope.FormLinkModel.dictAttributes.sfwEntity;
                            break;
                        case "InboundFile":
                        case "OutboundFile":
                            if (scope.objFile && scope.objFile.dictAttributes) {
                                if (scope.objFile.dictAttributes.sfwTitle) {
                                    title = scope.objFile.dictAttributes.sfwTitle;
                                }
                            }
                            break;
                        case "Entity":
                            if (scope.objEntity && scope.objEntity.dictAttributes.sfwParentEntity) {
                                $scope.currentEntityName = scope.objEntity.dictAttributes.sfwParentEntity;
                            }
                            break;
                        default:
                    }
                }
            }
            if (title == "" && $rootScope.currentopenfile.file.FileName) {
                title = $rootScope.currentopenfile.file.FileName;
            }
            return title;
        }
    };

    $scope.getTfsIcon = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType) {
            switch ($rootScope.currentopenfile.file.FileType) {
                case "BPMN":
                case "BPMTemplate":
                    "";
                    break;
            }
            return $rootScope.currentopenfile.file.FileName;
        }
    };

    $scope.getFileTypeIcon = function () {
    };

    $scope.isCurrentFileDirty = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                return scope.isDirty;
            }
        }
    };

    $scope.onDetailClick = function () {
        var curfileid = $rootScope.currentopenfile.file;
        var scope;
        scope = getScopeByFileName(curfileid.FileName);
        if (curfileid.FileType == "UserControl" || curfileid.FileType == "Tooltip") {
            if (scope && scope.onUserControlDetailsClick) {
                scope.onUserControlDetailsClick();
            }
        } else {
            if (scope && scope.onDetailClick) {
                scope.onDetailClick();
            }
        }
    };

    $scope.showWizardProperty = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.showWizardProperty) {
            scope.showWizardProperty();
        }
    };

    $scope.refreshConstants = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.onRefreshAllGroups) {
            scope.onRefreshAllGroups();
        }
    };

    $scope.openValidateNew = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenValidateNewDialog();
            }
        }
    };

    //#region Rule
    $scope.lstViewModel = ['Developer View', 'Analyst View'];
    $scope.ViewMode = "Developer View";
    $scope.setViewMode = function (viewmode) {
        $scope.ViewMode = viewmode;
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.setViewMode(viewmode);
        }
    };
    $scope.versionWiseDisplay = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.versionWiseDisplay();
        }
    };
    $scope.OpenRunResult = function () {
        var RuleName = $scope.getRuleName(scope);
        var scope = getScopeByFileName("MainPage");
        if (scope) {
            scope.OpenRunResultClick(false, RuleName);
        }
    };
    $scope.OpenRuleErrors = function () {
        var RuleName = $scope.getRuleName(scope);
        var scope = getScopeByFileName("MainPage");
        if (scope) {
            scope.OpenRuleErrorsClick(false, RuleName);
        }
    };
    $scope.getRuleName = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        var RuleName = "";
        if (scope) {
            if (scope.objLogicalRule.dictAttributes != undefined) {
                RuleName = scope.objLogicalRule.dictAttributes.ID;
            }
            else {
                RuleName = scope.objLogicalRule.RuleID;
            }
        }
        return RuleName;
    };
    $scope.buildRule = function (isObjectRule) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.buildRule(isObjectRule);
        }
    };
    $scope.openRuleInExpression = function (isObjectRule) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.openRuleInExpression(isObjectRule);
        }
    };
    $scope.SelectVersion = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            $scope.objEffectiveDate = scope.objEffectiveDate;
        }
    };
    $scope.UpdateRule = function (effectiveDate) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.UpdateRule(effectiveDate);
        }
    };

    $scope.buildRuleClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            $scope.CanCreateObjectBasedScenario = scope.CanCreateObjectBasedScenario;
        }
    }

    $scope.ScenariosClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            $scope.HasRelatedParameterBasedScenario = scope.HasRelatedParameterBasedScenario;
            $scope.CanCreateObjectBasedScenario = scope.CanCreateObjectBasedScenario;
            $scope.HasRelatedXLBasedScenario = scope.HasRelatedXLBasedScenario;
            $scope.HasRelatedObjectBasedScenario = scope.HasRelatedObjectBasedScenario;
        }
    };
    $scope.AddNewScenario = function (scenarioType, flag) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.AddNewScenario(scenarioType, flag);
        }
    };
    $scope.NavigateToScenario = function (scenarioType) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.NavigateToScenario(scenarioType);
        }
    };
    //#endregion


    //#region Fix all Node ids
    $scope.FixAllNodeIds = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.FixAllNodeIds();
        }
    }
    //#endregion
    //#region Scenario
    $scope.ScenarioRunAllTests = function (runType) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.RunAllTests) {
            scope.RunAllTests(runType);
        }
    };

    $scope.canCreateShowParameterScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            if ($rootScope.currentopenfile.file.FileType == 'ParameterScenario' || $rootScope.currentopenfile.file.FileType == 'ObjectScenario' || $rootScope.currentopenfile.file.FileType == 'ExcelScenario') {
                if (fileScope.IsObjectBasedScenario && fileScope.objEntityBasedScenario.SelectedStep.IsRunSelected == true) {
                    return true;
                }
            }
        }
    };

    $scope.NavigateToRule = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.NavigateToRule();
        }
    };

    $scope.RefreshScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.RefreshScenario();
        }
    };

    $scope.canShowNavigationImage = function (type) {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            if ($rootScope.currentopenfile.file.FileType == 'ParameterScenario' || $rootScope.currentopenfile.file.FileType == 'ObjectScenario' || $rootScope.currentopenfile.file.FileType == 'ExcelScenario') {
                if (fileScope.RuleType == type) {
                    return true;
                }
            }
        }
    };

    $scope.canShowAddParameterBasedScenario = function () {
        if ($rootScope.currentopenfile.file.FileType == 'ObjectScenario') {
            var fileScope = getCurrentFileScope();
            if (fileScope) {
                if (fileScope.objEntityBasedScenario && fileScope.objEntityBasedScenario.SelectedStep && fileScope.objEntityBasedScenario.SelectedStep.IsRunSelected) {
                    return true;
                }
            }
        }
        return false;
    };

    $scope.onCreateParameterScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.onCreateParameterScenario();
        }
    };
    //#endregion

    //#region Forms
    $scope.OnOpenValidationRuleDetailsClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);

        if (scope && scope.OnOpenValidationRuleDetailsClick) {
            scope.OnOpenValidationRuleDetailsClick();
        }
    };

    $scope.OnAddPanelClick = function (action) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope) {
            if (action == "Panel") {
                scope.OnAddPanelClick(-1, false);
            }
            if (action == "Wizard") {
                scope.OnAddWizardStepClick(-1, false);
            }
        }
    };

    $scope.getHightlightAttribute = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && !scope.selectedDesignSource) {
            scope.getHightlightAttribute();
        }
    };

    $scope.OnSwitchView = function (action) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope) {
            scope.OnSwitchView();
        }
    };

    $scope.isDeveloperView = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope) {
                return scope.isDeveloperView;
            }
        }
        else {
            return false;
        }
    };
    //#endregion

    //#region BPM
    $scope.deployBpmnMap = function (redeploy) {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.deployMap) {
                scope.deployMap(redeploy);
            }
        }
    };
    $scope.loadVersions = function () {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.loadVersions) {
                scope.loadVersions();
            }
        }
    };

    $scope.saveAsPng = function () {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.deployMap) {
                scope.saveBPMNAsPng();
            }
        }
    }
    //#endregion

    //#region Correspondence
    $scope.openWordTemplateClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.openWordTemplateClick) {
            scope.openWordTemplateClick();
        }
    };

    $scope.onSynchroniseClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.onSynchroniseClick) {
            scope.onSynchroniseClick();
        }
    };

    //#endregion

    //#region Html Forms
    $scope.onRefreshClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            if (!scope.isDirty) {
                scope.OnRefreshClick();
            } else {
                alert("Please save the file before refresh.");
            }
        }
    };

    $scope.onDisplayUnmapColumnclick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.OnDisplayUnmapColumnclick();
        }
    };
    //#endregion


    //#region Save logic
    $scope.onsavefileclick = function (obj) {

        if (!obj.IsSavePending) {
            if ($scope.selectDesignSource() == true) {
                $scope.onsavefilexmlclick(obj);
                return;
            }
            var filetype = "";
            var filedetails;
            if (obj) {
                filetype = obj.file.FileType;
                filedetails = obj.file;
            }

            if (filetype == "ExcelScenario") {
                alert('Excel Scenario cannot be saved.');
            }
            else if (obj) {
                var curscope = getScopeByFileName(obj.file.FileName);
                if (curscope && (!curscope.canSaveFile || (curscope.canSaveFile && curscope.canSaveFile()))) {
                    if (curscope && curscope.BeforeSaveToFile) {
                        curscope.BeforeSaveToFile();
                    }

                    if (filetype == "NeoRuleSettings") {
                        var scopeobject = $scope.GetCurrentScopeObjectModel(filetype, "SettingsPage");
                    }
                    else {
                        var scopeobject = $scope.GetCurrentScopeObjectModel(filetype, obj.file.FileName);
                    }

                    if (scopeobject) {
                        $rootScope.SaveModelWithPackets(scopeobject, filedetails, filetype, false);
                    }
                }
            }
        }

    };

    $scope.GetCurrentScopeObjectModel = function (FileType, FileName) {
        var scope = getScopeByFileName(FileName);
        if (FileType == "NeoRuleSettings") {
            return scope.configureSettingsDetails;
        }
        else if (FileType == "LogicalRule" || FileType == "DecisionTable" || FileType == "ExcelMatrix") {
            return scope.objLogicalRule;
        }
        else if (FileType == "ParameterScenario" || FileType == "ObjectScenario" || FileType == "ExcelScenario" || FileType == "DatasourceScenario") {
            return scope.objEntityBasedScenario;
        }
        else if (FileType == "Entity") {
            return scope.objEntity;
        }
        else if (FileType == "RuleConstants") {
            return scope.objDummyConstants;
        }
        else if (FileType == "AuditLog") {
            return scope.AuditlogFiledata;
        }
        else if (FileType == "ProjectConfiguration") {
            return scope.projectconfigurationfiledata;
        }
        else if (FileType == "CustomSettings") {
            return scope.customsettingsfiledata;
        }
        else if (FileType == "Correspondence") {
            return scope.objCorrespondenceDetails;
        }
        else if (FileType == "Maintenance" || FileType == "Lookup" || FileType == "Wizard" || FileType == "UserControl" || FileType == "Tooltip") {
            var model = GetBaseModel(scope.FormModel);
            return model;
        }
        else if (FileType == "Report") {
            return scope.ReportModel;
        }
        else if (FileType == "InboundFile" || FileType == "OutboundFile") {
            return scope.objFile;
        }
        else if (FileType == "BPMN" || FileType == "BPMTemplate") {
            return scope.BPMNModel;
        }
        else if (FileType == "FormLinkMaintenance" || FileType == "FormLinkLookup" || FileType == "FormLinkWizard") {
            return scope.FormLinkModel;
        }
        else if (FileType == "WorkflowMap") {
            return scope.WorkflowMapModel;
        }
    };



    $rootScope.SaveModelWithPackets = function (scopeobject, filedetails, filetype, iscreatenew) {
        var objreturn1;
        if (typeof (scopeobject) == "object") {
            if (filetype == "DecisionTable") {
                objreturn1 = scopeobject;
            }
            else if (filetype == "BPMN" || filetype == "BPMTemplate") {
                objreturn1 = scopeobject;
            }
            else {
                objreturn1 = GetBaseModel(scopeobject);
            }
        }
        if (objreturn1 && !(objreturn1.Name || objreturn1.ID || objreturn1.RuleID)) { // Fixed Bug 10330:while opening any correspondence and hitting on save button , stack trace is displayed 
            return;
        }
        var strobj;
        var saveType;
        if (typeof (scopeobject) == "object") {
            strobj = JSON.stringify(objreturn1);
            saveType = "SaveModel";
        }
        else {
            strobj = scopeobject;
            saveType = "SaveXMLSource";
        }
        for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
            if ($rootScope.lstopenedfiles[i].file.FileName === filedetails.FileName) {
                $rootScope.lstopenedfiles[i].IsSavePending = true;
                break;
            }
        }
        $rootScope.IsLoading = true;
        //if (strobj.length < 32000) {
        if (typeof (scopeobject) == "object") {
            hubMain.server.saveModel(strobj, filedetails, true, iscreatenew);
        }
        else {
            hubMain.server.saveXMLString(strobj, filedetails, true);
        }
        //}
        //else {
        //    var strpacket = "";
        //    var lstDataPackets = [];
        //    var count = 0;
        //    for (var i = 0; i < strobj.length; i++) {
        //        count++;
        //        strpacket = strpacket + strobj[i];
        //        if (count == 32000) {
        //            count = 0;
        //            lstDataPackets.push(strpacket);
        //            strpacket = "";
        //        }
        //    }
        //    if (count != 0) {
        //        lstDataPackets.push(strpacket);
        //    }
        //    //SendChunkByIndex(0, lstDataPackets, filedetails, saveType);
        //    SendDataPacketsToServer(lstDataPackets, filedetails, saveType);
        //}
    };

    //var SendChunkByIndex = function (i, Base64Chunks, filedetails, saveType, aNodeID) {
    //    if (i < Base64Chunks.length) {
    //        var EOF = false;
    //        if (i == Base64Chunks.length - 1) {
    //            EOF = true;
    //        }
    //        hubMain.server.sendDataPackets(Base64Chunks[i], EOF, i, filedetails, saveType, aNodeID).done(function () {
    //            SendChunkByIndex(i + 1, Base64Chunks, filedetails, saveType, aNodeID);
    //        }).fail(function () {
    //            //error message
    //        });
    //    }
    //}

    var SendDataPacketsToServer = function (lstpackets, filedetails, saveType) {

        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, saveType, null);
        }
    };

    $scope.receiveSavedModel = function (ErrorObject, fileDetails) {
        if ($rootScope.currentopenfile && fileDetails) {
            var FileName = fileDetails.FileName;
            var curscope = getScopeByFileName(FileName);
            if (curscope) {
                curscope.$evalAsync(function () {
                    curscope.isDirty = false;
                    if (curscope && curscope.AfterSaveToFile) {
                        curscope.AfterSaveToFile();
                    }
                    $rootScope.isFilesListUpdated = true;
                });
                //if (ErrorObject) {
                //    if (ErrorObject.currentfile) {
                //        if (curscope.currentfile.FileName.match("^rul")) {
                //            curscope.displayValidation(ErrorObject);
                //        }
                //    }
                //}

                if (curscope.currentfile && curscope.currentfile.FileName.match("^ent")) {
                    curscope.ValidateObject();
                }
            }
        }

        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileType == "RuleConstants") {
            hubMain.server.reloadConstantFroValidateExpression().done(function (strerror) {
                $scope.UpdateUI(curscope);
                if (strerror) {
                    alert(strerror);
                }
            });
        }
        else {
            $scope.UpdateUI(curscope);
        }
    };

    $scope.receivePendingSaveModel = function (strFileName) {
        if (strFileName) {
            for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                if ($rootScope.lstopenedfiles[i].file.FileName === strFileName) {
                    $rootScope.lstopenedfiles[i].IsSavePending = false;
                    break;
                }
            }
        }
    }

    $scope.UpdateUI = function (curscope) {
        $rootScope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            if ($rootScope.IsTFSConfigured) { // changes made coz after saving file,tfs state was not coming properly.
                if ($rootScope.currentopenfile) {
                    $rootScope.currentopenfile.file.TFSState = "InEdit";
                    $rootScope.updateUserFileDetails($rootScope.currentopenfile.file);
                }

                // for updating the tfs chart
                $rootScope.IsTfsUpdated = true;
                $DashboardFactory.getTfsStatusData();
            }

            if (curscope && curscope.removeExtraFieldsDataInToMainModel) {
                curscope.removeExtraFieldsDataInToMainModel();
            }
            if (curscope && curscope.isSilentSave) {
                curscope.isSilentSave = false;
            }
            else {
                toastr.success("File saved sucessfully.");
            }
        });
    }
    //#endregion

    //#region Save XML Source
    $scope.onsavefilexmlclick = function (obj) {
        if (obj) {

            var filetype = obj.file.FileType;
            var filedetails = obj.file;
            var curscope = getScopeByFileName(obj.file.FileName);
            if (filetype == "ExcelScenario") {
                alert('Excel Scenario cannot be saved.');
            }
            else {
                var scopexmlstring;
                if (curscope && curscope.editor) {
                    if (curscope.isSourceDirty) {
                        $rootScope.ClearUndoRedoListByFileName(curscope.currentfile.FileName);
                    }
                    scopexmlstring = curscope.editor.getValue();
                }
                if (scopexmlstring && scopexmlstring != "") {
                    $rootScope.SaveModelWithPackets(scopexmlstring, filedetails, filetype, false);
                }
            }

        }
    };
    //#endregion

    $scope.openCurrentEntity = function (entName) {
        var entFileName = entName;

        $.connection.hubMain.server.navigateToFile(entFileName, "").done(function (objfile) {
            $rootScope.openFile(objfile, undefined);
        });
    };

    $scope.validateFile = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.validateFileData) {
                $("#validation-btn").prop("disabled", true);
                scope.validateFileData(true);
            }
        }
    };

    $scope.addNoLockToQueries = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.addNoLockToQueries) {
                scope.addNoLockToQueries();
            }
        }
    };


    $scope.checkValidation = function () {
        var retVal = false;
        $scope.errorListObj = [];
        if ($Errors.validationListObj.length > 0 && $rootScope.currentopenfile) {
            angular.forEach($Errors.validationListObj, function (obj) {
                if (obj.FileName == $rootScope.currentopenfile.file.FileName) {
                    $scope.errorListObj = obj.errorList;
                    retVal = $scope.errorListObj.length > 0 ? true : false;
                }
            });
        }
        return retVal;
    };
    $scope.checkWarning = function () {
        var retVal = false;
        $scope.warningList = [];
        if ($Errors.validationListObj.length > 0 && $rootScope.currentopenfile) {
            angular.forEach($Errors.validationListObj, function (obj) {
                if (obj.FileName == $rootScope.currentopenfile.file.FileName) {
                    if (obj.warningList) {
                        $scope.warningList = obj.warningList;
                        retVal = $scope.warningList.length > 0 ? true : false;
                    }
                }
            });
        }
        return retVal;
    }
    $scope.showErrorList = function () {
        if ($Errors.validationListObj.length > 0) {
            if ($('#validation-error-list').length > 0) {
                $('#validation-error-list').toggle();
            }
        }
    };
    $scope.showWarnings = function () {
        if ($Errors.validationListObj.length > 0) {
            if ($('#validation-warning-list').length > 0) {
                $('#validation-warning-list').toggle();
            }
        }
    };
    $scope.navigateToError = function (errObj) {
        $('#validation-error-list').fadeOut();
        $('#validation-warning-list').fadeOut();
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.findParentAndChildObject) {
                scope.findParentAndChildObject(errObj);
            }
        }
    };
    $scope.toggleErrorList = function (obj) {
        if (obj.isExpand) {
            obj.isExpand = false;
        } else {
            obj.isExpand = true;
        }
    };

    //#region Load Details

    $scope.openLoadDetails = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenLoadDetailsDialog();
            }
        }
    };
    //#endregion


    //#region Workflow Refresh Map


    $scope.RefreshWorkflowMap = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.RefreshWorkflowMap();
        }
    };
    //#endregion

    $scope.CutCopyPasteClick = function (operation) {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (operation == "cut" || operation == "copy" || operation == "paste") {
                if (scope && scope.OnCopyCutPasteControlClick && !scope.selectedDesignSource) {
                    scope.OnCopyCutPasteControlClick(operation);
                }
            }
            else if ($rootScope.currentopenfile.file.FileType == "Lookup" || $rootScope.currentopenfile.file.FileType == "Maintenance" || $rootScope.currentopenfile.file.FileType == "Wizard" || $rootScope.currentopenfile.file.FileType == "Tooltip" || $rootScope.currentopenfile.file.FileType == "UserControl" || $rootScope.currentopenfile.file.FileType == "Report" || $rootScope.currentopenfile.file.FileType == "Correspondence") {
                if (operation == "InsertRowAbove" || operation == "InsertRowBelow" || operation == "InsertColumnLeft" || operation == "InsertColumnRight"
                    || operation == "MoveRowUp" || operation == "MoveRowDown" || operation == "MoveColumnLeft" || operation == "MoveColumnRight") {
                    if (scope && scope.OnRowColumnInsertMoveClick && !scope.selectedDesignSource) {
                        scope.OnRowColumnInsertMoveClick(operation);
                    }
                }
                else if (operation == "InsertCellLeft" || operation == "InsertCellRight") {
                    if (scope && scope.OnInsertCellClick && !scope.selectedDesignSource) {
                        scope.OnInsertCellClick(operation);
                    }
                }
            }
        }
    };



    //#region Centre Left

    $scope.openCentreLeft = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenCentreLeftDialog();
            }
        }
    };

    //#endregion
}]);

$(document).ready(function () {
    $(document).on("keydown", function (e) {
        if (e.shiftKey && e.keyCode == $.ui.keyCode.TAB) {
            e.preventDefault();
            e.stopPropagation();
            if (e.shiftKey && $("#active-files").hasClass("hide")) {
                if (document.getElementsByClassName("ui-dialog").length == 0) {
                    openActiveFileDialog();
                }
            }
        }
        if (e.ctrlKey && e.keyCode == 90) {
            if (!(e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $("#btnUndo:not([disabled]):visible").click();
            }
            e.preventDefault();

        }
        else if (e.ctrlKey && e.keyCode == 89) {
            if (!(e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $("#btnRedo:not([disabled]):visible").click();
            }
            e.preventDefault();

        }
        else if (e.ctrlKey && e.keyCode == 83) {
            if ((e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $(e.target).blur();
            }
            $("#btnSave:not([disabled]):visible").click();
            return false;
        }
        //cut
        else if (e.ctrlKey && e.keyCode == 88) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("cut");
            }
        }
        //copy
        else if (e.ctrlKey && e.keyCode == 67) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("copy");
            }
            // e.preventDefault();
        }
        //paste
        else if (e.ctrlKey && e.keyCode == 86) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("paste");
            }
            //e.preventDefault();
            //return;
        }
        //Insert Row Above
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 65) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertRowAbove");
            }
            e.preventDefault();
        }
        //Insert Row Below
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 66) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertRowBelow");
            }
            e.preventDefault();
        }
        //Insert Column Left
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertColumnLeft");
            }
            e.preventDefault();
        }
        //Insert Column Right
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertColumnRight");
            }
            e.preventDefault();
        }
        //Insert Cell Left
        else if (e.ctrlKey && e.altKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertCellLeft");
            }
            e.preventDefault();
        }
        //Insert Cell Right
        else if (e.ctrlKey && e.altKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertCellRight");
            }
            e.preventDefault();
        }
        //Delete Cell
        else if (e.altKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteCellClick && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteCellClick();
                });
                e.preventDefault();
            }
        }
        //Move Row Up
        else if (e.ctrlKey && e.keyCode == 85) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveRowUp");
            }
            e.preventDefault();
        }
        //Move Row Down
        else if (e.ctrlKey && e.keyCode == 68) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveRowDown");
            }
            e.preventDefault();
        }
        //Move Column Left
        else if (e.ctrlKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveColumnLeft");
            }
            e.preventDefault();
        }
        //Move Column Right
        else if (e.ctrlKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveColumnRight");
            }
            e.preventDefault();
        }
        //Delete Column
        else if (e.ctrlKey && e.shiftKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteColumn && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteColumn();
                });
                e.preventDefault();
            }
        }
        //Delete Row
        else if (e.ctrlKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteRow && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteRow();
                });
                e.preventDefault();
            }
        }
        //Clear Panel
        else if (e.shiftKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.clearPanel && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.clearPanel();
                });
                e.preventDefault();
            }
        }
        else if (e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.DeleteSelectedRowsAndColumns) {
                scope.DeleteSelectedRowsAndColumns();
            }
        }

    });
    $(document).on("keyup", function (e) {
        //    if (e.ctrlKey && e.keyCode == 83) {
        //        $("#btnSave").click();
        //    }
        if (!e.shiftKey && $("#active-files").hasClass("show")) {
            if ($("[ui-active-file-wrapper]").find("li.selected").length) {
                $("[ui-active-file-wrapper]").find("li.selected").trigger("click");
            } else {
                $("#active-files").removeClass("show");
                $("#active-files").addClass("hide");
            }
        }
    });
    //Prevent browser back button
    history.pushState(null, null, document.URL);
    window.addEventListener('popstate', function () {
        history.pushState(null, null, document.URL);
    });

    var openActiveFileDialog = function () {
        var element = $('body[ng-controller="MainController"]');
        if (element) {
            rootScope = angular.element(element).scope().$root;
            if (rootScope.IsLoading) {
                return;
            }
        }

        var domActiveFiles = $("#active-files");
        domActiveFiles.removeClass("hide");
        domActiveFiles.addClass("show");
        domActiveFiles.find("input").focus();
        domActiveFiles.find('li.selected').removeClass('selected');
        if (domActiveFiles.find("li:nth-child(2)").length) {
            domActiveFiles.find("li:nth-child(2)").addClass('selected');
        } else {
            domActiveFiles.find("li:nth-child(1)").addClass('selected');
        }
        domActiveFiles.find("input").trigger("keydown");
    };
});
