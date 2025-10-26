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
    $scope.ExportToExcel = function (isExcelMatrix) {
        $rootScope.IsLoading = true;
        var strobj = JSON.stringify($scope.objLogicalRule);
        if (strobj.length < 32000) {
            if (isExcelMatrix) {
                $.connection.hubRuleModel.server.exportExcelMatrix(strobj, $scope.selectedEffectiveDate);
            }
            else {
                $.connection.hubRuleModel.server.exportToExcel(strobj, $scope.selectedEffectiveDate);
            }
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

            SendDataPacketsToServerForExportExcel(lstDataPackets, isExcelMatrix);
        }

    };
    var SendDataPacketsToServerForExportExcel = function (lstpackets, isExcelMatrix) {

        for (var i = 0; i < lstpackets.length; i++) {
            $.connection.hubRuleModel.server.receiveDataPacketsForExportExcel(lstpackets[i], lstpackets.length, i, $scope.selectedEffectiveDate, isExcelMatrix);
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
        if (str) {
            if (datatypeprefix) {
                if ($scope.SelectedLocalVariable.dictAttributes.ID.match("^" + datatypeprefix)) {
                    str = str.substring(datatypeprefix.length);
                    isPrefixVisible = true;
                }

            }
        }
        else {
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
            if (str) {
                if (datatypeprefix) {
                    if ($scope.SelectedParameter.ParamID.match("^" + datatypeprefix)) {
                        str = str.substring(datatypeprefix.length);
                        isPrefixVisible = true;
                    }
                }
            }
            else {
                str = '';
            }
            $scope.OnEditParameter(datatypeprefix, str, $scope.SelectedParameter.DataType, $scope.SelectedParameter.Entity, $scope.SelectedParameter.Direction, $scope.SelectedParameter.Text, $scope.SelectedParameter.DataFormat, isPrefixVisible);
        }
        else {
            var str = $scope.SelectedParameter.dictAttributes.ID;
            var datatypeprefix = GetDataTypePrefix($scope.SelectedParameter.dictAttributes.sfwDataType);
            if (str) {
                if (datatypeprefix) {
                    if ($scope.SelectedParameter.dictAttributes.ID.match("^" + datatypeprefix)) {
                        str = str.substring(datatypeprefix.length);
                        isPrefixVisible = true;
                    }
                }
            }
            else {
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
app.directive("logicalRuleStartNodeList", ["$compile", function ($compile) {
    return {
        restrict: "E",
        scope: {
            model: "=",
            viewMode: "=",
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getStartTemplate = function () {
                return '<ul class="logical-rule-first-ul"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in siblingsCollection" model="item" view-mode="viewMode"></logical-rule-node-wrapper></ul>';
            };
            scope.init = function () {
                scope.siblingsCollection = scope.model.Elements;
            };
            scope.init();

            //var parentNode = element.parent();
            //if (parentNode) {
            //    var ul = $(scope.getStartTemplate());
            //    $compile(ul)(scope);
            //    element.remove();
            //    parentNode.append(ul);
            //}
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        templateUrl: "Rule/views/LogicalRule/LogicalRuleStartTemplate.html"
    };
}]);
app.directive("logicalRuleNodeList", ["$compile", function ($compile) {
    return {
        restrict: "E",
        replace: true,
        //scope: {
        //    viewMode: "=",
        //    model: "="
        //},
        link: function (scope, element, attributes) {
            scope.getElementsTemplate = function () {
                var elementsTemplateTextList = ['<ul ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':\'\'">                                                                                                         '
                    , '    <li ng-if="item.Name != \'variables\'" ng-repeat="item in siblingsCollection">  '
                    , '        <logical-rule-node-wrapper ng-if="item.Name != \'variables\'" model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                            '
                    , '    </li>                                                                                                                                                                            '
                    , '</ul>                                                                                                                                                                                '];

                return elementsTemplateTextList.join("");
            };

            scope.siblingsCollection = (scope.model.Name == "case" || scope.model.Name == "default") ? scope.model.Elements : scope.model.Children;


            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getElementsTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);
            }
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        //templateUrl: "Rule/views/LogicalRule/LogicalRuleStartTemplate.html"
    };
}]);
app.directive("loopSwitchChildren", ["$compile", function ($compile) {
    return {
        restrict: "E",
        replace: true,
        //scope:{
        //    viewMode: "=",
        //    model: "="
        //},
        link: function (scope, element, attributes) {
            scope.getChildrenTemplate = function () {
                var childrenTemplateTextList = ['<ul ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                                                                       '
                    , '    <li ng-repeat="item in model.Elements">  '
                    , '        <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                        '
                    , '    </li>                                                                                                                                     '
                    , '                                                                                                                                              '
                    , '</ul>                                                                                                                                         '];
                return childrenTemplateTextList.join("");
            };
            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getChildrenTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);
            }
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        //templateUrl: "Rule/views/LogicalRule/LoopSwitchChildrenTemplate.html"
    };
}]);

app.directive('logicalRuleNodeWrapper', ["$compile", "$rootScope", "$timeout", function ($compile, $rootScope, $timeout) {
    var AddItem = function (obj, param) {
        var objItem;
        var newObject = null;
        if (obj.Name == "case" || obj.Name == "default") {
            objItem = obj.Elements;
        }
        else {
            objItem = obj.Children;
        }

        var nodeid = generateUUID();
        newObject = { Name: param, value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] };
        if (param == 'switch') {
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'case', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'default', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
        }
        else if (param == 'actions') {
            newObject.Elements.push({ Name: 'action', value: '', dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: [] });
        }

        newObject.IsSelected = true;
        $rootScope.PushItem(newObject, objItem);

        $timeout(function () {
            if (obj) {
                checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
            }
            enableExpressionEditing(newObject.dictAttributes.sfwNodeID);
        });
    };

    var AddStep = function (obj, param) {
        var nodeid = generateUUID();
        var newObject = { Name: param, value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] };
        var insertionIndex = null;

        if (param == 'switch') {
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'case', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'default', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
        }
        else if (param == 'case') {
            var isFound = false;
            for (var i = 0; i < obj.Elements.length; i++) {
                if (obj.Elements[i].Name == 'default') {
                    isFound = true;
                    insertionIndex = obj.Elements.indexOf(obj.Elements[i]);
                    break;
                }
            }
        }
        else if (param == 'actions') {
            newObject.Elements.push({ Name: 'action', value: '', dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: [] });
        }

        newObject.IsSelected = true;
        if (insertionIndex) {
            $rootScope.InsertItem(newObject, obj.Elements, insertionIndex);
        }
        else {
            $rootScope.PushItem(newObject, obj.Elements);
        }

        $timeout(function () {
            if (obj) {
                checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
            }
            enableExpressionEditing(newObject.dictAttributes.sfwNodeID);
        });
    };

    var cutorcopyitem;
    var objParent;
    var Param;

    var CutOrCopyItem = function (obj, objofParent, param) {
        cutorcopyitem = obj;
        objParent = objofParent;
        Param = param;
    };

    var PasteItem = function (obj) {
        if (cutorcopyitem != undefined) {
            $rootScope.UndRedoBulkOp("Start");

            var data = JSON.stringify(cutorcopyitem);
            var objCutorCopyItem = JSON.parse(data);

            if (Param == 'cut') {

                if (cutorcopyitem.Name == 'case') {
                    if (obj.Name != 'switch') {
                        alert("If/Else block can be added only inside switch block");
                        objParent = undefined;
                    }
                    else {
                        var count = 0;
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name) {
                                count++;
                            }
                        }

                        if (count <= 1) {

                            alert("Atleast one if block has to be present inside switch block");
                            objParent = undefined;
                        }
                    }
                }

                if (cutorcopyitem.Name == 'default' && obj.Name == 'switch') {
                    for (var i = 0; i < obj.Elements.length; i++) {
                        if (obj.Elements[i].Name == cutorcopyitem.Name) {
                            alert("Only one Else block can be present");
                            objParent = undefined;
                        }
                    }
                }
                else if (cutorcopyitem.Name == 'default' && obj.Name != 'switch') {
                    alert("If/Else block can be added only inside the Condition");
                    objParent = undefined;
                }
                var ischildren = false;
                var iselements = false;
                if (objParent != undefined) {
                    if (objParent.Name == "foreach" || objParent.Name == "while") {
                        for (var i = 0; i < objParent.Children.length; i++) {
                            if (objParent.Children[i].Name == cutorcopyitem.Name && objParent.Children[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Children[i], objParent.Children);
                                ischildren = true;
                                var curIndex = i;
                                break;
                            }
                        }
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name && objParent.Elements[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Elements[i], objParent.Elements);
                                var curIndex = i;
                                iselements = true;
                                break;
                            }
                        }
                    }
                    if (objParent.Name != "logicalrule" && objParent.Name != "case" && objParent.Name != "default") {
                        for (var i = 0; i < objParent.Children.length; i++) {
                            if (objParent.Children[i].Name == cutorcopyitem.Name && objParent.Children[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Children[i], objParent.Children);
                                ischildren = true;
                                var curIndex = i;
                                break;
                            }
                        }
                    }
                    if (objParent.Name == "logicalrule" || objParent.Name == "case" || objParent.Name == "default" || objParent.Name == "switch") {
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name && objParent.Elements[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Elements[i], objParent.Elements);
                                var curIndex = i;
                                iselements = true;
                                break;
                            }
                        }
                    }

                    if ((cutorcopyitem.Name == "case" || cutorcopyitem.Name == "default") && obj.Name == "switch") {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == "default") {
                                isfound = true;
                                $rootScope.InsertItem(objCutorCopyItem, obj.Elements, i);
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                    else {
                        if ((obj.Name == 'case' && cutorcopyitem.Name == 'case') || (obj.Name == 'default' && cutorcopyitem.Name == 'default') || (obj.Name == 'default' && cutorcopyitem.Name == 'case') ||
                            (obj.Name == 'case' && cutorcopyitem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case')) {
                            alert("If/Else block can be added only inside the Condition");
                        }
                        else {



                            var data = JSON.stringify(cutorcopyitem);
                            var objCutorCopyItem = JSON.parse(data);
                            if (obj.Name == 'case' || obj.Name == 'default') {
                                $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                            }
                            else if (cutorcopyitem.Name == 'case' && obj.Name == 'switch') {
                                var casecount = 0;
                                for (var j = 0; j < obj.Elements.length; j++) {
                                    if (obj.Elements[j].Name == 'case') {
                                        casecount++;
                                    }
                                }
                                $rootScope.InsertItem(cutorcopyitem, obj.Elements, casecount);

                            }
                            else {
                                $rootScope.PushItem(objCutorCopyItem, obj.Children);
                            }
                        }
                    }

                }
                cutorcopyitem = undefined;
            }
            else if (Param == 'copy') {
                var nodeid = generateUUID();
                objCutorCopyItem.dictAttributes.sfwNodeID = nodeid;
                objCutorCopyItem.$$hashKey = generateUUID();
                if (objCutorCopyItem.Children != undefined) {
                    for (var i = 0; i < objCutorCopyItem.Children.length; i++) {
                        objCutorCopyItem.Children = [];
                    }
                }
                ChangeNodeidforCopyObject(objCutorCopyItem);

                if ((obj.Name == 'case' && objCutorCopyItem.Name == 'case') || (obj.Name == 'default' && objCutorCopyItem.Name == 'default') || (obj.Name == 'default' && objCutorCopyItem.Name == 'case')
                    || (obj.Name == 'case' && objCutorCopyItem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case') || (obj.Name != 'switch' && cutorcopyitem.Name == 'default')) {
                    alert("If/Else block can be added only inside the Condition");
                }
                else if (obj.Name == 'case' || obj.Name == 'default') {
                    $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                }
                else if ((objCutorCopyItem.Name == 'case' || objCutorCopyItem.Name == 'default') && obj.Name == 'switch') {
                    if (objCutorCopyItem.Name == 'case') {
                        var casecount = 0;
                        for (var j = 0; j < obj.Elements.length; j++) {
                            if (obj.Elements[j].Name == 'case') {
                                casecount++;
                            }
                        }
                        $rootScope.InsertItem(objCutorCopyItem, obj.Elements, casecount);

                    }
                    else if (objCutorCopyItem.Name == 'default') {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == cutorcopyitem.Name) {
                                alert("Only one Else block can be present");
                                isfound = true;
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                }
                else {
                    $rootScope.PushItem(objCutorCopyItem, obj.Children);

                }

            }
            else if (Param == 'copy++') {
                objCutorCopyItem.dictAttributes.sfwNodeID = generateUUID();
                objCutorCopyItem.$$hashKey = generateUUID();
                ChangeNodeidforCopyObject(objCutorCopyItem);
                if ((obj.Name == 'case' && objCutorCopyItem.Name == 'case') || (obj.Name == 'default' && objCutorCopyItem.Name == 'default') || (obj.Name == 'default' && objCutorCopyItem.Name == 'case')
                    || (obj.Name == 'case' && objCutorCopyItem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case') || (obj.Name != 'switch' && cutorcopyitem.Name == 'default')) {
                    alert("If/Else block can be added only inside the Condition");
                }
                else if (obj.Name == 'case' || obj.Name == 'default') {
                    $rootScope.PushItem(objCutorCopyItem, obj.Elements);

                }
                else if ((objCutorCopyItem.Name == 'case' || objCutorCopyItem.Name == 'default') && obj.Name == 'switch') {
                    if (objCutorCopyItem.Name == 'case') {
                        var casecount = 0;
                        for (var j = 0; j < obj.Elements.length; j++) {
                            if (obj.Elements[j].Name == 'case') {
                                casecount++;
                            }
                        }
                        $rootScope.InsertItem(objCutorCopyItem, obj.Elements, casecount);

                    }
                    else if (objCutorCopyItem.Name == 'default') {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == cutorcopyitem.Name) {
                                alert("Only one Else block can be present");
                                isfound = true;
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                }
                else {
                    $rootScope.PushItem(objCutorCopyItem, obj.Children);
                }
                cutorcopyitem = undefined;
            }
            $rootScope.UndRedoBulkOp("End");

            var rootScope = getCurrentFileScope();
            if (rootScope.objLogicalRule && rootScope.objLogicalRule.dictAttributes && rootScope.objSelectedLogicalRule) {
                rootScope.ResetAllvalues(rootScope.objSelectedLogicalRule);
            }
            objCutorCopyItem.IsSelected = true;

            $timeout(function () {
                if (obj) {
                    checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
                }
            });
        }
    };

    var isChildOfCutItem = function (parentObj, obj) {
        var retVal = false;
        angular.forEach(parentObj.Elements, function (itm) {
            if (!retVal) {
                if (itm.dictAttributes && obj.dictAttributes && itm.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                    retVal = true;
                }
                else {
                    retVal = isChildOfCutItem(itm, obj);
                }
            }
        });
        if (!retVal) {
            angular.forEach(parentObj.Children, function (itm) {
                if (!retVal) {
                    if (itm.dictAttributes && obj.dictAttributes && itm.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                        retVal = true;
                    }
                    else {
                        retVal = isChildOfCutItem(itm, obj);
                    }
                }
            });
        }
        return retVal;
    };

    var ChangeNodeidforCopyObject = function (obj) {
        if (obj.Children != undefined) {
            for (var i = 0; i < obj.Children.length; i++) {
                obj.Children[i].dictAttributes.sfwNodeID = generateUUID();
                if (obj.Children[i].$$hashKey) {
                    obj.Children[i].$$hashKey = generateUUID();
                }
                if (obj.Children[i].Children != undefined) {
                    if (obj.Children[i].Children.length > 0) {
                        ChangeNodeidforCopyObject(obj.Children[i]);
                    }
                }
                else {
                    if (obj.Children[i].Elements.length > 0) {
                        ChangeNodeidforCopyObject(obj.Children[i]);
                    }
                }
            }
        }
        if (obj.Elements != undefined) {
            for (var i = 0; i < obj.Elements.length; i++) {
                obj.Elements[i].dictAttributes.sfwNodeID = generateUUID();
                if (obj.Elements[i].$$hashKey) {
                    obj.Elements[i].$$hashKey = generateUUID();
                }
                if (obj.Elements[i].Elements.length > 0) {
                    ChangeNodeidforCopyObject(obj.Elements[i]);
                }
                if (obj.Elements[i].Children != undefined) {
                    if (obj.Elements[i].Children.length > 0) {
                        ChangeNodeidforCopyObject(obj.Elements[i]);
                    }
                }
            }
        }
    };

    // Delete Step Methods
    var OnDeleteCurrentStepClick = function (obj, objParent) {
        $rootScope.UndRedoBulkOp("Start");
        var retVal = false;
        if (obj.Name == 'case') {
            retVal = DeleteIf(obj, objParent);
        }
        else if (obj.Name == "action") {
            retVal = DeleteAssignBlock(obj, objParent);
        }
        else {
            if (objParent.Name == "logicalrule" || objParent.Name == "switch" || objParent.Name == "case" || objParent.Name == "default" || objParent.Name == "foreach" || objParent.Name == "while") {
                var curIndex = objParent.Elements.indexOf(obj);
                if (curIndex > -1) {
                    $rootScope.DeleteItem(obj, objParent.Elements);

                    //objParent.Elements.splice(curIndex, 1);
                    if (objParent.Name != "switch") {
                        if (obj.Children != undefined) {
                            angular.forEach(obj.Children, function (item) {
                                $rootScope.InsertItem(item, objParent.Elements, curIndex);

                                // objParent.Elements.splice(curIndex, 0, item);
                                curIndex++;
                            });
                        }
                    }
                }
            }
            if (objParent.Name != "logicalrule" && objParent.Name != "case" && objParent.Name != "default") {
                var curIndex = objParent.Children.indexOf(obj);
                if (curIndex > -1) {
                    $rootScope.DeleteItem(obj, objParent.Children);

                    //objParent.Children.splice(curIndex, 1);

                    if (obj.Children != undefined && obj.Children.length > 0) {
                        angular.forEach(obj.Children, function (item) {
                            $rootScope.InsertItem(item, objParent.Children, curIndex);


                            // objParent.Children.splice(curIndex, 0, item);
                            curIndex++;
                        });
                    }
                }
            }
            retVal = true;
        }
        $rootScope.UndRedoBulkOp("End");

        return retVal;
    };

    var DeleteIf = function (obj, objParent) {
        var retVal = false;
        var iCount = GetIfAndAssignBlockCount(objParent, obj.Name);
        if (iCount == 1) {
            alert("One If Block Should be there");
        }
        else {
            $rootScope.DeleteItem(obj, objParent.Elements);

            // objParent.Elements.splice(objParent.Elements.indexOf(obj), 1);
            retVal = true;
        }
        return retVal;
    };

    var GetIfAndAssignBlockCount = function (objParent, strName) {
        var iCount = 0;
        angular.forEach(objParent.Elements, function (item) {
            if (item.Name == strName) {
                iCount++;
            }
        });
        return iCount;
    };

    var DeleteAssignBlock = function (obj, objParent) {
        var retVal = false;
        var iCount = GetIfAndAssignBlockCount(objParent);
        if (iCount == 1) {
            alert("One Assign Should be there");
        }
        else {
            $rootScope.DeleteItem(obj, objParent.Elements);

            // objParent.Elements.splice(objParent.Elements.indexOf(obj), 1);
            retVal = true;
        }
    };

    var AddAction = function (obj) {
        var nodeid = generateUUID();
        var objaction = { Elements: [], dictAttributes: { sfwNodeID: nodeid, sfwExpression: "" }, Children: [], IsSelected: false, Name: "action", Value: "" };
        $rootScope.PushItem(objaction, obj.Elements);
    };

    var DeleteAction = function (obj) {
        if (obj.Elements.length > 1) {
            $rootScope.DeleteItem(obj.Elements[obj.Elements.length - 1], obj.Elements);

            // obj.Elements.splice(obj.Elements.length - 1, 1);
        }
    };

    return {
        restrict: "E",
        scope: {
            model: "=",
            parentModel: "=",
            viewMode: "=",
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getNodeTemplate = function () {
                var templateText = "";
                switch (scope.model.Name) {
                    case "actions":
                        templateText = '<actions-node></actions-node>';
                        break;
                    case "break":
                    case "continue":
                    case "default":
                        templateText = '<break-continue-default-node></break-continue-default-node>';
                        break;
                    case "notes":
                        templateText = '<notes-node></notes-node>';
                        break;
                    case "return":
                    case "switch":
                    case "case":
                    case "while":
                        templateText = '<return-switch-case-while-node></return-switch-case-while-node>';
                        break;
                    case "calllogicalrule":
                    case "calldecisiontable":
                    case "callexcelmatrix":
                        templateText = '<call-rule-node></call-rule-node>';
                        break;
                    case "foreach":
                        templateText = '<for-each-node></for-each-node>';
                        break;
                    case "query":
                        templateText = '<query-node></query-node>';
                        break;
                    case "method":
                        templateText = '<method-node></method-node>';
                        break;

                }
                return templateText;
            };
            scope.getTemplate = function () {
                var templateTextList = ['<div logical-rule-drag-drop node-id="{{::model.dictAttributes.sfwNodeID}}" node-name="{{::model.Name}}" ng-keydown="onkeydownevent($event,model,parentModel)">                                                                                 '
                    , '    <span ng-show="(model.Name === \'foreach\' || model.Name === \'while\' || model.Name === \'switch\') || (model.Name !== \'case\' && model.Name !== \'default\' && model.Children.length > 0) || ((model.Name === \'case\' || model.Name === \'default\') && model.Elements.length > 0)" class="node-expander node-expanded" ng-click="toggleNodeChildren($event)"></span>                                                                                              '
                    , '    <div class="node-wrapper node-wrapper-after">                                                                                                                                        '];
                templateTextList.push(scope.getNodeTemplate());
                templateTextList = templateTextList.concat(['<ul  logical-rule-drag-drop ng-if="model.Name == \'foreach\' || model.Name == \'while\' || model.Name == \'switch\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                    , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                    , '                <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                           '
                    , '            </li>                                                                                                                                                                        '
                    , '                                                                                                                                                                                         '
                    , '        </ul>                                                                                                                                                                            '
                    , '    </div>                                                                                                                                                                               '
                    , '    <ul ng-if="(model.Name == \'case\' || model.Name == \'default\') && model.Elements.length>0">                                                                                        '
                    , '        <li ng-repeat="item in model.Elements">                                                                                                                                          '
                    , '            <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                               '
                    , '        </li>                                                                                                                                                                            '
                    , '    </ul>                                                                                                                                                                                '
                    , '    <ul ng-if="model.Name != \'case\' && model.Name != \'default\' && model.Children.length>0">                                                                                          '
                    , '        <li ng-repeat="item in model.Children">                                                                                                                                          '
                    , '            <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                               '
                    , '        </li>                                                                                                                                                                            '
                    , '    </ul>                                                                                                                                                                                '
                    , '</div>                                                                                                                                                                                   ']);
                return templateTextList.join("");
            };
            scope.toggleParameters = function (event) {
                if (scope.parameters && scope.parameters.Elements.length > 0) {
                    var nodeElement = $(event.target).parents(".node").first();
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.showParameters(nodeElement);
                    }
                    else {
                        scope.hideParameters(nodeElement);
                    }
                }
            };
            scope.showParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideDown("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").addClass("fa-caret-down");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").removeClass("fa-caret-right");
            }
            scope.hideParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideUp("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").addClass("fa-caret-right");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").removeClass("fa-caret-down");
            }
            scope.syncParameterVisibility = function (nodeElement) {
                $timeout(function () {
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.hideParameters(nodeElement);
                    }
                    else {
                        scope.showParameters(nodeElement);
                    }
                });
            }
            scope.toggleNodeChildren = function (event) {
                if ($(event.target).hasClass("node-expanded")) {
                    collapseNode(event.target, true);
                }
                else {
                    expandNode(event.target, true);
                }
            };
            scope.onStepSelectChange = function (step) {
                var currentFileScope = getCurrentFileScope();
                currentFileScope.onSelectChange(step);
                scope.SelectedNode = step;
            };
            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    if (controllerScope.onActionKeyDown) {
                        controllerScope.SelectedNode = scope.SelectedNode;
                        controllerScope.onActionKeyDown(eargs);
                    }
                }
            };
            scope.onDescriptionKeyDown = function (eargs) {
            }; // Comes from a xml file, which we cannot access from web.
            scope.init = function () {
                if (scope.model.Name === "switch" || scope.model.Name === "foreach" || scope.model.Name === "while") {
                    scope.childrenCollection = scope.model.Elements;
                }

                if (scope.model.Name === "case" || scope.model.Name === "default") {
                    scope.siblingsCollection = scope.model.Elements;
                }
                else {
                    scope.siblingsCollection = scope.model.Children;
                }


                if (scope.model.Elements.length > 0) {
                    var parametersFilter = function (item) {
                        return item.Name == "parameters";
                    };
                    var parametersModel = scope.model.Elements.filter(parametersFilter);
                    if (parametersModel && parametersModel.length > 0) {
                        scope.parameters = parametersModel[0];
                    }

                }

                scope.menuOptions = [
                    ['AddItem', [
                        ['Condition', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'switch');
                        }],
                        ['Action', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'actions');
                        }],
                        ['Method', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'method');
                        }],
                        ['CallLogicalRule', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'calllogicalrule');
                        }],
                        ['CallDecisionTable', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'calldecisiontable');
                        }],
                        ['CallExcelMatrix', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'callexcelmatrix');
                        }],
                        ['Loop', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'foreach');
                        }],
                        ['Return', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'return');
                        }],
                        ['While', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'while');
                        }],
                        ['Break', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'break');
                        }],
                        ['Continue', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'continue');
                        }],
                        ['Query', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'query');
                        }],
                        ['Notes', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'notes');
                        }]
                    ]],
                    null, // Dividier
                    ['Add Step', [
                        ['Condition', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'switch');
                        }],
                        ['Action', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'actions');
                        }],
                        ['Method', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'method');
                        }],
                        ['CallLogicalRule', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'calllogicalrule');
                        }],
                        ['CallDecisionTable', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'calldecisiontable');
                        }],
                        ['CallExcelMatrix', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'callexcelmatrix');
                        }],
                        ['Loop', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'foreach');
                        }],
                        ['Return', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'return');
                        }],
                        ['While', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'while');
                        }],
                        ['Break', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'break');
                        }],
                        ['Continue', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'continue');
                        }],
                        ['Query', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'query');
                        }],
                        ['Notes', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'notes');
                        }]
                    ], function ($itemScope) {
                        return ($itemScope.model.Name.match(/foreach/) || $itemScope.model.Name.match(/while/)) != null;
                    }],
                    null,
                    ['Add if', function ($itemScope) {
                        var obj = scope.model;
                        AddStep(obj, 'case');
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/switch/) != null;
                    }
                    ],
                    null,
                    ['Add else', function ($itemScope) {
                        var obj = scope.model;
                        AddStep(obj, 'default');
                    }, function ($itemScope) {
                        for (var i = 0; i < $itemScope.model.Elements.length; i++) {
                            if ($itemScope.model.Elements[i].Name == 'default') {
                                return false;
                            }
                        }
                        return $itemScope.model.Name.match(/switch/) != null;
                    }
                    ],
                    null,
                    ['Add Action', function ($itemScope) {
                        var obj = scope.model;
                        AddAction(obj);
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/actions/) != null;
                    }
                    ],
                    null,
                    ['Delete Action', function ($itemScope) {
                        var obj = scope.model;
                        DeleteAction(obj);
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/actions/) != null && scope.model.Elements.length > 1;
                    }
                    ],
                    null,
                    ['Delete', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        if (lobjparent != undefined) {
                            OnDeleteCurrentStepClick(obj, lobjparent);
                        }
                    }, function ($itemScope) {
                        if ($itemScope.model.Name == "case") {
                            var count = 0;

                            for (var i = 0; i < $itemScope.parentModel.Elements.length; i++) {
                                if ($itemScope.parentModel.Elements[i].Name == 'case') {
                                    count++;
                                }
                            }
                            if (count <= 1) {
                                return false;
                            }
                            else {
                                return true;
                            }
                        }
                        else {
                            return true;
                        }
                    }],
                    null,
                    ['Cut', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'cut');
                    }],
                    null,
                    ['Copy', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'copy');
                    }],
                    null,
                    ['Copy++', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'copy++');
                    }],
                    null,
                    ['Paste', function ($itemScope) {
                        var obj = scope.model;
                        PasteItem(obj);
                    }, function ($itemScope) {
                        var obj = scope.model;
                        if (cutorcopyitem) {
                            if (isChildOfCutItem(cutorcopyitem, obj) && Param === 'cut') {
                                return false;
                            }
                            else if (scope.model == cutorcopyitem && Param === 'cut') {
                                return false;
                            }
                            else {
                                return true;
                            }
                        }
                        return true;
                    }],
                    null,
                    ['Toggle All Outlining', function ($itemScope) {
                        if ($itemScope.model) {
                            ToggleAllOutLining($("div[node-id='" + $itemScope.model.dictAttributes.sfwNodeID + "']").parents(".logical-rule-first-ul").first());
                        }
                    }],
                    null,
                    ['Comment', function ($itemScope) {
                        var obj = scope.model;
                        $rootScope.EditPropertyValue(obj.dictAttributes.sfwCommented, obj.dictAttributes, "sfwCommented", "True");
                    }, function ($itemScope) {
                        return $itemScope.model.dictAttributes.sfwCommented != "True" && $itemScope.model.Name != "case" && $itemScope.model.Name != "default";
                    }
                    ],
                    null,

                    ['Uncomment', function ($itemScope) {
                        var obj = scope.model;
                        $rootScope.EditPropertyValue(obj.dictAttributes.sfwCommented, obj.dictAttributes, "sfwCommented", "False");
                    }, function ($itemScope) {
                        return $itemScope.model.dictAttributes.sfwCommented == "True" && $itemScope.model.Name != "case" && $itemScope.model.Name != "default";
                    }],
                    null
                ];

                if (scope.model.IsSelected) {
                    scope.onStepSelectChange(scope.model);
                }
            };
            scope.getCssClass = function (model) {
                var cssClass = "";
                if (model.dictAttributes.sfwCommented === 'True') {
                    cssClass = "commented-node";
                }
                if (model.IsSelected) {
                    cssClass += " selected-node";
                    if (model.isAdvanceSearched) {
                        cssClass += ' bckgGreen';
                    }
                }
                else if (model.isAdvanceSearched) {
                    cssClass += ' bckgGrey';
                }
                return cssClass;
            }
            scope.init();

            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);

                if (scope.model.Name === "foreach" || scope.model.Name === "while") {
                    $timeout(function () {
                        var parentLoopsCount = ul.parents(".ul-children-even,.ul-children-odd").length;
                        if (parentLoopsCount > 0 && parentLoopsCount % 2 == 1) {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-even");
                        }
                        else {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-odd");
                        }
                    });
                }

            }
        },
        //templateUrl: "Rule/views/LogicalRule/NodeWrapperTemplate.html"
    };
}]);
function checkAndCollapseNode(nodeId) {
    var node = $("div[node-id='" + nodeId + "']");
    if (node && node.length > 0) {
        if (node.find("> .node-expander").hasClass("node-collapsed")) {
            collapseNode(node, true);
        }
    }
}
function expandNode(element, skipAnimation) {
    $(element).parents("li").first().find("> div > .node-wrapper > ul").show();
    if (skipAnimation) {
        $(element).parents("li").first().find("> div > ul").show();
        $(element).parents("li").first().find("> div > .node-wrapper > ul > li").show();
    }
    else {
        $(element).parents("li").first().find("> div > ul").slideDown("slow");
        $(element).parents("li").first().find("> div > .node-wrapper > ul > li").slideDown("slow");
    }
    $(element).parents("li").first().find("> div > .node-wrapper > ul").css("min-height", "");
    $(element).siblings(".node-wrapper").addClass("node-wrapper-after");
    $(element).removeClass("node-collapsed");
    $(element).addClass("node-expanded");
};
function collapseNode(element, skipAnimation) {
    $(element).parents("li").first().find("> div > .node-wrapper > ul").css("min-height", "0px");
    if (skipAnimation) {
        $(element).parents("li").first().find("> div > ul").hide();
        $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
        $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
    }
    else {
        $(element).parents("li").first().find("> div > ul").slideUp("slow", function () {
            $(element).parents("li").first().find("> div > ul").hide();
            $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
        });
        if ($(element).parents("li").first().find("> div > .node-wrapper > ul > li").length > 0) {
            $(element).parents("li").first().find("> div > .node-wrapper > ul > li").slideUp("slow", function () {
                $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
                $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
            });
        }
        else {
            $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
            $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
        }
    }
    $(element).removeClass("node-expanded");
    $(element).addClass("node-collapsed");
};

app.directive('actionsNode', ["$rootScope", "$timeout", function ($rootScope, $timeout) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.actionIndex = null;
            scope.setCurrentAction = function (index) {
                scope.actionIndex = index;
            };

            scope.onkeydownevent = function (event) {
                if (scope.model.Name == "actions") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.ENTER) {

                        var nodeid = generateUUID();
                        var obj = { Elements: [], dictAttributes: { sfwNodeID: nodeid, sfwExpression: "" }, Children: [], IsSelected: false, Name: "action", Value: "" };
                        if (scope.actionIndex || scope.actionIndex === 0) {
                            $rootScope.InsertItem(obj, scope.model.Elements, scope.actionIndex + 1);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.model.Elements);
                        }

                        $timeout(function () {
                            enableExpressionEditing(scope.model.dictAttributes.sfwNodeID, scope.actionIndex + 1);
                        });

                        //stop propagation so that same event won't be called for parent action.
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        }

                        //Due to above statement, it won't call keydown event for body and so in case of Ctrl+S it won't prevent default action.
                        //So explicitely prevent default action so that if user press Ctrl+S on action node, it won't show default save dialog.
                        if (event.preventDefault) {
                            event.preventDefault();
                        }
                    }
                    else if (event.ctrlKey && event.keyCode == $.ui.keyCode.DELETE) {
                        $(".page-header-fixed").css("pointer-events", "auto");
                        if (scope.model.Elements.length > 1) {
                            if (scope.actionIndex || scope.actionIndex === 0) {
                                $rootScope.DeleteItem(scope.model.Elements[scope.actionIndex], scope.model.Elements);
                            }
                            else {
                                $rootScope.DeleteItem(scope.model.Elements[scope.model.Elements.length - 1], scope.model.Elements);
                            }

                            if (scope.actionIndex > scope.model.Elements.length) {
                                scope.actionIndex--;
                            }
                        }

                        //stop propagation so that same event won't be called for parent action.
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        }
                        //Due to above statement, it won't call keydown event for body and so in case of Ctrl+S it won't prevent default action.
                        //So explicitely prevent default action so that if user press Ctrl+S on action node, it won't show default save dialog.
                        if (event.preventDefault) {
                            event.preventDefault();
                        }
                    }
                }
            };

        },
        templateUrl: "Rule/views/LogicalRule/ActionsNodeTemplate.html",
    };
}]);
app.directive('breakContinueDefaultNode', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Rule/views/LogicalRule/BreakContinueDefaultNodeTemplate.html",
    };
}]);
app.directive('callRuleNode', ["$rootScope", "$EntityIntellisenseFactory", function ($rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onRuleIDKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var input = eargs.target;
                //var arrText = getSplitArray($(input).val(), input.selectionStart);
                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));
                //if (arrText[arrText.length - 1] == "") {
                //    arrText.pop();
                //}
                rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);

                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                else {
                    entityID = rootScope.objLogicalRule.Entity;
                }

                var ruleType;
                if (scope.model.Name == "calllogicalrule") {
                    ruleType = "LogicalRule";
                }
                else if (scope.model.Name == "calldecisiontable") {
                    ruleType = "DecisionTable";
                }
                else if (scope.model.Name == "callexcelmatrix") {
                    ruleType = "ExcelMatrix";
                }

                //Get all the entities which have static rule of the specified rule type.
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var requiredEntities = entities.filter(function (x) { return x.Rules.some(function (element, index, array) { return element.IsStatic && (x.ID == entityID || !element.IsPrivate) && element.RuleType == ruleType; }); });

                var rules = [];
                var lstObject = [];
                //If current rule is non-static then load other non-static rules of the current entity.
                var isstatic = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    isstatic = rootScope.objLogicalRule.dictAttributes.sfwStatic;
                }
                else {
                    isstatic = rootScope.objLogicalRule.Static;
                }

                var lstRulesAndObject = [];
                if (isstatic.toLowerCase() != "true") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityname = entityID;
                    while (parententityname != "") {
                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            rules = entity[0].Rules.filter(function (y) { return !y.IsStatic && (parententityname == entityID || !y.IsPrivate) && y.RuleType == ruleType; });
                            lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                            if (entityID != parententityname) {
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
                data = data.concat(rootScope.lstSelectedAliasobjCollection);
                data = data.concat(rootScope.getVarParamAttributes(["Object"], true, entityID));

                var aliasArray = [];
                var arrayindex = 0;
                var isAlias = false;
                if (arrText.length > 1) {
                    for (var i = 0; i < arrText.length; i++) {
                        for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                            if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                arrayindex = i;
                                isAlias = true;
                                arrText.splice(i, 1);
                                aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                break;
                            }
                        }
                    }
                }

                for (var k = aliasArray.length - 1; k >= 0; k--) {
                    arrText.splice(arrayindex, 0, aliasArray[k]);
                }

                if (!isAlias) {

                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0 && typeof item[0].Rules != "undefined" && index < arrText.length) {
                                if (item[0].ID == arrText[index]) {
                                    data = item[0].Rules.filter(function (y) { return y.IsStatic && (item[0].ID == entityID || !y.IsPrivate) && y.RuleType == ruleType; });
                                }
                            }
                            else if (item.length > 0 && item[0].DataType != undefined && index < arrText.length) {
                                if (item[0].DataType == "Object" && item[0].ID == arrText[index]) {
                                    parententityname = item[0].Entity;
                                    data = [];
                                    while (parententityname != "") {
                                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                                        if (entity && entity.length > 0) {
                                            data = data.concat(entity[0].Rules.filter(function (y) { return !y.IsStatic && !y.IsPrivate && y.RuleType == ruleType; }));
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
                }
                else {
                    data = scope.GetAliasData(arrText, rootScope, ruleType);
                }

                // filtering rules 
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }


                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.onCallRuleDoubleClick = function () {
                var RuleID = scope.model.dictAttributes.sfwRuleID;
                if (RuleID != undefined) {
                    var lastdot = RuleID.lastIndexOf(".");
                    RuleID = RuleID.substring(lastdot + 1);
                    $.connection.hubMain.server.navigateToFile(RuleID, "").done(function (objfile) {
                        $rootScope.openFile(objfile);
                    });
                }
            };
            scope.getExpressionValue = function (parameter, oldParams) {
                var Expression = "";
                var lst = oldParams.filter(function (x) { return x.dictAttributes.ID == parameter.dictAttributes.ID && x.dictAttributes.sfwDataType == parameter.dictAttributes.sfwDataType && x.dictAttributes.sfwDirection == parameter.dictAttributes.sfwDirection; });
                if (lst && lst.length > 0) {
                    Expression = lst[0].dictAttributes.sfwExpression;
                }
                return Expression;
            };
            scope.refreshRuleParameters = function (event) {
                var rootScope = getCurrentFileScope();
                if (rootScope && rootScope.currentfile.FileName) {
                    $rootScope.ClearUndoRedoListByFileName(rootScope.currentfile.FileName);
                }
                var RuleName = scope.model.dictAttributes.sfwRuleID;
                var RuleID = "";
                var arrayText = [];
                if (!RuleName) {
                    RuleName = "";
                }
                if (RuleName.contains(".")) {
                    arrayText = RuleName.split(".");
                }
                if (arrayText.length > 0) {
                    RuleID = arrayText[arrayText.length - 1];
                }
                else {
                    RuleID = RuleName;
                }

                var parameters;
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        parameters = item;
                    }
                });
                $rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    scope.parameters = parameters;
                    $rootScope.PushItem(parameters, scope.model.Elements);
                    // scope.model.Elements.push(parameters);
                }
                var oldParams = [];
                angular.forEach(parameters.Elements, function (param) {
                    oldParams.push(param);
                });
                parameters.Elements = [];
                var isFound = false;
                if (RuleID != "") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < entityIntellisenseList.length; i++) {
                        if (entityIntellisenseList[i].Rules.length > 0) {
                            for (var j = 0; j < entityIntellisenseList[i].Rules.length; j++) {
                                if (entityIntellisenseList[i].Rules[j].ID == RuleID) {

                                    intellisenseruleobject = entityIntellisenseList[i].Rules[j];
                                    if (intellisenseruleobject != undefined) {
                                        angular.forEach(intellisenseruleobject.Parameters, function (param) {
                                            var parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType, sfwDirection: param.Direction }, Elements: [], Children: [] };
                                            parameter.dictAttributes.sfwExpression = scope.getExpressionValue(parameter, oldParams);
                                            if (param.DataType === "Collection" || param.DataType === "List" | param.DataType === "Object") {
                                                parameter.dictAttributes.sfwEntity = param.Entity;
                                            }
                                            parameter.dictAttributes.sfwNodeID = generateUUID();

                                            $rootScope.PushItem(parameter, parameters.Elements);
                                            //parameters.Elements.push(parameter);
                                        });
                                    }
                                    isFound = true;
                                    break;
                                }
                            }
                        }
                        if (isFound) {
                            break;
                        }
                    }
                }
                $rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);

            };
        },
        templateUrl: "Rule/views/LogicalRule/CallRuleNodeTemplate.html",
    };
}]);
app.directive('forEachNode', ["$rootScope", "$EntityIntellisenseFactory", function ($rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onLoopCollectionkeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var data = [];
                var input = eargs.target;
                //checking for alias objects
                rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);

                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));
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
                var currentopenentity = "";
                if (entity && entity.length > 0) {
                    currentopenentity = entity[0];
                }
                data = data.concat(rootScope.getVarParamAttributes(["CDOCollection", "Collection", "List", "Object"], true, entityName));
                if (rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "false") {
                    if (entity && entity.length > 0) {
                        var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                        data = data.concat(queries);
                    }

                    //If current rule is non-static then load other non-static rules of the current entity.
                    var isstatic = "";
                    if (rootScope.objLogicalRule.dictAttributes) {
                        isstatic = rootScope.objLogicalRule.dictAttributes.sfwStatic;
                    }
                    else {
                        isstatic = rootScope.objLogicalRule.Static;
                    }

                    if (entity && entity.length > 0 && entity[0] && entity[0].ParentId != "" && isstatic.toLowerCase() != "true") {
                        parententityname = entity[0].ParentId;
                        while (parententityname != "") {
                            entity = entities.filter(function (x) { return x.ID == parententityname; });
                            if (entity && entity.length > 0) {
                                var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                                data = data.concat(queries);
                            }
                            var attributes = $rootScope.getEntityAttributeIntellisense(parententityname, false);

                            attributes = attributes.filter(function (y) { return (y.DataType == "Collection" || y.DataType == "CDOCollection" || y.DataType == "Object" || y.DataType == "List"); });
                            data = data.concat(attributes);
                            if (entity && entity.length > 0) {
                                parententityname = entity[0].ParentId;
                            } else {
                                parententityname = "";
                            }
                        }
                    }
                }

                // Adding alias objects to data
                data = data.concat(rootScope.lstSelectedAliasobjCollection);

                var aliasArray = [];
                var arrayindex = 0;
                var isAlias = false;
                if (arrText.length > 1) {
                    for (var i = 0; i < arrText.length; i++) {
                        for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                            if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                arrayindex = i;
                                isAlias = true;
                                arrText.splice(i, 1);
                                aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                break;
                            }
                        }
                    }
                }

                for (var k = aliasArray.length - 1; k >= 0; k--) {
                    arrText.splice(arrayindex, 0, aliasArray[k]);
                }
                //var datacollection = data;
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        //data = datacollection;
                        //if (arrText[index] != "") {
                        //var item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()) });
                        var item = data.filter(function (x) { return x.ID == arrText[index]; });
                        if (item.length > 0) {
                            if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined" && item[0].ID == arrText[index] && index < arrText.length - 1) {
                                data = [];
                                parententityname = item[0].Entity;
                                while (parententityname != "") {
                                    entity = entities.filter(function (x) { return x.ID == parententityname; });
                                    if (entity && entity.length > 0) {
                                        //var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery" });
                                        //data = data.concat(queries);
                                    }
                                    data = data.concat($rootScope.getEntityAttributeIntellisense(parententityname, false).filter(function (x) { return (x.DataType == "Object" || x.DataType == "Collection" || x.DataType == "CDOCollection" || x.DataType == "List"); }));
                                    if (entity && entity.length > 0) {
                                        parententityname = entity[0].ParentId;
                                    } else {
                                        parententityname = "";
                                    }
                                }
                            }
                            else if (typeof item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && index < arrText.length - 1) {
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
                        //}
                    }
                }
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }

                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }

                //else {
                //    //data = [];
                //    setRuleIntellisense($(input), data, scope);
                //    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", null);
                //}
            };
        },
        templateUrl: "Rule/views/LogicalRule/ForEachNodeTemplate.html",
    };
}]);
app.directive('methodNode', ["$rootScope", "$EntityIntellisenseFactory", "$IentBaseMethodsFactory", function ($rootScope, $EntityIntellisenseFactory, $IentBaseMethodsFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onMethodNameKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var input = eargs.target;
                //var arrText = getSplitArray($(input).val(), input.selectionStart);
                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));

                if (arrText[arrText.length - 1] == "") {
                    arrText.pop();
                }

                var data = [];
                //rootScope.businessObjectMethods;
                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var parententityname = entityID;
                while (parententityname != "") {
                    var entity = entities.filter(function (x) { return x.ID == parententityname; });
                    if (entity && entity.length > 0 && entity[0].BusinessObjectName) {
                        var lstObjectMethods = [];
                        lstObjectMethods = entity[0].ObjectMethods.filter(function (item) { return item.ID; });
                        data = data.concat(lstObjectMethods);
                        parententityname = entity[0].ParentId;
                    } else {
                        parententityname = "";
                        var ientBaseMethods = $IentBaseMethodsFactory.getIentBaseMethods();
                        if (ientBaseMethods && ientBaseMethods.length) {
                            data = data.concat(ientBaseMethods);
                        }
                    }
                }

                // filtering method 
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }
                if (input.innerText.contains(".")) {
                    data = [];
                }

                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.getExpressionValue = function (parameter, oldParams) {
                var Expression = "";
                var lst = oldParams.filter(function (x) { return x.dictAttributes.ID == parameter.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    Expression = lst[0].dictAttributes.sfwExpression;
                }
                return Expression;
            };
            scope.refreshMethodParameters = function (event) {
                var MethodName = scope.model.dictAttributes.sfwMethodName;
                var parameters;
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        parameters = item;
                    }
                });
                var rootScope = getCurrentFileScope();
                $rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    $rootScope.PushItem(parameters, scope.model.Elements);

                    // scope.model.Elements.push(parameters);
                }
                var oldParams = [];
                angular.forEach(parameters.Elements, function (param) {
                    oldParams.push(param);
                });
                parameters.Elements = [];

                var lstMethod = [];
                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var entity = entities.filter(function (x) {
                    return x.ID == entityID;
                });

                if (entity && entity.length > 0 && entity[0].BusinessObjectName) {
                    lstMethod = entity[0].ObjectMethods;
                } else {
                    var ientBaseMethods = $IentBaseMethodsFactory.getIentBaseMethods();
                    if (ientBaseMethods && ientBaseMethods.length) {
                        lstMethod = lstMethod.concat(ientBaseMethods);
                    }
                }
                for (var i = 0; i < lstMethod.length; i++) {
                    if (lstMethod[i].ID == MethodName) {
                        var intellisenseruleobject = lstMethod[i];
                        angular.forEach(intellisenseruleobject.Parameters, function (param) {
                            var parameter = null;
                            if (intellisenseruleobject.MethodName) {
                                parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ParameterName, sfwDataType: param.ParameterType.Name, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                            } else {
                                parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType, sfwEntity: param.Entity, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                            }
                            parameter.dictAttributes.sfwExpression = scope.getExpressionValue(parameter, oldParams);
                            $rootScope.PushItem(parameter, parameters.Elements);

                            //parameters.Elements.push(parameter);
                        });
                        break;
                    }
                }
                $rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);
            };
        },
        templateUrl: "Rule/views/LogicalRule/MethodNodeTemplate.html",
    };
}]);
app.directive('notesNode', ["$rootScope", function ($rootScope) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.editNotes = function (notesobj) {
                var objele = null;
                if (notesobj.Elements.length == 0) {
                    objele = { Children: [], Elements: [], Name: "Text", Value: "", dictAttributes: {} };
                    notesobj.Elements.push(objele);
                }
                else {
                    objele = notesobj.Elements[0];
                }
                var newscope = scope.$new();
                var data = JSON.stringify(objele);
                var objnotes = JSON.parse(data);
                newscope.objText = objnotes;
                newscope.dialog = $rootScope.showDialog(newscope, "Edit Notes", "Rule/views/EditNotesTemplate.html", { width: 400, height: 400 });
                newscope.SaveChangedTextData = function () {
                    $rootScope.EditPropertyValue(objele.Value, objele, "Value", newscope.objText.Value);
                    objele.IsValueInCDATAFormat = true;
                    newscope.onCancelClick();
                };

                newscope.onCancelClick = function () {
                    if (newscope.dialog) {
                        newscope.dialog.close();
                    }
                };
            };
        },
        templateUrl: "Rule/views/LogicalRule/NotesNodeTemplate.html",
    };
}]);
app.directive('queryNode', ["$rootScope", "$EntityIntellisenseFactory", "$NavigateToFileService", function ($rootScope, $EntityIntellisenseFactory, $NavigateToFileService) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onQueryIDKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                // if (rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "false") 
                {
                    var input = eargs.target;
                    rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                    rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);
                    var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));

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
                    var currentopenentity = "";
                    if (entity[0]) {
                        currentopenentity = entity[0];
                    } else {
                        currentopenentity = "";
                    }

                    var parententityname = entityName;
                    var data = [];

                    if (parententityname != "") {
                        entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            data = data.concat(entity[0].Queries);
                        }
                    }

                    //We are showing only current entity query

                    //while (parententityname != "") {
                    //    entity = entities.filter(function (x) { return x.ID == parententityname });
                    //    if (entity.length > 0) {
                    //        data = data.concat(entity[0].Queries);
                    //    }
                    //    var lstobjects = [];
                    //    if (entity[0]) {
                    //        lstobjects = entity[0].Attributes.filter(function (x) { return x.DataType == "Object" });
                    //        data = data.concat(lstobjects);
                    //        parententityname = entity[0].ParentId;
                    //    } else {
                    //        parententityname = "";
                    //    }
                    //}
                    data = data.concat(rootScope.lstSelectedAliasobjCollection);

                    var aliasArray = [];
                    var arrayindex = 0;
                    var isAlias = false;
                    if (arrText.length > 1) {
                        for (var i = 0; i < arrText.length; i++) {
                            for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                                if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                    arrayindex = i;
                                    isAlias = true;
                                    arrText.splice(i, 1);
                                    aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                    break;
                                }
                            }
                        }
                    }

                    for (var k = aliasArray.length - 1; k >= 0; k--) {
                        arrText.splice(arrayindex, 0, aliasArray[k]);
                    }
                    if (!isAlias) {
                        if (arrText.length > 0) {
                            for (var index = 0; index < arrText.length; index++) {
                                //data = datacollection;
                                //if (arrText[index] != "") {
                                //var item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()) });
                                var item = data.filter(function (x) { return x.ID == arrText[index]; });
                                if (item.length > 0) {
                                    if (typeof item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && index < arrText.length - 1) {

                                        if (item[0].isQueryfieldsLoaded) {
                                            data = item[0].lstQueryFields;
                                        } else {
                                            rootScope.getQueryFields(item[0].Entity, item[0].ID);
                                            data = [];
                                        }
                                    }
                                    else if (item[0].DataType != "undefined" && item[0].DataType == "Object") {
                                        var parententityname = item[0].Entity;
                                        var data = [];
                                        while (parententityname != "") {
                                            entity = entities.filter(function (x) { return x.ID == parententityname; });
                                            if (entity && entity.length > 0) {
                                                data = data.concat(entity[0].Queries);
                                            }
                                            var lstobjects = [];
                                            if (entity && entity.length > 0) {
                                                lstobjects = entity[0].Attributes.filter(function (x) { return x.DataType == "Object"; });
                                                data = data.concat(lstobjects);
                                                parententityname = entity[0].ParentId;
                                            } else {
                                                parententityname = "";
                                            }
                                        }
                                        data = data.concat(rootScope.lstSelectedAliasobjCollection);
                                    }
                                    else {
                                        data = item;
                                    }
                                } else {
                                    if (index != arrText.length - 1) {
                                        data = [];
                                    }
                                }
                                //}
                            }
                        }
                    }
                    else {
                        data = scope.GetAliasData(arrText, rootScope);
                    }

                    // filtering query 
                    var item = [];
                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                        }
                        data = item;
                    }
                    setRuleIntellisense($(input), data, scope);

                    if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        eargs.preventDefault();
                    }
                }
                //else {
                //    data = [];
                //    setRuleIntellisense($(input), data, scope);
                //    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", null);
                //}
            };
            scope.navigateToQuery = function (item) {
                var currentFileScope = getCurrentFileScope();
                var queryID = item.dictAttributes.sfwQueryID;
                if (queryID && queryID != "") {
                    if (currentFileScope.objLogicalRule.dictAttributes) {
                        $NavigateToFileService.NavigateToFile(currentFileScope.objLogicalRule.dictAttributes.sfwEntity, "queries", queryID);
                    }
                    else {
                        $NavigateToFileService.NavigateToFile(currentFileScope.objLogicalRule.Entity, "queries", queryID);
                    }
                }

            };
            scope.refreshQueryParameters = function (event) {
                var QueryID = scope.model.dictAttributes.sfwQueryID;
                var parameters;
                var tempParametersList = [];
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        var param = { Children: item.Children, dictAttributes: item.dictAttributes, IsSelected: item.IsSelected, Name: item.Name, value: item.value, Elements: item.Elements };
                        parameters = item;
                        tempParametersList = param;
                    }
                });

                var entityid = "";

                var rootScope = getCurrentFileScope();

                if (rootScope.objLogicalRule.dictAttributes) {
                    entityid = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                else {
                    entityid = rootScope.objLogicalRule.Entity;
                }
                //$rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    scope.model.Elements.push(parameters);
                    //$rootScope.PushItem(parameters, scope.model.Elements);
                }

                parameters.Elements = [];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                for (var i = 0; i < entityIntellisenseList.length; i++) {
                    if (entityIntellisenseList[i].ID == entityid) {
                        if (entityIntellisenseList[i].Queries.length > 0) {
                            for (var j = 0; j < entityIntellisenseList[i].Queries.length; j++) {
                                if (entityIntellisenseList[i].Queries[j].ID == QueryID) {

                                    intellisenseruleobject = entityIntellisenseList[i].Queries[j];
                                    if (intellisenseruleobject != undefined) {
                                        angular.forEach(intellisenseruleobject.Parameters, function (param) {

                                            var parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType }, Elements: [], Children: [] };

                                            angular.forEach(tempParametersList.Elements, function (item) {
                                                if (item.dictAttributes.ID == param.ID) {
                                                    parameter.dictAttributes.sfwExpression = item.dictAttributes.sfwExpression;

                                                }

                                            });
                                            //$rootScope.PushItem(parameter, parameters.Elements);
                                            parameters.Elements.push(parameter);
                                            // parameters.Elements.push(parameter);
                                        });
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                //$rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);
            };
        },
        templateUrl: "Rule/views/LogicalRule/QueryNodeTemplate.html",
    };
}]);
app.directive('returnSwitchCaseWhileNode', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Rule/views/LogicalRule/ReturnSwitchCaseWhileNodeTemplate.html",
    };
}]);

app.directive('logicalRuleEditable', [function () {
    return {
        restrict: "A",
        link: function (scope, element, attrs) {
            element.dblclick(function (event) {
                if ($(event.target).attr("contenteditable") !== "true") {
                    enableEditing(this);
                }
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            });
            element.on("keyup", function (event) {
                if (event.which == 27) {
                    disableEditing(this);
                }
            });
            element.on("blur", function () {
                disableEditing(this);
            });
        },
    };
}]);
function setSelectionByCharacterOffsets(containerEl, start, end) {
    if (window.getSelection && document.createRange) {
        var charIndex = 0, range = document.createRange();
        range.setStart(containerEl, 0);
        range.collapse(true);
        var nodeStack = [containerEl], node, foundStart = false, stop = false;

        while (!stop && (node = nodeStack.pop())) {
            if (node.nodeType == 3) {
                var nextCharIndex = charIndex + node.length;
                if (!foundStart && start >= charIndex && start <= nextCharIndex) {
                    range.setStart(node, start - charIndex);
                    foundStart = true;
                }
                if (foundStart && end >= charIndex && end <= nextCharIndex) {
                    range.setEnd(node, end - charIndex);
                    stop = true;
                }
                charIndex = nextCharIndex;
            }
            else {
                var i = node.childNodes.length;
                while (i--) {
                    nodeStack.push(node.childNodes[i]);
                }
            }
        }

        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);

    }
    else if (document.selection) {
        var textRange = document.body.createTextRange();
        textRange.moveToElementText(containerEl);
        textRange.collapse(true);
        textRange.moveEnd("character", end);
        textRange.moveStart("character", start);
        textRange.select();
    }
}
function disableEditing(element) {
    $(element).parents(".node").first().attr("draggable", "true");
    $(element).attr("contenteditable", "false");

}
function enableEditing(element) {
    $(element).parents(".node").first().removeAttr("draggable");
    $(element).attr("contenteditable", "true");

    setSelectionByCharacterOffsets(element, $(element).text().length, $(element).text().length);

    $(element).focus();
}
function enableExpressionEditing(sfwNodeId, index) {
    if (sfwNodeId) {
        if (!index) {
            index = 0;
        }

        var editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-block[logical-rule-editable]");
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-container > .node-expression-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-container > .node-expression-inline[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            var editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-container > .node-header-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-container > .node-header-inline[logical-rule-editable]");
        }

        if (editableBlock.length > index) {
            enableEditing(editableBlock[index]);
        }
    }
}

app.directive('logicalRuleDragDrop', ["$timeout", function ($timeout) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            if ($(element).hasClass("logical-rule-container")) {
                var onMouseUpHandler = function (event) {
                    removeDropArea();
                    dragDropData = null;
                }
                $(element).on("mouseup drop", onMouseUpHandler);
            }
            scope.canDrop = function (mouseOver) {
                var retValue = false;
                if (dragDropData && (mouseOver.isOverTop || mouseOver.isOverLeft || mouseOver.isOverRight || mouseOver.isOverBottom || mouseOver.isOverInside)) {
                    retValue = true;
                    if (scope.model) {
                        var dragNodeId = null;
                        var dragNodeName = null;
                        if (dragDropData.length === 1) {
                            dragNodeId = dragDropData[0].dictAttributes.sfwNodeID;
                            dragNodeName = dragDropData[0].Name;
                        }
                        else if (dragDropData.length > 1) {
                            dragNodeId = dragDropData[1];
                            var dragNode = searchNodeByIdInCurrentRule(dragNodeId);
                            if (dragNode) {
                                dragNodeName = dragNode.Name;
                            }
                        }

                        //Don't allow to drop a node around itself. Also don't allow to drop a node on the node after  it.
                        if (dragNodeId === scope.model.dictAttributes.sfwNodeID || (dragNodeId === scope.parentModel.dictAttributes.sfwNodeID && mouseOver.isOverRight === false)) {
                            retValue = false;
                        }
                        //Don't allow to drop a foreach/while/switch node inside itself at any level.
                        else if ((dragNodeName === "foreach" || dragNodeName === "while" || dragNodeName === "switch") && $("div[node-id='" + dragNodeId + "']").find("> .node-wrapper > ul div[node-id='" + scope.model.dictAttributes.sfwNodeID + "']").length > 0) {
                            retValue = false;
                        }

                        //Don't allow to drop a node after a collapsed node.
                        else if ($("div[node-id='" + scope.model.dictAttributes.sfwNodeID + "'] > .node-expander").hasClass("node-collapsed") && mouseOver.isOverRight) {
                            retValue = false;
                        }

                        else if ((scope.model.Name === "case" || scope.model.Name === "default")) {

                            //Don't allow to drop a node to the left side of a case/default node.
                            if (mouseOver.isOverLeft) {
                                retValue = false;
                            }
                            //Don't allow to drop a node to the bottom of a default node.
                            else if (scope.model.Name === "default" && mouseOver.isOverBottom) {
                                retValue = false;
                            }
                            //Don't allow to drop a case/default node to the right of another case/default node.
                            else if ((dragNodeName === "case" || dragNodeName === "default") && mouseOver.isOverRight) {
                                retValue = false;
                            }
                            //Don't allow to drop a node to the top/bottom of a case/default node, if dragged node is not a case/default.
                            else if ((mouseOver.isOverTop || mouseOver.isOverBottom) && (dragNodeName !== "case" && dragNodeName !== "default")) {
                                retValue = false;
                            }
                            //Don't allow to drop a default node to a switch block if it already have a default node.
                            else if (dragNodeName === "default" && scope.parentModel.Elements.filter(function (item) { return item.Name === "default"; }).length > 0) {
                                retValue = false;
                            }
                            //Don't allow to drop a default node to the top of a case node.
                            else if (dragNodeName === "default" && scope.model.Name === "case" && mouseOver.isOverTop) {
                                retValue = false;
                            }
                        }
                        else {

                            //Don't allow to drop a case/default node around any other node.
                            if (dragNodeName === "case" || dragNodeName === "default") {
                                retValue = false;
                            }
                        }
                    }
                }
                return retValue;
            };
            var setInsideDropArea = function (event) {
                var originalEvent = event.clientX ? event : event.originalEvent;
                var bounds = event.currentTarget.getBoundingClientRect();
                var mouseOver = { isOverInside: (originalEvent.clientX > bounds.left && originalEvent.clientX < bounds.right && originalEvent.clientY > bounds.top && originalEvent.clientY < bounds.bottom) };
                if (scope.canDrop(mouseOver)) {
                    var container = $(event.currentTarget).closest(".logical-rule-container");
                    setDropAreaBounds("inside", bounds, container);
                }
            }
            var onDragOverHandler = function (event) {
                //To allow drop over above calculated area.
                if (event.preventDefault) {
                    event.preventDefault();
                }

                if (event.currentTarget.localName == "ul" && $(event.currentTarget).hasClass("ul-children")) {
                    if ($(event.currentTarget).find("> li").length === 0) {
                        setInsideDropArea(event);
                    }
                }
                else if (event.currentTarget.localName === "div") {
                    if ($(event.currentTarget).hasClass("logical-rule-container")) {
                        if ($(event.currentTarget).find("> ul > li").length === 0) {
                            setInsideDropArea(event);
                        }
                    }
                    else {
                        var bounds = getBounds(event.currentTarget);
                        var originalEvent = event.clientX ? event : event.originalEvent;
                        //Calculate the area for drag over in different directions.
                        //for Top drop

                        var mouseOver = checkMousePosition(originalEvent, bounds);

                        //clearDragOverTimeout();
                        if (scope.canDrop(mouseOver)) {
                            var container = $(event.currentTarget).parents(".logical-rule-container");

                            //setDragOverTimeout(function () {

                            removeDropArea();
                            //for Top drop
                            if (mouseOver.isOverTop) {
                                setDropAreaBounds("top", bounds, container);

                                var relatedLi = $(event.currentTarget).prev();
                                if (relatedLi && relatedLi.length > 0) {
                                    var relatedBounds = getBounds(relatedLi[0]);
                                    setDropAreaBounds("verticalfill", relatedBounds, container);
                                }
                            }
                            //for Left drop
                            else if (mouseOver.isOverLeft) {
                                setDropAreaBounds("left", bounds, container);
                            }
                            //for Right drop
                            else if (mouseOver.isOverRight) {
                                setDropAreaBounds("right", bounds, container);
                                setDropAreaBounds("horizontalfill", bounds, container);
                            }
                            //for Bottom drop
                            else if (mouseOver.isOverBottom) {
                                setDropAreaBounds("bottom", bounds, container);
                                setDropAreaBounds("verticalfill", bounds, container);
                            }
                            //});
                        }
                    }
                }
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            };
            $(element).on("dragover", onDragOverHandler);
            var getBounds = function (li) {
                var bounds = { liBounds: undefined, nodeBounds: undefined, elementsUlBounds: undefined, childrenUlBounds: undefined };
                bounds.liBounds = li.getBoundingClientRect();
                bounds.nodeBounds = $(li).find(" > .node-wrapper > .node")[0].getBoundingClientRect();

                //Required for right drop
                var elementsUlBounds;
                var elementsUl = $(li).find("> ul");
                if (elementsUl && elementsUl.length > 0) {
                    bounds.elementsUlBounds = elementsUl[0].getBoundingClientRect();
                }

                //Required for bottom drop
                var childrenUlBounds;
                var childrenUl = $(li).find(" > .node-wrapper > ul");
                if (childrenUl && childrenUl.length > 0) {
                    bounds.childrenUlBounds = childrenUl[0].getBoundingClientRect();
                }

                return bounds;
            };
            var setDropAreaBounds = function (direction, bounds, container) {
                var dropAreaBounds = { left: 0, right: 0, height: 0, width: 0 };
                switch (direction) {
                    case "top":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        dropAreaBounds.top = bounds.liBounds.top - 20;
                        dropAreaBounds.width = bounds.liBounds.width - 50;
                        dropAreaBounds.height = bounds.nodeBounds.top - bounds.liBounds.top + 20;
                        break;
                    case "left":
                        dropAreaBounds.left = bounds.liBounds.left;
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        dropAreaBounds.width = bounds.nodeBounds.left - bounds.liBounds.left;
                        dropAreaBounds.height = bounds.liBounds.height - 40;
                        break;
                    case "right":
                        if (bounds.elementsUlBounds) {
                            dropAreaBounds.left = bounds.elementsUlBounds.left - 30;
                        }
                        else {
                            dropAreaBounds.left = bounds.liBounds.right - 30;
                        }
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        dropAreaBounds.width = 30;
                        dropAreaBounds.height = bounds.liBounds.height - 40;
                        break;
                    case "bottom":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        dropAreaBounds.top = bounds.liBounds.bottom - 20;
                        dropAreaBounds.width = bounds.liBounds.width - 60;
                        dropAreaBounds.height = 40;
                        break;
                    case "horizontalfill":
                        dropAreaBounds.left = bounds.nodeBounds.right;
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        if (bounds.elementsUlBounds) {
                            dropAreaBounds.width = bounds.elementsUlBounds.left - 30 - dropAreaBounds.left;
                        }
                        else {
                            dropAreaBounds.width = bounds.liBounds.right - 30 - dropAreaBounds.left;
                        }
                        dropAreaBounds.height = bounds.nodeBounds.height;
                        break;
                    case "verticalfill":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        if (bounds.childrenUlBounds) {
                            dropAreaBounds.top = bounds.childrenUlBounds.bottom;
                            dropAreaBounds.width = bounds.childrenUlBounds.width + (bounds.childrenUlBounds.left - bounds.nodeBounds.left);
                        }
                        else {
                            dropAreaBounds.top = bounds.nodeBounds.bottom;
                            dropAreaBounds.width = bounds.nodeBounds.width;
                        }
                        dropAreaBounds.height = bounds.liBounds.bottom - 20 - dropAreaBounds.top;
                        break;
                    case "inside":
                        dropAreaBounds.left = bounds.left;
                        dropAreaBounds.top = bounds.top;
                        dropAreaBounds.width = bounds.width;
                        dropAreaBounds.height = bounds.height;
                        break;
                }

                //As we are adding drop area in the rule container only, so we will have to adjust the top and left as per container's 
                //offset and scrolloffset
                var containerBounds = $(container)[0].getBoundingClientRect();
                dropAreaBounds.left = dropAreaBounds.left - containerBounds.left + $(container).scrollLeft();
                dropAreaBounds.top = dropAreaBounds.top - containerBounds.top + $(container).scrollTop();

                addDropArea(direction, dropAreaBounds, container);
            };

            var addDropArea = function (direction, dropAreaBounds, container) {
                if (direction === "horizontalfill") {
                    direction = "right";
                }
                else if (direction === "verticalfill") {
                    direction = "bottom";
                }

                var dropAreaDiv = $('<div class="drop-area" ></div>');

                dropAreaDiv.css("left", dropAreaBounds.left);
                dropAreaDiv.css("top", dropAreaBounds.top);
                dropAreaDiv.css("height", dropAreaBounds.height);
                dropAreaDiv.css("width", dropAreaBounds.width);

                dropAreaDiv.on("dragover", function (event) {
                    if (event.preventDefault) {
                        event.preventDefault();
                    }
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                });
                dropAreaDiv.on("dragleave", function (event) {
                    //removeDropArea();
                });
                dropAreaDiv.on("drop", function (event) {
                    if (direction === "inside") {
                        if (attributes.isFirstNode && scope.rule) {
                            addItemOnDrop(dragDropData, [scope.rule.dictAttributes.sfwNodeID, direction], event);
                        }
                        else {
                            addItemOnDrop(dragDropData, [scope.model.dictAttributes.sfwNodeID, direction], event);
                        }
                    }
                    else {
                        addItemOnDrop(dragDropData, [scope.parentModel.dictAttributes.sfwNodeID, scope.model.dictAttributes.sfwNodeID, direction], event);
                    }
                    var dragData = dragDropData;
                    $timeout(function () {
                        if (dragData && dragData.length > 0 && dragData[0].dictAttributes)
                            enableExpressionEditing(dragData[0].dictAttributes.sfwNodeID);
                    });

                    removeDropArea();
                    dragDropData = null;
                });
                $(container).append(dropAreaDiv);
            };

            var removeDropArea = function () {
                $(".drop-area").remove();
            };
            var checkMousePosition = function (event, bounds) {
                var mouseOver = { isOverTop: false, isOverLeft: false, isOverRight: false, isOverBottom: false };
                mouseOver.isOverTop = (event.clientY > bounds.liBounds.top && event.clientY < bounds.nodeBounds.top && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.liBounds.right - 30);
                //for Left drop
                mouseOver.isOverLeft = (event.clientX > bounds.liBounds.left && event.clientX < bounds.nodeBounds.left && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20);

                //for Right drop
                var isOverHorizontalFillArea = ((bounds.elementsUlBounds && (event.clientX < bounds.elementsUlBounds.left - 30 && event.clientX > bounds.nodeBounds.right && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.nodeBounds.bottom)) || (!bounds.elementsUlBounds && (event.clientX < bounds.liBounds.right - 30 && event.clientX > bounds.nodeBounds.right && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.nodeBounds.bottom)));
                var isOverRightEdge = ((bounds.elementsUlBounds && (event.clientX < bounds.elementsUlBounds.left && event.clientX > bounds.elementsUlBounds.left - 30 && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20)) || (!bounds.elementsUlBounds && (event.clientX < bounds.liBounds.right && event.clientX > bounds.liBounds.right - 30 && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20)));
                mouseOver.isOverRight = isOverHorizontalFillArea || isOverRightEdge;

                //for Bottom drop
                var isOverVerticalFillArea = (bounds.childrenUlBounds && event.clientY < bounds.liBounds.bottom && event.clientY > bounds.childrenUlBounds.bottom && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.childrenUlBounds.right) || (!bounds.childrenUlBounds && event.clientY < bounds.liBounds.bottom && event.clientY > bounds.nodeBounds.bottom && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.nodeBounds.right);
                var isOverBottomEdge = (event.clientY < bounds.liBounds.bottom && event.clientY > bounds.liBounds.bottom - 20 && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.liBounds.right - 30);
                mouseOver.isOverBottom = isOverVerticalFillArea || isOverBottomEdge;

                return mouseOver;
            };
        }
    };
}]);

app.directive('customdraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
            isruletoolbar: '=',
            isobjectbased: '=',
            displaypath: '='
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
                    if (scope.dragdata instanceof Array) {
                        if (canDrag(scope.dragdata)) {
                            dragDropData = scope.dragdata;
                        }
                    } else {
                        dragDropData = scope.dragdata;
                        if (scope.isruletoolbar) {
                            dragDropData = [getNewObject(dragDropData)];
                        } else if (scope.isobjectbased) {
                            dragDropDataObject = scope.dragdata;
                            dragDropData = scope.dragdata.ShortName;
                            if (scope.displaypath) {
                                dragDropData = scope.displaypath + "." + scope.dragdata.ShortName;
                            }
                        }
                    }
                }
            }
        },
    };
}]);

app.directive('customdroppable', ["$rootScope", "$timeout", function ($rootScope, $timeout) {
    return {
        restrict: "A",
        scope: {
            dropdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.addEventListener('dragover', handleDragOver, false);
            el.addEventListener('drop', handleDrop, false);

            function handleDragOver(e) {

                if (canDrop(dragDropData, scope.dropdata)) {
                    if (e.preventDefault) {
                        e.preventDefault(); // Necessary. Allows us to drop.
                    }

                    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                    e.dataTransfer.setData("Text", "");
                    e.preventDefault();
                }
                return false;
            }

            function handleDrop(e) {
                if (e.target.localName == "span" && e.target.contentEditable == "true") {
                    return;
                }
                // Stops some browsers from redirecting.
                //if (e.stopPropagation) e.stopPropagation();
                var data = dragDropData;
                if (data != undefined && data != null && data != "") {
                    addItemOnDrop(data, scope.dropdata, e);
                }

                dragDropData = null;
            }
        }
    };
}]);

function ExpandCollapse(obj) {
    $(obj).parent().siblings().toggle();
    $(obj).siblings("ul").toggle();

    $(obj).toggleClass("logicalrule-expanded logicalrule-collapsed");

}

function searchNodeByIdInCurrentRule(nodeId) {
    var curScope = getCurrentFileScope();
    return searchByNodeId(curScope.objLogicalRule, nodeId);
}

function searchByNodeId(parent, nodeId) {
    var retValue = null;
    if (parent.dictAttributes.sfwNodeID == nodeId) {
        return parent;
    }
    else {
        if (parent.Name == "neorule" || parent.Name == "logicalrule" || parent.Name == "switch" || parent.Name == "foreach" || parent.Name == "while" || parent.Name == "case" || parent.Name == "default") {
            if (parent.Elements.length > 0) {
                for (var index = 0; index < parent.Elements.length; index++) {
                    retValue = searchByNodeId(parent.Elements[index], nodeId);
                    if (retValue != null) {
                        return retValue;
                    }
                }
            }
        }
        if (parent.Name != "parameters" && parent.Name != "ExtraFields" && parent.Name != "variables" && parent.Name != "neorule" && parent.Name != "logicalrule" && parent.Name != "case" && parent.Name != "default" && retValue == null) {
            if (parent.Children.length > 0) {
                for (var index = 0; index < parent.Children.length; index++) {
                    retValue = searchByNodeId(parent.Children[index], nodeId);
                    if (retValue != null) {
                        return retValue;
                    }
                }
            }
        }
    }
    return retValue;
}

function addItemOnDrop(dragdata, dropdata, event) {
    var curScope = getCurrentFileScope();
    if (dragdata == dropdata) {
        return;
    }

    var dragParent = null;
    var dragChild = null;

    if (dragdata.length == 2) {
        dragParent = searchByNodeId(curScope.objLogicalRule, dragdata[0]);
        dragChild = searchByNodeId(curScope.objLogicalRule, dragdata[1]);
    }
    else if (dragdata.length == 1) {
        dragChild = searchByNodeId(curScope.objLogicalRule, dragdata[0]);
        if (dragChild == null) {
            dragChild = dragdata[0];
            if (curScope.objSelectedLogicalRule) {
                curScope.ResetAllvalues(curScope.objSelectedLogicalRule);
            }
            dragChild.IsSelected = true;
        }
    }

    var dropParent = null;
    var dropChild = null;
    var dropDirection = null;

    if (dropdata.length == 3) {
        dropParent = searchByNodeId(curScope.objLogicalRule, dropdata[0]);
        dropChild = searchByNodeId(curScope.objLogicalRule, dropdata[1]);
        dropDirection = dropdata[2];
    }
    else if (dropdata.length == 2) {
        dropChild = searchByNodeId(curScope.objLogicalRule, dropdata[0]);
        dropDirection = dropdata[1];
    }

    curScope.$evalAsync(function () {
        curScope.$root.UndRedoBulkOp("Start");

        if (dropDirection == "top") {
            var dropIndex;
            if (dropParent.Children && dropParent.Children.length > 0) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }
                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild));
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {

                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }
                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild));
                }
            }
        }
        else if (dropDirection == "bottom") {
            var dropIndex;

            if (dropParent.Children != undefined) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }
                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild) + 1);
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {
                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }
                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild) + 1);
                }
            }
        }
        else if (dropDirection == "left") {
            var dropIndex;
            if (dropParent.Children != undefined) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }

                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild));
                curScope.$root.DeleteItem(dropChild, dropParent.Children);
                curScope.$root.PushItem(dropChild, dragChild.Children);
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {
                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }


                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild));
                    curScope.$root.DeleteItem(dropChild, dropParent.Elements);
                    curScope.$root.PushItem(dropChild, dragChild.Children);
                }
            }
        }
        else if (dropDirection == "right") {
            if (dragParent != null) {
                removeDragItemFromExistingParent(curScope, dragChild, dragParent);
            }
            if (dropChild.Name == "case" || dropChild.Name == "default") {

                //insert dropChild.Elements to dragChild.Children from top;
                for (var index = dropChild.Elements.length - 1; index >= 0; index--) {
                    var item = dropChild.Elements[index];
                    curScope.$root.DeleteItem(item, dropChild.Elements);
                    curScope.$root.InsertItem(item, dragChild.Children, 0);
                }

                //add dragChild to dropChild.Elements
                curScope.$root.PushItem(dragChild, dropChild.Elements);
            }
            else {

                //insert dropChild.Children to dragChild.Children from top;
                for (var index = dropChild.Children.length - 1; index >= 0; index--) {
                    var item = dropChild.Children[index];
                    curScope.$root.DeleteItem(item, dropChild.Children);
                    curScope.$root.InsertItem(item, dragChild.Children, 0);
                }

                //add dragChild to dropChild.Children
                curScope.$root.PushItem(dragChild, dropChild.Children);
            }
        }
        else if (dropDirection == "inside") {
            if (dragParent != null) {
                removeDragItemFromExistingParent(curScope, dragChild, dragParent);
            }
            curScope.$root.InsertItem(dragChild, dropChild.Elements, 0);
        }
        curScope.$root.UndRedoBulkOp("End");
    });
}

function removeDragItemFromExistingParent(curScope, dragChild, dragParent) {
    var dragParentCollection = getNodeCollection(dragParent, dragChild);
    var dragChildIndex = dragParentCollection.indexOf(dragChild);
    if (dragChildIndex > -1) {
        var dragChildCollection = getNodeCollection(dragChild);
        curScope.$root.DeleteItem(dragChild, dragParentCollection);

        if (dragChild.Name != "case" && dragChild.Name != "default") {
            for (var index = dragChildCollection.length - 1; index >= 0; index--) {
                var item = dragChildCollection[index];
                curScope.$root.DeleteItem(item, dragChildCollection);
                curScope.$root.InsertItem(item, dragParentCollection, dragChildIndex);
            }
        }
    }
}
function getNodeCollection(parentNode, childNode) {
    var retCollection = null;
    if (childNode) {
        if (parentNode.Children && parentNode.Children.indexOf(childNode) > -1) {
            retCollection = parentNode.Children;
        }
        else if (parentNode.Elements && parentNode.Elements.indexOf(childNode) > -1) {
            retCollection = parentNode.Elements;
        }
    }
    else {
        retCollection = parentNode.Name === "case" || parentNode.Name === "default" ? parentNode.Elements : parentNode.Children;
    }
    return retCollection;
}

var dragDropData = null;
var dragDropDataObject = null;
function GetLevel(element, type) {
    var outerClass = "";
    var innerClass = "";
    if (type == "foreach") {
        outerClass = "ul-foreach-outer";
        innerClass = "ul-foreach-inner";
    }
    else if (type == "while") {
        outerClass = "ul-while-outer";
        innerClass = "ul-while-inner";
    }
    else if (type == "condition") {
        outerClass = "ul-condition-outer";
        innerClass = "ul-condition-inner";
    }

    var parents = $(element).parents("." + outerClass + ",." + innerClass);
    if (parents.length > 0) {
        if ($(parents[0]).attr("class").contains(outerClass)) {
            return true;
        }
        else {
            return false;
        }
    }
}

String.prototype.contains = function (str) {
    return this.indexOf(str) > -1;
};


function expandCollapseParameters(e_args) {
    var p = $(e_args.target);
    var ele = p.parents("tr").siblings();
    var table = $(ele[ele.length - 1]).find("table");

    if (p != null) {
        if (p.hasClass("callactivity-arrow-right")) {

            p.removeClass('callactivity-arrow-right');
            p.addClass('callactivity-arrow-down');

            if (table != null) {
                table.removeAttr('style');
            }
        }
        else {

            p.removeClass('callactivity-arrow-down');
            p.addClass('callactivity-arrow-right');

            if (table != null) {
                table.attr('style', 'display:none');
            }
        }
    }
}

function ToggleAllOutLining(element) {
    var expandedNodes = $(element).find(".node-expanded");
    var collapsedNodes = $(element).find(".node-collapsed");

    if (collapsedNodes.length > 0) {
        collapsedNodes.each(function (index, childElement) {
            expandNode(childElement, true);
        });
    }
    else {
        expandedNodes.each(function (index, childElement) {
            collapseNode(childElement, true);
        });
    }
}

function canDrag(dragdata) {
    var retValue = true;
    var rootScope = getCurrentFileScope();
    if (dragdata != undefined && dragdata != null) {
        var dragParent = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
        var dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[1]);


        if (dragParent != null && dragChild != null && dragParent.Name == "switch" && dragChild.Name == "case") {
            var cases = dragParent.Elements.filter(function (x) {
                return x.Name == "case";
            });
            if (cases.length == 1 && cases[0] == dragChild) {
                retValue = false;
            }
        }
    }

    return retValue;
}
function getNewObject(nodeName) {
    var obj = {
        Name: nodeName, Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: []
    };

    if (nodeName == "actions") {
        obj.Elements.push({
            Name: "action", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: []
        });
    }
    else if (nodeName == "switch") {
        obj.Elements.push({
            Name: "case", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: []
        });
        obj.Elements.push({
            Name: "default", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: []
        });
    }

    return obj;
}

function canDrop(dragdata, dropdata) {
    var rootScope = getCurrentFileScope();
    var retValue = true;
    if (dragdata == undefined || dragdata == null || dropdata == undefined || dropdata == null) {
        retValue = false;
    }
    else {
        var dragParent = null;
        var dragChild = null;

        if (dragdata.length == 2) {
            dragParent = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
            dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[1]);
        }
        else if (dragdata.length == 1) {
            dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
            if (dragChild == null) {
                dragChild = dragdata[0];
            }
        }

        var dropParent = null;
        var dropChild = null;
        var dropDirection = null;

        if (dropdata.length == 3) {
            dropParent = searchByNodeId(rootScope.objLogicalRule, dropdata[0]);
            dropChild = searchByNodeId(rootScope.objLogicalRule, dropdata[1]);
            dropDirection = dropdata[2];
        }
        else if (dropdata.length == 2) {
            dropChild = searchByNodeId(rootScope.objLogicalRule, dropdata[0]);
            dropDirection = dropdata[1];
        }

        //If an item is dragged and dropped on itself, don't allow.
        if (dragChild == dropChild) {
            return false;
        }

        //If an item is dragged and dropped on any of it's descendents, don't allow.
        var descendents = getDescendents(dragChild);
        if (descendents.indexOf(dropChild) > -1) {
            return false;
        }

        //If item dropped on logical rule canvas area, allow on if canvas is empty.
        if (dropChild.Name == "logicalrule") {
            var elements = dropChild.Elements.filter(function (x) { return x.Name != "variables"; });
            if (elements.length > 0) {
                return false;
            }
        }
        else {

            //if (dropDirection == "top" && (dropParent == null || dropParent.Name != "logicalrule")) {
            //    return false;
            //}

            // Don't allow an item to be dropped above/below a case/default if it is not case/default.
            if ((dropChild.Name == "case" || dropChild.Name == "default") && dropDirection != "right" && !(dragChild.Name == "case" || dragChild.Name == "default")) {
                return false;
            }

            //Don't allow an item to be dropped on switch if it is not case.
            if (dropChild.Name == "switch" && dropDirection == "inside" && dragChild.Name != "case") {
                return false;
            }

            if (dragChild.Name == "case" || dragChild.Name == "default") {

                if (dropParent != null) {

                    //Don't allow to drop a case/default it it is dropped to the right side of an existing case/default or around anyother item inside switch.
                    if (dropParent.Name != "switch" || dropDirection == "right") {
                        return false;
                    }

                    //Don't allow, If a case is dragged and dropped after a default.
                    if (dragChild.Name == "case" && dropChild.Name == "default") {
                        return false;
                    }

                    //Don't allow, if a default is dragged and dropped inside a switch block where already a default exists.
                    if (dragChild.Name == "default") {

                        var defaults = dropParent.Elements.filter(function (x) { return x.Name == "default"; });
                        if (defaults.length > 0) {
                            return false;
                        }
                        else {
                            //Don't allow, if a default is dragged and dropped before a case, it should always be dropped after last case.
                            var cases = dropParent.Elements.filter(function (x) { return x.Name == "case"; });
                            if (cases.indexOf(dropChild) < cases.length - 1) {
                                return false;
                            }
                        }
                    }
                }
                else {
                    //Dont allow to drag and drop a default into switch at top position.
                    if (dropChild.Name == "switch") {
                        if (dragChild.Name == "default") {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
        }
    }

    return retValue;
}


app.directive("rowconditionitemdirective", ["$compile", "ngDialog", "$rootScope", function ($compile, ngDialog, $rootScope) {
    var changedataheaderid = function (elements, items) {
        if (elements.length > 0) {
            var i = 0;
            for (i = 0; i < elements.length; i++) {
                if (elements[i].Item != undefined) {
                    if (elements[i].Item.ItemType == "assignheader") {
                        if (elements[i].Item.DataHeaderID == items.Item.DataHeaderID) {
                            elements[i].Item.Expression = items.Item.Expression;
                        }
                    }
                }
                if (elements[i].Cells != undefined) {
                    changedataheaderid(elements[i].Cells, items);
                }
            }
        }
    };
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '=',
            rciviewmode: '='
        },
        templateUrl: "Rule/views/DecisionTable/rowconditionitem.html",
        link: function (scope, element, attrs) {
            scope.init = function () {
                scope.conditionType = "default";
                if (scope.items.Item.ItemType == 'notesheader' || scope.items.Item.ItemType == 'returnheader') {
                    scope.conditionType = "1";
                }
                else if ((scope.items.Item.ItemType == 'colheader' || scope.items.Item.ItemType == 'rowheader')) {
                    scope.conditionType = "2";
                }
                else if (scope.items.Item.ItemType == 'assignheader') {
                    scope.conditionType = "3";
                }
                else if (scope.items.Item.ItemType != 'notesheader' && scope.items.Item.ItemType != 'returnheader' && scope.items.Item.ItemType != 'colheader' && scope.items.Item.ItemType != 'rowheader' && scope.items.Item.ItemType != 'assignheader' && scope.items.Item.ItemType != 'notes') {
                    scope.conditionType = "4";
                }
            };
            scope.getclass = function (obj) {
                var classname = "";
                if (obj != undefined) {
                    if (obj.Item.IsSelected) {
                        classname = "decision-table-selection";
                    }
                    else if (obj.Item.ItemType == "if") {
                        classname = "dt-if";
                    }
                    else if (obj.Item.ItemType == "colheader" || obj.Item.ItemType == "rowheader") {
                        classname = "dt-row-col-header";
                    }
                    else if (obj.Item.ItemType == "assignheader" || obj.Item.ItemType == 'notesheader' || obj.Item.ItemType == 'returnheader') {
                        classname = "dt-assign-header";
                    }
                    else {
                        classname = "dt-assign";
                    }
                    if (obj.Item.isAdvanceSearched) {
                        classname += ' bckgGrey';
                    }
                    if (obj.Item.IsSelected && obj.Item.isAdvanceSearched) {
                        classname += ' bckgGreen';
                    }
                }

                if (scope.rciviewmode == 'Developer View') {
                    classname += ' inner-div';
                }
                else {
                    classname += ' inner-div-analystview';
                }
                return classname;
            };
            scope.columnnamechange = function () {
                changedataheaderid(scope.logicalrule.Rows, scope.items);
            };
            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    controllerScope.onActionKeyDown(eargs);
                }
            };
            scope.notesdailog = function (notesobj) {
                if (notesobj.Item.ItemType == "notes") {
                    var newscope = scope.$new();
                    var data = JSON.stringify(notesobj.Item);
                    var objnotes = JSON.parse(data);
                    objnotes.Value = notesobj.Item.NotesValue;
                    newscope.objText = objnotes;
                    newscope.dialog = $rootScope.showDialog(newscope, "Edit Notes", "Rule/views/EditNotesTemplate.html", { width: 400, height: 400 });
                    newscope.SaveChangedTextData = function () {
                        $rootScope.EditPropertyValue(notesobj.Item.NotesValue, notesobj.Item, "NotesValue", newscope.objText.Value);
                        newscope.onCancelClick();
                    };
                    newscope.onCancelClick = function () {
                        if (newscope.dialog) {
                            newscope.dialog.close();
                        }
                    };
                }
            };
            scope.AddContentEditable = function (event) {
                if ($(event.currentTarget).attr('content-editable') == undefined) {
                    var currentElement = $(event.currentTarget);
                    var caretPos = $(event.currentTarget).text().length;
                    var conditionType = currentElement.attr('data-condition-type');
                    // for description we need to compile the current element 
                    var elementTocompile = currentElement;
                    currentElement.attr('ng-model', $(event.currentTarget).attr('bindpropertyname'));
                    currentElement.attr('content-editable', '');
                    if (scope.items.Item.ItemType == "assignheader") {
                        currentElement.attr('ng-change', 'columnnamechange()');
                    }
                    if (conditionType == "Expression") {
                        elementTocompile = currentElement.closest("[expression-wrapper]");
                    }
                    $compile(elementTocompile)(scope);
                    setTimeout(function () {
                        var htmlElement = $("#" + $rootScope.currentopenfile.file.FileName).find("#" + scope.items.Item.NodeID + " span[data-condition-type= '" + conditionType + "']");
                        if (htmlElement && htmlElement.length > 0) {
                            htmlElement.first().focus();
                            setSelectionByCharacterOffsets(htmlElement[0], caretPos, caretPos);
                        }
                    }, 4);
                }
            };
            scope.init();
        }
    };
}]);

app.directive("rowconditiondirective", ["$compile", "$rootScope", function ($compile, $rootScope) {

    var AddItem = function (obj, param, itemscope, logicalrule) {
        var dataheadernodeid = generateUUID();
        $rootScope.UndRedoBulkOp("Start");
        var dataheader = { DataHeaderID: '', Description: '', Expression: '', ItemType: 'dataheader', NodeID: dataheadernodeid };
        for (var i = 0; i < logicalrule.DataHeaders.length; i++) {
            if (logicalrule.DataHeaders[i].NodeID == itemscope.objChild.Item.DataHeaderID) {
                $rootScope.InsertItem(dataheader, logicalrule.DataHeaders, i + 1);
                //logicalrule.DataHeaders.splice(i + 1, 0, dataheader);
                break;
            }
        }

        var indexofobj = logicalrule.Rows.indexOf(obj);
        for (var j = indexofobj; j < logicalrule.Rows.length; j++) {
            for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                if (logicalrule.Rows[j].Cells[k].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                    if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'assignheader' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'notesheader' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'returnheader') {
                        if (param == 'actions') {
                            itemName = 'assignheader';
                        }

                        else if (param == 'notes') {
                            itemName = 'notesheader';
                        }

                        else if (param == 'return') {
                            itemName = 'returnheader';
                        }
                        var cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID() } };
                    }
                    else if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'assign' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'notes' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'return') {
                        if (param == 'actions') {
                            itemName = 'assign';
                        }

                        else if (param == 'notes') {
                            itemName = 'notes';
                        }

                        else if (param == 'return') {
                            itemName = 'return';
                        }
                        var cell;
                        if (param == "notes") {
                            cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID(), NotesValue: '' } };
                        }
                        else {
                            cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID() } };
                        }
                    }
                    $rootScope.InsertItem(cell, logicalrule.Rows[j].Cells, k + 1);
                }
            }
        }

        // setting colspan to 'if' conditions
        if (indexofobj > 0) {
            for (var i = 0; i < obj.Cells.length; i++) {
                if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                    for (var j = indexofobj - 1; j >= 0; j--) {
                        var index = 0;
                        for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                                index = logicalrule.Rows[j].Cells[k].Colspan;
                            }
                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                                var totalcolspan = index + logicalrule.Rows[j].Cells[k].Colspan;
                                if (totalcolspan > i && index <= i) {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan + 1);
                                    break;
                                }
                                else {
                                    index = index + logicalrule.Rows[j].Cells[k].Colspan;
                                }
                            }
                        }
                    }
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteItem = function (obj, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        var count = 0;
        var countofif = 0;
        for (var k = 0; k < obj.Cells.length; k++) {
            if ((obj.Cells[k].Item.ItemType == 'assignheader') || (obj.Cells[k].Item.ItemType == 'notesheader') || (obj.Cells[k].Item.ItemType == 'returnheader')) {
                count++;
            }
        }
        if (indexofobj > 0) {
            for (var l = 0; l < logicalrule.Rows[indexofobj - 1].Cells.length; l++) {
                if (logicalrule.Rows[indexofobj - 1].Cells[l].Item.ItemType == 'if') {
                    countofif++;
                }
            }
        }

        isvalid = true;
        isreturnpresent = false;
        if (itemscope.objChild.Item.ItemType == "assignheader" && indexofobj > -1) {
            var headerid = itemscope.objChild.Item.DataHeaderID;
            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "returnheader") {
                    isreturnpresent = true;
                    break;
                }
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "assignheader") {
                    if (logicalrule.Rows[indexofobj].Cells[i].Item.DataHeaderID == headerid) {
                        isvalid = false;
                    } else {
                        isvalid = true;
                        break;
                    }
                }
            }

            if (isreturnpresent) {
                isvalid = true;
            }
        }

        if (itemscope.objChild.Item.ItemType == "returnheader" && indexofobj > -1) {
            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "assignheader") {
                    isvalid = true;
                    break;
                } else if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "returnheader" && logicalrule.Rows[indexofobj].Cells[i].Item.DataHeaderID != itemscope.objChild.Item.DataHeaderID) {
                    isvalid = true;
                    break;
                }
                else {
                    isvalid = false;
                }
            }
        }

        if (count < 2 || count == countofif || !isvalid) {
            alert("Atleast one action column should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = 0; i < logicalrule.DataHeaders.length; i++) {
                if (logicalrule.DataHeaders[i].NodeID == itemscope.objChild.Item.DataHeaderID) {
                    $rootScope.DeleteItem(logicalrule.DataHeaders[i], logicalrule.DataHeaders);
                    //logicalrule.DataHeaders.splice(i, 1);
                    break;
                }
            }
            var indexofobj = logicalrule.Rows.indexOf(obj);
            // setting colspan to 'if' conditions
            if (indexofobj > 0) {
                for (var i = 0; i < obj.Cells.length; i++) {
                    if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        for (var j = indexofobj - 1; j >= 0; j--) {
                            var index = 0;
                            for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                                if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                                    index = logicalrule.Rows[j].Cells[k].Colspan;
                                }
                                if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                                    var totalcolspan = index + logicalrule.Rows[j].Cells[k].Colspan;
                                    if (totalcolspan > i && index <= i) {
                                        $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan - 1);
                                        break;
                                    }
                                    else {
                                        index = index + logicalrule.Rows[j].Cells[k].Colspan;
                                    }
                                }
                            }
                        }
                        $rootScope.DeleteItem(obj.Cells[i], obj.Cells);
                        //obj.Cells.splice(i, 1);
                    }

                }
            }
            else {
                for (var i = 0; i < obj.Cells.length; i++) {
                    if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        $rootScope.DeleteItem(obj.Cells[i], obj.Cells);
                        //obj.Cells.splice(i, 1);
                    }
                }
            }

            // Deleting the condition

            for (var j = indexofobj + 1; j < logicalrule.Rows.length; j++) {
                for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[k].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        $rootScope.DeleteItem(logicalrule.Rows[j].Cells[k], logicalrule.Rows[j].Cells);
                        //logicalrule.Rows[j].Cells.splice(k, 1);
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var AddConditionInLeftOrRight = function (obj, param, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        $rootScope.UndRedoBulkOp("Start");
        if (param == 'Right') {
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    var rowheaderid;
                    if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                        if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                            rowheaderid = generateUUID();
                            var description = logicalrule.Rows[i].Cells[j].Item.Description;
                            var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                            var cell = { Colspan: '1', Rowspan: '1', Item: { Description: description, Expression: expression, ItemType: 'rowheader', NodeID: rowheaderid } };
                            $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                            //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                        }
                    }
                    else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                        if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                            var rowspan = logicalrule.Rows[i].Cells[j].Rowspan;
                            var description = logicalrule.Rows[i].Cells[j].Item.Description;
                            var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                            var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: description, Expression: expression, ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                            $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                            //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                        }
                    }
                }
            }
        }
        else if (param == 'Left') {
            var indexofele = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
            if (indexofele == 0) {
                var rowheaderid = generateUUID();
                var rowspan = 0;
                var positionofsplicing;
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        var rowheaderid;
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                                var cell = { Colspan: '1', Rowspan: '1', Item: { Description: '', Expression: '', ItemType: 'rowheader', NodeID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j);
                                //logicalrule.Rows[i].Cells.splice(j, 0, cell);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                                rowspan += logicalrule.Rows[i].Cells[j].Rowspan;
                                if (positionofsplicing == undefined) {
                                    positionofsplicing = i;
                                }
                            }
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length > logicalrule.Rows[i].conditionitemcount) {
                        logicalrule.Rows[i].conditionitemcount += 30;
                    }
                }
                var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: '', Expression: '', ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                if (logicalrule.Rows[positionofsplicing] && logicalrule.Rows[positionofsplicing].Cells) {
                    $rootScope.InsertItem(cell, logicalrule.Rows[positionofsplicing].Cells, 0);
                    if (logicalrule.Rows[positionofsplicing].Cells.length > logicalrule.Rows[positionofsplicing].conditionitemcount) {
                        logicalrule.Rows[positionofsplicing].conditionitemcount += 30;
                    }
                }
                //logicalrule.Rows[positionofsplicing].Cells.splice(0, 0, cell);
            }
            else {
                var objofprevele = logicalrule.Rows[indexofobj].Cells[indexofele - 1];

                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        var rowheaderid;
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (objofprevele.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                                rowheaderid = generateUUID();
                                var description = logicalrule.Rows[i].Cells[j].Item.Description;
                                var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                                var cell = { Colspan: '1', Rowspan: '1', Item: { Description: description, Expression: expression, ItemType: 'rowheader', NodeID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                                //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (objofprevele.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                                var rowspan = logicalrule.Rows[i].Cells[j].Rowspan;
                                var description = logicalrule.Rows[i].Cells[j].Item.Description;
                                var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                                var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: description, Expression: expression, ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                                //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                                break;
                            }
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length > logicalrule.Rows[i].conditionitemcount) {
                        logicalrule.Rows[i].conditionitemcount += 30;
                    }
                }
            }
        }

        // setting colspan to colheader

        for (var i = 0; i < logicalrule.Rows.length; i++) {
            for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                    $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", logicalrule.Rows[i].Cells[j].Colspan + 1);
                    break;
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var AddConditionInTopOrBottom = function (obj, param, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        $rootScope.UndRedoBulkOp("Start");
        if (param == 'Top') {
            if (indexofobj > 0) {
                var cells = { Cells: [] };
                for (var i = 0; i < logicalrule.Rows[indexofobj - 1].Cells.length; i++) {
                    colspan = logicalrule.Rows[indexofobj - 1].Cells[i].Colspan;
                    rowspan = logicalrule.Rows[indexofobj - 1].Cells[i].Rowspan;
                    itemName = logicalrule.Rows[indexofobj - 1].Cells[i].Item.ItemType;
                    descriprion = logicalrule.Rows[indexofobj - 1].Cells[i].Item.Description;
                    expression = logicalrule.Rows[indexofobj - 1].Cells[i].Item.Expression;
                    var colheaderid;
                    if (itemName == 'colheader') {
                        colheaderid = generateUUID();
                        cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: colheaderid } };
                    }
                    else {
                        cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: generateUUID(), HeaderID: colheaderid } };
                    }
                    cells.Cells.splice(i, 0, cell);
                }
                $rootScope.InsertItem(cells, logicalrule.Rows, indexofobj);
                //logicalrule.Rows.splice(indexofobj, 0, cells);
            }
            else {
                var cells = { Cells: [] };
                var colheaderid = generateUUID();
                var count = 0;
                var length;
                for (var i = 0; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            length = logicalrule.Rows[i].Cells.length;
                            count++;
                        }
                    }
                }
                if (logicalrule.Rows[indexofobj].Cells[0].Item.ItemType == "colheader") {
                    descriprion = logicalrule.Rows[indexofobj].Cells[0].Item.Description;
                    expression = logicalrule.Rows[indexofobj].Cells[0].Item.Expression;
                }
                else {
                    descriprion = '';
                    expression = '';
                }
                cell = { Colspan: count, Rowspan: 1, Item: { Description: descriprion, Expression: expression, ItemType: 'colheader', NodeID: colheaderid } };
                cells.Cells.push(cell);

                cell = { Colspan: length - count, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'if', NodeID: generateUUID(), HeaderID: colheaderid } };
                cells.Cells.push(cell);
                $rootScope.InsertItem(cells, logicalrule.Rows, 0);
                //logicalrule.Rows.splice(0, 0, cells);
            }
        }

        else if (param == 'Bottom') {
            var cells = { Cells: [] };

            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                colspan = logicalrule.Rows[indexofobj].Cells[i].Colspan;
                rowspan = logicalrule.Rows[indexofobj].Cells[i].Rowspan;
                itemName = logicalrule.Rows[indexofobj].Cells[i].Item.ItemType;
                descriprion = logicalrule.Rows[indexofobj].Cells[i].Item.Description;
                expression = logicalrule.Rows[indexofobj].Cells[i].Item.Expression;
                var colheaderid;
                if (itemName == 'colheader') {
                    colheaderid = generateUUID();
                    cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: colheaderid } };
                }
                else {
                    cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: generateUUID(), HeaderID: colheaderid } };
                }

                cells.Cells.splice(i, 0, cell);
            }
            $rootScope.InsertItem(cells, logicalrule.Rows, indexofobj + 1);
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteCondition = function (obj, itemscope, logicalrule) {
        $rootScope.UndRedoBulkOp("Start");
        var indexofobj = logicalrule.Rows.indexOf(obj);
        if (itemscope.objChild.Item.ItemType == 'colheader') {
            if (logicalrule.Rows[indexofobj + 1].Cells[0].Item.ItemType != 'rowheader') {
                $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                //logicalrule.Rows.splice(indexofobj, 1);
            }
            else {
                // getting 1st element data
                var objele;
                for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                    if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'notesheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'returnheader') {
                        objele = logicalrule.Rows[indexofobj + 1].Cells[j];
                        break;
                    }
                }

                if (indexofobj == 0 || (indexofobj > 0 && logicalrule.Rows[indexofobj - 1].Cells.length == 2)) {
                    for (var i = indexofobj + 1; i < logicalrule.Rows.length; i++) {
                        count = 0;
                        for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {

                            if (logicalrule.Rows[i].Cells[j].Item.DataHeaderID == objele.Item.DataHeaderID) {
                                count++;
                            }
                            if (count > 1) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                j--;
                            }
                        }
                    }

                    if (indexofobj > 0) {
                        $rootScope.EditPropertyValue(logicalrule.Rows[indexofobj - 1].Cells[1].Colspan, logicalrule.Rows[indexofobj - 1].Cells[1], "Colspan", logicalrule.Rows[indexofobj + 1].Cells.length - logicalrule.Rows[indexofobj - 1].Cells[0].Colspan);
                    }
                    $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                    //logicalrule.Rows.splice(indexofobj, 1);
                }

                else {
                    var objElements;
                    var celllength;
                    var startindex;
                    var endindex;
                    for (var i = 0; i < logicalrule.Rows[indexofobj - 1].Cells.length - 1; i++) {
                        if (logicalrule.Rows[indexofobj - 1].Cells[i].Item.ItemType == 'colheader') {
                            startindex = logicalrule.Rows[indexofobj - 1].Cells[i].Colspan;
                        }
                        if (logicalrule.Rows[indexofobj - 1].Cells[i + 1].Item.ItemType == 'if') {
                            endindex = startindex + logicalrule.Rows[indexofobj - 1].Cells[i + 1].Colspan;
                        }

                        for (var j = 0; j < logicalrule.Rows[indexofobj].Cells.length - 1; j++) {
                            var startindexofcurr;
                            var endindexofcurr;
                            if (logicalrule.Rows[indexofobj].Cells[j].Item.ItemType == 'colheader') {
                                startindexofcurr = logicalrule.Rows[indexofobj].Cells[j].Colspan;
                            }
                            if (logicalrule.Rows[indexofobj].Cells[j + 1].Item.ItemType == 'if') {
                                endindexofcurr = startindexofcurr + logicalrule.Rows[indexofobj].Cells[j + 1].Colspan;
                            }

                            if (startindex == startindexofcurr) {
                                if (objElements == undefined) {
                                    objElements = [];
                                    for (var k = indexofobj + 1; k < logicalrule.Rows.length; k++) {
                                        var objcells = [];
                                        for (var l = 0; l < logicalrule.Rows[k].Cells.length; l++) {
                                            if (celllength == undefined) {
                                                celllength = logicalrule.Rows[k].Cells.length;
                                            }
                                            if (celllength == logicalrule.Rows[k].Cells.length) {
                                                if (endindexofcurr > l && startindexofcurr <= l) {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'rowheader') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                else if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'if') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                            else {
                                                var diff = celllength - logicalrule.Rows[k].Cells.length;

                                                if ((endindexofcurr - diff) > l && (startindexofcurr - diff) <= l) {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'rowheader') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                else if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'if') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                        }
                                        var cells = { Cells: objcells };
                                        objElements.push(cells);
                                    }
                                    startindexofcurr = endindexofcurr;
                                }
                                else {
                                    var m = 0;
                                    for (var k = indexofobj + 1; k < logicalrule.Rows.length; k++) {

                                        for (var l = 0; l < logicalrule.Rows[k].Cells.length; l++) {
                                            if (celllength == logicalrule.Rows[k].Cells.length) {
                                                if (endindexofcurr > l && startindexofcurr <= l) {
                                                    objElements[m].Cells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                            else {
                                                var diff = celllength - logicalrule.Rows[k].Cells.length;
                                                if ((endindexofcurr - diff) > l && (startindexofcurr - diff) <= l) {
                                                    objElements[m].Cells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                        }
                                        m++;
                                    }
                                    startindexofcurr = endindexofcurr;
                                }
                            }
                            else {
                                startindexofcurr = endindexofcurr;
                            }
                        }
                        startindex = endindex;
                    }

                    var p = 0;
                    for (var i = indexofobj + 1; i < logicalrule.Rows.length; i++) {
                        logicalrule.Rows[i] = objElements[p];
                        p++;
                    }
                    $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                    //logicalrule.Rows.splice(indexofobj, 1);

                    //setting column span
                    var objele;
                    for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                        if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'notesheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader') {
                            objele = logicalrule.Rows[indexofobj + 1].Cells[j];
                            break;
                        }
                    }
                    var span;
                    var elecount = 0;
                    for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                        if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.DataHeaderID == objele.Item.DataHeaderID) {

                            if (elecount > 0) {
                                span = j;
                                break;
                            }
                            else {
                                elecount++;
                            }
                        }
                    }
                    var colspan;
                    var totalcolspan = 0;
                    for (var k = 0; k < logicalrule.Rows[indexofobj - 1].Cells.length; k++) {

                        if (logicalrule.Rows[indexofobj - 1].Cells[k].Item.ItemType == 'colheader') {
                            colspan = span - logicalrule.Rows[indexofobj - 1].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[indexofobj - 1].Cells[k].Item.ItemType == 'if') {
                            $rootScope.EditPropertyValue(logicalrule.Rows[indexofobj - 1].Cells[k].Colspan, logicalrule.Rows[indexofobj - 1].Cells[k], "Colspan", colspan);
                            totalcolspan += colspan;
                        }
                    }

                    if ((indexofobj - 1) > 0) {
                        for (var i = indexofobj - 2; i >= 0; i--) {
                            for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                                var colspanofcell = totalcolspan / (logicalrule.Rows[i].Cells.length - 1);
                                if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", colspanofcell);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (itemscope.objChild.Item.ItemType == 'rowheader') {
            var count = 0;
            var ifcount = 0;
            var indexofitem = obj.Cells.indexOf(itemscope.objChild);
            var blntrue = false;
            if (obj.Cells[indexofitem + 1].Item.ItemType == "rowheader") {
                blntrue = true;
            }
            for (var k = 0; k < obj.Cells.length; k++) {
                if (itemscope.objChild.Item.ItemType == obj.Cells[k].Item.ItemType) {
                    count++;
                }
            }

            if (count < 2) {
                alert("Atleast one  Row condition should be present");
            }
            else {
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (logicalrule.Rows[i].Cells[j].Item.NodeID == itemscope.objChild.Item.NodeID) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (logicalrule.Rows[i].Cells[j].Item.HeaderID == itemscope.objChild.Item.NodeID) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                ifcount++;
                                break;
                            }
                        }
                    }
                }
                if (indexofitem > 0) {
                    var previtemnodeid = obj.Cells[indexofitem - 1].Item.NodeID;
                    var itemcount = 0;
                    for (var l = indexofobj + 1; l < logicalrule.Rows.length; l++) {
                        for (m = 0; m < logicalrule.Rows[l].Cells.length; m++) {
                            if (logicalrule.Rows[l].Cells[m].Item.HeaderID == previtemnodeid && logicalrule.Rows[l].Cells[m].Item.ItemType == 'if') {
                                itemcount++;
                                break;
                            }
                        }
                    }

                    var maxlength = 0;
                    var leastrowlength = 0;
                    for (var i = 0; i < logicalrule.Rows.length; i++) {
                        if (maxlength < logicalrule.Rows[i].Cells.length) {
                            maxlength = logicalrule.Rows[i].Cells.length;
                        }

                        if (leastrowlength == 0 || leastrowlength > logicalrule.Rows[i].Cells.length) {
                            leastrowlength = logicalrule.Rows[i].Cells.length;
                        }
                    }

                    if (!blntrue && itemcount != ifcount) {
                        for (var i = logicalrule.Rows.length - 1; i >= 0; i--) {
                            if (logicalrule.Rows[i].Cells.length == leastrowlength && logicalrule.Rows[i].Cells[0].Item.ItemType != 'rowheader' && logicalrule.Rows[i].Cells[0].Item.ItemType != 'colheader') {
                                var cellslength = leastrowlength;
                                var headerid = undefined;
                                for (var j = i - 1; j >= 0; j--) {
                                    if (logicalrule.Rows[j].Cells.length > cellslength && logicalrule.Rows[j].Cells[0].Item.ItemType == 'if') {
                                        for (k = 0; k < logicalrule.Rows[j].Cells.length; k++)
                                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if' && (headerid == undefined || logicalrule.Rows[j].Cells[k].Item.HeaderID != headerid)) {
                                                $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Rowspan, logicalrule.Rows[j].Cells[k], "Rowspan", logicalrule.Rows[j].Cells[k].Rowspan - 1);
                                            }
                                            else {
                                                cellslength = logicalrule.Rows[j].Cells.length;
                                                headerid = logicalrule.Rows[j].Cells[0].Item.HeaderID;
                                                break;
                                            }
                                    }
                                }
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                            }
                        }
                    }
                }

                // setting colspan to colheader

                for (var i = 0; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", logicalrule.Rows[i].Cells[j].Colspan - 1);
                            break;
                        }
                    }
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var AddConditionforRow = function (obj, param, itemscope, logicalrule) {
        $rootScope.UndRedoBulkOp("Start");
        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var indexofobj = logicalrule.Rows.indexOf(obj);
        var lengthofobj = obj.Cells.length;
        var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
        var objElements = [];
        if (param == 'RowCondition') {
            //getting the rows
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                if (lengthofobj == maxlength && indexofcell == 0) {
                    if ((logicalrule.Rows[i].Cells.length < lengthofobj) || i == indexofobj) {
                        var cells = { Cells: logicalrule.Rows[i].Cells };
                        var data = JSON.stringify(cells);
                        var clonecells = JSON.parse(data);
                        objElements.push(clonecells);
                    }
                    else {
                        break;
                    }
                }
                else {
                    var objcells = [];
                    if ((logicalrule.Rows[i].Cells.length < lengthofobj - indexofcell) || i == indexofobj) {

                        if (i == indexofobj) {
                            for (var j = indexofcell; j < logicalrule.Rows[i].Cells.length; j++) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        else {
                            for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        var cells = { Cells: objcells };

                        var data = JSON.stringify(cells);
                        var clonecells = JSON.parse(data);
                        objElements.push(clonecells);
                    }
                    else {
                        break;
                    }
                }
            }
            // setting nodeid

            for (var i = 0; i < objElements.length; i++) {
                for (var j = 0; j < objElements[i].Cells.length; j++) {
                    objElements[i].Cells[j].Item.NodeID = generateUUID();
                }
            }

            // adding the rows
            var p = 0;

            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                if (p < objElements.length) {
                    $rootScope.InsertItem(objElements[p], logicalrule.Rows, i + objElements.length);
                    //logicalrule.Rows.splice(i + objElements.length, 0, objElements[p]);
                    p++;
                }
                else {
                    break;
                }
            }

            // setting rowspan
            var objheaderid = itemscope.objChild.Item.HeaderID;

            for (var i = indexofobj; i >= 0; i--) {
                if (logicalrule.Rows[i].Cells.length >= lengthofobj) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.HeaderID == objheaderid) {
                            objheaderid = logicalrule.Rows[i].Cells[0].Item.HeaderID;
                            lengthofobj = logicalrule.Rows[i].Cells.length;
                            break;
                        }
                        else {
                            $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Rowspan, logicalrule.Rows[i].Cells[j], "Rowspan", logicalrule.Rows[i].Cells[j].Rowspan + objElements.length);
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length == maxlength) {
                        break;
                    }
                }
            }

        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteConditionforRow = function (obj, param, itemscope, logicalrule) {

        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var count = 0;
        for (var j = 0; j < logicalrule.Rows.length; j++) {
            if (logicalrule.Rows[j].Cells.length == maxlength && logicalrule.Rows[j].Cells[0].Item.ItemType == 'if') {
                count++;
            }
        }
        if (count < 2 && obj.Cells.length == maxlength) {
            alert("Atleast one condition should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            var indexofobj = logicalrule.Rows.indexOf(obj);
            var indexofobjele = indexofobj;

            var lengthofobj = obj.Cells.length;
            var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
            var objElements = [];
            var count = 0;
            var cell;
            if (param == 'RowCondition') {
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    if (lengthofobj == maxlength && indexofcell == 0) {
                        if ((logicalrule.Rows[i].Cells.length < lengthofobj) || i == indexofobj) {
                            $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                            //logicalrule.Rows.splice(i, 1);
                            indexofobj -= 1;
                            indexofobjele -= 1;
                            i--;
                        }
                        else {
                            break;
                        }
                    }
                    else {

                        if ((logicalrule.Rows[i].Cells.length < lengthofobj - indexofcell) || i == indexofobj) {
                            count++;
                            if (cell == undefined) {
                                for (var m = 0; m < logicalrule.Rows[i].Cells.length; m++) {
                                    if (logicalrule.Rows[i].Cells[m].Rowspan == itemscope.objChild.Rowspan) {
                                        indexofcell = m;
                                        cell = logicalrule.Rows[i].Cells[m];
                                        break;
                                    }
                                }
                            }

                            if (indexofcell == 0 && i == indexofobj) {
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                                indexofobjele -= 1;
                                indexofobj -= 1;
                                i--;
                            }
                            else if (i == indexofobj) {


                                if (itemscope.objChild.Rowspan == logicalrule.Rows[i].Cells[0].Rowspan && logicalrule.Rows[i].Cells.length == maxlength) {
                                    $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                    //logicalrule.Rows.splice(i, 1);
                                    count = 0;
                                }
                                else {
                                    for (var j = indexofcell; j < logicalrule.Rows[i].Cells.length; j++) {
                                        $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                        //logicalrule.Rows[i].Cells.splice(j, 1);
                                        //indexofobj -= 1;
                                        //indexofobjele -= 1;
                                        j--;
                                    }

                                    for (l = 1; logicalrule.Rows.length; l++) {
                                        if (logicalrule.Rows[i + l].Cells[0].Item.HeaderID == cell.Item.HeaderID) {
                                            for (var k = 0; k < logicalrule.Rows[i + l].Cells.length; k++) {
                                                $rootScope.PushItem(logicalrule.Rows[i + l].Cells[k], logicalrule.Rows[i].Cells);
                                                //logicalrule.Rows[i].Cells.push(logicalrule.Rows[i + l].Cells[k]);
                                            }
                                            break;
                                        }
                                        else {
                                            $rootScope.DeleteItem(logicalrule.Rows[i + l], logicalrule.Rows);
                                            //logicalrule.Rows.splice(i + l, 1);
                                            l--;
                                        }

                                    }
                                    $rootScope.DeleteItem(logicalrule.Rows[i + l], logicalrule.Rows);
                                    //logicalrule.Rows.splice(i + l, 1);

                                    count = cell.Rowspan;
                                    break;
                                }
                            }
                            else {
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                                i--;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }

                //setting rowspan
                if (cell != undefined) {
                    var objheaderid = cell.Item.HeaderID;
                }
                else {
                    var objheaderid = itemscope.objChild.Item.HeaderID;
                }

                for (var i = indexofobjele; i >= 0; i--) {
                    if (logicalrule.Rows[i].Cells.length >= lengthofobj) {
                        for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                            if (logicalrule.Rows[i].Cells[j].Item.HeaderID == objheaderid) {
                                objheaderid = logicalrule.Rows[i].Cells[0].Item.HeaderID;
                                lengthofobj = logicalrule.Rows[i].Cells.length;
                                break;
                            }
                            else {
                                $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Rowspan, logicalrule.Rows[i].Cells[j], "Rowspan", logicalrule.Rows[i].Cells[j].Rowspan - count);
                            }
                        }
                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            break;
                        }
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var AddConditionforColumn = function (obj, param, itemscope, logicalrule) {

        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var startindex;
        var endindex;
        var objElements = [];
        for (var j = 0; j < obj.Cells.length; j++) {
            if (obj.Cells[j].Item.ItemType == 'colheader') {
                startindex = obj.Cells[j].Colspan;
            }
            if (obj.Cells[j].Item.ItemType == 'if') {
                endindex = startindex + obj.Cells[j].Colspan;
            }
            if (endindex != undefined) {
                if (itemscope.objChild.Item.NodeID == obj.Cells[j].Item.NodeID) {


                    break;
                }
                else {
                    startindex = endindex;
                }
            }
        }

        var indexofobj = logicalrule.Rows.indexOf(obj);
        var lengthofobj = obj.Cells.length;
        var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);

        if (param == 'ColumnCondition') {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                var elestartindex = 0;
                var eleendindex = 0;
                var objcells = [];
                for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (eleendindex != 0) {
                            if (startindex <= elestartindex && endindex >= eleendindex) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                                elestartindex = eleendindex;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }

                    else {

                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            if (endindex > j && startindex <= j) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        else {
                            var diff = maxlength - logicalrule.Rows[i].Cells.length;
                            if ((endindex - diff) > j && (startindex - diff) <= j) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                    }
                }

                var cells = { Cells: objcells };
                var data = JSON.stringify(cells);
                var clonecells = JSON.parse(data);
                objElements.push(clonecells);
            }

            // setting nodeid

            for (var i = 0; i < objElements.length; i++) {
                for (var j = 0; j < objElements[i].Cells.length; j++) {
                    objElements[i].Cells[j].$$hashKey = generateUUID() + "";
                    objElements[i].Cells[j].Item.NodeID = generateUUID();
                }
            }

            //inserting the elements

            var l = 0;
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                var elestartindex = 0;
                var eleendindex = 0;
                var objcells = [];
                for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (eleendindex != 0) {
                            if (endindex == eleendindex) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }

                    else {

                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            if ((endindex - 1) == j) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                        }
                        else {
                            var diff = maxlength - logicalrule.Rows[i].Cells.length;
                            if (((endindex - 1) - diff) == j) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                        }
                    }
                }
                l++;
            }

            // setting Column span
            var objelemaxlength = 0;
            for (var i = 0; i < objElements.length; i++) {
                if (objelemaxlength < objElements[i].Cells.length) {
                    objelemaxlength = objElements[i].Cells.length;
                }
            }

            for (var j = indexofobj - 1; j >= 0; j--) {
                var elestartindex = 0;
                var eleendindex = 0;
                for (k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (eleendindex != 0) {

                            if (elestartindex <= startindex && eleendindex >= endindex) {
                                $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan + objelemaxlength);
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var DeleteConditionforColumn = function (obj, param, itemscope, logicalrule) {
        if (obj.Cells.length == 2) {
            alert("Atleast one condition should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            var maxlength = 0;
            for (var i = 0; i < logicalrule.Rows.length; i++) {
                if (maxlength < logicalrule.Rows[i].Cells.length) {
                    maxlength = logicalrule.Rows[i].Cells.length;
                }
            }
            var startindex;
            var endindex;
            var objElements = [];
            for (var j = 0; j < obj.Cells.length; j++) {
                if (obj.Cells[j].Item.ItemType == 'colheader') {
                    startindex = obj.Cells[j].Colspan;
                }
                if (obj.Cells[j].Item.ItemType == 'if') {
                    endindex = startindex + obj.Cells[j].Colspan;
                }
                if (endindex != undefined) {
                    if (itemscope.objChild.Item.NodeID == obj.Cells[j].Item.NodeID) {


                        break;
                    }
                    else {
                        startindex = endindex;
                    }
                }
            }

            var indexofobj = logicalrule.Rows.indexOf(obj);
            var lengthofobj = obj.Cells.length;
            var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);

            if (param == 'ColumnCondition') {

                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    var elestartindex = 0;
                    var eleendindex = 0;
                    var objcells = [];
                    for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                            if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                                elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                            if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                                eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                            }
                            if (eleendindex != 0) {
                                if (startindex <= elestartindex && endindex >= eleendindex) {
                                    elestartindex = eleendindex;
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                    elestartindex = eleendindex;
                                }
                            }
                        }
                        else {

                            if (logicalrule.Rows[i].Cells.length == maxlength) {
                                if (endindex > j && startindex <= j) {
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                }
                            }
                            else {
                                var diff = maxlength - logicalrule.Rows[i].Cells.length;
                                if ((endindex - diff) > j && (startindex - diff) <= j) {
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                }
                            }
                        }
                    }

                    var cells = { Cells: objcells };
                    var data = JSON.stringify(cells);
                    var clonecells = JSON.parse(data);
                    objElements.push(clonecells);
                }
            }

            // setting columnspan

            for (var j = indexofobj - 1; j >= 0; j--) {
                var elestartindex = 0;
                var eleendindex = 0;
                for (k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (eleendindex != 0) {

                            if (elestartindex <= startindex && eleendindex >= endindex) {
                                if (logicalrule.Rows[j].Cells[k].Colspan != itemscope.objChild.Colspan) {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan - itemscope.objChild.Colspan);

                                }
                                else {
                                    $rootScope.DeleteItem(logicalrule.Rows[j].Cells[k], logicalrule.Rows[j].Cells);
                                    //logicalrule.Rows[j].Cells.splice(k, 1);
                                }
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }
                }
            }


            //replacing the elements
            var q = indexofobj;
            while (q < logicalrule.Rows.length) {
                $rootScope.DeleteItem(logicalrule.Rows[q], logicalrule.Rows);
            }

            for (var i = 0; i < objElements.length; i++) {
                $rootScope.PushItem(objElements[i], logicalrule.Rows);
            }
            //var q = 0;
            //for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
            //    logicalrule.Rows[i] = objElements[q];
            //    q++;
            //}
            $rootScope.UndRedoBulkOp("End");
        }

    };

    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '=',
            rcviewmode: '=',
            returntype: '='
        },
        template: '<td rowconditionitemdirective ng-repeat="objChild in items.Cells" items="objChild" context-menu="menuOptionsforitems" valign="top" rowspan={{objChild.Rowspan}} colspan={{objChild.Colspan}} class="dt-table-cell"  logicalrule="logicalrule" rciviewmode="rcviewmode" ng-mousedown="objStepSelectionChanged(objChild)"></td>',
        link: function (scope, element, attrs) {
            scope.menuOptionsforitems = [
                ['Add Condition ', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionforRow(obj, 'RowCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'if'));
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteConditionforRow(obj, 'RowCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'if'));
                }], null,

                ['Add Condition ', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionforColumn(obj, 'ColumnCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'colheader'));
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteConditionforColumn(obj, 'ColumnCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'colheader'));
                }], null,

                ['Add Action ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'actions', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null;
                }],
                null,
                ['Add Return ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'return', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return (($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null) && (scope.returntype);
                }],
                null,
                ['Add Notes ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'notes', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null;
                }],
                null,
                ['Delete Action ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/assignheader/) != null;
                }],
                null,
                ['Delete Return ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/returnheader/) != null;
                }],
                null,
                ['Delete Notes ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/notesheader/) != null;
                }],
                null,
                ['Add Horizantal Condition', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Top', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    var indexofobj = scope.logicalrule.Rows.indexOf(obj);
                    return ($itemScope.objChild.Item.ItemType.match(/rowheader/) && (indexofobj == 0));
                }],
                null,

                ['Add Condition in Right', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInLeftOrRight(obj, 'Right', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/rowheader/) != null;
                }],
                null,
                ['Add Condition in Left', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInLeftOrRight(obj, 'Left', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/rowheader/) != null;
                }], null,

                ['Add Condition in Bottom', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Bottom', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/colheader/) != null;
                }],
                null,
                ['Add Condition in Top', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Top', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/colheader/) != null;
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteCondition(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/colheader/) || $itemScope.objChild.Item.ItemType.match(/rowheader/)) != null;
                }], null
            ];
            scope.objStepSelectionChanged = function (step) {
                var rootScope = getCurrentFileScope();
                rootScope.onDecisionStepChange(step);
            };
        }
    };
}]);


app.directive('excelmatrixscrolldirective', ["$interval", function ($interval) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            scope.promise = null;
            scope.LoadExcelMatrix = function () {
                scope.LoadSomeRowsinExcelMatrix();
                if ((element.hasScrollBar() || scope.objSelectedLogicalRule.objColumnValues.length == scope.objSelectedLogicalRule.objExcelColumnValues.length) && scope.promise) {
                    $interval.cancel(scope.promise);
                }
            }
            setTimeout(function () {
                scope.promise = $interval(function () {
                    scope.LoadExcelMatrix();
                }, 500);
            }, 1000);
            $(element).scroll(function () {
                if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        scope.LoadSomeRowsinExcelMatrix();
                    });
                }
            });
        }
    };
}]);

app.directive('decisiontablescrolldirective', ["$interval", function ($interval) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            scope.promise = null;
            scope.LoadDecisionTable = function () {
                if ((element.hasScrollBar() || scope.objSelectedLogicalRule.Rows.length <= scope.objSelectedLogicalRule.noConditionsDisplayed) && scope.promise) {
                    $interval.cancel(scope.promise);
                } else {
                    if (scope.objSelectedLogicalRule.Rows.length > scope.objSelectedLogicalRule.noConditionsDisplayed) {
                        scope.objSelectedLogicalRule.noConditionsDisplayed = scope.objSelectedLogicalRule.noConditionsDisplayed + 10;
                    }
                }
            }
            setTimeout(function () {
                scope.promise = $interval(function () {
                    scope.LoadDecisionTable();
                }, 500);
            }, 1000);
            $(element).scroll(function () {
                if (raw.scrollTop + raw.offsetHeight >= (raw.scrollHeight - 5)) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        if (scope.objSelectedLogicalRule.Rows.length > scope.objSelectedLogicalRule.noConditionsDisplayed) {
                            scope.objSelectedLogicalRule.noConditionsDisplayed = scope.objSelectedLogicalRule.noConditionsDisplayed + 10;
                        }
                    });
                }
            });
        }
    };
}]);