app.controller("ConstantsController", ["$scope", "$filter", "$rootScope", "$timeout", "$resourceFactory", "CONSTANTS", "$ValidationService", "$Errors", "$EntityIntellisenseFactory", function ($scope, $filter, $rootScope, $timeout, constantFactory, CONST, $ValidationService, $Errors, $EntityIntellisenseFactory) {
    $scope.Groups = [];
    $scope.lstColumnNames = [];
    $scope.EntityQueryCollection = [];
    $scope.FilesCollection = [];
    $scope.IsFilesLoaded = false;
    $scope.IsEntityQueriesLoaded = false;
    $scope.parentGroupObj = null;
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.IsBaseAppFile = $scope.currentfile && $scope.currentfile.IsBaseAppFile;
    $scope.lstFilesCollection = [];
    $scope.CreateDuplicateModel = function (model) {
        var objModel;
        if (model) {
            objModel = {
                Children: model.Children, dictAttributes: model.dictAttributes, Elements: [], IsExpanded: false, isSelected: false,
                IsValueInCDATAFormat: model.IsValueInCDATAFormat, Name: model.Name, objExtraData: model.objExtraData, prefix: model.prefix, Value: model.Value
                , IsBaseAppModel: model.IsBaseAppModel, BaseAppModel: model.BaseAppModel
            };

            for (var i = 0; i < model.Elements.length; i++) {
                objModel.Elements.push($scope.CreateDuplicateModel(model.Elements[i]));
            }
        }
        return objModel;
    };
    var constants = angular.copy(constantFactory.getConstantModelData());
    $scope.constantfilemodel = $scope.CreateDuplicateModel(constants);
    $scope.entityQueryGroup = $scope.constantfilemodel.Elements.filter(function (x) { return x.Name === "groups" && x.dictAttributes.GroupType === "EntityGroup"; })[0];

    $scope.getconstants = function (Model) {
        $scope.objDummyConstants = Model;
        if ($scope.objDummyConstants != undefined) {
            $scope.Groups = [];

            $scope.objDummyConstants.IsExpanded = true;
            // $scope.objDummyConstants.isSelected = true;

            $scope.Groups.push($scope.objDummyConstants);
            //setWatchForDirtyFlag($scope, $scope.objDummyConstants);

            $scope.SelectedConstant = $scope.objDummyConstants;
            toggleSelectionOfConstants();

            $scope.PopulateDataTypes();
            ComponentsPickers.init();
        }
    };
    //Xml source Start
    $scope.showSource = false;
    $scope.selectedDesignSource = false;

    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {

            var xmlstring = $scope.editor.getValue();

            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;

                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;

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

                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Source-Design", lineNumber);
                }
            }
        }
    };
    $scope.receivedesignxmlobject = function (data, path) {

        $scope.selectedDesignSource = false;
        if ($scope.isSourceDirty) {
            $scope.constantfilemodel = data;
            $scope.getconstants($scope.constantfilemodel);
            constantFactory.setConstantsList($scope.constantfilemodel);
            $scope.isSourceDirty = false;
        }

        //$scope.receiveRuleConstantsModel(data);

        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;

            if (path != null && path != "") {
                var items = [];
                var objHierarchy;
                if (path.contains("-") || path.contains(",")) {
                    objHierarchy = $scope.objDummyConstants;
                    for (var i = 0; i < path.split(',').length; i++) {
                        objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                        items.push(objHierarchy);
                    }
                }

                if (items.length > 1) {
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].Name == "groups") {
                            items[i].IsExpanded = true;
                        }
                        if (i == 0) {
                            $scope.SelectConstantClick(items[items.length - 1], window.event);
                        }
                    }
                }
                else if (items.length == 1) {
                    $scope.SelectConstantClick(items[0], window.event);
                }
            }
        });
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
        $timeout(function () {
            var elem = $("#" + $rootScope.currentopenfile.file.FileName).find(".constant-tree-view .selected");
            if (elem) {
                $('.constant-tree-view').scrollTo(elem, null, null);
            }
        });
    };

    $scope.selectElement = function (path) {
        $scope.receivedesignxmlobject($scope.constantfilemodel, path);
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

            if ($scope.objDummyConstants != null && $scope.objDummyConstants != undefined) {
                $rootScope.IsLoading = true;
                var objreturn1 = GetBaseModel($scope.objDummyConstants);
                var strobj = JSON.stringify(objreturn1);

                var nodeId = [];
                if ($scope.objDummyConstants && $scope.SelectedConstant) {
                    //sObj = FindDeepNode($scope.objDummyConstants, $scope.SelectedConstant);
                    //pathString = getPathSource(sObj, indexPath);
                    //angular.copy(pathString.reverse(), nodeId);

                    var pathToObject = [];

                    sObj = $scope.FindDeepNode($scope.objDummyConstants, $scope.SelectedConstant, pathToObject);
                    pathString = $scope.getPathSource($scope.objDummyConstants, pathToObject, indexPath);
                    angular.copy(pathString, nodeId);
                }

                if (strobj.length < 32000) {
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
                        var ID = "constants";
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

    $scope.updateEntitySelection = function (isFromUndoRedoBlock) {
        if ($scope.entityQueryGroup) {
            for (var i = 0; i < $scope.EntityQueryCollection.length; i++) {
                var value = $scope.entityQueryGroup.Elements.some(function (ent) { return ent.dictAttributes.ID === $scope.EntityQueryCollection[i].Name; });
                if (isFromUndoRedoBlock) {
                    $rootScope.EditPropertyValue($scope.EntityQueryCollection[i].IsRuleSelected, $scope.EntityQueryCollection[i], "IsRuleSelected", value);
                }
                else {
                    $scope.EntityQueryCollection[i].IsRuleSelected = value;
                }
            }
        }
    };
    $scope.LoadEntityQueries = function (onLoad) {
        var compareObjectByID = function (obj1, obj2) {
            if (obj1.ID < obj2.ID) {
                return -1;
            }
            if (obj1.ID > obj2.ID) {
                return 1;
            }
            return 0;
        };
        var lstEntities = $EntityIntellisenseFactory.getEntityIntellisense().sort(compareObjectByID);

        //Remove entity from list, which are not present in the main list.
        var removedEntities = $scope.EntityQueryCollection.filter(function (ent) { return !lstEntities.some(function (x) { return ent.Name === x.ID; }); });
        for (var i = 0; i < removedEntities.length; i++) {
            var idx = $scope.EntityQueryCollection.indexOf(removedEntities[i]);
            if (idx > -1) {
                $scope.EntityQueryCollection.splice(idx, 1);
            }
        }

        for (var entityIndex = 0; entityIndex < lstEntities.length; entityIndex++) {
            var entity = lstEntities[entityIndex];

            //Check if entity not present in the list then add it.
            var objEntity = $scope.EntityQueryCollection.filter(function (ent) { return ent.Name === entity.ID; })[0];
            if (!objEntity) {
                objEntity = { Name: entity.ID, IsRuleSelected: false, Childs: [] };
                $scope.EntityQueryCollection.push(objEntity);
            }

            var lstQueries = entity.Queries.sort(compareObjectByID);

            //Remove query from entity, which are not present in the main list.
            var removedQueries = objEntity.Childs.filter(function (qr) { return !lstQueries.some(function (x) { return qr.Name === x.ID; }); });
            for (var i = 0; i < removedQueries.length; i++) {
                var idx = objEntity.Childs.indexOf(removedQueries[i]);
                if (idx > -1) {
                    objEntity.Childs.splice(idx, 1);
                }
            }

            for (var queryIndex = 0; queryIndex < lstQueries.length; queryIndex++) {
                var query = lstQueries[queryIndex];

                //Check if query not present in the list then add it.
                if (!objEntity.Childs.some(function (qr) { return qr.Name === query.ID; })) {
                    var objQuery = { Name: query.ID, IsRuleSelected: false, Childs: [] };
                    objEntity.Childs.push(objQuery);
                }
            }
        }
        if (onLoad) {
            $scope.updateEntitySelection();
        }
    };
    $scope.LoadFilesList = function () {
        $.connection.hubConstants.server.getFilesList().done(function (data) {
            $scope.$apply(function () {
                $scope.FilesCollection = data;
                var count = 0;
                for (var i = 0; i < $scope.FilesCollection.length; i++) {
                    count++;
                    if (count == 100) {
                        break;
                    } else {
                        $scope.lstFilesCollection.push($scope.FilesCollection[i]);
                    }
                }
                $scope.IsFilesLoaded = true;
            });
        });
    };

    $scope.BeforeSaveToFile = function () {
        var ObjModel = $scope.CreateDuplicateModel($scope.objDummyConstants);
        angular.copy(ObjModel, constantFactory.getConstantModelData());
    };

    $scope.PopulateDataTypes = function () {
        $scope.DataTypes = [];
        $scope.DataTypes.push("string");
        $scope.DataTypes.push("bool");
        $scope.DataTypes.push("decimal");
        $scope.DataTypes.push("double");
        $scope.DataTypes.push("float");
        $scope.DataTypes.push("int");
        $scope.DataTypes.push("long");
        $scope.DataTypes.push("short");
    };

    $scope.LoadParentForObject = function (objParent, item) {
        toggleSelectionOfConstants();
        if (objParent != undefined) {
            item.ParentGroupType = objParent.dictAttributes.GroupType;
            item.ParentVM = objParent;
            if (item.Name == "constant") {
                item.IsShowEffectiveDate = true;

                if (item.ParentGroupType == "Query" || item.ParentGroupType == "Files" || item.ParentGroupType == "Entity") {
                    item.IsShowEffectiveDate = false;
                }
                else if (objParent.dictAttributes.IsChildGroup == "True") {
                    item.IsShowEffectiveDate = false;
                }
                else if (objParent.ParentGroupType == "Query" || objParent.ParentGroupType == "Files" || objParent.ParentGroupType == "EntityQueries") {
                    item.IsShowEffectiveDate = false;
                }
            }
        }
        $timeout(function () {
            ComponentsPickers.init();
        });
    };

    $scope.SelectConstantClick = function (obj, event) {
        $scope.SelectedConstant = obj;
        toggleSelectionOfConstants();
        $scope.objParentGroup = $scope.GetParent(obj, $scope.constantfilemodel);
        if ($scope.SelectedConstant.dictAttributes.GroupType === "EntityGroup") {
            $scope.updateEntitySelection();
        }
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.SetConstantClass = function (object, step) {
        if (object == step) {
            //var element = document.getElementsByClassName("selectedconstant");
            //if (element.length > 0)
            //{
            //    //element[0].parentNode.getElementsByClassName("file-collapsed").click();
            //}
            return "selectedconstant";
        }
    };

    $scope.AddEffectiveDateClick = function () {
        if ($scope.SelectedConstant != undefined) {
            var obj = {
                Name: "item", dictAttributes: { Value: '' }, Elements: [], isSelected: true
            };
            $scope.currentSelectedRow(undefined);
            $rootScope.PushItem(obj, $scope.SelectedConstant.Elements);
            $scope.SelectedEffectiveDate = $scope.SelectedConstant.Elements[$scope.SelectedConstant.Elements.length - 1];
            $timeout(function () {
                ComponentsPickers.init();
            });
            $scope.validateDataType($scope.SelectedEffectiveDate, $scope.SelectedConstant.dictAttributes.sfwDataType);
        }
    };

    $scope.DeleteEffectiveDateClick = function () {
        if ($scope.SelectedConstant != undefined && $scope.SelectedEffectiveDate && $scope.SelectedConstant.Elements.length > 0) {
            var index = $scope.SelectedConstant.Elements.indexOf($scope.SelectedEffectiveDate);
            $rootScope.DeleteItem($scope.SelectedEffectiveDate, $scope.SelectedConstant.Elements, "");
            $ValidationService.removeObjInToArray($scope.validationErrorList, $scope.SelectedEffectiveDate);
            if (index < $scope.SelectedConstant.Elements.length) {
                $scope.currentSelectedRow($scope.SelectedConstant.Elements[index]);
            } else if ($scope.SelectedConstant.Elements.length > 0) {
                $scope.currentSelectedRow($scope.SelectedConstant.Elements[index - 1]);
            }
        }
        else {
            toastr.info("Please Select the Effective Date.");
        }
    };

    $scope.currentSelectedRow = function (selectedItem) {
        angular.forEach($scope.SelectedConstant.Elements, function (item, key) {
            item.isSelected = false;
        });
        if (selectedItem) {
            selectedItem.isSelected = true;
            $scope.SelectedEffectiveDate = selectedItem;
        }
    };

    $scope.ExpandCollapsedConstant = function (obj, $event) {
        obj.IsExpanded = !obj.IsExpanded;
        //expandCollapseConstants($event);
    };

    $scope.expandByFind = function (ParentList) {
        for (var j = 0; j < ParentList.length; j++) {
            if (ParentList[j].Name != "constant") {
                ParentList[j].IsExpanded = true;
            }
        }
    };

    $scope.ResetGroupQueryDetail = function (values) {
        if (values.indexOf($scope.SelectedConstant.dictAttributes.QueryFieldID) === -1) {            
            $scope.SelectedConstant.dictAttributes.QueryFieldID = "";
        }
        if (values.indexOf($scope.SelectedConstant.dictAttributes.QueryFieldValue) === -1) {
            $scope.SelectedConstant.dictAttributes.QueryFieldValue = "";
        }
        if (values.indexOf($scope.SelectedConstant.dictAttributes.QueryFieldDescription) === -1) {
            $scope.SelectedConstant.dictAttributes.QueryFieldDescription = "";
        }
        if (values.indexOf($scope.SelectedConstant.dictAttributes.ClassName) === -1) {
            $scope.SelectedConstant.dictAttributes.ClassName = "";
        }
    };

    $scope.GetQuerySchemaClick = function () {
        $.connection.hubConstants.server.getQuerySchema($scope.SelectedConstant.dictAttributes.sfwQuery).done(function (data) {
            $scope.$apply(function () {
                if (data) {
                    $scope.ResetGroupQueryDetail(data);
                    $scope.lstColumnNames = data;
                }
                else {
                    alert('Function getQuerySchema didnot return data.');
                }
            });
        });
    };

    $scope.onQueryFieldKeyDown = function (event) {
        var input = event.currentTarget;

        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            if ($scope.lstColumnNames && $scope.lstColumnNames.length > 0) {
                $(input).autocomplete("search", $(input).val().trim());
            }
            event.preventDefault();
        }

        if ($scope.lstColumnNames && $scope.lstColumnNames.length > 0) {
            if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                setSingleLevelAutoComplete($(input), $scope.lstColumnNames);
            }
        }
        if ($scope.lstColumnNames && $scope.lstColumnNames.length == 0) setSingleLevelAutoComplete($(input), $scope.lstColumnNames);
    };

    $scope.showQueryList = function (event) {
        var inputElement = $(event.target).prevAll("input[type='text']");
        inputElement.focus();

        if ($scope.lstColumnNames && $scope.lstColumnNames.length > 0) {
            setSingleLevelAutoComplete($(inputElement), $scope.lstColumnNames);
            if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val().trim());
        }
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.CanContainsValue = function (value) {
        var objItem;
        $scope.lstColumnNames.filter(function (item) {
            if (item == value) {
                objItem = item;
            }
        });

        if (objItem != undefined) {
            return true;
        }
        else
            return false;
    };

    $scope.ValidateData = function () {
        var IsValid = true;
        if ($scope.SelectedConstant.dictAttributes.sfwQuery != undefined && $scope.SelectedConstant.dictAttributes.sfwQuery != "") {
            var strMessage = "";
            if ($scope.SelectedConstant.dictAttributes.QueryFieldID == undefined || $scope.SelectedConstant.dictAttributes.QueryFieldID == "") {
                IsValid = false;
                strMessage = "Enter the ID Field Name.";
            }
            else if (!$scope.CanContainsValue($scope.SelectedConstant.dictAttributes.QueryFieldID)) {
                IsValid = false;
                strMessage = "Invalid ID Field Name.";
            }
            else if ($scope.SelectedConstant.dictAttributes.QueryFieldValue == undefined || $scope.SelectedConstant.dictAttributes.QueryFieldValue == "") {
                IsValid = false;
                strMessage = "Enter the Value Field Name.";
            }
            else if (!$scope.CanContainsValue($scope.SelectedConstant.dictAttributes.QueryFieldValue)) {
                IsValid = false;
                strMessage = "Invalid Value Field Name.";
            }
            else if ($scope.SelectedConstant.dictAttributes.QueryFieldDescription != undefined && $scope.SelectedConstant.dictAttributes.QueryFieldDescription != "" && !$scope.CanContainsValue($scope.SelectedConstant.dictAttributes.QueryFieldDescription)) {
                IsValid = false;
                strMessage = "Invalid Description Field Name.";
            }
            else if ($scope.SelectedConstant.dictAttributes.ClassName != undefined && $scope.SelectedConstant.dictAttributes.ClassName != "" && !$scope.CanContainsValue($scope.SelectedConstant.dictAttributes.ClassName)) {
                IsValid = false;
                strMessage = "Invalid ClassName Field Name.";
            }

            if (strMessage != undefined && strMessage != "") {
                alert(strMessage);
            }
        }

        return IsValid;
    };

    $scope.ImportQueryClick = function () {
        if (!$scope.SelectedConstant != undefined) {
            if ($scope.ValidateData()) {
                $rootScope.IsLoading = true;
                $.connection.hubConstants.server.importQueryInConstant($scope.SelectedConstant.dictAttributes.sfwQuery, $scope.SelectedConstant.dictAttributes.ClassName, $scope.SelectedConstant.dictAttributes.QueryFieldValue,
                    $scope.SelectedConstant.dictAttributes.QueryFieldID, $scope.SelectedConstant.dictAttributes.QueryFieldDescription).done(function (data) {
                        $scope.$apply(function () {
                            if (data) {
                                $rootScope.UndRedoBulkOp("Start");

                                if (!$scope.IsBaseAppFile) {
                                    while ($scope.SelectedConstant.Elements.length > 0) {
                                        $rootScope.DeleteItem($scope.SelectedConstant.Elements[0], $scope.SelectedConstant.Elements);
                                    }
                                }
                                //$scope.OnClearItemClick();

                                var lst = data;

                                angular.forEach(lst, function (item) {
                                    if (!$scope.SelectedConstant.Elements.some(function (x) { return item.dictAttributes.ID === x.dictAttributes.ID; })) {
                                        $rootScope.PushItem(item, $scope.SelectedConstant.Elements);
                                    }
                                });
                                $rootScope.UndRedoBulkOp("End");
                            }
                            else {
                                alert('Function importQueryInConstant didnot return data.');
                            }
                            $rootScope.IsLoading = false;
                        });
                    });

            }
        }
    };

    $scope.ImportFilesClick = function () {
        $rootScope.UndRedoBulkOp("Start");
        $scope.OnClearItemClick();
        if ($scope.lstFilesCollection.length > 0) {
            angular.forEach($scope.lstFilesCollection, function (file) {
                if (file.IsRuleSelected) {
                    var objConstant = {
                        Name: "constant", Value: '', dictAttributes: { ID: file.Name, Value: file.Name, sfwDataType: "string" }, Elements: []
                    };
                    $rootScope.PushItem(objConstant, $scope.SelectedConstant.Elements);
                    objConstant.ParentGroupType = $scope.SelectedConstant.dictAttributes.GroupType;
                    // $scope.SelectedConstant.Elements.push(objConstant);
                    file.IsRuleSelected = false;
                }
            });
        }
        $rootScope.UndRedoBulkOp("End");
    };

    $scope.SelectAllFilesClick = function (isSelectAll) {
        if (isSelectAll) {
            if ($scope.FilesCollection.length > 0) {
                angular.forEach($scope.FilesCollection, function (file) {
                    file.IsRuleSelected = true;
                });
            }
        }
        else {
            if ($scope.FilesCollection.length > 0) {
                angular.forEach($scope.FilesCollection, function (file) {
                    file.IsRuleSelected = false;
                });
            }
        }

    };

    $scope.ImportEntityQueryClick = function () {
        $rootScope.UndRedoBulkOp("Start");
        $scope.OnClearItemClick();
        if ($scope.EntityQueryCollection.length > 0) {
            angular.forEach($scope.EntityQueryCollection, function (entity) {
                if (entity.IsRuleSelected) {
                    var objGroup = {
                        Name: "groups", Value: '', dictAttributes: { ID: entity.Name, GroupType: "Manual" }, Elements: []
                    };
                    $scope.AddSelectedEntityQueries(entity.Childs, objGroup);

                    if (objGroup.Elements.length > 0) {
                        $rootScope.PushItem(objGroup, $scope.SelectedConstant.Elements);
                        // $scope.SelectedConstant.Elements.push(objGroup);
                    }
                    entity.IsRuleSelected = false;
                }
                else {
                    if (entity.Childs.length > 0) {
                        var objGroup = {
                            Name: "groups", Value: '', dictAttributes: { ID: entity.Name, GroupType: "Manual" }, Elements: []
                        };

                        var lstRules = [];
                        angular.forEach(entity.Childs, function (query) {
                            if (query.IsRuleSelected) {
                                $rootScope.PushItem(query, lstRules);
                                //lstRules.push(query);
                                query.IsRuleSelected = false;
                            }
                        });

                        if (lstRules.length > 0) {
                            $scope.AddSelectedEntityQueries(lstRules, objGroup);
                            $rootScope.PushItem(objGroup, $scope.SelectedConstant.Elements);
                            //$scope.SelectedConstant.Elements.push(objGroup);
                        }
                    }
                }
            });
        }
        $rootScope.UndRedoBulkOp("End");

    };

    $scope.AddSelectedEntityQueries = function (Childs, objGroup) {
        angular.forEach(Childs, function (query) {
            var objConstant = {
                Name: "constant", Value: '', dictAttributes: { ID: query.Name, Value: query.Name, sfwDataType: "string" }, Elements: []
            };
            $rootScope.PushItem(objConstant, objGroup.Elements);
            objConstant.ParentGroupType = $scope.SelectedConstant.dictAttributes.GroupType;
            // objGroup.Elements.push(objConstant);
        });
    };

    $scope.OnClearItemClick = function () {
        if ($scope.SelectedConstant) {
            if ($scope.SelectedConstant.dictAttributes) {
                $rootScope.EditPropertyValue($scope.SelectedConstant.dictAttributes.ClassName, $scope.SelectedConstant.dictAttributes, "ClassName", "");
                $rootScope.EditPropertyValue($scope.SelectedConstant.dictAttributes.sfwQuery, $scope.SelectedConstant.dictAttributes, "sfwQuery", "");
                $rootScope.EditPropertyValue($scope.SelectedConstant.dictAttributes.QueryFieldID, $scope.SelectedConstant.dictAttributes, "QueryFieldID", "");
                $rootScope.EditPropertyValue($scope.SelectedConstant.dictAttributes.QueryFieldValue, $scope.SelectedConstant.dictAttributes, "QueryFieldValue", "");
                $rootScope.EditPropertyValue($scope.SelectedConstant.dictAttributes.QueryFieldDescription, $scope.SelectedConstant.dictAttributes, "QueryFieldDescription", "");


                $scope.lstColumnNames = [];
            }
            while ($scope.SelectedConstant.Elements.length > 0) {
                $rootScope.DeleteItem($scope.SelectedConstant.Elements[0], $scope.SelectedConstant.Elements);
                //$scope.SelectedConstant.Elements.splice(0, 1);
            }
            if ($scope.SelectedConstant.dictAttributes.GroupType === "EntityGroup") {
                $scope.updateEntitySelection(true);
            }
        }
    };
    $scope.parentList = [];
    $scope.GetParent = function (objConstant, objParentConstants) {
        var parent;
        if (objParentConstants) {
            //$scope.parentList = [];
            angular.forEach(objParentConstants.Elements, function (item) {

                if (parent == undefined) {
                    if (item.dictAttributes.ID == objConstant.dictAttributes.ID && item.Name == objConstant.Name && item == objConstant) {
                        parent = objParentConstants;

                        $scope.parentList.push(parent);
                        return;
                    }
                    else {
                        parent = $scope.GetParent(objConstant, item);
                        $scope.parentList.push(parent);
                        if (parent != undefined) {
                            return;
                        }
                    }
                }
            });
        }
        return parent;
    };

    $scope.OpenAddWindow = function (strType, parent) {
        var newScope = $scope.$new();
        var TemplateHeader = "";
        newScope.Type = strType;
        if (strType == "groups") {
            TemplateHeader = "Group";
        } else if (strType === "entitygroup") {
            TemplateHeader = "Entity Query Group";
        }
        else {
            TemplateHeader = "Constant";
        }
        newScope.Parent = parent;
        newScope.templateName = "Constants/views/AddConstantTemplate.html";
        newScope.objConstant = {
            Name: newScope.Type, Value: '', dictAttributes: {}, Elements: [], ParentVM: parent
        };
        $scope.checkConstantName(newScope.objConstant.dictAttributes.ID, newScope.Parent);
        newScope.OnOkClick = function () {
            if (newScope.Type == "groups") {
                newScope.objConstant.dictAttributes.GroupType = "Manual";
            }
            if (newScope.objConstant != undefined && (newScope.objConstant.dictAttributes.ID != undefined || newScope.objConstant.dictAttributes.ID != "")) {
                $rootScope.PushItem(newScope.objConstant, newScope.Parent.Elements);
                if (!newScope.Parent.IsExpanded) {
                    newScope.Parent.IsExpanded = true;
                }
                $scope.SelectConstantClick(newScope.objConstant, window.event);
                // $scope.SelectedConstant = newScope.objConstant;
                $scope.SelectedConstant.IsShowEffectiveDate = true;
                toggleSelectionOfConstants();
                $timeout(function () {
                    var elem = $("#" + $rootScope.currentopenfile.file.FileName).find(".constant-tree-view .selected");
                    if (elem) {
                        $('.constant-tree-view').scrollTo(elem, null, null);
                    }
                });
            }
            $scope.validateConstants($scope.SelectedConstant.ParentVM, false, $scope.SelectedConstant.ParentVM, $scope.SelectedConstant.ParentVM);
            newScope.OnCancelClick();
        };

        newScope.OnCancelClick = function () {
            $scope.ErrorMessage = "";
            $scope.IsError = false;
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };
        if (strType === "entitygroup") {
            var override = true;
            var objQuery = newScope.Parent.Elements.filter(function (x) { return x.Name === 'groups' && x.dictAttributes.ID === "Query"; })[0];
            if (objQuery) {
                if (confirm("Already a group name 'Query' exists, do you want to override it?")) {
                    $rootScope.DeleteItem(objQuery, newScope.Parent.Elements);
                }
                else {
                    override = false;
                }
            }
            if (override) {
                newScope.objConstant.dictAttributes.ID = "Query";
                newScope.objConstant.Name = "groups";
                newScope.objConstant.dictAttributes.GroupType = "EntityGroup";
                $scope.entityQueryGroup = newScope.objConstant;
                newScope.OnOkClick();
            }
        }
        else if (newScope.templateName && newScope.templateName.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, "Add " + TemplateHeader, newScope.templateName, { height: 200, width: 400 });
        }
    };

    $scope.checkConstantName = function (currentElement, parent) {
        $scope.ErrorMessage = "";
        // $scope.validateID(parent); // validate constant ID
        $scope.IsError = false;
        if (!currentElement) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Error:ID can not be empty.";
        }
        else if (!isValidIdentifier(currentElement, false, false)) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Error:Invalid ID.";
        }
        else if (parent.ParentVM && parent.ParentVM.Elements) {
            parent.ParentVM.Elements.filter(function (item) {
                if (item.dictAttributes.ID && currentElement && item.dictAttributes.ID.toLowerCase() == currentElement.toLowerCase() && item != parent) {
                    $scope.IsError = true;
                    $scope.ErrorMessage = "Error:Duplicate ID.";
                }
                if (parent.ParentVM.dictAttributes.ID && currentElement && parent.ParentVM.dictAttributes.ID.toLowerCase() == currentElement.toLowerCase() && item != parent.ParentVM) {
                    $scope.IsError = true;
                    $scope.ErrorMessage = "Error:Duplicate ID.";
                }
            });
        }
        return $scope.IsError;
    };

    ///Constant Context Menu
    $scope.menuOptionsForConstant = [
        ['Add Entity Queries Group', function ($itemScope) {
            $scope.OpenAddWindow("entitygroup", $itemScope.itm);
        }, function ($itemScope) {
            if ($itemScope.itm === $scope.constantfilemodel) {
                if (!$itemScope.itm.Elements.some(function (x) { return x.Name === "groups" && x.dictAttributes.GroupType === "EntityGroup"; })) {
                    return true;
                }
            }
            return false;
        }], null,
        ['Add Groups', function ($itemScope) {
            $scope.OpenAddWindow("groups", $itemScope.itm);
        }, function ($itemScope) {
            var parentGroup = $scope.GetParent($itemScope.itm, $scope.constantfilemodel);
            if (parentGroup) {
                $scope.LoadParentForObject(parentGroup, $itemScope.itm);
            }
            $scope.SelectConstantClick($itemScope.itm);
            if ($itemScope.itm.Name == "groups" && $itemScope.itm.dictAttributes.GroupType == "Manual") {
                if ($itemScope.itm.ParentGroupType == undefined || $itemScope.itm.ParentGroupType == "Manual") {
                    return true;
                }
                return false;
            }
            return false;
        }], null,
        ['Add Constant', function ($itemScope) {
            $scope.OpenAddWindow("constant", $itemScope.itm);
        }, function ($itemScope) {
            if ($itemScope.itm.Name == "constant") {
                return false;
            }
            if ($itemScope.itm.Name == "groups" && $itemScope.itm.dictAttributes.GroupType == "Manual") {
                if ($itemScope.itm.dictAttributes.IsChildGroup == "True") {
                    return false;
                }
                if ($itemScope.itm.ParentGroupType == undefined || $itemScope.itm.ParentGroupType == "Manual") {
                    return true;
                }
                else {
                    return false;
                }
                return true;
            }

        }], null,
        ['Delete', function ($itemScope) {
            var objParent = $scope.GetParent($itemScope.itm, $scope.objDummyConstants);
            if (objParent != undefined) {
                var index = objParent.Elements.indexOf($itemScope.itm);
                $rootScope.DeleteItem($itemScope.itm, objParent.Elements);
                $scope.removeInvalidObject($itemScope.itm);
                $ValidationService.removeObjInToArray($scope.validationErrorList, $itemScope.itm);
                // objParent.Elements.splice(index, 1);

                if (index < objParent.Elements.length) {
                    $scope.SelectedConstant = objParent.Elements[index];
                    $scope.LoadParentForObject(objParent, $scope.SelectedConstant);
                    $scope.SelectConstantClick($scope.SelectedConstant);
                }
                else if (objParent.Elements.length > 0) {
                    $scope.SelectedConstant = objParent.Elements[index - 1];
                    $scope.LoadParentForObject(objParent, $scope.SelectedConstant);
                    $scope.SelectConstantClick($scope.SelectedConstant);
                }
            }
        }, function ($itemScope) {
            if ($itemScope.itm.IsBaseAppModel || $itemScope.itm.BaseAppModel) {
                return false;
            }

            var objParent = $scope.GetParent($itemScope.itm, $scope.objDummyConstants);
            if ($itemScope.itm.Name == "constant") {
                if ($itemScope.itm.ParentGroupType == "Query" || $itemScope.itm.ParentGroupType == "Files" || $itemScope.itm.ParentGroupType == "EntityQueries") {
                    return false;
                }
                else if (objParent != undefined && (objParent.dictAttributes.IsChildGroup != undefined && objParent.dictAttributes.IsChildGroup.toLowerCase() == "true")) {
                    return false;
                }
                return true;
            }
            if ($itemScope.itm.Name === "groups") {
                if (objParent == undefined) {
                    return false;
                }
                if ($itemScope.itm.dictAttributes.IsChildGroup == "True") {
                    return false;
                }
                else if (objParent != undefined && (objParent.dictAttributes.GroupType == "Files" || objParent.dictAttributes.GroupType == "EntityQueries")) {
                    return false;
                }
                return true;
            }
        }], null,
        ['Clear', function ($itemScope) {
            $rootScope.UndRedoBulkOp("Start");
            $scope.OnClearItemClick();
            $scope.validateFileData(false, null);
            $rootScope.UndRedoBulkOp("End");

        }, function ($itemScope) {
            if ($itemScope.itm.IsBaseAppModel || $itemScope.itm.BaseAppModel) {
                return false;
            }

            var objParent = $scope.GetParent($itemScope.itm, $scope.objDummyConstants);
            if ($itemScope.itm.Name == "groups") {
                if ($itemScope.itm.dictAttributes.IsChildGroup == "True") {
                    return false;
                }
                if ($itemScope.itm.Elements.length == 0) {
                    return false;
                }
                else if (objParent != undefined && (objParent.dictAttributes.GroupType == "Files" || objParent.dictAttributes.GroupType == "EntityQueries")) {
                    return false;
                }
                return true;
            }
        }], null,
        ['Revert', function ($itemScope) {
            $scope.revertToBaseAppModel($itemScope.itm);
        }, function ($itemScope) {
            return !$itemScope.itm.IsBaseAppModel && $itemScope.itm.BaseAppModel
        }], null
    ];
    ///End Constant Context Menu

    $scope.removeInvalidObject = function (paramObj) {
        if (paramObj && paramObj.Elements.length > 0) {
            angular.forEach(paramObj.Elements, function (obj) {
                $ValidationService.removeObjInToArray($scope.validationErrorList, obj);
                if (obj && angular.isArray(obj.Elements) && obj.Elements.length > 0) {
                    $scope.removeInvalidObject(obj);
                }
            });
        }
    };
    // Group  Type Change method
    $scope.GroupTypeChangeClick = function () {
        if ($scope.SelectedConstant != undefined) {
            if ($scope.SelectedConstant.dictAttributes.GroupType == "Manual") {
                $rootScope.UndRedoBulkOp("Start");
                $scope.OnClearItemClick();
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    // End Group  Type Change method

    //// *************** Refresh Groups Methods ***************** ///

    $scope.onRefreshAllGroups = function () {
        if ($scope.isDirty) {
            toastr.info("Please hit Save before refresh.");
        }
        else {
            $.connection.hubConstants.server.onRefreshAllGroups();
            $rootScope.IsLoading = true;
        }
    };

    $scope.receieveConstantsAfterRefresh = function (data, errMsgs) {
        $scope.setValidationList();
        $scope.isDirty = true;
        var constantfilemodel = JSON.parse(data);
        var lstErrMsg = "";
        for (var i = 0; i < errMsgs.length; i++) {
            lstErrMsg += (i + 1) + ") " + errMsgs[i] + "\n";
        }
        if (lstErrMsg != "" && lstErrMsg) {
            var newScope = $scope.$new();
            newScope.strMessage = lstErrMsg;
            newScope.isError = true;
            dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html");
            newScope.OkClick = function () {
                dialog.close();
            };
        }

        $scope.getconstants(constantfilemodel);
        $scope.constantfilemodel = constantfilemodel;
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };

    $scope.CountofConstants = 0;
    $scope.ConstantSearchID = "";
    var strConstantSearchID = "";
    $scope.onEnterKeySearch = function (e) {
        if (e.which == 13) {
            $scope.FindConstants($scope.ConstantSearchID);
        }
    };
    $scope.onTextChangeinConstant = function () {
        toggleSelectionOfConstants();
        $scope.lstConstants = [];
        $scope.CountofConstants = 0;
    };
    $scope.lstConstants = [];

    $scope.currentGroup;
    $scope.FindConstants = function (strConstantSearchID) {
        if ($scope.ConstantSearchID && strConstantSearchID) {
            $scope.CountofConstants = 0;
            $scope.newParentList = {};
            for (var i = 0; i < $scope.Groups.length; i++) {
                $scope.currentGroup = i;

                $scope.SearchConstantsWithID($scope.Groups[i], strConstantSearchID);
            }
            $scope.FindNextConstant();
        }
    };

    $scope.newParentList = {};
    $scope.currentPath = [];
    $scope.SearchConstantsWithID = function (groups, strConstantSearchID) {
        for (var i = 0; i < groups.Elements.length; i++) {
            if (groups.Elements[i].dictAttributes.ID != undefined) {
                if (groups.Elements[i].dictAttributes.ID.toLowerCase().contains(strConstantSearchID.toLowerCase()) || (groups.Elements[i].dictAttributes.Value && groups.Elements[i].dictAttributes.Value.toLowerCase().contains(strConstantSearchID.toLowerCase()))) {
                    var tempObj = groups.Elements[i];
                    tempObj.ParentVM = $scope.newParentList;
                    tempObj.isSelected = false;
                    $scope.lstConstants.push(tempObj);
                    tempObj = {};
                }

                if (groups.Elements[i].Elements.length > 0) {
                    $scope.newParentList = {};
                    $scope.newParentList = groups.Elements[i];
                    CreateParentVM($scope.newParentList);
                    $scope.SearchConstantsWithID(groups.Elements[i], strConstantSearchID);
                }
            }
        }
    };

    var CreateParentVM = function (parentList) {
        if (parentList.Elements.length > 0) {
            for (var i = 0; i < parentList.Elements.length; i++) {
                if (parentList.Elements[i].Name == "groups") {
                    parentList.Elements[i].ParentVM = parentList;
                    if (parentList.Elements[i].Elements.length > 0) {
                        CreateParentVM(parentList.Elements[i]);
                    }
                }
            }
        }
    };

    var toggleSelectionOfConstants = function () {
        if ($scope.SelectedConstant && !$scope.SelectedConstant.isSelected) {
            if ($scope.SelectedConstant) {
                $scope.SelectedConstant.isSelected = true;
            }
            if ($scope.PrevSelectedConstant) {
                $scope.PrevSelectedConstant.isSelected = false;
            }
            $scope.PrevSelectedConstant = $scope.SelectedConstant;
        }

    };

    $scope.FindNextConstant = function () {

        if ($scope.lstConstants.length > 0 && $scope.CountofConstants < $scope.lstConstants.length) {
            $scope.SelectedConstant = $scope.lstConstants[$scope.CountofConstants];
            toggleSelectionOfConstants();
            var currentObjectParent = $scope.lstConstants[$scope.CountofConstants];
            $scope.newParentList = [];

            while (currentObjectParent != undefined && currentObjectParent != "") {
                $scope.newParentList.push(currentObjectParent);
                currentObjectParent = currentObjectParent.ParentVM;
            }

            $scope.expandByFind($scope.newParentList);
            $timeout(function () {
                var elem = $("#" + $rootScope.currentopenfile.file.FileName).find(".constant-tree-view .selected");
                if (elem) {
                    $('.constant-tree-view').scrollTo(elem, null, null);
                }
            });
            $scope.CountofConstants++;
        }
        else {
            toastr.info("Finished searching,reached end of file");
            $scope.CountofConstants = 0;
            toggleSelectionOfConstants();
            $scope.lstConstants = [];
        }
    };

    $scope.togglebuttonForTreeView = function (item) {
        item.IsExpanded = !item.IsExpanded;
    };

    $scope.LoadSomeFiles = function () {
        var count = 0;
        for (var i = $scope.lstFilesCollection.length; i < $scope.FilesCollection.length; i++) {
            count++;
            if (count == 15) {
                break;
            }
            else {
                $scope.lstFilesCollection.push($scope.FilesCollection[i]);
            }
        }
    };

    $scope.LoadFilesDependingOnText = function () {
        for (var i = $scope.lstFilesCollection.length; i < $scope.FilesCollection.length; i++) {
            $scope.lstFilesCollection.push($scope.FilesCollection[i]);
        }
    };

    $scope.getconstants($scope.constantfilemodel);

    $scope.setValidationList = function () {
        $scope.validationErrorList = [];
        if ($Errors.validationListObj && angular.isArray($Errors.validationListObj)) {
            var checkErrObj = $filter('filter')($Errors.validationListObj, { FileName: $rootScope.currentopenfile.file.FileName });
            if (checkErrObj.length == 0) $Errors.validationListObj.push({ FileName: $rootScope.currentopenfile.file.FileName, errorList: [] });
            var fileErrObj = $filter('filter')($Errors.validationListObj, { FileName: $rootScope.currentopenfile.file.FileName })[0];
            $scope.validationErrorList = fileErrObj.errorList = [];
        }
    };
    $scope.validateFileData = function (isOnload, eventType) {
        $scope.setValidationList();
        if ($scope.constantfilemodel) {
            $scope.validateConstants($scope.constantfilemodel, isOnload, $scope.constantfilemodel, $scope.constantfilemodel);
            $scope.checkDuplicateId($scope.constantfilemodel);
            if (isOnload && eventType != 'change') {
                $timeout(function () {
                    toastr.success("Constants validated successfully.");
                }, 1000);
            }
        }
    };

    $scope.checkDuplicateId = function (mainModel) {
        if (mainModel && mainModel.Name == "groups") {
            angular.forEach(mainModel.Elements, function (obj) {

                //angular.forEach(obj.Elements, function (constObj) {
                if (obj.Name != "item") {
                    $scope.validateID(obj);
                }
                $ValidationService.checkDuplicateId(obj, mainModel, $scope.validationErrorList, false, ["item"]);
                if (obj.dictAttributes.ID && mainModel.dictAttributes.ID && obj.dictAttributes.ID == mainModel.dictAttributes.ID && obj != mainModel) {
                    if (!obj.errors && !angular.isObject(obj.errors)) {
                        obj.errors = {};
                    }
                    var errorMessage = CONST.VALIDATION.DUPLICATE_ID;
                    if (obj.errors && !obj.errors.duplicate_id) obj.errors.duplicate_id = errorMessage;
                    if ($scope.validationErrorList.indexOf(obj) <= -1) $scope.validationErrorList.push(obj);
                    if (mainModel.errors && !mainModel.errors.duplicate_id) mainModel.errors.duplicate_id = errorMessage;
                    if ($scope.validationErrorList.indexOf(mainModel) <= -1) $scope.validationErrorList.push(mainModel);
                }
                //  });

                if (obj.Elements.length > 0 && obj.Name == "groups") {
                    $scope.checkDuplicateId(obj);
                }
            });
        }
    };
    var parentConstantObj = null;
    $scope.validateConstants = function (model, isOnload, parentConstantObj, parentGroupObj) {
        angular.forEach(model.Elements, function (obj) {
            if (obj && obj.dictAttributes.hasOwnProperty("sfwEffectiveDate") && obj.dictAttributes.sfwEffectiveDate) {
                if (isOnload) {
                    if (obj.errors && obj.errors["sfwEffectiveDate"]) {
                        delete obj.errors["sfwEffectiveDate"];
                    }
                    if (obj.errors && !$ValidationService.isEmptyObj(obj.errors)) {
                        $ValidationService.removeObjInToArray($scope.validationErrorList, obj);
                    }
                }
                $ValidationService.findDuplicateDates(parentConstantObj, obj, $scope.validationErrorList, "sfwEffectiveDate");
            }
            if (obj.Name == "constant") {
                if (!obj.dictAttributes.sfwDataType) {
                    $ValidationService.checkValidListValue($scope.DataTypes, obj, obj.dictAttributes.sfwDataType, "sfwDataType", "sfwDataType", CONST.VALIDATION.INVALID_METHOD, $scope.validationErrorList);
                }
            }
            if (obj.Name == "constant" && parentGroupObj.dictAttributes.GroupType == "Manual") {
                $scope.validateDataType(obj, obj.dictAttributes.sfwDataType);
            }
            if (obj.Elements.length > 0) {
                if (obj.Name == "constant") {
                    parentConstantObj = obj;
                }
                if (obj.Name == "groups") {
                    parentGroupObj = obj;
                }
                $scope.validateConstants(obj, isOnload, parentConstantObj, parentGroupObj);
            }
        });
    };
    $scope.validateDataType = function (obj, dataType) {
        if (obj.Name == "constant") {
            $ValidationService.checkValidListValue($scope.DataTypes, obj, obj.dictAttributes.sfwDataType, "sfwDataType", "sfwDataType", CONST.VALIDATION.EMPTY_DATATYPE, $scope.validationErrorList, true);
        }
        if (obj) {
            $ValidationService.validateDataTypes(obj, obj.dictAttributes.Value, dataType, $scope.validationErrorList);
            if (obj.Elements.length > 0) {
                angular.forEach(obj.Elements, function (itemObj) {
                    $scope.validateDataType(itemObj, dataType);
                });
            }
        }
        $scope.updateModelForBaseAppModel();
    };
    $scope.validateID = function (obj) {
        if (obj) {
            $ValidationService.validateID(obj, $scope.validationErrorList, obj.dictAttributes.ID);
        }
    }

    $scope.findParentAndChildObject = function (selectedItem) {
        var findList = [];
        $scope.FindDeepNode($scope.constantfilemodel, selectedItem, findList);
        $scope.$evalAsync(function () {
            $scope.selectConstant(findList);
        });
    };

    $scope.selectConstant = function (items) {
        if (items.length > 1) {
            for (var i = 0; i < items.length; i++) {
                if (items[i].Name == "groups") {
                    items[i].IsExpanded = true;
                }
                if (items[i].Name != "item") {
                    var item = items[i];
                    var nextItem = items[i + 1];
                    $scope.SelectConstantClick(item, window.event);

                    if (item && item.Name != "groups" && items.length == 2) {
                        item = $scope.constantfilemodel;
                        nextItem = items[i];
                    }
                    if (item && nextItem) {
                        $scope.LoadParentForObject(item, nextItem);
                    }
                }
                if (items[i].Name == "item") {
                    $scope.currentSelectedRow(items[i]);
                    $timeout(function () {
                        var domItem = $(".constants-items-table").find(".selected").first();
                        if (domItem) {
                            $('.constants-items-table').scrollTo(domItem, { offsetTop: 300, offsetLeft: 0 }, null);
                        }
                    });
                }
            }
        }
        else if (items.length == 1) {
            $scope.SelectConstantClick(items[0], window.event);
        }
        $timeout(function () {
            var elem = document.getElementsByClassName("selected")[0];
            if (elem) {
                $('.constant-tree-view').scrollTo(elem, null, null);
            }
        });
    };
    $scope.PopulateDataTypes();
    $scope.setValidationList();
    //$scope.validateFileData(false);

    $scope.traverseMainModel = function (path) {
        var items = [];
        var objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.constantfilemodel;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                if (objHierarchy) {
                    items.push(objHierarchy);
                    if (objHierarchy.ParentID && (objHierarchy.ParentID == "softerror" || objHierarchy.ParentID == "harderror")) {
                        var obString = path.split(',');
                        var ruleString = obString[obString.length - 1].substring(0, obString[obString.length - 1].indexOf('-'));
                        if (objHierarchy.Children.length > 0 && ruleString == "rule") {
                            objHierarchy = FindChidrenHierarchy(objHierarchy, path.split(',')[i + 2].substring(path.split(',')[i + 2].lastIndexOf('-') + 1));
                            items.push(objHierarchy);
                            break;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
        }
        return items;
    };
    $scope.updateModelForBaseAppModel = function (updateParentName) {
        if ($scope.IsBaseAppFile && $scope.constantfilemodel && $scope.constantfilemodel.BaseAppModel) {
            $scope.$evalAsync(function () {
                updateModelForBaseAppModel($scope.constantfilemodel, $scope.constantfilemodel.BaseAppModel, updateParentName);
            });
        }
    };
    $scope.revertToBaseAppModel = function (model) {
        $scope.$evalAsync(function () {
            revertToBaseAppModel(model);
        });
    };

    $scope.LoadEntityQueries(true);
    $scope.onSelectAllEntities = function () {
        for (var i = 0; i < $scope.EntityQueryCollection.length; i++) {
            $scope.EntityQueryCollection[i].IsRuleSelected = $scope.isAllEntitiesSelected;
        }
    };
    $scope.updateEntityQueriesGroup = function () {
        $scope.LoadEntityQueries();
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue($scope.entityQueryGroup.Elements, $scope.entityQueryGroup, "Elements", []);
        var selectedEntities = $scope.EntityQueryCollection.filter(function (x) { return x.IsRuleSelected; });
        for (var entityIndex = 0; entityIndex < selectedEntities.length; entityIndex++) {
            var entity = selectedEntities[entityIndex];
            entityNode = {
                Name: "groups", dictAttributes: { ID: entity.Name, GroupType: "Entity" }, Elements: []
            };
            $rootScope.PushItem(entityNode, $scope.entityQueryGroup.Elements);
            $scope.updateEntityQueries(entityNode, entity);
        }
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.updateEntityQueries = function (group, entity) {
        if (!entity) {
            $scope.LoadEntityQueries();
        }
        if (group && group.dictAttributes.GroupType === 'Entity') {
            var objEntity = null;
            if (entity) {
                objEntity = entity;
            }
            else {
                $rootScope.UndRedoBulkOp("Start");
                objEntity = $scope.EntityQueryCollection.filter(function (x) { return x.Name === group.dictAttributes.ID; })[0];
            }
            $rootScope.EditPropertyValue(group.Elements, group, "Elements", []);
            if (objEntity) {
                for (var queryIndex = 0; queryIndex < objEntity.Childs.length; queryIndex++) {
                    var query = objEntity.Childs[queryIndex];
                    var queryNode = {
                        Name: "constant", dictAttributes: { ID: query.Name, Value: group.dictAttributes.ID + "." + query.Name, sfwDataType: "string" }, Elements: []
                    };
                    $rootScope.PushItem(queryNode, group.Elements);
                }
            }
            if (!entity) {
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };

}]);

function expandCollapseConstants(event) {
    var element = event.target;
    var tables = $(element).parents("table");
    if (tables.length > 0) {
        $(tables[0]).siblings("ul").slideToggle();
        // $(element).toggleClass("file-expanded file-collapsed");
    }
}

app.directive('scrollIf', ["$window", function ($window) {
    return {
        restrict: 'AE',
        scope: {
            val: '@'
        },
        link: function (scope, element, attributes) {

            scope.$watch('val', function (newValue, oldValue) {
                if (newValue == "true") {
                    var elWithTop = element[0];

                    while (elWithTop.offsetTop == 0) {
                        elWithTop = elWithTop.parentNode;
                    }

                    $("#dvScrollList").scrollTo(elWithTop, null, null);
                }
            }, true);
        }
    };
}]);

app.directive('treeviewconstants', ['$compile', function ($compile) {
    var getTreeView = function (item, element) {
        var template = ["<ul class='marg-left-20'>",
            "<li ng-repeat=\"itm in items.Elements | orderBy:'dictAttributes.ID'\" tabindex='0' key-up-down-for-tree callbackfn='selectConstant' objproperty='\"itm\"' objparent='items'>",
            "<i style='float:left' ng-click='toggleCondition(itm)' ng-if=\"itm.Elements.length > 0 && itm.Name !='constant'\" ng-class=\"itm.IsExpanded?'constants-tree-minus':'constants-tree-plus'\"></i>",
            "<span style='position:relative;'><span class='info-tooltip dtc-span' ng-if='itm.errors.empty_id || itm.errors.inValid_id || itm.errors.duplicate_id' ng-attr-title='{{itm.errors.empty_id || itm.errors.inValid_id || itm.errors.duplicate_id}}'><i style='color:red !important;' class='fa fa-exclamation-circle'></i></span></span>",
            "<span ng-class=\"itm.isSelected?'selected':''\" class='constant-tree-child-label' ng-click='selectConstant(itm,items,$event)' context-menu='menuoptionforconstant'>{{itm.dictAttributes.ID}}",
            "<span ng-if=\"itm.Name =='constant'\"> = </span> <span ng-if=\"itm.Name =='constant'\" style='color:blue'> {{itm.dictAttributes.Value}} </span>",
            "</span>",
            "<span treeviewconstants  items='itm' ng-if='itm.IsExpanded' menuoptionforconstant='menuoptionforconstant'/>",
            "</li>",
            "</ul>"].join(' ');
        return template;
    };

    return {
        restrict: "A",
        scope: {
            items: '=',
            menuoptionforconstant: "=",
        },
        link: function (scope, element, attributes) {
            scope.toggleCondition = function (itm) {
                itm.IsExpanded = !itm.IsExpanded;
            };

            scope.selectConstant = function (itm, items, event) {
                var Controllerscope = getCurrentFileScope();
                Controllerscope.SelectedConstant = itm;
                if (items) {
                    Controllerscope.LoadParentForObject(items, itm);
                }
                if (Controllerscope.SelectConstantClick) {
                    Controllerscope.SelectConstantClick(Controllerscope.SelectedConstant);
                }
                //if (Controllerscope.SelectedConstant && Controllerscope.SelectedConstant.dictAttributes.ID) {
                //    Controllerscope.checkConstantName(Controllerscope.SelectedConstant.dictAttributes.ID, Controllerscope.SelectedConstant);
                //}

                //scope.mainconstantgroup.SelectedConstant = itm;
                if (event) {
                    event.stopPropagation();
                }
            };
            if (angular.isArray(scope.items.Elements)) {
                element.html(getTreeView(scope.items, element));
                $compile(element.contents())(scope);
            }
        }
    };
}]);

app.directive('scrolldirectiveforfiles', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            console.log('loading directive');
            var scrollTopValue = 0;
            element.bind('scroll', function () {
                if (raw.scrollTop + raw.offsetHeight > raw.scrollHeight && raw.scrollTop > scrollTopValue) { //at the bottom
                    scrollTopValue = raw.scrollTop - 10;
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    var scope = getCurrentFileScope();
                    scope.$apply(function () {
                        scope.LoadSomeFiles();
                    });
                }
            });
        }

    };
});