// Drawing New Edge

function hoverOnElement(evt, eleorscope) {
    var scope;
    //var scope = getCurrentFileScope();
    if (evt && evt != null) {
        scope = angular.element(eleorscope).scope();
    } else {
        scope = eleorscope;
    }
    if (scope && scope.activeTab == "MAP" && scope.IsMapEditable && !scope.IsVersionDisplay && scope.ClonedElement == null) {
        var Element = "";
        if (scope.sourceElementOnMouseHover != null && evt != null) {
            if (scope.sourceElementOnMouseHover != evt.currentTarget.parentNode) {
                scope.targetElementOnMouseHover = evt.currentTarget.parentNode;
            }
        }
        if (evt != null && evt.currentTarget.parentNode.getAttribute("id") != "resizergroup") {
            scope.hoverElement = evt.currentTarget.parentNode;
            Element = evt.currentTarget.parentNode;
        }
        else {
            Element = scope.hoverElement;
        }

        if (scope.ClonedLineElement != null) {
            scope.targetElementForLine = Element;
        }

        var xpos = parseFloat(Element.getAttribute("x"));
        var ypos = parseFloat(Element.getAttribute("y"));
        var height = parseFloat(Element.getAttribute("height"));
        var width = parseFloat(Element.getAttribute("width"));
        var cx;
        var cy;
        var MainElement = null;
        for (var i = 0; i < Element.childNodes.length; i++) {
            if (Element.childNodes[i].getAttribute("id") == "MainElement") {
                MainElement = Element.childNodes[i];
                break;
            }
        }

        if (MainElement.nodeName != "circle" && scope.hoverElement.id != "participant") {
            cx = xpos + (width / 2);
            cy = ypos + (height / 2);
        }
        else if (scope.hoverElement.id == "participant") {
            cx = xpos + 20;
            cy = ypos + (height / 2);
        }
        else {
            cx = MainElement.getAttribute("cx");
            cy = MainElement.getAttribute("cy");
        }


        var isFound = false;
        removehovercirclefromsvgElement(scope);
        if (!isFound) {
            var myCircle = document.createElementNS(scope.svgNS, "circle");
            myCircle.setAttributeNS(null, "id", "hovercircle");
            myCircle.setAttributeNS(null, "cx", cx);
            myCircle.setAttributeNS(null, "cy", cy);
            myCircle.setAttributeNS(null, "r", 6);
            myCircle.setAttribute("onmousedown", 'selectoncircle(evt)');
            myCircle.setAttributeNS(null, "fill", "gray");
            myCircle.setAttributeNS(null, "stroke", "black");
            scope.svgElement.appendChild(myCircle);
        }
    }
}

function removehovercirclefromsvgElement(scope) {
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].nodeName == "circle") {
            if (scope.svgElement.childNodes[i].getAttribute("id") == "hovercircle") {
                //isFound = true;
                scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                break;
            }
        }
    }
}

function removehoverCircle(evt, ele) {
    var scope = angular.element(ele).scope();
    //var scope = getCurrentFileScope();
    var X = evt.offsetX;
    var Y = evt.offsetY;
    var MainElement = null;
    if (scope && scope.hoverElement != null) {
        for (var i = 0; i < scope.hoverElement.childNodes.length; i++) {
            if (scope.hoverElement.childNodes[i].getAttribute("id") == "MainElement") {
                MainElement = scope.hoverElement.childNodes[i];
                break;
            }
        }
        if (MainElement == null) {
            if (scope.hoverElement.id == "MainElement") {
                MainElement = scope.hoverElement;
            }
        }
        if (MainElement.nodeName != "circle") {
            var x = parseFloat(scope.hoverElement.getAttribute("x"));
            var y = parseFloat(scope.hoverElement.getAttribute("y"));
            var height = parseFloat(scope.hoverElement.getAttribute("height"));
            var width = parseFloat(scope.hoverElement.getAttribute("width"));
            if ((x + width) > X && X > x && (y + height) > Y && Y > y) {

            } else {
                if (scope.sourceElementOnMouseHover != null) {
                    scope.targetElementOnMouseHover = null;
                }
                scope.targetElementForLine = null;
                for (var i = scope.svgElement.childNodes.length - 1; i > 0 ; i--) {
                    if (scope.svgElement.childNodes[i].nodeName == "circle") {
                        if (scope.svgElement.childNodes[i].getAttribute("id") == "hovercircle") {
                            scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                            break;
                        }
                    }
                }
            }

        }
        else {
            var cx = parseFloat(MainElement.getAttribute("cx"));
            var cy = parseFloat(MainElement.getAttribute("cy"));
            var r = parseFloat(MainElement.getAttribute("r"));

            var x = cx - 15;
            var y = cy - 15;
            var height = 2 * r;
            var width = 2 * r;

            if ((x + width) > X && X > x && (y + height) > Y && Y > y) {

            } else {
                if (scope.sourceElementOnMouseHover != null) {
                    scope.targetElementOnMouseHover = null;
                }
                scope.targetElementForLine = null;
                for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
                    if (scope.svgElement.childNodes[i].nodeName == "circle") {
                        if (scope.svgElement.childNodes[i].getAttribute("id") == "hovercircle") {
                            scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
                            i--;
                        }
                    }
                }
            }
        }
    }
}


function selectoncircle(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    createDummyLine(scope);
    scope._previousPosition = { Left: 0, Top: 0 };
    setMouseEventsOnElement(scope.svgElement, scope);
}

function setMouseEventsOnElement(evt, scope) {
    //var scope = getCurrentFileScope();
    var svgElement = evt;
    scope.sourceElementOnMouseHover = scope.hoverElement;
    svgElement.setAttributeNS(null, "onmousemove", "movecursorfordrawingLine(evt)");
    svgElement.setAttributeNS(null, "onmouseup", "removeMouseMoveEvent(evt)");
}

function createDummyLine(scope) {
    var g = document.createElementNS(scope.svgNS, "g");
    g.setAttribute('id', 'dottedline');
    var line = document.createElementNS(scope.svgNS, "line");
    line.setAttribute("stroke-width", 2);
    line.setAttribute("stroke-dasharray", "5,5");
    line.setAttribute("x1", 0);
    line.setAttribute("y1", 0);
    line.setAttribute("x2", 0);
    line.setAttribute("y2", 0);
    line.setAttributeNS(null, "stroke", "#3a3a3a");
    g.appendChild(line);
    scope.svgElement.appendChild(g);
}

function movecursorfordrawingLine(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    //dx = evt.offsetX - currentX;
    //dy = evt.offsetY - currentY;
    if (scope && evt.buttons == 1) {
        var isFound = false;
        var index = "";
        for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
            if (scope.svgElement.childNodes[i].id == "dottedline") {
                isFound = true;
                index = i;
                break;
            }
        }
        if (scope.sourceElementOnMouseHover != null) {
            var MainElement = null;
            for (var i = 0; i < scope.sourceElementOnMouseHover.childNodes.length; i++) {
                if (scope.sourceElementOnMouseHover.childNodes[i].getAttribute("id") == "MainElement") {
                    MainElement = scope.sourceElementOnMouseHover.childNodes[i];
                    break;
                }
            }

            var x = parseFloat(MainElement.getAttribute("x"));
            var y = parseFloat(MainElement.getAttribute("y"));
            var height = parseFloat(MainElement.getAttribute("height"));
            var width = parseFloat(MainElement.getAttribute("width"));


            if (MainElement.nodeName == "circle") {
                var cx = parseFloat(MainElement.getAttribute("cx"));
                var cy = parseFloat(MainElement.getAttribute("cy"));
                var r = parseFloat(MainElement.getAttribute("r"));
                x = cx - 15;
                y = cy - 15;
                height = 2 * r;
                width = 2 * r;
            }

            var points;
            var source = { Left: x, Top: y, Height: height, Width: width };
            if (scope.targetElementOnMouseHover != null && scope.svgElement.childNodes[index] && scope.svgElement.childNodes[index].childNodes && scope.svgElement.childNodes[index].childNodes.length > 0) {
                var sourcePoint = { Left: parseFloat(scope.svgElement.childNodes[index].childNodes[0].getAttribute("x1")), Top: parseFloat(scope.svgElement.childNodes[index].childNodes[0].getAttribute("y1")) };
                var aCurPos = { Left: evt.offsetX, Top: evt.offsetY };
                var destination = { Left: parseFloat(scope.targetElementOnMouseHover.getAttribute("x")), Top: parseFloat(scope.targetElementOnMouseHover.getAttribute("y")), Height: parseFloat(scope.targetElementOnMouseHover.getAttribute("height")), Width: parseFloat(scope.targetElementOnMouseHover.getAttribute("width")) };
                var atargetPoint = UpdateTargetPoint(aCurPos, destination, source, scope);

                if (atargetPoint != undefined) {
                    points = GetEdgeWayPoints(source, destination, sourcePoint, atargetPoint);
                }
            }
            else {
                scope._previousPosition = { Left: evt.offsetX, Top: evt.offsetY };
                var destination = { Left: evt.offsetX, Top: evt.offsetY, Height: 10, Width: 10 };
                points = GetEdgeWayPointsForDottedLine(source, destination);
            }

            //var x1 = x + (width / 2);
            //var y1 = y + height;

            if (index != "" && points != undefined) {
                for (var i = 0; i < scope.svgElement.childNodes[index].childNodes.length; i++) {
                    scope.svgElement.childNodes[index].removeChild(scope.svgElement.childNodes[index].childNodes[i]);
                    i--;
                }
                scope.NewEdgeWayPoints = points;
                scope.NewEdgeWayPoints = setShapeNameToWayPoints(scope.NewEdgeWayPoints);
                for (var i = 0; i < points.length; i++) {
                    if (i < points.length - 1) {
                        var line = document.createElementNS(scope.svgNS, "line");
                        line.setAttribute('id', 'line');
                        line.setAttribute("stroke-width", 2);
                        line.setAttribute("stroke-dasharray", "5,5");
                        line.setAttribute("x1", points[i].Left);
                        line.setAttribute("y1", points[i].Top);
                        line.setAttribute("x2", points[i + 1].Left);
                        line.setAttribute("y2", points[i + 1].Top);
                        line.setAttributeNS(null, "stroke", "#3a3a3a");
                        scope.svgElement.childNodes[index].appendChild(line);
                    }
                }
            }
        }

        scope.currentX = evt.offsetX;
        scope.currentY = evt.offsetY;
    } else {
        removeMouseMoveEvent(evt);
    }
}

function UpdateTargetPoint(aCurPos, targetShapeForPreview, sourceShape, scope) {
    //var scope = getCurrentFileScope();
    var targetPoint = aCurPos;
    var edgeDiff = 10;
    if (scope._previousPosition.Left == 0 && scope._previousPosition.Top == 0) {
        scope._previousPosition = aCurPos;
        return;
    }

    if (null != targetShapeForPreview) {
        //var IsGateWayOrEvent = this.targetShapeForPreview.BPMNElement is BPMNGatewayVM || this.targetShapeForPreview.BPMNElement is EventVM;
        var IsGateWayOrEvent = false;


        //if (!IsEditingExistingEdge)
        //    this.NewEdgeVM.IsEndPosVisible = false;

        if (IsLeftLimitReached(targetShapeForPreview, aCurPos, scope, edgeDiff)) {
            if (IsGateWayOrEvent) {
                targetPoint.Left = targetShapeForPreview.Left;
                targetPoint.Top = targetShapeForPreview.Top + targetShapeForPreview.Height / 2;
            }
            else {
                targetPoint.Left = targetShapeForPreview.Left;
                targetPoint.Top = aCurPos.Top;
            }
        }
        else if (IsTopLimitReached(targetShapeForPreview, aCurPos, scope, edgeDiff)) {
            if (IsGateWayOrEvent) {
                targetPoint.Left = targetShapeForPreview.Left + targetShapeForPreview.Width / 2;
                targetPoint.Top = targetShapeForPreview.Top;
            }
            else {
                targetPoint.Left = aCurPos.Left;
                targetPoint.Top = targetShapeForPreview.Top;
            }
        }
        else if (IsRightLimitReached(targetShapeForPreview, aCurPos, scope, edgeDiff)) {
            if (IsGateWayOrEvent) {
                targetPoint.Left = targetShapeForPreview.Left + targetShapeForPreview.Width;
                targetPoint.Top = targetShapeForPreview.Top + targetShapeForPreview.Height / 2;
            }
            else {
                targetPoint.Left = targetShapeForPreview.Left + targetShapeForPreview.Width;
                targetPoint.Top = aCurPos.Top;
            }
        }
        else if (IsBottomLimitReached(targetShapeForPreview, aCurPos, scope, edgeDiff)) {
            if (IsGateWayOrEvent) {
                targetPoint.Left = targetShapeForPreview.Left + targetShapeForPreview.Width / 2;
                targetPoint.Top = targetShapeForPreview.Top + targetShapeForPreview.Height;
            }
            else {
                targetPoint.Left = aCurPos.Left;
                targetPoint.Top = targetShapeForPreview.Top + targetShapeForPreview.Height;
            }
        }
        else {
            var IsTargatePointUpdated = false;
            if (targetPoint.Left == targetShapeForPreview.Left || targetPoint.Left == (targetShapeForPreview.Left + targetShapeForPreview.Left)) {
                if (IsGateWayOrEvent) {
                    targetPoint.Top = targetShapeForPreview.Top + targetShapeForPreview.Height / 2;
                }
                else {
                    targetPoint.Top = aCurPos.Top;
                }

                //if (_targetEdge == null)
                //{
                //    this.NewEdgeVM.UpdateEndPoint(targetPoint);
                //}
                IsTargatePointUpdated = true;
            }
            if (targetPoint.Top == targetShapeForPreview.Top || targetPoint.Top == (targetShapeForPreview.Top + targetShapeForPreview.Height)) {
                if (IsGateWayOrEvent) {
                    targetPoint.Left = targetShapeForPreview.Left + targetShapeForPreview.Width / 2;
                }
                else {
                    targetPoint.Left = aCurPos.Left;
                }

                //if (this._targetEdge == null)
                //{
                //    this.NewEdgeVM.UpdateEndPoint(targetPoint);
                //}
                IsTargatePointUpdated = true;
            }
            if (!(IsTargatePointUpdated)) {
                if (UpdateEndpoint(aCurPos, targetShapeForPreview, sourceShape, targetPoint))
                    return targetPoint;
            }
        }
        //UpdateTargetShapeForPreview(aCurPos);  
    }
    scope._previousPosition = aCurPos;
    return targetPoint;
}

function IsLeftLimitReached(aShapeVM, CurPosition, scope, edgeDiff) {
    var retVal = false;
    var diff = CurPosition.Left - aShapeVM.Left;
    if (diff >= 0 && aShapeVM.Left >= scope._previousPosition.Left - edgeDiff) {
        retVal = true;
    }
    return retVal;
}

function IsRightLimitReached(aShapeVM, CurPosition, scope, edgeDiff) {
    var retVal = false;
    var diff = (aShapeVM.Left + aShapeVM.Width) - CurPosition.Left;
    if (diff >= 0 && (aShapeVM.Left + aShapeVM.Width) <= scope._previousPosition.Left + edgeDiff) {
        retVal = true;
    }
    return retVal;
}

function IsTopLimitReached(aShapeVM, CurPosition, scope, edgeDiff) {
    var retVal = false;
    var diff = CurPosition.Top - aShapeVM.Top;
    if (diff >= 0 && (scope._previousPosition.Top - edgeDiff) <= aShapeVM.Top) {
        retVal = true;
    }
    return retVal;
}

function IsBottomLimitReached(aShapeVM, CurPosition, scope, edgeDiff) {
    var retVal = false;
    var diff = (aShapeVM.Top + aShapeVM.Height) - CurPosition.Top;
    if (diff >= 0 && (aShapeVM.Top + aShapeVM.Height) <= scope._previousPosition.Top + edgeDiff) {
        retVal = true;
    }
    return retVal;
}

function UpdateEndpoint(aCurPos, targetShapeForPreview, _sourceShape, targetPoint) {
    if (null != targetShapeForPreview &&
        _sourceShape != null) {
        var sourceBounds = _sourceShape;
        var targetBounds = targetShapeForPreview;
        if (targetBounds.Left < aCurPos.Left &&
            targetBounds.Top < aCurPos.Top &&
            targetBounds.Top + targetBounds.Height > aCurPos.Top &&
            targetBounds.Left + targetBounds.Width > aCurPos.Left) {
            if (sourceBounds.Left + sourceBounds.Width < targetBounds.Left) // &&
                //sourceBounds.Y > targetBounds.Y && sourceBounds.Y < targetBounds.Y + targetBounds.height)
            {
                aCurPos.Left = targetBounds.Left;
            }
            else if (sourceBounds.Left > targetBounds.Left + targetBounds.Width) //&&
                //sourceBounds.Y > targetBounds.Y && sourceBounds.Y < targetBounds.Y + targetBounds.height)
            {
                aCurPos.Left = targetBounds.Left + targetBounds.Width;
            }
            else if (sourceBounds.Top + sourceBounds.Height < targetBounds.Top) {
                aCurPos.Top = targetBounds.Top;
            }
            else if (sourceBounds.Top > targetBounds.Top + targetBounds.Height) {
                aCurPos.Top = targetBounds.Top + targetBounds.Height;
            }

            this.targetPoint = aCurPos;
            //this.NewEdgeVM.UpdateEndPoint(aCurPos);
            return targetPoint;
        }
    }
}

function GetEdgeWayPointsForDottedLine(arecSource, arecDest) {
    var polyLinePointArray = [];

    if (arecSource != null && arecDest != null) {

        var sourceTop = arecSource.Top;
        var sourceLeft = arecSource.Left;
        var sourceWidth = arecSource.Width;
        var sourceHeight = arecSource.Height;

        var destTop = arecDest.Top;
        var destLeft = arecDest.Left;
        var destWidth = arecDest.Width;
        var destHeight = arecDest.Height;

        var sourceMidY = sourceTop + sourceHeight / 2;
        var sourceMidX = sourceLeft + sourceWidth / 2;

        var destMidY = destTop + destHeight / 2;
        var destMidX = destLeft + destWidth / 2;

        var MinDiff = 0;

        if (destLeft > (sourceLeft + sourceWidth + MinDiff)) {
            var pt = { Left: sourceLeft + sourceWidth, Top: sourceMidY };
            polyLinePointArray.push(pt);
            pt = { Left: (sourceLeft + sourceWidth) + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: sourceMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: (sourceLeft + sourceWidth) + ((destLeft - (sourceLeft + sourceWidth)) / 2), Top: destMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destLeft, Top: destMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }

        }
        else if (destTop > (sourceTop + sourceHeight + MinDiff)) {
            var pt = { Left: sourceMidX, Top: sourceTop + sourceHeight };
            polyLinePointArray.push(pt);
            pt = { Left: sourceMidX, Top: ((sourceTop + sourceHeight) + (destTop - (sourceTop + sourceHeight)) / 2) };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destMidX, Top: ((sourceTop + sourceHeight) + (destTop - (sourceTop + sourceHeight)) / 2) };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destMidX, Top: destTop };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }

        }
        else if (sourceLeft > (destLeft + destWidth + MinDiff)) {
            var pt = { Left: sourceLeft, Top: sourceMidY };
            polyLinePointArray.push(pt);
            pt = { Left: (sourceLeft) - ((sourceLeft - (destLeft + destWidth)) / 2), Top: sourceMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: (sourceLeft) - ((sourceLeft - (destLeft + destWidth)) / 2), Top: destMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destLeft + destWidth, Top: destMidY };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }

        }
        else if (sourceTop > (destTop + destHeight + MinDiff)) {
            var pt = { Left: sourceMidX, Top: sourceTop };
            polyLinePointArray.push(pt);
            pt = { Left: sourceMidX, Top: ((sourceTop) - (sourceTop - (destTop + destHeight)) / 2) };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destMidX, Top: ((sourceTop) - (sourceTop - (destTop + destHeight)) / 2) };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }
            pt = { Left: destMidX, Top: destTop + destHeight };
            if (isPointNotFound(pt, polyLinePointArray)) {
                polyLinePointArray.push(pt);
            }

        }
        else {
            polyLinePointArray.push({ Left: sourceMidX, Top: sourceTop + sourceHeight });
            polyLinePointArray.push({ Left: sourceMidX + (destMidX - sourceMidX), Top: sourceTop + sourceHeight });
            polyLinePointArray.push({ Left: destMidX, Top: destTop });

        }
    }

    return polyLinePointArray;
}

function isPointNotFound(pt, polyLinePointArray) {
    isNotFound = true;
    for (var i = 0; i < polyLinePointArray.length; i++) {
        if (polyLinePointArray[i].Left == pt.Left && polyLinePointArray[i].Top == pt.Top) {
            isNotFound = false;
            break;
        }
    }
    return isNotFound;
}

function removeMouseMoveEvent(evt) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(evt.currentTarget).scope();
    evt.currentTarget.removeAttributeNS(null, "onmousemove");

    if (scope.sourceElementOnMouseHover != null && scope.targetElementOnMouseHover != null) {
        // Adding into Object
        //getting sourceElement ID & TargetElement ID
        scope.isDirty = true;
        var MainElementofSource = null;
        for (var i = 0; i < scope.sourceElementOnMouseHover.childNodes.length; i++) {
            if (scope.sourceElementOnMouseHover.childNodes[i].getAttribute("id") == "MainElement") {
                MainElementofSource = scope.sourceElementOnMouseHover.childNodes[i];
                break;
            }
        }

        var MainElementofTarget = null;
        for (var i = 0; i < scope.targetElementOnMouseHover.childNodes.length; i++) {
            if (scope.targetElementOnMouseHover.childNodes[i].getAttribute("id") == "MainElement") {
                MainElementofTarget = scope.targetElementOnMouseHover.childNodes[i];
                break;
            }
        }

        var SourceElementID = "";
        var TargetElementID = "";
        var sourceProcessRef = "";
        var targetProcessRef = "";
        var srcElement = "";
        var tarElement = "";
        for (var i = 0; i < scope.objData.lstShapes.length; i++) {
            if (SourceElementID == "" || TargetElementID == "") {
                if ((-0.1 < (scope.objData.lstShapes[i].Left - parseFloat(MainElementofSource.getAttribute("x")))) && ((scope.objData.lstShapes[i].Left - parseFloat(MainElementofSource.getAttribute("x"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - parseFloat(MainElementofSource.getAttribute("y")))) && ((scope.objData.lstShapes[i].Top - parseFloat(MainElementofSource.getAttribute("y"))) < 0.1) && SourceElementID == "") {
                    SourceElementID = scope.objData.lstShapes[i].Id;
                    sourceProcessRef = scope.objData.lstShapes[i].processRef;
                    srcElement = scope.objData.lstShapes[i];
                }

                if ((-0.1 < (scope.objData.lstShapes[i].Left - parseFloat(MainElementofTarget.getAttribute("x")))) && ((scope.objData.lstShapes[i].Left - parseFloat(MainElementofTarget.getAttribute("x"))) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - parseFloat(MainElementofTarget.getAttribute("y")))) && ((scope.objData.lstShapes[i].Top - parseFloat(MainElementofTarget.getAttribute("y"))) < 0.1) && TargetElementID == "") {
                    TargetElementID = scope.objData.lstShapes[i].Id;
                    targetProcessRef = scope.objData.lstShapes[i].processRef;
                    tarElement = scope.objData.lstShapes[i];
                }

                if (MainElementofSource.nodeName == "circle" && SourceElementID == "") {
                    if ((-0.1 < (scope.objData.lstShapes[i].Left - (parseFloat(MainElementofSource.getAttribute("cx")) - 15))) && ((scope.objData.lstShapes[i].Left - (parseFloat(MainElementofSource.getAttribute("cx")) - 15)) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - (parseFloat(MainElementofSource.getAttribute("cy")) - 15))) && ((scope.objData.lstShapes[i].Top - (parseFloat(MainElementofSource.getAttribute("cy")) - 15)) < 0.1)) {
                        SourceElementID = scope.objData.lstShapes[i].Id;
                        sourceProcessRef = scope.objData.lstShapes[i].processRef;
                        srcElement = scope.objData.lstShapes[i];
                    }
                }

                if (MainElementofTarget.nodeName == "circle" && TargetElementID == "") {
                    if ((-0.1 < (scope.objData.lstShapes[i].Left - ((parseFloat(MainElementofTarget.getAttribute("cx")) - 15)))) && ((scope.objData.lstShapes[i].Left - (parseFloat(MainElementofTarget.getAttribute("cx")) - 15)) < 0.1) && (-0.1 < (scope.objData.lstShapes[i].Top - (parseFloat(MainElementofTarget.getAttribute("cy")) - 15))) && ((scope.objData.lstShapes[i].Top - (parseFloat(MainElementofTarget.getAttribute("cy")) - 15)) < 0.1)) {
                        TargetElementID = scope.objData.lstShapes[i].Id;
                        targetProcessRef = scope.objData.lstShapes[i].processRef;
                        tarElement = scope.objData.lstShapes[i];
                    }
                }
            }
            else {
                break;
            }
        }

        var isValidNode = CheckIsValidFlowNodeOrNot(srcElement, tarElement);
        if (isValidNode) {

            if (SourceElementID != "" && TargetElementID != "") {
                var isFound = false;
                for (var j = 0; j < scope.objData.lstShapes.length; j++) {
                    if (scope.objData.lstShapes[j].ShapeType == "sequenceFlow" || scope.objData.lstShapes[j].ShapeType == "dataOutputAssociation" || scope.objData.lstShapes[j].ShapeType == "association" || scope.objData.lstShapes[j].ShapeType == "messageFlow" || scope.objData.lstShapes[j].ShapeType == "dataInputAssociation") {
                        if (scope.objData.lstShapes[j].SourceElement == SourceElementID && scope.objData.lstShapes[j].TargetElement == TargetElementID) {
                            isFound = true;
                            break;
                        }
                    }
                }
                if (isFound) {
                    alert("Nodes Already Connected..");
                }
                else {
                    // Update the object
                    if (tarElement.ShapeType == "inclusiveGateway" || tarElement.ShapeType == "parallelGateway") {
                        tarElement.ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                        var direction = getgateWaydirectionbasedOnEdges(tarElement.Id, scope, 'In');
                        tarElement.ShapeModel.dictAttributes.gatewayDirection = direction;
                    } else if (srcElement.ShapeType == "inclusiveGateway" || srcElement.ShapeType == "parallelGateway") {
                        srcElement.ShapeModel.dictAttributes.gatewayDirection = "Unspecified";
                        var direction = getgateWaydirectionbasedOnEdges(srcElement.Id, scope, 'Out');
                        srcElement.ShapeModel.dictAttributes.gatewayDirection = direction;
                    }
                    var shapeId = "EDGE_" + generateUUID();
                    var Edgeid = generateUUID();
                    var lineobj = null;
                    if (sourceProcessRef == targetProcessRef && scope.targetElementOnMouseHover.id != "textAnnotation" && scope.sourceElementOnMouseHover.id != "textAnnotation" && srcElement.ShapeType != "dataStoreReference" && tarElement.ShapeType != "dataStoreReference" && srcElement.ShapeType != "dataObjectReference" && tarElement.ShapeType != "dataObjectReference") {
                        lineobj = {
                            Id: Edgeid, lstWayPoints: scope.NewEdgeWayPoints, ShapeName: "BPMNEdge", ShapeType: "sequenceFlow", SourceElement: SourceElementID,
                            TargetElement: TargetElementID, LabelLeft: 0, LabelTop: 0, LabelHeight: 50, LabelWidth: 50, Text: "", processRef: sourceProcessRef, ShapeId: shapeId
                        };
                        var EdgeShapeModel = null;
                        if (srcElement.ShapeType != "exclusiveGateway" && srcElement.ShapeType != "inclusiveGateway") {
                            EdgeShapeModel = { dictAttributes: { id: Edgeid, sourceRef: SourceElementID, targetRef: TargetElementID }, Elements: [], Children: [], Name: "sequenceFlow", prefix: null, Value: "" };
                        }
                        else {
                            EdgeShapeModel = {
                                dictAttributes: { id: Edgeid, sourceRef: SourceElementID, targetRef: TargetElementID }, Elements: [{
                                    dictAttributes: {},
                                    Elements: [{
                                        dictAttributes: {}, Elements: [{
                                            dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "leftside", prefix: "",
                                            IsValueInCDATAFormat: false, Value: ""
                                        }, { dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "rightside", prefix: "", IsValueInCDATAFormat: false, Value: "" }],
                                        Children: [], Name: "conditionExpression", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                    }, { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [], Name: "extensionElements", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                }], Children: [], Name: "sequenceFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                            };
                        }
                        lineobj.ShapeModel = EdgeShapeModel;
                    }
                    else if (scope.targetElementOnMouseHover.id == "textAnnotation" || scope.sourceElementOnMouseHover.id == "textAnnotation") {
                        var length = scope.NewEdgeWayPoints.length;
                        if (length > 2) {
                            var numberofElementsToRemove = length - 2;
                            scope.NewEdgeWayPoints.splice(1, numberofElementsToRemove);
                        }
                        lineobj = {
                            Id: Edgeid, lstWayPoints: scope.NewEdgeWayPoints, ShapeName: "BPMNEdge", ShapeType: "association", SourceElement: SourceElementID,
                            TargetElement: TargetElementID, LabelLeft: 0, LabelTop: 0, LabelHeight: 50, LabelWidth: 50, Text: "", processRef: sourceProcessRef, ShapeId: shapeId
                        };
                        var associationShapeModel = { dictAttributes: { id: Edgeid, sourceRef: SourceElementID, targetRef: TargetElementID, associationDirection: "None" }, Elements: [], Children: [], Name: "association", prefix: null, Value: "" };
                        lineobj.ShapeModel = associationShapeModel;
                    }
                    else if ((srcElement.ShapeType == "dataStoreReference" || tarElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference" || tarElement.ShapeType == "dataObjectReference")) {
                        var bool = IsValidShapeForAssociation(srcElement, tarElement);
                        if (bool) {
                            if (srcElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference") {
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
                                lineobj = {
                                    Id: Edgeid, lstWayPoints: scope.NewEdgeWayPoints, ShapeName: "BPMNEdge", ShapeType: "dataInputAssociation", SourceElement: SourceElementID,
                                    TargetElement: TargetElementID, LabelLeft: 0, LabelTop: 0, LabelHeight: 50, LabelWidth: 50, Text: "", processRef: sourceProcessRef, ShapeId: shapeId
                                };

                                var datainPutAssociationID = Edgeid;
                                var datainPutAssociationModel = {
                                    dictAttributes: { id: datainPutAssociationID },
                                    Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "sourceRef", prefix: "", IsValueInCDATAFormat: false, Value: srcElement.Id },
                                        { dictAttributes: {}, Elements: [], Children: [], Name: "targetRef", prefix: "", IsValueInCDATAFormat: false, Value: IOsepecificationID }],
                                    Children: [], Name: "dataInputAssociation", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                };
                                lineobj.ShapeModel = datainPutAssociationModel;
                            } else if (tarElement.ShapeType == "dataStoreReference" || tarElement.ShapeType == "dataObjectReference") {
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
                                lineobj = {
                                    Id: Edgeid, lstWayPoints: scope.NewEdgeWayPoints, ShapeName: "BPMNEdge", ShapeType: "dataOutputAssociation", SourceElement: SourceElementID,
                                    TargetElement: TargetElementID, LabelLeft: 0, LabelTop: 0, LabelHeight: 50, LabelWidth: 50, Text: "", processRef: sourceProcessRef, ShapeId: shapeId
                                };

                                var dataOutPutAssociationModel = {
                                    dictAttributes: { id: Edgeid },
                                    Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "sourceRef", prefix: "", IsValueInCDATAFormat: false, Value: IOsepecificationID },
                                        { dictAttributes: {}, Elements: [], Children: [], Name: "targetRef", prefix: "", IsValueInCDATAFormat: false, Value: tarElement.Id }],
                                    Children: [], Name: "dataOutputAssociation", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                };
                                lineobj.ShapeModel = dataOutPutAssociationModel;
                            }
                        }
                    }
                    else {
                        if (srcElement.ShapeType != "exclusiveGateway" && srcElement.ShapeType != "inclusiveGateway" && tarElement.ShapeType != "exclusiveGateway" && tarElement.ShapeType != "inclusiveGateway" && srcElement.ShapeType != "eventBasedGateway" && srcElement.ShapeType != "parallelGateway"
                        && tarElement.ShapeType != "eventBasedGateway" && tarElement.ShapeType != "parallelGateway") {
                            if (scope.objData.CollaborationId == "") {
                                scope.objData.CollaborationId = "COLLABORATION_" + generateUUID();
                            }
                            var processref = scope.objData.CollaborationId;
                            lineobj = {
                                Id: Edgeid, lstWayPoints: scope.NewEdgeWayPoints, ShapeName: "BPMNEdge", ShapeType: "messageFlow", SourceElement: SourceElementID,
                                TargetElement: TargetElementID, LabelLeft: 0, LabelTop: 0, LabelHeight: 50, LabelWidth: 50, Text: "", processRef: processref, ShapeId: shapeId
                            };
                            var MessageFlowModel = {
                                dictAttributes: { id: Edgeid, sourceRef: SourceElementID, targetRef: TargetElementID },
                                Elements: [{
                                    dictAttributes: {}, Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "docTypes", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" },
                                    { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [],
                                    Name: "extensionElements", "prefix": "", IsValueInCDATAFormat: false, Value: ""
                                }], Children: [], Name: "messageFlow", prefix: "", IsValueInCDATAFormat: false, Value: ""
                            };
                            lineobj.ShapeModel = MessageFlowModel;
                        }
                    }
                    if (lineobj != null) {
                        scope.$apply(function () {
                            scope.objData.lstShapes.push(lineobj);
                            AddLineArray(scope.svgElement, lineobj, scope);
                        });
                        //generateLine(scope.svgElement, lineobj);
                    }
                }
            }
        }
    }
    for (var i = 0; i < scope.svgElement.childNodes.length; i++) {
        if (scope.svgElement.childNodes[i].id == "dottedline") {
            scope.svgElement.removeChild(scope.svgElement.childNodes[i]);
            break;
        }
    }
    scope.sourceElementOnMouseHover = null;
    scope.targetElementOnMouseHover = null;
}


function getgateWaydirectionbasedOnEdges(ElementId, scope, param) {
    var outEdgesCount = 0;
    var inEdgesCount = 0;
    var direction = "";
    for (var i = 0; i < scope.objData.lstShapes.length; i++) {
        if (scope.objData.lstShapes[i].ShapeName == 'BPMNEdge') {
            if (scope.objData.lstShapes[i].SourceElement == ElementId) {
                outEdgesCount++;
            } else if (scope.objData.lstShapes[i].TargetElement == ElementId) {
                inEdgesCount++;
            }
        }
    }

    if (param == 'In') {
        inEdgesCount = inEdgesCount + 1;
    } else if (param == 'Out') {
        outEdgesCount = outEdgesCount + 1;
    }

    if (inEdgesCount > 1 && outEdgesCount > 1) {
        direction = "Mixed";
    } else if ((inEdgesCount > 1) && (inEdgesCount > outEdgesCount)) {
        direction = "Converging";
    } else if ((outEdgesCount > 1) && (outEdgesCount > inEdgesCount)) {
        direction = "Diverging";
    } else {
        direction = "Unspecified";
    }
    return direction;
}

function IsValidShapeForAssociation(srcElement, tarElement) {
    var bool = false;
    if (srcElement.ShapeType == "task" || srcElement.ShapeType == "userTask" || srcElement.ShapeType == "serviceTask" || srcElement.ShapeType == "businessRuleTask" || srcElement.ShapeType == "manualTask" || srcElement.ShapeType == "callActivity" ||
        tarElement.ShapeType == "task" || tarElement.ShapeType == "userTask" || tarElement.ShapeType == "serviceTask" || tarElement.ShapeType == "businessRuleTask" || tarElement.ShapeType == "manualTask" || tarElement.ShapeType == "callActivity") {
        bool = true;
    }
    return bool;
}


function CheckIsValidFlowNodeOrNot(srcElement, tarElement) {
    var isValidNode = false;
    if ((srcElement.ShapeType == "startEvent" || tarElement.ShapeType == "startEvent") && (srcElement.ShapeType != "participant" && tarElement.ShapeType != "participant")) {
        if (srcElement.ShapeType != tarElement.ShapeType) {
            if (srcElement.ShapeType == "startEvent") {
                if (tarElement.processRef == srcElement.processRef) {
                    if (tarElement.ShapeType != "dataObjectReference" && tarElement.ShapeType != "dataStoreReference") {
                        isValidNode = true;
                    }
                }
            }
            else {
                if (tarElement.processRef != srcElement.processRef) {
                    if (srcElement.ShapeType == "participant" || srcElement.ShapeType == "task" || srcElement.ShapeType == "serviceTask" || srcElement.ShapeType == "userTask"
                        || srcElement.ShapeType == "businessRuleTask" || srcElement.ShapeType == "callActivity" || srcElement.ShapeType == "manualTask") {
                        isValidNode = true;
                    }
                }
            }
        }
    }
    else if ((srcElement.ShapeType == "endEvent" || tarElement.ShapeType == "endEvent") && (srcElement.ShapeType != "participant" && tarElement.ShapeType != "participant")) {
        if (srcElement.ShapeType != tarElement.ShapeType) {
            if (srcElement.ShapeType == "endEvent") {
                if (tarElement.processRef != srcElement.processRef) {
                    if (tarElement.ShapeType == "participant" || tarElement.ShapeType == "task" || tarElement.ShapeType == "serviceTask" || tarElement.ShapeType == "userTask"
                         || tarElement.ShapeType == "businessRuleTask" || tarElement.ShapeType == "callActivity" || tarElement.ShapeType == "manualTask") {
                        isValidNode = true;
                    }
                }
            }
            else {
                if (tarElement.processRef == srcElement.processRef) {
                    if (srcElement.ShapeType != "dataObjectReference" && srcElement.ShapeType != "dataStoreReference") {
                        isValidNode = true;
                    }
                }
            }
        }
    }
    else if ((srcElement.ShapeType == "dataStoreReference" || tarElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference" || tarElement.ShapeType == "dataObjectReference") && (srcElement.ShapeType != "participant" && tarElement.ShapeType != "participant")) {
        if (srcElement.ShapeType == "dataStoreReference" || srcElement.ShapeType == "dataObjectReference") {
            if (tarElement.ShapeType == "participant" || tarElement.ShapeType == "task" || tarElement.ShapeType == "userTask" || tarElement.ShapeType == "serviceTask" || tarElement.ShapeType == "businessRuleTask"
                 || tarElement.ShapeType == "manualTask" || tarElement.ShapeType == "callActivity") {
                isValidNode = true;
            }
        }
        else {
            if (srcElement.ShapeType == "participant" || srcElement.ShapeType == "task" || srcElement.ShapeType == "userTask" || srcElement.ShapeType == "serviceTask" || srcElement.ShapeType == "businessRuleTask"
                             || srcElement.ShapeType == "manualTask" || srcElement.ShapeType == "callActivity") {
                isValidNode = true;
            }
        }
    }
    else if (srcElement.ShapeType == "participant" || tarElement.ShapeType == "participant") {
        if (srcElement.processRef != tarElement.processRef) {
            isValidNode = true;
        }
    }
    else {
        isValidNode = true;
    }

    return isValidNode;
}

