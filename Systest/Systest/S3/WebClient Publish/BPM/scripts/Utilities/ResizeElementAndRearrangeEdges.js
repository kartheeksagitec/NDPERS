// generate reizeEdges

function GenerateBoundsDynamicallyToElement(objselectedElement, isCloned, scope) {
    // resizer 
    var x = 0;
    var y = 0;
    var Height = 0;
    var Width = 0;
    x = parseFloat(objselectedElement.getAttribute("x")) - 5;
    y = parseFloat(objselectedElement.getAttribute("y")) - 5;
    Height = parseFloat(objselectedElement.getAttribute("height")) + 10;
    Width = parseFloat(objselectedElement.getAttribute("width")) + 10;
    //var scope = getCurrentFileScope();
    //removeResizerAroundElement();
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, "id", "resizergroup");
    g.setAttributeNS(null, "class", "layer-resizers");

    if (isCloned) {
        g.setAttribute("style", "opacity: 0.5;");
    }

    if (objselectedElement.getAttribute("id") != "message" && objselectedElement.getAttribute('id') != "endEvent" && objselectedElement.getAttribute('id') != "startEvent" && objselectedElement.getAttribute('id') != "intermediateCatchEvent"
        && objselectedElement.getAttribute('id') != "exclusiveGateway" && objselectedElement.getAttribute('id') != "inclusiveGateway" && objselectedElement.getAttribute('id') != "parallelGateway" && objselectedElement.getAttribute('id') != "eventBasedGateway" && objselectedElement.getAttribute('id') != "dataStoreReference" && objselectedElement.getAttribute('id') != "dataObjectReference") {

        //var gchildresizeborder = document.createElementNS(scope.svgNS, "g");
        //gchildresizeborder.setAttributeNS(null, "id", "resizerborder");

        //var Resizerpoints = [];
        //var Point1 = { Left: x, Top: y };
        //Resizerpoints.push(Point1);
        //var Point2 = { Left: x + Width, Top: y };
        //Resizerpoints.push(Point2);
        //var Point3 = { Left: x + Width, Top: y + Height };
        //Resizerpoints.push(Point3);
        //var Point4 = { Left: x, Top: y + Height };
        //Resizerpoints.push(Point4);
        //Resizerpoints.push(Point1);

        //for (var i = 0; i < Resizerpoints.length; i++) {
        //    if (i < Resizerpoints.length - 1) {
        //        var line = document.createElementNS(scope.svgNS, "line");
        //        line.setAttribute('id', "resizerline");
        //        line.setAttribute("x1", Resizerpoints[i].Left);
        //        line.setAttribute("y1", Resizerpoints[i].Top);
        //        line.setAttribute("x2", Resizerpoints[i + 1].Left);
        //        line.setAttribute("y2", Resizerpoints[i + 1].Top);
        //        line.setAttribute("stroke-width", 1);
        //        line.setAttributeNS(null, "stroke", "#4f4231");
        //        gchildresizeborder.appendChild(line);
        //    }
        //}
        //g.appendChild(gchildresizeborder);
        if (objselectedElement.getAttribute("id") != "lane") {
            var gchildnw = document.createElementNS(scope.svgNS, "g");
            gchildnw.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-nw");
            gchildnw.setAttributeNS(null, "id", "nw");
            gchildnw.setAttribute("onmousedown", 'selectedResizergtagElement(evt)');

            var rectanglenw = document.createElementNS(scope.svgNS, "rect");
            rectanglenw.setAttributeNS(null, "id", "rectanglenw");
            rectanglenw.setAttributeNS(null, "class", "djs-resizer-visual");
            rectanglenw.setAttributeNS(null, "x", x - 3);
            rectanglenw.setAttributeNS(null, "y", y - 3);
            rectanglenw.setAttributeNS(null, "height", 6);
            rectanglenw.setAttributeNS(null, "width", 6);
            rectanglenw.setAttributeNS(null, "fill", "white");
            rectanglenw.setAttributeNS(null, "stroke", "none");
            gchildnw.appendChild(rectanglenw);

            var rectangle2ndnw = document.createElementNS(scope.svgNS, "rect");
            rectangle2ndnw.setAttributeNS(null, "id", "rectanglenw");
            rectangle2ndnw.setAttributeNS(null, "class", "djs-resizer-hit");
            rectangle2ndnw.setAttributeNS(null, "x", x - 10);
            rectangle2ndnw.setAttributeNS(null, "y", y - 10);
            rectangle2ndnw.setAttributeNS(null, "height", 20);
            rectangle2ndnw.setAttributeNS(null, "width", 20);
            rectangle2ndnw.setAttributeNS(null, "fill", "white");
            rectangle2ndnw.setAttributeNS(null, "stroke", "none");
            gchildnw.appendChild(rectangle2ndnw);
            g.appendChild(gchildnw);


            var gchildne = document.createElementNS(scope.svgNS, "g");
            gchildne.setAttribute("onmousedown", 'selectedResizergtagElement(evt)');
            gchildne.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-ne");
            gchildne.setAttributeNS(null, "id", "ne");
            var rectanglene = document.createElementNS(scope.svgNS, "rect");
            rectanglene.setAttributeNS(null, "id", "rectanglene");
            rectanglene.setAttributeNS(null, "class", "djs-resizer-visual");
            rectanglene.setAttributeNS(null, "x", (x + Width - 3));
            rectanglene.setAttributeNS(null, "y", y - 3);
            rectanglene.setAttributeNS(null, "height", 6);
            rectanglene.setAttributeNS(null, "width", 6);
            rectanglene.setAttributeNS(null, "fill", "white");
            rectanglene.setAttributeNS(null, "stroke", "none");
            gchildne.appendChild(rectanglene);

            var rectangle2ndne = document.createElementNS(scope.svgNS, "rect");
            rectangle2ndne.setAttributeNS(null, "id", "rectanglene");
            rectangle2ndne.setAttributeNS(null, "class", "djs-resizer-hit");
            rectangle2ndne.setAttributeNS(null, "x", (x + Width - 10));
            rectangle2ndne.setAttributeNS(null, "y", y - 10);
            rectangle2ndne.setAttributeNS(null, "height", 20);
            rectangle2ndne.setAttributeNS(null, "width", 20);
            rectangle2ndne.setAttributeNS(null, "fill", "white");
            rectangle2ndne.setAttributeNS(null, "stroke", "none");
            gchildne.appendChild(rectangle2ndne);
            g.appendChild(gchildne);
        }

        var gchildse = document.createElementNS(scope.svgNS, "g");

        gchildse.setAttribute("onmousedown", 'selectedResizergtagElement(evt)');
        if (objselectedElement.getAttribute("id") != "lane") {
            gchildse.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-se");
            gchildse.setAttributeNS(null, "id", "se");
        } else {
            gchildse.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-s");
            gchildse.setAttributeNS(null, "id", "nse");
        }
        var rectanglese = document.createElementNS(scope.svgNS, "rect");
        rectanglese.setAttributeNS(null, "id", "rectanglese");
        rectanglese.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglese.setAttributeNS(null, "x", (x + Width - 3));
        rectanglese.setAttributeNS(null, "y", (y + Height - 3));
        rectanglese.setAttributeNS(null, "height", 6);
        rectanglese.setAttributeNS(null, "width", 6);
        rectanglese.setAttributeNS(null, "fill", "orange");
        rectanglese.setAttributeNS(null, "stroke", "none");
        gchildse.appendChild(rectanglese);

        var rectangle2ndse = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndse.setAttributeNS(null, "id", "rectanglese");
        rectangle2ndse.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndse.setAttributeNS(null, "x", (x + Width - 10));
        rectangle2ndse.setAttributeNS(null, "y", (y + Height - 10));
        rectangle2ndse.setAttributeNS(null, "height", 20);
        rectangle2ndse.setAttributeNS(null, "width", 20);
        rectangle2ndse.setAttributeNS(null, "fill", "orange");
        rectangle2ndse.setAttributeNS(null, "stroke", "none");
        gchildse.appendChild(rectangle2ndse);
        g.appendChild(gchildse);

        var gchildsw = document.createElementNS(scope.svgNS, "g");

        gchildsw.setAttribute("onmousedown", 'selectedResizergtagElement(evt)');
        if (objselectedElement.getAttribute("id") != "lane") {
            gchildsw.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-sw");
            gchildsw.setAttributeNS(null, "id", "sw");
        } else {
            gchildsw.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-s");
            gchildsw.setAttributeNS(null, "id", "nsw");
        }
        var rectanglesw = document.createElementNS(scope.svgNS, "rect");
        rectanglesw.setAttributeNS(null, "id", "rectanglesw");
        rectanglesw.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglesw.setAttributeNS(null, "x", x - 3);
        rectanglesw.setAttributeNS(null, "y", (y + Height - 3));
        rectanglesw.setAttributeNS(null, "height", 6);
        rectanglesw.setAttributeNS(null, "width", 6);
        rectanglesw.setAttributeNS(null, "fill", "orange");
        rectanglesw.setAttributeNS(null, "stroke", "none");
        gchildsw.appendChild(rectanglesw);

        var rectangle2ndsw = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndsw.setAttributeNS(null, "id", "rectanglesw");
        rectangle2ndsw.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndsw.setAttributeNS(null, "x", x - 10);
        rectangle2ndsw.setAttributeNS(null, "y", (y + Height - 10));
        rectangle2ndsw.setAttributeNS(null, "height", 20);
        rectangle2ndsw.setAttributeNS(null, "width", 20);
        rectangle2ndsw.setAttributeNS(null, "fill", "orange");
        rectangle2ndsw.setAttributeNS(null, "stroke", "none");
        gchildsw.appendChild(rectangle2ndsw);
        g.appendChild(gchildsw);
    }
    if (!isCloned) {
        if (objselectedElement.getAttribute("id") != "lane" && objselectedElement.getAttribute("id") != "participant" && objselectedElement.getAttribute("id") != "textAnnotation" && objselectedElement.getAttribute("id") != "message") {
            GenerateRightPanelForAddingElements(g, objselectedElement, scope);
        }
    }

    scope.svgElement.appendChild(g);

    return g;
}

function GenerateRightPanelForAddingElements(g, objselectedElement, scope) {

    for (var i = 0; i < g.childNodes.length; i++) {
        if (g.childNodes[i].id == "RightPanel") {
            g.removeChild(g.childNodes[i]);
            break;
        }
    }
    var x = 0;
    var y = 0;
    var Height = 0;
    var Width = 0;

    x = parseFloat(objselectedElement.getAttribute("x"));
    y = parseFloat(objselectedElement.getAttribute("y"));
    Height = parseFloat(objselectedElement.getAttribute("height"));
    Width = parseFloat(objselectedElement.getAttribute("width"));

    var grightpanel = document.createElementNS(scope.svgNS, "g");
    grightpanel.setAttributeNS(null, "id", "RightPanel");

    /// Right Panel
    var rectangleright = document.createElementNS(scope.svgNS, "rect");
    rectangleright.setAttributeNS(null, "id", "rectRight");
    rectangleright.setAttributeNS(null, "x", x + Width + 10);
    rectangleright.setAttributeNS(null, "y", y);
    rectangleright.setAttributeNS(null, "height", 70);
    rectangleright.setAttributeNS(null, "width", 20);
    rectangleright.setAttributeNS(null, "fill", "#908282");
    rectangleright.setAttributeNS(null, "stroke", "#4f4231");
    grightpanel.appendChild(rectangleright);

    //user task
    var task = document.createElementNS(scope.svgNS, "rect");
    task.setAttributeNS(null, "id", "task");
    task.setAttribute("rx", 3);
    task.setAttribute("ry", 3);
    task.setAttributeNS(null, "x", x + Width + 10 + 2.5);
    task.setAttribute("onmousedown", "drawNewTask(evt)");
    task.setAttributeNS(null, "y", y + 5);
    task.setAttributeNS(null, "height", 10);
    task.setAttributeNS(null, "width", 15);
    task.setAttributeNS(null, "fill", "#f7ce98");
    task.setAttributeNS(null, "stroke", "#4f4231");
    grightpanel.appendChild(task);

    // Exclusive gate
    var ExclusivGate = document.createElementNS(scope.svgNS, "polygon");
    var xpos = parseFloat(grightpanel.childNodes[grightpanel.childNodes.length - 1].getAttribute("x"));
    var ypos = parseFloat(grightpanel.childNodes[grightpanel.childNodes.length - 1].getAttribute("y")) + parseFloat(grightpanel.childNodes[grightpanel.childNodes.length - 1].getAttribute("height")) + 6;
    var point1 = {
        X: xpos,
        Y: ypos + (15 / 2)
    };
    var point2 = {
        X: xpos + (15 / 2),
        Y: ypos
    };
    var point3 = {
        X: xpos + 15,
        Y: ypos + (15 / 2)
    };
    var point4 = {
        X: xpos + (15 / 2),
        Y: ypos + 15
    };
    var points = point1.X + "," + point1.Y + "  " + point2.X + "," + point2.Y + "  " + point3.X + "," + point3.Y + "  " + point4.X + "," + point4.Y;
    ExclusivGate.setAttribute("id", "ExclusivGate");
    ExclusivGate.setAttribute("points", points);
    ExclusivGate.setAttribute("onmousedown", "drawExclusiveGate(evt)");
    ExclusivGate.setAttribute("stroke", "#4f4231");
    ExclusivGate.setAttribute('fill', "#f7ede1");
    grightpanel.appendChild(ExclusivGate);

    // cache
    var myCircle = document.createElementNS(scope.svgNS, "circle");
    myCircle.setAttributeNS(null, "id", "cacheouter");
    myCircle.setAttributeNS(null, "cx", xpos + 7.5);
    myCircle.setAttributeNS(null, "cy", ypos + 20 + 5);
    myCircle.setAttribute("onmousedown", "drawintermediateCatchEvent(evt)");
    myCircle.setAttributeNS(null, "r", 6);
    myCircle.setAttributeNS(null, "fill", "#f8cd98");
    myCircle.setAttributeNS(null, "stroke", "#4f4231");
    grightpanel.appendChild(myCircle);

    var myCircle = document.createElementNS(scope.svgNS, "circle");
    myCircle.setAttributeNS(null, "id", "cacheinner");
    myCircle.setAttributeNS(null, "cx", xpos + 7.5);
    myCircle.setAttribute("onmousedown", "drawintermediateCatchEvent(evt)");
    myCircle.setAttributeNS(null, "cy", ypos + 20 + 5);
    myCircle.setAttributeNS(null, "r", 4);
    myCircle.setAttributeNS(null, "fill", "#f8cd98");
    myCircle.setAttributeNS(null, "stroke", "#4f4231");
    grightpanel.appendChild(myCircle);


    // Annotation
    var textAnnotation = document.createElementNS(scope.svgNS, "rect");
    textAnnotation.setAttributeNS(null, "id", "textAnnotation");
    //rectangleright.setAttributeNS(null, "class", "djs-resizer-hit");
    textAnnotation.setAttributeNS(null, "x", xpos);
    textAnnotation.setAttributeNS(null, "y", ypos + 30 + 5);
    textAnnotation.setAttributeNS(null, "height", 10);
    textAnnotation.setAttributeNS(null, "width", 15);
    textAnnotation.setAttribute("onmousedown", "drawtextAnnotationFromPanel(evt)");
    textAnnotation.setAttributeNS(null, "fill", "#f7ede1");
    textAnnotation.setAttributeNS(null, "stroke", "none");
    grightpanel.appendChild(textAnnotation);

    var xpos = parseFloat(grightpanel.childNodes[grightpanel.childNodes.length - 1].getAttribute("x"));
    var ypos = parseFloat(grightpanel.childNodes[grightpanel.childNodes.length - 1].getAttribute("y"));
    var textAnnotationLinePoints = [];
    var point1 = { Left: xpos + (15 / 2), Top: ypos };
    textAnnotationLinePoints.push(point1);
    var point2 = { Left: xpos, Top: ypos };
    textAnnotationLinePoints.push(point2);
    var point3 = { Left: xpos, Top: ypos + 10 };
    textAnnotationLinePoints.push(point3);
    var point4 = { Left: xpos + (15 / 2), Top: ypos + 10 };
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
            textAnnotationLine.setAttributeNS(null, "stroke", "#000000");
            grightpanel.appendChild(textAnnotationLine);
        }
    }
    g.appendChild(grightpanel);
    //scope.svgElement.appendChild(g);
}


function removeResizerAroundElement(scope) {
    //var scope = getCurrentFileScope();
    for (var i = scope.svgElement.childNodes.length - 1; i > 0; i--) {
        if (scope.svgElement.childNodes[i].nodeName != "#text" && scope.svgElement.childNodes[i].nodeName != "#comment") {
            var ID = scope.svgElement.childNodes[i].getAttribute("id");
            if (ID == "resizergroup") {
                scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                i--;
            }
        }
    }
}

// drag Resizer For Element
function selectedResizergtagElement(evnt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    if (scope) {
        scope.selectedResizergElement = evnt.currentTarget;
        scope.currentX = evnt.offsetX;
        scope.currentY = evnt.offsetY;
        scope.isMouseDown = true;
        getMaxResizeLimitsOfParticipantorLane(scope);
        scope.svgElement.setAttributeNS(null, "onmouseup", "deselectResizergElement(evt)");
        scope.svgElement.setAttributeNS(null, "onmousemove", "moveResizergElement(evt)");
    }
}

function deselectResizergElement(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    scope.isMouseDown = false;
    if (scope.selectedResizergElement != 0) {
        if (scope.selectedElemForResize.id == "participant") {
            AdjustLanePositions(scope.selectedResizergElement, scope.selectedElemForResize, scope);
        }

        if (scope.selectedElemForResize.id == "lane") {
            var diffY = parseFloat(scope.selectedElemForResize.getAttribute("height")) - scope.objData.lstShapes[scope.indexofResizerElement].Height;
            AjustLanePositionAndElements(diffY, scope);
        }

        angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
            if (itm.nodeName == "rect") {
                scope.objData.lstShapes[scope.indexofResizerElement].Left = parseFloat(scope.selectedElemForResize.getAttribute("x"));
                scope.objData.lstShapes[scope.indexofResizerElement].Top = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                scope.objData.lstShapes[scope.indexofResizerElement].Height = parseFloat(scope.selectedElemForResize.getAttribute("height"));
                scope.objData.lstShapes[scope.indexofResizerElement].Width = parseFloat(scope.selectedElemForResize.getAttribute("width"));
            }
            else if (itm.nodeName == "text") {
                scope.objData.lstShapes[scope.indexofResizerElement].LabelLeft = parseFloat(itm.getAttribute("x"));
                scope.objData.lstShapes[scope.indexofResizerElement].LabelTop = parseFloat(itm.getAttribute("y"));
                //scope.objData.lstShapes[indexofResizerElement].LabelHeight = parseFloat(itm.getAttribute("height"));
                //scope.objData.lstShapes[indexofResizerElement].LabelWidth = parseFloat(itm.getAttribute("width"));
            }
        });

        ReArrangeEdges(scope.objData.lstShapes[scope.indexofResizerElement], scope);

        if (scope.selectedElemForResize.id == "participant") {
            RedrawingElementsAndEdges(scope);
            for (var k = 0; k < scope.svgElement.childNodes.length; k++) {
                if (scope.svgElement.childNodes[k].nodeName != "defs") {
                    if (scope.objData.lstShapes[scope.indexofResizerElement].Left == parseFloat(scope.svgElement.childNodes[k].getAttribute("x")) && scope.objData.lstShapes[scope.indexofResizerElement].Top == parseFloat(scope.svgElement.childNodes[k].getAttribute("y")) &&
                        scope.objData.lstShapes[scope.indexofResizerElement].Height == parseFloat(scope.svgElement.childNodes[k].getAttribute("height")) && scope.objData.lstShapes[scope.indexofResizerElement].Width == parseFloat(scope.svgElement.childNodes[k].getAttribute("width"))) {
                        generateSelectionRectangleDynamically(scope.svgElement.childNodes[k], scope);
                        scope.selectedElement = scope.svgElement.childNodes[k];
                        scope.selectedElemForResize = scope.selectedElement;
                        GenerateBoundsDynamicallyToElement(scope.selectedElement, false, scope);
                        break;
                    }
                }
            }
        }

        if (scope.selectedElemForResize.id == "lane") {
            RedrawingElementsAndEdges(scope);
        }
        scope.svgElement.removeAttributeNS(null, "onmousemove");
        scope.svgElement.removeAttributeNS(null, "onmouseup");
        scope.selectedResizergElement = 0;
    }
}

function AdjustLanePositions(selectedResizergElement, selectedElemForResize, scope) {
    var Left = parseFloat(selectedElemForResize.getAttribute("x")) - scope.objData.lstShapes[scope.indexofResizerElement].Left;
    var Top = parseFloat(selectedElemForResize.getAttribute("y")) - scope.objData.lstShapes[scope.indexofResizerElement].Top;
    var Height = parseFloat(selectedElemForResize.getAttribute("height")) - scope.objData.lstShapes[scope.indexofResizerElement].Height;
    var Width = parseFloat(selectedElemForResize.getAttribute("width")) - scope.objData.lstShapes[scope.indexofResizerElement].Width;

    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeType == "lane" && (((scope.objData.lstShapes[i].Top - scope.objData.lstShapes[scope.indexofResizerElement].Top) > -0.1 && (scope.objData.lstShapes[i].Top - scope.objData.lstShapes[scope.indexofResizerElement].Top) < 0.1 && (selectedResizergElement.getAttribute("id") == "nw" || selectedResizergElement.getAttribute("id") == "ne"))
            || (((scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) - (scope.objData.lstShapes[scope.indexofResizerElement].Top + scope.objData.lstShapes[scope.indexofResizerElement].Height)) > -0.1 && ((scope.objData.lstShapes[i].Top + scope.objData.lstShapes[i].Height) - (scope.objData.lstShapes[scope.indexofResizerElement].Top + scope.objData.lstShapes[scope.indexofResizerElement].Height)) < 0.1)
            && (selectedResizergElement.getAttribute("id") == "sw" || selectedResizergElement.getAttribute("id") == "se") && (scope.objData.lstShapes[scope.indexofResizerElement].processRef == scope.objData.lstShapes[i].processRef))) {
            if (selectedResizergElement.getAttribute("id") == "nw" || selectedResizergElement.getAttribute("id") == "ne") {
                scope.objData.lstShapes[i].Left = scope.objData.lstShapes[i].Left + Left;
                scope.objData.lstShapes[i].Top = scope.objData.lstShapes[i].Top + Top;
                scope.objData.lstShapes[i].Width = scope.objData.lstShapes[i].Width + Width;
                scope.objData.lstShapes[i].Height = scope.objData.lstShapes[i].Height + Height;
            }
            else if (selectedResizergElement.getAttribute("id") == "sw" || selectedResizergElement.getAttribute("id") == "se") {
                scope.objData.lstShapes[i].Width = scope.objData.lstShapes[i].Width + Width;
                scope.objData.lstShapes[i].Height = scope.objData.lstShapes[i].Height + Height;
                scope.objData.lstShapes[i].Left = scope.objData.lstShapes[i].Left + Left;
            }
        }
        else if (scope.objData.lstShapes[i].ShapeType == "lane" && scope.objData.lstShapes[scope.indexofResizerElement].processRef == scope.objData.lstShapes[i].processRef) {
            if (selectedResizergElement.getAttribute("id") == "nw" || selectedResizergElement.getAttribute("id") == "sw") {
                scope.objData.lstShapes[i].Width = scope.objData.lstShapes[i].Width + Width;
                scope.objData.lstShapes[i].Left = scope.objData.lstShapes[i].Left + Left;
            }
            if (selectedResizergElement.getAttribute("id") == "ne" || selectedResizergElement.getAttribute("id") == "se") {
                scope.objData.lstShapes[i].Width = scope.objData.lstShapes[i].Width + Width;
            }
        }
    }
}

function AjustLanePositionAndElements(diffY, scope) {

    var lane = scope.objData.lstShapes[scope.indexofResizerElement];
    var processId = lane.processRef;
    var ElementID = lane.Id;
    var objlaneCountandPoolIndex = scope.getLaneCountinPool(processId);

    var objPool = scope.objData.lstShapes[objlaneCountandPoolIndex.PoolIndex];
    if (objPool) {
        objPool.Height = objPool.Height + diffY;
    }

    ResetEdgePoints(objPool, -diffY, scope);
    // change position of the lane  and its elemnets present inside it
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeName != "BPMNEdge") {
            if (scope.objData.lstShapes[i].processRef == processId && scope.objData.lstShapes[i].ShapeType == 'lane' && scope.objData.lstShapes[i].Id != ElementID) {
                if (scope.objData.lstShapes[i].Top > lane.Top || scope.objData.lstShapes[i].Top - (lane.Top + lane.Height) <= 0.2 && scope.objData.lstShapes[i].Top - (lane.Top + lane.Height) >= -0.2) {
                    scope.objData.lstShapes[i].Top = scope.objData.lstShapes[i].Top + diffY;
                    for (var j = 0; j < scope.objData.lstShapes[i].ShapeModel.Elements.length; j++) {
                        if (scope.objData.lstShapes[i].ShapeModel.Elements[j].Name == "flowNodeRef") {
                            if (scope.objData.lstShapes[i].ShapeModel.Elements[j].Value && scope.objData.lstShapes[i].ShapeModel.Elements[j].Value != "") {
                                var objShapeInCurrentLane = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[i].ShapeModel.Elements[j].Value, scope, "Element");
                                objShapeInCurrentLane.Top = objShapeInCurrentLane.Top + diffY;
                                if (objShapeInCurrentLane.LabelTop > 0) {
                                    objShapeInCurrentLane.LabelTop = objShapeInCurrentLane.LabelTop + diffY;
                                }
                                ydiffislaneChanged = -diffY;
                                ResetEdgePoints(objShapeInCurrentLane, ydiffislaneChanged, scope);
                            }
                        }
                    }
                }
            }
        }
    }
}

function ReArrangeEdges(selectedshape, scope) {
    var lstSourcePoints = [];
    var lstTargetPoints = [];
    // source
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeName == "BPMNEdge") {
            if (selectedshape.Id == scope.objData.lstShapes[i].SourceElement) {
                lstSourcePoints.push(scope.objData.lstShapes[i]);
            }
        }
    }

    if (lstSourcePoints.length > 0) {
        for (var j = 0; j < lstSourcePoints.length; j++) {
            var objSourceElement = null;
            var objTargetElement = null;
            for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                if (objSourceElement == null || objTargetElement == null) {
                    if (scope.objData.lstShapes[i].Id == lstSourcePoints[j].SourceElement) {
                        objSourceElement = scope.objData.lstShapes[i];
                    }
                    else if (scope.objData.lstShapes[i].Id == lstSourcePoints[j].TargetElement) {
                        objTargetElement = scope.objData.lstShapes[i];
                    }
                }
                else {
                    break;
                }
            }
            var wayPoints = lstSourcePoints[j].lstWayPoints;
            var targetPoint = wayPoints[wayPoints.length - 1];
            var sourcePoint = wayPoints[0];
            UpdateCurPoint(scope.selectedElemPrevBounds, sourcePoint, objSourceElement);
            sourcePoint = GetNewPointOnShapeResize(scope.selectedElemPrevBounds, sourcePoint, objSourceElement);
            var newPoints = GetEdgeWayPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint);

            while (newPoints.length == 2 && sourcePoint.Left != targetPoint.Left && sourcePoint.Top != targetPoint.Top) {
                newPoints = GetEdgeWayPointsForDottedLine(objSourceElement, objTargetElement);
            }
            newPoints = setShapeNameToWayPoints(newPoints);
            if (lstSourcePoints[j].ShapeType == "association") {
                var length = newPoints.length;
                if (newPoints.length > 2) {
                    var numberofElementsToRemove = length - 2;
                    newPoints.splice(1, numberofElementsToRemove);
                }
            }
            //upadting object and redrawing lines
            for (var k = 0; k < scope.objData.lstShapes.length; k++) {
                if (scope.objData.lstShapes[k].Id == lstSourcePoints[j].Id) {
                    scope.objData.lstShapes[k].lstWayPoints = newPoints;
                    break;
                }
            }

            ReDrawingEdges(scope);

        }
    }

    // Target
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeName == "BPMNEdge") {
            if (selectedshape.Id == scope.objData.lstShapes[i].TargetElement) {
                lstTargetPoints.push(scope.objData.lstShapes[i]);
            }
        }
    }

    if (lstTargetPoints.length > 0) {
        for (var j = 0; j < lstTargetPoints.length; j++) {
            var objSourceElement = null;
            var objTargetElement = null;
            for (var i = 0; i < scope.objData.lstShapes.length; i++) {
                if (objSourceElement == null || objTargetElement == null) {
                    if (scope.objData.lstShapes[i].Id == lstTargetPoints[j].SourceElement) {
                        objSourceElement = scope.objData.lstShapes[i];
                    }
                    else if (scope.objData.lstShapes[i].Id == lstTargetPoints[j].TargetElement) {
                        objTargetElement = scope.objData.lstShapes[i];
                    }
                }
                else {
                    break;
                }
            }
            var wayPoints = lstTargetPoints[j].lstWayPoints;
            var targetPoint = wayPoints[wayPoints.length - 1];
            UpdateCurPoint(scope.selectedElemPrevBounds, targetPoint, objTargetElement);
            targetPoint = GetNewPointOnShapeResize(scope.selectedElemPrevBounds, targetPoint, objTargetElement);
            sourcePoint = wayPoints[0];

            var newPoints = GetEdgeWayPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint);
            while (newPoints.length == 2 && sourcePoint.Left != targetPoint.Left && sourcePoint.Top != targetPoint.Top) {
                newPoints = GetEdgeWayPointsForDottedLine(objSourceElement, objTargetElement);
            }
            newPoints = setShapeNameToWayPoints(newPoints);
            if (lstTargetPoints[j].ShapeType == "association") {
                var length = newPoints.length;
                if (newPoints.length > 2) {
                    var numberofElementsToRemove = length - 2;
                    newPoints.splice(1, numberofElementsToRemove);
                }
            }
            //upadting object and redrawing lines
            for (var k = 0; k < scope.objData.lstShapes.length; k++) {
                if (scope.objData.lstShapes[k].Id == lstTargetPoints[j].Id) {
                    scope.objData.lstShapes[k].lstWayPoints = newPoints;
                    break;
                }
            }

            ReDrawingEdges(scope);
        }
    }

    scope.selectedElemPrevBounds.Left = selectedshape.Left;
    scope.selectedElemPrevBounds.Top = selectedshape.Top;
    scope.selectedElemPrevBounds.Height = selectedshape.Height;
    scope.selectedElemPrevBounds.Width = selectedshape.Width;
}

function GenerateNewWaypoints(isStartingEdge, objSourceElement, objTargetElement, sourcePoint, targetPoint) {
    if (isStartingEdge) {
        if (sourcePoint.Left > objSourceElement.Left + objSourceElement.Width) {
            sourcePoint.Left = objSourceElement.Left + objSourceElement.Width;
        }
        else if (sourcePoint.Left < objSourceElement.Left) {
            sourcePoint.Left = objSourceElement.Left;
        }
        else if (sourcePoint.Left > objSourceElement.Left && sourcePoint.Left < objSourceElement.Left + objSourceElement.Width) {
            if (sourcePoint.Top > objSourceElement.Top + objSourceElement.Height) {
                sourcePoint.Top = objSourceElement.Top + objSourceElement.Height;
            }
            else if (sourcePoint.Top < objSourceElement.Top) {
                sourcePoint.Top = objSourceElement.Top;
            }
            else if (sourcePoint.Top < objSourceElement.Top + objSourceElement.Height && sourcePoint.Top > objSourceElement.Top) {
                if (targetPoint.Top > objSourceElement.Top + objSourceElement.Height) {
                    sourcePoint.Top = objSourceElement.Top + objSourceElement.Height;
                }
                else {
                    sourcePoint.Top = objSourceElement.Top;
                }
            }
        }

        if (sourcePoint.Top < objSourceElement.Top) {
            sourcePoint.Top = objSourceElement.Top;
        }
        else if (sourcePoint.Top > objSourceElement.Top + objSourceElement.Height) {
            sourcePoint.Top = objSourceElement.Top + objSourceElement.Height;
        }
        //targetPoint = { Left: parseFloat(selectedLineEle.childNodes[selectedLineEle.childNodes.length - 1].getAttribute("x2")), Top: parseFloat(selectedLineEle.childNodes[selectedLineEle.childNodes.length - 1].getAttribute("y2")) };

    }
    else {
        if (targetPoint.Left > objTargetElement.Left + objTargetElement.Width) {
            targetPoint.Left = objTargetElement.Left + objTargetElement.Width;
        }
        else if (targetPoint.Left < objTargetElement.Left) {
            targetPoint.Left = objTargetElement.Left;
        }
        else if (targetPoint.Left > objTargetElement.Left && targetPoint.Left < objTargetElement.Left + objTargetElement.Width) {

            if (targetPoint.Top > objTargetElement.Top + objTargetElement.Height) {
                targetPoint.Top = objTargetElement.Top + objTargetElement.Height;
            }
            else if (targetPoint.Top < objTargetElement.Top) {
                targetPoint.Top = objTargetElement.Top;
            }
            else if (targetPoint.Top < objTargetElement.Top + objTargetElement.Height && targetPoint.Top > objTargetElement.Top) {
                if (sourcePoint.Top > objTargetElement.Top + objTargetElement.Height) {
                    targetPoint.Top = objTargetElement.Top + objTargetElement.Height;
                }
                else {
                    targetPoint.Top = objTargetElement.Top;
                }
            }
        }

        if (targetPoint.Top < objTargetElement.Top) {
            targetPoint.Top = objTargetElement.Top;
        }
        else if (targetPoint.Top > objTargetElement.Top + objTargetElement.Height) {
            targetPoint.Top = objTargetElement.Top + objTargetElement.Height;
        }
        //else if (targetPoint.Top <= objTargetElement.Top + objTargetElement.Width && targetPoint.Top >= objTargetElement.Top) {

        //}

        //sourcePoint = { Left: parseFloat(selectedLineEle.childNodes[0].getAttribute("x1")), Top: parseFloat(selectedLineEle.childNodes[0].getAttribute("y1")) };
    }
    newPoints = GetEdgeWayPoints(objSourceElement, objTargetElement, sourcePoint, targetPoint);

    return newPoints;
}

function UpdateCurPoint(prvBounds, curPoint, shape) {
    if (curPoint.Left == prvBounds.Left) {
        curPoint.Left = shape.Left;
    }
    else if (curPoint.Left == (prvBounds.Left + prvBounds.Width)) {
        curPoint.Left = shape.Left + shape.Width;
    }
    if (curPoint.Top == prvBounds.Top) {
        curPoint.Top = shape.Top;
    }
    else if (curPoint.Top == (prvBounds.Top + prvBounds.Height)) {
        curPoint.Top = shape.Top + shape.Height;
    }
}

function GetNewPointOnShapeResize(prvBounds, curPoint, shape) {
    var retVal = { Left: curPoint.Left, Top: curPoint.Top };
    if (curPoint.Left == shape.Left || curPoint.Left == (shape.Left + shape.Width)) {
        //var curDiff = (prvBounds.Top + prvBounds.Height) - curPoint.Top;
        //var newDiff = (shape.Height * curDiff) / prvBounds.Height;
        //retVal.Top = shape.Top + newDiff;
        var curdiffper = ((shape.Top + shape.Height) / (prvBounds.Top + prvBounds.Height)) * 100;
        var newPoint = (curdiffper * curPoint.Top) / 100;
        if (newPoint < shape.Top || newPoint > shape.Top + shape.Height) {
            newPoint = shape.Top + (shape.Height / 2);
        }
        retVal.Top = newPoint;
    }
    else if (curPoint.Top == shape.Top || curPoint.Top == (shape.Top + shape.Height)) {
        //var curDiff = (prvBounds.Left + prvBounds.Width) - curPoint.Left;
        //var newDiff = (shape.Width * curDiff) / prvBounds.Width;
        //retVal.Left = shape.Left + newDiff;
        var curdiffper = ((shape.Left + shape.Width) / (prvBounds.Left + prvBounds.Width)) * 100;
        var newPoint = (curdiffper * curPoint.Left) / 100;
        if (newPoint < shape.Left || newPoint > shape.Left + shape.Width) {
            newPoint = shape.Left + (shape.Width / 2);
        }
        retVal.Left = newPoint;
    }

    return retVal;
}

function moveResizergElement(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    if (scope && scope.isMouseDown && evt.buttons == 1) {
        dx = evt.offsetX - scope.currentX;
        dy = evt.offsetY - scope.currentY;
        var isValid = false;
        if (scope.SelectedElementLane != null || scope.selectedShape.ShapeType == "participant" || scope.selectedShape.ShapeType == "lane") {
            var laneXpos = 0;
            var laneYpos = 0;
            var laneHeight = 0;
            var laneWidth = 0;
            var laneTotalWidthPosition = 0;
            var laneTotalHeightPosition = 0;
            if (scope.SelectedElementLane != null) {
                laneXpos = parseFloat(scope.SelectedElementLane.getAttribute("x"));
                laneYpos = parseFloat(scope.SelectedElementLane.getAttribute("y"));
                laneHeight = parseFloat(scope.SelectedElementLane.getAttribute("height"));
                laneWidth = parseFloat(scope.SelectedElementLane.getAttribute("width"));
                laneTotalWidthPosition = laneXpos + laneWidth;
                laneTotalHeightPosition = laneYpos + laneHeight;
            }
            var ElementXpos = "";
            var ElementYpos = "";
            var ElementHeight = "";
            var ElementWidth = "";
            var diffx = 0;
            var diffy = 0;
            if (scope.selectedResizergElement.getAttribute("id") == "nw") {
                ElementXpos = parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx;
                ElementYpos = parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy;
                ElementHeight = parseFloat(scope.selectedElemForResize.getAttribute("height")) - dy;
                ElementWidth = parseFloat(scope.selectedElemForResize.getAttribute("width")) - dx;
                diffx = (ElementXpos + ElementWidth) - ElementXpos;
                diffy = (ElementYpos + ElementHeight) - ElementYpos;
                if (scope.selectedShape.ShapeType != "participant") {
                    if (laneXpos < ElementXpos && laneTotalWidthPosition > ElementXpos && laneYpos < ElementYpos && laneTotalHeightPosition > ElementYpos && diffx > 20 && diffy > 20) {
                        isValid = true;
                    }
                } else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.Left && scope.objMaxResizeLimits.Top && scope.objMaxResizeLimits.Right && scope.objMaxResizeLimits.Bottom && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if (ElementXpos <= (scope.objMaxResizeLimits.Left - 80) && ElementYpos <= scope.objMaxResizeLimits.Top && ElementYpos < scope.objMaxResizeLimits.LaneTopLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                } else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if (ElementYpos < scope.objMaxResizeLimits.LaneTopLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                } else {
                    isValid = true;
                }
            } else if (scope.selectedResizergElement.getAttribute("id") == "ne") {
                ElementXpos = parseFloat(scope.selectedElemForResize.getAttribute("x"));
                ElementYpos = parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy;
                ElementHeight = parseFloat(scope.selectedElemForResize.getAttribute("height")) - dy;
                ElementWidth = parseFloat(scope.selectedElemForResize.getAttribute("width")) + dx;
                diffx = (ElementXpos + ElementWidth) - ElementXpos;
                diffy = (ElementYpos + ElementHeight) - ElementYpos;
                if (scope.selectedShape.ShapeType != "participant") {
                    if (laneXpos < (ElementXpos + ElementWidth) && laneTotalWidthPosition > (ElementXpos + ElementWidth) && laneYpos < ElementYpos && laneTotalHeightPosition > ElementYpos && diffx > 20 && diffy > 20) {
                        isValid = true;
                    }
                } else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.Left && scope.objMaxResizeLimits.Top && scope.objMaxResizeLimits.Right && scope.objMaxResizeLimits.Bottom && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if ((ElementWidth + ElementXpos) >= scope.objMaxResizeLimits.Right && ElementYpos <= scope.objMaxResizeLimits.Top && ElementYpos < scope.objMaxResizeLimits.LaneTopLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                } else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if (ElementYpos < scope.objMaxResizeLimits.LaneTopLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                } else {
                    isValid = true;
                }
            }
            else if (scope.selectedResizergElement.getAttribute("id") == "sw") {
                ElementXpos = parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx;
                ElementYpos = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                ElementHeight = parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy;
                ElementWidth = parseFloat(scope.selectedElemForResize.getAttribute("width")) - dx;
                diffx = (ElementXpos + ElementWidth) - ElementXpos;
                diffy = (ElementYpos + ElementHeight) - ElementYpos;
                if (scope.selectedShape.ShapeType != "participant") {
                    if (laneXpos < ElementXpos && laneTotalWidthPosition > ElementXpos && laneYpos < (ElementYpos + ElementHeight) && laneTotalHeightPosition > (ElementYpos + ElementHeight) && diffx > 20 && diffy > 20) {
                        isValid = true;
                    }
                }
                else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.Left && scope.objMaxResizeLimits.Top && scope.objMaxResizeLimits.Right && scope.objMaxResizeLimits.Bottom && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if (ElementXpos <= (scope.objMaxResizeLimits.Left - 80) && (ElementYpos + ElementHeight) >= scope.objMaxResizeLimits.Bottom && (ElementYpos + ElementHeight) > scope.objMaxResizeLimits.LaneBottomLimit && ElementWidth > 200) {
                        isValid = true;
                    } else if (parseFloat(scope.selectedElemForResize.getAttribute("x")) >= ElementXpos && (ElementYpos + ElementHeight) >= (parseFloat(scope.selectedElemForResize.getAttribute("y")) + parseFloat(scope.selectedElemForResize.getAttribute("height")))) {
                        isValid = true;
                    }
                }
                else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if ((ElementYpos + ElementHeight) > scope.objMaxResizeLimits.LaneBottomLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                }
                else {
                    isValid = true;
                }
            }
            else if (scope.selectedResizergElement.getAttribute("id") == "se") {
                ElementXpos = parseFloat(scope.selectedElemForResize.getAttribute("x"));
                ElementYpos = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                ElementHeight = parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy;
                ElementWidth = parseFloat(scope.selectedElemForResize.getAttribute("width")) + dx;
                diffx = (ElementXpos + ElementWidth) - ElementXpos;
                diffy = (ElementYpos + ElementHeight) - ElementYpos;
                if (scope.selectedShape.ShapeType != "participant") {
                    if (laneXpos < (ElementXpos + ElementWidth) && laneTotalWidthPosition > (ElementXpos + ElementWidth) && laneYpos < (ElementYpos + ElementHeight) && laneTotalHeightPosition > (ElementYpos + ElementHeight) && diffx > 20 && diffy > 20) {
                        isValid = true;
                    }
                } else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.Left && scope.objMaxResizeLimits.Top && scope.objMaxResizeLimits.Right && scope.objMaxResizeLimits.Bottom && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if ((ElementWidth + ElementXpos) >= scope.objMaxResizeLimits.Right && (ElementYpos + ElementHeight) >= scope.objMaxResizeLimits.Bottom && (ElementYpos + ElementHeight) > scope.objMaxResizeLimits.LaneBottomLimit && ElementWidth > 200) {
                        isValid = true;
                    } else if ((ElementWidth + ElementXpos) >= (parseFloat(scope.selectedElemForResize.getAttribute("x")) + parseFloat(scope.selectedElemForResize.getAttribute("width"))) && (ElementYpos + ElementHeight) >= (parseFloat(scope.selectedElemForResize.getAttribute("y")) + parseFloat(scope.selectedElemForResize.getAttribute("height")))) {
                        isValid = true;
                    }
                }
                else if (scope.objMaxResizeLimits && scope.objMaxResizeLimits.LaneTopLimit && scope.objMaxResizeLimits.LaneBottomLimit) {
                    if ((ElementYpos + ElementHeight) > scope.objMaxResizeLimits.LaneBottomLimit && ElementWidth > 200) {
                        isValid = true;
                    }
                } else {
                    isValid = true;
                }
            }
            else if (scope.selectedResizergElement.getAttribute("id") == "nse" || scope.selectedResizergElement.getAttribute("id") == "nsw") {
                ElementYpos = parseFloat(scope.selectedElemForResize.getAttribute("y"));
                ElementHeight = parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy;
                var Yposition = ElementYpos + ElementHeight;
                if (scope.objMaxResizeLimits && Yposition > scope.objMaxResizeLimits.Bottom) {
                    isValid = true;
                }
            }
        } else {
            isValid = true;
        }
        if (isValid) {
            scope.isDirty = true;
            var parentNode = scope.selectedResizergElement.parentNode;
            if (scope.selectedResizergElement.getAttribute("id") != "nse" && scope.selectedResizergElement.getAttribute("id") != "nsw") {
                scope.selectedResizergElement.setAttribute('x', parseFloat(scope.selectedResizergElement.getAttribute("x")) + dx);
            }
            scope.selectedResizergElement.setAttribute('y', parseFloat(scope.selectedResizergElement.getAttribute("y")) + dy);

            angular.forEach($(scope.selectedResizergElement).children(), function (item) {
                if (scope.selectedResizergElement.getAttribute("id") != "nse" && scope.selectedResizergElement.getAttribute("id") != "nsw") {
                    item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                }
                item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);
            });
            var MainElement = null;
            for (var i = 0; i < scope.selectedElemForResize.childNodes.length; i++) {
                if (scope.selectedElemForResize.childNodes[i].getAttribute("id") == "MainElement") {
                    MainElement = scope.selectedElemForResize.childNodes[i];
                    break;
                }
            }
            var param = "";
            if (scope.selectedElemForResize.id == "textAnnotation") {
                param = "textAnnotation";
            }

            if (scope.selectedResizergElement.getAttribute("id") == "nw") {
                if (scope.selectedElemForResize != 0) {

                    scope.selectedElemForResize.setAttribute('width', parseFloat(scope.selectedElemForResize.getAttribute("width")) - dx);
                    scope.selectedElemForResize.setAttribute('height', parseFloat(scope.selectedElemForResize.getAttribute("height")) - dy);
                    scope.selectedElemForResize.setAttribute('y', parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy);
                    scope.selectedElemForResize.setAttribute('x', parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx);

                    angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
                        if (itm.nodeName == "rect") {
                            if (itm.id != "rectangleinsquareparam") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                            }
                            itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        }
                        else if (itm.nodeName == "text") {
                            if (scope.selectedElemForResize.id != "participant") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                                itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                                itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                                itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);

                                for (var i = 0; i < itm.childNodes.length; i++) {
                                    itm.removeChild(itm.childNodes[i]);
                                }
                                itm.textContent = scope.objData.lstShapes[scope.indexofResizerElement].Text;
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, param, scope);
                            }
                            else {
                                itm.setAttribute('x', parseFloat(itm.getAttribute('x')) + dx);
                                itm.setAttribute('y', parseFloat(itm.getAttribute('y')) + dy);
                                var tx = parseFloat(itm.getAttribute('x'));
                                var ty = parseFloat(itm.getAttribute('y'));
                                var height = parseFloat(MainElement.getAttribute('height'));
                                var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, "Participant", scope);
                                var translate = 10 + ' ' + height / 2;
                                textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                                textnode.setAttribute('text-anchor', 'middle');
                            }
                        }
                        else if (itm.nodeName == "image") {
                            if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "callActivity") {
                                var x = parseFloat(scope.selectedElemForResize.getAttribute("x")) + (parseFloat(scope.selectedElemForResize.getAttribute("width")) / 2) - 10;
                                var y = parseFloat(scope.selectedElemForResize.getAttribute("y")) + (parseFloat(scope.selectedElemForResize.getAttribute("height"))) - 20;

                                itm.setAttribute('x', x);
                                itm.setAttribute('y', y);
                            }
                            else {
                                itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                                itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                            }
                        }
                        else if (itm.nodeName == "line" && scope.selectedElemForResize.id == "participant") {
                            itm.setAttribute('y1', parseFloat(itm.getAttribute("y1")) + dy);
                            itm.setAttribute('x1', parseFloat(itm.getAttribute("x1")) + dx);
                            itm.setAttribute('x2', parseFloat(itm.getAttribute("x2")) + dx);
                        }
                        else if (itm.nodeName == "g" && scope.selectedElemForResize.id == "textAnnotation" && itm.id == "textAnnotationLineGroup") {
                            itm.childNodes[0].setAttribute('y1', parseFloat(itm.childNodes[0].getAttribute("y1")) + dy);
                            itm.childNodes[0].setAttribute('y2', parseFloat(itm.childNodes[0].getAttribute("y2")) + dy);
                            itm.childNodes[0].setAttribute('x1', parseFloat(itm.childNodes[0].getAttribute("x1")) + dx);
                            itm.childNodes[0].setAttribute('x2', parseFloat(itm.childNodes[0].getAttribute("x2")) + dx);
                            itm.childNodes[1].setAttribute('y1', parseFloat(itm.childNodes[1].getAttribute("y1")) + dy);
                            itm.childNodes[1].setAttribute('x1', parseFloat(itm.childNodes[1].getAttribute("x1")) + dx);
                            itm.childNodes[1].setAttribute('x2', parseFloat(itm.childNodes[1].getAttribute("x2")) + dx);
                            itm.childNodes[2].setAttribute('x1', parseFloat(itm.childNodes[2].getAttribute("x1")) + dx);
                            itm.childNodes[2].setAttribute('x2', parseFloat(itm.childNodes[2].getAttribute("x2")) + dx);
                        }
                    });
                }


                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "ne") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "sw") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergElement.getAttribute("id") == "ne") {
                if (scope.selectedElemForResize != 0) {
                    scope.selectedElemForResize.setAttribute('width', parseFloat(scope.selectedElemForResize.getAttribute("width")) + dx);
                    scope.selectedElemForResize.setAttribute('height', parseFloat(scope.selectedElemForResize.getAttribute("height")) - dy);
                    scope.selectedElemForResize.setAttribute('y', parseFloat(scope.selectedElemForResize.getAttribute("y")) + dy);

                    angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
                        if (itm.nodeName == "rect") {
                            if (itm.id != "rectangleinsquareparam") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                            }
                            itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        }
                        else if (itm.nodeName == "text") {
                            if (scope.selectedElemForResize.id != "participant") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                                itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                                itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);

                                for (var i = 0; i < itm.childNodes.length; i++) {
                                    itm.removeChild(itm.childNodes[i]);
                                }
                                itm.textContent = scope.objData.lstShapes[scope.indexofResizerElement].Text;
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, param, scope);
                            }
                            else {
                                itm.setAttribute('y', parseFloat(itm.getAttribute('y')) + dy);
                                var tx = parseFloat(itm.getAttribute('x'));
                                var ty = parseFloat(itm.getAttribute('y'));
                                var height = parseFloat(MainElement.getAttribute('height'));
                                var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, "Participant", scope);
                                var translate = 10 + ' ' + height / 2;
                                textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                                textnode.setAttribute('text-anchor', 'middle');
                            }
                        }
                        else if (itm.nodeName == "image") {
                            if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "callActivity") {
                                var x = parseFloat(scope.selectedElemForResize.getAttribute("x")) + (parseFloat(scope.selectedElemForResize.getAttribute("width")) / 2) - 10;
                                var y = parseFloat(scope.selectedElemForResize.getAttribute("y")) + (parseFloat(scope.selectedElemForResize.getAttribute("height"))) - 20;

                                itm.setAttribute('x', x);
                                itm.setAttribute('y', y);
                            }
                            else {
                                itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                            }
                        }
                        else if (itm.nodeName == "line" && scope.selectedElemForResize.id == "participant") {
                            itm.setAttribute('y1', parseFloat(itm.getAttribute("y1")) + dy);
                        }
                        else if (itm.nodeName == "g" && scope.selectedElemForResize.id == "textAnnotation" && itm.id == "textAnnotationLineGroup") {
                            itm.childNodes[0].setAttribute('y1', parseFloat(itm.childNodes[0].getAttribute("y1")) + dy);
                            itm.childNodes[0].setAttribute('y2', parseFloat(itm.childNodes[0].getAttribute("y2")) + dy);
                            itm.childNodes[1].setAttribute('y1', parseFloat(itm.childNodes[1].getAttribute("y1")) + dy);
                        }
                    });
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "nw") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "se") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergElement.getAttribute("id") == "sw") {
                if (scope.selectedElemForResize != 0) {
                    scope.selectedElemForResize.setAttribute('width', parseFloat(scope.selectedElemForResize.getAttribute("width")) - dx);
                    scope.selectedElemForResize.setAttribute('height', parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy);
                    scope.selectedElemForResize.setAttribute('x', parseFloat(scope.selectedElemForResize.getAttribute("x")) + dx);

                    angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
                        if (itm.nodeName == "rect") {
                            if (itm.id != "rectangleinsquareparam") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                            }
                            itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        }
                        else if (itm.nodeName == "text") {
                            if (scope.selectedElemForResize.id != "participant") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                                itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);
                                itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);

                                for (var i = 0; i < itm.childNodes.length; i++) {
                                    itm.removeChild(itm.childNodes[i]);
                                }
                                itm.textContent = scope.objData.lstShapes[scope.indexofResizerElement].Text;
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, param, scope);
                            }
                            else {
                                itm.setAttribute('x', parseFloat(itm.getAttribute('x')) + dx);
                                itm.setAttribute('y', parseFloat(itm.getAttribute('y')) + dy);
                                var tx = parseFloat(itm.getAttribute('x'));
                                var ty = parseFloat(itm.getAttribute('y'));
                                var height = parseFloat(MainElement.getAttribute('height'));
                                var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, "Participant", scope);
                                var translate = 10 + ' ' + height / 2;
                                textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                                textnode.setAttribute('text-anchor', 'middle');
                            }
                        }
                        else if (itm.nodeName == "image") {
                            if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "callActivity") {
                                var x = parseFloat(scope.selectedElemForResize.getAttribute("x")) + (parseFloat(scope.selectedElemForResize.getAttribute("width")) / 2) - 10;
                                var y = parseFloat(scope.selectedElemForResize.getAttribute("y")) + (parseFloat(scope.selectedElemForResize.getAttribute("height"))) - 20;

                                itm.setAttribute('x', x);
                                itm.setAttribute('y', y);
                            }
                            else {
                                itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                            }
                        }
                        else if (itm.nodeName == "line" && scope.selectedElemForResize.id == "participant") {
                            itm.setAttribute('y2', parseFloat(itm.getAttribute("y2")) + dy);
                            itm.setAttribute('x1', parseFloat(itm.getAttribute("x1")) + dx);
                            itm.setAttribute('x2', parseFloat(itm.getAttribute("x2")) + dx);
                        }
                        else if (itm.nodeName == "g" && scope.selectedElemForResize.id == "textAnnotation" && itm.id == "textAnnotationLineGroup") {
                            itm.childNodes[2].setAttribute('y1', parseFloat(itm.childNodes[2].getAttribute("y1")) + dy);
                            itm.childNodes[2].setAttribute('y2', parseFloat(itm.childNodes[2].getAttribute("y2")) + dy);
                            itm.childNodes[2].setAttribute('x1', parseFloat(itm.childNodes[2].getAttribute("x1")) + dx);
                            itm.childNodes[2].setAttribute('x2', parseFloat(itm.childNodes[2].getAttribute("x2")) + dx);
                            itm.childNodes[1].setAttribute('y2', parseFloat(itm.childNodes[1].getAttribute("y2")) + dy);
                            itm.childNodes[1].setAttribute('x2', parseFloat(itm.childNodes[1].getAttribute("x2")) + dx);
                            itm.childNodes[1].setAttribute('x1', parseFloat(itm.childNodes[1].getAttribute("x1")) + dx);
                            itm.childNodes[0].setAttribute('x2', parseFloat(itm.childNodes[0].getAttribute("x2")) + dx);
                            itm.childNodes[0].setAttribute('x1', parseFloat(itm.childNodes[0].getAttribute("x1")) + dx);
                        }
                    });
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "se") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "nw") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergElement.getAttribute("id") == "se") {
                if (scope.selectedElemForResize != 0) {
                    scope.selectedElemForResize.setAttribute('width', parseFloat(scope.selectedElemForResize.getAttribute("width")) + dx);
                    scope.selectedElemForResize.setAttribute('height', parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy);

                    angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
                        if (itm.nodeName == "rect") {
                            if (itm.id != "rectangleinsquareparam") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                            }
                            itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);
                        }
                        else if (itm.nodeName == "text") {
                            if (scope.selectedElemForResize.id != "participant") {
                                itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                                itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);

                                for (var i = 0; i < itm.childNodes.length; i++) {
                                    itm.removeChild(itm.childNodes[i]);
                                }
                                itm.textContent = scope.objData.lstShapes[scope.indexofResizerElement].Text;
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, param, scope);
                            }
                            else {
                                itm.setAttribute('y', parseFloat(itm.getAttribute('y')) + dy);
                                var tx = parseFloat(itm.getAttribute('x'));
                                var ty = parseFloat(itm.getAttribute('y'));
                                var height = parseFloat(MainElement.getAttribute('height'));
                                var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                                var textnode = wraptorect(itm, MainElement, 0, 0.5, "Participant", scope);
                                var translate = 10 + ' ' + height / 2;
                                textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                                textnode.setAttribute('text-anchor', 'middle');
                            }
                        }
                        else if (itm.nodeName == "image") {
                            if (scope.objData.lstShapes[scope.indexofResizerElement].ShapeType == "callActivity") {
                                var x = parseFloat(scope.selectedElemForResize.getAttribute("x")) + (parseFloat(scope.selectedElemForResize.getAttribute("width")) / 2) - 10;
                                var y = parseFloat(scope.selectedElemForResize.getAttribute("y")) + (parseFloat(scope.selectedElemForResize.getAttribute("height"))) - 20;

                                itm.setAttribute('x', x);
                                itm.setAttribute('y', y);
                            }
                        }
                        else if (itm.nodeName == "line" && scope.selectedElemForResize.id == "participant") {
                            itm.setAttribute('y2', parseFloat(itm.getAttribute("y2")) + dy);
                        }
                        else if (itm.nodeName == "g" && scope.selectedElemForResize.id == "textAnnotation" && itm.id == "textAnnotationLineGroup") {
                            itm.childNodes[2].setAttribute('y1', parseFloat(itm.childNodes[2].getAttribute("y1")) + dy);
                            itm.childNodes[2].setAttribute('y2', parseFloat(itm.childNodes[2].getAttribute("y2")) + dy);
                            itm.childNodes[1].setAttribute('y2', parseFloat(itm.childNodes[1].getAttribute("y2")) + dy);
                        }
                    });
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "sw") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "ne") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergElement.getAttribute("id") == "nse" || scope.selectedResizergElement.getAttribute("id") == "nsw") {
                if (scope.selectedElemForResize != 0) {
                    scope.selectedElemForResize.setAttribute('height', parseFloat(scope.selectedElemForResize.getAttribute("height")) + dy);
                    angular.forEach($(scope.selectedElemForResize).children(), function (itm) {
                        if (itm.nodeName == "rect") {
                            itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);
                        }
                        else if (itm.nodeName == "text") {
                            itm.setAttribute('y', parseFloat(itm.getAttribute('y')) + dy);
                            var tx = parseFloat(itm.getAttribute('x'));
                            var ty = parseFloat(itm.getAttribute('y'));
                            var height = parseFloat(MainElement.getAttribute('height'));
                            var rotate = -90 + '  ' + tx + ',' + ty + ' ';
                            var textnode = wraptorect(itm, MainElement, 0, 0.5, "Participant", scope);
                            var translate = 10 + ' ' + height / 2;
                            textnode.setAttribute('transform', 'translate(' + translate + ') rotate(' + rotate + ')');
                            textnode.setAttribute('text-anchor', 'middle');
                        }

                    });
                    var resizepointsID = "nse";
                    if (scope.selectedResizergElement.getAttribute("id") == "nse") {
                        resizepointsID = "nsw";
                    }
                    angular.forEach($(parentNode).children(), function (item) {
                        if (item.getAttribute('id') == resizepointsID) {
                            item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                            angular.forEach($(item).children(), function (itm) {
                                itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                            });
                        }
                    });
                }
            }

            scope.hoverElement = scope.selectedElemForResize;
            hoverOnElement(null, scope);

            for (var i = 0; i < scope.selectedResizergElement.parentNode.childNodes.length; i++) {

                if (scope.selectedResizergElement.parentNode.childNodes[i].id == "resizerborder") {
                    x = parseFloat(scope.selectedElemForResize.getAttribute("x")) - 5;
                    y = parseFloat(scope.selectedElemForResize.getAttribute("y")) - 5;
                    Height = parseFloat(scope.selectedElemForResize.getAttribute("height")) + 10;
                    Width = parseFloat(scope.selectedElemForResize.getAttribute("width")) + 10;

                    for (var j = 0; j < scope.selectedResizergElement.parentNode.childNodes[i].childNodes.length; j++) {
                        scope.selectedResizergElement.parentNode.childNodes[i].removeChild(scope.selectedResizergElement.parentNode.childNodes[i].childNodes[j]);
                        j--;
                    }

                    var Resizerpoints = [];
                    var Point1 = { Left: x, Top: y };
                    Resizerpoints.push(Point1);
                    var Point2 = { Left: x + Width, Top: y };
                    Resizerpoints.push(Point2);
                    var Point3 = { Left: x + Width, Top: y + Height };
                    Resizerpoints.push(Point3);
                    var Point4 = { Left: x, Top: y + Height };
                    Resizerpoints.push(Point4);
                    Resizerpoints.push(Point1);

                    for (var k = 0; k < Resizerpoints.length; k++) {
                        if (k < Resizerpoints.length - 1) {
                            var line = document.createElementNS(scope.svgNS, "line");
                            line.setAttribute('id', "resizerline");
                            line.setAttribute("x1", Resizerpoints[k].Left);
                            line.setAttribute("y1", Resizerpoints[k].Top);
                            line.setAttribute("x2", Resizerpoints[k + 1].Left);
                            line.setAttribute("y2", Resizerpoints[k + 1].Top);
                            line.setAttribute("stroke-width", 2);
                            line.setAttributeNS(null, "stroke", "#4c4c4c");
                            scope.selectedResizergElement.parentNode.childNodes[i].appendChild(line);
                        }
                    }

                    break;
                }
            }
            if (scope.selectedElemForResize.id != "participant" && scope.selectedElemForResize.id != "textAnnotation" && scope.selectedElemForResize.id != "lane") {
                GenerateRightPanelForAddingElements(parentNode, scope.selectedElemForResize, scope);
            }
            scope.currentX = evt.offsetX;
            scope.currentY = evt.offsetY;
        }
    } else {
        deselectResizergElement(evt);
    }
}

// Re-Arranging the Edges
function removeRearrangingLinePoints(scope) {
    for (k = 0; k < scope.svgElement.childNodes.length; k++) {
        if (scope.svgElement.childNodes[k].nodeName != "#text" && scope.svgElement.childNodes[k].nodeName != "#comment") {
            if (scope.svgElement.childNodes[k].getAttribute("id") == "linedraggablegroup") {
                scope.svgElement.removeChild(scope.svgElement.childNodes[k]);
                break;
            }
        }
    }
}

function getObjShapeoftheLine(param, scope, lineId) {
    if (scope.activeTab == "EXECUTION") {
        for (var j = 0; j < scope.lstExecutionTabShapes.length; j++) {
            if (scope.lstExecutionTabShapes[j].ShapeType == "sequenceFlow" || scope.lstExecutionTabShapes[j].ShapeType == "dataOutputAssociation" || scope.lstExecutionTabShapes[j].ShapeType == "association" || scope.lstExecutionTabShapes[j].ShapeType == "messageFlow" || scope.lstExecutionTabShapes[j].ShapeType == "dataInputAssociation") {
                if (scope.lstExecutionTabShapes[j].Id == lineId) {
                    if (param == "") {
                        scope.indexOfSelectedLine = j;
                        scope.selectedShape = scope.lstExecutionTabShapes[j];
                    } else {
                        scope.objDragOverLineElement = scope.lstExecutionTabShapes[j];
                    }
                    break;
                }
            }
        }
    } else {
        for (var j = 0; j < scope.objData.lstShapes.length; j++) {
            if (scope.objData.lstShapes[j].ShapeType == "sequenceFlow" || scope.objData.lstShapes[j].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[j].ShapeType == "association" || scope.objData.lstShapes[j].ShapeType == "messageFlow" || scope.objData.lstShapes[j].ShapeType == "dataInputAssociation") {
                if (scope.objData.lstShapes[j].Id == lineId) {
                    if (param == "") {
                        scope.indexOfSelectedLine = j;
                        scope.selectedShape = scope.objData.lstShapes[j];
                    } else {
                        scope.objDragOverLineElement = scope.objData.lstShapes[j];
                    }
                    break;
                }
            }
        }
    }

    if (param != "") {
        return scope.objDragOverLineElement;
    }
}

function selectedlineElement(evt, eleorscope) {
    var scope;
    if (evt && evt != null) {
        scope = angular.element(eleorscope).scope();
    } else {
        scope = eleorscope;
    }
    if (scope.selectedElementID) {
        function resetSelectedShape() {
            scope.selectedElementID = undefined;
        }
        scope.$apply(resetSelectedShape);
    }
    //var scope = getCurrentFileScope();
    if (scope.selectedLineEle && scope.activeTab == "MAP") {
        setColorToSelectedLine(scope.selectedLineEle, "remove");
    }
    removeRearrangingLinePoints(scope);
    removeSelection("line", scope);
    removeSelectionForLane(scope);
    scope.selectedElemForResize = null;
    scope._previousPosition = { Left: 0, Top: 0 };
    scope.linecount = 0;
    removeResizerAroundElement(scope);
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttribute("id", "linedraggablegroup");
    //g.setAttribute("onmousedown", 'selectedlineElement(evt)');
    scope.svgElement.appendChild(g);

    if (evt != null && evt != undefined) {
        scope.selectedLineEle = evt.currentTarget;
    }
    if (scope.activeTab == "MAP") {
        setColorToSelectedLine(scope.selectedLineEle, "set");
    }
    checkTitleIsPresentOrDeleteTitle(scope.selectedLineEle, "Delete");
    var sourceElementId = "";
    var targetElementId = "";
    //getting selected Line
    var lineId = scope.selectedLineEle.childNodes[0].getAttribute("id");
    getObjShapeoftheLine("", scope, lineId);
    if (scope.selectedShape != null && scope.selectedShape != undefined && scope.selectedShape != "") {
        sourceElementId = scope.selectedShape.SourceElement;
        targetElementId = scope.selectedShape.TargetElement;
    }

    if (sourceElementId != "" && targetElementId != "") {
        //getting sourceElement && targetElement
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (scope.objData.lstShapes[i].Id == sourceElementId) {
                scope.selectedLineSourceElement = scope.objData.lstShapes[i];
            }
            else if (scope.objData.lstShapes[i].Id == targetElementId) {
                scope.selectedLineTargetElement = scope.objData.lstShapes[i];
            }
        }
    }

    if (scope.activeTab == "MAP" && scope.IsMapEditable && !scope.IsVersionDisplay) {
        if (scope.selectedLineEle.childNodes.length > 1) {
            for (var i = 0; i < scope.selectedLineEle.childNodes.length; i++) {
                var x = 0;
                var y = 0;
                if (i == 0) {
                    var x1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x1"));
                    var y1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y1"));
                    x = x1;
                    y = y1;
                    var x2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x2"));
                    var y2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y2"));
                    if ((x1 - x2) >= -1 && (x1 - x2) <= 1) {
                        var dir = "crosshair";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                    else if ((y1 - y2) >= -1 && (y1 - y2) <= 1) {
                        var dir = "crosshair";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                }
                else if (i == scope.selectedLineEle.childNodes.length - 1) {
                    var x2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x2"));
                    var y2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y2"));
                    x = x2;
                    y = y2;
                    var x1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x1"));
                    var y1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y1"));
                    if ((x1 - x2) >= -1 && (x1 - x2) <= 1) {
                        var dir = "crosshair";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                    else if ((y1 - y2) >= -1 && (y1 - y2) <= 1) {
                        var dir = "crosshair";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                }
                else {
                    var x1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x1"));
                    var y1 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y1"));

                    var x2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("x2"));
                    var y2 = parseFloat(scope.selectedLineEle.childNodes[i].childNodes[0].getAttribute("y2"));

                    if ((x1 - x2) >= -1 && (x1 - x2) <= 1) {
                        x = x1;
                        y = (y1 + y2) / 2;
                        var dir = "ew";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                    else if ((y1 - y2) >= -1 && (y1 - y2) <= 1) {
                        y = y1;
                        x = (x1 + x2) / 2;
                        var dir = "ns";
                        generateBoundsDynamicallyToLine(dir, x, y, g, scope);
                    }
                }
            }
        }
        else {
            var x1 = parseFloat(scope.selectedLineEle.childNodes[0].childNodes[0].getAttribute("x1"));
            var y1 = parseFloat(scope.selectedLineEle.childNodes[0].childNodes[0].getAttribute("y1"));
            var x2 = parseFloat(scope.selectedLineEle.childNodes[0].childNodes[0].getAttribute("x2"));
            var y2 = parseFloat(scope.selectedLineEle.childNodes[0].childNodes[0].getAttribute("y2"));

            if ((x1 - x2) >= -1 && (x1 - x2) <= 1) {
                var dir = "crosshair";
                generateBoundsDynamicallyToLine(dir, x1, y1, g, scope);
                generateBoundsDynamicallyToLine(dir, x2, y2, g, scope);
            }
            else if ((y1 - y2) >= -1 && (y1 - y2) <= 1) {
                var dir = "crosshair";
                generateBoundsDynamicallyToLine(dir, x1, y1, g, scope);
                generateBoundsDynamicallyToLine(dir, x2, y2, g, scope);
            }
            else {
                var dir = "crosshair";
                generateBoundsDynamicallyToLine(dir, x1, y1, g, scope);
                generateBoundsDynamicallyToLine(dir, x2, y2, g, scope);
            }
        }
    }
    scope.showDetails();
    if (scope.selectedShape) {
        scope.selectedElementID = scope.selectedShape.Id;
    }
    scope.$apply();
}

function setColorToSelectedLine(selectedlineElement, param) {
    if (selectedlineElement) {
        for (var i = 0; i < selectedlineElement.childNodes.length; i++) {
            if (selectedlineElement.childNodes[i].nodeName == "g") {
                if (param == 'set') {
                    selectedlineElement.childNodes[i].childNodes[0].setAttribute("stroke", "#dd4205");
                    selectedlineElement.childNodes[i].childNodes[0].setAttribute("stroke-width", 1);
                    selectedlineElement.childNodes[i].childNodes[1].setAttributeNS(null, "stroke", "#dd4205");
                    selectedlineElement.childNodes[i].childNodes[1].setAttributeNS(null, "stroke-opacity", 0.1);
                } else {
                    selectedlineElement.childNodes[i].childNodes[0].setAttribute("stroke", "#3a3a3a");
                    selectedlineElement.childNodes[i].childNodes[0].setAttribute("stroke-width", 1);
                    selectedlineElement.childNodes[i].childNodes[1].setAttributeNS(null, "stroke", "transparent");
                }
            }
        }
    }
}

function generateBoundsDynamicallyToLine(dir, x, y, g, scope) {
    //var scope = getCurrentFileScope();
    var gchild = document.createElementNS(scope.svgNS, "g");
    gchild.setAttribute("id", scope.linecount);
    scope.linecount++;
    gchild.setAttribute("onmousedown", 'selectedlinegtagElement(evt)');
    if (dir == "ns") {
        gchild.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-n");
    }
    else if (dir == "ew") {
        gchild.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-e");
    } else if (dir == "crosshair") {
        gchild.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-crosshair");
    }

    var rectangle = document.createElementNS(scope.svgNS, "rect");
    rectangle.setAttributeNS(null, "id", "rectanglene");
    rectangle.setAttributeNS(null, "class", "djs-resizer-visual");
    rectangle.setAttributeNS(null, "x", x - 2.5);
    rectangle.setAttributeNS(null, "y", y - 2.5);
    rectangle.setAttributeNS(null, "height", 5);
    rectangle.setAttributeNS(null, "width", 5);
    rectangle.setAttributeNS(null, "fill", "orange");
    rectangle.setAttributeNS(null, "stroke", "none");
    gchild.appendChild(rectangle);

    var rectangle2nd = document.createElementNS(scope.svgNS, "rect");
    rectangle2nd.setAttributeNS(null, "id", "rectanglene");
    rectangle2nd.setAttributeNS(null, "class", "djs-resizer-hit");
    rectangle2nd.setAttributeNS(null, "x", x - 5);
    rectangle2nd.setAttributeNS(null, "y", y - 5);
    rectangle2nd.setAttributeNS(null, "height", 10);
    rectangle2nd.setAttributeNS(null, "width", 10);
    rectangle2nd.setAttributeNS(null, "fill", "orange");
    rectangle2nd.setAttributeNS(null, "stroke", "none");
    gchild.appendChild(rectangle2nd);
    g.appendChild(gchild);
}


function selectedlinegtagElement(evnt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    if (scope.activeTab != "EXECUTION") {
        scope.selectedlinegtagEle = evnt.currentTarget;
        scope.selectedlineindex = parseFloat(scope.selectedlinegtagEle.getAttribute("id"));

        scope.currentX = evnt.offsetX;
        scope.currentY = evnt.offsetY;
        scope.isMouseDown = true;
        scope.cursorPositionOnElement = "";
        scope.svgElement.setAttributeNS(null, "onmousemove", "movelinegElement(evt)");
        //scope.svgElement.setAttributeNS(null, "onmouseout", "deselectlinegElementonMouseUp(evt)");
        scope.svgElement.setAttributeNS(null, "onmouseup", "deselectlinegElementonMouseUp(evt)");
    }
}

function getLastlaneIndex(scope) {
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        var elementType;
        var index = 0;
        if (scope.svgElement.childNodes[i].getAttribute) {
            elementType = scope.svgElement.childNodes[i].getAttribute("elementtype");
        }
        if (elementType && elementType != "participant" && elementType != "lane") {
            index = i;
            break;
        }
    }
    return index;
};

function getEdgePosition(XorYPosition, hoverElement, changeDiff, isYCoordinate) {
    var aPosition;
    if (isYCoordinate) {
        if (XorYPosition >= hoverElement.Top && XorYPosition <= (hoverElement.Top + changeDiff)) {
            aPosition = "Top";
        } else if (XorYPosition <= (hoverElement.Top + hoverElement.Height) && XorYPosition >= (hoverElement.Top + hoverElement.Height - changeDiff)) {
            aPosition = "Bottom";
        }
    } else {
        if (XorYPosition >= hoverElement.Left && XorYPosition <= (hoverElement.Left + changeDiff)) {
            aPosition = "Left";
        } else if (XorYPosition <= (hoverElement.Left + hoverElement.Width) && XorYPosition >= (hoverElement.Left + hoverElement.Width - changeDiff)) {
            aPosition = "Right";
        }
    }
    return aPosition;
}

function movelinegElement(evnt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    if (scope.isMouseDown && evnt.buttons == 1) {
        if (scope.ClonedLineElement == null) {
            scope.isDirty = true;
            scope.ClonedLineElement = scope.selectedLineEle.cloneNode(true);
            scope.$apply(function () { scope.DraggableElement = true; });

            scope.selectedLineEle.setAttribute("style", "opacity: 0.5;");
            scope.ClonedLineElement.setAttribute("id", "ClonedLine");
            var ind = getLastlaneIndex(scope);
            scope.svgElement.insertBefore(scope.ClonedLineElement, scope.svgElement.childNodes[ind]);
            //scope.svgElement.appendChild(scope.ClonedLineElement);
        }
        //checkTitleIsPresentOrDeleteTitle(scope.ClonedLineElement, "Delete");
        if (scope.hoverElement != null) {
            removehoverCircle(evnt, evnt.currentTarget);
        }
        dx = evnt.offsetX - scope.currentX;
        dy = evnt.offsetY - scope.currentY;

        var x1 = 0;
        var y1 = 0;
        var x2 = 0;
        var y2 = 0;

        var x_1 = 0;
        var y_1 = 0;
        var x_2 = 0;
        var y_2 = 0;
        var edgeVariationDiff = 4;
        var changeEdgeDiff = 7;
        var edgePosition;

        if (scope.selectedlinegtagEle != 0) {
            var ele = "";

            if (scope.ClonedLineElement.childNodes.length > 1) {
                if ((scope.selectedlineindex == scope.selectedLineEle.childNodes.length - 1) || (scope.selectedlineindex == 1 && scope.selectedLineEle.childNodes.length == 1)) {
                    ele = scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1];
                }
                else {
                    ele = scope.ClonedLineElement.childNodes[scope.selectedlineindex];
                }
                if (ele) {
                    x1 = parseFloat(ele.childNodes[0].getAttribute("x1"));
                    y1 = parseFloat(ele.childNodes[0].getAttribute("y1"));
                    x2 = parseFloat(ele.childNodes[0].getAttribute("x2"));
                    y2 = parseFloat(ele.childNodes[0].getAttribute("y2"));
                } else {
                    deselectlinegElementonMouseUp(evnt);
                }
            }
            if (scope.ClonedLineElement && scope.ClonedLineElement.childNodes) {
                if (scope.ClonedLineElement.childNodes.length == 1) {

                    if (scope.selectedlineindex == 0) {
                        scope.ClonedLineElement.childNodes[0].childNodes[0].setAttribute("x1", parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("x1")) + dx);
                        scope.ClonedLineElement.childNodes[0].childNodes[0].setAttribute("y1", parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("y1")) + dy);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[0].childNodes[1].setAttribute("x1", parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[1].getAttribute("x1")) + dx);
                        scope.ClonedLineElement.childNodes[0].childNodes[1].setAttribute("y1", parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[1].getAttribute("y1")) + dy);

                        if (scope.targetElementForLine != null) {
                            scope.ClonedLineSourceElement = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
                        } else {
                            scope.ClonedLineSourceElement = scope.selectedLineSourceElement;
                        }
                        var source = scope.ClonedLineSourceElement;
                        var sourcePoint = { Left: parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2")), Top: parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2")) };
                        var aCurPos = { Left: evnt.offsetX, Top: evnt.offsetY };
                        var destination = scope.selectedLineTargetElement;
                        scope._previousPosition = aCurPos;

                        if (scope.targetElementForLine == null) {
                            source = { Left: evnt.offsetX, Top: evnt.offsetY, Height: 10, Width: 10 };
                        }
                        var atargetPoint = UpdateTargetPoint(aCurPos, destination, source, scope);

                        if (atargetPoint != undefined) {
                            var Newpoints = GetEdgeWayPoints(destination, source, sourcePoint, atargetPoint);
                            var Points = [];
                            for (var i = Newpoints.length - 1; i >= 0; i--) {
                                Points.push(Newpoints[i]);
                            }
                            GenerateClonedLine(Points, scope.ClonedLineElement, scope);
                        }
                    }
                    else if (scope.selectedlineindex == 1) {
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("x2", parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2")) + dx);
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("y2", parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2")) + dy);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("x2", parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].getAttribute("x2")) + dx);
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("y2", parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].getAttribute("y2")) + dy);

                        if (scope.targetElementForLine != null) {
                            scope.ClonedLineTargetElement = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
                        } else {
                            scope.ClonedLineTargetElement = scope.selectedLineTargetElement;
                        }
                        var source = scope.selectedLineSourceElement;
                        var sourcePoint = { Left: parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("x1")), Top: parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("y1")) };
                        var aCurPos = { Left: evnt.offsetX, Top: evnt.offsetY };
                        var destination = scope.ClonedLineTargetElement;
                        scope._previousPosition = aCurPos;

                        if (scope.targetElementForLine == null) {
                            source = { Left: evnt.offsetX, Top: evnt.offsetY, Height: 10, Width: 10 };
                        }
                        var atargetPoint = UpdateTargetPoint(aCurPos, destination, source, scope);
                        if (atargetPoint != undefined) {
                            var Newpoints = GetEdgeWayPoints(source, destination, sourcePoint, atargetPoint);
                            GenerateClonedLine(Newpoints, scope.ClonedLineElement, scope);
                        }
                    }

                    var intersectionPoint = "";
                    if (scope.targetElementForLine != null) {
                        if (scope.selectedlineindex == 0) {
                            intersectionPoint = GetIntersectionPoint(scope.ClonedLineElement, "First", scope);
                        }
                        else {
                            intersectionPoint = GetIntersectionPoint(scope.ClonedLineElement, "Last", scope);
                        }
                        removeIntersectionPoint(scope);
                        generatePointAtintersectionOflineandElement(intersectionPoint, scope);
                    }
                    else {
                        removeIntersectionPoint(scope);
                    }

                }
                else if (scope.selectedlineindex == 0) {

                    if (scope.targetElementForLine != null) {
                        scope.ClonedLineSourceElement = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
                    } else {
                        scope.ClonedLineSourceElement = scope.selectedLineSourceElement;
                    }
                    x1 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x1"));
                    x2 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x2"));
                    y1 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y1"));
                    y2 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y2"));

                    if (scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1] != undefined) {
                        x_1 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].getAttribute("x1"));
                        y_1 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].getAttribute("y1"));
                        x_2 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].getAttribute("x2"));
                        y_2 = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].getAttribute("y2"));
                    }

                    if ((x1 - x2) >= -1 && (x1 - x2) <= 1 && (y_1 - y_2) > -1 && (y_1 - y_2) < 1) {
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x2", x2 + dx);
                        //for shadow line
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x2", x2 + dx);

                        if (scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1] != undefined) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].setAttribute("x1", x1 + dx);
                            //for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[1].setAttribute("x1", x1 + dx);
                        }
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", y1 + dy);
                        //for shadow line
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", y1 + dy);

                        scope.isEdgeChanged = false;
                        edgePosition = getEdgePosition(y1, scope.ClonedLineSourceElement, changeEdgeDiff, true);
                        if (!scope.cursorPositionOnElement) {
                            scope.cursorPositionOnElement = edgePosition;
                        }
                        var xpos = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x1"));
                        var ypos = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y1"));

                        if (ypos <= scope.ClonedLineSourceElement.Top + scope.ClonedLineSourceElement.Height + 1 && ypos >= scope.ClonedLineSourceElement.Top - 1) {
                            if (xpos <= (scope.ClonedLineSourceElement.Left + scope.ClonedLineSourceElement.Width - edgeVariationDiff) && xpos >= (scope.ClonedLineSourceElement.Left + edgeVariationDiff)) {
                                scope.isEdgeChanged = false;
                                if (edgePosition && scope.cursorPositionOnElement != edgePosition) {
                                    scope.isEdgeChanged = true;
                                    scope.cursorPositionOnElement = "";
                                }
                            }
                            else {
                                scope.isEdgeChanged = true;
                            }
                        }
                        else if (xpos > (scope.ClonedLineSourceElement.Left + scope.ClonedLineSourceElement.Width) || xpos < (scope.ClonedLineSourceElement.Left)) {
                            scope.isEdgeChanged = true;
                        }
                        else {
                            scope.isEdgeChanged = true;
                        }

                    }
                    else if ((y1 - y2) >= -1 && (y1 - y2) <= 1 && (x_1 - x_2) > -1 && (x_1 - x_2) < 1) {
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", y1 + dy);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y2", y2 + dy);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", y1 + dy);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y2", y2 + dy);
                        if (scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1] != undefined) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].setAttribute("y1", y1 + dy);
                            // for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[1].setAttribute("y1", y1 + dy);
                        }

                        var xpos = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x1"));
                        var ypos = parseFloat(scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y1"));
                        scope.isEdgeChanged = false;
                        edgePosition = getEdgePosition(x1, scope.ClonedLineSourceElement, changeEdgeDiff, false);
                        if (!scope.cursorPositionOnElement) {
                            scope.cursorPositionOnElement = edgePosition;
                        }
                        if (xpos <= scope.ClonedLineSourceElement.Left + scope.ClonedLineSourceElement.Width + 1 && xpos >= scope.ClonedLineSourceElement.Left - 1) {
                            if (ypos <= (scope.ClonedLineSourceElement.Top + scope.ClonedLineSourceElement.Height - edgeVariationDiff) && ypos >= (scope.ClonedLineSourceElement.Top + edgeVariationDiff)) {
                                scope.isEdgeChanged = false;
                                if (edgePosition && scope.cursorPositionOnElement != edgePosition) {
                                    scope.isEdgeChanged = true;
                                    scope.cursorPositionOnElement = "";
                                }
                            }
                            else {
                                scope.isEdgeChanged = true;
                            }
                        }
                        else if (ypos > scope.ClonedLineSourceElement.Top + scope.ClonedLineSourceElement.Height || ypos < scope.ClonedLineSourceElement.Top) {
                            scope.isEdgeChanged = true;
                        }
                        else {
                            scope.isEdgeChanged = true;
                        }
                    }

                    if (scope.isEdgeChanged && scope.targetElementForLine != null) {
                        var source = scope.ClonedLineSourceElement;
                        var sourcePoint = { Left: parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2")), Top: parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2")) };
                        var aCurPos = { Left: evnt.offsetX, Top: evnt.offsetY };
                        var destination = scope.selectedLineTargetElement;
                        scope._previousPosition = aCurPos;

                        if (scope.targetElementForLine == null) {
                            source = { Left: evnt.offsetX, Top: evnt.offsetY, Height: 10, Width: 10 };
                        }
                        var atargetPoint = UpdateTargetPoint(aCurPos, source, destination, scope);

                        if (atargetPoint != undefined) {
                            var Newpoints = GetEdgeWayPoints(destination, source, sourcePoint, atargetPoint);
                            var Points = [];
                            for (var i = Newpoints.length - 1; i >= 0; i--) {
                                Points.push(Newpoints[i]);
                            }
                            GenerateClonedLine(Points, scope.ClonedLineElement, scope);
                        }
                    }

                    if (scope.targetElementForLine != null) {
                        var intersectionPoint = GetIntersectionPoint(scope.ClonedLineElement, "First", scope);
                        removeIntersectionPoint(scope);
                        generatePointAtintersectionOflineandElement(intersectionPoint, scope);
                    }
                    else {
                        removeIntersectionPoint(scope);
                    }
                }
                else if (scope.selectedlineindex == scope.selectedLineEle.childNodes.length - 1 || (scope.selectedLineEle.childNodes.length == 1 && scope.selectedlineindex == 1)) {
                    if (scope.targetElementForLine != null) {
                        scope.ClonedLineTargetElement = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
                    } else {
                        scope.ClonedLineTargetElement = scope.selectedLineTargetElement;
                    }
                    x1 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x1"));
                    x2 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2"));
                    y1 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y1"));
                    y2 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2"));

                    if (scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2] != undefined) {
                        x_1 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].getAttribute("x1"));
                        y_1 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].getAttribute("y1"));
                        x_2 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].getAttribute("x2"));
                        y_2 = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].getAttribute("y2"));
                    }

                    if ((x1 - x2) >= -1 && (x1 - x2) <= 1 && (y_1 - y_2) > -1 && (y_1 - y_2) < 1) {
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("x2", x2 + dx);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("x2", x2 + dx);

                        if (scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2] != undefined) {
                            scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].setAttribute("x2", x2 + dx);
                            // for shadow line
                            scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[1].setAttribute("x2", x2 + dx);
                        }
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("y2", y2 + dy);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("y2", y2 + dy);

                        var xpos = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2"));
                        var ypos = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2"));
                        scope.isEdgeChanged = false;
                        edgePosition = getEdgePosition(y2, scope.ClonedLineTargetElement, changeEdgeDiff, true);
                        if (!scope.cursorPositionOnElement) {
                            scope.cursorPositionOnElement = edgePosition;
                        }
                        if (ypos <= scope.ClonedLineTargetElement.Top + scope.ClonedLineTargetElement.Height + 1 && ypos >= scope.ClonedLineTargetElement.Top - 1) {
                            if (xpos <= (scope.ClonedLineTargetElement.Left + scope.ClonedLineTargetElement.Width - edgeVariationDiff) && xpos >= (scope.ClonedLineTargetElement.Left + edgeVariationDiff)) {
                                scope.isEdgeChanged = false;
                                if (edgePosition && scope.cursorPositionOnElement != edgePosition) {
                                    scope.isEdgeChanged = true;
                                    scope.cursorPositionOnElement = "";
                                }
                            }
                            else {
                                scope.isEdgeChanged = true;
                            }
                        }
                        else if (xpos > scope.ClonedLineTargetElement.Left + scope.ClonedLineTargetElement.Width || xpos < scope.ClonedLineTargetElement.Left) {
                            scope.isEdgeChanged = true;
                        }
                        else {
                            scope.isEdgeChanged = true;
                        }
                    }
                    else if ((y1 - y2) >= -1 && (y1 - y2) <= 1 && (x_1 - x_2) > -1 && (x_1 - x_2) < 1) {
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("y2", (y2 + dy));
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("x2", (x2 + dx));
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].setAttribute("y1", (y1 + dy));
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("y2", (y2 + dy));
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("x2", (x2 + dx));
                        scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[1].setAttribute("y1", (y1 + dy));

                        if (scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2] != undefined) {
                            scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[0].setAttribute("y2", (y2 + dy));
                            // for shadow line
                            scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 2].childNodes[1].setAttribute("y2", (y2 + dy));
                        }

                        var xpos = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2"));
                        var ypos = parseFloat(scope.ClonedLineElement.childNodes[scope.ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2"));
                        scope.isEdgeChanged = false;
                        edgePosition = getEdgePosition(x2, scope.ClonedLineTargetElement, changeEdgeDiff, false);
                        if (!scope.cursorPositionOnElement) {
                            scope.cursorPositionOnElement = edgePosition;
                        }
                        if (xpos <= scope.ClonedLineTargetElement.Left + scope.ClonedLineTargetElement.Width + 1 && xpos >= scope.ClonedLineTargetElement.Left - 1) {
                            if (ypos <= scope.ClonedLineTargetElement.Top + scope.ClonedLineTargetElement.Height - edgeVariationDiff && ypos >= scope.ClonedLineTargetElement.Top + edgeVariationDiff) {
                                scope.isEdgeChanged = false;
                                if (edgePosition && scope.cursorPositionOnElement != edgePosition) {
                                    scope.isEdgeChanged = true;
                                    scope.cursorPositionOnElement = "";
                                }
                            }
                            else {
                                scope.isEdgeChanged = true;
                            }
                        }
                        else if (ypos > scope.ClonedLineTargetElement.Top + scope.ClonedLineTargetElement.Height || ypos < scope.ClonedLineTargetElement.Top) {
                            scope.isEdgeChanged = true;
                        }
                        else {
                            scope.isEdgeChanged = true;
                        }
                    }

                    if (scope.isEdgeChanged && scope.targetElementForLine != null) {
                        var source = scope.selectedLineSourceElement;
                        var sourcePoint = { Left: parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("x1")), Top: parseFloat(scope.ClonedLineElement.childNodes[0].childNodes[0].getAttribute("y1")) };
                        var aCurPos = { Left: evnt.offsetX, Top: evnt.offsetY };
                        var destination = scope.ClonedLineTargetElement;
                        scope._previousPosition = aCurPos;
                        if (scope.targetElementForLine == null) {
                            source = { Left: evnt.offsetX, Top: evnt.offsetY, Height: 10, Width: 10 };
                        }
                        var atargetPoint = UpdateTargetPoint(aCurPos, destination, source, scope);
                        if (atargetPoint != undefined) {
                            var Newpoints = GetEdgeWayPoints(source, destination, sourcePoint, atargetPoint);
                            GenerateClonedLine(Newpoints, scope.ClonedLineElement, scope);
                        }
                    }

                    if (scope.targetElementForLine != null) {
                        var intersectionPoint = GetIntersectionPoint(scope.ClonedLineElement, "Last", scope);
                        removeIntersectionPoint(scope);
                        generatePointAtintersectionOflineandElement(intersectionPoint, scope);
                    }
                    else {
                        removeIntersectionPoint(scope);
                    }
                }
                else if ((x1 - x2) >= -1 && (x1 - x2) <= 1) {
                    if ((scope.selectedlineindex != 0 && scope.selectedlineindex != scope.selectedLineEle.childNodes.length - 1)) {
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x2", x2 + dx);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", x1 + dx);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x2", x2 + dx);

                        if (scope.selectedlineindex != 0) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex - 1].childNodes[0].setAttribute("x2", x2 + dx);
                            //for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex - 1].childNodes[1].setAttribute("x2", x2 + dx);
                        }
                        if (scope.selectedlineindex != scope.ClonedLineElement.childNodes.length - 1) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].setAttribute("x1", x1 + dx);
                            //for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[1].setAttribute("x1", x1 + dx);
                        }
                    }
                }
                else if ((y1 - y2) >= -1 && (y1 - y2) <= 1) {
                    if ((scope.selectedlineindex != 0 && scope.selectedlineindex != scope.selectedLineEle.childNodes.length - 1)) {
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", y1 + dy);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y2", y2 + dy);
                        // for shadow line
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", y1 + dy);
                        scope.ClonedLineElement.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y2", y2 + dy);
                        if (scope.selectedlineindex != 0) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex - 1].childNodes[0].setAttribute("y2", y2 + dy);
                            // for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex - 1].childNodes[1].setAttribute("y2", y2 + dy);
                        }
                        if (scope.selectedlineindex != scope.ClonedLineElement.childNodes.length - 1) {
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[0].setAttribute("y1", y1 + dy);
                            // for shadow line
                            scope.ClonedLineElement.childNodes[scope.selectedlineindex + 1].childNodes[1].setAttribute("y1", y1 + dy);
                        }
                    }
                }
            }
        }

        scope.currentX = evnt.offsetX;
        scope.currentY = evnt.offsetY;
    } else {
        deselectlinegElementonMouseUp(evnt);
    }
}

//function isvalidEdgeornot(count, selectedlineelement) {
//    linecount = -1;
//    isvalid = false;
//    for (var i = 0; i < selectedlineelement.childNodes.length; i++) {
//        if (selectedlineelement.childNodes[i].localName != "title") {
//            linecount++;
//        }
//    }
//    if (linecount == count) {
//        isvalid = true;
//    }
//    return isvalid;
//}
function GetIntersectionPoint(ClonedLineElement, param, scope) {
    //var scope = getCurrentFileScope();
    var Element = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
    var Point = { Left: -20, Top: 0 };
    if (param == "First") {
        x1 = parseFloat(ClonedLineElement.childNodes[0].childNodes[0].getAttribute("x1"));
        y1 = parseFloat(ClonedLineElement.childNodes[0].childNodes[0].getAttribute("y1"));

        x2 = parseFloat(ClonedLineElement.childNodes[0].childNodes[0].getAttribute("x2"));
        y2 = parseFloat(ClonedLineElement.childNodes[0].childNodes[0].getAttribute("y2"));
        var isMiddle = false;
        if (y1 <= Element.Top + Element.Height && y1 >= Element.Top && x1 >= Element.Left && x1 <= Element.Left + Element.Width) {
            if ((y1 - y2) > -1 && (y1 - y2) < 1) {
                if (x2 > Element.Left + Element.Width && x1 < Element.Left + Element.Width) {
                    Point.Left = Element.Left + Element.Width;
                    Point.Top = y1;
                }
                else if (x2 < Element.Left && x1 > Element.Left) {
                    Point.Left = Element.Left;
                    Point.Top = y1;
                }
            }
            else if ((x1 - x2) > -1 && (x1 - x2) < 1) {
                if (y2 > Element.Top + Element.Height && y1 < Element.Top + Element.Height) {
                    Point.Left = x1;
                    Point.Top = Element.Top + Element.Height;
                }
                else if (y2 < Element.Top && y1 > Element.Top) {
                    Point.Left = x1;
                    Point.Top = Element.Top;
                }
            }
        }
    }

    else if (param == "Last") {
        x1 = parseFloat(ClonedLineElement.childNodes[ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x1"));
        y1 = parseFloat(ClonedLineElement.childNodes[ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y1"));

        x2 = parseFloat(ClonedLineElement.childNodes[ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("x2"));
        y2 = parseFloat(ClonedLineElement.childNodes[ClonedLineElement.childNodes.length - 1].childNodes[0].getAttribute("y2"));
        if (y2 <= Element.Top + Element.Height && y2 >= Element.Top && x2 <= Element.Left + Element.Width && x2 >= Element.Left) {
            if ((y1 - y2) > -1 && (y1 - y2) < 1) {
                if (x1 > Element.Left + Element.Width && x2 < Element.Left + Element.Width) {
                    Point.Left = Element.Left + Element.Width;
                    Point.Top = y2;
                }
                else if (x1 < Element.Left && x2 > Element.Left) {
                    Point.Left = Element.Left;
                    Point.Top = y2;
                }
            }
            else if ((x1 - x2) > -1 && (x1 - x2) < 1) {
                if (y1 > Element.Top + Element.Height && y2 < Element.Top + Element.Height) {
                    Point.Left = x2;
                    Point.Top = Element.Top + Element.Height;
                }
                else if (y1 < Element.Top && y2 > Element.Top) {
                    Point.Left = x2;
                    Point.Top = Element.Top;
                }
            }
        }
    }
    return Point;
}

function generatePointAtintersectionOflineandElement(point, scope) {
    var myCircle = document.createElementNS(scope.svgNS, "circle");
    myCircle.setAttributeNS(null, "id", "intersectionPoint");
    myCircle.setAttributeNS(null, "cx", point.Left);
    myCircle.setAttributeNS(null, "cy", point.Top);
    myCircle.setAttributeNS(null, "r", 3);
    myCircle.setAttribute("onmousedown", 'selectoncircle(evt)');
    myCircle.setAttributeNS(null, "fill", "red");
    myCircle.setAttributeNS(null, "stroke", "black");
    scope.svgElement.appendChild(myCircle);
}

function removeIntersectionPoint(scope) {
    for (var i = scope.svgElement.childNodes.length - 1; i > 0; i--) {
        if (scope.svgElement.childNodes[i].nodeName != "#text" && scope.svgElement.childNodes[i].nodeName != "#comment") {
            var ID = scope.svgElement.childNodes[i].getAttribute("id");
            if (ID == "intersectionPoint") {
                scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                i--;
            }
        }
    }
}

function GenerateClonedLine(Newpoints, ClonedLineElement, scope) {
    //var scope = getCurrentFileScope();
    var id = "";
    if (scope.ClonedLineElement) {
        id = scope.ClonedLineElement.childNodes[0].getAttribute("id");
    } else {
        id = ClonedLineElement.childNodes[0].getAttribute("id");
    }
    for (var i = 0; i < ClonedLineElement.childNodes.length; i++) {
        ClonedLineElement.removeChild(ClonedLineElement.childNodes[i]);
        i--;
    }

    for (var j = 0; j < Newpoints.length; j++) {
        if (j < Newpoints.length - 1) {
            var gline = document.createElementNS(scope.svgNS, "g");
            gline.setAttribute("id", id);
            gline.setAttribute("elementtype", "linegroup");
            var line = document.createElementNS(scope.svgNS, "line");
            line.setAttribute('id', id);
            line.setAttribute("x1", Newpoints[j].Left);
            line.setAttribute("y1", Newpoints[j].Top);
            line.setAttribute("x2", Newpoints[j + 1].Left);
            line.setAttribute("y2", Newpoints[j + 1].Top);

            if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "association"
                || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "messageFlow" || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "dataInputAssociation") {
                line.setAttribute("stroke-dasharray", "6,4");
                if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "messageFlow") {
                    ClonedLineElement.setAttribute("ondragover", 'SetDragOverLineElement(evt)');
                    ClonedLineElement.setAttribute("ondragleave", 'SetNullOnDragleaveLineElement(evt)');
                }
            }
            line.setAttributeNS(null, "stroke-width", 1);
            line.setAttributeNS(null, "stroke", "#dd4205");
            if (j == Newpoints.length - 2 || Newpoints.length == 2) {
                if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType != "association") {
                    var arrowMarker = "url(#arrow_" + scope.$root.currentopenfile.file.FileName + ")";
                    line.setAttribute("marker-end", arrowMarker);
                }
            }

            var lineshadow = document.createElementNS(scope.svgNS, "line");
            lineshadow.setAttribute('id', id);
            lineshadow.setAttribute("x1", Newpoints[j].Left);
            lineshadow.setAttribute("y1", Newpoints[j].Top);
            lineshadow.setAttribute("x2", Newpoints[j + 1].Left);
            lineshadow.setAttribute("y2", Newpoints[j + 1].Top);
            lineshadow.setAttribute("stroke-width", 15);
            //lineshadow.setAttribute("stroke-opacity", 0.1);
            if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "association"
                || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "messageFlow" || scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "dataInputAssociation") {
                line.setAttribute("stroke-dasharray", "6,4");
                if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType == "messageFlow") {
                    ClonedLineElement.setAttribute("ondragover", 'SetDragOverLineElement(evt)');
                    ClonedLineElement.setAttribute("ondragleave", 'SetNullOnDragleaveLineElement(evt)');
                }
            }
            lineshadow.setAttributeNS(null, "stroke", "#dd4205");
            lineshadow.setAttributeNS(null, "stroke-opacity", 0.1);
            gline.appendChild(line);
            gline.appendChild(lineshadow);
            ClonedLineElement.appendChild(gline);
        }
    }
}

function removeClonedLine(scope) {
    //var scope = getCurrentFileScope();
    for (var i = scope.svgElement.childNodes.length - 1; i > 0; i--) {
        if (scope.svgElement.childNodes[i].nodeName != "#text" && scope.svgElement.childNodes[i].nodeName != "#comment") {
            var ID = scope.svgElement.childNodes[i].getAttribute("id");
            if (ID == "ClonedLine") {
                scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                i--;
            }
        }
    }
}

function deselectlinegElementonMouseUp(evt) {
    var generateNewWayPoints = true;
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    scope.isMouseDown = false;
    var selectedLinePrevSourceElement = "";
    var selectedLinePrevTargetElement = "";
    if (scope.selectedLineEle) {
        selectedLinePrevSourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement, scope, "Element");
        selectedLinePrevTargetElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement, scope, "Element");
    }
    checkTitleIsPresentOrDeleteTitle(scope.selectedLineEle, "Delete");
    removeIntersectionPoint(scope);
    var prevWayPointsofLine = GetWayPointsFromLine(scope.selectedLineEle);
    if (scope.ClonedLineElement != null) {
        checkTitleIsPresentOrDeleteTitle(scope.ClonedLineElement, "Delete");
        var WaypointsFromClonedLine = GetWayPointsFromLine(scope.ClonedLineElement);
        if (scope.selectedlineindex != 0 && scope.selectedlineindex != scope.selectedlinegtagEle.parentNode.childNodes.length - 1) {
            scope.targetElementForLine = null;
        }

        if (scope.targetElementForLine != null) {
            var isValid = false;
            var objTargetElement = { Left: parseFloat(scope.targetElementForLine.getAttribute("x")), Top: parseFloat(scope.targetElementForLine.getAttribute("y")), Height: parseFloat(scope.targetElementForLine.getAttribute("height")), Width: parseFloat(scope.targetElementForLine.getAttribute("width")) };
            var Element = getElementBasedOnObject(objTargetElement, scope);
            // remove IoSpecification reference in task
            var elementType = scope.selectedLineEle.getAttribute("elementtype");
            if (elementType == "dataInputAssociation" || elementType == "dataOutputAssociation") {
                var dataAssociationId = "";
                if (scope.selectedlineindex == 0 || scope.selectedlineindex == scope.selectedlinegtagEle.parentNode.childNodes.length - 1 || (scope.selectedLineEle.childNodes.length == 1 && scope.selectedlineindex == 1)) {
                    var isValidNode = true;
                    if (scope.selectedlineindex == 0) {
                        isValidNode = CheckIsValidFlowNodeOrNot(Element, scope.selectedLineTargetElement);
                    } else {
                        isValidNode = CheckIsValidFlowNodeOrNot(scope.selectedLineSourceElement, Element);
                    }
                    if (isValidNode) {
                        if (scope.selectedLineSourceElement.ShapeType != "dataObjectReference" || scope.selectedLineSourceElement.ShapeType != "dataStoreReference") {
                            for (var i = 0; i < scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements.length; i++) {
                                if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements[i].Name == "sourceRef") {
                                    dataAssociationId = scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements[i].Value;
                                    break;
                                }
                            }
                            if (objTargetElement.Left != scope.selectedLineTargetElement.Left || objTargetElement.Top != scope.selectedLineTargetElement.Top || objTargetElement.Height != scope.selectedLineTargetElement.Height || objTargetElement.Width != scope.selectedLineTargetElement.Width) {
                                if (objTargetElement.Left != scope.selectedLineSourceElement.Left || objTargetElement.Top != scope.selectedLineSourceElement.Top || objTargetElement.Height != scope.selectedLineSourceElement.Height || objTargetElement.Width != scope.selectedLineSourceElement.Width) {
                                    RemoveDataAssociationIOspecificationFromtask(scope.selectedLineSourceElement, dataAssociationId, "output");
                                }
                            }
                        }
                        else if (scope.selectedLineTargetElement.ShapeType != "dataObjectReference" || scope.selectedLineTargetElement.ShapeType != "dataStoreReference") {
                            for (var i = 0; i < scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements.length; i++) {
                                if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements[i].Name == "targetRef") {
                                    dataAssociationId = scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel.Elements[i].Value;
                                    break;
                                }
                            }
                            if (objTargetElement.Left != scope.selectedLineSourceElement.Left || objTargetElement.Top != scope.selectedLineSourceElement.Top || objTargetElement.Height != scope.selectedLineSourceElement.Height || objTargetElement.Width != scope.selectedLineSourceElement.Width) {
                                if (objTargetElement.Left != scope.selectedLineTargetElement.Left || objTargetElement.Top != scope.selectedLineTargetElement.Top || objTargetElement.Height != scope.selectedLineTargetElement.Height || objTargetElement.Width != scope.selectedLineTargetElement.Width) {
                                    RemoveDataAssociationIOspecificationFromtask(scope.selectedLineTargetElement, dataAssociationId, "input");
                                }
                            }
                        }
                    }
                }
            }

            if (scope.selectedlineindex == 0) {
                var isValidNode = CheckIsValidFlowNodeOrNot(Element, scope.selectedLineTargetElement);
                if (isValidNode) {
                    if (objTargetElement.Left != scope.selectedLineTargetElement.Left || objTargetElement.Top != scope.selectedLineTargetElement.Top || objTargetElement.Height != scope.selectedLineTargetElement.Height || objTargetElement.Width != scope.selectedLineTargetElement.Width) {
                        if (objTargetElement.Left != scope.selectedLineSourceElement.Left || objTargetElement.Top != scope.selectedLineSourceElement.Top || objTargetElement.Height != scope.selectedLineSourceElement.Height || objTargetElement.Width != scope.selectedLineSourceElement.Width) {
                            if (Element != null && scope.selectedLineSourceElement.ShapeType != "textAnnotation" && scope.selectedLineTargetElement.ShapeType != "textAnnotation") {
                                var SourceElementId = Element.Id;
                                var TargetElementId = scope.selectedLineTargetElement.Id;
                                var bool = getNodeAlreadyConnected(SourceElementId, TargetElementId, scope);
                                if (bool) {
                                    alert("Nodes Already Connected");
                                }
                                else {
                                    isValid = true;
                                    //in shapemodel have to change ID
                                    //var id = generateUUID();
                                    //scope.ClonedLineElement.childNodes[0].setAttribute("id", id);
                                    var prevSelectedLineSourceElement = scope.selectedLineSourceElement;
                                    scope.selectedLineSourceElement = Element;
                                    var id = scope.ClonedLineElement.childNodes[0].getAttribute("id");
                                    if (scope.selectedLineSourceElement.processRef == scope.selectedLineTargetElement.processRef && scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType != "association" && scope.selectedLineSourceElement.ShapeType != "dataStoreReference" && scope.selectedLineTargetElement.ShapeType != "dataStoreReference" && scope.selectedLineSourceElement.ShapeType != "dataObjectReference" && scope.selectedLineTargetElement.ShapeType != "dataObjectReference") {
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "sequenceFlow";
                                        if (Element.ShapeType != "exclusiveGateway" && Element.ShapeType != "inclusiveGateway") {
                                            scope.selectedLineEle.setAttribute("elementtype", "sequenceFlow");
                                            EdgeShapeModel = {
                                                dictAttributes: {
                                                    id: id, sourceRef: Element.Id, name: scope.selectedShape.Text,
                                                    targetRef: scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement
                                                }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: ""
                                            };
                                        }
                                        else {
                                            scope.selectedLineEle.setAttribute("elementtype", "GatewayExpression");
                                            EdgeShapeModel = {
                                                dictAttributes: { id: id, sourceRef: Element.Id, targetRef: scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement, name: scope.selectedShape.Text },
                                                Elements: [{
                                                    dictAttributes: {}, Elements: [{
                                                        dictAttributes: {}, Elements: [{ dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "leftside", prefix: "", IsValueInCDATAFormat: false, Value: "" },
                                                        { dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "rightside", prefix: "", IsValueInCDATAFormat: false, Value: "" }], Children: [], Name: "conditionExpression", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                                    },
                                                    { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [], Name: "extensionElements", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                                }], Children: [], Name: "sequenceFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                            };
                                        }
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = EdgeShapeModel;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.selectedLineSourceElement.processRef;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else if (scope.selectedLineSourceElement.ShapeType == "dataStoreReference" || scope.selectedLineTargetElement.ShapeType == "dataStoreReference" || scope.selectedLineSourceElement.ShapeType == "dataObjectReference" || scope.selectedLineTargetElement.ShapeType == "dataObjectReference") {
                                        setDataOutPutAndDataInPutAssociationToShapeModel(scope.selectedLineSourceElement, scope.selectedLineTargetElement, id, scope);
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.selectedLineSourceElement.processRef;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType != "association" && Element.ShapeType != "exclusiveGateway" && Element.ShapeType != "inclusiveGateway" && scope.selectedLineTargetElement.ShapeType != "exclusiveGateway" && scope.selectedLineTargetElement.ShapeType != "inclusiveGateway"
                                        && Element.ShapeType != "eventBasedGateway" && Element.ShapeType != "parallelGateway" && scope.selectedLineTargetElement.ShapeType != "eventBasedGateway" && scope.selectedLineTargetElement.ShapeType != "parallelGateway") {
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "messageFlow";
                                        scope.selectedLineEle.setAttribute("elementtype", "messageFlow");
                                        var MessageFlowModel = {
                                            dictAttributes: { id: id, sourceRef: scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement, targetRef: scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement, name: scope.selectedShape.Text },
                                            Elements: [{
                                                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" },
                                                { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [],
                                                Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                                            }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                        };
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = MessageFlowModel;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.objData.CollaborationId;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else {
                                        if (Element.ShapeType == "exclusiveGateway" || Element.ShapeType == "inclusiveGateway" || scope.selectedLineTargetElement.ShapeType == "exclusiveGateway" || scope.selectedLineTargetElement.ShapeType == "inclusiveGateway"
                                            || Element.ShapeType == "eventBasedGateway" || Element.ShapeType == "parallelGateway" || scope.selectedLineTargetElement.ShapeType == "eventBasedGateway" || scope.selectedLineTargetElement.ShapeType == "parallelGateway") {
                                            isValid = false;
                                            scope.selectedLineSourceElement = prevSelectedLineSourceElement;
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            isValid = true;
                        }
                    }
                }
            }
            else if (scope.selectedlineindex == scope.selectedlinegtagEle.parentNode.childNodes.length - 1 || (scope.selectedLineEle.childNodes.length == 1 && scope.selectedlineindex == 1)) {
                var isValidNode = CheckIsValidFlowNodeOrNot(scope.selectedLineSourceElement, Element);
                if (isValidNode) {
                    if (objTargetElement.Left != scope.selectedLineSourceElement.Left || objTargetElement.Top != scope.selectedLineSourceElement.Top || objTargetElement.Height != scope.selectedLineSourceElement.Height || objTargetElement.Width != scope.selectedLineSourceElement.Width) {
                        if (objTargetElement.Left != scope.selectedLineTargetElement.Left || objTargetElement.Top != scope.selectedLineTargetElement.Top || objTargetElement.Height != scope.selectedLineTargetElement.Height || objTargetElement.Width != scope.selectedLineTargetElement.Width) {
                            if (Element != null && scope.selectedLineSourceElement.ShapeType != "textAnnotation" && scope.selectedLineTargetElement.ShapeType != "textAnnotation") {
                                var SourceElementId = scope.selectedLineSourceElement.Id;
                                var TargetElementId = Element.Id;
                                var bool = getNodeAlreadyConnected(SourceElementId, TargetElementId, scope);
                                if (bool) {
                                    alert("Nodes Already Connected");
                                }
                                else {
                                    isValid = true;
                                    //var id = generateUUID();
                                    //scope.ClonedLineElement.childNodes[0].setAttribute("id", id);
                                    var id = scope.ClonedLineElement.childNodes[0].getAttribute("id");
                                    var prevSelectedLineTargetElement = scope.selectedLineTargetElement;
                                    scope.selectedLineTargetElement = Element;

                                    if (scope.selectedLineSourceElement.processRef == scope.selectedLineTargetElement.processRef && scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType != "association" && scope.selectedLineSourceElement.ShapeType != "dataStoreReference" && scope.selectedLineTargetElement.ShapeType != "dataStoreReference" && scope.selectedLineSourceElement.ShapeType != "dataObjectReference" && scope.selectedLineTargetElement.ShapeType != "dataObjectReference") {
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "sequenceFlow";
                                        if (Element.ShapeType != "exclusiveGateway" && Element.ShapeType != "inclusiveGateway") {
                                            scope.selectedLineEle.setAttribute("elementtype", "sequenceFlow");
                                            EdgeShapeModel = {
                                                dictAttributes: {
                                                    id: id, sourceRef: scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement,
                                                    targetRef: Element.Id, name: scope.selectedShape.Text
                                                }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: ""
                                            };
                                        }
                                        else {
                                            scope.selectedLineEle.setAttribute("elementtype", "GatewayExpression");
                                            //in shapemodel have to change ID
                                            EdgeShapeModel = {
                                                dictAttributes: { id: id, sourceRef: scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement, targetRef: Element.Id, name: scope.selectedShape.Text },
                                                Elements: [{
                                                    dictAttributes: {}, Elements: [{
                                                        dictAttributes: {}, Elements: [{ dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "leftside", prefix: "", IsValueInCDATAFormat: false, Value: "" },
                                                        { dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "rightside", prefix: "", IsValueInCDATAFormat: false, Value: "" }], Children: [], Name: "conditionExpression", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                                    },
                                                    { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [], Name: "extensionElements", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                                }], Children: [], Name: "sequenceFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                            };
                                        }
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = EdgeShapeModel;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.selectedLineSourceElement.processRef;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else if (scope.selectedLineSourceElement.ShapeType == "dataStoreReference" || scope.selectedLineTargetElement.ShapeType == "dataStoreReference" || scope.selectedLineSourceElement.ShapeType == "dataObjectReference" || scope.selectedLineTargetElement.ShapeType == "dataObjectReference") {
                                        setDataOutPutAndDataInPutAssociationToShapeModel(scope.selectedLineSourceElement, scope.selectedLineTargetElement, id, scope);
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.selectedLineSourceElement.processRef;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else if (scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType != "association" && Element.ShapeType != "exclusiveGateway" && Element.ShapeType != "inclusiveGateway" && scope.selectedLineSourceElement.ShapeType != "exclusiveGateway" && scope.selectedLineSourceElement.ShapeType != "inclusiveGateway"
                                        && Element.ShapeType != "eventBasedGateway" && Element.ShapeType != "parallelGateway" && scope.selectedLineSourceElement.ShapeType != "eventBasedGateway" && scope.selectedLineSourceElement.ShapeType != "parallelGateway") {
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "messageFlow";
                                        scope.selectedLineEle.setAttribute("elementtype", "messageFlow");
                                        var MessageFlowModel = {
                                            dictAttributes: { id: id, sourceRef: scope.objData.lstShapes[scope.indexOfSelectedLine].SourceElement, targetRef: scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement, name: scope.selectedShape.Text },
                                            Elements: [{
                                                dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" },
                                                { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [],
                                                Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                                            }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                        };
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = MessageFlowModel;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].processRef = scope.objData.CollaborationId;
                                        scope.objData.lstShapes[scope.indexOfSelectedLine].TargetElement = Element.Id;
                                        //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = id;
                                    }
                                    else {
                                        if (Element.ShapeType == "exclusiveGateway" || Element.ShapeType == "inclusiveGateway" || scope.selectedLineSourceElement.ShapeType == "exclusiveGateway" || scope.selectedLineSourceElement.ShapeType == "inclusiveGateway"
                                            || Element.ShapeType == "eventBasedGateway" || Element.ShapeType == "parallelGateway" || scope.selectedLineSourceElement.ShapeType == "eventBasedGateway" || scope.selectedLineSourceElement.ShapeType == "parallelGateway") {
                                            isValid = false;
                                            scope.selectedLineTargetElement = prevSelectedLineTargetElement;
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            isValid = true;
                        }
                    }
                }
            }

            if (isValid) {
                // if the edge is added from other element to gateway change direction status to that gateway
                if (scope.selectedLineTargetElement.ShapeType == "inclusiveGateway" || scope.selectedLineTargetElement.ShapeType == "parallelGateway") {
                    scope.ChangeDirectionifElementisGateWay(scope.selectedLineSourceElement, scope.selectedLineTargetElement);
                } else if (scope.selectedLineSourceElement.ShapeType == "inclusiveGateway" || scope.selectedLineSourceElement.ShapeType == "parallelGateway") {
                    scope.ChangeDirectionifElementisGateWay(scope.selectedLineSourceElement, scope.selectedLineTargetElement);
                }

                //In this Scenario this function is used for generating the original line 
                GenerateClonedLine(WaypointsFromClonedLine, scope.selectedLineEle, scope);

                // if the Edge is changed from gateway element change direction status to that gateway
                if (selectedLinePrevTargetElement.ShapeType == "inclusiveGateway" || selectedLinePrevTargetElement.ShapeType == "parallelGateway") {
                    scope.ChangeDirectionifElementisGateWay(selectedLinePrevSourceElement, selectedLinePrevTargetElement);
                } else if (selectedLinePrevSourceElement.ShapeType == "inclusiveGateway" || selectedLinePrevSourceElement.ShapeType == "parallelGateway") {
                    scope.ChangeDirectionifElementisGateWay(selectedLinePrevSourceElement, selectedLinePrevTargetElement);
                }
            }
        }
        else if (scope.targetElementForLine == null && scope.selectedlineindex != 0 && scope.selectedlineindex != scope.selectedlinegtagEle.parentNode.childNodes.length - 1) {
            //In this Scenario this function is used for generating the original line 
            GenerateClonedLine(WaypointsFromClonedLine, scope.selectedLineEle, scope);
        }
    }
    scope.selectedLineEle.removeAttributeNS(null, "style");

    removeClonedLine(scope);
    if (isValid) {
        var isConditionSatisfied = false;
        if (scope.selectedlineindex == 0) {
            x1 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x1"));
            y1 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y1"));

            x2 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("x2"));
            y2 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].getAttribute("y2"));
            var isMiddle = false;
            if (y1 <= scope.selectedLineSourceElement.Top + scope.selectedLineSourceElement.Height && y1 >= scope.selectedLineSourceElement.Top && x1 >= scope.selectedLineSourceElement.Left && x1 <= scope.selectedLineSourceElement.Left + scope.selectedLineSourceElement.Width) {
                if ((y1 - y2) > -1 && (y1 - y2) < 1) {
                    if (x2 > scope.selectedLineSourceElement.Left + scope.selectedLineSourceElement.Width && x1 < scope.selectedLineSourceElement.Left + scope.selectedLineSourceElement.Width) {
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", scope.selectedLineSourceElement.Left + scope.selectedLineSourceElement.Width);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", y1);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", scope.selectedLineSourceElement.Left + scope.selectedLineSourceElement.Width);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", y1);
                        isConditionSatisfied = true;
                    }
                    else if (x2 < scope.selectedLineSourceElement.Left && x1 > scope.selectedLineSourceElement.Left) {
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", scope.selectedLineSourceElement.Left);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", y1);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", scope.selectedLineSourceElement.Left);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", y1);
                        isConditionSatisfied = true;
                    }
                }
                else if ((x1 - x2) > -1 && (x1 - x2) < 1) {
                    if (y2 > scope.selectedLineSourceElement.Top + scope.selectedLineSourceElement.Height && y1 < scope.selectedLineSourceElement.Top + scope.selectedLineSourceElement.Height) {
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", x1);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", scope.selectedLineSourceElement.Top + scope.selectedLineSourceElement.Height);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", x1);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", scope.selectedLineSourceElement.Top + scope.selectedLineSourceElement.Height);
                        isConditionSatisfied = true;
                    }
                    else if (y2 < scope.selectedLineSourceElement.Top && y1 > scope.selectedLineSourceElement.Top) {
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("x1", x1);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[0].setAttribute("y1", scope.selectedLineSourceElement.Top);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("x1", x1);
                        scope.selectedLineEle.childNodes[scope.selectedlineindex].childNodes[1].setAttribute("y1", scope.selectedLineSourceElement.Top);
                        isConditionSatisfied = true;
                    }
                }
            }
        }
        else if (scope.selectedlineindex == scope.selectedlinegtagEle.parentNode.childNodes.length - 1) {
            x1 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].getAttribute("x1"));
            y1 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].getAttribute("y1"));

            x2 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].getAttribute("x2"));
            y2 = parseFloat(scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].getAttribute("y2"));
            var isMiddle = false;
            if (y2 <= scope.selectedLineTargetElement.Top + scope.selectedLineTargetElement.Height && y2 >= scope.selectedLineTargetElement.Top && x2 <= scope.selectedLineTargetElement.Left + scope.selectedLineTargetElement.Width && x2 >= scope.selectedLineTargetElement.Left) {
                if ((y1 - y2) > -1 && (y1 - y2) < 1) {
                    if (x1 > scope.selectedLineTargetElement.Left + scope.selectedLineTargetElement.Width && x2 < scope.selectedLineTargetElement.Left + scope.selectedLineTargetElement.Width) {
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("x2", scope.selectedLineTargetElement.Left + scope.selectedLineTargetElement.Width);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("y2", y2);
                        //for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("x2", scope.selectedLineTargetElement.Left + scope.selectedLineTargetElement.Width);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("y2", y2);
                        isConditionSatisfied = true;
                    }
                    else if (x1 < scope.selectedLineTargetElement.Left && x2 > scope.selectedLineTargetElement.Left) {
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("x2", scope.selectedLineTargetElement.Left);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("y2", y2);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("x2", scope.selectedLineTargetElement.Left);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("y2", y2);
                        isConditionSatisfied = true;
                    }
                }
                else if ((x1 - x2) > -1 && (x1 - x2) < 1) {
                    if (y1 > scope.selectedLineTargetElement.Top + scope.selectedLineTargetElement.Height && y2 < scope.selectedLineTargetElement.Top + scope.selectedLineTargetElement.Height) {
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("x2", x2);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("y2", scope.selectedLineTargetElement.Top + scope.selectedLineTargetElement.Height);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("x2", x2);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("y2", scope.selectedLineTargetElement.Top + scope.selectedLineTargetElement.Height);
                        isConditionSatisfied = true;
                    }
                    else if (y1 < scope.selectedLineTargetElement.Top && y2 > scope.selectedLineTargetElement.Top) {
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("x2", x2);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[0].setAttribute("y2", scope.selectedLineTargetElement.Top);
                        // for shadow line
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("x2", x2);
                        scope.selectedLineEle.childNodes[scope.selectedLineEle.childNodes.length - 1].childNodes[1].setAttribute("y2", scope.selectedLineTargetElement.Top);
                        isConditionSatisfied = true;
                    }
                }
            }
        }
        if (!isConditionSatisfied) {
            //In this Scenario this function is used for generating the original line 
            GenerateClonedLine(prevWayPointsofLine, scope.selectedLineEle, scope);
        }
    }

    // upadting the objData
    if (scope.ClonedLineElement != null) {
        var updatedWayPoints = GetWayPointsFromLine(scope.selectedLineEle);
        updatedWayPoints = setShapeNameToWayPoints(updatedWayPoints);
        if (scope.selectedLineEle.getAttribute("elementtype") == "association") {
            var length = updatedWayPoints.length;
            if (updatedWayPoints.length > 2) {
                var numberofElementsToRemove = length - 2;
                updatedWayPoints.splice(1, numberofElementsToRemove);
                //In this Scenario this function is used for generating the original line 
                GenerateClonedLine(updatedWayPoints, scope.selectedLineEle, scope);
            }
        }
        scope.objData.lstShapes[scope.indexOfSelectedLine].lstWayPoints = updatedWayPoints;
    }
    scope.ClonedLineElement = null;
    scope.$apply(function () {
        scope.DraggableElement = null;
    });
    //scope.objData.lstShapes[scope.indexOfSelectedLine].Id = scope.selectedLineEle.childNodes[0].getAttribute("id");
    selectedlineElement(null, scope);
    scope.svgElement.removeAttributeNS(null, "onmouseup");
    scope.svgElement.removeAttributeNS(null, "onmouseout");
    scope.svgElement.removeAttributeNS(null, "onmousemove");
    scope.selectedlinegtagEle = 0;
}

function RemoveDataAssociationIOspecificationFromtask(TaskElement, Id, param) {
    for (var i = 0; i < TaskElement.ShapeModel.Elements.length; i++) {
        if (TaskElement.ShapeModel.Elements[i].Name == "ioSpecification") {
            for (var j = 0; j < TaskElement.ShapeModel.Elements[i].Elements.length; j++) {
                if (param == "output") {
                    if (TaskElement.ShapeModel.Elements[i].Elements[j].Name == "outputSet") {
                        for (var k = 0; k < TaskElement.ShapeModel.Elements[i].Elements[j].Elements.length; k++) {
                            if (TaskElement.ShapeModel.Elements[i].Elements[j].Elements[k].Value == Id) {
                                TaskElement.ShapeModel.Elements[i].Elements[j].Elements.splice(k, 1);
                                k--;
                            }
                        }
                    }
                    else {
                        if (TaskElement.ShapeModel.Elements[i].Elements[j].dictAttributes.id == Id) {
                            TaskElement.ShapeModel.Elements[i].Elements.splice(j, 1);
                            j--;
                        }
                    }
                } else {
                    if (TaskElement.ShapeModel.Elements[i].Elements[j].Name == "inputSet") {
                        for (var k = 0; k < TaskElement.ShapeModel.Elements[i].Elements[j].Elements.length; k++) {
                            if (TaskElement.ShapeModel.Elements[i].Elements[j].Elements[k].Value == Id) {
                                TaskElement.ShapeModel.Elements[i].Elements[j].Elements.splice(k, 1);
                                k--;
                            }
                        }
                    }
                    else {
                        if (TaskElement.ShapeModel.Elements[i].Elements[j].dictAttributes.id == Id) {
                            TaskElement.ShapeModel.Elements[i].Elements.splice(j, 1);
                            j--;
                        }
                    }
                }
            }
        }
    }
}

function setDataOutPutAndDataInPutAssociationToShapeModel(srcElement, tarElement, Edgeid, scope) {
    var bool = IsValidShapeForAssociation(srcElement, tarElement);
    if (bool) {
        if (srcElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference") {
            scope.selectedLineEle.setAttribute("elementtype", "dataInputAssociation");
            scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "dataInputAssociation";
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
            var isIoSpecModelIsFound = false;
            for (var k = 0; k < tarElement.ShapeModel.Elements.length; k++) {
                if (tarElement.ShapeModel.Elements[k].Name == "ioSpecification") {
                    var inputmodel = {
                        dictAttributes: { id: IOsepecificationID, isCollection: "false" }, Elements: [], Children: [], Name: "dataInput", prefix: "",
                        IsValueInCDATAFormat: false, Value: ""
                    };
                    var inputsetmodel = {
                        dictAttributes: {}, Elements: [{
                            dictAttributes: {}, Elements: [], Children: [], Name: "dataInputRefs", prefix: "", IsValueInCDATAFormat: false,
                            Value: IOsepecificationID
                        }], Children: [], Name: "inputSet", prefix: "", IsValueInCDATAFormat: false, Value: ""
                    };
                    var isInputSetFound = false;
                    for (var m = 0; m < tarElement.ShapeModel.Elements[k].Elements.length; m++) {
                        if (tarElement.ShapeModel.Elements[k].Elements[m].Name == "inputSet") {
                            var dataInputRefModel = {
                                dictAttributes: {}, Elements: [], Children: [], Name: "dataInputRefs", prefix: "", IsValueInCDATAFormat: false,
                                Value: IOsepecificationID
                            };
                            tarElement.ShapeModel.Elements[k].Elements[m].Elements.push(dataInputRefModel);
                            isInputSetFound = true;
                        }
                    }
                    tarElement.ShapeModel.Elements[k].Elements.push(inputmodel);
                    if (!isInputSetFound) {
                        tarElement.ShapeModel.Elements[k].Elements.push(inputsetmodel);
                    }
                    isIoSpecModelIsFound = true;
                }
            }
            if (!isIoSpecModelIsFound) {
                tarElement.ShapeModel.Elements.push(IOsepecificationModel);
            }
            var datainPutAssociationID = generateUUID();
            var datainPutAssociationModel = {
                dictAttributes: { id: dataOutPutAssociationID },
                Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "sourceRef", prefix: "", IsValueInCDATAFormat: false, Value: tarElement.Id },
                { dictAttributes: {}, Elements: [], Children: [], Name: "targetRef", prefix: "", IsValueInCDATAFormat: false, Value: IOsepecificationID }],
                Children: [], Name: "dataInputAssociation", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = datainPutAssociationModel;
        } else if (tarElement.ShapeType == "dataStoreReference" || tarElement.ShapeType == "dataObjectReference") {
            scope.selectedLineEle.setAttribute("elementtype", "dataOutputAssociation");
            scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeType = "dataOutputAssociation";
            var IOsepecificationID = generateUUID();
            var IOsepecificationModel = {
                dictAttributes: {}, Elements: [{
                    dictAttributes: { id: IOsepecificationID, isCollection: "false" }, Elements: [], Children: [], Name: "dataOutput", prefix: "",
                    IsValueInCDATAFormat: false, Value: ""
                }, {
                    dictAttributes: {}, Elements: [{
                        dictAttributes: {}, Elements: [], Children: [], Name: "dataOutputRefs", prefix: "", IsValueInCDATAFormat: false,
                        Value: IOsepecificationID
                    }], Children: [], Name: "outputSet", prefix: "", IsValueInCDATAFormat: false, Value: ""
                }], Children: [], Name: "ioSpecification", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };

            var isIoSpecModelIsFound = false;
            for (var k = 0; k < srcElement.ShapeModel.Elements.length; k++) {
                if (srcElement.ShapeModel.Elements[k].Name == "ioSpecification") {
                    var outputmodel = {
                        dictAttributes: { id: IOsepecificationID, isCollection: "false" }, Elements: [], Children: [], Name: "dataOutput", prefix: "",
                        IsValueInCDATAFormat: false, Value: ""
                    };
                    var outputsetmodel = {
                        dictAttributes: {}, Elements: [{
                            dictAttributes: {}, Elements: [], Children: [], Name: "dataOutputRefs", prefix: "", IsValueInCDATAFormat: false,
                            Value: IOsepecificationID
                        }], Children: [], Name: "outputSet", prefix: "", IsValueInCDATAFormat: false, Value: ""
                    };
                    var isOutputSetFound = false;
                    for (var m = 0; m < srcElement.ShapeModel.Elements[k].Elements.length; m++) {
                        if (srcElement.ShapeModel.Elements[k].Elements[m].Name == "outputSet") {
                            var dataOutputRefModel = {
                                dictAttributes: {}, Elements: [], Children: [], Name: "dataOutputRefs", prefix: "", IsValueInCDATAFormat: false,
                                Value: IOsepecificationID
                            };
                            srcElement.ShapeModel.Elements[k].Elements[m].Elements.push(dataOutputRefModel);
                            isOutputSetFound = true;
                        }
                    }

                    srcElement.ShapeModel.Elements[k].Elements.push(outputmodel);
                    if (!isOutputSetFound) {
                        srcElement.ShapeModel.Elements[k].Elements.push(outputsetmodel);
                    }
                    isIoSpecModelIsFound = true;
                }
            }
            if (!isIoSpecModelIsFound) {
                srcElement.ShapeModel.Elements.push(IOsepecificationModel);
            }

            var dataOutPutAssociationID = generateUUID();
            var dataOutPutAssociationModel = {
                dictAttributes: { id: dataOutPutAssociationID },
                Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "sourceRef", prefix: "", IsValueInCDATAFormat: false, Value: IOsepecificationID },
                { dictAttributes: {}, Elements: [], Children: [], Name: "targetRef", prefix: "", IsValueInCDATAFormat: false, Value: srcElement.Id }],
                Children: [], Name: "dataOutputAssociation", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            scope.objData.lstShapes[scope.indexOfSelectedLine].ShapeModel = dataOutPutAssociationModel;
        }
    }
}

function getNodeAlreadyConnected(SourceElementId, TargetElementId, scope) {
    //var scope = getCurrentFileScope();
    var isFound = false;
    for (var j = 0; j < scope.objData.lstShapes.length; j++) {
        if (scope.objData.lstShapes[j].ShapeType == "sequenceFlow" || scope.objData.lstShapes[j].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[j].ShapeType == "association" || scope.objData.lstShapes[j].ShapeType == "messageFlow" || scope.objData.lstShapes[j].ShapeType == "dataInputAssociation") {
            if (scope.objData.lstShapes[j].SourceElement == SourceElementId && scope.objData.lstShapes[j].TargetElement == TargetElementId) {
                isFound = true;
                break;
            }
        }
    }
    return isFound;
}

function getElementBasedOnObject(objElement, scope) {
    //var scope = getCurrentFileScope();
    var Element = null;
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].Left == objElement.Left && scope.objData.lstShapes[i].Top == objElement.Top && scope.objData.lstShapes[i].Height == objElement.Height && scope.objData.lstShapes[i].Width == objElement.Width) {
            Element = scope.objData.lstShapes[i];
            break;
        }
    }
    return Element;
}

function GetWayPointsFromLine(LineElement) {
    var waypoints = [];
    for (var i = 0; i < LineElement.childNodes.length; i++) {
        waypoint = { Left: parseFloat(LineElement.childNodes[i].childNodes[0].getAttribute("x1")), Top: parseFloat(LineElement.childNodes[i].childNodes[0].getAttribute("y1")) };
        waypoints.push(waypoint);
        if (i == LineElement.childNodes.length - 1) {
            waypoint = { Left: parseFloat(LineElement.childNodes[i].childNodes[0].getAttribute("x2")), Top: parseFloat(LineElement.childNodes[i].childNodes[0].getAttribute("y2")) };
            waypoints.push(waypoint);
        }
    }

    return waypoints;
}

function setResizeraroudtext(evt) {
    scope = angular.element(evt.currentTarget).scope();
    if (!scope.IsVersionDisplay && scope.activeTab != "EXECUTION") {
        scope.$apply(function () {
            scope.selectedText = evt.currentTarget;
            var id = scope.selectedText.getAttribute('elementid');
            removeSelection("NewElement", scope);
            removeRearrangingLinePoints(scope);
            removeResizerAroundElement(scope);
            removeSelectionForLane(scope);
            if (scope.selectedLineEle && scope.activeTab == "MAP") {
                setColorToSelectedLine(scope.selectedLineEle, "remove");
            }
            var Element = GetElementProcessIdorElementBasedOnIdorDeleteElement(id, scope, "Element");
            if (Element) {
                scope.selectedShapeofText = Element;
                scope.selectedShape = undefined;
                scope.selectedLineEle = null;
                scope.OnSelectLeftBPMTab('Toolbox');
            }
            GenerateBoundsDynamicallyToText(scope.selectedText, true, scope);
            if (evt && evt.stopPropagation) {
                evt.stopPropagation();
            }
            scope.isresizerRemoved = false;
            scope.svgElement.setAttributeNS(null, "onmouseup", "deselectTextElement(evt,this)");
            scope.svgElement.setAttributeNS(null, "onmousemove", "moveTextElement(evt)");
            scope.currentX = evt.offsetX;
            scope.currentY = evt.offsetY;
        });
    }
}

function moveTextElement(evt) {
    var scope = angular.element(evt.currentTarget).scope();
    var dx = evt.offsetX - scope.currentX;
    var dy = evt.offsetY - scope.currentY;
    if (scope && evt.buttons == 1 && scope.selectedShapeofText && scope.selectedText) {
        if ((scope.currentX != evt.offsetX || scope.currentY != evt.offsetY)) {
            if (!scope.isresizerRemoved) {
                removeResizerAroundElement(scope);
                scope.isresizerRemoved = true;
            }
            var item = scope.selectedText;
            item.setAttribute('x', parseFloat(item.getAttribute('x')) + dx);
            item.setAttribute('y', parseFloat(item.getAttribute('y')) + dy);
            for (var k = 0; k < $(item).children().length; k++) {
                var itm = $(item).children()[k];
                itm.setAttribute('x', parseFloat(itm.getAttribute('x')) + dx);
            }

            scope.currentX = evt.offsetX;
            scope.currentY = evt.offsetY;
        }
    } else {
        deselectTextElement(evt);
    }
}



function GenerateBoundsDynamicallyToText(objselectedElement, isCloned, scope) {
    // resizer 
    var x = 0;
    var y = 0;
    var Height = 0;
    var Width = 0;
    x = parseFloat(objselectedElement.getAttribute("x")) - 5;
    y = parseFloat(objselectedElement.getAttribute("y")) - 5;
    Height = parseFloat(objselectedElement.getAttribute("height")) + 10;
    Width = parseFloat(objselectedElement.getAttribute("width")) + 10;
    //var scope = getCurrentFileScope();
    //removeResizerAroundElement();
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttributeNS(null, "id", "resizergroup");
    g.setAttributeNS(null, "class", "layer-resizers");



    if (objselectedElement.getAttribute("id") != "lane" && objselectedElement.getAttribute("id") != "message" && objselectedElement.getAttribute('id') != "endEvent" && objselectedElement.getAttribute('id') != "startEvent" && objselectedElement.getAttribute('id') != "intermediateCatchEvent"
        && objselectedElement.getAttribute('id') != "exclusiveGateway" && objselectedElement.getAttribute('id') != "inclusiveGateway" && objselectedElement.getAttribute('id') != "parallelGateway" && objselectedElement.getAttribute('id') != "eventBasedGateway" && objselectedElement.getAttribute('id') != "dataStoreReference" && objselectedElement.getAttribute('id') != "dataObjectReference") {
        var gchildnw = document.createElementNS(scope.svgNS, "g");
        gchildnw.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-nw");
        gchildnw.setAttributeNS(null, "id", "nw");
        gchildnw.setAttribute("onmousedown", 'selectedResizergtagText(evt)');

        var rectanglenw = document.createElementNS(scope.svgNS, "rect");
        rectanglenw.setAttributeNS(null, "id", "rectanglenw");
        rectanglenw.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglenw.setAttributeNS(null, "x", x - 3);
        rectanglenw.setAttributeNS(null, "y", y - 3);
        rectanglenw.setAttributeNS(null, "height", 6);
        rectanglenw.setAttributeNS(null, "width", 6);
        rectanglenw.setAttributeNS(null, "fill", "white");
        rectanglenw.setAttributeNS(null, "stroke", "none");
        gchildnw.appendChild(rectanglenw);

        var rectangle2ndnw = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndnw.setAttributeNS(null, "id", "rectanglenw");
        rectangle2ndnw.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndnw.setAttributeNS(null, "x", x - 10);
        rectangle2ndnw.setAttributeNS(null, "y", y - 10);
        rectangle2ndnw.setAttributeNS(null, "height", 20);
        rectangle2ndnw.setAttributeNS(null, "width", 20);
        rectangle2ndnw.setAttributeNS(null, "fill", "white");
        rectangle2ndnw.setAttributeNS(null, "stroke", "none");
        gchildnw.appendChild(rectangle2ndnw);
        g.appendChild(gchildnw);

        var gchildne = document.createElementNS(scope.svgNS, "g");
        gchildne.setAttribute("onmousedown", 'selectedResizergtagText(evt)');
        gchildne.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-ne");
        gchildne.setAttributeNS(null, "id", "ne");
        var rectanglene = document.createElementNS(scope.svgNS, "rect");
        rectanglene.setAttributeNS(null, "id", "rectanglene");
        rectanglene.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglene.setAttributeNS(null, "x", (x + Width - 3));
        rectanglene.setAttributeNS(null, "y", y - 3);
        rectanglene.setAttributeNS(null, "height", 6);
        rectanglene.setAttributeNS(null, "width", 6);
        rectanglene.setAttributeNS(null, "fill", "white");
        rectanglene.setAttributeNS(null, "stroke", "none");
        gchildne.appendChild(rectanglene);

        var rectangle2ndne = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndne.setAttributeNS(null, "id", "rectanglene");
        rectangle2ndne.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndne.setAttributeNS(null, "x", (x + Width - 10));
        rectangle2ndne.setAttributeNS(null, "y", y - 10);
        rectangle2ndne.setAttributeNS(null, "height", 20);
        rectangle2ndne.setAttributeNS(null, "width", 20);
        rectangle2ndne.setAttributeNS(null, "fill", "white");
        rectangle2ndne.setAttributeNS(null, "stroke", "none");
        gchildne.appendChild(rectangle2ndne);
        g.appendChild(gchildne);

        var gchildse = document.createElementNS(scope.svgNS, "g");
        gchildse.setAttributeNS(null, "id", "se");
        gchildse.setAttribute("onmousedown", 'selectedResizergtagText(evt)');
        gchildse.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-se");
        var rectanglese = document.createElementNS(scope.svgNS, "rect");
        rectanglese.setAttributeNS(null, "id", "rectanglese");
        rectanglese.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglese.setAttributeNS(null, "x", (x + Width - 3));
        rectanglese.setAttributeNS(null, "y", (y + Height - 3));
        rectanglese.setAttributeNS(null, "height", 6);
        rectanglese.setAttributeNS(null, "width", 6);
        rectanglese.setAttributeNS(null, "fill", "orange");
        rectanglese.setAttributeNS(null, "stroke", "none");
        gchildse.appendChild(rectanglese);

        var rectangle2ndse = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndse.setAttributeNS(null, "id", "rectanglese");
        rectangle2ndse.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndse.setAttributeNS(null, "x", (x + Width - 10));
        rectangle2ndse.setAttributeNS(null, "y", (y + Height - 10));
        rectangle2ndse.setAttributeNS(null, "height", 20);
        rectangle2ndse.setAttributeNS(null, "width", 20);
        rectangle2ndse.setAttributeNS(null, "fill", "orange");
        rectangle2ndse.setAttributeNS(null, "stroke", "none");
        gchildse.appendChild(rectangle2ndse);
        g.appendChild(gchildse);

        var gchildsw = document.createElementNS(scope.svgNS, "g");
        gchildsw.setAttributeNS(null, "id", "sw");
        gchildsw.setAttribute("onmousedown", 'selectedResizergtagText(evt)');
        gchildsw.setAttributeNS(null, "class", "djs-resizer djs-resizer-Participant_1uqgzki djs-resizer-sw");
        var rectanglesw = document.createElementNS(scope.svgNS, "rect");
        rectanglesw.setAttributeNS(null, "id", "rectanglesw");
        rectanglesw.setAttributeNS(null, "class", "djs-resizer-visual");
        rectanglesw.setAttributeNS(null, "x", x - 3);
        rectanglesw.setAttributeNS(null, "y", (y + Height - 3));
        rectanglesw.setAttributeNS(null, "height", 6);
        rectanglesw.setAttributeNS(null, "width", 6);
        rectanglesw.setAttributeNS(null, "fill", "orange");
        rectanglesw.setAttributeNS(null, "stroke", "none");
        gchildsw.appendChild(rectanglesw);

        var rectangle2ndsw = document.createElementNS(scope.svgNS, "rect");
        rectangle2ndsw.setAttributeNS(null, "id", "rectanglesw");
        rectangle2ndsw.setAttributeNS(null, "class", "djs-resizer-hit");
        rectangle2ndsw.setAttributeNS(null, "x", x - 10);
        rectangle2ndsw.setAttributeNS(null, "y", (y + Height - 10));
        rectangle2ndsw.setAttributeNS(null, "height", 20);
        rectangle2ndsw.setAttributeNS(null, "width", 20);
        rectangle2ndsw.setAttributeNS(null, "fill", "orange");
        rectangle2ndsw.setAttributeNS(null, "stroke", "none");
        gchildsw.appendChild(rectangle2ndsw);
        g.appendChild(gchildsw);
    }
    scope.svgElement.appendChild(g);

    return g;
}

function selectedResizergtagText(evnt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evnt.currentTarget).scope();
    if (scope) {
        scope.selectedResizergText = evnt.currentTarget;
        scope.currentX = evnt.offsetX;
        scope.currentY = evnt.offsetY;
        scope.isMouseDown = true;
        scope.svgElement.setAttributeNS(null, "onmouseup", "deselectResizergText(evt)");
        scope.svgElement.setAttributeNS(null, "onmousemove", "moveResizergText(evt)");
    }
}

function moveResizergText(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    if (scope && scope.isMouseDown && evt.buttons == 1 && scope.selectedShapeofText) {
        var dx = evt.offsetX - scope.currentX;
        var dy = evt.offsetY - scope.currentY;
        var parentNode = scope.selectedResizergText.parentNode;
        var isValid = false;
        var ElementXpos = parseFloat(scope.selectedText.getAttribute("x"));
        var ElementYpos = parseFloat(scope.selectedText.getAttribute("y"));
        var ElementHeight = parseFloat(scope.selectedText.getAttribute("height"));
        var ElementWidth = parseFloat(scope.selectedText.getAttribute("width"));

        if (scope.selectedResizergText.getAttribute("id") == "nw") {
            ElementXpos1 = ElementXpos + dx;
            ElementYpos1 = ElementYpos + dy;
            ElementHeight1 = ElementHeight - dy;
            ElementWidth1 = ElementWidth - dx;
            var diffx = (ElementXpos1 + ElementWidth1) - ElementXpos1;
            var diffy = (ElementYpos1 + ElementHeight1) - ElementYpos1;

            var Actualdiffx = (ElementXpos + ElementWidth) - ElementXpos;
            var Actualdiffy = (ElementYpos + ElementHeight) - ElementYpos;

            if (diffx > 15 && diffy > 15) {
                isValid = true;
            } else if (diffx >= Actualdiffx && diffy >= Actualdiffy) {
                isValid = true;
            }
        } else if (scope.selectedResizergText.getAttribute("id") == "ne") {
            ElementXpos1 = ElementXpos;
            ElementYpos1 = ElementYpos + dy;
            ElementHeight1 = ElementHeight - dy;
            ElementWidth1 = ElementWidth + dx;
            var diffx = (ElementXpos1 + ElementWidth1) - ElementXpos1;
            var diffy = (ElementYpos1 + ElementHeight1) - ElementYpos1;

            var Actualdiffx = (ElementXpos + ElementWidth) - ElementXpos;
            var Actualdiffy = (ElementYpos + ElementHeight) - ElementYpos;

            if (diffx > 15 && diffy > 15) {
                isValid = true;
            } else if (diffx >= Actualdiffx && diffy >= Actualdiffy) {
                isValid = true;
            }
        }
        else if (scope.selectedResizergText.getAttribute("id") == "sw") {
            ElementXpos1 = ElementXpos + dx;
            ElementYpos1 = ElementYpos;
            ElementHeight1 = ElementHeight + dy;
            ElementWidth1 = ElementWidth - dx;
            var diffx = (ElementXpos1 + ElementWidth1) - ElementXpos1;
            var diffy = (ElementYpos1 + ElementHeight1) - ElementYpos1;

            var Actualdiffx = (ElementXpos + ElementWidth) - ElementXpos;
            var Actualdiffy = (ElementYpos + ElementHeight) - ElementYpos;

            if (diffx > 15 && diffy > 15) {
                isValid = true;
            } else if (diffx >= Actualdiffx && diffy >= Actualdiffy) {
                isValid = true;
            }
        }
        else if (scope.selectedResizergText.getAttribute("id") == "se") {
            ElementXpos1 = ElementXpos;
            ElementYpos1 = ElementYpos;
            ElementHeight1 = ElementHeight + dy;
            ElementWidth1 = ElementWidth + dx;
            var diffx = (ElementXpos1 + ElementWidth1) - ElementXpos1;
            var diffy = (ElementYpos1 + ElementHeight1) - ElementYpos1;

            var Actualdiffx = (ElementXpos + ElementWidth) - ElementXpos;
            var Actualdiffy = (ElementYpos + ElementHeight) - ElementYpos;

            if (diffx > 15 && diffy > 15) {
                isValid = true;
            } else if (diffx >= Actualdiffx && diffy >= Actualdiffy) {
                isValid = true;
            }
        }
        if (isValid) {
            scope.isDirty = true;
            scope.selectedResizergText.setAttribute('x', parseFloat(scope.selectedResizergText.getAttribute("x")) + dx);
            scope.selectedResizergText.setAttribute('y', parseFloat(scope.selectedResizergText.getAttribute("y")) + dy);

            angular.forEach($(scope.selectedResizergText).children(), function (item) {
                item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);
                item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);
            });


            if (scope.selectedResizergText.getAttribute("id") == "nw") {
                if (scope.selectedText != 0) {
                    var itm = scope.selectedText;
                    if (itm.parentNode == null) {
                        var x = 10;
                    }
                    itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                    itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                    itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                    itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                    for (var i = 0; i < itm.childNodes.length; i++) {
                        itm.removeChild(itm.childNodes[i]);
                    }
                    itm.textContent = scope.selectedShapeofText.Text;
                    var textnode = wraptorect(itm, null, 0, 0.5, undefined, scope);
                    scope.selectedText = textnode;
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "ne") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "sw") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergText.getAttribute("id") == "ne") {
                if (scope.selectedText != 0) {
                    var itm = scope.selectedText;
                    itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                    itm.setAttribute('height', parseFloat(itm.getAttribute("height")) - dy);
                    itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);

                    for (var i = 0; i < itm.childNodes.length; i++) {
                        itm.removeChild(itm.childNodes[i]);
                    }
                    itm.textContent = scope.selectedShapeofText.Text;
                    var textnode = wraptorect(itm, null, 0, 0.5, undefined, scope);
                    scope.selectedText = textnode;
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "nw") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "se") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergText.getAttribute("id") == "sw") {
                if (scope.selectedText != 0) {
                    var itm = scope.selectedText;
                    itm.setAttribute('width', parseFloat(itm.getAttribute("width")) - dx);
                    itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);
                    itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);

                    for (var i = 0; i < itm.childNodes.length; i++) {
                        itm.removeChild(itm.childNodes[i]);
                    }
                    itm.textContent = scope.selectedShapeofText.Text;
                    var textnode = wraptorect(itm, null, 0, 0.5, undefined, scope);
                    scope.selectedText = textnode;
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "se") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "nw") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }

            if (scope.selectedResizergText.getAttribute("id") == "se") {
                if (scope.selectedText != 0) {
                    var itm = scope.selectedText;
                    itm.setAttribute('width', parseFloat(itm.getAttribute("width")) + dx);
                    itm.setAttribute('height', parseFloat(itm.getAttribute("height")) + dy);

                    for (var i = 0; i < itm.childNodes.length; i++) {
                        itm.removeChild(itm.childNodes[i]);
                    }
                    itm.textContent = scope.selectedShapeofText.Text;
                    var textnode = wraptorect(itm, null, 0, 0.5, undefined, scope);
                    scope.selectedText = textnode;
                }

                angular.forEach($(parentNode).children(), function (item) {
                    if (item.getAttribute('id') == "sw") {
                        item.setAttribute('y', parseFloat(item.getAttribute("y")) + dy);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('y', parseFloat(itm.getAttribute("y")) + dy);
                        });
                    }
                    if (item.getAttribute('id') == "ne") {
                        item.setAttribute('x', parseFloat(item.getAttribute("x")) + dx);

                        angular.forEach($(item).children(), function (itm) {
                            itm.setAttribute('x', parseFloat(itm.getAttribute("x")) + dx);
                        });
                    }
                });
            }
            scope.currentX = evt.offsetX;
            scope.currentY = evt.offsetY;
        }
        //else {
        //    deselectResizergText(evt);
        //}

    } else {
        deselectResizergText(evt);
    }
}

function deselectTextElement(evt) {
    var scope = angular.element(evt.currentTarget).scope();
    if (scope.selectedShapeofText && scope.selectedText) {
        scope.selectedShapeofText.LabelLeft = parseFloat(scope.selectedText.getAttribute("x"));
        scope.selectedShapeofText.LabelTop = parseFloat(scope.selectedText.getAttribute("y"));
        scope.selectedShapeofText.LabelHeight = parseFloat(scope.selectedText.getAttribute("height"));
        scope.selectedShapeofText.LabelWidth = parseFloat(scope.selectedText.getAttribute("width"));
        scope.svgElement.removeAttributeNS(null, "onmousemove");
        scope.svgElement.removeAttributeNS(null, "onmouseup");
    }
}

function deselectResizergText(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    scope.isMouseDown = false;

    if (scope.selectedResizergText != 0 && scope.selectedShapeofText && scope.selectedText) {

        scope.selectedShapeofText.LabelLeft = parseFloat(scope.selectedText.getAttribute("x"));
        scope.selectedShapeofText.LabelTop = parseFloat(scope.selectedText.getAttribute("y"));
        scope.selectedShapeofText.LabelHeight = parseFloat(scope.selectedText.getAttribute("height"));
        scope.selectedShapeofText.LabelWidth = parseFloat(scope.selectedText.getAttribute("width"));
        scope.svgElement.removeAttributeNS(null, "onmousemove");
        scope.svgElement.removeAttributeNS(null, "onmouseup");
        scope.selectedResizergText = 0;
    }
}
