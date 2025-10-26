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

//When we're ready we can bootstrap our app and pass in
//data from our server to the config step for our app

var count_getcurrentfile = 0;

var bootstrap = function (app, dataFromServer) {
    app
        .provider('hubcontext', [function HubcontextProvider() {
            this.$get = function () { return $.connection; };
        }])
        .config(["$provide", "$httpProvider", function ($provide, $httpProvider) {
            $provide.decorator('$exceptionHandler', [extendExceptionHandler]);
            $httpProvider.interceptors.push('noCacheInterceptor');
            //$provide.decorator('inputDirective', function ($delegate) {
            //    var directive = $delegate[0],
            //        link = directive.link;
            //    link.post = function (scope, element, attrs) {
            //        attrs.$set('ngTrim', 'false');
            //    };
            //    return $delegate;
            //});
        }])
        .run(["hubcontext", "$rootScope", function (hubcontext, $rootScope) {
            if (hubcontext.hubMain) {
                hubcontext.hub.start().done(successCallbackHubStart).fail(errorCallbackHubStart);
                $.connection.hub.disconnected(disconnectedCallback);
                function successCallbackHubStart() {
                    $rootScope.$evalAsync(function () {
                        $rootScope.IsAppLoaded = true;
                        $rootScope.IsHubWorking = true;
                    });
                }
                function errorCallbackHubStart() {
                    // code for executing if hub fails
                }
                function disconnectedCallback() {
                    $.connection.hub.start().done(successCallbackHubStart).fail(errorCallbackHubStart);
                }
            }
            else {
                $rootScope.IsHubWorking = false;
                $rootScope.IsAppLoaded = true;
            }
        }]);

    var $app = $("[s3-app]");
    angular.bootstrap($app, ["myApp"]);
};

var app = angular.module('myApp', ['ngDialog', 'ngSanitize', 'mgo-angular-wizard', 'ngCookies', 'smart-table']);
var hubMain = null;
var s3Config = null;
URL_HTTP = URL_HUB = "http://localhost:8080",
    URL_HTTPS = "https://localhost:8083";

if (location.protocol === 'https:') {
    URL_HUB = URL_HTTPS;
}

$.ajax({
    url: 'api/Login/GetHostMachinePort',
    type: 'GET',
    async: false,
    dataType: 'json',
    data: {},
    success: function (data) {
        if (data) {
            if (data["HostMachineName"] && data["Port"]) {
                if (location.protocol === 'https:') {
                    URL_HUB = "https://" + data["HostMachineName"] + ":" + data["Port"];
                }
                else {
                    URL_HUB = "http://" + data["HostMachineName"] + ":" + data["Port"];
                }
            }
        }
    },
    error: function (response) {
        var objError = JSON.parse(response.responseText);
        if (objError && objError.Message) {
            var msg = objError.Message;
            if (objError.ExceptionMessage) {
                msg = msg + "\n" + objError.ExceptionMessage;
            }
            console.log(msg);
        }
        else {
            console.log("An error occured while creating a new GUID.");
        }
    }
});


$.ajax({
    async: false,
    url: URL_HUB + "/signalr/hubs",
    type: 'GET',
    success: function () {
        $.connection.hub.url = URL_HUB + "/signalr";
        hubMain = $.connection.hubMain;
        initializeHubreceivemethods();
        //to read the package file 
        $.getJSON("package.json?" + Date.now(), function (data) {
            s3Config = data;
        });

        angular.element(document.body).ready(function () {
            bootstrap(app);
        });
    },
    error: function (a, b, c) {
        window.location.href = "error_page.html?ERROR_CODE=101";
        //alert("HUB could not be reached");
    }
});

app.factory('noCacheInterceptor', [function () {
    return {
        request: function (config) {
            // to break html cache - url is appended with client version to 
            if (s3Config && config.method == 'GET' && config.url.indexOf('.html') > 0 && config.url != 'wizard.html' && config.url != 'step.html') {
                var separator = config.url.indexOf('?') === -1 ? '?' : '&';
                config.url = config.url + separator + 'rev=' + s3Config.versionClient;
            }
            return config;
        }
    };
}]);

function extendExceptionHandler() {
    return function (exception, cause) {
        var rootScope = angular.element(document.body).injector().get("$rootScope");
        if (rootScope) {
            rootScope.IsLoading = false;
            rootScope.IsProjectLoaded = false;
        }
        $(".page-header-fixed").css("pointer-events", "auto");
        //toastr.error("Exception:" + exception+"\nCause:"+cause);
        console.log(String.format("Exception:{0}\nCause:{1}\nStacktrace:{2}", exception, cause, exception.stack));
        alert(String.format("Exception:{0}\nCause:{1}\nStacktrace:{2}", exception, cause, exception.stack));
    };
}

app.filter('trustAsResourceUrl', ['$sce', function ($sce) {
    return function (val) {
        return $sce.trustAsResourceUrl(val);
    };
}]);
String.prototype.contains = function (str, ignoreCase) {
    if (ignoreCase) {
        return this.toLowerCase().indexOf(str.toLowerCase()) > -1;
    }
    else {
        return this.indexOf(str) > -1;
    }
};
String.prototype.startsWith = function (str, ignoreCase) {
    if (ignoreCase) {
        return this.toLowerCase().indexOf(str.toLowerCase()) === 0;
    }
    else {
        return this.indexOf(str) === 0;
    }
};
String.prototype.endsWith = function (str, ignoreCase) {
    if (ignoreCase) {
        if (this.toLowerCase().indexOf(str.toLowerCase()) > -1) {
            return this.toLowerCase().lastIndexOf(str.toLowerCase()) == this.length - str.length;
        } else {
            return false;
        }
    }
    else {
        if (this.indexOf(str) > -1) {
            return this.lastIndexOf(str) == this.length - str.length;
        } else {
            return false;
        }
    }
};
String.prototype.capitalizeFirstLetter = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
};
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
};

String.prototype.trimStart = function (trimString, ignoreCase) {
    var targetString = this;
    if (!trimString) {
        trimString = " ";
    }
    if (targetString.startsWith(trimString, ignoreCase)) {
        targetString = targetString.substring(trimString.length);
        targetString = targetString.trimStart(trimString, ignoreCase);
    }
    return targetString;
};
String.prototype.trimEnd = function (trimString, ignoreCase) {
    var targetString = this;
    if (!trimString) {
        trimString = " ";
    }
    if (targetString.endsWith(trimString, ignoreCase)) {
        targetString = targetString.substring(0, targetString.length - trimString.length);
        targetString = targetString.trimEnd(trimString, ignoreCase);
    }
    return targetString;
};
String.prototype.trimFull = function (trimString, ignoreCase) {
    return this.trimStart(trimString, ignoreCase).trimEnd(trimString, ignoreCase);
};

Array.prototype.unique = function () {
    var list = this;
    return list.filter(function (item, pos) {
        return list.indexOf(item) == pos;
    });
};

Array.prototype.difference = function (a, propertyname) {
    if (!propertyname) return this.filter(function (i) { return a.indexOf(i) < 0; });
    else return this.filter(function (i) {
        return a.map(function (element) { if (element) { return element[propertyname]; } }).indexOf(i[propertyname]) < 0;
    });
};

function getCurrentFileScope() {
    element = $('body[ng-controller="MainController"]');
    if (element) {
        rootScope = angular.element(element).scope().$root;
        if (rootScope.isConfigureSettingsVisible) {
            return getScopeByFileName("SettingsPage");
        }
        else if (rootScope && rootScope.currentopenfile && rootScope.currentopenfile.file) {
            return getScopeByFileName(rootScope.currentopenfile.file.FileName);
        }
    }
}

function getScopeByFileName(fileName) {
    /// <summary>Get the scope of a div which is created for the specified filename. For Main page pass fileName as 'MainPage'. For Start page pass fileName as 'StartPage'. For Settings page pass fileName as 'SettingsPage'. </summary>
    /// <param name="fileName" type="string">filename for which scope is required.</param>
    /// <returns type="object">scope of a div which is created for the specified filename.</returns>
    var element;
    var scope;
    if (fileName == "MainPage") {
        element = $('body[ng-controller="MainController"]');
    }

    else if (fileName == "StartPage") {
        element = $('div[ng-controller="StartController"]');
    }
    else if (fileName == "SettingsPage") {
        element = $('div[ng-controller="configureSettingsController"]');
    }
    else if (fileName == "PageLayout") {
        element = $('div[ng-controller="PageLayoutController"]');
    }
    else if (fileName == "ParameterNavigation") {
        element = $('div[ng-controller="NavigationParameterController"]');
    }
    else if (fileName == "ImageCondition") {
        element = $('div[ng-controller="ImageConditionController"]');
    }
    else if (fileName == "createNewObject") {
        element = $('div[ng-controller="createNewObjectController"]');
    }
    else if (fileName == "NewButtonWizard") {
        element = $('div[ng-controller="NewButtonWizardController"]');
    }
    else if (fileName == "ExecuteQuery") {
        element = $('div[ng-controller="executequerycontroller"]');
    }
    else if (fileName == "CustomSettings") {
        element = $('div[ng-controller="customSettingsController"]');
    }
    else if (fileName == "Form") {
        element = $('div[ng-controller="FormController"]');
    }
    else if (fileName == "SearchFiles") {
        element = $('div[ng-controller="SearchFilesController"]');
    }
    else if (fileName == "AutoCompleteColumnsController") {
        element = $('div[ng-controller="AutoCompleteColumnsController"]');
    }
    else if (fileName == "NavigationParameterController") {
        element = $('div[ng-controller="NavigationParameterController"]');
    }
    else if (fileName == "RetrievalControlsController") {
        element = $('div[ng-controller="RetrievalControlsController"]');
    }
    else if (fileName == "EntityValidationRulesController") {
        element = $('div[ng-controller="EntityValidationRulesController"]');
    }
    else if (fileName == "ConvertToHtx") {
        element = $('div[ng-controller="ConvertToHtxController"]');
    }
    else if (fileName == "ValidationUtilityControl") {
        element = $('div[ng-controller="ValidationUtilityControl"]');
    }
    else {
        element = $("div[id='" + fileName + "']").find("div[ng-controller]");
    }

    if (element) {
        scope = angular.element(element).scope();
    }
    return scope;
}

function GetVM(ControlName, acntrlVM) {
    var parentVM = acntrlVM;
    while (parentVM) {
        if (parentVM.Name == ControlName) {
            return parentVM;
        }
        parentVM = parentVM.ParentVM;
    }

}

function startsWith(actulalString, searchString, position) {
    return actulalString.indexOf(searchString, position) === position;
    //lowercase removed bcozit matches wrong in case of form
    // return actulalString.toLowerCase().indexOf(searchString.toLowerCase(), position) === position;
}

function endsWith(str, suffix) {
    if (str) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }
}

function setSingleLevelAutoComplete(controlSelection, data, scope, propertyName, displaypropertyname, callBackFunction) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // press up or down key    
        return;
    }
    // this code should be at server side
    if (data) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (a && b) {
                if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname) && a[displaypropertyname]) {
                    nameA = a[displaypropertyname].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName) && a[propertyName]) {
                    nameA = a[propertyName].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName) && a.dictAttributes[propertyName]) {
                    nameA = a.dictAttributes[propertyName].toLowerCase();
                }
                else if (a.hasOwnProperty("ID") && a.ID) {
                    nameA = a.ID.toLowerCase();
                }

                else if (a && a.toLowerCase) {
                    nameA = a.toLowerCase();
                }

                if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname) && b[displaypropertyname]) {
                    nameB = b[displaypropertyname].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName) && b[propertyName]) {
                    nameB = b[propertyName].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName) && b.dictAttributes[propertyName]) {
                    nameB = b.dictAttributes[propertyName].toLowerCase();
                }
                else if (b.hasOwnProperty("ID") && b.ID) {
                    nameB = b.ID.toLowerCase();
                }
                else if (b && b.toLowerCase) {
                    nameB = b.toLowerCase();
                }

                if (nameA && nameB) {
                    if (nameA < nameB) //sort string ascending
                        return -1;
                    if (nameA > nameB)
                        return 1;
                }
            }
            return 0; //default return value (no sorting)
        });

        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "span") {
                    var doc = controlSelection[0].ownerDocument || controlSelection[0].document;
                    var win = doc.defaultView || doc.parentWindow;
                    var Position;
                    var sel;
                    if (win && typeof win.getSelection != "undefined") {
                        sel = win.getSelection();
                        if (sel.rangeCount > 0) {
                            var range = win.getSelection().getRangeAt(0);
                            Position = range.getBoundingClientRect();
                        }
                    }
                    var options;
                    if (!Position || (Position.left == 0 && Position.top == 0)) {
                        options = {
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px",
                            "z-index": "999999999",
                        };
                    } else {
                        options = {
                            left: (Position.left) + "px",
                            top: (Position.top + Position.height) + "px",
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px",
                            "z-index": "999999999",
                        };
                    }
                    $("#dvIntellisense > ul").css(options);
                    setIntellisensePosition(event);
                }
                else if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type"))) {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        options.left = (pos.left) + "px";
                        // options.top = (pos.top) + "px";

                    }
                    $("#dvIntellisense > ul").css(options);
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    if ($(controlSelection).data('ui-autocomplete')) {
                        var $results = $(controlSelection).autocomplete("widget");
                        windowWidth = $(window).width(),
                            height = $results.height(),
                            inputHeight = $(controlSelection).height(),
                            windowsHeight = $(window).height();
                        if (windowsHeight < $results.position().top + height + inputHeight) {
                            newTop = $results.position().top - height - inputHeight - 10;
                            $results.css("top", newTop + "px");
                        }
                    }
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    $("#dvIntellisense > ul").css(options);
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            source: function (request, response) {
                // single level autocomplete will not filter with any special characters except alphabets
                //var isAlphabets = request.term.match(/^[a-zA-Z0-9 -_]*$/);
                // if (isAlphabets) {
                var result = $.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        var templogo = "";
                        var templogo = "";
                        if (!value) {
                            return;
                        }
                        if (value.Type && value.Type == "Method") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/method.png'/>";
                        }
                        else if (value.Type && value.Type == "Rule") {
                            if (value.RuleType && value.RuleType == "LogicalRule") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/logical_rule.png'/>";

                            }
                            else if (value.RuleType && value.RuleType == "DecisionTable") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/decision_table..png'/>";
                            }
                            else if (value.RuleType && value.RuleType == "ExcelMatrix") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Excel_matrix.png'/>";
                            }

                        }
                        else if (value.Type && value.Type == "Property") {
                            if (value.DataType && value.DataType.toLowerCase() == "int") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                            }
                            else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                        }
                        else if (value.Type && value.Type == "Column") {
                            if (value.DataType && value.DataType.toLowerCase() == "int") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                            }
                            else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                        }
                        else if (value.Type && value.Type == "Collection") {
                            templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                        }
                        else if (value.Type && value.Type == "Object") {
                            templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                        }
                        if (typeof displaypropertyname != "undefined" && value.hasOwnProperty(displaypropertyname)) {
                            return {
                                label: value[displaypropertyname],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                            return {
                                label: value[propertyName],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.dictAttributes && value.dictAttributes.hasOwnProperty(propertyName)) {
                            return {
                                label: value.dictAttributes[propertyName],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (value && typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (value) {
                            return {
                                label: value.ID,
                                value: value.ID,
                                logo: templogo
                            };
                        }


                    }), request.term/* extractLast(request.term, this.element[0].selectionStart)*/);
                response(result.slice(0, 50));
                // }
            },
            select: function (event, ui) {
                var control = this;
                var value = "";
                if (event.target.localName == "span") {
                    //this.innerText = this.innerText + ui.item.value.ID;
                    //this.innerText = [this.innerText.slice(0, caretindex), ui.item.value.ID, this.innerText.slice(caretindex)].join('');
                    if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                        value = ui.item.value[propertyName];
                    }
                    else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                        value = ui.item.value.dictAttributes[propertyName];
                    }
                    else if (ui.item.value.ID) {
                        value = ui.item.value.ID;
                    }
                    else {
                        value = ui.item.value;
                    }
                    this.innerText = value;
                    var scope1 = angular.element($(this)).scope();
                    if (scope1) {
                        scope1.$broadcast("UpdateOnClick", controlSelection[0]);
                    }
                }
                else {
                    if (typeof propertyName != "undefined" && ui.item.value.hasOwnProperty(propertyName)) {
                        value = ui.item.value[propertyName];
                    }
                    else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                        value = ui.item.value.dictAttributes[propertyName];
                    }
                    else if (ui.item.value.ID) {
                        value = ui.item.value.ID;
                    }
                    else {
                        value = ui.item.value;
                    }
                    control.value = value;
                }
                $(this).trigger("change");
                if (callBackFunction && typeof callBackFunction === "function") {
                    callBackFunction({ obj: ui.item.value });
                }
                return false;
            },
            close: function () {
                $(".page-header-fixed").css("pointer-events", "auto");
            }
        });
        //data = getIntellisenseRecord($(controlSelection).val(), data, propertyName);
    }
    //else return false;

}
$.ui.autocomplete.prototype._renderItem = function (ul, item) {
    var temp = "";
    if (item.logo && item.logo != "") {
        temp = item.logo + item.label;
    }
    else {
        temp = item.label;
    }

    return $("<li></li>")
        .data("item.autocomplete", item)
        .append("<a>" + temp + "</a>")
        .appendTo(ul);
};
function setSingleLevelAutoCompleteForCodeValues(controlSelection, data, scope, propertyName, displaypropertyname, model, type) {
    if (data && data.length > 0) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName].toLowerCase();
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID.toLowerCase();
            }
            else if (a.toLowerCase) {
                nameA = a.toLowerCase();
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName].toLowerCase();
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID.toLowerCase();
            }
            else if (b.toLowerCase) {
                nameB = b.toLowerCase();
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
            return 0; //default return value (no sorting)
        });
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type"))) {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        options.left = (pos.left) + "px";
                        options.top = (pos.top) + "px";

                    }
                    $("#dvIntellisense > ul").css(options);
                }
                setIntellisensePosition(event);

            },
            source: function (request, response) {

                //response();
                response($.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        if (typeof displaypropertyname != "undefined" && value.hasOwnProperty(displaypropertyname)) {
                            return {
                                label: value[displaypropertyname],
                                value: value,
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                            return {
                                label: value[propertyName],
                                value: value,
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.dictAttributes && value.dictAttributes.hasOwnProperty(propertyName)) {
                            return {
                                label: value.dictAttributes[propertyName],
                                value: value,
                            };
                        }
                        else if (typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value
                            };
                        }
                        else {

                            return {
                                label: value.ID,
                                value: value.ID,
                            };
                        }
                    }), extractLast(request.term, this.element[0].selectionStart)));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                    this.value = ui.item.value[propertyName];
                    if (type == "role") {
                        model.dictAttributes.role = ui.item.value.CodeID;
                    } else if (type == "skill") {
                        model.dictAttributes.skill = ui.item.value.CodeID;
                    }
                    else if (type == "location") {
                        model.dictAttributes.location = ui.item.value.CodeID;
                    } else if (type == "position") {
                        model.dictAttributes.position = ui.item.value.CodeID;
                    } else if (type == "authoritylevel") {
                        model.dictAttributes.authorityLevel = ui.item.value.CodeID;
                    } else if (type == "user") {
                        model.dictAttributes.user = ui.item.value.CodeID;
                    }
                }
                else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                    this.value = ui.item.value.dictAttributes[propertyName];
                }
                else if (ui.item.value.ID) {
                    control.value = ui.item.value.ID;
                }
                else {
                    control.value = ui.item.value;
                }
                $(this).trigger("change");
                return false;
            }
        });
    }

}

function containsMathOpe(text) {
    if (text.contains("+") || text.contains("-") || text.contains("*") || text.contains("/") || text.contains(" ") ||
        text.contains(">") || text.contains("<") || text.contains("%") || text.contains("==") || text.contains("!") ||
        text.contains("=") || text.contains("(")) {
        return true;
    }

    return false;
}
function isMathope(char) {
    if (char == '+' || char == '-' || char == '*' || char == '/' || char == ' ' || char == '\n' || char == '>' || char == '<' ||
        char == '%' || char == '=' || char == '"' || char == '(' || char == ')' || char == ',' || char == '?' || char == ':') {
        return true;
    }

    return false;
}


function getSplitArray(text, caretIndex) {
    var retVal = [];
    //if (containsMathOpe(text)) {
    var txtArr = text.split('');
    var res = [];
    for (var i = caretIndex - 1; i >= 0; i--) {
        if (txtArr[i] != '"' && isMathope(txtArr[i]))
            break;
        res.push(txtArr[i]);
    }

    if (res.length > 0) {
        text = res.reverse().join('');
        retVal = text.split('.');
    }
    //}
    //else {
    //    retVal = text.split('.');
    //}
    return retVal;
}

function split(val) {
    return val.split(".");
}
function extractLast(term, caretIndex) {
    var arr = getSplitArray(term, caretIndex);
    if (arr.length > 0) {
        return arr.pop();
    }
    else {
        return "";
    }
}


function caretPosition(element) {
    var caretOffset = 0;
    var doc = element.ownerDocument || element.document;
    var win = doc.defaultView || doc.parentWindow;
    var sel;
    if (typeof win.getSelection != "undefined") {
        sel = win.getSelection();
        if (sel.rangeCount > 0) {
            var range = win.getSelection().getRangeAt(0);
            var preCaretRange = range.cloneRange();
            preCaretRange.selectNodeContents(element);
            preCaretRange.setEnd(range.endContainer, range.endOffset);
            caretOffset = preCaretRange.toString().length;
        }
    } else if ((sel = doc.selection) && sel.type != "Control") {
        var textRange = sel.createRange();
        var preCaretTextRange = doc.body.createTextRange();
        preCaretTextRange.moveToElementText(element);
        preCaretTextRange.setEndPoint("EndToEnd", textRange);
        caretOffset = preCaretTextRange.text.length;
    }
    return caretOffset;
}

function setMultilevelAutoCompleteForObjectTreeIntellisense(controlSelection, data, propertyName, onSelectionCallback, scope) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // press up or down key    
        return;
    }
    if (data && data.length > 0) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (a.hasOwnProperty(propertyName)) {
                if (a[propertyName] && a[propertyName].toLowerCase) nameA = a[propertyName].toLowerCase();
                else nameA = a[propertyName];
            }
            else if (a.hasOwnProperty("ID")) {
                if (a.ID && a.ID.toLowerCase) nameA = a.ID.toLowerCase();
                else nameA = a.ID;
            }
            else if (a && a.toLowerCase) nameA = a.toLowerCase();

            if (b.hasOwnProperty(propertyName)) {
                if (b[propertyName] && b[propertyName].toLowerCase) nameB = b[propertyName].toLowerCase();
                else nameB = b[propertyName];
            }
            else if (b.hasOwnProperty("ID")) {
                if (b.ID && b.ID.toLowerCase) nameB = b.ID.toLowerCase();
                else nameB = b.ID;
            }
            else if (b && b.toLowerCase) nameB = b.toLowerCase();
            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
            return 0; //default return value (no sorting)
        });
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: "200px",
                        height: 'auto',
                        overflow: 'auto',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea") {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        var windowWidth = $(window).width();
                        if (pos.left + 220 > windowWidth) {
                            var diffinpos = (pos.left + 220) - windowWidth;
                            options.left = (pos.left - (diffinpos)) + "px";
                        } else {
                            options.left = (pos.left) + "px";
                        }
                        options.top = (pos.top) + "px";

                        $("#dvIntellisense > ul").css(options);
                    }
                    else {
                        $("#dvIntellisense > ul").css(options);
                        setIntellisensePosition(event);
                    }
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            source: function (request, response) {

                //response();
                if (data && data.length > 0)
                    response($.ui.autocomplete.filter(
                        $.map(data, function (value, key) {

                            var templogo = "";
                            if (value.Type && value.Type == "Method") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/method.png'/>";
                            }
                            else if (value.Type && value.Type == "Rule") {
                                if (value.RuleType && value.RuleType == "LogicalRule") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/logical_rule.png'/>";

                                }
                                else if (value.RuleType && value.RuleType == "DecisionTable") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/decision_table..png'/>";
                                }
                                else if (value.RuleType && value.RuleType == "ExcelMatrix") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Excel_matrix.png'/>";
                                }

                            }
                            else if (value.Type && value.Type == "Property") {
                                if (value.DataType && value.DataType.toLowerCase() == "int") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                                }
                                else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                                }
                            }
                            else if (value.Type && value.Type == "Column") {
                                if (value.DataType && value.DataType.toLowerCase() == "int") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                                }
                                else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                                }
                            }
                            else if (value.Type && value.Type == "Collection") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.Type && value.Type == "Object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                            if (value.hasOwnProperty(propertyName) && value[propertyName]) {
                                return {
                                    label: value[propertyName],
                                    value: value,
                                    logo: templogo
                                };
                            }
                            else if (value.hasOwnProperty("ID") && value.ID) {
                                return {
                                    label: value.ID,
                                    value: value,
                                    logo: templogo
                                };
                            }
                            else {
                                return {
                                    label: value,
                                    value: value,
                                    logo: templogo

                                };
                            }
                            if (templogo) {
                                return {
                                    label: value,
                                    value: value,
                                    logo: templogo

                                };
                            }
                        }), extractLast(request.term, this.element[0].selectionStart)));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                if (ui.item && ui.item.value && "Description" in ui.item.value && ui.item.value.Description) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Description);
                }
                else if (ui.item && ui.item.value && "Tooltip" in ui.item.value && ui.item.value.Tooltip) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Tooltip);
                }
                else {
                    /* code changes made coz in case of overloaded method , parameters needs to show to differentiate. (Bug 7598)*/
                    var titlevalue = ui.item.label;
                    if (ui.item.value && ui.item.value.Parameters && ui.item.value.Parameters.length > 0) {
                        titlevalue += "("
                        for (var i = 0; i < ui.item.value.Parameters.length; i++) {

                            titlevalue += ui.item.value.Parameters[i].ID;
                            if (ui.item.value.Parameters.length > 1) {
                                titlevalue += ",";
                            }

                        }
                        if (titlevalue.lastIndexOf(",") > -1) {
                            titlevalue = titlevalue.replace(/,\s*$/, "");
                        }
                        titlevalue += ")"
                    }
                    $(".ui-autocomplete > li").attr("title", titlevalue);
                }
                return false;
            },
            select: function (event, ui) {
                var arr = getSplitArray(this.value, this.selectionStart);
                var startPosition = this.selectionStart;
                var arrLen = 0;
                if (arr.length > 0) {
                    // this.value = this.value.substr(0, this.value.lastIndexOf(arr[arr.length - 1]));
                    arrLen = arr[arr.length - 1].length;
                }
                if (this.value == "") {
                    startPosition = 0;
                }
                var selectedtextlength = 0;

                if (ui.item.value[propertyName]) {
                    if (startPosition == 0) {
                        this.value = ui.item.value[propertyName];
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value[propertyName], this.value.slice(startPosition)].join('');
                        // this.value = this.value.substr(0, startPosition) + ui.item.value[propertyName] + this.value.substr(startPosition);
                        //this.value = this.value + ui.item.value[propertyName];
                    }
                    selectedtextlength = ui.item.value[propertyName].length;
                }
                else if (ui.item.value.ID) {
                    if (startPosition == 0) {
                        this.value = ui.item.value.ID;
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value.ID, this.value.slice(startPosition)].join('');
                        //this.value = this.value.substr(0, startPosition) + ui.item.value.ID + this.value.substr(startPosition);
                        //this.value = this.value + ui.item.value.ID;
                    }
                    selectedtextlength = ui.item.value.ID.length;
                }
                else {
                    this.value = this.value + ui.item.value;
                    selectedtextlength = ui.item.value.length;
                }
                this.value = this.value.trim();
                var prop = ui.item.value;
                if (prop.DataType == "TableObjectType" || prop.DataType == "CollctionType" || prop.DataType == "BusObjectType" || prop.DataType == "OtherReferenceType") {
                    if (!prop.HasLoadedProp || prop.HasLoadedProp == undefined) {
                        var txt = arr.join(".");
                        if (txt.indexOf("this.") == 0) {
                            txt = txt.substring(5);
                        }
                        $.connection.hubEntityModel.server.loadObjectTree(prop.ItemType.Name, prop.FullPath, true).done(function (data) {
                            scope.receiveObjectTree(data, prop.FullPath);
                        });
                    }
                }
                if (scope && scope.selectedobject != undefined) {
                    scope.selectedobject = prop;
                }
                $(this).trigger("change");
                //if (onSelectionCallback) {
                //    onSelectionCallback({ prop: prop });
                //}
                if (this.selectionEnd) {
                    this.selectionEnd = startPosition + selectedtextlength;
                }
                return false;
            },
            close: function () {
                controlSelection[0].focus();
                $(".page-header-fixed").css("pointer-events", "auto");
                //$(controlSelection).autocomplete("destroy");
            }
        });
    }
    else {
        if ($(controlSelection).data('ui-autocomplete')) {
            $(controlSelection).autocomplete("destroy");
            $(".page-header-fixed").css("pointer-events", "auto");
        }
    }
}

function generateUUID() {
    var guid;
    $.ajax({
        url: 'api/Login/GetGUID',
        type: 'GET',
        async: false,
        dataType: 'json',
        data: {},
        success: function (data) {
            guid = data;
        },
        error: function (response) {
            var objError = JSON.parse(response.responseText);
            if (objError && objError.Message) {
                var msg = objError.Message;
                if (objError.ExceptionMessage) {
                    msg = msg + "\n" + objError.ExceptionMessage;
                }
                MessageBox("Message", msg);
            }
            else {
                MessageBox("Message", "An error occured while creating a new GUID.");
            }
        }
    });

    return guid;
}

function onBodyClick(event) {
}

//#region Function for Creating New Objects
function GetFormattedTableName(astrTableName) {
    if (astrTableName.length > 4 &&
        astrTableName.toLowerCase().match("^sg") && astrTableName.substring(4, 3) == "_") {
        astrTableName = astrTableName.substring(4);
    }
    var strFinal = "";
    if (astrTableName == undefined || astrTableName == "") {
        return astrTableName;
    }
    var strSplit = astrTableName.split('_');
    angular.forEach(strSplit, function (strInclExcl) {
        if (strInclExcl == undefined || strInclExcl == "") {
            strFinal += strInclExcl;
        }
        else {
            strFinal += strInclExcl.substring(0, 1).toUpperCase();
            strFinal += strInclExcl.substring(1, strInclExcl.length).toLowerCase();
        }
    });

    return strFinal;
}


//#endregion

function onDragOverCommon(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
}
function isValidIdentifier(identifier, excludeNumber, includeKeywords, includeHyphen) {
    if (!identifier)
        return false;
    if (identifier.trim() == "") {
        return false;
    }


    if (!includeKeywords && globalKeywords && globalKeywords.length > 0 && globalKeywords.indexOf(identifier.toLowerCase()) > -1) {
        return false;
    }
    var regex;
    if (excludeNumber) {
        regex = new RegExp("^[_a-zA-Z][_a-zA-Z]*$");
    } else if (includeHyphen) {
        regex = new RegExp("^[-_a-zA-Z][-_a-zA-Z0-9]+$");
    } else {
        regex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
    }
    if (excludeNumber) {
        var alphaNumeric = new RegExp("^[a-zA-Z]*$");
    }
    else if (includeHyphen) {
        var alphaNumeric = new RegExp("^[-a-zA-Z0-9]*$");
    } else {
        var alphaNumeric = new RegExp("^[a-zA-Z0-9]*$");
    }

    if (regex.test(identifier)) {
        var tempString = identifier.replace(/_/g, "");
        if (tempString.trim() != "" && alphaNumeric.test(tempString)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}

function isValidFileID(identifier, IsForm) {
    if (!identifier)
        return false;
    if (identifier.trim() == "") {
        return false;
    }
    var regex = new RegExp("^[a-zA-Z][a-zA-Z0-9-]*$");
    if (IsForm) {
        regex = new RegExp("^[a-zA-Z][a-zA-Z]*$");
    }
    if (regex.test(identifier)) {
        return true;
    }
    else {
        return false;
    }
}


function getWebApplicationName() {
    var webAppName = window.location.pathname.substring(1);
    var index = webAppName.indexOf("/");
    if (index > -1) {
        webAppName = webAppName.substring(0, index);
    }
    return webAppName;
}

//#region getDescendents
function getDescendents(obj, name) {
    var elements = [];
    if (obj.Children != undefined) {
        if (obj.Children.length > 0) {
            for (var index = 0; index < obj.Children.length; index++) {
                if (!name || obj.Children[index].Name === name) {
                    elements.push(obj.Children[index]);
                }
                var childElements = getDescendents(obj.Children[index]);
                for (var i = 0; i < childElements.length; i++) {
                    if (!name || childElements[i].Name === name) {
                        elements.push(childElements[i]);
                    }
                }
            }
        }
    }

    if (obj.Elements != undefined) {
        if (obj.Elements.length > 0) {
            for (var index = 0; index < obj.Elements.length; index++) {
                if (!name || obj.Elements[index].Name === name) {
                    elements.push(obj.Elements[index]);
                }
                var childElements = getDescendents(obj.Elements[index]);
                for (var i = 0; i < childElements.length; i++) {
                    if (!name || childElements[i].Name === name) {
                        elements.push(childElements[i]);
                    }
                }
            }
        }
    }
    return elements;
}
//#endregion

//#region Function for New Rule, New Group
function GetNewStepName(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    while (CheckForDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    }

    return strItemName;
}

function CheckForDuplicateID(strId, objRules) {
    var blnReturn = false;
    if (objRules) {
        blnReturn = objRules.dictAttributes.ID == strId;
        if (!blnReturn) {
            angular.forEach(objRules.Elements, function (item) {
                if (!blnReturn) {
                    blnReturn = CheckForDuplicateID(strId, item);
                    if (blnReturn) {
                        return;
                    }
                }
            });
        }
    }
    return blnReturn;
}

function GetNewQueryName(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    while (CheckForDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    }

    return strItemName;


}
function CheckInitialLoadQueryDuplicateID(strId, objRules) {
    var blnReturn = false;
    if (objRules) {
        if (objRules.dictAttributes && objRules.dictAttributes.ID) {
            blnReturn = objRules.dictAttributes.ID == strId;
        }

        if (!blnReturn && objRules && objRules.length > 0) {
            angular.forEach(objRules, function (item) {
                if (!blnReturn) {
                    blnReturn = CheckInitialLoadQueryDuplicateID(strId, item);
                    if (blnReturn) {
                        return;
                    }
                }
            });
        }
    }
    return blnReturn;
}

function GetInitialLoadQueryID(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    while (CheckInitialLoadQueryDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    }

    return strItemName;


}
//#endregion

function getHtmlFromServer(url) {
    var htmlText;
    var path = [window.location.protocol, "//", window.location.host, "/", getWebApplicationName(), "/", url].join("");

    var successCallBack = function (response) {
        htmlText = response;
    };
    var errorCallBack = function (response) {
        console.log(String.format("Invalid Url: {0}", path));
    };

    $.ajax({
        url: path,
        type: 'GET',
        async: false,
        success: successCallBack,
        error: errorCallBack
    });

    return htmlText;
}

function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}


function getEntityObject(entitylist, entityname, isfirstlevelexpanded) {
    var obj;
    var lst = entitylist.filter(function (x) { return x.ID == entityname; });
    if (lst && lst.length > 0) {
        obj = { ID: lst[0].ID, TableName: lst[0].TableName, BusinessObjectName: lst[0].BusinessObjectName, Attributes: [], IsExpanded: isfirstlevelexpanded, XmlMethods: [] };
        if (lst[0].Attributes && lst[0].Attributes.length > 0) {
            if (isfirstlevelexpanded) {
                createEntityAttributeList(lst[0].Attributes, obj);
                //createEntityXmlMethodList(lst[0].XmlMethods, obj);
            }
        }
    }

    return obj;
}

function createEntityAttributeList(attributes, parent) {
    attributes.sort(sort_by('ID', false, function (a) { return a.toUpperCase(); }));


    angular.forEach(attributes, function (field) {
        function iterateAttr(attr) {
            var val = field.Value.replace('_id', '_value');
            if (attr.Value == val) {
                blnFound = true;
            }
        }
        //in entity tree we do not want to show the attributes ending with _id so the check is added by neha
        if (endsWith(field.Value, "_id")) {
            var blnFound = false;
            angular.forEach(attributes, iterateAttr);
            if (!blnFound) {
                var objField = { ID: field.ID, Value: field.Value, DataType: field.DataType, Entity: field.Entity, Type: field.Type, Attributes: [], XmlMethods: [], objParent: parent };
                parent.Attributes.push(objField);
            }
        }
        else {
            var objField = { ID: field.ID, Value: field.Value, DataType: field.DataType, Entity: field.Entity, Type: field.Type, Attributes: [], XmlMethods: [], objParent: parent };
            parent.Attributes.push(objField);
        }
    });
    //if (parent.Attributes) {
    //    var count = 0;
    //    while (count != 2) {
    //        count++
    //        var tempattr = JSON.stringify(parent.Attributes);
    //        var data = JSON.parse(tempattr);
    //        parent.Attributes = parent.Attributes.concat(data);
    //    }
    //}

}

/*Use of sort_by
For Int field => arrayList.sort(sort_by('price', true, parseInt));
For String field => arrayList.sort(sort_by('ID', function (a) { return a.toUpperCase() }));
*/

function sort_by(field, reverse, primer) {
    var key = primer ?
        function (x) { return primer(x[field]); } :
        function (x) { return x[field]; };

    reverse = !reverse ? 1 : -1;

    return function (a, b) {
        return a = key(a), b = key(b), reverse * ((a > b) - (b > a));
    };
}

function createEntityXmlMethodItemList(items, parent) {
    angular.forEach(items, function (item) {
        var objItem = { ID: item.ID, ItemType: item.ItemType, LoadType: item.LoadType, LoadSource: item.LoadSource, SfwParameters: item.SfwParameters, objParent: parent };
        parent.Items.push(objItem);
    });
}

function getIntellisenseRecord(value, genericlist, propertyName) {
    //Get First 100 records from the list.
    if (genericlist) {
        data = genericlist;
        data = data.sort();
        var lstFilterData = [];
        if (propertyName != "ActiveForm" && propertyName != "VisibleRule" && propertyName != "FileIntellisense") {
            for (var i = 0; i < data.length; i++) {
                //if (data[i].DisplayMessageID.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                if (data[i][propertyName].toLowerCase().indexOf(value.toLowerCase()) > -1) {
                    lstFilterData.push(data[i]);
                }
            }
        }
        else {
            lstFilterData = data;
        }
        lstTop100Msgs = [];
        if (lstFilterData.length > 100) {
            for (var i = 0; i <= 99; i++) {
                lstTop100Msgs.push(lstFilterData[i]);
            }
        }
        else {
            lstTop100Msgs = lstFilterData;
        }
    }
    return lstTop100Msgs;
}

function SetParentForObjTreeChild(objtree) {
    function iterator(item) {
        item.objParent = objtree;
        item.FullPath = GetFullItemPathFromBusObjectTree(item);
        if (item.ChildProperties && item.ChildProperties.length > 0) {
            SetParentForObjTreeChild(item);
        }
    }

    if (objtree) {
        angular.forEach(objtree.ChildProperties, iterator);
        angular.forEach(objtree.lstMethods, iterator);
    }
}

function stopPropogation(event) {
    event.stopPropagation();
}

function GetBaseModel(data) {
    if (data) {
        var objNewObject = {
            prefix: data.prefix, Name: data.Name, Value: data.Value, dictAttributes: {}, Elements: [], Children: [], IsValueInCDATAFormat: data.IsValueInCDATAFormat, IsBaseAppModel: data.IsBaseAppModel, BaseAppModel: GetBaseModel(data.BaseAppModel)
        };


        for (var key in data.dictAttributes) {
            if (data.dictAttributes.hasOwnProperty(key)) {
                objNewObject.dictAttributes[key] = data.dictAttributes[key];
            }
        }


        angular.forEach(data.Elements, function (step) {
            if (step.Name && step.Name === "sfwGridView" && step.prototypemodel) {
                var objDataModel = GetPrototypeDataModel(step.prototypemodel);
                var IsDataElement = false;
                if (step.Elements.length > 1) {

                    for (var i = 0; i < step.Elements.length; i++) {
                        if (step.Elements[i].Name == "Data") {
                            IsDataElement = true;
                            step.Elements[i].Elements = objDataModel;
                            break;
                        }
                    }

                }
                if (!IsDataElement && objDataModel && objDataModel.length > 0) {
                    var objNewData = { dictAttributes: {}, Elements: [], Name: "Data", Value: "", prefix: "" };
                    objNewData.Elements = objDataModel;
                    step.Elements.push(objNewData);
                }
            }
            var model = GetBaseModel(step);
            objNewObject.Elements.push(model);
        });

        angular.forEach(data.Children, function (step) {
            var model = GetBaseModel(step);
            objNewObject.Children.push(model);
        });
    }


    return objNewObject;
}
function GetPrototypeDataModel(data) {
    if (data) {
        var objdataRowsModel = [];
        angular.forEach(data.Elements, function (dataRows) {
            if (dataRows.Elements) {
                var objRows = { dictAttributes: {}, Elements: [], Name: "DataRow", Value: "", prefix: "" };
                angular.forEach(dataRows.Elements, function (dataItem) {
                    if (dataItem.Elements) {
                        angular.forEach(dataItem.Elements, function (dataCol) {
                            objRows.Elements.push(dataCol);
                        });
                    }
                });
                objdataRowsModel.push(objRows);
            }
        });
        return objdataRowsModel;
    }
}
function getPropertyValue(obj, propname) {
    if (propname) {
        if (propname.contains('.')) {
            while (propname.contains('.')) {
                var strProp = propname.substring(0, propname.indexOf('.'));
                propname = propname.substring(propname.indexOf('.') + 1);
                if (obj && obj.hasOwnProperty(strProp)) {
                    obj = obj[strProp];
                }
            }
        }

        if (obj && obj.hasOwnProperty(propname)) {
            return obj[propname];
        }
    }
    return "";
}


function setPropertyValue(obj, propname, value) {
    if (propname && obj) {
        if (propname.contains('.')) {
            while (propname.contains('.')) {
                var strProp = propname.substring(0, propname.indexOf('.'));
                propname = propname.substring(propname.indexOf('.') + 1);
                if (obj.hasOwnProperty(strProp) && obj[strProp] !== null && typeof obj[strProp] === "object") {
                    obj = obj[strProp];
                }
            }
        }
        obj[propname] = value;
    }
}

function selectBuildResultErrors() {

    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    var lobjSpliter = $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[id="divbottompanel"]').show();
    if (lobjSpliter && lobjSpliter.refresh) {
        lobjSpliter.refresh();
    }
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").addClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").removeClass("selected");
}
function selectBuildResultWarnings() {
    $('div[id="divbottompanel"]').show();
    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").addClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").removeClass("selected");
    showFooter();
}
function selectBuildResultMessages() {
    $('div[id="divbottompanel"]').show();
    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").addClass("selected");
}
function onClickBuildResultsDiv() {
    showBuildResults();
    toggleBuildResults(null, true);
}
function showBuildResults() {
    showFooter();
    $("div[class='footer-main-links'] #liBuildResults").show();
    selectBuildResultErrors();
    $('div[id="divBuildResultsbutton"]').hide();
}

function CloseBuildResultsButton(event) {
    $('div[id="divBuildResultsbutton"]').hide();
    if (event) {
        event.stopPropagation();
    }
}

function showFooter() {
    $(".footer").show();
}

function toggleBuildResults(event, showOnly) {
    onBodyClick();
    if ($('div[class="footer-slide-up"][id="dvBuildResults"]').is(':visible')) {
        if (!showOnly) {
            $(".footer-main-links #liBuildResults a").removeClass("selected");
            $('div[class="footer-slide-up"][id="dvBuildResults"]').hide("slide", {
                direction: "down"
            });
        }
    }
    else {
        $(".footer-main-links #liBuildResults a").addClass("selected");
        $('div[class="footer-slide-up"][id="dvBuildResults"]').show("slide", {
            direction: "down"
        });
        hideOtherFooters('BuildResults');
    }
    if (event) {
        event.stopPropagation();
    }
}

function hideBuildResults() {
    closeBuildResults();
    $("div[class='footer-main-links'] #liBuildResults").hide();
    if ($("div[class='footer-main-links'] ul").find("li:visible").length == 0) {
        hideFooter();
    }
    $('div[id="divBuildResultsbutton"]').show();
}

function closeBuildResults() {
    if ($('div[class="footer-slide-up"][id="dvBuildResults"]').is(':visible')) {
        $(".footer-main-links #liBuildResults a").removeClass("selected");
        $('div[class="footer-slide-up"][id="dvBuildResults"]').hide("slide", {
            direction: "down"
        });
    }
    $("#main-wrapper").removeClass("splitter_panel");
    $("#main-wrapper").removeClass("spliter");
    $('div[id="divbottompanel"]').hide();
    $('#main-wrapper .a').height('100%');
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});

}

function hideFooter() {
    $(".footer").hide();
}

function hideOtherFooters(currentTab) {
    if (currentTab != "BuildResults") {
        closeBuildResults();
    }
    if (currentTab != "FindResults") {
        closeFindResults();
    }
}

function closeFindResults() {
    if ($('div[class="footer-slide-up"][id="dvFindResults"]').is(':visible')) {
        $(".footer-main-links #liFindResults a").removeClass("selected");
        $('div[class="footer-slide-up"][id="dvFindResults"]').hide("slide", {
            direction: "down"
        });
    }
}


(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof module === 'object' && module.exports) {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function ($) {
    'use strict';
    var caretClass = 'textarea-helper-caret'
        , dataKey = 'textarea-helper'

        // Styles that could influence size of the mirrored element.
        , mirrorStyles = [
            // Box Styles.
            'box-sizing', 'height', 'width', 'padding-bottom'
            , 'padding-left', 'padding-right', 'padding-top'

            // Font stuff.
            , 'font-family', 'font-size', 'font-style'
            , 'font-variant', 'font-weight'

            // Spacing etc.
            , 'word-spacing', 'letter-spacing', 'line-height'
            , 'text-decoration', 'text-indent', 'text-transform'

            // The direction.
            , 'direction'
        ];

    var TextareaHelper = function (elem) {
        if (elem.nodeName.toLowerCase() !== 'textarea' && elem.nodeName.toLowerCase() !== 'input') return;
        this.$text = $(elem);
        this.$mirror = $('<div/>').css({
            'position': 'absolute'
            , 'overflow': 'auto'
            , 'white-space': 'pre-wrap'
            , 'word-wrap': 'break-word'
            , 'top': 0
            , 'left': -9999
        }).insertAfter(this.$text);
    };

    (function () {
        this.update = function () {

            // Copy styles.
            var styles = {};
            for (var i = 0, style; style = mirrorStyles[i]; i++) {
                styles[style] = this.$text.css(style);
            }
            this.$mirror.css(styles).empty();

            // Update content and insert caret.
            var caretPos = this.getOriginalCaretPos()
                , str = this.$text.val()
                , pre = document.createTextNode(str.substring(0, caretPos))
                , post = document.createTextNode(str.substring(caretPos))
                , $car = $('<span/>').addClass(caretClass).css('position', 'absolute').html('&nbsp;');
            this.$mirror.append(pre, $car, post)
                .scrollTop(this.$text.scrollTop());
        };

        this.destroy = function () {
            this.$mirror.remove();
            this.$text.removeData(dataKey);
            return null;
        };

        this.caretPos = function () {
            this.update();
            var $caret = this.$mirror.find('.' + caretClass)
                , pos = $caret.position();
            if (this.$text.css('direction') === 'rtl') {
                pos.right = this.$mirror.innerWidth() - pos.left - $caret.width();
                pos.left = 'auto';
            }

            return pos;
        };
        this.caretPosAbs = function () {
            var absPos = this.$text[0].getBoundingClientRect();
            var leftBorderWidth = this.$text.css("border-left-width");
            var topBorderWidth = this.$text.css("border-top-width");
            var leftPadding = this.$text.css("padding-left");
            var topPadding = this.$text.css("padding-top");
            var carPos = this.caretPos();

            if (!absPos) {
                absPos = { left: 0, top: 0 };
            }
            else {
                if (!absPos.left) {
                    absPos.left = 0;
                }
                if (!absPos.top) {
                    absPos.top = 0;
                }
            }

            if (!leftBorderWidth) {
                leftBorderWidth = 0;
            }
            if (!topBorderWidth) {
                topBorderWidth = 0;
            }
            if (!leftPadding) {
                leftPadding = 0;
            }
            if (!topPadding) {
                topPadding = 0;
            }

            var actPos = {
                left: parseFloat(absPos.left) + parseFloat(leftBorderWidth) + parseFloat(leftPadding) + parseFloat(carPos.left),
                top: parseFloat(absPos.top) + parseFloat(topBorderWidth) + parseFloat(topPadding) + parseFloat(carPos.top) + 15
            };
            return actPos;
        };
        this.height = function () {
            this.update();
            this.$mirror.css('height', '');
            return this.$mirror.height();
        };
        // XBrowser caret position
        // Adapted from http://stackoverflow.com/questions/263743/how-to-get-caret-position-in-textarea
        this.getOriginalCaretPos = function () {
            var text = this.$text[0];
            if (text.selectionStart) {
                return text.selectionStart;
            } else if (document.selection) {
                text.focus();
                var r = document.selection.createRange();
                if (r == null) {
                    return 0;
                }
                var re = text.createTextRange()
                    , rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);
                return rc.text.length;
            }
            return 0;
        };

    }).call(TextareaHelper.prototype);

    $.fn.textareaHelper = function (method) {
        this.each(function () {
            var $this = $(this)
                , instance = $this.data(dataKey);
            if (!instance) {
                instance = new TextareaHelper(this);
                $this.data(dataKey, instance);
            }
        });
        if (method) {
            var instance = this.first().data(dataKey);
            return instance[method]();
        } else {
            return this;
        }
    };

}));

$.fn.scrollTo = function (target, options, callback) {
    if (typeof options == 'function' && arguments.length == 2) { callback = options; options = target; }
    var settings = $.extend({
        scrollTarget: target,
        offsetTop: 100,
        offsetLeft: 100,
        duration: 300,
        easing: 'swing'
    }, options);
    return this.each(function () {
        var scrollPane = $(this);
        var scrollTarget = (typeof settings.scrollTarget == "number") ? settings.scrollTarget : $(settings.scrollTarget);
        var scrollY = 0;
        if ((typeof scrollTarget == "number")) {
            scrollY = scrollTarget;
        }
        else {
            if (scrollTarget.offset()) {
                scrollY = scrollTarget.offset().top + scrollPane.scrollTop() - parseInt(settings.offsetTop);
            }
            else {
                scrollY = scrollPane.scrollTop() - parseInt(settings.offsetTop);
            }
        }
        if (scrollY < scrollPane[0].scrollHeight) {
            scrollY -= 150;
        }

        scrollPane.animate({ scrollTop: scrollY }, parseInt(settings.duration), settings.easing, function () {
            if (typeof callback == 'function') { callback.call(this); }
        });
        var scrollX = 0;
        if (settings.offsetLeft > 0) {
            if ((typeof scrollTarget == "number")) {
                scrollX = scrollTarget;
            }
            else {
                if (scrollTarget.offset()) {
                    scrollX = scrollTarget.offset().left + scrollPane.scrollLeft() - parseInt(settings.offsetLeft);
                }
                else {
                    scrollX = scrollPane.scrollLeft() - parseInt(settings.offsetLeft);
                }
            }
        }

        if (scrollX < scrollPane[0].scrollWidth) {
            scrollX -= 150;
        }

        scrollPane.animate({ scrollLeft: scrollX }, parseInt(settings.duration), settings.easing, function () {
            if (typeof callback == 'function') { callback.call(this); }
        });

    });
};

function getArgumentDataTypePrefix(datatype) {
    var prefiex = "";
    if (datatype && datatype.trim() != "") {
        switch (datatype) {
            case "bool":
            case "boolean":
            case "Boolean":
                prefiex = "abln";
                break;
            case "datetime":
            case "DateTime":
                prefiex = "adt";
                break;
            case "decimal":
            case "Decimal":
                prefiex = "adcml";
                break;
            case "double":
            case "Double":
                prefiex = "adbl";
                break;
            case "float":
            case "Float":
                prefiex = "aflt";
                break;
            case "int":
            case "Int32":
            case "long":
            case "Int64":
            case "short":
            case "Int16":
                prefiex = "aint";
                break;
            case "string":
            case "String":
                prefiex = "astr";
                break;
            case "Collection":
                prefiex = "aclb";
                break;
            case "CDOCollection":
                prefiex = "aclb";
                break;
            case "Object":
                prefiex = "aobj";
                break;
            case "List":
                prefiex = "alst";
                break;
        }
    }
    return prefiex;
}
function GetDataTypePrefix(datatype) {
    var prefiex="";
    if (datatype) {
        switch (datatype) {
            case "bool":
                prefiex = "ibln";
                break;
            case "datetime":
                prefiex = "idt";
                break;
            case "decimal":
                prefiex = "idec";
                break;
            case "double":
                prefiex = "idbl";
                break;
            case "float":
                prefiex = "iflt";
                break;
            case "int":
                prefiex = "iint";
                break;
            case "long":
                prefiex = "ilong";
                break;
            case "short":
                prefiex = "ishrt";
                break;
            case "string":
                prefiex = "istr";
                break;
            case "Collection":
                prefiex = "iclb";
                break;
            case "CDOCollection":
                prefiex = "iclb";
                break;
            case "Object":
                prefiex = "obj";
                break;
            case "List":
                prefiex = "lst";
                break;
        }
    }
    return prefiex;
}

function makeStringToCamelCase(astrFieldName) {

    var strCtrlID = "";
    var strSep = "~`!@#$%^&*_-+=[{()}]|:;<,.>?/.";

    var blnCapsNext = true;
    for (var i = 0; i < astrFieldName.length; i++) {
        if (strSep.contains("" + astrFieldName[i]))
            blnCapsNext = true;
        else {
            strCtrlID += blnCapsNext ? astrFieldName.toUpperCase()[i] : astrFieldName[i];
            if ("" + astrFieldName[i] == " ") {
                blnCapsNext = true;
            }
            else {
                blnCapsNext = false;
            }
        }
    }

    if (strCtrlID.toLowerCase().match("^iclb") || strCtrlID.toLowerCase().match("^icol") || strCtrlID.toLowerCase().match("^ibus"))
        strCtrlID = strCtrlID.substring(4);

    return strCtrlID;
}




function codevaluesIDTextChanged(event) {
    var input = $(event.target);
    var scope = angular.element(event.target).scope();
    if (!scope.codeid) {
        scope.codevalueslist = [];
    }
    if (!scope.codevalueslist) {
        $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
    }
    if (scope.codevalueslist && scope.codevalueslist.length > 0) {
        var codeValuesList = [];
        var value = input.val();
        var data = scope.codevalueslist.sort();
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        } else {
            if (data && data.length > 100) {
                for (var i = 0; i < data.length; i++) {
                    if (codeValuesList.length < 100) {
                        if (data[i].CodeValueDescription.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                            codeValuesList.push(data[i]);
                        }
                    } else {
                        break;
                    }
                }
            } else {
                codeValuesList = data;
            }
            setSingleLevelAutoComplete(input, codeValuesList, scope, "CodeValue", "CodeValueDescription");
        }
    }
    else {
        setSingleLevelAutoComplete(input, [], scope, "CodeValue", "CodeValueDescription");
    }
}

//Design-Source Common Function
function getLineNumber(txtarea) {
    var LineNumber; //Current Line number
    var lines; //Total number of lines
    var line;  //Selected Line text content
    var lineno = 1;

    LineNumber = txtarea.val().substr(0, txtarea[0].selectionStart).split("\n").length;
    lines = txtarea.val().split('\n');
    line = lines[LineNumber - 1];

    if (line && ((line.trim().contains("<") && !line.trim().startsWith("<") && !line.trim().contains("</")) || (line.trim().contains("</") && line.trim().endsWith(">")) || (line.trim().contains("></") && line.trim().contains("<") && line[line.trim().indexOf("<") + 1] != "/"))) {
        lineno = LineNumber;
    }
    else if (line && ((!line.trim().startsWith("<") || line.trim().startsWith("</") || !line.trim().startsWith("/") || !line.trim().contains("<")))) {
        while (line != undefined) {
            line = lines[LineNumber - 1];
            LineNumber--;
            if (line && line.trim().startsWith("<")) {
                LineNumber++;
                lineno = LineNumber;
                break;
            }
        }
    }

    return lineno;
}

function scrollTextArea(txtArea, line, text) {
    var noOfRows = txtArea[0].value.split("\n");
    var lineHeight = txtArea[0].scrollHeight / noOfRows.length;
    txtArea[0].scrollTop = (line - 1) * lineHeight;

    // for scroll left
    if (line && text) {
        var lineWidth = txtArea[0].scrollWidth / 3;
        var noCharParsed = noOfRows[line - 1].indexOf(text);
        txtArea[0].scrollLeft = (noCharParsed * 4.4) + text.length;
    }
}

function selectTextareaLine(txtArea, lineNum) {
    lineNum--; // array starts at 0
    var lines = txtArea[0].value.split("\n");

    // calculate start/end
    var startPos = 0, endPos = txtArea[0].value.length;
    for (var x = 0; x < lines.length; x++) {
        if (x == lineNum) {
            break;
        }
        startPos += (lines[x].length + 1);
    }

    var endPos = lines[lineNum].length + startPos;

    if (typeof (txtArea[0].selectionStart) != undefined) {
        txtArea[0].selectionStart = startPos;
        txtArea.focus();
        txtArea[0].selectionEnd = endPos;
        //return true;
    }
    // return false;
}

var FindDeepNode = function (objParentElements, selectedItem) {
    if (objParentElements) {
        angular.forEach(objParentElements.Elements, function (item) {
            item.ParentVM = objParentElements;
            if (item == selectedItem) {
                return selectedItem;
            }
            else if (item.Elements && item.Elements.length > 0) {
                selectedItem = FindDeepNode(item, selectedItem);
                return selectedItem;
            }
            else if (item.Children && item.Children.length > 0) {
                selectedChildItem = FindDeepNodeChildren(item, selectedItem);
                if (selectedChildItem == selectedItem) {
                    return selectedItem;
                }
            }
        });
    }
    return selectedItem;
};

var FindDeepNodeChildren = function (item, selectedItem) {
    angular.forEach(item.Children, function (obj) {
        obj.ParentVM = item;
        if (obj == selectedItem) {
            return obj;
        }
    });
};

var getPathSource = function (sObj, indexPath) {
    while (sObj.ParentVM) {
        if (sObj.ParentVM.Elements.length > 0) {
            indexPath.push(sObj.ParentVM.Elements.indexOf(sObj));
        }
        else if (sObj.ParentVM.Children.length > 0) {
            indexPath.push(sObj.ParentVM.Children.indexOf(sObj));
        }
        sObj = sObj.ParentVM;
    }
    return indexPath;
};

var FindNodeHierarchy = function (objParentElements, index) {
    if (objParentElements && objParentElements.Elements) {
        var newObj = objParentElements.Elements[index];
        if (newObj == undefined) {
            newObj = objParentElements.Elements[index - 1];
        }
        if (newObj) {
            newObj.ParentVM = objParentElements;
        }
        return newObj;
    }
};

var FindChidrenHierarchy = function (objParentElements, index) {
    return objParentElements.Children[index];
};

var sourceChanged = function () {
    var scope = getCurrentFileScope();
    if (scope && scope.sourceChanged) {
        scope.sourceChanged();
    }
};

function setEntityQueryIntellisense(controlSelection, data, scope, propertyName) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // when pressed up or down key    
        return;
    }
    if (propertyName == "") propertyName = undefined;
    if (data && data.length > 0) {
        if (typeof data[0].ID == "undefined") data = data.sort();
        else {
            if (typeof propertyName != "undefined") {
                data = data.sort(function (a, b) {
                    var nameA = a[propertyName].toLowerCase(), nameB = b[propertyName].toLowerCase();
                    //sort string ascending
                    if (nameA < nameB) return -1;
                    if (nameA > nameB) return 1;
                    return 0; //default return value (no sorting)
                });
            }
            else {
                data = data.sort(function (a, b) {
                    if (a.ID && b.ID) {
                        var nameA = a.ID.toLowerCase(), nameB = b.ID.toLowerCase();
                        //sort string ascending
                        if (nameA < nameB) return -1;
                        if (nameA > nameB) return 1;
                    }
                    return 0; //default return value (no sorting)
                });
            }
        }
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var pos = controlSelection.textareaHelper('caretPosAbs');
                    $("#dvIntellisense > ul").css({
                        left: (pos.left) + "px",
                        //top: (pos.top) + "px",
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    });
                    setIntellisensePosition(event);
                }
                else if (controlSelection[0].localName == "span") {
                    var distanceToTop = this.getBoundingClientRect().top;
                    var distanceToleft = this.getBoundingClientRect().left;
                    var currentSpanWidth = this.getBoundingClientRect().width;
                    var currentSpanHeight = this.getBoundingClientRect().height;
                    var windowsHeight = $(window).height();
                    if (distanceToTop + 300 > windowsHeight) {
                        distanceToTop -= 300;
                    }
                    else {
                        distanceToTop += currentSpanHeight;
                    }
                    $("#dvIntellisense > ul").css({
                        left: (distanceToleft) + "px",
                        top: (distanceToTop) + "px",
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px"
                    });
                    var vrCurrentPopupHeight;
                    for (var i = 0; i < $("#dvIntellisense > ul").length; i++) {
                        if ($("#dvIntellisense > ul")[i].offsetHeight != 0) {
                            var vrCurrentPopupHeight = $("#dvIntellisense > ul")[i].offsetHeight;
                        }
                    }
                    if ((vrCurrentPopupHeight < 300) && (distanceToTop < this.getBoundingClientRect().top)) {
                        if ((this.getBoundingClientRect().top + this.getBoundingClientRect().height + vrCurrentPopupHeight) > windowsHeight) {
                            distanceToTop = this.getBoundingClientRect().top - vrCurrentPopupHeight;
                        }
                        else {
                            distanceToTop = this.getBoundingClientRect().top + this.getBoundingClientRect().height;
                        }
                        $("#dvIntellisense > ul").css({
                            left: (distanceToleft) + "px",
                            top: (distanceToTop) + "px",
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px"
                        });
                    }
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            source: function (request, response) {
                var result = $.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        if (typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value
                            };
                        }
                        else {
                            if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                                return {
                                    label: value[propertyName],
                                    value: value,
                                };
                            }
                            else {
                                return {
                                    label: value.ID,
                                    value: value,
                                };
                            }
                        }
                    }), extractLast(request.term, this.element[0].selectionStart));
                response(result.slice(0, 100));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                if (ui.item.value && ui.item.value.hasOwnProperty("Tooltip") && ui.item.value.Tooltip) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Tooltip);
                } else {
                    $(".ui-autocomplete > li").attr("title", ui.item.label);
                }
                return false;
            },
            select: function (event, ui) {
                var arr;
                var caretindex = 0;
                var startPosition = 0;
                var arrLen = 0;
                if (event.target.localName == "input") {
                    arr = getSplitArray(this.value, this.selectionStart);
                    startPosition = this.selectionStart;
                }
                else if (event.target.localName == "span") {
                    caretindex = caretPosition(this);
                    arr = getSplitArray(this.innerText, caretindex);
                    startPosition = caretindex;
                }

                if (arr.length > 0) {
                    if (event.target.localName == "input") {
                        //this.value = this.value.substr(0, this.value.lastIndexOf(arr[arr.length - 1]));
                        arrLen = arr[arr.length - 1].length;
                    }
                    else if (event.target.localName == "span") {
                        var index = caretindex - arr[arr.length - 1].length;
                        if (arr[arr.length - 1].length > 0) {
                            caretindex = index;
                        }
                        var splittedarray = arr[arr.length - 1].split('');
                        var totallength = index + splittedarray.length;
                        for (var i = index; i < totallength; i++) {
                            this.innerText = this.innerText.slice(0, index) + this.innerText.slice(index + 1, this.innerText.length);
                        }
                        //this.innerText = this.innerText.substr(0, this.innerText.lastIndexOf(arr[arr.length - 1]))
                    }
                }
                if (!(this.value)) {
                    startPosition = 0;
                }
                if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                    //this.value = this.value + ui.item.value[propertyName];
                    if (startPosition == 0) {
                        this.value = ui.item.value[propertyName];
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value[propertyName], this.value.slice(startPosition)].join('');
                    }
                }
                else if (ui.item.value.ID) {
                    if (event.target.localName == "input") {
                        // this.value = this.value + ui.item.value.ID;
                        if (startPosition == 0) {
                            this.value = ui.item.value.ID;
                        } else {
                            this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value.ID, this.value.slice(startPosition)].join('');
                        }
                    }
                    else if (event.target.localName == "span") {
                        this.innerText = [this.innerText.slice(0, caretindex), ui.item.value.ID, this.innerText.slice(caretindex)].join('');
                    }
                    if (scope && scope.items && scope.items.Name && (scope.items.Name == "calllogicalrule" || scope.items.Name == "calldecisiontable" || scope.items.Name == "callexcelmatrix" || scope.items.Name == "method" || scope.items.Name == "query")) {
                        var parameters = scope.items.Elements.filter(function (x) { return x.Name == "parameters"; });
                        if (ui.item.value.RuleType || ui.item.value.QueryType || ui.item.value.ReturnType) {
                            if (ui.item.value.Parameters) {
                                var parametersModel = null;
                                if (parameters.length > 0) {
                                    parametersModel = parameters[0];
                                }
                                else {
                                    parametersModel = {
                                        Name: "parameters", Value: "", dictAttributes: {}, Elements: [], Children: []
                                    };
                                }
                                scope.$root.UndRedoBulkOp("Start");
                                //Removing unwanted parameters.
                                if (parametersModel.Elements.length > 0) {
                                    scope.$apply(function () {
                                        parametersModel.Elements = parametersModel.Elements.filter(function (x) { return ui.item.value.Parameters.some(function (element) { return element.ID == x.dictAttributes.ID; }); });
                                    });
                                }
                                //Adding or updating existing parameters
                                for (var index in ui.item.value.Parameters) {
                                    var parameterModel = parametersModel.Elements.filter(function (element) { return element.dictAttributes.ID == ui.item.value.Parameters[index].ID; });
                                    if (parameterModel.length > 0) {

                                        scope.$apply(function () {
                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwDataType, parameterModel[0].dictAttributes, "sfwDataType", ui.item.value.Parameters[index].DataType);

                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwDirection, parameterModel[0].dictAttributes, "sfwDirection", ui.item.value.Parameters[index].Direction);

                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwEntity, parameterModel[0].dictAttributes, "sfwEntity", ui.item.value.Parameters[index].Entity);
                                        });
                                    }
                                    else {
                                        scope.$apply(function () {
                                            var newparam = { Name: "parameter", Value: "", dictAttributes: { ID: ui.item.value.Parameters[index].ID, sfwDataType: ui.item.value.Parameters[index].DataType, sfwDirection: ui.item.value.Parameters[index].Direction, sfwEntity: ui.item.value.Parameters[index].Entity, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                                            scope.$root.PushItem(newparam, parametersModel.Elements);
                                        });
                                    }
                                }

                                if (parameters.length == 0) {
                                    scope.$apply(function () {
                                        scope.$root.PushItem(parametersModel, scope.items.Elements);
                                    });
                                }
                                scope.$root.UndRedoBulkOp("End");
                            }
                        }
                        else {
                            if (parameters.length > 0) {
                                scope.$apply(function () {
                                    scope.$root.DeleteItem(scope.items.Elements[scope.items.Elements.indexOf(parameters[0])], scope.items.Elements);
                                });
                            }
                        }
                    }
                } else {
                    this.value = this.value + ui.item.value;
                    //this.innerText = this.value + ui.item.value;
                }
                var scope1 = angular.element($(this)).scope();
                if (scope1) scope1.$broadcast("UpdateOnClick", controlSelection[0]);
                if (this.childNodes[0] != undefined) {
                    var doc = this.ownerDocument || this.document;
                    var win = doc.defaultView || doc.parentWindow;
                    var sel;

                    var position = caretindex + ui.item.value.ID.length;
                    var range = document.createRange();
                    range.setStart(this.childNodes[0], position);
                    range.collapse(true);
                    if (typeof win.getSelection != "undefined") {
                        sel = win.getSelection();
                        sel.removeAllRanges();
                        sel.addRange(range);
                    } else if ((sel = doc.selection) && sel.type != "Control") {
                        sel.removeAllRanges();
                        sel.addRange(range);
                    }
                }
                else $(this).trigger("change");
                return false;
            },
            close: function () {
                $(".page-header-fixed").css("pointer-events", "auto");
            }
        });
    }
    else {
        if ($(controlSelection).data('ui-autocomplete')) {
            $(controlSelection).autocomplete("destroy");
        }
    }
}

function preventDefaultSaveUndoRedo(e) {
    if (e.ctrlKey && (e.keyCode == 90 || e.keyCode == 89 || e.keyCode == 83)) {
        e.preventDefault();
    }
}

function getDataType(objectDataType, dataTypeName) {
    var retVal = "";
    if (objectDataType == "CollctionType") {
        retVal = "Collection";
    }
    else if (objectDataType == "List") {
        retVal = "List";
    }
    else if (objectDataType == "BusObjectType" || objectDataType == "OtherReferenceType" || objectDataType == "TableObjectType") {
        retVal = "Object";
    }
    else
        if (dataTypeName != undefined && dataTypeName != "" && dataTypeName != null) {
            if (dataTypeName.toLowerCase() == "int32") {
                retVal = "int";
            }
            else if (dataTypeName.toLowerCase() == "int16") {
                retVal = "short";
            }
            else if (dataTypeName.toLowerCase() == "int64") {
                retVal = "long";
            }
            else if (dataTypeName.toLowerCase() == "single") {
                retVal = "float";
            }
            else if (dataTypeName.toLowerCase() == "boolean") {
                retVal = "bool";
            }
            else {
                retVal = dataTypeName.toLowerCase();
            }
        }
    return retVal;
}

function setIntellisensePosition(event) {
    var $input = $(event.target);
    if ($input.data('ui-autocomplete')) {
        var $results = $input.autocomplete("widget");
        //top = $results.position().top,
        left = $results.position().left,
            windowWidth = $(window).width(),
            height = $results.height(),
            inputHeight = $input.height(),
            windowsHeight = $(window).height();
        if (windowsHeight < $results.position().top + height + inputHeight) {
            newTop = $results.position().top - height - inputHeight - 10;
            $results.css("top", newTop + "px");
        }
        var width = $results.width();
        if (left + width > windowWidth) {
            var diff = (left + width) - windowWidth;
            newleft = left - (diff + 15);
            $results.css("left", newleft + "px");
        }
    }
    else {
        event.stopPropagation();
    }
}


function sortNumbersBasedOnproperty(lstobj, text, displaypropertyname) {
    lstobj = lstobj.sort(function (a, b) {
        var nameA;
        var nameB;
        if (a && b) {
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname];
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName];
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName];
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID;
            }
            else if (a) {
                nameA = a;
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname];
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName];
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName];
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID;
            }
            else if (b) {
                nameB = b;
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
        }
        return 0; //default return value (no sorting)
    });

}

function sortListBasedOnproperty(lstobj, text, displaypropertyname) {
    lstobj = lstobj.sort(function (a, b) {
        var nameA;
        var nameB;
        if (a && b) {
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName].toLowerCase();
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID.toLowerCase();
            }
            else if (a.toLowerCase) {
                nameA = a.toLowerCase();
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName].toLowerCase();
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID.toLowerCase();
            }
            else if (b.toLowerCase) {
                nameB = b.toLowerCase();
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
        }
        return 0; //default return value (no sorting)
    });

}

function getRemovedStringStartingSpace(strValue) {
    var strNewValue = "";
    var arrValue = strValue.split(" ");
    for (var i = 0; i < arrValue.length; i++) {
        if (arrValue[i].trim() == "") {
            arrValue.splice(i, 1);
            i--;
        } else {
            break;
        }
    }
    var strNewValue = arrValue.join('');
    return strNewValue;
}

var lstEntityTreeFieldData = null;
function onEntityTreeDragStart(e) {
    dragDropData = null;
    dragDropDataObject = null;
    //e.dataTransfer.effectAllowed = 'move';
    var ID = $(e.currentTarget).attr("drag-id");
    var DisplayName = $(e.currentTarget).attr("drag-display-name");
    var Datatype = $(e.currentTarget).attr("drag-datatype");
    var dragObject = $(e.currentTarget).attr("drag-object");
    var dragfieldtype = $(e.currentTarget).attr("drag-fieldtype");
    dragObject = JSON.parse(dragObject);
    var isparentTypeCollection = $(e.currentTarget).attr("drag-boolparenttype");
    var lookupfieldQueryId = $(e.currentTarget).attr("drag-fieldquery");
    var obj = [ID, DisplayName, Datatype, dragObject, isparentTypeCollection, dragfieldtype, lookupfieldQueryId];
    lstEntityTreeFieldData = obj;
    var strobj = JSON.stringify(obj);
    e.dataTransfer.setData("Text", "");
}

function onEntityFieldDragOver(e) {
    if (e) {
        var scp = angular.element($(e.target)).scope();
        if (e.preventDefault) {
            e.preventDefault();
        }
        if (e.stopPropagation) {
            e.stopPropagation();
        }
        if (scp && scp.candrop != undefined && scp.candrop == false) {
            return false;
        }
    }
}

function SetCodeIDDescriptionToList(lst) {
    if (lst && lst.length > 0) {
        for (var i = 0; i < lst.length; i++) {
            lst[i].CodeIDDescription = lst[i].CodeID + " - " + lst[i].Description;
        }
    }
}

function onEntityFieldDropInDirective(e) {
    var strData = e.dataTransfer.getData("Text");
    if (e.stopPropagation) {
        e.stopPropagation();
    }
    e.preventDefault();
    if (strData == "" && lstEntityTreeFieldData != null) {
        var obj = lstEntityTreeFieldData;//JSON.parse(strData);
        var Id = obj[0];
        var DisplayName = obj[1];
        var DataType = obj[2];
        var scp = angular.element($(e.target)).scope();
        if (scp && (scp.candrop == undefined || scp.candrop)) {
            var entityFullPath = "";
            if (DisplayName != "") {
                entityFullPath = DisplayName + "." + Id;
            } else {
                entityFullPath = Id;
            }
            if (obj[3] && obj[3].Type == "Description") {
                DataType = "Description";
            }
            if (DataType && DataType != "Object" && DataType != "Collection" && DataType != "List" && DataType != "CDOCollection" && scp.model && scp.model.Name != "sfwTableBookMark" && scp.model.Name != "sfwChildTemplateBookmark") {
                if (scp.model) {
                    scp.$evalAsync(function () {
                        if (scp.model.dictAttributes) {
                            if (DataType != "Description") {
                                if (scp.setcolumndatatype) {

                                    scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwColumnDataType, scp.model.dictAttributes, "sfwColumnDataType", DataType);
                                }
                                else {
                                    scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwDataType, scp.model.dictAttributes, "sfwDataType", DataType);
                                }
                            }
                            scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                        } else {
                            scp.$root.EditPropertyValue(scp.model.TrackingID, scp.model, "TrackingID", entityFullPath);
                        }

                    });
                }
            }
            else if (scp.model && scp.model.Name == "sfwChildTemplateBookmark") {
                if (scp.model.dictAttributes && scp.model.dictAttributes.sfwChildTemplateType == "Collection") {
                    if (DataType == "Collection" || DataType == "CDOCollection" || DataType == "List") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    }
                    else {
                        MessageBox("Message", "Field Type has to be 'Collection' or 'List' ");
                    }
                } else if (scp.model.dictAttributes && scp.model.dictAttributes.sfwChildTemplateType == "Entity") {
                    if (DataType == "Object") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    }
                    else {
                        MessageBox("Message", "Field Type has to be 'Object'");
                    }
                }
                else {
                    MessageBox("Message", "Select Object Type for template.");
                }
            }
            else {
                if (scp.model && scp.model.Name == "sfwTableBookMark") {
                    if (DataType == "Collection") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    } else {
                        MessageBox("Message", "Object/Field Cannot be added");
                    }
                } else {
                    MessageBox("Message", "Object/Collection Cannot be added");
                }
            }
            if (scp.validateEntityField && scp.validate) {
                scp.$evalAsync(function () {
                    scp.inputElement = undefined;
                    scp.validateEntityField();
                });
            }
        }
        e.dataTransfer.clearData();
        lstEntityTreeFieldData = null;
    } else {
        lstEntityTreeFieldData = null;
        if (e && e.preventDefault) {
            e.preventDefault();
        }
    }
}



window.onerror = function (msg, url, linenumber) {
    var rootScope = angular.element(document.body).injector().get("$rootScope");
    if (rootScope) {
        rootScope.IsLoading = false;
        rootScope.IsProjectLoaded = false;
    }
    console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
    //alert(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
    //ns.displayActivity(false);
    return true;
};

//Extra Fields Validation
function validateExtraFields(scope) {
    var flag = false;
    scope.FormDetailsErrorMessage = "";
    for (var i = 0; i < scope.objExtraFields.length; i++) {
        if (scope.objExtraFields[i].IsRequired != undefined && (scope.objExtraFields[i].IsRequired == "True" || scope.objExtraFields[i].IsRequired == true) && (!scope.objExtraFields[i].Value)) {
            scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
            flag = false;
            if (scope.objExtraFields[i].ControlType == "ComboBox") {
                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                    if (scope.objExtraFields[i].Children[j].Value) {
                        flag = true;
                        scope.FormDetailsErrorMessage = undefined;
                    }
                }
                if (flag == false) {
                    scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                    return true;
                }
            }
            if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                var flagChk = false;
                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                    if (scope.objExtraFields[i].Children[j].Value) {
                        flagChk = true;
                        flag = true;
                        scope.FormDetailsErrorMessage = undefined;
                    }
                }
                if (flagChk == false) {
                    scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                    return true;
                }
            }

            if (flag == false) {
                return true;
            }
        }

    }
    return false;
}

if (!String.prototype.includes) {
    String.prototype.includes = function () {
        'use strict';
        return String.prototype.indexOf.apply(this, arguments) !== -1;
    };
}

//Entity Message Description
var populateMessageByMessageID = function (messageID, lstMessages, objRule) {
    var messageIDFound = false;
    if (messageID && messageID.trim().length > 0) {
        var messages = lstMessages.filter(function (x) { return x.MessageID == messageID; });
        if (messages && messages.length > 0) {
            objRule.displayMessage = messages[0].DisplayMessage;

            if (messages[0].SeverityValue == 'I') {
                objRule.severityValue = "Information";
            }
            else if (messages[0].SeverityValue == 'E') {
                objRule.severityValue = "Error";
            }
            else if (messages[0].SeverityValue == 'W') {
                objRule.severityValue = "Warnings";
            }

            messageIDFound = true;

        }
    }

    if (!messageIDFound) {
        objRule.displayMessage = "";
        objRule.severityValue = "";
    }
};

// find in source - whole word search - remove special charaters before match
function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function toggleDuplicateIdErrList(event) {
    var selector = $(".duplicate-id-tooltip");
    if (selector.length > 0) {
        $(event.target).parent().siblings().find('.duplicate-id-tooltip').toggle();
        // $(event.target + ".duplicate-id-tooltip").fadeToggle();
    }
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function parsePathIntellisenseResult(data) {
    return $.map(JSON.parse(data), function (value, key) {
        return {
            label: value,
            value: value
        };
    });
}

function keyUpAction(selectedItem, ruleList) {

    var tempObj;
    var index = ruleList.Elements.indexOf(selectedItem);
    if (index > 0) {
        tempObj = ruleList.Elements[index - 1];
    }
    return tempObj;
}

function getNewBaseModel(name, dictAttributes, setNodeID) {
    if (name === undefined) {
        name = "";
    }
    var baseModel = { Name: name, Value: "", dictAttributes: {}, Elements: [], Children: [] };
    if (dictAttributes) {
        baseModel.dictAttributes = dictAttributes;
    }
    if (setNodeID) {
        baseModel.dictAttributes.sfwNodeID = generateUUID();
    }
    return baseModel;
}
/**
 * detect IE
 * returns version of IE or false, if browser is not Internet Explorer
 */
function detectIE() {
    var ua = window.navigator.userAgent;

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}

function stopEventPropagationConditional($event) {
    if ($event) {
        $event.stopPropagation();
    }
}


//#region set caret index & getting updated expression value
function addTextAtCaret(textAreaId, text) {
    var cursorPosition;
    var updatedExpression;
    var textArea = document.getElementById(textAreaId);
    var cursorPosition = textArea.selectionStart;
    cursorPosition = cursorPosition;

    updatedExpression = addTextAtCursorPosition(textArea, cursorPosition, text);
    updateCursorPosition(cursorPosition, text, textArea);
    return updatedExpression;
}
function addTextAtCursorPosition(textArea, cursorPosition, text) {
    var front = (textArea.value).substring(0, cursorPosition);
    var back = (textArea.value).substring(cursorPosition, textArea.value.length);
    textArea.value = front + text + back;
    return textArea.value;
}
function updateCursorPosition(cursorPosition, text, textArea) {
    if (text == undefined) {
        text.length = 0;
    }
    cursorPosition = cursorPosition + text.length;
    //   textArea.selectionStart = cursorPosition;
    textArea.focus();
    //  textArea.selectionEnd = cursorPosition;

}
(function ($) {
    $.fn.hasScrollBar = function () {
        if (this.get(0) && this.get(0).scrollHeight)
            return this.get(0).scrollHeight > this.height();
    }
})(jQuery);
//#endregion

function ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, objSelectedLogicalRule) {
    //angular.forEach(objSelectedLogicalRule.Rows, function (row) {
    //    if (row.Cells.length > 0 && row.Cells[0].Item && row.Cells[0].Item.IsStepSelected == true) {
    //        angular.forEach(row.Cells, function (step) {
    //            step.IsVisiblefterExecution = true;
    //        });
    //    }
    //});

    if (ablnShowOnlyExecutedPath) {
        for (var i = 0; i < objSelectedLogicalRule.Rows.length; i++) {
            var row = objSelectedLogicalRule.Rows[i];
            row.IsVisiblefterExecution = false;
            if (row.Cells.length > 0 && row.Cells[0].Item && row.Cells[0].Item.IsStepSelected == true) {

                row.IsVisiblefterExecution = true;
                var rowspan = row.Cells[0].Rowspan;
                if (rowspan > 1) {
                    for (var j = i + 1; j < i + rowspan; j++) {
                        objSelectedLogicalRule.Rows[j].IsVisiblefterExecution = true;
                    }
                    i = i + rowspan - 1;
                }
            }
        }
    }
    else {
        for (var i = 0; i < objSelectedLogicalRule.Rows.length; i++) {
            var row = objSelectedLogicalRule.Rows[i];
            row.IsVisiblefterExecution = true;
        }
    }

};

function setDataToEditor(scope, xmlstring, lineno, ID) {
    if (!lineno) {
        lineno = 1;
    }
    if (!xmlstring) {
        xmlstring = "";
    }
    if (scope && !scope.editor) {
        var divId = "editor_" + ID;
        scope.editor = ace.edit(divId);
        scope.editor.getSession().setMode("ace/mode/xml");
        scope.editor.resize(true);
    }
    if (scope && scope.editor) {
        scope.$evalAsync(function () {
            scope.editor.getSession().setValue(xmlstring);
            scope.isSourceDirty = false;
            scope.editor.setFontSize(14);
            scope.editor.getSession().on('change', function (e) {
                if (scope.editor.curOp && scope.editor.curOp.command.name) {
                    scope.$evalAsync(function () {
                        scope.isSourceDirty = true;
                        scope.isDirty = true;
                    });
                }
            });
            scope.editor.gotoLine(lineno);
            scope.editor.renderer.scrollCursorIntoView({ row: lineno, column: 1 }, 0.5);
        });
    }
};

//#region Knowtion Related Functions

//disable F1 (default help  Functionality)
function preventDefaultBrowserHelp() {
    $(function () {
        var cancelKeypress = false;
        // Need to cancel event (only applies to IE)
        if ("onhelp" in window) {
            // (jQuery cannot bind "onhelp" event)
            window.onhelp = function () {
                return false;
            };
        }
        $(document).keydown(function (evt) {
            // F1 pressed
            if (evt.keyCode === 112) {
                if (window.event) {
                    // Write back to IE's event object
                    window.event.keyCode = 0;
                }
                cancelKeypress = true;
                return false;
            }
        });
        // Needed for Opera (as in Andy E's answer)
        $(document).keypress(function (evt) {
            if (cancelKeypress) {
                cancelKeypress = false; // Only this keypress
                return false;
            }
        });
    });
}
//preventDefaultBrowserHelp();
function getKnowtionHelpUrl(knowtionId) {
    var knowtionUrl;

    $.ajax({
        url: "api/Login/GetKnowtionUrl?astrKnowtionID=" + knowtionId,
        type: 'POST',
        async: false,
        success: function (data) {
            knowtionUrl = data;
        },
        error: function (response) {
            var objError = JSON.parse(response.responseText);
            if (objError && objError.Message) {
                var msg = objError.Message;
                if (objError.ExceptionMessage) {
                    msg = msg + "\n" + objError.ExceptionMessage;
                }
                MessageBox("Message", msg);
            }
            else {
                MessageBox("Message", "An error occured while getting knowtion help url.");
            }
        }
    });

    return knowtionUrl;
}

//#endregion


function closest(el, predicate) {
    return predicate(el) ? el : (
        el && (el.parentNode instanceof Element) && closest(el.parentNode, predicate)
    );
}

// allowed numbers only in input 
function onlyAllowedNumbers(event) {
    if (event && !(event.charCode >= 48 && event.charCode <= 57)) {
        event.preventDefault();
    }
};

function validatePathForSpecialChar(path) {
    var regPatt = /^(\\|\s)|([\/:*?"<>|])|(\\\s*\\)|(\\$)/;
    return regPatt.test(path);
}

//get DBTypes 
function getlstQueryTypes(strQuertTypes) {
    var lstDB = strQuertTypes.split(';');
    var lstDBTypes = [];
    for (var i = 0; i < lstDB.length; i++) {
        var objDBType = {};
        var lstType = lstDB[i].split(':');
        objDBType.ID = lstType[0];
        objDBType.Attribute = lstType[1];
        lstDBTypes.push(objDBType);
    }
    return lstDBTypes;
};

//get Query
function getQuery(queryTypes, selectedQuery) {
    var query = undefined;
    for (var i = 0; i < queryTypes.length; i++) {
        if (queryTypes[i] && queryTypes[i].Attribute) {
            if (selectedQuery.dictAttributes[queryTypes[i].Attribute]) {
                query = selectedQuery.dictAttributes[queryTypes[i].Attribute];
                break;
            }
        }

    };
    return query;
};

function MessageBox(aTitle, aMessage, isConfirm, aCallback, aSize) {
    var MessagesService = angular.element(document.body).injector().get("$SgMessagesService");
    if (MessagesService && MessagesService.Message) {
        MessagesService.Message(aTitle, aMessage, isConfirm, aCallback, aSize);
    }
};

function getQueryFromEntityIntellisense(astrQueryId, alstEntity) {
    var lobjQuery = null;
    var lst = astrQueryId != undefined && astrQueryId != null ? astrQueryId.split('.') : [];
    if (lst && lst.length == 2) {
        var entityName = lst[0];
        var strQueryID = lst[1];
        var lstEntity = alstEntity.filter(function (x) {
            return x.ID == entityName;
        });
        if (lstEntity && lstEntity.length > 0) {
            var objEntity = lstEntity[0];
            var lstQuery = objEntity.Queries.filter(function (x) {
                return x.ID == strQueryID;
            });
            if (lstQuery && lstQuery.length > 0) {
                var objQuery = lstQuery[0];
                lobjQuery = objQuery;
            }
        }
    }
    return lobjQuery;
};
function combineAsPath() {
    var pathSeparator = "\\";
    var path = "";
    if (arguments && arguments.length) {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i].trim()) {
                path += arguments[i].trimFull(pathSeparator) + pathSeparator;
            }
        }
    }
    return path.trimFull(pathSeparator);
}

var GetTokens = function (str) {
    var count = 0;
    var regex = /{(.+?)}/g;
    var match, results = [];
    while (match = regex.exec(str)) {
        results.push(match[1]);
        count++;
    }

    return count;
};
function updateModelForBaseAppModel(mergedModel, baseAppModel, updateParentName, appParentModel, baseParentModel) {
    var updateParent = false;
    for (var baseIndex = 0; baseIndex < baseAppModel.Elements.length; baseIndex++) {
        var bmodel = baseAppModel.Elements[baseIndex];
        for (var appIndex = 0; appIndex < mergedModel.Elements.length; appIndex++) {
            var amodel = mergedModel.Elements[appIndex];
            if (bmodel.Name === amodel.Name) {
                if (baseAppModel.Name === "constant" && bmodel.Name === "item" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.sfwEffectiveDate === amodel.dictAttributes.sfwEffectiveDate) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (baseAppModel.Name === "constraint" && bmodel.Name === "item" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.sfwFieldName === amodel.dictAttributes.sfwFieldName) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (baseAppModel.Name === "ColumnName" && bmodel.Name === "Value" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.Text === amodel.dictAttributes.Text) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (bmodel.Name === "ColumnName" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.value === amodel.dictAttributes.value) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (bmodel.dictAttributes.ID === amodel.dictAttributes.ID) {
                    updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                }
            }
        }
    }
    if (updateParentName && mergedModel.Name === updateParentName) {
        updateParent = getDescendents(mergedModel).some(function (itm) { return itm.BaseAppModel; });
    }

    var duplicateMergedModel = {};
    var duplicateBaseAppModel = {};

    for (var prop in mergedModel.dictAttributes) {
        if (mergedModel.dictAttributes[prop]) {
            duplicateMergedModel[prop] = mergedModel.dictAttributes[prop];
        }
    }
    for (var prop in baseAppModel.dictAttributes) {
        if (baseAppModel.dictAttributes[prop]) {
            duplicateBaseAppModel[prop] = baseAppModel.dictAttributes[prop];
        }
    }

    var isSame = JSON.stringify(duplicateMergedModel) === JSON.stringify(duplicateBaseAppModel);
    if (!isSame || updateParent) {
        mergedModel.BaseAppModel = baseAppModel;
        mergedModel.IsBaseAppModel = false;
    }
}

function revertToBaseAppModel(model) {
    if (model && model.BaseAppModel) {
        model.Elements = JSON.parse(JSON.stringify(model.BaseAppModel.Elements));
        model.Children = JSON.parse(JSON.stringify(model.BaseAppModel.Children));
        model.dictAttributes = JSON.parse(JSON.stringify(model.BaseAppModel.dictAttributes));
        model.IsBaseAppModel = true;
        model.BaseAppModel = null;
    }
}


app.directive("openfiledirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: "E",
        scope: {
            curobj: '=',
        },
        replace: true,
        link: function (scope, element, attrs) {
            scope.setclassforopenfile = function (obj) {
                if (obj != undefined) {
                    if (obj.isvisible != undefined) {
                        if (obj.isvisible == true) {
                            return "file-wrapper";
                        }
                        else {
                            return "displaynone";
                        }
                    }
                    else {
                        return "file-wrapper";
                    }
                }
            };
            if (scope.curobj.file != undefined) {
                if (scope.curobj.file.FileType != undefined) {

                    if (scope.curobj.file.FileType == "LogicalRule" || scope.curobj.file.FileType == "DecisionTable" || scope.curobj.file.FileType == "ExcelMatrix") {
                        scope.contentUrl = "Rule/views/RuleCommon.html";
                    }
                    else if (scope.curobj.file.FileType == "ParameterScenario" || scope.curobj.file.FileType == "ObjectScenario" || scope.curobj.file.FileType == "ExcelScenario") {
                        scope.contentUrl = "Scenario/views/Scenario.html";
                    }
                    else if (scope.curobj.file.FileType == "Entity") {
                        scope.contentUrl = "Entity/views/Entity.html";
                    }
                    else if (scope.curobj.file.FileType == "CustomSettings") {
                        scope.contentUrl = "Tools/views/CustomSettings.html";
                    }
                    else if (scope.curobj.file.FileType == "ExecuteQuery") {
                        scope.contentUrl = "Tools/views/ExecuteQuery.html";
                    }
                    else if (scope.curobj.file.FileType == "RuleConstants") {
                        scope.contentUrl = "Constants/Views/Constants.html";
                    }
                    else if (scope.curobj.file.FileType == "ProjectConfiguration") {
                        scope.contentUrl = "Tools/views/ProjectConfiguration.html";
                    }
                    else if (scope.curobj.file.FileName == "RunResults") {
                        scope.contentUrl = "Views/Tools/RunResults.html";
                    }
                    else if (scope.curobj.file.FileType == "AuditLog") {
                        scope.contentUrl = "Tools/views/AuditLog.html";
                    }
                    else if (scope.curobj.file.FileType == "NeoRuleSettings") {
                        scope.contentUrl = "Views/Tools/ConfigureSettings.html";
                    }
                    else if (scope.curobj.file.FileType == "InboundFile" || scope.curobj.file.FileType == "OutboundFile") {
                        scope.contentUrl = "File/views/File.html";
                    }
                    else if (scope.curobj.file.FileType == "Maintenance" || scope.curobj.file.FileType == "Lookup" || scope.curobj.file.FileType == "Wizard" || scope.curobj.file.FileType == "UserControl" || scope.curobj.file.FileType == "Tooltip") {
                        scope.contentUrl = "Form/views/Form.html";

                    }
                    else if (scope.curobj.file.FileType == "FormLinkMaintenance" || scope.curobj.file.FileType == "FormLinkLookup" || scope.curobj.file.FileType == "FormLinkWizard") {

                        scope.contentUrl = "FormLink/views/FormLink.html";

                    }
                    else if (scope.curobj.file.FileType == "Correspondence" || scope.curobj.file.FileType == "PdfCorrespondence") {
                        scope.contentUrl = "Correspondence/views/Correspondence.html";
                    }
                    else if (scope.curobj.file.FileType == "BPMN" || scope.curobj.file.FileType == "BPMTemplate") {
                        scope.contentUrl = "BPM/views/BPMN.html";
                    }
                    else if (scope.curobj.file.FileType == "Report") {
                        scope.contentUrl = "Report/views/Report.html";
                    }
                    else if (scope.curobj.file.FileType == "SearchFiles") {
                        scope.contentUrl = "StartUp/views/SearchFiles.html";
                    }
                    else if (scope.curobj.file.FileType == "WorkflowMap") {
                        scope.contentUrl = "WorkFlow/views/WorkFlowMap.html";
                    }
                    else if (scope.curobj.file.FileType == "VersionBPM") {
                        scope.contentUrl = "BPM/views/BPMVersion.html";
                    }
                    else if (scope.curobj.file.FileType == "ERDiagram") {
                        scope.contentUrl = "Tools/views/NewERDiagram.html";
                    }
                }

            }


        },
        template: '<div id="{{curobj.file.FileName}}" ng-include="contentUrl" ng-class="setclassforopenfile(curobj)" ></div>',
    };
}]);

app.directive('contextMenu', ["$parse", "$q", function ($parse, $q) {

    var contextMenus = [];

    var removeContextMenus = function (level) {
        while (contextMenus.length && (!level || contextMenus.length > level)) {
            contextMenus.pop().remove();
        }
        if (contextMenus.length == 0 && $currentContextMenu) {
            $currentContextMenu.remove();
        }
    };

    var $currentContextMenu = null;

    var renderContextMenu = function ($scope, event, options, model, level) {
        if (!level) {
            level = 0;
        }
        if (!$) {
            var $ = angular.element;
        }
        $(event.currentTarget).addClass('context');
        var $contextMenu = $('<div>');
        if ($currentContextMenu) {
            $contextMenu = $currentContextMenu;
        } else {
            $currentContextMenu = $contextMenu;
        }
        $contextMenu.addClass('dropdown clearfix');
        var $ul = $('<ul class="contex-menu">');
        $ul.addClass('dropdown-menu');
        $ul.attr({
            'role': 'menu'
        });
        $ul.css({
            display: 'block',
            position: 'absolute',
            left: event.pageX + 'px',
            top: event.pageY + 'px',
            "z-index": 10000
        });
        var $promises = [];
        var count = 0;
        var enabledCount = 0;
        angular.forEach(options, function (item, i) {
            var $li = $('<li>');
            if (item === null) {
                $li.addClass('divider');
                count++;
            } else {
                var nestedMenu = angular.isArray(item[1])
                    ? item[1] : angular.isArray(item[2])
                        ? item[2] : angular.isArray(item[3])
                            ? item[3] : null;
                var $a = $('<a>');
                // $a.css("padding-right", "8px");
                $a.attr({
                    tabindex: '-1', href: '#'
                });
                var text = typeof item[0] == 'string' ? item[0] : item[0].call($scope, $scope, event, model);
                $promise = $q.when(text);
                $promises.push($promise);
                $promise.then(function (text) {
                    $a.text(text);
                    if (nestedMenu) {
                        $a.css("cursor", "default");
                        $a.append($('<strong style="font-family:monospace;font-weight:bold;float:right; position:absolute; margin-left:5px;">&gt;</strong>'));
                    }
                    if (item[4] && typeof item[4] === 'string') {
                        $span = $("<span>");
                        $span.css("float", "right");
                        $span.css("margin-left", "10px");
                        $span.text(item[4]);
                        $a.append($span);
                    }
                });
                $li.append($a);

                var enabled = angular.isFunction(item[2]) ? item[2].call($scope, $scope, event, model, text) : true;
                if (enabled) {
                    enabledCount++;
                    var openNestedMenu = function ($event) {
                        removeContextMenus(level + 1);
                        var a = $($ul[0]).outerWidth();

                        var childPopupX = $ul[0].offsetLeft;
                        var ev = {
                            pageX: childPopupX,
                            pageY: $ul[0].offsetTop + $li[0].offsetTop - 3,
                            currentMenuWidth: $ul[0].offsetWidth,
                        };
                        renderContextMenu($scope, ev, nestedMenu, model, level + 1);
                    };
                    $li.on('click', function ($event) {
                        $event.preventDefault();
                        $scope.$apply(function () {
                            if (nestedMenu) {
                                openNestedMenu($event);
                            } else {
                                $(event.currentTarget).removeClass('context');
                                removeContextMenus();
                                item[1].call($scope, $scope, event, model, text);
                            }
                        });
                    });
                    var tempmenu = "";
                    $li.on('mouseover', function ($event, $index) {
                        $scope.$evalAsync(function () {
                            if (nestedMenu && tempmenu != nestedMenu) {
                                tempmenu = nestedMenu;
                                openNestedMenu($event);
                            }
                            else if (!nestedMenu) {
                                while ((contextMenus.length - 1) > level) {
                                    contextMenus.pop().remove();
                                }
                            }
                        });
                    });
                    $li.on('mouseleave', function ($event, $index) {
                        tempmenu = "";
                    });
                } else {
                    $li.on('click', function ($event) {
                        $event.preventDefault();
                    });
                    $li.addClass('disabled');
                    $li.css({
                        display: 'none'
                    });

                    var div = $($ul[0].children);
                    if (count > 0) {
                        div[div.length - 1].className = "";
                    }
                }
            }
            $ul.append($li);
        });
        if (enabledCount > 0) {
            $contextMenu.append($ul);
        }
        //var height = Math.max(
        //    document.body.scrollHeight, document.documentElement.scrollHeight,
        //    document.body.offsetHeight, document.documentElement.offsetHeight,
        //    document.body.clientHeight, document.documentElement.clientHeight
        //);
        var height = $("#main-wrapper").height();
        $contextMenu.css({
            width: '100%',
            height: height + 'px',
            position: 'absolute',
            top: 0,
            left: 0,
            zIndex: 99999999
        });
        $(document).find('body').append($contextMenu);

        //calculate if drop down menu would go out of screen at left or bottom
        // calculation need to be done after element has been added (and all texts are set; thus thepromises)
        // to the DOM the get the actual height
        var levelWidth = 0;
        $q.all($promises).then(function () {
            if (level === 0) {
                var topCoordinate = event.pageY;
                var menuHeight = angular.element($ul[0]).prop('offsetHeight');
                var winHeight = $("#main-wrapper").height();
                if (topCoordinate > menuHeight && winHeight - topCoordinate < menuHeight) {
                    topCoordinate = event.pageY - menuHeight;
                }

                var leftCoordinate = event.pageX;
                var menuWidth = angular.element($ul[0]).prop('offsetWidth');
                var winWidth = event.view.innerWidth;
                if ((leftCoordinate > menuWidth && winWidth - leftCoordinate < menuWidth) || ((leftCoordinate + menuWidth) >= $(window).width())) {
                    leftCoordinate = event.pageX - menuWidth - 20;
                    if ((leftCoordinate + menuWidth) > ($(window).width() - 50)) {
                        leftCoordinate -= 20;
                    }
                }
                var maxHeight = 150;
                if ((winHeight - topCoordinate - 10) > 150) {
                    maxHeight = (winHeight - topCoordinate - 10);
                }
                $ul.css({
                    display: 'block',
                    position: 'absolute',
                    left: leftCoordinate + 'px',
                    top: topCoordinate + 'px',
                    "max-height": maxHeight + 'px',
                    overflow: 'auto'
                });
            }
            else {
                levelWidth = angular.element($ul[0]).prop('offsetWidth');
                var leftWidthNew = "0px";
                if (level > 0) {
                    if ((event.pageX + event.currentMenuWidth + levelWidth) > $(window).width()) {
                        $ul.css({
                            display: 'block',
                            position: 'absolute',
                            left: (event.pageX - levelWidth) + 'px'
                        });
                        leftWidthNew = event.pageX - levelWidth;
                    }
                    else {
                        $ul.css({
                            display: 'block',
                            position: 'absolute',
                            left: (event.pageX + event.currentMenuWidth) + 'px'
                        });
                        leftWidthNew = event.pageX + event.currentMenuWidth;
                    }
                    var topCoordinate = event.pageY;
                    var menuHeight = angular.element($ul[0]).prop('offsetHeight');
                    var winHeight = $("#main-wrapper").height();
                    if (menuHeight > winHeight || topCoordinate + menuHeight > winHeight) {
                        var height = winHeight - topCoordinate;
                        if (height > menuHeight) {
                            $ul.css({
                                "max-height": height + 'px',
                                overflow: 'auto'
                            });
                        }
                        else {
                            if (menuHeight + 50 > winHeight) {
                                $ul.css({
                                    top: '130px',
                                    left: leftWidthNew + 'px',
                                    "max-height": winHeight - 140 + 'px',
                                    overflow: 'auto'
                                });
                            }
                            else {
                                topCoordinate = event.pageY - menuHeight;
                                if (topCoordinate < 150) {
                                    topCoordinate = 150;
                                }
                                $ul.css({
                                    top: topCoordinate + 'px',
                                    "max-height": winHeight - 140 + 'px',
                                    overflow: 'auto'
                                });
                            }
                        }
                    }
                }
            }
        });

        $contextMenu.on("mousedown", function (e) {
            if ($(e.target).hasClass('dropdown')) {
                $(event.currentTarget).removeClass('context');
                removeContextMenus();
            }
        }).on('contextmenu', function (event) {
            $(event.currentTarget).removeClass('context');
            event.preventDefault();
            removeContextMenus(level);
        });
        $scope.$on("$destroy", function () {
            removeContextMenus();
        });

        contextMenus.push($ul);
    };
    return function ($scope, element, attrs) {
        element.on('contextmenu', function (event) {
            event.stopPropagation();
            $scope.$apply(function () {
                event.preventDefault();
                var options = $scope.$eval(attrs.contextMenu);
                var model = $scope.$eval(attrs.model);
                if (options instanceof Array) {
                    if (options.length === 0) {
                        return;
                    }
                    renderContextMenu($scope, event, options, model);
                } else {
                    throw '"' + attrs.contextMenu + '" not an array';
                }
            });
        });
    };
}]);

app.directive("newentityobjecttreetemplate", ["$compile", "$rootScope", "$EntityIntellisenseFactory", function ($compile, $rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            entityname: '=',
            selectedfield: '=',
            lstentity: '=',
            ischeckboxvisible: '=',
            selectedentityobjecttreefields: '=',
            iscollectionnextlevelload: '=',
            lstdisplayentity: '=',
            showonlycollection: '=',
            lstselectedmultiplelevelfield: '=',
            showonlyobject: '=',
            lstpreselectedfields: '=',
            lstpredisabledfields: '=',
            formid: '=',
            showonlycolumns: '=',
            showcolumndescription: '=',
            showvalueofcolumn: '=',
            ismainentity: '=',
            isallownavigation: '='
        },
        link: function (scope, element, attrs) {

            var unwatch = scope.$watch('entityname', function (newVal, oldVal) {
                scope.Init();
            });

            var unwatch2 = scope.$watch("lstpreselectedfields", function (newVal, oldVal) {
                if (newVal && newVal.length > 0) {
                    if (scope.lstpreselectedfields) {
                        angular.forEach(scope.lstpreselectedfields, function (itm) {
                            if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length) {
                                var lst = scope.selectedentityobjecttreefields.filter(function (x) { return x.ID == itm.ID; });
                                if (lst && lst.length > 0) {
                                    lst[0].IsSelected = 'True';
                                    lst[0].IsRecordSelected = true;
                                    lst[0].IsReadOnly = true;
                                    if (scope.lstselectedmultiplelevelfield) {
                                        var obj = { EntityField: lst[0].ID, FieldObject: lst[0], Entity: lst[0].EntityName };
                                        var isFieldFound = scope.isFieldFoundInSelectedList(scope.lstselectedmultiplelevelfield, lst[0].ID);
                                        if (!isFieldFound) {
                                            scope.lstselectedmultiplelevelfield.push(obj);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });

            scope.isFieldFoundInSelectedList = function (lstfields, fieldID) {
                var isFieldFound = false;
                for (var i = 0; i < lstfields.length; i++) {
                    if (lstfields[i].EntityField == fieldID) {
                        isFieldFound = true;
                        break;
                    }
                }
                return isFieldFound;
            };

            scope.CheckForPreDisableAttr = function (objEntity, path) {
                if (scope.lstpredisabledfields) {
                    angular.forEach(scope.lstpredisabledfields, function (itm) {
                        var lst = objEntity.SortedAttributes.filter(function (x) { return x.ID == itm.Value.ID; });
                        if (lst && lst.length > 0) {
                            if (path == "" || path == undefined) {
                                if (lst[0].ID == itm.Key) {
                                    lst[0].IsDisabled = true;
                                }
                            } else if ((path + "." + lst[0].ID) == itm.Key) {
                                lst[0].IsDisabled = true;
                            }
                        }
                    });
                }
            };

            scope.AddClsEntity = function (strEntityname, strdisplayName, strParentEntity, strDataTypeOfField) {
                if (strDataTypeOfField == 'Collection' || strDataTypeOfField == 'List') {
                    scope.isParentFieldCollection = true;
                }
                var objclsEntity = {};
                var Entity = scope.checkEntityIsPresent(strEntityname);
                if (Entity == "") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var parentID = strEntityname;
                    var entattributes = [];
                    while (parentID) {
                        if (entityIntellisenseList && entityIntellisenseList.length) {
                            var lst = entityIntellisenseList.filter(function (x) { return x.ID == parentID; });
                            if (lst && lst.length > 0) {
                                if (scope.showonlycollection != undefined && !scope.showonlycollection && lst[0].Attributes) {
                                    var tempAttributes = [];
                                    tempAttributes = lst[0].Attributes.filter(function (x) { return (x.DataType && ((x.DataType != 'Collection') && (x.DataType != 'List'))) || !x.DataType; });
                                    entattributes = entattributes.concat(tempAttributes);
                                }
                                else {
                                    entattributes = entattributes.concat(lst[0].Attributes);
                                }
                                parentID = lst[0].ParentId;
                            } else {
                                parentID = "";
                            }
                        }
                    }
                    var EntityAttributes = [];
                    var strattr = JSON.stringify(entattributes.filter(function (attr) { return !(attr.Value && attr.Value.toLowerCase() === "app_json_data"); }));
                    entattributes = JSON.parse(strattr);
                    sortListBasedOnproperty(entattributes, "", "ID");
                    EntityAttributes = entattributes;

                    objclsEntity = {
                        IsVisible: true, strEntityId: strEntityname, Attributes: EntityAttributes, SortedAttributes: EntityAttributes
                    };

                    scope.lstEntities.push(objclsEntity);
                    if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                        ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                    }
                    scope.selectedentityobjecttreefields = EntityAttributes;
                    if (scope.ismainentity && strEntityname && entityIntellisenseList) {
                        var tempEntity = entityIntellisenseList.filter(function (x) {
                            return x.ID == strEntityname;
                        });
                        if (tempEntity.length > 0 && tempEntity[0].ErrorTableName) {
                            var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                            var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                            scope.selectedentityobjecttreefields.push(objInternalError);
                            scope.selectedentityobjecttreefields.push(objExternalError);
                        }
                    }

                } else {
                    objclsEntity = Entity;
                    if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                        ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                    }
                    scope.selectedentityobjecttreefields = Entity.Attributes;
                }
                scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstEntities, strEntityname, "Entity");
                scope.AddClsDisplayEntity(objclsEntity, strdisplayName, strParentEntity, strDataTypeOfField);
                scope.CheckForPreDisableAttr(objclsEntity, scope.lstdisplayentity[scope.lstdisplayentity.length - 1].strDisplayName);
            };

            scope.AddClsDisplayEntity = function (clsEntity, strdisplayName, strParentEntity, strDataTypeOfField) {
                var objclsDisplayEntity = {
                    IsVisible: true, strDisplayName: strdisplayName, strID: clsEntity.strEntityId, entity: clsEntity, strParentID: strParentEntity, DataType: strDataTypeOfField, isParentFieldCollection: scope.isParentFieldCollection
                };
                scope.lstDisplayEntity.push(objclsDisplayEntity);
                scope.lstdisplayentity = scope.lstDisplayEntity;
                scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstDisplayEntity, strdisplayName, "DisplayEntity");
            };

            scope.setVisibilityForEntitiesOrDisplayEntities = function (obj, strname, param) {

                scope.objEntityFilter.EntityTreeFilter = "";

                for (var i = 0; i < obj.length; i++) {
                    if (param == "Entity") {
                        if (obj[i].strEntityId == strname) {
                            obj[i].IsVisible = true;
                            scope.currentEntity = obj[i];
                            obj[i].SortedAttributes = obj[i].Attributes;
                            if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                                ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                            }

                            scope.selectedentityobjecttreefields = obj[i].SortedAttributes;

                        } else {
                            obj[i].IsVisible = false;
                        }
                    } else {
                        if (obj[i].strDisplayName == strname) {
                            obj[i].IsVisible = true;
                            scope.objEntityFilter.DisplayPath = obj[i].strDisplayName;
                        } else {
                            obj[i].IsVisible = false;
                        }
                    }
                }

                if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                    scope.objEntityFilter.LimitCount = 50;
                } else {
                    scope.objEntityFilter.LimitCount = scope.selectedentityobjecttreefields.length;
                }

                if (scope.lstselectedmultiplelevelfield && param == "DisplayEntity") {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    if (displayEntity) {
                        for (var i = 0; i < scope.selectedentityobjecttreefields.length; i++) {
                            scope.setValueToSelectedField(scope.selectedentityobjecttreefields[i], displayEntity);
                        }
                    }
                }
            };

            scope.setValueToSelectedField = function (field, displayEntity) {
                var displayName = displayEntity.strDisplayName;
                fieldName = field.ID;
                if (displayName != "") {
                    fieldName = displayName + "." + field.ID;
                }
                if (field.DataType != "Object" && field.DataType != "Collection" && field.DataType != "List" && field.DataType != "CDOCollection") {
                    for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                        if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                            field.IsSelected = "True";
                            break;
                        }
                    }
                }
            };

            scope.checkEntityIsPresent = function (strentityID) {
                var Entity = "";
                for (var i = 0; i < scope.lstEntities.length; i++) {
                    if (scope.lstEntities[i].strEntityId == strentityID) {
                        Entity = scope.lstEntities[i];
                        break;
                    }
                }

                return Entity;
            };

            scope.getParentEntity = function () {
                var entity = "";
                for (var i = 0; i < scope.lstEntities.length; i++) {
                    if (scope.lstEntities[i].IsVisible) {
                        entity = scope.lstEntities[i].strEntityId;
                        break;
                    }
                }
                return entity;
            };

            scope.getPathFromDisplayedEntities = function () {
                var path = "";
                for (var i = 0; i < scope.lstDisplayEntity.length; i++) {
                    if (scope.lstDisplayEntity[i].IsVisible) {
                        path = scope.lstDisplayEntity[i].strDisplayName;
                        break;
                    }
                }
                return path;
            };
            scope.Init = function () {
                scope.lstEntities = [];
                scope.lstDisplayEntity = [];
                scope.objEntityFilter = {};
                scope.objEntityFilter.EntityTreeFilter = "";
                scope.objEntityFilter.LimitCount = 50;
                scope.objEntityFilter.DisplayPath = "";
                scope.currentEntity = '';
                scope.otherSelectedField = [];
                scope.selectedentityobjecttreefields = [];
                scope.isParentFieldCollection = false;

                var strdisplayName = "";
                var strParentEntity = null;
                scope.AddClsEntity(scope.entityname, strdisplayName, strParentEntity, "");
            };

            if (scope.entityname) {
                scope.Init();
            }

            scope.LoadNextLevelEntityField = function (field, $event) {
                if (field.DataType == 'Collection' || field.DataType == 'List' || field.DataType == 'Object') {
                    if ((field.DataType == 'Collection' || field.DataType == 'List') && !scope.iscollectionnextlevelload) {
                        //alert("Collection Fields Cannot be added..");
                    } else {
                        var objectpath = scope.getPathFromDisplayedEntities();
                        var strEntityname = field.Entity;
                        var strDataTypeOfField = field.DataType;
                        var strdisplayName = "";
                        if (objectpath != "") {
                            strdisplayName = objectpath + "." + field.DisplayName;
                        } else {
                            strdisplayName = field.DisplayName;
                        }
                        var strParentEntity = scope.getParentEntity();
                        scope.AddClsEntity(strEntityname, strdisplayName, strParentEntity, strDataTypeOfField);
                    }
                }
            };

            scope.Navigatetoprevlist = function () {
                if (scope.lstDisplayEntity.length > 1) {
                    scope.lstDisplayEntity.splice(scope.lstDisplayEntity.length - 1, 1);
                    scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].IsVisible = true;
                    scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstEntities, scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].strID, "Entity");
                    scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstdisplayentity, scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].strDisplayName, "DisplayEntity");
                    scope.CheckForPreDisableAttr(scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].entity, scope.lstdisplayentity[scope.lstdisplayentity.length - 1].DisplayName);
                }
            };

            scope.onRefreshAttributes = function () {
                if (scope.lstEntities && scope.lstEntities.length > 0) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < scope.lstEntities.length; i++) {
                        var entityId = scope.lstEntities[i].strEntityId;
                        var parentID = entityId;
                        var entattributes = [];
                        while (parentID) {
                            if (entityIntellisenseList && entityIntellisenseList.length) {
                                var lst = entityIntellisenseList.filter(function (x) { return x.ID == parentID; });
                                if (lst && lst.length > 0) {
                                    if (scope.showonlycollection != undefined && !scope.showonlycollection && lst[0].Attributes) {
                                        var tempAttributes = [];
                                        tempAttributes = lst[0].Attributes.filter(function (x) { return x.DataType && ((x.DataType != 'Collection') && (x.DataType != 'List')); });
                                        entattributes = entattributes.concat(tempAttributes);
                                    }
                                    else {
                                        entattributes = entattributes.concat(lst[0].Attributes);
                                    }
                                    //entattributes = entattributes.concat(lst[0].Attributes);
                                    parentID = lst[0].ParentId;
                                } else {
                                    parentID = "";
                                }
                            }
                        }
                        var EntityAttributes = [];
                        var strattr = JSON.stringify(entattributes.filter(function (attr) { return !(attr.Value && attr.Value.toLowerCase() === "app_json_data"); }));
                        entattributes = JSON.parse(strattr);

                        if (scope.ismainentity && scope.entityname && entityIntellisenseList) {
                            var tempEntity = entityIntellisenseList.filter(function (x) {
                                return x.ID == scope.entityname;
                            });
                            if (tempEntity.length > 0 && tempEntity[0].ErrorTableName) {
                                var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                                var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                                entattributes.push(objInternalError);
                                entattributes.push(objExternalError);
                            }
                        }

                        sortListBasedOnproperty(entattributes, "", "ID");
                        EntityAttributes = entattributes;
                        scope.lstEntities[i].Attributes = entattributes;
                        scope.lstEntities[i].SortedAttributes = EntityAttributes;

                        var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                        if (displayEntity && displayEntity.strID == entityId) {
                            scope.selectedentityobjecttreefields = EntityAttributes;
                        }
                    }
                    var newScope = scope.$new();
                    newScope.strMessage = "Refresh Completed.";
                    if (scope.objEntityFilter && scope.objEntityFilter.EntityTreeFilter) {
                        scope.objEntityFilter.EntityTreeFilter = "";
                    }
                    newScope.isError = true;
                    var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html");
                    newScope.OkClick = function () {
                        dialog.close();
                        scope.selectedfield = undefined;
                    };
                }
            };

            scope.selectField = function (field, event) {
                if (scope.selectedfield && !event.ctrlKey) {
                    scope.selectedfield.IsRecordSelected = false;
                    if (!scope.ischeckboxvisible) {
                        scope.selectedfield.IsSelected = "False";
                    }
                } else {
                    if (scope.selectedfield) {
                        var isFound = scope.RemoveOrIsFieldPresentFromOtherSelectedField(scope.selectedfield, "bool");
                        if (!isFound) {
                            scope.otherSelectedField.push(scope.selectedfield);
                        }
                        scope.selectedfield = undefined;
                    }
                }

                if (!event.ctrlKey) {
                    scope.selectedfield = field;
                    scope.selectedfield.IsRecordSelected = true;
                }
                if (event) {
                    event.stopPropagation();
                    event.stopImmediatePropagation();
                }

                if (!scope.ischeckboxvisible) {
                    if (event.ctrlKey) {
                        if (field.IsRecordSelected) {
                            field.IsRecordSelected = false;
                            field.IsSelected = "False";
                            scope.RemoveOrIsFieldPresentFromOtherSelectedField(field, "Delete");
                        }
                        else {
                            field.IsRecordSelected = true;
                            field.IsSelected = "True";
                            scope.otherSelectedField.push(field);
                        }
                    }
                    else {
                        for (var i = 0; i < scope.otherSelectedField.length; i++) {
                            scope.otherSelectedField[i].IsRecordSelected = false;
                            scope.otherSelectedField[i].IsSelected = "False";
                        }
                        scope.otherSelectedField = [];
                        field.IsRecordSelected = true;
                        field.IsSelected = "True";
                    }
                }
            };

            scope.RemoveOrIsFieldPresentFromOtherSelectedField = function (objField, param) {
                var isFound = false;
                for (var i = 0; i < scope.otherSelectedField.length; i++) {
                    if (scope.otherSelectedField[i].ID == objField.ID) {
                        if (param == "Delete") {
                            scope.otherSelectedField.splice(i, 1);
                        } else if (param == "bool") {
                            isFound = true;
                        }
                        break;
                    }
                }

                return isFound;
            };

            scope.sortList = function (clsEntity, strText) {
                if (strText != "") {
                    var lstExactMatchCaseSensitive = [];
                    var lstExactMatchCaseInSensitive = [];
                    var lstCaseSenesitive = [];
                    var lstCaseInsensitive = [];
                    var lstContainsCaseSensitive = [];
                    var lstContainsCaseInSensitive = [];
                    var attributeName = "ID";
                    if (scope.showvalueofcolumn) {
                        attributeName = "Value";
                    }
                    for (var i = 0; i < clsEntity.Attributes.length; i++) {
                        if (clsEntity.Attributes[i][attributeName] == strText) {
                            lstExactMatchCaseSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase() == strText.toLowerCase()) {
                            lstExactMatchCaseInSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && clsEntity.Attributes[i][attributeName].indexOf(strText) == 0) {
                            lstCaseSenesitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase().indexOf(strText.toLowerCase()) == 0) {
                            lstCaseInsensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && clsEntity.Attributes[i][attributeName].contains(strText)) {
                            lstContainsCaseSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase().contains(strText.toLowerCase())) {
                            lstContainsCaseInSensitive.push(clsEntity.Attributes[i]);
                        }
                    }
                    var lst = lstExactMatchCaseSensitive.concat(lstExactMatchCaseInSensitive).concat(lstCaseSenesitive).concat(lstCaseInsensitive).concat(lstContainsCaseSensitive).concat(lstContainsCaseInSensitive);
                    clsEntity.SortedAttributes = lst;
                    if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                        scope.objEntityFilter.LimitCount = 50;
                    } else {
                        scope.objEntityFilter.LimitCount = clsEntity.Attributes.length;
                    }

                } else {
                    clsEntity.SortedAttributes = clsEntity.Attributes;
                    if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                        scope.objEntityFilter.LimitCount = 50;
                    } else {
                        scope.objEntityFilter.LimitCount = clsEntity.Attributes.length;
                    }
                }
            };

            scope.AddIntoSelectedList = function (field) {
                if (scope.ischeckboxvisible) {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    var displayName = displayEntity.strDisplayName;
                    fieldName = field.ID;
                    if (displayName != "") {
                        fieldName = displayName + "." + field.ID;
                    }
                    if (scope.lstselectedmultiplelevelfield) {
                        if (field.IsSelected == "True") {
                            var obj = { EntityField: fieldName, FieldObject: field, Entity: displayEntity.strID };
                            scope.lstselectedmultiplelevelfield.push(obj);
                        } else {
                            for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                                if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                                    scope.lstselectedmultiplelevelfield.splice(i, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            };

            scope.NavigateToEntity = function (entityID) {
                if (entityID && (scope.isallownavigation === undefined || scope.isallownavigation === true)) {
                    hubMain.server.navigateToFile(entityID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, undefined);
                    });
                }
            };
        }, templateUrl: "Common/views/EntityTreeListTemplate.html"
    };
}]);

app.directive('checkboxdirective', [function () {
    return {
        restrict: 'A',
        scope: {
            attributefield: '=',
            lstdisplayentity: '=',
            lstselectedmultiplelevelfield: '='
        },
        link: function (scope, element, attrs) {
            var ele = element[0];

            scope.InitSelectedField = function (field, element) {
                if (scope.lstselectedmultiplelevelfield) {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    var displayName = displayEntity.strDisplayName;
                    fieldName = field.ID;
                    if (displayName != "") {
                        fieldName = displayName + "." + field.ID;
                    }
                    if (field.DataType != "Object" && field.DataType != "Collection" && field.DataType != "CDOCollection" && field.DataType != "List") {
                        for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                            if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                                field.IsSelected = "True";
                                break;
                            }
                        }
                    } else {
                        if (field.DataType == "Object") {
                            scope.setValueToLoadedField(fieldName, element);
                        }
                    }
                }
            };
            //checking object fields are selected or not
            scope.setValueToLoadedField = function (fieldName, element) {
                for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                    if (scope.lstselectedmultiplelevelfield[i].EntityField.contains(fieldName)) {
                        element.indeterminate = true;
                        break;
                    }
                }
            };

            if (scope.lstselectedmultiplelevelfield) {
                scope.InitSelectedField(scope.attributefield, ele);
            }
        }
    };
}]);


app.directive('entitytreescroll', [function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            var scrollTopValue = 0;
            element.bind('scroll', function () {
                var diff = (raw.scrollTop + raw.offsetHeight) - raw.scrollHeight;
                if ((raw.scrollTop + raw.offsetHeight == raw.scrollHeight) || (diff > -5 && diff < 5)) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        if (scope.FormModel && scope.FormModel.IsLookupCriteriaEnabled) {
                            scope.ColumnsLimitCount += 10;
                        } else if (scope.objEntityFilter) {
                            scope.objEntityFilter.LimitCount += 10;
                        } else if (scope.objBusinessObjectFilter) {
                            scope.objBusinessObjectFilter.LimitCount += 10;
                        }
                    });
                }
            });
        }

    };
}]);


app.directive("entityobjectchildtreetemplate", ["$compile", function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Common/views/EntityTreeTemplate.html',
    };
}]);

app.directive('entitytreefielddraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
            dragobject: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                dragDropData = null;
                dragDropDataObject = null;
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData("Text", "");

                if (scope.dragdata) {
                    dragDropData = scope.dragdata;
                    dragDropDataObject = scope.dragobject;
                }
            }
        },
    };
}]);

app.directive('entitytreefielddroppable', ["$rootScope", function ($rootScope) {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            fieldname: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.addEventListener('dragover', handleDragOver, false);
            el.addEventListener('drop', handleDrop, false);
            el.addEventListener('dragleave', handleDragLeave, false);

            function handleDragOver(e) {
                if (e.preventDefault) {
                    e.preventDefault(); // Necessary. Allows us to drop.
                }

                e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                e.dataTransfer.setData("Text", "");
                e.preventDefault();
            }

            function handleDrop(e) {
                var strData = e.dataTransfer.getData("Text");
                // Stops some browsers from redirecting.
                if (e.stopPropagation) e.stopPropagation();

                if (strData == "" && lstEntityTreeFieldData != null) {
                    e.preventDefault();
                    var obj = lstEntityTreeFieldData;//JSON.parse(strData);
                    var Id = obj[0];
                    var displayName = obj[1];
                    if (displayName != "") {
                        Id = displayName + "." + Id;
                    }
                    dragDropData = Id;
                    // Stops some browsers from redirecting.
                    //if (e.stopPropagation) e.stopPropagation();
                    var ID = dragDropData;
                    if (ID != undefined && ID != null && ID != "") {
                        if (scope.dropdata) {
                            scope.$apply(function () {
                                scope.dropdata[scope.fieldname] = ID;
                            });
                        }
                    }

                    //$(e.target).blur();
                    lstEntityTreeFieldData = null;
                    dragDropData = null;
                } else {
                    if (dragDropData) {

                        var data = dragDropData;
                        if (data != undefined && data != null && data != "") {
                            if (scope.dropdata) {
                                scope.$apply(function () {
                                    scope.dropdata[scope.fieldname] = data;
                                });
                            }
                        }

                        dragDropData = null;
                    }
                    lstEntityTreeFieldData = null;
                    if (e && e.preventDefault) {
                        e.preventDefault();
                    }
                }
            }

            function handleDragLeave(e) {

            }
        }
    };
}]);

app.directive("contentEditable", ["$parse", "$compile", "$rootScope", function ($parse, $compile, $rootScope) {
    return {
        restrict: "A",
        scope: {
            model: "=",
            propertyname: "=",
        },
        require: "ngModel",
        link: function (scope, element, attrs, ctrlr) {
            var ngmodelname = attrs.ngModel;
            var modelname = attrs.model;
            var oldvalue;
            var propname = scope.propertyname;
            if (ngmodelname && modelname && ngmodelname.match("^" + modelname)) {
                propname = ngmodelname.substring(modelname.length + 1, ngmodelname.length);
            }
            var curscope;
            var curscopeDivId;

            var unwatch = scope.$watch('model', function (newVal, oldVal) {
                if (newVal) {
                    oldvalue = getPropertyValue(scope.model, propname);
                    if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileName) {
                        curscopeDivId = $rootScope.currentopenfile.file.FileName;
                        curscope = getScopeByFileName(curscopeDivId);
                    }
                    unwatch();
                }
            });


            ctrlr.$render = function () {
                element.text(ctrlr.$viewValue || "");
            };

            function read() {
                ctrlr.$setViewValue(element.text());
            }


            scope.$watch(attrs.ngModel, function () {
                if (ctrlr.$viewValue) {
                    ctrlr.$viewValue = ctrlr.$viewValue.trim();
                }
                //if (ctrlr.$viewValue != "" && ctrlr.$viewValue != undefined) {
                //    element.removeAttr('placeholder')
                //}
                //else if (ctrlr.$viewValue == "" || ctrlr.$viewValue == undefined) {
                //    if (!element.attr('placeholder')) {
                //        if (attrs.ngModel == "items.dictAttributes.Text") {
                //            element.attr("placeholder", "Description");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwExpression" || attrs.ngModel == "x.dictAttributes.sfwExpression" || attrs.ngModel == "parameter.dictAttributes.sfwExpression") {
                //            element.attr("placeholder", "Expression");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwReturnField") {
                //            element.attr("placeholder", "ReturnField");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwRuleID") {
                //            element.attr("placeholder", "RuleID");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwEffectiveDate") {
                //            element.attr("placeholder", "EffectiveDate");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwMethodName") {
                //            element.attr("placeholder", "MethodName");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwQueryID") {
                //            element.attr("placeholder", "QueryID");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwItemName") {
                //            element.attr("placeholder", "ItemName");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwObjectID") {
                //            element.attr("placeholder", "Collection");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwEntityField") {
                //            element.attr("placeholder", "EntityField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwDataField") {
                //            element.attr("placeholder", "DataField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwObjectField") {
                //            element.attr("placeholder", "ObjectField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.Text") {
                //            element.attr("placeholder", "Text");
                //        }
                //        //$compile(element[0])(scope);
                //    }
                //}
            });
            scope.$on("UpdateOnClick", function (e, data) {
                if (angular.equals(data, element[0])) {
                    scope.valueChange(element.text());
                }
            });


            scope.valueChange = function (text) {
                ctrlr.$setViewValue(text);
            };
            element.bind("keyup", function () {
                scope.$parent.$apply(read);
                //scope.$apply(attrs.textAction); 
                if (attrs.descriptionKeydown) {
                    scope.$parent.$apply(attrs.descriptionKeydown);
                }
                if (attrs.loopCollectionKeydown) {
                    var vrloopCollectionKeydown = $parse(attrs.loopCollectionKeydown);
                    vrloopCollectionKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.actionKeydown) {
                    var vractionKeydown = $parse(attrs.actionKeydown);
                    vractionKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.queryidKeydown) {
                    var vractionQueryIdKeydown = $parse(attrs.queryidKeydown);
                    vractionQueryIdKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.methodnameKeydown) {
                    var vractionMethodNameKeydown = $parse(attrs.methodnameKeydown);
                    vractionMethodNameKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.ruleidKeydown) {
                    var vractionRuleIdKeydown = $parse(attrs.ruleidKeydown);
                    vractionRuleIdKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.spanClick) {
                    var vrclickonspan = $parse(attrs.spanClick);
                    vrclickonspan(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.loopCollectionKeydown) {
                    scope.$apply(attrs.loopCollectionKeydown);
                }

                var newvalue = getPropertyValue(scope.model, propname);
                //newvalue = getRemovedStringStartingSpace(newvalue);
                //setPropertyValue(scope.model, propname, newvalue);
                if (oldvalue != newvalue) {

                    if (curscopeDivId != undefined) {
                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {
                            };
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;

                            sessionstorageitem.UndoList.push(undoitem);

                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                }

            });
            element.bind("keydown", function (eargs) {
                if (eargs.keyCode == $.ui.keyCode.ENTER) {
                    eargs.preventDefault();
                }
            });
        }
    };
}]);

app.directive("commonTreeDirective", [function () {
    return {
        restrict: "E",
        replace: true,
        scope: {
            collection: "=",
            childCollectionProperty: "=",
            textProperty: "=",
            expandedProperty: "=",
            onitemdblclickCallback: "=",
            onitemclickCallback: "=",
        },
        link: function (scope, element, attrs) {
            scope.getPropertyValue = getPropertyValue;
            scope.onItemDoubleClick = function (event, element) {
                if (scope.onitemdblclickCallback) {
                    scope.onitemdblclickCallback(event, element);
                }
            };
            scope.onItemClick = function (event, element) {
                scope.selectedItem = element;
                if (scope.onitemclickCallback) {
                    scope.onitemclickCallback(element, event);
                }
            };
            scope.toggleExpandCollapse = function (event, element) {
                setPropertyValue(element, scope.expandedProperty, !scope.getPropertyValue(element, scope.expandedProperty));
            };
            scope.getPropertyValue = getPropertyValue;
        },
        templateUrl: "Common/views/CommonTreeTemplate.html"
    };
}]);

app.directive("commonTreeChildDirective", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Common/views/CommonTreeItemsTemplate.html',
    };
}]);

app.directive("ruleitemcommontemplate", ["$compile", "$http", "$rootScope", function ($compile, $http, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            iscontroldisable: '=',
        },
        link: function (scope, element, attrs) {

        },
        templateUrl: function (elem, attrs) {
            return attrs.templateUrl || 'Form/views/RuleItemCommonTemplate.html';
        }
    };
}]);

app.directive("undoredodirective", ["$parse", "$compile", "$rootScope", function ($parse, $compile, $rootScope) {
    return {
        restrict: "A",
        scope: {
            model: "=",
            propertyname: "=",
            undorelatedfunction: "=",
        },
        link: function (scope, element, attrs) {
            var propname = "";
            var oldvalue = "";
            var curscopeDivId;
            var curscope;

            var unwatch = scope.$watch('model', function (newVal, oldVal) {
                if (newVal) {
                    var ngmodelname = attrs.ngModel;
                    var modelname = attrs.model;

                    propname = scope.propertyname;

                    if (!propname && ngmodelname && modelname && ngmodelname.match("^" + modelname)) {
                        propname = ngmodelname.substring(modelname.length + 1, ngmodelname.length);
                    }

                    if (scope.model) {
                        oldvalue = getPropertyValue(scope.model, propname);
                    }
                    if ($rootScope.isConfigureSettingsVisible) {
                        curscopeDivId = "SettingsPage";
                    }
                    else {
                        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
                            curscopeDivId = $rootScope.currentopenfile.file.FileName;
                        }
                    }
                    curscope = getCurrentFileScope();

                    unwatch();

                }
            });

            element.bind("focus", function () {
                if (scope.model) {
                    oldvalue = getPropertyValue(scope.model, propname);
                }
            });

            if (attrs.type == "checkbox" || attrs.type == "radio") {
                element.bind("click", function () {
                    var newvalue = getPropertyValue(scope.model, propname);

                    if (oldvalue != newvalue) {

                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {};
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;

                            $rootScope.$evalAsync(function () {
                                sessionstorageitem.UndoList.push(undoitem);
                            });

                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                });
            }
            else {

                element.bind("change", function () {
                    var newvalue = getPropertyValue(scope.model, propname);

                    if (oldvalue != newvalue) {

                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {};
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;
                            undoitem.undorelatedfunction = scope.undorelatedfunction;
                            $rootScope.$evalAsync(function () {
                                sessionstorageitem.UndoList.push(undoitem);
                            });
                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                });
            }
        }
    };
}]);

app.directive("datepicker", [function () {
    return {
        restrict: "A",
        link: function (scope, el, attr) {
            el.datepicker({
                dateFormat: 'm/d/yy',
                onClose: function (dateText, inst) {
                    $("#tbDescription").focus();
                }, autoclose: true
            });
        }
    };
}]);

app.directive('editableRow', ["$compile", function ($compile) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            if (attributes && attributes.editTemplateUrl && attributes.readOnlyTemplateUrl) {
                scope.isEditable = false;

                var readOnlyTemplateHtml = getHtmlFromServer(attributes.readOnlyTemplateUrl);
                if (readOnlyTemplateHtml) {
                    var editTemplateHtml = getHtmlFromServer(attributes.editTemplateUrl);
                    if (editTemplateHtml) {

                        $(element).keyup(function (e) {
                            if (e.keyCode == 27 && scope.isEditable && !(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                scope.setRowContentReadOnly();
                            }
                        });
                        $(element).dblclick(function () {
                            if (!scope.isEditable && !(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                scope.setRowContentEditable();
                            }
                        });
                        $(element).click(function () {
                            scope.resetAllButThis();
                        });
                        scope.addActionButton = function (actionType, replaceExisting) {
                            var anchor;
                            if (!(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                var anchor = $("<i style='cursor:pointer'></i>");
                                if (actionType == "edit") {
                                    anchor.addClass("fa fa-edit");
                                    anchor.click(function () {
                                        scope.setRowContentEditable();
                                    });
                                }
                                else {
                                    anchor.addClass("fa fa-close");
                                    anchor.click(function () {
                                        scope.setRowContentReadOnly();
                                    });
                                }
                            }
                            var td = $("<td></td>");
                            if (!(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                td.append(anchor);
                            }
                            $(element).append(td);
                        };
                        scope.resetAllButThis = function () {
                            //Set rest of the rows as read-only, when this row is getting edited.
                            var rows = $(element).siblings();
                            for (var index = 0; index < rows.length; index++) {
                                var rowScope = angular.element($(rows[index])).scope();
                                if (rowScope && (rowScope.isEditable === true || rowScope.isEditable === "true") && rowScope.setRowContentReadOnly) {
                                    rowScope.setRowContentReadOnly();
                                }
                            }
                        };

                        scope.setRowContentEditable = function (onLoad) {
                            scope.isEditable = true;
                            scope.setRowContent(editTemplateHtml);
                            if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                                scope.addActionButton("cancel", !onLoad);
                            }
                            scope.compileContent();
                        };
                        scope.setRowContentReadOnly = function (onLoad) {
                            scope.isEditable = false;
                            scope.setRowContent(readOnlyTemplateHtml);
                            if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                                scope.addActionButton("edit", !onLoad);
                            }
                            scope.compileContent();
                        };
                        scope.setRowContent = function (content) {
                            $(element).html(content);
                        };
                        scope.compileContent = function () {
                            $compile($(element).contents())(scope);
                        };

                        if (attributes && (attributes.isNew === true || attributes.isNew === "true")) {
                            scope.setRowContentEditable(true);
                        }
                        else {
                            scope.setRowContentReadOnly(true);
                        }

                        if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                            //Add extra th if not exists.
                            if ($(element).closest("table").find("thead tr").length > 1) {
                                var colHeaderRow = $(element).closest("table").find("thead tr[role='columnheader']");
                                if (colHeaderRow.find("th").length < $(element).find("td").length) {
                                    //Increase heading colspan.
                                    var heading = $(element).closest("table").find("thead tr[role!='columnheader'] th:last-child");
                                    if (heading && heading.length > 0) {
                                        var colspan = parseInt(heading.attr("colspan")) + 1;
                                        heading.attr("colspan", colspan.toString());
                                    }

                                    colHeaderRow.append("<th style='width:35px'>");
                                }
                            }
                            else {
                                var colHeaderRow = $(element).closest("table").find("thead tr");
                                if (colHeaderRow.find("th").length < $(element).find("td").length) {
                                    colHeaderRow.append("<th style='width:35px'>");
                                }
                            }
                        }
                    }
                    else {
                        console.log("invalid edit template url for the directive.");
                    }
                }
                else {
                    console.log("invalid read-only template url for the directive.");
                }
            }
            else {
                console.log("Please set edit/read-only template url for the directive.");
            }
        }
    };
}]);

app.directive('editablelabletotext', ["$compile", function ($compile) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            var ele;
            scope.onChangeHandler = function () {
                var text = ele.val();
                growtextbox(ele, true, text);
            };
            $(element).dblclick(function () {
                ele = $("<input type='text' ng-model='model.dictAttributes.sfwEntityField' ng-change='onChangeHandler()' ng-keydown='onActionKeyDown($event)' class='temptext' undoredodirective model='model' />");
                element.after(ele);
                $compile(ele)(scope);
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwEntityField) {
                    growtextbox(ele, true, scope.model.dictAttributes.sfwEntityField);
                }
                element.hide();
                ele.blur(function () {
                    element.show();
                    $(this).remove();
                });
                ele.focus();
            });
        }
    };
}]);

app.directive('wizardTitleUpdate', ["WizardHandler", function (WizardHandler) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            scope.wizardHandler = WizardHandler;
            if (!scope.onNext) {
                scope.onNext = function () {
                    if (scope.setTitle) {
                        var steps = scope.wizardHandler.wizard().getEnabledSteps();
                        var nextStepIndex = steps.indexOf(scope.wizardHandler.wizard().currentStep()) + 1;
                        if (nextStepIndex > -1 && nextStepIndex < steps.length) {
                            var title = steps[nextStepIndex].wzTitle;
                            scope.setTitle(title);
                        }
                    }
                };
            }
            if (!scope.onPrevious) {
                scope.onPrevious = function () {
                    if (scope.setTitle) {
                        var steps = scope.wizardHandler.wizard().getEnabledSteps();
                        var previousStepIndex = steps.indexOf(scope.wizardHandler.wizard().currentStep()) - 1;
                        if (previousStepIndex > -1 && previousStepIndex < steps.length) {
                            var title = steps[previousStepIndex].wzTitle;
                            scope.setTitle(title);
                        }
                    }
                };
            }

            var wizardUnwatch = scope.$watchCollection(function () { return scope.wizardHandler.wizard().getEnabledSteps(); }, function (newval, oldval) {
                if (newval && newval.length > 0) {
                    if (scope.setTitle) {
                        var title = scope.wizardHandler.wizard().currentStep().wzTitle;
                        scope.setTitle(title);
                    }
                    wizardUnwatch();
                }
            });
        }
    };
}]);

app.directive("loadingBox", [function () {
    return {
        restrict: "E",
        scope: {
            lazymodel: "=",
            size: "@"
        },
        link: function (scope, element, attribute) {
            var loadwatch = scope.$watch('lazymodel', function (newval, oldvalue) {
                var elhover = '<div class="cs-loader-inner"><div><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label></div></div>';
                if (scope.size && scope.size == 'xs') {
                    elhover = '<div class="cs-loader-inner cs-loader-xs"><div><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label></div></div>';
                }
                if (newval == undefined) {
                    element.append(elhover);
                }
                else {
                    element.find(".cs-loader-inner").remove();
                    loadwatch();
                }
            });
        }
    };
}]);

app.directive("searchSource", ["$rootScope", "$SearchSource", function ($rootScope, $SearchSource) {
    return {
        restrict: "E",
        scope: {
            SearchSource: "=searchSource"
        },
        templateUrl: "Common/views/SearchSourceWrapper.html",
        link: function (scope, element, attribute) {
            scope.onsearchtextchange = function () {
                if (scope.searchtext == "") {
                    scope.SearchSource.searchFindarray = [];
                    scope.SearchSource.searchSourceText = "";
                }
            };
            scope.SearchCriteriaChange = function (criteria) {
                criteria = criteria ? criteria : "";
                scope.searchfocus = true;
                switch (criteria.toLowerCase()) {
                    case "case": scope.SearchSource.IsCaseSensitive = !scope.SearchSource.IsCaseSensitive; break;
                    case "word": scope.SearchSource.IsWholeMatchWord = !scope.SearchSource.IsWholeMatchWord; break;
                    case "regex": scope.SearchSource.IsSearchRegex = !scope.SearchSource.IsSearchRegex; break;
                    default: break;
                }
                if (criteria) {
                    scope.SearchSource.IsSearchCriteriaChange = true;
                    scope.SearchSourceFind();
                }
            };
            scope.SearchSourceFind = function () {
                // new search 
                // blur search div when user clicks 
                scope.searchfocus = false;
                if (scope.searchtext) {
                    // fresh search - when search text is changed / criteria is changed
                    if (scope.SearchSource.searchSourceText != scope.searchtext || (scope.SearchSource.IsSearchCriteriaChange)) {
                        scope.SearchSource.IsSearchCriteriaChange = false;
                        scope.SearchSource.searchSourceText = scope.searchtext;
                        scope.SearchSource.searchFindarray = $SearchSource.getIndicesOfSearchStr(scope.searchtext, scope.SearchSource.txtarea[0].value, scope.SearchSource.IsCaseSensitive, scope.SearchSource.IsWholeMatchWord, scope.SearchSource.IsSearchRegex);
                        if (scope.SearchSource.searchFindarray.length > 0) {
                            scope.SearchSourceMatch(true);
                        }
                    }
                    // next search - on search button
                    else if ((scope.SearchSource.searchFindarray.length - scope.SearchSource.searchfindindex) > 0) {
                        scope.SearchSourceMatch(false, true);
                    }
                }
            };
            scope.SearchSourceMatch = function (isNew, isNext, isPrevious) {
                // handle previous and next 
                var currentIndex = 0;
                if (isNew) scope.SearchSource.searchfindindex = 1; // when search text changes -- index becomes 1  
                else if (!isNew && isNext) scope.SearchSource.searchfindindex += 1;
                else if (!isNew && isPrevious) scope.SearchSource.searchfindindex -= 1;
                currentIndex = scope.SearchSource.searchfindindex - 1;
                if (currentIndex >= 0) {
                    scope.SearchSource.sourceSearchlineno = scope.SearchSource.txtarea[0].value.substr(0, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchFindarray[currentIndex] : scope.SearchSource.searchFindarray[currentIndex].startPos).split("\n").length;
                    $SearchSource.selectword(scope.SearchSource.txtarea, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchFindarray[currentIndex] : scope.SearchSource.searchFindarray[currentIndex].startPos, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchSourceText : scope.SearchSource.txtarea[0].value.substring(scope.SearchSource.searchFindarray[currentIndex].startPos, scope.SearchSource.searchFindarray[currentIndex].endPos), scope.SearchSource.sourceSearchlineno);
                    scrollTextArea(scope.SearchSource.txtarea, scope.SearchSource.sourceSearchlineno, scope.SearchSource.searchSourceText);
                }
            };
            scope.CloseSearchSource = function () {
                scope.SearchSource.IsCaseSensitive = false;
                scope.SearchSource.IsWholeMatchWord = false;
                scope.SearchSource.IsSearchRegex = false;
                scope.SearchSource.searchFindarray = [];
                scope.SearchSource.searchSourceText = "";
                scope.SearchSource.IsEnable = false;
                scope.SearchSource.txtarea.focus();
            };
            scope.SearchSource.txtarea[0].onkeydown = function (event) {
                if (event.ctrlKey && event.keyCode == 70) {
                    scope.$evalAsync(function () {
                        scope.SearchSource.IsEnable = true;
                    });
                    scope.SearchSourceFindBySelection();
                    event.preventDefault();
                }
                if (event.shiftKey && event.keyCode == 114) {
                    if (scope.SearchSource.searchfindindex - 1 > 0) scope.SearchSourceMatch(false, false, true);
                    event.preventDefault();
                }
                else if (event.keyCode == 114) {
                    // for next
                    if ((scope.SearchSource.searchFindarray.length - scope.SearchSource.searchfindindex) > 0) scope.SearchSourceMatch(false, true);
                    event.preventDefault();
                }
            };
            scope.inputKeydownCallback = function (e) {
                // for enter search
                var charCode = (e.which) ? e.which : e.keyCode;
                if (charCode == 13) {
                    scope.SearchSourceFind();
                    e.preventDefault();
                }
            };
            scope.inputpasteCallback = function (e) {
                setTimeout(function () {
                    scope.searchtext = (e.target) ? e.target.value : "";
                    if (scope.searchtext) {
                        scope.SearchSourceFind();
                    }
                }, 0); //or 4
            };
            scope.SearchSourceFindBySelection = function () {
                // if anything selected from textarea it should be searched by default on load of this directive
                if (typeof scope.SearchSource.txtarea[0].selectionStart == "number" && typeof scope.SearchSource.txtarea[0].selectionEnd == "number") {
                    if (scope.SearchSource.txtarea[0].selectionStart < scope.SearchSource.txtarea[0].selectionEnd) {
                        scope.searchtext = scope.SearchSource.txtarea[0].value.substring(scope.SearchSource.txtarea[0].selectionStart, scope.SearchSource.txtarea[0].selectionEnd);
                        scope.SearchSourceFind();
                    }
                }
            };
            scope.init = function () {
                // focus should be on input
                scope.inputelement = $("#" + $rootScope.currentopenfile.file.FileName + ' .searchSource').find("input[type='text']").focus();
                scope.inputelement.focus();
                scope.SearchSourceFindBySelection();
            };
            scope.init();
        }
    };
}]);

app.directive("extraFieldDirective", ["$rootScope", "$filter", function ($rootScope, $filter) {
    return {
        restrict: "E",
        scope: {
            extraFieldModel: "=",
            showExtraFieldsTab: "=isTab",
            objFn: "=",
            formName: "=",
            isCretaeNewObjectSteps: "=isSteps",
            closeWizardFn: "=",
            validateFn: "=",
            isFileCreation: "="
        },
        templateUrl: "Common/views/ExtraFieldsOnDetails.html",
        link: function (scope, elem, attr) {
            // # region init section
            scope.objExtraFields = [];
            scope.ID = undefined;

            //hub call for get extra fields 

            scope.onFormChange = function () {
                scope.extraFields = [];
                scope.objExtraFields = [];
                scope.IsBaseAppFile = false;
                if (!scope.isFileCreation && $rootScope.currentfile && $rootScope.currentopenfile.file) {
                    scope.IsBaseAppFile = $rootScope.currentopenfile.file.IsBaseAppFile;
                }
                hubMain.server.getExtraSettingsModel().done(function (data) {
                    scope.$apply(function () {
                        if (data) {
                            var objCustomSettings = data;
                            if (objCustomSettings && objCustomSettings.Elements) {
                                scope.extraFields = $filter('filter')(objCustomSettings.Elements, { Name: scope.formName })[0];
                                if (scope.extraFields && scope.extraFields.Elements && scope.extraFields.Elements.length > 0) {
                                    scope.showExtraFieldsTab = true;
                                    if (scope.objFn && scope.objFn.showExtraFields) scope.objFn.showExtraFields(true);
                                    scope.init();
                                } else {
                                    scope.showExtraFieldsTab = false;
                                    if (scope.objFn && scope.objFn.showExtraFields) scope.objFn.showExtraFields(false);
                                }
                            }
                        }
                    });
                });
            };
            scope.init = function () {
                scope.createExtraFieldList();

            };

            scope.createExtraFieldList = function (model) {
                if (model) {
                    scope.extraFieldModel = model;
                }
                if (scope.extraFields != undefined && scope.extraFields.Elements) {
                    scope.objExtraFields = [];
                    for (var i = 0; i < scope.extraFields.Elements.length; i++) {
                        if (scope.extraFields.Elements[i].dictAttributes.value) {
                            var dummyobj = {
                                ID: scope.extraFields.Elements[i].dictAttributes.value, Description: scope.extraFields.Elements[i].dictAttributes.Description, ControlType: scope.extraFields.Elements[i].dictAttributes.ControlType, Children: [], IsRequired: scope.extraFields.Elements[i].dictAttributes.IsRequired
                            };

                            // if (scope.isCretaeNewObjectSteps == "false" || scope.isCretaeNewObjectSteps == false) {
                            dummyobj.Value = GetExtraFieldValue(dummyobj.ID);
                            //   }

                            if (dummyobj.ControlType == "HyperLink") {
                                dummyobj.URL = undefined;
                                if (scope.extraFieldModel) {
                                    for (var m = 0; m < scope.extraFieldModel.Elements.length; m++) {
                                        if (scope.extraFieldModel.Elements[m].dictAttributes.ID == dummyobj.ID) {
                                            dummyobj.URL = scope.extraFieldModel.Elements[m].dictAttributes.URL;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (scope.extraFields.Elements[i].Elements.length > 0) {
                                for (var j = 0; j < scope.extraFields.Elements[i].Elements.length; j++) {
                                    var dummyobjchild = {
                                        ID: scope.extraFields.Elements[i].Elements[j].dictAttributes.Text, Description: scope.extraFields.Elements[i].Elements[j].dictAttributes.Description
                                    };
                                    if (dummyobj.ControlType == "CheckBoxList") {
                                        if (dummyobj.Value != undefined) {
                                            var val = dummyobj.Value.split(',');
                                            for (var k = 0; k < val.length; k++) {
                                                if (dummyobjchild.ID == val[k]) {
                                                    dummyobjchild.Value = "True";
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        if (dummyobjchild.ID == dummyobj.Value) {
                                            dummyobjchild.Value = true;
                                            scope.ID = dummyobj.Value;
                                        }
                                        else {
                                            dummyobjchild.Value = false;
                                        }
                                    }

                                    dummyobj.Children.push(dummyobjchild);
                                }
                            }
                            scope.objExtraFields.push(dummyobj);
                        }
                    }
                    if (scope.objExtraFields && scope.objExtraFields.length <= 0) {
                        scope.showExtraFieldsTab = false;
                    }
                }

            };


            //#endregion init

            var GetExtraFieldValue = function (ID) {
                if (scope.extraFieldModel) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i].dictAttributes.ID == ID) {
                            return scope.extraFieldModel.Elements[i].dictAttributes.Value;
                            break;
                        }
                    }
                }
            };

            // #region common function for extra fields

            scope.selectExtraFieldRow = function (obj) {
                scope.SelectExtraFieldRow = obj;
            };

            scope.AddHyperLinkUrl = function (obj) {
                var newHyperlinkScope = scope.$new();
                newHyperlinkScope.oldData = {};
                newHyperlinkScope.currentobjHyperLink = obj;
                var temp = JSON.stringify(obj);
                newHyperlinkScope.objHyperLink = JSON.parse(temp);

                newHyperlinkScope.oldData.url = newHyperlinkScope.objHyperLink.URL;
                newHyperlinkScope.oldData.value = newHyperlinkScope.objHyperLink.Value;

                newHyperlinkScope.validateHyperLink = function () {
                    newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = undefined;
                    if ((newHyperlinkScope.objHyperLink.Value == undefined || newHyperlinkScope.objHyperLink.Value == "")) {
                        newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Enter the Description.";
                        return true;
                    } else if (newHyperlinkScope.objHyperLink.URL == undefined) {
                        newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                        if (newHyperlinkScope.currentobjHyperLink.IsRequired == "True" || newHyperlinkScope.currentobjHyperLink.IsRequired == true) {
                            newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                        }
                        return true;
                    } else {
                        var match = newHyperlinkScope.objHyperLink.URL.match(new RegExp(/^http(s)?:\/\/(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?/));
                        if (match == null) {
                            newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                            if (newHyperlinkScope.currentobjHyperLink.IsRequired == "True" || newHyperlinkScope.currentobjHyperLink.IsRequired == true) {
                                newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                            }
                            return true;
                        }
                    }
                };

                newHyperlinkScope.closeEntityDetailsHyperlink = function () {
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].ID == newHyperlinkScope.currentobjHyperLink.ID) {
                            newHyperlinkScope.currentobjHyperLink.URL = newHyperlinkScope.objHyperLink.URL;
                            newHyperlinkScope.currentobjHyperLink.Value = newHyperlinkScope.objHyperLink.Value;
                        }
                    }
                    newHyperlinkScope.dialog.close();
                };

                newHyperlinkScope.cancelHyperlinkDialog = function () {
                    newHyperlinkScope.objHyperLink.URL = newHyperlinkScope.oldData.url;
                    newHyperlinkScope.objHyperLink.Value = newHyperlinkScope.oldData.value;
                    newHyperlinkScope.dialog.close();
                };

                newHyperlinkScope.dialog = $rootScope.showDialog(newHyperlinkScope, "Add Hyperlink Url", "Entity/views/AddHyperLinkUrl.html", { width: 400, height: 500 });

            };

            if (scope.objFn) {
                scope.objFn.getExtraFieldData = function () {
                    return scope.objExtraFields;
                };
                scope.objFn.prepareExtraFieldData = function () {
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].ControlType == "TextBox" || scope.objExtraFields[i].ControlType == "CheckBox" || scope.objExtraFields[i].ControlType == "ComboBox") {
                            scope.updateExtraFieldValue(scope.objExtraFields[i]);
                        }
                        if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                            scope.updateExtraFieldValue(scope.objExtraFields[i]);
                        }
                        if (scope.objExtraFields[i].ControlType == "HyperLink") {
                            scope.updateExtraFieldHyperLinkValue(scope.objExtraFields[i]);
                        }
                    }

                    if (scope.extraFieldModel && scope.extraFieldModel.Elements && scope.extraFieldModel.Elements.length > 0 /* && $rootScope.currentopenfile
                        && ($rootScope.currentopenfile.file.FileType == "DecisionTable" || $rootScope.currentopenfile.file.FileType == "BPMN" || $rootScope.currentopenfile.file.FileType == "BPMTemplate")
                       && (scope.isCretaeNewObjectSteps == "true" || scope.isCretaeNewObjectSteps == true)*/) {
                        var obj;
                        var arrObj = [];
                        for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                            obj = {};
                            obj.ID = scope.extraFieldModel.Elements[i].dictAttributes.ID;
                            obj.Value = scope.extraFieldModel.Elements[i].dictAttributes.Value;
                            obj.URL = scope.extraFieldModel.Elements[i].dictAttributes.URL;
                            arrObj.push(obj);
                        }
                        scope.newExtraFieldModel = arrObj;
                    }
                };

                scope.objFn.getNewExtraFieldData = function () {
                    return scope.newExtraFieldModel;
                };

                scope.objFn.validateExtraFieldsData = function () {
                    scope.extraFieldsErrorMessage = undefined;
                    var flag = false;
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].IsRequired != undefined && (scope.objExtraFields[i].IsRequired == "True" || scope.objExtraFields[i].IsRequired == true) && (!scope.objExtraFields[i].Value)) {
                            scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                            flag = false;
                            if (scope.objExtraFields[i].ControlType == "ComboBox") {
                                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                                    if (scope.objExtraFields[i].Children[j].Value != undefined && scope.objExtraFields[i].Children[j].Value != "") {
                                        flag = true;
                                        scope.extraFieldsErrorMessage = undefined;
                                    }
                                }
                                if (flag == false) {
                                    scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                                    return true;
                                }
                            }
                            if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                                var flagChk = false;
                                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                                    if (scope.objExtraFields[i].Children[j].Value != undefined && scope.objExtraFields[i].Children[j].Value != "") {
                                        flagChk = true;
                                        flag = true;
                                        scope.extraFieldsErrorMessage = undefined;
                                    }
                                }
                                if (flagChk == false) {
                                    scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                                    return true;
                                }
                            }

                            if (flag == false) {
                                return true;
                            }
                        }

                    }
                };

                scope.objFn.checkVisibility = function () {
                    return scope.showExtraFieldsTab;
                };
                scope.objFn.resetExtraField = function (model) {
                    scope.createExtraFieldList(model);
                };
            }

            scope.updateExtraFieldValue = function (textObject) {
                var flag = false;
                if (scope.extraFieldModel && scope.extraFieldModel.Elements && scope.extraFieldModel.Elements.length > 0) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i] && scope.extraFieldModel.Elements[i].dictAttributes.ID == textObject.ID) {
                            var temp;
                            if (textObject.ControlType == "CheckBoxList") {
                                temp = CreateNewObjectForExtraFieldlist(textObject);
                            } else {
                                temp = CreateNewObjectForExtraField(textObject);
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.Value = temp.dictAttributes.Value;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.Value, scope.extraFieldModel.Elements[i].dictAttributes, "Value", temp.dictAttributes.Value);
                            }

                            flag = true;
                        }
                    }
                }
                if (flag == false) {
                    var obj;
                    if (textObject.ControlType == "CheckBoxList") {
                        obj = CreateNewObjectForExtraFieldlist(textObject);
                        if (scope.isFileCreation) {
                            scope.extraFieldModel.Elements.push(obj);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.extraFieldModel.Elements);
                        }
                    } else if (textObject.Value != undefined) {
                        obj = CreateNewObjectForExtraField(textObject);
                        if (scope.isFileCreation) {
                            scope.extraFieldModel.Elements.push(obj);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.extraFieldModel.Elements);
                        }
                    }
                }
            };

            scope.updateExtraFieldHyperLinkValue = function (textObject) {
                var flag = false;
                if (scope.extraFieldModel && scope.extraFieldModel.Elements) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i].dictAttributes.ID == textObject.ID) {
                            var objItem = {
                                Name: "ExtraField", dictAttributes: {}, Elements: []
                            };
                            objItem.dictAttributes.ID = textObject.ID;
                            if (textObject.Value != undefined) {
                                objItem.dictAttributes.Value = textObject.Value;
                            }
                            if (textObject.URL != undefined) {
                                objItem.dictAttributes.URL = textObject.URL;
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.Value = objItem.dictAttributes.Value;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.Value, scope.extraFieldModel.Elements[i].dictAttributes, "Value", objItem.dictAttributes.Value);
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.URL = objItem.dictAttributes.URL;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.URL, scope.extraFieldModel.Elements[i].dictAttributes, "URL", objItem.dictAttributes.URL);
                            }

                            flag = true;
                        }
                    }
                }
                if (flag == false) {
                    var objItem = {
                        Name: "ExtraField", dictAttributes: {}, Elements: []
                    };
                    objItem.dictAttributes.ID = textObject.ID;
                    if (textObject.Value != undefined) {
                        objItem.dictAttributes.Value = textObject.Value;
                    }
                    if (textObject.URL != undefined) {
                        objItem.dictAttributes.URL = textObject.URL;
                    }
                    if (scope.isFileCreation) {
                        scope.extraFieldModel.Elements.push(objItem);
                    }
                    else {
                        $rootScope.PushItem(objItem, scope.extraFieldModel.Elements);
                    }
                }
            };

            var CreateNewObjectForExtraField = function (textObject) {
                var objItem = {
                    Name: "ExtraField", dictAttributes: {}, Elements: []
                };

                objItem.dictAttributes.ID = textObject.ID;
                if (textObject.Value) {
                    objItem.dictAttributes.Value = textObject.Value;
                }
                return objItem;
            };

            var CreateNewObjectForExtraFieldlist = function (textObject) {
                var val = "";
                var count = 0;
                for (var j = 0; j < textObject.Children.length; j++) {
                    if (textObject.Children[j].Value == "True") {
                        if (count == 0) {
                            val += textObject.Children[j].ID;
                        }
                        else {
                            val += "," + textObject.Children[j].ID;
                        }
                        count++;
                    }
                }

                var objItem = {
                    Name: "ExtraField", dictAttributes: {}, Elements: []
                };
                objItem.dictAttributes.ID = textObject.ID;
                if (val != "") {
                    objItem.dictAttributes.Value = val;
                }
                return objItem;
            };

            scope.CloseWizard = function () {
                scope.closeWizardFn();
            };

            scope.$watch("formName", function (newVal, oldVal) {
                if (newVal) {
                    scope.onFormChange();
                }
            });

            // #endregion 
        }
    };
}]);

app.directive("searchDesign", ["hubcontext", "$timeout", "$searchQuerybuilder", "$rootScope", "$searchFindReferences", "$interval", function (hubcontext, $timeout, $searchQuerybuilder, $rootScope, $searchFindReferences, $interval) {
    return {
        restrict: "E",
        scope: {
            toggleDesign: "&",
            traverseModel: "&",
            currentfile: "=currentFile",
            selectElementCallback: "&selectElement",
            mainModel: "="
        },
        templateUrl: "Common/views/SearchDesignWrapper.html",
        link: function (scope, element, attribute) {
            scope.deHighlightDesign = function (HighLightList) {
                if (HighLightList && HighLightList.length > 0) {
                    HighLightList.forEach(function (HighLightObj) {
                        if (HighLightObj.Objhighlight && HighLightObj.Objhighlight.length > 0) {
                            scope.toggleDesign({ items: HighLightObj.Objhighlight, IsAction: false });
                        }
                    });
                }
            };
            scope.highlightDesign = function (File) {
                var items = [];
                if (File.FileInfo.SelectNodePath != null && File.FileInfo.SelectNodePath.length > 1) {
                    if (scope.currentfile.FileType == "DecisionTable") {
                        items = scope.traverseModel({ path: File.FileInfo.SelectNodePath, strPath: File.ParentPath });
                    }
                    else {
                        items = scope.traverseModel({ path: File.FileInfo.SelectNodePath });
                    }
                    if (items.length > 0) {
                        scope.toggleDesign({ items: items, IsAction: true });
                    }
                }
                return items;
            };
            scope.SearchInDesign = function () {
                $rootScope.IsLoading = true;
                hubcontext.hubSearch.server.getFindAdvanceDesign(scope.DesignSearch.SearchObj, scope.currentfile).done(function (DesignNodes) {
                    scope.deHighlightDesign(scope.DesignSearch.HighLightList);
                    scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                        scope.DesignSearch.findSearchList = DesignNodes;
                        // grey effect comes here                
                        // iterate on each type
                        if (scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch) {
                            angular.forEach(scope.DesignSearch.findSearchList, function (item) {
                                // save a dictionary with path and object to traverse to de-highlight properties
                                var HighlightListItem = new scope.HighLightObj();
                                HighlightListItem.path = item.FileInfo.SelectNodePath;
                                HighlightListItem.Objhighlight = scope.highlightDesign(item);
                                scope.DesignSearch.HighLightList.push(HighlightListItem);
                            });
                        }
                        // if result is only one node - select it directly
                        if (DesignNodes.length > 0) {
                            scope.selectElement(DesignNodes[0]);
                        }
                        scope.queryBuilder.setQueryBuilderIsAdvanceOption(true);
                    });
                });
            };
            scope.selectElement = function (File) {
                scope.DesignSearch.SelectNodePath = File.FileInfo.SelectNodePath;
                if (scope.currentfile.FileType == "DecisionTable") {
                    scope.selectElementCallback({ path: File.ParentPath, strPath: File.FileInfo.SelectNodePath });
                }
                else {
                    scope.selectElementCallback({ path: File.FileInfo.SelectNodePath });
                }
            };
            scope.ClearSearchDesign = function () {
                scope.deHighlightDesign(scope.DesignSearch.HighLightList);
                scope.init();
            };
            // key defines when the operator is active - mode A for attribute, mode N for Node. "A" has precedency           
            scope.HighLightObj = function () {
                return { path: "", Objhighlight: {} };
            };
            scope.initAdvance = function () {
                // when file is opened
                // this handles scenario where you want to search with a query and get the references from the hub (ex - advance search, build rule navigation)
                if (scope.queryBuilder.getGlobalQueryBuilder.lstFilter.length > 0) {
                    if (!scope.DesignSearch) {
                        scope.DesignSearch = {};
                        scope.DesignSearch.HighLightList = [];
                        scope.DesignSearch.SearchObj = [];
                    }
                    $rootScope.IsLoading = true;
                    scope.DesignSearch.IsOpen = scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch ? true : false;
                    angular.copy(scope.queryBuilder.getGlobalQueryBuilder.lstFilter, scope.DesignSearch.SearchObj);
                    scope.queryBuilder.setGlobalQueryBuilderFilters([]);
                    scope.SearchInDesign();
                }
            };
            scope.init = function () {
                scope.queryBuilder = $searchQuerybuilder;
                scope.searchFindReferences = $searchFindReferences.getData;
                var deregisterActiveReferenceWatcher;
                // if file is open from advance search - trigger search on init
                if (scope.queryBuilder.getGlobalQueryBuilder.lstFilter.length <= 0) {
                    var isOpen = scope.DesignSearch && scope.DesignSearch.IsOpen && scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch ? true : false;
                    scope.DesignSearch = {};
                    scope.DesignSearch.IsOpen = isOpen ? true : false;
                    scope.DesignSearch.SearchObj = [];
                    scope.DesignSearch.HighLightList = [];
                    $timeout(scope.queryBuilder.AddsearchQuerybuilderObj(scope.DesignSearch.SearchObj), 1000); // add one query by default
                }

                // If we add a left container after render, we need to watch and react
                deregisterActiveReferenceWatcher = scope.$watch(function () { return scope.searchFindReferences.activeReference; }, function (newValue, oldValue) {
                    if (!newValue || (($searchFindReferences.getData.activeReference && $searchFindReferences.getData.activeReference.FileInfo.FileName != scope.currentfile.FileName))) {
                        return;
                    }
                    var promise;
                    if ($searchFindReferences.getData.lstReferences.length > 0 && $searchFindReferences.getData.activeReference && ($searchFindReferences.getData.activeReference.FileInfo.FileName == scope.currentfile.FileName)) {
                        function checkBaseModelIsLoaded() {
                            if (scope.mainModel) {
                                $interval.cancel(promise);
                                var activeRef = angular.copy($searchFindReferences.getData.activeReference);
                                $searchFindReferences.setData(null, null, null);
                                scope.selectElement(activeRef);
                            }
                        }
                        promise = $interval(checkBaseModelIsLoaded, 1000);
                    }
                }, true);

                // Unbind when the file is closed
                element.on('$destroy', function () {
                    deregisterActiveReferenceWatcher();
                });

            };
            scope.init();
        }
    };
}]);

// now this directive we are not using - we are using title for display error
app.directive("displayErrors", ["$compile", "$ValidationService", "$timeout", function ($compile, $ValidationService, $timeout) {
    return {
        restrict: 'A',
        scope: {
            currObj: '='
        },
        link: function (scope, elem, attr) {

            scope.showErrors = function (event) {
                $(".duplicate-id-tooltip").remove();
                var htmlText = ['<div ng-if="currObj.errors" class="duplicate-id-tooltip">',
                    '<ul class="list-unstyled" margin-bottom: 0px!important;>',
                    '<li ng-repeat="(property, value) in currObj.errors">',
                    '<span error="{{value}}"></span>',
                    '</li>',
                    '</ul>',
                    '</div>'].join(' ');
                $('body').append($compile(htmlText)(scope));
                $timeout(function () {
                    var selector = $(".duplicate-id-tooltip");
                    if (selector.length > 0) selector.show();

                    var xVal = event.pageX;
                    var yVal = event.pageY;
                    var windowWidth = $(window).width(); //retrieve current window width
                    var windowHeight = $(window).height(); //retrieve current window height
                    //var documentWidth = $(document).width(); //retrieve current document width
                    //var documentHeight = $(document).height(); //retrieve current document height

                    var divSelector = selector[0].getBoundingClientRect();

                    if ((xVal + divSelector.width) >= windowWidth) {
                        xVal -= divSelector.width;
                    }
                    if ((yVal + divSelector.height) >= windowHeight) {
                        yVal -= divSelector.height;
                    }

                    selector.css({
                        display: "block",
                        left: xVal + 'px',
                        top: yVal + 'px',
                        "z-index": 10000
                    });
                });

            };
            scope.isEmpty = function (obj) {
                var flag = false;
                flag = $ValidationService.isEmptyObj(obj);
                return flag;
            };
            // scope.$apply();
        },
        template: function (elem, attr) {
            var htmlText = ['<span class="info-tooltip error-mark" ng-click="showErrors($event)" ng-if="isEmpty(currObj.errors)" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>'];
            return htmlText;
        }
        //template: function (elem, attr) {
        //    var htmlText = ['<div ng-if="currObj.errors" class="duplicate-id-tooltip">',
        //                        '<ul class="list-unstyled" margin-bottom: 0px!important;>',
        //                            '<li ng-repeat="(property, value) in currObj.errors">',
        //                               '<span> {{ value }}  </span>',
        //                            '</li>',
        //                        '</ul>',
        //                     '</div>'].join(' ');
        //  //  $(document).find('body').append($compile(htmlText));
        //    return htmlText;
        //}
    };
}]);

app.directive("showErrors", ["$ValidationService", function ($ValidationService) {
    return {
        restrict: 'A',
        scope: {
            model: '='
        },
        link: function (scope, elem, attr) {
            scope.getErrorMessages = function (obj) {
                var text = "";
                for (var key in obj) {
                    var value = obj[key];
                    if (value && angular.isObject(value)) {
                        value = scope.getErrorMessages(value);
                    }
                    text += value + "\n";
                }
                return text;
            };
            scope.isEmpty = function (obj) {
                var flag = false;
                flag = $ValidationService.isEmptyObj(obj);
                return flag;
            };
        },
        template: function (elem, attr) {
            var htmlText = ["<span class='error-mark' ng-if='isEmpty(model.errors)' ng-attr-title='{{ getErrorMessages(model.errors) }}' style='color:red!important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span>"];
            return htmlText;
        }
    };
}]);
app.directive("showErrorMessage", ["$ValidationService", function ($ValidationService) {
    return {
        restrict: 'A',
        scope: {
            model: '=',
            prefix: '='
        },
        link: function (scope, elem, attr) {
            scope.getErrorMessages = function (obj) {
                var text = "";
                for (var key in obj) {

                    var value = obj[key];
                    if (value && angular.isObject(value)) {
                        value = scope.getErrorMessages(value);
                    }
                    if (key && key.startsWith(scope.prefix)) {
                        text += value + "\n";
                    }
                }
                return text;
            };
            scope.isEmpty = function (obj) {
                var flag = false;
                for (var key in obj) {
                    if (key && key.startsWith(scope.prefix)) {
                        flag = true;
                    }
                }
                return flag;
            };
        },
        template: function (elem, attr) {
            var htmlText = ["<span class='info-tooltip' ng-if='isEmpty(model.errors)' ng-attr-title='{{ getErrorMessages(model.errors) }}' style='color:red!important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span>"];
            return htmlText;
        }
    };
}]);
/*************
  USE Directive 
  1. with px: Ex- <input type="text" class="form-control input-sm" title="{{model.dictAttributes.Width}}" ng-model='model.dictAttributes.Width' postfix="px" numbers-only undoredodirective model='model'>
  2. without px: Ex- <input type="text" class="form-control input-sm" title="{{model.dictAttributes.Width}}" ng-model='model.dictAttributes.Width' numbers-only undoredodirective model='model'>
****************/
app.directive('numbersOnly', function () {
    return {
        link: function (scope, element, attr, ngModelCtrl) {
            var units = ["em", "ex", "%", "px", "cm", "mm", "in", "pt", "pc", "ch", "rem", "vh", "vw", "vmin", "vmax"];
            element.on('keydown keyup', function (event) {
                var input = $(event.target);
                var text;
                text = input.val();
                var key = event.keyCode ? event.keyCode : event.which;
                //if (key == $.ui.keyCode.RIGHT || key == $.ui.keyCode.END) { //if press right arrow button and End button
                //    var startPos = input[0].selectionStart;
                //    var char = text.charAt(startPos);
                //    if (attr.postfix && attr.postfix.indexOf(char) > -1) return false;
                //}

                if (key == $.ui.keyCode.SPACE) return false; // space not allowed
                //if (key == $.ui.keyCode.DELETE) { // when Press delete button px should not be deleted
                //    if ((input[0].selectionStart == (text.length - 2)) && attr.postfix) {
                //        return false;
                //    }
                //}
                if (attr.postfix && text == attr.postfix) {
                    text = "";
                    input.val(text);
                }
                if (text) {
                    //if (attr.postfix && text.indexOf(attr.postfix) <= -1) {
                    //    input[0].value += attr.postfix;
                    //    text += attr.postfix;
                    //    var len = text.length;
                    //    input[0].setSelectionRange(input[0].selectionStart, len - 2);
                    //}

                    //input.val(text);
                    //if (attr.postfix) {
                    //    var len = text.length;
                    //    if (input[0].selectionStart > (len - 2)) {
                    //        input[0].setSelectionRange(input[0].selectionStart, len - 2);
                    //    }
                    //    input.focus();
                    //}
                }
            });
            element.on('blur', function (event) {
                var input = $(event.target);
                var text = input.val();
                if (attr.postfix) {
                    var inputText = scope.filterInput(input);
                    var result = units.some(function (ch) { if (inputText.unit && inputText.unit.toLowerCase() == ch.toLowerCase()) { return true; } });
                    if (!result && input[0].value && inputText.number && !isNaN(inputText.number)) {
                        input[0].value = inputText.number + "px";
                    } else if (!inputText.number) {
                        input[0].value = "";
                    }
                    element.trigger("change");
                }
            });
            element.on("paste", function (e) {
                e.preventDefault();
            });
            scope.filterInput = function ($input) {
                var newInput = "";
                var objInputText = { number: '', unit: '' };
                if ($input && $input[0].value) {
                    var value = $input[0].value;
                    value = value.split("");
                    var index = 0;
                    var valid = true;
                    if (angular.isArray(value)) {
                        while (index < value.length) {
                            if (valid && value[index] == ".") {
                                valid = false;
                                index++;
                                continue;
                            }
                            if (isNaN(value[index])) {
                                break;
                            }
                            index++;
                        }
                        objInputText.number = $input[0].value.substring(0, index);
                        objInputText.unit = $input[0].value.substring(index, $input[0].value.length);
                    }
                }
                return objInputText;
            }
        }
    };
});

app.directive("findReferencebox", [function () {
    return {
        restrict: 'E',
        templateUrl: 'Common/views/FindReferenceBox.html',
    };
}]);

// Use this directive where we used ng-repeat on li or tr and want to navigate list using up and down key
app.directive("keyUpDownWithElement", [function () {
    return {
        scope: {
            callbackfn: '=',
            objproperty: "=",
            property: "=",
            propvalue: "=",
            parentelem: "=",
            isscroll: "=",
            requireEvent: "="
        },
        link: function (scope, element, attr) {
            element.on("keydown", function (e) {
                scope.currentEvent = e;
                scope.nextElement = null;
                var skip = false;
                $(".ui-autocomplete").each(function () { // if intellisense list is open then skip navigation
                    var display = $(this).css("display");
                    if (display == "block") {
                        skip = true;
                    }
                });
                if (skip) {
                    return;
                }

                var elem = element;
                if (scope.parentelem) { // if used this directive inside li or tr item then pass the value of parentelem e.g 'li'
                    elem = $(elem).parents(scope.parentelem).first();
                }
                if (e.keyCode == $.ui.keyCode.UP) {
                    scope.findNextItem(elem, 'UP');
                    e.preventDefault();
                } else if (e.keyCode == $.ui.keyCode.DOWN) {
                    scope.findNextItem(elem, 'DOWN');
                    e.preventDefault();
                }
            });
            scope.findNextItem = function (elem, navigation) {
                var nextElement = null;
                var elemScope = null;
                var currentElement = elem;
                $(currentElement).find("input:focus").blur();

                if (navigation == "UP") {
                    nextElement = $(currentElement).prevAll(':visible:first');
                    if (nextElement.length <= 0) {
                        nextElement = $(currentElement).prev();
                    }
                } else if (navigation == "DOWN") {
                    nextElement = $(currentElement).nextAll(':visible:first');
                    if (nextElement.length <= 0) {
                        nextElement = $(currentElement).next();
                    }
                }
                if (nextElement.length <= 0) {
                    return;
                }

                if (scope.parentelem) { // if used this directive inside li or tr item then focus on it
                    var elemtemp = nextElement.find("[key-up-down-with-element]");
                    elemtemp.focus();
                } else {
                    nextElement.focus();
                }

                elemScope = angular.element(nextElement).scope();
                if (nextElement.is("[ng-if]") && elemScope) {
                    elemScope = elemScope.$parent;
                }

                if (elemScope && elemScope.hasOwnProperty(scope.objproperty)) {
                    var obj = elemScope[scope.objproperty];
                    //if li or tr items having hidden element and if you want to navigate then pass below two property with value
                    if (obj && obj.dictAttributes && obj.dictAttributes.hasOwnProperty(scope.property) && obj.dictAttributes[scope.property] == scope.propvalue) {
                        scope.findNextItem(nextElement, navigation);
                    } else {
                        scope.nextElement = obj != undefined ? obj : null;
                        if (scope.nextElement != null && angular.isFunction(scope.callbackfn)) {
                            scope.$evalAsync(function () {
                                if (scope.requireEvent) {
                                    scope.callbackfn(scope.nextElement, scope.currentEvent);
                                } else {
                                    scope.callbackfn(scope.nextElement);
                                }
                                var parents = nextElement.parents();
                                if (!scope.isscroll) {
                                    $.each(parents, function (key, value) {
                                        if ($(this).hasScrollBar()) {
                                            $(this).scrollTo(nextElement, { offsetTop: 170, offsetLeft: 0 }, null);
                                            return false;
                                        }
                                    });
                                }
                            });
                        }
                    }
                }
            };
        }
    }
}]);


app.directive("basemodelPropertiesrow", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: {
            prop: "=",
            clickCallback: "&",
            changeCallback: "&",
            category: "@",
            dbclickCallback: "&",
            showDeleteButton: "=",
            deletePropCallback: "&",
        },
        templateUrl: 'Form/views/ControlProperties/advancePropertiesCatergory.html',
        link: function (scope, element, attributes) {
            scope.disabledProplist = attributes.disabledProplist ? scope.$eval(attributes.disabledProplist) : [];
            if (scope.prop.PropertyName == "sfwIsCaption" && scope.prop.PropertyValue == "True") {
                scope.disabledProplist.push("sfwIsCaption");
            }
            $(element).find("td:first-child").resizable({
                handles: "e", minWidth: 15,
                resize: function (event, ui) {
                    var lstTD = $(ui.element).closest("tbody").find("td:not(.advanced-prop-head):first-child");
                    $(lstTD).css("width", ui.size.width);
                    $(lstTD).find("div[advance-prop-caption-wrapper]").css("width", ui.size.width);
                }
            });
        }
    };
}]);
app.directive("workflowDrop", ["$rootScope", function ($rootScope) {
    return {
        restrict: 'A',
        scope:
        {
            dropData: "=",
            allowdrop: "="

        },

        link: function (scope, element, attrs) {
            var el = element[0];
            el.addEventListener('drop', dropHander, false);
            el.addEventListener("dragover", function (event) {
                event.preventDefault();
            });
            function dropHander(e) {
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                if (scope.allowdrop) {
                    // Stops some browsers from redirecting.
                    $(e.currentTarget).trigger("click");
                    var dataobject = dragDropDataObject;
                    var data = dragDropData;
                    scope.$apply(function () {
                        if (dataobject && (dataobject.DataType == "ValueType" || dataobject.DataType == "BusObjectType")) {
                            if (scope.dropData) {

                                $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwObjectField, scope.dropData.dictAttributes, 'sfwObjectField', data);
                                if (dataobject.ItemPath) {
                                    var objectID = dataobject.ItemPath.substr(0, dataobject.ItemPath.indexOf('.'));
                                    $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwObjectID, scope.dropData.dictAttributes, 'sfwObjectID', objectID);

                                }


                                var dataType = getDataType(dataobject.DataType, dataobject.DataTypeName);
                                $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwDataType, scope.dropData.dictAttributes, 'sfwDataType', dataType);


                            }

                        }
                    });
                    dragDropData = null;
                    dragDropDataObject = null;

                }
                else {
                    return false;
                }
            }
        }

    }

}]);

app.directive('multiValueinput', ["$compile", "$rootScope", "CONSTANTS", function ($compile, $rootScope, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            propertyvalue: "=",
            propertyname: "=",
            separator: "@"
        },
        templateUrl: "Common/views/Directives/multivalueinput.html",
        link: function (scope, element, attrs) {
            scope.onAddMultipleValueClick = function () {
                var newScope = scope.$new(true);
                newScope.lstPropertyValues = [];
                var lstPropertyValues = scope.propertyvalue ? scope.propertyvalue.split(scope.separator) : [];
                if (lstPropertyValues.length > 0) {
                    newScope.lstPropertyValues = $.map(lstPropertyValues, function (obj) {
                        return {
                            value: obj
                        }
                    });
                }
                var objDialogMultiValueinput = $rootScope.showDialog(newScope, "Set Value(s)", "Common/views/AddMultipleInputValue.html", { width: 400, height: 300 });
                newScope.AddValue = function () {
                    newScope.lstPropertyValues.push({ value: "" });
                };
                newScope.deletevalue = function (index) {
                    if (index >= 0) {
                        newScope.lstPropertyValues.splice(index, 1);
                    }
                };
                newScope.setValueClick = function () {
                    if (newScope.lstPropertyValues.length > 0) {
                        var arrvalues = $.map(newScope.lstPropertyValues, function (obj) {
                            if (obj.value) {
                                return obj.value;
                            }
                        });
                        scope.propertyvalue = arrvalues.join(scope.separator);
                    }
                    objDialogMultiValueinput.close();
                };
                newScope.cancelClick = function () {
                    objDialogMultiValueinput.close();
                }
            };
            function init() {
                // initial config will come here
                scope.separator = scope.separator ? scope.separator : ",";

            }
            init();
        }
    };
}]);

app.directive("xmlTree", function () {
    return {
        scope: {
            data: '=',
            selectfunction: '='
        },
        template: '<span ng-repeat="node in data.Elements">' +
            '<span xml-node-tree-directive data="node" selectfunction="selectfunction"></span>' +
            '</span>',
        link: function (scope) {

        }
    }
});

app.directive("xmlNodeTreeDirective", ["$timeout", "$rootScope", function ($timeout, $rootScope) {
    return {
        scope: {
            data: '=',
            selectfunction: '='
        },
        template: '<ul class="formtree">' +
            '<li ng-repeat="item in getColumns(data)" style="cursor: pointer;" data-ng-click="selectElement(item,$event)">' +
            '<span mainspan><i ng-show="item.Elements.length>0 && item.Elements[0].Name != \'ListItem\' && item.Elements[0].Name != \'Columns\'" ng-class="item.isExpand?\'fa fa-caret-down\':\'fa fa-caret-right\'" ng-click="toggle(item,$event)" aria-hidden="true"></i>' +
            '<span ng-bind="item.Name"></span> <span> - </span> <span>{{ item.Name == "sfwPanel" && item.dictAttributes.sfwCaption?item.dictAttributes.sfwCaption: item.dictAttributes.ID }}</span></span>' +
            '<span ng-show="item.isExpand" ng-if="item.Elements.length > 0" xml-tree data="item" selectfunction="selectfunction"></span>' +
            '</li>' +
            '</ul>',
        link: function (scope, ele, attr) {
            scope.toggle = function (item, event) {
                item.isExpand = !item.isExpand;
                //  scope.toggleExpander(event.currentTarget);
            };
            scope.toggleExpander = function (element) {
                var elem = $(element).parents("li").first().find("ul").first();
                $timeout(function () {
                    //$(elem).find("first:li").children('ul.formtree').slideToggle(300);
                    if ($(element).hasClass("fa fa-caret-down")) {
                        $(elem).slideDown("normal");
                    }
                    else {
                        // $(elem).find(".selected-tree-item").not().parent('ul .selected-tree-item').removeClass("selected-tree-item");
                        $(elem).slideUp("normal");
                    }
                });
            }
            scope.selectElement = function (item, event, elem) {
                //$(".formtree [mainspan]").find(".selected-tree-element").removeClass("selected-tree-element");
                var domLi = null;
                if (event) {
                    domLi = $(event.currentTarget);
                    event.stopPropagation();
                } else if (elem) {
                    domLi = elem;
                }
                $(".selected-tree-item").removeClass("selected-tree-item");
                $(domLi).find("[mainspan]:first").addClass("selected-tree-item");
                if (item) {
                    scope.selectedItem = item;
                    $timeout(function () {
                        scope.selectfunction(scope.selectedItem);
                    });
                }

            };
            scope.getColumns = function (model) {
                var list = [];
                if (model.Elements.length > 0) {
                    for (var i = 0; i < model.Elements.length; i++) {
                        var obj = model.Elements[i];
                        //  console.log(obj.Name);
                        if (obj.Name == "sfwTabSheet") {
                            list = list.concat(obj);
                        } else if (obj.Name == "sfwColumn") {
                            list = list.concat(obj.Elements);
                            continue;
                        }
                        if (obj && obj.Name != "sfwColumn" && obj.Name != "sfwTabSheet" && obj.Name != "ListItem" && obj.Name != "Columns") {
                            list = list.concat(scope.getColumns(obj));
                        }
                    }
                }
                return list;
            };
            //scope.$watch("inputid", function () {
            //    scope.searchById(scope.parent, scope.inputid);
            //});
            if (!$rootScope.$$listenerCount['searchById']) {
                $rootScope.$on('searchById', function (event, data) {
                    console.log("called function");
                    scope.searchById(data.model, data.input);
                });
            }
            scope.searchById = function (items, id) {
                var idFound = false;
                var recursiveFilter = function (items, id) {
                    angular.forEach(items.Elements, function (item) {
                        if (item && item.dictAttributes.ID && id && item.dictAttributes.ID.toLowerCase() == id.toLowerCase()) {
                            idFound = true;
                            scope.$evalAsync(function () {
                                var elem = getScope(item);
                                scope.selectElement(item, null, elem);
                                $timeout(function () {
                                    expandParent(item);
                                });
                            });
                        }
                        if (item && angular.isArray(item.Elements) && item.Elements.length > 0 && !idFound) {
                            // item.isExpand = true;
                            recursiveFilter(item, id);
                        }
                    });
                };


                var expandParent = function (item) {
                    var parent = item;
                    while (parent) {
                        parent.isExpand = true;
                        if (parent.ParentVM) {
                            parent = parent.ParentVM;
                        } else {
                            parent = null;
                        }
                    }
                };

                var getScope = function (item) {
                    var elem;
                    $('.formtree li').each(function () {
                        var elemScope = angular.element(this).scope();
                        if (elemScope && elemScope.item == item) {
                            elem = $(this);
                            return false; // stop looking at the rest
                        }
                    });
                    return elem;
                }

                recursiveFilter(items, id);
            };

        }
    }
}]);

app.directive("keyUpDownForTree", ["$timeout", function ($timeout) {
    return {
        scope: {
            callbackfn: '=',
            objproperty: "=",
            property: "=",
            propvalue: "=",
            parentelem: "=",
            objparent: "=",
            isscroll: "="
        },
        link: function (scope, element, attr) {
            element.on("keydown", function (e) {

                scope.nextElement = null;
                var skip = false;
                $(".ui-autocomplete").each(function () { // if intellisense list is open then skip navigation
                    var display = $(this).css("display");
                    if (display == "block") {
                        skip = true;
                    }
                });
                if (skip) {
                    return;
                }

                var elem = element;
                if (scope.parentelem) { // if used this directive inside li or tr item then pass the value of parentelem e.g 'li'
                    elem = $(elem).parents(scope.parentelem).first();
                }
                if (e.keyCode == $.ui.keyCode.UP) {
                    scope.findNextItem(elem, 'UP');
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.DOWN) {
                    scope.findNextItem(elem, 'DOWN');
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.RIGHT) {
                    scope.findNextItem(elem, "RIGHT");
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.LEFT) {
                    scope.findNextItem(elem, "LEFT");
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
            scope.findNextItem = function (elem, navigation) {
                var nextElement = null;
                var elemScope = null;
                var currentElement = elem;
                $(currentElement).find("input:focus").blur();

                if (navigation == "UP") {
                    nextElement = $(currentElement).prevAll(':visible:first');  // finding in current li previous siblings 
                    if (nextElement && nextElement.length > 0) {
                        var lastElem = $(nextElement).find("li:visible:last"); // if li previous having another children list then move to childrens last li
                        if (lastElem && lastElem.length > 0) {
                            nextElement = lastElem;
                        }
                    }
                    if (nextElement && nextElement.length <= 0) {
                        nextElement = $(currentElement).parents("li:visible:first"); // move to li first parent level li
                    }
                    if (nextElement.length <= 0) {  // move to li next sibling li
                        nextElement = $(currentElement).prev();
                    }
                    scope.goToNextElement(nextElement, currentElement, navigation);
                } else if (navigation == "DOWN") {
                    nextElement = $(currentElement).find("li:visible:first");  // find first decendent of current li next element
                    if (nextElement && nextElement.length <= 0) {
                        nextElement = $(currentElement).nextAll(':visible:first');  // move next li siblings next element
                    }
                    if (nextElement.length <= 0) {  //move to next element
                        nextElement = $(currentElement).next();
                    }
                    if (nextElement && nextElement.length <= 0) {    //  find next li if having siblings then go to parents first li next siblings li
                        nextElement = $(currentElement).parents("li:visible:first").next();
                        var elementParents = $(currentElement).parents("li:visible");
                        var index = 0;
                        while (elementParents && elementParents.length > index) {
                            var parentElem = elementParents[index];
                            if (parentElem) {
                                var siblings = $(parentElem).next().siblings('li');
                                if (siblings.length) {
                                    nextElement = $(parentElem).next();
                                    break;
                                }
                            }
                            index++;
                        }
                        $(currentElement).parents("li:visible").next().siblings('li');
                    }
                    scope.goToNextElement(nextElement, currentElement, navigation);
                } else if (navigation == "RIGHT") {
                    var currentElemScope = angular.element(currentElement).scope();
                    if (currentElemScope && currentElemScope.hasOwnProperty(scope.objproperty)) {
                        var currentElemScope = currentElemScope[scope.objproperty];
                        if (currentElemScope && currentElemScope.Elements.length > 0) {
                            currentElemScope.IsExpanded = true;
                        }
                        $timeout(function () {
                            //nextElement = $(currentElement).find("ul li:first");
                            scope.goToNextElement(currentElement, currentElement, navigation);
                        });
                    }

                } else if (navigation == "LEFT") {
                    var currentElemScope = angular.element(currentElement).scope();
                    if (currentElemScope && currentElemScope.hasOwnProperty(scope.objproperty)) {
                        var currentElemScope = currentElemScope[scope.objproperty];
                        if (currentElemScope && currentElemScope.Elements.length > 0) {
                            currentElemScope.IsExpanded = false;
                        }
                        $timeout(function () {
                            //nextElement = $(currentElement).parents("li:visible:first").next();
                            scope.goToNextElement(currentElement, currentElement, navigation);
                        });
                    }
                }

            };
            scope.goToNextElement = function (nextElem, currentElement, navigation) {
                var nextElement = nextElem;
                if (nextElement && nextElement.length <= 0) {
                    if (navigation == "UP") {
                        nextElement = $(currentElement).parents("li:visible:first");
                    } else if (navigation == "DOWN") {
                        nextElement = $(currentElement).find("ul").find("li:first");
                    }
                    //  return;
                }

                if (scope.parentelem) { // if used this directive inside li or tr item then focus on it
                    var elemtemp = nextElement.find("[key-up-down-with-element]");
                    elemtemp.focus();
                } else {
                    nextElement.focus();
                }

                elemScope = angular.element(nextElement).scope();
                if (nextElement.is("[ng-if]") && elemScope) {
                    elemScope = elemScope.$parent;
                }

                if (elemScope && elemScope.hasOwnProperty(scope.objproperty)) {
                    var obj = elemScope[scope.objproperty];
                    scope.nextElement = obj != undefined ? obj : null;
                    if (scope.nextElement != null && angular.isFunction(scope.callbackfn)) {
                        scope.$evalAsync(function () {
                            var parentObj = null;
                            if (scope.objparent) {
                                parentObj = scope.objparent;
                            }
                            scope.callbackfn(scope.nextElement, parentObj);
                            var parents = nextElement.parents();
                            if (!scope.isscroll) {
                                $.each(parents, function (key, value) {
                                    if ($(this).hasScrollBar()) {
                                        $(this).scrollTo(nextElement, { offsetTop: 170, offsetLeft: 0 }, null);
                                        return false;
                                    }
                                });
                            }
                        });
                    }
                }
            }
        }
    }
}]);

//app.directive("knowtionId", ["hubcontext", "$timeout", "$searchQuerybuilder", "$rootScope", "$searchFindReferences", "$interval", function (hubcontext, $timeout, $searchQuerybuilder, $rootScope, $searchFindReferences, $interval) {
//    return {
//        restrict: "A",
//        link: function (scope, element, attribute) {
//            $(element).attr("tabindex", "0");
//            $(element).focus();
//            $(element).on("keydown", function () {
//                if (event.keyCode == 112 && event.key == "F1") // while pressing F1 key
//                {
//                    scope.showKnowtionHelp();
//                    event.stopPropagation();
//                }
//            });

//            scope.showKnowtionHelp = function () {
//                $rootScope.showHelp(attribute.knowtionId)
//            }
//        }
//    };
//}]);


app.directive('activeformintellisensetemplate', ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            activeformname: "=",
            propertyname: '=',
            formtype: '=',
            multiactiveformflag: '=',
            formmodel: '=',
            islookup: '=',
            onactiveformchange: '&',
            canhidesearchactiveformwindow: "=",
            isupdatefromexternalprop: "=",
            isdisabledinput: '='
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_active_form' ng-attr-title='{{model.errors.invalid_active_form}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span><div class='input-group'><input type='text' class='form-control input-sm' ng-disabled='isdisabledinput' id='ScopeId_{{$id}}' title='{{activeformname?activeformname:model.placeHolder}}' ng-model='activeformname' ng-keydown='onkeydown($event)' ng-change='onactiveformchange();validateActiveForm()' ng-blur='onactiveformchange();validateActiveForm()' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateActiveForm' placeholder='{{model.placeHolder}}'/><span class='input-group-addon button-intellisense' ng-disabled='isdisabledinput' ng-click='showActivefromIntellisenseList($event)' ng-hide='multiactiveformflag'></span><span class='input-group-addon btn-search-popup'  ng-click='onSearchActiveFormClick($event)' ng-hide='canhidesearchactiveformwindow||multiactiveformflag'></span><span class='input-group-addon btn-search-multiple-popup' ng-click='onAddMultipleActiveFormClick($event)' ng-show='multiactiveformflag'></span></div></div>",
        link: function (scope, element, attrs) {

            scope.$watch("formtype", function (newVal, oldVal) {
                if (newVal) {
                    if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwMethodName && scope.model.dictAttributes.sfwMethodName == 'btnOpenReport_Click') {
                        scope.filelist = [];
                        scope.filelist.push("wfmReportClientMVVM");
                        scope.filelist.push("wfmReportClient");
                    }
                    else {
                        $.connection.hubMain.server.getFilesByType(scope.formtype, "ScopeId_" + scope.$id, null);
                    }
                }
            });
            scope.receiveList = function (data) {
                scope.filelist = data;
                scope.showplaceHolder();
            };
            scope.showplaceHolder = function () {
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwMethodName && (scope.model.dictAttributes.sfwMethodName == 'btnOpen_Click' || scope.model.dictAttributes.sfwMethodName == 'btnNew_Click') && scope.formmodel && (scope.formmodel.dictAttributes.sfwType == "Lookup" || scope.formmodel.dictAttributes.sfwType == "FormLinkLookup") && scope.filelist) {
                    var newFormID = scope.formmodel.dictAttributes.ID.replace("Lookup", "Maintenance");
                    for (var i = 0; i < scope.filelist.length; i++) {
                        if (scope.filelist[i] == newFormID) {
                            scope.$evalAsync(function () {
                                scope.model.placeHolder = newFormID;
                                scope.model.isPlaceHolder = true;
                            });
                            break;
                        }
                    }
                }
            };

            scope.onkeydown = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                if (!scope.multiactiveformflag) {

                    //if (input.val() == undefined || input.val().trim() == "") {
                    setSingleLevelAutoComplete(input, scope.filelist);
                    if ($(input).data('ui-autocomplete') != undefined) {
                        $(input).autocomplete("enable");
                    }
                    //}
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.filelist) {
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                }
                else {
                    if ($(input).data('ui-autocomplete') != undefined) {
                        $(input).autocomplete("disable");
                    }
                }
            };
            scope.onSearchActiveFormClick = function () {
                //scope.SearchActiveForm = ngDialog.open({
                //    template: "/Form/views/SearchActiveForms.html",
                //    scope: scope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.filelist = scope.filelist;
                var tempFormType = scope.formtype;
                newScope.validateActiveForm = scope.validateActiveForm;
                newScope.isupdatefromexternalprop = scope.isupdatefromexternalprop;
                newScope.selectFormType = tempFormType.split(',');
                newScope.SearchActiveFormDialog = $rootScope.showDialog(newScope, "Search Active Form", "Form/views/SearchActiveForms.html", { width: 800, height: 530 });


                function onActiveFormClick(event, data) {
                    if (scope.propertyname) {
                        $rootScope.EditPropertyValue(scope.activeformname, scope, "activeformname", data);
                    }
                    else {
                        scope.activeformname = data;
                    }
                }

                scope.$on("onSearchActiveFormOkClick", onActiveFormClick);

            };

            scope.onAddMultipleActiveFormClick = function () {
                //scope.ActiveFormType = scope.formtype;
                //scope.IsMultiActiveForm = false;

                //scope.ActiveForm = ngDialog.open({
                //    template: "Views/Form/ActiveForms.html",
                //    scope: scope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.formmodel = scope.formmodel;
                newScope.lstEntityField = scope.lstEntityField;
                newScope.islookup = scope.islookup;
                newScope.ActiveFormType = scope.formtype;
                newScope.IsMultiActiveForm = false;
                newScope.validateActiveForm = scope.validateActiveForm;

                newScope.ActiveFormDialog = $rootScope.showDialog(newScope, "Active Forms", "Form/views/ActiveForms.html", { width: 800, height: 500 });

            };
            scope.showActivefromIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.filelist && scope.filelist.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.filelist, scope);
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.validateActiveForm = function () {
                //if (scope.activeformname && scope.model) {
                //    scope.model.isPlaceHolder = false;
                //}
                //else 
                //if (scope.model) {
                //    scope.showplaceHolder();
                //}
                if (scope.propertyname) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else if (scope.model && scope.model.dictAttributes) {
                        input = scope.model.dictAttributes[property];
                    }
                    $ValidationService.checkActiveForm(scope.filelist ? scope.filelist : [], scope.model, input, property, 'invalid_active_form', CONST.VALIDATION.INVALID_ACTIVE_FORM, undefined);
                }
            };
        },
    };
}]);

app.directive("fileintellisensetemplate", ['$rootScope', '$ValidationService', 'CONSTANTS', function ($rootScope, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            filename: "=",
            propertyname: '=',
            filetype: '=',
            onfilechange: '&',
            isdisabled: '=',
            onfileblur: '&',
            insidegrid: "=",
            errorprop: "=",
            isvalidate: "=",
            showentbase: "=",
            isinsidedialog: "=",
            tablename: "=",
            isreset: "=",
            projectbasename: "="
        },
        template: "<div><div class='input-group' ng-show='!isinsidedialog'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input  ng-class=\"insidegrid?'form-control full-width form-filter input-sm':'form-control input-sm full-width'\" type='text' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-keyup='validateFileTemplate();validateEntity();' ng-change='onchangecallback()' ng-blur='onblur();' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateFileTemplate'/><span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-class=\"isdisabled?'disabledlink':''\"></span></div><div class='input-group' ng-show='isinsidedialog'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input  ng-class=\"insidegrid?'form-control full-width form-filter input-sm':'form-control input-sm full-width'\" type='text' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-keyup='validateFileTemplate();validateEntity();' ng-change='onchangecallback()' ng-blur='onblur();'/><span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-class=\"isdisabled?'disabledlink':''\"></span></div></div>",
        link: function (scope, element, attrs) {
            scope.prevvalue = scope.filename;
            scope.onchangecallback = function () {
                // this condition is only for first time - when change is called before we receive the list from the hub
                if (scope.propertyname && !scope.tempModel) {
                    scope.tempModel = "model";
                    var properties = scope.propertyname.split(".");
                    for (var i = 0, l = properties.length; i < l - 1; i++) {
                        scope.tempModel = scope.tempModel.concat("." + properties[i]);
                    }
                }
                if (!scope.model) {
                    scope.model = { dictAttributes: {} };
                }
                if (scope.tempModel) {
                    var properties = scope.propertyname.split(".");
                    //var t = eval(scope.tempModel);
                    var t;
                    var larrProps = scope.tempModel.split(".");
                    var ltmpObject = scope;
                    for (var i = 0, len = larrProps.length; i < len; i++) {
                        ltmpObject = ltmpObject[larrProps[i]];
                    }
                    if (ltmpObject !== null) {
                        t = ltmpObject;
                    }
                    t[properties[properties.length - 1]] = scope.filename;
                }
                scope.onfilechange();
                scope.validateFileTemplate();
                scope.validateEntity();
                //if (scope.model.dictAttributes && scope.model.dictAttributes.hasOwnProperty("sfwEntity")) {
                //    $ValidationService.validateEntity(scope.model, undefined);
                //}
            };
            scope.receiveList = function (data) {
                if (scope.propertyname) {
                    scope.tempModel = "model";
                    var properties = scope.propertyname.split(".");
                    for (var i = 0, l = properties.length; i < l - 1; i++) {
                        scope.tempModel = scope.tempModel.concat("." + properties[i]);
                    }
                }
                scope.filelist = data;
                if (scope.filetype && scope.filetype.toLowerCase() == "entity" && scope.showentbase && scope.filelist.indexOf("entBase") == -1) {
                    scope.filelist.splice(0, 0, "entBase");
                }
                scope.inputElement.focus();
                setSingleLevelAutoComplete(scope.inputElement, scope.filelist);
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
                scope.$evalAsync(function () {
                    scope.validateFileTemplate();
                });
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.filelist && !scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                }
                else if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    scope.inputElement.focus();
                    setSingleLevelAutoComplete(scope.inputElement, scope.filelist, scope);
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onkeydown = function (event) {
                if (!scope.filelist) {
                    scope.inputElement = $(event.target);
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.isreset && scope.filelist) {
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                    event.preventDefault();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.filelist) {
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    event.preventDefault();
                } else if (!scope.filelist && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    event.preventDefault();
                }
            };
            scope.validateFileTemplate = function () {
                if (scope.propertyname && scope.isvalidate) {
                    var errMessage, errProp;
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else input = scope.model.dictAttributes[property];
                    if (scope.filetype == "UserControl" || scope.filetype == "Tooltip") {
                        errProp = "invalid_active_form";
                        errMessage = CONST.VALIDATION.INVALID_ACTIVE_FORM;
                        scope.onfilechange();
                    }
                    if (scope.filetype == "Entity") errMessage = CONST.VALIDATION.INVALID_ENTITY_NAME;
                    $ValidationService.checkValidListValue(scope.filelist, scope.model, $(scope.inputElement).val(), property, errProp ? errProp : property, errMessage, undefined);
                }
            };
            scope.validateEntity = function () {
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.hasOwnProperty("sfwEntity")) {
                    $ValidationService.validateEntity(scope.model, undefined);
                }
            };
            scope.onblur = function () {
                if (scope.prevvalue != scope.filename) {
                    scope.onfileblur();
                    scope.prevvalue = scope.filename;
                }
            };
        }
    };
}]);

app.directive("entityfieldelementintellisense", ['$rootScope', '$filter', '$ValidationService', 'CONSTANTS', '$EntityIntellisenseFactory', function ($rootScope, $filter, $ValidationService, CONST, $EntityIntellisenseFactory) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modebinding: "=",
            entityid: "=",
            propertyname: '=',
            onchangecallback: '&',
            ontextchangecallback: '&',
            isshowonetoone: '=',
            isshowonetomany: '=',
            isshowcolumns: '=',
            isshowcdocollection: '=',
            isshowproperty: '=',
            isshowglobalparams: '=',
            disabled: "=isDisabled",
            isObject: "=",
            onblurcallback: '&',
            candrop: '=',
            validate: '=',
            isshowexpression: '=',
            errorprop: '=',
            isempty: '=',
            setcolumndatatype: '=',
            isaddthis: '=',
            exclude: '='
        },
        template: "<div><div class='input-group full-width'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group full-width'><span class='info-tooltip dtc-span' ng-if='model.errors.duplicate_id' ng-attr-title='model.errors.duplicate_id' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input ng-disabled='disabled' type='text' ng-change='onchange()' ondrop='onEntityFieldDropInDirective(event)' ng-blur='onBlur()' ondragover='onEntityFieldDragOver(event)' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateEntityField' ng-keyup='onkeyup($event);validateEntityField()' ng-keydown='onkeydown($event)' ng-change='validateEntityField()'/><span ng-show='!disabled' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.setAutocomplete = function (input, event) {
                // this function will only be called when you want to change the data for the intellisense i.e switch between single level to multilevel and vice-versa
                scope.isMultilevelActive = false;
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    var arrText = getSplitArray(input.val(), input[0].selectionStart);
                    var data = [];
                    if (scope.isaddthis) {
                        data.push({ ID: "this", DisplayName: "this" });
                    }
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = scope.entityid;
                    if (scope.model && scope.model.Name == "sfwGridView") {
                        var entity = entities.filter(function (x) {
                            return x.ID == scope.entityid;
                        });
                        if (entity.length > 0 && entity[0].ErrorTableName) {
                            var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                            var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                            data.push(objInternalError);
                            data.push(objExternalError);
                        }
                    }
                    while (parententityName) {
                        data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, true, scope.isshowonetoone, scope.isshowonetomany, scope.isshowcolumns, scope.isshowcdocollection, scope.isshowexpression, scope.isshowproperty));
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            parententityName = entity[0].ParentId;
                        } else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    var parententityName = item[0].Entity;
                                    data = [];
                                    while (parententityName) {
                                        //expression should not come for second level
                                        data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, true, scope.isshowonetoone, scope.isshowonetomany, scope.isshowcolumns, scope.isshowcdocollection, scope.isshowexpression, scope.isshowproperty));
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
                                else data = [];
                            }
                        }
                    }
                    if (scope.isObject && data && data.length > 0) {
                        data = $filter('filter')(data, { DataType: "Object" });
                    }
                    if (scope.exclude && scope.exclude.length) {
                        data = data.filter(function (x) { return !scope.exclude.some(function (y) { return y.dictAttributes.ID === x.ID; }); });
                    }
                    if (scope.isshowglobalparams) {
                        $.connection.hubForm.server.getGlobleParameters().done(function (globalParams) {
                            scope.$apply(function () {
                                var filterAttributes = [];
                                scope.objGlobleParameters = globalParams;
                                function iterator(itm) {
                                    if (itm.dictAttributes && itm.dictAttributes.ID) {
                                        var mainItem = { ID: "~" + itm.dictAttributes.ID };
                                        filterAttributes.push(mainItem);
                                    }
                                }
                                if (scope.objGlobleParameters) {
                                    if (scope.objGlobleParameters.Elements.length > 0) {
                                        angular.forEach(scope.objGlobleParameters.Elements, iterator);

                                    }
                                }
                                data = data.concat(filterAttributes);

                                setMultilevelAutoCompleteForObjectTreeIntellisense(input, data, "", scope.onchangecallback);
                            });

                        });
                    }
                    else {
                        setMultilevelAutoCompleteForObjectTreeIntellisense(input, data, "", scope.onchangecallback);
                    }
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, [], "", scope.onchangecallback);
                }
            };
            scope.onkeyup = function (event) {
                // this enables the second level autocomplete
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (scope.inputElement) {
                        var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                        if (arrText.length > 1 || arrText.length == 0) scope.setAutocomplete(scope.inputElement, event);
                    }
                }
                scope.ontextchangecallback();
            };
            scope.prevEntityID = undefined;
            scope.onkeydown = function (event) {
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (!scope.inputElement || scope.prevEntityID != scope.entityid) {
                        scope.inputElement = $(event.target);
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.prevEntityID = scope.entityid;
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        }
                        event.preventDefault();
                    }
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    // if user goes back from second level to first level intellisense
                    if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense($(event.target), [], "", scope.onchangecallback);
                }
                //else if (scope.inputElement) {
                //    scope.setAutocomplete(scope.inputElement, event);
                //}

            };
            scope.showIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }

                if (scope.entityid && scope.entityid.trim().length > 0) {

                    if (scope.prevEntityID != scope.entityid) {
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.inputElement.focus();
                    scope.prevEntityID = scope.entityid;
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    if (arrText.length > 1) scope.setAutocomplete(scope.inputElement, event);
                    // switch back from multilevel to single level  
                    else if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    }
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, [], "", scope.onchangecallback);
                }
            };
            scope.validateEntityField = function () {
                if (scope.propertyname && scope.validate) {
                    var attrType;
                    var one2one = scope.isshowonetoone ? 'Object,' : '';
                    var one2manay = scope.isshowonetomany ? 'Collection,List,' : '';
                    var column = scope.isshowcolumns ? 'Column,' : '';
                    var cdo = scope.isshowcdocollection ? 'CDOCollection,' : '';
                    var exp = scope.isshowexpression ? 'Expression,' : '';

                    attrType = one2one + one2manay + column + cdo + exp;

                    var propertyTemp = scope.propertyname.split('.');
                    property = propertyTemp[propertyTemp.length - 1];
                    var errorMsg = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    if (property != "sfwEntityField") {
                        errorMsg = CONST.VALIDATION.INVALID_FIELD;
                    }
                    var list = [];
                    $ValidationService.checkValidListValueForMultilevel(list, scope.model, $(scope.inputElement).val(), scope.entityid, property, property, errorMsg, undefined, scope.isempty, attrType);
                    var sfwRelFieldVal = $(scope.inputElement).val();
                    if (property == "sfwRelatedField" && (sfwRelFieldVal != "") && !scope.model.dictAttributes.sfwOperator) {
                        if (!scope.model.errors && !angular.isObject(scope.model.errors)) {
                            scope.model.errors = {};
                        }
                        if (!scope.model.errors.sfwOperator) {
                            scope.model.errors.sfwOperator = CONST.VALIDATION.EMPTY_FIELD;
                        }
                    }
                    if (property == "sfwRelatedField" && (sfwRelFieldVal == "") && !scope.model.dictAttributes.sfwOperator) {
                        if (!scope.model.errors && !angular.isObject(scope.model.errors)) {
                            scope.model.errors = {};
                        }
                        if (scope.model.errors.sfwOperator) {
                            delete scope.model.errors.sfwOperator;
                        }
                    }
                    else if (property == "sfwRelatedField" && (sfwRelFieldVal == "") && scope.model.dictAttributes.sfwOperator) {
                        scope.model.dictAttributes.sfwOperator = "";
                    }
                }
            };
            scope.onBlur = function () {
                if (scope.onblurcallback) scope.onblurcallback({ model: scope.model });
                if (scope.ontextchangecallback) scope.ontextchangecallback();
                //  scope.validateEntityField();
            };
            scope.onchange = function () {
                if (scope.onchangecallback) scope.onchangecallback();
                scope.$evalAsync(function () {
                    scope.validateEntityField();
                });
            };
        }
    };
}]);

app.directive("resourceintellisensetemplate", ["$compile", "$rootScope", "$timeout", "$ValidationService", "CONSTANTS", "$filter", function ($compile, $rootScope, $timeout, $ValidationService, CONST, $filter) {

    var getTemplate = function (ablnshowvertical) {
        var template = ""; //<div class='col-xs-6'>
        if (ablnshowvertical) {
            // template += "<div class='form-group'>",
            template += "<div> <label class='control-label' id='ScopeId_{{$id}}' ng-show='showresourcetext'>{{resourcetext}}</label>";
            template += "<div class='input-group' ng-show='!isinsidedialog'>";
            template += "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>";
            template += "<input  type='text' title='{{resourceid}}' ng-model='resourceid' class='form-control input-sm'  ng-change='onchangeresourcecallback();validateResource()' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateResource'/>";
            template += "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span></div>";
            template += "<div class='input-group' ng-show='isinsidedialog'>";
            template += "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>";
            template += "<input type='text' title='{{resourceid}}' ng-model='resourceid' class='form-control input-sm'  ng-change='onchangeresourcecallback();validateResource()' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)'/>";
            template += "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span></div>";
            //template += "</div>",
            template += "</div>";
        }
        else {
            template = [" <label class='{{resourcelabelclass}}' id='ScopeId_{{$id}}' ng-show='showresourcetext'>{{resourcetext}}</label>",
                "<div class='{{resourcetextboxclass}}'>",
                "<div class='input-group'>",
                "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                "<input type='text' class='form-control input-sm' title='{{resourceid}}' ng-model='resourceid' ng-change='onchangeresourcecallback();validateResource();' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)' ng-focus='focusIn()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateResource' class='form-control input-sm'/> ",
                "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span>",
                "<span class='input-group-addon btn-search-popup' ng-click='onResouceSearchClick($event)'></span>",
                "</div>",
                "</div>"];
            template = template.join(' ');
        }



        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            resourceid: "=",
            propertyname: "=",
            showresourcetext: '=',
            resourcetext: '=',
            resourcelabelclass: "@",
            resourcetextboxclass: "@",
            onchangeresourcecallback: '&',
            showvertical: "=",
            isinsidedialog: "="
        },
        link: function (scope, element, attrs) {

            if (!scope.showvertical) {
                if (scope.resourcelabelclass == undefined || scope.resourcelabelclass == "") {
                    scope.resourcelabelclass = "col-xs-4 control-label";
                }
                if (scope.resourcetextboxclass == undefined || scope.resourcetextboxclass == "") {
                    scope.resourcetextboxclass = "col-xs-8";
                }
            }
            else {
                scope.resourcelabelclass = "control-label";
            }

            var newScope;
            var parent = $(element).parent();
            parent.html(getTemplate());

            if (scope.showvertical) {
                parent.html(getTemplate(scope.showvertical));
            }
            else {
                parent.html(getTemplate(false));
            }
            $compile(parent.contents())(scope);
            // var newScope = undefined;
            //  $.connection.hubMain.server.getResources("ScopeId_" + scope.$id);
            scope.receiveList = function (data) {
                scope.resourcelist = data;
                if (newScope) {
                    newScope.resourcelist = scope.resourcelist;
                }
                if (scope.resourcelist && scope.inputElement) {
                    var reourceList = [];
                    if (scope.inputElement) {
                        reourceList = scope.resourcelist.sort();
                        setSingleLevelAutoComplete(scope.inputElement, reourceList, scope, "ResourceID", "ResourceIDDescription");
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        }
                    }
                }
            };

            scope.resourceIDTextChanged = function (event) {
                if (event && event.keyCode == $.ui.keyCode.ESCAPE) {
                    return;
                }
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                var value = scope.inputElement.val();

                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }
                if (scope.resourcelist && scope.resourcelist.length > 0) {
                    var data = scope.resourcelist.sort();
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        event.preventDefault();
                    } else {
                        setSingleLevelAutoComplete(scope.inputElement, data, scope, "ResourceID", "ResourceIDDescription");
                    }
                }
            };

            scope.showResourceIDIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }

                if (scope.inputElement && scope.resourcelist) {
                    var resourceList = scope.resourcelist.sort();
                    setSingleLevelAutoComplete(scope.inputElement, resourceList, scope, "ResourceID", "ResourceIDDescription");
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onResouceSearchClick = function (event) {
                scope.strCode = "Resources";

                newScope = scope.$new();
                newScope.strCode = "Resources";
                newScope.validateResource = scope.validateResource;
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                    $timeout(function () {
                        newScope.resourcelist = scope.resourcelist;
                    });
                } else {
                    newScope.resourcelist = scope.resourcelist;
                }

                newScope.SearchIDDescrDialog = $rootScope.showDialog(newScope, "Search ID Description", "Common/views/SearchIDDescription.html", { width: 530, height: 540 });

                newScope.$on('onOKClick', function (event, data) {

                    if (newScope.propertyname) {
                        if (newScope.propertyname.indexOf('.') > -1) {
                            var property = newScope.propertyname.split('.')[1];
                            $rootScope.EditPropertyValue(scope.model.dictAttributes[property], scope.model.dictAttributes, property, data ? data.ID : "");
                        }
                        else {
                            scope.model[newScope.propertyname] = data ? data.ID : "";
                        }
                    }

                    //scope.resourceid = data ? data.ID : "";
                    if (event) {
                        event.stopPropagation();
                    }
                });
                if (event) {
                    event.stopImmediatePropagation();
                }
            };
            scope.focusIn = function () {
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }
            };
            scope.validateResource = function () {
                if (scope.propertyname && scope.model) {
                    var property = scope.propertyname.split('.')[1];
                    var list = ["0"]; //default value
                    if (scope.resourcelist && scope.resourcelist.length > 0) {
                        list = $ValidationService.getListByPropertyName(scope.resourcelist, "ResourceID", true);
                        list.push("0");// default value
                    }
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, undefined);
                }
            };
        }
    };
}]);

app.directive("visibleruleintellisensetemplate", ["$compile", "$rootScope", "$NavigateToFileService", "$ValidationService", "CONSTANTS", "$GetEntityFieldObjectService", function ($compile, $rootScope, $NavigateToFileService, $ValidationService, CONST, $GetEntityFieldObjectService) {

    var getTemplate = function () {
        var template = "";
        template += " <label class='{{resourcelabelclass}}'><u ng-if='!sfwvisiblerule.trim() || !isnavigationlink'>{{visibleruletext}}</u><a ng-if='sfwvisiblerule.trim() && isnavigationlink' ng-click='selectVisibleRuleClick()'>{{visibleruletext}}</a></label>";
        template += "<div class='{{resourcetextboxclass}}'>";
        template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwVisibleRule\' && model.errors.invalid_visible_rule" ng-attr-title="{{model.errors.invalid_visible_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwEnableRule\' && model.errors.invalid_enable_rule" ng-attr-title="{{model.errors.invalid_enable_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwReadOnlyRule\' && model.errors.invalid_readonly_rule" ng-attr-title="{{model.errors.invalid_readonly_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwSelectColVisibleRule\' && model.errors.sfwSelectColVisibleRule" ng-attr-title="{{model.errors.sfwSelectColVisibleRule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += "<div class='input-group'>",
            template += "<input type='text' title='{{sfwvisiblerule}}' ng-model='sfwvisiblerule' ng-keydown='visibleRuleTextChanged($event)' ng-keyup='validateRule()' ng-change='validateRule()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateRule' class='form-control input-sm'/>",
            template += "<span class='input-group-addon button-intellisense' ng-click='showVisibleRuleIntellisense($event)'></span>",
            template += "</div></div>";
        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            sfwvisiblerule: '=',
            propertyname: '=',
            formmodel: '=',
            visibleruletext: '=',
            resourcelabelclass: "@",
            resourcetextboxclass: "@",
            isnavigationlink: '=',
            entityid: '='
        },
        link: function (scope, element, attributes) {

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.PrevEntity = scope.entityid;
            scope.PrevInitGroup = scope.formmodel.dictAttributes.sfwInitialLoadGroup;
            scope.visibleRuleTextChanged = function (event) {

                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.ctrlKey && event.keyCode == 83) { // if user press (ctrl + s) then need to close opened intellisense list
                    event.preventDefault();
                    return;
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    event.preventDefault();
                    //if (scope.formmodel && scope.formmodel.objExtraData) {
                    //    getVisibleRuleData(scope.inputElement);
                    //    event.preventDefault();
                    //}
                } else if (!$(scope.inputElement).val() && event.keyCode != 38 && event.keyCode != 40) {
                    getVisibleRuleData(scope.inputElement, "change");
                }
            };
            scope.validateRule = function () {
                if (scope.propertyname) {
                    var property = scope.propertyname.split('.')[1];
                    var list = scope.data ? scope.data : [];
                    if (property == "sfwVisibleRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_visible_rule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, undefined);
                    } else if (property == "sfwEnableRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_enable_rule", CONST.VALIDATION.ENABLE_RULE_NOT_EXISTS, undefined);
                    } else if (property == "sfwReadOnlyRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_readonly_rule", CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, undefined);
                    } else {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, undefined);
                    }
                }
            };
            scope.showVisibleRuleIntellisense = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();

                if (scope.inputElement) {
                    getVisibleRuleData(scope.inputElement, "change");
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.selectVisibleRuleClick = function () {
                getVisibleRuleData(undefined, "navigate");
            };

            var navigateToRule = function () {
                scope.validateRule();
                if (scope.propertyname) var property = scope.propertyname.split('.')[1];
                var errorProp = "";
                if (property == "sfwVisibleRule") {
                    errorProp = "invalid_visible_rule";
                }
                else if (property == "sfwEnableRule") {
                    errorProp = "invalid_enable_rule";
                }
                else {
                    errorProp = "invalid_readonly_rule";
                }
                var IsNavigate = false;
                if (!scope.model.errors) {
                    IsNavigate = true;
                }
                else if (scope.model.errors && !scope.model.errors[errorProp]) {
                    IsNavigate = true;
                }
                if (scope.sfwvisiblerule && scope.sfwvisiblerule.trim() != "" && IsNavigate) {
                    var nodeName = "initialload";
                    //if (scope.formmodel.dictAttributes.sfwType == "Wizard" || scope.formmodel.dictAttributes.sfwType == "FormLinkWizard") nodeName = "groupslist";
                    var entityID = "";
                    if (scope.formmodel && scope.formmodel.SelectedControl && scope.formmodel.SelectedControl.IsGridChildOfListView) {
                        entityID = scope.entityid;
                    }
                    else if (scope.model && (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwChart" || scope.model.Name == "sfwListView")) {
                        if (scope.model.dictAttributes.sfwParentGrid && scope.model.Name != "TemplateField") {
                            var objParentGrid = FindControlByID(scope.formmodel, scope.model.dictAttributes.sfwParentGrid);
                            if (objParentGrid) {
                                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                                if (entObject) {
                                    entityID = entObject.Entity;
                                }
                            }

                            else {
                                entityID = scope.formmodel.dictAttributes.sfwEntity;
                            }
                        }
                        else if (scope.model.dictAttributes.sfwParentGrid && scope.model.Name == "TemplateField") {
                            entityID = scope.entityid;
                        }
                        else {
                            entityID = scope.formmodel.dictAttributes.sfwEntity;
                        }
                    }
                    else if (scope.entityid) {
                        entityID = scope.entityid;
                    }
                    else if (scope.formmodel && scope.formmodel.dictAttributes) {
                        entityID = scope.formmodel.dictAttributes.sfwEntity;
                    }
                    $NavigateToFileService.NavigateToFile(entityID, nodeName, scope.sfwvisiblerule);
                }
            };

            var getVisibleRuleData = function (input, action) {
                var entityID = "";
                var iswizard = scope.formmodel.dictAttributes.sfwType == "Wizard" || scope.formmodel.dictAttributes.sfwType == "FormLinkWizard" ? true : false;
                if (scope.model && (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwChart" || scope.model.Name == "sfwListView")) {
                    if (scope.formmodel && scope.formmodel.SelectedControl && scope.formmodel.SelectedControl.IsGridChildOfListView) {
                        entityID = scope.entityid;
                    }
                    else if (scope.model.dictAttributes.sfwParentGrid) {
                        var objGrid = FindControlByID(scope.formmodel, scope.model.dictAttributes.sfwParentGrid);
                        if (objGrid) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                            if (object) {
                                entityID = object.Entity;
                            }
                        }
                    }
                    else {
                        entityID = scope.formmodel.dictAttributes.sfwEntity;
                    }
                }
                else if (scope.entityid) {
                    entityID = scope.entityid;
                }
                else {
                    var ObjGrid = FindParent(scope.model, "sfwGridView");
                    if (ObjGrid) {
                        if (ObjGrid.dictAttributes.sfwBoundToQuery && ObjGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                            if (scope.formmodel && scope.formmodel.dictAttributes) {
                                entityID = scope.formmodel.dictAttributes.sfwEntity;
                            }
                        }
                        else if (ObjGrid && ObjGrid.dictAttributes.sfwParentGrid && ObjGrid.dictAttributes.sfwEntityField) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, ObjGrid.dictAttributes.sfwEntityField);
                            if (object) {
                                entityID = object.Entity;
                            }
                        }
                    }
                }
                if (entityID) {
                    hubMain.server.getEntityExtraData(entityID).done(function (data) {
                        scope.$evalAsync(function () {
                            var groupName;
                            if (scope.model && scope.model.ParentVM && scope.model.ParentVM.Name != "ItemTemplate" && entityID == scope.formmodel.dictAttributes.sfwEntity && scope.formmodel.dictAttributes.sfwInitialLoadGroup) {
                                groupName = scope.formmodel.dictAttributes.sfwInitialLoadGroup;
                            }
                            scope.data = PopulateEntityRules(data, iswizard, groupName, scope.model.dictAttributes.sfwRulesGroup /* scope.formmodel.dictAttributes.sfwInitialLoadGroup*/);
                            if (action == "change") {
                                if (input) setSingleLevelAutoComplete(input, scope.data);
                                if ($(scope.inputElement).data('ui-autocomplete')) {
                                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                                }
                            } else if (action == "navigate") {
                                navigateToRule();
                            }
                        });
                    });
                } else if (scope.inputElement) { // if entity is empty then clear intellisense
                    setSingleLevelAutoComplete(scope.inputElement, []);
                }

            };
        }
    };
}]);

app.directive('autoCompleteConfigurationSetting', ["$timeout", "hubcontext", function ($timeout, hubcontext) {
    return {
        restrict: "A",
        scope: {
            basepath: '=',
            searchtext: '=',
        },

        link: function (scope, element, attrs) {
            element.keyup(function () {
                if (scope.basepath && scope.basepath != "") {
                    if (scope.searchtext.length > 2) {
                        hubcontext.hubMain.server.getFolderPathIntellisense(attrs.id, scope.searchtext, scope.basepath);
                    }
                }
            });
        },
    };
}]);

app.directive("messageintellisensetemplate", ["$compile", "$rootScope", "ngDialog", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, ngDialog, $ValidationService, CONST) {

    var getTemplate = function (ablnshowvertical) {
        var template = "";

        if (ablnshowvertical) {
            template += "<div>",
                template += "<div class='col-xs-12' ng-show='!isinsidedialog'>",
                template += " <label class='control-label col-xs-12 no-left-padding' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='input-group col-xs-3 fleft'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMessageID'/>",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "<div class='col-xs-9'><span ng-if='messageid' class=\"file-detail-msg msgfont-11\" ng-bind='newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ></span>";
            template += "<span ng-if='!messageid' class=\"file-empty-mesage\">No Message</span></div></div>";
            template += "<div class='col-xs-12' ng-show='isinsidedialog'>",
                template += " <label class='control-label col-xs-12 no-left-padding' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='input-group col-xs-3 fleft'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' />",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "<div class='col-xs-9' ng-if='!hideMessageText'><span ng-if='messageid' class=\"file-detail-msg msgfont-11\" ng-bind='newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ></span>";
            template += "<span ng-if='!messageid' class=\"file-empty-mesage\">No Message</span></div></div>";
            template += "</div>";

        }
        else {
            template += " <label class='{{messagelabelclass}}' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='{{messagetextboxclass}}'>";
            template += "<div ng-show='!isinsidedialog'>",
                template += "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' ng-blur='onblurcallback()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMessageID'/>",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                template += "<span class='input-group-addon' ng-if='parameterproperty' ng-class='!(lstParameters && lstParameters.length)?\"disabledlink\" : \"\"'  title='Set Message Parameters' ng-click='setMessageParameters()'><i class='fa fa-chevron-right'></i></span>",
                template += "</div>",
                template += "</div>";
            template += "<div  ng-show='isinsidedialog'>",
                template += "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' ng-blur='onblurcallback()' />",
                template += "<span class='input-group-addon button-intellisense' ng-class='isdisabled?\"disabledlink\" : \"\"' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "</div><span ng-if='newDisplayMessage && !hideMessageText'  class=\"help-block msgfont-11\" ng-attr-title='{{newDisplayMessage}}' ng-bind='newDisplayMessage'></span></div>";
            template += "</div>";

        }

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            messageid: "=",
            propertyname: "=",
            showmessagetext: '=',
            messagetext: '=',
            newDisplayMessage: '@',
            newSeverityValue: '@',
            messagelabelclass: "@",
            messagetextboxclass: "@",
            showvertical: "=",
            errorprop: '=',
            isvalidate: '=',
            onchangecallback: '&',
            onblurcallback: '&',
            isinsidedialog: "=",
            isempty: '=',
            isdisabled: '=',
            hideMessageText: "=",
            showTooltipIcons: "=",
            parameterproperty: "=",
            entityid: "="
        },
        link: function (scope, element, attrs) {

            if (scope.messagelabelclass == undefined || scope.messagelabelclass == "") {
                scope.messagelabelclass = "col-xs-4 control-label";
            }
            if (scope.messagetextboxclass == undefined || scope.messagetextboxclass == "") {
                scope.messagetextboxclass = "col-xs-8";
            }
            var parent = $(element).parent();
            if (scope.showvertical) {
                parent.html(getTemplate(scope.showvertical));
            }
            else {
                parent.html(getTemplate(false));
            }
            $compile(parent.contents())(scope);

            hubMain.server.populateMessageList().done(function (lstMessages) {
                scope.$evalAsync(function () {
                    scope.messagelist = lstMessages;
                    if (scope.messageid) {
                        scope.populateMessageForMessage(scope.messageid);
                    }
                });
            });

            scope.$watch("messageid", function (newValue, oldValue) {
                scope.populateMessageForMessage(scope.messageid);
            });
            scope.messageIDTextChanged = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                var value = input.val();
                var data = [];
                if (scope.messagelist) {
                    data = scope.messagelist.sort();
                }
                setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    // setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                // else {
                //    setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                //}
            };
            scope.showMessageIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    setSingleLevelAutoComplete(scope.inputElement, scope.messagelist, scope, "MessageID", "DisplayMessageID");
                }
                scope.inputElement.focus();

                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.populateMessageForMessage = function (messageID) {

                var messageIDFound = false;
                if (messageID && messageID.trim().length > 0) {
                    if (scope.messagelist && scope.messagelist.length > 0) {
                        var messages = scope.messagelist.filter(function (x) {
                            return x.MessageID == messageID;
                        });
                        if (messages && messages.length > 0) {
                            scope.newDisplayMessage = messages[0].DisplayMessage;

                            if (messages[0].SeverityValue == 'I') {
                                scope.newSeverityValue = "Information";
                            }
                            else if (messages[0].SeverityValue == 'E') {
                                scope.newSeverityValue = "Error";
                            }
                            else if (messages[0].SeverityValue == 'W') {
                                scope.newSeverityValue = "Warnings";
                            }
                            scope.newDisplayMessage = String.format("{0}{1}{2}{3}", " ", scope.newSeverityValue, scope.newDisplayMessage && scope.newDisplayMessage.trim().length > 0 ? " : " : "", scope.newDisplayMessage);
                            messageIDFound = true;
                            scope.getParameterByDescription(scope.newDisplayMessage);

                        }
                    }
                }

                if (!messageIDFound) {
                    scope.newDisplayMessage = "";
                    scope.newSeverityValue = "";
                    scope.getParameterByDescription(scope.newDisplayMessage);
                }
                return scope.newDisplayMessage;
            };
            scope.validateMessageID = function () {
                if (scope.propertyname && scope.isvalidate) {
                    var property = scope.propertyname.split('.');
                    property = property[property.length - 1];
                    var list = $ValidationService.getListByPropertyName(scope.messagelist, "MessageID");
                    list.unshift("0"); // deafult message id
                    //$ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "EMPTY_FIELD", CONST.VALIDATION.INVALID_MESSAGE_ID, undefined);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_MESSAGE_ID, undefined, scope.isempty);
                }
                if (scope.onchangecallback) {
                    //message inside model is not updating
                    if (scope.model) {
                        scope.model.messageId = scope.messageid;
                    }
                    scope.onchangecallback();
                }
            };
            scope.getParameterByDescription = function (MessageDescription) {
                if (MessageDescription && MessageDescription.trim().length > 0) {
                    var count = GetTokens(MessageDescription);
                    scope.lstParameters = [];
                    var paramValues = [];
                    if (scope.parameterproperty && scope.model && scope.model[scope.parameterproperty]) {
                        paramValues = scope.model[scope.parameterproperty].split(";");
                    }
                    for (var i = 0; i < count; i++) {
                        var obj = {
                            Id: paramValues[i] ? paramValues[i] : ""
                        };

                        scope.lstParameters.push(obj);
                    }
                }
                else {
                    scope.lstParameters = [];
                }
            };
            scope.setMessageParameters = function () {
                var paramScope = scope.$new();
                paramScope.lstParameters = scope.lstParameters;
                paramScope.model = scope.model;
                paramScope.entityid = scope.entityid;
                paramScope.parameterproperty = scope.parameterproperty;
                paramScope.prepareParams = function () {
                    paramScope.paramstring = "";
                    if (paramScope.lstParameters.length > 0) {
                        var strTemp = "";
                        angular.forEach(paramScope.lstParameters, function (item) {
                            if (item.Id != undefined && item.Id != "") {
                                if (strTemp == "") {
                                    strTemp = item.Id;
                                }
                                else {
                                    strTemp = strTemp + ";" + item.Id;
                                }

                            }
                        });

                        paramScope.paramstring = strTemp;
                    }
                };
                paramScope.prepareParams();
                paramScope.onOKClick = function () {
                    if (scope.model && scope.parameterproperty) {
                        scope.model[scope.parameterproperty] = paramScope.paramstring;
                    }
                    paramScope.paramDialog.close();
                };
                paramScope.template = `<div class="full-height">
                                        <div class="portlet-body">
                                          <table class="sws-table">
                                            <tbody>
                                                <tr ng-repeat="obj in lstParameters">
                                                    <td>
                                                        <entityfieldelementintellisense isshowcolumns="true" isshowproperty="true" isshowexpression="true" isshowonetoone="true" modebinding="obj.Id" model="obj" entityid="entityid" propertyname='"Id"' onblurcallback="prepareParams()">
                                                        </entityfieldelementintellisense>
                                                    </td>
                                               </tr>
                                            </tbody>
                                           </table>
                                         </div>
                                         <div class="col-xs-4 fright">
                                            <input type="button" value="OK" class="btn dialog-btn-default" ng-click="onOKClick()" ng-disabled="portalError" />
                                            <input type="button" value="Cancel" class="btn dialog-btn" ng-click="paramDialog.close()" />
                                         </div>
                                       </div>`;

                paramScope.paramDialog = $rootScope.showDialog(paramScope, "Set Message Parameters", paramScope.template, { isInlineHtml: true, showclose: true });


            };
        }
    };
}]);

app.directive("tablenameintellisense", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            selecteditem: '=',
            onchangecallback: '&',
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selecteditem}}" ng-model="selecteditem" ng-blur="onchangecallback($event)" ng-keydown="onTableNameKeyDown($event)" /><span class="input-group-addon button-intellisense" ng-click="showTableNameList($event)"></span></div>',
        link: function (scope, element, attributes) {

            scope.tablelist = [];
            hubMain.server.getUnusedTableList().done(function (data) {
                scope.tablelist = data;
            });

            scope.onTableNameKeyDown = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        event.preventDefault();
                    }
                    else if (scope.tablelist) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.tablelist, scope);
                        //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.showTableNameList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.tablelist, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
        },
        //template: 
    };
}]);

app.directive("locationintellisensetemplate", ["$compile", "$rootScope", "ConfigurationFactory", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, ConfigurationFactory, $ValidationService, CONSTANTS) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            locationname: "=",
            locationchanged: "&",
            islocationdisabled: "=",
            model: "=",
            validate: "=",
            validateLocationCallback: "=",
            errorprop: "=",
            hideCreateButton: "="
        },
        template: "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input type='text' id='ScopeId_{{$id}}' title='{{locationname}}' ng-model='locationname' ng-disabled='islocationdisabled' ng-keydown='locationNameTextChanged($event)' ng-keyup='triggerChange($event)' ng-change='locationCallback()' ng-blur='locationCallback();' class='form-control input-sm'/><span class='input-group-addon button-intellisense' ng-disabled='islocationdisabled' ng-click='showlocationNameList($event)'></span><span class='input-group-btn' style='left: 6px;'><img ng-hide='hideCreateButton' ng-disabled='disableCreateNewFolderBtn' src='images/Common/new-folder.svg' style='height: 22px;width: 22px;' ng-click='createNewDirectory()' alt='New Folder' title='New Folder' /></span></div>",
        link: function (scope, element, attributes) {
            scope.folderlist = [];
            scope.disableCreateNewFolderBtn = true;
            scope.receiveList = function (data) {
                scope.$evalAsync(function () {
                    scope.folderlist = data;
                    setSingleLevelAutoComplete(scope.inputElement, scope.folderlist);

                    if (scope.checkValidation) {
                        scope.checkValidation = false;
                        scope.checkAndValidatePath();
                    }
                    else {
                        scope.inputElement.focus();
                        if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        }
                    }
                });
            };
            if (scope.validateLocationCallback) {
                scope.validateLocationCallback = function () {
                    scope.folderlist = [];
                    scope.locationCallback();
                }
            }
            scope.locationCallback = function (isNewFolder) {
                if (isNewFolder != "newFolder") {
                    if (!scope.inputElement || scope.folderlist.length === 0) {
                        scope.inputElement = $(document.getElementById("ScopeId_" + scope.$id));
                        scope.getFolderPath();
                        scope.checkValidation = true;
                    }
                    else {
                        scope.checkAndValidatePath();
                    }
                }
                scope.locationchanged({ locationname: scope.locationname });
            };
            scope.checkAndValidatePath = function () {
                scope.validatePath();
                scope.checkValidPath();
            };
            scope.getFolderPath = function () {
                var basePath = ConfigurationFactory.getLastProjectDetails().BaseDirectory;
                var serachTerm = "";
                if (scope.inputElement) {
                    serachTerm += $(scope.inputElement).val();
                }
                hubMain.server.getFolderList(serachTerm).done(function (data) {
                    scope.$evalAsync(function () {
                        if (data) {
                            scope.receiveList(data);
                        }
                        else {
                            scope.receiveList([]);
                        }
                    });
                });
            };
            scope.locationNameTextChanged = function (event) {
                if (event && (event.keyCode === 38 || event.keyCode === 40)) { // press up or down key    
                    return;
                }
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                    scope.getFolderPath();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.getFolderPath();
                    event.preventDefault();
                }
                else if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && !scope.folderlist) {
                    event.preventDefault();
                }
            };
            scope.triggerChange = function (event) {
                //scope.locationchanged();
                if (scope.inputElement && $(scope.inputElement).val() /* && $(scope.inputElement).val().endsWith("\\") */) {
                    if (event.keyCode !== $.ui.keyCode.SPACE && event.keyCode !== 17 && event.keyCode !== 38 && event.keyCode !== 40) {
                        scope.getFolderPath();
                    }
                }
                scope.locationchanged({ locationname: scope.locationname });
            };
            scope.showlocationNameList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    scope.getFolderPath();
                }
                else if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete') && scope.folderlist && scope.folderlist.length > 0) {
                    scope.inputElement.focus();
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.createNewDirectory = function () {
                var folderName = scope.inputElement && $(scope.inputElement).val();
                $.connection.hubCreateNewObject.server.getNonExistsDirectories(folderName).done(function (data) {
                    if (data) {

                        var yesOrNo = confirm("Do you want to create \n" + data + "\n folder(s)?");
                        if (yesOrNo) {
                            $.connection.hubCreateNewObject.server.createNewDirectory(null, folderName).done(function (data) {
                                scope.$evalAsync(function () {
                                    alert("Created " + data.newFolders.toString() + " folders");
                                    if (scope.model && scope.model.errors) {
                                        delete scope.model.errors.invalid_path;
                                    }
                                    scope.locationCallback("newFolder");
                                    scope.disableCreateNewFolderBtn = true;
                                });
                            });
                        }
                    }
                });
            };
            scope.checkValidPath = function () {
                if (scope.validate && scope.model && scope.model.errors && scope.model.errors.invalid_path == "Folder Not Exist" && scope.locationname[scope.locationname.length - 1] != "\\") {
                    scope.disableCreateNewFolderBtn = false;
                }
                else {
                    scope.disableCreateNewFolderBtn = true;
                }
            }

            scope.validatePath = function () {
                if (scope.validate && scope.model) {
                    $ValidationService.checkValidListValue(scope.folderlist, scope.model, $(scope.inputElement).val(), null, "invalid_path", "Folder Not Exist", undefined, false, true, true, true);
                    if (validatePathForSpecialChar(scope.locationname)) {
                        scope.model.errors.invalid_path = 'Invalid Folder Path. It cannot contain any of the following special characters < > / : | * " : and also cannot start or end with \\ ';
                    }
                }
            };
        }
    };
}]);

app.directive("businessobjectintellisensetemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            busobjectname: "=",
            busobjectchange: "&",
            isentitycreation: "=?",
            isonlybusobject: "=",
            isbusobjectreadonly: "=",
            onvalidselection: "&",
            model: "=",
            propertyname: "=",
            onblurcallback: "&",
            undorelatedfunction: "=",
        },
        template: "<div class='input-group'><input ng-change='onChange($event)' type='text' id='ScopeId_{{$id}}' ng-blur='onblurcallback($event)' ng-disabled='isbusobjectreadonly' title='{{busobjectname}}' ng-model='busobjectname' ng-keyup='triggerChange($event)' ng-keydown='busObjectNameTextChanged($event)' class='form-control input-sm' undoredodirective model='model' propertyname='propertyname' undorelatedfunction='undorelatedfunction'/><span ng-disabled='isbusobjectreadonly' class='input-group-addon button-intellisense' ng-click='setbusobjectclick($event)'></span></div>",
        link: function (scope, element, attributes) {
            if (!scope.isentitycreation) {
                scope.isentitycreation = false;
            }

            hubMain.server.getLstBusinessObject(scope.isentitycreation).done(function (data) {
                scope.busobjectlist = data;
            });


            scope.triggerChange = function (event) {
                scope.busObjectNameTextChanged(event);
            };

            scope.setbusobjectclick = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (!scope.busobjectlist) {
                    hubMain.server.getLstBusinessObject(scope.isentitycreation).done(function (data) {
                        scope.busobjectlist = data;
                    });
                }
                if (scope.inputElement && scope.busobjectlist && scope.busobjectlist.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.busObjectNameTextChanged = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    var value = scope.inputElement.val();
                    var businessobjectList = [];
                    if (!scope.busobjectlist) {
                        hubMain.server.getLstBusinessObject(scope.isentitycreation).done(function (data) {
                            scope.busobjectlist = data;
                        });
                    }
                    if (scope.busobjectlist && scope.busobjectlist.length > 0) {
                        var data = scope.busobjectlist.sort();
                        //if (scope.isonlybusobject) {
                        //    data = data.filter(function (x) {
                        //        return x.ID.match("^bus");
                        //    });
                        //}
                        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", value);
                            event.preventDefault();
                        } else {
                            if (data && data.length > 100) {
                                for (var i = 0; i < data.length; i++) {
                                    if (businessobjectList.length < 100) {
                                        if (data[i].ID.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                                            businessobjectList.push(data[i]);
                                        }
                                    } else {
                                        break;
                                    }
                                }
                            } else {
                                businessobjectList = data;
                            }
                            setSingleLevelAutoComplete(scope.inputElement, businessobjectList, scope);
                        }
                    }
                }
            };

            scope.onChange = function () {
                if (scope.busobjectlist && scope.busobjectlist.length > 0 && scope.onvalidselection) {
                    var filter = function (item) {
                        return item.ID == scope.busobjectname;
                    };
                    var items = scope.busobjectlist.filter(filter);
                    if (items && items.length > 0) {
                        if (scope.model && scope.propertyname && getPropertyValue(scope.model, scope.propertyname) != scope.busobjectname) {
                            setPropertyValue(scope.model, scope.propertyname, scope.busobjectname);
                        }
                        scope.onvalidselection({ objEntityBusinessObject: scope.busobjectname });
                    }
                }
            };
        },
        //template: 
    };
}]);

app.directive("entityqueryintellisense", ['$compile', '$rootScope', '$getQueryparam', '$EntityIntellisenseFactory', "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $getQueryparam, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            selecteditem: '=',
            propertyname: '=',
            querytype: '=',
            onchangecallback: '&',
            model: '=',
            disableflag: '=',
            parametermodel: '=',
            showsearchbutton: '=',
            isbpm: '=',
            parameterallowedvalues: '=',
            isnavigatetoentity: '=',
            isscalarquery: '=',
            errorprop: '=',
            validate: '=',
            shownonparameterisedquery: '=',
            isinsidedialog: "@",
            blnWithoutParameter: "=",
            isempty: '=',
        },
        template: "<div style='position:relative'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group full-width' ng-show='!isinsidedialog'><input  type='text' title='{{selecteditem}}' ng-model='selecteditem' ng-change='onchange($event)' ng-disabled='disableflag' ng-keyup='onkeyUp($event);validateQuery()' ng-keydown='onKeyDown($event)' class='form-control input-sm' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateQuery' ng-change='clearParamters()'/><span ng-hide='disableflag' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span><span ng-show='showsearchbutton && !disableflag && !isbpm' class='input-group-addon btn-search-popup' ng-click='onBrowseForQuery()'></span> <span class='input-group-addon' ng-if='!disableflag && isbpm'  title='set Parameters' ng-click='setQueryParameters()'><i class='fa fa-chevron-right'></i></span><button class='input-group-addon' ng-if='model.dictAttributes.sfwLoadType == \"Query\" && isnavigatetoentity'  title={{model.dictAttributes.sfwLoadSource}} ng-click='NavigateToQueryFromBusMethod(model.dictAttributes.sfwLoadSource)' ng-disabled='isDisableNavigateQuery(model.dictAttributes.sfwLoadSource)'><i class='fa fa-chevron-right'></i></button></div><div class='input-group full-width' ng-show='isinsidedialog'><input  type='text' title='{{selecteditem}}' ng-model='selecteditem' ng-change='onchange($event);validateQuery()' ng-disabled='disableflag' ng-keyup='onkeyUp($event);validateQuery()' ng-keydown='onKeyDown($event)' class='form-control input-sm'/><span ng-hide='disableflag' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span><span ng-show='showsearchbutton && !disableflag && !isbpm' class='input-group-addon btn-search-popup' ng-click='onBrowseForQuery()'></span> <span class='input-group-addon' ng-if='!disableflag && isbpm'  title='set Parameters' ng-click='setQueryParameters()'><i class='fa fa-chevron-right'></i></span><button class='input-group-addon' ng-if='model.dictAttributes.sfwLoadType == \"Query\" && isnavigatetoentity'  title={{model.dictAttributes.sfwLoadSource}} ng-click='NavigateToQueryFromBusMethod(model.dictAttributes.sfwLoadSource)' ng-disabled='isDisableNavigateQuery(model.dictAttributes.sfwLoadSource)'><i class='fa fa-chevron-right'></i></button></div></div>",
        link: function (scope, element, attributes) {
            if (scope.propertyname) {
                var propertyName = scope.propertyname.split('.');
                scope.property = propertyName[propertyName.length - 1];
            }
            if (scope.querytextboxclass == undefined) scope.querytextboxclass = "col-xs-8";
            scope.setAutocomplete = function (input, data, event) {
                scope.isMultilevelActive = false;
                var propertyName = "ID";
                var arrText = getSplitArray(input.val().trim(), input[0].selectionStart);
                if (arrText[arrText.length - 1] == "") {
                    arrText.pop();
                }
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        var item = data.filter(function (x) {
                            return x.ID == arrText[index];
                        });
                        if (item.length > 0) {
                            if (item[0].Queries) {
                                scope.isMultilevelActive = true;
                                propertyName = "";
                                if (scope.querytype == "SubSelectQuery") {
                                    data = item[0].Queries.filter(function (x) { return x.QueryType == "SubSelectQuery"; });
                                }
                                else if (scope.querytype == "ScalarQuery") {
                                    data = item[0].Queries.filter(function (x) { return x.QueryType == "ScalarQuery"; });
                                }
                                else {
                                    //in normal entity query intellisense sub select query should not be displayed so this check is added
                                    data = item[0].Queries.filter(function (x) { return x.QueryType != "SubSelectQuery"; });
                                    //data = item[0].Queries;
                                }
                                if (scope.shownonparameterisedquery) {
                                    data = data.filter(function (x) { return !x.Parameters || (x.Parameters && x.Parameters.length == 0); });
                                }
                            }
                            else if ($(input).data('ui-autocomplete') != undefined) {
                                $(input).autocomplete("disable");
                            }
                        }
                        // first level value is not valid -- second level is disabled
                        else if ($(input).data('ui-autocomplete') != undefined) {
                            $(input).autocomplete("disable");
                        }
                    }
                }
                setEntityQueryIntellisense(input, data, scope, propertyName);
            };
            scope.onkeyUp = function (event) {
                // this enables the second level autocomplete
                if (scope.inputElement) {
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    if ((arrText.length > 1 && !scope.isMultilevelActive) || arrText.length == 0) {
                        scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                    } else {
                        if (arrText.length > 2) {
                            if ($(scope.inputElement).data('ui-autocomplete')) {
                                $(scope.inputElement).autocomplete("disable");
                            }
                        }
                    }
                }
            };
            scope.onKeyDown = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    scope.setAutocomplete(scope.inputElement, entityIntellisenseList, event);
                }
                var IsautocompleteSet = scope.inputElement.data('ui-autocomplete') != undefined;
                var Isdisabled = IsautocompleteSet ? $(scope.inputElement).autocomplete("option", "disabled") : false;
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    event.preventDefault();
                } else if (IsautocompleteSet && Isdisabled && !event.ctrlKey) {
                    $(scope.inputElement).autocomplete("enable");
                }

                var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                // if user goes back from second level to first level intellisense
                if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                }
                scope.inputElement.focus();
                var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                if (arrText.length > 1) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                // switch back from multilevel to single level  
                else if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                if ($(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                }
            };
            scope.onchange = function () {

                var aQueryParamColl = $getQueryparam.get(scope.selecteditem);
                if (scope.selecteditem != undefined && attributes.parametermodel && aQueryParamColl) {
                    if (scope.isinsidedialog) {
                        scope.parametermodel = aQueryParamColl;
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.parametermodel, scope, 'parametermodel', aQueryParamColl);
                    }
                }
                scope.validateQuery();

                if (scope.onchangecallback) {
                    if (scope.selecteditem != undefined) {
                        if (scope.model) {
                            if (scope.propertyname && !scope.tempModel) {
                                scope.tempModel = "model";
                                var properties = scope.propertyname.split(".");
                                for (var i = 0, l = properties.length; i < l - 1; i++) {
                                    scope.tempModel = scope.tempModel.concat("." + properties[i]);
                                }
                            }
                            if (scope.tempModel) {
                                var properties = scope.propertyname.split(".");
                                //var t = eval(scope.tempModel);
                                var t;
                                var larrProps = scope.tempModel.split(".");
                                var ltmpObject = scope;
                                for (var i = 0, len = larrProps.length; i < len; i++) {
                                    ltmpObject = ltmpObject[larrProps[i]];
                                }
                                if (ltmpObject !== null) {
                                    t = ltmpObject;
                                }
                                t[properties[properties.length - 1]] = scope.selecteditem;
                            }
                            scope.onchangecallback({ QueryID: scope.selecteditem });
                        } else {
                            scope.onchangecallback({ QueryID: scope.selecteditem });
                        }
                    }
                }
            };
            scope.onBrowseForQuery = function () {
                var newScope = scope.$new();
                newScope.$on('onQueryClickBPM', function (event, data) {
                    // pass true for setting query params after getting the response 
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        if (scope.isinsidedialog) {
                            newScope.mapObj.dictAttributes.sfwQueryID = data.Id;
                        }
                        else {
                            $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryID, newScope.mapObj.dictAttributes, "sfwQueryID", data.Id);
                        }
                    }
                    else if (newScope.mapObj) {
                        if (scope.isinsidedialog) {
                            newScope.mapObj.sfwQueryID = data.Id;
                        }
                        else {
                            $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryID, newScope.mapObj, "sfwQueryID", data.Id);
                        }
                    }

                    function iterator(value, key) {
                        queryParameters.push(value);
                        this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                    }
                    // if query parameters need to be set
                    if (data.Parameters.length > 0) {
                        var queryParametersDisplay = [];
                        var queryParameters = [];

                        angular.forEach(data.Parameters, iterator, queryParametersDisplay);
                        if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryParameters, newScope.mapObj.dictAttributes, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        else if (newScope.mapObj) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryParameters, newScope.mapObj, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                    }
                });
                newScope.$on('onQueryClick', function (event, data) {
                    if (scope.isinsidedialog) {
                        scope.selecteditem = data;
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.selecteditem, scope, "selecteditem", data); //Fixed Bug 10157:Dirty mark for unsaved changes is not displayed on adding Autocomplete or retrieval query
                    }
                    //if (scope.propertyname) {
                    //    $rootScope.EditPropertyValue(scope.selecteditem, scope, "selecteditem", data);
                    //}
                    //else {
                    //    scope.selecteditem = data;
                    //}
                    scope.onchange(event);
                    if (event) {
                        event.stopPropagation();
                    }
                });
                newScope.mapObj = scope.model;
                newScope.strSelectedQuery = scope.selecteditem;
                newScope.IsBPM = scope.isbpm;
                newScope.subQueryType = scope.querytype;
                if (scope.isbpm) {
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj.dictAttributes, "sfwQueryParameters", ";");
                    }
                    else if (newScope.mapObj) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj, "sfwQueryParameters", ";");
                    }
                }

                newScope.queryValues = scope.parameterallowedvalues;
                newScope.validateQuery = scope.validateQuery;
                newScope.QueryDialog = $rootScope.showDialog(newScope, "Browse Queries", "Form/views/BrowseForQuery.html", {
                    width: 1000, height: 700
                });
            };

            scope.setQueryParameters = function () {
                if (scope.isbpm) {
                    var newScope = scope.$new();
                    newScope.mapObj = scope.model;
                    newScope.queryValues = scope.parameterallowedvalues;
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj.dictAttributes, "sfwQueryParameters", ";");
                    }
                    else if (newScope.mapObj) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj, "sfwQueryParameters", ";");
                    }

                    newScope.onOKClickQueryParameters = function () {
                        var queryParametersDisplay = [];
                        var queryParameters = [];
                        function iterator(value, key) {
                            queryParameters.push(value);
                            this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                        }
                        angular.forEach(newScope.queryParameters, iterator, queryParametersDisplay);
                        if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryParameters, newScope.mapObj.dictAttributes, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        else if (newScope.mapObj) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryParameters, newScope.mapObj, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        newScope.parametersdialog.close();
                    };
                    newScope.parametersdialog = $rootScope.showDialog(newScope, "Set Parameters", "BPM/views/SetQueryParameters.html", {
                        width: 500, height: 500
                    });
                }
            };


            scope.NavigateToQueryFromBusMethod = function (loadSource) {
                var index = loadSource.indexOf(".");
                if (index > -1) {
                    var entityName = loadSource.substring(0, index);
                    var indexOfEntity = null;
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < entityIntellisenseList.length; i++) {
                        if (entityIntellisenseList[i].ID == entityName) {
                            indexOfEntity = i;
                            break;
                        }
                    }
                    var queryName = loadSource.substring(index + 1, loadSource.length);
                    $rootScope.queryID = queryName;
                    var entityScope;
                    for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                        if ($rootScope.lstopenedfiles[i].file.FileName == entityName) {
                            entityScope = getScopeByFileName(entityName);
                            if (entityScope != undefined) {
                                $.connection.hubMain.server.navigateToFile(entityName, "").done(function (objfile) {
                                    $rootScope.openFile(objfile, false);
                                });

                                entityScope.openQueryFromLogicalRule();
                                break;
                            }
                        }
                    }
                    if (entityScope == undefined) {
                        $.connection.hubMain.server.navigateToFile(entityName, "").done(function (objfile) {
                            $rootScope.openFile(objfile, false);
                        });

                    }
                }
            };

            scope.isDisableNavigateQuery = function (loadSource) {
                scope.isscalarquery = false;
                if (loadSource == undefined || loadSource == "") {
                    return true;
                }
                if (loadSource != undefined) {
                    scope.isscalarquery = true;
                    var index = loadSource.indexOf(".");
                    var flag = true;
                    if (index > -1) {
                        var entityName = loadSource.substring(0, index);
                        var indexOfEntity = null;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        for (var i = 0; i < entityIntellisenseList.length; i++) {
                            if (entityIntellisenseList[i].ID == entityName) {
                                indexOfEntity = i;
                                break;
                            }
                        }
                        var queryName = loadSource.substring(index + 1, loadSource.length);
                        if (queryName != undefined && queryName != "" && indexOfEntity) {
                            for (var i = 0; i < entityIntellisenseList[indexOfEntity].Queries.length; i++) {
                                if (entityIntellisenseList[indexOfEntity].Queries[i].ID == queryName) {
                                    if (entityIntellisenseList[indexOfEntity].Queries[i].QueryType == "ScalarQuery") {
                                        scope.isscalarquery = false;
                                    }
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                    return flag;
                }
                return false;
            };
            scope.validateQuery = function () {
                if (scope.property && scope.validate) {
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else
                        if (scope.model && scope.model.dictAttributes) {
                            input = scope.model.dictAttributes[scope.property];
                        }
                    var queryPropList = ["sfwQueryID", "sfwLoadSource", "sfwRetrievalQuery", "sfwAutoQuery", "sfwBaseQuery", "sfwQueryRef", "StrBaseQuery", "loadSource"];
                    var errorMessage = "";
                    if (queryPropList.indexOf(scope.property) > -1) {
                        errorMessage = CONST.VALIDATION.INVALID_QUERY;
                    } else if (scope.property == "sfwCodeTable") {
                        errorMessage = CONST.VALIDATION.INVALID_CODE_TABLE;
                    }
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), scope.model, input, scope.querytype, scope.property, scope.property, errorMessage, undefined, scope.blnWithoutParameter, scope.isempty);
                }
            };

        }
    };
}]);

app.directive("codegroupintellisensetemplate", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {

    var getTemplate = function () {
        var template = ""; //<div class='form-group' >

        template = [" <label class='{{codegrouplabelclass}}' id='ScopeId_{{$id}}' ng-show='showcodegrouptext'>{{codegrouptext}}</label>",
            "<div style='position: relative;' class='{{codegrouptextboxclass}}'>",
            '<span class="info-tooltip dtc-span" ng-if="model.errors.invalid_code_group" ng-attr-title="{{model.errors.invalid_code_group}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            "<div class='input-group'>",
            "<input type='text' ng-class=\"insidegrid?'form-control input-sm form-filter input-sm':'form-control input-sm'\" title='{{codegroupid?codegroupid:model.placeHolder}}' ng-model='codegroupid' ng-keydown='codegroupIDTextChanged($event)' ng-keyup='validateCodeGroup($event)' ng-init='validateCodeGroup()' ng-change='validateCodeGroup()' ng-blur='onblurcallback()' ondrop='return false'  undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateCodeGroup' class='form-control input-sm' placeholder='{{model.placeHolder}}'/> ",
            "<span class='input-group-addon button-intellisense' ng-click='showCodeGroupIDList($event)'></span>",
            "</div>",
            "</div>"];
        template = template.join(' ');

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            codegroupid: "=",
            propertyname: "=",
            insidegrid: "=",
            showcodegrouptext: '=',
            codegrouptext: '=',
            codegrouplabelclass: "@",
            codegrouptextboxclass: "@",
            onblurcallback: '&',
            isDialog: '='
        },
        link: function (scope, element, attrs) {

            if (scope.codegrouplabelclass == undefined || scope.codegrouplabelclass == "") {
                scope.codegrouplabelclass = "col-xs-4 control-label";
            }
            if (scope.codegrouptextboxclass == undefined || scope.codegrouptextboxclass == "") {
                scope.codegrouptextboxclass = "col-xs-8";
            }

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            // var newScope;
            scope.codegrouplist = [];
            scope.receiveList = function (data, inputElement) {
                if (data && data.length > 0) {
                    scope.codegrouplist = data;
                    setSingleLevelAutoComplete(inputElement, scope.codegrouplist, scope, "CodeID", "CodeIDDescription");
                    if (inputElement) {
                        $(inputElement).autocomplete("search", $(inputElement).val());
                    }
                }
            };

            scope.getCodeGroups = function (inputEle) {
                var inputElementForCodeGroup = inputEle;
                $.connection.hubMain.server.getCodeGroups().done(function (data) {
                    scope.$evalAsync(function () {
                        scope.receiveList(data, inputElementForCodeGroup);
                    });
                });
            };

            scope.codegroupIDTextChanged = function (event) {
                if (event && event.keyCode == $.ui.keyCode.TAB) { //  when user press tab key
                    return;
                }
                var input = $(event.target);
                scope.inputElement = input;
                var value = input.val();

                if (scope.codegrouplist.length == 0) {
                    scope.getCodeGroups(input);
                }
                else {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                }
            };

            scope.showCodeGroupIDList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                if (scope.codegrouplist.length == 0) {
                    scope.getCodeGroups(scope.inputElement);
                    scope.inputElement.focus();
                }
                else {
                    scope.inputElement.focus();
                    if (scope.inputElement && scope.codegrouplist) {
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        }
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.validateCodeGroup = function () {
                if (event && event.keyCode == $.ui.keyCode.TAB) { //  when user press tab key
                    return;
                }
                if (scope.propertyname && scope.codegrouplist && scope.codegrouplist.length > 0) {
                    var property = scope.propertyname.split('.')[1];
                    var errorMessage = CONST.VALIDATION.CODE_GROUP_NOT_EXISTS;
                    if (property == "sfwChecklistId") errorMessage = CONST.VALIDATION.CHECKLIST_NOT_EXISTS;
                    var list = $ValidationService.getListByPropertyName(scope.codegrouplist, "CodeID");
                    list.push("0");
                    var isEmpty = false;
                    if (["sfwDropDownList", "sfwCascadingDropDownList", "sfwMultiSelectDropDownList", "sfwListPicker", "sfwListBox", "sfwRadioButtonList", "sfwCheckBoxList", "entity"].indexOf(scope.model.Name) > -1) {
                        var prop = "sfwEntityField";
                        if (scope.model.Name == "sfwCheckBoxList") prop = "sfwCheckBoxField";
                        if (scope.model.dictAttributes[prop] && scope.model.errors && !scope.model.errors[prop]) {
                            if (scope.model.dictAttributes.sfwLoadType == "CodeGroup" && !(scope.model.placeHolder || $(scope.inputElement).val())) {
                                isEmpty = true;
                            }
                        }
                    }
                    //if (!$(scope.inputElement).val()){
                    //    isEmpty = true;
                    //}
                    if (isEmpty) {
                        $ValidationService.checkValidListValue([], scope.model, $(scope.inputElement).val(), "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, undefined, isEmpty);
                    } else {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_code_group", errorMessage, undefined, isEmpty, scope.isDialog);
                    }
                }
            };

        }
    };
}]);

app.directive("codevaluesintellisensetemplate", ["$compile", "$rootScope", "ngDialog", function ($compile, $rootScope, ngDialog) {

    var getTemplate = function () {
        var template = ""; //<div class='form-group' >

        template = ["<label class='{{codevalueslabelclass}}' id='ScopeId_{{$id}}' ng-show='showcodevaluestext'>{{codevaluestext}}</label>",
            "<div class='{{codevaluestextboxclass}}'>",
            "<div class='input-group'>",
            "<input type='text' class='form-control input-sm text-autocomplete' title='{{codevaluesid}}' ng-model='codevaluesid' onkeydown='codevaluesIDTextChanged(event)' ng-blur='codevaluesidchange($event)' ng-change='codevaluesidchange($event)' undoredodirective model='model' propertyname='propertyname' class='form-control input-sm'/>",
            "<span class='button-intellisense input-group-addon' value=''  ng-click='showCheckListIDList($event)'></span>",

            "</div>",
            "</div>"];
        template = template.join(' ');

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            codevaluesid: "=",
            propertyname: "=",
            showcodevaluestext: '=',
            codevaluestext: '=',
            codevalueslabelclass: "@",
            codevaluestextboxclass: "@",
            codedescription: '=',
            codeid: "=",
        },
        link: function (scope, element, attrs) {

            if (scope.codevalueslabelclass == undefined || scope.codevalueslabelclass == "") {
                scope.codevalueslabelclass = "col-xs-4 control-label";
            }
            if (scope.codevaluestextboxclass == undefined || scope.codevaluestextboxclass == "") {
                scope.codevaluestextboxclass = "col-xs-8";
            }

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            var newScope;
            scope.$watch('codeid', function (newVal, oldVal) {
                scope.codevalueslist = [];
                if (newVal) {
                    $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
                }
            });

            scope.codevaluesidchange = function (event) {
                function iterator(item) {
                    if (item.CodeValue == scope.codevaluesid) {
                        scope.codedescription = item.Description;
                    }
                }
                scope.codedescription = "";
                if (!scope.codeid) {
                    scope.codevalueslist = [];
                }
                if (scope.codevalueslist && scope.codevalueslist.length > 0) {

                    angular.forEach(scope.codevalueslist, iterator);
                }
            };

            scope.receiveList = function (data) {
                scope.codevalueslist = data;
                if (newScope) {
                    newScope.codevalueslist = scope.codevalueslist;
                }
                if (scope.codevaluesid && scope.codevalueslist && scope.codevalueslist.length > 0) {
                    scope.codevaluesidchange();
                }
            };


            scope.showCheckListIDList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                if (!scope.codevalueslist) {
                    $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.codevalueslist) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.codevalueslist, scope, "CodeValue", "CodeValueDescription");
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.$watch("codevaluesid", function (newVal, oldVal) {
                scope.codevaluesidchange();
            });
        }
    };
}]);

app.directive("busobjectintellisensetemplate", ["$compile", function ($compile) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            filename: "=",
            propertyname: '=',
            filetype: '=',
            onfilechange: '&',
            isdisabled: '=',
            onfileblur: '&'
        },
        template: "<div class='input-group'><input class='form-control input-sm' type='text' ng-blur='onblur()' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-change='onchangeCallback()' undoredodirective  model='model' propertyname='propertyname'/><span class='input-group-addon button-intellisense' ng-click='showBusObjectIntellisenseList($event)'></span></div>",
        link: function (scope, element, attrs) {


            scope.receiveList = function (data) {
                scope.busobjectlist = data;
                if (scope.busobjectlist) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope, "Description", "Description");
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.onchangeCallback = function () {
                var busobject;
                if (scope.busobjectlist) {
                    busobject = scope.busobjectlist.filter(function (x) { return x.Description == scope.filename; });
                }
                if (busobject && busobject.length > 0) {
                    scope.model.IsChildOfInboundFileBase = busobject[0].IsChildOfInboundFileBase;
                    scope.model.lstObjectMethod = busobject[0].LstObjectMethods;
                    scope.model.lstFileCollection = busobject[0].LstFieldNames;
                    scope.model.BusinessObject = busobject;
                }
                else {
                    scope.model.IsChildOfInboundFileBase = false;
                    scope.model.lstObjectMethod = [];
                    scope.model.lstFileCollection = [];
                    scope.model.BusinessObject = [];
                }
                if (scope.onfilechange) {
                    scope.onfilechange();
                }
            };
            scope.onblur = function () {
                if (scope.onfileblur) {
                    scope.onfileblur();
                }
            };
            scope.showBusObjectIntellisenseList = function (eargs) {
                if (!scope.busobjectlist) {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target).prevAll("input[type='text']");
                    }
                    $.connection.hubMain.server.getBusObjectsByType(scope.filetype).done(function (result) {
                        var result = JSON.parse(result);
                        scope.receiveList(result);
                    });
                }
                else {
                    scope.inputElement.focus();
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.onkeydown = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (!scope.busobjectlist) {
                    $.connection.hubMain.server.getBusObjectsByType(scope.filetype).done(function (result) {
                        var result = JSON.parse(result);
                        if (event && event.keyCode != $.ui.keyCode.ESCAPE) {
                            scope.receiveList(result);
                        }
                    });
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        event.preventDefault();
                    }
                }
                else {
                    //setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope, "Description", "Description");
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        event.preventDefault();
                    }
                }
            };
        }
    };
}]);

app.directive("entitycolumnintellisense", ['$rootScope', '$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($rootScope, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            formodel: '=',
            modebinding: "=",
            entityid: "=",
            propertyname: '=',
            queryid: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: "=",
            isshowcolumnvalues: "=",
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_data_field' ng-attr-title='{{model.errors.invalid_data_field}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' ng-keydown='filterIntelliseneList($event)' ng-keyup='validateDataField()' ng-change='validateDataField()'/><span class='input-group-addon button-intellisense' ng-click='showEntityColumnIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.$watch('entityid', function (newVal, oldVal) {
                if (newVal) {
                    scope.lstEntityColumnList = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    scope.lstEntityColumnList = getEntityAttributeByType(entityIntellisenseList, scope.entityid, "Column").concat(getEntityAttributeByType(entityIntellisenseList, scope.entityid, "AppJsonData"));
                }
            });

            scope.$watch('queryid', function (newVal, oldVal) {
                scope.PopulateColumnList(newVal);
            });

            scope.PopulateColumnList = function (queryid) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var result = PopulateColumnList(queryid, scope.formodel, entityIntellisenseList, scope.lstloadedentitycolumnstree);
                if (result) {
                    scope.lstColumnList = result.list;
                    scope.attributeName = result.attribute;
                }
                /*      if (queryid) {
                          scope.PopulateQueryColumnFromList(queryid);
                      }
                      else {
                          var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                          if (scope.formodel) {
                              var MainQuery = GetMainQueryFromFormObject(scope.formodel, entityIntellisenseList);
                              if (MainQuery) {
                                  scope.PopulateQueryColumnFromList(MainQuery.dictAttributes.ID);
                              }
                              else {
                                  scope.attributeName = "Value";
                                  if (scope.formodel.dictAttributes.sfwEntity) {
                                      var entities = entityIntellisenseList;
                                      var entity = entities.filter(function (x) {
                                          return x.ID == scope.formodel.dictAttributes.sfwEntity;
                                      });
                                      if (entity.length > 0) {
                                          var attributes = entity[0].Attributes;
                                          scope.lstColumnList = attributes.filter(function (itm) { return itm.Type == "Column" });
                                      }
                                  }
                              }
                          }
                      }
                      */
            };

            scope.PopulateQueryColumnFromList = function (queryid) {
                var blnFound = false;
                if (scope.lstloadedentitycolumnstree) {

                    var lst = scope.lstloadedentitycolumnstree.filter(function (itm) {
                        return itm.EntityName == queryid;
                    }
                    );
                    if (lst && lst.length > 0) {

                        scope.lstColumnList = JSON.parse(JSON.stringify(lst[0].lstselectedobjecttreefields));
                        blnFound = true;
                    }
                }

                if (!blnFound) {
                    var lstQueryID = GetQueryListFromObject(scope.formodel);
                    if (lstQueryID) {

                        var lst = lstQueryID.filter(function (itm) {
                            return itm.dictAttributes.ID == queryid;
                        });
                        if (lst && lst.length > 0) {
                            var objnew = { EntityName: lst[0].dictAttributes.ID, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: true };
                            if (scope.lstloadedentitycolumnstree) {
                                scope.lstloadedentitycolumnstree.push(objnew);
                                if (lst[0].dictAttributes.sfwQueryRef) {
                                    $.connection.hubForm.server.getEntityQueryColumns(lst[0].dictAttributes.sfwQueryRef, "LoadQueryFieldsForLookup").done(function (data) {
                                        var scope = GetFormFileScope(scope.formodel);
                                        if (scope && scope.receiveQueryFields) {
                                            scope.receiveQueryFields(data, lst[0].dictAttributes.sfwQueryRef);
                                        }
                                    });
                                }
                                blnFound = true;
                            }
                            if (!objnew.lstselectedobjecttreefields) {
                                objnew.lstselectedobjecttreefields = [];
                            }
                            scope.lstColumnList = objnew.lstselectedobjecttreefields;
                        }
                    }
                }
            };

            scope.filterIntelliseneList = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        //setAutoComplete(scope.inputElement);
                        if ($(scope.inputElement).data('ui-autocomplete') && (scope.lstEntityColumnList || scope.lstColumnList)) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        setAutoComplete(scope.inputElement);
                    }

                    //event.preventDefault();
                    //if (event.stopPropagation) {
                    //    event.stopPropagation();
                    //}
                }

            };

            scope.showEntityColumnIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                setAutoComplete(inputElement);
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };

            var setAutoComplete = function (input) {
                if (scope.entityid != undefined && scope.entityid != "") {
                    scope.attributeName = "ID";
                    if (scope.isshowcolumnvalues) {
                        scope.attributeName = "Value";
                    }
                    setSingleLevelAutoComplete(input, scope.lstEntityColumnList, scope, scope.attributeName, scope.attributeName);
                    //$(input).autocomplete("search", $(input).val());
                }
                else if (scope.lstColumnList) {
                    scope.PopulateColumnList(scope.queryid);
                    setSingleLevelAutoComplete(input, scope.lstColumnList, scope, scope.attributeName, scope.attributeName);
                    //$(input).autocomplete("search", $(input).val());

                }
            };

            scope.validateDataField = function () {
                var property = "sfwDataField";
                var list = [];
                /*    if (scope.entityid) {
                        list = scope.lstEntityColumnList;
                    }
                    else if (scope.lstColumnList) {
                        list = scope.lstColumnList;
                    }
                    $ValidationService.checkDataFieldValue(list, scope.model, $(scope.inputElement).val(), scope.attributeName, property, 'invalid_data_field', CONST.VALIDATION.INVALID_DATA_FIELD, undefined);
                */
            };
        }
    };
}]);

app.directive("entityexpressionintellisense", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modelvalue: "=",
            entityid: "=",
            propertyname: "="
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_expression' ng-attr-title='{{model.errors.invalid_expression}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateExpression' ng-keydown='getEntityExpressionList($event)' ng-keyup='validateExpression()' ng-change='validateExpression()'/>	<span class='input-group-addon button-intellisense' ng-click='showExpressionIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.createExpressionList = function () {
                scope.lstEntityExpressionList = [];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                scope.lstEntityExpressionList = getEntityAttributeByType(entityIntellisenseList, scope.entityid, "Expression");

            };

            scope.getEntityExpressionList = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                scope.createExpressionList();
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                } else {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(input, scope.lstEntityExpressionList, scope);
                    }
                }
            };

            scope.showExpressionIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                scope.createExpressionList();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.lstEntityExpressionList, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.validateExpression = function () {
                if (scope.propertyname) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstEntityExpressionList, "ID", false);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_expression", CONST.VALIDATION.INVALID_EXPRESSION, undefined);
                }
            };
        }
    };
}]);

app.directive("entitymethodsintellisense", ['$rootScope', '$filter', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', '$Entityintellisenseservice', function ($rootScope, $filter, $ValidationService, CONST, $NavigateToFileService, $Entityintellisenseservice) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            entityid: "=",
            modelvalue: "=",
            propertyname: "=",
            showonlycollection: "=",
            showonlyobject: "=",
            showonlyobjectmethods: "=",
            showrules: "=",
            showxmlmethods: "=",
            errorprop: "=",
            validate: "=",
            methodtext: "=",
            onchangecallback: '&',
            onblurcallback: '&'
        },
        template: "<div><label class='control-label col-xs-5' ng-if='methodtext && (!modelvalue || !modelvalue.trim())'>{{methodtext}}</label><label class='control-label col-xs-5' ng-if='methodtext && modelvalue && modelvalue.trim()'><a ng-click='navigateToMethodClick(modelvalue)'>{{methodtext}}</a></label><div ng-class='methodtext?\"col-xs-7\":\"col-xs-12\" '><div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ondrop='return false' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMethod' ng-keydown='getEntityMethodsList($event)' ng-keyup='validateMethod()' ng-change='validateMethod();onChange()' ng-blur='onblur()'/>	<span class='input-group-addon button-intellisense' ng-click='showEntityMethodsIntellisenseList($event)'></span></div></div></div>",
        link: function (scope, element, attributes) {
            scope.createMethodsList = function () {
                var lst = null;
                scope.lstMethods = [];

                if (scope.model && scope.model.dictAttributes.sfwExecuteMethodType) {
                    scope.showrules = false;
                    scope.showonlyobjectmethods = false;
                    scope.showxmlmethods = false;

                    if (scope.model.dictAttributes.sfwExecuteMethodType == "Rule") {
                        scope.showrules = true;
                    }
                    else if (scope.model.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                        scope.showonlyobjectmethods = true;
                    }
                    else if (scope.model.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                        scope.showxmlmethods = true;
                    }
                }
                if (scope.showrules) {
                    var tempRuleList = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, false, true, false, false);
                    if (tempRuleList && tempRuleList.length > 0) {
                        lst = tempRuleList.filter(function (itm) { return !itm.IsPrivate && !itm.IsStatic && itm.Status == "Active" });
                    }
                }
                else if (scope.showonlyobjectmethods) {
                    var lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, true, false, false, false);

                }
                else if (scope.showxmlmethods) {
                    var lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, false, false, false, true);

                }
                else {
                    lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, true, false, false, false);
                }

                if (lst && lst.length > 0) {
                    if (scope.showonlycollection) {
                        lst = $filter("filter")(lst, { ReturnType: "Collection" });
                        if (lst && lst.length > 0) {
                            scope.lstMethods = lst;
                        }
                    }
                    else if (scope.showonlyobject) {
                        lst = $filter("filter")(lst, { ReturnType: "Object" });
                        if (lst && lst.length > 0) {
                            scope.lstMethods = lst;
                        }
                    }
                    else {
                        angular.forEach(lst, function (method) {
                            if (method && method.ID) {
                                scope.lstMethods.push(method);
                            }
                        });
                    }
                }
            };

            scope.getEntityMethodsList = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                scope.createMethodsList();
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    if (scope.entityid && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    }
                    event.preventDefault();
                } else {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(input, scope.lstMethods, scope);
                    }
                }
            };

            scope.showEntityMethodsIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                scope.createMethodsList();
                if (scope.inputElement) {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(scope.inputElement, scope.lstMethods, scope);
                        if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                    }

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.onChange = function () {
                if (scope.onchangecallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.modelvalue);
                    }
                    scope.onchangecallback();
                }
            };

            scope.validateMethod = function () {
                if (scope.propertyname && scope.validate) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else input = scope.model.dictAttributes[property];
                    var list = $ValidationService.getListByPropertyName(scope.lstMethods, "ID", false);
                    var errorProp = "";
                    if (property && (property == "sfwXmlMethod" || property == "sfwLoadSource" || property == "sfwObjectMethod")) {
                        errorProp = "invalid_method";
                    } else if (property == "sfwRetrievalMethod") {
                        errorProp = "invalid_retrieval_method";
                    }
                    if (input) {
                        $ValidationService.checkValidListValue(list, scope.model, input, property, errorProp, CONST.VALIDATION.INVALID_METHOD, undefined);
                    } else {
                        $ValidationService.checkValidListValue(["null"], scope.model, "null", property, errorProp, CONST.VALIDATION.INVALID_METHOD, undefined);
                    }
                }
            };

            scope.navigateToMethodClick = function (aMethodID) {
                if (!scope.lstMethods) {
                    scope.createMethodsList();
                }
                scope.validateMethod();
                var errorProp = "";
                if (scope.propertyname) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    if (property && (property == "sfwXmlMethod" || property == "sfwLoadSource")) {
                        errorProp = "invalid_method";
                    } else if (property == "sfwRetrievalMethod") {
                        errorProp = "invalid_retrieval_method";
                    }
                }
                var IsNavigate = false;
                if (!scope.model.errors) {
                    IsNavigate = true;
                }
                else if (scope.model.errors && !scope.model.errors[errorProp]) {
                    IsNavigate = true;
                }
                if (aMethodID && scope.entityid && IsNavigate) {
                    if (scope.lstMethods && scope.lstMethods.length > 0) {
                        if (scope.model.dictAttributes && scope.model.dictAttributes.sfwExecuteMethodType) {
                            if (scope.model.dictAttributes.sfwExecuteMethodType == "Rule") {
                                $NavigateToFileService.NavigateToRule(aMethodID);
                            }
                            else if (scope.model.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                                $NavigateToFileService.NavigateToFile(scope.entityid, "objectmethods", aMethodID);
                            }

                            else if (scope.model.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                                $NavigateToFileService.NavigateToFile(scope.entityid, "methods", aMethodID);
                            }
                        }
                        else {
                            $NavigateToFileService.NavigateToFile(scope.entityid, "objectmethods", aMethodID);
                        }
                    }

                }
            };
            scope.onblur = function () {
                if (scope.onblurcallback) {
                    scope.onblurcallback();
                }
            }
        }
    };
}]);

app.directive("objectfieldintellisense", [function () {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modelvalue: "=",
            parameterlist: "=",
            propertyname: "="
        },
        template: "<div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' ng-keydown='getObjectFieldList($event)'/>	<span class='input-group-addon button-intellisense' ng-click='showObjectFieldIntellisenseList($event)'></span></div>",
        link: function (scope, element, attributes) {

            var filterParameterList = function () {
                if (scope.parameterlist && scope.parameterlist.length > 0) {
                    scope.paramNameList = [];
                    angular.forEach(scope.parameterlist, function (value, key) {
                        if ("ParamName" in value) {
                            scope.paramNameList.push(value.ParamName);
                        }
                    });
                }
            };
            scope.getObjectFieldList = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (scope.paramNameList && scope.paramNameList.length > 0 && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        if (scope.paramNameList && scope.paramNameList.length > 0) {
                            setSingleLevelAutoComplete(scope.inputElement, scope.paramNameList, scope);
                        }
                    }

                    //event.preventDefault();
                    if (!(event.ctrlKey && (event.keyCode === 83 || event.keyCode === 90 || event.keyCode === 89)) && event.stopPropagation) { // added "ctrl+S" condition coz while doing ctrl+s it was opening webpage save dialog rather saving the file .(by shilpi)
                        event.stopPropagation();
                    }
                }
            };

            scope.showObjectFieldIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.paramNameList && scope.paramNameList.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.paramNameList, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
            filterParameterList();
        }
    };
}]);

app.directive("commonIntellisense", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            selectedItem: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            updateDataCallback: '&',
            onblurcallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            isReset: "=",
            errorprop: "=",
            validate: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    if (scope.updateDataCallback) {
                        scope.updateDataCallback();
                    }
                    scope.initializeIntellisense();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedItem);
                    }
                    scope.changeCallback();
                }
                scope.validateInput();
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                    if (property == "sfwEntityField") errorMessage = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    //if (!$(scope.inputElement).val()) {
                    //    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined,true);
                    //}
                    //else {
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined);
                }
                //}
            };
        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedItem}}" ng-model="selectedItem" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)" ng-blur="onblurcallback()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateInput"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);
app.directive("ruleIntellisense", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", "$EntityIntellisenseFactory", function ($compile, $rootScope, $ValidationService, CONST, $EntityIntellisenseFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            entityId: "=",
            nonStatic: "=",
            returnTypes: "=",
            ruleTypes: "=",
            selectedRule: '=',
            changeCallback: '&',
            onblurcallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            vErrorprop: "=",
            vValidate: "=",
            vAllowEmpty: "=",
            vFieldName: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (eargs) {
                var input = eargs.target;

                var arrText = getSplitArray(input.value, input.selectionStart);

                //Get all the entities which have static rule of the specified rule type.
                var requiredEntities = [];
                if (!scope.nonStatic) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    requiredEntities = entities.filter(function (x) { return x.Rules.some(function (element, index, array) { return element.IsStatic && ((scope.entityId && x.ID == scope.entityId) || !element.IsPrivate); }); });
                }

                var rules = [];
                var lstObject = [];

                //If entity id is provided, then load non-static rules of the entity.
                var lstRulesAndObject = [];
                if (scope.entityId) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityname = scope.entityId;
                    while (parententityname != "") {
                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            rules = entity[0].Rules.filter(function (y) { return !y.IsStatic && (parententityname == scope.entityId || !y.IsPrivate); });
                            lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                            if (scope.entityId != parententityname) {
                                rules = rules.concat(lstObject);
                            }
                            parententityname = entity[0].ParentId;
                            lstRulesAndObject = lstRulesAndObject.concat(rules);
                        } else {
                            parententityname = "";
                        }
                    }
                }

                var data = requiredEntities.concat(lstRulesAndObject);

                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        var item = data.filter(function (x) { return x.ID == arrText[index]; });
                        if (item.length > 0 && typeof item[0].Rules != "undefined" && index < arrText.length) {
                            if (item[0].ID == arrText[index]) {
                                data = item[0].Rules.filter(function (y) { return y.IsStatic && ((scope.entityId && item[0].ID == scope.entityId) || !y.IsPrivate); });
                            }
                        }
                        else if (item.length > 0 && item[0].DataType != undefined && index < arrText.length) {
                            if (item[0].DataType == "Object" && item[0].ID == arrText[index]) {
                                parententityname = item[0].Entity;
                                data = [];
                                while (parententityname != "") {
                                    var entity = entities.filter(function (x) { return x.ID == parententityname; });
                                    if (entity && entity.length > 0) {
                                        data = data.concat(entity[0].Rules.filter(function (y) { return !y.IsStatic && !y.IsPrivate; }));
                                        lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                                        data = data.concat(lstObject);
                                        parententityname = entity[0].ParentId;
                                    } else {
                                        parententityname = "";
                                    }
                                }
                            }
                        }
                        else if (item.length > 0 && item[0].RuleType != undefined && index < arrText.length) {
                            if (item[0].RuleType == "LogicalRule" || item[0].RuleType == "DecisionTable" || item[0].RuleType == "ExcelMatrix") {
                                data = [];
                            }
                        } else {
                            if (index != arrText.length - 1) {
                                data = [];
                            }
                        }
                    }
                }

                // filtering rules
                if (scope.ruleTypes) {
                    for (var index = 0, len = scope.ruleTypes.length; index < len; index++) {
                        scope.ruleTypes[index] = scope.ruleTypes[index].toLowerCase();
                    }
                }
                if (scope.returnTypes) {
                    for (var index = 0, len = scope.returnTypes.length; index < len; index++) {
                        scope.returnTypes[index] = scope.returnTypes[index].toLowerCase();
                    }
                }
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) {
                            return x.ID.toLowerCase().contains(arrText[index].toLowerCase())
                                &&
                                (!x.Status || x.Status.toLowerCase() === "active")
                                &&
                                (!x.RuleType || !scope.ruleTypes || !scope.ruleTypes.length || scope.ruleTypes.indexOf(x.RuleType.toLowerCase()) > -1)
                                &&
                                (!x.ReturnType || !scope.returnTypes || !scope.returnTypes.length || scope.returnTypes.indexOf(x.ReturnType.toLowerCase()) > -1);
                        });
                    }
                    data = item;
                }


                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                scope.validate();
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedRule);
                    }
                    scope.changeCallback();
                }
            };
            scope.validate = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    $ValidationService.validateRule(scope.model, scope.selectedRule, scope.entityId, scope.nonStatic, scope.returnTypes, scope.ruleTypes, scope.propertyname, scope.vAllowEmpty, scope.vFieldName);
                }
            };
        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="vErrorprop" ng-attr-title="{{vErrorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedRule}}" ng-model="selectedRule" ng-keyup="onKeyDown($event)" ng-change="onChange()" ng-blur="onblurcallback()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateInput"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);

app.directive("commonIntellisenseWithoutUndoRedo", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            selectedItem: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            isReset: "=",
            errorprop: "=",
            validate: "=",
            isinsidedialog: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                scope.validateInput();
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedItem);
                    }
                    scope.changeCallback();
                }
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                    if (property == "sfwEntityField") errorMessage = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined, false, scope.isinsidedialog);
                }
            };
        },
        template: '<div style="position:relative"><span class="info-tooltip dtc-span" ng-show="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><div class="input-group"><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedItem}}" ng-model="selectedItem" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div></div>'
    };
}]);

app.directive("templateNameIntellisense", ["$ValidationService", "CONSTANTS", function ($ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            modelname: '=',
            onchangecallback: '&',
            model: '=',
            propertyname: '=',
            validate: '=',
            errorprop: '='
        },
        link: function (scope, element, attributes) {
            scope.onTemplateNameChange = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    var input = event.target;
                    scope.inputElement = input;
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val().trim());
                        event.preventDefault();
                    } else {
                        populateTemplateNameData(input);
                    }
                }
            };

            scope.showTemplateNameList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                inputElement.focus();
                scope.inputElement = inputElement;
                populateTemplateNameData(inputElement);
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val().trim());
                if (event) {
                    event.stopPropagation();
                }
            };

            var populateTemplateNameData = function (input) {
                if (scope.collection && scope.collection.length > 0) {
                    setSingleLevelAutoComplete($(input), scope.collection);
                }
            };

            scope.$watch('modelname', function (newVal, oldVal) {
                if (scope.onchangecallback) {
                    scope.onchangecallback();
                }
            }, true);
            scope.validateTemplateName = function () {
                if (scope.validate) {
                    var strProp = scope.propertyname.split('.');
                    var property = strProp[strProp.length - 1];
                    $ValidationService.checkValidListValue(scope.collection, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_TEMPLATE, undefined);
                }
            };
        },
        template: '<div><div class="input-group"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input type="text" class="form-control input-sm" title="{{modelname}}" ng-model="modelname" ng-keydown="onTemplateNameChange($event)" ng-keyup="validateTemplateName()" ng-change="validateTemplateName()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateTemplateName"><span class="input-group-addon button-intellisense" ng-click="showTemplateNameList($event);"></span></div></div>'
    };
}]);

app.directive("constantintellisene", ["$compile", "$rootScope", "$resourceFactory", function ($compile, $rootScope, $resourceFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            constantbinding: '=',
            model: '=',
            propertyname: '=',
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
                else {
                    scope.initializeIntellisense();
                }
            };

            scope.onKeyUp = function (event) {
                scope.initializeIntellisense();
            };

            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement) {
                    if ($(scope.inputElement).data('ui-autocomplete') && scope.collection) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.initializeIntellisense = function () {
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                if (!scope.collection) {
                    scope.collection = [];
                }

                scope.collection = $resourceFactory.getConstantsList();
                if (scope.inputElement && scope.inputElement.length > 0) {
                    var arrText = getSplitArray($(scope.inputElement).val(), scope.inputElement[0].selectionStart);
                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            var item = scope.collection.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (item[0].Type && item[0].ID == arrText[index] && item[0].Type == "Constant") {
                                    if (item[0].Type == "Constant") {
                                        scope.collection = $resourceFactory.getConstantsList(arrText.join("."));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (scope.inputElement) {
                    setRuleIntellisense($(scope.inputElement), scope.collection);
                }
                //setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope);
            };
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{constantbinding}}" ng-model="constantbinding" ng-keydown="onKeyDown($event)" ng-keyup="onKeyUp($event)" undoredodirective model="model" propertyname="propertyname"/><span class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>',
    };
}]);

app.directive("entityquerycolumnsintellisense", ['$rootScope', '$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($rootScope, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            formodel: '=',
            modebinding: "=",
            propertyname: '=',
            queryid: '=',
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_data_field' ng-attr-title='{{model.errors.invalid_data_field}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' ng-keydown='filterIntelliseneList($event)'/><span class='input-group-addon button-intellisense' ng-click='showEntityQueryColumnIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {

            var setAutoComplete = function (input, ablnShowIntellisense) {
                if (scope.queryid != undefined && scope.queryid != "") {
                    setSingleLevelAutoComplete(input, scope.lstQueryColumnList, scope, "CodeID", "CodeID");
                    if (ablnShowIntellisense && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    };
                }
            };
            scope.$watch('queryid', function (newVal, oldVal) {
                if (newVal != oldVal) {
                    scope.iblnresetData = true;
                    scope.lstQueryColumnList = [];
                }
            });

            scope.loadQueryColumns = function (ablnShowIntellisense) {
                $.connection.hubForm.server.getEntityQueryColumns(scope.queryid, "").done(function (data) {
                    scope.$evalAsync(function () {
                        scope.lstQueryColumnList = data;
                        if (scope.inputElement) {
                            setAutoComplete(scope.inputElement, ablnShowIntellisense);
                        }
                    });
                });
            }
            scope.filterIntelliseneList = function (event) {
                if (!scope.lstQueryColumnList || scope.iblnresetData) {
                    scope.iblnresetData = false;
                    scope.loadQueryColumns(false);
                }

                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        //setAutoComplete(scope.inputElement);
                        if ($(scope.inputElement).data('ui-autocomplete') && scope.lstQueryColumnList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        setAutoComplete(scope.inputElement);
                    }

                    //event.preventDefault();
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                }
            };

            scope.showEntityQueryColumnIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                if (!scope.lstQueryColumnList || scope.iblnresetData) {
                    scope.loadQueryColumns(true);
                }
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };

        }
    };
}]);

app.directive("rulegroupintellisense", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', function ($EntityIntellisenseFactor, $ValidationService, CONST, $NavigateToFileService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            modelvalue: '=',
            propertyname: '=',
            entityid: "=",
            labelText: "@",
            labelClass: "@",
            errorprop: "=",
            validate: "=",
            callback: "&"
        },
        template: "<div>" +
            "<label class={{labelClass}} ng-if='!modelvalue || errorprop'>{{labelText}} </label>" +
            "<label class={{labelClass}} ng-if='modelvalue && !errorprop'><a ng-click='selectGroupClick(modelvalue)'>{{labelText}}</a></label>" +
            "<div class='col-xs-7'>" +
            "<span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>" +
            "<div class='input-group'>" +
            "<input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateRuleGroup' ng-change='validateRuleGroup()' ng-keyup='LoadRuleList($event)' ng-keydown='filterIntelliseneList($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showRuleGroupIntellisenseList($event)'></span>" +
            "</div></div></div>",
        link: function (scope, element, attributes) {
            var setAutoComplete = function (input) {
                setSingleLevelAutoComplete(input, scope.lstRuleGroupList, scope);

            };

            scope.LoadRuleGroupList = function (isShowList) {
                if (scope.entityid) {
                    hubMain.server.getGroupList(scope.entityid).done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.lstRuleGroupList = scope.getFormatedData(data);
                                if (!scope.inputElement) scope.inputElement = $("#ScopeId_" + scope.$id);
                                if (scope.inputElement) {
                                    $(scope.inputElement).focus();
                                    setAutoComplete(scope.inputElement);
                                    if (isShowList && $(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                                }
                            }

                        });
                    });
                }
            };
            scope.getFormatedData = function (data) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].dictAttributes && data[i].dictAttributes.ID) {
                        tempData.push({ ID: data[i].dictAttributes.ID });
                    }
                }
                return tempData;
            };
            scope.filterIntelliseneList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (!scope.modelvalue || scope.modelvalue == "" || !scope.lstRuleGroupList) {
                            scope.LoadRuleGroupList(true);
                            //if ($(scope.inputElement).data('ui-autocomplete')) 
                            //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                        else if ($(scope.inputElement).data('ui-autocomplete') && scope.lstRuleGroupList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    }
                }
            };

            scope.showRuleGroupIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                scope.LoadRuleList();
                setAutoComplete(inputElement);
                inputElement.focus();

                if (event) {
                    event.stopPropagation();
                }
            };
            scope.LoadRuleList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (!scope.modelvalue || !scope.lstRuleGroupList) {
                    scope.LoadRuleGroupList(true);
                }
                scope.validateRuleGroup();
            };
            scope.selectGroupClick = function (aGroupID) {
                scope.validateRuleGroup();
                var property = scope.propertyname.split('.')[1];

                if (scope.validate && aGroupID && aGroupID != "" && scope.model.errors && !scope.model.errors[property]) {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
                if (!scope.validate && aGroupID && aGroupID != "") {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
            };
            scope.validateRuleGroup = function () {
                if (scope.propertyname && scope.validate) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstRuleGroupList, 'ID');
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_GROUP, undefined);
                }
            };
            scope.LoadRuleGroupList(false);
        }
    };
}]);
app.directive("rulegroupintellisensewithoutundoredo", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', function ($EntityIntellisenseFactor, $ValidationService, CONST, $NavigateToFileService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            modelvalue: '=',
            propertyname: '=',
            entityid: "=",
            labelText: "@",
            labelClass: "@",
            errorprop: "=",
            validate: "=",
            callback: "&"
        },
        template: "<div>" +
            "<label class={{labelClass}} ng-if='!modelvalue || errorprop'>{{labelText}} </label>" +
            "<label class={{labelClass}} ng-if='modelvalue && !errorprop'><a ng-click='selectGroupClick(modelvalue)'>{{labelText}}</a></label>" +
            "<div class='col-xs-7' >" +
            "<span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>" +
            "<div class='input-group'>" +
            "<input  type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue'  ng-change='validateRuleGroup()' ng-keyup='LoadRuleList($event)' ng-keydown='filterIntelliseneList($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showRuleGroupIntellisenseList($event)'></span></div>" +
            "</div>" +
            "</div>",
        link: function (scope, element, attributes) {
            var setAutoComplete = function (input) {
                setSingleLevelAutoComplete(input, scope.lstRuleGroupList, scope);
            };

            scope.LoadRuleGroupList = function (isShowList) {
                if (scope.entityid) {
                    hubMain.server.getGroupList(scope.entityid).done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.lstRuleGroupList = scope.getFormatedData(data);
                                if (!scope.inputElement) scope.inputElement = $("#ScopeId_" + scope.$id);
                                if (scope.inputElement) {
                                    $(scope.inputElement).focus();
                                    setAutoComplete(scope.inputElement);
                                    if (isShowList && $(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                                }
                            }

                        });
                    });
                }
            };
            scope.getFormatedData = function (data) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].dictAttributes && data[i].dictAttributes.ID) {
                        tempData.push({ ID: data[i].dictAttributes.ID });
                    }
                }
                return tempData;
            };
            scope.filterIntelliseneList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (!scope.modelvalue || scope.modelvalue == "" || !scope.lstRuleGroupList) {
                            scope.LoadRuleGroupList(true);
                            //if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                        else if ($(scope.inputElement).data('ui-autocomplete') && scope.lstRuleGroupList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    }
                }
            };

            scope.showRuleGroupIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                scope.LoadRuleList();
                setAutoComplete(inputElement);
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.LoadRuleList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (scope.modelvalue == "" || !scope.lstRuleGroupList) {
                    scope.LoadRuleGroupList(true);
                }
                scope.validateRuleGroup();
            };
            scope.selectGroupClick = function (aGroupID) {
                scope.validateRuleGroup();
                var property = scope.propertyname.split('.')[1];

                if (scope.validate && aGroupID && aGroupID != "" && scope.model.errors && !scope.model.errors[property]) {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
                if (!scope.validate && aGroupID && aGroupID != "") {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
            };
            scope.validateRuleGroup = function () {
                if (scope.propertyname && scope.validate) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstRuleGroupList, 'ID');
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_GROUP, undefined);
                }
            };
            scope.LoadRuleGroupList();
        }
    };
}]);

app.directive("filterList", ["$rootScope", "$filter", function ($rootScope, $filter) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            isChild: "=",
            changeCallback: '&',
            propertyname: "=",
            alternateProperty: "=",
            cssClass: "@",
            slideLeft: "=",
            skipProp: "=",
            skipValue: "="
        },
        link: function (scope, element, attributes) {
            scope.model = "";
            scope.inlineStyle = attributes.Style;
            var prop = scope.propertyname.split(".");
            var prop2;
            if (scope.alternateProperty) {
                prop2 = scope.alternateProperty.split(".");
            }
            scope.$watch('slideLeft', function () {
                if (!scope.slideLeft) {
                    scope.model = "";
                }
            });
            if (scope.cssClass) {
                scope.textBoxClass = scope.cssClass;
            } else {
                scope.textBoxClass = "col-xs-9";
            }
            scope.onKeyDown = function (event) {
                //if (!scope.inputElement) {
                //    scope.initializeIntellisense();
                //}
                scope.initializeIntellisense();

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    if (scope.model) {
                        scope.onChange();
                    }
                    event.preventDefault();
                }
            };

            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                var list = scope.collection;

                if (scope.collection && scope.collection.length > 0 && scope.skipProp && scope.skipValue != undefined) {  // scope.skipValue can be boolean type or string type 
                    list = scope.collection.filter(function (obj) {
                        if (prop.length > 1) {
                            return (obj[prop[0]][scope.skipProp] != scope.skipValue);
                        } else {
                            return (obj[scope.skipProp] != scope.skipValue);
                        }
                    });
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                newList = [];
                scope.maketList(list, prop, prop2);
                //  setSingleLevelAutoComplete($(scope.inputElement), list, scope, scope.selectedPropertyName, scope.displayPropertyName);
                setSingleLevelAutoComplete($(scope.inputElement), newList, scope);
            };
            scope.onChange = function () {
                if (scope.propertyname) {
                    scope.isMatched = false;
                    //scope.selectedItem = scope.collection.filter(function (obj) {
                    //    if (prop.length > 1) {
                    //        return (obj[prop[0]][prop[1]] == scope.model);
                    //    } else {
                    //        return (obj[prop[0]] == scope.model);
                    //    }
                    //});
                    scope.filterByProperty(scope.collection, prop, prop2);
                    //   scope.selectedItem = scope.selectedItem && scope.selectedItem[0];
                    if (scope.selectedItem && scope.model && scope.isMatched) {
                        scope.changeCallback({ obj: scope.selectedItem });
                    }
                }
            };

            var newList = [];
            scope.maketList = function (collection, prop, prop2) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var obj = collection[i];
                    if (prop.length > 1) {
                        if (prop2 && prop2.length > 1 && obj[prop2[0]][prop2[1]]) { // if alternate property and  property like "dictAttributes.ID"
                            newList.push(obj[prop2[0]][prop2[1]]);
                        } else if (obj[prop[0]][prop[1]]) { // if property like "dictAttributes.ID"
                            newList.push(obj[prop[0]][prop[1]]);
                        }
                    } else {
                        if (obj[prop[0]]) {  // if property like "ID"
                            newList.push(obj[prop[0]]);
                        }
                    }

                    if (scope.isChild && obj.hasOwnProperty("Children") && obj.Children.length > 0) {
                        scope.maketList(obj.Children, prop, prop2);
                    }
                    if (scope.isChild && obj.hasOwnProperty("Elements") && obj.Elements.length > 0) {
                        scope.maketList(obj.Elements, prop, prop2);
                    }
                }
            };

            scope.filterByProperty = function (collection, prop, prop2, parentObj) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var obj = collection[i];

                    if (prop2 && prop2.length > 0 && obj[prop2[0]][prop2[1]] == scope.model) { // if property like "dictAttributes.ID"
                        if (parentObj) parentObj.IsExpanded = true;
                        scope.selectedItem = obj;
                        scope.isMatched = true;
                        break;
                    }
                    if (prop.length > 1) {
                        if (obj[prop[0]][prop[1]] == scope.model) { // if property like "dictAttributes.ID"
                            if (parentObj) parentObj.IsExpanded = true;
                            scope.selectedItem = obj;
                            scope.isMatched = true;
                            break;
                        }
                    } else {
                        if (obj[prop[0]] == scope.model) { // if property like "ID"
                            if (parentObj) parentObj.IsExpanded = true;
                            scope.selectedItem = obj;
                            scope.isMatched = true;
                            break;
                        }
                    }

                    if (scope.isChild && obj.hasOwnProperty("Children") && obj.Children.length > 0) {
                        scope.filterByProperty(obj.Children, prop, prop2, obj);
                    }
                    if (scope.isChild && obj.hasOwnProperty("Elements") && obj.Elements.length > 0) {
                        scope.filterByProperty(obj.Elements, prop, prop2, obj);
                    }
                }
            };
        },
        template: "<div style='{{inlineStyle}}' class='{{textBoxClass}}' ng-show='slideLeft'><input type='text' style='float:right;width:0;border-radius:5px!important' ng-class='slideLeft? \"form-control input-sm search-text-visible\" : \"full-width\" ' id='ScopeId_{{$id}}' placeholder=' Search '  title='{{model}}' ng-model='model' ng-change='onChange()' ng-keydown='onKeyDown($event)' /></div>"
    };
}]);
app.directive("filterTextList", [function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            model: "=",
            changeCallback: '&',
            cssClass: "@",
            slideLeft: "="
        },
        link: function (scope, element, attributes) {
            if (scope.cssClass) {
                scope.textBoxClass = scope.cssClass;
            } else {
                scope.textBoxClass = "col-xs-9";
            }
            scope.onKeyDown = function (event) {
                scope.initializeIntellisense(event);

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    if (scope.model) {
                        scope.onChange();
                    }
                    event.preventDefault();
                }
                if (event && event.type == "click") {
                    scope.showIntellisenseList();
                }
            };

            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection);
            };
            scope.onChange = function () {

                scope.isMatched = false;

                scope.filterText(scope.collection);
                if (scope.selectedItem && scope.model && scope.isMatched) {
                    scope.changeCallback({ obj: scope.selectedItem });
                }

            };


            scope.filterText = function (collection) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var text = collection[i];
                    if (text == scope.model) {
                        scope.selectedItem = text;
                        scope.isMatched = true;
                        break;
                    }
                }
            };
        },
        template: "<div style='top:-6px' class='{{textBoxClass}}' ng-show='slideLeft'><div class='input-group'><input type='text'  ng-class='slideLeft? \"form-control input-sm search-text-visible\" : \"full-width\" ' id='ScopeId_{{$id}}' placeholder=' Search or (ctrl + space) display suggestion' title='{{model}}' ng-model='model' ng-change='onChange()' ng-keydown='onKeyDown($event)' /><span class='input-group-addon button-intellisense' ng-click='onKeyDown($event)'></span></div></div>"
    };
}]);

app.directive("servermethoddirective", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            servermethod: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            errorprop: "=",
            validate: "=",
            formobject: "=",
            isloadremoteobject: '=',
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                scope.initializeIntellisense();

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                scope.initializeIntellisense();
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };

            scope.getServerMethod = function () {
                scope.collection = [];
                var RemoteObjectName = "srvCommon";
                if (scope.formobject && scope.formobject.dictAttributes.sfwType != "Correspondence" && scope.formobject.dictAttributes.sfwType != "Report" && scope.formobject.dictAttributes.sfwRemoteObject) {
                    RemoteObjectName = scope.formobject.dictAttributes.sfwRemoteObject;
                }

                if (scope.inputElement && !$(scope.inputElement).val()) {
                    scope.formobject.RemoteObjectCollection = [];
                    $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.formobject.RemoteObjectCollection = data;
                                var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                                scope.collection = PopulateServerMethod([], scope.model, objServerObject, scope.isloadremoteobject);
                                if (scope.collection && scope.collection.length > 0) {
                                    setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                                }
                            }
                        });
                    });
                } else {

                    var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                    scope.collection = PopulateServerMethod([], scope.model, objServerObject, scope.isloadremoteobject);
                    if (scope.collection && scope.collection.length > 0) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                    }
                }

            };

            scope.initializeIntellisense = function () {
                scope.getServerMethod();
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                if (scope.collection && scope.collection.length > 0) {
                    setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                }
            };
            scope.onChange = function () {
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.servermethod);
                    }
                    scope.changeCallback();
                }
                scope.validateInput();
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_METHOD, undefined);
                }
            };

        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{servermethod}}" ng-model="servermethod" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)" undoredodirective model="model" propertyname="propertyname"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);


app.directive("internalfunctionsintellisense", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            selecteditem: '=',
            onchangecallback: '&',
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selecteditem}}" ng-model="selecteditem" ng-change="onChange()"  ng-keydown="onInternalFunctionKeyDown($event)" /><span class="input-group-addon button-intellisense" ng-click="showInternalFunctionList($event)"></span></div>',
        link: function (scope, element, attributes) {

            scope.lstInternalFunctions = [];
            $.connection.hubInternalFunctions.server.getRuleFunctions().done(function (data) {
                scope.$evalAsync(function () {
                    scope.lstInternalFunctions = data;
                });

            });
            scope.onInternalFunctionKeyDown = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        event.preventDefault();
                    }
                    else if (scope.lstInternalFunctions && scope.lstInternalFunctions.length > 0) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.lstInternalFunctions, scope, "Method", "Method", scope.onchangecallback);
                        //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.showInternalFunctionList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.lstInternalFunctions, scope, "Method", "Method", scope.onchangecallback);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.onChange = function () {
                if (scope.onchangecallback) {
                    var obj;
                    var lst = scope.lstInternalFunctions.filter(function (itm) { return itm.Method == scope.selecteditem; });
                    if (lst && lst.length > 0) {
                        obj = lst[0];
                    }
                    scope.onchangecallback({ obj: obj });
                }

            };
        },
        //template: 
    };
}]);
app.directive("ngScrolldown", [function () {
    return {
        scope: { callback: '&', ngpausescroll: '=', ngscrollheight: '=?' },
        link: function (scope, elements, attrs) {
            var col = $(elements);
            if (scope.ngscrollheight == undefined)
                scope.ngscrollheight = 0;
            $(elements).scroll(function () {
                if (!scope.ngpausescroll) {
                    if (col.outerHeight() + scope.ngscrollheight >= (col.get(0).scrollHeight - col.scrollTop())) {
                        scope.callback({ data: 1 });
                    }
                }
            });
        }
    };
}]);

app.filter('cut', function () {
    return function (value, wordwise, max, tail) {
        if (!value) return '';

        max = parseInt(max, 10);
        if (!max) return value;
        if (value.length <= max) return value;

        value = value.substr(0, max);
        if (wordwise) {
            var lastspace = value.lastIndexOf(' ');
            if (lastspace != -1) {
                //Also remove . and , so its gives a cleaner result.
                if (value.charAt(lastspace-1) == '.' || value.charAt(lastspace-1) == ',') {
                    lastspace = lastspace - 1;
                }
                value = value.substr(0, lastspace);
            }
        }

        return value + (tail || ' …');
    };
});


app.filter('entitybyTable', function () {

    return function (items, tablename, isSubQueryOnly) {
        var filtered = [];

        if (!tablename || !items.length) {
            return items;
        }

        items.forEach(function (itemElement, itemIndex) {
            itemElement.Queries.forEach(function (QElement, locationIndex) {
                if (QElement.SqlQuery) {
                    if (isSubQueryOnly && QElement.QueryType && QElement.QueryType == "SubSelectQuery") {
                        var testarray = QElement.SqlQuery.toLowerCase().split(/[()\r\n\s,]+/);
                        var testindex = testarray.indexOf(tablename.toLowerCase());
                        if (testindex >= 0) {
                            filtered.push({ ID: itemElement.EntityName + "." + QElement.ID, SqlQuery: QElement.SqlQuery, Parameters: QElement.Parameters ? QElement.Parameters : undefined });
                            return false;
                        }
                    }
                    else if (!isSubQueryOnly) {
                        var testarray = QElement.SqlQuery.toLowerCase().split(/[()\r\n\s,]+/);
                        var testindex = testarray.indexOf(tablename.toLowerCase());
                        if (testindex >= 0) {
                            filtered.push({ ID: itemElement.EntityName + "." + QElement.ID, SqlQuery: QElement.SqlQuery, Parameters: QElement.Parameters ? QElement.Parameters : undefined });
                            return false;
                        }
                    }
                }              
            });
        });

        return filtered;
    };
});

app.filter('filesize', function () {
    var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'];

    return function (bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) {
            return '?';
        }

        var unit = 0;

        while (bytes >= 1024) {
            bytes /= 1024;
            unit++;
        }

        return bytes.toFixed(+precision) + ' ' + units[unit];
    };
});

app.filter('range', function () {
    return function (input, min, max) {
        min = parseInt(min);
        max = parseInt(max);
        for (var i = min; i <= max; i++)
            input.push(i.toString());
        return input;
    };
});

app.filter('controlbyFormtype', function () {
    return function (controlList, visibleOption, ctrlType, formModel) {
        var filteredControlList = controlList,
            sfwType = formModel ? (formModel.hasOwnProperty("dictAttributes") ? formModel.dictAttributes.sfwType : "") : "";
            
        function checkCustomcondition(ctrl, formmodel) {
            var isValid = true;
            // for custom condition
            switch (ctrl.Name) {
                case "Label" : 
                    if (sfwType === 'Lookup' && (formModel.IsLookupCriteriaEnabled == true || formModel.IsPrototypeLookupCriteriaEnabled)) {
                        isValid = false;
                    }
                    break;
                default: break;
            }            
            return isValid;
        }
        if (ctrlType && sfwType && visibleOption && controlList.length) {
            filteredControlList = controlList.filter(function (ctrl) {
                if (ctrl && ctrl.hasOwnProperty("type") && angular.isArray(ctrl.excludeFormTypes) && angular.isArray(ctrl.optionEnabled)) {
                    if (ctrl.type === ctrlType && (ctrl.excludeFormTypes.indexOf(sfwType) < 0) && (ctrl.optionEnabled.indexOf(visibleOption) >= 0)) {
                        // custom condition go here
                        if (checkCustomcondition(ctrl, formModel)) {
                            return ctrl;
                        }
                    }
                }
            });
        }
        return filteredControlList;
    };
});
app.service('$getMultiIndexofArray', function () {
    // for returning mulitple indexes of the same element in an array
    return function (el, paramName) {
        var idxs = [];
        for (var i = el.length - 1; i >= 0; i--) {
            if (el[i] === paramName) {
                idxs.unshift(i);
            }
        }
        return idxs;
    };
});

// for adding and removing css class from an element or array of element 
// jquery addClass function dont work on some kind of elements like svg
app.service('$cssFunc', ['$getMultiIndexofArray', function ($getMultiIndexofArray) {
    this.addEachcssClass = function (ele, className) {
        function addclassIterator(item) {
            var oldClass = item.getAttribute("class");
            if (oldClass) {
                var allclass = oldClass.split(" ");
                if (allclass.indexOf(className) == -1) {
                    // not already disabled
                    item.setAttribute("class", oldClass + " " + className);
                }
            }
            else item.setAttribute("class", className);
        }
        if (!ele.length) addclassIterator(ele);
        else if (ele.length > 1) { angular.forEach(ele, addclassIterator); }
        else addclassIterator(ele[0]);
    };
    this.removeEachcssClass = function (ele, className) {
        function removeclassIterator(item) {
            var oldClass = item.getAttribute("class");
            if (oldClass) {
                var allclass = oldClass.split(" ");
                var tempindex = $getMultiIndexofArray(allclass, className);
                if (tempindex.length > 0) {
                    // class is present  
                    var arr = $.grep(allclass, function (n, i) {
                        return $.inArray(i, tempindex) == -1;
                    });
                    item.setAttribute("class", arr.join(" "));
                }
            }
        }
        if (!ele.length) removeclassIterator(ele);
        else if (ele.length > 1) { angular.forEach(ele, removeclassIterator); }
        else removeclassIterator(ele[0]);
    };
    // this will remove a class1 if present and add class2 if not present 
    this.TogglecssClass = function (ele, className1, className2) {
        function enableIterator(ele) {
            var oldClass = ele.getAttribute("class");
            if (oldClass) {
                var allclass = oldClass.split(" ");
                var arr = "";
                var tempindex = $getMultiIndexofArray(allclass, className1);
                var tempindex2 = $getMultiIndexofArray(allclass, className2);
                if (tempindex.length > 0) {
                    // disable class is present  
                    arr = $.grep(allclass, function (n, i) {
                        return $.inArray(i, tempindex) == -1;
                    });
                    arr = arr.join(" ");
                }
                if (tempindex2.length == 0) {
                    ele.setAttribute("class", arr + " " + className2);
                }
            }
            else ele.setAttribute("class", " " + className2);
        }
        if (typeof ele.length == "undefined") enableIterator(ele);
        else if (ele.length > 1) { angular.forEach(ele, enableIterator); }
        else if (ele.length == 1) enableIterator(ele[0]);
    };
}]);

app.factory('$resourceFactory', ['hubcontext', function (hubcontext) {
    var item = { resourceConstants: {}, resourceRfunctions: {}, resourceFileType: [], resourcelstTfsStatus: [] };
    // #region create filetype list
    item.resourceFileType.push({ name: "Entity", value: "Entity" });
    item.resourceFileType.push({ name: "Logical Rule", value: "LogicalRule" });
    item.resourceFileType.push({ name: "Decision Table", value: "DecisionTable" });
    item.resourceFileType.push({ name: "Excel Matrix", value: "ExcelMatrix" });
    item.resourceFileType.push({ name: "Entity Based Scenario", value: "ParameterScenario" });
    item.resourceFileType.push({ name: "Object Based Scenario", value: "ObjectScenario" });
    item.resourceFileType.push({ name: "Excel Based Scenario", value: "ExcelScenario" });
    item.resourceFileType.push({ name: "Lookup", value: "Lookup" });
    item.resourceFileType.push({ name: "Maintenance", value: "Maintenance" });
    item.resourceFileType.push({ name: "Wizard", value: "Wizard" });
    item.resourceFileType.push({ name: "User Control", value: "UserControl" });
    item.resourceFileType.push({ name: "Tooltip", value: "Tooltip" });
    item.resourceFileType.push({ name: "HTML Lookup", value: "FormLinkLookup" });
    item.resourceFileType.push({ name: "HTML Maintenance", value: "FormLinkMaintenance" });
    item.resourceFileType.push({ name: "HTML Wizard", value: "FormLinkWizard" });
    item.resourceFileType.push({ name: "Reports", value: "Report" });
    item.resourceFileType.push({ name: "Inbound File", value: "InboundFile" });
    item.resourceFileType.push({ name: "Outbound File", value: "OutboundFile" });
    item.resourceFileType.push({ name: "BPMN", value: "BPMN" });
    item.resourceFileType.push({ name: "BPM Template", value: "BPMTemplate" });
    item.resourceFileType.push({ name: "Correspondence", value: "Correspondence" });
    item.resourceFileType.push({ name: "Prototype", value: "Prototype" });
    item.resourceFileType.push({ name: "Workflow Map", value: "WorkflowMap" });
    //item.resourceFileType.push({ name: "Audit Log", value: "AuditLog" });
    //item.resourceFileType.push({ name: "Project Configuration", value: "ProjectConfiguration" });
    //item.resourceFileType.push({ name: "Custom Settings", value: "CustomSettings" });
    item.resourceFileType.push({ name: "Rule Constants", value: "RuleConstants" });
    // #endregion
    // #region create list of tfs status
    item.resourcelstTfsStatus.push({ name: "only local", value: "OnlyLocal" });
    item.resourcelstTfsStatus.push({ name: "Added", value: "InAdd" });
    item.resourcelstTfsStatus.push({ name: "Edited", value: "InEdit" });
    item.resourcelstTfsStatus.push({ name: "No Change", value: "LocalWithoutChange" });
    // #endregion
    if (hubcontext.hubMain) {
        hubcontext.hubMain.client.setConstantsListData = function (data) {
            if (data) item.resourceConstants = JSON.parse(data);
        };
        hubcontext.hubMain.client.setRfunctionsListData = function (data) {
            if (data) item.resourceRfunctions = JSON.parse(data);
        };
    }
    return {
        getConstantsListData: function () {
            hubcontext.hubMain.server.loadConstants();
        },
        getRfunctionsListData: function () {
            hubcontext.hubMain.server.getRFuncMethods();
        },
        getConstantModelData: function () {
            return item.resourceConstants;
        },
        getConstantsList: function (text) {
            var data = [];
            if (text && item.resourceConstants) {
                var arrText = text.split(".");
                if (arrText.length > 0) {
                    if (arrText[0] == item.resourceConstants.dictAttributes.ID) {
                        var currentElements = item.resourceConstants.Elements;
                        for (var index = 1; index < arrText.length; index++) {
                            var currentModel = currentElements.filter(function (x) {
                                if (x.dictAttributes && x.dictAttributes.ID) return x.dictAttributes.ID == arrText[index];
                            });
                            if (currentModel && currentModel.length > 0) {
                                if (currentModel[0].Name != "constant") {
                                    currentElements = currentModel[0].Elements;
                                }
                                else {
                                    currentElements = [];
                                }
                            }
                        }
                    }
                    if (currentElements && currentElements.length > 0) {
                        for (var index = 0; index < currentElements.length; index++) {
                            if (currentElements[index].dictAttributes && currentElements[index].dictAttributes.ID) {
                                data.push({
                                    ID: currentElements[index].dictAttributes.ID, DisplayName: currentElements[index].dictAttributes.ID, Value: currentElements[index].dictAttributes.ID, Tooltip: currentElements[index].dictAttributes.ID, Type: "Constant"
                                });
                            }
                        }
                    }
                }
                else {
                    data.push({
                        ID: item.resourceConstants.dictAttributes.ID, DisplayName: item.resourceConstants.dictAttributes.ID, Value: item.resourceConstants.dictAttributes.ID, Tooltip: item.resourceConstants.dictAttributes.ID, Type: "Constant"
                    });
                }
            } else {
                data.push({
                    ID: item.resourceConstants.dictAttributes.ID, DisplayName: item.resourceConstants.dictAttributes.ID, Value: item.resourceConstants.dictAttributes.ID, Tooltip: item.resourceConstants.dictAttributes.ID, Type: "Constant"
                });
            }
            return data;
        },
        getRfunctionsList: function () {
            return item.resourceRfunctions;
        },
        setConstantsList: function (data) {
            item.resourceConstants = data;
        },
        setRfunctionsList: function (data) {
            item.resourceRfunctions = data;
        },
        getFileTypeList: function () {
            return item.resourceFileType;
        },
        getTfsStatusList: function () {
            return item.resourcelstTfsStatus;
        },
        clearResourceFactory: function () {
            item.resourceConstants = {};
            item.resourceRfunctions = {};
        }
    };
}]);

app.factory('$EntityIntellisenseFactory', ['hubcontext', '$http', '$rootScope', function (hubcontext, $http, $rootScope, $Chart) {
    var item = { intellisenseData: {} };
    var entNames = { entNameList: [] };
    if (hubcontext.hubMain) {
        hubcontext.hubMain.client.setEntityListData = function (data) {
            if (data) {
                item.intellisenseData = JSON.parse(data);
                setEntityNameList();
            }
        };
    }
    function setEntityNameList() {
        var entList = item.intellisenseData;
        entNames.entNameList = [];
        if (entList && entList.length > 0) {
            entList.forEach(function (ent) {
                entNames.entNameList.push(ent.ID);
            });
        }
        entNames.entNameList.sort();
    }
    return {
        setEntityIntellisense: function (intellisenseData) {
            item.intellisenseData = intellisenseData;
            setEntityNameList();
        },
        getEntityIntellisense: function () {
            return item.intellisenseData;
        },
        getChildEntitiesIncludingThis: function (entityID) {
            /// <summary>Get all the child entities for the given entity id and include this entity itself.</summary>
            /// <param name="entityID" type="string">id of the entity for which all child entities are required.</param>
            /// <returns type="object">collection of entities include all child entities and given entity itself.</returns>

            if (entityID) {
                return item.intellisenseData.filter(function (x) { return x.ParentId == entityID || x.ID == entityID; });
            }
        },
        getChildEntitiesButThis: function (entityID) {
            /// <summary>Get all the child entities for the given entity id.</summary>
            /// <param name="entityID" type="string">id of the entity for which all child entities are required.</param>
            /// <returns type="object">collection of entities include all child entities for the given entity.</returns>

            if (entityID) {
                return item.intellisenseData.filter(function (x) { return x.ParentId == entityID; });
            }
        },
        getDescendentEntitiesIncludingThis: function (entityID) {
            /// <summary>Get all the descendent entities for the given entity id and include this entity itself.</summary>
            /// <param name="entityID" type="string">id of the entity for which all descendent entities are required.</param>
            /// <returns type="object">collection of entities include all descendent entities and given entity itself.</returns>

            if (entityID) {
                var entities = [];
                var col = item.intellisenseData.filter(function (x) { return x.ID == entityID; });
                if (col && col.length > 0) {
                    entities = entities.concat(col);
                }
                col = this.getDescendentEntitiesButThis(entityID);
                if (col && col.length > 0) {
                    entities = entities.concat(col);
                }
                return entities;
            }
        },
        getDescendentEntitiesButThis: function (entityID) {
            /// <summary>Get all the descendent entities for the given entity id.</summary>
            /// <param name="entityID" type="string">id of the entity for which all descendent entities are required.</param>
            /// <returns type="object">collection of entities include all descendent entities for the given entity.</returns>

            if (entityID) {
                var descendentEntities = [];
                var childEntities = this.getChildEntitiesButThis(entityID);
                if (childEntities && childEntities.length > 0) {
                    descendentEntities = descendentEntities.concat(childEntities);
                    for (var idx = 0; idx < childEntities.length; idx++) {
                        var col = this.getDescendentEntitiesButThis(childEntities[0].ID);
                        if (col && col.length > 0) {
                            descendentEntities = descendentEntities.concat(col);
                        }
                    }
                }
                return descendentEntities;
            }
        },
        getEntityNameList: function () {
            return entNames.entNameList;
        },
        clearEntityIntellisenseFactory: function () {
            item.intellisenseData = {};
            entNames.entNameList = {};
        },
        getXmlMethods: function (entityID, includeParentEntity) {
            var xmlMethods = [];
            while (entityID) {
                var entity = item.intellisenseData.filter(function (x) { return x.ID == entityID; });
                if (entity && entity.length) {
                    for (var i = 0, len = entity[0].XmlMethods.length; i < len; i++) {
                        xmlMethods.push(entity[0].XmlMethods[i]);
                    }

                    if (includeParentEntity) {
                        entityID = entity[0].ParentId;
                    }
                    else {
                        entityID = null;
                    }
                }
                else {
                    entityID = null;
                }
            }
            return xmlMethods;
        },
        getXmlMethodParameters: function (entityID, xmlMethodName, checkInParentEntity) {
            var parameters = [];
            var xmlMethods = this.getXmlMethods(entityID, checkInParentEntity);
            if (xmlMethods && xmlMethods.length > 0) {
                xmlMethod = xmlMethods.filter(function (xmethod) { return xmethod.ID === xmlMethodName; });
                if (xmlMethod && xmlMethod.length) {
                    parameters = xmlMethod[0].Parameters;
                }
            }
            return parameters
        },
        getQueryByQueryName: function (queryID) {
            objQuery = null;
            var lstEntityQuery = queryID.split('.');
            if (lstEntityQuery && lstEntityQuery.length == 2) {
                var entityName = lstEntityQuery[0];
                var strQueryID = lstEntityQuery[1];
                var lstEntity = item.intellisenseData.filter(function (x) { return x.ID == entityName; });
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) {
                        return x.ID == strQueryID;
                    });
                    if (lstQuery && lstQuery.length > 0) {
                        objQuery = lstQuery[0];
                    }
                }
            }
            return objQuery;
        },
        getAttributes: function (entityID, attributeTypes, datatypes, includeParentEntity) {
            var attributes = [];
            while (entityID) {
                var entity = item.intellisenseData.filter(function (x) { return x.ID == entityID; });
                if (entity && entity.length) {
                    for (var i = 0, len = entity[0].Attributes.length; i < len; i++) {
                        if (!attributeTypes || !attributeTypes.length || attributeTypes.indexOf(entity[0].Attributes[i].Type.toLowerCase()) > -1) {
                            if (!datatypes || !datatypes.length || datatypes.indexOf(entity[0].Attributes[i].DataType.toLowerCase()) > -1) {
                                attributes.push(entity[0].Attributes[i]);
                            }
                        }
                    }
                }
                if (includeParentEntity) {
                    entityID = entity[0].ParentId;
                }
                else {
                    entityID = null;
                }
            }
            return attributes;
        },
        getRules: function (entityId, nonStatic, returnTypes, ruleTypes, includeParentEntity) {
            var rules = [];
            while (entityId) {
                var entity = item.intellisenseData.filter(function (x) { return x.ID == entityId; });
                if (entity && entity.length) {
                    for (var i = 0, len = entity[0].Rules.length; i < len; i++) {
                        if (!ruleTypes || !ruleTypes.length || attributeTypes.indexOf(entity[0].Rules[i].RuleType.toLowerCase()) > -1) {
                            if (!returnTypes || !returnTypes.length || returnTypes.indexOf(entity[0].Rules[i].ReturnType.toLowerCase()) > -1) {
                                rules.push(entity[0].Rules[i]);
                            }
                        }
                    }
                }
                if (includeParentEntity) {
                    entityId = entity[0].ParentId;
                }
                else {
                    entityId = null;
                }
            }
            return rules;
        },
    };
}]);

app.service('$SearchSource', [function () {
    this.getIndicesOfSearchStr = function (strSearchWord, strSearch, IsCaseSensitive, IsWholeMatchWord, IsSearchRegex) {
        if (strSearchWord) {
            var searchStrLen = strSearchWord.length;
            if (searchStrLen == 0) {
                return [];
            }
            var startIndex = 0, index, indices = [];
            if (!IsCaseSensitive) {
                strSearch = strSearch.toLowerCase();
                strSearchWord = strSearchWord.toLowerCase();
            }
            if (IsSearchRegex) {
                var regexmatch = new RegExp(strSearchWord, "g");
                while ((match = regexmatch.exec(strSearch)) != null) {
                    //console.log(match.index + ' ' + regexmatch.lastIndex);
                    indices.push({ startPos: match.index, endPos: regexmatch.lastIndex });
                }
            }
            else {
                while ((index = strSearch.indexOf(strSearchWord, startIndex)) > -1) {
                    if (IsWholeMatchWord) {
                        var regex = '\\b';
                        regex += escapeRegExp(strSearchWord);
                        regex += '\\b';
                        var substrSearch = strSearch.substring(index - 1, index + searchStrLen + 1);
                        // check for space
                        var isWholeword = new RegExp(regex, "g").test(substrSearch);
                        if (!isWholeword) {
                            startIndex = index + searchStrLen;
                            continue;
                        }
                    }
                    indices.push(index);
                    startIndex = index + searchStrLen;
                }
            }

            return indices;
        }
    };
    this.selectword = function (cntrlSearch, indexOfMatchWord, strSearch, numberLine) {
        numberLine--; // array starts at 0
        var startPos = indexOfMatchWord, endPos = indexOfMatchWord + strSearch.length;
        if (typeof (cntrlSearch[0].selectionStart) != undefined) {
            cntrlSearch[0].selectionStart = startPos;
            cntrlSearch.focus();
            cntrlSearch[0].selectionEnd = endPos;
        }
    };
}]);

app.service('$NavigateToFileService', ['$rootScope', 'hubcontext', function ($rootScope, hubcontext) {
    this.NavigateToFile = function (fileName, nodeName, elementID) {
        hubcontext.hubMain.server.navigateToFile(fileName, nodeName + ";" + elementID).done(function (objfile) {
            if (objfile) {
                var tempfile = $rootScope.lstopenedfiles.filter(function (x) { return x.file.FileName == objfile.FileName; });
                $rootScope.openFile(objfile);
                if (objfile.SelectNodePath && objfile.SelectNodePath != "" && tempfile != "") {
                    var scope = getScopeByFileName(objfile.FileName);
                    if (scope) {
                        scope.selectElement(objfile.SelectNodePath);
                    }
                }
            }
            else {
                $rootScope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
            }
        });
    };

    this.NavigateToRule = function (ruleID) {
        hubcontext.hubMain.server.getFileInfoByID(ruleID).done(function (objfile) {
            if (objfile) {
                $rootScope.openFile(objfile);

            }
        });
    };
}]);

app.factory('FormDetailsFactory', function () {
    var FormDetails;
    return {
        getFormDetails: function () {
            return FormDetails;
        },
        setFormDetails: function (Details) {
            FormDetails = Details;
        },
    };
});

// use for storing validation errors object
app.value("$Errors", {
    validationListObj: []
});

//define constant
app.constant('CONSTANTS', {
    VALIDATION: {
        ID_EMPTY: 'ID can not be empty',
        NOT_VALID_ID: 'Invalid ID',
        DUPLICATE_ID: 'Duplicate ID present',
        DATA_TYPE_EMPTY: 'Data type can not be empty',
        INVALID_ENTITY: 'Invalid Entity type',
        FIELD_NAME_EMPTY: 'Field Name can not be empty',
        VISIBLE_RULE_NOT_EXISTS: 'The visible rule does not exist. Please change the rule.',
        ENABLE_RULE_NOT_EXISTS: 'The enable rule does not exist. Please change the rule.',
        READONLY_RULE_NOT_EXISTS: 'The item read only rule does not exist. Please change the rule.',
        CONDITIONAL_RULE_NOT_EXISTS: 'The conditional rule does not exist. Please change the rule.',
        RESOURCE_NOT_EXISTS: 'The resource does not exist. Please change the resource id.',
        ENTITY_FIELD_INCORRECT: 'The entity field is incorrect. Please change the entity field.',
        VALIDATION_RULE_NOT_EXISTS: 'The validation rule does not exist. Please change the validation rule.',
        CODE_GROUP_NOT_EXISTS: 'The code group does not exist. Please change the code id.',
        INVALID_QUERY: 'Invalid query.',
        INVALID_METHOD: 'The method does not exist. Please change the method.',
        INVALID_ACTIVE_FORM: 'The form does not exist. Please change the form name.',
        INVALID_QUERY_ID: 'The query ID does not exist. Please change the query id.',
        INVALID_DATA_FIELD: 'Invalid data field',
        INVALID_EXPRESSION: 'The expression does not exist. Please change the expression.',
        INVALID_MESSAGE_ID: 'The message id does not exist. Please change the message id.',
        MESSAGE_ID_EMPTY: 'Message ID can not be empty.',
        INVALID_ENTITY_NAME: 'The entity does not exist. Please change the entity.',
        INVALID_CODE_TABLE: 'The code table name does not exist. Please change the code table name.',
        BUTTON_NOT_EXISTS: 'The button not exist.Please change button name.',
        INVALID_GROUP: 'The group does not exist.Please change group name.',
        INVALID_FIELD: 'The field is incorrect. Please change the field name.',
        INVALID_TEMPLATE: 'The template name does not exist.Please change the template name.',
        CHECKLIST_NOT_EXISTS: 'The checklist does not exist. Please change the checklist id.',
        EMPTY_FIELD: 'The field can not be empty.',
        INVALID_RULE: 'Rule not present add or change rule',
        ONLY_OBJECT_OR_COLLECTION: 'Only Object or Collection type field not allowed',
        INVALID_NAME: 'Invalid method name',
        DUPLICATE_ID_IN_PARENT_ENTITY: 'Duplicate id present in parent entity.',
        DUPLICATE_FIELD: 'Duplicate field present',
        DUPLICATE_EFFECTIVE_DATE: "Effective date already used.",
        EMPTY_DATATYPE: "Datatype cannot be empty.",
        INVALID_DATATYPE: "Invalid value for the selected datatype",
        EMPTY_VALUE: "Value cannot be empty for selected datatype",
        QUERY_WITH_PARAMETER: "Parameterized query not allowed",
        OBJECT_FIELD_EMPTY: "Object Field can not be empty",
        EMPTY_ERROR_TABLE: "Error table can not be empty.",
        INVALID_ERROR_TABLE: "Invalid error table.",
        EMPTY_CHECKLIST_TABLE: "Checklist table can not be empty.",
        INVALID_CHECKLIST_TABLE: "Invalid checklist table.",
        EMPTY_MESSAGE_AND_ID: "Empty MessageID and Description.Enter value for atleast one field.",
        EMPTY_MESSAGE_DESC: "Message Description can not be empty.",
        EMPTY_GENERIC: "{0} cannot be empty.",
        INVALID_GENERIC: "Invalid {0}. Please select a valid {0}."
    },
    ENTITY: {
        NODES: ["queries", "query", "rules", "rule", "item", "grouplist", "group", "attributes", "attribute"],
        METHOD_NODES: ["objectmethods", "methods", "method"]
    },
    FORM: {
        NODES: ["sfwTable", "sfwRow", "sfwColumn", "sfwPanel", "DialogPanel", "sfwDialogPanel", "sfwListView", "TabContainer", "Tabs", "sfwTabSheet", "sfwTabContainer", "sfwWizard", "WizardSteps", "sfwWizardStep"],
        IGNORE_NODES: ["sfwRow", "sfwColumn", "TemplateField", "ItemTemplate", "DataColumn", "DataRow", "Data", "Columns", "Tabs", "Parameters", "Columns", "HeaderTemplate", "WizardSteps", "SideBarTemplate"],
        COLLECTION_TYPE_NODES: ["sfwGridView", "sfwListView", "sfwChart", "sfwCheckBoxList"],
        CONTROL_TYPES: [
            {
                "Name": "Caption",
                "method": "Caption",
                "type": "basic",
                "icon": "images/Form/icon-label.svg",
                "excludeFormTypes": [],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Compare Validator",
                "method": "CompareValidator",
                "type": "advanced",
                "icon": "images/Form/icon-compare-validator.svg",
                "excludeFormTypes": ["Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Dialog Panel",
                "method": "DialogPanel",
                "type": "advanced",
                "icon": "images/Form/icon-dialog-panel.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Hyper Link",
                "method": "sfwHyperLink",
                "type": "basic",
                "icon": "images/Form/icon-hyperlink.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "List Box",
                "method": "sfwListBox",
                "type": "advanced",
                "icon": "images/Form/icon-ListBox.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol"],
                "attributetype": "value"
            },
            {
                "Name": "List Picker",
                "method": "sfwListPicker",
                "type": "advanced",
                "icon": "images/Form/icon-mant4.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Open Detail",
                "method": "sfwOpenDetail",
                "type": "advanced",
                "icon": "images/Form/icon-open-detail.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Lookup", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "CR Viewer",
                "method": "sfwCRViewer",
                "type": "advanced",
                "icon": "images/Form/CR_Viewer.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Employer Soft Errors",
                "method": "sfwEmployerSoftErrors",
                "type": "advanced",
                "icon": "images/Form/Employee-soft_error.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "File Layout",
                "method": "sfwFileLayout",
                "type": "advanced",
                "icon": "images/Form/File_Layout.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "File Upload",
                "method": "sfwFileUpload",
                "type": "advanced",
                "icon": "images/Form/File_Upload.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Soft Errors",
                "method": "sfwSoftErrors",
                "type": "advanced",
                "icon": "images/Form/Soft_Errors.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Questionnaire Panel",
                "method": "sfwQuestionnairePanel",
                "type": "advanced",
                "icon": "images/Form/icon-QuestionnairePanel.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Button",
                "method": "sfwButton",
                "type": "basic",
                "icon": "images/Form/icon-button.svg",
                "excludeFormTypes": ["Tooltip", "Report", "Correspondence",],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "addcontrol", "addcontrol"]
            },
            {
                "Name": "Link Button",
                "method": "sfwLinkButton",
                "type": "basic",
                "excludeFormTypes": ["Report", "Correspondence"],
                "icon": "images/Form/icon-button.svg",
                "allowInParent": ["all"],
                "optionEnabled": []
            },
            {
                "Name": "Image Button",
                "method": "sfwImageButton",
                "type": "basic",
                "excludeFormTypes": ["Report", "Correspondence"],
                "icon": "images/Form/icon-button.svg",
                "allowInParent": ["all"],
                "optionEnabled": []
            },
            {
                "Name": "Custom Button",
                "method": "basic",
                "type": "",
                "excludeFormTypes": ["Report", "Correspondence"],
                "icon": "images/Form/icon-button.svg",
                "allowInParent": ["all"],
                "optionEnabled": []
            },
            {
                "Name": "Panel",
                "method": "Panel",
                "type": "basic",
                "icon": "images/Form/icon-mant2.svg",
                "excludeFormTypes": ["Report", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Range Validator",
                "method": "RangeValidator",
                "type": "advanced",
                "icon": "images/Form/icon-RangeValidator.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Regular Expression Validator",
                "method": "RegularExpressionValidator",
                "type": "advanced",
                "icon": "images/Form/regular_expression_validator.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Required Field Validator",
                "method": "RequiredFieldValidator",
                "type": "advanced",
                "icon": "images/Form/icon-required-field-validator.svg",
                "excludeFormTypes": ["Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Cascading DropDown",
                "method": "sfwCascadingDropDownList",
                "type": "basic",
                "icon": "images/Form/icon-cascading-dropdown.svg",
                "excludeFormTypes": ["Correspondence", "Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "CheckBox",
                "method": "sfwCheckBox",
                "type": "basic",
                "icon": "images/Form/icon-checkbox.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "CheckBox List",
                "method": "sfwCheckBoxList",
                "type": "advanced",
                "icon": "images/Form/icon-checkboxlist.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "collection"
            },
            {
                "Name": "DropDown",
                "method": "sfwDropDownList",
                "type": "basic",
                "icon": "images/Form/icon-form-dropdown.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "MultiSelect DropDown",
                "method": "sfwMultiSelectDropDownList",
                "type": "basic",
                "icon": "images/Form/icon-dropdown.svg",
                "excludeFormTypes": ["Tooltip", "Report"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Image",
                "method": "sfwImage",
                "type": "advanced",
                "icon": "images/Form/icon-image.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Label",
                "method": "sfwLabel",
                "type": "basic",
                "icon": "images/Form/icon-label.svg",
                "excludeFormTypes": ["Correspondence"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Literal",
                "method": "sfwLiteral",
                "type": "basic",
                "icon": "images/Form/icon-literal.svg",
                "excludeFormTypes": ["Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Multi Correspondence",
                "method": "sfwMultiCorrespondence",
                "type": "advanced",
                "icon": "images/Form/icon-repeater-control.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Radio Button",
                "method": "sfwRadioButton",
                "type": "basic",
                "icon": "images/Form/icon-radio.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Radio Button List",
                "method": "sfwRadioButtonList",
                "type": "advanced",
                "icon": "images/Form/icon-radiobuttonlist.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["changecontrol", "toolbox", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Rule Viewer",
                "method": "sfwRuleViewer",
                "type": "advanced",
                "icon": "images/Form/icon-Rule-Viewer.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Slider",
                "method": "sfwSlider",
                "type": "advanced",
                "icon": "images/Form/icon-slider.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Knob",
                "method": "sfwKnob",
                "type": "advanced",
                "icon": "images/Form/icon-knob.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Switch CheckBox",
                "method": "sfwSwitchCheckBox",
                "type": "advanced",
                "icon": "images/Form/icon-switch-CheckBox.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "TextBox",
                "method": "sfwTextBox",
                "type": "basic",
                "icon": "images/Form/icon-textbox.svg",
                "excludeFormTypes": ["Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "DateTime Picker",
                "method": "sfwDateTimePicker",
                "type": "advanced",
                "icon": "images/Form/icon-DateTimepicker.svg",
                "excludeFormTypes": ["Tooltip", "Correspondence", "Report"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Source List",
                "method": "sfwSourceList",
                "type": "advanced",
                "icon": "images/Form/icon-mant5.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Target List",
                "method": "sfwTargetList",
                "type": "advanced",
                "icon": "images/Form/icon-mant6.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Tooltip", "Lookup"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "value"
            },
            {
                "Name": "Retrieve Button",
                "method": "sfwImageButton",
                "type": "advanced",
                "icon": "images/Form/icon-retrieve-button.svg",
                "excludeFormTypes": ["Maintenance", "Lookup", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Tooltip Button",
                "method": "sfwToolTipButton",
                "type": "basic",
                "icon": "images/Form/icon-tooltip.svg",
                "excludeFormTypes": ["Correspondence", "Report", "Tooltip"],
                "allowInParent": ["sfwGrid"],
                "optionEnabled": ["changecontrol", "addcontrol"],
            },
            {
                "Name": "JSON Data",
                "method": "sfwJSONData",
                "type": "advanced",
                "icon": "images/Form/JSON_Data.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Lookup", "Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "User Defaults",
                "method": "sfwUserDefaults",
                "type": "advanced",
                "icon": "images/Form/icon-repeater-control.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Maintenance", "Wizard", "UserControl", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "User Control",
                "method": "UserControl",
                "type": "advanced",
                "icon": "images/Form/new-user-main.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Lookup", "UserControl", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"],
                "attributetype": "object"
            },
            {
                "Name": "Command Button",
                "method": "sfwCommandButton",
                "type": "advanced",
                "icon": "images/Form/CommandButton.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Maintenance", "Lookup", "UserControl", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Tab Container",
                "method": "TabContainer",
                "type": "advanced",
                "icon": "images/Form/icon-tabContainer.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Lookup", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Wizard Progress",
                "method": "sfwWizardProgress",
                "type": "advanced",
                "icon": "images/Form/WizardProgress.svg",
                "excludeFormTypes": ["Report", "Correspondence", "Maintenance", "Lookup", "UserControl", "Tooltip"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Break",
                "method": "br",
                "type": "html",
                "icon": "images/Form/break.svg",
                "excludeFormTypes": ["Report", "Correspondence"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Horizontal Rule",
                "method": "hr",
                "type": "html",
                "icon": "images/Form/horizontal_rule.svg",
                "excludeFormTypes": ["Report", "Correspondence"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Grid View",
                "method": "sfwGridView",
                "type": "advanced",
                "icon": "images/Form/icon-gridview.png",
                "excludeFormTypes": ["Lookup", "Report", "Correspondence"],
                "allowInParent": ["all"],
                "optionEnabled": ["toolbox"],
                "attributetype": "collection"
            },
            {
                "Name": "Button Group",
                "method": "sfwButtonGroup",
                "type": "advanced",
                "icon": "images/Form/group.svg",
                "excludeFormTypes": [],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            },
            {
                "Name": "Captcha",
                "method": "sfwCaptcha",
                "type": "advanced",
                "icon": "images/Form/captcha_control_normal.svg",
                "excludeFormTypes": ["Lookup", "Report", "Correspondence", "Tooltip"],
                "allowInParent": ["all", "sfwGrid"],
                "optionEnabled": ["toolbox", "changecontrol", "addcontrol"]
            }
        ],
        LIST_OF_PAGETYPE: [{ value: '', text: '' }, { value: 'letter', text: 'Letter' }, { value: 'a0', text: 'A0' }, { value: 'a1', text: 'A1' }, { value: 'a2', text: 'A2' }, { value: 'a3', text: 'A3' }, { value: 'a4', text: 'A4' }, { value: 'a5', text: 'A5' }, { value: 'a6', text: 'A6' }, { value: 'a7', text: 'A7' }, { value: 'a8', text: 'A8' }, { value: 'a9', text: 'A9' }, { value: 'a10', text: 'A10' },
        { value: 'b0', text: 'B0' }, { value: 'b1', text: 'B1' }, { value: 'b2', text: 'B2' }, { value: 'b3', text: 'B3' }, { value: 'b4', text: 'B4' }, { value: 'b5', text: 'B5' }, { value: 'b6', text: 'B6' }, { value: 'b7', text: 'B7' }, { value: 'b8', text: 'B8' }, { value: 'b9', text: 'B9' }, { value: 'b10', text: 'B10' },
        { value: 'c0', text: 'C0' }, { value: 'c1', text: 'C1' }, { value: 'c2', text: 'C2' }, { value: 'c3', text: 'C3' }, { value: 'c4', text: 'C4' }, { value: 'c5', text: 'C5' }, { value: 'c6', text: 'C6' }, { value: 'c7', text: 'C7' }, { value: 'c8', text: 'C8' }, { value: 'c9', text: 'C9' }, { value: 'c10', text: 'C10' },
        { value: 'd0', text: 'D0' }, { value: 'government-letter', text: 'Government Letter' }, { value: 'legal', text: 'Legal' }, { value: 'junior-legal', text: 'Junior Legal' }, { value: 'ledger', text: 'Ledger' }, { value: 'tabloid', text: 'Tabloid' }, { value: 'credit-card', text: 'Credit Card' }],
    },
    CORRESPONDENCEDATATYPES: [
        { CodeID: "", CodeValue: "" },
        { CodeID: "Int", CodeValue: "Int" },
        { CodeID: "String", CodeValue: "String" },
        { CodeID: "Decimal", CodeValue: "Decimal" },
        { CodeID: "Boolean", CodeValue: "Boolean" },
        { CodeID: "DateTime", CodeValue: "DateTime" },
        { CodeID: "sfwHtmlText", CodeValue: "Html Text" },
        { CodeID: "sfwImageFile", CodeValue: "Image File" },
        { CodeID: "sfwImageByte", CodeValue: "Image Byte" }
    ],
    VALIDATIONERROR: [
        {
            ERROR_KEY: "empty_id", ERROR_VALUE: "ID_EMPTY"
        },
        {
            ERROR_KEY: "inValid_id", ERROR_VALUE: "NOT_VALID_ID"
        },
        {
            ERROR_KEY: "duplicate_id", ERROR_VALUE: "DUPLICATE_ID"
        },
        {
            ERROR_KEY: "data_type", ERROR_VALUE: "DATA_TYPE_EMPTY"
        },
        {
            ERROR_KEY: "invalid_entity", ERROR_VALUE: "INVALID_ENTITY"
        },
        {
            ERROR_KEY: "empty_ObjField", ERROR_VALUE: "objField_EMPTY"
        },
        {
            ERROR_KEY: "emptyMessageIdAndDescription", ERROR_VALUE: "EMPTY_MESSAGE_AND_ID"
        },
        {
            ERROR_KEY: "emptyMessageDescription", ERROR_VALUE: "EMPTY_MESSAGE_DESC"
        },
        {
            ERROR_KEY: "sfwFieldName", ERROR_VALUE: "EMPTY_FIELD"
        }
    ],
});

//use this service for validate base model type object
app.factory("$ValidationService", ["CONSTANTS", "$timeout", "$rootScope", "$EntityIntellisenseFactory", "$Entityintellisenseservice", function (CONST, $timeout, $rootScope, $EntityIntellisenseFactory, $Entityintellisenseservice) {
    var factObj = {};
    var tempList = [];
    factObj.IsValidDate = function (d) {
        var isValid = true;
        if (Object.prototype.toString.call(d) === "[object Date]") {
            // it is a date
            if (isNaN(d.getTime())) {  // d.valueOf() could also work
                // date is not valid
                isValid = false;
            }
        }
        else {
            isValid = false;
            // not a date
        }
        return isValid;
    }

    var checkObjPresentInList = function (list, obj, isCaseSensitive) {
        if (obj && list && list.length > 0) {
            var index = -1;
            if (isCaseSensitive && typeof obj === "string") {
                var newTempList = list.map(function (value) {
                    return value.toLowerCase();
                });
                index = newTempList.indexOf(obj.toLowerCase());
            } else {
                index = list.indexOf(obj);
            }

            if (index > -1) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    };

    factObj.isEmptyObj = function (obj) {
        var retVal = false;
        for (var key in obj) {
            var value = obj[key];
            if (value != "" || value != undefined) {
                retVal = true;
            }
        }
        return retVal;
    };

    var getValidationListObj = function (isDialog) {
        if (isDialog) {
            return tempList;
        } else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.validationErrorList) {
                return scope.validationErrorList;
            } else {
                return undefined;
            }
        }
    };
    var getWarningList = function (modelProp) {
        var outputObj = {};
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.warningList) {
                outputObj["lstwarning"] = scope.warningList;
            }
            if (scope && scope[modelProp]) {
                outputObj["lstmodel"] = scope[modelProp];
            }
        }
        return outputObj;
    };

    factObj.getEntityAttributes = function (entity, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression) {
        var data = [];
        var entityId = entity;
        var entities = $EntityIntellisenseFactory.getEntityIntellisense();
        while (entityId) {
            data = data.concat($rootScope.getEntityAttributeIntellisense(entityId, true, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression));
            if (entities) {
                var entity = entities.filter(function (x) {
                    return x.ID == entityId;
                });
                if (entity.length > 0) {
                    entityId = entity[0].ParentId;
                } else {
                    entityId = "";
                }
            }
        }
        return data;
    };

    factObj.removeObjInToArray = function (arr, item) {
        if (arr && arr.length > 0 && item) {
            var index = arr.indexOf(item);
            if (index > -1) {
                arr.splice(index, 1);
            }
        }
    };

    factObj.validateID = function (obj, validationErrorList, ID, isEmpty, includeHyphen) {
        var inValid = false;
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }

        if (!ID && (isEmpty == undefined)) {
            delete obj.errors.inValid_id;
            if (!obj.errors.empty_id) obj.errors.empty_id = CONST.VALIDATION.ID_EMPTY;
            inValid = true;
        } else if (!isValidIdentifier(ID, false, false, includeHyphen)) {
            if (!obj.errors.inValid_id) obj.errors.inValid_id = CONST.VALIDATION.NOT_VALID_ID;
            inValid = true;
        }
        if (ID == "" && isEmpty == true) {
            obj.errors = {};
        }
        if (ID) {
            delete obj.errors.empty_id;
            //delete obj.errors.duplicate_id;
        }
        if (isValidIdentifier(ID, false, false, includeHyphen)) {
            delete obj.errors.inValid_id;
        }
        if (inValid) {
            if (validationErrorList && !checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }
        if (obj.errors && !factObj.isEmptyObj(obj.errors) && validationErrorList) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    };

    factObj.validateEmptyObjectField = function (obj, validationErrorList, ID) {
        var inValid = false;
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }
        if (!ID && !obj.dictAttributes.sfwValue) {
            delete obj.errors.inValid_id;
            if (!obj.errors.empty_ObjField) obj.errors.empty_ObjField = CONST.VALIDATION.OBJECT_FIELD_EMPTY;
            inValid = true;
        }
        if (ID) {
            delete obj.errors.empty_ObjField;
        }
        if (inValid) {
            if (validationErrorList && !checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }
        if (obj.errors && !factObj.isEmptyObj(obj.errors) && validationErrorList) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    };
    factObj.validateIsEmptyMessageId = function (obj, validationErrorList, ID) {
        var inValid = false;
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }
        if (!ID && (!ID == undefined)) {
            delete obj.errors.inValid_id;
            if (!obj.errors.sfwMessageId) obj.errors.sfwMessageId = CONST.VALIDATION.EMPTY_FIELD;
            inValid = true;
        }
        if (ID) {
            delete obj.errors.sfwMessageId;
        }
        if (inValid) {
            if (validationErrorList && !checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }
        if (obj.errors && !factObj.isEmptyObj(obj.errors) && validationErrorList) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    }
    factObj.validateIsEmptyMessageIdAndDescription = function (obj, validationErrorList, ID, Description) {
        var inValid = false;
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }

        if (!ID && !Description) {
            var list = undefined;
            factObj.checkValidListValue(list, obj, obj.dictAttributes.sfwMessageId, "sfwMessageId", "sfwMessageId", CONST.VALIDATION.MESSAGE_ID_EMPTY, validationErrorList, true);
            delete obj.errors.EMPTY_MESSAGE_AND_ID;
            if (!obj.errors.EMPTY_MESSAGE_AND_ID) obj.errors.EMPTY_MESSAGE_AND_ID = CONST.VALIDATION.EMPTY_MESSAGE_AND_ID;
            delete obj.errors.EMPTY_MESSAGE_DESC;
            if (!obj.errors.EMPTY_MESSAGE_DESC) obj.errors.EMPTY_MESSAGE_DESC = CONST.VALIDATION.EMPTY_MESSAGE_DESC;
            inValid = true;
        }
        if (ID) {
            delete obj.errors.EMPTY_MESSAGE_DESC;
            delete obj.errors.EMPTY_MESSAGE_AND_ID;
        }
        if (Description) {
            delete obj.errors.EMPTY_MESSAGE_DESC;
            delete obj.errors.EMPTY_MESSAGE_AND_ID;
            if (obj.errors.sfwMessageId) {
                delete obj.errors.sfwMessageId;
            }
            if (obj.errors.EMPTY_FIELD) {
                delete obj.errors.EMPTY_FIELD;
            }

        }
        if (obj.errors.sfwMessageId && obj.errors.EMPTY_FIELD) {
            delete obj.errors.sfwMessageId;
        }
        if (inValid) {
            if (validationErrorList && !checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }
        if (obj.errors && !factObj.isEmptyObj(obj.errors) && validationErrorList) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    }

    factObj.validateDataType = function (obj, validationErrorList, dataType) {
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }
        if (!dataType || dataType == "") {
            if (!obj.errors.data_type) obj.errors.data_type = CONST.VALIDATION.DATA_TYPE_EMPTY;
            if (!checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }

        if (dataType) {
            delete obj.errors.data_type;
        }
        if (obj.errors && !factObj.isEmptyObj(obj.errors)) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    };

    factObj.validateEntity = function (obj, validationErrorList_Param) {
        var input;
        var entityNameList = [];
        var validationErrorList;

        entityNameList = $EntityIntellisenseFactory.getEntityNameList();

        if (validationErrorList_Param) {
            validationErrorList = validationErrorList_Param;
        } else {
            validationErrorList = getValidationListObj();
        }
        if (!validationErrorList) return;
        if (!obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }

        input = obj && obj.dictAttributes ? obj.dictAttributes.sfwEntity : undefined;
        if (typeof input !== "undefined") {
            if (!obj.errors.invalid_entity) obj.errors.invalid_entity = CONST.VALIDATION.INVALID_ENTITY;
            if (!checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
        }

        if (obj && entityNameList && validationErrorList && entityNameList.length > 0) {
            if (obj.dictAttributes && (!obj.dictAttributes.sfwEntity || !checkObjPresentInList(entityNameList, obj.dictAttributes.sfwEntity))) {
                if (!obj.errors.invalid_entity) obj.errors.invalid_entity = CONST.VALIDATION.INVALID_ENTITY;
                if (!checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
            }

            if (checkObjPresentInList(entityNameList, obj.dictAttributes.sfwEntity)) {
                delete obj.errors.invalid_entity;
            }
            if (obj.errors && !factObj.isEmptyObj(obj.errors)) {
                factObj.removeObjInToArray(validationErrorList, obj);
            }
        }

    };
    factObj.checkDuplicateId = function (Obj, mainModel, validationErrorList, isNotFirstLoad, checkList, warningslist, ablnAddInWarning, ablnCaseSensitive) {
        if (isNotFirstLoad) {
            if (Obj.errors && Obj.errors.duplicate_id) {
                delete Obj.errors.duplicate_id;
            }
            if (Obj.errors && !factObj.isEmptyObj(Obj.errors)) {
                factObj.removeObjInToArray(validationErrorList, Obj);
            }
            if (Obj.warnings && Obj.warnings.duplicate_id) {
                delete Obj.warnings.duplicate_id;
            }
            if (Obj.errors && !factObj.isEmptyObj(Obj.warnings)) {
                factObj.removeObjInToArray(validationErrorList, Obj);
            }
        }
        //Obj.otherDuplicateObj = [];
        iterateAll(Obj, mainModel, validationErrorList, checkList, warningslist, ablnAddInWarning, ablnCaseSensitive);
    };

    // Method for traverse all nodes in  main model
    var iterateAll = function (Obj, mainModel, validationErrorList, checkList, warningslist, ablnAddInWarning, ablnCaseSensitive) {
        if (mainModel.Elements.length > 0) {
            angular.forEach(mainModel.Elements, function (currentObj) {
                if (currentObj.Name !== "item") {

                    var strCurrentID = currentObj.dictAttributes.ID ? currentObj.dictAttributes.ID : "";
                    var strID = Obj.dictAttributes.ID ? Obj.dictAttributes.ID : "";
                    strCurrentID = ablnCaseSensitive ? strCurrentID : strCurrentID.toLowerCase();
                    strID = ablnCaseSensitive ? strID : strID.toLowerCase();

                    if (strCurrentID && strID && strCurrentID === strID && currentObj !== Obj) {
                        if (!Obj.errors && !angular.isObject(Obj.errors)) {
                            Obj.errors = {};
                        }
                        if (!Obj.warnings && !angular.isObject(Obj.warnings)) {
                            Obj.warnings = {};
                        }
                        var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                        if (currentObj.hasOwnProperty("isParent") && currentObj.isParent) {
                            errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                        }
                        if (currentObj.hasOwnProperty("isParent") && currentObj.isParent && ablnAddInWarning) {
                            //Obj.errors = {};
                            if (Obj.warnings && !Obj.warnings.duplicate_id) Obj.warnings.duplicate_id = errorMessage;
                        }
                        else {
                            //Obj.warnings = {};
                            if (Obj.errors && !Obj.errors.duplicate_id) Obj.errors.duplicate_id = errorMessage;
                        }
                        if (!checkObjPresentInList(validationErrorList, Obj))
                            if (currentObj.hasOwnProperty("isParent") && currentObj.isParent && ablnAddInWarning) {
                                warningslist.push(Obj);
                            }
                            else {
                                validationErrorList.push(Obj);
                            }

                        //if (!angular.isArray(Obj.otherDuplicateObj)) Obj.otherDuplicateObj = [];
                        //if (!angular.isArray(currentObj.otherDuplicateObj)) currentObj.otherDuplicateObj = [];

                        //if (!checkObjPresentInList(Obj.otherDuplicateObj, currentObj)) Obj.otherDuplicateObj.push(currentObj);
                        //if (!checkObjPresentInList(currentObj.otherDuplicateObj, Obj)) currentObj.otherDuplicateObj.push(Obj);
                    }
                }

                if (currentObj.Elements.length > 0 && (checkList.indexOf(currentObj.Name) > -1)) {
                    // console.log(currentObj.Name);
                    iterateAll(Obj, currentObj, validationErrorList, checkList);
                }
            });
        }
    };
    // compaire two dates and find duplicate value
    factObj.findDuplicateDates = function (mainModel, paramObj, validationErrorList, prop) {
        angular.forEach(mainModel.Elements, function (obj) {
            if (obj && paramObj && obj.dictAttributes[prop] && paramObj.dictAttributes[prop] && obj != paramObj) {
                var dateValueFirst = new Date(obj.dictAttributes[prop]);
                var dateValueSecond = new Date(paramObj.dictAttributes[prop]);
                if (factObj.IsValidDate(dateValueFirst) && factObj.IsValidDate(dateValueSecond) && dateValueFirst.toDateString() == dateValueSecond.toDateString()) {
                    if (!paramObj.errors && !angular.isObject(paramObj.errors)) {
                        paramObj.errors = {};
                    }
                    if (paramObj.errors && !paramObj.errors[prop]) paramObj.errors[prop] = CONST.VALIDATION.DUPLICATE_EFFECTIVE_DATE;
                    if (!checkObjPresentInList(validationErrorList, paramObj)) validationErrorList.push(paramObj);
                }
            }
            if (obj && obj.Elements.length > 0) {
                factObj.findDuplicateDates(obj, paramObj, validationErrorList, prop);
            }
        });
    };
    factObj.findDuplicateId = function (model, inputID, nodeList, nodeName, obj, propertyName) {
        var result = null;
        if (model.Elements.length > 0) {
            for (var i = 0; i < model.Elements.length; i++) {
                currentObj = model.Elements[i];
                if (currentObj.Name != nodeName && currentObj != obj) {
                    var strPropertyValue = eval("currentObj." + propertyName);
                    if (strPropertyValue && inputID && strPropertyValue.toLowerCase() == inputID.toLowerCase()) {
                        result = currentObj;
                        break;
                    }
                    else if (currentObj.Elements.length > 0 && (nodeList.indexOf(currentObj.Name) > -1)) {
                        result = factObj.findDuplicateId(currentObj, inputID, nodeList, nodeName, obj, propertyName);
                        if (result) break;
                    }
                }
            }
        }
        return result;
    };

    factObj.checkValidListValue = function (List, obj, value, property, errMessageProp, errMsg, validationErrorList_Param, isEmptyCheck, isDialog, allowWhiteSpace, isCaseSensitive) {
        var validationErrorList;
        var input;
        if (typeof value !== "undefined") {
            input = value;
        }
        else {
            input = obj && obj.dictAttributes ? obj.dictAttributes[property] : undefined;
        }
        if (validationErrorList_Param) {
            validationErrorList = validationErrorList_Param;
        } else {
            validationErrorList = getValidationListObj(isDialog);
        }
        if (!validationErrorList) return;
        if (obj && !obj.errors && !angular.isObject(obj.errors)) {
            obj.errors = {};
        }
        if (isEmptyCheck && !input) {
            if (obj) {
                obj.errors[errMessageProp] = CONST.VALIDATION.EMPTY_FIELD;
            }
            if (!checkObjPresentInList(validationErrorList, obj, isCaseSensitive)) validationErrorList.push(obj);
            return;
        } else if (obj && obj.errors.hasOwnProperty(errMessageProp)) {
            obj.errors[errMessageProp] = "";
        }
        if (!input || !List) {
            if (obj && obj.errors.hasOwnProperty(errMessageProp)) {
                delete obj.errors[errMessageProp];
                // factObj.removeObjInToArray(validationErrorList, obj);
            }
        }
        else if (List && List.length <= 0) { // if List is empty and user enters input text
            if (obj && !obj.errors[errMessageProp]) obj.errors[errMessageProp] = errMsg;
            if (!checkObjPresentInList(validationErrorList, obj, isCaseSensitive)) validationErrorList.push(obj);
        }
        else if (obj && List && validationErrorList && List.length > 0) {
            List = $.map(List, $.trim); // trim each value present in the array
            if (input && !checkObjPresentInList(List, allowWhiteSpace ? input : input.trim(), isCaseSensitive)) { // if input value then check value is valid or not
                if (!obj.errors[errMessageProp]) obj.errors[errMessageProp] = errMsg;
                if (!checkObjPresentInList(validationErrorList, obj, isCaseSensitive)) validationErrorList.push(obj);
            }

            if (checkObjPresentInList(List, allowWhiteSpace ? input : input.trim(), isCaseSensitive) || !input) {
                delete obj.errors[errMessageProp];
            }
        }
        if (obj && obj.errors && !factObj.isEmptyObj(obj.errors)) {
            factObj.removeObjInToArray(validationErrorList, obj);
        }
    };

    factObj.checkDuplicateValue = function (obj, mainModel, warningList, property, errorProp, isNotFirstLoad, attrType) {
        var inputValue;
        if (!inputValue) {
            inputValue = obj.dictAttributes[property];
        }
        if (isNotFirstLoad) {
            if (obj.warnings && obj.warnings[errorProp]) {
                delete obj.warnings[errorProp];
            }

            if (!inputValue) {
                if (obj && obj.warnings && obj.warnings.hasOwnProperty(errorProp)) {
                    delete obj.warnings[errorProp];
                }
            }
            if (obj.warnings && !factObj.isEmptyObj(obj.warnings)) {
                factObj.removeObjInToArray(warningList, obj);
            }
        }
        if (inputValue) {
            factObj.checkDuplicateValues(obj, mainModel, warningList, property, errorProp, attrType);
        }
    };
    factObj.checkDuplicateValues = function (Obj, mainModel, warningList, property, errorProp, attrType) {
        if (mainModel.Elements.length > 0) {
            angular.forEach(mainModel.Elements, function (currentObj) {

                if (currentObj.dictAttributes[property] && Obj.dictAttributes[property] && (currentObj.dictAttributes[property]).toLowerCase() == (Obj.dictAttributes[property]).toLowerCase() && currentObj != Obj) {
                    if (angular.isArray(attrType) && attrType.indexOf(currentObj.dictAttributes.sfwType) > -1) {
                        if (!Obj.warnings && !angular.isObject(Obj.warnings)) {
                            Obj.warnings = {};
                        }
                        var errorMessage = CONST.VALIDATION.DUPLICATE_FIELD;
                        if (currentObj.hasOwnProperty("isParent") && currentObj.isParent) {
                            errorMessage = "Duplicate field present in parent entity ";
                        }
                        if (Obj.warnings && !Obj.warnings[errorProp]) Obj.warnings[errorProp] = errorMessage;
                        if (!checkObjPresentInList(warningList, Obj)) warningList.push(Obj);
                    }
                }
                if (currentObj.Elements.length > 0) {
                    factObj.checkDuplicateValues(Obj, currentObj, warningList, property, errorProp, attrType);
                }
            });
        }
    };

    factObj.checkValidListValueForMultilevel = function (List, obj, value, entityId, property, errMessageProp, errMsg, validationErrorList_Param, isEmptyCheck, attrType) {
        var input = [], data = [], list = [];
        var inputValue;
        var isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression;
        if (typeof value !== "undefined") {
            inputValue = value;
        } else if (obj.dictAttributes && obj.dictAttributes[property]) {
            inputValue = obj.dictAttributes[property];
        } else if (obj[property]) {
            inputValue = obj[property];
        }

        if (inputValue) {
            input = inputValue.split('.');
            if (input.indexOf("") > -1) {
                list = [null]; // creating dummy list
                factObj.checkValidListValue(list, obj, ".", property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                return;
            }
            var arr = input.filter(Boolean);
            input = arr;
        } else {
            factObj.checkValidListValue(list, obj, value, property, errMessageProp, errMsg, validationErrorList_Param, isEmptyCheck, false, true);
        }
        if (attrType && attrType.length > 0) {
            isshowonetoone = attrType.contains('Object', true) ? true : false;
            isshowonetomany = attrType.contains('Collection', true) || attrType.contains('List', true) ? true : false;
            isshowcolumns = attrType.contains('Column', true) ? true : false;
            isshowcdocollection = attrType.contains('CDOCollection', true) ? true : false;
            isshowexpression = attrType.contains('Expression', true) ? true : false;
        }

        if (isshowonetomany) {
            if (input.length > 1) isshowonetoone = true;
            else {
                isshowonetomany = true; isshowonetoone = false;
            }
        }

        if (obj.Name) {
            if (["sfwGridView", "sfwChart", "sfwListView", "sfwCalendar", "sfwScheduler"].indexOf(obj.Name) > -1 && property == "sfwEntityField") {
                if (input.length > 1) isshowonetoone = true;
                else {
                    isshowonetomany = true; isshowonetoone = false;
                }
            } else if ("sfwCheckBoxList" == obj.Name && property == "sfwEntityField") {
                if (input.length > 1) isshowonetoone = true;
                else {
                    isshowcdocollection = true; isshowonetomany = true; isshowonetoone = false;
                }
            } else if (obj.Name == "sfwLabel") {
                isshowexpression = true;
            } else if (obj.Name == "udc" || obj.Name == "property") {
                isshowonetoone = true;
            }
        }
        var isDefaultFields = false;
        if (!isshowonetoone && !isshowonetomany && !isshowcolumns && !isshowcdocollection) {
            isDefaultFields = true;
        }


        if (input.length > 0) {
            // data = $rootScope.getEntityAttributeIntellisense(entityId, true, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression);
            data = factObj.getEntityAttributes(entityId, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression);
            if (obj.Name == "sfwDateTimePicker" && input.length == 1) {
                data = data.filter(function (x) { return x.DataType && x.DataType.toLowerCase() == "datetime"; });
            }
            list = factObj.getListByPropertyName(data, 'ID');
            if (obj.Name == "sfwGridView" || obj.Name == "sfwSoftErrors") {
                list.push("InternalErrors");
                list.push("ExternalErrors");
            }
            factObj.checkValidListValue(list, obj, input[0], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
            if (obj.errors && obj.errors[errMessageProp]) {
                return;
            }
            if (isDefaultFields) {
                var item = data.filter(function (x) { return x.ID == input[0]; });
                if (item.length > 0 && input.length == 1 && item[0].DataType && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection")) {
                    factObj.checkValidListValue([], obj, input[0], property, errMessageProp, CONST.VALIDATION.ONLY_OBJECT_OR_COLLECTION, validationErrorList_Param, false, false, true);
                }
            }
        }
        if (input.length > 1) {
            var invalidCase = false;
            for (var i = 0; i < input.length; i++) {
                if (attrType && attrType.length > 0 && attrType.contains('Object', true) && (attrType.contains('Collection', true) || attrType.contains('List', true)) && (i + 1) == (input.length - 1)) {
                    if (obj.Name != "property") isshowonetoone = false; isshowonetomany = true;
                }
                if (obj.Name == "sfwCheckBoxList" && property == "sfwEntityField" && (i + 1) == input.length - 1) {
                    isshowonetoone = false; isshowcdocollection = true; isshowonetomany = true;
                }
                if (["sfwGridView", "sfwChart", "sfwListView", "sfwCalendar"].indexOf(obj.Name) > -1 && property == "sfwEntityField" && (i + 1) == input.length - 1) {
                    isshowonetoone = false; isshowonetomany = true;
                }
                var item = data.filter(function (x) { return x.ID == input[i]; });
                if (item.length > 0 && item[0].DataType && item[0].DataType.toLowerCase() != "object" && input[i + 1]) { // when multilevel attribute excluding attribute type object next level attributes not valid attributes
                    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                    break;
                }
                if (item.length > 0 && isDefaultFields && i == (input.length - 1) && item[0].DataType && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection")) {
                    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, CONST.VALIDATION.ONLY_OBJECT_OR_COLLECTION, validationErrorList_Param, false, false, true);
                    break;
                }
                if (item.length > 0) {
                    if (typeof item[0].DataType != "undefined" && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection" || item[0].DataType.toLowerCase() == "cdocollection" || item[0].DataType.toLowerCase() == "list") && typeof item[0].Entity != "undefined") {
                        if (input[i + 1]) {
                            // data = $rootScope.getEntityAttributeIntellisense(item[0].Entity, true, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression);
                            //expression should not come for second level
                            data = factObj.getEntityAttributes(item[0].Entity, isshowonetoone, isshowonetomany, isshowcolumns, isshowcdocollection, isshowexpression);
                            if (obj.Name == "sfwDateTimePicker" && (i + 1) == input.length - 1) {
                                data = data.filter(function (x) { return x.DataType && x.DataType.toLowerCase() == "datetime"; });
                            }
                            list = factObj.getListByPropertyName(data, 'ID');
                            factObj.checkValidListValue(list, obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                        }
                    } else if (input[i + 1]) {
                        invalidCase = true;
                    }
                } else invalidCase = true;

                if (invalidCase) {
                    data = [];
                    factObj.checkValidListValue(data, obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                }
            }
        }
    };

    factObj.checkValidListValueForXMLMethod = function (List, obj, value, entityId, property, errMessageProp, errMsg, validationErrorList_Param, attrType, dataType, isDelete, isCorrespondenceXmlMethod) {
        var input = [], data = [], list = [];
        var inputValue;
        if (typeof value !== "undefined") {
            inputValue = value;
        } else if (obj.dictAttributes && obj.dictAttributes[property]) {
            inputValue = obj.dictAttributes[property];
        } else if (obj[property]) {
            inputValue = obj[property];
        }

        if (inputValue) {
            input = inputValue.split('.');
            if (input.indexOf("") > -1) {
                list = [null]; // creating dummy list
                factObj.checkValidListValue(list, obj, ".", property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                return;
            }
            var arr = input.filter(Boolean);
            input = arr;
        } else {
            factObj.checkValidListValue(list, obj, value, property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
        }
        if (angular.isArray(List) && List.length > 0) {
            data = List;
        }
        if (isDelete) {
            var lstData = $Entityintellisenseservice.GetIntellisenseData(entityId, attrType, dataType, true, true, true, false, false, false);
            var lsttempData = [];
            if (lstData) {
                for (var i = 0; i < lstData.length; i++) {
                    if (lstData[i].Parameters && lstData[i].Type == "Method" && lstData[i].Parameters.length == 0) {
                        lsttempData.push(lstData[i]);
                    }
                    if (lstData[i].Type != "Method") {
                        lsttempData.push(lstData[i]);
                    }
                }
            }
            data = data.concat(lsttempData);
        } else if (isCorrespondenceXmlMethod) {
            data = data.concat($Entityintellisenseservice.GetIntellisenseData(entityId, attrType, dataType, true, false, false, false, false, true));
        } else {
            data = data.concat($Entityintellisenseservice.GetIntellisenseData(entityId, attrType, dataType, true, true, true, true, false, false));
        }

        if (input.length > 0) {
            list = factObj.getListByPropertyName(data, 'ID');

            factObj.checkValidListValue(list, obj, input[0], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
            if (obj.errors && obj.errors[errMessageProp]) {
                return;
            }
            //if (isDefaultFields) {
            //    var item = data.filter(function (x) { return x.ID == input[0] });
            //    if (item.length > 0 && input.length == 1 && item[0].DataType && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection")) {
            //        factObj.checkValidListValue([], obj, input[0], property, errMessageProp, CONST.VALIDATION.ONLY_OBJECT_OR_COLLECTION, validationErrorList_Param, false, false, true);
            //    }
            //}
        }

        if (input.length > 1) {
            var invalidCase = false;
            for (var i = 0; i < input.length; i++) {

                var item = data.filter(function (x) { return x.ID == input[i]; });
                if (item.length > 0 && item[0].DataType && item[0].DataType.toLowerCase() != "object" && input[i + 1]) { // when multilevel attribute excluding attribute type object next level attributes not valid attributes
                    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                    break;
                }
                //if (item.length > 0 && isDefaultFields && i == (input.length - 1) && item[0].DataType && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection")) {
                //    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, CONST.VALIDATION.ONLY_OBJECT_OR_COLLECTION, validationErrorList_Param, false, false, true);
                //    break;
                //}
                if (item.length > 0) {
                    if (typeof item[0].DataType != "undefined" && (item[0].DataType.toLowerCase() == "object" || item[0].DataType.toLowerCase() == "collection" || item[0].DataType.toLowerCase() == "cdocollection" || item[0].DataType.toLowerCase() == "list") && typeof item[0].Entity != "undefined") {
                        if (input[i + 1]) {
                            if (isDelete) {
                                var lstData = $Entityintellisenseservice.GetIntellisenseData(item[0].Entity, attrType, dataType, true, true, true, false, false, false);
                                var lsttempData = [];
                                if (lstData) {
                                    for (var j = 0; j < lstData.length; j++) {
                                        if (lstData[j].Parameters && lstData[j].Type == "Method" && lstData[j].Parameters.length == 0) {
                                            lsttempData.push(lstData[j]);
                                        }
                                        if (lstData[j].Type != "Method") {
                                            lsttempData.push(lstData[j]);
                                        }
                                    }
                                }
                                data = lsttempData;
                            } else {
                                data = $Entityintellisenseservice.GetIntellisenseData(item[0].Entity, attrType, dataType, true, true, true, true, false, false);
                            }
                            list = factObj.getListByPropertyName(data, 'ID');
                            factObj.checkValidListValue(list, obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                        }
                    } else if (input[i + 1]) {
                        invalidCase = true;
                    }
                } else invalidCase = true;

                if (invalidCase) {
                    data = [];
                    factObj.checkValidListValue(data, obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, false, false, true);
                }
            }
        }

    };

    factObj.checkDataFieldValue = function (List, obj, value, attributeName, property, errMessageProp, errMsg, validationErrorList_Param) {
        var data = [], list = [];
        if (List && List.length > 0) {
            if (obj.Name == "sfwDateTimePicker") {
                data = List.filter(function (x) { return x.DataType && x.DataType.toLowerCase() == "datetime"; });
                list = factObj.getListByPropertyName(data, attributeName, false);
                factObj.checkValidListValue(list, obj, value, property, errMessageProp, errMsg, validationErrorList_Param);
            } else {
                list = factObj.getListByPropertyName(List, attributeName, false);
                factObj.checkValidListValue(list, obj, value, property, errMessageProp, errMsg, validationErrorList_Param);
            }
        } else {
            factObj.checkValidListValue(list, obj, value, property, errMessageProp, errMsg, validationErrorList_Param);
        }

    };


    factObj.checkValidQuery = function (List, obj, value, querytype, property, errMessageProp, errMsg, validationErrorList_Param, ablnWithoutParameter, isempty) {
        var input = [], data = [], list = [];
        if (value == "") {
            factObj.checkValidListValue(List, obj, value, property, errMessageProp, errMsg, validationErrorList_Param, isempty);
            return;
        }
        if (typeof value !== "undefined") {
            input = value.split('.');
            if (input.indexOf("") > -1 || input.length == 1) {
                factObj.checkValidListValue(["null"], obj, ".", property, errMessageProp, errMsg, validationErrorList_Param, isempty);
                return;
            }
            var arr = input.filter(Boolean);
            input = arr;
        } else {
            factObj.checkValidListValue(["null"], obj, "null", property, errMessageProp, errMsg, validationErrorList_Param, isempty);
            return;
        }

        if (input.length > 0) {
            data = $EntityIntellisenseFactory.getEntityNameList();
            factObj.checkValidListValue(data, obj, input[0], property, errMessageProp, errMsg, validationErrorList_Param, isempty);
            if (obj.errors && obj.errors[errMessageProp]) {
                return;
            }
        }
        if (input.length > 1) {
            for (var i = 0; i < input.length; i++) {
                var item = List.filter(function (x) { return x.ID == input[i]; });
                if (item.length > 0 && item[0].Queries) {
                    if (querytype == "SubSelectQuery") {
                        data = item[0].Queries.filter(function (x) { return x.QueryType == "SubSelectQuery"; });
                    }
                    else if (querytype == "ScalarQuery") {
                        data = item[0].Queries.filter(function (x) { return x.QueryType == "ScalarQuery"; });
                    }
                    else {
                        data = item[0].Queries.filter(function (x) { return x.QueryType != "SubSelectQuery"; });
                    }
                    if (input[i + 1]) {
                        list = factObj.getListByPropertyName(data, 'ID');
                        factObj.checkValidListValue(list, obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, isempty);
                        if (ablnWithoutParameter) {
                            var lobjquery = data.filter(function (x) { return x.ID == input[i + 1]; });
                            if (lobjquery && lobjquery.length > 0) {
                                if (lobjquery[0].Parameters.length > 0) {
                                    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, CONST.VALIDATION.QUERY_WITH_PARAMETER, validationErrorList_Param, isempty);
                                }
                            }
                        }
                    }
                    else data = [];
                } else if (i > 1) {
                    factObj.checkValidListValue([], obj, input[i + 1], property, errMessageProp, errMsg, validationErrorList_Param, isempty);
                }
            }
        }
    };

    factObj.getListByPropertyName = function (list, property, reverse) {
        var resultList = [];
        if (list && list.length > 0 && property) {
            angular.forEach(list, function (obj) {
                if (obj && obj.hasOwnProperty(property)) {
                    var value = obj[property];
                    if (value) {
                        resultList.push(value.trim());
                    }
                }
                else if (obj && obj.dictAttributes && obj.dictAttributes.hasOwnProperty(property)) {
                    var value = obj.dictAttributes[property];
                    if (value) {
                        resultList.push(value.trim());
                    }
                }
            });
        }
        resultList = resultList.sort();
        if (reverse) {
            resultList = resultList.reverse();
        }

        return resultList;
    };

    factObj.checkMultipleValueWithList = function (List, obj, input, splitChar, property, errMessageProp, errMsg, validationErrorList_Param) {
        var value;
        var isValid = false;
        if (input) {
            input = input.split(splitChar);
        } else {
            factObj.checkValidListValue(["null"], obj, "null", property, errMessageProp, errMsg, validationErrorList_Param);
            return;
        }
        for (var i = 0; i < input.length; i++) {
            value = input[i];
            if (!checkObjPresentInList(List, value)) {
                isValid = true;
                break;
            }
        }
        if (isValid || (obj.errors && obj.errors[errMessageProp])) {
            factObj.checkValidListValue(List, obj, value, property, errMessageProp, errMsg, validationErrorList_Param);
        }
    };
    factObj.checkActiveForm = function (List, obj, input, property, errMessageProp, errMsg, validationErrorList_Param) {
        if (input && (input.indexOf(';') > -1 || input.contains('='))) {
            var value = input.split(';');
            var newInput = "";
            for (var i = 0; i < value.length; i++) {
                var text;
                if (value[i].indexOf('=') > -1) newInput += (value[i].split('=')[1]) + ";";
                else newInput += value[i] + ";";
            }
            newInput = newInput.slice(0, -1);
            factObj.checkMultipleValueWithList(List, obj, newInput, ";", property, errMessageProp, errMsg, validationErrorList_Param);
        } else {
            factObj.checkValidListValue(List, obj, input, property, errMessageProp, errMsg, validationErrorList_Param);
        }
    };

    // data type like int,string,bool,short,double ...
    factObj.validateDataTypes = function (obj, value, dataType, validationErrorList_Param) {
        var errMessageProp = "invalid_data_type_value";
        var errorMessage = CONST.VALIDATION.INVALID_DATATYPE;
        var invalid = false;
        if (!value) {
            value = ""; invalid = true;
            errorMessage = CONST.VALIDATION.EMPTY_VALUE;
        }
        if (obj) {
            switch (dataType) {
                case "int":
                    if (isNaN(value) || value.contains(".")) {
                        invalid = true;
                    } if (!isNaN(value) && parseInt(value) > 2147483647 || parseInt(value) < -2147483648) {
                        invalid = true;
                    }
                    break;
                case "string":
                    break;
                case "bool":
                    if (["true", "false"].indexOf(value) <= -1) {
                        invalid = true;
                    }
                    break;
                case "decimal":
                    if (isNaN(value)) {
                        invalid = true;
                    } else if (!isNaN(value) && parseFloat(value) > 79228162514264337593543950335 || parseFloat(value) < -79228162514264337593543950335) {
                        invalid = true;
                    }
                    break;
                case "double":
                    if (isNaN(value)) {
                        invalid = true;
                    } else if (!isNaN(value) && parseFloat(value) > Number.MAX_VALUE || parseFloat(value) == Number.NEGATIVE_INFINITY) {
                        invalid = true;
                    }
                    break;
                case "float":
                    if (isNaN(value)) {
                        invalid = true;
                    } else if (!isNaN(value) && parseFloat(value) > Number.MAX_VALUE || parseFloat(value) == Number.NEGATIVE_INFINITY) {
                        invalid = true;
                    }
                    break;
                case "long":
                    if (isNaN(value) || value.contains(".")) {
                        invalid = true;
                    } else if (!isNaN(value) && parseInt(value) > 9223372036854775807 || parseInt(value) < -9223372036854775808) {
                        invalid = true;
                    }
                    break;
                case "short":
                    if (isNaN(value) || value.contains(".")) {
                        invalid = true;
                    } else if (!isNaN(value) && parseInt(value) > 32767 || parseInt(value) < -32768) {
                        invalid = true;
                    }
                    break;
            }
            if (validationErrorList_Param) {
                validationErrorList = validationErrorList_Param;
            } else {
                validationErrorList = getValidationListObj(isDialog);
            }
            if (obj && !obj.errors && !angular.isObject(obj.errors)) {
                obj.errors = {};
            }
            delete obj.errors[errMessageProp];

            if (invalid) {
                if (!obj.errors[errMessageProp]) obj.errors[errMessageProp] = errorMessage;
                if (!checkObjPresentInList(validationErrorList, obj)) validationErrorList.push(obj);
            }
            if (obj && obj.errors && !factObj.isEmptyObj(obj.errors)) {
                factObj.removeObjInToArray(validationErrorList, obj);
            }
        }
    }
    factObj.createFullPath = function (modelObj, findList) {
        var path = "";
        if (modelObj && findList) {
            var parentObjIndex = -1;
            parentObjIndex = modelObj.Elements.indexOf(findList[0]);
            if (parentObjIndex > -1) {
                path += findList[0].Name + "-" + parentObjIndex;
            }
            var childObjIndex = -1;
            var len = findList.length - 1;
            for (var i = 0; i < len; i++) {
                childObjIndex = -1;
                childObjIndex = findList[i].Elements.indexOf(findList[i + 1]);
                if (childObjIndex > -1) {
                    path += "," + findList[i + 1].Name + "-" + childObjIndex;
                }
            }
        }
        return path;
    };
    // Method for go to perticular position
    factObj.scrollToPosition = function (parent, child, targetClass) {
        var ID = $rootScope.currentopenfile && $rootScope.currentopenfile.file ? $rootScope.currentopenfile.file.FileName : undefined;
        if ((parent.indexOf('.') > -1 || parent.indexOf('#') > -1) && child.indexOf('.') > -1 || child.indexOf('#') > -1 && ID) {
            var mainSelector = $('#' + ID).contents();
            $timeout(function () {
                setTimeout(function () {
                    var elem = $(mainSelector).find(parent + child).find('.' + targetClass);
                    if (elem) {
                        $(mainSelector).find(parent + child).scrollTo(elem, null, null);
                    }
                }, 500);
            });
        }
    };

    //Validate Rules
    factObj.validateRule = function (obj, value, baseEntityId, nonStatic, returnTypes, ruleTypes, propertyName, allowEmpty, fieldName) {
        if (obj) {
            if (!fieldName) fieldName = "field value";
            if (value) {
                isValid = false;
                var entityId = null;
                var ruleId = null;
                var dotIndex = value.indexOf(".");
                if (dotIndex > -1) {
                    var entityRule = value.split(".");
                    if (entityRule.length == 2) {
                        entityId = entityRule[0];
                        ruleId = entityRule[1];
                    }
                }
                else {
                    entityId = baseEntityId;
                    ruleId = value;
                }

                if (entityId && ruleId) {
                    var objEntity = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (itm) { return itm.ID === entityId; })[0];
                    if (objEntity) {
                        if (objEntity.Rules.some(function (itm) {
                            return itm.ID === ruleId
                                &&
                                (itm.Status.toLowerCase() === "active")
                                &&
                                (!ruleTypes || !ruleTypes.length || ruleTypes.indexOf(itm.RuleType.toLowerCase()) > -1)
                                &&
                                (!returnTypes || !returnTypes.length || returnTypes.indexOf(itm.ReturnType.toLowerCase()) > -1);
                        })) {
                            isValid = true;
                        }
                    }
                }

                if (!isValid) {
                    if (!obj.errors) {
                        obj.errors = {};
                    }
                    obj.errors[propertyName] = String.format(CONST.VALIDATION.INVALID_GENERIC, fieldName);
                }
                else if (obj && obj.errors && obj.errors[propertyName]) {
                    obj.errors[propertyName] = null;
                }
            }
            else if (!allowEmpty) {
                if (!obj.errors) {
                    obj.errors = {};
                }
                obj.errors[propertyName] = String.format(CONST.VALIDATION.EMPTY_GENERIC, fieldName);
            }
        }
    };

    return factObj;
}]);

app.filter('unique', [function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });

        return output;
    };
}]);

app.value("$ObjectsSearch", {
    QueryObj: { NodeName: "", Attribute: "", Operator: "", AttributeValue: "", isWholeWord: false, isMatchCase: false, operatorList: [] },
    AdvanceQueryObj: {
        fileName: "",
        containingText: "",
        location: "",
        isCreatedDate: false,
        isModifiedDate: false,
        fromDate: "",
        toDate: "",
        fileType: [],
        IsGetAllFiles: false,
        QuerySearchList: []
    },
    FileTypesQueryReference: ["Entity", "Lookup", "Maintenance", "Wizard", "UserControl",
        "Tooltip", "FormLinkLookup", "FormLinkMaintenance", "FormLinkWizard",
        "Report", "BPMN", "BPMTemplate", "Correspondence", "Prototype", "OutboundFile"
    ]
});

app.factory("$searchQuerybuilder", ["$ObjectsSearch", function ($ObjectsSearch) {
    var QueryBuilder = { lstFilter: [], isAdvanceSearch: true };
    return {
        AddsearchQuerybuilderObj: function (DesignQuerySearchCriteriaList) {
            var objQuery = angular.copy($ObjectsSearch.QueryObj);
            objQuery.operatorList = this.getOperatorList();
            DesignQuerySearchCriteriaList.push(objQuery);
        },
        DeletesearchQuerybuilderObj: function (DesignQuerySearchCriteriaList, DesignQuerySearchObj) {
            var index = DesignQuerySearchCriteriaList.indexOf(DesignQuerySearchObj);
            DesignQuerySearchCriteriaList.splice(index, 1);
        },
        getOperatorList: function () {
            if (arguments.length == 0) {
                return [{ name: "Has Attribute", value: "HasAttribute", key: "A" }, { name: "Has Node", value: "HasNode", key: "N" }, { name: "Contains", value: "Contains", key: "V" }, { name: "Equal To", value: "EqualTo", key: "V" }];
            }
            else {
                switch (arguments[0]) {
                    case "N": return [{ name: "Has Node", value: "HasNode", key: "N" }];
                    case "A": return [{ name: "Has Attribute", value: "HasAttribute", key: "A" }];
                    case "V": return [{ name: "Contains", value: "Contains", key: "V" }, { name: "Equal To", value: "EqualTo", key: "V" }];
                }
            }
        },
        onchangeQueryBuilder: function (DesignSearchObj, form, index) {
            if (DesignSearchObj.AttributeValue) {
                if (form.hasOwnProperty('AttributeValue' + index)) {
                    if (form['AttributeValue' + index].$invalid) {
                        form['AttributeValue' + index].$setValidity('data-value', true);
                    }
                }
            }
            if (DesignSearchObj.Attribute) {
                if (form.hasOwnProperty('Attribute' + index)) {
                    if (form['Attribute' + index].$invalid) {
                        form['Attribute' + index].$setValidity('data-value', true);
                    }
                }
            }
            if (DesignSearchObj.NodeName) {
                if (form.hasOwnProperty('NodeName' + index)) {
                    if (form['NodeName' + index].$invalid) {
                        form['NodeName' + index].$setValidity('data-value', true);
                    }
                }
            }
            this.setcontrolValidity(DesignSearchObj, form, index);
            // set operators and validity for single query line                
            //if (isAttribute && !DesignSearchObj.Attribute) {
            //    DesignSearchObj.AttributeValue = "";
            //}
            //if (DesignSearchObj.AttributeValue) {
            //    var IsNotValueMode = this.IsOperatorMode("V", DesignSearchObj);
            //    if (IsNotValueMode) {
            //        DesignSearchObj.operatorList = this.getOperatorList("V");
            //        DesignSearchObj.Operator = "";
            //    }
            //}
            //else if (DesignSearchObj.Attribute) {
            //    var IsNotAttributeMode = this.IsOperatorMode("A", DesignSearchObj);
            //    if (IsNotAttributeMode) {
            //        DesignSearchObj.operatorList = this.getOperatorList("A");
            //        DesignSearchObj.Operator = "HasAttribute";
            //    }
            //}
            //else if (DesignSearchObj.NodeName) {
            //    var IsNotNodeMode = this.IsOperatorMode("N", DesignSearchObj);
            //    if (IsNotNodeMode) {
            //        DesignSearchObj.operatorList = this.getOperatorList("N");
            //        DesignSearchObj.Operator = "HasNode";
            //    }
            //}
            //else {
            //    DesignSearchObj.operatorList = this.getOperatorList();
            //    DesignSearchObj.Operator = "";
            //}
        },
        setcontrolValidity: function (DesignSearchObj, form, index) {
            if (DesignSearchObj.Operator == "Contains" || DesignSearchObj.Operator == "EqualTo") {
                if (!DesignSearchObj.AttributeValue) {
                    if (form.hasOwnProperty('AttributeValue' + index)) {
                        form['AttributeValue' + index].$setValidity('data-value', false);
                    }
                }
            }
            else if (DesignSearchObj.Operator == "HasAttribute") {
                if (!DesignSearchObj.Attribute) {
                    if (form.hasOwnProperty('Attribute' + index)) {
                        form['Attribute' + index].$setValidity('data-value', false);
                    }
                }
            }
            else if (DesignSearchObj.Operator == "HasNode") {
                if (!DesignSearchObj.NodeName) {
                    if (form.hasOwnProperty('NodeName' + index)) {
                        form['NodeName' + index].$setValidity('data-value', false);
                    }
                }
            }
        },
        onchangeOperatorQueryBuilder: function (DesignSearchObj, form, index) {
            if (form.hasOwnProperty('AttributeValue' + index)) {
                form['AttributeValue' + index].$setValidity('data-value', true);
            }
            if (form.hasOwnProperty('Attribute' + index)) {
                form['Attribute' + index].$setValidity('data-value', true);
            }
            if (form.hasOwnProperty('NodeName' + index)) {
                form['NodeName' + index].$setValidity('data-value', true);
            }
            this.setcontrolValidity(DesignSearchObj, form, index);
        },
        getcssClassInvalidError: function (form, propertyname) {
            if (form.hasOwnProperty(propertyname)) {
                if (form[propertyname].$invalid) {
                    return 'invalid-input-error';
                }
            }
            return '';
        },
        IsOperatorMode: function (mode, DesignSearchObj) {
            if (DesignSearchObj.operatorList.length > 0) {
                return DesignSearchObj.operatorList.some(function (operator) {
                    if (operator.key != mode) {
                        return true;
                    }
                });
            }
        },
        getGlobalQueryBuilder: QueryBuilder,
        setGlobalQueryBuilderFilters: function (Filters) {
            QueryBuilder.lstFilter = Filters;
        },
        setQueryBuilderIsAdvanceOption: function (isAdvanceSearch) {
            QueryBuilder.isAdvanceSearch = isAdvanceSearch;
        },
        clearSearchQueryBuilder: function () {
            QueryBuilder.lstFilter = [];
            QueryBuilder.isAdvanceSearch = true;
        }
    };
}]);

app.factory("$searchFindReferences", ["$ObjectsSearch", "$rootScope", "hubcontext", function ($ObjectsSearch, $rootScope, hubcontext) {
    var objReferences = { type: '', lstReferences: [], activeReference: null, ReferenceValue: "", activeReferenceID: "" };
    return {
        get: function (strQueryName, ReferenceType) {
            var ObjSearch = angular.copy($ObjectsSearch.AdvanceQueryObj);
            var ObjQuery = angular.copy($ObjectsSearch.QueryObj);
            ObjQuery.Operator = "Contains";
            ObjQuery.AttributeValue = strQueryName;
            ObjQuery.isWholeWord = true;
            ObjQuery.isMatchCase = true;
            objReferences.ReferenceValue = strQueryName;
            ObjSearch.ReferenceType = ReferenceType;
            ObjSearch.FileName = $rootScope.currentopenfile.file.FileName;
            ObjSearch.FileType = $ObjectsSearch.FileTypesQueryReference;
            ObjSearch.QuerySearchList.push(ObjQuery);
            return hubcontext.hubSearch.server.getFindReference(ObjSearch);
        },
        getData: objReferences,
        setData: function (type, lstReferences, activeReference) {
            objReferences.type = type ? type : objReferences.type;
            objReferences.lstReferences = lstReferences ? lstReferences : objReferences.lstReferences;
            objReferences.activeReference = activeReference;
            objReferences.activeReferenceID = activeReference ? activeReference.FileInfo.FileName + activeReference.LineNumber : objReferences.activeReferenceID;
        },
        resetData: function () {
            objReferences.type = "";
            objReferences.lstReferences = [];
            objReferences.activeReference = null;
            objReferences.activeReferenceID = "";
            objReferences.ReferenceValue = "";
        }
    };
}]);

app.service('$ValidateBaseModelStructure', [function () {
    this.is_validModelElements = function (objModel, strModelName, IsParent) {
        var IsValid = false;
        if (objModel && objModel.dictAttributes && objModel.Name == strModelName) {
            IsValid = true;
            if (IsParent) {
                IsValid = (objModel.Elements && objModel.Elements.length > 0) ? true : false;
            }
        }
        return IsValid;
    };
}]);

app.service("$SgMessagesService", ["$rootScope", function ($rootScope) {
    this.Message = function (aTitle, aMessage, aIsConfirm, aCallback, aSize) {
        var config = $rootScope.$new(true);
        if (dialog) {
            closeDailog(dialog);
        }
        var dialog = null;
        config.message = aMessage;
        config.isConfirmBox = aIsConfirm;
        var dialogwidth = 400;
        var dialogheight = 170;
        if (aSize) {
            if (aSize.height) {
                dialogheight = aSize.height;
            }
            if (aSize.width) {
                dialogwidth = aSize.width;
            }
        }
        config.$evalAsync(function () {
            dialog = $rootScope.showDialog(config, aTitle, "Common/views/sgAlertBox.html", {
                width: dialogwidth, height: dialogheight, isAlert: true
            });
        });
        config.cancelConfirmClick = function () {
            if (typeof aCallback === "function") {
                aCallback(false);
            }
            closeDailog(dialog);
        }
        config.okConfirmClick = function () {
            if (typeof aCallback === "function") {
                aCallback(true);
            }
            closeDailog(dialog);
        }
        return;
    }
    var closeDailog = function (aDailog) {
        if (aDailog) {
            aDailog.close();
        }
    }
}]);

app.service("$GetGridEntity", ["$GetEntityFieldObjectService", function ($GetEntityFieldObjectService) {
    this.getEntityName = function (formObj, selectedControl) {
        var entityName = null;
        var objGrid = null;
        if (formObj && (formObj.dictAttributes.sfwType == "Lookup" || formObj.dictAttributes.sfwType == "FormLinkLookup")) {
            entityName = formObj.dictAttributes.sfwEntity;
        }
        else if (formObj && selectedControl) {
            objGrid = FindParent(selectedControl, "sfwGridView");
            if (objGrid && objGrid.dictAttributes.sfwParentGrid && objGrid.dictAttributes.sfwEntityField) {
                var objParentGrid = FindControlByID(formObj, objGrid.dictAttributes.sfwParentGrid);
                if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                    var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(formObj.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                    if (entObject) {
                        entityName = entObject.Entity;
                        if (FindParent(selectedControl, "ItemTemplate")) {
                            var entObj = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityName, objGrid.dictAttributes.sfwEntityField);
                            if (entObj) {
                                entityName = entObj.Entity;
                            }
                        }
                    }
                }
            }
            if (selectedControl && objGrid && objGrid.dictAttributes.sfwParentGrid && (selectedControl.Name == "TemplateField" || FindParent(selectedControl, "HeaderTemplate"))) {
                var objParentGrid = FindControlByID(formObj, objGrid.dictAttributes.sfwParentGrid);
                if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                    var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(formObj.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                    if (entObject) {
                        entityName = entObject.Entity;
                    }
                }
            } else if (selectedControl && (selectedControl.Name == "TemplateField" || FindParent(selectedControl, "HeaderTemplate"))) {
                entityName = formObj.dictAttributes.sfwEntity;
            } else if (objGrid) {
                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(formObj.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                if (entObject) {
                    entityName = entObject.Entity;
                }
            }
        }
        return entityName;
    }
}]);

app.factory('$IentBaseMethodsFactory', [function () {
    var ientBaseMethods = [];
    return {
        setIentBaseMethods: function (methods) {
            angular.forEach(methods, function (method) {
                if (method.MethodName) {
                    method.ID = method.MethodName;
                }
            });
            ientBaseMethods = methods;
        },
        getIentBaseMethods: function () {
            return ientBaseMethods;
        },
    };
}]);

app.factory("$ModuleValidationService", ["$FormValidationService", "$entityValidationService", function ($FormValidationService, $entityValidationService) {
    var lobjFact = {};
    lobjFact.validate = function (aobjData, aobjScope, astrType) {
        if (astrType === "Form") {
            $FormValidationService.validateFileData(aobjData, aobjScope);
        } else if (astrType === "Entity") {
            $entityValidationService.validateFileData(aobjData, aobjScope);
        }
    }
    return lobjFact;
}]);
app.service('$getEnitityRule', ["$rootScope", "$filter", "$Entityintellisenseservice", function ($rootScope, $filter, $Entityintellisenseservice) {
    // will give a rule info like all rule ids (an array) and a rule object with {ID,Parameters,ReturnType} for entityID as input;
    this.getWithParam = function (entityID, isStatic, ruleType) {
        var entityRule = [];
        var ruleIds = [];
        if (entityID) {
            var lstRules = $Entityintellisenseservice.GetIntellisenseData(entityID, "", "", true, false, false, true, false, false);
            if (lstRules && lstRules.length > 0) {
                // valid entity ID
                var Rules = [];
                if (ruleType) {
                    lstRuleTypes = ruleType.split(",");
                    for (var i = 0; i < lstRuleTypes.length; i++) {
                        var ruletype = lstRuleTypes[i];
                        Rules = Rules.concat($filter('filter')(lstRules, { IsStatic: isStatic, RuleType: ruletype }, true));
                    }
                }
                else {
                    Rules = $filter('filter')(lstRules, { IsStatic: isStatic }, true);
                }
                // selected entity has rules
                if (Rules.length > 0) {
                    function iterator(value) {
                        ruleIds.push(value.ID);
                        this.push({ ID: value.ID, Parameters: value.Parameters, ReturnType: value.ReturnType });
                    }
                    angular.forEach(Rules, iterator, entityRule);
                }
            }
        }
        return { objRule: entityRule, ruleIds: ruleIds };
    };
}]);

app.service('$getEnitityXmlMethods', ["$rootScope", "$filter", "$Entityintellisenseservice", function ($rootScope, $filter, $Entityintellisenseservice) {
    // will give a XMl methods for entityID as input;
    this.get = function (entityID, methodType, mode) {
        var entityXmlmethods = [];
        if (entityID) {
            entityXmlmethods = $Entityintellisenseservice.GetIntellisenseData(entityID, "", "", true, false, false, false, false, true);
            if (entityXmlmethods && entityXmlmethods.length > 0) {
                if (methodType && methodType.trim().length > 0) {
                    if (methodType.toLowerCase() == "load") {
                        entityXmlmethods = entityXmlmethods.filter(function (item) { return item.MethodType.toLowerCase() == "load"; });
                        if (mode && mode.trim().length > 0) {
                            if (mode.toLowerCase() == "new") {
                                entityXmlmethods = entityXmlmethods.filter(function (item) { return item.Mode.toLowerCase() == "new" || item.Mode.toLowerCase() == "all"; });
                            }
                            else if (mode.toLowerCase() == "update") {
                                entityXmlmethods = entityXmlmethods.filter(function (item) { return item.Mode.toLowerCase() == "update" || item.Mode.toLowerCase() == "all"; });
                            }
                        }
                    }
                    else if (methodType.toLowerCase() == "nonload") {
                        entityXmlmethods = entityXmlmethods.filter(function (item) { item.MethodType.toLowerCase() != "load"; });
                    }
                }
            }
        }
        return entityXmlmethods;
    };
    // will give a XMl methods for business object ID as input;
    this.getByBusinessObject = function (objectID) {
        var entityXmlmethods = [];
        if (objectID) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = $filter('filter')(entityIntellisenseList, { BusinessObjectName: objectID }, true);
            if (entity && entity.length > 0) {
                entityXmlmethods = entity[0].XmlMethods;
            }
        }
        return entityXmlmethods;
    };
}]);

app.service('$getEnitityObjectMethods', ["$rootScope", "$filter", "$Entityintellisenseservice", function ($rootScope, $filter, $Entityintellisenseservice) {
    // will give a XMl methods for entityID as input;
    this.get = function (entityID) {
        var entityObjectMethods = [];
        if (entityID) {
            entityObjectMethods = $Entityintellisenseservice.GetIntellisenseData(entityID, "", "", true, false, true, false, false, false);
        }
        return entityObjectMethods;
    };
}]);

app.service('$Entityintellisenseservice', ["$filter", "$EntityIntellisenseFactory", "$Entityintellisensefilterservice", function ($filter, $EntityIntellisenseFactory, $Entityintellisensefilterservice) {
    // will give a rule info like all rule ids (an array) and a rule object with {ID,Parameters,ReturnType} for entityID as input;
    this.GetIntellisenseData = function (entityID, types, datatypes, isloadinheritedobjects, isloadAttributes, isloadobjectmethods, isloadrules, isloadQueries, isloadXMLMethods) {
        var data = [];
        var lstAttributes = [];
        var lstRules = [];
        var lstObjectMethods = [];
        var lstQueries = [];
        var lstXMLMethods = [];
        if (entityID) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            parentEntityID = entityID;
            while (parentEntityID) {
                var entity = $filter('filter')(entityIntellisenseList, { ID: parentEntityID }, true);
                if (entity && entity.length > 0) {
                    // valid entity ID
                    if (isloadAttributes) {
                        var attributes = entity[0].Attributes;
                        lstAttributes = lstAttributes.concat(attributes);
                    }
                    if (isloadinheritedobjects) {
                        parentEntityID = entity[0].ParentId;
                    } else {
                        parentEntityID = "";
                    }
                    if (isloadobjectmethods) {
                        lstObjectMethods = lstObjectMethods.concat(entity[0].ObjectMethods);
                    }
                    if (isloadrules) {
                        lstRules = lstRules.concat(entity[0].Rules);
                    }
                    if (isloadQueries) {
                        lstQueries = lstQueries.concat(entity[0].Queries);
                    }

                    if (isloadXMLMethods) {
                        lstXMLMethods = lstXMLMethods.concat(entity[0].XmlMethods);
                    }
                } else {
                    parentEntityID = "";
                }
            }
            if (lstAttributes.length > 0 && (types || datatypes)) {
                lstAttributes = $Entityintellisensefilterservice.GetIntellisenseFilterData(lstAttributes, types, datatypes);
            }

            data = data.concat(lstAttributes).concat(lstObjectMethods).concat(lstRules).concat(lstQueries).concat(lstXMLMethods);
        }
        return data;
    };
}]);

app.service('$Entityintellisensefilterservice', ["$filter", function ($filter) {
    // will give a rule info like all rule ids (an array) and a rule object with {ID,Parameters,ReturnType} for entityID as input;
    this.GetIntellisenseFilterData = function (lstAttributes, type, datatype) {
        var filteredData = [];
        if (type) {
            var types = type.split(",");
            //types
            var lstTypeAttr = [];
            var isObjectFound = false;
            for (var i = 0; i < types.length; i++) {
                if (types[i] == "Object") {
                    isObjectFound = true;
                }
                lstTypeAttr = lstTypeAttr.concat($filter('filter')(lstAttributes, { Type: types[i] }));
            }
            if (!isObjectFound) {
                lstTypeAttr = lstTypeAttr.concat($filter('filter')(lstAttributes, { Type: "Object" }));
            }
            if (datatype && lstTypeAttr.length > 0) {
                //datatypes
                var datatypes = datatype.split(",");
                lstDataTypeAttr = [];
                for (var i = 0; i < datatypes.length; i++) {
                    lstDataTypeAttr = lstDataTypeAttr.concat($filter('filter')(lstTypeAttr, { DataType: datatypes[i] }));
                }
                filteredData = lstDataTypeAttr;
            } else {
                filteredData = lstTypeAttr;
            }
        } else {
            if (datatype) {
                //datatypes
                var datatypes = datatype.split(",");
                lstDataTypeAttr = [];
                for (var i = 0; i < datatypes.length; i++) {
                    lstDataTypeAttr = lstDataTypeAttr.concat($filter('filter')(lstAttributes, { DataType: datatypes[i] }));
                }
                filteredData = lstDataTypeAttr;
            } else {
                filteredData = lstAttributes;
            }
        }
        return filteredData;
    };
}]);

app.service('$GetEntityFieldObjectService', ["$filter", "$Entityintellisenseservice", function ($filter, $Entityintellisenseservice) {
    // will give a rule info like all rule ids (an array) and a rule object with {ID,Parameters,ReturnType} for entityID as input;
    this.GetEntityFieldObjectFromEntityField = function (entityID, EntityField) {
        var objField = "";
        if (EntityField) {
            var lstFieldNames = EntityField.split('.');
            if (entityID && EntityField) {
                var data = $Entityintellisenseservice.GetIntellisenseData(entityID, "", "", true, true, false, false, false, false);
                if (lstFieldNames.length > 0) {
                    for (var i = 0; i < lstFieldNames.length; i++) {
                        var objItem = $filter('filter')(data, { ID: lstFieldNames[i] }, true);
                        if (objItem && objItem.length > 0) {
                            objField = objItem[0];
                            data = $Entityintellisenseservice.GetIntellisenseData(objField.Entity, "", "", true, true, false, false, false, false);
                        } else {
                            objField = "";
                        }
                    }
                }
            }
        }
        return objField;
    };
    this.GetEntityFieldObjectFromFieldValue = function (entityID, ObjectField) {
        var entField = "";
        var data = $Entityintellisenseservice.GetIntellisenseData(entityID, "", "", true, true, false, false, false, false);
        if (ObjectField) {
            var objItem = $filter('filter')(data, { Value: ObjectField }, true);
            if (objItem && objItem.length > 0) {
                entField = objItem[0];
            } else {
                entField = "";
            }
        }
        return entField;
    };
}]);

app.service('$getModelList', [function ($rootScope, $filter, $Entityintellisenseservice) {
    // will give a rule info like all rule ids (an array) and a rule object with {ID,Parameters,ReturnType} for entityID as input;
    this.getModelListFromQueryFieldlist = function (lstfields) {
        var lst = [];
        if (lstfields && lstfields.length > 0) {
            for (var i = 0; i < lstfields.length; i++) {
                var query = lstfields[i];
                if (query.Data1 && query.Data1 != null) {
                    datatype = query.Data1.toLowerCase();
                }
                else if (query.DataType) {
                    datatype = query.DataType.toLowerCase();
                }
                var newquery = {
                    ID: query.CodeID, DisplayName: query.CodeID, Value: query.CodeID, DataType: datatype, Entity: "", Direction: "", IsPrivate: "", Type: "", KeyNo: "", CodeID: ""
                };
                lst.push(newquery);
            }
        }
        return lst;
    };
}]);

app.service('$getQueryparam', ["$rootScope", "$filter", "$EntityIntellisenseFactory", function ($rootScope, $filter, $EntityIntellisenseFactory) {
    // will give a string like "@param_id=person;@param_id2=org" with input as queryID;
    this.get = function (queryId) {
        var queryparam = "";
        var temp = queryId.split('.');
        if (temp.length > 1) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var list = entityIntellisenseList;
            var queries = $filter('filter')(entityIntellisenseList, { ID: temp[0] }, true);
            if (queries.length == 1) {
                var mainquery = $filter('filter')(queries[0].Queries, { ID: temp[1] }, true);
                // valid query ID
                if (mainquery.length == 1) {
                    var Parameters = mainquery[0].Parameters;
                    // selected query has parameters
                    if (Parameters.length > 0) {
                        var queryParametersDisplay = [];
                        function iterator(value) {
                            this.push(value.ID + '=');
                        }
                        angular.forEach(Parameters, iterator, queryParametersDisplay);
                        queryparam = queryParametersDisplay.join(";") + ";";
                    }
                }
            }
        }
        return queryparam;
    };
    // will return an array for all ids present in the node;
    this.getMapVariableIds = function (ele) {
        var queryParametersValues = [];
        function iteratorqueryParametersValues(value) {
            if (value.dictAttributes && value.dictAttributes.id) {
                this.push(value.dictAttributes.id);
            }
        }
        angular.forEach(ele, iteratorqueryParametersValues, queryParametersValues);
        return queryParametersValues;
    };
    // will return an array of object as {id , value} pair for parameters
    this.getQueryparamfromString = function (mapObj, paramName, splitChar) {
        var queryParameters = [];
        if (mapObj[paramName]) {
            var temp = mapObj[paramName].split(splitChar);
            function iteratorqueryParameters(value) {
                if (value) {
                    var temp1 = value.split("=");
                    this.push({ ID: temp1[0], Value: temp1[1] });
                }
            }
            angular.forEach(temp, iteratorqueryParameters, queryParameters);
        }
        return queryParameters;
    };
    this.getQueryparamfromObjArray = function (arr, idpropname, valuepropname) {
        if (arr && idpropname && valuepropname) {
            var queryParameters = [];
            for (var index = 0; index < arr.length; index++) {
                var obj = { ID: getPropertyValue(arr[index], idpropname), Value: getPropertyValue(arr[index], valuepropname) };
                queryParameters.push(obj);
            }
            return queryParameters;
        }
    };
}]);
app.controller("SearchIDDescriptionController", ["$scope", "$rootScope", "$timeout", function ($scope, $rootScope, $timeout) {

    //#region Init Methods
    $scope.Search = { ID: "", Description: "" };
    $scope.Init = function () {
        $scope.SearchResultCollection = [];
    };
    //#endregion 

    //#region Common Methods
    $scope.PopulateSearchResult = function () {
        $scope.SearchResultCollection = [];
        function iterator(obj) {
            var objSearchResult = {};
            if ($scope.SearchResultCollection.length < 100) {
                if ($scope.strCode == "Codegroups") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.CodeID && obj.CodeID.contains($scope.Search.ID) && obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.CodeID && obj.CodeID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else if ($scope.strCode == "Codeiddescription") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.CodeValue && obj.CodeValue.toLowerCase().contains($scope.Search.ID.toLowerCase()) && obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.CodeValue && obj.CodeValue.toLowerCase().contains($scope.Search.ID.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else if ($scope.strCode == "Resources") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.ResourceID && obj.ResourceID.contains($scope.Search.ID) && obj.ResourceDescription && obj.ResourceDescription.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.ResourceID && obj.ResourceID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.ResourceDescription && obj.ResourceDescription.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.MessageID && obj.MessageID.contains($scope.Search.ID) && obj.DisplayMessage && obj.DisplayMessage.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.MessageID && obj.MessageID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.DisplayMessage && obj.DisplayMessage.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
            }
        }

        if ($scope.strCode == "Codegroups") {
            angular.forEach($scope.codegrouplist, iterator);
        }
        else if ($scope.strCode == "Codeiddescription") {
            angular.forEach($scope.codevalueslist, iterator);
        }
        else if ($scope.strCode == "Resources") {
            angular.forEach($scope.resourcelist, iterator);
        }
        else if ($scope.strCode == "Messages") {
            angular.forEach($scope.lstMessages, iterator);
        }
    };
    //#endregion

    //#region Common Events
    $scope.SelectSearchObject = function (obj) {
        $scope.SelectedSearchResult = obj;
    };

    $scope.onOkClick = function () {
        $scope.$emit("onOKClick", $scope.SelectedSearchResult);
        $timeout(function () {
            if ($scope.validateResource) $scope.validateResource(); // validate resource
            if ($scope.validateMessageID) $scope.validateMessageID(); // validate message id
        });
        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        //ngDialog.close($scope.dialog.id);
        $scope.SearchIDDescrDialog.close();
    };
    //#endregion

    $scope.Init();
}]);


app.controller("FindAdvanceFilesController", ['$scope', '$rootScope', 'hubcontext', '$resourceFactory', function ($scope, $rootScope, hubcontext, $resourceFactory) {

    $scope.Init = function () {
        $scope.AdvanceSearch = {};      
        $scope.AdvanceSearch.filetypes = $resourceFactory.getFileTypeList();
        $scope.AdvanceSearch.searchFileType = "All";
        $scope.AdvanceSearch.PageIndex = 0;
        $scope.totalDisplayed = 20;
    };

    $scope.searchTrigger = function () {
        $scope.ngpausescroll = true;
        $scope.before = performance.now();
        $rootScope.IsLoading = true;
        hubcontext.hubSearch.server.getFileFindAdvance($scope.getSearchObj());
    };
    $scope.searchFileSelect = function (file) {
        $scope.FindDialog.close();
        $rootScope.openFile(file.FileInfo);
    };
    $scope.resetFind = function () {
        $scope.AdvanceSearch.PageIndex = 0;
    };
    $("#findFilesDiv").scroll(function () {
        if ($scope.totalDisplayed >= $scope.AdvanceSearch.findSearchList.length && !$scope.ngpausescroll) {
            $scope.AdvanceSearch.PageIndex = $scope.AdvanceSearch.PageIndex + 1;
            $scope.searchTrigger();
        }
        else if ($scope.totalDisplayed < $scope.AdvanceSearch.findSearchList.length && !$scope.ngpausescroll){
            if ($(this)[0].scrollTop + $(this)[0].offsetHeight == $(this)[0].scrollHeight)
                if ($scope.$$phase) {
                    $scope.$evalAsync(function () {
                        $scope.totalDisplayed += 20;
                    });
                } else {
                    $scope.$apply(function () {
                        $scope.totalDisplayed += 20;
                    });
                }                         
        }
    });
    $scope.getSearchObj = function () {
        return {
            SearchText: $scope.AdvanceSearch.searchText,
            MatchCase: $scope.AdvanceSearch.matchCase,
            MatchWholeWord: $scope.AdvanceSearch.matchWholeWord,
            SearchPattern: $scope.AdvanceSearch.searchFileType,
            PageIndex: $scope.AdvanceSearch.PageIndex
        };
    };
    $scope.setFileFindAdvance = function (findObj, data) {
        $scope.ngpausescroll = false;
        //console.log("search text : " + $scope.AdvanceSearch.searchText + " with match case : " + ($scope.AdvanceSearch.matchCase ? true : false) + " with matchwholeword : " + ($scope.AdvanceSearch.matchWholeWord ? true : false) + " took => " + (performance.now() - $scope.before) / 1000);
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            $scope.AdvanceSearch.CountFilesParsed = findObj.CountFilesParsed;
            if (findObj.PageIndex == 1) {
                $scope.totalDisplayed = 20;
                if (data != null) $scope.AdvanceSearch.findSearchList = data;
            }
            else {
                if (data != null) $scope.AdvanceSearch.findSearchList = $scope.AdvanceSearch.findSearchList.concat(data);
            }
            // if result is less than the item displayed and all files have not been traversed yet
            if ($scope.AdvanceSearch.findSearchList.length <= $scope.totalDisplayed && findObj.ScrollLoadEnable) {
                $scope.AdvanceSearch.PageIndex = $scope.AdvanceSearch.PageIndex + 1;
                $scope.searchTrigger();
            }
        });
    };
    $scope.Init();
}]);


app.factory('$FormatQueryFactory', ['hubcontext', '$http', function (hubcontext, $http) {
    var reservewords;
    var item = { sqlReserveWords: reservewords };
    if (hubcontext.hubMain) {
        hubcontext.hubMain.client.setsqlserverreservedwords = function (data) {
            var arrReserveWords = JSON.parse(data);
            item.sqlReserveWords = arrReserveWords;
        };
    }
    return {
        getSqlReserveWords: function () {
            return item.sqlReserveWords;
        },
        formatQuery: function (query) {
            if (query) {
                return $.connection.hubEntityModel.server.formatQuery(query);
            }
        },
        createQueryWithNoLock: function (query) {
            if (query) {
                return $.connection.hubEntityModel.server.createQueryWithNoLock(query);
            }
        }
    };
}]);


app.service('$Chart', ["$rootScope", function ($rootScope) {
    var p = ["#219ae7", "#294ec7", "#5E78CD", "#30437E", "#763555", "#421D2F", "#662A48", "#CD6E1F", "#e77b21", "#EE9F5D", "#f7caa5", "#e0ab7f", "#ef6000", "#1FA28E", "#25bba4", "#34E0C6", "#366657", "#6d2d80", "#6A5870", "#004C63", "#136B85", "#149398", "#107074", "#149398", "#107074"];
    var exampleNodes = [];
    var exampleLinks = [];
    // draws bar chart on giving input data, wrapper id;
    this.drawBarChart = function (dataset, wrapperid, propname, propvalue) {
        var chartWrapper = document.getElementById(wrapperid),
                 chartWrapperWidth = chartWrapper.clientWidth,
                 chartWrapperHeight = chartWrapper.clientHeight,
                 chartWrapperPaddingleft = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-left').replace("px", ""))),
                 chartWrapperPaddingRight = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-right').replace("px", ""))),
                 chartWrapperPaddingTop = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-top').replace("px", ""))),
                 chartWrapperPaddingBottom = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-bottom').replace("px", ""))),
                 totalWidth = chartWrapperWidth - chartWrapperPaddingleft - chartWrapperPaddingRight,
                 totalHeight = chartWrapperHeight - chartWrapperPaddingTop - chartWrapperPaddingBottom;

        var svg = d3.select(chartWrapper).append("svg"),
               margin = {
                   top: 20,
                   right: 20,
                   bottom: 35,
                   left: 40
               },
               width = totalWidth - margin.left - margin.right,
               height = totalHeight - margin.top - margin.bottom;

        svg.attr("width", totalWidth).attr("height", totalHeight);

        var xScale = d3.scale.ordinal()
            .domain(dataset.map(function (d) { return d[propname]; }))
            .rangeRoundBands([0, width], .1);

        var yScale = d3.scale.linear()
            .domain([0, d3.max(dataset, function (d) { return d[propvalue]; })])
            .range([height, 0]);

        var xAxis = d3.svg.axis()
            .scale(xScale)
            .orient("bottom")
            .innerTickSize(-height)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxis = d3.svg.axis()
            .scale(yScale)
            .orient("left")
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        var svg = d3.select("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis);

        // for bar columns
        svg.selectAll(".bar")
            .data(dataset)
            .enter().append("rect")
            .attr("class", "bar")
            .attr("x", function (d) {
                return xScale(d[propname]);
            })
            .attr("y", function (d) {
                return yScale(d[propvalue]);
            })
            .attr("width", xScale.rangeBand())
            .attr("height", function (d) {
                return height - yScale(d[propvalue]);
            })
            .attr("fill", "#dab45f");

        svg.selectAll(".x .tick text")
        .attr("transform", "translate(0,2)rotate(-12)");

    };
    this.drawStackedBarChart = function (dataset, wrapperid, propname, propvalue) {
        if ($('#' + wrapperid)[0])
        {
            $('#' + wrapperid)[0].innerHTML = "";
            var types = ["Entity", "LogicalRule", "DecisionTable", "ExcelMatrix", "ExcelScenario", "ObjectScenario", "ParameterScenario", "Lookup", "Maintenance", "Wizard", "UserControl", "Tooltip", "Prototype", "FormLinkLookup", "FormLinkMaintenance", "FormLinkWizard", "Report", "InboundFile", "OutboundFile", "Correspondence", "PDFCorrespondence", "BPMN", "BPMTemplate", "WorkflowMap"];
            var typecount = [];
            var maindata = dataset;
            var chartWrapper = document.getElementById(wrapperid),
                chartWrapperWidth = chartWrapper.clientWidth,
                chartWrapperHeight = chartWrapper.clientHeight,
                chartWrapperPaddingleft = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-left').replace("px", ""))),
                chartWrapperPaddingRight = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-right').replace("px", ""))),
                chartWrapperPaddingTop = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-top').replace("px", ""))),
                chartWrapperPaddingBottom = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-bottom').replace("px", ""))),
                totalWidth = chartWrapperWidth - chartWrapperPaddingleft - chartWrapperPaddingRight,
                totalHeight = chartWrapperHeight - chartWrapperPaddingTop - chartWrapperPaddingBottom;
            var margin = {
                top: 20,
                right: 50,
                bottom: 35,
                left: 20
            },
                width = totalWidth - margin.left - margin.right,
                height = totalHeight - margin.top - margin.bottom;
            var barWidth = 35;
            var border = 0.75;
            var bordercolor = 'grey';

            var x = d3.scale.ordinal().rangeRoundBands([0, width], .1);
            var y = d3.scale.linear().range([height, 0]);
            var xAxis = d3.svg.axis().scale(x).orient("bottom").outerTickSize(0)
                .tickPadding(10);
            var yAxis = d3.svg.axis().scale(y).orient("left").innerTickSize(-width)
                .outerTickSize(0)
                .tickPadding(10);

            var tip = d3.tip()
                .attr('class', 'd3-tip')
                .offset([-10, 0])
                .html(function (d) {
                    return "<strong>" + d.z + " :</strong> <span>" + d.y + "</span>";
                });

            var svg = d3.select(chartWrapper).append("svg").attr("width", totalWidth).attr("height", totalHeight)
                .append("g").attr("transform", "translate(" + margin.right + "," + margin.top + ")").call(tip);

            // actual data is prepared here
            var layers = d3.layout.stack()(types.map(function (c) {
                return maindata.map(function (d) {
                    return {
                        x: d[propname],
                        y: d[propvalue][c],
                        z: c
                    };
                });
            }));

            x.domain(layers[0].map(function (d) {
                return d.x;
            }));
            y.domain([0, d3.max(layers[layers.length - 1], function (d) {
                return d.y0 + d.y;
            })]).nice();

            // draw x-axis
            svg.append("g")
                .attr("class", "axis axis--x")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis);
            // draw y-axis , add a text "count" beside it
            svg.append("g")
                .attr("class", "axis axis--y")
                .call(yAxis)
                .append("text")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", ".71em")
                .style("text-anchor", "end")
                .text("Count");
            // draw bars 
            var layer = svg.selectAll(".layer")
                .data(layers)
                .enter().append("g")
                .attr("class", "layer")
                .style("fill", function (d, i) {
                    return p[i];
                });

            layer.selectAll("rect")
                .data(function (d) {
                    return d;
                })

                .enter().append("rect")
                .attr("x", function (d, i) {
                    return x(d.x) + (x.rangeBand() - barWidth) / 2;
                })
                .attr("y", function (d) {
                    return y(d.y + d.y0);
                })
                .attr("height", function (d) {
                    return y(d.y0) - y(d.y + d.y0);
                })
                .attr("width", Math.min(x.rangeBand(), barWidth))
                //.style("filter", "url(#drop-shadow)")
                .on('mouseover', tip.show)
                .on('mouseout', tip.hide);
        }
    };
    this.drawPieChart = function (dataset, wrapperid, propname, propvalue) {
        if ($('#' + wrapperid)[0])
        {
            $('#' + wrapperid)[0].innerHTML = "";
            var total = d3.sum(dataset, function (d) { return d3.sum(d3.values(d)); });
            var chartWrapper = document.getElementById("TfsChart"), chartWrapperWidth = chartWrapper.clientWidth, chartWrapperHeight = chartWrapper.clientHeight,
                inner = 70;
            var width = chartWrapperWidth, height = chartWrapperHeight, r = Math.min(width, height) / 2;
            var color = d3.scale.ordinal().range(["#a79773", "#dab45f", "#ffdf96"]);
            var vis = d3.select("#TfsChart").append("svg:svg")
                .data([dataset])
                .attr("width", chartWrapperWidth)
                .attr("height", chartWrapperHeight)
                .append("svg:g")
                .attr("transform", "translate(" + (r * 1 + ((Math.max(width, height) - r) / 4)) + "," + r * 1 + ")");
            var textTop = vis.append("text")
                .attr("dy", ".35em")
                .style("text-anchor", "middle")
                .attr("class", "textTop")
                .text("TOTAL")
                .attr("y", -10),
                textBottom = vis.append("text")
                    .attr("dy", ".35em")
                    .style("text-anchor", "middle")
                    .attr("class", "textBottom")
                    .text(total)
                    .attr("y", 10);

            var arc = d3.svg.arc().innerRadius(inner + 5).outerRadius(r - 10);
            var arcOver = d3.svg.arc().innerRadius(inner + 10).outerRadius(r - 5);
            var pie = d3.layout.pie().value(function (d) { return d[propvalue]; });
            var arcs = vis.selectAll("g.slice")
                .data(pie)
                .enter()
                .append("svg:g")
                .attr("class", "slice")
                .on("mouseover", function (d) {
                    d3.select(this).select("path").transition()
                        .duration(200)
                        .attr("d", arcOver);
                    textTop.text(d3.select(this).datum().data.Name)
                        .attr("y", -10);
                    textBottom.text(d3.select(this).datum().data.Count)
                        .attr("y", 10);
                })
                .on("mouseout", function (d) {
                    d3.select(this).select("path").transition()
                        .duration(100)
                        .attr("d", arc);
                    textTop.text("TOTAL")
                        .attr("y", -10);
                    textBottom.text(total);
                });

            arcs.append("svg:path")
                .attr("fill", function (d, i) { return color(i); })
                .attr("d", arc);

        //var legend = d3.select("#TfsChart").append("svg")
        //              .attr("class", "legend")
        //              .attr("width", r)
        //              .attr("height", r * 2)
        //              .selectAll("g")
        //              .data(dataset)
        //              .enter().append("g")
        //              .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

        //                legend.append("rect")
        //                    .attr("width", 18)
        //                    .attr("height", 18)
        //                    .style("fill", function (d, i) { return color(i); });

        //                legend.append("text")
        //                    .attr("x", 24)
        //                    .attr("y", 9)
        //                    .attr("dy", ".35em")
        //                    .text(function (d) { return d[propname]; });
        }
    };

    this.drawErDiagram = function (strFileName) {
      
        var svg, tooltip, biHiSankey, path, defs, colorScale, highlightColorScale, isTransitioning;

        var OPACITY = {
            NODE_DEFAULT: 0.9,
            NODE_FADED: 0.1,
            NODE_HIGHLIGHT: 0.8,
            LINK_DEFAULT: 0.6,
            LINK_FADED: 0.05,
            LINK_HIGHLIGHT: 0.9
        },
          TYPES = ["Entity", ],
          TYPE_COLORS = ["#77b3d4"],
          TYPE_HIGHLIGHT_COLORS = ["#66c2a5", "#fc8d62", "#8da0cb", "#e78ac3", "#a6d854", "#ffd92f", "#e5c494"],
          LINK_COLOR = "#FFFFFF",
          INFLOW_COLOR = "#2E86D1",
          OUTFLOW_COLOR = "#D63028",
          NODE_WIDTH = 160,
          COLLAPSER = {
              RADIUS: NODE_WIDTH / 2,
              SPACING: 2
          },
          OUTER_MARGIN = 50,
          MARGIN = {
              TOP: (OUTER_MARGIN),
              RIGHT: OUTER_MARGIN,
              BOTTOM: OUTER_MARGIN,
              LEFT: OUTER_MARGIN
          },
          TRANSITION_DURATION = 400,
          HEIGHT = 700 - MARGIN.TOP - MARGIN.BOTTOM,
          WIDTH = 1400 - MARGIN.LEFT - MARGIN.RIGHT,
          LAYOUT_INTERATIONS = 32,
          REFRESH_INTERVAL = 7000;
        var calulateHieght = 0;
        for (var i = 0; i < exampleNodes.length; i++) {
            if (exampleNodes[i].attributes.length > 0) {
                calulateHieght = calulateHieght + (exampleNodes[i].attributes.length * 25);
            }
            else {
                calulateHieght = calulateHieght + 25;
            }
        }
        if (calulateHieght > HEIGHT) {
            HEIGHT = calulateHieght;
        }
        var formatNumber = function (d) {
            var numberFormat = d3.format(",.0f"); // zero decimal places
            return "£" + numberFormat(d);
        },

        menu = contextMenu().items('Navigate to Entity');

        formatFlow = function (d) {
            var flowFormat = d3.format(",.0f"); // zero decimal places with sign
            return "£" + flowFormat(Math.abs(d)) + (d < 0 ? " CR" : " DR");
        },

        // Used when temporarily disabling user interractions to allow animations to complete
        disableUserInterractions = function (time) {
            isTransitioning = true;
            setTimeout(function () {
                isTransitioning = false;
            }, time);
        },

        hideTooltip = function () {
            return tooltip.transition()
              .duration(TRANSITION_DURATION)
              .style("opacity", 0);
        },

        showTooltip = function () {
            return tooltip
              .style("left", d3.event.pageX + "px")
              .style("top", d3.event.pageY + 15 + "px")
              .transition()
                .duration(TRANSITION_DURATION)
                .style("opacity", 1);
        };

        colorScale = d3.scale.ordinal().domain(TYPES).range(TYPE_COLORS),
        highlightColorScale = d3.scale.ordinal().domain(TYPES).range(TYPE_HIGHLIGHT_COLORS),

        svg = d3.select("#" + strFileName + " [wrapper-erdiagram]").append("svg")
                .attr("width", WIDTH + MARGIN.LEFT + MARGIN.RIGHT)
                .attr("height", HEIGHT + MARGIN.TOP + MARGIN.BOTTOM)
              .append("g")
                .attr("transform", "translate(" + MARGIN.LEFT + "," + MARGIN.TOP + ")");

        svg.append("g").attr("id", "links");
        svg.append("g").attr("id", "nodes");
        svg.append("g").attr("id", "collapsers");

        tooltip = d3.select("#" + strFileName + " [wrapper-erdiagram]").append("div").attr("id", "tooltip");

        tooltip.style("opacity", 0)
            .append("p")
              .attr("class", "value");

        biHiSankey = d3.biHiSankey();

        // Set the biHiSankey diagram properties
        biHiSankey
          .nodeWidth(NODE_WIDTH)
          .nodeSpacing(1)
          .linkSpacing(4)
          .arrowheadScaleFactor(0.5) // Specifies that 0.5 of the link's stroke WIDTH should be allowed for the marker at the end of the link.
          .size([WIDTH, HEIGHT]);

        path = biHiSankey.link().curvature(0.45);

        defs = svg.append("defs");

        defs.append("marker")
          .style("fill", LINK_COLOR)
          .attr("id", strFileName + "arrowHead")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
                  .style("fill", INFLOW_COLOR)
                  .attr("id", strFileName + "arrowHeadOneToOne")
                  .attr("viewBox", "0 0 10 10")
                  .attr("refX", "5")
                  .attr("refY", "5")
                  .attr("markerUnits", "strokeWidth")
                  .attr("markerWidth", "3")
                  .attr("markerHeight", "3")
                  .attr("orient", "auto")
                  .append("path")
                    .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
                         .style("fill", OUTFLOW_COLOR)
                         .attr("id", strFileName + "arrowHeadOneToMany")
                         .attr("viewBox", "0 0 10 10")
                         .attr("refX", "5")
                         .attr("refY", "5")
                         .attr("markerUnits", "strokeWidth")
                         .attr("markerWidth", "3")
                         .attr("markerHeight", "3")
                         .attr("orient", "auto")
                         .append("path")
                           .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
          .style("fill", OUTFLOW_COLOR)
          .attr("id", strFileName + "arrowHeadInflow")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
          .style("fill", INFLOW_COLOR)
          .attr("id", strFileName + "arrowHeadOutlow")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        function contextMenu() {
            var height,
                width,
                margin = 0.1, // fraction of width
                items = [],
                rescale = false,
                style = {
                    'rect': {
                        'mouseout': {
                            'fill': 'rgb(244,244,244)',
                            'stroke': 'white',
                            'stroke-width': '1px'
                        },
                        'mouseover': {
                            'fill': 'rgb(200,200,200)'
                        }
                    },
                    'text': {
                        'fill': 'steelblue',
                        'font-size': '13'
                    }
                };

            function menu(x, y, nodedata) {
                d3.select("#" + strFileName + " .context-menu").remove();
                scaleItems();

                // Draw the menu
                d3.select("#" + strFileName + ' svg')
                    .append('g').attr('class', 'context-menu')
                    .selectAll('tmp')
                    .data(items).enter()
                    .append('g').attr('class', 'menu-entry')
                    .style({ 'cursor': 'pointer' })
                    .on('mouseover', function () {
                        disableUserInterractions(500);
                        d3.select(this).select('rect').style(style.rect.mouseover)
                    })
                    .on('mouseout', function () {
                        disableUserInterractions(500);
                        d3.select(this).select('rect').style(style.rect.mouseout)
                    })
                     .on('click', function () {
                         $.connection.hubMain.server.navigateToFile(nodedata.id, "").done(function (objfile) {
                             $rootScope.openFile(objfile, false);
                         });
                     });

                d3.selectAll("#" + strFileName + ' svg .menu-entry')
                    .append('rect')
                    .attr('x', x)
                    .attr('y', function (d, i) { return y + (i * height); })
                    .attr('width', width)
                    .attr('height', height)
                    .style(style.rect.mouseout);

                d3.selectAll("#" + strFileName + ' svg .menu-entry')
                    .append('text')
                    .text(function (d) { return d; })
                    .attr('x', x)
                    .attr('y', function (d, i) { return y + (i * height); })
                    .attr('dy', height - margin / 2)
                    .attr('dx', margin)
                    .style(style.text);

                // Other interactions
                d3.select('body')
                    .on('click', function () {
                        d3.select("[wrapper-erdiagram] .context-menu").remove();
                    });

            }

            menu.items = function (e) {
                if (!arguments.length) return items;
                for (i in arguments) items.push(arguments[i]);
                rescale = true;
                return menu;
            }

            // Automatically set width, height, and margin;
            function scaleItems() {
                if (rescale) {                    
                    d3.select("#" + strFileName + ' svg').selectAll('tmp')
                        .data(items).enter()
                        .append('text')
                        .text(function (d) { return d; })
                        .style(style.text)
                        .attr('x', -1000)
                        .attr('y', -1000)
                        .attr('class', 'tmp');
                    var z = d3.selectAll("#" + strFileName + ' .tmp')[0]
                              .map(function (x) { return x.getBBox(); });
                    width = d3.max(z.map(function (x) { return x.width; }));
                    margin = margin * width;
                    width = width + 2 * margin;
                    height = d3.max(z.map(function (x) { return x.height + margin / 2; }));

                    // cleanup
                    d3.selectAll("#" + strFileName + ' .tmp').remove();
                    rescale = false;
                }
            }

            return menu;
        }

        function update() {
            var link, linkEnter, node, nodeEnter, nodeEnterBox, collapser, collapserEnter;

            function dragmove(node) {
                node.x = Math.max(0, Math.min(WIDTH - node.width, d3.event.x));
                node.y = Math.max(0, Math.min(HEIGHT - node.height, d3.event.y));
                d3.select(this).attr("transform", "translate(" + node.x + "," + node.y + ")");
                biHiSankey.relayout();
                svg.selectAll(".node").selectAll("rect").attr("height", function (d) { return d.height; });
                link.attr("d", path);
            }

            function containChildren(node) {
                node.children.forEach(function (child) {
                    child.state = "contained";
                    child.parent = this;
                    child._parent = null;
                    containChildren(child);
                }, node);
            }

            function expand(node) {
                node.state = "expanded";
                node.children.forEach(function (child) {
                    child.state = "collapsed";
                    child._parent = this;
                    child.parent = null;
                    containChildren(child);
                }, node);
            }

            function collapse(node) {
                node.state = "collapsed";
                containChildren(node);
            }

            function restoreLinksAndNodes() {
                link
                  .style("stroke", function (d, i) {
                      return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                  })
                  .style("marker-end", function (d) {
                      return d.type == "onetoone" ? 'url(#' + strFileName + 'arrowHeadOneToOne)' : 'url(#' + strFileName + 'arrowHeadOneToMany)';
                  })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.LINK_DEFAULT);

                node
                  .selectAll("rect")
                    .style("fill", function (d) {
                        d.color = colorScale(d.type.replace(/ .*/, ""));
                        return d.color;
                    })
                     .style("stroke", "white")
                .style("stroke-WIDTH", "4")
                    .style("fill-opacity", OPACITY.NODE_DEFAULT);

                node.filter(function (n) { return n.state === "collapsed"; })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.NODE_DEFAULT);
            }

            function showHideChildren(node) {
                disableUserInterractions(2 * TRANSITION_DURATION);
                hideTooltip();
                if (node.state === "collapsed") { expand(node); }
                else { collapse(node); }

                biHiSankey.relayout();
                update();
                link.attr("d", path);
                restoreLinksAndNodes();
            }

            function highlightConnected(g) {
                link.filter(function (d) { return d.type === "onetomany"; })
                  .style("marker-end", function () { return 'url(#' + strFileName + 'arrowHeadInflow)'; })
                  .style("stroke", OUTFLOW_COLOR)
                  .style("opacity", OPACITY.LINK_DEFAULT);

                link.filter(function (d) { return d.type === "onetoone"; })
                  .style("marker-end", function () { return 'url(#' + strFileName + 'arrowHeadOutlow)'; })
                  .style("stroke", INFLOW_COLOR)
                  .style("opacity", OPACITY.LINK_DEFAULT);
            }

            function fadeUnconnected(g) {
                link.filter(function (d) { return d.source !== g && d.target !== g; })
                  .style("marker-end", function () {
                      return 'url(#' + strFileName + 'arrowHead)';
                  })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.LINK_FADED);

                node.filter(function (d) {
                    return (d.name === g.name) ? false : !biHiSankey.connected(d, g);
                }).transition()
                  .duration(TRANSITION_DURATION)
                  .style("opacity", OPACITY.NODE_FADED);
            }

            node = svg.select("#nodes").selectAll(".node")
                .data(biHiSankey.collapsedNodes(), function (d) { return d.id; });

            node.transition()
              .duration(TRANSITION_DURATION)
              .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; })
              .style("opacity", OPACITY.NODE_DEFAULT)
              .select("rect")
                .style("fill", function (d) {
                    d.color = colorScale(d.type.replace(/ .*/, ""));
                    return d.color;
                })
                .style("stroke", function (d) { return d3.rgb(colorScale(d.type.replace(/ .*/, ""))).darker(0.1); })
                .style("stroke-WIDTH", "1px")
                .attr("height", function (d) { return d.height; })
                .attr("width", biHiSankey.nodeWidth());

            node.exit()
              .transition()
                .duration(TRANSITION_DURATION)
                .attr("transform", function (d) {
                    var collapsedAncestor, endX, endY;
                    collapsedAncestor = d.ancestors.filter(function (a) {
                        return a.state === "collapsed";
                    })[0];
                    endX = collapsedAncestor ? collapsedAncestor.x : d.x;
                    endY = collapsedAncestor ? collapsedAncestor.y : d.y;
                    return "translate(" + endX + "," + endY + ")";
                })
                .remove();

            nodeEnter = node.enter().append("g").attr("class", "node");

            nodeEnter
              .attr("transform", function (d) {
                  var startX = d._parent ? d._parent.x : d.x,
                      startY = d._parent ? d._parent.y : d.y;
                  return "translate(" + startX + "," + startY + ")";
              })
              .style("opacity", 1e-6)
              .transition()
                .duration(TRANSITION_DURATION)
                .style("opacity", OPACITY.NODE_DEFAULT)
                .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });

            nodeEnter.append("text").attr("class", "node-name");

            nodeEnter.append("rect");

            nodeEnter.selectAll(".attributes")
                  .data(function (d) {
                      return d.attributes;
                  })
                  .enter().append("text")
                  .attr("x", 6)
                  .attr("y", function (d, i) { return i * 20 + 15 })
                  .attr("dy", ".35em")
                 .text(function (d, i) { return d })
                  .attr("class", "attributes");


            nodeEnter.select("rect")
            .style("fill", function (d) {
                d.color = colorScale(d.type.replace(/ .*/, ""));
                return d.color;
            })
              .style("stroke", "white")
              .style("stroke-WIDTH", "4")
              .attr("stroke-linecap", "round")
              .attr("height", function (d) { return d.height; })
              .attr("width", function (d) {
                  var rectMaxWidth = CalculateRectangleWidth(d);
                  if (rectMaxWidth > 160) {
                      rectMaxWidth = rectMaxWidth + 15;
                      d.width = rectMaxWidth;
                  }
                  return rectMaxWidth;
                  //if (this.parentNode && this.parentNode.getBBox().width>0) {
                  //    return this.parentNode.getBBox().width + 20;
                  //}
                  //else {
                  //    return biHiSankey.nodeWidth();
                  //}
              })
                .on('contextmenu', function (g) {
                    d3.event.preventDefault();
                    disableUserInterractions(500);
                    //console.log("clientX: " + d3.event.clientX + " - clientY: " + d3.event.clientY);
                    //console.log("pageX: " + d3.event.pageX + " - pageY: " + d3.event.pageY);
                    //console.log("offesetX: " + d3.event.offsetX + " - offesetY: " + d3.event.offsetY);
                    //console.log("mousex: " + d3.mouse(this)[0] + " - mouseY: " + d3.mouse(this)[1]);
                    //console.log("screenX: " + d3.event.screenX + " - screenY: " + d3.event.screenY);
                    menu(g.x + d3.mouse(this)[0] + 35, g.y + d3.mouse(this)[1] + 35, g);
                });

            node.on("mouseenter", function (g) {
                //console.log("mouseenter called");
                if (!isTransitioning && $rootScope.currentopenfile.file.FileName == strFileName) {
                    restoreLinksAndNodes();
                    highlightConnected(g);
                    fadeUnconnected(g);

                    d3.select(this).select("rect")
                      .style("fill", function (d) {
                          d.color = INFLOW_COLOR;
                          // d.color = d.netFlow > 0 ? INFLOW_COLOR : OUTFLOW_COLOR;
                          return d.color;
                      })
                       .style("stroke", "white")
                      .style("stroke-WIDTH", "4")
                      .style("fill-opacity", OPACITY.LINK_DEFAULT);

                    tooltip
                      .style("left", g.x + MARGIN.LEFT + "px")
                      .style("top", g.y + g.height + MARGIN.TOP + 15 + "px")
                      .transition()
                        .duration(TRANSITION_DURATION)
                        .style("opacity", 1).select(".value")
                        .text(function () {
                            var additionalInstructions = g.children.length ? "\n(Double click to expand)" : "";
                            return g.name;
                        });
                }
            });

            node.on("mouseleave", function () {
                //console.log("mouseleave called");
                if (!isTransitioning && $rootScope.currentopenfile.file.FileName == strFileName) {
                    hideTooltip();
                    restoreLinksAndNodes();
                }
            });

            node.filter(function (d) { return d.children.length; })
              .on("dblclick", showHideChildren);

            // allow nodes to be dragged to new positions
            node.call(d3.behavior.drag()
              .origin(function (d) { return d; })
              .on("dragstart", function () {
                  d3.select("#" + strFileName + " .context-menu").remove();
                  this.parentNode.appendChild(this);
              })
              .on("drag", dragmove));

            // add in the text for the nodes
            node.filter(function (d) {
                //return d.value !== 0;
                return true;
            })
              .select("text.node-name")
                .attr("x", -6)
               .attr("y", function (d) { return d.height / 2; })
                .attr("dy", ".35em")
                .attr("text-anchor", "end")
                .attr("transform", null)
                 .text(function (d) { return d.name; })
                .filter(function (d) { return d.x < WIDTH / 2; })
                .attr("x", function (d) {

                    var rectMaxWidth = CalculateRectangleWidth(d);
                    if (rectMaxWidth > 160) {
                        rectMaxWidth = rectMaxWidth + 20;
                        d.width = rectMaxWidth - 5;
                    }
                    else {
                        rectMaxWidth = rectMaxWidth + 6;
                    }
                    return rectMaxWidth;
                    //if (this.parentNode && d3.select(this.parentNode).select("rect")[0][0].getBBox().width>0) {
                    //    d.width = d3.select(this.parentNode).select("rect")[0][0].getBBox().width;
                    //    return d3.select(this.parentNode).select("rect")[0][0].getBBox().width + 6;
                    //}
                    //else {
                    //    return 6 + biHiSankey.nodeWidth();
                    //}
                })
                .attr("text-anchor", "start");

            link = svg.select("#links").selectAll("path.link")
              .data(biHiSankey.visibleLinks(), function (d) {
                  return d.id;
              });

            link.transition()
              .duration(TRANSITION_DURATION)
              .attr("d", path)
              .style("opacity", OPACITY.LINK_DEFAULT);

            link.exit().remove();

            linkEnter = link.enter().append("path")
              .attr("class", "link")
              .style("fill", "none");

            linkEnter.on('mouseenter', function (d) {
                if (!isTransitioning) {
                    showTooltip().select(".value").text(function () {
                        //if (d.direction > 0) {
                        return d.source.name + " → " + d.target.name + "\n" + d.type;
                        //}
                        //return d.target.name + " ← " + d.source.name + "\n" + d.type;
                    });

                    d3.select(this)
                       .style("stroke", function (d, i) {
                           return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                       })
                      .transition()
                        .duration(TRANSITION_DURATION / 2)
                        .style("opacity", OPACITY.LINK_HIGHLIGHT);
                }
            });

            linkEnter.on('mouseleave', function () {
                if (!isTransitioning) {
                    hideTooltip();

                    d3.select(this)
                       .style("stroke", function (d, i) {
                           return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                       })
                      .transition()
                        .duration(TRANSITION_DURATION / 2)
                        .style("opacity", OPACITY.LINK_DEFAULT);
                }
            });

            linkEnter.sort(function (a, b) { return b.thickness - a.thickness; })
              .classed("leftToRight", function (d) {
                  return d.direction > 0;
              })
              .classed("rightToLeft", function (d) {
                  return d.direction < 0;
              })
              .style("marker-end", function (d) {
                  //return 'url(#arrowHead)';
                  return d.type == "onetoone" ? 'url(#' + strFileName + 'arrowHeadOneToOne)' : 'url(#' + strFileName + 'arrowHeadOneToMany)';
              })
              .style("stroke", function (d, i) {
                  return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
              })
              .style("opacity", 0)
              .transition()
                .delay(TRANSITION_DURATION)
                .duration(TRANSITION_DURATION)
                .attr("d", path)
                .style("stroke-WIDTH", function (d) { return Math.max(1, d.thickness); })
                .style("opacity", OPACITY.LINK_DEFAULT);

            collapser = svg.select("#collapsers").selectAll(".collapser")
              .data(biHiSankey.expandedNodes(), function (d) { return d.id; });

            collapserEnter = collapser.enter().append("g").attr("class", "collapser");

            collapserEnter.append("circle")
              .attr("r", COLLAPSER.RADIUS)
              .style("fill", function (d) {
                  d.color = colorScale(d.type.replace(/ .*/, ""));
                  return d.color;
              });

            collapserEnter
              .style("opacity", OPACITY.NODE_DEFAULT)
              .attr("transform", function (d) {
                  return "translate(" + (d.x + d.width / 2) + "," + (d.y + COLLAPSER.RADIUS) + ")";
              });

            collapserEnter.on("dblclick", showHideChildren);

            collapser.select("circle")
              .attr("r", COLLAPSER.RADIUS);

            collapser.transition()
              .delay(TRANSITION_DURATION)
              .duration(TRANSITION_DURATION)
              .attr("transform", function (d, i) {
                  return "translate("
                    + (COLLAPSER.RADIUS + i * 2 * (COLLAPSER.RADIUS + COLLAPSER.SPACING))
                    + ","
                    + (-COLLAPSER.RADIUS - OUTER_MARGIN)
                    + ")";
              });

            collapser.on("mouseenter", function (g) {
                if (!isTransitioning) {
                    showTooltip().select(".value")
                      .text(function () {
                          return g.name + "\n(Double click to collapse)";
                      });

                    var highlightColor = highlightColorScale(g.type.replace(/ .*/, ""));

                    d3.select(this)
                      .style("opacity", OPACITY.NODE_HIGHLIGHT)
                      .select("circle")
                        .style("fill", highlightColor);

                    node.filter(function (d) {
                        return d.ancestors.indexOf(g) >= 0;
                    }).style("opacity", OPACITY.NODE_HIGHLIGHT)
                      .select("rect")
                        .style("fill", highlightColor);
                }
            });

            collapser.on("mouseleave", function (g) {
                if (!isTransitioning) {
                    hideTooltip();
                    d3.select(this)
                      .style("opacity", OPACITY.NODE_DEFAULT)
                      .select("circle")
                        .style("fill", function (d) { return d.color; });

                    node.filter(function (d) {
                        return d.ancestors.indexOf(g) >= 0;
                    }).style("opacity", OPACITY.NODE_DEFAULT)
                      .select("rect")
                        .style("fill", function (d) { return d.color; });
                }
            });

            collapser.exit().remove();


        }

        //var exampleNodes = [{ "type": "Entity", "id": "entFile", "name": "entFile", "attributes": ["lstRecordLayout"] }, { "type": "Entity", "id": "entutlRecordLayout", "name": "entutlRecordLayout", "attributes": [] }, { "type": "Entity", "id": "entFileHdr", "name": "entFileHdr", "attributes": ["objFile", "lstStatusSummary", "lstFileHdrError", "objtest"] }, { "type": "Entity", "id": "entStatusSummary", "name": "entStatusSummary", "attributes": ["objFileHdr"] }, { "type": "Entity", "id": "entFileHdrError", "name": "entFileHdrError", "attributes": [] }];

        //var exampleLinks = [{ "source": "entFile", "target": "entutlRecordLayout", "value": 5, "type": "onetomany" }, { "source": "entFileHdr", "target": "entFile", "value": 5, "type": "onetoone" }, { "source": "entFileHdr", "target": "entutlRecordLayout", "value": 5, "type": "onetoone" }, { "source": "entFileHdr", "target": "entStatusSummary", "value": 5, "type": "onetomany" }, { "source": "entFileHdr", "target": "entFileHdrError", "value": 5, "type": "onetomany" }];



        function CalculateRectangleWidth(d) {
            var rectMaxWidth = biHiSankey.nodeWidth();
            for (var i = 0; i < d.attributes.length; i++) {
                if ((d.attributes[i].length * 6) > rectMaxWidth) {
                    rectMaxWidth = (d.attributes[i].length * 6);
                }
            }
            return rectMaxWidth;
        }

        biHiSankey
          .nodes(exampleNodes)
          .links(exampleLinks)
          .initializeNodes(function (node) {
              node.state = node.parent ? "contained" : "collapsed";
          })
          .layout(LAYOUT_INTERATIONS);

        disableUserInterractions(2 * TRANSITION_DURATION);

        update();
    }
    this.setData = function (aobjExampleNodes, aobjExampleLinks) {
        exampleNodes = aobjExampleNodes;
        exampleLinks = aobjExampleLinks;
    }
}]);
app.factory("$FormValidationService", ["CONSTANTS", "$timeout", "$rootScope", "$EntityIntellisenseFactory", "$Entityintellisenseservice", "$Errors", "$filter", "$GetEntityFieldObjectService", '$ValidationService', '$GetGridEntity', function (CONST, $timeout, $rootScope, $EntityIntellisenseFactory, $Entityintellisenseservice, $Errors, $filter, $GetEntityFieldObjectService, $ValidationService, $GetGridEntity) {
    var lobjFactoryData = {};
    var lobjRequiredData = {
        FormModel: {}, SfxMainTable: {}, validationErrorList: [], iswizard: false, isLookup: false, IsPrototype: false, lstEntity: [], parameterList: [], gridQueryResult: [], lstQueriesName: [], validationData: {}, objEntityExtraData: {}, resourceList: [], queryIDList: [], lstMessages: []
    };

    var createValidationData = function (aobjFormModel, aobjScope) {
        /** form validation **/


        if (aobjFormModel) {
            if (aobjFormModel.dictAttributes.hasOwnProperty('sfwEntity')) {
                $ValidationService.validateEntity(aobjFormModel, undefined);
            }
            if (aobjFormModel.dictAttributes.sfwType.toLowerCase() == "lookup") {
                if (aobjFormModel.dictAttributes.ID.toLowerCase().startsWith("wfp")) {
                    lobjRequiredData.IsPrototype = true;
                }
                else {
                    lobjRequiredData.isLookup = true;
                }
            } else if (aobjFormModel.dictAttributes.sfwType.toLowerCase() == "wizard") {
                lobjRequiredData.iswizard = true;
            }

        }
        if (lobjRequiredData.isLookup) {
            PopulateQueryId();
        }
        let lblnLoadCommonData = true;
        if (lobjRequiredData.validationData.CodeGroup || lobjRequiredData.validationData.Resources || lobjRequiredData.validationData.ActiveForms || lobjRequiredData.validationData.RemoteObject) {
            lblnLoadCommonData = false;
        }
        if (lblnLoadCommonData) {
            $.connection.hubForm.server.getGlobleParameters().done(function (data) {
                if (data) {
                    objGlobleParametersList(data);
                }
            });
        }
        if (aobjFormModel.dictAttributes.sfwEntity && aobjFormModel.errors && !aobjFormModel.errors.invalid_entity) {
            lobjRequiredData.lstEntity.push(aobjFormModel.dictAttributes.sfwEntity);
            lobjRequiredData.lstQueriesName = [];
            traverseModelAndGetEntities(aobjFormModel, aobjFormModel.dictAttributes.sfwEntity, aobjFormModel);
            if (angular.isArray(lobjRequiredData.lstQueriesName) && lobjRequiredData.lstQueriesName.length > 0) {
                //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
                $.connection.hubForm.server.getQueriesResult(lobjRequiredData.lstQueriesName, 'dummy').done(function (data) {
                    if (data) {
                        lobjRequiredData.gridQueryResult = data;
                        if (lblnLoadCommonData) {
                            $.connection.hubMain.server.getFormValidationData().done(function (data) {
                                lobjRequiredData.validationData = data;
                                lobjRequiredData.lstMessages = data["messages"];
                                validateFormCommon(aobjFormModel, aobjScope);

                            }).fail(function () {
                                if (aobjScope && aobjScope.validateFile) {
                                    aobjScope.$evalAsync(function () {
                                        aobjScope.validateFile("Failed");
                                    });
                                }
                            });
                        }
                        else {
                            validateFormCommon(aobjFormModel, aobjScope);
                        }
                    }
                }).fail(function () {
                    if (aobjScope && aobjScope.validateFile) {
                        aobjScope.$evalAsync(function () {
                            aobjScope.validateFile("Failed");
                        });
                    }
                });

            }
            else {
                if (lblnLoadCommonData) {
                    $.connection.hubMain.server.getFormValidationData().done(function (data) {
                        lobjRequiredData.validationData = data;
                        lobjRequiredData.lstMessages = data["messages"];
                        validateFormCommon(aobjFormModel, aobjScope);

                    }).fail(function () {
                        if (aobjScope && aobjScope.validateFile) {
                            aobjScope.$evalAsync(function () {
                                aobjScope.validateFile("Failed");
                            });
                        }
                    });
                }
                else {
                    validateFormCommon(aobjFormModel, aobjScope);
                }
            }
        }
        else {
            if (aobjScope && aobjScope.validateFile) {
                aobjScope.$evalAsync(function () {
                    aobjScope.validateFile("Completed");
                });
            }
        }
    };

    var validateFormCommon = function (aobjFormModel, aobjScope) {
        lobjRequiredData.resourceList = $ValidationService.getListByPropertyName(lobjRequiredData.validationData.Resources, "ResourceID", false);
        if (lobjRequiredData.lstEntity.length > 0) {
            if (lobjRequiredData.validationData.hasOwnProperty(aobjFormModel.dictAttributes.sfwEntity)) {
                createRuledata(lobjRequiredData.validationData[aobjFormModel.dictAttributes.sfwEntity]);
                lobjRequiredData.objEntityExtraData.lstHardErrors = createValidationRuleList(lobjRequiredData.validationData[aobjFormModel.dictAttributes.sfwEntity], false, aobjFormModel.dictAttributes.sfwHardErrorGroup);
            }
        }
        // validate detail window fields
        if (aobjFormModel.dictAttributes.hasOwnProperty('sfwResource') && aobjFormModel.dictAttributes.sfwResource) {
            $ValidationService.checkValidListValue(lobjRequiredData.resourceList, aobjFormModel, aobjFormModel.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, lobjRequiredData.validationErrorList);
        }
        var list = $ValidationService.getListByPropertyName(lobjRequiredData.lstMessages, "MessageID", false);
        if (aobjFormModel.dictAttributes.hasOwnProperty('sfwNewMessageID') && aobjFormModel.dictAttributes.sfwNewMessageID) {
            $ValidationService.checkValidListValue(list, aobjFormModel, aobjFormModel.dictAttributes.sfwNewMessageID, "sfwNewMessageID", "sfwNewMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjRequiredData.validationErrorList);
        }
        if (aobjFormModel.dictAttributes.hasOwnProperty('sfwOpenMessageID') && aobjFormModel.dictAttributes.sfwOpenMessageID) {
            $ValidationService.checkValidListValue(list, aobjFormModel, aobjFormModel.dictAttributes.sfwOpenMessageID, "sfwOpenMessageID", "sfwOpenMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjRequiredData.validationErrorList);
        }
        if (aobjFormModel.dictAttributes.hasOwnProperty("sfwDefaultButtonID") && aobjFormModel.dictAttributes.sfwDefaultButtonID) {
            var lstButtons = [];
            FindControlListByName(lobjRequiredData.SfxMainTable, "sfwButton", lstButtons);
            lstButtons = $ValidationService.getListByPropertyName(lstButtons, "ID", false);
            $ValidationService.checkValidListValue(lstButtons, aobjFormModel, aobjFormModel.dictAttributes.sfwDefaultButtonID, "sfwDefaultButtonID", "sfwDefaultButtonID", CONST.VALIDATION.BUTTON_NOT_EXISTS, lobjRequiredData.validationErrorList);
        }
        //End of details window validation

        validateForm(lobjRequiredData.SfxMainTable, aobjFormModel.dictAttributes.sfwEntity, lobjRequiredData.objEntityExtraData);
        if (aobjScope && aobjScope.validateFile) {
            aobjScope.$evalAsync(function () {
                aobjScope.validateFile("Completed");
            });
        }

    };
    lobjFactoryData.validateFileData = function (aobjFormModel, aobjScope) {

        var fileErrObj;
        if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
            var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: aobjFormModel.dictAttributes.ID });
            if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: aobjFormModel.dictAttributes.ID, errorList: [] });
            fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: aobjFormModel.dictAttributes.ID })[0];
        }
        lobjRequiredData.FormModel = aobjFormModel;
        aobjFormModel.errors = {};
        SetMainTable();
        lobjRequiredData.validationErrorList = fileErrObj.errorList = [];
        lobjRequiredData.lstEntity = [];
        lobjRequiredData.parameterList = [];
        lobjRequiredData.gridQueryResult = [];
        lobjRequiredData.objEntityExtraData = {};
        lobjRequiredData.resourceList = [];
        // createValidationData();
        if (lobjRequiredData.SfxMainTable) {
            iterateModel(lobjRequiredData.SfxMainTable);
            createValidationData(aobjFormModel, aobjScope);
        }
        return lobjRequiredData.validationErrorList;

    };

    var objGlobleParametersList = function (data) {
        angular.forEach(data.Elements, function (paramObj) {
            if (paramObj && paramObj.dictAttributes.ID) {
                var item = "~" + paramObj.dictAttributes.ID;
                lobjRequiredData.parameterList.push(item);
            }
        });
    };
    var traverseModelAndGetEntities = function (model, entityid, aobjFormModel) {
        angular.forEach(model.Elements, function (obj) {
            if (CONST.FORM.COLLECTION_TYPE_NODES.indexOf(obj.Name) > -1) {
                var entity = null;
                if (obj.Name == "sfwGridView" && obj.dictAttributes.sfwBoundToQuery && obj.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && obj.dictAttributes.sfwBaseQuery) {
                    lobjRequiredData.lstQueriesName.push(obj.dictAttributes.sfwBaseQuery);
                }
                if (obj.Name == "sfwGridView" && obj.dictAttributes.sfwParentGrid) {
                    var objParentGrid = FindControlByID(lobjRequiredData.SfxMainTable, obj.dictAttributes.sfwParentGrid);
                    if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                        var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(aobjFormModel.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                        if (entObject) {
                            var entObj = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entObject.Entity, obj.dictAttributes.sfwEntityField);
                            if (entObj) {
                                entity = entObj.Entity;
                            }
                        }
                    }
                } else {
                    entity = getEntityName(obj.dictAttributes.sfwEntityField, entityid);
                }
                if (entity && lobjRequiredData.lstEntity.indexOf(entity) <= -1) lobjRequiredData.lstEntity.push(entity);
            }
            if (obj && obj.dictAttributes.ID) {
                lobjRequiredData.parameterList.push(obj.dictAttributes.ID);
            }
            if (obj.Elements && obj.Elements.length > 0) {
                traverseModelAndGetEntities(obj, entityid, aobjFormModel);
            }
        });
    };
    var getQueryIdData = function (mainModel) {
        angular.forEach(mainModel.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("sfwDataField") && obj.dictAttributes.sfwDataField) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var list = PopulateColumnList(obj.dictAttributes.sfwQueryID, lobjRequiredData.FormModel, entityIntellisenseList, $scope.lstLoadedEntityColumnsTree);
            }
            if (obj.Elements && obj.Elements.length > 0) {
                getQueryIdData(obj);
            }
        });
    };
    var createRuledata = function (entExtraData) {
        lobjRequiredData.objEntityExtraData.lstRules = PopulateEntityRules(entExtraData, lobjRequiredData.iswizard, lobjRequiredData.FormModel.dictAttributes.sfwInitialLoadGroup);
    };
    var validateForm = function (mainModel, entityid, extraData, chartObj, query) {
        angular.forEach(mainModel.Elements, function (obj) {
            obj.ParentVM = mainModel;
            if (obj.dictAttributes.hasOwnProperty('sfwVisibleRule') && obj.dictAttributes.sfwVisibleRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                if (obj.dictAttributes.sfwParentGrid) {
                    listRule = getRuleData(obj);
                }
                if (obj.Name != "HeaderTemplate" && obj.Name != "FooterTemplate" && FindParent(obj, "sfwGridView")) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwVisibleRule', "invalid_visible_rule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwSelectColVisibleRule') && obj.dictAttributes.sfwSelectColVisibleRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                if (obj.dictAttributes.sfwParentGrid) {
                    listRule = getRuleData(obj);
                }
                if (obj.Name != "HeaderTemplate" && obj.Name != "FooterTemplate" && FindParent(obj, "sfwGridView")) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwSelectColVisibleRule', "sfwSelectColVisibleRule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwEnableRule') && obj.dictAttributes.sfwEnableRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwEnableRule', "invalid_enable_rule", CONST.VALIDATION.ENABLE_RULE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwReadOnlyRule') && obj.dictAttributes.sfwReadOnlyRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwReadOnlyRule', "invalid_readonly_rule", CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRulesGroup') && obj.dictAttributes.sfwRulesGroup) {
                validateGroups(obj, entityid);
            }

            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && query) {
                validateGridControlField(obj, query, mainModel);
            } else if (obj.dictAttributes.hasOwnProperty('sfwEntityField') && obj.Name != "parameter" && obj.dictAttributes.sfwEntityField) {
                var attrType = '';
                var entityname = entityid;
                if (FindParent(obj, "sfwGridView")) {
                    entityname = $GetGridEntity.getEntityName(lobjRequiredData.FormModel, obj);
                }

                if (obj.dictAttributes.sfwEntityField == "InternalErrors" || obj.dictAttributes.sfwEntityField == "ExternalErrors") {
                    entityname = "entError";
                }
                if (obj.dictAttributes.sfwRelatedGrid) {
                    entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
                }
                if (obj.dictAttributes.sfwParentGrid) {
                    entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
                }
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityname, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjRequiredData.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalQuery') && obj.dictAttributes.sfwRetrievalQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwRetrievalQuery, undefined, "sfwRetrievalQuery", "sfwRetrievalQuery", CONST.VALIDATION.INVALID_QUERY, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalMethod') && obj.dictAttributes.sfwRetrievalMethod) {
                var entityname = entityid;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
                }
                var methodList = getMethodList(entityname, false, true);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwRetrievalMethod, "sfwRetrievalMethod", "invalid_retrieval_method", CONST.VALIDATION.INVALID_METHOD, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwAutoQuery') && obj.dictAttributes.sfwAutoQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwAutoQuery, undefined, "sfwAutoQuery", "sfwAutoQuery", CONST.VALIDATION.INVALID_QUERY, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwBaseQuery') && obj.dictAttributes.sfwBaseQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwBaseQuery, undefined, "sfwBaseQuery", "sfwBaseQuery", CONST.VALIDATION.INVALID_QUERY, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwResource') && obj.dictAttributes.sfwResource && obj.dictAttributes.sfwResource != 0) {
                $ValidationService.checkValidListValue(lobjRequiredData.resourceList, obj, obj.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwValidationRules') && obj.dictAttributes.sfwValidationRules) {

                var lobjControl = obj;

                while (lobjControl) {
                    if (lobjControl.Name == "sfwWizardStep") {
                        break;
                    }
                    lobjControl = lobjControl.ParentVM;
                }
                var listHardRule = extraData.lstHardErrors;
                if (lobjControl) {
                    var strRuleGroup = lobjControl.dictAttributes.sfwRulesGroup;
                    listHardRule = createValidationRuleList(lobjRequiredData.validationData[entityid], lobjRequiredData.iswizard, strRuleGroup);
                }
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listHardRule = getValidationRuleData(obj);
                }
                $ValidationService.checkMultipleValueWithList(listHardRule, obj, obj.dictAttributes.sfwValidationRules, ";", 'sfwValidationRules', "invalid_validation_rule", CONST.VALIDATION.VALIDATION_RULE_NOT_EXISTS, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwActiveForm") && obj.dictAttributes.sfwActiveForm) {
                checkActiveForm(obj);
            }
            if (obj.dictAttributes.hasOwnProperty("Name") && obj.dictAttributes.Name && obj.Name == "udc") {
                checkActiveForm(obj);
            }

            if (obj.dictAttributes.hasOwnProperty('sfwExpression') && obj.dictAttributes.sfwExpression) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var list = getEntityAttributeByType(entityIntellisenseList, entityid, "Expression");
                list = $ValidationService.getListByPropertyName(list, "ID", false);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwExpression, "sfwExpression", "invalid_expression", CONST.VALIDATION.INVALID_EXPRESSION, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwMessageID") && obj.dictAttributes.sfwMessageID) {
                list = $ValidationService.getListByPropertyName(lobjRequiredData.lstMessages, "MessageID", false);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwMessageID, "sfwMessageID", "sfwMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwMessageId") && obj.dictAttributes.sfwMessageId) {
                list = $ValidationService.getListByPropertyName(lobjRequiredData.lstMessages, "MessageID", false);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwMessageID, "sfwMessageId", "sfwMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjRequiredData.validationErrorList);
            }
            if (obj.Name == "sfwDropDownList" || obj.Name == "sfwCascadingDropDownList" || obj.Name == "sfwMultiSelectDropDownList" || obj.Name == "sfwListPicker" || obj.Name == "sfwListBox" || obj.Name == "sfwRadioButtonList") {
                if (obj.dictAttributes.sfwLoadType == "CodeGroup") {
                    var list = $ValidationService.getListByPropertyName(lobjRequiredData.validationData.CodeGroup, "CodeID");
                    list.push("0");
                    validateEmptyCodeId(obj, list, entityid);
                }
            }
            if (obj.dictAttributes.hasOwnProperty('sfwLoadSource') && obj.dictAttributes.sfwLoadSource) {
                //if (obj.dictAttributes.sfwLoadType == "CodeGroup") {
                //    var list = $ValidationService.getListByPropertyName(lobjRequiredData.validationData.CodeGroup, "CodeID");
                //    if (obj.Name == "sfwDropDownList" || obj.Name == "sfwCascadingDropDownList" || obj.Name == "sfwMultiSelectDropDownList" || obj.Name == "sfwListPicker" || obj.Name == "sfwListBox" || obj.Name == "sfwRadioButtonList") {
                //        validateEmptyCodeId(obj, list);
                //    } else {
                //        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, lobjRequiredData.validationErrorList);
                //    }
                //} else
                if (obj.dictAttributes.sfwLoadType == "Query") {
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwLoadSource, undefined, "sfwLoadSource", "sfwLoadSource", CONST.VALIDATION.INVALID_QUERY, lobjRequiredData.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "Method") {
                    var entityname = entityid;
                    if (obj.dictAttributes.sfwRelatedGrid) {
                        entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
                    }
                    var methodList = getMethodList(entityname, true, false);
                    $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_method", CONST.VALIDATION.INVALID_METHOD, lobjRequiredData.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "ServerMethod") {
                    validateServerMethod(obj);
                }
            }
            if (obj.dictAttributes.hasOwnProperty('XValueMember') && obj.dictAttributes.XValueMember) {
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.XValueMember, entityid, "XValueMember", "XValueMember", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, '');
                copyErrorMessages(obj, chartObj);
            }
            if (obj.dictAttributes.hasOwnProperty('YValueMembers') && obj.dictAttributes.YValueMembers) {
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.YValueMembers, entityid, "YValueMembers", "YValueMembers", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, '');
                copyErrorMessages(obj, chartObj);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwCodeTable') && obj.dictAttributes.sfwCodeTable) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwCodeTable, undefined, "sfwCodeTable", "sfwCodeTable", CONST.VALIDATION.INVALID_CODE_TABLE, lobjRequiredData.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && obj.dictAttributes.sfwEntityField && obj.dictAttributes.hasOwnProperty("sfwMethodName") && obj.dictAttributes.sfwMethodName == "btnOpen_Click") {
                var list = PopulateEntityFieldsForOpenButton(obj, lobjRequiredData.isLookup);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjRequiredData.validationErrorList);
            }
            if (lobjRequiredData.isLookup && !lobjRequiredData.IsPrototype) {
                if (obj.dictAttributes.hasOwnProperty("sfwQueryID") && obj.dictAttributes.sfwQueryID) {
                    $ValidationService.checkValidListValue(lobjRequiredData.queryIDList, obj, obj.dictAttributes.sfwQueryID, "sfwQueryID", "invalid_query_id", CONST.VALIDATION.INVALID_QUERY_ID, lobjRequiredData.validationErrorList);
                }
                /*     if (obj.dictAttributes.hasOwnProperty("sfwDataField") && obj.dictAttributes.sfwDataField) { // user can enter substring,CAST function we can allow it
                         var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                         var list = [], attributeName = "ID";
                         var result = PopulateColumnList(obj.dictAttributes.sfwQueryID, lobjRequiredData.FormModel, entityIntellisenseList, $scope.lstLoadedEntityColumnsTree);
                         if (result) {
                             list = result.list;
                             attributeName = result.attribute;
                         }
                         // list = $ValidationService.getListByPropertyName(list, attributeName, false);
                         $ValidationService.checkDataFieldValue(list, obj, obj.dictAttributes.sfwDataField, attributeName, "sfwDataField", "invalid_data_field", CONST.VALIDATION.INVALID_DATA_FIELD, lobjRequiredData.validationErrorList);
                     }
                 */

            }

            if (obj.dictAttributes.hasOwnProperty("sfwParameters") && obj.dictAttributes.sfwParameters) {
                validateParameters(obj, obj.dictAttributes.sfwParameters, "sfwParameters");
            }
            if (obj.dictAttributes.hasOwnProperty("sfwAutoParameters") && obj.dictAttributes.sfwAutoParameters) {
                validateParameters(obj, obj.dictAttributes.sfwAutoParameters, "sfwAutoParameters");
            }
            if (obj.dictAttributes.hasOwnProperty("sfwCascadingRetrievalParameters") && obj.dictAttributes.sfwCascadingRetrievalParameters) {
                validateParameters(obj, obj.dictAttributes.sfwCascadingRetrievalParameters, "sfwCascadingRetrievalParameters");
            }

            if (obj.dictAttributes.hasOwnProperty("sfwObjectMethod") && obj.dictAttributes.sfwObjectMethod) {
                var methodList = getMethodList(entityid, false, false);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwObjectMethod, "sfwObjectMethod", "sfwObjectMethod", CONST.VALIDATION.INVALID_METHOD, lobjRequiredData.validationErrorList);
                if (obj && obj.errors && obj.errors.sfwObjectMethod) {
                    validateServerMethod(obj, "sfwObjectMethod");
                }
            }

            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && obj.dictAttributes.sfwEntityField && (obj.Name == "sfwScheduler" || obj.Name == "sfwCalendar")) {
                var entityname = getEntityName(obj.dictAttributes.sfwEntityField, entityid);
                if (obj.dictAttributes.hasOwnProperty("sfwEventId") && obj.dictAttributes.sfwEventId) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventId, entityname, "sfwEventId", "sfwEventId", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventName") && obj.dictAttributes.sfwEventName) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventName, entityname, "sfwEventName", "sfwEventName", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventStartDate") && obj.dictAttributes.sfwEventStartDate) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventStartDate, entityname, "sfwEventStartDate", "sfwEventStartDate", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventEndDate") && obj.dictAttributes.sfwEventEndDate) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventEndDate, entityname, "sfwEventEndDate", "sfwEventEndDate", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventCategory") && obj.dictAttributes.sfwEventCategory) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventCategory, entityname, "sfwEventCategory", "sfwEventCategory", CONST.VALIDATION.INVALID_FIELD, lobjRequiredData.validationErrorList, false, "");
                }
            }
            if (obj.Elements && obj.Elements.length > 0) {
                if (chartObj && chartObj.errors && chartObj.errors.hasOwnProperty("series_errors") && !$ValidationService.isEmptyObj(chartObj.errors.series_errors)) delete chartObj.errors.series_errors;

                var newExtraData = {};
                if (obj.Name == "sfwWizardStep") {
                    var strRuleGroup = obj.dictAttributes.sfwRulesGroup;
                    newExtraData.lstHardErrors = createValidationRuleList(lobjRequiredData.validationData[entityid], lobjRequiredData.iswizard, strRuleGroup);
                    newExtraData.lstRules = PopulateEntityRules(lobjRequiredData.validationData[entityid], lobjRequiredData.iswizard, null);
                    validateForm(obj, entityid, newExtraData, chartObj, query);
                }
                else if (CONST.FORM.COLLECTION_TYPE_NODES.indexOf(obj.Name) > -1) {
                    var entity;
                    var baseQuery = "";
                    if (obj.dictAttributes.sfwEntityField && (obj.dictAttributes.sfwEntityField == "InternalErrors" || obj.dictAttributes.sfwEntityField == "ExternalErrors")) {
                        entity = "entError";
                    }
                    else if (obj.Name == "sfwGridView" && obj.dictAttributes.sfwBoundToQuery && obj.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                        baseQuery = obj.dictAttributes.sfwBaseQuery;
                    } else if (obj.dictAttributes.sfwParentGrid && obj.dictAttributes.sfwEntityField) { // if parent grid setted to grid then get entity of parent grid
                        entity = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity, true);
                    } else {
                        entity = getEntityName(obj.dictAttributes.sfwEntityField, entityid);
                    }
                    var chart = {};
                    if (obj.Name == "sfwChart") chart = obj;
                    if (entity) {
                        if (lobjRequiredData.validationData && lobjRequiredData.validationData.hasOwnProperty(entity)) {
                            newExtraData.lstHardErrors = createValidationRuleList(lobjRequiredData.validationData[entity], lobjRequiredData.iswizard, null);
                            newExtraData.lstRules = PopulateEntityRules(lobjRequiredData.validationData[entity], lobjRequiredData.iswizard, null);
                        }
                        validateForm(obj, entity, newExtraData, chart, query);
                    } else {
                        validateForm(obj, entityid, extraData, chartObj, baseQuery);
                    }
                }
                else if (obj.Name == "sfwDialogPanel") {
                    var strdialogpanelid = obj.dictAttributes.ID;
                    var entityfieldname;
                    if (strdialogpanelid) {
                        var button = GetFieldFromFormObject(lobjRequiredData.SfxMainTable, 'sfwButton', 'sfwRelatedDialogPanel', strdialogpanelid);
                        if (button && button.length > 0 && button[0].dictAttributes.sfwRelatedControl) {
                            var gridview = GetFieldFromFormObject(lobjRequiredData.SfxMainTable, 'sfwGridView', 'ID', button[0].dictAttributes.sfwRelatedControl);
                            if (gridview && gridview.length > 0) {
                                entityfieldname = gridview[0].dictAttributes.sfwEntityField;
                            }
                        }
                    }
                    var entityName;
                    if (entityfieldname) {
                        entityName = getEntityName(entityfieldname, entityid);
                        if (lobjRequiredData.validationData && lobjRequiredData.validationData.hasOwnProperty(entityName)) {
                            newExtraData.lstHardErrors = createValidationRuleList(lobjRequiredData.validationData[entityName], lobjRequiredData.iswizard, null);
                            newExtraData.lstRules = PopulateEntityRules(lobjRequiredData.validationData[entityName], lobjRequiredData.iswizard, null);
                        }
                    }
                    if (entityName) {
                        validateForm(obj, entityName, newExtraData, chartObj, query);
                    } else validateForm(obj, entityid, extraData, chartObj, query);
                }
                else {
                    validateForm(obj, entityid, extraData, chartObj, query);
                }
            }
        });
        //$("#validation-btn").prop("disabled", false);
    };

    validateParameters = function (obj, params, prop) {
        if (angular.isArray(lobjRequiredData.parameterList) && lobjRequiredData.parameterList.length <= 0) {
            return;
        }
        var prefix = "prop-";
        if (prop == "sfwAutoParameters") {
            prefix = "autoprop-";
        } else if (prop == "sfwCascadingRetrievalParameters") {
            prefix = "cprop-";
        }
        var param = params.split(";");
        for (var i = 0; i < param.length; i++) {
            var str1 = param[i].split("=");
            var strId = str1[str1.length - 1];
            $ValidationService.checkValidListValue(lobjRequiredData.parameterList, obj, strId, prop, prefix + strId, "parameter value(" + strId + ") does not exists", lobjRequiredData.validationErrorList);
        }
    };
    var validateGroups = function (obj, entityid) {
        if (lobjRequiredData.validationData && lobjRequiredData.validationData.hasOwnProperty(entityid)) {
            var lstRules = lobjRequiredData.validationData[entityid];
            var group = lstRules && lstRules.lstGroupsList;
            var lstGroups = [];
            if (angular.isArray(group) && group.length > 0) {
                lstGroups = $ValidationService.getListByPropertyName(group[0].Elements, 'ID');
            }
            $ValidationService.checkValidListValue(lstGroups, obj, obj.dictAttributes.sfwRulesGroup, 'sfwRulesGroup', "sfwRulesGroup", CONST.VALIDATION.INVALID_GROUP, lobjRequiredData.validationErrorList);
        }
    };
    var validateGridControlField = function (obj, query, parentObj) {
        if ((obj.Name === "sfwLabel" || obj.Name == "parameter") && parentObj && (parentObj.Name == "HeaderTemplate" || parentObj.Name == "FooterTemplate" || parentObj.Name == "Parameters")) {
            $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, lobjRequiredData.FormModel.dictAttributes.sfwEntity, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjRequiredData.validationErrorList, false, "");
        } else if (lobjRequiredData.gridQueryResult && lobjRequiredData.gridQueryResult.hasOwnProperty(query)) {
            var list = $ValidationService.getListByPropertyName(lobjRequiredData.gridQueryResult[query], "CodeID", false);
            $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjRequiredData.validationErrorList);
        }
    };
    var getRuleData = function (obj) {
        var lstRules = [];
        var entityname = null;
        if (FindParent(obj, "sfwGridView")) {
            entityname = $GetGridEntity.getEntityName(lobjRequiredData.FormModel, obj);

        } else {
            entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
        }
        if (entityname) {
            lstRules = PopulateEntityRules(lobjRequiredData.validationData[entityname], lobjRequiredData.iswizard, null);
        }
        return lstRules;
    };
    var getValidationRuleData = function (obj) {
        var lstHardErrors = [];
        var entityname = FindEntityName(obj, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
        if (entityname) {
            lstHardErrors = createValidationRuleList(lobjRequiredData.validationData[entityname], lobjRequiredData.iswizard, null);
        }
        return lstHardErrors;
    };
    var validateEmptyCodeId = function (model, list, entity) {
        var property = "";
        if (model.dictAttributes.sfwRelatedGrid) {
            entity = FindEntityName(model, lobjRequiredData.FormModel.dictAttributes.sfwEntity);
        }
        if (IsCriteriaField(model) && model.dictAttributes.sfwDataField) {
            property = "sfwDataField";
        } else if (model.dictAttributes.sfwEntityField) {
            property = "sfwEntityField";
        }
        if (model.Name == "sfwCheckBoxList") {
            property = "sfwCheckBoxField";
        }

        if (model.dictAttributes.sfwLoadSource) {
            $ValidationService.checkValidListValue(list, model, model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, lobjRequiredData.validationErrorList);
        } else if (model.dictAttributes[property] && model.errors && !model.errors[property]) {
            var placeHolderValue = GetCodeID(entity, model.dictAttributes[property], $EntityIntellisenseFactory.getEntityIntellisense());
            if (!placeHolderValue) {
                $ValidationService.checkValidListValue([], model, model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, lobjRequiredData.validationErrorList, true);
            }
        }

    };

    var validateServerMethod = function (obj, attribute) {
        if (!(attribute && attribute.trim().length > 0)) {
            attribute = "sfwLoadSource";
        }
        var RemoteObjectName = "srvCommon";
        var lstRemoteObj = [];
        if (lobjRequiredData.FormModel && lobjRequiredData.FormModel.dictAttributes.sfwRemoteObject) {
            RemoteObjectName = lobjRequiredData.FormModel.dictAttributes.sfwRemoteObject;
        }
        if (lobjRequiredData.validationData && lobjRequiredData.validationData.hasOwnProperty("RemoteObject")) {
            lstRemoteObj = lobjRequiredData.validationData["RemoteObject"];
        }
        var objServerObject = GetServerMethodObject(RemoteObjectName, lstRemoteObj);
        var list = PopulateServerMethod([], obj, objServerObject, true);
        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes[attribute], attribute, attribute, CONST.VALIDATION.INVALID_METHOD, lobjRequiredData.validationErrorList);
    };

    var getMethodList = function (entityid, showonlycollection, showonlyobject) {
        var lst = $Entityintellisenseservice.GetIntellisenseData(entityid, "", "", true, false, true, true, false, false);
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

    var getEntityName = function (entField, entityid) {
        var entity = "";
        if (entField) {
            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityid, entField);
            if (object) {
                entity = object.Entity;
            }
        }
        return entity;
    };
    var iterateModel = function (model) {
        angular.forEach(model.Elements, function (obj) {
            if (CONST.FORM.IGNORE_NODES.indexOf(obj.Name) <= -1) {
                if (obj.Name == "sfwTable" && lobjRequiredData.FormModel.dictAttributes.sfwType.toLowerCase() == "lookup") {
                    $ValidationService.validateID(obj, lobjRequiredData.validationErrorList, obj.dictAttributes.ID);
                } else if (obj.Name != "sfwTable") {
                    $ValidationService.validateID(obj, lobjRequiredData.validationErrorList, obj.dictAttributes.ID);
                }
                $ValidationService.checkDuplicateId(obj, lobjRequiredData.FormModel, lobjRequiredData.validationErrorList, false, CONST.FORM.NODES);
            }
            if (obj.Elements && obj.Elements.length > 0 && (CONST.FORM.NODES.indexOf(obj.Name) > -1)) {
                iterateModel(obj);
            }
        });
    };

    var checkActiveForm = function (model) {


        var property = "sfwActiveForm";
        var ActiveFormType = "Lookup,Maintenance,Wizard,FormLinkLookup,FormLinkMaintenance,FormLinkWizard";

        if (model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") {
            ActiveFormType = "Maintenance,FormLinkMaintenance";
        }
        else if (model.dictAttributes.sfwMethodName == "btnOpenLookup_Click") {
            ActiveFormType = "Lookup,FormLinkLookup,Maintenance,FormLinkMaintenance";
        }

        if ((model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") ||
            (model.dictAttributes.sfwMethodName == "btnPrototypeSearch_Click")) {
            ActiveFormType = "Maintenance,FormLinkMaintenance";
        }

        if (model.dictAttributes.sfwMethodName === "btnRetrieve_Click") {
            ActiveFormType = "Lookup,FormLinkLookup";
        }

        if (model.Name == "sfwOpenDetail" || model.Name == "sfwMultiCorrespondence") {
            ActiveFormType = "Maintenance,FormLinkMaintenance";
        }
        if (model.Name == "sfwToolTipButton") {
            ActiveFormType = "Tooltip,";
        }
        if (model.Name == "udc") {
            ActiveFormType = "UserControl,";
            property = "Name";
        }
        var listOfFiles = [];
        function getListofFiles(file) {
            if (file.FileType == files[i]) {
                listOfFiles.push(file.FileID);
            }
        }
        if (ActiveFormType.indexOf(',') > -1) {
            var files = ActiveFormType.split(',');
            for (var i = 0; i < files.length; i++) {
                angular.forEach(lobjRequiredData.validationData.ActiveForms, getListofFiles);
            }
        }
        $ValidationService.checkActiveForm(listOfFiles, model, model.dictAttributes[property], property, 'invalid_active_form', CONST.VALIDATION.INVALID_ACTIVE_FORM, lobjRequiredData.validationErrorList);
    };
    var copyErrorMessages = function (objSeries, model) {
        if (model && model.errors && !model.errors.hasOwnProperty("series_errors")) model.errors.series_errors = {};
        for (var prop in objSeries.errors) {
            if (model && model.errors && !model.errors.series_errors.hasOwnProperty(prop)) {
                model.errors.series_errors[prop + "_" + objSeries.dictAttributes.Name] = objSeries.errors[prop];
            }
        }
    };
    var createValidationRuleList = function (objExtraData, isWizard, strRuleGroup) {
        var list = [];

        if (isWizard) {
            lstHardErrorsTemp = GetBuisnessRules(objExtraData, "", "", strRuleGroup, true);
            angular.forEach(lstHardErrorsTemp, function (strCodeDescription) {
                if (strCodeDescription != undefined && strCodeDescription != "") {
                    list.push(strCodeDescription);
                }
            });
        }
        else {
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
        }
        return list;
    };
    var PopulateEntityFieldsForOpenButton = function (obj, isLookup) {
        var alAvlFlds = [];
        PopulateControlsForActiveForm(alAvlFlds, lobjRequiredData.FormModel, obj, isLookup);
        var lstEntityFields = [];
        var objGrid = FindParent(obj, "sfwGridView");
        if (obj.dictAttributes.sfwRelatedControl || objGrid) {
            if (alAvlFlds.length > 0) {
                for (var i = 0; i < alAvlFlds.length; i++) {
                    var s = alAvlFlds[i];
                    var strParamValue = "";
                    if (s.indexOf("~") > -1)
                        strParamValue = s.substring(0, s.indexOf("~"));
                    else
                        strParamValue = s;
                    lstEntityFields.push(strParamValue);
                }
            }
        }
        return lstEntityFields;
    };
    //var getMessageList = function () {
    //    hubMain.server.populateMessageList().done(function (lstMessages) {
    //        lobjRequiredData.lstMessages = lstMessages;
    //    });
    //};
    //getMessageList();
    var FindEntityName = function (model, entityid, isChildOfGrid) {
        var entityName = entityid;
        if (model.dictAttributes.sfwParentGrid && model.dictAttributes.sfwEntityField) {
            var parentGrid = FindControlByID(lobjRequiredData.FormModel, model.dictAttributes.sfwParentGrid);
            var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(lobjRequiredData.FormModel.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
            if (objParentField) {
                entityName = objParentField.Entity;
                if (isChildOfGrid) {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, model.dictAttributes.sfwEntityField);
                    if (objField) {
                        entityName = objField.Entity;
                    }
                }
            }
        } else {
            var objmodel = FindControlByID(lobjRequiredData.FormModel, model.dictAttributes.sfwRelatedGrid);
            if (objmodel && objmodel.dictAttributes.sfwEntityField) {
                entityName = getEntityName(objmodel.dictAttributes.sfwEntityField, entityid);
            }
        }
        return entityName;
    };
    var SetMainTable = function () {
        function iLoopMainTable(row) {
            row.ParentVM = lobjRequiredData.SfxMainTable;
            if (row.Elements.length > 0) {
                if (lobjRequiredData.FormModel.dictAttributes.sfwType == "Wizard") {
                    for (k = 0; k < row.Elements.length; k++) {
                        var column = row.Elements[k];
                        column.ParentVM = row;
                        var panel = column.Elements.filter(function (x) {
                            return x.Name == "sfwWizard";
                        });

                        if (panel.length > 0) {
                            panel = panel[0];
                            panel.ParentVM = column;

                            var wizardstep = panel.Elements.filter(function (x) {
                                return x.Name == "WizardSteps";
                            });

                            var HeaderTemplate = panel.Elements.filter(function (x) {
                                return x.Name == "HeaderTemplate";
                            });

                            if (wizardstep.length > 0) {
                                wizardstep = wizardstep[0];
                                for (var iwizard = 0; iwizard < wizardstep.Elements.length; iwizard++) {
                                    var step = wizardstep.Elements[iwizard];
                                    step.ParentVM = wizardstep;
                                    if (step.Name == "sfwWizardStep") {
                                        if (step.Elements[0].Name == "sfwTable") {
                                            step.TableVM = step.Elements[0];
                                        }
                                        step.isLoaded = false;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                else {
                    angular.forEach(row.Elements, function (column) {
                        column.ParentVM = row;
                        var panel = column.Elements.filter(function (x) {
                            return x.Name == "sfwPanel" || x.Name == "sfwDialogPanel";
                        });
                        if (panel.length > 0) {
                            panel = panel[0];
                            panel.ParentVM = column;
                            panel.IsMainPanel = true;
                            for (j = 0; j < panel.Elements.length; j++) {
                                if (panel.Elements[j].Name == "sfwTable") {
                                    panel.TableVM = panel.Elements[j];
                                    break;
                                }
                            }
                            panel.isLoaded = false;
                        }

                    });
                }
            }
        }

        for (var ielem = 0; ielem < lobjRequiredData.FormModel.Elements.length; ielem++) {
            if (lobjRequiredData.FormModel.Elements[ielem].Name == "sfwTable") {
                lobjRequiredData.SfxMainTable = lobjRequiredData.FormModel.Elements[ielem];
                angular.forEach(lobjRequiredData.SfxMainTable.Elements, iLoopMainTable);
                break;
            }
        }
    };

    var PopulateQueryId = function () {

        var lstQueryID = [];
        var initialload = lobjRequiredData.FormModel.Elements.filter(function (x) { return x.Name == 'initialload'; });

        if (initialload.length > 0) {
            for (i = 0; i < initialload[0].Elements.length; i++) {
                if (initialload[0].Elements[i] && initialload[0].Elements[i].dictAttributes && initialload[0].Elements[i].dictAttributes.ID) {
                    lstQueryID.push(initialload[0].Elements[i]);
                }
            }
        }

        if (lstQueryID.length > 0) {
            var SelectedQuery = lstQueryID[0];
            //if (!$scope.MainQuery) {
            //    $scope.lstQueryID.splice(0, 0, { Name: "", dictAttributes: { ID: '' } });
            //}
            //   populateQueryFields(SelectedQuery);
        }

        function AddInqueryIDList(item) {
            if (item.dictAttributes.ID) {
                lobjRequiredData.queryIDList.push(item.dictAttributes.ID);
            }
        }
        lobjRequiredData.queryIDList = [];
        angular.forEach(lstQueryID, AddInqueryIDList);
    };

    return lobjFactoryData;
}]);