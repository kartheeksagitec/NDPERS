function GetClosestLogicalRule(objLogicalRule, istrEffectiveDate, ruleType) {
    var retVal;
    var blnFound = false;
    if (ruleType == "DecisionTable") {
        if (objLogicalRule.Rules != null && objLogicalRule.Rules.length > 0) {
            angular.forEach(objLogicalRule.Rules, function (value, key) {
                var effDate = value.EffectiveDate;
                if (effDate == istrEffectiveDate) {
                    retVal = value;
                    blnFound = true;
                    return;
                }
            });
            if (!blnFound) {
                angular.forEach(objLogicalRule.Rules, function (value, key) {
                    if (value.EffectiveDate == undefined || value.EffectiveDate == "") {
                        retVal = value;
                        return;
                    }
                });
            }
        }
    }
    else {
        angular.forEach(objLogicalRule.Elements, function (item) {
            if (item.Name == "logicalrule") {
                if (item.dictAttributes.sfwEffectiveDate == istrEffectiveDate) {
                    retVal = item;
                    blnFound = true;
                    return;
                }
            }
        });

        if (!blnFound) {
            angular.forEach(objLogicalRule.Elements, function (item) {
                if (item.Name == "logicalrule") {
                    if (item.dictAttributes.sfwEffectiveDate == undefined || item.dictAttributes.sfwEffectiveDate == "") {
                        retVal = item;
                    }
                    return;
                }
            });
        }
    }

    return retVal;
}

//#region Update Execution Flow Methods

function UpdateTestExecutionFlow(ilstRuleSteps, ruleType, objSelectedLogicalRule, strRunType) {
    if (ruleType == "LogicalRule") {
        CheckAndSelectStepForLogicalRule(ilstRuleSteps, objSelectedLogicalRule, strRunType);
    }
    else if (ruleType == "DecisionTable") {
        CheckAndSelectStepForDecisionTable(ilstRuleSteps, objSelectedLogicalRule);
    }
    else if (ruleType == "ExcelMatrix") {

        prepareExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);
    }
}

//#region For Logical Rule
function CheckAndSelectStepForLogicalRule(ilstRuleSteps, objSelectedLogicalRule, strRunType) {
    angular.forEach(objSelectedLogicalRule.Elements, function (step) {
        if (step.Name != "variables") {
            CheckAndUpdateStep(step, ilstRuleSteps, strRunType);
        }
    });
}

function CheckAndUpdateStep(step, executedSteps, strRunType) {
    step.VisibilityStatus = false;
    step.IsStepSelected = false;

    CheckAndSelectStep(step, executedSteps, strRunType);

    angular.forEach(step.Elements, function (childStep) {
        if (childStep.Name != "items") {
            CheckAndUpdateStep(childStep, executedSteps, strRunType);
        }
    });

    if (step.Children != undefined) {
        angular.forEach(step.Children, function (childStep) {
            if (childStep.Name != "items") {
                CheckAndUpdateStep(childStep, executedSteps, strRunType);
            }
        });
    }

    CheckAndUpdateBottomLineUnSelectedStatus(step.Elements);
    if (step.Children != null) {
        CheckAndUpdateBottomLineUnSelectedStatus(step.Children);
    }

    if (step.Name == "switch") {
        CheckAndUpdateCondtionChildrenLineStatus(step);
    }

}

function CheckAndUpdateBottomLineUnSelectedStatus(itms) {

}

function CheckAndUpdateCondtionChildrenLineStatus(condtionVM) {

}
function getRuleStep(executedSteps, step) {
    var ruleStep;
    for (var i = 0; i < executedSteps.length; i++) {
        if (executedSteps[i].istrNodeID == step.dictAttributes.sfwNodeID) {
            ruleStep = executedSteps[i];
            break;
        }
    }
    if (!ruleStep && step.dictAttributes.sfwCommented == "True" && step.Children && step.Children.length > 0) {
        for (var j = 0; j < step.Children.length; j++) {
            ruleStep = getRuleStep(executedSteps, step.Children[j]);
            if (ruleStep) {
                break;
            }
        }
    }
    return ruleStep;
}
function CheckAndSelectStep(step, executedSteps, strRunType) {
    step.IsStepSelected = false;
    step.isExpanded = false;

    var ruleStep;
    ruleStep = getRuleStep(executedSteps, step);
    step.ActualValue = undefined;

    if (ruleStep != undefined) {
        if (ruleStep.istrNodeID == step.dictAttributes.sfwNodeID) {
            if (ruleStep.iobjValue instanceof Date) {
                step.ActualValue = ruleStep.iobjValue.toString();
            }
            else {
                if (undefined != ruleStep.iobjValue) {
                    step.ActualValue = ruleStep.iobjValue.toString();
                }

                if (ruleStep.iobjValue != undefined) {
                    //Type type = ruleStep.iobjValue.GetType();
                    //if (null != type)
                    //{
                    //    var lst = type.GetGenericArguments();
                    //    if (null != lst && lst.Count() > 0)
                    //    {
                    //        step.ActualValue = GetCollectionEntityActualVale(step["sfwExpression"]);
                    //    }
                    //}
                }
            }

            step.IsErrorOccured = ruleStep.iblnErrorOccurred;

            step.IsStepSelected = true;
            step.isExpanded = true;

            CheckAndUpdateCondtionStepsStatus(step);

            step.VisibilityStatus = true;

            if (strRunType == "AllRuleSteps" && (step.Name == "calllogicalrule" || step.Name == "calldecisiontable" || step.Name == "callexcelmatrix")) {
                step.TestExeRuleObject = ruleStep;
                step.OpenExecutionWindow = true;
            }
            else if (step.Name == "foreach" || step.Name == "while") {
                step.CanExpandOrCollapse = false;
                step.TestExeRuleObject = ruleStep;
                UpdateLoopExecutionCountForLoop(step.TestExeRuleObject, step);
            }
        } else {
            step.IsStepSelected = true;
            step.isExpanded = true;
            step.VisibilityStatus = true;
        }

    } else {
        step.isExpanded = false;
    }
}

function CheckAndUpdateCondtionStepsStatus(step) {

}

function UpdateLoopExecutionCountForLoop(TestExeRuleObject, step) {
    if (TestExeRuleObject != undefined) {
        var executionCount = 0;
        angular.forEach(TestExeRuleObject.ilstSteps, function (ruleObj) {
            if (ruleObj.length > 0) {
                var ruleStep;
                angular.forEach(ruleObj, function (item) {
                    if (item.istrNodeID == step.dictAttributes.sfwNodeID) {
                        ruleStep = item;
                        return;
                    }
                });

                if (ruleStep != undefined) {
                    executionCount++;
                }
            }
        });

        step.LoopExecutionCount = executionCount;
    }
}

function GetCollectionEntityActualVale(Expression) {

}

//#endregion

//#region For Decision Table
function CheckAndSelectStepForDecisionTable(ilstRuleSteps, objSelectedLogicalRule) {
    if (objSelectedLogicalRule != undefined) {
        angular.forEach(objSelectedLogicalRule.Rows, function (row) {
            row.IsVisiblefterExecution = true;
            angular.forEach(row.Cells, function (step) {
                CheckAndUpdateStepForDecisionTable(step.Item, ilstRuleSteps, objSelectedLogicalRule);
            });
        });

        angular.forEach(objSelectedLogicalRule.DataHeaders, function (step) {
            CheckAndUpdateStepForDecisionTable(step, ilstRuleSteps, objSelectedLogicalRule);
        });
    }
}

function CheckAndUpdateStepForDecisionTable(step, ilstRuleSteps, objSelectedLogicalRule) {
    var ruleStep;
    var lst = ilstRuleSteps.filter(function (x) { return x.istrNodeID == step.NodeID; });
    if (lst && lst.length > 0) {
        ruleStep = lst[0];
    }

    step.ActualValue = undefined;
    if (ruleStep != undefined) {

        if (undefined != ruleStep.iobjValue) {
            step.ActualValue = ruleStep.iobjValue.toString();
        }


        step.IsErrorOccured = ruleStep.iblnErrorOccurred;
        step.IsStepSelected = true;

        if (step.ItemType == 'assign') {
            //Discussed with Nayan, we are not highlighting related cell now, as it introducing some more issues.
            //UpdateRelatedStepBackground(step, objSelectedLogicalRule);
        }
        //step.IsRelatedStepAfterExecution = false;
    }
    else if (step.ItemType == "rowheader") {
        step.IsStepSelected = true;
    }
}

function UpdateRelatedStepBackground(step, objSelectedLogicalRule) {

    var indexrow = -1;
    var indexcell = -1;
    angular.forEach(objSelectedLogicalRule.Rows, function (row) {
        angular.forEach(row.Cells, function (item) {
            if (item.Item.NodeID == step.NodeID) {
                indexrow = objSelectedLogicalRule.Rows.indexOf(row);
                indexcell = row.Cells.indexOf(item);
                return;
            }
        });
        if (indexrow >= 0) {
            return;
        }
    });

    if (indexrow >= 0 && indexcell >= 0) {
        for (i = 0; i < indexcell; i++) {
            var item = objSelectedLogicalRule.Rows[indexrow].Cells[i];
            if (item.Item.ItemType == "assign") {
                item.Item.IsRelatedStepAfterExecution = true;
            }
        }
    }
}

//#endregion

//#region For Excel Matrix
function CheckAndSelectStepForExcelMatrix(ilstRuleSteps, objSelectedLogicalRule) {

    if (objSelectedLogicalRule.Elements != undefined && objSelectedLogicalRule.Elements.length > 0) {
        angular.forEach(objSelectedLogicalRule.Elements, function (item) {
            if (item.Name.toString().toLowerCase() == "rows") {
                for (var row = 0; row < item.Elements.length; row++) {
                    angular.forEach(ilstRuleSteps, function (ruleStep) {
                        if (item.Elements[row].dictAttributes.sfwNodeID == ruleStep.istrNodeID) {
                            item.IsStepSelected = true;
                            item.Elements[row].OtherData = ruleStep.iobjData;
                            return;
                        }
                    });
                }
            }
        });
    }

}

function prepareExcelMatrix(ilstRuleSteps, objSelectedLogicalRule) {
    objSelectedLogicalRule.objColumnValues = [];
    objSelectedLogicalRule.objExcelColumnValues = [];
    objSelectedLogicalRule.objHeaderRowValues = [];
    objSelectedLogicalRule.objHeaderColumnValues = [];
    objSelectedLogicalRule.objHeaderRowTitle = "";
    objSelectedLogicalRule.objRows = [];

    var objExcelMatrixRule = objSelectedLogicalRule;
    for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
        if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
            var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

            for (var row = 0; row < objExcelMatrixRows.Elements.length; row++) {
                if (objExcelMatrixRows.Elements[row].dictAttributes.ID == 0) {
                    var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;

                    if (objExcelMatrixRows.Elements[row].dictAttributes.Value != undefined) {
                        objSelectedLogicalRule.objHeaderRowTitle = objExcelMatrixRows.Elements[row].dictAttributes.Value;
                    }


                    var columns = [];
                    columns = strColumnValues.split(objSelectedLogicalRule.dictAttributes.sfwDelimiter);

                    objSelectedLogicalRule.objHeaderColumnValues.push(columns);
                }
                else {
                    var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;
                    var columns = [];
                    columns = strColumnValues.split(objSelectedLogicalRule.dictAttributes.sfwDelimiter);
                    //objSelectedLogicalRule.objColumnValues.push(columns);
                    objSelectedLogicalRule.objExcelColumnValues.push(columns);
                    objSelectedLogicalRule.objHeaderRowValues.push(objExcelMatrixRows.Elements[row].dictAttributes.Value);
                }
            }
        }
    }
    var index = CheckAndSelectStepForExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);

    var selectedcellrowindex = 0;
    if (ilstRuleSteps) {
        for (var j = 0; j < ilstRuleSteps.length; j++) {
            if (ilstRuleSteps[j].istrNodeName == "row") {
                for (var i = 0; i < objSelectedLogicalRule.objHeaderRowValues.length; i++) {
                    if (objSelectedLogicalRule.objHeaderRowValues[i] == ilstRuleSteps[j].iobjData[0]) {
                        selectedcellrowindex = i;
                        objSelectedLogicalRule.selectedHeaderValue = ilstRuleSteps[j].iobjData[1];
                    }
                }
            }
        }
    }

    if (objSelectedLogicalRule.objExcelColumnValues.length > 25) {
        for (var i = 0; i < 25; i++) {
            objSelectedLogicalRule.objColumnValues.push(objSelectedLogicalRule.objExcelColumnValues[i]);
        }
    }
    else {
        objSelectedLogicalRule.objColumnValues = objSelectedLogicalRule.objExcelColumnValues;
    }

    for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
        if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
            var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

            for (var index = 0; index < objExcelMatrixRows.Elements.length; index++) {
                if (objExcelMatrixRows.Elements[index].OtherData != undefined) {
                    var columnindex = -1;
                    angular.forEach(objSelectedLogicalRule.objHeaderColumnValues[0], function (item) {
                        if (item == objExcelMatrixRows.Elements[index].OtherData[1]) {
                            columnindex = objSelectedLogicalRule.objHeaderColumnValues[0].indexOf(item);
                            return;
                        }
                    });
                    objSelectedLogicalRule.objOtherData = [objExcelMatrixRows.Elements[index].OtherData[0], columnindex];
                }
            }
        }
    }
    //var scope = GetCurrentScopeObject("Scenario");
    //scope.objSelectedLogicalRule1 = objSelectedLogicalRule;
}


//#endregion

//#endregion

function GetExpressionValue(curVM, expression) {
    var retVal = '';

    if (expression !== undefined && expression !== '') {
        if (expression.contains(".")) {
            retVal = CheckAndGetExpressionValue(curVM, expression);
        }
        else {
            retVal = expression;
        }
    }
    return retVal;
}

function CheckAndGetExpressionValue(curVM, loopCollectionName) {
    var retVal = '';
    var loopHierarchyList = loopCollectionName.split('.');
    if (loopHierarchyList !== undefined && loopHierarchyList.length != undefined) {
        for (var ind = 0; ind < loopHierarchyList.length; ind++) {
            var collectionName = GetCollectionName(curVM, loopHierarchyList[ind]);
            if (loopHierarchyList[ind] !== collectionName) {
                var fullPath = CheckAndGetExpressionValue(curVM, collectionName);
                if (fullPath !== undefined && fullPath != '') {
                    retVal += fullPath + ".";
                }
            }
            else {
                retVal += collectionName + ".";
            }
        }
    }

    retVal = retVal.substring(0, retVal.length - 1);
    return retVal;
}

function GetCollectionName(curVM, loopItmName) {
    var retVal = loopItmName;
    var parentLoopViewModel = GetLoopParent(curVM);
    while (parentLoopViewModel !== undefined) {
        if (parentLoopViewModel !== undefined) {
            if (parentLoopViewModel.dictAttributes.sfwObjectID == loopItmName || parentLoopViewModel.dictAttributes.sfwItemName == loopItmName) {
                retVal = parentLoopViewModel.dictAttributes.sfwObjectID;
                break;
            }
        }
        parentLoopViewModel = GetLoopParent(parentLoopViewModel);
    }
    return retVal;
}

function GetLoopParent(curStepViewModel) {
    var loopViewModel;
    if (curStepViewModel != undefined) {
        var parentObj = curStepViewModel.ParentVM;
        while (parentObj !== undefined) {
            if (parentObj.Name == "foreach") {
                loopViewModel = parentObj;
                break;
            }
            parentObj = parentObj.ParentVM;
        }
    }

    return loopViewModel;
}

function GetLoopParameter(logicalRuleModel, collectionName) {
    var loopParameter;
    if (collectionName !== undefined && collectionName !== "") {
        if (collectionName.contains(".")) {
            angular.forEach(collectionName.split('.'), function (name) {
                if (loopParameter == undefined) {
                    var parameters;
                    angular.forEach(logicalRuleModel.Elements, function (item) {
                        if (item.Name == "parameters") {
                            parameters = item;
                        }
                    });

                    if (parameters !== undefined) {
                        loopParameter = GetLoopParameter1(parameters.Elements, name);
                    }
                }
                else {
                    if (loopParameter !== undefined) {
                        loopParameter = GetLoopParameter1(loopParameter.Elements, name);
                    }
                }
            });
        }
        else {
            var parameters;
            angular.forEach(logicalRuleModel.Elements, function (item) {
                if (item.Name == "parameters") {
                    parameters = item;
                }
            });

            if (parameters !== undefined) {
                loopParameter = GetLoopParameter1(parameters.Elements, name);
            }
        }
    }
    return loopParameter;
}

function GetLoopParameter1(parameters, name) {
    var parameterModel;
    angular.forEach(parameters, function (objparameterVar) {
        var strDataType = objparameterVar.dictAttributes.sfwDataType;
        var id = objparameterVar.dictAttributes.ID;
        if (id == name && (strDataType == "Collection" || strDataType == "CDOCollection" || strDataType == "List")) {
            parameterModel = objparameterVar;
            return;
        }
    });

    return parameterModel;
}

//#region Common Method for Collection and Object

function GetEntityFieldsFromRuleDataInfo(objField, objEntity) {
    var lst = [];

    if (objField.ruleDataInfo != undefined) {
        if (objField.ruleDataInfo.ilstEntityFields != undefined) {
            lst = objField.ruleDataInfo.ilstEntityFields;
        }
        else if (objField.dictAttributes.ParameterType == "InputParameter") {
            lst = objField.ruleDataInfo.ilstInputFields;
        }
        else if (objField.dictAttributes.ParameterType == "OutputParameter") {
            lst = objField.ruleDataInfo.ilstOutputFields;
        }
        if (lst == undefined) {
            lst = [];
        }
    }
    return lst;
}

//#region Common Methods For Open popups  for collection and Object

function AddColumnForCollection(SelectedTestField, newScope, $scope, $rootScope) {

    if (SelectedTestField.ruleDataInfo != undefined) {
        var EntityID = SelectedTestField.ruleDataInfo.istrEntity;

        if (EntityID != undefined && EntityID != "") {
            var attributes = $rootScope.getEntityAttributeIntellisense(EntityID, true);
            var strAttributes = JSON.stringify(attributes);
            attributes = JSON.parse(strAttributes);
            newScope.lstEntityFieldsCollection = [];
            $rootScope.UndRedoBulkOp("Start");
            angular.forEach(attributes, function (item) {

                var lstEntityfields = GetEntityFieldsFromRuleDataInfo(SelectedTestField, undefined);

                if (!this.IsFieldExist(lstEntityfields, item.ID, SelectedTestField.ExtraFields)) {
                    item.IsSelected = false;
                    item.EntityId = EntityID;
                    $rootScope.PushItem(item, newScope.lstEntityFieldsCollection);
                }
            });
            $rootScope.UndRedoBulkOp("End");
            var newColumnScope = $scope.$new();
            newColumnScope.lstEntityFieldsCollection = newScope.lstEntityFieldsCollection;
            newColumnScope.SelectAllEntityFields = function () {
                SelectAllEntityFields(newColumnScope.lstEntityFieldsCollection);
            };
            newColumnScope.ClearAllEntityFields = function () {
                ClearAllEntityFields(newColumnScope.lstEntityFieldsCollection);
            };
            newColumnScope.OnOKClickEntityFields = function () {
                OnOKClickEntityFields(newScope.SelectedTestField, newColumnScope.lstEntityFieldsCollection, newColumnScope, $rootScope);
            };
            newColumnScope.OnCloseClickEntityFields = function () {
                OnCloseClickEntityFields(newColumnScope);
            };
            newColumnScope.searchColumnsForCollection = "";
            newColumnScope.AddColumnDialog = $rootScope.showDialog(newColumnScope, "Entity Fields", "Scenario/views/AddColumnsForCollection.html", { height: 500 });
        }
    }
}

function SelectAllEntityFields(lstEntityFieldsCollection) {
    if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
        SelectAndClearEntityField(lstEntityFieldsCollection, true);
    }
}

function ClearAllEntityFields(lstEntityFieldsCollection) {
    if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
        SelectAndClearEntityField(lstEntityFieldsCollection, false);
    }
}

function OnOKClickEntityFields(SelectedTestField, lstEntityFieldsCollection, newColumnScope, $rootScope) {

    if (SelectedTestField != undefined) {
        $rootScope.UndRedoBulkOp("Start");
        if (SelectedTestField.dictAttributes.sfwDataType == "Object") {

            if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
                angular.forEach(lstEntityFieldsCollection, function (item) {
                    if (item.IsSelected) {
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        if (SelectedTestField.ExtraFields == undefined) {
                            SelectedTestField.ExtraFields = [];
                        }
                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);

                        var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'Input', isExtraEntityField: 'true', ID: item.ID, sfwDataType: item.DataType, istrEntity: item.Entity }, Elements: [] };
                        testParameter.ruleDataInfo = entityFieldInfo;
                        CheckAndAddObjectFields(testParameter, SelectedTestField.objEntity, $rootScope);

                        if (SelectedTestField.objFieldsVM == undefined) {
                            SelectedTestField.objFieldsVM = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                            $rootScope.PushItem(SelectedTestField.objFieldsVM, SelectedTestField.Elements);
                        }
                        if (!SelectedTestField.objFieldsVM.Elements.some(function (itm) { return itm.dictAttributes.ID == item.ID })) {
                            $rootScope.PushItem(testParameter, SelectedTestField.objFieldsVM.Elements);
                        }
                    }
                });
            }
        }
        else {
            var extraColumns = [];
            if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
                angular.forEach(lstEntityFieldsCollection, function (item) {
                    if (item.IsSelected) {
                        extraColumns.push(item);
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        if (SelectedTestField.ExtraFields == undefined) {
                            SelectedTestField.ExtraFields = [];
                        }
                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);

                        if (SelectedTestField.IsOutput) {
                            var index = 1;
                            for (var i = 1; i < SelectedTestField.lstCollectionColumn.length; i++) {
                                if (SelectedTestField.lstCollectionColumn[i].istrName == "Ignore") {
                                    index = i;
                                    break;
                                }
                            }

                            if (index > 0) {
                                $rootScope.InsertItem(entityFieldInfo, SelectedTestField.lstCollectionColumn, index);
                            }
                        }
                        else {
                            $rootScope.PushItem(entityFieldInfo, SelectedTestField.lstCollectionColumn);
                        }
                    }
                });
            }
            if (extraColumns != undefined && extraColumns.length > 0) {
                angular.forEach(extraColumns, function (item) {
                    angular.forEach(SelectedTestField.Elements, function (objFields) {
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'Input', ID: item.ID, sfwDataType: item.DataType, isExtraEntityField: "true", istrEntity: item.Entity }, Elements: [] };
                        testParameter.ruleDataInfo = entityFieldInfo;
                        CheckAndAddObjectFields(testParameter, SelectedTestField.objEntity, $rootScope);
                        $rootScope.PushItem(testParameter, objFields.Elements);
                        //objFields.Elements.push(testParameter);
                    });
                });
            }
        }
        $rootScope.UndRedoBulkOp("End");
        OnCloseClickEntityFields(newColumnScope);
    }
}

function OnCloseClickEntityFields(newColumnScope) {
    if (newColumnScope.AddColumnDialog != undefined) {
        newColumnScope.AddColumnDialog.close();
    }
}

//#endregion Common Methods For Open popups  for collection and Object

//#region Methods for add ,delete collection and object field

function UpdateSelectedCollectionField(objFields, SelectedTestField) {
    SelectedTestField.SelectedFieldsVM = objFields;
}

function DeleteCollectionField(SelectedTestField, $rootScope) {
    if (SelectedTestField != undefined && SelectedTestField.SelectedFieldsVM != undefined) {
        var index = SelectedTestField.Elements.indexOf(SelectedTestField.SelectedFieldsVM);
        if (index >= 0) {
            $rootScope.DeleteItem(SelectedTestField.SelectedFieldsVM, SelectedTestField.Elements);
            if (index < SelectedTestField.Elements.length) {
                SelectedTestField.SelectedFieldsVM = SelectedTestField.Elements[index];
            }
            else if (SelectedTestField.Elements.length > 0) {
                SelectedTestField.SelectedFieldsVM = SelectedTestField.Elements[index - 1];
            }
        }
    }
}

function AddCollectionField(SelectedTestField, objEntity, $rootScope) {
    $rootScope.UndRedoBulkOp("Start");
    if (SelectedTestField != undefined) {
        var objFields = { Name: "fields", Value: '', dictAttributes: { ParameterType: SelectedTestField.dictAttributes.ParameterType }, Elements: [] };
        var blnAdd = AddParameter(objFields, SelectedTestField, objEntity, $rootScope);
        if (blnAdd) {
            $rootScope.PushItem(objFields, SelectedTestField.Elements);
            SelectedTestField.SelectedFieldsVM = objFields;
        }
    }
    $rootScope.UndRedoBulkOp("End");
}

function UpdateSelectedObjectField(objFields, SelectedTestField) {
    SelectedTestField.objFieldsVM.SelectedField = objFields;
}

function DeleteObjectField(SelectedTestField, $rootScope) {
    if (SelectedTestField.objFieldsVM != undefined && SelectedTestField.objFieldsVM.SelectedField != undefined) {
        var index = SelectedTestField.objFieldsVM.Elements.indexOf(SelectedTestField.objFieldsVM.SelectedField);
        if (index >= 0) {
            if (SelectedTestField.objFieldsVM.SelectedField.dictAttributes.isExtraEntityField == "true") {
                for (var i = 0; i < SelectedTestField.ExtraFields.length; i++) {
                    if (SelectedTestField.ExtraFields[i].istrName == SelectedTestField.objFieldsVM.SelectedField.dictAttributes.ID) {
                        SelectedTestField.ExtraFields.splice(i, 1);
                    }
                }
                $rootScope.DeleteItem(SelectedTestField.objFieldsVM.SelectedField, SelectedTestField.objFieldsVM.Elements);
                if (index < SelectedTestField.objFieldsVM.Elements.length) {
                    SelectedTestField.objFieldsVM.SelectedField = SelectedTestField.objFieldsVM.Elements[index];
                }
                else if (SelectedTestField.Elements.length > 0) {
                    SelectedTestField.objFieldsVM.SelectedField = SelectedTestField.objFieldsVM.Elements[index - 1];
                }
            }
            else {
                alert('Selected field cannot be deleted as it is a mandatory field.');
            }
        }
    }
}

function DeleteColumnFromCollection(aSelectedTestField, aobjcol) {
    if (aSelectedTestField != undefined) {
        if (aobjcol != undefined) {
            var blnFound = false;
            for (var i = 0; i < aSelectedTestField.ExtraFields.length; i++) {
                if (aSelectedTestField.ExtraFields[i].istrName == aobjcol.istrName) {
                    blnFound = true;
                    aSelectedTestField.ExtraFields.splice(i, 1);
                    for (var j = 0; j < aSelectedTestField.lstCollectionColumn.length; j++) {
                        if (aSelectedTestField.lstCollectionColumn[j].istrName == aobjcol.istrName) {
                            aSelectedTestField.lstCollectionColumn.splice(j, 1);
                            break;
                        }
                    }

                    var fieldsCount = -1;
                    if (aSelectedTestField.Elements && aSelectedTestField.Elements.length > 0) {
                        fieldsCount = aSelectedTestField.Elements.length - 1;
                    }
                    while (fieldsCount > -1) {
                        var fields = aSelectedTestField.Elements[fieldsCount];
                        angular.forEach(fields.Elements, function (field) {
                            if (field.dictAttributes.ID == aobjcol.istrName) {
                                fields.Elements.splice(fields.Elements.indexOf(field), 1);
                                if (!fields.Elements.length) {
                                    aSelectedTestField.Elements.splice(fieldsCount, 1)
                                }
                            }
                        });
                        fieldsCount--;
                    }
                    break;
                }
            }
            if (!blnFound) {
                alert('Selected field cannot be deleted as it is a mandatory field.');
            }
        }
    }
}


//#endregion Methods for add ,delete collection and object field

function AddParameter(objFields, SelectedTestField, objEntity, $rootScope) {
    var blnAdd = false;
    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(SelectedTestField, objEntity);
    if (lstEntityFields != undefined) {
        angular.forEach(lstEntityFields, function (item) {

            if (!objFields.Elements.some(function (itm) { return itm.dictAttributes.ID == item.istrName })) {
                var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: objFields.dictAttributes.ParameterType, ID: item.istrName, sfwDataType: item.istrDataType }, Elements: [] };
                testParameter.ruleDataInfo = item;
                CheckAndAddObjectFields(testParameter, objEntity, $rootScope);
                $rootScope.PushItem(testParameter, objFields.Elements);
                blnAdd = true;
            }
        });
    }
    if (SelectedTestField.ExtraFields != undefined) {
        angular.forEach(SelectedTestField.ExtraFields, function (item) {
            if (!objFields.Elements.some(function (itm) { return itm.dictAttributes.ID == item.istrName })) {
                var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'InputParameter', ID: item.istrName, sfwDataType: item.istrDataType, isExtraEntityField: 'true', istrEntity: item.istrEntity }, Elements: [] };
                testParameter.ruleDataInfo = item;
                CheckAndAddObjectFields(testParameter, objEntity, $rootScope);
                $rootScope.PushItem(testParameter, objFields.Elements);
                blnAdd = true;
            }
        });
    }
    return blnAdd;
}

function CheckAndAddObjectFields(testParameter, objEntity, $rootScope) {
    if (testParameter.dictAttributes.sfwDataType == "Object") {
        var lstEntityFields = GetEntityFieldsFromRuleDataInfo(testParameter, objEntity);
        if (lstEntityFields != undefined && lstEntityFields.length > 0) {
            var objFields = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
            angular.forEach(lstEntityFields, function (item) {
                var testField = { Name: "field", Value: '', dictAttributes: { ParameterType: 'InputParameter', ID: item.istrName, sfwDataType: item.istrDataType }, Elements: [] };
                testField.ruleDataInfo = item;
                CheckAndAddObjectFields(testField, objEntity, $rootScope);
                $rootScope.PushItem(testField, objFields.Elements);
            });
            $rootScope.PushItem(objFields, testParameter.Elements);
        }
    }
}

function IsFieldExist(lstEntityfields, strID, ExtraFields) {
    var blnFound = false;
    angular.forEach(lstEntityfields, function (item) {
        if (item.istrName == strID) {
            blnFound = true;
        }
    });
    if (!blnFound) {
        angular.forEach(ExtraFields, function (item) {
            if (item.istrName == strID) {
                blnFound = true;
            }
        });
    }
    return blnFound;
}

function SelectAndClearEntityField(lstEntityField, isSelected) {
    angular.forEach(lstEntityField, function (item) {
        item.IsSelected = isSelected;
    });
}

//#endregion Common Method for Collection and Object


//#region Refresh Sceanrio Methods
function ReloadTestParameterModel(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope) {
    if (objRuleDataInfo) {
        LoadTestParameterModel(testParameterModel, objRuleDataInfo, objEntity, $rootScope, true);
    }
    else {
        testParameterModel.dictAttributes.ID = curTestParameter.dictAttributes.ID;
        testParameterModel.dictAttributes.sfwDataType = curTestParameter.dictAttributes.sfwDataType;
        testParameterModel.dictAttributes.Value = curTestParameter.dictAttributes.Value;
    }
    testParameterModel.dictAttributes.isExtraEntityField = curTestParameter.dictAttributes.isExtraEntityField;
    testParameterModel.dictAttributes.istrEntity = curTestParameter.dictAttributes.istrEntity;

    testParameterModel.dictAttributes.sfwIgnore = curTestParameter.dictAttributes.sfwIgnore;

    CheckAndUpdateInputOrExpectedValue(testParameterModel, curTestParameter);
    testParameterModel.dictAttributes.sfwIsconstant = curTestParameter.dictAttributes.sfwIsconstant;
    testParameterModel.ruleDataInfo = objRuleDataInfo;
    CheckAndLoadChildParameters(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope);

}

function LoadTestParameterModel(testParameterModel, objRuleDataInfo, objEntity, $rootScope, ignoreObject) {
    if (objRuleDataInfo) {
        testParameterModel.dictAttributes.ID = objRuleDataInfo.istrName;
        testParameterModel.dictAttributes.sfwDataType = objRuleDataInfo.istrDataType;
        testParameterModel.ruleDataInfo = objRuleDataInfo;
        if (!ignoreObject) {
            CheckAndAddObjectFields(testParameterModel, objEntity, $rootScope);
        }
    }
}

function CheckAndUpdateInputOrExpectedValue(testParameterModel, curTestParameter) {
    if (curTestParameter) {
        testParameterModel.dictAttributes.Value = curTestParameter.dictAttributes.Value;
    }
}


function CheckAndLoadChildParameters(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope) {
    if (curTestParameter && objRuleDataInfo) {
        var ilstEntityFields = GetEntityFieldsFromRuleDataInfo(testParameterModel, objEntity);

        if (testParameterModel.dictAttributes.sfwDataType == "Collection" || testParameterModel.dictAttributes.sfwDataType == "List") {
            angular.forEach(curTestParameter.Elements, function (step) {
                var curChildParametersModel = step;


                var childParametersModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };//new TestParametersModel(this, this.objRuleDataInfo, this.strType);
                childParametersModel.dictAttributes.ParameterType = testParameterModel.dictAttributes.ParameterType;
                childParametersModel.ruleDataInfo = objRuleDataInfo;

                ReloadParameters(curChildParametersModel, childParametersModel, objRuleDataInfo, objEntity, $rootScope)
                    testParameterModel.Elements.push(childParametersModel);
                
            });
        }
        else if (testParameterModel && testParameterModel.dictAttributes.sfwDataType == "Object") {
            angular.forEach(ilstEntityFields, function (fld) {
                var testFieldsModel = null;
                testFieldsModels = testParameterModel.Elements.filter(function (x) { return x.Name == "fields"; });
                if (!testFieldsModels || (testFieldsModels && testFieldsModels.length == 0)) {
                    testFieldsModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                    testParameterModel.Elements.push(testFieldsModel);
                }
                else {
                    testFieldsModel = testFieldsModels[0];
                }

                var testParameter = { Name: "field", Value: '', dictAttributes: {}, Elements: [] };
                testParameter.dictAttributes.ParameterType = "Input";

                if (testFieldsModel) {
                    testFieldsModel.Elements.push(testParameter);
                }

                var fieldsVM = curTestParameter.Elements.filter(function (x) { return x.Name == "fields"; });
                var curChildTestParameter = null;
                if (fieldsVM && fieldsVM.length > 0) {
                    var lst = fieldsVM[0].Elements.filter(function (itm) { return itm.dictAttributes.ID == fld.istrName; });
                    if (lst && lst.length > 0) {
                        curChildTestParameter = lst[0];
                    }
                }

                if (curChildTestParameter && curChildTestParameter.Name == "field") {
                    ReloadTestParameterModel(testParameter, curChildTestParameter, fld, objEntity, $rootScope);
                }
                else {
                    LoadTestParameterModel(testParameter, fld, objEntity, $rootScope);
                }
            });

            //for extra Entity Feilds
            angular.forEach(curTestParameter.Elements, function (flds) {
                if (flds.Name == "fields") {
                    var testFieldsModel;

                    testFieldsModels = testParameterModel.Elements.filter(function (x) { return x.Name == "fields"; });
                    if (!testFieldsModels || (testFieldsModels && testFieldsModels.length == 0)) {
                        testFieldsModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                        testParameterModel.Elements.push(testFieldsModel);
                    }
                    else {
                        testFieldsModel = testFieldsModels[0];
                    }
                    angular.forEach(flds.Elements, function (fld) {
                        if (!getDescendents(testParameterModel).some(function (x) { return x.dictAttributes.ID == fld.dictAttributes.ID; })) {
                            if ('isExtraEntityField' in fld.dictAttributes && fld.dictAttributes.isExtraEntityField.toLowerCase() == "true") {
                                var ruleDataInfo = {};
                                ruleDataInfo.istrName = fld.dictAttributes.ID;
                                ruleDataInfo.istrDataType = fld.dictAttributes.sfwDataType;
                                var testParamModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                                testParamModel.dictAttributes.ParameterType = "Input";
                                ReloadTestParameterModel(testParamModel, fld, ruleDataInfo, objEntity, $rootScope);
                                testFieldsModel.Elements.push(testParamModel);
                            }
                        }
                    });
                }
            });
        }
    }
}


function ReloadParameters(curChildParametersVM, childParametersModel, objRuleDataInfo, objEntity, $rootScope) {
    childParametersModel.dictAttributes.indexNumber = curChildParametersVM.dictAttributes.indexNumber;
    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(childParametersModel, objEntity);
    if (lstEntityFields) {
        angular.forEach(lstEntityFields, function (entityFeildInfo) {

            LoadField(childParametersModel, curChildParametersVM, entityFeildInfo, objEntity, $rootScope);
           
        });

    }
    angular.forEach(curChildParametersVM.Elements, function (aobjextrafield) {
        if (aobjextrafield.dictAttributes.isExtraEntityField && aobjextrafield.dictAttributes.isExtraEntityField == "true") {
            var curChildTestParameter = childParametersModel.Elements.filter(function (itm) { return itm.dictAttributes.ID == aobjextrafield.dictAttributes.ID; });
            if (curChildTestParameter && curChildTestParameter.length > 0) {
            }
            else {
                childParametersModel.Elements.push(aobjextrafield);
            }
        }
    });
}

function LoadField(childParametersModel, curChildParametersVM, entityFeildInfo, objEntity, $rootScope) {
    // TestParameterModel testParameter = new TestParameterModel(this, entityFeildInfo, ParameterType.Input, null);
    if (childParametersModel && !childParametersModel.Elements.some(function (itm) { return itm.dictAttributes.ID == entityFeildInfo.istrName; })) {
        var testParameter = { Name: "field", Value: '', dictAttributes: {}, Elements: [] };
        testParameter.dictAttributes.ParameterType = "Input";

        var curChildTestParameter = curChildParametersVM.Elements.filter(function (itm) { return itm.dictAttributes.ID == entityFeildInfo.istrName; });

        if (curChildTestParameter && curChildTestParameter.length > 0 && curChildTestParameter[0].Name == "field") {
            ReloadTestParameterModel(testParameter, curChildTestParameter[0], entityFeildInfo, objEntity, $rootScope);
        }
        else {
            LoadTestParameterModel(testParameter, entityFeildInfo, objEntity, $rootScope);
        }

        childParametersModel.Elements.push(testParameter);
    }
}

//#endregion