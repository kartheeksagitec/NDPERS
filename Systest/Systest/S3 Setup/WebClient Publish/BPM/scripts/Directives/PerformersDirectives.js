function bpmLanePerformersPropertiesController($filter) {
    var extensionElements = $filter('filter')(this.shapeModel.Elements, { Name: "extensionElements" });
    if (extensionElements && extensionElements.length > 0) {
        this.extensionElementModel = extensionElements[0];
    }
    var userRoleModel = $filter('filter')(this.extensionElementModel.Elements, { Name: "userrole" })[0];
    if (userRoleModel == null || userRoleModel == undefined) {
        userRoleModel = { prefix: "sbpmn", Name: "userrole", dictAttributes: { roles: "" }, IsValueInCDATAFormat: false, Elements: [], Children: [] };
        this.extensionElementModel.Elements.push(userRoleModel);
    }
    else {
        if (userRoleModel.dictAttributes.roles == undefined) {
            userRoleModel.dictAttributes.roles = "";
        }
    }
    this.userRoleModel = userRoleModel;
}


function bpmUserTaskPerformersProperties($filter, $getQueryparam, $scope, $rootScope) {
    //newScope.TargetSiteList = $scope.TargetSiteList;
    //newScope.lstbpmnrelatedcodeValues = $scope.lstBPMNRelatedCodeValues;
    var ctrl = this;
    ctrl.queryParametersValues = $getQueryparam.getMapVariableIds(ctrl.mapvariablesmodel.Elements);
    var extrasettingsModel = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extraSettings" });
    if (extrasettingsModel && extrasettingsModel.length > 0) {
        ctrl.ExtraSettingsModel = extrasettingsModel[0];
    }
    if (ctrl.ExtraSettingsModel && !ctrl.ExtraSettingsModel.dictAttributes.sfwCanCompleteFromLeftPanel) {
        ctrl.ExtraSettingsModel.dictAttributes.sfwCanCompleteFromLeftPanel = "False";
    }
    var extensionElements = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extensionElements" });
    if (extensionElements && extensionElements.length > 0) {
        ctrl.extensionElementModel = extensionElements[0];
    }
    var performersModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "performers" });
    if (performersModel && performersModel.length > 0) {
        ctrl.PerformersModel = performersModel[0];
        ctrl.PerformersModels = performersModel;
    }
    if (ctrl.PerformersModel) {
        var defaultConditions = $filter('filter')(ctrl.PerformersModel.Elements, { Name: "defaultcondition" });
        if (defaultConditions && defaultConditions.length > 0) {
            ctrl.defaultConditionModel = defaultConditions[0];
            ctrl.defaultConditionModels = defaultConditions;
        }
    }
    if (ctrl.PerformersModel && ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter == undefined) {
        ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter = "False";
    }
    ctrl.AssignmentOptions = ["By Load", "Everyone", "Sequential"];
    //newScope.FollowAssignmentOptions = ["True", "False"];
    ctrl.ObjConditonAndExternaltask = { IsPreCondition: false, IsExternaltask: false };
    if (ctrl.PerformersModel && ctrl.PerformersModel.Elements[0].Name == "preconditions") {
        ctrl.ObjConditonAndExternaltask.IsPreCondition = true;
        ctrl.objPreconditonsModel = ctrl.PerformersModel.Elements[0];
        ctrl.objPreconditonModel = ctrl.objPreconditonsModel.Elements[0];
    }
    var externalTaskModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "externalTask" });
    if (externalTaskModel && externalTaskModel.length > 0) {
        ctrl.ObjConditonAndExternaltask.IsExternaltask = true;
        ctrl.externalTaskModel = externalTaskModel[0];
    }

    ctrl.onChangeConstant = function () {
        ctrl.PerformersModel.dictAttributes.sfwFollowAssignment = "";
        ctrl.populateFollowAssignmentValues();
    };
    ctrl.populateFollowAssignmentValues = function () {
        ctrl.FollowAssignmentOptions = [];
        if (ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter && ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter != "False") {
            var booleanVariables = ctrl.mapvariablesmodel.Elements.filter(function (item) { return item.dictAttributes.dataType == "bool"; });
            if (booleanVariables && booleanVariables.length > 0) {
                ctrl.FollowAssignmentOptions = booleanVariables.map(function (x) { return x.dictAttributes.id; });
            }
        }
        else {
            ctrl.FollowAssignmentOptions = ["True", "False"];
        }
    };

    ctrl.setPreConditionOrDefaultCondition = function () {
        var newScope = $scope.$new();
        newScope.obj = {};
        var performersModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "performers" });
        if (performersModel && performersModel.length > 0) {
            ctrl.PerformersModel = performersModel[0];
            ctrl.PerformersModels = performersModel;
        }
        if (ctrl.PerformersModel) {
            var defaultConditions = $filter('filter')(ctrl.PerformersModel.Elements, { Name: "defaultcondition" });
            if (defaultConditions && defaultConditions.length > 0) {
                ctrl.defaultConditionModel = defaultConditions[0];
                ctrl.defaultConditionModels = defaultConditions;
            }
        }
        var lstForms;
        if (ctrl.extensionElementModel && ctrl.extensionElementModel.Elements.length > 0) {
            lstForms = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "form" }, true);
        }
        var ReEnterForm;
        if (lstForms && lstForms.length > 0) {
            for (var i = 0; i < lstForms.length; i++) {
                if (lstForms[i].dictAttributes.sfwMode == "update") {
                    ReEnterForm = lstForms[i];
                    break;
                }
            }
        }
        newScope.lstActiveForm = [];
        newScope.setActiveFomList = function (FormName) {
            if (FormName && FormName.contains("=")) {
                var tempActiveForm = FormName.split(";");
                if (tempActiveForm) {
                    for (var i = 0; i < tempActiveForm.length; i++) {
                        var tempObj = tempActiveForm[i].split("=");
                        newScope.lstActiveForm.push(tempObj[1]);
                    }
                }
            }
        };
        if (ReEnterForm) {
            newScope.setActiveFomList(ReEnterForm.dictAttributes.sfwFormName);
        }

        newScope.AddDefaultConditionModelsForMultipleForms = function (ConditionModels, lstActiveForms, param) {
            if (param == "Default") {
                //Deleting unwanted Default/pre Conditons
                for (var i = ConditionModels.length - 1; i >= 0; i--) {
                    if (!ConditionModels[i].dictAttributes.sfwActiveForm) {
                        ConditionModels.splice(i, 1);
                    } else {
                        var isConditionFound = false;
                        for (var j = 0; j < lstActiveForms.length > 0; j++) {
                            if (ConditionModels[i].dictAttributes.sfwActiveForm == lstActiveForms[j]) {
                                isConditionFound = true;
                                break;
                            }
                        }
                        if (!isConditionFound) {
                            ConditionModels.splice(i, 1);
                        }
                    }
                }

                //Adding Default/pre Condition
                for (var k = 0; k < lstActiveForms.length; k++) {
                    var isConditionFound = false;
                    for (l = 0; l < ConditionModels.length; l++) {
                        if (ConditionModels[l].dictAttributes.sfwActiveForm == lstActiveForms[k]) {
                            isConditionFound = true;
                            break;
                        }
                    }

                    if (!isConditionFound) {

                        var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };
                        defaultConditionModel.dictAttributes.sfwActiveForm = lstActiveForms[k];
                        ConditionModels.push(defaultConditionModel);
                    }
                }
            }
            else {
                //Deleting unwanted Default/pre Conditons

                for (var j = ConditionModels.Elements.length - 1; j >= 0; j--) {

                    if (!ConditionModels.Elements[j].dictAttributes.sfwActiveForm) {
                        ConditionModels.Elements.splice(j, 1);
                    } else {
                        var isConditionFound = false;
                        for (var j = 0; j < lstActiveForms.length > 0; j++) {
                            if (ConditionModels.Elements[j].dictAttributes.sfwActiveForm == lstActiveForms[j]) {
                                isConditionFound = true;
                                break;
                            }
                        }
                        if (!isConditionFound) {
                            ConditionModels.Elements.splice(i, 1);
                        }
                    }
                }

                var performersModel = ConditionModels;
                //if (param == "PreConditions") {
                //    performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };
                //    ConditionModels.push(performersModel);
                //}
                //Adding Default/pre Condition
                for (var k = 0; k < lstActiveForms.length; k++) {
                    var isConditionFound = false;

                    for (m = 0; m < ConditionModels.Elements.length; m++) {

                        if (ConditionModels.Elements[m].dictAttributes.sfwActiveForm == lstActiveForms[k]) {
                            isConditionFound = true;
                            break;
                        }
                    }


                    if (!isConditionFound && performersModel) {
                        var preConditionsModel = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: { sfwName: "Always" }, Elements: [],
                                Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                            }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                        };
                        preConditionsModel.dictAttributes.sfwActiveForm = lstActiveForms[k];
                        performersModel.Elements.push(preConditionsModel);
                    }
                }
            }
        };

        var ObjConditonAndExternaltask = JSON.stringify(ctrl.ObjConditonAndExternaltask);
        newScope.ObjConditonAndExternaltask = JSON.parse(ObjConditonAndExternaltask);
        if (ctrl.defaultConditionModel && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            if (newScope.lstActiveForm && newScope.lstActiveForm.length == 0) {
                var defaultConditionModel = JSON.stringify(ctrl.defaultConditionModel);
                newScope.defaultConditionModel = JSON.parse(defaultConditionModel);
            } else {
                var defaultConditionModels = JSON.stringify(ctrl.defaultConditionModels);
                newScope.defaultConditionModels = JSON.parse(defaultConditionModels);
                newScope.AddDefaultConditionModelsForMultipleForms(newScope.defaultConditionModels, newScope.lstActiveForm, "Default");
            }
        }

        newScope.onChangePreconditionsActiveForm = function (activeForm) {
            for (var i = 0; i < newScope.PerformersModels.length; i++) {
                for (var j = 0; j < newScope.PerformersModels[i].Elements.length; j++) {
                    if (newScope.PerformersModels[i].Elements[j].dictAttributes.sfwActiveForm == activeForm) {
                        newScope.objPreconditonsModel = newScope.PerformersModels[i].Elements[j];
                        newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[0];
                        break;
                    }
                }

            }
        };
        if (ctrl.PerformersModel && ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            var PerformersModel = JSON.stringify(ctrl.PerformersModel);
            newScope.PerformersModel = JSON.parse(PerformersModel);
            if (newScope.PerformersModel && newScope.PerformersModel.Elements[0].Name == "preconditions") {
                newScope.ObjConditonAndExternaltask.IsPreCondition = true;
                if (newScope.lstActiveForm && newScope.lstActiveForm.length == 0) {
                    newScope.objPreconditonsModel = newScope.PerformersModel.Elements[0];
                    newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[0];
                } else {
                    var PerformersModels = JSON.stringify(ctrl.PerformersModels);
                    newScope.PerformersModels = JSON.parse(PerformersModels);
                    newScope.AddDefaultConditionModelsForMultipleForms(newScope.PerformersModels[0], newScope.lstActiveForm, "PreConditions");
                    newScope.PerformersModels.sfwActiveForm = newScope.lstActiveForm[0];
                    newScope.onChangePreconditionsActiveForm(newScope.PerformersModels.sfwActiveForm);
                }
            }
        }

        newScope.mapvariablesmodel = ctrl.mapvariablesmodel;
        newScope.IsDisabledforVersioning = ctrl.isdisabledforversioning;
        newScope.queryParametersValues = ctrl.queryParametersValues;
        newScope.UserRoles = ctrl.userRoles;
        newScope.operatorsList = ['GreaterThan', 'GreaterThanEqual', 'Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'StartsWith', 'EndsWith'];


        newScope.setClasstoSelectedObj = function (index) {
            newScope.obj.selectedobjindex = index;
        };

        newScope.lstBPMNRelatedCodeValues = ctrl.lstBpmnRelatedCodeValues;

        newScope.addPerformersDefaultCondition = function () {
            var conditionModel = {
                dictAttributes: { roleoperator: "Equal", skilloperator: "Equal", locationoperator: "Equal", authorityLeveloperator: "Equal", positionoperator: "Equal", useroperator: "Equal" },
                Elements: [], Children: [], Name: "condition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
            };
            if (newScope.defaultConditionModel && newScope.lstActiveForm.length == 0) {
                newScope.defaultConditionModel.Elements.push(conditionModel);
            } else if (newScope.defaultConditionModels.sfwActiveForm) {
                for (var i = 0; i < newScope.defaultConditionModels.length; i++) {
                    if (newScope.defaultConditionModels[i].dictAttributes.sfwActiveForm == newScope.defaultConditionModels.sfwActiveForm) {
                        newScope.defaultConditionModels[i].Elements.push(conditionModel);
                        break;
                    }
                }

            }
            conditionModel.IsUsertaskConditionVisibility = true;
        };

        newScope.deletePerformersDefaultCondition = function () {
            if (newScope.defaultConditionModel && newScope.defaultConditionModel.selectedCondition && newScope.lstActiveForm.length == 0) {
                var idx = newScope.defaultConditionModel.Elements.indexOf(newScope.defaultConditionModel.selectedCondition);
                newScope.defaultConditionModel.Elements.splice(idx, 1);
                newScope.defaultConditionModel.selectedCondition = undefined;
            } else if (newScope.defaultConditionModels && newScope.defaultConditionModels.sfwActiveForm) {
                for (var i = 0; i < newScope.defaultConditionModels.length; i++) {
                    if (newScope.defaultConditionModels[i].dictAttributes.sfwActiveForm == newScope.defaultConditionModels.sfwActiveForm) {
                        var indx = newScope.defaultConditionModels[i].Elements.indexOf(newScope.defaultConditionModels[i].selectedCondition);
                        newScope.defaultConditionModels[i].Elements.splice(indx, 1);
                        newScope.defaultConditionModels[i].selectedCondition = undefined;
                        break;
                    }
                }
            }
        };

        newScope.changePreConditionModelBasedonselectedtab = function (preconditonModel, index) {
            //newScope.objTab = {};
            //newScope.objTab.selectedtabindex = index;
            newScope.objPreconditonModel = preconditonModel;
            newScope.obj.selectedobjindex = undefined;
        };

        newScope.addPerformersPreCondition = function () {
            if (newScope.objPreconditonModel != null && newScope.objPreconditonModel != undefined) {
                var preConditionModel = {
                    dictAttributes: { roleoperator: "Equal", skilloperator: "Equal", locationoperator: "Equal", authorityLeveloperator: "Equal", positionoperator: "Equal", useroperator: "Equal" },
                    Elements: [], Children: [], Name: "condition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                };
                newScope.objPreconditonModel.Elements.push(preConditionModel);
                newScope.objPreconditonModel.selectedCondition = preConditionModel;
                newScope.objPreconditonModel.selectedCondition.IsUsertaskConditionVisibility = true;
            }
        };

        newScope.deletePerformersPreCondition = function () {
            if (newScope.objPreconditonModel != null && newScope.objPreconditonModel.selectedCondition) {
                var idx = newScope.objPreconditonModel.Elements.indexOf(newScope.objPreconditonModel.selectedCondition);
                newScope.objPreconditonModel.Elements.splice(idx, 1);
                newScope.objPreconditonModel.selectedCondition = undefined;
            }
        };

        newScope.AddNewTabForPreConditonsClick = function () {
            var preconditonName = GetNewConditonName("PreCondition", newScope.objPreconditonsModel, 1);
            var newScopeForPreconditionDailog = $scope.$new();
            newScopeForPreconditionDailog.objConditon = {};
            newScopeForPreconditionDailog.objConditon.PreConditionName = preconditonName;
            newScopeForPreconditionDailog.dialog = $rootScope.showDialog(newScopeForPreconditionDailog, "", "BPM/views/Performer/NewPreConditionTemplate.html");
            newScopeForPreconditionDailog.setNewPreConditionDetails = function () {
                var isInvalidId = false;
                var strErrorMsg = "";
                if (!newScopeForPreconditionDailog.objConditon.PreConditionName) {
                    isInvalidId = true;
                    strErrorMsg = "ID cannot be blank.";
                }
                else {
                    isInvalidId = CheckForDuplicateIDForConditions(newScopeForPreconditionDailog.objConditon.PreConditionName, newScope.objPreconditonsModel);
                    strErrorMsg = "Duplicate ID Present. Please enter another ID";
                }
                if (!isInvalidId) {
                    var preConditionsModel = {
                        dictAttributes: { sfwName: "PreCondition" }, Elements: [],
                        Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                    };
                    preConditionsModel.dictAttributes.sfwName = newScopeForPreconditionDailog.objConditon.PreConditionName;
                    newScope.objPreconditonsModel.Elements.push(preConditionsModel);

                    newScope.objPreconditonModel = preConditionsModel;
                    newScope.obj.selectedobjindex = undefined;
                    newScopeForPreconditionDailog.dialog.close();
                } else {
                    if (strErrorMsg) {
                        alert(strErrorMsg);
                    }
                }
            };

            newScopeForPreconditionDailog.closeNewConditionDialog = function () {
                newScopeForPreconditionDailog.dialog.close();
            };
        };
        newScope.removePreconditonfromPreconditionsModel = function (index) {
            newScope.objPreconditonsModel.Elements.splice(index, 1);
            if (newScope.objPreconditonsModel.Elements.length > 0) {
                newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[newScope.objPreconditonsModel.Elements.length - 1];
                newScope.obj.selectedobjindex = undefined;
            }
        };
        newScope.scrollLeft = function (event) {
            event.preventDefault();
            $element = $(event.target);
            $element.next("ul").animate({
                scrollLeft: "-=200px"
            }, "slow");
        }
        newScope.scrollRight = function (event) {
            event.preventDefault();
            $element = $(event.target);
            $element.prev("ul").animate({
                scrollLeft: "+=200px"
            }, "slow");
        }

        if (newScope.lstActiveForm && newScope.defaultConditionModels && newScope.lstActiveForm.length > 0) {
            newScope.defaultConditionModels.sfwActiveForm = newScope.lstActiveForm[0];
        }

        newScope.templateName = "BPM/views/Performer/UserTaskDefaultandPreconditions.html";
        newScope.dialogOptions = { height: 600, width: 800 };

        newScope.editNameForPreCondition = function (obj) {
            if (obj && !newScope.IsDisabledforVersioning) {
                var preconditonName = obj.dictAttributes.sfwName;
                var newScopeForPreconditionDailog = $scope.$new();
                newScopeForPreconditionDailog.objConditon = {};
                newScopeForPreconditionDailog.objConditon.PreConditionName = preconditonName;
                newScopeForPreconditionDailog.dialog = $rootScope.showDialog(newScopeForPreconditionDailog, "", "BPM/views/Performer/NewPreConditionTemplate.html");
                newScopeForPreconditionDailog.setNewPreConditionDetails = function () {
                    var isInvalidId = false;
                    var strErrorMsg = "";
                    if (!newScopeForPreconditionDailog.objConditon.PreConditionName) {
                        isInvalidId = true;
                        strErrorMsg = "ID cannot be blank.";
                    }
                    else {
                        var index = newScope.objPreconditonsModel.Elements.indexOf(obj);
                        isInvalidId = CheckForDuplicateIDForConditionsWhileEdit(newScopeForPreconditionDailog.objConditon.PreConditionName, newScope.objPreconditonsModel, index);
                        strErrorMsg = "Duplicate ID Present. Please enter another ID";
                    }
                    if (!isInvalidId) {

                        obj.dictAttributes.sfwName = newScopeForPreconditionDailog.objConditon.PreConditionName;

                        newScopeForPreconditionDailog.dialog.close();
                    } else {
                        if (strErrorMsg) {
                            alert(strErrorMsg);
                        }
                    }
                };

                newScopeForPreconditionDailog.closeNewConditionDialog = function () {
                    newScopeForPreconditionDailog.dialog.close();
                };
            }
        };

        newScope.onOKClick = function () {
            ctrl.ObjConditonAndExternaltask = newScope.ObjConditonAndExternaltask;
            $rootScope.UndRedoBulkOp("Start");
            if (newScope.lstActiveForm.length > 0 && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.EditPropertyValue(ctrl.PerformersModel.Elements, ctrl.PerformersModel, "Elements", newScope.defaultConditionModels);
            } else if (ctrl.defaultConditionModel && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.EditPropertyValue(ctrl.defaultConditionModel.Elements, ctrl.defaultConditionModel, "Elements", newScope.defaultConditionModel.Elements);
            }
            if (ctrl.PerformersModels && ctrl.ObjConditonAndExternaltask.IsPreCondition && newScope.lstActiveForm.length > 0) {
                for (var i = ctrl.extensionElementModel.Elements.length - 1; i >= 0; i--) {
                    if (ctrl.extensionElementModel.Elements[i].Name == "performers") {
                        $rootScope.DeleteItem(ctrl.extensionElementModel.Elements[i], ctrl.extensionElementModel.Elements, null);
                    }
                }
                for (var j = 0; j < newScope.PerformersModels.length; j++) {
                    $rootScope.InsertItem(newScope.PerformersModels[j], ctrl.extensionElementModel.Elements, 0, null);
                }
            }
            else if (ctrl.PerformersModel && ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.DeleteItem(ctrl.PerformersModel.Elements[0], ctrl.PerformersModel.Elements, null);
                $rootScope.InsertItem(newScope.PerformersModel.Elements[0], ctrl.PerformersModel.Elements, 0, null);
            }
            $rootScope.UndRedoBulkOp("End");
            newScope.dialog.close();
        };
        newScope.onCancelClick = function () {
            newScope.dialog.close();
        };

        newScope.dialog = $rootScope.showDialog(newScope, "Condition", newScope.templateName, newScope.dialogOptions);
    };

    ctrl.removeOrAddModelBasedOnExternaltaskCondition = function () {
        if (ctrl.ObjConditonAndExternaltask.IsExternaltask) {
            ctrl.externalTaskModel = {
                dictAttributes: {
                }, Elements: [], Children: [], Name: "externalTask", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            ctrl.extensionElementModel.Elements.push(ctrl.externalTaskModel);
        } else {
            for (var i = 0; i < ctrl.extensionElementModel.Elements.length; i++) {
                if (ctrl.extensionElementModel.Elements[i].Name == 'externalTask') {
                    ctrl.extensionElementModel.Elements.splice(i, 1);
                    break;
                }
            }
        }
    };

    ctrl.changeModelBasedOnConditionInPreformers = function () {
        if (ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            var preConditionsModel = {
                dictAttributes: {}, Elements: [{
                    dictAttributes: { sfwName: "Always" }, Elements: [],
                    Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
            };
            // Remove old model
            while (ctrl.PerformersModel.Elements.length > 0) {
                ctrl.PerformersModel.Elements.splice(0, 1);
            }
            ctrl.PerformersModel.Elements.splice(0, 0, preConditionsModel);
            //newScope.PerformersModel.Elements[0] = preConditionsModel;
            ctrl.defaultConditionModel = null;
            ctrl.objPreconditonsModel = ctrl.PerformersModel.Elements[0];
            ctrl.objPreconditonModel = ctrl.objPreconditonsModel.Elements[0];
            //newScope.obj.selectedobjindex = undefined;
        }
        else {
            var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };

            // Remove old model
            //for (var i = ctrl.extensionElementModel.Elements.length - 1; i >= 0; i--) {
            //    if (ctrl.extensionElementModel.Elements[i].Name == "performers") {
            //        $rootScope.DeleteItem(ctrl.extensionElementModel.Elements[i], ctrl.extensionElementModel.Elements, null);
            //    }
            //}
            //var performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };
            //ctrl.PerformersModel = performersModel;
            //$rootScope.InsertItem(ctrl.PerformersModel, ctrl.extensionElementModel.Elements, 0, null);
            //removed above code beacuse if deleted and added the assignment option and other attributes were getting cleared
            // Remove old model
            while (ctrl.PerformersModel.Elements.length > 0) {
                ctrl.PerformersModel.Elements.splice(0, 1);
            }
            ctrl.PerformersModel.Elements.splice(0, 0, defaultConditionModel);
            //newScope.PerformersModel.Elements[0] = defaultConditionModel;
            ctrl.defaultConditionModel = defaultConditionModel;
        }
    };


    ctrl.populateFollowAssignmentValues();
}

app.component('bpmLanePerformersProperties', {
    bindings: {
        shapeModel: '<',
        userRoles: '<',
        isdisabledforversioning: '<'
    },
    templateUrl: 'BPM/views/Performer/LanePerformersProperties.html',
    controller: ["$filter", bpmLanePerformersPropertiesController]
});

app.component('bpmUserTaskPerformersProperties', {
    bindings: {
        shapeModel: '<',
        isdisabledforversioning: '<',
        targetsitelist: '<',
        lstBpmnRelatedCodeValues: '<',
        mapvariablesmodel: '<',
        userRoles: '<',
    },
    templateUrl: 'BPM/views/Performer/UserTaskPerformersProperties.html',
    controller: ["$filter", "$getQueryparam", "$scope", "$rootScope", bpmUserTaskPerformersProperties]
});