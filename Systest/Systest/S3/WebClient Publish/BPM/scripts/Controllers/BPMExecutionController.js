app.controller('BPMExecutionController', ["$scope", "$rootScope", "$filter", "$cssFunc", "$getQueryparam", "$getEnitityXmlMethods", "$getEnitityRule", "hubcontext", '$timeout', function ($scope, $rootScope, $filter, $cssFunc, $getQueryparam, $getEnitityXmlMethods, $getEnitityRule, hubcontext, $timeout) {
    // svg reconstruction code goes here  
    $scope.Guid = "";
    $scope.Guid = generateUUID();
    $scope.isIterationDialog = false;
    $scope.makeMapSVG = function () {
        if (!$scope.$parent.BPMExceutionTabDetails.isIteration) {
            $scope.ExecutedSteps = $scope.$parent.BPMExceutionTabDetails.ExecutedData.lstExecutedSteps;
        }

        $scope.lstExecutionTabShapes = $scope.$parent.BPMExceutionTabDetails.ExecutedData.lstShapes;
        $scope.isIterationDialog = $scope.$parent.BPMExceutionTabDetails.isIteration;
        if ($scope.isIterationDialog) {
            $scope.lstIterationsForTab = $scope.$parent.BPMExceutionTabDetails.ExecutedData.lstIterations;
            $scope.ExecutedSteps = $scope.$parent.BPMExceutionTabDetails.ExecutedData.lstIterations[0].lstExecutedSteps;
            $scope.objTabIteration = $scope.$parent.BPMExceutionTabDetails.ExecutedData.lstIterations[0];
        }
        $scope.FilterTheElementsBasedOnTypeAndLoadShapes($scope.lstExecutionTabShapes);

        if ($scope.$parent.BPMExceutionTabDetails.Mode == "Demo") {
            $scope.index = 0;
            $scope.ShowAnimationForExecutedSteps();
        }
    };

    $scope.onIterationTabClick = function (obj) {
        $scope.objTabIteration = obj;
        clearShapes($scope);
        $scope.ExecutedSteps = obj.lstExecutedSteps;
        $scope.FilterTheElementsBasedOnTypeAndLoadShapes($scope.lstExecutionTabShapes);
    };

    $scope.ShowAnimationForExecutedSteps = function () {
        if ($scope.index < $scope.ExecutedSteps.length) {
            $timeout(function () {
                if ($scope.ExecutedSteps[$scope.index].ExecutedShapeElement) {
                    $scope.FillColourToExecutedStep($scope.ExecutedSteps[$scope.index].ExecutedShapeElement);
                }
                $scope.index++;
                if ($scope.index < $scope.ExecutedSteps.length) {
                    if ($scope.ExecutedSteps[$scope.index - 1].ExecutedShapeElement && $scope.ExecutedSteps[$scope.index - 1].ExecutedShapeElement.getAttribute("id") != "callActivity") {
                        $scope.ShowAnimationForExecutedSteps();
                    }
                } else {
                    if ($scope.$parent.BPMExceutionTabDetails.isChildWindow) {
                        $timeout(function () {
                            $scope.close();
                        }, 300);
                    } else {
                        alert("Execution Completed");
                    }
                }
            }, 300);
        }
    };

    $scope.FillColourToExecutedStep = function (ExecutedShape) {
        var ShapeType = ExecutedShape.getAttribute("id");
        switch (ShapeType) {
            case "task":
            case "userTask":
            case "serviceTask":
            case "businessRuleTask":
            case "manualTask":
            case "callActivity":
                for (var i = 0; i < ExecutedShape.childNodes.length; i++) {
                    if (ExecutedShape.childNodes[i].nodeName == "rect") {
                        shadowcolor = "url(#rectshadow_" + $scope.Guid + ")";
                        fillcolor = "url(#task_" + $scope.Guid + ")";
                        ExecutedShape.childNodes[i].setAttributeNS(null, "fill", fillcolor);
                        ExecutedShape.childNodes[i].setAttributeNS(null, "filter", shadowcolor);
                        break;
                    }
                }
                if (ShapeType == "callActivity") {
                    for (var i = 0; i < $scope.lstExecutionTabShapes.length; i++) {
                        if ($scope.lstExecutionTabShapes[i].Left == parseFloat(ExecutedShape.getAttribute("x")) && $scope.lstExecutionTabShapes[i].Top == parseFloat(ExecutedShape.getAttribute("y"))) {
                            $scope.selectedShape = $scope.lstExecutionTabShapes[i];
                            break;
                        }
                    }
                    if ($scope.selectedShape && $scope.selectedShape.ShapeType == "callActivity") {
                        $scope.showDetails();
                    }
                }
                break;
            case "endEvent":
            case "startEvent":
            case "intermediateCatchEvent":
                for (var i = 0; i < ExecutedShape.childNodes.length; i++) {
                    if (ExecutedShape.childNodes[i].nodeName == "circle") {
                        var fillcolor = "";
                        if (ShapeType == "startEvent") {
                            fillcolor = "url(#start_" + $scope.Guid + ")";
                        } else if (ShapeType == "endEvent") {
                            fillcolor = "url(#end_" + $scope.Guid + ")";
                        } else if (ShapeType == "intermediateCatchEvent") {
                            fillcolor = "url(#intermediate_" + $scope.Guid + ")";
                        }
                        ExecutedShape.childNodes[i].setAttributeNS(null, "fill", fillcolor);
                        break;
                    }
                }
                break;

            case "exclusiveGateway":
            case "inclusiveGateway":
            case "parallelGateway":
            case "eventBasedGateway":
                for (var i = 0; i < ExecutedShape.childNodes.length; i++) {
                    if (ExecutedShape.childNodes[i].nodeName == "polygon") {
                        var fillcolor = "";
                        fillcolor = "url(#exclusive_" + $scope.Guid + ")";
                        ExecutedShape.childNodes[i].setAttributeNS(null, "fill", fillcolor);
                        break;
                    }
                }
                break;
            case "line":
                for (var i = 0; i < ExecutedShape.childNodes.length; i++) {
                    if (ExecutedShape.childNodes[i].getAttribute("elementtype") == "linegroup") {
                        for (var j = 0; j < ExecutedShape.childNodes[i].childNodes.length; j++) {
                            if (ExecutedShape.childNodes[i].childNodes[j].nodeName == "line" && ExecutedShape.childNodes[i].childNodes[j].getAttribute("stroke-width") != 15) {
                                ExecutedShape.childNodes[i].childNodes[j].setAttributeNS(null, "stroke", "black");
                            }
                        }
                    }
                }
                break;
        }
    };

    $scope.init = function () {
        $scope.initVariables();
        $scope.initSvgVariables();
        //$rootScope.IsLoading = true;

        $timeout(function () {
            $scope.$apply(function () {
                $scope.Guid;
            });
            $scope.svgElement = $("div[id='" + $scope.Guid + "']").contents().find("#ExecutionSVG")[0];
            $scope.SetUniqueIdtoGradientsInsideSVG();
            $scope.makeMapSVG();
        });
        //$rootScope.IsLoading = false;
    };
    $scope.initVariables = function () {
        $scope.versionList = [];
        $scope.activeTab = "EXECUTION";
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
        $scope.PaticipantIndex;
        $scope.LaneIndex;
        $scope.PrevlaneElement = null;
        $scope.objDragOverLineElement = null;
    };
    $scope.SetUniqueIdtoGradientsInsideSVG = function () {
        for (var i = 0; i < $scope.svgElement.childNodes.length; i++) {
            if ($scope.svgElement.childNodes[i].nodeName == "defs") {
                for (var j = 0; j < $scope.svgElement.childNodes[i].childNodes.length; j++) {
                    if ($scope.svgElement.childNodes[i].childNodes[j].nodeName != "#text" && $scope.svgElement.childNodes[i].childNodes[j].nodeName != "#comment") {
                        $scope.svgElement.childNodes[i].childNodes[j].setAttribute("id", $scope.svgElement.childNodes[i].childNodes[j].getAttribute("id") + "_" + $scope.Guid);
                    }
                }
            }
        }
    };
    $scope.FilterTheElementsBasedOnTypeAndLoadShapes = function (lstShapes) {
        var objSvgBounds = { Height: 2500, Width: 2500 },
         objSvgLimits = $scope.getMaxHeightAndWidthofSvg(lstShapes);
        if (objSvgLimits.Height && objSvgLimits.Height > objSvgBounds.Height) {
            objSvgBounds.Height = objSvgLimits.Height + 100;
        }
        if (objSvgLimits.Width && objSvgLimits.Width > objSvgBounds.Width) {
            objSvgBounds.Width = objSvgLimits.Width + 100;
        }
        $scope.svgElement.setAttribute("height", objSvgBounds.Height);
        $scope.svgElement.setAttribute("width", objSvgBounds.Width);

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

    $scope.ShowVariables = function () {
        var ExecutedStep = $scope.GetExecutedstep($scope.selectedShape.Id);
        if (ExecutedStep) {
            if (ExecutedStep.ParametersSnapShot) {
                $.connection.hubBPMN.server.loadParametersFromSnapShot(ExecutedStep.ParametersSnapShot).done(function (result) {
                    $scope.$evalAsync(function () {
                        $scope.lstParametersofExecutedStep = result;
                        if ($scope.lstParametersofExecutedStep) {
                            var ExecutionVariableScope = $scope.$new();
                            ExecutionVariableScope.selectedExecutedStep = "";
                            ExecutionVariableScope.lstParametersofExecutedStep = $scope.lstParametersofExecutedStep;
                            ExecutionVariableScope.templateName = "BPM/views/Execution/ExecutionVariablesDialog.html";
                            ExecutionVariableScope.dialogOptions = { height: 500, width: 650, showclose: true };
                            ExecutionVariableScope.dialog = $rootScope.showDialog(ExecutionVariableScope, "Parameters", ExecutionVariableScope.templateName, ExecutionVariableScope.dialogOptions);
                            ExecutionVariableScope.close = function () {
                                $scope.$evalAsync(function () {
                                    ExecutionVariableScope.dialog.close();
                                });
                            };

                            ExecutionVariableScope.SelectExecutedStep = function (step) {
                                ExecutionVariableScope.selectedExecutedStep = step;
                            };
                        }
                    });
                });
            }
        }
    };


    $scope.showDetails = function () {
        var isIterationWindowShow = false;
        if (event) {
            $scope.$parent.BPMExceutionTabDetails.Mode = "Normal";
            var shapeElement = event.currentTarget;
            if (shapeElement.iterationCount > 1) {
                isIterationWindowShow = true;
                $scope.ShowIterationWindow();
            }
        }
        if ($scope.selectedShape && !isIterationWindowShow) {
            if ($scope.selectedShape.ShapeType == "callActivity") {
                var shapeModel = $scope.selectedShape.ShapeModel;
                var calledElement = "";
                for (var i = 0; i < shapeModel.Elements.length; i++) {
                    if (shapeModel.Elements[i].Elements && shapeModel.Elements[i].Elements.length > 0) {
                        for (var j = 0; j < shapeModel.Elements[i].Elements.length; j++) {
                            if (shapeModel.Elements[i].Elements[j].Name && shapeModel.Elements[i].Elements[j].Name == "calledCaseBpmMap") {
                                calledElement = shapeModel.Elements[i].Elements[j].Value;
                                break;
                            }
                        }
                    }
                }
                if (calledElement) {
                    if (calledElement.length > 32000) {
                        var strpacket = "";
                        var lstDataPackets = [];
                        var count = 0;
                        for (var i = 0; i < calledElement.length; i++) {
                            count++;
                            strpacket = strpacket + calledElement[i];
                            if (count == 32000) {
                                count = 0;
                                lstDataPackets.push(strpacket);
                                strpacket = "";
                            }
                        }
                        if (count != 0) {
                            lstDataPackets.push(strpacket);
                        }
                        SendDataPacketsToServer(lstDataPackets);
                    } else {
                        hubMain.server.getExecutedBPMModel(calledElement).done(function (data) {
                            if (data) {
                                var objData = {};
                                var exeSteps = $scope.getExecutedStepsForCallElement($scope.selectedShape.Id);
                                objData.lstShapes = data.lstShapes;
                                if (exeSteps) {
                                    objData.lstExecutedSteps = exeSteps;
                                    $scope.ShowExecutedDailog(objData);
                                }

                            }
                        });
                    }
                }
            }
        }
    };

    $scope.ShowIterationWindow = function () {
        $scope.lstIteration = [];
        var objIteration = null;
        var blnMatchId = false;
        var iIteration = 1;
        for (var i = 0; i < $scope.ExecutedSteps.length; i++) {
            if (!blnMatchId) {
                if ($scope.selectedShape.Id == $scope.ExecutedSteps[i].ElementId) {
                    blnMatchId = true;
                }
            }

            if (blnMatchId) {
                if ($scope.ExecutedSteps[i].ElementId == $scope.selectedShape.Id) {
                    objIteration = {};
                    objIteration.Id = "iteration" + iIteration;
                    objIteration.lstExecutedSteps = [];
                    $scope.lstIteration.push(objIteration);
                    iIteration++;
                }

                if (objIteration != null) {
                    objIteration.lstExecutedSteps.push($scope.ExecutedSteps[i]);
                }
            }
        }

        //Showing iteration window 
        //$scope.$apply(function () {
        $scope.OpenIterationWindowDialog($scope.lstIteration);
        //});
    };

    $scope.OpenIterationWindowDialog = function (lstIterations) {
        $scope.BPMExceutionTabDetails = {};
        $scope.BPMExceutionTabDetails.ExecutedData = {};
        $scope.BPMExceutionTabDetails.ExecutedData.lstIterations = lstIterations;
        $scope.BPMExceutionTabDetails.ExecutedData.lstShapes = $scope.lstExecutionTabShapes;
        $scope.BPMExceutionTabDetails.isIteration = true;
        $scope.BPMExceutionTabDetails.Mode = $scope.$parent.BPMExceutionTabDetails.Mode;
        var DialogIterationElementScope = $scope.$new();
        DialogIterationElementScope.BPMExceutionTabDetails = $scope.BPMExceutionTabDetails;
        DialogIterationElementScope.templateName = "BPM/views/Execution/BPMExecutionDialog.html";
        DialogIterationElementScope.dialogOptions = { height: 700, width: 1300, showclose: true };
        DialogIterationElementScope.dialog = $rootScope.showDialog(DialogIterationElementScope, "Iteration Result", DialogIterationElementScope.templateName, DialogIterationElementScope.dialogOptions);
        DialogIterationElementScope.close = function () {
            $scope.$evalAsync(function () {
                DialogIterationElementScope.dialog.close();
                if ($scope.$parent.BPMExceutionTabDetails.Mode == "Demo") {
                    $scope.ShowAnimationForExecutedSteps();
                }
            });

        };
    };

    $scope.getExecutedStepsForCallElement = function (id) {
        if ($scope.ExecutedSteps.length > 0) {
            for (var i = 0; i < $scope.ExecutedSteps.length; i++) {
                if (id == $scope.ExecutedSteps[i].ElementId) {
                    var ExecutedSteps = $scope.ExecutedSteps[i].lstExecutedSteps;
                    return ExecutedSteps;
                }
            }
        }
    };

    $scope.ShowExecutedDailog = function (data) {
        if (data) {
            $scope.BPMExceutionTabDetails = {};
            $scope.BPMExceutionTabDetails.ExecutedData = data;
            $scope.BPMExceutionTabDetails.Mode = $scope.$parent.BPMExceutionTabDetails.Mode;
            $scope.BPMExceutionTabDetails.isChildWindow = true;
            var DialogCalledElementScope = $scope.$new();
            DialogCalledElementScope.BPMExceutionTabDetails = $scope.BPMExceutionTabDetails;
            DialogCalledElementScope.templateName = "BPM/views/Execution/BPMExecutionDialog.html";
            DialogCalledElementScope.dialogOptions = { height: 700, width: 1300, showclose: true };
            DialogCalledElementScope.dialog = $rootScope.showDialog(DialogCalledElementScope, "CallActivity Result", DialogCalledElementScope.templateName, DialogCalledElementScope.dialogOptions);
            DialogCalledElementScope.close = function () {
                $scope.$evalAsync(function () {
                    DialogCalledElementScope.dialog.close();
                    if ($scope.$parent.BPMExceutionTabDetails.Mode == "Demo") {
                        $scope.ShowAnimationForExecutedSteps();
                    }
                });

            };
        }
    };

    var SendDataPacketsToServer = function (lstpackets) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPacketsForBPMNModel(lstpackets[i], lstpackets.length, i).done(function (data) {
                if (data) {
                    var objData = {};
                    var exeSteps = $scope.getExecutedStepsForCallElement($scope.selectedShape.Id);
                    objData.lstShapes = data.lstShapes;
                    if (exeSteps) {
                        objData.lstExecutedSteps = exeSteps;
                        $scope.ShowExecutedDailog(objData);
                    }
                }
            });
        }
    };

    $scope.init();

    $scope.CheckStepisExceutedOrNot = function (id, shapeElement) {
        var retValue = false;
        if ($scope.ExecutedSteps.length > 0) {
            for (var i = 0; i < $scope.ExecutedSteps.length; i++) {
                if (id == $scope.ExecutedSteps[i].ElementId) {
                    retValue = true;
                    $scope.ExecutedSteps[i].ExecutedShapeElement = shapeElement;
                    var iterationCount = 0;
                    var lstofsameobj = $scope.ExecutedSteps.filter(function (x) { return x.ElementId == id; });
                    if (lstofsameobj && lstofsameobj.length > 0) {
                        iterationCount = lstofsameobj.length;
                    }
                    shapeElement.iterationCount = iterationCount;
                    break;
                }
            }
        } else {
            return false;
        }

        return retValue;
    };


    $scope.GetExecutedstep = function (id) {
        var retValue = "";
        if ($scope.ExecutedSteps.length > 0) {
            for (var i = 0; i < $scope.ExecutedSteps.length; i++) {
                if (id == $scope.ExecutedSteps[i].ElementId) {
                    retValue = $scope.ExecutedSteps[i];
                    break;
                }
            }
        }
        return retValue;
    };


}]);

function getHtmlForContextMenu(selectedShape) {
    var htmltext = "";
    if (selectedShape.ShapeName == "BPMNShape") {
        switch (selectedShape.ShapeType) {
            case "participant":
                htmltext = '<li onclick="AddLaneToPoolFromContextMenu(event)">Add Lane To Pool</li>';
                break;
            case "callActivity":
                htmltext = '<li onclick="OnOpenCalledElementClick(event)">Open Called Process</li>';
                break;
            case "task":
            case "userTask":
            case "serviceTask":
            case "businessRuleTask":
            case "manualTask":
                htmltext = '<li> Task Type >> <ul><li onclick="OnChangeTaskTypeClick(event,\'Task\')"><img src="images/BPM/Bpm_Contextmenu_Task.svg" /> Task</li><li onclick="OnChangeTaskTypeClick(event,\'Service Task\')"><img src="images/BPM/Bpm_Contextmenu_Service_task.svg" />Service Task</li><li onclick="OnChangeTaskTypeClick(event,\'User Task\')"><img src="images/BPM/Bpm_Contextmenu_User_task.svg" />User Task</li><li onclick="OnChangeTaskTypeClick(event,\'Business Rule Task\')"><img src="images/BPM/Bpm_Contextmenu_Business_rule_task.svg" />Business Rule Task</li></ul></li>';
                break;
            case "intermediateCatchEvent":
                htmltext = '<li> Event Type >> <ul><li onclick="OnChangeCatchEventClick(event,\'Message Event\')">Message Event</li><li onclick="OnChangeCatchEventClick(event,\'Timer Event\')">Timer Event</li></ul></li>';
                break;
            case "endEvent":
            case "startEvent":
                break;
            case "exclusiveGateway":
            case "inclusiveGateway":
            case "parallelGateway":
            case "eventBasedGateway":
                htmltext = '<li> Gateway Type >> <ul><li  onclick="OnChangeGatewayTypeClick(event,\'Exclusive Gateway\')">Exclusive Gateway</li><li onclick="OnChangeGatewayTypeClick(event,\'Inclusive Gateway\')">Inclusive Gateway</li><li onclick="OnChangeGatewayTypeClick(event,\'Parallel Gateway\')">Parallel Gateway</li><li onclick="OnChangeGatewayTypeClick(event,\'Event Gateway\')">Event Gateway</li></ul></li>';
                break;
            case "dataStoreReference":
                break;
            case "dataObjectReference":
                break;
            case "textAnnotation":
                break;
            case "message":
                break;
            default:
                break;
        }
    }
    if (selectedShape.ShapeName == "BPMNEdge") {
        htmltext = '<li onclick="OnFixEdgeButtonClick(event)">Fix Edge</li>';
        if (selectedShape.ShapeType == "messageFlow") {
            htmltext += '<li>Message Kind >> <ul><li onclick="OnMessageKindChangeClick(event,\'initiating\')">Initiating</li><li onclick="OnMessageKindChangeClick(event,\'non_initiating\')">Non-Initiating</li><li onclick="OnMessageKindChangeClick(event,\'None\')">None</li></ul> </li>';
        }
    }
    htmltext += '<li onclick="OnDeleteButtonClick(event)">Delete</li>';
    return htmltext;
}

var scopeforexecutionvaraibles;
function showContextMenuForSVG(event) {
    scopeforexecutionvaraibles = angular.element(event.currentTarget).scope();
    event.preventDefault();
    event.stopPropagation();
    var htmltext = "";
    if (scopeforexecutionvaraibles.activeTab == "MAP" && !scopeforexecutionvaraibles.IsVersionDisplay) {
        htmltext = '<li onclick="EditSVGBoundsButtonClick(event)">Edit SVG Height/Width</li>';
    }
    $("#container-menu").find("li").remove();
    $("#container-menu").append(htmltext);
    // Show contextmenu
    $("#container-menu").finish().toggle(100). // In the right position (the mouse)
    css({
        top: event.pageY + "px",
        left: event.pageX + "px"
    });
    $(".page-header-fixed").css("pointer-events", "none");
    $("#container-menu").css("pointer-events", "auto");
}

function showContextMenu(event) {
    scopeforexecutionvaraibles = angular.element(event.currentTarget).scope();
    event.preventDefault();
    event.stopPropagation();
    if (event.currentTarget.id != 'line') {
        selectElement(event, event.currentTarget);
    } else {
        selectedlineElement(event, event.target);
    }
    var htmltext = "";
    if (scopeforexecutionvaraibles.activeTab == "MAP" && !scopeforexecutionvaraibles.IsVersionDisplay) {
        if (scopeforexecutionvaraibles.selectedShape) {
            htmltext = getHtmlForContextMenu(scopeforexecutionvaraibles.selectedShape);
        }
    } else if (scopeforexecutionvaraibles.activeTab == "EXECUTION") {
        htmltext = '<li onclick="ShowVariables(event)">Show Varaibles</li>';
    }
    $("#container-menu").find("li").remove();
    $("#container-menu").append(htmltext);
    // Show contextmenu
    $("#container-menu").finish().toggle(100). // In the right position (the mouse)
    css({
        top: event.pageY + "px",
        left: event.pageX + "px"
    });
    $(".page-header-fixed").css("pointer-events", "none");
    $("#container-menu").css("pointer-events", "auto");
}

function ShowVariables(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.$evalAsync(function () {
            scopeforexecutionvaraibles.ShowVariables();
            scopeforexecutionvaraibles = undefined;
        });
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnDeleteButtonClick(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.$evalAsync(function () {
            scopeforexecutionvaraibles.isDirty = true;
            scopeforexecutionvaraibles.onDeleteKeyDown(e, "Delete");
        });
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function EditSVGBoundsButtonClick(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.$evalAsync(function () {
            scopeforexecutionvaraibles.EditSVGheightandwidth(e);
        });
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function AddLaneToPoolFromContextMenu(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        AddLaneToPool(e, scopeforexecutionvaraibles);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnOpenCalledElementClick(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        OpenCalledElement(scopeforexecutionvaraibles);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnChangeTaskTypeClick(e, param) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        ChangeTaskFromContextMenu(scopeforexecutionvaraibles, param);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnChangeGatewayTypeClick(e, param) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        ChangeGatewayFromContextMenu(scopeforexecutionvaraibles, param);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnChangeCatchEventClick(e, param) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        ChangeEventTypeFromContextMenu(scopeforexecutionvaraibles, param);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnFixEdgeButtonClick(e) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        FixEdgeFromContextMenu(scopeforexecutionvaraibles);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}

function OnMessageKindChangeClick(e, param) {
    if (scopeforexecutionvaraibles) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        scopeforexecutionvaraibles.isDirty = true;
        OnChangeMessageKindFromContextMenu(scopeforexecutionvaraibles, param);
    }
    $(".custom-menu").hide(100);
    $(".page-header-fixed").css("pointer-events", "auto");
}
// If the document is clicked somewhere
$(document).bind("mousedown", function (e) {

    // If the clicked element is not the menu
    if (!$(e.target).parents(".custom-menu").length > 0) {

        // Hide it
        $(".custom-menu").hide(100);
        scopeforexecutionvaraibles = undefined;
        $(".page-header-fixed").css("pointer-events", "auto");
    }
});







