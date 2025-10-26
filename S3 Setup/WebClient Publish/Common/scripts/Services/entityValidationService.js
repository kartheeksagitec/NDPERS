app.factory("$entityValidationService", ["$filter", "$Errors", "$ValidationService", "CONSTANTS", "$Entityintellisenseservice", "$rootScope", function ($filter, $Errors, $ValidationService, CONST, $Entityintellisenseservice, $rootScope) {
    var lobjFactory = {};
    var lobjData = {
        validationErrorList: [], warningList: [], objEntity: null, blnShowCreateWithClass: true,
        objInheritedRules: {}, PartialFileData: [], codegrouplist: [], lstMessages: [], lstBusBaseMethods: [], PartialFileDataTypes: [],
        lstBusinessObjectForProperty: [], lstMethods: []
    };

    lobjFactory.validateFileData = function (aobjEntity, aobjScope) {
        lobjData.objEntity = aobjEntity;
        lobjData.objInheritedRules = {};
        lobjData.PartialFileData = [];
        lobjData.codegrouplist = [];
        lobjData.lstMessages = [];
        lobjData.lstBusBaseMethods = [];
        lobjData.PartialFileDataTypes = [];
        lobjData.lstBusinessObjectForProperty = [];
        lobjData.lstMethods = [];
        if (aobjEntity.dictAttributes.CreateWithClass && aobjEntity.dictAttributes.CreateWithClass.toLowerCase() == "false") {
            lobjData.blnShowCreateWithClass = false;
        }
        lobjData.objEntity.errors = {};
        //var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: aobjEntity.dictAttributes.ID })[0];

        //lobjData.validationErrorList = fileErrObj.errorList = [];
        //lobjData.warningList = fileErrObj.warningList = [];

        var fileErrObj;
        if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
            var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: aobjEntity.dictAttributes.ID });
            if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: aobjEntity.dictAttributes.ID, errorList: [] });
            fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: aobjEntity.dictAttributes.ID })[0];
        }
        lobjData.validationErrorList = fileErrObj.errorList = [];
        lobjData.warningList = fileErrObj.warningList = [];

        getValidationData(aobjEntity, aobjScope);
    };

    var validateEntityFile = function () {
        /** attribute validation **/
        var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
        angular.forEach(lobjAttributes.Elements, function (obj) {
            validateId(obj, undefined, true);
            validateDataType(obj, undefined, true);
            validateEntity(obj);
        });
        /** Query validation **/
        var lobjQueries = getElementData(lobjData.objEntity, "queries");
        angular.forEach(lobjQueries.Elements, function (obj) {
            $ValidationService.validateID(obj, lobjData.validationErrorList, obj.dictAttributes.ID);
            if (obj.Elements.length > 0) {
                var customMapping = $filter('filter')(obj.Elements, { Name: "mappedcolumns" })[0];
                if (customMapping && customMapping.Elements.length > 0) {
                    angular.forEach(customMapping.Elements, function (columnObj) {
                        if (columnObj && columnObj.dictAttributes.hasOwnProperty("sfwEntityField") && columnObj.dictAttributes.sfwEntityField) {
                            var attrType = "";
                            $ValidationService.checkValidListValueForMultilevel([], columnObj, columnObj.dictAttributes.sfwEntityField, lobjData.objEntity.dictAttributes.ID, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjData.validationErrorList, false, attrType);
                        }
                    });
                }
            }
        });
        /** Checking dupplicate ID in entire entity excluding Methods **/
        checkDuplicateId(false);
        /** Methods validation **/
        validateQueries(false);
        validateValidationRulesID(false);
        var lobjMethods = getElementData(lobjData.objEntity, "methods");
        angular.forEach(lobjMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, lobjData.validationErrorList, obj.dictAttributes.ID);
        });

        /** Object Methods Validation **/
        if (lobjData.objEntity.dictAttributes.sfwObjectID) {

            var methodList = lobjData.lstMethods && $ValidationService.getListByPropertyName(lobjData.lstMethods, "ShortName");
            validateBusinessObjectMethod(methodList);


        } else if (lobjData.blnShowCreateWithClass) {
            validateBusinessObjectMethod([]);
        }

        var lobjHardError = getElementData(lobjData.objEntity, "harderror");
        angular.forEach(lobjHardError.Elements, function (obj) {
            if (obj.objRule) {
                $ValidationService.validateIsEmptyMessageId(obj.objRule, lobjData.validationErrorList, obj.objRule.dictAttributes.sfwMessageId);
                $ValidationService.validateIsEmptyMessageIdAndDescription(obj.objRule, lobjData.validationErrorList, obj.objRule.dictAttributes.sfwMessageId, obj.objRule.dictAttributes.sfwMessage);
            }
        });
        var lobjSoftError = getElementData(lobjData.objEntity, "softerror");
        angular.forEach(lobjSoftError.Elements, function (obj) {
            if (obj.objRule) {
                $ValidationService.validateIsEmptyMessageId(obj.objRule, lobjData.validationErrorList, obj.objRule.dictAttributes.sfwMessageId);
                $ValidationService.validateIsEmptyMessageIdAndDescription(obj.objRule, lobjData.validationErrorList, obj.objRule.dictAttributes.sfwMessageId, obj.objRule.dictAttributes.sfwMessage);
            }
        });
        /* Validate Delete Method section */
        var lobjDelete = getElementData(lobjData.objEntity, "delete");
        angular.forEach(lobjDelete.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("ID") && obj.dictAttributes.ID) {
                var attrType = 'Object,Collection,List';
                $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.ID, lobjData.objEntity.dictAttributes.ID, "ID", "ID", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, lobjData.validationErrorList, false, attrType);
            }
            if (obj.dictAttributes.hasOwnProperty("sfwMethodName") && obj.dictAttributes.sfwMethodName) {
                var attrType = 'Property,Collection,Object';
                $ValidationService.checkValidListValueForXMLMethod([], obj, obj.dictAttributes.sfwMethodName, lobjData.objEntity.dictAttributes.ID, "sfwMethodName", "invalid_method_name", CONST.VALIDATION.INVALID_NAME, lobjData.validationErrorList, attrType, null, true);
            }
        });
        /* validate code id in column attributes */
        if (lobjData.codegrouplist.length > 0) {
            var list = $ValidationService.getListByPropertyName(lobjData.codegrouplist, "CodeID");
            validateCodeGroup(list);
        }

        /** Checking duplicate ID of methods for object and XML methods **/
        checkDuplicateIdForMethod(false);

        validateCSProperties();
        // check if inherited validation rule used in initial load,hard error, soft error,validate delete,group rules,check list and is it present in parent entity rules

        if (lobjData.objEntity.dictAttributes.sfwParentEntity) {
            if (lobjData.objInheritedRules) {
                validateValidationRulesID(true);
                checkValidationRules(lobjData.objInheritedRules.Elements);
            }
        } else {
            var list = [];
            if (lobjData.objEntity.dictAttributes.sfwParentEntity && lobjData.objInheritedRules && lobjData.objInheritedRules.Elements) {
                list = lobjData.objInheritedRules.Elements;
            }
            checkValidationRules(list);
        }
        checkObjectFieldValue();
        validateMessageIds();
        if (true) {
            ValidateObject();
            validateXMLMethod();
            validateRuleMessageId();
        }

        //below function calls are to validate parent key value if it's empty and error table is set.
        validateParentKeyValue();

        //below function calls are to validate status value if it's empty and error table is set.
        validateStatusValue();

        console.dir($Errors);
    };

    var getElementData = function (aobjEntity, astrNodeName) {
        var lobjNode = null;
        for (var i = 0; i < aobjEntity.Elements.length; i++) {
            if (aobjEntity.Elements[i].Name == astrNodeName) {
                lobjNode = aobjEntity.Elements[i];
                break;
            }
        }

        if (!lobjNode) {
            lobjNode = CreateNewObject(astrNodeName, aobjEntity);
        }
        return lobjNode;
    }
    var getChildNodes = function (aobjData, astrParentName, astrChildName) {
        var lobjChildNodes = getElementData(aobjData, astrParentName);
        var lobjChildNode = getElementData(lobjChildNodes, astrChildName);
        return lobjChildNode;
    }
    var CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };
    var validateId = function (obj, ID, isNotFirstLoad) {
        if (obj) {
            if (isNotFirstLoad) {
                $ValidationService.validateID(obj, lobjData.validationErrorList, obj.dictAttributes.ID);
            } else {
                $ValidationService.validateID(obj, lobjData.validationErrorList, ID);
            }
        }
    };

    var validateDataType = function (obj, dataType, isFirstLoad) {
        if (obj) {
            if (isFirstLoad) {
                if (obj.dictAttributes.sfwType == "Property") {
                    $ValidationService.validateDataType(obj, lobjData.validationErrorList, obj.dictAttributes.sfwDataType);
                }
            } else {
                $ValidationService.validateDataType(obj, lobjData.validationErrorList, dataType);
            }
        }
    };
    var validateEntity = function (obj) {
        if (obj && (obj.dictAttributes.sfwType == "Collection" || obj.dictAttributes.sfwType == "CDOCollection" || obj.dictAttributes.sfwType == "Object" || obj.dictAttributes.sfwType == "List")) {
            $ValidationService.validateEntity(obj, lobjData.validationErrorList);
        }
    };
    var checkDuplicateId = function (isNotFirstLoad) {
        var attributeModel = getAttributesList();
        var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
        angular.forEach(lobjAttributes.Elements, function (obj) {
            if (obj.dictAttributes.sfwType !== "Description") {
                $ValidationService.checkDuplicateId(obj, attributeModel, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES, lobjData.warningList, $rootScope.iblnShowEntityDuplicateIdAsWarning, true);
            }
        });
    };
    var validateQueries = function (isNotFirstLoad) {
        var lobjQueries = getElementData(lobjData.objEntity, "queries");
        angular.forEach(lobjQueries.Elements, function (obj) {
            $ValidationService.checkDuplicateId(obj, lobjQueries, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES);
        });
    };
    var validateValidationRulesID = function (isNotFirstLoad) {
        var objParentRules = null;
        if (lobjData.objInheritedRules && lobjData.objInheritedRules.Elements) {
            objParentRules = lobjData.objInheritedRules;
        }
        var ruleModel = getRuleModel(objParentRules);
        var lobjRules = getElementData(lobjData.objEntity, "rules");
        if (lobjRules) {
            angular.forEach(lobjRules.Elements, function (obj) {
                if (obj.Name == "rule") { // only validating type of Rules
                    $ValidationService.checkDuplicateId(obj, ruleModel, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES);
                }
            });
        }
    };

    var validateBusinessObjectMethod = function (methodList) {
        var lobjObjectMethods = getElementData(lobjData.objEntity, "objectmethods");
        angular.forEach(lobjObjectMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, lobjData.validationErrorList, obj.dictAttributes.ID);
            $ValidationService.checkValidListValue(methodList, obj, obj.dictAttributes.ID, "ID", "inValid_id", CONST.VALIDATION.INVALID_FIELD, lobjData.validationErrorList);
        });
    };

    var validateCodeGroup = function (list) {
        var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
        angular.forEach(lobjAttributes.Elements, function (obj) {
            if (obj.dictAttributes.hasOwnProperty("sfwCodeID") && obj.dictAttributes.sfwCodeID) {
                $ValidationService.checkValidListValue(list, obj, obj.dictAttributes.sfwCodeID, "sfwCodeID", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, lobjData.validationErrorList);
            }
        });
    };

    var checkDuplicateIdForMethod = function (isNotFirstLoad) {
        var xmlMethodModel, objMethodModel;
        xmlMethodModel = getMethodsModel("xmlmethod");
        var lobjMethods = getElementData(lobjData.objEntity, "methods");
        angular.forEach(lobjMethods.Elements, function (obj) {
            $ValidationService.validateID(obj, lobjData.validationErrorList, obj.dictAttributes.ID);
            $ValidationService.checkDuplicateId(obj, xmlMethodModel, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.METHOD_NODES);
        });
        if (lobjData.blnShowCreateWithClass) {
            objMethodModel = getMethodsModel("objmethod");
            var lobjObjectMethods = getElementData(lobjData.objEntity, "objectmethods");
            angular.forEach(lobjObjectMethods.Elements, function (obj) {
                $ValidationService.checkDuplicateId(obj, objMethodModel, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.METHOD_NODES);
            });
        }
    };
    var validateCSProperties = function () {
        if (lobjData.PartialFileData && angular.isArray(lobjData.PartialFileData)) {
            for (var index = 0; index < lobjData.PartialFileData.length; index++) {
                validateCSProperty(lobjData.PartialFileData[index]);
            }
        }
    };
    var validateValidationRulesID = function (isNotFirstLoad) {
        var objParentRules = null;
        if (lobjData.objInheritedRules && lobjData.objInheritedRules.Elements) {
            objParentRules = lobjData.objInheritedRules;
        }
        var ruleModel = getRuleModel(objParentRules);
        var lobjRules = getElementData(lobjData.objEntity, "rules");
        if (lobjRules) {
            angular.forEach(lobjRules.Elements, function (obj) {
                if (obj.Name == "rule") { // only validating type of Rules
                    $ValidationService.checkDuplicateId(obj, ruleModel, lobjData.validationErrorList, isNotFirstLoad, CONST.ENTITY.NODES);
                }
            });
        }
    };
    var checkValidationRules = function (parentEntityRules) {
        var ruleList = [];
        var lobjRules = getElementData(lobjData.objEntity, "rules");
        ruleList = ruleList.concat($ValidationService.getListByPropertyName(lobjRules.Elements, "ID", false));
        ruleList = ruleList.concat($ValidationService.getListByPropertyName(parentEntityRules, "ID", false));
        // validate initial load
        var lobjInitialLoad = getElementData(lobjData.objEntity, "initialload");
        validateRules(lobjInitialLoad.Elements, ruleList);

        // validate check list
        var lobjCheckList = getElementData(lobjData.objEntity, "checklist");
        validateRules(lobjCheckList.Elements, ruleList);
    };
    var checkObjectFieldValue = function () {
        var attrType = ["Object", "CDOCollection", "Collection", "List", "Property"];
        var attributeModel = getAttributesList();
        var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
        angular.forEach(lobjAttributes.Elements, function (aobj) {
            if (angular.isArray(attrType) && attrType.indexOf(aobj.dictAttributes.sfwType) > -1) {
                if (aobj && aobj.warnings) {
                    delete aobj.warnings["sfwValue"];
                }
                $ValidationService.checkDuplicateValue(aobj, attributeModel, lobjData.warningList, "sfwValue", "sfwValue", true, attrType);
            }
        });
    }
    var validateMessageIds = function () {
        var list = $ValidationService.getListByPropertyName(lobjData.lstMessages, "MessageID");
        var lobjConstraints = getElementData(lobjData.objEntity, "constraint");
        if (lobjConstraints && lobjConstraints.Elements.length > 0) {
            angular.forEach(lobjConstraints.Elements, function (constraint) {
                if (constraint && constraint.dictAttributes.sfwMaxValue && constraint.dictAttributes.sfwMaxMessageId) {
                    $ValidationService.checkValidListValue(list, constraint, constraint.dictAttributes.sfwMaxMessageId, "sfwMaxMessageId", "sfwMaxMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjData.validationErrorList, false);
                }
                if (constraint && constraint.dictAttributes.sfwMinValue && constraint.dictAttributes.sfwMinMessageId) {
                    $ValidationService.checkValidListValue(list, constraint, constraint.dictAttributes.sfwMinMessageId, "sfwMinMessageId", "sfwMinMessageId", CONST.VALIDATION.INVALID_MESSAGE_ID, lobjData.validationErrorList, false);
                }
            });
        }

    };
    var ValidateObject = function () {
        ValidateRulesandExpressions();
    };
    var validateXMLMethod = function () {
        var lobjMethods = getElementData(lobjData.objEntity, "methods");
        angular.forEach(lobjMethods.Elements, function (obj) {
            for (var i = 0; i < obj.Elements.length; i++) {
                if (obj.Elements[i].Name == "item") {
                    var attrType = 'Column,Property,Collection,Object';
                    $ValidationService.checkValidListValueForXMLMethod(lobjData.lstBusBaseMethods, obj.Elements[i], obj.Elements[i].dictAttributes.ID, lobjData.objEntity.dictAttributes.ID, "ID", "invalid_method_name", CONST.VALIDATION.INVALID_NAME, lobjData.validationErrorList, attrType, null, false);
                }
            }
        });
    };
    var validateRuleMessageId = function () {
        var lobjRules = getElementData(lobjData.objEntity, "rules");
        if (lobjRules) {
            var lobjHardError = getElementData(lobjData.objEntity, "harderror");
            angular.forEach(lobjRules.Elements, function (obj) {
                if (obj.Name == "rule") { // only validating type of Rules
                    var objHardError = lobjHardError.Elements.filter(function (hardErrorObj) {
                        if (hardErrorObj && hardErrorObj.dictAttributes.ID == obj.dictAttributes.ID) {
                            return hardErrorObj;
                        }
                    })
                    if (objHardError && objHardError.length) {
                        validateEmptyMessageID(obj);
                    }
                }
            });
        }
    };
    var validateParentKeyValue = function () {
        if (lobjData.objEntity.dictAttributes.sfwErrorTable && !(lobjData.objEntity.dictAttributes.sfwParentKeyValue)) {
            $ValidationService.checkValidListValue([], lobjData.objEntity, lobjData.objEntity.dictAttributes.sfwParentKeyValue, "sfwParentKeyValue", "sfwParentKeyValue", "Parent Key Value for Error Table cannot be empty.", lobjData.validationErrorList, true);
        }
        else if (lobjData.objEntity.dictAttributes.sfwErrorTable && lobjData.objEntity.dictAttributes.sfwParentKeyValue) {
            if (lobjData.objEntity.errors && lobjData.objEntity.errors.sfwParentKeyValue) {
                lobjData.objEntity.errors.sfwParentKeyValue = undefined;
            }
        } else if (!(lobjData.objEntity.dictAttributes.sfwErrorTable) && lobjData.objEntity.errors) {
            lobjData.objEntity.errors.sfwParentKeyValue = undefined;
        }
    }
    var validateStatusValue = function () {

        if (lobjData.objEntity.dictAttributes.sfwErrorTable && !(lobjData.objEntity.dictAttributes.sfwStatusColumn)) {
            $ValidationService.checkValidListValue([], lobjData.objEntity, lobjData.objEntity.dictAttributes.sfwStatusColumn, "sfwStatusColumn", "sfwStatusColumn", "Status Column for Error Table cannot be empty.", lobjData.validationErrorList, true);
        }
        else if (lobjData.objEntity.dictAttributes.sfwErrorTable && lobjData.objEntity.dictAttributes.sfwStatusColumn) {
            if (lobjData.objEntity.dictAttributes.errors && lobjData.objEntity.dictAttributes.errors.sfwStatusColumn) {
                lobjData.objEntity.dictAttributes.errors.sfwStatusColumn = undefined;
            }
        } else if (!(lobjData.objEntity.dictAttributes.sfwErrorTable) && lobjData.objEntity.dictAttributes.errors) {
            lobjData.objEntity.dictAttributes.errors.sfwStatusColumn = undefined;
        }
    }
    var getRuleModel = function (inheritedRules) {
        var ruleModel = { Elements: [] };
        var lstRules = [];
        var lobjRules = getElementData(lobjData.objEntity, "rules");
        ruleModel.Elements = lobjRules.Elements && lobjRules.Elements.slice();
        if (lobjData.objEntity.dictAttributes.sfwParentEntity && inheritedRules && inheritedRules.Elements) {
            angular.forEach(inheritedRules.Elements, function (rule) {
                if (rule) {
                    var tempRule = {};
                    tempRule = angular.copy(rule, tempRule);
                    tempRule.isParent = true; // added property for identify parent entity rule
                    lstRules.push(tempRule);
                }
            });
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
            var lobjMethods = getElementData(lobjData.objEntity, "methods");
            xmlMethodModel.Elements = lobjMethods.Elements.slice();
            isObjMethod = false;
        } else {
            var lobjObjectMethods = getElementData(lobjData.objEntity, "objectmethods");
            xmlMethodModel.Elements = lobjObjectMethods.Elements.slice();
            isXmlMethod = false;
        }

        if (lobjData.objEntity.dictAttributes.sfwParentEntity) {
            var xmlMethodData = $Entityintellisenseservice.GetIntellisenseData(lobjData.objEntity.dictAttributes.sfwParentEntity, "", "", true, false, isObjMethod, false, false, isXmlMethod);
            if (xmlMethodData && xmlMethodData.length > 0) {
                lstXmlMethods = convertToBaseModel(xmlMethodData);
                xmlMethodModel.Elements = xmlMethodModel.Elements.concat(lstXmlMethods);
            }
        }
        return xmlMethodModel;

    };
    var validateCSProperty = function (obj) {
        validateCSPropertyDataType(obj);
        validateCSPropertyClass(obj);
        validateCSPropertyPropertyName(obj);
    };
    var validateCSPropertyDataType = function (obj) {
        obj.DataTypeError = "";
        if (!(obj.DataType && obj.DataType.trim().length > 0)) {
            obj.DataTypeError = "Enter Data Type.";
        }
        else if (lobjData.PartialFileDataTypes.indexOf(obj.DataType) == -1) {
            obj.DataTypeError = "Invalid Data Type.";
        }
        setIsValid(obj);
    };
    var validateCSPropertyClass = function (obj) {
        obj.ClassNameError = "";
        if (obj.DataType && (obj.DataType.toLowerCase() == "object" || obj.DataType.toLowerCase() == "collection" || obj.DataType.toLowerCase() == "cdocollection" || obj.DataType.toLowerCase() == "list")) {
            if (!(obj.Class && obj.Class.trim().length > 0)) {
                obj.ClassNameError = "Enter Class Name.";
            }
            else if (lobjData.lstBusinessObjectForProperty && !lobjData.lstBusinessObjectForProperty.some(function (x) { return x.ID == obj.Class; })) {
                obj.ClassNameError = "Invalid Class Name.";
            }
        }
        setIsValid(obj);
    };
    var validateCSPropertyPropertyName = function (obj) {
        obj.PropertyNameError = "";
        if (!(obj.PropertyName && obj.PropertyName.trim().length > 0)) {
            obj.PropertyNameError = "Enter Property Name.";
        }
        else if (!isValidIdentifier(obj.PropertyID)) {
            obj.PropertyNameError = "Invalid Property Name.";
        }
        else if (lobjData.PartialFileData.filter(function (x) { return x.PropertyID == obj.PropertyID; }).length > 1) {
            obj.PropertyNameError = "Duplicate Property ID.";
        }
        setIsValid(obj);
    };
    var validateRules = function (objList, ruleList) {
        angular.forEach(objList, function (obj) {
            $ValidationService.checkValidListValue(ruleList, obj, obj.dictAttributes.ID, "ID", "invalid_rule", obj.dictAttributes.ID + " - " + CONST.VALIDATION.INVALID_RULE, lobjData.validationErrorList);

            if (angular.isArray(obj.Children) && obj.Children.length > 0) {
                validateRules(obj.Children, ruleList);
            }
        });
    };
    var getAttributesList = function () {
        var attributeModel = { Elements: [] };
        var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
        attributeModel.Elements = lobjAttributes.Elements && lobjAttributes.Elements.slice();
        if (lobjData.objEntity.dictAttributes.sfwParentEntity) {
            var data = $Entityintellisenseservice.GetIntellisenseData(lobjData.objEntity.dictAttributes.sfwParentEntity, "", "", true, true, false, false, false, false);
            if (data && data.length > 0) {
                attributeModel.Elements = attributeModel.Elements.concat(convertToBaseModel(data));
            }
        }
        return attributeModel;
    }
    var ValidateRulesandExpressions = function () {
        $.connection.hubEntityModel.server.validateExpressions(lobjData.objEntity.dictAttributes.ID, true).done(function (data) {
            var lobjAttributes = getElementData(lobjData.objEntity, "attributes");
            angular.forEach(data, function (value, key) {

                var lst = lobjAttributes.Elements.filter(function (itm) {
                    if (itm.dictAttributes.sfwType == 'Expression') {
                        return itm.dictAttributes.ID == key;
                    }
                });
                if (lst && lst.length > 0) {

                    lst[0].Error = value;

                }
            });
        });

        $.connection.hubEntityModel.server.validateExpressions(lobjData.objEntity.dictAttributes.ID, false).done(function (data) {
            var lobjRules = getElementData(lobjData.objEntity, "rules");
            angular.forEach(data, function (value, key) {

                var lst = lobjRules.Elements.filter(function (itm) {
                    return itm.dictAttributes.ID == key;
                });
                if (lst && lst.length > 0) {

                    lst[0].Error = value;

                }
            });
        });

    };

    var validateEmptyMessageID = function (SelectedRule) {
        if (SelectedRule) {
            var list = $ValidationService.getListByPropertyName(lobjData.lstMessages, "MessageID");
            list.unshift("0"); // deafult message id
            $ValidationService.checkValidListValue(list, SelectedRule, SelectedRule.dictAttributes.sfwMessageId, "sfwMessageId", "sfwMessageId", CONST.VALIDATION.MESSAGE_ID_EMPTY, lobjData.validationErrorList, true);
            if (SelectedRule.dictAttributes.sfwMessage) {
                delete SelectedRule.errors.sfwMessageId;
                if (SelectedRule && SelectedRule.errors && !$ValidationService.isEmptyObj(SelectedRule.errors)) {
                    $ValidationService.removeObjInToArray(lobjData.validationErrorList, SelectedRule);
                }
            }

        }
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
    var setIsValid = function (obj) {
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

    var getValidationData = function (aobjEntity, aobjScope) {
        hubMain.server.getEntityValidationData(lobjData.objEntity.dictAttributes.ID, lobjData.objEntity.dictAttributes.sfwParentEntity).done(function (data) {
            if (data) {
                lobjData.codegrouplist = data.codegroup;
                lobjData.objInheritedRules = data.inheritedRule;
                lobjData.lstMessages = data.messages;
                lobjData.lstBusinessObjectForProperty = data.businessObjectProperty;
            }
            $.connection.hubEntityModel.server.getObjectDataForValidation(lobjData.objEntity.dictAttributes.sfwObjectID).done(function (aChildData) {
                lobjData.lstBusBaseMethods = aChildData.busBaseMethods;
                lobjData.lstMethods = aChildData.objectTree ? aChildData.objectTree.lstMethods : [];
                lobjData.PartialFileData = aChildData.partialFileData ? aChildData.partialFileData[0] : {};
                lobjData.PartialFileDataTypes = aChildData.partialFileData ? aChildData.partialFileData[1] : [];
                validateEntityFile();
                if (aobjScope && aobjScope.validateFile) {
                    aobjScope.$evalAsync(function () {
                        aobjScope.validateFile("Completed");
                    });
                }
            }).fail(function () {
                if (aobjScope && aobjScope.validateFile) {
                    aobjScope.$evalAsync(function () {
                        aobjScope.validateFile("Failed");
                    });
                }
            });
        }).fail(function () {
            if (aobjScope && aobjScope.validateFile) {
                aobjScope.$evalAsync(function () {
                    aobjScope.validateFile("Failed");
                });
            }
        });
    };
    return lobjFactory;
}]);