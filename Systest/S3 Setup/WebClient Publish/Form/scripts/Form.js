app.controller('FormController', ["$scope", "$rootScope", "ngDialog", "$EntityIntellisenseFactory", "$NavigateToFileService", "$timeout", "$filter", "hubcontext", "$compile", "ConfigurationFactory", '$getQueryparam', 'FormDetailsFactory', 'CONSTANTS', '$ValidationService', '$Errors', '$GetEntityFieldObjectService', '$Entityintellisenseservice', '$getModelList', '$SgMessagesService', '$GetGridEntity', function ($scope, $rootScope, ngDialog, $EntityIntellisenseFactory, $NavigateToFileService, $timeout, $filter, hubcontext, $compile, ConfigurationFactory, $getQueryparam, FormDetailsFactory, CONST, $ValidationService, $Errors, $GetEntityFieldObjectService, $Entityintellisenseservice, $getModelList, $SgMessagesService, $GetGridEntity) {
    $rootScope.IsLoading = true;

    //#region Common Properties  
    $scope.lstLoadedEntityTrees = [];
    $scope.lstLoadedEntityColumnsTree = [];
    $scope.entityTreeName = "";
    $scope.currentEntiyTreeObject = undefined;
    $scope.lookupTreeObject = [];
    $scope.ColumnsLimitCount = 50;
    $scope.LstDisplayedEntities = [];
    $scope.MainPanels = [];
    $scope.MainPanelID = "";
    $scope.selectedDesignSource = false;
    $scope.IsEntityTreeExpanded = false;
    $scope.IsToolsDivCollapsed = false;
    $scope.isDeveloperView = false;
    $scope.SelectedQuery = undefined;
    $scope.DesignHighlightAttribute = null;
    $scope.lstselectedobjecttreefields = [];
    $scope.selectedobjecttreefield = undefined;
    $scope.lstEntity = [];
    $scope.ObjgridBoundedQuery = {};
    $scope.lstControl = CONST.FORM.CONTROL_TYPES;
    $scope.parameterList = [];
    //#endregion

    //#region On Load
    $scope.currentfile = $rootScope.currentopenfile.file;

    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveformmodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receiveformmodel = function (data) {
        $scope.$apply(function () {
            var FormDetials;
            if ($scope.currentfile.FileType == "Maintenance") {
                FormDetials = FormDetailsFactory.getFormDetails();
                if (FormDetials) {
                    var objFileDetails = {
                        FileType: 'Lookup', FileName: "wfm" + FormDetials.ID + "Lookup", FilePath: FormDetials.Path, FileID: "wfm" + FormDetials.ID + "Lookup"
                    };
                    $rootScope.SaveModelWithPackets(FormDetials.objSfxLookupForm, objFileDetails, undefined, true);
                    FormDetailsFactory.setFormDetails(undefined);
                }
            }

            $scope.FormModel = data;
            $scope.MainPanels = [];
            $scope.UDCTableList = [];
            $scope.objFormExtraFields = [];
            $scope.SetMainTable();
            if (!$scope.InitialLoad) {
                $scope.InitialLoad = { Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                //$scope.FormModel.Elements.splice(0, 0, $scope.InitialLoad);
            }
            if ($scope.FormModel.dictAttributes.sfwType != "UserControl") {
                $scope.FindDeepNodeUDC($scope.FormModel, $scope.UDCTableList);
                if ($scope.UDCTableList.length > 0) {
                    $scope.SetUDCMainTable();
                }
            }

            $scope.MainPanelID = "MainParentPanel" + $scope.FormModel.dictAttributes.ID;
            if ($scope.MainPanels.length > 0) {
                if ($scope.MainPanels[0].dictAttributes.ID == "pnltoolbar" && $scope.MainPanels.length > 1) $scope.selectPanelControl($scope.MainPanels[1]);
                else $scope.selectPanelControl($scope.MainPanels[0]);
            }

            if ($scope.FormModel.dictAttributes.sfwType == "Lookup") {
                $scope.PopulateQueryId(true);
            }

            var objnew = {
                EntityName: $scope.FormModel.dictAttributes.sfwEntity, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: false, IsMainEntity: true
            };
            if ($scope.FormModel.IsLookupCriteriaEnabled) {
                objnew.IsVisible = false;
            }

            if ($scope.lstLoadedEntityTrees && $scope.lstLoadedEntityTrees.length > 0) {
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                    if ($scope.lstLoadedEntityTrees[i].EntityName == $scope.FormModel.dictAttributes.sfwEntity && !$scope.lstLoadedEntityTrees[i].IsQuery) {
                        $scope.lstLoadedEntityTrees[i].IsVisible = true;
                        // $scope.currentEntiyTreeObject = $scope.lstLoadedEntityTrees[i];
                    }
                    else {
                        $scope.lstLoadedEntityTrees[i].IsVisible = false;
                    }
                }
            }
            else {
                $scope.lstLoadedEntityTrees.push(objnew);
                $scope.entityTreeName = $scope.FormModel.dictAttributes.sfwEntity;
                $scope.currentEntiyTreeObject = objnew;

            }

            $scope.InitialLoadSection();
            if ($scope.FormModel.dictAttributes.sfwType == "UserControl" || $scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                $scope.FormModel.IsLookupCriteriaEnabled = false;
            }
            $scope.objFormExtraFields = $filter('filter')($scope.FormModel.Elements, {
                Name: 'ExtraFields'
            });
            if ($scope.objFormExtraFields.length > 0) {
                $scope.objFormExtraFields = $scope.objFormExtraFields[0];
                //$scope.removeExtraFieldsDataInToMainModel();
            }

            if ($scope.objFormExtraFields.length == 0) {
                $scope.objFormExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                };
            }
            $timeout(function () {
                if (!FormDetials) {
                    $rootScope.IsLoading = false;
                }
            });

            $scope.PopulateRemoteObjects();

            $scope.validationErrorList = [];
            if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
                var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName });
                if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: $scope.currentfile.FileName, errorList: [] });
                //$scope.validateFileData();
                var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];

                $scope.validationErrorList = fileErrObj.errorList = [];
            }
            $scope.formTableModel = $filter('filter')($scope.FormModel.Elements, { Name: "sfwTable" })[0];
            //  createValidationData();
            createRuledata($scope.FormModel.objExtraData);
            $scope.iswizard = $scope.FormModel.dictAttributes.sfwType == "Wizard" ? true : false;
            $scope.isLookup = $scope.FormModel.dictAttributes.sfwType == "Lookup" ? true : false;
            $scope.IsPrototype = $scope.FormModel.dictAttributes.ID && $scope.FormModel.dictAttributes.ID.startsWith("wfp") ? true : false;
        });

        if ($scope.currentfile.SelectNodePath && $scope.currentfile.SelectNodePath != "") {
            $scope.selectElement($scope.currentfile.SelectNodePath);
        }

        CheckForFilterGrid($scope.FormModel);
    };

    $scope.SetFormSelectedControl = function (curelement) {
        SetFormSelectedControl($scope.FormModel, curelement, event);
    };

    $scope.PopulateRemoteObjects = function () {
        $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.FormModel.RemoteObjectCollection = data;
                    if ($scope.FormModel.RemoteObjectCollection && $scope.FormModel.RemoteObjectCollection.length > 0) {
                        $scope.FormModel.RemoteObjectCollection.splice(0, 0, {
                            dictAttributes: {
                                ID: ""
                            }
                        });
                    }
                }
            });
        });
    }

    // #region validation
    var worker;
    $scope.validateFileData = function () {

        if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
            var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName });
            if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: $scope.currentfile.FileName, errorList: [] });
            //$scope.validateFileData();
            var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $scope.currentfile.FileName })[0];

            $scope.validationErrorList = fileErrObj.errorList = [];
        }
        // createValidationData();
        if ($scope.formTableModel) {
            $scope.iterateModel($scope.formTableModel);
            createValidationData();
        }
        $timeout(function () {
            $("#validation-btn").prop("disabled", false);
        });
        //using Web worker 
        /*   if (window.Worker) { // Check if Browser supports the Worker api.
               if(!worker) worker = new Worker("Common/scripts/Worker.js");
   
               worker.postMessage({ 'request': 'data','response': 'DATA' });
               worker.postMessage({ 'request': 'validate_id', 'response': 'VALIDATE_ID' });
               worker.onmessage = function (e) {
               if (e.data == "DATA") {
                   console.log("data created");
                   $scope.createValidationData();
               }
               if (e.data == "VALIDATE_ID") {
                   if ($scope.formTableModel) {
                       $scope.iterateModel($scope.formTableModel);
                   }
               }
               };
           }
           */
    };

    var createValidationData = function () {
        /** form validation **/
        $scope.lstEntity = [];
        $scope.parameterList = [];
        if ($scope.FormModel) {
            if ($scope.FormModel.dictAttributes.hasOwnProperty('sfwEntity')) {
                $ValidationService.validateEntity($scope.FormModel, undefined);
            }
        }
        if ($scope.FormModel.dictAttributes.sfwEntity && $scope.FormModel.errors && !$scope.FormModel.errors.invalid_entity) {
            $rootScope.IsLoading = true;
            $scope.lstEntity.push($scope.FormModel.dictAttributes.sfwEntity);
            traverseModelAndGetEntities($scope.FormModel, $scope.FormModel.dictAttributes.sfwEntity);
            if (angular.isArray($scope.lstQueriesName) && $scope.lstQueriesName.length > 0) {
                //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
                $.connection.hubForm.server.getQueriesResult($scope.lstQueriesName, 'dummy').done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data) {
                            $scope.gridQueryResult = data;
                        }
                    });
                });
            }
            $.connection.hubForm.server.getGlobleParameters().done(function (data) {
                $scope.$evalAsync(function () {
                    if (data) {
                        $scope.objGlobleParametersList(data);
                    }
                });
            });
            if ($scope.isLookup) {
                getQueryIdData($scope.FormModel);
            }
            $.connection.hubMain.server.getFormValidationData($scope.lstEntity).done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.validationData = data;
                    $scope.objEntityExtraData = {};
                    $scope.resourceList = $ValidationService.getListByPropertyName($scope.validationData.Resources, "ResourceID", false);
                    if ($scope.validationData.hasOwnProperty($scope.FormModel.dictAttributes.sfwEntity)) {
                        createRuledata($scope.validationData[$scope.FormModel.dictAttributes.sfwEntity]);
                        $scope.objEntityExtraData.lstHardErrors = $scope.createValidationRuleList($scope.validationData[$scope.FormModel.dictAttributes.sfwEntity], false, $scope.FormModel.dictAttributes.sfwHardErrorGroup);
                    }
                    // validate detail window fields
                    if ($scope.FormModel.dictAttributes.hasOwnProperty('sfwResource') && $scope.FormModel.dictAttributes.sfwResource) {
                        $ValidationService.checkValidListValue($scope.resourceList, $scope.FormModel, $scope.FormModel.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, $scope.validationErrorList);
                    }
                    var list = $ValidationService.getListByPropertyName($scope.lstMessages, "MessageID", false);
                    if ($scope.FormModel.dictAttributes.hasOwnProperty('sfwNewMessageID') && $scope.FormModel.dictAttributes.sfwNewMessageID) {
                        $ValidationService.checkValidListValue(list, $scope.FormModel, $scope.FormModel.dictAttributes.sfwNewMessageID, "sfwNewMessageID", "sfwNewMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, $scope.validationErrorList);
                    }
                    if ($scope.FormModel.dictAttributes.hasOwnProperty('sfwOpenMessageID') && $scope.FormModel.dictAttributes.sfwOpenMessageID) {
                        $ValidationService.checkValidListValue(list, $scope.FormModel, $scope.FormModel.dictAttributes.sfwOpenMessageID, "sfwOpenMessageID", "sfwOpenMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, $scope.validationErrorList);
                    }
                    if ($scope.FormModel.dictAttributes.hasOwnProperty("sfwDefaultButtonID") && $scope.FormModel.dictAttributes.sfwDefaultButtonID) {
                        var lstButtons = [];
                        FindControlListByName($scope.SfxMainTable, "sfwButton", lstButtons);
                        lstButtons = $ValidationService.getListByPropertyName(lstButtons, "ID", false);
                        $ValidationService.checkValidListValue(lstButtons, $scope.FormModel, $scope.FormModel.dictAttributes.sfwDefaultButtonID, "sfwDefaultButtonID", "sfwDefaultButtonID", CONST.VALIDATION.BUTTON_NOT_EXISTS, $scope.validationErrorList);
                    }
                    //End of details window validation
                    $timeout(function () {
                        validateForm($scope.formTableModel, $scope.FormModel.dictAttributes.sfwEntity, $scope.objEntityExtraData);
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                        });
                    });
                });
            });
        }
    };
    $scope.objGlobleParametersList = function (data) {
        angular.forEach(data.Elements, function (paramObj) {
            if (paramObj && paramObj.dictAttributes.ID) {
                var item = "~" + paramObj.dictAttributes.ID;
                $scope.parameterList.push(item);
            }
        });
    };
    $scope.lstQueriesName = [];
    var traverseModelAndGetEntities = function (model, entityid) {
        angular.forEach(model.Elements, function (obj) {
            if (CONST.FORM.COLLECTION_TYPE_NODES.indexOf(obj.Name) > -1) {
                var entity = null;
                if (obj.Name == "sfwGridView" && obj.dictAttributes.sfwBoundToQuery && obj.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && obj.dictAttributes.sfwBaseQuery) {
                    $scope.lstQueriesName.push(obj.dictAttributes.sfwBaseQuery);
                }
                if (obj.Name == "sfwGridView" && obj.dictAttributes.sfwParentGrid) {
                    var objParentGrid = FindControlByID($scope.SfxMainTable, obj.dictAttributes.sfwParentGrid);
                    if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                        var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
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
                if (entity && $scope.lstEntity.indexOf(entity) <= -1) $scope.lstEntity.push(entity);
            }
            if (obj && obj.dictAttributes.ID) {
                $scope.parameterList.push(obj.dictAttributes.ID);
            }
            if (obj.Elements && obj.Elements.length > 0) {
                traverseModelAndGetEntities(obj, entityid);
            }
        });
    };
    var getQueryIdData = function (mainModel) {
        angular.forEach(mainModel.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("sfwDataField") && obj.dictAttributes.sfwDataField) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var list = PopulateColumnList(obj.dictAttributes.sfwQueryID, $scope.FormModel, entityIntellisenseList, $scope.lstLoadedEntityColumnsTree);
            }
            if (obj.Elements && obj.Elements.length > 0) {
                getQueryIdData(obj);
            }
        });
    };
    $scope.objEntityExtraData = {};
    var createRuledata = function (entExtraData) {
        $scope.objEntityExtraData.lstRules = PopulateEntityRules(entExtraData, $scope.iswizard, $scope.FormModel.dictAttributes.sfwInitialLoadGroup);
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
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwVisibleRule', "invalid_visible_rule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, $scope.validationErrorList);
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
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwSelectColVisibleRule', "sfwSelectColVisibleRule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwEnableRule') && obj.dictAttributes.sfwEnableRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwEnableRule', "invalid_enable_rule", CONST.VALIDATION.ENABLE_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwReadOnlyRule') && obj.dictAttributes.sfwReadOnlyRule) {
                var listRule = extraData.lstRules;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listRule = getRuleData(obj);
                }
                $ValidationService.checkValidListValue(listRule, obj, undefined, 'sfwReadOnlyRule', "invalid_readonly_rule", CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRulesGroup') && obj.dictAttributes.sfwRulesGroup) {
                $scope.validateGroups(obj, entityid);
            }

            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && query) {
                $scope.validateGridControlField(obj, query, mainModel);
            } else if (obj.dictAttributes.hasOwnProperty('sfwEntityField') && obj.Name != "parameter" && obj.dictAttributes.sfwEntityField) {
                var attrType = '';
                var entityname = entityid;
                if (FindParent(obj, "sfwGridView")) {
                    entityname = $GetGridEntity.getEntityName($scope.FormModel, obj);
                }

                if (obj.dictAttributes.sfwEntityField == "InternalErrors" || obj.dictAttributes.sfwEntityField == "ExternalErrors") {
                    entityname = "entError";
                }
                if (obj.dictAttributes.sfwRelatedGrid) {
                    entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
                }
                if (obj.dictAttributes.sfwParentGrid) {
                    entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
                }
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityname, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalQuery') && obj.dictAttributes.sfwRetrievalQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwRetrievalQuery, undefined, "sfwRetrievalQuery", "sfwRetrievalQuery", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwRetrievalMethod') && obj.dictAttributes.sfwRetrievalMethod) {
                var entityname = entityid;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
                }
                var methodList = getMethodList(entityname, false, true);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwRetrievalMethod, "sfwRetrievalMethod", "invalid_retrieval_method", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwAutoQuery') && obj.dictAttributes.sfwAutoQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwAutoQuery, undefined, "sfwAutoQuery", "sfwAutoQuery", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwBaseQuery') && obj.dictAttributes.sfwBaseQuery) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwBaseQuery, undefined, "sfwBaseQuery", "sfwBaseQuery", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwResource') && obj.dictAttributes.sfwResource && obj.dictAttributes.sfwResource != 0) {
                $ValidationService.checkValidListValue($scope.resourceList, obj, obj.dictAttributes.sfwResource, "sfwResource", "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwValidationRules') && obj.dictAttributes.sfwValidationRules) {
                var listHardRule = extraData.lstHardErrors;
                if (obj.dictAttributes.sfwRelatedGrid) {
                    listHardRule = getValidationRuleData(obj);
                }
                $ValidationService.checkMultipleValueWithList(listHardRule, obj, obj.dictAttributes.sfwValidationRules, ";", 'sfwValidationRules', "invalid_validation_rule", CONST.VALIDATION.VALIDATION_RULE_NOT_EXISTS, $scope.validationErrorList);
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
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwExpression, "sfwExpression", "invalid_expression", CONST.VALIDATION.INVALID_EXPRESSION, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwMessageID") && obj.dictAttributes.sfwMessageID) {
                list = $ValidationService.getListByPropertyName($scope.lstMessages, "MessageID", false);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwMessageID, "sfwMessageID", "sfwMessageID", CONST.VALIDATION.INVALID_MESSAGE_ID, $scope.validationErrorList);
            }
            if (obj.Name == "sfwDropDownList" || obj.Name == "sfwCascadingDropDownList" || obj.Name == "sfwMultiSelectDropDownList" || obj.Name == "sfwListPicker" || obj.Name == "sfwListBox" || obj.Name == "sfwRadioButtonList") {
                if (obj.dictAttributes.sfwLoadType == "CodeGroup") {
                    var list = $ValidationService.getListByPropertyName($scope.validationData.CodeGroup, "CodeID");
                    list.push("0");
                    $scope.validateEmptyCodeId(obj, list, entityid);
                }
            }
            if (obj.dictAttributes.hasOwnProperty('sfwLoadSource') && obj.dictAttributes.sfwLoadSource) {
                //if (obj.dictAttributes.sfwLoadType == "CodeGroup") {
                //    var list = $ValidationService.getListByPropertyName($scope.validationData.CodeGroup, "CodeID");
                //    if (obj.Name == "sfwDropDownList" || obj.Name == "sfwCascadingDropDownList" || obj.Name == "sfwMultiSelectDropDownList" || obj.Name == "sfwListPicker" || obj.Name == "sfwListBox" || obj.Name == "sfwRadioButtonList") {
                //        $scope.validateEmptyCodeId(obj, list);
                //    } else {
                //        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, $scope.validationErrorList);
                //    }
                //} else
                if (obj.dictAttributes.sfwLoadType == "Query") {
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwLoadSource, undefined, "sfwLoadSource", "sfwLoadSource", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "Method") {
                    var entityname = entityid;
                    if (obj.dictAttributes.sfwRelatedGrid) {
                        entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
                    }
                    var methodList = getMethodList(entityname, true, false);
                    $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_method", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
                } else if (obj.dictAttributes.sfwLoadType == "ServerMethod") {
                    validateServerMethod(obj);
                }
            }
            if (obj.dictAttributes.hasOwnProperty('XValueMember') && obj.dictAttributes.XValueMember) {
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.XValueMember, entityid, "XValueMember", "XValueMember", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, '');
                copyErrorMessages(obj, chartObj);
            }
            if (obj.dictAttributes.hasOwnProperty('YValueMembers') && obj.dictAttributes.YValueMembers) {
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.YValueMembers, entityid, "YValueMembers", "YValueMembers", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, '');
                copyErrorMessages(obj, chartObj);
            }
            if (obj.dictAttributes.hasOwnProperty('sfwCodeTable') && obj.dictAttributes.sfwCodeTable) {
                $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), obj, obj.dictAttributes.sfwCodeTable, undefined, "sfwCodeTable", "sfwCodeTable", CONST.VALIDATION.INVALID_CODE_TABLE, $scope.validationErrorList);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && obj.dictAttributes.sfwEntityField && obj.dictAttributes.hasOwnProperty("sfwMethodName") && obj.dictAttributes.sfwMethodName == "btnOpen_Click") {
                var list = $scope.PopulateEntityFieldsForOpenButton(obj, $scope.isLookup);
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList);
            }
            if ($scope.isLookup && !$scope.IsPrototype) {
                if (obj.dictAttributes.hasOwnProperty("sfwQueryID") && obj.dictAttributes.sfwQueryID) {
                    $ValidationService.checkValidListValue($scope.queryIDList, obj, obj.dictAttributes.sfwQueryID, "sfwQueryID", "invalid_query_id", CONST.VALIDATION.INVALID_QUERY_ID, $scope.validationErrorList);
                }
                /*     if (obj.dictAttributes.hasOwnProperty("sfwDataField") && obj.dictAttributes.sfwDataField) { // user can enter substring,CAST function we can allow it
                         var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                         var list = [], attributeName = "ID";
                         var result = PopulateColumnList(obj.dictAttributes.sfwQueryID, $scope.FormModel, entityIntellisenseList, $scope.lstLoadedEntityColumnsTree);
                         if (result) {
                             list = result.list;
                             attributeName = result.attribute;
                         }
                         // list = $ValidationService.getListByPropertyName(list, attributeName, false);
                         $ValidationService.checkDataFieldValue(list, obj, obj.dictAttributes.sfwDataField, attributeName, "sfwDataField", "invalid_data_field", CONST.VALIDATION.INVALID_DATA_FIELD, $scope.validationErrorList);
                     }
                 */

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

            if (obj.dictAttributes.hasOwnProperty("sfwObjectMethod") && obj.dictAttributes.sfwObjectMethod) {
                var methodList = getMethodList(entityid, false, false);
                $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.sfwObjectMethod, "sfwObjectMethod", "sfwObjectMethod", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
                if (obj && obj.errors && obj.errors.sfwObjectMethod) {
                    validateServerMethod(obj, "sfwObjectMethod");
                }
            }

            if (obj.dictAttributes.hasOwnProperty("sfwEntityField") && obj.dictAttributes.sfwEntityField && (obj.Name == "sfwScheduler" || obj.Name == "sfwCalendar")) {
                var entityname = getEntityName(obj.dictAttributes.sfwEntityField, entityid);
                if (obj.dictAttributes.hasOwnProperty("sfwEventId") && obj.dictAttributes.sfwEventId) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventId, entityname, "sfwEventId", "sfwEventId", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventName") && obj.dictAttributes.sfwEventName) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventName, entityname, "sfwEventName", "sfwEventName", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventStartDate") && obj.dictAttributes.sfwEventStartDate) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventStartDate, entityname, "sfwEventStartDate", "sfwEventStartDate", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventEndDate") && obj.dictAttributes.sfwEventEndDate) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventEndDate, entityname, "sfwEventEndDate", "sfwEventEndDate", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, "");
                }
                if (obj.dictAttributes.hasOwnProperty("sfwEventCategory") && obj.dictAttributes.sfwEventCategory) {
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEventCategory, entityname, "sfwEventCategory", "sfwEventCategory", CONST.VALIDATION.INVALID_FIELD, $scope.validationErrorList, false, "");
                }
            }
            if (obj.Elements && obj.Elements.length > 0) {
                if (chartObj && chartObj.errors && chartObj.errors.hasOwnProperty("series_errors") && !$ValidationService.isEmptyObj(chartObj.errors.series_errors)) delete chartObj.errors.series_errors;

                var newExtraData = {};
                if (obj.Name == "sfwWizardStep") {
                    var strRuleGroup = obj.dictAttributes.sfwRulesGroup;
                    newExtraData.lstHardErrors = $scope.createValidationRuleList($scope.validationData[entityid], $scope.iswizard, strRuleGroup);
                    newExtraData.lstRules = PopulateEntityRules($scope.validationData[entityid], $scope.iswizard, null);
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
                        entity = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity, true);
                    } else {
                        entity = getEntityName(obj.dictAttributes.sfwEntityField, entityid);
                    }
                    var chart = {};
                    if (obj.Name == "sfwChart") chart = obj;
                    if (entity) {
                        if ($scope.validationData && $scope.validationData.hasOwnProperty(entity)) {
                            newExtraData.lstHardErrors = $scope.createValidationRuleList($scope.validationData[entity], $scope.iswizard, null);
                            newExtraData.lstRules = PopulateEntityRules($scope.validationData[entity], $scope.iswizard, null);
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
                        var button = GetFieldFromFormObject($scope.SfxMainTable, 'sfwButton', 'sfwRelatedDialogPanel', strdialogpanelid);
                        if (button && button.length > 0 && button[0].dictAttributes.sfwRelatedControl) {
                            var gridview = GetFieldFromFormObject($scope.SfxMainTable, 'sfwGridView', 'ID', button[0].dictAttributes.sfwRelatedControl);
                            if (gridview && gridview.length > 0) {
                                entityfieldname = gridview[0].dictAttributes.sfwEntityField;
                            }
                        }
                    }
                    var entityName;
                    if (entityfieldname) {
                        entityName = getEntityName(entityfieldname, entityid);
                        if ($scope.validationData && $scope.validationData.hasOwnProperty(entityName)) {
                            newExtraData.lstHardErrors = $scope.createValidationRuleList($scope.validationData[entityName], $scope.iswizard, null);
                            newExtraData.lstRules = PopulateEntityRules($scope.validationData[entityName], $scope.iswizard, null);
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

    $scope.validateParameters = function (obj, params, prop) {
        if (angular.isArray($scope.parameterList) && $scope.parameterList.length <= 0) {
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
            $ValidationService.checkValidListValue($scope.parameterList, obj, strId, prop, prefix + strId, "parameter value(" + strId + ") does not exists", $scope.validationErrorList);
        }
    };
    $scope.validateGroups = function (obj, entityid) {
        if ($scope.validationData && $scope.validationData.hasOwnProperty(entityid)) {
            var lstRules = $scope.validationData[entityid];
            var group = lstRules && lstRules.lstGroupsList;
            var lstGroups = [];
            if (angular.isArray(group) && group.length > 0) {
                lstGroups = $ValidationService.getListByPropertyName(group[0].Elements, 'ID');
            }
            $ValidationService.checkValidListValue(lstGroups, obj, obj.dictAttributes.sfwRulesGroup, 'sfwRulesGroup', "sfwRulesGroup", CONST.VALIDATION.INVALID_GROUP, $scope.validationErrorList);
        }
    };
    $scope.validateGridControlField = function (obj, query, parentObj) {
        if ((obj.Name === "sfwLabel" || obj.Name == "parameter") && parentObj && (parentObj.Name == "HeaderTemplate" || parentObj.Name == "FooterTemplate" || parentObj.Name == "Parameters")) {
            $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, $scope.FormModel.dictAttributes.sfwEntity, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, "");
        } else if ($scope.gridQueryResult && $scope.gridQueryResult.hasOwnProperty(query)) {
            var list = $ValidationService.getListByPropertyName($scope.gridQueryResult[query], "CodeID", false);
            $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList);
        }
    };
    var getRuleData = function (obj) {
        var lstRules = [];
        var entityname = null;
        if (FindParent(obj, "sfwGridView")) {
            entityname = $GetGridEntity.getEntityName($scope.FormModel, obj);

        } else {
            entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
        }
        if (entityname) {
            lstRules = PopulateEntityRules($scope.validationData[entityname], $scope.iswizard, null);
        }
        return lstRules;
    };
    var getValidationRuleData = function (obj) {
        var lstHardErrors = [];
        var entityname = $scope.FindEntityName(obj, $scope.FormModel.dictAttributes.sfwEntity);
        if (entityname) {
            lstHardErrors = $scope.createValidationRuleList($scope.validationData[entityname], $scope.iswizard, null);
        }
        return lstHardErrors;
    };
    $scope.validateEmptyCodeId = function (model, list, entity) {
        var property = "";
        if (model.dictAttributes.sfwRelatedGrid) {
            entity = $scope.FindEntityName(model, $scope.FormModel.dictAttributes.sfwEntity);
        }
        if ($scope.IsSearchCriteriaSelected && model.dictAttributes.sfwDataField) {
            property = "sfwDataField";
        } else if (model.dictAttributes.sfwEntityField) {
            property = "sfwEntityField";
        }
        if (model.Name == "sfwCheckBoxList") {
            property = "sfwCheckBoxField";
        }

        if (model.dictAttributes.sfwLoadSource) {
            $ValidationService.checkValidListValue(list, model, model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, $scope.validationErrorList);
        } else if (model.dictAttributes[property] && model.errors && !model.errors[property]) {
            var placeHolderValue = GetCodeID(entity, model.dictAttributes[property], $EntityIntellisenseFactory.getEntityIntellisense());
            if (!placeHolderValue) {
                $ValidationService.checkValidListValue([], model, model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, $scope.validationErrorList, true);
            }
        }

    };

    var validateServerMethod = function (obj, attribute) {
        if (!(attribute && attribute.trim().length > 0)) {
            attribute = "sfwLoadSource";
        }
        var RemoteObjectName = "srvCommon";
        var lstRemoteObj = [];
        if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwRemoteObject) {
            RemoteObjectName = $scope.FormModel.dictAttributes.sfwRemoteObject;
        }
        if ($scope.validationData && $scope.validationData.hasOwnProperty("RemoteObject")) {
            lstRemoteObj = $scope.validationData["RemoteObject"];
        }
        var objServerObject = GetServerMethodObject(RemoteObjectName, lstRemoteObj);
        var list = PopulateServerMethod([], obj, objServerObject, true);
        $ValidationService.checkValidListValue(list, obj, obj.dictAttributes[attribute], attribute, attribute, CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
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
    $scope.iterateModel = function (model) {
        angular.forEach(model.Elements, function (obj) {
            if (CONST.FORM.IGNORE_NODES.indexOf(obj.Name) <= -1) {
                if (obj.Name == "sfwTable" && $scope.currentfile.FileType == "Lookup") {
                    $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
                } else if (obj.Name != "sfwTable") {
                    $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
                }
                $ValidationService.checkDuplicateId(obj, $scope.formTableModel, $scope.validationErrorList, false, CONST.FORM.NODES);
            }
            if (obj.Elements && obj.Elements.length > 0 && (CONST.FORM.NODES.indexOf(obj.Name) > -1)) {
                $scope.iterateModel(obj);
            }
        });
    };
    $scope.findParentAndChildObject = function (selectedItem) {
        var findList = [];
        $scope.FindDeepNode($scope.FormModel, selectedItem, findList);

        $scope.$evalAsync(function () {
            var path = $ValidationService.createFullPath($scope.FormModel, findList);
            $scope.selectElement(path);
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
                angular.forEach($scope.validationData.ActiveForms, getListofFiles);
            }
        }
        $ValidationService.checkActiveForm(listOfFiles, model, model.dictAttributes[property], property, 'invalid_active_form', CONST.VALIDATION.INVALID_ACTIVE_FORM, $scope.validationErrorList);
    };
    var copyErrorMessages = function (objSeries, model) {
        if (model && model.errors && !model.errors.hasOwnProperty("series_errors")) model.errors.series_errors = {};
        for (var prop in objSeries.errors) {
            if (model && model.errors && !model.errors.series_errors.hasOwnProperty(prop)) {
                model.errors.series_errors[prop + "_" + objSeries.dictAttributes.Name] = objSeries.errors[prop];
            }
        }
    };
    // #endregion

    $scope.PopulateQueryId = function (isVisible) {

        $scope.$evalAsync(function () {
            $scope.lstQueryID = [];
            var initialload = $scope.FormModel.Elements.filter(function (x) { return x.Name == 'initialload'; });

            if (initialload.length > 0) {
                for (i = 0; i < initialload[0].Elements.length; i++) {
                    if (initialload[0].Elements[i] && initialload[0].Elements[i].dictAttributes && initialload[0].Elements[i].dictAttributes.ID) {
                        $scope.lstQueryID.push(initialload[0].Elements[i]);
                    }
                }
            }

            if ($scope.lstQueryID.length > 0) {
                $scope.SelectedQuery = $scope.lstQueryID[0];
                if (!$scope.MainQuery) {
                    $scope.lstQueryID.splice(0, 0, { Name: "", dictAttributes: { ID: '' } });
                }
                $scope.populateQueryFields($scope.SelectedQuery, isVisible);
            }

            function AddInqueryIDList(item) {
                if (item.dictAttributes.ID) {
                    $scope.queryIDList.push(item.dictAttributes.ID);
                }
            }
            $scope.queryIDList = [];
            angular.forEach($scope.lstQueryID, AddInqueryIDList);
        });
    };

    $scope.toggleSideBar = function () {
        if (!$scope.isDeveloperView) {
            $scope.IsToolsDivCollapsed = !$scope.IsToolsDivCollapsed;
        }
    };

    $scope.ClearSelectedColumns = function () {
        for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
            if ($scope.lstLoadedEntityColumnsTree[i].IsVisible == true) {
                if ($scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield) {
                    $scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield.IsRecordSelected = false;
                    $scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield.IsSelected = "False";
                }
                if ($scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields.length > 0) {
                    for (var j = 0; j < $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields.length; j++) {
                        $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields[j].IsRecordSelected = false;
                        $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields[j].IsSelected = "False";
                    }
                }
                break;
            }
        }
    };
    $scope.populateQueryFieldsFromLookupTree = function (query, isVisible) {
        if (query) {
            $scope.SelectedQuery = query;
            $scope.populateQueryFields(query, isVisible);
        }
        else {
            $scope.SelectedQuery = undefined;
        }
    };

    $scope.onRefreshQueryFields = function () {
        if ($scope.SelectedQuery) {
            $scope.populateQueryFields($scope.SelectedQuery, true);
            $scope.ShowRefreshCompletedDialog();
        }
    }

    $scope.ShowRefreshCompletedDialog = function () {
        var newScope = $scope.$new();
        newScope.strMessage = "Refresh Completed.";
        newScope.isError = true;
        var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html");
        newScope.OkClick = function () {
            dialog.close();
        };
    };

    $scope.populateQueryFields = function (query, isVisible) {
        $scope.ClearSelectedColumns();
        if (query) {
            var objnew;
            for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
                if ($scope.lstLoadedEntityColumnsTree[i].EntityName == query.dictAttributes.ID && $scope.lstLoadedEntityColumnsTree[i].IsQuery == true) {
                    $scope.lstLoadedEntityColumnsTree[i].IsVisible = true;
                    $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields = [];
                    //$scope.entityTreeName = $scope.lstLoadedEntityColumnsTree[i].EntityName;
                    $scope.lookupTreeObject = $scope.lstLoadedEntityColumnsTree[i];
                    objnew = $scope.lstLoadedEntityColumnsTree[i];
                }
                else {
                    //if ($scope.FormModel.IsLookupCriteriaEnabled) {
                    $scope.lstLoadedEntityColumnsTree[i].IsVisible = false;
                    //}
                }
            }
            if (!objnew) {
                objnew = { EntityName: query.dictAttributes.ID, IsVisible: isVisible, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: true, QueryRef: query.dictAttributes.sfwQueryRef, SortedColumns: [] };
                $scope.lstLoadedEntityColumnsTree.push(objnew);
                $scope.lookupTreeObject = objnew;
                // $scope.entityTreeName = query.dictAttributes.ID;
            }
            objnew.SortedColumns = objnew.lstselectedobjecttreefields;
            $scope.ColumnsLimitCount = 50;
            if (query.dictAttributes.sfwQueryRef != undefined && query.dictAttributes.sfwQueryRef != undefined) {
                $.connection.hubForm.server.getEntityQueryColumns(query.dictAttributes.sfwQueryRef, 'LoadQueryFieldsForLookup').done(function (data) {
                    $scope.receiveQueryFields(data, query.dictAttributes.sfwQueryRef);
                });
            }
        }
    };

    $scope.sortcolumnslist = function (objlookuptree, strText) {
        if (strText != "") {
            var lstExactMatchCaseSensitive = [];
            var lstExactMatchCaseInSensitive = [];
            var lstCaseSenesitive = [];
            var lstCaseInsensitive = [];
            var lstContainsCaseSensitive = [];
            var lstContainsCaseInSensitive = [];

            for (var i = 0; i < objlookuptree.lstselectedobjecttreefields.length; i++) {
                if (objlookuptree.lstselectedobjecttreefields[i].ID == strText) {
                    lstExactMatchCaseSensitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                } else if (objlookuptree.lstselectedobjecttreefields[i].ID.toLowerCase() == strText.toLowerCase()) {
                    lstExactMatchCaseInSensitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                } else if (objlookuptree.lstselectedobjecttreefields[i].ID.indexOf(strText) == 0) {
                    lstCaseSenesitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                } else if (objlookuptree.lstselectedobjecttreefields[i].ID.toLowerCase().indexOf(strText.toLowerCase()) == 0) {
                    lstCaseInsensitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                } else if (objlookuptree.lstselectedobjecttreefields[i].ID.contains(strText)) {
                    lstContainsCaseSensitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                } else if (objlookuptree.lstselectedobjecttreefields[i].ID.toLowerCase().contains(strText.toLowerCase())) {
                    lstContainsCaseInSensitive.push(objlookuptree.lstselectedobjecttreefields[i]);
                }
            }
            var lst = lstExactMatchCaseSensitive.concat(lstExactMatchCaseInSensitive).concat(lstCaseSenesitive).concat(lstCaseInsensitive).concat(lstContainsCaseSensitive).concat(lstContainsCaseInSensitive);
            objlookuptree.SortedColumns = lst;
            $scope.ColumnsLimitCount = 50;

        } else {
            objlookuptree.SortedColumns = objlookuptree.lstselectedobjecttreefields;
            $scope.ColumnsLimitCount = 50;
        }
    };

    $scope.receiveQueryFields = function (lstqueryfields, sfwQueryRef) {

        $scope.$apply(function () {
            function iAddSelectedFields(query) {
                var datatype = "";
                if (query.Data1 && query.Data1 != null) {
                    datatype = query.Data1.toLowerCase();
                }
                else if (query.DataType) {
                    datatype = query.DataType.toLowerCase();
                }
                var newquery = { ID: query.CodeID, DisplayName: query.CodeID, Value: query.CodeID, DataType: datatype, Entity: "", Direction: "", IsPrivate: "", Type: "", KeyNo: "", CodeID: "" };
                objnew.lstselectedobjecttreefields.push(newquery);

            }
            if ($scope.lstQueryID) {


                var lst = $scope.lstQueryID.filter(function (itm) {
                    return itm.dictAttributes.sfwQueryRef == sfwQueryRef;
                });
                if (lst && lst.length > 0) {

                    var lst1 = $scope.lstLoadedEntityColumnsTree.filter(function (itm) {
                        return itm.EntityName == lst[0].dictAttributes.ID;
                    });
                    if (lst1 && lst1.length > 0) {
                        var objnew = lst1[0];
                        if (lstqueryfields) {
                            sortListBasedOnproperty(lstqueryfields, "", "CodeID");

                            angular.forEach(lstqueryfields, iAddSelectedFields);
                        }
                    }
                }
            }
        });
    };

    $scope.otherSelectedField = [];
    $scope.RemoveOrIsFieldPresentFromOtherSelectedField = function (objField, param) {
        var isFound = false;
        for (var i = 0; i < $scope.otherSelectedField.length; i++) {
            if ($scope.otherSelectedField[i].ID == objField.ID) {
                if (param == "Delete") {
                    $scope.otherSelectedField.splice(i, 1);
                } else if (param == "bool") {
                    isFound = true;
                }
                break;
            }
        }

        return isFound;
    };

    $scope.selectField = function (field, event) {
        if ($scope.selectedfield && !event.ctrlKey) {
            $scope.selectedfield.IsRecordSelected = false;
            if (!$scope.ischeckboxvisible) {
                $scope.selectedfield.IsSelected = "False";
            }
        } else {
            if ($scope.selectedfield) {
                var isFound = $scope.RemoveOrIsFieldPresentFromOtherSelectedField($scope.selectedfield, "bool");
                if (!isFound) {
                    $scope.otherSelectedField.push($scope.selectedfield);
                }
                $scope.selectedfield = undefined;
            }
        }

        if (!event.ctrlKey) {
            $scope.selectedfield = field;
            $scope.selectedfield.IsRecordSelected = true;
        }
        if (event) {
            event.stopPropagation();
            event.stopImmediatePropagation();
        }

        if (!$scope.ischeckboxvisible) {
            if (event.ctrlKey) {
                if (field.IsRecordSelected) {
                    field.IsRecordSelected = false;
                    field.IsSelected = "False";
                    $scope.RemoveOrIsFieldPresentFromOtherSelectedField(field, "Delete");
                }
                else {
                    field.IsRecordSelected = true;
                    field.IsSelected = "True";
                    $scope.otherSelectedField.push(field);
                }
            }
            else {
                for (var i = 0; i < $scope.otherSelectedField.length; i++) {
                    $scope.otherSelectedField[i].IsRecordSelected = false;
                    $scope.otherSelectedField[i].IsSelected = "False";
                }
                $scope.otherSelectedField = [];
                field.IsRecordSelected = true;
                field.IsSelected = "True";
            }
        }
        for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
            if ($scope.lstLoadedEntityColumnsTree[i].IsVisible == true) {
                $scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield = $scope.selectedfield;
                //$scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields = $scope.otherSelectedField;
                break;
            }
        }

        if ($scope.ObjgridBoundedQuery.IsQuery) {
            $scope.ObjgridBoundedQuery.selectedobjecttreefield = $scope.selectedfield;
        }
    };

    $scope.selectPanelControl = function (obj, e) {
        if (obj) {
            if ($scope.CurrPanel && $scope.CurrPanel != obj) {
                $scope.CurrPanel.IsVisible = false;
                $scope.CurrPanel.initialvisibilty = false;
            }
            if (!obj.isLoaded) {
                obj.isLoaded = true;
            }
            $scope.CurrPanel = obj;
            $scope.IsGridSeleected = false;
            $scope.CurrPanel.IsVisible = true;
            // $scope.CurrPanel.initialvisibilty = true;
            if (!$scope.isAnyPanelOpen) {
                $scope.CurrPanel.initialvisibilty = true;
                $scope.isAnyPanelOpen = true;
            }

            $scope.CurrPanel.IsPanelToggle = !$scope.CurrPanel.IsPanelToggle;
            SetFormSelectedControl($scope.FormModel, $scope.CurrPanel);

            //$scope.FormModel.SelectedControl = $scope.CurrPanel;
            $scope.CurrPanel.IsHeaderTemplate = false;

            var panelName = $scope.GetPanelName(obj);
            $scope.setobjtemplate(panelName);
            var lstPanels = [];
            FindControlListByName($scope.CurrPanel, "sfwPanel", lstPanels);
            FindControlListByName($scope.CurrPanel, "sfwListView", lstPanels);
            FindControlListByName($scope.CurrPanel, "sfwDialogPanel", lstPanels);
            FindControlListByName($scope.CurrPanel, "sfwWizardStep", lstPanels);
            if (lstPanels && lstPanels.length > 0) {
                for (var i = 0; i < lstPanels.length; i++) {
                    lstPanels[i].IsVisible = true;
                    lstPanels[i].initialvisibilty = true;
                    lstPanels[i].isLoaded = true;
                }
            }
            if ($scope.setDisplayNoneToTable) {
                $scope.setDisplayNoneToTable();
            }
            $scope.ObjgridBoundedQuery.IsQuery = false;
        }
    };

    $scope.GetPanelName = function (obj) {
        var strCaption = obj.dictAttributes.ID;
        if (obj.dictAttributes.Title) {
            strCaption = obj.dictAttributes.Title;
        }
        else if (obj.dictAttributes.sfwCaption) {
            strCaption = obj.dictAttributes.sfwCaption;
        }
        return strCaption;
    };

    $scope.setobjtemplate = function (panelName) {
        function SetTabState(sfxtabSheet) {
            var strTabName = sfxtabSheet.dictAttributes.ID;
            if (strTabName.contains("tshAdvCriteria")) {
                $scope.IsLookupAdvCriteriaSelected = true;
            }
            else if (strTabName.contains("tshAdvSort")) {
                $scope.IsLookupAdvSortSelected = true;
            }
            else if (strTabName.contains("tshSql")) {
                $scope.IsLookupQuerySelected = true;
            }
        }
        $scope.FormModel.IsLookupCriteriaEnabled = false;
        $scope.FormModel.IsPrototypeLookupCriteriaEnabled = false;
        if ($scope.FormModel.dictAttributes.sfwType == "Lookup" && !$scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
            var isGridFound = isGridPresentInsidePanel($scope.CurrPanel);
            if ($scope.CurrPanel.dictAttributes.ID && !isGridFound) {
                $scope.FormModel.IsLookupCriteriaEnabled = true;
                $scope.IsGridSeleected = false;

                for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {

                    if ($scope.SelectedQuery && $scope.lstLoadedEntityColumnsTree[i].EntityName == $scope.SelectedQuery.dictAttributes.ID && $scope.lstLoadedEntityColumnsTree[i].IsQuery == true) {

                        $scope.lstLoadedEntityColumnsTree[i].IsVisible = true;
                        $scope.lookupTreeObject = $scope.lstLoadedEntityColumnsTree[i];
                        //$scope.entityTreeName = $scope.lstLoadedEntityColumnsTree[i].EntityName;
                        //$scope.currentEntiyTreeObject = $scope.lstLoadedEntityColumnsTree[i];
                    }
                    else {
                        $scope.lstLoadedEntityColumnsTree[i].IsVisible = false;
                    }
                }
            }
        }

        if ($scope.FormModel.dictAttributes.sfwType == "Lookup") {
            if ($scope.CurrPanel && $scope.CurrPanel.TableVM) {
                var rowVM = $scope.CurrPanel.TableVM.Elements[0];

                var sfxCellVM = rowVM.Elements[0];
                if (sfxCellVM.Elements.length > 0 && sfxCellVM.Elements[0].Name == "sfwTabContainer") {
                    var sfxTabContainerModel = sfxCellVM.Elements[0];


                    if (sfxTabContainerModel) {
                        var tabsVM = sfxTabContainerModel.Elements[0];
                        if (tabsVM) {

                            angular.forEach(tabsVM.Elements, SetTabState);
                        }
                    }

                }
            }
        }

        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
            for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                if ($scope.lstLoadedEntityTrees[i].EntityName == $scope.FormModel.dictAttributes.sfwEntity && !$scope.lstLoadedEntityTrees[i].IsQuery) {

                    $scope.lstLoadedEntityTrees[i].IsVisible = true;
                    $scope.entityTreeName = $scope.lstLoadedEntityTrees[i].EntityName;
                    $scope.currentEntiyTreeObject = $scope.lstLoadedEntityTrees[i];
                }
                else {
                    $scope.lstLoadedEntityTrees[i].IsVisible = false;
                }
            }
        }
        if ($scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
            var isGridFound = isGridPresentInsidePanel($scope.CurrPanel);
            if ($scope.CurrPanel.dictAttributes.ID && !isGridFound) {
                $scope.FormModel.IsPrototypeLookupCriteriaEnabled = true;
            }
        }
    };

    $scope.SetMainTable = function () {
        function iLoopMainTable(row) {
            row.ParentVM = $scope.SfxMainTable;
            if (row.Elements.length > 0) {
                if ($scope.FormModel.dictAttributes.sfwType == "Wizard") {
                    for (k = 0; k < row.Elements.length; k++) {
                        var column = row.Elements[k];
                        column.ParentVM = row;
                        var panel = column.Elements.filter(function (x) {
                            return x.Name == "sfwWizard";
                        });

                        if (panel.length > 0) {
                            $scope.objWizard = panel;
                            panel = panel[0];
                            $scope.objWizard = panel;
                            panel.ParentVM = column;

                            var wizardstep = panel.Elements.filter(function (x) {
                                return x.Name == "WizardSteps";
                            });

                            var HeaderTemplate = panel.Elements.filter(function (x) {
                                return x.Name == "HeaderTemplate";
                            });
                            if (HeaderTemplate.length > 0) {
                                $scope.HeaderTemplate = HeaderTemplate[0];
                            }

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
                                        //if ((step.dictAttributes.ID != "pnltoolbar" || wizardstep.Elements.length == 1) && !$scope.isAnyPanelOpen) {
                                        //    step.initialvisibilty = true;
                                        //    $scope.isAnyPanelOpen = true;
                                        //}
                                        $scope.MainPanels.push(step);
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
                            //if (panel.dictAttributes.ID != "pnltoolbar" && !$scope.isAnyPanelOpen) {
                            //    panel.initialvisibilty = true;
                            //    $scope.isAnyPanelOpen = true;
                            //}
                            $scope.MainPanels.push(panel);

                        }

                    });
                }
            }
        }

        for (var ielem = 0; ielem < $scope.FormModel.Elements.length; ielem++) {
            if ($scope.FormModel.Elements[ielem].Name == "sfwTable") {
                $scope.SfxMainTable = $scope.FormModel.Elements[ielem];
                angular.forEach($scope.SfxMainTable.Elements, iLoopMainTable);

            }
            else if ($scope.FormModel.Elements[ielem].Name == "initialload") {
                $scope.InitialLoad = $scope.FormModel.Elements[ielem];
            }

            else if ($scope.FormModel.Elements[ielem].Name == "centerleft") {
                $scope.CenterLeft = $scope.FormModel.Elements[ielem];
            }

            else if ($scope.FormModel.Elements[ielem].Name == "validatenew") {
                $scope.ValidateNew = $scope.FormModel.Elements[ielem];
            }
        }
    };

    // Iterating UserControl from each table
    $scope.FindDeepNodeUDC = function (objParentElements, pathToObject) {
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                if (item.Name == "udc") {
                    pathToObject.push(item);
                }

                if (item.Elements && item.Elements.length > 0) {
                    $scope.FindDeepNodeUDC(item, pathToObject);
                }
            });
        }
    };

    $scope.SetUDCMainTable = function () {
        var fileList = [];
        if ($scope.UDCTableList && $scope.UDCTableList.length > 0) {
            for (var i = 0; i < $scope.UDCTableList.length > 0; i++) {
                var obj = { FileName: $scope.UDCTableList[i].dictAttributes.Name, ID: $scope.UDCTableList[i].dictAttributes.ID };
                fileList.push(obj);
            }
        }

        $.connection.hubForm.server.getUserControlModel(fileList, "").done(function (udcFileList) {
            $scope.receiveUcMainTable(udcFileList);
        });
    };

    hubMain.server.populateMessageList().done(function (lstMessages) {
        $scope.lstMessages = lstMessages;
    });

    //#endregion

    // for find in design check condition before setting to false - if its not there dont set the property      
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        if (items[0] && items[0].Name) {
            items[items.length - 1].isAdvanceSearched = IsAction;
        }
    };

    // ================================================= property highlighter was here      
    function addHighlightBox(objElement, strResponseTemplate, strAttributeName) {
        var highlightbox = $(strResponseTemplate);
        var HighlightRow = highlightbox.find(".HighlightRowWrapper");
        switch (strAttributeName.toLowerCase()) {
            case "sfwvisiblerule": HighlightRow.append("<span title='" + $(objElement).attr("sfwVisibleRule") + "' class='blk-blue'></span>"); break;
            case "sfwselectcolvisiblerule": HighlightRow.append("<span title='" + $(objElement).attr("sfwSelectColVisibleRule") + "' class='blk-blue'></span>"); break;
            case "sfwautoquery": HighlightRow.append("<span title='" + $(objElement).attr("sfwAutoQuery") + "' class='blk-red'></span>"); break;
            case "sfwretrievalquery": HighlightRow.append("<span title='" + $(objElement).attr("sfwRetrievalQuery") + "' class='blk-green'></span>"); break;
            case "sfwreadonlyrule": HighlightRow.append("<span title='" + $(objElement).attr("sfwReadOnlyRule") + "' class='blk-purple'></span>"); break;
            case "sfwvalidationrules": HighlightRow.append("<span title='" + $(objElement).attr("sfwValidationRules") + "' class='blk-brown'></span>"); break;
        }
        $(objElement).append(highlightbox);
    }
    $scope.receieveHTMLFromModel = function (strHtml) {
        if (strHtml) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = false;
            });
            $scope.PrepareHighlightedForm(strHtml);
        }
    };
    $scope.PrepareHighlightedForm = function (strHtml) {
        $("#" + $scope.currentfile.FileName).find("#transformedxml")[0].innerHTML = strHtml;
        var highlighttemplatePath = [window.location.protocol, "//", window.location.host, "/", getWebApplicationName(), "/", "Form/views/highlightElementTemplate.html"].join("");
        // get highlight template
        $.ajax({
            url: highlighttemplatePath,
            type: 'GET',
            async: false,
            success: function (response) {
                if ($scope.DesignSearchAttributes.indexOf("sfwVisibleRule") != -1) {
                    var sfwvisibleruleElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwVisibleRule]");
                    var sfwSelectColVisibleRuleElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwSelectColVisibleRule]");
                    sfwvisibleruleElements.each(function () {
                        addHighlightBox(this, response, "sfwvisiblerule");
                    });
                    sfwSelectColVisibleRuleElements.each(function () {
                        var existingBox = $(this).find("> .highlightElement .HighlightRowWrapper");
                        if (existingBox.length > 0) {
                            existingBox.append("<span title='" + $(this).attr("sfwSelectColVisibleRule") + "' class='blk-blue'></span>");
                        }
                        else {
                            addHighlightBox(this, response, "sfwSelectColVisibleRule");
                        }
                    });
                }
                // check if these elements contains highlight box already - if no append it otherwise append only the attribute span
                if ($scope.DesignSearchAttributes.indexOf("sfwAutoQuery") != -1) {
                    var listsfwAutoQueryElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwAutoQuery]");
                    listsfwAutoQueryElements.each(function () {
                        var existingBox = $(this).find("> .highlightElement .HighlightRowWrapper");
                        if (existingBox.length > 0) {
                            existingBox.append("<span title='" + $(this).attr("sfwAutoQuery") + "' class='blk-red'></span>");
                        }
                        else {
                            addHighlightBox(this, response, "sfwAutoQuery");
                        }
                    });
                }
                if ($scope.DesignSearchAttributes.indexOf("sfwRetrievalQuery") != -1) {
                    var listsfwRetrievalQueryElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwRetrievalQuery]");
                    listsfwRetrievalQueryElements.each(function () {
                        var existingBox = $(this).find("> .highlightElement .HighlightRowWrapper");
                        if (existingBox.length > 0) {
                            existingBox.append("<span title='" + $(this).attr("sfwRetrievalQuery") + "' class='blk-green'></span>");
                        }
                        else {
                            addHighlightBox(this, response, "sfwRetrievalQuery");
                        }
                    });
                }
                if ($scope.DesignSearchAttributes.indexOf("sfwReadOnlyRule") != -1) {
                    var listsfwReadOnlyRuleElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwReadOnlyRule]");
                    listsfwReadOnlyRuleElements.each(function () {
                        var existingBox = $(this).find("> .highlightElement .HighlightRowWrapper");
                        if (existingBox.length > 0) {
                            existingBox.append("<span title='" + $(this).attr("sfwReadOnlyRule") + "' class='blk-purple'></span>");
                        }
                        else {
                            addHighlightBox(this, response, "sfwReadOnlyRule");
                        }
                    });
                }
                if ($scope.DesignSearchAttributes.indexOf("sfwValidationRules") != -1) {
                    var listsfwValidationRulesElements = $("#" + $scope.currentfile.FileName).find("#transformedxml [sfwValidationRules]");
                    listsfwValidationRulesElements.each(function () {
                        var existingBox = $(this).find("> .highlightElement .HighlightRowWrapper");
                        if (existingBox.length > 0) {
                            existingBox.append("<span title='" + $(this).attr("sfwValidationRules") + "' class='blk-brown'></span>");
                        }
                        else {
                            addHighlightBox(this, response, "sfwValidationRules");
                        }
                    });
                }
            }
        });
    };
    $scope.getHightlightAttribute = function () {
        var dialogScope = $scope.$new(true);
        dialogScope.attributesList = [{ label: "Visible Rule", value: "sfwVisibleRule" }, { label: "Auto Query", value: "sfwAutoQuery" }, { label: "Retrieval Query", value: "sfwRetrievalQuery" }, { label: "Readonly Rule", value: "sfwReadOnlyRule" }, { label: "Validation Rules", value: "sfwValidationRules" }];
        dialogScope.AllFilesObj = { name: "All", value: "All", selected: false };
        if ($scope.DesignHighlightAttribute == null && !$scope.isDeveloperView) {
            // first time from normal mode to analyst view
            $scope.DesignHighlightAttribute = {};
            dialogScope.DesignAttribute = {};
        }
        else {
            dialogScope.DesignAttribute = {};
            angular.copy($scope.DesignHighlightAttribute, dialogScope.DesignAttribute);
        }
        dialogScope.AttributeDialog = $rootScope.showDialog(dialogScope, "Select attributes to highlight", "Form/views/highlightAttribute.html", { width: 300, height: 500, showclose: true });
        dialogScope.onOKClick = function () {
            angular.copy(dialogScope.DesignAttribute, $scope.DesignHighlightAttribute);
            dialogScope.AttributeDialog.close();
            var highlightthis = dialogScope.attributesList.filter(function (item) {
                return dialogScope.DesignAttribute[item.value];
            });
            dialogScope.$destroy();
            $scope.DesignSearchAttributes = [];
            highlightthis.forEach(function (item) {
                $scope.DesignSearchAttributes.push(item.value);
            });
            if ($scope.DesignSearchAttributes.length > 0) {
                // ================================================= get xml from from form object      
                var objFormModel = GetBaseModel($scope.FormModel);
                if (objFormModel != "") {
                    var strobj = JSON.stringify(objFormModel);
                    if (strobj.length < 32000) {
                        $rootScope.IsLoading = true;
                        hubcontext.hubForm.server.getHTMLfromModel(strobj, $scope.currentfile).done(function (data) {
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });
                            $scope.PrepareHighlightedForm(data);
                        });
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
                        $rootScope.IsLoading = true;
                        SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Highlight_XML");
                    }
                }
                // =================================================                 
                $scope.blackStyle = { 'height': '99%', 'overflow': 'auto', 'margin': '10px', 'padding-top': '10px' };
                $scope.IsToolsDivCollapsed = true;
                $scope.isDeveloperView = true;
            }
            else {
                $("#" + $scope.currentfile.FileName).find("#transformedxml")[0].innerHTML = "";
                $scope.isDeveloperView = false;
                $scope.blackStyle = null;
            }
        };
        dialogScope.selectAll = function () {
            if (dialogScope.AllFilesObj.selected) {
                dialogScope.attributesList.filter(function (item) {
                    dialogScope.DesignAttribute[item.value] = true;
                });
            }
            else {
                dialogScope.attributesList.filter(function (item) {
                    dialogScope.DesignAttribute[item.value] = false;
                });
            }
        };
        dialogScope.close = function () {
            $scope.$evalAsync(function () {

                dialogScope.AttributeDialog.close();
                dialogScope.$destroy();
            });
        };
    };
    $scope.OnSwitchView = function () {
        $scope.$evalAsync(function () {
            $scope.blackStyle = null;
            $scope.isDeveloperView = false;
        });
        $("#" + $scope.currentfile.FileName).find("#transformedxml")[0].innerHTML = "";
        $scope.DesignHighlightAttribute = null;
    };
    // ================================================= property highlighter was here      

    //#region Xml source Start
    $rootScope.isFromSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            //$scope.selectedDesignSource = false;
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
                $scope.receiveformcorrdesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;

                    if ($scope.isSourceDirty) {
                        var formmodel = data;

                        $scope.receiveformmodel(formmodel);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}

                    //Commented following code, because source to design synchronization is hold for now.
                    $scope.selectElement(path);
                };
            }
        }
    };
    $scope.selectElement = function (path) {
        $("#" + $scope.currentfile.FileName).find("[drag-handler-btn]").remove();
        $scope.$evalAsync(function () {
            if ($scope.FormModel.dictAttributes.sfwType == "UserControl") {
                if (path != "form") {
                    var items = [];
                    items = $scope.objectPath(path);
                    if (items.length > 0) {
                        $scope.highlightControl(items);
                    }
                }
            }
            else {
                if ($scope.CurrPanel) {
                    $scope.CurrPanel.IsVisible = false;
                    $scope.CurrPanel.IsPanelToggle = false;
                    $scope.CurrPanel.initialvisibilty = false;
                }
                var PanelControl;
                if (path != null && path != "form") {
                    var items = [];
                    items = $scope.objectPath(path);
                    if (items && items.length > 0) {
                        if (items.length > 3 && items[0].Name != "initialload" && items[0].Name != "validatenew" && items[items.length - 1].Name != "SideBarTemplate" && items[items.length - 1].ParentVM.Name != "SideBarTemplate") {
                            $scope.highlightControl(items);
                        }
                        else {
                            //Select Main Panel Control, If user Clicked on initialload,validatenew
                            if ($scope.MainPanels.length > 1 && $scope.MainPanels[0].dictAttributes.ID == "pnltoolbar") {
                                PanelControl = $scope.MainPanels[1];
                            }
                            else {
                                PanelControl = $scope.MainPanels[0];
                            }

                            //If there are only three levels, It means it is Table-Row-Column level
                            //Then if Column is selected then selects the Panel inside of that column
                            if (items[items.length - 1] && items[items.length - 1].Name == "sfwColumn" && $scope.FormModel.dictAttributes.sfwType != "Wizard") {
                                PanelControl = items[items.length - 1].Elements[0];
                            }
                            else if (items[items.length - 1] && items[items.length - 1].Name == "sfwRow" && $scope.FormModel.dictAttributes.sfwType != "Wizard") {
                                PanelControl = items[items.length - 1].Elements[0].Elements[0];
                            }
                            else if (items[items.length - 1] && items[items.length - 1].Name == "WizardSteps" || items[items.length - 1].Name == "sfwWizard") {
                                if (items[items.length - 1].Name == "WizardSteps") {
                                    PanelControl = items[items.length - 1].Elements[0];
                                }
                                else {
                                    PanelControl = items[items.length - 1].Elements[0].Elements[0];
                                }
                            }
                            else if (items[items.length - 1] && items[items.length - 1].Name == "HeaderTemplate" && $scope.FormModel.dictAttributes.sfwType == "Wizard") {
                                PanelControl = items[items.length - 1];
                            }
                            $scope.setLookupCriteriaVisiblityVariable(PanelControl);
                            if (PanelControl) {
                                $scope.MainPanels.some(function (panel) {
                                    if (panel.dictAttributes.ID == PanelControl.dictAttributes.ID) {

                                        $scope.CurrPanel.IsVisible = false;
                                        $scope.CurrPanel.IsPanelToggle = true;

                                        $scope.CurrPanel = panel;
                                        $scope.CurrPanel.IsVisible = false;
                                        $scope.CurrPanel.IsPanelToggle = false;

                                        panel.initialvisibilty = true;
                                        panel.IsVisible = true;
                                        panel.isLoaded = true;
                                    }
                                });
                            }

                            SetFormSelectedControl($scope.FormModel, PanelControl, window.event);
                            setEntity($scope.FormModel.SelectedControl);
                        }
                    }
                }
                else {
                    if ($scope.MainPanels.length > 1 && $scope.MainPanels[0].dictAttributes.ID == "pnltoolbar") {
                        PanelControl = $scope.MainPanels[1];
                    }
                    else { PanelControl = $scope.MainPanels[0]; }

                    if ($scope.CurrPanel && PanelControl) {
                        $scope.CurrPanel.IsVisible = false;
                        $scope.CurrPanel.IsPanelToggle = true;

                        $scope.CurrPanel = PanelControl;
                        $scope.CurrPanel.IsVisible = false;
                        $scope.CurrPanel.IsPanelToggle = false;
                    }
                    $scope.setLookupCriteriaVisiblityVariable(PanelControl);
                    if (PanelControl) {
                        PanelControl.initialvisibilty = true;
                        PanelControl.IsVisible = true;
                        PanelControl.isLoaded = true;
                        SetFormSelectedControl($scope.FormModel, PanelControl, window.event);
                        setEntity($scope.FormModel.SelectedControl);
                    }
                }
            }
            $rootScope.IsLoading = false;
            $timeout(function () {
                var mainDiv = $("#" + $scope.FormModel.dictAttributes.ID);
                var selElem = $(mainDiv).find(".select-label-control,.select-textbox-control,.select-button-control,.select-hyperlink-control,.select-gridview-control,.select-cell-control,.select-dropdown-control,.select-cascading-dropdown-control");

                if (mainDiv && selElem && selElem.length) {
                    $(mainDiv).find('.form-area-editable').animate({ scrollTop: $(selElem).offset().top - 300 }, 100);
                    $(mainDiv).find('.form-area-editable').animate({ scrollLeft: $(selElem).offset().left - 400 }, 100);
                }
            });
        });
    };

    $scope.setLookupCriteriaVisiblityVariable = function (PanelControl) {
        $scope.FormModel.IsLookupCriteriaEnabled = false;
        if ($scope.FormModel.dictAttributes.sfwType == "Lookup" || $scope.FormModel.dictAttributes.sfwType == "FormLinkLookup") {
            var isGridFound = isGridPresentInsidePanel(PanelControl);
            if (PanelControl && PanelControl.dictAttributes && PanelControl.dictAttributes.ID && !isGridFound) {
                $scope.FormModel.IsLookupCriteriaEnabled = true;
                $scope.IsGridSeleected = false;
            }
        }
        $scope.FormModel.IsPrototypeLookupCriteriaEnabled = false;
        if ($scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
            var isGridFound = isGridPresentInsidePanel(PanelControl);
            if (PanelControl && PanelControl.dictAttributes && PanelControl.dictAttributes.ID && !isGridFound) {
                $scope.FormModel.IsPrototypeLookupCriteriaEnabled = true;
                $scope.FormModel.IsLookupCriteriaEnabled = false;
            }
        }
    };

    $scope.objectPath = function (path) {
        var items = [];
        var objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.FormModel;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                if (objHierarchy) {
                    items.push(objHierarchy);
                }
            }
        }
        return items;
    };

    $scope.highlightControl = function (items) {
        var PanelControl, SelectedControlInSource;
        for (var i = 0; i < items.length; i++) {
            var obj = items[i];
            if (obj.Name == "sfwPanel" || obj.Name == "sfwWizardStep" || obj.Name == "HeaderTemplate") {
                PanelControl = obj;
                var IsMainPanel = false;
                if (PanelControl && PanelControl.Name != "HeaderTemplate" && $scope.MainPanels.length > 0) {
                    $scope.MainPanels.some(function (panel) {
                        if (panel.dictAttributes.ID == PanelControl.dictAttributes.ID) {

                            IsMainPanel = true;
                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = true;

                            $scope.CurrPanel = panel;
                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = false;

                            panel.initialvisibilty = true;
                            panel.IsVisible = true;
                            panel.isLoaded = true;

                            // $scope.setobjtemplate($scope.CurrPanel.dictAttributes.ID);
                            $scope.FormModel.IsLookupCriteriaEnabled = false;
                            if (($scope.FormModel.dictAttributes.sfwType == "Lookup" || $scope.FormModel.dictAttributes.sfwType == "FormLinkLookup") && !$scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                                var isGridFound = isGridPresentInsidePanel($scope.CurrPanel);
                                if ($scope.CurrPanel.dictAttributes.ID && !isGridFound) {
                                    $scope.FormModel.IsLookupCriteriaEnabled = true;
                                    $scope.IsGridSeleected = false;
                                }
                            }
                            $scope.FormModel.IsPrototypeLookupCriteriaEnabled = false;
                            if ($scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                                var isGridFound = isGridPresentInsidePanel($scope.CurrPanel);
                                if ($scope.CurrPanel.dictAttributes.ID && !isGridFound) {
                                    $scope.FormModel.IsPrototypeLookupCriteriaEnabled = true;
                                }
                            }
                        }
                    });
                }
                else if (PanelControl && PanelControl.Name == "HeaderTemplate" && $scope.CurrPanel) {
                    IsMainPanel = true;
                    $scope.CurrPanel.IsVisible = false;
                    $scope.CurrPanel.IsPanelToggle = true;

                    $scope.CurrPanel = PanelControl;
                    $scope.CurrPanel.IsVisible = false;
                    $scope.CurrPanel.IsPanelToggle = false;

                    PanelControl.initialvisibilty = true;
                    PanelControl.IsVisible = true;
                    PanelControl.isLoaded = true;
                }

                if (!IsMainPanel) {
                    PanelControl.initialvisibilty = true;
                    PanelControl.IsVisible = true;
                    PanelControl.isLoaded = true;
                }
            }
            else if (obj.Name == "sfwDialogPanel" || obj.Name == "sfwListView") {
                PanelControl = obj;
                PanelControl.initialvisibilty = true;
                PanelControl.isLoaded = true;
            }
            else if (obj.Name == "sfwTabSheet") {
                $rootScope.isFromSource = true;
                PanelControl = obj;
                PanelControl.isLoaded = true;
                PanelControl.ParentVM.SelectedTabSheet = obj;
                PanelControl.ParentVM.SelectedTabSheet.IsSelected = true;
            }
            else if ((i == items.length - 1) && (obj.Name == "ItemTemplate" || obj.Name == "Columns")) {
                if (obj.Name == "ItemTemplate") {
                    SetFormSelectedControl($scope.FormModel, obj.Elements[0], window.event);
                    setEntity($scope.FormModel.SelectedControl);
                }
                else {
                    SetFormSelectedControl($scope.FormModel, items[items.length - 2], window.event);
                    setEntity($scope.FormModel.SelectedControl);
                }
            }
            else if ((i == items.length - 1) && (obj.Name == "sfwTabContainer" || obj.Name == "Tabs")) {
                $rootScope.isFromSource = false;
                if (obj.Name == "sfwTabContainer") {
                    SetFormSelectedControl($scope.FormModel, obj.Elements[0].Elements[0], window.event);
                    setEntity($scope.FormModel.SelectedControl);
                }
                else {
                    SetFormSelectedControl($scope.FormModel, obj.Elements[0], window.event);
                    setEntity($scope.FormModel.SelectedControl);
                }
            }
            else if ((i == items.length - 1) && (obj.Name == "sfwRow")) {
                var ParentFound;
                while (obj.ParentVM) {
                    ParentFound = false;
                    if (obj.ParentVM.Name == "sfwPanel" || obj.ParentVM.Name == "sfwWizardStep" || obj.ParentVM.Name == "HeaderTemplate") {
                        SetFormSelectedControl($scope.FormModel, obj.ParentVM, window.event);
                        setEntity($scope.FormModel.SelectedControl);
                        ParentFound = true;
                        break;
                    }
                    else if (obj.ParentVM.Name == "sfwDialogPanel") {
                        SetFormSelectedControl($scope.FormModel, obj.ParentVM, window.event);
                        setEntity($scope.FormModel.SelectedControl);
                        ParentFound = true;
                        break;
                    }
                    else if (obj.ParentVM.Name == "sfwListView") {
                        SetFormSelectedControl($scope.FormModel, obj.ParentVM, window.event);
                        setEntity($scope.FormModel.SelectedControl);
                        ParentFound = true;
                        break;
                    }
                    else if (obj.ParentVM.Name == "sfwTabSheet") {
                        SetFormSelectedControl($scope.FormModel, obj.ParentVM, window.event);
                        setEntity($scope.FormModel.SelectedControl);
                        ParentFound = true;
                        break;
                    }
                    obj = obj.ParentVM;
                }

                if (!ParentFound && $scope.MainPanels.length > 0) {
                    var PanelControl;
                    if ($scope.MainPanels.length > 1 && $scope.MainPanels[0].dictAttributes.ID == "pnltoolbar") {
                        PanelControl = $scope.MainPanels[1];
                    }
                    else {
                        PanelControl = $scope.MainPanels[0];
                    }

                    $scope.MainPanels.some(function (panel) {
                        if (panel.dictAttributes.ID == PanelControl.dictAttributes.ID) {

                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = true;

                            $scope.CurrPanel = panel;
                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = false;

                            panel.initialvisibilty = true;
                            panel.IsVisible = true;
                            panel.isLoaded = true;
                        }
                    });
                }
            }
            else if ((i == items.length - 1) && (obj.Name == "WizardSteps" || obj.Name == "sfwWizard")) {
                if (obj.Name == "WizardSteps") {
                    PanelControl = obj.Elements[0];
                }
                else {
                    PanelControl = obj.Elements[0].Elements[0];
                }
                if ($scope.MainPanels.length > 0) {
                    $scope.MainPanels.some(function (panel) {
                        if (panel.dictAttributes.ID == PanelControl.dictAttributes.ID) {

                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = true;

                            $scope.CurrPanel = panel;
                            $scope.CurrPanel.IsVisible = false;
                            $scope.CurrPanel.IsPanelToggle = false;

                            panel.initialvisibilty = true;
                            panel.IsVisible = true;
                            panel.isLoaded = true;
                        }
                    });
                }

                SetFormSelectedControl($scope.FormModel, PanelControl, window.event);
                setEntity($scope.FormModel.SelectedControl);
                break;
            }
            else if ((i == items.length - 1) && (obj.Name == "Area3DStyle" || obj.Name == "sfwChartArea"
                || obj.Name == "ChartAreas" || obj.Name == "Series" || obj.Name == "sfwSeries")) {
                while (obj.Name != "sfwChart") {
                    obj = obj.ParentVM;
                }
                SetFormSelectedControl($scope.FormModel, obj, window.event);
                setEntity($scope.FormModel.SelectedControl);
                break;
            }

            if (i == items.length - 1 && obj.Name != "ItemTemplate" && obj.Name != "Columns"
                && obj.Name != "sfwTabContainer" && obj.Name != "Tabs" && obj.Name != "sfwRow"
                && obj.Name != "Area3DStyle" && obj.Name != "sfwChartArea"
                && obj.Name != "ChartAreas" && obj.Name != "Series" && obj.Name != "sfwSeries") {
                SetFormSelectedControl($scope.FormModel, obj, window.event);
                setEntity($scope.FormModel.SelectedControl);
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
        if ($scope.selectedDesignSource == false && !$scope.isDeveloperView) {
            if ($scope.FormModel != null && $scope.FormModel != undefined) {
                if (!$scope.IsPrototype && $scope.FormModel.dictAttributes.sfwType != "UserControl" && $scope.FormModel.dictAttributes.sfwType != "Lookup") {

                    $scope.dummyLstLoadDetails = LoadDetails($scope.FormModel, $scope.objLoadDetails, false, $rootScope, false);
                }
            }
            $scope.selectedDesignSource = true;
            if ($scope.FormModel != null && $scope.FormModel != undefined) {
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();

                //usercontrol and tooltip will be supported by fwk later

                var objreturn1 = GetBaseModel($scope.FormModel);
                if (objreturn1 != "") {

                    var nodeId = [];
                    var nodes = [];
                    var indexes = [];
                    var formSelectedControl;
                    $rootScope.IsLoading = true;
                    var sObj;
                    var indexPath = [];
                    var pathString;

                    var formSelectedControl = $scope.FormModel.SelectedControl;

                    if (formSelectedControl) {
                        var pathToObject = [];
                        //sObj = FindDeepNode($scope.FormModel, formSelectedControl);
                        //pathString = getPathSource(sObj, indexPath);
                        //angular.copy(pathString.reverse(), nodeId);
                        sObj = $scope.FindDeepNode($scope.FormModel, formSelectedControl, pathToObject);
                        pathString = $scope.getPathSource($scope.FormModel, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }

                    var strobj = JSON.stringify(objreturn1);
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
                                //$scope.FormModel.SelectedControl = undefined;
                                //$scope.CurrPanel.IsPanelToggle = false;
                                //$scope.CurrPanel = undefined;
                                $rootScope.isFromSource = false;
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
            if (newObj) {
                newObj.ParentVM = objParentElements;
            }
            return newObj;
        }
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                //item.ParentVM = objParentElements
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

    //#region Common Functions

    $scope.OnSelectLeftFormTab = function (opt) {
        if (opt == 'Entity') {
            $scope.ActiveTabForForm = 'Entity';
            if (!$scope.IsEntityTreeExpanded) {
                $scope.IsEntityTreeExpanded = true;
                if ($scope.FormModel) {
                    if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                        if ($scope.currentEntiyTreeObject && $scope.currentEntiyTreeObject.lstselectedobjecttreefields.length > 0) {
                            $scope.currentEntiyTreeObject.selectedobjecttreefield = $scope.currentEntiyTreeObject.lstselectedobjecttreefields[0];
                            $scope.currentEntiyTreeObject.selectedobjecttreefield.IsRecordSelected = true;
                            $scope.currentEntiyTreeObject.selectedobjecttreefield.IsSelected = "True";
                        }
                    }
                    else if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType == 'Lookup' && $scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        if ($scope.lstselectedobjecttreefields.length > 0) {
                            $scope.selectedobjecttreefield = $scope.lstselectedobjecttreefields[0];
                            $scope.selectedobjecttreefield.IsRecordSelected = true;
                            $scope.selectedobjecttreefield.IsSelected = "True";
                        }
                    }
                    else {
                        if ($scope.lookupTreeObject && $scope.lookupTreeObject.SortedColumns && $scope.lookupTreeObject.SortedColumns.length > 0) {
                            $scope.selectedfield = $scope.lookupTreeObject.SortedColumns[0];
                            $scope.selectedfield.IsRecordSelected = true;
                            $scope.selectedfield.IsSelected = "True";
                        }
                    }
                }

            }

        }
        else if (opt == 'Properties') {
            $scope.ActiveTabForForm = 'Properties';
            if (!$scope.IsPropsExpanded) {
                $scope.IsPropsExpanded = true;
            }
        }
        else {
            $scope.ActiveTabForForm = 'Toolbox';
        }

    };

    $scope.GetMainPanelID = function () {
        if ($scope.FormModel) {
            return "MainParentPanel" + $scope.FormModel.dictAttributes.ID;
        }
    };

    $scope.showOtherControl = false;

    $scope.showHTMLControl = false;

    $scope.ToggleOtherControlmenudropdown = function () {
        $scope.showHTMLControl = false;
        $scope.showOtherControl = !$scope.showOtherControl;
    };

    $scope.ToggleHTMLControlmenudropdown = function () {
        $scope.showOtherControl = false;
        $scope.showHTMLControl = !$scope.showHTMLControl;
    };


    $scope.InitialLoadSection = function () {
        $scope.SubQueryCollection = [];
        function iteration(objcustommethod) {
            if (objcustommethod.Name == "callmethods") {
                if (!objcustommethod.dictAttributes.sfwMode) {
                    $scope.SelectedNewMethod = objcustommethod;
                    $scope.SelectedUpdateMethod = "";
                    $scope.FormModel.IsSameAsNew = true;
                }
                if (objcustommethod.dictAttributes.sfwMode == 'New' || objcustommethod.dictAttributes.sfwMode == 'All') {
                    $scope.SelectedNewMethod = objcustommethod;
                }
                if (objcustommethod.dictAttributes.sfwMode == 'Update' || objcustommethod.dictAttributes.sfwMode == 'All') {
                    $scope.SelectedUpdateMethod = objcustommethod;
                }
            }
            else if (objcustommethod.Name == "query") {
                var strQuery = objcustommethod.dictAttributes.sfwQueryRef;
                if ($scope.IsSubQuery(strQuery)) {

                    $scope.SubQueryCollection.push(objcustommethod);
                    $scope.SelectedSubQuery = objcustommethod;
                }

                else if (objcustommethod.dictAttributes.sfwQueryRef) {
                    $scope.MainQuery = objcustommethod;

                    //****** NEED TO IMPLEMENT*******//

                    //foreach (CustomMethodDetails strNew in LoadMethodCollection)
                    //{
                    //    if (strNew.Description.StartsWith(objcustommethod[ApplicationConstants.XMLFacade.SFWRETURNTYPE] + " " + objcustommethod[ApplicationConstants.XMLFacade.SFWMETHODNAME]))
                    //{
                    //    SelectedLoadMethod = strNew.Name;
                    //    break;
                    //}

                }
            }
            else if (objcustommethod.Name == "session") {
                $scope.SessionFields = objcustommethod;
                $scope.SessionFields.lstselectedobjecttreefields = [];
                $scope.SessionFields.LstDisplayedEntities = [];
            }
        }
        if ($scope.InitialLoad) {
            angular.forEach($scope.InitialLoad.Elements, iteration);
        }
        else {
            $scope.MainQuery = undefined;
            $scope.SelectedQuery = undefined;
        }
        if ($scope.FormModel.dictAttributes.sfwType == "Maintenance") {
            if ($scope.InitialLoad) {
                if ($scope.SessionFields == undefined) {
                    $scope.SessionFields = {
                        Name: 'session', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    $scope.SessionFields.lstselectedobjecttreefields = [];
                    $scope.SessionFields.LstDisplayedEntities = [];
                }
            }
        }
    };

    $scope.IsSubQuery = function (strQuery) {
        var retValue = false;
        function iIssubselectquery(Query) {
            if (!retValue) {
                if (Query.ID == strQueryName && Query.QueryType && Query.QueryType.toLowerCase() == "subselectquery") {
                    retValue = true;
                }
            }
        }
        if (strQuery != "" && strQuery != undefined) {
            var strCDOName = strQuery.substring(0, strQuery.indexOf("."));
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lstObj = entityIntellisenseList.filter(function (x) {
                return x.ID == strCDOName;
            });
            if (lstObj && lstObj.length > 0) {

                var strQueryName = strQuery.substring(strQuery.indexOf(".") + 1);

                angular.forEach(lstObj[0].Queries, iIssubselectquery);
            }
        }

        return retValue;
    };



    //#endregion

    //#region Add Control From Object Tree

    $scope.AddControlFromObjectTree = function (cntrlClassName) {

        function getDropDownList(obj) {
            if (obj.Value && endsWith(obj.Value.toLowerCase(), "_description")) {
                message = obj.Value;
                descriptionList.push(obj);
            }
        }
        function getSelectedList(obj) {
            lstselectedfields.splice(obj, 1);
        }

        function iteratorisInvalidExpression(obj) {
            if (obj.Type == "Expression") {
                if (cntrlClassName != "sfwLabel") {
                    isInvalidExpressionMapping = true;
                    obj.IsChecked = false;
                    lstselectedfields.splice(obj, 1);
                }
            }
        }

        function iteratorAddActualControl(field) {
            var isGrid = true;
            if (objcontrolVM && objcontrolVM.Name == "sfwButtonGroup") {
                isGrid = false;
            } else {
                isGrid = true;
            }
            $scope.AddActualControl(blnIsLookup, cntrlClassName, field, false, objcontrolVM, isGrid);
        }

        function iteratorAddsfwGridView(field) {
            var prefix = "asp";

            var aTemplateField = {
                Name: "TemplateField", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
            };
            aTemplateField.ParentVM = columnvm;
            aTemplateField.dictAttributes.SortExpression = field.ID;
            aTemplateField.dictAttributes.HeaderText = GetCaptionFromField(field);

            var aItemTemplate = {
                Name: "ItemTemplate", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
            };
            aItemTemplate.ParentVM = aTemplateField;

            $scope.AddActualControl(blnIsLookup, cntrlClassName, field, false, aItemTemplate, true);

            aTemplateField.Elements.push(aItemTemplate);

            $rootScope.PushItem(aTemplateField, columnvm.Elements);
        }
        if (cntrlClassName) {

            var objcontrolVM = $scope.GetSfxControlVM();
            var objGrid = FindParent(objcontrolVM, "sfwGridView");
            if (!objcontrolVM) {
                $SgMessagesService.Message('Message', 'Select control in right panel to which elements have to be added.');
            }
            if ($scope.FormModel.dictAttributes.sfwType.toLowerCase() == "maintenance" && objcontrolVM.Name == "sfwGridView" && objcontrolVM.dictAttributes.sfwBoundToQuery && objcontrolVM.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && ["sfwLabel", "sfwHyperLink", "sfwButton", "sfwImage", "sfwToolTipButton"].indexOf(cntrlClassName) <= -1) {
                $SgMessagesService.Message("Not Allowed", "Can not add this control.");
                return;
            }
            if ($scope.FormModel.dictAttributes.sfwType.toLowerCase() == "maintenance" && objGrid && objGrid.dictAttributes.sfwBoundToQuery && objGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && ["sfwLabel", "sfwHyperLink", "sfwButton", "sfwImage", "sfwToolTipButton"].indexOf(cntrlClassName) <= -1) {
                $SgMessagesService.Message("Not Allowed", "Can not add this control.");
                return;
            }
            if ($scope.FormModel.dictAttributes.sfwType.toLowerCase() == "maintenance" && objGrid && objGrid.dictAttributes.sfwBoundToQuery && objGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && ["sfwLabel", "sfwHyperLink", "sfwButton", "sfwImage", "sfwToolTipButton"].indexOf(cntrlClassName) <= -1) {
                $SgMessagesService.Message("Not Allowed", "Can not add this control.");
                return;
            }
            if ($scope.FormModel.dictAttributes.sfwType.toLowerCase() == "lookup" && cntrlClassName != "sfwButton") {
                var objpanel = FindParent(objcontrolVM, "sfwPanel");
                if (objpanel && objcontrolVM.Name == "sfwColumn" && objpanel.dictAttributes.ID == "pnlResult") {
                    $SgMessagesService.Message("Not Allowed", "Can not add any control outside the grid in result panel.");
                    return;
                }
            }
            var selectedobjecttreefield;
            var isCollectionField = true;
            var lstselectedobjecttreefields = [];
            if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                if ($scope.ObjgridBoundedQuery.IsQuery) {
                    selectedobjecttreefield = $scope.ObjgridBoundedQuery.selectedobjecttreefield;
                    lstselectedobjecttreefields = $scope.ObjgridBoundedQuery.lstselectedobjecttreefields;
                } else {
                    for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                        if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {
                            selectedobjecttreefield = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                            lstselectedobjecttreefields = $scope.lstLoadedEntityTrees[i].lstselectedobjecttreefields;
                            break;
                        }
                    }
                }
            } else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                selectedobjecttreefield = $scope.selectedobjecttreefield;
                lstselectedobjecttreefields = $scope.lstselectedobjecttreefields;
            } else {
                for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
                    if ($scope.lstLoadedEntityColumnsTree[i].IsVisible == true) {
                        selectedobjecttreefield = $scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield;
                        lstselectedobjecttreefields = $scope.lstLoadedEntityColumnsTree[i].lstselectedobjecttreefields;
                        break;
                    }
                }
            }

            if (objcontrolVM) {
                switch (cntrlClassName) {
                    case "Grid":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message('Invalid Field', 'Select a valid one-to-many field from entity tree to add grid control.');
                        }
                        else {
                            if (selectedobjecttreefield.DataType == "Collection" || selectedobjecttreefield.DataType == "List" || selectedobjecttreefield.DataType == "CDOCollection") {
                                $scope.AddSfxGridviewToGrid(objcontrolVM);

                            }
                            else {
                                $SgMessagesService.Message('Invalid Field', 'Select a valid one-to-many field from entity tree to add grid control.');
                            }


                        }
                        break;
                    case "SfxChart":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add chart control.");
                        }
                        else {
                            if (selectedobjecttreefield.DataType == "Collection" || selectedobjecttreefield.DataType == "List" || selectedobjecttreefield.DataType == "CDOCollection") {
                                $scope.AddSfxChartToGrid(objcontrolVM);

                            }
                            else {
                                $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add chart control.");
                            }
                        }
                        break;
                    case "UserControl":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-one field from entity tree to add user control.");
                        }
                        else if (selectedobjecttreefield.DataType != "Object") {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-one field from entity tree to add user control.");
                        }
                        else if (objcontrolVM.Name == "sfwColumn" || objcontrolVM.Name == "sfwButtonGroup") {
                            $scope.AddControlToCell(objcontrolVM, cntrlClassName, undefined);
                        }
                        else {
                            if (objcontrolVM && objcontrolVM.ParentVM && (objcontrolVM.ParentVM.Name == "sfwColumn" || objcontrolVM.ParentVM.Name == "sfwButtonGroup")) {
                                $scope.AddControlToCell(objcontrolVM.ParentVM, cntrlClassName, undefined);
                            }
                        }

                        break;
                    case "sfwCalendar":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add calendar control.");
                        }
                        else {
                            if (selectedobjecttreefield.DataType == "Collection" || selectedobjecttreefield.DataType == "List" || selectedobjecttreefield.DataType == "CDOCollection") {
                                $scope.AddSfxCalendarControl(objcontrolVM, false);
                            }
                            else {
                                $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add calendar control.");
                            }
                        }
                        break;
                    case "sfwScheduler":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add scheduler control.");
                        }
                        else if (FindParent(objcontrolVM, "sfwDialogPanel")) {
                            $SgMessagesService.Message("Message", "Select a valid one-to-many field from entity tree to add scheduler control.");
                        }
                        else {
                            if (selectedobjecttreefield.DataType == "Collection" || selectedobjecttreefield.DataType == "List" || selectedobjecttreefield.DataType == "CDOCollection") {
                                $scope.AddSfxCalendarControl(objcontrolVM, true);
                            }
                            else {
                                $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add scheduler control.");
                            }
                        }
                        break;
                    case "sfwListView":
                        if (!selectedobjecttreefield) {
                            $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add repeater control.");
                        }
                        else {
                            if (selectedobjecttreefield.DataType == "Collection" || selectedobjecttreefield.DataType == "List") {
                                $scope.AddSfxRepeaterControl(objcontrolVM);
                            }
                            else {
                                $SgMessagesService.Message("Invalid Field", "Select a valid one-to-many field from entity tree to add repeater control.");
                            }
                        }
                        break;
                    default:
                        var lstselectedfields = [];
                        lstselectedfields = GetSelectedFieldList(lstselectedobjecttreefields, lstselectedfields);
                        var isCollectionField = false;
                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
                            if (DisplayedEntity && DisplayedEntity.isParentFieldCollection) {
                                lstselectedfields = [];
                                isCollectionField = true;
                            }
                        }
                        var isDateTimeField = false;
                        var isCollectionField = false;
                        var isCollection = false;
                        if (lstselectedfields.length > 0) {
                            for (var i = 0; i < lstselectedfields.length; i++) {
                                if (!lstselectedfields[i].DataType || (lstselectedfields[i].DataType && lstselectedfields[i].DataType.toLowerCase() == "cdocollection")) {
                                    if (cntrlClassName == "sfwCheckBoxList" && !(lstselectedfields[i].Type && lstselectedfields[i].Type === "Description")) {
                                        isCollection = true;
                                    }
                                }
                                if (!lstselectedfields[i].DataType || (lstselectedfields[i].DataType && (lstselectedfields[i].DataType.toLowerCase() != "datetime") && lstselectedfields[i].DataType.toLowerCase() != "date")) {
                                    if (cntrlClassName == "sfwDateTimePicker") {
                                        isDateTimeField = true;
                                    }
                                }
                            }
                        }

                        var isValid = true;
                        if (isCollectionField) {
                            isValid = false;
                            $SgMessagesService.Message("Message", "Collection field/s cannot be added.");
                        }
                        else if (lstselectedfields.length == 0) {
                            isValid = false;
                            $SgMessagesService.Message("Message", "Select field/s from entity tree to add control.");
                        }
                        else if (isDateTimeField) {
                            isValid = false;
                            $SgMessagesService.Message("Invalid Field", "Select valid DateTime field/s from entity tree to add DateTime Picker control.");
                        }
                        else {
                            if (!isCollection && cntrlClassName == "sfwCheckBoxList" && !$scope.FormModel.IsLookupCriteriaEnabled) {
                                isValid = false;
                                $SgMessagesService.Message("Invalid Field", "Select valid one-to-many field of type CDOCollection to add Checkbox List control.");
                            }
                        }

                        if (cntrlClassName == "sfwLabel" || cntrlClassName == "sfwTextBox" || cntrlClassName == "sfwRadioButton" || cntrlClassName == "sfwCheckBox" || cntrlClassName == "sfwDateTimePicker"
                            || cntrlClassName == "sfwRadioButtonList" || cntrlClassName == "sfwDropDownList" || cntrlClassName == "sfwCascadingDropDownList" || cntrlClassName == "sfwMultiSelectDropDownList") {
                            if (lstselectedfields.length > 0) {
                                var tempSelectedList = [];
                                for (var i = 0; i < lstselectedfields.length; i++) {
                                    if (lstselectedfields[i].DataType !== "Collection" && lstselectedfields[i].DataType !== "List" && lstselectedfields[i].DataType !== "CDOCollection" && lstselectedfields[i].DataType !== "Object") {
                                        tempSelectedList.push(lstselectedfields[i]);
                                    }
                                }
                                lstselectedfields = tempSelectedList;
                            }
                        }
                        if (isValid) {
                            var descriptionList = [];
                            var message = "";
                            if (cntrlClassName == "sfwDropDownList") {


                                angular.forEach(lstselectedfields, getDropDownList);


                                angular.forEach(descriptionList, getSelectedList);

                            }

                            var isInvalidExpressionMapping = false;

                            angular.forEach(lstselectedfields, iteratorisInvalidExpression);


                            if (lstselectedfields.length > 0) {

                                if (objcontrolVM) {
                                    var buttonGroup = GetVM("sfwButtonGroup", objcontrolVM);
                                    if (objcontrolVM.Name == "ItemTemplate") {
                                        var blnIsLookup = $scope.FormModel.dictAttributes.sfwType == "Lookup";
                                        $rootScope.UndRedoBulkOp("Start");

                                        angular.forEach(lstselectedfields, iteratorAddActualControl);
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                    else if (objcontrolVM.Name == "sfwGridView") {
                                        var blnIsLookup = $scope.FormModel.dictAttributes.sfwType == "Lookup";
                                        var columnvm = objcontrolVM.Elements[0];
                                        $rootScope.UndRedoBulkOp("Start");
                                        if (!columnvm) {
                                            columnvm = {
                                                Name: 'Columns', Value: '', dictAttributes: {}, Elements: []
                                            };
                                            $rootScope.PushItem(columnvm, objcontrolVM.Elements);
                                        }

                                        angular.forEach(lstselectedfields, iteratorAddsfwGridView);

                                        $rootScope.UndRedoBulkOp("End");

                                    } else if (buttonGroup || objcontrolVM.Name == "sfwButtonGroup") {
                                        objcontrolVM = buttonGroup ? buttonGroup : objcontrolVM;
                                        $rootScope.UndRedoBulkOp("Start");
                                        angular.forEach(lstselectedfields, iteratorAddActualControl);
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                    else {
                                        if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.IsLookupCriteriaEnabled
                                            && (objcontrolVM.Name == "sfwTabContainer" || (objcontrolVM.Name == 'sfwColumn' && objcontrolVM.Elements.length > 0 && objcontrolVM.Elements[0].Name == "sfwTabContainer"))) {
                                            $SgMessagesService.Message("Not Allowed", "Can not add a control outside the criteria tab container");
                                        }
                                        else if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.IsPrototypeLookupCriteriaEnabled
                                            && (objcontrolVM.Name == "sfwTabContainer" || (objcontrolVM.Name == 'sfwColumn' && objcontrolVM.Elements.length > 0 && objcontrolVM.Elements[0].Name == "sfwTabContainer"))) {
                                            $SgMessagesService.Message("Not Allowed", "Can not add a control outside the criteria tab container");
                                        }
                                        else {
                                            var tableVM = GetVM("sfwTable", objcontrolVM);
                                            var sfxCellVM = $scope.GetSelectedControl();
                                            if (sfxCellVM && tableVM == GetVM("sfwTable", sfxCellVM)) {
                                                objcontrolVM = sfxCellVM;
                                            }
                                            if (tableVM) {
                                                $rootScope.UndRedoBulkOp("Start");
                                                $scope.AddControls(tableVM, objcontrolVM, cntrlClassName, lstselectedfields, false);
                                                $rootScope.UndRedoBulkOp("End");
                                            }
                                        }
                                    }
                                }
                            }

                            if (lstselectedfields.length == 0 && cntrlClassName != "sfwDropDownList") {
                                $SgMessagesService.Message("Not Allowed", "Select valid field/ s from entity tree to add control " + cntrlClassName + ".");
                            }
                            else if (descriptionList.length > 0) {
                                message.slice(0, -1);
                                //  message = message.TrimEnd(',');
                                $SgMessagesService.Message("Not Allowed", "Field that ends with _description can't be added as DropDownList");
                                //this.ReSet(descriptionList);
                            }
                            else if (isInvalidExpressionMapping) {
                                $SgMessagesService.Message("Invalid Expression", "Invalid Expression Binding");
                                //this.ReSet(expFields as List<ObjectTreeModel>);
                            }
                        }
                        break;
                }
            }
            ClearSelectedFieldList(lstselectedobjecttreefields);
            if ($scope.FormModel.IsLookupCriteriaEnabled && $scope.lstLoadedEntityColumnsTree) {
                for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++)
                    $scope.lstLoadedEntityColumnsTree[i].selectedobjecttreefield = undefined;
            } else if ($scope.lstLoadedEntityTrees) {
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++)
                    $scope.lstLoadedEntityTrees[i].selectedobjecttreefield = undefined;
            }
        }
    };


    $scope.AddControlToCell = function (objcontrolVM, cntrlName, sfxControlModel) {
        if (!sfxControlModel) {
            sfxControlModel = CreateControl($scope.FormModel, objcontrolVM, cntrlName);
        }

        if (sfxControlModel && sfxControlModel.Name != "udc") {
            $rootScope.PushItem(sfxControlModel, objcontrolVM.Elements);

            //this.ObjVM.DesignVM.CheckAndUpdateSelectedControlStatus(this.MirrorElements[this.MirrorElements.Count - 1] as SfxControlVM, false);
            //this.PopulateObjectID(this.ObjVM.Model, sfxControlModel);
        }

        //#region Add User Control
        if (sfxControlModel != undefined && sfxControlModel.Name == "udc") {

            var entityname = $scope.FormModel.dictAttributes.sfwEntity;
            var selectedobjecttreefield;
            for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {

                    selectedobjecttreefield = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                    break;
                }

            }

            if (selectedobjecttreefield) {
                entityname = selectedobjecttreefield.Entity;
            }
            var newScope = $scope.$new();
            var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
            var itempath = selectedobjecttreefield.ID;
            if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                itempath = DisplayedEntity.strDisplayName + "." + selectedobjecttreefield.ID;
            }
            newScope.objSetUCProp = {
                StrId: sfxControlModel.dictAttributes.ID, StrName: '', StrEntityField: itempath, StrResource: '', formObject: $scope.FormModel
            };
            newScope.formodel = $scope.FormModel;
            newScope.objSetUCProp.IsAddedFromObjectTree = true;
            newScope.onUserControlOkClick = function () {

                sfxControlModel.dictAttributes.ID = newScope.objSetUCProp.StrId;
                sfxControlModel.dictAttributes.Name = newScope.objSetUCProp.StrName;
                if (newScope.objSetUCProp.StrEntityField) {
                    sfxControlModel.dictAttributes.sfwEntityField = newScope.objSetUCProp.StrEntityField;
                }

                sfxControlModel.dictAttributes.sfwResource = newScope.objSetUCProp.StrResource;

                if (sfxControlModel.dictAttributes.Name != undefined && sfxControlModel.dictAttributes.Name != "") {
                    var fileList = [];
                    var obj = {
                        FileName: sfxControlModel.dictAttributes.Name, ID: sfxControlModel.dictAttributes.ID
                    };
                    fileList.push(obj);
                    $.connection.hubForm.server.getUserControlModel(fileList, "").done(function (udcFileList) {
                        $scope.receiveUcMainTable(udcFileList);
                    });
                }

                $rootScope.PushItem(sfxControlModel, objcontrolVM.Elements);

                newScope.onUserControlCancelClick();
            };

            newScope.onUserControlCancelClick = function () {
                $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.objSetUCProp);
                if (ucPropDialog) {
                    ucPropDialog.close();
                }
            };

            newScope.ValidateUserProp = function () {
                var retVal = false;
                newScope.ErrorMessageForDisplay = "";
                if (newScope.objSetUCProp.StrId == undefined || newScope.objSetUCProp.StrId == "") {
                    newScope.ErrorMessageForDisplay = "Error: Enter the ID.";
                    retVal = true;
                }
                else {
                    var lstIds = [];
                    CheckforDuplicateID($scope.FormModel, newScope.objSetUCProp.StrId, lstIds);
                    if (lstIds.length > 0) {
                        newScope.ErrorMessageForDisplay = "Error: Duplicate ID.";
                        retVal = true;
                    } else if (!isValidIdentifier(newScope.objSetUCProp.StrId, false, false)) {
                        newScope.ErrorMessageForDisplay = "Error: Invalid ID.";
                        retVal = true;
                    }
                }
                if (!newScope.objSetUCProp.StrName || newScope.objSetUCProp.StrName == '') {
                    newScope.ErrorMessageForDisplay = "Please Enter Active Form.";
                    retVal = true;
                }
                //else if (!newScope.objSetUCProp.StrEntityField || newScope.objSetUCProp.StrEntityField == "") {
                //    newScope.ErrorMessageForDisplay = "Please Enter Entity field.";
                //    retVal = true;
                //}
                else if (!newScope.objSetUCProp.StrResource || newScope.objSetUCProp.StrResource == '') {
                    newScope.ErrorMessageForDisplay = "Please Enter Resource.";
                    retVal = true;
                }

                if (newScope.ErrorMessageForDisplay == undefined || newScope.ErrorMessageForDisplay == "") {
                    if (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "") {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, newScope.objSetUCProp.StrEntityField);
                        if (!object || object.Type != "Object") {
                            newScope.ErrorMessageForDisplay = "Entity Field should be Object.";
                            retVal = true;
                        }
                    }
                }
                if (newScope.objSetUCProp.errors && $ValidationService.isEmptyObj(newScope.objSetUCProp.errors)) {
                    retVal = true;
                }
                return retVal;
            };


            var ucPropDialog = $rootScope.showDialog(newScope, "User Control", "Form/views/SetUserControlProperties.html");
        }

        //#endregion

    };

    $scope.AddControls = function (tableVM, selectedCntrlVM, astrControlClass, alst, isListView) {
        var blnIsLookup = $scope.FormModel.dictAttributes.sfwType == "Lookup";
        var strTabSheetID = "";

        if (blnIsLookup) {
            if (tableVM.ParentVM.Name == "sfwTabSheet") {
                strTabSheetID = tableVM.ParentVM.dictAttributes.ID;
                if (strTabSheetID && (strTabSheetID == "tshAdvSort" || strTabSheetID == "tshSql")) {
                    $SgMessagesService.Message("Not Allowed", "New controls cannot be added in Adv Sort/Query tab.");
                    return;
                }
            }

        }


        if (astrControlClass == "sfwHyperLink") {
            astrControlClass = "sfwLinkButton";
        }

        var dRowMultiplier = 0.4;




        var totalControlCount = 1;
        if (astrControlClass == "sfwRange") {
            var afield = alst[0];
            if (afield.DataType && (afield.DataType.toLowerCase() == "date" || afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "decimal" ||
                afield.DataType.toLowerCase() == "double" || startsWith(afield.DataType.toLowerCase(), "int", 0))) {
                totalControlCount = alst.length * 4;
            }
            else {
                totalControlCount = 0;
                $SgMessagesService.Message("Message", "Only fields having datatype Date/DateTime/Decimal/Double/Int can be added for range.");
                return;
            }
        }
        else if (blnIsLookup && strTabSheetID && strTabSheetID == "tshAdvCriteria") {
            totalControlCount = alst.length * 3;
        }
        else {
            totalControlCount = alst.length * 2;
        }

        var intRows = 1;
        var ColCount = GetMaxColCount(tableVM.Elements[0], tableVM);


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

            var sfxRowModel = {
                Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
            };
            sfxRowModel.ParentVM = tableVM;

            for (colInd = 0; colInd < ColCount; colInd++) {
                var sfxCellModel = {
                    Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = sfxRowModel;

                $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);
            }

            $rootScope.InsertItem(sfxRowModel, tableVM.Elements, rowInd + intCurRowInd);

            angular.forEach(tableVM.Elements[rowInd + intCurRowInd].Elements, function (vm) {
                var cellitem = {
                };
                cellitem.key = cellInd;
                cellitem.value = vm;
                cellLst.push(cellitem);
                cellInd++;
            });

        }


        cellInd = 0;
        function getCellIndex(field) {
            if (astrControlClass == "sfwRange") {
                cellInd = $scope.AddRange(cellLst, blnIsLookup, astrControlClass, field, cellInd);
            }
            else if (blnIsLookup && strTabSheetID && strTabSheetID == "tshAdvCriteria") {
                $scope.AddControlInAdvCriteria(cellLst, blnIsLookup, astrControlClass, field, cellInd);
            }
            else {
                cellInd = $scope.AddControlsGrid(cellLst, blnIsLookup, astrControlClass, field, cellInd, isListView);
            }
        }
        angular.forEach(alst, getCellIndex);


    };

    $scope.AddControlInAdvCriteria = function (acellLst, ablnIsLookup, astrControlClass, afield, intCellInd) {
        var aintCellInd = intCellInd;
        var strOpCodeGroup = "0";
        var strControlInit = "";
        switch (astrControlClass) {
            case "sfwLabel":
                strControlInit = "lblAdv";
                strOpCodeGroup = "0";
                break;
            case "sfwTextBox":
                strControlInit = "txtAdv";
                strOpCodeGroup = "3";
                break;
            case "sfwDropDownList":
                strControlInit = "ddlAdv";
                strOpCodeGroup = "4";
                break;
            case "sfwCheckBox":
                strControlInit = "chkAdv";
                strOpCodeGroup = "0";
                break;
            case "sfwCheckBoxList":
                strControlInit = "cblAdv";
                strOpCodeGroup = "8";
                break;
        }

        var strControlID = CreateControlID($scope.FormModel, afield.ID, astrControlClass);

        var cellVM;
        var newControl;
        var prefix = "swc";
        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;
                aintCellInd++;

                newControl = {
                    Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = cellVM;

                var strLabelID = CreateControlID($scope.FormModel, afield.ID, "sfwLabel", true);
                if (startsWith(strLabelID, "cap", 0)) {
                    strLabelID = "capAdv" + strLabelID.substring(3);
                }
                newControl.dictAttributes.ID = strLabelID;
                if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                    newControl.dictAttributes.Text = GetCaptionFromField(afield);
                    if (newControl.dictAttributes.Text && newControl.dictAttributes.Text.trim().length > 0 && !newControl.dictAttributes.Text.contains(":")) {
                        newControl.dictAttributes.Text = newControl.dictAttributes.Text + " : ";
                    }
                }
                else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                    newControl.dictAttributes.Text = GetCaptionFromField(afield);
                    if (newControl.dictAttributes.Text && newControl.dictAttributes.Text.trim().length > 0 && !newControl.dictAttributes.Text.contains(":")) {
                        newControl.dictAttributes.Text = newControl.dictAttributes.Text + " : ";
                    }
                }
                else {
                    newControl.dictAttributes.Text = GetCaptionFromFieldName(afield.ID) + " : ";
                }
                newControl.dictAttributes.sfwIsCaption = "True";
                newControl.dictAttributes.AssociatedControlID = strControlID;

                $rootScope.PushItem(newControl, cellVM.Elements);

                break;
            }
        }

        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;
                aintCellInd++;

                newControl = {
                    Name: "sfwDropDownList", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = cellVM;

                newControl.dictAttributes.ID = "ddlAdvOp" + CreateControlIDInCamelCase(afield.ID);
                newControl.dictAttributes.sfwLoadType = "CodeGroup";
                newControl.dictAttributes.sfwLoadSource = strOpCodeGroup;

                $rootScope.PushItem(newControl, cellVM.Elements);

                break;
            }
        }
        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;
                aintCellInd++;
                newControl = {
                    Name: astrControlClass, value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = cellVM;


                newControl.dictAttributes.ID = strControlID;
                if ($scope.SelectedQuery) {
                    newControl.dictAttributes.sfwQueryID = $scope.SelectedQuery.dictAttributes.ID;//$scope.GetsfwQueryID();
                }
                if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                    newControl.dictAttributes.sfwDataField = afield.Value;
                }
                else {
                    newControl.dictAttributes.sfwDataField = afield.ID;
                }

                SetDefultValuesBasedOnDataType(afield, astrControlClass, newControl);

                if (astrControlClass == "sfwDropDownList" || astrControlClass == "sfwCheckBoxList") {
                    if (endsWith(afield.Value, "_value")) {
                        var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
                        if (DisplayedEntity) {
                            var entityname = DisplayedEntity.strID;
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            var strCodeGroup = 0;
                            strCodeGroup = GetCodeIDForLookup(entityname, afield.Value, entityIntellisenseList);
                            newControl.dictAttributes.sfwLoadType = "CodeGroup";
                            newControl.placeHolder = strCodeGroup;
                            //newControl.dictAttributes.sfwLoadSource = strCodeGroup;
                        }
                    }
                }

                if (astrControlClass == "sfwCheckBoxList") {
                    newControl.dictAttributes.RepeatDirection = "Horizontal";
                    newControl.dictAttributes.RepeatColumns = "4";
                }

                $rootScope.PushItem(newControl, cellVM.Elements);

                break;
            }
        }


        return aintCellInd;
    };

    $scope.GetsfwQueryID = function () {
        var initialload = $scope.FormModel.Elements.filter(function (x) {
            return x.Name == 'initialload';
        });

        if (initialload.length > 0) {
            for (i = 0; i < initialload[0].Elements.length; i++) {

                var sfwQueryRef = initialload[0].Elements[i].dictAttributes.sfwQueryRef;
                if (!$scope.IsSubQuery(sfwQueryRef)) {
                    return initialload[0].Elements[i].dictAttributes.ID;
                }
            }
        }

    };

    $scope.AddRange = function (acellLst, ablnIsLookup, astrControlClass, afield, intCellInd) {
        var aintCellInd = intCellInd;

        if (afield.DataType && (afield.DataType.toLowerCase() == "date" || afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "decimal" ||
            afield.DataType.toLowerCase() == "double" || startsWith(afield.DataType.toLowerCase(), "int", 0))) {
            $rootScope.UndRedoBulkOp("Start");
            var controlID = CreateControlIDInCamelCase(afield.ID);

            var cellVM;
            var newCntrl;
            var prefix = "swc";
            for (i = 0; i < acellLst.length; i++) {
                if (acellLst[i].key == aintCellInd) {
                    cellVM = acellLst[i].value;
                    aintCellInd++;

                    newCntrl = {
                        Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: { sfwIsCaption: "True" }, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "cap" + controlID + "From";

                    if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                        newCntrl.dictAttributes.Text = GetCaptionFromField(afield) + " From : ";
                    }
                    else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        newCntrl.dictAttributes.Text = GetCaptionFromField(afield) + " From : ";
                    }
                    else {
                        newCntrl.dictAttributes.Text = GetCaptionFromFieldName(afield.ID) + " From : ";
                    }

                    $rootScope.PushItem(newCntrl, cellVM.Elements);
                    break;
                }
            }

            for (i = 0; i < acellLst.length; i++) {
                if (acellLst[i].key == aintCellInd) {
                    cellVM = acellLst[i].value;
                    aintCellInd++;

                    newCntrl = {
                        Name: "sfwTextBox", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "txt" + controlID + "From";
                    if ($scope.SelectedQuery) {
                        newCntrl.dictAttributes.sfwQueryID = $scope.SelectedQuery.dictAttributes.ID;//$scope.GetsfwQueryID();
                    }
                    if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        newCntrl.dictAttributes.sfwDataField = afield.Value;
                    }
                    else {
                        newCntrl.dictAttributes.sfwDataField = afield.ID;
                    }
                    newCntrl.dictAttributes.sfwOperator = "between";
                    newCntrl.dictAttributes.sfwRelatedControl = "txt" + controlID + "To";

                    if (afield.DataType && (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")) {
                        newCntrl.dictAttributes.sfwDataType = "DateTime";
                        newCntrl.dictAttributes.sfwDataFormat = "{0:d}";
                    }
                    else {
                        newCntrl.dictAttributes.sfwDataType = "Numeric";
                    }

                    $rootScope.PushItem(newCntrl, cellVM.Elements);

                    newCntrl = {
                        Name: "CompareValidator", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "val" + controlID + "From";
                    newCntrl.dictAttributes.Text = "*";
                    newCntrl.dictAttributes.sfwOperator = "DataTypeCheck";
                    newCntrl.dictAttributes.ControlToValidate = "txt" + controlID + "From";

                    if (afield.DataType && (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")) {
                        newCntrl.dictAttributes.Type = "Date";

                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + "From must be in mm/dd/yyyy format.";
                        }
                        else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + "From must be in mm/dd/yyyy format.";
                        }
                        else {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + "From must be in mm/dd/yyyy format.";
                        }
                    }
                    else {
                        newCntrl.dictAttributes.Type = "Integer";

                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " From must be an Integer value.";
                        }
                        else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " From must be an Integer value.";
                        }
                        else {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " From must be an Integer value.";
                        }
                    }
                    $rootScope.PushItem(newCntrl, cellVM.Elements);

                    break;
                }
            }


            for (i = 0; i < acellLst.length; i++) {
                if (acellLst[i].key == aintCellInd) {
                    cellVM = acellLst[i].value;
                    aintCellInd++;

                    newCntrl = {
                        Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: { sfwIsCaption: "True" }, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "cap" + controlID + "To";

                    if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                        newCntrl.dictAttributes.Text = GetCaptionFromField(afield) + " To : ";
                    }
                    else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        newCntrl.dictAttributes.Text = GetCaptionFromField(afield) + " To : ";
                    }
                    else {
                        newCntrl.dictAttributes.Text = GetCaptionFromFieldName(afield.ID) + " To : ";
                    }


                    $rootScope.PushItem(newCntrl, cellVM.Elements);


                    break;
                }
            }

            for (i = 0; i < acellLst.length; i++) {
                if (acellLst[i].key == aintCellInd) {
                    cellVM = acellLst[i].value;
                    aintCellInd++;

                    newCntrl = {
                        Name: "sfwTextBox", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "txt" + controlID + "To";
                    if ($scope.SelectedQuery) {
                        newCntrl.dictAttributes.sfwQueryID = $scope.SelectedQuery.dictAttributes.ID;//$scope.GetsfwQueryID();
                    }
                    if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        newCntrl.dictAttributes.sfwDataField = afield.Value;
                    }
                    else {
                        newCntrl.dictAttributes.sfwDataField = afield.ID;
                    }

                    newCntrl.dictAttributes.sfwOperator = "between";


                    if (afield.DataType && (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")) {
                        newCntrl.dictAttributes.sfwDataType = "DateTime";
                        newCntrl.dictAttributes.sfwDataFormat = "{0:d}";
                    }
                    else {
                        newCntrl.dictAttributes.sfwDataType = "Numeric";
                    }
                    $rootScope.PushItem(newCntrl, cellVM.Elements);

                    newCntrl = {
                        Name: "CompareValidator", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "val" + controlID + "To";
                    newCntrl.dictAttributes.Text = "*";
                    newCntrl.dictAttributes.Operator = "DataTypeCheck";
                    newCntrl.dictAttributes.ControlToValidate = "txt" + controlID + "To";

                    if (afield.DataType && (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")) {
                        newCntrl.dictAttributes.Type = "Date";
                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + "From must be in mm/dd/yyyy format.";
                        }
                        else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + "From must be in mm/dd/yyyy format.";
                        }
                        else {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + "From must be in mm/dd/yyyy format.";
                        }
                    }
                    else {
                        newCntrl.dictAttributes.Type = "Integer";
                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " From must be an Integer value.";
                        }
                        else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " From must be an Integer value.";
                        }
                        else {
                            newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " From must be an Integer value.";
                        }
                    }
                    $rootScope.PushItem(newCntrl, cellVM.Elements);

                    // Insert second CompareValidator

                    newCntrl = {
                        Name: "CompareValidator", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newCntrl.ParentVM = cellVM;


                    newCntrl.dictAttributes.ID = "val" + controlID + "Range";
                    newCntrl.dictAttributes.Text = "*";
                    newCntrl.dictAttributes.Operator = "GreaterThanEqual";
                    newCntrl.dictAttributes.ControlToValidate = "txt" + controlID + "To";
                    newCntrl.dictAttributes.ControlToCompare = "txt" + controlID + "From";


                    if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " To cannot be less than " + GetCaptionFromField(afield) + " From.";
                    }
                    else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromField(afield) + " To cannot be less than " + GetCaptionFromField(afield) + " From.";
                    }
                    else {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " To cannot be less than " + GetCaptionFromFieldName(afield.ID) + " From.";
                    }

                    if (afield.DataType && (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")) {
                        newCntrl.dictAttributes.Type = "Date";
                    }
                    else {
                        newCntrl.dictAttributes.Type = "Integer";
                    }
                    $rootScope.PushItem(newCntrl, cellVM.Elements);

                    break;
                }
            }

            $rootScope.UndRedoBulkOp("End");

        }
        return aintCellInd;
    };

    $scope.AddControlsGrid = function (acellLst, ablnIsLookup, astrControlClass, afield, cellInd, isListViewControl) {
        var aintCellInd = cellInd;
        var strControlID = CreateControlID($scope.FormModel, afield.ID, astrControlClass);

        var cellVM;
        var newControl;
        var prefix = "swc";
        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;

                aintCellInd++;

                newControl = {
                    Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = cellVM;

                var strLabelID = CreateControlID($scope.FormModel, afield.ID, "sfwLabel", true);
                newControl.dictAttributes.ID = strLabelID;
                newControl.dictAttributes.Text = GetCaptionFromField(afield);
                if (newControl.dictAttributes.Text && newControl.dictAttributes.Text.trim().length > 0 && !newControl.dictAttributes.Text.contains(":")) {
                    newControl.dictAttributes.Text = newControl.dictAttributes.Text + " : ";
                }
                if (!isListViewControl) {
                    newControl.dictAttributes.AssociatedControlID = strControlID;
                    newControl.dictAttributes.sfwIsCaption = "True";
                }

                $rootScope.PushItem(newControl, cellVM.Elements);
                break;
            }
        }


        for (i = 0; i < acellLst.length; i++) {
            if (acellLst[i].key == aintCellInd) {
                cellVM = acellLst[i].value;
                aintCellInd++;

                $scope.AddActualControl(ablnIsLookup, astrControlClass, afield, isListViewControl, cellVM, false);
                break;
            }
        }


        return aintCellInd;


    };

    $scope.AddActualControl = function (ablnIsLookup, astrControlClass, afield, isListViewControl, cellVM, blnAddedinGrid) {
        var prefix = "swc";
        var newControl = {
            Name: astrControlClass, value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
        };
        var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
        var itempath = afield.ID;
        if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
            itempath = DisplayedEntity.strDisplayName + "." + afield.ID;
        }
        newControl.ParentVM = cellVM;
        if (!blnAddedinGrid) {
            if (astrControlClass == "sfwLabel") {
                strControlID = CreateControlID($scope.FormModel, afield.ID, astrControlClass, false);
            }
            else {
                strControlID = CreateControlID($scope.FormModel, afield.ID, astrControlClass, true);
            }


            newControl.dictAttributes.ID = strControlID;
        }
        else if (blnAddedinGrid && astrControlClass && astrControlClass == "sfwCascadingDropDownList") {
            strControlID = CreateControlID($scope.FormModel, afield.ID, astrControlClass, true);
            newControl.dictAttributes.ID = strControlID;
        }
        if (ablnIsLookup) {
            if (!blnAddedinGrid) {
                if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                    newControl.dictAttributes.sfwDataField = afield.Value;
                }
                else if ($scope.FormModel.IsLookupCriteriaEnabled == true) {
                    newControl.dictAttributes.sfwDataField = afield.ID;
                }
                else {
                    newControl.dictAttributes.sfwEntityField = itempath;
                }
            }
            else {
                newControl.dictAttributes.sfwEntityField = itempath;
            }
            if ($scope.FormModel.IsLookupCriteriaEnabled && $scope.SelectedQuery) {
                newControl.dictAttributes.sfwQueryID = $scope.SelectedQuery.dictAttributes.ID;//$scope.GetsfwQueryID();
            }
        }
        else if (isListViewControl) {
            newControl.dictAttributes.sfwDataField = itempath;// GetItemPathForEntityObject(afield);
        }
        else {
            if (newControl.Name == "sfwLabel") {
                newControl.dictAttributes.sfwEntityField = itempath;//GetItemPathForEntityObject(afield);
            }
            else {
                newControl.dictAttributes.sfwEntityField = itempath;// GetItemPathForEntityObject(afield);
            }

        }

        if (astrControlClass == "sfwLinkButton") {
            newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
        }
        else if (astrControlClass == "sfwDropDownList" || astrControlClass == "sfwCascadingDropDownList" || astrControlClass == "sfwCheckBoxList" || astrControlClass == "sfwRadioButtonList" || astrControlClass == "sfwMultiSelectDropDownList") {
            if (endsWith(afield.Value, "_value")) {
                var entityName = "";
                if (DisplayedEntity) {
                    entityName = DisplayedEntity.strID;
                }
                if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                    entityName = $scope.FormModel.dictAttributes.sfwEntity;
                }
                else if ($scope.FormModel.IsLookupCriteriaEnabled && $scope.SelectedQuery) {
                    if ($scope.SelectedQuery.dictAttributes.sfwQueryRef.contains('.')) {
                        entityName = $scope.SelectedQuery.dictAttributes.sfwQueryRef.substring(0, $scope.SelectedQuery.dictAttributes.sfwQueryRef.indexOf('.'));
                    }
                }
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var strCodeGroup = "";
                if (entityName) {
                    if (!ablnIsLookup) {
                        strCodeGroup = GetCodeID(entityName, afield.ID, entityIntellisenseList);
                    } else {
                        if ($scope.FormModel.IsLookupCriteriaEnabled) {
                            strCodeGroup = GetCodeIDForLookup(entityName, afield.Value, entityIntellisenseList);
                        }
                        else {
                            strCodeGroup = GetCodeID(entityName, afield.ID, entityIntellisenseList);
                        }
                    }
                }
                if (!strCodeGroup) {
                    strCodeGroup = "0";
                }
                newControl.dictAttributes.sfwLoadType = "CodeGroup";
                newControl.placeHolder = strCodeGroup;
                //newControl.dictAttributes.sfwLoadSource = strCodeGroup;

                if (ablnIsLookup) {
                    if (astrControlClass == "sfwDropDownList") {
                        newControl.dictAttributes.sfwOperator = "=";
                    }
                }
            }
        }
        else if (astrControlClass == "sfwRadioButton" && afield.Value) {
            var strIbusName = afield.Value.substring(afield.Value.indexOf(".") + 1);
            newControl.dictAttributes.GroupName = strIbusName;
        }
        else {
            SetDefultValuesBasedOnDataType(afield, astrControlClass, newControl);
            if (ablnIsLookup && afield.DataType) {
                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date") {
                    newControl.dictAttributes.sfwDataType = "DateTime";
                }
                else if (afield.DataType.toLowerCase() == "decimal") {
                    newControl.dictAttributes.sfwDataType = "Decimal";
                }
                else if (afield.DataType == "Int32" || afield.DataType.toLowerCase() == "int" || afield.DataType == "int32") {
                    newControl.dictAttributes.sfwDataType = "Numeric";
                }
            }
        }

        if (blnAddedinGrid) {
            if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType != "Lookup") {

                if (newControl.Name != "sfwLabel" && newControl.Name != "sfwButton" && newControl.Name != "sfwLinkButton" && newControl.Name != "sfwImageButton" && newControl.Name !== "sfwButtonGroup") {
                    var objGridView = FindParent(newControl, "sfwGridView");
                    if (objGridView) {
                        $rootScope.EditPropertyValue(objGridView.dictAttributes.AllowEditing, objGridView.dictAttributes, "AllowEditing", "True");
                        $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwTwoWayBinding, objGridView.dictAttributes, "sfwTwoWayBinding", "True");
                        $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwCommonFilterBox, objGridView.dictAttributes, "sfwCommonFilterBox", "False");
                        $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwFilterOnKeyPress, objGridView.dictAttributes, "sfwFilterOnKeyPress", "False");

                    }
                }
            }
        }
        $rootScope.PushItem(newControl, cellVM.Elements, "SetFormSelectedControl");
        $scope.SetFormSelectedControl(newControl);
    };

    $scope.AddSfxChartToGrid = function (objcontrolVM) {
        var selectedField;
        for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
            if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {
                selectedField = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                break;
            }
        }

        if (selectedField) {

            // if (UtilityFunctions.ValidateSfwObjectFieldForGrid(selectedField.BusObjName, selectedField.ItemPath, this.ObjVM.VMMain.ActiveProject))
            {

                if (objcontrolVM) {
                    //var entitycollname = GetItemPathForEntityObject(selectedField);
                    var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
                    var DisplayName = "";
                    if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                        DisplayName = DisplayedEntity.strDisplayName + "." + selectedField.ID;
                    } else {
                        DisplayName = selectedField.ID;
                    }
                    var entitycollname = DisplayName;
                    $scope.ParentEntityName = selectedField.Entity;
                    var cellVM = null;
                    var buttonGroup = GetVM("sfwButtonGroup", objcontrolVM);
                    if (buttonGroup) {
                        cellVM = buttonGroup;
                    } else {
                        cellVM = GetVM("sfwColumn", objcontrolVM);
                    }

                    // var cellVM = GetVM("sfwColumn", objcontrolVM);
                    if (null != cellVM) {
                        var newScope = $scope.$new();
                        newScope.ObjChartModel = {
                            Name: 'sfwChart',
                            prefix: 'swc',
                            Value: '',
                            dictAttributes: {
                                ID: '', sfwEntityField: entitycollname, ChartType: '', Width: '', Height: '', ShowLegend: 'True'
                            },
                            Elements: [],
                            Children: []
                        };
                        newScope.ObjChartModel.lstselectedobjecttreefields = [];
                        newScope.ObjChartModel.selectedobjecttreefield;
                        newScope.lstChartType = $rootScope.ChartTypes;
                        newScope.lstToolTipTypes = ['None', 'Chart', 'Table', 'Both'];
                        newScope.DataFormats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];


                        newScope.SeriesModel = {
                            Name: 'Series',
                            prefix: '',
                            Value: '',
                            dictAttributes: {
                            },
                            Elements: [],
                            Children: []
                        };
                        newScope.SeriesModel.ParentVM = newScope.ObjChartModel;
                        newScope.ObjChartModel.Elements.push(newScope.SeriesModel);

                        var ChartAreasModel = {
                            Name: 'ChartAreas',
                            prefix: '',
                            Value: '',
                            dictAttributes: {
                            },
                            Elements: [],
                            Children: []
                        };
                        ChartAreasModel.ParentVM = newScope.ObjChartModel;
                        newScope.ObjChartModel.Elements.push(ChartAreasModel);

                        newScope.sfwChartAreaModel = {
                            Name: 'sfwChartArea',
                            prefix: 'swc',
                            Value: '',
                            dictAttributes: {
                                ChartAreaName: '', BackColor: ''
                            },
                            Elements: [],
                            Children: []
                        };
                        newScope.sfwChartAreaModel.ParentVM = ChartAreasModel;
                        ChartAreasModel.Elements.push(newScope.sfwChartAreaModel);

                        newScope.removeSeriescolumnValues = function (chartModel) {
                            for (var i = 0; i < chartModel.Elements.length; i++) {
                                if (chartModel.Elements[i].Name == "Series") {
                                    var seriesModel = chartModel.Elements[i];
                                    for (var j = 0; j < seriesModel.Elements.length; j++) {
                                        seriesModel.Elements[j].dictAttributes.sfwSeriesColumnName = "";
                                    }
                                    break;
                                }
                            }
                        };

                        newScope.onChartTypeChanged = function () {
                            if (newScope.ObjChartModel.dictAttributes.ChartType && (newScope.ObjChartModel.dictAttributes.ChartType == "Donut" || newScope.ObjChartModel.dictAttributes.ChartType == "Pie")) {
                                newScope.ObjChartModel.dictAttributes.sfwIsDynamicSeries = "False";
                                newScope.removeSeriescolumnValues(newScope.ObjChartModel);
                            }
                        };
                        newScope.Area3DStyle = {
                            Name: 'Area3DStyle',
                            prefix: '',
                            Value: '',
                            dictAttributes: {
                                IsEnable3D: 'false', Inclination: '', LightStyle: ''
                            },
                            Elements: [],
                            Children: []
                        };
                        newScope.Area3DStyle.ParentVM = newScope.sfwChartAreaModel;
                        newScope.sfwChartAreaModel.Elements.push(newScope.Area3DStyle);


                        newScope.onSfxChartFinishClick = function () {
                            newScope.UpdateNavigationParam();
                            var tableVM = null;
                            if (cellVM.Name == "sfwButtonGroup") {
                                tableVM = cellVM;
                            } else {
                                tableVM = GetVM('sfwTable', cellVM);
                            }

                            // var tableVM = GetVM('sfwTable', cellVM);
                            if (tableVM && tableVM.Name != "sfwButtonGroup") {
                                var newRowModel = {
                                    Name: 'sfwRow',
                                    prefix: 'swc',
                                    Value: '',
                                    dictAttributes: {
                                    },
                                    Elements: [],
                                    Children: []
                                };
                                newRowModel.ParentVM = tableVM;

                                var newcellModel = {
                                    Name: 'sfwColumn',
                                    prefix: 'swc',
                                    Value: '',
                                    dictAttributes: {
                                    },
                                    Elements: [],
                                    Children: []
                                };
                                newcellModel.ParentVM = newRowModel;
                                if (newScope.ObjChartModel && newScope.ObjChartModel.dictAttributes) {
                                    if (!newScope.ObjChartModel.dictAttributes.ID.match("^chr")) {
                                        newScope.ObjChartModel.dictAttributes.ID = "chr" + newScope.ObjChartModel.dictAttributes.ID;
                                    }
                                    newcellModel.Elements.push(newScope.ObjChartModel);
                                }

                                newRowModel.Elements.push(newcellModel);

                                var ColCount = GetMaxColCount(tableVM.Elements[0]);
                                for (ind = 1; ind < ColCount; ind++) {
                                    newcellModel = {
                                        Name: 'sfwColumn',
                                        prefix: 'swc',
                                        Value: '',
                                        dictAttributes: {
                                        },
                                        Elements: [],
                                        Children: []
                                    };
                                    newcellModel.ParentVM = newRowModel;

                                    newRowModel.Elements.push(newcellModel);
                                }

                                tableVM.Elements.push(newRowModel);
                            }
                            if (tableVM && tableVM.Name == "sfwButtonGroup") {
                                if (newScope.ObjChartModel && newScope.ObjChartModel.dictAttributes) {
                                    if (!newScope.ObjChartModel.dictAttributes.ID.match("^chr")) {
                                        newScope.ObjChartModel.dictAttributes.ID = "chr" + newScope.ObjChartModel.dictAttributes.ID;
                                    }
                                    tableVM.Elements.push(newScope.ObjChartModel);
                                }
                            }
                            $scope.selectControl(newScope.ObjChartModel);

                            newScope.onSfxChartCancelClick();
                        };


                        newScope.UpdateNavigationParam = function () {
                            var strParameters = "";
                            function item(grdParam) {
                                var strParamValue = grdParam.ParmeterValue;

                                if (strParamValue != null && strParamValue != "") {
                                    var blnConstant = grdParam.ParmeterConstant;
                                    if (blnConstant)
                                        strParamValue = "#" + strParamValue;

                                    var strParam = strParamValue;
                                    var strParamField = grdParam.ParmeterField;

                                    if (strParamValue.toLowerCase() != strParamField.toLowerCase())
                                        strParam = strParamField + '=' + strParamValue;

                                    if (strParameters == "")
                                        strParameters = strParam;
                                    else
                                        strParameters += ';' + strParam;
                                }
                            }
                            function iterator(aobjSeriesModel) {
                                strParameters = "";
                                angular.forEach(aobjSeriesModel.FormParameters, item);
                                aobjSeriesModel.dictAttributes.sfwNavigationParameter = strParameters;
                            }
                            angular.forEach(newScope.SeriesModel.Elements, iterator);
                        };

                        newScope.onSfxChartCancelClick = function () {
                            if (newScope.CreateChartDialog) {
                                newScope.CreateChartDialog.close();
                            }
                            //ngDialog.close(newScope.objGridViewWizardVM.id);
                        };

                        newScope.isChartNextDisable = function () {
                            var IsValid = false;
                            newScope.ObjChartModel.ErrorMessageForDisplay = "";
                            if (!newScope.ObjChartModel.ValidateChartName()) {
                                IsValid = true;
                            }
                            else if (!newScope.ObjChartModel.ValidateEntityCollection()) {
                                IsValid = true;
                            }

                            else if (!newScope.ObjChartModel.ValidateChartType()) {
                                IsValid = true;
                            }
                            else if (!newScope.ObjChartModel.ValidateWidth()) {
                                IsValid = true;
                            }
                            else if (!newScope.ObjChartModel.ValidateHeight()) {
                                IsValid = true;
                            }
                            //else if (!newScope.ObjChartModel.ValidateChartAreaName()) {
                            //    IsValid = true;
                            //}
                            return IsValid;

                        };


                        //#region Validation for Create Chart

                        //#region Validate Chart Name
                        newScope.ObjChartModel.ValidateChartName = function () {
                            var retValue = true;
                            if (!newScope.ObjChartModel.dictAttributes.ID) {
                                newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter ID.";
                                retValue = false;
                            }
                            return retValue;
                        };
                        //#endregion

                        //#region Validation for Entity Collecction
                        newScope.ObjChartModel.ValidateEntityCollection = function () {
                            var retValue = true;
                            if (!newScope.ObjChartModel.dictAttributes.sfwEntityField) {
                                newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Entity.";
                                retValue = false;
                            }

                            return retValue;
                        };
                        //#endregion


                        //#region Validation for Chart Type
                        newScope.ObjChartModel.ValidateChartType = function () {
                            var retValue = true;
                            if (!newScope.ObjChartModel.dictAttributes.ChartType) {
                                newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please Select a ChartType.";
                                retValue = false;
                            }
                            return retValue;
                        };
                        //#endregion

                        //#region Validation for Width
                        newScope.ObjChartModel.ValidateWidth = function () {
                            var retValue = true;
                            if (!newScope.ObjChartModel.dictAttributes.Width) {
                                newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Width.";
                                retValue = false;
                            }
                            //else if (newScope.ObjChartModel.dictAttributes.Width) {
                            //    var reg = new RegExp("[^0-9]");
                            //    if (reg.test(newScope.ObjChartModel.dictAttributes.Width)) {
                            //        newScope.ObjChartModel.ErrorMessageForDisplay = "Invalid Width '" + newScope.ObjChartModel.dictAttributes.Width + "'.";
                            //        return false;
                            //    }
                            //}
                            return retValue;
                        };
                        //#endregion

                        //#region Validation for Height
                        newScope.ObjChartModel.ValidateHeight = function () {
                            var retValue = true;
                            if (!newScope.ObjChartModel.dictAttributes.Height) {
                                newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Height.";
                                retValue = false;
                            }
                            //else if (newScope.ObjChartModel.dictAttributes.Height) {
                            //    var reg = new RegExp("[^0-9]");
                            //    if (reg.test(newScope.ObjChartModel.dictAttributes.Height)) {
                            //        newScope.ObjChartModel.ErrorMessageForDisplay = "Invalid Height '" + newScope.ObjChartModel.dictAttributes.Height + "'.";
                            //        return false;
                            //    }
                            //}
                            return retValue;
                        };
                        //#endregion

                        //#region Validation for Chart Type
                        //newScope.ObjChartModel.ValidateChartAreaName = function () {
                        //    var retValue = true;
                        //    if (!newScope.sfwChartAreaModel.dictAttributes.ChartAreaName) {
                        //        newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a ChartAreaName.";
                        //        retValue = false;
                        //    }
                        //    return retValue;
                        //}
                        //#endregion

                        //#endregion


                        //#region add series

                        newScope.ShowSeriesDetail = function (itm) {

                            itm.ShowDetails = !itm.ShowDetails;
                        };

                        newScope.SelectSeries = function (obj) {
                            if (newScope.ObjSeriesModel && newScope.ObjSeriesModel != obj) {
                                if (newScope.ObjSeriesModel.ShowDetails) {
                                    newScope.ObjSeriesModel.ShowDetails = false;
                                }
                            }
                            newScope.ObjSeriesModel = obj;
                        };

                        newScope.GetSeriesID = function () {
                            var strItemKey = "Series";
                            var iItemNum = 0;
                            var strItemName = strItemKey;

                            var newTemp = newScope.SeriesModel.Elements.filter(function (x) {
                                return x.dictAttributes.Name == strItemName;
                            });

                            while (newTemp.length > 0) {
                                iItemNum++;
                                strItemName = strItemKey + iItemNum;
                                newTemp = newScope.SeriesModel.Elements.filter(function (x) {
                                    return x.dictAttributes.Name == strItemName;
                                });
                            }
                            return strItemName;
                        };

                        newScope.OnAddSeriesClick = function () {
                            var newSeriesScope = newScope.$new();

                            newSeriesScope.ObjSeriesModel = {
                                Name: 'sfwSeries',
                                prefix: 'swc',
                                Value: '',
                                dictAttributes: {
                                    Name: '', XValueMember: '', YValueMembers: '', YMemberColor: '', IsValueShownAsLabel: 'False', sfwFormatField: '', sfwTooltipType: '', sfwTooltipTableParams: '', sfwActiveForm: ''
                                },
                                Elements: [],
                                Children: []
                            };
                            newSeriesScope.ObjSeriesModel.dictAttributes.Name = newScope.GetSeriesID();
                            newSeriesScope.ObjSeriesModel.ParentVM = newScope.SeriesModel;
                            newSeriesScope.ObjSeriesModel.lstselectedobjecttreefields = [];
                            newSeriesScope.ObjSeriesModel.selectedobjecttreefield;
                            newSeriesScope.ObjSeriesModel.ShowDetails = true;
                            newSeriesScope.ObjSeriesModel.FormParameters = [];
                            newSeriesScope.ShowParameters = false;
                            //$scope.populateParamtersForform = function () {

                            //    $.connection.hubForm.server.getFormParameters(newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm, "").done(function (lstparams) {
                            //        $scope.receiveFormParameters(lstparams, "");
                            //    });
                            //};

                            $scope.receiveFormParameters = function (lstparams, formtype) {
                                if (lstparams) {
                                    newSeriesScope.$evalAsync(function () {
                                        newSeriesScope.ObjSeriesModel.FormParameters = lstparams;
                                        newSeriesScope.ShowParameters = true;
                                    });
                                }
                            };
                            newSeriesScope.ClearParamtersForform = function () {
                                if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.FormParameters) {
                                    newSeriesScope.ObjSeriesModel.FormParameters = "";
                                }
                                if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm != "") {
                                    newSeriesScope.seriesactiveform = true;
                                }
                                newSeriesScope.ShowParameters = false;
                            };

                            //#region Validation for Add Series


                            newSeriesScope.isChartFinishDisable = function () {
                                var IsValid = false;
                                newSeriesScope.ErrorMessageForDisplay = "";
                                if (newSeriesScope.ObjSeriesModel) {
                                    if (!newSeriesScope.ValidateSeriesName()) {
                                        IsValid = true;
                                    }
                                    else if (!newSeriesScope.ValidateXValue()) {
                                        IsValid = true;
                                    }
                                    else if (!newSeriesScope.ValidateYValue()) {
                                        IsValid = true;
                                    }
                                    //else if (!newSeriesScope.ValidateActiveForm()) {
                                    //    IsValid = true;
                                    //}
                                }
                                return IsValid;
                            };

                            //#region Validation for Series Name
                            newSeriesScope.ValidateSeriesName = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.Name) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Name.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion

                            //#region Validation for X Value Member
                            newSeriesScope.ValidateXValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.XValueMember) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series XValueMember.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion


                            //#region Validation for Y Value Member
                            newSeriesScope.ValidateYValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion


                            //#region Validation for Y Value Member
                            //newSeriesScope.ValidateActiveForm = function () {
                            //    var retValue = true;
                            //    if (!newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm) {
                            //        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Navigation Form.";
                            //        retValue = false;
                            //    }

                            //    return retValue;
                            //}
                            //#endregion


                            //#region Validation for Y Value Member
                            newSeriesScope.ValidateYValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion

                            //#endregion

                            newSeriesScope.OpenTooltipParams = function () {
                                newScope.OpenTooltipParams(newSeriesScope);
                            };

                            newSeriesScope.onAdditionalChartColumnClick = function () {
                                newScope.onAdditionalChartColumnClick(newSeriesScope);
                            };


                            newSeriesScope.onCancelClick = function () {
                                if (seriesDialog) {
                                    seriesDialog.close();
                                }
                            };

                            newSeriesScope.OnOkClick = function () {
                                newSeriesScope.ObjSeriesModel.dictAttributes.ChartType = newScope.ObjChartModel.dictAttributes.ChartType;
                                newScope.SeriesModel.Elements.push(newSeriesScope.ObjSeriesModel);
                                if (seriesDialog) {
                                    seriesDialog.close();
                                }
                            };


                            var seriesDialog = $rootScope.showDialog(newSeriesScope, "Add Series", "Form/views/AddEditSeries.html", { width: 600, height: 600 });

                        };

                        newScope.OpenTooltipParams = function (newSeriesScope) {
                            var newParamScope = newSeriesScope.$new();
                            newScope.objTooltipParamsVM = $rootScope.showDialog(newParamScope, "Set Tooltip Parameters", "Form/views/SetToolTipParameters.html", { width: 500, height: 420 });

                            newParamScope.onSfxChartTooltipTableCancelClick = function () {
                                if (newScope.objTooltipParamsVM) {
                                    newScope.objTooltipParamsVM.close();
                                }
                                //ngDialog.close(newScope.objTooltipParamsVM.id);
                            };

                            newParamScope.onSfxChartTooltipTableOKClick = function () {
                                var lstselectedfields = [];
                                lstselectedfields = GetSelectedFieldList(newSeriesScope.ObjSeriesModel.lstselectedobjecttreefields, lstselectedfields);
                                var DisplayedEntity = getDisplayedEntity(newSeriesScope.ObjSeriesModel.LstDisplayedEntities);
                                var itempath = "";
                                if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                                    itempath = DisplayedEntity.strDisplayName;
                                }
                                function iUpdatetooltipparam(itm) {
                                    var ID = itm.ID;
                                    if (itempath) {
                                        ID = itempath + "." + ID;
                                    }
                                    if (tooltipparam) {
                                        tooltipparam = tooltipparam + ";" + ID;
                                    }
                                    else {
                                        tooltipparam = ID;
                                    }
                                }
                                if (lstselectedfields.length > 0) {
                                    var tooltipparam;

                                    angular.forEach(lstselectedfields, iUpdatetooltipparam);

                                    newSeriesScope.ObjSeriesModel.dictAttributes.sfwTooltipTableParams = tooltipparam;

                                }
                                newParamScope.onSfxChartTooltipTableCancelClick();
                                //ngDialog.close(newScope.objTooltipParamsVM.id);
                            };
                        };

                        newScope.OnRemoveSeriesClick = function () {

                            if (newScope.ObjSeriesModel) {
                                var index = newScope.SeriesModel.Elements.indexOf(newScope.ObjSeriesModel);
                                newScope.SeriesModel.Elements.splice(index, 1);
                                //$rootScope.DeleteItem(newScope.ObjSeriesModel, SeriesModel.Elements);
                                if (index < newScope.SeriesModel.Elements.length) {
                                    newScope.SelectSeries(newScope.SeriesModel.Elements[index]);
                                }
                                else if (newScope.SeriesModel.Elements.length > 0) {
                                    newScope.SelectSeries(newScope.SeriesModel.Elements[index - 1]);
                                }
                                else {
                                    newScope.ObjSeriesModel = undefined;
                                }
                            }

                            //                    if (this.SelectedSeries != null)
                            //                    {
                            //                        foreach (ChildWizardPage objChild in this.ObjChartViewVM.WizardItemCollection)
                            //                    {
                            //                            if (objChild.DataContext is AddSeriesNavigationParamVM)
                            //                    {
                            //                                if ((objChild.DataContext as AddSeriesNavigationParamVM).objSeriesModel.Equals(this.SelectedSeries))
                            //                    {
                            //                                    this.ObjChartViewVM.WizardItemCollection.Remove(objChild);
                            //                        break;
                            //                    }
                            //                }
                            //            }

                            //            this.Series.Elements.Remove(this.SelectedSeries);

                            //            int index = 1;
                            //            foreach (ChildWizardPage objChild in this.ObjChartViewVM.WizardItemCollection)
                            //            {
                            //                if (objChild.DataContext is AddSeriesNavigationParamVM)
                            //            {
                            //                    objChild.Title = string.Format("Step {0} - Enter Series {1} Navigation Parameters. ", index,
                            //                    (objChild.DataContext as AddSeriesNavigationParamVM).objSeriesModel[ApplicationConstants.XMLFacade.NAME_C]);
                            //        }

                            //        index++;
                            //    }


                            //    this.AddOrRemoveFinishButtonVisibility();
                            //}
                        };

                        newScope.OnEditSeries = function (itm) {
                            var newSeriesScope = newScope.$new();
                            newSeriesScope.ObjSeriesModel = itm;
                            newSeriesScope.ObjSeriesModel.ParentVM = undefined;
                            if (newSeriesScope.ObjSeriesModel.FormParameters && newSeriesScope.ObjSeriesModel.FormParameters.length > 0) {
                                newSeriesScope.ShowParameters = true;
                            }
                            else {
                                newSeriesScope.ShowParameters = false;
                            }

                            newSeriesScope.seriesactiveform = true;
                            //$scope.populateParamtersForform = function () {

                            //    $.connection.hubForm.server.getFormParameters(newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm, "").done(function (lstparams) {
                            //        $scope.receiveFormParameters(lstparams, "");
                            //    });
                            //};

                            $scope.receiveFormParameters = function (lstparams, formtype) {

                                if (lstparams) {
                                    newSeriesScope.$evalAsync(function () {
                                        function iUpdateParams(param) {

                                            var lst = params.filter(function (obj) {
                                                return param.ParmeterField == obj.ParmeterField;
                                            });
                                            if (lst && lst.length > 0) {
                                                param.ParmeterValue = lst[0].ParmeterValue;
                                                param.ParmeterConstant = lst[0].ParmeterConstant;
                                            }
                                        }
                                        var params = newSeriesScope.ObjSeriesModel.FormParameters;
                                        newSeriesScope.ObjSeriesModel.FormParameters = [];
                                        if (params && params.length > 0) {


                                            angular.forEach(lstparams, iUpdateParams);
                                        }
                                        newSeriesScope.ObjSeriesModel.FormParameters = lstparams;
                                        newSeriesScope.ShowParameters = true;
                                    });
                                }
                            };
                            newSeriesScope.ClearParamtersForform = function () {
                                if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.FormParameters) {
                                    newSeriesScope.ObjSeriesModel.FormParameters = "";
                                }
                                if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm != "") {
                                    newSeriesScope.seriesactiveform = true;
                                }
                                newSeriesScope.ShowParameters = false;
                            };

                            //#region Validation for Add Series


                            newSeriesScope.isChartFinishDisable = function () {
                                var IsValid = false;
                                newSeriesScope.ErrorMessageForDisplay = "";
                                if (newSeriesScope.ObjSeriesModel) {
                                    if (!newSeriesScope.ValidateSeriesName()) {
                                        IsValid = true;
                                    }
                                    else if (!newSeriesScope.ValidateXValue()) {
                                        IsValid = true;
                                    }
                                    else if (!newSeriesScope.ValidateYValue()) {
                                        IsValid = true;
                                    }
                                    //else if (!newSeriesScope.ValidateActiveForm()) {
                                    //    IsValid = true;
                                    //}
                                }
                                return IsValid;
                            };

                            //#region Validation for Series Name
                            newSeriesScope.ValidateSeriesName = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.Name) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Name.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion

                            //#region Validation for X Value Member
                            newSeriesScope.ValidateXValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.XValueMember) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series XValueMember.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion


                            //#region Validation for Y Value Member
                            newSeriesScope.ValidateYValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion


                            //#region Validation for Y Value Member
                            newScope.ValidateActiveForm = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Navigation Form.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion


                            //#region Validation for Y Value Member
                            newScope.ValidateYValue = function () {
                                var retValue = true;
                                if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                                    newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                                    retValue = false;
                                }

                                return retValue;
                            };
                            //#endregion

                            //#endregion

                            newSeriesScope.OpenTooltipParams = function () {
                                newScope.OpenTooltipParams(newSeriesScope);
                            };

                            newSeriesScope.onAdditionalChartColumnClick = function () {
                                newScope.onAdditionalChartColumnClick(newSeriesScope);
                            };

                            newSeriesScope.onCancelClick = function () {
                                if (seriesDialog) {
                                    seriesDialog.close();
                                }
                            };

                            newSeriesScope.OnOkClick = function () {
                                newSeriesScope.ObjSeriesModel.ParentVM = newScope.SeriesModel;
                                newSeriesScope.onCancelClick();
                            };


                            var seriesDialog = $rootScope.showDialog(newSeriesScope, "Edit Series", "Form/views/AddEditSeries.html", { width: 600, height: 600 });
                        };


                        newScope.onAdditionalChartColumnClick = function (newSeriesScope) {
                            var newColumnScope = newSeriesScope.$new();
                            newColumnScope.sfwAddtionalChartColumns = [];
                            newColumnScope.SelectedObject = newSeriesScope.ObjSeriesModel;
                            //newColumnScope.ParentEntityName = $scope.FormModel.dictAttributes.sfwEntity;
                            if (newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns) {
                                var temp = newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns.split(",");
                                for (var i = 0; i < temp.length; i++) {
                                    newColumnScope.sfwAddtionalChartColumns.push({ Property: temp[i] });
                                }
                            }
                            var AdditionalChartdialog = $rootScope.showDialog(newColumnScope, "Multiple Objects Selection", "Form/views/AdditionalChartColumnsDialog.html", { width: 1000, height: 490 });
                            newColumnScope.onCancelClick = function () {
                                AdditionalChartdialog.close();
                            };
                            newColumnScope.onOkClick = function () {
                                var AddtionalChartColumns = "";
                                for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                                    if (newColumnScope.sfwAddtionalChartColumns[i].Property != "") {
                                        if (AddtionalChartColumns == "") {
                                            AddtionalChartColumns = newColumnScope.sfwAddtionalChartColumns[i].Property;
                                        }
                                        else {
                                            AddtionalChartColumns += "," + newColumnScope.sfwAddtionalChartColumns[i].Property;
                                        }
                                    }
                                }
                                $rootScope.EditPropertyValue(newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns, newSeriesScope.ObjSeriesModel.dictAttributes, "swfAddtionalChartColumns", AddtionalChartColumns);
                                AdditionalChartdialog.close();
                            };
                            newColumnScope.selectRow = function (row) {
                                newColumnScope.selectedRow = row;
                            };
                            newColumnScope.addProperty = function () {
                                newColumnScope.sfwAddtionalChartColumns.push({ Property: "" });
                            };
                            newColumnScope.deleteProperty = function () {
                                var index = -1;
                                if (newColumnScope.selectedRow) {
                                    for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                                        if (newColumnScope.selectedRow == newColumnScope.sfwAddtionalChartColumns[i]) {
                                            index = i;
                                            break;
                                        }
                                    }
                                }
                                if (index > -1) {
                                    newColumnScope.sfwAddtionalChartColumns.splice(index, 1);
                                    if (newColumnScope.sfwAddtionalChartColumns.length > 0) {
                                        if (index > 0) {
                                            newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[index - 1];
                                        }
                                        else {
                                            newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[newColumnScope.sfwAddtionalChartColumns.length - 1];
                                        }
                                    }
                                    else {
                                        newColumnScope.selectedRow = undefined;
                                    }
                                }
                            };
                        };

                        //newScope.onSfxChartSeriesOKClick = function () {
                        //    newScope.ObjSeriesModel.dictAttributes.ChartType = newScope.ObjChartModel.dictAttributes.ChartType;
                        //    newScope.SeriesModel.Elements.push(newScope.ObjSeriesModel);
                        //    //this.AddNavParamPage(ObjAddSeriesDialogVM.ObjSeriesModel);
                        //    newScope.onSfxChartSeriesCancelClick();
                        //}
                        //#endregion


                        newScope.title = "Create Chart";
                        newScope.CreateChartDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateChart.html", { width: 1000, height: 700 });
                        newScope.setTitle = function (title) {
                            if (newScope.title) {
                                newScope.title = title;
                                newScope.CreateChartDialog.updateTitle(newScope.title);
                            }
                        };
                    }
                }
            }
        }
    };

    //#region Add Calendar Control
    $scope.AddSfxCalendarControl = function (controlVM, isAddScheduler) {
        var selectedField;
        for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
            if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {

                selectedField = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                break;
            }

        }
        if (selectedField) {
            if (controlVM) {
                //var cellVM = GetVM('sfwColumn', controlVM);
                var cellVM = null;
                var buttonGroup = GetVM("sfwButtonGroup", controlVM);
                if (buttonGroup) {
                    cellVM = buttonGroup;
                } else {
                    cellVM = GetVM("sfwColumn", controlVM);
                }
                if (cellVM) {
                    var newScope = $scope.$new();
                    newScope.ShowNewParameters = false;
                    newScope.ShowOpenParameters = false;

                    newScope.IsAddScheduler = isAddScheduler;
                    var name = "sfwCalendar";
                    if (isAddScheduler) {
                        name = "sfwScheduler";
                    }
                    newScope.objCalendar = {
                        Name: name,
                        prefix: 'swc',
                        Value: '',
                        dictAttributes: {
                        },
                        Elements: [],
                        Children: []
                    };
                    newScope.objCalendar.isNewSelected = false;
                    newScope.objCalendar.NewbuttonID = "";
                    newScope.objCalendar.NewformID = "";

                    newScope.objCalendar.isOpenSelected = false;
                    newScope.objCalendar.OpenbuttonID = "";
                    newScope.objCalendar.OpenformID = "";

                    var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
                    var itempath = selectedField.ID;
                    if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                        itempath = DisplayedEntity.strDisplayName + "." + selectedField.ID;
                    }
                    strControlID = CreateControlID($scope.FormModel, selectedField.ID, 'sfwCalendar');
                    newScope.objCalendar.dictAttributes.ID = strControlID;
                    newScope.objCalendar.dictAttributes.sfwEntityField = itempath;
                    newScope.FormModel = $scope.FormModel;
                    newScope.ParentEntityName = selectedField.Entity;
                    newScope.SfxMainTable = $scope.SfxMainTable;

                    newScope.title = "Create New Calendar";

                    if (isAddScheduler) {
                        newScope.title = "Create New Scheduler";
                        if (startsWith(strControlID, "cal", 0))
                            strSchedulerID = strControlID.substring(3);
                        //     newScope.lstRelatedDialog = PopulateRelatedDialogList(newScope.SfxMainTable, undefined);

                        if (!newScope.objCalendar.NewbuttonID) {
                            newScope.objCalendar.NewbuttonID = "btnNew" + strSchedulerID;
                        }
                        if (!newScope.objCalendar.OpenbuttonID) {
                            newScope.objCalendar.OpenbuttonID = "btnOpen" + strSchedulerID;
                        }


                    }
                    newScope.onAddButtons = function () {
                        var prefix = "swc";
                        newScope.ialGridButtons = [];
                        if (newScope.objCalendar.isNewSelected && newScope.objCalendar.isNewSelected == 'True') {
                            var newControl = {
                                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newControl.dictAttributes.ID = newScope.objCalendar.NewbuttonID;
                            newControl.dictAttributes.sfwMethodName = "btnNew_Click";
                            newControl.dictAttributes.sfwActiveForm = newScope.objCalendar.NewformID;
                            newControl.dictAttributes.sfwRelatedControl = newScope.objCalendar.dictAttributes.ID;
                            newControl.dictAttributes.Text = "New";
                            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters(newScope.FormNewParameters);

                            newScope.ialGridButtons.push(newControl);
                        }
                        if (newScope.objCalendar.isOpenSelected && newScope.objCalendar.isOpenSelected == 'True') {
                            var newControl = {
                                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newControl.dictAttributes.ID = newScope.objCalendar.OpenbuttonID;
                            newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
                            newControl.dictAttributes.sfwActiveForm = newScope.objCalendar.OpenformID;
                            newControl.dictAttributes.sfwRelatedControl = newScope.objCalendar.dictAttributes.ID;
                            newControl.dictAttributes.Text = "Open";
                            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters(newScope.FormOpenParameters);

                            newScope.ialGridButtons.push(newControl);
                        }

                        newScope.ShowNewParameters = false;
                        newScope.ShowOpenParameters = false;

                    };
                    //#region Validate Calendar Control

                    newScope.ValidateCalendar = function () {
                        newScope.ErrorMessageForDisplay = "";
                        if (newScope.objCalendar.dictAttributes.ID == undefined || newScope.objCalendar.dictAttributes.ID == "") {
                            newScope.ErrorMessageForDisplay = "Enter ID.";
                            return true;
                        }
                        else if (newScope.objCalendar.dictAttributes.ID && !isValidIdentifier(newScope.objCalendar.dictAttributes.ID, false, false)) {
                            newScope.ErrorMessageForDisplay = "Invalid ID.";
                            return true;
                        } else {
                            var lstIds = [];
                            CheckforDuplicateID($scope.FormModel, newScope.objCalendar.dictAttributes.ID, lstIds);
                            if (lstIds.length > 0) {
                                newScope.ErrorMessageForDisplay = "Duplicate ID.";
                                return true;
                            }
                        }
                        if (newScope.objCalendar.dictAttributes.sfwEventId == undefined || newScope.objCalendar.dictAttributes.sfwEventId == "") {
                            newScope.ErrorMessageForDisplay = "Enter Event Id.";
                            return true;
                        }
                        if (newScope.objCalendar.dictAttributes.sfwEventName == undefined || newScope.objCalendar.dictAttributes.sfwEventName == "") {
                            newScope.ErrorMessageForDisplay = "Enter Event Name.";
                            return true;
                        }
                        if (newScope.objCalendar.dictAttributes.sfwEventStartDate == undefined || newScope.objCalendar.dictAttributes.sfwEventStartDate == "") {
                            newScope.ErrorMessageForDisplay = "Enter Event Start Date.";
                            return true;
                        }
                        if (newScope.objCalendar.dictAttributes.sfwEventEndDate == undefined || newScope.objCalendar.dictAttributes.sfwEventEndDate == "") {
                            newScope.ErrorMessageForDisplay = "Enter Event End Date.";
                            return true;
                        }
                        //if (isAddScheduler && !newScope.objCalendar.dictAttributes.sfwEventCategory) {
                        //    newScope.ErrorMessageForDisplay = "Enter Event Category.";
                        //    return true;
                        //}
                        //if (isAddScheduler && !newScope.objCalendar.dictAttributes.sfwRelatedDialogPanel) {
                        //    newScope.ErrorMessageForDisplay = "Enter Related Dialog Panel.";
                        //    return true;
                        //}
                        if (newScope.objCalendar.errors && $ValidationService.isEmptyObj(newScope.objCalendar.errors)) {
                            return true;
                        }

                        return false;
                    };

                    //#endregion

                    newScope.onOkClick = function () {
                        newScope.onAddButtons();
                        //var tableVM = GetVM("sfwTable", cellVM);
                        var tableVM = null;
                        if (cellVM.Name == "sfwButtonGroup") {
                            tableVM = cellVM;
                        } else {
                            tableVM = GetVM('sfwTable', cellVM);
                        }
                        if (tableVM && tableVM.Name != "sfwButtonGroup") {
                            var ColCount = GetMaxColCount(tableVM.Elements[0], tableVM);
                            /*adding new and open button */
                            if (newScope.ialGridButtons.length > 0) {
                                var sfxRowModel = {
                                    Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                                };
                                sfxRowModel.ParentVM = tableVM;

                                for (ind = 0; ind < ColCount; ind++) {
                                    var sfxCellModel = {
                                        Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                                    };
                                    sfxCellModel.ParentVM = sfxRowModel;
                                    sfxRowModel.Elements.push(sfxCellModel);
                                }

                                var index = 0;

                                function AddInsfxCellModel(btn) {
                                    btn.ParentVM = sfxCellModel;
                                    sfxCellModel.Elements.push(btn);
                                }
                                var sfxCellModel = sfxRowModel.Elements[index];
                                angular.forEach(newScope.ialGridButtons, AddInsfxCellModel);
                                $rootScope.PushItem(sfxRowModel, tableVM.Elements);


                            }

                            var newRowModel = {
                                Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newRowModel.ParentVM = tableVM;

                            var newcellModel = {
                                Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newcellModel.ParentVM = newRowModel;


                            newcellModel.Elements.push(newScope.objCalendar);

                            newRowModel.Elements.push(newcellModel);

                            for (ind = 1; ind < ColCount; ind++) {
                                newcellModel = {
                                    Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                                };
                                newcellModel.ParentVM = newRowModel;


                                newRowModel.Elements.push(newcellModel);
                            }

                            $rootScope.PushItem(newRowModel, tableVM.Elements);
                            SetFormSelectedControl($scope.FormModel, newScope.objCalendar, event);
                        }
                        if (tableVM && tableVM.Name == "sfwButtonGroup") {
                            if (newScope.objCalendar) {
                                if (newScope.ialGridButtons.length > 0) {
                                    function AddBtnInCell(btn) {

                                        $rootScope.PushItem(btn, tableVM.Elements);
                                    }
                                    angular.forEach(newScope.ialGridButtons, AddBtnInCell);
                                }

                                $rootScope.PushItem(newScope.objCalendar, tableVM.Elements);
                                SetFormSelectedControl($scope.FormModel, newScope.objCalendar, event);
                            }
                        }
                        newScope.onCancelClick();
                    };

                    newScope.onCancelClick = function () {
                        if (newScope.objCalendar.errors) {
                            newScope.objCalendar.errors = {};
                            $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.objCalendar);
                        }
                        if (newScope.objCalendarDialog) {
                            newScope.objCalendarDialog.close();
                        }
                    };

                    //#region for Expand and Collapse


                    newScope.showParametersForScheduler = function (obj) {

                        newScope.FieldControlCollection = [];
                        if (obj == 'New') {
                            newScope.ShowOpenParameters = false;

                            if (newScope.ShowNewParameters) {
                                newScope.ShowNewParameters = false;
                            }
                            else {
                                newScope.ShowNewParameters = true;
                            }
                        }
                        else if (obj == 'Open') {
                            if (newScope.ShowNewParameters) {
                                newScope.ShowNewParameters = false;
                            }

                            if (newScope.ShowOpenParameters) {
                                newScope.ShowOpenParameters = false;
                            }
                            else {
                                newScope.ShowOpenParameters = true;
                            }
                        }

                        PopulateAvailableControl(newScope.FieldControlCollection, newScope.SfxMainTable, newScope.ShowNewParameters, false);

                        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
                            $scope.$apply(function () {
                                newScope.objGlobleParameters = data;
                                PopulateGlobalParameters(newScope.objGlobleParameters, newScope.FieldControlCollection);
                            });
                        });


                    };

                    newScope.populateParamtersForform = function (formname, formtype) {
                        if (formname) {
                            var filename = formname;
                            if (formtype == "New") {
                                newScope.ShowNewParameters = false;
                            }
                            else if (formtype == "Open") {
                                newScope.ShowOpenParameters = false;
                            }

                            $.connection.hubForm.server.getFormParameters(filename, formtype).done(function (lstparams) {
                                newScope.receiveFormParameters(lstparams, formtype);
                            });
                        }
                    };

                    newScope.receiveFormParameters = function (lstparams, formtype) {
                        if (lstparams) {
                            if (formtype == "New") {
                                newScope.FormNewParameters = lstparams;
                            }
                            else if (formtype == "Open") {
                                newScope.FormOpenParameters = lstparams;
                            }

                        }

                    };

                    newScope.ExpandCollapsedCustomAttrField = function (field, event) {
                        field.IsExpanded = !field.IsExpanded;
                    };


                    newScope.SetFieldClass = function (obj) {
                        if (obj == newScope.SelectedField) {
                            return "selected";
                        }
                    };

                    newScope.SelectFieldClick = function (obj, event) {
                        newScope.SelectedField = obj;
                        if (event) {
                            event.stopPropagation();
                        }
                    };
                    //#endregion


                    newScope.onChangeSchedulerCheckBox = function (formtype) {
                        if (formtype == "New") {
                            newScope.FormNewParameters = [];
                            newScope.objCalendar.NewformID = "";
                            newScope.ShowNewParameters = false;
                        }
                        else if (formtype == "Open") {
                            newScope.FormOpenParameters = [];
                            newScope.objCalendar.OpenformID = "";
                            newScope.ShowOpenParameters = false;
                        }
                    };


                    newScope.objCalendarDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/AddCalendarControl.html", { width: 700, height: 700 });
                    ComponentsPickers.init();
                }
            }
        }
    };
    //#endregion

    //#region Add Repeater Control 

    $scope.AddSfxRepeaterControl = function (controlVM) {
        var strID = CreateControlID($scope.FormModel, "RepeaterViewPanel", "sfwListView");
        var prefix = "swc";
        /*scope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
        scope.ParentEntityName = formodel.dictAttributes.sfwEntity;
        scope.objRepeaterControl.selectedobjecttreefield;
        scope.objRepeaterControl.lstselectedobjecttreefields = [];
        RepeaterControldialog = ngDialog.open({
            template: 'RepeaterControlTemplate',
            scope: scope,
            closeByDocument: false
        });*/
        var selectedField;
        for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
            if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {

                selectedField = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                break;
            }

        }
        if (selectedField) {

            var objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
            var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
            var itempath = selectedField.ID;
            if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                itempath = DisplayedEntity.strDisplayName + "." + selectedField.ID;
            }
            strControlID = CreateControlID($scope.FormModel, selectedField.ID, 'sfwListView');
            objRepeaterControl.dictAttributes.ID = strControlID;
            objRepeaterControl.dictAttributes.sfwEntityField = itempath;

            //var cellVM = GetVM('sfwColumn', controlVM);
            var cellVM = null;
            var buttonGroup = GetVM("sfwButtonGroup", controlVM);
            if (buttonGroup) {
                cellVM = buttonGroup;
            } else {
                cellVM = GetVM("sfwColumn", controlVM);
            }
            if (cellVM) {


                //var tableVM = GetVM("sfwTable", controlVM);
                var tableVM = null;
                if (cellVM.Name == "sfwButtonGroup") {
                    tableVM = cellVM;
                } else {
                    tableVM = GetVM('sfwTable', cellVM);
                }
                if (tableVM) {




                    if (objRepeaterControl) {
                        objRepeaterControl.dictAttributes.sfwSelection = "Many";
                        objRepeaterControl.dictAttributes.sfwCaption = "List View";
                        objRepeaterControl.dictAttributes.AllowPaging = "True";
                        objRepeaterControl.dictAttributes.PageSize = "1";


                        var parentenetityname = selectedField.Entity;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        objRepeaterControl.dictAttributes.sfwDataKeyNames = GetTableKeyFields(parentenetityname, entityIntellisenseList);

                        var prefix = "swc";

                        var objListTableModel = AddListViewTable($scope.FormModel, objRepeaterControl);

                        objRepeaterControl.Elements.push(objListTableModel);
                        objRepeaterControl.initialvisibilty = true;
                        objRepeaterControl.isLoaded = true;

                        $rootScope.UndRedoBulkOp("Start");

                        if (tableVM.Name == "sfwButtonGroup") {
                            $rootScope.PushItem(objRepeaterControl, tableVM.Elements);
                        } else {
                            var ColCount = GetMaxColCount(tableVM.Elements[0], tableVM);


                            var sfxMainRowModel = {
                                Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            sfxMainRowModel.ParentVM = tableVM;

                            var sfxCellModel = {
                                Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            sfxCellModel.ParentVM = sfxMainRowModel;

                            sfxMainRowModel.Elements.push(sfxCellModel);
                            $rootScope.PushItem(sfxMainRowModel, tableVM.Elements);
                            $rootScope.PushItem(objRepeaterControl, sfxCellModel.Elements);
                            for (ind = 1; ind < ColCount; ind++) {
                                var newcellModel = {
                                    Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                                };
                                newcellModel.ParentVM = sfxMainRowModel;

                                $rootScope.PushItem(newcellModel, sfxMainRowModel.Elements);
                            }
                        }



                        $rootScope.UndRedoBulkOp("End");

                        if ($scope.selectControl) {
                            $scope.selectControl(objRepeaterControl);
                        }
                    }
                }

            }
        }
    };

    //#endregion

    $scope.AddSfxGridviewToGrid = function (controlVM) {
        var selectedField;
        for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
            if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {

                selectedField = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                break;
            }
        }
        $scope.AddGridControl(selectedField, controlVM);
    };

    $scope.AddGridControl = function (selectedField, controlVM, isAddToExistingCell, isAddGridWithPanel, gridEntityField) {
        if (selectedField) {
            //if (UtilityFunctions.ValidateSfwObjectFieldForGrid(selectedField.BusObjName, selectedField.ItemPath, this.ObjVM.VMMain.ActiveProject))
            {
                if (controlVM) {
                    // var cellVM = GetVM('sfwColumn', controlVM);
                    var cellVM = null;
                    var buttonGroup = GetVM("sfwButtonGroup", controlVM);
                    if (buttonGroup) {
                        cellVM = buttonGroup;
                    } else {
                        cellVM = GetVM("sfwColumn", controlVM);
                    }

                    if (cellVM) {

                        var newScope = $scope.$new();
                        newScope.objGridView = {
                            Name: 'sfwGridView',
                            prefix: 'swc',
                            Value: '',
                            dictAttributes: {
                                //sfwDatasourceType: "EntityCollection",
                            },
                            Elements: [],
                            Children: []
                        };
                        newScope.objGridView.LstDisplayedEntities = [];
                        newScope.objGridView.lstselectedmultiplelevelfield = [];
                        newScope.objGridView.selectedentityobjecttreefields = [];
                        newScope.cellVM = cellVM;
                        newScope.IsAddToExistingCell = isAddToExistingCell;
                        newScope.IsAddGridWithPanel = isAddGridWithPanel;
                        newScope.selectedEntityField = selectedField;
                        newScope.FormModel = $scope.FormModel;
                        newScope.ParentEntityName = selectedField.Entity;
                        newScope.SfxMainTable = $scope.SfxMainTable;
                        newScope.GridEntityField = gridEntityField;
                        newScope.IsGridInsideListView = false;
                        if (controlVM.Name === "sfwListView" || FindParent(controlVM, "sfwListView")) {
                            newScope.objGridView.IsGridInsideListView = true
                        }

                        newScope.title = "Create New Grid";
                        newScope.LstDisplayedEntities = $scope.LstDisplayedEntities;
                        if ($scope.FormModel.dictAttributes.sfwType == "Tooltip") newScope.skipSecondStep = true;
                        if (newScope.FormModel && newScope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                            newScope.IsPrototype = true;
                        }
                        else {
                            newScope.IsPrototype = false;
                        }
                        newScope.objGridDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateGridViewControl.html", {
                            width: 1000, height: 700
                        });
                        newScope.setTitle = function (title) {
                            if (newScope.title) {
                                newScope.title = title;
                                newScope.objGridDialog.updateTitle(newScope.title);
                            }
                        };
                    }
                }
            }
        }
    };

    $scope.GetSfxControlVM = function () {
        var controlVM = $scope.GetSelectedControl();

        if (controlVM) {
            if (controlVM && (controlVM.Name == "sfwDialogPanel" || controlVM.Name == "sfwPanel") && controlVM.IsMainPanel) {
                var panelVM = controlVM;

                if (null != panelVM.TableVM && panelVM.TableVM.Elements.length > 0) {
                    var sfxRow = panelVM.TableVM.Elements[panelVM.TableVM.Elements.length - 1];
                    if (null != sfxRow && sfxRow.Elements.length > 0) {

                        if ($scope.FormModel.IsLookupCriteriaEnabled) {
                            var tabsVM = $scope.GetTabsVM(panelVM.TableVM);
                            if (tabsVM) {
                                controlVM = $scope.GetDefaultControlVM(tabsVM);
                            }
                            else {
                                if (sfxRow.Elements[0].Name == "sfwColumn") {
                                    controlVM = sfxRow.Elements[0];
                                }
                            }
                        }
                        else {
                            if (sfxRow.Elements[0].Name == "sfwColumn") {
                                controlVM = sfxRow.Elements[0];
                            }
                        }
                    }
                    else {
                        controlVM = undefined;
                    }
                }
                else {
                    controlVM = undefined;
                }
            }
            else if (controlVM.Name == "sfwWizardStep" || controlVM.Name == "HeaderTemplate") {
                var wizardItems = controlVM;
                if (wizardItems.TableVM && wizardItems.TableVM.Elements.length > 0) {
                    var sfxRow = wizardItems.TableVM.Elements[wizardItems.TableVM.Elements.length - 1];
                    if (sfxRow && sfxRow.Elements.length > 0) {
                        if (sfxRow.Elements[0].Name == "sfwColumn") {
                            controlVM = sfxRow.Elements[0];
                        }

                    }
                    else {
                        controlVM = null;
                    }
                }
                else {
                    controlVM = null;
                }
            }
            else if (controlVM.Name == "sfwTabSheet" || controlVM.Name === "sfwListView") {
                var tabsheetVM = controlVM;
                var TableVM = tabsheetVM.Elements[0];
                if (null != TableVM) {
                    var sfxRow = TableVM.Elements[TableVM.Elements.length - 1];
                    if (null != sfxRow && sfxRow.Elements.length > 0) {
                        controlVM = sfxRow.Elements[0].Name == "sfwColumn" ? sfxRow.Elements[0] : null;

                    }
                    else {
                        controlVM = null;
                    }
                }
            }
            else if (controlVM.Name == "sfwTable") { //added this code while selecting table in usercontrol and clicking collection from Object Tree , it was not getting added.
                var tableVM = controlVM;
                if (null != tableVM) {
                    var sfxRow = tableVM.Elements[tableVM.Elements.length - 1];
                    if (null != sfxRow && sfxRow.Elements.length > 0) {
                        controlVM = sfxRow.Elements[0].Name == "sfwColumn" ? sfxRow.Elements[0] : null;

                    }
                    else {
                        controlVM = null;
                    }
                }
            }
            //commented by neha because when we drop in adv criteria it was failing
            //else if (controlVM.Name == "sfwColumn") {
            //    var panelVM = GetVM('sfwPanel', controlVM);
            //    if (!panelVM) {
            //        panelVM = GetVM('sfwDialogPanel', controlVM);

            //    }
            //    if (panelVM) {
            //        if (panelVM.IsMainPanel && $scope.FormModel.IsLookupCriteriaEnabled) {
            //            if (panelVM.TableVM) {
            //                var tabsVM = $scope.GetTabsVM(panelVM.TableVM);

            //                var sfxRow = panelVM.TableVM.Elements[panelVM.TableVM.Elements.length - 1];

            //                if (tabsVM) {
            //                    controlVM = $scope.GetDefaultControlVM(tabsVM);
            //                }
            //                else {
            //                    if (sfxRow && sfxRow.Elements && sfxRow.Elements.length > 0 && sfxRow.Elements[0].Name == "sfwColumn")
            //                        controlVM = sfxRow.Elements[0];
            //                }
            //            }
            //        }
            //    }

            //}
            else if (controlVM.Name == "TemplateField" && controlVM.Elements.length > 0) {
                if (controlVM.Elements[0].Name == "ItemTemplate")
                    controlVM = controlVM.Elements[0];
            }
            else if (controlVM.Name == "sfwWizard") {
                if ($scope.CurrPanel && $scope.CurrPanel.Name == "sfwWizardStep") {
                    controlVM = $scope.CurrPanel;
                    var wizardItems = controlVM;
                    if (wizardItems.TableVM && wizardItems.TableVM.Elements.length > 0) {
                        var sfxRow = wizardItems.TableVM.Elements[wizardItems.TableVM.Elements.length - 1];
                        if (sfxRow && sfxRow.Elements.length > 0) {
                            if (sfxRow.Elements[0].Name == "sfwColumn") {
                                controlVM = sfxRow.Elements[0];
                            }
                        }
                        else {
                            controlVM = null;
                        }
                    }
                    else {
                        controlVM = null;
                    }
                }
            }
            else if (controlVM.Name == "sfwButtonGroup") {

            }
            else if (controlVM.ParentVM && controlVM.ParentVM.Name == "ItemTemplate") {
                controlVM = controlVM.ParentVM;
            }
        }

        return controlVM;
    };

    $scope.GetDefaultControlVM = function (tabsVM) {
        var retVM;
        if (tabsVM.SelectedTabSheet) {
            for (i = 0; i < tabsVM.SelectedTabSheet.Elements.length; i++) {
                if (tabsVM.SelectedTabSheet.Elements[i].Name == "sfwTable") {
                    var TableVM = tabsVM.SelectedTabSheet.Elements[i];
                    if (TableVM) {
                        var sfxRow = TableVM.Elements[TableVM.Elements.length - 1];
                        if (sfxRow && sfxRow.Elements.length > 0) {
                            if (sfxRow.Elements[0].Name == "sfwColumn") {
                                retVM = sfxRow.Elements[0];
                            }
                        }
                    }


                    break;
                }
            }
        }



        return retVM;
    };

    $scope.GetTabsVM = function (TableVM) {
        var tabsVM;
        if (TableVM) {
            var sfxCellVM = TableVM.Elements[0].Elements[0];
            if (sfxCellVM.Elements.length > 0 && sfxCellVM.Elements[0].Name == "sfwTabContainer") {
                var tabContainerVM = sfxCellVM.Elements[0];
                if (tabContainerVM.Elements.length > 0 && tabContainerVM.Elements[0].Name == "Tabs") {
                    tabsVM = tabContainerVM.Elements[0];
                }
            }
        }
        return tabsVM;
    };

    $scope.GetSelectedControl = function () {
        var retVal;

        retVal = $scope.FormModel.SelectedControl;
        //if (this.SelectedControls.Count > 0)
        //{
        //    retVal = this.SelectedControls[0];
        //}

        return retVal;
    };


    //#endregion

    //#region User Control Details 
    $scope.onUserControlDetailsClick = function () {
        var newScope = $scope.$new();
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {
        };
        newScope.showExtraFieldsTab = false;
        newScope.SelectedNewMethod = undefined;
        newScope.formName = "Form";
        var entityname = $scope.FormModel.dictAttributes.sfwEntity;
        newScope.Init = function () {
            if ($scope.InitialLoad) {
                newScope.InitialLoad = {};
                angular.copy($scope.InitialLoad, newScope.InitialLoad);
            }
            else {
                newScope.InitialLoad = { Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            }



            newScope.FormModel = { dictAttributes: {} };
            angular.forEach($scope.FormModel.dictAttributes, function (val, key) {
                newScope.FormModel.dictAttributes[key] = val;
            });

            //#region Call Init Method
            newScope.InitialLoadSectionForDetail();
            //newScope.Init();

            if ($scope.FormModel.errors) {
                newScope.FormModel.errors = {
                };
                newScope.FormModel.errors = $scope.FormModel.errors;
                $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.FormModel);
                if ($ValidationService.isEmptyObj($scope.FormModel.errors)) {
                    $scope.validationErrorList.push(newScope.FormModel);
                }
            }

            if ($scope.FormModel.dictAttributes.sfwType == "Tooltip") {
                newScope.PopulateXmlMethodForTooltip(entityname);
                if (!newScope.SelectedNewMethod) {
                    newScope.SelectedNewMethod = { Name: 'callmethods', Value: '', dictAttributes: {}, Elements: [] };
                    newScope.InitialLoad.Elements.push(newScope.SelectedNewMethod);
                }
                if (newScope.SelectedNewMethod) {
                    newScope.populateXmlMethodParamtersForTooltip(newScope.SelectedNewMethod.dictAttributes.sfwMethodName);
                }

            }
        }

        newScope.InitialLoadSectionForDetail = function () {
            function iteration(objcustommethod) {
                if (objcustommethod.Name == "callmethods") {
                    newScope.SelectedNewMethod = objcustommethod;
                }
            }
            if (newScope.InitialLoad) {
                angular.forEach(newScope.InitialLoad.Elements, iteration);
            }
        };

        newScope.ErrorMessageForDisplay = "";
        newScope.validateUCDetails = function () {
            if (!$scope.FormModel.dictAttributes.sfwEntity) {
                newScope.ErrorMessageForDisplay = "Entity Should be required";
                return false;
            }

            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var extraValueFlag = validateExtraFields(newScope);
            if (extraValueFlag) {
                newScope.ErrorMessageForDisplay = newScope.FormDetailsErrorMessage;
                return false;
            }
            newScope.ErrorMessageForDisplay = "";
            return true;
        };
        newScope.updateEntityTree = function (entityID) {
            $scope.entityTreeName = entityID;
        };
        newScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                if ($scope.FormModel && $scope.FormModel.dictAttributes && $scope.FormModel.dictAttributes.hasOwnProperty("sfwEntity")) {
                    $ValidationService.validateEntity($scope.FormModel, $scope.validationErrorList);
                }
                if ($scope.FormModel.errors && !$scope.FormModel.errors.invalid_entity) {
                    newScope.userControlOkClick();
                    $NavigateToFileService.NavigateToFile(aEntityID, "", "");
                }

            }
        };
        newScope.selectXmlMethodClick = function (aXmlMethodID) {
            if (aXmlMethodID && aXmlMethodID != "" && aXmlMethodID != "NewObject") {
                newScope.userControlOkClick();
                $NavigateToFileService.NavigateToFile($scope.FormModel.dictAttributes.sfwEntity, "methods", aXmlMethodID);
            }
        };


        newScope.PopulateXmlMethodForTooltip = function (entityID) {
            newScope.XmlMethodsCollectionForTooltip = [];
            newScope.entityTreeName = entityID;

            newScope.XmlMethodsCollectionForTooltip.push({ ID: '' });
            newScope.XmlMethodsCollectionForTooltip.push({ ID: 'NewObject' });
            var entityIntellisenseList = $EntityIntellisenseFactory.getXmlMethods(entityID, true);

            function getXmlmethods(item) {
                if (item.MethodType == "Load") {
                    newScope.XmlMethodsCollectionForTooltip.push(item);
                }
            }

            if (entityIntellisenseList && entityIntellisenseList.length > 0) {
                angular.forEach(entityIntellisenseList, getXmlmethods);
            }

            //function getMethods(method) {
            //    angular.forEach(method.XmlMethods, getXmlmethods);
            //}
            //if (entityID) {
            //    var lst = [];
            //    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            //    var parentEntityID = entityID;

            //    while (parentEntityID) {
            //        lst = entityIntellisenseList.filter(function (x) { return x.ID == parentEntityID; });
            //        if (lst && lst.length > 0) {
            //            parentEntityID = lst[0].ParentId;
            //        }
            //        else {
            //            parentEntityID = '';
            //        }
            //    }

            //    if (lst && lst.length > 0) {

            //        angular.forEach(lst, getMethods);
            //    }
            //}
        };
        newScope.populateXmlMethodParamtersForTooltip = function (xmlMethod) {
            newScope.paramtersForXmlMethodforTooltip = [];
            function iterator(itm) {
                newScope.paramtersForXmlMethodforTooltip.push(itm);
            }
            if (xmlMethod) {
                var lst = newScope.XmlMethodsCollectionForTooltip.filter(function (x) { return x.ID == xmlMethod; });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Parameters, iterator);
                }
            }
        };


        newScope.userControlOkClick = function () {

            if (newScope.validateUCDetails()) {
                $rootScope.UndRedoBulkOp("Start");

                if (newScope.objDirFunctions.prepareExtraFieldData) {
                    newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                }

                if (entityname && entityname != newScope.FormModel.dictAttributes.sfwEntity) {
                    for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                        if ($scope.lstLoadedEntityTrees[i].EntityName == entityname) {
                            $scope.lstLoadedEntityTrees[i].EntityName = $scope.FormModel.dictAttributes.sfwEntity;
                            break;
                        }
                    }
                    newScope.updateEntityTree(newScope.FormModel.dictAttributes.sfwEntity);

                    //If previous entity name was 'entPrototype' then remove all 'Data' nodes from all 'sfwGridView' controls.
                    if (entityname === "entPrototype") {
                        var allGridViews = getDescendents($scope.FormModel, "sfwGridView");
                        for (var i = 0, gridCount = allGridViews.length; i < gridCount; i++) {
                            for (var j = 0, gridElementCount = allGridViews[i].Elements.length; j < gridElementCount; j++) {
                                if (allGridViews[i].Elements[j].Name.toLowerCase() === "data") {
                                    $rootScope.DeleteItem(allGridViews[i].Elements[j], allGridViews[i].Elements);
                                    $rootScope.EditPropertyValue(allGridViews[i].prototypemodel, allGridViews[i], "prototypemodel", null);
                                    break;
                                }
                            }
                        }
                    }
                }


                if (newScope.FormModel.dictAttributes.sfwType == "Tooltip") {

                    if (newScope.objDirFunctions.prepareExtraFieldData) {
                        newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                    }

                    angular.forEach(newScope.FormModel.dictAttributes, function (val, key) {
                        $rootScope.EditPropertyValue($scope.FormModel.dictAttributes[key], $scope.FormModel.dictAttributes, key, val);
                        // $scope.FormModel.dictAttributes[key] = val;
                    });

                    $scope.FormModel.errors = {};
                    if (newScope.FormModel.errors) {
                        $scope.FormModel.errors = newScope.FormModel.errors;
                        $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.FormModel);
                        if ($ValidationService.isEmptyObj(newScope.FormModel.errors)) {
                            $scope.validationErrorList.push($scope.FormModel);
                        }
                    }

                    if (newScope.SelectedNewMethod && (newScope.SelectedNewMethod.dictAttributes.sfwMethodName == undefined || newScope.SelectedNewMethod.dictAttributes.sfwMethodName == "")) {
                        var NewMethodIndex = newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod);
                        if (NewMethodIndex > -1) {
                            newScope.InitialLoad.Elements.splice(NewMethodIndex, 1);
                            newScope.SelectedNewMethod = undefined;
                        }
                    }
                    if (!newScope.SelectedNewMethod) {
                        newScope.InitialLoad = undefined;
                    }

                    if (newScope.InitialLoad && !$scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                        $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, 0);
                    }
                    else if (newScope.InitialLoad && $scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                        for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                            if ($scope.FormModel.Elements[i].Name == "initialload") {
                                $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                                $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, i);
                                break;
                            }
                        }
                    }
                    else if (!newScope.InitialLoad) {
                        for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                            if ($scope.FormModel.Elements[i].Name == "initialload") {
                                $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                                //$scope.FormModel.Elements.splice(i, 1);
                                break;
                            }
                        }
                    }

                    $rootScope.EditPropertyValue($scope.InitialLoad, $scope, "InitialLoad", newScope.InitialLoad);
                }
                else {

                    if (newScope.objDirFunctions.prepareExtraFieldData) {
                        newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                    }

                    angular.forEach(newScope.FormModel.dictAttributes, function (val, key) {
                        $rootScope.EditPropertyValue($scope.FormModel.dictAttributes[key], $scope.FormModel.dictAttributes, key, val);
                        // $scope.FormModel.dictAttributes[key] = val;
                    });

                    $scope.FormModel.errors = {};
                    if (newScope.FormModel.errors) {
                        $scope.FormModel.errors = newScope.FormModel.errors;
                        $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.FormModel);
                        if ($ValidationService.isEmptyObj(newScope.FormModel.errors)) {
                            $scope.validationErrorList.push($scope.FormModel);
                        }
                    }

                }

                $rootScope.UndRedoBulkOp("End");
            }

            newScope.dialogObj.close();
        };

        newScope.dialogObj = $rootScope.showDialog(newScope, "Details", "Form/views/userControlDetails.html", { width: 550, height: 555 });

        newScope.Init();
    };
    //#endregion

    //#region Form Details Method
    $scope.onDetailClick = function () {
        $rootScope.IsLoading = true;

        var newScope = $scope.$new(true);
        newScope.MainQuery = undefined;
        newScope.objExtraFields = [];
        newScope.objFormExtraFields = $scope.objFormExtraFields;
        newScope.objDirFunctions = {
        };
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Form";
        newScope.FormModel = {};
        newScope.remoteObject = null;
        if ($scope.InitialLoad) {
            newScope.InitialLoad = {};
            angular.copy($scope.InitialLoad, newScope.InitialLoad);
        }
        else {
            newScope.InitialLoad = { Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        }

        newScope.InitialLoadSectionForDetail = function () {
            newScope.$evalAsync(function () {
                newScope.SubQueryCollection = { Elements: [] };
                function iteration(objcustommethod) {
                    if (objcustommethod.Name == "callmethods") {
                        if (!objcustommethod.dictAttributes.sfwMode) {
                            newScope.SelectedNewMethod = objcustommethod;
                            newScope.SelectedUpdateMethod = "";
                            newScope.FormModel.IsSameAsNew = true;
                        }
                        if (objcustommethod.dictAttributes.sfwMode == 'New' || objcustommethod.dictAttributes.sfwMode == 'All') {
                            newScope.SelectedNewMethod = objcustommethod;
                        }
                        if (objcustommethod.dictAttributes.sfwMode == 'Update' || objcustommethod.dictAttributes.sfwMode == 'All') {
                            newScope.SelectedUpdateMethod = objcustommethod;
                        }
                    }
                    else if (objcustommethod.Name == "query") {
                        var strQuery = objcustommethod.dictAttributes.sfwQueryRef;
                        if ($scope.IsSubQuery(strQuery)) {

                            newScope.SubQueryCollection.Elements.push(objcustommethod);
                            newScope.SelectedSubQuery = objcustommethod;
                        }

                        else if (!newScope.MainQuery) {
                            newScope.MainQuery = objcustommethod;
                        }
                        else {
                            newScope.SubQueryCollection.Elements.push(objcustommethod);
                        }
                    }
                    else if (objcustommethod.Name == "session") {
                        newScope.SessionFields = objcustommethod;
                        newScope.SessionFields.lstselectedobjecttreefields = [];
                        newScope.SessionFields.LstDisplayedEntities = [];
                    }
                }
                if (newScope.InitialLoad) {
                    angular.forEach(newScope.InitialLoad.Elements, iteration);
                }
                if ($scope.FormModel.dictAttributes.sfwType == "Maintenance") {
                    if (newScope.InitialLoad) {
                        if (newScope.SessionFields == undefined) {
                            newScope.SessionFields = {
                                Name: 'session', Value: '', dictAttributes: {}, Elements: [], Children: []
                            };
                            newScope.SessionFields.lstselectedobjecttreefields = [];
                            newScope.SessionFields.LstDisplayedEntities = [];
                        }
                    }
                }
            });
        };

        newScope.FormModel = { dictAttributes: {} };
        angular.forEach($scope.FormModel.dictAttributes, function (val, key) {
            newScope.FormModel.dictAttributes[key] = val;
        });

        if ($scope.FormModel.errors) {
            newScope.FormModel.errors = {};
            newScope.FormModel.errors = $scope.FormModel.errors;
            $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.FormModel);
            if ($ValidationService.isEmptyObj($scope.FormModel.errors)) {
                $scope.validationErrorList.push(newScope.FormModel);
            }
        }
        newScope.FormModel.RemoteObjectCollection = [];
        if ($scope.FormModel.RemoteObjectCollection) {
            angular.copy($scope.FormModel.RemoteObjectCollection, newScope.FormModel.RemoteObjectCollection);
        }
        newScope.$evalAsync(function () {
            newScope.IsPrototypeDetails = false;
        });

        if ($scope.FormModel.dictAttributes.ID.startsWith("wfp")) {
            newScope.$evalAsync(function () {
                newScope.IsPrototypeDetails = true;
            });
        }
        var entityname = $scope.FormModel.dictAttributes.sfwEntity;
        $.connection.hubMain.server.getFormExtraData(entityname).done(function (extradata) {
            newScope.$evalAsync(function () {
                newScope.FormModel.objExtraData = extradata;
                newScope.populateXmlMethods(newScope.MethodType);
                newScope.Init();
            });
        });


        newScope.populateXmlMethods = function (aMethodType) {
            newScope.XmlNewMethodsCollection = [];
            newScope.XmlUpdateMethodsCollection = [];
            if (aMethodType != 'SrvMethod') {
                newScope.paramtersForNewMethod = [];
                newScope.paramtersForUpdateMethod = [];
            }
            if (newScope.FormModel.objExtraData) {
                var dummyObj = {
                    Name: '', Value: '', dictAttributes: { ID: "" }, Elements: [], Children: []
                };
                if (newScope.FormModel.objExtraData.lstMethodsList) {
                    var methodsModel = newScope.FormModel.objExtraData.lstMethodsList.filter(function (x) {
                        return x.Name.toLowerCase() == "methods";
                    });
                    newScope.XmlUpdateMethodsCollection.push(dummyObj);
                    newScope.XmlNewMethodsCollection.push(dummyObj);
                    var dummyObj = {
                        Name: '', Value: '', dictAttributes: { ID: "NewObject" }, Elements: [], Children: []
                    };
                    newScope.XmlNewMethodsCollection.push(dummyObj);
                    if (methodsModel && methodsModel.length > 0) {
                        for (var i = 0; i < methodsModel.length; i++) {
                            for (j = 0; j < methodsModel[i].Elements.length; j++) {
                                var item = methodsModel[i].Elements[j];
                                if (item.Name == "method") {
                                    if (item.dictAttributes.sfwMode == 'New' || item.dictAttributes.sfwMode == 'All') {
                                        newScope.XmlNewMethodsCollection.push(item);
                                    }
                                    if (item.dictAttributes.sfwMode == 'Update' || item.dictAttributes.sfwMode == 'All') {
                                        newScope.XmlUpdateMethodsCollection.push(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        if ($scope.FormModel.dictAttributes.sfwType != "Lookup") {
            newScope.populateXmlMethods();
        }
        newScope.RemoteObjectCollection = [];
        newScope.IsEntityTabSelected = false;
        newScope.IsGroupTabSelected = false;
        newScope.IsSessionTabSelected = false;
        newScope.SrvMethodCollection = [];
        newScope.lstWebsite = [];
        var objNewDialog;
        var temObj = ConfigurationFactory.getLastProjectDetails();
        if (temObj && temObj != null) {
            var tempWbsite = ConfigurationFactory.getLastProjectDetails().Website;
            if (tempWbsite && tempWbsite != null && tempWbsite.contains(";")) {
                newScope.lstWebsite = ConfigurationFactory.getLastProjectDetails().Website.split(";");
            }
            else {
                newScope.lstWebsite.push(ConfigurationFactory.getLastProjectDetails().Website);
            }
            newScope.lstWebsite.splice(0, 0, "");
        }
        newScope.Init = function () {
            newScope.IsSameAsNewDisabled = true;
            var controlNames = $scope.lstControl.filter(function (ctrl) { return ctrl.attributetype && ctrl.attributetype === "value"; }).map(function (itm) { return itm.method; });
            var formcontrols = getDescendents($scope.FormModel);
            newScope.valueTypeFormControls = formcontrols.filter(function (ctrl) { return controlNames.indexOf(ctrl.Name) > -1 && ctrl.dictAttributes.ID && ctrl.dictAttributes.sfwIsCaption !== "True"; }).map(function (itm) { return itm.dictAttributes.ID; });

            $scope.PopulateRemoteObjects();

            //#region for loading message description while opening detail popup
            if (newScope.FormModel.dictAttributes.sfwNewMessageID) {
                newScope.populateMessageForNewMessage(newScope.FormModel.dictAttributes.sfwNewMessageID);
            }

            if (newScope.FormModel.dictAttributes.sfwOpenMessageID) {
                newScope.populateMessageForOpenMessage(newScope.FormModel.dictAttributes.sfwOpenMessageID);
            }
            //#endregion
            newScope.MethodType = "XmlMethod";
            if (newScope.FormModel.dictAttributes.sfwRemoteObject) {
                newScope.MethodType = "SrvMethod";
                newScope.remoteObject = newScope.FormModel.dictAttributes.sfwRemoteObject;
                $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                    newScope.$evalAsync(function () {
                        if (data) {
                            newScope.FormModel.RemoteObjectCollection = data;
                            if (newScope.FormModel.RemoteObjectCollection && newScope.FormModel.RemoteObjectCollection.length > 0) {
                                newScope.FormModel.RemoteObjectCollection.splice(0, 0, {
                                    dictAttributes: {
                                        ID: ""
                                    }
                                });
                            }
                            newScope.PopulateRemoteObject();
                        }
                    });
                });
                //newScope.PopulateRemoteObject();
            }
            else {
                if (newScope.SelectedNewMethod) {
                    newScope.populateParamtersForNew(newScope.SelectedNewMethod);
                } else {
                    newScope.SelectedNewMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "New" }, Elements: [] };
                }
                if (newScope.SelectedUpdateMethod) {
                    newScope.populateParamtersForUpdate(newScope.SelectedUpdateMethod);
                } else {
                    newScope.SelectedUpdateMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "Update" }, Elements: [] };
                }
            }

            newScope.lstButtons = [];
            if (newScope.FormModel.objExtraData && newScope.FormModel.objExtraData.lstRulesList) {
                newScope.lstLogRules = newScope.FormModel.objExtraData.lstRulesList;
                newScope.lstLogRules.splice(0, 0, { dictAttributes: { ID: '' } });
            }

            if ($scope.SfxMainTable && $scope.SfxMainTable.Elements) {
                FindControlListByName($scope.SfxMainTable, "sfwButton", newScope.lstButtons);
            }
            if ($scope.FormModel.dictAttributes.sfwType == "Lookup") {
                if (!newScope.MainQuery) {
                    newScope.MainQuery = { Name: 'query', Value: '', dictAttributes: {}, Elements: [] };
                    newScope.InitialLoad.Elements.push(newScope.MainQuery);
                }
                //else {
                //    newScope.MainQuery = {};
                //    angular.copy($scope.MainQuery, newScope.MainQuery);
                //}
            }

            //validate session fileds 
            if (newScope.SessionFields) {
                angular.forEach(newScope.SessionFields.Elements, function (field) {
                    newScope.validateSeesionID(field);
                });
                newScope.valiidateSessionField();
            }
            if (newScope.SubQueryCollection && newScope.SubQueryCollection.Elements.length > 0) {
                newScope.validateQueryID();
            }
            $rootScope.IsLoading = false;
            objNewDialog = $rootScope.showDialog(newScope, "Form Details", "Form/views/FormDetails.html", { width: 700, height: 700 });
        };

        //#region Method type methods
        newScope.PopulateRemoteObject = function () {
            newScope.RemoteObjectCollection = newScope.FormModel.RemoteObjectCollection;
            if (newScope.RemoteObjectCollection && newScope.RemoteObjectCollection.length > 0) {
                if (newScope.RemoteObjectCollection[0].dictAttributes.ID != "") {
                    newScope.RemoteObjectCollection.splice(0, 0, { dictAttributes: { ID: "" } });
                }
                newScope.onRemoteObjectChange();
            }
            if (newScope.FormModel && newScope.FormModel.dictAttributes && newScope.FormModel.dictAttributes.sfwType != "Lookup") {
                if (newScope.SelectedNewMethod) {
                    newScope.populateSrvParamtersForNew(newScope.SelectedNewMethod);
                } else {
                    newScope.SelectedNewMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "New" }, Elements: [] };
                }
                if (newScope.SelectedUpdateMethod) {
                    newScope.populateSrvParamtersForUpdate(newScope.SelectedUpdateMethod);
                } else {
                    newScope.SelectedUpdateMethod = { Name: 'callmethods', Value: '', dictAttributes: { sfwMode: "Update" }, Elements: [] };
                }
            }
        };

        //newScope.receiveRemoteObjectCollection = function (data) {
        //    newScope.$evalAsync(function () {
        //        newScope.RemoteObjectCollection = data;
        //        newScope.RemoteObjectCollection.splice(0, 0, { dictAttributes: { ID: "" } });
        //        newScope.onRemoteObjectChange();
        //        if ($scope.SelectedNewMethod) {
        //            newScope.populateSrvParamtersForNew($scope.SelectedNewMethod.dictAttributes.sfwMethodName);
        //        }
        //        if ($scope.SelectedUpdateMethod) {
        //            newScope.populateSrvParamtersForUpdate($scope.SelectedUpdateMethod.dictAttributes.sfwMethodName);
        //        }
        //    });
        //}

        newScope.onRemoteObjectChange = function () {
            newScope.isReset = true;
            newScope.SrvMethodCollection = [];  /*Bug 8836:In Lookup Form Details Popup- On Selecting 'Blank' in Remote Object - Load Methods List Should not be Displayed.*/
            if (newScope.FormModel.dictAttributes.sfwRemoteObject != undefined && newScope.FormModel.dictAttributes.sfwRemoteObject != "") {
                if (newScope.remoteObject != newScope.FormModel.dictAttributes.sfwRemoteObject) {
                    if (newScope.SelectedNewMethod) {
                        newScope.SelectedNewMethod.dictAttributes.sfwMethodName = "";
                    }
                    if (newScope.SelectedUpdateMethod) {
                        newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName = "";
                    }
                    newScope.paramtersForNewMethod = [];
                    newScope.paramtersForUpdateMethod = [];
                }
                var lst = newScope.RemoteObjectCollection.filter(function (itm) {
                    return itm.dictAttributes.ID == newScope.FormModel.dictAttributes.sfwRemoteObject;
                });
                if (lst && lst.length > 0) {
                    newScope.SrvMethodCollection = lst[0].Elements;
                    newScope.SrvNewMethodCollection = lst[0].Elements.filter(function (itm) { return itm.dictAttributes.sfwMode == "New" || itm.dictAttributes.sfwMode == "All"; });
                    newScope.SrvUpdateMethodCollection = lst[0].Elements.filter(function (itm) { return itm.dictAttributes.sfwMode == "Update" || itm.dictAttributes.sfwMode == "All"; });
                    newScope.SrvMethodCollection.splice(0, 0, { dictAttributes: { ID: '' } });
                }

            }
            else {
                newScope.SrvNewMethodCollection = [];
                newScope.SrvUpdateMethodCollection = [];
            }
        };

        newScope.onMethodTypeChange = function (aMethodType) {
            newScope.MethodType = aMethodType;
            newScope.PopulateRemoteObject();
            if (newScope.MainQuery && newScope.MainQuery.dictAttributes) {
                newScope.MainQuery.dictAttributes.sfwMethodName = "";
            }
            if (newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMethodName = "";
            }
            if (newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName = "";
            }

            newScope.FormModel.dictAttributes.sfwRemoteObject = "";

            newScope.FormModel.dictAttributes.sfwGridCollection = "";


            newScope.paramtersForNewMethod = [];


            newScope.paramtersForUpdateMethod = [];


            newScope.SrvMethodCollection = [];


            newScope.SrvNewMethodCollection = [];


            newScope.SrvUpdateMethodCollection = [];

        };
        newScope.selectXmlMethodClick = function (aXmlMethodID) {
            if (aXmlMethodID && aXmlMethodID != "" && aXmlMethodID != "NewObject") {
                //objNewDialog.close();
                newScope.OkClick();
                $NavigateToFileService.NavigateToFile(newScope.FormModel.dictAttributes.sfwEntity, "methods", aXmlMethodID);
            }
        };

        //#endregion

        //#region Main Query Methods
        newScope.openMainQueryDialog = function () {
            var newQueryScope = newScope.$new();
            newQueryScope.strSelectedQuery = newScope.MainQuery.dictAttributes.sfwQueryRef;

            newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });

            newQueryScope.$on('onQueryClick', function (event, data) {
                if (!$scope.IsSubQuery(data)) {
                    if (data.contains('.')) {
                        newScope.MainQuery.dictAttributes.ID = data.split('.')[0];
                    }

                    newScope.MainQuery.dictAttributes.sfwQueryRef = data;
                }
                if (event) {
                    event.stopPropagation();
                }
            });

        };

        newScope.onMainQueryChange = function (data) {
            newScope.updateMainQueryChange();
            if (!$scope.IsSubQuery(data)) {
                if (!newScope.MainQuery) {
                    newScope.MainQuery = {
                        Name: 'query', Value: '', dictAttributes: {}, Elements: []
                    };
                    newScope.InitialLoad.Elements.push(newScope.MainQuery);
                }
                if (data && data.contains('.')) {
                    if (!newScope.MainQuery.dictAttributes.ID) {
                        newScope.MainQuery.dictAttributes.ID = data.split('.')[0];
                        newScope.MainQueryIDChange(newScope.MainQuery);
                    }
                }
                else if (!data) {
                    newScope.MainQuery.dictAttributes.ID = "";
                    newScope.MainQueryIDChange(newScope.MainQuery);
                }
            }

        };
        newScope.MainQueryIDChange = function (aobjMainQuery) {
            if (newScope.InitialLoad && newScope.InitialLoad.Elements) {
                for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                    if (newScope.InitialLoad.Elements[i].dictAttributes && newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef == aobjMainQuery.dictAttributes.sfwQueryRef) {
                        newScope.InitialLoad.Elements[i].dictAttributes.ID = aobjMainQuery.dictAttributes.ID;
                    }
                }
            }
        };
        newScope.updateMainQueryChange = function () {
            if (newScope.InitialLoad && newScope.InitialLoad.Elements) {
                for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                    if (newScope.InitialLoad.Elements[i].dictAttributes && newScope.InitialLoad.Elements[i].dictAttributes.ID == newScope.MainQuery.dictAttributes.ID) {
                        newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef = newScope.MainQuery.dictAttributes.sfwQueryRef;
                    }
                }
            }
        };

        newScope.onSubQueryClick = function (obj) {
            if (obj) {
                newScope.SelectedSubQuery = obj;
            }
        };
        //#endregion

        //#region Group functionality Implemented
        newScope.getGroupList = function (event) {
            var input = $(event.target);
            var lstGroupList = [];
            lstGroupList = createGroupList();
            if (lstGroupList.length > 0 && lstGroupList[0].Elements.length > 0) {
                var data = lstGroupList[0].Elements;
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    if ($(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    }
                    event.preventDefault();
                }
                else {
                    setSingleLevelAutoComplete(input, data, newScope, "ID");
                }
            }
            else {
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    event.preventDefault();
                }
            }
        };

        newScope.showGroupList = function (event) {
            var inputElement = $(event.target).prevAll("input[type='text']");
            var lstGroupList = [];

            lstGroupList = createGroupList();
            if (lstGroupList.length > 0 && lstGroupList[0].Elements.length > 0) {
                var data = lstGroupList[0].Elements;
                inputElement.focus();

                setSingleLevelAutoComplete(inputElement, data, newScope, "ID");
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
            }
            else {
                setSingleLevelAutoComplete(inputElement, [], newScope, "ID");
            }


            if (event) {
                event.stopPropagation();
            }
        };

        newScope.validdateGroupList = function (property) {
            if (property) {
                var lstGroupList = createGroupList();
                var list = [];
                if (lstGroupList && lstGroupList.length > 0) {
                    angular.forEach(lstGroupList[0].Elements, function (obj) {
                        if (obj.dictAttributes && obj.dictAttributes.ID) {
                            list.push(obj.dictAttributes.ID);
                        }
                    });
                }
                var inputVal = newScope.FormModel.dictAttributes && newScope.FormModel.dictAttributes[property] ? newScope.FormModel.dictAttributes[property] : "";
                $ValidationService.checkValidListValue(list, newScope.FormModel, inputVal, property, property, CONST.VALIDATION.INVALID_GROUP, $scope.validationErrorList);
            }
        };
        var createGroupList = function () {
            if (newScope.FormModel.objExtraData) {

                var lstGroupList = newScope.FormModel.objExtraData.lstGroupsList;

                if (lstGroupList && lstGroupList.length > 0) {
                    return lstGroupList;
                } else {
                    lstGroupList = [];
                    return lstGroupList;
                }
            }
        };
        newScope.selectGroupClick = function (aGroupID, property) {
            if (property) {
                newScope.validdateGroupList(property);
            }

            if ((newScope.FormModel.errors && !newScope.FormModel.errors[property])) {
                if (aGroupID && aGroupID != " ") {
                    objNewDialog.close();
                    $NavigateToFileService.NavigateToFile(newScope.FormModel.dictAttributes.sfwEntity, "groupslist", aGroupID);
                }
            }
        };
        //#endregion

        //#region for changing MessageID for New message ID
        newScope.populateMessageForNewMessage = function (messageID) {

            var messageIDFound = false;
            if (messageID && messageID.trim().length > 0) {
                var messages = $scope.lstMessages.filter(function (x) {
                    return x.MessageID == messageID;
                });
                if (messages && messages.length > 0) {
                    $scope.newDisplayMessage = messages[0].DisplayMessage;

                    if (messages[0].SeverityValue == 'I') {
                        $scope.newSeverityValue = "Information";
                    }
                    else if (messages[0].SeverityValue == 'E') {
                        $scope.newSeverityValue = "Error";
                    }
                    else if (messages[0].SeverityValue == 'W') {
                        $scope.newSeverityValue = "Warnings";
                    }

                    messageIDFound = true;

                }
            }

            if (!messageIDFound) {
                $scope.newDisplayMessage = "";
                $scope.newSeverityValue = "";
            }
        };
        //#endregion

        //#region for changing MessageID for Open Message ID
        newScope.populateMessageForOpenMessage = function (messageID) {

            var messageIDFound = false;
            if (messageID && messageID.trim().length > 0) {
                var messages = $scope.lstMessages.filter(function (x) {
                    return x.MessageID == messageID;
                });
                if (messages && messages.length > 0) {
                    $scope.openDisplayMessage = messages[0].DisplayMessage;

                    if (messages[0].SeverityValue == 'I') {
                        $scope.openSeverityValue = "Information";
                    }
                    else if (messages[0].SeverityValue == 'E') {
                        $scope.openSeverityValue = "Error";
                    }
                    else if (messages[0].SeverityValue == 'W') {
                        $scope.openSeverityValue = "Warnings";
                    }

                    messageIDFound = true;

                }
            }

            if (!messageIDFound) {
                $scope.openDisplayMessage = "";
                $scope.openSeverityValue = "";
            }
        };
        //#endregion

        //#region InitialLoad for CallMethods(Maintenance Entity Tab)

        //#region Populate Xml Methods while changing the Entity Name
        newScope.populateXmlMethodsForEntityChange = function (entityName) {
            newScope.isReset = true;
            var entity = entityName;
            newScope.XmlNewMethodsCollection = [];
            newScope.XmlUpdateMethodsCollection = [];
            newScope.lstLogRules = [];
            newScope.FormModel.objExtraData = {
            };
            if (newScope.MethodType == "XmlMethod") {
                if (newScope.SelectedNewMethod && newScope.SelectedNewMethod.dictAttributes.sfwMethodName) {
                    newScope.SelectedNewMethod.dictAttributes.sfwMethodName = "";
                }
                if (newScope.SelectedUpdateMethod && newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName) {
                    newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName = "";
                }
                newScope.paramtersForUpdateMethod = [];
                newScope.paramtersForNewMethod = [];
            }
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lst = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
            //$scope.entityTreeName = entityName;
            if (lst && lst.length > 0) {
                $.connection.hubMain.server.getFormExtraData(entity).done(function (extradata) {
                    newScope.FormModel.objExtraData = extradata;
                    newScope.populateXmlMethods(newScope.MethodType);
                    newScope.lstLogRules = newScope.FormModel.objExtraData.lstRulesList;
                    newScope.lstLogRules.splice(0, 0, { dictAttributes: { ID: '' } });
                });
            }
            //  createRuledata(); // when entity change rule data list also update

            //if ($scope.SessionFields) {
            //    $rootScope.EditPropertyValue($scope.SessionFields.Elements, $scope.SessionFields, "Elements", []);
            //}

            // when entity change validate seesion field present in current entity
            if (newScope.SessionFields) {
                newScope.valiidateSessionField();
            }
        };
        //#endregion

        //#region Load Parameters for New Method
        newScope.populateParamtersForNew = function (xmlMethod) {
            newScope.paramtersForNewMethod = [];
            if (!newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: xmlMethod.dictAttributes.sfwMethodName }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedNewMethod);
            }
            function iAddparamtersForNewMethod(itm) {
                if (itm.Name == "parameter") {
                    newScope.paramtersForNewMethod.push(itm);
                }
            }
            if (xmlMethod.dictAttributes.sfwMethodName && newScope.XmlNewMethodsCollection) {

                var lst = newScope.XmlNewMethodsCollection.filter(function (x) {
                    return x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "Update";
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, iAddparamtersForNewMethod);

                }
            }

            if (newScope.XmlUpdateMethodsCollection) {
                var lst = newScope.XmlUpdateMethodsCollection.filter(function (x) {
                    return xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New";
                });
                if (lst && lst.length > 0) {
                    newScope.IsSameAsNewDisabled = false;
                }
                else {
                    newScope.IsSameAsNewDisabled = true;
                    newScope.FormModel.IsSameAsNew = false;
                }
            }

            if (!newScope.FormModel.IsSameAsNew && newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMode = "New";
            }
        };

        newScope.populateSrvParamtersForNew = function (srvMethod) {
            if (!newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: srvMethod.dictAttributes.sfwMethodName }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedNewMethod);
            }
            newScope.paramtersForNewMethod = [];
            function iAddparamtersForNewMethod(itm) {
                if (itm.Name == "parameter") {
                    newScope.paramtersForNewMethod.push(itm);
                }
            }

            if (srvMethod.dictAttributes.sfwMethodName && newScope.SrvNewMethodCollection) {

                var lst = newScope.SrvNewMethodCollection.filter(function (x) {
                    return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "Update";
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, iAddparamtersForNewMethod);

                }
            }
            if (newScope.SrvUpdateMethodCollection) {
                var lst = newScope.SrvUpdateMethodCollection.filter(function (x) { return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New"; });
                if (lst && lst.length > 0) {
                    newScope.IsSameAsNewDisabled = false;
                }
                else {
                    newScope.IsSameAsNewDisabled = true;
                    newScope.FormModel.IsSameAsNew = false;
                }
            }
            if (!newScope.FormModel.IsSameAsNew && newScope.SelectedNewMethod) {
                newScope.SelectedNewMethod.dictAttributes.sfwMode = "New";
            }
        };
        //#endregion

        //#region Load Parameters for Update Method
        newScope.populateParamtersForUpdate = function (xmlMethod) {
            newScope.paramtersForUpdateMethod = [];
            if (!newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: xmlMethod.dictAttributes.sfwMethodName, sfwMode: "Update" }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedUpdateMethod);
            }
            xmlMethod.dictAttributes.sfwMode = "Update";
            newScope.paramtersForUpdateMethod = [];
            function iAddparamtersForUpdateMethod(itm) {
                if (itm.Name == "parameter") {
                    newScope.paramtersForUpdateMethod.push(itm);
                }
            }
            if (xmlMethod.dictAttributes.sfwMethodName && newScope.XmlUpdateMethodsCollection) {

                var lst = newScope.XmlUpdateMethodsCollection.filter(function (x) {
                    return x.dictAttributes.ID == xmlMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New";
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, iAddparamtersForUpdateMethod);

                }
            }
        };
        newScope.populateSrvParamtersForUpdate = function (srvMethod) {
            if (!newScope.SelectedUpdateMethod) {
                newScope.SelectedUpdateMethod = {
                    Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: srvMethod.dictAttributes.sfwMethodName, sfwMode: "Update" }
                };
            }
            if (newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod) <= -1) {
                newScope.InitialLoad.Elements.push(newScope.SelectedUpdateMethod);
            }
            srvMethod.dictAttributes.sfwMode = "Update";
            newScope.paramtersForUpdateMethod = [];
            function iparamtersForUpdateMethod(itm) {
                if (itm.Name == "parameter") {
                    newScope.paramtersForUpdateMethod.push(itm);
                }
            }
            if (srvMethod.dictAttributes.sfwMethodName && newScope.SrvUpdateMethodCollection) {

                var lst = newScope.SrvUpdateMethodCollection.filter(function (x) {
                    return x.dictAttributes.ID == srvMethod.dictAttributes.sfwMethodName && x.dictAttributes.sfwMode != "New";
                });
                if (lst && lst.length > 0) {

                    angular.forEach(lst[0].Elements, iparamtersForUpdateMethod);

                }
            }
        };
        //#endregion

        //#endregion

        //#region SubQuery Dialog

        newScope.openSubQueryDialog = function () {
            var newQueryScope = $scope.$new();
            newQueryScope.subQueryType = "SubSelectQuery";

            newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });


            newQueryScope.$on('onQueryClick', function (event, data) {

                var objSubQuery = {
                    Name: 'query', Value: '', dictAttributes: {}, Elements: [], Children: []
                };


                if (data.contains('.')) {
                    var lstQuery = newScope.InitialLoad.Elements.filter(function (itm) { return itm.dictAttributes.ID && itm.dictAttributes.ID.contains(data.split('.')[0]); });
                    if (lstQuery && lstQuery.length > 0) {
                        objSubQuery.dictAttributes.ID = GetInitialLoadQueryID(data.split('.')[0], newScope.InitialLoad.Elements, 1);
                    }
                    else {
                        objSubQuery.dictAttributes.ID = GetInitialLoadQueryID(data.split('.')[0], newScope.SubQueryCollection.Elements, 1);
                    }
                }
                objSubQuery.dictAttributes.sfwQueryRef = data;
                if (newScope.InitialLoad) {
                    newScope.InitialLoad.Elements.push(objSubQuery);
                    newScope.SubQueryCollection.Elements.push(objSubQuery);

                    newScope.SelectedSubQuery = objSubQuery;
                }
                if (event) {
                    event.stopPropagation();
                }
            });

        };

        newScope.onChangeSubQuery = function (QueryID) {
            if (newScope.SelectedSubQuery) {
                if (QueryID && QueryID.contains('.')) {
                    var strQueryId = newScope.GetQueryId(QueryID.split('.')[0]);
                    newScope.SelectedSubQuery.dictAttributes.ID = strQueryId;
                }
            }
        };

        newScope.GetQueryId = function (QueryId) {
            var iItemNum = 0;
            var strItemKey = QueryId;

            var strItemName = strItemKey;
            if (newScope.InitialLoad) {
                var newTemp = newScope.InitialLoad.Elements.filter(function (x) {
                    return x.dictAttributes.ID == strItemName;
                });

                while (newTemp && newTemp.length > 0) {
                    iItemNum++;
                    strItemName = strItemKey + iItemNum;
                    newTemp = newScope.InitialLoad.Elements.filter(function (x) {
                        return x.dictAttributes.ID == strItemName;
                    });
                }
            }
            return strItemName;
        };

        //#endregion

        //#region Edit Query
        newScope.openEditQueryDialog = function () {
            var newQueryScope = newScope.$new();
            if (newScope.SelectedSubQuery) {
                newQueryScope.strSelectedQuery = newScope.SelectedSubQuery.dictAttributes.sfwQueryRef;
                newQueryScope.subQueryType = "SubSelectQuery";
                newQueryScope.QueryDialog = $rootScope.showDialog(newQueryScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });

                newQueryScope.$on('onQueryClick', function (event, data) {
                    if (data.contains('.')) {
                        newScope.SelectedSubQuery.dictAttributes.ID = data.split('.')[0];
                    }
                    newScope.SelectedSubQuery.dictAttributes.sfwQueryRef = data;
                });
                if (event) {
                    event.stopPropagation();
                }
            }
        };
        //#endregion

        //#region delete Subquery Item
        newScope.deleteSelectedSubQuery = function () {
            if (newScope.SelectedSubQuery) {
                var Fieldindex = -1;
                if (newScope.SelectedSubQuery) {
                    Fieldindex = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                }
                newScope.SubQueryCollection.Elements.splice(Fieldindex, 1);


                var index = -1;
                if (newScope.SelectedSubQuery) {
                    index = newScope.InitialLoad.Elements.indexOf(newScope.SelectedSubQuery);
                }
                if (index > -1) {
                    newScope.InitialLoad.Elements.splice(index, 1);
                }
                newScope.SelectedSubQuery = undefined;

                if (Fieldindex < newScope.SubQueryCollection.Elements.length) {

                    newScope.SelectedSubQuery = newScope.SubQueryCollection.Elements[Fieldindex];
                }
                else if (newScope.SubQueryCollection.Elements.length > 0) {
                    newScope.SelectedSubQuery = newScope.SubQueryCollection.Elements[Fieldindex - 1];
                }
            }
        };

        // disable if there is no element for SFW row
        newScope.canDelete = function () {
            if (newScope.SelectedSubQuery) {
                return true;
            }
            else {
                return false;
            }
        };
        //#endregion

        //#region Move up for Subquery
        newScope.moveUpClick = function () {
            if (newScope.SelectedSubQuery) {
                var index = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                var item = newScope.SubQueryCollection.Elements[index - 1];
                newScope.SubQueryCollection.Elements[index - 1] = newScope.SelectedSubQuery;
                newScope.SubQueryCollection.Elements[index] = item;
                $scope.scrollBySelectedField(".manage-subquery-scroll", ".selected", { offsetTop: 480, offsetLeft: 0 });
            }
        };

        // disable the move up button if there is no element to move up
        newScope.canmoveUp = function () {
            newScope.Flag = true;
            if (newScope.SelectedSubQuery != undefined) {
                for (var i = 0; i < newScope.SubQueryCollection.Elements.length; i++) {
                    if (newScope.SubQueryCollection.Elements[i] == newScope.SelectedSubQuery) {
                        if (i > 0) {
                            newScope.Flag = false;
                        }
                    }
                }
            }

            return newScope.Flag;
        };

        //#endregion

        //#region Move down for Sub query    

        newScope.moveDownClick = function () {
            if (newScope.SelectedSubQuery) {
                var index = newScope.SubQueryCollection.Elements.indexOf(newScope.SelectedSubQuery);
                var item = newScope.SubQueryCollection.Elements[index + 1];
                newScope.SubQueryCollection.Elements[index + 1] = newScope.SelectedSubQuery;
                newScope.SubQueryCollection.Elements[index] = item;
                $scope.scrollBySelectedField(".manage-subquery-scroll", ".selected", { offsetTop: 480, offsetLeft: 0 });
            }
        };


        // disable move down when there is no element to move down
        newScope.canMoveDown = function () {
            newScope.Flag = true;
            if (newScope.SelectedSubQuery != undefined) {
                for (var i = 0; i < newScope.SubQueryCollection.Elements.length; i++) {
                    if (newScope.SubQueryCollection.Elements[i] == newScope.SelectedSubQuery) {
                        if (i < newScope.SubQueryCollection.Elements.length - 1) {
                            newScope.Flag = false;
                        }
                    }
                }

            }

            return newScope.Flag;
        };
        //#endregion

        //#region Default Button functionality 
        newScope.getButtonList = function (event) {
            if (newScope.lstButtons && newScope.lstButtons.length > 0) {
                if (!newScope.inputElement) {
                    newScope.inputElement = $(event.target);
                }

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    setSingleLevelAutoComplete($(newScope.inputElement), newScope.lstButtons, newScope, "ID");
                    if ($(newScope.inputElement).data('ui-autocomplete')) $(newScope.inputElement).autocomplete("search", $(newScope.inputElement).val());
                    event.preventDefault();
                }
                setSingleLevelAutoComplete($(newScope.inputElement), newScope.lstButtons, newScope, "ID");
            }
        };

        newScope.showButtonList = function (event) {
            if (!newScope.inputElement) {
                newScope.inputElement = $(event.target).prevAll("input[type='text']");
            }
            newScope.inputElement.focus();
            if (newScope.inputElement) {
                setSingleLevelAutoComplete(newScope.inputElement, newScope.lstButtons, newScope, "ID");
                if ($(newScope.inputElement).data('ui-autocomplete')) $(newScope.inputElement).autocomplete("search", $(newScope.inputElement).val());
            }
            if (event) {
                event.stopPropagation();
            }
        };

        newScope.validateButtonName = function () {
            var list = [];
            if (newScope.lstButtons) {
                angular.forEach(newScope.lstButtons, function (btnObj) {
                    if (btnObj.dictAttributes && btnObj.dictAttributes.ID) {
                        list.push(btnObj.dictAttributes.ID);
                    }
                });
            }
            $ValidationService.checkValidListValue(list, newScope.FormModel, $(newScope.inputElement).val(), "sfwDefaultButtonID", "sfwDefaultButtonID", CONST.VALIDATION.BUTTON_NOT_EXISTS, $scope.validationErrorList);
        };
        //#endregion

        //#region Session Mathods
        //#region adding session fields and object fields

        newScope.CheckAndAddSessionFields = function () {
            if (newScope.InitialLoad) {
                if (!newScope.InitialLoad.Elements.some(function (itm) { return itm.Name == "session"; })) {
                    newScope.InitialLoad.Elements.push(newScope.SessionFields);
                }
            }
        };

        newScope.addSessionFields = function () {
            function iAddSessionFields(item) {
                var DisplayedEntity = getDisplayedEntity(newScope.SessionFields.LstDisplayedEntities);
                var itempath = item.ID;
                if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                    itempath = DisplayedEntity.strDisplayName + "." + item.ID;
                }
                if (item.IsSelected.toLowerCase() == "true") {
                    if (!newScope.SessionFields.Elements.some(function (x) { return x.dictAttributes.ID == item.ID; })) {
                        var strField = itempath;// GetItemPathForEntityObject(item);
                        var objField = {
                            Name: 'field', Value: '', dictAttributes: {
                                ID: item.ID, sfwEntityField: strField
                            }, Elements: []
                        };
                        newScope.SessionFields.Elements.push(objField);
                    }
                    item.IsRecordSelected = false;
                }
            }
            newScope.CheckAndAddSessionFields();
            var lst = [];
            lst = GetSelectedFieldList(newScope.SessionFields.lstselectedobjecttreefields, lst);//GetSelectedFieldList($scope.SessionFields.lstEntity[0].Attributes, lst);
            if (lst && lst.length > 0) {

                angular.forEach(lst, iAddSessionFields);
            }
            if (newScope.SessionFields.lstselectedobjecttreefields && newScope.SessionFields.lstselectedobjecttreefields.length > 0) {
                ClearSelectedFieldList(newScope.SessionFields.lstselectedobjecttreefields);
            }
        };

        //#endregion

        //#region click on selected sessionfield row
        newScope.selectedSessionFieldClick = function (obj) {
            if (obj) {
                newScope.selectedCurrentSessionRow = obj;
            }
        };

        //#endregion

        //#region Delete for  Session Fields

        // delete selected column details
        newScope.deleteSelectedRow = function () {
            var Fieldindex = -1;
            if (newScope.selectedCurrentSessionRow) {
                Fieldindex = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
            }
            newScope.SessionFields.Elements.splice(Fieldindex, 1);
            $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.SessionFields);
            newScope.selectedCurrentSessionRow = undefined;

            if (Fieldindex < newScope.SessionFields.Elements.length) {

                newScope.selectedCurrentSessionRow = newScope.SessionFields.Elements[Fieldindex];
            }
            else if (newScope.SessionFields.Elements.length > 0) {
                newScope.selectedCurrentSessionRow = newScope.SessionFields.Elements[Fieldindex - 1];
            }

        };

        // disable if there is no element for SFW row
        newScope.canDeleteRow = function () {
            if (newScope.selectedCurrentSessionRow) {
                return true;
            }
            else {
                return false;
            }
        };
        //#endregion


        //#region  Move up for session Fields
        // Move up functionality for Row from Record Layout
        newScope.moveSelectedRowUp = function () {
            if (newScope.selectedCurrentSessionRow) {
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var item = newScope.SessionFields.Elements[index - 1];
                newScope.SessionFields.Elements[index - 1] = newScope.selectedCurrentSessionRow;
                newScope.SessionFields.Elements[index] = item;
                $scope.scrollBySelectedField(".details-session-panel-body", ".selected", { offsetTop: 300, offsetLeft: 0 });
            }
        };

        // disable the move up button if there is no element to move up
        newScope.canmoveSelectedRowUp = function () {
            var Flag = true;
            if (newScope.selectedCurrentSessionRow != undefined) {
                for (var i = 0; i < newScope.SessionFields.Elements.length; i++) {
                    if (newScope.SessionFields.Elements[i] == newScope.selectedCurrentSessionRow) {
                        if (i > 0) {
                            Flag = false;
                        }
                    }
                }

            }

            return Flag;
        };
        //#endregion


        //#region Move down for session fields
        // Move Down function for Row from Record Layout

        newScope.moveSelectedRowDown = function () {
            if (newScope.selectedCurrentSessionRow) {
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var index = newScope.SessionFields.Elements.indexOf(newScope.selectedCurrentSessionRow);
                var item = newScope.SessionFields.Elements[index + 1];
                newScope.SessionFields.Elements[index + 1] = newScope.selectedCurrentSessionRow;
                newScope.SessionFields.Elements[index] = item;
                $scope.scrollBySelectedField(".details-session-panel-body", ".selected", { offsetTop: 300, offsetLeft: 0 });
            }
        };


        // disable move down when there is no element to move down
        newScope.canmoveSelectedRowDown = function () {
            var Flag = true;
            if (newScope.selectedCurrentSessionRow != undefined) {
                for (var i = 0; i < newScope.SessionFields.Elements.length; i++) {
                    if (newScope.SessionFields.Elements[i] == newScope.selectedCurrentSessionRow) {
                        if (i < newScope.SessionFields.Elements.length - 1) {
                            Flag = false;
                        }
                    }
                }

            }

            return Flag;
        };

        //#endregion

        //#endregion

        //#region Call Init Method
        newScope.InitialLoadSectionForDetail();
        //newScope.Init();

        //#endregion

        //#region Select Tab
        newScope.OnSelectDetailTab = function (tabName) {
            newScope.selectedDetailTab = tabName;
            if (tabName == 'Entity') {
                if (!newScope.IsEntityTabSelected) {
                    newScope.IsEntityTabSelected = true;
                }
            }
            if (tabName == 'Group') {
                if (!newScope.IsGroupTabSelected) {
                    newScope.IsGroupTabSelected = true;
                }
            }
            if (tabName == 'Session') {
                if (!newScope.IsSessionTabSelected) {
                    newScope.IsSessionTabSelected = true;
                }
            }
            if (tabName == 'ExtraFields') {
                if (!newScope.IsExtraFieldTabSelected) {
                    newScope.IsExtraFieldTabSelected = true;
                }
            }
        };
        //#endregion

        //#region Ok click
        newScope.OkClick = function () {
            objNewDialog.close();


            if (entityname != newScope.FormModel.dictAttributes.sfwEntity) {
                $scope.entityTreeName = newScope.FormModel.dictAttributes.sfwEntity;
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                    if ($scope.lstLoadedEntityTrees[i].EntityName == entityname) {
                        $scope.lstLoadedEntityTrees[i].EntityName = newScope.FormModel.dictAttributes.sfwEntity;
                        break;
                    }
                }
            }

            $rootScope.UndRedoBulkOp("Start");
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            angular.forEach(newScope.FormModel.dictAttributes, function (val, key) {
                $rootScope.EditPropertyValue($scope.FormModel.dictAttributes[key], $scope.FormModel.dictAttributes, key, val);
                // $scope.FormModel.dictAttributes[key] = val;
            });
            // for updating entitytree - when selected control is inside 
            $scope.selectControl($scope.FormModel.SelectedControl);
            $scope.FormModel.errors = {};
            if (newScope.FormModel.errors) {
                $scope.FormModel.errors = newScope.FormModel.errors;
                $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.FormModel);
                if ($ValidationService.isEmptyObj(newScope.FormModel.errors)) {
                    $scope.validationErrorList.push($scope.FormModel);
                }
            }

            if (newScope.MainQuery && $scope.MainQuery && newScope.MainQuery.dictAttributes.ID != $scope.MainQuery.dictAttributes.ID) {
                var objCriteriaPanel = GetCriteriaPanel($scope.FormModel);
                $scope.setNewQueryIdForLookupControl(objCriteriaPanel, $scope.MainQuery.dictAttributes.ID, newScope.MainQuery.dictAttributes.ID, true);
            }

            newScope.removeObjectFromErrorList();

            if ($scope.FormModel.dictAttributes.sfwType == "Lookup") {
                if (newScope.MainQuery && !newScope.MainQuery.dictAttributes.sfwQueryRef) {
                    newScope.MainQuery = undefined;
                    newScope.InitialLoad = undefined;
                    $rootScope.EditPropertyValue($scope.MainQuery, $scope, "MainQuery", undefined);
                    $rootScope.EditPropertyValue($scope.SelectedQuery, $scope, "SelectedQuery", undefined);
                }

                if (newScope.MainQuery && newScope.SubQueryCollection.Elements && $scope.SubQueryCollection && $scope.MainQuery) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormModel);
                    angular.forEach($scope.SubQueryCollection, function (itemOld) {
                        var IsSubQueryIDChanged = false;
                        var IsSubQueryDeleted = true;
                        var newQueryID = "";
                        if (newScope.SubQueryCollection && newScope.SubQueryCollection.Elements.length > 0) {
                            angular.forEach(newScope.SubQueryCollection.Elements, function (item) {
                                if (itemOld.dictAttributes.ID != item.dictAttributes.ID && itemOld.dictAttributes.sfwQueryRef == item.dictAttributes.sfwQueryRef) {
                                    IsSubQueryIDChanged = true;
                                    newQueryID = item.dictAttributes.ID;
                                    IsSubQueryDeleted = false;
                                }
                                if (itemOld.dictAttributes.ID == item.dictAttributes.ID) {
                                    IsSubQueryDeleted = false;
                                }
                            });
                        }
                        else {
                            IsSubQueryIDChanged = true;
                        }
                        if (IsSubQueryIDChanged) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, itemOld.dictAttributes.ID, newQueryID);
                        }
                        if (IsSubQueryDeleted) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, itemOld.dictAttributes.ID, "");
                        }
                    });

                }
                else if ($scope.SubQueryCollection) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormModel);
                    angular.forEach($scope.SubQueryCollection, function (item) {
                        if (item.dictAttributes && item.dictAttributes.ID) {
                            $scope.setNewQueryIdForLookupControl(objCriteriaPanel, item.dictAttributes.ID, "");
                        }
                    });
                }
                else if (!newScope.MainQuery && $scope.MainQuery && $scope.MainQuery.dictAttributes && $scope.MainQuery.dictAttributes.ID) {
                    var objCriteriaPanel = GetCriteriaPanel($scope.FormModel);
                    $scope.setNewQueryIdForLookupControl(objCriteriaPanel, $scope.MainQuery.dictAttributes.ID, newScope.MainQuery.dictAttributes.ID, true);
                }
                if (newScope.MainQuery && newScope.InitialLoad) {
                    for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                        if (newScope.InitialLoad.Elements[i].dictAttributes.sfwQueryRef == newScope.MainQuery.dictAttributes.sfwQueryRef) {
                            newScope.InitialLoad.Elements[i] = newScope.MainQuery;
                            break;
                        }
                    }
                    if ($scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                        for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                            if ($scope.FormModel.Elements[i].Name == "initialload") {
                                $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                                $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, i);
                                $scope.FormModel.Elements[i] = newScope.InitialLoad;
                                break;
                            }
                        }
                    }
                    else if (newScope.InitialLoad && !$scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                        $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, 0);
                    }
                }
                else {
                    for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                        if ($scope.FormModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                            break;
                        }
                    }
                }
            }
            else if ($scope.FormModel && ($scope.FormModel.dictAttributes.sfwType == "Maintenance" || $scope.FormModel.dictAttributes.sfwType == "Wizard")) {

                if (newScope.SelectedNewMethod && (newScope.SelectedNewMethod.dictAttributes.sfwMethodName == undefined || newScope.SelectedNewMethod.dictAttributes.sfwMethodName == "")) {
                    var NewMethodIndex = newScope.InitialLoad.Elements.indexOf(newScope.SelectedNewMethod);
                    if (NewMethodIndex > -1) {
                        newScope.InitialLoad.Elements.splice(NewMethodIndex, 1);
                        newScope.SelectedNewMethod = undefined;
                    }
                }
                if (newScope.SelectedUpdateMethod && (newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName == undefined || newScope.SelectedUpdateMethod.dictAttributes.sfwMethodName == "")) {
                    var UpdateMethodIndex = newScope.InitialLoad.Elements.indexOf(newScope.SelectedUpdateMethod);
                    if (UpdateMethodIndex > -1) {
                        newScope.InitialLoad.Elements.splice(UpdateMethodIndex, 1);
                        newScope.SelectedUpdateMethod = undefined;
                    }
                }


                if (!newScope.InitialLoad.Elements.some(function (itm) { return itm.Name == "session"; })) {
                    if (newScope.SessionFields && newScope.SessionFields.Elements.length > 0) {
                        newScope.InitialLoad.Elements.push(newScope.SessionFields);
                    }
                }
                else {
                    for (var i = 0; i < newScope.InitialLoad.Elements.length; i++) {
                        if (newScope.InitialLoad.Elements[i].Name == "session") {
                            if (newScope.SessionFields && newScope.SessionFields.Elements.length > 0) {
                                newScope.InitialLoad.Elements[i] = newScope.SessionFields;
                            }
                            else {
                                newScope.InitialLoad.Elements.splice(i, 1);
                            }
                            break;
                        }
                    }
                }

                if (!newScope.SelectedNewMethod && !newScope.SelectedUpdateMethod && (!newScope.SessionFields || (newScope.SessionFields && newScope.SessionFields.Elements.length == 0))) {
                    newScope.InitialLoad = undefined;
                }

                if (newScope.InitialLoad && !$scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                    $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, 0);
                }
                else if (newScope.InitialLoad && $scope.FormModel.Elements.some(function (x) { return x.Name == "initialload"; })) {
                    for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                        if ($scope.FormModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                            $rootScope.InsertItem(newScope.InitialLoad, $scope.FormModel.Elements, i);
                            break;
                        }
                    }
                }
                else if (!newScope.InitialLoad) {
                    for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
                        if ($scope.FormModel.Elements[i].Name == "initialload") {
                            $rootScope.DeleteItem($scope.FormModel.Elements[i], $scope.FormModel.Elements);
                            //$scope.FormModel.Elements.splice(i, 1);
                            break;
                        }
                    }
                }

            }

            $rootScope.EditPropertyValue($scope.InitialLoad, $scope, "InitialLoad", newScope.InitialLoad);
            //$scope.InitialLoad = newScope.InitialLoad;
            $scope.InitialLoadSection();

            $rootScope.UndRedoBulkOp("End");


            //if (!$scope.FormModel.Elements.some(function (x) { return x.Name == "initialload" })) {
            //    if ($scope.InitialLoad) {
            //        if ($scope.FormModel && ($scope.FormModel.dictAttributes.sfwType == "Maintenance" || $scope.FormModel.dictAttributes.sfwType == "Wizard")) {
            //            if (($scope.SelectedNewMethod && $scope.SelectedNewMethod.dictAttributes.sfwMethodName != undefined && $scope.SelectedNewMethod.dictAttributes.sfwMethodName != "")
            //                || ($scope.SelectedUpdateMethod && $scope.SelectedUpdateMethod.dictAttributes.sfwMethodName != undefined && $scope.SelectedUpdateMethod.dictAttributes.sfwMethodName != "")
            //                || ($scope.SessionFields && $scope.SessionFields.Elements && $scope.SessionFields.Elements.length > 0)) {
            //                $scope.FormModel.Elements.splice(0, 0, $scope.InitialLoad);
            //            }
            //        }
            //        else if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType == "Lookup") {
            //            if (($scope.MainQuery && $scope.MainQuery.dictAttributes.sfwQueryRef != undefined && $scope.MainQuery.dictAttributes.sfwQueryRef != "") || ($scope.SubQueryCollection && $scope.SubQueryCollection.length > 0)) {
            //                $scope.FormModel.Elements.splice(0, 0, $scope.InitialLoad);
            //            }
            //            else {
            //                $scope.MainQuery = undefined;
            //            }
            //        }
            //    }
            //}
            //else {
            //    if ($scope.InitialLoad) {
            //        var index = $scope.FormModel.Elements.indexOf($scope.InitialLoad);
            //        if (index > -1) {
            //            if ($scope.FormModel && ($scope.FormModel.dictAttributes.sfwType == "Maintenance" || $scope.FormModel.dictAttributes.sfwType == "Wizard")) {
            //                if ($scope.SelectedNewMethod && ($scope.SelectedNewMethod.dictAttributes.sfwMethodName == undefined || $scope.SelectedNewMethod.dictAttributes.sfwMethodName == "")) {
            //                    var NewMethodIndex = $scope.InitialLoad.Elements.indexOf($scope.SelectedNewMethod);
            //                    $scope.InitialLoad.Elements.splice(NewMethodIndex, 1);
            //                    $scope.SelectedNewMethod = undefined;
            //                }
            //                if ($scope.SelectedUpdateMethod && ($scope.SelectedUpdateMethod.dictAttributes.sfwMethodName == undefined || $scope.SelectedUpdateMethod.dictAttributes.sfwMethodName == "")) {
            //                    var UpdateMethodIndex = $scope.InitialLoad.Elements.indexOf($scope.SelectedUpdateMethod);
            //                    $scope.InitialLoad.Elements.splice(UpdateMethodIndex, 1);
            //                    $scope.SelectedUpdateMethod = undefined;
            //                }
            //                if ((!$scope.SelectedNewMethod || ($scope.SelectedNewMethod && ($scope.SelectedNewMethod.dictAttributes.sfwMethodName == undefined || $scope.SelectedNewMethod.dictAttributes.sfwMethodName == "")))
            //                    && (!$scope.SelectedUpdateMethod || ($scope.SelectedUpdateMethod && ($scope.SelectedUpdateMethod.dictAttributes.sfwMethodName == undefined || $scope.SelectedUpdateMethod.dictAttributes.sfwMethodName == "")))
            //                    && (!$scope.SessionFields || ($scope.SessionFields && $scope.SessionFields.Elements.length == 0))) {
            //                    $scope.FormModel.Elements.splice(index, 1);
            //                }
            //            }
            //            else if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType == "Lookup") {
            //                if ($scope.MainQuery && ($scope.MainQuery.dictAttributes.sfwQueryRef == undefined || $scope.MainQuery.dictAttributes.sfwQueryRef == "")) {
            //                    var MainQueryIndex = $scope.InitialLoad.Elements.indexOf($scope.MainQuery);
            //                    $scope.InitialLoad.Elements.splice(MainQueryIndex, 1);
            //                    $scope.MainQuery = undefined;
            //                }
            //                if ((!$scope.MainQuery || ($scope.MainQuery && ($scope.MainQuery.dictAttributes.sfwQueryRef == undefined || $scope.MainQuery.dictAttributes.sfwQueryRef == ""))) && ($scope.SubQueryCollection && $scope.SubQueryCollection.length == 0)) {
            //                    $scope.FormModel.Elements.splice(index, 1);
            //                }
            //            }
            //        }
            //    }
            //}

            if ($scope.FormModel.dictAttributes.sfwType == "Lookup") {
                var isVisible = false;
                if ($scope.FormModel.IsLookupCriteriaEnabled) {
                    isVisible = true;
                }
                $scope.PopulateQueryId(isVisible);
            }
            //  createRuledata();
        };
        $scope.setNewQueryIdForLookupControl = function (aModel, astrOldQueryID, astrNewQueryID, aIsMainQuery) {
            if (aModel) {
                angular.forEach(aModel.Elements, function (objModel) {
                    if (objModel.dictAttributes && objModel.dictAttributes.sfwQueryID && objModel.dictAttributes.sfwQueryID == astrOldQueryID) {
                        if (astrNewQueryID && astrNewQueryID != "") {
                            $rootScope.EditPropertyValue(objModel.dictAttributes.sfwQueryID, objModel.dictAttributes, "sfwQueryID", "");
                            objModel.dictAttributes.sfwQueryID = astrNewQueryID;
                        }
                        else {
                            $rootScope.EditPropertyValue(objModel.dictAttributes.sfwQueryID, objModel.dictAttributes, "sfwQueryID", "");
                            objModel.dictAttributes.sfwQueryID = "";
                            if (!aIsMainQuery) {
                                $rootScope.EditPropertyValue(objModel.dictAttributes.sfwDataField, objModel.dictAttributes, "sfwDataField", "");
                                objModel.dictAttributes.sfwDataField = "";
                            }
                        }
                    }
                    if (objModel.Elements) {
                        $scope.setNewQueryIdForLookupControl(objModel, astrOldQueryID, astrNewQueryID, aIsMainQuery);
                    }

                });
            }
        };
        newScope.validateFormDetails = function () {
            if (!newScope.IsPrototypeDetails) {
                newScope.FormDetailsErrorMessage = "";
                if (newScope.objDirFunctions.getExtraFieldData) {
                    newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
                }

                var flag = validateExtraFields(newScope);
                return flag;
            }

        };
        newScope.NavigateToEntityQuery = function (aQueryID) {
            if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                //objNewDialog.close();
                newScope.OkClick();
                var query = aQueryID.split(".");
                $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
            }
        };
        newScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                //objNewDialog.close();
                newScope.OkClick();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };

        newScope.openQueryEditClick = function (obj, astrTitle) {
            var newQueryScope = newScope.$new();
            newQueryScope.Title = astrTitle;
            newQueryScope.newID = "";
            if (obj && obj.dictAttributes.ID) {
                newQueryScope.newID = obj.dictAttributes.ID;
            }
            newQueryScope.QueryEditDialog = $rootScope.showDialog(newQueryScope, newQueryScope.Title, "Form/views/SetQueryName.html", {
                width: 500, height: 150
            });
            newQueryScope.setQueryName = function () {
                obj.dictAttributes.ID = newQueryScope.newID;
                newQueryScope.closeQueryDialog();
            };

            newQueryScope.closeQueryDialog = function () {
                newQueryScope.QueryEditDialog.close();
            };
        };

        newScope.IsSameAsNewChecked = function (value) {
            function iterationInitialLoadElements(objcustommethod) {
                if (objcustommethod.Name == "callmethods") {
                    if (objcustommethod.dictAttributes.sfwMode == 'Update') {
                        blnfound = true;
                    }
                    if (!objcustommethod.dictAttributes.sfwMode) {
                        objcustommethod.dictAttributes.sfwMode = 'New';
                    }
                }

            }
            function Getindexforupdate(objcustommethod) {
                if (objcustommethod.Name == "callmethods") {

                    if (objcustommethod.dictAttributes.sfwMode == 'New') {
                        objcustommethod.dictAttributes.sfwMode = "";
                    }
                    if (objcustommethod.dictAttributes.sfwMode == 'Update') {
                        indexforupdate = newScope.InitialLoad.Elements.indexOf(objcustommethod);
                    }
                }

            }
            newScope.SelectedUpdateMethod = {
                Name: "callmethods", Value: '', Elements: [], dictAttributes: { sfwMethodName: "", sfwMode: "Update" }
            };
            if (newScope.FormModel.IsSameAsNew) {
                newScope.paramtersForUpdateMethod = [];
                var indexforupdate = -1;

                angular.forEach(newScope.InitialLoad.Elements, Getindexforupdate);
                if (indexforupdate > -1) {
                    newScope.InitialLoad.Elements.splice(indexforupdate, 1);
                    //$rootScope.DeleteItem($scope.InitialLoad.Elements[indexforupdate], $scope.InitialLoad.Elements);
                }
            }
            else {
                var blnfound = false;


                angular.forEach(newScope.InitialLoad.Elements, iterationInitialLoadElements);

                if (!blnfound) {
                    var acallmethods = {
                        Name: "callmethods", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: []
                    };
                    acallmethods.ParentVM = $scope.InitialLoad;
                    acallmethods.dictAttributes.sfwMode = 'Update';
                    //$rootScope.PushItem(acallmethods, $scope.InitialLoad.Elements);
                    newScope.InitialLoad.Elements.push(acallmethods);
                    newScope.SelectedUpdateMethod = acallmethods;
                }
            }
        };
        newScope.validateSeesionID = function (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID, undefined, true);
            angular.forEach(newScope.SessionFields.Elements, function (field) {
                $ValidationService.checkDuplicateId(field, newScope.SessionFields, $scope.validationErrorList, true, ["session", "field"]);
            });
        };

        newScope.valiidateSessionField = function () {
            angular.forEach(newScope.SessionFields.Elements, function (obj) {
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, newScope.FormModel.dictAttributes.sfwEntity, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, $scope.validationErrorList, false, '');
            });
        };

        newScope.validateQueryID = function () {
            if (newScope.SubQueryCollection && $scope.FormModel.dictAttributes.sfwType == "Lookup") {
                angular.forEach(newScope.SubQueryCollection.Elements, function (query) {
                    $ValidationService.checkDuplicateId(query, newScope.SubQueryCollection, $scope.validationErrorList, true, ["query"]);
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), query, query.dictAttributes.sfwQueryRef, "SubSelectQuery", "sfwQueryRef", "sfwQueryRef", CONST.VALIDATION.INVALID_QUERY, $scope.validationErrorList);
                });
            }
        };
        newScope.removeObjectFromErrorList = function () {
            $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.FormModel);
            if (newScope.SessionFields && newScope.SessionFields.Elements) {
                angular.forEach(newScope.SessionFields.Elements, function (field) {
                    $ValidationService.removeObjInToArray($scope.validationErrorList, field);
                });
            }
            if (newScope.SubQueryCollection && newScope.SubQueryCollection.Elements.length > 0) {
                angular.forEach(newScope.SubQueryCollection.Elements, function (query) {
                    $ValidationService.removeObjInToArray($scope.validationErrorList, query);
                });
            }
        };
        newScope.closeDetailDialog = function () {
            newScope.removeObjectFromErrorList();
            objNewDialog.close();
        };

    };
    //#endregion


    //#region Receive User Control Table Model
    $scope.receiveUcMainTable = function (data) {

        $scope.tableTemp;
        for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
            if ($scope.FormModel.Elements[i].Name == "sfwTable") {
                $scope.tableTemp = $scope.FormModel.Elements[i];
                break;
            }
        }
        for (var i = 0; i < data.length; i++) {
            var ucControl = FindControlByID($scope.tableTemp, data[i].ID);
            //console.log("table: ", $scope.tableTemp);
            if (ucControl) {
                $scope.$apply(function () {
                    ucControl.UcChild = data ? [data[i].udcModel] : []; // converting object to array 
                    setParentControlName(ucControl.UcChild[0]);
                });
            }
        }
    };

    //removed by neha as we dont need that functionality now
    //$scope.OnOpenValidationRuleDetailsClick = function () {
    //    if ($scope.FormModel && $scope.FormModel.Data) {
    //        var newScope = $scope.$new(true);
    //        newScope.lstMessages = $scope.lstMessages;
    //        newScope.FormModel = $scope.FormModel;
    //        newScope.EntityValidationDialog = $rootScope.showDialog(newScope, "Validation Rules Details", "Form/views/EntityValidationRules.html", { width: 800, height: 600 });
    //    }
    //}
    //#endregion

    //#region Entity Intellisense
    $scope.onActionKeyDown = function (eargs) {
        var input = eargs.target;
        var data = [];
        var entityName;
        if ($scope.ObjgridBoundedQuery && !$scope.ObjgridBoundedQuery.IsQuery) {
            var entitytree = $scope.lstLoadedEntityTrees.filter(function (x) { return x.IsVisible; });
            if (entitytree && entitytree.length > 0) {
                entityName = entitytree[0].EntityName;
            }
            else if ($scope.FormModel.SelectedControl && !$scope.FormModel.IsLookupCriteriaEnabled && FindParent($scope.FormModel.SelectedControl, "sfwDialogPanel")) {
                entityName = "";
            }
            else {
                entityName = $scope.FormModel.dictAttributes.sfwEntity;
            }

            if ($scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.dictAttributes.sfwRelatedGrid) {
                var model = FindControlByID($scope.FormModel, $scope.FormModel.SelectedControl.dictAttributes.sfwRelatedGrid);
                if (model && model.dictAttributes.sfwEntityField) {
                    var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, model.dictAttributes.sfwEntityField);
                    if (object) {
                        entityName = object.Entity;
                    }
                } else {
                    entityName = "";
                }
            }
            if ($scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.IsChildOfGrid) {
                var objGrid = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
                if (objGrid && objGrid.dictAttributes.sfwParentGrid && objGrid.dictAttributes.sfwEntityField) {
                    entityName = $scope.FindEntityName(objGrid, $scope.FormModel.dictAttributes.sfwEntity, true);
                }
            }

            var isshowExpression = false;
            if ($scope.FormModel.SelectedControl.Name == "sfwLabel") {
                isshowExpression = true;
            }
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList;
            var parententityName = entityName;
            while (parententityName) {
                data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, false, false, false, false, false, isshowExpression));
                var entity = entities.filter(function (x) {
                    return x.ID == parententityName;
                });
                if (entity.length > 0) {
                    parententityName = entity[0].ParentId;
                } else {
                    parententityName = "";
                }
            }
        } else {
            if ($scope.ObjgridBoundedQuery.lstselectedobjecttreefields && $scope.ObjgridBoundedQuery.lstselectedobjecttreefields.length > 0) {
                data = $scope.ObjgridBoundedQuery.lstselectedobjecttreefields;
            }
        }
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
                            while (parententityName) {
                                //expression should not come for second level
                                data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, false, false, false, false, false, isshowExpression, false));
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
                        //else if (item[0].DataType != "undefined" && item[0].DataType == "AliasObject" && index < arrText.length - 1) {
                        //    data=item[0].lstobjCollection;
                        //}
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

    //#region Validate New
    $scope.OpenValidateNewDialog = function () {

        var newScope = $scope.$new();

        newScope.lstButton = [];
        newScope.lstButton = PopulateButtonID($scope.FormModel, newScope.lstButton);
        if ($scope.ValidateNew) {
            newScope.objValidateNew = {};
            angular.copy($scope.ValidateNew, newScope.objValidateNew);
            if (newScope.objValidateNew.Elements.length > 0) {
                for (var j = 0; j < newScope.objValidateNew.Elements.length; j++) {
                    newScope.objValidateNew.Elements[j].IsFieldVisibility = false;
                }
            }
        }
        else {
            newScope.objValidateNew = {
                dictAttributes: {}, Children: [], Elements: [], Name: "validatenew", Value: ""
            };
        }
        //for (var i = 0; i < $scope.FormModel.Elements.length; i++) {
        //    if ($scope.FormModel.Elements[i].Name == "validatenew") {
        //        newScope.objValidateNew = $scope.FormModel.Elements[i];
        //        if (newScope.objValidateNew.Elements.length > 0) {
        //            for (var j = 0; j < newScope.objValidateNew.Elements.length; j++) {
        //                newScope.objValidateNew.Elements[j].IsFieldVisibility = false;
        //            }
        //        }
        //    }
        //}
        //if (newScope.objValidateNew == undefined) {
        //    newScope.objValidateNew = {
        //        dictAttributes: {}, Children: [], Elements: [], Name: "validatenew", Value: ""
        //    };
        //    $scope.FormModel.Elements.push(newScope.objValidateNew);
        //}
        dialog = $rootScope.showDialog(newScope, "Set New Validation", "Form/views/ValidateNewDialog.html", { width: 1280, height: 450 });
        newScope.onOkClick = function () {

            var lst = $scope.FormModel.Elements.filter(function (x) { return x.Name === "validatenew" });
            if (lst && lst.length > 0) {
                $rootScope.UndRedoBulkOp("Start");
                if ($scope.ValidateNew) {
                    $rootScope.EditPropertyValue($scope.ValidateNew.Elements, $scope.ValidateNew, "Elements", []);
                }

                if (newScope.objValidateNew && newScope.objValidateNew.Elements) {
                    for (var i = 0; i < newScope.objValidateNew.Elements.length > 0; i++) {
                        $rootScope.PushItem(newScope.objValidateNew.Elements[i], $scope.ValidateNew.Elements);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
            else {


                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(newScope.objValidateNew, $scope.FormModel.Elements);
                $rootScope.EditPropertyValue($scope.ValidateNew, $scope, "ValidateNew", newScope.objValidateNew);
                $rootScope.UndRedoBulkOp("End");


            }

            newScope.onCancelClick();
        };
        newScope.onCancelClick = function () {
            dialog.close();
        }
        newScope.GetButtonIntellisense = function (event) {
            if (event.type == 'click') {
                var input = $(event.target).prevAll('input');
                if (input) {
                    $(input).focus();
                    setSingleLevelAutoComplete(input, newScope.lstButton, newScope, "ID");
                    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                }
                event.preventDefault();
            }
            else {
                var input = $(event.target);
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                else {
                    setSingleLevelAutoComplete(input, newScope.lstButton, newScope, "ID");
                }
            }
        };
        newScope.validateButtonID = function (model) {
            var list = [];
            if (newScope.lstButton && newScope.lstButton.length > 0) {
                angular.forEach(newScope.lstButton, function (btn) {
                    if (btn.dictAttributes && btn.dictAttributes.ID) {
                        list.push(btn.dictAttributes.ID);
                    }
                });
            }
            $ValidationService.checkValidListValue(list, model, model.dictAttributes.ButtonID, "ButtonID", "inValid_id", CONST.VALIDATION.NOT_VALID_ID, $scope.validationErrorList);
        };
        newScope.AddNewValidationRule = function () {
            var objNewRule = {
                dictAttributes: {}, Elements: [], Children: [], Name: "button", Value: ""
            };
            newScope.objValidateNew.Elements.push(objNewRule);
            newScope.objSelectedRule = newScope.objValidateNew.Elements[newScope.objValidateNew.Elements.length - 1];
            newScope.ExpandRule(objNewRule);
        };
        newScope.selectedValidateRule = function (obj) {
            newScope.selectedItem = null;
            newScope.objSelectedRule = obj;
        };
        newScope.canDeleteRule = function () {
            if (newScope.objSelectedRule) {
                return true;
            }
            else {
                return false;
            }
        };
        newScope.DeleteValidationRule = function () {
            var index = newScope.objValidateNew.Elements.indexOf(newScope.objSelectedRule);
            if (index > -1) {
                newScope.objValidateNew.Elements.splice(index, 1);
                if (newScope.objValidateNew.Elements.length == 0) {
                    newScope.objSelectedRule = undefined;
                }
                else if (newScope.objValidateNew.Elements.length > index) {
                    newScope.objSelectedRule = newScope.objValidateNew.Elements[index];
                }
                else if (newScope.objValidateNew.Elements.length > 0) {
                    newScope.objSelectedRule = newScope.objValidateNew.Elements[index - 1];
                }
            }
        };
        newScope.ExpandRule = function (objPara) {
            if (!objPara.IsFieldVisibility && newScope.objValidateNew.Elements.length > 0) {
                for (var j = 0; j < newScope.objValidateNew.Elements.length; j++) {
                    newScope.objValidateNew.Elements[j].IsFieldVisibility = false;
                }
            }

            objPara.IsFieldVisibility = !objPara.IsFieldVisibility;
        };
        newScope.selectItem = function (item, obj) {
            newScope.selectedItem = item;
            newScope.objSelectedRule = obj;
        };
        newScope.canMoveItemsDown = function () {

            if (newScope.selectedItem) {
                var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
                if (index < newScope.objSelectedRule.Elements.length - 1) {
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
        newScope.canMoveItemsUp = function () {
            if (newScope.selectedItem) {
                var index = newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem);
                if (index <= newScope.objSelectedRule.Elements.length && index > 0) {
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
        newScope.canDeleteItems = function () {
            if (newScope.selectedItem) {
                if (newScope.objSelectedRule) {
                    if (newScope.objSelectedRule.Elements.indexOf(newScope.selectedItem) > -1) {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        };
        newScope.moveItemsDown = function (obj) {

            var index = obj.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                var item = obj.Elements[index];
                obj.Elements[index] = obj.Elements[index + 1];
                obj.Elements[index + 1] = item;
            }
        };
        newScope.moveItemsUp = function (obj) {

            var index = obj.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                var item = obj.Elements[index];
                obj.Elements[index] = obj.Elements[index - 1];
                obj.Elements[index - 1] = item;
            }
        };
        newScope.deleteItems = function (obj) {
            newScope.objSelectedRule = obj;
            var index = obj.Elements.indexOf(newScope.selectedItem);
            if (index > -1) {
                obj.Elements.splice(index, 1);
                if (obj.Elements.length == 0) {
                    newScope.selectedItem = undefined;
                }
                else if (obj.Elements.length > index) {
                    newScope.selectedItem = obj.Elements[index];
                }
                else if (obj.Elements.length > 0) {
                    newScope.selectedItem = obj.Elements[index - 1];
                }
            }
        };

        newScope.EditItems = function (objSelectedRule, objItem) {
            newScope.selectedItem = objItem;
            newScope.addItems(objSelectedRule, "Update");
        }
        newScope.addItems = function (obj, Flag) {
            newScope.objSelectedRule = obj;
            var newItemScope = $scope.$new();
            newItemScope.lstControlID = [];
            newItemScope.lstControlID = PopulateControlID($scope.FormModel, newItemScope.lstControlID);
            newItemScope.lstMethodParam = [];
            if (newScope.lstButton.length > 0 && newScope.objSelectedRule && newScope.objSelectedRule.dictAttributes.ButtonID) {
                for (var i = 0; i < newScope.lstButton.length; i++) {
                    if (newScope.lstButton[i].dictAttributes.ID == newScope.objSelectedRule.dictAttributes.ButtonID) {
                        if (newScope.lstButton[i].dictAttributes.sfwNavigationParameter != "" && newScope.lstButton[i].dictAttributes.sfwNavigationParameter != undefined) {
                            var templstNavParam = newScope.lstButton[i].dictAttributes.sfwNavigationParameter.split(";");
                            for (var j = 0; j < templstNavParam.length; j++) {
                                if (templstNavParam[i] != "") {
                                    var tmpPara = templstNavParam[j].split("=");
                                    newItemScope.lstMethodParam.push(tmpPara[0]);
                                }

                            }

                        }
                    }
                }
            }
            if (Flag == "Add") {
                newItemScope.objItem = {
                    dictAttributes: { sfwValidationType: "Required", sfwRequired: 'True' }, Name: "item", Value: "", Children: [], Elements: []
                };
            }
            else {
                newItemScope.objItem = newScope.selectedItem;
            }
            newItemScope.lstoperator = ["", "=", "!=", "<", ">", "<=", ">="];
            dialogItem = $rootScope.showDialog(newItemScope, "Add New Items", "Form/views/AddNewItemsInValidationRule.html", { width: 690, height: 420 });
            newItemScope.onOkClick = function (objItemParam) {
                if (Flag == "Add") {
                    $rootScope.PushItem(objItemParam, newScope.objSelectedRule.Elements);
                    //newScope.objSelectedRule.Elements.push();
                }
                dialogItem.close();
            };
            newItemScope.getControlIDIntellisense = function (event) {
                if (event) {
                    if (event.type == 'click') {
                        var input = $(event.target).prevAll('input');
                        if (input) {
                            setSingleLevelAutoComplete(input, newItemScope.lstControlID);
                            if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                        }
                        event.preventDefault();
                    }
                    else {
                        var input = $(event.target);
                        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                            $(input).autocomplete("search", $(input).val());
                            event.preventDefault();
                        }
                        else {
                            setSingleLevelAutoComplete(input, newItemScope.lstControlID);
                        }
                    }
                }
            };
            newItemScope.validateNewItem = function (obj) {
                if (obj) {
                    var list = newItemScope.lstControlID && newItemScope.lstControlID.length > 0 ? newItemScope.lstControlID : [];
                    $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwControlID, "sfwControlID", "sfwControlID", CONST.VALIDATION.NOT_VALID_ID, $scope.validationErrorList);
                }
            };
            newItemScope.QueryChange = function () {
                newItemScope.objItem.dictAttributes.sfwParameters = "";
            };
            if (newItemScope.objItem && !newItemScope.objItem.TempQueryParameters) {
                if (newItemScope.objItem.dictAttributes.sfwQueryID != undefined && newItemScope.objItem.dictAttributes.sfwQueryID != "") {
                    newItemScope.objItem.TempQueryParameters = $getQueryparam.get(newItemScope.objItem.dictAttributes.sfwQueryID);
                }
            }
            newItemScope.openQueryParameterDialog = function () {
                var newParamScope = newItemScope.$new();
                newParamScope.objParameter = [];
                newParamScope.lstParameters = newItemScope.lstMethodParam;
                if (newItemScope.objItem.dictAttributes.sfwParameters) {
                    var tempparameter = newItemScope.objItem.dictAttributes.sfwParameters.split(";");
                    for (var i = 0; i < tempparameter.length; i++) {
                        var paraField = tempparameter[i].split("=");
                        if (paraField[0] != "") {
                            newParamScope.objParameter.push({ ParameterField: paraField[0], ParameterValue: paraField[1] });
                        }
                    }
                }
                if (newItemScope.objItem.TempQueryParameters) {
                    var lstTempQueryParameter = newItemScope.objItem.TempQueryParameters.split(";");
                    for (var i = 0; i < lstTempQueryParameter.length; i++) {
                        var flag = true;
                        for (var j = 0; j < newParamScope.objParameter.length; j++) {
                            if (newParamScope.objParameter[j].ParameterField == lstTempQueryParameter[i].replace("=", "")) {
                                flag = false;
                                break;
                            }
                        }
                        if (flag && lstTempQueryParameter[i] != "") {
                            newParamScope.objParameter.push({ ParameterField: lstTempQueryParameter[i].replace("=", ""), ParameterValue: "" });
                        }
                    }
                }
                dialogParam = $rootScope.showDialog(newParamScope, "Set Query Parameters", "Form/views/SetQueryParameter.html", { width: 550, height: 300 });
                newParamScope.paraOkClick = function (objItemParam) {
                    var strpaameter = "";
                    for (var i = 0; i < newParamScope.objParameter.length; i++) {
                        if (newParamScope.objParameter[i].ParameterValue && newParamScope.objParameter[i].ParameterValue != "") {
                            strpaameter += newParamScope.objParameter[i].ParameterField + "=" + newParamScope.objParameter[i].ParameterValue + ";";
                        }
                    }
                    newItemScope.objItem.dictAttributes.sfwParameters = strpaameter;
                    dialogParam.close();
                };
                newParamScope.paraCancelClick = function () {
                    dialogParam.close();
                };
            };
        };
    };

    //#endregion

    $scope.showWizardProperty = function () {
        //$scope.FormModel.SelectedControl.Name = "sfwWizard";
        SetFormSelectedControl($scope.FormModel, $scope.objWizard);
        $scope.ActiveTabForForm = 'Properties';
        $scope.IsToolsDivCollapsed = false;
    };

    //#region Add Panel and Wizard Step
    $scope.OnAddPanelClick = function (index, isInsert) {
        var newScope = $scope.$new();
        newScope.formmodel = $scope.FormModel;
        newScope.AddPanelType = "AddPanel";
        newScope.OkClick = function () {
            if (newScope.AddPanelType === "AddPanel") {
                $scope.AddPanel(index, isInsert);
                newScope.closeDetailDialog();
            }
            else if (newScope.AddPanelType === "AddPanelWithTab") {
                $scope.AddPanelWithTabContainer(index, isInsert);
                newScope.closeDetailDialog();
            }
            else if (newScope.AddPanelType === "AddPanelWithGrid") {

                $scope.AddPanelWithGrid(index, isInsert);
                newScope.closeDetailDialog();
            }
        }

        newScope.validatePanel = function () {
            var retVal = false;
            newScope.ErrorMessage = "";

            if (!newScope.AddPanelType) {
                newScope.ErrorMessage = "Select Option.";
                retVal = true;
            }
            return retVal;
        };

        newScope.closeDetailDialog = function () {
            newScope.dialogToAddPanel.close();
        }

        newScope.dialogToAddPanel = $rootScope.showDialog(newScope, "Add Panel", "Form/views/AddPanels.html", { width: 500, height: 195 });

    };

    $scope.AddPanel = function (index, isInsert) {
        var strPanelId;
        $rootScope.UndRedoBulkOp("Start");
        var sfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
        };
        sfxRowModel.ParentVM = $scope.SfxMainTable;

        var sfxColumnModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        sfxColumnModel.ParentVM = sfxRowModel;

        strPanelId = CreateControlID($scope.FormModel, "NewPanel", "sfwPanel", false);


        var sfxPanelModel = {
            Name: "sfwPanel", prefix: "swc", Value: '', dictAttributes: { ID: strPanelId, sfwCaption: "New Page" }, Elements: [], Children: []
        };
        sfxPanelModel.ParentVM = sfxColumnModel;
        $rootScope.PushItem(sfxPanelModel, sfxColumnModel.Elements);
        $rootScope.PushItem(sfxColumnModel, sfxRowModel.Elements);

        var strCtrlId = CreateControlID($scope.FormModel, "NewPage", "sfwTable", false);
        var sfxTableModel = {
            Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: { ID: strCtrlId }, Elements: [], Children: []
        };
        sfxTableModel.ParentVM = sfxPanelModel;

        var newSfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxRowModel.ParentVM = sfxTableModel;
        var newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;
        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;
        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;
        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;
        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        $rootScope.PushItem(newSfxRowModel, sfxTableModel.Elements);
        $rootScope.PushItem(sfxTableModel, sfxPanelModel.Elements);

        //$('.form-panel-wrapper .panel-collapse').each(function () {
        //    if ($(this).hasClass('in')) {
        //        $(this).collapse('hide');
        //        return true;
        //    }
        //});

        $scope.MainPanels.some(function (panel) {
            if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                panel.initialvisibilty = false;
                panel.IsVisible = false;
                panel.IsPanelToggle = false;
                return true;
            }
        });

        // when we add panel for the first 
        if (!$scope.SfxMainTable) {
            $scope.SfxMainTable = { Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: [] };
            $rootScope.PushItem($scope.SfxMainTable, $scope.FormModel.Elements);
        }
        if (isInsert && index > -1) {

            $rootScope.InsertItem(sfxRowModel, $scope.SfxMainTable.Elements, index);
            $rootScope.InsertItem(sfxPanelModel, $scope.MainPanels, index);

        }
        else {
            $rootScope.PushItem(sfxRowModel, $scope.SfxMainTable.Elements);
            $rootScope.PushItem(sfxPanelModel, $scope.MainPanels);
        }
        sfxPanelModel.initialvisibilty = true;
        sfxPanelModel.TableVM = sfxTableModel;
        sfxPanelModel.IsMainPanel = true;

        $scope.selectPanelControl(sfxPanelModel);
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.AddPanelWithTabContainer = function (index, isInsert) {
        var strPanelId;
        $rootScope.UndRedoBulkOp("Start");
        var sfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {
            }, Elements: []
        };
        sfxRowModel.ParentVM = $scope.SfxMainTable;

        var sfxColumnModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {
            }, Elements: [], Children: []
        };
        sfxColumnModel.ParentVM = sfxRowModel;

        strPanelId = CreateControlID($scope.FormModel, "NewPanel", "sfwPanel", false);


        var sfxPanelModel = {
            Name: "sfwPanel", prefix: "swc", Value: '', dictAttributes: {
                ID: strPanelId, sfwCaption: "New Page"
            }, Elements: [], Children: []
        };
        sfxPanelModel.ParentVM = sfxColumnModel;
        $rootScope.PushItem(sfxPanelModel, sfxColumnModel.Elements);
        $rootScope.PushItem(sfxColumnModel, sfxRowModel.Elements);

        var strCtrlId = CreateControlID($scope.FormModel, "NewPage", "sfwTable", false);
        var sfxTableModel = {
            Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: { ID: strCtrlId }, Elements: [], Children: []
        };
        sfxTableModel.ParentVM = sfxPanelModel;

        var newSfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxRowModel.ParentVM = sfxTableModel;
        var newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;
        var newTabContainer = CreateTabContainer($scope.FormModel, newSfxCellModel);
        $rootScope.PushItem(newTabContainer, newSfxCellModel.Elements);
        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        $rootScope.PushItem(newSfxRowModel, sfxTableModel.Elements);
        $rootScope.PushItem(sfxTableModel, sfxPanelModel.Elements);

        $scope.MainPanels.some(function (panel) {
            if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                panel.initialvisibilty = false;
                panel.IsVisible = false;
                panel.IsPanelToggle = false;
                return true;
            }
        });

        // when we add panel for the first 
        if (!$scope.SfxMainTable) {
            $scope.SfxMainTable = {
                Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            $rootScope.PushItem($scope.SfxMainTable, $scope.FormModel.Elements);
        }
        if (isInsert && index > -1) {

            $rootScope.InsertItem(sfxRowModel, $scope.SfxMainTable.Elements, index);
            $rootScope.InsertItem(sfxPanelModel, $scope.MainPanels, index);

        }
        else {
            $rootScope.PushItem(sfxRowModel, $scope.SfxMainTable.Elements);
            $rootScope.PushItem(sfxPanelModel, $scope.MainPanels);
        }
        sfxPanelModel.initialvisibilty = true;
        sfxPanelModel.TableVM = sfxTableModel;
        sfxPanelModel.IsMainPanel = true;

        $scope.selectPanelControl(sfxPanelModel);
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.AddPanelWithGrid = function (index, isInsert) {
        var strPanelId;
        $rootScope.UndRedoBulkOp("Start");
        var sfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {
            }, Elements: []
        };
        sfxRowModel.ParentVM = $scope.SfxMainTable;

        var sfxColumnModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {
            }, Elements: [], Children: []
        };
        sfxColumnModel.ParentVM = sfxRowModel;

        strPanelId = CreateControlID($scope.FormModel, "NewPanel", "sfwPanel", false);


        var sfxPanelModel = {
            Name: "sfwPanel", prefix: "swc", Value: '', dictAttributes: {
                ID: strPanelId, sfwCaption: "New Page"
            }, Elements: [], Children: []
        };
        sfxPanelModel.ParentVM = sfxColumnModel;
        $rootScope.PushItem(sfxPanelModel, sfxColumnModel.Elements);
        $rootScope.PushItem(sfxColumnModel, sfxRowModel.Elements);

        var strCtrlId = CreateControlID($scope.FormModel, "NewPage", "sfwTable", false);
        var sfxTableModel = {
            Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: { ID: strCtrlId }, Elements: [], Children: []
        };
        sfxTableModel.ParentVM = sfxPanelModel;

        var newSfxRowModel = {
            Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxRowModel.ParentVM = sfxTableModel;
        var newSfxCellModel = {
            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newSfxCellModel.ParentVM = newSfxRowModel;

        var newScope = $scope.$new();
        newScope.formobject = $scope.FormModel;
        newScope.model = undefined;
        newScope.dropdata = newSfxCellModel;
        newScope.IsAddNewGrid = true;
        newScope.IsAddGridWithPanel = true;
        newScope.BindToQueryDialog = $rootScope.showDialog(newScope, "Bind to Query", "Form/views/BindToQuery.html", { width: 600, height: 250 });

        newScope.onAfterOkClick = function () {
            $scope.AddPanelToList(sfxRowModel, sfxTableModel, isInsert, index);
        }

        $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

        $rootScope.PushItem(newSfxRowModel, sfxTableModel.Elements);
        $rootScope.PushItem(sfxTableModel, sfxPanelModel.Elements);

    };

    $scope.AddPanelToList = function (sfxRowModel, sfxTableModel, isInsert, index) {
        var sfxPanelModel = FindParent(sfxTableModel, "sfwPanel");

        $scope.MainPanels.some(function (panel) {
            if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                panel.initialvisibilty = false;
                panel.IsVisible = false;
                panel.IsPanelToggle = false;
                return true;
            }
        });

        // when we add panel for the first 
        if (!$scope.SfxMainTable) {
            $scope.SfxMainTable = {
                Name: "sfwTable", prefix: "swc", Value: '', dictAttributes: {
                }, Elements: [], Children: []
            };
            $rootScope.PushItem($scope.SfxMainTable, $scope.FormModel.Elements);
        }
        if (isInsert && index > -1) {

            $rootScope.InsertItem(sfxRowModel, $scope.SfxMainTable.Elements, index);
            $rootScope.InsertItem(sfxPanelModel, $scope.MainPanels, index);

        }
        else {
            $rootScope.PushItem(sfxRowModel, $scope.SfxMainTable.Elements);
            $rootScope.PushItem(sfxPanelModel, $scope.MainPanels);
        }
        sfxPanelModel.initialvisibilty = true;
        sfxPanelModel.TableVM = sfxTableModel;
        sfxPanelModel.IsMainPanel = true;

        $scope.selectPanelControl(sfxPanelModel);
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.OnAddWizardStepClick = function (index, isInsert) {
        var newScope = $scope.$new();
        newScope.formmodel = $scope.FormModel;
        newScope.AddPanelType = "AddWizardStep";
        newScope.OkClick = function () {
            if (newScope.AddPanelType === "AddWizardStep") {
                $scope.OnAddWizardStep(index, isInsert);
                newScope.closeDetailDialog();
            }
            else if (newScope.AddPanelType === "AddWizardStepWithTab") {
                $scope.OnAddWizardStepWithTabContainer(index, isInsert);
                newScope.closeDetailDialog();
            }
            else if (newScope.AddPanelType === "AddWizardStepWithGrid") {

                $scope.OnAddWizardStepWithGridView(index, isInsert);
                newScope.closeDetailDialog();
            }
        }

        newScope.validatePanel = function () {
            var retVal = false;
            newScope.ErrorMessage = "";

            if (!newScope.AddPanelType) {
                newScope.ErrorMessage = "Select Option.";
                retVal = true;
            }
            return retVal;
        };

        newScope.closeDetailDialog = function () {
            newScope.dialogToAddPanel.close();
        }

        newScope.dialogToAddPanel = $rootScope.showDialog(newScope, "Add Panel", "Form/views/AddPanels.html", {
            width: 620, height: 195
        });
    };

    $scope.OnAddWizardStep = function (index, isInsert) {
        $rootScope.UndRedoBulkOp("Start");
        var lst = [];
        var objWizardSteps;
        FindControlListByName($scope.SfxMainTable, "WizardSteps", lst);
        if (lst.length > 0) {
            objWizardSteps = lst[0];
        }
        if (objWizardSteps) {
            var strClientId = CreateControlID($scope.FormModel, "wzsStep", "sfwWizardStep", false);
            var objModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.dictAttributes.ID == strClientId;
            });

            var iNum = 0;
            var strID = strClientId;
            while (objModel && objModel.length > 0) {
                iNum++;
                strClientId = strID + iNum;
                objModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID == strClientId;
                });
            }
            var strTitle = strClientId;
            var objStep = {
                Name: 'sfwWizardStep', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objStep.ParentVM = objWizardSteps;
            objStep.dictAttributes.runat = "server";
            objStep.dictAttributes.sfwShowInHeader = "True";
            objStep.dictAttributes.ID = strClientId;
            objStep.dictAttributes.Title = strTitle.replace('wzsStep', 'New Step ');


            var objTable = {
                Name: 'sfwTable', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objTable.ParentVM = objStep;
            objTable.dictAttributes.runat = "server";
            objTable.dictAttributes.CssClass = "Table";

            var strTableID = "tblStep1";
            var objtableModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.Elements[0].dictAttributes.ID == strTableID;
            });

            var iNum = 0;
            var strNewTableID = strTableID;
            while (objtableModel && objtableModel.length > 0) {
                iNum++;
                strTableID = strNewTableID + iNum;
                objtableModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.Elements[0].dictAttributes.ID == strTableID;
                });
            }
            objTable.dictAttributes.ID = strTableID;

            $rootScope.PushItem(objTable, objStep.Elements);


            var objRow = {
                Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objRow.ParentVM = objTable;

            $rootScope.PushItem(objRow, objTable.Elements);

            for (var i = 0; i < 4; i++) {

                var objCell = {
                    Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
                };
                objCell.ParentVM = objRow;
                $rootScope.PushItem(objCell, objRow.Elements);
            }

            $scope.MainPanels.some(function (panel) {
                if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                    panel.initialvisibilty = false;
                    panel.IsVisible = false;
                    panel.IsPanelToggle = false;
                    return true;
                }
            });

            if (isInsert && index > -1) {
                $rootScope.InsertItem(objStep, objWizardSteps.Elements, index);
                $rootScope.InsertItem(objStep, $scope.MainPanels, index);
            }
            else {
                $rootScope.PushItem(objStep, objWizardSteps.Elements);
                $rootScope.PushItem(objStep, $scope.MainPanels);
            }

            objStep.initialvisibilty = true;
            objStep.TableVM = objTable;

            $scope.selectPanelControl(objStep);

        }
        $rootScope.UndRedoBulkOp("End");
    }

    $scope.OnAddWizardStepWithTabContainer = function (index, isInsert) {
        $rootScope.UndRedoBulkOp("Start");
        var lst = [];
        var objWizardSteps;
        FindControlListByName($scope.SfxMainTable, "WizardSteps", lst);
        if (lst.length > 0) {
            objWizardSteps = lst[0];
        }
        if (objWizardSteps) {
            var strClientId = CreateControlID($scope.FormModel, "wzsStep", "sfwWizardStep", false);
            var objModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.dictAttributes.ID == strClientId;
            });

            var iNum = 0;
            var strID = strClientId;
            while (objModel && objModel.length > 0) {
                iNum++;
                strClientId = strID + iNum;
                objModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID == strClientId;
                });
            }
            var strTitle = strClientId;
            var objStep = {
                Name: 'sfwWizardStep', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objStep.ParentVM = objWizardSteps;
            objStep.dictAttributes.runat = "server";
            objStep.dictAttributes.sfwShowInHeader = "True";
            objStep.dictAttributes.ID = strClientId;
            objStep.dictAttributes.Title = strTitle.replace('wzsStep', 'New Step ');


            var objTable = {
                Name: 'sfwTable', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objTable.ParentVM = objStep;
            objTable.dictAttributes.runat = "server";
            objTable.dictAttributes.CssClass = "Table";

            var strTableID = "tblStep1";
            var objtableModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.Elements[0].dictAttributes.ID == strTableID;
            });

            var iNum = 0;
            var strNewTableID = strTableID;
            while (objtableModel && objtableModel.length > 0) {
                iNum++;
                strTableID = strNewTableID + iNum;
                objtableModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.Elements[0].dictAttributes.ID == strTableID;
                });
            }
            objTable.dictAttributes.ID = strTableID;

            $rootScope.PushItem(objTable, objStep.Elements);


            var objRow = {
                Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objRow.ParentVM = objTable;

            $rootScope.PushItem(objRow, objTable.Elements);

            var objCell = {
                Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objCell.ParentVM = objRow;
            var newTabContainer = CreateTabContainer($scope.FormModel, objCell);
            $rootScope.PushItem(newTabContainer, objCell.Elements);
            $rootScope.PushItem(objCell, objRow.Elements);


            $scope.MainPanels.some(function (panel) {
                if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                    panel.initialvisibilty = false;
                    panel.IsVisible = false;
                    panel.IsPanelToggle = false;
                    return true;
                }
            });

            if (isInsert && index > -1) {
                $rootScope.InsertItem(objStep, objWizardSteps.Elements, index);
                $rootScope.InsertItem(objStep, $scope.MainPanels, index);
            }
            else {
                $rootScope.PushItem(objStep, objWizardSteps.Elements);
                $rootScope.PushItem(objStep, $scope.MainPanels);
            }

            objStep.initialvisibilty = true;
            objStep.TableVM = objTable;

            $scope.selectPanelControl(objStep);

        }
        $rootScope.UndRedoBulkOp("End");
    }

    $scope.OnAddWizardStepWithGridView = function (index, isInsert) {
        $rootScope.UndRedoBulkOp("Start");
        var lst = [];
        var objWizardSteps;
        FindControlListByName($scope.SfxMainTable, "WizardSteps", lst);
        if (lst.length > 0) {
            objWizardSteps = lst[0];
        }
        if (objWizardSteps) {
            var strClientId = CreateControlID($scope.FormModel, "wzsStep", "sfwWizardStep", false);
            var objModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.dictAttributes.ID == strClientId;
            });

            var iNum = 0;
            var strID = strClientId;
            while (objModel && objModel.length > 0) {
                iNum++;
                strClientId = strID + iNum;
                objModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID == strClientId;
                });
            }
            var strTitle = strClientId;
            var objStep = {
                Name: 'sfwWizardStep', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objStep.ParentVM = objWizardSteps;
            objStep.dictAttributes.runat = "server";
            objStep.dictAttributes.sfwShowInHeader = "True";
            objStep.dictAttributes.ID = strClientId;
            objStep.dictAttributes.Title = strTitle.replace('wzsStep', 'New Step ');


            var objTable = {
                Name: 'sfwTable', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objTable.ParentVM = objStep;
            objTable.dictAttributes.runat = "server";
            objTable.dictAttributes.CssClass = "Table";

            var strTableID = "tblStep1";
            var objtableModel = objWizardSteps.Elements.filter(function (itm) {
                return itm.Elements[0].dictAttributes.ID == strTableID;
            });

            var iNum = 0;
            var strNewTableID = strTableID;
            while (objtableModel && objtableModel.length > 0) {
                iNum++;
                strTableID = strNewTableID + iNum;
                objtableModel = objWizardSteps.Elements.filter(function (itm) {
                    return itm.Elements[0].dictAttributes.ID == strTableID;
                });
            }
            objTable.dictAttributes.ID = strTableID;

            $rootScope.PushItem(objTable, objStep.Elements);


            var objRow = {
                Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objRow.ParentVM = objTable;

            $rootScope.PushItem(objRow, objTable.Elements);

            var objCell = {
                Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: []
            };
            objCell.ParentVM = objRow;


            var newScope = $scope.$new();
            newScope.formobject = $scope.FormModel;
            newScope.model = undefined;
            newScope.dropdata = objCell;
            newScope.IsAddNewGrid = true;
            newScope.IsAddGridWithPanel = true;
            newScope.BindToQueryDialog = $rootScope.showDialog(newScope, "Bind to Query", "Form/views/BindToQuery.html", {
                width: 600, height: 250
            });

            newScope.onAfterOkClick = function () {
                $scope.AddWizardStepToList(objStep, objTable, objWizardSteps, isInsert, index);
            }

            $rootScope.PushItem(objCell, objRow.Elements);
        }
    }

    $scope.AddWizardStepToList = function (objStep, objTable, objWizardSteps, isInsert, index) {
        $scope.MainPanels.some(function (panel) {
            if (panel.dictAttributes.ID == $scope.CurrPanel.dictAttributes.ID) {
                panel.initialvisibilty = false;
                panel.IsVisible = false;
                panel.IsPanelToggle = false;
                return true;
            }
        });

        if (isInsert && index > -1) {
            $rootScope.InsertItem(objStep, objWizardSteps.Elements, index);
            $rootScope.InsertItem(objStep, $scope.MainPanels, index);
        }
        else {
            $rootScope.PushItem(objStep, objWizardSteps.Elements);
            $rootScope.PushItem(objStep, $scope.MainPanels);
        }

        objStep.initialvisibilty = true;
        objStep.TableVM = objTable;

        $scope.selectPanelControl(objStep);

        $rootScope.UndRedoBulkOp("End");

    }

    //#endregion


    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objFormExtraFields && $scope.objFormExtraFields.Elements && $scope.objFormExtraFields.Elements.length > 0) {
            var index = $scope.FormModel.Elements.indexOf($scope.objFormExtraFields);
            if (index == -1) {
                $scope.FormModel.Elements.push($scope.objFormExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();

        //usercontrol and tooltip will be supported by fwk later
        if (!$scope.IsPrototype && $scope.FormModel && $scope.FormModel.dictAttributes && $scope.FormModel.dictAttributes.sfwType != "UserControl" && $scope.FormModel.dictAttributes.sfwType != "Lookup") {
            $scope.dummyLstLoadDetails = LoadDetails($scope.FormModel, $scope.objLoadDetails, false, $rootScope, false);
        }
    };

    $scope.AfterSaveToFile = function () {
    };

    $scope.ClearSelectFields = function () {
        lstEntityTreeFieldData = null;
    };

    //#region Load Details
    $scope.OpenLoadDetailsDialog = function (IsSave) {
        var newScope = $scope.$new();

        $scope.dummyLstLoadDetails = LoadDetails($scope.FormModel, $scope.objLoadDetails, true, $rootScope, false);
        dialog = $rootScope.showDialog(newScope, "Load Details", "Form/views/LoadDetails.html", { width: 500, height: 400 });
        newScope.onOkClick = function () {

            dialog.close();
        };
    };


    //#endregion

    $scope.createValidationRuleList = function (objExtraData, isWizard, strRuleGroup) {
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

    var setEntity = function (objChild) {
        var ObjGrid;
        // console.log(objChild);
        if (objChild && objChild.Name == "sfwGridView") {
            ObjGrid = objChild;
        }
        else {
            ObjGrid = FindParent(objChild, "sfwGridView");
        }
        if (ObjGrid) {
            $scope.loadGridEntityTree(ObjGrid);
        }
        else {
            if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                var Objdialog = FindParent(objChild, "sfwDialogPanel");
                if (Objdialog) {
                    $scope.loadGridEntityTree(Objdialog);
                }
                else {
                    var Objlist = FindParent(objChild, "sfwListView");
                    if (Objlist) {
                        $scope.loadGridEntityTree(Objlist);
                    }
                    else {
                        $scope.loadFormEntityTree(objChild);
                    }
                }
            }
        }
    };

    $scope.loadGridEntityTree = function (item) {
        $scope.IsGridSeleected = true;
        $scope.IsListViewSelected = false;
        $scope.FormModel.SelectedControl.IsGridChildOfListView = false;
        var isItemTemplate = false;
        var blnFoundEntity = false;
        $scope.IsGridCollectionEmpty = false;
        var listViewparent = FindParent(item, "sfwListView");
        if (item) {
            var entityfieldname;
            if (item.Name == "sfwGridView" && listViewparent) {


                $scope.FormModel.SelectedControl.IsGridChildOfListView = true;
                var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                if (objParentField) {
                    objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, item.dictAttributes.sfwEntityField);
                    if (objField) {
                        entityfieldname = objField.Entity;
                    }
                }

            }

            else if (item.Name == "sfwGridView") {

                if (item.dictAttributes.sfwParentGrid) {
                    var objGrid = FindControlByID($scope.SfxMainTable, item.dictAttributes.sfwParentGrid);
                    if (objGrid && objGrid.dictAttributes.sfwEntityField) {
                        var entityName = null;
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                        if (object) {
                            entityfieldname = object.ID;
                            entityName = object.Entity;
                            if (entityName && $scope.FormModel.SelectedControl && item.dictAttributes.sfwEntityField) {
                                var entObj = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityName, item.dictAttributes.sfwEntityField);
                                if (entObj) {
                                    if (entObj.DataType && entObj.DataType.toLowerCase() == "collection") {
                                        entityfieldname = entObj.Entity;
                                        blnFoundEntity = true;
                                    }
                                }
                            }
                            else if ($scope.FormModel.SelectedControl.Name != "TemplateField") {
                                $scope.IsGridCollectionEmpty = true;
                            }

                        }
                    }
                } else {
                    entityfieldname = item.dictAttributes.sfwEntityField;
                }
            }

            else if (item.Name == "sfwDialogPanel") {

                var strdialogpanelid = item.dictAttributes.ID;
                if (strdialogpanelid) {
                    var button = GetFieldFromFormObject($scope.SfxMainTable, 'sfwButton', 'sfwRelatedDialogPanel', strdialogpanelid);
                    if (button && button.length > 0 && button[0].dictAttributes.sfwRelatedControl) {
                        var gridview = GetFieldFromFormObject($scope.SfxMainTable, 'sfwGridView', 'ID', button[0].dictAttributes.sfwRelatedControl);
                        if (gridview && gridview.length > 0) {
                            entityfieldname = gridview[0].dictAttributes.sfwEntityField;
                        }
                    }
                    else {
                        var objScheduler = GetFieldFromFormObject($scope.SfxMainTable, 'sfwScheduler', 'sfwRelatedDialogPanel', strdialogpanelid);
                        if (objScheduler && objScheduler.length > 0) {
                            entityfieldname = objScheduler[0].dictAttributes.sfwEntityField;
                        }
                    }
                }
            }
            else if (item.Name == "sfwListView") {

                $scope.IsListViewSelected = true;
                entityfieldname = item.dictAttributes.sfwEntityField;
            }
            // if (entityfieldname && isLoadEntityTreeFromList) {
            if (entityfieldname) {
                if (entityfieldname == "InternalErrors" || entityfieldname == "ExternalErrors") {
                    entityfieldname = "entError";
                }
                else {
                    var objField = undefined;
                    if (item.Name == "sfwGridView" && listViewparent) {

                        $scope.FormModel.SelectedControl.IsGridChildOfListView = true;
                        var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                        if (objParentField) {
                            objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, entityfieldname);
                            if (objField) {
                                entityfieldname = objField.Entity;
                            }
                        }

                    }
                    else if ($scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.IsChildOfGrid && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.Name != "TemplateField") {
                        var objGrid = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
                        if (objGrid && objGrid.dictAttributes.sfwParentGrid && objGrid.dictAttributes.sfwEntityField) {
                            var objParentGrid = FindControlByID($scope.FormModel, objGrid.dictAttributes.sfwParentGrid);
                            if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                                if (entObject) {
                                    entityfieldname = entObject.Entity;
                                    if (FindParent($scope.FormModel.SelectedControl, "ItemTemplate")) {
                                        var entObj = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityfieldname, objGrid.dictAttributes.sfwEntityField);
                                        if (entObj) {
                                            entityfieldname = entObj.Entity;
                                        }
                                    }
                                }
                            }
                        } else if (objGrid && objGrid.dictAttributes.sfwEntityField) {
                            var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                            if (objField) {
                                entityfieldname = objField.Entity;
                            }

                        }
                    }
                    else if (!blnFoundEntity) {
                        var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, entityfieldname);
                        if (objField && item.Name != "sfwGridView") {
                            entityfieldname = objField.Entity;
                        } else if (objField && item.Name == "sfwGridView" && objField.DataType && objField.DataType.toLowerCase() != "object") {
                            entityfieldname = objField.Entity;
                        } else {
                            entityfieldname = '';
                        }
                    }
                }
                $scope.CheckAndLoadEntityTree(entityfieldname);
            }
            else {
                if ($scope.FormModel.dictAttributes.sfwType !== "Lookup") {
                    $scope.$evalAsync(function () {

                        //if (item.Name == "sfwGridView" && listViewparent) {

                        //        $scope.FormModel.SelectedControl.IsGridChildOfListView = true;
                        //        var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                        //        if (objParentField) {
                        //            entityfieldname = objParentField.Entity;
                        //        }

                        //}

                        //else {
                        if (!(item.Name == "sfwGridView" && listViewparent)) {
                            for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                                $scope.lstLoadedEntityTrees[i].IsVisible = false;
                            }
                            $scope.entityTreeName = "";
                            $scope.currentEntiyTreeObject = undefined;
                        }
                        //}

                    });
                }
            }
        }
    };

    $scope.CheckAndLoadEntityTree = function (entityfieldname) {
        var blnFound = false;
        for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
            if ($scope.lstLoadedEntityTrees[i].EntityName == entityfieldname && !$scope.lstLoadedEntityTrees[i].IsQuery) {
                blnFound = true;
                $scope.lstLoadedEntityTrees[i].IsVisible = true;
                $scope.entityTreeName = $scope.lstLoadedEntityTrees[i].EntityName;
                $scope.currentEntiyTreeObject = $scope.lstLoadedEntityTrees[i];
                break;
            }
            else {
                $scope.lstLoadedEntityTrees[i].IsVisible = false;

            }
        }
        if (!blnFound) {
            var objnew = {
                EntityName: entityfieldname, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: false
            };
            $scope.lstLoadedEntityTrees.push(objnew);
            $scope.entityTreeName = objnew.EntityName;
            $scope.currentEntiyTreeObject = objnew;
        }
    };

    $scope.PopulateEntityFieldsForOpenButton = function (obj, isLookup) {
        var alAvlFlds = [];
        PopulateControlsForActiveForm(alAvlFlds, $scope.FormModel, obj, isLookup);
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

    $scope.loadFormEntityTree = function (item) {
        if ($scope.FormModel && $scope.FormModel.IsLookupCriteriaEnabled == false) {
            $scope.IsGridSeleected = false;

            var blnFound = false;
            if ($scope.lstLoadedEntityTrees) {
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                    if ($scope.lstLoadedEntityTrees[i].EntityName == $scope.FormModel.dictAttributes.sfwEntity && !$scope.lstLoadedEntityTrees[i].IsQuery) {
                        blnFound = true;
                        $scope.lstLoadedEntityTrees[i].IsVisible = true;
                        $scope.entityTreeName = $scope.lstLoadedEntityTrees[i].EntityName;
                        $scope.currentEntiyTreeObject = $scope.lstLoadedEntityTrees[i];
                    }
                    else {
                        $scope.lstLoadedEntityTrees[i].IsVisible = false;

                    }

                }
                if (!blnFound) {
                    var objNew = {
                        EntityName: $scope.FormModel.dictAttributes.sfwEntity, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: false
                    };
                    $scope.lstLoadedEntityTrees.push(objNew);
                    $scope.entityTreeName = objNew.EntityName;
                    $scope.currentEntiyTreeObject = objNew;
                }
            }
        }
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

    $scope.GetParent = function (itm) {
        var control = FindParent(itm, "sfwGridView", true);
        if (!control) {
            control = FindParent(itm, "sfwDialogPanel", true);
            if (!control) {
                control = FindParent(itm, "sfwListView", true);
            }
        }
        return control;
    };

    $scope.CanPasteControl = function (cellVM, model, obj) {
        var retVal = true;
        if (model && model.Name == "sfwTable") {
            return false;
        }
        var dragElementParent = FindParent(obj, "sfwGridView");
        var dropElementParent = FindParent(cellVM, "sfwGridView");
        if (dragElementParent && dropElementParent && dragElementParent.dictAttributes.ID == dropElementParent.dictAttributes.ID) {
            retVal = true;
        }
        else if (FindParent(cellVM, "sfwTable") && FindParent(obj, "sfwGridView")) {
            return false;
        }
        var parent = $scope.GetParent(cellVM);
        if (["sfwScheduler", "sfwGridView", "sfwListView", "sfwChart", "sfwCalendar"].indexOf(model.Name) > -1 && FindParent(cellVM, "sfwDialogPanel")) {
            retVal = false;
        }
        else if (FindParent(cellVM, "sfwGridView") && FindParent(obj, "sfwDialogPanel")) {
            return false;
        }
        else if (FindParent(cellVM, "sfwListView") && FindParent(obj, "sfwDialogPanel")) {
            return false;
        }
        else if (["sfwScheduler", "sfwDialogPanel", "sfwListView", "sfwChart", "sfwCalendar"].indexOf(model.Name) > -1 && FindParent(cellVM, "sfwGridView")) {
            retVal = false;
        }

        else if ((model.Name === "sfwGridView" || model.Name === "sfwDialogPanel" || model.Name === "sfwListView" || model.Name === "sfwChart" || model.Name === "sfwCalendar" || model.Name === "sfwScheduler") && parent) {
            if (!CanaddGridInsideListView(obj, cellVM, "sfwListView")) {
                retVal = true;
            }
            else {
                retVal = false;
            }
        }
        else if (model.Name === "sfwGridView" && !parent && CanaddGridInsideListView(obj, cellVM, "sfwListView")) {

            retVal = false;

        }
        else if (CanaddGridInsideListView(obj, cellVM, "sfwGridView")) {
            return false;
        }
        return retVal;
    }
    function CanaddGridInsideListView(obj, cellVM, ParentName) {
        var retValue = false;
        var dragDataParent = FindParent(obj, ParentName);
        var dropDataParent = FindParent(cellVM, ParentName);
        if (!dragDataParent && !dropDataParent) {
            retValue = false;
        }
        else {
            if (dragDataParent && dropDataParent && dragDataParent.dictAttributes && dropDataParent.dictAttributes) {
                if (dragDataParent.dictAttributes.ID != dropDataParent.dictAttributes.ID) {
                    retValue = true;
                }
            }
            else if (!dragDataParent || !dropDataParent) {
                retValue = true;
            }
        }
        return retValue;
    }
    $scope.OnPasteControl = function (cellVM) {

        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                $rootScope.UndRedoBulkOp("Start");
                function iteratorClipboardData(obj) {
                    var model = GetBaseModel(obj);
                    if (!($scope.FormModel.dictAttributes && $scope.FormModel.dictAttributes.sfwType == "Lookup" && $scope.FormModel.IsLookupCriteriaEnabled && model && model.Name == "sfwLabel" && !model.dictAttributes.sfwIsCaption)) {

                        if ($scope.CanPasteControl(cellVM, model, obj)) {
                            var dropItemTemplate = FindParent(cellVM, "ItemTemplate");
                            var dragItemTemplate = FindParent(obj, "ItemTemplate");
                            if (dropItemTemplate && dragItemTemplate) {
                                $rootScope.PushItem(model, dropItemTemplate.Elements);
                            }

                            else {
                                $rootScope.PushItem(model, cellVM.Elements);
                            }

                            if ($scope.IsCutOper) {
                                $scope.OnDeleteControlClick(obj);
                            }
                            else {
                                if (obj.Name == "sfwLabel" && obj.dictAttributes && obj.dictAttributes.sfwIsCaption && model.dictAttributes.ID) {
                                    model.dictAttributes.ID = GetControlIDForCaption($scope.FormModel, obj.Name, true);
                                }
                                else if (model.dictAttributes.ID) {
                                    model.dictAttributes.ID = GetControlID($scope.FormModel, obj.Name);
                                }
                                if (model && model.Elements.length > 0) {
                                    $scope.ChangeControlID(model);
                                }
                            }
                            if (obj.Name == "udc") {
                                model.UcChild = [];
                                model.UcChild.push(obj.UcChild[0]);
                            }
                            if ($scope.FormModel.dictAttributes && $scope.FormModel.dictAttributes.sfwType == "Lookup") {
                                if ($scope.FormModel.IsLookupCriteriaEnabled && (model.dictAttributes.sfwEntityField || model.dictAttributes.sfwLinkable || model.dictAttributes.sfwRelatedControl)) {
                                    if (model.dictAttributes.sfwEntityField) {
                                        delete model.dictAttributes.sfwEntityField;
                                    }
                                    if (model.dictAttributes.sfwLinkable) {
                                        delete model.dictAttributes.sfwLinkable;
                                    }
                                    if (model.dictAttributes.sfwRelatedControl) {
                                        delete model.dictAttributes.sfwRelatedControl;
                                    }
                                }
                                else if (!$scope.FormModel.IsLookupCriteriaEnabled && (model.dictAttributes.sfwDataField || model.dictAttributes.sfwQueryId)) {
                                    if (model.dictAttributes.sfwDataField) {
                                        delete model.dictAttributes.sfwDataField;
                                    }
                                    if (model.dictAttributes.sfwQueryID) {
                                        delete model.dictAttributes.sfwQueryID;
                                    }
                                }
                            }

                            if ($scope.IsCutOper) {
                                $scope.CheckForFilterGrid(obj, model);
                            }
                            model.ParentVM = cellVM;
                        }
                    }
                }

                angular.forEach($scope.ClipboardData, iteratorClipboardData);
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
    $scope.ChangeControlID = function (objModel) {
        angular.forEach(objModel.Elements, function (obj) {
            var model = obj;
            if (obj.Name == "sfwLabel" && obj.dictAttributes && obj.dictAttributes.sfwIsCaption && model.dictAttributes.ID) {
                model.dictAttributes.ID = GetControlIDForCaption($scope.FormModel, obj.Name, true);
            }
            else if (model.dictAttributes.ID) {
                model.dictAttributes.ID = GetControlID($scope.FormModel, obj.Name);
            }
            if (model && model.Elements && model.Elements.length > 0) {
                $scope.ChangeControlID(model);
            }
        });
    };

    $scope.selectControlOnDoubleClick = function (objChild, event) {
        $scope.selectControl(objChild, event);
        $scope.OnSelectLeftFormTab('Properties');
        if ($scope.IsToolsDivCollapsed) {
            $scope.IsToolsDivCollapsed = !$scope.IsToolsDivCollapsed;
        }
    };

    $scope.selectControl = function (objChild, event) {
        $scope.isTemplateFieldSelected = false;
        $scope.IsGridCollectionEmpty = false;
        if (objChild) {
            if (!objChild.isLoaded) {
                objChild.isLoaded = true;
            }
            if ($scope.FormModel) {
                if ($scope.FormModel.SelectedControl && ($scope.FormModel.SelectedControl.Name == "sfwPanel" || $scope.FormModel.SelectedControl.Name == "sfwDialogPanel" || $scope.FormModel.SelectedControl.Name == "sfwListView")) {
                    $scope.FormModel.SelectedControl.IsVisible = false;
                }

                SetFormSelectedControl($scope.FormModel, objChild, event);

                if (objChild.Name == "sfwTabSheet") {
                    objChild.ParentVM.SelectedTabSheet = objChild;
                    objChild.ParentVM.SelectedTabSheet.IsSelected = true;
                }

                if (objChild.Name == "sfwPanel" || objChild.Name == "sfwDialogPanel" || objChild.Name == "sfwListView") {
                    //objChild.initialvisibilty = !objChild.initialvisibilty;
                    objChild.IsVisible = true;
                }
                if (objChild.Name == "sfwGridView") {
                    if (objChild.dictAttributes.sfwBoundToQuery && objChild.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                        $scope.setQueryFieldsIfGridisBoundedToQuery(objChild);
                    } else {

                        $scope.loadGridEntityTree(objChild);
                        $scope.ObjgridBoundedQuery.IsQuery = false;
                    }
                }
                else if (objChild.Name == "sfwDialogPanel") {
                    $scope.loadGridEntityTree(objChild);
                    $scope.ObjgridBoundedQuery.IsQuery = false;
                }
                else if (objChild.Name == "sfwListView") {
                    $scope.loadGridEntityTree(objChild);
                    $scope.ObjgridBoundedQuery.IsQuery = false;
                }
                else {
                    var ObjGrid = FindParent(objChild, "sfwGridView");

                    if (ObjGrid && objChild.Name == "TemplateField") {
                        $scope.isTemplateFieldSelected = true;
                    }

                    //For template field we need from main entity
                    if (ObjGrid) {
                        if (ObjGrid.dictAttributes.sfwBoundToQuery && ObjGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                            $scope.setQueryFieldsIfGridisBoundedToQuery(ObjGrid);
                        } else {
                            $scope.FormModel.SelectedControl.IsChildOfGrid = true;
                            $scope.loadGridEntityTree(ObjGrid);
                            $scope.ObjgridBoundedQuery.IsQuery = false;
                        }

                    }
                    else {
                        $scope.ObjgridBoundedQuery.IsQuery = false;
                        if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                            var Objdialog = FindParent(objChild, "sfwDialogPanel");
                            if (Objdialog) {
                                $scope.loadGridEntityTree(Objdialog);
                            }
                            else {
                                var Objlist = FindParent(objChild, "sfwListView");
                                if (Objlist) {
                                    $scope.loadGridEntityTree(Objlist);
                                }
                                else {
                                    var gridModel = FindParent(objChild, "sfwGridView");
                                    if (gridModel && gridModel.dictAttributes.sfwParentGrid) {
                                        $scope.loadGridEntityTree(gridModel);
                                    } else {
                                        $scope.loadFormEntityTree(objChild);
                                    }
                                }
                            }
                        }
                    }
                }
                //$scope.AddRowsAndColumns(objChild, event);
                if ($scope.setDisplayNoneToTable) {
                    $scope.setDisplayNoneToTable();
                }

                if (event && event.stopPropagation) {
                    event.stopPropagation();
                }
            }
        }
    };

    $scope.onRefreshBoundtoQueryFields = function () {
        if ($scope.FormModel.SelectedControl) {
            $scope.selectControl($scope.FormModel.SelectedControl, event);
            $scope.ShowRefreshCompletedDialog();
        }
    };

    $scope.setQueryFieldsIfGridisBoundedToQuery = function (objChild) {
        if ((objChild && objChild.dictAttributes.sfwBoundToQuery && objChild.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") || ObjGrid) {
            var QueryID = objChild.dictAttributes.sfwBaseQuery;
            $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = [];
            $scope.ObjgridBoundedQuery.SortedColumns = [];
            //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
            $.connection.hubForm.server.getEntityQueryColumns(QueryID, "dummy").done(function (data) {
                //$scope.receiveQueryFields(data, query.dictAttributes.sfwQueryRef);
                $scope.$evalAsync(function () {
                    if (data && data.length > 0) {
                        var lstDataFields = data;
                        sortListBasedOnproperty(lstDataFields, "", "CodeID");
                        $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = $scope.getModelForBoundedQueryList(lstDataFields);
                        $scope.ObjgridBoundedQuery.SortedColumns = $scope.ObjgridBoundedQuery.lstselectedobjecttreefields;
                    }
                });
            });
            $scope.IsGridSeleected = true;
            if (QueryID) {
                $scope.ObjgridBoundedQuery.IsQuery = true;
            }
            $scope.entityTreeName = $scope.FormModel.dictAttributes.sfwEntity;
        } else {
            $scope.ObjgridBoundedQuery.IsQuery = false;
        }
    };

    $scope.getModelForBoundedQueryList = function (lstfields) {
        var lst = [];
        lst = $getModelList.getModelListFromQueryFieldlist(lstfields);
        return lst;
    };

    $scope.OnDeleteControlClick = function (aParam) {
        if (aParam) {
            $rootScope.DeleteItem(aParam, aParam.ParentVM.Elements);
            if (aParam.ParentVM) {
                $scope.selectControl(aParam.ParentVM, null);
            }
        }
    };


    $scope.OnCreateCompatibleLabelClickForPanel = function (panelModel) {
        if (panelModel) {
            $rootScope.UndRedoBulkOp("Start");
            $scope.CreateCompatibleLabelForControls(panelModel);
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.CreateCompatibleLabelForControls = function (objModel) {
        if ($scope.ValidateControl(objModel)) {
            CreateCompatibleLabel(objModel, $scope.FormModel, $EntityIntellisenseFactory, $rootScope);
        }

        function iterator(itm) {
            $scope.CreateCompatibleLabelForControls(itm);
        }
        angular.forEach(objModel.Elements, iterator);
    };

    $scope.ValidateControl = function (objModel) {
        if ((objModel && objModel.dictAttributes.sfwIsCaption)) {
            return false;
        }
        else if (objModel && (objModel.Name == "sfwGridView" || objModel.Name == "sfwChart" || objModel.Name == "sfwPanel" ||
            objModel.Name == "sfwDialogPanel" || objModel.Name == "sfwButton" || objModel.Name == "sfwTabContainer" || objModel.Name == "sfwTabSheet"
            || objModel.Name == "udc" || objModel.Name == "sfwListViewer")) {
            return false;
        }
        else {
            objModel.IsChildOfGrid = false;
            var objGridView = FindParent(objModel, "sfwGridView");
            if (objGridView) {
                objModel.IsChildOfGrid = true;
            }
            if (objModel.IsChildOfGrid) {
                return false;
            }
            else if (objModel.dictAttributes && (!objModel.dictAttributes.sfwEntityField && !objModel.dictAttributes.sfwDataField)) {
                return false;
            }
            else if (hasCaption(objModel)) {
                return false;
            }
            return true;
        }
    };

    //#region Cut/Copy/Paste Cell

    $scope.OnCutCell = function (cellVM) {
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', true);
    };

    $scope.OnCopyCell = function (cellVM) {
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', false);
    };

    $scope.CopyToClipBoard = function (data, opetype, iscutoper) {
        $scope.ClipboardData = [];
        $scope.ClipboardDataOpeType = opetype;
        $scope.IsCutOper = iscutoper;
        function iterator(obj) {
            //var model = GetBaseModel(obj);
            $scope.ClipboardData.push(obj);
        }
        angular.forEach(data, iterator);
    };

    $scope.OnPasteCell = function (cellVM) {
        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                $rootScope.UndRedoBulkOp("Start");
                function iteratorClipboardData(obj) {
                    if ($scope.IsCutOper) {
                        $scope.ClearCell(obj.ParentVM);
                    }
                    var model = GetBaseModel(obj);
                    $rootScope.PushItem(model, cellVM.Elements);
                    if (!$scope.IsCutOper) {
                        if (obj.Name == "sfwLabel" && obj.dictAttributes && obj.dictAttributes.sfwIsCaption) {
                            model.dictAttributes.ID = GetControlIDForCaption($scope.FormModel, obj.Name, true);
                        }
                        else {
                            model.dictAttributes.ID = GetControlID($scope.FormModel, obj.Name);
                        }
                        if (model && model.Elements.length > 0) {
                            $scope.ChangeControlID(model);
                        }
                    }

                    if ($scope.IsCutOper) {
                        $scope.CheckForFilterGrid(obj, model);
                    }
                    if (obj.Name == "udc") {
                        model.UcChild = [];
                        model.UcChild.push(obj.UcChild[0]);
                    }

                    model.ParentVM = cellVM;
                }

                angular.forEach($scope.ClipboardData, iteratorClipboardData);
                if ($scope.IsCutOper) {
                    $scope.ClipboardData = [];
                    $scope.ClipboardDataOpeType = "";
                    $scope.IsCutOper = false;
                }
                $rootScope.UndRedoBulkOp("End");
            });
        }
    };

    $scope.CheckForFilterGrid = function (oldModel, newModel) {
        if (oldModel.IsShowDataField) {
            newModel.IsShowDataField = oldModel.IsShowDataField;
        }
        function iterator(itm) {
            for (var i = 0; i < oldModel.Elements.length; i++) {
                if (itm.Name == oldModel.Elements[i].Name && itm.dictAttributes.ID == oldModel.Elements[i].dictAttributes.ID) {
                    $scope.CheckForFilterGrid(oldModel.Elements[i], itm);
                }
            }
        }

        angular.forEach(newModel.Elements, iterator);
    };

    $scope.ClearCell = function (cellVM) {
        while (cellVM.Elements.length > 0) {
            $rootScope.DeleteItem(cellVM.Elements[0], cellVM.Elements);
        }
    };
    //#endregion

    $scope.OnCopyCutPasteControlClick = function (operation) {
        //cut
        if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.dictAttributes.sfwType == "Lookup" && ($scope.FormModel.SelectedControl.Name == "sfwTabContainer" || $scope.FormModel.SelectedControl.Name == "sfwGridView")) {
            return;
        }
        else if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.Name == "sfwToolTipButton") {
            return;
        }
        else if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.Name == "sfwPanel" && $scope.FormModel.SelectedControl.IsMainPanel) {
            return;
        }
        else if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.Name != "TemplateField") {
            if (operation == "cut") {
                if ($scope.FormModel.SelectedControl.Name != "sfwColumn") {
                    $scope.OnCutControlClick($scope.FormModel.SelectedControl);
                }
                else if ($scope.FormModel.SelectedControl.Name == "sfwColumn" || $scope.FormModel.SelectedControl.Name == "sfwButtonGroup") {
                    $scope.OnCutCell($scope.FormModel.SelectedControl);
                }
            }
            //copy
            else if (operation == "copy") {
                if ($scope.FormModel.SelectedControl.Name != "sfwColumn") {
                    $scope.OnCopyControlClick($scope.FormModel.SelectedControl);
                }
                else if ($scope.FormModel.SelectedControl.Name == "sfwColumn" || $scope.FormModel.SelectedControl.Name == "sfwButtonGroup") {
                    $scope.OnCopyCell($scope.FormModel.SelectedControl);
                }
            }
            //paste
            else if (operation == "paste" && $scope.FormModel.SelectedControl.Name == "sfwColumn" || $scope.FormModel.SelectedControl.Name == "sfwButtonGroup") {
                if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Control") {
                    $scope.OnPasteControl($scope.FormModel.SelectedControl);
                }
                else if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Cell") {
                    $scope.OnPasteCell($scope.FormModel.SelectedControl);
                }
            }
            else if (operation == "paste" && FindParent($scope.FormModel.SelectedControl, "sfwGridView")) {
                if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Control") {
                    $scope.OnPasteControl($scope.FormModel.SelectedControl);
                }
            }
        }
        else if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.Name == "TemplateField" && operation == "paste") {
            if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Control") {
                $scope.OnPasteControl($scope.FormModel.SelectedControl.Elements[0]);
            }
        }
    };

    $scope.OnRowColumnInsertMoveClick = function (operation) {
        if ($scope.FormModel && $scope.FormModel.SelectedControl) {
            if ($scope.FormModel.SelectedControl.Name == "sfwPanel" && $scope.FormModel.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                var tableVM = null;
                if ($scope.FormModel.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.FormModel.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.FormModel.SelectedControl, "sfwColumn");
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
        if ($scope.FormModel && $scope.FormModel.SelectedControl) {
            if ($scope.FormModel.SelectedControl.Name == "sfwPanel" && $scope.FormModel.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                //var objGridView = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
                if ($scope.FormModel.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.FormModel.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.FormModel.SelectedControl, "sfwColumn");
                }
                $scope.deleteCell(cellVM);
            }
        }
    };

    $scope.deleteCell = function (aCellVM) {
        //if table has only one cell it cannot be deleted
        if (aCellVM.ParentVM && aCellVM.ParentVM.ParentVM && aCellVM.ParentVM.ParentVM.Name == 'sfwTable' && aCellVM.ParentVM.ParentVM.Elements.length == 1 && aCellVM.ParentVM.Elements.length == 1) {
            $SgMessagesService.Message("Message", "Cannot delete as only one cell present.");

        }
        else {
            $rootScope.DeleteItem(aCellVM, aCellVM.ParentVM.Elements);
            if ($scope.FormModel && $scope.FormModel.SelectedControl && $scope.FormModel.SelectedControl.ParentVM) {
                if ($scope.FormModel.SelectedControl.ParentVM.Elements.length == 0 && $scope.FormModel.SelectedControl.ParentVM.Name == "sfwRow") {
                    $rootScope.DeleteItem($scope.FormModel.SelectedControl.ParentVM, $scope.FormModel.SelectedControl.ParentVM.ParentVM.Elements);
                    $scope.selectControl($scope.FormModel.SelectedControl.ParentVM.ParentVM, null);
                } else {
                    $scope.selectControl($scope.FormModel.SelectedControl.ParentVM, null);
                }
            }
        }
    };

    $scope.OnInsertCellClick = function (operation) {
        if ($scope.FormModel && $scope.FormModel.SelectedControl) {
            if ($scope.FormModel.SelectedControl.Name == "sfwPanel" && $scope.FormModel.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                var tableVM = null;
                //var objGridView = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
                if ($scope.FormModel.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.FormModel.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.FormModel.SelectedControl, "sfwColumn");
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
        var objGridView = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
        if (objGridView) {
            var col = FindParent($scope.FormModel.SelectedControl, "TemplateField");
            if ($scope.FormModel.SelectedControl.Name == "TemplateField") {
                col = $scope.FormModel.SelectedControl;
            }
            if (col) {
                var ColIndex = col.ParentVM.Elements.indexOf(col);
                if (ColIndex > 0) {
                    $rootScope.UndRedoBulkOp("Start");

                    var model = col.ParentVM.Elements[ColIndex];
                    //Removing
                    $rootScope.DeleteItem(col.ParentVM.Elements[ColIndex], col.ParentVM.Elements);

                    //Adding
                    $rootScope.InsertItem(model, col.ParentVM.Elements, ColIndex - 1);

                    $rootScope.UndRedoBulkOp("End");

                    // this.SelectControlAfterMoveOpe(selectedControlVM, curRowVM.RowIndex, cellVM.ColIndex - 1);
                }
            }
        }
        else {
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
        }
    };

    $scope.MoveColumnRight = function (aParam, tableVM) {
        var objGridView = FindParent($scope.FormModel.SelectedControl, "sfwGridView");
        if (objGridView) {
            var col = FindParent($scope.FormModel.SelectedControl, "TemplateField");
            if ($scope.FormModel.SelectedControl.Name == "TemplateField") {
                col = $scope.FormModel.SelectedControl;
            }
            if (col) {
                var ColCount = col.ParentVM.Elements.length;
                var ColIndex = col.ParentVM.Elements.indexOf(col);
                if (ColIndex < ColCount - 1) {
                    $rootScope.UndRedoBulkOp("Start");

                    var model = col.ParentVM.Elements[ColIndex];
                    //Removing
                    $rootScope.DeleteItem(col.ParentVM.Elements[ColIndex], col.ParentVM.Elements);

                    //Adding
                    $rootScope.InsertItem(model, col.ParentVM.Elements, ColIndex + 1);

                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
        else {
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
        }
    };

    //#endregion


    //#region create button method list
    $scope.ButtonsDetailsCollection = [];
    $scope.LoadButtonDetails = function () {
        var type = $scope.currentfile.FileType;
        var lstMethod = [];
        if (type) {
            if (type == "Lookup") {
                lstMethod = $rootScope.LstButtonMethodLookup;
            }
            else if (type == "Wizard") {
                lstMethod = $rootScope.LstButtonMethodWizard;
            }
            else if (type == "Maintenance" || type == "UserControl") {
                lstMethod = $rootScope.LstButtonMethodMaintenance;
            }

            if (lstMethod.length > 0) {
                //console.log(lstMethod);
                angular.forEach(lstMethod, function (objBtnMethod) {
                    var buttonDetails = {
                    };
                    var attr = null;
                    attr = objBtnMethod.Attribute;
                    buttonDetails.Method = objBtnMethod.Code;
                    buttonDetails.Description = objBtnMethod.Description;
                    buttonDetails.Category = objBtnMethod.Category;

                    $scope.ButtonsDetailsCollection.push(buttonDetails);
                });
            }
        }
    };
    $scope.LoadButtonDetails();
    //#endregion of create method list section

    $scope.FindEntityName = function (model, entityid, isChildOfGrid) {
        var entityName = entityid;
        if (model.dictAttributes.sfwParentGrid && model.dictAttributes.sfwEntityField) {
            var parentGrid = FindControlByID($scope.FormModel, model.dictAttributes.sfwParentGrid);
            var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.FormModel.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
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
            var objmodel = FindControlByID($scope.FormModel, model.dictAttributes.sfwRelatedGrid);
            if (objmodel && objmodel.dictAttributes.sfwEntityField) {
                entityName = getEntityName(objmodel.dictAttributes.sfwEntityField, entityid);
            }
        }
        return entityName;
    };

    //#region Functions for Clear Panel, Delete Row and Delete Column.

    $scope.clearCell = function (cell) {
        while (cell.Elements.length > 0) {
            $rootScope.DeleteItem(cell.Elements[0], cell.Elements);
        }
    };
    $scope.clearPanel = function () {
        if ($scope.FormModel.SelectedControl) {
            if ($scope.FormModel.dictAttributes.sfwType && $scope.FormModel.dictAttributes.sfwType.toLowerCase() == "lookup" && !$scope.FormModel.IsLookupCriteriaEnabled && !$scope.FormModel.IsPrototypeLookupCriteriaEnabled) {
                return;
            }
            if ($scope.FormModel.dictAttributes.sfwType === "Lookup") {
                var cellVM = FindParent($scope.FormModel.SelectedControl, "sfwColumn", true);
                if (cellVM && cellVM.Elements && cellVM.Elements.length > 0 && cellVM.Elements[0].Name === "sfwTabContainer") {
                    return;
                }
            }


            var table = null;
            if ($scope.FormModel.SelectedControl.Name === "sfwPanel") {
                table = $scope.FormModel.SelectedControl.Elements[0];
            }
            else {
                table = FindParent($scope.FormModel.SelectedControl, "sfwTable", true);
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
                if ($scope.FormModel.SelectedControl.Name !== "sfwPanel" && $scope.FormModel.SelectedControl.Name !== "sfwTable" && $scope.FormModel.SelectedControl.Name !== "sfwColumn") {
                    var parentCell = FindParent($scope.FormModel.SelectedControl, "sfwColumn", true);
                    if (parentCell) {
                        $scope.selectControl(parentCell);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    $scope.deleteRow = function () {
        if ($scope.FormModel.SelectedControl) {
            var row = FindParent($scope.FormModel.SelectedControl, "sfwRow", true);
            if (row && row.ParentVM && row.ParentVM.Elements) {
                if (row.ParentVM.Elements.length > 1) {

                    var rowIndex = row.ParentVM.Elements.indexOf(row);
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(row, row.ParentVM.Elements);

                    if (rowIndex === row.ParentVM.Elements.length) {
                        rowIndex--;
                    }

                    //sselecting the first cell of next row.
                    if (rowIndex > -1 && row.ParentVM.Elements.length > 0 && row.ParentVM.Elements[rowIndex].Elements.length > 0) {
                        $scope.selectControl(row.ParentVM.Elements[rowIndex].Elements[0]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    $SgMessagesService.Message("Message", "Atleast one row should be present");

                }
            }
        }
    };
    $scope.deleteColumn = function () {
        if ($scope.FormModel.SelectedControl) {
            var cell = FindParent($scope.FormModel.SelectedControl, "sfwColumn", true);
            if (cell && cell.Elements && cell.Elements.length > 0 && cell.Elements[0].Name === "sfwTabContainer") {
                return;
            }
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
                    if (colIndex > -1 && (cell.ParentVM.Elements.length > colIndex)) {
                        $scope.selectControl(cell.ParentVM.Elements[colIndex]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    $SgMessagesService.Message("Message", "Atleast one column should be present");
                }
            }
        }
    };


    //#endregion


    //#region Show controls according to selected type form Entity Tree
    $scope.showControlbasedOnType = function (controlName) {
        var isShow = false;
        var SelectedField = undefined;
        if ($scope.FormModel) {
            if (!$scope.FormModel.IsLookupCriteriaEnabled) {
                for (var i = 0; i < $scope.lstLoadedEntityTrees.length; i++) {
                    if ($scope.lstLoadedEntityTrees[i].IsVisible == true) {
                        SelectedField = $scope.lstLoadedEntityTrees[i].selectedobjecttreefield;
                        break;
                    }
                }
            } else if ($scope.FormModel.IsLookupCriteriaEnabled == true && (!$scope.MainQuery && (!$scope.SelectedQuery || ($scope.SelectedQuery && !$scope.SelectedQuery.dictAttributes.ID)))) {
                SelectedField = $scope.selectedobjecttreefield;
            } else {
                for (var i = 0; i < $scope.lstLoadedEntityColumnsTree.length; i++) {
                    if ($scope.lstLoadedEntityColumnsTree[i].IsVisible == true) {

                        SelectedField = $scope.selectedfield;
                        break;
                    }
                }
            }
        }


        if ($scope.FormModel && $scope.FormModel.dictAttributes && $scope.FormModel.dictAttributes.sfwType && SelectedField) {

            if (controlName == "sfwLabel" && ["Lookup", "Tooltip"].indexOf($scope.FormModel.dictAttributes.sfwType) === -1 && !$scope.FormModel.IsLookupCriteriaEnabled && ["Collection", "Object", "List", "CDOCollection"].indexOf(SelectedField.Type) == -1 && SelectedField.DataType && SelectedField.DataType.toLowerCase() != "datetime") {
                isShow = true;
            }
            else if (controlName == "sfwLabel" && $scope.FormModel.dictAttributes.sfwType === "Tooltip" && !$scope.FormModel.IsLookupCriteriaEnabled && ["Collection", "Object", "List", "CDOCollection"].indexOf(SelectedField.Type) == -1) {
                isShow = true;
            }
            else if ((controlName == "sfwTextBox" || controlName == "sfwRadioButtonList" || controlName == "sfwDropDownList" || controlName == "sfwMultiSelectDropDownList" || controlName == "sfwCheckBox") && $scope.FormModel.dictAttributes.sfwType != "Tooltip" && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) == -1 && SelectedField.DataType && SelectedField.DataType.toLowerCase() != "datetime") {
                isShow = true;
            }
            else if (controlName == "sfwCascadingDropDownList" && $scope.FormModel.dictAttributes.sfwType != "Tooltip" && !$scope.IsGridSeleected && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) == -1 && SelectedField.DataType && SelectedField.DataType.toLowerCase() != "datetime") {
                isShow = true;
            }

            else if (controlName == "sfwRadioButton" && ["Tooltip", "Lookup"].indexOf($scope.FormModel.dictAttributes.sfwType) == -1 && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) == -1 && SelectedField.DataType && SelectedField.DataType.toLowerCase() != "datetime") {
                isShow = true;
            }
            else if (controlName == "sfwCheckBoxList" && !$scope.IsGridSeleected && $scope.FormModel.dictAttributes.sfwType != 'Tooltip' && ["CDOCollection"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }
            else if ((controlName == "Grid" || controlName == "SfxChart") && !$scope.IsGridSeleected && $scope.FormModel.dictAttributes.sfwType != 'Lookup' && ["Collection", "List"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }
            else if (controlName == "UserControl" && !$scope.IsGridSeleected && ["Lookup", "UserControl", "Tooltip"].indexOf($scope.FormModel.dictAttributes.sfwType) == -1 && ["Object"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }

            else if (controlName == "sfwRange" && !$scope.IsGridSeleected && ["Lookup"].indexOf($scope.FormModel.dictAttributes.sfwType) > -1 && SelectedField.DataType && ["DateTime", "Int", "Decimal", "Double"].indexOf(SelectedField.DataType) > -1) {
                isShow = true;
            }

            else if (controlName == "sfwDateTimePicker" && ["Tooltip"].indexOf($scope.FormModel.dictAttributes.sfwType) == -1 && SelectedField.DataType && ["DateTime", "datetime"].indexOf(SelectedField.DataType) > -1) {
                isShow = true;
            }
            else if (controlName == "sfwCalendar" && !$scope.IsGridSeleected && ["Maintenance", "Wizard"].indexOf($scope.FormModel.dictAttributes.sfwType) > -1 && ["Collection", "List"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }
            else if ((controlName == "sfwListView") && ["Maintenance", "Wizard"].indexOf($scope.FormModel.dictAttributes.sfwType) > -1 && !$scope.IsGridSeleected && ["Collection", "List"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }
            else if ((controlName == "sfwJSONData") && ["Maintenance", "Wizard", "UserControl"].indexOf($scope.FormModel.dictAttributes.sfwType) > -1 && !$scope.IsGridSeleected && ["string", "String"].indexOf(SelectedField.DataType) > -1) {
                isShow = true;
            }
            else if (controlName == "sfwScheduler" && !$scope.IsGridSeleected && ["Maintenance", "Wizard"].indexOf($scope.FormModel.dictAttributes.sfwType) > -1 && ["Collection", "List"].indexOf(SelectedField.Type) > -1) {
                isShow = true;
            }
        }

        return isShow;
    };



    $scope.scrollBySelectedField = function (parentDiv, selectedElement, settings) {
        $timeout(function () {
            var $divDom = $(parentDiv);
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), settings, null);
                return false;
            }
        });
    }
    //#endregion


    //#region Centre Left Dialog
    $scope.OpenCentreLeftDialog = function () {
        var newScope = $scope.$new();
        newScope.formmodel = $scope.FormModel;
        if ($scope.CenterLeft) {
            newScope.CenterLeft = {};
            angular.copy($scope.CenterLeft, newScope.CenterLeft);
        }
        else {
            newScope.CenterLeft = { Name: 'centerleft', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        }


        dialog = $rootScope.showDialog(newScope, "Centre Left", "Form/views/CentreLeft.html", {
            width: 900, height: 600
        });

        newScope.onOkClick = function () {

            var lst = $scope.FormModel.Elements.filter(function (x) {
                return x.Name === "centerleft"
            });
            if (lst && lst.length > 0) {
                $rootScope.UndRedoBulkOp("Start");
                if ($scope.CenterLeft) {
                    $rootScope.EditPropertyValue($scope.CenterLeft.Elements, $scope.CenterLeft, "Elements", []);
                }

                if (newScope.CenterLeft && newScope.CenterLeft.Elements) {
                    for (var i = 0; i < newScope.CenterLeft.Elements.length > 0; i++) {
                        $rootScope.PushItem(newScope.CenterLeft.Elements[i], $scope.CenterLeft.Elements);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
            else {

                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(newScope.CenterLeft, $scope.FormModel.Elements);
                $rootScope.EditPropertyValue($scope.CenterLeft, $scope, "CenterLeft", newScope.CenterLeft);
                $rootScope.UndRedoBulkOp("End");
            }

            newScope.onCancelClick();
        };

        newScope.onCancelClick = function () {
            dialog.close();
        }

    };
    //#endregion

    $scope.showFormTreeMap = function () {
        $scope.viewTreeMap = !$scope.viewTreeMap;
        if ($scope.viewTreeMap) {
            $timeout(function () {
                $("#" + $scope.FormModel.dictAttributes.ID).find(".xml-control-tree").slideDown();
            });
        } else {
            $timeout(function () {
                $("#" + $scope.FormModel.dictAttributes.ID).find(".xml-control-tree").slideUp();
            });
        }
        $scope.TreeObject = {};
        $scope.TreeObject.btnLabel = "Expand All";
        $scope.TreeObject.formTableModel = $scope.formTableModel;
    };
    $scope.searchControlID = function (event) {
        $rootScope.$broadcast('searchById', { model: $scope.TreeObject.formTableModel, input: $scope.TreeObject.ID });
        if (event && event.keyCode == 13) {

        }
    };
    $scope.expandOrCollapseAll = function () {
        var isExpand = true;
        if ($scope.TreeObject.btnLabel == "Expand All") {
            isExpand = true;
            $scope.TreeObject.btnLabel = "Collapse All";
        } else if ($scope.TreeObject.btnLabel == "Collapse All") {
            isExpand = false;
            $scope.TreeObject.btnLabel = "Expand All"
        }
        expandOrCollpase($scope.formTableModel.Elements, isExpand);
    };
    var expandOrCollpase = function (controls, isExpand) {
        angular.forEach(controls, function (control) {
            control.isExpand = isExpand;
            if (control.Elements.length) {
                expandOrCollpase(control.Elements, isExpand);
            }
        });
    };
}]);