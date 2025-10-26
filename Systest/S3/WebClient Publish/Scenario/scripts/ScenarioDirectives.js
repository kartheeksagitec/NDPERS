//#region directives for Collection and object popup

app.directive("collactionrowdirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function (modelobject, objselectedstep) {
        if (modelobject.IsOutput) {

            var template = '<td > <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm min-width-80" title="{{items.dictAttributes.indexNumber}}" undoredodirective model="items" ng-model="items.dictAttributes.indexNumber"></td>';
            template += '<td valign="top" ng-repeat="item in items.Elements" ng-model="SelectedTestField">';
            template += '  <table><tr><td> <span ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"> Expected Value :</span></td>';
            template += ' <td>  <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm min-width-80" title="{{item.dictAttributes.Value}}" ng-model="item.dictAttributes.Value" undoredodirective model="item" ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"/> </td>';
            template += '  <td> <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopupForCollection(item)" ng-show="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\'"/></a> </td>';
            template += '  <td> <a><img src="images/Common/icon_add_dark.svg"  ng-click="OpenPopupForObject(item)" ng-show="item.dictAttributes.sfwDataType == \'Object\'"/></a></td></tr> ';
            if (objselectedstep.IsRunSelected) {
                template += '<tr ng-hide="(item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\') "><td> <div > Actual Value :</div></td><td><span>{{item.ResultValue}}</span></td></tr>';
            }

            template += '<tr ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'">  <td> <span>Ignore Field :</span></td> <td><input type="checkbox" ng-disabled="isexcelbasedscenario" ng-true-value="\'True\'" ng-false-value="\'False\'" ng-model="item.dictAttributes.sfwIgnore" undoredodirective model="item"></td></tr>';

            if (objselectedstep.IsRunSelected) {
                template += '<tr ng-hide="(item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\')">   <td ng-hide="(item.dictAttributes.sfwIgnore == \'True\')"> Field Result :</td> <td ng-hide="(item.dictAttributes.sfwIgnore == \'True\')"> <i class="fa fa-check faCheckScenario" ng-show="item.Status"></i><i class="fa fa-times faTimesScenario" ng-show="!item.Status"></i></td></tr>';
            }

            template += '</table></td>';
            template += '<td > <input type="checkbox" ng-disabled="isexcelbasedscenario" ng-model="items.dictAttributes.sfwIgnore" undoredodirective model="items" ng-checked="items.dictAttributes.sfwIgnore" ng-true-value="\'True\'" ng-false-value="\'False\'"></td>';
            template += '<td ng-show="objselectedstep.IsRunSelected==true && (items.dictAttributes.sfwIgnore == \'False\' || items.dictAttributes.sfwIgnore==undefined)"><i class="fa fa-check faCheckScenario" ng-show="items.Status==true"></i><i class="fa fa-times faTimesScenario" ng-show="items.Status!=true"></i></td>';
            return template;
        }
        else {
            var template = '<td valign="top" ng-repeat="item in items.Elements" ng-model="SelectedTestField">';
            template += '   <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm" title="{{item.dictAttributes.Value}}" ng-model="item.dictAttributes.Value" undoredodirective model="item" ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"/>';
            template += '   <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopupForCollection(item)" ng-show="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\'"/></a>';
            template += '   <a><img src="images/Common/icon_add_dark.svg"  ng-click="OpenPopupForObject(item)" ng-show="item.dictAttributes.sfwDataType == \'Object\'"/></a>';
            template += '</td>';
            return template;
        }
        return "";
    };

    return {
        restrict: "A",
        replace: true,
        scope: {
            items: '=',
            model: '=',
            objentity: '=',
            objselectedstep: "=",
            isexcelbasedscenario: '=',
        },
        link: function (scope, element, attrs) {

            element.html(getTemplate(scope.model, scope.objselectedstep));
            $compile(element.contents())(scope);

            //#region Methods For Open popups  for collection and Object

            scope.OpenPopupForCollection = function (objField) {
                objField.objentity = scope.objentity;
                objField.lstCollectionColumn = [];
                objField.IsOutput = scope.model.IsOutput;
                objField.ExtraFields = [];

                if (objField.ruleDataInfo != undefined) {
                    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(objField, scope.objentity);
                    if (lstEntityFields != undefined) {
                        angular.forEach(lstEntityFields, function (childParameter) {
                            objField.lstCollectionColumn.push(childParameter);
                        });
                    }
                }

                if (objField.Elements.length > 0) {
                    angular.forEach(objField.Elements[0].Elements, function (item) {
                        if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                            var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                            if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                                entityFieldInfo.ilstEntityFields = [];
                            }
                            if (objField.ExtraFields == undefined) {
                                objField.ExtraFields = [];
                            }
                            $rootScope.PushItem(entityFieldInfo, objField.ExtraFields);
                        }
                    });
                }
                if (objField.ExtraFields != undefined) {
                    angular.forEach(objField.ExtraFields, function (childParameter) {
                        objField.lstCollectionColumn.push(childParameter);
                    });
                }

                var newScope = scope.$new();
                newScope.SelectedTestField = objField;
                newScope.SelectedTestField.IsOutput = false;
                newScope.objselectedstep = scope.objselectedstep;
                newScope.IsExcelBasedScenario = scope.isexcelbasedscenario;
                newScope.AddColumnForCollection = function () {
                    AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                };
                newScope.DeleteCollectionField = function () {
                    DeleteCollectionField(newScope.SelectedTestField, $rootScope);
                };
                newScope.UpdateSelectedCollectionField = function (objFields) {
                    UpdateSelectedCollectionField(objFields, newScope.SelectedTestField);
                };
                newScope.AddCollectionField = function () {
                    AddCollectionField(newScope.SelectedTestField, scope.objentity, $rootScope);
                };
                newScope.onCancelClick = function () {
                    newScope.collectionDialog.close();
                };

                newScope.menuOptionForCollection = [
                    ['Delete Column', function ($itemScope) {
                        DeleteColumnFromCollection(newScope.SelectedTestField, $itemScope.objcol);
                    }], null
                ];

                newScope.collectionDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenCollectionPopup.html");
            };

            scope.OpenPopupForObject = function (objField) {
                if (scope.model.IsOutput) {
                    scope.OpenPopupForOutputObject(objField, scope.objselectedstep);
                }
                else {
                    scope.SelectedTestField = objField;
                    scope.SelectedTestField.objentity = scope.objentity;
                    angular.forEach(scope.SelectedTestField.Elements, function (item) {
                        if (item.Name == "fields") {
                            scope.SelectedTestField.objFieldsVM = item;
                            return;
                        }
                    });
                    if (scope.SelectedTestField.objFieldsVM != undefined) {
                        angular.forEach(scope.SelectedTestField.Elements, function (item) {
                            if (item.Name == "field") {
                                scope.SelectedTestField.objFieldsVM.SelectedField = item;
                                return;
                            }
                        });
                    }

                    var newScope = scope.$new();
                    newScope.SelectedTestField = scope.SelectedTestField;
                    newScope.SelectedTestField.ExtraFields = [];

                    newScope.AddExtraFields = function (SelectedTestField) {
                        if (SelectedTestField.Elements.length > 0) {
                            angular.forEach(SelectedTestField.Elements[0].Elements, function (item) {
                                if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                                    var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                                    if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                                        entityFieldInfo.ilstEntityFields = [];
                                    }

                                    $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);
                                }
                            });
                        }
                    };
                    newScope.AddExtraFields(newScope.SelectedTestField);

                    newScope.IsExcelBasedScenario = scope.isexcelbasedscenario;
                    newScope.UpdateSelectedObjectField = function (objFields) {
                        UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
                    };
                    newScope.DeleteObjectField = function () {
                        DeleteObjectField(newScope.SelectedTestField, $rootScope);
                    };
                    newScope.AddColumnForCollection = function () {
                        AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                    };

                    newScope.OnConstantValueChange = function (objField) {
                        if (objField) {
                            $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
                        }
                    };

                    newScope.objectInputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopup.html");
                }
            };

            scope.OpenPopupForOutputObject = function (objField, objselectedstep) {
                scope.SelectedOutputTestField = objField;
                scope.SelectedOutputTestField.objentity = scope.objentity;
                angular.forEach(scope.SelectedOutputTestField.Elements, function (item) {
                    if (item.Name == "fields") {
                        scope.SelectedOutputTestField.objFieldsVM = item;
                        return;
                    }
                });
                if (scope.SelectedOutputTestField.objFieldsVM != undefined) {
                    angular.forEach(scope.SelectedOutputTestField.Elements, function (item) {
                        if (item.Name == "field") {
                            scope.SelectedOutputTestField.objFieldsVM.SelectedField = item;
                            return;
                        }
                    });
                }

                var newScope = scope.$new();
                newScope.SelectedTestField = scope.SelectedOutputTestField;
                newScope.objselectedstep = objselectedstep;

                newScope.UpdateSelectedObjectField = function (objFields) {
                    UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
                };
                newScope.DeleteObjectField = function () {
                    DeleteObjectField(newScope.SelectedTestField, $rootScope);
                };
                newScope.AddColumnForCollection = function () {
                    AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                };
                newScope.OnConstantValueChange = function (objField) {
                    if (objField) {
                        $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
                    }
                };
                newScope.objectOutputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopupForOutput.html");
            };

            //#endregion  Methods For Open popups  for collection and Object

        }
    };
}]);

//#endregion directives for Collection and object popup


//#region Logical rule  Execution directive

app.directive('ruledirectiveforscenarioexecution', ["$compile", function ($compile) {
    var getTemplatetable = function (content) {
        var template;
        if (content != undefined && content.dictAttributes != undefined) {

            if (content.dictAttributes.sfwRuleType == 'LogicalRule') {
                template = '<div class="logical-rule-container" id="dvLogicalRule">',
                    template += '<ul class="logical-rule-first-ul">',
                    template += "<li ng-if=\"item.Name != 'variables'\" ng-repeat='item in objSelectedLogicalRule.Elements'>",
                    template += '<logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="objSelectedLogicalRule" objentity="objEntity" objmainlogicalrule="objLogicalRule" scenarioruntype="scenarioruntype"></logical-rule-node-wrapper>',
                    template += '</li>',
                    template += '</ul>',
                    template += '</div>';
            }
            else if (content.dictAttributes.sfwRuleType == 'ExcelMatrix') {
                template = ' <div class="tree" custom-scroll >';
                template += '<span class="excelmatrix-columnheader">{{objLogicalRule.dictAttributes.columnHeader}}</span>';
                template += '<div class="excelmatrix-rowheader-wrapper">';
                template += '                            <span class="excelmatrix-rowheader" ng-bind="objLogicalRule.dictAttributes.rowHeader"></span>';
                template += '                        </div>';
                template += '<div class="excelmatrix-table" excelmatrixscrolldirective>';
                template += '    <table class="excel-table" cellspacing="0" cellpadding="0" id="tblExcelMatricsSecond" >';
                template += '        <thead>';
                template += '            <tr ng-repeat="objcol in objSelectedLogicalRule.objHeaderColumnValues">';
                template += '      <th valign= "top" class="excel-colheader">';
                template += '          <span ng-bind="objSelectedLogicalRule.objHeaderRowTitle"></span>';
                template += '  </th>';
                template += '      <td class="excel-colheader" ng-repeat="obj in objcol track by $index" ng-class="(obj==objSelectedLogicalRule.selectedHeaderValue)?\'excel-rowheader-execution\':\'excel-data\'"  >';
                template += '          <span valign="top" ng-bind="obj"></span>';
                template += '      </td>';
                template += '  </tr > ';
                template += '        </thead>';
                template += '        <tbody>';
                template += '            <tr ng-repeat="objval in objSelectedLogicalRule.objColumnValues" ng-init="cindex = $index">';
                template += '                <td ng-class="(objSelectedLogicalRule.objHeaderRowValues[$index] == objSelectedLogicalRule.objOtherData[0])?\'excel-rowheader-execution\' : \'excel-rowheader\'" valign= "top" >';
                template += '                    <span ng-bind="objSelectedLogicalRule.objHeaderRowValues[$index]"></span>';
                template += '                </td >';
                template += '    <td ng-class="GetExcelMatrixStepSelectedClass(obj, cindex, objSelectedLogicalRule, objval, $index)" valign="top" ng-repeat="obj in objval track by $index">';
                template += '        <span valign="top" ng-bind="obj"></span>';
                template += '    </td>';
                template += '            </tr > ';
                template += '        </tbody>';
                template += '    </table>';
                template += '</div>';
                template += '</div>';
            }
        }
        else {
            template = '<div class="tree" id="dvDecisionTable" >';
            template += '<div>     <table class="dt-table-border" cellspacing="0" cellpadding="0">'; //dont remove this empty div it is needed for design by nikhil
            template += '         <tbody>';
            template += '             <tr ng-show="obj.IsVisiblefterExecution" rowconditiondirectiveforscenarioexecution ng-repeat="obj in objSelectedLogicalRule.Rows  | limitTo : noConditionsDisplayed" items="obj" logicalrule="objSelectedLogicalRule"></tr>';
            template += '         </tbody>';
            template += '     </table></div>';
            template += ' </div>';

        }

        return template;
    };
    return {
        restrict: "E",
        replace: true,
        scope: false,

        link: function (scope, element, attrs) {

            scope.GetExcelMatrixStepSelectedClass = function (obj, index, rule, items, colvalindex) {
                var retVal = 'excel-data';
                if (rule.objOtherData != undefined) {
                    if (rule.objHeaderRowValues[index] == rule.objOtherData[0] || rule.objOtherData[1] == colvalindex) {
                        retVal += " excel-cell-execution";
                    }
                    // this needs to be optimized
                    if (rule.objHeaderRowValues[index] == rule.objOtherData[0]) {
                        var val = items[rule.objOtherData[1]];
                        if (rule.objOtherData[1] == colvalindex) {
                            if (val == obj) {
                                // scope.selectedRecord = obj;
                                retVal = 'excel-data-execution';
                            }
                        }
                    }
                }
                return retVal;
            };
            scope.LoadSomeRowsinExcelMatrix = function () {
                if (scope.objSelectedLogicalRule.objColumnValues) {
                    var startingpostion = scope.objSelectedLogicalRule.objColumnValues.length;
                    count = 0;
                    for (var i = startingpostion; i < scope.objSelectedLogicalRule.objExcelColumnValues.length; i++) {
                        if (count == 5) {
                            break;
                        }
                        else {
                            scope.objSelectedLogicalRule.objColumnValues.push(scope.objSelectedLogicalRule.objExcelColumnValues[i]);
                        }
                        count++;
                    }
                }
            };

            element.html(getTemplatetable(scope.objLogicalRule));
            $compile(element.contents())(scope);
        }
    };
}]);

//#endregion Logical rule  Execution directive