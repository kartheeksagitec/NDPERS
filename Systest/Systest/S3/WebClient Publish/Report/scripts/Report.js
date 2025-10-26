app.controller('ReportController', ["$scope", "$rootScope", "ngDialog", "$EntityIntellisenseFactory", "$filter", "$NavigateToFileService", "$timeout", "CONSTANTS", function ($scope, $rootScope, ngDialog, $EntityIntellisenseFactory, $filter, $NavigateToFileService, $timeout, CONST) {

    //#region Variables
    $scope.isDirty = false;
    $rootScope.IsLoading = true;
    $scope.objReportDetailsDialog = {};
    $scope.lstReportMethod = [];
    $scope.validationErrorList = [];
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.lstControl = CONST.FORM.CONTROL_TYPES;
    //#endregion

    $scope.traverseMainModel = function (path) {
        var items = [];
        var objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.ReportModel;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                if (objHierarchy) {
                    items.push(objHierarchy);
                }
            }
        }
        return items;
    };
    $scope.selectElement = function (path) {
        $scope.$evalAsync(function () {
            if (path != null && path != "form" && path != "") {
                var items = $scope.traverseMainModel(path);
                if (items.length > 0 && items[0].Name != "initialload" && items[items.length - 1].Name != "sfwRow" && items[items.length - 1].Name != "sfwTable") {
                    SetFormSelectedControl($scope.ReportModel, items[items.length - 1], window.event);
                }
            }
            $rootScope.IsLoading = false;
        });
    };
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        if (items.length > 0 && items[0].Name != "initialload" && items[items.length - 1].Name != "sfwRow" && items[items.length - 1].Name != "sfwTable") {
            items[items.length - 1].isAdvanceSearched = IsAction;
        }
    };
    //#region Xml source Start
    $scope.selectedDesignSource = false;

    $scope.ShowDesign = function () {

        if ($scope.selectedDesignSource == true) {
            //$scope.selectedDesignSource = false;
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
                $scope.receivedesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receiveReportModel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}

                    //Commented following code, because source to design synchronization is hold for now.
                    $scope.$evalAsync(function () {

                        //Select the particular tab according to path
                        $scope.selectElement(path);



                        if (path != null && path != "form" && path != "") {


                        }
                        else {
                            //$scope.onDetailClick();
                        }
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
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, null);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            var sObj;
            var indexPath = [];
            var pathString;

            if ($scope.ReportModel != null && $scope.ReportModel != undefined) {
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();
                var objreturn1 = GetBaseModel($scope.ReportModel);
                if (objreturn1 != "") {

                    var nodeId = [];
                    if ($scope.ReportModel.SelectedControl != undefined && $scope.ReportModel.SelectedControl.IsSelected) {
                        //sObj = FindDeepNode($scope.ReportModel, $scope.ReportModel.SelectedControl);
                        //pathString = getPathSource(sObj, indexPath);
                        //angular.copy(pathString.reverse(), nodeId);

                        var pathToObject = [];

                        sObj = $scope.FindDeepNode($scope.ReportModel, $scope.ReportModel.SelectedControl, pathToObject);
                        pathString = $scope.getPathSource($scope.ReportModel, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }
                    //else if ($scope.currentSelectedParams != undefined) {
                    //    sObj = $scope.FindDeepNode($scope.ReportModel, $scope.currentSelectedParams);
                    //    pathString = $scope.getPathSource(sObj, indexPath);
                    //    angular.copy(pathString.reverse(), nodeId);
                    //}

                    var strobj = JSON.stringify(objreturn1);
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
                                //$scope.ReportModel.SelectedControl = undefined;
                                $scope.currentSelectedParams = undefined;
                            });
                            if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                                $scope.$evalAsync(function () {
                                    $rootScope.IsLoading = false;
                                });
                            }
                        });
                    };
                }
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

    //#endregion

    //#region OnLoad
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveReportModel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.setMainTableandInitialLoadFormModel = function (aReportModel) {
        for (i = 0; i < aReportModel.Elements.length; i++) {
            var control = aReportModel.Elements[i];
            if (control.Name == "initialload") {
                $scope.InitialLoad = control;
            }
            else if (control.Name == 'sfwTable') {
                $scope.MainTable = control;
            }
        }
    }

    $scope.receiveReportModel = function (data) {

        $scope.$apply(function () {
            $scope.ReportModel = data;
            $scope.ReportModel.RemoteObjectCollection = [];
            $scope.objReportExtraFields = [];
            $scope.MainTable = undefined;
            $scope.InitialLoad = undefined;
            $scope.setMainTableandInitialLoadFormModel($scope.ReportModel);
            if ($scope.InitialLoad && $scope.InitialLoad.Elements.length > 0) {
                $scope.PopulateInitialLoad();
                $scope.PopulateParameters();
            }
            //Get extra field data
            $scope.objReportExtraFields = $filter('filter')($scope.ReportModel.Elements, { Name: 'ExtraFields' });
            if ($scope.objReportExtraFields.length > 0) {
                $scope.objReportExtraFields = $scope.objReportExtraFields[0];
                //$scope.removeExtraFieldsDataInToMainModel();
            }

            if ($scope.objReportExtraFields.length == 0) {
                $scope.objReportExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                };
            }

        });

        $.connection.hubCreateNewObject.server.loadMethodList().done(function (result) {
            $scope.$evalAsync(function () {
                if (result) {
                    var data = JSON.parse(result);
                    $scope.lstReportMethod = data;
                    $scope.PopulateParameters();
                }
            });
        }).fail(function (data) { alert(data); });

        $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.ReportModel.RemoteObjectCollection = data;
                    if ($scope.ReportModel.RemoteObjectCollection && $scope.ReportModel.RemoteObjectCollection.length > 0) {
                        $scope.ReportModel.RemoteObjectCollection.splice(0, 0, {
                            dictAttributes: {
                                ID: ""
                            }
                        });
                    }
                }
            });
        });

        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };

    $scope.PopulateInitialLoad = function () {
        angular.forEach($scope.InitialLoad.Elements, function (objcustommethod) {
            if (objcustommethod.Name == "query") {
                $scope.InitialLoad.Query = objcustommethod.dictAttributes.sfwQueryRef;
                $scope.objMainQuery = objcustommethod;
            }
            else if (objcustommethod.Name == "custommethod") {
                $scope.InitialLoad.Method = objcustommethod.dictAttributes.sfwMethodName;
                $scope.InitialLoad.objCustomMethod = objcustommethod;
                strParams = $scope.GetMethodParameters($scope.InitialLoad.Method);
            }

        });
    };



    //#endregion

    //#region Populate Parameter
    $scope.PopulateParameters = function (isAddUndoRedoStack) {
        var strParams = [];
        if (isAddUndoRedoStack) {
            $rootScope.EditPropertyValue($scope.lstParameters, $scope, "lstParameters", []);
        }
        else {
            $scope.lstParameters = [];
        }
        if ($scope.objMainQuery) {
            if ($scope.objMainQuery.dictAttributes.sfwQueryRef != undefined && $scope.objMainQuery.dictAttributes.sfwQueryRef != "") {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                strParams = onQueryChange($scope.objMainQuery.dictAttributes.sfwQueryRef, entityIntellisenseList);
            }
        }
        else {
            if ($scope.InitialLoad && $scope.InitialLoad.objCustomMethod && $scope.InitialLoad.objCustomMethod.dictAttributes.sfwMethodName != undefined && $scope.InitialLoad.objCustomMethod.dictAttributes.sfwMethodName != "") {
                strParams = $scope.GetMethodParameters($scope.InitialLoad.objCustomMethod.dictAttributes.sfwMethodName);
            }
        }

        if (strParams.length > 0) {
            //$scope.$evalAsync(function () {
            angular.forEach(strParams, function (item) {
                var strParameter = item.ID.substr(1);
                var strDataType = item.DataType;
                var obj = { ParamName: strParameter, ParamDataType: strDataType };
                if (isAddUndoRedoStack) {
                    $rootScope.PushItem(obj, $scope.lstParameters);
                }
                else {
                    $scope.lstParameters.push(obj);
                }
            });
            //});
        }

        $scope.ReportModel.lstParameters = $scope.lstParameters;

    };

    $scope.GetMethodParameters = function (methodName) {
        var alParams = [];
        function filter(itm) {
            return itm.ShortName == methodName;
        }
        var lstMethod = $scope.lstReportMethod.filter(filter);
        function iterator(itm) {
            var strParamType = itm.ParameterType.FullName;
            strParamType = strParamType.substring(strParamType.lastIndexOf('.') + 1);

            alParams.push({ ID: " " + itm.ParameterName, DataType: strParamType });
        }

        if (lstMethod && lstMethod.length > 0) {
            var objMethod = lstMethod[0];
            angular.forEach(objMethod.Parameters, iterator);
        }
        return alParams;
    };

    //#endregion

    //#region Add Row Command
    $scope.menuOptionsForReport = [
        ['Add Row', function ($itemScope) {
            $scope.AddRowCommand();
        }], null,

    ];
    $scope.AddRowCommand = function () {
        $rootScope.UndRedoBulkOp("Start");
        $scope.MainTable = null;
        $scope.setMainTableandInitialLoadFormModel($scope.ReportModel);
        if (!$scope.MainTable) {
            $scope.MainTable = { Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            $scope.MainTable.dictAttributes.ID = "";
            $rootScope.PushItem($scope.MainTable, $scope.ReportModel.Elements);
        }

        if ($scope.MainTable) {

            var sfxRowModel = { Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            var colCnt = GetMaxColCount(sfxRowModel, $scope.MainTable);
            if (colCnt < 1) {
                colCnt = 1;
            }
            for (var colIndex = 0; colIndex < colCnt; colIndex++) {
                var sfxCellModel = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
                $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);
            }
            $rootScope.PushItem(sfxRowModel, $scope.MainTable.Elements);
        }

        $rootScope.UndRedoBulkOp("End");
    };
    //#endregion

    //#region Selected Paramters from params list
    $scope.selectedParameters = function (obj) {
        $scope.currentSelectedParams = obj;
    };
    //#endregion

    //#region Report Details 
    $scope.onDetailClick = function () {
        var newScope = $scope.$new();

        newScope.objExtraFields = [];
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "Report";

        newScope.InitialLoad = undefined;
        newScope.objMainQuery = undefined;
        newScope.ReportModel = {};


        newScope.PopulateInitialLoad = function () {
            angular.forEach(newScope.InitialLoad.Elements, function (objcustommethod) {
                if (objcustommethod.Name == "query") {
                    newScope.InitialLoad.Query = objcustommethod.dictAttributes.sfwQueryRef;
                    newScope.objMainQuery = objcustommethod;
                }
                else if (objcustommethod.Name == "custommethod") {
                    newScope.InitialLoad.Method = objcustommethod.dictAttributes.sfwMethodName;
                    newScope.InitialLoad.objCustomMethod = objcustommethod;
                    strParams = $scope.GetMethodParameters(newScope.InitialLoad.Method);
                }

            });
        };

        newScope.Init = function () {
            newScope.ReportModel = { dictAttributes: {} };
            angular.forEach($scope.ReportModel.dictAttributes, function (val, key) {
                newScope.ReportModel.dictAttributes[key] = val;
            });
            if ($scope.InitialLoad) {
                newScope.InitialLoad = {};
                angular.copy($scope.InitialLoad, newScope.InitialLoad);
            }
            else {
                newScope.InitialLoad = { Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            }

            newScope.ReportModel.IsQuery = 'Query';
            if (newScope.InitialLoad && newScope.InitialLoad.objCustomMethod) {
                newScope.ReportModel.IsQuery = "Method";
            }

            newScope.PopulateInitialLoad();
        };

        //getting Resource list
        $.connection.hubMain.server.getResources("").done(function (data) {
            if (data && data.length > 0) {
                newScope.resourceList = data;
            }
        });
        //making hub call to get Report data
        $.connection.hubCreateNewObject.server.loadReportData().done(function (data) {
            newScope.$apply(function () {
                newScope.lstReportData = data;
            });
        });

        if (!newScope.lstReportCategory) {
            $.connection.hubCreateNewObject.server.loadReportCategory().done(function (result) {
                newScope.$evalAsync(function () {
                    if (result) {
                        var data = JSON.parse(result);
                        newScope.lstReportCategory = data;
                    }
                });
            }).fail(function (data) { alert(data); });
        }

        newScope.validateReportDetails = function () {
            newScope.ReportDetailsErrorMessage = "";

            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            if (newScope.resourceList && newScope.resourceList.length > 0 && newScope.ReportModel.dictAttributes.sfwResource) {
                if (!newScope.resourceList.filter(function (x) { return x.ResourceID && x.ResourceID.trim() == newScope.ReportModel.dictAttributes.sfwResource.trim(); }).length > 0) { // checking Resources Validation
                    newScope.ReportDetailsErrorMessage = "Error: Invalid Resource ID.";
                    return true;
                }


            }
            if (newScope.lstReportCategory && newScope.lstReportCategory.length > 0 && newScope.ReportModel.dictAttributes.sfwReportCategory) {
                if (!newScope.lstReportCategory.filter(function (x) { return x.CodeValue == newScope.ReportModel.dictAttributes.sfwReportCategory.trim(); }).length > 0) { // checking Resources Validation
                    newScope.ReportDetailsErrorMessage = "Error: Invalid Report Category.";
                    return true;
                }

            }

            if (!newScope.ValidateQuery()) {
                return true;
            }

            var flag = validateExtraFields(newScope);
            newScope.ReportDetailsErrorMessage = newScope.FormDetailsErrorMessage;
            return flag;



        };

        newScope.ValidateQuery = function () {
            var isValid = true;
            if (newScope.objMainQuery) {
                if (newScope.objMainQuery.dictAttributes.sfwQueryRef) {
                    if (!newScope.objMainQuery.dictAttributes.sfwQueryRef.contains(".")) {
                        newScope.ReportDetailsErrorMessage = "Error: Invalid Query";
                        isValid = false;
                    }
                    else {
                        var queryId = newScope.objMainQuery.dictAttributes.sfwQueryRef;
                        var lst = queryId.split('.');
                        if (lst && lst.length == 2) {
                            var entityName = lst[0];
                            var strQueryID = lst[1];
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                            if (lstEntity && lstEntity.length > 0) {
                                var objEntity = lstEntity[0];
                                if (!objEntity.Queries.some(function (x) { return x.ID == strQueryID; })) {
                                    newScope.ReportDetailsErrorMessage = "Error: Invalid Query";
                                    isValid = false;
                                }
                            }
                            else {
                                newScope.ReportDetailsErrorMessage = "Error: Invalid Query";
                                isValid = false;
                            }
                        }

                        return isValid;
                    }
                }
            }
            return isValid;
        };


        newScope.OkClick = function () {
            $scope.PopulateParameters();
            // #region extra field data
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            //#endregion
            newScope.ReportDetailsDialog.close();
        };

        newScope.onReportTypeChange = function (type) {
            if (newScope.objMainQuery) {
                newScope.objMainQuery.dictAttributes.sfwQueryRef = "";
            }
            else if (newScope.InitialLoad.objCustomMethod) {
                newScope.InitialLoad.objCustomMethod.dictAttributes.sfwMethodName = "";
            }

            while (newScope.InitialLoad.Elements.length > 0) {
                newScope.InitialLoad.Elements.splice(0, 1);
            }

            newScope.objMainQuery = undefined;
            newScope.InitialLoad.objCustomMethod = undefined;

            if (type == 'Query') {

                if (!newScope.objMainQuery) {
                    newScope.objMainQuery = { Name: 'query', Value: '', dictAttributes: {}, Elements: [] };
                    newScope.InitialLoad.Elements.push(newScope.objMainQuery);
                }
            }
            else {
                if (!newScope.InitialLoad.objCustomMethod) {
                    newScope.InitialLoad.objCustomMethod = { Name: 'custommethod', Value: '', dictAttributes: {}, Elements: [] };
                    newScope.InitialLoad.Elements.push(newScope.InitialLoad.objCustomMethod);
                }
            }
        };

        newScope.NavigateToEntityQuery = function (aQueryID) {
            if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                newScope.OkClick();
                var query = aQueryID.split(".");
                $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
            }
        };

        newScope.OkClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            angular.forEach(newScope.ReportModel.dictAttributes, function (val, key) {
                $rootScope.EditPropertyValue($scope.ReportModel.dictAttributes[key], $scope.ReportModel.dictAttributes, key, val);
            });

            for (var i = 0; i < $scope.ReportModel.Elements.length; i++) {
                if ($scope.ReportModel.Elements[i].Name == "initialload") {
                    $rootScope.DeleteItem($scope.ReportModel.Elements[i], $scope.ReportModel.Elements);
                    $rootScope.InsertItem(newScope.InitialLoad, $scope.ReportModel.Elements, i);
                    break;
                }
            }

            $rootScope.EditPropertyValue($scope.InitialLoad, $scope, "InitialLoad", newScope.InitialLoad);
            $scope.PopulateInitialLoad();

            $rootScope.EditPropertyValue($scope.objMainQuery, $scope, "objMainQuery", newScope.objMainQuery);

            $rootScope.EditPropertyValue($scope.InitialLoad.objcustommethod, $scope.InitialLoad, "objcustommethod", newScope.InitialLoad.objcustommethod);


            $scope.PopulateParameters(true);
            // #region extra field data
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            //#endregion
            $rootScope.UndRedoBulkOp("End");
            newScope.ReportDetailsDialog.close();
        };


        newScope.ReportDetailsDialog = $rootScope.showDialog(newScope, "Details", "Report/views/ReportDetails.html", { width: 700, height: 500 });

        newScope.Init();
    };
    //#endregion
    //#endregion

    //#region Parameter Intellisense for controller
    $scope.onActionKeyDown = function (event) {
        var input = event.target;
        var inputValue = input.tagName == "INPUT" ? $(input).val() : input.innerText;
        var data = [];
        if ($scope.lstParameters) {
            angular.forEach($scope.lstParameters, function (obj, index) {
                if (obj.ParamName && obj.ParamName.toLowerCase().contains(inputValue.toLowerCase())) {
                    data.push(obj.ParamName);
                }
            });
        }

        setSingleLevelAutoComplete($(input), data);
        //  setRuleIntellisense($(input), data);

        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        }
    };
    //#endregion

    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objReportExtraFields) {
    //        var index = $scope.ReportModel.Elements.indexOf($scope.objReportExtraFields);
    //        if (index > -1) {
    //            $scope.ReportModel.Elements.splice(index, 1);
    //        }
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objReportExtraFields) {
            var index = $scope.ReportModel.Elements.indexOf($scope.objReportExtraFields);
            if (index == -1) {
                $scope.ReportModel.Elements.push($scope.objReportExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };

    $scope.ClearSelectFields = function () {
        lstEntityTreeFieldData = null;
    };
    $scope.OnCutControlClick = function (model) {
        // $rootScope.UndRedoBulkOp("Start");
        var arr = [];
        arr.push(model);
        $scope.CopyToClipBoard(arr, 'Control', true);
        // $rootScope.UndRedoBulkOp("End");
    };

    $scope.OnCopyControlClick = function (model) {
        //$rootScope.UndRedoBulkOp("Start");
        var arr = [];
        arr.push(model);
        $scope.CopyToClipBoard(arr, 'Control', false);
        // $rootScope.UndRedoBulkOp("End");
    };

    $scope.OnPasteControl = function (cellVM) {


        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                function iterator(obj) {
                    var model = GetBaseModel(obj);

                    if ($scope.IsCutOper) {
                        $scope.OnDeleteControlClick(obj);
                    }
                    else {
                        model.dictAttributes.ID = GetControlID($scope.ReportModel, obj.Name);
                    }
                    if (obj.Name == "udc") {
                        model.UcChild = [];
                        model.UcChild.push(obj.UcChild[0]);
                    }

                    $rootScope.PushItem(model, cellVM.Elements);
                    model.ParentVM = cellVM;
                }
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach($scope.ClipboardData, iterator);
                if ($scope.IsCutOper) {
                    $scope.ClipboardData = [];
                    $scope.ClipboardDataOpeType = "";
                    $scope.IsCutOper = false;
                }
                $rootScope.UndRedoBulkOp("End");
            });
        }
    };
    //#region Select Control Methods
    $scope.selectControlOnDoubleClick = function (objChild, event) {
        $scope.selectControl(objChild, event);
        $scope.ActiveTabForReport = 'Properties';
    };


    $scope.selectControl = function (objChild, event) {
        if (objChild) {
            if (!objChild.isLoaded) {
                objChild.isLoaded = true;
            }

            if ($scope.ReportModel.SelectedControl && ($scope.ReportModel.SelectedControl.Name == "sfwPanel" || $scope.ReportModel.SelectedControl.Name == "sfwDialogPanel" || $scope.ReportModel.SelectedControl.Name == "sfwListView")) {
                $scope.ReportModel.SelectedControl.IsVisible = false;
            }

            SetFormSelectedControl($scope.ReportModel, objChild, event);

            if (objChild.Name == "sfwTabSheet") {
                objChild.ParentVM.SelectedTabSheet = objChild;
                objChild.ParentVM.SelectedTabSheet.IsSelected = true;
            }

            if (objChild.Name == "sfwPanel" || objChild.Name == "sfwDialogPanel" || objChild.Name == "sfwListView") {
                //objChild.initialvisibilty = !objChild.initialvisibilty;
                objChild.IsVisible = true;
            }
        }
        if ($scope.setDisplayNoneToTable) {
            $scope.setDisplayNoneToTable();
        }
    };
    $scope.OnDeleteControlClick = function (aParam) {
        if (aParam) {
            $rootScope.DeleteItem(aParam, aParam.ParentVM.Elements);
            if (aParam.ParentVM) {
                $scope.selectControl(aParam.ParentVM, null);
            }
        }
    };

    $scope.CopyToClipBoard = function (data, opetype, iscutoper) {
        $scope.ClipboardData = [];
        $scope.ClipboardDataOpeType = opetype;
        $scope.IsCutOper = iscutoper;
        function iterator(obj) {
            //var model = GetBaseModel(obj);
            $scope.ClipboardData.push(obj);
        }
        angular.forEach(data, iterator);
    };

    //#region Cut/Copy/Paste Cell

    $scope.OnCutCell = function (cellVM) {
        // $rootScope.UndRedoBulkOp("Start");
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', true);
        //scope.ClearCell(scope.model);
        //$rootScope.UndRedoBulkOp("End");
    };

    $scope.OnCopyCell = function (cellVM) {
        // $rootScope.UndRedoBulkOp("Start");
        $scope.CopyToClipBoard(cellVM.Elements, 'Cell', false);
        // $rootScope.UndRedoBulkOp("End");
    };

    $scope.OnPasteCell = function (cellVM) {

        if ($scope.ClipboardData) {
            $scope.$evalAsync(function () {
                function iterator(obj) {
                    if ($scope.IsCutOper) {
                        $scope.ClearCell(obj.ParentVM);
                    }
                    var model = GetBaseModel(obj);
                    if (!$scope.IsCutOper) {
                        model.dictAttributes.ID = GetControlID($scope.ReportModel, obj.Name);
                    }
                    $rootScope.PushItem(model, cellVM.Elements);
                    model.ParentVM = cellVM;
                }
                $rootScope.UndRedoBulkOp("Start");


                angular.forEach($scope.ClipboardData, iterator);
                if ($scope.IsCutOper) {
                    $scope.ClipboardData = [];
                    $scope.ClipboardDataOpeType = "";
                    $scope.IsCutOper = false;
                }
                $rootScope.UndRedoBulkOp("End");
            });
        }
    };
    $scope.ClearCell = function (cellVM) {
        while (cellVM.Elements.length > 0) {
            $rootScope.DeleteItem(cellVM.Elements[0], cellVM.Elements);
        }
    };
    //#endregion

    $scope.OnCopyCutPasteControlClick = function (operation) {
        //cut
        if (operation == "cut") {
            if ($scope.ReportModel && $scope.ReportModel.SelectedControl && $scope.ReportModel.SelectedControl.Name != "sfwColumn") {
                $scope.OnCutControlClick($scope.ReportModel.SelectedControl);
            }
            else if ($scope.ReportModel && $scope.ReportModel.SelectedControl && ($scope.ReportModel.SelectedControl.Name == "sfwColumn" || $scope.ReportModel.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnCutCell($scope.ReportModel.SelectedControl);
            }
        }
        //copy
        else if (operation == "copy") {
            if ($scope.ReportModel && $scope.ReportModel.SelectedControl && $scope.ReportModel.SelectedControl.Name != "sfwColumn") {
                $scope.OnCopyControlClick($scope.ReportModel.SelectedControl);
            }
            else if ($scope.ReportModel && $scope.ReportModel.SelectedControl && ($scope.ReportModel.SelectedControl.Name == "sfwColumn" || $scope.ReportModel.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnCopyCell($scope.ReportModel.SelectedControl);
            }
        }
        //paste
        else if (operation == "paste") {
            if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Control" && $scope.ReportModel.SelectedControl && ($scope.ReportModel.SelectedControl.Name == "sfwColumn" || $scope.ReportModel.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnPasteControl($scope.ReportModel.SelectedControl);
            }
            else if ($scope.ClipboardData && $scope.ClipboardDataOpeType == "Cell" && $scope.ReportModel.SelectedControl && ($scope.ReportModel.SelectedControl.Name == "sfwColumn" || $scope.ReportModel.SelectedControl.Name == "sfwButtonGroup")) {
                $scope.OnPasteCell($scope.ReportModel.SelectedControl);
            }
        }
    };

    $scope.OnRowColumnInsertMoveClick = function (operation) {
        if ($scope.ReportModel && $scope.ReportModel.SelectedControl) {

            var cellVM = null;
            var tableVM = null;
            if ($scope.ReportModel.SelectedControl.Name == "sfwColumn") {
                cellVM = $scope.ReportModel.SelectedControl;
            }
            else {
                cellVM = FindParent($scope.ReportModel.SelectedControl, "sfwColumn");
            }
            tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;
            if (tableVM && cellVM) {
                $scope.$evalAsync(function () {
                    if (operation == "InsertRowAbove") {
                        var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

                        var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
                        var index = GetIndexToInsert(false, iRowIndex);

                        $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
                    }
                    else if (operation == "InsertRowBelow") {
                        var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

                        var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
                        var index = GetIndexToInsert(true, iRowIndex);
                        $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
                    }
                    else if (operation == "InsertColumnLeft") {
                        var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
                        $scope.InsertColumn(cellVM, false, iColumnIndex, tableVM);
                    }
                    else if (operation == "InsertColumnRight") {
                        var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
                        $scope.InsertColumn(cellVM, true, iColumnIndex, tableVM);
                    }
                    else if (operation == "MoveRowUp") {
                        $scope.MoveRowUp(cellVM, tableVM)
                    }
                    else if (operation == "MoveRowDown") {
                        $scope.MoveRowDown(cellVM, tableVM);
                    }
                    else if (operation == "MoveColumnLeft") {
                        $scope.MoveColumnLeft(cellVM, tableVM);
                    }
                    else if (operation == "MoveColumnRight") {
                        $scope.MoveColumnRight(cellVM, tableVM);
                    }
                });
            }
        }
    };

    //#region Insert Column Left Or Right
    $scope.InsertColumn = function (aParam, isRight, curColIndex, tableVM) {
        function iAddColumn(rowVM) {
            var cellVM;
            for (var i = 0; i < rowVM.Elements.length; i++) {
                var acellvm = rowVM.Elements[i];
                if (rowVM.Elements.indexOf(acellvm) == curColIndex) {
                    cellVM = acellvm;
                    break;
                }
            }

            if (!cellVM) {
                $scope.CheckAndAddColumnsToRow(rowVM, curColIndex, curColIndex);
            }

            if (cellVM) {
                var index = rowVM.Elements.indexOf(cellVM);

                var prefix = "swc";
                var sfxCellModel = {
                    Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {
                    }, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = rowVM;

                if (index < rowVM.Elements.length) {
                    $rootScope.InsertItem(sfxCellModel, rowVM.Elements, index);
                }
                else {
                    $rootScope.PushItem(sfxCellModel, rowVM.Elements);
                }
            }
        }
        function iAddcolumnToRight(rowVM) {
            $scope.CheckAndAddColumnsToRow(rowVM, nextColIndex, curColIndex);
        }
        if (aParam) {

            if (isRight)//inserting next to current column
            {

                var nextColIndex = $scope.GetNextColIndex(aParam);
                $rootScope.UndRedoBulkOp("Start");


                angular.forEach(tableVM.Elements, iAddcolumnToRight);

                $rootScope.UndRedoBulkOp("End");

            }
            else //inserting before to current column
            {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iAddColumn);

                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    $scope.CheckAndAddColumnsToRow = function (arowVM, aintnextColIndex, aintCurColIndex) {
        var colIndex = 0;
        var isAdded = false;
        while (colIndex <= aintnextColIndex) {
            var obj = undefined;
            for (var i = 0; i < arowVM.Elements.length; i++) {
                var cellvm = arowVM.Elements[i];
                if (arowVM.Elements.indexOf(cellvm) == colIndex) {
                    obj = cellvm;
                    break;
                }
            }

            if (obj) {
                colIndex = $scope.GetNextColIndex(obj);
            }

            else {
                var prefix = "swc";
                var sfxCellModel = {
                    Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = arowVM;

                $rootScope.PushItem(sfxCellModel, arowVM.Elements);


                isAdded = true;
                colIndex++;
            }
        }

        if (!isAdded) {
            var prefix = "swc";
            var sfxCellModel = {
                Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
            };
            sfxCellModel.ParentVM = arowVM;


            if (aintnextColIndex < arowVM.Elements.length) {
                $rootScope.InsertItem(sfxCellModel, arowVM.Elements, aintCurColIndex + 1);
            }
            else {
                $rootScope.PushItem(sfxCellModel, arowVM.Elements);
            }
        }
    };

    $scope.GetNextColIndex = function (aCellVM) {

        var nextColIndex = aCellVM.ParentVM.Elements.indexOf(aCellVM);

        var colSpan = $scope.getColspan(aCellVM);
        if (colSpan > 0) {
            nextColIndex = aCellVM.ParentVM.Elements.indexOf(aCellVM) + colSpan;
        }
        else {
            nextColIndex++;
        }

        return nextColIndex;
    };

    $scope.getColspan = function (item) {
        if (item && item.dictAttributes.ColumnSpan && parseInt(item.dictAttributes.ColumnSpan)) {
            return item.dictAttributes.ColumnSpan;
        }
        else {
            return 1;
        }
    };


    //#endregion

    //#region Move Row/Column Up/Down

    $scope.MoveRowUp = function (aParam, tableVM) {
        if (aParam) {
            var cellVM = aParam;
            var RowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);
            if (RowIndex > 0) {
                //Removing
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(cellVM.ParentVM, tableVM.Elements);

                //Adding
                $rootScope.InsertItem(cellVM.ParentVM, tableVM.Elements, RowIndex - 1);
                $rootScope.UndRedoBulkOp("End");

            }
        }
    };

    $scope.MoveRowDown = function (aParam, tableVM) {
        if (aParam) {
            {
                var cellVM = aParam;

                var RowCount = tableVM.Elements.length;
                var RowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);
                if (RowIndex < RowCount - 1) {

                    //Removing
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(cellVM.ParentVM, tableVM.Elements);


                    //Adding
                    $rootScope.InsertItem(cellVM.ParentVM, tableVM.Elements, RowIndex + 1);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
        }
    };

    $scope.MoveColumnLeft = function (aParam, tableVM) {
        function iMoveColumnLeft(rowVM) {
            var model = rowVM.Elements[ColIndex];
            if (model) {
                //Removing
                $rootScope.DeleteItem(rowVM.Elements[ColIndex], rowVM.Elements);

                //Adding
                $rootScope.InsertItem(model, rowVM.Elements, ColIndex - 1);
            }
        }

        if (aParam) {
            var ColIndex = aParam.ParentVM.Elements.indexOf(aParam);
            if (ColIndex > 0) {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iMoveColumnLeft);

                $rootScope.UndRedoBulkOp("End");

            }
        }
    };

    $scope.MoveColumnRight = function (aParam, tableVM) {
        function iMoveColumnRight(rowVM) {
            var model = rowVM.Elements[ColIndex];
            if (model) {
                //Removing
                $rootScope.DeleteItem(rowVM.Elements[ColIndex], rowVM.Elements);

                //Adding
                $rootScope.InsertItem(model, rowVM.Elements, ColIndex + 1);
            }
        }

        if (aParam) {
            var ColCount = GetMaxColCount(aParam.ParentVM, tableVM);
            var ColIndex = aParam.ParentVM.Elements.indexOf(aParam);
            if (ColIndex < ColCount - 1) {
                $rootScope.UndRedoBulkOp("Start");

                angular.forEach(tableVM.Elements, iMoveColumnRight);

                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

    //#endregion

    //#region Functions for Clear Panel, Delete Row and Delete Column.

    $scope.clearCell = function (cell) {
        while (cell.Elements.length > 0) {
            $rootScope.DeleteItem(cell.Elements[0], cell.Elements);
        }
    }
    $scope.clearPanel = function () {
        if ($scope.ReportModel.SelectedControl) {
            var table = null;
            if ($scope.ReportModel.SelectedControl.Name === "sfwPanel") {
                table = $scope.ReportModel.SelectedControl.Elements[0];
            }
            else {
                table = FindParent($scope.ReportModel.SelectedControl, "sfwTable", true);
            }

            if (table) {
                $rootScope.UndRedoBulkOp("Start");
                for (var rowIndex = 0; rowIndex < table.Elements.length; rowIndex++) {
                    var row = table.Elements[rowIndex];
                    if (row) {
                        for (var cellIndex = 0; cellIndex < row.Elements.length; cellIndex++) {
                            $scope.clearCell(row.Elements[cellIndex]);
                        }
                    }
                }
                if ($scope.ReportModel.SelectedControl.Name !== "sfwPanel" && $scope.ReportModel.SelectedControl.Name !== "sfwTable" && $scope.ReportModel.SelectedControl.Name !== "sfwColumn") {
                    var parentCell = FindParent($scope.ReportModel.SelectedControl, "sfwColumn", true);
                    if (parentCell) {
                        $scope.selectControl(parentCell);
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    }
    $scope.deleteRow = function () {
        if ($scope.ReportModel.SelectedControl) {
            var row = FindParent($scope.ReportModel.SelectedControl, "sfwRow", true);
            if (row && row.ParentVM && row.ParentVM.Elements) {
                if (row.ParentVM.Elements.length > 1) {

                    var rowIndex = row.ParentVM.Elements.indexOf(row);
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(row, row.ParentVM.Elements);

                    if (rowIndex === row.ParentVM.Elements.length) {
                        rowIndex--;
                    }

                    //selecting the first cell of next row.
                    if (rowIndex > -1 && row.ParentVM.Elements.length > 0 && row.ParentVM.Elements[rowIndex].Elements.length > 0) {
                        $scope.selectControl(row.ParentVM.Elements[rowIndex].Elements[0]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    alert("Atleast one row should be present");
                }
            }
        }
    }
    $scope.deleteColumn = function () {
        if ($scope.ReportModel.SelectedControl) {
            var cell = FindParent($scope.ReportModel.SelectedControl, "sfwColumn", true);
            if (cell && cell.ParentVM && cell.ParentVM.ParentVM) {
                var table = cell.ParentVM.ParentVM;
                var ColCount = GetMaxColCount(cell.ParentVM, table);
                if (ColCount > 1) {
                    var colIndex = cell.ParentVM.Elements.indexOf(cell);

                    var deleteCellFromRow = function (row) {
                        if (row.Elements.length > colIndex) {
                            $rootScope.DeleteItem(row.Elements[colIndex], row.Elements);
                            if (row.Elements && row.Elements.length == 0 && row.ParentVM) {
                                $rootScope.DeleteItem(row, row.ParentVM.Elements);
                            }
                        }
                    };

                    $rootScope.UndRedoBulkOp("Start");

                    angular.forEach(table.Elements, deleteCellFromRow);

                    if (colIndex === cell.ParentVM.Elements.length) {
                        colIndex--;
                    }

                    //selecting the cell just after the selected cell.
                    if (cell.ParentVM.Elements.length > colIndex) {
                        $scope.selectControl(cell.ParentVM.Elements[colIndex]);
                    }

                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    alert("Atleast one column should be present");
                }
            }
        }
    }

    //#endregion

    //#region Insert Cell / Delete Cell

    $scope.InsertCell = function (arowVM, acellVM, isRight) {
        var icurColIndex = -1;
        if (isRight) {
            icurColIndex = arowVM.Elements.indexOf(acellVM) + 1;
        }
        else {
            icurColIndex = arowVM.Elements.indexOf(acellVM);

            if (icurColIndex > 0) {
                icurColIndex -= icurColIndex;
            }
        }
        if (icurColIndex > -1) {
            var prefix = "swc";
            var sfxCellModel = {
                Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {
                }, Elements: [], Children: []
            };
            sfxCellModel.ParentVM = arowVM;

            if (icurColIndex < arowVM.Elements.length) {
                $rootScope.InsertItem(sfxCellModel, arowVM.Elements, icurColIndex);
            }
            else {
                $rootScope.PushItem(sfxCellModel, arowVM.Elements);
            }
        }
    };

    $scope.deleteCellClick = function () {
        if ($scope.ReportModel && $scope.ReportModel.SelectedControl) {
            if ($scope.ReportModel.SelectedControl.Name == "sfwPanel" && $scope.ReportModel.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                //var objGridView = FindParent($scope.ReportModel.SelectedControl, "sfwGridView");
                if ($scope.ReportModel.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.ReportModel.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.ReportModel.SelectedControl, "sfwColumn");
                }

                $scope.deleteCell(cellVM);
            }
        }
    };

    $scope.deleteCell = function (aCellVM) {
        if (aCellVM.ParentVM && aCellVM.ParentVM.ParentVM && aCellVM.ParentVM.ParentVM.Name == 'sfwTable' && aCellVM.ParentVM.ParentVM.Elements.length == 1 && aCellVM.ParentVM.Elements.length == 1) {
            alert("Cannot delete as only one cell present.")
        }
        else {
            $rootScope.DeleteItem(aCellVM, aCellVM.ParentVM.Elements);
            if ($scope.ReportModel && $scope.ReportModel.SelectedControl && $scope.ReportModel.SelectedControl.ParentVM) {
                if ($scope.ReportModel.SelectedControl.ParentVM.Elements.length == 0 && $scope.ReportModel.SelectedControl.ParentVM.Name == "sfwRow") {
                    $rootScope.DeleteItem($scope.ReportModel.SelectedControl.ParentVM, $scope.ReportModel.SelectedControl.ParentVM.ParentVM.Elements);
                    $scope.selectControl($scope.ReportModel.SelectedControl.ParentVM.ParentVM, null);
                } else {
                    $scope.selectControl($scope.ReportModel.SelectedControl.ParentVM, null);
                }
            }
        }
    };

    $scope.OnInsertCellClick = function (operation) {
        if ($scope.ReportModel && $scope.ReportModel.SelectedControl) {
            if ($scope.ReportModel.SelectedControl.Name == "sfwPanel" && $scope.ReportModel.SelectedControl.IsMainPanel) {
                return;
            }
            else {
                var cellVM = null;
                var tableVM = null;
                //var objGridView = FindParent($scope.ReportModel.SelectedControl, "sfwGridView");
                if ($scope.ReportModel.SelectedControl.Name == "sfwColumn") {
                    cellVM = $scope.ReportModel.SelectedControl;
                }
                else {
                    cellVM = FindParent($scope.ReportModel.SelectedControl, "sfwColumn");
                }
                tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;
                if (tableVM && cellVM) {
                    $scope.$evalAsync(function () {
                        if (operation == "InsertCellLeft") {
                            $scope.InsertCell(cellVM.ParentVM, cellVM, false);
                        }
                        else if (operation == "InsertCellRight") {
                            $scope.InsertCell(cellVM.ParentVM, cellVM, true);
                        }
                    });
                }
            }
        }
    };

    //#endregion

}]);