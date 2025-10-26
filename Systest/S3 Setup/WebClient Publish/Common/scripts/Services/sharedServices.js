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
    ]
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
            delete obj.errors.duplicate_id;
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
                    isshowcdocollection = true; isshowonetoone = false;
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
                    isshowonetoone = false; isshowcdocollection = true;
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