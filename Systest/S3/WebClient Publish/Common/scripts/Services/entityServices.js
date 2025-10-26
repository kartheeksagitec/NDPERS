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
