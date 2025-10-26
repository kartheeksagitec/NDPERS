//ngdialog is closed using ID so maintaing id across functions
var openedDialogId = '';
var openedQueryDialogId = '';
var openedExecuteQueryDialogId = '';
var openNewExpressionDialog = '';
var openNewRuleDialog = '';
var openNewGroupDialog = '';
var mappedDialog = '';

var lst = ['Select', 'From', 'Where', 'Equal', 'Coma', 'And', 'On', 'InnerJoin', 'LeftOuterJoin', 'RightOuterJoin', 'FullOuterJoin', 'CrossJoin',
    'Join', 'OrderBy', 'GroupBy', 'As', 'OpeningBracket', 'ClosingBracket'];
var dragDropData = null;
var dragDropDataObject = null;

app.controller("EntityController", ["$scope", "$http", "$rootScope", "hubcontext", "$FormatQueryFactory", "$resourceFactory", "$EntityIntellisenseFactory", "$NavigateToFileService", "$filter", "CONSTANTS", "$ValidationService", "$Errors", "$timeout", "$interval", "ConfigurationFactory", "$GetEntityFieldObjectService", "$compile", "$searchFindReferences", "$Entityintellisenseservice", function ($scope, $http, $rootScope, hubcontext, $FormatQueryFactory, $resourceFactory, $EntityIntellisenseFactory, $NavigateToFileService, $filter, CONST, $ValidationService, $Errors, $timeout, $interval, ConfigurationFactory, $GetEntityFieldObjectService, $compile, $searchFindReferences, $Entityintellisenseservice) {
    $scope.initialize = function () {
        //#region Variables
        $rootScope.IsLoading = true;
        $scope.currentfile = $rootScope.currentopenfile.file;
        $scope.objAttributes;
        $scope.objQueries;
        $scope.objConstraints;
        $scope.objMethods;
        $scope.objLifecycle;
        $scope.objEntity;
        $scope.lstlifecycle = [];
        $scope.lstCutandPaste = [];
        $scope.lstCopiedRule = [];
        $scope.lstRules = [];
        $scope.lstCopiedExpression = [];
        $scope.lstCodeValue = [];
        $scope.lstMessages = [];
        $scope.lstCodeGroups = [];
        $scope.IsExpressions = false;
        $scope.IsRule = false;
        $scope.lstEntityRules = [];
        $scope.lstEntityRulesDetails = [];
        $scope.lstLoadTypes = [];
        $scope.lstItemsRulesList = [];
        $scope.lstItemsCorrList = [];
        $scope.lstItemsFormsList = [];
        $scope.lstItemsHtxList = [];
        $scope.showSource = false;
        $scope.lstBusinessObject = undefined;
        $scope.lstBusinessObjectForProperty = undefined;
        $scope.selectedDesignSource = false;
        $scope.selectedRow = null;
        $scope.IsExecuteQueryShow = false;
        $scope.objRuleSearch = {
            strSearchRulesID: undefined,
            SelectedRuleStatus: undefined,
            SelectedReturnType: undefined,
            IsLogicalRuleChecked: undefined,
            IsDecisionTableChecked: undefined,
            IsExcelMatrixChecked: undefined,
            IsRuleStaticSelected: undefined,
            IsRulePrivateSelected: undefined
        };
        $scope.activeGridRow = undefined;
        $scope.SearchFieldconstraint = false;
        $scope.SearchBusinessObject = false;
        $scope.SearchAttributes = false;
        $scope.SearchRelationship = false;
        $scope.SearchExpressions = false;
        $scope.SearchColumns = false;
        $scope.SearchValidationRules = false;
        $scope.SearchRules = false;
        $scope.SearchQueries = false;
        $scope.SearchInitialLoad = false;
        $scope.SearchHardErrors = false;
        $scope.SearchSoftErrors = false;
        $scope.SearchValidateDelete = false;
        $scope.SearchGroups = false;
        $scope.SearchCheckList = false;
        $scope.selectedIneritedTab = 'Rule';
        $scope.datatypes = ['string', 'bool', 'decimal', 'datetime', 'double', 'float', 'int', 'long', 'short'];
        $scope.dataformats = ['', '{0:(###) ###-####}', '{0:d}', '{0:###-##-####}', '{0:$#,###,##0.00;($#,###,##0.00)}'];
        $scope.objRuleID = {};
        $scope.objRuleID.lstTypeIds = [{ "ID": "ID", "value": 'ID' }, { "ID": "Message ID", "value": 'sfwMessageId' }, { "ID": "Business Rule Id", "value": 'sfwBRID' }, { "ID": "Use Case Id", "value": 'sfwUseCaseID' }];
        $scope.objRuleID.selectedTypeID = {};
        $scope.objRuleID.searchRuleByID = "";
        $scope.index = 0;

        $scope.IsInitialLoadLoaded = false;
        $scope.IsHardErrorLoaded = false;
        $scope.IsSoftErrorLoaded = false;
        $scope.IsValidateDeleteLoaded = false;
        $scope.IsGroupsLoaded = false;
        $scope.IsCheckListLoaded = false;
        $scope.InheritedRulesMenuOptions = [];
        $scope.lstobjecttree = [];
        $scope.objBusinessObject = {};
        $scope.objBusinessObject.ObjTree = undefined;
        $scope.ItemExpanded = 'Forms';
        $scope.lstBusBaseMethods = [];
        $scope.blnShowCreateWithClass = true;
        $scope.blnShowBusinessObjectSection = true;
        //#endregion

        //DBTypes
        var Projectdetails = ConfigurationFactory.getLastProjectDetails();
        var strQueryTypes = Projectdetails.QueryTypes;
        if (!strQueryTypes) {
            strQueryTypes = "SQL:sfwSql";
        }
        $scope.lstDBQueryTypes = getlstQueryTypes(strQueryTypes);

        // Business object tree properties
        $scope.objBusinessObjectTree = {};
        $scope.objBusinessObjectTree.lstmultipleselectedfields = [];
        $scope.objBusinessObjectTree.lstCurrentBusinessObjectProperties = [];
        $scope.objBusinessObjectTree.lstdisplaybusinessobject = [];
        $scope.customTypes = [];

        // Editor Properties
        $scope.QueryEditor;

        $scope.validateEmptyMessageID = function (SelectedRule) {
            if (SelectedRule) {
                var list = $ValidationService.getListByPropertyName($scope.lstMessages, "MessageID");
                list.unshift("0"); // deafult message id
                $ValidationService.checkValidListValue(list, SelectedRule, SelectedRule.dictAttributes.sfwMessageId, "sfwMessageId", "sfwMessageId", CONST.VALIDATION.MESSAGE_ID_EMPTY, $scope.validationErrorList, true);
                if (SelectedRule.dictAttributes.sfwMessage) {
                    delete SelectedRule.errors.sfwMessageId;
                    if (SelectedRule && SelectedRule.errors && !$ValidationService.isEmptyObj(SelectedRule.errors)) {
                        $ValidationService.removeObjInToArray($scope.validationErrorList, SelectedRule);
                    }
                }

            }
        };
        //#region On Load
        hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
            if (data) {
                $scope.receiveentitymodel(data);

                hubMain.server.populateMessageList().done(function (lstmsgs) {
                    $scope.lstMessages = lstmsgs;
                    validateRuleMessageId();
                    validateMessageIds();
                });

                hubMain.server.getEntityItemsList($scope.currentfile.FileName); // Get Items which is mapped with current entity
                //load busbase methods

                $.connection.hubEntityModel.server.loadBusBaseMethods().done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data) {
                            $scope.lstBusBaseMethods = data;
                            validateXMLMethod();
                        }
                    });
                });
            }
            else {
                $rootScope.closeFile($scope.currentfile.FileName);
            }
        });

        $('#tblAttribute').on('mouseenter', function () {
            $("#tblAttribute > tr > th").resizable({
                handles: 'e',
                minWidth: 10

            });
        });


        var reserveWords = $FormatQueryFactory.getSqlReserveWords();
        if (!reserveWords) {
            hubcontext.hubMain.server.getSqlServerReservedWords();
        }

        //#endregion

        $scope.objnewonetooneAttribute = getNewBaseModel("attribute", { ID: 'obj', sfwType: 'Object', sfwEntity: '', sfwPrivate: 'False' });
        $scope.objnewonetomanyAttribute = getNewBaseModel("attribute", { ID: 'lst', sfwType: 'Collection', sfwValue: '', sfwEntity: '', sfwPrivate: 'False', Text: '' });
        $scope.objnewpropertyAttribute = getNewBaseModel("attribute", { ID: '', sfwDataType: '', sfwDataFormat: '', sfwPrivate: false, sfwValue: '', sfwEnumerator: '', Text: '', sfwType: "Property" });
        $scope.objnewexpressionAttribute = getNewBaseModel("attribute", { ID: '', sfwDataType: '', sfwDataFormat: '', sfwPrivate: false, sfwValue: '', sfwEnumerator: '', Text: '', sfwType: "Expression" });
        $scope.objnewConstraint = getNewBaseModel("item", { sfwFieldName: '', sfwRequired: false });
    }

    $scope.initialize();

    // Recieve Item list which is mapped with current entity
    $scope.recieveItemListMappedWithEntity = function (data, filetype) {
        if (data) {
            var arrList = JSON.parse(data);
            if (filetype == "Form") {
                $scope.lstItemsFormsList = $scope.lstItemsFormsList.concat(arrList);

            }
            else if (filetype == "Correspondence") {
                $scope.lstItemsCorrList = $scope.lstItemsCorrList.concat(arrList);

            }
            else if (filetype == "Rule") {
                $scope.lstItemsRulesList = $scope.lstItemsRulesList.concat(arrList);

            }

            else if (filetype == "Htx") {
                $scope.lstItemsHtxList = $scope.lstItemsHtxList.concat(arrList);

            }

        }
    };

    // for find in design check condition before setting to false - if its not there dont set the property   
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        if (items[0] && items[0].Name) {
            switch (items[0].Name) {
                case "attributes":
                    if (items[1]) {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "rules":
                    if (items[1]) {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "initialload":
                    if (items[items.length - 1].Name != "initialload") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "validatedelete":
                    if (items[items.length - 1].Name != "validatedelete") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "harderror":
                    if (items[items.length - 1].Name != "harderror") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "softerror":
                    if (items[items.length - 1].Name != "softerror") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "groupslist":
                    if (items[items.length - 1].Name != "groupslist") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "checklist":
                    if (items[items.length - 1].Name != "checklist") {
                        items[items.length - 1].isAdvanceSearched = IsAction;
                    } break;
                case "constraint":
                    if (items[1]) {
                        items[1].isAdvanceSearched = IsAction;
                    } break;
                case "queries":
                    if (items[1]) {
                        // if only query is searched
                        if (!items[2]) {
                            items[1].isAdvanceSearched = IsAction;
                        }
                        else if (items[3]) { // parameters node 
                            items[3].isAdvanceSearched = IsAction;
                        }
                    } break;
                case "methods":
                    if (items[1]) {
                        items[1].isAdvanceSearched = IsAction;
                    } break;
                case "lifecycle":
                    var lifecycleObj = $scope.lstlifecycle.filter(function (lifecycleitem) {
                        return lifecycleitem.ID == items[items.length - 1].dictAttributes.ID;
                    });
                    if (lifecycleObj && lifecycleObj.length > 0) {
                        lifecycleObj[0].isAdvanceSearched = IsAction;
                    } break;
                case "delete":
                    if (items[1]) {
                        items[1].isAdvanceSearched = IsAction;
                    } break;
                case "objectmethods":
                    if (items[1]) {
                        items[1].isAdvanceSearched = IsAction;
                    } break;
                default: $scope.selectDashboard();
            }
        }
    };

    $scope.receiveUpdatedEntityModelandIntelleenselist = function (lst, model) {
        $scope.receiveentitymodel(model);
        //update entity intellisense list
        var entityid = $scope.objEntity.dictAttributes.ID;
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var intellisenselist = lst[2];
        var lstSummaryReport = lst[1];
        EnitiyIntellisese = undefined;

        if (intellisenselist && intellisenselist.length) {
            for (var j = 0; j < intellisenselist.length; j++) {
                if (intellisenselist[j].ID == entityid) {
                    EnitiyIntellisese = intellisenselist[j];
                } else {
                    entityIntellisenseList.push(intellisenselist[j]);
                }
            }
        }
        for (var i = 0; i < entityIntellisenseList.length; i++) {
            if (entityIntellisenseList[i].ID == entityid) {
                if (EnitiyIntellisese) {
                    entityIntellisenseList[i].Queries = EnitiyIntellisese.Queries;
                    entityIntellisenseList[i].Attributes = EnitiyIntellisese.Attributes;
                    entityIntellisenseList[i].XmlMethods = EnitiyIntellisese.XmlMethods;
                    entityIntellisenseList[i].ObjectMethods = EnitiyIntellisese.ObjectMethods;
                }
                entityIntellisenseList[i].ParentId = $scope.objEntity.dictAttributes.sfwParentEntity ? $scope.objEntity.dictAttributes.sfwParentEntity : "";
                entityIntellisenseList[i].TableName = $scope.objEntity.dictAttributes.sfwTableName ? $scope.objEntity.dictAttributes.sfwTableName : "";
                entityIntellisenseList[i].BusinessObjectName = $scope.objEntity.dictAttributes.sfwObjectID ? $scope.objEntity.dictAttributes.sfwObjectID : "";
            }
        }

        var newScope = $scope.$new();
        newScope.lstSummaryReport = lstSummaryReport;
        newScope.openCSFileClick = function (obj) {
            if (obj && (obj.FileContent && obj.FileContent != "")) {
                $scope.strCSFileContent = obj.FileContent;
                var newCSScope = newScope.$new();
                newCSScope.closeCsDialog = function () {
                    newCSScope.csDialog.close();
                };
                newCSScope.csDialog = $rootScope.showDialog(newCSScope, "CS File Content", "CreateNewObject/views/Entity/OpenCSFile.html");
            }
        };

        newScope.closeDialog = function () {
            newScope.summaryDialog.close();
        };
        if (lstSummaryReport && lstSummaryReport.length > 0) {
            newScope.summaryDialog = $rootScope.showDialog(newScope, "Summary Report", "CreateNewObject/views/Entity/FileSummaryReport.html");
        }
    };

    $scope.receiveentitymodel = function (data) {
        $rootScope.IsLoading = false;
        $scope.$evalAsync(function () {
            $scope.objEntityExtraFields = [];
            $scope.objEntity = data;
            if ($scope.objEntity.dictAttributes.CreateWithClass && $scope.objEntity.dictAttributes.CreateWithClass.toLowerCase() == "false") {
                $scope.blnShowCreateWithClass = false;
            }
            $scope.lstRules = [];
            $scope.lstlifecycle = [];
            $scope.lstQueryGroups = [];
            $scope.lstQueryGroups.push("");

            if ($scope.objEntity.objExtraData != null && $scope.objEntity.objExtraData.objInheritedRules && $scope.objEntity.objExtraData.objInheritedRules != null) {
                $scope.objInheritedRules = $scope.objEntity.objExtraData.objInheritedRules;
                $scope.objInheritedRules.Text = "Rules";
                $scope.objInheritedRules.IsExpanded = true;
            }

            for (var i = 0; i < $scope.objEntity.Elements.length; i++) {
                if ($scope.objEntity.Elements[i].Name == 'queries') {
                    $scope.objQueries = $scope.objEntity.Elements[i];

                    for (j = 0; j < $scope.objQueries.Elements.length; j++) {
                        if ($scope.objQueries.Elements[j].dictAttributes.sfwGroupName) {
                            $scope.UpdateGroupName($scope.objQueries.Elements[j].dictAttributes.sfwGroupName);
                        }
                    }

                }

                else if ($scope.objEntity.Elements[i].Name == 'constraint') {
                    $scope.objConstraints = $scope.objEntity.Elements[i];
                }
                else if ($scope.objEntity.Elements[i].Name == 'rules') {

                    $scope.objRules = $scope.objEntity.Elements[i];
                    $scope.objRules.Text = "Rules";
                    $scope.objRules.IsExpanded = true;
                    $scope.lstRules.push($scope.objRules);
                }

                else if ($scope.objEntity.Elements[i].Name == 'attributes') {
                    $scope.objAttributes = $scope.objEntity.Elements[i];
                }

                else if ($scope.objEntity.Elements[i].Name == 'initialload') {
                    $scope.objInitialLoad = $scope.objEntity.Elements[i];
                }

                else if ($scope.objEntity.Elements[i].Name == 'softerror') {
                    $scope.objSoftError = $scope.objEntity.Elements[i];
                    $scope.LoadParentForObject($scope.objSoftError);
                }

                else if ($scope.objEntity.Elements[i].Name == 'harderror') {
                    $scope.objHardError = $scope.objEntity.Elements[i];
                    $scope.LoadParentForObject($scope.objHardError);
                }
                else if ($scope.objEntity.Elements[i].Name == 'validatedelete') {
                    $scope.objValidateDelete = $scope.objEntity.Elements[i];
                }
                else if ($scope.objEntity.Elements[i].Name == 'groupslist') {
                    $scope.objGroupList = $scope.objEntity.Elements[i];
                    $scope.LoadParentForObject($scope.objGroupList);
                }

                else if ($scope.objEntity.Elements[i].Name == 'checklist') {
                    $scope.objCheckList = $scope.objEntity.Elements[i];
                }

                else if ($scope.objEntity.Elements[i].Name == 'methods') {
                    $scope.objMethods = $scope.objEntity.Elements[i];
                }

                else if ($scope.objEntity.Elements[i].Name == 'lifecycle') {
                    $scope.objLifecycle = $scope.objEntity.Elements[i];

                }

                else if ($scope.objEntity.Elements[i].Name == 'delete') {
                    $scope.objDelete = $scope.objEntity.Elements[i];
                }
                else if ($scope.objEntity.Elements[i].Name == 'ExtraFields') {
                    $scope.objEntityExtraFields = $scope.objEntity.Elements[i];
                }
                else if ($scope.objEntity.Elements[i].Name == "objectmethods") {
                    $scope.objObjectMethods = $scope.objEntity.Elements[i];
                }
            }

            //$scope.removeExtraFieldsDataInToMainModel();
            //========================= adding elements when there is no element in Entity
            if ($scope.objQueries == undefined) {
                $scope.objQueries = $scope.CreateNewObject("queries", $scope.objEntity);
            }
            if ($scope.objConstraints == undefined) {

                $scope.objConstraints = $scope.CreateNewObject("constraint", $scope.objEntity);

            }
            if ($scope.objRules == undefined) {

                $scope.objRules = $scope.CreateNewObject("rules", $scope.objEntity);
                $scope.objRules.Text = "Rules";
                $scope.objRules.IsExpanded = true;
                $scope.lstRules.push($scope.objRules);
            }
            if ($scope.objAttributes == undefined) {

                $scope.objAttributes = $scope.CreateNewObject("attributes", $scope.objEntity);

            }
            if ($scope.objInitialLoad == undefined) {

                $scope.objInitialLoad = $scope.CreateNewObject("initialload", $scope.objEntity);

            }
            if ($scope.objSoftError == undefined) {

                $scope.objSoftError = $scope.CreateNewObject("softerror", $scope.objEntity);

            }
            if ($scope.objHardError == undefined) {
                $scope.objHardError = $scope.CreateNewObject("harderror", $scope.objEntity);

            }
            if ($scope.objValidateDelete == undefined) {

                $scope.objValidateDelete = $scope.CreateNewObject("validatedelete", $scope.objEntity);

            }
            if ($scope.objDelete == undefined) {

                $scope.objDelete = $scope.CreateNewObject("delete", $scope.objEntity);

            }
            if ($scope.objGroupList == undefined) {

                $scope.objGroupList = $scope.CreateNewObject("groupslist", $scope.objEntity);

            }
            if ($scope.objCheckList == undefined) {

                $scope.objCheckList = $scope.CreateNewObject("checklist", $scope.objEntity);

            }
            if ($scope.objMethods == undefined) {

                $scope.objMethods = $scope.CreateNewObject("methods", $scope.objEntity);

            }
            if ($scope.objLifecycle == undefined) {

                $scope.objLifecycle = $scope.CreateNewObject("lifecycle", $scope.objEntity);

            }
            if ($scope.objEntityExtraFields.length == 0) {
                $scope.objEntityExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                };
            }

            if ($scope.objObjectMethods == undefined && $scope.blnShowCreateWithClass) {
                $scope.objObjectMethods = $scope.CreateNewObject("objectmethods", $scope.objEntity);
            }

            var objBeforeValidate = { DispalyName: 'Before Validate', ID: 'BeforeValidate', XmlMethod: '' };
            $scope.lstlifecycle.push(objBeforeValidate);
            var objAfterValidate = { DispalyName: 'After Validate', ID: 'AfterValidate', XmlMethod: '', Parameters: '' };
            $scope.lstlifecycle.push(objAfterValidate);
            var objBeforePersist = { DispalyName: 'Before Persist', ID: 'BeforePersistChanges', XmlMethod: '', Parameters: '' };
            $scope.lstlifecycle.push(objBeforePersist);
            var objAfterPersist = { DispalyName: 'After Persist', ID: 'AfterPersistChanges', XmlMethod: '', Parameters: '' };
            $scope.lstlifecycle.push(objAfterPersist);
            var Correspondence = { DispalyName: 'Correspondence', ID: 'Correspondence', XmlMethod: '', Parameters: '' };
            $scope.lstlifecycle.push(Correspondence);

            if ($scope.objLifecycle != undefined) {
                if ($scope.objLifecycle.Elements.length > 0) {
                    for (var i = 0; i < $scope.objLifecycle.Elements.length; i++) {
                        for (var j = 0; j < $scope.lstlifecycle.length; j++) {
                            if ($scope.lstlifecycle[j].ID == $scope.objLifecycle.Elements[i].dictAttributes.ID) {
                                $scope.lstlifecycle[j].XmlMethod = $scope.objLifecycle.Elements[i].dictAttributes.sfwXmlMethod;
                                $scope.lstlifecycle[j].Parameters = $scope.objLifecycle.Elements[i].dictAttributes.sfwNavigationParameter;
                            }
                        }
                    }
                }
            }

            $.connection.hubEntityModel.server.loadEntityRules($scope.objEntity.dictAttributes.ID).done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.lstScenarioDetails = [];
                    $scope.lstEntityRules = data;
                    $scope.selectEntityRules(0);
                    $scope.lstEntityRulesDetails = data;

                });
            });

            if ($scope.objEntity.dictAttributes.sfwChecklistId != undefined && $scope.objEntity.dictAttributes.sfwChecklistId != null) {

                $.connection.hubEntityModel.server.populateCodeValues($scope.objEntity.dictAttributes.sfwChecklistId);
            }

            $.connection.hubEntityModel.server.populateTables().done(function (data) {
                if (data && data.length == 3) {
                    $scope.recieveGetPopulateErrorTables(data[0]);
                    $scope.recieveGetPopulateChecklistTables(data[1]);
                    $scope.recieveGetPopulateExpressionTables(data[2]);
                }
            });

            if ($scope.objEntity.dictAttributes.sfwObjectID != undefined && $scope.objEntity.dictAttributes.sfwObjectID != "") {
                $.connection.hubEntityModel.server.getDeleteProperties($scope.objEntity.dictAttributes.sfwObjectID).done(function (data) {
                    $scope.$evalAsync(function () {
                        $scope.DeleteProperties = data;
                    });
                });
                $.connection.hubEntityModel.server.getPartialFileData($scope.objEntity.dictAttributes.sfwObjectID).done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data && data.length > 0) {
                            $scope.PartialFileData = data[0];
                            $scope.PartialFileDataTypes = data[1];
                            if ($scope.PartialFileDataTypes) {
                                $scope.PartialFileDataTypes.splice(0, 0, "");
                            }
                            $scope.BusObjPartialFileName = data[2];
                            for (var index = 0; index < $scope.PartialFileData.length; index++) {
                                $scope.setPropertyPrefix($scope.PartialFileData[index]);
                            }
                            $scope.validateCSProperties();
                        }
                    });
                });
            }

            if (!$scope.lstBusinessObject) {
                hubMain.server.getLstBusinessObject().done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data && data.length > 0) {
                            $scope.lstBusinessObject = data;
                            if ($scope.lstBusinessObject && !$scope.lstBusinessObject.some(function (x) { return x.ID == $scope.objEntity.dictAttributes.sfwObjectID; })) {
                                $scope.blnShowBusinessObjectSection = false;
                            }
                        } else {
                            $scope.lstBusinessObject = [];
                        }
                    });
                });
            }
            if (!$scope.lstBusinessObjectForProperty) {
                hubMain.server.getLstBusinessObjectForProperty().done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data && data.length > 0) {
                            $scope.lstBusinessObjectForProperty = data;
                        } else {
                            $scope.lstBusinessObjectForProperty = [];
                        }
                    });
                });
            }

            $scope.PopulateLoadTypes();
            if ($rootScope.queryID != undefined && $rootScope.queryID != "") {
                $scope.openQueryFromLogicalRule();
            }
            if ($scope.currentfile.SelectNodePath && $scope.currentfile.SelectNodePath != "") {
                $scope.selectElement($scope.currentfile.SelectNodePath);
            }
            //$scope.ValidateRulesandExpressions();
            if ($scope.objInheritedRules && $scope.objInheritedRules.Elements.length > 0) {
                angular.forEach($scope.objInheritedRules.Elements, function (obj) {
                    $scope.LoadRulesDetails(obj);
                });
            }
            $scope.validationErrorList = [];
            $scope.warningList = [];
            if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
                var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName });
                if (fileErrObj.length == 0) $Errors.validationListObj.push({ FileName: $scope.currentfile.FileName, errorList: [], warningList: [] });
                $scope.validateFileData();
            }

            $.connection.hubEntityModel.server.getMainCDOList($scope.objEntity.dictAttributes.sfwObjectID).done(function (data) {
                if (data) {
                    $scope.$evalAsync(function () {
                        $scope.tableObjects = data;
                        if ($scope.tableObjects.length > 0) {
                            $scope.tableObjects.splice(0, 0, "");
                        }
                    });
                }
            });

            $scope.LoadRulesForChild($scope.lstRules);

            $.connection.hubMain.server.getCustomTypes().done(function (data) {
                $scope.customTypes = data;
            });

        });

    };

    $scope.isSet = function (tabnum) {
        return $scope.tab == tabnum;
    };

    //#region Column Sorting Start
    $scope.sort = {
        active: '',
        descending: undefined
    };

    //#endregion Column Sorting End

    $scope.clearValidationRuleSelection = function () {
        $scope.SelectedRule = undefined;
        $scope.SelectedInitialLoad = undefined;
        $scope.SelectedHardError = undefined;
        $scope.SelectedSoftError = undefined;
        $scope.SelectedValidateDelete = undefined;
        $scope.SelectedGroup = undefined;
    };
    $scope.clearBusinessObjectTabSelection = function () {
        $scope.objMethods.Elements.forEach(function (method) {
            method.selected = false;
        });
        $scope.SelectedXMLMethod = undefined;
        $scope.objDelete.Elements.forEach(function (Delete) {
            Delete.isSelected = false;
        });
        if ($scope.PartialFileData && Object.prototype.toString.call($scope.PartialFileData) === '[object Array]') {
            $scope.PartialFileData.forEach(function (item) {
                item.isSelected = false;
            });
        }
        $scope.selectedLifeCycle = undefined;
        $scope.SelectedObjectMethod = undefined;
    };
    //#region Source To XML

    var editor_html;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
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
                $scope.receivedesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.selectedMethodProperty = undefined;
                        //$scope.objMethods = undefined;
                        $scope.SelectedGroup = undefined;
                        $scope.SelectedCheckList = undefined;
                        $scope.SelectedValidateDelete = undefined;
                        $scope.SelectedSoftError = undefined;
                        $scope.SelectedHardError = undefined;
                        $scope.SelectedInitialLoad = undefined;
                        $scope.SelectedRule = undefined;
                        $scope.selectedCollectionItem = undefined;
                        $scope.selectedOneToOne = undefined;
                        $scope.selectedOneToMany = undefined;
                        $scope.selectedExpression = undefined;
                        $scope.selectedColumn = undefined;
                        $scope.selectedProperty = undefined;
                        $scope.selectedQuery = undefined;
                        $scope.SelectedObjectMethod = undefined;
                        $scope.selectedLifeCycle = undefined;

                        $scope.receiveentitymodel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}
                    //Commented following code, because source to design synchronization is hold for now.
                    var selectedItem = [];

                    //Select the particular tab according to path
                    $scope.selectElement(path);
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
    $scope.traverseMainModel = function (path) {
        var items = [];
        var objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.objEntity;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                if (objHierarchy) {
                    items.push(objHierarchy);
                    if (objHierarchy.ParentID && (objHierarchy.ParentID == "softerror" || objHierarchy.ParentID == "harderror")) {
                        var obString = path.split(',');
                        var ruleString = obString[obString.length - 1].substring(0, obString[obString.length - 1].indexOf('-'));
                        if (objHierarchy.Children.length > 0 && ruleString == "rule") {
                            objHierarchy = FindChidrenHierarchy(objHierarchy, path.split(',')[i + 2].substring(path.split(',')[i + 2].lastIndexOf('-') + 1));
                            items.push(objHierarchy);
                            break;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
        }
        return items;
    };
    $scope.selectElement = function (path) {
        $scope.$evalAsync(function () {
            if (path != null && path.length > 1) {
                var idSelectedDiv = ""; // for scrolling the selected item to view
                var items = $scope.traverseMainModel(path);
                if (items != null && items != "") {
                    if (items[0]) {
                        $scope.activeGridRow = undefined;
                        if (items[0].Name == "attributes") {
                            $scope.selectAttributeTab();
                            var strAttributetype = "";
                            if (items[1] && items[1].dictAttributes.sfwType.toLowerCase() == "expression") {
                                strAttributetype = "EXPRESSION";
                            }
                            else if (items[1] && (items[1].dictAttributes.sfwType.toLowerCase() == "column" || items[1].dictAttributes.sfwType.toLowerCase() == "description")) {
                                strAttributetype = "COLUMNS";
                            }
                            else if (items[1] && items[1].dictAttributes.sfwType.toLowerCase() == "object") {
                                strAttributetype = "ONETOONE";
                            }
                            else if (items[1] && (items[1].dictAttributes.sfwType.toLowerCase() == "collection" || items[1].dictAttributes.sfwType.toLowerCase() == "cdocollection" || items[1].dictAttributes.sfwType.toLowerCase() == "list")) {
                                strAttributetype = "ONETOMANY";
                            }
                            else if (items[1] && items[1].dictAttributes.sfwType.toLowerCase() == "property") {
                                strAttributetype = "PROPERTIES";
                            }
                            $timeout(function () {
                                setAttributesActiveTab(strAttributetype);
                                $scope.activeGridRow = items[items.length - 1];
                            });
                        }
                        else if (items[0].Name == "groupslist") {
                            $scope.clearValidationRuleSelection();
                            $scope.selectValidationRulesTab();
                            $scope.SelectChildRuleView('Group');
                            if (items.length > 2) {
                                for (var i = 1; i < items.length - 1; i++) {
                                    items[i].IsExpanded = true;
                                }
                            }
                            if (items[items.length - 1].Name != "groupslist") {
                                $scope.SelectGroupClick(items[items.length - 1]);
                            }
                            idSelectedDiv = "validation-rules-grouplist-section";
                        }
                        else if (items[0].Name == "queries") {
                            $scope.selectQueriesTab();
                            if (items[1]) {
                                $scope.selectQuery(items[1], items[0].Elements.indexOf(items[1]));
                                if (items[2] && items[2].Name == "parameters") {

                                    if (items[3]) $scope.selectedParam = items[3];
                                }
                                else if (items[2] && items[2].Name == "mappedcolumns") {

                                    if (items[3]) $scope.selectedUnmappedColumn = items[3];
                                }
                            }
                            idSelectedDiv = "entity-queries-section";
                        }
                        else if (items[0].Name == "constraint") {
                            $scope.selectFieldConstraintsTab();
                            $timeout(function () {
                                $scope.activeGridRow = items[items.length - 1];
                            });
                        }
                        else if (items[0].Name == "methods") {
                            $scope.clearBusinessObjectTabSelection();
                            $scope.selectBusinessObjectTab();
                            if (items[1]) {
                                $scope.SelectXmlMethod(items[1]);

                            }
                            idSelectedDiv = "entity-xml-method-section";
                        }
                        else if (items[0].Name == "rules") {
                            $scope.clearValidationRuleSelection();
                            $scope.SelectedRuleView = undefined;
                            $scope.selectValidationRulesTab();
                            if (items.length > 2) {
                                for (var i = 1; i < items.length - 1; i++) {
                                    items[i].IsExpanded = true;
                                }
                            }
                            if (items.length > 1) {
                                $scope.selectedRules(items[items.length - 1]);
                            }
                            idSelectedDiv = "entity-validation-main-rules-section";
                        }
                        else if (items[0].Name == "initialload") {
                            $scope.selectValidationRulesTab();
                            if (items[items.length - 1].Name != "initialload") {
                                $scope.SelectedInitialLoadClick(items[items.length - 1], window.event);
                                $scope.scrollToPosition("#entity-validation-initialload-section", ".page-sidebar-content", $scope.SelectedInitialLoad.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
                            }
                        }
                        else if (items[0].Name == "validatedelete") {
                            $scope.clearValidationRuleSelection();
                            $scope.selectValidationRulesTab();
                            $scope.selectValidateDeleteTab();
                            if (items[items.length - 1].Name != "validatedelete") {
                                $scope.SelectedValidateDeleteClick(items[items.length - 1]);
                            }
                            idSelectedDiv = "entity-validation-delete-section";
                        }
                        else if (items[0].Name == "checklist") {
                            $scope.clearValidationRuleSelection();
                            $scope.selectValidationRulesTab();
                            $scope.selectCheckListTab();
                            if (items[items.length - 1].Name != "checklist") {
                                $scope.SelectCheckListClick(items[items.length - 1]);
                            }
                            idSelectedDiv = "entity-validation-checklist-section";
                        }
                        else if (items[0].Name == "harderror") {
                            $scope.clearValidationRuleSelection();
                            $scope.selectValidationRulesTab();
                            $scope.selectHardErrorTab();
                            $scope.SelectChildRuleView('HardError');
                            if (items.length > 2) {
                                for (var i = 1; i < items.length - 1; i++) {
                                    items[i].IsExpanded = true;
                                }
                            }
                            if (items[items.length - 1].Name != "harderror") {
                                $scope.SelectedErrorClick(items[items.length - 1], null, 'HardError');
                            }
                            idSelectedDiv = "entity-validation-harderror-section";
                        }
                        else if (items[0].Name == "softerror") {
                            $scope.clearValidationRuleSelection();
                            $scope.selectValidationRulesTab();
                            $scope.selectSoftErrorTab();
                            $scope.SelectChildRuleView('SoftError');
                            if (items.length > 2) {
                                for (var i = 1; i < items.length - 1; i++) {
                                    items[i].IsExpanded = true;
                                }
                            }
                            if (items[items.length - 1].Name != "softerror") {
                                $scope.SelectedErrorClick(items[items.length - 1], null, 'SoftError');
                            }
                            idSelectedDiv = "entity-validation-softerror-section";
                        }
                        else if (items[0].Name == "lifecycle") {
                            $scope.clearBusinessObjectTabSelection();
                            $scope.selectBusinessObjectTab();
                            $scope.selectLifeCycleMethod(items[items.length - 1]);
                            idSelectedDiv = "entity-businessobject-LifeCycle-section";
                        }
                        else if (items[0].Name == "delete") {
                            $scope.clearBusinessObjectTabSelection();
                            $scope.selectBusinessObjectTab();
                            $scope.setSelectedDeleteProperty(items[items.length - 1]);
                            idSelectedDiv = "entity-businessobject-delete-section";
                            //$scope.selectLifeCycleTab();
                        }
                        else if (items[0].Name == "objectmethods") {
                            $scope.clearBusinessObjectTabSelection();
                            $scope.selectBusinessObjectTab();
                            if (items[items.length - 1].Name != "objectmethods") {
                                if (items[items.length - 1].Name != "method") {
                                    for (var i = 0; i < items.length; i++) {
                                        if (items[i].Name == "method") {
                                            $scope.SelectObjectMethod(items[i]);
                                            break;
                                        }
                                    }
                                }
                                else {
                                    $scope.SelectObjectMethod(items[items.length - 1]);
                                }
                            }
                            idSelectedDiv = "entity-businessobject-objectmethod-section";
                        }
                        else {
                            $scope.selectDashboard();
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
                    else {
                        $scope.selectDashboard();
                    }
                }
            }
            $rootScope.IsLoading = false;
        });
    };
    $scope.FindNode = function (objParentElements, path, selectedItem, count) {
        var parent;
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                for (var i = path.length - 2; i >= 0; i--) {
                    if ((item.dictAttributes.ID != undefined)) {
                        if (item.Name + "=\"" + item.dictAttributes.ID + "\"" == path[i]) {
                            selectedItem[count] = item;
                            count++;
                            $scope.FindNode(item, path, selectedItem, count);
                        }
                    }
                    else if (item.Name == path[i]) {
                        selectedItem[count] = item;
                        count++;
                        $scope.FindNode(item, path, selectedItem, count);
                    }
                }
            });
        }
        return selectedItem;
    };
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            if ($scope.objEntity != null && $scope.objEntity != undefined) {
                //$scope.addExtraFieldsDataInToMainModel();
                $scope.CheckAndRemoveUnmappedColumnNode();
                var objreturn1 = GetBaseModel($scope.objEntity);
                var nodeId = [];
                $rootScope.IsLoading = true;
                var sObj;
                var indexPath = [];
                var pathString;
                //Send current selected object
                if (objreturn1 != "") {
                    var selectedTab = $scope.SelectedView;
                    var selectedEntityItem;
                    if (selectedTab == "Attributes") {
                        var activeAttributeTabHTMLEl = document.querySelector("#" + $scope.currentfile.FileName + " ul[attribute-tab-wrapper] li.active a");
                        if (activeAttributeTabHTMLEl) {
                            var strActiveTab = activeAttributeTabHTMLEl.getAttribute("data-attribute-type");
                            if (strActiveTab) {
                                var attrType = [];
                                switch (strActiveTab) {
                                    case 'COLUMNS': attrType = ["Column"]; break;
                                    case 'ONETOONE': attrType = ["Object"]; break;
                                    case 'ONETOMANY': attrType = ["Collection", "List", "CDOCollection"]; break;
                                    case 'PROPERTIES': attrType = ["Property"]; break;
                                    case 'EXPRESSION': attrType = ["Expression"]; break;
                                    default: break;
                                }
                                for (var i = 0; i < $scope.objAttributes.Elements.length; i++) {
                                    if ($scope.objAttributes.Elements[i].isSelected && attrType.indexOf($scope.objAttributes.Elements[i].dictAttributes.sfwType) > -1) {
                                        selectedEntityItem = $scope.objAttributes.Elements[i];
                                        break;
                                    }
                                }
                            }
                        }
                        selectedEntityItem = !selectedEntityItem ? $scope.objAttributes : selectedEntityItem;
                    }
                    else if (selectedTab == "Queries") {
                        if ($scope.selectedQuery) {
                            selectedEntityItem = $scope.selectedQuery;
                        }
                        else {
                            selectedEntityItem = $scope.objQueries;
                        }
                    }
                    else if (selectedTab == "ValidationRules") {
                        if ($scope.SelectedRuleView == 'ValidationRules') {
                            if ($scope.SelectedInitialLoad) {
                                selectedEntityItem = $scope.SelectedInitialLoad;
                            }
                            else if ($scope.SelectedHardError) {
                                selectedEntityItem = $scope.SelectedHardError;
                            }
                            else if ($scope.SelectedSoftError) {
                                selectedEntityItem = $scope.SelectedSoftError;
                            }
                            else if ($scope.SelectedValidateDelete) {
                                selectedEntityItem = $scope.SelectedValidateDelete;
                            }
                            else if ($scope.SelectedGroup) {
                                selectedEntityItem = $scope.SelectedGroup;
                            }
                            else if ($scope.SelectedRule) {
                                selectedEntityItem = $scope.SelectedRule;
                            }
                            else {
                                selectedEntityItem = $scope.objRules;
                            }
                        }
                        else if ($scope.SelectedRuleView == 'Checklist') {
                            if ($scope.SelectedCheckList) {
                                selectedEntityItem = $scope.SelectedCheckList;
                            }
                            else {
                                selectedEntityItem = $scope.objCheckList;
                            }
                        }
                        else {
                            //nodeId[0] = "";
                            //nodeId[1] = "";
                        }
                    }
                    else if (selectedTab == "FieldConstraints") {
                        for (var i = 0; i < $scope.objConstraints.Elements.length; i++) {
                            if ($scope.objConstraints.Elements[i].isSelected) {
                                selectedEntityItem = $scope.objConstraints.Elements[i];
                                break;
                            }
                        }
                        selectedEntityItem = !selectedEntityItem ? $scope.objConstraints : selectedEntityItem;
                    }
                    else if (selectedTab == "BusinessObject") {
                        if ($scope.objMethods && $scope.SelectedXMLMethod) {
                            selectedEntityItem = $scope.SelectedXMLMethod;
                        }
                        else if ($scope.selectedLifeCycle) {
                            var SelectedLifeCycleMethod = $scope.objLifecycle.Elements.filter(function (x) { return x.dictAttributes.ID == $scope.selectedLifeCycle.ID; });
                            selectedEntityItem = SelectedLifeCycleMethod[0];
                        }
                        else if ($scope.SelectedObjectMethod) {
                            selectedEntityItem = $scope.SelectedObjectMethod;
                        }
                        else if ($scope.objDelete.Elements.some(function (item) { if (item.isSelected == true) { selectedEntityItem = item; return true; } })) {

                        }
                        else {
                            selectedEntityItem = $scope.objMethods;
                        }
                    }
                    else if (selectedTab == "Expressions") {
                        if ($scope.SelectedExp != undefined) {
                            selectedEntityItem = $scope.SelectedExp;
                        }
                    }
                    if (selectedEntityItem) {
                        var pathToObject = [];
                        //sObj = $scope.FindDeepNode($scope.objEntity, selectedEntityItem, pathToObject);
                        //pathString = getPathSource(sObj, indexPath);
                        //angular.copy(pathString.reverse(), nodeId);

                        sObj = $scope.FindDeepNode($scope.objEntity, selectedEntityItem, pathToObject);
                        pathString = $scope.getPathSource($scope.objEntity, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }
                }
                var strobj = JSON.stringify(objreturn1);
                if (strobj.length < 32000) {
                    hubMain.server.getEntityXmlObject(strobj, nodeId, $scope.currentfile);
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
                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "DesignToSourceEntity", nodeId);
                }
            }
            $scope.receivesourcexml = function (xmlstring, lineno) {
                $scope.$apply(function () {
                    $scope.xmlSource = xmlstring;
                    var ID = $scope.currentfile.FileName;
                    setDataToEditor($scope, xmlstring, lineno, ID);
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                    if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                        });
                    }
                    $rootScope.IsLoading = false;
                });
            };
        }
    };

    $scope.selectTextareaLine = function (txtArea, lineNum) {
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
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {

            angular.forEach(objParentElements.Elements, function (item) {
                //item.ParentVM = objParentElements;

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
                else if (item.Children && item.Children.length > 0) {
                    selectedChildItem = $scope.FindDeepNodeChildren(item, selectedItem);
                    if (selectedChildItem[0] == selectedItem) {
                        pathToObject.push(selectedChildItem[0]);
                        return selectedItem;
                    }
                }
            });
        }
        return selectedItem;
    };
    $scope.FindDeepNodeChildren = function (item, selectedItem) {
        var newItem;
        newItem = item.Children.filter(function (newChild) {
            return newChild == selectedItem;
        });
        return newItem;
    };
    $scope.getPathSource = function (objModel, pathToObject, indexPath) {
        for (var i = 0; i < pathToObject.length; i++) {
            if (i == 0) {
                var indx = objModel.Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
            else {
                var indx = pathToObject[i - 1].Elements.indexOf(pathToObject[i]);

                if (indx == -1) {
                    indx = pathToObject[i - 1].Children.indexOf(pathToObject[i]);
                }
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
                //break;
            }
            if (objParentElements.Elements[ele].Elements && objParentElements.Elements[ele].Elements.length > 0) {
                for (iele in objParentElements.Elements[ele].Elements) {
                    result = $scope.isValidObject(objParentElements.Elements[ele].Elements[iele], selectedItem);
                    if (result == true) {
                        return result;
                        //break;
                    }
                    //return result;
                }
            }
            else if (objParentElements.Elements[ele].Children && objParentElements.Elements[ele].Children.length > 0) {
                for (iele in objParentElements.Elements[ele].Children) {
                    result = $scope.isValidObject(objParentElements.Elements[ele].Children[iele], selectedItem);
                    if (result == true) {
                        return result;
                        //break;
                    }
                    //return result;
                }
            }
        }

        for (var chil in objParentElements.Children) {
            if (objParentElements.Children[chil] == selectedItem) {
                result = true;
                return result;
                //break;
            }
        }
        return result;
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
    //#endregion

    $scope.openQueryFromLogicalRule = function () {
        $scope.selectQueriesTab();
        for (var i = 0; i < $scope.objQueries.Elements.length; i++) {
            if ($scope.objQueries.Elements[i].dictAttributes.ID == $rootScope.queryID) {
                $scope.selectQuery($scope.objQueries.Elements[i], i);
                break;
            }
        }
        $rootScope.queryID = undefined;
    };

    //#region Business Object - Delete Section

    $scope.addToDeleteProperties = function () {

        var obj = {
            Name: "property", Value: "", Children: [], Elements: [], dictAttributes: {}
        };
        $rootScope.PushItem(obj, $scope.objDelete.Elements, "setSelectedDeleteProperty");
        $scope.setSelectedDeleteProperty(obj);
        $timeout(function () {
            var elem = $("#" + $scope.currentfile.FileName).find("#entity-businessobject-delete-section").find(".selected");
            if (elem) {
                if (elem.length > 0) {
                    elem[0].scrollIntoView();
                }
            }
        });
    };


    $scope.onChangeDeletePropertyId = function (obj) {

        if (obj && $scope.DeleteProperties && obj.dictAttributes && obj.dictAttributes.ID) {

            var delObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.objEntity.dictAttributes.ID, obj.dictAttributes.ID.trim());
            var busObjectID = null;
            var EntityName = null;
            if (delObject) {
                busObjectID = $scope.GetBusinessObject(delObject.DeclaringEntity);
                EntityName = delObject.DeclaringEntity;
            }

            if (delObject && delObject.Value && delObject.DeclaringEntity && $scope.blnShowCreateWithClass && $scope.DeleteProperties) {
                $.connection.hubEntityModel.server.getXmlMethodForeignKeyValues($scope.objEntity.dictAttributes.ID, busObjectID, delObject.DataType).done(function (data) {
                    if (data) {
                        obj.foreignKeys = data.Foreignkeys;
                        if (obj.foreignKeys && obj.foreignKeys.length > 0 && obj.foreignKeys.indexOf(obj.dictAttributes.sfwForeignKey) == -1) {
                            obj.dictAttributes.sfwForeignKey = "";
                        }
                        //$scope.receiveDeleteForeignKeyValues(data.Foreignkeys, data.ForeignkeyDefaultValue, obj.dictAttributes.ID);
                    }
                });
            } else if (!$scope.blnShowCreateWithClass) {
                var lstForeignKeyFields = [];
                if (EntityName) {
                    lstForeignKeyFields = $Entityintellisenseservice.GetIntellisenseData(EntityName, "Column", "Int", true, true, false, false, false, false);
                }
                $scope.receiveDeleteForeignKeyValues(lstForeignKeyFields, "", obj.dictAttributes.ID);
            } else {
                obj.foreignKeys = [];
                obj.dictAttributes.sfwForeignKey = "";
            }
        }
        else {
            obj.foreignKeys = [];
            obj.dictAttributes.sfwForeignKey = "";
        }
    };

    $scope.receiveDeleteForeignKeyValues = function (foreignKeys, defaultValue, astrID) {
        $scope.$apply(function () {
            var deleteProps = $scope.objDelete.Elements.filter(function (x) { return x.dictAttributes.ID == astrID; });
            if (deleteProps && deleteProps.length > 0) {
                angular.forEach(deleteProps, function (deleitem) {
                    deleitem.foreignKeys = foreignKeys;
                    //deleteProps[0].dictAttributes.sfwForeignKey = defaultValue; 
                });

            }
        });
    };
    $scope.setSelectedDeleteProperty = function (obj) {
        $scope.clearSelection();
        $scope.selectedDeleteProperty = obj;
        for (var i = 0; i < $scope.objDelete.Elements.length; i++) {
            if ($scope.objDelete.Elements[i].Name == "property") {
                if (obj == $scope.objDelete.Elements[i]) {
                    $scope.objDelete.Elements[i].isSelected = true;
                }
                else {
                    $scope.objDelete.Elements[i].isSelected = false;
                }
            }
        }
    };
    $scope.canRemoveDeleteProperty = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            return true;
        }
        return false;
    };
    $scope.removeDeleteProperty = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            var index = $scope.objDelete.Elements.indexOf(selectedItems[0]);
            var itemIndex = $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).indexOf(selectedItems[0]);
            if (index > -1 && itemIndex > -1) {
                if (confirm("DeleteID : '" + selectedItems[0].dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                    $rootScope.UndRedoBulkOp("Start");

                    $rootScope.DeleteItem(selectedItems[0], $scope.objDelete.Elements, "setSelectedDeleteProperty");

                    //Select next item
                    if ($scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).length > 0) {
                        if (itemIndex == $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).length) {
                            itemIndex--;
                        }
                        $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; })[itemIndex].isSelected = true;
                        //$scope.focusIndex = itemIndex;                                              
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
    };
    $scope.canMoveDeletePropertyUp = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            return $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).indexOf(selectedItems[0]) > 0;
        }
        return false;
    };
    $scope.moveDeletePropertyUp = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            $rootScope.UndRedoBulkOp("Start");
            var index = $scope.objDelete.Elements.indexOf(selectedItems[0]);
            $rootScope.DeleteItem(selectedItems[0], $scope.objDelete.Elements);
            $rootScope.InsertItem(selectedItems[0], $scope.objDelete.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            scrollBySelectedElement(false, "#entity-businessobject-delete-section", { offsetTop: 400, offsetLeft: 0 });
        }
    };
    $scope.canMoveDeletePropertyDown = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            return $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).indexOf(selectedItems[0]) < $scope.objDelete.Elements.filter(function (x) { return x.Name == "property"; }).length - 1;
        }
        return false;
    };
    $scope.moveDeletePropertyDown = function () {
        var selectedItems = $scope.objDelete.Elements.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            var index = $scope.objDelete.Elements.indexOf(selectedItems[0]);
            $rootScope.UndRedoBulkOp("Start");
            var index = $scope.objDelete.Elements.indexOf(selectedItems[0]);
            $rootScope.DeleteItem(selectedItems[0], $scope.objDelete.Elements);
            $rootScope.InsertItem(selectedItems[0], $scope.objDelete.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            scrollBySelectedElement(false, "#entity-businessobject-delete-section", { offsetTop: 400, offsetLeft: 0 });
        }
    };
    var scrollBySelectedElement = function (isDialog, div, settings) {
        $timeout(function () {
            if (isDialog == false) {
                var $divDom = $("#" + $scope.currentfile.FileName).find(div);
            } else {
                var $divDom = $(div);
            }
            $divDom.scrollTo($divDom.find(".selected"), settings, null);
        });
    }
    //#endregion

    //#region Business Object - Properties.cs Section

    $scope.setPropertyPrefix = function (obj) {
        obj.PropertyPrefix = GetDataTypePrefix(obj.DataType);
        if (obj.DataType && obj.DataType.trim() != "") {
            if (obj.DataType.toLowerCase() == "object") {
                if (obj.Class && obj.Class.trim() != "" && obj.Class.indexOf("bus") == 0) {
                    obj.PropertyPrefix = "ibus";
                }
                else if (obj.Class && obj.Class.trim() != "" && obj.Class.indexOf("utl") == 0) {
                    obj.PropertyPrefix = "iutl";
                }
                else {
                    obj.PropertyPrefix = "icdo";
                }
            }
            else if (obj.DataType.toLowerCase() == "list" || obj.DataType.toLowerCase() == "collection") {
                if (obj.Class && obj.Class.trim() != "" && obj.Class.indexOf("utl") == 0) {
                    obj.PropertyPrefix = "iclu";
                }
                else {
                    obj.PropertyPrefix = "iclb";
                }
            }
        }
        $scope.setPropertyID(obj);
    };
    $scope.setDescription = function (obj) {
        var strDesc = "Gets or sets the non-collection object of type "; // <bs>NCClassName<be>.
        if (obj.DataType && obj.DataType.trim().length > 0) {
            if (obj.DataType.toLowerCase() == "collection" || obj.DataType.toLowerCase() == "cdocollection" || obj.DataType.toLowerCase() == "list") {
                strDesc = "Gets or sets the collection object of type " + obj.Class;
            }
            else if (obj.DataType.toLowerCase() == "object") {
                strDesc = "Gets or sets the non-collection object of type " + obj.Class;
            }
            else {
                strDesc = "Gets or sets the non-collection object of type " + obj.DataType;
            }
        }

        obj.Description = strDesc;
    };
    $scope.onDataTypeChanged = function (obj) {
        obj.Class = "";
        $scope.setPropertyPrefix(obj);
        $scope.IsDirtyPartialFile = true;
        $scope.setDescription(obj);
        $scope.validateCSProperty(obj);
    };
    $scope.onClassNameChanged = function (obj) {
        $scope.setPropertyPrefix(obj);
        $scope.IsDirtyPartialFile = true;
        $scope.setDescription(obj);
        $scope.setDefaultPropertyName(obj);
        $scope.validateCSProperty(obj);
    };
    $scope.onPropertyNameChanged = function (obj) {
        $scope.IsDirtyPartialFile = true;
        $scope.setDescription(obj);
        $scope.setPropertyID(obj);
        $scope.validateCSProperty(obj);
    };
    $scope.setPropertyID = function (obj) {
        obj.PropertyID = obj.PropertyPrefix + obj.PropertyName;
    };

    $scope.setDefaultPropertyName = function (obj) {
        if (obj.Class && obj.Class.trim().length > 0 && (obj.Class.indexOf("bus") == 0 || obj.Class.indexOf("utl") == 0)) {
            obj.PropertyName = obj.Class.substring(3);
            $scope.setPropertyID(obj);
            $scope.validateCSPropertyPropertyName(obj);
        }
        else if (obj.Class && obj.Class.trim().length > 0 && obj.Class.indexOf("do") == 0) {
            obj.PropertyName = obj.Class.substring(2);
            $scope.setPropertyID(obj);
            $scope.validateCSPropertyPropertyName(obj);
        }
    };
    $scope.setSelectedCSProperty = function (index) {
        $scope.clearSelection();
        $scope.setCurrentCSProperty(index);

    };
    $scope.setSelectedCSPropertyUsingKeyUpDown = function (aObj) {
        var index = $scope.PartialFileData.indexOf(aObj);
        if (index > -1) {
            $scope.setSelectedCSProperty(index);
        }
    };


    // extracted this method, need to use for selection while adding row(Property CS )
    $scope.setCurrentCSProperty = function (index) {
        for (var i = 0; i < $scope.PartialFileData.length; i++) {
            if (i == index) {
                $scope.PartialFileData[i].isSelected = true;
            }
            else {
                $scope.PartialFileData[i].isSelected = false;
            }
        }
    };
    $scope.addCSProperty = function () {
        $scope.IsDirtyPartialFile = true;
        var obj = {
            DataType: "", Class: "", PropertyPrefix: "", PropertyName: "", PropertyID: "", Description: "", IsValid: true
        };
        $scope.validateCSProperty(obj);
        if ($scope.PartialFileData != undefined) {
            $rootScope.PushItem(obj, $scope.PartialFileData);
            //$scope.PartialFileData.push(obj);
        }
        // for selection of row while adding new Property
        var index = $scope.PartialFileData.indexOf(obj);
        if (index > 0) {
            $scope.setCurrentCSProperty(index);
        }
        $timeout(function () {
            var div = $('.entity-business-object-properties');
            if (div.length) {
                div.scrollTop(div.prop("scrollHeight"));
            }
        });
    };
    $scope.checkForPropertyFile = function () {
        if ($scope.BusObjPartialFileName == undefined) {
            return true;
        }
    };

    $scope.canDeleteCSProperty = function () {
        if ($scope.PartialFileData != undefined) {
            return $scope.PartialFileData.some(function (x) { return x.isSelected; });
        }
    };
    $scope.deleteCSProperty = function () {
        var selectedItems = $scope.PartialFileData.filter(function (x) { return x.isSelected == true; });
        if (selectedItems && selectedItems.length > 0) {
            var index = $scope.PartialFileData.indexOf(selectedItems[0]);
            if (index > -1) {
                if (confirm("CSProperty : '" + selectedItems[0].PropertyID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                    $rootScope.DeleteItem(selectedItems[0], $scope.PartialFileData);
                    //$scope.PartialFileData.splice(index, 1);
                    $scope.IsDirtyPartialFile = true;

                    //Select next item
                    if ($scope.PartialFileData.length > 0) {
                        if (index == $scope.PartialFileData.length) {
                            index--;
                        }
                        $scope.PartialFileData[index].isSelected = true;
                    }
                }
            }
        }
    };
    $scope.onClassNameKeyDown = function (event) {
        var input = $(event.target);
        if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                if ($scope.lstBusinessObjectForProperty && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                }
                event.preventDefault();
            } else {
                if ($scope.lstBusinessObjectForProperty) {
                    setSingleLevelAutoComplete(input, $scope.lstBusinessObjectForProperty);
                }
            }
        }
    };
    $scope.showClassNameList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        if (input && $scope.lstBusinessObjectForProperty) {
            input.focus();
            setSingleLevelAutoComplete(input, $scope.lstBusinessObjectForProperty);
            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
        }
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.validateCSProperties = function () {
        if ($scope.PartialFileData && angular.isArray($scope.PartialFileData)) {
            for (var index = 0; index < $scope.PartialFileData.length; index++) {
                $scope.validateCSProperty($scope.PartialFileData[index]);
            }
        }
    };
    $scope.validateCSProperty = function (obj) {
        $scope.validateCSPropertyDataType(obj);
        $scope.validateCSPropertyClass(obj);
        $scope.validateCSPropertyPropertyName(obj);
    };
    $scope.validateCSPropertyDataType = function (obj) {
        obj.DataTypeError = "";
        if (!(obj.DataType && obj.DataType.trim().length > 0)) {
            obj.DataTypeError = "Enter Data Type.";
        }
        else if ($scope.PartialFileDataTypes.indexOf(obj.DataType) == -1) {
            obj.DataTypeError = "Invalid Data Type.";
        }
        $scope.setIsValid(obj);
    };
    $scope.validateCSPropertyClass = function (obj) {
        obj.ClassNameError = "";
        if (obj.DataType && (obj.DataType.toLowerCase() == "object" || obj.DataType.toLowerCase() == "collection" || obj.DataType.toLowerCase() == "cdocollection" || obj.DataType.toLowerCase() == "list")) {
            if (!(obj.Class && obj.Class.trim().length > 0)) {
                obj.ClassNameError = "Enter Class Name.";
            }
            //else if ($scope.lstBusinessObjectForProperty && !$scope.lstBusinessObjectForProperty.some(function (x) { return x.ID == obj.Class; })) {
            //    obj.ClassNameError = "Invalid Class Name.";
            //}
        }
        $scope.setIsValid(obj);
    };
    $scope.validateCSPropertyPropertyName = function (obj) {
        obj.PropertyNameError = "";
        if (!(obj.PropertyName && obj.PropertyName.trim().length > 0)) {
            obj.PropertyNameError = "Enter Property Name.";
        }
        else if (!isValidIdentifier(obj.PropertyID)) {
            obj.PropertyNameError = "Invalid Property Name.";
        }
        else if ($scope.PartialFileData.filter(function (x) { return x.PropertyID == obj.PropertyID; }).length > 1) {
            obj.PropertyNameError = "Duplicate Property ID.";
        }
        $scope.setIsValid(obj);
    };
    $scope.setIsValid = function (obj) {
        obj.IsValid = true;
        if (obj.DataTypeError && obj.DataTypeError.trim().length > 0) {
            obj.IsValid = false;
        }
        if (obj.ClassNameError && obj.ClassNameError.trim().length > 0) {
            obj.IsValid = false;
        }
        if (obj.PropertyNameError && obj.PropertyNameError.trim().length > 0) {
            obj.IsValid = false;
        }
    };
    //#endregion

    //#region Common Methods
    $scope.BeforeSaveToFile = function () {
        if ($scope.objEntity && $scope.objEntity.dictAttributes) {

            if ($scope.IsDirtyPartialFile && $scope.BusObjPartialFileName && $scope.BusObjPartialFileName.trim().length > 0) {
                $.connection.hubEntityModel.server.savePartialBusObjectFile($scope.objEntity.dictAttributes.ID, $scope.BusObjPartialFileName, $scope.PartialFileData);
            }
            $scope.addExtraFieldsDataInToMainModel();
            $scope.CheckAndRemoveUnmappedColumnNode();

        }
    };

    $scope.AfterSaveToFile = function () {
        if ($scope.objEntity && $scope.objEntity.dictAttributes) {
            var entityid = $scope.objEntity.dictAttributes.ID;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            for (var i = 0; i < entityIntellisenseList.length; i++) {
                if (entityIntellisenseList[i].ID == entityid) {
                    entityIntellisenseList[i].Queries = [];
                    entityIntellisenseList[i].Attributes = [];
                    entityIntellisenseList[i].XmlMethods = [];
                    entityIntellisenseList[i].ObjectMethods = [];
                    entityIntellisenseList[i].ParentId = $scope.objEntity.dictAttributes.sfwParentEntity ? $scope.objEntity.dictAttributes.sfwParentEntity : "";
                    entityIntellisenseList[i].TableName = $scope.objEntity.dictAttributes.sfwTableName ? $scope.objEntity.dictAttributes.sfwTableName : "";
                    entityIntellisenseList[i].BusinessObjectName = $scope.objEntity.dictAttributes.sfwObjectID ? $scope.objEntity.dictAttributes.sfwObjectID : "";
                    entityIntellisenseList[i].ErrorTableName = $scope.objEntity.dictAttributes.sfwErrorTable ? $scope.objEntity.dictAttributes.sfwErrorTable : "";

                    angular.forEach($scope.objEntity.Elements, function (item) {
                        if (item.Name == "queries") {
                            angular.forEach(item.Elements, function (query) {
                                if (query.Name == "query") {
                                    var sql = query.dictAttributes.sfwSql;
                                    if ($rootScope.IsOracleDB) {
                                        sql = query.dictAttributes.sfwOracle;
                                    }
                                    var newquery = {
                                        ID: query.dictAttributes.ID, DisplayName: query.dictAttributes.ID, Value: query.dictAttributes.ID, Tooltip: query.dictAttributes.ID, QueryType: query.dictAttributes.sfwQueryType, DataType: query.dictAttributes.sfwDataType, Parameters: [], Columns: [], SqlQuery: sql,
                                        Entity: entityid, isQueryfieldsLoaded: false, lstQueryFields: []
                                    };


                                    angular.forEach(query.Elements, function (params) {
                                        if (params.Name == "parameters") {
                                            angular.forEach(params.Elements, function (param) {
                                                if (param.Name == "parameter") {
                                                    var parameter = {
                                                        ID: param.dictAttributes.ID, DisplayName: param.dictAttributes.ID, Value: param.dictAttributes.ID, Tooltip: param.dictAttributes.ID, DataType: param.dictAttributes.sfwDataType, Direction: param.dictAttributes.sfwDirection, Entity: entityid
                                                    };

                                                    newquery.Parameters.push(parameter);
                                                }
                                            });
                                        }
                                    });

                                    entityIntellisenseList[i].Queries.push(newquery);

                                }
                            });
                        }
                        else if (item.Name == "attributes") {
                            if ($scope.IsDirtyPartialFile && $scope.BusObjPartialFileName && $scope.BusObjPartialFileName.trim().length > 0) {
                                $scope.CheckAndAddNewlyAddedAttributes(item, $scope.PartialFileData, entityIntellisenseList);
                                $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
                                $scope.IsDirtyPartialFile = false;
                            }
                            $scope.RefreshIntellisenseAttributes(item);
                        }

                        else if (item.Name == "methods") {
                            angular.forEach(item.Elements, function (method) {
                                var objXmlMethod = {
                                    ID: method.dictAttributes.ID, MethodType: method.dictAttributes.sfwMethodType, Mode: method.dictAttributes.sfwMode, Items: [], Parameters: []
                                };
                                if (method.Elements.length > 0) {
                                    angular.forEach(method.Elements, function (obj) {
                                        if (obj.Name == "item") {
                                            var objItem = {
                                                ID: obj.dictAttributes.ID, ItemType: obj.dictAttributes.sfwItemType, LoadType: obj.dictAttributes.sfwLoadType, LoadSource: obj.dictAttributes.sfwLoadSource, SfwParameters: obj.dictAttributes.sfwParameter
                                            };
                                            objXmlMethod.Items.push(objItem);
                                        }
                                        else if (obj.Name == "parameter") {
                                            var objParam = { ID: obj.dictAttributes.ID, DataType: obj.dictAttributes.sfwDataType, Value: obj.dictAttributes.Value };
                                            objXmlMethod.Parameters.push(objParam);
                                        }
                                    });
                                }
                                entityIntellisenseList[i].XmlMethods.push(objXmlMethod);
                            });
                        }
                        else if (item.Name == "objectmethods") {
                            angular.forEach(item.Elements, function (method) {
                                var objXmlMethod = {
                                    ID: method.dictAttributes.ID, ReturnType: method.dictAttributes.sfwReturnType, Entity: method.dictAttributes.sfwEntity, Parameters: []
                                };
                                if (method.Elements.length > 0) {
                                    if (method.Elements[0].Elements.length > 0) {
                                        angular.forEach(method.Elements[0].Elements, function (obj) {
                                            if (obj.Name == "parameter") {
                                                var objParam = { ID: obj.dictAttributes.ID, DataType: obj.dictAttributes.sfwDataType, Entity: obj.dictAttributes.sfwEntity };
                                                objXmlMethod.Parameters.push(objParam);
                                            }
                                        });
                                    }
                                }
                                objXmlMethod.Type = "Method";
                                if (objXmlMethod.ID) {
                                    entityIntellisenseList[i].ObjectMethods.push(objXmlMethod);
                                }
                            });
                        }
                    });
                    break;
                }

            }
        }
    };

    $scope.RefreshIntellisenseAttributes = function (attributes) {
        var entityid = $scope.objEntity.dictAttributes.ID;
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        for (var i = 0; i < entityIntellisenseList.length; i++) {
            if (entityIntellisenseList[i].ID == entityid) {
                entityIntellisenseList[i].Attributes = [];
                angular.forEach(attributes.Elements, function (attribute) {
                    if (attribute && attribute.dictAttributes && attribute.dictAttributes.ID) {
                        var datatype = attribute.dictAttributes.sfwDataType;
                        if (attribute.dictAttributes.sfwType == "List" || attribute.dictAttributes.sfwType == "Collection" || attribute.dictAttributes.sfwType == "Object" || attribute.dictAttributes.sfwType == "CDOCollection") {
                            datatype = attribute.dictAttributes.sfwType;
                        }
                        var newattr = {

                            ID: attribute.dictAttributes.ID, DisplayName: attribute.dictAttributes.ID, Value: attribute.dictAttributes.sfwValue, Tooltip: attribute.dictAttributes.ID, DataType: datatype, Direction: "", Entity: attribute.dictAttributes.sfwEntity, IsPrivate: attribute.dictAttributes.sfwPrivate, Type: attribute.dictAttributes.sfwType, CodeID: attribute.dictAttributes.sfwCodeID, KeyNo: attribute.dictAttributes.sfwKeyNo, Caption: attribute.dictAttributes.sfwCaption, DeclaringEntity: entityid
                        };

                        entityIntellisenseList[i].Attributes.push(newattr);
                    }
                });
                break;
            }
        }
    };

    $scope.CheckAndAddNewlyAddedAttributes = function (lstAttributes, Properties, entityIntellisenseList) {
        $rootScope.UndRedoBulkOp("Start");
        for (var i = 0; i < Properties.length; i++) {
            var isObjectFound = false;
            var PropertyID = Properties[i].PropertyID;
            for (var j = 0; j < lstAttributes.Elements.length; j++) {
                if (lstAttributes.Elements[j].dictAttributes.sfwValue == PropertyID) {
                    isObjectFound = true;
                    break;
                }
            }

            if (!isObjectFound) {
                if (Properties[i].DataType == "Object" && Properties[i].Class.indexOf("utl") !== 0) {
                    var EntityID = $scope.GetEntityIdBasedOnBusinessObject(entityIntellisenseList, Properties[i].Class);
                    if (EntityID) {
                        var newOneToOne = {
                        };
                        newOneToOne.Value = '';
                        newOneToOne.Name = 'attribute';
                        newOneToOne.Elements = [];
                        var ID = "obj" + Properties[i].PropertyName;
                        newOneToOne.dictAttributes = {
                            ID: ID, sfwType: 'Object', sfwEntity: EntityID, sfwPrivate: 'False', sfwValue: Properties[i].PropertyID
                        };

                        $rootScope.PushItem(newOneToOne, lstAttributes.Elements, null);
                    }
                } else if (Properties[i].DataType == "Collection" && Properties[i].Class.indexOf("utl") !== 0) {
                    var EntityID = $scope.GetEntityIdBasedOnBusinessObject(entityIntellisenseList, Properties[i].Class);
                    if (EntityID) {
                        var newOneToMany = {
                        };
                        newOneToMany.Value = '';
                        newOneToMany.Name = 'attribute';
                        newOneToMany.Elements = [];
                        var ID = "lst" + Properties[i].PropertyName;
                        newOneToMany.dictAttributes = {
                            ID: ID, sfwType: 'Collection', sfwEntity: EntityID, sfwValue: Properties[i].PropertyID, sfwPrivate: 'False', Text: ''
                        };
                        $rootScope.PushItem(newOneToMany, lstAttributes.Elements, null);
                    }
                } else {
                    var newProperty = {
                    };
                    newProperty.Value = '';
                    newProperty.Name = 'attribute';
                    newProperty.Elements = [];
                    var ID = Properties[i].PropertyName;
                    newProperty.dictAttributes = {
                        ID: ID, sfwDataType: Properties[i].DataType, sfwDataFormat: '', sfwPrivate: false, sfwValue: Properties[i].PropertyID, sfwEnumerator: '', Text: '', sfwType: "Property"
                    };
                    $rootScope.PushItem(newProperty, lstAttributes.Elements, null);
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.GetEntityIdBasedOnBusinessObject = function (lstEntities, busID) {
        var Entityname = "";
        for (var i = 0; i < lstEntities.length; i++) {
            if (lstEntities[i].BusinessObjectName == busID) {
                Entityname = lstEntities[i].EntityName;
                break;
            }
        }
        return Entityname;
    };

    $scope.LoadParentForObject = function (objParent) {
        angular.forEach(objParent.Elements, function (item) {
            if (objParent.Name == 'harderror' || objParent.Name == 'softerror' || objParent.Name == 'groupslist' || objParent.Name == 'group') {
                item.ParentID = objParent.dictAttributes.ID;
            }

            else {
                item.ParentID = objParent.dictAttributes.ID;
            }


            if (item.Elements.length > 0) {
                $scope.LoadParentForObject(item);
            }

            if (item.Children.length > 0) {
                $scope.LoadParentForObject(item);
            }
        });
        angular.forEach(objParent.Children, function (item) {
            item.ParentID = objParent.dictAttributes.ID;
            if (item.Children.length > 0) {
                $scope.LoadParentForObject(item);
            }

            if (item.Elements.length > 0) {
                $scope.LoadParentForObject(item);
            }
        });
    };

    $scope.setObjectEditable = function (obj) {
        obj.isEditable = true;
    };
    $scope.setObjectReadOnly = function (obj) {
        obj.isEditable = false;
    };
    $scope.checkAndSetRowReadOnly = function (eargs, obj) {
        if (eargs.keyCode == $.ui.keyCode.ESCAPE) {
            $scope.setObjectReadOnly(obj);
        }
    };
    $scope.setAllObjectReadOnlyExceptThis = function (obj) {
        angular.forEach($scope.objAttributes.Elements.filter(function (item) { return item != obj; }), function (attr) {
            $scope.setObjectReadOnly(attr);
        });
    };

    $scope.GetRuleByName = function (ruleName) {
        var objRule;
        if ($scope.selectedIneritedTab == "InheritedRule") {
            if ($scope.objInheritedRules && $scope.objInheritedRules.Elements.length > 0) objRule = $scope.FindRuleById($scope.objInheritedRules, ruleName);
        }
        else if ($scope.objRules && $scope.objRules.Elements.length > 0) {
            objRule = $scope.FindRuleById($scope.objRules, ruleName);
        }
        return objRule;
    };

    $scope.FindRuleById = function (obj, ruleName) {
        var objRule;


        angular.forEach(obj.Elements, function (itm) {
            if (!objRule) {
                if (itm.dictAttributes.ID == ruleName) {
                    objRule = itm;
                }
                if (itm.Elements.length > 0 && !objRule) {
                    $scope.FindRuleById(itm, ruleName);
                }
            }
        });
        return objRule;
    };

    $scope.onkeydownInExpressions = function (event) {
        if (event && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            event.preventDefault();
        }
    };

    $scope.onActionKeyDownForExpressions = function (eargs, isObjectBased, loadAttribute, loadConstants, loadRFunc) {

        if (!$scope.objBusinessObject.ObjTree && isObjectBased != undefined && isObjectBased.toString().toLowerCase() == "true") {
            $.connection.hubEntityModel.server.loadObjectTree($scope.objEntity.dictAttributes.sfwObjectID, "", true).done(function (data) {
                $scope.receiveObjectTree(data);
                $scope.IntellisenseonActionKeyDownForExpressions(eargs, isObjectBased, loadAttribute, loadConstants, loadRFunc);
            });
        }
        else {
            $scope.IntellisenseonActionKeyDownForExpressions(eargs, isObjectBased, loadAttribute, loadConstants, loadRFunc);
        }
    };

    $scope.IntellisenseonActionKeyDownForExpressions = function (eargs, isObjectBased, loadAttribute, loadConstants, loadRFunc) {
        var input = eargs.target;
        var arrText = getSplitArray($(input).val(), input.selectionStart);
        if (arrText[arrText.length - 1] == "") {
            arrText.pop();
        }
        if (isObjectBased != undefined && isObjectBased.toString().toLowerCase() == "true") {
            isObjectBased = "true";
        }
        else {
            isObjectBased = "false";
        }
        var propertyName = "ID";
        var data = $scope.getFirstLevelIntellisenseForExpressions(isObjectBased && isObjectBased.toLowerCase() == "true",
            loadAttribute == false ? false : true,
            loadConstants == false ? false : true,
            loadRFunc == false ? false : true);
        var alreadySet = false;
        if (arrText.length > 0) {
            for (var index = 0; index < arrText.length; index++) {
                var item = data.filter(function (x) {
                    return (x.ID && x.ID == arrText[index]) || (x.ShortName && x.ShortName == arrText[index]);
                });
                if (item.length > 0) {
                    if (item[0].ID == "this") {
                        if ($scope.objBusinessObject.ObjTree) {
                            data = $scope.objBusinessObject.ObjTree.ChildProperties;
                            data = data.concat($scope.objBusinessObject.ObjTree.lstMethods);
                        }
                        var propertyName = "ShortName";
                    }
                    else if (item[0].ID == "RFunc") {
                        data = $resourceFactory.getRfunctionsList();
                        var propertyName = "ID";
                    }
                    else if (item[0].isCustomType) {
                        hubMain.server.getCustomTypeMethods(item[0].ID).done(function (customTypeMethods) {
                            data = customTypeMethods;
                            $scope.setIntellisenseData(data, input, eargs, propertyName);
                        });
                        alreadySet = true;
                    }
                    else if (item[0].ItemType && item[0].ShortName) {
                        if (item[0].DataType == "TableObjectType" || item[0].DataType == "CollctionType" || item[0].DataType == "BusObjectType" || item[0].DataType == "OtherReferenceType") {
                            if (!item[0].HasLoadedProp || item[0].HasLoadedProp == undefined) {
                                var txt = arrText.join(".");
                                if (txt.indexOf("this.") == 0) {
                                    txt = txt.substring(5);
                                }
                                $.connection.hubEntityModel.server.loadObjectTree(item[0].ItemType.Name, item[0].FullPath, true).done(function (data) {
                                    var fullPath = "";
                                    if (item && item.length > 0 && item[0].FullPath) {
                                        fullPath = item[0].FullPath;
                                    }
                                    $scope.receiveObjectTree(data, fullPath);
                                });
                                data = [];
                            }
                            else {
                                data = item[0].ChildProperties;
                                data = data.concat(item[0].lstMethods);
                            }
                        }
                        else {
                            data = [];
                        }
                        var propertyName = "ShortName";
                    }
                    else {
                        if (typeof item[0].DataType != "undefined" && item[0].DataType == "Object" && typeof item[0].Entity != "undefined") {
                            entityName = item[0].Entity;
                            data = $rootScope.getEntityAttributeIntellisense(entityName, true);
                        }
                        else if (item[0].Type) {
                            if (item[0].Type == "Constant") {
                                data = $resourceFactory.getConstantsList(arrText.join("."));
                                break;
                            }
                            else {
                                data = [];
                            }
                        }
                        else {
                            data = [];
                        }
                        var propertyName = "ID";
                    }
                }
            }
        }

        if (!alreadySet) {
            $scope.setIntellisenseData(data, input, eargs, propertyName);
        }
    };

    $scope.setIntellisenseData = function (data, input, eargs, propertyName) {
        setMultilevelAutoCompleteForObjectTreeIntellisense($(input), data, propertyName, undefined, $scope);
        if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            eargs.preventDefault();
        }
    }
    $scope.showList = function (event, isObjectBased, loadAttribute, loadConstants, loadRFunc) {
        var inputElement;

        inputElement = $(event.target).prevAll("input[type='text']");
        if (inputElement && inputElement.length == 0) { // If it is textarea then select textarea using id textarea-expression
            inputElement = $("#textarea-expression");
        }
        inputElement.focus();
        if (inputElement) {
            if (isObjectBased != undefined && isObjectBased.toString().toLowerCase() == "true") {
                isObjectBased = "true";
            }
            else {
                isObjectBased = "false";
            }
            var propertyName = "ID";
            var data = $scope.getFirstLevelIntellisenseForExpressions(isObjectBased && isObjectBased.toLowerCase() == "true",
                loadAttribute == false ? false : true,
                loadConstants == false ? false : true,
                loadRFunc == false ? false : true);
            setSingleLevelAutoComplete(inputElement, data);
        }
        if (inputElement && data && $(inputElement).data('ui-autocomplete')) {
            $(inputElement).autocomplete("search", $(inputElement).val().trim());
        }
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.getFirstLevelIntellisenseForExpressions = function (isObjectBased, loadAttributes, loadConstants, loadRFunc) {
        var data = [];
        if (loadConstants == undefined || loadConstants == true) {
            data = data.concat($resourceFactory.getConstantsList());
        }
        if (loadRFunc == undefined || loadRFunc == true) {
            data.push({
                ID: "RFunc", DisplayName: "RFunc", Value: "RFunc", Tooltip: "RFunc"
            });
            if ($scope.customTypes && $scope.customTypes.length > 0) {
                for (var idx = 0, len = $scope.customTypes.length; idx < len; idx++) {
                    data.push({ ID: $scope.customTypes[idx], DisplayName: $scope.customTypes[idx], Value: $scope.customTypes[idx], Tooltip: $scope.customTypes[idx], isCustomType: true });
                }
            }

            //data = data.concat($resourceFactory.getRfunctionsList());
        }

        if (isObjectBased && $scope.objEntity.dictAttributes.sfwObjectID && $scope.objEntity.dictAttributes.sfwObjectID.trim().length > 0) {
            data.push({
                ID: "this", DisplayName: "this", Value: "this", Tooltip: "this"
            });
        }
        else {
            if (loadAttributes == undefined || loadAttributes == true) {
                var entityID = $scope.objEntity.dictAttributes.ID;
                var attrs = $EntityIntellisenseFactory.getAttributes(entityID, null, null, true);
                data = data.concat(attrs);
            }
        }
        return data;
    };

    //#endregion

    //#region Intellisense Data Loading

    $scope.receiveObjectTree = function (data, path) {
        $scope.$evalAsync(function () {
            data = JSON.parse(data);
            var obj = data;

            if (path != undefined && path != "") {
                var busObject = getBusObjectByPath(path, $scope.objBusinessObject.ObjTree);
                if (busObject && busObject.ItemType.Name == obj.ObjName) {
                    busObject.ChildProperties = obj.ChildProperties;
                    busObject.lstMethods = obj.lstMethods;
                    busObject.HasLoadedProp = true;
                }
                SetParentForObjTreeChild(busObject);
            }
            else {
                $scope.objBusinessObject.ObjTree = obj;
                //Instead of re-initializing the array, clearing it so that it won't 
                //loose the reference which we are already using while creating new xml method.
                if ($scope.lstobjecttree.length > 0) {
                    $scope.lstobjecttree.splice(0, $scope.lstobjecttree.length);
                }
                $scope.lstobjecttree.push($scope.objBusinessObject.ObjTree);
                $scope.objBusinessObject.ObjTree.IsMainBusObject = true;
                $scope.objBusinessObject.ObjTree.IsVisible = true;
                SetParentForObjTreeChild($scope.objBusinessObject.ObjTree);


                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var objEntityIntellisense = entityIntellisenseList.filter(function (entity) {
                    return entity.ID == $scope.objEntity.dictAttributes.ID;
                });

                if (objEntityIntellisense != undefined && objEntityIntellisense.length > 0 && objEntityIntellisense[0].Rules.length > 0) {
                    $scope.objBusinessObject.ObjTree.Rules = objEntityIntellisense[0].Rules;
                }
            }
        });
    };

    $scope.recieveGetCodeValues = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstCodeValue = JSON.parse(data);
        });
    };

    $scope.recieveGetPopulateErrorTables = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstErrorTable = data;
        });
    };

    $scope.recieveGetPopulateChecklistTables = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstChecklistTable = data;
        });
    };

    $scope.recieveGetPopulateExpressionTables = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstExpressionTable = data;
        });
    };
    //#endregion

    //#region Columns Section


    $scope.getBoolValue = function (value) {
        if (value != undefined && value != "") {
            if (value == "True" || value == "true") {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }

    };

    $scope.getView = function (value) {
        var isFound = false;
        if (value && value.toLowerCase().contains("id")) {
            var id = value.replace("id", "value");
            for (var i = 0; i < $scope.objAttributes.Elements.length; i++) {
                if (id.toLowerCase() == $scope.objAttributes.Elements[i].dictAttributes.sfwValue) {
                    isFound = true;
                    break;
                }
            }
        }

        return isFound;
    };

    //#endregion

    //#region Fields Constraints    

    $scope.AddFieldConstraintFromDashBoard = function (objFieldConstraint, flag) {
        var newScope = $scope.$new();
        if ($scope.objEntity.dictAttributes && $scope.objEntity.dictAttributes.ID) {
            newScope.entityName = $scope.objEntity.dictAttributes.ID;
        }
        newScope.model = {
            Children: [], dictAttributes: {
            }, Elements: [], Name: "item", Value: ''
        };
        newScope.model.errors = {};
        if (!objFieldConstraint && newScope.model.errors) {
            newScope.model.errors.sfwFieldName = CONST.VALIDATION.EMPTY_FIELD;
        }
        newScope.model.errors.empty_id = CONST.VALIDATION.ID_EMPTY;
        newScope.onCancelClick = function () {
            dialog.close();
        };
        newScope.OkClick = function () {
            delete newScope.model.errors;
            if (flag == 'Add') {
                $rootScope.PushItem(newScope.model, $scope.objConstraints.Elements, "SelectFieldConstraint");
                $scope.SelectFieldConstraint(newScope.model);
                $timeout(function () {
                    var div = $(".constraint-container");
                    if (div.length) {
                        div.scrollTop(div.prop("scrollHeight"));
                    }
                });
            }
            var newaddconstraintsfwDisplayName = newScope.model.dictAttributes.sfwDisplayName;
            if (!newaddconstraintsfwDisplayName) {
                newaddconstraintsfwDisplayName = newScope.model.dictAttributes.sfwFieldName;
            }
            newScope.onCancelClick();
            $scope.scrollToPosition("#constraints", ".entity-dashboard-inner-section-subcontainer", newaddconstraintsfwDisplayName);
        };
        var strtitle = "Edit Field Constraint";
        if (flag == "Add") {
            strtitle = "Add Field Constraint";
        }
        var dialog = $rootScope.showDialog(newScope, strtitle, "Entity/views/Constraint/AddFieldConstraint.html", {
            width: 665, height: 670
        });
    };

    $scope.Operator = ['', 'Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'GreaterThan', 'GreaterThanEqual'];
    $scope.addEditAttributeConstraint = function (attributeName) {
        addEditAttributeConstraint($scope, attributeName, $scope.objEntity, $scope.objEntity.dictAttributes.ID);
    }

    //Validate operator from Add FieldConstraint dialog
    $scope.validateOperator = function (AddFieldConstraintModel, sfwOperatorVal) {
        if (sfwOperatorVal == "" && AddFieldConstraintModel.dictAttributes.sfwRelatedField != "") {
            if (!AddFieldConstraintModel.errors && !angular.isObject(AddFieldConstraintModel.errors)) {
                AddFieldConstraintModel.errors = {};
            }
            if (!AddFieldConstraintModel.errors.sfwOperator) {
                AddFieldConstraintModel.errors.sfwOperator = CONST.VALIDATION.EMPTY_FIELD;
            }
        }
        else {
            if (AddFieldConstraintModel.errors && AddFieldConstraintModel.errors.sfwOperator) {
                delete AddFieldConstraintModel.errors.sfwOperator;
            }
        }
    };
    $scope.SelectFieldConstraint = function (obj, isScroll) {
        if ($scope.objConstraints.SelectedField) {
            if ($scope.objConstraints.SelectedField != obj) {
                $scope.objConstraints.SelectedField.IsFieldConstraintDetailVisibility = false;
            }
        }
        $scope.clearSelection();
        $scope.objConstraints.SelectedField = obj;
        if (isScroll) {
            if (obj.dictAttributes.sfwDisplayName) {
                $scope.scrollToPosition("#constraints", ".entity-dashboard-inner-section-subcontainer", obj.dictAttributes.sfwDisplayName);
            } else {
                $scope.scrollToPosition("#constraints", ".entity-dashboard-inner-section-subcontainer", obj.dictAttributes.sfwFieldName);
            }
        }
    };

    $scope.SelectFieldConstraintdbclick = function (objConstraint) {
        $scope.selectFieldConstraintsTab();
        $scope.SelectFieldConstraint(objConstraint, false);
        if (objConstraint) {
            $scope.activeGridRow = objConstraint;
        }
    };

    $scope.DeleteFieldConstraint = function (selectedTabName) {
        var Fieldindex = -1;
        if ($scope.objConstraints.SelectedField) {
            Fieldindex = $scope.objConstraints.Elements.indexOf($scope.objConstraints.SelectedField);
            var fieldName = $scope.objConstraints.SelectedField.dictAttributes.sfwDisplayName ? $scope.objConstraints.SelectedField.dictAttributes.sfwDisplayName : $scope.objConstraints.SelectedField.dictAttributes.sfwFieldName;
            if (confirm("FieldConstraint : '" + fieldName + "'" + "  " + " will be deleted, Do you want to continue?")) {
                if (Fieldindex > -1) {
                    $rootScope.DeleteItem($scope.objConstraints.Elements[Fieldindex], $scope.objConstraints.Elements, selectedTabName);
                    //$scope.objConstraints.Elements.splice(Fieldindex, 1);

                    if (selectedTabName) {
                        if (Fieldindex < $scope.objConstraints.Elements.length) {
                            $scope.objConstraints.SelectedField = $scope.objConstraints.Elements[Fieldindex];
                        }
                        else if ($scope.objConstraints.Elements.length > 0) {
                            $scope.objConstraints.SelectedField = $scope.objConstraints.Elements[Fieldindex - 1];
                        }
                        else {
                            $scope.objConstraints.SelectedField = undefined;
                        }
                    } else {
                        $scope.clearSelection();
                    }
                    $("#tblFieldConstraint").css('height', 'auto');
                }

            }
        }
    };

    $scope.onFieldNameChanged = function (contraint) {
        var isDataTypeSet = false;
        $ValidationService.validateID(contraint, undefined, contraint.dictAttributes.sfwFieldName);
        if (contraint.dictAttributes.sfwFieldName && contraint.dictAttributes.sfwFieldName.trim().length > 0 && $scope.objEntity.dictAttributes.ID) {
            var entityFields = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.objEntity.dictAttributes.ID, contraint.dictAttributes.sfwFieldName);
            if (entityFields && entityFields.DataType) {
                $scope.$evalAsync(function () {
                    contraint.dictAttributes.sfwDataType = entityFields.DataType;
                });
                isDataTypeSet = true;
            }
        }
        if (!isDataTypeSet) {
            contraint.dictAttributes.sfwDataType = "";
        }
    };

    //#endregion

    //#region Business Object - Details Section
    $scope.DetailsMode = ['', 'All', 'New', 'Update'];

    $scope.PopulateLoadTypes = function () {
        $scope.lstLoadTypes = ['None', 'ForeignKey', 'Query'];
    };

    $scope.onMainCdoChange = function (event) {
        var input = $(event.target);
        if ($scope.tableObjects) {
            setSingleLevelAutoComplete(input, $scope.tableObjects, $scope);
        }
        if (event && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $scope.tableObjects) {
            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        }
    };

    $scope.showMainCdoIntellisenseList = function (event) {
        var inputElement;
        inputElement = $(event.target).prevAll("input[type='text']");
        inputElement.focus();
        if (inputElement && $scope.tableObjects) {
            setSingleLevelAutoComplete(inputElement, $scope.tableObjects, $scope);
            if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
        }
        if (event) {
            event.stopPropagation();
        }
    };

    //#endregion
    $scope.AddObjectMethod = function (method, flag) {
        var newObjMethodScope = $scope.$new();
        newObjMethodScope.IsUpdate = false;
        newObjMethodScope.objObjectMethods = $scope.objObjectMethods;
        var title = "Add Object Method";
        var dialog = $rootScope.showDialog(newObjMethodScope, title, "Entity/views/UpdateObjectMethod.html", {
            width: 1070, height: 450
        });
        if (flag == "Add") {
            //newObjMethodScope.ErrorMessageForDisplay = "Name cannot be empty.";
            newObjMethodScope.model = {
                Children: [], dictAttributes: { ID: '', sfwReturnType: '' }, Elements: [], Name: 'method', Value: ''
            };
            $ValidationService.validateID(newObjMethodScope.model, $scope.validationErrorList, newObjMethodScope.model.dictAttributes.ID);
        }
        else {
            title = "Edit Object Method";
            newObjMethodScope.IsUpdate = true;
            // var methodCopy = method;
            var methodCopy = JSON.parse(JSON.stringify(method));
            newObjMethodScope.model = methodCopy;
        }
        newObjMethodScope.onCancelClick = function () {
            if (method && flag == "Update" && newObjMethodScope.model.errors && $.isEmptyObject(newObjMethodScope.model.errors)) {
                $ValidationService.removeObjInToArray($scope.validationErrorList, method);
            } else if (flag == "Add" && newObjMethodScope.model) {
                $ValidationService.removeObjInToArray($scope.validationErrorList, newObjMethodScope.model);
            }
            dialog.close();
        };
        newObjMethodScope.OkClick = function () {
            if (flag == "Add") {
                $rootScope.PushItem(newObjMethodScope.model, $scope.objObjectMethods.Elements, "SelectXmlMethod");
                $scope.SelectObjectMethod(newObjMethodScope.model);
            }
            else if (flag == "Update") {
                var index = $scope.objObjectMethods.Elements.indexOf(method);
                $scope.objObjectMethods.Elements[index] = newObjMethodScope.model;
                $scope.SelectObjectMethod(newObjMethodScope.model);
            }
            newObjMethodScope.onCancelClick();
        };
    }

    $scope.SelectObjectMethod = function (method) {
        if (method) {
            $scope.clearSelection();
        }


        for (var i = 0; i < $scope.objObjectMethods.Elements.length; i++) {
            if ($scope.objObjectMethods.Elements[i] == method) {
                $scope.objObjectMethods.Elements[i].selected = true;
                $scope.SelectedObjectMethod = $scope.objObjectMethods.Elements[i];
            }
            else {
                $scope.objObjectMethods.Elements[i].selected = false;
            }
        }
    };
    //#region Business Object - Methods Section

    $scope.AddMethod = function (obj, flag) {

        var newMethodScope = $scope.$new();
        newMethodScope.IsUpdate = false;
        newMethodScope.lstobjecttree = $scope.lstobjecttree;
        var title = "Add Business Object Method";

        if (flag == "Add") {
            newMethodScope.ErrorMessageForDisplay = "Name cannot be empty.";
            newMethodScope.obj = {
                Children: [], dictAttributes: { ID: '', sfwDescription: '', sfwMethodType: 'Other', sfwMode: '' }, Elements: [], Name: 'method', Value: ''
            };
        }
        else {
            title = "Edit Business Object Method";
            newMethodScope.IsUpdate = true;
            newMethodScope.obj = obj;
            for (var i = 0; i < newMethodScope.obj.Elements.length; i++) {
                if (newMethodScope.obj.Elements[i].Name == "item") {
                    newMethodScope.obj.Elements[i].isSelected = false;
                }
            }
        }
        var dialog = $rootScope.showDialog(newMethodScope, title, "Entity/views/AddBusinessObjectMethod.html", {
            width: 1070, height: 450
        });

        newMethodScope.canOkClick = function () {
            var retVal = false;
            newMethodScope.ErrorMessageForDisplay = "";
            if (newMethodScope.obj.dictAttributes.ID == undefined || newMethodScope.obj.dictAttributes.ID == "") {
                retVal = true;
                newMethodScope.ErrorMessageForDisplay = "Name cannot be empty.";
            }
            else if (!isValidIdentifier(newMethodScope.obj.dictAttributes.ID, false)) {
                retVal = true;
                newMethodScope.ErrorMessageForDisplay = "Invalid Name.";
            }
            else {

                var lstMethods = getMethodsModel("xmlmethod");
                var lst = lstMethods.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID && newMethodScope.obj.dictAttributes.ID && itm.dictAttributes.ID.toLowerCase() == newMethodScope.obj.dictAttributes.ID.toLowerCase();
                });
                if (lst && lst.length > 0) {
                    if (newMethodScope.IsUpdate) {
                        if (lst.length > 1) {
                            retVal = true;
                            newMethodScope.ErrorMessageForDisplay = "Duplicate Name.";
                        }
                    }
                    else {
                        if (lst.length > 0) {
                            retVal = true;
                            newMethodScope.ErrorMessageForDisplay = "Duplicate Name.";
                        }
                    }
                }
            }

            return retVal;
        };

        newMethodScope.onOkClick = function () {
            if (flag == "Add") {
                $rootScope.PushItem(newMethodScope.obj, $scope.objMethods.Elements, "SelectXmlMethod");
                $scope.SelectXmlMethod(newMethodScope.obj);
            }
            $scope.$evalAsync(function () {
                checkDuplicateIdForMethod(true);
                validateXMLMethod();
            });
            newMethodScope.onCancelClick();

            $timeout(function () {
                var elem = $("#" + $scope.currentfile.FileName).find("#entity-xml-method-section").find("span").filter(function () {
                    return $(this).text() === newMethodScope.obj.dictAttributes.ID;
                });
                elem = $(elem)[0];
                if (elem) {
                    elem.scrollIntoView();
                }
            });
            //$scope.scrollToPosition("#entity-xml-method-section", ".entity-business-object-box-content span", newMethodScope.obj.dictAttributes.ID);
        };
        newMethodScope.ChangeMethodType = function (obj, bool) {
            if (obj.dictAttributes.sfwMethodType == 'Other') {
                obj.dictAttributes.sfwMode = '';
            }
        };
        newMethodScope.setXmlMethodNavigationParameter = function (selectedXmlMethod) {
            $scope.xmlMethodParameters = selectedXmlMethod.Elements.filter(function (x) { return x.Name == "parameter"; }).map(function (y) {
                return {
                    ParamID: y.dictAttributes.ID, IsConstant: y.dictAttributes.Value != undefined && y.dictAttributes.Value.indexOf("#") == 0, Value: y.dictAttributes.Value != undefined && y.dictAttributes.Value.indexOf("#") == 0 ? y.dictAttributes.Value.substring(1) : y.dictAttributes.Value
                };
            });
            var newScope = $scope.$new(true);
            newScope.xmlMethodParameters = $scope.xmlMethodParameters;
            newScope.lstobjecttree = $scope.lstobjecttree;
            newScope.ObjTree = $scope.objBusinessObject.ObjTree;
            newScope.StrXmlMethodObjectTreePath = $scope.StrXmlMethodObjectTreePath;
            newScope.searchXmlMethodObjectTree = $scope.searchXmlMethodObjectTree;
            newScope.objEntity = $scope.objEntity;
            newScope.StrXmlMethodObjectTreeText = $scope.StrXmlMethodObjectTreeText;
            newScope.StrXmlMethodObjectTreePath = $scope.StrXmlMethodObjectTreePath;
            newScope.StrObjectTreeText = $scope.StrObjectTreeText;
            newScope.StrObjectTreePath = $scope.StrObjectTreePath;
            newScope.isParamAlreadySet = newScope.isParamAlreadySet;
            newScope.setnavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Entity/views/SetNavigationParameters.html", {
                width: 830, height: 670
            });
            newScope.onSetNavParamOKClick = function () {
                for (var index = 0; index < newScope.xmlMethodParameters.length; index++) {
                    var xmlParameters = selectedXmlMethod.Elements.filter(function (x) {
                        return x.Name == "parameter" && x.dictAttributes.ID == newScope.xmlMethodParameters[index].ParamID;
                    });
                    if (xmlParameters && xmlParameters.length > 0) {
                        if (newScope.xmlMethodParameters[index].IsConstant) {
                            if (newScope.xmlMethodParameters[index].Value) {
                                $rootScope.EditPropertyValue(xmlParameters[0].dictAttributes.Value, xmlParameters[0].dictAttributes, "Value", "#" + newScope.xmlMethodParameters[index].Value);
                            } else {
                                $rootScope.EditPropertyValue(xmlParameters[0].dictAttributes.Value, xmlParameters[0].dictAttributes, "Value", "");
                            }
                        } else {
                            $rootScope.EditPropertyValue(xmlParameters[0].dictAttributes.Value, xmlParameters[0].dictAttributes, "Value", newScope.xmlMethodParameters[index].Value ? newScope.xmlMethodParameters[index].Value : "");
                        }
                    }
                }

                $scope.xmlMethodParameters = undefined;
                newScope.xmlMethodParameters = undefined;
                newScope.onSetNavParamOKClick = undefined;
                newScope.setnavigationParameterDialog.close();
            };

        };
        newMethodScope.moveBusMethodDown = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    var index = selectedXmlMethod.Elements.indexOf(selectedItems[0]);
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(selectedItems[0], selectedXmlMethod.Elements);

                    while (selectedXmlMethod.Elements[index].Name == "parameter")
                        index++;
                    $rootScope.InsertItem(selectedItems[0], selectedXmlMethod.Elements, index + 1);
                    $rootScope.UndRedoBulkOp("End");
                    scrollBySelectedElement(true, ".entity-xml-addmethodlist", { offsetTop: 300, offsetLeft: 0 });
                }
            }
        };
        newMethodScope.canMoveBusMethodUp = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    return selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) > 0;
                }
            }
            return false;
        };

        newMethodScope.moveBusMethodUp = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    var index = selectedXmlMethod.Elements.indexOf(selectedItems[0]);
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(selectedItems[0], selectedXmlMethod.Elements);

                    while (selectedXmlMethod.Elements[index - 1].Name == "parameter")
                        index--;

                    $rootScope.InsertItem(selectedItems[0], selectedXmlMethod.Elements, index - 1);
                    $rootScope.UndRedoBulkOp("End");
                    scrollBySelectedElement(true, ".entity-xml-addmethodlist", { offsetTop: 300, offsetLeft: 0 });
                }
            }
        };
        newMethodScope.canMoveBusMethodDown = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    return selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) < selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).length - 1;
                }
            }
            return false;
        };
        newMethodScope.canDeleteBusMethod = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    return true;
                }
            }
            return false;
        };
        newMethodScope.deleteBusMethod = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var selectedItems = selectedXmlMethod.Elements.filter(function (x) {
                    return x.isSelected == true;
                });
                if (selectedItems && selectedItems.length > 0) {
                    if (!selectedItems[0].dictAttributes.ID) {
                        selectedItems[0].dictAttributes.ID = "";
                    }
                    if (confirm("BusMethod : '" + selectedItems[0].dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                        var index = selectedXmlMethod.Elements.indexOf(selectedItems[0]);
                        var itemIndex = selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]);
                        if (index > -1 && itemIndex > -1) {
                            $rootScope.UndRedoBulkOp("Start");
                            $rootScope.DeleteItem(selectedItems[0], selectedXmlMethod.Elements);

                            newMethodScope.removeUnwantedXmlMethodParameters(selectedXmlMethod);
                            $rootScope.UndRedoBulkOp("End");
                            $ValidationService.removeObjInToArray($scope.validationErrorList, selectedItems[0]); // remove method from validation list
                            //Select next item
                            if (selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).length > 0) {
                                if (itemIndex == selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; }).length) {
                                    itemIndex--;
                                }
                                selectedXmlMethod.Elements.filter(function (x) { return x.Name == "item"; })[itemIndex].isSelected = true;
                            }
                        }
                    }
                }
            }
        };
        newMethodScope.removeUnwantedXmlMethodParameters = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var parameterNames = selectedXmlMethod.Elements.map(function (x) { return x.dictAttributes.sfwParameter; }).join(",").split(",");
                if (selectedXmlMethod.dictAttributes.sfwParameter) {
                    var methodparameters = selectedXmlMethod.dictAttributes.sfwParameter.split(",");
                    parameterNames = parameterNames.concat(methodparameters);
                }
                var parameters = selectedXmlMethod.Elements.filter(function (x) {
                    return x.Name == "parameter";
                });
                for (var i = 0; i < parameters.length; i++) {
                    var paramId = parameters[i].dictAttributes.ID;

                    if (parameterNames.indexOf(paramId) == -1 && parameterNames.indexOf('@' + paramId) == -1) {
                        var index = selectedXmlMethod.Elements.indexOf(parameters[i]);
                        if (index > -1) {
                            $rootScope.DeleteItem(parameters[i], selectedXmlMethod.Elements);
                        }
                    }
                }
            }
        };
        newMethodScope.addBusMethod = function (selectedXmlMethod) {
            if (selectedXmlMethod) {
                var busMethod = {
                    Name: "item", Value: "", Elements: [], Children: [], dictAttributes: {}, isSelected: false
                };
                $rootScope.PushItem(busMethod, selectedXmlMethod.Elements);
                newMethodScope.setSelectedBusMethod(selectedXmlMethod, selectedXmlMethod.Elements.length - 1);
            }
        };
        newMethodScope.setSelectedBusMethod = function (selectedXmlMethod, index) {
            for (var i = 0; i < selectedXmlMethod.Elements.length; i++) {
                if (selectedXmlMethod.Elements[i].Name == "item") {
                    if (i == index) {
                        selectedXmlMethod.Elements[i].isSelected = true;
                        $scope.selectedMethodProperty = selectedXmlMethod.Elements[i];
                        if (selectedXmlMethod.Elements[i].dictAttributes.sfwLoadType && selectedXmlMethod.Elements[i].dictAttributes.sfwLoadType == "ForeignKey") {
                            if (!selectedXmlMethod.Elements[i].foreignKeys) {
                                newMethodScope.getForignKeysForselectedObject(selectedXmlMethod.Elements[i]);
                            }
                        }
                    }
                    else {
                        selectedXmlMethod.Elements[i].isSelected = false;
                    }
                }
            }
        };

        newMethodScope.getForignKeysForselectedObject = function (objItem) {
            if (objItem.dictAttributes.ID && objItem.dictAttributes.ID.trim() != "") {
                var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.objEntity.dictAttributes.ID, objItem.dictAttributes.ID.trim());
                var BusinessObjectName = "";
                var EntityName;
                if (object) {
                    BusinessObjectName = $scope.GetBusinessObject(object.DeclaringEntity);
                    EntityName = object.DeclaringEntity;
                }

                if (BusinessObjectName && $scope.blnShowCreateWithClass) {
                    if (objItem.dictAttributes.sfwItemType == "Collection") {
                        //Setting ForeignKey is pending
                        $.connection.hubEntityModel.server.getXmlMethodForeignKeyValues($scope.objEntity.dictAttributes.ID, BusinessObjectName, "Collection").done(function (dictforeignKeyValues) {
                            var foreignKeys = dictforeignKeyValues.Foreignkeys;
                            objItem.foreignKeys = foreignKeys;
                        });

                    }
                    else if (objItem.dictAttributes.sfwItemType == "Object") //Non-Collection
                    {
                        $.connection.hubEntityModel.server.getXmlMethodForeignKeyValues($scope.objEntity.dictAttributes.ID, BusinessObjectName, "Object").done(function (dictforeignKeyValues) {
                            var foreignKeys = dictforeignKeyValues.Foreignkeys;
                            objItem.foreignKeys = foreignKeys;
                        });
                    }
                } else if (!$scope.blnShowCreateWithClass) {
                    var lstForeignKeyFields = [];
                    if (EntityName) {
                        lstForeignKeyFields = $Entityintellisenseservice.GetIntellisenseData(EntityName, "Column", "Int", true, true, false, false, false, false);
                    }
                    objItem.foreignKeys = lstForeignKeyFields;
                }
            }
        };

        newMethodScope.addBusMethodParameter = function (selectedXmlMethod, id, datatype) {
            if (!selectedXmlMethod.Elements.some(function (x) {
                return x.Name == "parameter" && x.dictAttributes.ID == id;
            })) {
                var paramModel = {
                    Name: "parameter", Value: "", Elements: [], Children: [], dictAttributes: {
                        ID: id, sfwDataType: datatype
                    }
                };
                selectedXmlMethod.Elements.push(paramModel);
            }
        };
        newMethodScope.onChangeLoadMainCDO = function (objMethod) {
            if (objMethod) {
                objMethod.dictAttributes.sfwLoadQuery = "";
                objMethod.dictAttributes.sfwParameter = "";
                if (objMethod.dictAttributes.sfwLoadMainObject == 'True') {
                    objMethod.dictAttributes.sfwParameter = "aintPrimaryKey";
                    var isIDfound = false;
                    for (var i = 0; i < objMethod.Elements.length; i++) {
                        if (objMethod.Elements[i].Name == "parameter" && objMethod.Elements[i].dictAttributes.ID == "aintPrimaryKey") {
                            isIDfound = true;
                            break;
                        }
                    }

                    if (!isIDfound) {
                        var objParam = {
                            Name: "parameter", Value: '', dictAttributes: { ID: "aintPrimaryKey", sfwDataType: "int" }, Elements: []
                        };
                        $rootScope.PushItem(objParam, objMethod.Elements);
                    }
                } else {
                    newMethodScope.removeUnwantedXmlMethodParameters(objMethod);
                }
            }
        };

        newMethodScope.removeUnwantedParameters = function () {
            if (newMethodScope.obj) {
                newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
            }
        };

        newMethodScope.removeParameterFromSelectedXmlMethod = function (selectedXmlMethod, id) {
            for (var i = 0; i < selectedXmlMethod.Elements.length; i++) {

            }
        };
        newMethodScope.resetLoadSource = function (objmethod) {
            if (objmethod) {
                objmethod.dictAttributes.sfwLoadSource = "";
                if (objmethod.dictAttributes && objmethod.dictAttributes.sfwParameter) {
                    objmethod.dictAttributes.sfwParameter = "";
                    if (newMethodScope.obj) {
                        newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
                    }
                }
            }
            if (objmethod.dictAttributes.sfwLoadType == "ForeignKey") {
                var selectedXmlMethod, busMethod;
                var selectedMethod = [];
                if (flag == "Add") {
                    selectedMethod.push(newMethodScope.obj);
                } else {
                    selectedMethod = $scope.getselectedMethod();
                }
                if (selectedMethod && selectedMethod.length > 0) {
                    selectedXmlMethod = selectedMethod[0];
                    var selectedBusMethod = selectedXmlMethod.Elements.filter(function (x) {
                        return x.isSelected == true;
                    });
                    if (selectedBusMethod && selectedBusMethod.length > 0) {
                        busMethod = selectedBusMethod[0];
                    }
                }
                var method = null;
                if (selectedXmlMethod && busMethod) {
                    if (busMethod.dictAttributes.ID && busMethod.dictAttributes.ID.trim() != "") {
                        //method = newMethodScope.getBusMethod(selectedXmlMethod, busMethod);
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.objEntity.dictAttributes.ID, busMethod.dictAttributes.ID.trim());
                        var BusinessObjectName = "";
                        var EntityName;
                        if (object) {
                            BusinessObjectName = $scope.GetBusinessObject(object.DeclaringEntity);
                            EntityName = object.DeclaringEntity;
                        }
                        if (BusinessObjectName && $scope.blnShowCreateWithClass) {
                            if (busMethod.dictAttributes.sfwItemType == "Collection") {
                                //Setting ForeignKey is pending
                                $.connection.hubEntityModel.server.getXmlMethodForeignKeyValues($scope.objEntity.dictAttributes.ID, BusinessObjectName, "Collection").done(function (dictforeignKeyValues) {
                                    var foreignKeys = dictforeignKeyValues.Foreignkeys;
                                    var defaultForeignKeyValue = dictforeignKeyValues.ForeignkeyDefaultValue;
                                    newMethodScope.receiveXmlMethodForeignKeyValues(foreignKeys, defaultForeignKeyValue);
                                });

                            }
                            else if (busMethod.dictAttributes.sfwItemType == "Object") //Non-Collection
                            {
                                $.connection.hubEntityModel.server.getXmlMethodForeignKeyValues($scope.objEntity.dictAttributes.ID, BusinessObjectName, "Object").done(function (dictforeignKeyValues) {
                                    var foreignKeys = dictforeignKeyValues.Foreignkeys;
                                    var defaultForeignKeyValue = dictforeignKeyValues.ForeignkeyDefaultValue;
                                    newMethodScope.receiveXmlMethodForeignKeyValues(foreignKeys, defaultForeignKeyValue);
                                });
                            }
                        } else if (!$scope.blnShowCreateWithClass) {
                            var lstForeignKeyFields = [];
                            if (EntityName) {
                                lstForeignKeyFields = $Entityintellisenseservice.GetIntellisenseData(EntityName, "Column", "Int", true, true, false, false, false, false);
                            }
                            newMethodScope.receiveXmlMethodForeignKeyValues(lstForeignKeyFields, "");
                        }
                    }
                }
            }
        };
        newMethodScope.getBusMethod = function (selectedXmlMethod, busMethod) {
            var method = null;
            if ($scope.objBusinessObject.ObjTree) {
                if (busMethod.dictAttributes.ID.indexOf(".") == -1 && $scope.objBusinessObject.ObjTree.Rules) {
                    var rules = $scope.objBusinessObject.ObjTree.Rules.filter(function (x) {
                        return x.ID == busMethod.dictAttributes.ID;
                    });
                    if (rules && rules.length > 0) {
                        method = rules[0];
                    }
                }
                if (method == null) {
                    method = getBusObjectByPath(busMethod.dictAttributes.ID, $scope.objBusinessObject.ObjTree);
                }

                var id = busMethod.dictAttributes.ID.substring(busMethod.dictAttributes.ID.lastIndexOf(".") + 1);

                if (method == null) {
                    method = getBusObjectByPath(id, $scope.objBusinessObject.ObjTree);
                }
            }
            return method;
        };

        newMethodScope.NavigateToQueryFromBusMethod = function (loadSource) {
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
        newMethodScope.validateQuery = function (method) {
            newMethodScope.loadSourceError = "";
            if (method.dictAttributes.sfwLoadType && method.dictAttributes.sfwLoadType == "Query" &&
                method.dictAttributes.sfwLoadSource && method.dictAttributes.sfwLoadSource.trim().length > 0) {
                var index = method.dictAttributes.sfwLoadSource.indexOf(".");
                if (index > -1) {
                    var entityName = method.dictAttributes.sfwLoadSource.substring(0, index);
                    var indexOfEntity = null;
                    var entities = $filter('filter')($EntityIntellisenseFactory.getEntityIntellisense(), {
                        ID: entityName
                    }, true);
                    if (entities && entities.length > 0) {
                        var entity = entities[0];

                        var queryName = method.dictAttributes.sfwLoadSource.substring(index + 1, method.dictAttributes.sfwLoadSource.length);
                        if (queryName && queryName.trim().length > 0) {
                            var queries = $filter('filter')(entity.Queries, {
                                ID: queryName
                            }, true);
                            if (queries && queries.length > 0) {
                                var query = queries[0];
                                if (query.Parameters.length == 1) {
                                    if (query.Parameters[0].DataType != "int") {
                                        newMethodScope.loadSourceError = "DataType of parameter must be integer.";
                                    }
                                }
                                else {
                                    newMethodScope.loadSourceError = "Query should take only one parameter of type integer.";
                                }
                            }
                            else {
                                newMethodScope.loadSourceError = "Invalid Query.";
                            }

                        }
                        else {
                            newMethodScope.loadSourceError = "Invalid Query.";
                        }

                    }
                    else {
                        newMethodScope.loadSourceError = "Invalid Query.";
                    }

                }
                else {
                    newMethodScope.loadSourceError = "Invalid Query.";
                }
            }
        };
        newMethodScope.onForeignKeyChange = function (method, event) {
            setSingleLevelAutoComplete($(event.currentTarget), method.foreignKeys);
        };
        newMethodScope.receiveXmlMethodForeignKeyValues = function (foreignKeys, defaultForeignKeyValue) {
            $scope.$apply(function () {
                //var selectedMethod = $scope.objMethods.Elements.filter(function (x) {
                //    return x.selected == true
                //});
                var selectedMethod = newMethodScope.obj;
                if (selectedMethod) {
                    var selectedBusMethod = selectedMethod.Elements.filter(function (x) {
                        return x.isSelected == true;
                    });
                    if (selectedBusMethod && selectedBusMethod.length > 0) {
                        selectedBusMethod[0].foreignKeys = foreignKeys;
                        selectedBusMethod[0].dictAttributes.sfwLoadType = "ForeignKey";
                        selectedBusMethod[0].dictAttributes.sfwLoadSource = defaultForeignKeyValue;
                    }
                }
            });
        };
        newMethodScope.onCancelClick = function () {
            if (flag == "Add" && newMethodScope.obj && newMethodScope.obj.Elements) {
                angular.forEach(newMethodScope.obj.Elements, function (method) {
                    $ValidationService.removeObjInToArray($scope.validationErrorList, method);
                });
            }
            dialog.close();
        };

        newMethodScope.onChangeLoadQuery = function (objmethod) {
            if (objmethod && objmethod.dictAttributes.sfwLoadQuery) {
                var index = objmethod.dictAttributes.sfwLoadQuery.indexOf(".");
                objmethod.dictAttributes.sfwParameter = "";
                if (index > -1) {
                    var entityName = objmethod.dictAttributes.sfwLoadQuery.substring(0, index);
                    var indexOfEntity = null;
                    var entities = $filter('filter')($EntityIntellisenseFactory.getEntityIntellisense(), {
                        ID: entityName
                    }, true);
                    if (entities && entities.length > 0) {
                        var entity = entities[0];

                        var queryName = objmethod.dictAttributes.sfwLoadQuery.substring(index + 1, objmethod.dictAttributes.sfwLoadQuery.length);
                        if (queryName && queryName.trim().length > 0) {
                            var queries = $filter('filter')(entity.Queries, {
                                ID: queryName
                            }, true);
                            if (queries && queries.length > 0) {
                                var query = queries[0];
                                if (query.Parameters && query.Parameters.length > 0) {
                                    for (var i = 0; i < query.Parameters.length > 0; i++) {
                                        if (!objmethod.dictAttributes.sfwParameter) {
                                            objmethod.dictAttributes.sfwParameter = query.Parameters[i].ID;
                                        }
                                        else {
                                            objmethod.dictAttributes.sfwParameter += ',' + query.Parameters[i].ID;
                                        }
                                        if (newMethodScope.obj) {
                                            var paramId = query.Parameters[i].ID;
                                            if (paramId && paramId.match("^@")) {
                                                paramId = paramId.substring(1);
                                            }
                                            newMethodScope.addBusMethodParameter(newMethodScope.obj, paramId, query.Parameters[i].DataType);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                newMethodScope.onChangeLoadMainCDO(objmethod);
            }
            newMethodScope.removeUnwantedXmlMethodParameters(objmethod);
        };

        newMethodScope.onMethodorRuleChange = function (objObjectMethodorRule, objItem) {
            if (objObjectMethodorRule.Parameters && objObjectMethodorRule.Parameters.length > 0) {
                for (var i = 0; i < objObjectMethodorRule.Parameters.length > 0; i++) {
                    if (!objItem.dictAttributes.sfwParameter) {
                        objItem.dictAttributes.sfwParameter = objObjectMethodorRule.Parameters[i].ID;
                    }
                    else {
                        objItem.dictAttributes.sfwParameter += ',' + objObjectMethodorRule.Parameters[i].ID;
                    }
                }
                newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
                for (var j = 0; j < objObjectMethodorRule.Parameters.length > 0; j++) {
                    if (newMethodScope.obj) {
                        var paramId = objObjectMethodorRule.Parameters[j].ID;
                        if (paramId && paramId.match("^@")) {
                            paramId = paramId.substring(1);
                        }
                        newMethodScope.addBusMethodParameter(newMethodScope.obj, paramId, objObjectMethodorRule.Parameters[j].DataType);
                    }
                }
            } else {
                objItem.dictAttributes.sfwParameter = "";
                newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
            }
        };

        newMethodScope.onPropertyChanged = function (objItem) {
            if (objItem) {
                var paramID = objItem.dictAttributes.sfwParameter;
                if (paramID) {
                    newMethodScope.addBusMethodParameter(newMethodScope.obj, paramID, objItem.dictAttributes.sfwDataType);
                }
                newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
            }
        };

        newMethodScope.onChangeQuery = function (method) {
            if (method && method.dictAttributes && method.dictAttributes.sfwLoadType && method.dictAttributes.sfwLoadType == "Query" &&
                method.dictAttributes.sfwLoadSource && method.dictAttributes.sfwLoadSource.trim().length > 0) {
                var index = method.dictAttributes.sfwLoadSource.indexOf(".");
                method.dictAttributes.sfwParameter = "";
                if (index > -1) {
                    var entityName = method.dictAttributes.sfwLoadSource.substring(0, index);
                    var indexOfEntity = null;
                    var entities = $filter('filter')($EntityIntellisenseFactory.getEntityIntellisense(), {
                        ID: entityName
                    }, true);
                    if (entities && entities.length > 0) {
                        var entity = entities[0];

                        var queryName = method.dictAttributes.sfwLoadSource.substring(index + 1, method.dictAttributes.sfwLoadSource.length);
                        if (queryName && queryName.trim().length > 0) {
                            var queries = $filter('filter')(entity.Queries, {
                                ID: queryName
                            }, true);
                            if (queries && queries.length > 0) {
                                var query = queries[0];
                                if (query.Parameters && query.Parameters.length > 0) {
                                    for (var i = 0; i < query.Parameters.length > 0; i++) {
                                        if (!method.dictAttributes.sfwParameter) {
                                            method.dictAttributes.sfwParameter = query.Parameters[i].ID;
                                        }
                                        else {
                                            method.dictAttributes.sfwParameter += ',' + query.Parameters[i].ID;
                                        }
                                        if (newMethodScope.obj) {
                                            var paramId = query.Parameters[i].ID;
                                            if (paramId && paramId.match("^@")) {
                                                paramId = paramId.substring(1);
                                            }
                                            newMethodScope.addBusMethodParameter(newMethodScope.obj, paramId, query.Parameters[i].DataType);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (method && method.dictAttributes && method.dictAttributes.sfwLoadType && method.dictAttributes.sfwLoadType == "Query" && !method.dictAttributes.sfwLoadSource) {
                method.dictAttributes.sfwParameter = "";
            }
            if (newMethodScope.obj) {
                newMethodScope.removeUnwantedXmlMethodParameters(newMethodScope.obj);
            }
        };
    };

    $scope.GetBusinessObject = function (entityID) {
        var BusinessobjectName = "";
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var parentId = entityID;
        while (parentId) {
            var lst = entityIntellisenseList.filter(function (x) {
                return x.ID == parentId;
            });
            if (lst && lst.length > 0) {
                BusinessobjectName = lst[0].BusinessObjectName;
                if (!BusinessobjectName) {
                    parentId = lst[0].ParentId;
                } else {
                    parentId = "";
                }
            } else {
                parentId = "";
            }
        }
        return BusinessobjectName;
    };

    $scope.isSelectedMethoddbclick = function (obj) {
        $scope.SelectXmlMethod(obj);
        $scope.selectBusinessObjectTab();
    };
    $scope.SelectXmlMethod = function (obj) {
        if (obj) {
            $scope.clearSelection();
        }


        for (var i = 0; i < $scope.objMethods.Elements.length; i++) {
            if ($scope.objMethods.Elements[i] == obj) {
                $scope.objMethods.Elements[i].selected = true;
                $scope.SelectedXMLMethod = $scope.objMethods.Elements[i];
            }
            else {
                $scope.objMethods.Elements[i].selected = false;
            }
        }
    };

    $scope.canDeleteMethod = function () {
        if ($scope.objMethods != undefined && $scope.objMethods.Elements != undefined) {
            return $scope.objMethods.Elements.some(function (x) {
                return x.selected;
            });
        }
    };
    $scope.DeleteMethod = function (selectedTabName) {
        var selectedItems = $scope.objMethods.Elements.filter(function (x) {
            return x.selected;
        });
        if (selectedItems && selectedItems.length > 0) {
            if (confirm("XML Method : '" + selectedItems[0].dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                var index = $scope.objMethods.Elements.indexOf(selectedItems[0]);
                $ValidationService.removeObjInToArray($scope.validationErrorList, selectedItems[0]);
                if (index > -1) {
                    $rootScope.DeleteItem(selectedItems[0], $scope.objMethods.Elements, selectedTabName);
                    //$scope.objMethods.Elements.splice(index, 1);
                    //Select next item
                    if (selectedTabName) {
                        if ($scope.objMethods.Elements.length > 0) {
                            if (index == $scope.objMethods.Elements.length) {
                                index--;
                            }
                            $scope.objMethods.Elements[index].selected = true;
                        }
                    } else {
                        selectedItems[0].selected = false;
                        $scope.clearSelection();
                    }
                    checkDuplicateIdForMethod(true);
                }
            }
        }
    };

    $scope.SelectedXmlMethodItem = function (obj) {
        if (obj && $scope.SelectedXMLMethod) {
            var index = $scope.SelectedXMLMethod.Elements.indexOf(obj);
            $scope.setSelectedBusMethod(obj, index);
        }
    };

    $scope.setSelectedBusMethod = function (selectedXmlMethod, index) {
        for (var i = 0; i < selectedXmlMethod.Elements.length; i++) {
            if (selectedXmlMethod.Elements[i].Name == "item") {
                if (i == index) {
                    selectedXmlMethod.Elements[i].isSelected = true;
                    $scope.selectedMethodProperty = selectedXmlMethod.Elements[i];
                }
                else {
                    selectedXmlMethod.Elements[i].isSelected = false;
                }
            }
        }
    };
    $scope.getselectedMethod = function () {
        var tempselectedMethod = $scope.objMethods.Elements.filter(function (x) {
            return x.selected == true;
        });
        return tempselectedMethod;
    };
    $scope.getBusMethod = function (selectedXmlMethod, busMethod) {
        var method = null;
        if (busMethod.dictAttributes.ID.indexOf(".") == -1 && $scope.objBusinessObject.ObjTree.Rules) {
            var rules = $scope.objBusinessObject.ObjTree.Rules.filter(function (x) {
                return x.ID == busMethod.dictAttributes.ID;
            });
            if (rules && rules.length > 0) {
                method = rules[0];
            }
        }
        if (method == null) {
            method = getBusObjectByPath(busMethod.dictAttributes.ID, $scope.objBusinessObject.ObjTree);
        }

        var id = busMethod.dictAttributes.ID.substring(busMethod.dictAttributes.ID.lastIndexOf(".") + 1);

        if (method == null) {
            method = getBusObjectByPath(id, $scope.objBusinessObject.ObjTree);
        }

        return method;
    };

    $scope.onForeignKeyChange = function (method, event) {
        var input;
        if ((event.keyCode && event.keyCode != 13 && event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") || event.type == "click") {
            if (event.type && event.type == "click") input = $(event.target).prevAll("input[type='text']");
            else input = $(event.target);
            if (method && !method.foreignKeys) {
                $scope.onChangeDeletePropertyId(method);
            }
            if (method.foreignKeys) {
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                } else {
                    setSingleLevelAutoComplete(input, method.foreignKeys);
                    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                }
                if (event.type && event.type == "click") {
                    input.focus();
                    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                }
            }
        }
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            event.preventDefault();
        }
    };

    //#endregion

    //#region Business Object - LifeCylce Section
    $scope.onLifeCycleMethodChange = function (obj) {
        var isFound = false;
        if ($scope.objLifecycle.Elements.length > 0) {
            for (var i = 0; i < $scope.objLifecycle.Elements.length; i++) {
                if ($scope.objLifecycle.Elements[i].dictAttributes.ID == obj.ID) {
                    isFound = true;
                    if (obj.XmlMethod != '' && $scope.isValidLifeCycleMethod(obj.XmlMethod)) {
                        $scope.objLifecycle.Elements[i].dictAttributes.sfwXmlMethod = obj.XmlMethod;
                        $scope.objLifecycle.Elements[i].dictAttributes.sfwNavigationParameter = "";
                    }
                    else {
                        $scope.objLifecycle.Elements.splice(i, 1);
                    }
                }
            }
        }
        if (!isFound && $scope.isValidLifeCycleMethod(obj.XmlMethod)) {
            var objlifecyclemethod = {
                Children: [], dictAttributes: { ID: obj.ID, sfwXmlMethod: obj.XmlMethod }, Name: 'method', Value: ''
            };
            $scope.objLifecycle.Elements.push(objlifecyclemethod);
        }
        obj.Parameters = "";
    };
    $scope.onLifeCycleMethodKeyDown = function (event) {
        var inputElement = $(event.target);
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            getLifeCucleMethodsList(inputElement);
            event.preventDefault();
        }
    };

    $scope.showLifeCycleMethodsList = function (event) {
        var inputElement = $(event.target).prevAll("input[type='text']");
        inputElement.focus();
        getLifeCucleMethodsList(inputElement);

        if (event) {
            event.stopPropagation();
        }
    };

    var getLifeCucleMethodsList = function (input) {
        var nonLoadMethods = [];
        nonLoadMethods = $scope.objMethods.Elements.filter(function (x) { return x.dictAttributes.sfwMethodType != "Load"; }).map(function (y) {
            return y.dictAttributes.ID;
        });
        setSingleLevelAutoComplete($(input), nonLoadMethods);
        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val().trim());
    };

    $scope.isValidLifeCycleMethod = function (xmlMethodID) {
        if ($scope.objMethods) {
            return $scope.objMethods.Elements.some(function (x) {
                return x.dictAttributes.ID == xmlMethodID && x.dictAttributes.sfwMethodType != "Load";
            });
        }
        else {
            return false;
        }
    };
    $scope.onLifeCycleSetNavParamClick = function (objMethod) {
        var methods = $scope.objMethods.Elements.filter(function (x) {
            return x.dictAttributes.ID == objMethod.XmlMethod;
        });
        if (methods && methods.length > 0) {
            var selectedXmlMethod = methods[0];
        }

        if (selectedXmlMethod) {
            $scope.xmlMethodParameters = selectedXmlMethod.Elements.filter(function (x) { return x.Name == "parameter"; }).map(function (y) {
                return {
                    ParamID: y.dictAttributes.ID, IsConstant: y.dictAttributes.Value != undefined && y.dictAttributes.Value.indexOf("#") == 0, Value: y.dictAttributes.Value != undefined && y.dictAttributes.Value.indexOf("#") == 0 ? y.dictAttributes.Value.substring(1) : y.dictAttributes.Value
                };
            });

            var unSetParams = $scope.xmlMethodParameters.filter(function (x) {
                return x.Value == undefined || x.Value.trim() == "";
            });
            var lifeCycleMethods = $scope.objLifecycle.Elements.filter(function (x) {
                return x.dictAttributes.ID == objMethod.ID && x.dictAttributes.sfwXmlMethod == objMethod.XmlMethod;
            });
            if (lifeCycleMethods && lifeCycleMethods.length > 0) {
                var lifeCycleMethod = lifeCycleMethods[0];
            }
            if (unSetParams && unSetParams.length > 0) {
                if (lifeCycleMethod.dictAttributes.sfwNavigationParameter) {
                    var params = lifeCycleMethod.dictAttributes.sfwNavigationParameter.split(";");
                    for (var index = 0; index < params.length; index++) {
                        var param = params[index].split("=");
                        if (param && param.length == 2) {
                            var id = param[0];
                            var isConstant = param[1].indexOf("#") == 0;
                            var value = isConstant ? param[1].substring(param[1].indexOf("#") + 1) : param[1];
                            var unSetParam = unSetParams.filter(function (x) {
                                return x.ParamID == id;
                            });
                            if (unSetParam && unSetParam.length > 0) {
                                unSetParam[0].IsConstant = isConstant;
                                unSetParam[0].Value = value;
                            }
                        }
                    }
                }
            }

            /*var dialog = ngDialog.open({
                template: 'setNavigationParameters',
                className: 'ngdialog-theme-default ngdialog-theme-custom',
                closeByDocument: false,
                scope: $scope,
            });*/

            var newScope = $scope.$new(true);
            newScope.ObjTree = $scope.objBusinessObject.ObjTree;
            newScope.lstobjecttree = $scope.lstobjecttree;
            newScope.StrXmlMethodObjectTreePath = $scope.StrXmlMethodObjectTreePath;
            newScope.searchXmlMethodObjectTree = $scope.searchXmlMethodObjectTree;
            newScope.xmlMethodParameters = $scope.xmlMethodParameters;
            newScope.objEntity = $scope.objEntity;
            newScope.StrXmlMethodObjectTreeText = $scope.StrXmlMethodObjectTreeText;
            newScope.StrXmlMethodObjectTreePath = $scope.StrXmlMethodObjectTreePath;
            newScope.StrObjectTreeText = $scope.StrObjectTreeText;
            newScope.StrObjectTreePath = $scope.StrObjectTreePath;
            newScope.isParamAlreadySet = newScope.isParamAlreadySet;

            newScope.setnavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Entity/views/SetNavigationParameters.html", {
                width: 830, height: 670
            });

            newScope.onSetNavParamOKClick = function () {
                if (lifeCycleMethod) {
                    var paramValue = [];
                    for (var index = 0; index < $scope.xmlMethodParameters.length; index++) {
                        if (!$scope.isParamAlreadySet($scope.xmlMethodParameters[index]) && $scope.xmlMethodParameters[index].Value && $scope.xmlMethodParameters[index].Value.trim().length > 0) {
                            paramValue.push($scope.xmlMethodParameters[index].ParamID + "=" + ($scope.xmlMethodParameters[index].IsConstant ? "#" + $scope.xmlMethodParameters[index].Value : $scope.xmlMethodParameters[index].Value));
                        }
                    }
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(lifeCycleMethod.dictAttributes.sfwNavigationParameter, lifeCycleMethod.dictAttributes, "sfwNavigationParameter", paramValue.join(";"));
                    $rootScope.EditPropertyValue(objMethod.Parameters, objMethod, "Parameters", paramValue.join(";"));
                    $rootScope.UndRedoBulkOp("End");

                }
                newScope.setnavigationParameterDialog.close();
                $scope.xmlMethodParameters = undefined;
                $scope.isParamAlreadySet = undefined;

                newScope.xmlMethodParameters = undefined;
                newScope.onSetNavParamOKClick = undefined;
                newScope.isParamAlreadySet = undefined;
            };

            newScope.isParamAlreadySet = function (param) {
                return selectedXmlMethod.Elements.some(function (x) {
                    return x.Name == "parameter" && x.dictAttributes.ID == param.ParamID && x.dictAttributes.Value && x.dictAttributes.Value.trim().length > 0;
                });
            };

            $scope.isParamAlreadySet = function (param) {
                return selectedXmlMethod.Elements.some(function (x) {
                    return x.Name == "parameter" && x.dictAttributes.ID == param.ParamID && x.dictAttributes.Value && x.dictAttributes.Value.trim().length > 0;
                });
            };


        }
    };
    $scope.selectedLifeCycle = undefined;
    $scope.selectLifeCycleMethod = function (obj) {
        if (obj.XmlMethod) {
            $scope.clearSelection();
            $scope.selectedLifeCycle = obj;
        }
        else if (obj.dictAttributes && obj.dictAttributes.sfwXmlMethod) {
            var selected = $scope.lstlifecycle.filter(function (x) {
                return x.ID == obj.dictAttributes.ID;
            });
            $scope.selectedLifeCycle = selected[0];
        }
    };
    //#endregion

    //#region Business Object - Object Methods Section

    $scope.SelectObjectMethod = function (obj) {
        if ($scope.SelectedObjectMethod) {
            if ($scope.SelectedObjectMethod != obj) {
                if ($scope.SelectedObjectMethod.IsShowParams) {
                    $scope.SelectedObjectMethod.IsShowParams = false;
                }
            }
        }
        $scope.clearSelection();
        $scope.SelectedObjectMethod = obj;
    };

    $scope.onExpandCollapsedObjectMethodParams = function (objObjectMethod) {
        if (objObjectMethod) {
            objObjectMethod.IsShowParams = !objObjectMethod.IsShowParams;
        }
    };

    $scope.IsTypeOfbusOrDo = function (RetType) {
        var retVal = false;
        if (RetType) {
            var baseType = RetType;
            while ((baseType != null || baseType != undefined) && baseType.Name != "busBase" && baseType.Name != "doBase") {
                baseType = baseType.BaseType;
            }

            if ((baseType != null || baseType != undefined) && (baseType.Name == "busBase" || baseType.Name == "doBase")) {
                retVal = true;
            }
        }
        return retVal;
    };

    $scope.onAddObjectMethods = function () {
        var objMethod = {
            Name: 'method', Value: '', dictAttributes: {}, Elements: []
        };
        $rootScope.PushItem(objMethod, $scope.objObjectMethods.Elements, "SelectObjectMethod");
        $scope.SelectObjectMethod(objMethod);
        $scope.validateId($scope.objObjectMethods.Elements[$scope.objObjectMethods.Elements.length - 1], undefined, true);
        $timeout(function () {
            var elem = $("#" + $scope.currentfile.FileName).find("#entity-businessobject-objectmethod-section").find(".selected");
            if (elem) {
                if (elem.length > 0) {
                    elem[0].scrollIntoView();
                }
            }
        });
    };

    $scope.AddOrRefreshObjectMethodParams = function (objMethod, objParams) {

        angular.forEach(objMethod.Parameters, function (itm) {


            var isParameterBusType = $scope.IsTypeOfbusOrDo(itm.ParameterType);

            var paramDataType = null;// itm.ParameterType.Name;
            var objectid = "";
            if (itm.ParameterType.Name.match("]$")) {
                paramDataType = "Collection";
                objectid = itm.ParameterType.Name.substring(0, itm.ParameterType.Name.length - 1).toLowerCase();
            }
            else if (isParameterBusType) {
                paramDataType = "Object";
                objectid = itm.ParameterType.Name.toLowerCase();
            }
            if (paramDataType == null) {
                switch (itm.ParameterType.Name.toLowerCase()) {
                    case "short":
                    case "int16":
                        paramDataType = "short";
                        break;
                    case "int":
                    case "int32":
                        paramDataType = "int";
                        break;
                    case "long":
                    case "int64":
                        paramDataType = "long";
                        break;
                    case "single":
                        paramDataType = "float";
                        break;
                    case "boolean":
                        paramDataType = "bool";
                        break;
                    default:
                        paramDataType = itm.ParameterType.Name.toLowerCase();
                }
            }
            var objParam = {
                Name: "parameter", Value: '', dictAttributes: { ID: itm.ParameterName, sfwDataType: paramDataType }, Elements: []
            };
            if (objectid != undefined && objectid != "") {
                if (objectid.indexOf("do") == 0) {
                    objectid = "bus" + objectid.substring(2);
                }

                var entities = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (x) {
                    return x.BusinessObjectName && x.BusinessObjectName.toLowerCase() == objectid.toLowerCase();
                });
                if (entities && entities.length > 0) {
                    objParam.dictAttributes.sfwEntity = entities[0].ID;
                }
            }


            $rootScope.PushItem(objParam, objParams.Elements);
        });
    };

    $scope.DeleteObjectMethod = function () {
        if ($scope.SelectedObjectMethod) {
            var index = $scope.objObjectMethods.Elements.indexOf($scope.SelectedObjectMethod);
            if (index > -1) {
                if (confirm("ObjectMethod : '" + $scope.SelectedObjectMethod.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                    $rootScope.DeleteItem($scope.SelectedObjectMethod, $scope.objObjectMethods.Elements);
                    $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.SelectedObjectMethod);
                    if (index < $scope.objObjectMethods.Elements.length) {
                        $scope.SelectedObjectMethod = $scope.objObjectMethods.Elements[index];
                    }
                    else if ($scope.objObjectMethods.Elements.length > 0) {
                        $scope.SelectedObjectMethod = $scope.objObjectMethods.Elements[index - 1];
                    }

                    if ($scope.objObjectMethods.Elements.length == 0) {
                        $scope.SelectedObjectMethod = undefined;
                    }
                    checkDuplicateIdForMethod(true);
                }
            }
        }
    };

    $scope.onObjectMethodIdChanged = function (astrId, model) {
        var lstParam = model.Elements.filter(function (itm) {
            return itm.Name == "parameters";
        });
        if (lstParam && lstParam.length > 0) {
            for (i = 0; i < lstParam[0].Elements.length; i++) {
                $rootScope.DeleteItem(lstParam[0].Elements[i], lstParam[0].Elements);
            }
            // $rootScope.EditPropertyValue(lstParam[0].Elements, lstParam[0], "Elements", []);
        }
        if (astrId && astrId != "") {
            if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
                var lstMethod = $scope.objBusinessObject.ObjTree.lstMethods.filter(function (itm) {
                    return itm.ShortName == astrId;
                });
                if (lstMethod && lstMethod.length > 0) {
                    var istype = false;
                    if (lstMethod[0].Typeof) {
                        istype = true;
                    }
                    var returnType = lstMethod[0].DataTypeName;
                    var objectid = "";
                    if (lstMethod[0].DataTypeName.match("]$")) {
                        returnType = "Collection";
                        objectid = lstMethod[0].DataTypeName.substring(0, lstMethod[0].DataTypeName.length - 1);
                    }
                    else if (istype) {
                        returnType = "Object";
                        objectid = lstMethod[0].DataTypeName;
                    }
                    $rootScope.EditPropertyValue(model.dictAttributes.sfwReturnType, model.dictAttributes, "sfwReturnType", returnType);

                    // model.dictAttributes.sfwReturnType = returnType;
                    if (objectid != undefined && objectid != "") {
                        if (objectid.indexOf("do") == 0) {
                            objectid = "bus" + objectid.substring(2);
                        }
                        else if (objectid.indexOf("cdo") == 0) {
                            objectid = "bus" + objectid.substring(3);
                        }
                        var entities = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (x) {
                            return x.BusinessObjectName == objectid;
                        });
                        if (entities && entities.length > 0) {
                            model.dictAttributes.sfwEntity = entities[0].ID;
                        }
                    }
                    if (lstMethod[0].Parameters && lstMethod[0].Parameters.length > 0) {
                        //model.Elements = [];
                        //var objParams = {
                        //    Name: "parameters", Value: '', dictAttributes: {}, Elements: []
                        //};
                        //model.Elements.push(objParams);
                        //if (objParams) {
                        //    $scope.AddOrRefreshObjectMethodParams(lstMethod[0], objParams);
                        //}

                        var lstParam = model.Elements.filter(function (itm) {
                            return itm.Name == "parameters";
                        });
                        if (!lstParam || (lstParam && lstParam.length == 0)) {
                            objParams = {
                                Name: "parameters", Value: '', dictAttributes: {}, Elements: []
                            };
                            //$rootScope.PushItem(objParams, $scope.SelectedObjectMethod.Elements);
                            model.Elements.push(objParams);
                        }
                        else {
                            objParams = lstParam[0];
                        }
                        if (objParams) {
                            $scope.AddOrRefreshObjectMethodParams(lstMethod[0], objParams);
                        }

                    }
                    else {
                        model.Elements = [];
                    }
                }
                else {
                    model.dictAttributes.sfwReturnType = null;
                    model.dictAttributes.sfwEntity = null;
                    model.Elements = [];
                }
            }
        }
        else {
            model.dictAttributes.sfwReturnType = null;
            model.dictAttributes.sfwEntity = null;
            model.Elements = [];
        }
        $ValidationService.validateID(model, $scope.validationErrorList, model.dictAttributes.ID);
        var methodList = getMethodsModel("objectmethod");
        if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
            var lstobjBusinessObject = $scope.objBusinessObject.ObjTree.lstMethods;
            var ValidObjMethod = lstobjBusinessObject.filter(function (obj) { if (obj.ShortName) { return obj.ShortName === model.dictAttributes.ID } });
            //var ValidObjMethod = lstobjBusinessObject.filter(function (obj) { return obj.ShortName === model.dictAttributes.ID });
            if (ValidObjMethod.length <= 0) {
                var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                model.errors.inValid_id = errorMessage;
            }
        }

        var objDuplicate = $ValidationService.findDuplicateId(methodList, model.dictAttributes.ID, CONST.ENTITY.NODES, null, model, "dictAttributes.ID");
        if (objDuplicate) {
            var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
            if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
            }
            model.errors.duplicate_id = errorMessage;
        }
        else {
            if (model.errors && model.errors.hasOwnProperty("duplicate_id")) {
                delete model.errors.duplicate_id;
            }
        }
    };

    $scope.getObjectMethodList = function (event) {
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            event.preventDefault();
        }
        else if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            $scope.onEntityChanged($scope.objEntity.dictAttributes.ID);
            setSingleLevelAutoComplete($(event.currentTarget), $scope.lstObjectMethod, $scope, "ID", "ID");
            if ($(event.currentTarget).data('ui-autocomplete')) {
                $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            }
            event.preventDefault();
        }
        else if ($(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
        }

    };
    $scope.showObjectMethodList = function (event) {
        $scope.inputElement = $(event.target).prevAll("input[type='text']");
        if ($scope.inputElement) {
            $($scope.inputElement).focus();
            //if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
            //    setSingleLevelAutoComplete($scope.inputElement, $scope.objBusinessObject.ObjTree.lstMethods, $scope, "ShortName", "Description");
            //    if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            //}
            if ($($scope.inputElement).val() == "") {
                $scope.onEntityChanged($scope.objEntity.dictAttributes.ID);
            }
            if ($scope.lstObjectMethod && $scope.lstObjectMethod.length > 0) {
                setSingleLevelAutoComplete($scope.inputElement, $scope.lstObjectMethod, $scope, "ID", "ID");
                if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            }
            else {
                $scope.onEntityChanged($scope.objEntity.dictAttributes.ID);
            }
        }
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.onEntityChanged = function (entityID) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        parententityName = $scope.objEntity.dictAttributes.ID;
        $scope.lstObjectMethod = [];
        while (parententityName) {
            var entity = entityIntellisenseList.filter(function (x) {
                return x.ID == parententityName;
            });
            if (entity.length > 0 && entity[0].ObjectMethods && entity[0].ObjectMethods.length > 0) {
                var tempdata = [];
                for (var i = 0; i < entity[0].ObjectMethods.length; i++) {
                    if (entity[0].ObjectMethods[i].Parameters && entity[0].ObjectMethods[i].Parameters.length == 0) {
                        tempdata.push(entity[0].ObjectMethods[i]);
                    }
                }
                $scope.lstObjectMethod = $scope.lstObjectMethod.concat(tempdata);
            }
            if (entity.length > 0) {
                parententityName = entity[0].ParentId;
            } else {
                parententityName = "";
            }
        }
    };
    //#endregion

    //#region Attributes Section
    $scope.EditAttributeRowValue = -1;
    $scope.SetEditAttributeRowValue = function (index) {
        $scope.EditAttributeRowValue = index;
    };

    $scope.AddAttributeFromDashBoard = function () {
        //$scope.ResetAllValuesInAttributesDailog();
        var newScope = $scope.$new();
        newScope.AttributeName = "";
        newScope.DescriptionInAttribute = "";
        newScope.DataTypeInAttribute = "";
        newScope.ObjectFieldInAttribute = "";
        newScope.JSONFieldInAttribute = "";
        newScope.DefaultValue = "";
        newScope.DataFormatInAttribute = "";
        newScope.IsPrivateAttribute = false;
        var dialog = $rootScope.showDialog(newScope, "Add Attribute", "Entity/views/AddNewAttribute.html", {
            width: 610, height: 430
        });
        newScope.onCancelAttributeClick = function () {
            dialog.close();
        };

        newScope.clearDefaultValue = function (id) {
            newScope.DefaultValue = "";
        }
        newScope.onOkClickAttributeDailog = function () {
            var newValueField = {
            };
            newValueField.Value = '';
            newValueField.Name = 'attribute';
            newValueField.Elements = [];
            if (this.IsPrivateAttribute == true) {
                this.IsPrivateAttribute = "True";
            }
            else {
                this.IsPrivateAttribute = "False";
            }
            newValueField.dictAttributes = {
                ID: this.AttributeName, sfwDataType: this.DataTypeInAttribute, sfwDataFormat: this.DataFormatInAttribute, sfwPrivate: this.IsPrivateAttribute, sfwValue: this.ObjectFieldInAttribute, sfwRelatedJSONField: this.JSONFieldInAttribute, sfwEnumerator: '', Text: this.DescriptionInAttribute, sfwDefaultValue: this.DefaultValue, sfwType: "Property"
            };
            $rootScope.PushItem(newValueField, $scope.objAttributes.Elements);
            $scope.selectAttribute(newValueField, null, $scope.objAttributes.Elements.length - 1);
            dialog.close();
            $scope.scrollToPosition("#attributes", ".entity-dashboard-inner-section-container", newValueField.dictAttributes.ID);
        };

        newScope.validateAttributeDailog = function () {
            newScope.attributeError = undefined;
            if (!newScope.AttributeName) {
                newScope.attributeError = "Error: Enter Attribute Name/Id.";
                return;
            } else if (!isValidIdentifier(newScope.AttributeName, false, false)) {
                newScope.attributeError = "Error: " + CONST.VALIDATION.NOT_VALID_ID;
                return;
            } else {
                var attributeModel = getAttributesList();
                var objDuplicate = $ValidationService.findDuplicateId(attributeModel, newScope.AttributeName, CONST.ENTITY.NODES, "", null, "dictAttributes.ID");
                if (objDuplicate) {
                    var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                    if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                        errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                    }
                    newScope.attributeError = errorMessage;
                    return;
                }
            }
            if (!newScope.DataTypeInAttribute || newScope.DataTypeInAttribute == "Collection") {
                newScope.attributeError = "Error: Enter Data Type.";
                return;
            }
        };
        newScope.validateAttributeDailog();
    };

    $scope.SelectedObject = "Collection";

    // #region attributes
    $scope.removeValueAttribute = function () {
        if ($scope.selectedItem != undefined) {
            if ($scope.selectedItem.dictAttributes.sfwType == 'Column') {
                alert('Column attribute cannot be deleted');
            }
            else {
                var index = $scope.objAttributes.Elements.indexOf($scope.selectedItem);
                if (index > -1) {
                    if (confirm("Attribute : '" + $scope.selectedItem.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                        $rootScope.DeleteItem($scope.selectedItem, $scope.objAttributes.Elements, "selectAttribute");
                        $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.selectedItem);
                    }
                }
                $scope.clearSelection();
                $scope.checkDuplicateId(true);
                $("#tblAttribute").css('height', 'auto');
            }
        }
    };
    $scope.updateValidation = function (updatedobject, phase) {
        if (updatedobject) {
            if (phase === "BEFORE") {
                $ValidationService.removeObjInToArray($scope.validationErrorList, updatedobject);
            }
            else if (phase === "AFTER") {
                $scope.checkDuplicateId(true);
                $scope.checkObjectFieldValue();
            }
        }
    }
    //#region Attributes - Column
    $scope.isCodeIdAttribute = function (attribute) {
        if (attribute && attribute.dictAttributes && attribute.dictAttributes.sfwValue && attribute.dictAttributes.sfwValue.trim().length > 0) {
            var columnName = attribute.dictAttributes.sfwValue.toLowerCase();
            if (columnName.indexOf("_id") == columnName.length - 3) {
                columnName = columnName.substring(0, columnName.length - 3) + "_value";
                var elementFilter = function (x) {
                    return x.dictAttributes && x.dictAttributes.sfwType && x.dictAttributes.sfwType.toLowerCase() == "column" && x.dictAttributes.sfwValue && x.dictAttributes.sfwValue.toLowerCase() == columnName;
                };
                var items = $scope.objAttributes.Elements.filter(elementFilter);
                if (items && items.length > 0) {
                    return true;
                }
            }
        }
        return false;
    };
    $scope.refreshColumns = function () {
        var mainPageScope = getScopeByFileName("MainPage");
        if (mainPageScope && mainPageScope.strDBConnection && mainPageScope.strDBConnection === "DB : Not Connected.") {
            toastr.error("Database not connected.");
        }
        else if ($scope.isDirty) {
            alert('Please save the file before refresh');
        }
        else {
            var result = confirm("Refreshing the entity will update and save the file.Do you want to continue?");
            if (result == true) {
                $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
                if ($scope.objEntity.dictAttributes.sfwTableName != undefined && $scope.objEntity.dictAttributes.sfwTableName != "") {
                    $rootScope.IsLoading = true;
                    $.connection.hubEntityModel.server.refreshEntityTable($rootScope.currentopenfile.file).done(function (data) {
                        var objRefreshEntity = data;
                        if (objRefreshEntity && objRefreshEntity.lstCodeValues.length > 0) {
                            var dialogScope = $scope.$new();
                            dialogScope.$evalAsync(function () {
                                dialogScope.lstCodeValues = objRefreshEntity.lstCodeValues;
                            });
                            dialogScope.title = "Set Code IDs for Columns - " + $scope.objEntity.dictAttributes.ID;
                            dialogScope.template = "Entity/views/SetCodeIDForColumn.html";
                            dialogScope.onOKClick = function () {
                                var objattributes;
                                angular.forEach(objRefreshEntity.EntityModel.Elements, function (item) {
                                    if (item.Name == "attributes") {
                                        objattributes = item;
                                        return;
                                    }
                                });
                                if (dialogScope.lstCodeValues) {
                                    for (var i = 0; i < dialogScope.lstCodeValues.length; i++) {
                                        if (objattributes != undefined) {
                                            angular.forEach(objattributes.Elements, function (item) {
                                                if (item.dictAttributes.sfwType === "Column" && item.dictAttributes.sfwValue == dialogScope.lstCodeValues[i].FieldName) {
                                                    item.dictAttributes.sfwCodeID = dialogScope.lstCodeValues[i].CodeValue;
                                                    return;
                                                }
                                            });
                                        }
                                    }
                                }

                                var objFile;
                                for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                                    var strFileNamerfromList = $rootScope.lstopenedfiles[i].file.FileName;
                                    if (strFileNamerfromList == $rootScope.currentopenfile.file.FileName) {
                                        objFile = $rootScope.lstopenedfiles[i];
                                        break;
                                    }
                                }
                                $scope.refreshEntity(objRefreshEntity);
                                $scope.onSaveFile(objRefreshEntity.EntityModel, objFile);
                                dialogScope.closeDialog();
                                $scope.receiveentitymodel(objRefreshEntity.EntityModel);
                            };
                            dialogScope.onCancelClick = function () {
                                $scope.refreshEntity(objRefreshEntity);
                                dialogScope.closeDialog();
                            };
                            dialogScope.closeDialog = function () {
                                if (dialogScope.dialog && dialogScope.dialog.close) {
                                    dialogScope.dialog.close();
                                }
                            };
                            dialogScope.dialog = $rootScope.showDialog(dialogScope, dialogScope.title, dialogScope.template);
                        }
                        else {
                            $scope.refreshEntity(objRefreshEntity);
                        }
                        $rootScope.IsLoading = false;
                    });
                }
            }
        }
    };
    $scope.refreshEntity = function (objRefreshEntity) {
        $scope.$evalAsync(function () {
            var obj = $scope.currentfile;
            if (objRefreshEntity && objRefreshEntity.EntityModel) {
                if ($scope.objEntity.dictAttributes.sfwTableName != undefined) {
                    $scope.objEntity.dictAttributes.sfwDataObjectID = objRefreshEntity.EntityModel.dictAttributes.sfwDataObjectID;
                }
                for (var i = 0; i < objRefreshEntity.EntityModel.Elements.length; i++) {
                    if (objRefreshEntity.EntityModel.Elements[i].Name == "attributes") {
                        // $scope.objAttributes = $scope.objRefreshEntity.EntityModel.Elements[i];
                        var attrModel = objRefreshEntity.EntityModel.Elements[i];
                        $scope.objAttributes = [];
                        var tmepAttrObj = $filter('filter')($scope.objEntity.Elements, { Name: "attributes" })[0];
                        angular.copy(attrModel, tmepAttrObj);
                        $scope.objAttributes = tmepAttrObj;
                        $scope.RefreshIntellisenseAttributes($scope.objAttributes);
                        break;
                    }
                }
            }
        });
    };
    $scope.onSaveFile = function (scopeobject, objFile) {
        if (scopeobject != undefined) {
            $rootScope.SaveModelWithPackets(scopeobject, objFile.file, objFile.file.FileType, false);
        }
    };
    //#endregion

    function setAttributesActiveTab(tab) {
        $('#' + $scope.currentfile.FileName + ' .nav-tabs a[data-target="#entity-attributes-' + tab + '-tab-content-' + $scope.currentfile.FileName + '"]').tab('show');
    }

    $scope.selectAttribute = function (field, isScroll) {
        $scope.clearSelection();
        $scope.selectedItem = field;
        if (isScroll) {
            $scope.scrollToPosition("#attributes", ".entity-dashboard-inner-section-container", $scope.selectedItem.dictAttributes.ID);
        }
    };

    $scope.selectAttributedbclick = function (field) {
        $scope.selectAttribute(field);
        $scope.selectAttributeTab();
        var tabname = "";
        switch (field.dictAttributes.sfwType) {
            case "Property": tabname = 'PROPERTIES'; break;
            case "Column":
            case "Description": tabname = 'COLUMNS'; break;
            case "Expression": tabname = 'EXPRESSION'; break;
            case "Object": tabname = 'ONETOONE'; break;
            case "List":
            case "CDOCollection":
            case "Collection": tabname = 'ONETOMANY'; break;
        }
        $timeout(function () {
            setAttributesActiveTab(tabname);
            $scope.activeGridRow = field;
        });
    };

    // #endregion   

    //#region Expression

    $scope.ValidateExpression = function (expression, astrObjectBased, ablnIsRule, dialogId) {
        var lstEntityModels = [];
        if ($rootScope.lstopenedfiles.length > 0) {
            for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                if ($rootScope.lstopenedfiles[i].file && ($rootScope.lstopenedfiles[i].file.FileType == "Entity" || $rootScope.lstopenedfiles[i].file.FileType == "RuleConstants")) {
                    var scope = getScopeByFileName($rootScope.lstopenedfiles[i].file.FileName);
                    if (scope && scope.objEntity) {
                        var obj = GetBaseModel(scope.objEntity);
                        lstEntityModels.push(obj);
                    }
                    else if (scope && scope.constantfilemodel) {
                        var obj = GetBaseModel(scope.constantfilemodel);
                        lstEntityModels.push(obj);
                    }
                }
            }
        }

        if (lstEntityModels && lstEntityModels.length > 0) {
            var strobj = JSON.stringify(lstEntityModels);
            if (strobj.length < 32000) {
                $.connection.hubEntityModel.server.validateExpression(expression, astrObjectBased, ablnIsRule, $scope.objEntity.dictAttributes.ID, dialogId, strobj);
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
                for (var i = 0; i < lstDataPackets.length; i++) {
                    $.connection.hubEntityModel.server.receiveDataPacketsForValidateExpr(expression, astrObjectBased, ablnIsRule, $scope.objEntity.dictAttributes.ID, dialogId, lstDataPackets[i], lstDataPackets.length, i);
                }
            }
        }

    };

    $scope.setExpression = function (attr) {
        var dialogScope = $scope.$new();
        dialogScope.isObjectBased = attr.dictAttributes.sfwObjectBased && attr.dictAttributes.sfwObjectBased.toLowerCase() == "true";
        dialogScope.expression = attr.dictAttributes.sfwValue;

        dialogScope.receiveValidateExpressionMsg = function (error) {
            $scope.$evalAsync(function () {
                if (attr) {
                    dialogScope.ValidationMessageForDisplay = error;

                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(attr.dictAttributes.sfwValue, attr.dictAttributes, "sfwValue", dialogScope.expression);
                    $rootScope.EditPropertyValue(attr.Error, attr, "Error", error);
                    $rootScope.UndRedoBulkOp("End");
                    if (!error) {
                        dialogScope.closeDialog();
                    }
                }
            });
        };

        dialogScope.onSaveClick = function () {
            var dialogid = "ScopeId_" + dialogScope.$id;
            $scope.ValidateExpression(dialogScope.expression, "False", false, dialogid);
        };
        dialogScope.onCancelClick = function () {

            // attr.Error = dialogScope.ValidationMessageForDisplay;
            if (!attr.Error) {
                if (!attr.dictAttributes.sfwValue) {
                    attr.Error = "Null or Empty expression.";
                }
            }
            dialogScope.closeDialog();
        };
        dialogScope.closeDialog = function () {
            if (dialogScope.dialog && dialogScope.dialog.close) {
                dialogScope.dialog.close();
            }
        };
        dialogScope.dialog = $rootScope.showDialog(dialogScope, "Set Expression", "Entity/views/ExpressionEditorTemplate.html");
    };

    //#endregion





    $scope.closeCollectionDialog = function () {
        $scope.isDialogOKClick = false;
        if (openedDialogId != '') {
            ngDialog.close(openedDialogId);
            var index = $scope.objAttributes.Elements.indexOf($scope.selectedElement);
            $scope.objAttributes.Elements[index] = $scope.temporaryElement;
            openedDialogId = '';
        }
    };

    $scope.setIDPrefix = function (selectedItem, prefix) {
        selectedItem.prefix = prefix;
        if (selectedItem.dictAttributes.ID) {
            if (selectedItem.dictAttributes.ID.indexOf(prefix) == 0) {
                selectedItem.idWithoutPrefix = selectedItem.dictAttributes.ID.substring(prefix.length, selectedItem.dictAttributes.ID.length);
            }
            else {
                selectedItem.idWithoutPrefix = selectedItem.dictAttributes.ID;
            }
        }
        else {
            selectedItem.idWithoutPrefix = "";
        }
    };

    $scope.onIdWithoutPrefixChanged = function (item, olditem, isUpdate) {
        $scope.$evalAsync(function () {
            item.dictAttributes.ID = item.prefix + item.idWithoutPrefix;
            var attributeModel = getAttributesList();
            var currentItemIndex = attributeModel.Elements.indexOf(olditem);
            if (isUpdate) {
                attributeModel.Elements.splice(currentItemIndex, 1);
            }
            var objDuplicate = $ValidationService.findDuplicateId(attributeModel, item.dictAttributes.ID, CONST.ENTITY.NODES, "description", item, "dictAttributes.ID");
            if (objDuplicate) {
                var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                    errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                }
                item.errors.duplicate_id = errorMessage;
            }
            else {
                if (item.errors) {
                    delete item.errors.duplicate_id;
                }
            }
        });
    };

    $scope.attributesMenuOptions = [
        ['Navigate To Entity', function ($itemScope) {
            $scope.NavigateToEntity($itemScope.$parent.attribute.dictAttributes.sfwEntity);
        }, function ($itemScope) {
            return true;
        }],
        //['Find All References', function ($itemScope) {
        //}, function ($itemScope) {
        //    return true;
        //}]
    ];

    $scope.NavigateToEntity = function (fileName) {
        if (fileName) {
            $.connection.hubMain.server.navigateToFile(fileName, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
        $scope.closeCollectionDialog();
    };



    //#endregion

    //#region Query Section
    $scope.queryTypes = ['', 'SelectQuery', 'SubSelectQuery', 'ScalarQuery', 'NonQuery'];
    $scope.queryDataTypes = ['', 'bool', 'DateTime', 'decimal', 'double', 'float', 'int', 'long', 'short', 'string'];
    $scope.parameterTypes = ['', 'int', 'long', 'string', 'decimal', 'DateTime', 'uniqueidentifier'];
    $scope.isMappedColumnSelectAllChecked = false;
    $scope.isXmlFileChecked = true;
    $scope.isCSFileChecked = true;
    $scope.lstQueryGroups = [];
    $scope.lstQueryGroups.push("");

    $scope.filterByGroupName = function (selectedTypeID) {
        if (selectedTypeID) {
            $scope.filterproperty = "sfwGroupName";

        }
        else {
            $scope.filterproperty = "ID";

        }
    }

    $scope.UpdateGroupName = function (astrGroupName) {
        if (astrGroupName) {
            var ablnfound = false;
            for (i = 0; i < $scope.lstQueryGroups.length; i++) {
                if ($scope.lstQueryGroups[i] == astrGroupName) {
                    ablnfound = true;
                    break;
                }
            }
            if (!ablnfound) {
                $scope.lstQueryGroups.push(astrGroupName);
            }
        }
    };


    $scope.SelectEntityQuery = function (obj) {
        if (obj != undefined) {
            var index = $scope.objQueries.Elements.indexOf(obj);
            $scope.selectQuery(obj, index);
        }
    };

    $scope.disableQueryDelete = function () {
        if ($scope.selectedQuery == undefined || $scope.objQueries == undefined || $scope.objQueries.Elements.length == 0) {
            $scope.selectedQuery = undefined;
            return true;
        }
        else {
            return false;
        }
    };

    $scope.selectQueryEntityDashboard = function (obj, astrDivId, isScroll) {
        $scope.clearSelection();
        $scope.objselectedQueryTab = $scope.lstDBQueryTypes[0];
        $scope.selectQuery(obj, astrDivId, isScroll);
    };
    $scope.selectQueryEntityDashboarddbclick = function (obj) {
        $scope.selectQueryEntityDashboard(obj);
        $scope.IsQueriesOpen = true;
        $scope.selectQueriesTab();
        var queryType = "";
        if ($scope.selectedQuery.dictAttributes.sfwQueryType) queryType = $scope.selectedQuery.dictAttributes.sfwQueryType;
        if (queryType == "SelectQuery") {
            $scope.isCustomMappingTab = true;
            // $scope.queryDataTypes.push("EntityTable");
            $scope.selectedQuery.dictAttributes.sfwDataType = 'EntityTable';
        }
        else {
            $scope.isCustomMappingTab = false;
        }
    };

    $scope.clearSelection = function () {
        //  $scope.selectedRow = null;
        $scope.selectedItem = null;
        $scope.objConstraints.SelectedField = null;
        $scope.SelectedRule = null;
        $scope.SelectedCheckList = null;
        $scope.selectedQuery = null;
        $scope.clearValidationRuleSelection();
        $scope.clearBusinessObjectTabSelection();
        $scope.selectedDeleteProperty = null;
    };

    $scope.showSelectedQueryInEditor = function () {
        if ($scope.selectedQuery) {
            var strquery;
            var strQueryAttribute;
            if ($scope.objselectedQueryTab) {
                strQueryAttribute = $scope.objselectedQueryTab.Attribute;
                strquery = $scope.selectedQuery.dictAttributes[strQueryAttribute];
            }
            if (!strquery) {
                strquery = "";
            }
            if (!$scope.QueryEditor) {
                var divId = "Queryeditor_" + $scope.objEntity.dictAttributes.ID;
                var Querypromise = $interval(function () {
                    if ($("#" + divId).length > 0) {
                        $scope.QueryEditor = ace.edit(divId);
                        $scope.QueryEditor.getSession().setMode("ace/mode/sql");
                        $scope.QueryEditor.setFontSize(13);
                        $scope.QueryEditor.resize(true);
                        $scope.QueryEditor.getSession().setValue(strquery);
                        $scope.setAutoCompleteForQuery($scope.QueryEditor);
                        $scope.QueryEditor.getSession().on('change', function (e) {
                            $scope.strQueryValue = $scope.QueryEditor.getValue();
                            if ($scope.QueryEditor.curOp && $scope.QueryEditor.curOp.command.name) {
                                strQueryAttribute = $scope.objselectedQueryTab.Attribute;
                                if ($scope.currentQuery.dictAttributes.ID == $scope.selectedQuery.dictAttributes.ID && $scope.strQueryValue != $scope.selectedQuery.dictAttributes[strQueryAttribute]) {
                                    $scope.$evalAsync(function () {
                                        $rootScope.EditPropertyValue($scope.selectedQuery.dictAttributes[strQueryAttribute], $scope.selectedQuery.dictAttributes, strQueryAttribute, $scope.strQueryValue, "showSelectedQueryInEditor");
                                        if ($scope.selectedQuery.dictAttributes[strQueryAttribute] && $scope.selectedQuery.dictAttributes[strQueryAttribute].trim().toLowerCase().indexOf("select") === 0) {
                                            $scope.IsExecuteQueryShow = true;
                                        }
                                        else {
                                            $scope.IsExecuteQueryShow = false;
                                        }
                                    });
                                }
                            }
                        });
                        $scope.currentQuery = $scope.selectedQuery;
                        $interval.cancel(Querypromise);
                    }
                }, 500);
            }
            if ($scope.QueryEditor) {
                if (strquery != $scope.QueryEditor.getValue()) {
                    $scope.QueryEditor.getSession().setValue(strquery);
                }
                $scope.currentQuery = $scope.selectedQuery;
            }
        }
    }

    $scope.setAutoCompleteForQuery = function (QueryEditor) {
        QueryEditor.setOptions({
            enableBasicAutocompletion: true
        });
        var count = 0;
        $scope.staticWordCompleter = {
            getCompletions: function (editor, session, pos, prefix, callback) {
                var wordList = ["foo", "bar", "baz"];
                wordList.push("Test" + count++);
                callback(null, wordList.map(function (word) {
                    return {
                        caption: word,
                        value: word,
                        meta: ""
                    };
                }));

            }
        }
        QueryEditor.completers = [$scope.staticWordCompleter]
    };

    $scope.selectedQuery = undefined;
    $scope.selectDBTypeTab = function (objDBType) {
        $scope.objselectedQueryTab = objDBType;
        $scope.showSelectedQueryInEditor();
    }
    $scope.selectQuery = function (obj, astrDivId, isScroll) {
        if (obj != undefined) {
            if ($scope.selectedQuery) {
                $scope.selectedQuery.objMappedColumns = null;
            }
            $scope.clearSelection();
            var query = obj.dictAttributes.sfwSql;
            if (!query && obj.dictAttributes.sfwOracle) {
                query = obj.dictAttributes.sfwOracle;
            }
            $scope.IsExecuteQueryShow = false;
            if (query && query.trim().toLowerCase().indexOf("select") === 0) {
                $scope.IsExecuteQueryShow = true;
            }
        }
        $(".list-items-queries").find(".popover").remove();
        $scope.IsQueryReferencePopover = false;
        $scope.selectedQuery = obj;
        $scope.showSelectedQueryInEditor();
        //$scope.ScrollToCurrentQuery();      
        if (obj != undefined) {
            $scope.selectedQueryName = $scope.objEntity.dictAttributes.ID + "." + $scope.selectedQuery.dictAttributes.ID;
            $scope.onQueryTypeChange(obj.dictAttributes.sfwQueryType);
            if ($scope.isCustomMappingTab) {
                $scope.GetMappedColumns();
            }
        }
        if (isScroll) {
            if (astrDivId && astrDivId == "entity-queries-section") {
                $scope.scrollToPosition("#" + astrDivId, ".page-sidebar-content", $scope.selectedQuery.dictAttributes.ID);
            } else {
                $scope.scrollToPosition("#queries", ".entity-dashboard-inner-section-subcontainer", $scope.selectedQuery.dictAttributes.ID);
            }
        }
    };

    $scope.ScrollToCurrentQuery = function () {
        if ($scope.selectedQuery) {
            var item = document.getElementById($scope.objEntity.dictAttributes.ID + "_" + $scope.selectedQuery.dictAttributes.ID);
            if (item) {
                item.scrollIntoView();
            }
            else {
                var selectionInterval = setInterval(function () {
                    var item = document.getElementById($scope.objEntity.dictAttributes.ID + "_" + $scope.selectedQuery.dictAttributes.ID);
                    if (item) {
                        item.scrollIntoView();
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }
                    setTimeout(function () {
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }, 5000);
                }, 200);
            }
        }
    };


    $scope.selectQueryGroup = function (obj, e, astrDivId, isScroll) {
        $scope.selectedQuery = obj;
        $scope.selectQuery(obj, astrDivId, isScroll);
        if (e) {
            e.stopPropagation();
        }
    };
    $scope.removeSelectedQuery = function (selectedTabName) {
        if ($scope.selectedQuery != undefined) {
            if (confirm("Query : '" + $scope.selectedQuery.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                $rootScope.UndRedoBulkOp("Start");
                var index = $scope.objQueries.Elements.indexOf($scope.selectedQuery);
                var flag = true;
                var ablnfound = false;
                if ($scope.selectedQuery.dictAttributes.sfwGroupName) {
                    for (i = 0; i < $scope.objQueries.Elements.length; i++) {
                        if ($scope.objQueries.Elements[i].dictAttributes.ID != $scope.selectedQuery.dictAttributes.ID && $scope.objQueries.Elements[i].dictAttributes.sfwGroupName == $scope.selectedQuery.dictAttributes.sfwGroupName) {
                            ablnfound = true;
                            break;
                        }
                    }

                    if (!ablnfound) {
                        $rootScope.DeleteItem($scope.selectedQuery.dictAttributes.sfwGroupName, $scope.lstQueryGroups, false);
                    }
                }


                $rootScope.DeleteItem($scope.selectedQuery, $scope.objQueries.Elements, selectedTabName);
                $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.selectedQuery);
                if (flag == true) {

                    if ($scope.objQueries.Elements.length > 0 && selectedTabName) {
                        if (index != -1) {
                            if (index != 0) {
                                $scope.selectedQuery = $scope.objQueries.Elements[index - 1];
                                $scope.selectQuery($scope.selectedQuery, index - 1);
                            } else {
                                if ($scope.objQueries.Elements.length > 0) {
                                    $scope.selectedQuery = $scope.objQueries.Elements[index];
                                    $scope.selectQuery($scope.selectedQuery, index);
                                }
                            }
                        }
                    }
                    else {
                        $scope.clearSelection();
                    }
                }

                $rootScope.UndRedoBulkOp("End");
                validateQueries(true);
            }
        }
    };
    $scope.canDeleteQuery = function () {

        if ($scope.objQueries && $scope.objQueries.Elements.length > 0) {
            return $scope.selectedQuery != undefined;
        }
        else {
            return false;
        }

    };
    //on clicking '+' , new query dialog will be shown
    $scope.ExpandCollapsedQueryGroup = function (obj) {
        obj.IsExpanded = !obj.IsExpanded;
        //expandCollapseConstants(event);
    };

    $scope.addNewQuery = function (astrDivId) {
        var newScope = $scope.$new(true);
        newScope.IsExecuteQueryShow = false;
        $scope.IsExecuteQueryShow = false;
        var newQueryID = GetNewQueryName("NewQuery", $scope.objQueries, 1);
        newScope.newQueryName = newQueryID;
        newScope.isEditQueryID = false;
        newScope.queryError = undefined;

        var addNewQueryDialog = $rootScope.showDialog(newScope, "Add Query Name", "Entity/views/NewQueryTemplate.html", {
            width: 500, height: 180
        });


        newScope.validateQueryDailog = function () {
            newScope.queryError = undefined;
            if (!newScope.newQueryName) {
                newScope.queryError = "Error: Enter ID.";
                return;
            }
            else if (!isValidIdentifier(newScope.newQueryName)) {
                newScope.queryError = "Error:Invalid Query ID.";
                return;
            }
            else {
                var objDuplicate = $ValidationService.findDuplicateId($scope.objEntity, newScope.newQueryName, CONST.ENTITY.NODES, "", null, "dictAttributes.ID");
                if (objDuplicate) {
                    newScope.queryError = CONST.VALIDATION.DUPLICATE_ID;
                    return;
                }
            }
        };

        newScope.setNewQueryDetails = function () {
            if (!newScope.queryError) {
                $rootScope.UndRedoBulkOp("Start");
                $scope.IsExecuteQueryShow = false;

                var objNewQuery = {
                };

                objNewQuery.Name = 'query';
                objNewQuery.dictAttributes = {
                    ID: '', sfwQueryType: '', sfwDataType: '', sfwSql: ''
                };
                objNewQuery.dictAttributes.ID = this.newQueryName;
                objNewQuery.dictAttributes.sfwQueryType = 'SelectQuery';
                objNewQuery.dictAttributes.sfwDataType = 'EntityTable';
                if ($scope.objEntity.dictAttributes.sfwTableName != undefined) {
                    objNewQuery.dictAttributes.sfwSql = 'select * from ' + $scope.objEntity.dictAttributes.sfwTableName;
                }
                objNewQuery.Elements = [];
                var newQueryElement = {
                };
                newQueryElement.Value = '';
                newQueryElement.Name = 'parameters';
                newQueryElement.Elements = [];
                newQueryElement.dictAttributes = {
                };
                objNewQuery.Elements.push(newQueryElement);

                $rootScope.PushItem(objNewQuery, $scope.objQueries.Elements, "SelectEntityQuery");
                $scope.selectedQuery = objNewQuery;
                $scope.onQueryTypeChange(objNewQuery.dictAttributes.sfwQueryType);
                $scope.selectQuery($scope.objQueries.Elements[$scope.objQueries.Elements.length - 1], $scope.objQueries.Elements.length - 1);
                if (astrDivId && astrDivId == "entity-queries-section") {
                    $scope.scrollToPosition("#" + astrDivId, ".page-sidebar-content", objNewQuery.dictAttributes.ID);
                }
                else {
                    $scope.scrollToPosition("#queries", ".entity-dashboard-inner-section-subcontainer", objNewQuery.dictAttributes.ID);
                }

                newScope.closeNewQueryDialog();

                $rootScope.UndRedoBulkOp("End");
                $scope.$evalAsync(function () {
                    var elem = $("div.selected")[0];
                    if (elem) {
                        $('.page-sidebar-content').scrollTo(elem, null, null);
                    }
                });

            }
        };

        newScope.closeNewQueryDialog = function () {
            addNewQueryDialog.close();
        };
    };
    $scope.closeNewQueryDialog = function () {
        if (openedQueryDialogId != undefined)
            ngDialog.close(openedQueryDialogId);
        openedQueryDialogId = '';
    };


    //#endregion

    //#region Validation Rules Section

    //#region Entity DashBoard Rules Section
    $scope.SelectedDashBoardRule = "InitialLoad";
    $scope.setSelectedRuleFromDashBoard = function (selectedRuleName) {
        $scope.SelectedDashBoardRule = selectedRuleName;
    };

    $scope.SetSelectedValidationRule = function (objselectedRule, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        $scope.SelectedRule = objselectedRule;
        if (isScroll) {
            $scope.scrollToPosition("#validations", ".validation-panel-left", $scope.SelectedRule.dictAttributes.ID);
        }
    };

    $scope.SetSelectedValidationRuledbclick = function (objselectedRule) {
        $scope.SetSelectedValidationRule(objselectedRule);
        $scope.selectValidationRulesTab();
    };

    $scope.DeleteValidationRule = function () {
        if ($scope.SelectedRule != undefined) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = true;
            });
            //Below hub call is to get list of validation rule ids, which are being used in child entities. 
            //Result dictionary is being used for validation at the time of delete/rename validation rule.
            $.connection.hubEntityModel.server.getRulesUsedInChildEntities($scope.objEntity.dictAttributes.ID, $scope.SelectedRule.dictAttributes.ID).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
                var Error = "";
                var objselectedrule;
                if (data && data.length > 0) {
                    objselectedrule = data[0];
                } else {
                    objselectedrule = $scope.SelectedRule;
                }

                if (objselectedrule) {
                    Error = CheckingSelectedRule(objselectedrule, "deleted");
                }

                if (Error) {
                    alert(Error);
                }
                else {

                    var result = confirm("Do you want to remove the selected rule '" + $scope.SelectedRule.dictAttributes.ID + "'from this section ?");
                    if (result == true) {
                        var objParent = $scope.GetParent($scope.SelectedRule, $scope.objRules);
                        if (objParent != undefined) {
                            var index = objParent.Elements.indexOf($scope.SelectedRule);


                            if (index > -1) {

                                $rootScope.DeleteItem($scope.SelectedRule, objParent.Elements, "SelectValidationRules");
                                validateValidationRulesID(true);
                                $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.SelectedRule);
                                // objParent.Elements.splice(index, 1);
                                if (index < objParent.Elements.length) {
                                    $scope.SelectedRule = objParent.Elements[index];
                                    $scope.selectedRules($scope.SelectedRule, null);
                                }
                                else if (objParent.Elements.length > 0) {
                                    $scope.SelectedRule = objParent.Elements[index - 1];
                                    $scope.selectedRules($scope.SelectedRule, null);
                                }
                                else {
                                    $scope.selectedRules(objParent, null);
                                }
                            }
                        }
                    }
                }
            });

        }
    };

    $scope.setSelectedInitialLoadFromDashBoard = function (objselectedinitialload, isFromDashboard, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        if (!$scope.IsInitialLoadLoaded) {
            $scope.IsInitialLoadLoaded = true;
        }
        $scope.SelectedInitialLoadClick(objselectedinitialload, undefined, isFromDashboard, isScroll);
    };

    $scope.setSelectedInitialLoadFromDashBoarddbclick = function (obj) {
        $scope.setSelectedInitialLoadFromDashBoard(obj);
        $scope.selectValidationRulesTab();
    };

    $scope.DeleteInitialLoadFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedInitialLoad, 'InitialLoad');
    };

    $scope.setSelectedHardErrorFromDashBoard = function (objselectedharderror, isFromDashboard, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        if (!$scope.IsHardErrorLoaded) {
            $scope.IsHardErrorLoaded = true;
        }
        $scope.SelectHardError(objselectedharderror, isFromDashboard, isScroll);
    };
    $scope.setSelectedHardErrorFromDashBoarddbclick = function (obj) {
        $scope.setSelectedHardErrorFromDashBoard(obj);
        $scope.selectValidationRulesTab();
    };

    $scope.DeleteHardErrorFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedHardError, 'HardError');
    };

    $scope.setSelectedSoftErrorFromDashBoard = function (objselectedsofterror, isFromDashboard, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        if (!$scope.IsSoftErrorLoaded) {
            $scope.IsSoftErrorLoaded = true;
        }
        $scope.SelectSoftError(objselectedsofterror, isFromDashboard, isScroll);
    };
    $scope.setSelectedSoftErrorFromDashBoarddbclick = function (objselectedsofterror) {
        $scope.setSelectedSoftErrorFromDashBoard(objselectedsofterror);
        $scope.selectValidationRulesTab();
    };

    $scope.DeleteSoftErrorFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedSoftError, 'SoftError');
    };

    $scope.setSelectedGroupFromDashBoard = function (objselectedgroup, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        if (!$scope.IsGroupsLoaded) {
            $scope.IsGroupsLoaded = true;
        }
        $scope.SelectGroupClick(objselectedgroup, null, isScroll);
    };
    $scope.setSelectedGroupFromDashBoarddbclick = function (objselectedgroup) {
        $scope.setSelectedGroupFromDashBoard(objselectedgroup);
        $scope.selectValidationRulesTab();
    };
    $scope.DeleteGroupFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedGroup, 'Group');
    };

    $scope.setSelectedValidateDeleteFromDashBoard = function (objselectedvalidateDelete, isFromDashboard, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "ValidationRules";
        if (!$scope.IsValidateDeleteLoaded) {
            $scope.IsValidateDeleteLoaded = true;
        }
        $scope.SelectedValidateDeleteClick(objselectedvalidateDelete, undefined, isFromDashboard, isScroll);
    };
    $scope.setSelectedValidateDeleteFromDashBoarddbclick = function (obj) {
        $scope.setSelectedValidateDeleteFromDashBoard(obj);
        $scope.selectValidationRulesTab();
    };
    $scope.DeleteValidateDeleteFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedValidateDelete, 'ValidateDelete');
    };

    $scope.setSelectedCheckListFromDashBoard = function (objselectedchecklist, isFromDashboard, isScroll) {
        $scope.clearSelection();
        $scope.SelectedRuleView = "Checklist";
        if (!$scope.IsCheckListLoaded) {
            $scope.IsCheckListLoaded = true;
        }
        if (!isFromDashboard) isFromDashboard = false;
        $scope.SelectedCheckList = objselectedchecklist;
        if (objselectedchecklist.objRule && objselectedchecklist.objRule.dictAttributes && isFromDashboard == false) {
            $scope.SelectValidationRules(objselectedchecklist.objRule);
        }
        if (isScroll) {
            $scope.scrollToPosition("#dsb-checklist", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedCheckList.dictAttributes.ID);
        }
    };
    $scope.setSelectedCheckListFromDashBoarddbclick = function (obj) {
        $scope.selectValidationRulesTab();
        $scope.setSelectedCheckListFromDashBoard(obj);
    };
    $scope.DeleteCheckListFromDashBoard = function () {
        $scope.DeleteRuleFromContextMenu($scope.SelectedCheckList, 'Checklist');
    };

    $scope.AddRuleFromDashBoard = function () {
        var newScope = $scope.$new();
        newScope.RuleID = "";
        newScope.isObjectBasedRule = false;
        newScope.ExpressioninRule = "";
        newScope.validationRuleError = "Error: Enter ID.";
        var dialog = $rootScope.showDialog(newScope, "Add Validation Rules", "Entity/views/AddNewRule.html", {
            width: 500, height: 290
        });
        newScope.OnRuleTemplateCancelClick = function () {
            dialog.close();
        };
        newScope.validateValidationRuleDailog = function () {
            newScope.validationRuleError = undefined;
            if (!newScope.RuleID) {
                newScope.validationRuleError = "Error: Enter ID.";
                return;
            } else {
                var objParentRules = null;
                if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
                    objParentRules = $scope.objInheritedRules;
                }
                var ruleModel = getRuleModel(objParentRules);
                var objDuplicate = $ValidationService.findDuplicateId(ruleModel, newScope.RuleID, CONST.ENTITY.NODES, "", null, "dictAttributes.ID");
                if (objDuplicate) {
                    var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                    if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                        errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                    }
                    newScope.validationRuleError = errorMessage;
                    return;
                }
            }
        };

        newScope.onRuleTemplateOkClick = function () {
            if (this.RuleID != "") {
                var objRule = {
                    Children: [], dictAttributes: { ID: this.RuleID, sfwObjectBased: this.isObjectBasedRule, sfwExpression: this.ExpressioninRule, sfwRuleType: 'Validation' }, Elements: [], Name: 'rule', Value: ''
                };
                $rootScope.PushItem(objRule, $scope.objRules.Elements, "SelectValidationRules");
                //$scope.objRules.Elements.push(objRule);
                $scope.SelectedRule = objRule;
                $scope.scrollToPosition("#validations", ".validation-panel-left", objRule.dictAttributes.ID);
            }
            newScope.OnRuleTemplateCancelClick();
        };
    };

    $scope.refreshInheritedRules = function () {
        hubMain.server.getInheritedRule($scope.objEntity.dictAttributes.ID, $scope.objEntity.dictAttributes.sfwParentEntity).done(function (data) {
            if (data) {

                $scope.$evalAsync(function () {
                    $scope.selectedIneritedTab = 'InheritedRule';
                    $scope.objInheritedRules = data;
                    if ($scope.objInheritedRules.Elements && $scope.objInheritedRules.Elements.length > 0) {
                        $scope.selectedRules($scope.objInheritedRules.Elements[0]);
                    }
                    else {
                        $scope.selectedRules(undefined);
                    }
                    $scope.objInheritedRules.Text = "Rules";
                    $scope.objInheritedRules.IsExpanded = true;
                    if ($scope.objInheritedRules && $scope.objInheritedRules.Elements.length > 0) {
                        angular.forEach($scope.objInheritedRules.Elements, function (obj) {
                            $scope.LoadRulesDetails(obj);
                        });
                    }

                });
            }
        });

    }

    //#endregion

    $scope.RulesMenuOptions = [


        ['Delete Rule', function ($itemScope) {

            if ($itemScope.element != undefined) {
                $scope.SelectedRule = $itemScope.element;
                $scope.DeleteValidationRule();

            }

        }, function ($itemScope) {

            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == "rule") {

                    return true;
                }

                else {
                    return false;
                }
            }
        }],


        ['Rename Rule', function ($itemScope) {
            if ($itemScope.element != undefined) {
                $scope.SelectedRule = $itemScope.element;

                var Error = CheckingSelectedRule($scope.SelectedRule, "renamed");

                if (Error) {

                    alert(Error);
                }
                else {
                    var newScope = $scope.$new(true);
                    newScope.selectedRule = $itemScope.element;
                    newScope.objRules = $scope.objRules;
                    if (newScope.selectedRule != undefined) {
                        newScope.newRuleID = newScope.selectedRule.dictAttributes.ID;
                        newScope.IsEdit = true;

                        var editRuleDialog = $rootScope.showDialog(newScope, "Edit ID", "Entity/views/AddRule.html", {
                            width: 500, height: 180
                        });

                        newScope.validateNewGroup = function (type) {
                            newScope.groupErrorMessageForDisplay = "";
                            var NewId = "";
                            if (type == "Group") {
                                NewId = this.newGroupID.toLowerCase();
                            }
                            else if (type == "Rule") {
                                NewId = this.newRuleID.toLowerCase();
                            }
                            if (NewId == undefined || NewId == '') {
                                newScope.groupErrorMessageForDisplay = "Error: ID cannot be empty.";
                                return true;
                            }
                            else {
                                var objParentRules = null;
                                if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
                                    objParentRules = $scope.objInheritedRules;
                                }
                                var ruleModel = getRuleModel(objParentRules);
                                var objDuplicate = $ValidationService.findDuplicateId(ruleModel, NewId, CONST.ENTITY.NODES, "item", $scope.SelectedRule, "dictAttributes.ID");
                                if (objDuplicate) {
                                    var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                                    if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                                        errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                                    }
                                    newScope.groupErrorMessageForDisplay = errorMessage;
                                    return;
                                }
                            }
                            return false;
                        };

                        newScope.addNewRule = function (Rule) {
                            if (newScope.IsEdit) {
                                $rootScope.UndRedoBulkOp("Start");
                                if ($scope.SelectedRule.IsInitialLoad != undefined && $scope.SelectedRule.IsInitialLoad) {
                                    $scope.renameSubSectionRules($scope.objInitialLoad, this.newRuleID);
                                }
                                if ($scope.SelectedRule.IsHardError != undefined && $scope.SelectedRule.IsHardError) {
                                    $scope.renameSubSectionRules($scope.objHardError, this.newRuleID);
                                }
                                if ($scope.SelectedRule.IsSoftError != undefined && $scope.SelectedRule.IsSoftError) {
                                    $scope.renameSubSectionRules($scope.objSoftError, this.newRuleID);
                                }
                                if ($scope.SelectedRule.IsValidateDelete != undefined && $scope.SelectedRule.IsValidateDelete) {
                                    $scope.renameSubSectionRules($scope.objValidateDelete, this.newRuleID);
                                }
                                if ($scope.SelectedRule.IsCheckList != undefined && $scope.SelectedRule.IsCheckList) {
                                    $scope.renameSubSectionRules($scope.objCheckList, this.newRuleID);
                                }
                                if ($scope.SelectedRule.IsGroup != undefined && $scope.SelectedRule.IsGroup) {
                                    $scope.renameSubSectionRules($scope.objGroupList, this.newRuleID);
                                }
                                $rootScope.EditPropertyValue($scope.SelectedRule.dictAttributes.ID, $scope.SelectedRule.dictAttributes, "ID", this.newRuleID);
                                $rootScope.UndRedoBulkOp("End");
                            }

                            validateValidationRulesID(true);
                            newScope.closeNewRule();
                        };

                        newScope.closeNewRule = function () {
                            newScope.IsEdit = false;
                            newScope.IsCopy = false;
                            editRuleDialog.close();
                        };
                    }
                }

            }
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == "rule") {
                    $scope.SelectedRule = $itemScope.element;
                    return true;
                }
            }
            else
                return false;
        }],


        null

    ];

    $scope.menuoptionforexpression = [
        ['Copy Expression', function ($itemScope) {
            if ($itemScope.$parent.attribute) {
                $scope.Copiedexpression = $itemScope.$parent.attribute;
                var newExpression = {
                };
                newExpression.Value = '';
                newExpression.Name = 'attribute';
                newExpression.Elements = [];
                var newid = $scope.Copiedexpression.dictAttributes.ID;

                if ($scope.Copiedexpression.dictAttributes.ID) {
                    newid = GetNewQueryName($scope.Copiedexpression.dictAttributes.ID, $scope.objAttributes, 1);
                }

                newExpression.dictAttributes = {
                    ID: newid, sfwDataType: $scope.Copiedexpression.dictAttributes.sfwDataType, sfwDataFormat: $scope.Copiedexpression.dictAttributes.sfwDataFormat, sfwPrivate: $scope.Copiedexpression.dictAttributes.sfwPrivate, sfwValue: $scope.Copiedexpression.dictAttributes.sfwValue, sfwEnumerator: $scope.Copiedexpression.dictAttributes.sfwEnumerator, Text: $scope.Copiedexpression.dictAttributes.Text, sfwType: "Expression"
                };
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(newExpression, $scope.objAttributes.Elements, null);
                $scope.selectExpression(newExpression, true);
                //$scope.setObjectEditable(newExpression);
                $rootScope.UndRedoBulkOp("End");
                $scope.validateId($scope.objAttributes.Elements[$scope.objAttributes.Elements.length - 1], newid);
                $scope.setFocustoTextbox('attributesexpression', true, $scope.objAttributes.Elements[$scope.objAttributes.Elements.length - 1]);
            }
        }, function ($itemScope) {
            return true;

        }], null

    ];

    var CheckingSelectedRule = function (SelectedRule, astrOperation) {
        var strError = "";
        if (SelectedRule.IsInitialLoad) {
            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Initial Load section of this or child entities.";
        }
        else if (SelectedRule.IsHardError || SelectedRule.IsHardErrorAsChild) {
            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Hard Errors section of this or child entities.";
        }
        else if (SelectedRule.IsSoftError || SelectedRule.IsSoftErrorAsChild) {

            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Soft Errors section of this or child entities.";
        }
        else if (SelectedRule.IsValidateDelete) {
            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Validate Delete section of this or child entities.";
        }
        else if (SelectedRule.IsCheckList) {
            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Check List section of this or child entities.";
        }
        else if (SelectedRule.IsGroup) {
            strError = "The selected Rule cannot be " + astrOperation + " because it is being used in Group List section of this or child entities.";
        }
        return strError;
    };

    $scope.onMessageIdChange = function () {
        if ($scope.SelectedRule) {
            $scope.SelectedRule.dictAttributes.sfwMessageId = "";
            $scope.SelectedRule.dictAttributes.sfwMessage = "";
            $scope.SelectedRule.lstMessageParams = [];
        }

        if ($scope.SelectedRule.IsMessageSelected == "MessageID") {
            var ruleObj = $scope.objRules.Elements.filter(function (rObj) { if (rObj && rObj.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) { return rObj; } });
            if (ruleObj && ruleObj.length) {
                $scope.validateEmptyMessageID(ruleObj[0]);
            }
        }
        else if ($scope.SelectedRule.IsMessageSelected == "Description") {
            $ValidationService.validateIsEmptyMessageIdAndDescription($scope.SelectedRule, $scope.validationErrorList, $scope.SelectedRule.dictAttributes.sfwMessageId, $scope.SelectedRule.dictAttributes.sfwMessage);
        }
    };

    $scope.selectedRulesTrack = [];
    $scope.selectedRules = function (obj, event, isScroll) {
        if (obj != undefined) {
            $scope.selectValidationRuleInAllTypes(obj, isScroll, "");
        }
        else {
            $scope.SelectedRule = undefined;
        }
        if (event != null) {
            event.stopPropagation();
        }
    };

    $scope.SelectValidationRules = function (obj) {
        var isNotInheritedRule = false;
        if (obj) {
            $scope.selectedIneritedTab = 'Rule';
            $scope.SelectedRule = obj;
            if ($scope.SelectedRule.IsMessageSelected == undefined || $scope.SelectedRule.IsMessageSelected == "") {
                $scope.SelectedRule.IsMessageSelected = 'MessageID';
            }
            else if ($scope.SelectedRule.dictAttributes.sfwMessageId != undefined && $scope.SelectedRule.dictAttributes.sfwMessageId != "") {
                $scope.SelectedRule.IsMessageSelected = 'MessageID';
            }
            if ($scope.SelectedRule.dictAttributes.sfwMessage != undefined && $scope.SelectedRule.dictAttributes.sfwMessage != "") {
                $scope.SelectedRule.IsMessageSelected = 'Description';
            }
            if (obj.Name == "rule" && $scope.SelectedView == "ValidationRules") {

                $scope.IsRule = true;
            }
            else {
                $scope.IsRule = false;

            }

            if (angular.isArray($scope.objRules.Elements) && $scope.objRules.Elements.indexOf(obj) > -1) { // first check if rule present in current entity
                isNotInheritedRule = true;
            }

            // if any inherited rule is selected - show the inherted rule tab
            if ($scope.objInheritedRules && $scope.objInheritedRules.Elements && !isNotInheritedRule) {
                $scope.objInheritedRules.Elements.some(function (rule) {
                    if (rule.dictAttributes.ID == obj.dictAttributes.ID) {
                        $scope.selectedIneritedTab = 'InheritedRule';
                        return true;
                    }
                });
            }
        }
        else {
            $scope.SelectedRule = undefined;
            $scope.IsRule = false;
        }
    };

    $scope.onValidateExpression = function () {
        if ($scope.SelectedRule) {
            $scope.SelectedRule.Error = "";
            $scope.ValidateExpression($scope.SelectedRule.dictAttributes.sfwExpression, $scope.SelectedRule.dictAttributes.sfwObjectBased, true, "");
        }
    };

    $scope.receiveValidateExpressionMsg = function (error) {
        $scope.$evalAsync(function () {
            if ($scope.SelectedRule) {
                $scope.SelectedRule.Error = error;
            }
        });
    };

    $scope.SetClass = function (obj, step) {
        var retClass = "";
        if (obj == step) {
            retClass = "active";

            retClass = "selected";
        }
        if ((obj.Name == "rule" || obj.Name == "rules") && $scope.objCut) {
            if (angular.equals($scope.objCut, obj)) {
                retClass = retClass + " redforeground";
            }
        }
        if (obj.isAdvanceSearched) {
            retClass = retClass + " bckgGrey";
        }
        if (obj == step && obj.isAdvanceSearched) {
            retClass = retClass + " bckgGreen";
        }
        return retClass;
    };

    $scope.SelectChildRuleView = function (viewName) {
        if (viewName == "ValidationRules") {
            $scope.SelectedRuleView = viewName;
            if (!$scope.SelectedRule) {
                if ($scope.objRules.Elements.length > 0) {
                    $scope.selectedRules($scope.objRules.Elements[0]);
                }
            }
        } else if (viewName == "InitialLoad") {
            if (!$scope.IsInitialLoadLoaded) {
                $scope.IsInitialLoadLoaded = true;
            }
            if (!$scope.SelectedInitialLoad) {
                if ($scope.objInitialLoad.Elements.length > 0) {
                    $scope.SelectedInitialLoadClick($scope.objInitialLoad.Elements[0]);
                }
            }
        } else if (viewName == "HardError") {
            if (!$scope.IsHardErrorLoaded) {
                $scope.IsHardErrorLoaded = true;
            }
            if (!$scope.SelectedHardError) {
                if ($scope.objHardError.Elements.length > 0) {
                    $scope.SelectHardError($scope.objHardError.Elements[0]);
                }
            }
        }
        else if (viewName == "SoftError") {
            if (!$scope.IsSoftErrorLoaded) {
                $scope.IsSoftErrorLoaded = true;
            }
            if (!$scope.SelectedSoftError) {
                if ($scope.objSoftError.Elements.length > 0) {
                    $scope.SelectSoftError($scope.objSoftError.Elements[0]);
                }
            }
        } else if (viewName == "ValidateDelete") {
            if (!$scope.IsValidateDeleteLoaded) {
                $scope.IsValidateDeleteLoaded = true;
            }
            if (!$scope.SelectedValidateDelete) {
                if ($scope.objValidateDelete.Elements.length > 0) {
                    $scope.SelectedValidateDeleteClick($scope.objValidateDelete.Elements[0]);
                }
            }
        }
        else if (viewName == "Group") {
            if (!$scope.IsGroupsLoaded) {
                $scope.IsGroupsLoaded = true;
            }
            if (!$scope.SelectedGroup) {
                if ($scope.objGroupList.Elements.length > 0) {
                    $scope.SelectGroupClick($scope.objGroupList.Elements[0]);
                }
            }
        }

        else if (viewName == "Checklist") {
            $scope.SelectedRuleView = viewName;
            if (!$scope.IsCheckListLoaded) {
                $scope.IsCheckListLoaded = true;
            }
            if (!$scope.SelectedCheckList) {
                if ($scope.objCheckList.Elements.length > 0) {
                    $scope.SelectCheckListClick($scope.objCheckList.Elements[0]);
                }
            }
        }
        if ($scope.selectedIneritedTab && $scope.selectedIneritedTab == 'InheritedRule') {
            if ($scope.objInheritedRules && $scope.objInheritedRules.Elements && $scope.objInheritedRules.Elements.length > 0) {
                $scope.selectedRules($scope.objInheritedRules.Elements[0], null);
            }
            else {
                $scope.selectedRules(undefined, null);
            }
        }
    };

    $scope.selectRuleType = function (obj, isSearchChild) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        $scope.selectValidationRuleInAllTypes(obj, true, "", isSearchChild);
    };
    $scope.SelectedInitialLoadClick = function (obj, event, isFromDashboard, isScroll) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        if (!isFromDashboard) isFromDashboard = false;
        if (obj != undefined) {

            if (!isFromDashboard) {
                $scope.selectValidationRuleInAllTypes(obj, true, "InitialLoad");
            }
            $scope.SelectedInitialLoad = obj;
            if (isScroll && isFromDashboard) {
                $scope.scrollToPosition("#dsb-initial-load", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedInitialLoad.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        else {
            $scope.SelectedInitialLoad = undefined;
        }
        if (event != undefined && event != null) {
            event.stopPropagation();
        }
    };

    $scope.SelectSoftError = function (obj, isFromDashboard, isScroll) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        if (!isFromDashboard) isFromDashboard = false;
        if (obj) {
            if (!isFromDashboard) {
                $scope.selectValidationRuleInAllTypes(obj, true, "SoftError");
            }
            $scope.SelectedSoftError = obj;
            if (isScroll && isFromDashboard) {
                $scope.scrollToPosition("#dsb-softerror", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedSoftError.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        else {
            $scope.SelectedSoftError = undefined;
        }
    };

    $scope.SelectHardError = function (obj, isFromDashboard, isScroll) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        if (!isFromDashboard) isFromDashboard = false;
        if (obj) {

            if (!isFromDashboard) {
                $scope.selectValidationRuleInAllTypes(obj, true, "HardError");
            }
            $scope.SelectedHardError = obj;
            if (isScroll && isFromDashboard) {
                $scope.scrollToPosition("#dsb-harderror", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedHardError.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        else {
            $scope.SelectedHardError = undefined;
        }
    };
    $scope.SelectedErrorClick = function (obj, event, SelectedRuleView, isScroll) {
        if (obj != undefined) {
            $scope.clearSelection();
            if (SelectedRuleView == 'HardError') {
                $scope.SelectHardError(obj, false, isScroll);
            }
            else if (SelectedRuleView == 'SoftError') {
                $scope.SelectSoftError(obj, false, isScroll);
            }
        }

        if (event != undefined || event != null) {
            event.stopPropagation();
        }
    };

    $scope.SelectedValidateDeleteClick = function (obj, event, isFromDashboard, isScroll) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        if (!isFromDashboard) isFromDashboard = false;
        if (obj != undefined) {
            $scope.clearSelection();
            if (!isFromDashboard) {
                $scope.selectValidationRuleInAllTypes(obj, true, "ValidateDelete");
            }
            $scope.SelectedValidateDelete = obj;
            if (isScroll && isFromDashboard) {
                $scope.scrollToPosition("#dsb-validatedelete", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedValidateDelete.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        else {
            $scope.SelectedValidateDelete = undefined;
        }

        if (event != undefined || event != null) {
            event.stopPropagation();
        }
    };

    $scope.SelectGroupClick = function (obj, event, isScroll) {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        if (obj != undefined) {
            // custom condition for group selection - obj can be group or item(with rule)
            if (obj.Name != 'group') {
                $scope.clearSelection();
            }
            else if (obj.Name == 'group') {
                $scope.SelectedInitialLoad = undefined;
                $scope.SelectedHardError = undefined;
                $scope.SelectedSoftError = undefined;
                $scope.SelectedValidateDelete = undefined;
            }
            if (obj.Name != 'group') {
                $scope.selectValidationRuleInAllTypes(obj, true, "GroupList");
            }
            $scope.SelectedGroup = obj;

            if (isScroll) {
                $scope.scrollToPosition("#dsb-groups", ".entity-dashboard-inner-section-subcontainer", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        else {
            $scope.SelectedGroup = undefined;
        }
        if (event != undefined || event != null) {
            event.stopPropagation();
        }
    };

    $scope.SelectCheckListClick = function (obj, event, isScroll) {
        if (obj != undefined) {
            $scope.clearSelection();
            $scope.SelectedCheckList = obj;
            if (obj.objRule && obj.objRule.dictAttributes) {
                $scope.SelectValidationRules(obj.objRule);
            }
            if (isScroll) {
                $scope.scrollToPosition("#rule-checklist", ".page-sidebar-content", $scope.SelectedCheckList.dictAttributes.ID, { offsetTop: 400 });
            }
        }
        if (event != undefined || event != null) {
            event.stopPropagation();
        }
    };

    $scope.selectRuleDetailsFromInheritedRule = function (obj) {
        if ($scope.objInheritedRules && $scope.objInheritedRules.Elements && $scope.objInheritedRules.Elements.length > 0) {
            for (var i = 0; i < $scope.objInheritedRules.Elements.length; i++) {
                if ($scope.objInheritedRules.Elements[i].dictAttributes.ID == obj.dictAttributes.ID) {
                    obj.objRule = $scope.objInheritedRules.Elements[i];
                    populateMessageByMessageID(obj.objRule.dictAttributes.sfwMessageId, $scope.lstMessages, obj.objRule, false);
                }
            }
        }
    };
    var LoadParentDetails = function (objRule) {
        var objParent;
        angular.forEach(objRule.Elements, function (item) {
            if (objParent == undefined) {
                if (item.Name == 'rules' || item.Name == 'expressions') {
                    objParent = LoadParentDetails(item);
                }
                else if (item.Name == "rule") {
                    if (item.dictAttributes.ID == $scope.objCut.dictAttributes.ID) {
                        objParent = objRule;

                    }
                }
            }
        });
        return objParent;
    };

    $scope.LoadRulesForChild = function (list) {
        angular.forEach(list, function (item) {
            if (item.Name == 'rules') {
                $scope.LoadRulesForChild(item.Elements);
            }
            else if (item.Name == "rule") {
                $scope.LoadRulesDetails(item);
            }

        });
    };

    $scope.LoadRulesDetails = function (obj) {
        angular.forEach($scope.objInitialLoad.Elements, function (item) {
            if (item.dictAttributes.ID == obj.dictAttributes.ID) {
                obj.IsInitialLoad = true;
                item.objRule = obj;
            }

        });
        angular.forEach($scope.objHardError.Elements, function (item) {

            if (item.dictAttributes.ID == obj.dictAttributes.ID) {
                obj.IsHardError = true;
                item.objRule = obj;
            }

            angular.forEach(item.Children, function (rule) {
                if (rule.dictAttributes.ID == obj.dictAttributes.ID) {
                    obj.IsHardErrorAsChild = true;
                    rule.objRule = obj;
                }
            });

        });
        angular.forEach($scope.objSoftError.Elements, function (item) {

            if (item.dictAttributes.ID == obj.dictAttributes.ID) {
                obj.IsSoftError = true;
                item.objRule = obj;
            }

            angular.forEach(item.Children, function (rule) {
                if (rule.dictAttributes.ID == obj.dictAttributes.ID) {
                    obj.IsSoftErrorAsChild = true;
                    rule.objRule = obj;
                }
            });

        });
        angular.forEach($scope.objValidateDelete.Elements, function (item) {

            if (item.dictAttributes.ID == obj.dictAttributes.ID) {
                obj.IsValidateDelete = true;
                item.objRule = obj;
            }

        });
        angular.forEach($scope.objCheckList.Elements, function (item) {

            if (item.dictAttributes.ID == obj.dictAttributes.ID) {
                obj.IsCheckLisk = true;
                item.objRule = obj;
            }

        });

        angular.forEach($scope.objGroupList.Elements, function (item) {

            $scope.LoadDetailsForGroup(item, obj);

        });

        //Below code is added to check if, the rule is being used in any of the child entities.
        if ($scope.rulesUsedInChildEntities && $scope.rulesUsedInChildEntities.length > 0) {
            var rule = $scope.rulesUsedInChildEntities.filter(function (rule) { return rule.RuleID === obj.dictAttributes.ID; });
            if (rule && rule.length > 0) {
                obj.IsInitialLoad = obj.IsInitialLoad || rule[0].IsInitialLoad;
                obj.IsHardError = obj.IsHardError || rule[0].IsHardError;
                obj.IsSoftError = obj.IsSoftError || rule[0].IsSoftError;
                obj.IsValidateDelete = obj.IsValidateDelete || rule[0].IsValidateDelete;
                obj.IsCheckList = obj.IsCheckList || rule[0].IsCheckList;
                obj.IsGroup = obj.IsGroup || rule[0].IsGroupsList;
            }
        }
    };

    $scope.LoadDetailsForGroup = function (item, obj) {
        angular.forEach(item.Elements, function (itm) {
            if (itm.dictAttributes.ID == obj.dictAttributes.ID) {
                itm.objRule = obj;
                obj.IsGroup = true;
            }

            if (itm.Name == "group") {
                $scope.LoadDetailsForGroup(itm, obj);
            }

        });
    };
    $scope.populateMessageByMessageID = function (messageID, MessageParameters, addinundoredo) {
        var messageIDFound = false;
        if (messageID && messageID.trim().length > 0) {
            var messages = $scope.lstMessages.filter(function (x) {
                return x.MessageID == messageID;
            });
            if (messages && messages.length > 0) {
                $scope.SelectedRule.displayMessage = messages[0].DisplayMessage;
                $scope.getParameterByDescription($scope.SelectedRule.displayMessage, addinundoredo);

                if (messages[0].SeverityValue == 'I') {
                    $scope.SelectedRule.severityValue = "Information";
                }
                else if (messages[0].SeverityValue == 'E') {
                    $scope.SelectedRule.severityValue = "Error";
                }
                else if (messages[0].SeverityValue == 'W') {
                    $scope.SelectedRule.severityValue = "Warnings";
                }
                if (MessageParameters && $scope.SelectedRule.lstMessageParams) {
                    var lstParameters = MessageParameters.split(';');
                    for (var i = 0; i < $scope.SelectedRule.lstMessageParams.length; i++) {
                        var tempobj = {
                            Id: lstParameters[i]
                        };
                        $scope.SelectedRule.lstMessageParams[i] = tempobj;
                    }
                }
                else {
                    $rootScope.EditPropertyValue($scope.SelectedRule.dictAttributes.sfwMessageParameters, $scope.SelectedRule.dictAttributes, "sfwMessageParameters", "");
                }
                messageIDFound = true;

            }
        }

        if (!messageIDFound) {
            $scope.SelectedRule.displayMessage = "";
            $scope.SelectedRule.lstMessageParams = [];
            $scope.SelectedRule.severityValue = "";
            var ruleObj = $scope.objRules.Elements.filter(function (rObj) { if (rObj && rObj.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) { return rObj; } });
            if (ruleObj && ruleObj.length) {
                $scope.validateEmptyMessageID(ruleObj[0]);
            }
        }
        //if ($scope.SelectedRule.dictAttributes.sfwMessageId == "" && $scope.SelectedRule.dictAttributes.sfwMessage=="") {
        if (!$scope.SelectedRule.errors) {
            $scope.SelectedRule.errors = {};
        }
        //if ($scope.SelectedRule.dictAttributes.sfwMessageId == "" && $scope.SelectedRule.dictAttributes.sfwMessage == "") {    
        $ValidationService.validateIsEmptyMessageIdAndDescription($scope.SelectedRule, $scope.validationErrorList, $scope.SelectedRule.dictAttributes.sfwMessageId, $scope.SelectedRule.dictAttributes.sfwMessage);
        //}
        //}
        $scope.DeletePropertyFromObject('sfwMessageId');

    };

    /* clearing params after changing Message Description */
    $scope.populateMessageByMessageDescription = function () {
        $rootScope.EditPropertyValue($scope.SelectedRule.dictAttributes.sfwMessageParameters, $scope.SelectedRule.dictAttributes, "sfwMessageParameters", "");
        //if ($scope.SelectedRule.dictAttributes.sfwMessageId == "" && $scope.SelectedRule.dictAttributes.sfwMessage == "") {
        $ValidationService.validateIsEmptyMessageIdAndDescription($scope.SelectedRule, $scope.validationErrorList, $scope.SelectedRule.dictAttributes.sfwMessageId, $scope.SelectedRule.dictAttributes.sfwMessage);
        //}
    }

    $scope.getParameterByDescription = function (MessageDescription, addinundoredo) {

        if (MessageDescription && MessageDescription.trim().length > 0) {

            var count = GetTokens(MessageDescription);
            $scope.SelectedRule.lstMessageParams = [];
            if (addinundoredo) {
                $rootScope.UndRedoBulkOp("Start");
            }
            for (var i = 0; i < count; i++) {
                var obj = {
                    Id: ""
                };
                if (addinundoredo) {
                    $rootScope.PushItem(obj, $scope.SelectedRule.lstMessageParams);
                }
                else {
                    $scope.SelectedRule.lstMessageParams.push(obj);
                }
            }
            if (addinundoredo) {
                $rootScope.UndRedoBulkOp("End");
            }
        }
        else {
            $scope.SelectedRule.lstMessageParams = [];
        }
    };

    $scope.getParameter = function () {
        if ($scope.SelectedRule) {
            $scope.Message = $scope.SelectedRule.dictAttributes.sfwMessage;

            if ($scope.Message != undefined) {
                var count = GetTokens($scope.Message);
                $scope.SelectedRule.lstMessageParams = [];
                $rootScope.UndRedoBulkOp("Start");
                for (var i = 0; i < count; i++) {
                    var obj = {
                        Id: ""
                    };
                    $rootScope.PushItem(obj, $scope.SelectedRule.lstMessageParams);
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
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

    $scope.UpdateRuleParameter = function (id, obj) {
        if (obj) {
            obj.Id = id;
        }
        if ($scope.SelectedRule.lstMessageParams.length > 0) {
            var strTemp = "";
            angular.forEach($scope.SelectedRule.lstMessageParams, function (item) {
                if (item.Id != undefined && item.Id != "") {
                    if (strTemp == "") {
                        strTemp = item.Id;
                    }
                    else {
                        strTemp = strTemp + ";" + item.Id;
                    }

                }
            });

            $scope.SelectedRule.dictAttributes.sfwMessageParameters = strTemp;
        }
    };
    $scope.CloseRule = function (obj) {
        $rootScope.EditPropertyValue(obj.Id, obj, "Id", '');
        $scope.UpdateRuleParameter(obj.Id, obj);
    };
    $scope.OpenRuleDialog = function () {
        $scope.newRuleID = GetNewStepName("NewRule", $scope.objRules, 1);

        var newScope = $scope.$new(true);

        newScope.objRules = $scope.objRules;
        newScope.IsEdit = false;
        newScope.IsCopy = false;
        newScope.HeaderText = "Add New Id";
        newScope.newRuleID = GetNewStepName("NewRule", newScope.objRules, 1);

        var addRuleDialog = $rootScope.showDialog(newScope, "Add New ID", "Entity/views/AddRule.html", {
            width: 500, height: 180
        });


        newScope.validateNewGroup = function (type) {
            newScope.groupErrorMessageForDisplay = "";
            var NewId = "";
            if (type == "Group") {
                NewId = this.newGroupID.toLowerCase();
            }
            else if (type == "Rule") {
                NewId = this.newRuleID.toLowerCase();
            }
            if (NewId == undefined || NewId == '') {
                newScope.groupErrorMessageForDisplay = "Error: ID cannot be empty.";
                return true;
            }
            else {
                var objParentRules = null;
                if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
                    objParentRules = $scope.objInheritedRules;
                }
                var ruleModel = getRuleModel(objParentRules);
                var objDuplicate = $ValidationService.findDuplicateId(ruleModel, NewId, CONST.ENTITY.NODES, "item", null, "dictAttributes.ID");
                if (objDuplicate) {
                    var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                    if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                        errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
                    }
                    newScope.groupErrorMessageForDisplay = errorMessage;
                    return;
                }
            }
            return false;
        };

        newScope.addNewRule = function (Rule) {



            var objNewRule = {
                Name: 'rule', value: '', dictAttributes: {
                    ID: this.newRuleID, sfwRuleType: "Validation", sfwObjectBased: "False" /* added sfwObjectBased for expression builder ,if IsObjectbased is uncheck then "Business Object property method icon ll be disable" */
                }, Elements: []
            };
            if ($scope.SelectedRule) {
                var objrule = $scope.SelectedRule;
                if ($scope.SelectedRule.Name == "rule") {
                    objrule = $scope.GetParent($scope.SelectedRule, $scope.objRules);
                }
                if (objrule != undefined) {
                    $rootScope.PushItem(objNewRule, objrule.Elements, "SelectValidationRules");

                    // objrule.Elements.push(objNewRule);
                    newScope.SelectedRule = objNewRule;
                    $scope.selectedRules(newScope.SelectedRule, null);
                    newScope.IsRule = true;
                }
            }
            else {
                $rootScope.PushItem(objNewRule, $scope.objRules.Elements, "SelectValidationRules");
                newScope.SelectedRule = objNewRule;
                $scope.selectedRules(newScope.SelectedRule, null);
                newScope.IsRule = true;
            }

            newScope.closeNewRule();
            if (!newScope.IsEdit) {
                if (!$scope.objInheritedRules) {
                    $scope.scrollToPosition("#validationrule", ".page-sidebar-content", $scope.SelectedRule.dictAttributes.ID);
                }
                else {
                    $scope.scrollToPosition("#validationrule", ".inherited-rules-list", $scope.SelectedRule.dictAttributes.ID);
                }
            }
        };


        newScope.closeNewRule = function () {
            newScope.IsEdit = false;
            newScope.IsCopy = false;

            addRuleDialog.close();
        };
        newScope.validateNewGroup("Rule");
    };

    $scope.renameSubSectionRules = function (subsectionObject, NewID) {
        for (var i = 0; i < subsectionObject.Elements.length; i++) {
            if (subsectionObject.Elements[i].dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) {
                $rootScope.EditPropertyValue(subsectionObject.Elements[i].dictAttributes.ID, subsectionObject.Elements[i].dictAttributes, "ID", NewID);
            }
            if (subsectionObject.Elements[i].Elements.length > 0) {
                $scope.renameSubSectionRules(subsectionObject.Elements[i], NewID);
            }
        }
    };

    $scope.OnCheckInitialLoad = function (obj) {
        if (obj) {
            if ($scope.SelectedRule != undefined) {
                var objItem = {
                    Name: 'item', value: '', dictAttributes: { ID: $scope.SelectedRule.dictAttributes.ID, sfwMode: "All", sfwStatus: "Active" }, Elements: []
                };
                objItem.objRule = $scope.SelectedRule;
                $rootScope.PushItem(objItem, $scope.objInitialLoad.Elements);

                //$scope.objInitialLoad.Elements.push(objItem);

                if ($scope.objRule) {
                    angular.forEach($scope.objRules.Elements, function (item) {
                        if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                            $scope.SelectedRule = item;
                        }
                    });
                }

            }
        }
        else {
            angular.forEach($scope.objInitialLoad.Elements, function (item) {
                if (item.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) {
                    var index = $scope.objInitialLoad.Elements.indexOf(item);
                    if (index > -1) {
                        $rootScope.DeleteItem(item, $scope.objInitialLoad.Elements);

                        //$scope.objInitialLoad.Elements.splice(index, 1);
                    }
                }
            });
        }
    };

    $scope.OnCheckHardError = function (obj) {
        if (obj) {
            if ($scope.SelectedRule != undefined) {
                if (($scope.SelectedRule.dictAttributes.sfwMessageId == undefined || $scope.SelectedRule.dictAttributes.sfwMessageId == "") && ($scope.SelectedRule.dictAttributes.sfwMessage == undefined || $scope.SelectedRule.dictAttributes.sfwMessage == "")) {
                    alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                    $scope.SelectedRule.IsHardError = false;
                    return;
                }
                else {
                    var objItem = {
                        Name: 'item', value: '', dictAttributes: { ID: $scope.SelectedRule.dictAttributes.ID, sfwMode: "All", sfwStatus: "Active" }, Elements: [], Children: []
                    };
                    objItem.objRule = $scope.SelectedRule;
                    $rootScope.PushItem(objItem, $scope.objHardError.Elements);
                    // $scope.objHardError.Elements.push(objItem);

                    if ($scope.objRules) {
                        angular.forEach($scope.objRules.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                $scope.SelectedRule = item;
                            }
                        });
                    }
                }
            }
        }
        else {
            angular.forEach($scope.objHardError.Elements, function (item) {
                if (item.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) {
                    var index = $scope.objHardError.Elements.indexOf(item);
                    if (index > -1) {
                        if (item.Children && item.Children.length > 0) {
                            function iteratorHardError(itm) {
                                if ($scope.CheckHardErrorAsChild(itm)) {
                                    $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "HardErrorAsChild");
                                }
                            }
                            angular.forEach(item.Children, iteratorHardError);
                        }
                        $rootScope.DeleteItem(item, $scope.objHardError.Elements);
                        //$scope.objHardError.Elements.splice(index, 1);
                    }
                }
            });
        }
    };

    $scope.OnCheckSoftError = function (obj) {
        if (obj) {
            if ($scope.SelectedRule != undefined) {
                if (($scope.SelectedRule.dictAttributes.sfwMessageId == undefined || $scope.SelectedRule.dictAttributes.sfwMessageId == "") && ($scope.SelectedRule.dictAttributes.sfwMessage == undefined || $scope.SelectedRule.dictAttributes.sfwMessage == "")) {
                    alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                    $scope.SelectedRule.IsSoftError = false;
                    return;
                }
                else {
                    var objItem = {
                        Name: 'item', value: '', dictAttributes: { ID: $scope.SelectedRule.dictAttributes.ID, sfwMode: "All", sfwStatus: "Active" }, Elements: [], Children: []
                    };
                    objItem.objRule = $scope.SelectedRule;
                    $rootScope.PushItem(objItem, $scope.objSoftError.Elements);
                    //$scope.objSoftError.Elements.push(objItem);

                    if ($scope.objRules) {
                        angular.forEach($scope.objRules.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                $scope.SelectedRule = item;
                            }
                        });
                    }
                }
            }
        }
        else {
            angular.forEach($scope.objSoftError.Elements, function (item) {
                if (item.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) {
                    var index = $scope.objSoftError.Elements.indexOf(item);
                    if (index > -1) {
                        if (item.Children && item.Children.length > 0) {
                            function iteratorSoftError(itm) {
                                if ($scope.CheckSoftErrorAsChild(itm)) {
                                    $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "SoftErrorAsChild");
                                }
                            }
                            angular.forEach(item.Children, iteratorSoftError);
                        }
                        $rootScope.DeleteItem(item, $scope.objSoftError.Elements);
                        // $scope.objSoftError.Elements.splice(index, 1);

                    }
                }
            });
        }
    };

    $scope.OnCheckValidateDelete = function (obj) {
        if (obj) {
            if ($scope.SelectedRule != undefined) {
                if ($scope.SelectedRule.dictAttributes.sfwMessageId == undefined && $scope.SelectedRule.dictAttributes.sfwMessage == undefined) {
                    alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                    $scope.SelectedRule.IsValidateDelete = false;
                    return;
                }
                else {
                    var objItem = {
                        Name: 'item', value: '', dictAttributes: { ID: $scope.SelectedRule.dictAttributes.ID, sfwMode: "All", sfwStatus: "Active" }, Elements: []
                    };
                    objItem.objRule = $scope.SelectedRule;
                    $rootScope.PushItem(objItem, $scope.objValidateDelete.Elements);
                    // $scope.objValidateDelete.Elements.push(objItem);

                    if ($scope.objRules) {
                        angular.forEach($scope.objRules.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                $scope.SelectedRule = item;
                            }
                        });
                    }
                }
            }
        }
        else {
            angular.forEach($scope.objValidateDelete.Elements, function (item) {
                if (item.dictAttributes.ID == $scope.SelectedRule.dictAttributes.ID) {
                    var index = $scope.objValidateDelete.Elements.indexOf(item);
                    if (index > -1) {
                        $rootScope.DeleteItem(item, $scope.objValidateDelete.Elements);
                        // $scope.objValidateDelete.Elements.splice(index, 1);
                    }
                }
            });
        }
    };

    $scope.SetMessage = function () {
        if ($scope.objMessage != undefined) {
            $scope.SelectedRule.dictAttributes.sfwMessageId = $scope.objMessage.MessageID;
            $scope.SelectedRule.displayMessage = $scope.objMessage.DisplayMessage;
            if ($scope.objMessage.SeverityValue == 'I') {
                $scope.SelectedRule.severityValue = "Information";
            }
            else if ($scope.objMessage.SeverityValue == 'E') {
                $scope.SelectedRule.severityValue = "Error";
            }
            else if ($scope.objMessage.SeverityValue == 'W') {
                $scope.SelectedRule.severityValue = "Warnings";
            }

            ngDialog.close(SearchMessage);
        }

    };

    $scope.OnSelectMessage = function (msg) {
        $scope.objMessage = msg;
    };

    $scope.CloseMessageDialog = function () {
        ngDialog.close(SearchMessage);
    };

    $scope.CheckSoftErrorAsChild = function (obj) {
        var IsSoftErrorAsChild = false;

        angular.forEach($scope.objSoftError.Elements, function (item) {

            angular.forEach(item.Children, function (rule) {
                if (rule.dictAttributes.ID == obj.dictAttributes.ID) {
                    IsSoftErrorAsChild = true;
                }
            });
        });
        return IsSoftErrorAsChild;
    };

    $scope.CheckHardErrorAsChild = function (obj) {
        var IsHardErrorAsChild = false;

        angular.forEach($scope.objHardError.Elements, function (item) {

            angular.forEach(item.Children, function (rule) {
                if (rule.dictAttributes.ID == obj.dictAttributes.ID) {
                    IsHardErrorAsChild = true;
                }
            });
        });
        return IsHardErrorAsChild;
    };

    $scope.ErrorMenuOptionsForHardError = [
        ['Add Child Rule', function ($itemScope) {
            $scope.SelectedErrorRule = $itemScope.element;
            if ($scope.SelectedErrorRule != undefined) {
                $scope.addRuleAsChild($scope.SelectedErrorRule, "HardError");
            }
        }, function ($itemScope) {
            if ($itemScope.element != undefined && $itemScope.element.Name == "item") {
                $scope.SelectedErrorClick($itemScope.element, null, "HardError");
                return true;
            }
        }],
        ['Delete', function ($itemScope) {
            $scope.SelectedErrorRule = $itemScope.element;
            if ($scope.SelectedErrorRule != undefined) {
                $scope.DeleteRuleFromContextMenu($itemScope.element, 'HardError');
            }
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                $scope.SelectedErrorClick($itemScope.element, null, "HardError");
                return true;
            }
        }

        ],
        null
    ];

    $scope.ErrorMenuOptionsForSoftError = [
        ['Add Child Rule', function ($itemScope) {
            $scope.SelectedErrorRule = $itemScope.element;
            if ($scope.SelectedErrorRule != undefined) {
                $scope.addRuleAsChild($scope.SelectedErrorRule, "SoftError");
            }
        }, function ($itemScope) {
            if ($itemScope.element != undefined && $itemScope.element.Name == "item") {
                $scope.SelectedErrorClick($itemScope.element, null, "SoftError");
                return true;
            }
        }],
        ['Delete', function ($itemScope) {
            $scope.SelectedErrorRule = $itemScope.element;
            if ($scope.SelectedErrorRule != undefined) {
                $scope.DeleteRuleFromContextMenu($itemScope.element, 'SoftError');
            }
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                $scope.SelectedErrorClick($itemScope.element, null, "SoftError");
                return true;
            }
        }],

        null
    ];

    $scope.addRuleAsChild = function (aobjCurrentElement, astrRuleType) {
        var newScope = $scope.$new();
        newScope.lstValidationRule = [];
        if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
            validateValidationRulesID($scope.objInheritedRules, true);
        }
        if ($scope.objRules && $scope.objRules.Elements) {
            newScope.lstValidationRule = angular.copy($scope.objRules.Elements);
        }
        if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
            newScope.lstValidationRule = newScope.lstValidationRule.concat(angular.copy($scope.objInheritedRules.Elements));
        }
        var addChildRuleDialog = $rootScope.showDialog(newScope, "Add Child Rule", "Entity/views/AddChildRule.html");
        newScope.onCancelClick = function () {
            addChildRuleDialog.close();
        };
        newScope.onAddChildRuleClick = function () {
            var lstSelectedRule = newScope.lstValidationRule.filter(function (aObjRule) { return aObjRule.IsSelected == true; });
            if (lstSelectedRule.length > 0) {
                var strInValidRule = null;
                var strSameRuleExist = null;
                var strSameChildRuleExist = null;
                angular.forEach(lstSelectedRule, function (ruleItem) {
                    if ((ruleItem.dictAttributes.sfwMessageId == undefined || ruleItem.dictAttributes.sfwMessageId == "") && (ruleItem.dictAttributes.sfwMessage == undefined || ruleItem.dictAttributes.sfwMessage == "")) {
                        if (strInValidRule) {
                            strInValidRule += "," + ruleItem.dictAttributes.ID;
                        }
                        else {
                            strInValidRule = ruleItem.dictAttributes.ID;
                        }
                    }
                    if (aobjCurrentElement.dictAttributes.ID == ruleItem.dictAttributes.ID) {
                        if (strSameRuleExist) {
                            strSameRuleExist += "," + ruleItem.dictAttributes.ID;
                        }
                        else {
                            strSameRuleExist = ruleItem.dictAttributes.ID;
                        }
                    }
                    if (aobjCurrentElement.dictAttributes.ID != ruleItem.dictAttributes.ID && aobjCurrentElement.Children) {
                        angular.forEach(aobjCurrentElement.Children, function (item) {
                            if (item.dictAttributes.ID == ruleItem.dictAttributes.ID) {
                                if (strSameChildRuleExist) {
                                    strSameChildRuleExist += "," + ruleItem.dictAttributes.ID;
                                }
                                else {
                                    strSameChildRuleExist = ruleItem.dictAttributes.ID;
                                }
                            }
                        });
                    }
                });
                if (strInValidRule && strInValidRule.contains(",")) {
                    alert('Cannot add "' + strInValidRule + '" rules to validation. Rules must contain MessageID/Description.');
                    return;
                }
                else if (strInValidRule) {
                    alert('Cannot add "' + strInValidRule + '" rule to validation. Rule must contain MessageID/Description.');
                    return;
                }
                else if (strSameRuleExist) {
                    alert('Cannot add "' + strSameRuleExist + '" rule to validation. Same rule already exists.');
                    return;
                }
                else if (strSameChildRuleExist) {
                    alert('Cannot add "' + strSameChildRuleExist + '" rule to validation. Same Child rule already exists.');
                    return;
                }
                angular.forEach(lstSelectedRule, function (ruleItem) {
                    var objRule = $scope.GetRuleByName(ruleItem.dictAttributes.ID);
                    var objItem = {
                        Name: 'rule', value: '', dictAttributes: { ID: ruleItem.dictAttributes.ID, sfwMode: "All", sfwStatus: 'Active' }, Elements: []
                    };
                    objItem.objRule = objRule;
                    $rootScope.UndRedoBulkOp("Start");
                    if (astrRuleType == "SoftError") {
                        $rootScope.PushItem(objItem, aobjCurrentElement.Children, "SelectSoftError");
                        $scope.SelectSoftError(objItem);
                        $scope.setValidationRuleCheckboxValue(ruleItem.dictAttributes.ID, true, "SoftErrorAsChild");
                    }
                    else if (astrRuleType == "HardError") {
                        $rootScope.PushItem(objItem, aobjCurrentElement.Children, "SelectHardError");
                        $scope.SelectHardError(objItem);
                        $scope.setValidationRuleCheckboxValue(ruleItem.dictAttributes.ID, true, "HardErrorAsChild");
                    }
                    $rootScope.EditPropertyValue(objItem.ParentID, objItem, "ParentID", aobjCurrentElement.dictAttributes.ID);

                    $rootScope.UndRedoBulkOp("End");
                });
                newScope.onCancelClick();
            }
        };
    };
    $scope.setValidationRuleCheckboxValue = function (ruleID, value, type) {
        var desc;
        if ($scope.selectedIneritedTab == "Rule") {
            desc = getDescendents($scope.objRules);
        }
        else {
            desc = getDescendents($scope.objInheritedRules);
        }
        var rules = desc.filter(function (x) {
            return x.Name == "rule" && x.dictAttributes.ID == ruleID;
        });
        if (rules && rules.length > 0) {
            if (type == "InitialLoad") {
                $rootScope.EditPropertyValue(rules[0].IsInitialLoad, rules[0], "IsInitialLoad", value);
            }
            else if (type == "ValidateDelete") {
                $rootScope.EditPropertyValue(rules[0].IsValidateDelete, rules[0], "IsValidateDelete", value);
            }
            else if (type == "HardError") {
                $rootScope.EditPropertyValue(rules[0].IsHardError, rules[0], "IsHardError", value);
            }
            else if (type == "HardErrorAsChild") {
                $rootScope.EditPropertyValue(rules[0].IsHardErrorAsChild, rules[0], "IsHardErrorAsChild", value);
            }
            else if (type == "SoftError") {
                $rootScope.EditPropertyValue(rules[0].IsSoftError, rules[0], "IsSoftError", value);
            }
            else if (type == "SoftErrorAsChild") {
                $rootScope.EditPropertyValue(rules[0].IsSoftErrorAsChild, rules[0], "IsSoftErrorAsChild", value);
            }
            else if (type == "CheckList") {
                $rootScope.EditPropertyValue(rules[0].IsCheckList, rules[0], "IsCheckList", value);
            }
            else if (type == "Group") {
                $rootScope.EditPropertyValue(rules[0].IsGroup, rules[0], "IsGroup", value);
            }
        }
    };
    $scope.selectIneritedTab = function (tabName) {
        $scope.selectedIneritedTab = tabName;
    };
    $scope.AddGroupFromContextMenu = function ($itemScope, $index, isrename) {
        var newScope = $scope.$new(true);
        var addGroupRuleDialog = "";

        if (isrename) {
            if ($itemScope.element != undefined) {
                $scope.selectedGroupRule = $itemScope.element;
                newScope.selectedGroupRule = $itemScope.element;
                newScope.selectedGroupIndex = $itemScope.$index;

                if (newScope.selectedGroupRule != undefined) {

                    newScope.newGroupID = newScope.selectedGroupRule.dictAttributes.ID;
                    newScope.objRules = $scope.objRules;
                    newScope.objGroupList = $scope.objGroupList;
                    newScope.IsGroupRename = true;

                    addGroupRuleDialog = $rootScope.showDialog(newScope, "Edit ID", "Entity/views/AddGroupRule.html", {
                        width: 500, height: 180
                    });
                }
            }
        }
        else {
            newScope.IsNewGroup = true;
            newScope.SelectedRule = $scope.SelectedRule;
            newScope.objGroupList = $scope.objGroupList;
            newScope.newGroupID = GetNewStepName("NewGroup", newScope.objGroupList, 1);

            addGroupRuleDialog = $rootScope.showDialog(newScope, "Add New ID", "Entity/views/AddGroupRule.html", {
                width: 500, height: 180
            });
        }

        newScope.addGroup = function () {
            if (newScope.IsGroupRename) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(newScope.selectedGroupRule.dictAttributes.ID, newScope.selectedGroupRule.dictAttributes, "ID", this.newGroupID);

                // while rename the group name , updating parent Id for UP & Down features
                if (newScope.selectedGroupRule.Elements.length > 0) {
                    angular.forEach(newScope.selectedGroupRule.Elements, function (item) {
                        $rootScope.EditPropertyValue(item.ParentID, item, "ParentID", newScope.selectedGroupRule.dictAttributes.ID);
                    });
                }

                $rootScope.UndRedoBulkOp("End");
            }
            else if (newScope.IsNewGroup) {
                var objNewGroup = {
                    Name: 'group', value: '', dictAttributes: {
                        ID: this.newGroupID
                    }, Elements: []
                };
                $rootScope.PushItem(objNewGroup, newScope.objGroupList.Elements, "SelectGroupClick");
                $scope.SelectGroupClick(objNewGroup);
            }

            validateValidationRulesID(true);
            newScope.closeGroup();
            newScope.IsNewGroup = false;
        };


        newScope.validateNewGroup = function (type) {

            newScope.groupErrorMessageForDisplay = "";
            var NewId = "";
            if (type == "Group") {
                NewId = this.newGroupID.toLowerCase();
            }
            if (NewId == undefined || NewId == '') {
                newScope.groupErrorMessageForDisplay = "Error: ID cannot be empty.";
                return true;
            }
            else {
                for (var i = 0; i < newScope.objGroupList.Elements.length; i++) {
                    if (isrename) {
                        if (newScope.objGroupList.Elements[i] !== newScope.selectedGroupRule && newScope.objGroupList.Elements[i].dictAttributes.ID.toLowerCase() == NewId) {
                            newScope.groupErrorMessageForDisplay = "Error:Duplicate ID present. Please enter another ID.";
                            return true;
                        }
                    }

                    else if (newScope.objGroupList.Elements[i].dictAttributes.ID.toLowerCase() == NewId) {
                        newScope.groupErrorMessageForDisplay = "Error:Duplicate ID present. Please enter another ID.";
                        return true;
                    }
                }
            }

            return false;
        };


        newScope.closeGroup = function () {
            addGroupRuleDialog.close();

            newScope.IsGroupRename = false;
            newScope.IsNewGroup = false;
        };


    };
    $scope.DeleteGroupFromContextMenu = function ($itemScope) {
        if ($itemScope.element != undefined) {
            $scope.selectedGroupRule = $itemScope.element;
            if ($itemScope.element.Name == "group") {
                var result = confirm("Do you want to remove the selected Group '" + $itemScope.element.dictAttributes.ID + "'from this section ?");
                if (result == true) {
                    $rootScope.UndRedoBulkOp("Start");
                    if ($scope.selectedGroupRule.Elements.length > 0) {

                        angular.forEach($scope.selectedGroupRule.Elements, function (itm) {
                            $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "Group");

                        });
                    }

                    var index = $scope.objGroupList.Elements.indexOf($itemScope.element);
                    $rootScope.DeleteItem($itemScope.element, $scope.objGroupList.Elements, "SelectGroupClick");
                    $rootScope.UndRedoBulkOp("End");
                    //$scope.objGroupList.Elements.splice(index, 1);
                }
            }
        }
    };
    $scope.DeleteRuleFromContextMenu = function (aobjSelectedRule, astrSelectedRuleSection) {
        if (aobjSelectedRule != undefined) {

            var result = confirm("Do you want to remove the selected rule '" + aobjSelectedRule.dictAttributes.ID + "'from this section ?");
            if (result == true) {
                $rootScope.UndRedoBulkOp("Start");

                if (aobjSelectedRule.Name == "item") {
                    var objParent;
                    var functiontocall = "SelectGroupClick";
                    if (astrSelectedRuleSection == 'Group') {
                        objParent = $scope.GetParentForGroups(aobjSelectedRule, $scope.objGroupList);
                        //$scope.GetParent(aobjSelectedRule, $scope.objGroupList);
                    }
                    else if (astrSelectedRuleSection == 'Checklist') {
                        objParent = $scope.GetParent(aobjSelectedRule, $scope.objCheckList);
                        functiontocall = "SelectCheckListClick";
                    }
                    else if (astrSelectedRuleSection == 'InitialLoad') {
                        objParent = $scope.GetParent(aobjSelectedRule, $scope.objInitialLoad);
                        functiontocall = "SelectedInitialLoadClick";
                    }
                    else if (astrSelectedRuleSection == 'ValidateDelete') {
                        objParent = $scope.GetParent(aobjSelectedRule, $scope.objValidateDelete);
                        functiontocall = "SelectedValidateDeleteClick";
                    }
                    else if (astrSelectedRuleSection == 'HardError') {
                        objParent = $scope.GetParent(aobjSelectedRule, $scope.objHardError);
                        functiontocall = "SelectHardError";
                    }
                    else if (astrSelectedRuleSection == 'SoftError') {
                        objParent = $scope.GetParent(aobjSelectedRule, $scope.objSoftError);
                        functiontocall = "SelectSoftError";
                    }
                    if (objParent != undefined) {
                        var index = objParent.Elements.indexOf(aobjSelectedRule);
                        $rootScope.DeleteItem(aobjSelectedRule, objParent.Elements, functiontocall);
                        $ValidationService.removeObjInToArray($scope.validationErrorList, aobjSelectedRule);
                        validateValidationRulesID(true);
                        // objParent.Elements.splice(index, 1);

                        if (astrSelectedRuleSection == 'Group') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "Group");
                            if (index < objParent.Elements.length) {
                                $scope.SelectGroupClick(objParent.Elements[index], null);
                            }
                            else if (objParent.Elements.length > 0) {
                                $scope.SelectGroupClick(objParent.Elements[index - 1], null);
                            }
                            else {
                                $scope.SelectGroupClick(undefined, null);
                            }
                        }
                        else if (astrSelectedRuleSection == 'Checklist') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "CheckList");
                            if (index < objParent.Elements.length) {
                                $scope.SelectCheckListClick(objParent.Elements[index], null);
                            }
                            else if (objParent.Elements.length > 0) {
                                $scope.SelectCheckListClick(objParent.Elements[index - 1], null);
                            }
                            else {
                                $scope.SelectCheckListClick(undefined, null);
                            }
                        }

                        else if (astrSelectedRuleSection == 'InitialLoad') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "InitialLoad");
                            if (index < objParent.Elements.length) {

                                $scope.SelectedInitialLoadClick(objParent.Elements[index], null);
                            }
                            else if (objParent.Elements.length > 0) {

                                $scope.SelectedInitialLoadClick(objParent.Elements[index - 1], null);
                            }
                            else {
                                $scope.SelectedInitialLoadClick(undefined, null);
                            }
                        }

                        else if (astrSelectedRuleSection == 'ValidateDelete') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "ValidateDelete");
                            if (index < objParent.Elements.length) {
                                $scope.SelectedValidateDeleteClick(objParent.Elements[index]);
                            }
                            else if (objParent.Elements.length > 0) {
                                $scope.SelectedValidateDeleteClick(objParent.Elements[index - 1]);
                            }
                            else {
                                $scope.SelectedValidateDeleteClick(undefined, null);
                            }
                        }
                        else if (astrSelectedRuleSection == 'HardError') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "HardError");
                            if (index < objParent.Elements.length) {
                                $scope.SelectHardError(objParent.Elements[index]);
                            }
                            else if (objParent.Elements.length > 0) {
                                $scope.SelectHardError(objParent.Elements[index - 1]);
                            }
                            else {
                                $scope.SelectHardError(undefined, null);
                            }
                            if (aobjSelectedRule.Children.length > 0) {

                                angular.forEach(aobjSelectedRule.Children, function (itm) {
                                    if (!$scope.CheckHardErrorAsChild(itm)) {
                                        $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "HardErrorAsChild");
                                    }
                                });
                            }

                        }
                        else if (astrSelectedRuleSection == 'SoftError') {
                            $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "SoftError");
                            if (index < objParent.Elements.length) {
                                $scope.SelectSoftError(objParent.Elements[index]);
                            }
                            else if (objParent.Elements.length > 0) {
                                $scope.SelectSoftError(objParent.Elements[index - 1]);
                            }
                            else {
                                $scope.SelectSoftError(undefined, null);
                            }
                            if (aobjSelectedRule.Children.length > 0) {

                                angular.forEach(aobjSelectedRule.Children, function (itm) {
                                    if (!$scope.CheckSoftErrorAsChild(itm)) {
                                        $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "SoftErrorAsChild");
                                    }
                                });
                            }
                        }
                    }
                }
                else if (aobjSelectedRule.Name == 'rule') {
                    if (astrSelectedRuleSection == 'HardError') {
                        var objParent = $scope.GetParentForErrors(aobjSelectedRule, $scope.objHardError);
                        if (objParent != undefined) {
                            var index = objParent.Children.indexOf(aobjSelectedRule);

                            if (index > -1) {
                                $rootScope.DeleteItem(aobjSelectedRule, objParent.Children, "SelectHardError");


                                if (index < objParent.Children.length) {
                                    $scope.SelectHardError(objParent.Children[index]);
                                }
                                else if (objParent.Children.length > 0) {
                                    $scope.SelectHardError(objParent.Children[index - 1]);
                                }
                                else {
                                    $scope.SelectHardError(objParent);
                                }
                                if (!$scope.CheckHardErrorAsChild(aobjSelectedRule)) {
                                    $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "HardErrorAsChild");
                                }
                            }
                        }
                    }

                    else if (astrSelectedRuleSection == 'SoftError') {
                        var objParent = $scope.GetParentForErrors(aobjSelectedRule, $scope.objSoftError);
                        if (objParent != undefined) {
                            var index = objParent.Children.indexOf(aobjSelectedRule);

                            if (index > -1) {
                                $rootScope.DeleteItem(aobjSelectedRule, objParent.Children, "SelectSoftError");


                                if (index < objParent.Children.length) {
                                    $scope.SelectSoftError(objParent.Children[index]);
                                }
                                else if (objParent.Children.length > 0) {
                                    $scope.SelectSoftError(objParent.Children[index - 1]);
                                }
                                else {
                                    $scope.SelectSoftError(objParent);
                                }
                                if (!$scope.CheckSoftErrorAsChild(aobjSelectedRule)) {
                                    $scope.setValidationRuleCheckboxValue(aobjSelectedRule.dictAttributes.ID, false, "SoftErrorAsChild");
                                }
                            }
                        }
                    }
                }
                else if (aobjSelectedRule.Name == "group") {

                    if (aobjSelectedRule.Elements.length > 0) {

                        angular.forEach(aobjSelectedRule.Elements, function (itm) {
                            $scope.setValidationRuleCheckboxValue(itm.dictAttributes.ID, false, "Group");

                        });
                    }

                    var index = $scope.objGroupList.Elements.indexOf(aobjSelectedRule);
                    $rootScope.DeleteItem(aobjSelectedRule, $scope.objGroupList.Elements, "SelectGroupClick");
                    if (index < $scope.objGroupList.Elements.length) {
                        $scope.setSelectedGroupFromDashBoard($scope.objGroupList.Elements[index]);
                    }
                    else if ($scope.objGroupList.Elements.length > 0) {
                        $scope.setSelectedGroupFromDashBoard($scope.objGroupList.Elements[index - 1]);
                    }
                    else {
                        $scope.setSelectedGroupFromDashBoard(undefined);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    $scope.GroupMenuOptionsForInitialLoad = [
        ['Delete Rule', function ($itemScope, param, value) {
            $scope.DeleteRuleFromContextMenu($itemScope.element, 'InitialLoad');
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == 'item') {
                    $scope.SelectedInitialLoadClick($itemScope.element);
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        null
    ];
    $scope.GroupMenuOptionsForValidateDelete = [
        ['Delete Rule', function ($itemScope, param, value) {
            $scope.DeleteRuleFromContextMenu($itemScope.element, 'ValidateDelete');
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == 'item') {
                    $scope.SelectedValidateDeleteClick($itemScope.element);
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        null
    ];
    $scope.GroupMenuOptionsForGroups = [
        ['Add Group', function ($itemScope) {
            $scope.AddGroupFromContextMenu($itemScope, -1, false);
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == "group") {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return true;
            }
        }],
        ['Delete Group', function ($itemScope) {
            $scope.DeleteRuleFromContextMenu($itemScope.element);

        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == "group") {
                    $scope.SelectGroupClick($itemScope.element);
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        ['Rename Group', function ($itemScope, $index) {
            $scope.AddGroupFromContextMenu($itemScope, $index, true);
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == "group") {
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        ['Delete Rule', function ($itemScope, param, value) {
            $scope.DeleteRuleFromContextMenu($itemScope.element, 'Group');
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == 'item') {
                    $scope.SelectGroupClick($itemScope.element);
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        null

    ];
    $scope.GroupMenuOptionsForCheckList = [
        ['Delete Rule', function ($itemScope, param, value) {
            $scope.DeleteRuleFromContextMenu($itemScope.element, 'Checklist');
        }, function ($itemScope) {
            if ($itemScope.element != undefined) {
                if ($itemScope.element.Name == 'item') {
                    $scope.SelectCheckListClick($itemScope.element);
                    return true;
                }
                else {
                    return false;
                }
            }
        }],
        null
    ];
    $scope.GetParent = function (objGroupRule, objParentGroup) {
        var parent;
        if (objParentGroup) {
            angular.forEach(objParentGroup.Elements, function (item) {
                if (parent == undefined) {
                    $scope.result = angular.equals(item, objGroupRule);
                    if ($scope.result) {
                        parent = objParentGroup;
                        return;
                    }
                    else {
                        parent = $scope.GetParent(objGroupRule, item);
                        if (parent != undefined) {
                            return;
                        }
                    }
                }
            });
        }
        return parent;
    };

    $scope.GetParentForErrors = function (objGroupRule, objParentGroup) {
        var parent;
        if (objParentGroup) {
            angular.forEach(objParentGroup.Elements, function (item) {
                if (parent == undefined) {
                    $scope.result = angular.equals(item, objGroupRule);
                    if ($scope.result) {
                        parent = objParentGroup;
                        return;
                    }
                    else {
                        parent = $scope.GetParentForErrors(objGroupRule, item);
                        if (parent != undefined) {
                            return;
                        }
                    }
                }
            });

            if (parent == undefined) {
                parent = $scope.GetParentFromChildenForErrors(objGroupRule, objParentGroup);
            }
        }
        return parent;
    };

    $scope.GetParentFromChildenForErrors = function (objGroupRule, objParentGroup) {
        var parent;
        angular.forEach(objParentGroup.Children, function (item) {
            if (parent == undefined) {
                $scope.result = angular.equals(item, objGroupRule);
                if ($scope.result) {
                    parent = objParentGroup;
                    return;
                }
                else {
                    parent = $scope.GetParentFromChildenForErrors(objGroupRule, item);
                    if (parent != undefined) {
                        return;
                    }
                }
            }
        });

        return parent;
    };

    $scope.GetParentForGroups = function (objGroupRule, objParentGroup) {
        var parent;

        if (objGroupRule.ParentID) {
            var lst = objParentGroup.Elements.filter(function (itm) { return itm.dictAttributes.ID == objGroupRule.ParentID; });
            if (lst && lst.length > 0) {
                parent = lst[0];
            }
        }
        return parent;
    };


    $scope.setSelectedError = function (selectedError, index) {
        var a = selectedError;
        for (var i = 0; i < selectedError.Elements.length; i++) {
            if (selectedError.Elements[i].Name == "item") {
                if (i == index) {
                    selectedError.Elements[i].isSelected = true;
                }
                else {
                    selectedError.Elements[i].isSelected = false;
                }
            }
        }
    };

    $scope.canMoveErrorUp = function (selectedError) {
        if (selectedError) {
            var selectedItems = selectedError.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if ((selectedItems && selectedItems.length > 0 && $scope.SelectedSoftError) && (($scope.SelectedRuleView == 'SoftError' ? $scope.SelectedSoftError.ParentID == "softerror" : $scope.SelectedHardError.ParentID == "harderror"))) {
                return selectedError.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) > 0;
            }
        }
        return false;
    };
    $scope.moveErrorUp = function (selectedError) {
        var a = selectedError;
        if (selectedError) {
            var selectedItems = selectedError.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                var index = selectedError.Elements.indexOf(selectedItems[0]);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(selectedItems[0], selectedError.Elements);
                while (selectedError.Elements[index - 1].Name == "harderror" || selectedError.Elements[index - 1].Name == "softerror")
                    index--;
                $rootScope.InsertItem(selectedItems[0], selectedError.Elements, index - 1);
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    $scope.canMoveErrorDown = function (selectedError) {
        if (selectedError) {
            var selectedItems = selectedError.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if ((selectedItems && selectedItems.length > 0 && $scope.SelectedSoftError) && (($scope.SelectedRuleView == 'SoftError' ? $scope.SelectedSoftError.ParentID == "softerror" : $scope.SelectedHardError.ParentID == "harderror"))) {
                return selectedError.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) < selectedError.Elements.filter(function (x) { return x.Name == "item"; }).length - 1;
            }
        }
        return false;
    };
    $scope.moveErrorDown = function (selectedError) {
        var a = selectedError;

        if (selectedError) {
            var selectedItems = selectedError.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                var index = selectedError.Elements.indexOf(selectedItems[0]);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(selectedItems[0], selectedError.Elements);
                while (selectedError.Elements[index].Name == "harderror" || selectedError.Elements[index].Name == "softerror")
                    index++;
                $rootScope.InsertItem(selectedItems[0], selectedError.Elements, index + 1);
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    //#endregion

    //#region Query Section
    $scope.onQueryTypeChange = function (queryType) {
        var index = $scope.queryDataTypes.indexOf("EntityTable");
        if (index > 0) {
            $scope.queryDataTypes.splice(index, 1);
        }

        if (queryType != undefined && queryType != '') {

            if (queryType == "SelectQuery") {
                //show the custom mapping tab
                //$('[href="#custommap"]').closest('li').show();
                $scope.isCustomMappingTab = true;
                $scope.queryDataTypes.push("EntityTable");
                if ($scope.selectedQuery && $scope.selectedQuery.dictAttributes) {
                    $scope.selectedQuery.dictAttributes.sfwDataType = 'EntityTable';
                }
            }
            else {
                //hide custom mapping tab
                //$('[href="#custommap"]').closest('li').hide();
                //if (queryType == "ScalarQuery") {
                //    $('#ddlSqlQueryDataTypes').val($scope.queryDataTypes[0]);
                //    var index = $scope.queryDataTypes.indexOf('EntityTable');
                //    if (index > -1)
                //        $scope.queryDataTypes.splice(index, 1);
                //}
                $scope.isCustomMappingTab = false;
            }

            //on selecting ScalarQuery, default empty value will  be shown.
            if (queryType == "NonQuery") {
                $scope.selectedQuery.dictAttributes.sfwDataType = 'int';
            }

            if (queryType === "SubSelectQuery") {
                $scope.selectedQuery.dictAttributes.sfwDataType = '';
            }
        }
        //$scope.getQueryParameters();

    };

    $scope.executeQuery = function () {
        if ($scope.selectedQuery && $scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute]) {
            var objParameters = $scope.selectedQuery.Elements.filter(function (itm) {
                return itm.Name === "parameters";
            })[0];
            if (objParameters && objParameters.Elements && objParameters.Elements.length) {
                dialogScope = $scope.$new();
                dialogScope.ExecutionParameters = [];

                for (var i = 0; i < objParameters.Elements.length; i++) {
                    var parameter = objParameters.Elements[i].dictAttributes.ID;
                    var type = objParameters.Elements[i].dictAttributes.sfwDataType;
                    var value = "";
                    var objparameters = {
                        Parameter: parameter, Type: type, Value: value
                    };
                    dialogScope.ExecutionParameters.push(objparameters);
                }

                dialogScope.canExecuteQuery = function () {
                    return !dialogScope.ExecutionParameters.some(function (item) {
                        return !(item.Value && item.Value.trim().length > 0);
                    });
                };
                dialogScope.isExecuted = false;
                dialogScope.isShowingTopRecords = !$scope.hasQueryTopClause($scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute]);
                dialogScope.onOKClick = function () {
                    $rootScope.IsLoading = true;
                    var strQuery = undefined;
                    if ($scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute]) {
                        strQuery = $scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute];
                    }

                    if (objParameters && strQuery) {
                        if (objParameters.Elements.length > 0) {
                            for (var i = 0; i < dialogScope.ExecutionParameters.length; i++) {
                                var search = dialogScope.ExecutionParameters[i].Parameter;
                                if (dialogScope.ExecutionParameters[i].Type && dialogScope.ExecutionParameters[i].Type.toLowerCase() == "int" || dialogScope.ExecutionParameters[i].Type.toLowerCase() == "decimal")
                                    strQuery = strQuery.replace(new RegExp(search, 'gi'), dialogScope.ExecutionParameters[i].Value);
                                else if (dialogScope.ExecutionParameters[i].Type && dialogScope.ExecutionParameters[i].Type.toLowerCase() == "string")
                                    strQuery = strQuery.replace(new RegExp(search, 'gi'), "'" + dialogScope.ExecutionParameters[i].Value + "'");
                                else
                                    strQuery = strQuery.replace(new RegExp(search, 'gi'), dialogScope.ExecutionParameters[i].Value);
                            }
                        }
                    }

                    $scope.executeQueryByLimit(strQuery, 50);

                    dialogScope.closeDialog();

                };
                dialogScope.onCancelClick = function () {
                    dialogScope.closeDialog();
                };
                dialogScope.closeDialog = function () {
                    if (dialogScope.dialog && dialogScope.dialog.close) {
                        dialogScope.dialog.close();
                    }
                };
                $scope.showExecutionQueryDialog(dialogScope);
            }
            else {
                $scope.executeQueryByLimit($scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute], 50);
            }
        }
        else {
            alert('Query is missing in current tab.');
        }
    };
    $scope.executeQueryByLimit = function (astrQuery, aintLimit) {
        if (aintLimit) {
            aintLimit = 50;
        }
        var lstrQueryWrapper = "";
        astrQuery = astrQuery.replace(/\n/g, " ").replace(/\t/g, " ").toLowerCase().trim();
        if (astrQuery.indexOf("select") > -1) {
            if ($rootScope.IsOracleDB) {
                if (!$scope.hasQueryTopClause(astrQuery)) {
                    lstrQueryWrapper = astrQuery + " fetch first {0} rows only";
                }
            }
            else {
                if (!$scope.hasQueryTopClause(astrQuery)) {
                    lstrQueryWrapper = astrQuery.substring(0, 7) + " top {0} " + astrQuery.substring(7);
                }
            }
            lstrQueryWrapper = String.format(lstrQueryWrapper, aintLimit);
            if (!(lstrQueryWrapper && lstrQueryWrapper.trim().length > 0)) {
                lstrQueryWrapper = astrQuery;
            }
            $.connection.hubEntityModel.server.executeQuery(lstrQueryWrapper).done(function (data) {
                $scope.receieveExecuteAllQueries(data);
            });
        }
        else {
            toastr.error("Not a select query");
        }
    };
    $scope.hasQueryTopClause = function (astrQuery) {
        var lblnHasTopClause = false;
        astrQuery = astrQuery.replace(/\n/g, " ").replace(/\t/g, " ").toLowerCase().trim();
        if ($rootScope.IsOracleDB) {
            lblnHasTopClause = astrQuery.indexOf(" fetch ") > -1;
        }
        else {
            lblnHasTopClause = astrQuery.indexOf(" top ") > -1;
        }
        return lblnHasTopClause;
    };
    $scope.receieveExecuteAllQueries = function (data) {
        $scope.$evalAsync(function () {
            var dialogScope = $scope.$new();
            $rootScope.IsLoading = false;
            dialogScope.EntityExecuteQueryData = data;
            dialogScope.isExecuted = true;
            dialogScope.isShowingTopRecords = !$scope.hasQueryTopClause($scope.selectedQuery.dictAttributes[$scope.objselectedQueryTab.Attribute]);
            $scope.showExecutionQueryDialog(dialogScope);
        });
    };
    $scope.showExecutionQueryDialog = function (dialogScope) {
        dialogScope.title = "Execute Query";
        dialogScope.template = "Entity/views/ExecuteQuery.html";
        dialogScope.dialog = $rootScope.showDialog(dialogScope, dialogScope.title, dialogScope.template, {
            width: 800, height: 400
        });
    };

    var objofCutorCopyQuery = "";
    var paramitem = "";

    var CutOrCopyQuery = function (param) {
        if (param == 'copy') {
            $rootScope.UndRedoBulkOp("Start");
            var data = GetBaseModel(objofCutorCopyQuery);

            data.dictAttributes.ID = GetNewQueryName("NewQuery", $scope.objQueries, 1);

            $rootScope.PushItem(data, $scope.objQueries.Elements, "SelectEntityQuery");
            $scope.selectedQuery = data;
            $scope.selectQuery($scope.selectedQuery, 0);
            $rootScope.UndRedoBulkOp("End");

        }
        objofCutorCopyQuery = "";
    };

    $scope.QueryPasteMenuOption = [['Paste', function ($itemScope) {
        CutOrCopyQuery(paramitem);
    }, function ($itemScope) {
        return (objofCutorCopyQuery != "");
    }]
    ];

    $scope.queryMenuOptions = [
        ['Rename', function ($itemScope) {
            $scope.newQueryName = $scope.selectedQuery.dictAttributes.ID;
            var newScope = $scope.$new(true);
            newScope.IsExecuteQueryShow = false;
            $scope.IsExecuteQueryShow = false;
            var newQueryID = $scope.selectedQuery.dictAttributes.ID;
            newScope.newQueryName = newQueryID;
            $scope.newQueryName = $scope.selectedQuery.dictAttributes.ID;
            newScope.isEditQueryID = true;

            var addNewQueryDialog = $rootScope.showDialog(newScope, "Rename Query Name", "Entity/views/NewQueryTemplate.html", {
                width: 500, height: 180
            });

            newScope.validateQueryDailog = function () {
                newScope.queryError = undefined;
                if (!newScope.newQueryName) {
                    newScope.queryError = "Error: Enter ID.";
                    return;
                }
                else if (!isValidIdentifier(newScope.newQueryName)) {
                    newScope.queryError = "Error:Invalid Query ID.";
                    return;
                }
                else {
                    var objDuplicate = $ValidationService.findDuplicateId($scope.objEntity, newScope.newQueryName, CONST.ENTITY.NODES, null, $scope.selectedQuery, "dictAttributes.ID");
                    if (objDuplicate) {
                        newScope.queryError = CONST.VALIDATION.DUPLICATE_ID;
                        return;
                    }
                }
            };

            newScope.setNewQueryDetails = function () {
                $rootScope.UndRedoBulkOp("Start");
                $scope.IsExecuteQueryShow = false;
                if (newScope.isEditQueryID == true) {
                    var index = $scope.objQueries.Elements.indexOf($scope.selectedQuery);
                    $rootScope.EditPropertyValue($scope.objQueries.Elements[index].dictAttributes.ID, $scope.objQueries.Elements[index].dictAttributes, "ID", this.newQueryName);
                    $rootScope.EditPropertyValue($scope.selectedQueryName, $scope, "selectedQueryName", $scope.objEntity.dictAttributes.ID + "." + $scope.objQueries.Elements[index].dictAttributes.ID);
                }

                newScope.closeNewQueryDialog();

                $rootScope.UndRedoBulkOp("End");
                validateQueries(true);
            };

            newScope.closeNewQueryDialog = function () {
                addNewQueryDialog.close();
            };


        }, function ($itemScope) {
            return ($scope.objQueries != undefined);
        }],
        ['Delete', function ($itemScope) {
            $scope.removeSelectedQuery('SelectEntityQuery');

        }, function ($itemScope) {
            return ($scope.objQueries != undefined);
        }],
        ['Copy', function ($itemScope) {

            objofCutorCopyQuery = $scope.selectedQuery;
            paramitem = 'copy';
        }, function ($itemScope) {
            return ($itemScope.objQueries != undefined && ($itemScope.objQueries.dictAttributes.ID != '' && $itemScope.objQueries.Name == 'query') || ($itemScope.objQueries.Name == 'queries' && $scope.selectedQuery.Name != "group"));
        }],
        ['Paste', function ($itemScope) {
            CutOrCopyQuery(paramitem);
        }, function ($itemScope) {
            return (objofCutorCopyQuery != "");
        }],
        ['Find Reference(s)', function ($itemScope, event, model, text) {
            // find logic comes here - with a loading animation
            // after loading - popover animation effect will come            
            var currentobj = $(event.currentTarget);
            if (currentobj[0]) {
                var idSelectedQuery = $scope.objEntity.dictAttributes.ID + '_' + $scope.selectedQuery.dictAttributes.ID;
                $searchFindReferences.resetData();
                $scope.IsQueryReferencePopover = true;
                $scope.$searchFindReferences = undefined;
                // clear the existing popover is present
                if ($(currentobj[0]).data('bs.popover')) {
                    $(currentobj[0]).popover('destroy');
                }
                // clear floating button if present
                $('body').find(".findReferences-floating-btn").remove();
                var popoverTemplate = ['<div class="find-reference-popover popover">',
                    '<div class="arrow"></div>',
                    '<div class="popover-content pull-left" onclick="stopEventPropagationConditional(event)">',
                    '</div>',
                    '</div>'].join('');
                $(currentobj[0]).popover({
                    trigger: 'manual',
                    animation: true,
                    html: true,
                    template: popoverTemplate,
                    content: $compile("<loading-box lazymodel='$searchFindReferences' size='xs'></loading-box>")($scope)
                });
                $(currentobj[0]).popover('show');
                // check if screen will show popover content properly
                var $results = $(currentobj[0]).closest('#' + idSelectedQuery).find('.popover')[0],
                    windowsHeight = $(window).height(), $currentParent = $(currentobj[0]).closest(".list-items-queries");
                // for horizontal positioning if element is not entirely visible 
                if ($currentParent && $currentParent.length > 0 && $currentParent[0] && ($(currentobj[0]).width() > $($currentParent[0]).width())) {
                    $($results).css("left", $($results).position().left - ($($results).position().left - $($currentParent[0]).width()) + "px");
                }
                //var timer = performance.now();
                // call factory method to find reference and show popover
                var searchPromise = $searchFindReferences.get($scope.selectedQueryName, "Query");
                searchPromise.done(function (data) {
                    //console.log(performance.now() - timer);
                    if (data && data.length > 0) {
                        $searchFindReferences.setData("Query", data, null);
                        $scope.$searchFindReferences = $searchFindReferences.getData;
                        var popover = $(currentobj[0]).data('bs.popover');
                        if (popover) {
                            popover.options.content = $compile('<find-referencebox></find-referencebox>')($scope);
                            $(currentobj[0]).popover('show');
                            // 264px is the max height reference popover can take
                            // 200px is the max height of the result ul (css), +64px for header and padding
                            var numReferencetobeLoadedInitial = 6;
                            var popoverHeight = data.length <= numReferencetobeLoadedInitial ? (data.length * 32) + 20 : (numReferencetobeLoadedInitial * 32) + 20;
                            // for horizontal positioning if element is not entirely visible 
                            if ($currentParent && $currentParent.length > 0 && $currentParent[0] && ($(currentobj[0]).width() > $($currentParent[0]).width())) {
                                $($results).css("left", $($results).position().left - ($($results).position().left - $($currentParent[0]).width()) + "px");
                            }
                            // for vertical positioning if popover content is going below the screen
                            if (windowsHeight < $($results)[0].getBoundingClientRect().top + popoverHeight + 40) {
                                var arrowElement = $($results).find('.arrow')[0];
                                var topElement = $(arrowElement).position().top;
                                $(arrowElement).css("top", topElement + popoverHeight - 30 + "px");
                                $($results).css("top", $($results).position().top - popoverHeight + 10 + "px");
                            }
                        }
                    }
                    else {
                        $(currentobj[0]).closest('#' + idSelectedQuery).find('.popover').remove();
                        toastr.warning("No reference found");
                    }
                });
            }
            return true;
        }, function ($itemScope) {
            return ($itemScope.objQuery.dictAttributes.ID == $scope.selectedQuery.dictAttributes.ID);
        }]
    ];

    $scope.closeFindReferencesPopover = function (e) {
        if ($scope.IsQueryReferencePopover) {
            $(e.currentTarget).closest('.popover').remove();
        }
        else {
            $scope.triggerFloatingFind(e);
        }
    };

    $scope.triggerFindReferences = function (objRef, e) {
        var idSelectedQuery = $scope.$searchFindReferences.ReferenceValue.split(".");
        var objPopoverElement = $(e.currentTarget).closest('#' + idSelectedQuery[0] + '_' + idSelectedQuery[1]).find('.popover');
        if (objPopoverElement) {
            $(objPopoverElement).remove();
            $scope.IsQueryReferencePopover = false;
            // add a new element to body and set data in the factory for all the references 
            if ($('body').find(".findReferences-floating-btn").length <= 0) {
                var FloatingButtonTemplate = ['<span class="findReferences-floating-btn" filename="{{currentfile.FileName}}" data-toggle="popover" ng-click="triggerFloatingFind($event)">',
                    '<i class="fa fa-times-circle-o" data-role="close-button" aria-hidden="true" ng-click="triggerFloatingFind($event)"></i>',
                    '</span>'].join('');
                $('body').append($compile(FloatingButtonTemplate)($scope));
                var floatingbox = $compile('<find-referencebox onclick="stopEventPropagationConditional(event)"></find-referencebox>')($scope);
                $('body').find(".findReferences-floating-btn").append(floatingbox);
                $timeout(function () {
                    $searchFindReferences.setData(null, null, objRef);
                    $rootScope.openFile(objRef.FileInfo, false);
                }, 600);
            }
            else {
                $searchFindReferences.setData(null, null, objRef);
                $rootScope.openFile(objRef.FileInfo, false);
                $('body').find(".floating-reference-box").removeClass("showref");
            }
        }
    };

    $scope.triggerFloatingFind = function (e) {
        if (e && $(e.currentTarget).attr("data-role") === "close-button") {
            $('body').find(".findReferences-floating-btn").remove();
            e.stopPropagation();
        }
        else {
            $('body').find(".floating-reference-box").toggleClass("showref");
        }
    };

    //#endregion

    //#region Entity Details/Extra Fields
    $scope.validateParentKeyValue = function () {
        if ($scope.objEntity.dictAttributes.sfwErrorTable && !($scope.objEntity.dictAttributes.sfwParentKeyValue)) {
            $ValidationService.checkValidListValue([], $scope.objEntity, $scope.objEntity.dictAttributes.sfwParentKeyValue, "sfwParentKeyValue", "sfwParentKeyValue", "Parent Key Value for Error Table cannot be empty.", $scope.validationErrorList, true);
        }
        else if ($scope.objEntity.dictAttributes.sfwErrorTable && $scope.objEntity.dictAttributes.sfwParentKeyValue) {
            if ($scope.objEntity.errors && $scope.objEntity.errors.sfwParentKeyValue) {
                $scope.objEntity.errors.sfwParentKeyValue = undefined;
            }
        } else if (!($scope.objEntity.dictAttributes.sfwErrorTable) && $scope.objEntity.errors) {
            $scope.objEntity.errors.sfwParentKeyValue = undefined;
        }
    }

    $scope.validateStatusValue = function () {

        if ($scope.objEntity.dictAttributes.sfwErrorTable && !($scope.objEntity.dictAttributes.sfwStatusColumn)) {
            $ValidationService.checkValidListValue([], $scope.objEntity, $scope.objEntity.dictAttributes.sfwStatusColumn, "sfwStatusColumn", "sfwStatusColumn", "Status Column for Error Table cannot be empty.", $scope.validationErrorList, true);
        }
        else if ($scope.objEntity.dictAttributes.sfwErrorTable && $scope.objEntity.dictAttributes.sfwStatusColumn) {
            if ($scope.objEntity.dictAttributes.errors && $scope.objEntity.dictAttributes.errors.sfwStatusColumn) {
                $scope.objEntity.dictAttributes.errors.sfwStatusColumn = undefined;
            }
        } else if (!($scope.objEntity.dictAttributes.sfwErrorTable) && $scope.objEntity.dictAttributes.errors) {
            $scope.objEntity.dictAttributes.errors.sfwStatusColumn = undefined;
        }
    }

    $scope.onDetailClick = function () {
        var newScope = $scope.$new();

        newScope.SelectedEntityDetailsDialogTab = 'Details';
        newScope.objExtraFields = [];
        newScope.ID = undefined;
        newScope.objDummy = [];
        newScope.objVisibility = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Entity";
        newScope.objDirFunctions = {};
        newScope.objEntityDetails = {};
        newScope.objEntityID = $scope.objEntity.dictAttributes.ID;
        newScope.objEnitityDescription = $scope.objEntity.dictAttributes.Text;
        newScope.strParentEntity = $scope.objEntity.dictAttributes.sfwParentEntity;
        newScope.objEntityTableName = $scope.objEntity.dictAttributes.sfwTableName;
        newScope.objEnitityAS400 = $scope.objEntity.dictAttributes.sfwAS400Library;
        newScope.objEnitityDatabaseName = $scope.objEntity.dictAttributes.sfwDatabase;
        newScope.objHistoryEnabled = $scope.objEntity.dictAttributes.sfwHistoryEnabled;
        newScope.objVisibility.showInputTableName = false;
        newScope.blnShowCreateWithClass = $scope.blnShowCreateWithClass;

        newScope.objEntityDetails.sfwMainCDO = $scope.objEntity.dictAttributes.sfwMainCDO;
        newScope.objEntityDetails.sfwErrorTable = $scope.objEntity.dictAttributes.sfwErrorTable;
        newScope.objEntityDetails.sfwParentKey = $scope.objEntity.dictAttributes.sfwParentKey;
        newScope.objEntityDetails.sfwParentKeyValue = $scope.objEntity.dictAttributes.sfwParentKeyValue;
        newScope.objEntityDetails.sfwStatusColumn = $scope.objEntity.dictAttributes.sfwStatusColumn;
        newScope.objEntityDetails.sfwExpressionTable = $scope.objEntity.dictAttributes.sfwExpressionTable;
        newScope.objEntityDetails.sfwReadOnlyRule = $scope.objEntity.dictAttributes.sfwReadOnlyRule;
        newScope.objEntityDetails.sfwChecklistId = $scope.objEntity.dictAttributes.sfwChecklistId;
        newScope.objEntityDetails.sfwChecklistTable = $scope.objEntity.dictAttributes.sfwChecklistTable;
        newScope.objEntityDetails.sfwActive = $scope.objEntity.dictAttributes.sfwActive;
        //newScope.objEntityDetails.sfwCheckListTableHasIdentity = $scope.objEntity.dictAttributes.sfwCheckListTableHasIdentity;
        //newScope.objEntityDetails.sfwErrorTableHasIdentity = $scope.objEntity.dictAttributes.sfwErrorTableHasIdentity;

        newScope.objEntityBusinessObject = $scope.objEntity.dictAttributes.sfwObjectID;

        if (newScope.objEntityDetails.hasOwnProperty("sfwReadOnlyRule") && newScope.objEntityDetails.sfwReadOnlyRule) {

            var list = $ValidationService.getListByPropertyName($scope.objInitialLoad.Elements, "ID");
            $ValidationService.checkValidListValue(list, newScope.objEntityDetails, newScope.objEntityDetails.sfwReadOnlyRule, "sfwReadOnlyRule", "sfwReadOnlyRule", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList);
        }

        newScope.cdoChange = function (cdoValue) {
            var MainCdo = cdoValue;
            if (!MainCdo.sfwMainCDO) {
                // $ValidationService.validateMainCdo(cdoValue, $scope.validationErrorList, MainCdo.sfwMainCDO);
                $ValidationService.checkValidListValue($scope.tableObjects, newScope.objEntityDetails, newScope.objEntityDetails.sfwMainCDO, "sfwMainCDO", "sfwMainCDO", CONST.VALIDATION.EMPTY_FIELD, $scope.validationErrorList, true);
            }
            else {
                var tblObjects = $scope.tableObjects.filter(function (obj) { if (obj) { return obj === cdoValue.sfwMainCDO } });
                if (tblObjects.length <= 0) {
                    var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                    if (!newScope.objEntityDetails.errors) {
                        newScope.objEntityDetails.errors = {};
                    }
                    newScope.objEntityDetails.errors.inValid_id = errorMessage;
                    delete newScope.objEntityDetails.errors.sfwMainCDO;
                    delete newScope.objEntityDetails.errors.empty_id;
                }
                else {
                    delete newScope.objEntityDetails.errors.sfwMainCDO;
                    newScope.objEntityDetails.errors.inValid_id = "";
                    newScope.objEntityDetails.errors.empty_id = "";
                }
            }

        };
        if (!newScope.objEntityDetails.sfwMainCDO && (newScope.objEntityBusinessObject != undefined && newScope.objEntityBusinessObject != "")) {
            newScope.cdoChange(newScope.objEntityDetails);
        }
        newScope.checkValidBusinessObject = function () {
            if (newScope.objEntityBusinessObject == undefined || newScope.objEntityBusinessObject == "") {
                newScope.IsValidBO = false;
            }
            else {
                //if business object is already added only validate it in the business object list 
                // var objEntityBusinessObject = newScope.objEntityBusinessObject;

                if (newScope.lstBusinessObject && !newScope.lstBusinessObject.some(function (x) { return x.ID == newScope.objEntityBusinessObject; })) {
                    newScope.EntityDetailsErrorMessage = "Error: Invalid Business Object.";
                    newScope.IsCircular = false;
                    newScope.IsValidBO = false;
                    $scope.blnShowBusinessObjectSection = false;
                }
                else {
                    newScope.IsValidBO = true;
                }
            }
        };

        $rootScope.IsLoading = true;
        hubMain.server.getTableIntellisense().done(function (data) {
            $scope.$apply(function () {
                $rootScope.IsLoading = false;
                newScope.lstMainTable = data;
                newScope.lstBusinessObject = $scope.lstBusinessObject;
                if ($scope.blnShowCreateWithClass) {
                    newScope.checkValidBusinessObject();
                }

                if (newScope.objEntityTableName == undefined || newScope.objEntityTableName == "") {
                    newScope.objVisibility.showInputTableName = true;
                }
                else {
                    var objEntityTableName = newScope.objEntityTableName;

                    if (newScope.lstMainTable && !newScope.lstMainTable.some(function (x) { return x.ID && x.ID.toLowerCase() == objEntityTableName.toLowerCase(); })) {
                        newScope.EntityDetailsErrorMessage = "Error: Invalid Table Name";
                        newScope.objVisibility.showInputTableName = true;
                        newScope.IsCircular = false;
                    }
                }

            });
        });

        newScope.checkValidEntityInheritence = function () {
            if (newScope.strParentEntity) {
                hubMain.server.checkValidParentEntity(newScope.objEntityID, newScope.strParentEntity).done(function (isValidParentEntity) {
                    newScope.$evalAsync(function () {
                        newScope.IsCircular = !isValidParentEntity;
                        newScope.validateEntityDetails();
                    });
                });
            }
        }
        newScope.receiveExtraSettingsModel = function (data) {
            $scope.$apply(function () {
                newScope.objCustomSettings = data;
                for (var i = 0; i < newScope.objCustomSettings.Elements.length; i++) {
                    if (newScope.objCustomSettings.Elements[i].Name == "Entity") {
                        newScope.temp = newScope.objCustomSettings.Elements[i];
                        break;
                    }
                }
            });
        };

        newScope.setEntityDetails = function () {

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.ID, $scope.objEntity.dictAttributes, "ID", newScope.objEntityID);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.Text, $scope.objEntity.dictAttributes, "Text", newScope.objEnitityDescription);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwTableName, $scope.objEntity.dictAttributes, "sfwTableName", newScope.objEntityTableName);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwObjectID, $scope.objEntity.dictAttributes, "sfwObjectID", newScope.objEntityBusinessObject);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwMainCDO, $scope.objEntity.dictAttributes, "sfwMainCDO", newScope.objEntityDetails.sfwMainCDO);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwErrorTable, $scope.objEntity.dictAttributes, "sfwErrorTable", newScope.objEntityDetails.sfwErrorTable);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwParentKey, $scope.objEntity.dictAttributes, "sfwParentKey", newScope.objEntityDetails.sfwParentKey);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwParentKeyValue, $scope.objEntity.dictAttributes, "sfwParentKeyValue", newScope.objEntityDetails.sfwParentKeyValue);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwStatusColumn, $scope.objEntity.dictAttributes, "sfwStatusColumn", newScope.objEntityDetails.sfwStatusColumn);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwExpressionTable, $scope.objEntity.dictAttributes, "sfwExpressionTable", newScope.objEntityDetails.sfwExpressionTable);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwReadOnlyRule, $scope.objEntity.dictAttributes, "sfwReadOnlyRule", newScope.objEntityDetails.sfwReadOnlyRule);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwChecklistId, $scope.objEntity.dictAttributes, "sfwChecklistId", newScope.objEntityDetails.sfwChecklistId);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwChecklistTable, $scope.objEntity.dictAttributes, "sfwChecklistTable", newScope.objEntityDetails.sfwChecklistTable);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwActive, $scope.objEntity.dictAttributes, "sfwActive", newScope.objEntityDetails.sfwActive);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwDatabase, $scope.objEntity.dictAttributes, "sfwDatabase", newScope.objEnitityDatabaseName);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwAS400Library, $scope.objEntity.dictAttributes, "sfwAS400Library", newScope.objEnitityAS400);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwParentEntity, $scope.objEntity.dictAttributes, "sfwParentEntity", newScope.strParentEntity);
            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwHistoryEnabled, $scope.objEntity.dictAttributes, "sfwHistoryEnabled", newScope.objHistoryEnabled);
            //if (newScope.objEntityDetails.sfwErrorTable) {
            //    $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwErrorTableHasIdentity, $scope.objEntity.dictAttributes, "sfwErrorTableHasIdentity", newScope.objEntityDetails.sfwErrorTableHasIdentity);
            //}
            //else if ($scope.objEntity.dictAttributes.sfwErrorTableHasIdentity && $scope.objEntity.dictAttributes.sfwErrorTableHasIdentity=="True"){
            //    $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwErrorTableHasIdentity, $scope.objEntity.dictAttributes, "sfwErrorTableHasIdentity", "");
            //}
            //if (newScope.objEntityDetails.sfwChecklistTable) {
            //    $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwCheckListTableHasIdentity, $scope.objEntity.dictAttributes, "sfwCheckListTableHasIdentity", newScope.objEntityDetails.sfwCheckListTableHasIdentity);
            //}
            //else if ($scope.objEntity.dictAttributes.sfwCheckListTableHasIdentity && $scope.objEntity.dictAttributes.sfwCheckListTableHasIdentity=="True"){
            //    $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwCheckListTableHasIdentity, $scope.objEntity.dictAttributes, "sfwCheckListTableHasIdentity","");
            //}
            newScope.EntityDetailsErrorMessage = "";
            var tempparentEntity = $scope.objEntity.dictAttributes.sfwParentEntity;
            if (newScope.strParentEntity && newScope.strParentEntity != "") {
                hubMain.server.getInheritedRule(newScope.objEntityID, newScope.strParentEntity).done(function (data) {
                    if (data == null) {
                        newScope.$evalAsync(function () {
                            newScope.IsCircular = true;
                            $rootScope.EditPropertyValue($scope.objEntity.dictAttributes.sfwParentEntity, $scope.objEntity.dictAttributes, "sfwParentEntity", "");

                            newScope.validateEntityDetails();
                        });
                    }
                    else {
                        $scope.$evalAsync(function () {
                            $scope.selectedIneritedTab = 'InheritedRule';
                            $scope.objInheritedRules = data;
                            if ($scope.SelectedView === "ValidationRules") {
                                if ($scope.objInheritedRules.Elements && $scope.objInheritedRules.Elements.length > 0) {
                                    $scope.selectedRules($scope.objInheritedRules.Elements[0]);
                                }
                                else {
                                    $scope.selectedRules(undefined);
                                }
                            }
                            $scope.objInheritedRules.Text = "Rules";
                            $scope.objInheritedRules.IsExpanded = true;
                            if ($scope.objInheritedRules && $scope.objInheritedRules.Elements.length > 0) {
                                angular.forEach($scope.objInheritedRules.Elements, function (obj) {
                                    $scope.LoadRulesDetails(obj);
                                });
                            }

                        });
                        newScope.closeEntityDetails();
                    }
                });
            }
            else if ($scope.objInheritedRules) {
                $scope.$evalAsync(function () {
                    $scope.objInheritedRules = undefined;
                    $scope.selectedIneritedTab = 'Rule';
                    if ($scope.objRules.Elements.length > 0 && $scope.SelectedView === "ValidationRules") {
                        $scope.selectedRules($scope.objRules.Elements[0], null);
                    }
                });
            }

            newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data

            $rootScope.UndRedoBulkOp("End");


            if (!newScope.IsCircular && !newScope.strParentEntity) {
                newScope.closeEntityDetails();
            }
            if ($scope.objEntity.dictAttributes.sfwObjectID) {
                $scope.blnShowBusinessObjectSection = true;
            }

            //ngDialog.close(entityDialog);
        };
        //newScope.onReadOnlyRuleChange = function (objEntityDetails) {
        //    var obj = objEntityDetails;
        //}
        newScope.validateEntityDetails = function () {
            newScope.EntityDetailsErrorMessage = "";
            if (!newScope.IsValidBO && newScope.objEntityBusinessObject) {
                if (newScope.lstBusinessObject && !newScope.lstBusinessObject.some(function (x) { return x.ID == newScope.objEntityBusinessObject; })) {
                    newScope.EntityDetailsErrorMessage = "Error: Invalid Business Object.";
                    newScope.IsCircular = false;
                    newScope.IsValidBO = false;
                    $scope.blnShowBusinessObjectSection = false;
                }
            }

            if (!newScope.objVisibility.showInputTableName) {
                var objEntityTableName = newScope.objEntityTableName;

                if (newScope.lstMainTable && !newScope.lstMainTable.some(function (x) { return x.ID && objEntityTableName && x.ID.toLowerCase() == objEntityTableName.toLowerCase(); })) {

                    newScope.EntityDetailsErrorMessage = "Error: Invalid Table Name";
                    newScope.objVisibility.showInputTableName = true;
                    newScope.IsCircular = false;
                }
            }

            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

            var strParentEntity = this.strParentEntity;
            if (this.strParentEntity && !entityIntellisenseList.some(function (x) { return x.ID == strParentEntity; })) {
                newScope.EntityDetailsErrorMessage = "Error: Invalid Parent Entity";
                newScope.IsCircular = false;
            }

            if (this.strParentEntity && newScope.objEntityTableName && entityIntellisenseList) {
                var larrEntity = entityIntellisenseList.filter(function (aobjEntity) { return aobjEntity.ID == strParentEntity; });
                if (larrEntity.length > 0 && larrEntity[0].TableName) {
                    newScope.EntityDetailsErrorMessage = "Error: Multiple tables not allowed within parent and child entity.";
                    return true;
                }
            }
            if (newScope.IsCircular && this.strParentEntity && this.strParentEntity != "") {
                newScope.EntityDetailsErrorMessage = "Error: Multiple tables not allowed within parent and child entity.";
                return true;
            }
            else {
                newScope.IsCircular = false;
            }


            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var extraValueFlag = validateExtraFields(newScope);
            if (extraValueFlag) {
                newScope.EntityDetailsErrorMessage = newScope.FormDetailsErrorMessage;
                return true;
            }


            return false;
        };

        newScope.closeEntityDetails = function () {
            if ($scope.objEntity && $scope.objEntity.errors && $scope.objEntity.errors.invalid_code_group) {
                delete $scope.objEntity.errors.invalid_code_group;
            }
            $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.objEntity);
            newScope.entityDetailDialog.close();
        };

        newScope.selectEntityDetailsDialogTab = function (selecteditm) {
            newScope.SelectedEntityDetailsDialogTab = selecteditm;
        };


        newScope.entityDetailDialog = $rootScope.showDialog(newScope, "Entity Details", "Entity/views/EntityDetails.html", {
            width: 600, height: 661
        });
        newScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                //newScope.setEntityDetails();
                newScope.closeEntityDetails();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };
        newScope.selectRuleClick = function (aRuleID) {
            if (aRuleID && aRuleID != "") {
                newScope.closeEntityDetails();
                $NavigateToFileService.NavigateToFile($rootScope.currentopenfile.file.FileName, "rules", aRuleID);
            }
        };
        newScope.openBusinessObjectEditClick = function () {
            var newBOScope = newScope.$new();
            newBOScope.objEntityBusinessObject = "";
            newScope.BusinessObjectEditDialog = $rootScope.showDialog(newBOScope, "Set Business Object", "Entity/views/SetBusinessObject.html", {
                width: 400, height: 150
            });

            newBOScope.setBusinessObject = function () {


                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var dummyobjEntityBusinessObject = newBOScope.objEntityBusinessObject;

                //if business object is getting added for first time validate that the same business object is not being used in any other entity
                if (dummyobjEntityBusinessObject && (entityIntellisenseList.some(function (x) { return x.BusinessObjectName == dummyobjEntityBusinessObject; }))) {
                    alert("Error: Invalid Business Object. The business object is used in another entity.");
                }
                else {
                    //if business object is getting added for first time and the same business object is not being used in any other entity then check if it is a existing business object it should have the 
                    //valid do property in it only then it can get associated. Also check if properties.cs not present then the bus class should add be a "partial" class only then properties.cs will get added

                    if (dummyobjEntityBusinessObject && newScope.objEntityTableName && newScope.lstBusinessObject && newScope.lstBusinessObject.some(function (x) { return x.ID == dummyobjEntityBusinessObject; })) {

                        $.connection.hubEntityModel.server.validateBusinessObject(dummyobjEntityBusinessObject, newScope.objEntityTableName).done(function (data) {
                            if (data == false) {
                                alert("Error: Invalid Business Object. The business object does not contain a valid do property.");
                            }
                            else {
                                newScope.objEntityBusinessObject = newBOScope.objEntityBusinessObject;
                                newScope.$evalAsync(function () {
                                    newScope.IsValidBO = true;
                                    newScope.EntityDetailsErrorMessage = "";
                                    newScope.validateEntityDetails();
                                });
                                newBOScope.closeBusinessObjectDialog();
                            }
                        });

                    }
                    //if table name is not specified then user cannot create new business object. They can only associate to an exisiting business object
                    else if (!newScope.objEntityTableName && newScope.lstBusinessObject && !newScope.lstBusinessObject.some(function (x) { return x.ID == dummyobjEntityBusinessObject; })) {
                        alert("Error: Invalid Business Object.");
                    }
                    else {
                        newScope.objEntityBusinessObject = newBOScope.objEntityBusinessObject;
                        newScope.IsValidBO = true;
                        newScope.EntityDetailsErrorMessage = "";
                        newScope.validateEntityDetails();
                        newBOScope.closeBusinessObjectDialog();
                    }

                }
            };

            newBOScope.closeBusinessObjectDialog = function () {
                newScope.BusinessObjectEditDialog.close();
            };
        };


        newScope.openTableNameEditClick = function () {
            var newTableScope = newScope.$new();
            newTableScope.objEntityTableName = "";
            newScope.TableEditDialog = $rootScope.showDialog(newTableScope, "Set Table Name", "Entity/views/SetTableName.html", {
                width: 500, height: 150
            });

            newTableScope.setTableName = function () {
                newTableScope.EntityTableErrorMessage = "";

                if (newScope.objEntityBusinessObject) {
                    //if data object file is not there we will create it and associate it with the business object
                    $.connection.hubEntityModel.server.isDataObjectFilePresent(newTableScope.objEntityTableName).done(function (data) {
                        $scope.$evalAsync(function () {
                            if (data == true) {
                                //if data object file present then business object should have a valid property corresponding to the data object
                                $.connection.hubEntityModel.server.validateBusinessObject(newScope.objEntityBusinessObject, newTableScope.objEntityTableName).done(function (data) {
                                    if (data == false) {
                                        alert("Error: Invalid Table. The business object does not contain a valid do property.");
                                    }
                                    else {
                                        newScope.$evalAsync(function () {
                                            newScope.objEntityTableName = newTableScope.objEntityTableName;
                                            newScope.objVisibility.showInputTableName = false;
                                            newScope.validateEntityDetails();
                                        });
                                        newTableScope.closeTableDialog();
                                    }
                                });
                            }
                            else {
                                //alert("Error: Invalid Table. The table not have a valid do .cs file.");
                                if (newTableScope.objEntityTableName != undefined && newTableScope.objEntityTableName != "") {
                                    newScope.strEntityID = GetFormattedTableName(newTableScope.objEntityTableName.toLowerCase());
                                    newScope.FormattedClassName = newScope.strEntityID;
                                    newScope.objEntityTableName = newTableScope.objEntityTableName;
                                    newScope.objVisibility.showInputTableName = false;
                                    newScope.validateEntityDetails();
                                    newTableScope.closeTableDialog();
                                }
                            }
                        });
                    });
                }
                else {
                    if (newTableScope.objEntityTableName != undefined && newTableScope.objEntityTableName != "") {
                        newScope.strEntityID = GetFormattedTableName(newTableScope.objEntityTableName.toLowerCase());
                        newScope.FormattedClassName = newScope.strEntityID;
                        if ($scope.blnShowCreateWithClass) {
                            newScope.objEntityBusinessObject = "bus" + newScope.strEntityID;
                        }

                        newScope.objEntityTableName = newTableScope.objEntityTableName;
                        newScope.objVisibility.showInputTableName = false;
                        newScope.validateEntityDetails();
                        newTableScope.closeTableDialog();
                    }
                }

            };
            newTableScope.validateTableName = function () {
                newTableScope.EntityTableErrorMessage = "";

                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var dummyobjEntityTableName = newTableScope.objEntityTableName;
                if (dummyobjEntityTableName) {
                    if (entityIntellisenseList.some(function (y) { return y.TableName == dummyobjEntityTableName; })) {
                        newTableScope.EntityTableErrorMessage = "Error: Invalid Table Name. The table name is used in another entity.";
                    }
                    else {
                        if (newScope.lstMainTable && !newScope.lstMainTable.some(function (x) { return x.ID == dummyobjEntityTableName; })) {
                            newTableScope.EntityTableErrorMessage = "Error: Invalid Table Name.";
                        }
                    }
                }
                else {
                    newTableScope.EntityTableErrorMessage = "Error: Invalid Table Name.";
                }

            };

            newTableScope.closeTableDialog = function () {
                newScope.TableEditDialog.close();
            };

        };

        newScope.PopulateErrorDetails = function (obj, errortable) {

            newScope.objEntityDetails.sfwParentKey = "";
            newScope.objEntityDetails.sfwParentKeyValue = "";
            newScope.objEntityDetails.sfwStatusColumn = "";
            if (!obj.errors && !angular.isObject(obj.errors)) {
                obj.errors = {};
            }
            obj.errors.sfwErrorTable = "";
            if (errortable != undefined && errortable != "") {
                var lst = $scope.lstErrorTable.filter(function (x) {
                    return x == errortable;
                });
                if (lst && lst.length > 0) {
                    $.connection.hubEntityModel.server.populateErrorTableDetails(errortable).done(function (data) {
                        $scope.$apply(function () {

                            newScope.lstErrorDetails = data;

                            if (newScope.lstErrorDetails != undefined) {
                                newScope.objEntityDetails.sfwParentKey = newScope.lstErrorDetails;


                                if (newScope.objEntityDetails.sfwMainCDO != undefined) {
                                    newScope.objEntityDetails.sfwParentKeyValue = newScope.objEntityDetails.sfwMainCDO + "." + data;
                                }
                                else {
                                    newScope.objEntityDetails.sfwParentKeyValue = "." + data;
                                }

                                newScope.objEntityDetails.sfwStatusColumn = "status_value";
                            }
                        });
                    });
                }
                else {
                    if (lst.length <= 0) {
                        var errorMessage = CONST.VALIDATION.INVALID_ERROR_TABLE;
                        obj.errors.sfwErrorTable = errorMessage;
                    }
                }

            }
            //else {
            //    var inValid = false;
            //    var ID = errortable;                
            //    if (!ID) {
            //        delete obj.errors.inValid_id;
            //        if (!obj.errors.sfwErrorTable)
            //            obj.errors.sfwErrorTable = CONST.VALIDATION.EMPTY_ERROR_TABLE;
            //        inValid = true;
            //    }               
            //    if (ID) {
            //        delete obj.errors.sfwErrorTable;
            //    }                
            //}
        };
        newScope.PopulateChecklistTable = function (obj, checklistTable) {

            if (!obj.errors && !angular.isObject(obj.errors)) {
                obj.errors = {};
            }
            obj.errors.sfwChecklistTable = "";
            if (checklistTable != undefined && checklistTable != "") {
                var lst = $scope.lstChecklistTable.filter(function (x) {
                    return x == checklistTable;
                });
                if (lst.length <= 0) {
                    var errorMessage = CONST.VALIDATION.INVALID_CHECKLIST_TABLE;
                    obj.errors.sfwChecklistTable = errorMessage;
                }



            }
            //else {
            //    var inValid = false;
            //    var ID = checklistTable;
            //    if (!ID) {
            //        delete obj.errors.inValid_id;
            //        if (!obj.errors.sfwChecklistTable)
            //            obj.errors.sfwChecklistTable = CONST.VALIDATION.EMPTY_CHECKLIST_TABLE;
            //        inValid = true;
            //    }               
            //    if (ID) {
            //        delete obj.errors.sfwChecklistTable;
            //    }                
            //}

        };
        //if (!newScope.objEntityDetails.sfwErrorTable) {
        //    newScope.PopulateErrorDetails(newScope.objEntityDetails, "");
        //}
        //if (!newScope.objEntityDetails.sfwChecklistTable) {
        //    newScope.PopulateChecklistTable(newScope.objEntityDetails,"");
        //}
        newScope.checkValidEntityInheritence();
        if ($scope.blnShowCreateWithClass) {
            newScope.checkValidBusinessObject();
        }

    };
    //#endregion 

    //#region Custom mapping

    $scope.isObjectTreeVisible = false;
    $scope.toggleObjectTreeName = 'Expand Object Tree';
    $scope.toggleObjectTreeView = function () {
        $scope.isObjectTreeVisible = !$scope.isObjectTreeVisible;
        if ($scope.isObjectTreeVisible)
            $scope.toggleObjectTreeName = 'Collapse Object Tree';
        else
            $scope.toggleObjectTreeName = 'Expand Object Tree';
    };


    $scope.addMappedColumn = function () {
        var strQuery = undefined;
        var newScope = $scope.$new(true);
        if ($scope.selectedQuery) {
            if ($scope.lstDBQueryTypes.length > 0) {
                var queryTypes = $scope.lstDBQueryTypes;
                strQuery = getQuery(queryTypes, $scope.selectedQuery);
            }
            var Param;
            for (var i = 0; i < $scope.selectedQuery.Elements.length; i++) {
                if ($scope.selectedQuery.Elements[i].Name == "parameters") {
                    Param = $scope.selectedQuery.Elements[i];
                }
            }

            $.connection.hubEntityModel.server.getQuerySchemaFields(strQuery, Param).done(function (data) {
                $scope.$evalAsync(function () {
                    newScope.QueryColumns = data;
                    newScope.TableColumnNames = [];
                    newScope.lstUnmappedColumns = [];
                    if ($scope.objAttributes) {
                        angular.forEach($scope.objAttributes.Elements, function (value, key) {
                            if (value.dictAttributes.sfwType == "Column") {
                                newScope.TableColumnNames.push(value.dictAttributes.sfwValue);
                            }
                        });
                    }
                    if (!$scope.selectedQuery.objMappedColumns) {
                        $rootScope.PushItem({ Name: "mappedcolumns", Value: '', dictAttributes: {}, Elements: [] }, $scope.selectedQuery.Elements);
                        var lst = $scope.selectedQuery.Elements.filter(function (x) {
                            return x.Name == "mappedcolumns";
                        });
                        if (lst && lst.length > 0) {
                            $scope.selectedQuery.objMappedColumns = lst[0];
                        }
                    }
                    angular.forEach(newScope.QueryColumns, function (value) {

                        var columnName = value.toLowerCase();
                        if (newScope.TableColumnNames.indexOf(columnName) > -1) {
                            var strEntityField = "";
                            if ($scope.selectedQuery.objMappedColumns) {
                                var mapColumn = $scope.selectedQuery.objMappedColumns.Elements.filter(function (x) {
                                    return x.dictAttributes.ID && x.dictAttributes.ID.toLowerCase() == columnName;
                                });
                                if (mapColumn && mapColumn.length == 0) {
                                    newScope.lstUnmappedColumns.push({ dictAttributes: { ID: columnName, sfwEntityField: "" }, Elements: [], Name: "column" });
                                }
                            }
                        }
                    });
                    $rootScope.IsLoading = false;
                });
            });
        }
        newScope.isMappedColumnSelectAllChecked = false;
        var mappedDialog = $rootScope.showDialog(newScope, "Add Mapped Column", "Entity/views/AddMappedColumnTemplate.html", {
            width: 500, height: 600
        });

        newScope.onSelectAllMappedColumnChange = function () {
            if (newScope.isMappedColumnSelectAllChecked) {
                for (var i = 0; i < newScope.lstUnmappedColumns.length; i++) {
                    newScope.lstUnmappedColumns[i].IsSelect = true;
                }
            }
            else {
                for (var i = 0; i < newScope.lstUnmappedColumns.length; i++) {
                    newScope.lstUnmappedColumns[i].IsSelect = false;
                }
            }
        };

        newScope.setUnmappedColumns = function () {
            $rootScope.UndRedoBulkOp("Start");

            for (var i = 0; i < newScope.lstUnmappedColumns.length; i++) {
                if (newScope.lstUnmappedColumns[i].IsSelect) {
                    newScope.lstUnmappedColumns[i].IsDeleteColumn = true;
                    newScope.lstUnmappedColumns[i].dictAttributes.sfwEntityField = $scope.GetEntityField(newScope.lstUnmappedColumns[i].dictAttributes.ID);
                    if ($scope.selectedQuery.objMappedColumns) {
                        $rootScope.PushItem(newScope.lstUnmappedColumns[i], $scope.selectedQuery.objMappedColumns.Elements);
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");

            newScope.closeAddUnmappedColumnsDialog();

            //$scope.$apply();
            //$scope.$evalAsync();
        };

        newScope.closeAddUnmappedColumnsDialog = function () {
            mappedDialog.close();
        };
    };

    $scope.OnRemoveMappedQueryClickNew = function (obj, index) {
        $rootScope.DeleteItem($scope.selectedQuery.objMappedColumns.Elements[index], $scope.selectedQuery.objMappedColumns.Elements);
    };


    $scope.refreshUnmappedColumnNew = function () {
        $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
        $scope.refreshUnmappedColumn();
    };

    $scope.GetMappedColumns = function () {
        if ($scope.selectedQuery) {
            var strQuery = undefined;
            if ($scope.lstDBQueryTypes.length > 0) {
                var queryTypes = $scope.lstDBQueryTypes;
                strQuery = getQuery(queryTypes, $scope.selectedQuery);
            }

            var Param;
            for (var i = 0; i < $scope.selectedQuery.Elements.length; i++) {
                if ($scope.selectedQuery.Elements[i].Name == "parameters") {
                    Param = $scope.selectedQuery.Elements[i];
                }
            }

            $scope.TableColumnNames = [];
            if ($scope.objAttributes) {
                angular.forEach($scope.objAttributes.Elements, function (value, key) {
                    if (value.dictAttributes.sfwType == "Column") {
                        $scope.TableColumnNames.push(value.dictAttributes.sfwValue);
                    }
                });
            }

            if ($scope.selectedQuery) {
                var lst = $scope.selectedQuery.Elements.filter(function (x) {
                    return x.Name == "mappedcolumns";
                });
                if (lst && lst.length > 0) {
                    $scope.selectedQuery.objMappedColumns = lst[0];
                }
            }
            if ($scope.selectedQuery.objMappedColumns && $scope.selectedQuery.objMappedColumns.Elements) {
                angular.forEach($scope.selectedQuery.objMappedColumns.Elements, function (item) {
                    if (item.dictAttributes && item.dictAttributes.ID) {
                        if ($scope.TableColumnNames.indexOf(item.dictAttributes.ID) > -1) {
                            item.IsDeleteColumn = true;
                        }
                    }
                });
            }
        }
    };

    $scope.refreshUnmappedColumn = function () {
        var strQuery = undefined;
        if ($scope.selectedQuery) {
            if ($scope.lstDBQueryTypes.length > 0) {
                var queryTypes = $scope.lstDBQueryTypes;
                strQuery = getQuery(queryTypes, $scope.selectedQuery);
            }
            var Param;
            for (var i = 0; i < $scope.selectedQuery.Elements.length; i++) {
                if ($scope.selectedQuery.Elements[i].Name == "parameters") {
                    Param = $scope.selectedQuery.Elements[i];
                }
            }
            $rootScope.IsLoading = true;
            $.connection.hubEntityModel.server.getQuerySchemaFields(strQuery, Param).done(function (data) {
                $scope.$apply(function () {
                    $scope.QueryColumns = data;
                    $scope.TableColumnNames = [];
                    if ($scope.objAttributes) {
                        angular.forEach($scope.objAttributes.Elements, function (value, key) {
                            if (value.dictAttributes.sfwType == "Column") {
                                $scope.TableColumnNames.push(value.dictAttributes.sfwValue);
                            }
                        });
                    }
                    $rootScope.UndRedoBulkOp("Start");
                    if (!$scope.selectedQuery.objMappedColumns) {
                        $rootScope.PushItem({ Name: "mappedcolumns", Value: '', dictAttributes: {}, Elements: [] }, $scope.selectedQuery.Elements);
                        var lst = $scope.selectedQuery.Elements.filter(function (x) {
                            return x.Name == "mappedcolumns";
                        });
                        if (lst && lst.length > 0) {
                            $scope.selectedQuery.objMappedColumns = lst[0];
                        }
                    }
                    //preserving old Custom mapping columns
                    var existingParameters = $scope.selectedQuery.objMappedColumns.Elements;

                    //clearing the current binded list
                    $scope.selectedQuery.objMappedColumns.Elements = [];


                    function iterator(value) {
                        var columnName = value.toLowerCase();
                        var customMappingParameters = new objElements();
                        customMappingParameters.Name = "column";
                        customMappingParameters.dictAttributes.ID = columnName;
                        customMappingParameters.dictAttributes.sfwEntityField = '';

                        if ($scope.TableColumnNames.indexOf(columnName) == -1) {
                            if ($scope.selectedQuery.objMappedColumns.Elements.length == 0)
                                $rootScope.PushItem(customMappingParameters, $scope.selectedQuery.objMappedColumns.Elements);
                            else {
                                if (!$scope.selectedQuery.objMappedColumns.Elements.some(function (param) { return param.dictAttributes.sfwParamaterName && param.dictAttributes.sfwParamaterName.toLowerCase() == columnName })) {
                                    $rootScope.PushItem(customMappingParameters, $scope.selectedQuery.objMappedColumns.Elements);
                                }
                            }
                        }
                    }
                    angular.forEach($scope.QueryColumns, iterator);


                    // retain the values of Parameters
                    if (existingParameters && existingParameters.length > 0) {
                        for (var idx = 0; idx < existingParameters.length; idx++) {
                            if (existingParameters[idx] && existingParameters[idx] && existingParameters[idx].dictAttributes.ID) {
                                var params = $scope.selectedQuery.objMappedColumns.Elements.filter(function (x) { return x.dictAttributes.ID == existingParameters[idx].dictAttributes.ID; });
                                if (params && params.length > 0) {
                                    if (existingParameters[idx].dictAttributes.sfwEntityField) {
                                        $rootScope.EditPropertyValue(params[0].dictAttributes.sfwEntityField, params[0].dictAttributes, "sfwEntityField", existingParameters[idx].dictAttributes.sfwEntityField);
                                    }
                                }

                            }
                        }
                    }

                    if ($scope.selectedQuery.objMappedColumns && $scope.selectedQuery.objMappedColumns.Elements) {
                        angular.forEach($scope.selectedQuery.objMappedColumns.Elements, function (item) {
                            if (item.dictAttributes && item.dictAttributes.ID) {
                                if ($scope.TableColumnNames.indexOf(item) > -1) {
                                    item.IsDeleteColumn = true;
                                }
                            }
                        });
                    }
                    $rootScope.UndRedoBulkOp("End");
                    $rootScope.IsLoading = false;
                });
            });
        }
    };
    $scope.GetEntityField = function (columnName) {
        var strEntityField = "";
        var lst1 = $scope.objAttributes.Elements.filter(function (x) {
            return x.dictAttributes.sfwValue == columnName;
        });
        if (lst1 && lst1.length > 0) {
            strEntityField = lst1[0].dictAttributes.ID;
        }
        return strEntityField;
    };

    $scope.onUnmappedColumnSelectionChange = function (objUnMapped) {
        $scope.selectedQuery.selectedMappedColumn = objUnMapped;
        $scope.selectedUnmappedColumn = objUnMapped;
    };

    $scope.CheckAndRemoveUnmappedColumnNode = function () {
        if ($scope.objQueries && $scope.objQueries.Elements && $scope.objQueries.Elements.length > 0) {
            angular.forEach($scope.objQueries.Elements, function (aQuery) {
                if (aQuery && aQuery.Elements) {
                    if (aQuery.objMappedColumns) {
                        if (aQuery.objMappedColumns.Name == "mappedcolumns" && aQuery.objMappedColumns.Elements && aQuery.objMappedColumns.Elements.length > 0) {
                            for (var i = aQuery.objMappedColumns.Elements.length; i--;) {
                                if (!aQuery.objMappedColumns.Elements[i].dictAttributes.sfwEntityField) {
                                    aQuery.objMappedColumns.Elements.splice(i, 1);
                                }
                            }

                            if (aQuery.Elements.indexOf(aQuery.objMappedColumns) <= -1 && aQuery.objMappedColumns.Elements.length > 0) {
                                aQuery.Elements.push(aQuery.objMappedColumns);
                            }
                        }
                    }
                    if (aQuery.objMappedColumns && aQuery.objMappedColumns.Elements && aQuery.objMappedColumns.Elements.length == 0) {
                        var index = aQuery.Elements.indexOf(aQuery.objMappedColumns);
                        if (index > -1) {
                            aQuery.Elements.splice(index, 1);
                        }
                    }
                }
            });
        }
    };
    //replace object field with entity field

    $scope.GetMainCdoProperty = function (objectField) {
        var retVal = false;
        if (objectField.match("^" + $scope.objEntity.dictAttributes.sfwMainCDO)) {
            retVal = true;
        }
        return retVal;
    };

    $scope.closeAddUnmappedColumnsDialog = function () {
        ngDialog.close(mappedDialog.id);
        mappedDialog.id = '';

    };

    //#endregion

    //#region Sql Query Tab fnctionalities

    //TODO:strNewQuery by ref returning or not

    $scope.getQueryParameters = function () {

        if ($scope.selectedQuery && $scope.selectedQuery.dictAttributes) {
            var query = undefined;
            var isParameterised = false;
            if ($scope.lstDBQueryTypes.length > 0) {
                var queryTypes = $scope.lstDBQueryTypes;
                for (var i = 0; i < queryTypes.length; i++) {
                    if (queryTypes[i] && queryTypes[i].Attribute) {
                        if ($scope.selectedQuery.dictAttributes[queryTypes[i].Attribute]) {
                            query = $scope.selectedQuery.dictAttributes[queryTypes[i].Attribute];
                            if (query && query.indexOf("@") > -1) {
                                isParameterised = true;
                                break;
                            }

                        }
                    }

                };
            }
            if (!isParameterised) {
                for (var j = 0; j < $scope.selectedQuery.Elements.length; j++) {
                    if ($scope.selectedQuery.Elements[j].Name == 'parameters') {
                        $scope.selectedQuery.Elements[j].Elements = [];
                    }
                }
            }

            if (query) {
                var larrSql = query.split(/[\s,()=\r\n]+/);
                $scope.larrParameter = [];
                var lstrParameterName;
                var queryParameter;
                var objParameters;
                var lstParameters = $scope.selectedQuery.Elements.filter(function (itm) {
                    return itm.Name == "parameters";
                });
                if (lstParameters && lstParameters.length > 0) {
                    objParameters = lstParameters[0];
                }

                if (!objParameters) {
                    objParameters = { Name: 'parameters', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                    $rootScope.PushItem(objParameters, $scope.selectedQuery.Elements);
                }

                //adds all parameters of query to array and whichever param is not in Elements is added to Elements
                for (var i = 0; i < larrSql.length; i++) {
                    var lblnFound = false;
                    if (larrSql[i].indexOf("@") > -1) {
                        queryParameter = larrSql[i];
                        if (objParameters) {
                            var parameterElements = objParameters.Elements;
                            for (var param = 0; param < parameterElements.length; param++) {

                                if (parameterElements[param].dictAttributes.ID == queryParameter) {
                                    lblnFound = true;
                                    $scope.larrParameter.push(queryParameter);
                                    //var objparameterdetails = { Parameter: queryParameter, Type: parameterElements[param].dictAttributes['sfwDataType'] };
                                    break;
                                }
                            }
                            if (lblnFound == false) {
                                var newQueryParameter = {
                                };
                                newQueryParameter.Name = "parameter";
                                newQueryParameter.dictAttributes = {
                                };
                                newQueryParameter.dictAttributes.ID = queryParameter;
                                newQueryParameter.dictAttributes.sfwDataType = "";
                                newQueryParameter.Value = "";
                                $rootScope.PushItem(newQueryParameter, parameterElements);
                                $scope.larrParameter.push(queryParameter);
                                //var objparameterdetails = { Parameter: queryParameter, Type: newQueryParameter.dictAttributes['sfwDataType'] };
                            }
                        }
                    }
                }
            }
            //checks for updated query parameters and removes params from elements which are not in current query
            if (objParameters && objParameters.Elements && objParameters.Elements.length > 0) {
                for (var i = 0; i < objParameters.Elements.length; i++) {
                    var parameterName = objParameters.Elements[i].dictAttributes.ID;
                    if ($scope.larrParameter.indexOf(parameterName) == -1) {
                        $rootScope.DeleteItem(objParameters.Elements[i], objParameters.Elements);
                    }
                }
            }
        }
    };

    $scope.AddNodeLockOperation = function () {
        var strQueryAttribute = $scope.objselectedQueryTab.Attribute;
        if ($scope.selectedQuery.dictAttributes[strQueryAttribute] != undefined && $scope.selectedQuery.dictAttributes[strQueryAttribute] != '') {
            var callQueryWithNoLock = $FormatQueryFactory.createQueryWithNoLock($scope.selectedQuery.dictAttributes[strQueryAttribute]);
            callQueryWithNoLock.then(function (astrQuery) {
                if (astrQuery) {
                    $scope.$evalAsync(function () {
                        $rootScope.EditPropertyValue($scope.selectedQuery.dictAttributes[strQueryAttribute], $scope.selectedQuery.dictAttributes, strQueryAttribute, astrQuery, "showSelectedQueryInEditor");
                        $scope.showSelectedQueryInEditor();
                    });
                }
            });
        }
    };

    $scope.formatQuery = function () {
        var strQueryAttribute = $scope.objselectedQueryTab.Attribute;
        if ($scope.selectedQuery.dictAttributes[strQueryAttribute] != undefined && $scope.selectedQuery.dictAttributes[strQueryAttribute] != '') {
            var callformattedQuery = $FormatQueryFactory.formatQuery($scope.selectedQuery.dictAttributes[strQueryAttribute]);
            callformattedQuery.then(function (astrQuery) {
                if (astrQuery) {
                    $scope.$evalAsync(function () {
                        $rootScope.EditPropertyValue($scope.selectedQuery.dictAttributes[strQueryAttribute], $scope.selectedQuery.dictAttributes, strQueryAttribute, astrQuery, "showSelectedQueryInEditor");
                        $scope.showSelectedQueryInEditor();
                    });
                }
            });
        }
    };

    //#endregion

    //#region Dashboard select functionality
    $scope.selectDashboard = function () {
        $scope.IsDashboardVisible = true;
        $scope.SelectedView = "DashBoard";
    };


    $scope.IsAttributesOpen = false;
    $scope.IsQueriesOpen = false;
    $scope.IsValidationRulesOpen = false;
    $scope.IsFieldConstraintOpen = false;
    $scope.IsBusinessObjectOpen = false;

    $scope.selectAttributeTab = function () {
        $scope.IsDashboardVisible = false;
        $scope.SelectedView = "Attributes";
        if (!$scope.IsAttributesOpen) {
            $scope.IsAttributesOpen = true;
        }
    };

    $scope.selectQueriesTab = function () {
        $scope.IsDashboardVisible = false;
        $scope.tab = 1;
        $scope.SelectedView = "Queries";

        if (!$scope.IsQueriesOpen) {
            $scope.IsQueriesOpen = true;
            $scope.objselectedQueryTab = $scope.lstDBQueryTypes[0];
            if ($scope.objQueries.Elements.length > 0) {
                $scope.selectQuery($scope.objQueries.Elements[0], 0);
            }
        }
        // if no query is selected - select the first one
        if (!$scope.selectedQuery && $scope.objQueries.Elements.length > 0) {
            $scope.selectQuery($scope.objQueries.Elements[0], 0);
        }
    };

    $scope.selectValidationRulesTab = function () {
        $scope.objRuleID.searchRuleByID = "";
        $scope.objRuleID.selectedTypeID = $scope.objRuleID.lstTypeIds[0];
        $scope.IsDashboardVisible = false;
        // setSelectedTabClass("Validation Rules");
        $scope.SelectedView = "ValidationRules";
        if ($scope.SelectedRuleView == "" || $scope.SelectedRuleView == undefined) {
            $scope.SelectedRuleView = "ValidationRules";
        }
        if ($scope.objBusinessObject.ObjTree) {
            $scope.objBusinessObject.ObjTree.IsExpanded = false;
        }
        $scope.CollapsedAllChildNode($scope.objBusinessObject.ObjTree);

        if (!$scope.IsValidationRulesOpen) {
            $scope.IsValidationRulesOpen = true;
        }

        if ($scope.objRules.Elements.length > 0 && $scope.SelectedRule == undefined) {
            $scope.selectedRules($scope.objRules.Elements[0], null, true);
        }
        else if ($scope.SelectedRule) {

            $scope.selectValidationRuleInAllTypes($scope.SelectedRule, true);
        }
    };
    $scope.selectInitialLoadTab = function () {
        $scope.SelectedRuleView = "ValidationRules";
        $scope.clearSelection();
        if ($scope.objInitialLoad.Elements.length > 0 && !$scope.SelectedInitialLoad) {
            $scope.SelectedInitialLoadClick($scope.objInitialLoad.Elements[0], null);
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('InitialLoad');
    };

    $scope.selectHardErrorTab = function () {
        $scope.SelectedRuleView = "ValidationRules";
        $scope.clearSelection();
        if ($scope.objHardError.Elements.length > 0 && !$scope.SelectedHardError) {
            $scope.SelectedErrorClick($scope.objHardError.Elements[0], null);
            $scope.setSelectedError($scope.objHardError, 0);
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('HardError');
    };

    $scope.selectSoftErrorTab = function () {
        $scope.SelectedRuleView = "ValidationRules";
        $scope.clearSelection();
        if ($scope.objSoftError.Elements.length > 0 && !$scope.SelectedSoftError) {
            $scope.SelectedErrorClick($scope.objSoftError.Elements[0], null);
            $scope.setSelectedError($scope.objSoftError, 0);
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('SoftError');
    };

    $scope.selectValidateDeleteTab = function () {
        $scope.SelectedRuleView = "ValidationRules";
        $scope.clearSelection();
        if ($scope.objValidateDelete.Elements.length > 0 && !$scope.SelectedValidateDelete) {
            $scope.SelectedValidateDeleteClick($scope.objValidateDelete.Elements[0], null);
            $scope.setSelectedValidateDelete($scope.objValidateDelete, 0);
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('ValidateDelete');
    };

    $scope.selectGroupTab = function () {
        $scope.SelectedRuleView = "ValidationRules";
        $scope.clearSelection();
        $scope.clearValidationRuleSelection();
        if ($scope.objGroupList.Elements.length > 0 && !$scope.SelectedGroup) {
            if ($scope.objGroupList.Elements[0].Elements.length > 0) {
                $scope.objGroupList.Elements[0].Elements[0];
                $scope.SelectGroupClick($scope.objGroupList.Elements[0].Elements[0], null);
            }
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('Group');
    };

    $scope.selectCheckListTab = function () {
        $scope.SelectedRuleView = "Checklist";
        $scope.clearSelection();
        if ($scope.objCheckList.Elements.length > 0 && !$scope.SelectedCheckList) {
            $scope.SelectCheckListClick($scope.objCheckList.Elements[0], null);
        }
        $scope.selectValidationRulesTab();
        $scope.SelectChildRuleView('Checklist');
    };

    $scope.selectFieldConstraintsTab = function () {
        $scope.IsDashboardVisible = false;
        $scope.SelectedView = "FieldConstraints";
        if (!$scope.IsFieldConstraintOpen) {
            $scope.IsFieldConstraintOpen = true;
        }
    };

    $scope.selectMainRulesTab = function () {
        $scope.IsDashboardVisible = false;
        $scope.SelectedView = "EntityRules";
    };

    $scope.selectBusinessObjectTab = function () {
        $scope.IsDashboardVisible = false;

        if (!$scope.IsBusinessObjectOpen) {
            $scope.IsBusinessObjectOpen = true;
        }
        if ($scope.objBusinessObject.ObjTree) {
            $scope.objBusinessObject.ObjTree.IsExpanded = false;
            $scope.CollapsedAllChildNode($scope.objBusinessObject.ObjTree);
        }
        $scope.SelectedView = "BusinessObject";
    };

    $scope.CollapsedAllChildNode = function (objtree) {
        if (objtree) {

            angular.forEach(objtree.ChildProperties, function (item) {
                item.IsExpanded = false;
                if (item.ChildProperties && item.ChildProperties.length > 0) {
                    $scope.CollapsedAllChildNode(item);
                }
            });
            angular.forEach(objtree.lstMethods, function (item) {
                item.IsExpanded = false;
                if (item.ChildProperties && item.ChildProperties.length > 0) {
                    $scope.CollapsedAllChildNode(item);
                }
            });
        }
    };

    function setSelectedTabClass(tabName) {
        $(".tab-links>ul>li>a").each(function (index, element) {
            if ($(element).text() === tabName) {
                $(element).attr("class", "selected-tab");
            }
            else {
                $(element).removeAttr("class");
            }
        });
    }

    $scope.IsDashboardVisible = true;
    //#endregion

    //#region Methods for Add New Logical Rule,Decision Table and Excel Matrix


    $scope.addRulesFromEntity = function (ruleType) {
        var scope = getScopeByFileName("MainPage");
        if (scope != undefined) {
            scope.currentFileName = $rootScope.currentopenfile.file.FileName;
            scope.currentFilePath = $rootScope.currentopenfile.file.FilePath;
            scope.addRules(ruleType);
        }
    };

    $scope.getClassforRulesItemDashboard = function (objRuleNode) {
        var retClass = "";
        if (objRuleNode.isSelected) {
            retClass = "selected";
        }
        if (objRuleNode.Data && objRuleNode.Data.IsStatic) {
            retClass = retClass + " home-filetype-" + objRuleNode.FileType + "-static-black";
        }
        else {
            retClass = retClass + " home-filetype-" + objRuleNode.FileType + "-black";
        }
        if (objRuleNode.isSelected && objRuleNode.isAdvanceSearched) {
            retClass = retClass + " bckgGreen";
        }
        else if (objRuleNode.isAdvanceSearched) {
            retClass = retClass + " bckgGrey";
        }
        return retClass;
    };

    $scope.SelectedMatrix = "ImportExcelMatrix";
    $scope.SelectedFile = "ExcelFile";
    $scope.SelectedTable = "Decisiontable";
    $scope.ExcelFilePath = "";
    $scope.ExcelSelectedQuery = "";
    $scope.Precision = "6";
    $scope.Delimiter = ",";
    $scope.ColumnIndex = -1;
    $scope.RowIndex = -1;



    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };
    //#endregion

    //#region Object Tree methods
    $scope.addFieldFromObjectTreeClick = function () {
        var newScope = $scope.$new();
        newScope.objBusinessObjectTree = {};
        newScope.objBusinessObjectTree.lstmultipleselectedfields = [];
        newScope.objBusinessObjectTree.lstCurrentBusinessObjectProperties = [];
        newScope.objBusinessObjectTree.lstdisplaybusinessobject = [];
        newScope.objBusinessObjectTree.BusObjectName = $scope.objEntity.dictAttributes.sfwObjectID;

        var dialog = $rootScope.showDialog(newScope, "Object Tree", "Entity/views/ObjTree/ObjectTreeDialog.html", {
            width: 500, height: 600
        });
        newScope.onOkObjectTree = function () {
            if (newScope.objBusinessObjectTree.lstmultipleselectedfields != undefined) {
                $rootScope.IsLoading = true;
                $timeout(function () {
                    var lst = newScope.objBusinessObjectTree.lstmultipleselectedfields;
                    //var lst = newScope.objBusinessObjectTree.lstmultipleselectedfields.filter(function (x) {
                    //    return x.IsChecked
                    //})
                    $rootScope.UndRedoBulkOp("Start");
                    angular.forEach(lst, function (obj) {

                        var strId = makeStringToCamelCase(obj.ShortName);
                        strId = newScope.GetValidID(strId);
                        var dataType = getDataType(obj.DataType, obj.DataTypeName);
                        var lstrRelatedBusName = "";
                        var lstrRelatedEntity = "";
                        var type = "Property";

                        if (dataType == "Collection" || dataType == "CDOCollection" || dataType == "List") {
                            strId = "lst" + strId;
                            type = dataType;
                            dataType = "";
                            lstrRelatedBusName = obj.ItemType.Name;
                        }
                        else if (dataType == "Object") {
                            strId = "obj" + strId;
                            type = dataType;
                            dataType = "";
                            lstrRelatedBusName = obj.ItemType.Name;
                        }

                        if (lstrRelatedBusName) {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

                            for (var i = 0; i < entityIntellisenseList.length; i++) {
                                if (entityIntellisenseList[i].BusinessObjectName == lstrRelatedBusName) {
                                    lstrRelatedEntity = entityIntellisenseList[i].ID;
                                    break;
                                }
                            }
                        }

                        var itemPath = "";
                        var DisplayBusObject = getDisplayedEntity(newScope.objBusinessObjectTree.lstdisplaybusinessobject);
                        var strDisplayName = DisplayBusObject.strDisplayName;
                        if (strDisplayName) {
                            itemPath = strDisplayName + "." + obj.ShortName;
                        } else {
                            itemPath = obj.ShortName;
                        }
                        var objField = {
                            Name: "attribute", Value: '', dictAttributes: { ID: strId, sfwDataType: dataType, sfwValue: itemPath, sfwType: type, sfwEntity: lstrRelatedEntity }, Elements: []
                        };
                        var InsertFlag = true;
                        if ($scope.objAttributes && $scope.objAttributes.Elements.length > 0) {
                            for (var i = 0; i < $scope.objAttributes.Elements.length; i++) {
                                if ($scope.objAttributes.Elements[i].dictAttributes.ID != undefined && $scope.objAttributes.Elements[i].dictAttributes.ID.toLowerCase() == strId.toLowerCase()) {
                                    InsertFlag = false;
                                }
                            }
                        }
                        if (InsertFlag) {
                            $rootScope.PushItem(objField, $scope.objAttributes.Elements);
                            var newObj = $scope.objAttributes.Elements[$scope.objAttributes.Elements.length - 1];
                            $scope.validateId(newObj, newObj.dictAttributes.ID);
                            validateEntity(newObj);
                            $scope.checkObjectFieldValue();
                        }

                    });
                    $rootScope.UndRedoBulkOp("End");
                    newScope.onCancelObjectTreeclick();
                    $rootScope.IsLoading = false;
                }, 10);
            }
            else {
                $rootScope.IsLoading = false;
                newScope.onCancelObjectTreeclick();
            }
        };

        newScope.onCancelObjectTreeclick = function () {
            dialog.close();
        };

        newScope.GetValidID = function (strId) {
            var index = 1;
            var id = strId;
            var cnt = 0; //$scope.MirrorElements.Where(itm => null != itm && !string.IsNullOrWhiteSpace(itm[XMLConstants.ID]) && itm[XMLConstants.ID].ToLower() == strId.ToLower()).Count();
            for (var i = 0; i < $scope.objAttributes.Elements.length; i++) {
                var itm = $scope.objAttributes.Elements[i];
                if (itm.Name == "attribute") {
                    if (itm.dictAttributes.ID && itm.dictAttributes.ID.toLowerCase() == strId.toLowerCase()) {
                        cnt++;
                    }
                }
            }
            while (cnt > 0) {
                id = strId + index;
                cnt = 0;
                for (var i = 0; i < $scope.objAttributes.Elements.length; i++) {
                    var itm = $scope.objAttributes.Elements[i];
                    if (itm.Name == "attribute") {
                        if (itm.dictAttributes.ID && itm.dictAttributes.ID.toLowerCase() == id.toLowerCase()) {
                            cnt++;
                        }
                    }
                }
                index++;
            }

            return id;
        };
    };

    //#endregion

    //#region Validate Delete
    $scope.setSelectedValidateDelete = function (selectedValidateDelete, index) {
        var a = selectedValidateDelete;
        for (var i = 0; i < selectedValidateDelete.Elements.length; i++) {
            if (selectedValidateDelete.Elements[i].Name == "item") {
                if (i == index) {
                    selectedValidateDelete.Elements[i].isSelected = true;
                }
                else {
                    selectedValidateDelete.Elements[i].isSelected = false;
                }
            }
        }
    };
    $scope.canMoveValidateDeleteUp = function (selectedValidateDelete) {
        if (selectedValidateDelete) {
            var selectedItems = selectedValidateDelete.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                return selectedValidateDelete.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) > 0;
            }
        }
        return false;
    };
    $scope.moveValidateDeleteUp = function (selectedValidateDelete) {
        var a = selectedValidateDelete;
        if (selectedValidateDelete) {
            var selectedItems = selectedValidateDelete.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                var index = selectedValidateDelete.Elements.indexOf(selectedItems[0]);
                //selectedValidateDelete.Elements.splice(index, 1);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(selectedItems[0], selectedValidateDelete.Elements);
                while (selectedValidateDelete.Elements[index - 1].Name == "validatedelete")
                    index--;
                $rootScope.InsertItem(selectedItems[0], selectedValidateDelete.Elements, index - 1);
                $rootScope.UndRedoBulkOp("End");
                // selectedValidateDelete.Elements.splice(index - 1, 0, selectedItems[0]);
            }
        }
    };

    $scope.canMoveValidateDeleteDown = function (selectedValidateDelete) {
        if (selectedValidateDelete) {
            var selectedItems = selectedValidateDelete.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                return selectedValidateDelete.Elements.filter(function (x) { return x.Name == "item"; }).indexOf(selectedItems[0]) < selectedValidateDelete.Elements.filter(function (x) { return x.Name == "item"; }).length - 1;
            }
        }
        return false;
    };
    $scope.moveValidateDeleteDown = function (selectedValidateDelete) {
        var a = selectedValidateDelete;

        if (selectedValidateDelete) {
            var selectedItems = selectedValidateDelete.Elements.filter(function (x) {
                return x.isSelected == true;
            });
            if (selectedItems && selectedItems.length > 0) {
                var index = selectedValidateDelete.Elements.indexOf(selectedItems[0]);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(selectedItems[0], selectedValidateDelete.Elements);

                while (selectedValidateDelete.Elements[index].Name == "validatedelete")
                    index++;

                $rootScope.InsertItem(selectedItems[0], selectedValidateDelete.Elements, index + 1);
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    //#endregion

    //#region Rules

    $scope.removeRules = function (filetype) {


        if ($scope.currentFileObj && $scope.currentFileObj.isSelected) {
            var isValidFileToDelete = false;
            if (filetype == "Forms" && ($scope.currentFileObj.FileType == "Maintenance" || $scope.currentFileObj.FileType == "Lookup" || $scope.currentFileObj.FileType == "Wizard" || $scope.currentFileObj.FileType == "UserControl" || $scope.currentFileObj.FileType == "Tooltip")) {
                isValidFileToDelete = true;
            }
            else if (filetype == "HTML" && ($scope.currentFileObj.FileType == "FormLinkMaintenance" || $scope.currentFileObj.FileType == "FormLinkLookup" || $scope.currentFileObj.FileType == "FormLinkWizard")) {
                isValidFileToDelete = true;
            }
            else if (filetype == "Rules" && ($scope.currentFileObj.FileType == "LogicalRule" || $scope.currentFileObj.FileType == "DecisionTable" || $scope.currentFileObj.FileType == "ExcelMatrix")) {
                isValidFileToDelete = true;
            }
            else if (filetype == "Correspondence" && $scope.currentFileObj.FileType == "Correspondence") {
                isValidFileToDelete = true;
            }
            if (isValidFileToDelete == true) {
                var newScope = $scope.$new();
                newScope.strMessage = "Are you sure you want to delete this file ";
                newScope.FileName = $scope.currentFileObj.FileName;
                var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                    width: 500, height: 170
                });

                newScope.OkClick = function () {
                    var promiseDeleteFile = $rootScope.DeleteFile($scope.currentFileObj);
                    promiseDeleteFile.done(function (isDelete) {
                        $rootScope.deleteFileSucessCallback($scope.currentFileObj, isDelete);
                        if (isDelete) {
                            if (filetype == "Forms") {
                                var index = $scope.lstItemsFormsList.indexOf($scope.currentFileObj);
                                if (index > -1) {
                                    $scope.lstItemsFormsList.splice(index, 1);
                                    $scope.currentFileObj = undefined;
                                    //else {
                                    //    if (index < $scope.lstItemsFormsList.length) {
                                    //        $scope.onClickOnItems($scope.lstItemsFormsList[index]);
                                    //    }
                                    //    else if ($scope.lstItemsFormsList.length > 0) {
                                    //        $scope.onClickOnItems($scope.lstItemsFormsList[index - 1]);
                                    //    }
                                    //}
                                }
                            }
                            else if (filetype == "HTML") {
                                var index = $scope.lstItemsHtxList.indexOf($scope.currentFileObj);
                                if (index > -1) {
                                    $scope.lstItemsHtxList.splice(index, 1);
                                    $scope.currentFileObj = undefined;
                                    //else {
                                    //    if (index < $scope.lstItemsHtxList.length) {
                                    //        $scope.onClickOnItems($scope.lstItemsHtxList[index]);
                                    //    }
                                    //    else if ($scope.lstItemsHtxList.length > 0) {
                                    //        $scope.onClickOnItems($scope.lstItemsHtxList[index - 1]);
                                    //    }
                                    //}
                                }

                            }
                            else if (filetype == "Rules") {
                                var index = $scope.lstItemsRulesList.indexOf($scope.currentFileObj);
                                if (index > -1) {
                                    $scope.lstItemsRulesList.splice(index, 1);
                                    $scope.currentFileObj = undefined;
                                    //else {
                                    //    if (index < $scope.lstItemsRulesList.length) {
                                    //        $scope.onClickOnItems($scope.lstItemsRulesList[index]);
                                    //    }
                                    //    else if ($scope.lstItemsRulesList.length > 0) {
                                    //        $scope.onClickOnItems($scope.lstItemsRulesList[index - 1]);
                                    //    }
                                    //}
                                }
                            }
                            else if (filetype == "Correspondence") {
                                var index = $scope.lstItemsCorrList.indexOf($scope.currentFileObj);
                                if (index > -1) {
                                    $scope.lstItemsCorrList.splice(index, 1);
                                    $scope.currentFileObj = undefined;
                                    //else {
                                    //    if (index < $scope.lstItemsCorrList.length) {
                                    //        $scope.onClickOnItems($scope.lstItemsCorrList[index]);
                                    //    }
                                    //    else if ($scope.lstItemsCorrList.length > 0) {
                                    //        $scope.onClickOnItems($scope.lstItemsCorrList[index - 1]);
                                    //    }
                                    //}
                                }

                            }
                        }
                    });
                    dialog.close();
                };
                newScope.closeDialog = function () {
                    dialog.close();
                };
            }
            else {
                alert("Please Select file to delete.");
            }
        }
    };


    $scope.canDeleteRules = function () {
        if ($scope.lstEntityRules.length > 0) {
            return $scope.selectedEntityRule != undefined && $scope.selectedEntityRule > -1;
        }
        else {
            return false;
        }
    };
    $scope.onOpenRuleFile = function (obj) {

        if (obj != null) {
            $.connection.hubMain.server.navigateToFile(obj.FileName, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
    };


    $scope.SearchRuleCommand = function () {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var lst = entityIntellisenseList.filter(function (x) {
            return x.ID == $scope.objEntity.dictAttributes.ID;
        });
        var currentEntity;
        if (lst && lst.length > 0) {
            currentEntity = lst[0];
        }
        if (currentEntity) {
            var selectedRuleTypes = [];
            if ($scope.objRuleSearch.IsLogicalRuleChecked) {
                selectedRuleTypes.push("logicalrule");
            }
            if ($scope.objRuleSearch.IsDecisionTableChecked) {
                selectedRuleTypes.push("decisiontable");
            }
            if ($scope.objRuleSearch.IsExcelMatrixChecked) {
                selectedRuleTypes.push("excelmatrix");
            }

            var filteredRules = currentEntity.Rules.filter(function (x) {
                return (selectedRuleTypes.length == 0 || selectedRuleTypes.indexOf(x.RuleType.toLowerCase()) > -1) &&
                    ($scope.objRuleSearch.strSearchRulesID == undefined || $scope.objRuleSearch.strSearchRulesID.trim().length == 0 || x.ID.toLowerCase().indexOf($scope.objRuleSearch.strSearchRulesID.toLowerCase()) > -1) &&
                    ($scope.objRuleSearch.SelectedRuleStatus == undefined || $scope.objRuleSearch.SelectedRuleStatus.trim().length == 0 || x.Status == $scope.objRuleSearch.SelectedRuleStatus) &&
                    ($scope.objRuleSearch.SelectedReturnType == undefined || $scope.objRuleSearch.SelectedReturnType.trim().length == 0 || x.ReturnType == $scope.objRuleSearch.SelectedReturnType) &&
                    ($scope.objRuleSearch.IsRuleStaticSelected == undefined || $scope.objRuleSearch.IsRuleStaticSelected == false || x.IsStatic == $scope.objRuleSearch.IsRuleStaticSelected) &&
                    ($scope.objRuleSearch.IsRulePrivateSelected == undefined || $scope.objRuleSearch.IsRulePrivateSelected == false || x.IsPrivate == $scope.objRuleSearch.IsRulePrivateSelected);
            });

            angular.forEach($scope.lstEntityRules, function (rule) {
                rule.IsVisible = filteredRules.some(function (x) {
                    return x.ID == rule.ID;
                });
            });
        }
    };


    $scope.ClearCommand = function () {
        $scope.objRuleSearch.strSearchRulesID = undefined;
        $scope.objRuleSearch.SelectedRuleStatus = undefined;
        $scope.objRuleSearch.SelectedReturnType = undefined;
        $scope.objRuleSearch.IsLogicalRuleChecked = undefined;
        $scope.objRuleSearch.IsDecisionTableChecked = undefined;
        $scope.objRuleSearch.IsExcelMatrixChecked = undefined;
        $scope.objRuleSearch.IsRuleStaticSelected = undefined;
        $scope.objRuleSearch.IsRulePrivateSelected = undefined;
        angular.forEach($scope.lstEntityRules, function (rule) {
            rule.IsVisible = true;
        });
    };

    $scope.receieveFilterRuleData = function (data) {
        $scope.lstEntityRules = [];
        $scope.$apply(function () {

            $scope.lstEntityRules = data;
            if ($scope.lstEntityRules.length > 0) {
                $scope.selectEntityRules(0);
            }

        });
    };
    $scope.selectEntityRules = function (index) {
        $scope.selectedEntityRule = index;
    };
    //#endregion

    //#region Add Expression from Query,businessObject and Methods

    $scope.onQueryClick = function (obj) {

        var newScope = $scope.$new();
        newScope.onOkclick = function () {
            if ($scope.SelectedView == "ValidationRules") {
                if ($scope.SelectedRule != undefined) {
                    $scope.SelectedRule.dictAttributes.sfwExpression = newScope.GetExpression();
                }
            }
            //else if ($scope.SelectedView == "Expressions") {
            //    if ($scope.SelectedExp != undefined) {
            //        $scope.SelectedExp.dictAttributes.sfwExpression = newScope.GetExpression();
            //    }
            //}

            newScope.onCancelclick();
        };

        newScope.selectQueryParam = function (param) {
            newScope.SelectedQuery.SelectedParameter = param;
        };

        newScope.onCancelclick = function () {
            ngDialog.close(newScope.queryDialog.id);
        };

        newScope.onQueryClick = function () {
            $scope.onQueryDialogClick(newScope);
        };

        newScope.GetExpression = function () {
            return $scope.getExpressionForQuery(newScope);
        };

        newScope.onQuerySelectionChange = function (obj, event) {
            newScope.SelectedQuery = obj;
            event.stopPropagation();
        };

        newScope.ExpandCollapsedQuery = function (query, $event) {
            query.IsExpanded = !query.IsExpanded;
            expandCollapseQuery($event);
        };


        newScope.onQueryDialogClick = function () {

            if (newScope.SelectedQuery.SelectedParameter != undefined) {
                var newQueryScope = newScope.$new();
                newQueryScope.lstEntityQueryList = newScope.lstEntityQueryList;
                newQueryScope.onOkclick = function () {
                    newScope.SelectedQuery.SelectedParameter.Expression = newQueryScope.GetExpression();
                    newQueryScope.onCancelclick();
                };

                newQueryScope.GetExpression = function () {
                    return $scope.getExpressionForQuery(newQueryScope);
                };

                newQueryScope.selectQueryParam = function (param) {
                    newQueryScope.SelectedQuery.SelectedParameter = param;
                };

                newQueryScope.onQuerySelectionChange = function (obj, event) {
                    newQueryScope.SelectedQuery = obj;
                    event.stopPropagation();
                };

                newQueryScope.onCancelclick = function () {
                    ngDialog.close(newQueryScope.queryDialog.id);
                };

                newQueryScope.ExpandCollapsedQuery = function (query, $event) {
                    query.IsExpanded = !query.IsExpanded;
                    expandCollapseQuery($event);
                };

                newQueryScope.queryDialog = ngDialog.open({
                    template: 'querySelectionWindowTemplate',
                    scope: newQueryScope,
                    closeByDocument: false,
                    className: 'ngdialog-theme-default ngdialog-theme-custom',

                });

            }
        };

        newScope.PopulateDataTypes = function () {
            newScope.DataTypes = [];
            newScope.DataTypes.push("string");
            newScope.DataTypes.push("bool");
            newScope.DataTypes.push("decimal");
            newScope.DataTypes.push("double");
            newScope.DataTypes.push("float");
            newScope.DataTypes.push("int");
            newScope.DataTypes.push("long");
            newScope.DataTypes.push("short");
        };

        newScope.Load = function () {

            $.connection.hubEntityModel.server.getEntityQueryList().done(function (data) {
                newScope.lstEntityQueryList = data;

                newScope.queryDialog = ngDialog.open({
                    template: 'querySelectionWindowTemplate',
                    scope: newScope,
                    closeByDocument: false,
                    className: 'ngdialog-theme-default ngdialog-theme-custom',

                });
            });



            newScope.PopulateDataTypes();
        };

        newScope.Load();
    };

    $scope.getExpressionForQuery = function (scope) {
        var expression = "RFunc.ExecuteQuery";
        if (scope.SelectedQuery.StrGenericType != undefined && scope.SelectedQuery.StrGenericType != "") {
            expression = expression + "<" + scope.SelectedQuery.StrGenericType + ">";
        }

        expression = expression + "(";

        expression = expression + '"' + scope.SelectedQuery.cdoId + "." + scope.SelectedQuery.Name + '"';

        angular.forEach(scope.SelectedQuery.Parameters, function (methodPar) {

            if (methodPar.IsStringDataType && methodPar.IsConstant) {
                expression = expression + ',"' + methodPar.Expression + '"';
            }
            else {
                expression = expression + "," + methodPar.Expression;
            }
        });

        expression = expression + ")";

        return expression;
    };


    $scope.dashboardFilters = {
        isAttributesFilterVisible: false,
        attributesFilterText: "",
        isFieldConstraintsFilterVisible: false,
        fieldConstraintsFilterText: "",
        isBusinessObjectFilterVisible: false,
        businessObjectFilterText: "",
        isRelationshipsFilterVisible: false,
        relationshipsFilterText: "",
        isExpressionsFilterVisible: false,
        expressionsFilterText: "",
        isColumnsFilterVisible: false,
        columnsFilterText: "",
        isValidationRulesFilterVisible: false,
        validationRulesFilterText: "",
        isRulesFilterVisible: false,
        rulesFilterText: "",
        isQueriesFilterVisible: false,
        queriesFilterText: "",
        isInitialLoadFilterVisible: false,
        initialLoadFilterText: "",
        isHardErrorsFilterVisible: false,
        hardErrorsFilterText: "",
        isSoftErrorsFilterVisible: false,
        softErrorsFilterText: "",
        isValidateDeleteFilterVisible: false,
        validateDeleteFilterText: "",
        isGroupsFilterVisible: false,
        groupsFilterText: "",
        isCheckListFilterVisible: false,
        checkListFilterText: "",
        isInitialLoadFilterVisible: false,
        searchInitialLoadRuleByID: "",
        isHardErrorFilterVisible: false,
        searchHardErrorsRuleByID: "",
        isSoftErrorFilterVisible: false,
        searchSoftErrorsRuleByID: "",
        isValidateDeleteFilterVisible: false,
        searchValidateDeleteRuleByID: "",
        isGroupsFilterVisible: false,
        searchGroupRuleByID: "",
        toggleDashboardFilter: function (para) {
            if (para == 'FieldConstraints') {
                $scope.dashboardFilters.isFieldConstraintsFilterVisible = !$scope.dashboardFilters.isFieldConstraintsFilterVisible;
                $scope.dashboardFilters.fieldConstraintsFilterText = "";
            }
            if (para == 'BusinessObject') {
                $scope.dashboardFilters.isBusinessObjectFilterVisible = !$scope.dashboardFilters.isBusinessObjectFilterVisible;
                $scope.dashboardFilters.businessObjectFilterText = "";
            }
            if (para == "Attributes") {
                $scope.dashboardFilters.isAttributesFilterVisible = !$scope.dashboardFilters.isAttributesFilterVisible;
                $scope.dashboardFilters.attributesFilterText = "";
            }
            if (para == "Relationship") {
                $scope.dashboardFilters.isRelationshipsFilterVisible = !$scope.dashboardFilters.isRelationshipsFilterVisible;
                $scope.dashboardFilters.relationshipsFilterText = "";
            }
            if (para == "Expressions") {
                $scope.dashboardFilters.isExpressionsFilterVisible = !$scope.dashboardFilters.isExpressionsFilterVisible;
                $scope.dashboardFilters.expressionsFilterText = "";
            }

            if (para == "ValidationRules") {
                $scope.dashboardFilters.isValidationRulesFilterVisible = !$scope.dashboardFilters.isValidationRulesFilterVisible;
                $scope.dashboardFilters.validationRulesFilterText = "";
            }
            if (para == "Rules") {
                $scope.dashboardFilters.isRulesFilterVisible = !$scope.dashboardFilters.isRulesFilterVisible;
                $scope.dashboardFilters.rulesFilterText = "";
            }
            if (para == "Queries") {
                $scope.dashboardFilters.isQueriesFilterVisible = !$scope.dashboardFilters.isQueriesFilterVisible;
                $scope.dashboardFilters.queriesFilterText = "";
            }
            if (para == "InitialLoad") {
                $scope.dashboardFilters.isInitialLoadFilterVisible = !$scope.dashboardFilters.isInitialLoadFilterVisible;
                $scope.dashboardFilters.initialLoadFilterText = "";
            }
            if (para == "HardErrors") {
                $scope.dashboardFilters.isHardErrorsFilterVisible = !$scope.dashboardFilters.isHardErrorsFilterVisible;
                $scope.dashboardFilters.hardErrorsFilterText = "";
            }
            if (para == "SoftErrors") {
                $scope.dashboardFilters.isSoftErrorsFilterVisible = !$scope.dashboardFilters.isSoftErrorsFilterVisible;
                $scope.dashboardFilters.softErrorsFilterText = "";
            }
            if (para == "ValidateDelete") {
                $scope.dashboardFilters.isValidateDeleteFilterVisible = !$scope.dashboardFilters.isValidateDeleteFilterVisible;
                $scope.dashboardFilters.validateDeleteFilterText = "";
            }
            if (para == "Groups") {
                $scope.dashboardFilters.isGroupsFilterVisible = !$scope.dashboardFilters.isGroupsFilterVisible;
                $scope.dashboardFilters.groupsFilterText = "";
            }
            if (para == "CheckList") {
                $scope.dashboardFilters.isCheckListFilterVisible = !$scope.dashboardFilters.isCheckListFilterVisible;
                $scope.dashboardFilters.checkListFilterText = "";
            }
        }
    };
    //#endregion

    $scope.openEntityClick = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, "", "");
        }
    };
    $scope.selectRuleClick = function (aRuleID) {
        if (aRuleID && aRuleID != "") {
            $NavigateToFileService.NavigateToFile($rootScope.currentopenfile.file.FileName, "rules", aRuleID);
        }
    };

    //#region Validate Object

    $scope.ValidateRulesandExpressions = function () {
        $.connection.hubEntityModel.server.validateExpressions($scope.currentfile.FileName, true).done(function (data) {

            angular.forEach(data, function (value, key) {

                var lst = $scope.objAttributes.Elements.filter(function (itm) {
                    if (itm.dictAttributes.sfwType == 'Expression') {
                        return itm.dictAttributes.ID == key;
                    }
                });
                if (lst && lst.length > 0) {
                    $scope.$evalAsync(function () {
                        lst[0].Error = value;
                    });
                }
            });
        });

        $.connection.hubEntityModel.server.validateExpressions($scope.currentfile.FileName, false).done(function (data) {

            angular.forEach(data, function (value, key) {

                var lst = $scope.objRules.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID == key;
                });
                if (lst && lst.length > 0) {
                    $scope.$evalAsync(function () {
                        lst[0].Error = value;
                    });
                }
            });
        });

    };

    $scope.ValidateObject = function () {
        $scope.ValidateRulesandExpressions();
    };
    //#endregion

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objEntityExtraFields) {
            var index = $scope.objEntity.Elements.indexOf($scope.objEntityExtraFields);
            if (index == -1) {
                $scope.objEntity.Elements.push($scope.objEntityExtraFields);
            }
        }
    };

    $scope.onClickOnItems = function (item) {
        $scope.currentFileObj = item;
        if ($scope.currentFileObj && !$scope.currentFileObj.isSelected) {
            $scope.currentFileObj.isSelected = true;
            if ($scope.previousFileObj) {
                $scope.previousFileObj.isSelected = false;
            }
            $scope.previousFileObj = $scope.currentFileObj;
        }
    };

    // #region validation
    $scope.validateFileData = function (showMessage) {
        if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
            var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName });
            if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: $scope.currentfile.FileName, errorList: [] });
            //$scope.validateFileData();
            var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];

            $scope.validationErrorList = fileErrObj.errorList = [];
            $scope.warningList = fileErrObj.warningList = [];
        }
        //var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];

        //$scope.validationErrorList = fileErrObj.errorList = [];
        //$scope.warningList = fileErrObj.warningList = [];

        /** attribute validation **/
        angular.forEach($scope.objAttributes.Elements, function (obj) {
            $scope.validateId(obj, undefined, true);
            $scope.validateDataType(obj, undefined, true);
            validateEntity(obj);
        });
        /** Query validation **/
        angular.forEach($scope.objQueries.Elements, function (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
            if (obj.Elements.length > 0) {
                var customMapping = $filter('filter')(obj.Elements, { Name: "mappedcolumns" })[0];
                if (customMapping && customMapping.Elements.length > 0) {
                    angular.forEach(customMapping.Elements, function (columnObj) {
                        if (columnObj && columnObj.dictAttributes.hasOwnProperty("sfwEntityField") && columnObj.dictAttributes.sfwEntityField) {
                            var attrType = "";
                            $ValidationService.checkValidListValueForMultilevel([], columnObj, columnObj.dictAttributes.sfwEntityField, $scope.objEntity.dictAttributes.ID, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
                        }
                    });
                }
            }
        });
        /** Checking dupplicate ID in entire entity excluding Methods **/
        $scope.checkDuplicateId(false);
        /** Methods validation **/
        validateQueries(false);
        validateValidationRulesID(false);
        angular.forEach($scope.objMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
        });

        /** Object Methods Validation **/
        if ($scope.objEntity.dictAttributes.sfwObjectID) {
            $.connection.hubEntityModel.server.loadObjectTree($scope.objEntity.dictAttributes.sfwObjectID, "", true).done(function (data) {
                $scope.$evalAsync(function () {
                    var objectData = JSON.parse(data);
                    var methodList = objectData.lstMethods && $ValidationService.getListByPropertyName(objectData.lstMethods, "ShortName");
                    validateBusinessObjectMethod(methodList);
                });
            });
        } else if ($scope.blnShowCreateWithClass) {
            validateBusinessObjectMethod([]);
        }
        angular.forEach($scope.objHardError.Elements, function (obj) {
            if (obj.objRule) {
                $ValidationService.validateIsEmptyMessageId(obj.objRule, $scope.validationErrorList, obj.objRule.dictAttributes.sfwMessageId);
                $ValidationService.validateIsEmptyMessageIdAndDescription(obj.objRule, $scope.validationErrorList, obj.objRule.dictAttributes.sfwMessageId, obj.objRule.dictAttributes.sfwMessage);
            }
        });
        angular.forEach($scope.objSoftError.Elements, function (obj) {
            if (obj.objRule) {
                $ValidationService.validateIsEmptyMessageId(obj.objRule, $scope.validationErrorList, obj.objRule.dictAttributes.sfwMessageId);
                $ValidationService.validateIsEmptyMessageIdAndDescription(obj.objRule, $scope.validationErrorList, obj.objRule.dictAttributes.sfwMessageId, obj.objRule.dictAttributes.sfwMessage);
            }
        });
        /* Validate Delete Method section */
        angular.forEach($scope.objDelete.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("ID") && obj.dictAttributes.ID) {
                var attrType = 'Object,Collection,List';
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.ID, $scope.objEntity.dictAttributes.ID, "ID", "ID", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwMethodName") && obj.dictAttributes.sfwMethodName) {
                var attrType = 'Property,Collection,Object';
                $ValidationService.checkValidListValueForXMLMethod([], obj, obj.dictAttributes.sfwMethodName, $scope.objEntity.dictAttributes.ID, "sfwMethodName", "invalid_method_name", CONST.VALIDATION.INVALID_NAME, $scope.validationErrorList, attrType, null, true);
            }
        });
        /* validate code id in column attributes */
        if (!$scope.codegrouplist) {
            $.connection.hubMain.server.getCodeGroups().done(function (data) {
                $scope.codegrouplist = data;
                var list = $ValidationService.getListByPropertyName($scope.codegrouplist, "CodeID");
                validateCodeGroup(list);
            });
        } else {
            var list = $ValidationService.getListByPropertyName($scope.codegrouplist, "CodeID");
            validateCodeGroup(list);
        }

        /** Checking duplicate ID of methods for object and XML methods **/
        checkDuplicateIdForMethod(false);

        $scope.validateCSProperties();
        // check if inherited validation rule used in initial load,hard error, soft error,validate delete,group rules,check list and is it present in parent entity rules

        if ($scope.objEntity.dictAttributes.sfwParentEntity && showMessage) {
            hubMain.server.getInheritedRule($scope.objEntity.dictAttributes.ID, $scope.objEntity.dictAttributes.sfwParentEntity).done(function (data) {
                if (data) {
                    $scope.$evalAsync(function () {
                        validateValidationRulesID(data, true);
                        $scope.checkValidationRules(data.Elements);
                    });
                }
            });
        } else {
            var list = [];
            if ($scope.objEntity.dictAttributes.sfwParentEntity && $scope.objInheritedRules && $scope.objInheritedRules.Elements) {
                list = $scope.objInheritedRules.Elements;
            }
            $scope.checkValidationRules(list);
        }
        $scope.checkObjectFieldValue();
        validateMessageIds();
        if (showMessage) {
            $scope.ValidateObject();
            validateXMLMethod();
            validateRuleMessageId();
            $scope.$evalAsync(function () {
                toastr.info("Entity validated successfully");
            });
        }

        //below function calls are to validate parent key value if it's empty and error table is set.
        $scope.validateParentKeyValue();

        //below function calls are to validate status value if it's empty and error table is set.
        $scope.validateStatusValue();
    };

    var validateMessageIds = function () {
        var list = $ValidationService.getListByPropertyName($scope.lstMessages, "MessageID");
        if ($scope.objConstraints && $scope.objConstraints.Elements.length > 0) {
            angular.forEach($scope.objConstraints.Elements, function (constraint) {
                if (constraint && constraint.dictAttributes.sfwMaxValue && constraint.dictAttributes.sfwMaxMessageId) {
                    $ValidationService.checkValidListValue(list, constraint, constraint.dictAttributes.sfwMaxMessageId, "sfwMaxMessageId", "sfwMaxMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, $scope.validationErrorList, false);
                }
                if (constraint && constraint.dictAttributes.sfwMinValue && constraint.dictAttributes.sfwMinMessageId) {
                    $ValidationService.checkValidListValue(list, constraint, constraint.dictAttributes.sfwMinMessageId, "sfwMinMessageId", "sfwMinMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, $scope.validationErrorList, false);
                }
            });
        }

    };
    var validateBusinessObjectMethod = function (methodList) {
        angular.forEach($scope.objObjectMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
            $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.ID, "ID", "inValid_id", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList);
        });
    };
    $scope.checkObjectFieldValue = function () {
        var attrType = ["Object", "CDOCollection", "Collection", "List", "Property"];
        var attributeModel = getAttributesList();
        angular.forEach($scope.objAttributes.Elements, function (aobj) {
            if (angular.isArray(attrType) && attrType.indexOf(aobj.dictAttributes.sfwType) > -1) {
                if (aobj && aobj.warnings) {
                    delete aobj.warnings["sfwValue"];
                }
                $ValidationService.checkDuplicateValue(aobj, attributeModel, $scope.warningList, "sfwValue", "sfwValue", true, attrType);
            }
        });
    }
    $scope.checkValidationRules = function (parentEntityRules) {
        var ruleList = [];
        ruleList = ruleList.concat($ValidationService.getListByPropertyName($scope.objRules.Elements, "ID", false));
        ruleList = ruleList.concat($ValidationService.getListByPropertyName(parentEntityRules, "ID", false));
        // validate initial load
        $scope.validateRules($scope.objInitialLoad.Elements, ruleList);

        // validate check list
        $scope.validateRules($scope.objCheckList.Elements, ruleList);
    };
    $scope.validateRules = function (objList, ruleList) {
        angular.forEach(objList, function (obj) {
            $ValidationService.checkValidListValue(ruleList, obj, obj.dictAttributes.ID, "ID", "invalid_rule", obj.dictAttributes.ID + " - " + CONST.VALIDATION.INVALID_RULE, $scope.validationErrorList);

            if (angular.isArray(obj.Children) && obj.Children.length > 0) {
                $scope.validateRules(obj.Children, ruleList);
            }
        });
    };

    var validateXMLMethod = function () {
        angular.forEach($scope.objMethods.Elements, function (obj) {
            for (var i = 0; i < obj.Elements.length; i++) {
                if (obj.Elements[i].Name == "item") {
                    var attrType = 'Column,Property,Collection,Object';
                    $ValidationService.checkValidListValueForXMLMethod($scope.lstBusBaseMethods, obj.Elements[i], obj.Elements[i].dictAttributes.ID, $scope.objEntity.dictAttributes.ID, "ID", "invalid_method_name", CONST.VALIDATION.INVALID_NAME, $scope.validationErrorList, attrType, null, false);
                }
            }
        });
    };
    var validateCodeGroup = function (list) {
        angular.forEach($scope.objAttributes.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("sfwCodeID") && obj.dictAttributes.sfwCodeID) {
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwCodeID, "sfwCodeID", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, $scope.validationErrorList);
            }
        });
    };

    $scope.validateId = function (obj, ID, isNotFirstLoad) {
        if (obj) {
            if (isNotFirstLoad) {
                $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
            } else {
                $ValidationService.validateID(obj, $scope.validationErrorList, ID);
            }
        }
    };
    $scope.validateIsEmptyMessageId = function (obj, ID, isNotFirstLoad) {
        if (obj) {
            if (isNotFirstLoad) {
                $ValidationService.validateIsEmptyMessageId(obj, $scope.validationErrorList, obj.dictAttributes.sfwMessageId);
            } else {
                $ValidationService.validateIsEmptyMessageId(obj, $scope.validationErrorList, ID);
            }
        }
    };
    $scope.checkDuplicateObj = function checkDuplicateObj(item, olditem, isUpdate) {
        var attributeModel = getAttributesList();
        var currentItemIndex = attributeModel.Elements.indexOf(olditem);
        if (isUpdate) {
            attributeModel.Elements.splice(currentItemIndex, 1);
        }
        var objDuplicate = $ValidationService.findDuplicateId(attributeModel, item.dictAttributes.ID, CONST.ENTITY.NODES, "description", item, "dictAttributes.ID");
        if (objDuplicate) {
            var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
            if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                errorMessage = CONST.VALIDATION.DUPLICATE_ID_IN_PARENT_ENTITY;
            }
            item.errors.duplicate_id = errorMessage;
        }
        else {
            if (item.errors) {
                delete item.errors.duplicate_id;
            }
        }
    };

    function getAttributesList() {
        var attributeModel = { Elements: [] };
        attributeModel.Elements = $scope.objAttributes.Elements && $scope.objAttributes.Elements.slice();
        if ($scope.objEntity.dictAttributes.sfwParentEntity) {
            var data = $Entityintellisenseservice.GetIntellisenseData($scope.objEntity.dictAttributes.sfwParentEntity, "", "", true, true, false, false, false, false);
            if (data && data.length > 0) {
                attributeModel.Elements = attributeModel.Elements.concat(convertToBaseModel(data));
            }
        }
        return attributeModel;
    }

    $scope.checkDuplicateId = function (isNotFirstLoad) {
        var attributeModel = getAttributesList();
        angular.forEach($scope.objAttributes.Elements, function (obj) {
            if (obj.dictAttributes.sfwType !== "Description") {
                $ValidationService.checkDuplicateId(obj, attributeModel, $scope.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES, $scope.warningList, $rootScope.iblnShowEntityDuplicateIdAsWarning, true);
            }
        });
    };

    var validateQueries = function (isNotFirstLoad) {
        angular.forEach($scope.objQueries.Elements, function (obj) {
            $ValidationService.checkDuplicateId(obj, $scope.objQueries, $scope.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES);
        });
    };

    var validateValidationRulesID = function (isNotFirstLoad) {
        var objParentRules = null;
        if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
            objParentRules = $scope.objInheritedRules;
        }
        var ruleModel = getRuleModel(objParentRules);
        if ($scope.objRules) {
            angular.forEach($scope.objRules.Elements, function (obj) {
                if (obj.Name == "rule") { // only validating type of Rules
                    $ValidationService.checkDuplicateId(obj, ruleModel, $scope.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES);
                }
            });
        }
    };
    var validateRuleMessageId = function () {
        if ($scope.objRules) {
            angular.forEach($scope.objRules.Elements, function (obj) {
                if (obj.Name == "rule") { // only validating type of Rules
                    var objHardError = $scope.objHardError.Elements.filter(function (hardErrorObj) {
                        if (hardErrorObj && hardErrorObj.dictAttributes.ID == obj.dictAttributes.ID) {
                            return hardErrorObj;
                        }
                    })
                    if (objHardError && objHardError.length) {
                        $scope.validateEmptyMessageID(obj);
                    }
                }
            });
        }
    };
    var getRuleModel = function (inheritedRules) {
        var ruleModel = { Elements: [] };
        var lstRules = [];
        ruleModel.Elements = $scope.objRules.Elements && $scope.objRules.Elements.slice();
        if ($scope.objEntity.dictAttributes.sfwParentEntity && inheritedRules && inheritedRules.Elements) {
            angular.forEach(inheritedRules.Elements, function (rule) {
                if (rule) {
                    var tempRule = {};
                    tempRule = angular.copy(rule, tempRule);
                    tempRule.isParent = true; // added property for identify parent entity rule
                    lstRules.push(tempRule);
                }
            });
            // lstRules = $scope.objInheritedRules.Elements && $scope.objInheritedRules.Elements.slice();
            ruleModel.Elements = ruleModel.Elements.concat(lstRules);
        }
        return ruleModel;
    };
    var getMethodsModel = function (methodType) {
        var xmlMethodModel = { Elements: [] };
        var lstXmlMethods = [];
        var isXmlMethod = true;
        var isObjMethod = true;
        if (methodType == "xmlmethod") {
            xmlMethodModel.Elements = $scope.objMethods.Elements.slice();
            isObjMethod = false;
        } else {
            xmlMethodModel.Elements = $scope.objObjectMethods.Elements.slice();
            isXmlMethod = false;
        }

        if ($scope.objEntity.dictAttributes.sfwParentEntity) {
            var xmlMethodData = $Entityintellisenseservice.GetIntellisenseData($scope.objEntity.dictAttributes.sfwParentEntity, "", "", true, false, isObjMethod, false, false, isXmlMethod);
            if (xmlMethodData && xmlMethodData.length > 0) {
                lstXmlMethods = convertToBaseModel(xmlMethodData);
                xmlMethodModel.Elements = xmlMethodModel.Elements.concat(lstXmlMethods);
            }
        }
        return xmlMethodModel;

    };
    var convertToBaseModel = function (list) {
        var listObj = [];
        angular.forEach(list, function (obj) {
            if (obj && obj.ID) {
                var model = { dictAttributes: {}, Elements: [] };
                model.dictAttributes.ID = obj.ID;
                model.Name = obj.Type;
                model.isParent = true; // added property for identify parent entity attritbutes,methods
                if (obj.hasOwnProperty("Value") && obj.Value) {
                    model.dictAttributes.sfwValue = obj.Value;
                }
                if (obj.hasOwnProperty("Type") && obj.Type) {
                    model.dictAttributes.sfwType = obj.Type;
                }
                listObj.push(model);
            }
        });
        return listObj;
    };

    var checkDuplicateIdForMethod = function (isNotFirstLoad) {
        var xmlMethodModel, objMethodModel;
        xmlMethodModel = getMethodsModel("xmlmethod");
        angular.forEach($scope.objMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
            $ValidationService.checkDuplicateId(obj, xmlMethodModel, $scope.validationErrorList, isNotFirstLoad, CONST.ENTITY.METHOD_NODES);
        });
        if ($scope.blnShowCreateWithClass) {
            objMethodModel = getMethodsModel("objmethod");
            angular.forEach($scope.objObjectMethods.Elements, function (obj) {
                $ValidationService.checkDuplicateId(obj, objMethodModel, $scope.validationErrorList, isNotFirstLoad, CONST.ENTITY.METHOD_NODES);
            });
        }
    };

    $scope.validateDataType = function (obj, dataType, isFirstLoad) {
        if (obj) {
            if (isFirstLoad) {
                if (obj.dictAttributes.sfwType == "Property") {
                    $ValidationService.validateDataType(obj, $scope.validationErrorList, obj.dictAttributes.sfwDataType);
                }
            } else {
                $ValidationService.validateDataType(obj, $scope.validationErrorList, dataType);
            }
        }
    };

    var validateEntity = function (obj) {
        if (obj && (obj.dictAttributes.sfwType == "Collection" || obj.dictAttributes.sfwType == "CDOCollection" || obj.dictAttributes.sfwType == "Object" || obj.dictAttributes.sfwType == "List")) {
            $ValidationService.validateEntity(obj, $scope.validationErrorList);
        }
    };

    $scope.findParentAndChildObject = function (selectedItem) {
        var findList = [];
        $scope.FindDeepNode($scope.objEntity, selectedItem, findList);
        $scope.$evalAsync(function () {
            var path = $ValidationService.createFullPath($scope.objEntity, findList);
            $scope.selectElement(path);
        });
    };

    // #endregion validation

    //#region Validation Rules Section Move Up and Down Events

    //#region Initial Load Section
    $scope.MoveUpInitialLoad = function () {
        if ($scope.SelectedInitialLoad) {
            var index = $scope.objInitialLoad.Elements.indexOf($scope.SelectedInitialLoad);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedInitialLoad, $scope.objInitialLoad.Elements);
            $rootScope.InsertItem($scope.SelectedInitialLoad, $scope.objInitialLoad.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#entity-validation-initialload-section", ".page-sidebar-content", $scope.SelectedInitialLoad.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveUpInitialLoad = function () {
        if ($scope.SelectedInitialLoad == undefined) {
            return true;
        }
        else {
            var index = $scope.objInitialLoad.Elements.indexOf($scope.SelectedInitialLoad);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };

    $scope.MoveDownInitialLoad = function () {
        if ($scope.SelectedInitialLoad) {
            var index = $scope.objInitialLoad.Elements.indexOf($scope.SelectedInitialLoad);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedInitialLoad, $scope.objInitialLoad.Elements);
            $rootScope.InsertItem($scope.SelectedInitialLoad, $scope.objInitialLoad.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#entity-validation-initialload-section", ".page-sidebar-content", $scope.SelectedInitialLoad.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveDownInitialLoad = function () {
        if ($scope.SelectedInitialLoad == undefined) {
            return true;
        }
        else {
            var index = $scope.objInitialLoad.Elements.indexOf($scope.SelectedInitialLoad);
            if (index != $scope.objInitialLoad.Elements.length - 1 && index != -1) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    //#endregion

    //#region Hard Errors Section
    $scope.MoveUpHardError = function () {
        $scope.MoveUpValidationRule($scope.SelectedHardError, $scope.objHardError, "rule");
        $scope.scrollToPosition("#entity-validation-harderror-section", ".page-sidebar-content", $scope.SelectedHardError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
    };

    $scope.isMoveUpHardError = function () {
        return $scope.isMoveUpValidationRule($scope.SelectedHardError, $scope.objHardError, "rule");
    };

    $scope.MoveDownHardError = function () {
        $scope.MoveDownValidationRule($scope.SelectedHardError, $scope.objHardError, "rule");
        $scope.scrollToPosition("#entity-validation-harderror-section", ".page-sidebar-content", $scope.SelectedHardError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
    };

    $scope.isMoveDownHardError = function () {
        return $scope.isMoveDownValidationRule($scope.SelectedHardError, $scope.objHardError, "rule");
    };
    //#endregion

    $scope.MoveUpValidationRule = function (aobjSelectedRule, aobjParentObject, astrChildName) {
        if (aobjSelectedRule) {
            if (aobjSelectedRule.Name == astrChildName) {
                var lst = aobjParentObject.Elements.filter(function (x) { return aobjSelectedRule.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Children.indexOf(aobjSelectedRule);
                    if (index > 0) {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem(aobjSelectedRule, lst[0].Children);
                        $rootScope.InsertItem(aobjSelectedRule, lst[0].Children, index - 1);
                        $rootScope.UndRedoBulkOp("End");
                    }

                }
            }
            else {
                var index = aobjParentObject.Elements.indexOf(aobjSelectedRule);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(aobjSelectedRule, aobjParentObject.Elements);
                $rootScope.InsertItem(aobjSelectedRule, aobjParentObject.Elements, index - 1);
                $rootScope.UndRedoBulkOp("End");
            }
        }


    };

    $scope.isMoveUpValidationRule = function (aobjSelectedRule, aobjParentObject, astrChildName) {
        if (aobjSelectedRule == undefined) {
            return true;
        }
        else {

            if (aobjSelectedRule.Name == astrChildName) {
                var lst = aobjParentObject.Elements.filter(function (x) { return aobjSelectedRule.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Children.indexOf(aobjSelectedRule);
                    if (index > 0) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                var index = aobjParentObject.Elements.indexOf(aobjSelectedRule);
                if (index > 0) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }

    };

    $scope.MoveDownValidationRule = function (aobjSelectedRule, aobjParentObject, astrChildName) {
        if (aobjSelectedRule) {
            if (aobjSelectedRule.Name == astrChildName) {
                var lst = aobjParentObject.Elements.filter(function (x) { return aobjSelectedRule.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Children.indexOf(aobjSelectedRule);
                    if (index != lst[0].Children.length - 1 && index != -1) {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem(aobjSelectedRule, lst[0].Children);
                        $rootScope.InsertItem(aobjSelectedRule, lst[0].Children, index + 1);
                        $rootScope.UndRedoBulkOp("End");
                    }

                }
            }
            else {
                var index = aobjParentObject.Elements.indexOf(aobjSelectedRule);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(aobjSelectedRule, aobjParentObject.Elements);
                $rootScope.InsertItem(aobjSelectedRule, aobjParentObject.Elements, index + 1);
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    $scope.isMoveDownValidationRule = function (aobjSelectedRule, aobjParentObject, astrChildName) {
        if (aobjSelectedRule == undefined) {
            return true;
        }
        else {
            if (aobjSelectedRule.Name == astrChildName) {
                var lst = aobjParentObject.Elements.filter(function (x) { return aobjSelectedRule.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Children.indexOf(aobjSelectedRule);
                    if (index != lst[0].Children.length - 1 && index != -1) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                var index = aobjParentObject.Elements.indexOf(aobjSelectedRule);
                if (index != aobjParentObject.Elements.length - 1 && index != -1) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
    };


    //#region Soft Errors Section
    $scope.MoveUpSoftError = function () {
        $scope.MoveUpValidationRule($scope.SelectedSoftError, $scope.objSoftError, "rule");
        $scope.scrollToPosition("#entity-validation-softerror-section", ".page-sidebar-content", $scope.SelectedSoftError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
    };

    $scope.isMoveUpSoftError = function () {
        return $scope.isMoveUpValidationRule($scope.SelectedSoftError, $scope.objSoftError, "rule");
    };

    $scope.MoveDownSoftError = function () {
        $scope.MoveDownValidationRule($scope.SelectedSoftError, $scope.objSoftError, "rule");
        $scope.scrollToPosition("#entity-validation-softerror-section", ".page-sidebar-content", $scope.SelectedSoftError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
    };

    $scope.isMoveDownSoftError = function () {
        return $scope.isMoveDownValidationRule($scope.SelectedSoftError, $scope.objSoftError, "rule");
    };

    //#endregion

    //#region Validate Delete Section
    $scope.MoveUpValidateDelete = function () {
        if ($scope.SelectedValidateDelete) {
            var index = $scope.objValidateDelete.Elements.indexOf($scope.SelectedValidateDelete);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedValidateDelete, $scope.objValidateDelete.Elements);
            $rootScope.InsertItem($scope.SelectedValidateDelete, $scope.objValidateDelete.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#entity-validation-delete-section", ".page-sidebar-content", $scope.SelectedValidateDelete.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveUpValidateDelete = function () {
        if ($scope.SelectedValidateDelete == undefined) {
            return true;
        }
        else {
            var index = $scope.objValidateDelete.Elements.indexOf($scope.SelectedValidateDelete);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };

    $scope.MoveDownValidateDelete = function () {
        if ($scope.SelectedValidateDelete) {
            var index = $scope.objValidateDelete.Elements.indexOf($scope.SelectedValidateDelete);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedValidateDelete, $scope.objValidateDelete.Elements);
            $rootScope.InsertItem($scope.SelectedValidateDelete, $scope.objValidateDelete.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#entity-validation-delete-section", ".page-sidebar-content", $scope.SelectedValidateDelete.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveDownValidateDelete = function () {
        if ($scope.SelectedValidateDelete == undefined) {
            return true;
        }
        else {
            var index = $scope.objValidateDelete.Elements.indexOf($scope.SelectedValidateDelete);
            if (index != $scope.objValidateDelete.Elements.length - 1 && index != -1) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    //#endregion

    //#region Group List Section
    $scope.MoveUpGroupList = function () {
        if ($scope.SelectedGroup) {
            if ($scope.SelectedGroup.Name == "item") {
                var lst = $scope.objGroupList.Elements.filter(function (x) { return $scope.SelectedGroup.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Elements.indexOf($scope.SelectedGroup);
                    if (index > 0) {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem($scope.SelectedGroup, lst[0].Elements);
                        $rootScope.InsertItem($scope.SelectedGroup, lst[0].Elements, index - 1);
                        $rootScope.UndRedoBulkOp("End");
                        $scope.scrollToPosition("#validation-rules-grouplist-section", ".page-sidebar-content", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
                    }

                }
            }
            else {
                var index = $scope.objGroupList.Elements.indexOf($scope.SelectedGroup);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.SelectedGroup, $scope.objGroupList.Elements);
                $rootScope.InsertItem($scope.SelectedGroup, $scope.objGroupList.Elements, index - 1);
                $rootScope.UndRedoBulkOp("End");
                $scope.scrollToPosition("#validation-rules-grouplist-section", ".page-sidebar-content", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
            }
        }
    };

    $scope.isMoveUpGroupList = function () {
        if ($scope.SelectedGroup == undefined) {
            return true;
        }
        else {

            if ($scope.SelectedGroup.Name == "item") {
                var lst = $scope.objGroupList.Elements.filter(function (x) { return $scope.SelectedGroup.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Elements.indexOf($scope.SelectedGroup);
                    if (index > 0) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                var index = $scope.objGroupList.Elements.indexOf($scope.SelectedGroup);
                if (index > 0) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
    };

    $scope.MoveDownGroupList = function () {
        if ($scope.SelectedGroup) {
            if ($scope.SelectedGroup.Name == "item") {
                var lst = $scope.objGroupList.Elements.filter(function (x) { return $scope.SelectedGroup.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Elements.indexOf($scope.SelectedGroup);
                    if (index != lst[0].Elements.length - 1 && index != -1) {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem($scope.SelectedGroup, lst[0].Elements);
                        $rootScope.InsertItem($scope.SelectedGroup, lst[0].Elements, index + 1);
                        $rootScope.UndRedoBulkOp("End");
                        $scope.scrollToPosition("#validation-rules-grouplist-section", ".page-sidebar-content", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
                    }

                }
            }
            else {
                var index = $scope.objGroupList.Elements.indexOf($scope.SelectedGroup);
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.SelectedGroup, $scope.objGroupList.Elements);
                $rootScope.InsertItem($scope.SelectedGroup, $scope.objGroupList.Elements, index + 1);
                $rootScope.UndRedoBulkOp("End");
                $scope.scrollToPosition("#validation-rules-grouplist-section", ".page-sidebar-content", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
            }
        }
    };

    $scope.isMoveDownGroupList = function () {
        if ($scope.SelectedGroup == undefined) {
            return true;
        }
        else {
            if ($scope.SelectedGroup.Name == "item") {
                var lst = $scope.objGroupList.Elements.filter(function (x) { return $scope.SelectedGroup.ParentID == x.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    var index = lst[0].Elements.indexOf($scope.SelectedGroup);
                    if (index != lst[0].Elements.length - 1 && index != -1) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                var index = $scope.objGroupList.Elements.indexOf($scope.SelectedGroup);
                if (index != $scope.objGroupList.Elements.length - 1 && index != -1) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
    };
    //#endregion

    //#region Check List Section
    $scope.MoveUpCheckList = function () {
        if ($scope.SelectedCheckList) {
            var index = $scope.objCheckList.Elements.indexOf($scope.SelectedCheckList);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedCheckList, $scope.objCheckList.Elements);
            $rootScope.InsertItem($scope.SelectedCheckList, $scope.objCheckList.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#rule-checklist", ".page-sidebar-content", $scope.SelectedCheckList.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveUpCheckList = function () {
        if ($scope.SelectedCheckList == undefined) {
            return true;
        }
        else {
            var index = $scope.objCheckList.Elements.indexOf($scope.SelectedCheckList);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };

    $scope.MoveDownCheckList = function () {
        if ($scope.SelectedCheckList) {
            var index = $scope.objCheckList.Elements.indexOf($scope.SelectedCheckList);
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.SelectedCheckList, $scope.objCheckList.Elements);
            $rootScope.InsertItem($scope.SelectedCheckList, $scope.objCheckList.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollToPosition("#rule-checklist", ".page-sidebar-content", $scope.SelectedCheckList.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
        }
    };

    $scope.isMoveDownCheckList = function () {
        if ($scope.SelectedCheckList == undefined) {
            return true;
        }
        else {
            var index = $scope.objCheckList.Elements.indexOf($scope.SelectedCheckList);
            if (index != $scope.objCheckList.Elements.length - 1 && index != -1) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    //#endregion


    //#endregion

    //#region Creation Logic
    $scope.onnewfileclick = function (filetype) {
        var newScope = $scope.$new();
        newScope.IsEntityDisabled = true;
        newScope.SelectedCreationOption = filetype;
        newScope.SelectedEntity = $scope.objEntity.dictAttributes.ID;
        newScope.title = "Create New Entity";
        newScope.dialogForNew = $rootScope.showDialog(newScope, newScope.title, "CreateNewObject/views/CreateNewObject.html", {
            width: 830, height: 700
        });
    };
    //#endregion

    //#region Refresh Object Method Parameters
    $scope.refreshMethodParams = function (method) {
        if ($scope.objEntity.dictAttributes.sfwObjectID && method.dictAttributes.ID) {
            $.connection.hubEntityModel.server.refreshMethodParameterClick(method.dictAttributes.ID, $scope.objEntity.dictAttributes.sfwObjectID).done(function (objmethod) {
                $scope.$evalAsync(function () {
                    if (objmethod) {
                        $rootScope.UndRedoBulkOp("Start");
                        var objParams;
                        var params = method.Elements.filter(function (itm) {
                            return itm.Name == "parameters";
                        });
                        if (!params || (params && params.length == 0)) {
                            objParams = {
                                Name: "parameters", Value: '', dictAttributes: {}, Elements: []
                            };
                            $rootScope.PushItem(objParams, method.Elements);
                        }
                        else {
                            if (objmethod.Parameters.length > 0) {
                                objParams = params[0];
                                $rootScope.EditPropertyValue(objParams.Elements, objParams, "Elements", []);
                            }
                            else {
                                $rootScope.DeleteItem(params[0], method.Elements);
                            }
                        }

                        if (objParams) {
                            $scope.AddOrRefreshObjectMethodParams(objmethod, objParams);
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                });
            });
        }
    };
    //#endregion

    $scope.PopulateRelatedEntity = function (id, model, olditem, isUpdate) {
        $scope.setObjectFieldForProperty(id, model, olditem, isUpdate);
        if (model.dictAttributes.sfwValue) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            for (var i = 0; i < entityIntellisenseList.length; i++) {
                if (entityIntellisenseList[i].BusinessObjectName == model.selecteditembusinessobjectname) {
                    model.dictAttributes.sfwEntity = entityIntellisenseList[i].ID;
                    break;
                }
            }
        }
        else {
            model.dictAttributes.sfwEntity = "";
        }
        $ValidationService.validateEntity(model, $scope.validationErrorList);
    };
    $scope.CallValidateEmptyObjectField = function (model) {
        if ($scope.objEntity && $scope.objEntity.dictAttributes && $scope.objEntity.dictAttributes.sfwObjectID) {
            $ValidationService.validateEmptyObjectField(model, $scope.validationErrorList, '');
        }
    }
    $scope.setObjectFieldForProperty = function (id, model, olditem, isUpdate) {
        if (model) {
            model.dictAttributes.sfwValue = id;
            if (model.dictAttributes.hasOwnProperty("sfwDefaultValue")) {
                model.dictAttributes.sfwDefaultValue = "";
            }
            var attributeModel = getAttributesList();
            var currentItemIndex = attributeModel.Elements.indexOf(olditem);
            if (isUpdate) {
                attributeModel.Elements.splice(currentItemIndex, 1);
            }
            if ($scope.objEntity && $scope.objEntity.dictAttributes && $scope.objEntity.dictAttributes.sfwObjectID) {
                $ValidationService.validateEmptyObjectField(model, $scope.validationErrorList, model.dictAttributes.sfwValue);
            }
            var objDuplicate = $ValidationService.findDuplicateId(attributeModel, model.dictAttributes.sfwValue, CONST.ENTITY.NODES, "description", model, "dictAttributes.sfwValue");
            if (objDuplicate) {
                var errorMessage = CONST.VALIDATION.DUPLICATE_FIELD;
                if (objDuplicate.hasOwnProperty("isParent") && objDuplicate.isParent) {
                    errorMessage = "Duplicate field present in parent entity ";
                }
                if (!model.warnings && !angular.isObject(model.warnings)) {
                    model.warnings = {};
                }
                model.warnings.sfwValue = errorMessage;
            }
            else {
                if (model.warnings) {
                    delete model.warnings.sfwValue;
                }
            }
        }
    };

    //#region Arrow Key Up & Down

    //$scope.KeyUp = function (ruleType) {
    //    if (ruleType && ruleType == 'InitialLoad' && $scope.SelectedInitialLoad) {
    //        var tempObj = keyUpAction($scope.SelectedInitialLoad, $scope.objInitialLoad);
    //        if (tempObj) {
    //            $scope.SelectedInitialLoadClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }



    //    }

    //    else if (ruleType && ruleType == 'HardError' && $scope.SelectedHardError) {

    //        var tempObj = keyUpAction($scope.SelectedHardError, $scope.objHardError);
    //        if (tempObj) {
    //            $scope.SelectedErrorClick(tempObj, null, 'HardError');
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'SoftError' && $scope.SelectedSoftError) {


    //        var tempObj = keyUpAction($scope.SelectedSoftError, $scope.objSoftError);
    //        if (tempObj) {
    //            $scope.SelectedErrorClick(tempObj, null, 'SoftError');

    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'ValidateDelete' && $scope.SelectedValidateDelete) {


    //        var tempObj = keyUpAction($scope.SelectedValidateDelete, $scope.objValidateDelete);
    //        if (tempObj) {
    //            $scope.SelectedValidateDeleteClick(tempObj);

    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'Group' && $scope.SelectedGroup) {


    //        var tempObj = keyUpAction($scope.SelectedGroup, $scope.objGroupList);
    //        if (tempObj) {
    //            $scope.SelectGroupClick(tempObj);

    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'Rule' && $scope.SelectedRule) {


    //        var tempObj = keyUpAction($scope.SelectedRule, $scope.objRules);
    //        if (tempObj) {
    //            $scope.selectedRules(tempObj);

    //        }
    //        else {

    //            return true;
    //        }

    //    }
    //    else if (ruleType && ruleType == 'Query' && $scope.selectedQuery) {

    //        var tempObj = keyUpAction($scope.selectedQuery, $scope.objQueries);
    //        if (tempObj) {
    //            $scope.selectQuery(tempObj);

    //        }
    //        else {

    //            return true;
    //        }

    //    }
    //}

    //$scope.KeyDown = function (ruleType) {
    //    if (ruleType && ruleType == 'InitialLoad' && $scope.SelectedInitialLoad) {

    //        var tempObj = keyDownAction($scope.SelectedInitialLoad, $scope.objInitialLoad);
    //        if (tempObj) {
    //            $scope.SelectedInitialLoadClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }


    //    }


    //    else if (ruleType && ruleType == 'HardError' && $scope.SelectedHardError) {


    //        var tempObj = keyDownAction($scope.SelectedHardError, $scope.objHardError);
    //        if (tempObj) {
    //            $scope.SelectedErrorClick(tempObj, null, 'HardError');
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'SoftError' && $scope.SelectedSoftError) {

    //        var tempObj = keyDownAction($scope.SelectedSoftError, $scope.objSoftError);
    //        if (tempObj) {
    //            $scope.SelectedErrorClick(tempObj, null, 'SoftError');
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'ValidateDelete' && $scope.SelectedValidateDelete) {

    //        var tempObj = keyDownAction($scope.SelectedValidateDelete, $scope.objValidateDelete);
    //        if (tempObj) {
    //            $scope.SelectedValidateDeleteClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'Group' && $scope.SelectedGroup) {

    //        var tempObj = keyDownAction($scope.SelectedGroup, $scope.objGroupList);
    //        if (tempObj) {
    //            $scope.SelectGroupClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'Rule' && $scope.SelectedRule) {

    //        var tempObj = keyDownAction($scope.SelectedRule, $scope.objRules);
    //        if (tempObj) {
    //            $scope.selectedRules(tempObj);
    //        }
    //        else {

    //            return true;
    //        }

    //    }

    //    else if (ruleType && ruleType == 'Query' && $scope.selectedQuery) {

    //        var tempObj = keyDownAction($scope.selectedQuery, $scope.objQueries);
    //        if (tempObj) {
    //            $scope.selectQuery(tempObj);
    //        }
    //        else {

    //            return true;
    //        }

    //    }
    //}
    //#endregion

    $scope.onKeyDownForQueryColumn = function (event) {
        var input = $(event.target);
        var data = [];
        if ($scope.objAttributes && $scope.objAttributes.Elements && $scope.objAttributes.Elements.length > 0) {
            angular.forEach($scope.objAttributes.Elements, function (item) {
                if (item.dictAttributes.sfwType == "Column" && item.dictAttributes.sfwValue) {
                    data.push({
                        ID: item.dictAttributes.sfwValue
                    });
                }
            });
        }
        if (data.length > 0) {
            setMultilevelAutoCompleteForObjectTreeIntellisense($(input), data, undefined, undefined, $scope);
        }
    };

    $scope.scrollToPosition = function (astrDivID, astrClassName, astrID, settings, obj) {
        $timeout(function () {
            var elem = $("#" + $scope.currentfile.FileName).find(astrDivID).find(astrClassName + " ul li span").filter(function () {
                return $(this).text() === astrID;
            });
            var newElem = null;
            if (elem && elem.length > 1 && obj) {
                elem.each(function () {
                    var parentElem = $(this).parents("li").first();
                    var scopeObj = parentElem.scope().element;
                    if (scopeObj && scopeObj == obj) {
                        newElem = $(this);
                    }
                });
            }
            elem = $(elem)[0];
            if (newElem) elem = newElem;
            if (elem) {
                $("#" + $scope.currentfile.FileName).find(astrDivID).find(astrClassName).scrollTo(elem, settings, null);
            }
        });
    };

    //#region Expression Builder

    //#region Add Business Object Method/Property
    $scope.onBusinessMethodCommand = function () {
        var newScope = $scope.$new();
        newScope.objBusinessObject = {};
        newScope.objBusinessObject.IsParentDialog = true;

        newScope.afterOkClick = function (expression) {
            //if ($scope.SelectedRule) {
            //    if (!$scope.SelectedRule.dictAttributes.sfwExpression) {
            //        $scope.SelectedRule.dictAttributes.sfwExpression = "";
            //    }
            //    $scope.SelectedRule.dictAttributes.sfwExpression += expression;
            //}
            $scope.setExpressionValue(expression);
        };

        if ($scope.objEntity && $scope.objEntity.dictAttributes && $scope.objEntity.dictAttributes.sfwObjectID) {
            newScope.busObjectName = $scope.objEntity.dictAttributes.sfwObjectID;
            if ($scope.SelectedRule && $scope.SelectedRule.dictAttributes.sfwObjectBased) {
                newScope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;
            }
            if ($scope.SelectedRule) {
                newScope.BusinessObjectPropertyMethodDialog = $rootScope.showDialog(newScope, "Business Object Property / Method", "Entity/views/ExpressionBuilder/BusinessObjectPropertyMethod.html", {
                    width: 1070, height: 450
                });
            }
        }
    };
    //#endregion


    //#region Query

    $scope.onQueryCommand = function () {
        var newScope = $scope.$new();
        newScope.objQuery = {};
        newScope.objQuery.IsParentDialog = true;
        newScope.afterOkClick = function (expression) {
            $scope.setExpressionValue(expression);
        };
        if ($scope.objEntity && $scope.objEntity.dictAttributes) {
            newScope.busObjectName = $scope.objEntity.dictAttributes.sfwObjectID;
            if ($scope.SelectedRule && $scope.SelectedRule.dictAttributes.sfwObjectBased) {
                newScope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;
            }
            if ($scope.SelectedRule) {
                newScope.QueryDialog = $rootScope.showDialog(newScope, "Query", "Entity/views/ExpressionBuilder/Query.html", {
                    width: 1070, height: 450
                });
            }
        }

    };
    //#endregion


    //#region Internal Functions

    $scope.onInternalFunctionsClick = function () {
        var newScope = $scope.$new();
        newScope.ObjInternalFunctions = {};
        newScope.ObjInternalFunctions.IsParentDialog = true;


        newScope.afterOkClick = function (expression) {
            $scope.setExpressionValue(expression);
        };
        if ($scope.objEntity && $scope.objEntity.dictAttributes) {
            newScope.busObjectName = $scope.objEntity.dictAttributes.sfwObjectID;
            if ($scope.SelectedRule && $scope.SelectedRule.dictAttributes && $scope.SelectedRule.dictAttributes.sfwObjectBased) {
                newScope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;
            }
            if ($scope.SelectedRule) {
                newScope.InternalFunctionsDialog = $rootScope.showDialog(newScope, "Internal Functions", "Entity/views/ExpressionBuilder/InternalFunctions.html", {
                    width: 1070, height: 450
                });
            }
        }
    };

    $scope.setExpressionValue = function (exp) {
        if (exp == undefined) {
            exp = '';
        }
        if ($scope.SelectedRule && $scope.SelectedRule.dictAttributes) {
            var expression = addTextAtCaret('textArea' + $scope.objEntity.dictAttributes.ID, exp);
            $rootScope.EditPropertyValue($scope.SelectedRule.dictAttributes.sfwExpression, $scope.SelectedRule.dictAttributes, "sfwExpression", expression.trim());
        }
    };
    //#endregion


    //#endregion

    $scope.FindRuleIsPresentAsChild = function (aclbCollection, astrRuleID, isSearchChild) {
        var arrResultantRule = [];
        for (var i = 0; i < aclbCollection.length; i++) {
            if (aclbCollection[i].dictAttributes.ID && aclbCollection[i].dictAttributes.ID == astrRuleID) {
                arrResultantRule.push(aclbCollection[i]);
                break;
            }
            else if (!isSearchChild && aclbCollection[i].Children && aclbCollection[i].Children.length > 0) {
                arrResultantRule = aclbCollection[i].Children.filter(function (aObjRule) { return aObjRule.dictAttributes.ID && aObjRule.dictAttributes.ID == astrRuleID });
                if (arrResultantRule.length > 0) {
                    aclbCollection[i].IsExpanded = true;
                    break;
                }
            }
            else if (!isSearchChild && aclbCollection[i].Elements && aclbCollection[i].Elements.length > 0) {
                arrResultantRule = aclbCollection[i].Elements.filter(function (aObjRule) { return aObjRule.dictAttributes.ID && aObjRule.dictAttributes.ID == astrRuleID });
                if (arrResultantRule.length > 0) {
                    aclbCollection[i].IsExpanded = true;
                    break;
                }
            }
        }

        return arrResultantRule;
    };
    $scope.selectValidationRuleInAllTypes = function (aObjRule, ablnScroll, astrCurrentSelecionClick, isSearchChild) {
        $scope.SelectedRule = null;
        $scope.SelectedInitialLoad = null;
        $scope.SelectedHardError = null;
        $scope.SelectedSoftError = null;
        $scope.SelectedValidateDelete = null;

        if (aObjRule && aObjRule.dictAttributes.ID) {
            $scope.clearSelection();
            if ($scope.objRules.Elements || ($scope.objInheritedRules && $scope.objInheritedRules.Elements)) {
                var objInheritedRule = [];
                var objRule = $scope.objRules.Elements.filter(function (aRule) { return aRule.dictAttributes.ID && aRule.dictAttributes.ID == aObjRule.dictAttributes.ID });
                if (objRule && objRule.length > 0 && ($scope.selectedIneritedTab && $scope.selectedIneritedTab == 'Rule')) {
                    $scope.SelectedRule = objRule[0];
                    $scope.selectedIneritedTab = 'Rule';
                }
                else if ($scope.objInheritedRules && $scope.objInheritedRules.Elements) {
                    objInheritedRule = $scope.objInheritedRules.Elements.filter(function (aRule) { return aRule.dictAttributes.ID && aRule.dictAttributes.ID == aObjRule.dictAttributes.ID });
                    if (objInheritedRule && objInheritedRule.length > 0) {
                        $scope.SelectedRule = objInheritedRule[0];
                        $scope.selectedIneritedTab = 'InheritedRule';
                    }
                }
                if (!$scope.SelectedRule) {
                    if (objRule && objRule.length > 0) {
                        $scope.SelectedRule = objRule[0];
                        $scope.selectedIneritedTab = 'Rule';
                    }
                    else if (objInheritedRule && objInheritedRule.length > 0) {
                        $scope.SelectedRule = objInheritedRule[0];
                        $scope.selectedIneritedTab = 'InheritedRule';
                    }
                }
                if ($scope.SelectedRule) {
                    if ($scope.SelectedRule.IsMessageSelected == undefined || $scope.SelectedRule.IsMessageSelected == "") {
                        $scope.SelectedRule.IsMessageSelected = 'MessageID';
                    }
                    else if ($scope.SelectedRule.dictAttributes.sfwMessageId) {
                        $scope.SelectedRule.IsMessageSelected = 'MessageID';
                    }
                    if ($scope.SelectedRule.dictAttributes.sfwMessage) {
                        $scope.SelectedRule.IsMessageSelected = 'Description';
                    }
                    if ($scope.SelectedRule.dictAttributes.sfwMessageId && !$scope.SelectedRule.lstMessageParams) {
                        $scope.populateMessageByMessageID($scope.SelectedRule.dictAttributes.sfwMessageId, $scope.SelectedRule.dictAttributes.sfwMessageParameters, false);
                    }
                    else if ($scope.SelectedRule.dictAttributes.sfwMessage) {
                        $scope.getParameterByDescription($scope.SelectedRule.dictAttributes.sfwMessage, false);
                        if ($scope.SelectedRule.dictAttributes.sfwMessageParameters) {
                            var lstParameters = $scope.SelectedRule.dictAttributes.sfwMessageParameters.split(';');
                            for (var i = 0; i < $scope.SelectedRule.lstMessageParams.length; i++) {
                                $scope.SelectedRule.lstMessageParams[i].Id = lstParameters[i];
                            }
                        }
                    }
                    if (ablnScroll && astrCurrentSelecionClick != "ValidationRule") {
                        if (!$scope.objInheritedRules) {
                            $scope.scrollToPosition("#validationrule", ".page-sidebar-content", $scope.SelectedRule.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
                        }
                        else {
                            $scope.scrollToPosition("#validationrule", ".inherited-rules-list", $scope.SelectedRule.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
                        }
                    }

                }
                if ($scope.objInitialLoad && $scope.objInitialLoad.Elements) {
                    var objInitialLoadPresent = $scope.objInitialLoad.Elements.filter(function (aInitialLoadRule) { return aInitialLoadRule.dictAttributes.ID && aInitialLoadRule.dictAttributes.ID == aObjRule.dictAttributes.ID });
                    if (objInitialLoadPresent.length > 0) {
                        $scope.SelectedInitialLoad = objInitialLoadPresent[0];
                        if (ablnScroll && astrCurrentSelecionClick != "InitialLoad") {
                            $scope.scrollToPosition("#entity-validation-initialload-section", ".page-sidebar-content", $scope.SelectedInitialLoad.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
                        }

                    }
                }

                if ($scope.objHardError && $scope.objHardError.Elements) {
                    var objHardErrorPresent = $scope.FindRuleIsPresentAsChild($scope.objHardError.Elements, aObjRule.dictAttributes.ID, isSearchChild);
                    if (objHardErrorPresent.length > 0) {
                        $scope.SelectedHardError = objHardErrorPresent[0];
                        if (ablnScroll && astrCurrentSelecionClick != "HardError") {
                            $scope.scrollToPosition("#entity-validation-harderror-section", ".page-sidebar-content", $scope.SelectedHardError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 }, $scope.SelectedHardError);
                        }
                    }
                }
                if ($scope.objSoftError && $scope.objSoftError.Elements) {

                    var objSoftErrorPresent = $scope.FindRuleIsPresentAsChild($scope.objSoftError.Elements, aObjRule.dictAttributes.ID, isSearchChild);
                    //var objSoftErrorPresent = $scope.objSoftError.Elements.filter(function (aSoftErrorRule) { return aSoftErrorRule.dictAttributes.ID && aSoftErrorRule.dictAttributes.ID == aObjRule.dictAttributes.ID });


                    if (objSoftErrorPresent.length > 0) {
                        $scope.SelectedSoftError = objSoftErrorPresent[0];
                        if (ablnScroll && astrCurrentSelecionClick != "SoftError") {
                            $scope.scrollToPosition("#entity-validation-softerror-section", ".page-sidebar-content", $scope.SelectedSoftError.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 }, $scope.SelectedSoftError);
                        }
                    }
                }
                if ($scope.objValidateDelete && $scope.objValidateDelete.Elements) {
                    var objValidateDeletePresent = $scope.objValidateDelete.Elements.filter(function (aValidateDeleteRule) { return aValidateDeleteRule.dictAttributes.ID && aValidateDeleteRule.dictAttributes.ID == aObjRule.dictAttributes.ID });
                    if (objValidateDeletePresent.length > 0) {
                        $scope.SelectedValidateDelete = objValidateDeletePresent[0];
                        if (ablnScroll && astrCurrentSelecionClick != "ValidateDelete") {
                            $scope.scrollToPosition("#entity-validation-delete-section", ".page-sidebar-content", $scope.SelectedValidateDelete.dictAttributes.ID, { offsetTop: 400, offsetLeft: 0 });
                        }
                    }
                }
                if ($scope.objGroupList && $scope.objGroupList.Elements) {
                    var objGroupListPresent = $scope.FindRuleIsPresentAsChild($scope.objGroupList.Elements, aObjRule.dictAttributes.ID);
                    if (objGroupListPresent.length > 0) {
                        $scope.SelectedGroup = objGroupListPresent[0];
                        if (ablnScroll && astrCurrentSelecionClick != "GroupList") {
                            $scope.scrollToPosition("#validation-rules-grouplist-section", ".page-sidebar-content", $scope.SelectedGroup.dictAttributes.ID, { offsetTop: 400 });
                        }
                    }
                }
            }
        }

    };

    //#region Creation of Table Hierarchy from Entity

    $scope.addColumn = function () {
        var column = {
            Name: "attribute",
            Value: "",
            Elements: [],
            Children: [],
            dictAttributes: {
                sfwType: "Column"
            }
        }
        $rootScope.PushItem(column, $scope.objAttributes.Elements);
        $scope.selectColumn(column);
        $scope.setFocustoTextbox('attributescolumn', true, column);
    }
    $scope.deleteColumn = function () {
        if ($scope.selectedColumn) {
            if (confirm("Attribute : '" + $scope.selectedColumn.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                var index = $scope.objAttributes.Elements.indexOf($scope.selectedColumn);
                if (index > -1) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem($scope.selectedColumn, $scope.objAttributes.Elements, null);

                    var found = false;
                    var containsAny = function (item) {
                        return item.dictAttributes.sfwType == "Column";
                    };
                    if ($scope.objAttributes.Elements.some(containsAny)) {

                        //decrement the index if it's goes out of index on higher bound.
                        if (index == $scope.objAttributes.Elements.length) {
                            index--;
                        }

                        var nextItemsOfSameType = function (item) {
                            return $scope.objAttributes.Elements.indexOf(item) >= index && item.dictAttributes.sfwType == "Column";
                        };
                        //set the selected property as the next property if exists, other wise the previous property if exist.
                        var items = $scope.objAttributes.Elements.filter(nextItemsOfSameType);
                        if (items && items.length > 0) {
                            $scope.selectColumn(items[0], true);
                            found = true;
                        }
                        else {
                            var previousItemsOfSameType = function (item) {
                                return $scope.objAttributes.Elements.indexOf(item) <= index && item.dictAttributes.sfwType == "Column";
                            };

                            var items = $scope.objAttributes.Elements.filter(previousItemsOfSameType);
                            if (items && items.length > 0) {
                                $scope.selectColumn(items[items.length - 1], true);
                                found = true;
                            }
                        }
                    }

                    //if no property is found to be selected, set it as null.
                    if (!found) {
                        $scope.selectColumn(null, true);
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
    }
    $scope.createTable = function (all) {
        $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
        $rootScope.IsLoading = true;
        $.connection.hubEntityModel.server.createTable($rootScope.currentopenfile.file.FileName, all).done(function (tableStatusList) {
            $scope.$evalAsync(function () {
                if (tableStatusList && tableStatusList.length > 0) {
                    if (tableStatusList.length === 1 && tableStatusList[0].Operation === "DB SYNC" && tableStatusList[0].Error) {
                        toastr.error(tableStatusList[0].Error);
                    }
                    else {
                        //if (tableStatusList[0].TableName && tableStatusList[0].EntityID === $scope.objEntity.dictAttributes.ID) {
                        //    $scope.$evalAsync(function () {
                        //        $scope.objEntity.dictAttributes.sfwTableName = tableStatusList[0].TableName;
                        //    });
                        //}
                        var newScope = $scope.$new();
                        newScope.lstSummaryReport = tableStatusList;
                        newScope.createTableQueryCount = 0;
                        newScope.alterTableQueryCount = 0;
                        newScope.createTableSuccessQueryCount = 0;
                        newScope.alterTableSuccessQueryCount = 0;
                        for (var i = 0; i < newScope.lstSummaryReport.length; i++) {
                            if (newScope.lstSummaryReport[i].Operation && newScope.lstSummaryReport[i].Operation.toUpperCase() == "CREATE TABLE") {
                                newScope.createTableQueryCount++;
                                if (!newScope.lstSummaryReport[i].Error) {
                                    newScope.createTableSuccessQueryCount++;
                                }
                            }
                            else if (newScope.lstSummaryReport[i].Operation && newScope.lstSummaryReport[i].Operation.toUpperCase() == "ALTER TABLE" && !newScope.lstSummaryReport[i].Error) {
                                newScope.alterTableQueryCount++;
                                if (!newScope.lstSummaryReport[i].Error) {
                                    newScope.alterTableSuccessQueryCount++;
                                }
                            }
                        }

                        newScope.summaryDialog = $rootScope.showDialog(newScope, "Summary Report", "Entity/views/TableSummaryReport.html", { width: 700, height: 450 });
                        newScope.onOKClick = function () {
                            newScope.summaryDialog.close();
                            $scope.initialize();
                            //var errors = newScope.lstSummaryReport.filter(function (x) { return x.Error && x.Error.trim().length > 0; })
                            //if (errors && errors.length == 0) {
                            //}
                        }
                    }
                }
                else {
                    toastr.info("No change found.");
                }
                $rootScope.IsLoading = false;
            });
        });
    }
    $scope.syncDBWithEntity = function () {
        var mainPageScope = getScopeByFileName("MainPage");
        if (mainPageScope && mainPageScope.strDBConnection && mainPageScope.strDBConnection === "DB : Not Connected.") {
            toastr.error("Database not connected.");
        }
        else if (confirm("This operation will update the database hierarchically as per your entity structure.\nAre you sure you want to continue?")) {
            $scope.createTable(true);
        }
    }
    $scope.validateLength = function (e) {
        var returnValue = false;
        var input = e.currentTarget;
        var currentValue = $(input).val();

        if (e.keyCode === 109 || e.keyCode === 77) {
            $(input).val("MAX");
        }
        if (e.keyCode >= 48 && e.keyCode <= 57 && currentValue == "MAX") {
            $(input).val("");
        }

        currentValue = $(input).val();

        if ((currentValue.trim() === "" || currentValue == "MAX" || currentValue.indexOf(",") > -1) && e.keyCode === 44) {
            returnValue = false;
        }
        else if (currentValue == "" && e.keyCode >= 48 && e.keyCode <= 57) {
            returnValue = true;
        }
        else {
            var lastChar = currentValue.substr(currentValue.length - 1);
            if (lastChar == "," && e.keyCode >= 48 && e.keyCode <= 57) {
                returnValue = true;
            }
            else if (e.keyCode == 44 || (e.keyCode >= 48 && e.keyCode <= 57)) {
                returnValue = true;
            }
            else {
                returnValue = false;
            }
        }

        if (returnValue === false) {
            e.preventDefault();
        }

        return returnValue;
    }

    $scope.onColumnDataTypeChanged = function (attribute) {
        if (attribute && attribute.dictAttributes && ['int', 'long', 'short'].indexOf(attribute.dictAttributes.sfwDataType) === -1) {
            attribute.dictAttributes.sfwIsIdentity = "";
        }
    }
    //#endregion

    // on item list on enter open file
    $timeout(function () {
        $("#" + $scope.currentfile.FileName).find(".entity-dashboard-items-wrapper").on("keydown", function (e) {
            var key = e.which;
            e.stopPropagation();
            if (key == 13)  // the enter key code
            {
                if ($scope.currentFileObj && $scope.currentFileObj.isSelected) {
                    $rootScope.openFile($scope.currentFileObj);
                    e.preventDefault();
                }
                return false;
            }
        });
    });

    $scope.DeletePropertyFromObject = function (astrProperty) {
        if ($scope.SelectedRule && !$scope.SelectedRule.dictAttributes[astrProperty]) {
            delete $scope.SelectedRule.dictAttributes[astrProperty];
        }
    }

    $scope.validateReqMessageID = function (obj, value) {
        var list = $ValidationService.getListByPropertyName($scope.lstMessages, "MessageID");
        list.unshift("0"); // deafult message id 
        if (value == "ReqMsgID") {
            if (obj.dictAttributes.sfwRequired == "True") {
                obj.dictAttributes.sfwReqMessageId = "";
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwReqMessageId, "sfwReqMessageId", "sfwReqMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, undefined, false);
            }
            else {
                if (obj.dictAttributes.sfwRequired == "False") {
                    delete obj.errors.sfwReqMessageId;
                }
            }
        }
        if (value == "MaxMsgID") {
            if (obj.dictAttributes.sfwMaxValue) {
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwMaxMessageId, "sfwMaxMessageId", "sfwMaxMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, undefined, false);
            }
            else {
                if (obj.dictAttributes.sfwMaxMessageId != "") {
                    delete obj.errors.sfwMaxMessageId;
                }
                if (obj.dictAttributes.sfwMaxMessageId == "" && obj.errors.sfwMaxMessageId) {
                    delete obj.errors.sfwMaxMessageId;
                }
            }
        }
        if (value == "MinMsgID") {
            if (obj.dictAttributes.sfwMinValue) {
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwMinMessageId, "sfwMinMessageId", "sfwMinMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, undefined, false);
            }
            else {
                if (obj.dictAttributes.sfwMinMessageId != "") {
                    delete obj.errors.sfwMinMessageId;
                }
                if (obj.dictAttributes.sfwMinMessageId == "" && obj.errors.sfwMinMessageId) {
                    delete obj.errors.sfwMinMessageId;
                }
            }
        }
        if (obj && obj.errors && $ValidationService.isEmptyObj(obj.errors)) {
            $ValidationService.removeObjInToArray($scope.validationErrorList, obj);
        }
    }

    $scope.addNoLockToQueries = function () {

        for (var k = 0; k < $scope.objEntity.Elements.length; k++) {
            var item = $scope.objEntity.Elements[k];
            if (item.Name == "queries") {

                $scope.HubCallToAddNolockQuery(item.Elements, 0);
                break;

            };

        }
    };

    $scope.HubCallToAddNolockQuery = function (lstElements, acntr) {
        var query = lstElements[acntr];
        if (query && query.Name == "query") {
            if ($scope.lstDBQueryTypes.length > 0) {
                var queryTypes = $scope.lstDBQueryTypes;
                for (var i = 0; i < queryTypes.length; i++) {
                    if (queryTypes[i] && queryTypes[i].Attribute) {
                        if (query.dictAttributes[queryTypes[i].Attribute]) {
                            var strQueryAttribute = queryTypes[i].Attribute;
                            if (query.dictAttributes[strQueryAttribute] != undefined && query.dictAttributes[strQueryAttribute] != '') {
                                var callQueryWithNoLock = $FormatQueryFactory.createQueryWithNoLock(query.dictAttributes[strQueryAttribute]);
                                callQueryWithNoLock.then(function (astrQuery) {
                                    if (astrQuery) {
                                        $scope.$evalAsync(function () {
                                            if (lstElements.indexOf(query) == 0) {
                                                $rootScope.IsLoading = true;

                                                $rootScope.UndRedoBulkOp("Start");
                                            }
                                            $rootScope.EditPropertyValue(query.dictAttributes[strQueryAttribute], query.dictAttributes, strQueryAttribute, astrQuery, "showSelectedQueryInEditor");
                                            $scope.showSelectedQueryInEditor();
                                            acntr++;
                                            if (acntr < lstElements.length) {
                                                $scope.HubCallToAddNolockQuery(lstElements, acntr);
                                            }
                                            if (lstElements.indexOf(query) == lstElements.length - 1) {
                                                $rootScope.IsLoading = false;

                                                $rootScope.UndRedoBulkOp("End");
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
    };
}]);
