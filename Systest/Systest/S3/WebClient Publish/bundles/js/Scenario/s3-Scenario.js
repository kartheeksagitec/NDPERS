app.controller("ScenarioController", ["$scope", "$filter", "$rootScope", "$timeout", "$EntityIntellisenseFactory", "$resourceFactory", "$NavigateToFileService", "$interval", function ($scope, $filter, $rootScope, $timeout, $EntityIntellisenseFactory, $resourceFactory, $NavigateToFileService, $interval) {
    //rootScope = $scope;

    //#region Variables
    $rootScope.IsLoading = true;

    $scope.lstEntityFields = [];
    $scope.currentfile = $rootScope.currentopenfile.file;

    if ($scope.currentfile.FileName.match("^rso")) {
        $scope.IsObjectBasedScenario = true;
    }
    else {
        $scope.IsObjectBasedScenario = false;
    }
    if ($scope.currentfile.FileName.match("^rsx")) {
        $scope.IsExcelBasedScenario = true;
    }
    else {
        $scope.IsExcelBasedScenario = false;
    }
    $scope.isDirty = false;
    $scope.selectedTestFilter = "All";
    $scope.showSource = false;
    $scope.selectedDesignSource = false;
    var editor_html;
    $scope.lstEntityFieldsCollection = [];
    $scope.IsDiagramDivExpanded = true;
    //#endregion

    //#region Methods to Load Scenario Model
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (model) {
        if (model) {
            $scope.receivescenariomodel(model);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }
    });

    $scope.selectDefaultScenario = function () {
        if ($scope.objEntityBasedScenario != undefined && $scope.objEntityBasedScenario.Elements.length > 0) {
            $scope.objEntityBasedScenario.Elements.some(function (item) {
                if (item.Name == "test") {
                    $scope.TestSelectionChange(item);
                    $scope.objEntityBasedScenario.SelectedStep.IsRunSelected = false;
                    return true;
                }
            });
        }
    };
    $scope.receivescenariomodel = function (data) {
        $scope.$apply(function () {


            $scope.objScenarioExtraFields = [];
            $scope.lstXmlMethods = [];

            $scope.objEntityBasedScenario = data;

            $scope.objEntity = $scope.objEntityBasedScenario.objExtraData.entityModel;

            $scope.objScenarioExtraFields = $filter('filter')($scope.objEntityBasedScenario.Elements, { Name: 'ExtraFields' });
            if ($scope.objScenarioExtraFields.length > 0) {
                $scope.objScenarioExtraFields = $scope.objScenarioExtraFields[0];
                //$scope.removeExtraFieldsDataInToMainModel();
            }
            $scope.PopulateEntityRelatedDetail();

            $scope.CompileResult = $scope.objEntityBasedScenario.objExtraData.CompileResult;
            $scope.PopulateCompilationResultDetail();

            if ($scope.objEntityBasedScenario.objExtraData.ruleFileInfo) {
                $scope.RuleType = $scope.objEntityBasedScenario.objExtraData.ruleFileInfo.FileType;
                $scope.ruleFileInfo = $scope.objEntityBasedScenario.objExtraData.ruleFileInfo;
            }

            angular.forEach($scope.objEntityBasedScenario.Elements, function (step) {
                if (step.Name == "test") {
                    $scope.InitializeData(step);
                }
            });

            angular.forEach($scope.objEntityBasedScenario.Elements, function (step) {
                if (step.Name == "test") {
                    $scope.SetValue(step, true);
                }
            });

            if ($scope.objScenarioExtraFields.length == 0) {
                $scope.objScenarioExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {
                    }, Elements: []
                };
            }

            $scope.selectDefaultScenario();

            $timeout(function () {
                ComponentsPickers.init();
            });
            $rootScope.IsLoading = false;
        });
    };


    //#endregion

    //#region Methods for Design To source and Source To Design 

    $scope.ShowDesign = function () {

        if ($scope.selectedDesignSource == true) {
            var xmlstring = $scope.editor.getValue();

            if (xmlstring != null && xmlstring != "") {

                $rootScope.IsLoading = true;
                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;
                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $scope.currentfile, lineno);
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

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Source-Design", lineNumber);
                }
                $scope.receivescenariodesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receivescenariomodel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}
                    //Commented following code, because source to design synchronization is hold for now.
                    $scope.$evalAsync(function () {
                        $scope.testSelectedObj = undefined;
                        $scope.parameterSelectedObj = undefined;
                        if (path != null) {

                            var items = [];
                            var objHierarchy;
                            if (path.contains("-") || path.contains(",")) {
                                objHierarchy = $scope.objEntityBasedScenario;
                                for (var i = 0; i < path.split(',').length; i++) {
                                    objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                                    items.push(objHierarchy);
                                }
                            }

                            if (items != null && items.length > 0) {
                                // $scope.TestSelectionChange(items[0]);
                                $scope.objEntityBasedScenario.SelectedStep = items[0];
                            } else {
                                $scope.selectDefaultScenario();
                            }
                            if (items.length > 1) {

                                if (items[items.length - 2] && (items[items.length - 2].Name == "input" || items[items.length - 2].Name == "parameter")) {
                                    $scope.setSelectedParameters(items[items.length - 1]);
                                }
                                else if (items[items.length - 2] && items[items.length - 2].Name == "output") {
                                    $scope.setSelectedOutputParam(items[items.length - 1]);
                                }
                                else if (items[items.length - 2] && items[items.length - 2].Name == "parameters") {
                                    $scope.setSelectedMethodParameters(items[items.length - 1]);
                                }
                                else if (items[items.length - 1].Name == "input" || items[items.length - 1].Name == "parameter") {
                                    if ($scope.SelectedParameter) {
                                        var vrSelectedParameter = FindDeepNode($scope.objEntityBasedScenario, $scope.SelectedParameter);
                                        $scope.setSelectedParameters(vrSelectedParameter);
                                    }
                                }
                                else if (items[items.length - 1].Name == "output") {
                                    if ($scope.SelectedOutputParam) {
                                        var vrSelectedParameter = FindDeepNode($scope.objEntityBasedScenario, $scope.SelectedOutputParam);
                                        $scope.setSelectedOutputParam(vrSelectedParameter);
                                    }
                                }
                                else if (items[items.length - 1].Name == "parameters") {
                                    if ($scope.SelectedMethodParameter) {
                                        var vrSelectedParameter = FindDeepNode($scope.objEntityBasedScenario, $scope.SelectedMethodParameter);
                                        $scope.setSelectedMethodParameters(vrSelectedParameter);
                                    }
                                }
                            }
                        }

                        $rootScope.IsLoading = false;
                    });
                };
            }
        }
    };
    $scope.isSourceDirty;
    $scope.sourceChanged = function () {
        $scope.isSourceDirty = true;
        $scope.isDirty = true;
        $scope.SearchSource.IsSearchCriteriaChange = true;
    };
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            var sObj;
            var indexPath = [];
            var pathString;

            //$scope.showSource = true;
            if ($scope.objEntityBasedScenario != null && $scope.objEntityBasedScenario != undefined) {
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();
                var objreturn1 = GetBaseModel($scope.objEntityBasedScenario);
                var strobj = JSON.stringify(objreturn1);
                var nodeId = [];
                var selectedScenarioItem;

                if ($scope.objEntityBasedScenario.SelectedStep != undefined) {
                    //sObj = FindDeepNode($scope.objEntityBasedScenario, $scope.objEntityBasedScenario.SelectedStep);
                    //pathString = getPathSource(sObj, indexPath);
                    //angular.copy(pathString.reverse(), nodeId);

                    var pathToObject = [];

                    sObj = $scope.FindDeepNode($scope.objEntityBasedScenario, $scope.objEntityBasedScenario.SelectedStep, pathToObject);
                    pathString = $scope.getPathSource($scope.objEntityBasedScenario, pathToObject, indexPath);
                    angular.copy(pathString, nodeId);
                }

                if ($scope.SelectedParameter) {
                    selectedScenarioItem = $scope.SelectedParameter;
                }
                else if ($scope.SelectedOutputParam) {
                    selectedScenarioItem = $scope.SelectedOutputParam;
                }
                else if ($scope.SelectedMethodParameter) {
                    selectedScenarioItem = $scope.SelectedMethodParameter;
                }

                if (selectedScenarioItem) {
                    sObj = undefined;
                    indexPath = [];
                    nodeId = [];

                    //sObj = FindDeepNode($scope.objEntityBasedScenario, selectedScenarioItem);
                    //pathString = getPathSource(sObj, indexPath);
                    //angular.copy(pathString.reverse(), nodeId);

                    var pathToObject = [];

                    sObj = $scope.FindDeepNode($scope.objEntityBasedScenario, selectedScenarioItem, pathToObject);
                    pathString = $scope.getPathSource($scope.objEntityBasedScenario, pathToObject, indexPath);
                    angular.copy(pathString, nodeId);
                }

                if (strobj.length < 32000) {
                    hubMain.server.getSourceXmlObject(strobj, $scope.currentfile, nodeId);
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
                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Design-Source", nodeId);
                }
                $scope.receivesourcexml = function (xmlstring, lineno) {
                    $scope.$apply(function () {
                        $scope.xmlSource = xmlstring;
                        var ID = $scope.currentfile.FileName;
                        setDataToEditor($scope, xmlstring, lineno, ID);
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $scope.objEntityBasedScenario.SelectedStep = undefined;
                        });

                        if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                            $rootScope.IsLoading = false;
                        }
                    });
                };
            }
        }
    };

    $scope.FindNodeHierarchy = function (objParentElements, index) {
        if (objParentElements && objParentElements.Elements) {
            var newObj = objParentElements.Elements[index];
            if (newObj == undefined) {
                newObj = objParentElements.Elements[index - 1];
            }
            return newObj;
        }
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                var isNodeInPath = $scope.isValidObject(item, selectedItem);
                if (isNodeInPath) {
                    pathToObject.push(item);
                }
                if (item == selectedItem) {
                    return selectedItem;
                }
                else if (item.Elements && item.Elements.length > 0) {
                    selectedItem = $scope.FindDeepNode(item, selectedItem, pathToObject);
                    return selectedItem;
                }
            });
        }
        return selectedItem;
    };
    $scope.getPathSource = function (objModel, pathToObject, indexPath) {
        for (var i = 0; i < pathToObject.length; i++) {
            if (i == 0) {
                var indx = objModel.Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
            else {
                var indx = pathToObject[i - 1].Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
        }
        return indexPath;
    };
    $scope.isValidObject = function (objParentElements, selectedItem) {
        var result;
        if (objParentElements == selectedItem) {
            result = true;
            return result;
        }

        for (var ele in objParentElements.Elements) {
            if (objParentElements.Elements[ele] == selectedItem) {
                result = true;
                return result;
            }
            if (objParentElements.Elements[ele].Elements && objParentElements.Elements[ele].Elements.length > 0) {
                for (iele in objParentElements.Elements[ele].Elements) {
                    result = $scope.isValidObject(objParentElements.Elements[ele].Elements[iele], selectedItem);
                    if (result == true) {
                        return result;
                    }
                }
            }
        }
        return result;
    };
    //xml Source End

    //#endregion

    //#region Init Methods


    $scope.InitializeData = function (step) {
        if (step) {
            angular.forEach(step.Elements, function (value, key) {
                if (value.Name == 'input') {
                    step.objInputVM = value;
                }

                else if (value.Name == 'parameter') {
                    step.objParameterVM = value;
                }

                else if (value.Name == 'output') {
                    step.objOutputVM = value;
                }

                else if ($scope.IsObjectBasedScenario && value.Name == 'parameters') {
                    step.objParameters = value;
                }
            });

            if (step.objInputVM == undefined) {
                step.objInputVM = $scope.CreateNewObject("input", step);
            }

            if (step.objOutputVM == undefined) {
                step.objOutputVM = $scope.CreateNewObject("output", step);
            }

            if (step.objParameterVM == undefined) {
                step.objParameterVM = $scope.CreateNewObject("parameter", step);
            }

            if ($scope.IsObjectBasedScenario && step.objParameters == undefined) {
                step.objParameters = $scope.CreateNewObject("parameters", step);
            }
        }
    };

    $scope.PopulateCompilationResultDetail = function () {
        if ($scope.objEntityBasedScenario.Elements.length > 0) {

            angular.forEach($scope.objEntityBasedScenario.Elements, function (step) {
                if (step.Name != "ExtraFields") {
                    $scope.SetValue(step, false);
                }
            });
        }
    };

    $scope.receiveCompilationResult = function (data) {
        $scope.$apply(function () {
            $scope.CompileResult = JSON.parse(data);

            $scope.PopulateCompilationResultDetail();
        });
    };

    $scope.PopulateEntityRelatedDetail = function () {
        $scope.lstXmlMethods = [];
        var lstEntities = $EntityIntellisenseFactory.getEntityIntellisense();
        if ($scope.objEntity) {
            if (lstEntities && lstEntities.length > 0) {
                var lst = lstEntities.filter(function (item) {
                    return item.ID == $scope.objEntity.dictAttributes.ID;
                }
                );
                if (lst && lst.length > 0) {
                    //
                    //    angular.forEach($scope.objEntity.Elements, function (ele) {
                    if (lst[0].XmlMethods && lst[0].XmlMethods.length > 0) {
                        // var methods = ele.Elements.filter(function (x) { return x.dictAttributes.ID && x.dictAttributes.ID.trim() != "" && x.dictAttributes.sfwMethodType == "Load" })
                        angular.forEach(lst[0].XmlMethods, function (method) {
                            if (method.ID && method.ID.trim() && method.MethodType == "Load") {
                                $scope.lstXmlMethods.push(method);
                            }
                        });
                    }
                }
            }
        }

        // not needed - selection is done in receive function
        //if ($scope.objEntityBasedScenario != undefined && $scope.objEntityBasedScenario.Elements.length > 0) {
        //    $scope.objEntityBasedScenario.SelectedStep = $scope.objEntityBasedScenario.Elements[$scope.objEntityBasedScenario.Elements.length - 1];
        //    $scope.objEntityBasedScenario.SelectedStep.IsRunSelected = false;
        //}
    };


    $scope.UpdateDisplayInputOutputList = function (step, isLoading, isUndoRedoEnable) {
        if (isUndoRedoEnable) {
            $rootScope.EditPropertyValue(step.lstInputValues, step, "lstInputValues", []);
            $rootScope.EditPropertyValue(step.lstOutputValues, step, "lstOutputValues", []);
        }
        else {
            step.lstInputValues = [];
            step.lstOutputValues = [];
        }

        if (step.objInputVM != undefined) {
            angular.forEach(step.objInputVM.Elements, function (input, keyvalue) {
                if ($scope.CompileResult != undefined) {
                    angular.forEach($scope.CompileResult.ilstInputFields, function (item) {
                        if (item.istrName == input.dictAttributes.ID) {
                            input.ruleDataInfo = item;
                        }
                    });
                    $scope.SetCompilationResultForChild(input);

                }
                if (isUndoRedoEnable) {
                    $rootScope.PushItem(input, step.lstInputValues);
                }
                else {
                    step.lstInputValues.push(input);
                }
            });
        }

        if (step.objParameterVM != undefined) {
            angular.forEach(step.objParameterVM.Elements, function (parameter, keyvalue) {
                if ($scope.CompileResult != undefined) {
                    angular.forEach($scope.CompileResult.ilstParameters, function (item) {
                        if (item.istrName == parameter.dictAttributes.ID) {
                            parameter.ruleDataInfo = item;
                        }
                    });
                    $scope.SetCompilationResultForChild(parameter, parameter.ruleDataInfo);
                }

                if (parameter.dictAttributes.ParameterType == undefined || parameter.dictAttributes.ParameterType == "") {
                    if (!isLoading) {
                        if (parameter.ruleDataInfo) {
                            if (parameter.ruleDataInfo.ienmDirection == 0) {
                                parameter.dictAttributes.ParameterType = 'InputParameter';
                            }
                            else {
                                parameter.dictAttributes.ParameterType = 'OutputParameter';
                            }
                        }
                        else {
                            parameter.dictAttributes.ParameterType = 'InputParameter';
                        }
                    }
                }
                if (parameter.dictAttributes.ParameterType == 'InputParameter') {
                    if (isUndoRedoEnable) {
                        $rootScope.PushItem(parameter, step.lstInputValues);
                    }
                    else {
                        step.lstInputValues.push(parameter);
                    }
                }
                else if (parameter.dictAttributes.ParameterType == 'OutputParameter') {
                    if (isUndoRedoEnable) {
                        $rootScope.PushItem(parameter, step.lstOutputValues);
                    }
                    else {
                        step.lstOutputValues.push(parameter);
                    }
                }
            });
        }

        if (step.objOutputVM != undefined) {
            angular.forEach(step.objOutputVM.Elements, function (output, keyvalue) {
                if ($scope.CompileResult != undefined) {
                    angular.forEach($scope.CompileResult.ilstOutputFields, function (item) {
                        if (item.istrName == output.dictAttributes.ID) {
                            output.ruleDataInfo = item;
                        }
                    });
                    $scope.SetCompilationResultForChild(output, output.ruleDataInfo);
                }
                if (isUndoRedoEnable) {
                    $rootScope.PushItem(output, step.lstOutputValues);
                }
                else {
                    step.lstOutputValues.push(output);
                }
            });
        }
    };

    $scope.SetCompilationResultForChild = function (objField) {
        if (objField.ruleDataInfo == undefined) {
            $scope.InitRuleDataInfo(objField);
        }

        angular.forEach(objField.Elements, function (item) {
            angular.forEach(item.Elements, function (field) {
                var lstEntityFields = GetEntityFieldsFromRuleDataInfo(objField, $scope.objEntity);
                if (lstEntityFields != undefined) {
                    angular.forEach(lstEntityFields, function (ruleInfo) {
                        if (ruleInfo.istrName == field.dictAttributes.ID) {
                            field.ruleDataInfo = ruleInfo;
                        }
                    });
                }
                if (field.ruleDataInfo == undefined && objField.dictAttributes.sfwDataType == "Object") {
                    $scope.CheckAndUpdateExtraFields(objField, field, objField.ExtraFields);
                }

                $scope.SetCompilationResultForChild(field, field.ruleDataInfo);
            });
        });
    };

    $scope.CheckAndUpdateExtraFields = function (objFieldParent, field) {
        var blnFound = false;
        angular.forEach(objFieldParent.ExtraFields, function (item) {
            if (item.istrName == field.dictAttributes.ID) {
                blnFound = true;
            }
        });
        if (!blnFound) {
            if (objFieldParent.ExtraFields == undefined) {
                objFieldParent.ExtraFields = [];
            }
            objFieldParent.ExtraFields.push({ ilstEntityFields: [], istrDataType: field.dictAttributes.sfwDataType, istrEntity: '', istrName: field.dictAttributes.ID, istrObjectField: '', itypDataType: '' });
        }
    };

    $scope.InitRuleDataInfo = function (objField) {
        if (objField.ruleDataInfo == undefined) {
            if (objField.dictAttributes.ID == "return") {
                $.connection.hubScenarioModel.server.prepareEntityFiledInfo($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile).done(function (data) {
                    $scope.$apply(function () {
                        objField.ruleDataInfo = data;
                    });
                });


            }
            else if (objField.dictAttributes.isExtraEntityField != undefined && objField.dictAttributes.isExtraEntityField == "true") {
                objField.ruleDataInfo = { ilstEntityFields: [], istrEntity: objField.dictAttributes.istrEntity };
            }
        }
    };

    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = { Name: strNodeName, Value: '', dictAttributes: {}, Elements: [] };
        objParent.Elements.push(objItem);
        return objItem;
    };

    //#endregion

    //#region Test Selection Methods

    $scope.TestSelectionChange = function (step) {
        $scope.objEntityBasedScenario.SelectedStep = step;
        $scope.selectedTest = step;
        $scope.SelectedParameter = undefined;
        $scope.SelectedOutputParam = undefined;
        $scope.SelectedMethodParameter = undefined;
    };

    $scope.SetValue = function (step, isLoading) {
        $scope.InitializeData(step);
        if (step) {
            $scope.UpdateDisplayInputOutputList(step, isLoading);
        }
    };

    //#endregion

    //#region Menu options for Copy/Edit Test

    $scope.menuOptions = [
        ['Copy Test', function ($itemScope) {
            var newScope = $scope.$new();
            var newTestID = $scope.objEntityBasedScenario.SelectedStep.dictAttributes.ID + " - copy";
            newScope.objNewTest = { Name: 'test', value: '', dictAttributes: { ID: newTestID, Text: $scope.objEntityBasedScenario.SelectedStep.dictAttributes.Text, sfwEffectiveDate: '' }, Elements: [] };
            $scope.IsCopy = true;
            newScope.preCloseCallback = function () {
                $scope.IsCopy = false;
                $scope.IsEdit = false;
            };
            newScope.AddNewTest = function () {
                $scope.AddNewTest(newScope);
            };
            newScope.ValidateTestID = function () {
                return $scope.ValidateTestID(newScope);
            };
            newScope.dialog = $rootScope.showDialog(newScope, "Copy Test", "Scenario/views/AddCopyEditNewTest.html", { beforeClose: newScope.preCloseCallback });
            ComponentsPickers.init();

        }, function ($itemScope) {
            if ($scope.IsExcelBasedScenario) {
                return false;
            }
            return true;
        }], null,
        ['Edit Test', function ($itemScope) {
            var newScope = $scope.$new();
            var newTestID = $scope.objEntityBasedScenario.SelectedStep.dictAttributes.ID;
            newScope.objNewTest = { Name: 'test', value: '', dictAttributes: { ID: newTestID, Text: $scope.objEntityBasedScenario.SelectedStep.dictAttributes.Text, sfwEffectiveDate: $scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwEffectiveDate }, Elements: [] };
            newScope.objNewTest.dictAttributes.ID = newTestID;
            newScope.preCloseCallback = function () {
                $scope.IsCopy = false;
                $scope.IsEdit = false;
            };
            newScope.AddNewTest = function () {
                $scope.AddNewTest(newScope);
            };
            newScope.ValidateTestID = function () {
                return $scope.ValidateTestID(newScope);
            };
            $scope.IsEdit = true;
            newScope.dialog = $rootScope.showDialog(newScope, "Edit Test", "Scenario/views/AddCopyEditNewTest.html", { beforeClose: newScope.preCloseCallback });
            ComponentsPickers.init();

        }, function ($itemScope) {
            if ($scope.IsExcelBasedScenario) {
                return false;
            }
            return true;
        }], null
    ];
    //#endregion

    //#region Common Events

    $scope.openWindow = function () {
        var newScope = $scope.$new();
        newScope.EntityField = true;
        function filter(item) {
            return item.Name == "attributes";
        }
        newScope.lstEntityFields = [];
        function iterator(item) {
            item.Selected = false;
            newScope.lstEntityFields.push(item);
        }
        if ($scope.objEntity) {
            var attributes = $scope.objEntity.Elements.filter(filter);
            var strAttributes = JSON.stringify(attributes);
            attributes = JSON.parse(strAttributes);
            if (attributes && attributes.length > 0) {

                angular.forEach(attributes[0].Elements, iterator);
            }
        }

        newScope.unCheckAll = function () {

            angular.forEach(newScope.lstEntityFields, function (item) {
                item.Selected = false;
            });
        };

        newScope.checkAll = function () {
            angular.forEach(newScope.lstEntityFields, function (item) {
                item.Selected = true;
            });
        };

        newScope.onCancelClick = function () {
            newScope.dialog.close();
        };

        newScope.UpdateData = function () {
            $rootScope.UndRedoBulkOp("Start");
            angular.forEach(newScope.lstEntityFields, function (item) {
                if (item.Selected) {
                    var IsFound = false;
                    angular.forEach($scope.objEntityBasedScenario.SelectedStep.lstInputValues, function (objInput) {
                        if (objInput.dictAttributes.ID == item.dictAttributes.ID) {
                            IsFound = true;
                        }

                    });
                    if (!IsFound) {
                        var strDataType = item.dictAttributes.sfwDataType;
                        if (item.dictAttributes.sfwType == "Collection" || item.dictAttributes.sfwType == "Object" || item.dictAttributes.sfwType == "List" || item.dictAttributes.sfwType == "Description") {
                            strDataType = item.dictAttributes.sfwType;
                        }
                        var objItem = { Name: 'field', value: '', dictAttributes: { ParameterType: "Input", ID: item.dictAttributes.ID, sfwDataType: strDataType, isExtraEntityField: "true", istrEntity: item.dictAttributes.sfwEntity }, Elements: [] };
                        var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.sfwEntity };
                        objItem.ruleDataInfo = entityFieldInfo;
                        if ($scope.objEntityBasedScenario.SelectedStep.lstInputValues != undefined) {
                            $rootScope.PushItem(objItem, $scope.objEntityBasedScenario.SelectedStep.lstInputValues);
                            $rootScope.PushItem(objItem, $scope.objEntityBasedScenario.SelectedStep.objInputVM.Elements);
                        }
                    }
                }
            });
            $rootScope.UndRedoBulkOp("End");
            newScope.dialog.close();
        };
        newScope.searchEntityFields = "";
        newScope.dialog = $rootScope.showDialog(newScope, "Add Entity Fields", "Scenario/views/AddEntityFields.html", { height: 500 });
        //EntityFieldsDialogID = dialog.id;
    };

    $scope.DeleteInputValues = function () {

        if ($scope.SelectedParameter != undefined && $scope.SelectedParameter.dictAttributes.ParameterType == 'InputParameter') {
            alert('can not delete selected  field, value is mandatory for execution.');
        }
        else {
            var index = $scope.objEntityBasedScenario.SelectedStep.lstInputValues.indexOf($scope.SelectedParameter);
            if (index > -1) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.SelectedParameter, $scope.objEntityBasedScenario.SelectedStep.lstInputValues);
                var indexEntity = $scope.objEntityBasedScenario.SelectedStep.objInputVM.Elements.indexOf($scope.SelectedParameter);
                $rootScope.DeleteItem($scope.SelectedParameter, $scope.objEntityBasedScenario.SelectedStep.objInputVM.Elements);
                $scope.SelectedParameter = $scope.objEntityBasedScenario.SelectedStep.lstInputValues[0];
                $rootScope.UndRedoBulkOp("End");
            }
            else {
                alert('Select the item.');
            }
        }

    };

    $scope.DeleteOutputValues = function () {
        if ($scope.SelectedOutputParam != undefined) {
            if ($scope.SelectedOutputParam.dictAttributes.ParameterType == 'Output' && $scope.SelectedOutputParam.dictAttributes.ID == "return") {
                alert('Cannot delete return value.');
            }
            else if ($scope.SelectedOutputParam.dictAttributes.ParameterType == 'OutputParameter') {
                // alert('can not delete selected  field, value is mandatory for execution.');
            }
            else {
                var index = $scope.objEntityBasedScenario.SelectedStep.lstOutputValues.indexOf($scope.SelectedOutputParam);
                if (index > -1) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem($scope.SelectedOutputParam, $scope.objEntityBasedScenario.SelectedStep.lstOutputValues);
                    var indexEntity = $scope.objEntityBasedScenario.SelectedStep.objOutputVM.Elements.indexOf($scope.SelectedOutputParam);
                    $rootScope.DeleteItem($scope.SelectedOutputParam, $scope.objEntityBasedScenario.SelectedStep.objOutputVM.Elements);
                    $scope.SelectedOutputParam = $scope.objEntityBasedScenario.SelectedStep.lstOutputValues[0];
                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    alert('Select the item.');
                }
            }
        }
    };

    $scope.SelectedMethodParameter = undefined;
    $scope.setSelectedMethodParameters = function (selectedMethodParam) {
        $scope.SelectedMethodParameter = selectedMethodParam;
        $scope.SelectedOutputParam = undefined;
        $scope.SelectedParameter = undefined;
    };

    $scope.setSelectedParameters = function (selectedParam) {
        $scope.SelectedParameter = selectedParam;
        $scope.SelectedOutputParam = undefined;
        $scope.SelectedMethodParameter = undefined;
    };

    $scope.setSelectedOutputParam = function (objOutput) {
        $scope.SelectedOutputParam = objOutput;
        $scope.SelectedParameter = undefined;
        $scope.SelectedMethodParameter = undefined;
    };

    $scope.DeleteTest = function () {
        var index = $scope.objEntityBasedScenario.Elements.indexOf($scope.objEntityBasedScenario.SelectedStep);
        if (index > -1) {
            if (confirm("Test : '" + $scope.objEntityBasedScenario.SelectedStep.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.objEntityBasedScenario.SelectedStep, $scope.objEntityBasedScenario.Elements, "TestSelectionChange");
                var lsttestElement = $scope.objEntityBasedScenario.Elements.filter(function (item) {
                    return item.Name == 'test';
                });
                if (lsttestElement.length > 0) {
                    for (var i = index; ;) {
                        if (index == 0) {
                            if (i < $scope.objEntityBasedScenario.Elements.length) {
                                if ($scope.objEntityBasedScenario.Elements[i] && $scope.objEntityBasedScenario.Elements[i].Name == 'test') {
                                    $scope.objEntityBasedScenario.SelectedStep = $scope.objEntityBasedScenario.Elements[i];
                                    break;
                                }
                            }
                            else {
                                break;
                            }
                            i++;
                        }
                        else {
                            if (i >= 0) {
                                if ($scope.objEntityBasedScenario.Elements[i]) {
                                    if ($scope.objEntityBasedScenario.Elements[i].Name == 'test') {
                                        $scope.objEntityBasedScenario.SelectedStep = $scope.objEntityBasedScenario.Elements[i];
                                        break;
                                    }
                                }
                                // if last element is deleted
                                else {
                                    if ($scope.objEntityBasedScenario.Elements[i - 1].Name == 'test') {
                                        $scope.objEntityBasedScenario.SelectedStep = $scope.objEntityBasedScenario.Elements[i - 1];
                                        break;
                                    }
                                }
                            }
                            else {
                                break;
                            }
                            i--;
                        }
                    }
                }
                else {
                    $scope.objEntityBasedScenario.SelectedStep = undefined;
                }
                $rootScope.UndRedoBulkOp("End");
                $scope.SetValue($scope.objEntityBasedScenario.SelectedStep, false);
                var x = $scope.objEntityBasedScenario.Elements.indexOf($scope.objEntityBasedScenario.SelectedStep);

                //  $('#ul1 li a:eq("'+x+'")').addClass('active'); // adding class for aftr deleting
            }

        }
        else {
            alert('Select the item.');
        }
    };

    $scope.canDeleteTest = function () {
        if ($scope.objEntityBasedScenario != undefined && $scope.objEntityBasedScenario.Elements != undefined && $scope.objEntityBasedScenario.Elements.length > 0) {
            return $scope.objEntityBasedScenario.SelectedStep != undefined;
        }
        else {
            return false;
        }
    };

    $scope.OpenAddTest = function () {

        $scope.NewTest = true;
        var strItemKey = "NewTest";
        var iItemNum = 0;
        var strItemName = strItemKey;

        var newTemp = $scope.objEntityBasedScenario.Elements.filter(function (x) {
            return x.dictAttributes.ID == strItemName;
        });

        while (newTemp.length > 0) {
            iItemNum++;
            strItemName = strItemKey + " (" + iItemNum + ")";
            newTemp = $scope.objEntityBasedScenario.Elements.filter(function (x) {
                return x.dictAttributes.ID == strItemName;
            });
        }
        var newScope = $scope.$new();
        newScope.objNewTest = { Name: 'test', value: '', dictAttributes: { ID: strItemName, Text: '', sfwEffectiveDate: '' }, Elements: [] };
        newScope.preCloseCallback = function () {
            $scope.IsCopy = false;
            $scope.IsEdit = false;
        };
        newScope.AddNewTest = function () {
            $scope.AddNewTest(newScope);
        };
        newScope.ValidateTestID = function () {
            return $scope.ValidateTestID(newScope);
        };
        newScope.dialog = $rootScope.showDialog(newScope, "New Test", "Scenario/views/AddCopyEditNewTest.html", { beforeClose: newScope.preCloseCallback });
        ComponentsPickers.init();

    };

    $scope.AddNewTest = function (newScope) {
        if ($scope.IsCopy) {
            var data = JSON.stringify($scope.objEntityBasedScenario.SelectedStep);
            var objData = JSON.parse(data);
            objData.dictAttributes.ID = newScope.objNewTest.dictAttributes.ID;
            objData.dictAttributes.Text = newScope.objNewTest.dictAttributes.Text;
            objData.dictAttributes.sfwEffectiveDate = newScope.objNewTest.dictAttributes.sfwEffectiveDate;

            var objcopyTest = { Name: 'test', value: '', dictAttributes: objData.dictAttributes, Elements: objData.Elements };
            $rootScope.PushItem(objcopyTest, $scope.objEntityBasedScenario.Elements, "TestSelectionChange");
            $scope.objEntityBasedScenario.SelectedStep = objcopyTest;
            $scope.SetValue(objcopyTest, false);
            $scope.IsCopy = false;
        }
        else if ($scope.IsEdit) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.EditPropertyValue($scope.objEntityBasedScenario.SelectedStep.dictAttributes.ID, $scope.objEntityBasedScenario.SelectedStep.dictAttributes, "ID", newScope.objNewTest.dictAttributes.ID);
            $rootScope.EditPropertyValue($scope.objEntityBasedScenario.SelectedStep.dictAttributes.Text, $scope.objEntityBasedScenario.SelectedStep.dictAttributes, "Text", newScope.objNewTest.dictAttributes.Text);
            $rootScope.EditPropertyValue($scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwEffectiveDate, $scope.objEntityBasedScenario.SelectedStep.dictAttributes, "sfwEffectiveDate", newScope.objNewTest.dictAttributes.sfwEffectiveDate);
            $scope.IsEdit = false;

            $rootScope.UndRedoBulkOp("End");
        }

        else {
            $rootScope.PushItem(newScope.objNewTest, $scope.objEntityBasedScenario.Elements, "TestSelectionChange");

            $scope.objEntityBasedScenario.SelectedStep = newScope.objNewTest;
            $scope.SetValue(newScope.objNewTest, false);
            $scope.LoadInputOutputValues(newScope.objNewTest);
        }
        newScope.dialog.close();
    };

    $scope.ValidateTestID = function (newScope) {
        var retVal = false;
        newScope.ErrorMessageForDisplay = "";
        if (newScope.objNewTest.dictAttributes.ID == undefined || newScope.objNewTest.dictAttributes.ID == "") {
            newScope.ErrorMessageForDisplay = "Enter the Test ID.";
            retVal = true;
        }
        else {
            if ($scope.IsEdit) {
                if ($scope.CheckForDuplicateTestID(newScope.objNewTest.dictAttributes.ID, newScope.objNewTest)) {
                    newScope.ErrorMessageForDisplay = "Duplicate Test ID.";
                    retVal = true;
                }
            }
            else {
                if ($scope.CheckForDuplicateTestID(newScope.objNewTest.dictAttributes.ID)) {
                    newScope.ErrorMessageForDisplay = "Duplicate Test ID.";
                    retVal = true;
                }
            }
        }
        return retVal;
    };

    $scope.CheckForDuplicateTestID = function (strId, objEditedTest) {
        var blnReturn = false;
        function iterator(item) {
            if (!blnReturn) {
                if (objEditedTest) {
                    if (item.dictAttributes.ID == strId && item.dictAttributes.ID != $scope.objEntityBasedScenario.SelectedStep.dictAttributes.ID) {
                        blnReturn = true;
                    }
                }
                else if (item.dictAttributes.ID == strId && item.dictAttributes.ID) {
                    blnReturn = true;
                }
            }
        }

        if ($scope.objEntityBasedScenario) {
            angular.forEach($scope.objEntityBasedScenario.Elements, iterator);

        }
        return blnReturn;
    };

    $scope.UpdateMethodParameter = function () {
        $scope.SelectedMethodParameter = undefined;
        $rootScope.UndRedoBulkOp("Start");
        $scope.ClearMethodParameters();
        if ($scope.objEntityBasedScenario.SelectedStep) {

            var objMethod;
            var lst = $scope.lstXmlMethods.filter(function (item) { return item.ID == $scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwXmlMethod; });
            if (lst && lst.length > 0) {
                objMethod = lst[0];
            }
            if (objMethod != undefined && objMethod.Parameters != undefined) {
                angular.forEach(objMethod.Parameters, function (methodParam) {
                    var objParam = { Name: 'parameter', Value: '', dictAttributes: { ID: methodParam.ID, sfwDataType: methodParam.DataType }, Elements: [] };
                    $rootScope.PushItem(objParam, $scope.objEntityBasedScenario.SelectedStep.objParameters.Elements);
                });
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.ClearMethodParameters = function () {
        while ($scope.objEntityBasedScenario.SelectedStep.objParameters.Elements.length > 0) {
            $rootScope.DeleteItem($scope.objEntityBasedScenario.SelectedStep.objParameters.Elements[0], $scope.objEntityBasedScenario.SelectedStep.objParameters.Elements);
        }
    };

    $scope.TestFilter = function (opt) {
        $scope.selectedTestFilter = opt;
        $scope.selectedTest = undefined;
        $scope.objEntityBasedScenario.SelectedStep = undefined;
        if (opt == "All") {
            angular.forEach($scope.objEntityBasedScenario.Elements, function (item) {
                if (item.TestFilter != undefined && item.Name != 'ExtraFields') {
                    item.TestFilter = true;
                    if (!$scope.selectedTest) {
                        $scope.selectedTest = item;
                        $scope.objEntityBasedScenario.SelectedStep = item;
                    }
                }
            });
        }
        if (opt == "Pass") {
            angular.forEach($scope.objEntityBasedScenario.Elements, function (item) {
                if (item.TestFilter != undefined && item.Name != 'ExtraFields') {
                    if (item.Status && item.IsRunSelected) {
                        item.TestFilter = true;
                        if (!$scope.selectedTest) {
                            $scope.selectedTest = item;
                            $scope.objEntityBasedScenario.SelectedStep = item;
                        }
                    }
                    else {
                        item.TestFilter = false;
                    }
                }
            });
        }
        if (opt == "Fail") {
            angular.forEach($scope.objEntityBasedScenario.Elements, function (item) {
                if (item.TestFilter != undefined && item.Name != 'ExtraFields') {
                    if (!item.Status && item.IsRunSelected) {
                        item.TestFilter = true;
                        if (!$scope.selectedTest) {
                            $scope.selectedTest = item;
                            $scope.objEntityBasedScenario.SelectedStep = item;
                        }
                    }
                    else {
                        item.TestFilter = false;
                    }
                }
            });
        }
    };

    $scope.OnConstantValueChange = function (objField) {
        if (objField) {
            $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
        }
    };

    //#endregion

    //#region Methods For Open popups  for collection and Object

    $scope.OpenPopupForCollection = function (objField, isOutput) {
        $scope.SelectedTestField = objField;
        objField.objEntity = $scope.objEntity;
        $scope.SelectedTestField.IsOutput = isOutput;
        objField.lstCollectionColumn = [];

        if (objField.ruleDataInfo != undefined) {
            var lstEntityFields = GetEntityFieldsFromRuleDataInfo(objField, $scope.objEntity);
            if (lstEntityFields != undefined) {
                angular.forEach(lstEntityFields, function (childParameter) {
                    if ($scope.SelectedTestField.Elements.length > 0) {

                        var lst = $scope.SelectedTestField.Elements[0].Elements.filter(function (itm) {
                            return itm.dictAttributes.ID == childParameter.istrName;
                        });
                        if (lst && lst.length > 0) {
                            if (!objField.lstCollectionColumn.some(function (itm) { return itm.istrName == childParameter.istrName })) {
                                objField.lstCollectionColumn.push(childParameter);
                            }
                        }
                    }
                    else {
                        objField.lstCollectionColumn.push(childParameter);
                    }
                });
            }
            if ($scope.SelectedTestField.Elements.length > 0) {
                angular.forEach($scope.SelectedTestField.Elements[0].Elements, function (aobjFieldElement) {

                    if (aobjFieldElement.dictAttributes.isExtraEntityField && aobjFieldElement.dictAttributes.isExtraEntityField.toLowerCase() == "true") {
                        if (!objField.lstCollectionColumn.some(function (itm) { return itm.istrName == aobjFieldElement.dictAttributes.ID })) {

                            var aobjExtraField = { iblnImportant: false, ilstEntityFields: aobjFieldElement.ruleDataInfo.ilstEntityFields, istrDataType: aobjFieldElement.dictAttributes.sfwDataType, istrName: aobjFieldElement.dictAttributes.ID };
                            objField.lstCollectionColumn.push(aobjExtraField);
                        }
                    }
                });
            }

            if (isOutput) {
                var objcol = { istrName: "Index" };
                objField.lstCollectionColumn.splice(0, 0, objcol);
            }
        }


        if (isOutput) {
            var objcol = { istrName: "Ignore" };
            objField.lstCollectionColumn.push(objcol);

            if ($scope.objEntityBasedScenario.SelectedStep.IsRunSelected) {
                var objcol = { istrName: "Result" };
                objField.lstCollectionColumn.push(objcol);
            }
        }

        var newScope = $scope.$new();
        newScope.SelectedTestField = objField;
        newScope.objselectedstep = $scope.objEntityBasedScenario.SelectedStep;
        newScope.SelectedTestField.objentity = $scope.objEntity;
        newScope.IsExcelBasedScenario = $scope.IsExcelBasedScenario;
        newScope.SelectedTestField.ExtraFields = [];

        newScope.AddExtraFields = function (SelectedTestField) {
            if (SelectedTestField.Elements.length > 0) {
                angular.forEach(SelectedTestField.Elements[0].Elements, function (item) {
                    if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                        var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        if (SelectedTestField.ExtraFields == undefined) {
                            SelectedTestField.ExtraFields = [];
                        }
                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);
                    }
                });
            }
        };
        newScope.AddExtraFields(newScope.SelectedTestField);

        newScope.AddColumnForCollection = function () {
            AddColumnForCollection(newScope.SelectedTestField, newScope, $scope, $rootScope);
        };
        newScope.DeleteCollectionField = function () {
            DeleteCollectionField(newScope.SelectedTestField, $rootScope);
        };
        newScope.UpdateSelectedCollectionField = function (objFields) {
            UpdateSelectedCollectionField(objFields, newScope.SelectedTestField);
        };
        newScope.AddCollectionField = function () {
            AddCollectionField(newScope.SelectedTestField, $scope.objEntity, $rootScope);
        };
        newScope.onCancelClick = function () {
            newScope.collectionDialog.close();
        };

        newScope.menuOptionForCollection = [
            ['Delete Column', function ($itemScope) {
                DeleteColumnFromCollection(newScope.SelectedTestField, $itemScope.objcol);

            }], null,
        ];

        newScope.collectionDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenCollectionPopup.html", { height: 400 });
    };

    $scope.OpenPopupForObject = function (objField) {
        $scope.SelectedTestField = objField;
        $scope.SelectedTestField.objEntity = $scope.objEntity;
        $scope.SelectedTestField.IsOutput = false;

        angular.forEach($scope.SelectedTestField.Elements, function (item) {
            if (item.Name == "fields") {
                $scope.SelectedTestField.objFieldsVM = item;
                return;
            }
        });
        if ($scope.SelectedTestField.objFieldsVM != undefined) {
            angular.forEach($scope.SelectedTestField.Elements, function (item) {
                if (item.Name == "field") {
                    $scope.SelectedTestField.objFieldsVM.SelectedField = item;
                    return;
                }
            });
        }

        var newScope = $scope.$new();
        newScope.SelectedTestField = $scope.SelectedTestField;
        newScope.SelectedTestField.ExtraFields = [];

        newScope.AddExtraFields = function (SelectedTestField) {
            if (SelectedTestField.Elements.length > 0) {
                angular.forEach(SelectedTestField.Elements[0].Elements, function (item) {
                    if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                        var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }

                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);
                    }
                });
            }
        };
        newScope.AddExtraFields(newScope.SelectedTestField);

        newScope.UpdateSelectedObjectField = function (objFields) {
            UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
        };
        newScope.DeleteObjectField = function () {
            DeleteObjectField(newScope.SelectedTestField, $rootScope);
        };
        newScope.AddColumnForCollection = function () {
            AddColumnForCollection(newScope.SelectedTestField, newScope, $scope, $rootScope);
        };

        newScope.OnConstantValueChange = function (objField) {
            if (objField) {
                $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
            }
        };

        newScope.objectInputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopup.html", { height: 500 });
    };

    $scope.OpenPopupForOutputObject = function (objField, objselectedstep) {
        $scope.SelectedOutputTestField = objField;
        $scope.SelectedOutputTestField.objEntity = $scope.objEntity;
        $scope.SelectedOutputTestField.IsOutput = true;

        angular.forEach($scope.SelectedOutputTestField.Elements, function (item) {
            if (item.Name == "fields") {
                $scope.SelectedOutputTestField.objFieldsVM = item;
                return;
            }
        });
        if ($scope.SelectedOutputTestField.objFieldsVM != undefined) {
            angular.forEach($scope.SelectedOutputTestField.Elements, function (item) {
                if (item.Name == "field") {
                    $scope.SelectedOutputTestField.objFieldsVM.SelectedField = item;
                    return;
                }
            });
        }

        var newScope = $scope.$new();
        newScope.SelectedTestField = $scope.SelectedOutputTestField;
        newScope.objselectedstep = objselectedstep;
        newScope.UpdateSelectedObjectField = function (objFields) {
            UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
        };
        newScope.DeleteObjectField = function () {
            DeleteObjectField(newScope.SelectedTestField, $rootScope);
        };
        newScope.AddColumnForCollection = function () {
            AddColumnForCollection(newScope.SelectedTestField, newScope, $scope, $rootScope);
        };
        newScope.OnConstantValueChange = function (objField) {
            if (objField) {
                $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
            }
        };
        newScope.objectOutputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopupForOutput.html", { height: 500 });

    };

    //#endregion Methods For Open popups  for collection and Object

    //#region Run Scenario Methods

    $scope.RunAllTests = function (objSelectedOption) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = true;
        });
        /* Bug 8792:In Scienario -On Clicking Run All Tests/Steps -Loading Is Displayed(added checked when no test is thr.).*/

        var lstTest = $scope.objEntityBasedScenario.Elements.filter(function (x) { return x.Name == "test" });
        if (lstTest && lstTest.length > 0) {
            if ($scope.objEntityBasedScenario && $scope.objEntityBasedScenario.Elements && $scope.objEntityBasedScenario.Elements.length > 0) {
                $.connection.hubScenarioModel.server.createCompileResult($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile).done(function () {
                    angular.forEach($scope.objEntityBasedScenario.Elements, function (item) {
                        if (item.Name == "test") {
                            $scope.Runtest(item, objSelectedOption, true);
                        }
                    });
                    $.connection.hubScenarioModel.server.disposeCompileResult();
                });

            }
        }
        else {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = false;
            });

        }
    };


    $scope.GetLastStepID = function () {
        var stepID = "";
        var lstTest = $scope.objEntityBasedScenario.Elements.filter(function (x) { return x.Name === "test" });
        if (lstTest && lstTest.length > 0) {
            stepID = lstTest[0].dictAttributes.ID;
        }
        return stepID;
    }

    $scope.RunScenario = function (objSelectedOption) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = true;
        });
        $scope.Runtest($scope.objEntityBasedScenario.SelectedStep, objSelectedOption, false);
    };

    $scope.Runtest = function (objselectedtestStep, objSelectedOption, isRunAll) {
        if (objselectedtestStep) {

            var lastStepID = $scope.GetLastStepID();
            objselectedtestStep.IsRunSelected = false;
            objselectedtestStep.Status = true;
            objselectedtestStep.TestFilter = true;
            objselectedtestStep.IsViewdiagramVisible = false;
            objselectedtestStep.objSelectedLogicalRule = undefined;
            objselectedtestStep.strRunType = objSelectedOption;
            var lstInputAndOuputConstantValues = $scope.GetInputAndOuputConstantValues(objselectedtestStep);
            if ($scope.objEntityBasedScenario != undefined && objselectedtestStep != undefined) {
                var objreturn1 = GetBaseModel(objselectedtestStep);
                $.connection.hubScenarioModel.server.runScenario($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile, objreturn1, objSelectedOption, lstInputAndOuputConstantValues, $scope.IsObjectBasedScenario).done(function (lst) {
                    $scope.$evalAsync(function () {
                        //As we are returning a list having scenario result and rule data, then length of list mush be greater than 1.
                        if (lst && lst.length > 1) {
                            var objRunResult = lst[0];
                            $scope.objLogicalRule = lst[1];
                            if (objRunResult && objRunResult.StepId) {
                                objselectedtest = $scope.getSelectedStep(objRunResult.StepId);
                                objselectedtest.objScenarioRunResult = objRunResult;
                                objselectedtest.objScenarioRunResult.ScenarioRuleType = $scope.RuleType;
                                if (objselectedtest.objScenarioRunResult) {
                                    if (objselectedtest.objScenarioRunResult.ErrorMessage == undefined || objselectedtest.objScenarioRunResult.ErrorMessage == null) {
                                        if (objSelectedOption != "OnlyResult") {
                                            objselectedtest.IsViewdiagramVisible = true;
                                        }
                                        else {
                                            objselectedtest.IsViewdiagramVisible = false;
                                        }

                                        objselectedtest.IsRunSelected = true;
                                        if (objselectedtest.objScenarioRunResult.istrEffectiveDate != undefined) {
                                            objselectedtest.ExcecutedRuleEffectiveDate = objselectedtest.objScenarioRunResult.istrEffectiveDate;
                                        }
                                        else {
                                            objselectedtest.ExcecutedRuleEffectiveDate = "";
                                        }
                                        objselectedtest.TestResult = objselectedtest.objScenarioRunResult.TestResult;
                                        objselectedtest.ExecutionTime = objselectedtest.objScenarioRunResult.ExecutionTime;

                                        if ($scope.objLogicalRule != undefined) {
                                            objselectedtest.objSelectedLogicalRule = GetClosestLogicalRule($scope.objLogicalRule, objselectedtest.objScenarioRunResult.istrEffectiveDate, objselectedtest.objScenarioRunResult.ScenarioRuleType);
                                            if (objselectedtest.objSelectedLogicalRule != undefined) {
                                                UpdateTestExecutionFlow(objselectedtest.objScenarioRunResult.ilstRuleSteps, objselectedtest.objScenarioRunResult.ScenarioRuleType, objselectedtest.objSelectedLogicalRule, objSelectedOption);
                                            }
                                        }


                                        if (objselectedtest.objScenarioRunResult.ScenarioRuleType == "ExcelMatrix" && !$scope.IsObjectBasedScenario) {
                                            if (objselectedtest != undefined && objselectedtest.objOutputVM != undefined) {
                                                var returnParmVM;
                                                angular.forEach(objselectedtest.objOutputVM.Elements, function (item) {
                                                    if (item.dictAttributes.ID == "return") {
                                                        returnParmVM = item;
                                                    }
                                                });

                                                if (returnParmVM != undefined && objselectedtest.objScenarioRunResult.iobjReturnValue) {
                                                    returnParmVM.ResultValue = objselectedtest.objScenarioRunResult.iobjReturnValue.Value;
                                                    var blnIsIgnore = false;
                                                    if (returnParmVM.dictAttributes.sfwIgnore != undefined) {
                                                        blnIsIgnore = returnParmVM.dictAttributes.sfwIgnore.toLowerCase() == "true";
                                                    }
                                                    if (!blnIsIgnore) {
                                                        returnParmVM.Status = objselectedtest.objScenarioRunResult.iobjReturnValue.Status;
                                                        objselectedtest.Status = returnParmVM.Status;
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            if ($scope.IsObjectBasedScenario) {
                                                $scope.UpdateOutputParameters(objselectedtest, objselectedtest.objScenarioRunResult.outputParameters);
                                            }
                                            else {
                                                $scope.UpdateActualValues(objselectedtest, lstInputAndOuputConstantValues);
                                            }
                                        }

                                        objselectedtest.TestExecutionResults = [];

                                        objselectedtest.TestExecutionResults = $scope.GetScenarioExecutionResults(objselectedtest);
                                    }
                                }
                            }
                        }
                        if (!isRunAll) {
                            $rootScope.IsLoading = false;
                        }
                        else if (objselectedtestStep.dictAttributes.ID === lastStepID) {
                            $rootScope.IsLoading = false;
                        }
                    });
                });


            }
        }
    };

    $scope.getSelectedStep = function (StepId) {
        var objselectedtest;
        var tests = $scope.objEntityBasedScenario.Elements.filter(function (x) { return x.dictAttributes.ID == StepId; });
        if (tests && tests.length > 0) {
            objselectedtest = tests[0];
        }
        return objselectedtest;
    };

    $scope.GetInputAndOuputConstantValues = function (objselectedtest) {
        var lst = [];
        if (objselectedtest.objParameterVM != undefined) {
            $scope.PopulateConstantList(objselectedtest.objParameterVM.Elements, lst);
        }

        if (objselectedtest.objInputVM != undefined) {
            $scope.PopulateConstantList(objselectedtest.objInputVM.Elements, lst);
        }

        if (objselectedtest.objOutputVM != undefined) {
            $scope.PopulateConstantList(objselectedtest.objOutputVM.Elements, lst);
        }

        return lst;
    };

    $scope.PopulateConstantList = function (Elements, lst) {
        angular.forEach(Elements, function (parm) {
            if (parm) {
                if (parm.Name == "field" && parm.dictAttributes.sfwIsconstant) {
                    $scope.AddIntoConstantList(parm, lst);
                }

                if (parm.Elements && parm.Elements.length > 0) {
                    $scope.PopulateConstantList(parm.Elements, lst);
                }
            }
        });
    };

    $scope.AddIntoConstantList = function (parm, lst) {
        if (parm) {
            var value = $scope.GetConstantValue(parm.dictAttributes.Value);
            if (value != undefined && value.dictAttributes.Value != undefined) {
                var objclsInputOutputConstants = { strID: parm.dictAttributes.ID, strValue: value.dictAttributes.Value, strType: parm.dictAttributes.ParameterType, strDataType: parm.dictAttributes.sfwDataType, FullID: parm.dictAttributes.Value };
                lst.push(objclsInputOutputConstants);
            }
        }
    };

    $scope.GetConstantValue = function (strId) {
        var objReturn = null;

        if (strId != undefined && strId != null) {
            var lst = strId.split(".");
            var objConstants = $resourceFactory.getConstantModelData();

            if (lst !== undefined && lst.length != undefined) {
                for (var ind = 0; ind < lst.length; ind++) {
                    var val = lst[ind];
                    if (objReturn == null) {

                        if (objConstants != undefined) {
                            if (objConstants.dictAttributes.ID == val) {
                                objReturn = objConstants;
                            }
                        }
                    }
                    else {
                        objReturn = $scope.GetModelFromConstantsFile(objReturn, val);
                    }
                }
            }
        }
        return objReturn;
    };

    $scope.GetModelFromConstantsFile = function (objReturn, val) {
        var returnVal;
        for (var ind = 0; ind < objReturn.Elements.length; ind++) {

            if (objReturn.Elements[ind].dictAttributes.ID == val) {
                returnVal = objReturn.Elements[ind];
                break;
            }
        }
        return returnVal;
    };

    $scope.ViewDiagram = function () {
        var newScope = $scope.$new();
        $rootScope.IsLoading = true;
        $timeout(function () {
            newScope.IsRunResults = false;
            var strRuleID = "";
            newScope.objLogicalRule = $scope.objLogicalRule;
            newScope.objSelectedLogicalRule = JSON.parse(JSON.stringify($scope.objEntityBasedScenario.SelectedStep.objSelectedLogicalRule));
            newScope.testExecutionResults = $scope.objEntityBasedScenario.SelectedStep.TestExecutionResults;
            newScope.objEntity = $scope.objEntity;
            newScope.scenarioruntype = $scope.objEntityBasedScenario.SelectedStep.strRunType;
            if (newScope.objLogicalRule.RuleType == "DecisionTable") {
                strRuleID = $scope.objLogicalRule.RuleID;
                newScope.noConditionsDisplayed = 30;
                newScope.LazyLoadScenarioDecisionTable = function () {
                    if (newScope.objSelectedLogicalRule.Rows.length > newScope.noConditionsDisplayed) {
                        newScope.noConditionsDisplayed += 30;
                    }
                    else {
                        $interval.cancel(promise);
                    }
                }
                var promise = $interval(function () {
                    newScope.LazyLoadScenarioDecisionTable();
                }, 100);
            }
            else {
                strRuleID = $scope.objLogicalRule.dictAttributes.ID;
            }
            var EffectiveDate = ($scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwEffectiveDate == "" || $scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwEffectiveDate == undefined) ? 'Default' : !$scope.objEntityBasedScenario.SelectedStep.objScenarioRunResult.istrEffectiveDate ? "Default" : $scope.objEntityBasedScenario.SelectedStep.objScenarioRunResult.istrEffectiveDate;
            newScope.viewDiagramDialog = $rootScope.showDialog(newScope, "Rule : " + strRuleID + " - Effective Date :" + EffectiveDate + "[ Elapsed Time:" + $scope.objEntityBasedScenario.SelectedStep.objScenarioRunResult.ExecutionTime + "]", "Scenario/views/ViewRuleDiagram.html", { height: 700, width: 1400 });

            $timeout(function () {
                var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-expander");
                if (elements && elements.length > 0) {
                    for (var i = 0; i < elements.length > 0; i++) {
                        var scope = angular.element(elements[i]).scope();
                        if (scope && scope.model && !scope.model.IsStepSelected) {
                            collapseNode(elements[i], true);
                        }
                    }
                }
                $rootScope.IsLoading = false;
            });
        });
    };

    //#region Updating Execution Results

    $scope.GetScenarioExecutionResults = function (objselectedtest) {
        var lst = [];
        if (objselectedtest.objParameterVM != undefined) {
            angular.forEach(objselectedtest.objParameterVM.Elements, function (parm) {
                if (parm.Name == "field") {
                    var obj = {};
                    obj.Id = parm.dictAttributes.ID;

                    if (parm.ruleDataInfo != undefined) {
                        if (parm.ruleDataInfo.ienmDirection == "0") {
                            obj.InputValue = parm.dictAttributes.Value;
                            obj.Direction = "In";
                        }
                        else if (parm.ruleDataInfo.ienmDirection == "1") {
                            obj.ActualValue = parm.ResultValue;
                            obj.ExpectedValue = parm.dictAttributes.Value;
                            obj.Direction = "Out";
                            obj.Status = parm.Status;
                        }
                    }
                    else {
                        obj.InputValue = parm.dictAttributes.Value;
                        obj.Direction = "In";
                    }
                    obj.OutputType = "Parameter";
                    obj.DataType = parm.dictAttributes.sfwDataType;

                    lst.push(obj);
                }
            });
        }

        if (objselectedtest.objInputVM != undefined) {
            angular.forEach(objselectedtest.objInputVM.Elements, function (parm) {
                var obj = {};
                obj.Id = parm.dictAttributes.ID;
                obj.InputValue = parm.dictAttributes.Value;
                obj.OutputType = "Entity";
                obj.Direction = "In";
                obj.DataType = parm.dictAttributes.sfwDataType;

                lst.push(obj);
            });
        }

        if (objselectedtest.objOutputVM != undefined) {
            angular.forEach(objselectedtest.objOutputVM.Elements, function (parm) {
                if (parm.Name == "field") {
                    var fld;
                    angular.forEach(lst, function (item) {
                        if (item.Id == parm.dictAttributes.ID) {
                            fld = item;
                            return;
                        }
                    });
                    if (fld != undefined) {
                        fld.ExpectedValue = parm.dictAttributes.Value;
                        fld.ActualValue = parm.ResultValue;
                        fld.Status = parm.Status;
                    }
                    else {
                        var obj = {};
                        obj.Id = parm.dictAttributes.ID;
                        obj.ActualValue = parm.ResultValue;
                        obj.ExpectedValue = parm.dictAttributes.Value;
                        if (parm.dictAttributes.ID == "return") {
                            obj.OutputType = "Return";
                        }
                        else {
                            obj.OutputType = "Entity";
                        }
                        obj.Status = parm.Status;
                        obj.Direction = "Out";
                        obj.DataType = parm.dictAttributes.sfwDataType;

                        lst.push(obj);
                    }
                }
            });
        }

        return lst;
    };

    $scope.UpdateActualValues = function (objselectedtest, lstInputAndOuputConstantValues) {
        var testStatus = true;
        if (objselectedtest.objScenarioRunResult != undefined) {
            angular.forEach(objselectedtest.lstOutputValues, function (fieldVM) {
                fieldVM.IsRuleExecuted = true;
                if (fieldVM.dictAttributes.ParameterType == "OutputParameter") {
                    fieldVM.Status = $scope.UpdateParameterActualValues(fieldVM, objselectedtest.objScenarioRunResult.parms, objselectedtest.objScenarioRunResult, objselectedtest, lstInputAndOuputConstantValues, fieldVM);
                }
                else {
                    fieldVM.Status = $scope.UpdateResultParameterValues(fieldVM, objselectedtest.objScenarioRunResult.ruleEntity, objselectedtest.objScenarioRunResult, objselectedtest, lstInputAndOuputConstantValues, fieldVM);
                }

                testStatus = testStatus && fieldVM.Status;
            });
        }
        objselectedtest.Status = testStatus;
    };

    $scope.UpdateParameterActualValues = function (testParameterModel, parms, RuleResult, objselectedtest, lstInputAndOuputConstantValues, objParent) {
        var retVal = true;
        if (testParameterModel.dictAttributes.sfwDataType == "Collection" || testParameterModel.dictAttributes.sfwDataType == "CDOCollection" || testParameterModel.dictAttributes.sfwDataType == "List") {
            if (parms[testParameterModel.dictAttributes.ID] != undefined) {
                var objparms = parms[testParameterModel.dictAttributes.ID];
                if (objparms && objparms.lstData && objparms.lstData.length > 0) {
                    testParameterModel.Status = $scope.UpdateCollectionFieldActualValuesWithRuleEntity(testParameterModel, RuleResult, objparms.lstData, objselectedtest, lstInputAndOuputConstantValues, objParent);
                }
            }
        }
        else if (testParameterModel.dictAttributes.sfwDataType == "Object") {
            if (parms[testParameterModel.dictAttributes.ID] != undefined) {
                var obj = parms[testParameterModel.dictAttributes.ID];
                if (obj && obj.objData) {
                    var ruleEntity = obj.objData;
                    angular.forEach(testParameterModel.Elements, function (item) {
                        if (item.Name == "fields") {
                            testParameterModel.objFieldsVM = item;
                            return;
                        }
                    });
                    if (ruleEntity != undefined && testParameterModel.objFieldsVM != undefined) {
                        testParameterModel.Status = $scope.UpdateChildTestParameterActualValues(testParameterModel.objFieldsVM, ruleEntity, RuleResult, objselectedtest, lstInputAndOuputConstantValues, objParent);
                    }
                }
            }
        }
        else {
            testParameterModel.ResultValue = '';
            testParameterModel.Status = false;
            if (RuleResult.idictOutParameters[testParameterModel.dictAttributes.ID] != undefined) {
                var obj = RuleResult.idictOutParameters[testParameterModel.dictAttributes.ID];
                if (obj) {
                    testParameterModel.ResultValue = obj.Value;
                    testParameterModel.Status = obj.Status;
                }
                else {
                    testParameterModel.ResultValue = "";
                    testParameterModel.Status = false;
                }
            }
        }

        var blnIsIgnore = false;
        if (testParameterModel.dictAttributes.sfwIgnore != undefined) {
            blnIsIgnore = testParameterModel.dictAttributes.sfwIgnore.toLowerCase() == "true";
        }
        if (!blnIsIgnore) {
            retVal = retVal && testParameterModel.Status;
        }

        return retVal;
    };

    $scope.UpdateResultParameterValues = function (testParameterModel, response, ruleResult, objselectedtest, lstInputAndOuputConstantValues, objParent) {
        var testStatus = true;
        if (testParameterModel.dictAttributes.sfwDataType == "Collection" || testParameterModel.dictAttributes.sfwDataType == "CDOCollection" || testParameterModel.dictAttributes.sfwDataType == "List") {
            testParameterModel.Status = $scope.UpdateCollectionFieldActualValues(testParameterModel, response, ruleResult, objselectedtest, lstInputAndOuputConstantValues, objParent);
            var blnIsIgnore = false;
            if (testParameterModel.dictAttributes.sfwIgnore != undefined) {
                blnIsIgnore = testParameterModel.dictAttributes.sfwIgnore.toLowerCase() == "true";
            }
            if (!blnIsIgnore) {
                testStatus = testStatus && testParameterModel.Status;
            }
        }
        else if (testParameterModel.dictAttributes.sfwDataType == "Object") {
            testParameterModel.Status = $scope.UpdateObjectFieldActualValues(testParameterModel, response, ruleResult, objselectedtest, lstInputAndOuputConstantValues, objParent);
            var blnIsIgnore = false;
            if (testParameterModel.dictAttributes.sfwIgnore != undefined) {
                blnIsIgnore = testParameterModel.dictAttributes.sfwIgnore.toLowerCase() == "true";
            }
            if (!blnIsIgnore) {
                testStatus = testStatus && testParameterModel.Status;
            }
        }
        else {
            testParameterModel.ResultValue = '';
            var obj;
            if (testParameterModel.dictAttributes.ID == "return") {
                if (!ruleResult.iblnErrorOccurred) {
                    obj = ruleResult.iobjReturnValue;
                    if (obj) {
                        testParameterModel.ResultValue = obj.Value;
                    }
                }
                else {
                    obj = ruleResult.iobjReturnValue;
                }
            }
            else if (response && response.EntityFields && response.EntityFields[testParameterModel.dictAttributes.ID]) {
                obj = response.EntityFields[testParameterModel.dictAttributes.ID];
                if (obj) {
                    testParameterModel.ResultValue = obj.Value;
                }
            }

            var blnIsIgnore = false;
            if (testParameterModel.dictAttributes.sfwIgnore != undefined) {
                blnIsIgnore = testParameterModel.dictAttributes.sfwIgnore.toLowerCase() == "true";
            }
            if (!blnIsIgnore) {
                if (obj) {
                    testParameterModel.Status = obj.Status;
                }
                else {
                    testParameterModel.Status = false;
                }
                testStatus = testStatus && testParameterModel.Status;
            }
        }
        return testStatus;
    };

    $scope.UpdateObjectFieldActualValues = function (testParameterModel, response, ruleResult, objselectedtest, lstInputAndOuputConstantValues, objParent) {
        var objRuleEntity;
        var testStatus = true;
        if (testParameterModel.dictAttributes.ID == "return") {
            if (ruleResult) {
                var obj = ruleResult.iobjReturnValue;
                if (obj && obj.Value) {
                    objRuleEntity = obj.Value;
                }
            }
        }
        else {
            if (response && response.Entities && response.Entities[testParameterModel.dictAttributes.ID] != undefined) {
                objRuleEntity = response.Entities[testParameterModel.dictAttributes.ID];
            }
        }

        if (objRuleEntity != undefined) {
            angular.forEach(testParameterModel.Elements, function (item) {
                if (item.Name == "fields") {
                    testParameterModel.objFieldsVM = item;
                    return;
                }
            });

            if (testParameterModel.objFieldsVM != undefined) {
                testStatus = $scope.UpdateChildTestParameterActualValues(testParameterModel.objFieldsVM, objRuleEntity, ruleResult, objselectedtest, lstInputAndOuputConstantValues, objParent);
            }
            else {
                testStatus = false;
            }
        }
        return testStatus;
    };

    $scope.UpdateCollectionFieldActualValues = function (testParameterModel, response, RuleResult, objselectedtest, lstInputAndOuputConstantValues, objParent) {

        var lstData;
        if (testParameterModel.dictAttributes.ID == "return") {
            var obj = RuleResult.iobjReturnValue;
            if (obj && obj.Value) {
                lstData = obj.lstData;
            }
        }
        else {
            if (response && response.Collections && response.Collections[testParameterModel.dictAttributes.ID] != undefined) {
                lstData = response.Collections[testParameterModel.dictAttributes.ID];
            }
        }
        return $scope.UpdateCollectionFieldActualValuesWithRuleEntity(testParameterModel, RuleResult, lstData, objselectedtest, lstInputAndOuputConstantValues, objParent);
    };

    $scope.UpdateCollectionFieldActualValuesWithRuleEntity = function (testParameterModel, aRuleResult, lstData, objselectedtest, lstInputAndOuputConstantValues, objParent) {
        var retVal = true;
        if (lstData != undefined) {
            angular.forEach(testParameterModel.Elements, function (testParamsModel) {
                var objRuleEntity;
                if (testParamsModel.dictAttributes.indexNumber != undefined && testParamsModel.dictAttributes.indexNumber < lstData.length) {
                    objRuleEntity = lstData[testParamsModel.dictAttributes.indexNumber];
                }

                testParamsModel.IsRuleExecuted = true;
                testParamsModel.Status = $scope.UpdateChildTestParameterActualValues(testParamsModel, objRuleEntity, aRuleResult, objselectedtest, lstInputAndOuputConstantValues, testParameterModel);

                var blnIsIgnore = false;
                if (testParameterModel.dictAttributes.sfwIgnore != undefined) {
                    blnIsIgnore = testParameterModel.dictAttributes.sfwIgnore.toLowerCase() == "true";
                }
                if (!blnIsIgnore) {
                    retVal = retVal && testParamsModel.Status;
                }
            });
        }
        else {
            retVal = false;
        }
        return retVal;
    };

    $scope.UpdateChildTestParameterActualValues = function (testParameterModel, ruleEntity, RuleResult, objselectedtest, lstInputAndOuputConstantValues, objParent) {
        var testStatus = true;
        angular.forEach(testParameterModel.Elements, function (childParameter) {
            childParameter.IsRuleExecuted = true;
            childParameter.Status = $scope.UpdateResultParameterValues(childParameter, ruleEntity, RuleResult, objselectedtest, lstInputAndOuputConstantValues, objParent);
            testStatus = testStatus && childParameter.Status;
        });
        return testStatus;
    };

    $scope.findObject = function (list, paramId) {
        var testParameterModel;

        angular.forEach(list, function (item) {
            if (!testParameterModel) {
                if (item.dictAttributes.ID == paramId) {
                    testParameterModel = item;
                }
                else {
                    testParameterModel = $scope.findObject(item.Elements, paramId);
                }
            }
        });
        return testParameterModel;
    };

    //#region Upadate Flow for Object Based Scenario 

    $scope.UpdateOutputParameters = function (objselectedtest, aUpdatedOutpuParameters) {
        var testStatus = true;

        angular.forEach(objselectedtest.lstOutputValues, function (outParm) {
            var obj = aUpdatedOutpuParameters.filter(function (itm) { return itm.ID == outParm.dictAttributes.ID; });
            if (obj && obj.length) {
                outParm.ResultValue = obj[0].strActualValue;
                var blnIsIgnore = false;
                if (outParm.dictAttributes.sfwIgnore != undefined) {
                    blnIsIgnore = outParm.dictAttributes.sfwIgnore.toLowerCase() == "true";
                }
                if (!blnIsIgnore) {
                    outParm.Status = obj[0].Status;
                    testStatus = testStatus && outParm.Status;
                }
            }
            else {
                outParm.Status = false;
            }
        });

        objselectedtest.Status = testStatus;
    };

    //#endregion

    //#endregion Updating Execution Results

    //#region Update Execution Flow Methods
    $scope.ShowOnlyExecutedPath = function (ablnShowOnlyExecutedPath, objRule) {
        ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, objRule);
    };

    $scope.UpdateTestExecutionFlow = function (ilstRuleSteps, ruleType, objSelectedLogicalRule, strRunType) {
        if (ruleType == "LogicalRule") {
            CheckAndSelectStepForLogicalRule(ilstRuleSteps, objSelectedLogicalRule, strRunType);
        }
        else if (ruleType == "DecisionTable") {
            CheckAndSelectStepForDecisionTable(ilstRuleSteps, objSelectedLogicalRule);
        }
        else if (ruleType == "ExcelMatrix") {

            $scope.prepareExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);
        }
    };

    $scope.prepareExcelMatrix = function (ilstRuleSteps, objSelectedLogicalRule) {
        objSelectedLogicalRule.objColumnValues = [];
        objSelectedLogicalRule.objHeaderRowValues = [];
        objSelectedLogicalRule.objHeaderColumnValues = [];
        objSelectedLogicalRule.objHeaderRowTitle = "";
        objSelectedLogicalRule.objRows = [];

        var objExcelMatrixRule = objSelectedLogicalRule;
        for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
            if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
                var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

                for (var row = 0; row < objExcelMatrixRows.Elements.length; row++) {
                    if (objExcelMatrixRows.Elements[row].dictAttributes.ID == 0) {
                        var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;

                        if (objExcelMatrixRows.Elements[row].dictAttributes.Value != undefined) {
                            objSelectedLogicalRule.objHeaderRowTitle = objExcelMatrixRows.Elements[row].dictAttributes.Value;
                        }


                        var columns = [];
                        columns = strColumnValues.split(',');

                        objSelectedLogicalRule.objHeaderColumnValues.push(columns);
                    }
                    else {
                        var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;
                        var columns = [];
                        columns = strColumnValues.split(',');

                        objSelectedLogicalRule.objColumnValues.push(columns);
                        objSelectedLogicalRule.objHeaderRowValues.push(objExcelMatrixRows.Elements[row].dictAttributes.Value);
                    }
                }
            }
        }
        var index = CheckAndSelectStepForExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);

        for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
            if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
                var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

                for (var index = 0; index < objExcelMatrixRows.Elements.length; index++) {
                    if (objExcelMatrixRows.Elements[index].OtherData != undefined) {
                        var columnindex = -1;
                        angular.forEach(objSelectedLogicalRule.objHeaderColumnValues[0], function (item) {
                            if (item == objExcelMatrixRows.Elements[index].OtherData[1]) {
                                columnindex = objSelectedLogicalRule.objHeaderColumnValues[0].indexOf(item);
                                return;
                            }
                        });
                        objSelectedLogicalRule.objOtherData = [objExcelMatrixRows.Elements[index].OtherData[0], columnindex];
                    }
                }
            }
        }
    };

    //#endregion

    //#endregion

    //#region Methods for refresh Scenario

    $scope.RefreshScenario = function () {
        $rootScope.ClearUndoRedoListByFileName($scope.currentfile.FileName);
        $rootScope.IsLoading = true;
        if ($scope.IsObjectBasedScenario) {
            $scope.PopulateEntityRelatedDetail();
        }

        if ($scope.objEntityBasedScenario) {
            if ($scope.objEntityBasedScenario.Elements && $scope.objEntityBasedScenario.Elements.length > 0) {
                for (var i = 0; i < $scope.objEntityBasedScenario.Elements.length; i++) {
                    $scope.objEntityBasedScenario.Elements[i].IsViewdiagramVisible = false;
                }
            }
            $.connection.hubScenarioModel.server.refreshScenario($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile, $scope.IsObjectBasedScenario).done(function (data) {
                $scope.$apply(function () {
                    if (data && data.length == 2) {
                        $scope.CompileResult = data[0];
                        $scope.ruleFileInfo = data[1];
                        if ($scope.objEntityBasedScenario.Elements.length > 0) {
                            angular.forEach($scope.objEntityBasedScenario.Elements, function (step) {
                                if (step.Name == 'test') {
                                    $scope.LoadInputOutputValues(step);
                                    if ($scope.IsObjectBasedScenario) {
                                        $scope.RefreshXmlParameter(step);
                                    }
                                }
                            });
                        }
                    }
                    $rootScope.IsLoading = false;
                });
            });
        }
    };

    $scope.RefreshXmlParameter = function (step) {
        function iterateParameter(param) {
            var oldParams = dummyParameters.filter(function (oldParam) {
                return oldParam.dictAttributes.ID == param.ID;
            });
            var objParam = { Name: 'parameter', Value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType }, Elements: [] };
            if (oldParams && oldParams.length > 0) {
                if (oldParams[0].dictAttributes.Value != undefined && oldParams[0].dictAttributes.Value != "") {
                    objParam.dictAttributes.Value = oldParams[0].dictAttributes.Value;
                }
            }
            step.objParameters.Elements.push(objParam);
        }

        function iAddIndummyParameters(item) {
            dummyParameters.push(item);
        }
        if ($scope.objEntityBasedScenario.objExtraData && $scope.objEntityBasedScenario.objExtraData.entityModel) {
            var lstEntities = $EntityIntellisenseFactory.getEntityIntellisense();

            if (lstEntities && lstEntities.length > 0) {


                var dummyParameters = [];
                if (step.objParameters) {

                    angular.forEach(step.objParameters.Elements, iAddIndummyParameters);
                    step.objParameters.Elements = [];
                }

                if (!step.objParameters) {
                    step.objParameters = { Name: 'parameters', Value: '', dictAttributes: {}, Elements: [] };
                    step.Elements.push(step.objParameters);
                }
                if (step.objParameters) {
                    var entities = lstEntities.filter(function (item) {
                        return item.ID == $scope.objEntityBasedScenario.objExtraData.entityModel.dictAttributes.ID;
                    });
                    if (entities && entities.length > 0) {
                        var entity = entities[0];
                        if (entity.XmlMethods && entity.XmlMethods.length > 0) {

                            var lstMethod = entity.XmlMethods.filter(function (itm) {
                                return itm.ID == step.dictAttributes.sfwXmlMethod;
                            });
                            if (lstMethod && lstMethod.length > 0) {
                                var objMethod = lstMethod[0];
                                if (objMethod.Parameters && objMethod.Parameters.length > 0) {
                                    angular.forEach(objMethod.Parameters, iterateParameter);
                                }
                            }
                        }
                    }
                }
            }
        }
    };

    $scope.NavigateToRule = function () {
        if ($scope.ruleFileInfo != undefined) {
            $rootScope.openFile($scope.ruleFileInfo);
        }
    };

    $scope.LoadInputOutputValues = function (step) {
        $scope.SetDefaultSettings(step);

        step.inputflds = [];
        step.outputflds = [];

        if ($scope.CompileResult != undefined) {
            var lstInputs = [];
            var lstOutputs = [];

            if (!$scope.IsObjectBasedScenario) {
                lstInputs = $scope.CompileResult.ilstInputFields;
                lstOutputs = $scope.CompileResult.ilstOutputFields;
            }

            if (step.objInputVM != undefined) {
                step.inputflds = $scope.UpdateTestList(step, lstInputs, step.objInputVM.Elements, "Input");
            }
            if (step.objOutputVM != undefined) {
                step.outputflds = $scope.UpdateTestList(step, lstOutputs, step.objOutputVM.Elements, "Output");
            }
        }

        if (step.objOutputVM != undefined) {
            var returnParameter = $scope.CheckAndReLoadReturnParameter(step);
        }
    };

    $scope.SetDefaultSettings = function (step) {
        step.IsRunSelected = false;
        step.TestResult = "";
        step.Status = false;
    };

    $scope.UpdateTestListParameter = function (step, alst, aOldList) {
        var lstReturn = [];
        angular.forEach(alst, function (itm) {
            var parmType = itm.ienmDirection == "0" ? "InputParameter" : "OutputParameter";
            var objParameter;
            angular.forEach(aOldList, function (item) {
                if (item.dictAttributes.ID == itm.istrName && item.dictAttributes.ParameterType == parmType) {
                    objParameter = angular.copy(item);
                    objParameter.dictAttributes.ParameterType = parmType;
                    return;
                }
            });

            if (objParameter == undefined) {
                objParameter = { Name: "field", Value: '', dictAttributes: { ID: itm.istrName, sfwDataType: itm.istrDataType, ParameterType: parmType }, Elements: [] };
                objParameter.ruleDataInfo = itm;
                CheckAndAddObjectFields(objParameter, $scope.objEntity, $rootScope);
            }

            lstReturn.push(objParameter);

            if (itm.istrDataType == "Collection" || itm.istrDataType == "CDOCollection" || itm.istrDataType == "List" || itm.istrDataType == "Object") {
                if (itm.ilstInputFields.length > 0 && parmType != "InputParameter") {
                    var testinputParameterModel;
                    angular.forEach(aOldList, function (item) {
                        if (item.dictAttributes.ID == itm.istrName && item.dictAttributes.ParameterType != "OutputParameter") {
                            testinputParameterModel = angular.copy(item);
                            testinputParameterModel.dictAttributes.ParameterType = "InputParameter";
                            return;
                        }
                    });

                    if (testinputParameterModel == undefined) {
                        testinputParameterModel = { Name: "field", Value: '', dictAttributes: { ID: itm.istrName, sfwDataType: itm.istrDataType, ParameterType: "InputParameter" }, Elements: [] };
                        testinputParameterModel.ruleDataInfo = itm;
                        CheckAndAddObjectFields(testinputParameterModel, $scope.objEntity, $rootScope);
                    }

                    lstReturn.push(testinputParameterModel);
                }

                if (itm.ilstOutputFields.length > 0 && parmType != "OutputParameter") {
                    var testoutParameterModel;
                    angular.forEach(aOldList, function (item) {
                        if (item.dictAttributes.ID == itm.istrName && item.dictAttributes.ParameterType != "InputParameter") {
                            testoutParameterModel = angular.copy(item);
                            testoutParameterModel.dictAttributes.ParameterType = "OutputParameter";
                            return;
                        }
                    });

                    if (testoutParameterModel == undefined) {
                        testoutParameterModel = { Name: "field", Value: '', dictAttributes: { ID: itm.istrName, sfwDataType: itm.istrDataType, ParameterType: "OutputParameter" }, Elements: [] };
                        testoutParameterModel.ruleDataInfo = itm;
                        CheckAndAddObjectFields(testoutParameterModel, $scope.objEntity, $rootScope);
                    }

                    lstReturn.push(testoutParameterModel);
                }
            }
        });
        return lstReturn;
    };

    $scope.UpdateTestList = function (step, alst, aOldList, parametertype) {
        var lstReturn = [];

        angular.forEach(alst, function (itm) {
            var objParameter;
            angular.forEach(aOldList, function (item) {
                if (item.dictAttributes.ID == itm.istrName) {
                    objParameter = { Name: 'field', Value: '', dictAttributes: {}, Elements: [] };
                    ReloadTestParameterModel(objParameter, item, itm, $scope.objEntity, $rootScope);
                    //  objParameter = angular.copy(item);
                    objParameter.dictAttributes.ParameterType = parametertype;
                    return;
                }
            });
            if (objParameter == undefined) {
                objParameter = { Name: "field", Value: '', dictAttributes: { ID: itm.istrName, sfwDataType: itm.istrDataType, ParameterType: parametertype }, Elements: [] };
                objParameter.ruleDataInfo = itm;
                CheckAndAddObjectFields(objParameter, $scope.objEntity, $rootScope);
            }

            ////for extra Entity Fields
            if (objParameter && objParameter.dictAttributes.sfwDataType == "Object") {
                angular.forEach(aOldList, function (curParmModel) {

                    if (curParmModel.Name == "field") {
                        if (!$scope.IsObjectBasedScenario) {
                            if (curParmModel.dictAttributes.isExtraEntityField != undefined && curParmModel.dictAttributes.isExtraEntityField != "" && curParmModel.dictAttributes.isExtraEntityField.toLowerCase() == "true") {
                                var objExistingField;
                                angular.forEach(lstReturn, function (itm) {
                                    if (itm.dictAttributes.ID == curParmModel.dictAttributes.ID) {
                                        objExistingField = itm;
                                        return;
                                    }
                                });
                                if (objExistingField == undefined) {
                                    var testParameterModel = angular.copy(curParmModel);
                                    lstReturn.push(testParameterModel);
                                }
                            }
                        }
                        else {
                            if ($scope.CanProceedOpe(curParmModel.dictAttributes.ID)) {
                                var ruleDataInfo = { istrName: curParmModel.dictAttributes.ID, istrDataType: curParmModel.dictAttributes.sfwDataType };
                                var testParameterModel = angular.copy(curParmModel);
                                testParameterModel.ruleDataInfo = ruleDataInfo;
                                lstReturn.push(testParameterModel);
                            }
                        }
                    }
                });
            }


            lstReturn.push(objParameter);
        });


        return lstReturn;
    };

    $scope.CanProceedOpe = function (id) {
        var retVal = true;

        if (id == "return") {
            retVal = false;
        }
        else {
            if (null != $scope.CompileResult) {
                var lst = $scope.CompileResult.ilstInputFields.filter(function (itm) { return itm.istrName == id; });
                var lst1 = $scope.CompileResult.ilstOutputFields.filter(function (itm) { return itm.istrName == id; });
                if (lst && lst1) {
                    if (lst.length == 0 && lst1.length == 0) {
                        retVal = true;
                    }
                }
            }
        }
        return retVal;
    };

    $scope.CheckAndReLoadReturnParameter = function (teststep) {
        $.connection.hubScenarioModel.server.prepareEntityFiledInfo($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile).done(function (data) {
            $scope.$apply(function () {

                var testParameterModel;
                var entityFieldInfo = data;

                step = $scope.getSelectedStep(teststep.dictAttributes.ID);
                if (entityFieldInfo != null && entityFieldInfo != undefined) {
                    angular.forEach(step.objOutputVM.Elements, function (item) {
                        if (item.dictAttributes.ID == "return") {
                            testParameterModel = { Name: 'field', Value: '', dictAttributes: {}, Elements: [] };
                            ReloadTestParameterModel(testParameterModel, item, entityFieldInfo, $scope.objEntity, $rootScope);
                            testParameterModel.dictAttributes.ParameterType = "Output";
                            return;
                        }
                    });

                    if (testParameterModel == undefined) {
                        testParameterModel = { Name: "field", Value: '', dictAttributes: { ID: "return", ParameterType: "Output", sfwDataType: entityFieldInfo.istrDataType }, Elements: [] };
                        testParameterModel.ruleDataInfo = entityFieldInfo;
                        CheckAndAddObjectFields(testParameterModel, $scope.objEntity, $rootScope);
                    }
                    step.outputflds.push(testParameterModel);
                }

                $scope.UpdateTestListForAll(step);

            });
        });
    };

    $scope.UpdateTestListForAll = function (step) {
        step.paramflds = undefined;
        if ($scope.CompileResult != undefined && step.objParameterVM != undefined) {
            step.paramflds = $scope.UpdateTestListParameter(step, $scope.CompileResult.ilstParameters, step.objParameterVM.Elements);
        }

        //removing Existing  children
        $rootScope.UndRedoBulkOp("Start");
        if (step.objInputVM != undefined) {
            var inputElementsCount = step.objInputVM.Elements.length - 1;
            while (inputElementsCount > -1) {
                if (step.objInputVM.Elements[inputElementsCount] && step.objInputVM.Elements[inputElementsCount].dictAttributes.isExtraEntityField != "true") {
                    $rootScope.DeleteItem(step.objInputVM.Elements[inputElementsCount], step.objInputVM.Elements);
                }
                inputElementsCount--;
                //step.objInputVM.Elements.splice(0, 1);
            }
        }

        if (step.objOutputVM != undefined) {
            var outputElementsCount = step.objOutputVM.Elements.length - 1;
            while (outputElementsCount > -1) {
                if (step.objOutputVM.Elements[outputElementsCount] && step.objOutputVM.Elements[outputElementsCount].dictAttributes.isExtraEntityField != "true") {
                    $rootScope.DeleteItem(step.objOutputVM.Elements[outputElementsCount], step.objOutputVM.Elements);
                }
                outputElementsCount--;
                //step.objOutputVM.Elements.splice(0, 1);
            }
        }

        if (step.objParameterVM != undefined) {
            while (step.objParameterVM.Elements.length > 0) {
                $rootScope.DeleteItem(step.objParameterVM.Elements[0], step.objParameterVM.Elements);
                //step.objParameterVM.Elements.splice(0, 1);
            }
        }

        //Add New children

        if (step.inputflds != undefined) {
            if (step.objInputVM != undefined) {
                angular.forEach(step.inputflds, function (testParam) {
                    $rootScope.PushItem(testParam, step.objInputVM.Elements);
                    //step.objInputVM.Elements.push(testParam);
                });
            }
        }

        if (step.outputflds != undefined) {
            if (step.objOutputVM != undefined) {
                angular.forEach(step.outputflds, function (testParam) {
                    $rootScope.PushItem(testParam, step.objOutputVM.Elements);
                    //step.objOutputVM.Elements.push(testParam);
                });
            }
        }

        if (step.paramflds != undefined) {
            if (step.objParameterVM != undefined) {
                angular.forEach(step.paramflds, function (testParam) {
                    $rootScope.PushItem(testParam, step.objParameterVM.Elements);
                    //step.objParameterVM.Elements.push(testParam);
                });
            }
        }

        $scope.UpdateDisplayInputOutputList(step, false, true);
        $rootScope.UndRedoBulkOp("End");

    };

    //#endregion

    //#region Method  To Show Object Tree for Object Based Scenario

    $scope.onBusinessObjectPropertViewerOpenClick = function (isInputField) {
        var newScope = $scope.$new();
        newScope.IsInputField = isInputField;
        var strBusObject = $scope.objEntityBasedScenario.dictAttributes.BusinessObjectName;

        newScope.StrObjectTreeText = "";
        newScope.StrObjectTreePath = strBusObject;

        if (strBusObject != undefined && strBusObject != "") {
            $.connection.hubScenarioModel.server.loadObjectTree(strBusObject, true).done(function (data) {
                newScope.receiveObjectTree(data);
            });
        }

        newScope.receiveObjectTree = function (data) {
            newScope.$evalAsync(function () {
                newScope.ObjTreeForFields = data;
            });
        };

        newScope.onOkObjectTree = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (newScope.ObjTreeForFields != undefined) {
                var strId = "";
                var strDataType = "";
                var strObjectField = "";
                var strBusName = $scope.objEntityBasedScenario.dictAttributes.BusinessObjectName;
                var lst = newScope.ObjTreeForFields.ChildProperties.filter(function (x) { return x.IsChecked; });
                angular.forEach(lst, function (obj) {
                    strId = $scope.makeStringToCamelCase(obj.ShortName);
                    dataType = getDataType(obj.DataType, obj.DataTypeName);
                    var itemPath = "";
                    if (newScope.StrObjectTreePath != undefined && newScope.StrObjectTreePath != "") {
                        itemPath = newScope.StrObjectTreePath;
                    }

                    if ((itemPath != undefined && itemPath != "") && itemPath.contains($scope.objEntity.dictAttributes.sfwObjectID)) {
                        itemPath = itemPath.substring($scope.objEntity.dictAttributes.sfwObjectID.length + 1);
                    }
                    if (itemPath != undefined && itemPath != "") {
                        itemPath = itemPath + ".";
                    }
                    strObjectField = itemPath + obj.ShortName;

                    var entityFieldInfo = { istrName: strId, istrDataType: dataType };


                    if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                        entityFieldInfo.ilstEntityFields = [];
                    }
                    var strParamType = "Input";
                    if (!newScope.IsInputField) {
                        strParamType = "output";
                    }
                    var fieldModel = { Name: "field", Value: '', dictAttributes: { ID: strId, sfwDataType: dataType, ParameterType: strParamType }, Elements: [] };
                    CheckAndAddObjectFields(fieldModel, $scope.objEntity, $rootScope);

                    if (newScope.IsInputField) {
                        if ($scope.objEntityBasedScenario.SelectedStep.objInputVM) {
                            $rootScope.PushItem(fieldModel, $scope.objEntityBasedScenario.SelectedStep.objInputVM.Elements);
                            $rootScope.PushItem(fieldModel, $scope.objEntityBasedScenario.SelectedStep.lstInputValues);

                            //$scope.objEntityBasedScenario.SelectedStep.objInputVM.Elements.push(fieldModel);
                        }
                    }
                    else {
                        if ($scope.objEntityBasedScenario.SelectedStep.objOutputVM) {
                            $rootScope.PushItem(fieldModel, $scope.objEntityBasedScenario.SelectedStep.objOutputVM.Elements);
                            $rootScope.PushItem(fieldModel, $scope.objEntityBasedScenario.SelectedStep.lstOutputValues);
                            // $scope.objEntityBasedScenario.SelectedStep.objOutputVM.Elements.push(fieldModel);
                        }
                    }
                });

                // $scope.UpdateDisplayInputOutputList($scope.objEntityBasedScenario.SelectedStep, false);
            }
            $rootScope.UndRedoBulkOp("End");
            newScope.onCancelObjectTreeclick();
        };

        newScope.onCancelObjectTreeclick = function () {
            newScope.fieldsDialog.close();
        };

        newScope.onObjectTreeClick = function (obj, isQueryObjTree, isXmlMethodObjTree) {
            if (obj.DataType == "TableObjectType" || obj.DataType == "CollctionType" || obj.DataType == "BusObjectType" || obj.DataType == "OtherReferenceType") {

                if (newScope.StrObjectTreeText == undefined || newScope.StrObjectTreeText == "") {
                    newScope.StrObjectTreeText = newScope.objEntity.dictAttributes.sfwObjectID;
                    newScope.StrObjectTreePath = newScope.objEntity.dictAttributes.sfwObjectID + "." + obj.ShortName;
                }
                else {
                    newScope.StrObjectTreeText = newScope.StrObjectTreeText + "." + newScope.ObjTreeForFields.ItemType.Name;
                    newScope.StrObjectTreePath = newScope.StrObjectTreePath + "." + obj.ShortName;
                }
            }

            if (obj.ItemType.Name != undefined && obj.ItemType.Name != "") {
                $.connection.hubScenarioModel.server.loadObjectTree(obj.ItemType.Name, true).done(function (data) {
                    newScope.receiveObjectTree(data);
                });

            }
        };

        newScope.onToggleSelectionClick = function (isSelectAll) {
            if (newScope.ObjTreeForFields != undefined) {
                angular.forEach(newScope.ObjTreeForFields.ChildProperties, function (obj) {
                    if (obj.IsCheckboxVisible) {
                        obj.IsChecked = isSelectAll;
                    }
                });
            }
        };

        newScope.onLoadPreviosObjectTree = function (isQueryObjTree, isXmlMethodObjTree) {

            if (newScope.StrObjectTreeText != undefined || newScope.StrObjectTreeText != "") {
                if (newScope.StrObjectTreeText.contains('.')) {
                    var str = newScope.StrObjectTreeText.substring(newScope.StrObjectTreeText.lastIndexOf('.') + 1);
                    $.connection.hubScenarioModel.server.loadObjectTree(str, true).done(function (data) {
                        newScope.receiveObjectTree(data);
                    });
                    newScope.StrObjectTreeText = newScope.StrObjectTreeText.substring(0, newScope.StrObjectTreeText.lastIndexOf('.'));
                    newScope.StrObjectTreePath = newScope.StrObjectTreePath.substring(0, newScope.StrObjectTreePath.lastIndexOf('.'));
                }
                else {

                    $.connection.hubScenarioModel.server.loadObjectTree(newScope.StrObjectTreeText, true).done(function (data) {
                        newScope.receiveObjectTree(data);
                    });
                    newScope.StrObjectTreeText = undefined;
                    newScope.StrObjectTreePath = undefined;
                }
            }
        };

        newScope.fieldsDialog = $rootScope.showDialog(newScope, "Object Tree Fields", "Scenario/views/ScenarioObjectTree.html", { width: 550, height: 550 });
    };



    $scope.makeStringToCamelCase = function (astrFieldName) {

        var strCtrlID = "";
        var strSep = "~`!@#$%^&*_-+=[{()}]|:;<,.>?/.";

        var blnCapsNext = true;
        for (var i = 0; i < astrFieldName.length; i++) {
            if (strSep.contains("" + astrFieldName[i]))
                blnCapsNext = true;
            else {
                strCtrlID += blnCapsNext ? astrFieldName.toUpperCase()[i] : astrFieldName[i];
                if ("" + astrFieldName[i] == " ") {
                    blnCapsNext = true;
                }
                else {
                    blnCapsNext = false;
                }
            }
        }

        if (strCtrlID.toLowerCase().match("^iclb") || strCtrlID.toLowerCase().match("^icol") || strCtrlID.toLowerCase().match("^ibus"))
            strCtrlID = strCtrlID.substring(4);

        return strCtrlID;
    };

    //#endregion

    //#region Method  for Create Parameterised Scenario From Object Based Scenario

    $scope.onCreateParameterScenario = function () {
        if ($scope.objEntityBasedScenario.SelectedStep && $scope.objEntityBasedScenario.SelectedStep.IsRunSelected) {
            var fileName = "rsn" + $scope.objEntityBasedScenario.dictAttributes.ID.substring(3);
            var files = $rootScope.lstopenedfiles.filter(function (value, key) { return value.file.FileName == fileName; });
            if (files && files.length > 0) {
                alert("File " + fileName + " is already open. Close it for further updation .");
            }
            else {
                $rootScope.IsLoading = true;
                $.connection.hubScenarioModel.server.createParameterScenario($scope.objEntityBasedScenario.SelectedStep.objParameters, $scope.objEntityBasedScenario.dictAttributes.sfwRuleFile, $scope.objEntityBasedScenario.dictAttributes.RemoteObjectName, $scope.objEntityBasedScenario.dictAttributes.BusinessObjectName, $scope.objEntityBasedScenario.SelectedStep.dictAttributes.sfwXmlMethod).done(function (data) {
                    var objSceanrioDetail = data;
                    $rootScope.openFile(objSceanrioDetail);

                    var promise1 = $interval(function () {
                        if ($rootScope.lstopenedfiles.some(function (x) { return x.file && x.file.FileName == objSceanrioDetail.FileName; })) {
                            var scenarioScope = getScopeByFileName(objSceanrioDetail.FileName);
                            if (scenarioScope && scenarioScope.objEntityBasedScenario) {
                                scenarioScope.TestSelectionChange(scenarioScope.objEntityBasedScenario.Elements[scenarioScope.objEntityBasedScenario.Elements.length - 1]);
                                $interval.cancel(promise1);
                                if ($rootScope.IsLoading) {
                                    $rootScope.IsLoading = false;
                                }
                            }
                        }
                    }, 100);

                    if ($rootScope.lstopenedfiles.some(function (x) { return x.file && x.file.FileName == $scope.objEntityBasedScenario.dictAttributes.sfwRuleFile; })) {
                        var ruleScope = getScopeByFileName($scope.objEntityBasedScenario.dictAttributes.sfwRuleFile);
                        if (ruleScope) {
                            ruleScope.SetParameterScenario(data);
                        }
                    }
                });
            }
        }
    };


    //#endregion

    //#region Common functions

    $scope.scrolldownfunction = function (objSelectedLogicalRule) {
        var index = objSelectedLogicalRule.objExcelColumnValues.indexOf(objSelectedLogicalRule.objColumnValues[objSelectedLogicalRule.objColumnValues.length - 1]);
        var count = 0;
        for (var i = index + 1; i < objSelectedLogicalRule.objExcelColumnValues.length; i++) {
            if (count == 5) {
                break;
            }
            else {
                objSelectedLogicalRule.objColumnValues.push(objSelectedLogicalRule.objExcelColumnValues[i]);
            }
            count++;

        }
        //deleting rows
        for (var j = 0; j < 5; j++) {
            if (objSelectedLogicalRule.objColumnValues.length > 25) {
                objSelectedLogicalRule.objColumnValues.splice(0, 1);
            }
        }
    };

    $scope.scrollupfunction = function (objSelectedLogicalRule) {
        var index = objSelectedLogicalRule.objExcelColumnValues.indexOf(objSelectedLogicalRule.objColumnValues[0]);
        var count = 0;
        if (index != 0) {
            index = index - 1;
            for (var i = index; i >= 0; i--) {
                if (count == 5) {
                    break;
                }
                else {
                    objSelectedLogicalRule.objColumnValues.splice(0, 0, objSelectedLogicalRule.objExcelColumnValues[i]);
                }
                count++;
            }
            //deleting rows
            var count = 0;
            for (var j = objSelectedLogicalRule.objColumnValues.length - 1; j > 0; j--) {
                if (count == 5) {
                    break;
                }
                else {
                    if (objSelectedLogicalRule.objColumnValues.length > 25) {
                        objSelectedLogicalRule.objColumnValues.splice(j, 1);
                    }
                }
                count++;
            }
        }
    };

    $scope.ExpandCollapseDiagram = function () {
        $scope.IsDiagramDivExpanded = !$scope.IsDiagramDivExpanded;
    };

    $scope.render = function (condition) {
        if (condition) {
            $('.run-result-spliter').width('100%').height('530px').split({ orientation: 'vertical', limit: 20 });
        }

    };

    //#endregion

    //#region scenario details 
    $scope.onDetailClick = function () {
        var newScope = $scope.$new();
        //Extra Field varibles
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Scenario";

        newScope.onOKClick = function () {

            // #region extra field data
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }

            //#endregion
            //close the dialog
            newScope.closeDialog();
        };

        newScope.validateDetails = function () {
            newScope.DetailsErrorMessage = undefined;
            var flag = false;

            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var extraValueFlag = validateExtraFields(newScope);
            if (extraValueFlag) {
                newScope.DetailsErrorMessage = newScope.FormDetailsErrorMessage;
                return true;
            }

            if (newScope.DetailsErrorMessage != undefined) {
                return true;
            }

            return false;
        };

        newScope.closeDialog = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };

        newScope.dialog = $rootScope.showDialog(newScope, "Details", "Scenario/views/ScenarioDetails.html", { height: 500, width: 500 });
    };
    //#endregion
    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objScenarioExtraFields) {
    //        var index = $scope.objEntityBasedScenario.Elements.indexOf($scope.objScenarioExtraFields);
    //        if (index > -1) {
    //            $scope.objEntityBasedScenario.Elements.splice(index, 1);
    //        }
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objScenarioExtraFields) {
            var index = $scope.objEntityBasedScenario.Elements.indexOf($scope.objScenarioExtraFields);
            if (index == -1) {
                $scope.objEntityBasedScenario.Elements.push($scope.objScenarioExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };


    ////#region Key Up & Down

    //$scope.KeyUp = function (ruleType) {
    //    if (ruleType && ruleType == 'Scenario' && $scope.objEntityBasedScenario.SelectedStep) {
    //        var tempObj = keyUpAction($scope.objEntityBasedScenario.SelectedStep, $scope.objEntityBasedScenario);
    //        if (tempObj) {
    //            $scope.TestSelectionChange(tempObj);

    //        }
    //        else {

    //            return true;
    //        }
    //    }


    //}


    //$scope.KeyDown = function (ruleType) {
    //    var lstScenario;
    //    if (ruleType && ruleType == 'Scenario' && $scope.objEntityBasedScenario.SelectedStep) {
    //        var objScenario = { Name: 'testscenario', Value: '', dictAttributes: {}, Elements: [] };


    //        lstlstScenario = $scope.objEntityBasedScenario.Elements.filter(function (x) { return x.Name == 'test' });
    //        if (lstlstScenario && lstlstScenario.length > 0) {
    //            angular.forEach(lstlstScenario, function (itm) {
    //                objScenario.Elements.push(itm);
    //            });

    //        }

    //        var tempObj = keyDownAction($scope.objEntityBasedScenario.SelectedStep, objScenario);
    //        if (tempObj) {
    //            $scope.TestSelectionChange(tempObj);
    //        }
    //        else {

    //            return true;
    //        }


    //    }



    //}
    ////#endregion


    //#region Navigate to Entity

    $scope.selectXmlMethodClick = function (aXmlMethodID) {
        if (aXmlMethodID && aXmlMethodID != "" && aXmlMethodID != "NewObject") {
            $NavigateToFileService.NavigateToFile($scope.objEntity.dictAttributes.ID, "methods", aXmlMethodID);
        }
    };
    //#endregion
}]);
//#region directives for Collection and object popup

app.directive("collactionrowdirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var getTemplate = function (modelobject, objselectedstep) {
        if (modelobject.IsOutput) {

            var template = '<td > <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm min-width-80" title="{{items.dictAttributes.indexNumber}}" undoredodirective model="items" ng-model="items.dictAttributes.indexNumber"></td>';
            template += '<td valign="top" ng-repeat="item in items.Elements" ng-model="SelectedTestField">';
            template += '  <table><tr><td> <span ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"> Expected Value :</span></td>';
            template += ' <td>  <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm min-width-80" title="{{item.dictAttributes.Value}}" ng-model="item.dictAttributes.Value" undoredodirective model="item" ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"/> </td>';
            template += '  <td> <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopupForCollection(item)" ng-show="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\'"/></a> </td>';
            template += '  <td> <a><img src="images/Common/icon_add_dark.svg"  ng-click="OpenPopupForObject(item)" ng-show="item.dictAttributes.sfwDataType == \'Object\'"/></a></td></tr> ';
            if (objselectedstep.IsRunSelected) {
                template += '<tr ng-hide="(item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\') "><td> <div > Actual Value :</div></td><td><span>{{item.ResultValue}}</span></td></tr>';
            }

            template += '<tr ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'">  <td> <span>Ignore Field :</span></td> <td><input type="checkbox" ng-disabled="isexcelbasedscenario" ng-true-value="\'True\'" ng-false-value="\'False\'" ng-model="item.dictAttributes.sfwIgnore" undoredodirective model="item"></td></tr>';

            if (objselectedstep.IsRunSelected) {
                template += '<tr ng-hide="(item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\')">   <td ng-hide="(item.dictAttributes.sfwIgnore == \'True\')"> Field Result :</td> <td ng-hide="(item.dictAttributes.sfwIgnore == \'True\')"> <i class="fa fa-check faCheckScenario" ng-show="item.Status"></i><i class="fa fa-times faTimesScenario" ng-show="!item.Status"></i></td></tr>';
            }

            template += '</table></td>';
            template += '<td > <input type="checkbox" ng-disabled="isexcelbasedscenario" ng-model="items.dictAttributes.sfwIgnore" undoredodirective model="items" ng-checked="items.dictAttributes.sfwIgnore" ng-true-value="\'True\'" ng-false-value="\'False\'"></td>';
            template += '<td ng-show="objselectedstep.IsRunSelected==true && (items.dictAttributes.sfwIgnore == \'False\' || items.dictAttributes.sfwIgnore==undefined)"><i class="fa fa-check faCheckScenario" ng-show="items.Status==true"></i><i class="fa fa-times faTimesScenario" ng-show="items.Status!=true"></i></td>';
            return template;
        }
        else {
            var template = '<td valign="top" ng-repeat="item in items.Elements" ng-model="SelectedTestField">';
            template += '   <input type="text" ng-disabled="isexcelbasedscenario" class="form-control form-filter input-sm" title="{{item.dictAttributes.Value}}" ng-model="item.dictAttributes.Value" undoredodirective model="item" ng-hide="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\' || item.dictAttributes.sfwDataType == \'Object\'"/>';
            template += '   <a><img src="images/Common/icon_add_dark.svg" ng-click="OpenPopupForCollection(item)" ng-show="item.dictAttributes.sfwDataType == \'Collection\' || item.dictAttributes.sfwDataType == \'List\'"/></a>';
            template += '   <a><img src="images/Common/icon_add_dark.svg"  ng-click="OpenPopupForObject(item)" ng-show="item.dictAttributes.sfwDataType == \'Object\'"/></a>';
            template += '</td>';
            return template;
        }
        return "";
    };

    return {
        restrict: "A",
        replace: true,
        scope: {
            items: '=',
            model: '=',
            objentity: '=',
            objselectedstep: "=",
            isexcelbasedscenario: '=',
        },
        link: function (scope, element, attrs) {

            element.html(getTemplate(scope.model, scope.objselectedstep));
            $compile(element.contents())(scope);

            //#region Methods For Open popups  for collection and Object

            scope.OpenPopupForCollection = function (objField) {
                objField.objentity = scope.objentity;
                objField.lstCollectionColumn = [];
                objField.IsOutput = scope.model.IsOutput;
                objField.ExtraFields = [];

                if (objField.ruleDataInfo != undefined) {
                    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(objField, scope.objentity);
                    if (lstEntityFields != undefined) {
                        angular.forEach(lstEntityFields, function (childParameter) {
                            objField.lstCollectionColumn.push(childParameter);
                        });
                    }
                }

                if (objField.Elements.length > 0) {
                    angular.forEach(objField.Elements[0].Elements, function (item) {
                        if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                            var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                            if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                                entityFieldInfo.ilstEntityFields = [];
                            }
                            if (objField.ExtraFields == undefined) {
                                objField.ExtraFields = [];
                            }
                            $rootScope.PushItem(entityFieldInfo, objField.ExtraFields);
                        }
                    });
                }
                if (objField.ExtraFields != undefined) {
                    angular.forEach(objField.ExtraFields, function (childParameter) {
                        objField.lstCollectionColumn.push(childParameter);
                    });
                }

                var newScope = scope.$new();
                newScope.SelectedTestField = objField;
                newScope.SelectedTestField.IsOutput = false;
                newScope.objselectedstep = scope.objselectedstep;
                newScope.IsExcelBasedScenario = scope.isexcelbasedscenario;
                newScope.AddColumnForCollection = function () {
                    AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                };
                newScope.DeleteCollectionField = function () {
                    DeleteCollectionField(newScope.SelectedTestField, $rootScope);
                };
                newScope.UpdateSelectedCollectionField = function (objFields) {
                    UpdateSelectedCollectionField(objFields, newScope.SelectedTestField);
                };
                newScope.AddCollectionField = function () {
                    AddCollectionField(newScope.SelectedTestField, scope.objentity, $rootScope);
                };
                newScope.onCancelClick = function () {
                    newScope.collectionDialog.close();
                };

                newScope.menuOptionForCollection = [
                    ['Delete Column', function ($itemScope) {
                        DeleteColumnFromCollection(newScope.SelectedTestField, $itemScope.objcol);
                    }], null
                ];

                newScope.collectionDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenCollectionPopup.html");
            };

            scope.OpenPopupForObject = function (objField) {
                if (scope.model.IsOutput) {
                    scope.OpenPopupForOutputObject(objField, scope.objselectedstep);
                }
                else {
                    scope.SelectedTestField = objField;
                    scope.SelectedTestField.objentity = scope.objentity;
                    angular.forEach(scope.SelectedTestField.Elements, function (item) {
                        if (item.Name == "fields") {
                            scope.SelectedTestField.objFieldsVM = item;
                            return;
                        }
                    });
                    if (scope.SelectedTestField.objFieldsVM != undefined) {
                        angular.forEach(scope.SelectedTestField.Elements, function (item) {
                            if (item.Name == "field") {
                                scope.SelectedTestField.objFieldsVM.SelectedField = item;
                                return;
                            }
                        });
                    }

                    var newScope = scope.$new();
                    newScope.SelectedTestField = scope.SelectedTestField;
                    newScope.SelectedTestField.ExtraFields = [];

                    newScope.AddExtraFields = function (SelectedTestField) {
                        if (SelectedTestField.Elements.length > 0) {
                            angular.forEach(SelectedTestField.Elements[0].Elements, function (item) {
                                if (item.dictAttributes.isExtraEntityField && item.dictAttributes.isExtraEntityField == "true") {
                                    var entityFieldInfo = { istrName: item.dictAttributes.ID, istrDataType: item.dictAttributes.sfwDataType, istrEntity: item.dictAttributes.istrEntity };
                                    if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                                        entityFieldInfo.ilstEntityFields = [];
                                    }

                                    $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);
                                }
                            });
                        }
                    };
                    newScope.AddExtraFields(newScope.SelectedTestField);

                    newScope.IsExcelBasedScenario = scope.isexcelbasedscenario;
                    newScope.UpdateSelectedObjectField = function (objFields) {
                        UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
                    };
                    newScope.DeleteObjectField = function () {
                        DeleteObjectField(newScope.SelectedTestField, $rootScope);
                    };
                    newScope.AddColumnForCollection = function () {
                        AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                    };

                    newScope.OnConstantValueChange = function (objField) {
                        if (objField) {
                            $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
                        }
                    };

                    newScope.objectInputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopup.html");
                }
            };

            scope.OpenPopupForOutputObject = function (objField, objselectedstep) {
                scope.SelectedOutputTestField = objField;
                scope.SelectedOutputTestField.objentity = scope.objentity;
                angular.forEach(scope.SelectedOutputTestField.Elements, function (item) {
                    if (item.Name == "fields") {
                        scope.SelectedOutputTestField.objFieldsVM = item;
                        return;
                    }
                });
                if (scope.SelectedOutputTestField.objFieldsVM != undefined) {
                    angular.forEach(scope.SelectedOutputTestField.Elements, function (item) {
                        if (item.Name == "field") {
                            scope.SelectedOutputTestField.objFieldsVM.SelectedField = item;
                            return;
                        }
                    });
                }

                var newScope = scope.$new();
                newScope.SelectedTestField = scope.SelectedOutputTestField;
                newScope.objselectedstep = objselectedstep;

                newScope.UpdateSelectedObjectField = function (objFields) {
                    UpdateSelectedObjectField(objFields, newScope.SelectedTestField);
                };
                newScope.DeleteObjectField = function () {
                    DeleteObjectField(newScope.SelectedTestField, $rootScope);
                };
                newScope.AddColumnForCollection = function () {
                    AddColumnForCollection(newScope.SelectedTestField, newScope, scope, $rootScope);
                };
                newScope.OnConstantValueChange = function (objField) {
                    if (objField) {
                        $rootScope.EditPropertyValue(objField.dictAttributes.Value, objField.dictAttributes, "Value", "");
                    }
                };
                newScope.objectOutputDialog = $rootScope.showDialog(newScope, newScope.SelectedTestField.dictAttributes.ID, "Scenario/views/OpenObjectPopupForOutput.html");
            };

            //#endregion  Methods For Open popups  for collection and Object

        }
    };
}]);

//#endregion directives for Collection and object popup


//#region Logical rule  Execution directive

app.directive('ruledirectiveforscenarioexecution', ["$compile", function ($compile) {
    var getTemplatetable = function (content) {
        var template;
        if (content != undefined && content.dictAttributes != undefined) {

            if (content.dictAttributes.sfwRuleType == 'LogicalRule') {
                template = '<div class="logical-rule-container" id="dvLogicalRule">',
                    template += '<ul class="logical-rule-first-ul">',
                    template += "<li ng-if=\"item.Name != 'variables'\" ng-repeat='item in objSelectedLogicalRule.Elements'>",
                    template += '<logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="objSelectedLogicalRule" objentity="objEntity" objmainlogicalrule="objLogicalRule" scenarioruntype="scenarioruntype"></logical-rule-node-wrapper>',
                    template += '</li>',
                    template += '</ul>',
                    template += '</div>';
            }
            else if (content.dictAttributes.sfwRuleType == 'ExcelMatrix') {
                template = ' <div class="tree" custom-scroll >';
                template += '<span class="excelmatrix-columnheader">{{objLogicalRule.dictAttributes.columnHeader}}</span>';
                template += '<div class="excelmatrix-rowheader-wrapper">';
                template += '                            <span class="excelmatrix-rowheader" ng-bind="objLogicalRule.dictAttributes.rowHeader"></span>';
                template += '                        </div>';
                template += '<div class="excelmatrix-table" excelmatrixscrolldirective>';
                template += '    <table class="excel-table" cellspacing="0" cellpadding="0" id="tblExcelMatricsSecond" >';
                template += '        <thead>';
                template += '            <tr ng-repeat="objcol in objSelectedLogicalRule.objHeaderColumnValues">';
                template += '      <th valign= "top" class="excel-colheader">';
                template += '          <span ng-bind="objSelectedLogicalRule.objHeaderRowTitle"></span>';
                template += '  </th>';
                template += '      <td class="excel-colheader" ng-repeat="obj in objcol track by $index" ng-class="(obj==objSelectedLogicalRule.selectedHeaderValue)?\'excel-rowheader-execution\':\'excel-data\'"  >';
                template += '          <span valign="top" ng-bind="obj"></span>';
                template += '      </td>';
                template += '  </tr > ';
                template += '        </thead>';
                template += '        <tbody>';
                template += '            <tr ng-repeat="objval in objSelectedLogicalRule.objColumnValues" ng-init="cindex = $index">';
                template += '                <td ng-class="(objSelectedLogicalRule.objHeaderRowValues[$index] == objSelectedLogicalRule.objOtherData[0])?\'excel-rowheader-execution\' : \'excel-rowheader\'" valign= "top" >';
                template += '                    <span ng-bind="objSelectedLogicalRule.objHeaderRowValues[$index]"></span>';
                template += '                </td >';
                template += '    <td ng-class="GetExcelMatrixStepSelectedClass(obj, cindex, objSelectedLogicalRule, objval, $index)" valign="top" ng-repeat="obj in objval track by $index">';
                template += '        <span valign="top" ng-bind="obj"></span>';
                template += '    </td>';
                template += '            </tr > ';
                template += '        </tbody>';
                template += '    </table>';
                template += '</div>';
                template += '</div>';
            }
        }
        else {
            template = '<div class="tree" id="dvDecisionTable" >';
            template += '<div>     <table class="dt-table-border" cellspacing="0" cellpadding="0">'; //dont remove this empty div it is needed for design by nikhil
            template += '         <tbody>';
            template += '             <tr ng-show="obj.IsVisiblefterExecution" rowconditiondirectiveforscenarioexecution ng-repeat="obj in objSelectedLogicalRule.Rows  | limitTo : noConditionsDisplayed" items="obj" logicalrule="objSelectedLogicalRule"></tr>';
            template += '         </tbody>';
            template += '     </table></div>';
            template += ' </div>';

        }

        return template;
    };
    return {
        restrict: "E",
        replace: true,
        scope: false,

        link: function (scope, element, attrs) {

            scope.GetExcelMatrixStepSelectedClass = function (obj, index, rule, items, colvalindex) {
                var retVal = 'excel-data';
                if (rule.objOtherData != undefined) {
                    if (rule.objHeaderRowValues[index] == rule.objOtherData[0] || rule.objOtherData[1] == colvalindex) {
                        retVal += " excel-cell-execution";
                    }
                    // this needs to be optimized
                    if (rule.objHeaderRowValues[index] == rule.objOtherData[0]) {
                        var val = items[rule.objOtherData[1]];
                        if (rule.objOtherData[1] == colvalindex) {
                            if (val == obj) {
                                // scope.selectedRecord = obj;
                                retVal = 'excel-data-execution';
                            }
                        }
                    }
                }
                return retVal;
            };
            scope.LoadSomeRowsinExcelMatrix = function () {
                if (scope.objSelectedLogicalRule.objColumnValues) {
                    var startingpostion = scope.objSelectedLogicalRule.objColumnValues.length;
                    count = 0;
                    for (var i = startingpostion; i < scope.objSelectedLogicalRule.objExcelColumnValues.length; i++) {
                        if (count == 5) {
                            break;
                        }
                        else {
                            scope.objSelectedLogicalRule.objColumnValues.push(scope.objSelectedLogicalRule.objExcelColumnValues[i]);
                        }
                        count++;
                    }
                }
            };

            element.html(getTemplatetable(scope.objLogicalRule));
            $compile(element.contents())(scope);
        }
    };
}]);

//#endregion Logical rule  Execution directive
function GetClosestLogicalRule(objLogicalRule, istrEffectiveDate, ruleType) {
    var retVal;
    var blnFound = false;
    if (ruleType == "DecisionTable") {
        if (objLogicalRule.Rules != null && objLogicalRule.Rules.length > 0) {
            angular.forEach(objLogicalRule.Rules, function (value, key) {
                var effDate = value.EffectiveDate;
                if (effDate == istrEffectiveDate) {
                    retVal = value;
                    blnFound = true;
                    return;
                }
            });
            if (!blnFound) {
                angular.forEach(objLogicalRule.Rules, function (value, key) {
                    if (value.EffectiveDate == undefined || value.EffectiveDate == "") {
                        retVal = value;
                        return;
                    }
                });
            }
        }
    }
    else {
        angular.forEach(objLogicalRule.Elements, function (item) {
            if (item.Name == "logicalrule") {
                if (item.dictAttributes.sfwEffectiveDate == istrEffectiveDate) {
                    retVal = item;
                    blnFound = true;
                    return;
                }
            }
        });

        if (!blnFound) {
            angular.forEach(objLogicalRule.Elements, function (item) {
                if (item.Name == "logicalrule") {
                    if (item.dictAttributes.sfwEffectiveDate == undefined || item.dictAttributes.sfwEffectiveDate == "") {
                        retVal = item;
                    }
                    return;
                }
            });
        }
    }

    return retVal;
}

//#region Update Execution Flow Methods

function UpdateTestExecutionFlow(ilstRuleSteps, ruleType, objSelectedLogicalRule, strRunType) {
    if (ruleType == "LogicalRule") {
        CheckAndSelectStepForLogicalRule(ilstRuleSteps, objSelectedLogicalRule, strRunType);
    }
    else if (ruleType == "DecisionTable") {
        CheckAndSelectStepForDecisionTable(ilstRuleSteps, objSelectedLogicalRule);
    }
    else if (ruleType == "ExcelMatrix") {

        prepareExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);
    }
}

//#region For Logical Rule
function CheckAndSelectStepForLogicalRule(ilstRuleSteps, objSelectedLogicalRule, strRunType) {
    angular.forEach(objSelectedLogicalRule.Elements, function (step) {
        if (step.Name != "variables") {
            CheckAndUpdateStep(step, ilstRuleSteps, strRunType);
        }
    });
}

function CheckAndUpdateStep(step, executedSteps, strRunType) {
    step.VisibilityStatus = false;
    step.IsStepSelected = false;

    CheckAndSelectStep(step, executedSteps, strRunType);

    angular.forEach(step.Elements, function (childStep) {
        if (childStep.Name != "items") {
            CheckAndUpdateStep(childStep, executedSteps, strRunType);
        }
    });

    if (step.Children != undefined) {
        angular.forEach(step.Children, function (childStep) {
            if (childStep.Name != "items") {
                CheckAndUpdateStep(childStep, executedSteps, strRunType);
            }
        });
    }

    CheckAndUpdateBottomLineUnSelectedStatus(step.Elements);
    if (step.Children != null) {
        CheckAndUpdateBottomLineUnSelectedStatus(step.Children);
    }

    if (step.Name == "switch") {
        CheckAndUpdateCondtionChildrenLineStatus(step);
    }

}

function CheckAndUpdateBottomLineUnSelectedStatus(itms) {

}

function CheckAndUpdateCondtionChildrenLineStatus(condtionVM) {

}
function getRuleStep(executedSteps, step) {
    var ruleStep;
    for (var i = 0; i < executedSteps.length; i++) {
        if (executedSteps[i].istrNodeID == step.dictAttributes.sfwNodeID) {
            ruleStep = executedSteps[i];
            break;
        }
    }
    if (!ruleStep && step.dictAttributes.sfwCommented == "True" && step.Children && step.Children.length > 0) {
        for (var j = 0; j < step.Children.length; j++) {
            ruleStep = getRuleStep(executedSteps, step.Children[j]);
            if (ruleStep) {
                break;
            }
        }
    }
    return ruleStep;
}
function CheckAndSelectStep(step, executedSteps, strRunType) {
    step.IsStepSelected = false;
    step.isExpanded = false;

    var ruleStep;
    ruleStep = getRuleStep(executedSteps, step);
    step.ActualValue = undefined;

    if (ruleStep != undefined) {
        if (ruleStep.istrNodeID == step.dictAttributes.sfwNodeID) {
            if (ruleStep.iobjValue instanceof Date) {
                step.ActualValue = ruleStep.iobjValue.toString();
            }
            else {
                if (undefined != ruleStep.iobjValue) {
                    step.ActualValue = ruleStep.iobjValue.toString();
                }

                if (ruleStep.iobjValue != undefined) {
                    //Type type = ruleStep.iobjValue.GetType();
                    //if (null != type)
                    //{
                    //    var lst = type.GetGenericArguments();
                    //    if (null != lst && lst.Count() > 0)
                    //    {
                    //        step.ActualValue = GetCollectionEntityActualVale(step["sfwExpression"]);
                    //    }
                    //}
                }
            }

            step.IsErrorOccured = ruleStep.iblnErrorOccurred;

            step.IsStepSelected = true;
            step.isExpanded = true;

            CheckAndUpdateCondtionStepsStatus(step);

            step.VisibilityStatus = true;

            if (strRunType == "AllRuleSteps" && (step.Name == "calllogicalrule" || step.Name == "calldecisiontable" || step.Name == "callexcelmatrix")) {
                step.TestExeRuleObject = ruleStep;
                step.OpenExecutionWindow = true;
            }
            else if (step.Name == "foreach" || step.Name == "while") {
                step.CanExpandOrCollapse = false;
                step.TestExeRuleObject = ruleStep;
                UpdateLoopExecutionCountForLoop(step.TestExeRuleObject, step);
            }
        } else {
            step.IsStepSelected = true;
            step.isExpanded = true;
            step.VisibilityStatus = true;
        }

    } else {
        step.isExpanded = false;
    }
}

function CheckAndUpdateCondtionStepsStatus(step) {

}

function UpdateLoopExecutionCountForLoop(TestExeRuleObject, step) {
    if (TestExeRuleObject != undefined) {
        var executionCount = 0;
        angular.forEach(TestExeRuleObject.ilstSteps, function (ruleObj) {
            if (ruleObj.length > 0) {
                var ruleStep;
                angular.forEach(ruleObj, function (item) {
                    if (item.istrNodeID == step.dictAttributes.sfwNodeID) {
                        ruleStep = item;
                        return;
                    }
                });

                if (ruleStep != undefined) {
                    executionCount++;
                }
            }
        });

        step.LoopExecutionCount = executionCount;
    }
}

function GetCollectionEntityActualVale(Expression) {

}

//#endregion

//#region For Decision Table
function CheckAndSelectStepForDecisionTable(ilstRuleSteps, objSelectedLogicalRule) {
    if (objSelectedLogicalRule != undefined) {
        angular.forEach(objSelectedLogicalRule.Rows, function (row) {
            row.IsVisiblefterExecution = true;
            angular.forEach(row.Cells, function (step) {
                CheckAndUpdateStepForDecisionTable(step.Item, ilstRuleSteps, objSelectedLogicalRule);
            });
        });

        angular.forEach(objSelectedLogicalRule.DataHeaders, function (step) {
            CheckAndUpdateStepForDecisionTable(step, ilstRuleSteps, objSelectedLogicalRule);
        });
    }
}

function CheckAndUpdateStepForDecisionTable(step, ilstRuleSteps, objSelectedLogicalRule) {
    var ruleStep;
    var lst = ilstRuleSteps.filter(function (x) { return x.istrNodeID == step.NodeID; });
    if (lst && lst.length > 0) {
        ruleStep = lst[0];
    }

    step.ActualValue = undefined;
    if (ruleStep != undefined) {

        if (undefined != ruleStep.iobjValue) {
            step.ActualValue = ruleStep.iobjValue.toString();
        }


        step.IsErrorOccured = ruleStep.iblnErrorOccurred;
        step.IsStepSelected = true;

        if (step.ItemType == 'assign') {
            //Discussed with Nayan, we are not highlighting related cell now, as it introducing some more issues.
            //UpdateRelatedStepBackground(step, objSelectedLogicalRule);
        }
        //step.IsRelatedStepAfterExecution = false;
    }
    else if (step.ItemType == "rowheader") {
        step.IsStepSelected = true;
    }
}

function UpdateRelatedStepBackground(step, objSelectedLogicalRule) {

    var indexrow = -1;
    var indexcell = -1;
    angular.forEach(objSelectedLogicalRule.Rows, function (row) {
        angular.forEach(row.Cells, function (item) {
            if (item.Item.NodeID == step.NodeID) {
                indexrow = objSelectedLogicalRule.Rows.indexOf(row);
                indexcell = row.Cells.indexOf(item);
                return;
            }
        });
        if (indexrow >= 0) {
            return;
        }
    });

    if (indexrow >= 0 && indexcell >= 0) {
        for (i = 0; i < indexcell; i++) {
            var item = objSelectedLogicalRule.Rows[indexrow].Cells[i];
            if (item.Item.ItemType == "assign") {
                item.Item.IsRelatedStepAfterExecution = true;
            }
        }
    }
}

//#endregion

//#region For Excel Matrix
function CheckAndSelectStepForExcelMatrix(ilstRuleSteps, objSelectedLogicalRule) {

    if (objSelectedLogicalRule.Elements != undefined && objSelectedLogicalRule.Elements.length > 0) {
        angular.forEach(objSelectedLogicalRule.Elements, function (item) {
            if (item.Name.toString().toLowerCase() == "rows") {
                for (var row = 0; row < item.Elements.length; row++) {
                    angular.forEach(ilstRuleSteps, function (ruleStep) {
                        if (item.Elements[row].dictAttributes.sfwNodeID == ruleStep.istrNodeID) {
                            item.IsStepSelected = true;
                            item.Elements[row].OtherData = ruleStep.iobjData;
                            return;
                        }
                    });
                }
            }
        });
    }

}

function prepareExcelMatrix(ilstRuleSteps, objSelectedLogicalRule) {
    objSelectedLogicalRule.objColumnValues = [];
    objSelectedLogicalRule.objExcelColumnValues = [];
    objSelectedLogicalRule.objHeaderRowValues = [];
    objSelectedLogicalRule.objHeaderColumnValues = [];
    objSelectedLogicalRule.objHeaderRowTitle = "";
    objSelectedLogicalRule.objRows = [];

    var objExcelMatrixRule = objSelectedLogicalRule;
    for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
        if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
            var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

            for (var row = 0; row < objExcelMatrixRows.Elements.length; row++) {
                if (objExcelMatrixRows.Elements[row].dictAttributes.ID == 0) {
                    var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;

                    if (objExcelMatrixRows.Elements[row].dictAttributes.Value != undefined) {
                        objSelectedLogicalRule.objHeaderRowTitle = objExcelMatrixRows.Elements[row].dictAttributes.Value;
                    }


                    var columns = [];
                    columns = strColumnValues.split(objSelectedLogicalRule.dictAttributes.sfwDelimiter);

                    objSelectedLogicalRule.objHeaderColumnValues.push(columns);
                }
                else {
                    var strColumnValues = objExcelMatrixRows.Elements[row].dictAttributes.sfwColumnValues;
                    var columns = [];
                    columns = strColumnValues.split(objSelectedLogicalRule.dictAttributes.sfwDelimiter);
                    //objSelectedLogicalRule.objColumnValues.push(columns);
                    objSelectedLogicalRule.objExcelColumnValues.push(columns);
                    objSelectedLogicalRule.objHeaderRowValues.push(objExcelMatrixRows.Elements[row].dictAttributes.Value);
                }
            }
        }
    }
    var index = CheckAndSelectStepForExcelMatrix(ilstRuleSteps, objSelectedLogicalRule);

    var selectedcellrowindex = 0;
    if (ilstRuleSteps) {
        for (var j = 0; j < ilstRuleSteps.length; j++) {
            if (ilstRuleSteps[j].istrNodeName == "row") {
                for (var i = 0; i < objSelectedLogicalRule.objHeaderRowValues.length; i++) {
                    if (objSelectedLogicalRule.objHeaderRowValues[i] == ilstRuleSteps[j].iobjData[0]) {
                        selectedcellrowindex = i;
                        objSelectedLogicalRule.selectedHeaderValue = ilstRuleSteps[j].iobjData[1];
                    }
                }
            }
        }
    }

    if (objSelectedLogicalRule.objExcelColumnValues.length > 25) {
        for (var i = 0; i < 25; i++) {
            objSelectedLogicalRule.objColumnValues.push(objSelectedLogicalRule.objExcelColumnValues[i]);
        }
    }
    else {
        objSelectedLogicalRule.objColumnValues = objSelectedLogicalRule.objExcelColumnValues;
    }

    for (var i = 0; i < objExcelMatrixRule.Elements.length; i++) {
        if (objExcelMatrixRule.Elements[i].Name.toString().toLowerCase() == "rows") {
            var objExcelMatrixRows = objExcelMatrixRule.Elements[i];

            for (var index = 0; index < objExcelMatrixRows.Elements.length; index++) {
                if (objExcelMatrixRows.Elements[index].OtherData != undefined) {
                    var columnindex = -1;
                    angular.forEach(objSelectedLogicalRule.objHeaderColumnValues[0], function (item) {
                        if (item == objExcelMatrixRows.Elements[index].OtherData[1]) {
                            columnindex = objSelectedLogicalRule.objHeaderColumnValues[0].indexOf(item);
                            return;
                        }
                    });
                    objSelectedLogicalRule.objOtherData = [objExcelMatrixRows.Elements[index].OtherData[0], columnindex];
                }
            }
        }
    }
    //var scope = GetCurrentScopeObject("Scenario");
    //scope.objSelectedLogicalRule1 = objSelectedLogicalRule;
}


//#endregion

//#endregion

function GetExpressionValue(curVM, expression) {
    var retVal = '';

    if (expression !== undefined && expression !== '') {
        if (expression.contains(".")) {
            retVal = CheckAndGetExpressionValue(curVM, expression);
        }
        else {
            retVal = expression;
        }
    }
    return retVal;
}

function CheckAndGetExpressionValue(curVM, loopCollectionName) {
    var retVal = '';
    var loopHierarchyList = loopCollectionName.split('.');
    if (loopHierarchyList !== undefined && loopHierarchyList.length != undefined) {
        for (var ind = 0; ind < loopHierarchyList.length; ind++) {
            var collectionName = GetCollectionName(curVM, loopHierarchyList[ind]);
            if (loopHierarchyList[ind] !== collectionName) {
                var fullPath = CheckAndGetExpressionValue(curVM, collectionName);
                if (fullPath !== undefined && fullPath != '') {
                    retVal += fullPath + ".";
                }
            }
            else {
                retVal += collectionName + ".";
            }
        }
    }

    retVal = retVal.substring(0, retVal.length - 1);
    return retVal;
}

function GetCollectionName(curVM, loopItmName) {
    var retVal = loopItmName;
    var parentLoopViewModel = GetLoopParent(curVM);
    while (parentLoopViewModel !== undefined) {
        if (parentLoopViewModel !== undefined) {
            if (parentLoopViewModel.dictAttributes.sfwObjectID == loopItmName || parentLoopViewModel.dictAttributes.sfwItemName == loopItmName) {
                retVal = parentLoopViewModel.dictAttributes.sfwObjectID;
                break;
            }
        }
        parentLoopViewModel = GetLoopParent(parentLoopViewModel);
    }
    return retVal;
}

function GetLoopParent(curStepViewModel) {
    var loopViewModel;
    if (curStepViewModel != undefined) {
        var parentObj = curStepViewModel.ParentVM;
        while (parentObj !== undefined) {
            if (parentObj.Name == "foreach") {
                loopViewModel = parentObj;
                break;
            }
            parentObj = parentObj.ParentVM;
        }
    }

    return loopViewModel;
}

function GetLoopParameter(logicalRuleModel, collectionName) {
    var loopParameter;
    if (collectionName !== undefined && collectionName !== "") {
        if (collectionName.contains(".")) {
            angular.forEach(collectionName.split('.'), function (name) {
                if (loopParameter == undefined) {
                    var parameters;
                    angular.forEach(logicalRuleModel.Elements, function (item) {
                        if (item.Name == "parameters") {
                            parameters = item;
                        }
                    });

                    if (parameters !== undefined) {
                        loopParameter = GetLoopParameter1(parameters.Elements, name);
                    }
                }
                else {
                    if (loopParameter !== undefined) {
                        loopParameter = GetLoopParameter1(loopParameter.Elements, name);
                    }
                }
            });
        }
        else {
            var parameters;
            angular.forEach(logicalRuleModel.Elements, function (item) {
                if (item.Name == "parameters") {
                    parameters = item;
                }
            });

            if (parameters !== undefined) {
                loopParameter = GetLoopParameter1(parameters.Elements, name);
            }
        }
    }
    return loopParameter;
}

function GetLoopParameter1(parameters, name) {
    var parameterModel;
    angular.forEach(parameters, function (objparameterVar) {
        var strDataType = objparameterVar.dictAttributes.sfwDataType;
        var id = objparameterVar.dictAttributes.ID;
        if (id == name && (strDataType == "Collection" || strDataType == "CDOCollection" || strDataType == "List")) {
            parameterModel = objparameterVar;
            return;
        }
    });

    return parameterModel;
}

//#region Common Method for Collection and Object

function GetEntityFieldsFromRuleDataInfo(objField, objEntity) {
    var lst = [];

    if (objField.ruleDataInfo != undefined) {
        if (objField.ruleDataInfo.ilstEntityFields != undefined) {
            lst = objField.ruleDataInfo.ilstEntityFields;
        }
        else if (objField.dictAttributes.ParameterType == "InputParameter") {
            lst = objField.ruleDataInfo.ilstInputFields;
        }
        else if (objField.dictAttributes.ParameterType == "OutputParameter") {
            lst = objField.ruleDataInfo.ilstOutputFields;
        }
        if (lst == undefined) {
            lst = [];
        }
    }
    return lst;
}

//#region Common Methods For Open popups  for collection and Object

function AddColumnForCollection(SelectedTestField, newScope, $scope, $rootScope) {

    if (SelectedTestField.ruleDataInfo != undefined) {
        var EntityID = SelectedTestField.ruleDataInfo.istrEntity;

        if (EntityID != undefined && EntityID != "") {
            var attributes = $rootScope.getEntityAttributeIntellisense(EntityID, true);
            var strAttributes = JSON.stringify(attributes);
            attributes = JSON.parse(strAttributes);
            newScope.lstEntityFieldsCollection = [];
            $rootScope.UndRedoBulkOp("Start");
            angular.forEach(attributes, function (item) {

                var lstEntityfields = GetEntityFieldsFromRuleDataInfo(SelectedTestField, undefined);

                if (!this.IsFieldExist(lstEntityfields, item.ID, SelectedTestField.ExtraFields)) {
                    item.IsSelected = false;
                    item.EntityId = EntityID;
                    $rootScope.PushItem(item, newScope.lstEntityFieldsCollection);
                }
            });
            $rootScope.UndRedoBulkOp("End");
            var newColumnScope = $scope.$new();
            newColumnScope.lstEntityFieldsCollection = newScope.lstEntityFieldsCollection;
            newColumnScope.SelectAllEntityFields = function () {
                SelectAllEntityFields(newColumnScope.lstEntityFieldsCollection);
            };
            newColumnScope.ClearAllEntityFields = function () {
                ClearAllEntityFields(newColumnScope.lstEntityFieldsCollection);
            };
            newColumnScope.OnOKClickEntityFields = function () {
                OnOKClickEntityFields(newScope.SelectedTestField, newColumnScope.lstEntityFieldsCollection, newColumnScope, $rootScope);
            };
            newColumnScope.OnCloseClickEntityFields = function () {
                OnCloseClickEntityFields(newColumnScope);
            };
            newColumnScope.searchColumnsForCollection = "";
            newColumnScope.AddColumnDialog = $rootScope.showDialog(newColumnScope, "Entity Fields", "Scenario/views/AddColumnsForCollection.html", { height: 500 });
        }
    }
}

function SelectAllEntityFields(lstEntityFieldsCollection) {
    if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
        SelectAndClearEntityField(lstEntityFieldsCollection, true);
    }
}

function ClearAllEntityFields(lstEntityFieldsCollection) {
    if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
        SelectAndClearEntityField(lstEntityFieldsCollection, false);
    }
}

function OnOKClickEntityFields(SelectedTestField, lstEntityFieldsCollection, newColumnScope, $rootScope) {

    if (SelectedTestField != undefined) {
        $rootScope.UndRedoBulkOp("Start");
        if (SelectedTestField.dictAttributes.sfwDataType == "Object") {

            if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
                angular.forEach(lstEntityFieldsCollection, function (item) {
                    if (item.IsSelected) {
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        if (SelectedTestField.ExtraFields == undefined) {
                            SelectedTestField.ExtraFields = [];
                        }
                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);

                        var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'Input', isExtraEntityField: 'true', ID: item.ID, sfwDataType: item.DataType, istrEntity: item.Entity }, Elements: [] };
                        testParameter.ruleDataInfo = entityFieldInfo;
                        CheckAndAddObjectFields(testParameter, SelectedTestField.objEntity, $rootScope);

                        if (SelectedTestField.objFieldsVM == undefined) {
                            SelectedTestField.objFieldsVM = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                            $rootScope.PushItem(SelectedTestField.objFieldsVM, SelectedTestField.Elements);
                        }
                        if (!SelectedTestField.objFieldsVM.Elements.some(function (itm) { return itm.dictAttributes.ID == item.ID })) {
                            $rootScope.PushItem(testParameter, SelectedTestField.objFieldsVM.Elements);
                        }
                    }
                });
            }
        }
        else {
            var extraColumns = [];
            if (lstEntityFieldsCollection != undefined && lstEntityFieldsCollection.length > 0) {
                angular.forEach(lstEntityFieldsCollection, function (item) {
                    if (item.IsSelected) {
                        extraColumns.push(item);
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        if (SelectedTestField.ExtraFields == undefined) {
                            SelectedTestField.ExtraFields = [];
                        }
                        $rootScope.PushItem(entityFieldInfo, SelectedTestField.ExtraFields);

                        if (SelectedTestField.IsOutput) {
                            var index = 1;
                            for (var i = 1; i < SelectedTestField.lstCollectionColumn.length; i++) {
                                if (SelectedTestField.lstCollectionColumn[i].istrName == "Ignore") {
                                    index = i;
                                    break;
                                }
                            }

                            if (index > 0) {
                                $rootScope.InsertItem(entityFieldInfo, SelectedTestField.lstCollectionColumn, index);
                            }
                        }
                        else {
                            $rootScope.PushItem(entityFieldInfo, SelectedTestField.lstCollectionColumn);
                        }
                    }
                });
            }
            if (extraColumns != undefined && extraColumns.length > 0) {
                angular.forEach(extraColumns, function (item) {
                    angular.forEach(SelectedTestField.Elements, function (objFields) {
                        var entityFieldInfo = { istrName: item.ID, istrDataType: item.DataType, istrEntity: item.Entity };
                        if ((entityFieldInfo.istrDataType == "Object") || (entityFieldInfo.istrDataType == "Collection") || (entityFieldInfo.istrDataType == "CDOCollection") || (entityFieldInfo.istrDataType == "List")) {
                            entityFieldInfo.ilstEntityFields = [];
                        }
                        var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'Input', ID: item.ID, sfwDataType: item.DataType, isExtraEntityField: "true", istrEntity: item.Entity }, Elements: [] };
                        testParameter.ruleDataInfo = entityFieldInfo;
                        CheckAndAddObjectFields(testParameter, SelectedTestField.objEntity, $rootScope);
                        $rootScope.PushItem(testParameter, objFields.Elements);
                        //objFields.Elements.push(testParameter);
                    });
                });
            }
        }
        $rootScope.UndRedoBulkOp("End");
        OnCloseClickEntityFields(newColumnScope);
    }
}

function OnCloseClickEntityFields(newColumnScope) {
    if (newColumnScope.AddColumnDialog != undefined) {
        newColumnScope.AddColumnDialog.close();
    }
}

//#endregion Common Methods For Open popups  for collection and Object

//#region Methods for add ,delete collection and object field

function UpdateSelectedCollectionField(objFields, SelectedTestField) {
    SelectedTestField.SelectedFieldsVM = objFields;
}

function DeleteCollectionField(SelectedTestField, $rootScope) {
    if (SelectedTestField != undefined && SelectedTestField.SelectedFieldsVM != undefined) {
        var index = SelectedTestField.Elements.indexOf(SelectedTestField.SelectedFieldsVM);
        if (index >= 0) {
            $rootScope.DeleteItem(SelectedTestField.SelectedFieldsVM, SelectedTestField.Elements);
            if (index < SelectedTestField.Elements.length) {
                SelectedTestField.SelectedFieldsVM = SelectedTestField.Elements[index];
            }
            else if (SelectedTestField.Elements.length > 0) {
                SelectedTestField.SelectedFieldsVM = SelectedTestField.Elements[index - 1];
            }
        }
    }
}

function AddCollectionField(SelectedTestField, objEntity, $rootScope) {
    $rootScope.UndRedoBulkOp("Start");
    if (SelectedTestField != undefined) {
        var objFields = { Name: "fields", Value: '', dictAttributes: { ParameterType: SelectedTestField.dictAttributes.ParameterType }, Elements: [] };
        var blnAdd = AddParameter(objFields, SelectedTestField, objEntity, $rootScope);
        if (blnAdd) {
            $rootScope.PushItem(objFields, SelectedTestField.Elements);
            SelectedTestField.SelectedFieldsVM = objFields;
        }
    }
    $rootScope.UndRedoBulkOp("End");
}

function UpdateSelectedObjectField(objFields, SelectedTestField) {
    SelectedTestField.objFieldsVM.SelectedField = objFields;
}

function DeleteObjectField(SelectedTestField, $rootScope) {
    if (SelectedTestField.objFieldsVM != undefined && SelectedTestField.objFieldsVM.SelectedField != undefined) {
        var index = SelectedTestField.objFieldsVM.Elements.indexOf(SelectedTestField.objFieldsVM.SelectedField);
        if (index >= 0) {
            if (SelectedTestField.objFieldsVM.SelectedField.dictAttributes.isExtraEntityField == "true") {
                for (var i = 0; i < SelectedTestField.ExtraFields.length; i++) {
                    if (SelectedTestField.ExtraFields[i].istrName == SelectedTestField.objFieldsVM.SelectedField.dictAttributes.ID) {
                        SelectedTestField.ExtraFields.splice(i, 1);
                    }
                }
                $rootScope.DeleteItem(SelectedTestField.objFieldsVM.SelectedField, SelectedTestField.objFieldsVM.Elements);
                if (index < SelectedTestField.objFieldsVM.Elements.length) {
                    SelectedTestField.objFieldsVM.SelectedField = SelectedTestField.objFieldsVM.Elements[index];
                }
                else if (SelectedTestField.Elements.length > 0) {
                    SelectedTestField.objFieldsVM.SelectedField = SelectedTestField.objFieldsVM.Elements[index - 1];
                }
            }
            else {
                alert('Selected field cannot be deleted as it is a mandatory field.');
            }
        }
    }
}

function DeleteColumnFromCollection(aSelectedTestField, aobjcol) {
    if (aSelectedTestField != undefined) {
        if (aobjcol != undefined) {
            var blnFound = false;
            for (var i = 0; i < aSelectedTestField.ExtraFields.length; i++) {
                if (aSelectedTestField.ExtraFields[i].istrName == aobjcol.istrName) {
                    blnFound = true;
                    aSelectedTestField.ExtraFields.splice(i, 1);
                    for (var j = 0; j < aSelectedTestField.lstCollectionColumn.length; j++) {
                        if (aSelectedTestField.lstCollectionColumn[j].istrName == aobjcol.istrName) {
                            aSelectedTestField.lstCollectionColumn.splice(j, 1);
                            break;
                        }
                    }

                    var fieldsCount = -1;
                    if (aSelectedTestField.Elements && aSelectedTestField.Elements.length > 0) {
                        fieldsCount = aSelectedTestField.Elements.length - 1;
                    }
                    while (fieldsCount > -1) {
                        var fields = aSelectedTestField.Elements[fieldsCount];
                        angular.forEach(fields.Elements, function (field) {
                            if (field.dictAttributes.ID == aobjcol.istrName) {
                                fields.Elements.splice(fields.Elements.indexOf(field), 1);
                                if (!fields.Elements.length) {
                                    aSelectedTestField.Elements.splice(fieldsCount, 1)
                                }
                            }
                        });
                        fieldsCount--;
                    }
                    break;
                }
            }
            if (!blnFound) {
                alert('Selected field cannot be deleted as it is a mandatory field.');
            }
        }
    }
}


//#endregion Methods for add ,delete collection and object field

function AddParameter(objFields, SelectedTestField, objEntity, $rootScope) {
    var blnAdd = false;
    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(SelectedTestField, objEntity);
    if (lstEntityFields != undefined) {
        angular.forEach(lstEntityFields, function (item) {

            if (!objFields.Elements.some(function (itm) { return itm.dictAttributes.ID == item.istrName })) {
                var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: objFields.dictAttributes.ParameterType, ID: item.istrName, sfwDataType: item.istrDataType }, Elements: [] };
                testParameter.ruleDataInfo = item;
                CheckAndAddObjectFields(testParameter, objEntity, $rootScope);
                $rootScope.PushItem(testParameter, objFields.Elements);
                blnAdd = true;
            }
        });
    }
    if (SelectedTestField.ExtraFields != undefined) {
        angular.forEach(SelectedTestField.ExtraFields, function (item) {
            if (!objFields.Elements.some(function (itm) { return itm.dictAttributes.ID == item.istrName })) {
                var testParameter = { Name: "field", Value: '', dictAttributes: { ParameterType: 'InputParameter', ID: item.istrName, sfwDataType: item.istrDataType, isExtraEntityField: 'true', istrEntity: item.istrEntity }, Elements: [] };
                testParameter.ruleDataInfo = item;
                CheckAndAddObjectFields(testParameter, objEntity, $rootScope);
                $rootScope.PushItem(testParameter, objFields.Elements);
                blnAdd = true;
            }
        });
    }
    return blnAdd;
}

function CheckAndAddObjectFields(testParameter, objEntity, $rootScope) {
    if (testParameter.dictAttributes.sfwDataType == "Object") {
        var lstEntityFields = GetEntityFieldsFromRuleDataInfo(testParameter, objEntity);
        if (lstEntityFields != undefined && lstEntityFields.length > 0) {
            var objFields = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
            angular.forEach(lstEntityFields, function (item) {
                var testField = { Name: "field", Value: '', dictAttributes: { ParameterType: 'InputParameter', ID: item.istrName, sfwDataType: item.istrDataType }, Elements: [] };
                testField.ruleDataInfo = item;
                CheckAndAddObjectFields(testField, objEntity, $rootScope);
                $rootScope.PushItem(testField, objFields.Elements);
            });
            $rootScope.PushItem(objFields, testParameter.Elements);
        }
    }
}

function IsFieldExist(lstEntityfields, strID, ExtraFields) {
    var blnFound = false;
    angular.forEach(lstEntityfields, function (item) {
        if (item.istrName == strID) {
            blnFound = true;
        }
    });
    if (!blnFound) {
        angular.forEach(ExtraFields, function (item) {
            if (item.istrName == strID) {
                blnFound = true;
            }
        });
    }
    return blnFound;
}

function SelectAndClearEntityField(lstEntityField, isSelected) {
    angular.forEach(lstEntityField, function (item) {
        item.IsSelected = isSelected;
    });
}

//#endregion Common Method for Collection and Object


//#region Refresh Sceanrio Methods
function ReloadTestParameterModel(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope) {
    if (objRuleDataInfo) {
        LoadTestParameterModel(testParameterModel, objRuleDataInfo, objEntity, $rootScope, true);
    }
    else {
        testParameterModel.dictAttributes.ID = curTestParameter.dictAttributes.ID;
        testParameterModel.dictAttributes.sfwDataType = curTestParameter.dictAttributes.sfwDataType;
        testParameterModel.dictAttributes.Value = curTestParameter.dictAttributes.Value;
    }
    testParameterModel.dictAttributes.isExtraEntityField = curTestParameter.dictAttributes.isExtraEntityField;
    testParameterModel.dictAttributes.istrEntity = curTestParameter.dictAttributes.istrEntity;

    testParameterModel.dictAttributes.sfwIgnore = curTestParameter.dictAttributes.sfwIgnore;

    CheckAndUpdateInputOrExpectedValue(testParameterModel, curTestParameter);
    testParameterModel.dictAttributes.sfwIsconstant = curTestParameter.dictAttributes.sfwIsconstant;
    testParameterModel.ruleDataInfo = objRuleDataInfo;
    CheckAndLoadChildParameters(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope);

}

function LoadTestParameterModel(testParameterModel, objRuleDataInfo, objEntity, $rootScope, ignoreObject) {
    if (objRuleDataInfo) {
        testParameterModel.dictAttributes.ID = objRuleDataInfo.istrName;
        testParameterModel.dictAttributes.sfwDataType = objRuleDataInfo.istrDataType;
        testParameterModel.ruleDataInfo = objRuleDataInfo;
        if (!ignoreObject) {
            CheckAndAddObjectFields(testParameterModel, objEntity, $rootScope);
        }
    }
}

function CheckAndUpdateInputOrExpectedValue(testParameterModel, curTestParameter) {
    if (curTestParameter) {
        testParameterModel.dictAttributes.Value = curTestParameter.dictAttributes.Value;
    }
}


function CheckAndLoadChildParameters(testParameterModel, curTestParameter, objRuleDataInfo, objEntity, $rootScope) {
    if (curTestParameter && objRuleDataInfo) {
        var ilstEntityFields = GetEntityFieldsFromRuleDataInfo(testParameterModel, objEntity);

        if (testParameterModel.dictAttributes.sfwDataType == "Collection" || testParameterModel.dictAttributes.sfwDataType == "List") {
            angular.forEach(curTestParameter.Elements, function (step) {
                var curChildParametersModel = step;


                var childParametersModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };//new TestParametersModel(this, this.objRuleDataInfo, this.strType);
                childParametersModel.dictAttributes.ParameterType = testParameterModel.dictAttributes.ParameterType;
                childParametersModel.ruleDataInfo = objRuleDataInfo;

                ReloadParameters(curChildParametersModel, childParametersModel, objRuleDataInfo, objEntity, $rootScope)
                    testParameterModel.Elements.push(childParametersModel);
                
            });
        }
        else if (testParameterModel && testParameterModel.dictAttributes.sfwDataType == "Object") {
            angular.forEach(ilstEntityFields, function (fld) {
                var testFieldsModel = null;
                testFieldsModels = testParameterModel.Elements.filter(function (x) { return x.Name == "fields"; });
                if (!testFieldsModels || (testFieldsModels && testFieldsModels.length == 0)) {
                    testFieldsModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                    testParameterModel.Elements.push(testFieldsModel);
                }
                else {
                    testFieldsModel = testFieldsModels[0];
                }

                var testParameter = { Name: "field", Value: '', dictAttributes: {}, Elements: [] };
                testParameter.dictAttributes.ParameterType = "Input";

                if (testFieldsModel) {
                    testFieldsModel.Elements.push(testParameter);
                }

                var fieldsVM = curTestParameter.Elements.filter(function (x) { return x.Name == "fields"; });
                var curChildTestParameter = null;
                if (fieldsVM && fieldsVM.length > 0) {
                    var lst = fieldsVM[0].Elements.filter(function (itm) { return itm.dictAttributes.ID == fld.istrName; });
                    if (lst && lst.length > 0) {
                        curChildTestParameter = lst[0];
                    }
                }

                if (curChildTestParameter && curChildTestParameter.Name == "field") {
                    ReloadTestParameterModel(testParameter, curChildTestParameter, fld, objEntity, $rootScope);
                }
                else {
                    LoadTestParameterModel(testParameter, fld, objEntity, $rootScope);
                }
            });

            //for extra Entity Feilds
            angular.forEach(curTestParameter.Elements, function (flds) {
                if (flds.Name == "fields") {
                    var testFieldsModel;

                    testFieldsModels = testParameterModel.Elements.filter(function (x) { return x.Name == "fields"; });
                    if (!testFieldsModels || (testFieldsModels && testFieldsModels.length == 0)) {
                        testFieldsModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                        testParameterModel.Elements.push(testFieldsModel);
                    }
                    else {
                        testFieldsModel = testFieldsModels[0];
                    }
                    angular.forEach(flds.Elements, function (fld) {
                        if (!getDescendents(testParameterModel).some(function (x) { return x.dictAttributes.ID == fld.dictAttributes.ID; })) {
                            if ('isExtraEntityField' in fld.dictAttributes && fld.dictAttributes.isExtraEntityField.toLowerCase() == "true") {
                                var ruleDataInfo = {};
                                ruleDataInfo.istrName = fld.dictAttributes.ID;
                                ruleDataInfo.istrDataType = fld.dictAttributes.sfwDataType;
                                var testParamModel = { Name: "fields", Value: '', dictAttributes: {}, Elements: [] };
                                testParamModel.dictAttributes.ParameterType = "Input";
                                ReloadTestParameterModel(testParamModel, fld, ruleDataInfo, objEntity, $rootScope);
                                testFieldsModel.Elements.push(testParamModel);
                            }
                        }
                    });
                }
            });
        }
    }
}


function ReloadParameters(curChildParametersVM, childParametersModel, objRuleDataInfo, objEntity, $rootScope) {
    childParametersModel.dictAttributes.indexNumber = curChildParametersVM.dictAttributes.indexNumber;
    var lstEntityFields = GetEntityFieldsFromRuleDataInfo(childParametersModel, objEntity);
    if (lstEntityFields) {
        angular.forEach(lstEntityFields, function (entityFeildInfo) {

            LoadField(childParametersModel, curChildParametersVM, entityFeildInfo, objEntity, $rootScope);
           
        });

    }
    angular.forEach(curChildParametersVM.Elements, function (aobjextrafield) {
        if (aobjextrafield.dictAttributes.isExtraEntityField && aobjextrafield.dictAttributes.isExtraEntityField == "true") {
            var curChildTestParameter = childParametersModel.Elements.filter(function (itm) { return itm.dictAttributes.ID == aobjextrafield.dictAttributes.ID; });
            if (curChildTestParameter && curChildTestParameter.length > 0) {
            }
            else {
                childParametersModel.Elements.push(aobjextrafield);
            }
        }
    });
}

function LoadField(childParametersModel, curChildParametersVM, entityFeildInfo, objEntity, $rootScope) {
    // TestParameterModel testParameter = new TestParameterModel(this, entityFeildInfo, ParameterType.Input, null);
    if (childParametersModel && !childParametersModel.Elements.some(function (itm) { return itm.dictAttributes.ID == entityFeildInfo.istrName; })) {
        var testParameter = { Name: "field", Value: '', dictAttributes: {}, Elements: [] };
        testParameter.dictAttributes.ParameterType = "Input";

        var curChildTestParameter = curChildParametersVM.Elements.filter(function (itm) { return itm.dictAttributes.ID == entityFeildInfo.istrName; });

        if (curChildTestParameter && curChildTestParameter.length > 0 && curChildTestParameter[0].Name == "field") {
            ReloadTestParameterModel(testParameter, curChildTestParameter[0], entityFeildInfo, objEntity, $rootScope);
        }
        else {
            LoadTestParameterModel(testParameter, entityFeildInfo, objEntity, $rootScope);
        }

        childParametersModel.Elements.push(testParameter);
    }
}

//#endregion
app.directive('logicalRuleNodeWrapperForScenarioExecution', ["$compile", "$rootScope", "$timeout", "$EntityIntellisenseFactory", function ($compile, $rootScope, $timeout, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        scope: {
            model: "=",
            objentity: '=',
            objmainlogicalrule: '=',
            scenarioruntype: "=",
            parentModel: "="
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getNodeTemplate = function () {
                var templateText = "";
                switch (scope.model.Name) {
                    case "actions":
                        templateText = '<actions-node-for-scenario-execution></actions-node-for-scenario-execution>';
                        break;
                    case "break":
                    case "continue":
                    case "default":
                        templateText = '<break-continue-default-node-for-scenario-execution></break-continue-default-node-for-scenario-execution>';
                        break;
                    case "notes":
                        templateText = '<notes-node-for-scenario-execution></notes-node-for-scenario-execution>';
                        break;
                    case "return":
                    case "switch":
                    case "case":
                    case "while":
                        templateText = '<return-switch-case-while-node-for-scenario-execution></return-switch-case-while-node-for-scenario-execution>';
                        break;
                    case "calllogicalrule":
                    case "calldecisiontable":
                    case "callexcelmatrix":
                        templateText = '<call-rule-node-for-scenario-execution></call-rule-node-for-scenario-execution>';
                        break;
                    case "foreach":
                        templateText = '<for-each-node-for-scenario-execution></for-each-node-for-scenario-execution>';
                        break;
                    case "query":
                        templateText = '<query-node-for-scenario-execution></query-node-for-scenario-execution>';
                        break;
                    case "method":
                        templateText = '<method-node-for-scenario-execution></method-node-for-scenario-execution>';
                        break;

                }
                return templateText;
            };
            scope.getTemplate = function () {
                var templateTextList = ['<div node-id="{{::model.dictAttributes.sfwNodeID}}" node-name="{{::model.Name}}" ng-keydown="onkeydownevent($event,model,parentModel)">                                                                                 '
                                        , '    <span ng-show="(model.Name === \'foreach\' || model.Name === \'while\' || model.Name === \'switch\') || (model.Name !== \'case\' && model.Name !== \'default\' && model.Children.length > 0) || ((model.Name === \'case\' || model.Name === \'default\') && model.Elements.length > 0)" class="node-expander node-expanded" ng-click="toggleNodeChildren($event)"></span>                                                                                              '
                                        , '    <div class="node-wrapper node-wrapper-after">                                                                                                                                        '];
                templateTextList.push(scope.getNodeTemplate());
                templateTextList = templateTextList.concat(['<ul ng-show="model.CanExpandOrCollapse" ng-if="model.Name == \'foreach\' || model.Name == \'while\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                                                            , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                                                            , '                <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                           '
                                                            , '            </li>                                                                                                                                                                        '
                                                            , '                                                                                                                                                                                         '
                                                            , '        </ul>'
                                                            , '<ul ng-if="model.Name == \'switch\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                                                            , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                                                            , '                <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                           '
                                                            , '            </li>                                                                                                                                                                        '
                                                            , '                                                                                                                                                                                         '
                                                            , '        </ul> '
                                                            , '    </div>                                                                                                                                                                               '
                                                            , '    <ul ng-if="(model.Name == \'case\' || model.Name == \'default\') && model.Elements.length>0">                                                                                        '
                                                            , '        <li ng-repeat="item in model.Elements">                                                                                                                                          '
                                                            , '            <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                               '
                                                            , '        </li>                                                                                                                                                                            '
                                                            , '    </ul>                                                                                                                                                                                '
                                                            , '    <ul ng-if="model.Name != \'case\' && model.Name != \'default\' && model.Children.length>0">                                                                                          '
                                                            , '        <li ng-repeat="item in model.Children">                                                                                                                                          '
                                                            , '            <logical-rule-node-wrapper-for-scenario-execution model="item" parent-model="model" objmainlogicalrule="objmainlogicalrule" scenarioruntype="scenarioruntype" objentity="objentity"></logical-rule-node-wrapper-for-scenario-execution>                                                               '
                                                            , '        </li>                                                                                                                                                                            '
                                                            , '    </ul>                                                                                                                                                                                '
                                                            , '</div>                                                                                                                                                                                   ']);
                return templateTextList.join("");
            };
            scope.toggleParameters = function (event) {
                if (scope.parameters && scope.parameters.Elements.length > 0) {
                    var nodeElement = $(event.target).parents(".node").first();
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.showParameters(nodeElement);
                    }
                    else {
                        scope.hideParameters(nodeElement);
                    }
                }
            };
            scope.showParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideDown("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").addClass("fa-caret-down");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").removeClass("fa-caret-right");
            }
            scope.hideParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideUp("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").addClass("fa-caret-right");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").removeClass("fa-caret-down");
            }
            scope.syncParameterVisibility = function (nodeElement) {
                $timeout(function () {
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.hideParameters(nodeElement);
                    }
                    else {
                        scope.showParameters(nodeElement);
                    }
                });
            }
            scope.toggleNodeChildren = function (event) {
                if ($(event.target).hasClass("node-expanded")) {
                    collapseNode(event.target, true);
                }
                else {
                    expandNode(event.target, true);
                }
            };
            scope.onStepSelectChange = function (step) {
                var currentFileScope = getCurrentFileScope();
                currentFileScope.onSelectChange(step);
                scope.SelectedNode = step;
            };
            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    if (controllerScope.onActionKeyDown) {
                        controllerScope.SelectedNode = scope.SelectedNode;
                        controllerScope.onActionKeyDown(eargs);
                    }
                }
            };
            scope.onDescriptionKeyDown = function (eargs) {
            }; // Comes from a xml file, which we cannot access from web.
            scope.init = function () {
                if (scope.model.Name === "switch" || scope.model.Name === "foreach" || scope.model.Name === "while") {
                    scope.childrenCollection = scope.model.Elements;
                }

                if (scope.model.Name === "case" || scope.model.Name === "default") {
                    scope.siblingsCollection = scope.model.Elements;
                }
                else {
                    scope.siblingsCollection = scope.model.Children;
                }


                if (scope.model.Elements.length > 0) {
                    var parametersFilter = function (item) {
                        return item.Name == "parameters";
                    };
                    var parametersModel = scope.model.Elements.filter(parametersFilter);
                    if (parametersModel && parametersModel.length > 0) {
                        scope.parameters = parametersModel[0];
                    }

                }


                if (scope.model.IsSelected) {
                    scope.onStepSelectChange(scope.model);
                }
            };
            scope.getCssClass = function (model) {
                var cssClass = "";
                if (model.dictAttributes.sfwCommented === 'True') {
                    cssClass = "commented-node";
                }
                if (model.IsSelected) {
                    cssClass += " selected-node";
                    if (model.isAdvanceSearched) {
                        cssClass += ' bckgGreen';
                    }
                }
                else if (model.isAdvanceSearched) {
                    cssClass += ' bckgGrey';
                }
                return cssClass;
            }

            scope.getClass = function () {

                if (scope.model) {
                    if (scope.model.Name == "default") {
                        if (scope.model.IsStepSelected) {
                            return "case-block-execution";
                        }
                        else if (scope.model.IsErrorOccured) {
                            return "case-block-error-occur ";
                        }
                        else {
                            return "case-block-default";
                        }
                    }
                    else {
                        if (scope.model.dictAttributes && scope.model.dictAttributes.sfwCommented == "True") {
                            return "text-assign-execution-commented";
                        }
                        if (scope.model.IsStepSelected) {
                            return "text-assign-execution";
                        }
                        else if (scope.model.IsErrorOccured) {
                            return "text-assign-error-occur ";
                        }
                        else {
                            return "text-assign-default";
                        }
                    }
                }
            };

            scope.OnLoopExecutionClick = function () {
                if (scope.model.Name == "foreach" || scope.model.Name == "while") {
                    if ((scope.model.CanExpandOrCollapse != undefined && !scope.model.CanExpandOrCollapse) && scope.model.LoopExecutionCount !== undefined && scope.model.LoopExecutionCount > 0) {

                        var newScope = scope.$new();
                        newScope.LoopViewModel = GetBaseModel(scope.model);//angular.copy(scope.model);
                        newScope.LoopViewModel.TestExeRuleObject = scope.model.TestExeRuleObject;
                        newScope.LoopViewModel.IsStepSelected = scope.model.IsStepSelected;
                        newScope.LoopViewModel.CanExpandOrCollapse = true;
                        newScope.LoopViewModel.LoopExecutionCount = 0;
                        newScope.IsDiagramDivExpanded = true;
                        newScope.Elements = [];
                        newScope.IsWhileLoop = false;
                        if (scope.model.Name == "while") {
                            newScope.IsWhileLoop = true;
                        }
                        newScope.InitLoopExecutionSteps = function () {


                            if (newScope.LoopViewModel.Name == "foreach") {

                                newScope.SetEntity();

                                if (typeof newScope.LoopViewModel.TestExeRuleObject.iobjData != "undefined" && newScope.LoopViewModel.TestExeRuleObject.iobjData.length !== undefined) {
                                    var loopData = newScope.LoopViewModel.TestExeRuleObject.iobjData;
                                    if (loopData != undefined) {
                                        var i = 0;
                                        angular.forEach(loopData, function (obj) {
                                            var step;
                                            if (i < newScope.LoopViewModel.TestExeRuleObject.ilstSteps.length) {
                                                step = { RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: newScope.LoopViewModel.TestExeRuleObject.ilstSteps[i], index: i, properties: {} };
                                            }
                                            else {
                                                step = { RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: undefined, index: i, properties: {} };
                                            }
                                            if (step != undefined) {
                                                newScope.setValueForStep(step, i);
                                            }
                                            newScope.Elements.push(step);
                                            i++;
                                        });
                                    }
                                }
                                else {
                                    var i = 0;
                                    var step;
                                    angular.forEach(newScope.LoopViewModel.TestExeRuleObject.ilstSteps, function (item) {
                                        step = { RuleObj: undefined, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: item, index: index, properties: {} };
                                        newScope.Elements.push(step);
                                        index++;
                                    });
                                }

                                newScope.GetLoopParameter();

                                newScope.GetColumns();
                            }
                            else if (newScope.LoopViewModel.Name == "while") {
                                var loopData = newScope.LoopViewModel.TestExeRuleObject.ilstSteps;
                                var index = 0;
                                angular.forEach(loopData, function (obj) {
                                    var step;
                                    step = { Name: "Iteration" + index, RuleObj: obj, logicalRuleLoopViewModel: newScope.LoopViewModel, ExecutedSteps: obj, index: i, properties: {} };
                                    if (step != undefined) {
                                        newScope.setValueForStep(step, index);
                                    }
                                    newScope.Elements.push(step);
                                    index++;
                                });
                            }

                            if (newScope.Elements !== undefined && newScope.Elements.length > 0) {
                                newScope.UpdateStepOnSelectionChange(newScope.Elements[0]);
                            }
                        };

                        newScope.SetEntity = function () {
                            var loopParameterExp = newScope.LoopViewModel.dictAttributes.sfwObjectID;
                            var loopColExpFullPath = CheckAndGetExpressionValue(newScope.LoopViewModel, loopParameterExp);
                            var curEntityID = "";
                            if (scope.objentity != undefined) {
                                curEntityID = scope.objentity.dictAttributes.ID;
                            }

                            var entityId = "";
                            if (loopColExpFullPath !== undefined && loopColExpFullPath !== "") {
                                angular.forEach(loopColExpFullPath.split("."), function (parameter) {
                                    if (scope.objentity != undefined) {
                                        entityId = GetEntityID(scope.objentity, parameter);
                                    }
                                });


                                if (entityId !== undefined && entityId !== "") {
                                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                    angular.forEach(entityIntellisenseList, function (entity) {
                                        if (entity.ID == entityId) {
                                            newScope.objEntity = entity;
                                        }
                                    });
                                }
                            }
                        };
                        newScope.CheckAndSetValFotIsDataTable = function () {
                            var stIndex = newScope.LoopViewModel.dictAttributes.sfwObjectID.lastIndexOf('.');
                            if (stIndex < 0) {
                                stIndex = 0;
                            }

                            var fieldName = newScope.LoopViewModel.dictAttributes.sfwObjectID.substring(stIndex);
                            if (fieldName !== undefined && fieldName !== "") {
                                newScope.IsDataTable = false;
                                var model = newScope.objEntity.Queries.filter(function (item) {
                                    if (item.ID == fieldName) {
                                        newScope.IsDataTable = true;
                                    }
                                });



                            }
                        };

                        newScope.setValueForStep = function (step, index) {

                            if (step.ExecutedSteps == undefined) {
                                step.IsExecuted = false;
                            }
                            else {
                                step.IsExecuted = true;
                            }

                            if (step.RuleObj != undefined) {
                                if (step.RuleObj.EntityFields == undefined && step.RuleObj.Collections == undefined) {
                                    step.Name = "Iteration " + index;
                                }
                                else if (Object.keys(step.RuleObj.EntityFields).length == 0 && Object.keys(step.RuleObj.Collections).length == 0) {
                                    step.Name = "Iteration " + index;
                                }
                                else {
                                    angular.forEach(step.RuleObj.EntityFields, function (value, key) {
                                        step.properties[key] = value;
                                    });

                                    angular.forEach(step.RuleObj.Collections, function (value, key) {
                                        step.properties[key] = step.index;
                                    });

                                    angular.forEach(step.RuleObj.Entities, function (value, key) {
                                        newScope.setValueForStepForObject(value, key, step);
                                    });
                                }
                            }
                        };

                        newScope.setValueForStepForObject = function (ruleObj, ParentNodeName, step) {
                            var columnName = ParentNodeName;

                            angular.forEach(ruleObj.EntityFields, function (value, key) {
                                step.properties[columnName + "." + key] = value;
                            });

                            angular.forEach(ruleObj.Collections, function (value, key) {
                                step.properties[columnName + "." + key] = step.index;
                            });

                            angular.forEach(ruleObj.Entities, function (value, key) {
                                newScope.setValueForStepForObject(value, columnName + "." + key, step);
                            });
                        }

                        newScope.GetLoopParameter = function () {
                            var loopParameterExp = GetExpressionValue(newScope.LoopViewModel, newScope.LoopViewModel.dictAttributes.sfwObjectID);
                            if (scope.objmainlogicalrule != undefined) {
                                newScope.LoopParameter = GetLoopParameter(scope.objmainlogicalrule, loopParameterExp);
                            }
                        };

                        newScope.GetColumns = function () {
                            newScope.Columns = [];
                            if (newScope.Elements.length > 0) {
                                var ruleObj = newScope.Elements[0].RuleObj;
                                if (ruleObj !== undefined) {
                                    angular.forEach(ruleObj.EntityFields, function (value, key) {
                                        newScope.Columns.push(key);
                                    });

                                    angular.forEach(ruleObj.Collections, function (value, key) {
                                        newScope.Columns.push(key);
                                    });

                                    angular.forEach(ruleObj.Entities, function (value, key) {
                                        newScope.GetColumnsForObject(value, key);
                                    });
                                }
                                else {
                                    if (newScope.objEntity !== undefined) {
                                        var attributes = $rootScope.getEntityAttributeIntellisense(newScope.objEntity.ID, true);
                                        angular.forEach(attributes, function (field) {
                                            newScope.Columns.push(field.ID);
                                        });
                                    }
                                    else if (newScope.LoopParameter !== undefined) {
                                        angular.forEach(newScope.LoopParameter.Elements, function (parameter) {
                                            newScope.Columns.push(parameter.dictAttributes.ID);
                                        });
                                    }
                                    else {
                                        newScope.Columns.push("Iteration");
                                    }
                                }
                            }
                        };

                        newScope.GetColumnsForObject = function (ruleObj, ParentNodeName) {
                            var columnName = ParentNodeName;

                            angular.forEach(ruleObj.EntityFields, function (value, key) {
                                newScope.Columns.push(columnName + "." + key);
                            });

                            angular.forEach(ruleObj.Collections, function (value, key) {
                                newScope.Columns.push(columnName + "." + key);
                            });

                            angular.forEach(ruleObj.Entities, function (value, key) {
                                newScope.GetColumnsForObject(value, columnName + "." + key);
                            });
                        }

                        newScope.render = function (condition) {
                            if (condition) {
                                var loopwindows = [];
                                loopwindows = $('.run-result-spliter-loop');
                                angular.forEach(loopwindows, function (childwindow) {
                                    $(childwindow).width('100%').height('400px').split({ orientation: 'vertical', limit: 20 });

                                });

                            }

                        };

                        newScope.ExpandCollapseNode = function () {
                            $timeout(function () {
                                var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-expander");
                                if (elements && elements.length > 0) {
                                    for (var i = 0; i < elements.length > 0; i++) {
                                        var scope = angular.element(elements[i]).scope();
                                        if (scope && scope.model && !scope.model.IsStepSelected) {
                                            collapseNode(elements[i], true);
                                        }
                                    }
                                }

                                var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-collapsed");
                                if (elements && elements.length > 0) {
                                    for (var i = 0; i < elements.length > 0; i++) {
                                        var scope = angular.element(elements[i]).scope();
                                        if (scope && scope.model && scope.model.IsStepSelected) {
                                            expandNode(elements[i], true);
                                        }
                                    }
                                }
                            });
                        }

                        newScope.UpdateStepOnSelectionChange = function (objStep) {

                            newScope.SelectedStep = objStep;
                            if (newScope.SelectedStep != undefined) {
                                newScope.LoopViewModel.IsShowDiagram = newScope.SelectedStep.IsExecuted;
                                newScope.LoopViewModel.IsStepSelected = false;
                                if (newScope.SelectedStep.IsExecuted) {
                                    newScope.LoopViewModel.IsStepSelected = true;
                                    angular.forEach(newScope.LoopViewModel.Elements, function (childStep) {
                                        if (childStep.Name != "items") {
                                            CheckAndUpdateStep(childStep, newScope.SelectedStep.ExecutedSteps, scope.scenarioruntype);
                                        }
                                    });

                                    if (newScope.LoopViewModel.Children != null) {
                                        angular.forEach(newScope.LoopViewModel.Children, function (childStep) {
                                            if (childStep.Name != "items") {
                                                CheckAndUpdateStep(childStep, newScope.SelectedStep.ExecutedSteps, scope.scenarioruntype);
                                            }
                                        });
                                    }
                                }

                                newScope.ExpandCollapseNode();
                            }
                        };

                        newScope.setClassForLoop = function (step) {
                            if (step == newScope.SelectedStep) {
                                return "selected";
                            }

                            else if (!step.IsExecuted) {
                                return "disabled";
                            }
                        };

                        newScope.ExpandCollapseDiagram = function () {
                            newScope.IsDiagramDivExpanded = !newScope.IsDiagramDivExpanded;
                        };

                        newScope.loopExecutionDialog = $rootScope.showDialog(newScope, "Run Result For Loop", "Scenario/views/TestLoopExecution.html", { height: 550, width: 1000 });

                        newScope.InitLoopExecutionSteps();

                        newScope.ExpandCollapseNode();

                    }
                }
            };


            scope.init();

            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);

                if (scope.model.Name === "foreach" || scope.model.Name === "while") {
                    $timeout(function () {
                        var parentLoopsCount = ul.parents(".ul-children-even,.ul-children-odd").length;
                        if (parentLoopsCount > 0 && parentLoopsCount % 2 == 1) {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-even");
                        }
                        else {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-odd");
                        }
                    });
                }

            }
        },
    };
}]);

app.directive('actionsNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ActionsNodeTemplate.html",
    };
}]);
app.directive('breakContinueDefaultNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/BreakContinueDefaultNodeTemplate.html",
    };
}]);
app.directive('callRuleNodeForScenarioExecution', ["$rootScope", "$EntityIntellisenseFactory", "$timeout", function ($rootScope, $EntityIntellisenseFactory, $timeout) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.OnCallLogicalRuleDoubleClick = function () {
                if (scope.scenarioruntype == "AllRuleSteps") {
                    if (scope.model != undefined) {
                        if (scope.model.Name == "calllogicalrule" || scope.model.Name == "calldecisiontable" || scope.model.Name == "callexcelmatrix") {
                            var ruleId = scope.model.dictAttributes.sfwRuleID;
                            $.connection.hubScenarioModel.server.checkAndLoadRuleDiagram(ruleId).done(function (data) {
                                scope.$apply(function () {
                                    scope.objLogicalRule = data;
                                    var newScope = scope.$new();
                                    newScope.objLogicalRule = scope.objLogicalRule;
                                    newScope.scenarioruntype = scope.scenarioruntype;
                                    var strRuleID = "";
                                    var ruleType = "DecisionTable";
                                    if (scope.objLogicalRule.dictAttributes != undefined) {
                                        ruleType = scope.objLogicalRule.dictAttributes.sfwRuleType;
                                        strRuleID = scope.objLogicalRule.dictAttributes.ID;
                                    }
                                    else {
                                        strRuleID = scope.objLogicalRule.RuleID;
                                    }
                                    newScope.objEntity = scope.objentity;
                                    var ElapsedTime = "";
                                    var EffectiveDate = "Default";
                                    if (scope.model.TestExeRuleObject != undefined) {
                                        newScope.objSelectedLogicalRule = GetClosestLogicalRule(scope.objLogicalRule, scope.model.TestExeRuleObject.istrOtherData, ruleType);
                                        EffectiveDate = !scope.model.TestExeRuleObject.istrOtherData ? 'Default' : scope.model.TestExeRuleObject.istrOtherData;
                                        ElapsedTime = scope.model.TestExeRuleObject.ExecutionTime;
                                        var executedRuleSteps = [];
                                        if (scope.model.TestExeRuleObject.ilstSteps.length > 0 && scope.model.TestExeRuleObject.ilstSteps[0]) {
                                            executedRuleSteps = scope.model.TestExeRuleObject.ilstSteps[0];
                                        }
                                        UpdateTestExecutionFlow(executedRuleSteps, ruleType, newScope.objSelectedLogicalRule, newScope.scenarioruntype);
                                        //var scenarioscope = GetCurrentScopeObject("Scenario");
                                        //scope.objSelectedLogicalRule =  scenarioscope.objSelectedLogicalRule1;
                                    }
                                    newScope.callRuleExecutionDialog = $rootScope.showDialog(newScope, "Rule : " + strRuleID + " - Effective Date : " + EffectiveDate + "[ Elapsed Time: " + ElapsedTime + "]", "Scenario/views/TestCallLogicalRuleExecution.html", { width: 900, height: 500 });
                                    newScope.ShowOnlyExecutedPath = function (ablnShowOnlyExecutedPath) {
                                        ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, newScope.objSelectedLogicalRule);
                                    };
                                    $timeout(function () {
                                        var elements = $("#ScopeId_" + newScope.$id).find(".logical-rule-container").find(".node-expander");
                                        if (elements && elements.length > 0) {
                                            for (var i = 0; i < elements.length > 0; i++) {
                                                var scope = angular.element(elements[i]).scope();
                                                if (scope && scope.model && !scope.model.IsStepSelected) {
                                                    collapseNode(elements[i], true);
                                                }
                                            }
                                        }
                                    });

                                });
                            });
                        }
                    }
                }
            };


        },
        templateUrl: "Scenario/views/LogicalRule/CallRuleNodeTemplate.html",
    };
}]);
app.directive('forEachNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ForEachNodeTemplate.html",
    };
}]);
app.directive('methodNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/MethodNodeTemplate.html",
    };
}]);
app.directive('notesNodeForScenarioExecution', ["$rootScope", function ($rootScope) {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/NotesNodeTemplate.html",
    };
}]);
app.directive('queryNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/QueryNodeTemplate.html",
    };
}]);
app.directive('returnSwitchCaseWhileNodeForScenarioExecution', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Scenario/views/LogicalRule/ReturnSwitchCaseWhileNodeTemplate.html",
    };
}]);

var GetEntityID = function (objEntity, parameter) {
    var retVal = "";
    var objField;
    var Attributes = objEntity.Elements.filter(function (x) {
        if (x.Name == "attributes") {
            return x;
        }
    });
    if (Attributes && Attributes.length > 0) {
        objField = Attributes[0].Elements.filter(function (x) {
            if (x.Name == "attribute" && x.dictAttributes.ID == parameter) {
                return x;
            }
        });
    }

    if (objField != undefined && objField.length > 0) {
        retVal = objField[0].dictAttributes.sfwEntity;
    }

    return retVal;
};


app.directive("rowconditionitemdirectiveforscenarioexecution", ["$compile", function ($compile) {
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '='
        },
        templateUrl: "Rule/views/DecisionTable/rowconditionitem.html",
        link: function (scope, element, attrs) {
            scope.init = function () {
                scope.conditionType = "default";
                if (scope.items.Item.ItemType == 'notesheader' || scope.items.Item.ItemType == 'returnheader') {
                    scope.conditionType = "1";
                }
                else if ((scope.items.Item.ItemType == 'colheader' || scope.items.Item.ItemType == 'rowheader')) {
                    scope.conditionType = "2";
                }
                else if (scope.items.Item.ItemType == 'assignheader') {
                    scope.conditionType = "3";
                }
                else if (scope.items.Item.ItemType != 'notesheader' && scope.items.Item.ItemType != 'returnheader' && scope.items.Item.ItemType != 'colheader' && scope.items.Item.ItemType != 'rowheader' && scope.items.Item.ItemType != 'assignheader' && scope.items.Item.ItemType != 'notes') {
                    if (scope.items.Item.Expression && scope.items.Item.Expression.length > 0) {
                        scope.conditionType = "5";
                    }
                }
            };

            scope.getclass = function (obj) {
                if (obj != undefined) {
                    if (obj.Item.ItemType == "if") {
                        if (obj.Item.IsStepSelected) {
                            return "dt-if-execution";
                        }
                        else if (obj.Item.IsRelatedStepAfterExecution) {
                            return "dt-if-execution-related-step";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-if-step-execution-error-occur";
                        }
                        else {
                            return "dt-if-step-execution";
                        }
                    }
                    else if (obj.Item.ItemType == "colheader" || obj.Item.ItemType == "rowheader") {
                        return "dt-row-col-header-execution";
                    }
                    else if (obj.Item.ItemType == "assignheader") {
                        if (obj.Item.IsStepSelected) {
                            return "dt-assign-header-execution";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-assign-header-execution-related-step";
                        }
                        else {
                            return "dt-row-assign-header-execution";
                        }
                    }
                    else {
                        if (obj.Item.IsStepSelected) {
                            return "dt-assign-execution-decisiontable";
                        }
                        else if (obj.Item.IsRelatedStepAfterExecution) {
                            return "dt-assign-execution-related-step";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-assign-execution-error-occur";
                        }
                        else {
                            return "dt-assign-step-execution";
                        }
                    }
                }
            };
            scope.init();
        }
    };
}]);


app.directive("rowconditiondirectiveforscenarioexecution", ["$compile", "$interval", function ($compile, $interval) {
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '='
        },
        template: '<td rowconditionitemdirectiveforscenarioexecution ng-repeat="objChild in items.Cells" items="objChild" valign="top" rowspan={{objChild.Rowspan}} colspan={{objChild.Colspan}} class="dt-table-cell"  logicalrule="logicalrule"></td>',
    };
}]);