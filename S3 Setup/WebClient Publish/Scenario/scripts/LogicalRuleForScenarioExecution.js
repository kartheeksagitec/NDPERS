app.directive('logicalRuleNodeWrapperForScenarioExecution', ["$compile", "$rootScope", "$timeout", "$EntityIntellisenseFactory", function ($compile, $rootScope, $timeout, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        scope: {
            model: "=",
            objentity: '=',
            objmainlogicalrule: '=',
            scenarioruntype: "=",
            parentModel: "="
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getNodeTemplate = function () {
                var templateText = "";
                switch (scope.model.Name) {
                    case "actions":
                        templateText = '<actions-node-for-scenario-execution></actions-node-for-scenario-execution>';
                        break;
                    case "break":
                    case "continue":
                    case "default":
                        templateText = '<break-continue-default-node-for-scenario-execution></break-continue-default-node-for-scenario-execution>';
                        break;
                    case "notes":
                        templateText = '<notes-node-for-scenario-execution></notes-node-for-scenario-execution>';
                        break;
                    case "return":
                    case "switch":
                    case "case":
                    case "while":
                        templateText = '<return-switch-case-while-node-for-scenario-execution></return-switch-case-while-node-for-scenario-execution>';
                        break;
                    case "calllogicalrule":
                    case "calldecisiontable":
                    case "callexcelmatrix":
                        templateText = '<call-rule-node-for-scenario-execution></call-rule-node-for-scenario-execution>';
                        break;
                    case "foreach":
                        templateText = '<for-each-node-for-scenario-execution></for-each-node-for-scenario-execution>';
                        break;
                    case "query":
                        templateText = '<query-node-for-scenario-execution></query-node-for-scenario-execution>';
                        break;
                    case "method":
                        templateText = '<method-node-for-scenario-execution></method-node-for-scenario-execution>';
                        break;

                }
                return templateText;
            };
            scope.getTemplate = function () {
                var templateTextList = ['<div node-id="{{::model.dictAttributes.sfwNodeID}}" node-name="{{::model.Name}}" ng-keydown="onkeydownevent($event,model,parentModel)">                                                                                 '
                                        , '    <span ng-show="(model.Name === \'foreach\' || model.Name === \'while\' || model.Name === \'switch\') || (model.Name !== \'case\' && model.Name !== \'default\' && model.Children.length > 0) || ((model.Name === \'case\' || model.Name === \'default\') && model.Elements.length > 0)" class="node-expander node-expanded" ng-click="toggleNodeChildren($event)"></span>                                                                                              '
                                        , '    <div class="node-wrapper node-wrapper-after">                                                                                                                                        '];
                templateTextList.push(scope.getNodeTemplate());
                templateTextList = templateTextList.concat(['<ul ng-show="model.CanExpandOrCollapse" ng-if="model.Name == \'foreach\' || model.Name == \'while\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                                                            , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                                                            , '                <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                           '
                                                            , '            </li>                                                                                                                                                                        '
                                                            , '                                                                                                                                                                                         '
                                                            , '        </ul>'
                                                            , '<ul ng-if="model.Name == \'switch\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                                                            , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                                                            , '                <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                           '
                                                            , '            </li>                                                                                                                                                                        '
                                                            , '                                                                                                                                                                                         '
                                                            , '        </ul> '
                                                            , '    </div>                                                                                                                                                                               '
                                                            , '    <ul ng-if="(model.Name == \'case\' || model.Name == \'default\') && model.Elements.length>0">                                                                                        '
                                                            , '        <li ng-repeat="item in model.Elements">                                                                                                                                          '
                                                            , '            <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                               '
                                                            , '        </li>                                                                                                                                                                            '
                                                            , '    </ul>                                                                                                                                                                                '
                                                            , '    <ul ng-if="model.Name != \'case\' && model.Name != \'default\' && model.Children.length>0">                                                                                          '
                                                            , '        <li ng-repeat="item in model.Children">                                                                                                                                          '
                                                            , '            <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                               '
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

            scope.getClass = function () {

                if (scope.model) {
                    if (scope.model.Name == "default") {
                        if (scope.model.IsStepSelected) {
                            return "case-block-execution";
                        }
                        else if (scope.model.IsErrorOccured) {
                            return "case-block-error-occur ";
                        }
                        else {
                            return "case-block-default";
                        }
                    }
                    else {
                        if (scope.model.dictAttributes && scope.model.dictAttributes.sfwCommented == "True") {
                            return "text-assign-execution-commented";
                        }
                        if (scope.model.IsStepSelected) {
                            return "text-assign-execution";
                        }
                        else if (scope.model.IsErrorOccured) {
                            return "text-assign-error-occur ";
                        }
                        else {
                            return "text-assign-default";
                        }
                    }
                }
            };

            scope.OnLoopExecutionClick = function () {
                if (scope.model.Name == "foreach" || scope.model.Name == "while") {
                    if ((scope.model.CanExpandOrCollapse != undefined && !scope.model.CanExpandOrCollapse) && scope.model.LoopExecutionCount !== undefined && scope.model.LoopExecutionCount > 0) {

                        var newScope = scope.$new();
                        newScope.LoopViewModel = GetBaseModel(scope.model);//angular.copy(scope.model);
                        newScope.LoopViewModel.TestExeRuleObject = scope.model.TestExeRuleObject;
                        newScope.LoopViewModel.IsStepSelected = scope.model.IsStepSelected;
                        newScope.LoopViewModel.CanExpandOrCollapse = true;
                        newScope.LoopViewModel.LoopExecutionCount = 0;
                        newScope.IsDiagramDivExpanded = true;
                        newScope.Elements = [];
                        newScope.IsWhileLoop = false;
                        if (scope.model.Name == "while") {
                            newScope.IsWhileLoop = true;
                        }
                        newScope.InitLoopExecutionSteps = function () {


                            if (newScope.LoopViewModel.Name == "foreach") {

                                newScope.SetEntity();

                                if (typeof newScope.LoopViewModel.TestExeRuleObject.iobjData != "undefined" && newScope.LoopViewModel.TestExeRuleObject.iobjData.length !== undefined) {
                                    var loopData = newScope.LoopViewModel.TestExeRuleObject.iobjData;
                                    if (loopData != undefined) {
                                        var i = 0;
                                        angular.forEach(loopData, function (obj) {
                                            var step;
                                            if (i < newScope.LoopViewModel.TestExeRuleObject.ilstSteps.length) {
                                                step = { RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: newScope.LoopViewModel.TestExeRuleObject.ilstSteps[i], index: i, properties: {} };
                                            }
                                            else {
                                                step = { RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: undefined, index: i, properties: {} };
                                            }
                                            if (step != undefined) {
                                                newScope.setValueForStep(step, i);
                                            }
                                            newScope.Elements.push(step);
                                            i++;
                                        });
                                    }
                                }
                                else {
                                    var i = 0;
                                    var step;
                                    angular.forEach(newScope.LoopViewModel.TestExeRuleObject.ilstSteps, function (item) {
                                        step = { RuleObj: undefined, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: item, index: index, properties: {} };
                                        newScope.Elements.push(step);
                                        index++;
                                    });
                                }

                                newScope.GetLoopParameter();

                                newScope.GetColumns();
                            }
                            else if (newScope.LoopViewModel.Name == "while") {
                                var loopData = newScope.LoopViewModel.TestExeRuleObject.ilstSteps;
                                var index = 0;
                                angular.forEach(loopData, function (obj) {
                                    var step;
                                    step = { Name: "Iteration" + index, RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: obj, index: i, properties: {} };
                                    if (step != undefined) {
                                        newScope.setValueForStep(step, index);
                                    }
                                    newScope.Elements.push(step);
                                    index++;
                                });
                            }

                            if (newScope.Elements !== undefined && newScope.Elements.length > 0) {
                                newScope.UpdateStepOnSelectionChange(newScope.Elements[0]);
                            }
                        };

                        newScope.SetEntity = function () {
                            var loopParameterExp = newScope.LoopViewModel.dictAttributes.sfwObjectID;
                            var loopColExpFullPath = CheckAndGetExpressionValue(newScope.LoopViewModel, loopParameterExp);
                            var curEntityID = "";
                            if (scope.objentity != undefined) {
                                curEntityID = scope.objentity.dictAttributes.ID;
                            }

                            var entityId = "";
                            if (loopColExpFullPath !== undefined && loopColExpFullPath !== "") {
                                angular.forEach(loopColExpFullPath.split("."), function (parameter) {
                                    if (scope.objentity != undefined) {
                                        entityId = GetEntityID(scope.objentity, parameter);
                                    }
                                });


                                if (entityId !== undefined && entityId !== "") {
                                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                    angular.forEach(entityIntellisenseList, function (entity) {
                                        if (entity.ID == entityId) {
                                            newScope.objEntity = entity;
                                        }
                                    });
                                }
                            }
                        };
                        newScope.CheckAndSetValFotIsDataTable = function () {
                            var stIndex = newScope.LoopViewModel.dictAttributes.sfwObjectID.lastIndexOf('.');
                            if (stIndex < 0) {
                                stIndex = 0;
                            }

                            var fieldName = newScope.LoopViewModel.dictAttributes.sfwObjectID.substring(stIndex);
                            if (fieldName !== undefined && fieldName !== "") {
                                newScope.IsDataTable = false;
                                var model = newScope.objEntity.Queries.filter(function (item) {
                                    if (item.ID == fieldName) {
                                        newScope.IsDataTable = true;
                                    }
                                });



                            }
                        };

                        newScope.setValueForStep = function (step, index) {

                            if (step.ExecutedSteps == undefined) {
                                step.IsExecuted = false;
                            }
                            else {
                                step.IsExecuted = true;
                            }

                            if (step.RuleObj != undefined) {
                                if (step.RuleObj.EntityFields == undefined && step.RuleObj.Collections == undefined) {
                                    step.Name = "Iteration " + index;
                                }
                                else if (Object.keys(step.RuleObj.EntityFields).length == 0 && Object.keys(step.RuleObj.Collections).length == 0) {
                                    step.Name = "Iteration " + index;
                                }
                                else {
                                    angular.forEach(step.RuleObj.EntityFields, function (value, key) {
                                        step.properties[key] = value;
                                    });

                                    angular.forEach(step.RuleObj.Collections, function (value, key) {
                                        step.properties[key] = step.index;
                                    });

                                    angular.forEach(step.RuleObj.Entities, function (value, key) {
                                        newScope.setValueForStepForObject(value, key, step);
                                    });
                                }
                            }
                        };

                        newScope.setValueForStepForObject = function (ruleObj, ParentNodeName, step) {
                            var columnName = ParentNodeName;

                            angular.forEach(ruleObj.EntityFields, function (value, key) {
                                step.properties[columnName + "." + key] = value;
                            });

                            angular.forEach(ruleObj.Collections, function (value, key) {
                                step.properties[columnName + "." + key] = step.index;
                            });

                            angular.forEach(ruleObj.Entities, function (value, key) {
                                newScope.setValueForStepForObject(value, columnName + "." + key, step);
                            });
                        }

                        newScope.GetLoopParameter = function () {
                            var loopParameterExp = GetExpressionValue(newScope.LoopViewModel, newScope.LoopViewModel.dictAttributes.sfwObjectID);
                            if (scope.objmainlogicalrule != undefined) {
                                newScope.LoopParameter = GetLoopParameter(scope.objmainlogicalrule, loopParameterExp);
                            }
                        };

                        newScope.GetColumns = function () {
                            newScope.Columns = [];
                            if (newScope.Elements.length > 0) {
                                var ruleObj = newScope.Elements[0].RuleObj;
                                if (ruleObj !== undefined) {
                                    angular.forEach(ruleObj.EntityFields, function (value, key) {
                                        newScope.Columns.push(key);
                                    });

                                    angular.forEach(ruleObj.Collections, function (value, key) {
                                        newScope.Columns.push(key);
                                    });

                                    angular.forEach(ruleObj.Entities, function (value, key) {
                                        newScope.GetColumnsForObject(value, key);
                                    });
                                }
                                else {
                                    if (newScope.objEntity !== undefined) {
                                        var attributes = $rootScope.getEntityAttributeIntellisense(newScope.objEntity.ID, true);
                                        angular.forEach(attributes, function (field) {
                                            newScope.Columns.push(field.ID);
                                        });
                                    }
                                    else if (newScope.LoopParameter !== undefined) {
                                        angular.forEach(newScope.LoopParameter.Elements, function (parameter) {
                                            newScope.Columns.push(parameter.dictAttributes.ID);
                                        });
                                    }
                                    else {
                                        newScope.Columns.push("Iteration");
                                    }
                                }
                            }
                        };

                        newScope.GetColumnsForObject = function (ruleObj, ParentNodeName) {
                            var columnName = ParentNodeName;

                            angular.forEach(ruleObj.EntityFields, function (value, key) {
                                newScope.Columns.push(columnName + "." + key);
                            });

                            angular.forEach(ruleObj.Collections, function (value, key) {
                                newScope.Columns.push(columnName + "." + key);
                            });

                            angular.forEach(ruleObj.Entities, function (value, key) {
                                newScope.GetColumnsForObject(value, columnName + "." + key);
                            });
                        }

                        newScope.render = function (condition) {
                            if (condition) {
                                var loopwindows = [];
                                loopwindows = $('.run-result-spliter-loop');
                                angular.forEach(loopwindows, function (childwindow) {
                                    $(childwindow).width('100%').height('400px').split({ orientation: 'vertical', limit: 20 });

                                });

                            }

                        };

                        newScope.ExpandCollapseNode = function () {
                            $timeout(function () {
                                var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-expander");
                                if (elements && elements.length > 0) {
                                    for (var i = 0; i < elements.length > 0; i++) {
                                        var scope = angular.element(elements[i]).scope();
                                        if (scope && scope.model && !scope.model.IsStepSelected) {
                                            collapseNode(elements[i], true);
                                        }
                                    }
                                }

                                var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-collapsed");
                                if (elements && elements.length > 0) {
                                    for (var i = 0; i < elements.length > 0; i++) {
                                        var scope = angular.element(elements[i]).scope();
                                        if (scope && scope.model && scope.model.IsStepSelected) {
                                            expandNode(elements[i], true);
                                        }
                                    }
                                }
                            });
                        }

                        newScope.UpdateStepOnSelectionChange = function (objStep) {

                            newScope.SelectedStep = objStep;
                            if (newScope.SelectedStep != undefined) {
                                newScope.LoopViewModel.IsShowDiagram = newScope.SelectedStep.IsExecuted;
                                newScope.LoopViewModel.IsStepSelected = false;
                                if (newScope.SelectedStep.IsExecuted) {
                                    newScope.LoopViewModel.IsStepSelected = true;
                                    angular.forEach(newScope.LoopViewModel.Elements, function (childStep) {
                                        if (childStep.Name != "items") {
                                            CheckAndUpdateStep(childStep, newScope.SelectedStep.ExecutedSteps, scope.scenarioruntype);
                                        }
                                    });

                                    if (newScope.LoopViewModel.Children != null) {
                                        angular.forEach(newScope.LoopViewModel.Children, function (childStep) {
                                            if (childStep.Name != "items") {
                                                CheckAndUpdateStep(childStep, newScope.SelectedStep.ExecutedSteps, scope.scenarioruntype);
                                            }
                                        });
                                    }
                                }

                                newScope.ExpandCollapseNode();
                            }
                        };

                        newScope.setClassForLoop = function (step) {
                            if (step == newScope.SelectedStep) {
                                return "selected";
                            }

                            else if (!step.IsExecuted) {
                                return "disabled";
                            }
                        };

                        newScope.ExpandCollapseDiagram = function () {
                            newScope.IsDiagramDivExpanded = !newScope.IsDiagramDivExpanded;
                        };

                        newScope.loopExecutionDialog = $rootScope.showDialog(newScope, "Run Result For Loop", "Scenario/views/TestLoopExecution.html", { height: 550, width: 1000 });

                        newScope.InitLoopExecutionSteps();

                        newScope.ExpandCollapseNode();

                    }
                }
            };


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
    };
}]);

app.directive('actionsNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ActionsNodeTemplate.html",
    };
}]);
app.directive('breakContinueDefaultNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/BreakContinueDefaultNodeTemplate.html",
    };
}]);
app.directive('callRuleNodeForScenarioExecution', ["$rootScope", "$EntityIntellisenseFactory", "$timeout", function ($rootScope, $EntityIntellisenseFactory, $timeout) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.OnCallLogicalRuleDoubleClick = function () {
                if (scope.scenarioruntype == "AllRuleSteps") {
                    if (scope.model != undefined) {
                        if (scope.model.Name == "calllogicalrule" || scope.model.Name == "calldecisiontable" || scope.model.Name == "callexcelmatrix") {
                            var ruleId = scope.model.dictAttributes.sfwRuleID;
                            $.connection.hubScenarioModel.server.checkAndLoadRuleDiagram(ruleId).done(function (data) {
                                scope.$apply(function () {
                                    scope.objLogicalRule = data;
                                    var newScope = scope.$new();
                                    newScope.objLogicalRule = scope.objLogicalRule;
                                    newScope.scenarioruntype = scope.scenarioruntype;
                                    var strRuleID = "";
                                    var ruleType = "DecisionTable";
                                    if (scope.objLogicalRule.dictAttributes != undefined) {
                                        ruleType = scope.objLogicalRule.dictAttributes.sfwRuleType;
                                        strRuleID = scope.objLogicalRule.dictAttributes.ID;
                                    }
                                    else {
                                        strRuleID = scope.objLogicalRule.RuleID;
                                    }
                                    newScope.objEntity = scope.objentity;
                                    var ElapsedTime = "";
                                    var EffectiveDate = "Default";
                                    if (scope.model.TestExeRuleObject != undefined) {
                                        newScope.objSelectedLogicalRule = GetClosestLogicalRule(scope.objLogicalRule, scope.model.TestExeRuleObject.istrOtherData, ruleType);
                                        EffectiveDate = !scope.model.TestExeRuleObject.istrOtherData ? 'Default' : scope.model.TestExeRuleObject.istrOtherData;
                                        ElapsedTime = scope.model.TestExeRuleObject.ExecutionTime;
                                        var executedRuleSteps = [];
                                        if (scope.model.TestExeRuleObject.ilstSteps.length > 0 && scope.model.TestExeRuleObject.ilstSteps[0]) {
                                            executedRuleSteps = scope.model.TestExeRuleObject.ilstSteps[0];
                                        }
                                        UpdateTestExecutionFlow(executedRuleSteps, ruleType, newScope.objSelectedLogicalRule, newScope.scenarioruntype);
                                        //var scenarioscope = GetCurrentScopeObject("Scenario");
                                        //scope.objSelectedLogicalRule =  scenarioscope.objSelectedLogicalRule1;
                                    }
                                    newScope.callRuleExecutionDialog = $rootScope.showDialog(newScope, "Rule : " + strRuleID + " - Effective Date : " + EffectiveDate + "[ Elapsed Time: " + ElapsedTime + "]", "Scenario/views/TestCallLogicalRuleExecution.html", { width: 900, height: 500 });
                                    newScope.ShowOnlyExecutedPath = function (ablnShowOnlyExecutedPath) {
                                        ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, newScope.objSelectedLogicalRule);
                                    };
                                    $timeout(function () {
                                        var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-expander");
                                        if (elements && elements.length > 0) {
                                            for (var i = 0; i < elements.length > 0; i++) {
                                                var scope = angular.element(elements[i]).scope();
                                                if (scope && scope.model && !scope.model.IsStepSelected) {
                                                    collapseNode(elements[i], true);
                                                }
                                            }
                                        }
                                    });

                                });
                            });
                        }
                    }
                }
            };


        },
        templateUrl: "Scenario/views/LogicalRule/CallRuleNodeTemplate.html",
    };
}]);
app.directive('forEachNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ForEachNodeTemplate.html",
    };
}]);
app.directive('methodNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/MethodNodeTemplate.html",
    };
}]);
app.directive('notesNodeForScenarioExecution', ["$rootScope", function ($rootScope) {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/NotesNodeTemplate.html",
    };
}]);
app.directive('queryNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/QueryNodeTemplate.html",
    };
}]);
app.directive('returnSwitchCaseWhileNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ReturnSwitchCaseWhileNodeTemplate.html",
    };
}]);

var GetEntityID = function (objEntity, parameter) {
    var retVal = "";
    var objField;
    var Attributes = objEntity.Elements.filter(function (x) {
        if (x.Name == "attributes") {
            return x;
        }
    });
    if (Attributes && Attributes.length > 0) {
        objField = Attributes[0].Elements.filter(function (x) {
            if (x.Name == "attribute" && x.dictAttributes.ID == parameter) {
                return x;
            }
        });
    }

    if (objField != undefined && objField.length > 0) {
        retVal = objField[0].dictAttributes.sfwEntity;
    }

    return retVal;
};

