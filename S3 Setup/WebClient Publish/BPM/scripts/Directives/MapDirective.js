function bpmUsertaskProcessMapPropertiesController() {
    this.$onInit = function () {
        if (this.model && this.model.Elements) {
            function filterExtraSetting(x) {
                return x.Name == "extraSettings";
            }
            var extraSettings = this.model.Elements.filter(filterExtraSetting);
            if (extraSettings && extraSettings.length > 0) {
                this.model.extraSettingsModel = extraSettings[0];
                if (!this.model.extraSettingsModel.dictAttributes.sfwCanExecuteBpmTaskOnSoftErrors) {
                    this.model.extraSettingsModel.dictAttributes.sfwCanExecuteBpmTaskOnSoftErrors = "True";
                }
            }
        }
        if (this.mapvariablesmodel) {
            this.dateTypeVariables = this.mapvariablesmodel.Elements.filter(function (itm) { return itm.dictAttributes.dataType === "datetime"; }).map(function (itm) { return itm.dictAttributes.id; });
        }
    };
}

function intermediateCatchEventMapPropertiesController($rootScope, $filter, $getQueryparam) {
    this.initialize = function () {
        this.selected = { valueSource: undefined, eventType: undefined };
        this.durationModel = undefined;
        if (this.mapvariablesmodel) {
            this.queryParametersValues = $getQueryparam.getMapVariableIds(this.mapvariablesmodel.Elements);
        }
        var items = $filter('filter')(this.model.Elements, { Name: "timerEventDefinition" });
        if (items && items.length > 0) {
            this.selected.eventType = "Timer";
            var items = $filter('filter')(items[0].Elements, { Name: "timeDuration" });
            if (items && items.length > 0) {
                var items = $filter('filter')(items[0].Elements, { Name: "extensionElements" });
                if (items && items.length > 0) {
                    var items = $filter('filter')(items[0].Elements, { Name: "Duration" });
                    if (items && items.length > 0) {
                        this.durationModel = items[0];
                        this.selected.valueSource = this.durationModel.dictAttributes.sfwValueSource;
                    }
                }
            }
        }
        var items = $filter('filter')(this.model.Elements, { Name: "messageEventDefinition" });
        if (items && items.length > 0) {
            this.selected.eventType = "Message";
        }
        else if (this.selected.eventType == undefined) {
            this.selected.eventType = "";
        }
    };
    this.onEventTypeChanged = function () {
        if (this.selected.eventType == "Message") {
            this.addMessageEvent();
            this.ChangeImageBasedOnType("Message");
        }
        else if (this.selected.eventType == "Timer") {
            this.addTimerEvent();
            this.ChangeImageBasedOnType("Timer");
        }
        else {
            this.addDefaultEvent();
            this.ChangeImageBasedOnType("Default");
        }
        this.initialize();
    };
    this.removeMessageEvent = function () {
        var items = $filter('filter')(this.model.Elements, { Name: "messageEventDefinition" });
        if (items && items.length > 0) {
            $rootScope.DeleteItem(items[0], this.model.Elements, null);
        }
    };
    this.removeDefaultEvent = function () {
        var items = $filter('filter')(this.model.Elements, { Name: "extensionElements" });
        if (items && items.length > 0) {
            $rootScope.DeleteItem(items[0], this.model.Elements, null);
        }
    };
    this.ChangeImageBasedOnType = function (param) {
        ChangeCatchEventImageBasedonType(this.selectedelemforresize, param);
    };
    this.addMessageEvent = function () {
        $rootScope.UndRedoBulkOp("Start");
        this.removeTimerEvent();
        this.removeDefaultEvent();
        var messageModel = getMessageEventModel();
        $rootScope.PushItem(messageModel, this.model.Elements);
        $rootScope.UndRedoBulkOp("End");
    };
    this.removeTimerEvent = function () {
        var items = $filter('filter')(this.model.Elements, { Name: "timerEventDefinition" });
        if (items && items.length > 0) {
            $rootScope.DeleteItem(items[0], this.model.Elements, null);
        }
    };
    this.addDefaultEvent = function () {
        $rootScope.UndRedoBulkOp("Start");
        this.removeTimerEvent();
        this.removeMessageEvent();
        var extElements = { prefix: "", Name: "extensionElements", Value: "", dictAttributes: {}, Elements: [], Children: [] };
        var isSyncModel = { prefix: "sbpmn", Name: "isSynchronous", Value: "False", dictAttributes: {}, Elements: [], Children: [] };
        extElements.Elements.push(isSyncModel);
        var defaultModel = extElements;
        $rootScope.PushItem(defaultModel, this.model.Elements);
        $rootScope.UndRedoBulkOp("End");
    };
    this.addTimerEvent = function () {
        $rootScope.UndRedoBulkOp("Start");
        this.removeMessageEvent();
        this.removeDefaultEvent();
        var timerModel = getTimerEventModel();
        $rootScope.PushItem(timerModel, this.model.Elements);
        $rootScope.UndRedoBulkOp("End");
    };
    this.calculateValue = function () {
        if (this.durationModel) {
            var days = parseInt(this.durationModel.dictAttributes.Days) || 0;
            var hours = parseInt(this.durationModel.dictAttributes.Hours) || 0;
            var minutes = parseInt(this.durationModel.dictAttributes.Minutes) || 0;
            var seconds = parseInt(this.durationModel.dictAttributes.Seconds) || 0;

            var value = (seconds * 10000000) + (minutes * 60 * 10000000) + (hours * 60 * 60 * 10000000) + (days * 24 * 60 * 60 * 10000000);
            $rootScope.EditPropertyValue(this.durationModel.dictAttributes.sfwParameterValue, this.durationModel.dictAttributes, "sfwParameterValue", value);
        }
    };
    this.onValueSourceChanged = function () {
        if (this.durationModel) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(this.durationModel.dictAttributes.sfwValueSource, this.durationModel.dictAttributes, "sfwValueSource", this.selected.valueSource);

            if (this.selected.valueSource == "Parameter") {
                $rootScope.EditPropertyValue(this.durationModel.dictAttributes.Days, this.durationModel.dictAttributes, "Days", "");
                $rootScope.EditPropertyValue(this.durationModel.dictAttributes.Hours, this.durationModel.dictAttributes, "Hours", "");
                $rootScope.EditPropertyValue(this.durationModel.dictAttributes.Minutes, this.durationModel.dictAttributes, "Minutes", "");
                $rootScope.EditPropertyValue(this.durationModel.dictAttributes.Seconds, this.durationModel.dictAttributes, "Seconds", "");
            }

            $rootScope.EditPropertyValue(this.durationModel.dictAttributes.sfwParameterValue, this.durationModel.dictAttributes, "sfwParameterValue", "");
            $rootScope.UndRedoBulkOp("End");
        }
    };
    this.initialize();
}

function messageflowMapPropertiesController($filter, $rootScope) {
    var scope = getCurrentFileScope();
    var extensionElements = $filter('filter')(this.model.Elements, { Name: "extensionElements" });
    if (extensionElements && extensionElements.length > 0) {
        this.extensionElementModel = extensionElements[0];
    }
    this.selectedShape = scope.selectedShape;
    if (this.extensionElementModel) {
        var items = $filter('filter')(this.extensionElementModel.Elements, { Name: "docTypes" }, true);
    }
    if (items && items.length > 0) {
        this.docTypesModel = items[0];
    }

    this.addDocType = function () {
        var selectedDocType = this.obj.selectedDocType;
        if (!this.docTypesModel.Elements.some(function (x) { return x.Value == selectedDocType.Code; })) {
            var newDocTypeModel = { prefix: "sbpmn", Name: "docType", Value: this.obj.selectedDocType.Code, dictAttributes: {}, Elements: [], Children: [] };

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.PushItem(newDocTypeModel, this.docTypesModel.Elements);
            $rootScope.PushItem(this.obj.selectedDocType.CodeDescription, this.obj.docTypes);

            $rootScope.UndRedoBulkOp("End");

        }
    };
    this.canAddDocType = function () {
        var selectedDocType = this.obj.selectedDocType;
        return (selectedDocType != undefined && !this.docTypesModel.Elements.some(function (x) { return x.Value == selectedDocType.Code; }));
    };
    this.deleteDocType = function () {
        var items = $filter('filter')(this.docTypes, { CodeDescription: this.obj.selectedDocTypeFromGrid }, true);
        $rootScope.UndRedoBulkOp("Start");

        if (items && items.length > 0) {
            var items = $filter('filter')(this.docTypesModel.Elements, { Value: items[0].Code }, true);
            if (items && items.length > 0) {
                $rootScope.DeleteItem(items[0], this.docTypesModel.Elements, null);

                //var index = this.docTypesModel.Elements.indexOf(items[0]);
                //if (index > -1) {
                //    this.docTypesModel.Elements.splice(index, 1);
                //}
            }
        }
        $rootScope.DeleteItem(this.obj.selectedDocTypeFromGrid, this.obj.docTypes, null);
        $rootScope.UndRedoBulkOp("End");

        //var index = this.obj.docTypes.indexOf(this.obj.selectedDocTypeFromGrid);
        //if (index > -1) {
        //    this.obj.docTypes.splice(index, 1);
        //}

        this.obj.selectedDocTypeFromGrid = undefined;
    };
    this.canDeleteDocType = function () {
        return (this.obj.selectedDocTypeFromGrid && this.obj.selectedDocTypeFromGrid.trim().length > 0);
    };
    this.selectDocType = function (doctype) {
        this.obj.selectedDocTypeFromGrid = doctype;
    };
    this.onMessageKindChange = function (messageKind) {
        if (messageKind == "initiating" || messageKind == "non_initiating") {

            if (this.model.dictAttributes.messageRef && this.model.dictAttributes.messageRef.trim().length > 0) {
                scope.selectedShape.MessageVisibleKind = messageKind;
                var messageElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(this.model.dictAttributes.messageRef, scope, "Element");
                ChangeImageOfTheShape(messageKind, scope, messageElement);
            }
            else {
                this.addMessage(messageKind);
            }
        }
        else {
            this.removeMessage();
        }
    };
    this.addMessage = function (messageKind) {
        var messageRef = generateUUID();
        var shapeId = generateUUID();
        var ShapeCoordinates = getShapeCoordinatesBasedOnResizerLine(scope);
        this.model.dictAttributes.messageRef = messageRef;
        var MessageShapeDetails = {
            Height: 20, Width: 20, Left: ShapeCoordinates.Left, Top: ShapeCoordinates.Top, LabelHeight: 0, LabelWidth: 0, LabelLeft: 0, LabelTop: 0,
            ShapeName: "BPMNShape", ShapeType: "message", processRef: "", lstWayPoints: [], Text: "", Id: messageRef, ShapeId: shapeId
        };
        scope.selectedShape.MessageVisibleKind = messageKind;
        scope.selectedShape.MessageReference = messageRef;
        var messageModel = { prefix: "", Name: "message", Value: "", dictAttributes: { id: this.model.dictAttributes.messageRef }, Elements: [], children: [] };
        MessageShapeDetails.ShapeModel = messageModel;
        scope.objData.lstShapes.push(MessageShapeDetails);
        drawInitiatingorNonInitiatingMessage(scope.svgElement, MessageShapeDetails, scope);
    };
    this.removeMessage = function () {
        if (this.model.dictAttributes.messageRef && this.model.dictAttributes.messageRef.trim().length > 0) {
            var objShape = GetElementProcessIdorElementBasedOnIdorDeleteElement(this.model.dictAttributes.messageRef, scope, "Delete");
            removeElementFromSvg(objShape, scope);
            scope.selectedShape.MessageVisibleKind = "";
            scope.selectedShape.MessageReference = "";
            this.model.dictAttributes.messageRef = "";
        }
    };
    this.obj = { docTypes: [], selectedDocType: undefined, selectedDocTypeFromGrid: "" };

    /* changed this code for Message flow issue(Bug 8023), was not retaining doc type value.*/
    if (this.docTypesModel && this.docTypesModel.Elements.length) {
        for (var i = 0; i < this.docTypesModel.Elements.length; i++) {
            if (this.docTypes && this.docTypes.length) {
                for (var j = 0; j < this.docTypes.length; j++) {
                    if (this.docTypesModel.Elements[i].Value == this.docTypes[j].Code) {
                        this.obj.docTypes.push(this.docTypes[j].CodeDescription);
                        break;
                    }
                }
            }
        }
    }
}

app.component('bpmUsertaskShapeMapProperties', {
    templateUrl: 'BPM/views/Map/UserTaskMapProperties.html',
    bindings: {
        model: '<',
        mapvariablesmodel: '<',
        isDisabledforVersioning: '<?'
    },
    controller: bpmUsertaskProcessMapPropertiesController
});

app.component('bpmProcessShapeMapProperties', {
    templateUrl: 'BPM/views/Map/ProcessMapProperties.html',
    bindings: {
        model: '<',
        processTypes: '<',
        isDisabledforVersioning: '<?'
    },
    controller: bpmUsertaskProcessMapPropertiesController
});

function bpmCallactivityMapPropertiesController($rootScope, $scope) {
    var ctrl = this;
    this.navigateToCallElement = function (calledElement) {
        calledElement = calledElement ? calledElement : "";
        var arrCalledElement = calledElement.split(".");
        if ((Object.prototype.toString.call(arrCalledElement) === '[object Array]') && arrCalledElement.length > 1) {
            $.connection.hubMain.server.navigateToFile(arrCalledElement[0], "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };
    this.openVersionCallActivity = function () {
        if (ctrl.calledCaseBpmModel) {
            var dialogScope = $scope.$new();
            dialogScope.calledCaseBpmModel = ctrl.calledCaseBpmModel;
            dialogScope.closeDialog = function () {
                if (dialogScope.dialog && dialogScope.dialog.close) {
                    dialogScope.dialog.close();
                }
            }
            dialogScope.dialog = $rootScope.showDialog(dialogScope, "", "BPM/views/BPMVersion.html", { width: 1000, height: 600, showclose: true, closeOnEscape: true });
        }
    };
    this.selectCallElement = function () {
        if (!ctrl.isDisabledforVersioning) {
            var selectCallElementScope = $scope.$new();
            function successCallbackReusableBpm(data) {
                function filter(item) { return item.BpmName != $rootScope.currentopenfile.file.FileName; }
                if (Object.prototype.toString.call(data) === '[object Array]') {
                    function setResusableBpm() {
                        selectCallElementScope.reUsableBpm = data.filter(filter);
                    }
                    selectCallElementScope.$evalAsync(setResusableBpm);
                }
            }
            $.connection.hubBPMN.server.getReusableBpmns().done(successCallbackReusableBpm);
            selectCallElementScope.selectProcess = function (element, process) {
                selectCallElementScope.selectedProcessName = element.BpmName + "." + process.ProcessID;
            };
            selectCallElementScope.closeDialog = function () {
                if (selectCallElementScope.dialog) {
                    selectCallElementScope.dialog.close();
                }
            };
            selectCallElementScope.dialog = $rootScope.showDialog(selectCallElementScope, "Select Process", 'BPM/views/Map/CallElementSearchTemplate.html', { height: 400, width: 800 });
            selectCallElementScope.onOKClick = function () {
                if (selectCallElementScope.selectedProcessName) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(ctrl.model.dictAttributes.calledElement, ctrl.model.dictAttributes, "calledElement", selectCallElementScope.selectedProcessName);
                    function filterExtension(x) {
                        return x.Name == "extensionElements";
                    }
                    var extensionElement = ctrl.model.Elements.filter(filterExtension);
                    var processBpmName = selectCallElementScope.selectedProcessName.split('.');
                    ctrl.setCalledelementparameters({ mapName: processBpmName[0], extensionElementModel: extensionElement ? extensionElement[0] : null });
                    $rootScope.UndRedoBulkOp("End");
                }
                selectCallElementScope.closeDialog();
            };
        }
    };
}

app.component('bpmCallactivityShapeMapProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        setCalledelementparameters: '&',
        calledCaseBpmModel: '<'
    },
    templateUrl: 'BPM/views/Map/CallActivityMapProperties.html',
    controller: ["$rootScope", "$scope", bpmCallactivityMapPropertiesController]
});

app.component('processusertaskcommonproperties', {
    bindings: {
        model: '=',
        showseconds: '<',
        istimerevent: "<",
        processtypes: "<",
        onchangeDuration: '&',
        datetypevariables: '<',
        isDisabledforVersioning: '<?'
    },
    templateUrl: 'BPM/views/Map/ProcessUserTaskMapProperties.html'
});

app.component('bpmIntermediateCatchEventMapProperties', {
    bindings: {
        model: '<',
        mapvariablesmodel: '<',
        isDisabledforVersioning: '<?',
        selectedelemforresize: '<',
        processtypes: '<'
    },
    templateUrl: 'BPM/views/Map/IntermediateCatchEventMapProperties.html',
    controller: ["$rootScope", "$filter", "$getQueryparam", intermediateCatchEventMapPropertiesController]
});


app.component('bpmMessageflowMapProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        docTypes: '<'
    },
    templateUrl: 'BPM/views/Map/MessageFlowMapProperties.html',
    controller: ["$filter", "$rootScope", messageflowMapPropertiesController]
});

function bpmTextAnnotationMapPropertiesController($rootScope, $scope) {
    var ctrl = this;
    this.$onInit = function () {
        var textNode = ctrl.shapeModel.Elements.filter(function (x) { return x.Name == "text"; });
        if (textNode && textNode.length > 0) {
            ctrl.shapeModel.textModel = textNode[0];
        }
    };
}


app.component("bpmTextAnnotationProperties", {
    bindings: {
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        onChangeText: "&"
    },
    replace: true,
    templateUrl: "BPM/views/Map/TextAnnotationProperties.html",
    controller: ["$rootScope", "$scope", bpmTextAnnotationMapPropertiesController]
});

function bpmInclusiveGatewayController($scope, $filter) {
    var ctrl = this;
    if (ctrl.shapeModel.Name == "inclusiveGateway" && ctrl.shapeModel.dictAttributes.gatewayDirection == "Converging") {
        var extensionElements = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extensionElements" });
        if (extensionElements && extensionElements.length > 0) {
            $scope.extensionElementModel = extensionElements[0];
        }
        $scope.SyncWithModel = null;


        var filescope = getCurrentFileScope();

        //Set Initiator On changing the sync Id
        $scope.onChangeDivergingGateWay = function () {
            if ($scope.SyncWithModel) {

                this.selectedShape = filescope.selectedShape;
                if (this.selectedShape && this.selectedShape.ShapeType == "inclusiveGateway") {
                    var targetPoints = getTargetPointsofElement(this.selectedShape, filescope);
                    if (targetPoints && targetPoints.length > 0) {
                        for (var i = 0; i < targetPoints.length; i++) {
                            var isFound = false;
                            if ($scope.SyncWithModel.Value) {
                                var obj = {};
                                if (targetPoints[i].SourceElement != $scope.SyncWithModel.Value) {
                                    var objShapeElement = GetElementProcessIdorElementBasedOnIdorDeleteElement(targetPoints[i].SourceElement, filescope, "Element");
                                    obj.initiationElementId = "";
                                    isFound = $scope.SetInitiatorForInicomingEdge($scope.SyncWithModel.Value, objShapeElement, obj);
                                } else {
                                    isFound = true;
                                    obj.initiationElementId = targetPoints[i].Id;
                                }
                            }
                            if (isFound) {
                                var isExtensionElementsFound = false;
                                var extensionElementsModel = undefined;
                                if (targetPoints[i].ShapeModel.Elements && targetPoints[i].ShapeModel.Elements.length > 0) {
                                    for (var j = 0; j < targetPoints[i].ShapeModel.Elements.length; j++) {
                                        if (targetPoints[i].ShapeModel.Elements[j].Name = "extensionElements") {
                                            extensionElementsModel = targetPoints[i].ShapeModel.Elements[j];
                                            isExtensionElementsFound = true;
                                            break;
                                        }
                                    }
                                }

                                if (!isExtensionElementsFound) {
                                    extensionElementsModel = {
                                        dictAttributes: {}, Elements: [{
                                            dictAttributes: {}, Elements: [{ dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "leftside", prefix: "", IsValueInCDATAFormat: false, Value: "" },
                                            { dictAttributes: { type: "Variable" }, Elements: [], Children: [], Name: "rightside", prefix: "", IsValueInCDATAFormat: false, Value: "" }], Children: [], Name: "conditionExpression", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                        },
                                        { dictAttributes: {}, Elements: [], Children: [], Name: "isSynchronous", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "False" }], Children: [], Name: "extensionElements", prefix: "", IsValueInCDATAFormat: false, Value: ""
                                    };
                                    targetPoints[i].ShapeModel.Elements.push(extensionElementsModel);
                                }
                                var InitiatorModel = undefined;
                                var isInitiatorFound = false;
                                if (extensionElementsModel) {
                                    for (k = 0; k < extensionElementsModel.Elements.length; k++) {
                                        if (extensionElementsModel.Elements[k].Name == "initiator") {
                                            isInitiatorFound = true;
                                            InitiatorModel = extensionElementsModel.Elements[k];
                                            break;
                                        }
                                    }
                                }
                                if (!isInitiatorFound) {
                                    InitiatorModel = { prefix: "sbpmn", Name: "initiator", Value: "", dictAttributes: {}, Elements: [], Children: [] };
                                    extensionElementsModel.Elements.push(InitiatorModel);
                                }
                                InitiatorModel.Value = obj.initiationElementId;
                            } else {
                                $scope.RemoveInitiatorModel(targetPoints[i].ShapeModel);
                            }
                        }
                    }
                }
            }
        };

        $scope.RemoveInitiatorModel = function (shapeModel) {
            for (var i = 0; i < shapeModel.Elements.length; i++) {
                if (shapeModel.Elements[i].Name == "extensionElements") {
                    for (j = 0; j < shapeModel.Elements[i].Elements.length; j++) {
                        if (shapeModel.Elements[i].Elements[j].Name == "initiator") {
                            shapeModel.Elements[i].Elements.splice(j, 1);
                            break;
                        }
                    }
                    break;
                }
            }
        };

        $scope.SetInitiatorForInicomingEdge = function (DivergingGateWayID, objShape, obj) {
            var isFound = false;
            var targetPoints = getTargetPointsofElement(objShape, filescope);
            if (targetPoints && targetPoints.length > 0) {
                for (var i = 0; i < targetPoints.length; i++) {
                    if (targetPoints[i].SourceElement != DivergingGateWayID) {
                        var objShape = GetElementProcessIdorElementBasedOnIdorDeleteElement(targetPoints[i].SourceElement, filescope, "Element");
                        isFound = $scope.SetInitiatorForInicomingEdge(DivergingGateWayID, objShape, obj);
                    } else {
                        isFound = true;
                        obj.initiationElementId = targetPoints[i].Id;
                        break;
                    }
                }
            }
            return isFound;
        };
        // get lstDivergingandmixed gateways of inclusive gateway
        $scope.lstDivergingGateWays = ctrl.getDivergingGatewaysCallback();
        //getting sync model
        for (var j = 0; j < $scope.extensionElementModel.Elements.length; j++) {
            if ($scope.extensionElementModel.Elements[j].Name == "syncwith") {
                $scope.SyncWithModel = $scope.extensionElementModel.Elements[j];
                if ($scope.lstDivergingGateWays.length == 0) {
                    $scope.extensionElementModel.Elements.splice(j, 1);
                } else if ($scope.SyncWithModel.Value) {
                    for (var i = 0; i < $scope.lstDivergingGateWays.length; i++) {
                        var isFound = false;
                        if ($scope.lstDivergingGateWays[i].ID == $scope.SyncWithModel.Value) {
                            isFound = true;
                            break;
                        }
                        if (!isFound) {
                            $scope.SyncWithModel.Value = "";
                        }
                    }
                }
                break;
            }
        }
        // Add sync Model if it is null
        if ($scope.lstDivergingGateWays.length > 0 && $scope.SyncWithModel == null) {
            var isSyncWithModel = { prefix: "sbpmn", Name: "syncwith", Value: "", dictAttributes: {}, Elements: [], Children: [] };
            $scope.extensionElementModel.Elements.push(isSyncWithModel);
            $scope.SyncWithModel = isSyncWithModel;
        }
    }
}
app.component("bpmInclusiveGatewayProperties", {
    bindings: {
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        getDivergingGatewaysCallback: "&",
    },
    templateUrl: "BPM/views/Map/InclusiveGatewayProperties.html",
    controller: ["$scope", "$filter", bpmInclusiveGatewayController],
});

