// Adding Element from ToolBar
function AddNewElementToBPM(evt) {
    onBodyClick();
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    var x = evt.offsetX;
    var y = evt.offsetY;
    if (scope.DraggableElement != null) {
        if (scope.activeTab == "MAP") {
            DrawShape(scope.DraggableElement, evt, scope);
        }
        scope.DraggableElement = null;
        removeSelection('NewElement', scope);
        removeSelectionForLane(scope);
        removeRearrangingLinePoints(scope);
        removeResizerAroundElement(scope);
        scope.selectedShape = undefined;
        scope.$apply();
    }
    scope.svgElement.removeAttributeNS(null, "onmouseup");
    angular.element(scope.svgElement.parentNode).removeAttr("ondrop");
    angular.element(scope.svgElement.parentNode).removeAttr("ondragover");
}

function DrawShape(ElementName, event, scope) {
    var shapeProcRef = getProcessRefId(event, scope);
    if (ElementName == "Pool") {
        CreatePoolandLaneObjects(event, scope);
        scope.isDirty = true;
    }
    else if (shapeProcRef) {
        scope.isDirty = true;
        if (ElementName == "Lane") {
            CreateLaneObject(event, scope, shapeProcRef);
        }
        else if (ElementName == "Start Event" || ElementName == "End Event" || ElementName == "Intermediate Catch Event") {
            CreateEventObject(event, scope, ElementName, false, shapeProcRef);
        }
        else if (ElementName == "Exclusive Gateway" || ElementName == "Inclusive Gateway" || ElementName == "Parallel Gateway" || ElementName == "Event Gateway") {
            CreateGatewayObject(event, scope, ElementName, false, shapeProcRef);
        }
        else if (ElementName == "Task" || ElementName == "Manual Task" || ElementName == "User Task" || ElementName == "Business Rule Task" || ElementName == "Service Task") {
            CreateTaskObject(event, scope, ElementName, false, shapeProcRef);
        }
        else if (ElementName == "Data Store") {
            CreateDataStoreObject(event, scope, shapeProcRef);
        }
        else if (ElementName == "Data object") {
            CreateDataObjReferenceObject(event, scope, shapeProcRef);
        }
        else if (ElementName == "Text Annotation") {
            CreateTextAnnotationObject(event, scope, false, shapeProcRef);
        }
        else if (ElementName == "SubProcess") {

        }
        else if (ElementName == "Calling Process") {
            CreateCallActivityObject(event, scope, shapeProcRef);
        }
    } else if (ElementName == "Initiating Message" || ElementName == "Non Initiating Message") {
        scope.isDirty = true;
        if (scope.DragOverElement != null) {
            CreateMessageReferenceObject(event, scope, ElementName);
            resetNodeProperties(scope.DragOverElement, scope);
            scope.DragOverElement = null;
        }
    }
}

function CreatePoolandLaneObjects(event, scope) {
    var procRef = "PROCESS_" + generateUUID();
    var poolId = "PARTICIPANT_" + generateUUID();
    var shapeId = generateUUID();
    var objShapeDetailsOfPool = {
        Height: 200, Width: 800, Left: event.offsetX, Top: event.offsetY, LabelHeight: 21, LabelWidth: 62, LabelLeft: 0, LabelTop: 0,
        ShapeName: "BPMNShape", ShapeType: "participant", processRef: procRef, lstWayPoints: [], Text: "Participant", Id: poolId, ShapeId: shapeId
    };

    var poolShapeModel = {
        dictAttributes: { id: poolId, name: "Participant", processRef: procRef }, Elements: [{ dictAttributes: { minimum: "0", maximum: "1" }, Elements: [], Children: [], Name: "participantMultiplicity", prefix: null, Value: "" },
        { dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: "" }], Children: [],
        Name: "participant", prefix: null, Value: ""
    };

    var processModel = {
        dictAttributes: { id: procRef, isClosed: "false", isExecutable: "true", processType: "None", name: "Participant" },
        Elements: [{
            dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [], Children: [], Name: "extraSettings", prefix: "",
            IsValueInCDATAFormat: false, Value: ""
        }, { dictAttributes: {}, Elements: [], "Children": [], "Name": "laneSet", "prefix": "", "IsValueInCDATAFormat": false, "Value": "" }], "Children": [], "Name": "process", "prefix": "", "IsValueInCDATAFormat": false, "Value": ""
    };

    objShapeDetailsOfPool.ProcessModel = processModel;
    objShapeDetailsOfPool.ShapeModel = poolShapeModel;
    scope.objData.lstShapes.push(objShapeDetailsOfPool);

    //Add process ref to list of process ids in model.
    scope.objData.Processes.push(procRef);
    drawSquareParam(scope.svgElement, objShapeDetailsOfPool, scope);
    scope.PaticipantIndex = undefined;
    CreateLaneObject(event, scope, procRef);
}

function CreateLaneObject(event, scope, procRef) {
    var LaneXpos = "";
    var LaneYpos = "";

    if (scope.PaticipantIndex || scope.PaticipantIndex == 0) {
        LaneXpos = scope.objData.lstShapes[scope.PaticipantIndex].Left + 40;
        LaneYpos = scope.objData.lstShapes[scope.PaticipantIndex].Top + scope.objData.lstShapes[scope.PaticipantIndex].Height;
    }

    if (procRef != "") {
        var LaneId = "Lane_" + generateUUID();
        var shapeId = generateUUID();
        var objShapeDetailsOfLane = {
            Height: 200, Width: 760, Left: LaneXpos, Top: LaneYpos, LabelHeight: 21, LabelWidth: 62, LabelLeft: 0, LabelTop: 0,
            ShapeName: "BPMNShape", ShapeType: "lane", processRef: procRef, lstWayPoints: [], Text: "Lane", Id: LaneId, ShapeId: shapeId
        };
        if (LaneXpos == "" && LaneYpos == "") {
            objShapeDetailsOfLane.Left = event.offsetX + 40;
            objShapeDetailsOfLane.Top = event.offsetY;
        }
        else {
            scope.objData.lstShapes[scope.PaticipantIndex].Height = scope.objData.lstShapes[scope.PaticipantIndex].Height + objShapeDetailsOfLane.Height;
            objShapeDetailsOfLane.Width = scope.objData.lstShapes[scope.PaticipantIndex].Width - 40;
        }

        var LaneShapeModel = {
            dictAttributes: { id: LaneId, name: "Lane" }, Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "userrole", prefix: "sbpmn", Value: "" },
                { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            }],
            Children: [], Name: "lane", prefix: null, Value: ""
        };

        objShapeDetailsOfLane.ShapeModel = LaneShapeModel;
        scope.objData.lstShapes.push(objShapeDetailsOfLane);
        //drawSquareParam(scope.svgElement, objShapeDetailsOfLane, scope);
        RedrawingElementsAndEdges(scope);
    }
}

function ChangeCatchEventImageBasedonType(selectedElement, param) {
    for (var i = 0; i < selectedElement.childNodes.length; i++) {
        if (selectedElement.childNodes[i].nodeName == "image") {
            if (param == "Message") {
                selectedElement.childNodes[i].setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/messageEventDefinition.svg');
            } else if (param == "Timer") {
                selectedElement.childNodes[i].setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/timerEventDefinition.svg');
            } else {
                selectedElement.childNodes[i].setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/intermediateCatchEvent.svg');
            }
        }
    }
}
function DrawCatchEventSvg(newimage) {
    var svgNS = "http://www.w3.org/2000/svg";

    var newimagecircle = document.createElementNS(svgNS, "circle");
    newimagecircle.setAttribute("fill", "none");
    newimagecircle.setAttribute('stroke', "#333333");
    newimagecircle.setAttribute('stroke-width', "0.5");
    newimagecircle.setAttribute('stroke-miterlimit', "10");
    newimagecircle.setAttribute('cx', "12.912");
    newimagecircle.setAttribute('cy', "12.646");
    newimagecircle.setAttribute('r', '9.25');
    newimage.appendChild(newimagecircle);

}


function DrawTimerEventSvg(newimage) {
    var svgNS = "http://www.w3.org/2000/svg";

    var newimagecircle = document.createElementNS(svgNS, "circle");
    newimagecircle.setAttribute("fill", "none");
    newimagecircle.setAttribute('stroke', "#333333");
    newimagecircle.setAttribute('stroke-width', "0.5");
    newimagecircle.setAttribute('stroke-miterlimit', "10");
    newimagecircle.setAttribute('cx', "12.912");
    newimagecircle.setAttribute('cy', "12.646");
    newimagecircle.setAttribute('r', '11.5');
    newimage.appendChild(newimagecircle);

    var newimagecircle1 = document.createElementNS(svgNS, "circle");
    newimagecircle1.setAttribute("fill", "none");
    newimagecircle1.setAttribute('stroke', "#333333");
    newimagecircle1.setAttribute('stroke-width', "0.5");
    newimagecircle1.setAttribute('stroke-miterlimit', "10");
    newimagecircle1.setAttribute('cx', "13.051");
    newimagecircle1.setAttribute('cy', "12.808");
    newimagecircle1.setAttribute('r', '9');
    newimage.appendChild(newimagecircle1);

    var newimagerecte = document.createElementNS(svgNS, "rect");
    newimagerecte.setAttribute("x", "12");
    newimagerecte.setAttribute('y', "5");
    newimagerecte.setAttribute('fill', "#333333");
    newimagerecte.setAttribute('width', "1");
    newimagerecte.setAttribute('height', "3");
    newimage.appendChild(newimagerecte);

    var newimagerecte1 = document.createElementNS(svgNS, "rect");
    newimagerecte1.setAttribute("x", "12");
    newimagerecte1.setAttribute('y', "18");
    newimagerecte1.setAttribute('fill', "#333333");
    newimagerecte1.setAttribute('width', "1");
    newimagerecte1.setAttribute('height', "3");
    newimage.appendChild(newimagerecte1);

    var newimagerecte2 = document.createElementNS(svgNS, "rect");
    newimagerecte2.setAttribute("x", "18");
    newimagerecte2.setAttribute('y', "12");
    newimagerecte2.setAttribute('fill', "#333333");
    newimagerecte2.setAttribute('width', "3");
    newimagerecte2.setAttribute('height', "1");
    newimage.appendChild(newimagerecte2);

    var newimagerecte3 = document.createElementNS(svgNS, "rect");
    newimagerecte3.setAttribute("x", "12");
    newimagerecte3.setAttribute('y', "5");
    newimagerecte3.setAttribute('fill', "#333333");
    newimagerecte3.setAttribute('width', "3");
    newimagerecte3.setAttribute('height', "1");
    newimage.appendChild(newimagerecte3);

    var newimagerecte4 = document.createElementNS(svgNS, "rect");
    newimagerecte4.setAttribute("x", "10.732");
    newimagerecte4.setAttribute('y', "9.768");
    newimagerecte4.setAttribute('transform', "matrix(0.7071 -0.7071 0.7071 0.7071 -5.3847 11.5354)");
    newimagerecte4.setAttribute('fill', "#333333");
    newimagerecte4.setAttribute('width', "1");
    newimagerecte4.setAttribute('height', "5");
    newimage.appendChild(newimagerecte4);

    var newimagerecte5 = document.createElementNS(svgNS, "rect");
    newimagerecte5.setAttribute("x", "11.414");
    newimagerecte5.setAttribute('y', "11.414");
    newimagerecte5.setAttribute('transform', "matrix(0.7071 -0.7071 0.7071 0.7071 -4.2027 13.6818)");
    newimagerecte5.setAttribute('fill', "#333333");
    newimagerecte5.setAttribute('width', "6");
    newimagerecte5.setAttribute('height', "1");
    newimage.appendChild(newimagerecte5);

}

function DrawMessageEventSvg(newimage) {
    var svgNS = "http://www.w3.org/2000/svg";

    var newimagecircle = document.createElementNS(svgNS, "circle");
    newimagecircle.setAttribute("fill", "none");
    newimagecircle.setAttribute('stroke', "#333333");
    newimagecircle.setAttribute('stroke-width', "0.5");
    newimagecircle.setAttribute('stroke-miterlimit', "10");
    newimagecircle.setAttribute('cx', "12.912");
    newimagecircle.setAttribute('cy', "12.646");
    newimagecircle.setAttribute('r', '11.5');
    newimage.appendChild(newimagecircle);

    var newimagerecte = document.createElementNS(svgNS, "rect");
    newimagerecte.setAttribute("x", "4.5");
    newimagerecte.setAttribute('y', "8.5");
    newimagerecte.setAttribute('fill', "none");
    newimagerecte.setAttribute('stroke', "#333333");
    newimagerecte.setAttribute('stroke-miterlimit', "10");
    newimagerecte.setAttribute('width', "16");
    newimagerecte.setAttribute('height', "10");
    newimage.appendChild(newimagerecte);

    var newimagepoly = document.createElementNS(svgNS, "polyline");
    newimagepoly.setAttribute('fill', "none");
    newimagepoly.setAttribute('stroke', "#333333");
    newimagepoly.setAttribute('stroke-miterlimit', "10");
    newimagepoly.setAttribute('points', "4.75,8.935 12.281,14 20.5,8.625");
    newimage.appendChild(newimagepoly);

    var newimageline = document.createElementNS(svgNS, "line");
    newimageline.setAttribute("x1", "205");
    newimageline.setAttribute('y1', "8.5");
    newimageline.setAttribute('fill', "#FFFFFF");
    newimageline.setAttribute('stroke', "#333333");
    newimageline.setAttribute('stroke-miterlimit', "10");
    newimageline.setAttribute('x2', "5");
    newimageline.setAttribute('y2', "8.5");
    newimage.appendChild(newimageline);
}

function getTimerEventModel() {
    var timerModel = { prefix: "", Name: "timerEventDefinition", Value: "", dictAttributes: { id: generateUUID() }, Elements: [], Children: [] };
    var timerDuration = { prefix: "", Name: "timeDuration", Value: "", dictAttributes: {}, Elements: [], Children: [] };
    var extElements = { prefix: "", Name: "extensionElements", Value: "", dictAttributes: {}, Elements: [], Children: [] };
    var durationModel = { prefix: "", Name: "Duration", Value: "", dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [], Children: [] };
    var isSyncModel = { prefix: "sbpmn", Name: "isSynchronous", Value: "False", dictAttributes: {}, Elements: [], Children: [] };
    extElements.Elements.push(durationModel);
    extElements.Elements.push(isSyncModel);
    timerDuration.Elements.push(extElements);
    timerModel.Elements.push(timerDuration);

    return timerModel;
}

function getMessageEventModel() {
    var messageModel = { prefix: "", Name: "messageEventDefinition", Value: "", dictAttributes: { id: generateUUID() }, Elements: [], Children: [] };
    return messageModel;
}

function getProcessRefId(event, scope) {
    var procRef = "";
    scope.PaticipantIndex = null;
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType == "participant") {
            if (scope.objData.lstShapes[i].Left <= event.offsetX && (scope.objData.lstShapes[i].Left + scope.objData.lstShapes[i].Width) >= event.offsetX && scope.objData.lstShapes[i].Top <= event.offsetY && (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) >= event.offsetY) {
                procRef = scope.objData.lstShapes[i].processRef;
                scope.PaticipantIndex = i;
                break;
            }
        }
    }
    //if (procRef == "") {
    //    scope.PaticipantIndex = null;
    //    if (scope.objData.DefaultProcessId == "") {
    //        scope.objData.DefaultProcessId = "PROCESS_" + generateUUID();
    //    }
    //    procRef = scope.objData.DefaultProcessId;
    //}
    scope.LaneIndex = null;
    // setting Lane index in which lane the element is present
    if (scope.PaticipantIndex != null) {
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (scope.objData.lstShapes[i].ShapeType == "lane") {
                if (scope.objData.lstShapes[i].Left <= event.offsetX && (scope.objData.lstShapes[i].Left + scope.objData.lstShapes[i].Width) >= event.offsetX && scope.objData.lstShapes[i].Top <= event.offsetY && (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) >= event.offsetY) {
                    scope.LaneIndex = i;
                    break;
                }
            }
        }
    }
    return procRef;
}

function AddFlowNodeModelInLaneShapeModel(shape, scope) {
    if (scope.LaneIndex != null) {
        var objflowNode = { dictAttributes: {}, Elements: [], Children: [], Name: "flowNodeRef", prefix: null, Value: shape.Id };
        scope.objData.lstShapes[scope.LaneIndex].ShapeModel.Elements.push(objflowNode);
    }
}

function CreateEventObject(event, scope, ElementName, isFormPanel, procRef) {

    if (procRef != "" && scope.LaneIndex != null) {
        var EventId = generateUUID();
        var shapeId = generateUUID();
        var objShapeDetailsOfEvent = {
            Height: 32, Width: 32, Left: event.offsetX, Top: event.offsetY, LabelHeight: 21, LabelWidth: 62, LabelLeft: event.offsetX - 10, LabelTop: event.offsetY + 38,
            ShapeName: "BPMNShape", ShapeType: "", processRef: procRef, lstWayPoints: [], Text: "", Id: EventId, ShapeId: shapeId
        };

        var EventModel = {
            dictAttributes: { id: EventId, name: "Start Event" }, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "extensionElements", prefix: null, Value: "" }],
            Children: [], Name: "startEvent", prefix: null, Value: ""
        };
        if (ElementName == "Start Event") {
            objShapeDetailsOfEvent.Text = "Start Event";
            objShapeDetailsOfEvent.ShapeType = "startEvent";
            EventModel.dictAttributes.name = "Start Event";
            EventModel.Name = "startEvent";
            EventModel.dictAttributes.isInterrupting = "true";
        }
        else if (ElementName == "End Event") {
            objShapeDetailsOfEvent.Text = "End Event";
            objShapeDetailsOfEvent.ShapeType = "endEvent";
            objShapeDetailsOfEvent.LabelLeft = event.offsetX - 8;

            EventModel.dictAttributes.name = "End Event";
            EventModel.Name = "endEvent";
        }
        else if (ElementName == "Intermediate Catch Event") {
            objShapeDetailsOfEvent.Text = "Intermediate Catch Event";
            objShapeDetailsOfEvent.ShapeType = "intermediateCatchEvent";
            objShapeDetailsOfEvent.LabelLeft = event.offsetX - 50;
            objShapeDetailsOfEvent.LabelWidth = 138;

            EventModel = {
                dictAttributes: { id: EventId, name: "Intermediate Catch Event" }, Elements: [{
                    dictAttributes: {}, Elements: [{
                        dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous",
                        prefix: null, Value: "False"
                    }], Children: [], Name: "extensionElements", prefix: null, Value: ""
                }], Children: [], Name: "intermediateCatchEvent", prefix: null, Value: ""
            };
        }
        objShapeDetailsOfEvent.ShapeModel = EventModel;
        // adding flownode model in lane shapemodel
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfEvent, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfEvent);
        drawCircle(scope.svgElement, objShapeDetailsOfEvent, scope);
        if (isFormPanel) {
            var isValid = isvalidEdgeForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfEvent);
            if (isValid) {
                if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "parallelGateway" || scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "inclusiveGateway") {
                    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                }
                DrawLineForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfEvent, scope);
            }
        }
    }
}

function isvalidEdgeForNewAddedElement(srcElement, tarElement) {
    var isValid = false;
    if (srcElement.ShapeType == "startEvent") {
        if (srcElement.processRef != tarElement.processRef) {
            if (tarElement.ShapeType == "intermediateCatchEvent") {
                alert("The startEvent must not have any outgoing message flows. Only Message Intermediate Event may have incoming or outgoing message flows.");
            } else if (tarElement.ShapeType == "exclusiveGateway") {
                var message = "The GateWay must not have any incoming or outgoing message flows.";
                if (srcElement.ShapeType == "startEvent") {
                    alert("The StartEvent must not have any outgoing message flows. " + message);
                } else {
                    alert(message);
                }
            } else if (tarElement.ShapeType == "task") {
                alert("The StartEvent must not have any outgoing message flows");
            } else if (tarElement.ShapeType == "textAnnotation") {
                alert("The StartEvent must not have any outgoing message flows");
            }
        } else {
            isValid = true;
        }
    }
    else if (srcElement.ShapeType == "endEvent") {
        if (srcElement.processRef == tarElement.processRef) {
            alert("The EndEvent must not have any outgoing sequence flows");
        } else {
            if (tarElement.ShapeType == "intermediateCatchEvent") {
                alert(" Only Message Intermediate Event may have incoming or outgoing message flows.");
            } else if (tarElement.ShapeType == "exclusiveGateway") {
                var message = "The GateWay must not have any incoming or outgoing message flows.";
                alert(message);
            } else {
                isValid = true;
            }
        }
    }
    else if (srcElement.ShapeType == "intermediateCatchEvent") {
        if (srcElement.processRef != tarElement.processRef) {
            if (tarElement.ShapeType == "intermediateCatchEvent" || tarElement.ShapeType == "task" || tarElement.ShapeType == "textAnnotation") {
                alert("Only Message Intermediate Event may have incoming or outgoing message flows.");
            } else if (tarElement.ShapeType == "exclusiveGateway") {
                var message = "The GateWay must not have any incoming or outgoing message flows.";
                if (srcElement.ShapeType == "intermediateCatchEvent") {
                    alert("Only Message Intermediate Event may have incoming or outgoing message flows. " + message);
                } else {
                    alert(message);
                }
            }
        } else {
            isValid = true;
        }
    }
    else if (srcElement.ShapeType == "exclusiveGateway" || srcElement.ShapeType == "inclusiveGateway" || srcElement.ShapeType == "parallelGateway" || srcElement.ShapeType == "eventBasedGateway") {
        if (srcElement.processRef != tarElement.processRef) {
            var message = "The GateWay must not have any incoming or outgoing message flows.";
            if (tarElement.ShapeType == "intermediateCatchEvent") {
                alert("Only Message Intermediate Event may have incoming or outgoing message flows. " + message);
            } else {
                alert(message);
            }
        } else {
            isValid = true;
        }
    }
    else if (srcElement.ShapeType == "task" || srcElement.ShapeType == "manualTask" || srcElement.ShapeType == "userTask" || srcElement.ShapeType == "businessRuleTask" ||
        srcElement.ShapeType == "serviceTask" || srcElement.ShapeType == "callActivity") {
        if (srcElement.processRef != tarElement.processRef) {
            if (tarElement.ShapeType == "intermediateCatchEvent") {
                alert("Only Message Intermediate Event may have incoming or outgoing message flows. ");
            } else if (tarElement.ShapeType == "exclusiveGateway") {
                alert("The GateWay must not have any incoming or outgoing message flows.");
            } else {
                isValid = true;
            }
        } else {
            isValid = true;
        }
    }
    else if (srcElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference") {
        if (tarElement.ShapeType != "task") {
            var DataStoreMessage = "The source or the target of the Data Association Connected to the DataStore must be an actitvity. ";
            if (tarElement.ShapeType == "intermediateCatchEvent") {
                var message = "Only Message Intermediate Event may have incoming or outgoing message flows.";
                alert(DataStoreMessage + message);
            } else if (tarElement.ShapeType == "exclusiveGateway") {
                var message = "The GateWay must not have any incoming or outgoing message flows.";
                alert(DataStoreMessage + message);
            }
            else {
                alert(DataStoreMessage);
            }
        } else {
            isValid = true;
        }
    }

    return isValid;
}

function getGateWayTypeModel(param, GateWayId) {
    var GatewayModel = "";
    if (param == "Exclusive Gateway") {
        GatewayModel = {
            dictAttributes: { id: GateWayId, name: "Exclusive Gateway", gatewayDirection: "Unspecified" }, Elements: [{
                dictAttributes: {}, Elements: [{
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "isSynchronous", prefix: null, Value: "False"
                }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "exclusiveGateway", prefix: null, Value: ""
        };
    } else if (param == "Inclusive Gateway") {
        GatewayModel = {
            dictAttributes: { id: GateWayId, name: "Inclusive Gateway", gatewayDirection: "Unspecified" }, Elements: [{
                dictAttributes: {}, Elements: [{
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False"
                }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "inclusiveGateway", prefix: null, Value: ""
        };
    } else if (param == "Parallel Gateway") {
        GatewayModel = {
            dictAttributes: { id: GateWayId, name: "Parallel Gateway", gatewayDirection: "Unspecified" }, Elements: [{
                dictAttributes: {}, Elements: [{
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False"
                }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "parallelGateway", prefix: null, Value: ""
        };
    } else if (param == "Event Gateway") {
        GatewayModel = {
            dictAttributes: { id: GateWayId, name: "Event Gateway", gatewayDirection: "Unspecified", eventGatewayType: "Exclusive", instantiate: "false" },
            Elements: [{ dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: "" }],
            Children: [], Name: "eventBasedGateway", prefix: null, Value: ""
        };
    }

    return GatewayModel;
}

function CreateGatewayObject(event, scope, ElementName, isFormPanel, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var GateWayId = generateUUID();
        var shapeId = generateUUID();

        var objShapeDetailsOfGateway = {
            Id: GateWayId, ShapeType: "", lstWayPoints: [], Left: event.offsetX, Width: 42, Height: 42, Top: event.offsetY, ShapeName: "BPMNShape", Text: "",
            LabelLeft: event.offsetX - 28, LabelWidth: 100, LabelHeight: 21, LabelTop: event.offsetY + 47, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };
        var GatewayModel = getGateWayTypeModel(ElementName, GateWayId);
        if (ElementName == "Exclusive Gateway") {
            objShapeDetailsOfGateway.ShapeType = "exclusiveGateway";
            objShapeDetailsOfGateway.Text = "Exclusive Gateway";
        }
        else if (ElementName == "Inclusive Gateway") {
            objShapeDetailsOfGateway.ShapeType = "inclusiveGateway";
            objShapeDetailsOfGateway.Text = "Inclusive Gateway";
        }
        else if (ElementName == "Parallel Gateway") {
            objShapeDetailsOfGateway.ShapeType = "parallelGateway";
            objShapeDetailsOfGateway.Text = "Parallel Gateway";
        }
        else if (ElementName == "Event Gateway") {
            objShapeDetailsOfGateway.ShapeType = "eventBasedGateway";
            objShapeDetailsOfGateway.Text = "Event Gateway";
        }


        objShapeDetailsOfGateway.ShapeModel = GatewayModel;
        // adding flownode model in lane shapemodel
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfGateway, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfGateway);
        drawDiamond(scope.svgElement, objShapeDetailsOfGateway, scope);
        if (isFormPanel) {
            var isValid = isvalidEdgeForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfGateway);
            if (isValid) {
                if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "parallelGateway" || scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "inclusiveGateway") {
                    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                }
                DrawLineForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfGateway, scope);
            }
        }
    }
}

function getTaskTypeModel(param, TaskID) {
    var TaskModel;
    if (param == "Task") {
        TaskModel = {
            dictAttributes: { id: TaskID, name: "Task", completionQuantity: "1", isForCompensation: "false", startQuantity: "1" }, Elements: [{
                dictAttributes: {},
                Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            },
            { dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [], Children: [], Name: "extraSettings", prefix: null, Value: "" }], Children: [], Name: "task", prefix: null, Value: ""
        };
    } else if (param == "Manual Task") {
        TaskModel = {
            dictAttributes: { id: TaskID, name: "Manual Task", completionQuantity: "1", isForCompensation: "false", startQuantity: "1" }, Elements: [{
                dictAttributes: {},
                Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            },
            { dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [], Children: [], Name: "extraSettings", prefix: null, Value: "" }], Children: [], Name: "manualTask",
            prefix: null, Value: ""
        };
    } else if (param == "User Task") {
        TaskModel = {
            dictAttributes: { id: TaskID, name: "User Task", completionQuantity: "1", implementation: "##unspecified", isForCompensation: "false", startQuantity: "1" },
            Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "actions", prefix: "sbpmn", Value: "" }, {
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "ReferenceId", prefix: "sbpmn", Value: ""
                }, { dictAttributes: {}, Elements: [], Children: [], Name: "userTaskExtension", prefix: "sbpmn", Value: "" },
                { dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", Value: "" }], Children: [], Name: "performers", prefix: "sbpmn", Value: "" },
                { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            },
            { dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True", sfwCanExecuteBpmTaskOnSoftErrors: "True" }, Elements: [], Children: [], Name: "extraSettings", prefix: null, Value: "" }],
            Children: [], Name: "userTask", prefix: null, Value: ""
        };
    } else if (param == "Business Rule Task") {
        TaskModel = {
            dictAttributes: { id: TaskID, name: "Business Rule Task", completionQuantity: "1", implementation: "##unspecified", isForCompensation: "false", startQuantity: "1" },
            Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "rule", prefix: "sbpmn", Value: "" }, {
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False"
                }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            },
            { dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [], Children: [], Name: "extraSettings", prefix: null, Value: "" }], Children: [], Name: "businessRuleTask", prefix: null, Value: ""
        };
    } else if (param == "Service Task") {
        TaskModel = {
            dictAttributes: { id: TaskID, name: "Service Task", completionQuantity: "1", implementation: "##WebService", isForCompensation: "false", startQuantity: "1" },
            Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }],
                Children: [], Name: "extensionElements", prefix: null, Value: ""
            }, {
                dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [],
                Children: [], Name: "extraSettings", prefix: null, Value: ""
            }], Children: [], Name: "serviceTask", prefix: null, Value: ""
        };
    }

    return TaskModel;
}

function CreateTaskObject(event, scope, ElementName, isFormPanel, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var TaskID = generateUUID();
        var shapeId = generateUUID();
        var objShapeDetailsOfTask = {
            Id: TaskID, ShapeType: "", lstWayPoints: [], Left: event.offsetX, Width: 85, Height: 55, Top: event.offsetY, ShapeName: "BPMNShape", Text: "",
            LabelLeft: event.offsetX + 1, LabelWidth: 90, LabelHeight: 37, LabelTop: event.offsetY + 25, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };
        var TaskModel = getTaskTypeModel(ElementName, TaskID);

        if (ElementName == "Task") {
            objShapeDetailsOfTask.ShapeType = "task";
            objShapeDetailsOfTask.Text = "Task";
        }
        else if (ElementName == "Manual Task") {
            objShapeDetailsOfTask.ShapeType = "manualTask";
            objShapeDetailsOfTask.Text = "Manual Task";
        }
        else if (ElementName == "User Task") {
            objShapeDetailsOfTask.ShapeType = "userTask";
            objShapeDetailsOfTask.Text = "User Task";
        }
        else if (ElementName == "Business Rule Task") {
            objShapeDetailsOfTask.ShapeType = "businessRuleTask";
            objShapeDetailsOfTask.Text = "Business Rule Task";
        }
        else if (ElementName == "Service Task") {
            objShapeDetailsOfTask.ShapeType = "serviceTask";
            objShapeDetailsOfTask.Text = "Service Task";
        }

        objShapeDetailsOfTask.ShapeModel = TaskModel;
        // adding flownode model in lane shapemodel
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfTask, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfTask);
        drawTask(scope.svgElement, objShapeDetailsOfTask, scope);
        if (isFormPanel) {
            var isValid = isvalidEdgeForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfTask);
            if (isValid) {
                if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "inclusiveGateway" || scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "parallelGateway") {
                    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                    var direction = getgateWaydirectionbasedOnEdges(scope.objData.lstShapes[scope.indexofResizerElement].Id, scope, 'Out');
                    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = direction;
                }

                //if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "parallelGateway" || scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "inclusiveGateway") {
                //    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                //}
                DrawLineForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfTask, scope);
            }
        }
    }
}

function CreateCallActivityObject(event, scope, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var activityId = generateUUID();
        var shapeId = generateUUID();
        var objShapeDetailsOfCallActivity = {
            Id: activityId, ShapeType: "callActivity", lstWayPoints: [], Left: event.offsetX, Width: 85, Height: 55, Top: event.offsetY, ShapeName: "BPMNShape", Text: "Call Activity",
            LabelLeft: event.offsetX + 1, LabelWidth: 90, LabelHeight: 37, LabelTop: event.offsetY + 25, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };

        var callActivityShapeModel = {
            dictAttributes: { id: activityId, name: "Call Activity", completionQuantity: "1", isForCompensation: "false", startQuantity: "1" },
            Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }],
                Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "callActivity", prefix: null, Value: ""
        };
        objShapeDetailsOfCallActivity.ShapeModel = callActivityShapeModel;
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfCallActivity, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfCallActivity);
        drawTask(scope.svgElement, objShapeDetailsOfCallActivity, scope);
    }
}

function CreateDataStoreObject(event, scope, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var DataStoreId = generateUUID();
        var shapeId = generateUUID();
        var DataStoreRefID = "DS_" + generateUUID();
        var objShapeDetailsOfDataStore = {
            Id: DataStoreId, ShapeType: "dataStoreReference", lstWayPoints: [], Left: event.offsetX, Width: 32, Height: 32, Top: event.offsetY, ShapeName: "BPMNShape", Text: "Data Store",
            LabelLeft: event.offsetX - 12, LabelWidth: 60, LabelHeight: 21, LabelTop: event.offsetY + 37, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };

        var DataStoreShapeModel = {
            dictAttributes: { id: DataStoreId, name: "Data Store", dataStoreRef: DataStoreRefID }, Elements: [{
                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False" }],
                Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "dataStoreReference", prefix: null, Value: ""
        };

        var DataStoreModel = { dictAttributes: { id: DataStoreRefID, name: "Data Store", isUnlimited: "false" }, Elements: [], Children: [], Name: "dataStore", prefix: null, Value: "" };

        objShapeDetailsOfDataStore.ShapeModel = DataStoreShapeModel;
        objShapeDetailsOfDataStore.DataStoreModel = DataStoreModel;
        // adding flownode model in lane shapemodel
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfDataStore, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfDataStore);
        drawdataStoreReference(scope.svgElement, objShapeDetailsOfDataStore, scope);
    }
}

function CreateDataObjReferenceObject(event, scope, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var DataRefId = generateUUID();
        var shapeId = generateUUID();
        var DataObjectRefID = "DO_" + generateUUID();
        var objShapeDetailsOfDataObject = {
            Id: DataRefId, ShapeType: "dataObjectReference", lstWayPoints: [], Left: event.offsetX, Width: 29, Height: 38, Top: event.offsetY, ShapeName: "BPMNShape", Text: "Data Object",
            LabelLeft: event.offsetX - 15, LabelWidth: 60, LabelHeight: 21, LabelTop: event.offsetY + 43, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };

        var DataObjectShapeModel = {
            dictAttributes: { id: DataRefId, name: "Data Object", dataObjectRef: DataObjectRefID }, Elements: [{
                dictAttributes: {}, Elements: [{
                    dictAttributes: {}, Elements: [],
                    Children: [], Name: "isSynchronous", prefix: "sbpmn", Value: "False"
                }], Children: [], Name: "extensionElements", prefix: null, Value: ""
            }], Children: [], Name: "dataObjectReference", prefix: null, Value: ""
        };

        var DataObjectModel = { dictAttributes: { id: DataObjectRefID, name: "Data Object", isCollection: "false" }, Elements: [], Children: [], Name: "dataObject", prefix: null, Value: "" };

        objShapeDetailsOfDataObject.ShapeModel = DataObjectShapeModel;
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfDataObject, scope);
        objShapeDetailsOfDataObject.DataObjectModel = DataObjectModel;
        scope.objData.lstShapes.push(objShapeDetailsOfDataObject);
        drawdataObjectReference(scope.svgElement, objShapeDetailsOfDataObject, scope);
    }
}

function CreateTextAnnotationObject(event, scope, isFromPanel, procRef) {
    if (procRef != "" && scope.LaneIndex != null) {
        var textAnnotationId = generateUUID();
        var shapeId = generateUUID();
        var objShapeDetailsOfTextAnnotation = {
            Id: textAnnotationId, ShapeType: "textAnnotation", lstWayPoints: [], Left: event.offsetX, Width: 85, Height: 55, Top: event.offsetY, ShapeName: "BPMNShape", Text: "",
            LabelLeft: event.offsetX, LabelWidth: 85, LabelHeight: 55, LabelTop: event.offsetY, processRef: procRef, lstWayPoints: [], ShapeId: shapeId
        };

        var TextAnnotationShapeModel = {
            dictAttributes: { id: textAnnotationId, textFormat: "text/plain" }, Elements: [{
                dictAttributes: {},
                Elements: [], Children: [], Name: "text", prefix: null, Value: ""
            }], Children: [], Name: "textAnnotation", prefix: null, Value: ""
        };

        objShapeDetailsOfTextAnnotation.ShapeModel = TextAnnotationShapeModel;
        AddFlowNodeModelInLaneShapeModel(objShapeDetailsOfTextAnnotation, scope);
        scope.objData.lstShapes.push(objShapeDetailsOfTextAnnotation);
        drawtextAnnotation(scope.svgElement, objShapeDetailsOfTextAnnotation, scope);
        if (isFromPanel) {
            var isValid = isvalidEdgeForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfTextAnnotation);
            if (isValid) {
                if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "parallelGateway" || scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "inclusiveGateway") {
                    scope.objData.lstShapes[scope.indexofResizerElement].ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                }
                DrawLineForNewAddedElement(scope.objData.lstShapes[scope.indexofResizerElement], objShapeDetailsOfTextAnnotation, scope);
            }
        }
    }
}

function CreateMessageReferenceObject(event, scope, ElementName) {
    var messageKind = "";
    if (ElementName == "Initiating Message") {
        messageKind = "initiating";
    } else {
        messageKind = "non_initiating";
    }
    var messageRef = generateUUID();
    var shapeId = generateUUID();
    var ShapeCoordinates = getCoordinatesForMessageEvent(scope);
    scope.objDragOverLineElement.ShapeModel.dictAttributes.messageRef = messageRef;
    var MessageShapeDetails = {
        Height: 20, Width: 20, Left: ShapeCoordinates.Left, Top: ShapeCoordinates.Top, LabelLeft: event.offsetX - 15, LabelWidth: 60, LabelHeight: 21, LabelTop: event.offsetY + 30,
        ShapeName: "BPMNShape", ShapeType: "message", processRef: "", lstWayPoints: [], Text: "", Id: messageRef, ShapeId: shapeId
    };
    scope.objDragOverLineElement.MessageVisibleKind = messageKind;
    scope.objDragOverLineElement.MessageReference = messageRef;
    var messageModel = { prefix: "", Name: "message", Value: "", dictAttributes: { id: scope.objDragOverLineElement.ShapeModel.dictAttributes.messageRef }, Elements: [], children: [] };
    MessageShapeDetails.ShapeModel = messageModel;
    scope.objData.lstShapes.push(MessageShapeDetails);
    drawInitiatingorNonInitiatingMessage(scope.svgElement, MessageShapeDetails, scope);
}


// Adding New Elements From panel


// Adding New Elements From panel

function drawNewTask(evt) {
    //var scope = getCurrentFileScope();
    if (evt && evt.buttons != 2) {
        var scope = angular.element(evt.currentTarget).scope();
        var x = parseFloat(scope.selectedElemForResize.getAttribute("x"));
        var y = parseFloat(scope.selectedElemForResize.getAttribute("y"));
        var width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
        var height = parseFloat(scope.selectedElemForResize.getAttribute("height"));

        var shapeypos = y + (height / 2) - 30;

        objShape = { Height: 55, Width: 85, Left: (x + width + 100), Top: shapeypos };
        var event = { offsetX: objShape.Left, offsetY: objShape.Top };
        var shapeProcRef = getProcessRefId(event, scope);
        CreateTaskObject(event, scope, "Task", true, shapeProcRef);
        removeSelection('NewElement', scope);
        removeRearrangingLinePoints(scope);
        removeResizerAroundElement(scope);
        scope.isDirty = true;
    }
}

function drawExclusiveGate(evt) {
    //var scope = getCurrentFileScope();
    if (evt && evt.buttons != 2) {
        var scope = angular.element(evt.currentTarget).scope();
        var x = parseFloat(scope.selectedElemForResize.getAttribute("x"));
        var y = parseFloat(scope.selectedElemForResize.getAttribute("y"));
        var width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
        var height = parseFloat(scope.selectedElemForResize.getAttribute("height"));

        var shapeypos = y + (height / 2) - 30;

        objShape = { Height: 42, Width: 42, Left: (x + width + 100), Top: shapeypos };
        var event = { offsetX: objShape.Left, offsetY: objShape.Top };
        var shapeProcRef = getProcessRefId(event, scope);
        CreateGatewayObject(event, scope, "Exclusive Gateway", true, shapeProcRef);
        removeSelection('NewElement', scope);
        removeRearrangingLinePoints(scope);
        removeResizerAroundElement(scope);
        scope.isDirty = true;
    }
}

function drawintermediateCatchEvent(evt) {
    //var scope = getCurrentFileScope();
    if (evt && evt.buttons != 2) {
        var scope = angular.element(evt.currentTarget).scope();
        var x = parseFloat(scope.selectedElemForResize.getAttribute("x"));
        var y = parseFloat(scope.selectedElemForResize.getAttribute("y"));
        var width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
        var height = parseFloat(scope.selectedElemForResize.getAttribute("height"));

        var shapeypos = y + (height / 2) - 30;
        objShape = { Height: 32, Width: 32, Left: (x + width + 100), Top: shapeypos };

        var event = { offsetX: objShape.Left, offsetY: objShape.Top };
        var shapeProcRef = getProcessRefId(event, scope);
        CreateEventObject(event, scope, "Intermediate Catch Event", true, shapeProcRef);
        removeSelection('NewElement', scope);
        removeRearrangingLinePoints(scope);
        removeResizerAroundElement(scope);
        scope.isDirty = true;
    }
}

function drawtextAnnotationFromPanel(evt) {
    //var scope = getCurrentFileScope();
    if (evt && evt.buttons != 2) {
        var scope = angular.element(evt.currentTarget).scope();
        var x = parseFloat(scope.selectedElemForResize.getAttribute("x"));
        var y = parseFloat(scope.selectedElemForResize.getAttribute("y"));
        var width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
        var height = parseFloat(scope.selectedElemForResize.getAttribute("height"));

        var shapeypos = y + (height / 2) - 30;
        objShape = { Height: 32, Width: 32, Left: (x + width + 100), Top: shapeypos };

        var event = { offsetX: objShape.Left, offsetY: objShape.Top };
        var shapeProcRef = getProcessRefId(event, scope);
        CreateTextAnnotationObject(event, scope, true, shapeProcRef);
        removeSelection('NewElement', scope);
        removeRearrangingLinePoints(scope);
        removeResizerAroundElement(scope);
        scope.isDirty = true;
    }
}

function DrawLineForNewAddedElement(sourceObj, objShape, scope) {
    var lstwayPoints = GetEdgeWayPointsForDottedLine(sourceObj, objShape);
    lstwayPoints = setShapeNameToWayPoints(lstwayPoints);
    var sourceID = sourceObj.Id;
    var shapeId = "EDGE_" + generateUUID();
    var isSequenceFlow = false;
    if (sourceID != "") {
        var procref = "";
        if (sourceObj.processRef == objShape.processRef && sourceObj.ShapeType != "dataStoreReference" && sourceObj.ShapeType != "dataObjectReference") {
            procref = sourceObj.processRef;
            isSequenceFlow = true;
        }
        else {
            if (scope.objData.CollaborationId == "") {
                scope.objData.CollaborationId = "COLLABORATION_" + generateUUID();
            }
            procref = scope.objData.CollaborationId;
        }
        var Edgeid = generateUUID();
        var Edgeobj = {
            Id: Edgeid, lstWayPoints: lstwayPoints, ShapeName: "BPMNEdge", ShapeType: "sequenceFlow", SourceElement: sourceID,
            TargetElement: objShape.Id, LabelLeft: 0, LabelTop: 0, LabelHeight: 0, LabelWidth: 0, Text: "", processRef: procref, ShapeId: shapeId
        };
        var EdgeShapeModel = "";

        if (objShape.ShapeType == "textAnnotation") {
            Edgeobj.ShapeType = "association";
            var length = lstwayPoints.length;
            if (length > 2) {
                var numberofElementsToRemove = length - 2;
                lstwayPoints.splice(1, numberofElementsToRemove);
                Edgeobj.lstWayPoints = lstwayPoints;
            }
            EdgeShapeModel = { dictAttributes: { id: Edgeid, sourceRef: sourceID, targetRef: objShape.Id, associationDirection: "None" }, Elements: [], Children: [], Name: "association", prefix: null, Value: "" };
        }
        else if (isSequenceFlow) {
            var LineSourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(Edgeobj.SourceElement, scope, "Element");
            if (LineSourceElement.ShapeType == "exclusiveGateway" || LineSourceElement.ShapeType == "inclusiveGateway") {
                EdgeShapeModel = {
                    dictAttributes: { id: Edgeid, sourceRef: sourceID, targetRef: objShape.Id },
                    Elements: [{
                        dictAttributes: {}, Elements: [{
                            dictAttributes: {}, Elements: [{ dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "leftside", prefix: "", IsValueInCDATAFormat: false, Value: "" },
                            { dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "rightside", prefix: "", IsValueInCDATAFormat: false, Value: "" }], Children: [], Name: "conditionExpression", prefix: "", IsValueInCDATAFormat: false, Value: ""
                        },
                        { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [], Name: "extensionElements", prefix: "", IsValueInCDATAFormat: false, Value: ""
                    }], Children: [], Name: "sequenceFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                };
            }
            else {
                EdgeShapeModel = { dictAttributes: { id: Edgeid, sourceRef: sourceID, targetRef: objShape.Id, associationDirection: "None" }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: "" };
            }
        }
        else if (sourceObj.ShapeType == "dataStoreReference" || sourceObj.ShapeType == "dataObjectReference") {
            var IOsepecificationID = generateUUID();
            var IOsepecificationModel = {
                dictAttributes: {}, Elements: [{
                    dictAttributes: { id: IOsepecificationID, isCollection: "false" }, Elements: [], Children: [], Name: "dataInput", prefix: "",
                    IsValueInCDATAFormat: false, Value: ""
                }, {
                    dictAttributes: {}, Elements: [{
                        dictAttributes: {}, Elements: [], Children: [], Name: "dataInputRefs", prefix: "", IsValueInCDATAFormat: false,
                        Value: IOsepecificationID
                    }], Children: [], Name: "inputSet", prefix: "", IsValueInCDATAFormat: false, Value: ""
                }], Children: [], Name: "ioSpecification", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            objShape.ShapeModel.Elements.push(IOsepecificationModel);

            Edgeobj = {
                Id: Edgeid, lstWayPoints: lstwayPoints, ShapeName: "BPMNEdge", ShapeType: "dataInputAssociation", SourceElement: sourceID,
                TargetElement: objShape.Id, LabelLeft: 0, LabelTop: 0, LabelHeight: 0, LabelWidth: 0, Text: "", processRef: procref, ShapeId: shapeId
            };

            var datainPutAssociationID = Edgeid;
            var datainPutAssociationModel = {
                dictAttributes: { id: datainPutAssociationID },
                Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "sourceRef", prefix: "", IsValueInCDATAFormat: false, Value: sourceObj.Id },
                { dictAttributes: {}, Elements: [], Children: [], Name: "targetRef", prefix: "", IsValueInCDATAFormat: false, Value: IOsepecificationID }],
                Children: [], Name: "dataInputAssociation", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            EdgeShapeModel = datainPutAssociationModel;
        }
        else {
            Edgeobj.ShapeType = "messageFlow";
            var EdgeShapeModel = {
                dictAttributes: { id: Edgeid, sourceRef: sourceID, targetRef: objShape.Id },
                Elements: [{
                    dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" },
                    { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [],
                    Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
        }
        Edgeobj.ShapeModel = EdgeShapeModel;
        AddLineArray(scope.svgElement, Edgeobj, scope);
        //generateLine(scope.svgElement, Edgeobj);
        scope.objData.lstShapes.push(Edgeobj);
    }
}

function setShapeNameToWayPoints(lstwayPoints) {
    for (var i = 0; i < lstwayPoints.length; i++) {
        lstwayPoints[i].ShapeName = "waypoint";
    }
    return lstwayPoints;
}


// Add Elements from Context Menu

function AddLaneToPool(event, scope) {
    if (scope) {
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (scope.selectedShape.Id == scope.objData.lstShapes[i].Id) {
                scope.PaticipantIndex = i;
                CreateLaneObject(event, scope, scope.selectedShape.processRef);
                break;
            }
        }
    }
}

function ChangeTaskFromContextMenu(scope, param) {
    if (scope.selectedShape) {
        var TaskModel = getTaskTypeModel(param, scope.selectedShape.Id);
        TaskModel.dictAttributes.name = scope.selectedShape.Text;
        if (param == "Task") {
            scope.selectedShape.ShapeType = "task";
        }
        else if (param == "Manual Task") {
            scope.selectedShape.ShapeType = "manualTask";
        }
        else if (param == "User Task") {
            scope.selectedShape.ShapeType = "userTask";
        }
        else if (param == "Business Rule Task") {
            scope.selectedShape.ShapeType = "businessRuleTask";
        }
        else if (param == "Service Task") {
            scope.selectedShape.ShapeType = "serviceTask";
        }
        var selectedsvgElement = scope.selectedElemForResize;
        if (selectedsvgElement) {
            selectedsvgElement.setAttribute("id", scope.selectedShape.ShapeType);
            selectedsvgElement.setAttribute("elementtype", scope.selectedShape.ShapeType);
            //Remove image
            for (var i = 0; i < selectedsvgElement.childNodes.length; i++) {
                if (selectedsvgElement.childNodes[i].nodeName == "image") {
                    selectedsvgElement.removeChild(selectedsvgElement.childNodes[i]);
                    break;
                }
            }
        }
        scope.selectedShape.ShapeModel = TaskModel;
        //Adding Image
        var image = getImageDependOnType(scope.selectedShape, selectedsvgElement, scope);
        image.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }
}

function ChangeGatewayFromContextMenu(scope, param) {
    if (scope.selectedShape) {
        var GatewayModel = getGateWayTypeModel(param, scope.selectedShape.Id);
        GatewayModel.dictAttributes.name = scope.selectedShape.Text;

        if (GatewayModel.Name == "inclusiveGateway" || GatewayModel.Name == "parallelGateway") {
            var direction = getgateWaydirectionbasedOnEdges(GatewayModel.dictAttributes.id, scope, "");
            GatewayModel.dictAttributes.gatewayDirection = direction;
        } else {
            GatewayModel.dictAttributes.gatewayDirection = scope.selectedShape.ShapeModel.dictAttributes.gatewayDirection;
        }

        if (param == "Exclusive Gateway") {
            scope.selectedShape.ShapeType = "exclusiveGateway";
        }
        else if (param == "Inclusive Gateway") {
            scope.selectedShape.ShapeType = "inclusiveGateway";
        }
        else if (param == "Parallel Gateway") {
            scope.selectedShape.ShapeType = "parallelGateway";
        }
        else if (param == "Event Gateway") {
            scope.selectedShape.ShapeType = "eventBasedGateway";
        }
        var selectedsvgElement = scope.selectedElemForResize;
        if (selectedsvgElement) {
            selectedsvgElement.setAttribute("id", scope.selectedShape.ShapeType);
            selectedsvgElement.setAttribute("elementtype", scope.selectedShape.ShapeType);
            //Remove image
            for (var i = 0; i < selectedsvgElement.childNodes.length; i++) {
                if (selectedsvgElement.childNodes[i].nodeName == "image") {
                    selectedsvgElement.removeChild(selectedsvgElement.childNodes[i]);
                    break;
                }
            }
        }
        scope.$evalAsync(function () {
            scope.selectedShape.ShapeModel = GatewayModel;
        });
        //Adding Image
        var image = getImageDependOnType(scope.selectedShape, selectedsvgElement, scope);
        image.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }
}

function ChangeEventTypeFromContextMenu(scope, param) {
    if (scope.selectedShape && scope.selectedElemForResize) {
        //remove event
        for (var i = 0; i < scope.selectedShape.ShapeModel.Elements.length; i++) {
            if (scope.selectedShape.ShapeModel.Elements[i].Name == "extensionElements" || scope.selectedShape.ShapeModel.Elements[i].Name == "messageEventDefinition" || scope.selectedShape.ShapeModel.Elements[i].Name == "timerEventDefinition") {
                scope.selectedShape.ShapeModel.Elements.splice(i, 1);
                break;
            }
        }
        if (param == 'Message Event') {
            var MessageEventModel = getMessageEventModel();
            scope.selectedShape.ShapeModel.Elements.push(MessageEventModel);
            ChangeCatchEventImageBasedonType(scope.selectedElemForResize, "Message");
        } else if (param == 'Timer Event') {
            var TimerEventModel = getTimerEventModel();
            scope.selectedShape.ShapeModel.Elements.push(TimerEventModel);
            ChangeCatchEventImageBasedonType(scope.selectedElemForResize, "Timer");
        }
    }
}

function OpenCalledElement(scope) {
    if (scope.selectedShape) {
        var strcalledElement = scope.selectedShape.ShapeModel.dictAttributes.calledElement;
        if (strcalledElement) {
            var items = strcalledElement.split(".");
            if (items && items.length > 0) {
                var fileName = items[0];
                scope.NavigateToCalledElementFromContextMenu(fileName);
            }
        }
    }
}

function FixEdgeFromContextMenu(scope) {
    if (scope && scope.selectedShape && scope.selectedLineEle) {
        var sourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.selectedShape.SourceElement, scope, "Element");
        var targetElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.selectedShape.TargetElement, scope, "Element");
        var lstwayPoints = GetEdgeWayPointsForDottedLine(sourceElement, targetElement);
        Newpoints = setShapeNameToWayPoints(lstwayPoints);
        if (sourceElement.ShapeType == "textAnnotation" || targetElement.ShapeType == "textAnnotation") {
            var length = Newpoints.length;
            if (Newpoints.length > 2) {
                var numberofElementsToRemove = length - 2;
                Newpoints.splice(1, numberofElementsToRemove);
            }
        }
        GenerateClonedLine(Newpoints, scope.selectedLineEle, scope);
        removeRearrangingLinePoints(scope);
        //update way points
        scope.selectedShape.lstWayPoints = Newpoints;
        if (scope.selectedShape.ShapeType == "messageFlow") {
            if (scope.selectedShape.ShapeModel.dictAttributes && scope.selectedShape.ShapeModel.dictAttributes.messageRef && scope.selectedShape.ShapeModel.dictAttributes.messageRef.trim().length > 0) {
                var messageElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.selectedShape.ShapeModel.dictAttributes.messageRef, scope, "Element");
                removeElementFromSvg(messageElement, scope);
                var ShapeCoordinates = getCoordinatesForMessageEvent(scope);
                messageElement.Left = ShapeCoordinates.Left;
                messageElement.Top = ShapeCoordinates.Top;
                drawInitiatingorNonInitiatingMessage(scope.svgElement, messageElement, scope);
            }
        }
    }
}

function OnChangeMessageKindFromContextMenu(scope, messageKind) {
    if (scope && scope.selectedShape) {
        scope.$apply(function () {
            if (messageKind == "initiating" || messageKind == "non_initiating") {
                if (scope.selectedShape.ShapeModel.dictAttributes && scope.selectedShape.ShapeModel.dictAttributes.messageRef && scope.selectedShape.ShapeModel.dictAttributes.messageRef.trim().length > 0) {
                    scope.selectedShape.MessageVisibleKind = messageKind;
                    var messageElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.selectedShape.ShapeModel.dictAttributes.messageRef, scope, "Element");
                    ChangeImageOfTheShape(messageKind, scope, messageElement);
                }
                else {
                    AddMessageKind(messageKind, scope);
                }
            } else {
                RemoveMessageKind(scope);
            }
        });
    }
}

function AddMessageKind(messageKind, scope) {
    var messageRef = generateUUID();
    var shapeId = generateUUID();
    var ShapeCoordinates = getShapeCoordinatesBasedOnResizerLine(scope);
    scope.selectedShape.ShapeModel.dictAttributes.messageRef = messageRef;
    var MessageShapeDetails = {
        Height: 20, Width: 20, Left: ShapeCoordinates.Left, Top: ShapeCoordinates.Top, LabelHeight: 0, LabelWidth: 0, LabelLeft: 0, LabelTop: 0,
        ShapeName: "BPMNShape", ShapeType: "message", processRef: "", lstWayPoints: [], Text: "", Id: messageRef, ShapeId: shapeId
    };
    scope.selectedShape.MessageVisibleKind = messageKind;
    scope.selectedShape.MessageReference = messageRef;
    var messageModel = { prefix: "", Name: "message", Value: "", dictAttributes: { id: scope.selectedShape.ShapeModel.dictAttributes.messageRef }, Elements: [], children: [] };
    MessageShapeDetails.ShapeModel = messageModel;
    scope.objData.lstShapes.push(MessageShapeDetails);
    drawInitiatingorNonInitiatingMessage(scope.svgElement, MessageShapeDetails, scope);
}

function RemoveMessageKind(scope) {
    if (scope.selectedShape.ShapeModel.dictAttributes.messageRef && scope.selectedShape.ShapeModel.dictAttributes.messageRef.trim().length > 0) {
        var objShape = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.selectedShape.ShapeModel.dictAttributes.messageRef, scope, "Delete");
        removeElementFromSvg(objShape, scope);
        scope.selectedShape.MessageVisibleKind = "";
        scope.selectedShape.MessageReference = "";
        scope.selectedShape.ShapeModel.dictAttributes.messageRef = "";
    }
}

//functions for ToolBar dropdown
function toggleOpenBPMEventsList(event) {
    onBodyClick();
    if ($("div[class='displayblock main-parent']").find("#BPMEvents").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMEvents").slideUp();
    }
    else {
        $("div[class='displayblock main-parent']").find("#BPMEvents").slideDown();
    }
    event.stopPropagation();
}

function closeBPMEventList() {
    if ($("div[class='displayblock main-parent']").find("#BPMEvents").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMEvents").slideUp();
    }
}

function toggleOpenBPMGatewayList(event) {
    onBodyClick();
    if ($("div[class='displayblock main-parent']").find("#BPMGateway").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMGateway").slideUp();
    }
    else {
        $("div[class='displayblock main-parent']").find("#BPMGateway").slideDown();
    }
    event.stopPropagation();
}

function closeBPMGatewayList() {
    if ($("div[class='displayblock main-parent']").find("#BPMGateway").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMGateway").slideUp();
    }
}

function toggleOpenBPMTasksList(event) {
    onBodyClick();
    if ($("div[class='displayblock main-parent']").find("#BPMTasks").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMTasks").slideUp();
    }
    else {
        $("div[class='displayblock main-parent']").find("#BPMTasks").slideDown();
    }
    event.stopPropagation();
}

function closeBPMTasksList() {
    if ($("div[class='displayblock main-parent']").find("#BPMTasks").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMTasks").slideUp();
    }
}

function toggleOpenBPMDataList(event) {
    onBodyClick();
    if ($("div[class='displayblock main-parent']").find("#BPMData").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMData").slideUp();
    }
    else {
        $("div[class='displayblock main-parent']").find("#BPMData").slideDown();
    }
    event.stopPropagation();
}

function closeBPMDataList() {
    if ($("div[class='displayblock main-parent']").find("#BPMData").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMData").slideUp();
    }
}

function toggleOpenBPMCallActivity(event) {
    onBodyClick();
    if ($("div[class='displayblock main-parent']").find("#BPMCallActivity").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMCallActivity").slideUp();
    }
    else {
        $("div[class='displayblock main-parent']").find("#BPMCallActivity").slideDown();
    }
    event.stopPropagation();
}

function closeBPMCallActivityList() {
    if ($("div[class='displayblock main-parent']").find("#BPMCallActivity").is(':visible')) {
        $("div[class='displayblock main-parent']").find("#BPMCallActivity").slideUp();
    }
}

function CheckandAddExtrasettingsModel(objShape) {
    var extrasettingsModel = objShape.ShapeModel.Elements.filter(function (x) { return x.Name == "extraSettings"; });
    if (extrasettingsModel && extrasettingsModel.length == 0) {
        var extrasettingsModel = {
            dictAttributes: { sfwExcludeHolidays: "True", sfwExcludeWeekends: "True" }, Elements: [],
            Children: [], Name: "extraSettings", prefix: null, Value: ""
        };
        objShape.ShapeModel.Elements.push(extrasettingsModel);
    }
}