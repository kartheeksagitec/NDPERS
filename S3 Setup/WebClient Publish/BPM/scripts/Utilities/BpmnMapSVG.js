function removeExistingTextNodeAndAddNewNode(scope) {
    var mainElement = "";
    if (scope.selectedShape && scope.selectedElemForResize) {
        for (var i = 0; i < scope.selectedElemForResize.childNodes.length; i++) {
            if (scope.selectedElemForResize.childNodes[i].nodeName == "text") {
                scope.selectedElemForResize.removeChild(scope.selectedElemForResize.childNodes[i]);
                i--;
            } else if (scope.selectedElemForResize.childNodes[i].id == "MainElement") {
                mainElement = scope.selectedElemForResize.childNodes[i];
            }
        }
        var param = "";
        if (scope.selectedElemForResize.id == "textAnnotation") {
            param = "textAnnotation";
        }


        var text = document.createElementNS(scope.svgNS, "text");
        text.setAttributeNS(null, "id", "text");
        if (scope.selectedShape.ShapeType == "participant" || scope.selectedShape.ShapeType == "lane") {
            param = "Participant";
            text.setAttribute('x', scope.selectedShape.Left);
            text.setAttribute('y', scope.selectedShape.Top);
        } else {
            text.setAttribute('x', scope.selectedShape.LabelLeft);
            text.setAttribute('y', scope.selectedShape.LabelTop);
        }
        text.setAttribute('width', scope.selectedShape.LabelWidth);
        text.setAttribute('height', scope.selectedShape.LabelHeight);
        text.setAttribute("class", "textbpmn");
        text.setAttribute('fill', 'black');

        if (scope.selectedShape.ShapeType && scope.selectedShape.ShapeType != "task" && scope.selectedShape.ShapeType != "userTask" && scope.selectedShape.ShapeType != "serviceTask" && scope.selectedShape.ShapeType != "businessRuleTask" && scope.selectedShape.ShapeType != "manualTask" && scope.selectedShape.ShapeType != "callActivity"
            && scope.selectedShape.ShapeType != "participant" && scope.selectedShape.ShapeType != "lane" && scope.selectedShape.ShapeType != "textAnnotation") {
            text.setAttribute("elementid", scope.selectedShape.Id);
            text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
        }
        //text.setAttribute("font-size", 11);
        text.textContent = scope.selectedShape.Text;
        //text.setAttributeNS(null, "stroke", "black");
        scope.selectedElemForResize.appendChild(text);
        var textnode = wraptorect(text, mainElement, 0, 0.5, param, scope);
        if (scope.selectedShape.ShapeType == "participant" || scope.selectedShape.ShapeType == "lane") {
            var rotate = -90 + '  ' + scope.selectedShape.Left + ',' + scope.selectedShape.Top + ' ';
            var translate = 10 + ' ' + scope.selectedShape.Height / 2;
            textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
            textnode.setAttribute('text-anchor', 'middle');
        }
    }
}

function RemoveExistingTextAndAddNewNodeForFlowLines(scope) {
    if (scope.selectedLineEle && scope.selectedShape) {
        if (scope.selectedLineEle.parentNode.id == "line") {
            for (var i = 0; i < scope.selectedLineEle.parentNode.childNodes.length; i++) {
                if (scope.selectedLineEle.parentNode.childNodes[i].nodeName == "text") {
                    scope.selectedLineEle.parentNode.removeChild(scope.selectedLineEle.parentNode.childNodes[i]);
                    i--;
                }
            }
            if (scope.selectedShape.LabelLeft <= 0 || scope.selectedShape.LabelTop <= 0 || scope.selectedShape.LabelHeight <= 0 || scope.selectedShape.LabelWidth <= 0) {
                var coordinates = getCoordinatesForMessageEvent(scope);
                scope.selectedShape.LabelLeft = coordinates.Left;
                scope.selectedShape.LabelTop = coordinates.Top;
                //var text = document.createElementNS(scope.svgNS, "text");
                //text.textContent = scope.selectedShape.Text;
                //var width = getlabelWidth(text, null, 0, 0.5, scope);
                //scope.selectedShape.LabelWidth = width;
                //scope.selectedShape.LabelHeight = 20;
            }

            var text = document.createElementNS(scope.svgNS, "text");
            text.setAttributeNS(null, "id", "text");
            text.setAttribute('x', scope.selectedShape.LabelLeft);
            text.setAttribute('y', scope.selectedShape.LabelTop);
            text.setAttribute('width', scope.selectedShape.LabelWidth);
            text.setAttribute('height', scope.selectedShape.LabelHeight);
            text.setAttribute("class", "textbpmn");
            text.setAttribute("elementid", scope.selectedShape.Id);
            text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
            //text.setAttribute('fill', 'black');
            text.setAttribute("font-size", 11);
            text.textContent = scope.selectedShape.Text;
            //text.setAttributeNS(null, "stroke", "black");
            scope.selectedLineEle.parentNode.appendChild(text);
            var textnode = wraptorect(text, null, 0, 0.5, undefined, scope);
        }
    }
}

function removeElementFromSvg(objShape, scope) {
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].id != undefined && objShape != "" && scope.svgElement.childNodes[i].id == objShape.ShapeType) {
            var Left = parseFloat(scope.svgElement.childNodes[i].getAttribute("x"));
            var Top = parseFloat(scope.svgElement.childNodes[i].getAttribute("y"));
            var Height = parseFloat(scope.svgElement.childNodes[i].getAttribute("height"));
            var width = parseFloat(scope.svgElement.childNodes[i].getAttribute("width"));
            var isFound = false;
            if (objShape.Left == Left && objShape.Top == Top && objShape.Height == Height && objShape.Width == width) {
                scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                break;
            }
        }
    }
}

function getShapeCoordinatesBasedOnResizerLine(scope) {
    var Point = { Left: 0, Top: 0 };
    angular.forEach(scope.svgElement.childNodes, function (element) {
        if (element.nodeName != "#text" && element.nodeName != "#comment" && element.nodeName != "defs") {
            if (element.getAttribute("id") == "linedraggablegroup") {
                if (element.childNodes.length > 2) {
                    var midElementIndex = element.childNodes.length / 2;
                    midElementIndex = midElementIndex.toFixed(0) - 1;
                    Point.Left = element.childNodes[midElementIndex].childNodes[0].getAttribute("x") - 7.5;
                    Point.Top = element.childNodes[midElementIndex].childNodes[0].getAttribute("y") - 7.5;
                }
                else {
                    var waypoints = scope.objData.lstShapes[scope.indexOfSelectedLine].lstWayPoints;
                    if (waypoints.length > 2) {
                        if ((waypoints[1].Left - waypoints[2].Left) > -0.1 && (waypoints[1].Left - waypoints[2].Left) < 0.1) {
                            if (waypoints[1].Top > waypoints[2].Top) {
                                var ypoint = waypoints[2].Top + (waypoints[1].Top - waypoints[2].Top) / 2;
                                Point.Top = ypoint - 10;
                            }
                            else {
                                var ypoint = waypoints[1].Top + (waypoints[2].Top - waypoints[1].Top) / 2;
                                Point.Top = ypoint - 10;
                            }
                            Point.Left = waypoints[1].Left - 10;

                        } else {
                            if (waypoints[1].Left > waypoints[2].Left) {
                                var xpoint = waypoints[2].Left + (waypoints[1].Left - waypoints[2].Left) / 2;
                                Point.Left = xpoint - 10;
                            }
                            else {
                                var xpoint = waypoints[1].Left + (waypoints[2].Left - waypoints[1].Left) / 2;
                                Point.Left = xpoint - 10;
                            }
                            Point.Top = waypoints[1].Top - 10;
                        }
                    } else if (waypoints.length == 2) {
                        if ((waypoints[0].Left - waypoints[1].Left) > -0.1 && (waypoints[0].Left - waypoints[1].Left) < 0.1) {
                            if (waypoints[0].Top > waypoints[1].Top) {
                                var ypoint = waypoints[1].Top + (waypoints[0].Top - waypoints[1].Top) / 2;
                                Point.Top = ypoint - 10;
                            }
                            else {
                                var ypoint = waypoints[0].Top + (waypoints[1].Top - waypoints[0].Top) / 2;
                                Point.Top = ypoint - 10;
                            }
                            Point.Left = waypoints[0].Left - 10;

                        } else {
                            if (waypoints[0].Left > waypoints[1].Left) {
                                var xpoint = waypoints[1].Left + (waypoints[0].Left - waypoints[1].Left) / 2;
                                Point.Left = xpoint - 10;
                            }
                            else {
                                var xpoint = waypoints[0].Left + (waypoints[1].Left - waypoints[0].Left) / 2;
                                Point.Left = xpoint - 10;
                            }
                            Point.Top = waypoints[0].Top - 10;
                        }
                    }
                }
            }
        }
    });
    return Point;
}

function ChangeImageOfTheShape(messagekind, scope, objShape) {
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].id == "message") {
            var Left = parseFloat(scope.svgElement.childNodes[i].getAttribute("x"));
            var Top = parseFloat(scope.svgElement.childNodes[i].getAttribute("y"));
            var Height = parseFloat(scope.svgElement.childNodes[i].getAttribute("height"));
            var width = parseFloat(scope.svgElement.childNodes[i].getAttribute("width"));
            var isFound = false;
            if (objShape.Left == Left && objShape.Top == Top && objShape.Height == Height && objShape.Width == width) {
                isFound = true;
                for (var j = 0; j < scope.svgElement.childNodes[i].childNodes.length; j++) {
                    if (scope.svgElement.childNodes[i].childNodes[j].nodeName == "image") {
                        if (messagekind == "initiating") {
                            scope.svgElement.childNodes[i].childNodes[j].setAttribute("href", "images/BPM/initiating-message-icon.svg");
                            break;
                        } else if (messagekind == "non_initiating") {
                            scope.svgElement.childNodes[i].childNodes[j].setAttribute("href", "images/BPM/non-initiating-message-icon.svg");
                            break;
                        }
                    }
                }
            }

            if (isFound) {
                break;
            }
        }
    }
}

function drawInitiatingorNonInitiatingMessage(svgElement, objShape, scope) {
    var mesageType = getMessageType(objShape, scope);
    if (mesageType != "") {
        var g = document.createElementNS(scope.svgNS, "g");
        g.setAttributeNS(null, 'x', objShape.Left);
        g.setAttributeNS(null, 'y', objShape.Top);
        g.setAttribute("elementtype", objShape.ShapeType);
        g.setAttributeNS(null, 'height', objShape.Height);
        g.setAttributeNS(null, 'width', objShape.Width);
        g.setAttribute("class", 'draggable');
        g.setAttribute("id", objShape.ShapeType);
        g.setAttribute("onmousedown", 'selectElement(evt,this)');
        //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
        generateTitletoElement(objShape, g, scope);
        var image = document.createElementNS(scope.svgNS, "image");
        image.setAttribute("id", "img");
        image.setAttribute('x', objShape.Left);
        image.setAttribute('y', objShape.Top);
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        if (mesageType == "initiating") {
            image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/initiating-message-icon.svg');
        } else {
            image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/non-initiating-message-icon.svg');
        }
        g.appendChild(image);

        var text = document.createElementNS(scope.svgNS, "text");
        text.setAttributeNS(null, "id", "text");
        text.setAttribute('x', objShape.LabelLeft);
        text.setAttribute('y', objShape.LabelTop);
        text.setAttribute('height', objShape.LabelHeight);
        text.setAttribute('width', objShape.LabelWidth);
        text.setAttribute('fill', 'black');
        text.setAttribute("class", "textbpmn");
        text.setAttribute("elementid", objShape.Id);
        text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
        //text.setAttribute("font-size", 11);
        text.textContent = objShape.Text;
        //text.setAttributeNS(null, "stroke", "black");
        g.appendChild(text);
        var textnode = wraptorect(text, null, 0, 0.5, undefined, scope);
        svgElement.appendChild(g);
        $(g).bind("contextmenu", function (event) {
            showContextMenu(event);
        });
    }
}

function getMessageType(objShape, scope) {
    var messageVisiblekind = "";
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType == "messageFlow") {
            if (objShape.Id == scope.objData.lstShapes[i].MessageReference) {
                messageVisiblekind = scope.objData.lstShapes[i].MessageVisibleKind;
                break;
            }
        }
    }
    return messageVisiblekind;
}

function AddLineArray(svgElement, objShape, scope) {
    if (objShape.lstWayPoints) {
        var isExecuted = false;
        var gtextandlines = document.createElementNS(scope.svgNS, "g");
        gtextandlines.setAttribute("id", "line");
        gtextandlines.setAttribute("elementtype", "linegroup");
        var text = document.createElementNS(scope.svgNS, "text");
        text.setAttributeNS(null, "id", "text");
        text.setAttribute('x', objShape.LabelLeft);
        text.setAttribute('y', objShape.LabelTop);
        text.setAttribute('height', objShape.LabelHeight);
        text.setAttribute('width', objShape.LabelWidth);
        text.setAttribute('fill', 'black');
        text.setAttribute("font-size", 11);
        text.setAttribute("elementid", objShape.Id);
        text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
        text.setAttribute("class", "textbpmn");
        text.textContent = objShape.Text;
        //text.setAttribute("transform", "rotate" + "(" + 30 + ")");
        //text.setAttributeNS(null, "stroke", "black");
        gtextandlines.appendChild(text);
        var textnode = wraptorect(text, null, 0, 0.5, undefined, scope);
        var g = document.createElementNS(scope.svgNS, "g");
        g.setAttribute("onmousedown", 'selectedlineElement(evt,this)');
        if (scope.activeTab != "Execution") {
            g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
        }
        //g.setAttribute("onmouseover", 'setToolTipToLine(evt)');
        var EdgeSourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(objShape.SourceElement, scope, "Element");
        if (EdgeSourceElement.ShapeType == "exclusiveGateway" || EdgeSourceElement.ShapeType == "inclusiveGateway") {
            g.setAttribute("elementtype", "GatewayExpression");
            gtextandlines.setAttribute("elementtype", "GatewayExpression");
        }
        else {
            g.setAttribute("elementtype", objShape.ShapeType);
            gtextandlines.setAttribute("elementtype", objShape.ShapeType);
        }
        //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
        g.setAttribute("id", "line");
        if (scope.activeTab != "EXECUTION") {
            isExecuted = true;
        } else {
            if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
                isExecuted = true;
            }
        }
        for (var i = 0; i < objShape.lstWayPoints.length; i++) {
            if (i < objShape.lstWayPoints.length - 1) {
                var gline = document.createElementNS(scope.svgNS, "g");
                gline.setAttribute("id", objShape.Id);
                gline.setAttribute("elementtype", "linegroup");
                var line = document.createElementNS(scope.svgNS, "line");
                line.setAttribute('id', objShape.Id);
                line.setAttribute("x1", objShape.lstWayPoints[i].Left);
                line.setAttribute("y1", objShape.lstWayPoints[i].Top);
                line.setAttribute("x2", objShape.lstWayPoints[i + 1].Left);
                line.setAttribute("y2", objShape.lstWayPoints[i + 1].Top);
                if (objShape.ShapeType == "dataOutputAssociation" || objShape.ShapeType == "association" || objShape.ShapeType == "messageFlow" || objShape.ShapeType == "dataInputAssociation") {
                    line.setAttribute("stroke-dasharray", "6,4");
                    if (objShape.ShapeType == "messageFlow") {
                        g.setAttribute("ondragover", 'SetDragOverLineElement(evt)');
                        g.setAttribute("ondragleave", 'SetNullOnDragleaveLineElement(evt)');
                    }
                }
                if (isExecuted) {
                    line.setAttributeNS(null, "stroke", "#3a3a3a");
                } else {
                    line.setAttributeNS(null, "stroke", "#d3d3d3");
                }
                if (i == objShape.lstWayPoints.length - 2 || objShape.lstWayPoints.length == 1) {
                    if (objShape.ShapeType != "association") {
                        var arrowMarker = "url(#arrow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
                        if (scope.activeTab == "EXECUTION") {
                            arrowMarker = "url(#arrow_" + scope.Guid + ")";
                        }
                        line.setAttribute("marker-end", arrowMarker);
                    }
                }
                var lineshadow = document.createElementNS(scope.svgNS, "line");
                lineshadow.setAttribute('id', objShape.Id);
                lineshadow.setAttribute("x1", objShape.lstWayPoints[i].Left);
                lineshadow.setAttribute("y1", objShape.lstWayPoints[i].Top);
                lineshadow.setAttribute("x2", objShape.lstWayPoints[i + 1].Left);
                lineshadow.setAttribute("y2", objShape.lstWayPoints[i + 1].Top);
                lineshadow.setAttribute("stroke-width", 15);
                //lineshadow.setAttribute("stroke-opacity", 0.1);
                if (objShape.ShapeType == "dataOutputAssociation" || objShape.ShapeType == "association" || objShape.ShapeType == "messageFlow" || objShape.ShapeType == "dataInputAssociation") {
                    line.setAttribute("stroke-dasharray", "6,4");
                    if (objShape.ShapeType == "messageFlow") {
                        g.setAttribute("ondragover", 'SetDragOverLineElement(evt)');
                        g.setAttribute("ondragleave", 'SetNullOnDragleaveLineElement(evt)');
                    }
                }
                lineshadow.setAttributeNS(null, "stroke", "transparent");
                gline.appendChild(line);
                gline.appendChild(lineshadow);
                g.appendChild(gline);
            }
        }
        gtextandlines.appendChild(g);
        svgElement.appendChild(gtextandlines);
        $(g).bind("contextmenu", function (event) {
            showContextMenu(event);
        });
    }
}

function drawDiamond(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("class", 'draggable');
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    if (scope.activeTab != "Execution") {
        g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
    }
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var poly = document.createElementNS(scope.svgNS, "polygon");
    var point1 = {
        X: objShape.Left,
        Y: objShape.Top + (objShape.Height / 2)
    };
    var point2 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top
    };
    var point3 = {
        X: objShape.Left + objShape.Width,
        Y: objShape.Top + objShape.Height / 2
    };
    var point4 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top + objShape.Height
    };
    points = point1.X + "," + point1.Y + "  " + point2.X + "," + point2.Y + "  " + point3.X + "," + point3.Y + "  " + point4.X + "," + point4.Y;
    poly.setAttributeNS(null, 'x', objShape.Left);
    poly.setAttributeNS(null, 'y', objShape.Top);
    poly.setAttributeNS(null, 'height', objShape.Height);
    poly.setAttributeNS(null, 'width', objShape.Width);
    poly.setAttribute("id", "MainElement");
    poly.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    poly.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    var shadowcolor = "url(#rectshadow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
    if (scope.activeTab == "EXECUTION") {
        shadowcolor = "url(#rectshadow_" + scope.Guid + ")";
    }
    poly.setAttributeNS(null, "filter", shadowcolor);
    poly.setAttribute("points", points);
    //poly.setAttribute("stroke", "black");
    var fillcolor = "url(#exclusive_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
    if (scope.activeTab == "EXECUTION") {
        g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
        if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
            fillcolor = "url(#exclusive_" + scope.Guid + ")";
            if (g.iterationCount > 1) {
                drawCirclewithCount(g, scope);
            }
        } else {
            fillcolor = "#d3d3d3";
        }
    }
    poly.setAttribute('fill', fillcolor);
    g.appendChild(poly);

    if (objShape.ShapeType != "exclusiveGateway") {
        var image = getImageDependOnType(objShape, g, scope);
        image.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.LabelLeft);
    text.setAttribute('y', objShape.LabelTop);
    text.setAttribute('height', objShape.LabelHeight);
    text.setAttribute('width', objShape.LabelWidth);
    text.setAttribute('fill', 'black');
    text.setAttribute("class", "textbpmn");
    text.setAttribute("elementid", objShape.Id);
    text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
    //text.setAttribute("font-size", 11);
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);

    var textnode = wraptorect(text, poly, 0, 0.5, undefined, scope);
    svgElement.appendChild(g);

    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });

}

function drawCircle(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("class", 'draggable');
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    if (scope.activeTab != "Execution") {
        g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
    }
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);

    var myCircle = document.createElementNS(scope.svgNS, "circle"); //to create a circle. for rectangle use "rectangle"
    myCircle.setAttributeNS(null, "id", "MainElement");
    myCircle.setAttributeNS(null, "cx", objShape.Left + 15);
    myCircle.setAttributeNS(null, "cy", objShape.Top + 15);
    myCircle.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    myCircle.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    myCircle.setAttributeNS(null, "r", objShape.Width / 2);
    var fillcolor = "";
    if (objShape.ShapeType == "startEvent") {
        fillcolor = "url(#start_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
            if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
                fillcolor = "url(#start_" + scope.Guid + ")";
                if (g.iterationCount > 1) {
                    drawCirclewithCount(g, scope);
                }
            } else {
                fillcolor = "#d3d3d3";
            }
        }
    }
    else if (objShape.ShapeType == "endEvent") {
        myCircle.setAttribute("stroke-width", 2);
        fillcolor = "url(#end_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
            if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
                fillcolor = "url(#end_" + scope.Guid + ")";
                if (g.iterationCount > 1) {
                    drawCirclewithCount(g, scope);
                }
            } else {
                fillcolor = "#d3d3d3";
            }
        }
    }
    else if (objShape.ShapeType == "intermediateCatchEvent") {
        fillcolor = "url(#intermediate_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
            if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
                fillcolor = "url(#intermediate_" + scope.Guid + ")";
                if (g.iterationCount > 1) {
                    drawCirclewithCount(g, scope);
                }
            } else {
                fillcolor = "#d3d3d3";
            }
        }
    }
    myCircle.setAttributeNS(null, "fill", fillcolor);
    myCircle.setAttributeNS(null, "stroke", "black");
    var shadowcolor = "url(#circleShadow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
    if (scope.activeTab == "EXECUTION") {
        shadowcolor = "url(#circleShadow_" + scope.Guid + ")";
    }
    myCircle.setAttributeNS(null, "filter", shadowcolor);
    g.appendChild(myCircle);

    if (objShape.ShapeType == "intermediateCatchEvent") {
        var image = getImageDependOnType(objShape, g, scope);
        image.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.LabelLeft);
    text.setAttribute('y', objShape.LabelTop);
    text.setAttribute('height', objShape.LabelHeight);
    text.setAttribute('width', objShape.LabelWidth);
    text.setAttribute('fill', 'black');
    text.setAttribute("class", "textbpmn");
    text.setAttribute("elementid", objShape.Id);
    text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
    //text.setAttribute("font-size", 11);
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);

    var textnode = wraptorect(text, myCircle, 0, 0.5, undefined, scope);
    svgElement.appendChild(g);

    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });

}

function drawCirclewithCount(gElement, scope) {
    var left = parseFloat(gElement.getAttribute("x"));
    var Top = parseFloat(gElement.getAttribute("y"));
    var width = parseFloat(gElement.getAttribute("width"));
    var XposOfCircle = left + width;
    var circle = document.createElementNS(scope.svgNS, "circle"); //to create a circle. for rectangle use "rectangle"
    circle.setAttributeNS(null, "id", "circle");
    circle.setAttributeNS(null, "cx", XposOfCircle + 3);
    circle.setAttributeNS(null, "cy", Top - 2);
    circle.setAttributeNS(null, "r", 16 / 2);
    var fillcolor = "#FFA500";
    circle.setAttributeNS(null, "fill", fillcolor);
    gElement.appendChild(circle);

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "textcount");
    text.setAttribute('x', XposOfCircle);
    text.setAttribute('y', Top);
    text.setAttribute('height', 7);
    text.setAttribute('width', 7);
    text.setAttribute('fill', 'white');
    text.setAttribute("font-size", 10);
    text.textContent = gElement.iterationCount;
    //text.setAttributeNS(null, "stroke", "white");
    gElement.appendChild(text);
}

function drawTask(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("class", 'draggable');
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    if (scope.activeTab != "Execution") {
        g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
    }
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var rectangle = document.createElementNS(scope.svgNS, "rect");
    rectangle.setAttributeNS(null, "id", "MainElement");
    rectangle.setAttributeNS(null, 'rx', 5);
    rectangle.setAttributeNS(null, 'ry', 5);
    rectangle.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    rectangle.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    rectangle.setAttributeNS(null, 'x', objShape.Left);
    rectangle.setAttributeNS(null, 'y', objShape.Top);
    rectangle.setAttributeNS(null, 'height', objShape.Height);
    rectangle.setAttributeNS(null, 'width', objShape.Width);
    //rectangle.setAttributeNS(null, "stroke", "black");
    g.appendChild(rectangle);
    var fillcolor = "";
    var shadowcolor = "";
    if (scope.activeTab != "EXECUTION") {
        fillcolor = "url(#task_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        shadowcolor = "url(#rectshadow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
    }
    else if (scope.activeTab == "EXECUTION") {
        g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
        if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
            shadowcolor = "url(#rectshadow_" + scope.Guid + ")";
            fillcolor = "url(#task_" + scope.Guid + ")";
            if (g.iterationCount > 1) {
                drawCirclewithCount(g, scope);
            }
        } else {
            fillcolor = "#d3d3d3";
            shadowcolor = "url(#rectshadow_" + scope.Guid + ")";
        }
    }
    rectangle.setAttributeNS(null, "filter", shadowcolor);
    rectangle.setAttributeNS(null, "fill", fillcolor);

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute("class", "textbpmn");
    text.setAttribute('x', objShape.LabelLeft);
    text.setAttribute('y', objShape.LabelTop);
    text.setAttribute('width', objShape.LabelWidth);
    text.setAttribute('height', objShape.LabelHeight);
    text.setAttribute('fill', 'black');
    //text.setAttribute("font-size", 11);
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);
    var textnode = wraptorect(text, rectangle, 0, 0.5, undefined, scope);
    textnode.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    if (objShape.ShapeType != "task") {
        var image = getImageDependOnType(objShape, g, scope);
        image.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }
    svgElement.appendChild(g);

    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });

}

function drawSquareParam(svgElement, objShape, scope) {
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    if (scope.activeTab != "Execution") {
        g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
    }
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    if (objShape.ShapeType != "lane") {
        //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
        g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    }
    g.setAttribute("id", objShape.ShapeType);
    //g.setAttribute("class", 'draggable');
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var rectangle = document.createElementNS(scope.svgNS, "rect");
    rectangle.setAttributeNS(null, "id", "MainElement");
    if (objShape.ShapeType != "lane") {
        rectangle.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    }
    rectangle.setAttributeNS(null, 'x', objShape.Left);
    rectangle.setAttributeNS(null, 'y', objShape.Top);
    rectangle.setAttributeNS(null, 'height', objShape.Height);
    rectangle.setAttributeNS(null, 'width', objShape.Width);
    if (objShape.ShapeType != "lane") {
        rectangle.setAttributeNS(null, "fill", "#fefef2");
    }
    else {
        rectangle.setAttributeNS(null, "fill", "transparent");
        rectangle.setAttribute("onmouseover", 'removehoverCircle(evt,this)');
    }
    rectangle.setAttributeNS(null, "stroke", "black");
    if (objShape.ShapeType != "lane") {
        var shadowcolor = "url(#rectshadow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            shadowcolor = "url(#rectshadow_" + scope.Guid + ")";
        }
        rectangle.setAttributeNS(null, "filter", shadowcolor);
    }
    g.appendChild(rectangle);

    //drawing line
    if (objShape.ShapeType == "lane") {
        //var line = document.createElementNS(scope.svgNS, "line");
        //line.setAttribute('id', 'lineinsquareparam');
        //line.setAttribute('x1', objShape.Left + 40);
        //line.setAttribute('y1', objShape.Top);
        //line.setAttribute('x2', objShape.Left + 40);
        //line.setAttribute('y2', objShape.Top + objShape.Height);
        //line.setAttributeNS(null, "stroke", "black");
        //g.appendChild(line);
        var rectangle = document.createElementNS(scope.svgNS, "rect");
        rectangle.setAttributeNS(null, "id", "rectangleinsquareparam");
        rectangle.setAttributeNS(null, 'x', objShape.Left);
        rectangle.setAttributeNS(null, 'y', objShape.Top);
        rectangle.setAttributeNS(null, 'height', objShape.Height);
        rectangle.setAttributeNS(null, 'width', 40);
        rectangle.setAttributeNS(null, "fill", "#f7ede1");
        rectangle.setAttributeNS(null, "stroke", "black");
        g.appendChild(rectangle);
    } else {
        var rectangle = document.createElementNS(scope.svgNS, "rect");
        rectangle.setAttributeNS(null, "id", "rectangleinsquareparam");
        rectangle.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
        rectangle.setAttributeNS(null, 'x', objShape.Left);
        rectangle.setAttributeNS(null, 'y', objShape.Top);
        rectangle.setAttributeNS(null, 'height', objShape.Height);
        rectangle.setAttributeNS(null, 'width', 40);
        var fillcolor = "";
        if (scope.activeTab != "EXECUTION") {
            fillcolor = "url(#headerRect_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        }
        else if (scope.activeTab == "EXECUTION") {
            if (scope.CheckStepisExceutedOrNot(objShape.Id, g) && scope.BPMExceutionTabDetails.Mode != "Demo") {
                fillcolor = "url(#headerRect_" + scope.Guid + ")";
            } else {
                fillcolor = "#d3d3d3";
            }
        }
        rectangle.setAttributeNS(null, "fill", fillcolor);
        rectangle.setAttributeNS(null, "stroke", "black");
        g.appendChild(rectangle);
    }

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.Left);
    text.setAttribute('y', objShape.Top);
    text.setAttribute('fill', 'black');
    //text.setAttribute("font-size", 11);
    text.setAttribute("class", "textbpmn");
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);
    var textnode = wraptorect(text, rectangle, 0, 0.5, "Participant", scope);
    var rotate = -90 + '  ' + objShape.Left + ',' + objShape.Top + ' ';
    var translate = 10 + ' ' + objShape.Height / 2;
    textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
    textnode.setAttribute('text-anchor', 'middle');
    svgElement.appendChild(g);
    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });
}

function drawdataStoreReference(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("class", 'draggable');
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.LabelLeft);
    text.setAttribute('y', objShape.LabelTop);
    text.setAttribute("class", "textbpmn");
    text.setAttribute('width', objShape.LabelWidth);
    text.setAttribute('height', objShape.LabelHeight);
    text.setAttribute('fill', 'black');
    //text.setAttribute("font-size", 11);
    text.textContent = objShape.Text;
    text.setAttribute("elementid", objShape.Id);
    text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);
    var textnode = wraptorect(text, g, 0, 0.5, undefined, scope);

    var imageNode = getImageDependOnType(objShape, g, scope);
    imageNode.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    imageNode.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    svgElement.appendChild(g);

    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });

}

function drawdataObjectReference(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("class", 'draggable');
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.LabelLeft);
    text.setAttribute('y', objShape.LabelTop);
    text.setAttribute('width', objShape.LabelWidth);
    text.setAttribute('height', objShape.LabelHeight);
    text.setAttribute("class", "textbpmn");
    text.setAttribute('fill', 'black');
    text.setAttribute("elementid", objShape.Id);
    text.setAttribute("onmousedown", 'setResizeraroudtext(evt)');
    //text.setAttribute("font-size", 11);
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);
    var textnode = wraptorect(text, g, 0, 0.5, undefined, scope);

    var imageNode = getImageDependOnType(objShape, g, scope);
    imageNode.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    imageNode.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    svgElement.appendChild(g);

    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });

}

function drawtextAnnotation(svgElement, objShape, scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, 'x', objShape.Left);
    g.setAttributeNS(null, 'y', objShape.Top);
    g.setAttribute("elementtype", objShape.ShapeType);
    g.setAttributeNS(null, 'height', objShape.Height);
    g.setAttributeNS(null, 'width', objShape.Width);
    g.setAttribute("id", objShape.ShapeType);
    g.setAttribute("class", 'draggable');
    g.setAttribute("onmousedown", 'selectElement(evt,this)');
    if (scope.activeTab != "Execution") {
        g.setAttribute("ondblclick", 'selectPropertiesTab(evt)');
    }
    //g.setAttribute("ondblclick", 'showElementProperties(evt,this)');
    //g.setAttribute("onmouseover", 'hoverOnElement(evt)');
    g.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    generateTitletoElement(objShape, g, scope);
    var rectangle = document.createElementNS(scope.svgNS, "rect");
    rectangle.setAttributeNS(null, "id", "MainElement");
    rectangle.setAttributeNS(null, 'rx', 5);
    rectangle.setAttributeNS(null, 'ry', 5);
    rectangle.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    rectangle.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    rectangle.setAttributeNS(null, 'x', objShape.Left);
    rectangle.setAttributeNS(null, 'y', objShape.Top);
    rectangle.setAttributeNS(null, 'height', objShape.Height);
    rectangle.setAttributeNS(null, 'width', objShape.Width);
    rectangle.setAttributeNS(null, "fill", "transparent");
    rectangle.setAttributeNS(null, "stroke", "none");
    g.appendChild(rectangle);
    var gtagforLine = document.createElementNS(scope.svgNS, "g");
    gtagforLine.setAttribute("id", "textAnnotationLineGroup");
    var textAnnotationLinePoints = [];
    var point1 = { Left: objShape.Left + (objShape.Width / 2), Top: objShape.Top };
    textAnnotationLinePoints.push(point1);
    var point2 = { Left: objShape.Left, Top: objShape.Top };
    textAnnotationLinePoints.push(point2);
    var point3 = { Left: objShape.Left, Top: objShape.Top + objShape.Height };
    textAnnotationLinePoints.push(point3);
    var point4 = { Left: objShape.Left + (objShape.Width / 2), Top: objShape.Top + objShape.Height };
    textAnnotationLinePoints.push(point4);

    for (var i = 0; i < textAnnotationLinePoints.length; i++) {
        if (i < textAnnotationLinePoints.length - 1) {
            var textAnnotationLine = document.createElementNS(scope.svgNS, "line");
            textAnnotationLine.setAttribute('id', 'line');
            textAnnotationLine.setAttribute("stroke-width", 1);
            textAnnotationLine.setAttribute("x1", textAnnotationLinePoints[i].Left);
            textAnnotationLine.setAttribute("y1", textAnnotationLinePoints[i].Top);
            textAnnotationLine.setAttribute("x2", textAnnotationLinePoints[i + 1].Left);
            textAnnotationLine.setAttribute("y2", textAnnotationLinePoints[i + 1].Top);
            textAnnotationLine.setAttributeNS(null, "stroke", "black");
            gtagforLine.appendChild(textAnnotationLine);
        }
    }
    g.appendChild(gtagforLine);

    var text = document.createElementNS(scope.svgNS, "text");
    text.setAttributeNS(null, "id", "text");
    text.setAttribute('x', objShape.Left + 2);
    text.setAttribute('y', objShape.Top + 2);
    text.setAttribute('width', objShape.Width);
    text.setAttribute('height', objShape.Height);
    text.setAttribute('fill', 'black');
    //text.setAttribute("font-size", 11);
    text.setAttribute("class", "textbpmn");
    text.textContent = objShape.Text;
    //text.setAttributeNS(null, "stroke", "black");
    g.appendChild(text);
    var textnode = wraptorect(text, rectangle, 0, 0.5, "textAnnotation", scope);
    textnode.setAttribute("onmouseover", 'hoverOnElement(evt,this)');
    scope.svgElement.appendChild(g);
    $(g).bind("contextmenu", function (event) {
        showContextMenu(event);
    });
}

function getlabelWidth(textnode, boxObject, padding, linePadding, scope) {
    //var scope = getCurrentFileScope();
    var x_pos = 0;
    var y_pos = 0;
    var boxobjHeight;
    if (boxObject != null) {
        x_pos = parseFloat(boxObject.getAttribute('x')),
            y_pos = parseFloat(boxObject.getAttribute('y'));
    }

    fz = 12;
    var line_height = fz + linePadding;
    // We use this to calculate dy for each TSPAN.
    // Clone the original text node to store and display the final wrapping text.

    var wrapping = textnode.cloneNode(false);        // False means any TSPANs in the textnode will be discarded
    wrapping.setAttributeNS(null, 'x', x_pos + padding);
    wrapping.setAttributeNS(null, 'y', y_pos + padding);

    // Make a copy of this node and hide it to progressively draw, measure and calculate line breaks.

    var testing = wrapping.cloneNode(false);
    testing.setAttributeNS(null, 'visibility', 'hidden');  // Comment this out to debug

    var testingTSPAN = document.createElementNS(null, 'tspan');
    var testingTEXTNODE = document.createTextNode(textnode.textContent);
    testingTSPAN.appendChild(testingTEXTNODE);

    testing.appendChild(testingTSPAN);
    //var tester = document.getElementsByTagName('svg')[0].appendChild(testing);
    if (scope.activeTab == "EXECUTION") {
        $("div[id='" + scope.Guid + "']").find("#ExecutionSVG")[0].appendChild(testing);
        //$("#ScopeId_" + $scope.$parent.$id).find("#ExecutionBPMN")[0].appendChild(testing);
    }
    else {
        $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#mySVG")[0].appendChild(testing);
    }
    var charcs = textnode.textContent.split('');
    var linecounter = 0;
    testing.textContent = textnode.textContent + "";
    testwidth = testing.getBBox().width;

    return testwidth;
}

function getImageDependOnType(objShape, g, scope) {
    var image = document.createElementNS(scope.svgNS, "image");
    image.setAttribute("id", "img");
    image.setAttribute('x', objShape.Left);
    image.setAttribute('y', objShape.Top);

    if (objShape.ShapeType == "userTask") {
        image.setAttribute('height', '25');
        image.setAttribute('width', '25');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/user-task.svg');
    }
    else if (objShape.ShapeType == "serviceTask") {
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/service-task.svg');
    }
    else if (objShape.ShapeType == "businessRuleTask") {
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/business-rule-task.svg');
    }
    else if (objShape.ShapeType == "businessRuleTask") {
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/business-rule-task.svg');
    }
    else if (objShape.ShapeType == "manualTask") {
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/manual-task.svg');
    }
    else if (objShape.ShapeType == "callActivity") {
        image.setAttribute('x', objShape.Left + (objShape.Width / 2) - 10);
        image.setAttribute('y', objShape.Top + objShape.Height - 20);
        image.setAttribute('height', '20');
        image.setAttribute('width', '20');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/calling-process.svg');
    }
    else if (objShape.ShapeType == "intermediateCatchEvent") {
        var isEvent = false;
        image.setAttribute('x', objShape.Left + 2);
        image.setAttribute('y', objShape.Top + 2);
        image.setAttribute('height', '25');
        image.setAttribute('width', '25');
        for (var i = 0; i < objShape.ShapeModel.Elements.length; i++) {
            if (objShape.ShapeModel.Elements[i].Name == "messageEventDefinition") {
                isEvent = true;
                image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/messageEventDefinition.svg');
            } else if (objShape.ShapeModel.Elements[i].Name == "timerEventDefinition") {
                isEvent = true;
                image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/timerEventDefinition.svg');
            }
        }
        if (!isEvent) {
            image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/intermediateCatchEvent.svg');
        }
    }
    else if (objShape.ShapeType == "eventBasedGateway") {
        image.setAttribute('x', objShape.Left + 8.5);
        image.setAttribute('y', objShape.Top + 8);
        image.setAttribute('height', '25');
        image.setAttribute('width', '25');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/event-gateway.svg');
    }
    else if (objShape.ShapeType == "inclusiveGateway") {
        image.setAttribute('x', objShape.Left + 5.5);
        image.setAttribute('y', objShape.Top + 5.5);
        image.setAttribute('height', '30');
        image.setAttribute('width', '30');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/inclusive-gateway.svg');
    }
    else if (objShape.ShapeType == "parallelGateway") {
        image.setAttribute('x', objShape.Left + 5.5);
        image.setAttribute('y', objShape.Top + 5.5);
        image.setAttribute('height', '30');
        image.setAttribute('width', '30');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/parallel-gateway.svg');
    }
    else if (objShape.ShapeType == "dataStoreReference") {
        image.setAttribute('id', "MainElement");
        image.setAttribute('x', objShape.Left);
        image.setAttribute('y', objShape.Top);
        image.setAttribute('height', '32');
        image.setAttribute('width', '32');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/bpmn-data-source-icon.svg');
    }
    else if (objShape.ShapeType == "dataObjectReference") {
        image.setAttribute('id', "MainElement");
        image.setAttribute('x', objShape.Left);
        image.setAttribute('y', objShape.Top);
        image.setAttribute('height', '38');
        image.setAttribute('width', '29');
        image.setAttributeNS('http://www.w3.org/1999/xlink', 'href', 'images/BPM/bpmn-data-object-icon.svg');

    }
    g.appendChild(image);
    return image;
}

function wraptorect(textnode, boxObject, padding, linePadding, param, scope) {
    //var scope = getCurrentFileScope();
    var x_pos = 0;
    var y_pos = 0;
    var boxobjHeight;
    var boxwidth = 0;
    if (boxObject && boxObject != null && boxObject.nodeName != "circle" && boxObject.nodeName != "polygon" && boxObject.nodeName != "g" && boxObject.nodeName != "image") {
        x_pos = parseFloat(boxObject.getAttribute('x'));
        if (param != "textAnnotation" && param != "Participant") {
            y_pos = parseFloat(boxObject.getAttribute('y')) + 25;
        } else {
            y_pos = parseFloat(boxObject.getAttribute('y'));
        }

        if (param != "Participant") {
            boxwidth = parseFloat(boxObject.getAttribute('width'));
            boxobjHeight = parseFloat(boxObject.getAttribute('y')) + parseFloat(boxObject.getAttribute('height'));
        } else {
            boxwidth = parseFloat(boxObject.getAttribute('height'));
            boxobjHeight = parseFloat(boxObject.getAttribute('y')) + 35;
        }
    }
    else {
        x_pos = parseFloat(textnode.getAttribute('x')),
            y_pos = parseFloat(textnode.getAttribute('y'));

        boxwidth = parseFloat(textnode.getAttribute('width'));
        boxobjHeight = parseFloat(textnode.getAttribute('y')) + parseFloat(textnode.getAttribute('height'));
    }

    fz = 14;
    var line_height = fz + linePadding;
    // We use this to calculate dy for each TSPAN.
    // Clone the original text node to store and display the final wrapping text.

    var wrapping = textnode.cloneNode(false);        // False means any TSPANs in the textnode will be discarded
    wrapping.setAttributeNS(null, 'x', x_pos + padding);
    wrapping.setAttributeNS(null, 'y', y_pos + padding);

    // Make a copy of this node and hide it to progressively draw, measure and calculate line breaks.

    var testing = wrapping.cloneNode(false);
    testing.setAttributeNS(null, 'visibility', 'hidden');  // Comment this out to debug

    var testingTSPAN = document.createElementNS(null, 'tspan');
    var testingTEXTNODE = document.createTextNode(textnode.textContent);
    testingTSPAN.appendChild(testingTEXTNODE);

    testing.appendChild(testingTSPAN);
    //var tester = document.getElementsByTagName('svg')[0].appendChild(testing);
    var tester = "";
    if (scope.activeTab != "EXECUTION") {
        tester = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#mySVG")[0].appendChild(testing);
    }
    else if (scope.activeTab == "EXECUTION") {
        tester = $("div[id='" + scope.Guid + "']").find("#ExecutionSVG")[0].appendChild(testing);
        //tester = $("#ScopeId_" + $scope.$parent.$id).find("#ExecutionBPMN")[0].appendChild(testing);
    }
    var charcs = textnode.textContent.split('');
    var line = line2 = "";
    var linecounter = 0;
    var testwidth;
    var TotalHeightofText = 0;
    for (var n = 0; n < charcs.length; n++) {

        line2 = line + charcs[n];
        testing.textContent = line2;
        testwidth = testing.getBBox().width;

        if ((testwidth + 2 * padding) > boxwidth) {

            testingTSPAN = document.createElementNS(scope.svgNS, 'tspan');
            testingTSPAN.setAttributeNS(null, 'x', x_pos + padding);
            testingTSPAN.setAttributeNS(null, 'dy', line_height);
            TotalHeightofText += line_height;
            if (boxObject != null) {
                if ((boxobjHeight > y_pos + TotalHeightofText + line_height) || (boxObject.nodeName == "circle" || boxObject.nodeName == "polygon" || boxObject.nodeName == "image" || boxObject.nodeName == "g")) {
                    var newlinearray = line.split(" ");
                    line = "";
                    for (var i = 0; i < newlinearray.length; i++) {
                        if (i < newlinearray.length - 1) {
                            line += newlinearray[i] + " ";
                        }
                        else if (newlinearray.length == 1) {
                            line += newlinearray[i] + " ";
                        }
                    }
                    testingTEXTNODE = document.createTextNode(line);
                    testingTSPAN.appendChild(testingTEXTNODE);
                    wrapping.appendChild(testingTSPAN);
                    if (newlinearray.length > 1) {
                        line = newlinearray[newlinearray.length - 1] + charcs[n];
                    }
                    else {
                        line = charcs[n];
                    }
                    linecounter++;
                }
            }
            else {
                var newlinearray = line.split(" ");
                line = "";
                for (var i = 0; i < newlinearray.length; i++) {
                    if (i < newlinearray.length - 1) {
                        line += newlinearray[i] + " ";
                    }
                    else if (newlinearray.length == 1) {
                        line += newlinearray[i] + " ";
                    }
                }

                testingTEXTNODE = document.createTextNode(line);
                testingTSPAN.appendChild(testingTEXTNODE);
                wrapping.appendChild(testingTSPAN);

                if (newlinearray.length > 1) {
                    line = newlinearray[newlinearray.length - 1] + charcs[n];
                }
                else {
                    line = charcs[n];
                }
                linecounter++;
            }
        }
        else {
            if (boxObject != null) {
                if ((boxobjHeight > y_pos + TotalHeightofText + line_height) || (boxObject.nodeName == "g" || boxObject.nodeName == "image")) {
                    line = line2;
                }
            } else {
                if ((boxobjHeight > y_pos + TotalHeightofText + line_height)) {
                    line = line2;
                }
            }
        }
    }

    var testingTSPAN = document.createElementNS(scope.svgNS, 'tspan');
    testingTSPAN.setAttributeNS(null, 'x', x_pos + padding);
    testingTSPAN.setAttributeNS(null, 'dy', line_height);

    var testingTEXTNODE = document.createTextNode(line);
    testingTSPAN.appendChild(testingTEXTNODE);

    wrapping.appendChild(testingTSPAN);

    testing.parentNode.removeChild(testing);
    textnode.parentNode.replaceChild(wrapping, textnode);

    return wrapping;
}

function generateTitletoElement(objShape, g, scope) {
    var title = document.createElementNS(scope.svgNS, "title");
    title.textContent = objShape.ShapeType;
    g.appendChild(title);
}

function setToolTipToLine(evt) {
    var currentElement = evt.currentTarget;
    var bool = checkTitleIsPresentOrDeleteTitle(currentElement, "bool");
    if (!bool) {
        //var scope = getCurrentFileScope();
        var scope = angular.element(currentElement).scope();
        var titlecontext = currentElement.getAttribute("elementtype");
        var title = document.createElementNS(scope.svgNS, "title");
        title.textContent = titlecontext;
        currentElement.appendChild(title);
    }
}

function checkTitleIsPresentOrDeleteTitle(currentElement, param) {
    var isFound = false;
    for (var i = 0; i < currentElement.childNodes.length; i++) {
        if (currentElement.childNodes[i].nodeName == "title") {
            isFound = true;
            if (param == "Delete") {
                currentElement.removeChild(currentElement.childNodes[i]);
            }
            break;
        }
    }
    return isFound;
}

function onElementDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
}

function onCriteriaFieldDrag(e) {
    var scp = angular.element($(e.target).closest(".portlet-body")).scope();
    scp.dragDropData = scp.selectedCriteriaField;
}

function onCriteriaFieldDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
}

function onCriteriaFieldDrop(e) {
    var scp = angular.element($(e.target).closest(".portlet-body")).scope();
    if (scp.dragDropData) {
        scp.$apply(function () {
            if (!scp.parameters.some(function (x) { return x.name == scp.dragDropData.name; }))
                scp.parameters.push(scp.dragDropData);
            scp.dragDropData = undefined;
        });
    }
}

function GetNewConditonName(strItemKey, objConditions, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    while (CheckForDuplicateIDForConditions(strItemName, objConditions)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    }

    return strItemName;


}

function CheckForDuplicateIDForConditions(strId, objConditions) {
    var blnReturn = false;
    angular.forEach(objConditions.Elements, function (item) {
        if (item.dictAttributes.sfwName && strId && item.dictAttributes.sfwName.toLowerCase() == strId.toLowerCase()) {
            blnReturn = true;
        }
    });
    return blnReturn;
}

function CheckForDuplicateIDForConditionsWhileEdit(strId, objConditions, index) {
    var blnReturn = false;
    var count = 0;
    for (var i = 0; i < objConditions.Elements.length; i++) {
        if (objConditions.Elements[i].dictAttributes.sfwName && strId && objConditions.Elements[i].dictAttributes.sfwName.toLowerCase() == strId.toLowerCase() && index != i) {
            count++;
        }
    }

    if (count > 0) {
        blnReturn = true;
    }
    return blnReturn;
}

function SetDragOverLineElement(evnt) {
    //scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    checkTitleIsPresentOrDeleteTitle(evnt.currentTarget, "Delete");
    scope.DragOverElement = evnt.currentTarget;
    var lineId = scope.DragOverElement.childNodes[0].getAttribute("id");
    var objLineElement = getObjShapeoftheLine("Line", scope, lineId);
    if (objLineElement.MessageReference == null || objLineElement.MessageReference == "" || objLineElement.MessageReference == undefined) {
        if (scope.DraggableElement == "Initiating Message" || scope.DraggableElement == "Non Initiating Message") {
            if (scope.DragOverElement != undefined && scope.DragOverElement != null) {
                scope.DragOverElement.childNodes[scope.DragOverElement.childNodes.length - 1].childNodes[0].removeAttributeNS(null, "marker-end");
                angular.forEach(scope.DragOverElement.childNodes, function (item) {
                    if (item.getAttribute("elementtype") == 'linegroup') {
                        item.childNodes[0].setAttribute("stroke-width", 5);
                        item.childNodes[0].setAttribute("stroke", "#723837");
                    }
                });
            }
        }
    } else {
        scope.DragOverElement = null;
    }
}

function resetNodeProperties(lineElement, scope) {
    if (lineElement != undefined && lineElement != null) {

        angular.forEach(lineElement.childNodes, function (item) {
            if (item.getAttribute("elementtype") == 'linegroup') {
                item.childNodes[0].setAttribute("stroke-width", 1);
                item.childNodes[0].setAttribute("stroke", "black");
            }
        });
        var arrowMarker = "url(#arrow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            arrowMarker = "url(#arrow_" + scope.Guid + ")";
        }
        lineElement.childNodes[lineElement.childNodes.length - 1].childNodes[0].setAttribute("marker-end", arrowMarker);
    }
}

function SetNullOnDragleaveLineElement(evnt) {
    //scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    checkTitleIsPresentOrDeleteTitle(evnt.currentTarget, "Delete");
    scope.DragOverElement = null;
    var DragLeaveElement = evnt.currentTarget;
    if (DragLeaveElement != undefined && DragLeaveElement != null) {

        angular.forEach(DragLeaveElement.childNodes, function (item) {
            if (item.getAttribute("elementtype") == 'linegroup') {
                item.childNodes[0].setAttribute("stroke-width", 1);
                item.childNodes[0].setAttribute("stroke", "black");
            }
        });
        var arrowMarker = "url(#arrow_" + scope.$root.currentopenfile.file.FileName.trim() + ")";
        if (scope.activeTab == "EXECUTION") {
            fillcolor = "url(#arrow_" + scope.Guid + ")";
        }
        DragLeaveElement.childNodes[DragLeaveElement.childNodes.length - 1].childNodes[0].setAttribute("marker-end", arrowMarker);
    }
}

function getCoordinatesForMessageEvent(scope) {
    var waypoints = "";
    if (scope.objDragOverLineElement != null) {
        waypoints = scope.objDragOverLineElement.lstWayPoints;
    }
    else {
        waypoints = scope.selectedShape.lstWayPoints;
    }
    var Point = { Left: 0, Top: 0 };
    if (waypoints.length > 2) {
        if ((waypoints[1].Left - waypoints[2].Left) > -0.1 && (waypoints[1].Left - waypoints[2].Left) < 0.1) {
            if (waypoints[1].Top > waypoints[2].Top) {
                var ypoint = waypoints[2].Top + (waypoints[1].Top - waypoints[2].Top) / 2;
                Point.Top = ypoint - 10;
            }
            else {
                var ypoint = waypoints[1].Top + (waypoints[2].Top - waypoints[1].Top) / 2;
                Point.Top = ypoint - 10;
            }
            Point.Left = waypoints[1].Left - 10;

        } else {
            if (waypoints[1].Left > waypoints[2].Left) {
                var xpoint = waypoints[2].Left + (waypoints[1].Left - waypoints[2].Left) / 2;
                Point.Left = xpoint - 10;
            }
            else {
                var xpoint = waypoints[1].Left + (waypoints[2].Left - waypoints[1].Left) / 2;
                Point.Left = xpoint - 10;
            }
            Point.Top = waypoints[1].Top - 10;
        }
    } else if (waypoints.length == 2) {
        if ((waypoints[0].Left - waypoints[1].Left) > -0.1 && (waypoints[0].Left - waypoints[1].Left) < 0.1) {
            if (waypoints[0].Top > waypoints[1].Top) {
                var ypoint = waypoints[1].Top + (waypoints[0].Top - waypoints[1].Top) / 2;
                Point.Top = ypoint - 10;
            }
            else {
                var ypoint = waypoints[0].Top + (waypoints[1].Top - waypoints[0].Top) / 2;
                Point.Top = ypoint - 10;
            }
            Point.Left = waypoints[0].Left - 10;

        } else {
            if (waypoints[0].Left > waypoints[1].Left) {
                var xpoint = waypoints[1].Left + (waypoints[0].Left - waypoints[1].Left) / 2;
                Point.Left = xpoint - 10;
            }
            else {
                var xpoint = waypoints[0].Left + (waypoints[1].Left - waypoints[0].Left) / 2;
                Point.Left = xpoint - 10;
            }
            Point.Top = waypoints[0].Top - 10;
        }
    }

    return Point;
}

//function EditSVGheightandwidth() {
//    var scope = getCurrentFileScope();
//    scope.EditSVGheightandwidth();
//}