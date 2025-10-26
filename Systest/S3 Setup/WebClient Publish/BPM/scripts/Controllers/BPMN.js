app.controller('BPMNController', ["$scope", "$rootScope", "$filter", "$getQueryparam", "$getMultiIndexofArray", "$getEnitityRule", "$getEnitityXmlMethods", "$cssFunc", "$EntityIntellisenseFactory", "$NavigateToFileService", "$getEnitityObjectMethods", "hubcontext", function ($scope, $rootScope, $filter, $getQueryparam, $getMultiIndexofArray, $getEnitityRule, $getEnitityXmlMethods, $cssFunc, $EntityIntellisenseFactory, $NavigateToFileService, $getEnitityObjectMethods, hubcontext) {
    $rootScope.IsLoading = true;
    $scope.currentfile = $rootScope.currentopenfile.file;
    var objMapVarible = function () {
        return { Children: [], Elements: [], Name: "variable", Value: "", dictAttributes: {}, prefix: "sbpmn" };
    };
    $scope.BPMNContextMenu = [
        ['Edit SVG Bounds', function ($itemScope) {
            $scope.EditSVGheightandwidth();
        }, function ($itemScope) {
            return true;
        }]];
    $scope.init = function () {
        $scope.selectedDesignSource = false;
        $scope.initVariables();
        $scope.initSvgVariables();
        hubMain.server.getModel($rootScope.currentopenfile.file.FilePath, $rootScope.currentopenfile.file.FileType).done(function (data) {
            if (data) {
                $scope.receiveBpmnModel(data);
            }
            else {
                $rootScope.closeFile($scope.currentfile.FileName);
            }
        });
    };
    $scope.initVariables = function () {
        $scope.isDirty = false;
        $scope.activeTab = "MAP";
        $scope.associationActiveTab = "FORM";
        $scope.datatypes = ['bool', 'datetime', 'decimal', 'double', 'float', 'int', 'long', 'short', 'string', 'Object'];
        $scope.types = ['local', 'in', 'out'];
        $scope.Requiredtypes = ['', 'true', 'false'];
        $scope.dvtypes = ['Query', 'Constant'];
        $scope.mandatoryElementIds = ['PersonId', 'OrgId', 'ReferenceId', 'Source', 'InstanceInfo1', 'InstanceInfo2'];
        $scope.queryParameterallowedValues = ['PersonId', 'OrgId', 'ReferenceId', 'Source'];
        $scope.currentUserTaskData = { enterFormData: { entityID: "", formTitle: "", controlTree: [], initialLoadModel: {}, visibleRules: [] }, reEnterFormData: { entityID: "", formTitle: "", controlTree: [], entities: [], buttons: [], initialLoadModel: {}, visibleRules: [] }, selectedEntity: "BpmActivityInstance", formList: [] };
        $scope.selectedCallActivityData = { reUsableBpm: [], CalledMapVariable: [] };
        $scope.operatorsList = ['GreaterThan', 'GreaterThanEqual', 'Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'StartsWith', 'EndsWith'];
        $scope.TargetSiteList = undefined;
        $scope.isExecutionTabLoaded = false;
        $scope.lstMultipleSelectedShapes = [];
        $scope.isShowTopandBottomAlignButtons = true;
        $scope.IsDisabledforVersioning = false;
        $scope.selectedElementID = undefined;
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

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            $rootScope.IsLoading = true;
            $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
            var objreturn1;
            //$scope.addExtraFieldsDataInToMainModel();
            objreturn1 = $scope.BPMNModel;
            var strobj = JSON.stringify(objreturn1);

            var nodeId = [];

            if ($scope.activeTab == 'MAP' && $scope.selectedShape) {
                nodeId[0] = $scope.selectedShape.ShapeModel.dictAttributes.id;
            }
            else if ($scope.activeTab == 'VARIABLES' && $scope.selectedVariable) {
                nodeId[0] = $scope.selectedVariable.dictAttributes.id;
            }

            if (strobj.length < 32000) {
                hubMain.server.getBPMSourceXmlObject(strobj, nodeId);
            }
            else {
                var strpacket = "";
                var lstDataPackets = [];
                var count = 0;
                for (var i = 0; i < strobj.length; i++) {
                    count++;
                    strpacket = strpacket + strobj[i];
                    if (count == 32000) {
                        count = 0;
                        lstDataPackets.push(strpacket);
                        strpacket = "";
                    }
                }
                if (count != 0) {
                    lstDataPackets.push(strpacket);
                }

                SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Design-Source-BPM", nodeId);
            }
            $scope.receivebpmsourcexml = function (xmlstring, lineno) {
                $scope.$apply(function () {
                    var ID = $scope.currentfile.FileName;
                    setDataToEditor($scope, xmlstring, lineno, ID);
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                });
            };
        }
    };
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            //$scope.removeExtraFieldsDataInToMainModel();
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;

                if (xmlstring.length < 32000) {
                    hubMain.server.getBPMDesignXmlString(xmlstring, lineno);
                }
                else {
                    var lineNumber = [];
                    if (lineno > 0) {
                        lineNumber[0] = lineno;
                    }
                    else {
                        lineNumber[0] = 1;
                    }
                    var strpacket = "";
                    var lstDataPackets = [];
                    var count = 0;
                    for (var i = 0; i < xmlstring.length; i++) {
                        count++;
                        strpacket = strpacket + xmlstring[i];
                        if (count == 32000) {
                            count = 0;
                            lstDataPackets.push(strpacket);
                            strpacket = "";
                        }
                    }
                    if (count != 0) {
                        lstDataPackets.push(strpacket);
                    }

                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Source-Design-BPM", lineNumber);
                }

                $scope.receivebpmdesignxmlstring = function (data, nodeId) {
                    $scope.$evalAsync(function () {
                        $scope.selectedDesignSource = false;
                    });

                    //$scope.receiveBpmnModel(data);

                    //Commented following code, because source to design synchronization is hold for now.
                    //$scope.$evalAsync(function () { });
                    //var selectedShape = nodeId;
                    //var selectElement = $scope.objData.lstShapes.filter(function (item) { return item.ShapeId == selectedShape });
                };
            }
        }
    };

    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };
    $scope.getMaxHeightAndWidthofSvg = function (lstShapes) {
        var objMaxBounds = {};
        objMaxBounds.Height = 0;
        objMaxBounds.Width = 0;
        for (var i = 0; i < lstShapes.length; i++) {
            if (lstShapes[i].ShapeName == "BPMNShape") {
                if (objMaxBounds.Height < lstShapes[i].Top + lstShapes[i].Height) {
                    objMaxBounds.Height = lstShapes[i].Top + lstShapes[i].Height;
                }
                if (objMaxBounds.Width < lstShapes[i].Left + lstShapes[i].Width) {
                    objMaxBounds.Width = lstShapes[i].Left + lstShapes[i].Width;
                }
            }
        }
        return objMaxBounds;
    };
    $scope.receiveBpmnModel = function (data) {
        $scope.$apply(function () {
            $rootScope.IsLoading = false;
            $scope.objBPMExtraFields = undefined;
            $scope.BpmFileDetails = $rootScope.currentopenfile.file;
            $scope.BPMNModel = data;
            $scope.objData = $scope.BPMNModel;
            if ($scope.BPMNModel.Type != "BPMTemplate") {
                $scope.mapVariablesModel = $filter('filter')($scope.BPMNModel.ExtensionElementsModel.Elements, { Name: "mapVariables" })[0];
            }
            $scope.svgElement = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG")[0];
            var objSvgBounds = {};
            objSvgBounds.Height = 2500;
            objSvgBounds.Width = 2500;

            $scope.objSvgLimits = $scope.getMaxHeightAndWidthofSvg($scope.BPMNModel.lstShapes);
            if ($scope.objSvgLimits.Height && $scope.objSvgLimits.Height > objSvgBounds.Height) {
                objSvgBounds.Height = $scope.objSvgLimits.Height + 100;
            }
            if ($scope.objSvgLimits.Width && $scope.objSvgLimits.Width > objSvgBounds.Width) {
                objSvgBounds.Width = $scope.objSvgLimits.Width + 100;
            }

            $scope.svgElement.setAttribute("height", objSvgBounds.Height);
            $scope.svgElement.setAttribute("width", objSvgBounds.Width);
            $($scope.svgElement).bind("contextmenu", function (event) {
                showContextMenuForSVG(event);
            });
            var extraDetailsModel = null;
            if ($scope.BPMNModel.ExtensionElementsModel) {
                extraDetailsModel = $filter('filter')($scope.BPMNModel.ExtensionElementsModel.Elements, { Name: "extraDetails" })[0];
            }
            if (!extraDetailsModel && $scope.BPMNModel.Type != "BPMTemplate") {
                extraDetailsModel = {
                    objExtraData: null, dictAttributes: {}, Elements: [{ objExtraData: null, dictAttributes: {}, Elements: [], Children: [], Name: "editable", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "True" }],
                    Children: [], Name: "extraDetails", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                };
                $scope.BPMNModel.ExtensionElementsModel.Elements.push(extraDetailsModel);
            }
            var editableModel = null;
            if (extraDetailsModel) {
                editableModel = $filter('filter')(extraDetailsModel.Elements, { Name: "editable" })[0];
            }
            if (editableModel) {
                if (editableModel.Value && editableModel.Value.toLowerCase() == "true") {
                    $scope.IsMapEditable = true;
                } else {
                    $scope.IsMapEditable = false;
                }
            } else {
                $scope.IsMapEditable = true;
            }
            $scope.SetUniqueIdtoGradientsInsideSVG();
            $scope.FilterTheElementsBasedOnTypeAndLoadShapes($scope.objData.lstShapes);
            //Get extra fields data
            var tempExtraFields = $scope.BPMNModel.ExtraFields;
            if (tempExtraFields && tempExtraFields.length > 0) {
                $scope.objBPMExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {
                    }, Elements: []
                };
                for (var i = 0; i < tempExtraFields.length; i++) {
                    var objChild = {
                        Name: "ExtraField", Value: '', dictAttributes: {
                        }, Elements: []
                    };
                    objChild.dictAttributes.ID = tempExtraFields[i].ID;
                    objChild.dictAttributes.Value = tempExtraFields[i].Value;
                    objChild.dictAttributes.URL = tempExtraFields[i].URL;
                    $scope.objBPMExtraFields.Elements.push(objChild);
                }
                //$scope.removeExtraFieldsDataInToMainModel();
            }
            else {
                $scope.objBPMExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {
                    }, Elements: []
                };
            }

            $.connection.hubBPMN.server.getInitialData().done(function (data) {
                var lst = data;
                if (lst.length == 4) {
                    $scope.docTypes = lst[0];
                    $scope.UserRoles = lst[1];
                    $scope.lstBPMNRelatedCodeValues = lst[2];
                    $scope.lstGlobalParameters = lst[3];
                }
            });
            hubMain.server.getCodeValues("", "2035");
            $scope.ProcessTypes = [];
            $.connection.hubBPMN.server.getProcessTypes().done(function (result) {
                $scope.ProcessTypes = result;
            });

            if ($scope.mapVariablesModel) {
                $scope.queryParametersValues = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
            }

        });
    };


    $scope.receiveList = function (data) {
        $scope.TargetSiteList = data;
    };
    $scope.onDetailClick = function () {
        $scope.showMapDetails();
        ComponentsPickers.init();
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
                        if (objShape.ShapeName != "lane") {
                            CheckandAddExtrasettingsModel(objShape);
                        }
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
                        if (objShape.ShapeName != "callActivity") {
                            CheckandAddExtrasettingsModel(objShape);
                        }
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
        if ($scope.activeTab == "EXECUTION") {
            $scope.InitExecutionTab();
        }
        $scope.SetallSvgelementEnable(false);
        if ($scope.activeTab !== 'MAP') {
            $scope.SetallSvgelementDisable(true);
            $scope.SVGstateWrapper($scope.activeTab);
            removehovercirclefromsvgElement($scope);
            removeRearrangingLinePoints($scope);
            removeResizerAroundElement($scope);
        }
        else {
            $scope.SetallSvgelementDisable(false);
            if ($scope.mapVariablesModel) {
                $scope.queryParametersValues = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
            }
        }
    };

    $scope.InitExecutionTab = function () {
        if (!$scope.isExecutionTabLoaded) {
            $scope.isExecutionTabLoaded = true;
            $scope.BPMExceutionTabDetails = {};
            $scope.BPMExceutionTabDetails.CaseVersion = "";
            $scope.BPMExceutionTabDetails.CaseVersionID = "";
            $scope.BPMExceutionTabDetails.CaseVersionList = [];
            $scope.BPMExceutionTabDetails.PersonID = "";
            $scope.BPMExceutionTabDetails.OrgID = "";
            $scope.BPMExceutionTabDetails.Status = "";
            $scope.BPMExceutionTabDetails.FromDate = "";
            $scope.BPMExceutionTabDetails.ToDate = "";
            $scope.selectedCaseInstance = "";
            $scope.BPMExceutionTabDetails.lstPersonId = [];
            $scope.BPMExceutionTabDetails.lstOrgID = [];
            $scope.BPMExceutionTabDetails.lstStatusValue = [];
            $scope.SelectCaseResult = function (obj) {
                $scope.selectedCaseInstance = obj;
            };
            ComponentsPickers.init();
            $scope.RefreshCaseVesrsion = function () {
                $scope.BPMExceutionTabDetails.PersonID = "";
                $scope.BPMExceutionTabDetails.OrgID = "";
                $scope.BPMExceutionTabDetails.Status = "";
                $scope.BPMExceutionTabDetails.FromDate = "";
                $scope.BPMExceutionTabDetails.ToDate = "";
                $scope.onChangeCaseVersion();
            };
            $scope.onChangeCaseVersion = function () {
                if ($scope.BPMExceutionTabDetails.CaseVersionID) {
                    $scope.BPMExceutionTabDetails.lstPersonId = [];
                    $scope.BPMExceutionTabDetails.lstOrgID = [];
                    $scope.BPMExceutionTabDetails.lstStatusValue = [];
                    for (var i = 0; i < $scope.BPMExceutionTabDetails.CaseVersionList.length; i++) {
                        if ($scope.BPMExceutionTabDetails.CaseVersionID == $scope.BPMExceutionTabDetails.CaseVersionList[i].Version) {
                            $scope.BPMExceutionTabDetails.CaseVersion = $scope.BPMExceutionTabDetails.CaseVersionList[i];
                            break;
                        }
                    }
                    if ($scope.BPMExceutionTabDetails.CaseVersion) {
                        var caseid = $scope.BPMExceutionTabDetails.CaseVersion.CaseID;
                        $.connection.hubBPMN.server.getCaseSelectionInstances(caseid).done(function (data) {
                            $scope.$evalAsync(function () {
                                $scope.BPMExceutionTabDetails.lstCaseInstances = [];
                                if (data) {
                                    if (data.length > 0) {
                                        $scope.selectedCaseInstance = data[0];
                                    } else {
                                        $scope.selectedCaseInstance = null;
                                    }
                                    $scope.BPMExceutionTabDetails.lstCaseInstances = data;
                                    $scope.BPMExceutionTabDetails.lstInstances = data;
                                    $scope.uniqueList(data, $scope.BPMExceutionTabDetails.lstPersonId, "PersonId");
                                    $scope.uniqueList(data, $scope.BPMExceutionTabDetails.lstOrgID, "OrgId");
                                    $scope.uniqueList(data, $scope.BPMExceutionTabDetails.lstStatusValue, "statusValue");
                                } else {
                                    $scope.selectedCaseInstance = null;
                                }
                            });
                        });
                    }
                } else {
                    $scope.selectedCaseInstance = null;
                    $scope.BPMExceutionTabDetails.CaseVersion = null;
                    $scope.BPMExceutionTabDetails.lstCaseInstances = [];
                    $scope.BPMExceutionTabDetails.lstInstances = [];
                    $scope.BPMExceutionTabDetails.lstPersonId = [];
                    $scope.BPMExceutionTabDetails.lstOrgID = [];
                    $scope.BPMExceutionTabDetails.lstStatusValue = [];
                }
            };
            $scope.uniqueList = function (source, destination, property) {
                for (var i = 0; i < source.length; i++) {
                    if (destination.indexOf(source[i][property]) == -1) {
                        destination.push(source[i][property]);
                    }
                }
            }
            $scope.onSearchCaseInstancesClick = function () {
                var lstDummyInstances = $scope.BPMExceutionTabDetails.lstCaseInstances;
                if (lstDummyInstances && lstDummyInstances.length > 0) {
                    if ($scope.BPMExceutionTabDetails.PersonID) {
                        lstDummyInstances = lstDummyInstances.filter(function (x) { return x.PersonId == $scope.BPMExceutionTabDetails.PersonID; });
                    }
                    if ($scope.BPMExceutionTabDetails.OrgID) {
                        lstDummyInstances = lstDummyInstances.filter(function (x) { return x.OrgId == $scope.BPMExceutionTabDetails.OrgID; });

                    }
                    if ($scope.BPMExceutionTabDetails.Status) {
                        lstDummyInstances = lstDummyInstances.filter(function (x) { return x.statusValue == $scope.BPMExceutionTabDetails.Status; });
                    }
                    if ($scope.BPMExceutionTabDetails.FromDate) {
                        lstDummyInstances = lstDummyInstances.filter(function (x) { return new Date(x.Date) >= new Date($scope.BPMExceutionTabDetails.FromDate); });
                    }
                    if ($scope.BPMExceutionTabDetails.ToDate) {
                        lstDummyInstances = lstDummyInstances.filter(function (x) { return new Date(x.Date) <= new Date($scope.BPMExceutionTabDetails.ToDate); });
                    }
                    $scope.BPMExceutionTabDetails.lstInstances = lstDummyInstances;
                    if ($scope.BPMExceutionTabDetails.lstInstances.length > 0) {
                        $scope.selectedCaseInstance = $scope.BPMExceutionTabDetails.lstInstances[0];
                    } else {
                        $scope.selectedCaseInstance = "";
                    }
                }
            };

            $.connection.hubBPMN.server.loadCaseData($rootScope.currentopenfile.file.FileName).done(function (data) {
                $scope.$evalAsync(function () {
                    if (data) {
                        $scope.BPMExceutionTabDetails.CaseVersionList = data;
                    }
                });
            });

            $scope.OpenExecutedBPM = function (mode) {
                if ($scope.selectedCaseInstance && $scope.BPMExceutionTabDetails && $scope.BPMExceutionTabDetails.CaseVersion) {
                    var caseid = $scope.BPMExceutionTabDetails.CaseVersion.CaseID;
                    var versionid = $scope.BPMExceutionTabDetails.CaseVersion.Version;
                    var caseinstanceid = $scope.selectedCaseInstance.caseInstanceId;
                    if (caseid && versionid && caseinstanceid) {
                        $.connection.hubBPMN.server.getVersionAndLoadedExecutedSteps(caseid, versionid, caseinstanceid).done(function (data) {
                            $scope.$evalAsync(function () {
                                if (data) {
                                    $scope.BPMExceutionTabDetails.ExecutedData = data;
                                    var DialogExcecutionScope = $scope.$new();
                                    $scope.BPMExceutionTabDetails.Mode = mode;
                                    $scope.BPMExceutionTabDetails.isChildWindow = false;
                                    DialogExcecutionScope.BPMExceutionTabDetails = $scope.BPMExceutionTabDetails;
                                    DialogExcecutionScope.templateName = "BPM/views/Execution/BPMExecutionDialog.html";
                                    DialogExcecutionScope.dialogOptions = { height: 700, width: 1300, showclose: true };
                                    DialogExcecutionScope.dialog = $rootScope.showDialog(DialogExcecutionScope, "Execution Result", DialogExcecutionScope.templateName, DialogExcecutionScope.dialogOptions);
                                    DialogExcecutionScope.close = function () {
                                        $scope.$evalAsync(function () {
                                            DialogExcecutionScope.dialog.close();
                                        });
                                    };
                                }
                            });
                        });
                    }
                }
            };
        }
    };

    $scope.SVGstateWrapper = function (activeTab) {
        switch (activeTab.toLowerCase()) {
            case "association": $scope.SetSvgelementEnable(); break;
            case "performers":
                var allLaneElements = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG [elementtype= " + "lane" + "]");
                var allUsertaskElements = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG [elementtype= " + "userTask" + "]");
                $cssFunc.TogglecssClass(allLaneElements, "customDisableElement", "customEnableElement");
                $cssFunc.TogglecssClass(allUsertaskElements, "customDisableElement", "customEnableElement");
                break;
        }
    };
    $scope.SetallSvgelementDisable = function (check) {
        var ele = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG").children();
        if (check) $cssFunc.addEachcssClass(ele, "customDisableElement");
        else $cssFunc.removeEachcssClass(ele, "customDisableElement");
    };
    $scope.SetallSvgelementEnable = function (check) {
        var ele = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG").children();
        if (check) $cssFunc.addEachcssClass(ele, "customEnableElement");
        else $cssFunc.removeEachcssClass(ele, "customEnableElement");
    };
    $scope.SetSvgelementEnable = function () {
        // remove disable class if any and add enable class to active elements
        var enableID = "";
        switch ($scope.associationActiveTab) {
            case "CALLACTIVITY": enableID = "callActivity"; break;
            case "GATEWAYEXPRESSION": enableID = "GatewayExpression "; break;
            case "SERVICETASK": enableID = "serviceTask"; break;
            case "RULE": enableID = "businessRuleTask"; break;
            case "FORM": enableID = "userTask"; break;
            default:
        }
        if (enableID != "") {
            var alltobeEnableElements = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#mySVG [elementtype= " + enableID + "]");
            $cssFunc.TogglecssClass(alltobeEnableElements, "customDisableElement", "customEnableElement");
        }
    };

    $scope.onChangeofText = function () {
        if ($scope.selectedShape) {
            if ($scope.selectedShape.ShapeModel.dictAttributes.name != undefined && $scope.selectedShape.ShapeName != "BPMNEdge" && $scope.selectedShape.ShapeModel.Name != "textAnnotation") {
                $scope.selectedShape.Text = $scope.selectedShape.ShapeModel.dictAttributes.name;
                removeExistingTextNodeAndAddNewNode($scope);
            }
            else if ($scope.selectedShape.ShapeModel.Name == "textAnnotation") {
                if ($scope.selectedShape.ShapeModel.Elements[0].Value != $scope.selectedShape.Text) {
                    $scope.selectedShape.Text = $scope.selectedShape.ShapeModel.Elements[0].Value;
                    removeExistingTextNodeAndAddNewNode($scope);
                }
            }
            else if ($scope.selectedShape.ShapeModel.dictAttributes.name != undefined && $scope.selectedShape.ShapeName == "BPMNEdge") {
                $scope.selectedShape.Text = $scope.selectedShape.ShapeModel.dictAttributes.name;
                RemoveExistingTextAndAddNewNodeForFlowLines($scope);
            }
        }
    };

    $scope.setAssociationTab = function (tab) {
        $scope.associationActiveTab = tab;
        $scope.SetallSvgelementEnable(false);
        $scope.SetallSvgelementDisable(true);
        $scope.SetSvgelementEnable();
    };
    $scope.selectVariable = function (variable) {
        $scope.selectedVariable = variable;
        $scope.setAllVariableReadOnlyExceptThis(variable);
    };
    $scope.addVariable = function () {
        $rootScope.UndRedoBulkOp("Start");
        var newMapVar = new objMapVarible();
        $rootScope.PushItem(newMapVar, $scope.mapVariablesModel.Elements);
        $rootScope.EditPropertyValue($scope.selectedVariable, $scope, "selectedVariable", newMapVar);
        $scope.setVariableEditable(newMapVar);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.deleteVariable = function () {
        if ($scope.selectedVariable && $scope.mandatoryElementIds.indexOf($scope.selectedVariable.dictAttributes.id) == -1) {
            var deleteIndex = $scope.mapVariablesModel.Elements.indexOf($scope.selectedVariable);
            if (deleteIndex > -1) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.selectedVariable, $scope.mapVariablesModel.Elements, null);
                if (deleteIndex < $scope.mapVariablesModel.length - 1) deleteIndex--;
                $rootScope.EditPropertyValue($scope.selectedVariable, $scope, "selectedVariable", $scope.mapVariablesModel.Elements[deleteIndex]);
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    $scope.canDeleteVariable = function () {
        return $scope.selectedVariable && $scope.mandatoryElementIds.indexOf($scope.selectedVariable.dictAttributes.id) == -1;
    };
    $scope.changeMapType = function (mapvar) {
        if (mapvar.dictAttributes.dataType == 'Object') {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwDefaultValueType, mapvar.dictAttributes, "sfwDefaultValueType", "");
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwDefaultValue, mapvar.dictAttributes, "sfwDefaultValue", "");
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwQueryID, mapvar.dictAttributes, "sfwQueryID", "");
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwQueryParameters, mapvar.dictAttributes, "sfwQueryParameters", "");
            $rootScope.UndRedoBulkOp("End");
        }
        else $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwEntity, mapvar.dictAttributes, "sfwEntity", "");
    };
    $scope.changeMapdvType = function (mapvar) {
        if (mapvar.dictAttributes.sfwDefaultValueType == 'Query') $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwDefaultValue, mapvar.dictAttributes, "sfwDefaultValue", "");
        else {
            $rootScope.UndRedoBulkOp("Start");
            if (!mapvar.dictAttributes.sfwDefaultValueType) {
                $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwDefaultValue, mapvar.dictAttributes, "sfwDefaultValue", "");
            }
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwQueryID, mapvar.dictAttributes, "sfwQueryID", "");
            $rootScope.EditPropertyValue(mapvar.dictAttributes.sfwQueryParameters, mapvar.dictAttributes, "sfwQueryParameters", "");
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.setVariableEditable = function (variable) {
        if ($scope.mandatoryElementIds.indexOf(variable.dictAttributes.id) == -1) {
            variable.isEditable = true;
            variable.isMandatory = true;
        } else {
            variable.isEditable = true;
            variable.isMandatory = false;
        }
    };
    $scope.setVariableReadOnly = function (variable) {
        variable.isEditable = false;
        variable.isMandatory = true;
    };
    $scope.checkAndSetVariableReadOnly = function (eargs, variable) {
        if (eargs.keyCode == $.ui.keyCode.ESCAPE) {
            $scope.setVariableReadOnly(variable);
        }
    };
    $scope.validateVariable = function (variable) {
        var isDuplicate = false;
        angular.forEach($scope.mapVariablesModel.Elements, function (varObj) {
            if (varObj.dictAttributes.id && variable.dictAttributes.id && varObj.dictAttributes.id.toLowerCase() == variable.dictAttributes.id.toLowerCase() && varObj != variable) {
                isDuplicate = true;
            }
        });
        if (isDuplicate) {
            alert("Duplicate Id present.Please change the id");
            variable.dictAttributes.id = "";
            return false;
        }
    };
    $scope.setAllVariableReadOnlyExceptThis = function (variable) {
        angular.forEach($scope.mapVariablesModel.Elements.filter(function (item) { return item != variable; }), function (otherVariable) {
            $scope.setVariableReadOnly(otherVariable);
        });
    };

    $scope.getDivergingGateWays = function () {
        var lstgateWays = [];
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            if ($scope.objData.lstShapes[i].ShapeName == "BPMNShape" && $scope.objData.lstShapes[i].ShapeType == "inclusiveGateway") {
                if ($scope.objData.lstShapes[i].ShapeModel.dictAttributes.gatewayDirection == "Diverging" || $scope.objData.lstShapes[i].ShapeModel.dictAttributes.gatewayDirection == "Mixed") {
                    var objGateWayModel = { Name: $scope.objData.lstShapes[i].ShapeModel.dictAttributes.name, ID: $scope.objData.lstShapes[i].ShapeModel.dictAttributes.id };
                    lstgateWays.push(objGateWayModel);
                }
            }
        }
        return lstgateWays;
    };

    // on double clicking an element this is called - for opening dialogs

    $scope.EditSVGheightandwidth = function () {
        //$scope.$apply(function () {
        var newScope = $scope.$new();
        newScope.svgHeight = $scope.svgElement.getAttribute('height');
        newScope.svgWidth = $scope.svgElement.getAttribute('width');
        newScope.templateName = "BPM/views/EditSvgTemplate.html";
        newScope.dialogOptions = { height: 200, width: 350 };
        newScope.onOKClick = function () {
            $scope.svgElement.setAttribute('height', newScope.svgHeight);
            $scope.svgElement.setAttribute('width', newScope.svgWidth);
            newScope.dialog.close();
        };
        newScope.onCancelClick = function () {
            newScope.dialog.close();
        };
        newScope.dialog = $rootScope.showDialog(newScope, "Edit SVG Bounds", newScope.templateName, newScope.dialogOptions);
        //});
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
    $scope.deployMap = function (redeploy) {
        if (redeploy) {
            var newScope = $scope.$new();
            newScope.templateName = "BPM/views/ReDeployDialog.html";
            newScope.dialogOptions = { height: 245, width: 600 };
            newScope.onOKClick = function () {
                $rootScope.IsLoading = true;
                $.connection.hubBPMN.server.deployMap($rootScope.currentopenfile.file.FilePath, newScope.selectedCaseID).done(function (data) {
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                    if (data.IsValid) {
                        toastr.success(data.Errormsg);
                        if ($scope.ActiveTabForBPM == 'Versions') {
                            $scope.loadVersions();
                        }
                    }
                });
                newScope.closeDialog();
            };
            newScope.closeDialog = function () {
                if (newScope.dialog) {
                    newScope.dialog.close();
                }
            };
            $.connection.hubBPMN.server.loadCaseData($rootScope.currentopenfile.file.FileName).done(function (data) {
                $scope.$evalAsync(function () {
                    newScope.versionList = data;
                });
            });
            newScope.dialog = $rootScope.showDialog(newScope, "Re-Deploy - Select Case ID", newScope.templateName, newScope.dialogOptions);
        }
        else {
            $rootScope.IsLoading = true;
            $.connection.hubBPMN.server.deployMap($rootScope.currentopenfile.file.FilePath, 0).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
                if (data.IsValid) {
                    toastr.success(data.Errormsg);
                    if ($scope.ActiveTabForBPM == 'Versions') {
                        $scope.loadVersions();
                    }
                    if ($scope.BPMExceutionTabDetails && $scope.BPMExceutionTabDetails.CaseVersionList) {
                        $.connection.hubBPMN.server.loadCaseData($rootScope.currentopenfile.file.FileName).done(function (data) {
                            $scope.$evalAsync(function () {
                                $scope.BPMExceutionTabDetails.CaseVersionList = data;
                            });
                        });
                    }
                }
            });
        }
    };
    $scope.loadVersions = function () {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = true;
        });
        hubcontext.hubBPMN.server.loadCaseData($rootScope.currentopenfile.file.FileName).done(function (data) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = false;
                $scope.versionList = data;
            });
        }).fail(function (data) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = false;
            });
        });
    };
    $scope.showMapDetails = function () {
        var newScope = $scope.$new();
        newScope.bpmnModel = {};
        //Extra Field varibles
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "BPM";

        newScope.Init = function () {
            newScope.bpmnModel.Title = $scope.BPMNModel.Title;
            newScope.bpmnModel.ResourceID = $scope.BPMNModel.ResourceID;
            newScope.bpmnModel.Type = $scope.BPMNModel.Type;
            newScope.bpmnModel.IsReusable = $scope.BPMNModel.IsReusable;
            $.connection.hubBPMN.server.isBPMReused($scope.currentfile.FileName).done(function (isreused) {
                newScope.bpmnModel.isReused = isreused;
            })
            if ($scope.BPMNModel.ExtensionElementsModel) {
                newScope.bpmnModel.ExtensionElementsModel = { Elements: [] };
                for (var i = 0; i < $scope.BPMNModel.ExtensionElementsModel.Elements.length; i++) {
                    var ele = {};
                    angular.copy($scope.BPMNModel.ExtensionElementsModel.Elements[i], ele);
                    newScope.bpmnModel.ExtensionElementsModel.Elements.push(ele);
                }
            }

            if (newScope.bpmnModel.ExtensionElementsModel) {
                newScope.effectiveDateModel = $filter('filter')(newScope.bpmnModel.ExtensionElementsModel.Elements, { Name: "effectiveDate" })[0];
            }
            if (!newScope.effectiveDateModel && newScope.bpmnModel.ExtensionElementsModel) {
                var effectivedateModel = { objExtraData: null, dictAttributes: {}, Elements: [], Children: [], Name: "effectiveDate", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };
                newScope.bpmnModel.ExtensionElementsModel.Elements.push(effectivedateModel);
                newScope.effectiveDateModel = effectivedateModel;
            }
            var extraDetailsModel = null;
            if (newScope.bpmnModel.ExtensionElementsModel) {
                extraDetailsModel = $filter('filter')(newScope.bpmnModel.ExtensionElementsModel.Elements, { Name: "extraDetails" })[0];
            }
            if (extraDetailsModel) {
                newScope.editableModel = $filter('filter')(extraDetailsModel.Elements, { Name: "editable" })[0];
            }
        };

        newScope.onChangeEditableModel = function () {
            if (!newScope.editableModel || newScope.editableModel.Value.toLowerCase() == "true") {
                $rootScope.EditPropertyValue($scope.IsMapEditable, $scope, "IsMapEditable", true);
                //$scope.IsMapEditable = true;
            } else {
                $rootScope.EditPropertyValue($scope.IsMapEditable, $scope, "IsMapEditable", false);
                //$scope.IsMapEditable = false;
                removeRearrangingLinePoints($scope);
                removeResizerAroundElement($scope);
                removehovercirclefromsvgElement($scope);
            }
        };
        newScope.onOKClick = function () {
            //Do something on OK click
            $rootScope.UndRedoBulkOp("Start");
            // #region extra field data
            newScope.onChangeEditableModel();
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            //#endregion

            $rootScope.EditPropertyValue($scope.BPMNModel.Title, $scope.BPMNModel, "Title", newScope.bpmnModel.Title);
            $rootScope.EditPropertyValue($scope.BPMNModel.ResourceID, $scope.BPMNModel, "ResourceID", newScope.bpmnModel.ResourceID);
            $rootScope.EditPropertyValue($scope.BPMNModel.IsReusable, $scope.BPMNModel, "IsReusable", newScope.bpmnModel.IsReusable);

            if ($scope.BPMNModel.ExtensionElementsModel) {
                var effectiveDateModel = $filter('filter')($scope.BPMNModel.ExtensionElementsModel.Elements, { Name: "effectiveDate" })[0];
                var DumEffectiveDateModel = $filter('filter')(newScope.bpmnModel.ExtensionElementsModel.Elements, { Name: "effectiveDate" })[0];
                if (!effectiveDateModel) {
                    $rootScope.PushItem(DumEffectiveDateModel, $scope.BPMNModel.ExtensionElementsModel.Elements)
                } else {
                    $rootScope.EditPropertyValue(effectiveDateModel.Value, effectiveDateModel, "Value", DumEffectiveDateModel.Value);
                }
                var extraDetailsModel = $filter('filter')($scope.BPMNModel.ExtensionElementsModel.Elements, { Name: "extraDetails" })[0];
                if (extraDetailsModel) {
                    var editableModel = $filter('filter')(extraDetailsModel.Elements, { Name: "editable" })[0];
                    editableModel.Value = newScope.editableModel.Value;
                }
            }


            $rootScope.UndRedoBulkOp("End");
            //close the dialog
            newScope.closeDialog();
        };
        newScope.closeDialog = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };

        newScope.validateBPMDetails = function () {
            newScope.FormDetailsErrorMessage = "";
            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var flag = validateExtraFields(newScope);
            return flag;
        };

        newScope.dialog = $rootScope.showDialog(newScope, "Details", "BPM/views/BpmDetailsTemplate.html");
        newScope.Init();
    };

    // ===================================  needs refactoring
    var browseQuery = (function () {
        var dialogScope = $scope.$new();
        dialogScope.DialogConstants = { queryIdParamName: "sfwQueryID", queryParamsName: "sfwQueryParameters" };
        return {
            setnewScope: function () {
                dialogScope = $scope.$new();
                dialogScope.DialogConstants = { queryIdParamName: "sfwQueryID", queryParamsName: "sfwQueryParameters" };
            },
            loadDialog: function (mapObj, DialogConstants) {
                dialogScope.DialogConstants = DialogConstants ? DialogConstants : dialogScope.DialogConstants;
                dialogScope.strSelectedQuery = mapObj.dictAttributes[dialogScope.DialogConstants.queryIdParamName];
                dialogScope.mapObj = mapObj;
                dialogScope.IsBPM = true;
                dialogScope.queryValues = $scope.queryParameterallowedValues;
                dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse Queries", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });
            },
            afterDialog: function (data, isSetQueryParams) {
                if (angular.isDefined(dialogScope.mapObj && dialogScope.mapObj.dictAttributes)) {
                    $rootScope.EditPropertyValue(dialogScope.mapObj.dictAttributes[dialogScope.DialogConstants.queryIdParamName], dialogScope.mapObj.dictAttributes, dialogScope.DialogConstants.queryIdParamName, data.Id);
                    // if query parameters need to be set
                    if (isSetQueryParams) {
                        var queryParametersDisplay = [];
                        var queryParameters = [];
                        function iterator(value, key) {
                            queryParameters.push(value);
                            this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                        }
                        angular.forEach(data.Parameters, iterator, queryParametersDisplay);
                        $rootScope.EditPropertyValue(dialogScope.mapObj.dictAttributes[dialogScope.DialogConstants.queryParamsName], dialogScope.mapObj.dictAttributes, dialogScope.DialogConstants.queryParamsName, queryParametersDisplay.join(";") + ";");
                    }
                    else {
                        dialogScope.$destroy();
                    }
                }
            },
            setQueryParam: function (mapObj) {
                if (angular.isDefined(mapObj)) {
                    dialogScope.mapObj = {};
                    // no need to for this as this mapobj is not used in html anywhere
                    //angular.copy(mapObj, dialogScope.mapObj);
                    dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(mapObj, "sfwQueryParameters", ";");
                    dialogScope.queryValues = $scope.queryParameterallowedValues;
                    dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Query Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
                    dialogScope.setQueryParameters = function () {
                        var queryParametersDisplay = [];
                        function iterator(value, key) {
                            this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                        }
                        angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
                        if (dialogScope.mapObj.dictAttributes) {
                            $rootScope.EditPropertyValue(dialogScope.mapObj.dictAttributes[dialogScope.DialogConstants.queryParamsName], dialogScope.mapObj.dictAttributes, dialogScope.DialogConstants.queryParamsName, queryParametersDisplay.join(";") + ";");
                        }
                        else {
                            $rootScope.EditPropertyValue(mapObj.sfwQueryParameters, mapObj, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                        }
                        //angular.merge(mapObj, dialogScope.mapObj);
                        dialogScope.QueryDialog.close();
                    };
                }
            }
        };
    }()); // IFEE 
    $scope.getQuery_dialog = function (mapObj) {
        // you can override the DialogConstants param here 
        // pass null if you dont want to override 
        var DialogConstants = null;
        browseQuery.setnewScope();
        browseQuery.loadDialog(mapObj, DialogConstants);
    };
    //$scope.$on('onQueryClickBPM', function (event, data) {
    //    // pass true for setting query params after getting the response 
    //    browseQuery.afterDialog(data, true);
    //});
    $scope.getQueryParam_dialog = function (mapObj) {
        // you can override the DialogConstants param here 
        // pass null if you dont want to override        
        browseQuery.setnewScope();
        var DialogConstants = null;
        browseQuery.setQueryParam(mapObj);
    };
    // ===================================  needs refactoring

    $scope.SetCalledElementParameters = function (mapName, extensionElementModel) {
        if (mapName && extensionElementModel) {
            function successCallbackReusableBpm(data) {
                $scope.$evalAsync(function () {
                    function filterParameters(x) {
                        return x.Name == "parameters";
                    }
                    var lstUpdatedparameter = data;
                    var objParameters = extensionElementModel.Elements.filter(filterParameters)[0];
                    // if no parameter model add to extension element
                    $rootScope.UndRedoBulkOp("Start");
                    if (!objParameters) {
                        var parametersObj = new objElements();
                        parametersObj.prefix = "";
                        parametersObj.Name = "parameters";
                        objParameters = parametersObj;
                        $rootScope.PushItem(parametersObj, extensionElementModel.Elements);
                    }

                    //preserving old parameters.
                    var existingParameters = objParameters.Elements;
                    $rootScope.EditPropertyValue(objParameters.Elements, objParameters, "Elements", []);

                    //Adding new parameters.
                    if (Object.prototype.toString.call(lstUpdatedparameter) === '[object Array]') {
                        function iterator(param) {
                            if (param && param.dictAttributes && param.dictAttributes.id) {
                                var newObj = { prefix: "", Name: "parameter", dictAttributes: { sfwParamaterName: param.dictAttributes.id, sfwDataType: param.dictAttributes.dataType, sfwEntity: param.dictAttributes.sfwEntity, sfwDirection: param.dictAttributes.type, sfwValueSource: "Parameter" }, Elements: [], Children: [] };
                                $rootScope.PushItem(newObj, objParameters.Elements);
                            }
                        }
                        angular.forEach(lstUpdatedparameter, iterator);
                    }

                    //Updating current parameter values from old parameters.
                    for (var idx = 0; idx < existingParameters.length; idx++) {
                        if (existingParameters[idx] && existingParameters[idx].dictAttributes && existingParameters[idx].dictAttributes.sfwParamaterName) {
                            var params = objParameters.Elements.filter(function (x) { return x.dictAttributes.sfwParamaterName == existingParameters[idx].dictAttributes.sfwParamaterName; });
                            if (params && params.length > 0) {
                                if (params[0].dictAttributes.sfwDirection.toLowerCase() == "in") {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwValueSource, params[0].dictAttributes, "sfwValueSource", existingParameters[idx].dictAttributes.sfwValueSource);
                                }
                                else {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwValueSource, params[0].dictAttributes, "sfwValueSource", "Parameter");
                                }
                                if (params[0].dictAttributes.sfwDataType == "Object" && params[0].dictAttributes.sfwValueSource == "Constant") {
                                    if (existingParameters[idx].dictAttributes.sfwDataType == "Object" && params[0].dictAttributes.sfwEntity == existingParameters[idx].dictAttributes.sfwEntity) {
                                        if (existingParameters[idx].Elements && existingParameters[idx].Elements.length > 0 && existingParameters[idx].Elements[0].Name == "value") {
                                            $rootScope.PushItem(existingParameters[idx].Elements[0], params[0].Elements);
                                        }
                                    }
                                    $scope.checkAndUpdateParamChildModels(params[0], true);
                                }
                                else {
                                    params[0].dictAttributes.sfwParameterValue = existingParameters[idx].dictAttributes.sfwParameterValue;
                                }
                            }
                        }
                    }

                    $rootScope.UndRedoBulkOp("End");
                });
            }
            // get updated parameter for the set called element
            $.connection.hubBPMN.server.getReusableBpmnParamterlist(mapName).done(successCallbackReusableBpm);
        }
    };

    // ===================================  map tab functions

    // ===================================  association tab functions


    $scope.setParamEntityValue = function (parentScope, param) {
        if (param && param.valueModel) {
            var dialogScope = parentScope.$new();
            dialogScope.objSetParam = {};
            if (param.dictAttributes.sfwEntity && param.dictAttributes.sfwEntity != "entBase") {
                dialogScope.validEntities = $EntityIntellisenseFactory.getDescendentEntitiesIncludingThis(param.dictAttributes.sfwEntity);
            }
            else {
                dialogScope.validEntities = $EntityIntellisenseFactory.getEntityIntellisense();
            }
            dialogScope.businessObjectModel = param.valueModel.businessObjectModel;
            dialogScope.getSrvObjects = function (refreshSrvParametersCallback) {
                $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                    $scope.$evalAsync(function () {
                        dialogScope.businessObjectModel.initializeModel.srvObjects = data;
                        if (dialogScope.businessObjectModel.initializeModel.srvMethodModel && dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.srvName) {
                            dialogScope.populateSrvMethods();
                        }

                        if (refreshSrvParametersCallback) {
                            refreshSrvParametersCallback();
                        }
                    });
                });
            };
            dialogScope.onObjectTypeChanged = function () {
                $rootScope.UndRedoBulkOp("Start");
                if (dialogScope.businessObjectModel.initializeModel.dictAttributes.ObjectType && dialogScope.businessObjectModel.initializeModel.dictAttributes.ObjectType === "SrvMethod") {
                    if (dialogScope.businessObjectModel.initializeModel.xmlMethodModel) {
                        var index = dialogScope.businessObjectModel.initializeModel.Elements.indexOf(dialogScope.businessObjectModel.initializeModel.xmlMethodModel)
                        if (index > -1) {
                            $rootScope.DeleteItem(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel.Elements);
                            $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel, "xmlMethodModel", null);
                        }
                    }
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.srvMethodModel, dialogScope.businessObjectModel.initializeModel, "srvMethodModel", { Name: "SrvMethod", dictAttributes: {}, Elements: [] });
                    $rootScope.PushItem(dialogScope.businessObjectModel.initializeModel.srvMethodModel, dialogScope.businessObjectModel.initializeModel.Elements)
                }
                else {
                    if (dialogScope.businessObjectModel.initializeModel.srvMethodModel) {
                        var index = dialogScope.businessObjectModel.initializeModel.Elements.indexOf(dialogScope.businessObjectModel.initializeModel.srvMethodModel)
                        if (index > -1) {
                            $rootScope.DeleteItem(dialogScope.businessObjectModel.initializeModel.srvMethodModel, dialogScope.businessObjectModel.initializeModel.Elements);
                            $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.srvMethodModel, dialogScope.businessObjectModel.initializeModel, "srvMethodModel", null);
                        }
                    }
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel, "xmlMethodModel", { Name: "XMLMethod", dictAttributes: {}, Elements: [] });
                    $rootScope.PushItem(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel.Elements)
                }
                $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", "");
                $rootScope.UndRedoBulkOp("End");
            };
            dialogScope.populateSrvMethods = function () {
                dialogScope.businessObjectModel.initializeModel.srvMethods = [];
                //Populate srv Method intellisense for selected srv object.
                if (dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.srvName) {
                    for (var index = 0, len = dialogScope.businessObjectModel.initializeModel.srvObjects.length; index < len; index++) {
                        if (dialogScope.businessObjectModel.initializeModel.srvObjects[index].dictAttributes.ID === dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.srvName) {
                            for (var idx = 0, len = dialogScope.businessObjectModel.initializeModel.srvObjects[index].Elements.length; idx < len; idx++) {
                                dialogScope.businessObjectModel.initializeModel.srvMethods.push(dialogScope.businessObjectModel.initializeModel.srvObjects[index].Elements[idx]);
                            }
                            break;
                        }
                    }
                }
}
            dialogScope.onSrvObjectChanged = function () {
                $rootScope.UndRedoBulkOp("Start");
                //Clear Existing Intellisense ;
                if (dialogScope.businessObjectModel.initializeModel.srvMethods) {
                    for (var index = 0, len = dialogScope.businessObjectModel.initializeModel.srvMethods.length; index < len; index++) {
                        $rootScope.DeleteItem(dialogScope.businessObjectModel.initializeModel.srvMethods[index], dialogScope.businessObjectModel.initializeModel.srvMethods);
                    }
                }
                else {
                    dialogScope.businessObjectModel.initializeModel.srvMethods = [];
                }

                //Populate srv Method intellisense for selected srv object.
                if (dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.srvName) {
                    for (var index = 0, len = dialogScope.businessObjectModel.initializeModel.srvObjects.length; index < len; index++) {
                        if (dialogScope.businessObjectModel.initializeModel.srvObjects[index].dictAttributes.ID === dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.srvName) {
                            for (var idx = 0, len = dialogScope.businessObjectModel.initializeModel.srvObjects[index].Elements.length; idx < len; idx++) {
                                $rootScope.PushItem(dialogScope.businessObjectModel.initializeModel.srvObjects[index].Elements[idx], dialogScope.businessObjectModel.initializeModel.srvMethods);
                            }
                            break;
                        }
                    }
                }
                dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.ID = "";
                $rootScope.UndRedoBulkOp("End");
            };
            dialogScope.onSrvMethodChanged = function () {
                // load navigation parameters and items 
                if (dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.ID) {
                    var srvMethod = $filter('filter')(dialogScope.businessObjectModel.initializeModel.srvMethods, { dictAttributes: { ID: dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.ID } }, true)[0];
                    if (srvMethod) {
                        dialogScope.businessObjectModel.initializeModel.xmlmethodItems = srvMethod.Elements;
                        var queryParametersDisplay = [];
                        function iterator(value, key) {
                            this.push(value.dictAttributes.ID + '=');
                        }
                        angular.forEach(srvMethod.Elements, iterator, queryParametersDisplay);
                        $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(";"));
                    }
                }
            };
            dialogScope.getSrvObjects();

            dialogScope.loadXmlmethods = function () {
                dialogScope.xmlMethods = $getEnitityXmlMethods.get(dialogScope.businessObjectModel.dictAttributes.sfwEntity, "load", "");
            };

            dialogScope.onXmlMethodChange = function () {
                // load navigation parameters and items 
                if (dialogScope.objSetParam.selectedXmlMethodID && dialogScope.objSetParam.selectedXmlMethodID.trim().length > 0) {
                    dialogScope.addUpdatedXmlMethod();
                }
                else {
                    dialogScope.removeXmlMethod();
                }
            };
            dialogScope.addUpdatedXmlMethod = function () {
                $rootScope.UndRedoBulkOp("Start");
                if (!dialogScope.businessObjectModel.initializeModel.xmlMethodModel && $filter('filter')(dialogScope.businessObjectModel.initializeModel.Elements, { Name: "XMLMethod" }, true).length == 0) {
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel, "xmlMethodModel", { prefix: "", Name: "XMLMethod", Value: "", dictAttributes: {}, Elements: [], Children: [] });
                    $rootScope.PushItem(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel.Elements);
                }

                $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID, dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes, "ID", dialogScope.objSetParam.selectedXmlMethodID);
                var xmlmethods = $filter('filter')(dialogScope.xmlMethods, { ID: dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID }, true);
                if (xmlmethods && xmlmethods.length > 0) {
                    dialogScope.xmlMethodParameters = xmlmethods[0].Items;

                    var queryParametersDisplay = [];
                    function iterator(value, key) {
                        this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                    }
                    angular.forEach(xmlmethods[0].Parameters, iterator, queryParametersDisplay);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(";"));
                }
                $rootScope.UndRedoBulkOp("End");
            };
            dialogScope.removeXmlMethod = function () {
                if (dialogScope.businessObjectModel.initializeModel.xmlMethodModel && $filter('filter')(dialogScope.businessObjectModel.initializeModel.Elements, { Name: "XMLMethod" }, true).length > 0) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel.Elements);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel, "xmlMethodModel", undefined);
                    $rootScope.EditPropertyValue(dialogScope.xmlMethodParameters, dialogScope, "xmlMethodParameters", []);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", undefined);
                    $rootScope.UndRedoBulkOp("End");
                }
            };
            dialogScope.setXmlMethodParameters = function () {
                // open pop up for setting parameters
                var childDialogScope = dialogScope.$new();
                childDialogScope.DialogConstants = { queryParamsName: "parameterValue" };
                childDialogScope.mapObj = {};

                childDialogScope.queryValues = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements); //$scope.queryParameterallowedValues;
                childDialogScope.queryParameters = $getQueryparam.getQueryparamfromString(dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", ";");
                childDialogScope.onRefreshParams = function () {
                    if (dialogScope.businessObjectModel.initializeModel && dialogScope.businessObjectModel.initializeModel.dictAttributes.ObjectType === "SrvMethod") {
                        dialogScope.getSrvObjects(childDialogScope.refreshSrvParameters);
                    }
                    else {

                        var initializeModelXmlMethods = $getEnitityXmlMethods.get(dialogScope.businessObjectModel.dictAttributes.sfwEntity.trim())
                        if (initializeModelXmlMethods.length > 0) {
                            var initializeXmlMethods = $filter('filter')(initializeModelXmlMethods, { ID: dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID }, true);
                            if (initializeXmlMethods && initializeXmlMethods.length > 0) {
                                var initializeXmlMethod = initializeXmlMethods[0];

                                childDialogScope.refreshParams(initializeXmlMethod.Parameters);
                            }
                        }
                    }
                };
                childDialogScope.refreshSrvParameters = function () {
                    if (dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.ID) {
                        var srvMethod = $filter('filter')(dialogScope.businessObjectModel.initializeModel.srvMethods, { dictAttributes: { ID: dialogScope.businessObjectModel.initializeModel.srvMethodModel.dictAttributes.ID } }, true);
                        if (srvMethod && srvMethod.length) {
                            childDialogScope.refreshParams(srvMethod[0].Elements);
                        }
                    }
                }
                childDialogScope.refreshParams = function (paramCollection) {
                    //preserving old parameters.
                    var existingParameters = childDialogScope.queryParameters;
                    childDialogScope.queryParameters = [];

                    function iterator(value) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        var paramID = "";
                        if (value.dictAttributes) {
                            paramID = value.dictAttributes.ID;
                        }
                        else {
                            paramID = value.ID;
                        }
                        nodeParameters.ID = paramID;

                        if (childDialogScope.queryParameters.length > 0 && !childDialogScope.queryParameters.some(function (param) { return param.dictAttributes.ID == paramID })) {
                            $rootScope.PushItem(nodeParameters, childDialogScope.queryParameters);
                        }
                        if (childDialogScope.queryParameters.length == 0) {
                            $rootScope.PushItem(nodeParameters, childDialogScope.queryParameters);
                        }
                    }
                    angular.forEach(paramCollection, iterator, childDialogScope.queryParameters);

                    for (var idx = 0; idx < existingParameters.length; idx++) {
                        if (existingParameters[idx] && existingParameters[idx].ID) {
                            var params = childDialogScope.queryParameters.filter(function (x) { return x.ID == existingParameters[idx].ID; });
                            if (params && params.length > 0) {
                                if (existingParameters[idx].Value) {
                                    $rootScope.EditPropertyValue(params[0].Value, params[0], "Value", existingParameters[idx].Value);
                                }
                            }
                        }
                    }
                };
                childDialogScope.QueryDialog = $rootScope.showDialog(childDialogScope, "Set Method Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
                childDialogScope.setQueryParameters = function () {
                    var queryParametersDisplay = [];
                    function iterator(value, key) {
                        this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                    }
                    angular.forEach(childDialogScope.queryParameters, iterator, queryParametersDisplay);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(";"));
                    childDialogScope.QueryDialog.close();
                };
            };
            dialogScope.onEntityChanged = function () {
                dialogScope.loadXmlmethods();

                if (dialogScope.businessObjectModel.initializeModel.xmlMethodModel) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel.Elements);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel, dialogScope.businessObjectModel.initializeModel, "xmlMethodModel", undefined);
                    $rootScope.EditPropertyValue(dialogScope.objSetParam.selectedXmlMethodID, dialogScope.objSetParam, "selectedXmlMethodID", undefined);
                    $rootScope.EditPropertyValue(dialogScope.xmlMethodParameters, dialogScope, "xmlMethodParameters", []);
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.dictAttributes.sfwNavigationParameter, dialogScope.businessObjectModel.dictAttributes, "sfwNavigationParameter", undefined);
                    $rootScope.UndRedoBulkOp("End");
                }
            };

            if (dialogScope.businessObjectModel && dialogScope.businessObjectModel.dictAttributes && dialogScope.businessObjectModel.dictAttributes.sfwEntity) {
                dialogScope.loadXmlmethods();
            }

            if (dialogScope.businessObjectModel.initializeModel) {

                items = dialogScope.businessObjectModel.initializeModel.Elements.filter(function (item) { return item.Name == "XMLMethod"; });
                if (items && items.length > 0) {
                    dialogScope.businessObjectModel.initializeModel.xmlMethodModel = items[0];
                    dialogScope.objSetParam.selectedXmlMethodID = dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID;
                    var xmlmethods = $filter('filter')(dialogScope.xmlMethods, { ID: dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID }, true);
                    if (xmlmethods && xmlmethods.length > 0) {
                        dialogScope.xmlMethodParameters = xmlmethods[0].Items;
                    }
                }
                else {
                    items = dialogScope.businessObjectModel.initializeModel.Elements.filter(function (item) { return item.Name == "SrvMethod"; });
                    if (items && items.length > 0) {
                        dialogScope.businessObjectModel.initializeModel.srvMethodModel = items[0];
                    }
                }
            }

            dialogScope.onOKClick = function () {
                if (dialogScope.businessObjectModel.initializeModel.xmlMethodModel) {
                    $rootScope.EditPropertyValue(dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes.ID, dialogScope.businessObjectModel.initializeModel.xmlMethodModel.dictAttributes, "ID", dialogScope.objSetParam.selectedXmlMethodID);
                }
                dialogScope.dialog.close();
            };
            dialogScope.closeDialog = function () {
                if (dialogScope.dialog && dialogScope.dialog.close) {
                    dialogScope.dialog.close();
                }
            };
            dialogScope.dialog = $rootScope.showDialog(dialogScope, "Set Xml Method", "BPM/views/SetXmlMethod.html", { width: 500, height: 500 });
        } else {
            alert("Invalid Data Type");
        }
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

        items = param.valueModel.businessObjectModel.Elements.filter(function (item) { return item.Name == "initialize"; });
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
                $rootScope.EditPropertyValue(param.valueModel.businessObjectModel.initializeModel, param.valueModel.businessObjectModel, "initializeModel", { prefix: "", Name: "initialize", Value: "", dictAttributes: { ObjectType: "XmlMethod" }, Elements: [], Children: [] });
                $rootScope.PushItem(param.valueModel.businessObjectModel.initializeModel, param.valueModel.businessObjectModel.Elements);
            }
            else {
                param.valueModel.businessObjectModel.initializeModel = { prefix: "", Name: "initialize", Value: "", dictAttributes: { ObjectType: "XmlMethod" }, Elements: [], Children: [] };
                param.valueModel.businessObjectModel.Elements.push(param.valueModel.businessObjectModel.initializeModel);
            }
        }
    };

    $scope.onValueSourceChanged = function (param) {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(param.dictAttributes.sfwParameterValue, param.dictAttributes, "sfwParameterValue", "");
        if (param.dictAttributes.sfwDataType == "Object") {
            if (param.dictAttributes.sfwValueSource == "Constant") {
                $scope.checkAndUpdateParamChildModels(param, true);
            }
            else {
                if (param.valueModel) {
                    $rootScope.DeleteItem(param.valueModel, param.Elements, null);
                    $rootScope.EditPropertyValue(param.valueModel, param, "valueModel", undefined);
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };
    // ===================================  association tab functions


    $scope.setDraggableElement = function (param) {
        onBodyClick();
        if (param != "Pointer") {
            $scope.DraggableElement = param;
        }
        else {
            $scope.DraggableElement = null;
        }
        $scope.svgElement.setAttribute("onmouseup", "AddNewElementToBPM(evt)");
        angular.element($scope.svgElement.parentNode).attr("ondrop", "AddNewElementToBPM(event)");
        angular.element($scope.svgElement.parentNode).attr("ondragover", "onElementDragOver(event)");
    };
    $scope.onChangeOfTitle = function (obj) {
        if (obj) {
            $scope.BPMNModel.HeaderID = obj;
        }
        else {
            $scope.BPMNModel.HeaderID = $rootScope.currentopenfile.file.FileName;
        }
    };
    $scope.onDeleteKeyDown = function (event, param) {
        if ((event.keyCode == 46 || param == "Delete") && $scope.activeTab == "MAP") {
            if ($scope.selectedElemForResize != null) {
                $scope.isDirty = true;
                if ($scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType != "participant" && $scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType != "lane") {
                    var ElementID = $scope.objData.lstShapes[$scope.indexofResizerElement].Id;
                    //Delete from shapes

                    //Remove MessageReference
                    if ($scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType == "message") {
                        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                            if ($scope.objData.lstShapes[i].ShapeType == "messageFlow") {
                                if ($scope.objData.lstShapes[$scope.indexofResizerElement].Id == $scope.objData.lstShapes[i].ShapeModel.dictAttributes.messageRef) {
                                    $scope.objData.lstShapes[i].MessageVisibleKind = "";
                                    $scope.objData.lstShapes[i].MessageReference = "";
                                    $scope.objData.lstShapes[i].ShapeModel.dictAttributes.messageRef = "";
                                }
                            }
                        }
                    }

                    $scope.objData.lstShapes.splice($scope.indexofResizerElement, 1);
                    $scope.DeleteFlowNodesBasedOnID(ElementID);
                    //Delete ref from lane model
                    $scope.DeleteRefNodeFromLaneModel(ElementID);
                    $scope.selectedElemForResize = null;
                    RedrawingElementsAndEdges($scope);

                } else if ($scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType == "participant") {
                    if (confirm("Do you want to remove Selected pool ?")) {
                        $scope.DeletePoolanditsElements();
                    }
                } else if ($scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType == "lane") {

                    var processId = $scope.objData.lstShapes[$scope.indexofResizerElement].processRef;
                    var ElementID = $scope.objData.lstShapes[$scope.indexofResizerElement].Id;

                    var objLaneCountandPoolindex = $scope.getLaneCountinPool(processId);
                    if (objLaneCountandPoolindex.LaneCount > 1) {
                        if (confirm("Do you want to remove Selected Lane ?")) {
                            var xpos = $scope.objData.lstShapes[$scope.indexofResizerElement].Left;
                            var ypos = $scope.objData.lstShapes[$scope.indexofResizerElement].Top;
                            var width = $scope.objData.lstShapes[$scope.indexofResizerElement].Width;
                            var height = $scope.objData.lstShapes[$scope.indexofResizerElement].Height;
                            var poolypos = $scope.objData.lstShapes[objLaneCountandPoolindex.PoolIndex].Top;
                            $scope.objData.lstShapes[objLaneCountandPoolindex.PoolIndex].Height = $scope.objData.lstShapes[objLaneCountandPoolindex.PoolIndex].Height - height;
                            //Delete Elements which are present in Lane
                            var objLaneElement = $scope.objData.lstShapes[$scope.indexofResizerElement];

                            for (var j = 0; j < objLaneElement.ShapeModel.Elements.length; j++) {
                                if (objLaneElement.ShapeModel.Elements[j].Name == "flowNodeRef") {
                                    if (objLaneElement.ShapeModel.Elements[j].Value && objLaneElement.ShapeModel.Elements[j].Value != "") {
                                        $scope.DeleteFlowNodesBasedOnID(objLaneElement.ShapeModel.Elements[j].Value);
                                        GetElementProcessIdorElementBasedOnIdorDeleteElement(objLaneElement.ShapeModel.Elements[j].Value, $scope, "Delete");
                                    }
                                }
                            }

                            //Delete lane from shapes
                            GetElementProcessIdorElementBasedOnIdorDeleteElement(objLaneElement.Id, $scope, "Delete");

                            // change position of the lane  and its elemnets present inside it
                            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                                if ($scope.objData.lstShapes[i].ShapeName != "BPMNEdge") {
                                    if ($scope.objData.lstShapes[i].processRef == processId && $scope.objData.lstShapes[i].ShapeType == 'lane') {
                                        if ($scope.objData.lstShapes[i].Top != poolypos && $scope.objData.lstShapes[i].Top > ypos) {
                                            $scope.objData.lstShapes[i].Top = $scope.objData.lstShapes[i].Top - height;
                                            for (var j = 0; j < $scope.objData.lstShapes[i].ShapeModel.Elements.length; j++) {
                                                if ($scope.objData.lstShapes[i].ShapeModel.Elements[j].Name == "flowNodeRef") {
                                                    if ($scope.objData.lstShapes[i].ShapeModel.Elements[j].Value && $scope.objData.lstShapes[i].ShapeModel.Elements[j].Value != "") {
                                                        var objShapeInCurrentLane = GetElementProcessIdorElementBasedOnIdorDeleteElement($scope.objData.lstShapes[i].ShapeModel.Elements[j].Value, $scope, "Element");
                                                        objShapeInCurrentLane.Top = objShapeInCurrentLane.Top - height;
                                                        if (objShapeInCurrentLane.LabelTop > 0) {
                                                            objShapeInCurrentLane.LabelTop = objShapeInCurrentLane.LabelTop - height;
                                                        }
                                                        ydiffislanedeleted = height;
                                                        ResetEdgePoints(objShapeInCurrentLane, ydiffislanedeleted, $scope);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    } else {
                        if (confirm("This is the last lane in current pool. Removing the lane will also remove the pool.Do you want to remove selected lane ?")) {
                            if (objLaneCountandPoolindex.PoolIndex > -1) {
                                $scope.indexofResizerElement = objLaneCountandPoolindex.PoolIndex;
                                $scope.DeletePoolanditsElements();
                            }
                        }
                    }
                    $scope.selectedElemForResize = null;
                    RedrawingElementsAndEdges($scope);
                }

            }

            if ($scope.selectedLineEle != null) {
                var sourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement($scope.objData.lstShapes[$scope.indexOfSelectedLine].SourceElement, $scope, "Element");
                var targetElement = GetElementProcessIdorElementBasedOnIdorDeleteElement($scope.objData.lstShapes[$scope.indexOfSelectedLine].TargetElement, $scope, "Element");
                $scope.objData.lstShapes.splice($scope.indexOfSelectedLine, 1);
                $scope.ChangeDirectionifElementisGateWay(sourceElement, targetElement);
                RedrawingElementsAndEdges($scope);
                $scope.selectedLineEle = null;
            }
            $scope.selectedShape = undefined;
            $scope.OnSelectLeftBPMTab('Toolbox');
        }
    };

    $scope.ChangeDirectionifElementisGateWay = function (sourceElement, targetElement) {
        if (targetElement && targetElement.ShapeType == "inclusiveGateway" || targetElement.ShapeType == "parallelGateway") {
            var direction = getgateWaydirectionbasedOnEdges(targetElement.Id, $scope, "");
            targetElement.ShapeModel.dictAttributes.gatewayDirection = direction;
        } else if (sourceElement && sourceElement.ShapeType == "inclusiveGateway" || sourceElement.ShapeType == "parallelGateway") {
            var direction = getgateWaydirectionbasedOnEdges(sourceElement.Id, $scope, "");
            sourceElement.ShapeModel.dictAttributes.gatewayDirection = direction;
        }

        // Delete syncwith model if the gateway direction is changed
        if ((targetElement && targetElement.ShapeType == "inclusiveGateway") || (sourceElement && sourceElement.ShapeType == "inclusiveGateway")) {
            if ((targetElement && targetElement.ShapeModel.dictAttributes.gatewayDirection != "Converging") || (targetElement && targetElement.ShapeModel.dictAttributes.gatewayDirection != "Mixed")
                || (sourceElement && sourceElement.ShapeModel.dictAttributes.gatewayDirection != "Converging") || (sourceElement && sourceElement.ShapeModel.dictAttributes.gatewayDirection != "Mixed")) {
                if (targetElement && targetElement.ShapeType == "inclusiveGateway") {
                    $scope.DeleteSyncWithifweUsedinAnyinclusiveGateBasedOnID(targetElement.Id);
                } else {
                    if (sourceElement) {
                        $scope.DeleteSyncWithifweUsedinAnyinclusiveGateBasedOnID(sourceElement.Id);
                    }
                }
            }
        }

        //Delete from current ShapeModel
        if (targetElement.ShapeType == "inclusiveGateway" && targetElement.ShapeModel.dictAttributes.gatewayDirection != "Converging") {
            $scope.DeleteSyncWithFromExtensionElements(targetElement.ShapeModel, "");
        }
        if (sourceElement.ShapeType == "inclusiveGateway" && sourceElement.ShapeModel.dictAttributes.gatewayDirection != "Converging") {
            $scope.DeleteSyncWithFromExtensionElements(sourceElement.ShapeModel, "");
        }
    };

    $scope.DeleteSyncWithifweUsedinAnyinclusiveGateBasedOnID = function (inclusivegateWayID) {
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            if ($scope.objData.lstShapes[i].ShapeType == "inclusiveGateway" && $scope.objData.lstShapes[i].ShapeModel.dictAttributes.gatewayDirection == "Converging") {
                $scope.DeleteSyncWithFromExtensionElements($scope.objData.lstShapes[i].ShapeModel, inclusivegateWayID);
            }
        }
    };

    $scope.DeleteSyncWithFromExtensionElements = function (ShapeModel, gateWayID) {
        var extensionElements = $filter('filter')(ShapeModel.Elements, { Name: "extensionElements" });
        if (extensionElements && extensionElements.length > 0) {
            var extensionElementModel = extensionElements[0];
            for (var j = 0; j < extensionElementModel.Elements.length; j++) {
                if (extensionElementModel.Elements[j].Name == "syncwith") {
                    if (gateWayID != "") {
                        if (gateWayID == extensionElementModel.Elements[j].Value) {
                            extensionElementModel.Elements.splice(j, 1);
                        }
                    } else {
                        extensionElementModel.Elements.splice(j, 1);
                    }
                    break;
                }
            }
        }
    };

    $scope.getLaneCountinPool = function (processId) {
        var obj = { LaneCount: '', PoolIndex: -1 };
        var count = 0;
        var poolindex;
        var isFound = false;
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            if ($scope.objData.lstShapes[i].ShapeType == "lane") {
                if ($scope.objData.lstShapes[i].processRef == processId) {
                    count++;
                }
            } else if ($scope.objData.lstShapes[i].ShapeType == "participant" && $scope.objData.lstShapes[i].processRef == processId) {
                poolindex = i;
                isFound = true;
            }
        }

        if (isFound) {
            obj.PoolIndex = poolindex;
        }
        obj.LaneCount = count;

        return obj;
    };

    $scope.DeletePoolanditsElements = function () {
        var processId = $scope.objData.lstShapes[$scope.indexofResizerElement].processRef;
        var ElementID = $scope.objData.lstShapes[$scope.indexofResizerElement].Id;
        var isProcess = $scope.objData.lstShapes[$scope.indexofResizerElement].ShapeType == "participant";
        //Delete from shapes
        $scope.objData.lstShapes.splice($scope.indexofResizerElement, 1);
        $scope.DeleteFlowNodesBasedOnID(ElementID);
        $scope.DeleteElementsBasedOnProcessID(processId);
        $scope.selectedElemForResize = null;

        //If deleting shape is process, then remove process id from list of process ids in model.
        if (isProcess) {
            var procIndex = $scope.objData.Processes.indexOf(processId);
            if (procIndex > -1) {
                $scope.objData.Processes.splice(procIndex, 1);
            }
        }
        RedrawingElementsAndEdges($scope);
    };


    $scope.DeleteFlowNodesBasedOnID = function (ElementID) {
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            if ($scope.objData.lstShapes[i].ShapeName == "BPMNEdge") {
                if ($scope.objData.lstShapes[i].SourceElement == ElementID || $scope.objData.lstShapes[i].TargetElement == ElementID) {
                    var sourceElement = GetElementProcessIdorElementBasedOnIdorDeleteElement($scope.objData.lstShapes[i].SourceElement, $scope, "Element");
                    var targetElement = GetElementProcessIdorElementBasedOnIdorDeleteElement($scope.objData.lstShapes[i].TargetElement, $scope, "Element");
                    $scope.objData.lstShapes.splice(i, 1);
                    $scope.ChangeDirectionifElementisGateWay(sourceElement, targetElement);
                    i--;
                }
            }
        }
    };

    $scope.DeleteRefNodeFromLaneModel = function (ElementID) {
        if ($scope.SelectedElementLane != null) {
            var objLaneElement = { Left: parseFloat($scope.SelectedElementLane.getAttribute("x")), Top: parseFloat($scope.SelectedElementLane.getAttribute("y")), Height: parseFloat($scope.SelectedElementLane.getAttribute("height")), Width: parseFloat($scope.SelectedElementLane.getAttribute("width")) };
            var LaneElement = getElementBasedOnObject(objLaneElement, $scope);
            if (LaneElement != null) {
                for (var i = 0; i < LaneElement.ShapeModel.Elements.length; i++) {
                    if (LaneElement.ShapeModel.Elements[i].Value == ElementID) {
                        LaneElement.ShapeModel.Elements.splice(i, 1);
                        break;
                    }
                }
            }
        }
    };

    $scope.DeleteElementsBasedOnProcessID = function (processID) {
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            if ($scope.objData.lstShapes[i].ShapeName != "BPMNEdge") {
                if ($scope.objData.lstShapes[i].processRef == processID) {
                    var ElementID = $scope.objData.lstShapes[i].Id;
                    $scope.DeleteFlowNodesBasedOnID(ElementID);
                    $scope.objData.lstShapes.splice(i, 1);
                    i = -1;
                }
            }
        }
    };
    // receive functions - called from the hub
    $scope.receiveReusableBpm = function (data) {
        $scope.selectedCallActivityData.reUsableBpm = data;
    };
    $scope.versionWiseDisplay = function () {
        $scope.setActiveTab('MAP');
        $scope.ActiveTabForBPM = 'Versions';
        $scope.isVersionsLoaded = true;
        $scope.loadVersions();
    };
    $scope.openVersionedFile = function (version) {
        var objNewFileInfo = { FileName: $rootScope.currentopenfile.file.FileName + "-" + version.CaseID + "-" + version.Version, FileType: "VersionBPM" };
        $rootScope.openFile(objNewFileInfo);
    };
    // this is the first function to be called when the controller is initialized - everything happens after this call
    $scope.init();

    $scope.openEntityClick = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, "", "");
        }
    };
    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objBPMExtraFields) {
    //        $scope.BPMNModel.ExtraFields = {};
    //        delete $scope.BPMNModel["ExtraFields"];
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        // convert basemodel format into Extrafield object 
        if ($scope.objBPMExtraFields && $scope.objBPMExtraFields.Elements.length > 0) {
            var obj;
            var arrObj = [];
            for (var i = 0; i < $scope.objBPMExtraFields.Elements.length; i++) {
                obj = {};
                obj.ID = $scope.objBPMExtraFields.Elements[i].dictAttributes.ID;
                obj.Value = $scope.objBPMExtraFields.Elements[i].dictAttributes.Value;
                obj.URL = $scope.objBPMExtraFields.Elements[i].dictAttributes.URL;
                arrObj.push(obj);
            }
            $scope.BPMNModel.ExtraFields = arrObj;
        }
        else {
            $scope.BPMNModel.ExtraFields = [];
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };


    //BPM Shapes alignment functions

    $scope.CheckAndSetBoolAllSelectedShapesInSameLane = function (lstselectedShapes) {
        var objLaneElement = null;
        //Get one Lane Element
        for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
            objLaneElement = null;
            if ($scope.objData.lstShapes[i].ShapeType == 'lane') {
                objLaneElement = $scope.objData.lstShapes[i];
            }

            if (objLaneElement) {
                var count = 0;
                for (var k = 0; k < lstselectedShapes.length; k++) {
                    var isSameLane = true;
                    for (var j = 0; j < objLaneElement.ShapeModel.Elements.length; j++) {
                        if (objLaneElement.ShapeModel.Elements[j].Name == "flowNodeRef") {
                            if (objLaneElement.ShapeModel.Elements[j].Value) {
                                if (objLaneElement.ShapeModel.Elements[j].Value == lstselectedShapes[k].Id) {
                                    isSameLane = true;
                                    count++;
                                    break;
                                } else {
                                    isSameLane = false;
                                }
                            }
                        }
                    }
                }

                if (count != lstselectedShapes.length) {
                    $scope.isShowTopandBottomAlignButtons = false;
                } else {
                    $scope.isShowTopandBottomAlignButtons = true;
                    break;
                }
            }
        }


    };
    $scope.OnMakeSameWidthClick = function () {
        $scope.UpdateShapeBoundsDependsOnParameter("width");
    };
    $scope.OnMakeSameHeightClick = function () {
        $scope.UpdateShapeBoundsDependsOnParameter("height");
    };
    $scope.OnMakeSameSizeClick = function () {
        $scope.UpdateShapeBoundsDependsOnParameter("samesize");
    };
    $scope.UpdateShapeBoundsDependsOnParameter = function (param) {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var index = $scope.objData.lstShapes.length;
            var objSelectedShape = null;
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if (objShape.ShapeName != "BPMNEdge" && (objShape.ShapeType == "task" || objShape.ShapeType == "userTask" || objShape.ShapeType == "serviceTask" || objShape.ShapeType == "businessRuleTask" || objShape.ShapeType == "manualTask" || objShape.ShapeType == "callActivity")) {
                    if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                        //if (index > $scope.objData.lstShapes.indexOf(objShape)) {
                        //    index = $scope.objData.lstShapes.indexOf(objShape);
                        if ($scope.lstMultipleSelectedShapes[0] == objShape) {
                            objSelectedShape = objShape;
                            break;
                        }
                        //}
                    }
                }
            }
        }
        if (objSelectedShape != null) {
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && (objShape.ShapeType == "task" || objShape.ShapeType == "userTask" || objShape.ShapeType == "serviceTask" || objShape.ShapeType == "businessRuleTask" || objShape.ShapeType == "manualTask" || objShape.ShapeType == "callActivity")) {
                        if (param == "width") {
                            objShape.Width = objSelectedShape.Width;
                        } else if (param == "height") {
                            objShape.Height = objSelectedShape.Height;
                        } else if (param == "samesize") {
                            objShape.Width = objSelectedShape.Width;
                            objShape.Height = objSelectedShape.Height;
                        }
                        //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                        //{
                        //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                        //}
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
            //$scope.ChangeS
            RedrawingElementsAndEdges($scope);
        }

        $scope.lstMultipleSelectedShapes = [];
        //this.CheckEdges(undoRedoIndex);
    };

    $scope.OnLeftAlignClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var planeWidth = parseFloat($scope.svgElement.getAttribute("width"));
            var minXValue = planeWidth;
            if ($scope.lstMultipleSelectedShapes[0]) {
                minXValue = $scope.lstMultipleSelectedShapes[0].Left;
            }
        }
        if (minXValue < planeWidth) {
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var xdiff = minXValue - objShape.Left;
                        objShape.Left = minXValue;
                        if (objShape.LabelLeft && objShape.LabelLeft < planeWidth && (objShape.LabelLeft + xdiff) < planeWidth) {
                            objShape.LabelLeft = objShape.LabelLeft + xdiff;
                        } else {
                            objShape.LabelLeft = objShape.Left + 1;
                        }
                        //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                        //{
                        //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                        //}
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }

        RedrawingElementsAndEdges($scope);
        $scope.lstMultipleSelectedShapes = [];
        //this.CheckEdges(undoRedoIndex);
    };
    $scope.OnRightAlignClick = function () {
        var maxXValue = 0;
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            if ($scope.lstMultipleSelectedShapes[0]) {
                maxXValue = $scope.lstMultipleSelectedShapes[0].Left + $scope.lstMultipleSelectedShapes[0].Width;
            }
        }
        if (maxXValue > 0) {
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var xdiff = (maxXValue - objShape.Width) - objShape.Left;
                        objShape.Left = maxXValue - objShape.Width;
                        if (objShape.LabelLeft && objShape.LabelLeft > 0 && (objShape.LabelLeft + xdiff) > 0) {
                            objShape.LabelLeft = objShape.LabelLeft + xdiff;
                        } else {
                            objShape.LabelLeft = objShape.Left + 1;
                        }
                        //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                        //{
                        //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                        //}
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }
        RedrawingElementsAndEdges($scope);
        $scope.lstMultipleSelectedShapes = [];
    };

    $scope.OnTopAlignClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var planeheight = parseFloat($scope.svgElement.getAttribute("height"));
            var minYValue = planeheight;
            if ($scope.lstMultipleSelectedShapes[0]) {
                minYValue = $scope.lstMultipleSelectedShapes[0].Top;
            }
        }
        if (minYValue < planeheight) {
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var ydiff = minYValue - objShape.Top;
                        objShape.Top = minYValue;
                        if (objShape.LabelTop && objShape.LabelTop < planeheight && (objShape.LabelTop + ydiff) < planeheight) {
                            objShape.LabelTop = objShape.LabelTop + ydiff;
                        } else {
                            objShape.LabelTop = objShape.Top + 25;
                        }
                        //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                        //{
                        //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                        //}
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }
        RedrawingElementsAndEdges($scope);
        $scope.lstMultipleSelectedShapes = [];
        //this.CheckEdges(undoRedoIndex);
    };
    $scope.OnBottomAlignClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var maxYValue = 0;
            if ($scope.lstMultipleSelectedShapes[0]) {
                maxYValue = $scope.lstMultipleSelectedShapes[0].Top + $scope.lstMultipleSelectedShapes[0].Height;
            }
        }
        if (maxYValue > 0) {
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var ydiff = (maxYValue - objShape.Height) - objShape.Top;
                        objShape.Top = maxYValue - objShape.Height;
                        if (objShape.LabelTop && objShape.LabelTop > 0 && (objShape.LabelTop + ydiff) > 0) {
                            objShape.LabelTop = objShape.LabelTop + ydiff;
                        } else {
                            objShape.LabelTop = objShape.Top + 25;
                        }
                        //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                        //{
                        //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                        //}
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }
        RedrawingElementsAndEdges($scope);
        $scope.lstMultipleSelectedShapes = [];
        //this.CheckEdges(undoRedoIndex);
    };

    $scope.OnCenterVerticallyClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var objSelectedShape = null;
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                    if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                        if ($scope.lstMultipleSelectedShapes[0] == objShape) {
                            objSelectedShape = objShape;
                            break;
                        }
                    }
                }
            }
        }
        if (objSelectedShape != null) {
            var xVal = objSelectedShape.Left + (objSelectedShape.Width / 2);
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var xdiff = (xVal - (objShape.Width / 2)) - objShape.Left;
                        objShape.Left = (xVal - (objShape.Width / 2));
                        if (objShape.LabelLeft && objShape.LabelLeft > 0 && (objShape.LabelLeft + xdiff) > 0) {
                            objShape.LabelLeft = objShape.LabelLeft + xdiff;
                        } else {
                            objShape.LabelLeft = objShape.Left + 1;
                        }

                        //ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }
        $scope.$evalAsync(function () {
            for (var i = 0; i < $scope.lstMultipleSelectedShapes.length; i++) {
                ReArrangeEdges($scope.lstMultipleSelectedShapes[i], $scope);
            }
            RedrawingElementsAndEdges($scope);
            $scope.lstMultipleSelectedShapes = [];
        });
    };
    $scope.OnCenterHorizontallyClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var objSelectedShape = null;
            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                    if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                        if ($scope.lstMultipleSelectedShapes[0] == objShape) {
                            objSelectedShape = objShape;
                            break;
                        }
                    }
                }
            }
        }
        if (objSelectedShape != null) {
            var yVal = objSelectedShape.Top + (objSelectedShape.Height / 2);

            for (var i = 0; i < $scope.objData.lstShapes.length; i++) {
                var objShape = $scope.objData.lstShapes[i];
                if ($scope.lstMultipleSelectedShapes.indexOf(objShape) > -1) {
                    if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                        var ydiff = (yVal - (objShape.Height / 2)) - objShape.Top;
                        objShape.Top = (yVal - (objShape.Height / 2));
                        if (objShape.LabelTop && objShape.LabelTop > 0 && (objShape.LabelTop + ydiff) > 0) {
                            objShape.LabelTop = objShape.LabelTop + ydiff;
                        } else {
                            objShape.LabelTop = objShape.Top + 25;
                        }
                        ReArrangeEdges(objShape, $scope);
                    }
                }
            }
        }
        RedrawingElementsAndEdges($scope);
        $scope.lstMultipleSelectedShapes = [];
    };

    $scope.OnDistributeHorizontalClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            var planewidth = parseFloat($scope.svgElement.getAttribute("width"));
            $scope.isDirty = true;
            var minXValue = planewidth;
            var maxXValue = 0;
            for (var i = 0; i < $scope.lstMultipleSelectedShapes.length; i++) {
                var objShape = $scope.lstMultipleSelectedShapes[i];
                if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                    if (minXValue > objShape.Left) {
                        minXValue = objShape.Left;
                    }
                    if (maxXValue < objShape.Left) {
                        maxXValue = objShape.Left;
                    }
                }
            }

            if (maxXValue > 0.0 && minXValue < planewidth) {
                sortNumbersBasedOnproperty($scope.lstMultipleSelectedShapes, "", "Left");
                var lstShapes = $scope.lstMultipleSelectedShapes;
                var MidVal = (maxXValue - minXValue) / (lstShapes.length - 1);
                var dummyVal = MidVal;
                var index = 0;
                for (var i = 0; i < lstShapes.length; i++) {
                    for (var j = 0; j < $scope.objData.lstShapes.length; j++) {
                        if (lstShapes[i].Id == $scope.objData.lstShapes[j].Id) {
                            var objShape = $scope.objData.lstShapes[j];
                            if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                                if (index == 0) {
                                    dummyVal = MidVal + objShape.Left;
                                }
                                else if (index > 0 && index < lstShapes.length) {

                                    var xdiff = dummyVal - objShape.Left;
                                    objShape.Left = dummyVal;
                                    if (objShape.LabelLeft && objShape.LabelLeft > 0 && (objShape.LabelLeft + xdiff) > 0) {
                                        objShape.LabelLeft = objShape.LabelLeft + xdiff;
                                    } else {
                                        objShape.LabelLeft = objShape.Left + 1;
                                    }
                                    ReArrangeEdges(objShape, $scope);
                                    //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                                    //{
                                    //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                                    //}
                                    dummyVal += MidVal;
                                }
                                index++;
                            }
                        }
                    }
                }
            }
            RedrawingElementsAndEdges($scope);
            $scope.lstMultipleSelectedShapes = [];
            //this.CheckEdges(undoRedoIndex);
        }
    };

    $scope.OnDistributeVerticalClick = function () {
        if ($scope.lstMultipleSelectedShapes && $scope.lstMultipleSelectedShapes.length > 1) {
            $scope.isDirty = true;
            var planeheight = parseFloat($scope.svgElement.getAttribute("height"));
            var minYValue = planeheight;
            var maxYValue = 0;
            for (var i = 0; i < $scope.lstMultipleSelectedShapes.length; i++) {
                var objShape = $scope.lstMultipleSelectedShapes[i];
                if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                    if (minYValue > objShape.Top) {
                        minYValue = objShape.Top;
                    }
                    if (maxYValue < objShape.Top) {
                        maxYValue = objShape.Top;
                    }
                }
            }

            if (maxYValue > 0.0 && minYValue < planeheight) {
                sortNumbersBasedOnproperty($scope.lstMultipleSelectedShapes, "", "Top");
                var lstShapes = $scope.lstMultipleSelectedShapes;
                var MidVal = (maxYValue - minYValue) / (lstShapes.length - 1);
                var dummyVal = MidVal;
                var index = 0;
                for (var i = 0; i < lstShapes.length; i++) {
                    for (var j = 0; j < $scope.objData.lstShapes.length; j++) {
                        if (lstShapes[i].Id == $scope.objData.lstShapes[j].Id) {
                            var objShape = $scope.objData.lstShapes[j];
                            if (objShape.ShapeName != "BPMNEdge" && objShape.ShapeType != "message") {
                                if (index == 0) {
                                    dummyVal = MidVal + objShape.Top;
                                }
                                else if (index > 0 && index < lstShapes.length) {
                                    var ydiff = dummyVal - objShape.Top;
                                    objShape.Top = dummyVal;
                                    if (objShape.LabelTop && objShape.LabelTop < planeheight && (objShape.LabelTop + ydiff) < planeheight) {
                                        objShape.LabelTop = objShape.LabelTop + ydiff;
                                    } else {
                                        objShape.LabelTop = objShape.Top + 25;
                                    }
                                    ReArrangeEdges(objShape, $scope);
                                    //if (objShape.BPMNElement != null && (objShape.BPMNElement is BPMNTaskVM || objShape.BPMNElement is BPMNSubProcessVM))
                                    //{
                                    //    this.UpdateBoundaryEvent(objShape as BPMNShapeVM, undoRedoIndex);
                                    //}
                                    dummyVal += MidVal;
                                }
                                index++;
                            }
                        }
                    }
                }
            }
            RedrawingElementsAndEdges($scope);
            $scope.lstMultipleSelectedShapes = [];
            //this.CheckEdges(undoRedoIndex);
        }
    };

    $scope.NavigateToCalledElementFromContextMenu = function (fileName) {
        $.connection.hubBPMN.server.getBPMNFileInfo(fileName).done(function (data) {
            var fileModel = data;
            if (fileModel) {
                $rootScope.openFile(fileModel);
            }
        });
    };

    $scope.MoveElementUsingKeyBoard = function (param) {
        if ($scope.selectedShape && $scope.selectedElemForResize) {
            if ($scope.selectedShape.ShapeType != "lane") {
                if (param == "Down") {
                    moveElementUsingKeyBoard($scope, 0, 2);
                } else if (param == "Top") {
                    moveElementUsingKeyBoard($scope, 0, -2);
                } else if (param == "Left") {
                    moveElementUsingKeyBoard($scope, -2, 0);
                } else if (param == "Right") {
                    moveElementUsingKeyBoard($scope, 2, 0);
                }
            }
        }
    };


    $scope.OnSelectLeftBPMTab = function (opt) {
        if (opt == 'Properties') {
            $scope.ActiveTabForBPM = 'Properties';
            if (!$scope.IsPropsExpanded) {
                $scope.IsPropsExpanded = true;
            }
        }
        else if (opt == 'Toolbox') {
            $scope.ActiveTabForBPM = 'Toolbox';
        }
        else if (opt == 'Versions') {
            $scope.ActiveTabForBPM = 'Versions';
            $scope.versionWiseDisplay();
        }
    };

    $scope.toggleSideBar = function () {
        $scope.IsToolsDivCollapsed = !$scope.IsToolsDivCollapsed;
    };

    $scope.showEnterExitDialog = function (isEnter, model, entityId) {
        var dialogScope = $scope.$new();
        dialogScope.isEnter = isEnter;
        dialogScope.model = JSON.parse(JSON.stringify(model));
        dialogScope.mapvarids = $getQueryparam.getMapVariableIds($scope.mapVariablesModel.Elements);
        dialogScope.getquerydialog = $scope.getQuery_dialog;
        dialogScope.entityid = entityId;
        dialogScope.parameterallowedvalues = $scope.queryParameterallowedValues;
        dialogScope.IsDisabledforVersioning = $scope.IsDisabledforVersioning;
        dialogScope.addVariable = function () {
            var newItem = { prefix: "sbpmn", Name: "variable", dictAttributes: {}, Elements: [], Value: "", Children: [] };
            dialogScope.model.Elements.push(newItem);
            dialogScope.SelectedVariable = newItem;
        };
        dialogScope.deleteVariable = function () {
            if (dialogScope.SelectedVariable) {
                var index = dialogScope.model.Elements.indexOf(dialogScope.SelectedVariable);
                if (index > -1) {
                    //Get and select the next variable, before removing the desired item.
                    dialogScope.SelectedVariable = index + 1 == dialogScope.model.Elements.length ? dialogScope.model.Elements[index - 1] : dialogScope.model.Elements[index + 1];
                    dialogScope.model.Elements.splice(index, 1);
                }
            }

        };
        dialogScope.onVariableTypeChanged = function (variable) {
            variable.dictAttributes.sfwEntityField = "";
            variable.dictAttributes.sfwQueryID = "";
        };
        dialogScope.onOKClick = function () {
            $rootScope.UndRedoBulkOp("Start");

            while (model.Elements.length > 0) {
                $rootScope.DeleteItem(model.Elements[0], model.Elements, null);
            }

            for (var idx = 0; idx < dialogScope.model.Elements.length; idx++) {
                $rootScope.PushItem(dialogScope.model.Elements[idx], model.Elements);
            }
            $rootScope.UndRedoBulkOp("End");

            dialogScope.closeDialog();
        };
        dialogScope.closeDialog = function () {
            if (dialogScope.dialog)
                dialogScope.dialog.close();
        };
        dialogScope.dialog = $rootScope.showDialog(dialogScope, "Set " + (isEnter ? "Enter" : "Exit") + " Actions", "BPM/views/Association/EnterExitTemplate.html", { width: 1100 });
    };


    $scope.saveBPMNAsPng = function () {

        $rootScope.isIEBrowser = false;
        var userAgent = window.navigator.userAgent;
        if (userAgent.indexOf("MSIE ") > -1 || userAgent.indexOf("Trident/") > -1 || userAgent.indexOf("Edge/") > -1) {
            $rootScope.isIEBrowser = true;
        }
        var svgElement = $scope.svgElement;
        var html = new XMLSerializer().serializeToString($scope.svgElement);
        var imgsrc = 'data:image/svg+xml;base64,' + btoa(html);

        var w = open("");
        if (w) {
            $scope.objSvgLimits = $scope.getMaxHeightAndWidthofSvg($scope.BPMNModel.lstShapes);
            var image = new Image;
            image.crossOrigin = 'Anonymous';
            if ($rootScope.isIEBrowser) {
                if (userAgent.indexOf("Edge/") > -1) {
                    image.style.width = $scope.objSvgLimits.Width + 100;
                    image.style.height = $scope.objSvgLimits.Height + 100;
                } else {
                    image.width = $scope.objSvgLimits.Width + 100;
                    image.height = $scope.objSvgLimits.Height + 100;
                }
                image.src = imgsrc;
                image.onload = function () {
                    w.document.write(image.outerHTML);
                }
            } else {
                var canvasEle = document.createElement("canvas");
                canvasEle.width = $scope.objSvgLimits.Width + 100;
                canvasEle.height = $scope.objSvgLimits.Height + 100;
                document.body.appendChild(canvasEle);
                var context = canvasEle.getContext("2d");
                image.src = imgsrc;
                image.onload = function () {
                    context.drawImage(image, 0, 0);
                    var canvasdata = canvasEle.toDataURL("image/png");
                    document.body.removeChild(canvasEle);
                    var img = new Image;
                    img.src = canvasdata;
                    w.document.write(img.outerHTML);
                }
            }
        } else {
            alert("please allow pop-up");
        }

        // #region If we want direct download , rather open in new tab 
        //var canvasEle = document.createElement("canvas");
        //canvasEle.width = $scope.objSvgLimits.Width + 100;
        //canvasEle.height = $scope.objSvgLimits.Height + 100;

        //$("#crowbar-workspace")[0].appendChild(canvasEle);
        //var context = canvasEle.getContext("2d");
        //var image = new Image;
        //image.src = imgsrc;
        //image.crossOrigin = 'Anonymous';

        //image.onload = function () {
        //context.drawImage(image, 0, 0);

        //if ($rootScope.isIEBrowser) { // for IE and edge
        //    $(image).dialog({
        //        //modal: true,
        //        resizable: true,
        //        draggable: true,
        //        width: $scope.objSvgLimits.Width + 100,
        //        height: $scope.objSvgLimits.Height + 100
        //    });
        //} else { // other than IE and edge
        //    var canvasdata = canvasEle.toDataURL("image/png");
        //    // download the data
        //    var a = document.createElement("a");
        //    document.body.appendChild(a);//for firefox , optional for chrome
        //    a.download = $rootScope.currentopenfile.file.FileName + ".png";
        //    a.href = canvasdata;
        //    a.click();
        //    document.body.removeChild(a);
        //    $("#crowbar-workspace")[0].removeChild(canvasEle);
        //}
        //#endregion

    };

}]);

function onKeyDownInSVG(event) {
    //var scope = getCurrentFileScope();
    var scope = angular.element(event.currentTarget).scope();
    if (scope) {
        scope.$apply(function () {
            if (scope.activeTab == "MAP") {
                if (event.keyCode == 46) {
                    scope.onDeleteKeyDown(event, "");
                } else if (event.keyCode == 40) {
                    scope.MoveElementUsingKeyBoard("Down");
                } else if (event.keyCode == 38) {
                    scope.MoveElementUsingKeyBoard("Top");
                } else if (event.keyCode == 37) {
                    scope.MoveElementUsingKeyBoard("Left");
                } else if (event.keyCode == 39) {
                    scope.MoveElementUsingKeyBoard("Right");
                }
                if (event && event.keyCode == 40 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 38) {
                    event.preventDefault();
                }
            }
        });
    }
}
var objElements = function () {
    return {
        dictAttributes: {}, Elements: [], Children: [], Name: "", prefix: "sbpmn"
    };
};
