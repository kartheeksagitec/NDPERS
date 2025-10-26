app.controller('BpmTerminationController', ["$scope", "$rootScope", "$filter", "$getQueryparam", "$getMultiIndexofArray", "$getEnitityRule", "$getEnitityXmlMethods", "$cssFunc", "$EntityIntellisenseFactory", "$NavigateToFileService", "$getEnitityObjectMethods", "hubcontext", function ($scope, $rootScope, $filter, $getQueryparam, $getMultiIndexofArray, $getEnitityRule, $getEnitityXmlMethods, $cssFunc, $EntityIntellisenseFactory, $NavigateToFileService, $getEnitityObjectMethods, hubcontext) {
    $scope.bpmModel = null;
    $scope.extraSettingsModel = null;
    $scope.terminationModel = null;
    $scope.terminationMethodModel = null;
    $scope.mapVariablesModel = null;
    $scope.xmlMethods = [];
    $scope.objectMethods = [];

    $scope.selectedEntity = "";
    $scope.selectedVariable = "";
    $scope.selectedXmlMethod = "";
    $scope.selectedXmlMethodParameters = "";
    $scope.selectedObjectMethod = "";
    $scope.selectedObjectMethodParameters = "";
    $scope.queryParametersValues = [];

    $scope.init = function () {
        $scope.selectedEntity = "";
        $scope.selectedVariable = "";
        $scope.selectedXmlMethod = "";
        $scope.selectedXmlMethodParameters = "";
        $scope.selectedObjectMethod = "";
        $scope.selectedObjectMethodParameters = "";
        $scope.xmlMethods = [];
        $scope.objectMethods = [];

        $scope.bpmModel = $scope.getBpmModel();
        $scope.mapVariablesModel = $scope.getMapVariables();
        $scope.queryParametersValues = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
        $scope.extraSettingsModel = $scope.getExtraSettingsModel();
        $scope.terminationModel = $scope.getTerminationModel();
        $scope.terminationMethodModel = $scope.getTerminationMethodModel();
        if ($scope.terminationModel) {
            if ($scope.terminationModel.dictAttributes.sfwEntity) {
                $scope.selectedEntity = $scope.terminationModel.dictAttributes.sfwEntity;
                $scope.onEntityChanged();
                if ($scope.terminationMethodModel) {
                    if ($scope.terminationMethodModel.dictAttributes.sfwExecuteOnTermination) {
                        $scope.selectedObjectMethod = $scope.terminationMethodModel.dictAttributes.sfwExecuteOnTermination;
                        $scope.onObjectMethodChanged();
                    }
                    if ($scope.terminationMethodModel.dictAttributes.sfwNavigationParameter) {
                        $scope.selectedObjectMethodParameters = $scope.terminationMethodModel.dictAttributes.sfwNavigationParameter;
                    }
                }
                var failOverModel = $scope.terminationModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "failoverbusinessobject"; })[0];
                var initializeModel = null;
                if (failOverModel && failOverModel.dictAttributes.variable && $scope.isVariablePresent(failOverModel.dictAttributes.variable, $scope.terminationModel.dictAttributes.sfwEntity)) {
                    $scope.selectedVariable = failOverModel.dictAttributes.variable;
                    $scope.onVariableChanged();
                    initializeModel = failOverModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "initialize"; })[0];
                }
                else {
                    initializeModel = $scope.terminationModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "initialize"; })[0];
                }
                if (initializeModel) {
                    var xmlMethodModel = initializeModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "xmlmethod"; })[0];
                    if (xmlMethodModel) {
                        if (xmlMethodModel.dictAttributes.ID) {
                            $scope.selectedXmlMethod = xmlMethodModel.dictAttributes.ID;
                            $scope.onXmlMethodChanged();
                        }
                        if (xmlMethodModel.dictAttributes.sfwNavigationParameter) {
                            $scope.selectedXmlMethodParameters = xmlMethodModel.dictAttributes.sfwNavigationParameter;
                        }
                    }
                }
            }
        }
    };
    $scope.isVariablePresent = function (variable, entity) {
        var present = false;
        if (variable && entity && $scope.mapVariablesModel) {
            present = $scope.mapVariablesModel.Elements.some(function (x) { return x.dictAttributes.id === variable && x.dictAttributes.sfwEntity === entity; });
        }
        return present;
    }
    $scope.getBpmModel = function () {
        var parentScope = $scope.$parent;
        var bpmModel = null;
        while (!bpmModel && parentScope) {
            bpmModel = parentScope.BPMNModel;
            parentScope = parentScope.$parent;
        }
        return bpmModel;
    };
    $scope.getExtraSettingsModel = function () {
        var extraSettingsModel = null;
        if ($scope.bpmModel) {
            extraSettingsModel = $scope.bpmModel.ExtensionElementsModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "extrasettings"; })[0];
        }
        return extraSettingsModel;
    };
    $scope.getTerminationModel = function () {
        var terminationModel = null;
        if ($scope.extraSettingsModel) {
            terminationModel = $scope.extraSettingsModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "termination"; })[0];
        }
        return terminationModel;
    };
    $scope.getTerminationMethodModel = function () {
        var terminationMethodModel = null;
        if ($scope.terminationModel) {
            terminationMethodModel = $scope.terminationModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "terminationmethod"; })[0];
        }
        return terminationMethodModel;
    };
    $scope.getMapVariables = function () {
        if ($scope.bpmModel) {
            return $scope.bpmModel.ExtensionElementsModel.Elements.filter(function (x) { return x.Name && x.Name.toLowerCase() === "mapvariables"; })[0];
        }
        return null;
    };
    $scope.onEntityChanged = function () {
        $scope.clearEntityDependents();
        if ($scope.selectedEntity) {
            $scope.getXmlMethods();
            $scope.getObjectMethods();
        }
    };
    $scope.clearEntityDependents = function () {
        $scope.xmlMethods = [];
        $scope.objectMethods = [];
        $scope.selectedObjectMethod = "";
        $scope.selectedObjectMethodParameters = "";
        $scope.selectedVariable = "";
        $scope.selectedXmlMethod = "";
        $scope.selectedXmlMethodParameters = "";
    }
    $scope.showVariablesByEntity = function () {
        return function (item) {
            var retValue = false;
            if ($scope.selectedEntity && $scope.selectedEntity.trim().length > 0) {
                retValue = item.dictAttributes.sfwEntity === $scope.selectedEntity;
            }
            return retValue;
        };
    };
    $scope.onVariableChanged = function () {
    };
    $scope.getXmlMethods = function () {
        $scope.xmlMethods = $getEnitityXmlMethods.get($scope.selectedEntity.trim());
    };
    $scope.onXmlMethodChanged = function () {
        $scope.getXmlMethodParameters();
    };
    $scope.getXmlMethodParameters = function () {
        // load navigation parameters and items 
        if ($scope.selectedXmlMethod) {
            var xmlmethod = $filter('filter')($scope.xmlMethods, { ID: $scope.selectedXmlMethod }, true)[0];
            if (xmlmethod) {
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(xmlmethod.Parameters, iterator, queryParametersDisplay);
                $scope.selectedXmlMethodParameters = queryParametersDisplay.join(",");
            }
        }
        else {
            $scope.selectedXmlMethodParameters = "";
        }
    };
    $scope.setXmlMethodParameters = function () {
        // open pop up for setting parameters
        var dialogScope = $scope.$new(true);
        dialogScope.DialogConstants = { queryParamsName: "selectedXmlMethodParameters" };
        dialogScope.mapObj = {};
        dialogScope.queryValues = $scope.queryParametersValues;
        dialogScope.queryParameters = $getQueryparam.getQueryparamfromString($scope, "selectedXmlMethodParameters", ",");

        dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Xml Method Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
        dialogScope.setQueryParameters = function () {
            var queryParametersDisplay = [];
            function iterator(value, key) {
                this.push(value.ID + '=' + (value.Value ? value.Value : ""));
            }
            angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
            $scope.selectedXmlMethodParameters = queryParametersDisplay.join(",");
            dialogScope.QueryDialog.close();
        };

        dialogScope.onRefreshParams = function () {
            var initializeModelXmlMethods = $getEnitityXmlMethods.get($scope.selectedEntity);
            if (initializeModelXmlMethods.length > 0) {
                var initializeXmlMethods = $filter('filter')(initializeModelXmlMethods, { ID: $scope.selectedXmlMethod }, true);
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
    $scope.getObjectMethods = function () {
        $scope.objectMethods = $getEnitityObjectMethods.get($scope.selectedEntity.trim());
    };
    $scope.onObjectMethodChanged = function () {
        $scope.getObjectMethodParameters();
    };
    $scope.getObjectMethodParameters = function () {
        // load navigation parameters and items 
        if ($scope.selectedObjectMethod) {
            var objectMethod = $filter('filter')($scope.objectMethods, { ID: $scope.selectedObjectMethod }, true)[0];
            if (objectMethod) {
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(objectMethod.Parameters, iterator, queryParametersDisplay);
                $scope.selectedObjectMethodParameters = queryParametersDisplay.join(",");
            }
        }
        else {
            $scope.selectedObjectMethodParameters = "";
        }
    };
    $scope.setObjectMethodParameters = function () {
        // open pop up for setting parameters
        var dialogScope = $scope.$new(true);
        dialogScope.DialogConstants = { queryParamsName: "selectedObjectMethodParameters" };
        dialogScope.mapObj = {};
        dialogScope.queryValues = $scope.queryParametersValues;
        dialogScope.queryParameters = $getQueryparam.getQueryparamfromString($scope, "selectedObjectMethodParameters", ",");

        dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Object Method Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
        dialogScope.setQueryParameters = function () {
            var queryParametersDisplay = [];
            function iterator(value, key) {
                this.push(value.ID + '=' + (value.Value ? value.Value : ""));
            }
            angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
            $scope.selectedObjectMethodParameters = queryParametersDisplay.join(",");
            dialogScope.QueryDialog.close();
        };
        dialogScope.onRefreshParams = function () {
            var arrObjectMethods = $getEnitityObjectMethods.get($scope.selectedEntity);
            if (arrObjectMethods.length > 0) {
                var objObjectMethod = $filter('filter')(arrObjectMethods, { ID: $scope.selectedObjectMethod }, true)[0];
                if (objObjectMethod) {

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
                    angular.forEach(objObjectMethod.Parameters, iterator, dialogScope.queryParameters);

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
        };
    };
    $scope.updateBpmModel = function () {

        //Prepare new termination node
        var newTerminationModel = null;


        //If set sfwEntity if exists, and accordingly set other dependent attributes.
        if ($scope.selectedEntity) {
            newTerminationModel = {
                Name: "termination",
                dictAttributes: {},
                Elements: []
            };

            var newTerminationMethodModel = {
                Name: "TerminationMethod",
                dictAttributes: { ObjectType: "ObjectMethod" },
                Elements: []
            };
            newTerminationModel.Elements.push(newTerminationMethodModel);

            newTerminationModel.dictAttributes.sfwEntity = $scope.selectedEntity;
            if ($scope.selectedObjectMethod) {
                newTerminationMethodModel.dictAttributes.sfwExecuteOnTermination = $scope.selectedObjectMethod;
                if ($scope.selectedObjectMethodParameters) {
                    newTerminationMethodModel.dictAttributes.sfwNavigationParameter = $scope.selectedObjectMethodParameters;
                }
            }

            //Prepare initialize and XmlMethod node.
            var initializeModel = { Name: "initialize", dictAttributes: { ObjectType: "XmlMethod" }, Elements: [] };
            var xmlMethodModel = { Name: "XmlMethod", dictAttributes: {}, Elements: [] };
            initializeModel.Elements.push(xmlMethodModel);

            //Set xml method and parameters if exists.
            if ($scope.selectedXmlMethod) {
                xmlMethodModel.dictAttributes.ID = $scope.selectedXmlMethod;
                if ($scope.selectedXmlMethodParameters) {
                    xmlMethodModel.dictAttributes.sfwNavigationParameter = $scope.selectedXmlMethodParameters;
                }
            }

            //If variable is set then prepare FailOverBusinessObject node and add initialze node as child, else add initialize node as child of extraSettings.
            if ($scope.selectedVariable) {
                var failOverModel = { Name: "FailOverBusinessObject", dictAttributes: {}, Elements: [] };
                failOverModel.dictAttributes.variable = $scope.selectedVariable;
                failOverModel.Elements.push(initializeModel);
                newTerminationModel.Elements.push(failOverModel);
            }
            else {
                newTerminationModel.Elements.push(initializeModel);
            }
        }

        if (!$scope.extraSettingsModel && newTerminationModel) {
            $scope.extraSettingsModel = {
                Name: "extraSettings", dictAttributes: {}, Elements: []
            };

            //If mapvariables node exists, then add extraSettings node above mapvariables, else add it to the last.
            if ($scope.mapVariablesModel) {
                var index = $scope.bpmModel.ExtensionElementsModel.Elements.indexOf($scope.mapVariablesModel);
                if (index > -1) {
                    $rootScope.InsertItem($scope.extraSettingsModel, $scope.bpmModel.ExtensionElementsModel.Elements, index);
                }
                else {
                    $rootScope.PushItem($scope.extraSettingsModel, $scope.bpmModel.ExtensionElementsModel.Elements);
                }
            }
            else {
                $rootScope.PushItem($scope.extraSettingsModel, $scope.bpmModel.ExtensionElementsModel.Elements);
            }
        }

        if ($scope.terminationModel) {
            var terminationModelIndex = $scope.extraSettingsModel.Elements.indexOf($scope.terminationModel);
            if (terminationModelIndex > -1) {
                if (newTerminationModel) {
                    $rootScope.InsertItem(newTerminationModel, $scope.extraSettingsModel.Elements, terminationModelIndex);
                }
                $rootScope.DeleteItem($scope.terminationModel, $scope.extraSettingsModel.Elements);
            }
            else {
                if (newTerminationModel) {
                    $rootScope.PushItem(newTerminationModel, $scope.extraSettingsModel.Elements);
                }
            }
        }
        else {
            if (newTerminationModel) {
                $rootScope.PushItem(newTerminationModel, $scope.extraSettingsModel.Elements);
            }
        }

        //if ($scope.extraSettingsModel && !$scope.extraSettingsModel.Elements.length) {
        //    $rootScope.DeleteItem($scope.extraSettingsModel, $scope.bpmModel.ExtensionElementsModel.Elements);
        //    $scope.extraSettingsModel = null;
        //}
    };
    $scope.$on("UpdateBpmTerminate", function (event, data) {
        $scope.updateBpmModel();
    });
    $scope.init();
}]);