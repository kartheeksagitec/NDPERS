app.directive("usertaskperformerstableheadertemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<table style='width:100%' >" +
            "<thead>" +
            "<tr class='boldfont grey-bottom-border'>" +
            " <th style='width:10px'><div class='arrow-td'></div></th>" +
            "<th><div>Role</div> </th>" +
            "<th><div>Skill</div> </th>" +
            "<th><div>Location</div></th>" +
            "<th><div>Authority Level</div></th>" +
            "<th><div>Position</div>" +
            "</th><th><div>User</div> </th>" +
            "</tr>" +
            " </thead>" +
            "</table>";
        return template;
    };
    return {
        restrict: 'E',
        replace: true,
        scope: {
        },
        link: function (scope, element, attrs) {
            //$(element).parent().html("<input type='text' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/>");
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
        }
    };
}]);

app.directive("usertaskperformerstabledatatemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            conditionsModel: "=",
            lstbpmnrelatedcodevalues: "=",
            lstuserroles: "=",
            mapvariablesmodel: "=",
            selectedCondition: "=",
            isdisabledforversioning: '<'
        },
        link: function (scope, element, attrs) {
            scope.types = { Role: "role", Skill: "skill", Location: "location", AuthorityLevel: "authoritylevel", Position: "position", User: "user" };
            scope.setEmptyvalueOnChange = function (objcondition, param) {
                if (param == "role") {
                    objcondition.dictAttributes.role = "";
                }
                else if (param == "skill") {
                    objcondition.dictAttributes.skill = "";
                } else if (param == "location") {
                    objcondition.dictAttributes.location = "";
                } else if (param == "position") {
                    objcondition.dictAttributes.position = "";
                }
                else if (param == "authoritylevel") {
                    objcondition.dictAttributes.authorityLevel = "";
                }
                else if (param == "user") {
                    objcondition.dictAttributes.user = "";
                }
            };
            scope.selectCondition = function (condition) {
                scope.selectedCondition = condition;
            };
        },
        templateUrl: 'BPM/views/Performer/userTaskPerformersTableData.html'
    };
}]);

function enterReEnterController($scope, $rootScope, $filter, $EntityIntellisenseFactory, $NavigateToFileService) {
    var scope = this;
    scope.model.SelectActiveFormType = 'Single';
    scope.ActiveFormType = "Lookup,Maintenance,Wizard";
    if (scope.model.dictAttributes.sfwFormName != undefined && scope.model.dictAttributes.sfwFormName != "") {
        if (scope.model.dictAttributes.sfwFormName.contains("=")) {
            scope.model.SelectActiveFormType = 'Multiple';
        }
    }

    scope.onChangeActiveForm = function () {
        $rootScope.UndRedoBulkOp("Start");
        if (scope.model.SelectActiveFormType == 'Single') {
            $rootScope.EditPropertyValue('Multiple', scope.model, "SelectActiveFormType", 'Single');
        } else {
            $rootScope.EditPropertyValue('Single', scope.model, "SelectActiveFormType", 'Multiple');
        }
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwVariableName, scope.model.dictAttributes, "sfwVariableName", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", "");
        //removing parameters
        while (scope.model.Elements.length > 0) {
            $rootScope.DeleteItem(scope.model.Elements[0], scope.model.Elements);
        }
        //removing actions
        while (scope.model.dictAttributes.sfwMode == 'update' && scope.parent.actionsModel && scope.parent.actionsModel.Elements.length > 0) {
            $rootScope.DeleteItem(scope.parent.actionsModel.Elements[0], scope.parent.actionsModel.Elements);
        }

        if (scope.model.dictAttributes.sfwMode == 'update' && scope.parent.extensionElementModel) {
            var PerformersModel = $filter('filter')(scope.parent.extensionElementModel.Elements, { Name: "performers" }, true);
            if (PerformersModel && PerformersModel.length > 0) {
                if (PerformersModel[0].Elements && PerformersModel[0].Elements.length > 0) {
                    if (PerformersModel[0].Elements[0].Name == "preconditions") {
                        for (var i = scope.parent.extensionElementModel.Elements.length - 1; i >= 0; i--) {
                            if (scope.parent.extensionElementModel.Elements[i].Name == "performers") {
                                $rootScope.DeleteItem(scope.parent.extensionElementModel.Elements[i], scope.parent.extensionElementModel.Elements);
                            }
                        }

                        var performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };

                        var preConditionsModel = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: { sfwName: "Always" }, Elements: [],
                                Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                            }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                        };
                        performersModel.Elements.push(preConditionsModel);

                        $rootScope.InsertItem(performersModel, scope.parent.extensionElementModel.Elements, 0);

                    } else if (PerformersModel[0].Elements[0].Name == "defaultcondition") {
                        var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };
                        while (PerformersModel[0].Elements.length > 0) {
                            $rootScope.DeleteItem(PerformersModel[0].Elements[0], PerformersModel[0].Elements);
                        }
                        $rootScope.InsertItem(defaultConditionModel, PerformersModel[0].Elements, 0);
                    }
                }
            }
        }
        scope.setFormType(true);
        $rootScope.UndRedoBulkOp("End");
    };

    scope.lstActiveForm = [];
    scope.setlstActiveForms = function () {
        scope.lstActiveForm = [];
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.contains("=")) {
            var tempActiveForm = scope.model.dictAttributes.sfwFormName.split(";");
            if (tempActiveForm) {
                for (var i = 0; i < tempActiveForm.length; i++) {
                    var tempObj = tempActiveForm[i].split("=");
                    scope.lstActiveForm.push({ FieldValue: tempObj[0], ActiveForm: tempObj[1] });
                }
            }
        }
    };
    scope.setlstActiveForms();


    scope.setVisibleRulesForMultipleForms = function (param) {
        var visibleRuleDialogScope = $scope.$new(true);
        visibleRuleDialogScope.lstActiveForm = scope.lstActiveForm;
        visibleRuleDialogScope.model = scope.model;
        visibleRuleDialogScope.lstVisibleRules = [];
        visibleRuleDialogScope.getlstVisibleRules = function (param) {
            var modelValue = visibleRuleDialogScope.model.dictAttributes[param];
            if (modelValue && modelValue.contains("=")) {
                var tempActiveForm = modelValue.split(";");
                if (tempActiveForm) {
                    for (var i = 0; i < tempActiveForm.length; i++) {
                        var tempObj = tempActiveForm[i].split("=");
                        visibleRuleDialogScope.lstVisibleRules.push({ ActiveForm: tempObj[0], VisibleRule: tempObj[1] });
                    }
                }
            }
        };
        visibleRuleDialogScope.getlstVisibleRules(param);
        //delete ActiveForm visibleRules
        for (var j = visibleRuleDialogScope.lstVisibleRules.length - 1; j >= 0; j--) {
            var isFound = false;
            for (var k = 0; k < visibleRuleDialogScope.lstActiveForm.length; k++) {
                if (visibleRuleDialogScope.lstVisibleRules[j].ActiveForm == visibleRuleDialogScope.lstActiveForm[k].ActiveForm) {
                    isFound = true;
                    break;
                }
            }
            if (!isFound) {
                visibleRuleDialogScope.lstVisibleRules.splice(j, 1);
            }
        }

        //Add newly added forms
        for (var j = 0; j < visibleRuleDialogScope.lstActiveForm.length; j++) {
            var isFound = false;
            for (var k = 0; k < visibleRuleDialogScope.lstVisibleRules.length; k++) {
                if (visibleRuleDialogScope.lstVisibleRules[k].ActiveForm == visibleRuleDialogScope.lstActiveForm[j].ActiveForm) {
                    isFound = true;
                    break;
                }
            }

            if (!isFound) {
                var objVisbileRule = {};
                objVisbileRule.ActiveForm = visibleRuleDialogScope.lstActiveForm[j].ActiveForm;
                objVisbileRule.VisibleRule = "";
                visibleRuleDialogScope.lstVisibleRules.push(objVisbileRule);
            }
        }
        visibleRuleDialogScope.selectActiveFormRow = function (form) {
            visibleRuleDialogScope.selectedActiveForm = form;
        };
        visibleRuleDialogScope.OkClick = function () {
            var activeFormString = "";
            var oldFormValue = scope.model.dictAttributes[param];
            if (visibleRuleDialogScope.lstVisibleRules.length > 0) {
                for (var i = 0; i < visibleRuleDialogScope.lstVisibleRules.length; i++) {
                    if (visibleRuleDialogScope.lstVisibleRules[i].ActiveForm != "" && visibleRuleDialogScope.lstVisibleRules[i].VisibleRule != "") {
                        if (activeFormString == "") {
                            activeFormString = visibleRuleDialogScope.lstVisibleRules[i].ActiveForm + "=" + visibleRuleDialogScope.lstVisibleRules[i].VisibleRule;
                        }
                        else {
                            activeFormString += ";" + visibleRuleDialogScope.lstVisibleRules[i].ActiveForm + "=" + visibleRuleDialogScope.lstVisibleRules[i].VisibleRule;
                        }
                    }
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes[param], scope.model.dictAttributes, param, activeFormString);
            }
            else {
                $rootScope.EditPropertyValue(scope.model.dictAttributes[param], scope.model.dictAttributes, param, "");
            }

            visibleRuleDialogScope.CancelClick();
        };
        visibleRuleDialogScope.CancelClick = function () {
            visibleRuleDialogScope.VisibleRuleFormDialog.close();
        };

        visibleRuleDialogScope.onVisibleRuleChange = function (form, event) {
            var input;
            if ((event.keyCode && event.keyCode != 13 && event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") || event.type == "click") {
                if (event.type && event.type == "click") input = $(event.target).prevAll("input[type='text']");
                else input = $(event.target);
                if (form && !form.VisibleRules) {
                    if (form.ActiveForm && form.ActiveForm.contains("Maintenance")) {
                        $.connection.hubBPMN.server.getFormModel(form.ActiveForm).done(function (data) {
                            if (form && data) {
                                form.VisibleRules = data.VisibleRules;
                                setSingleLevelAutoComplete(input, form.VisibleRules, visibleRuleDialogScope, "Code", "Description");
                            }
                        });
                    }
                }
                if (form.VisibleRules) {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    } else {
                        setSingleLevelAutoComplete(input, form.VisibleRules, visibleRuleDialogScope, "Code", "Description");
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                    }
                    if (event.type && event.type == "click") {
                        input.focus();
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                    }
                }
            }
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                event.preventDefault();
            }
        };
        visibleRuleDialogScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        visibleRuleDialogScope.VisibleRuleFormDialog = $rootScope.showDialog(visibleRuleDialogScope, "Set Visible Rules", "BPM/views/Association/BpmVisibleRulesForMultipleActiveForms.html", { width: 800, height: 500 });
    };

    scope.onAddMultipleActiveFormClick = function (event) {
        var newScope = $scope.$new(true);
        newScope.model = scope.model;
        scope.setlstActiveForms();
        newScope.ActiveFormType = scope.ActiveFormType;
        newScope.lstActiveForm = scope.lstActiveForm;
        newScope.AddActiveForm = function () {
            newScope.lstActiveForm.push({ FieldValue: "", ActiveForm: "" });
        };

        newScope.DeleteActiveForm = function () {
            if (newScope.selectedActiveForm && newScope.selectedActiveForm != "") {
                var index = newScope.lstActiveForm.indexOf(newScope.selectedActiveForm);
                if (index > -1) {
                    newScope.lstActiveForm.splice(index, 1);
                }
                else {
                    newScope.selectedActiveForm = undefined;
                }
            }
        };
        newScope.selectActiveFormRow = function (form) {
            newScope.selectedActiveForm = form;
        };
        newScope.validateActiveForms = function () {
            var IsValid = false;
            newScope.ErrorMessageForDisplay = "";
            if (newScope.lstActiveForm.length == 0) {
                newScope.ErrorMessageForDisplay = "Atleast one active form has to be added.";
                return true;
            }
            else if (newScope.lstActiveForm.length > 0 && newScope.lstActiveForm.some(function (x) { return x.FieldValue === "" })) {
                newScope.ErrorMessageForDisplay = "Field Value can not be null.";
                return true;
            }

            else if (newScope.lstActiveForm.length > 0 && newScope.lstActiveForm.some(function (x) { return x.ActiveForm === "" })) {
                newScope.ErrorMessageForDisplay = "Active Form can not be null.";
                return true;
            }
            return IsValid;
        };
        newScope.OkClick = function () {
            var activeFormString = "";
            var oldFormValue = scope.model.dictAttributes.sfwFormName;
            if (newScope.lstActiveForm.length > 0) {
                for (var i = 0; i < newScope.lstActiveForm.length; i++) {
                    if (newScope.lstActiveForm[i].FieldValue != "" && newScope.lstActiveForm[i].ActiveForm != "") {
                        if (activeFormString == "") {
                            activeFormString = newScope.lstActiveForm[i].FieldValue + "=" + newScope.lstActiveForm[i].ActiveForm;
                        }
                        else {
                            activeFormString += ";" + newScope.lstActiveForm[i].FieldValue + "=" + newScope.lstActiveForm[i].ActiveForm;
                        }
                    }
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", activeFormString);
            }
            else {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", "");
            }
            if (oldFormValue != activeFormString) {
                scope.onFormNameChanged();
            }
            newScope.CancelClick();
        };
        newScope.CancelClick = function () {
            newScope.ActiveFormDialog.close();
        };
        newScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        newScope.ActiveFormDialog = $rootScope.showDialog(newScope, "Active Forms", "BPM/views/BpmActiveForms.html", { width: 800, height: 500 });
    };



    scope.$onInit = function () {
        scope.refreshParam = false;
        scope.formData = { entityID: "", formTitle: "", controlTree: [], initialLoadModel: {}, visibleRules: [] };
        scope.setNewActiveFormData();
        if (scope.parent.currentUserTaskData.formList.length == 0) {
            $.connection.hubMain.server.getFilesByType("Maintenance,Lookup,Wizard", "ScopeId_" + $scope.$id, null);
            $scope.receiveList = function (data) {
                scope.parent.currentUserTaskData.formList = data;
            };
        }

        scope.setFormType();
    };

    scope.setNewActiveFormData = function () {
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            if (scope.model.dictAttributes.sfwFormName.contains("=") && scope.lstActiveForm.length > 0) {
                $.connection.hubBPMN.server.getFormModel(scope.lstActiveForm[0].ActiveForm).done(receiveFormModel);
            }
            else {
                $.connection.hubBPMN.server.getFormModel(scope.model.dictAttributes.sfwFormName).done(receiveFormModel);
            }
        }
    };
    var receiveFormModel = function (data) {
        if (data) {
            var updateValues = function () { updateCurrentUserTaskData(scope, data); };
            $scope.$evalAsync(updateValues);
            if (scope.refreshParam) {
                $scope.$evalAsync(function () {
                    $scope.initParam();
                });
            }
        }
    };

    scope.onFormNameChanged = function () {

        $rootScope.UndRedoBulkOp("Start");
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            scope.addModel();
            scope.resetModel();
            var FormName = scope.model.dictAttributes.sfwFormName;
            if (FormName.contains('=') && scope.lstActiveForm.length > 0) {
                if (scope.lstActiveForm[0].ActiveForm) {
                    FormName = scope.lstActiveForm[0].ActiveForm;
                }
            }
            var isValid = scope.isValidFile(FormName);
            if (isValid) {
                $.connection.hubBPMN.server.getFormModel(FormName).done(receiveFormModel);
            }
            else {
                scope.removeModel();
                scope.resetModel();
            }
        }
        else {
            scope.removeModel();
            scope.resetModel();
        }
        $rootScope.UndRedoBulkOp("End");
    };
    scope.addModel = function () {
        var index = scope.extelementmodel.Elements.indexOf(scope.model);
        if (index == -1) {
            $rootScope.PushItem(scope.model, scope.extelementmodel.Elements);
        }
    };
    scope.removeModel = function () {
        var index = scope.extelementmodel.Elements.indexOf(scope.model);
        if (index > -1) {
            $rootScope.DeleteItem(scope.model, scope.extelementmodel.Elements);
        }
    };
    scope.resetModel = function () {
        $rootScope.EditPropertyValue(scope.model.dictAttributes.FormTitle, scope.model.dictAttributes, "FormTitle", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmSubmitButtonVisibleRule, scope.model.dictAttributes, "sfwBpmSubmitButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmApproveButtonVisibleRule, scope.model.dictAttributes, "sfwBpmApproveButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmRejectButtonVisibleRule, scope.model.dictAttributes, "sfwBpmRejectButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);
        scope.setFormType(true);
        scope.formData.initialLoadModel = null;
        scope.formData.entityID = null;
        scope.formData.remoteObject = null;
        scope.formData.formTitle = null;
        scope.formData.controlTree = null;
        scope.formData.visibleRules = null;
        scope.formData.lookupParameters = null;
    };
    scope.setFormType = function (fromUndoRedoBlock) {
        var formName = "";
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.contains("=") && scope.lstActiveForm.length > 0) {
            formName = scope.lstActiveForm[0].ActiveForm;

        } else {
            formName = scope.model.dictAttributes.sfwFormName;
        }
        if (formName && formName.trim().length > 0) {
            if (fromUndoRedoBlock) {
                if (formName.indexOf("Lookup") == formName.length - 6) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "lookup");
                }
                else if (formName.indexOf("Maintenance") == formName.length - 11) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "maintenance");
                }
                else if (formName.indexOf("Wizard") == formName.length - 6) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "wizard");
                }
                else {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "");
                }
            }
            else {
                if (formName.indexOf("Lookup") == formName.length - 6) {
                    scope.formType = "lookup";
                }
                else if (formName.indexOf("Maintenance") == formName.length - 11) {
                    scope.formType = "maintenance";
                }
                else if (formName.indexOf("Wizard") == formName.length - 6) {
                    scope.formType = "wizard";
                }
                else {
                    scope.formType = "";
                }
            }
        } else {
            $rootScope.EditPropertyValue(scope.formType, scope, "formType", "");
        }
    };
    scope.onFormNameKeyDown = function (event) {
        var input = $(event.target);
        //if (input.val() == undefined || input.val().trim() == "") {
        var data = scope.parent.currentUserTaskData.formList;
        setSingleLevelAutoComplete(input, data);
        //}
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        }
    };
    scope.showFormNameIntellisenseList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        input.focus();
        // if (input.val() == undefined || input.val().trim() == "") {
        var data = scope.parent.currentUserTaskData.formList;
        setSingleLevelAutoComplete(input, data);
        //}
        if ($(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
        }
    };

    scope.setNavigationParameters = function () {
        var paramDialogScope = $scope.$new();
        var bpmScope = getCurrentFileScope();
        paramDialogScope.lstGlobalParameters = bpmScope.lstGlobalParameters;
        if (bpmScope) {
            paramDialogScope.variables = bpmScope.mapVariablesModel.Elements;
        }
       
        paramDialogScope.formType = scope.formType;
        $scope.initParam = function () {
            paramDialogScope.parameters = scope.getParameters();
            if (scope.formType == "lookup") {
                paramDialogScope.criteriaFields = scope.formData.lookupParameters;
                paramDialogScope.selectCriteriaField = function (field) {
                    paramDialogScope.selectedCriteriaField = field;
                };
                paramDialogScope.deleteParameter = function () {
                    var index = paramDialogScope.parameters.indexOf(paramDialogScope.selectedParameter);
                    if (index > -1) {
                        paramDialogScope.parameters.splice(index, 1);
                    }
                };
                paramDialogScope.canDeleteParameter = function () {
                    return paramDialogScope.selectedParameter ? true : false;
                };

            } else {
                if (!paramDialogScope.parameters || paramDialogScope.parameters.length == 0) {
                    paramDialogScope.parameters = [];
                    var param = {};
                    param.fieldName = "";
                    if (scope.model.dictAttributes.sfwFormMode != "new") {
                        param.name = "aintPrimaryKey";
                    } else {
                        param.name = "ParentKey";
                    }
                    param.source = "";
                    param.value = "";
                    paramDialogScope.parameters.push(param);

                    if (scope.model.dictAttributes.sfwFormMode == "new") {
                        var param = {};
                        param.fieldName = "";
                        param.name = "ParentKeyName";
                        param.source = "";
                        param.value = "";
                        paramDialogScope.parameters.push(param);
                    }
                }
                if (paramDialogScope.parameters) {
                    for (var i = scope.model.Elements.length - 1; i > -1; i--) {
                        var isValidparameter = false;
                        if (scope.model.Elements[i].Name == "parameter") {
                            for (var j = 0; j < paramDialogScope.parameters.length; j++) {
                                if (scope.model.Elements[i].dictAttributes.sfwParamaterName == paramDialogScope.parameters[j].name) {
                                    isValidparameter = true;
                                    paramDialogScope.parameters[j].value = scope.model.Elements[i].dictAttributes.sfwParameterValue;
                                    paramDialogScope.parameters[j].source = scope.model.Elements[i].dictAttributes.sfwValueSource;
                                    break;
                                }
                            }
                        }
                        if (!isValidparameter) {
                            scope.model.Elements.splice(i, 1);
                        }
                    }
                }

            }
        };
        paramDialogScope.selectParameter = function (param) {
            paramDialogScope.selectedParameter = param;
        };
        paramDialogScope.onSaveClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);
            function iterator(param) {
                $rootScope.PushItem({ prefix: "", Name: "parameter", dictAttributes: { sfwParamaterName: param.name, sfwValueSource: param.source, sfwParameterValue: param.value, sfwFieldName: param.fieldName, sfwDataType: param.dataType }, Elements: [], Children: [] }, scope.model.Elements);
            }
            angular.forEach(paramDialogScope.parameters, iterator);
            paramDialogScope.onCancelClick();
            $rootScope.UndRedoBulkOp("End");
        };
        paramDialogScope.onCancelClick = function () {
            if (paramDialogScope.dialog)
                paramDialogScope.dialog.close();
        };
        paramDialogScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        paramDialogScope.setActiveFormData = function () {
            scope.setNewActiveFormData();
            scope.refreshParam = true;
        };
        $scope.initParam();
        paramDialogScope.dialog = $rootScope.showDialog(paramDialogScope, "Set Navigation Parameters", "BPM/views/Association/UserTaskSetNavigationParametersTemplate.html", { height: 450, width: 1100 });
    };
    scope.getParameters = function () {
        if (scope.formType == "lookup") {
            var params = scope.model.Elements.map(function (param) {
                return {
                    name: param.dictAttributes.sfwParamaterName,
                    source: param.dictAttributes.sfwValueSource,
                    value: param.dictAttributes.sfwParameterValue,
                    dataType: param.dictAttributes.sfwDataType,
                    fieldName: param.dictAttributes.sfwFieldName
                };
            });
            return params;
        }
        else {
            var initialLoadModel = scope.formData.initialLoadModel;
            var entityID = scope.formData.entityID;
            var remoteObject = scope.formData.remoteObject;

            if (initialLoadModel) {
                var selectedMethod;
                var mode = scope.isenter && (scope.model.dictAttributes.sfwFormMode == undefined || scope.model.dictAttributes.sfwFormMode == "new") ? "New" : "Update";
                var callMethods = undefined;
                if (initialLoadModel.Elements && initialLoadModel.Elements.length > 0) {
                    callMethods = initialLoadModel.Elements.filter(function (item) { return item.Name == "callmethods" && ((!item.dictAttributes.hasOwnProperty("sfwMode")) || item.dictAttributes.sfwMode == mode); });
                }
                if (callMethods && callMethods.length > 0) {
                    selectedMethod = callMethods[0].dictAttributes.sfwMethodName;
                }
                if (selectedMethod) {
                    var params;
                    if (remoteObject) {
                        var methods = $filter('filter')(remoteObject.Elements, { dictAttributes: { ID: selectedMethod } }, true);
                        if (methods && methods.length > 0) {
                            params = methods[0].Elements.map(function (param) { return { name: param.dictAttributes.ID, source: "", value: "", fieldName: param.ID }; });
                        }
                    }
                    else {
                        var lparams = $EntityIntellisenseFactory.getXmlMethodParameters(entityID, selectedMethod, true);
                        if (lparams && lparams.length > 0) {
                            params = lparams.map(function (param) { return { name: param.ID, source: "", value: "", fieldName: param.ID }; });
                        }
                    }

                    if (params && params.length > 0) {
                        function iterator(param) {
                            var existingParams = $filter('filter')(scope.model.Elements, { dictAttributes: { sfwParamaterName: param.name } }, true);
                            if (existingParams && existingParams.length > 0) {
                                param.source = existingParams[0].dictAttributes.sfwValueSource;
                                param.value = existingParams[0].dictAttributes.sfwParameterValue;
                            }
                        }
                        angular.forEach(params, iterator);
                        return params;
                    }
                } else {
                    if (scope.model.Elements.length > 0) {
                        var Params = scope.model.Elements.map(function (param) { return { name: param.dictAttributes.sfwParamaterName, source: param.dictAttributes.sfwValueSource, value: param.dictAttributes.sfwParameterValue, fieldName: param.dictAttributes.sfwFieldName }; });
                        return Params;
                    }
                }
            }
        }
    };
    scope.setFocusControlID = function () {
        focusControlDialogScope = $scope.$new();
        focusControlDialogScope.controlTree = scope.formData.controlTree;
        focusControlDialogScope.onSaveClick = function () {
            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", focusControlDialogScope.selectedControlID);
            focusControlDialogScope.onCancelClick();
        };
        focusControlDialogScope.onCancelClick = function () {
            if (focusControlDialogScope.dialog)
                focusControlDialogScope.dialog.close();
        };
        focusControlDialogScope.selectControlID = function (element, event) {
            focusControlDialogScope.selectedControlID = element.ID;
        };
        focusControlDialogScope.dialog = $rootScope.showDialog(focusControlDialogScope, "", "BPM/views/SelectFocusControlTemplate.html");
    };
    scope.onVisibleRuleKeyDown = function (event) {
        var visibleRules = [];
        var input = $(event.target);
        if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
            visibleRules = scope.formData.visibleRules;
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                $(input).autocomplete("search", $(input).val());
                event.preventDefault();
            } else {
                setSingleLevelAutoComplete($(event.target), visibleRules, scope, "Code", "Description");
            }
        }

        //if (event) {
        //    event.preventDefault();
        //}
    };
    scope.showVisibleRuleIntellisenseList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        var visibleRules = [];
        visibleRules = scope.formData.visibleRules;
        input.focus();
        setSingleLevelAutoComplete(input, visibleRules, scope, "Code", "CodeDescription");
        if ($(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
        }
    };
    scope.NavigateToForm = function (aFormID) {
        if (aFormID && aFormID != "") {
            if (aFormID.contains("=") && scope.lstActiveForm.length > 0) {
                if (scope.lstActiveForm[0].ActiveForm) {
                    aFormID = scope.lstActiveForm[0].ActiveForm;
                }
            }
            hubMain.server.navigateToFile(aFormID, "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };
    scope.NavigateToFormControl = function (aControlID, aFormID) {
        if (aFormID.contains("=") && scope.lstActiveForm.length > 0) {
            if (scope.lstActiveForm[0].ActiveForm) {
                aFormID = scope.lstActiveForm[0].ActiveForm;
            }
        }
        if (aControlID && aFormID && aFormID != "" && aControlID != "") {
            $NavigateToFileService.NavigateToFile(aFormID, "", aControlID);
        }
    };
    scope.NavigateToVisibleRule = function (aVisibleRuleID) {
        var entity = scope.formData.entityID;
        if (aVisibleRuleID && aVisibleRuleID != "") {
            $NavigateToFileService.NavigateToFile(entity, "initialload", aVisibleRuleID);
        }
    };
    scope.setTaskActions = function () {
        if (scope.model && scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            var ActiveFormName = scope.model.dictAttributes.sfwFormName;
            if (scope.model.dictAttributes.sfwFormName.contains('=')) {
                scope.setlstActiveForms();
                if (scope.lstActiveForm.length > 0 && scope.lstActiveForm[0].ActiveForm) {
                    ActiveFormName = scope.lstActiveForm[0].ActiveForm;
                }
            }
            if (scope.isValidFile(ActiveFormName)) {
                taskActionsScope = $scope.$new();
                taskActionsScope.ActiveFormType = scope.model.SelectActiveFormType;
                taskActionsScope.lstActiveForm = scope.lstActiveForm;
                if (scope.parent.actionsModel) {
                    taskActionsScope.actionsModel = JSON.parse(JSON.stringify(scope.parent.actionsModel));
                    var actions = taskActionsScope.actionsModel.Elements.filter(function (x) { return x.Elements.length > 0; });
                    function iterator(action) {
                        action.selectedVariable = action.Elements[0];
                        action.selectedVariableID = action.selectedVariable.dictAttributes.ID;
                    }
                    angular.forEach(actions, iterator);
                }
                taskActionsScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
                taskActionsScope.isObjectbased = scope.isObjectbased;
                var success = function (objectList) {
                    var updateAssociatedFormData = function () {
                        taskActionsScope.entities = objectList[0];
                        taskActionsScope.buttons = objectList[1];
                    };
                    taskActionsScope.$evalAsync(updateAssociatedFormData);
                };
                taskActionsScope.variables = scope.parent.queryParametersValues;
                $.connection.hubBPMN.server.getAssociatedFormData(ActiveFormName).done(success);
                taskActionsScope.onEntityChanged = function (action) {
                    if (action && action.selectedVariable) {
                        action.selectedVariable.dictAttributes.sfwEntityField = "";
                    }
                };
                taskActionsScope.onProcessVariableChanged = function (action) {
                    if (action) {

                        if (action.Elements.length > 0) {
                            action.Elements.splice(0, 1);
                            // $rootScope.DeleteItem(action.Elements[0], action.Elements, null);
                        }
                        if (action.dictAttributes.sfwButtonID && action.dictAttributes.sfwButtonID.trim().length > 0) {

                            action.selectedVariable = {
                                prefix: "sbpmn",
                                Name: "variable",
                                Value: "",
                                Elements: [],
                                Children: [],
                                dictAttributes: { ID: action.selectedVariableID, sfwEntity: action.dictAttributes.sfwEntity }
                            };
                            action.Elements.push(action.selectedVariable);
                        }

                    }
                };
                taskActionsScope.onIsConstantChanged = function (action) {
                    if (action) {
                        action.dictAttributes.sfwEntity = "";
                        if (action.selectedVariable) {
                            action.selectedVariable.dictAttributes.sfwEntity = "";
                            action.selectedVariable.dictAttributes.sfwEntityField = "";
                        }
                    }
                };

                taskActionsScope.getButtonsAndEntitiesFromActiveForm = function (action) {
                    if (action && action.dictAttributes && action.dictAttributes.sfwActiveForm) {
                        $.connection.hubBPMN.server.getAssociatedFormData(action.dictAttributes.sfwActiveForm).done(function (result) {
                            taskActionsScope.$evalAsync(function () {
                                action.entities = result[0];
                                action.buttons = result[1];
                            });
                        });
                    } else {
                        if (action) {
                            action.entities = [];
                            action.buttons = [];
                        }
                    }
                };

                taskActionsScope.selectAction = function (action, isInUndoRedoBlock) {
                    if (isInUndoRedoBlock) {
                        var selectedAction = taskActionsScope.actionsModel.Elements.filter(function (x) { return x.isSelected; });
                        if (selectedAction && selectedAction.length > 0) {
                            selectedAction[0].isSelected = false;
                        }
                        action.isSelected = true;
                    }
                    else {
                        function iterator(act) {
                            act.isSelected = act == action;
                        }
                        angular.forEach(taskActionsScope.actionsModel.Elements, iterator);
                    }
                };
                taskActionsScope.addAction = function () {
                    var newItem = { prefix: "sbpmn", Name: "action", dictAttributes: {}, Elements: [], Value: "", Children: [] };
                    taskActionsScope.actionsModel.Elements.push(newItem);
                    taskActionsScope.selectAction(newItem, true);
                };
                taskActionsScope.canDeleteAction = function () {
                    if (taskActionsScope.actionsModel != undefined && taskActionsScope.actionsModel.Elements != undefined) {
                        return taskActionsScope.actionsModel.Elements.some(function (x) {
                            return x.isSelected;
                        });
                    }
                };
                taskActionsScope.deleteAction = function () {
                    var selectedActions = taskActionsScope.actionsModel.Elements.filter(function (x) {
                        return x.isSelected;
                    });
                    if (selectedActions && selectedActions.length > 0) {
                        var index = taskActionsScope.actionsModel.Elements.indexOf(selectedActions[0]);
                        if (index > -1) {

                            //Get and select the next action, before removing the desired item.
                            var nextActionToBeSelected = index + 1 == taskActionsScope.actionsModel.Elements.length ? taskActionsScope.actionsModel.Elements[index - 1] : taskActionsScope.actionsModel.Elements[index + 1];
                            if (nextActionToBeSelected) {
                                taskActionsScope.selectAction(nextActionToBeSelected, true);
                            }
                            taskActionsScope.actionsModel.Elements.splice(index, 1);
                        }
                    }

                };
                taskActionsScope.onOKClick = function () {
                    $rootScope.UndRedoBulkOp("Start");
                    while (scope.parent.actionsModel.Elements.length > 0) {
                        $rootScope.DeleteItem(scope.parent.actionsModel.Elements[0], scope.parent.actionsModel.Elements, null);
                    }

                    for (var idx = 0; idx < taskActionsScope.actionsModel.Elements.length; idx++) {
                        $rootScope.PushItem(taskActionsScope.actionsModel.Elements[idx], scope.parent.actionsModel.Elements);
                    }
                    $rootScope.UndRedoBulkOp("End");
                    taskActionsScope.closeDialog();
                };
                taskActionsScope.closeDialog = function () {
                    if (taskActionsScope.dialog)
                        taskActionsScope.dialog.close();
                };
                taskActionsScope.dialog = $rootScope.showDialog(taskActionsScope, "Task Actions", "BPM/views/Association/SetUserTaskActions.html", { width: 1100 });

                //#region While Changing Active form Entity id and field should get clear
                taskActionsScope.OnChangeOfActiveForms = function (action) {
                    if (action) {
                        action.dictAttributes.sfwButtonID = "";
                        if (action.selectedVariable && action.selectedVariable.dictAttributes) {
                            action.selectedVariable.dictAttributes.sfwEntity = "";
                            action.selectedVariable.dictAttributes.sfwEntityField = "";
                            action.selectedVariableID = "";
                        }
                    }
                };
                //#endregion


            }
        }
    };
    scope.isValidFile = function (strfilename) {
        var isvalid = false;
        for (var i = 0; i < scope.parent.currentUserTaskData.formList.length; i++) {
            if (scope.parent.currentUserTaskData.formList[i] && strfilename) {
                if (scope.parent.currentUserTaskData.formList[i].toLowerCase() == strfilename.toLowerCase().trim()) {

                    isvalid = true;
                    break;
                }
            }
        }
        return isvalid;
    };
}
app.component("bpmEnterReEnterFormDetails", {
    require: { parent: '^bpmUserTaskAssociationProperties' },
    bindings: {
        model: "<",
        approvalactivity: "<",
        isenter: "<",
        extelementmodel: "<",
        IsDisabledforVersioning: "<isDisabledforVersioning",
        isObjectbased: "<?"
    },
    controller: ["$scope", "$rootScope", "$filter", "$EntityIntellisenseFactory", "$NavigateToFileService", enterReEnterController],
    templateUrl: "BPM/views/Association/EnterReEnterFormDetailsTemplate.html"
});


// usertask directives for performers tab

app.directive("userrolesintellisensetemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<div class='input-group full-width'><input type='text' ng-disabled='isdisabledforversioning' undoredodirective  model='model' propertyname='propertyname' class='form-control input-sm' title='{{rolename}}' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/><span ng-if='!isdisabledforversioning' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div>";
        return template;
    };
    return {
        restrict: 'E',
        replace: true,
        scope: {
            lstuserroles: "=",
            rolename: "=",
            isdisabledforversioning: '<',
            model: "=",
            propertyname: "="

        },
        link: function (scope, element, attrs) {
            //$(element).parent().html("<input type='text' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/>");
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.intellienseList = [];
            scope.intellienseList = scope.lstuserroles;
            SetCodeIDDescriptionToList(scope.intellienseList);
            scope.UserRoleTextChanged = function (event) {
                var input = $(event.target);
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                } else {
                    setSingleLevelAutoComplete(input, scope.intellienseList, scope, "Description", "CodeIDDescription");
                }
            };
            scope.showIntellisenseList = function (event) {
                var input = $(event.target).prevAll("input[type='text']");
                setSingleLevelAutoComplete(input, scope.lstuserroles, scope, "Description", "CodeIDDescription");
                if ($(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val().trim());
                }
                input.focus();
                if (event) {
                    event.stopPropagation();
                }
            };
        }
    };
}]);


app.directive("bpmncodevaluesbasedontypeintellisense", ['$compile', '$rootScope', function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<div class='input-group full-width'>" + "<input class='form-control input-sm' type='text' title='{{objconditionmodel.dictAttributes[type]}}' ng-model='objconditionmodel.dictAttributes[type]' ng-show='getBoolValue(false)' ng-keydown='TextChanged($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-show='getBoolValue(false)'></span>" +
            '<select class="form-control input-sm" ng-options="variable.dictAttributes.id as variable.dictAttributes.id for variable in mapvariablesmodel.Elements"  ng-show="getBoolValue(true)" title="{{objconditionmodel.dictAttributes[type]}}" ng-model="objconditionmodel.dictAttributes[type]" ></select>'
            + "</div>";
        return template;
    };

    var setModelValue = function (lst, value) {
        var modelvalue = "";
        if (value != undefined) {
            data = lst.filter(function (x) {
                return x.CodeID.match(value);
            });
            if (data.length > 0) {
                modelvalue = data[0].CodeIDDescription;
            }
        }
        return modelvalue;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            lstbpmnrelatedcodevalues: "=",
            type: "=",
            lstuserroles: "=",
            mapvariablesmodel: "=",
            objconditionmodel: "="
        },
        link: function (scope, element, attrs) {
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.lst = [];
            if (scope.type == "role") {
                scope.lst = scope.lstuserroles;

                if (scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable.toLowerCase() == "true") {
                    scope.modelvalue = scope.objconditionmodel.dictAttributes.role;
                }
                else {
                    scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.role);
                }
            }
            else {
                if (scope.type == "skill") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "SkillList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.skill;
                            }
                            else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.skill);
                            }
                        }
                    });
                }
                else if (scope.type == "location") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "LocationList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.location;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.location);
                            }
                        }
                    });
                } else if (scope.type == "position") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "PositionList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.position;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.position);
                            }
                        }
                    });
                } else if (scope.type == "authorityLevel") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "AuthorityLevelList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.authorityLevel;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.authorityLevel);
                            }
                        }
                    });
                } else if (scope.type == "user") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "UserIds") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.user;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.user);
                            }
                        }
                    });
                }
            }
            SetCodeIDDescriptionToList(scope.lst);
            scope.getBoolValue = function (isMapVariable) {
                var value;
                if (scope.type == "role") {
                    value = scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable;
                }
                else if (scope.type == "skill") {
                    value = scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable;
                }
                else if (scope.type == "location") {
                    value = scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable;
                } else if (scope.type == "position") {
                    value = scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable;
                } else if (scope.type == "authorityLevel") {
                    value = scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable;
                } else if (scope.type == "user") {
                    value = scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable;
                }
                if (value != undefined) {
                    if (value.toLowerCase() == "false" && !isMapVariable) {
                        return true;
                    } else if (value.toLowerCase() == "true" && isMapVariable) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (!isMapVariable) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            };

            scope.TextChanged = function (event) {
                var input = $(event.target);
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                    else {
                        var propertyName = "CodeID";
                        if (scope.type == "role") {
                            propertyName = "Description";
                        }
                        setSingleLevelAutoCompleteForCodeValues(input, scope.lst, scope, propertyName, "CodeIDDescription", scope.objconditionmodel, scope.type);
                    }
                }
            };

            scope.showIntellisenseList = function (event) {
                var input = $(event.target).prevAll("input[type='text']");
                setSingleLevelAutoCompleteForCodeValues(input, scope.lst, scope, "CodeID", "CodeIDDescription", scope.objconditionmodel, scope.type);
                input.focus();
                if ($(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val().trim());
                }
            };
        }
    };
}]);


function updateCurrentUserTaskData(scope, formDetails) {
    scope.formData.entityID = formDetails.EntityID;
    scope.model.dictAttributes.FormTitle = formDetails.FormTitle;
    scope.formData.controlTree = formDetails.ControlTree;
    scope.formData.initialLoadModel = formDetails.InitialLoadModel;
    scope.formData.visibleRules = formDetails.VisibleRules;
    scope.formData.lookupParameters = formDetails.LookupParameters;
    scope.formData.remoteObject = formDetails.RemoteObject;
}
