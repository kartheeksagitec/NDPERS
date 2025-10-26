app.controller("CreateGridViewController", ["$scope", "$rootScope", "$EntityIntellisenseFactory", "$GetEntityFieldObjectService", "$timeout", "$getModelList", function ($scope, $rootScope, $EntityIntellisenseFactory, $GetEntityFieldObjectService, $timeout, $getModelList) {

    $scope.ObjgridBoundedQuery = undefined;
    $scope.FieldCollection = [];
    $scope.GridFirstStepTitle = "Select the collection fields to be displayed in the grid";
    if (!$scope.IsAddColumnSelected) {
        $scope.objGridView.isNewSelected = false;
        $scope.objGridView.NewbuttonID = "";
        $scope.objGridView.NewformID = "";

        $scope.objGridView.isOpenSelected = false;
        $scope.objGridView.OpenbuttonID = "";
        $scope.objGridView.OpenformID = "";

        $scope.objGridView.isOpenLookupSelected = false;
        $scope.objGridView.OpenLookupbuttonID = "";
        $scope.objGridView.OpenLookupformID = "";

        $scope.objGridView.isOpenMaintenanceSelected = false;
        $scope.objGridView.OpenMaintenancebuttonID = "";
        $scope.objGridView.OpenMaintenanceformID = "";

        $scope.objGridView.isDeleteSelected = false;
        $scope.objGridView.DeletebuttonID = "";

        $scope.objGridView.isExportToExcelSelected = false;
        $scope.objGridView.ExportbuttonID = "";
    }

    $scope.ExpandCollapsedCustomAttrField = function (field, event) {
        field.IsExpanded = !field.IsExpanded;
    };


    $scope.SetFieldClass = function (obj) {
        if (obj == $scope.SelectedField) {
            return "selected";
        }
    };

    $scope.SelectFieldClick = function (obj, event) {
        $scope.SelectedField = obj;
        if (event) {
            event.stopPropagation();
        }
    };



    $scope.objGridView.lstselectedobjecttreefields = [];
    $scope.objGridView.preselectedfields = [];
    $scope.objGridView.lstPreDisabledField = [];

    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
    var lst = entityIntellisenseList.filter(function (x) {
        return x.ID == $scope.ParentEntityName;
    });

    function iteratorColumnElements(itm) {
        function iteratTemplate(ctrl) {
            if ("sfwEntityField" in ctrl.dictAttributes) {
                if (lst && lst.length > 0 && lst[0].Attributes.length > 0) {
                    var objAttr = "";
                    if ($scope.ParentEntityName && ctrl.dictAttributes.sfwEntityField) {
                        objAttr = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.ParentEntityName, ctrl.dictAttributes.sfwEntityField);
                    }
                    if (objAttr) {
                        var attr = { Key: ctrl.dictAttributes.sfwEntityField, Value: objAttr };
                        $scope.objGridView.lstPreDisabledField.push(attr);
                    }
                }
                else if ($scope.ObjgridBoundedQuery && $scope.ObjgridBoundedQuery.SortedColumns) {
                    var lstField = $scope.ObjgridBoundedQuery.SortedColumns.filter(function (x) { return x.ID == ctrl.dictAttributes.sfwEntityField; });
                    if (lstField && lstField.length > 0) {
                        lstField[0].IsCheckboxDisabled = true;
                    }
                }
            }
        }
        if (itm.Name == "TemplateField") {
            var lstItem = itm.Elements.filter(function (itm) { return itm.Name == "ItemTemplate"; });
            if (lstItem && lstItem.length > 0) {
                var ItemTemplate = lstItem[0];

                angular.forEach(ItemTemplate.Elements, iteratTemplate);
            }
        }
    }

    $scope.getModelForBoundedQueryList = function (lstfields) {
        var lst = [];
        lst = $getModelList.getModelListFromQueryFieldlist(lstfields);
        return lst;
    };

    if ($scope.selectedEntityField) {
        if (lst && lst.length > 0) {
            if (lst[0].Attributes.length > 0) {
                var primarykeyattribute = lst[0].Attributes.filter(function (x) {
                    return x.Type == "Column" && x.KeyNo == '1';
                });
                if (primarykeyattribute.length > 0) {
                    $scope.objGridView.preselectedfields.push(primarykeyattribute[0]);
                }
            }
        }

        //var entitycollname = GetItemPathForEntityObject($scope.selectedEntityField);       
        var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
        var DisplayName = "";
        if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
            DisplayName = DisplayedEntity.strDisplayName + "." + $scope.selectedEntityField.ID;
        } else {
            DisplayName = $scope.selectedEntityField.ID;
        }

        var entitycollname = DisplayName;
        if ($scope.IsAddGridWithPanel) {
            entitycollname = $scope.GridEntityField;
        }
        //entitycollname = $scope.selectedEntityField.Entity;
        $scope.objGridView.ParentVM = $scope.cellVM;
        $scope.objGridView.dictAttributes.ID = "dgrResult";
        $scope.objGridView.dictAttributes.AllowPaging = "True";
        $scope.objGridView.dictAttributes.AllowSorting = "True";
        $scope.objGridView.dictAttributes.sfwSelection = "Many";
        $scope.objGridView.dictAttributes.sfwEntityField = entitycollname;

        if ($scope.selectedEntityField.ID) {
            var strControlID = $scope.selectedEntityField.ID;
            strControlID = CreateControlID($scope.FormModel, $scope.selectedEntityField.ID, 'sfwGridView');
            $scope.objGridView.dictAttributes.ID = strControlID;
        }
        var blnIsLookup = $scope.FormModel.dictAttributes.sfwType == "Lookup";

        var strGridViewID = $scope.objGridView.dictAttributes.ID;
        if (startsWith(strGridViewID, "grv", 0))
            strGridViewID = strGridViewID.substring(3);

        if (blnIsLookup) {
            if ($scope.objGridView.NewbuttonID == "")
                $scope.objGridView.NewbuttonID = "btnNew";
            if ($scope.objGridView.OpenbuttonID == "")
                $scope.objGridView.OpenbuttonID = "btnOpen";
            if ($scope.objGridView.OpenLookupbuttonID == "")
                $scope.objGridView.OpenLookupbuttonID = "btnOpenLookup";
            if ($scope.objGridView.OpenMaintenancebuttonID == "")
                $scope.objGridView.OpenMaintenancebuttonID = "btnOpenMaintenance";
            if ($scope.objGridView.DeletebuttonID == "")
                $scope.objGridView.DeletebuttonID = "btnDelete";
            if ($scope.objGridView.ExportbuttonID == "")
                $scope.objGridView.ExportbuttonID = "btnExportExcel";
        }
        else {

            if (!$scope.objGridView.NewbuttonID)
                $scope.objGridView.NewbuttonID = "btnNew" + strGridViewID;
            if (!$scope.objGridView.OpenbuttonID)
                $scope.objGridView.OpenbuttonID = "btnOpen" + strGridViewID;
            if (!$scope.objGridView.OpenLookupbuttonID)
                $scope.objGridView.OpenLookupbuttonID = "btnOpen" + strGridViewID + "Lookup";
            if (!$scope.objGridView.OpenMaintenancebuttonID)
                $scope.objGridView.OpenMaintenancebuttonID = "btnOpen" + strGridViewID + "Maintenance";
            if (!$scope.objGridView.DeletebuttonID)
                $scope.objGridView.DeletebuttonID = "btnDelete" + strGridViewID;
            if (!$scope.objGridView.ExportbuttonID)
                $scope.objGridView.ExportbuttonID = "btnExportExcel" + strGridViewID;
        }
    }
    else if (!$scope.IsAddColumnSelected && ($scope.IsAddFromToolBox || $scope.objGridView.dictAttributes.sfwBaseQuery)) {
        $scope.objGridView.ParentVM = $scope.cellVM;
        $scope.objGridView.dictAttributes.ID = "dgrResult";
        $scope.objGridView.dictAttributes.AllowPaging = "True";
        $scope.objGridView.dictAttributes.AllowSorting = "True";
        $scope.objGridView.dictAttributes.sfwSelection = "Many";
        if ($scope.objGridView.dictAttributes.sfwEntityField) {
            var strControlID = CreateControlID($scope.FormModel, $scope.objGridView.dictAttributes.sfwEntityField, 'sfwGridView');
            $scope.objGridView.dictAttributes.ID = strControlID;
        }
        else {
            var strControlID = CreateControlID($scope.FormModel, "GridView", 'sfwGridView');
            $scope.objGridView.dictAttributes.ID = strControlID;
        }

        var strGridViewID = $scope.objGridView.dictAttributes.ID;
        if (startsWith(strGridViewID, "grv", 0))
            strGridViewID = strGridViewID.substring(3);

        if (blnIsLookup) {
            if ($scope.objGridView.NewbuttonID == "")
                $scope.objGridView.NewbuttonID = "btnNew";
            if ($scope.objGridView.OpenbuttonID == "")
                $scope.objGridView.OpenbuttonID = "btnOpen";
            if ($scope.objGridView.OpenLookupbuttonID == "")
                $scope.objGridView.OpenLookupbuttonID = "btnOpenLookup";
            if ($scope.objGridView.OpenMaintenancebuttonID == "")
                $scope.objGridView.OpenMaintenancebuttonID = "btnOpenMaintenance";
            if ($scope.objGridView.DeletebuttonID == "")
                $scope.objGridView.DeletebuttonID = "btnDelete";
            if ($scope.objGridView.ExportbuttonID == "")
                $scope.objGridView.ExportbuttonID = "btnExportExcel";
        }
        else {

            if (!$scope.objGridView.NewbuttonID)
                $scope.objGridView.NewbuttonID = "btnNew" + strGridViewID;
            if (!$scope.objGridView.OpenbuttonID)
                $scope.objGridView.OpenbuttonID = "btnOpen" + strGridViewID;
            if (!$scope.objGridView.OpenLookupbuttonID)
                $scope.objGridView.OpenLookupbuttonID = "btnOpen" + strGridViewID + "Lookup";
            if (!$scope.objGridView.OpenMaintenancebuttonID)
                $scope.objGridView.OpenMaintenancebuttonID = "btnOpen" + strGridViewID + "Maintenance";
            if (!$scope.objGridView.DeletebuttonID)
                $scope.objGridView.DeletebuttonID = "btnDelete" + strGridViewID;
            if (!$scope.objGridView.ExportbuttonID)
                $scope.objGridView.ExportbuttonID = "btnExportExcel" + strGridViewID;
        }

        if ($scope.objGridView.dictAttributes.sfwBaseQuery && $scope.objGridView.dictAttributes.sfwBoundToQuery == "True") {
            $scope.ObjgridBoundedQuery = {};
            $scope.GridFirstStepTitle = "Select the query fields to be displayed in the grid";
            var QueryID = $scope.objGridView.dictAttributes.sfwBaseQuery;
            //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
            $.connection.hubForm.server.getEntityQueryColumns(QueryID, "dummy").done(function (data) {
                //$scope.receiveQueryFields(data, query.dictAttributes.sfwQueryRef);
                $scope.$evalAsync(function () {
                    if (data && data.length > 0) {
                        var lstDataFields = data;
                        sortListBasedOnproperty(lstDataFields, "", "CodeID");
                        $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = [];
                        $scope.ObjgridBoundedQuery.SortedColumns = [];
                        $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = $scope.getModelForBoundedQueryList(lstDataFields);
                        $scope.ObjgridBoundedQuery.SortedColumns = $scope.ObjgridBoundedQuery.lstselectedobjecttreefields;
                    }
                });
            });
        }
    }
    else {
        if ($scope.objGridView) {
            var lstColumns = $scope.objGridView.Elements.filter(function (itm) { return itm.Name == "Columns"; });
            if (!lstColumns || (lstColumns && lstColumns.length == 0)) {
                var columnvm = { Name: 'Columns', Value: '', dictAttributes: {}, Elements: [] };
                $rootScope.PushItem(columnvm, $scope.objGridView.Elements);
                lstColumns = $scope.objGridView.Elements.filter(function (itm) { return itm.Name == "Columns"; });
            }
            if ($scope.objGridView.dictAttributes.sfwBaseQuery && $scope.objGridView.dictAttributes.sfwBoundToQuery == "True") {
                $scope.ObjgridBoundedQuery = {};
                $scope.GridFirstStepTitle = "Select the query fields to be displayed in the grid";
                var QueryID = $scope.objGridView.dictAttributes.sfwBaseQuery;
                //dummy dialog id is given as second parameter in below call, so that it gets the column alias name from query instead of actual columns.
                $.connection.hubForm.server.getEntityQueryColumns(QueryID, "dummy").done(function (data) {
                    //$scope.receiveQueryFields(data, query.dictAttributes.sfwQueryRef);
                    $scope.$evalAsync(function () {
                        if (data && data.length > 0) {
                            var lstDataFields = data;
                            sortListBasedOnproperty(lstDataFields, "", "CodeID");
                            $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = [];
                            $scope.ObjgridBoundedQuery.SortedColumns = [];
                            $scope.ObjgridBoundedQuery.lstselectedobjecttreefields = $scope.getModelForBoundedQueryList(lstDataFields);
                            $scope.ObjgridBoundedQuery.SortedColumns = $scope.ObjgridBoundedQuery.lstselectedobjecttreefields;

                            if (lstColumns && lstColumns.length > 0) {

                                angular.forEach(lstColumns[0].Elements, iteratorColumnElements);
                            }
                        }
                    });
                });
            }
            else {
                if (lstColumns && lstColumns.length > 0) {

                    angular.forEach(lstColumns[0].Elements, iteratorColumnElements);
                }
            }

        }
    }


    $scope.populateParamtersForform = function (formname, formtype) {
        if (formname) {
            var filename = formname;
            if (formtype == "New") {
                $scope.ShowNewParameters = false;
            }
            else if (formtype == "Open") {
                $scope.ShowOpenParameters = false;
            }
            else if (formtype == "OpenLookup") {
                $scope.ShowOpenLookupParameters = false;
            }
            else if (formtype == "OpenMaintenance") {
                $scope.ShowOpenMaintenanceParameters = false;
            }
            $.connection.hubForm.server.getFormParameters(filename, formtype).done(function (lstparams) {
                $scope.receiveFormParameters(lstparams, formtype);
            });
        }
    };

    $scope.receiveFormParameters = function (lstparams, formtype) {
        if (lstparams) {
            if (formtype == "New") {
                $scope.FormNewParameters = lstparams;
            }
            else if (formtype == "Open") {
                $scope.FormOpenParameters = lstparams;
            }
            else if (formtype == "OpenLookup") {
                $scope.FormOpenLookupParameters = lstparams;
            }
            else if (formtype == "OpenMaintenance") {
                $scope.FormOpenMaintenanceParameters = lstparams;
            }
        }

    };

    $scope.onSfxGridCancelClick = function () {

        if ($scope.objGridDialog) {
            $scope.objGridDialog.close();
        }
        if ($scope.onCancelClick) {
            $scope.onCancelClick();
        }
    };

    //#region  Validate Next Button
    $scope.isNextDisableForGrid = function () {
        var IsValid = true;
        $scope.objGridView.ErrorMessageForDisplay = "";
        if (!$scope.objGridView.ValidateData()) {
            IsValid = false;
        }

        return IsValid;
    };

    $scope.objGridView.ValidateData = function () {
        var lstselectedfields = [];
        var retValue = false;
        if ($scope.ObjgridBoundedQuery) {
            if ($scope.ObjgridBoundedQuery.SortedColumns)
                lstselectedfields = $scope.ObjgridBoundedQuery.SortedColumns.filter(function (itm) { return itm.IsSelected });
        }
        else {
            lstselectedfields = $scope.objGridView.lstselectedmultiplelevelfield;//GetSelectedFieldList($scope.objGridView.lstselectedobjecttreefields, lstselectedfields);
        }
        if (lstselectedfields.length == 0) {
            $scope.objGridView.ErrorMessageForDisplay = "Error: Please select atleast one value from the list.";
            retValue = true;
        }
        return retValue;
    };

    $scope.isNextDisableForGridButton = function () {
        var IsValid = false;
        $scope.objGridView.ErrorMessageForGridButton = "";
        if ($scope.objGridView.ValidateGridButtonData()) {
            IsValid = true;
        }

        return IsValid;
    };

    $scope.objGridView.ValidateGridButtonData = function () {
        var retValue = false;

        if ($scope.objGridView.isOpenMaintenanceSelected == "True") {
            if ($scope.objGridView.OpenMaintenanceformID == undefined || $scope.objGridView.OpenMaintenanceformID == "") {
                $scope.objGridView.ErrorMessageForGridButton = "Error: Please Select Maintenance Form.";
                retValue = true;
            }
        }

        if ($scope.objGridView.isOpenLookupSelected == "True") {
            if ($scope.objGridView.OpenLookupformID == undefined || $scope.objGridView.OpenLookupformID == "") {
                $scope.objGridView.ErrorMessageForGridButton = "Error: Please Select Lookup Form.";
                retValue = true;
            }
        }

        if ($scope.objGridView.isOpenSelected == "True") {
            if ($scope.objGridView.OpenformID == undefined || $scope.objGridView.OpenformID == "") {
                $scope.objGridView.ErrorMessageForGridButton = "Error: Please Select Maintenance Form.";
                retValue = true;
            }
        }

        if ($scope.objGridView.isNewSelected == "True") {
            if ($scope.objGridView.NewformID == undefined || $scope.objGridView.NewformID == "") {
                $scope.objGridView.ErrorMessageForGridButton = "Error: Please Select Maintenance Form.";
                retValue = true;
            }
        }




        return retValue;
    };

    //#endregion

    $scope.onCollectionStepNext = function () {
        if ($scope.IsAddColumnSelected || $scope.skipSecondStep) {
            $scope.LoadControlTypes();
            $scope.LoadDataFormat();
            $scope.LoadDataKey();
            $scope.LoadOrder();
            $scope.LoadControls();
            $scope.LoadSort();
            $scope.LoadRelativeControl();
        }
    };

    var ialGridButtons = [];

    $scope.onAddButtonsNextClick = function () {
        var prefix = "swc";
        ialGridButtons = [];
        if ($scope.objGridView.isNewSelected && $scope.objGridView.isNewSelected == 'True') {
            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.NewbuttonID;
            newControl.dictAttributes.sfwMethodName = "btnNew_Click";
            newControl.dictAttributes.sfwActiveForm = $scope.objGridView.NewformID;
            newControl.dictAttributes.Text = "New";

            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters($scope.FormNewParameters);
            ialGridButtons.push(newControl);
        }
        if ($scope.objGridView.isOpenSelected && $scope.objGridView.isOpenSelected == 'True') {
            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.OpenbuttonID;
            newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
            newControl.dictAttributes.sfwActiveForm = $scope.objGridView.OpenformID;
            newControl.dictAttributes.sfwRelatedControl = $scope.objGridView.dictAttributes.ID;
            newControl.dictAttributes.Text = "Open";
            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters($scope.FormOpenParameters);

            ialGridButtons.push(newControl);
        }
        if ($scope.objGridView.isOpenLookupSelected && $scope.objGridView.isOpenLookupSelected == 'True') {

            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.OpenLookupbuttonID;
            newControl.dictAttributes.Visible = "False";
            newControl.dictAttributes.sfwMethodName = "btnOpenLookup_Click";
            newControl.dictAttributes.sfwActiveForm = $scope.objGridView.OpenLookupformID;
            newControl.dictAttributes.Text = "Open Lookup";
            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters($scope.FormOpenLookupParameters);

            ialGridButtons.push(newControl);
        }
        if ($scope.objGridView.isOpenMaintenanceSelected && $scope.objGridView.isOpenMaintenanceSelected == 'True') {
            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.OpenMaintenancebuttonID;
            newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
            newControl.dictAttributes.sfwActiveForm = $scope.objGridView.OpenMaintenanceformID;
            newControl.dictAttributes.sfwRelatedControl = $scope.objGridView.dictAttributes.ID;
            newControl.dictAttributes.Text = "Open Maintenance";
            newControl.dictAttributes.sfwNavigationParameter = GetNavigationParameters($scope.FormOpenMaintenanceParameters);

            ialGridButtons.push(newControl);
        }
        if ($scope.objGridView.isDeleteSelected && $scope.objGridView.isDeleteSelected == 'True') {
            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.DeletebuttonID;
            newControl.dictAttributes.sfwMethodName = "btnDelete_Click";
            newControl.dictAttributes.Text = "Delete";

            ialGridButtons.push(newControl);
        }
        if ($scope.objGridView.isExportToExcelSelected && $scope.objGridView.isExportToExcelSelected == 'True') {
            var newControl = {
                Name: 'sfwButton', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = $scope.objGridView.ExportbuttonID;
            newControl.dictAttributes.sfwMethodName = "btnColumnsToExport_Click";
            newControl.dictAttributes.sfwRelatedControl = $scope.objGridView.dictAttributes.ID;
            newControl.dictAttributes.Text = "Export To Excel";

            ialGridButtons.push(newControl);
        }
        $scope.LoadControlTypes();
        $scope.LoadDataFormat();
        $scope.LoadDataKey();
        $scope.LoadOrder();
        $scope.LoadControls();
        $scope.LoadSort();
        $scope.LoadRelativeControl();

        $scope.ShowNewParameters = false;
        $scope.ShowOpenParameters = false;
        $scope.ShowOpenLookupParameters = false;
        $scope.ShowOpenMaintenanceParameters = false;
    };


    $scope.ArrControlTypes = [];
    $scope.ArrDataFormat = [];
    $scope.ArrDataKey = [];
    $scope.ArrSort = [];
    $scope.ArrOrder = [];
    $scope.LoadControlTypes = function () {
        $scope.ArrControlTypes = [];
        $scope.ArrControlTypes.push("");
        $scope.ArrControlTypes.push("Label");
        if ($scope.FormModel.dictAttributes.sfwType != "Lookup" && !$scope.ObjgridBoundedQuery) {
            $scope.ArrControlTypes.push("HyperLink");
            $scope.ArrControlTypes.push("Checkbox");
            $scope.ArrControlTypes.push("JSONData");
        }
        if (!$scope.IsPrototype && $scope.FormModel.dictAttributes.sfwType != "Lookup" && !$scope.ObjgridBoundedQuery) {
            $scope.ArrControlTypes.push("TextBox");
            $scope.ArrControlTypes.push("DropDownList");
        }
    };

    $scope.ChangeSortExpression = function (selectedcolumn) {
        if (selectedcolumn && !selectedcolumn.istrSort) {
            selectedcolumn.istrOrder = "";
        }
    };

    $scope.SelectQueryField = function (obj, event) {
        $scope.SelectedQueryField = obj;
    }

    $scope.LoadDataFormat = function () {
        $scope.ArrDataFormat = [];
        $scope.ArrDataFormat.push("");
        $scope.ArrDataFormat.push("{0:d}");              // Date
        $scope.ArrDataFormat.push("{0:C}");              // Currency
        $scope.ArrDataFormat.push("{0:000-##-####}");    // SSN
        $scope.ArrDataFormat.push("{0:(###)###-####}");  // Phone/Fax
    };

    $scope.LoadDataKey = function () {
        $scope.ArrDataKey = [];
        $scope.ArrDataKey.push("");
        $scope.ArrDataKey.push("1");
        $scope.ArrDataKey.push("2");
        $scope.ArrDataKey.push("3");
        $scope.ArrDataKey.push("4");
        $scope.ArrDataKey.push("5");
    };

    $scope.LoadSort = function () {
        $scope.ArrSort = [];
        $scope.ArrSort.push("");
        if ($scope.FieldCollection && $scope.FieldCollection.length > 0) {
            for (i = 1; i < $scope.FieldCollection.length + 1; i++) {
                $scope.ArrSort.push(i);
            }
        }
    };

    $scope.LoadOrder = function () {
        $scope.ArrOrder = [];
        $scope.ArrOrder.push("");
        $scope.ArrOrder.push("asc");
        $scope.ArrOrder.push("desc");
    };

    $scope.LoadControls = function () {
        var lstList = [];
        if ($scope.ObjgridBoundedQuery) {
            if ($scope.ObjgridBoundedQuery && $scope.ObjgridBoundedQuery.SortedColumns) {
                lstList = $scope.ObjgridBoundedQuery.SortedColumns.filter(function (itm) { return itm.IsSelected });
            }
        }
        else {
            lstList = $scope.objGridView.lstselectedmultiplelevelfield;//GetSelectedFieldList($scope.objGridView.lstselectedobjecttreefields, lstList);
        }
        var dummyList = angular.copy($scope.FieldCollection);
        $scope.FieldCollection = [];

        function AddInFieldCollection(obj) {
            var lstField = [];
            if ($scope.ObjgridBoundedQuery) {
                lstField = dummyList.filter(function (itm) { return itm.istrFieldName === obj.ID });
            }
            else {
                lstField = dummyList.filter(function (itm) { return itm.istrFieldName === obj.FieldObject.ID });
            }
            var sfxField = {};
            if (lstField && lstField.length > 0) {
                sfxField = lstField[0];
            }
            else {
                if ($scope.ObjgridBoundedQuery) {
                    objModel = obj;
                }
                else {
                    objModel = obj.FieldObject;
                }
                if (objModel) {

                    sfxField.istrFieldName = objModel.ID;
                    sfxField.istrObjectID = objModel.Value;
                    sfxField.istrDataType = objModel.DataType;
                    var entityname = obj.Entity;
                    sfxField.istrEntityName = entityname;
                    sfxField.istrControlType = "Label";

                    //var strHeaderText = sfxField.istrFieldName.substring(sfxField.istrFieldName.lastIndexOf('.') + 1);
                    var strHeaderText = GetCaptionFromField(objModel);
                    sfxField.istrHeader = strHeaderText;
                    if ($scope.ObjgridBoundedQuery) {
                        sfxField.istrItemPath = obj.ID;

                        sfxField.istrPropertyName = obj.ID;
                    }
                    else {
                        sfxField.istrItemPath = obj.EntityField; //GetItemPathForEntityObject(objModel);
                        sfxField.istrPropertyName = obj.EntityField;//GetItemPathForEntityObject(objModel);
                    }

                    sfxField.istrVisible = "True";
                }
            }
            if (sfxField) {
                $scope.FieldCollection.push(sfxField);
            }
        }

        if (lstList.length > 0) {
            var primaryKeyFileds = [];
            //if (!string.IsNullOrEmpty(this.objGridView.BusObjKeyFields))
            //{
            //    primaryKeyFileds = this.objGridView.BusObjKeyFields.Split(',');
            //}
            var intCountPrimary = 0;


            angular.forEach(lstList, AddInFieldCollection);

        }
    };

    $scope.LoadRelativeControl = function () {
        function AddInArrRelativeControl(theButton) {
            if (theButton.dictAttributes.sfwRelatedControl == $scope.objGridView.dictAttributes.ID) {
                var strButtonID = theButton.dictAttributes.ID;
                if (strButtonID) {
                    $scope.ArrRelativeControl.push(strButtonID);
                }
            }
        }
        if ($scope.IsAddColumnSelected) {
            $scope.ArrRelativeControl = [];
            $scope.ArrRelativeControl.push("");
            if ($scope.objGridView) {
                var panel = FindParent($scope.objGridView, "sfwPanel");
                if (panel) {
                    var lstButtons = [];
                    FindControlListByName(panel, "sfwButton", lstButtons);
                    if (lstButtons && lstButtons.length > 0) {

                        angular.forEach(lstButtons, AddInArrRelativeControl);
                    }
                }
            }
        }
        else {
            $scope.ArrRelativeControl = [];
            $scope.ArrRelativeControl.push("");
            function item(theButton) {
                var strButtonID = theButton.dictAttributes.ID;
                if (strButtonID) {
                    $scope.ArrRelativeControl.push(strButtonID);
                }
            }
            angular.forEach(ialGridButtons, item);
        }
    };

    $scope.UpdateGridProperties = function () {
        var strKeySeq;
        var strKeyNames = "";

        var strSortSeq;
        var strSortExpression = "";
        function getSortExpression(objSfxField) {
            strKeySeq = objSfxField.istrKey;
            if (strKeySeq) {
                if (strKeyNames.length == 0) {
                    strKeyNames = strKeySeq + ";" + objSfxField.istrPropertyName;
                }
                else {
                    strKeyNames += "," + strKeySeq + ";" + objSfxField.istrPropertyName;
                }
            }

            strSortSeq = objSfxField.istrSort;
            if (strSortSeq) {
                if (strSortExpression.length == 0) {
                    if (objSfxField.istrOrder) {
                        strSortExpression = strSortSeq + ";" + objSfxField.istrPropertyName + " " + objSfxField.istrOrder;
                    }
                    else {
                        strSortExpression = strSortSeq + ";" + objSfxField.istrPropertyName;
                    }
                }
                else {
                    if (objSfxField.istrOrder) {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.istrPropertyName + " " + objSfxField.istrOrder;
                    }
                    else {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.istrPropertyName;
                    }
                }
            }
        }

        if ($scope.FieldCollection.length > 0) {


            angular.forEach($scope.FieldCollection, getSortExpression);
        }
        var slFieldSeq = [];
        var strDataKeyNames = strKeyNames.split(',');
        //slFieldSeq.Capacity = strDataKeyNames.length;
        function getFieldSequence(strKeyName) {
            var strKeyField = strKeyName.substring(strKeyName.indexOf(';') + 1);
            var obj = {};
            obj.key = strKeyName;
            obj.Value = strKeyField;
            slFieldSeq.push(obj);
        }
        angular.forEach(strDataKeyNames, getFieldSequence);


        strKeyNames = "";

        // IDictionaryEnumerator ide = slFieldSeq.GetEnumerator();
        function getKeyName(key) {
            if (strKeyNames.length == 0) {
                strKeyNames = key.Value;
            }
            else {
                strKeyNames += "," + key.Value;
            }
        }
        angular.forEach(slFieldSeq, getKeyName);

        $scope.objGridView.dictAttributes.sfwDataKeyNames = strKeyNames;

        slFieldSeq = [];
        var strSortExpFields = strSortExpression.split(',');
        // slFieldSeq.Capacity = strSortExpFields.length;

        function iteratorSort(strSortExp) {
            var strSortField = strSortExp.substring(strSortExp.indexOf(';') + 1);
            var obj = {};
            obj.key = strSortExp;
            obj.Value = strSortField;
            slFieldSeq.push(obj);
        }

        angular.forEach(strSortExpFields, iteratorSort);


        strSortExpression = "";

        //ide = slFieldSeq.GetEnumerator();
        function iterator(key) {
            if (strSortExpression.length == 0)
                strSortExpression = key.Value;
            else
                strSortExpression += "," + key.Value;
        }
        angular.forEach(slFieldSeq, iterator);

        $scope.objGridView.dictAttributes.sfwSortExpression = strSortExpression;
    };

    $scope.UpdateGridFields = function () {
        $scope.objGridView.Elements = [];

        if ($scope.FieldCollection.length > 0) {

            var objColumn = {
                Name: 'Columns', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            objColumn.ParentVM = $scope.objGridView;
            $scope.AddGridTemplateColumn(objColumn);

            $scope.objGridView.Elements.push(objColumn);
        }
    };

    $scope.AddGridTemplateColumn = function (objColumn) {
        var prefix = "asp";
        var sfxControl;

        function iterator(objSfxField) {
            var strDataType = objSfxField.istrDataType;
            var strHAlign = "";
            if (strDataType == "Decimal")
                strHAlign = "Right";

            var objTemp = {
                Name: 'TemplateField', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            objTemp.ParentVM = objColumn;


            objTemp.dictAttributes.Visible = objSfxField.istrVisible;
            objTemp.dictAttributes.HeaderText = objSfxField.istrHeader;
            objTemp.dictAttributes["ItemStyle.HorizontalAlign"] = strHAlign;

            var objItmTemp = {
                Name: 'ItemTemplate', prefix: prefix, Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            objItmTemp.ParentVM = objTemp;


            switch (objSfxField.istrControlType) {
                case "HyperLink":
                    sfxControl = {
                        Name: 'sfwLabel', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;
                    sfxControl.dictAttributes.sfwLinkable = "True";
                    sfxControl.dictAttributes.sfwRelatedControl = "btnOpen";
                    break;
                case "TextBox":
                    sfxControl = {
                        Name: 'sfwTextBox', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;


                    if (strDataType == "DateTime")
                        sfxControl.dictAttributes.sfwDataType = strDataType;
                    break;
                case "DropDownList":
                    sfxControl = {
                        Name: 'sfwDropDownList', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var strCodeGroup = GetCodeID(objSfxField.istrEntityName, objSfxField.istrFieldName, entityIntellisenseList);
                    if (!strCodeGroup) {
                        strCodeGroup = "0";
                    }
                    sfxControl.dictAttributes.sfwLoadType = "CodeGroup";

                    break;
                case "Checkbox":
                    sfxControl = {
                        Name: 'sfwCheckBox', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;

                    break;
                case "JSONData":
                    sfxControl = {
                        Name: 'sfwJSONData', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;

                    break;
                default:
                    sfxControl = {
                        Name: 'sfwLabel', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxControl.ParentVM = objItmTemp;
                    break;
            }

            if ($scope.FormModel && $scope.FormModel.dictAttributes.sfwType != "Lookup") {
                if (sfxControl.Name != "sfwLabel" && sfxControl.Name != "sfwButton" && sfxControl.Name != "sfwLinkButton" && sfxControl.Name != "sfwImageButton" && sfxControl.Name !== "sfwButtonGroup") {
                    $scope.objGridView.dictAttributes.AllowEditing = "True";
                    $scope.objGridView.dictAttributes.sfwTwoWayBinding = "True";
                    $scope.objGridView.dictAttributes.sfwCommonFilterBox = "False";
                    $scope.objGridView.dictAttributes.sfwFilterOnKeyPress = "False";
                }
            }


            sfxControl.dictAttributes.sfwEntityField = objSfxField.istrItemPath;

            //  sfxControl.dictAttributes.sfwDataFormat = objSfxField.istrFormat;
            sfxControl.dictAttributes.sfwRelatedControl = objSfxField.istrRelatedControl;
            $rootScope.PushItem(sfxControl, objItmTemp.Elements);
            $rootScope.PushItem(objItmTemp, objTemp.Elements);
            $rootScope.PushItem(objTemp, objColumn.Elements);
        }
        angular.forEach($scope.FieldCollection, iterator);
    };

    $scope.onSfxGridFinishClick = function () {
        if (!$scope.IsAddGridWithPanel) {
            $rootScope.UndRedoBulkOp("Start");
        }

        if (!$scope.IsAddColumnSelected) {
            $scope.UpdateGridProperties();
            $scope.UpdateGridFields();
            // var tableVM = GetVM("sfwTable", $scope.cellVM);
            var tableVM = null;
            if ($scope.cellVM.Name == "sfwButtonGroup") {
                tableVM = $scope.cellVM;
            } else {
                tableVM = GetVM('sfwTable', $scope.cellVM);
            }
            if (tableVM && tableVM.Name != "sfwButtonGroup") {

                var ColCount = GetMaxColCount(tableVM.Elements[0], tableVM);

                if (ialGridButtons.length > 0) {
                    var sfxRowModel = {
                        Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxRowModel.ParentVM = tableVM;

                    for (ind = 0; ind < ColCount; ind++) {
                        var sfxCellModel = {
                            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                        };
                        sfxCellModel.ParentVM = sfxRowModel;

                        sfxRowModel.Elements.push(sfxCellModel);

                    }

                    var index = 0;
                    if ($scope.IsAddToExistingCell) {
                        var parent = $scope.cellVM.ParentVM;
                        index = $scope.cellVM.ParentVM.Elements.indexOf($scope.cellVM);
                    }

                    var sfxCellModel = sfxRowModel.Elements[index];
                    var AddInsfxCellModel = function(btn) {
                        btn.ParentVM = sfxCellModel;
                        sfxCellModel.Elements.push(btn);
                    }
                    angular.forEach(ialGridButtons, AddInsfxCellModel);

                    if ($scope.IsAddToExistingCell) {
                        index = tableVM.Elements.indexOf($scope.cellVM.ParentVM);
                        $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
                    }
                    else {
                        $rootScope.PushItem(sfxRowModel, tableVM.Elements);
                    }

                }

                if ($scope.IsAddToExistingCell) {
                    if ($scope.cellVM) {

                        $rootScope.PushItem($scope.objGridView, $scope.cellVM.Elements);
                    }
                }
                else {

                    var newRowModel = {
                        Name: 'sfwRow', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    newRowModel.ParentVM = tableVM;

                    var newcellModel = {
                        Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    newcellModel.ParentVM = newRowModel;

                    $scope.objGridView.ParentVM = newcellModel;
                    newcellModel.Elements.push($scope.objGridView);

                    newRowModel.Elements.push(newcellModel);

                    for (ind = 1; ind < ColCount; ind++) {
                        newcellModel = {
                            Name: 'sfwColumn', prefix: "swc", Value: '', dictAttributes: {}, Elements: [], Children: []
                        };
                        newcellModel.ParentVM = newRowModel;


                        newRowModel.Elements.push(newcellModel);
                    }

                    $rootScope.PushItem(newRowModel, tableVM.Elements);
                }
            }
            if (tableVM.Name == "sfwButtonGroup") {
                if ($scope.objGridView && $scope.objGridView.dictAttributes) {
                    if (ialGridButtons.length > 0) {
                        var AddBtnInCell = function (btn) {
                            $rootScope.PushItem(btn, tableVM.Elements);
                        }
                        angular.forEach(ialGridButtons, AddBtnInCell);
                    }
                    $rootScope.PushItem($scope.objGridView, tableVM.Elements);
                }
            }
            if ($scope.selectControl) {
                $scope.selectControl($scope.objGridView);
            }

        }
        else {
            $scope.UpdateGridProperties();

            $scope.AddGridColumns();
        }


        if ($scope.onAfterOkGridClick) {
            $scope.onAfterOkGridClick();
        }

        $scope.onSfxGridCancelClick();
        if ($scope.closeClick) {
            $scope.closeClick();
        }

        if (!$scope.IsAddGridWithPanel) {
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.AddGridColumns = function () {
        if ($scope.objGridView) {
            var lstColumns = $scope.objGridView.Elements.filter(function (itm) { return itm.Name == "Columns"; });
            if (lstColumns && lstColumns.length > 0) {
                var objColumn = lstColumns[0];
                $scope.AddGridTemplateColumn(objColumn);
            }
        }
    };


    //#region for Expand and Collapse
    $scope.ShowNewParameters = false;
    $scope.ShowOpenParameters = false;
    $scope.ShowOpenLookupParameters = false;
    $scope.ShowOpenMaintenanceParameters = false;

    $scope.showParametersForGrid = function (obj) {
        if (obj == 'New') {
            $scope.ShowOpenParameters = false;
            $scope.ShowOpenLookupParameters = false;
            $scope.ShowOpenMaintenanceParameters = false;
            if ($scope.ShowNewParameters) {
                $scope.ShowNewParameters = false;
            }
            else {
                $scope.ShowNewParameters = true;
            }
        }
        else if (obj == 'Open') {
            if ($scope.ShowNewParameters) {
                $scope.ShowNewParameters = false;
            }
            $scope.ShowOpenLookupParameters = false;
            $scope.ShowOpenMaintenanceParameters = false;
            if ($scope.ShowOpenParameters) {
                $scope.ShowOpenParameters = false;
            }
            else {
                $scope.ShowOpenParameters = true;
            }
        }
        else if (obj == 'Lookup') {
            if ($scope.ShowNewParameters) {
                $scope.ShowNewParameters = false;
            }
            $scope.ShowOpenParameters = false;
            $scope.ShowOpenMaintenanceParameters = false;
            if ($scope.ShowOpenLookupParameters) {
                $scope.ShowOpenLookupParameters = false;
            }
            else {
                $scope.ShowOpenLookupParameters = true;
            }
        }
        else if (obj == 'Maintenance') {
            if ($scope.ShowNewParameters) {
                $scope.ShowNewParameters = false;
            }
            $scope.ShowOpenParameters = false;
            $scope.ShowOpenLookupParameters = false;
            if ($scope.ShowOpenMaintenanceParameters) {
                $scope.ShowOpenMaintenanceParameters = false;
            }
            else {
                $scope.ShowOpenMaintenanceParameters = true;
            }
        }
        $scope.FieldControlCollection = [];
        PopulateAvailableControl($scope.FieldControlCollection, $scope.SfxMainTable, $scope.ShowNewParameters, $scope.IsAddColumnSelected);
        if (obj != 'Lookup') {
            $.connection.hubForm.server.getGlobleParameters().done(function (data) {
                $scope.$apply(function () {
                    $scope.objGlobleParameters = data;
                    PopulateGlobalParameters($scope.objGlobleParameters, $scope.FieldControlCollection);
                });
            });
        }

    };
    //#endregion

    $scope.SelectField = function (objfield) {
        $scope.SelectedField = objfield;
    };

    $scope.onChangeGridCheckBox = function (formtype) {
        if (formtype == "New") {
            $scope.FormNewParameters = [];
            $scope.objGridView.NewformID = "";
            $scope.ShowNewParameters = false;
        }
        else if (formtype == "Open") {
            $scope.FormOpenParameters = [];
            $scope.objGridView.OpenformID = "";
            $scope.ShowOpenParameters = false;
        }
        else if (formtype == "OpenLookup") {
            $scope.FormOpenLookupParameters = [];
            $scope.objGridView.OpenLookupformID = "";
            $scope.ShowOpenLookupParameters = false;
        }
        else if (formtype == "OpenMaintenance") {
            $scope.FormOpenMaintenanceParameters = [];
            $scope.objGridView.OpenMaintenanceformID = "";
            $scope.ShowOpenMaintenanceParameters = false;
        }

    };

    //#region Move Up /Down Functionality
    $scope.isMoveUp = function () {
        if ($scope.SelectedField == undefined) {
            return true;
        }
        else {
            var index = $scope.FieldCollection.indexOf($scope.SelectedField);
            if (index > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.isMoveDown = function () {
        if ($scope.SelectedField == undefined) {
            return true;
        }
        else {
            var index = $scope.FieldCollection.indexOf($scope.SelectedField);
            if (index != $scope.FieldCollection.length - 1 && index != -1) {
                return false;
            }
            else {
                return true;
            }
        }
    };
    $scope.MoveUpField = function () {
        var index = $scope.FieldCollection.indexOf($scope.SelectedField);
        var tempObj = $scope.FieldCollection[index - 1];
        $scope.FieldCollection[index - 1] = $scope.FieldCollection[index];
        $scope.FieldCollection[index] = tempObj;
        $scope.scrollBySelectedField("#create-result-grid", ".selected");
    };
    $scope.MoveDownField = function () {
        var index = $scope.FieldCollection.indexOf($scope.SelectedField);
        var tempObj = $scope.FieldCollection[index + 1];
        $scope.FieldCollection[index + 1] = $scope.FieldCollection[index];
        $scope.FieldCollection[index] = tempObj;
        $scope.scrollBySelectedField("#create-result-grid", ".selected");
    };

    $scope.scrollBySelectedField = function (parentDiv, selectedElement) {
        $timeout(function () {
            var $divDom = $(parentDiv);
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 300, offsetLeft: 0 }, null);
                return false;
            }

        });
    }
    //#endregion
}]);