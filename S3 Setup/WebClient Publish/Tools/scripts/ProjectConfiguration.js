app.controller("projectconfigurationcontroller", ["$scope", "$rootScope", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", "$filter", "$timeout", function ($scope, $rootScope, $EntityIntellisenseFactory, $getEnitityXmlMethods, $filter, $timeout) {
    //#region Varaibles
    $scope.objCommonproperties = {};
    $scope.stdBookmarksModel = {};
    $scope.objServerMethods = {};
    $scope.remoteObjCollection = {};
    $scope.objBusinessObject = {};
    $scope.objBusinessObject.ObjTree = undefined;
    $scope.objBusinessObject.BusObjectName = "";
    $scope.isSourceDirty = false;
    $scope.serverMethodFilters = { isServiceFilterVisible: false, isServerMethodFilterVisible: false };
    //Added a collection of string, to maintain the names of loaded tab contents.
    $scope.loadedTabs = [];

    //#endregion

    //#region On Load
    $scope.initialize = function () {
        $rootScope.IsLoading = true;
        $scope.currentfile = $rootScope.currentopenfile.file;
        hubMain.server.getModel($rootScope.currentopenfile.file.FilePath, $rootScope.currentopenfile.file.FileType).done(function (data) {
            if (data) {
                $scope.receiveProjectConfigurationmodel(data);
            }
            else {
                $rootScope.closeFile($scope.currentfile.FileName);
            }

        });
        function setRemoteObjectCollection(data) {
            if (data) {
                $scope.remoteObjCollection = data;
            }
        }
        hubMain.server.getRemoteObjectCollection().done(setRemoteObjectCollection);
    };

    $scope.receiveProjectConfigurationmodel = function (data) {
        $scope.$evalAsync(function () {
            $scope.projectconfigurationfiledata = data;

            //Check and create commonproperties model
            var items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "commonproperties" }, true);
            if (items && items.length > 0) {
                $scope.objCommonproperties = items[0];
            }
            else {
                $scope.objCommonproperties = $scope.CreateNewObject("commonproperties", $scope.projectconfigurationfiledata);
            }

            //Check and create properties model
            items = $filter("filter")($scope.objCommonproperties.Elements, { Name: "properties" }, true);
            if (items && items.length > 0) {
                $scope.propertiesModel = items[0];
            }
            else {
                $scope.propertiesModel = $scope.CreateNewObject("properties", $scope.objCommonproperties);
            }

            //Check and create correspondence model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "correspondence" }, true);
            if (items && items.length > 0) {
                $scope.correspondenceModel = items[0];
            }
            else {
                $scope.correspondenceModel = $scope.CreateNewObject("correspondence", $scope.projectconfigurationfiledata);
            }

            //Check and create stdbookmarks model
            items = $filter("filter")($scope.correspondenceModel.Elements, { Name: "stdbookmarks" }, true);
            if (items && items.length > 0) {
                $scope.stdBookmarksModel = items[0];
            }
            else {
                $scope.stdBookmarksModel = $scope.CreateNewObject("stdbookmarks", $scope.correspondenceModel);
            }

            //Check and create servermethods model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "servermethods" }, true);
            if (items && items.length > 0) {
                $scope.objServerMethods = items[0];
            }
            else {
                $scope.objServerMethods = $scope.CreateNewObject("servermethods", $scope.projectconfigurationfiledata);
            }

            //Check and create globalparameters model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "globalparameters" }, true);
            if (items && items.length > 0) {
                $scope.globalParametersModel = items[0];
            }
            else {
                $scope.globalParametersModel = $scope.CreateNewObject("globalparameters", $scope.projectconfigurationfiledata);
            }

            //Check and create portals model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "applicationportals" }, true);
            if (items && items.length > 0) {
                $scope.portalsModel = items[0];
            }
            else {
                $scope.portalsModel = $scope.CreateNewObject("applicationportals", $scope.projectconfigurationfiledata);
            }


            for (var index = 0; index < $scope.stdBookmarksModel.Elements.length; index++) {
                //$scope.loadXmlMethods($scope.stdBookmarksModel.Elements[index]);
                $scope.onCorrespondanceEntityChanged($scope.stdBookmarksModel.Elements[index]);
            }

            $scope.selectCommonPropertiesTab();
            // if server method groups are present - select the first method group
            if ($scope.objServerMethods.Elements && $scope.objServerMethods.Elements.length > 0) {
                $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[0]);
            }

            $scope.validatePortals();

            $rootScope.IsLoading = false;
        });
    };

    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };


    //#endregion

    //#region Xml source 
    $scope.showSource = false;
    $scope.selectedDesignSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            $scope.selectedDesignSource = false;
            var xmlstring = $scope.editor.getValue();
            var lineno = $scope.editor.selection.getCursor().row;
            lineno = lineno + 1;
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $rootScope.currentopenfile.file, lineno);
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

                    //  SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "SourceToDesignCommon");
                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Source-Design", lineNumber);
                }
                $scope.receivedesignxmlobject = function (data, path) {
                    if ($scope.isSourceDirty) {
                        $scope.receiveProjectConfigurationmodel(data);
                        $scope.$evalAsync(function () {
                            $scope.selectItemByPath(path);
                        });
                        $scope.isSourceDirty = false;
                    } else {
                        $scope.selectItemByPath(path);
                    }
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                };
            }
        }
    };

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
            if ($scope.projectconfigurationfiledata != null && $scope.projectconfigurationfiledata != undefined) {
                $rootScope.IsLoading = true;
                var objreturn1 = GetBaseModel($scope.projectconfigurationfiledata);

                if (objreturn1 != "") {
                    var nodeId = [];
                    var selectedItem;
                    $rootScope.IsLoading = true;
                    var sObj;
                    var indexPath = [];
                    var pathString;
                    var selectedItem = $scope.getSelectedItem();

                    if (selectedItem) {
                        var pathToObject = [];
                        sObj = $scope.FindDeepNode($scope.projectconfigurationfiledata, selectedItem, pathToObject);
                        pathString = $scope.getPathSource($scope.projectconfigurationfiledata, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }

                    var strobj = JSON.stringify(objreturn1);
                    if (strobj.length < 32000) {
                        //hubMain.server.getXmlObject(strobj, $rootScope.currentopenfile.file);
                        hubMain.server.getSourceXmlObject(strobj, $rootScope.currentopenfile.file, nodeId);
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
                        SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Design-Source", nodeId);

                    }
                    $scope.receivesourcexml = function (xmlstring, lineno) {
                        $scope.$apply(function () {
                            $scope.xmlSource = xmlstring;
                            var ID = "projectconfiguration";
                            setDataToEditor($scope, xmlstring, lineno, ID);
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
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

    $scope.getSelectedItem = function () {
        var item = null;
        //select common property
        switch ($scope.SelectedTab) {
            case "CommonProperties":
                if ($scope.selectedCommonProperty) {
                    item = $scope.selectedCommonProperty;
                }
                if ($scope.selectedObject) {
                    if ($scope.selectedObjectItem) {
                        item = $scope.selectedObjectItem;
                    } else {
                        item = $scope.selectedObject;
                    }
                }
                break;
            case "Correspondance":
                if ($scope.selectedStandardBookmark) {
                    if ($scope.selectedStandardBookmarkItem) {
                        item = $scope.selectedStandardBookmarkItem;
                    } else {
                        item = $scope.selectedStandardBookmark;
                    }
                }
                break;
            case "ServerMethods":
                if ($scope.currentSrvMethod) {
                    if ($scope.currentMethod) {
                        item = $scope.currentMethod;
                    } else {
                        item = $scope.currentSrvMethod;
                    }
                }
                break;
            case "GlobalParameters":
                if ($scope.selectedGlobalParameter) {
                    item = $scope.selectedGlobalParameter;
                }
                break;
            default:
                item = $scope.selectedCommonProperty;
        }
        return item;
    };

    $scope.selectItemByPath = function (path) {
        console.log(path);
        if (path != null && path != "") {
            var items = [];
            var objHierarchy;
            if (path.contains("-") || path.contains(",")) {
                objHierarchy = $scope.projectconfigurationfiledata;
                for (var i = 0; i < path.split(',').length; i++) {
                    objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                    items.push(objHierarchy);
                }
            }

            if (items.length > 1) {
                for (var i = 0; i < items.length; i++) {
                    // commonproperties-0,object-2,item-1
                    if (items[i].Name == "commonproperties") {
                        $scope.selectCommonPropertiesTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && i > 0 && items[i - 1].Name == "properties" && items[i].Name == "item") {
                        $scope.selectCommonProperty(items[i]);
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && items[i].Name == "object") {
                        $scope.selectObject(items[i]);
                        $scope.toggleObjectExpander(items[i], true);
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && i > 0 && items[i - 1].Name == "object" && items[i].Name == "item") {
                        $scope.selectObjectItem(items[i - 1], items[i]);
                        continue;
                    }
                    // correspondence - 2, stdbookmarks - 0, object - 0, item - 1
                    if (items[i].Name == "correspondence") {
                        $scope.selectCorrespondanceTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && items[i].Name == "stdbookmarks") {
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && i > 0 && items[i - 1].Name == "stdbookmarks" && items[i].Name == "object") {
                        $scope.selectStandardBookmark(items[i]);
                        $scope.toggleStandardBookmarkExpander(items[i], true);
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && i > 1 && items[i - 1].Name == "object" && items[i].Name == "item") {
                        $scope.selectStandardBookmarkItem(items[i - 1], items[i]);
                        continue;
                    }
                    //servermethods-1,methodgroup-7,method-1,parameter-0
                    if (items[i].Name == "servermethods") {
                        $scope.selectServerMethods();
                        continue;
                    }
                    if ($scope.SelectedTab == "ServerMethods" && items[i].Name == "methodgroup") {
                        $scope.getCurrentSrvMethod(items[i]);
                        continue;
                    }
                    if ($scope.SelectedTab == "ServerMethods" && i > 0 && items[i - 1].Name == "methodgroup" && items[i].Name == "method") {
                        $scope.getCurrentMethod(items[i]);
                        continue;
                    }
                    //globalparameters-3,globlarparameter-1
                    if (items[i].Name == "globalparameters") {
                        $scope.selectGlobalParametersTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "GlobalParameters" && items[i].Name == "globlarparameter") {
                        $scope.selectGlobalParameter(items[i]);
                        continue;
                    }
                }
            }
            else if (items.length == 1) {
                $scope.selectCommonPropertiesTab();
                $scope.selectCommonProperty(items[0]);
            }
        }
    }

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
                //item.ParentVM = objParentElements
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
    //#endregion

    //#region Common Methods

    $scope.selectCommonPropertiesTab = function () {
        $scope.SelectedTab = "CommonProperties";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only.
        if ($scope.loadedTabs.indexOf("CommonProperties") == -1) {
            $scope.loadedTabs.push("CommonProperties");
        }
        //if ($scope.objCommonproperties && $scope.objCommonproperties.Elements) {
        //    $scope.loadObjectTreeForExpandedObject($scope.objCommonproperties.Elements);
        //}
    };
    $scope.selectCorrespondanceTab = function () {
        $scope.SelectedTab = "Correspondance";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("Correspondance") == -1) {
            $scope.loadedTabs.push("Correspondance");
        }
        //if ($scope.stdBookmarksModel && $scope.stdBookmarksModel.Elements) {
        //    $scope.loadObjectTreeForExpandedObject($scope.stdBookmarksModel.Elements);
        //}
    };
    $scope.selectServerMethods = function () {
        $scope.SelectedTab = "ServerMethods";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("ServerMethods") == -1) {
            $scope.loadedTabs.push("ServerMethods");
        }
    };
    $scope.selectGlobalParametersTab = function () {
        $scope.SelectedTab = "GlobalParameters";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("GlobalParameters") == -1) {
            $scope.loadedTabs.push("GlobalParameters");
        }
    };
    $scope.selectPortalsTab = function () {
        $scope.SelectedTab = "Portals";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("Portals") == -1) {
            $scope.loadedTabs.push("Portals");
        }
    };

    $scope.onBusObjectChanged = function (object, loadXmlMethods) {
        if (object && object.IsObjectVisibility) {
            $scope.$evalAsync(function () {
                $scope.objBusinessObject.BusObjectName = object.dictAttributes.ID;
                $scope.objBusinessObject.ObjTree = undefined;
            });
        }
        if (loadXmlMethods) {
            $scope.loadXmlMethods(object);
        }
    };

    //#endregion

    //#region Object Tree Methods

    //This method loads the object tree of the expanded object (object/standard bookmark) for the selected tab.
    $scope.loadObjectTreeForExpandedObject = function (collection) {
        if (collection && collection.length > 0) {
            var items = $filter('filter')(collection, { IsObjectVisibility: true }, true);
            if (items && items.length > 0) {
                if (items[0].dictAttributes && items[0].dictAttributes.ID && items[0].dictAttributes.ID.trim().length > 0
                    && $scope.objBusinessObject.ObjTree.ObjName != items[0].dictAttributes.ID) {
                    $scope.$evalAsync(function () {
                        $scope.objBusinessObject.BusObjectName = items[0].dictAttributes.ID;
                        $scope.objBusinessObject.ObjTree = undefined;
                    });
                }
            }
        }
    };

    $scope.receiveObjectTree = function (data, path) {
        $scope.$evalAsync(function () {
            var obj = JSON.parse(data);
            if (path != undefined && path != "") {
                var busObject = getBusObjectByPath(path, $scope.objBusinessObject.ObjTree);
                if (busObject && busObject.ItemType.Name == obj.ObjName) {
                    busObject.ChildProperties = obj.ChildProperties;
                    busObject.lstMethods = obj.lstMethods;
                    busObject.HasLoadedProp = true;
                }
                SetParentForObjTreeChild(busObject);
            }
            else {
                $scope.objBusinessObject.ObjTree = obj;
                $scope.objBusinessObject.ObjTree.IsMainBusObject = true;
                $scope.objBusinessObject.ObjTree.IsVisible = true;
                SetParentForObjTreeChild($scope.objBusinessObject.ObjTree);
            }
        });
    };

    //#endregion

    //#region Common Properties

    $scope.selectCommonProperty = function (property, fromUndoRedoBlock) {
        if ($scope.selectedObject) {
            $scope.selectedObject.IsObjectVisibility = false;
            $scope.selectedObject = null;
        }
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedCommonProperty, $scope, "selectedCommonProperty", property);
        }
        else {
            $scope.selectedCommonProperty = property;
        }
    };
    $scope.addEditCommonProperty = function (commonProperty) {
        var newScope = $scope.$new();
        if (commonProperty) {
            newScope.id = commonProperty.dictAttributes.ID;
            newScope.description = commonProperty.dictAttributes.sfwDescription;
        }
        else {
            newScope.id = "";
            newScope.description = "";
        }
        newScope.validateID = function () {
            newScope.errorMessage = "";
            if (!(newScope.id && newScope.id.trim().length > 0)) {
                newScope.errorMessage = "ID cannot be blank or whitespace(s).";
            }
            else if ($scope.propertiesModel.Elements.some(function (x) { return commonProperty ? (x.dictAttributes.ID == newScope.id && x != commonProperty) : x.dictAttributes.ID == newScope.id; })) {
                newScope.errorMessage = "Duplicate ID.";
            }
        };
        newScope.updateCommonProperty = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (commonProperty) {
                $rootScope.EditPropertyValue(commonProperty.dictAttributes.ID, commonProperty.dictAttributes, "ID", newScope.id);
                $rootScope.EditPropertyValue(commonProperty.dictAttributes.sfwDescription, commonProperty.dictAttributes, "sfwDescription", newScope.description);
            }
            else {
                var property = { Name: "item", Value: "", dictAttributes: { ID: newScope.id, sfwDescription: newScope.description }, Elements: [] };
                $rootScope.PushItem(property, $scope.propertiesModel.Elements);
                $scope.selectCommonProperty(property, true);
            }
            $rootScope.UndRedoBulkOp("End");
            newScope.cancel();
        };
        newScope.cancel = function () {
            if (newScope.dialog && newScope.dialog.close) {
                newScope.dialog.close();
            }
        };
        newScope.dialog = $rootScope.showDialog(newScope, commonProperty ? "Edit Common Property" : "Add Common Property", "Tools/views/AddEditCommonProperty.html", { width: 500, height: 300 });
        newScope.validateID();
    };
    $scope.canDeleteCommonProperty = function () {
        return $scope.selectedCommonProperty;
    };
    $scope.deleteCommonProperty = function () {
        var index = $scope.propertiesModel.Elements.indexOf($scope.selectedCommonProperty);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCommonProperty, $scope.propertiesModel.Elements);

            //Select next item
            if ($scope.propertiesModel.Elements.length > 0) {
                if (index == $scope.propertiesModel.Elements.length) {
                    index--;
                }
                $scope.selectCommonProperty($scope.propertiesModel.Elements[index], true);
            }
            else {
                $scope.selectCommonProperty(null, true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    //#region Common Properties - Objects

    $scope.selectObject = function (object, fromUndoRedoBlock) {
        $scope.selectedCommonProperty = null;
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedObject, $scope, "selectedObject", object);
        }
        else {
            $scope.selectedObject = object;
        }
    };
    $scope.addObject = function () {
        var objofObject = {
            dictAttributes: {},
            Elements: [],
            Children: [],
            Name: "object",
            Value: ""
        };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(objofObject, $scope.objCommonproperties.Elements);
        //objofObject.IsObjectVisibility = true;
        $scope.selectObject(objofObject, true);
        $scope.toggleObjectExpander(objofObject);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteObject = function () {
        return $scope.selectedObject;
    };
    $scope.deleteObject = function () {
        var objects = $scope.objCommonproperties.Elements.filter(function (item) { return item.Name == "object"; });
        if (objects && objects.length > 0) {
            var index = $scope.objCommonproperties.Elements.indexOf($scope.selectedObject);
            var objIndex = objects.indexOf($scope.selectedObject);
            if (index > -1 && objIndex > -1) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.selectedObject, $scope.objCommonproperties.Elements);
                objects = $scope.objCommonproperties.Elements.filter(function (item) { return item.Name == "object"; });

                //Select next item
                if (objects && objects.length > 0) {
                    if (objIndex == objects.length) {
                        objIndex--;
                    }
                    $scope.selectObject(objects[objIndex], true);
                }
                else {
                    $scope.selectObject(null, true);
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    $scope.toggleObjectExpander = function (object, isExpand) {
        if (object) {
            for (var i = 0; i < $scope.objCommonproperties.Elements.length; i++) {
                if ($scope.objCommonproperties.Elements[i].Name == "object") {
                    if ($scope.objCommonproperties.Elements[i] == object) {
                        if (isExpand) {
                            $scope.objCommonproperties.Elements[i].IsObjectVisibility = true;
                        } else {
                            $scope.objCommonproperties.Elements[i].IsObjectVisibility = !$scope.objCommonproperties.Elements[i].IsObjectVisibility;
                        }
                    }
                    else {
                        $scope.objCommonproperties.Elements[i].IsObjectVisibility = false;
                    }
                }
                $scope.onEntityChanged(object.dictAttributes.ID);
            }

            //if ((!$scope.objBusinessObject.ObjTree) ||
            //    (object.IsObjectVisibility &&
            //    object.dictAttributes && object.dictAttributes.ID && object.dictAttributes.ID.trim().length > 0 &&
            //    $scope.objBusinessObject.ObjTree.ObjName != object.dictAttributes.ID)) {
            //    $scope.$evalAsync(function () {
            //        $scope.objBusinessObject.BusObjectName = object.dictAttributes.ID;
            //        $scope.objBusinessObject.ObjTree = undefined;
            //    });
            //}
        }
    };

    //#region Common Properties - Objects - Items
    $scope.selectObjectItem = function (object, item, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue(object.selectedObjectItem, object, "selectedObjectItem", item);
        }
        else {
            object.selectedObjectItem = item;
            $scope.selectedObjectItem = item;
        }
    };
    $scope.addObjectItem = function (object) {
        var item = {
            dictAttributes: {},
            Elements: [],
            Children: [],
            Name: "item",
            Value: ""
        };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, object.Elements);
        $scope.selectObjectItem(object, item, true);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteObjectItem = function (object) {
        return object.selectedObjectItem;
    };
    $scope.deleteObjectItem = function (object) {
        var index = object.Elements.indexOf(object.selectedObjectItem);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem(object.selectedObjectItem, object.Elements);

            //Select next item
            if (object.Elements.length > 0) {
                if (index == object.Elements.length) {
                    index--;
                }
                $scope.selectObjectItem(object, object.Elements[index], true);
            }
            else {
                $scope.selectObjectItem(object, null, true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    $scope.onObjectFieldChanged = function (item) {
        if (item.dictAttributes.sfwEntityField && item.dictAttributes.sfwEntityField.trim().length > 0) {
            item.dictAttributes.sfwObjectMethod = '';
        }
    };
    $scope.onObjectMethodChanged = function (item) {
        if (item.dictAttributes.sfwObjectMethod && item.dictAttributes.sfwObjectMethod.trim().length > 0) {
            item.dictAttributes.sfwEntityField = '';
        }
    };
    $scope.getObjectMethodList = function (entityID, event) {
        //if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
        //    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(event.currentTarget).data('ui-autocomplete')) {
        //        $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
        //        event.preventDefault();
        //    } else {
        //        setSingleLevelAutoComplete($(event.currentTarget), $scope.objBusinessObject.ObjTree.lstMethods, $scope, "ShortName", "Description");
        //    }
        //}
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            event.preventDefault();
        }
        else if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            $scope.onEntityChanged(entityID);
            setSingleLevelAutoComplete($(event.currentTarget), $scope.lstObjectMethod, $scope, "ID", "ID");
            if ($(event.currentTarget).data('ui-autocomplete')) {
                $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            }
            event.preventDefault();
        }
        else if ($(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
        }

    };
    $scope.showObjectMethodList = function (entityID, event) {
        $scope.inputElement = $(event.target).prevAll("input[type='text']");
        if ($scope.inputElement) {
            $($scope.inputElement).focus();
            //if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
            //    setSingleLevelAutoComplete($scope.inputElement, $scope.objBusinessObject.ObjTree.lstMethods, $scope, "ShortName", "Description");
            //    if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            //}
            if ($($scope.inputElement).val() == "") {
                $scope.onEntityChanged(entityID);
            }
            if ($scope.lstObjectMethod && $scope.lstObjectMethod.length > 0) {
                setSingleLevelAutoComplete($scope.inputElement, $scope.lstObjectMethod, $scope, "ID", "ID");
                if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            }
            else {
                $scope.onEntityChanged(entityID);
            }
        }
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.onEntityChanged = function (entityID) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        parententityName = entityID;
        $scope.lstObjectMethod = [];
        while (parententityName) {
            var entity = entityIntellisenseList.filter(function (x) {
                return x.ID == parententityName;
            });
            if (entity.length > 0 && entity[0].ObjectMethods && entity[0].ObjectMethods.length > 0) {
                var tempdata = [];
                for (var i = 0; i < entity[0].ObjectMethods.length; i++) {
                    if (entity[0].ObjectMethods[i].Parameters && entity[0].ObjectMethods[i].Parameters.length == 0 && entity[0].ObjectMethods[i].ReturnType != "Void") {
                        if (entity[0].ObjectMethods[i].ID) {
                            tempdata.push(entity[0].ObjectMethods[i]);
                        }
                    }
                }
                $scope.lstObjectMethod = $scope.lstObjectMethod.concat(tempdata);
            }
            if (entity.length > 0) {
                parententityName = entity[0].ParentId;
            } else {
                parententityName = "";
            }
        }
    };
    //#endregion

    //#endregion

    //#endregion

    //#region Correspondence - Standard Bookmarks

    $scope.selectStandardBookmark = function (stdBookmark, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedStandardBookmark, $scope, "selectedStandardBookmark", stdBookmark);
        }
        else {
            $scope.selectedStandardBookmark = stdBookmark;
        }
    };
    $scope.addStandardBookmark = function () {
        var object = { Name: "object", Value: "", dictAttributes: {}, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(object, $scope.stdBookmarksModel.Elements);
        $scope.selectStandardBookmark(object, true);
        $rootScope.UndRedoBulkOp("End");
        $scope.toggleStandardBookmarkExpander(object);
    };
    $scope.canDeleteStandardBookmark = function () {
        return $scope.selectedStandardBookmark;
    };
    $scope.deleteStandardBookmark = function () {
        var index = $scope.stdBookmarksModel.Elements.indexOf($scope.selectedStandardBookmark);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedStandardBookmark, $scope.stdBookmarksModel.Elements);

            //Select next item
            if ($scope.stdBookmarksModel.Elements.length > 0) {
                if (index == $scope.stdBookmarksModel.Elements.length) {
                    index--;
                }
                $scope.selectStandardBookmark($scope.stdBookmarksModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    $scope.toggleStandardBookmarkExpander = function (stdBookmark, isExpand) {
        if (stdBookmark) {
            for (var i = 0; i < $scope.stdBookmarksModel.Elements.length; i++) {
                if ($scope.stdBookmarksModel.Elements[i] == stdBookmark) {
                    if (isExpand) {
                        $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = true;
                    } else {
                        $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = !$scope.stdBookmarksModel.Elements[i].IsObjectVisibility;
                    }
                }
                else {
                    $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = false;
                }
            }
            $scope.onCorrespondanceEntityChanged(stdBookmark);
            //if ((!$scope.objBusinessObject.ObjTree) ||
            //    (stdBookmark.IsObjectVisibility &&
            //    stdBookmark.dictAttributes && stdBookmark.dictAttributes.ID && stdBookmark.dictAttributes.ID.trim().length > 0 &&
            //    $scope.objBusinessObject.ObjTree.ObjName != stdBookmark.dictAttributes.ID)) {
            //    $scope.$evalAsync(function () {
            //        $scope.objBusinessObject.ObjTree = undefined;
            //        $scope.objBusinessObject.BusObjectName = stdBookmark.dictAttributes.ID;
            //    });
            //}
        }
    };
    $scope.loadXmlMethods = function (object) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entities = $filter('filter')(entityIntellisenseList, { BusinessObjectName: object.dictAttributes.ID }, true);
        if (entities && entities.length > 0)
            object.xmlMethods = $getEnitityXmlMethods.get(entities[0].ID);
    };
    $scope.onCorrespondanceEntityChanged = function (object) {
        if (object) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            parententityName = object.dictAttributes.ID;
            object.xmlMethods = [];
            while (parententityName) {
                var entity = entityIntellisenseList.filter(function (x) {
                    return x.ID == parententityName;
                });
                if (entity.length > 0 && entity[0].XmlMethods && entity[0].XmlMethods.length > 0) {
                    object.xmlMethods = object.xmlMethods.concat(entity[0].XmlMethods);
                }
                if (entity.length > 0) {
                    parententityName = entity[0].ParentId;
                } else {
                    parententityName = "";
                }
            }
        }
        else {
            object.xmlMethods = [];
        }
    };
    //#region Standard Bookmarks - Items
    $scope.selectStandardBookmarkItem = function (parentBookmark, stdBookmarkItem, fromUndoRedoBlock) {
        if (parentBookmark) {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(parentBookmark.selectedStandardBookmarkItem, parentBookmark, "selectedStandardBookmarkItem", stdBookmarkItem);
                parentBookmark.selectedStandardBookmarkItem = stdBookmarkItem;
            }
            else {
                parentBookmark.selectedStandardBookmarkItem = stdBookmarkItem;
            }
            $scope.selectedStandardBookmarkItem = stdBookmarkItem;
        }
    };
    $scope.addStandardBookmarkItem = function (parentBookmark) {
        if (parentBookmark) {
            var item = { Name: "item", Value: "", dictAttributes: {}, Elements: [] };
            $scope.AddOrEditStandardBookMark(parentBookmark, item, "Add");
        }
    };
    $scope.canDeleteStandardBookmarkItem = function (parentBookmark) {
        return parentBookmark.selectedStandardBookmarkItem;
    };
    $scope.deleteStandardBookmarkItem = function (parentBookmark) {
        var index = parentBookmark.Elements.indexOf(parentBookmark.selectedStandardBookmarkItem);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem(parentBookmark.selectedStandardBookmarkItem, parentBookmark.Elements);

            //Select next item
            if (parentBookmark.Elements.length > 0) {
                if (index == parentBookmark.Elements.length) {
                    index--;
                }
                $scope.selectStandardBookmarkItem(parentBookmark.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.AddOrEditStandardBookMark = function (objBookMarkParent, objStdBookMark, param) {
        var newStdBookMarkScope = $scope.$new();
        newStdBookMarkScope.objBookMark = {};
        newStdBookMarkScope.objBookMark.Entity = objBookMarkParent.dictAttributes.ID;
        if (objStdBookMark) {
            newStdBookMarkScope.objBookMark.id = objStdBookMark.dictAttributes.ID;
            newStdBookMarkScope.objBookMark.EntityField = objStdBookMark.dictAttributes.sfwEntityField;
        }
        else {
            newStdBookMarkScope.objBookMark.id = "";
            newStdBookMarkScope.objBookMark.EntityField = "";
        }
        newStdBookMarkScope.errorMessage = "";

        newStdBookMarkScope.validateID = function () {
            newStdBookMarkScope.errorMessage = "";
            if (!(newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.trim().length > 0)) {
                newStdBookMarkScope.errorMessage = "ID cannot be blank or whitespace(s).";
            }
            else if (newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.substring(0, 3) != "std") {
                newStdBookMarkScope.errorMessage = "ID should start with 'std' prefix .";
            }
            else if (newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.substring(0, 3) == "std" && newStdBookMarkScope.objBookMark.id.length == 3) {
                newStdBookMarkScope.errorMessage = "'std' is prefix , Enter valid ID.";
            }
            else if ($scope.stdBookmarksModel && $scope.stdBookmarksModel.Elements.length) {
                for (var i = 0; i < $scope.stdBookmarksModel.Elements.length; i++) {
                    for (var j = 0; j < $scope.stdBookmarksModel.Elements[i].Elements.length; j++) {
                        if ($scope.stdBookmarksModel.Elements[i].Elements[j] != objStdBookMark && newStdBookMarkScope.objBookMark.id == $scope.stdBookmarksModel.Elements[i].Elements[j].dictAttributes.ID) {
                            newStdBookMarkScope.errorMessage = "Duplicate ID.";
                            break;
                        }
                    }
                    if (newStdBookMarkScope.errorMessage) {
                        break;
                    }
                }
            }
        };
        newStdBookMarkScope.cancel = function () {
            if (newStdBookMarkScope.dialog && newStdBookMarkScope.dialog.close) {
                newStdBookMarkScope.dialog.close();
            }
        };

        newStdBookMarkScope.updateStandardBookMark = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (param == "Add") {
                objStdBookMark.dictAttributes.ID = newStdBookMarkScope.objBookMark.id;
                objStdBookMark.dictAttributes.sfwEntityField = newStdBookMarkScope.objBookMark.EntityField;
                $rootScope.PushItem(objStdBookMark, objBookMarkParent.Elements);
                $scope.selectStandardBookmarkItem(objBookMarkParent, objStdBookMark, true);
            } else if (param == "Edit") {
                $rootScope.EditPropertyValue(objStdBookMark.dictAttributes.ID, objStdBookMark.dictAttributes, 'ID', newStdBookMarkScope.objBookMark.id);
                $rootScope.EditPropertyValue(objStdBookMark.dictAttributes.sfwEntityField, objStdBookMark.dictAttributes, 'sfwEntityField', newStdBookMarkScope.objBookMark.EntityField);
            }
            $rootScope.UndRedoBulkOp("End");
            newStdBookMarkScope.cancel();
        };
        var Title = "";
        if (param == "Edit") {
            Title = "Edit Standard Book Mark";
        } else {
            Title = "Add Standard Book Mark";
        }
        newStdBookMarkScope.dialog = $rootScope.showDialog(newStdBookMarkScope, Title, "Tools/views/AddEditStandardBookMark.html", { width: 500, height: 300 });
        newStdBookMarkScope.validateID();
    }
    //#endregion

    //#endregion

    //#region Server Methods
    $scope.clearServerMethodFilter = function () {
        $scope.serverMethodFilters.isServerMethodFilterVisible = false;
    };
    $scope.selectAndScrollSrvMethod = function (methodObj, fromUndoRedoBlock) {
        $scope.getCurrentSrvMethod(methodObj, fromUndoRedoBlock);
        $timeout(function () {
            var selectedItem = $("#server-method-list").find(".searchrowSelected");
            if (selectedItem && selectedItem.length) {
                selectedItem[0].scrollIntoView();
            }
        });
    };
    $scope.getCurrentSrvMethod = function (methodObj, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.currentSrvMethod, $scope, "currentSrvMethod", methodObj);
            $rootScope.EditPropertyValue($scope.currentMethod, $scope, "currentMethod", null);
        }
        else {
            $scope.currentSrvMethod = methodObj;
            $scope.currentMethod = null;
        }
        $scope.clearServerMethodFilter();
    };

    $scope.selectAndScrollRemoteMethod = function (methodObj, fromUndoRedoBlock) {
        $scope.getCurrentMethod(methodObj, fromUndoRedoBlock);
        $timeout(function () {
            var selectedItem = $("#remote-method-list").find(".searchrowSelected");
            if (selectedItem && selectedItem.length) {
                selectedItem[0].scrollIntoView();
            }
        });
    };
    $scope.getCurrentMethod = function (methods, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.currentMethod, $scope, "currentMethod", methods);
        }
        else {
            $scope.currentMethod = methods;
        }
    };

    // ======================== referesh server methods start ==============================================

    $scope.refreshAllRemoteObjects = function () {
        /// <summary>test</summary>
        // check for classes - if not present delete        
        if ($scope.objServerMethods.Elements.length > 0) {
            $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
            var indexofSelectedMethodGroup = $scope.objServerMethods.Elements.indexOf($scope.currentSrvMethod);
            for (var i = $scope.objServerMethods.Elements.length - 1; i >= 0; i--) {
                if ($scope.remoteObjCollection.indexOf($scope.objServerMethods.Elements[i].dictAttributes.ID) < 0) {
                    if (indexofSelectedMethodGroup >= i) {
                        indexofSelectedMethodGroup -= 1;
                    }
                    $scope.isDirty = true;
                    $scope.objServerMethods.Elements.splice(i, 1);
                }
            }
            if ($scope.objServerMethods.Elements.length > 0) {
                var isSelectedGroupExist = false;
                // loop on classes and check methods are present - if not present delete
                $scope.objServerMethods.Elements.forEach(function (remoteObject) {
                    if ($scope.currentSrvMethod && ($scope.currentSrvMethod.dictAttributes.ID == remoteObject.dictAttributes.ID)) {
                        isSelectedGroupExist = true;
                    }
                    $scope.refreshRemoteObject(remoteObject, true);
                });
                // if selected object does not exist - select the one at its original index or the first one from the remaining list
                if (!isSelectedGroupExist) {
                    $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[indexofSelectedMethodGroup] ? $scope.objServerMethods.Elements[indexofSelectedMethodGroup] : $scope.objServerMethods.Elements[0]);
                }
            }
            else {
                $scope.getCurrentSrvMethod(undefined);
            }
        }
    };

    $scope.refreshRemoteObject = function (remoteObject, isGlobalRefresh) {
        $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
        var indexofSelectedMethod = remoteObject.Elements.indexOf($scope.currentMethod);
        $rootScope.IsLoading = true;
        $.connection.hubMain.server.getRemoteObjectMethods(remoteObject.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    if (data.length > 0) {
                        for (var i = remoteObject.Elements.length - 1; i >= 0; i--) {
                            function filterRemotemethods(Rmethods, index, remoteMethods) {
                                if (Rmethods.MethodName == remoteObject.Elements[i].dictAttributes.ID) {
                                    return true;
                                }
                            }
                            if (!data.some(filterRemotemethods)) {
                                if (indexofSelectedMethod >= i) {
                                    indexofSelectedMethod -= 1;
                                }
                                $scope.isDirty = true;
                                remoteObject.Elements.splice(i, 1);
                            }
                        }
                        if (remoteObject.Elements.length > 0) {
                            var isSelectedMethodExist = false;
                            // if method present check for parameters - and replace
                            remoteObject.Elements.forEach(function (localRemoteMethod) {
                                // if any method is selected - preserve the selection
                                if ($scope.currentMethod && (localRemoteMethod.dictAttributes.ID == $scope.currentMethod.dictAttributes.ID)) {
                                    isSelectedMethodExist = true;
                                }
                                $scope.refreshRemoteMethod(remoteObject.dictAttributes.ID, localRemoteMethod);
                            });
                            if (isGlobalRefresh) {
                                $scope.getCurrentMethod(undefined);
                            }
                            else if (!isSelectedMethodExist) {
                                $scope.getCurrentMethod(remoteObject.Elements[indexofSelectedMethod] ? remoteObject.Elements[indexofSelectedMethod] : remoteObject.Elements[0]);
                            }

                        }
                        else {
                            $scope.getCurrentMethod(undefined);
                        }
                    }
                    // remoteobject does not have any method
                    else {
                        $scope.isDirty = true;
                        for (var i = remoteObject.Elements.length - 1; i >= 0; i--) {
                            remoteObject.Elements.splice(i, 1);
                        }
                        $scope.getCurrentMethod(undefined);
                    }
                }
                else {
                    toastr.warning(remoteObject.dictAttributes.ID + " is not a valid method group");
                }
                $rootScope.IsLoading = false;
            });
        });
    };

    $scope.refreshRemoteMethod = function (remoteObjectID, localRemoteMethod) {
        if ($rootScope.currentopenfile) {
            $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
        }
        $rootScope.IsLoading = true;
        $.connection.hubMain.server.getRemoteMethod(remoteObjectID, localRemoteMethod.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.isDirty = true;
                    localRemoteMethod.Elements = [];
                    for (var i = 0; i < data.Parameters.length; i++) {
                        var newParameters = { Name: 'parameter', Value: '', dictAttributes: { ID: data.Parameters[i].ParameterName, sfwDataType: data.Parameters[i].ParameterType.FullName }, Elements: [] };
                        localRemoteMethod.Elements.push(newParameters);
                    }
                }
                else {
                    toastr.warning(localRemoteMethod.dictAttributes.ID + " is not a valid method");
                }
                $rootScope.IsLoading = false;
            });
        });
    };

    // ======================== referesh server methods ends ===========================================

    // Add method for selected server method
    $scope.OpenAddMethodWindow = function (methodGrp) {
        var newScope = $scope.$new();
        $.connection.hubMain.server.getRemoteObjectMethods($scope.currentSrvMethod.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                newScope.lstRemoteObjMethod = data;
            });
        });

        newScope.templateUrl = "Tools/views/AddServerMethod.html";
        newScope.methodMessage = "Please select method.";

        newScope.OnOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            var newMethod = {
                Name: 'method', Value: '', dictAttributes: { ID: newScope.selectedMethodName.MethodName, sfwMode: newScope.sfwMode }, Elements: []
            };
            angular.forEach(newScope.lstParameters, function (param, key) {
                var newParameters = { Name: 'parameter', Value: '', dictAttributes: { ID: param.ParameterName, sfwDataType: param.ParameterType.FullName }, Elements: [] };
                $rootScope.PushItem(newParameters, newMethod.Elements);
                newParameters = {};
            });
            if (newScope.selectedMethodName && newScope.sfwMode) {
                angular.forEach($scope.objServerMethods.Elements, function (method, key) {
                    if (method.dictAttributes.ID == $scope.currentSrvMethod.dictAttributes.ID) {
                        $rootScope.PushItem(newMethod, method.Elements);
                        newScope.OnCancelClick();
                        //console.log("Before add method", $scope.objServerMethods);
                    }
                });
            }
            $scope.getCurrentMethod(newMethod, true);
            $rootScope.UndRedoBulkOp("End");
        };

        newScope.OnCancelClick = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };


        newScope.getParameters = function () {
            newScope.lstParameters = newScope.selectedMethodName.Parameters;
        };

        newScope.validateServerMethod = function () {
            newScope.methodMessage = "";
            if (!newScope.selectedMethodName) {
                newScope.methodMessage = "Please select method.";
                return true;
            }
            else if (!newScope.sfwMode) {
                newScope.methodMessage = "Please select Mode.";
                return true;
            }
            else if (newScope.currentSrvMethod) {
                var flag = false;
                if ($scope.currentSrvMethod.Elements.length == 0) {
                    return false;
                }
                angular.forEach($scope.currentSrvMethod.Elements, function (method, key) {
                    if (method.dictAttributes.ID == newScope.selectedMethodName.MethodName) {
                        newScope.methodMessage = "This method already exists.";
                        flag = true;
                    }
                });
                return flag;
            }
            else {
                return false;
            }
        };

        if (newScope.templateUrl && newScope.templateUrl.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, "Add Method", newScope.templateUrl, { width: 500, height: 300 });
        }
    };

    // Add server methods
    $scope.OpenAddMethodGroup = function () {
        var newScope = $scope.$new();
        newScope.templateUrl = "Tools/views/AddServerMethodGroup.html";
        newScope.remoteObjCollection = angular.copy($scope.remoteObjCollection);
        // method group which are not already present can be added
        function filterRemoteMethodGroup(methodGroup) {
            var numIndexRemoteObject = newScope.remoteObjCollection.indexOf(methodGroup.dictAttributes.ID);
            if (numIndexRemoteObject >= 0) {
                newScope.remoteObjCollection.splice(numIndexRemoteObject, 1);
            }
        }
        $scope.objServerMethods.Elements.forEach(filterRemoteMethodGroup);
        newScope.srvMethodMessage = "Please select method.";
        newScope.validData = true;
        newScope.OnOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (newScope.selectedMethodGroup) {
                var newSrvMethod = { Name: "methodgroup", Value: '', dictAttributes: { ID: newScope.selectedMethodGroup }, Elements: [] };
                $rootScope.PushItem(newSrvMethod, $scope.objServerMethods.Elements);
                newScope.OnCancelClick();
            }
            $scope.getCurrentSrvMethod(newSrvMethod, true);
            $rootScope.UndRedoBulkOp("End");
        };

        newScope.OnCancelClick = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };

        newScope.checkMethodGroupValidity = function (obj) {
            newScope.validData = false;
            angular.forEach($scope.objServerMethods.Elements, function (method, key) {
                if (method.dictAttributes.ID == newScope.selectedMethodGroup) {
                    newScope.srvMethodMessage = "This server method already exists.";
                    newScope.validData = true;
                    return false;
                }
            });
        };

        if (newScope.templateUrl && newScope.templateUrl.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, "Add Method Group", newScope.templateUrl, { width: 500, height: 300 });
        }
    };

    // Delete server method group
    $scope.DeleteMethodGroup = function () {
        var index = $scope.objServerMethods.Elements.indexOf($scope.currentSrvMethod);
        if (index > -1) {
            toastr.success("Server group deleted Successfully");
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.currentSrvMethod, $scope.objServerMethods.Elements);
            //Select next item
            if ($scope.objServerMethods.Elements.length > 0) {
                if (index == $scope.objServerMethods.Elements.length) {
                    index--;
                }
                $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    // Delete Single method 
    $scope.DeleteMethod = function () {
        var index = $scope.currentSrvMethod.Elements.indexOf($scope.currentMethod);
        if (index > -1) {
            toastr.success("Method deleted Successfully");
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.currentMethod, $scope.currentSrvMethod.Elements);
            //Select next item
            if ($scope.currentSrvMethod.Elements.length > 0) {
                if (index == $scope.currentSrvMethod.Elements.length) {
                    index--;
                }
                $scope.getCurrentMethod($scope.currentSrvMethod.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.removeOldID = function (parameter) {
        $rootScope.EditPropertyValue(parameter.dictAttributes.OldID, parameter.dictAttributes, 'OldID', "");
    };

    //#endregion

    //#region Global Parameters

    $scope.selectGlobalParameter = function (globalParameter, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedGlobalParameter, $scope, "selectedGlobalParameter", globalParameter);
        }
        else {
            $scope.selectedGlobalParameter = globalParameter;
        }
    };
    $scope.addGlobalParameter = function () {
        var item = { Name: "globlarparameter", Value: "", dictAttributes: {}, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, $scope.globalParametersModel.Elements);
        $scope.selectGlobalParameter(item, true);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteGlobalParameter = function () {
        return $scope.selectedGlobalParameter;
    };
    $scope.deleteGlobalParameter = function () {
        var index = $scope.globalParametersModel.Elements.indexOf($scope.selectedGlobalParameter);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedGlobalParameter, $scope.globalParametersModel.Elements);
            //Select next item
            if ($scope.globalParametersModel.Elements.length > 0) {
                if (index == $scope.globalParametersModel.Elements.length) {
                    index--;
                }
                $scope.selectGlobalParameter($scope.globalParametersModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    //#endregion

    //#region Portals

    $scope.selectPortal = function (portal, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedPortal, $scope, "selectedPortal", portal);
        }
        else {
            $scope.selectedPortal = portal;
        }
    };
    $scope.addPortal = function () {
        var item = { Name: "portal", Value: "", dictAttributes: { sfwPortalType: "MVVM" }, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, $scope.portalsModel.Elements);
        $scope.selectPortal(item, true);
        $rootScope.UndRedoBulkOp("End");
        $scope.validatePortals();
    };
    $scope.canDeletePortal = function () {
        return $scope.selectedPortal;
    };
    $scope.deletePortal = function () {
        var index = $scope.portalsModel.Elements.indexOf($scope.selectedPortal);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedPortal, $scope.portalsModel.Elements);
            //Select next item
            if ($scope.portalsModel.Elements.length > 0) {
                if (index == $scope.portalsModel.Elements.length) {
                    index--;
                }
                $scope.selectPortal($scope.portalsModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
        $scope.validatePortals();
    };
    $scope.validatePortal = function (portal) {
        if (portal.dictAttributes.ID) {
            if ($scope.portalsModel.Elements.some(function (itm) { return itm.dictAttributes.ID === portal.dictAttributes.ID && itm !== portal; })) {
                if (!portal.errors) {
                    portal.errors = {};
                }
                portal.errors.ID = "Duplicate Portal ID. Please provide a different Portal ID.";
            }
            else if (portal.errors) {
                portal.errors.ID = null;
            }
        }
        else {
            if (!portal.errors) {
                portal.errors = {};
            }
            portal.errors.ID = "Portal ID cannot be empty.";
        }

        if (portal.dictAttributes.sfwPortalName) {
            if ($scope.portalsModel.Elements.some(function (itm) { return itm.dictAttributes.sfwPortalName === portal.dictAttributes.sfwPortalName && itm !== portal; })) {
                if (!portal.errors) {
                    portal.errors = {};
                }
                portal.errors.sfwPortalName = "Duplicate Portal Name. Please provide a different Portal Name.";
            }
            else if (portal.errors) {
                portal.errors.sfwPortalName = null;
            }
        }
        else {
            if (!portal.errors) {
                portal.errors = {};
            }
            portal.errors.sfwPortalName = "Portal Name cannot be empty.";
        }

    }
    $scope.validatePortals = function () {
        for (var index = 0, len = $scope.portalsModel.Elements.length; index < len; index++) {
            $scope.validatePortal($scope.portalsModel.Elements[index]);
        }
    }
    //#endregion

    $scope.initialize();
}]);