
function initializeHubreceivemethods() {
    hubMain.client.receiveProjectafterLoad = function (dbConnection, ProjectName, errorList, IsOracleDB) {
        var scope = getScopeByFileName("MainPage");
        if (scope != undefined && scope.receiveProjectafterLoad) {
            scope.receiveProjectafterLoad(dbConnection, ProjectName, errorList, IsOracleDB);
        }
    };

    hubMain.client.receiveFilePath = function (data, source) {
        if (source == "NewExcelMatrix") {
            var scope = getScopeByFileName("MainPage");
            if (scope != undefined && scope.receiveFilePath) {
                scope.receiveFilePath(data);
            }
        }
        else if (source == "ProjectSettings") {
            var scope = getScopeByFileName("SettingsPage");
            if (scope != undefined && scope.receiveProjectSettingsFilePath) {
                scope.receiveProjectSettingsFilePath(data);
            }
        }
        else if (source == "CreateNewObject") {
            var scope = getScopeByFileName("createNewObject");
            if (scope != undefined && scope.receiveNewHtmlFilePath) {
                scope.receiveNewHtmlFilePath(data);
            }
        }
    };

    //#region Rule methods which are recieved from hubMain



    hubMain.client.receiveSavedModel = function (data, strFileName) {
        var scope = getScopeByFileName("PageLayout");
        if (scope != undefined && scope.receiveSavedModel) {
            scope.receiveSavedModel(data, strFileName);
        }
    };
    hubMain.client.updateUserFilesForApp = function (fileDetails) {
        var scope = getScopeByFileName("MainPage");
        if (scope && scope.updateUserFilesForApp) {
            scope.updateUserFilesForApp(fileDetails);
        }
    };

    hubMain.client.receivePendingSaveModel = function (strFileName) {
        var scope = getScopeByFileName("PageLayout");
        if (scope != undefined && scope.receivePendingSaveModel) {
            scope.receivePendingSaveModel(strFileName);
        }
    };

    hubMain.client.receiveUpdatedEntityModelandIntelleenselist = function (lst, model, filename) {
        var scope = getScopeByFileName(filename);
        if (scope != undefined && scope.receiveUpdatedEntityModelandIntelleenselist) {
            scope.receiveUpdatedEntityModelandIntelleenselist(lst, model);
        }
    };

    hubMain.client.receieveMovedRuleFile = function (data, FileName) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receieveMovedRuleFile) {
            scope.receieveMovedRuleFile(data);
        }
    };

    hubMain.client.recieveRefreshNodeIds = function (data) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.recieveRefreshNodeIds) {
            scope.recieveRefreshNodeIds(data);
        }
    };

    //#endregion

    //#region Error/Warning/Message display section
    hubMain.client.receiveExceptionDetails = function (exceptionData) {
        var scope = getScopeByFileName("MainPage");
        if (scope && scope.$root && scope.$root.showExceptionDetails) {
            scope.$root.showExceptionDetails(exceptionData);
        }
    };
    hubMain.client.receiveErrorMessage = function (errorMessage) {
        var scope = getScopeByFileName("MainPage");
        if (scope != undefined && scope.receiveErrorMessage) {
            scope.receiveErrorMessage(errorMessage);
        }
    };
    hubMain.client.receiveProjectLoadError = function (errorMessage) {
        var scope = getScopeByFileName("MainPage");
        if (scope != undefined && scope.receiveProjectLoadError) {
            scope.receiveProjectLoadError(errorMessage);
        }
    };

    //#endregion

    //#region TFS Receive
    //hubMain.client.receiveCheckInTfsStatus = function (FileInfo) {
    //    var scope = getScopeByFileName("MainPage");
    //    if (scope != undefined && scope.receiveCheckInTfsStatus) {
    //        scope.receiveCheckInTfsStatus(FileInfo);
    //    }
    //}
    //hubMain.client.receiveCheckOutTfsStatus = function (FileInfo) {
    //    var scope = getScopeByFileName("MainPage");
    //    if (scope != undefined && scope.receiveCheckOutTfsStatus) {
    //        scope.receiveCheckOutTfsStatus(FileInfo);
    //    }
    //}
    //hubMain.client.receiveFileAddInTFSStatus = function (FileInfo) {
    //    var scope = getScopeByFileName("MainPage");
    //    if (scope != undefined && scope.receiveFileAddInTFSStatus) {
    //        scope.receiveFileAddInTFSStatus(FileInfo);
    //    }
    //}
    //hubMain.client.receiveUndoModel = function (FileInfo) {
    //    var scope = getScopeByFileName("MainPage");
    //    if (scope != undefined && scope.receiveUndoModel) {
    //        scope.receiveUndoModel(FileInfo);
    //    }
    //}
    //hubMain.client.receiveGetLatest = function (FileInfo) {
    //    var scope = getScopeByFileName("MainPage");
    //    if (scope != undefined && scope.receiveGetLatest) {
    //        scope.receiveGetLatest(FileInfo);
    //    }
    //}

    hubMain.client.recieveItemListMappedWithEntity = function (data, filetype) {
        var scope = getCurrentFileScope();
        if (scope && scope.recieveItemListMappedWithEntity) {
            scope.recieveItemListMappedWithEntity(data, filetype);
        }
    };

    //#endregion

    //#region Form Receive Method
    $.connection.hubForm.client.receiveEntityTreeObject = function (data, path) {
        var mainscope = getScopeByFileName("MainPage");
        if (mainscope != undefined) {
            var curfileid = mainscope.$root.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.receiveEntityTreeObject) {
                var data = JSON.parse(data);
                scope.receiveEntityTreeObject(data, path);
            }
        }
    };

    $.connection.hubForm.client.receiveFormParameters = function (data, receiveFormParameters) {
        var mainscope = getScopeByFileName("MainPage");
        if (mainscope != undefined) {
            var curfileid = mainscope.$root.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope != undefined && scope.receiveFormParameters) {
                var data = JSON.parse(data);
                scope.receiveFormParameters(data, receiveFormParameters);
            }
        }
    };

    $.connection.hubForm.client.receiveFormEntityModel = function (data) {
        var mainscope = getScopeByFileName("MainPage");
        if (mainscope != undefined) {
            var curfileid = mainscope.$root.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope != undefined && scope.receiveFormEntityModel) {
                var data = JSON.parse(data);
                scope.receiveFormEntityModel(data);
            }
        }
    };

    $.connection.hubForm.client.receiveGridBaseQueryColumns = function (data) {
        var scope = getScopeByFileName("NewButtonWizard");
        if (scope != undefined && scope.receiveGridBaseQueryColumns) {
            var data = JSON.parse(data);
            scope.receiveGridBaseQueryColumns(data);
        }
    };

    $.connection.hubForm.client.receivenewformmodel = function (data, isFromNewButton, scopeId) {
        if (isFromNewButton) {
            var scope = getScopeByFileName("NewButtonWizard");
            if (scope != undefined && scope.receivenewformmodel) {
                var data = JSON.parse(data);
                scope.receivenewformmodel(data);
            }
        }
        else {
            var scope = angular.element(document.getElementById(scopeId)).scope();
            if (scope && scope.receivenewformmodel) {
                var data = JSON.parse(data);
                scope.receivenewformmodel(data);
            }
            else {
                var scope = getCurrentFileScope();
                if (scope && scope.receivenewformmodel) {
                    var data = JSON.parse(data);
                    scope.receivenewformmodel(data);
                }
            }
        }
    };




    $.connection.hubForm.client.receiveRetrievalControlsColumns = function (data, latestDialogId) {
        var currentelement = $('div[id=' + latestDialogId + ']');
        if (currentelement != undefined) {
            var scope = angular.element(currentelement).scope();
            if (scope != undefined && scope.receiveQueryColumns) {
                var data = JSON.parse(data);
                scope.receiveQueryColumns(data);
            }
            else {
                var scope = getScopeByFileName(latestDialogId);
                if (scope != undefined && scope.receiveQueryColumns) {
                    var data = JSON.parse(data);
                    scope.receiveQueryColumns(data);
                }
            }
        }
    };

    $.connection.hubForm.client.receiveQueryColumns = function (data, scopeId) {
        var scope = angular.element(document.getElementById(scopeId)).scope();
        if (scope && scope.receiveQueryColumns) {
            var data = JSON.parse(data);
            scope.receiveQueryColumns(data);
        }
        else {
            var scope = getScopeByFileName(scopeId);
            if (scope != undefined && scope.receiveQueryColumns) {
                var data = JSON.parse(data);
                scope.receiveQueryColumns(data);
            }
        }
    };

    //#endregion

    //#region Rule methods
    $.connection.hubRuleModel.client.receiveEntityBusinessObject = function (data) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveEntityBusinessObject) {
            scope.receiveEntityBusinessObject(data);
        }
    };

    $.connection.hubRuleModel.client.receiveRuleExpression = function (data, isChildRule, ruleId, FileName) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receiveRuleExpression) {
            scope.receiveRuleExpression(data, isChildRule, ruleId);
        }
    };



    //#endregion

    //#region BPMN Receive Methods
    if ($.connection.hubBPMN) {

        $.connection.hubBPMN.client.receiveAssociatedFormData = function (data) {
            var scope = getCurrentFileScope();
            if (scope && scope.receiveAssociatedFormData) {
                scope.receiveAssociatedFormData(data);
            }
        };

        $.connection.hubBPMN.client.receiveFormModel = function (data, isEnter) {
            var scope = getCurrentFileScope();
            if (scope && scope.receiveFormModel) {
                scope.receiveFormModel(data, isEnter);
            }
        };
    }
    //#endregion

    //#region Entity 


    $.connection.hubEntityModel.client.recievegetcodevalues = function (data) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.recieveGetCodeValues) {
            scope.recieveGetCodeValues(data);
        }
    };

    $.connection.hubEntityModel.client.receiveQueryObjectTree = function (data) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveQueryObjectTree) {
            scope.receiveQueryObjectTree(data);
        }
    };



    $.connection.hubEntityModel.client.receiveObjectTree = function (data, path) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveObjectTree) {
            scope.receiveObjectTree(data, path);
        }
    };

    $.connection.hubEntityModel.client.receiveObjectTreeIntellisenseChildData = function (data, text) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveObjectTreeIntellisenseChildData) {
            scope.receiveObjectTreeIntellisenseChildData(data, text);
        }
    };



    $.connection.hubEntityModel.client.receivePartialFileData = function (data, types, busObjPartialFileName) {
        var partialFileData = JSON.parse(data);
        var dataTypes = JSON.parse(types);
        var scope = getCurrentFileScope();
        if (scope && scope.receivePartialFileData) {
            scope.receivePartialFileData(partialFileData, dataTypes, busObjPartialFileName);
        }
    };

    $.connection.hubEntityModel.client.receiveValidateExpressionMsg = function (data, dialogId) {
        var scope;
        if (dialogId) {
            var currentelement = $('div[id=' + dialogId + ']');
            scope = angular.element(currentelement).scope();
        }
        else {
            scope = getCurrentFileScope();
        }
        if (scope && scope.receiveValidateExpressionMsg) {
            scope.receiveValidateExpressionMsg(data);
        }
    };

    //#endregion

    $.connection.hubMain.client.receiveAllRuleFiles = function (data, scopeId) {
        var scope = angular.element(document.getElementById(scopeId)).scope();
        if (scope && scope.receiveAllRuleFiles) {
            var data = JSON.parse(data);
            scope.receiveAllRuleFiles(data);
        }
    };

    hubMain.client.receiveList = function (data, scopeId) {
        var scope;
        if (scopeId && scopeId.trim().length > 0) {
            scope = angular.element(document.getElementById(scopeId)).scope();
        }
        else {
            scope = getCurrentFileScope();
        }
        if (scope && scope.receiveList) {
            var data = JSON.parse(data);
            scope.receiveList(data);
        }
    };

    hubMain.client.setFileListTypewise = function (data, totalRecords) {
        var mainscope = getScopeByFileName("MainPage");
        if (mainscope != undefined && mainscope.setFileTypeWise) {
            mainscope.setFileTypeWise(data, totalRecords);
        }
    };

    hubMain.client.receieveExecuteQuery = function (data, dialogID) {
        //var scope = GetCurrentScopeObject("ExecuteQuery");
        var scope;
        if (dialogID != "" && dialogID != undefined) {
            var currentelement = $('div[id=' + "ScopeId_" + dialogID + ']');
            scope = angular.element(currentelement).scope();
        }
        else {
            //scope = getScopeByFileName("ExecuteQuery");
            scope = getCurrentFileScope();
        }
        if (scope != undefined && scope.receieveExecuteQuery) {
            scope.receieveExecuteQuery(data);
        }
    };

    hubMain.client.receiveErrorMessageQuery = function (data, dialogID) {
        var scope;
        if (dialogID != "" && dialogID != undefined) {
            var currentelement = $('div[id=' + "ScopeId_" + dialogID + ']');
            scope = angular.element(currentelement).scope();
        }
        else {
            scope = getCurrentFileScope();
        }
        if (scope != undefined && scope.receiveErrorMessageQuery) {
            scope.receiveErrorMessageQuery(data);
        }
    };



    $.connection.hubConstants.client.receieveConstantsAfterRefresh = function (data, errMsgs) {
        var scope = getCurrentFileScope();
        if (scope && scope.receieveConstantsAfterRefresh) {
            scope.receieveConstantsAfterRefresh(data, errMsgs);
        }
    };

    //#region Scenario Receive Methods



    $.connection.hubScenarioModel.client.receiveObjectTree = function (data, scopeID) {
        var scope = angular.element(document.getElementById(scopeID)).scope();
        //var scope =  getCurrentFileScope();
        if (scope != undefined && scope.receiveObjectTree) {
            scope.receiveObjectTree(data);
        }
    };


    //#endregion

    //#region Create Object Receive Method


    $.connection.hubCreateNewObject.client.receiveFolderPathForFile = function (data, source) {
        var scope = getScopeByFileName("createNewObject");
        if (scope != undefined && scope.receiveFolderPathForFile) {
            scope.receiveFolderPathForFile(data, source);
        }
    };

    $.connection.hubCreateNewObject.client.receiveQueryColumnsForLookup = function (data) {
        var scope = getScopeByFileName("createNewObject");
        if (scope != undefined && scope.receiveQueryColumnsForLookup) {
            scope.receiveQueryColumnsForLookup(data);
        }
    };

    $.connection.hubCreateNewObject.client.receiveModelForForms = function (data, fileName) {
        var scope = getScopeByFileName("createNewObject");
        if (scope != undefined && scope.receiveModelForForms) {
            scope.receiveModelForForms(data, fileName);
        }
    };





    $.connection.hubCreateNewObject.client.receiveMapUnmapControls = function (data, controlTypes, isNew, newModel, oldModel) {
        var scope = getCurrentFileScope();
        if (scope && scope.receiveMapUnmapControls) {
            scope.receiveMapUnmapControls(data, controlTypes, newModel, oldModel);
        }
    };


    //#endregion

    //#region Html form Receive method


    $.connection.hubFormLink.client.receiveRefreshedModel = function (data) {
        var scope = getCurrentFileScope();
        if (scope && scope.receiveRefreshedModel) {
            scope.receiveRefreshedModel(data);
        }
    };

    $.connection.hubFormLink.client.receiveRemovedControls = function (data, oldModel) {
        var scope = getCurrentFileScope();
        if (scope && scope.receiveRemovedControls) {
            scope.receiveRemovedControls(data, oldModel);
        }
    };

    $.connection.hubFormLink.client.receiveControlMappingForDisplay = function (data) {
        var scope = getCurrentFileScope();
        if (scope && scope.receiveControlMappingForDisplay) {
            var data = JSON.parse(data);
            scope.receiveControlMappingForDisplay(data);
        }
    };
    //#endregion

    //#region Correspondence Receive Methods

    $.connection.hubCorrespondence.client.receieveSyncronizeBookMarkData = function (data) {
        var scope = getCurrentFileScope();
        if (scope && scope.receieveSyncronizeBookMarkData) {
            scope.receieveSyncronizeBookMarkData(data);
        }
    };
    $.connection.hubCorrespondence.client.receieveDummyCall = function () {
        var scope = getCurrentFileScope();
        if (scope && scope.receieveDummyCall) {
            scope.receieveDummyCall();
        }
    };


    //#endregion

    //#region Design To Source Methods Entity

    hubMain.client.receiveentityxmlobject = function (data, path, FileName) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receiveentityxmlobject) {
            scope.receiveentityxmlobject(data, path);
        }
    };
    //#endregion

    //#region Design To Source Methods Rule
    hubMain.client.receiverulexml = function (xmlstring, lineno) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiverulexml) {
            scope.receiverulexml(xmlstring, lineno);
        }
    };
    hubMain.client.receiverulexmlobject = function (data, path, strPath) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiverulexmlobject) {
            scope.receiverulexmlobject(data, path, strPath);
        }
    };
    //#endregion

    //#region Design-Source View For Correspondence, File, Report, Scenario, Form,Constant
    hubMain.client.receivesourcexml = function (xmlstring, lineno) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivesourcexml) {
            scope.receivesourcexml(xmlstring, lineno);
        }
    };
    hubMain.client.receivedesignxmlobject = function (data, path) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivedesignxmlobject) {
            scope.receivedesignxmlobject(data, path);
        }
    };
    //for correspondence and form
    hubMain.client.receiveformcorrdesignxmlobject = function (data, data1, path) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveformcorrdesignxmlobject) {
            scope.receiveformcorrdesignxmlobject(data, data1, path);
        }
    };
    //for file
    hubMain.client.receivefiledesignxmlobject = function (data, IsChildOfInboundFileBase, lstMethods, lstFields, path) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivefiledesignxmlobject) {
            scope.receivefiledesignxmlobject(data, IsChildOfInboundFileBase, lstMethods, lstFields, path);
        }
    };
    //for scenario
    hubMain.client.receivescenariodesignxmlobject = function (data, data1, data2, objRuleFileInfo, path) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivescenariodesignxmlobject) {
            scope.receivescenariodesignxmlobject(data, data1, data2, objRuleFileInfo, path);
        }
    };
    //#endregion

    //#region Common Receive Method
    hubMain.client.receivexml = function (xmlstring, FileName) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receivexml) {
            scope.receivexml(xmlstring);
        }
    };
    hubMain.client.receivexmlobject = function (data, FileName) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receivexmlobject) {
            scope.receivexmlobject(data);
        }
    };
    hubMain.client.setFileFindAdvance = function (dataFindObj, dataList) {
        var scope = getScopeByFileName("FindAdvanceFile");
        if (scope != undefined && scope.setFileFindAdvance) {
            scope.setFileFindAdvance(dataFindObj, dataList);
        }
    };

    //#endregion

    //#region Design/Source Receive Method for FormLink
    hubMain.client.receiveFormLinkXml = function (xmlstring, FileName, lineNumber) {
        var scope = getScopeByFileName(FileName);
        if (scope != undefined && scope.receiveFormLinkXml) {
            scope.receiveFormLinkXml(xmlstring, lineNumber);
        }
    };

    //#endregion
    //#region Design To Source Methods BPM
    hubMain.client.receivebpmsourcexml = function (xmlstring, lineno) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivebpmsourcexml) {
            scope.receivebpmsourcexml(xmlstring, lineno);
        }
    };
    hubMain.client.receivebpmdesignxmlstring = function (data, nodeId) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receivebpmdesignxmlstring) {
            scope.receivebpmdesignxmlstring(data, nodeId);
        }
    };
    //#endregion

    //#region Save Resources
    hubMain.client.saveResourceException = function (lstResourceSaveException, dialogID) {
        var scope;
        if (dialogID != "" && dialogID != undefined) {
            var currentelement = $('div[id=' + dialogID + ']');
            scope = angular.element(currentelement).scope();
        }
        if (scope && scope.receiveSaveResourceException) {
            scope.receiveSaveResourceException(lstResourceSaveException);
        }
    };
    //#endregion

    hubMain.client.receieveHTMLFromModel = function (xmlstring) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receieveHTMLFromModel) {
            scope.receieveHTMLFromModel(xmlstring);
        }
    };

    hubMain.client.receiveFileStatus = function (data) {
        var scope = getScopeByFileName("ConvertToHtx");
        if (scope != undefined && scope.receiveFileStatus) {
            scope.receiveFileStatus(data);
        }
    };
    hubMain.client.logoutCurrentClient = function (connectionId) {
        window.location.href = 'error_page.html?ERROR_CODE=102';
    };

    hubMain.client.verifyCurInstanceOperation = function (newconnectionId) {
        if (confirm('One instance of s3 is already running. Unsaved changed will be lost. Do you want to continue ?')) {
            hubMain.server.reloadCurrentInstance(newconnectionId).done(function () {
            });
        }
        else {
            window.location.href = 'error_page.html?ERROR_CODE=102';
        }
    };
    hubMain.client.receiveAfterUpdateAllProjectLoadDetails = function (data) {
        var scope = getCurrentFileScope();
        if (scope != undefined && scope.receiveAfterUpdateAllProjectLoadDetails) {
            scope.receiveAfterUpdateAllProjectLoadDetails(data);
        }
    }

    hubMain.client.receiveExportvalidationReport = function (strFileName) {
        var scope = getScopeByFileName("ValidationUtilityControl");
        if (scope != undefined && scope.receiveExportvalidationReport) {
            scope.receiveExportvalidationReport(strFileName);
        }
    };
}
