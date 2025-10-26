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