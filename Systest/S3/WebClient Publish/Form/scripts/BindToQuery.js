app.controller("BindToQueryController", ["$scope", "$rootScope", "ngDialog", "$GetEntityFieldObjectService", "$Entityintellisenseservice", "$getModelList", "$EntityIntellisenseFactory", function ($scope, $rootScope, ngDialog, $GetEntityFieldObjectService, $Entityintellisenseservice, $getModelList, $EntityIntellisenseFactory) {
    $scope.lstQueryLoadType = ["Solution", "New and Update", "New", "Update"];
    $scope.IsGridChildOfListView = false;
    $scope.entityName = $scope.formobject.dictAttributes.sfwEntity;

    $scope.getParentEntityName = function (model) {
        if ($scope.formobject && model) {
            if (model.dictAttributes.sfwParentGrid) {
                var parentGrid = FindControlByID($scope.formobject, model.dictAttributes.sfwParentGrid);
                var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
                if (objParentField) {
                    $scope.entityName = objParentField.Entity;
                }
            }
            else if (model.IsGridChildOfListView) {
                $scope.IsGridChildOfListView = true;
                if (model.Name == "sfwGridView") {
                    var listViewparent = FindParent(model, "sfwListView");
                    if (listViewparent) {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                        if (object) {
                            $scope.entityName = object.Entity;
                        }
                    }
                }
                else if (model.Name == "sfwColumn") {
                    var listViewparent = FindParent(model, "sfwListView");
                    if (listViewparent) {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                        if (object) {
                            $scope.entityName = object.Entity;
                        }
                    }
                }
                else if (model.Name == "sfwListView") {

                    var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, model.dictAttributes.sfwEntityField);
                    if (object) {
                        $scope.entityName = object.Entity;
                    }
                }
            }
        }
    }

    if ($scope.model) {
        $scope.blnBoundToQuery = $scope.model.dictAttributes.sfwBoundToQuery;
        $scope.strQueryLoadType = $scope.model.dictAttributes.sfwQueryLoadType;
        $scope.strBaseQuery = $scope.model.dictAttributes.sfwBaseQuery;
        $scope.strEntityField = $scope.model.dictAttributes.sfwEntityField;
        $scope.getParentEntityName($scope.model);

    }

    else if ($scope.dropdata) {
        $scope.getParentEntityName($scope.dropdata);
    }
    $scope.lstMappedFields = [];
    $scope.objTempGrid = {};
    $scope.lstQueryFields = [];
    $scope.GridEntityName;

    $scope.onBindToQuerySaveClick = function (conditions) {
        if ($scope.IsAddNewGrid) {
            var controlVM = $scope.dropdata;
            if (controlVM) {
                var cellVM = GetVM('sfwColumn', controlVM);
                if (cellVM) {

                    var newScope = $scope.$new();
                    newScope.objGridView = {
                        Name: 'sfwGridView',
                        prefix: 'swc',
                        Value: '',
                        dictAttributes: {
                            sfwEntityField: $scope.strEntityField,
                            sfwBoundToQuery: $scope.blnBoundToQuery,
                            sfwQueryLoadType: $scope.strQueryLoadType,
                            sfwBaseQuery: $scope.strBaseQuery,
                        },
                        Elements: [],
                        Children: []
                    };
                    var lstTable = $scope.formobject.Elements.filter(function (itm) { return itm.Name === "sfwTable" });
                    newScope.objGridView.LstDisplayedEntities = [];
                    newScope.objGridView.lstselectedmultiplelevelfield = [];
                    newScope.objGridView.selectedentityobjecttreefields = [];
                    newScope.cellVM = cellVM;
                    newScope.IsAddToExistingCell = true;
                    newScope.IsAddFromToolBox = true;
                    newScope.IsAddGridWithPanel = $scope.IsAddGridWithPanel;
                    newScope.selectedEntityField = $scope.SelectedField;
                    newScope.FormModel = $scope.formobject;
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.entityName, $scope.strEntityField)
                    if (objField) {
                        newScope.ParentEntityName = objField.Entity;
                    }
                    if (lstTable && lstTable.length > 0) {
                        newScope.SfxMainTable = lstTable[0];
                    }
                    //newScope.GridEntityField = gridEntityField;
                    newScope.title = "Create New Grid";
                    //newScope.LstDisplayedEntities = $scope.LstDisplayedEntities;
                    if ($scope.formobject.dictAttributes.sfwType == "Tooltip") {
                        newScope.skipSecondStep = true;
                    }
                    if (newScope.FormModel && newScope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                        newScope.IsPrototype = true;
                    }
                    else {
                        newScope.IsPrototype = false;
                    }
                    newScope.objGridDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateGridViewControl.html", {
                        width: 1000, height: 700
                    });
                    newScope.setTitle = function (title) {
                        if (newScope.title) {
                            newScope.title = title;
                            newScope.objGridDialog.updateTitle(newScope.title);
                        }
                    };
                }
            }
        }
        else {
            $rootScope.UndRedoBulkOp("Start");

            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwEntityField, $scope.model.dictAttributes, "sfwEntityField", $scope.strEntityField);
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwBoundToQuery, $scope.model.dictAttributes, "sfwBoundToQuery", $scope.blnBoundToQuery);
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwQueryLoadType, $scope.model.dictAttributes, "sfwQueryLoadType", $scope.strQueryLoadType);
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwBaseQuery, $scope.model.dictAttributes, "sfwBaseQuery", $scope.strBaseQuery);
            if ($scope.model.dictAttributes.sfwBoundToQuery === "True" && !$scope.model.dictAttributes.sfwBaseQuery && $scope.objgridboundedquery) {
                $scope.$evalAsync(function () { $scope.objgridboundedquery.IsQuery = false; });

            }
            var lsttemplatefields = $scope.model.Elements.length > 0 ? $scope.model.Elements[0].Elements : null;
            if (lsttemplatefields) {
                angular.forEach(lsttemplatefields, function (objtemplatefield) {
                    if (objtemplatefield.Elements.length > 0) {
                        var itemtemplate = objtemplatefield.Elements[0];
                        angular.forEach(itemtemplate.Elements, function (itemtemplatecontrol) {
                            if (itemtemplatecontrol.dictAttributes.sfwEntityField) {
                                for (i = 0; i < $scope.lstMappedFields.length; i++) {
                                    if ($scope.lstMappedFields[i].GridControl == itemtemplatecontrol.dictAttributes.sfwEntityField) {
                                        $rootScope.EditPropertyValue(itemtemplatecontrol.dictAttributes.sfwEntityField, itemtemplatecontrol.dictAttributes, "sfwEntityField", $scope.lstMappedFields[i].MappedControl);

                                        break;
                                    }
                                }
                            }
                        });
                    }
                });
            }


            var filescope = getCurrentFileScope();
            if (filescope) {
                if (filescope.selectControl) {
                    filescope.selectControl($scope.model, event);
                }
                if (filescope.LoadDetails) {
                    filescope.LoadDetails(true);
                }
            }
            $rootScope.UndRedoBulkOp("End");
            $scope.onCancelClick();
        }
    };

    $scope.onAfterOkGridClick = function () {
        if ($scope.onAfterOkClick) {
            $scope.onAfterOkClick();
        }
    }

    $scope.onCancelClick = function () {
        $scope.BindToQueryDialog.close();
    };

    $scope.ValidateGridView = function () {
        var retVal = false;
        $scope.ErrorMessage = "";
        if ($scope.blnBoundToQuery && $scope.blnBoundToQuery.toLowerCase() == "true") {
            if (!$scope.strBaseQuery) {
                retVal = true;
                $scope.ErrorMessage = "Error: Select Base Query For a Grid";
            }
            else {
                if (!$scope.strBaseQuery.contains(".")) {
                    retVal = true;
                    $scope.ErrorMessage = "Error: Invalid Query";
                }
                else {
                    var lst = $scope.strBaseQuery.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            if (!objEntity.Queries.some(function (x) { return x.ID == strQueryID; })) {
                                $scope.ErrorMessage = "Error: Invalid Query";
                                retValue = true;
                            }
                        }
                        else {
                            $scope.ErrorMessage = "Error: Invalid Query";
                            retValue = true;
                        }
                    }
                }
            }
        }
        else {
            if ($scope.formobject && $scope.formobject.dictAttributes.sfwType !== "Lookup" && $scope.formobject.dictAttributes.sfwType !== "FormLinkLookup") {
                if (!$scope.strEntityField) {
                    retVal = true;
                    $scope.ErrorMessage = "Error: Select Collection For a Grid";
                }
                else {
                    //if ($scope.formobject && $scope.formobject.SelectedControl && $scope.formobject.SelectedControl.IsGridChildOfListView) {

                    //    if ($scope.model && $scope.model.Name == "sfwGridView") {
                    //        var listViewparent = FindParent($scope.model, "sfwListView");
                    //        if (listViewparent) {
                    //            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                    //            if (object && object.DataType !== "Collection" && object.DataType !== "List") {
                    //                $scope.ErrorMessage = "Error: Select valid collection for a grid.";
                    //                retVal = true;
                    //            }
                    //            else {
                    //                $scope.ErrorMessage = "Error: Select valid collection for a grid.";
                    //                retVal = true;
                    //            }
                    //        }
                    //    }
                    //}
                    //else {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.entityName, $scope.strEntityField)
                    if (!objField) {
                        $scope.ErrorMessage = "Error: Select valid collection for a grid.";
                        retVal = true;
                    }
                    else if (objField && objField.DataType !== "Collection" && objField.DataType !== "List") {
                        $scope.ErrorMessage = "Error: Select valid collection for a grid.";
                        retVal = true;
                    }
                    //}
                }
            }
        }
        return retVal;
    }

    $scope.BoundToQueryChange = function () {
        $scope.lstMappedFields = [];
        if ($scope.blnBoundToQuery && $scope.blnBoundToQuery.toLowerCase() == "true") {
            $scope.strEntityField = "";
        }
        else {
            $scope.strQueryLoadType = "";
            $scope.strBaseQuery = "";
        }
    };

    $scope.ExecuteQueryClick = function () {
        $scope.lstMappedFields = [];
        $scope.objTempGrid = GetBaseModel($scope.model);
        if ($scope.strEntityField || (!$scope.blnBoundToQuery && ($scope.formobject.dictAttributes.sfwType === "Lookup" || $scope.formobject.dictAttributes.sfwType === "FormLinkLookup"))) {
            var strgridentityname;
            var objgridentityfield = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, $scope.strEntityField);
            if (objgridentityfield) {
                strgridentityname = objgridentityfield.Entity;
            }
            else if ($scope.formobject.dictAttributes.sfwType === "Lookup" || $scope.formobject.dictAttributes.sfwType === "FormLinkLookup") {
                strgridentityname = $scope.formobject.dictAttributes.sfwEntity;
            }
            $scope.GridEntityName = strgridentityname;
            if ($scope.objTempGrid.Elements.length > 0) {
                var lsttemplatefields = $scope.objTempGrid.Elements[0].Elements;
                if (lsttemplatefields) {
                    angular.forEach(lsttemplatefields, function (objtemplatefield) {
                        if (objtemplatefield.Elements.length > 0) {
                            var itemtemplate = objtemplatefield.Elements[0];
                            angular.forEach(itemtemplate.Elements, function (itemtemplatecontrol) {
                                if (itemtemplatecontrol.dictAttributes.sfwEntityField && itemtemplatecontrol.Name !== "cellformat") {
                                    var blnMatchfound = false;
                                    if ($scope.blnBoundToQuery == "False") {
                                        var lstData = $Entityintellisenseservice.GetIntellisenseData(strgridentityname, "", "", true, true, false, false, false, false);
                                        if (lstData) {
                                            for (i = 0; i < lstData.length; i++) {
                                                if (lstData[i].Value == itemtemplatecontrol.dictAttributes.sfwEntityField) {
                                                    blnMatchfound = true;

                                                    $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: lstData[i].ID });
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        var objField;
                                        if (strgridentityname) {
                                            objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strgridentityname, itemtemplatecontrol.dictAttributes.sfwEntityField);
                                        }
                                        if (objField) {
                                            blnMatchfound = true;

                                            $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: itemtemplatecontrol.dictAttributes.sfwEntityField });
                                        }
                                    }


                                    if (!blnMatchfound) {
                                        $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: "" });

                                        itemtemplatecontrol.dictAttributes.sfwEntityField = "";
                                    }

                                }
                            });
                        }

                    });
                }
            }
        }
        else if ($scope.strBaseQuery && $scope.objTempGrid) {
            var strgridentityname;
            var objgridentityfield = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formobject.dictAttributes.sfwEntity, $scope.objTempGrid.dictAttributes.sfwEntityField);
            if (objgridentityfield) {
                strgridentityname = objgridentityfield.Entity;
            }

            //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
            $.connection.hubForm.server.getEntityQueryColumns($scope.strBaseQuery, 'dummy').done(function (data) {
                $scope.$evalAsync(function () {
                    if (data && $scope.objTempGrid.Elements.length > 0) {
                        $scope.lstQueryFields = $getModelList.getModelListFromQueryFieldlist(data);
                        var lsttemplatefields = $scope.objTempGrid.Elements[0].Elements;
                        if (lsttemplatefields) {
                            angular.forEach(lsttemplatefields, function (objtemplatefield) {
                                if (objtemplatefield.Elements.length > 0) {
                                    var itemtemplate = objtemplatefield.Elements[0];
                                    angular.forEach(itemtemplate.Elements, function (itemtemplatecontrol) {
                                        if (itemtemplatecontrol.dictAttributes.sfwEntityField && itemtemplatecontrol.Name !== "cellformat") {
                                            if (strgridentityname) {

                                                var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strgridentityname, itemtemplatecontrol.dictAttributes.sfwEntityField);
                                                if (object) {
                                                    var blnMatchfound = false;
                                                    for (i = 0; i < data.length; i++) {

                                                        if (data[i].CodeID == object.Value) {
                                                            blnMatchfound = true;

                                                            $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: data[i].CodeID })
                                                            break;
                                                        }

                                                    }
                                                    if (!blnMatchfound) {
                                                        $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: "" })
                                                    }
                                                } else {
                                                    $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: "" })
                                                }
                                            }
                                            else {
                                                var blnMatchfound = false;
                                                for (i = 0; i < data.length; i++) {

                                                    if (data[i].CodeID == itemtemplatecontrol.dictAttributes.sfwEntityField) {
                                                        blnMatchfound = true;

                                                        $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: data[i].CodeID })
                                                        break;
                                                    }

                                                }
                                                if (!blnMatchfound) {
                                                    $scope.lstMappedFields.push({ GridControl: itemtemplatecontrol.dictAttributes.sfwEntityField, MappedControl: "" })
                                                }
                                            }
                                        }
                                    });
                                }

                            });
                        }
                    }
                });
            });
        }
    };

    $scope.setRowSelection = function (value) {
        $scope.selectedfield = value;
    };

}]);