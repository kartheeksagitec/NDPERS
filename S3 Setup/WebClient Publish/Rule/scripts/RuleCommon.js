app.controller('RuleController', ["$scope", "$rootScope", "ngDialog", "$resourceFactory", "$EntityIntellisenseFactory", "$timeout", "$DashboardFactory", "$cookies", "$NavigateToFileService", "ConfigurationFactory", "$interval", "$ValidationService", function ($scope, $rootScope, ngDialog, $resourceFactory, $EntityIntellisenseFactory, $timeout, $DashboardFactory, $cookies, $NavigateToFileService, ConfigurationFactory, $interval, $ValidationService) {

    //#region Variabales
    $rootScope.IsLoading = true;
    $scope.loadedRules = [];
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.isDirty = false;
    $scope.customTypes = [];
    $scope.navigateToFileService = $NavigateToFileService;
    $scope.isDescriptionText = false;
    $scope.isStatusOption = false;
    $scope.CheckParameterVisible = false;
    $scope.objLogicalRule = null;

    $scope.IsParameterVisible = false;
    $scope.IsLocalVariableVisible = false;
    $scope.IsParentRulesVisible = false;
    $scope.IsChildRulesVisible = false;
    $scope.selectedEffectiveDate = undefined;
    $scope.selectedDesignSource = false;
    $scope.activeLeftTab = "Parameters";
    $scope.ShowMethod = true;
    $scope.blnIsEntityObjectBased = true;
    $scope.isExcelMatrix = false;
    $scope.isDecisionTable = false;
    $scope.isLogicalRule = false;
    $scope.hasDefaultVersion = false;

    $scope.IsToolsDivCollapsed = true;

    $scope.lstDataFormat = ['{0:(###) ###-####} ', '{0:d}', '{0:###-##-####}', '{0:$#,###,##0.00;($#,###,##0.00)}'];
    //#endregion

    //#region On Load
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiverulemodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }
    });

    $scope.LoadUIforDesiredRule = function () {
        if ($scope.objLogicalRule) {
            if ($scope.objLogicalRule.dictAttributes) {
                if ($scope.objLogicalRule.dictAttributes.sfwRuleType == "LogicalRule") {
                    $scope.isLogicalRule = true;
                }
                else if ($scope.objLogicalRule.dictAttributes.sfwRuleType == "ExcelMatrix") {
                    $scope.isExcelMatrix = true;
                }
            }
            else if ($scope.objLogicalRule.RuleType == "DecisionTable") {
                $scope.isDecisionTable = true;
            }
        }
    };

    $scope.LoadRuleStatus = {
        repeatSelect: null,
        StatusValue: [
            {
                id: '1', name: 'Active'
            },
            { id: '2', name: 'Inactive' }
        ],
    };

    $scope.LoadRuleReturnTypes = {
        repeatSelect: null,
        ReturnValue: [
            {
                id: '0', name: ''
            },
            {
                id: '1', name: 'bool'
            },
            {
                id: '2', name: 'datetime'
            },
            {
                id: '3', name: 'decimal'
            },
            {
                id: '4', name: 'float'
            },
            {
                id: '5', name: 'double'
            },
            {
                id: '6', name: 'int'
            },
            {
                id: '7', name: 'long'
            },
            {
                id: '8', name: 'short'
            },
            {
                id: '9', name: 'string'
            },
            {
                id: '10', name: 'Collection'
            },
            {
                id: '11', name: 'Object'
            },
            { id: '12', name: 'List' }
        ],
    };

    $scope.LoadRuleReturnTypesForDecisionTable = {
        repeatSelect: null,
        ReturnValue: [
            {
                id: '0', name: ''
            },
            {
                id: '1', name: 'bool'
            },
            {
                id: '2', name: 'datetime'
            },
            {
                id: '3', name: 'decimal'
            },
            {
                id: '4', name: 'float'
            },
            {
                id: '5', name: 'double'
            },
            {
                id: '6', name: 'int'
            },
            {
                id: '7', name: 'long'
            },
            {
                id: '8', name: 'short'
            },
            {
                id: '9', name: 'string'
            }

        ],
    };

    $scope.receiverulemodel = function (data) {
        $scope.$evalAsync(function () {
            $scope.objLogicalRule = data;
            $scope.ruleID = "";
            $scope.loadedRules = [];
            $scope.objSelectedLogicalRule = undefined;
            $scope.objEffectiveDate = [];
            $scope.objstatus = ['', 'Active', 'Inactive'];
            $scope.status = $scope.objstatus[0];
            $scope.IsDecisionTable = false;
            $scope.effectiveDate = $scope.objEffectiveDate[0];
            $scope.lstViewModel = ['Developer View', 'Analyst View'];
            $scope.ViewMode = "Developer View";
            $scope.objRuleExtraFields = undefined;
            if ($scope.currentfile.FileType == "LogicalRule") {
                $scope.activeLeftTab = "Toolbox";
            }
            $scope.LoadUIforDesiredRule();
            if ($scope.currentfile.FileType != "DecisionTable") {
                $scope.ruleID = $scope.objLogicalRule.dictAttributes.ID;
                angular.forEach($scope.objLogicalRule.Elements, function (value, key) {
                    if (value.Name == "logicalrule") {
                        var effDate = value.dictAttributes.sfwEffectiveDate;
                        if (effDate && effDate.trim().length > 0) {
                            $scope.objEffectiveDate.push(effDate);
                        }
                        else {
                            $scope.objEffectiveDate.push("Default");
                        }
                    }
                    else if (value.Name == "ExtraFields") {
                        $scope.objRuleExtraFields = value;
                    }
                });
                //$scope.removeExtraFieldsDataInToMainModel();
            }
            else {
                $scope.IsDecisionTable = true;
                $scope.ruleID = $scope.objLogicalRule.RuleID;
                var tempExtraFields = $scope.objLogicalRule.ExtraFields;
                if (tempExtraFields && tempExtraFields.length > 0) {
                    $scope.objRuleExtraFields = {
                        Name: "ExtraFields", Value: '', dictAttributes: {
                        }, Elements: []
                    };
                    for (var i = 0; i < tempExtraFields.length; i++) {
                        var objChild = {
                            Name: "ExtraField", Value: '', dictAttributes: {
                            }, Elements: []
                        };
                        objChild.dictAttributes.ID = tempExtraFields[i].ID;
                        objChild.dictAttributes.Value = tempExtraFields[i].Value;
                        objChild.dictAttributes.URL = tempExtraFields[i].URL;
                        $scope.objRuleExtraFields.Elements.push(objChild);
                    }
                    //$scope.removeExtraFieldsDataInToMainModel();

                }

                angular.forEach($scope.objLogicalRule.Rules, function (value, key) {
                    var effDate = value.EffectiveDate;
                    if (effDate == undefined || effDate == "") {
                        $scope.objEffectiveDate.push("Default");
                    }
                    else {
                        $scope.objEffectiveDate.push(effDate);
                    }

                });
            }
            $scope.hasDefaultVersion = $scope.objEffectiveDate.indexOf("Default") > -1;
            if (!$scope.objRuleExtraFields) {
                $scope.objRuleExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {
                    }, Elements: []
                };
            }

            $scope.sortEffectiveDates();
            var currentEffectiveDate = getCurrentEffectiveDate($scope.objEffectiveDate);
            if (currentEffectiveDate) {
                $scope.selectRuleAndScrollToEffectiveDate(currentEffectiveDate);
            }
            else {
                $scope.selectRuleAndScrollToEffectiveDate("Default");
            }




            //#region local variables and parameters

            $scope.variables = GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', $scope.IsDecisionTable);
            $scope.parameters = GetVariablesAndParams($scope.objLogicalRule, 'parameters', $scope.IsDecisionTable);

            if ($scope.variables != undefined && $scope.variables.Elements.length > 0) {
                angular.forEach($scope.variables.Elements, function (value, key) {
                    value.IsEditLocalVar = false;
                });
                $scope.SelectedLocalVariable = $scope.variables.Elements[0];
            }

            if ($scope.IsDecisionTable) {
                if ($scope.parameters != undefined && $scope.parameters.length > 0) {
                    angular.forEach($scope.parameters, function (value, key) {
                        value.IsEditParameter = false;
                    });
                    $scope.SelectedParameter = $scope.parameters[0];
                }

                if ($scope.objSelectedLogicalRule && $scope.objSelectedLogicalRule.Rows && $scope.objSelectedLogicalRule.Rows.length > 0) {
                    var objRow = $scope.objSelectedLogicalRule.Rows[0];
                    if (objRow && objRow.Cells && objRow.Cells.length > 0) {
                        objRow.Cells[0].Item.IsSelected = true;
                        $scope.selectedDecisionTable = objRow.Cells[0];
                    }
                }

                var scope = getScopeByFileName("MainPage");
                if (scope.ErrorNode) {
                    var nodeid = scope.ErrorNode.istrNodeID;
                    for (var i = 0; i < $scope.objLogicalRule.Rules.length; i++) {
                        scope.getCurrentErrorStepNodeForDecisionTable(nodeid, $scope.objLogicalRule.Rules[i]);
                    }
                    var Currenterrornode = scope.currenterrornode;
                    $scope.objStepSelectionChanged(Currenterrornode);
                    scope.ErrorNode = "";
                }
            }
            else {
                if ($scope.parameters != undefined && $scope.parameters.Elements.length > 0) {
                    angular.forEach($scope.parameters.Elements, function (value, key) {
                        value.IsEditParameter = false;
                    });
                    $scope.SelectedParameter = $scope.parameters.Elements[0];
                }

                if ($scope.objLogicalRule.dictAttributes.sfwRuleType == "LogicalRule") {
                    if ($scope.objSelectedLogicalRule && $scope.objSelectedLogicalRule.Elements.length > 0) {
                        var obj = $scope.objSelectedLogicalRule.Elements.filter(function (x) {
                            return x.Name != "variables";
                        });
                        if (obj && obj.length > 0) {
                            //var selectedNode = $scope.FindSelectedNode($scope.objSelectedLogicalRule);//Check if user already selected item, Sachin on 12 Dec
                            //if (selectedNode == undefined) {
                            obj[0].IsSelected = true;
                            $scope.selectedItem = obj[0];
                            //}
                        }
                    }
                }

                var scope = getScopeByFileName("MainPage");
                if (scope.ErrorNode) {
                    var nodeid = scope.ErrorNode.istrNodeID;
                    scope.getCurrentErrorStepNodeForLogicalRule(nodeid, $scope.objLogicalRule);
                    var Currenterrornode = scope.currenterrornode;
                    $scope.onStepSelectChange(Currenterrornode);
                    scope.ErrorNode = "";
                }
            }

            //#endregion local variables and parameters


            $scope.HasRelatedParameterBasedScenario = $scope.objLogicalRule.objExtraData.HasRelatedParameterBasedScenario;
            $scope.HasRelatedObjectBasedScenario = $scope.objLogicalRule.objExtraData.HasRelatedObjectBasedScenario;
            $scope.HasRelatedXLBasedScenario = $scope.objLogicalRule.objExtraData.HasRelatedExcelBasedScenario;
            $scope.HasRelatedDataSourceBasedScenario = $scope.objLogicalRule.objExtraData.HasRelatedDataSourceBasedScenario;

            $scope.UpdateScenarioDetails();

            $scope.updateParentRules();


            //var ruleId = "";
            //if ($scope.objLogicalRule.dictAttributes) {
            //    ruleId = $scope.objLogicalRule.dictAttributes.ID;
            //}
            //else {
            //    ruleId = $scope.objLogicalRule.RuleID;
            //}
            //$.connection.hubRuleModel.server.validateRule(ruleId).done(function (data) {
            //    // $scope.displayValidation(data);
            //});
            $.connection.hubMain.server.getCustomTypes().done(function (data) {
                $scope.customTypes = data;
            });
            $rootScope.IsLoading = false;
        });
    };
    $scope.selectRuleAndScrollToEffectiveDate = function (effectiveDate) {
        effectiveDate = effectiveDate ? effectiveDate : "Default";
        $scope.selectRule(effectiveDate);
        $scope.scrollToEffectiveDate(effectiveDate);
    };
    $scope.scrollToEffectiveDate = function (effectiveDate) {
        effectiveDate = effectiveDate ? effectiveDate : "Default";
        var item = document.getElementById([$scope.currentfile.FileName, "_version_", effectiveDate.replace('\\', '')].join(""));
        if (item) {
            item.scrollIntoView();
        }
        else {
            var selectionInterval = setInterval(function () {
                var item = document.getElementById([$scope.currentfile.FileName, "_version_", effectiveDate.replace('\\', '')].join(""));
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
                }, 1000);
            }, 100);
        }
    };

    $scope.sortEffectiveDates = function () {
        $scope.objEffectiveDate = sortDates($scope.objEffectiveDate);
    };
    //$scope.$watch('$viewContentLoaded', function () {
    //    $rootScope.IsLoading = false;
    //});

    $scope.canShowStartArrow = function () {
        if ($scope.objSelectedLogicalRule) {
            var lst = $scope.objSelectedLogicalRule.Elements.filter(function (x) { return x.Name != "variables"; });
            if (lst && lst.length > 0) {
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
    //#endregion

    //#region Xml source Start
    $scope.showSource = false;
    //$scope.selectedDesignSource = "Design";
    var editor_html;

    // used for find in design - does not support excel matrix
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        var ruleType = $scope.objLogicalRule.dictAttributes ? $scope.objLogicalRule.dictAttributes.sfwRuleType : $scope.objLogicalRule.RuleType;
        if (ruleType == "LogicalRule" && items[0] && items[0].Name) {
            if (items[items.length - 1].Name != "logicalrule" && items[0].Name != "parameters" && items[1].Name != "variables" && items[items.length - 1].Name != "ExtraField" && items[items.length - 1].Name != "ExtraFields") {
                if (items[items.length - 1].Name != "action") {
                    var itemss = items[items.length - 1];
                    if (items[items.length - 1].Name == "parameter" || items[items.length - 1].Name == "parameters") {
                        //If parameter and parameters selected inside calldecisiontable,calllogicalrule and callexcelmatrix
                        //Then show particular calldecisiontable, calllogicalrule and callexcelmatrix highlighted
                        var lastItem = items[items.length - 1];
                        items.pop();
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].Name == "calldecisiontable") {
                                itemss = $scope.callRule(lastItem, items, "calldecisiontable");
                                break;
                            }
                            else if (items[i].Name == "calllogicalrule") {
                                itemss = $scope.callRule(lastItem, items, "calllogicalrule");
                                break;
                            }
                            else if (items[i].Name == "callexcelmatrix") {
                                itemss = $scope.callRule(lastItem, items, "callexcelmatrix");
                                break;
                            }
                        }
                    }
                    itemss.isAdvanceSearched = IsAction;
                }
                else {
                    items[items.length - 1].isAdvanceSearched = IsAction;
                }
            }
        }
        else if (ruleType == "DecisionTable" && items[0] && items[0].Item) {
            items[items.length - 1].Item.isAdvanceSearched = IsAction;
        }
        //else if (ruleType == "DecisionTable" && items[0]) {
        //    if (items[0].Item) {
        //        items[items.length - 1].Item.isAdvanceSearched = IsAction;
        //    }
        //    else {
        //        items[items.length - 1].isAdvanceSearched = IsAction;
        //    }
        //}
    };
    $scope.traverseMainModel = function (path, strPath) {
        var items = [], objHierarchy = $scope.objLogicalRule;
        if (path.contains("-") || path.contains(",")) {
            var ruleType = $scope.objLogicalRule.dictAttributes ? $scope.objLogicalRule.dictAttributes.sfwRuleType : $scope.objLogicalRule.RuleType;
            switch (ruleType) {
                case "LogicalRule":
                    for (var i = 0; i < path.split(',').length; i++) {
                        if (path.split(',')[i].substring(0, path.split(',')[i].lastIndexOf('-')) == "items") {
                            //find children method
                            if (i != path.split(',').length - 1) {
                                objHierarchy = FindChidrenHierarchy(objHierarchy, path.split(',')[i + 1].substring(path.split(',')[i + 1].lastIndexOf('-') + 1));
                                i++;
                            }
                        }
                        else {
                            objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                        }
                        if (objHierarchy) {
                            if (objHierarchy.Name == "logicalrule") {
                                $scope.objSelectedLogicalRule = objHierarchy;
                            }
                            items.push(objHierarchy);
                        }
                    }
                    break;
                case "ExcelMatrix":
                    for (var i = 0; i < path.split(',').length; i++) {
                        objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));

                        if (objHierarchy) {
                            if (objHierarchy.Name == "logicalrule") {
                                $scope.objSelectedLogicalRule = objHierarchy;
                            }
                            items.push(objHierarchy);
                        }
                    }
                    break;
                case "DecisionTable":
                    //  -- path is nodepath, strPath is parent path                    
                    var parentRule = $scope.objLogicalRule.Rules[path.split(',')[0].substring(path.split(',')[0].lastIndexOf('-') + 1)];
                    if (strPath.length > 3 && !strPath[0].startsWith("ExtraFields") && !strPath[0].startsWith("ExtraField")) {
                        //if (strPath.indexOf("dataheaders") >= 0) {
                        //    for (var i = 0; i < parentRule.DataHeaders.length; i++) {
                        //        var currentRow = parentRule.DataHeaders[i];
                        //        if (currentRow.NodeID != undefined && currentRow.ItemType != undefined) {
                        //            for (var l = strPath.length - 3; l >= 0; l--) {
                        //                if (currentRow.ItemType + "=\"" + currentRow.NodeID + "\"" == strPath[l]) {
                        //                    items.push(currentRow);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //else {
                        for (var i = 0; i < parentRule.Rows.length; i++) {
                            var currentRow = parentRule.Rows[i];
                            for (var j = 0; j < currentRow.Cells.length; j++) {
                                if (currentRow.Cells[j].Item.NodeID != undefined && currentRow.Cells[j].Item.ItemType != undefined) {
                                    for (var l = strPath.length - 3; l >= 0; l--) {
                                        if (currentRow.Cells[j].Item.ItemType + "=\"" + currentRow.Cells[j].Item.NodeID + "\"" == strPath[l]) {
                                            items.push(currentRow.Cells[j]);
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                    break;
            }
        }
        return items;
    };
    $scope.selectElement = function (path, strPath) {
        $scope.selectedRuleItem = [];
        $scope.objSelectedRule = undefined;
        $scope.objSelectedSwitch = undefined;
        $scope.objSelectedAction = undefined;
        $scope.objSelectedDefault = undefined;
        $scope.objSelectedDefaultAction = undefined;
        // for logical rule
        if (path != null && $scope.objLogicalRule.dictAttributes && $scope.objLogicalRule.dictAttributes.sfwRuleType == "LogicalRule" && path != "neorule" && !path.includes("ExtraFields") && !path.includes("ExtraField")) {
            var items = $scope.traverseMainModel(path);
            // for toggling to effective date tab
            if (items[0] && items[0].Name == "logicalrule") {
                $scope.selectedRuleItem.push(items[0]);
                if (items[0].dictAttributes.sfwEffectiveDate) {
                    $scope.selectRuleAndScrollToEffectiveDate(items[0].dictAttributes.sfwEffectiveDate);
                }
                else {
                    $scope.selectRuleAndScrollToEffectiveDate("Default");
                }
            }
            if (items.length > 0 && items[items.length - 1].Name != "logicalrule") {
                if (items[0].Name == "parameters") {
                    $scope.IsToolsDivCollapsed = false;
                    $scope.setActiveLeftTab('Parameters');
                    $scope.setSelectedParameters(items[items.length - 1], window.event);
                }
                else if (items[1] && items[1].Name == "variables") {
                    $scope.setSelectedLocalVar(items[items.length - 1], window.event);
                }
                else if (items[items.length - 1].Name == "ExtraField" || items[items.length - 1].Name == "ExtraFields") {
                    //$scope.HideparameterLocalVars();
                    //$scope.onSelectChange($scope.selectedItem);
                }
                else if (items[items.length - 1].Name != "action") {
                    var itemss = items[items.length - 1];
                    if (items[items.length - 1].Name == "parameter" || items[items.length - 1].Name == "parameters") {
                        //If parameter and parameters selected inside calldecisiontable,calllogicalrule and callexcelmatrix
                        //Then show particular calldecisiontable, calllogicalrule and callexcelmatrix highlighted
                        var lastItem = items[items.length - 1];
                        items.pop();
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].Name == "calldecisiontable") {
                                itemss = $scope.callRule(lastItem, items, "calldecisiontable");
                                break;
                            }
                            else if (items[i].Name == "calllogicalrule") {
                                itemss = $scope.callRule(lastItem, items, "calllogicalrule");
                                break;
                            }
                            else if (items[i].Name == "callexcelmatrix") {
                                itemss = $scope.callRule(lastItem, items, "callexcelmatrix");
                                break;
                            }
                        }
                    }
                    $scope.onSelectChange(itemss);
                }
                else {
                    // on selected actions - action node was getting highlighted - now actual action will be highlighted
                    if (items[items.length - 1].Name == "action") {
                        $scope.onSelectChange(items[items.length - 2]);
                    }
                    else {
                        $scope.onSelectChange(items[items.length - 1]);
                    }
                    //for (var i = items.length - 1; i >= 0; i--) {
                    //    if (items[i].Name != "action") {
                    //        $scope.onSelectChange(items[i]);
                    //        break;
                    //    }
                    //}
                }
            }
        }
        // for excel matrix 
        if (path != null && $scope.objLogicalRule.dictAttributes && $scope.objLogicalRule.dictAttributes.sfwRuleType == "ExcelMatrix" && path != "neorule" && !path.includes("ExtraFields") && !path.includes("ExtraField")) {
            var items = $scope.traverseMainModel(path);
            if (items[0] && items[0].Name == "logicalrule") {
                if (items[0].dictAttributes.sfwEffectiveDate) {
                    $scope.selectRuleAndScrollToEffectiveDate(items[0].dictAttributes.sfwEffectiveDate);
                }
                else {
                    $scope.selectRuleAndScrollToEffectiveDate("Default");
                }
            }
            if (items.length > 0 && items[items.length - 1].Name != "logicalrule") {
                if (items[0].Name == "parameters") {
                    $scope.setActiveLeftTab('Parameters');
                    if (items[items.length - 1].Name != "parameters") {
                        $scope.setSelectedParameters(items[items.length - 1], window.event);
                    }
                }
                else if (items[1] && items[1].Name == "variables") {
                    $scope.setSelectedLocalVar(items[items.length - 1], window.event);
                }
            }
        }
        // for decision table -- path is parent path, strPath is nodepath
        if (path != null && $scope.objLogicalRule.RuleType == "DecisionTable") {
            if (!path[0].startsWith("parameter") && !path[0].startsWith("neorule") && !path[0].startsWith("ExtraFields") && !path[0].startsWith("ExtraField")) {
                $scope.objSelectedLogicalRule = $scope.objLogicalRule.Rules[strPath.split(',')[0].substring(strPath.split(',')[0].lastIndexOf('-') + 1)];
                $scope.selectRuleAndScrollToEffectiveDate($scope.objSelectedLogicalRule.EffectiveDate);
            }
            if (path.length > 0 && path.length < 4 && (path[0] == "parameters" || path[0].startsWith("parameter"))) {
                for (var i = 0; i < $scope.objLogicalRule.Parameters.length; i++) {
                    if ($scope.objLogicalRule.Parameters[i].ParamID == path[0].substring(path[0].indexOf("\"") + 1).replace("\"", "")) {
                        $scope.IsToolsDivCollapsed = false;
                        $scope.setSelectedParameters($scope.objLogicalRule.Parameters[i], window.event);
                    }
                }
            }
            else if (path.length > 3 && !path[0].startsWith("ExtraFields") && !path[0].startsWith("ExtraField")) {
                for (var i = 0; i < $scope.objSelectedLogicalRule.Rows.length; i++) {
                    var currentRow = $scope.objSelectedLogicalRule.Rows[i];
                    for (var j = 0; j < currentRow.Cells.length; j++) {
                        if (currentRow.Cells[j].Item.NodeID != undefined && currentRow.Cells[j].Item.ItemType != undefined) {
                            for (var l = path.length - 3; l >= 0; l--) {
                                if (currentRow.Cells[j].Item.ItemType + "=\"" + currentRow.Cells[j].Item.NodeID + "\"" == path[l]) {
                                    if ($scope.objSelectedLogicalRule.noConditionsDisplayed < i) {
                                        $scope.objSelectedLogicalRule.noConditionsDisplayed = i + 1;
                                    }
                                    $scope.onDecisionStepChange(currentRow.Cells[j]);
                                }
                            }
                        }
                    }
                }
            }
        }

        $rootScope.IsLoading = false;
        // for scrolling to the selected node in logical rule
        $timeout(function () {
            var elem = $("#" + $scope.currentfile.FileName).find(".selected-node");
            if (elem && $scope.objLogicalRule.dictAttributes && $scope.objLogicalRule.dictAttributes.sfwRuleType == "LogicalRule" && path != "neorule") {
                var selectedRuleIndex;
                if ($scope.selectedRuleItem.length > 0) {
                    selectedRuleIndex = $scope.loadedRules.indexOf($scope.selectedRuleItem[0]);
                }
                if (elem.length == 1) {
                    elem[0].scrollIntoView();
                }
                else {
                    if (elem[selectedRuleIndex]) {
                        elem[selectedRuleIndex].scrollIntoView();
                    }
                }
            }
            else if ($scope.objLogicalRule && $scope.objLogicalRule.RuleType == "DecisionTable") {
                var elem = $("#" + $scope.currentfile.FileName).find(".decision-table-selection");
                var index = 0;
                if (elem && elem.length > 0) {
                    elem[index].scrollIntoView();
                }
                else {
                    function ScrollToViewAfterLoadingDecisiontable() {
                        var Selectedele = $("#" + $scope.currentfile.FileName).find(".decision-table-selection");
                        if (Selectedele && Selectedele.length > 0) {
                            if (Selectedele[index].scrollIntoView) {
                                Selectedele[index].scrollIntoView();
                            }
                            $interval.cancel(DecTablepromise);
                        }
                    }
                    var DecTablepromise = $interval(ScrollToViewAfterLoadingDecisiontable, 500);
                }
            }
        });
    };

    $scope.onDecisionStepChange = function (step) {
        $scope.selectedDecisionTable = step;
        if ($scope.objLogicalRule && $scope.objSelectedLogicalRule) {
            $scope.ResetAllvaluesForDecisionTable($scope.objSelectedLogicalRule);
        }
        if (step.Item) {
            step.Item.IsSelected = true;
        }
    };

    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;
                if (xmlstring.length < 32000) {
                    hubMain.server.getRuleXmlString(xmlstring, $scope.currentfile.FileType, lineno);
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

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "SourceToDesignForRule", lineNumber);
                }
                $scope.receiverulexmlobject = function (data, path, strPath) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.selectedDecisionTable = undefined;
                        $scope.selectedItem = undefined;
                        $scope.ResetAllvalues($scope.objSelectedLogicalRule);
                        $scope.receiverulemodel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}
                    // navigate and highlight the node (path)
                    $scope.$evalAsync($scope.selectElement(path, strPath));
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

    $scope.onSelectChange = function (step) {
        $scope.selectedItem = step;
        $scope.SelectedParameter = undefined;
        if ($scope.objLogicalRule && $scope.objLogicalRule.dictAttributes && $scope.objSelectedLogicalRule) {
            $scope.ResetAllvalues($scope.objSelectedLogicalRule);
        }
        step.IsSelected = true;
    };

    $scope.callRule = function (obj, parentList, itemType) {
        var obje;
        if (obj && parentList[parentList.length - 1] && parentList[parentList.length - 1].Name == itemType) {
            obje = parentList[parentList.length - 1];
            return parentList[parentList.length - 1];
        }
        else {
            if (obj) {
                parentList.pop();
                obje = $scope.callRule(parentList[parentList.length - 1], parentList, itemType);
            }
            else {
                return obje;
            }
        }
        return obje;
    };
    $scope.FindNode = function (objParentElements, path, selectedItem, count) {
        var parent;
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                item.ParentVM = objParentElements;
                for (var i = path.length - 2; i >= 0; i--) {
                    if ((item.dictAttributes.sfwNodeID != undefined)) {
                        if (item.Name + "=\"" + item.dictAttributes.sfwNodeID + "\"" == path[i]) {
                            selectedItem[count] = item;
                            count++;
                            $scope.FindNode(item, path, selectedItem, count);
                        }
                    }
                    else {
                        if (item.Name == path[i]) {
                            selectedItem[count] = item;
                            count++;
                            $scope.FindNode(item, path, selectedItem, count);
                        }
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
    var SendDataPacketsToServerForDecisionTable = function (lstpackets, Description, effectivedate, status) {

        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPacketsForDecisionTable(lstpackets[i], lstpackets.length, i, Description, effectivedate, status);
        }
    };
    var returnSelectedItem = function (item, selectcol) {

        item.Cells.filter(function (itm) {
            if (itm.Item.IsSelected == true) {
                selectcol.push(itm.Item);
                return itm.Item;
            }
        });
    };

    var selectD;
    var findSelectedDecision = function (obj) {
        if (obj.Rules && obj.Rules.length > 0) {
            angular.forEach(obj.Rules, function (item) {
                item.ParentVM = obj;
                if (item.Rows) {
                    selectD = findSelectedDecision(item);
                    return selectD;
                }
            });
        }
        else if (obj.Rows) {
            angular.forEach(obj.Rows, function (item) {
                item.ParentVM = obj;
                if (item.Cells) {
                    selectD = findSelectedDecision(item);
                    return selectD;
                }
            });
        }
        else if (obj.Cells) {

            obj.Cells.filter(function (itm) {
                itm.ParentVM = obj;
                if (itm.Item.IsSelected == true) {
                    itm.Item.ParentVM = itm;
                    selectD = itm.Item;
                    return selectD;
                }
            });
        }
        return selectD;
    };
    var getPathDecisionTable = function (sObj, indexPath) {
        var b = $scope.objLogicalRule;
        while (sObj.ParentVM) {
            if (sObj.ParentVM.Cells && sObj.ParentVM.Cells.length > 0) {
                indexPath.push(sObj.ParentVM.Cells.indexOf(sObj));
            }
            else if (sObj.ParentVM.Rows && sObj.ParentVM.Rows.length > 0) {
                indexPath.push(sObj.ParentVM.Rows.indexOf(sObj));
            }
            else if (sObj.ParentVM.Rules && sObj.ParentVM.Rules.length > 0) {
                indexPath.push(sObj.ParentVM.Rules.indexOf(sObj));
            }
            sObj = sObj.ParentVM;
        }
        return indexPath;
    };


    $scope.ShowSource = function () {
        //$scope.selectedDesignSource = "Source";
        if ($scope.selectedDesignSource == false) {
            $scope.showSource = true;
            $scope.selectedDesignSource = true;

            if ($("#dvBuildResults").is(":visible")) {
                hideBuildResults();
            }

            var sObj;
            var indexPath = [];
            var pathString;

            if ($scope.objLogicalRule != null && $scope.objLogicalRule != undefined) {
                var objreturn1;
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();
                if ($scope.currentfile.FileType == "DecisionTable") {
                    objreturn1 = $scope.objLogicalRule;
                }
                else {
                    objreturn1 = GetBaseModel($scope.objLogicalRule);
                }
                var strobj = JSON.stringify(objreturn1);

                var nodeId = [];
                var nodeId1 = [];
                if ($scope.currentfile.FileType == "DecisionTable") {
                    if ($scope.selectedDecisionTable != undefined) {
                        nodeId[0] = $scope.selectedDecisionTable.Item.NodeID;

                        // for Dataheaders
                        var selectedNode = $scope.objSelectedLogicalRule.DataHeaders.filter(function (item) { return item.IsSelected; });
                        var selectedRuleIndex = $scope.objLogicalRule.Rules.indexOf($scope.objSelectedLogicalRule);

                        if (selectedNode.length == 0) {
                            var selectcol = [];
                            selectedNode = $scope.objSelectedLogicalRule.Rows.filter(function (item) {
                                return returnSelectedItem(item, selectcol);
                            });

                            if (selectcol.length > 0) {
                                nodeId[0] = selectcol[0].NodeID;
                                selectcol[0].IsSelected = false;//on 9 Jan 
                            }

                        }
                        else {
                            nodeId[0] = selectedNode[0].NodeID;
                        }
                        nodeId[0] = nodeId[0] + "~" + selectedRuleIndex;
                    }
                    else {
                        var selectedRuleIndex = $scope.objLogicalRule.Rules.indexOf($scope.objSelectedLogicalRule);
                        nodeId[0] = "~" + selectedRuleIndex;
                    }
                    //if ($scope.SelectedParameter != undefined && $scope.IsParameterVisible == true) {
                    //    nodeId[1] = $scope.SelectedParameter.ParamID;
                    //}
                }
                else {
                    var selectedNode = $scope.FindSelectedNode($scope.objSelectedLogicalRule);
                    if (selectedNode && selectedNode.IsSelected) {
                        var pathToObject = [];

                        sObj = $scope.FindDeepNode($scope.objLogicalRule, selectedNode, pathToObject);
                        pathString = $scope.getPathSource($scope.objLogicalRule, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }
                    else {
                        var pathToObject = [];

                        sObj = $scope.FindDeepNode($scope.objLogicalRule, $scope.objSelectedLogicalRule, pathToObject);
                        pathString = $scope.getPathSource($scope.objLogicalRule, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }

                    if ($scope.SelectedParameter != undefined && $scope.activeLeftTab == "Parameters") {
                        var pathToObject = [];

                        sObj = undefined;
                        indexPath = [];
                        sObj = $scope.FindDeepNode($scope.objLogicalRule, $scope.SelectedParameter, pathToObject);
                        pathString = $scope.getPathSource($scope.objLogicalRule, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }
                    else if ($scope.SelectedLocalVariable != undefined && $scope.IsLocalVariableVisible == true) {
                        sObj = undefined;
                        indexPath = [];

                        sObj = $scope.FindDeepNode($scope.objLogicalRule, $scope.SelectedLocalVariable, pathToObject);
                        pathString = $scope.getPathSource($scope.objLogicalRule, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }
                }
                if (nodeId.length == 0) {
                    nodeId[0] = "";
                }
                if (strobj.length < 32000) {
                    hubMain.server.getRuleXmlObject(strobj, $scope.currentfile, nodeId);
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

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "DesignToSourceForRule", nodeId);
                }
                $scope.receiverulexml = function (xmlstring, lineno) {
                    $scope.$apply(function () {
                        $scope.xmlSource = xmlstring;
                        var ID = $scope.currentfile.FileName;
                        setDataToEditor($scope, xmlstring, lineno, ID);
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $scope.selectedDecisionTable = undefined;
                            //$scope.selectedItem = undefined;
                            $scope.ResetAllvalues($scope.objSelectedLogicalRule);

                        });
                    });

                };

            }

        }

    };
    var selectedItem;
    $scope.FindSelectedNode = function (objParentElements) {

        if (objParentElements) {
            if (objParentElements.Elements.length > 0) {
                angular.forEach(objParentElements.Elements, function (item) {
                    if (item.IsSelected == true) {
                        selectedItem = item;
                        return selectedItem;
                    }
                    else if ((item.Elements && item.Elements.length > 0) || (item.Children && item.Children.length > 0)) {
                        selectedItem = $scope.FindSelectedNode(item);
                        return selectedItem;
                    }
                });

            }

            if (objParentElements.Children && objParentElements.Children.length > 0) {
                angular.forEach(objParentElements.Children, function (item) {
                    if (item.IsSelected == true) {
                        selectedItem = item;
                        return selectedItem;
                    }
                    else if ((item.Elements && item.Elements.length > 0) || (item.Children && item.Children.length > 0)) {
                        selectedItem = $scope.FindSelectedNode(item);
                        return selectedItem;
                    }
                });
            }
        }
        return selectedItem;
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {
            if (objParentElements.Elements && objParentElements.Elements.length > 0) {
                angular.forEach(objParentElements.Elements, function (item) {
                    var isNodeInPath = $scope.isValidObject(item, selectedItem);
                    if (isNodeInPath) {
                        pathToObject.push(item);
                    }
                    if (item == selectedItem) {
                        return selectedItem;
                    }
                    else if ((item.Elements && item.Elements.length > 0) || (item.Children && item.Children.length > 0)) {
                        selectedItem = $scope.FindDeepNode(item, selectedItem, pathToObject);
                        return selectedItem;
                    }
                });
            }

            if (objParentElements.Children && objParentElements.Children.length > 0) {
                angular.forEach(objParentElements.Children, function (item) {
                    var isNodeInPath = $scope.isValidObject(item, selectedItem);
                    if (isNodeInPath) {
                        pathToObject.push(item);
                    }
                    if (item == selectedItem) {
                        return selectedItem;
                    }
                    else if ((item.Elements && item.Elements.length > 0) || (item.Children && item.Children.length > 0)) {
                        selectedItem = $scope.FindDeepNode(item, selectedItem, pathToObject);
                        return selectedItem;
                    }
                });
            }
        }
        return selectedItem;
    };
    $scope.FindNodeHierarchy = function (objParentElements, index) {
        if (objParentElements && objParentElements.Elements) {
            var newObj = objParentElements.Elements[index];
            return newObj;
        }
    };

    $scope.getPathSource = function (objModel, pathToObject, indexPath) {
        for (var i = 0; i < pathToObject.length; i++) {
            if (i == 0) {
                var indx = objModel.Elements.indexOf(pathToObject[i]);
                indexPath.push("elements-" + indx);
            }
            else {
                var indx = pathToObject[i - 1].Elements.indexOf(pathToObject[i]);
                if (indx == -1) {
                    indx = pathToObject[i - 1].Children.indexOf(pathToObject[i]);
                    indexPath.push("children-" + indx);
                }
                else {
                    indexPath.push("elements-" + indx);
                }
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

        if (objParentElements.Elements && objParentElements.Elements.length > 0) {
            for (var ele = 0; ele < objParentElements.Elements.length; ele++) {
                if (objParentElements.Elements[ele] == selectedItem) {
                    result = true;
                    return result;
                }
                if (objParentElements.Elements[ele].Elements && objParentElements.Elements[ele].Elements.length > 0) {
                    for (var iele = 0; iele < objParentElements.Elements[ele].Elements.length; iele++) {
                        result = $scope.isValidObject(objParentElements.Elements[ele].Elements[iele], selectedItem);
                        if (result == true) {
                            return result;
                        }
                    }
                }
                if (objParentElements.Elements[ele].Children && objParentElements.Elements[ele].Children.length > 0) {
                    for (var iele = 0; iele < objParentElements.Elements[ele].Children.length; iele++) {
                        result = $scope.isValidObject(objParentElements.Elements[ele].Children[iele], selectedItem);
                        if (result == true) {
                            return result;
                        }
                    }
                }
            }
        }

        if (objParentElements.Children && objParentElements.Children.length > 0) {
            for (var chil = 0; chil < objParentElements.Children.length; chil++) {
                if (objParentElements.Children[chil] == selectedItem) {
                    result = true;
                    return result;
                }
                if (objParentElements.Children[chil].Elements && objParentElements.Children[chil].Elements.length > 0) {
                    for (var iele = 0; iele < objParentElements.Children[chil].Elements.length; iele++) {
                        result = $scope.isValidObject(objParentElements.Children[chil].Elements[iele], selectedItem);
                        if (result == true) {
                            return result;
                        }
                    }
                }
                if (objParentElements.Children[chil].Children && objParentElements.Children[chil].Children.length > 0) {
                    for (var iele = 0; iele < objParentElements.Children[chil].Children.length; iele++) {
                        result = $scope.isValidObject(objParentElements.Children[chil].Children[iele], selectedItem);
                        if (result == true) {
                            return result;
                        }
                    }
                }
            }
        }
        return result;
    };
    //#endregion


    //#region Common Methods
    $scope.setActiveLeftTab = function (tab) {
        $scope.activeLeftTab = tab;
    };
    $scope.setViewMode = function (viewmode) {
        $scope.ViewMode = viewmode;
        if (viewmode == "Analyst View") {
            $("#" + $rootScope.currentopenfile.file.FileName).find(".hidden-span-expression").hide();
        } else {
            $("#" + $rootScope.currentopenfile.file.FileName).find(".hidden-span-expression").show();
        }
    };


    $scope.selectRule = function (effectiveDate, fromUndoRedoBlock) {
        if ($scope.IsDecisionTable) {
            var rules = $scope.loadedRules.filter(function (item) { return item.EffectiveDate == effectiveDate || (effectiveDate == "Default" && !item.EffectiveDate); });
            if (rules && rules.length > 0) {
                if (fromUndoRedoBlock) {
                    $rootScope.EditPropertyValue($scope.objSelectedLogicalRule, $scope, "objSelectedLogicalRule", rules[0]);
                }
                else {
                    $scope.objSelectedLogicalRule = rules[0];
                }
            }
            else {
                rules = $scope.objLogicalRule.Rules.filter(function (item) { return item.EffectiveDate == effectiveDate || (effectiveDate == "Default" && !item.EffectiveDate); });
                if (rules && rules.length > 0) {
                    rules[0].noConditionsDisplayed = 30;
                    if (fromUndoRedoBlock) {
                        $rootScope.EditPropertyValue($scope.objSelectedLogicalRule, $scope, "objSelectedLogicalRule", rules[0]);
                        $rootScope.PushItem(rules[0], $scope.loadedRules);
                    }
                    else {
                        $scope.objSelectedLogicalRule = rules[0];
                        $scope.loadedRules.push(rules[0]);
                    }
                }
            }
        }
        else {

            var rules = $scope.loadedRules.filter(function (item) { return item.dictAttributes.sfwEffectiveDate == effectiveDate || (effectiveDate == "Default" && !item.dictAttributes.sfwEffectiveDate); });
            if (rules && rules.length > 0) {
                if (fromUndoRedoBlock) {
                    $rootScope.EditPropertyValue($scope.objSelectedLogicalRule, $scope, "objSelectedLogicalRule", rules[0]);
                }
                else {
                    $scope.objSelectedLogicalRule = rules[0];
                }
            }
            else {
                rules = $scope.objLogicalRule.Elements.filter(function (item) { return item.Name == "logicalrule" && (item.dictAttributes.sfwEffectiveDate == effectiveDate || (effectiveDate == "Default" && !item.dictAttributes.sfwEffectiveDate)); });
                if (rules && rules.length > 0) {
                    if (fromUndoRedoBlock) {
                        $rootScope.EditPropertyValue($scope.objSelectedLogicalRule, $scope, "objSelectedLogicalRule", rules[0]);
                        $rootScope.PushItem(rules[0], $scope.loadedRules);
                    }
                    else {
                        $scope.objSelectedLogicalRule = rules[0];
                        $scope.loadedRules.push(rules[0]);
                    }
                }
            }
        }

        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedEffectiveDate, $scope, "selectedEffectiveDate", effectiveDate);
            $rootScope.EditPropertyValue($scope.variables, $scope, "variables", GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', $scope.IsDecisionTable));
        }
        else {
            $scope.selectedEffectiveDate = effectiveDate;
            $scope.variables = GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', $scope.IsDecisionTable);
        }

        $scope.updateChildRules(fromUndoRedoBlock);
        if ($scope.objLogicalRule.dictAttributes && $scope.objLogicalRule.dictAttributes.sfwRuleType == 'ExcelMatrix') {
            if (!$scope.objSelectedLogicalRule.objColumnValues) {
                $scope.prepareExcelMatrix();
            }
        }
    };
    $scope.SelectLogicalRule = function (obj) {
        var effectiveDate;
        if (obj) {
            if ($scope.objLogicalRule.dictAttributes) {
                effectiveDate = obj.dictAttributes.sfwEffectiveDate;

                for (var i = 0; i < $scope.objLogicalRule.Elements.length; i++) {
                    if ($scope.objLogicalRule.Elements[i].Name == "logicalrule") {
                        if (($scope.objLogicalRule.Elements[i].dictAttributes.sfwEffectiveDate == effectiveDate)) {
                            $scope.objSelectedLogicalRule = $scope.objLogicalRule.Elements[i];
                            $scope.updateChildRules();
                            if ($scope.objLogicalRule.dictAttributes.sfwRuleType == 'ExcelMatrix') {
                                $scope.prepareExcelMatrix();
                            }
                            $scope.variables = GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', $scope.IsDecisionTable);

                            angular.forEach($scope.variables.Elements, function (value, key) {
                                value.IsEditLocalVar = false;
                            });
                        }
                        else if (($scope.objLogicalRule.Elements[i].dictAttributes.sfwEffectiveDate == undefined || $scope.objLogicalRule.Elements[i].dictAttributes.sfwEffectiveDate == "" || $scope.objLogicalRule.Elements[i].dictAttributes.sfwEffectiveDate == "Default") && (effectiveDate == undefined || effectiveDate == "" || effectiveDate == "Default")) {
                            $scope.objSelectedLogicalRule = $scope.objLogicalRule.Elements[i];
                            $scope.updateChildRules();
                            $scope.variables = GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', $scope.IsDecisionTable);

                            angular.forEach($scope.variables.Elements, function (value, key) {
                                value.IsEditLocalVar = false;
                            });
                        }
                    }
                }
            }
            else {
                if ($scope.objLogicalRule.dictAttributes == undefined) {
                    effectiveDate = obj.EffectiveDate;
                    for (var i = 0; i < $scope.objLogicalRule.Rules.length; i++) {

                        if ($scope.objLogicalRule.Rules[i].EffectiveDate == effectiveDate) {
                            $scope.objSelectedLogicalRule = $scope.objLogicalRule.Rules[i];
                        }
                        else if (($scope.objLogicalRule.Rules[i].EffectiveDate == undefined || $scope.objLogicalRule.Rules[i].EffectiveDate == "" || $scope.objLogicalRule.Rules[i].EffectiveDate == "Default") && (effectiveDate == undefined || effectiveDate == "" || effectiveDate == "Default")) {
                            $scope.objSelectedLogicalRule = $scope.objLogicalRule.Rules[i];
                        }
                        $scope.updateChildRules();
                    }
                }
            }
        }



    };

    $scope.SetClass = function (object, step) {

        if (object == step) {
            return "active";
        }
    };
    $scope.receiveEntityBusinessObject = function (astrBusinessObjectName) {
        $scope.IsBusObjPresent = true;
        if ($scope.objLogicalRule.dictAttributes) {
            if ($scope.objLogicalRule.dictAttributes.sfwRuleType != "ExcelMatrix" && astrBusinessObjectName) {
                $scope.CanCreateObjectBasedScenario = true;
            }
            $.connection.hubRuleModel.server.getEntityBusinessObjectMethods(astrBusinessObjectName).done(function (data) {
                $scope.businessObjectMethods = data;
            });

            if (($scope.objLogicalRule.dictAttributes.sfwStatic && $scope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "true") || !astrBusinessObjectName) {
                $scope.ShowMethod = false;
            }
        }
        else {
            if (astrBusinessObjectName) {
                $scope.CanCreateObjectBasedScenario = true;
            }
            $.connection.hubRuleModel.server.getEntityBusinessObjectMethods(astrBusinessObjectName).done(function (data) {
                $scope.businessObjectMethods = data;
            });
            if (($scope.objLogicalRule.Static && $scope.objLogicalRule.Static.toLowerCase() == "true") || !astrBusinessObjectName) {
                $scope.ShowMethod = false;
            }

        }

    };

    $scope.canSaveFile = function () {
        var retValue = true;
        if (isEntityIdChanged) {
            var dirtyScenarioNames = $rootScope.lstopenedfiles
                .filter(function (item) {
                    return (item.file.FileName.startsWith("rsn") || item.file.FileName.startsWith("rso")) && item.file.FileName.substring(3) == $scope.currentfile.FileName.substring(3);
                })
                .filter(function (scenario) {
                    var scenarioScope = getScopeByFileName(scenario.file.FileName);
                    return scenarioScope && scenarioScope.isDirty;
                }).map(function (scenario) {
                    return "'" + scenario.file.FileName + "'";
                });

            if (dirtyScenarioNames && dirtyScenarioNames.length > 0) {
                alert("Please save following files before saving this rule.\n\n" + dirtyScenarioNames.join("\n"));
                retValue = false;
            }
        }
        return retValue;
    }
    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };

    $scope.AfterSaveToFile = function () {
        if (!isEntityIdChanged && $scope.objLogicalRule) {
            var entityid = "";
            if (!$scope.IsDecisionTable && $scope.objLogicalRule.dictAttributes) {
                entityid = $scope.objLogicalRule.dictAttributes.sfwEntity;
            }
            else {
                entityid = $scope.objLogicalRule.Entity;
            }
            $scope.UpdateIntellisenseListAttributesAfterSave(entityid);
        }

    };

    $scope.UpdateIntellisenseListAttributesAfterSave = function (entityid) {
        var ruleid = "";

        if ($scope.IsDecisionTable) {
            ruleid = $scope.objLogicalRule.RuleID;
        }
        else {
            ruleid = $scope.objLogicalRule.dictAttributes.ID;
        }
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        for (var i = 0; i < entityIntellisenseList.length; i++) {
            if (entityIntellisenseList[i].ID == entityid) {
                if (entityIntellisenseList[i].Rules.length > 0) {
                    for (var j = 0; j < entityIntellisenseList[i].Rules.length; j++) {
                        if (entityIntellisenseList[i].Rules[j].ID == ruleid) {
                            intellisenseruleobject = entityIntellisenseList[i].Rules[j];
                            intellisenseruleobject.Parameters = [];
                            if ($scope.IsDecisionTable) {

                                intellisenseruleobject.IsStatic = $scope.objLogicalRule.Static != undefined && $scope.objLogicalRule.Static.toLowerCase() == "true" ? true : false;
                                intellisenseruleobject.ReturnType = $scope.objLogicalRule.ReturnType;
                                intellisenseruleobject.Status = $scope.objLogicalRule.Status;
                                intellisenseruleobject.IsPrivate = $scope.objLogicalRule.Private != undefined && $scope.objLogicalRule.Private.toLowerCase() == "true" ? true : false;
                                angular.forEach($scope.objLogicalRule.Parameters, function (param) {
                                    var parameter = { ID: param.ParamID, DisplayName: param.ParamID, Value: param.ParamID, Tooltip: param.ParamID, DataType: param.DataType, Direction: param.Direction, Entity: param.Entity };

                                    intellisenseruleobject.Parameters.push(parameter);
                                });

                            }
                            else {
                                intellisenseruleobject.IsStatic = $scope.objLogicalRule.dictAttributes.sfwStatic != undefined && $scope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "true" ? true : false;
                                intellisenseruleobject.ReturnType = $scope.objLogicalRule.dictAttributes.sfwReturnType;
                                intellisenseruleobject.Status = $scope.objLogicalRule.dictAttributes.sfwStatus;
                                intellisenseruleobject.IsPrivate = $scope.objLogicalRule.dictAttributes.sfwPrivate != undefined && $scope.objLogicalRule.dictAttributes.sfwPrivate.toLowerCase() == "true" ? true : false;
                                angular.forEach($scope.objLogicalRule.Elements, function (item) {
                                    if (item.Name == "parameters") {
                                        angular.forEach(item.Elements, function (param) {
                                            if (param.Name == "parameter") {
                                                var parameter = { ID: param.dictAttributes.ID, DisplayName: param.dictAttributes.ID, Value: param.dictAttributes.ID, Tooltip: param.dictAttributes.ID, DataType: param.dictAttributes.sfwDataType, Direction: param.dictAttributes.sfwDirection, Entity: param.dictAttributes.sfwEntity };
                                                intellisenseruleobject.Parameters.push(parameter);
                                            }
                                        });
                                    }

                                });
                            }
                        }
                    }
                }
                break;
            }

        }
    };

    $scope.onStepSelectChange = function (step) {
        var rootScope = getCurrentFileScope();
        if (rootScope.objLogicalRule && rootScope.objLogicalRule.dictAttributes && rootScope.objSelectedLogicalRule) {
            rootScope.ResetAllvalues(rootScope.objSelectedLogicalRule);
        }
        step.IsSelected = true;
    };

    $scope.objStepSelectionChanged = function (step) {
        var rootScope = getCurrentFileScope();
        if (rootScope.objLogicalRule && rootScope.objSelectedLogicalRule) {
            rootScope.ResetAllvaluesForDecisionTable(rootScope.objSelectedLogicalRule);
        }
        if (step.Item) {
            step.Item.IsSelected = true;
        }
    };

    $scope.openEntityFile = function () {
        if ($scope.objLogicalRule.dictAttributes) {
            $.connection.hubMain.server.navigateToFile($scope.objLogicalRule.dictAttributes.sfwEntity, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
        else {
            $.connection.hubMain.server.navigateToFile($scope.objLogicalRule.Entity, "").done(function (objfile) {
                $rootScope.openFile(objfile, false);
            });
        }
    };

    //#endregion

    //#region Add/Change Effective Date
    $scope.NewVersion = function () {
        var newScope = $scope.$new();
        newScope.objDummyRule = {
        };
        newScope.objDummyRule.effectiveDatePicker = "";
        newScope.objDummyRule.description = "";
        newScope.objDummyRule.status = $scope.objstatus[0];
        newScope.objDummyRule.chkCopyCurrentRule = false;
        newScope.objDummyRule.chkDefaultVersion = false;
        newScope.objDummyRule.IsOkDisable = true;
        newScope.CancelNewVersion = function () {
            $scope.CancelNewVersion(newScope);
        };
        newScope.getDisableValue = function () {
            var retVal = $scope.getDisableValue(newScope);
            return retVal;
        };
        newScope.isValidVersion = function () {
            $scope.getDisableValue(newScope);
        };
        newScope.AddLogicalRule = function () {
            $scope.AddLogicalRule(newScope);
        };

        var dialog = $rootScope.showDialog(newScope, "Add New Version", "Rule/views/AddNewVersion.html");
        ComponentsPickers.init();
        $scope.CancelNewVersion = function (newScope) {
            dialog.close();
        };
        newScope.isValidVersion();
    };

    $scope.EditEffectiveDateDailog = function () {
        var newScope = $scope.$new();
        newScope.hasDefaultVersion = $scope.hasDefaultVersion;
        newScope.Init = function () {
            newScope.objEditSelectedRule = {};
            newScope.objEditSelectedRule.IsDefaultVersion = false;
            newScope.objEditSelectedRule.lstEffectiveDate = [];
            newScope.objEditSelectedRule.IsDefault = false;
            if ($scope.objSelectedLogicalRule.dictAttributes != undefined) {
                if ($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate == undefined || $scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate == "") {
                    newScope.objEditSelectedRule.IsDefaultVersion = true;
                    newScope.objEditSelectedRule.IsDefault = true;
                }
                else {
                    newScope.objEditSelectedRule.EffectiveDate = $scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate;
                }
            }
            else if ($scope.objSelectedLogicalRule.dictAttributes == undefined) {
                if ($scope.objSelectedLogicalRule.EffectiveDate == undefined || $scope.objSelectedLogicalRule.EffectiveDate == "") {
                    newScope.objEditSelectedRule.IsDefaultVersion = true;
                    newScope.objEditSelectedRule.IsDefault = true;
                }
                else {
                    newScope.objEditSelectedRule.EffectiveDate = $scope.objSelectedLogicalRule.EffectiveDate;
                }
            }

            function iterator(itm) {
                if (newScope.objEditSelectedRule.EffectiveDate != itm && itm != "Default") {
                    newScope.objEditSelectedRule.lstEffectiveDate.push(itm);
                }
            }
            angular.forEach($scope.objEffectiveDate, iterator);
        };

        newScope.ValidateEffectiveDate = function () {
            var IsValid = false;
            newScope.objEditSelectedRule.ErrorMsgForDisplay = "";
            if (newScope.objEditSelectedRule.IsDefault) {
                if (newScope.objEditSelectedRule.EffectiveDate) {
                    if (newScope.objEditSelectedRule.lstEffectiveDate.some(function (itm) { return itm == newScope.objEditSelectedRule.EffectiveDate && itm != newScope.objEditSelectedRule.SelectDefaultVersion; })) {
                        IsValid = true;
                        newScope.objEditSelectedRule.ErrorMsgForDisplay = "Duplicate Effective Date.";
                    }
                    else if (!newScope.objEditSelectedRule.SelectDefaultVersion) {
                        IsValid = true;
                        newScope.objEditSelectedRule.ErrorMsgForDisplay = "Select Rule to set as Default.";
                    }
                }
                else if (!newScope.objEditSelectedRule.IsDefaultVersion) {
                    IsValid = true;
                    newScope.objEditSelectedRule.ErrorMsgForDisplay = "Select Effective Date.";
                }
            }
            else {
                if ($scope.hasDefaultVersion) {
                    if (!newScope.objEditSelectedRule.EffectiveDate) {
                        newScope.objEditSelectedRule.ErrorMsgForDisplay = "Select Effective Date for Default version.";
                        IsValid = true;
                    }
                    else if (newScope.objEditSelectedRule.lstEffectiveDate.some(function (itm) { return itm == newScope.objEditSelectedRule.EffectiveDate; })) {
                        IsValid = true;
                        newScope.objEditSelectedRule.ErrorMsgForDisplay = "Duplicate Effective Date.";
                    }
                }
            }
            return IsValid;
        };

        newScope.onOkClick = function () {
            if (newScope.objEditSelectedRule.IsDefault) {
                if (!newScope.objEditSelectedRule.IsDefaultVersion) {
                    $rootScope.UndRedoBulkOp("Start");
                    if ($scope.objSelectedLogicalRule.dictAttributes != undefined) {
                        if ($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate == undefined || $scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate == "") {

                            var lst = $scope.objLogicalRule.Elements.filter(function (itm) {
                                return itm.dictAttributes.sfwEffectiveDate == newScope.objEditSelectedRule.SelectDefaultVersion;
                            });
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate, $scope.objSelectedLogicalRule.dictAttributes, "sfwEffectiveDate", newScope.objEditSelectedRule.EffectiveDate);

                            if (lst && lst.length > 0) {
                                $rootScope.EditPropertyValue(lst[0].dictAttributes.sfwEffectiveDate, lst[0].dictAttributes, "sfwEffectiveDate", "");


                            }
                        }
                    }
                    else if ($scope.objSelectedLogicalRule.dictAttributes == undefined) {
                        if ($scope.objSelectedLogicalRule.EffectiveDate == undefined || $scope.objSelectedLogicalRule.EffectiveDate == "") {

                            var lst = $scope.objLogicalRule.Rules.filter(function (itm) {
                                return itm.EffectiveDate == newScope.objEditSelectedRule.SelectDefaultVersion;
                            });
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.EffectiveDate, $scope.objSelectedLogicalRule, "EffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                            if (lst && lst.length > 0) {
                                $rootScope.EditPropertyValue(lst[0].EffectiveDate, lst[0], "EffectiveDate", "");
                            }
                        }
                    }

                    var effDateIndex = $scope.objEffectiveDate.indexOf(newScope.objEditSelectedRule.SelectDefaultVersion);
                    $rootScope.DeleteItem($scope.objEffectiveDate[effDateIndex], $scope.objEffectiveDate);
                    $rootScope.InsertItem("Default", $scope.objEffectiveDate, effDateIndex);

                    var effDateIndex = $scope.objEffectiveDate.indexOf($scope.selectedEffectiveDate);
                    $rootScope.DeleteItem($scope.objEffectiveDate[effDateIndex], $scope.objEffectiveDate);
                    $rootScope.InsertItem(newScope.objEditSelectedRule.EffectiveDate, $scope.objEffectiveDate, effDateIndex);
                    $rootScope.EditPropertyValue($scope.selectedEffectiveDate, $scope, "selectedEffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
            else {
                if (newScope.objEditSelectedRule.IsDefaultVersion) {
                    $rootScope.UndRedoBulkOp("Start");

                    if ($scope.objSelectedLogicalRule.dictAttributes != undefined) {
                        if ($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate) {

                            var lst = $scope.objLogicalRule.Elements.filter(function (itm) {
                                return itm.Name == "logicalrule" && !itm.dictAttributes.sfwEffectiveDate;
                            });
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate, $scope.objSelectedLogicalRule.dictAttributes, "sfwEffectiveDate", "");
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.sfwStatus, $scope.objSelectedLogicalRule.dictAttributes, "sfwStatus", "");
                            if (lst && lst.length > 0) {
                                $rootScope.EditPropertyValue(lst[0].dictAttributes.sfwEffectiveDate, lst[0].dictAttributes, "sfwEffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                            }
                        }

                    }
                    else if ($scope.objSelectedLogicalRule.dictAttributes == undefined) {
                        if ($scope.objSelectedLogicalRule.EffectiveDate) {

                            var lst = $scope.objLogicalRule.Rules.filter(function (itm) {
                                return !itm.EffectiveDate;
                            });
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.EffectiveDate, $scope.objSelectedLogicalRule, "EffectiveDate", "");
                            $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.Status, $scope.objSelectedLogicalRule, "Status", "");
                            if (lst && lst.length > 0) {
                                $rootScope.EditPropertyValue(lst[0].EffectiveDate, lst[0], "EffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                            }
                        }
                    }

                    var effDateIndex = $scope.objEffectiveDate.indexOf("Default");
                    if (effDateIndex > -1) {
                        $rootScope.DeleteItem($scope.objEffectiveDate[effDateIndex], $scope.objEffectiveDate);
                        $rootScope.InsertItem(newScope.objEditSelectedRule.EffectiveDate, $scope.objEffectiveDate, effDateIndex);
                    }
                    var effDateIndex = $scope.objEffectiveDate.indexOf($scope.selectedEffectiveDate);
                    $rootScope.DeleteItem($scope.objEffectiveDate[effDateIndex], $scope.objEffectiveDate);
                    $rootScope.InsertItem("Default", $scope.objEffectiveDate, effDateIndex);
                    $rootScope.EditPropertyValue($scope.selectedEffectiveDate, $scope, "selectedEffectiveDate", "Default");
                    if (!$scope.hasDefaultVersion) {
                        $rootScope.EditPropertyValue($scope.hasDefaultVersion, $scope, "hasDefaultVersion", true);
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    $rootScope.UndRedoBulkOp("Start");
                    var effDateIndex = $scope.objEffectiveDate.indexOf($scope.selectedEffectiveDate);
                    if ($scope.objSelectedLogicalRule.dictAttributes != undefined) {
                        $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate, $scope.objSelectedLogicalRule.dictAttributes, "sfwEffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                    }
                    else {
                        $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.EffectiveDate, $scope.objSelectedLogicalRule, "EffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                    }
                    $rootScope.DeleteItem($scope.objEffectiveDate[effDateIndex], $scope.objEffectiveDate);
                    $rootScope.InsertItem(newScope.objEditSelectedRule.EffectiveDate, $scope.objEffectiveDate, effDateIndex);
                    $rootScope.EditPropertyValue($scope.selectedEffectiveDate, $scope, "selectedEffectiveDate", newScope.objEditSelectedRule.EffectiveDate);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
            newScope.CloseDialog();
        };

        newScope.CloseDialog = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };
        newScope.onChangeDefaultVersion = function () {
            newScope.objEditSelectedRule.EffectiveDate = "";
            newScope.objEditSelectedRule.SelectDefaultVersion = "";
        };
        newScope.Init();
        newScope.dialog = $rootScope.showDialog(newScope, "Edit Effective Date", "Rule/views/EditEffectiveDate.html", { width: 550, height: 400 });
        ComponentsPickers.init();
    };

    $scope.getDisableValue = function (newScope) {

        var retValue = false;
        newScope.objDummyRule.IsOkDisable = false;
        newScope.objDummyRule.errorMessage = "";
        if (!newScope.objDummyRule.chkDefaultVersion) {
            if (newScope.objDummyRule.effectiveDatePicker == undefined || newScope.objDummyRule.effectiveDatePicker == '') {
                newScope.objDummyRule.IsOkDisable = true;
                newScope.objDummyRule.errorMessage = "Effective Date cannot be empty.";
                retValue = true;
            }
            else {

                if ($scope.objLogicalRule.Elements == undefined && $scope.objLogicalRule.RuleType == "DecisionTable") {
                    angular.forEach($scope.objLogicalRule.Rules, function (rule) {
                        if (rule.EffectiveDate == newScope.objDummyRule.effectiveDatePicker) {
                            newScope.objDummyRule.IsOkDisable = true;
                            newScope.objDummyRule.errorMessage = "Version with same effective date already exists.";
                            retValue = true;
                        }
                    });
                }
                else {
                    angular.forEach($scope.objLogicalRule.Elements, function (rule) {
                        if (rule.Name == "logicalrule") {
                            if (rule.dictAttributes.sfwEffectiveDate == newScope.objDummyRule.effectiveDatePicker) {
                                newScope.objDummyRule.IsOkDisable = true;
                                newScope.objDummyRule.errorMessage = "Version with same effective date already exists.";
                                retValue = true;
                            }
                        }
                    });
                }
            }
        }
        else if (newScope.objDummyRule.chkDefaultVersion) {
            newScope.objDummyRule.effectiveDatePicker = "";
            var isFound = false;
            for (var i = 0; i < $scope.objEffectiveDate.length; i++) {
                if ($scope.objEffectiveDate[i] == undefined || $scope.objEffectiveDate[i] == "" || $scope.objEffectiveDate[i] == "Default") {
                    isFound = true;
                    newScope.objDummyRule.errorMessage = "Default version already exists.";
                    newScope.objDummyRule.IsOkDisable = isFound;
                    retValue = isFound;
                    break;
                }
            }
            if (!isFound) {
                newScope.objDummyRule.effectiveDatePicker = '';
                newScope.objDummyRule.IsOkDisable = isFound;
                retValue = isFound;
            }
        }
        //else {
        //    angular.forEach($scope.objLogicalRule.Elements, function (rule) {
        //        if (rule.Name == "logicalrule") {
        //            if (rule.dictAttributes.sfwEffectiveDate == newScope.objDummyRule.effectiveDatePicker) {
        //                newScope.objDummyRule.IsOkDisable = true;
        //                retValue = true;
        //            }
        //        }
        //    });
        //}
        return retValue;
    };

    $scope.AddLogicalRule = function (newScope) {
        var effectivedate;
        if (newScope.objDummyRule.chkDefaultVersion) {
            var effectivedate = "";
        }
        else {
            effectivedate = newScope.objDummyRule.effectiveDatePicker;
        }
        var Description = newScope.objDummyRule.description;
        var Status = newScope.objDummyRule.status;

        var objlogicalrule;
        if (newScope.objDummyRule.chkCopyCurrentRule) {
            if ($scope.objSelectedLogicalRule.dictAttributes) { // i.e. logical rule /Excel matrix
                var data = JSON.stringify($scope.objSelectedLogicalRule);
                objlogicalrule = resetNodeIds(JSON.parse(data));
                objlogicalrule.NodeID = generateUUID();
            }
            else {
                var strobj = JSON.stringify($scope.objSelectedLogicalRule);
                if (strobj.length < 32000) {

                    $.connection.hubMain.server.refreshNodeIdsForDT(strobj, Description, effectivedate, Status);

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

                    SendDataPacketsToServerForDecisionTable(lstDataPackets, Description, effectivedate, Status);
                }
            }
        }
        else {
            if ($scope.objLogicalRule.dictAttributes == undefined) {
                if ($scope.objLogicalRule.RuleType == 'DecisionTable') {

                    objlogicalrule = {
                        Rows: [], DataHeaders: []
                    };
                    objlogicalrule.NodeID = generateUUID();
                    var dataheadernodeid = generateUUID();
                    var dataheaders = {
                        Description: '', Expression: '', ItemType: 'dataheader', NodeID: dataheadernodeid
                    };

                    objlogicalrule.DataHeaders.push(dataheaders);

                    var cells = {
                        Cells: []
                    };
                    var rowheaderid = generateUUID();

                    cell = {
                        Colspan: 1, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'rowheader', NodeID: rowheaderid }
                    };
                    cells.Cells.push(cell);
                    cell = {
                        Colspan: 1, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'assignheader', NodeID: generateUUID(), DataHeaderID: dataheadernodeid }
                    };
                    cells.Cells.push(cell);
                    objlogicalrule.Rows.push(cells);

                    var cells = {
                        Cells: []
                    };
                    cell = {
                        Colspan: 1, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid }
                    };
                    cells.Cells.push(cell);
                    cell = {
                        Colspan: 1, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'assign', NodeID: generateUUID(), DataHeaderID: dataheadernodeid }
                    };
                    cells.Cells.push(cell);
                    objlogicalrule.Rows.push(cells);
                }
            }
            else {
                objlogicalrule = {
                    Name: 'logicalrule', value: '', dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: []
                };
            }
        }
        $rootScope.UndRedoBulkOp("Start");

        if (objlogicalrule) {
            if ($scope.objLogicalRule.dictAttributes != undefined) {
                objlogicalrule.dictAttributes.Text = Description;
                objlogicalrule.dictAttributes.sfwStatus = Status;
                objlogicalrule.dictAttributes.sfwEffectiveDate = effectivedate;
                $rootScope.PushItem(objlogicalrule, $scope.objLogicalRule.Elements, "SelectLogicalRule");
            }
            else {
                objlogicalrule.Description = Description;
                objlogicalrule.Status = Status;
                objlogicalrule.EffectiveDate = effectivedate;

                $rootScope.PushItem(objlogicalrule, $scope.objLogicalRule.Rules, "SelectLogicalRule");
            }

            $rootScope.PushItem(effectivedate ? effectivedate : "Default", $scope.objEffectiveDate);
            $scope.selectRule(effectivedate ? effectivedate : "Default", true);

            $scope.effectiveDate = effectivedate;
            $rootScope.UndRedoBulkOp("End");

            newScope.objDummyRule.effectiveDatePicker = null;
            newScope.objDummyRule.description = null;
            newScope.objDummyRule.status = $scope.objstatus[0];
            newScope.objDummyRule.chkCopyCurrentRule = false;
            newScope.objDummyRule.chkDefaultVersion = false;
            if ($scope.objSelectedLogicalRule != undefined) {
                $scope.variables = GetVariablesAndParams($scope.objSelectedLogicalRule, 'variables', false);
                if ($scope.variables != undefined) {
                    angular.forEach($scope.variables.Elements, function (value, key) {
                        value.IsEditLocalVar = false;
                    });
                }
            }
        }
        newScope.CancelNewVersion();
    };

    $scope.DeleteLogicalRule = function (effectiveDate) {
        if (effectiveDate == "Default") {
            alert("Cannot Delete the Default Rule ");
            return;
        }

        var result = confirm(String.format("Do you want to delete rule version '{0}'?", effectiveDate));
        if (result) {
            if ($scope.objLogicalRule.dictAttributes != undefined) {
                var rules = $scope.objLogicalRule.Elements.filter(function (item) { return item.Name == "logicalrule" && item.dictAttributes.sfwEffectiveDate == effectiveDate; });
                if (rules && rules.length > 0) {
                    $rootScope.UndRedoBulkOp("Start");
                    var effectiveDateIndex = $scope.objEffectiveDate.indexOf(effectiveDate);
                    $rootScope.DeleteItem(rules[0], $scope.objLogicalRule.Elements, null);

                    var lstRule = $scope.loadedRules.filter(function (x) {
                        return x.dictAttributes.sfwEffectiveDate == effectiveDate;
                    });
                    if (lstRule && lstRule.length > 0) {
                        $rootScope.DeleteItem(lstRule[0], $scope.loadedRules, null);
                    }
                    $rootScope.DeleteItem(effectiveDate, $scope.objEffectiveDate);
                    if (effectiveDateIndex > 0) {
                        $scope.selectRule($scope.objEffectiveDate[effectiveDateIndex - 1], true);
                    }
                    else {
                        $scope.selectRule($scope.objEffectiveDate[effectiveDateIndex], true);
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
            }
            else if ($scope.objLogicalRule.dictAttributes == undefined) {
                var rules = $scope.objLogicalRule.Rules.filter(function (item) { return item.EffectiveDate == effectiveDate; });
                if (rules && rules.length > 0) {
                    $rootScope.UndRedoBulkOp("Start");
                    var effectiveDateIndex = $scope.objEffectiveDate.indexOf(effectiveDate);
                    $rootScope.DeleteItem(rules[0], $scope.objLogicalRule.Rules, null);
                    if ($scope.loadedRules.indexOf(rules[0]) > 0) {
                        $rootScope.DeleteItem(rules[0], $scope.loadedRules, null);
                    }
                    $rootScope.DeleteItem(effectiveDate, $scope.objEffectiveDate);
                    if (effectiveDateIndex > 0) {
                        $scope.selectRule($scope.objEffectiveDate[effectiveDateIndex - 1], true);
                    }
                    else {
                        $scope.selectRule($scope.objEffectiveDate[effectiveDateIndex], true);
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
    };

    $scope.recieveRefreshNodeIds = function (data) {
        $scope.$apply(function () {
            var objlogicalrule = JSON.parse(data);
            $rootScope.PushItem(objlogicalrule, $scope.objLogicalRule.Rules, "SelectLogicalRule");
            $rootScope.PushItem(objlogicalrule.EffectiveDate ? objlogicalrule.EffectiveDate : "Default", $scope.objEffectiveDate);
            $scope.selectRule(objlogicalrule.EffectiveDate ? objlogicalrule.EffectiveDate : "Default", true);

        });
    };

    //#region Export To Excel
    $scope.ExportToExcel = function () {
        $rootScope.IsLoading = true;
        var strobj = JSON.stringify($scope.objLogicalRule)
        if (strobj.length < 32000) {
            $.connection.hubRuleModel.server.exportToExcel(strobj, $scope.selectedEffectiveDate);
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

            SendDataPacketsToServerForExportExcel(lstDataPackets);
        }

    }
    var SendDataPacketsToServerForExportExcel = function (lstpackets) {

        for (var i = 0; i < lstpackets.length; i++) {
            $.connection.hubRuleModel.server.receiveDataPacketsForExportExcel(lstpackets[i], lstpackets.length, i, $scope.selectedEffectiveDate);
        }
    };

    //#endregion

    //#endregion

    //#region Build Rule /Rule In Expression 
    $scope.buildRule = function (isObjectRule) {
        if ($scope.objLogicalRule.dictAttributes == undefined) {
            $rootScope.IsLoading = true;
            $.connection.hubRuleModel.server.getRuleBuildResult($scope.objLogicalRule.RuleID, isObjectRule).done(function (data) {
                $rootScope.receiveRuleBuildResult(data);
            });
        }
        else {
            $rootScope.IsLoading = true;
            $.connection.hubRuleModel.server.getRuleBuildResult($scope.objLogicalRule.dictAttributes.ID, isObjectRule).done(function (data) {
                $rootScope.receiveRuleBuildResult(data);
            });
        }

    };

    $scope.openRuleInExpression = function (isObjectRule) {
        if ($scope.objLogicalRule.dictAttributes == undefined) {
            $.connection.hubRuleModel.server.getRuleExpression($scope.objLogicalRule.RuleID, $scope.objSelectedLogicalRule.EffectiveDate, isObjectRule, false, $rootScope.currentopenfile.file.FileName);
        }
        else {
            $.connection.hubRuleModel.server.getRuleExpression($scope.objLogicalRule.dictAttributes.ID, $scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate, isObjectRule, false, $rootScope.currentopenfile.file.FileName);
        }
    };

    $scope.loadChildRuleInExpression = function (obj) {
        if (obj.HasChildRules && !obj.IsExpanded && obj.lstChildren.length == 0) {
            if ($scope.objLogicalRule.dictAttributes) {
                $.connection.hubRuleModel.server.getRuleExpression(obj.RuleID, "", obj.IsObjectRule, true);

            }
        }
        obj.IsExpanded = !obj.IsExpanded;
    };

    $scope.receiveRuleExpression = function (data, isChildRule, ruleId) {
        $scope.$apply(function () {
            var newScope = $scope.$new(true);
            if (isChildRule) {
                if ($scope.SelectedRuleInExpression) {
                    var lstRuleInExpression = JSON.parse(data);
                    $scope.SearchParentRuleForExpression(ruleId, lstRuleInExpression, $scope.SelectedRuleInExpression);
                }
            }
            else {
                var objRuleInExpression = JSON.parse(data);

                newScope.lstRuleInExpression = [];
                newScope.lstRuleInExpression.push(objRuleInExpression);
                $scope.SelectedRuleInExpression = objRuleInExpression;
                $scope.SelectedRuleInExpression.IsWithOutTracing = "WithoutTracing";
                $scope.SelectedRuleInExpression.SelectedEffectiveDate = objRuleInExpression.lstExpressions[0];


                var dialog = $rootScope.showDialog(newScope, "Rule In Expression", "Rule/views/RuleInExpression.html", { width: 800, height: 600 });
                newScope.CloseDialog = function () {
                    dialog.close();
                };
                newScope.getClassForRuleInExpression = function (objExpression) {
                    if ($scope.SelectedRuleInExpression == objExpression) {
                        return "selected";
                    }
                    else {
                        return "";
                    }
                };
                newScope.onSelectRuleInExpression = function (objExpression, event) {
                    $scope.SelectedRuleInExpression = objExpression;
                    $scope.SelectedRuleInExpression.SelectedEffectiveDate = objExpression.lstExpressions[0];
                    event.stopPropagation();
                };
                newScope.loadChildRuleInExpression = function (obj) {
                    if (obj.HasChildRules && !obj.IsExpanded && obj.lstChildren.length == 0) {
                        if ($scope.objLogicalRule.dictAttributes) {
                            $.connection.hubRuleModel.server.getRuleExpression(obj.RuleID, "", obj.IsObjectRule, true, $rootScope.currentopenfile.file.FileName);
                        }
                    }
                    obj.IsExpanded = !obj.IsExpanded;
                };
            }
        });
    };

    $scope.SearchParentRuleForExpression = function (ruleId, lstRuleInExpression, obj) {
        if (obj.RuleID == ruleId) {
            obj.lstChildren = lstRuleInExpression;
        }
        else {
            angular.forEach(obj.lstChildren, function (x) {
                //if (x.RuleID == ruleId) {
                //    x.lstChildren = lstRuleInExpression;
                //}
                $scope.SearchParentRuleForExpression(ruleId, lstRuleInExpression, x);
            });
        }
    };

    $scope.onSelectRuleInExpression = function (objExpression, event) {
        $scope.SelectedRuleInExpression = objExpression;
        $scope.SelectedRuleInExpression.SelectedEffectiveDate = objExpression.lstExpressions[0];
        //$scope.selectRuleInExpressionXml();
        event.stopPropagation();
    };

    $scope.selectRuleInExpressionXml = function () {
        //$scope.$apply(function () {
        //$("div[class='displayblock main-parent']").find("#code-html").text(xmlstring);
        //$("div[class='displayblock main-parent']").find(".CodeMirror").remove();
        var latestDialogId = ngDialog.getOpenDialogs();
        if (latestDialogId.length > 0) {
            var te_html = $('div[id=' + latestDialogId[latestDialogId.length - 1] + ']').find("#txtruleInExpression");
            // var te_html =document.getElementById("#txtruleInExpression");
            te_html[0].value = xmlstring;
            editor_html = CodeMirror.fromTextArea(te_html[0], {
                mode: "application/xml",
                styleActiveLine: true,
                lineNumbers: true,
                foldGutter: true,
                autofocus: true,
                readonly: true,
                gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
                lineWrapping: false,

            });
        }
        //editor_html.on("blur", function () {
        //    editor_html.save();
        //    editor_html.refresh();

        //});
        //});
    };

    $scope.getClassForRuleInExpression = function (objExpression) {
        if ($scope.SelectedRuleInExpression == objExpression) {
            return "selected";
        }
    };
    //#endregion

    //#region Local Variable Methods

    $scope.AddLocalVariable = function () {
        var variable = { Name: 'variable', value: '', dictAttributes: {}, Elements: [] };
        $scope.OnLocalVarEdit(variable, true);
    };

    $scope.deleteLocalVariable = function (variable) {

    };

    $scope.RemoveLocalVariable = function (variable, canSkipConfirmation) {
        $scope.SelectedLocalVariable = variable;
        var confirmation = true;
        if (!canSkipConfirmation) {
            confirmation = confirm("Local Variable : '" + $scope.SelectedLocalVariable.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?");
        }
        if (confirmation) {
            var index = $scope.variables.Elements.indexOf($scope.SelectedLocalVariable);
            $rootScope.DeleteItem($scope.SelectedLocalVariable, $scope.variables.Elements);

            if (index < $scope.variables.Elements.length) {
                $scope.SelectedLocalVariable = $scope.variables.Elements[index];
            }
            else if ($scope.variables.Elements.length > 0) {
                $scope.SelectedLocalVariable = $scope.variables.Elements[index - 1];
            }
        }
    };

    $scope.setSelectedLocalVar = function (selectedVar, event) {
        //var li = event.currentTarget;
        //if (li) {
        //    $(li).addClass("selected-variable");
        //    $(li).siblings().removeClass("selected-variable");
        //}

        $scope.SelectedLocalVariable = selectedVar;
        angular.forEach($scope.variables.Elements, function (value, key) {
            if (value != selectedVar) {
                value.IsEditLocalVar = false;
            }
        });
    };

    $scope.getClassForLocalVar = function (objVar) {
        if (objVar == $scope.SelectedLocalVariable) {
            return "selected-variable";
        }
    };

    $scope.OnLocalVarEdit = function (selectedVar, isAdd) {

        $scope.SelectedLocalVariable = selectedVar;
        var dialogScope = $scope.$new();
        dialogScope.isAdd = isAdd;
        dialogScope.addType = "Variable";
        dialogScope.variable = selectedVar;
        var str = $scope.SelectedLocalVariable.dictAttributes.ID;
        var isPrefixVisible = false;
        var datatypeprefix = GetDataTypePrefix($scope.SelectedLocalVariable.dictAttributes.sfwDataType);
        if (datatypeprefix != undefined) {
            if ($scope.SelectedLocalVariable.dictAttributes.ID.match("^" + datatypeprefix)) {
                str = str.substring(datatypeprefix.length);
                isPrefixVisible = true;
            }

        }
        if (str == undefined) {
            str = '';
        }
        dialogScope.variable.strID = str;
        dialogScope.variable.IsPrefixVisible = isPrefixVisible;
        dialogScope.variable.StrDataTypePrefix = datatypeprefix;
        dialogScope.variable.strDataType = $scope.SelectedLocalVariable.dictAttributes.sfwDataType;
        dialogScope.variable.strEntity = $scope.SelectedLocalVariable.dictAttributes.sfwEntity;
        dialogScope.variable.strValue = $scope.SelectedLocalVariable.dictAttributes.Value;
        dialogScope.variable.strText = $scope.SelectedLocalVariable.dictAttributes.Text;
        dialogScope.variable.strDataFormat = $scope.SelectedLocalVariable.dictAttributes.sfwDataFormat;

        dialogScope.UpdateLocalVarDatatype = function (datatype) {
            dialogScope.variable.StrDataTypePrefix = GetDataTypePrefix(datatype);
            dialogScope.variable.IsPrefixVisible = true;

            if (datatype != "Collection" && datatype != "CDOCollection" && datatype != "List" && datatype != "Object") {
                dialogScope.variable.strEntity = "";
            }
        };
        dialogScope.closeDialog = function () {
            if (dialogScope.isAdd) {
                $scope.SelectedLocalVariable = $scope.variables.length > 0 ? $scope.variables[0] : undefined;
            }
            if (dialog && dialog.close) {
                dialog.close();
            }
        };

        dialogScope.OnLocalVarOkClick = function () {
            $scope.SelectedLocalVariable = dialogScope.variable;
            $scope.SelectedLocalVariable.IsEditLocalVar = false;
            var strId = $scope.SelectedLocalVariable.strID;
            if ($scope.SelectedLocalVariable.IsPrefixVisible) {
                strId = ($scope.SelectedLocalVariable.StrDataTypePrefix ? $scope.SelectedLocalVariable.StrDataTypePrefix : "") + ($scope.SelectedLocalVariable.strID ? $scope.SelectedLocalVariable.strID : "");
            }
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.ID, $scope.SelectedLocalVariable.dictAttributes, "ID", strId);

            if ($scope.SelectedLocalVariable.dictAttributes.ID == undefined) {
                $scope.SelectedLocalVariable.dictAttributes.ID = '';
            }

            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.sfwDataType, $scope.SelectedLocalVariable.dictAttributes, "sfwDataType", $scope.SelectedLocalVariable.strDataType);

            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.sfwEntity, $scope.SelectedLocalVariable.dictAttributes, "sfwEntity", $scope.SelectedLocalVariable.strEntity);

            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.Value, $scope.SelectedLocalVariable.dictAttributes, "Value", $scope.SelectedLocalVariable.strValue);

            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.Text, $scope.SelectedLocalVariable.dictAttributes, "Text", $scope.SelectedLocalVariable.strText);

            $rootScope.EditPropertyValue($scope.SelectedLocalVariable.dictAttributes.sfwDataFormat, $scope.SelectedLocalVariable.dictAttributes, "sfwDataFormat", $scope.SelectedLocalVariable.strDataFormat);

            if (dialogScope.isAdd) {
                $rootScope.PushItem($scope.SelectedLocalVariable, $scope.variables.Elements);
            }
            $rootScope.UndRedoBulkOp("End");

            dialogScope.closeDialog();
        };
        dialogScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                dialogScope.OnLocalVarOkClick();
                //dialogScope.closeDialog();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };

        if (dialogScope.isAdd) {
            dialogScope.variable.strDataType = "string";
            dialogScope.UpdateLocalVarDatatype(dialogScope.variable.strDataType);
        }
        var dialog = $rootScope.showDialog(dialogScope, dialogScope.isAdd ? "Add Variable" : "Edit Variable", "Rule/views/AddEditVariable.html");
    };

    $scope.isInvalidEntity = function (selectedVar) {
        var retValue = false;
        if (selectedVar.strDataType == "Collection" || selectedVar.strDataType == "CDOCollection" || selectedVar.strDataType == "Object" || selectedVar.strDataType == "List") {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList;
            if (!entities.some(function (element, index, array) { return element.ID == selectedVar.strEntity; })) {
                retValue = true;
            }
        }
        return retValue;
    };

    $scope.isInvalidVariable = function (selectedVar) {
        var retValue = false;
        if (selectedVar) {
            selectedVar.ErrorMessage = "";

            if (!selectedVar.strID || selectedVar.strID.replace(" ", "").length == 0) {
                retValue = true;
                selectedVar.ErrorMessage = "Error: Enter the ID.";
            }
            else if (isDuplicateID(selectedVar)) {
                retValue = true;
                selectedVar.ErrorMessage = "Error: Duplicate ID.";
            }
            else if (!selectedVar.strDataType || selectedVar.strDataType.replace(" ", "").length == 0) {
                retValue = true;
                selectedVar.ErrorMessage = "Error: Enter the Datatype.";
            }
            else if ((selectedVar.strDataType == "Collection" || selectedVar.strDataType == "CDOCollection" || selectedVar.strDataType == "Object" || selectedVar.strDataType == "List") && (!selectedVar.strEntity || selectedVar.strEntity.replace(" ", "").length == 0)) {
                retValue = true;
                selectedVar.ErrorMessage = "Error: Enter the Entity.";
            }
            else if ($scope.isInvalidEntity(selectedVar)) {
                retValue = true;
                selectedVar.ErrorMessage = "Error: Invalid Entity.";
            }
        }
        return retValue;
    };

    function isDuplicateID(selectedVar) {
        var retValue = false;
        var id = (selectedVar.StrDataTypePrefix ? selectedVar.StrDataTypePrefix : "") + selectedVar.strID;
        if ($scope.IsDecisionTable) {
            if ($scope.parameters) {
                var params = $scope.parameters.filter(function (x) { return x.ParamID == id && !angular.equals(x, selectedVar); });
                if (params.length > 0) {
                    retValue = true;
                }
            }
        }
        else {
            if ($scope.parameters && $scope.parameters.Elements && $scope.parameters.Elements.length > 0) {
                var params = $scope.parameters.Elements.filter(function (x) { return x.dictAttributes.ID == id && !angular.equals(x, selectedVar); });
                if (params.length > 0) {
                    retValue = true;
                }
            }
            if ($scope.variables && $scope.variables.Elements && $scope.variables.Elements.length > 0) {
                var variabls = $scope.variables.Elements.filter(function (x) { return x.dictAttributes.ID == id && !angular.equals(x, selectedVar); });
                if (variabls.length > 0) {
                    retValue = true;
                }
            }
            var oneToOneAttributeList = $ValidationService.getEntityAttributes($scope.objLogicalRule.dictAttributes.sfwEntity, true, false, false, false, false);
            if (angular.isArray(oneToOneAttributeList)) {
                var attributes = oneToOneAttributeList.filter(function (attr) { return attr && attr.ID && attr.ID.toLowerCase() == id.toLowerCase(); });
                if (angular.isArray(attributes) && attributes.length > 0) {
                    retValue = true;
                }
            }
        }
        return retValue;
    }

    $scope.menuOptionsForLocalVariables = [
        ['Convert to Parameter', function ($itemScope) {
            if ($itemScope.objVar != undefined) {
                if ($scope.IsDecisionTable) {
                    if (confirm("Local Variable : '" + $itemScope.objVar.ParamID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                        $rootScope.UndRedoBulkOp("Start");
                        var objnewparam = {
                            ParamID: $itemScope.objVar.ParamID,
                            DataType: $itemScope.objVar.DataType,
                            Direction: "In",
                            DataFormat: $itemScope.objVar.DataFormat,
                            Entity: $itemScope.objVar.Entity,
                            Text: $itemScope.objVar.Text
                        };
                        if ($scope.parameters.length > 13) {
                            alert("Cannot convert to parameter as maximum limit has been reached.");
                            return;
                        }
                        $rootScope.PushItem(objnewparam, $scope.parameters);

                        var index = $scope.parameters.length - 1;
                        $scope.SelectedParameter = $scope.parameters[index];
                        $scope.RemoveLocalVariable($itemScope.objVar, true);
                        $rootScope.UndRedoBulkOp("End");
                    }
                }
                else {
                    if (confirm("Local Variable : '" + $itemScope.objVar.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                        $rootScope.UndRedoBulkOp("Start");

                        var objnewparam = {
                            Name: 'parameter',
                            value: $itemScope.objVar.value,
                            dictAttributes: {
                                ID: $itemScope.objVar.dictAttributes.ID,
                                sfwDataType: $itemScope.objVar.dictAttributes.sfwDataType,
                                sfwEntity: $itemScope.objVar.dictAttributes.sfwEntity,
                                Text: $itemScope.objVar.dictAttributes.Text,
                                sfwDataFormat: $itemScope.objVar.dictAttributes.sfwDataFormat,
                                sfwDirection: "In",
                            }, Elements: []
                        };

                        if ($scope.parameters.Elements.length > 13) {
                            alert("Cannot convert to parameter as maximum limit has been reached.");
                            return;
                        }

                        $rootScope.PushItem(objnewparam, $scope.parameters.Elements);

                        var index = $scope.parameters.Elements.length - 1;
                        $scope.SelectedParameter = $scope.parameters.Elements[index];
                        $scope.RemoveLocalVariable($itemScope.objVar, true);
                        $rootScope.UndRedoBulkOp("End");

                    }
                }
            }
        }, function ($itemScope) {
            if ($itemScope.objVar != undefined) {
                $scope.SelectedLocalVariable = $itemScope.objVar;
            }
            return true;
        }], null,

    ];

    $scope.ShowLocalVars = function (event) {
        var localVar = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvLocalVar");
        $scope.IsParameterVisible = false;
        $scope.IsParentRulesVisible = false;
        $scope.IsChildRulesVisible = false;
        if (localVar.is(':visible')) {
            localVar.hide("slide", { direction: "right" });
            $scope.IsLocalVariableVisible = false;
        }
        else {
            localVar.show("slide", { direction: "right" });
            $scope.IsLocalVariableVisible = true;
        }
        //setSelectionForCurrentPanel(event);
        closeOtherRightPanels("LocalVariables");
    };
    //#endregion

    //#region Parameters Methods
    $scope.AddParameters = function () {
        if ($scope.IsDecisionTable) {
            var itm = { ParamID: '', DataType: '', Direction: '', DataFormat: '', Entity: '', Text: '' };

        }
        else {
            var itm = { Name: 'parameter', value: '', dictAttributes: {}, Elements: [] };

        }
        $scope.AddOrUpdateParameter(itm, true);

    };

    $scope.RemoveParameters = function (canSkipConfirmation) {
        if ($scope.IsDecisionTable) {
            $scope.DeleteParameter($scope.parameters, canSkipConfirmation);
        }
        else {
            $scope.DeleteParameter($scope.parameters.Elements, canSkipConfirmation);
        }
    };

    $scope.DeleteParameter = function (Elements, canSkipConfirmation) {
        if ($scope.SelectedParameter) {
            var id = "";
            if ($scope.SelectedParameter.ParamID) {
                id = $scope.SelectedParameter.ParamID;
            } else if ($scope.SelectedParameter.strID) {
                id = $scope.SelectedParameter.strID;
            }
            else if ($scope.SelectedParameter.dictAttributes && $scope.SelectedParameter.dictAttributes.ID) {
                id = $scope.SelectedParameter.dictAttributes.ID;
            }
            var confirmation = true;
            if (!canSkipConfirmation) {
                confirmation = confirm("Parameter : '" + id + "'" + "  " + " will be deleted, Do you want to continue?");
            }
            if (confirmation) {
                var index = Elements.indexOf($scope.SelectedParameter);
                $rootScope.DeleteItem($scope.SelectedParameter, Elements);

                if (index < Elements.length) {
                    $scope.SelectedParameter = Elements[index];
                }
                else if (Elements.length > 0) {
                    $scope.SelectedParameter = Elements[index - 1];
                }
            }
        }
    };

    $scope.setSelectedParameters = function (selectedParam, event) {
        $scope.selectedItem = undefined;
        //$scope.showEditIcon = index;
        //var li = event.currentTarget;
        //if (li) {
        //    $(li).addClass("selected-variable");
        //    $(li).siblings().removeClass("selected-variable");
        //}

        $scope.SelectedParameter = selectedParam;
        if ($scope.IsDecisionTable) {
            angular.forEach($scope.parameters, function (value, key) {
                if (value != selectedParam) {
                    value.IsEditParameter = false;
                }
            });
        }
        else {
            angular.forEach($scope.parameters.Elements, function (value, key) {
                if (value != selectedParam) {
                    value.IsEditParameter = false;
                }
            });
        }
    };

    $scope.getClassForParameters = function (objVar) {
        if (objVar == $scope.SelectedParameter) {
            return "selected-variable";
        }
    };

    $scope.OnParameterEdit = function (selectedVar) {
        $scope.SelectedParameter = selectedVar;
        var isPrefixVisible = false;
        if ($scope.IsDecisionTable) {
            var str = $scope.SelectedParameter.ParamID;
            var datatypeprefix = GetDataTypePrefix($scope.SelectedParameter.DataType);
            if (datatypeprefix != undefined) {
                if ($scope.SelectedParameter.ParamID.match("^" + datatypeprefix)) {
                    str = str.substring(datatypeprefix.length);
                    isPrefixVisible = true;
                }
            }
            if (str == undefined) {
                str = '';
            }
            $scope.OnEditParameter(datatypeprefix, str, $scope.SelectedParameter.DataType, $scope.SelectedParameter.Entity, $scope.SelectedParameter.Direction, $scope.SelectedParameter.Text, $scope.SelectedParameter.DataFormat, isPrefixVisible);
        }
        else {
            var str = $scope.SelectedParameter.dictAttributes.ID;
            var datatypeprefix = GetDataTypePrefix($scope.SelectedParameter.dictAttributes.sfwDataType);
            if (datatypeprefix != undefined) {
                if ($scope.SelectedParameter.dictAttributes.ID.match("^" + datatypeprefix)) {
                    str = str.substring(datatypeprefix.length);
                    isPrefixVisible = true;
                }
            }
            if (str == undefined) {
                str = '';
            }

            $scope.OnEditParameter(datatypeprefix, str, $scope.SelectedParameter.dictAttributes.sfwDataType, $scope.SelectedParameter.dictAttributes.sfwEntity, $scope.SelectedParameter.dictAttributes.sfwDirection, $scope.SelectedParameter.dictAttributes.Text, $scope.SelectedParameter.dictAttributes.sfwDataFormat, isPrefixVisible);
        }
    };

    $scope.OnEditParameter = function (astrdatatypeprefix, astrId, astrDataType, astrEntity, astrDirection, astrText, astrDataFormat, isPrefixVisible) {
        $scope.SelectedParameter.strID = astrId;
        $scope.SelectedParameter.StrDataTypePrefix = astrdatatypeprefix;
        $scope.SelectedParameter.IsPrefixVisible = isPrefixVisible;
        $scope.SelectedParameter.strDataType = astrDataType;
        $scope.SelectedParameter.strEntity = astrEntity;
        if (astrDirection != undefined && astrDirection != "") {
            $scope.SelectedParameter.strDirection = astrDirection;
        } else {
            $scope.SelectedParameter.strDirection = "In";
        }

        $scope.SelectedParameter.strText = astrText;
        $scope.SelectedParameter.strDataFormat = astrDataFormat;
    };

    $scope.AddOrUpdateParameter = function (selectedParam, isAdd) {
        var dialogScope = $scope.$new();
        dialogScope.addType = "Parameter";
        dialogScope.isAdd = isAdd;
        dialogScope.parameter = selectedParam;
        var isPrefixVisible = false;
        $scope.OnParameterEdit(dialogScope.parameter);

        dialogScope.UpdateParameterDataType = function (datatype) {
            dialogScope.parameter.StrDataTypePrefix = GetDataTypePrefix(datatype);
            dialogScope.parameter.IsPrefixVisible = true;
            if (datatype != "Collection" && datatype != "CDOCollection" && datatype != "List" && datatype != "Object") {
                dialogScope.parameter.strEntity = "";
            }
        };

        dialogScope.UpdateDirection = function (strDirection) {
            dialogScope.parameter.strEntity = "";
            dialogScope.parameter.strDataType = "";
            dialogScope.parameter.StrDataTypePrefix = "";
        };

        dialogScope.closeDialog = function () {
            if (dialogScope.isAdd && $scope.IsDecisionTable) {
                $scope.SelectedParameter = $scope.parameters.length > 0 ? $scope.parameters[$scope.parameters.length - 1] : undefined;
            } else if (dialogScope.isAdd) {
                $scope.SelectedParameter = $scope.parameters.Elements.length > 0 ? $scope.parameters.Elements[$scope.parameters.Elements.length - 1] : undefined;
            }

            if (dialog && dialog.close) {
                dialog.close();
            }
        };

        dialogScope.OnParameterOkClick = function () {
            $scope.SelectedParameter = dialogScope.parameter;
            if (dialogScope.isAdd && $scope.IsDecisionTable && $scope.parameters && $scope.parameters.length > 13) {
                alert("Cannot add parameter as maximum limit has been reached.");
                return;
            } else if (dialogScope.isAdd && $scope.parameters && $scope.parameters.Elements && $scope.parameters.Elements.length > 13) {
                alert("Cannot add parameter as maximum limit has been reached.");
                return;
            }
            $rootScope.UndRedoBulkOp("Start");

            var strId = $scope.SelectedParameter.strID;
            if ($scope.SelectedParameter.IsPrefixVisible) {
                strId = $scope.SelectedParameter.StrDataTypePrefix + $scope.SelectedParameter.strID;
            }
            if ($scope.IsDecisionTable) {

                $rootScope.EditPropertyValue($scope.SelectedParameter.ParamID, $scope.SelectedParameter, "ParamID", strId);
                if ($scope.SelectedParameter.ParamID == undefined) {
                    $rootScope.EditPropertyValue($scope.SelectedParameter.ParamID, $scope.SelectedParameter, "ParamID", '');
                }

                $rootScope.EditPropertyValue($scope.SelectedParameter.DataType, $scope.SelectedParameter, "DataType", $scope.SelectedParameter.strDataType);

                $rootScope.EditPropertyValue($scope.SelectedParameter.Entity, $scope.SelectedParameter, "Entity", $scope.SelectedParameter.strEntity);

                $rootScope.EditPropertyValue($scope.SelectedParameter.Direction, $scope.SelectedParameter, "Direction", $scope.SelectedParameter.strDirection);

                $rootScope.EditPropertyValue($scope.SelectedParameter.Text, $scope.SelectedParameter, "Text", $scope.SelectedParameter.strText);

                $rootScope.EditPropertyValue($scope.SelectedParameter.DataFormat, $scope.SelectedParameter, "DataFormat", $scope.SelectedParameter.strDataFormat);
            }
            else {

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.ID, $scope.SelectedParameter.dictAttributes, "ID", strId);
                if ($scope.SelectedParameter.dictAttributes.ID == undefined) {
                    $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.ID, $scope.SelectedParameter.dictAttributes, "ID", '');
                }

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.sfwDataType, $scope.SelectedParameter.dictAttributes, "sfwDataType", $scope.SelectedParameter.strDataType);

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.sfwEntity, $scope.SelectedParameter.dictAttributes, "sfwEntity", $scope.SelectedParameter.strEntity);

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.sfwDirection, $scope.SelectedParameter.dictAttributes, "sfwDirection", $scope.SelectedParameter.strDirection);

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.Text, $scope.SelectedParameter.dictAttributes, "Text", $scope.SelectedParameter.strText);

                $rootScope.EditPropertyValue($scope.SelectedParameter.dictAttributes.sfwDataFormat, $scope.SelectedParameter.dictAttributes, "sfwDataFormat", $scope.SelectedParameter.strDataFormat);
            }
            if (dialogScope.isAdd && $scope.IsDecisionTable) {
                $rootScope.PushItem($scope.SelectedParameter, $scope.parameters);
            } else if (dialogScope.isAdd) {
                $rootScope.PushItem($scope.SelectedParameter, $scope.parameters.Elements);
            }
            $rootScope.UndRedoBulkOp("End");


            dialogScope.closeDialog();
        };
        dialogScope.openEntityClick = function (aEntityID) {
            if (aEntityID && aEntityID != "") {
                dialogScope.OnParameterOkClick();
                $NavigateToFileService.NavigateToFile(aEntityID, "", "");
            }
        };
        if (dialogScope.isAdd) {
            dialogScope.parameter.strDataType = "string";
            dialogScope.UpdateParameterDataType(dialogScope.parameter.strDataType);
        }
        var dialog = $rootScope.showDialog(dialogScope, dialogScope.isAdd ? "Add Parameter" : "Edit Parameter", "Rule/views/AddEditVariable.html");
    };

    $scope.menuOptionsForParameters = [
        ['Convert to Local Variable', function ($itemScope) {
            if ($itemScope.objParam != undefined) {
                if (confirm("Parameter : '" + $itemScope.objParam.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                    $rootScope.UndRedoBulkOp("Start");
                    var objnew = {
                        Name: 'variable',
                        value: $itemScope.objParam.value,
                        dictAttributes: {
                            ID: $itemScope.objParam.dictAttributes.ID,
                            sfwDataType: $itemScope.objParam.dictAttributes.sfwDataType,
                            sfwEntity: $itemScope.objParam.dictAttributes.sfwEntity,
                            Text: $itemScope.objParam.dictAttributes.Text,
                            sfwDataFormat: $itemScope.objParam.dictAttributes.sfwDataFormat,
                        },
                        Elements: []
                    };
                    $rootScope.PushItem(objnew, $scope.variables.Elements);

                    var index = $scope.variables.Elements.length - 1;
                    $scope.SelectedLocalVariable = $scope.variables.Elements[index];
                    $scope.RemoveParameters(true);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }, function ($itemScope) {
            if ($itemScope.objParam != undefined) {
                $scope.SelectedParameter = $itemScope.objParam;
            }
            if ($scope.objLogicalRule.dictAttributes && $scope.objLogicalRule.dictAttributes.sfwRuleType == "ExcelMatrix") {
                return false;
            } else {
                return true;
            }
        }], null,

    ];

    $scope.ShowParameters = function (event) {
        var params = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvParameters");
        $scope.IsLocalVariableVisible = false;
        $scope.IsParentRulesVisible = false;
        $scope.IsChildRulesVisible = false;
        if (params.is(':visible')) {
            params.hide("slide", { direction: "right" });
            $scope.IsParameterVisible = false;
        }
        else {
            params.show("slide", { direction: "right" });
            $scope.IsParameterVisible = true;
        }
        //setSelectionForCurrentPanel(event);
        closeOtherRightPanels("Parameters");
    };

    $scope.HideparameterLocalVars = function () {
        var params = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvParameters");
        $scope.IsLocalVariableVisible = false;
        if (params.is(':visible')) {
            params.hide("slide", { direction: "right" });
            $scope.IsParameterVisible = false;
        }
        closeOtherRightPanels("");
        var localVar = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvLocalVar");
        $scope.IsParameterVisible = false;
        if (localVar.is(':visible')) {
            localVar.hide("slide", { direction: "right" });
            $scope.IsLocalVariableVisible = false;
        }
    };
    $scope.ShowDecisionTableParameters = function (event) {
        var params = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvDecisionTableParameters");
        if (params.is(':visible')) {
            params.hide("slide", { direction: "right" });
            $scope.IsParameterVisible = false;
        }
        else {
            params.show("slide", { direction: "right" });
            $scope.IsParameterVisible = true;
        }
        // setSelectionForCurrentPanel(event);
        closeOtherRightPanels("DecisionTableParameters");
    };

    $scope.showChildRules = function (event) {
        var childRules = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvChildRules");
        $scope.IsLocalVariableVisible = false;
        $scope.IsParentRulesVisible = false;
        $scope.IsParameterVisible = false;
        if (childRules.is(':visible')) {
            childRules.hide("slide", { direction: "right" });
            $scope.IsChildRulesVisible = false;
        }
        else {
            childRules.show("slide", { direction: "right" });
            $scope.IsChildRulesVisible = true;
        }
        //setSelectionForCurrentPanel(event);
        closeOtherRightPanels("ChildRules");
    };

    $scope.showParentRules = function (event) {
        var parentRules = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#dvParentRules");
        $scope.IsLocalVariableVisible = false;
        $scope.IsParameterVisible = false;
        $scope.IsChildRulesVisible = false;
        if (parentRules.is(':visible')) {
            parentRules.hide("slide", { direction: "right" });
            $scope.IsParentRulesVisible = false;
        }
        else {
            parentRules.show("slide", { direction: "right" });
            $scope.IsParentRulesVisible = true;
        }
        //setSelectionForCurrentPanel(event);
        closeOtherRightPanels("ParentRules");
    };
    //#endregion

    //#region Excel Matrix methods
    $scope.prepareExcelMatrix = function () {
        $scope.objSelectedLogicalRule.objColumnValues = [];
        $scope.objSelectedLogicalRule.objExcelColumnValues = [];
        $scope.objSelectedLogicalRule.objHeaderRowValues = [];
        $scope.objSelectedLogicalRule.objHeaderColumnValues = [];
        $scope.objSelectedLogicalRule.objHeaderRowTitle = "";

        var objExcelMatrixRule = $scope.objSelectedLogicalRule;
        for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
            if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
                var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

                for (var row = 0; row < objExcelMatrixRows.Elements.length; row++) {
                    if (objExcelMatrixRows.Elements[row].dictAttributes.ID == 0) {
                        var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;
                        if (strColumnValues) {
                            if (objExcelMatrixRows.Elements[row].dictAttributes.Value != undefined) {
                                $scope.objSelectedLogicalRule.objHeaderRowTitle = objExcelMatrixRows.Elements[row].dictAttributes.Value;
                            }

                            var columns = [];
                            columns = strColumnValues.split($scope.objSelectedLogicalRule.dictAttributes.sfwDelimiter);

                            $scope.objSelectedLogicalRule.objHeaderColumnValues.push(columns);
                        }
                    }
                    else {
                        var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;
                        if (strColumnValues) {
                            var columns = [];
                            columns = strColumnValues.split($scope.objSelectedLogicalRule.dictAttributes.sfwDelimiter);
                            $scope.objSelectedLogicalRule.objExcelColumnValues.push(columns);
                            $scope.objSelectedLogicalRule.objHeaderRowValues.push(objExcelMatrixRows.Elements[row].dictAttributes.Value);
                        }
                    }
                }
            }
        }
        if ($scope.objSelectedLogicalRule.objExcelColumnValues.length > 25) {
            for (var i = 0; i < 25; i++) {
                $scope.objSelectedLogicalRule.objColumnValues.push($scope.objSelectedLogicalRule.objExcelColumnValues[i]);
            }
        }
        else {
            $scope.objSelectedLogicalRule.objColumnValues = $scope.objSelectedLogicalRule.objExcelColumnValues;
        }

    };

    $scope.LoadSomeRowsinExcelMatrix = function () {
        if ($scope.objSelectedLogicalRule.objColumnValues) {
            var startingpostion = $scope.objSelectedLogicalRule.objColumnValues.length;
            count = 0;
            for (var i = startingpostion; i < $scope.objSelectedLogicalRule.objExcelColumnValues.length; i++) {
                if (count == 5) {
                    break;
                }
                else {
                    $scope.objSelectedLogicalRule.objColumnValues.push($scope.objSelectedLogicalRule.objExcelColumnValues[i]);
                }
                count++;
            }
        }
    };

    //#endregion

    //#region Child/Parent Rules
    $scope.updateChildRules = function (fromUndoRedoBlock) {
        $scope.isChildRulesLoading = true;
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.childRules, $scope, "childRules", getChildRules($scope.objSelectedLogicalRule));
        }
        else {
            $scope.childRules = getChildRules($scope.objSelectedLogicalRule);
        }
        $scope.isChildRulesLoading = false;
    };
    $scope.updateParentRules = function () {
        $scope.isParentRulesLoading = true;
        var isStatic = false;
        var entityID = "";
        var ruleType = "";
        var ruleID = "";

        if ($scope.objLogicalRule.dictAttributes) {
            ruleID = $scope.objLogicalRule.dictAttributes.ID;
            entityID = $scope.objLogicalRule.dictAttributes.sfwEntity;
            ruleType = $scope.objLogicalRule.dictAttributes.sfwRuleType;

            if ($scope.objLogicalRule.dictAttributes.sfwStatic && $scope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == 'true') {
                isStatic = true;
            }
            else {
                isStatic = false;
            }
        }
        else {
            ruleID = $scope.objLogicalRule.RuleID;
            entityID = $scope.objLogicalRule.Entity;
            ruleType = $scope.objLogicalRule.RuleType;

            if ($scope.objLogicalRule.Static && $scope.objLogicalRule.Static.toLowerCase() == 'true') {
                isStatic = true;
            }
            else {
                isStatic = false;
            }
        }

        $.connection.hubRuleModel.server.getParentRules(ruleID, ruleType, isStatic, entityID).done(function (data) {
            $scope.$evalAsync(function () {
                $scope.parentRules = data;
                $scope.isParentRulesLoading = false;
            });
        });
    };
    $scope.openRule = function (ruleID) {
        //closeOtherRightPanels()
        $.connection.hubMain.server.navigateToFile(ruleID, "").done(function (objfile) {
            $rootScope.openFile(objfile, undefined);
        });
    };
    //#endregion

    //#region Add/Navigate To Scenario

    $scope.UpdateScenarioDetails = function () {

        if ($scope.objLogicalRule != undefined) {
            var id;
            var entityname = "";
            if ($scope.currentfile.FileType == "DecisionTable") {
                entityname = $scope.objLogicalRule.Entity;
                id = $scope.objLogicalRule.RuleID;
            }
            else {
                entityname = $scope.objLogicalRule.dictAttributes.sfwEntity;
                id = $scope.objLogicalRule.dictAttributes.ID;
            }



            if (entityname != undefined && entityname != "") {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var entity = entities.filter(function (x) {
                    return x.ID == entityname;
                });
                if (entity && entity.length > 0) {
                    var busobjName = "";
                    if (entity[0].BusinessObjectName) {
                        busobjName = entity[0].BusinessObjectName;
                    }
                    $scope.receiveEntityBusinessObject(busobjName);
                }
            }
        }
    };

    $scope.SetParameterScenario = function (data) {
        $scope.$apply(function () {
            $scope.HasRelatedParameterBasedScenario = true;
            $scope.objLogicalRule.objExtraData.RelatedParameterBasedScenarioFile = data;
            $scope.objLogicalRule.objExtraData.HasRelatedParameterBasedScenario = true;
        });
    };

    $scope.NavigateToScenario = function (scenarioType) {
        var scenarioFileModel;
        if (scenarioType == "ParameterScenario") {
            scenarioFileModel = $scope.objLogicalRule.objExtraData.RelatedParameterBasedScenarioFile;
        }

        else if (scenarioType == "ObjectScenario") {
            scenarioFileModel = $scope.objLogicalRule.objExtraData.RelatedObjectBasedScenarioFile;
        }
        else if (scenarioType == "ExcelScenario") {
            scenarioFileModel = $scope.objLogicalRule.objExtraData.RelatedExcelBasedScenarioFile;
        }
        if (scenarioFileModel) {
            $rootScope.openFile(scenarioFileModel);
        }

    };

    $scope.AddNewScenario = function (scenarioType, blnIsUpdateModeForExcel) {
        if ($scope.isDirty) {
            alert('Save Rule file before creating scenario.');
        }
        else {
            var RuleId;
            var EntityName;

            if ($scope.objLogicalRule.dictAttributes == undefined) {
                RuleId = $scope.objLogicalRule.RuleID;
                EntityName = $scope.objLogicalRule.Entity;
            }
            else {
                RuleId = $scope.objLogicalRule.dictAttributes.ID;
                EntityName = $scope.objLogicalRule.dictAttributes.sfwEntity;
            }
            if (ConfigurationFactory.getLastProjectDetails() && ConfigurationFactory.getLastProjectDetails().ScenarioXmlFileLocation) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });

                $.connection.hubRuleModel.server.createNewScenario(scenarioType, EntityName, RuleId, $scope.currentfile.FilePath, blnIsUpdateModeForExcel).done(function (data) {
                    $scope.$apply(function () {
                        if (!blnIsUpdateModeForExcel) {
                            var objSceanrioDetail = data;

                            if (scenarioType != 'ExcelScenario') {
                                $rootScope.openFile(objSceanrioDetail);
                            }

                            if (scenarioType == 'ParameterScenario') {
                                $scope.HasRelatedParameterBasedScenario = true;
                                $scope.objLogicalRule.objExtraData.RelatedParameterBasedScenarioFile = objSceanrioDetail;
                            }
                            else if (scenarioType == 'ObjectScenario') {
                                $scope.HasRelatedObjectBasedScenario = true;

                                $scope.objLogicalRule.objExtraData.RelatedObjectBasedScenarioFile = objSceanrioDetail;
                            }
                            else if (scenarioType == 'ExcelScenario') {
                                $scope.HasRelatedXLBasedScenario = true;
                                $scope.objLogicalRule.objExtraData.RelatedExcelBasedScenarioFile = objSceanrioDetail;
                            }
                        }
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                        });
                    });
                });
            }
            else {
                toastr.warning("Check the Scenario XML File Location in settings file.");
            }
        }
        //closeScenarioToggle();
    };

    //#endregion

    //#region Fix all Node Ids
    $scope.FixAllNodeIds = function () {
        if ($scope.isDirty) {
            alert('Please save the file First.');
        }
        else {
            $rootScope.IsLoading = true;
            $.connection.hubRuleModel.server.fixAllNodeIds($scope.currentfile).done(function (model) {
                if (model) {
                    $scope.receiverulemodel(model);
                }
            });

            $rootScope.IsLoading = false;
            alert('All node ids are fixed.');
        }
    }

    //#endregion

    //#region Rule Details/Extra Fields 
    var isEntityIdChanged = false;
    var oldentityid = "";
    $scope.RuleDetailsOkClick = function () {

        $rootScope.UndRedoBulkOp("Start");
        if (!$scope.IsDecisionTable) {
            if ($scope.objLogicalRule.dictAttributes.sfwEntity != this.sfwEntity) {
                oldentityid = $scope.objLogicalRule.dictAttributes.sfwEntity;
                isEntityIdChanged = true;
            }
            $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwEntity, $scope.objLogicalRule.dictAttributes, "sfwEntity", this.sfwEntity);

            $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.Text, $scope.objLogicalRule.dictAttributes, "Text", this.Text);

            $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwReturnType, $scope.objLogicalRule.dictAttributes, "sfwReturnType", this.sfwReturnType);

            $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwStatus, $scope.objLogicalRule.dictAttributes, "sfwStatus", this.sfwStatus);

            $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwNeoTrackID, $scope.objLogicalRule.dictAttributes, "sfwNeoTrackID", this.sfwNeoTrackID);

            if ($scope.objLogicalRule.dictAttributes.sfwReturnType == 'List' || $scope.objLogicalRule.dictAttributes.sfwReturnType == 'Object' || $scope.objLogicalRule.dictAttributes.sfwReturnType == 'Collection') {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwReturnEntity, $scope.objLogicalRule.dictAttributes, "sfwReturnEntity", this.sfwReturnEntity);
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwReturnEntity, $scope.objLogicalRule.dictAttributes, "sfwReturnEntity", "");
            }

            if (this.sfwPrivate == undefined || this.sfwPrivate == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwPrivate, $scope.objLogicalRule.dictAttributes, "sfwPrivate", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwPrivate, $scope.objLogicalRule.dictAttributes, "sfwPrivate", "True");
            }

            if (this.sfwTrace == undefined || this.sfwTrace == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwTrace, $scope.objLogicalRule.dictAttributes, "sfwTrace", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwTrace, $scope.objLogicalRule.dictAttributes, "sfwTrace", "True");
            }

            if (this.sfwStatic == undefined || this.sfwStatic == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwStatic, $scope.objLogicalRule.dictAttributes, "sfwStatic", "False");
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwCacheResult, $scope.objLogicalRule.dictAttributes, "sfwCacheResult", "False");

            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwStatic, $scope.objLogicalRule.dictAttributes, "sfwStatic", "True");

                if (this.sfwCacheResult == undefined || this.sfwCacheResult == false) {
                    $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwCacheResult, $scope.objLogicalRule.dictAttributes, "sfwCacheResult", "False");
                }
                else {
                    $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwCacheResult, $scope.objLogicalRule.dictAttributes, "sfwCacheResult", "True");
                }
            }


            if ($scope.objLogicalRule.dictAttributes.sfwRuleType == 'ExcelMatrix') {
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.rowHeader, $scope.objLogicalRule.dictAttributes, "rowHeader", this.rowHeader);
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.columnHeader, $scope.objLogicalRule.dictAttributes, "columnHeader", this.columnHeader);
                $rootScope.EditPropertyValue($scope.objLogicalRule.dictAttributes.sfwDefaultValue, $scope.objLogicalRule.dictAttributes, "sfwDefaultValue", this.sfwDefaultValue);

            }

            // #region extra field data
            if ($scope.objDirFunctions.prepareExtraFieldData) {
                $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            //#endregion

            if ($scope.objLogicalRule.dictAttributes.sfwStatic && $scope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "true") {
                $scope.ShowMethod = false;
            }
            else {
                if ($scope.IsBusObjPresent) {
                    $scope.ShowMethod = true;
                }
            }

        }
        else if ($scope.IsDecisionTable) {
            if ($scope.objLogicalRule.Entity != this.Entity) {
                oldentityid = $scope.objLogicalRule.Entity;
                isEntityIdChanged = true;
            }
            $rootScope.EditPropertyValue($scope.objLogicalRule.Entity, $scope.objLogicalRule, "Entity", this.Entity);

            $rootScope.EditPropertyValue($scope.objLogicalRule.Description, $scope.objLogicalRule, "Description", this.Description);

            if (this.MatchAllConditions == undefined || this.MatchAllConditions == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.MatchAllConditions, $scope.objLogicalRule, "MatchAllConditions", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.MatchAllConditions, $scope.objLogicalRule, "MatchAllConditions", "True");
            }

            if (this.ThrowError == undefined || this.ThrowError == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.ThrowError, $scope.objLogicalRule, "ThrowError", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.ThrowError, $scope.objLogicalRule, "ThrowError", "True");
            }

            $rootScope.EditPropertyValue($scope.objLogicalRule.ReturnType, $scope.objLogicalRule, "ReturnType", this.ReturnType);

            $rootScope.EditPropertyValue($scope.objLogicalRule.Status, $scope.objLogicalRule, "Status", this.Status);

            $rootScope.EditPropertyValue($scope.objLogicalRule.NeoTrackID, $scope.objLogicalRule, "NeoTrackID", this.NeoTrackID);

            if (this.Private == undefined || this.Private == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Private, $scope.objLogicalRule, "Private", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Private, $scope.objLogicalRule, "Private", "True");
            }

            if (this.Trace == undefined || this.Trace == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Trace, $scope.objLogicalRule, "Trace", "False");
            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Trace, $scope.objLogicalRule, "Trace", "True");
            }

            if (this.Static == undefined || this.Static == false) {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Static, $scope.objLogicalRule, "Static", "False");
                $rootScope.EditPropertyValue($scope.objLogicalRule.CacheResult, $scope.objLogicalRule, "CacheResult", "False");

            }
            else {
                $rootScope.EditPropertyValue($scope.objLogicalRule.Static, $scope.objLogicalRule, "Static", "True");

                if (this.CacheResult == undefined || this.CacheResult == false) {
                    $rootScope.EditPropertyValue($scope.objLogicalRule.CacheResult, $scope.objLogicalRule, "CacheResult", "False");
                }
                else {
                    $rootScope.EditPropertyValue($scope.objLogicalRule.CacheResult, $scope.objLogicalRule, "CacheResult", "True");
                }
            }

            // #region extra field data
            if ($scope.objDirFunctions.prepareExtraFieldData) {
                $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                //if ($scope.objDirFunctions.getNewExtraFieldData) {
                //    $scope.objRuleExtraFields = $scope.objDirFunctions.getNewExtraFieldData();
                //}
            }
            //#endregion


            if ($scope.objLogicalRule.Static && $scope.objLogicalRule.Static.toLowerCase() == "true") {
                $scope.ShowMethod = false;
            }
        }


        // updating the intellisense list
        if ($scope.objLogicalRule.dictAttributes != undefined) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            for (var i = 0; i < entityIntellisenseList.length; i++) {
                if (entityIntellisenseList[i].ID == $scope.objLogicalRule.dictAttributes.sfwEntity) {
                    var entity = entityIntellisenseList[i];
                    for (var j = 0; j < entityIntellisenseList[i].Rules.length; j++) {
                        if (entityIntellisenseList[i].Rules[j].ID == $scope.objLogicalRule.dictAttributes.ID) {
                            var rule = entityIntellisenseList[i].Rules[j];
                            $rootScope.EditPropertyValue(entityIntellisenseList[i].Rules[j].IsStatic, entityIntellisenseList[i].Rules[j], "IsStatic", this.sfwStatic);
                            $rootScope.EditPropertyValue(entityIntellisenseList[i].Rules[j].IsPrivate, entityIntellisenseList[i].Rules[j], "IsPrivate", this.sfwPrivate);
                            $rootScope.EditPropertyValue(entityIntellisenseList[i].Rules[j].ReturnType, entityIntellisenseList[i].Rules[j], "ReturnType", this.sfwReturnType);

                            break;
                        }
                    }
                    break;
                }
            }
        }

        $rootScope.UndRedoBulkOp("End");
        ruledetailsdialogid = "";
        if (objNewDialog) {
            objNewDialog.close();
        }
    };

    $scope.NavigateToEntity = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            //$scope.RuleDetailsOkClick();
            $NavigateToFileService.NavigateToFile(aEntityID, "", "");
        }
    };
    $scope.receieveMovedRuleFile = function (data) {
        var lstFileDetails = JSON.parse(data);
        $rootScope.isFilesListUpdated = true;
        var newentityid = "";
        if ($scope.IsDecisionTable) {
            ruleid = $scope.objLogicalRule.RuleID;
            newentityid = $scope.objLogicalRule.Entity;
        }
        else {
            ruleid = $scope.objLogicalRule.dictAttributes.ID;
            newentityid = $scope.objLogicalRule.dictAttributes.sfwEntity;
        }
        // update current file object with new object (with new file path)
        for (var i = 0; i < lstFileDetails.length; i++) {
            if (lstFileDetails[i].FileName === $scope.currentfile.FileName) {
                $scope.currentfile.FilePath = lstFileDetails[i].FilePath;
                break;
            }
        }
        var entityScenarioDetails;
        var objectScenarioDetails;
        var datasourceScenario;
        var xlScnarioDetails;
        var ruleDetails;
        if (lstFileDetails && lstFileDetails.length > 0) {
            $scope.updateUserFiles(lstFileDetails);
        }
    };

    $scope.updateUserFiles = function (lstFileDetails) {
        var userCookie = $cookies.getObject("UserDetails");
        for (var i = 0; i < lstFileDetails.length; i++) {
            lstFileDetails[i].UserId = JSON.parse(userCookie).UserID;
            lstFileDetails[i].Data = null;
        }
        var filesdataPromise = $DashboardFactory.updateUserFiles(lstFileDetails);
        filesdataPromise.then(function (result) {
            if (result) {
                for (var i = 0; i < lstFileDetails.length; i++) {
                    $rootScope.updateUserFileDetails(lstFileDetails[i]);
                    var files = $rootScope.lstopenedfiles.filter(function (x) {
                        return x.file.FileName == lstFileDetails[i].FileName;
                    });
                    if (files && files.length > 0) $rootScope.closeFile(lstFileDetails[i].FileName);
                }
            }
        });
    };

    var objNewDialog;
    $scope.onDetailClick = function () {
        var newScope = $scope.$new();
        //Extra Field varibles
        newScope.objExtraFields = [];
        $scope.objDirFunctions = {
        };
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Rule";

        if (!$scope.IsDecisionTable) {
            newScope.ID = $scope.objLogicalRule.dictAttributes.ID;

            newScope.sfwEntity = $scope.objLogicalRule.dictAttributes.sfwEntity;
            newScope.Text = $scope.objLogicalRule.dictAttributes.Text;
            newScope.sfwReturnType = $scope.objLogicalRule.dictAttributes.sfwReturnType;
            newScope.sfwReturnEntity = $scope.objLogicalRule.dictAttributes.sfwReturnEntity;
            newScope.sfwStatus = $scope.objLogicalRule.dictAttributes.sfwStatus;
            newScope.sfwNeoTrackID = $scope.objLogicalRule.dictAttributes.sfwNeoTrackID;

            if ($scope.objLogicalRule.dictAttributes.sfwCacheResult == "True" || $scope.objLogicalRule.dictAttributes.sfwCacheResult == "true") {
                newScope.sfwCacheResult = true;
            }
            else {
                newScope.sfwCacheResult = false;
            }

            if ($scope.objLogicalRule.dictAttributes.sfwStatic == "True" || $scope.objLogicalRule.dictAttributes.sfwStatic == "true") {
                newScope.sfwStatic = true;
            }
            else {
                newScope.sfwStatic = false;
            }
            if ($scope.objLogicalRule.dictAttributes.sfwPrivate == "True" || $scope.objLogicalRule.dictAttributes.sfwPrivate == "true") {
                newScope.sfwPrivate = true;
            }
            else {
                newScope.sfwPrivate = false;
            }

            if ($scope.objLogicalRule.dictAttributes.sfwTrace == "True" || $scope.objLogicalRule.dictAttributes.sfwTrace == "true") {
                newScope.sfwTrace = true;
            }
            else {
                newScope.sfwTrace = false;
            }

            if ($scope.objLogicalRule.dictAttributes.sfwRuleType == 'ExcelMatrix') {
                newScope.rowHeader = $scope.objLogicalRule.dictAttributes.rowHeader;
                newScope.columnHeader = $scope.objLogicalRule.dictAttributes.columnHeader;
                newScope.sfwDefaultValue = $scope.objLogicalRule.dictAttributes.sfwDefaultValue;
            }
        }
        else if ($scope.IsDecisionTable) {

            newScope.RuleID = $scope.objLogicalRule.RuleID;

            newScope.Entity = $scope.objLogicalRule.Entity;
            newScope.Description = $scope.objLogicalRule.Description;
            if ($scope.objLogicalRule.MatchAllConditions == "True" || $scope.objLogicalRule.MatchAllConditions == "true") {
                newScope.MatchAllConditions = true;
            }
            else {
                newScope.MatchAllConditions = false;
            }

            if ($scope.objLogicalRule.ThrowError == "True" || $scope.objLogicalRule.ThrowError == "true") {
                newScope.ThrowError = true;
            }
            else {
                newScope.ThrowError = false;
            }

            newScope.ReturnType = $scope.objLogicalRule.ReturnType;
            newScope.Status = $scope.objLogicalRule.Status;
            newScope.NeoTrackID = $scope.objLogicalRule.NeoTrackID;
            if ($scope.objLogicalRule.CacheResult == "true" || $scope.objLogicalRule.CacheResult == "True") {
                newScope.CacheResult = true;
            }
            else {
                newScope.CacheResult = false;
            }

            if ($scope.objLogicalRule.Static == "true" || $scope.objLogicalRule.Static == "True") {
                newScope.Static = true;
            }
            else {
                newScope.Static = false;
            }
            if ($scope.objLogicalRule.Private == "True" || $scope.objLogicalRule.Private == "true") {
                newScope.Private = true;
            }
            else {
                newScope.Private = false;
            }

            if ($scope.objLogicalRule.Trace == "True" || $scope.objLogicalRule.Trace == "true") {
                newScope.Trace = true;
            }
            else {
                newScope.Trace = false;
            }

        }

        newScope.SelectedRuleDetailsDialogTab = 'Details';
        newScope.ID = undefined;

        objNewDialog = $rootScope.showDialog(newScope, "Rule Details", "Rule/views/RuleDetails.html", { height: 600, width: 660 });

        newScope.selectRuleDetailsDialogTab = function () {
            newScope.SelectedRuleDetailsDialogTab = 'Details';
        };
        newScope.selectRuleExtraFieldsDialogTab = function () {
            newScope.SelectedRuleDetailsDialogTab = 'ExtraFields';
        };

        newScope.onReturnTypeChange = function () {
            newScope.sfwReturnEntity = "";
        };


        newScope.validateRuleDetails = function () {
            newScope.RuleDetailsErrorMessage = undefined;
            var flag = false;

            if (!newScope.IsDecisionTable) {
                if (!newScope.sfwEntity) {
                    newScope.RuleDetailsErrorMessage = "Entity is mandatory.";
                    return true;
                }

                if (newScope.sfwReturnType == "Collection" || newScope.sfwReturnType == "Object" || newScope.sfwReturnType == "List") {
                    if (!newScope.sfwReturnEntity) {
                        newScope.RuleDetailsErrorMessage = "Enter Return Entity Type.";
                        return true;
                    }
                }
            }
            else {

                if (!newScope.Entity) {
                    newScope.RuleDetailsErrorMessage = "Entity is mandatory.";
                    return true;
                }

                if (newScope.ReturnType == "Collection" || newScope.ReturnType == "Object" || newScope.ReturnType == "List") {
                    if (!newScope.sfwReturnEntity) {
                        newScope.RuleDetailsErrorMessage = "Enter Return Entity Type.";
                        return true;
                    }
                }

            }

            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var extraValueFlag = validateExtraFields(newScope);
            if (extraValueFlag) {
                newScope.RuleDetailsErrorMessage = newScope.FormDetailsErrorMessage;
                return true;
            }


            if (newScope.RuleDetailsErrorMessage != undefined) {
                return true;
            }

            return false;
        };

        newScope.closeRuleDetails = function () {
            objNewDialog.close();
            //ngDialog.close(ruledetailsdialogid);
        };
    };


    $scope.updateView = function () {
        var selectedView = $scope.ViewMode;
        if ($scope.objSelectedLogicalRule != undefined) {
            var data = $scope.objLogicalRule;
            var ruleType = "";
            if (data.dictAttributes !== undefined) {
                ruleType = data.dictAttributes.sfwRuleType;
            }
            else if (data.RuleType !== undefined) {
                ruleType = data.RuleType;
            }
            else {
                alert("Rule Type not found.");
                return;
            }

            if (selectedView == "Analyst View") {
                setAnalystView(ruleType);
            }
            else {
                setDeveloperView(ruleType);
            }
        }
    };

    $scope.showDescriptiontext = function () {
        $scope.isDescriptionText = true;
    };
    $scope.showStatusOptions = function () {
        $scope.isStatusOption = true;
    };
    $scope.focusOutFromDescriptionText = function () {
        $scope.isDescriptionText = false;
    };

    $scope.focusOutFromStatus = function () {
        $scope.isStatusOption = false;
    };

    //#endregion

    //#region Intellisense related methods.
    $scope.getVarParamAttributes = function (types, includePrivate, entityname) {
        if (!types) {
            types = [];
        }

        var data = [];

        var parameters = [];
        var localVariables = [];
        var rootScope = getCurrentFileScope();
        var parameterElements = [];
        var variableElements = [];
        if (rootScope.IsDecisionTable) {
            if (rootScope.parameters) {
                parameterElements = rootScope.parameters;
            }
        }
        else {
            if (rootScope.parameters && rootScope.parameters.Elements) {
                parameterElements = rootScope.parameters.Elements;
            }
            if (rootScope.variables && rootScope.variables.Elements) {
                variableElements = rootScope.variables.Elements;
            }
        }

        for (var i = 0; i < parameterElements.length; i++) {
            if (parameterElements[i].dictAttributes) {
                if (types.length == 0 || types.indexOf(parameterElements[i].dictAttributes.sfwDataType) > -1) {
                    parameters.push({
                        ID: parameterElements[i].dictAttributes.ID,
                        DisplayName: parameterElements[i].dictAttributes.ID,
                        Value: parameterElements[i].dictAttributes.ID,
                        Tooltip: parameterElements[i].dictAttributes.ID,
                        DataType: parameterElements[i].dictAttributes.sfwDataType,
                        Direction: parameterElements[i].dictAttributes.sfwDirection,
                        Entity: parameterElements[i].dictAttributes.sfwEntity
                    });
                }
            }
            else {
                if (types.length == 0 || types.indexOf(parameterElements[i].DataType) > 0) {
                    parameters.push({
                        ID: parameterElements[i].ParamID,
                        DisplayName: parameterElements[i].ParamID,
                        Value: parameterElements[i].ParamID,
                        Tooltip: parameterElements[i].ParamID,
                        DataType: parameterElements[i].DataType,
                        Direction: parameterElements[i].Direction,
                        Entity: parameterElements[i].Entity
                    });
                }
            }
        }
        for (var i = 0; i < variableElements.length; i++) {
            if (types.length == 0 || types.indexOf(variableElements[i].dictAttributes.sfwDataType) > -1) {
                localVariables.push({
                    ID: variableElements[i].dictAttributes.ID,
                    DisplayName: variableElements[i].dictAttributes.ID,
                    Value: variableElements[i].dictAttributes.ID,
                    Tooltip: variableElements[i].dictAttributes.ID,
                    DataType: variableElements[i].dictAttributes.sfwDataType,
                    Entity: variableElements[i].dictAttributes.sfwEntity,
                });
            }
        }
        data = parameters.concat(localVariables);
        if (rootScope.objLogicalRule.dictAttributes) {
            if (rootScope.objLogicalRule.dictAttributes.sfwStatic && rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() != "true") {
                var entityName;
                if (entityname != undefined && entityname != null) {
                    entityName = entityname;
                }
                else {
                    entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                var attributes = $rootScope.getEntityAttributeIntellisense(entityName, includePrivate);
                attributes = attributes.filter(function (x) {
                    if (types.length == 0 || types.indexOf(x.DataType) > -1) {
                        return x;
                    }
                });
                data = data.concat(attributes);
            }
        }
        else {
            if (rootScope.objLogicalRule.Static.toLowerCase() != "true") {
                var entityName;
                if (entityname != undefined && entityname != null) {
                    entityName = entityname;
                }
                else {
                    entityName = rootScope.objLogicalRule.Entity;
                }
                data = data.concat($rootScope.getEntityAttributeIntellisense(entityName, includePrivate));
            }
        }

        return data;
    };
    $scope.getFirstLevelIntellisense = function () {
        var data = $scope.getVarParamAttributes(undefined, true);
        data = data.concat($resourceFactory.getConstantsList());
        data.push({ ID: "RFunc", DisplayName: "RFunc", Value: "RFunc", Tooltip: "RFunc" });
        if ($scope.customTypes && $scope.customTypes.length > 0) {
            for (var idx = 0, len = $scope.customTypes.length; idx < len; idx++) {
                data.push({ ID: $scope.customTypes[idx], DisplayName: $scope.customTypes[idx], Value: $scope.customTypes[idx], Tooltip: $scope.customTypes[idx], isCustomType: true });
            }
        }
        //data = data.concat($resourceFactory.getRfunctionsList());
        return data;
    };


    function GetText(text, caretIndex) {
        var reVal = "";
        var res = "";

        var ind = caretIndex;
        for (; ind >= 0; ind--) {
            if (isMathope(text.charAt(ind)) || text.charAt(ind) == '.')
                break;
            res += text.charAt(ind);
        }

        if (res.length > 0) {
            reVal = res.split("").reverse().join("");//res.ToString().Reverse();
        }

        return reVal;

    }

    function IsBuisnessObjectNamesReq(text, caretIndex) {
        var retVal = false;

        var stIndex = text.indexOf("NewObject");

        if (-1 != stIndex) {
            var index = stIndex + "NewObject".length + 1;

            if (caretIndex >= index) {
                retVal = true;
            }
        }
        else {
            stIndex = text.indexOf("NewCollection");
            if (-1 != stIndex) {
                var index = stIndex + "NewCollection".length + 1;
                if (caretIndex >= index) {
                    retVal = true;
                }
            }
            else {
                stIndex = text.indexOf("NewList");
                if (-1 != stIndex) {
                    var index = stIndex + "NewList".length + 1;
                    if (caretIndex >= index) {
                        retVal = true;
                    }
                }
            }
        }

        return retVal;
    }
    function checkForBracketsForExpression(strText) {
        var OpeningCount = 0;
        var ClosingCount = 0;
        if (strText) {
            for (var i = 0; i < strText.length; i++) {
                if (strText[i] == "(") {
                    OpeningCount++;
                } else if (strText[i] == ")") {
                    ClosingCount++;
                }
            }
        }

        return OpeningCount == ClosingCount;
    }
    $scope.onActionKeyDown = function (eargs) {
        var rootScope = getCurrentFileScope();
        var input = eargs.target;

        //if (arrText[arrText.length - 1] == "") {
        //    arrText.pop();
        //}

        if (rootScope.objLogicalRule.dictAttributes) {
            var entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
        }
        else {
            var entityName = rootScope.objLogicalRule.Entity;
        }
        var data = [];
        data = $scope.getFirstLevelIntellisense();
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entities = entityIntellisenseList;
        var entity = entities.filter(function (x) {
            return x.ID == entityName;
        });

        //If current rule is non-static then load other non-static rules of the current entity.
        var isstatic = "";
        if (rootScope.objLogicalRule.dictAttributes) {
            isstatic = rootScope.objLogicalRule.dictAttributes.sfwStatic;
        }
        else {
            isstatic = rootScope.objLogicalRule.Static;
        }

        if (entity[0] && entity[0].ParentId != "" && isstatic.toLowerCase() != "true") {
            var parententity = entity[0].ParentId;
            while (parententity != "") {
                data = data.concat($rootScope.getEntityAttributeIntellisense(parententity, false));
                var entity = entities.filter(function (x) {
                    return x.ID == parententity;
                });
                if (entity[0]) {
                    parententity = entity[0].ParentId;
                } else {
                    parententity = "";
                }
            }
        }

        $scope.GetAliasObjects($scope.objSelectedLogicalRule, true);
        $scope.CheckForLoopAndGetLoopParameters(rootScope.SelectedNode, true, $scope.objSelectedLogicalRule);
        data = data.concat($scope.lstSelectedAliasobjCollection);
        var cursorIndex = getCaretCharacterOffsetWithin(input);
        var arrText = getSplitArray(input.innerText, cursorIndex);
        var strInnerText = input.innerText.substring(0, cursorIndex);
        if (strInnerText && strInnerText.match(".*RFunc")) {
            var rfuncLatIndex = strInnerText.lastIndexOf("RFunc");
            var strExpression = input.innerText.substring(rfuncLatIndex, cursorIndex);
            var isValidcheck = checkForBracketsForExpression(strExpression);
            if (!isValidcheck) {
                var nodePath = GetText(input.innerText.substring(0, input.innerText.length - 1), getCaretCharacterOffsetWithin(input));

                if (nodePath == "NewCollection" || nodePath == "NewObject" || nodePath == "NewList") {
                    data = entities;
                }
                else if (IsBuisnessObjectNamesReq(input.innerText, getCaretCharacterOffsetWithin(input))) {
                    data = entities;
                }
            }
        }

        var aliasArray = [];
        var arrayindex = 0;
        var isAlias = false;
        if (arrText.length > 1) {
            for (var i = 0; i < arrText.length; i++) {
                for (var j = 0; j < $scope.lstSelectedAliasobjCollection.length; j++) {
                    if (arrText[i] == $scope.lstSelectedAliasobjCollection[j].ID) {
                        arrayindex = i;
                        isAlias = true;
                        arrText.splice(i, 1);
                        aliasArray = $scope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                        break;
                    }
                }
            }
        }

        for (var k = aliasArray.length - 1; k >= 0; k--) {
            arrText.splice(arrayindex, 0, aliasArray[k]);
        }
        var alreadySet = false;
        if (!isAlias) {
            if (arrText.length > 0) {
                for (var index = 0; index < arrText.length; index++) {
                    var item = data.filter(function (x) { return x.ID == arrText[index]; });
                    if (item.length > 0) {
                        if (item[0].Type && item[0].ID == arrText[index] && item[0].Type == "Constant") {
                            if (item[0].Type == "Constant") {
                                data = $resourceFactory.getConstantsList(arrText.join("."));
                                break;
                            }
                        }
                        else if (item[0].ID == "RFunc" && arrText[index] == "RFunc") {
                            data = $resourceFactory.getRfunctionsList();
                        }
                        else if (item[0].isCustomType) {
                            hubMain.server.getCustomTypeMethods(item[0].ID).done(function (customTypeMethods) {
                                data = customTypeMethods;
                                $scope.setRuleIntellisenseData(data, arrText, input, eargs);
                            });
                            alreadySet = true;
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
                                    if (entity[0] && entity.length > 0) {
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
                    else {
                        if (index != arrText.length - 1) {
                            data = [];
                        }
                    }
                }
            }
        }
        else {
            //if ($scope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "false") {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList;
            var entityName = "";
            if (rootScope.objLogicalRule.dictAttributes) {
                entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
            }
            else {
                entityName = rootScope.objLogicalRule.Entity;
            }
            var entity = entities.filter(function (x) { return x.ID == entityName; });
            if (entity.length > 0) {
                var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                data = queries;
            }
            data = data.concat(rootScope.getVarParamAttributes(["Collection", "CDOCollection", "List", "Object"], true, entityName));

            if (entity && entity.length > 0 && entity[0].ParentId != "") {
                parententityname = entity[0].ParentId;
                while (parententityname != "") {
                    entity = entities.filter(function (x) { return x.ID == parententityname; });
                    if (entity && entity.length > 0) {
                        var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                        data = data.concat(queries);
                    }
                    var attributes = $rootScope.getEntityAttributeIntellisense(parententityname, false);

                    attributes = attributes.filter(function (y) {
                        return (y.DataType == "Collection" || y.DataType == "CDOCollection" || y.DataType == "Object" || y.DataType == "List");
                    });
                    data = data.concat(attributes);
                    if (entity && entity.length > 0) {
                        parententityname = entity[0].ParentId;
                    } else {
                        parententityname = "";
                    }
                }
            }

            //var datacollection = data;
            if (arrText.length > 0) {
                for (var index = 0; index < arrText.length; index++) {
                    var item = data.filter(function (x) { return x.ID == arrText[index]; });
                    if (item.length > 0) {
                        if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CODCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined" && item[0].ID == arrText[index] && index < arrText.length - 1) {
                            data = [];
                            parententityname = item[0].Entity;
                            while (parententityname != "") {
                                entity = entities.filter(function (x) { return x.ID == parententityname; });
                                data = data.concat($rootScope.getEntityAttributeIntellisense(parententityname, false));
                                if (entity && entity.length > 0) {
                                    parententityname = entity[0].ParentId;
                                } else {
                                    parententityname = "";
                                }
                            }
                        }
                        else if (typeof item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && item[0].DataType != "string" && index < arrText.length - 1) {
                            if (item[0].isQueryfieldsLoaded) {
                                data = item[0].lstQueryFields;
                            } else {
                                rootScope.getQueryFields(item[0].Entity, item[0].ID);
                                data = [];
                            }
                        }
                        else {
                            data = item;
                        }
                    } else {
                        if (index != arrText.length - 1) {
                            data = [];
                        }
                    }
                }
            }
            //}
            //else {
            //    data = [];
            //}
        }


        if (!alreadySet) {
            $scope.setRuleIntellisenseData(data, arrText, input, eargs);
        }
    };
    $scope.setRuleIntellisenseData = function (data, arrText, input, eargs) {
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

    }
    var selectednode;
    $scope.CheckForLoopAndGetLoopParameters = function (node, bool, selectedrule) {
        if (bool) {
            selectednode = node;
            $scope.lstSelectedAliasobjCollection = [];
        }

        if (selectedrule.Elements != undefined && selectedrule.Elements.length > 0) {
            for (var i = 0; i < selectedrule.Elements.length; i++) {
                if (selectedrule.Elements[i].dictAttributes != undefined && selectedrule.Elements[i].dictAttributes.sfwNodeID == node.dictAttributes.sfwNodeID) {
                    if (selectedrule.Elements[i].Name == "foreach") {
                        for (var j = 0; j < $scope.lstAliasObjCollection.length; j++) {
                            if ($scope.lstAliasObjCollection[j].ID == selectedrule.Elements[i].dictAttributes.sfwItemName) {
                                var isFound = false;
                                for (k = 0; k < $scope.lstSelectedAliasobjCollection.length; k++) {
                                    if ($scope.lstSelectedAliasobjCollection[k].ID == $scope.lstAliasObjCollection[j].ID) {
                                        isFound = true;
                                    }
                                }
                                if (!isFound) {
                                    if (selectednode.dictAttributes.sfwNodeID != selectedrule.Elements[i].dictAttributes.sfwNodeID) {
                                        $scope.lstSelectedAliasobjCollection.push($scope.lstAliasObjCollection[j]);
                                    }
                                }
                            }
                        }
                    }
                    $scope.CheckForLoopAndGetLoopParameters(selectedrule, false, $scope.objSelectedLogicalRule);
                }
                else {
                    if (selectedrule.Elements[i].Elements != undefined && selectedrule.Elements[i].Elements.length > 0) {
                        $scope.CheckForLoopAndGetLoopParameters(node, false, selectedrule.Elements[i]);
                    }
                    if (selectedrule.Elements[i].Children != undefined && selectedrule.Elements[i].Children.length > 0) {
                        $scope.CheckForLoopAndGetLoopParameters(node, false, selectedrule.Elements[i]);
                    }
                }
            }
        }
        if (selectedrule.Children != undefined && selectedrule.Children.length > 0) {
            for (var i = 0; i < selectedrule.Children.length; i++) {
                if (selectedrule.Children[i].dictAttributes != undefined && selectedrule.Children[i].dictAttributes.sfwNodeID == node.dictAttributes.sfwNodeID) {
                    if (selectedrule.Children[i].Name == "foreach") {
                        for (var j = 0; j < $scope.lstAliasObjCollection.length; j++) {
                            if ($scope.lstAliasObjCollection[j].ID == selectedrule.Children[i].dictAttributes.sfwItemName) {
                                var isFound = false;
                                for (k = 0; k < $scope.lstSelectedAliasobjCollection.length; k++) {
                                    if ($scope.lstSelectedAliasobjCollection[k].ID == $scope.lstAliasObjCollection[j].ID) {
                                        isFound = true;
                                    }
                                }
                                if (!isFound) {
                                    if (selectednode.dictAttributes.sfwNodeID != selectedrule.Children[i].dictAttributes.sfwNodeID) {
                                        $scope.lstSelectedAliasobjCollection.push($scope.lstAliasObjCollection[j]);
                                    }
                                }
                            }
                        }
                    }
                    $scope.CheckForLoopAndGetLoopParameters(selectedrule, false, $scope.objSelectedLogicalRule);
                }
                else {
                    if (selectedrule.Children[i].Elements != undefined && selectedrule.Children[i].Elements.length > 0) {
                        $scope.CheckForLoopAndGetLoopParameters(node, false, selectedrule.Children[i]);
                    }
                    if (selectedrule.Children[i].Children != undefined && selectedrule.Children[i].Children.length > 0) {
                        $scope.CheckForLoopAndGetLoopParameters(node, false, selectedrule.Children[i]);
                    }
                }
            }
        }
    };

    $scope.GetAliasObjects = function (selectedrule, bool) {
        if (bool) {
            $scope.lstAliasObjCollection = [];
        }
        if (selectedrule.Elements != undefined && selectedrule.Elements.length > 0) {
            for (var i = 0; i < selectedrule.Elements.length; i++) {
                if (selectedrule.Elements[i].Name == "foreach") {
                    var collection = [];
                    //var collection = $scope.GetLoopCollection(selectedrule.Elements[i].dictAttributes.sfwObjectID);
                    $scope.GetAliasName(selectedrule.Elements[i].dictAttributes.sfwObjectID);
                    if (selectedrule.Elements[i].dictAttributes.sfwItemName != undefined && selectedrule.Elements[i].dictAttributes.sfwItemName != "" && selectedrule.Elements[i].dictAttributes.sfwObjectID != "" && selectedrule.Elements[i].dictAttributes.sfwObjectID != undefined) {
                        var obj = {
                            ID: selectedrule.Elements[i].dictAttributes.sfwItemName, Tooltip: selectedrule.Elements[i].dictAttributes.sfwItemName, DataType: "AliasObject", lstobjCollection: collection, Value: selectedrule.Elements[i].dictAttributes.sfwItemName, AliasName: $scope.AliasName
                        };
                        $scope.lstAliasObjCollection.push(obj);
                    }
                    $scope.GetAliasObjects(selectedrule.Elements[i], false);
                }
                else {
                    if (selectedrule.Elements[i].Elements != undefined) {
                        if (selectedrule.Elements[i].Elements.length > 0) {
                            $scope.GetAliasObjects(selectedrule.Elements[i], false);
                        }
                    }
                    if (selectedrule.Elements[i].Children != undefined) {
                        if (selectedrule.Elements[i].Children.length > 0) {
                            $scope.GetAliasObjects(selectedrule.Elements[i], false);
                        }
                    }
                }
            }
        }
        if (selectedrule.Children != undefined && selectedrule.Children.length > 0) {
            for (var i = 0; i < selectedrule.Children.length; i++) {
                if (selectedrule.Children[i].Name == "foreach") {
                    //var collection = $scope.GetLoopCollection(selectedrule.Children[i].dictAttributes.sfwObjectID);
                    var collection = [];
                    $scope.GetAliasName(selectedrule.Children[i].dictAttributes.sfwObjectID);
                    if (selectedrule.Children[i].dictAttributes.sfwItemName != undefined && selectedrule.Children[i].dictAttributes.sfwItemName != "" && selectedrule.Children[i].dictAttributes.sfwObjectID != undefined && selectedrule.Children[i].dictAttributes.sfwObjectID != "") {
                        var obj = {
                            ID: selectedrule.Children[i].dictAttributes.sfwItemName, Tooltip: selectedrule.Children[i].dictAttributes.sfwItemName, DataType: "AliasObject", lstobjCollection: collection, Value: selectedrule.Children[i].dictAttributes.sfwItemName, AliasName: $scope.AliasName
                        };
                        $scope.lstAliasObjCollection.push(obj);
                    }
                    $scope.GetAliasObjects(selectedrule.Children[i]);
                }
                else {
                    if (selectedrule.Children[i].Elements != undefined && selectedrule.Children[i].Elements.length > 0) {
                        $scope.GetAliasObjects(selectedrule.Children[i]);
                    }
                    if (selectedrule.Children[i].Children != undefined && selectedrule.Children[i].Children.length > 0) {
                        $scope.GetAliasObjects(selectedrule.Children[i]);
                    }
                }
            }
        }
    };

    $scope.GetAliasName = function (text) {
        if (text != undefined) {
            var arrtext = text.split(".");
            for (var i = 0; i < $scope.lstAliasObjCollection.length; i++) {
                for (var j = 0; j < arrtext.length; j++) {
                    if (arrtext[j] == $scope.lstAliasObjCollection[i].ID) {
                        arrtext[j] = $scope.lstAliasObjCollection[i].AliasName;
                    }
                }
            }
            text = "";
            for (var k = 0; k < arrtext.length; k++) {
                if (k == 0) {
                    text += arrtext[k];
                }
                else {
                    text += "." + arrtext[k];
                }
            }
            $scope.AliasName = text;
        }
    };

    $scope.getQueryFields = function (entityId, queryId) {
        hubMain.server.getEntityQueryFields(entityId, queryId).done(function (lstfields) {
            $scope.$apply(function () {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entity = entityIntellisenseList.filter(function (x) { return x.ID == entityId; });
                var query = entity[0].Queries.filter(function (y) { return y.ID == queryId; });
                if (query.length > 0) {
                    query[0].lstQueryFields = lstfields;
                    query[0].isQueryfieldsLoaded = true;
                }
            });
        });
    };

    //#endregion

    //#region Find Text

    $scope.FindMatchingList = function () {


        if ($scope.objLogicalRule.dictAttributes != undefined && $scope.objLogicalRule.dictAttributes.sfwRuleType == 'LogicalRule') {
            $.connection.hubRuleModel.server.findTextforRule($scope.findtext, $scope.objSelectedLogicalRule).done(function (data) {
                $scope.lstMatchingtext = undefined;
                $scope.$apply(function () {

                    $scope.ResetAllvalues($scope.objSelectedLogicalRule);
                    $scope.lstMatchingtext = data;
                    if ($scope.lstMatchingtext.length > 0) {
                        var obj = $scope.lstMatchingtext[$scope.FindIndex];
                        if (obj != undefined) {
                            $scope.findSelfVM(obj.SelfVM, $scope.objSelectedLogicalRule);
                        }
                    }
                    else {
                        $scope.FindIndex = -1;
                    }
                });
            });
        }

        else if ($scope.objLogicalRule.RuleType == 'DecisionTable') {
            $.connection.hubRuleModel.server.findTextListForDecisionTable($scope.findtext, $scope.objSelectedLogicalRule).done(function (data) {
                $scope.lstMatchingtext = undefined;
                $scope.$apply(function () {

                    $scope.ResetAllvaluesForDecisionTable($scope.objSelectedLogicalRule);
                    $scope.lstMatchingtext = data;
                    if ($scope.lstMatchingtext.length > 0) {
                        var obj = $scope.lstMatchingtext[$scope.FindIndex];
                        if (obj != undefined) {
                            $scope.findSelfVMForDecisionTable(obj.SelfVM, $scope.objSelectedLogicalRule);
                        }
                    }
                    else {
                        $scope.FindIndex = -1;
                    }
                });
            });
        }
        $scope.FindIndex = 0;

    };

    $scope.IsFindOpen = false;

    $scope.FindNext = function () {

        if ($scope.lstMatchingtext != undefined) {

            if ($scope.lstMatchingtext.length > 0 && $scope.FindIndex < $scope.lstMatchingtext.length) {
                $scope.FindIndex++;
                var obj = $scope.lstMatchingtext[$scope.FindIndex];
                if (obj != undefined) {
                    if ($scope.objLogicalRule.dictAttributes != undefined && $scope.objLogicalRule.dictAttributes.sfwRuleType == 'LogicalRule') {
                        $scope.ResetAllvalues($scope.objSelectedLogicalRule);
                        $scope.findSelfVM(obj.SelfVM, $scope.objSelectedLogicalRule);
                    }
                    else if ($scope.objLogicalRule.RuleType == 'DecisionTable') {
                        $scope.ResetAllvaluesForDecisionTable($scope.objSelectedLogicalRule);
                        $scope.findSelfVMForDecisionTable(obj.SelfVM, $scope.objSelectedLogicalRule);
                    }

                }

                else {
                    $scope.FindIndex--;
                    alert('Finished searching, reached end of the file');
                }
            }
            else {
                alert('Finished searching, reached end of the file');
            }
        }
    };

    $scope.FindPrevious = function () {

        if ($scope.lstMatchingtext != undefined) {
            if ($scope.lstMatchingtext.length > 0 && $scope.FindIndex > -1) {

                $scope.FindIndex--;
                if ($scope.FindIndex > -1) {

                    if ($scope.objLogicalRule.dictAttributes != undefined && $scope.objLogicalRule.dictAttributes.sfwRuleType == 'LogicalRule') {
                        $scope.ResetAllvalues($scope.objSelectedLogicalRule);
                        var obj = $scope.lstMatchingtext[$scope.FindIndex];
                        if (obj != undefined) {
                            $scope.findSelfVM(obj.SelfVM, $scope.objSelectedLogicalRule);

                        }
                    }
                    else if ($scope.objLogicalRule.RuleType == 'DecisionTable') {

                        $scope.ResetAllvaluesForDecisionTable($scope.objSelectedLogicalRule);
                        var obj = $scope.lstMatchingtext[$scope.FindIndex];
                        if (obj != undefined) {
                            $scope.findSelfVMForDecisionTable(obj.SelfVM, $scope.objSelectedLogicalRule);

                        }
                    }


                }
                else {

                    alert('Finished searching, reached start of the file');
                }

            }

        }
    };

    $scope.findSelfVM = function (obj, stepVM) {

        angular.forEach(stepVM.Elements, function (item) {

            if (item.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                if (item.Name == "action") {
                    stepVM.IsSelected = true;

                } else {
                    item.IsSelected = true;

                }

            }
            $scope.findSelfVM(obj, item);
        });

        angular.forEach(stepVM.Children, function (item) {

            if (item.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                if (item.Name == "action") {
                    stepVM.IsSelected = true;

                } else {
                    item.IsSelected = true;

                }

            }
            $scope.findSelfVM(obj, item);
        });
    };

    $scope.ResetAllvalues = function (stepVM) {

        angular.forEach(stepVM.Elements, function (item) {
            item.IsSelected = false;
            $scope.ResetAllvalues(item);
        });

        angular.forEach(stepVM.Children, function (item) {

            item.IsSelected = false;
            $scope.ResetAllvalues(item);
        });
    };

    $scope.findSelfVMForDecisionTable = function (obj, stepVM) {

        // for Dataheaders
        angular.forEach(stepVM.DataHeaders, function (item) {

            if (item.NodeID == obj.NodeID) {
                item.IsSelected = true;
                return;
            }

        });

        // for Row
        angular.forEach(stepVM.Rows, function (item) {
            angular.forEach(item.Cells, function (itm) {

                if (itm.Item.NodeID == obj.NodeID) {
                    itm.Item.IsSelected = true;
                    return;

                }


            });

        });
    };

    $scope.ResetAllvaluesForDecisionTable = function (stepVM) {
        // for Dataheaders
        angular.forEach(stepVM.DataHeaders, function (item) {
            item.IsSelected = false;
        });

        // for Row
        angular.forEach(stepVM.Rows, function (item) {
            angular.forEach(item.Cells, function (itm) {
                itm.Item.IsSelected = false;
                return;
            });

        });
    };
    //#endregion

    //#region Validation Methods 
    //$scope.displayValidation = function (lstErrorValidation) {
    //    $scope.resetAllSpanValue();
    //    var objValidation = JSON.parse(lstErrorValidation);
    //    if (objValidation && objValidation.ilstErrors && objValidation.ilstErrors.length > 0) {
    //        angular.forEach(objValidation.ilstErrors, function (objError) {
    //            if (objError.istrNodeID && objError.istrNodeID != "") {
    //                var startindex = objError.iintPosition;
    //                var endIndex = -1;
    //                if (objError.istrToken && objError.istrToken != "") {
    //                    endIndex = startindex + objError.istrToken.length - 1;

    //                    var vm = $scope.findVM(objError.istrNodeID);
    //                    if (vm) {

    //                        var lstSpan = document.getElementsByClassName("hidden-span-expression");
    //                        for (var i = 0; i < lstSpan.length; i++) {
    //                            var index = startindex;

    //                            if ($scope.isMatchHtmlNodeText($(lstSpan[i])[0].innerHTML, vm)) {
    //                                var htmlContent = $(lstSpan[i])[0].innerHTML;
    //                                // alert(htmlContent);
    //                                var ulist = $(lstSpan[i]).find("u");
    //                                if (ulist && ulist.length > 0) {
    //                                    for (var k = 0; k < ulist.length; k++) {
    //                                        index += 7;

    //                                        endIndex = index + objError.istrToken.length - 1;
    //                                    }
    //                                }
    //                                var gtcount = (htmlContent.match(/&gt;/g) || []).length;
    //                                if (gtcount > 0) {
    //                                    var strHtml = htmlContent;
    //                                    for (var count = 0; count < gtcount; count++) {
    //                                        var index1 = strHtml.indexOf("&gt;");
    //                                        if (index1 < index) {
    //                                            index += 3;
    //                                            endIndex = index + objError.istrToken.length - 1;
    //                                        }
    //                                        strHtml = strHtml.substring(index1 + 4);
    //                                    }
    //                                }

    //                                var ltcount = (htmlContent.match(/&lt;/g) || []).length;
    //                                if (ltcount > 0) {
    //                                    var strHtml = htmlContent;
    //                                    for (var count = 0; count < ltcount; count++) {
    //                                        var index1 = strHtml.indexOf("&lt;");
    //                                        if (index1 < index) {
    //                                            index += 3;
    //                                            endIndex = index + objError.istrToken.length - 1;
    //                                        }
    //                                        strHtml = strHtml.substring(index1 + 4);
    //                                    }
    //                                }

    //                                var strHtml = "";
    //                                for (var j = 0; j < htmlContent.length; j++) {
    //                                    if (j == index) {
    //                                        strHtml += "<u>" + htmlContent[j];
    //                                    }
    //                                    else if (j < endIndex && j > index) {
    //                                        strHtml += htmlContent[j];
    //                                    }
    //                                    else if (j == endIndex) {
    //                                        strHtml += htmlContent[j] + "</u>";
    //                                    }

    //                                    else {
    //                                        strHtml += htmlContent[j];
    //                                    }

    //                                }
    //                                $(lstSpan[i])[0].innerHTML = strHtml;
    //                            }

    //                        }
    //                    }
    //                }
    //            }
    //        });
    //    }
    //};

    $scope.isMatchHtmlNodeText = function (innerHtml, vm) {
        var retVal = false;
        var htmlContent = innerHtml;
        htmlContent = htmlContent.replace(/<\/?u[^>]*>/g, "");
        htmlContent = htmlContent.replace(new RegExp('(&gt;)', 'gi'), ">");
        htmlContent = htmlContent.replace(new RegExp('(&lt;)', 'gi'), "<");
        if (vm.dictAttributes) {
            if (htmlContent == vm.dictAttributes.sfwExpression) {
                retVal = true;
            }
            else if (vm.Name == "method") {
                if (htmlContent == vm.dictAttributes.sfwReturnField) {
                    retVal = true;
                }
                else if (htmlContent == vm.dictAttributes.sfwMethodName) {
                    retVal = true;
                }
            }
            else if (vm.Name == "calllogicalrule" || vm.Name == "calldecisiontable" || vm.Name == "callexcelmatrix") {
                if (htmlContent == vm.dictAttributes.sfwReturnField) {
                    retVal = true;
                }
                else if (htmlContent == vm.dictAttributes.sfwRuleID) {
                    retVal = true;
                }
            }
            else if (vm.Name == "foreach") {
                if (htmlContent == vm.dictAttributes.sfwObjectID) {
                    retVal = true;
                }
            }
        }
        else if (!vm.dictAttributes) {
            if (htmlContent == vm.Expression) {
                retVal = true;
            }
        }

        return retVal;
    };

    $scope.resetAllSpanValue = function () {
        var lstSpan = document.getElementsByClassName("hidden-span-expression");
        for (var i = 0; i < lstSpan.length; i++) {
            var htmlContent = $(lstSpan[i])[0].innerHTML;
            //htmlContent.replace(new RegExp('(<u>)', 'gi'), "");
            //htmlContent.replace(new RegExp('(</u>)', 'gi'), "");
            htmlContent = htmlContent.replace(/<\/?u[^>]*>/g, "");
            $(lstSpan[i])[0].innerHTML = htmlContent;
        }
    };

    $scope.findVM = function (istrNodeID) {
        var vm;
        if ($scope.objLogicalRule.dictAttributes != undefined) {
            var lstRules = $scope.objLogicalRule.Elements.filter(function (x) { return x.Name == "logicalrule"; });
            angular.forEach(lstRules, function (rule) {
                if (!vm) {
                    var lstSteps = rule.Elements.filter(function (x) { return x.Name != "variables"; });
                    angular.forEach(rule.Elements, function (step) {
                        if (!vm) {
                            if (step.Name != "variables") {
                                vm = $scope.FindVMByNodeID(istrNodeID, step);
                            }
                        }
                    });
                }
            });
        }
        else {
            angular.forEach($scope.objLogicalRule.Rules, function (rule) {
                if (!vm) {
                    vm = $scope.FindDecisionTableVMByNodeID(istrNodeID, rule);
                }
            });
        }

        return vm;
    };

    $scope.FindVMByNodeID = function (istrNodeID, stepVM) {
        var objStep;
        if (stepVM.dictAttributes.sfwNodeID == istrNodeID) {
            return stepVM;
        }
        else {
            angular.forEach(stepVM.Elements, function (item) {
                if (!objStep) {
                    if (item.dictAttributes.sfwNodeID == istrNodeID) {
                        objStep = item;

                    }
                    objStep = $scope.FindVMByNodeID(istrNodeID, item);
                }
            });

            angular.forEach(stepVM.Children, function (item) {
                if (!objStep) {
                    if (item.dictAttributes.sfwNodeID == istrNodeID) {
                        objStep = item;

                    }
                    objStep = $scope.FindVMByNodeID(istrNodeID, item);
                }
            });

            return objStep;
        }
    };

    $scope.FindDecisionTableVMByNodeID = function (istrNodeID, stepVM) {
        var objStep;
        angular.forEach(stepVM.DataHeaders, function (item) {
            if (!objStep) {
                if (item.NodeID == istrNodeID) {
                    objStep = item;
                    return;
                }
            }
        });

        // for Row
        angular.forEach(stepVM.Rows, function (item) {
            angular.forEach(item.Cells, function (itm) {
                if (!objStep) {
                    if (itm.Item.NodeID == istrNodeID) {
                        objStep = itm.Item;
                        return;

                    }
                }
            });
        });
        return objStep;
    };
    //#endregion

    $scope.openEntityClick = function (aObjParam) {
        if (aObjParam.strEntity && aObjParam.strEntity != "") {
            $scope.OnParameterOkClick(aObjParam);
            $NavigateToFileService.NavigateToFile(aObjParam.strEntity, "", "");
        }
    };

    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.currentfile.FileType != "DecisionTable") {
    //        if ($scope.objRuleExtraFields) {
    //            var index = $scope.objLogicalRule.Elements.indexOf($scope.objRuleExtraFields);
    //            if (index > -1) {
    //                $scope.objLogicalRule.Elements.splice(index, 1);
    //            }
    //        }
    //    } else {

    //        //$scope.objLogicalRule.ExtraFields = {};
    //        delete $scope.objLogicalRule["ExtraFields"];
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.currentfile.FileType != "DecisionTable") {
            if ($scope.objRuleExtraFields) {
                var index = $scope.objLogicalRule.Elements.indexOf($scope.objRuleExtraFields);
                if (index == -1) {
                    $scope.objLogicalRule.Elements.push($scope.objRuleExtraFields);
                }
            }
        } else {
            // convert basemodel format into Extrafield object 
            if ($scope.objRuleExtraFields.Elements.length > 0) {
                var obj;
                var arrObj = [];
                for (var i = 0; i < $scope.objRuleExtraFields.Elements.length; i++) {
                    obj = {
                    };
                    obj.ID = $scope.objRuleExtraFields.Elements[i].dictAttributes.ID;
                    obj.Value = $scope.objRuleExtraFields.Elements[i].dictAttributes.Value;
                    obj.URL = $scope.objRuleExtraFields.Elements[i].dictAttributes.URL;
                    arrObj.push(obj);
                }
                $scope.objLogicalRule.ExtraFields = arrObj;
            }
            else {
                $scope.objLogicalRule.ExtraFields = [];
            }
        }
    };
    $scope.toggleSideBar = function () {
        $scope.IsToolsDivCollapsed = !$scope.IsToolsDivCollapsed;
    };

    $scope.EditVersionDetailsDailog = function () {
        var newScope = $scope.$new();
        newScope.Init = function () {
            newScope.objRuleVersion = {};
            newScope.objRuleVersion.IsDefaultVersion = false;
            if ($scope.objSelectedLogicalRule.dictAttributes != undefined) {
                if (!$scope.objSelectedLogicalRule.dictAttributes.sfwEffectiveDate) {
                    newScope.objRuleVersion.IsDefaultVersion = true;
                }
                newScope.objRuleVersion.Description = $scope.objSelectedLogicalRule.dictAttributes.Text;
                newScope.objRuleVersion.Status = $scope.objSelectedLogicalRule.dictAttributes.sfwStatus;
            }
            else if ($scope.objSelectedLogicalRule.dictAttributes == undefined) {
                if (!$scope.objSelectedLogicalRule.EffectiveDate) {
                    newScope.objRuleVersion.IsDefaultVersion = true;
                }
                newScope.objRuleVersion.Description = $scope.objSelectedLogicalRule.Description;
                newScope.objRuleVersion.Status = $scope.objSelectedLogicalRule.Status;
            }
        };

        newScope.onOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");

            if ($scope.objLogicalRule.dictAttributes) {
                $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.Text, $scope.objSelectedLogicalRule.dictAttributes, "Text", newScope.objRuleVersion.Description);
                $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.dictAttributes.sfwStatus, $scope.objSelectedLogicalRule.dictAttributes, "sfwStatus", newScope.objRuleVersion.Status);
            }
            else {
                $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.Description, $scope.objSelectedLogicalRule, "Description", newScope.objRuleVersion.Description);
                $rootScope.EditPropertyValue($scope.objSelectedLogicalRule.Status, $scope.objSelectedLogicalRule, "Status", newScope.objRuleVersion.Status);
            }

            $rootScope.UndRedoBulkOp("End");
            newScope.CloseDialog();
        };

        newScope.CloseDialog = function () {
            if (newScope.versionDetailDialog) {
                newScope.versionDetailDialog.close();
            }
        };

        newScope.Init();
        newScope.versionDetailDialog = $rootScope.showDialog(newScope, "Version Details", "Rule/views/RuleVersionDetails.html");
    };

}]);

function GetVariablesAndParams(parent, type, isdecisiontable) {
    if (isdecisiontable) {
        if (type == "parameters") {
            var lst = [];
            if (parent && parent.Parameters) {
                lst = parent.Parameters;
            }
            return lst;
        }
    }
    else if (parent && parent.Elements) {

        var lst = parent.Elements.filter(function (x) {
            if (x.Name == type)
                return true;
            else
                return false;
        });


        if (lst.length > 0) {
            return lst[0];
        }
        else {
            var obj = {
                Name: type, value: '', dictAttributes: {}, Elements: []
            };
            parent.Elements.push(obj);
            return obj;
        }

    }
}

function GetDataTypePrefix(datatype) {
    var prefiex;
    if (datatype != undefined) {
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

function closeOtherRightPanels(panelName) {
    var scope = getCurrentFileScope();
    if (panelName != "LocalVariables") {
        var localVar = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#dvLocalVar");
        localVar.hide("slide", { direction: "right" });
    }
    if (panelName != "Parameters") {
        var params = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#dvParameters");
        params.hide("slide", { direction: "right" });
    }
    if (panelName != "DecisionTableParameters") {
        var params = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#dvDecisionTableParameters");
        params.hide("slide", { direction: "right" });
    }
    if (panelName != "ChildRules") {
        var childRules = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#dvChildRules");
        childRules.hide("slide", { direction: "right" });
    }
    if (panelName != "ParentRules") {
        var parentRules = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#dvParentRules");
        parentRules.hide("slide", { direction: "right" });
    }

}

function setSelectionForCurrentPanel(event) {
    var li = event.currentTarget;
    if (li) {
        $(li).addClass("selected");
        $(li).siblings().removeClass("selected");
    }
}

function setAnalystView(ruleType) {
    if (ruleType == "LogicalRule") {
        $(".tbl-assign").parent("span").css("margin-top", "15px");
        $(".default-text-assign").css("height", "22px");

        $(".tbl-assign").each(function () {

            $(this).find("tr").each(function (index) {
                if (index > 0) {
                    $(this).hide();
                }
            });
        });
    }
    else if (ruleType == "DecisionTable") {
        $(".dt-assign-header, .dt-assign, .dt-row-col-header, .dt-if").each(function () {
            $(this).find("table tr").each(function (index) {
                if (index > 0) {
                    $(this).hide();
                }
            });
        });
    }
}
function setDeveloperView(ruleType) {
    if (ruleType == "LogicalRule") {
        $(".tbl-assign").parent("span").removeAttr("style");
        $(".default-text-assign").removeAttr("style");

        $(".tbl-assign").each(function () {

            $(this).find("tr").each(function (index) {
                if (index > 0) {
                    $(this).show();
                }
            });
        });
    }
    else if (ruleType == "DecisionTable") {
        $(".dt-assign-header, .dt-assign, .dt-row-col-header, .dt-if").each(function () {
            $(this).find("table tr").each(function (index) {
                if (index > 0) {
                    $(this).show();
                }
            });
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
        char == '%' || char == '=' || char == '"' || char == '(' || char == ')' || char == ',' || char == '?' || char == ':' || char == ';') {
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
var ruleIntellisenseData;
function setRuleIntellisense(controlSelection, data, scope, propertyName) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // when pressed up or down key    
        return;
    }
    if (propertyName == "") {
        propertyName = undefined;
    }
    ruleIntellisenseData = data;
    if (data.length > 0) {
        if (typeof data[0].ID == "undefined") {
            data = data.sort();
        }
        else {
            if (typeof propertyName != "undefined") {
                data = data.sort(function (a, b) {
                    var nameA = a[propertyName].toLowerCase(), nameB = b[propertyName].toLowerCase();
                    if (nameA < nameB) //sort string ascending
                        return -1;
                    if (nameA > nameB)
                        return 1;
                    return 0; //default return value (no sorting)
                });
            }
            else {
                data = data.sort(function (a, b) {
                    if (a.ID && b.ID) {
                        var nameA = a.ID.toLowerCase(), nameB = b.ID.toLowerCase();
                        if (nameA < nameB) //sort string ascending
                            return -1;
                        if (nameA > nameB)
                            return 1;
                    }
                    return 0; //default return value (no sorting)
                });
            }
        }
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var pos = controlSelection.textareaHelper('caretPosAbs');

                    $("#dvIntellisense > ul").css({
                        left: (pos.left) + "px",
                        top: (pos.top) + "px",
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    });
                }
                else if (controlSelection[0].localName == "span") {
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
                    //var $input = $(event.target),
                    //$results = $input.autocomplete("widget");
                    ////top = $results.position().top,
                    //left = $results.position().left,
                    //windowWidth = $(window).width(),
                    //height = $results.height(),
                    //inputHeight = $input.height(),
                    //windowsHeight = $(window).height();
                    //if (windowsHeight < $results.position().top + height + inputHeight) {
                    //    newTop = $results.position().top - height - inputHeight - 10;
                    //    $results.css("top", newTop + "px");
                    //}
                    //if (left + 300 > windowWidth) {
                    //    newleft = left - 300;
                    //    $results.css("left", newleft + "px");
                    //}
                    //var distanceToTop = this.getBoundingClientRect().top;
                    //var distanceToleft = this.getBoundingClientRect().left;
                    //var currentSpanWidth = this.getBoundingClientRect().width;
                    //var currentSpanHeight = this.getBoundingClientRect().height;
                    //var windowsHeight = $(window).height();
                    //if (distanceToTop + 300 > windowsHeight) {
                    //    distanceToTop -= 300;
                    //}
                    //else {
                    //    distanceToTop += currentSpanHeight;
                    //}
                    //$("#dvIntellisense > ul").css({
                    //    left: (distanceToleft) + "px",
                    //    top: (distanceToTop) + "px",
                    //    width: 'auto',
                    //    height: 'auto',
                    //    overflow: 'auto',
                    //    maxWidth: '300px',
                    //    maxHeight: "300px"
                    //});
                    //var vrCurrentPopupHeight;
                    //for (var i = 0; i < $("#dvIntellisense > ul").length; i++) {
                    //    if ($("#dvIntellisense > ul")[i].offsetHeight != 0) {
                    //        var vrCurrentPopupHeight = $("#dvIntellisense > ul")[i].offsetHeight;
                    //    }
                    //}
                    //if ((vrCurrentPopupHeight < 300) && (distanceToTop < this.getBoundingClientRect().top)) {
                    //    if ((this.getBoundingClientRect().top + this.getBoundingClientRect().height + vrCurrentPopupHeight) > windowsHeight) {
                    //        distanceToTop = this.getBoundingClientRect().top - vrCurrentPopupHeight;
                    //    }
                    //    else {
                    //        distanceToTop = this.getBoundingClientRect().top + this.getBoundingClientRect().height;
                    //    }
                    //    $("#dvIntellisense > ul").css({
                    //        left: (distanceToleft) + "px",
                    //        top: (distanceToTop) + "px",
                    //        width: 'auto',
                    //        height: 'auto',
                    //        overflow: 'auto',
                    //        maxWidth: '300px',
                    //        maxHeight: "300px"
                    //    });
                    //}
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            source: function (request, response) {
                data = ruleIntellisenseData;
                //response();
                response($.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        var templogo = "";
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
                        else if (value.EntityName && value.EntityName != "") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Entity.png'/>";
                        }
                        else if (value.RuleType && value.RuleType == "LogicalRule") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/logical_rule.png'/>";
                        }
                        else if (value.RuleType && value.RuleType == "DecisionTable") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/decision_table..png'/>";
                        }
                        else if (value.RuleType && value.RuleType == "ExcelMatrix") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Excel_matrix.png'/>";
                        }
                        if (typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value,
                                logo: templogo
                            };
                        }
                        else {
                            if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                                return {
                                    label: value[propertyName],
                                    value: value,
                                    logo: templogo
                                };
                            }
                            else {
                                return {
                                    label: value.ID,
                                    value: value,
                                    logo: templogo
                                };
                            }
                        }
                    }), extractLast(request.term, this.element[0].selectionStart)));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                if (ui.item.value && ui.item.value.hasOwnProperty("SqlQuery")) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.SqlQuery);
                }
                else if (ui.item.value && ui.item.value.hasOwnProperty("Tooltip") && ui.item.value.Tooltip) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Tooltip);
                } else {
                    $(".ui-autocomplete > li").attr("title", ui.item.label);
                }
                return false;
            },
            select: function (event, ui) {

                var arr;
                var caretindex = 0;
                if (event.target.localName == "input") {
                    arr = getSplitArray(this.value, this.selectionStart);
                }
                else if (event.target.localName == "span") {
                    caretindex = caretPosition(this);
                    arr = getSplitArray(this.innerText, caretindex);
                }
                if (arr.length > 0) {
                    if (event.target.localName == "input") {
                        this.value = this.value.substr(0, this.value.lastIndexOf(arr[arr.length - 1]));
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
                if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                    this.value = this.value + ui.item.value[propertyName];
                }
                else if (ui.item.value.ID) {
                    if (event.target.localName == "input") {
                        this.value = this.value + ui.item.value.ID;
                    }
                    else if (event.target.localName == "span") {
                        //this.innerText = this.innerText + ui.item.value.ID;
                        this.innerText = [this.innerText.slice(0, caretindex), ui.item.value.ID, this.innerText.slice(caretindex)].join('');
                    }
                    if (scope && scope.model && scope.model.Name && (scope.model.Name == "calllogicalrule" || scope.model.Name == "calldecisiontable" || scope.model.Name == "callexcelmatrix" || scope.model.Name == "method" || scope.model.Name == "query")) {
                        var parameters = scope.model.Elements.filter(function (x) { return x.Name == "parameters"; });
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
                                    scope.parameters = parametersModel;
                                }
                                scope.$root.UndRedoBulkOp("Start");
                                //Removing unwanted parameters.
                                if (parametersModel.Elements.length > 0) {
                                    scope.$apply(function () {
                                        parametersModel.Elements = parametersModel.Elements.filter(function (x) { return ui.item.value.Parameters.some(function (element) { return element.ID == x.dictAttributes.ID; }); });
                                    });
                                }

                                //Adding or updating existing parameters
                                for (var index = 0; index < ui.item.value.Parameters.length; index++) {
                                    var parameterModel = parametersModel.Elements.filter(function (element) {
                                        return element.dictAttributes.ID == ui.item.value.Parameters[index].ID;
                                    });
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
                                        scope.$root.PushItem(parametersModel, scope.model.Elements);
                                    });
                                }
                                scope.$root.UndRedoBulkOp("End");
                            }
                        }
                        else {
                            if (parameters.length > 0) {
                                scope.$apply(function () {
                                    scope.$root.DeleteItem(scope.model.Elements[scope.model.Elements.indexOf(parameters[0])], scope.model.Elements);
                                });
                            }
                        }
                    }
                } else {
                    this.innerText = this.localName == 'input' ? this.value : this.innerText + ui.item.value;
                }

                var scope1 = angular.element($(this)).scope();
                if (scope1) {
                    //var val = scope.attributes("ng-model");
                    //var val1 = scope.attributes["ng-model"];
                    //var val = $(this)[0].attributes["ng-model"].value;
                    //var lst = val.split('.');
                    //var obj = scope;
                    //if (val.contains('.')) {
                    //    while (val.contains('.')) {
                    //        var strProp = val.substring(0, val.indexOf('.'));
                    //        val = val.substring(val.indexOf('.') + 1);
                    //        obj = obj[strProp];
                    //    }

                    //}

                    //angular.forEach(lst, function (x) {
                    //    if (!obj) {
                    //        obj = returnScope(scope, x);
                    //    }
                    //    else
                    //    {
                    //        obj = returnScope(obj, x)
                    //    }
                    //});

                    //if (obj) {
                    //    scope.$apply(function () {
                    //        obj[val] = "test";
                    //    })
                    //}
                    scope1.$broadcast("UpdateOnClick", controlSelection[0]); //valueChange(this.innerText);
                }
                //$(this).trigger("change");
                if (this.childNodes[0] != undefined) {
                    var doc = this.ownerDocument || this.document;
                    var win = doc.defaultView || doc.parentWindow;
                    var sel;

                    var len = ui.item.value && ui.item.value.ID != undefined ? ui.item.value.ID.length : ui.item.value.length;
                    var position = caretindex + len;
                    var range = document.createRange();
                    var numDiffLength = this.childNodes[0].length - position;
                    if (numDiffLength < 0) {
                        range.setStart(this.childNodes[0], position + numDiffLength);
                    }
                    else {
                        range.setStart(this.childNodes[0], position);
                    }
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
                else {
                    $(this).trigger("change");
                }

                return false;
            },
            close: function () {
                $(".page-header-fixed").css("pointer-events", "auto");
            }
        });
    }


}

function returnScope(scope, x) {
    var obj = scope[x];
    return obj;
}

function resetNodeIds(model) {
    if (model.dictAttributes && model.dictAttributes.sfwNodeID) {
        model.dictAttributes.sfwNodeID = generateUUID();
    }

    if (model.Elements && (model.Name == "neorule" || model.Name == "logicalrule" || model.Name == "switch" || model.Name == "foreach" || model.Name == "while" || model.Name == "case" || model.Name == "default" || model.Name == "actions")) {
        for (var index = 0; index < model.Elements.length; index++) {
            model.Elements[index] = resetNodeIds(model.Elements[index]);
        }
    }
    if (model.Elements && model.Name != "parameters" && model.Name != "ExtraFields" && model.Name != "variables" && model.Name != "neorule" && model.Name != "logicalrule" && model.Name != "case" && model.Name != "default") {
        for (var index = 0; index < model.Children.length; index++) {
            model.Children[index] = resetNodeIds(model.Children[index]);
        }
    }
    return model;
}

function getEntityIntellisense(entityName) {
    var data = [];
    var entities = JSON.parse(sessionStorage.intellisenseData);
    var entity = entities.filter(function (x) { return x.ID == entityName; });
    if (entity.length > 0) {
        data = data.concat(entity[0].Attributes);
    }
    return data;
}
function getFirstLevelIntellisense() {
    var data = [];
    var parameters = [];
    var localVariables = [];
    var rootScope = getCurrentFileScope();
    var parameterElements = [];
    var variableElements = [];
    if (rootScope.IsDecisionTable) {
        if (rootScope.parameters) {
            parameterElements = rootScope.parameters;
        }
    }
    else {
        if (rootScope.parameters && rootScope.parameters.Elements) {
            parameterElements = rootScope.parameters.Elements;
        }
        if (rootScope.variables && rootScope.variables.Elements) {
            variableElements = rootScope.variables.Elements;
        }
    }

    for (var index = 0; index < parameterElements.length; index++) {
        if (parameterElements[index].ParamID) {
            if (parameterElements[index].dictAttributes) {
                parameters.push({
                    ID: parameterElements[index].dictAttributes.ID,
                    DataType: parameterElements[index].dictAttributes.sfwDataType,
                    Direction: parameterElements[index].dictAttributes.sfwDirection,
                    Entity: parameterElements[index].dictAttributes.sfwEntity
                });
            }
            else {
                parameters.push({
                    ID: parameterElements[index].ParamID,
                    DataType: parameterElements[index].DataType,
                    Direction: parameterElements[index].Direction,
                    Entity: parameterElements[index].Entity
                });
            }
        }
    }
    for (var index = 0; index < variableElements.length; index++) {
        if (variableElements[index].dictAttributes.ID) {
            localVariables.push({
                ID: variableElements[index].dictAttributes.ID,
                DataType: variableElements[index].dictAttributes.sfwDataType,
                Entity: variableElements[index].dictAttributes.sfwEntity,
            });
        }
    }
    data = parameters.concat(localVariables);

    //Get the $rootScope object
    var scope = rootScope;
    while (scope != null) {
        if (scope.$root) {
            scope = scope.$root;
            break;
        }
        else {
            scope = scope.$parent;
        }
    }

    //Get constants object from $rootScope object.
    if (scope && scope.objConstants) {
        data.push({ ID: scope.objConstants.dictAttributes.ID, Type: "Constants" });
    }
    if (rootScope.objLogicalRule.dictAttributes) {
        if (rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() != "true") {
            var entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
            data = data.concat(getEntityIntellisense(entityName));
        }
    }
    else {
        if (rootScope.objLogicalRule.Static.toLowerCase() != "true") {
            var entityName = rootScope.objLogicalRule.Entity;
            data = data.concat(getEntityIntellisense(entityName));
        }
    }
    return data;
}
function getConstants(text) {
    data = [];


    //Get the $rootScope object
    var scope = getCurrentFileScope();
    while (scope != null) {
        if (scope.$root) {
            scope = scope.$root;
            break;
        }
        else {
            scope = scope.$parent;
        }
    }

    //Get constants object from $rootScope object.
    if (scope && scope.objConstants) {
        var arrText = text.split(".");
        if (arrText[0] == scope.objConstants.dictAttributes.ID) {
            var currentElements = scope.objConstants.Elements;
            for (var index = 1; index < arrText.length; index++) {
                var currentModel = currentElements.filter(function (x) { return x.dictAttributes.ID == arrText[index]; });
                if (currentModel && currentModel.length > 0) {
                    currentElements = currentModel[0].Elements;
                }
            }
        }

        if (currentElements && currentElements.length > 0) {
            for (var index = 0; index < currentElements.length; index++) {
                data.push({ ID: currentElements[index].dictAttributes.ID, Type: "Constants" });
            }
        }

        return data;
    }
}
function getEntityRules(entityName, ruleType, isStatic) {
    var entities = JSON.parse(sessionStorage.intellisenseData);
    entities = entities.filter(function (x) { return x.Name == entityName; });
    var rules = [];
    for (var index = 0; index < entities.length; index++) {
        rules = rules.concat(entities[index].Rules.filter(function (x) { return x.RuleType == ruleType && x.IsStatic == isStatic; }));
    }
    return rules;
}

function getChildRules(data) {
    var desc = getDescendents(data);
    var callActivities = desc.filter(function (x) {
        return x.Name == "calllogicalrule" || x.Name == "calldecisiontable" || x.Name == "callexcelmatrix";
    });
    var childRules = [];
    for (var index = 0; index < callActivities.length; index++) {
        var ruleType = callActivities[index].Name == "calllogicalrule" ? "LogicalRule" : (callActivities[index].Name == "calldecisiontable" ? "DecisionTable" : "ExcelMatrix");
        var id = callActivities[index].dictAttributes.sfwRuleID;
        var isStatic = false;
        if (id && id.length > 0) {
            if (id.lastIndexOf(".") > -1) {
                isStatic = true;
                id = id.substring(id.lastIndexOf(".") + 1);
            }

            if (!childRules.some(function (x) { return x.ID == id; })) {
                childRules.push({ ID: id, Type: ruleType, IsStatic: isStatic });
            }
        }
    }
    return childRules;
}

function getCaretCharacterOffsetWithin(element) {
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

app.directive("checkTabOverflow", ["$parse", "$compile", "$rootScope", function ($parse, $compile, $rootScope) {
    return {
        restrict: "A",
        link: function (scope, element, attrs) {
            if (attrs.functionName) {
                scope[attrs.functionName] = function () {
                    return scope.hasOverflow(element);
                };
                if (!scope.hasOverflow) {
                    scope.hasOverflow = function (ele) {
                        var returnValue = false;
                        var ul = ele.parent().siblings(".rule-scrollable-tab");
                        if (ul && ul.length > 0) {
                            if (ul[0].offsetWidth < ul[0].scrollWidth) {
                                returnValue = true;
                            }
                        }
                        return returnValue;
                    };
                }
            }
        }
    };
}]);

function scrollToLeft(event) {
    var element = $(event.target);
    var ul = element.parent().siblings(".rule-scrollable-tab");
    if (ul && ul.length > 0 && ul[0].scrollLeft > 0) {
        if (ul[0].scrollLeft >= 30) {
            ul[0].scrollLeft = ul[0].scrollLeft - 30;
        }
        else {
            ul[0].scrollLeft = 0;
        }
    }

}
function scrollToRight(event) {
    var element = $(event.target);
    var ul = element.parent().siblings(".rule-scrollable-tab");
    if (ul && ul.length > 0 && ul[0].scrollLeft < ul[0].scrollWidth) {
        if (ul[0].scrollLeft + 30 <= ul[0].scrollWidth) {
            ul[0].scrollLeft = ul[0].scrollLeft + 30;
        }
        else {
            ul[0].scrollLeft = ul[0].scrollWidth;
        }
    }
}
function sortDates(dates, order) {
    if (dates && dates.length > 0) {
        return dates.sort(function (a, b) {
            var date1 = new Date(a == "Default" || a == undefined ? "01/01/0001" : a);
            var date2 = new Date(b == "Default" || b == undefined ? "01/01/0001" : b);

            if (date1 && date2) {
                if (order && order == "asc") {
                    if (date1 < date2)
                        return -1;
                    if (date1 > date2)
                        return 1;
                }
                else {
                    if (date1 > date2)
                        return -1;
                    if (date1 < date2)
                        return 1;
                }
            }
            return 0;
        });
    }
}
function getCurrentEffectiveDate(dates) {
    if (dates && dates.length > 0) {
        var today = new Date();
        var effectiveDates = dates.filter(function (date) { date = new Date(date == "Default" || date == undefined ? "01/01/0001" : date); return new Date(date) <= today; });
        if (effectiveDates && effectiveDates.length > 0) {
            effectiveDates = sortDates(effectiveDates);
            return effectiveDates[0];
        }
    }
}