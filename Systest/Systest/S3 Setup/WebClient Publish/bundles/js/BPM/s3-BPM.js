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


function userTaskAssociationPropertiesController($scope, $filter, $rootScope) {
    var scope = this;
    scope.$onInit = function () {
        scope.init();
    };
    scope.init = function () {

        scope.currentUserTaskData = { selectedEntity: "BpmActivityInstance", formList: [] };

        var extensionElements = $filter('filter')(scope.shapeModel.Elements, { Name: "extensionElements" });
        if (extensionElements && extensionElements.length > 0) {
            scope.extensionElementModel = extensionElements[0];
        }
        if (scope.extensionElementModel) {
            var items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "ReferenceId" });
            if (items && items.length > 0) {
                scope.referenceIDModel = items[0];
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "form", dictAttributes: { sfwMode: "new" } });
            if (items && items.length > 0) {
                scope.enterFormModel = items[0];
            }
            else {
                scope.enterFormModel = scope.getNewEnterReEnterModel("new");
                scope.enterFormModel.dictAttributes.sfwFormMode = "new";
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "form", dictAttributes: { sfwMode: "update" } });
            if (items && items.length > 0) {
                scope.reEnterFormModel = items[0];
            }
            else {
                scope.reEnterFormModel = scope.getNewEnterReEnterModel("update");
            }
            items = $filter('filter')(scope.extensionElementModel.Elements, { Name: "actions" });
            if (items && items.length > 0) {
                scope.actionsModel = items[0];
            }
        }
    };

    scope.onModeChange = function () {
        if (scope.enterFormModel && scope.enterFormModel.Elements.length > 0) {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = scope.enterFormModel.Elements.length - 1; i >= 0; i--) {
                var parameter = scope.enterFormModel.Elements[i];
                $rootScope.DeleteItem(parameter, scope.enterFormModel.Elements);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    scope.getNewEnterReEnterModel = function (mode) {
        var model = { prefix: "sbpmn", Name: "form", Value: "", dictAttributes: { sfwMode: mode }, Elements: [], Children: [] };
        return model;
    };
}
app.component('bpmUserTaskAssociationProperties', {
    bindings: {
        queryParametersValues: "<",
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        isObjectbased: "<?"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", userTaskAssociationPropertiesController],
    templateUrl: 'BPM/views/Association/UserTaskAssociationProperties.html',
});

function businessRuleTaskAssociationController($scope, $filter, $rootScope, $getEnitityRule, $EntityIntellisenseFactory, $getEnitityXmlMethods) {
    var ctrl = this;
    ctrl.$onInit = function () {
        $scope.init();
    };
    var objtemp = {};
    $scope.init = function () {
        $scope.arrMapVariableforEntity = [];
        var extensionElement = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extensionElements" })[0];
        var rule = $filter('filter')(extensionElement.Elements, { Name: "rule" })[0];
        $scope.model = rule;

        var parametersNode = $filter('filter')($scope.model.Elements, { Name: "parameters" }, true);
        if (parametersNode && parametersNode.length > 0) {
            $scope.model.parameterschild = parametersNode[0];
        }

        // in case of bussiness rule there are two child nodes 
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule') {
            $scope.model.baseChild = $filter('filter')(rule.Elements, { Name: "ServiceBusinessObject" }, true)[0];

            if ($scope.model.baseChild && $scope.model.baseChild.Elements) {
                if ($scope.model.baseChild.dictAttributes.variable) {
                    $scope.model.baseChild.FailOverServiceModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "FailOverServiceBusinessObject" }, true)[0];
                    $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.FailOverServiceModel.Elements, { Name: "initialize" }, true)[0];
                    $scope.selectedTab = "FailOverServiceBusinessObject";
                } else {
                    $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "initialize" }, true)[0];
                }
                $scope.model.baseChild.entryModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
                $scope.model.baseChild.exitModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnExit" }, true)[0];

                if ($scope.model.baseChild.dictAttributes.sfwEntity && $scope.model.baseChild.entryModel && $scope.model.baseChild.exitModel) { /*Bug 8830:Exception on clicking service task of existing map in BPM*/
                    var entryElement = $scope.model.baseChild.entryModel.Elements;
                    var exitElement = $scope.model.baseChild.exitModel.Elements;
                    var sfwNavigationParameter = $scope.model.baseChild.dictAttributes.sfwNavigationParameter;
                    $scope.getMapVariableEntityWise();
                    if ($scope.isBusinessRuleTypeValid) {
                        if ($scope.model.baseChild.initializeModel) {
                            $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                        }
                        if ($scope.model.baseChild.initializeModel.xmlmethodModel && $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes && $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                            var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.xmlMethods, { ID: $scope.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true)[0];
                            $scope.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                            if (sfwNavigationParameter) $scope.model.baseChild.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                        }
                        if (entryElement.length > 0) $scope.model.baseChild.entryModel.Elements = entryElement;
                        if (exitElement.length > 0) $scope.model.baseChild.exitModel.Elements = exitElement;
                    }
                }
            }
            if ($scope.model.dictAttributes.ID) {
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
                var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
                if (objRule) { $scope.returnType = objRule.ReturnType; }
            }
        }
        // for static rule 
        if ($scope.model.dictAttributes.sfwEntity && $scope.model.dictAttributes.sfwType === 'StaticRule') {
            objtemp = $getEnitityRule.getWithParam($scope.model.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
            $scope.arrRuleIds = objtemp.ruleIds;
            var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
            if (objRule) { $scope.returnType = objRule.ReturnType; }
        }
    };
    $scope.onChangeType = function (type) {
        $scope.model.dictAttributes.sfwType = type;
        $scope.Clear("baserule");
        $scope.isBusinessRuleTypeValid = false;
        var nodeParameters = new objElements();
        switch ($scope.model.dictAttributes.sfwType.toLowerCase()) {
            case "staticrule": nodeParameters.Name = "parameters"; break;
            case "businessrule":
                var childName = ["initialize", "OnEnter", "OnExit"];
                nodeParameters.Name = "ServiceBusinessObject";
                for (i = 0; i < childName.length; i++) {
                    var nodeServiceChild = new objElements();
                    nodeServiceChild.Name = childName[i];
                    nodeParameters.Elements.push(nodeServiceChild);
                }
                break;
        }
        $scope.model.Elements.push(nodeParameters);
        var ServiceBusiness = $filter('filter')($scope.model.Elements, { Name: "ServiceBusinessObject" });
        var StaticQuery = $filter('filter')($scope.model.Elements, { Name: "parameters" });
        if (StaticQuery && StaticQuery.length > 0) {
            $scope.model.parameterschild = StaticQuery[0];
        }
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule' && ServiceBusiness.length == 1) {
            $scope.model.baseChild = ServiceBusiness[0];
            $scope.model.baseChild.initializeModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "initialize" }, true)[0];
            $scope.model.baseChild.initializeModel.dictAttributes.ObjectType = "XmlMethod";
            $scope.model.baseChild.entryModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
            $scope.model.baseChild.exitModel = $filter('filter')($scope.model.baseChild.Elements, { Name: "OnExit" }, true)[0];

            // add xml method to initialize here only - because object is not an option 
            if ($scope.model.baseChild.initializeModel) {
                var nodeXmlmethod = new objElements();
                nodeXmlmethod.Name = "XMLMethod";
                $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
            }
            // check this code if it is used or not
            if ($scope.model.baseChild.dictAttributes.sfwEntity) {
                var nodeParameterssecond = new objElements();
                nodeParameterssecond.Name = "parameters";
                $scope.model.Elements.push(nodeParameterssecond);
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
            }
        }
        else $scope.model.baseChild = StaticQuery[0];
    };
    $scope.onVariableChanged = function () {
        if ($scope.model && $scope.model.baseChild && $scope.model.baseChild.dictAttributes) {
            if ($scope.model.baseChild.dictAttributes.variable && $scope.model.baseChild.dictAttributes.variable.trim().length > 0) {
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                $scope.RemoveInitializeorFailOverModel("initialize");
                $scope.PrepareFailOverServiceBusObjModel();
                $scope.model.baseChild.Elements.splice(0, 0, $scope.model.baseChild.FailOverServiceModel);
                $scope.selectedTab = "FailOverServiceBusinessObject";
            }
            else {
                $scope.RemoveInitializeorFailOverModel("FailOverServiceBusinessObject");
                $scope.PrepareInitializeModel();
                $scope.model.baseChild.Elements.splice(0, 0, $scope.model.baseChild.initializeModel);
                $scope.selectedTab = "Initialize";
            }
        }
    };

    $scope.RemoveInitializeorFailOverModel = function (param) {
        for (var i = 0; i < $scope.model.baseChild.Elements.length; i++) {
            if ($scope.model.baseChild.Elements[i].Name == param) {
                $scope.model.baseChild.Elements.splice(i, 1);
                break;
            }
        }
    };

    $scope.PrepareInitializeModel = function () {
        var initializeModel = {
            dictAttributes: { ObjectType: "XmlMethod" },
            Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "XMLMethod", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" }],
            Children: [], Name: "initialize", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        $scope.model.baseChild.initializeModel = initializeModel;
        $scope.loadXmlmethods();
        var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
        if (xmlmethod.length <= 0) {
            var nodeXmlmethod = new objElements();
            nodeXmlmethod.Name = "XMLMethod";
            $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
        }
        $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
        $scope.isBusinessRuleTypeValid = true;
    };
    $scope.PrepareFailOverServiceBusObjModel = function () {
        $scope.PrepareInitializeModel();
        var FailOverServiceModel = {
            dictAttributes: { sfwNavigationParameter: "" },
            Elements: [],
            Children: [], Name: "FailOverServiceBusinessObject", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        FailOverServiceModel.Elements.push($scope.model.baseChild.initializeModel);
        $scope.model.baseChild.FailOverServiceModel = FailOverServiceModel;
    };

    $scope.loadXmlmethods = function () {
        $scope.model.baseChild.initializeModel.xmlMethods = $getEnitityXmlMethods.get($scope.model.baseChild.dictAttributes.sfwEntity.trim());
    };
    $scope.Clear = function (type) {
        // clear this model for all childerns 
        switch (type) {
            case "baserule": $scope.model.dictAttributes.sfwEntity = "";
            case "type":
                $scope.model.baseChild = {};
                $scope.model.Elements = [];
                $scope.model.dictAttributes.sfwReturnField = "";
                $scope.model.dictAttributes.ID = "";
                $scope.returnType = "";
                $scope.arrRuleIds = "";
                break;
            default: break;
        }
    };
    $scope.getRuledetails = function () {
        if ($scope.model.dictAttributes.sfwType === 'BusinessRule') objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        else if ($scope.model.dictAttributes.sfwType === 'StaticRule') {
            $scope.Clear("type");
            objtemp = $getEnitityRule.getWithParam($scope.model.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        }
        $scope.arrRuleIds = objtemp.ruleIds;
    };
    $scope.getRuleParmeters = function () {
        $scope.returnType = "";
        if (!$scope.model.dictAttributes.ID) {
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwReturnField, $scope.model.dictAttributes, "sfwReturnField", "");
        }
        if ($scope.model.parameterschild) {
            $scope.model.parameterschild.Elements = [];
        }
        else {
            var parametersNode = new objElements();
            parametersNode.Name = "parameters";
            $scope.model.Elements.push(parametersNode);
            $scope.model.parameterschild = parametersNode;
        }
        var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
        if (objRule) {
            $scope.returnType = objRule.ReturnType;
            if (objRule.Parameters.length > 0) {
                if (!($scope.model.Elements.length > 0) && $scope.model.dictAttributes.sfwType !== 'BusinessRule') {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameters";
                    $scope.model.Elements.push(nodeParameters);
                }
                if (!($scope.model.Elements.length > 1) && $scope.model.dictAttributes.sfwType === 'BusinessRule') {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameters";
                    $scope.model.Elements.push(nodeParameters);
                }
                var parametersNode = $filter('filter')($scope.model.Elements, { Name: "parameters" });
                if (parametersNode.length == 1) {
                    $scope.model.parameterschild = parametersNode[0];
                    $scope.model.parameterschild.Elements = [];
                    function iterator(value, key) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        nodeParameters.dictAttributes.sfwParamaterName = value.DisplayName; nodeParameters.dictAttributes.sfwDirection = value.Direction; nodeParameters.dictAttributes.sfwDataType = value.DataType;
                        this.Elements.push(nodeParameters);
                    }
                    angular.forEach(objRule.Parameters, iterator, parametersNode[0]);
                }
            }
        }
    };
    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };
    $scope.getMapVariableEntityWise = function () {
        $scope.arrMapVariableforEntity = [];
        //check if this is valid - model.baseChild.baseChild.dictAttributes.type => isBusinessRuleTypeValid
        if ($scope.model.baseChild.dictAttributes && $scope.model.baseChild.dictAttributes.sfwEntity && $scope.model.baseChild.dictAttributes.sfwEntity.trim().length > 0 && $scope.model.dictAttributes.sfwType.toLowerCase() === 'businessrule') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = $filter('filter')(entityIntellisenseList, { ID: $scope.model.baseChild.dictAttributes.sfwEntity.trim() }, true);
            if (entity.length == 1) {
                $scope.loadXmlmethods();
                $scope.model.baseChild.entryModel.Elements = [];
                $scope.model.baseChild.exitModel.Elements = [];
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                objtemp = $getEnitityRule.getWithParam($scope.model.baseChild.dictAttributes.sfwEntity, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
                $scope.arrRuleIds = objtemp.ruleIds;
                var mapVar = $filter('filter')($scope.$parent.mapVariablesModel.Elements, { dictAttributes: { sfwEntity: $scope.model.baseChild.dictAttributes.sfwEntity.trim() } }, true);
                function iterator(value, key) {
                    this.push(value.dictAttributes.id);
                }
                angular.forEach(mapVar, iterator, $scope.arrMapVariableforEntity);
                // add xml method model
                var xmlmethod = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
                if (xmlmethod.length <= 0) {
                    var nodeXmlmethod = new objElements();
                    nodeXmlmethod.Name = "XMLMethod";
                    $scope.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                    $scope.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')($scope.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                }
                $scope.isBusinessRuleTypeValid = true;
            }
            else {
                $scope.model.baseChild.initializeModel.Elements = [];
                $scope.model.baseChild.initializeModel.xmlMethods = [];
                $scope.model.baseChild.initializeModel.xmlmethodItems = [];
                $scope.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                $scope.returnType = "";
                $scope.model.dictAttributes.sfwReturnField = "";
                $scope.model.dictAttributes.ID = "";
                if ($scope.model.parameterschild) $scope.model.parameterschild.Elements = [];
                $scope.arrRuleIds = [];
                $scope.isBusinessRuleTypeValid = false;
            }
        }
        else $scope.isBusinessRuleTypeValid = false;
    };
    $scope.refreshRuleParameters = function (event) {
        var ruleId = "";
        if ($scope.model.baseChild && $scope.model.baseChild.dictAttributes) {
            ruleId = $scope.model.baseChild.dictAttributes ? $scope.model.baseChild.dictAttributes.sfwEntity : undefined;
        }
        if ($scope.model.dictAttributes.sfwType === 'StaticRule') {
            ruleId = $scope.model.dictAttributes.sfwEntity;
        }
        objtemp = $getEnitityRule.getWithParam(ruleId, $scope.model.dictAttributes.sfwType === 'StaticRule' ? true : false, "LogicalRule,DecisionTable");
        var objRule = $filter('filter')(objtemp.objRule, { ID: $scope.model.dictAttributes.ID }, true)[0];
        if (objRule) {
            $scope.returnType = objRule.ReturnType;
            var parameter = angular.copy(objRule.Parameters);
            var parameterParent = $scope.model.parameterschild;
            if (!parameterParent) {
                var parametersObj = new objElements();
                parametersObj.prefix = "";
                parametersObj.Name = "parameters";
                parameterParent = parametersObj;
                $rootScope.PushItem(parametersObj, $scope.model.Elements);
            }
            for (var i = parameterParent.Elements.length - 1; i >= 0; i--) {
                items = $filter('filter')(parameter, { ID: parameterParent.Elements[i].dictAttributes.sfwParamaterName });
                if (items && items.length > 0) {
                    var index = parameter.indexOf(items[0]);
                    parameter.splice(index, 1);
                }
                else {
                    $rootScope.DeleteItem(parameterParent.Elements[i], parameterParent.Elements);
                }
            }
            // objRule.Parameters will have only the new parameters
            function iterator(value, key) {
                var nodeParameters = new objElements();
                nodeParameters.Name = "parameter";
                nodeParameters.dictAttributes.ID = value.ID; nodeParameters.dictAttributes.sfwParamaterName = value.DisplayName; nodeParameters.dictAttributes.sfwDirection = value.Direction; nodeParameters.dictAttributes.sfwDataType = value.DataType;
                this.Elements.push(nodeParameters);
            }
            angular.forEach(parameter, iterator, parameterParent);
        }

        event.preventDefault();
        event.stopPropagation();
    };
    $scope.NavigateToFile = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            hubMain.server.navigateToFile(aEntityID, "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };
    $scope.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    $scope.setEnterExitActions = function (isEnter) {
        if ($scope.$parent.showEnterExitDialog) {
            $scope.$parent.showEnterExitDialog(isEnter, isEnter ? $scope.model.baseChild.entryModel : $scope.model.baseChild.exitModel, $scope.model.baseChild.dictAttributes.sfwEntity);
        }
    };
}
app.component("bpmBusinessRuleTaskAssociationProperties", {
    bindings: {
        shapeModel: "<",
        isDisabledforVersioning: "<?",
        queryParametersValues: "<",
        isObjectbased: "<?"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", "$getEnitityRule", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", businessRuleTaskAssociationController],
    templateUrl: "BPM/views/Association/PropertiesBussinessRuleTask.html"
});

function initializeController($scope, $filter, $rootScope, $getQueryparam, $NavigateToFileService, $getEnitityXmlMethods) {
    var ctrl = this;
    $scope.getXmlMethods = function () {
        if ($scope.$ctrl.callbackFunction) {
            $scope.$ctrl.callbackFunction();
        }
    };
    $scope.onXmlMethodChange = function () {
        // load navigation parameters and items 
        if (ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
            var xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.xmlMethods, { ID: ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true)[0];
            if (xmlmethod) {
                ctrl.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(xmlmethod.Parameters, iterator, queryParametersDisplay);
                if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
                }
                else {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
                }
            }
        }
        else {
            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                if (ctrl.model.baseChild.FailOverServiceModel && ctrl.model.baseChild.FailOverServiceModel.dictAttributes) {
                    $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", "");
                }
            }
            else {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", "");
            }
        }
    };
    $scope.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    $scope.setXmlparameters = function () {
        // open pop up for setting parameters
        var dialogScope = $scope.$new(true);
        dialogScope.DialogConstants = { queryParamsName: "sfwNavigationParameter" };
        dialogScope.mapObj = {};
        dialogScope.queryValues = ctrl.queryParametersValues;
        if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", ",");
        }
        else {
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", ",");
        }
        dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Xml Method Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
        dialogScope.setQueryParameters = function () {
            var queryParametersDisplay = [];
            function iterator(value, key) {
                this.push(value.ID + '=' + (value.Value ? value.Value : ""));
            }
            angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.FailOverServiceModel.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
            }
            else {
                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", queryParametersDisplay.join(","));
            }
            dialogScope.QueryDialog.close();
        };

        dialogScope.onRefreshParams = function () {
            var initializeModelXmlMethods = $getEnitityXmlMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim())
            if (initializeModelXmlMethods.length > 0) {
                var initializeXmlMethods = $filter('filter')(initializeModelXmlMethods, { ID: ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID }, true);
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

}
app.component("bpmInitializeSection", {
    bindings: {
        model: "<",
        queryParametersValues: "<",
        isDisabledforVersioning: "<",
        callbackFunction: "&"
    },
    replace: true,
    controller: ["$scope", "$filter", "$rootScope", "$getQueryparam", "$NavigateToFileService", "$getEnitityXmlMethods", initializeController],
    templateUrl: "BPM/views/Association/InitializeTemplate.html"
});

function bpmCallactivityAssociationPropertiesController($rootScope, $scope) {
    var ctrl = this;
    ctrl.$onInit = function () {
        function filterExtension(x) {
            function filterParameters(y) {
                return y.Name == "parameters";
            }
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                ctrl.objExtensionModel = x;
                var objParameterModel = x.Elements.filter(filterParameters)[0];
                if (objParameterModel) {
                    ctrl.objParameterModel = objParameterModel;
                    return true;
                }
            }
        }
        ctrl.model.Elements.some(filterExtension);
        if (ctrl.objParameterModel) {
            for (var idx = 0; idx < ctrl.objParameterModel.Elements.length; idx++) {
                if (ctrl.objParameterModel.Elements[idx].dictAttributes.sfwDataType == "Object" && ctrl.objParameterModel.Elements[idx].dictAttributes.sfwValueSource == "Constant") {
                    ctrl.checkAndUpdateParamChildModels({ param: ctrl.objParameterModel.Elements[idx], fromUndoRedoBlock: false });
                }
            }
        }
    };
    ctrl.refreshParameters = function () {
        var processBpmName = ctrl.model.dictAttributes.calledElement ? ctrl.model.dictAttributes.calledElement.split('.')[0] : null;
        if (processBpmName) {
            $rootScope.UndRedoBulkOp("Start");
            ctrl.setCalledelementparameters({ mapName: processBpmName, extensionElementModel: ctrl.objExtensionModel ? ctrl.objExtensionModel : null });
            //  if parameter model was not set initially
            if (ctrl.objExtensionModel) {
                function filterParameters(z) {
                    if (z.Name == "parameters") {
                        ctrl.objParameterModel = z;
                        return true;
                    }
                }
                ctrl.objExtensionModel.Elements.some(filterParameters);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    ctrl.setVParamEntityValue = function (param) {
        ctrl.setParamEntityValue({ parentScope: $scope, param: param });
    };
}

app.component('bpmCallactivityShapeAssociationProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        queryParametersvalues: '<',
        setCalledelementparameters: '&',
        onValueSourceChanged: '&',
        setParamEntityValue: '&',
        checkAndUpdateParamChildModels: '&'
    },
    templateUrl: 'BPM/views/Association/CallActivityAssociationProperties.html',
    controller: ["$rootScope", "$scope", bpmCallactivityAssociationPropertiesController]
});

function bpmServiceRuleAssociationPropertiesController($rootScope, $scope, $filter, $EntityIntellisenseFactory, $getEnitityXmlMethods, $getEnitityObjectMethods, $NavigateToFileService, $getQueryparam) {
    var ctrl = this;
    ctrl.$onInit = function () {
        ctrl.isBusinessRuleTypeValid = false;
        // ctrl.arrMapVariableforEntity = [];
        function filterextensionElements(x) {
            function filterBaseChild(y) {
                function filterInitializeModel(i) {
                    if (i.Name == "initialize") {
                        ctrl.model.baseChild.initializeModel = i;
                    }
                }
                function filterFailOver(z) {
                    if (z.Name == "FailOverServiceBusinessObject") {
                        ctrl.model.baseChild.FailOverServiceModel = z;
                        z.Elements.some(filterInitializeModel);
                    }
                }
                function filterModel(i) {
                    if (i.Name == "OnEnter") {
                        ctrl.model.baseChild.entryModel = i;
                    }
                    else if (i.Name == "OnExit") {
                        ctrl.model.baseChild.exitModel = i;
                    }
                    else if (i.Name == "ServiceMethod") {
                        ctrl.model.baseChild.ServiceMethodmodel = i;
                    }
                    else if (i.Name == "XMLMethod") {
                        ctrl.model.baseChild.initializeModel.xmlmethodModel = i;
                    }
                }
                function filterXmlModel(i) {
                    if (i.ID == ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                        var xmlmethod = ctrl.model.baseChild.initializeModel.xmlmethodModel;
                        ctrl.model.baseChild.initializeModel.xmlmethodItems = xmlmethod.Items;
                        if (sfwNavigationParameter) {
                            if (ctrl.model.baseChild.dictAttributes.variable) {
                                ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                            } else {
                                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = sfwNavigationParameter;
                            }
                        }
                    }
                }
                // for service business object
                if (y.Name == "ServiceBusinessObject") {
                    ctrl.baseType = "entity";
                    ctrl.model.baseChild = y;
                    if (ctrl.model.baseChild.dictAttributes.variable) {
                        ctrl.model.baseChild.Elements.forEach(filterFailOver);
                        ctrl.selectedTab = "FailOverServiceBusinessObject";
                    }
                    else {
                        ctrl.model.baseChild.Elements.forEach(filterInitializeModel);
                        ctrl.selectedTab = "Initialize";
                    }
                    ctrl.model.baseChild.Elements.forEach(filterModel);
                    if (ctrl.model.baseChild.dictAttributes.variable && !ctrl.model.baseChild.FailOverServiceModel) {
                        ctrl.PrepareFailOverServiceBusObjModel();
                    }
                    if (ctrl.model.baseChild.dictAttributes.sfwEntity) {
                        var entryElement = ctrl.model.baseChild.entryModel.Elements;
                        var exitElement = ctrl.model.baseChild.exitModel.Elements;
                        var sfwNavigationParameter = "";
                        if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.FailOverServiceModel) {
                            sfwNavigationParameter = ctrl.model.baseChild.FailOverServiceModel.dictAttributes.sfwNavigationParameter;
                        }
                        else {
                            sfwNavigationParameter = ctrl.model.baseChild.dictAttributes.sfwNavigationParameter;
                        }
                        ctrl.getMapVariableEntityWise();
                        if (ctrl.isBusinessRuleTypeValid) {
                            if (ctrl.model.baseChild.initializeModel) {
                                ctrl.model.baseChild.initializeModel.Elements.some(filterModel);
                            }
                            if (ctrl.model && ctrl.model.baseChild && ctrl.model.baseChild.ServiceMethodmodel) {
                                for (var idx = 0; idx < ctrl.model.baseChild.ServiceMethodmodel.Elements.length; idx++) {
                                    if (ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].Name == "parameter" &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes.sfwDataType == "Object" &&
                                        ctrl.model.baseChild.ServiceMethodmodel.Elements[idx].dictAttributes.sfwValueSource == "Constant") {
                                        ctrl.checkAndUpdateParamChildModels({ param: ctrl.model.baseChild.ServiceMethodmodel.Elements[idx], fromUndoRedoBlock: false });
                                    }
                                }
                            }
                            if (ctrl.model.baseChild.initializeModel && ctrl.model.baseChild.initializeModel.xmlmethodModel && ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes && ctrl.model.baseChild.initializeModel.xmlmethodModel.dictAttributes.ID) {
                                ctrl.model.baseChild.initializeModel.xmlMethods.forEach(filterXmlModel);
                            }
                            ctrl.model.baseChild.Elements.some(filterModel);
                            if (entryElement.length > 0) ctrl.model.baseChild.entryModel.Elements = entryElement;
                            if (exitElement.length > 0) ctrl.model.baseChild.exitModel.Elements = exitElement;
                        }
                    }
                }
                else if (y.Name == "Queries") {
                    ctrl.model.baseChild = y;
                    ctrl.baseType = "query";
                }
            }
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                ctrl.model = x;
                x.Elements.forEach(filterBaseChild);
                return true;
            }
        }
        ctrl.shapeModel.Elements.some(filterextensionElements);
        if (ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes.variable) {
            ctrl.model.lstrInitVariable = ctrl.model.baseChild.dictAttributes.variable;
        }
        else {
            ctrl.model.lstrInitVariable = "";
        }
    };
    ctrl.onChangebaseType = function (type) {
        ctrl.baseType = type;
        // clear and reload the models
        ctrl.isBusinessRuleTypeValid = false;
        ctrl.model.baseChild = {};
        // newScope.model.Elements = [];
        var nodeParameters = new objElements();
        switch (type) {
            case "query": nodeParameters.Name = "Queries";
                var ServiceBusiness = $filter('filter')(ctrl.model.Elements, { Name: "ServiceBusinessObject" });
                if (ServiceBusiness.length > 0) {
                    var tempindex = ctrl.model.Elements.indexOf(ServiceBusiness[0]);
                    ctrl.model.Elements.splice(tempindex, 1);
                }
                break;
            case "entity":
                var ScalarQuery = $filter('filter')(ctrl.model.Elements, { Name: "Queries" });
                if (ScalarQuery.length > 0) {
                    var tempindex = ctrl.model.Elements.indexOf(ScalarQuery[0]);
                    ctrl.model.Elements.splice(tempindex, 1);
                }
                var childName = ["initialize", "ServiceMethod", "OnEnter", "OnExit"];
                nodeParameters.Name = "ServiceBusinessObject";
                for (i = 0; i < childName.length; i++) {
                    var nodeServiceChild = new objElements();
                    nodeServiceChild.Name = childName[i];
                    nodeParameters.Elements.push(nodeServiceChild);
                }
                break;
        }
        // newScope.model = extension elements
        ctrl.model.Elements.push(nodeParameters);
        var ServiceBusiness = $filter('filter')(ctrl.model.Elements, { Name: "ServiceBusinessObject" });
        var ScalarQuery = $filter('filter')(ctrl.model.Elements, { Name: "Queries" });
        // for service business object
        if (ServiceBusiness.length == 1 && ScalarQuery.length <= 0) {
            ctrl.model.baseChild = ServiceBusiness[0];
            ctrl.model.baseChild.initializeModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "initialize" }, true)[0];
            ctrl.model.baseChild.initializeModel.dictAttributes.ObjectType = "XmlMethod";
            ctrl.model.baseChild.entryModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "OnEnter" }, true)[0];
            ctrl.model.baseChild.exitModel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "OnExit" }, true)[0];
            ctrl.model.baseChild.ServiceMethodmodel = $filter('filter')(ctrl.model.baseChild.Elements, { Name: "ServiceMethod" }, true)[0];
            if (ctrl.model.baseChild.initializeModel) {
                var nodeXmlmethod = new objElements();
                nodeXmlmethod.Name = "XMLMethod";
                ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
            }
        }
        else ctrl.model.baseChild = ScalarQuery[0];
    };
    ctrl.NavigateToFile = function (aEntityID) {
        if (aEntityID) {
            $.connection.hubMain.server.navigateToFile(aEntityID, "").done(function (objfile) {
                $rootScope.openFile(objfile, null);
            });
        }
    };
    ctrl.loadXmlmethods = function () {
        if (ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes && ctrl.model.baseChild.dictAttributes.sfwEntity) {
            if (ctrl.model.baseChild.initializeModel) {
                ctrl.model.baseChild.initializeModel.xmlMethods = $getEnitityXmlMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim());
            }
            ctrl.model.baseChild.ServiceMethodmodel.objectMethods = $getEnitityObjectMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim());
        }
    };
    ctrl.getMapVariableEntityWise = function () {
        //ctrl.arrMapVariableforEntity = [];
        // load service method
        if (ctrl.model.baseChild.dictAttributes && ctrl.model.baseChild.dictAttributes.sfwEntity && ctrl.model.baseChild.dictAttributes.sfwEntity.trim().length > 0 && ctrl.baseType.toLowerCase() === 'entity') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = $filter('filter')(entityIntellisenseList, { ID: ctrl.model.baseChild.dictAttributes.sfwEntity.trim() }, true);
            if (entity.length == 1) {
                ctrl.loadXmlmethods();
                ctrl.model.baseChild.entryModel.Elements = [];
                ctrl.model.baseChild.exitModel.Elements = [];
                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";

                /*--------- commented these lines coz after adding variable(in variable tab ) n coming back to map tab,initialize variable list was not getting updated.--------- */
                /*--------- Instead of "arrMapVariableforEntity" now using "mapVariablesModel" and apply filter for Entity change , on EntityChange calling "showVariablesByEntity()"--------- */

                //var mapVar = $filter('filter')(ctrl.mapVariablesModel.Elements, { dictAttributes: { sfwEntity: ctrl.model.baseChild.dictAttributes.sfwEntity.trim() } }, true);
                //function iterator(value, key) {
                //    this.push(value.dictAttributes.id);
                //}
                //angular.forEach(mapVar, iterator, ctrl.arrMapVariableforEntity);


                /*--------- End--------- */


                // add xml method model
                var xmlmethod;
                if (ctrl.model.baseChild.initializeModel) {
                    xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
                }
                if (xmlmethod && xmlmethod.length <= 0) {
                    var nodeXmlmethod = new objElements();
                    nodeXmlmethod.Name = "XMLMethod";
                    ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
                    ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
                }
                ctrl.isBusinessRuleTypeValid = true;
            }
            else {
                ctrl.model.baseChild.initializeModel.Elements = [];
                ctrl.model.baseChild.ServiceMethodmodel.Elements = [];
                ctrl.model.baseChild.ServiceMethodmodel.dictAttributes = {};
                ctrl.model.baseChild.initializeModel.xmlMethods = [];
                ctrl.model.baseChild.initializeModel.xmlmethodItems = [];
                ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                ctrl.isBusinessRuleTypeValid = false;
            }
        }
        else ctrl.isBusinessRuleTypeValid = false;
    };
    ctrl.RemoveInitializeorFailOverModel = function (param) {
        for (var i = 0; i < ctrl.model.baseChild.Elements.length; i++) {
            if (ctrl.model.baseChild.Elements[i].Name == param) {
                $rootScope.DeleteItem(ctrl.model.baseChild.Elements[i], ctrl.model.baseChild.Elements);

                //ctrl.model.baseChild.Elements.splice(i, 1);
                break;
            }
        }
    };
    ctrl.PrepareInitializeModel = function () {
        var initializeModel = {
            dictAttributes: { ObjectType: "XmlMethod" },
            Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "XMLMethod", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" }],
            Children: [], Name: "initialize", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };
        ctrl.model.baseChild.initializeModel = initializeModel;
        ctrl.loadXmlmethods();
        var xmlmethod = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true);
        if (xmlmethod.length <= 0) {
            var nodeXmlmethod = new objElements();
            nodeXmlmethod.Name = "XMLMethod";
            $rootScope.PushItem(nodeXmlmethod, ctrl.model.baseChild.initializeModel.Elements);

            // ctrl.model.baseChild.initializeModel.Elements.push(nodeXmlmethod);
        }
        ctrl.model.baseChild.initializeModel.xmlmethodModel = $filter('filter')(ctrl.model.baseChild.initializeModel.Elements, { Name: "XMLMethod" }, true)[0];
        ctrl.isBusinessRuleTypeValid = true;
    };
    ctrl.PrepareFailOverServiceBusObjModel = function () {
        ctrl.PrepareInitializeModel();
        var FailOverServiceModel = {
            dictAttributes: { sfwNavigationParameter: "" },
            Elements: [],
            Children: [], Name: "FailOverServiceBusinessObject", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
        };

        $rootScope.PushItem(ctrl.model.baseChild.initializeModel, FailOverServiceModel.Elements);

        //  FailOverServiceModel.Elements.push(ctrl.model.baseChild.initializeModel);
        ctrl.model.baseChild.FailOverServiceModel = FailOverServiceModel;
    };
    ctrl.onVariableChanged = function () {

        if (ctrl.model && ctrl.model.baseChild && ctrl.model.baseChild.dictAttributes) {
            var lOldVariableValue = ctrl.model.baseChild.dictAttributes.variable;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(lOldVariableValue, ctrl.model.baseChild.dictAttributes, "variable", ctrl.model.lstrInitVariable);
            $rootScope.EditPropertyValue(lOldVariableValue, ctrl.model, "lstrInitVariable", ctrl.model.lstrInitVariable);

            if (ctrl.model.baseChild.dictAttributes.variable && ctrl.model.baseChild.dictAttributes.variable.trim().length > 0) {

                $rootScope.EditPropertyValue(ctrl.model.baseChild.dictAttributes.sfwNavigationParameter, ctrl.model.baseChild.dictAttributes, "sfwNavigationParameter", "");

                //ctrl.model.baseChild.dictAttributes.sfwNavigationParameter = "";
                ctrl.RemoveInitializeorFailOverModel("initialize");
                ctrl.PrepareFailOverServiceBusObjModel();

                $rootScope.InsertItem(ctrl.model.baseChild.FailOverServiceModel, ctrl.model.baseChild.Elements, 0);

                //  ctrl.model.baseChild.Elements.splice(0, 0, ctrl.model.baseChild.FailOverServiceModel);
                ctrl.selectedTab = "FailOverServiceBusinessObject";
            } else {
                ctrl.RemoveInitializeorFailOverModel("FailOverServiceBusinessObject");
                ctrl.PrepareInitializeModel();

                $rootScope.InsertItem(ctrl.model.baseChild.initializeModel, ctrl.model.baseChild.Elements, 0);

                //  ctrl.model.baseChild.Elements.splice(0, 0, ctrl.model.baseChild.initializeModel);
                ctrl.selectedTab = "Initialize";
            }
            $rootScope.UndRedoBulkOp("End");

        }
    };
    ctrl.setEnterExitActions = function (isEnter) {
        if ($scope.$parent.showEnterExitDialog) {
            $scope.$parent.showEnterExitDialog(isEnter, isEnter ? ctrl.model.baseChild.entryModel : ctrl.model.baseChild.exitModel, ctrl.model.baseChild.dictAttributes.sfwEntity);
        }
    };
    ctrl.selectMethodClick = function (aEntityID, aNodeName, aMethodID) {
        if (aMethodID && aMethodID != "") {
            $NavigateToFileService.NavigateToFile(aEntityID, aNodeName, aMethodID);
        }
    };
    ctrl.getServiceMethodparameters = function () {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.Elements, ctrl.model.baseChild.ServiceMethodmodel, "Elements", []);
        $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwReturnType, ctrl.model.baseChild.ServiceMethodmodel.dictAttributes, "sfwReturnType", "");
        if (ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwObjectMethod) {
            var servicemethods = $filter('filter')(ctrl.model.baseChild.ServiceMethodmodel.objectMethods, { ID: ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwObjectMethod }, true);
            if (servicemethods && servicemethods.length > 0) {
                var servicemethod = servicemethods[0];
                function iterator(value, key) {
                    var nodeParameters = new objElements();
                    nodeParameters.Name = "parameter";
                    nodeParameters.dictAttributes.sfwParamaterName = value.ID; nodeParameters.dictAttributes.sfwDataType = value.DataType; nodeParameters.dictAttributes.sfwEntity = value.Entity, nodeParameters.dictAttributes.sfwValueSource = "Parameter", nodeParameters.dictAttributes.sfwParameterValue = value.Value;
                    $rootScope.PushItem(nodeParameters, this.Elements);
                }
                angular.forEach(servicemethod.Parameters, iterator, ctrl.model.baseChild.ServiceMethodmodel);
                $rootScope.EditPropertyValue(ctrl.model.baseChild.ServiceMethodmodel.dictAttributes.sfwReturnType, ctrl.model.baseChild.ServiceMethodmodel.dictAttributes, "sfwReturnType", servicemethod.ReturnType);
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };
    ctrl.setServiceMethodParameter = function () {
        var objNewParamScope = $scope.$new();
        objNewParamScope.model = angular.copy(ctrl.model.baseChild.ServiceMethodmodel);
        objNewParamScope.queryParametersValues = ctrl.queryParametersvalues;
        objNewParamScope.IsDisabledforVersioning = ctrl.isDisabledforVersioning;
        objNewParamScope.onValueSourceChanged = function (param) {
            ctrl.onValueSourceChanged({ param: param });
        };
        objNewParamScope.setParamEntityValue = function (item) {
            ctrl.setParamEntityValue({ parentScope: $scope, param: item });
        };
        objNewParamScope.onOKClick = function () {
            while (ctrl.model.baseChild.ServiceMethodmodel.Elements.length > 0) {
                $rootScope.DeleteItem(ctrl.model.baseChild.ServiceMethodmodel.Elements[0], ctrl.model.baseChild.ServiceMethodmodel.Elements, null);
            }
            for (var idx = 0; idx < objNewParamScope.model.Elements.length; idx++) {
                $rootScope.PushItem(objNewParamScope.model.Elements[idx], ctrl.model.baseChild.ServiceMethodmodel.Elements);
            }
            objNewParamScope.closeDialog();
        };
        objNewParamScope.closeDialog = function () {
            if (objNewParamScope.dialog) {
                objNewParamScope.dialog.close();
            }
        };
        objNewParamScope.onRefreshParams = function () {
            var serviceModelObjectMethod = $getEnitityObjectMethods.get(ctrl.model.baseChild.dictAttributes.sfwEntity.trim())
            if (serviceModelObjectMethod.length > 0) {
                var servicemethods = $filter('filter')(serviceModelObjectMethod, { ID: objNewParamScope.model.dictAttributes.sfwObjectMethod }, true);
                if (servicemethods && servicemethods.length > 0) {
                    var servicemethod = servicemethods[0];
                    //preserving old parameters.
                    var existingParameters = objNewParamScope.model.Elements;


                    //clearing current binded list
                    objNewParamScope.model.Elements = [];


                    function iterator(value) {
                        var nodeParameters = new objElements();
                        nodeParameters.Name = "parameter";
                        nodeParameters.dictAttributes.sfwParamaterName = value.ID; nodeParameters.dictAttributes.sfwDataType = value.DataType; nodeParameters.dictAttributes.sfwEntity = value.Entity, nodeParameters.dictAttributes.sfwValueSource = "Parameter", nodeParameters.dictAttributes.sfwParameterValue = value.Value;
                        if (this.Elements.length == 0)
                            $rootScope.PushItem(nodeParameters, this.Elements);
                        else {
                            if (!this.Elements.some(function (param) { return param.dictAttributes.sfwParamaterName == value.ID })) {
                                $rootScope.PushItem(nodeParameters, this.Elements);
                            }
                        }
                    }
                    angular.forEach(servicemethod.Parameters, iterator, objNewParamScope.model);

                    ////Updating current parameter values from old parameters.

                    for (var idx = 0; idx < existingParameters.length; idx++) {
                        if (existingParameters[idx] && existingParameters[idx].dictAttributes && existingParameters[idx].dictAttributes.sfwParamaterName) {
                            var params = objNewParamScope.model.Elements.filter(function (x) { return x.dictAttributes.sfwParamaterName == existingParameters[idx].dictAttributes.sfwParamaterName && x.dictAttributes.sfwDataType == existingParameters[idx].dictAttributes.sfwDataType; });
                            if (params && params.length > 0) {
                                if (existingParameters[idx].dictAttributes.sfwValueSource) {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwValueSource, params[0].dictAttributes, "sfwValueSource", existingParameters[idx].dictAttributes.sfwValueSource);
                                }
                                if (existingParameters[idx].dictAttributes.sfwParameterValue) {
                                    $rootScope.EditPropertyValue(params[0].dictAttributes.sfwParameterValue, params[0].dictAttributes, "sfwParameterValue", existingParameters[idx].dictAttributes.sfwParameterValue);
                                }
                                if (existingParameters[idx].Elements.length > 0 && existingParameters[idx].Elements[0].Name == "value") {
                                    $rootScope.EditPropertyValue(params[0].valueModel, params[0], "valueModel", existingParameters[idx].Elements[0]);
                                    $rootScope.PushItem(params[0].valueModel, params[0].Elements);
                                }
                            }

                        }
                    }

                }
            }
        }
        objNewParamScope.dialog = $rootScope.showDialog(objNewParamScope, "Set Method Parameter", 'BPM/views/Association/SetServiceMethodParameter.html', { height: 400, width: 800 });
    };
    ctrl.setscalarquery = function () {
        var objNewScalarQueryScope = $scope.$new();
        objNewScalarQueryScope.model = angular.copy(ctrl.model.baseChild);
        objNewScalarQueryScope.IsDisabledforVersioning = ctrl.isDisabledforVersioning;
        objNewScalarQueryScope.queryParametersValues = ctrl.queryParametersvalues;
        objNewScalarQueryScope.queryParameterallowedValues = ctrl.queryParameterallowedValues;
        objNewScalarQueryScope.addQuery = function () {
            var nodeQueries = new objElements();
            nodeQueries.Name = "Query";
            objNewScalarQueryScope.model.Elements.push(nodeQueries);
        };
        objNewScalarQueryScope.getQueryParam_dialog = function (queryObj) {
            var dialogScope = objNewScalarQueryScope.$new(true);
            dialogScope.DialogConstants = { queryParamsName: "sfwNavigationParameter" };
            dialogScope.mapObj = {};
            dialogScope.queryValues = objNewScalarQueryScope.queryParameterallowedValues;
            dialogScope.queryParameters = $getQueryparam.getQueryparamfromString(queryObj.dictAttributes, "sfwQueryParameters", ";");
            dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Set Query Parameters", "Common/views/SetQueryParameters.html", { width: 500, height: 500 });
            dialogScope.setQueryParameters = function () {
                var queryParametersDisplay = [];
                function iterator(value, key) {
                    this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                }
                angular.forEach(dialogScope.queryParameters, iterator, queryParametersDisplay);
                queryObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";");
                dialogScope.QueryDialog.close();
            };
        };
        objNewScalarQueryScope.onOKClick = function () {
            $rootScope.UndRedoBulkOp("Start");

            while (ctrl.model.baseChild.Elements.length > 0) {
                $rootScope.DeleteItem(ctrl.model.baseChild.Elements[0], ctrl.model.baseChild.Elements, null);
            }
            for (var idx = 0; idx < objNewScalarQueryScope.model.Elements.length; idx++) {
                $rootScope.PushItem(objNewScalarQueryScope.model.Elements[idx], ctrl.model.baseChild.Elements);
            }
            $rootScope.UndRedoBulkOp("End");

            objNewScalarQueryScope.closeDialog();
        };
        objNewScalarQueryScope.closeDialog = function () {
            if (objNewScalarQueryScope.dialog) {
                objNewScalarQueryScope.dialog.close();
            }
        };
        objNewScalarQueryScope.NavigateToEntityQuery = function (aQueryID) {
            if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                objNewScalarQueryScope.onOKClick();
                var query = aQueryID.split(".");
                $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
            }
        };
        objNewScalarQueryScope.deleteQuery = function () {
            var deleteNodeIndex = objNewScalarQueryScope.model.Elements.indexOf(objNewScalarQueryScope.selectedqueryNode);
            objNewScalarQueryScope.model.Elements.splice(deleteNodeIndex, 1);
            objNewScalarQueryScope.selectedqueryNode = undefined;
        };
        objNewScalarQueryScope.dialog = $rootScope.showDialog(objNewScalarQueryScope, "Set Scalar Queries", 'BPM/views/Association/SetScalarQueries.html', { height: 400, width: 800 });
    };
    //#region show Variables from respective Entity
    ctrl.showVariablesByEntity = function () {
        return function (item) {
            var retValue = false;
            if (ctrl.model.baseChild.dictAttributes.sfwEntity && ctrl.model.baseChild.dictAttributes.sfwEntity.trim().length > 0) {
                retValue = item.dictAttributes.sfwEntity === ctrl.model.baseChild.dictAttributes.sfwEntity;
            }
            return retValue;
        };
    };
    //#endregion
}

app.component('bpmServiceruleShapeAssociationProperties', {
    bindings: {
        shapeModel: '<',
        isDisabledforVersioning: '<?',
        queryParametersvalues: '<',
        queryParameterallowedValues: '<',
        checkAndUpdateParamChildModels: '&',
        onValueSourceChanged: '&',
        mapVariablesModel: "<",
        setParamEntityValue: "&",
        isObjectbased: "<?"
    },
    templateUrl: 'BPM/views/Association/ServiceRuleTaskAssociationProperties.html',
    controller: ["$rootScope", "$scope", "$filter", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", "$getEnitityObjectMethods", "$NavigateToFileService", "$getQueryparam", bpmServiceRuleAssociationPropertiesController]
});

function bpmSequenceFlowAssociationPropertiesController($rootScope, $scope) {
    var ctrl = this;
    ctrl.$onInit = function () {
        function filterextensionElements(x) {
            if (x.Name == "extensionElements" && x.Elements && (Object.prototype.toString.call(x.Elements) === '[object Array]') && x.Elements.length > 0) {
                function filterConditionExpression(y) {
                    if (y.Name == "conditionExpression") {
                        ctrl.gatewayOperator = y.dictAttributes;
                        function filterside(z) {
                            if (z.Name == "leftside") {
                                ctrl.leftSideExpression = z.dictAttributes;
                            }
                            else if (z.Name == "rightside") {
                                ctrl.rightSideExpression = z.dictAttributes;
                            }
                        }
                        y.Elements.forEach(filterside);
                        return true;
                    }
                }
                x.Elements.forEach(filterConditionExpression);
                return true;
            }
        }
        ctrl.model.Elements.some(filterextensionElements);
    };
}

app.component('bpmSequenceflowShapeAssociationProperties', {
    bindings: {
        model: '<',
        isDisabledforVersioning: '<?',
        queryParameterallowedValues: '<',
        operatorsList: '<',
        queryParametersvalues: "<",
        setQueryParam: "&"
    },
    templateUrl: 'BPM/views/Association/GatewayExpressionAssociationProperties.html',
    controller: ["$rootScope", "$scope", bpmSequenceFlowAssociationPropertiesController]
});

function setValueByConstantVariableQueryController($rootScope, $getQueryparam) {
    var ctrl = this;
    ctrl.onTypechange = function () {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryID, ctrl.model, "sfwQueryID", "");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryParameters, ctrl.model, "sfwQueryParameters", "");
        $rootScope.EditPropertyValue(ctrl.model.value, ctrl.model, "value", "");
        $rootScope.UndRedoBulkOp("End");
    };
    ctrl.getParamfromQueryId = function (queryId) {
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue(ctrl.model.value, ctrl.model, "value", "");
        $rootScope.EditPropertyValue(ctrl.model.sfwQueryParameters, ctrl.model, "sfwQueryParameters", $getQueryparam.get(queryId));
        $rootScope.UndRedoBulkOp("End");
    };
}

app.component('setValueByConstantVariableQuery', {
    bindings: {
        model: '<',
        setqueryparam: '&',
        direction: "<",
        isDisabledforVersioning: '<?',
        isLeftSideExpression: "<",
    },
    require: {
        parent: "^bpmSequenceflowShapeAssociationProperties"
    },
    templateUrl: 'BPM/views/Association/setValueByConstantVariableQuery.html',
    controller: ["$rootScope", "$getQueryparam", setValueByConstantVariableQueryController]
});

app.directive("usertaskperformerstableheadertemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<table style='width:100%' >" +
            "<thead>" +
            "<tr class='boldfont grey-bottom-border'>" +
            " <th style='width:10px'><div class='arrow-td'></div></th>" +
            "<th><div>Role</div> </th>" +
            "<th><div>Skill</div> </th>" +
            "<th><div>Location</div></th>" +
            "<th><div>Authority Level</div></th>" +
            "<th><div>Position</div>" +
            "</th><th><div>User</div> </th>" +
            "</tr>" +
            " </thead>" +
            "</table>";
        return template;
    };
    return {
        restrict: 'E',
        replace: true,
        scope: {
        },
        link: function (scope, element, attrs) {
            //$(element).parent().html("<input type='text' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/>");
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
        }
    };
}]);

app.directive("usertaskperformerstabledatatemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            conditionsModel: "=",
            lstbpmnrelatedcodevalues: "=",
            lstuserroles: "=",
            mapvariablesmodel: "=",
            selectedCondition: "=",
            isdisabledforversioning: '<'
        },
        link: function (scope, element, attrs) {
            scope.types = { Role: "role", Skill: "skill", Location: "location", AuthorityLevel: "authoritylevel", Position: "position", User: "user" };
            scope.setEmptyvalueOnChange = function (objcondition, param) {
                if (param == "role") {
                    objcondition.dictAttributes.role = "";
                }
                else if (param == "skill") {
                    objcondition.dictAttributes.skill = "";
                } else if (param == "location") {
                    objcondition.dictAttributes.location = "";
                } else if (param == "position") {
                    objcondition.dictAttributes.position = "";
                }
                else if (param == "authoritylevel") {
                    objcondition.dictAttributes.authorityLevel = "";
                }
                else if (param == "user") {
                    objcondition.dictAttributes.user = "";
                }
            };
            scope.selectCondition = function (condition) {
                scope.selectedCondition = condition;
            };
        },
        templateUrl: 'BPM/views/Performer/userTaskPerformersTableData.html'
    };
}]);

function enterReEnterController($scope, $rootScope, $filter, $EntityIntellisenseFactory, $NavigateToFileService) {
    var scope = this;
    scope.model.SelectActiveFormType = 'Single';
    scope.ActiveFormType = "Lookup,Maintenance,Wizard";
    if (scope.model.dictAttributes.sfwFormName != undefined && scope.model.dictAttributes.sfwFormName != "") {
        if (scope.model.dictAttributes.sfwFormName.contains("=")) {
            scope.model.SelectActiveFormType = 'Multiple';
        }
    }

    scope.onChangeActiveForm = function () {
        $rootScope.UndRedoBulkOp("Start");
        if (scope.model.SelectActiveFormType == 'Single') {
            $rootScope.EditPropertyValue('Multiple', scope.model, "SelectActiveFormType", 'Single');
        } else {
            $rootScope.EditPropertyValue('Single', scope.model, "SelectActiveFormType", 'Multiple');
        }
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwVariableName, scope.model.dictAttributes, "sfwVariableName", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", "");
        //removing parameters
        while (scope.model.Elements.length > 0) {
            $rootScope.DeleteItem(scope.model.Elements[0], scope.model.Elements);
        }
        //removing actions
        while (scope.model.dictAttributes.sfwMode == 'update' && scope.parent.actionsModel && scope.parent.actionsModel.Elements.length > 0) {
            $rootScope.DeleteItem(scope.parent.actionsModel.Elements[0], scope.parent.actionsModel.Elements);
        }

        if (scope.model.dictAttributes.sfwMode == 'update' && scope.parent.extensionElementModel) {
            var PerformersModel = $filter('filter')(scope.parent.extensionElementModel.Elements, { Name: "performers" }, true);
            if (PerformersModel && PerformersModel.length > 0) {
                if (PerformersModel[0].Elements && PerformersModel[0].Elements.length > 0) {
                    if (PerformersModel[0].Elements[0].Name == "preconditions") {
                        for (var i = scope.parent.extensionElementModel.Elements.length - 1; i >= 0; i--) {
                            if (scope.parent.extensionElementModel.Elements[i].Name == "performers") {
                                $rootScope.DeleteItem(scope.parent.extensionElementModel.Elements[i], scope.parent.extensionElementModel.Elements);
                            }
                        }

                        var performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };

                        var preConditionsModel = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: { sfwName: "Always" }, Elements: [],
                                Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                            }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                        };
                        performersModel.Elements.push(preConditionsModel);

                        $rootScope.InsertItem(performersModel, scope.parent.extensionElementModel.Elements, 0);

                    } else if (PerformersModel[0].Elements[0].Name == "defaultcondition") {
                        var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };
                        while (PerformersModel[0].Elements.length > 0) {
                            $rootScope.DeleteItem(PerformersModel[0].Elements[0], PerformersModel[0].Elements);
                        }
                        $rootScope.InsertItem(defaultConditionModel, PerformersModel[0].Elements, 0);
                    }
                }
            }
        }
        scope.setFormType(true);
        $rootScope.UndRedoBulkOp("End");
    };

    scope.lstActiveForm = [];
    scope.setlstActiveForms = function () {
        scope.lstActiveForm = [];
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.contains("=")) {
            var tempActiveForm = scope.model.dictAttributes.sfwFormName.split(";");
            if (tempActiveForm) {
                for (var i = 0; i < tempActiveForm.length; i++) {
                    var tempObj = tempActiveForm[i].split("=");
                    scope.lstActiveForm.push({ FieldValue: tempObj[0], ActiveForm: tempObj[1] });
                }
            }
        }
    };
    scope.setlstActiveForms();


    scope.setVisibleRulesForMultipleForms = function (param) {
        var visibleRuleDialogScope = $scope.$new(true);
        visibleRuleDialogScope.lstActiveForm = scope.lstActiveForm;
        visibleRuleDialogScope.model = scope.model;
        visibleRuleDialogScope.lstVisibleRules = [];
        visibleRuleDialogScope.getlstVisibleRules = function (param) {
            var modelValue = visibleRuleDialogScope.model.dictAttributes[param];
            if (modelValue && modelValue.contains("=")) {
                var tempActiveForm = modelValue.split(";");
                if (tempActiveForm) {
                    for (var i = 0; i < tempActiveForm.length; i++) {
                        var tempObj = tempActiveForm[i].split("=");
                        visibleRuleDialogScope.lstVisibleRules.push({ ActiveForm: tempObj[0], VisibleRule: tempObj[1] });
                    }
                }
            }
        };
        visibleRuleDialogScope.getlstVisibleRules(param);
        //delete ActiveForm visibleRules
        for (var j = visibleRuleDialogScope.lstVisibleRules.length - 1; j >= 0; j--) {
            var isFound = false;
            for (var k = 0; k < visibleRuleDialogScope.lstActiveForm.length; k++) {
                if (visibleRuleDialogScope.lstVisibleRules[j].ActiveForm == visibleRuleDialogScope.lstActiveForm[k].ActiveForm) {
                    isFound = true;
                    break;
                }
            }
            if (!isFound) {
                visibleRuleDialogScope.lstVisibleRules.splice(j, 1);
            }
        }

        //Add newly added forms
        for (var j = 0; j < visibleRuleDialogScope.lstActiveForm.length; j++) {
            var isFound = false;
            for (var k = 0; k < visibleRuleDialogScope.lstVisibleRules.length; k++) {
                if (visibleRuleDialogScope.lstVisibleRules[k].ActiveForm == visibleRuleDialogScope.lstActiveForm[j].ActiveForm) {
                    isFound = true;
                    break;
                }
            }

            if (!isFound) {
                var objVisbileRule = {};
                objVisbileRule.ActiveForm = visibleRuleDialogScope.lstActiveForm[j].ActiveForm;
                objVisbileRule.VisibleRule = "";
                visibleRuleDialogScope.lstVisibleRules.push(objVisbileRule);
            }
        }
        visibleRuleDialogScope.selectActiveFormRow = function (form) {
            visibleRuleDialogScope.selectedActiveForm = form;
        };
        visibleRuleDialogScope.OkClick = function () {
            var activeFormString = "";
            var oldFormValue = scope.model.dictAttributes[param];
            if (visibleRuleDialogScope.lstVisibleRules.length > 0) {
                for (var i = 0; i < visibleRuleDialogScope.lstVisibleRules.length; i++) {
                    if (visibleRuleDialogScope.lstVisibleRules[i].ActiveForm != "" && visibleRuleDialogScope.lstVisibleRules[i].VisibleRule != "") {
                        if (activeFormString == "") {
                            activeFormString = visibleRuleDialogScope.lstVisibleRules[i].ActiveForm + "=" + visibleRuleDialogScope.lstVisibleRules[i].VisibleRule;
                        }
                        else {
                            activeFormString += ";" + visibleRuleDialogScope.lstVisibleRules[i].ActiveForm + "=" + visibleRuleDialogScope.lstVisibleRules[i].VisibleRule;
                        }
                    }
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes[param], scope.model.dictAttributes, param, activeFormString);
            }
            else {
                $rootScope.EditPropertyValue(scope.model.dictAttributes[param], scope.model.dictAttributes, param, "");
            }

            visibleRuleDialogScope.CancelClick();
        };
        visibleRuleDialogScope.CancelClick = function () {
            visibleRuleDialogScope.VisibleRuleFormDialog.close();
        };

        visibleRuleDialogScope.onVisibleRuleChange = function (form, event) {
            var input;
            if ((event.keyCode && event.keyCode != 13 && event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") || event.type == "click") {
                if (event.type && event.type == "click") input = $(event.target).prevAll("input[type='text']");
                else input = $(event.target);
                if (form && !form.VisibleRules) {
                    if (form.ActiveForm && form.ActiveForm.contains("Maintenance")) {
                        $.connection.hubBPMN.server.getFormModel(form.ActiveForm).done(function (data) {
                            if (form && data) {
                                form.VisibleRules = data.VisibleRules;
                                setSingleLevelAutoComplete(input, form.VisibleRules, visibleRuleDialogScope, "Code", "Description");
                            }
                        });
                    }
                }
                if (form.VisibleRules) {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    } else {
                        setSingleLevelAutoComplete(input, form.VisibleRules, visibleRuleDialogScope, "Code", "Description");
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                    }
                    if (event.type && event.type == "click") {
                        input.focus();
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                    }
                }
            }
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                event.preventDefault();
            }
        };
        visibleRuleDialogScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        visibleRuleDialogScope.VisibleRuleFormDialog = $rootScope.showDialog(visibleRuleDialogScope, "Set Visible Rules", "BPM/views/Association/BpmVisibleRulesForMultipleActiveForms.html", { width: 800, height: 500 });
    };

    scope.onAddMultipleActiveFormClick = function (event) {
        var newScope = $scope.$new(true);
        newScope.model = scope.model;
        scope.setlstActiveForms();
        newScope.ActiveFormType = scope.ActiveFormType;
        newScope.lstActiveForm = scope.lstActiveForm;
        newScope.AddActiveForm = function () {
            newScope.lstActiveForm.push({ FieldValue: "", ActiveForm: "" });
        };

        newScope.DeleteActiveForm = function () {
            if (newScope.selectedActiveForm && newScope.selectedActiveForm != "") {
                var index = newScope.lstActiveForm.indexOf(newScope.selectedActiveForm);
                if (index > -1) {
                    newScope.lstActiveForm.splice(index, 1);
                }
                else {
                    newScope.selectedActiveForm = undefined;
                }
            }
        };
        newScope.selectActiveFormRow = function (form) {
            newScope.selectedActiveForm = form;
        };
        newScope.validateActiveForms = function () {
            var IsValid = false;
            newScope.ErrorMessageForDisplay = "";
            if (newScope.lstActiveForm.length == 0) {
                newScope.ErrorMessageForDisplay = "Atleast one active form has to be added.";
                return true;
            }
            else if (newScope.lstActiveForm.length > 0 && newScope.lstActiveForm.some(function (x) { return x.FieldValue === "" })) {
                newScope.ErrorMessageForDisplay = "Field Value can not be null.";
                return true;
            }

            else if (newScope.lstActiveForm.length > 0 && newScope.lstActiveForm.some(function (x) { return x.ActiveForm === "" })) {
                newScope.ErrorMessageForDisplay = "Active Form can not be null.";
                return true;
            }
            return IsValid;
        };
        newScope.OkClick = function () {
            var activeFormString = "";
            var oldFormValue = scope.model.dictAttributes.sfwFormName;
            if (newScope.lstActiveForm.length > 0) {
                for (var i = 0; i < newScope.lstActiveForm.length; i++) {
                    if (newScope.lstActiveForm[i].FieldValue != "" && newScope.lstActiveForm[i].ActiveForm != "") {
                        if (activeFormString == "") {
                            activeFormString = newScope.lstActiveForm[i].FieldValue + "=" + newScope.lstActiveForm[i].ActiveForm;
                        }
                        else {
                            activeFormString += ";" + newScope.lstActiveForm[i].FieldValue + "=" + newScope.lstActiveForm[i].ActiveForm;
                        }
                    }
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", activeFormString);
            }
            else {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFormName, scope.model.dictAttributes, "sfwFormName", "");
            }
            if (oldFormValue != activeFormString) {
                scope.onFormNameChanged();
            }
            newScope.CancelClick();
        };
        newScope.CancelClick = function () {
            newScope.ActiveFormDialog.close();
        };
        newScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        newScope.ActiveFormDialog = $rootScope.showDialog(newScope, "Active Forms", "BPM/views/BpmActiveForms.html", { width: 800, height: 500 });
    };



    scope.$onInit = function () {
        scope.refreshParam = false;
        scope.formData = { entityID: "", formTitle: "", controlTree: [], initialLoadModel: {}, visibleRules: [] };
        scope.setNewActiveFormData();
        if (scope.parent.currentUserTaskData.formList.length == 0) {
            $.connection.hubMain.server.getFilesByType("Maintenance,Lookup,Wizard", "ScopeId_" + $scope.$id, null);
            $scope.receiveList = function (data) {
                scope.parent.currentUserTaskData.formList = data;
            };
        }

        scope.setFormType();
    };

    scope.setNewActiveFormData = function () {
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            if (scope.model.dictAttributes.sfwFormName.contains("=") && scope.lstActiveForm.length > 0) {
                $.connection.hubBPMN.server.getFormModel(scope.lstActiveForm[0].ActiveForm).done(receiveFormModel);
            }
            else {
                $.connection.hubBPMN.server.getFormModel(scope.model.dictAttributes.sfwFormName).done(receiveFormModel);
            }
        }
    };
    var receiveFormModel = function (data) {
        if (data) {
            var updateValues = function () { updateCurrentUserTaskData(scope, data); };
            $scope.$evalAsync(updateValues);
            if (scope.refreshParam) {
                $scope.$evalAsync(function () {
                    $scope.initParam();
                });
            }
        }
    };

    scope.onFormNameChanged = function () {

        $rootScope.UndRedoBulkOp("Start");
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            scope.addModel();
            scope.resetModel();
            var FormName = scope.model.dictAttributes.sfwFormName;
            if (FormName.contains('=') && scope.lstActiveForm.length > 0) {
                if (scope.lstActiveForm[0].ActiveForm) {
                    FormName = scope.lstActiveForm[0].ActiveForm;
                }
            }
            var isValid = scope.isValidFile(FormName);
            if (isValid) {
                $.connection.hubBPMN.server.getFormModel(FormName).done(receiveFormModel);
            }
            else {
                scope.removeModel();
                scope.resetModel();
            }
        }
        else {
            scope.removeModel();
            scope.resetModel();
        }
        $rootScope.UndRedoBulkOp("End");
    };
    scope.addModel = function () {
        var index = scope.extelementmodel.Elements.indexOf(scope.model);
        if (index == -1) {
            $rootScope.PushItem(scope.model, scope.extelementmodel.Elements);
        }
    };
    scope.removeModel = function () {
        var index = scope.extelementmodel.Elements.indexOf(scope.model);
        if (index > -1) {
            $rootScope.DeleteItem(scope.model, scope.extelementmodel.Elements);
        }
    };
    scope.resetModel = function () {
        $rootScope.EditPropertyValue(scope.model.dictAttributes.FormTitle, scope.model.dictAttributes, "FormTitle", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmSubmitButtonVisibleRule, scope.model.dictAttributes, "sfwBpmSubmitButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmApproveButtonVisibleRule, scope.model.dictAttributes, "sfwBpmApproveButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBpmRejectButtonVisibleRule, scope.model.dictAttributes, "sfwBpmRejectButtonVisibleRule", "");
        $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);
        scope.setFormType(true);
        scope.formData.initialLoadModel = null;
        scope.formData.entityID = null;
        scope.formData.remoteObject = null;
        scope.formData.formTitle = null;
        scope.formData.controlTree = null;
        scope.formData.visibleRules = null;
        scope.formData.lookupParameters = null;
    };
    scope.setFormType = function (fromUndoRedoBlock) {
        var formName = "";
        if (scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.contains("=") && scope.lstActiveForm.length > 0) {
            formName = scope.lstActiveForm[0].ActiveForm;

        } else {
            formName = scope.model.dictAttributes.sfwFormName;
        }
        if (formName && formName.trim().length > 0) {
            if (fromUndoRedoBlock) {
                if (formName.indexOf("Lookup") == formName.length - 6) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "lookup");
                }
                else if (formName.indexOf("Maintenance") == formName.length - 11) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "maintenance");
                }
                else if (formName.indexOf("Wizard") == formName.length - 6) {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "wizard");
                }
                else {
                    $rootScope.EditPropertyValue(scope.formType, scope, "formType", "");
                }
            }
            else {
                if (formName.indexOf("Lookup") == formName.length - 6) {
                    scope.formType = "lookup";
                }
                else if (formName.indexOf("Maintenance") == formName.length - 11) {
                    scope.formType = "maintenance";
                }
                else if (formName.indexOf("Wizard") == formName.length - 6) {
                    scope.formType = "wizard";
                }
                else {
                    scope.formType = "";
                }
            }
        } else {
            $rootScope.EditPropertyValue(scope.formType, scope, "formType", "");
        }
    };
    scope.onFormNameKeyDown = function (event) {
        var input = $(event.target);
        //if (input.val() == undefined || input.val().trim() == "") {
        var data = scope.parent.currentUserTaskData.formList;
        setSingleLevelAutoComplete(input, data);
        //}
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        }
    };
    scope.showFormNameIntellisenseList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        input.focus();
        // if (input.val() == undefined || input.val().trim() == "") {
        var data = scope.parent.currentUserTaskData.formList;
        setSingleLevelAutoComplete(input, data);
        //}
        if ($(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
        }
    };

    scope.setNavigationParameters = function () {
        var paramDialogScope = $scope.$new();
        var bpmScope = getCurrentFileScope();
        paramDialogScope.lstGlobalParameters = bpmScope.lstGlobalParameters;
        if (bpmScope) {
            paramDialogScope.variables = bpmScope.mapVariablesModel.Elements;
        }
       
        paramDialogScope.formType = scope.formType;
        $scope.initParam = function () {
            paramDialogScope.parameters = scope.getParameters();
            if (scope.formType == "lookup") {
                paramDialogScope.criteriaFields = scope.formData.lookupParameters;
                paramDialogScope.selectCriteriaField = function (field) {
                    paramDialogScope.selectedCriteriaField = field;
                };
                paramDialogScope.deleteParameter = function () {
                    var index = paramDialogScope.parameters.indexOf(paramDialogScope.selectedParameter);
                    if (index > -1) {
                        paramDialogScope.parameters.splice(index, 1);
                    }
                };
                paramDialogScope.canDeleteParameter = function () {
                    return paramDialogScope.selectedParameter ? true : false;
                };

            } else {
                if (!paramDialogScope.parameters || paramDialogScope.parameters.length == 0) {
                    paramDialogScope.parameters = [];
                    var param = {};
                    param.fieldName = "";
                    if (scope.model.dictAttributes.sfwFormMode != "new") {
                        param.name = "aintPrimaryKey";
                    } else {
                        param.name = "ParentKey";
                    }
                    param.source = "";
                    param.value = "";
                    paramDialogScope.parameters.push(param);

                    if (scope.model.dictAttributes.sfwFormMode == "new") {
                        var param = {};
                        param.fieldName = "";
                        param.name = "ParentKeyName";
                        param.source = "";
                        param.value = "";
                        paramDialogScope.parameters.push(param);
                    }
                }
                if (paramDialogScope.parameters) {
                    for (var i = scope.model.Elements.length - 1; i > -1; i--) {
                        var isValidparameter = false;
                        if (scope.model.Elements[i].Name == "parameter") {
                            for (var j = 0; j < paramDialogScope.parameters.length; j++) {
                                if (scope.model.Elements[i].dictAttributes.sfwParamaterName == paramDialogScope.parameters[j].name) {
                                    isValidparameter = true;
                                    paramDialogScope.parameters[j].value = scope.model.Elements[i].dictAttributes.sfwParameterValue;
                                    paramDialogScope.parameters[j].source = scope.model.Elements[i].dictAttributes.sfwValueSource;
                                    break;
                                }
                            }
                        }
                        if (!isValidparameter) {
                            scope.model.Elements.splice(i, 1);
                        }
                    }
                }

            }
        };
        paramDialogScope.selectParameter = function (param) {
            paramDialogScope.selectedParameter = param;
        };
        paramDialogScope.onSaveClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);
            function iterator(param) {
                $rootScope.PushItem({ prefix: "", Name: "parameter", dictAttributes: { sfwParamaterName: param.name, sfwValueSource: param.source, sfwParameterValue: param.value, sfwFieldName: param.fieldName, sfwDataType: param.dataType }, Elements: [], Children: [] }, scope.model.Elements);
            }
            angular.forEach(paramDialogScope.parameters, iterator);
            paramDialogScope.onCancelClick();
            $rootScope.UndRedoBulkOp("End");
        };
        paramDialogScope.onCancelClick = function () {
            if (paramDialogScope.dialog)
                paramDialogScope.dialog.close();
        };
        paramDialogScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
        paramDialogScope.setActiveFormData = function () {
            scope.setNewActiveFormData();
            scope.refreshParam = true;
        };
        $scope.initParam();
        paramDialogScope.dialog = $rootScope.showDialog(paramDialogScope, "Set Navigation Parameters", "BPM/views/Association/UserTaskSetNavigationParametersTemplate.html", { height: 450, width: 1100 });
    };
    scope.getParameters = function () {
        if (scope.formType == "lookup") {
            var params = scope.model.Elements.map(function (param) {
                return {
                    name: param.dictAttributes.sfwParamaterName,
                    source: param.dictAttributes.sfwValueSource,
                    value: param.dictAttributes.sfwParameterValue,
                    dataType: param.dictAttributes.sfwDataType,
                    fieldName: param.dictAttributes.sfwFieldName
                };
            });
            return params;
        }
        else {
            var initialLoadModel = scope.formData.initialLoadModel;
            var entityID = scope.formData.entityID;
            var remoteObject = scope.formData.remoteObject;

            if (initialLoadModel) {
                var selectedMethod;
                var mode = scope.isenter && (scope.model.dictAttributes.sfwFormMode == undefined || scope.model.dictAttributes.sfwFormMode == "new") ? "New" : "Update";
                var callMethods = undefined;
                if (initialLoadModel.Elements && initialLoadModel.Elements.length > 0) {
                    callMethods = initialLoadModel.Elements.filter(function (item) { return item.Name == "callmethods" && ((!item.dictAttributes.hasOwnProperty("sfwMode")) || item.dictAttributes.sfwMode == mode); });
                }
                if (callMethods && callMethods.length > 0) {
                    selectedMethod = callMethods[0].dictAttributes.sfwMethodName;
                }
                if (selectedMethod) {
                    var params;
                    if (remoteObject) {
                        var methods = $filter('filter')(remoteObject.Elements, { dictAttributes: { ID: selectedMethod } }, true);
                        if (methods && methods.length > 0) {
                            params = methods[0].Elements.map(function (param) { return { name: param.dictAttributes.ID, source: "", value: "", fieldName: param.ID }; });
                        }
                    }
                    else {
                        var lparams = $EntityIntellisenseFactory.getXmlMethodParameters(entityID, selectedMethod, true);
                        if (lparams && lparams.length > 0) {
                            params = lparams.map(function (param) { return { name: param.ID, source: "", value: "", fieldName: param.ID }; });
                        }
                    }

                    if (params && params.length > 0) {
                        function iterator(param) {
                            var existingParams = $filter('filter')(scope.model.Elements, { dictAttributes: { sfwParamaterName: param.name } }, true);
                            if (existingParams && existingParams.length > 0) {
                                param.source = existingParams[0].dictAttributes.sfwValueSource;
                                param.value = existingParams[0].dictAttributes.sfwParameterValue;
                            }
                        }
                        angular.forEach(params, iterator);
                        return params;
                    }
                } else {
                    if (scope.model.Elements.length > 0) {
                        var Params = scope.model.Elements.map(function (param) { return { name: param.dictAttributes.sfwParamaterName, source: param.dictAttributes.sfwValueSource, value: param.dictAttributes.sfwParameterValue, fieldName: param.dictAttributes.sfwFieldName }; });
                        return Params;
                    }
                }
            }
        }
    };
    scope.setFocusControlID = function () {
        focusControlDialogScope = $scope.$new();
        focusControlDialogScope.controlTree = scope.formData.controlTree;
        focusControlDialogScope.onSaveClick = function () {
            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFocusControlID, scope.model.dictAttributes, "sfwFocusControlID", focusControlDialogScope.selectedControlID);
            focusControlDialogScope.onCancelClick();
        };
        focusControlDialogScope.onCancelClick = function () {
            if (focusControlDialogScope.dialog)
                focusControlDialogScope.dialog.close();
        };
        focusControlDialogScope.selectControlID = function (element, event) {
            focusControlDialogScope.selectedControlID = element.ID;
        };
        focusControlDialogScope.dialog = $rootScope.showDialog(focusControlDialogScope, "", "BPM/views/SelectFocusControlTemplate.html");
    };
    scope.onVisibleRuleKeyDown = function (event) {
        var visibleRules = [];
        var input = $(event.target);
        if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
            visibleRules = scope.formData.visibleRules;
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                $(input).autocomplete("search", $(input).val());
                event.preventDefault();
            } else {
                setSingleLevelAutoComplete($(event.target), visibleRules, scope, "Code", "Description");
            }
        }

        //if (event) {
        //    event.preventDefault();
        //}
    };
    scope.showVisibleRuleIntellisenseList = function (event) {
        var input = $(event.target).prevAll("input[type='text']");
        var visibleRules = [];
        visibleRules = scope.formData.visibleRules;
        input.focus();
        setSingleLevelAutoComplete(input, visibleRules, scope, "Code", "CodeDescription");
        if ($(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
        }
    };
    scope.NavigateToForm = function (aFormID) {
        if (aFormID && aFormID != "") {
            if (aFormID.contains("=") && scope.lstActiveForm.length > 0) {
                if (scope.lstActiveForm[0].ActiveForm) {
                    aFormID = scope.lstActiveForm[0].ActiveForm;
                }
            }
            hubMain.server.navigateToFile(aFormID, "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };
    scope.NavigateToFormControl = function (aControlID, aFormID) {
        if (aFormID.contains("=") && scope.lstActiveForm.length > 0) {
            if (scope.lstActiveForm[0].ActiveForm) {
                aFormID = scope.lstActiveForm[0].ActiveForm;
            }
        }
        if (aControlID && aFormID && aFormID != "" && aControlID != "") {
            $NavigateToFileService.NavigateToFile(aFormID, "", aControlID);
        }
    };
    scope.NavigateToVisibleRule = function (aVisibleRuleID) {
        var entity = scope.formData.entityID;
        if (aVisibleRuleID && aVisibleRuleID != "") {
            $NavigateToFileService.NavigateToFile(entity, "initialload", aVisibleRuleID);
        }
    };
    scope.setTaskActions = function () {
        if (scope.model && scope.model.dictAttributes.sfwFormName && scope.model.dictAttributes.sfwFormName.trim().length > 0) {
            var ActiveFormName = scope.model.dictAttributes.sfwFormName;
            if (scope.model.dictAttributes.sfwFormName.contains('=')) {
                scope.setlstActiveForms();
                if (scope.lstActiveForm.length > 0 && scope.lstActiveForm[0].ActiveForm) {
                    ActiveFormName = scope.lstActiveForm[0].ActiveForm;
                }
            }
            if (scope.isValidFile(ActiveFormName)) {
                taskActionsScope = $scope.$new();
                taskActionsScope.ActiveFormType = scope.model.SelectActiveFormType;
                taskActionsScope.lstActiveForm = scope.lstActiveForm;
                if (scope.parent.actionsModel) {
                    taskActionsScope.actionsModel = JSON.parse(JSON.stringify(scope.parent.actionsModel));
                    var actions = taskActionsScope.actionsModel.Elements.filter(function (x) { return x.Elements.length > 0; });
                    function iterator(action) {
                        action.selectedVariable = action.Elements[0];
                        action.selectedVariableID = action.selectedVariable.dictAttributes.ID;
                    }
                    angular.forEach(actions, iterator);
                }
                taskActionsScope.IsDisabledforVersioning = scope.IsDisabledforVersioning;
                taskActionsScope.isObjectbased = scope.isObjectbased;
                var success = function (objectList) {
                    var updateAssociatedFormData = function () {
                        taskActionsScope.entities = objectList[0];
                        taskActionsScope.buttons = objectList[1];
                    };
                    taskActionsScope.$evalAsync(updateAssociatedFormData);
                };
                taskActionsScope.variables = scope.parent.queryParametersValues;
                $.connection.hubBPMN.server.getAssociatedFormData(ActiveFormName).done(success);
                taskActionsScope.onEntityChanged = function (action) {
                    if (action && action.selectedVariable) {
                        action.selectedVariable.dictAttributes.sfwEntityField = "";
                    }
                };
                taskActionsScope.onProcessVariableChanged = function (action) {
                    if (action) {

                        if (action.Elements.length > 0) {
                            action.Elements.splice(0, 1);
                            // $rootScope.DeleteItem(action.Elements[0], action.Elements, null);
                        }
                        if (action.dictAttributes.sfwButtonID && action.dictAttributes.sfwButtonID.trim().length > 0) {

                            action.selectedVariable = {
                                prefix: "sbpmn",
                                Name: "variable",
                                Value: "",
                                Elements: [],
                                Children: [],
                                dictAttributes: { ID: action.selectedVariableID, sfwEntity: action.dictAttributes.sfwEntity }
                            };
                            action.Elements.push(action.selectedVariable);
                        }

                    }
                };
                taskActionsScope.onIsConstantChanged = function (action) {
                    if (action) {
                        action.dictAttributes.sfwEntity = "";
                        if (action.selectedVariable) {
                            action.selectedVariable.dictAttributes.sfwEntity = "";
                            action.selectedVariable.dictAttributes.sfwEntityField = "";
                        }
                    }
                };

                taskActionsScope.getButtonsAndEntitiesFromActiveForm = function (action) {
                    if (action && action.dictAttributes && action.dictAttributes.sfwActiveForm) {
                        $.connection.hubBPMN.server.getAssociatedFormData(action.dictAttributes.sfwActiveForm).done(function (result) {
                            taskActionsScope.$evalAsync(function () {
                                action.entities = result[0];
                                action.buttons = result[1];
                            });
                        });
                    } else {
                        if (action) {
                            action.entities = [];
                            action.buttons = [];
                        }
                    }
                };

                taskActionsScope.selectAction = function (action, isInUndoRedoBlock) {
                    if (isInUndoRedoBlock) {
                        var selectedAction = taskActionsScope.actionsModel.Elements.filter(function (x) { return x.isSelected; });
                        if (selectedAction && selectedAction.length > 0) {
                            selectedAction[0].isSelected = false;
                        }
                        action.isSelected = true;
                    }
                    else {
                        function iterator(act) {
                            act.isSelected = act == action;
                        }
                        angular.forEach(taskActionsScope.actionsModel.Elements, iterator);
                    }
                };
                taskActionsScope.addAction = function () {
                    var newItem = { prefix: "sbpmn", Name: "action", dictAttributes: {}, Elements: [], Value: "", Children: [] };
                    taskActionsScope.actionsModel.Elements.push(newItem);
                    taskActionsScope.selectAction(newItem, true);
                };
                taskActionsScope.canDeleteAction = function () {
                    if (taskActionsScope.actionsModel != undefined && taskActionsScope.actionsModel.Elements != undefined) {
                        return taskActionsScope.actionsModel.Elements.some(function (x) {
                            return x.isSelected;
                        });
                    }
                };
                taskActionsScope.deleteAction = function () {
                    var selectedActions = taskActionsScope.actionsModel.Elements.filter(function (x) {
                        return x.isSelected;
                    });
                    if (selectedActions && selectedActions.length > 0) {
                        var index = taskActionsScope.actionsModel.Elements.indexOf(selectedActions[0]);
                        if (index > -1) {

                            //Get and select the next action, before removing the desired item.
                            var nextActionToBeSelected = index + 1 == taskActionsScope.actionsModel.Elements.length ? taskActionsScope.actionsModel.Elements[index - 1] : taskActionsScope.actionsModel.Elements[index + 1];
                            if (nextActionToBeSelected) {
                                taskActionsScope.selectAction(nextActionToBeSelected, true);
                            }
                            taskActionsScope.actionsModel.Elements.splice(index, 1);
                        }
                    }

                };
                taskActionsScope.onOKClick = function () {
                    $rootScope.UndRedoBulkOp("Start");
                    while (scope.parent.actionsModel.Elements.length > 0) {
                        $rootScope.DeleteItem(scope.parent.actionsModel.Elements[0], scope.parent.actionsModel.Elements, null);
                    }

                    for (var idx = 0; idx < taskActionsScope.actionsModel.Elements.length; idx++) {
                        $rootScope.PushItem(taskActionsScope.actionsModel.Elements[idx], scope.parent.actionsModel.Elements);
                    }
                    $rootScope.UndRedoBulkOp("End");
                    taskActionsScope.closeDialog();
                };
                taskActionsScope.closeDialog = function () {
                    if (taskActionsScope.dialog)
                        taskActionsScope.dialog.close();
                };
                taskActionsScope.dialog = $rootScope.showDialog(taskActionsScope, "Task Actions", "BPM/views/Association/SetUserTaskActions.html", { width: 1100 });

                //#region While Changing Active form Entity id and field should get clear
                taskActionsScope.OnChangeOfActiveForms = function (action) {
                    if (action) {
                        action.dictAttributes.sfwButtonID = "";
                        if (action.selectedVariable && action.selectedVariable.dictAttributes) {
                            action.selectedVariable.dictAttributes.sfwEntity = "";
                            action.selectedVariable.dictAttributes.sfwEntityField = "";
                            action.selectedVariableID = "";
                        }
                    }
                };
                //#endregion


            }
        }
    };
    scope.isValidFile = function (strfilename) {
        var isvalid = false;
        for (var i = 0; i < scope.parent.currentUserTaskData.formList.length; i++) {
            if (scope.parent.currentUserTaskData.formList[i] && strfilename) {
                if (scope.parent.currentUserTaskData.formList[i].toLowerCase() == strfilename.toLowerCase().trim()) {

                    isvalid = true;
                    break;
                }
            }
        }
        return isvalid;
    };
}
app.component("bpmEnterReEnterFormDetails", {
    require: { parent: '^bpmUserTaskAssociationProperties' },
    bindings: {
        model: "<",
        approvalactivity: "<",
        isenter: "<",
        extelementmodel: "<",
        IsDisabledforVersioning: "<isDisabledforVersioning",
        isObjectbased: "<?"
    },
    controller: ["$scope", "$rootScope", "$filter", "$EntityIntellisenseFactory", "$NavigateToFileService", enterReEnterController],
    templateUrl: "BPM/views/Association/EnterReEnterFormDetailsTemplate.html"
});


// usertask directives for performers tab

app.directive("userrolesintellisensetemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<div class='input-group full-width'><input type='text' ng-disabled='isdisabledforversioning' undoredodirective  model='model' propertyname='propertyname' class='form-control input-sm' title='{{rolename}}' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/><span ng-if='!isdisabledforversioning' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div>";
        return template;
    };
    return {
        restrict: 'E',
        replace: true,
        scope: {
            lstuserroles: "=",
            rolename: "=",
            isdisabledforversioning: '<',
            model: "=",
            propertyname: "="

        },
        link: function (scope, element, attrs) {
            //$(element).parent().html("<input type='text' ng-model='rolename' ng-keydown='UserRoleTextChanged($event)'/>");
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.intellienseList = [];
            scope.intellienseList = scope.lstuserroles;
            SetCodeIDDescriptionToList(scope.intellienseList);
            scope.UserRoleTextChanged = function (event) {
                var input = $(event.target);
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                } else {
                    setSingleLevelAutoComplete(input, scope.intellienseList, scope, "Description", "CodeIDDescription");
                }
            };
            scope.showIntellisenseList = function (event) {
                var input = $(event.target).prevAll("input[type='text']");
                setSingleLevelAutoComplete(input, scope.lstuserroles, scope, "Description", "CodeIDDescription");
                if ($(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val().trim());
                }
                input.focus();
                if (event) {
                    event.stopPropagation();
                }
            };
        }
    };
}]);


app.directive("bpmncodevaluesbasedontypeintellisense", ['$compile', '$rootScope', function ($compile, $rootScope) {
    var getTemplate = function () {
        var template = "<div class='input-group full-width'>" + "<input class='form-control input-sm' type='text' title='{{objconditionmodel.dictAttributes[type]}}' ng-model='objconditionmodel.dictAttributes[type]' ng-show='getBoolValue(false)' ng-keydown='TextChanged($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-show='getBoolValue(false)'></span>" +
            '<select class="form-control input-sm" ng-options="variable.dictAttributes.id as variable.dictAttributes.id for variable in mapvariablesmodel.Elements"  ng-show="getBoolValue(true)" title="{{objconditionmodel.dictAttributes[type]}}" ng-model="objconditionmodel.dictAttributes[type]" ></select>'
            + "</div>";
        return template;
    };

    var setModelValue = function (lst, value) {
        var modelvalue = "";
        if (value != undefined) {
            data = lst.filter(function (x) {
                return x.CodeID.match(value);
            });
            if (data.length > 0) {
                modelvalue = data[0].CodeIDDescription;
            }
        }
        return modelvalue;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            lstbpmnrelatedcodevalues: "=",
            type: "=",
            lstuserroles: "=",
            mapvariablesmodel: "=",
            objconditionmodel: "="
        },
        link: function (scope, element, attrs) {
            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.lst = [];
            if (scope.type == "role") {
                scope.lst = scope.lstuserroles;

                if (scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable.toLowerCase() == "true") {
                    scope.modelvalue = scope.objconditionmodel.dictAttributes.role;
                }
                else {
                    scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.role);
                }
            }
            else {
                if (scope.type == "skill") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "SkillList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.skill;
                            }
                            else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.skill);
                            }
                        }
                    });
                }
                else if (scope.type == "location") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "LocationList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.location;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.location);
                            }
                        }
                    });
                } else if (scope.type == "position") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "PositionList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.position;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.position);
                            }
                        }
                    });
                } else if (scope.type == "authorityLevel") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "AuthorityLevelList") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.authorityLevel;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.authorityLevel);
                            }
                        }
                    });
                } else if (scope.type == "user") {
                    angular.forEach(scope.lstbpmnrelatedcodevalues, function (value, key) {
                        if (value.Name == "UserIds") {
                            scope.lst = value.lstCodeID;
                            if (scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable != undefined && scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable.toLowerCase() == "true") {
                                scope.modelvalue = scope.objconditionmodel.dictAttributes.user;
                            } else {
                                scope.modelvalue = setModelValue(scope.lst, scope.objconditionmodel.dictAttributes.user);
                            }
                        }
                    });
                }
            }
            SetCodeIDDescriptionToList(scope.lst);
            scope.getBoolValue = function (isMapVariable) {
                var value;
                if (scope.type == "role") {
                    value = scope.objconditionmodel.dictAttributes.isRoleBasedOnMapVariable;
                }
                else if (scope.type == "skill") {
                    value = scope.objconditionmodel.dictAttributes.isSkillBasedOnMapVariable;
                }
                else if (scope.type == "location") {
                    value = scope.objconditionmodel.dictAttributes.isLocationBasedOnMapVariable;
                } else if (scope.type == "position") {
                    value = scope.objconditionmodel.dictAttributes.isPositionBasedOnMapVariable;
                } else if (scope.type == "authorityLevel") {
                    value = scope.objconditionmodel.dictAttributes.isAuthorityLevelBasedOnMapVariable;
                } else if (scope.type == "user") {
                    value = scope.objconditionmodel.dictAttributes.isUserBasedOnMapVariable;
                }
                if (value != undefined) {
                    if (value.toLowerCase() == "false" && !isMapVariable) {
                        return true;
                    } else if (value.toLowerCase() == "true" && isMapVariable) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (!isMapVariable) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            };

            scope.TextChanged = function (event) {
                var input = $(event.target);
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                    else {
                        var propertyName = "CodeID";
                        if (scope.type == "role") {
                            propertyName = "Description";
                        }
                        setSingleLevelAutoCompleteForCodeValues(input, scope.lst, scope, propertyName, "CodeIDDescription", scope.objconditionmodel, scope.type);
                    }
                }
            };

            scope.showIntellisenseList = function (event) {
                var input = $(event.target).prevAll("input[type='text']");
                setSingleLevelAutoCompleteForCodeValues(input, scope.lst, scope, "CodeID", "CodeIDDescription", scope.objconditionmodel, scope.type);
                input.focus();
                if ($(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val().trim());
                }
            };
        }
    };
}]);


function updateCurrentUserTaskData(scope, formDetails) {
    scope.formData.entityID = formDetails.EntityID;
    scope.model.dictAttributes.FormTitle = formDetails.FormTitle;
    scope.formData.controlTree = formDetails.ControlTree;
    scope.formData.initialLoadModel = formDetails.InitialLoadModel;
    scope.formData.visibleRules = formDetails.VisibleRules;
    scope.formData.lookupParameters = formDetails.LookupParameters;
    scope.formData.remoteObject = formDetails.RemoteObject;
}

function bpmLanePerformersPropertiesController($filter) {
    var extensionElements = $filter('filter')(this.shapeModel.Elements, { Name: "extensionElements" });
    if (extensionElements && extensionElements.length > 0) {
        this.extensionElementModel = extensionElements[0];
    }
    var userRoleModel = $filter('filter')(this.extensionElementModel.Elements, { Name: "userrole" })[0];
    if (userRoleModel == null || userRoleModel == undefined) {
        userRoleModel = { prefix: "sbpmn", Name: "userrole", dictAttributes: { roles: "" }, IsValueInCDATAFormat: false, Elements: [], Children: [] };
        this.extensionElementModel.Elements.push(userRoleModel);
    }
    else {
        if (userRoleModel.dictAttributes.roles == undefined) {
            userRoleModel.dictAttributes.roles = "";
        }
    }
    this.userRoleModel = userRoleModel;
}


function bpmUserTaskPerformersProperties($filter, $getQueryparam, $scope, $rootScope) {
    //newScope.TargetSiteList = $scope.TargetSiteList;
    //newScope.lstbpmnrelatedcodeValues = $scope.lstBPMNRelatedCodeValues;
    var ctrl = this;
    ctrl.queryParametersValues = $getQueryparam.getMapVariableIds(ctrl.mapvariablesmodel.Elements);
    var extrasettingsModel = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extraSettings" });
    if (extrasettingsModel && extrasettingsModel.length > 0) {
        ctrl.ExtraSettingsModel = extrasettingsModel[0];
    }
    if (ctrl.ExtraSettingsModel && !ctrl.ExtraSettingsModel.dictAttributes.sfwCanCompleteFromLeftPanel) {
        ctrl.ExtraSettingsModel.dictAttributes.sfwCanCompleteFromLeftPanel = "False";
    }
    var extensionElements = $filter('filter')(ctrl.shapeModel.Elements, { Name: "extensionElements" });
    if (extensionElements && extensionElements.length > 0) {
        ctrl.extensionElementModel = extensionElements[0];
    }
    var performersModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "performers" });
    if (performersModel && performersModel.length > 0) {
        ctrl.PerformersModel = performersModel[0];
        ctrl.PerformersModels = performersModel;
    }
    if (ctrl.PerformersModel) {
        var defaultConditions = $filter('filter')(ctrl.PerformersModel.Elements, { Name: "defaultcondition" });
        if (defaultConditions && defaultConditions.length > 0) {
            ctrl.defaultConditionModel = defaultConditions[0];
            ctrl.defaultConditionModels = defaultConditions;
        }
    }
    if (ctrl.PerformersModel && ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter == undefined) {
        ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter = "False";
    }
    ctrl.AssignmentOptions = ["By Load", "Everyone", "Sequential"];
    //newScope.FollowAssignmentOptions = ["True", "False"];
    ctrl.ObjConditonAndExternaltask = { IsPreCondition: false, IsExternaltask: false };
    if (ctrl.PerformersModel && ctrl.PerformersModel.Elements[0].Name == "preconditions") {
        ctrl.ObjConditonAndExternaltask.IsPreCondition = true;
        ctrl.objPreconditonsModel = ctrl.PerformersModel.Elements[0];
        ctrl.objPreconditonModel = ctrl.objPreconditonsModel.Elements[0];
    }
    var externalTaskModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "externalTask" });
    if (externalTaskModel && externalTaskModel.length > 0) {
        ctrl.ObjConditonAndExternaltask.IsExternaltask = true;
        ctrl.externalTaskModel = externalTaskModel[0];
    }

    ctrl.onChangeConstant = function () {
        ctrl.PerformersModel.dictAttributes.sfwFollowAssignment = "";
        ctrl.populateFollowAssignmentValues();
    };
    ctrl.populateFollowAssignmentValues = function () {
        ctrl.FollowAssignmentOptions = [];
        if (ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter && ctrl.PerformersModel.dictAttributes.sfwFollowAssignmentUsingParameter != "False") {
            var booleanVariables = ctrl.mapvariablesmodel.Elements.filter(function (item) { return item.dictAttributes.dataType == "bool"; });
            if (booleanVariables && booleanVariables.length > 0) {
                ctrl.FollowAssignmentOptions = booleanVariables.map(function (x) { return x.dictAttributes.id; });
            }
        }
        else {
            ctrl.FollowAssignmentOptions = ["True", "False"];
        }
    };

    ctrl.setPreConditionOrDefaultCondition = function () {
        var newScope = $scope.$new();
        newScope.obj = {};
        var performersModel = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "performers" });
        if (performersModel && performersModel.length > 0) {
            ctrl.PerformersModel = performersModel[0];
            ctrl.PerformersModels = performersModel;
        }
        if (ctrl.PerformersModel) {
            var defaultConditions = $filter('filter')(ctrl.PerformersModel.Elements, { Name: "defaultcondition" });
            if (defaultConditions && defaultConditions.length > 0) {
                ctrl.defaultConditionModel = defaultConditions[0];
                ctrl.defaultConditionModels = defaultConditions;
            }
        }
        var lstForms;
        if (ctrl.extensionElementModel && ctrl.extensionElementModel.Elements.length > 0) {
            lstForms = $filter('filter')(ctrl.extensionElementModel.Elements, { Name: "form" }, true);
        }
        var ReEnterForm;
        if (lstForms && lstForms.length > 0) {
            for (var i = 0; i < lstForms.length; i++) {
                if (lstForms[i].dictAttributes.sfwMode == "update") {
                    ReEnterForm = lstForms[i];
                    break;
                }
            }
        }
        newScope.lstActiveForm = [];
        newScope.setActiveFomList = function (FormName) {
            if (FormName && FormName.contains("=")) {
                var tempActiveForm = FormName.split(";");
                if (tempActiveForm) {
                    for (var i = 0; i < tempActiveForm.length; i++) {
                        var tempObj = tempActiveForm[i].split("=");
                        newScope.lstActiveForm.push(tempObj[1]);
                    }
                }
            }
        };
        if (ReEnterForm) {
            newScope.setActiveFomList(ReEnterForm.dictAttributes.sfwFormName);
        }

        newScope.AddDefaultConditionModelsForMultipleForms = function (ConditionModels, lstActiveForms, param) {
            if (param == "Default") {
                //Deleting unwanted Default/pre Conditons
                for (var i = ConditionModels.length - 1; i >= 0; i--) {
                    if (!ConditionModels[i].dictAttributes.sfwActiveForm) {
                        ConditionModels.splice(i, 1);
                    } else {
                        var isConditionFound = false;
                        for (var j = 0; j < lstActiveForms.length > 0; j++) {
                            if (ConditionModels[i].dictAttributes.sfwActiveForm == lstActiveForms[j]) {
                                isConditionFound = true;
                                break;
                            }
                        }
                        if (!isConditionFound) {
                            ConditionModels.splice(i, 1);
                        }
                    }
                }

                //Adding Default/pre Condition
                for (var k = 0; k < lstActiveForms.length; k++) {
                    var isConditionFound = false;
                    for (l = 0; l < ConditionModels.length; l++) {
                        if (ConditionModels[l].dictAttributes.sfwActiveForm == lstActiveForms[k]) {
                            isConditionFound = true;
                            break;
                        }
                    }

                    if (!isConditionFound) {

                        var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };
                        defaultConditionModel.dictAttributes.sfwActiveForm = lstActiveForms[k];
                        ConditionModels.push(defaultConditionModel);
                    }
                }
            }
            else {
                //Deleting unwanted Default/pre Conditons

                for (var j = ConditionModels.Elements.length - 1; j >= 0; j--) {

                    if (!ConditionModels.Elements[j].dictAttributes.sfwActiveForm) {
                        ConditionModels.Elements.splice(j, 1);
                    } else {
                        var isConditionFound = false;
                        for (var j = 0; j < lstActiveForms.length > 0; j++) {
                            if (ConditionModels.Elements[j].dictAttributes.sfwActiveForm == lstActiveForms[j]) {
                                isConditionFound = true;
                                break;
                            }
                        }
                        if (!isConditionFound) {
                            ConditionModels.Elements.splice(i, 1);
                        }
                    }
                }

                var performersModel = ConditionModels;
                //if (param == "PreConditions") {
                //    performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };
                //    ConditionModels.push(performersModel);
                //}
                //Adding Default/pre Condition
                for (var k = 0; k < lstActiveForms.length; k++) {
                    var isConditionFound = false;

                    for (m = 0; m < ConditionModels.Elements.length; m++) {

                        if (ConditionModels.Elements[m].dictAttributes.sfwActiveForm == lstActiveForms[k]) {
                            isConditionFound = true;
                            break;
                        }
                    }


                    if (!isConditionFound && performersModel) {
                        var preConditionsModel = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: { sfwName: "Always" }, Elements: [],
                                Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                            }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                        };
                        preConditionsModel.dictAttributes.sfwActiveForm = lstActiveForms[k];
                        performersModel.Elements.push(preConditionsModel);
                    }
                }
            }
        };

        var ObjConditonAndExternaltask = JSON.stringify(ctrl.ObjConditonAndExternaltask);
        newScope.ObjConditonAndExternaltask = JSON.parse(ObjConditonAndExternaltask);
        if (ctrl.defaultConditionModel && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            if (newScope.lstActiveForm && newScope.lstActiveForm.length == 0) {
                var defaultConditionModel = JSON.stringify(ctrl.defaultConditionModel);
                newScope.defaultConditionModel = JSON.parse(defaultConditionModel);
            } else {
                var defaultConditionModels = JSON.stringify(ctrl.defaultConditionModels);
                newScope.defaultConditionModels = JSON.parse(defaultConditionModels);
                newScope.AddDefaultConditionModelsForMultipleForms(newScope.defaultConditionModels, newScope.lstActiveForm, "Default");
            }
        }

        newScope.onChangePreconditionsActiveForm = function (activeForm) {
            for (var i = 0; i < newScope.PerformersModels.length; i++) {
                for (var j = 0; j < newScope.PerformersModels[i].Elements.length; j++) {
                    if (newScope.PerformersModels[i].Elements[j].dictAttributes.sfwActiveForm == activeForm) {
                        newScope.objPreconditonsModel = newScope.PerformersModels[i].Elements[j];
                        newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[0];
                        break;
                    }
                }

            }
        };
        if (ctrl.PerformersModel && ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            var PerformersModel = JSON.stringify(ctrl.PerformersModel);
            newScope.PerformersModel = JSON.parse(PerformersModel);
            if (newScope.PerformersModel && newScope.PerformersModel.Elements[0].Name == "preconditions") {
                newScope.ObjConditonAndExternaltask.IsPreCondition = true;
                if (newScope.lstActiveForm && newScope.lstActiveForm.length == 0) {
                    newScope.objPreconditonsModel = newScope.PerformersModel.Elements[0];
                    newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[0];
                } else {
                    var PerformersModels = JSON.stringify(ctrl.PerformersModels);
                    newScope.PerformersModels = JSON.parse(PerformersModels);
                    newScope.AddDefaultConditionModelsForMultipleForms(newScope.PerformersModels[0], newScope.lstActiveForm, "PreConditions");
                    newScope.PerformersModels.sfwActiveForm = newScope.lstActiveForm[0];
                    newScope.onChangePreconditionsActiveForm(newScope.PerformersModels.sfwActiveForm);
                }
            }
        }

        newScope.mapvariablesmodel = ctrl.mapvariablesmodel;
        newScope.IsDisabledforVersioning = ctrl.isdisabledforversioning;
        newScope.queryParametersValues = ctrl.queryParametersValues;
        newScope.UserRoles = ctrl.userRoles;
        newScope.operatorsList = ['GreaterThan', 'GreaterThanEqual', 'Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'StartsWith', 'EndsWith'];


        newScope.setClasstoSelectedObj = function (index) {
            newScope.obj.selectedobjindex = index;
        };

        newScope.lstBPMNRelatedCodeValues = ctrl.lstBpmnRelatedCodeValues;

        newScope.addPerformersDefaultCondition = function () {
            var conditionModel = {
                dictAttributes: { roleoperator: "Equal", skilloperator: "Equal", locationoperator: "Equal", authorityLeveloperator: "Equal", positionoperator: "Equal", useroperator: "Equal" },
                Elements: [], Children: [], Name: "condition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
            };
            if (newScope.defaultConditionModel && newScope.lstActiveForm.length == 0) {
                newScope.defaultConditionModel.Elements.push(conditionModel);
            } else if (newScope.defaultConditionModels.sfwActiveForm) {
                for (var i = 0; i < newScope.defaultConditionModels.length; i++) {
                    if (newScope.defaultConditionModels[i].dictAttributes.sfwActiveForm == newScope.defaultConditionModels.sfwActiveForm) {
                        newScope.defaultConditionModels[i].Elements.push(conditionModel);
                        break;
                    }
                }

            }
            conditionModel.IsUsertaskConditionVisibility = true;
        };

        newScope.deletePerformersDefaultCondition = function () {
            if (newScope.defaultConditionModel && newScope.defaultConditionModel.selectedCondition && newScope.lstActiveForm.length == 0) {
                var idx = newScope.defaultConditionModel.Elements.indexOf(newScope.defaultConditionModel.selectedCondition);
                newScope.defaultConditionModel.Elements.splice(idx, 1);
                newScope.defaultConditionModel.selectedCondition = undefined;
            } else if (newScope.defaultConditionModels && newScope.defaultConditionModels.sfwActiveForm) {
                for (var i = 0; i < newScope.defaultConditionModels.length; i++) {
                    if (newScope.defaultConditionModels[i].dictAttributes.sfwActiveForm == newScope.defaultConditionModels.sfwActiveForm) {
                        var indx = newScope.defaultConditionModels[i].Elements.indexOf(newScope.defaultConditionModels[i].selectedCondition);
                        newScope.defaultConditionModels[i].Elements.splice(indx, 1);
                        newScope.defaultConditionModels[i].selectedCondition = undefined;
                        break;
                    }
                }
            }
        };

        newScope.changePreConditionModelBasedonselectedtab = function (preconditonModel, index) {
            //newScope.objTab = {};
            //newScope.objTab.selectedtabindex = index;
            newScope.objPreconditonModel = preconditonModel;
            newScope.obj.selectedobjindex = undefined;
        };

        newScope.addPerformersPreCondition = function () {
            if (newScope.objPreconditonModel != null && newScope.objPreconditonModel != undefined) {
                var preConditionModel = {
                    dictAttributes: { roleoperator: "Equal", skilloperator: "Equal", locationoperator: "Equal", authorityLeveloperator: "Equal", positionoperator: "Equal", useroperator: "Equal" },
                    Elements: [], Children: [], Name: "condition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                };
                newScope.objPreconditonModel.Elements.push(preConditionModel);
                newScope.objPreconditonModel.selectedCondition = preConditionModel;
                newScope.objPreconditonModel.selectedCondition.IsUsertaskConditionVisibility = true;
            }
        };

        newScope.deletePerformersPreCondition = function () {
            if (newScope.objPreconditonModel != null && newScope.objPreconditonModel.selectedCondition) {
                var idx = newScope.objPreconditonModel.Elements.indexOf(newScope.objPreconditonModel.selectedCondition);
                newScope.objPreconditonModel.Elements.splice(idx, 1);
                newScope.objPreconditonModel.selectedCondition = undefined;
            }
        };

        newScope.AddNewTabForPreConditonsClick = function () {
            var preconditonName = GetNewConditonName("PreCondition", newScope.objPreconditonsModel, 1);
            var newScopeForPreconditionDailog = $scope.$new();
            newScopeForPreconditionDailog.objConditon = {};
            newScopeForPreconditionDailog.objConditon.PreConditionName = preconditonName;
            newScopeForPreconditionDailog.dialog = $rootScope.showDialog(newScopeForPreconditionDailog, "", "BPM/views/Performer/NewPreConditionTemplate.html");
            newScopeForPreconditionDailog.setNewPreConditionDetails = function () {
                var isInvalidId = false;
                var strErrorMsg = "";
                if (!newScopeForPreconditionDailog.objConditon.PreConditionName) {
                    isInvalidId = true;
                    strErrorMsg = "ID cannot be blank.";
                }
                else {
                    isInvalidId = CheckForDuplicateIDForConditions(newScopeForPreconditionDailog.objConditon.PreConditionName, newScope.objPreconditonsModel);
                    strErrorMsg = "Duplicate ID Present. Please enter another ID";
                }
                if (!isInvalidId) {
                    var preConditionsModel = {
                        dictAttributes: { sfwName: "PreCondition" }, Elements: [],
                        Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                    };
                    preConditionsModel.dictAttributes.sfwName = newScopeForPreconditionDailog.objConditon.PreConditionName;
                    newScope.objPreconditonsModel.Elements.push(preConditionsModel);

                    newScope.objPreconditonModel = preConditionsModel;
                    newScope.obj.selectedobjindex = undefined;
                    newScopeForPreconditionDailog.dialog.close();
                } else {
                    if (strErrorMsg) {
                        alert(strErrorMsg);
                    }
                }
            };

            newScopeForPreconditionDailog.closeNewConditionDialog = function () {
                newScopeForPreconditionDailog.dialog.close();
            };
        };
        newScope.removePreconditonfromPreconditionsModel = function (index) {
            newScope.objPreconditonsModel.Elements.splice(index, 1);
            if (newScope.objPreconditonsModel.Elements.length > 0) {
                newScope.objPreconditonModel = newScope.objPreconditonsModel.Elements[newScope.objPreconditonsModel.Elements.length - 1];
                newScope.obj.selectedobjindex = undefined;
            }
        };
        newScope.scrollLeft = function (event) {
            event.preventDefault();
            $element = $(event.target);
            $element.next("ul").animate({
                scrollLeft: "-=200px"
            }, "slow");
        }
        newScope.scrollRight = function (event) {
            event.preventDefault();
            $element = $(event.target);
            $element.prev("ul").animate({
                scrollLeft: "+=200px"
            }, "slow");
        }

        if (newScope.lstActiveForm && newScope.defaultConditionModels && newScope.lstActiveForm.length > 0) {
            newScope.defaultConditionModels.sfwActiveForm = newScope.lstActiveForm[0];
        }

        newScope.templateName = "BPM/views/Performer/UserTaskDefaultandPreconditions.html";
        newScope.dialogOptions = { height: 600, width: 800 };

        newScope.editNameForPreCondition = function (obj) {
            if (obj && !newScope.IsDisabledforVersioning) {
                var preconditonName = obj.dictAttributes.sfwName;
                var newScopeForPreconditionDailog = $scope.$new();
                newScopeForPreconditionDailog.objConditon = {};
                newScopeForPreconditionDailog.objConditon.PreConditionName = preconditonName;
                newScopeForPreconditionDailog.dialog = $rootScope.showDialog(newScopeForPreconditionDailog, "", "BPM/views/Performer/NewPreConditionTemplate.html");
                newScopeForPreconditionDailog.setNewPreConditionDetails = function () {
                    var isInvalidId = false;
                    var strErrorMsg = "";
                    if (!newScopeForPreconditionDailog.objConditon.PreConditionName) {
                        isInvalidId = true;
                        strErrorMsg = "ID cannot be blank.";
                    }
                    else {
                        var index = newScope.objPreconditonsModel.Elements.indexOf(obj);
                        isInvalidId = CheckForDuplicateIDForConditionsWhileEdit(newScopeForPreconditionDailog.objConditon.PreConditionName, newScope.objPreconditonsModel, index);
                        strErrorMsg = "Duplicate ID Present. Please enter another ID";
                    }
                    if (!isInvalidId) {

                        obj.dictAttributes.sfwName = newScopeForPreconditionDailog.objConditon.PreConditionName;

                        newScopeForPreconditionDailog.dialog.close();
                    } else {
                        if (strErrorMsg) {
                            alert(strErrorMsg);
                        }
                    }
                };

                newScopeForPreconditionDailog.closeNewConditionDialog = function () {
                    newScopeForPreconditionDailog.dialog.close();
                };
            }
        };

        newScope.onOKClick = function () {
            ctrl.ObjConditonAndExternaltask = newScope.ObjConditonAndExternaltask;
            $rootScope.UndRedoBulkOp("Start");
            if (newScope.lstActiveForm.length > 0 && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.EditPropertyValue(ctrl.PerformersModel.Elements, ctrl.PerformersModel, "Elements", newScope.defaultConditionModels);
            } else if (ctrl.defaultConditionModel && !ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.EditPropertyValue(ctrl.defaultConditionModel.Elements, ctrl.defaultConditionModel, "Elements", newScope.defaultConditionModel.Elements);
            }
            if (ctrl.PerformersModels && ctrl.ObjConditonAndExternaltask.IsPreCondition && newScope.lstActiveForm.length > 0) {
                for (var i = ctrl.extensionElementModel.Elements.length - 1; i >= 0; i--) {
                    if (ctrl.extensionElementModel.Elements[i].Name == "performers") {
                        $rootScope.DeleteItem(ctrl.extensionElementModel.Elements[i], ctrl.extensionElementModel.Elements, null);
                    }
                }
                for (var j = 0; j < newScope.PerformersModels.length; j++) {
                    $rootScope.InsertItem(newScope.PerformersModels[j], ctrl.extensionElementModel.Elements, 0, null);
                }
            }
            else if (ctrl.PerformersModel && ctrl.ObjConditonAndExternaltask.IsPreCondition) {
                $rootScope.DeleteItem(ctrl.PerformersModel.Elements[0], ctrl.PerformersModel.Elements, null);
                $rootScope.InsertItem(newScope.PerformersModel.Elements[0], ctrl.PerformersModel.Elements, 0, null);
            }
            $rootScope.UndRedoBulkOp("End");
            newScope.dialog.close();
        };
        newScope.onCancelClick = function () {
            newScope.dialog.close();
        };

        newScope.dialog = $rootScope.showDialog(newScope, "Condition", newScope.templateName, newScope.dialogOptions);
    };

    ctrl.removeOrAddModelBasedOnExternaltaskCondition = function () {
        if (ctrl.ObjConditonAndExternaltask.IsExternaltask) {
            ctrl.externalTaskModel = {
                dictAttributes: {
                }, Elements: [], Children: [], Name: "externalTask", prefix: "", IsValueInCDATAFormat: false, Value: ""
            };
            ctrl.extensionElementModel.Elements.push(ctrl.externalTaskModel);
        } else {
            for (var i = 0; i < ctrl.extensionElementModel.Elements.length; i++) {
                if (ctrl.extensionElementModel.Elements[i].Name == 'externalTask') {
                    ctrl.extensionElementModel.Elements.splice(i, 1);
                    break;
                }
            }
        }
    };

    ctrl.changeModelBasedOnConditionInPreformers = function () {
        if (ctrl.ObjConditonAndExternaltask.IsPreCondition) {
            var preConditionsModel = {
                dictAttributes: {}, Elements: [{
                    dictAttributes: { sfwName: "Always" }, Elements: [],
                    Children: [], Name: "precondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
                }], Children: [], Name: "preconditions", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: ""
            };
            // Remove old model
            while (ctrl.PerformersModel.Elements.length > 0) {
                ctrl.PerformersModel.Elements.splice(0, 1);
            }
            ctrl.PerformersModel.Elements.splice(0, 0, preConditionsModel);
            //newScope.PerformersModel.Elements[0] = preConditionsModel;
            ctrl.defaultConditionModel = null;
            ctrl.objPreconditonsModel = ctrl.PerformersModel.Elements[0];
            ctrl.objPreconditonModel = ctrl.objPreconditonsModel.Elements[0];
            //newScope.obj.selectedobjindex = undefined;
        }
        else {
            var defaultConditionModel = { dictAttributes: {}, Elements: [], Children: [], Name: "defaultcondition", prefix: "sbpmn", IsValueInCDATAFormat: false, Value: "" };

            // Remove old model
            //for (var i = ctrl.extensionElementModel.Elements.length - 1; i >= 0; i--) {
            //    if (ctrl.extensionElementModel.Elements[i].Name == "performers") {
            //        $rootScope.DeleteItem(ctrl.extensionElementModel.Elements[i], ctrl.extensionElementModel.Elements, null);
            //    }
            //}
            //var performersModel = { Children: [], dictAttributes: {}, Elements: [], Value: "", Name: "performers" };
            //ctrl.PerformersModel = performersModel;
            //$rootScope.InsertItem(ctrl.PerformersModel, ctrl.extensionElementModel.Elements, 0, null);
            //removed above code beacuse if deleted and added the assignment option and other attributes were getting cleared
            // Remove old model
            while (ctrl.PerformersModel.Elements.length > 0) {
                ctrl.PerformersModel.Elements.splice(0, 1);
            }
            ctrl.PerformersModel.Elements.splice(0, 0, defaultConditionModel);
            //newScope.PerformersModel.Elements[0] = defaultConditionModel;
            ctrl.defaultConditionModel = defaultConditionModel;
        }
    };


    ctrl.populateFollowAssignmentValues();
}

app.component('bpmLanePerformersProperties', {
    bindings: {
        shapeModel: '<',
        userRoles: '<',
        isdisabledforversioning: '<'
    },
    templateUrl: 'BPM/views/Performer/LanePerformersProperties.html',
    controller: ["$filter", bpmLanePerformersPropertiesController]
});

app.component('bpmUserTaskPerformersProperties', {
    bindings: {
        shapeModel: '<',
        isdisabledforversioning: '<',
        targetsitelist: '<',
        lstBpmnRelatedCodeValues: '<',
        mapvariablesmodel: '<',
        userRoles: '<',
    },
    templateUrl: 'BPM/views/Performer/UserTaskPerformersProperties.html',
    controller: ["$filter", "$getQueryparam", "$scope", "$rootScope", bpmUserTaskPerformersProperties]
});