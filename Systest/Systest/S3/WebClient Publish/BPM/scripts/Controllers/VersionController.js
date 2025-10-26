app.controller('VersionController', ["$scope", "$rootScope", "$filter", "$cssFunc", "$getQueryparam", "$getEnitityXmlMethods", "$getEnitityRule", "hubcontext", "$timeout", function ($scope, $rootScope, $filter, $cssFunc, $getQueryparam, $getEnitityXmlMethods, $getEnitityRule, hubcontext, $timeout) {
    $scope.init = function () {
        $scope.initVariables();
        $scope.initSvgVariables();
        $rootScope.IsLoading = true;
        if ($scope.$parent.calledCaseBpmModel) {
            $scope.receiveBpmnModel($scope.$parent.calledCaseBpmModel);
        }
        else {
            var arrayVersionName = $rootScope.currentopenfile.file.FileName.split("-");
            hubcontext.hubBPMN.server.getVersionBpmModel(arrayVersionName[1], arrayVersionName[2]).done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.receiveBpmnModel(data);
                });
            });
        }
    };
    $scope.initVariables = function () {
        $scope.versionList = [];
        $scope.activeTab = "MAP";
        $scope.currentfilename = $rootScope.currentopenfile.file.FileName;
        $scope.IsVersionDisplay = true;
        $scope.IsToolsDivCollapsed = true;
        $scope.IsDisabledforVersioning = true;
        $scope.associationActiveTab = "FORM";
        $scope.datatypes = ['bool', 'datetime', 'decimal', 'double', 'float', 'int', 'long', 'short', 'string', 'Object', 'BusinessObject'];
        $scope.types = ['local', 'in', 'out'];
        $scope.Requiredtypes = ['', 'true', 'false'];
        $scope.dvtypes = ['Query', 'Constant'];
        $scope.mandatoryElementIds = ['PersonId', 'OrgId', 'ReferenceId', 'Source', 'InstanceInfo1', 'InstanceInfo2'];
        $scope.queryParameterallowedValues = ['PersonId', 'OrgId', 'ReferenceId', 'Source'];
        $scope.currentUserTaskData = { enterFormData: { entityID: "", formTitle: "", controlTree: [], initialLoadModel: {}, visibleRules: [] }, reEnterFormData: { entityID: "", formTitle: "", controlTree: [], entities: [], buttons: [], initialLoadModel: {}, visibleRules: [] }, selectedEntity: "BpmActivityInstance", formList: [] };
        $scope.selectedCallActivityData = { reUsableBpm: [], CalledMapVariable: [] };
        $scope.operatorsList = ['GreaterThan', 'GreaterThanEqual', 'Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'StartsWith', 'EndsWith'];
        $scope.TargetSiteList = undefined;
        $scope.BPMNModel = undefined;
        $scope.objData = undefined;
        $scope.mapVariablesModel = undefined;
    };
    $scope.initSvgVariables = function () {
        $scope.svgNS = 'http://www.w3.org/2000/svg';
        // drag selected Element functions
        $scope.selectedElement = 0;
        $scope.currentX = 0;
        $scope.currentY = 0;
        $scope.selectedElementTop = 0;
        $scope.selectedElementLeft = 0;
        $scope.selectedElemstartLeft = 0;
        $scope.selectedElemstartTop = 0;
        $scope.indexofResizerElement;
        $scope.selectedElemPrevBounds = 0;
        $scope.isElementMoved = false;
        $scope.ClonedElement = null;
        $scope.ClonedResizer = null;
        $scope.LaneElement = null;
        $scope.SelectedElementLane = null;
        // drag Resizer For Element
        $scope.selectedResizergElement = 0;
        $scope.selectedElemForResize = 0;
        // Re-Arranging the Edges
        $scope.selectedlineEle = 0;
        $scope.linecount = 0;
        $scope.selectedLineSourceElement = 0;
        $scope.selectedLineTargetElement = 0;
        $scope.indexOfSelectedLine;
        $scope.ClonedLineElement = null;
        $scope.selectedlinegtagEle = 0;
        $scope.selectedlineindex = 0;
        $scope.isEdgeChanged = false;
        $scope.ClonedLineSourceElement = null;
        $scope.ClonedLineTargetElement = null;
        // Drawing New Edge
        $scope.sourceElementOnMouseHover = null;
        $scope.targetElementOnMouseHover = null;
        $scope.hoverElement = null;
        $scope.targetElementForLine = null;
        $scope._previousPosition;
        $scope.NewEdgeWayPoints;
        $scope.LaneIndex;
        $scope.PrevlaneElement = null;
        $scope.objDragOverLineElement = null;
    };
    $scope.receiveBpmnModel = function (data) {
        $scope.$evalAsync(function () {
            if (data != undefined) {
                $scope.objBPMExtraFields = undefined;
                $scope.BPMNModel = data;
                $scope.objData = $scope.BPMNModel;
                if ($scope.BPMNModel.Type != "BPMTemplate") {
                    $scope.mapVariablesModel = $filter('filter')($scope.BPMNModel.ExtensionElementsModel.Elements, { Name: "mapVariables" })[0];
                }
                $timeout(function () {
                    $scope.svgElement = $("div[id='versionBPMN-" + $scope.$id + "']").find("#mySVG")[0];
                    $scope.SetUniqueIdtoGradientsInsideSVG();
                    $scope.FilterTheElementsBasedOnTypeAndLoadShapes($scope.objData.lstShapes);
                }, 0);
                if ($scope.mapVariablesModel) {
                    $scope.queryParametersValues = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
                }
                $scope.ProcessTypes = [];
                $.connection.hubBPMN.server.getProcessTypes().done(function (result) {
                    $scope.ProcessTypes = result;
                });
            }
            $rootScope.IsLoading = false;
        });
    };
    $scope.SetUniqueIdtoGradientsInsideSVG = function () {
        for (var i = 0; i < $scope.svgElement.childNodes.length; i++) {
            if ($scope.svgElement.childNodes[i].nodeName == "defs") {
                for (var j = 0; j < $scope.svgElement.childNodes[i].childNodes.length; j++) {
                    if ($scope.svgElement.childNodes[i].childNodes[j].nodeName != "#text" && $scope.svgElement.childNodes[i].childNodes[j].nodeName != "#comment") {
                        $scope.svgElement.childNodes[i].childNodes[j].setAttribute("id", $scope.svgElement.childNodes[i].childNodes[j].getAttribute("id") + "_" + $rootScope.currentopenfile.file.FileName.trim());
                    }
                }
            }
        }
    };
    $scope.FilterTheElementsBasedOnTypeAndLoadShapes = function (lstShapes) {
        var lstpoolAndLane = [];
        var lstShapesElement = [];
        for (var i = 0; i < lstShapes.length; i++) {
            if (lstShapes[i].ShapeType == "participant" || lstShapes[i].ShapeType == "lane") {
                lstpoolAndLane.push(lstShapes[i]);
            }
            else {
                lstShapesElement.push(lstShapes[i]);
            }
        }
        $scope.LoadShapes($scope.svgElement, lstpoolAndLane);
        $scope.LoadShapes($scope.svgElement, lstShapesElement);
    };
    $scope.LoadShapes = function (svgElement, alstShapes) {
        for (var ind = 0; ind < alstShapes.length; ind++) {
            var objShape = alstShapes[ind];
            objShape.IsExecuted = true;
            if (objShape.ShapeName == "BPMNShape") {
                var isVerticalText = false;
                var isTask = false;
                switch (objShape.ShapeType) {
                    case "participant":
                    case "lane":
                        //drawSquareParam(acontext, objShape);
                        drawSquareParam(svgElement, objShape, $scope);
                        isVerticalText = true;
                        break;
                    case "task":
                    case "userTask":
                    case "serviceTask":
                    case "businessRuleTask":
                    case "manualTask":
                    case "callActivity":
                        drawTask(svgElement, objShape, $scope);
                        isTask = true;
                        break;
                    case "endEvent":
                    case "startEvent":
                    case "intermediateCatchEvent":
                        drawCircle(svgElement, objShape, $scope);
                        break;

                    case "exclusiveGateway":
                    case "inclusiveGateway":
                    case "parallelGateway":
                    case "eventBasedGateway":
                        drawDiamond(svgElement, objShape, $scope);
                        break;
                    case "dataStoreReference":
                        drawdataStoreReference(svgElement, objShape, $scope);
                        break;
                    case "dataObjectReference":
                        drawdataObjectReference(svgElement, objShape, $scope);
                        break;
                    case "textAnnotation":
                        drawtextAnnotation(svgElement, objShape, $scope);
                        break;
                    case "message":
                        drawInitiatingorNonInitiatingMessage(svgElement, objShape, $scope);
                        break;
                    default:
                        drawTask(svgElement, objShape, $scope);
                        break;
                }
            }
            if (objShape.ShapeName == "BPMNEdge") {

                AddLineArray(svgElement, objShape, $scope);
            }
        }
    };
    $scope.setActiveTab = function (tab) {
        $scope.DraggableElement = null;
        $scope.activeTab = tab;
    };
    $scope.selectVariable = function (variable) {
        $scope.selectedVariable = variable;
    };
    $scope.showEnterExitDialog = function (isEnter, model, entityId) {
        var dialogScope = $scope.$new();
        dialogScope.model = JSON.parse(JSON.stringify(model));
        dialogScope.mapvarids = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
        dialogScope.entityid = entityId;
        dialogScope.IsDisabledforVersioning = $scope.IsDisabledforVersioning;
        dialogScope.isObjectbased = $scope.BPMNModel.IsObjectBased;
        dialogScope.closeDialog = function () {
            if (dialogScope.dialog)
                dialogScope.dialog.close();
        };
        dialogScope.dialog = $rootScope.showDialog(dialogScope, "Set " + (isEnter ? "Enter" : "Exit") + " Actions", "BPM/views/Association/EnterExitTemplate.html", { width: 1100 });
    };
    $scope.checkAndUpdateParamChildModels = function (param, fromUndoRedoBlock) {
        var items = param.Elements.filter(function (item) { return item.Name == "value"; });
        if (items && items.length > 0) {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel, param, "valueModel", items[0]);
            }
            else {
                param.valueModel = items[0];
            }
        }
        else {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel, param, "valueModel", { prefix: "", Name: "value", Value: "", dictAttributes: {}, Elements: [], Children: [] });
                $rootScope.PushItem(param.valueModel, param.Elements);
            }
            else {
                param.valueModel = { prefix: "", Name: "value", Value: "", dictAttributes: {}, Elements: [], Children: [] };
                param.Elements.push(param.valueModel);
            }
        }
        items = param.valueModel.Elements.filter(function (item) { return item.Name == "BusinessObject"; });
        if (items && items.length > 0) {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel.businessObjectModel, param.valueModel, "businessObjectModel", items[0]);
            }
            else {
                param.valueModel.businessObjectModel = items[0];
            }
        }
        else {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel.businessObjectModel, param.valueModel, "businessObjectModel", { prefix: "", Name: "BusinessObject", Value: "", dictAttributes: {}, Elements: [], Children: [] });
                $rootScope.PushItem(param.valueModel.businessObjectModel, param.valueModel.Elements);
            }
            else {
                param.valueModel.businessObjectModel = { prefix: "", Name: "BusinessObject", Value: "", dictAttributes: {}, Elements: [], Children: [] };
                param.valueModel.Elements.push(param.valueModel.businessObjectModel);
            }
        }
        items = param.valueModel.businessObjectModel.Elements.filter(function (item) { return item.Name == "Initialize"; });
        if (items && items.length > 0) {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel.businessObjectModel.initializeModel, param.valueModel.businessObjectModel, "initializeModel", items[0]);
            }
            else {
                param.valueModel.businessObjectModel.initializeModel = items[0];
            }
        }
        else {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(param.valueModel.businessObjectModel.initializeModel, param.valueModel.businessObjectModel, "initializeModel", { prefix: "", Name: "Initialize", Value: "", dictAttributes: { ObjectType: "XmlMethod" }, Elements: [], Children: [] });
                $rootScope.PushItem(param.valueModel.businessObjectModel.initializeModel, param.valueModel.businessObjectModel.Elements);
            }
            else {
                param.valueModel.businessObjectModel.initializeModel = { prefix: "", Name: "Initialize", Value: "", dictAttributes: { ObjectType: "XmlMethod" }, Elements: [], Children: [] };
                param.valueModel.businessObjectModel.Elements.push(param.valueModel.businessObjectModel.initializeModel);
            }
        }
    };
    $scope.showDetails = function () {
        if ($scope.selectedShape) {
            var extensionElements = $filter('filter')($scope.selectedShape.ShapeModel.Elements, { Name: "extensionElements" });
            if (extensionElements && extensionElements.length > 0) {
                var items = $filter('filter')(extensionElements[0].Elements, { Name: "isSynchronous" });
                if (items && items.length > 0) {
                    $scope.selectedShape.ShapeModel.isSynchronousModel = items[0];
                }
            }
        }
    };
    $scope.init();
}]);