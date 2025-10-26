// drag selected Element functions

function removeSelection(param, scope) {
    var isFound = false;
    //var scope = getCurrentFileScope();
    var Elements = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#mySVG  #SelectionElement");
    for (var i = 0; i < Elements.length; i++) {
        if (scope.selectedElement != Elements[i].parentNode || param == "line" || param == 'NewElement') {
            Elements[i].parentNode.removeChild(Elements[i]);
            if (param == 'NewElement') {
                scope.selectedElemForResize = null;
            }
        }
        else {
            isFound = true;
        }
    }
    return isFound;
}

function removeSelectionForLane(scope) {
    //var scope = getCurrentFileScope();
    var Elements = $("div[id='" + scope.$root.currentopenfile.file.FileName + "']").find("#mySVG  #lane");
    for (var i = 0; i < Elements.length; i++) {
        for (var j = 0; j < Elements[i].childNodes.length; j++) {
            if (Elements[i].childNodes[j].nodeName == "rect" || Elements[i].childNodes[j].nodeName == "line") {
                Elements[i].childNodes[j].setAttribute("stroke", "black");
                Elements[i].childNodes[j].setAttribute("stroke-width", "1");
            }
        }
    }
}

function selectElement(evt, ele) {
    var scope = angular.element(ele).scope();
    if (scope) {
        if (scope.selectedElementID) {
            function resetSelectedShape() {
                scope.selectedElementID = undefined;
            }
            scope.$apply(resetSelectedShape);
        }
        if (scope.selectedLineEle && scope.activeTab == "MAP") {
            setColorToSelectedLine(scope.selectedLineEle, "remove");
        }
        scope.$apply(function () {
            scope.selectedElement = evt.currentTarget;
            scope.isElementMoved = false;
            scope.selectedLineEle = null;
            scope.selectedElemForResize = scope.selectedElement;
            var isSame = false;
            var isSelectionRemoved = false;
            if (scope.activeTab != "EXECUTION") {
                if (!evt.ctrlKey) {
                    isSame = removeSelection(undefined, scope);
                    removeSelectionForLane(scope);
                    scope.lstMultipleSelectedShapes = [];
                } else {
                    //For Multiple Selection
                    if (scope.selectedShape && scope.selectedShape.ShapeType != 'lane' && scope.selectedShape.ShapeType != 'participant' && scope.selectedShape.ShapeName != "BPMNEdge" && scope.selectedShape.ShapeType != "message") {
                        scope.lstMultipleSelectedShapes.push(scope.selectedShape);
                        scope.selectedShape = undefined;
                        scope.OnSelectLeftBPMTab('Toolbox');
                    }
                }
                for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                    if (scope.objData.lstShapes[i].Left == parseFloat(scope.selectedElemForResize.getAttribute("x")) && scope.objData.lstShapes[i].Top == parseFloat(scope.selectedElemForResize.getAttribute("y"))) {
                        scope.indexofResizerElement = i;
                        if (!evt.ctrlKey) {
                            scope.selectedShape = scope.objData.lstShapes[i];
                        } else {
                            // Add or Remove into Multiple Selection List
                            if (scope.objData.lstShapes[i] && scope.objData.lstShapes[i].ShapeType != 'lane' && scope.objData.lstShapes[i].ShapeType != 'participant' && scope.objData.lstShapes[i].ShapeType != 'message') {
                                if (!checkShapeAlreadyPresentinListorNot(scope.lstMultipleSelectedShapes, scope.objData.lstShapes[i])) {
                                    scope.lstMultipleSelectedShapes.push(scope.objData.lstShapes[i]);
                                } else {
                                    //if it is already there we are removing that selection
                                    var Elements = $(scope.selectedElement).find("#SelectionElement");
                                    for (var j = 0; j < Elements.length; j++) {
                                        Elements[j].parentNode.removeChild(Elements[j]);
                                        isSelectionRemoved = true;
                                    }
                                }
                                scope.CheckAndSetBoolAllSelectedShapesInSameLane(scope.lstMultipleSelectedShapes);
                            }
                        }
                    }
                }
            } else {
                for (var i = 0; i < scope.lstExecutionTabShapes.length; i++) {
                    if (scope.lstExecutionTabShapes[i].Left == parseFloat(scope.selectedElemForResize.getAttribute("x")) && scope.lstExecutionTabShapes[i].Top == parseFloat(scope.selectedElemForResize.getAttribute("y"))) {
                        scope.indexofResizerElement = i;
                        scope.selectedShape = scope.lstExecutionTabShapes[i];
                    }
                }
            }

            if (scope.activeTab != "EXECUTION") {
                scope.selectedElementTop = scope.selectedElement.getAttribute("y");
                scope.selectedElementLeft = scope.selectedElement.getAttribute("x");
                scope.selectedElemstartLeft = scope.selectedElementLeft;
                scope.selectedElemstartTop = scope.selectedElementTop;
                scope.selectedElemPrevBounds = {
                    Left: parseFloat(scope.selectedElementLeft), Top: parseFloat(scope.selectedElementTop), Width: parseFloat(scope.selectedElement.getAttribute("width")), Height: parseFloat(scope.selectedElement.getAttribute("height"))
                };

                //setLaneElement if the Element is Present in Particular Lane
                if (scope.selectedElement.getAttribute("id") != "lane" && scope.selectedElement.getAttribute("id") != "participant") {
                    var isLaneFound = false;
                    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
                        if (scope.svgElement.childNodes[i].nodeName != "#text" && scope.svgElement.childNodes[i].nodeName != "#comment" && scope.svgElement.childNodes[i].nodeName != "defs") {
                            if (scope.svgElement.childNodes[i].getAttribute("id") == "lane") {
                                var xpos = parseFloat(scope.svgElement.childNodes[i].getAttribute("x"));
                                var ypos = parseFloat(scope.svgElement.childNodes[i].getAttribute("y"));
                                var height = parseFloat(scope.svgElement.childNodes[i].getAttribute("height"));
                                var width = parseFloat(scope.svgElement.childNodes[i].getAttribute("width"));
                                if (xpos < scope.objData.lstShapes[scope.indexofResizerElement].Left && scope.objData.lstShapes[scope.indexofResizerElement].Left < (xpos + width) &&
                                    scope.objData.lstShapes[scope.indexofResizerElement].Top > ypos && scope.objData.lstShapes[scope.indexofResizerElement].Top < (ypos + height)) {
                                    scope.PrevlaneElement = scope.svgElement.childNodes[i];
                                    scope.SelectedElementLane = scope.svgElement.childNodes[i];
                                    isLaneFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!isLaneFound) {
                        scope.SelectedElementLane = null;
                    }
                } else {
                    scope.SelectedElementLane = null;
                }

                removeRearrangingLinePoints(scope);
                removeResizerAroundElement(scope);
                if (scope.activeTab == "MAP" && scope.IsMapEditable && !scope.IsVersionDisplay && !evt.ctrlKey) {
                    GenerateBoundsDynamicallyToElement(scope.selectedElement, false, scope);
                }

                if (scope.selectedElement.getAttribute("id") != "lane") {
                    //For Multiple Selection
                    if (evt.ctrlKey) {
                        if (!isSelectionRemoved) {
                            generateSelectionRectangleDynamically(scope.selectedElement, scope);
                        }
                    }
                    else if (!isSame) {
                        generateSelectionRectangleDynamically(scope.selectedElement, scope);
                    }
                    if (scope.activeTab == "MAP" && scope.IsMapEditable && !scope.IsVersionDisplay && !evt.ctrlKey) {
                        scope.svgElement.setAttributeNS(null, "onmouseup", "deselectElement(evt,this)");
                        scope.svgElement.setAttributeNS(null, "onmousemove", "moveElement(evt,this)");
                    }
                } else {
                    scope.svgElement.removeAttributeNS(null, "onmousemove");
                    for (var i = 0; i < scope.selectedElement.childNodes.length; i++) {
                        if (scope.selectedElement.childNodes[i].nodeName == "rect" || scope.selectedElement.childNodes[i].nodeName == "line") {
                            scope.selectedElement.childNodes[i].setAttribute("stroke", "#dd4205");
                            scope.selectedElement.childNodes[i].setAttribute("stroke-width", "2");
                        }
                    }

                }

                scope.isMouseDown = true;
                scope.currentX = evt.offsetX;
                scope.currentY = evt.offsetY;
            }
            if (scope.activeTab != "EXECUTION") {
                showElementProperties(evt, ele);
            }

            if (scope.selectedShape) {
                scope.selectedElementID = scope.selectedShape.Id;
            }
        });
    }
}

function getMaxResizeLimitsOfParticipantorLane(scope) {
    scope.objMaxResizeLimits = {
    };
    if (scope.selectedShape && scope.selectedShape.ShapeType == "participant") {
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (scope.objData.lstShapes[i].ShapeName == "BPMNShape" && scope.objData.lstShapes[i].ShapeType != "participant") {
                if (scope.objData.lstShapes[i].processRef == scope.selectedShape.processRef) {
                    if (scope.objData.lstShapes[i].ShapeType != "lane") {
                        if (!scope.objMaxResizeLimits.Top || scope.objData.lstShapes[i].Top < scope.objMaxResizeLimits.Top) {
                            scope.objMaxResizeLimits.Top = scope.objData.lstShapes[i].Top;
                        }
                        if (!scope.objMaxResizeLimits.Right || (scope.objData.lstShapes[i].Left + scope.objData.lstShapes[i].Width) > scope.objMaxResizeLimits.Right) {
                            scope.objMaxResizeLimits.Right = (scope.objData.lstShapes[i].Left + scope.objData.lstShapes[i].Width);
                        }
                        if (!scope.objMaxResizeLimits.Left || scope.objData.lstShapes[i].Left < scope.objMaxResizeLimits.Left) {
                            scope.objMaxResizeLimits.Left = scope.objData.lstShapes[i].Left;
                        }

                        if (!scope.objMaxResizeLimits.Bottom || (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) > scope.objMaxResizeLimits.Bottom) {
                            scope.objMaxResizeLimits.Bottom = (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height);
                        }
                    } else {
                        if ((scope.objData.lstShapes[scope.indexofResizerElement].Top - scope.objData.lstShapes[i].Top) > -1 && (scope.objData.lstShapes[scope.indexofResizerElement].Top - scope.objData.lstShapes[i].Top) < 1) {
                            scope.objMaxResizeLimits.LaneTopLimit = (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) - 50;
                        }
                        if (((scope.objData.lstShapes[scope.indexofResizerElement].Top + scope.objData.lstShapes[scope.indexofResizerElement].Height) - (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height)) > -1 && ((scope.objData.lstShapes[scope.indexofResizerElement].Top + scope.objData.lstShapes[scope.indexofResizerElement].Height) - (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height)) < 1) {
                            scope.objMaxResizeLimits.LaneBottomLimit = scope.objData.lstShapes[i].Top + 50;
                        }
                    }
                }
            }
        }
    } else if (scope.selectedShape && scope.selectedShape.ShapeType == "lane") {
        scope.objMaxResizeLimits.Bottom = scope.selectedShape.Top + 150;
        if (scope.objMaxResizeLimits.Bottom > (scope.selectedShape.Top + scope.selectedShape.Height)) {
            scope.objMaxResizeLimits.Bottom = scope.selectedShape.Top + scope.selectedShape.Height;
        }
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (scope.objData.lstShapes[i].ShapeName == "BPMNShape" && scope.objData.lstShapes[i].ShapeType != "participant" && scope.objData.lstShapes[i].ShapeType != "lane") {
                if (scope.objData.lstShapes[i].Top > scope.selectedShape.Top && scope.objData.lstShapes[i].Top < (scope.selectedShape.Top + scope.selectedShape.Height) && (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) > scope.objMaxResizeLimits.Bottom) {
                    scope.objMaxResizeLimits.Bottom = (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height);
                }
            }
        }
    }
}

function showElementProperties(evt, ele) {
    var scope = angular.element(ele).scope();
    if (scope && scope.selectedShape && scope.showDetails) {
        scope.showDetails();
    }
}

function selectPropertiesTab(evt) {
    if (evt) {
        var scope = angular.element(evt.currentTarget).scope();
        if (scope && scope.selectedShape) {
            scope.$apply(function () {
                scope.ActiveTabForBPM = 'Properties';
                scope.IsToolsDivCollapsed = false;
            });
        }
    }
}

function moveElement(evt, ele) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(ele).scope();
    if (scope && evt.buttons == 1) {
        scope.$apply(function () {
            if (scope.isMouseDown && (scope.currentX != evt.offsetX || scope.currentY != evt.offsetY)) {
                scope.isDirty = true;
                if (scope.ClonedElement == null) {
                    scope.ClonedElement = scope.selectedElement.cloneNode(true);
                    scope.ClonedElement.setAttribute("style", "opacity: 0.5;");
                    scope.svgElement.appendChild(scope.ClonedElement);
                }
                var dx = evt.offsetX - scope.currentX;
                var dy = evt.offsetY - scope.currentY;
                if (scope.selectedElement != 0 && scope.ClonedElement != null && ((parseFloat(scope.ClonedElement.getAttribute("x")) + dx) > 0) && ((parseFloat(scope.ClonedElement.getAttribute("y")) + dy) > 0)) {
                    if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType != "participant") {
                        setLaneBorderBasedOnElementPosition(scope.ClonedElement, scope);
                    }

                    scope.ClonedElement.setAttribute('x', parseFloat(scope.ClonedElement.getAttribute("x")) + dx);
                    scope.ClonedElement.setAttribute('y', parseFloat(scope.ClonedElement.getAttribute("y")) + dy);
                    var isTextOut = false;
                    var ElementIndex = 0;
                    var mainElement = "";
                    for (var i = 0; i < $(scope.ClonedElement).children().length; i++) {
                        var item = $(scope.ClonedElement).children()[i];
                        if (item.getAttribute("id") == "MainElement") {
                            mainElement = item;
                        }
                        if (item.nodeName == "polygon") {
                            isTextOut = true;
                            var Left = parseFloat(scope.ClonedElement.getAttribute("x"));
                            var Top = parseFloat(scope.ClonedElement.getAttribute("y"));
                            var Height = parseFloat(scope.ClonedElement.getAttribute("height"));
                            var Width = parseFloat(scope.ClonedElement.getAttribute("width"));
                            var points;
                            var point1 = {
                                X: Left,
                                Y: Top + (Height / 2)
                            };
                            var point2 = {
                                X: Left + (Width / 2),
                                Y: Top
                            };
                            var point3 = {
                                X: Left + Width,
                                Y: Top + (Height / 2)
                            };
                            var point4 = {
                                X: Left + (Width / 2),
                                Y: Top + Height
                            };
                            points = point1.X + "," + point1.Y + "  " + point2.X + "," + point2.Y + "  " + point3.X + "," + point3.Y + "  " + point4.X + "," + point4.Y;
                            item.setAttribute("points", points);
                            item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                            item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        }

                        else if (item.nodeName == "circle") {

                            item.setAttribute("cx", parseFloat(item.getAttribute("cx")) + dx);
                            item.setAttribute("cy", parseFloat(item.getAttribute("cy")) + dy);
                            isTextOut = true;
                        }
                        else if (item.nodeName != "text" && item.getAttribute("id") != "SelectionElement" && item.nodeName != "line" && item.nodeName != "g") {
                            item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                            item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        }
                        else if (item.getAttribute("id") == "SelectionElement") {
                            item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                            item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);
                        }
                        else if (item.nodeName == "line") {
                            item.setAttribute('x1', parseFloat(item.getAttribute("x1")) + dx);
                            item.setAttribute('y1', parseFloat(item.getAttribute("y1")) + dy);
                            item.setAttribute('x2', parseFloat(item.getAttribute("x2")) + dx);
                            item.setAttribute('y2', parseFloat(item.getAttribute("y2")) + dy);
                        }
                        else if (item.nodeName == "g") {
                            //function nestedIterator(itm) {
                            for (j = 0; j < $(item).children().length; j++) {
                                var itm = $(item).children()[j];
                                if (itm.nodeName == "line") {
                                    itm.setAttribute('x1', parseFloat(itm.getAttribute("x1")) + dx);
                                    itm.setAttribute('y1', parseFloat(itm.getAttribute("y1")) + dy);
                                    itm.setAttribute('x2', parseFloat(itm.getAttribute("x2")) + dx);
                                    itm.setAttribute('y2', parseFloat(itm.getAttribute("y2")) + dy);
                                }
                            }
                            //}
                            // angular.forEach($(item).children(), nestedIterator);
                        }
                        if (item.nodeName == "text") {
                            if (scope.ClonedElement.id != "participant") {
                                item.setAttribute('x', parseFloat(item.getAttribute('x')) + dx);
                                item.setAttribute('y', parseFloat(item.getAttribute('y')) + dy);
                                //function nestedIterator(itm) {
                                for (var k = 0; k < $(item).children().length; k++) {
                                    var itm = $(item).children()[k];
                                    itm.setAttribute('x', parseFloat(itm.getAttribute('x')) + dx);
                                }
                                //angular.forEach($(item).children(), nestedIterator);
                            }
                            else {
                                item.setAttribute('x', parseFloat(item.getAttribute('x')) + dx);
                                item.setAttribute('y', parseFloat(item.getAttribute('y')) + dy);
                                //var tx = parseFloat(item.getAttribute('x')) + 5 + (scope.objData.lstShapes[scope.indexofResizerElement].LabelWidth / 2);
                                //var ty = parseFloat(item.getAttribute('y')) + (scope.objData.lstShapes[scope.indexofResizerElement].LabelHeight / 2);
                                var tx = parseFloat(item.getAttribute('x'));
                                var ty = parseFloat(item.getAttribute('y'));
                                if (mainElement) {
                                    var height = parseFloat(mainElement.getAttribute('height'));
                                    var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                                    var textnode = wraptorect(item, mainElement, 0, 0.5, "Participant", scope);
                                    var translate = 10 + ' ' + height / 2;
                                    textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                                    textnode.setAttribute('text-anchor', 'middle');
                                }
                            }
                        }

                    }
                    //angular.forEach($(scope.ClonedElement).children(), iterator)

                    scope.isElementMoved = true;
                    scope.currentX = evt.offsetX;
                    scope.currentY = evt.offsetY;
                } else {
                    deselectElement(evt, ele);
                }
            }
        });
    } else {
        deselectElement(evt, ele);
    }
}

function setLaneBorderBasedOnElementPosition(selectedEle, scope) {
    var xpos = parseFloat(selectedEle.getAttribute("x"));
    var ypos = parseFloat(selectedEle.getAttribute("y"));
    var isSame = false;
    if (scope.LaneElement != null) {
        var xlanepos = parseFloat(scope.LaneElement.getAttribute("x"));
        var ylanepos = parseFloat(scope.LaneElement.getAttribute("y"));
        var laneHeight = parseFloat(scope.LaneElement.getAttribute("height"));
        var laneWidth = parseFloat(scope.LaneElement.getAttribute("width"));
        if (xpos >= xlanepos && xpos <= (xlanepos + laneWidth) && ypos >= ylanepos && ypos <= (ylanepos + laneHeight)) {
            isSame = true;
        }
        else {
            for (var j = 0; j < scope.LaneElement.childNodes.length; j++) {
                if (scope.LaneElement.childNodes[j].id == "MainElement") {
                    scope.LaneElement.childNodes[j].setAttribute("stroke-width", 1);
                    scope.LaneElement.childNodes[j].setAttribute("stroke", "Black");
                    break;
                }
            }
            scope.LaneElement = null;
        }
    }
    if (!isSame) {
        for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
            if (scope.svgElement.childNodes[i].id == "lane") {
                var lanexpos = parseFloat(scope.svgElement.childNodes[i].getAttribute("x"));
                var laneypos = parseFloat(scope.svgElement.childNodes[i].getAttribute("y"));
                var laneHeight = parseFloat(scope.svgElement.childNodes[i].getAttribute("height"));
                var laneWidth = parseFloat(scope.svgElement.childNodes[i].getAttribute("width"));
                if (xpos >= lanexpos && xpos <= (lanexpos + laneWidth) && ypos >= laneypos && ypos <= (laneypos + laneHeight)) {
                    scope.LaneElement = scope.svgElement.childNodes[i];
                    for (var j = 0; j < scope.svgElement.childNodes[i].childNodes.length; j++) {
                        if (scope.svgElement.childNodes[i].childNodes[j].id == "MainElement") {
                            scope.svgElement.childNodes[i].childNodes[j].setAttribute("stroke-width", 5);
                            scope.svgElement.childNodes[i].childNodes[j].setAttribute("stroke", "#723837");
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}

function deselectElement(evt, ele) {
    var scope = angular.element(ele).scope();
    scope.isMouseDown = false;
    if (scope.selectedElement && scope.selectedElement != 0 && scope.ClonedElement != null) {
        setLaneBorderBasedOnElementPosition(scope.ClonedElement, scope);
        scope.ClonedElement.removeAttributeNS(null, "style");
        scope.svgElement.removeAttributeNS(null, "onmousemove");
        scope.svgElement.removeAttributeNS(null, "onmouseup");
        var clonedElementXpos = parseFloat(scope.ClonedElement.getAttribute("x"));
        var clonedElementYpos = parseFloat(scope.ClonedElement.getAttribute("y"));
        var xposOfSelectedElement = scope.objData.lstShapes[scope.indexofResizerElement].Left;
        var yposOfSelectedElement = scope.objData.lstShapes[scope.indexofResizerElement].Top;
        var xPosdiff = xposOfSelectedElement - clonedElementXpos;
        var yPosdiff = yposOfSelectedElement - clonedElementYpos;
        if ((isElementInsideTheLane(scope.ClonedElement, scope) || scope.ClonedElement.id == 'participant') && (xPosdiff > 10 || xPosdiff < -10 || yPosdiff > 10 || yPosdiff < -10)) {
            removeResizerAroundElement(scope);
            // getting values from cloned Element to actual Element
            scope.selectedElement.setAttribute("x", clonedElementXpos);
            scope.selectedElement.setAttribute("y", clonedElementYpos);
            scope.selectedElement.setAttribute("height", parseFloat(scope.ClonedElement.getAttribute("height")));
            scope.selectedElement.setAttribute("width", parseFloat(scope.ClonedElement.getAttribute("width")));

            for (var i = 0; i < scope.selectedElement.childNodes.length; i++) {
                if (scope.selectedElement.childNodes[i].nodeName == "rect" || scope.selectedElement.childNodes[i].nodeName == "polygon" || scope.selectedElement.childNodes[i].nodeName == "image") {
                    scope.selectedElement.childNodes[i].setAttribute("x", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("x")));
                    scope.selectedElement.childNodes[i].setAttribute("y", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("y")));
                    scope.selectedElement.childNodes[i].setAttribute("height", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("height")));
                    scope.selectedElement.childNodes[i].setAttribute("width", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("width")));
                    if (scope.selectedElement.childNodes[i].nodeName == "polygon") {
                        scope.selectedElement.childNodes[i].setAttribute("points", scope.ClonedElement.childNodes[i].getAttribute("points"));
                    }
                }
                else if (scope.selectedElement.childNodes[i].nodeName == "circle") {
                    scope.selectedElement.childNodes[i].setAttribute("cx", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("cx")));
                    scope.selectedElement.childNodes[i].setAttribute("cy", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("cy")));
                    scope.selectedElement.childNodes[i].setAttribute("r", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("r")));
                }
                else if (scope.selectedElement.childNodes[i].nodeName == "text") {
                    scope.selectedElement.childNodes[i].setAttribute("x", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("x")));
                    scope.selectedElement.childNodes[i].setAttribute("y", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("y")));
                    if (scope.selectedElement.childNodes[i].childNodes.length > 0) {
                        for (var j = 0; j < scope.selectedElement.childNodes[i].childNodes.length; j++) {
                            if (scope.selectedElement.childNodes[i].childNodes[j].nodeName == 'tspan') {
                                scope.selectedElement.childNodes[i].childNodes[j].setAttribute("x", parseFloat(scope.ClonedElement.childNodes[i].childNodes[j].getAttribute("x")));
                            }
                        }
                    }
                }
                else if (scope.selectedElement.childNodes[i].nodeName == "line") {
                    scope.selectedElement.childNodes[i].setAttribute("x1", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("x1")));
                    scope.selectedElement.childNodes[i].setAttribute("y1", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("y1")));
                    scope.selectedElement.childNodes[i].setAttribute("x2", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("x2")));
                    scope.selectedElement.childNodes[i].setAttribute("y2", parseFloat(scope.ClonedElement.childNodes[i].getAttribute("y2")));
                }
                else if (scope.selectedElement.childNodes[i].nodeName == "g") {
                    for (var k = 0; k < scope.selectedElement.childNodes[i].childNodes.length; k++) {
                        if (scope.selectedElement.childNodes[i].childNodes[k].nodeName == "line") {
                            scope.selectedElement.childNodes[i].childNodes[k].setAttribute("x1", parseFloat(scope.ClonedElement.childNodes[i].childNodes[k].getAttribute("x1")));
                            scope.selectedElement.childNodes[i].childNodes[k].setAttribute("y1", parseFloat(scope.ClonedElement.childNodes[i].childNodes[k].getAttribute("y1")));
                            scope.selectedElement.childNodes[i].childNodes[k].setAttribute("x2", parseFloat(scope.ClonedElement.childNodes[i].childNodes[k].getAttribute("x2")));
                            scope.selectedElement.childNodes[i].childNodes[k].setAttribute("y2", parseFloat(scope.ClonedElement.childNodes[i].childNodes[k].getAttribute("y2")));
                        }
                    }
                }
            }
            scope.svgElement.removeChild(scope.ClonedElement, false);
            scope.ClonedElement = null;
            var xdiff = parseFloat(scope.selectedElement.getAttribute("x")) - scope.objData.lstShapes[scope.indexofResizerElement].Left;
            var ydiff = parseFloat(scope.selectedElement.getAttribute("y")) - scope.objData.lstShapes[scope.indexofResizerElement].Top;

            angular.forEach(scope.selectedElement.childNodes, function (itm) {
                if (itm.nodeName == "text") {
                    scope.objData.lstShapes[scope.indexofResizerElement].LabelLeft = parseFloat(itm.getAttribute("x"));
                    scope.objData.lstShapes[scope.indexofResizerElement].LabelTop = parseFloat(itm.getAttribute("y"));
                }
            });

            dx = evt.offsetX - scope.currentX;
            dy = evt.offsetY - scope.currentY;
            if (scope.isElementMoved) {
                scope.objData.lstShapes[scope.indexofResizerElement].Left = parseFloat(scope.selectedElement.getAttribute("x"));
                scope.objData.lstShapes[scope.indexofResizerElement].Top = parseFloat(scope.selectedElement.getAttribute("y"));
                scope.objData.lstShapes[scope.indexofResizerElement].Height = parseFloat(scope.selectedElement.getAttribute("height"));
                scope.objData.lstShapes[scope.indexofResizerElement].Width = parseFloat(scope.selectedElement.getAttribute("width"));

                if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "participant") {
                    var processReference = scope.objData.lstShapes[scope.indexofResizerElement].processRef;
                    UdjustCoordinatesForChildElementsOfParticipant(xdiff, ydiff, processReference, scope);
                    RedrawingElementsAndEdges(scope);
                    for (var k = 0; k < scope.svgElement.childNodes.length; k++) {
                        if (scope.svgElement.childNodes[k].nodeName != "defs") {
                            if (scope.objData.lstShapes[scope.indexofResizerElement].Left == parseFloat(scope.svgElement.childNodes[k].getAttribute("x")) && scope.objData.lstShapes[scope.indexofResizerElement].Top == parseFloat(scope.svgElement.childNodes[k].getAttribute("y")) &&
                                scope.objData.lstShapes[scope.indexofResizerElement].Height == parseFloat(scope.svgElement.childNodes[k].getAttribute("height")) && scope.objData.lstShapes[scope.indexofResizerElement].Width == parseFloat(scope.svgElement.childNodes[k].getAttribute("width"))) {
                                generateSelectionRectangleDynamically(scope.svgElement.childNodes[k], scope);
                                scope.selectedElement = scope.svgElement.childNodes[k];
                                scope.selectedElemForResize = scope.selectedElement;
                                break;
                            }
                        }
                    }
                }

                ResetlaneStrokeWidth(scope);
                if (scope.selectedShape && scope.selectedShape.ShapeType != "participant") {
                    if (scope.LaneElement != null && scope.PrevlaneElement != null) {
                        var isSamelane = false;
                        // check currentlane and prev Lane is Same or Not
                        if (parseFloat(scope.PrevlaneElement.getAttribute("x")) == parseFloat(scope.LaneElement.getAttribute("x")) && parseFloat(scope.PrevlaneElement.getAttribute("y")) == parseFloat(scope.LaneElement.getAttribute("y"))
                            && parseFloat(scope.PrevlaneElement.getAttribute("height")) == parseFloat(scope.LaneElement.getAttribute("height")) && parseFloat(scope.PrevlaneElement.getAttribute("width")) == parseFloat(scope.LaneElement.getAttribute("width"))) {
                            isSamelane = true;
                        }
                        if (!isSamelane) {
                            RemoveAndAddnodeInThelaneShapeModel(scope.PrevlaneElement, scope.LaneElement, scope);
                        }
                        scope.PrevlaneElement = null;
                        scope.LaneElement = null;
                    }
                    else if (scope.LaneElement != null && scope.PrevlaneElement == null) {
                        RemoveAndAddnodeInThelaneShapeModel(scope.PrevlaneElement, scope.LaneElement, scope);
                        scope.LaneElement = null;
                    }
                    else if (scope.LaneElement == null && scope.PrevlaneElement != null) {
                        RemoveAndAddnodeInThelaneShapeModel(scope.PrevlaneElement, scope.LaneElement, scope);
                        scope.PrevlaneElement = null;
                    }
                }
                if (scope.selectedElement.id != "message") {
                    var ydiffislanedeleted;
                    ResetEdgePoints(scope.selectedElement, ydiffislanedeleted, scope);
                }
            }
            GenerateBoundsDynamicallyToElement(scope.selectedElement, false, scope);
            scope.selectedElement = 0;
        } else {
            scope.svgElement.removeChild(scope.ClonedElement, false);
            ResetlaneStrokeWidth(scope);
            scope.ClonedElement = null;
        }
    }
    if (scope.selectedElement && scope.selectedElement != 0) {
        scope.svgElement.removeAttributeNS(null, "onmousemove");
        scope.svgElement.removeAttributeNS(null, "onmouseup");
    }
    ResetlaneStrokeWidth(scope);
}

function ResetlaneStrokeWidth(scope) {
    if (scope && scope.LaneElement != null) {
        for (var j = 0; j < scope.LaneElement.childNodes.length; j++) {
            if (scope.LaneElement.childNodes[j].id == "MainElement") {
                scope.LaneElement.childNodes[j].setAttribute("stroke-width", 1);
                scope.LaneElement.childNodes[j].setAttribute("stroke", "Black");
                break;
            }
        }
    }
}

function isElementInsideTheLane(Element, scope) {
    var xpos = parseFloat(Element.getAttribute("x"));
    var ypos = parseFloat(Element.getAttribute("y"));
    var isFound = false;
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType == "lane") {
            if (scope.objData.lstShapes[i].Left <= xpos && (scope.objData.lstShapes[i].Left + scope.objData.lstShapes[i].Width) >= xpos && scope.objData.lstShapes[i].Top <= ypos && (scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) >= ypos) {
                isFound = true;
                break;
            }
        }
    }
    return isFound;
}

function RemoveAndAddnodeInThelaneShapeModel(prevLane, currentLane, scope) {
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType == "lane") {
            if (prevLane != null) {
                if ((-0.1 < (scope.objData.lstShapes[i].Left - parseFloat(prevLane.getAttribute("x")))) && ((scope.objData.lstShapes[i].Left - parseFloat(prevLane.getAttribute("x"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - parseFloat(prevLane.getAttribute("y")))) && ((scope.objData.lstShapes[i].Top - parseFloat(prevLane.getAttribute("y"))) < 0.1) &&
                    (-0.1 < (scope.objData.lstShapes[i].Height - parseFloat(prevLane.getAttribute("height")))) && ((scope.objData.lstShapes[i].Height - parseFloat(prevLane.getAttribute("height"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Width - parseFloat(prevLane.getAttribute("width")))) && ((scope.objData.lstShapes[i].Width - parseFloat(prevLane.getAttribute("width"))) < 0.1)) {
                    for (var j = 0; j < scope.objData.lstShapes[i].ShapeModel.Elements.length; j++) {
                        if (scope.objData.lstShapes[i].ShapeModel.Elements[j].Value == scope.objData.lstShapes[scope.indexofResizerElement].Id) {
                            scope.objData.lstShapes[i].ShapeModel.Elements.splice(j, 1);
                            break;
                        }
                    }
                }
            }

            if (currentLane != null) {
                if ((-0.1 < (scope.objData.lstShapes[i].Left - parseFloat(currentLane.getAttribute("x")))) && ((scope.objData.lstShapes[i].Left - parseFloat(currentLane.getAttribute("x"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - parseFloat(currentLane.getAttribute("y")))) && ((scope.objData.lstShapes[i].Top - parseFloat(currentLane.getAttribute("y"))) < 0.1) &&
                    (-0.1 < (scope.objData.lstShapes[i].Height - parseFloat(currentLane.getAttribute("height")))) && ((scope.objData.lstShapes[i].Height - parseFloat(currentLane.getAttribute("height"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Width - parseFloat(currentLane.getAttribute("width")))) && ((scope.objData.lstShapes[i].Width - parseFloat(currentLane.getAttribute("width"))) < 0.1)) {
                    var objflowNode = {
                        dictAttributes: {}, Elements: [], Children: [], Name: "flowNodeRef", prefix: null, Value: scope.objData.lstShapes[scope.indexofResizerElement].Id
                    };
                    scope.objData.lstShapes[i].ShapeModel.Elements.push(objflowNode);
                    scope.objData.lstShapes[scope.indexofResizerElement].processRef = scope.objData.lstShapes[i].processRef;
                    scope.SelectedElementLane = currentLane;
                }
            }
            else {
                //if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType != "participant") {
                //    //if (scope.objData.DefaultProcessId == "") {
                //    //    scope.objData.DefaultProcessId = "PROCESS_" + generateUUID();
                //    //}
                //    scope.objData.lstShapes[scope.indexofResizerElement].processRef = scope.objData.DefaultProcessId;
                //    scope.SelectedElementLane = null;
                //}
            }
        }
    }
}

function clearShapes(scope) {
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].nodeName != "defs") {
            scope.svgElement.removeChild((scope.svgElement.childNodes[i]));
            i--;
        }
        else if (scope.svgElement.childNodes.length == 1) {
            break;
        }
    }
}

function RedrawingElementsAndEdges(scope) {
    clearShapes(scope);
    scope.FilterTheElementsBasedOnTypeAndLoadShapes(scope.objData.lstShapes);
}

function UdjustCoordinatesForChildElementsOfParticipant(xdiff, ydiff, processReference, scope) {
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType != "participant") {
            if (scope.objData.lstShapes[i].processRef == processReference) {
                if (scope.objData.lstShapes[i].ShapeName == "BPMNEdge") {
                    if (scope.objData.lstShapes[i].lstWayPoints) {
                        for (var j = 0; j < scope.objData.lstShapes[i].lstWayPoints.length; j++) {
                            scope.objData.lstShapes[i].lstWayPoints[j].Left = scope.objData.lstShapes[i].lstWayPoints[j].Left + xdiff;
                            scope.objData.lstShapes[i].lstWayPoints[j].Top = scope.objData.lstShapes[i].lstWayPoints[j].Top + ydiff;
                        }
                        scope.objData.lstShapes[i].LabelLeft = scope.objData.lstShapes[i].LabelLeft + xdiff;
                        scope.objData.lstShapes[i].LabelTop = scope.objData.lstShapes[i].LabelTop + ydiff;
                    }
                }
                else if (scope.objData.lstShapes[i].ShapeName == "BPMNShape") {
                    scope.objData.lstShapes[i].Left = scope.objData.lstShapes[i].Left + xdiff;
                    scope.objData.lstShapes[i].Top = scope.objData.lstShapes[i].Top + ydiff;
                    scope.objData.lstShapes[i].LabelLeft = scope.objData.lstShapes[i].LabelLeft + xdiff;
                    scope.objData.lstShapes[i].LabelTop = scope.objData.lstShapes[i].LabelTop + ydiff;
                }
            }
        }
    }

    // iteration for MessageFlow, dataIO Association and Annotation
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeName == "BPMNEdge") {
            var srcElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].SourceElement, scope, "Element");
            var tarElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].TargetElement, scope, "Element");
            if (srcElement.processRef != tarElement.processRef) {
                if (srcElement.processRef == processReference || tarElement.processRef == processReference) {
                    var newWayPoints = GetEdgeWayPointsForDottedLine(srcElement, tarElement);
                    newWayPoints = setShapeNameToWayPoints(newWayPoints);
                    if (srcElement.ShapeType == "textAnnotation" || tarElement.ShapeType == "textAnnotation") {
                        var length = newWayPoints.length;
                        if (newWayPoints.length > 2) {
                            var numberofElementsToRemove = length - 2;
                            newWayPoints.splice(1, numberofElementsToRemove);
                        }
                    }
                    //if (srcElement.ShapeType != "participant" && tarElement.ShapeType != "participant") {
                    scope.objData.lstShapes[i].lstWayPoints = newWayPoints;
                    //}
                }
            }
        }
    }
}

function ResetEdgePoints(selectedelem, ydiffislanedeleted, scope) {
    //var scope = getCurrentFileScope();
    var xdiff;
    var ydiff;
    var objshape;
    if (!ydiffislanedeleted && typeof ydiffislanedeleted === 'undefined') {
        xdiff = parseFloat(selectedelem.getAttribute("x")) - parseFloat(scope.selectedElemstartLeft);
        ydiff = parseFloat(selectedelem.getAttribute("y")) - parseFloat(scope.selectedElemstartTop);

        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (selectedelem.firstChild.nodeName == "circle") {
                if (parseFloat(selectedelem.getAttribute("x")) == scope.objData.lstShapes[i].Left && scope.objData.lstShapes[i].Top == parseFloat(selectedelem.getAttribute("y"))) {
                    objshape = scope.objData.lstShapes[i];
                    break;
                }
            }
            else {
                if (parseFloat(selectedelem.getAttribute("x")) == scope.objData.lstShapes[i].Left && scope.objData.lstShapes[i].Top == parseFloat(selectedelem.getAttribute("y"))) {
                    objshape = scope.objData.lstShapes[i];
                    break;
                }
            }
        }
    } else {
        xdiff = 0;
        ydiff = -(ydiffislanedeleted);
        objshape = selectedelem;
    }


    //selected Element object


    var IsMsgFlowLine = false;
    if (objshape.processRef == scope.objData.DefaultProcessId) {
        IsMsgFlowLine = true;
    }
    resetEdgePointsForSelectedShape(objshape, xdiff, ydiff, scope, ydiffislanedeleted);
}

function resetEdgePointsForSelectedShape(objshape, xdiff, ydiff, scope, ydiffislanedeleted) {
    if (objshape != undefined) {
        var lstSourcePoints = [];
        var lstTargetPoints = [];
        // source
        lstSourcePoints = getSourcePointsofElement(objshape, scope);
        if (lstSourcePoints.length > 0) {
            for (var j = 0; j < lstSourcePoints.length; j++) {
                var objSourceElement = null;
                var objTargetElement = null;
                for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                    if (scope.objData.lstShapes[i].Id == lstSourcePoints[j].SourceElement) {
                        objSourceElement = scope.objData.lstShapes[i];
                    }
                    else if (scope.objData.lstShapes[i].Id == lstSourcePoints[j].TargetElement) {
                        objTargetElement = scope.objData.lstShapes[i];
                    }
                }
                UpdateEdgeBound1(false, lstSourcePoints[j], xdiff, ydiff, objSourceElement, objTargetElement, scope);
            }
        }
        // Target
        var lstTargetPoints = getTargetPointsofElement(objshape, scope);
        if (lstTargetPoints.length > 0) {
            for (var j = 0; j < lstTargetPoints.length; j++) {
                var objSourceElement = null;
                var objTargetElement = null;
                for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                    if (scope.objData.lstShapes[i].Id == lstTargetPoints[j].SourceElement) {
                        objSourceElement = scope.objData.lstShapes[i];
                    }
                    else if (scope.objData.lstShapes[i].Id == lstTargetPoints[j].TargetElement) {
                        objTargetElement = scope.objData.lstShapes[i];
                    }
                }
                UpdateEdgeBound1(true, lstTargetPoints[j], xdiff, ydiff, objSourceElement, objTargetElement, scope);
            }
        }

        if (!ydiffislanedeleted) {
            ReDrawingEdges(scope);
        }
    }
}
function getTargetPointsofElement(objshape, scope) {
    var lstTargetPoints = [];
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (objshape.Id == scope.objData.lstShapes[i].TargetElement) {
            isValidEdge = true;
            if (scope.objData.lstShapes[i].ShapeType != "association" && scope.objData.lstShapes[i].ShapeType != "dataOutputAssociation" && scope.objData.lstShapes[i].ShapeType != "dataInputAssociation") {
                var sourceProcessId = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].SourceElement, scope, "processId");
                var srcElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].SourceElement, scope, "Element");
                var tarElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].TargetElement, scope, "Element");
                if (sourceProcessId == objshape.processRef) {
                    if (scope.objData.lstShapes[i].ShapeType != "sequenceFlow" && scope.objData.lstShapes[i].ShapeType != "association") {
                        if (scope.objData.lstShapes[i].MessageReference != "" && scope.objData.lstShapes[i].MessageVisibleKind != "") {
                            var objShape = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].MessageReference, scope, "Delete");
                            scope.objData.lstShapes[i].MessageReference = "";
                            scope.objData.lstShapes[i].MessageVisibleKind = "";
                            removeElementFromSvg(objShape, scope);
                        }

                        if ((tarElement.ShapeType == "startEvent" || srcElement.ShapeType == "endEvent" || tarElement.ShapeType == "endEvent" || srcElement.ShapeType == "startEvent") && scope.objData.lstShapes[i].ShapeType != "sequenceFlow") {
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            DisplayMessage(srcElement, tarElement);
                            isValidEdge = false;
                        }
                        else {
                            scope.objData.lstShapes[i].ShapeType = "sequenceFlow";
                            EdgeShapeModel = {
                                dictAttributes: { name: scope.objData.lstShapes[i].Text, id: scope.objData.lstShapes[i].ShapeModel.dictAttributes.id, sourceRef: scope.objData.lstShapes[i].SourceElement, targetRef: scope.objData.lstShapes[i].TargetElement }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: ""
                            };
                            scope.objData.lstShapes[i].ShapeModel = EdgeShapeModel;
                            scope.objData.lstShapes[i].processRef = objshape.processRef;
                        }
                    }
                }
                else {
                    if (scope.objData.lstShapes[i].ShapeType != "messageFlow" && scope.objData.lstShapes[i].ShapeType != "association" && srcElement.ShapeType != "inclusiveGateway" && srcElement.ShapeType != "exclusiveGateway"
                        && srcElement.ShapeType != "parallelGateway" && srcElement.ShapeType != "eventBasedGateway" && tarElement.ShapeType != "inclusiveGateway" && tarElement.ShapeType != "exclusiveGateway"
                        && tarElement.ShapeType != "parallelGateway" && tarElement.ShapeType != "eventBasedGateway") {

                        if ((srcElement.ShapeType == "startEvent" || srcElement.ShapeType == "endEvent" || tarElement.ShapeType == "startEvent" || tarElement.ShapeType == "endEvent") && scope.objData.lstShapes[i].ShapeType != "messageFlow") {
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            DisplayMessage(srcElement, tarElement);
                            isValidEdge = false;
                        }
                        else {
                            scope.objData.lstShapes[i].ShapeType = "messageFlow";
                            //var messageFlowId = generateUUID();
                            var messageFlowId = scope.objData.lstShapes[i].ShapeModel.dictAttributes.id;
                            var MessageFlowModel = {
                                dictAttributes: {
                                    id: messageFlowId, name: scope.objData.lstShapes[i].Text, sourceRef: scope.objData.lstShapes[i].SourceElement, targetRef: scope.objData.lstShapes[i].TargetElement
                                },
                                Elements: [{
                                    dictAttributes: {
                                    }, Elements: [{
                                        dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                                    },
                                    {
                                        dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False"
                                    }], Children: [],
                                    Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                                }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                            };
                            scope.objData.lstShapes[i].ShapeModel = MessageFlowModel;
                            if (scope.objData.CollaborationId == "") {
                                scope.objData.CollaborationId = "COLLABORATION_" + generateUUID();
                            }
                            scope.objData.lstShapes[i].processRef = scope.objData.CollaborationId;
                        }
                    }
                    else {
                        if (srcElement.ShapeType == "inclusiveGateway" || srcElement.ShapeType == "exclusiveGateway" || srcElement.ShapeType == "parallelGateway" || srcElement.ShapeType == "eventBasedGateway"
                            || tarElement.ShapeType == "inclusiveGateway" || tarElement.ShapeType == "exclusiveGateway" || tarElement.ShapeType == "parallelGateway" || tarElement.ShapeType == "eventBasedGateway") {
                            alert("The gateway must not have any incoming or outgoing message");
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            isValidEdge = false;
                        }
                    }
                }
            }
            if (isValidEdge) {
                lstTargetPoints.push(scope.objData.lstShapes[i]);
            }
        }
    }
    return lstTargetPoints;
}

function getSourcePointsofElement(objshape, scope) {
    var isValidEdge = true;
    var lstSourcePoints = [];
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (objshape.Id == scope.objData.lstShapes[i].SourceElement) {
            isValidEdge = true;
            if (scope.objData.lstShapes[i].ShapeType != "association" && scope.objData.lstShapes[i].ShapeType != "dataOutputAssociation" && scope.objData.lstShapes[i].ShapeType != "dataInputAssociation") {
                var TargetProcessId = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].TargetElement, scope, "processId");
                var srcElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].SourceElement, scope, "Element");
                var tarElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].TargetElement, scope, "Element");
                if (TargetProcessId == objshape.processRef) {
                    if (scope.objData.lstShapes[i].ShapeType != "sequenceFlow" && scope.objData.lstShapes[i].ShapeType != "association") {
                        if (scope.objData.lstShapes[i].MessageReference != "" && scope.objData.lstShapes[i].MessageVisibleKind != "") {
                            var objShape = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].MessageReference, scope, "Delete");
                            scope.objData.lstShapes[i].MessageReference = "";
                            scope.objData.lstShapes[i].MessageVisibleKind = "";
                            removeElementFromSvg(objShape, scope);
                        }
                        if ((tarElement.ShapeType == "startEvent" || srcElement.ShapeType == "endEvent" || tarElement.ShapeType == "endEvent" || srcElement.ShapeType == "startEvent") && scope.objData.lstShapes[i].ShapeType != "sequenceFlow") {
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            DisplayMessage(srcElement, tarElement);
                            isValidEdge = false;
                        }
                        else {
                            scope.objData.lstShapes[i].ShapeType = "sequenceFlow";
                            EdgeShapeModel = {
                                dictAttributes: { id: scope.objData.lstShapes[i].ShapeModel.dictAttributes.id, name: scope.objData.lstShapes[i].Text, sourceRef: scope.objData.lstShapes[i].SourceElement, targetRef: scope.objData.lstShapes[i].TargetElement }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: ""
                            };
                            scope.objData.lstShapes[i].ShapeModel = EdgeShapeModel;
                            scope.objData.lstShapes[i].processRef = objshape.processRef;
                        }
                    }
                }
                else {
                    if (scope.objData.lstShapes[i].ShapeType != "messageFlow" && scope.objData.lstShapes[i].ShapeType != "association" && srcElement.ShapeType != "inclusiveGateway" && srcElement.ShapeType != "exclusiveGateway"
                        && srcElement.ShapeType != "parallelGateway" && srcElement.ShapeType != "eventBasedGateway" && tarElement.ShapeType != "inclusiveGateway" && tarElement.ShapeType != "exclusiveGateway"
                        && tarElement.ShapeType != "parallelGateway" && tarElement.ShapeType != "eventBasedGateway") {

                        if ((tarElement.ShapeType == "startEvent" || srcElement.ShapeType == "endEvent" || tarElement.ShapeType == "endEvent" || srcElement.ShapeType == "startEvent") && scope.objData.lstShapes[i].ShapeType != "messageFlow") {
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            DisplayMessage(srcElement, tarElement);
                            isValidEdge = false;
                        }
                        else {
                            scope.objData.lstShapes[i].ShapeType = "messageFlow";
                            //var messageFlowId = generateUUID();
                            var messageFlowId = scope.objData.lstShapes[i].ShapeModel.dictAttributes.id;
                            var MessageFlowModel = {
                                dictAttributes: {
                                    id: messageFlowId, name: scope.objData.lstShapes[i].Text, sourceRef: scope.objData.lstShapes[i].SourceElement, targetRef: scope.objData.lstShapes[i].TargetElement
                                },
                                Elements: [{
                                    dictAttributes: {
                                    }, Elements: [{
                                        dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                                    },
                                    {
                                        dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False"
                                    }], Children: [],
                                    Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                                }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                            };
                            scope.objData.lstShapes[i].ShapeModel = MessageFlowModel;
                            if (scope.objData.CollaborationId == "") {
                                scope.objData.CollaborationId = "COLLABORATION_" + generateUUID();
                            }
                            scope.objData.lstShapes[i].processRef = scope.objData.CollaborationId;
                        }
                    }
                    else {
                        if (srcElement.ShapeType == "inclusiveGateway" || srcElement.ShapeType == "exclusiveGateway" || srcElement.ShapeType == "parallelGateway" || srcElement.ShapeType == "eventBasedGateway"
                            || tarElement.ShapeType == "inclusiveGateway" || tarElement.ShapeType == "exclusiveGateway" || tarElement.ShapeType == "parallelGateway" || tarElement.ShapeType == "eventBasedGateway") {
                            alert("The gateway must not have any incoming or outgoing message");
                            scope.objData.lstShapes.splice(i, 1);
                            i--;
                            isValidEdge = false;
                        }
                    }
                }
            }
            if (isValidEdge) {
                lstSourcePoints.push(scope.objData.lstShapes[i]);
            }
        }
    }

    return lstSourcePoints;
}

function DisplayMessage(srcElement, tarElement) {
    if (srcElement.ShapeType == "startEvent" || tarElement.ShapeType == "startEvent") {
        if (srcElement.ShapeType == "startEvent") {
            alert("The Start Event must not have any outgoing messageflows");
        } else {
            alert("The Start Event must not have any incoming messageflows");
        }
    } else {
        if (srcElement.ShapeType == "endEvent") {
            alert("The End Event must not have any outgoing messageflows");
        } else {
            alert("The End Event must not have any incoming messageflows");
        }
    }
}

function GetElementProcessIdorElementBasedOnIdorDeleteElement(ID, scope, param) {
    var Data = "";
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].Id == ID) {
            if (param == "processId") {
                Data = scope.objData.lstShapes[i].processRef;
            }
            else if (param == "Delete") {
                Data = scope.objData.lstShapes[i];
                scope.objData.lstShapes.splice(i, 1);
            }
            else {
                Data = scope.objData.lstShapes[i];
            }
            break;
        }
    }
    return Data;
}

function generateSelectionRectangleDynamically(selectedElement, scope) {
    //var scope = getCurrentFileScope();
    var x = parseFloat(selectedElement.getAttribute("x"));
    var y = parseFloat(selectedElement.getAttribute("y"));
    var height = parseFloat(selectedElement.getAttribute("height"));
    var width = parseFloat(selectedElement.getAttribute("width"));
    //selection rect
    var rectangle = document.createElementNS(scope.svgNS, "rect");
    rectangle.setAttributeNS(null, "id", "SelectionElement");
    rectangle.setAttributeNS(null, 'rx', 5);
    rectangle.setAttributeNS(null, 'ry', 5);
    if (scope.selectedElement != 0 && scope.selectedElement.getAttribute("id") != "message") {
        rectangle.setAttribute("onmouseout", 'removehoverCircle(evt,this)');
    }
    rectangle.setAttributeNS(null, 'x', x - 15);
    rectangle.setAttributeNS(null, 'y', y - 15);
    rectangle.setAttributeNS(null, 'height', height + 30);
    rectangle.setAttributeNS(null, 'width', width + 30);
    var fillcolor = "url(#selectionRect_" + scope.$root.currentopenfile.file.FileName + ")";
    rectangle.setAttributeNS(null, "fill", fillcolor);
    rectangle.setAttribute("stroke-width", 0.5);
    rectangle.setAttributeNS(null, "stroke", "black");
    selectedElement.insertBefore(rectangle, selectedElement.childNodes[0]);
}

function UpdateEdgeBound1(aisOutEdge, obj, xDiff, yDiff, objSourceElement, objTargetElement, scope) {
    var lst = obj.lstWayPoints;
    //var scope = getCurrentFileScope();
    if (scope != undefined) {
        if (lst.length > 1) {
            var sourcePoint = {
                Left: lst[0].Left, Top: lst[0].Top
            };
            var targetPoint = {
                Left: lst[lst.length - 1].Left, Top: lst[lst.length - 1].Top
            };
            if (!aisOutEdge) {
                sourcePoint.Left += xDiff;
                sourcePoint.Top += yDiff;
            }
            else {
                targetPoint.Left += xDiff;
                targetPoint.Top += yDiff;
            }

            var newWayPoints = GetEdgeWayPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint);

            while (newWayPoints.length == 2 && sourcePoint.Left != targetPoint.Left && sourcePoint.Top != targetPoint.Top) {
                if (!aisOutEdge) {
                    newWayPoints = GetEdgeWayPointsForDottedLine(objSourceElement, objTargetElement);
                }
                else {
                    newWayPoints = GetEdgeWayPointsForDottedLine(objSourceElement, objTargetElement);
                }
            }

            if (objSourceElement.ShapeType == "textAnnotation" || objTargetElement.ShapeType == "textAnnotation") {
                var length = newWayPoints.length;
                if (newWayPoints.length > 2) {
                    var numberofElementsToRemove = length - 2;
                    newWayPoints.splice(1, numberofElementsToRemove);
                }
            }
            newWayPoints = setShapeNameToWayPoints(newWayPoints);
            // updating object 
            for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                if (scope.objData.lstShapes[i].Id == obj.Id) {
                    scope.objData.lstShapes[i].lstWayPoints = newWayPoints;
                    break;
                }
            }
            //ReDrawingEdges(scope);
        }
    }
}

function ReDrawingEdges(scope) {
    // Removing Old Lines
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].id == "line") {
            scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
            i--;
        }
    }

    // redrawing
    for (var k = 0; k < scope.objData.lstShapes.length; k++) {
        if (scope.objData.lstShapes[k].ShapeType == "sequenceFlow" || scope.objData.lstShapes[k].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[k].ShapeType == "association" || scope.objData.lstShapes[k].ShapeType == "messageFlow" || scope.objData.lstShapes[k].ShapeType == "dataInputAssociation") {
            AddLineArray(scope.svgElement, scope.objData.lstShapes[k], scope);
        }
    }
}

function GetEdgeWayPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint) {
    PointCorrection(objSourceElement, sourcePoint);
    PointCorrection(objTargetElement, targetPoint);

    return GetEdgeWayNewPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint);
}

function PointCorrection(recShape, point) {
    if (IsSame(point.Left, recShape.Left)) {
        point.Left = recShape.Left;
    }
    else if (IsSame(point.Left, recShape.Left + recShape.Width)) {
        point.Left = recShape.Left + recShape.Width;
    }
    if (IsSame(point.Top, recShape.Top)) {
        point.Top = recShape.Top;
    }
    else if (IsSame(point.Top, recShape.Top + recShape.Height)) {
        point.Top = recShape.Top + recShape.Height;
    }
}

function IsSame(source, target) {
    var retVal = false;
    retVal = Math.abs(source - target) < 2;
    return retVal;
}

function GetEdgeWayNewPoints(arecSource, arecDest, sourcePoint, targetPoint) {
    var polyLinePointArray = [];

    if (arecSource != null && arecDest != null) {

        var sourceTop = arecSource.Top;
        var sourceLeft = arecSource.Left;
        var sourceWidth = arecSource.Width;
        var sourceHeight = arecSource.Height;

        var sourceMidY = sourceTop + sourceHeight / 2;
        var sourceMidX = sourceLeft + sourceWidth / 2;


        var destTop = arecDest.Top;
        var destLeft = arecDest.Left;
        var destWidth = arecDest.Width;
        var destHeight = arecDest.Height;

        polyLinePointArray.push(sourcePoint);

        if (targetPoint.Left > sourcePoint.Left) //target is greater than source (horizontally)
        {
            if (targetPoint.Top == destTop) // target point y value is equal to destination y val(top)
            {
                CreateWaypointsPointsForTargetYvalEqualtoTop(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Left == (destLeft + destWidth))// target point x  is equal to destination width
            {
                CreateWaypointsForTargetXvalEqualtoWidth(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Top == (destTop + destHeight))// target point y  is equal to destination height
            {
                CreateWaypointsPointsForTargetYvalEqualtoHeight(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Left == destLeft)// target point x  is equal to destination left
            {
                CreateWaypointsForTargetXvalEqualtoLeft(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }

        }

        else if (sourcePoint.Left > targetPoint.Left) //source is greater than target (horizontally)
        {
            if (targetPoint.Top == destTop) // target point y value is equal to destination y val(top)
            {
                CreateWaypointsPointsForTargetYvalEqualtoTop1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Left == (destLeft + destWidth))// target point x  is equal to destination width
            {
                CreateWaypointsForTargetXvalEqualtoWidth1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Top == (destTop + destHeight))// target point y  is equal to destination height
            {
                CreateWaypointsPointsForTargetYvalEqualtoHeight1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
            else if (targetPoint.Left == destLeft)// target point x  is equal to destination left
            {
                CreateWaypointsForTargetXvalEqualtoLeft1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth);
            }
        }

        polyLinePointArray.push(targetPoint);

    }

    return polyLinePointArray;
}

function CreateWaypointsPointsForTargetYvalEqualtoTop(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                var point = {
                    Left: sourcePoint.Left, Top: sourcePoint.Top - 20
                };
                polyLinePointArray.push(point);

                if (destLeft > sourceLeft + sourceWidth) //if destLeft is greater than source left+width
                {
                    var point = {
                        Left: targetPoint.Left, Top: sourcePoint.Top - 20
                    };
                    polyLinePointArray.push(point);
                }
                else //if destLeft is less than source left+width
                {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                }

            }
            else // if source point y val is greater than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) //if destLeft is greater than source left+width
                {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else //if destLeft is less than source left+width
                {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });

                }
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
            }
            else // if source point y val is greater than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });

                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (destTop > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop - 20 });
                }

            }
            else //if source point y val is greater than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    var nextPointX = destLeft - 20;
                    if (nextPointX > sourceLeft) {
                        nextPointX = sourceLeft - 20;
                    }

                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top });
            }
            else //if source point y val is greater than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top - 20 });
                }

                polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
            }
        }
    }
}

function CreateWaypointsForTargetXvalEqualtoWidth(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destTop > sourceTop + sourceHeight - 20) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                    var nextPointX = targetPoint.Left + 20;
                    if (nextPointX < sourceLeft + sourceWidth) {
                        nextPointX = sourceLeft + sourceWidth + 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
            }
            else // if source point y val is greater than target point y val
            {
                if (sourceTop > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }

            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destTop > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top });
                }

            }
            else // if source point y val is greater than target point y val
            {
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (destTop > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }

            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (sourceTop > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }

            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (sourcePoint.Top < destTop) {
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
            }
            else //if source point y val is greater than target point y val
            {
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left + 20, Top: targetPoint.Top });
                }
            }
        }
    }
}

function CreateWaypointsPointsForTargetYvalEqualtoHeight(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    var nextPointX = sourceLeft - 20;
                    if (nextPointX > destLeft) {
                        nextPointX = destLeft - 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                if (targetPoint.Left > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                    var nextPointY = targetPoint.Top + 20;
                    if (nextPointY <= sourceTop + sourceHeight) {
                        nextPointY = sourceTop + sourceHeight + 20;
                    }
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: nextPointY });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: nextPointY });
                }
                else {
                    var nextPointX = destLeft - 20;
                    if (nextPointX > sourceLeft) {
                        nextPointX = sourceLeft - 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }

            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (sourceTop > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + 20 });
                }
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top });
            }
        }
    }
}

function CreateWaypointsForTargetXvalEqualtoLeft(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });
                }
                else {
                    var nextPointX = sourceLeft - 20;
                    if (nextPointX > destLeft) {
                        nextPointX = destLeft - 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top });
                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top });
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top });

            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                if (destLeft > sourceLeft + sourceWidth) {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: targetPoint.Top });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (targetPoint.Top > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });

                }
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourcePoint.Top });
                if (targetPoint.Top < sourceTop) {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left - 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });
                }
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourcePoint.Top });
                polyLinePointArray.push({ Left: sourceLeft + sourceWidth + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: targetPoint.Top });
            }
        }
    }
}

function CreateWaypointsPointsForTargetYvalEqualtoTop1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                if (targetPoint.Left < sourceLeft) {
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                }
            }
            else // if source point y val is greater than target point y val
            {
                if (sourcePoint.Left > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                if (destLeft + destWidth < sourceLeft) {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft - 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top });
            }
            else //if source point y val is greater than target point y val
            {
                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft - 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });

                }

            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (targetPoint.Top > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop - 20 });
                }
            }
            else //if source point y val is greater than target point y val
            {
                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
                else {
                    var nextPointX = sourceLeft + sourceWidth + 20;
                    if (nextPointX < destLeft + destWidth) {
                        nextPointX = destLeft + destWidth + 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top - 20 });
                }
            }
        }
    }
}

function CreateWaypointsForTargetXvalEqualtoWidth1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });

                if (destLeft + destWidth < sourceLeft) {
                    var nextPointX = destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2);
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: targetPoint.Top });
                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top });
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top });

            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });

                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft + sourceWidth + 20, Top: targetPoint.Top });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top });

            }
            else //if source point y val is greater than target point y val
            {

                polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top });
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (targetPoint.Top > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top });
                }
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourceTop - 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top });
                }
            }
        }
    }
}

function CreateWaypointsPointsForTargetYvalEqualtoHeight1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    var nextPointX = sourceLeft + sourceWidth + 20;
                    if (nextPointX < destLeft + destWidth) {
                        nextPointX = destLeft + destWidth + 20;
                    }

                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Top, Top: targetPoint.Top + 20 });
                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2) });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });

                }
            }
            else // if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                if (sourceLeft > targetPoint.Left) {
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: sourceLeft - 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (sourceLeft > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft - 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft - 20, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: targetPoint.Left, Top: sourcePoint.Top });
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (sourcePoint.Left > destLeft + destWidth) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                    var nextPointY = targetPoint.Top + 20;
                    if (nextPointY < sourceTop + sourceHeight) {
                        nextPointY = sourceTop + sourceHeight + 20;
                    }

                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: nextPointY });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: nextPointY });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + 20, Top: targetPoint.Top + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: targetPoint.Top + 20 });
                }
            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (targetPoint.Top < sourceTop) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2) });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourceTop + sourceHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left, Top: sourceTop + sourceHeight + 20 });
                }
            }
        }
    }
}

function CreateWaypointsForTargetXvalEqualtoLeft1(sourcePoint, targetPoint, polyLinePointArray, sourceTop, sourceLeft, sourceWidth, sourceHeight, destTop, destLeft, destHeight, destWidth) {
    if (sourcePoint.Left > sourceLeft && sourcePoint.Left < sourceLeft + sourceWidth) // source point x val is greater than source shape left and less than source shape width
    {
        if (sourcePoint.Top == sourceTop) // source point y value is equql to source shape y val(top)
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destTop > sourcePoint.Top) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top - 20 });
                    var nextPointX = targetPoint.Left - 20;
                    if (nextPointX > sourceLeft) {
                        nextPointX = sourceLeft - 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top - 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
            else // if source point y val is greater than target point y val
            {
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: (destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: (destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
        }
        else if (sourcePoint.Top == sourceTop + sourceHeight) // if source point y val is equal to source shape height
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destTop > sourcePoint.Top) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: (sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: (sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }

            }
            else // if source point y val is greater than target point y val
            {
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: sourcePoint.Top + 20 });
                    var nextPointX = targetPoint.Left - 20;
                    if (nextPointX > sourceLeft) {
                        nextPointX = sourceLeft - 20;
                    }
                    polyLinePointArray.push({ Left: nextPointX, Top: sourcePoint.Top + 20 });
                    polyLinePointArray.push({ Left: nextPointX, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
        }
    }
    else //source point y val is greater than source shape y val and less than source shape height val
    {
        if (sourcePoint.Left == sourceLeft) //if source point x val is equal to source shape left
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                if (destTop > sourcePoint.Top) {
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
            else //if source point y val is greater than target point y val
            {
                if (sourcePoint.Top > destTop + destHeight) {
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourcePoint.Top });
                    polyLinePointArray.push({ Left: destLeft + destWidth + ((sourceLeft - (destLeft + destWidth)) / 2), Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
        }
        else if (sourcePoint.Left == sourceLeft + sourceWidth)//if source point x val is equal to source shape width
        {
            if (sourcePoint.Top < targetPoint.Top) //if source point y val is less than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (destTop > sourceTop + sourceHeight) {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: (sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: (sourceTop + sourceHeight + ((destTop - (sourceTop + sourceHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop + destHeight + 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }

            }
            else //if source point y val is greater than target point y val
            {
                polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: sourcePoint.Top });
                if (sourceTop > destTop + destHeight) {

                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: (destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: (destTop + destHeight + ((sourceTop - (destTop + destHeight)) / 2)) });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
                else {
                    polyLinePointArray.push({ Left: sourcePoint.Left + 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: destTop - 20 });
                    polyLinePointArray.push({ Left: targetPoint.Left - 20, Top: targetPoint.Top });
                }
            }
        }
    }
}


function checkShapeAlreadyPresentinListorNot(lstSelectedShapes, selectedShape) {
    isPresent = false;
    for (var i = 0; i < lstSelectedShapes.length; i++) {
        if (lstSelectedShapes[i].Id == selectedShape.Id) {
            isPresent = true;
            lstSelectedShapes.splice(i, 1);
            break;
        }
    }
    return isPresent;
}

function moveElementUsingKeyBoard(scope, dx, dy) {
    if (scope.selectedElemForResize && ((parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx) > 0) && ((parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy) > 0)) {
        var isValid = false;
        if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType != "participant") {
            if (scope.SelectedElementLane) {
                var xpos = parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx;
                var ypos = parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy;
                var xposofLane = parseFloat(scope.SelectedElementLane.getAttribute("x"));
                var yposofLane = parseFloat(scope.SelectedElementLane.getAttribute("y"));
                var heightofLane = parseFloat(scope.SelectedElementLane.getAttribute("height"));
                var widthofLane = parseFloat(scope.SelectedElementLane.getAttribute("width"));

                if (xpos > (xposofLane + 40) && ypos > yposofLane && ypos < (yposofLane + heightofLane) && xpos < (xposofLane + widthofLane)) {
                    isValid = true;
                }
            }
        } else {
            isValid = true;
        }
        if (isValid) {
            scope.isDirty = true;
            scope.selectedElemForResize.setAttribute('x', parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx);
            scope.selectedElemForResize.setAttribute('y', parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy);
            var isTextOut = false;
            var ElementIndex = 0;
            var mainElement = "";
            for (var i = 0; i < $(scope.selectedElemForResize).children().length; i++) {
                var item = $(scope.selectedElemForResize).children()[i];
                if (item.getAttribute("id") == "MainElement") {
                    mainElement = item;
                }
                if (item.nodeName == "polygon") {
                    isTextOut = true;
                    var Left = parseFloat(scope.selectedElemForResize.getAttribute("x"));
                    var Top = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                    var Height = parseFloat(scope.selectedElemForResize.getAttribute("height"));
                    var Width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
                    var points;
                    var point1 = {
                        X: Left,
                        Y: Top + (Height / 2)
                    };
                    var point2 = {
                        X: Left + (Width / 2),
                        Y: Top
                    };
                    var point3 = {
                        X: Left + Width,
                        Y: Top + (Height / 2)
                    };
                    var point4 = {
                        X: Left + (Width / 2),
                        Y: Top + Height
                    };
                    points = point1.X + "," + point1.Y + "  " + point2.X + "," + point2.Y + "  " + point3.X + "," + point3.Y + "  " + point4.X + "," + point4.Y;
                    item.setAttribute("points", points);
                    item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                    item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                }

                else if (item.nodeName == "circle") {

                    item.setAttribute("cx", parseFloat(item.getAttribute("cx")) + dx);
                    item.setAttribute("cy", parseFloat(item.getAttribute("cy")) + dy);
                    isTextOut = true;
                }
                else if (item.nodeName != "text" && item.getAttribute("id") != "SelectionElement" && item.nodeName != "line" && item.nodeName != "g") {
                    item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                    item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                }
                else if (item.getAttribute("id") == "SelectionElement") {
                    item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                    item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);
                }
                else if (item.nodeName == "line") {
                    item.setAttribute('x1', parseFloat(item.getAttribute("x1")) + dx);
                    item.setAttribute('y1', parseFloat(item.getAttribute("y1")) + dy);
                    item.setAttribute('x2', parseFloat(item.getAttribute("x2")) + dx);
                    item.setAttribute('y2', parseFloat(item.getAttribute("y2")) + dy);
                }
                else if (item.nodeName == "g") {
                    //function nestedIterator(itm) {
                    for (j = 0; j < $(item).children().length; j++) {
                        var itm = $(item).children()[j];
                        if (itm.nodeName == "line") {
                            itm.setAttribute('x1', parseFloat(itm.getAttribute("x1")) + dx);
                            itm.setAttribute('y1', parseFloat(itm.getAttribute("y1")) + dy);
                            itm.setAttribute('x2', parseFloat(itm.getAttribute("x2")) + dx);
                            itm.setAttribute('y2', parseFloat(itm.getAttribute("y2")) + dy);
                        }
                    }
                    //}
                    // angular.forEach($(item).children(), nestedIterator);
                }
                if (item.nodeName == "text") {
                    if (scope.selectedElemForResize.id != "participant") {
                        item.setAttribute('x', parseFloat(item.getAttribute('x')) + dx);
                        item.setAttribute('y', parseFloat(item.getAttribute('y')) + dy);
                        //function nestedIterator(itm) {
                        for (var k = 0; k < $(item).children().length; k++) {
                            var itm = $(item).children()[k];
                            itm.setAttribute('x', parseFloat(itm.getAttribute('x')) + dx);
                        }
                        //angular.forEach($(item).children(), nestedIterator);
                    }
                    else {
                        item.setAttribute('x', parseFloat(item.getAttribute('x')) + dx);
                        item.setAttribute('y', parseFloat(item.getAttribute('y')) + dy);
                        //var tx = parseFloat(item.getAttribute('x')) + 5 + (scope.objData.lstShapes[scope.indexofResizerElement].LabelWidth / 2);
                        //var ty = parseFloat(item.getAttribute('y')) + (scope.objData.lstShapes[scope.indexofResizerElement].LabelHeight / 2);
                        var tx = parseFloat(item.getAttribute('x'));
                        var ty = parseFloat(item.getAttribute('y'));
                        if (mainElement) {
                            var height = parseFloat(mainElement.getAttribute('height'));
                            var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                            var textnode = wraptorect(item, mainElement, 0, 0.5, "Participant", scope);
                            var translate = 10 + ' ' + height / 2;
                            textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                            textnode.setAttribute('text-anchor', 'middle');
                        }
                    }
                }

            }

            if (scope.objData.lstShapes[scope.indexofResizerElement]) {
                scope.objData.lstShapes[scope.indexofResizerElement].Left = parseFloat(scope.selectedElemForResize.getAttribute("x"));
                scope.objData.lstShapes[scope.indexofResizerElement].Top = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                scope.objData.lstShapes[scope.indexofResizerElement].Height = parseFloat(scope.selectedElemForResize.getAttribute("height"));
                scope.objData.lstShapes[scope.indexofResizerElement].Width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
            }

            if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "participant") {
                var processReference = scope.objData.lstShapes[scope.indexofResizerElement].processRef;
                UdjustCoordinatesForChildElementsOfParticipant(dx, dy, processReference, scope);
                RedrawingElementsAndEdges(scope);
                for (var k = 0; k < scope.svgElement.childNodes.length; k++) {
                    if (scope.svgElement.childNodes[k].nodeName != "defs") {
                        if (scope.objData.lstShapes[scope.indexofResizerElement].Left == parseFloat(scope.svgElement.childNodes[k].getAttribute("x")) && scope.objData.lstShapes[scope.indexofResizerElement].Top == parseFloat(scope.svgElement.childNodes[k].getAttribute("y")) &&
                            scope.objData.lstShapes[scope.indexofResizerElement].Height == parseFloat(scope.svgElement.childNodes[k].getAttribute("height")) && scope.objData.lstShapes[scope.indexofResizerElement].Width == parseFloat(scope.svgElement.childNodes[k].getAttribute("width"))) {
                            generateSelectionRectangleDynamically(scope.svgElement.childNodes[k], scope);
                            scope.selectedElement = scope.svgElement.childNodes[k];
                            scope.selectedElemForResize = scope.selectedElement;
                            break;
                        }
                    }
                }
            } else {
                resetEdgePointsForSelectedShape(scope.objData.lstShapes[scope.indexofResizerElement], dx, dy, scope, undefined);
            }
            removehovercirclefromsvgElement(scope);
            removeResizerAroundElement(scope);
            GenerateBoundsDynamicallyToElement(scope.selectedElemForResize, false, scope);
        }
    }
}

