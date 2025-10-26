function userTaskAssociationPropertiesController($scope, $filter, $rootScope) {
    var scope = this;
    scope.$onInit = function () {
        scope.init();
    };
    scope.init = function () {

        scope.currentUserTaskData = { selectedEntity: "BpmActivityInstance", formList: [] };

        var extensionElements = $filter('filter')(scope.shapeModel.Elements, { Name: "extensionElements" });
        if (extensionElements && extensionElements.length > 0) {
            scope.extensionElementModel = extensionElements[0];
        }
        if (scope.extensionElementModel) {
            var items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "ReferenceId" });
            if (items && items.length > 0) {
                scope.referenceIDModel = items[0];
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "form", dictAttributes: { sfwMode: "new" } });
            if (items && items.length > 0) {
                scope.enterFormModel = items[0];
            }
            else {
                scope.enterFormModel = scope.getNewEnterReEnterModel("new");
                scope.enterFormModel.dictAttributes.sfwFormMode = "new";
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "form", dictAttributes: { sfwMode: "update" } });
            if (items && items.length > 0) {
                scope.reEnterFormModel = items[0];
            }
            else {
                scope.reEnterFormModel = scope.getNewEnterReEnterModel("update");
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "actions" });
            if (items && items.length > 0) {
                scope.actionsModel = items[0];
            }
        }
    };

    scope.onModeChange = function () {
        if (scope.enterFormModel && scope.enterFormModel.Elements.length > 0) {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = scope.enterFormModel.Elements.length - 1; i >= 0; i--) {
                var parameter = scope.enterFormModel.Elements[i];
                $rootScope.DeleteItem(parameter, scope.enterFormModel.Elements);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    scope.getNewEnterReEnterModel = function (mode) {
        var model = { prefix: "sbpmn", Name: "form", Value: "", dictAttributes: { sfwMode: mode }, Elements: [], Children: [] };
        return model;
    };
}
app.component('bpmUserTaskAssociationProperties', {
    bindings: {
        queryParametersValues: "<",
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        isObjectbased: "<?"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", userTaskAssociationPropertiesController],
    templateUrl: 'BPM/views/Association/UserTaskAssociationProperties.html',
});

function businessRuleTaskAssociationController($scope, $filter, $rootScope, $getEnitityRule, $EntityIntellisenseFactory, $getEnitityXmlMethods) {
    var ctrl = this;
    ctrl.$onInit = function () {
        $scope.init();
    };
    var objtemp = {};
    $scope.init = function () {
        $scope.arrMapVariableforEntity = [];
        var extensionElement = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extensionElements" })[0];
        var rule = $filter('filter')(extensionElement.Elements, { Name: "rule" })[0];
        $scope.model = rule;

        var parametersNode = $filter('filter')($scope.model.Elements, { Name: "parameters" }, true);
        if (parametersNode && parametersNode.length > 0) {
            $scope.model.parameterschild = parametersNode[0];
        }

        // in case of bussiness rule there are two child nodes 
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule') {
            $scope.model.baseChild = $filter('filter')(rule.Elements, { Name: "ServiceBusinessObject" }, true)[0];

            if ($scope.model.baseChild && $scope.model.baseChild.Elements) {
                if ($scope.model.baseChild.dictAttributes.variable) {
                    $scope.model.baseChild.FailOverServiceModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "FailOverServiceBusinessObject" }, true)[0];
                    $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.FailOverServiceModel.Elements, { Name: "initialize" }, true)[0];
                    $scope.selectedTab = "FailOverServiceBusinessObject";
                } else {
                    $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "initialize" }, true)[0];
                }
                $scope.model.baseChild.entryModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
                $scope.model.baseChild.exitModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnExit" }, true)[0];

                if ($scope.model.baseChild.dictAttributes.sfwEntity && $scope.model.baseChild.entryModel && $scope.model.baseChild.exitModel) { /*Bug 8830:Exception on clicking service task of existing map in BPM*/
                    var entryElement = $scope.model.baseChild.entryModel.Elements;
                    var exitElement = $scope.model.baseChild.exitModel.Elements;
                    var sfwNavigationParameter = $scope.model.baseChild.dictAttributes.sfwNavigationParameter;
                    $scope.getMapVariableEntityWise();
                    if ($scope.isBusinessRuleTypeValid) {
                        if ($scope.model.baseChild.initializeModel) {
                            $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                        }
                        if ($scope.model.baseChild.initializeModel.xmlmethodModel && $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes && $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                            var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.xmlMethods, { ID: $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true)[0];
                            $scope.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                            if (sfwNavigationParameter) $scope.model.baseChild.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                        }
                        if (entryElement.length > 0) $scope.model.baseChild.entryModel.Elements = entryElement;
                        if (exitElement.length > 0) $scope.model.baseChild.exitModel.Elements = exitElement;
                    }
                }
            }
            if ($scope.model.dictAttributes.ID) {
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
                var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
                if (objRule) { $scope.returnType = objRule.ReturnType; }
            }
        }
        // for static rule 
        if ($scope.model.dictAttributes.sfwEntity && $scope.model.dictAttributes.sfwType === 'StaticRule') {
            objtemp = $getEnitityRule.getWithParam($scope.model.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
            $scope.arrRuleIds = objtemp.ruleIds;
            var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
            if (objRule) { $scope.returnType = objRule.ReturnType; }
        }
    };
    $scope.onChangeType = function (type) {
        $scope.model.dictAttributes.sfwType = type;
        $scope.Clear("baserule");
        $scope.isBusinessRuleTypeValid = false;
        var nodeParameters = new objElements();
        switch ($scope.model.dictAttributes.sfwType.toLowerCase()) {
            case "staticrule": nodeParameters.Name = "parameters"; break;
            case "businessrule":
                var childName = ["initialize", "OnEnter", "OnExit"];
                nodeParameters.Name = "ServiceBusinessObject";
                for (i = 0; i < childName.length; i++) {
                    var nodeServiceChild = new objElements();
                    nodeServiceChild.Name = childName[i];
                    nodeParameters.Elements.push(nodeServiceChild);
                }
                break;
        }
        $scope.model.Elements.push(nodeParameters);
        var ServiceBusiness = $filter('filter')($scope.model.Elements, { Name: "ServiceBusinessObject" });
        var StaticQuery = $filter('filter')($scope.model.Elements, { Name: "parameters" });
        if (StaticQuery && StaticQuery.length > 0) {
            $scope.model.parameterschild = StaticQuery[0];
        }
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule' && ServiceBusiness.length == 1) {
            $scope.model.baseChild = ServiceBusiness[0];
            $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "initialize" }, true)[0];
            $scope.model.baseChild.initializeModel.dictAttributes.ObjectType = "XmlMethod";
            $scope.model.baseChild.entryModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
            $scope.model.baseChild.exitModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnExit" }, true)[0];

            // add xml method to initialize here only - because object is not an option 
            if ($scope.model.baseChild.initializeModel) {
                var nodeXmlmethod = new objElements();
                nodeXmlmethod.Name = "XMLMethod";
                $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
            }
            // check this code if it is used or not
            if ($scope.model.baseChild.dictAttributes.sfwEntity) {
                var nodeParameterssecond = new objElements();
                nodeParameterssecond.Name = "parameters";
                $scope.model.Elements.push(nodeParameterssecond);
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
            }
        }
        else $scope.model.baseChild = StaticQuery[0];
    };
    $scope.onVariableChanged = function () {
        if ($scope.model && $scope.model.baseChild && $scope.model.baseChild.dictAttributes) {
            if ($scope.model.baseChild.dictAttributes.variable && $scope.model.baseChild.dictAttributes.variable.trim().length > 0) {
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                $scope.RemoveInitializeorFailOverModel("initialize");
                $scope.PrepareFailOverServiceBusObjModel();
                $scope.model.baseChild.Elements.splice(0, 0, $scope.model.baseChild.FailOverServiceModel);
                $scope.selectedTab = "FailOverServiceBusinessObject";
            }
            else {
                $scope.RemoveInitializeorFailOverModel("FailOverServiceBusinessObject");
                $scope.PrepareInitializeModel();
                $scope.model.baseChild.Elements.splice(0, 0, $scope.model.baseChild.initializeModel);
                $scope.selectedTab = "Initialize";
            }
        }
    };

    $scope.RemoveInitializeorFailOverModel = function (param) {
        for (var i = 0; i < $scope.model.baseChild.Elements.length; i++) {
            if ($scope.model.baseChild.Elements[i].Name == param) {
                $scope.model.baseChild.Elements.splice(i, 1);
                break;
            }
        }
    };

    $scope.PrepareInitializeModel = function () {
        var initializeModel = {
            dictAttributes: { ObjectType: "XmlMethod" },
            Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "XMLMethod", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" }],
            Children: [], Name: "initialize", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        $scope.model.baseChild.initializeModel = initializeModel;
        $scope.loadXmlmethods();
        var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
        if (xmlmethod.length <= 0) {
            var nodeXmlmethod = new objElements();
            nodeXmlmethod.Name = "XMLMethod";
            $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
        }
        $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
        $scope.isBusinessRuleTypeValid = true;
    };
    $scope.PrepareFailOverServiceBusObjModel = function () {
        $scope.PrepareInitializeModel();
        var FailOverServiceModel = {
            dictAttributes: { sfwNavigationParameter: "" },
            Elements: [],
            Children: [], Name: "FailOverServiceBusinessObject", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        FailOverServiceModel.Elements.push($scope.model.baseChild.initializeModel);
        $scope.model.baseChild.FailOverServiceModel = FailOverServiceModel;
    };

    $scope.loadXmlmethods = function () {
        $scope.model.baseChild.initializeModel.xmlMethods = $getEnitityXmlMethods.get($scope.model.baseChild.dictAttributes.sfwEntity.trim());
    };
    $scope.Clear = function (type) {
        // clear this model for all childerns 
        switch (type) {
            case "baserule": $scope.model.dictAttributes.sfwEntity = "";
            case "type":
                $scope.model.baseChild = {};
                $scope.model.Elements = [];
                $scope.model.dictAttributes.sfwReturnField = "";
                $scope.model.dictAttributes.ID = "";
                $scope.returnType = "";
                $scope.arrRuleIds = "";
                break;
            default: break;
        }
    };
    $scope.getRuledetails = function () {
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule') objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        else if ($scope.model.dictAttributes.sfwType === 'StaticRule') {
            $scope.Clear("type");
            objtemp = $getEnitityRule.getWithParam($scope.model.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        }
        $scope.arrRuleIds = objtemp.ruleIds;
    };
    $scope.getRuleParmeters = function () {
        $scope.returnType = "";
        if (!$scope.model.dictAttributes.ID) {
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwReturnField, $scope.model.dictAttributes, "sfwReturnField", "");
        }
        if ($scope.model.parameterschild) {
            $scope.model.parameterschild.Elements = [];
        }
        else {
            var parametersNode = new objElements();
            parametersNode.Name = "parameters";
            $scope.model.Elements.push(parametersNode);
            $scope.model.parameterschild = parametersNode;
        }
        var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
        if (objRule) {
            $scope.returnType = objRule.ReturnType;
            if (objRule.Parameters.length > 0) {
                if (!($scope.model.Elements.length > 0) && $scope.model.dictAttributes.sfwType !== 'BusinessRule') {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameters";
                    $scope.model.Elements.push(nodeParameters);
                }
                if (!($scope.model.Elements.length > 1) && $scope.model.dictAttributes.sfwType === 'BusinessRule') {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameters";
                    $scope.model.Elements.push(nodeParameters);
                }
                var parametersNode = $filter('filter')($scope.model.Elements, { Name: "parameters" });
                if (parametersNode.length == 1) {
                    $scope.model.parameterschild = parametersNode[0];
                    $scope.model.parameterschild.Elements = [];
                    function iterator(value, key) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        nodeParameters.dictAttributes.sfwParamaterName = value.DisplayName; nodeParameters.dictAttributes.sfwDirection = value.Direction; nodeParameters.dictAttributes.sfwDataType = value.DataType;
                        this.Elements.push(nodeParameters);
                    }
                    angular.forEach(objRule.Parameters, iterator, parametersNode[0]);
                }
            }
        }
    };
    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };
    $scope.getMapVariableEntityWise = function () {
        $scope.arrMapVariableforEntity = [];
        //check if this is valid - model.baseChild.baseChild.dictAttributes.type => isBusinessRuleTypeValid
        if ($scope.model.baseChild.dictAttributes && $scope.model.baseChild.dictAttributes.sfwEntity && $scope.model.baseChild.dictAttributes.sfwEntity.trim().length > 0 && $scope.model.dictAttributes.sfwType.toLowerCase() === 'businessrule') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = $filter('filter')(entityIntellisenseList, { ID: $scope.model.baseChild.dictAttributes.sfwEntity.trim() }, true);
            if (entity.length == 1) {
                $scope.loadXmlmethods();
                $scope.model.baseChild.entryModel.Elements = [];
                $scope.model.baseChild.exitModel.Elements = [];
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
                var mapVar = $filter('filter')($scope.$parent.mapVariablesModel.Elements, { dictAttributes: { sfwEntity: $scope.model.baseChild.dictAttributes.sfwEntity.trim() } }, true);
                function iterator(value, key) {
                    this.push(value.dictAttributes.id);
                }
                angular.forEach(mapVar, iterator, $scope.arrMapVariableforEntity);
                // add xml method model
                var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
                if (xmlmethod.length <= 0) {
                    var nodeXmlmethod = new objElements();
                    nodeXmlmethod.Name = "XMLMethod";
                    $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                    $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                }
                $scope.isBusinessRuleTypeValid = true;
            }
            else {
                $scope.model.baseChild.initializeModel.Elements = [];
                $scope.model.baseChild.initializeModel.xmlMethods = [];
                $scope.model.baseChild.initializeModel.xmlmethodItems = [];
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                $scope.returnType = "";
                $scope.model.dictAttributes.sfwReturnField = "";
                $scope.model.dictAttributes.ID = "";
                if ($scope.model.parameterschild) $scope.model.parameterschild.Elements = [];
                $scope.arrRuleIds = [];
                $scope.isBusinessRuleTypeValid = false;
            }
        }
        else $scope.isBusinessRuleTypeValid = false;
    };
    $scope.refreshRuleParameters = function (event) {
        var ruleId = "";
        if ($scope.model.baseChild && $scope.model.baseChild.dictAttributes) {
            ruleId = $scope.model.baseChild.dictAttributes ? $scope.model.baseChild.dictAttributes.sfwEntity : undefined;
        }
        if ($scope.model.dictAttributes.sfwType === 'StaticRule') {
            ruleId = $scope.model.dictAttributes.sfwEntity;
        }
        objtemp = $getEnitityRule.getWithParam(ruleId, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
        if (objRule) {
            $scope.returnType = objRule.ReturnType;
            var parameter = angular.copy(objRule.Parameters);
            var parameterParent = $scope.model.parameterschild;
            if (!parameterParent) {
                var parametersObj = new objElements();
                parametersObj.prefix = "";
                parametersObj.Name = "parameters";
                parameterParent = parametersObj;
                $rootScope.PushItem(parametersObj, $scope.model.Elements);
            }
            for (var i = parameterParent.Elements.length - 1; i >= 0; i--) {
                items = $filter('filter')(parameter, { ID: parameterParent.Elements[i].dictAttributes.sfwParamaterName });
                if (items && items.length > 0) {
                    var index = parameter.indexOf(items[0]);
                    parameter.splice(index, 1);
                }
                else {
                    $rootScope.DeleteItem(parameterParent.Elements[i], parameterParent.Elements);
                }
            }
            // objRule.Parameters will have only the new parameters
            function iterator(value, key) {
                var nodeParameters = new objElements();
                nodeParameters.Name = "parameter";
                nodeParameters.dictAttributes.ID = value.ID; nodeParameters.dictAttributes.sfwParamaterName = value.DisplayName; nodeParameters.dictAttributes.sfwDirection = value.Direction; nodeParameters.dictAttributes.sfwDataType = value.DataType;
                this.Elements.push(nodeParameters);
            }
            angular.forEach(parameter, iterator, parameterParent);
        }

        event.preventDefault();
        event.stopPropagation();
    };
    $scope.NavigateToFile = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            hubMain.server.navigateToFile(aEntityID, "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };
    $scope.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    $scope.setEnterExitActions = function (isEnter) {
        if ($scope.$parent.showEnterExitDialog) {
            $scope.$parent.showEnterExitDialog(isEnter, isEnter ? $scope.model.baseChild.entryModel : $scope.model.baseChild.exitModel, $scope.model.baseChild.dictAttributes.sfwEntity);
        }
    };
}
app.component("bpmBusinessRuleTaskAssociationProperties", {
    bindings: {
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        queryParametersValues: "<",
        isObjectbased: "<?"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", "$getEnitityRule", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", businessRuleTaskAssociationController],
    templateUrl: "BPM/views/Association/PropertiesBussinessRuleTask.html"
});

function initializeController($scope, $filter, $rootScope, $getQueryparam, $NavigateToFileService, $getEnitityXmlMethods) {
    var ctrl = this;
    $scope.getXmlMethods = function () {
        if ($scope.$ctrl.callbackFunction) {
            $scope.$ctrl.callbackFunction();
        }
    };
    $scope.onXmlMethodChange = function () {
        // load navigation parameters and items 
        if (ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
            var xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.xmlMethods, { ID: ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true)[0];
            if (xmlmethod) {
                ctrl.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(xmlmethod.Parameters, iterator, queryParametersDisplay);
                if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
                }
                else {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
                }
            }
        }
        else {
            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                if (ctrl.model.baseChild.FailOverServiceModel && ctrl.model.baseChild.FailOverServiceModel.dictAttributes) {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", "");
                }
            }
            else {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", "");
            }
        }
    };
    $scope.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    $scope.setXmlparameters = function () {
        // open pop up for setting parameters
        var dialogScope = $scope.$new(true);
        dialogScope.DialogConstants = { queryParamsName: "sfwNavigationParameter" };
        dialogScope.mapObj = {};
        dialogScope.queryValues = ctrl.queryParametersValues;
        if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", ",");
        }
        else {
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", ",");
        }
        dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Xml Method Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
        dialogScope.setQueryParameters = function () {
            var queryParametersDisplay = [];
            function iterator(value, key) {
                this.push(value.ID + '=' + (value.Value ? value.Value : ""));
            }
            angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
            }
            else {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
            }
            dialogScope.QueryDialog.close();
        };

        dialogScope.onRefreshParams = function () {
            var initializeModelXmlMethods = $getEnitityXmlMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim())
            if (initializeModelXmlMethods.length > 0) {
                var initializeXmlMethods = $filter('filter')(initializeModelXmlMethods, { ID: ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true);
                if (initializeXmlMethods && initializeXmlMethods.length > 0) {
                    var initializeXmlMethod = initializeXmlMethods[0];

                    //preserving old parameters.
                    var existingParameters = dialogScope.queryParameters;

                    dialogScope.queryParameters = [];

                    function iterator(value) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        nodeParameters.ID = value.ID; nodeParameters.Value = value.Value;
                        if (dialogScope.queryParameters.length > 0 && !dialogScope.queryParameters.some(function (param) { return param.dictAttributes.ID == value.ID })) {
                            $rootScope.PushItem(nodeParameters, dialogScope.queryParameters);
                        }
                        if (dialogScope.queryParameters.length == 0) {
                            $rootScope.PushItem(nodeParameters, dialogScope.queryParameters);
                        }
                    }
                    angular.forEach(initializeXmlMethod.Parameters, iterator, dialogScope.queryParameters);

                    for (var idx = 0; idx < existingParameters.length; idx++) {
                        if (existingParameters[idx] && existingParameters[idx] && existingParameters[idx].ID) {
                            var params = dialogScope.queryParameters.filter(function (x) { return x.ID == existingParameters[idx].ID; });
                            if (params && params.length > 0) {
                                if (existingParameters[idx].Value) {
                                    $rootScope.EditPropertyValue(params[0].Value, params[0], "Value", existingParameters[idx].Value);
                                }
                            }

                        }
                    }
                }
            }
        }
    };

}
app.component("bpmInitializeSection", {
    bindings: {
        model: "<",
        queryParametersValues: "<",
        isDisabledforVersioning: "<",
        callbackFunction: "&"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", "$getQueryparam", "$NavigateToFileService", "$getEnitityXmlMethods", initializeController],
    templateUrl: "BPM/views/Association/InitializeTemplate.html"
});

function bpmCallactivityAssociationPropertiesController($rootScope, $scope) {
    var ctrl = this;
    ctrl.$onInit = function () {
        function filterExtension(x) {
            function filterParameters(y) {
                return y.Name == "parameters";
            }
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                ctrl.objExtensionModel = x;
                var objParameterModel = x.Elements.filter(filterParameters)[0];
                if (objParameterModel) {
                    ctrl.objParameterModel = objParameterModel;
                    return true;
                }
            }
        }
        ctrl.model.Elements.some(filterExtension);
        if (ctrl.objParameterModel) {
            for (var idx = 0; idx < ctrl.objParameterModel.Elements.length; idx++) {
                if (ctrl.objParameterModel.Elements[idx].dictAttributes.sfwDataType == "Object" && ctrl.objParameterModel.Elements[idx].dictAttributes.sfwValueSource == "Constant") {
                    ctrl.checkAndUpdateParamChildModels({ param: ctrl.objParameterModel.Elements[idx], fromUndoRedoBlock: false });
                }
            }
        }
    };
    ctrl.refreshParameters = function () {
        var processBpmName = ctrl.model.dictAttributes.calledElement ? ctrl.model.dictAttributes.calledElement.split('.')[0] : null;
        if (processBpmName) {
            $rootScope.UndRedoBulkOp("Start");
            ctrl.setCalledelementparameters({ mapName: processBpmName, extensionElementModel: ctrl.objExtensionModel ? ctrl.objExtensionModel : null });
            //  if parameter model was not set initially
            if (ctrl.objExtensionModel) {
                function filterParameters(z) {
                    if (z.Name == "parameters") {
                        ctrl.objParameterModel = z;
                        return true;
                    }
                }
                ctrl.objExtensionModel.Elements.some(filterParameters);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    ctrl.setVParamEntityValue = function (param) {
        ctrl.setParamEntityValue({ parentScope: $scope, param: param });
    };
}

app.component('bpmCallactivityShapeAssociationProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        queryParametersvalues: '<',
        setCalledelementparameters: '&',
        onValueSourceChanged: '&',
        setParamEntityValue: '&',
        checkAndUpdateParamChildModels: '&'
    },
    templateUrl: 'BPM/views/Association/CallActivityAssociationProperties.html',
    controller: ["$rootScope", "$scope", bpmCallactivityAssociationPropertiesController]
});

function bpmServiceRuleAssociationPropertiesController($rootScope, $scope, $filter, $EntityIntellisenseFactory, $getEnitityXmlMethods, $getEnitityObjectMethods, $NavigateToFileService, $getQueryparam) {
    var ctrl = this;
    ctrl.$onInit = function () {
        ctrl.isBusinessRuleTypeValid = false;
        // ctrl.arrMapVariableforEntity = [];
        function filterextensionElements(x) {
            function filterBaseChild(y) {
                function filterInitializeModel(i) {
                    if (i.Name == "initialize") {
                        ctrl.model.baseChild.initializeModel = i;
                    }
                }
                function filterFailOver(z) {
                    if (z.Name == "FailOverServiceBusinessObject") {
                        ctrl.model.baseChild.FailOverServiceModel = z;
                        z.Elements.some(filterInitializeModel);
                    }
                }
                function filterModel(i) {
                    if (i.Name == "OnEnter") {
                        ctrl.model.baseChild.entryModel = i;
                    }
                    else if (i.Name == "OnExit") {
                        ctrl.model.baseChild.exitModel = i;
                    }
                    else if (i.Name == "ServiceMethod") {
                        ctrl.model.baseChild.ServiceMethodmodel = i;
                    }
                    else if (i.Name == "XMLMethod") {
                        ctrl.model.baseChild.initializeModel.xmlmethodModel = i;
                    }
                }
                function filterXmlModel(i) {
                    if (i.ID == ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                        var xmlmethod = ctrl.model.baseChild.initializeModel.xmlmethodModel;
                        ctrl.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                        if (sfwNavigationParameter) {
                            if (ctrl.model.baseChild.dictAttributes.variable) {
                                ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                            } else {
                                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                            }
                        }
                    }
                }
                // for service business object
                if (y.Name == "ServiceBusinessObject") {
                    ctrl.baseType = "entity";
                    ctrl.model.baseChild = y;
                    if (ctrl.model.baseChild.dictAttributes.variable) {
                        ctrl.model.baseChild.Elements.forEach(filterFailOver);
                        ctrl.selectedTab = "FailOverServiceBusinessObject";
                    }
                    else {
                        ctrl.model.baseChild.Elements.forEach(filterInitializeModel);
                        ctrl.selectedTab = "Initialize";
                    }
                    ctrl.model.baseChild.Elements.forEach(filterModel);
                    if (ctrl.model.baseChild.dictAttributes.variable && !ctrl.model.baseChild.FailOverServiceModel) {
                        ctrl.PrepareFailOverServiceBusObjModel();
                    }
                    if (ctrl.model.baseChild.dictAttributes.sfwEntity) {
                        var entryElement = ctrl.model.baseChild.entryModel.Elements;
                        var exitElement = ctrl.model.baseChild.exitModel.Elements;
                        var sfwNavigationParameter = "";
                        if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.FailOverServiceModel) {
                            sfwNavigationParameter = ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter;
                        }
                        else {
                            sfwNavigationParameter = ctrl.model.baseChild.dictAttributes.sfwNavigationParameter;
                        }
                        ctrl.getMapVariableEntityWise();
                        if (ctrl.isBusinessRuleTypeValid) {
                            if (ctrl.model.baseChild.initializeModel) {
                                ctrl.model.baseChild.initializeModel.Elements.some(filterModel);
                            }
                            if (ctrl.model && ctrl.model.baseChild && ctrl.model.baseChild.ServiceMethodmodel) {
                                for (var idx = 0; idx < ctrl.model.baseChild.ServiceMethodmodel.Elements.length; idx++) {
                                    if (ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].Name == "parameter" &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes.sfwDataType == "Object" &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes.sfwValueSource == "Constant") {
                                        ctrl.checkAndUpdateParamChildModels({ param: ctrl.model.baseChild.ServiceMethodmodel.Elements[idx], fromUndoRedoBlock: false });
                                    }
                                }
                            }
                            if (ctrl.model.baseChild.initializeModel && ctrl.model.baseChild.initializeModel.xmlmethodModel && ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes && ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                                ctrl.model.baseChild.initializeModel.xmlMethods.forEach(filterXmlModel);
                            }
                            ctrl.model.baseChild.Elements.some(filterModel);
                            if (entryElement.length > 0) ctrl.model.baseChild.entryModel.Elements = entryElement;
                            if (exitElement.length > 0) ctrl.model.baseChild.exitModel.Elements = exitElement;
                        }
                    }
                }
                else if (y.Name == "Queries") {
                    ctrl.model.baseChild = y;
                    ctrl.baseType = "query";
                }
            }
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                ctrl.model = x;
                x.Elements.forEach(filterBaseChild);
                return true;
            }
        }
        ctrl.shapeModel.Elements.some(filterextensionElements);
        if (ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes.variable) {
            ctrl.model.lstrInitVariable = ctrl.model.baseChild.dictAttributes.variable;
        }
        else {
            ctrl.model.lstrInitVariable = "";
        }
    };
    ctrl.onChangebaseType = function (type) {
        ctrl.baseType = type;
        // clear and reload the models
        ctrl.isBusinessRuleTypeValid = false;
        ctrl.model.baseChild = {};
        // newScope.model.Elements = [];
        var nodeParameters = new objElements();
        switch (type) {
            case "query": nodeParameters.Name = "Queries";
                var ServiceBusiness = $filter('filter')(ctrl.model.Elements, { Name: "ServiceBusinessObject" });
                if (ServiceBusiness.length > 0) {
                    var tempindex = ctrl.model.Elements.indexOf(ServiceBusiness[0]);
                    ctrl.model.Elements.splice(tempindex, 1);
                }
                break;
            case "entity":
                var ScalarQuery = $filter('filter')(ctrl.model.Elements, { Name: "Queries" });
                if (ScalarQuery.length > 0) {
                    var tempindex = ctrl.model.Elements.indexOf(ScalarQuery[0]);
                    ctrl.model.Elements.splice(tempindex, 1);
                }
                var childName = ["initialize", "ServiceMethod", "OnEnter", "OnExit"];
                nodeParameters.Name = "ServiceBusinessObject";
                for (i = 0; i < childName.length; i++) {
                    var nodeServiceChild = new objElements();
                    nodeServiceChild.Name = childName[i];
                    nodeParameters.Elements.push(nodeServiceChild);
                }
                break;
        }
        // newScope.model = extension elements
        ctrl.model.Elements.push(nodeParameters);
        var ServiceBusiness = $filter('filter')(ctrl.model.Elements, { Name: "ServiceBusinessObject" });
        var ScalarQuery = $filter('filter')(ctrl.model.Elements, { Name: "Queries" });
        // for service business object
        if (ServiceBusiness.length == 1 && ScalarQuery.length <= 0) {
            ctrl.model.baseChild = ServiceBusiness[0];
            ctrl.model.baseChild.initializeModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "initialize" }, true)[0];
            ctrl.model.baseChild.initializeModel.dictAttributes.ObjectType = "XmlMethod";
            ctrl.model.baseChild.entryModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
            ctrl.model.baseChild.exitModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "OnExit" }, true)[0];
            ctrl.model.baseChild.ServiceMethodmodel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "ServiceMethod" }, true)[0];
            if (ctrl.model.baseChild.initializeModel) {
                var nodeXmlmethod = new objElements();
                nodeXmlmethod.Name = "XMLMethod";
                ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
            }
        }
        else ctrl.model.baseChild = ScalarQuery[0];
    };
    ctrl.NavigateToFile = function (aEntityID) {
        if (aEntityID) {
            $.connection.hubMain.server.navigateToFile(aEntityID, "").done(function (objfile) {
                $rootScope.openFile(objfile, null);
            });
        }
    };
    ctrl.loadXmlmethods = function () {
        if (ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes && ctrl.model.baseChild.dictAttributes.sfwEntity) {
            if (ctrl.model.baseChild.initializeModel) {
                ctrl.model.baseChild.initializeModel.xmlMethods = $getEnitityXmlMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim());
            }
            ctrl.model.baseChild.ServiceMethodmodel.objectMethods = $getEnitityObjectMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim());
        }
    };
    ctrl.getMapVariableEntityWise = function () {
        //ctrl.arrMapVariableforEntity = [];
        // load service method
        if (ctrl.model.baseChild.dictAttributes && ctrl.model.baseChild.dictAttributes.sfwEntity && ctrl.model.baseChild.dictAttributes.sfwEntity.trim().length > 0 && ctrl.baseType.toLowerCase() === 'entity') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = $filter('filter')(entityIntellisenseList, { ID: ctrl.model.baseChild.dictAttributes.sfwEntity.trim() }, true);
            if (entity.length == 1) {
                ctrl.loadXmlmethods();
                ctrl.model.baseChild.entryModel.Elements = [];
                ctrl.model.baseChild.exitModel.Elements = [];
                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";

                /*--------- commented these lines coz after adding variable(in variable tab ) n coming back to map tab,initialize variable list was not getting updated.--------- */
                /*--------- Instead of "arrMapVariableforEntity" now using "mapVariablesModel" and apply filter for Entity change , on EntityChange calling "showVariablesByEntity()"--------- */

                //var mapVar = $filter('filter')(ctrl.mapVariablesModel.Elements, { dictAttributes: { sfwEntity: ctrl.model.baseChild.dictAttributes.sfwEntity.trim() } }, true);
                //function iterator(value, key) {
                //    this.push(value.dictAttributes.id);
                //}
                //angular.forEach(mapVar, iterator, ctrl.arrMapVariableforEntity);


                /*--------- End--------- */


                // add xml method model
                var xmlmethod;
                if (ctrl.model.baseChild.initializeModel) {
                    xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
                }
                if (xmlmethod && xmlmethod.length <= 0) {
                    var nodeXmlmethod = new objElements();
                    nodeXmlmethod.Name = "XMLMethod";
                    ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                    ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                }
                ctrl.isBusinessRuleTypeValid = true;
            }
            else {
                ctrl.model.baseChild.initializeModel.Elements = [];
                ctrl.model.baseChild.ServiceMethodmodel.Elements = [];
                ctrl.model.baseChild.ServiceMethodmodel.dictAttributes = {};
                ctrl.model.baseChild.initializeModel.xmlMethods = [];
                ctrl.model.baseChild.initializeModel.xmlmethodItems = [];
                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                ctrl.isBusinessRuleTypeValid = false;
            }
        }
        else ctrl.isBusinessRuleTypeValid = false;
    };
    ctrl.RemoveInitializeorFailOverModel = function (param) {
        for (var i = 0; i < ctrl.model.baseChild.Elements.length; i++) {
            if (ctrl.model.baseChild.Elements[i].Name == param) {
                $rootScope.DeleteItem(ctrl.model.baseChild.Elements[i], ctrl.model.baseChild.Elements);

                //ctrl.model.baseChild.Elements.splice(i, 1);
                break;
            }
        }
    };
    ctrl.PrepareInitializeModel = function () {
        var initializeModel = {
            dictAttributes: { ObjectType: "XmlMethod" },
            Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "XMLMethod", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" }],
            Children: [], Name: "initialize", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        ctrl.model.baseChild.initializeModel = initializeModel;
        ctrl.loadXmlmethods();
        var xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
        if (xmlmethod.length <= 0) {
            var nodeXmlmethod = new objElements();
            nodeXmlmethod.Name = "XMLMethod";
            $rootScope.PushItem(nodeXmlmethod, ctrl.model.baseChild.initializeModel.Elements);

            // ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
        }
        ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
        ctrl.isBusinessRuleTypeValid = true;
    };
    ctrl.PrepareFailOverServiceBusObjModel = function () {
        ctrl.PrepareInitializeModel();
        var FailOverServiceModel = {
            dictAttributes: { sfwNavigationParameter: "" },
            Elements: [],
            Children: [], Name: "FailOverServiceBusinessObject", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };

        $rootScope.PushItem(ctrl.model.baseChild.initializeModel, FailOverServiceModel.Elements);

        //  FailOverServiceModel.Elements.push(ctrl.model.baseChild.initializeModel);
        ctrl.model.baseChild.FailOverServiceModel = FailOverServiceModel;
    };
    ctrl.onVariableChanged = function () {

        if (ctrl.model && ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes) {
            var lOldVariableValue = ctrl.model.baseChild.dictAttributes.variable;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(lOldVariableValue, ctrl.model.baseChild.dictAttributes, "variable", ctrl.model.lstrInitVariable);
            $rootScope.EditPropertyValue(lOldVariableValue, ctrl.model, "lstrInitVariable", ctrl.model.lstrInitVariable);

            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {

                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", "");

                //ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                ctrl.RemoveInitializeorFailOverModel("initialize");
                ctrl.PrepareFailOverServiceBusObjModel();

                $rootScope.InsertItem(ctrl.model.baseChild.FailOverServiceModel, ctrl.model.baseChild.Elements, 0);

                //  ctrl.model.baseChild.Elements.splice(0, 0, ctrl.model.baseChild.FailOverServiceModel);
                ctrl.selectedTab = "FailOverServiceBusinessObject";
            } else {
                ctrl.RemoveInitializeorFailOverModel("FailOverServiceBusinessObject");
                ctrl.PrepareInitializeModel();

                $rootScope.InsertItem(ctrl.model.baseChild.initializeModel, ctrl.model.baseChild.Elements, 0);

                //  ctrl.model.baseChild.Elements.splice(0, 0, ctrl.model.baseChild.initializeModel);
                ctrl.selectedTab = "Initialize";
            }
            $rootScope.UndRedoBulkOp("End");

        }
    };
    ctrl.setEnterExitActions = function (isEnter) {
        if ($scope.$parent.showEnterExitDialog) {
            $scope.$parent.showEnterExitDialog(isEnter, isEnter ? ctrl.model.baseChild.entryModel : ctrl.model.baseChild.exitModel, ctrl.model.baseChild.dictAttributes.sfwEntity);
        }
    };
    ctrl.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    ctrl.getServiceMethodparameters = function () {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.Elements, ctrl.model.baseChild.ServiceMethodmodel, "Elements", []);
        $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwReturnType, ctrl.model.baseChild.ServiceMethodmodel.dictAttributes, "sfwReturnType", "");
        if (ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwObjectMethod) {
            var servicemethods = $filter('filter')(ctrl.model.baseChild.ServiceMethodmodel.objectMethods, { ID: ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwObjectMethod }, true);
            if (servicemethods && servicemethods.length > 0) {
                var servicemethod = servicemethods[0];
                function iterator(value, key) {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameter";
                    nodeParameters.dictAttributes.sfwParamaterName = value.ID; nodeParameters.dictAttributes.sfwDataType = value.DataType; nodeParameters.dictAttributes.sfwEntity = value.Entity, nodeParameters.dictAttributes.sfwValueSource = "Parameter", nodeParameters.dictAttributes.sfwParameterValue = value.Value;
                    $rootScope.PushItem(nodeParameters, this.Elements);
                }
                angular.forEach(servicemethod.Parameters, iterator, ctrl.model.baseChild.ServiceMethodmodel);
                $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwReturnType, ctrl.model.baseChild.ServiceMethodmodel.dictAttributes, "sfwReturnType", servicemethod.ReturnType);
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };
    ctrl.setServiceMethodParameter = function () {
        var objNewParamScope = $scope.$new();
        objNewParamScope.model = angular.copy(ctrl.model.baseChild.ServiceMethodmodel);
        objNewParamScope.queryParametersValues = ctrl.queryParametersvalues;
        objNewParamScope.IsDisabledforVersioning = ctrl.isDisabledforVersioning;
        objNewParamScope.onValueSourceChanged = function (param) {
            ctrl.onValueSourceChanged({ param: param });
        };
        objNewParamScope.setParamEntityValue = function (item) {
            ctrl.setParamEntityValue({ parentScope: $scope, param: item });
        };
        objNewParamScope.onOKClick = function () {
            while (ctrl.model.baseChild.ServiceMethodmodel.Elements.length > 0) {
                $rootScope.DeleteItem(ctrl.model.baseChild.ServiceMethodmodel.Elements[0], ctrl.model.baseChild.ServiceMethodmodel.Elements, null);
            }
            for (var idx = 0; idx < objNewParamScope.model.Elements.length; idx++) {
                $rootScope.PushItem(objNewParamScope.model.Elements[idx], ctrl.model.baseChild.ServiceMethodmodel.Elements);
            }
            objNewParamScope.closeDialog();
        };
        objNewParamScope.closeDialog = function () {
            if (objNewParamScope.dialog) {
                objNewParamScope.dialog.close();
            }
        };
        objNewParamScope.onRefreshParams = function () {
            var serviceModelObjectMethod = $getEnitityObjectMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim())
            if (serviceModelObjectMethod.length > 0) {
                var servicemethods = $filter('filter')(serviceModelObjectMethod, { ID: objNewParamScope.model.dictAttributes.sfwObjectMethod }, true);
                if (servicemethods && servicemethods.length > 0) {
                    var servicemethod = servicemethods[0];
                    //preserving old parameters.
                    var existingParameters = objNewParamScope.model.Elements;


                    //clearing current binded list
                    objNewParamScope.model.Elements = [];


                    function iterator(value) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        nodeParameters.dictAttributes.sfwParamaterName = value.ID; nodeParameters.dictAttributes.sfwDataType = value.DataType; nodeParameters.dictAttributes.sfwEntity = value.Entity, nodeParameters.dictAttributes.sfwValueSource = "Parameter", nodeParameters.dictAttributes.sfwParameterValue = value.Value;
                        if (this.Elements.length == 0)
                            $rootScope.PushItem(nodeParameters, this.Elements);
                        else {
                            if (!this.Elements.some(function (param) { return param.dictAttributes.sfwParamaterName == value.ID })) {
                                $rootScope.PushItem(nodeParameters, this.Elements);
                            }
                        }
                    }
                    angular.forEach(servicemethod.Parameters, iterator, objNewParamScope.model);

                    ////Updating current parameter values from old parameters.

                    for (var idx = 0; idx < existingParameters.length; idx++) {
                        if (existingParameters[idx] && existingParameters[idx].dictAttributes && existingParameters[idx].dictAttributes.sfwParamaterName) {
                            var params = objNewParamScope.model.Elements.filter(function (x) { return x.dictAttributes.sfwParamaterName == existingParameters[idx].dictAttributes.sfwParamaterName && x.dictAttributes.sfwDataType == existingParameters[idx].dictAttributes.sfwDataType; });
                            if (params && params.length > 0) {
                                if (existingParameters[idx].dictAttributes.sfwValueSource) {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwValueSource, params[0].dictAttributes, "sfwValueSource", existingParameters[idx].dictAttributes.sfwValueSource);
                                }
                                if (existingParameters[idx].dictAttributes.sfwParameterValue) {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwParameterValue, params[0].dictAttributes, "sfwParameterValue", existingParameters[idx].dictAttributes.sfwParameterValue);
                                }
                                if (existingParameters[idx].Elements.length > 0 && existingParameters[idx].Elements[0].Name == "value") {
                                    $rootScope.EditPropertyValue(params[0].valueModel, params[0], "valueModel", existingParameters[idx].Elements[0]);
                                    $rootScope.PushItem(params[0].valueModel, params[0].Elements);
                                }
                            }

                        }
                    }

                }
            }
        }
        objNewParamScope.dialog = $rootScope.showDialog(objNewParamScope, "Set Method Parameter", 'BPM/views/Association/SetServiceMethodParameter.html', { height: 400, width: 800 });
    };
    ctrl.setscalarquery = function () {
        var objNewScalarQueryScope = $scope.$new();
        objNewScalarQueryScope.model = angular.copy(ctrl.model.baseChild);
        objNewScalarQueryScope.IsDisabledforVersioning = ctrl.isDisabledforVersioning;
        objNewScalarQueryScope.queryParametersValues = ctrl.queryParametersvalues;
        objNewScalarQueryScope.queryParameterallowedValues = ctrl.queryParameterallowedValues;
        objNewScalarQueryScope.addQuery = function () {
            var nodeQueries = new objElements();
            nodeQueries.Name = "Query";
            objNewScalarQueryScope.model.Elements.push(nodeQueries);
        };
        objNewScalarQueryScope.getQueryParam_dialog = function (queryObj) {
            var dialogScope = objNewScalarQueryScope.$new(true);
            dialogScope.DialogConstants = { queryParamsName: "sfwNavigationParameter" };
            dialogScope.mapObj = {};
            dialogScope.queryValues = objNewScalarQueryScope.queryParameterallowedValues;
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(queryObj.dictAttributes, "sfwQueryParameters", ";");
            dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Query Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
            dialogScope.setQueryParameters = function () {
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
                queryObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";");
                dialogScope.QueryDialog.close();
            };
        };
        objNewScalarQueryScope.onOKClick = function () {
            $rootScope.UndRedoBulkOp("Start");

            while (ctrl.model.baseChild.Elements.length > 0) {
                $rootScope.DeleteItem(ctrl.model.baseChild.Elements[0], ctrl.model.baseChild.Elements, null);
            }
            for (var idx = 0; idx < objNewScalarQueryScope.model.Elements.length; idx++) {
                $rootScope.PushItem(objNewScalarQueryScope.model.Elements[idx], ctrl.model.baseChild.Elements);
            }
            $rootScope.UndRedoBulkOp("End");

            objNewScalarQueryScope.closeDialog();
        };
        objNewScalarQueryScope.closeDialog = function () {
            if (objNewScalarQueryScope.dialog) {
                objNewScalarQueryScope.dialog.close();
            }
        };
        objNewScalarQueryScope.NavigateToEntityQuery = function (aQueryID) {
            if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                objNewScalarQueryScope.onOKClick();
                var query = aQueryID.split(".");
                $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
            }
        };
        objNewScalarQueryScope.deleteQuery = function () {
            var deleteNodeIndex = objNewScalarQueryScope.model.Elements.indexOf(objNewScalarQueryScope.selectedqueryNode);
            objNewScalarQueryScope.model.Elements.splice(deleteNodeIndex, 1);
            objNewScalarQueryScope.selectedqueryNode = undefined;
        };
        objNewScalarQueryScope.dialog = $rootScope.showDialog(objNewScalarQueryScope, "Set Scalar Queries", 'BPM/views/Association/SetScalarQueries.html', { height: 400, width: 800 });
    };
    //#region show Variables from respective Entity
    ctrl.showVariablesByEntity = function () {
        return function (item) {
            var retValue = false;
            if (ctrl.model.baseChild.dictAttributes.sfwEntity && ctrl.model.baseChild.dictAttributes.sfwEntity.trim().length > 0) {
                retValue = item.dictAttributes.sfwEntity === ctrl.model.baseChild.dictAttributes.sfwEntity;
            }
            return retValue;
        };
    };
    //#endregion
}

app.component('bpmServiceruleShapeAssociationProperties', {
    bindings: {
        shapeModel: '<',
        isDisabledforVersioning: '<?',
        queryParametersvalues: '<',
        queryParameterallowedValues: '<',
        checkAndUpdateParamChildModels: '&',
        onValueSourceChanged: '&',
        mapVariablesModel: "<",
        setParamEntityValue: "&",
        isObjectbased: "<?"
    },
    templateUrl: 'BPM/views/Association/ServiceRuleTaskAssociationProperties.html',
    controller: ["$rootScope", "$scope", "$filter", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", "$getEnitityObjectMethods", "$NavigateToFileService", "$getQueryparam", bpmServiceRuleAssociationPropertiesController]
});

function bpmSequenceFlowAssociationPropertiesController($rootScope, $scope) {
    var ctrl = this;
    ctrl.$onInit = function () {
        function filterextensionElements(x) {
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                function filterConditionExpression(y) {
                    if (y.Name == "conditionExpression") {
                        ctrl.gatewayOperator = y.dictAttributes;
                        function filterside(z) {
                            if (z.Name == "leftside") {
                                ctrl.leftSideExpression = z.dictAttributes;
                            }
                            else if (z.Name == "rightside") {
                                ctrl.rightSideExpression = z.dictAttributes;
                            }
                        }
                        y.Elements.forEach(filterside);
                        return true;
                    }
                }
                x.Elements.forEach(filterConditionExpression);
                return true;
            }
        }
        ctrl.model.Elements.some(filterextensionElements);
    };
}

app.component('bpmSequenceflowShapeAssociationProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        queryParameterallowedValues: '<',
        operatorsList: '<',
        queryParametersvalues: "<",
        setQueryParam: "&"
    },
    templateUrl: 'BPM/views/Association/GatewayExpressionAssociationProperties.html',
    controller: ["$rootScope", "$scope", bpmSequenceFlowAssociationPropertiesController]
});

function setValueByConstantVariableQueryController($rootScope, $getQueryparam) {
    var ctrl = this;
    ctrl.onTypechange = function () {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryID, ctrl.model, "sfwQueryID", "");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryParameters, ctrl.model, "sfwQueryParameters", "");
        $rootScope.EditPropertyValue(ctrl.model.value, ctrl.model, "value", "");
        $rootScope.UndRedoBulkOp("End");
    };
    ctrl.getParamfromQueryId = function (queryId) {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.value, ctrl.model, "value", "");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryParameters, ctrl.model, "sfwQueryParameters", $getQueryparam.get(queryId));
        $rootScope.UndRedoBulkOp("End");
    };
}

app.component('setValueByConstantVariableQuery', {
    bindings: {
        model: '<',
        setqueryparam: '&',
        direction: "<",
        isDisabledforVersioning: '<?',
        isLeftSideExpression: "<",
    },
    require: {
        parent: "^bpmSequenceflowShapeAssociationProperties"
    },
    templateUrl: 'BPM/views/Association/setValueByConstantVariableQuery.html',
    controller: ["$rootScope", "$getQueryparam", setValueByConstantVariableQueryController]
});
