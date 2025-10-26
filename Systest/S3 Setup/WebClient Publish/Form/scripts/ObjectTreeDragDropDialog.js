app.controller("ObjectTreeDragDropController", ["$scope", "$http", "$rootScope", "$GetEntityFieldObjectService", "$ValidationService", "$EntityIntellisenseFactory", "$SgMessagesService", function ($scope, $http, $rootScope, $GetEntityFieldObjectService, $ValidationService, $EntityIntellisenseFactory, $SgMessagesService) {

    $scope.lstControls = [];
    $scope.lstValidControls = [];
    var dragDropdata = $scope.dragdropdata;

    $scope.$evalAsync(function () { });

    for (var i = 0; i < $scope.model.Elements.length; i++) {
        var att = $scope.model.Elements[i].dictAttributes;
        var fieldname = "";
        if (att.sfwDataField) {
            fieldname = att.sfwDataField;
        }
        else {
            fieldname = att.sfwEntityField;
        }

        var canAddControl = false;
        if ($rootScope.lstWebControls && $rootScope.lstWebControls.length > 0) {
            var lst = $rootScope.lstWebControls.filter(function (x) {
                return x.ControlName == $scope.model.Elements[i].Name;
            });
            lst = JSON.parse(JSON.stringify(lst));
            if (lst && lst.length > 0) {

                canAddControl = CanAddControlToDropList(lst[0], $scope.formmodel, $scope.model.Elements[i]);
            }
        }

        if (canAddControl && ["Collection", "List", "Object", "CDOCollection"].indexOf($scope.dragdropdata.Type) == -1)

            $scope.lstControls.push({ Controltype: $scope.model.Elements[i].Name, ID: att.ID, Name: fieldname, Index: i, IsExistingControl: true });
    }
    var fieldname = "";
    if ($scope.currentControl.dictAttributes.sfwDataField) {
        fieldname = $scope.currentControl.dictAttributes.sfwDataField;
    }
    else {
        fieldname = $scope.currentControl.dictAttributes.sfwEntityField;
    }
    $scope.lstControls.push({ Controltype: $scope.currentControl.Name, ID: $scope.currentControl.dictAttributes.ID, Name: fieldname, Index: "New Control" });
    $scope.selectedControl = $scope.lstControls[$scope.lstControls.length - 1];

    $scope.selectRow = function (control) {
        $scope.selectedControl = control;
    };

    $scope.IsChildOfGrid = function () {
        var isChild = false;
        var objParent = FindParent($scope.model, "sfwGridView");
        if (!objParent) {
            objParent = FindParent($scope.model, "sfwDialogPanel");
            if (!objParent) {
                objParent = FindParent($scope.model, "sfwListView");
            }
        }

        if (objParent) {
            isChild = true;
        }
        return isChild;
    };

    //#region Show controls according to selected type form Entity Tree
    $scope.showControlbasedOnType = function (SelectedField) {


        $scope.IsGridSeleected = $scope.IsChildOfGrid();
        var IsFound = false;
        if (["Collection", "List", "Object", "CDOCollection"].indexOf(SelectedField.Type) > -1 && (FindParent($scope.model, "sfwListView") || FindParent($scope.model, "sfwDialogPanel"))) {
            IsFound = true;
        }
        if ($scope.formmodel && $scope.formmodel.dictAttributes && $scope.formmodel.dictAttributes.sfwType && SelectedField) {

            if (["Lookup", "Tooltip"].indexOf($scope.formmodel.dictAttributes.sfwType) === -1 && !$scope.formmodel.IsLookupCriteriaEnabled && ["Collection", "Object", "List", "CDOCollection"].indexOf(SelectedField.Type) === -1 && SelectedField.DataType) {
                $scope.lstValidControls.push("sfwLabel");
            }

            else if ($scope.formmodel.dictAttributes.sfwType === "Tooltip" && !$scope.formmodel.IsLookupCriteriaEnabled && ["Collection", "Object", "List", "CDOCollection"].indexOf(SelectedField.Type) === -1) {
                $scope.lstValidControls.push("sfwLabel");
            }
            if ($scope.formmodel.dictAttributes.sfwType !== "Tooltip" && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) === -1 && SelectedField.DataType) {
                $scope.lstValidControls.push("sfwTextBox", "sfwDropDownList", "sfwMultiSelectDropDownList", "sfwCheckBox", "sfwRadioButtonList");
            }
            if ($scope.formmodel.dictAttributes.sfwType !== "Tooltip" && !$scope.IsGridSeleected && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) === -1 && SelectedField.DataType) {
                $scope.lstValidControls.push("sfwCascadingDropDownList");
            }
            if (["Tooltip", "Lookup"].indexOf($scope.formmodel.dictAttributes.sfwType) === -1 && ["Collection", "Object", "CDOCollection", "List",].indexOf(SelectedField.Type) === -1 && SelectedField.DataType) {
                $scope.lstValidControls.push("sfwRadioButton");
            }
            if (!$scope.IsGridSeleected && $scope.formmodel.dictAttributes.sfwType !== 'Tooltip' && ["CDOCollection"].indexOf(SelectedField.Type) > -1) {
                $scope.lstValidControls.push("sfwCheckBoxList");
            }
            if (!$scope.IsGridSeleected && $scope.formmodel.dictAttributes.sfwType !== 'Lookup' && ["Collection", "List"].indexOf(SelectedField.Type) > -1 && !IsFound) {
                $scope.lstValidControls.push("sfwGridView", "sfwChart");
            }
            if (!$scope.IsGridSeleected && ["Lookup", "UserControl", "Tooltip"].indexOf($scope.formmodel.dictAttributes.sfwType) === -1 && ["Object"].indexOf(SelectedField.Type) > -1 && !IsFound) {
                $scope.lstValidControls.push("UserControl");
            }
            if (!$scope.IsGridSeleected && ["Lookup"].indexOf($scope.formmodel.dictAttributes.sfwType) > -1 && SelectedField.DataType && ["DateTime", "Int", "Decimal", "Double"].indexOf(SelectedField.DataType) > -1 && !IsFound) {
                $scope.lstValidControls.push("sfwRange");
            }
            if (["Tooltip"].indexOf($scope.formmodel.dictAttributes.sfwType) === -1 && SelectedField.DataType && ["DateTime", "datetime"].indexOf(SelectedField.DataType) > -1) {
                $scope.lstValidControls.push("sfwDateTimePicker");
            }
            if (!$scope.IsGridSeleected && ["Maintenance", "Wizard"].indexOf($scope.formmodel.dictAttributes.sfwType) > -1 && ["Collection", "List"].indexOf(SelectedField.Type) > -1) {
                $scope.lstValidControls.push("sfwCalendar", "sfwScheduler");
            }
            if (["Maintenance", "Wizard"].indexOf($scope.formmodel.dictAttributes.sfwType) > -1 && !$scope.IsGridSeleected && ["Collection", "List"].indexOf(SelectedField.Type) > -1 && !IsFound) {
                $scope.lstValidControls.push("sfwListView");
            }
            if (["Maintenance", "Wizard", "UserControl",].indexOf($scope.formmodel.dictAttributes.sfwType) > -1 && ["String", "string"].indexOf(SelectedField.DataType) > -1) {
                $scope.lstValidControls.push("sfwJSONData");
            }
        }

    };
    if (dragDropdata) {
        $scope.showControlbasedOnType(dragDropdata);
        if ($scope.lstControls && $scope.lstControls.length > 0 && $scope.lstValidControls && $scope.lstValidControls.length > 0) {
            $scope.lstControls[$scope.lstControls.length - 1].Controltype = $scope.lstValidControls[0];
            $scope.lstControls[$scope.lstControls.length - 1].ID = CreateControlID($scope.formmodel, $scope.fieldName, $scope.lstControls[$scope.lstControls.length - 1].Controltype, false);
        }
    }

    $scope.setControlIDByControlType = function () {
        var controlType = $scope.selectedControl.Controltype;

        if (controlType === "UserControl") {
            controlType = "udc";
        }
        $scope.selectedControl.ID = CreateControlID($scope.formmodel, $scope.fieldName, controlType, false)
    };
    //#endregion

    //#region Add User Control

    $scope.addUserControl = function () {
        if ($scope.selectedControl) {
            var newScope = $scope.$new();
            $scope.currentControl.Name = "udc";
            $scope.currentControl.dictAttributes.ID = $scope.selectedControl.ID;
            newScope.objSetUCProp = {
                StrId: $scope.currentControl.dictAttributes.ID, StrName: '', StrEntityField: $scope.selectedControl.Name, StrResource: '', formObject: $scope.formmodel
            };
            newScope.formodel = $scope.formmodel;
            newScope.onUserControlOkClick = function () {

                $scope.currentControl.dictAttributes.ID = newScope.objSetUCProp.StrId;
                $scope.currentControl.dictAttributes.Name = newScope.objSetUCProp.StrName;
                if (newScope.objSetUCProp.StrEntityField) {
                    $scope.currentControl.dictAttributes.sfwEntityField = newScope.objSetUCProp.StrEntityField;
                }

                $scope.currentControl.dictAttributes.sfwResource = newScope.objSetUCProp.StrResource;

                if ($scope.currentControl.dictAttributes.Name !== undefined && $scope.currentControl.dictAttributes.Name != "") {
                    var fileList = [];
                    var obj = {
                        FileName: $scope.currentControl.dictAttributes.Name, ID: $scope.currentControl.dictAttributes.ID
                    };
                    fileList.push(obj);
                    $.connection.hubForm.server.getUserControlModel(fileList, "").done(function (udcFileList) {
                        newScope.receiveUcMainTable(udcFileList);
                        newScope.onUserControlCancelClick();
                    });
                }

                $rootScope.PushItem($scope.currentControl, $scope.model.Elements);

            };

            newScope.onUserControlCancelClick = function () {
                if (ucPropDialog) {
                    ucPropDialog.close();
                }
                $scope.closeClick();
            };

            newScope.receiveUcMainTable = function (data) {

                newScope.tableTemp;
                for (var i = 0; i < $scope.formmodel.Elements.length; i++) {
                    if ($scope.formmodel.Elements[i].Name == "sfwTable") {
                        newScope.tableTemp = $scope.formmodel.Elements[i];
                        break;
                    }
                }
                for (var i = 0; i < data.length; i++) {
                    var ucControl = FindControlByID(newScope.tableTemp, data[i].ID);
                    //console.log("table: ", $scope.tableTemp);
                    if (ucControl) {
                        $scope.$apply(function () {
                            ucControl.UcChild = data ? [data[i].udcModel] : []; // converting object to array 
                            setParentControlName(ucControl.UcChild[0]);
                        });
                    }
                }
            };


            newScope.ValidateUserProp = function () {
                var retVal = false;
                newScope.ErrorMessageForDisplay = "";
                if (newScope.objSetUCProp.StrId == undefined || newScope.objSetUCProp.StrId == "") {
                    newScope.ErrorMessageForDisplay = "Error: Enter the ID.";
                    retVal = true;
                }
                else {
                    var lstIds = [];
                    CheckforDuplicateID($scope.formmodel, newScope.objSetUCProp.StrId, lstIds);
                    if (lstIds.length > 0) {
                        newScope.ErrorMessageForDisplay = "Error: Duplicate ID.";
                        retVal = true;
                    } else if (!isValidIdentifier(newScope.objSetUCProp.StrId, false, false)) {
                        newScope.ErrorMessageForDisplay = "Error: Invalid ID.";
                        retVal = true;
                    }
                }
                if (!newScope.objSetUCProp.StrName || newScope.objSetUCProp.StrName == '') {
                    newScope.ErrorMessageForDisplay = "Please Enter Active Form.";
                    retVal = true;
                }
                //else if (!newScope.objSetUCProp.StrEntityField || newScope.objSetUCProp.StrEntityField == "") {
                //    newScope.ErrorMessageForDisplay = "Please Enter Entity field.";
                //    retVal = true;
                //}
                else if (!newScope.objSetUCProp.StrResource || newScope.objSetUCProp.StrResource == '') {
                    newScope.ErrorMessageForDisplay = "Please Enter Resource.";
                    retVal = true;
                }

                if (newScope.ErrorMessageForDisplay == undefined || newScope.ErrorMessageForDisplay == "") {
                    if (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "") {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formmodel.dictAttributes.sfwEntity, newScope.objSetUCProp.StrEntityField);
                        if (!object || object.Type != "Object") {
                            newScope.ErrorMessageForDisplay = "Entity Field should be Object.";
                            retVal = true;
                        }
                    }
                }
                return retVal;
            };


            var ucPropDialog = $rootScope.showDialog(newScope, "User Control", "Form/views/SetUserControlProperties.html");
        }
    };
    //#endregion

    //#region Add Grid Control

    $scope.addGridControl = function () {
        var newScope = $scope.$new();
        newScope.objGridView = {
            Name: 'sfwGridView',
            prefix: 'swc',
            Value: '',
            dictAttributes: {
                //sfwDatasourceType: "EntityCollection",
            },
            Elements: [],
            Children: []
        };
        newScope.objGridView.LstDisplayedEntities = [];
        newScope.objGridView.lstselectedmultiplelevelfield = [];
        newScope.objGridView.selectedentityobjecttreefields = [];
        newScope.cellVM = $scope.model;
        newScope.selectedEntityField = $scope.dragdropdata;
        newScope.FormModel = $scope.formmodel;
        newScope.ParentEntityName = $scope.dragdropdata.Entity;
        if ($scope.formmodel.dictAttributes.sfwType == "Tooltip") {
            newScope.skipSecondStep = true;
        }
        newScope.IsAddToExistingCell = true;
        var lstTable = $scope.formmodel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (lstTable && lstTable.length > 0) {
            newScope.SfxMainTable = lstTable[0];
        }
        newScope.title = "Create New Grid";
        newScope.LstDisplayedEntities = [];
        if ($scope.formmodel.dictAttributes.sfwType == "Tooltip") newScope.skipSecondStep = true;
        if (newScope.FormModel && newScope.FormModel.dictAttributes.ID.startsWith("wfp")) {
            newScope.IsPrototype = true;
        }
        else {
            newScope.IsPrototype = false;
        }
        newScope.objGridDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateGridViewControl.html", { width: 1000, height: 700 });
    };
    //#endregion

    //#region Add Chart Control

    $scope.addChartControl = function () {
        $scope.ParentEntityName = $scope.dragdropdata.Entity;
        var cellVM = $scope.model;
        if (null != cellVM) {
            var newScope = $scope.$new();
            newScope.ObjChartModel = {
                Name: 'sfwChart',
                prefix: 'swc',
                Value: '',
                dictAttributes: {
                    ID: '', sfwEntityField: $scope.selectedControl.Name, ChartType: '', Width: '', Height: '', ShowLegend: 'True'
                },
                Elements: [],
                Children: []
            };
            newScope.ObjChartModel.lstselectedobjecttreefields = [];
            newScope.ObjChartModel.selectedobjecttreefield;
            newScope.lstChartType = $rootScope.ChartTypes;
            newScope.lstToolTipTypes = ['None', 'Chart', 'Table', 'Both'];
            newScope.DataFormats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];


            newScope.SeriesModel = {
                Name: 'Series',
                prefix: '',
                Value: '',
                dictAttributes: {
                },
                Elements: [],
                Children: []
            };
            newScope.SeriesModel.ParentVM = newScope.ObjChartModel;
            newScope.ObjChartModel.Elements.push(newScope.SeriesModel);

            var ChartAreasModel = {
                Name: 'ChartAreas',
                prefix: '',
                Value: '',
                dictAttributes: {
                },
                Elements: [],
                Children: []
            };
            ChartAreasModel.ParentVM = newScope.ObjChartModel;
            newScope.ObjChartModel.Elements.push(ChartAreasModel);

            newScope.sfwChartAreaModel = {
                Name: 'sfwChartArea',
                prefix: 'swc',
                Value: '',
                dictAttributes: {
                    ChartAreaName: '', BackColor: ''
                },
                Elements: [],
                Children: []
            };
            newScope.sfwChartAreaModel.ParentVM = ChartAreasModel;
            ChartAreasModel.Elements.push(newScope.sfwChartAreaModel);

            newScope.Area3DStyle = {
                Name: 'Area3DStyle',
                prefix: '',
                Value: '',
                dictAttributes: {
                    IsEnable3D: 'false', Inclination: '', LightStyle: ''
                },
                Elements: [],
                Children: []
            };
            newScope.Area3DStyle.ParentVM = newScope.sfwChartAreaModel;
            newScope.sfwChartAreaModel.Elements.push(newScope.Area3DStyle);


            newScope.onSfxChartFinishClick = function () {
                newScope.UpdateNavigationParam();

                if (newScope.ObjChartModel && newScope.ObjChartModel.dictAttributes) {
                    if (!newScope.ObjChartModel.dictAttributes.ID.match("^chr")) {
                        newScope.ObjChartModel.dictAttributes.ID = "chr" + newScope.ObjChartModel.dictAttributes.ID;
                    }
                }
                $rootScope.PushItem(newScope.ObjChartModel, $scope.model.Elements);

                SetFormSelectedControl($scope.formmodel, newScope.ObjChartModel, event);

                newScope.onSfxChartCancelClick();
                $scope.closeClick();
            };

            newScope.UpdateNavigationParam = function () {
                var strParameters = "";
                function item(grdParam) {
                    var strParamValue = grdParam.ParmeterValue;

                    if (strParamValue != null && strParamValue != "") {
                        var blnConstant = grdParam.ParmeterConstant;
                        if (blnConstant)
                            strParamValue = "#" + strParamValue;

                        var strParam = strParamValue;
                        var strParamField = grdParam.ParmeterField;

                        if (strParamValue.toLowerCase() != strParamField.toLowerCase())
                            strParam = strParamField + '=' + strParamValue;

                        if (strParameters == "")
                            strParameters = strParam;
                        else
                            strParameters += ';' + strParam;
                    }
                }
                function iterator(aobjSeriesModel) {
                    strParameters = "";
                    angular.forEach(aobjSeriesModel.FormParameters, item);
                    aobjSeriesModel.dictAttributes.sfwNavigationParameter = strParameters;
                }
                angular.forEach(newScope.SeriesModel.Elements, iterator);
            };

            newScope.onSfxChartCancelClick = function () {
                if (newScope.CreateChartDialog) {
                    newScope.CreateChartDialog.close();
                }

            };

            newScope.isChartNextDisable = function () {
                var IsValid = false;
                newScope.ObjChartModel.ErrorMessageForDisplay = "";
                if (!newScope.ObjChartModel.ValidateChartName()) {
                    IsValid = true;
                }
                else if (!newScope.ObjChartModel.ValidateEntityCollection()) {
                    IsValid = true;
                }

                else if (!newScope.ObjChartModel.ValidateChartType()) {
                    IsValid = true;
                }
                else if (!newScope.ObjChartModel.ValidateWidth()) {
                    IsValid = true;
                }
                else if (!newScope.ObjChartModel.ValidateHeight()) {
                    IsValid = true;
                }

                return IsValid;

            };


            //#region Validation for Create Chart

            //#region Validate Chart Name
            newScope.ObjChartModel.ValidateChartName = function () {
                var retValue = true;
                if (!newScope.ObjChartModel.dictAttributes.ID) {
                    newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter ID.";
                    retValue = false;
                }
                return retValue;
            };
            //#endregion

            //#region Validation for Entity Collecction
            newScope.ObjChartModel.ValidateEntityCollection = function () {
                var retValue = true;
                if (!newScope.ObjChartModel.dictAttributes.sfwEntityField) {
                    newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Entity.";
                    retValue = false;
                }

                return retValue;
            };
            //#endregion


            //#region Validation for Chart Type
            newScope.ObjChartModel.ValidateChartType = function () {
                var retValue = true;
                if (!newScope.ObjChartModel.dictAttributes.ChartType) {
                    newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please Select a ChartType.";
                    retValue = false;
                }
                return retValue;
            };
            //#endregion

            //#region Validation for Width
            newScope.ObjChartModel.ValidateWidth = function () {
                var retValue = true;
                if (!newScope.ObjChartModel.dictAttributes.Width) {
                    newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Width.";
                    retValue = false;
                }

                return retValue;
            };
            //#endregion

            //#region Validation for Height
            newScope.ObjChartModel.ValidateHeight = function () {
                var retValue = true;
                if (!newScope.ObjChartModel.dictAttributes.Height) {
                    newScope.ObjChartModel.ErrorMessageForDisplay = "Error: Please enter a Height.";
                    retValue = false;
                }

                return retValue;
            };
            //#endregion


            //#region add series

            newScope.ShowSeriesDetail = function (itm) {

                itm.ShowDetails = !itm.ShowDetails;
            };

            newScope.SelectSeries = function (obj) {
                if (newScope.ObjSeriesModel && newScope.ObjSeriesModel != obj) {
                    if (newScope.ObjSeriesModel.ShowDetails) {
                        newScope.ObjSeriesModel.ShowDetails = false;
                    }
                }
                newScope.ObjSeriesModel = obj;
            };

            newScope.GetSeriesID = function () {
                var strItemKey = "Series";
                var iItemNum = 0;
                var strItemName = strItemKey;

                var newTemp = newScope.SeriesModel.Elements.filter(function (x) {
                    return x.dictAttributes.Name == strItemName;
                });

                while (newTemp.length > 0) {
                    iItemNum++;
                    strItemName = strItemKey + iItemNum;
                    newTemp = newScope.SeriesModel.Elements.filter(function (x) {
                        return x.dictAttributes.Name == strItemName;
                    });
                }
                return strItemName;
            };

            newScope.OnAddSeriesClick = function () {
                var newSeriesScope = newScope.$new();

                newSeriesScope.ObjSeriesModel = {
                    Name: 'sfwSeries',
                    prefix: 'swc',
                    Value: '',
                    dictAttributes: {
                        Name: '', XValueMember: '', YValueMembers: '', YMemberColor: '', IsValueShownAsLabel: 'False', sfwFormatField: '', sfwTooltipType: '', sfwTooltipTableParams: '', sfwActiveForm: ''
                    },
                    Elements: [],
                    Children: []
                };
                newSeriesScope.ObjSeriesModel.dictAttributes.Name = newScope.GetSeriesID();
                newSeriesScope.ObjSeriesModel.ParentVM = newScope.SeriesModel;
                newSeriesScope.ObjSeriesModel.lstselectedobjecttreefields = [];
                newSeriesScope.ObjSeriesModel.selectedobjecttreefield;
                newSeriesScope.ObjSeriesModel.ShowDetails = true;
                newSeriesScope.ObjSeriesModel.FormParameters = [];
                newSeriesScope.ShowParameters = false;
                //$scope.populateParamtersForform = function () {

                //    $.connection.hubForm.server.getFormParameters(newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm, "").done(function (lstparams) {
                //        $scope.receiveFormParameters(lstparams, "");
                //    });
                //};

                $scope.receiveFormParameters = function (lstparams, formtype) {
                    if (lstparams) {
                        newSeriesScope.$evalAsync(function () {
                            newSeriesScope.ObjSeriesModel.FormParameters = lstparams;
                            newSeriesScope.ShowParameters = true;
                        });
                    }
                };
                newSeriesScope.ClearParamtersForform = function () {
                    if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.FormParameters) {
                        newSeriesScope.ObjSeriesModel.FormParameters = "";
                    }
                    if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm != "") {
                        newSeriesScope.seriesactiveform = true;
                    }
                    newSeriesScope.ShowParameters = false;
                };

                //#region Validation for Add Series


                newSeriesScope.isChartFinishDisable = function () {
                    var IsValid = false;
                    newSeriesScope.ErrorMessageForDisplay = "";
                    if (newSeriesScope.ObjSeriesModel) {
                        if (!newSeriesScope.ValidateSeriesName()) {
                            IsValid = true;
                        }
                        else if (!newSeriesScope.ValidateXValue()) {
                            IsValid = true;
                        }
                        else if (!newSeriesScope.ValidateYValue()) {
                            IsValid = true;
                        }

                    }
                    return IsValid;
                };

                //#region Validation for Series Name
                newSeriesScope.ValidateSeriesName = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.Name) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Name.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion

                //#region Validation for X Value Member
                newSeriesScope.ValidateXValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.XValueMember) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series XValueMember.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion


                //#region Validation for Y Value Member
                newSeriesScope.ValidateYValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion

                //#region Validation for Y Value Member
                newSeriesScope.ValidateYValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion

                //#endregion

                newSeriesScope.OpenTooltipParams = function () {
                    newScope.OpenTooltipParams(newSeriesScope);
                };

                newSeriesScope.onAdditionalChartColumnClick = function () {
                    newScope.onAdditionalChartColumnClick(newSeriesScope);
                };


                newSeriesScope.onCancelClick = function () {
                    if (seriesDialog) {
                        seriesDialog.close();
                    }
                };

                newSeriesScope.OnOkClick = function () {
                    newSeriesScope.ObjSeriesModel.dictAttributes.ChartType = newScope.ObjChartModel.dictAttributes.ChartType;
                    newScope.SeriesModel.Elements.push(newSeriesScope.ObjSeriesModel);
                    if (seriesDialog) {
                        seriesDialog.close();
                    }
                };


                var seriesDialog = $rootScope.showDialog(newSeriesScope, "Add Series", "Form/views/AddEditSeries.html", { width: 600, height: 600 });

            };

            newScope.OpenTooltipParams = function (newSeriesScope) {
                var newParamScope = newSeriesScope.$new();
                newScope.objTooltipParamsVM = $rootScope.showDialog(newParamScope, "Set Tooltip Parameters", "Form/views/SetToolTipParameters.html", { width: 500, height: 420 });

                newParamScope.onSfxChartTooltipTableCancelClick = function () {
                    if (newScope.objTooltipParamsVM) {
                        newScope.objTooltipParamsVM.close();
                    }

                };

                newParamScope.onSfxChartTooltipTableOKClick = function () {
                    var lstselectedfields = [];
                    lstselectedfields = GetSelectedFieldList(newSeriesScope.ObjSeriesModel.lstselectedobjecttreefields, lstselectedfields);
                    var DisplayedEntity = getDisplayedEntity(newSeriesScope.ObjSeriesModel.LstDisplayedEntities);
                    var itempath = "";
                    if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                        itempath = DisplayedEntity.strDisplayName;
                    }
                    function iUpdatetooltipparam(itm) {
                        var ID = itm.ID;
                        if (itempath) {
                            ID = itempath + "." + ID;
                        }
                        if (tooltipparam) {
                            tooltipparam = tooltipparam + ";" + ID;
                        }
                        else {
                            tooltipparam = ID;
                        }
                    }
                    if (lstselectedfields.length > 0) {
                        var tooltipparam;

                        angular.forEach(lstselectedfields, iUpdatetooltipparam);

                        newSeriesScope.ObjSeriesModel.dictAttributes.sfwTooltipTableParams = tooltipparam;

                    }
                    newParamScope.onSfxChartTooltipTableCancelClick();

                };
            };

            newScope.OnRemoveSeriesClick = function () {

                if (newScope.ObjSeriesModel) {
                    var index = newScope.SeriesModel.Elements.indexOf(newScope.ObjSeriesModel);
                    newScope.SeriesModel.Elements.splice(index, 1);

                    if (index < newScope.SeriesModel.Elements.length) {
                        newScope.SelectSeries(newScope.SeriesModel.Elements[index]);
                    }
                    else if (newScope.SeriesModel.Elements.length > 0) {
                        newScope.SelectSeries(newScope.SeriesModel.Elements[index - 1]);
                    }
                    else {
                        newScope.ObjSeriesModel = undefined;
                    }
                }

            };

            newScope.OnEditSeries = function (itm) {
                var newSeriesScope = newScope.$new();
                newSeriesScope.ObjSeriesModel = itm;
                newSeriesScope.ObjSeriesModel.ParentVM = undefined;
                if (newSeriesScope.ObjSeriesModel.FormParameters && newSeriesScope.ObjSeriesModel.FormParameters.length > 0) {
                    newSeriesScope.ShowParameters = true;
                }
                else {
                    newSeriesScope.ShowParameters = false;
                }

                newSeriesScope.seriesactiveform = true;
                //$scope.populateParamtersForform = function () {

                //    $.connection.hubForm.server.getFormParameters(newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm, "").done(function (lstparams) {
                //        $scope.receiveFormParameters(lstparams, "");
                //    });
                //};

                $scope.receiveFormParameters = function (lstparams, formtype) {

                    if (lstparams) {
                        newSeriesScope.$evalAsync(function () {
                            function iUpdateParams(param) {

                                var lst = params.filter(function (obj) {
                                    return param.ParmeterField == obj.ParmeterField;
                                });
                                if (lst && lst.length > 0) {
                                    param.ParmeterValue = lst[0].ParmeterValue;
                                    param.ParmeterConstant = lst[0].ParmeterConstant;
                                }
                            }
                            var params = newSeriesScope.ObjSeriesModel.FormParameters;
                            newSeriesScope.ObjSeriesModel.FormParameters = [];
                            if (params && params.length > 0) {


                                angular.forEach(lstparams, iUpdateParams);
                            }
                            newSeriesScope.ObjSeriesModel.FormParameters = lstparams;
                            newSeriesScope.ShowParameters = true;
                        });
                    }
                };
                newSeriesScope.ClearParamtersForform = function () {
                    if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.FormParameters) {
                        newSeriesScope.ObjSeriesModel.FormParameters = "";
                    }
                    if (newSeriesScope.ObjSeriesModel && newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm != "") {
                        newSeriesScope.seriesactiveform = true;
                    }
                    newSeriesScope.ShowParameters = false;
                };

                //#region Validation for Add Series


                newSeriesScope.isChartFinishDisable = function () {
                    var IsValid = false;
                    newSeriesScope.ErrorMessageForDisplay = "";
                    if (newSeriesScope.ObjSeriesModel) {
                        if (!newSeriesScope.ValidateSeriesName()) {
                            IsValid = true;
                        }
                        else if (!newSeriesScope.ValidateXValue()) {
                            IsValid = true;
                        }
                        else if (!newSeriesScope.ValidateYValue()) {
                            IsValid = true;
                        }

                    }
                    return IsValid;
                };

                //#region Validation for Series Name
                newSeriesScope.ValidateSeriesName = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.Name) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Name.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion

                //#region Validation for X Value Member
                newSeriesScope.ValidateXValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.XValueMember) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series XValueMember.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion


                //#region Validation for Y Value Member
                newSeriesScope.ValidateYValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion


                //#region Validation for Y Value Member
                newScope.ValidateActiveForm = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.sfwActiveForm) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series Navigation Form.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion


                //#region Validation for Y Value Member
                newScope.ValidateYValue = function () {
                    var retValue = true;
                    if (!newSeriesScope.ObjSeriesModel.dictAttributes.YValueMembers) {
                        newSeriesScope.ErrorMessageForDisplay = "Error: Please enter a Series YValueMembers.";
                        retValue = false;
                    }

                    return retValue;
                };
                //#endregion

                //#endregion

                newSeriesScope.OpenTooltipParams = function () {
                    newScope.OpenTooltipParams(newSeriesScope);
                };

                newSeriesScope.onAdditionalChartColumnClick = function () {
                    newScope.onAdditionalChartColumnClick(newSeriesScope);
                };

                newSeriesScope.onCancelClick = function () {
                    if (seriesDialog) {
                        seriesDialog.close();
                    }
                };

                newSeriesScope.OnOkClick = function () {
                    newSeriesScope.ObjSeriesModel.ParentVM = newScope.SeriesModel;
                    newSeriesScope.onCancelClick();
                };


                var seriesDialog = $rootScope.showDialog(newSeriesScope, "Edit Series", "Form/views/AddEditSeries.html", { width: 600, height: 600 });
            };
            //#endregion

            newScope.onAdditionalChartColumnClick = function (newSeriesScope) {
                var newColumnScope = newSeriesScope.$new();
                newColumnScope.sfwAddtionalChartColumns = [];
                newColumnScope.SelectedObject = newSeriesScope.ObjSeriesModel;
                //newColumnScope.ParentEntityName = $scope.FormModel.dictAttributes.sfwEntity;
                if (newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns) {
                    var temp = newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns.split(",");
                    for (var i = 0; i < temp.length; i++) {
                        newColumnScope.sfwAddtionalChartColumns.push({ Property: temp[i] });
                    }
                }
                var AdditionalChartdialog = $rootScope.showDialog(newColumnScope, "Multiple Objects Selection", "Form/views/AdditionalChartColumnsDialog.html", { width: 1000, height: 490 });
                newColumnScope.onCancelClick = function () {
                    AdditionalChartdialog.close();
                };
                newColumnScope.onOkClick = function () {
                    var AddtionalChartColumns = "";
                    for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                        if (newColumnScope.sfwAddtionalChartColumns[i].Property != "") {
                            if (AddtionalChartColumns == "") {
                                AddtionalChartColumns = newColumnScope.sfwAddtionalChartColumns[i].Property;
                            }
                            else {
                                AddtionalChartColumns += "," + newColumnScope.sfwAddtionalChartColumns[i].Property;
                            }
                        }
                    }
                    $rootScope.EditPropertyValue(newSeriesScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns, newSeriesScope.ObjSeriesModel.dictAttributes, "swfAddtionalChartColumns", AddtionalChartColumns);
                    AdditionalChartdialog.close();
                };
                newColumnScope.selectRow = function (row) {
                    newColumnScope.selectedRow = row;
                };
                newColumnScope.addProperty = function () {
                    newColumnScope.sfwAddtionalChartColumns.push({ Property: "" });
                };
                newColumnScope.deleteProperty = function () {
                    var index = -1;
                    if (newColumnScope.selectedRow) {
                        for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                            if (newColumnScope.selectedRow == newColumnScope.sfwAddtionalChartColumns[i]) {
                                index = i;
                                break;
                            }
                        }
                    }
                    if (index > -1) {
                        newColumnScope.sfwAddtionalChartColumns.splice(index, 1);
                        if (newColumnScope.sfwAddtionalChartColumns.length > 0) {
                            if (index > 0) {
                                newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[index - 1];
                            }
                            else {
                                newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[newColumnScope.sfwAddtionalChartColumns.length - 1];
                            }
                        }
                        else {
                            newColumnScope.selectedRow = undefined;
                        }
                    }
                };
            };
            newScope.title = "Create Chart";
            newScope.CreateChartDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateChart.html", { width: 1000, height: 700 });
            //#endregion 
        }


    };

    //#endregion

    //#region Add Calendar Control

    $scope.addCalendarControl = function (isAddScheduler) {
        var newScope = $scope.$new();
        var name = "sfwCalendar";
        newScope.IsAddScheduler = isAddScheduler;
        if (isAddScheduler) {
            name = "sfwScheduler";
        }
        newScope.objCalendar = {
            Name: name,
            prefix: 'swc',
            Value: '',
            dictAttributes: {
            },
            Elements: [],
            Children: []
        };
        //var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
        //var itempath = selectedField.ID;
        //if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
        //    itempath = DisplayedEntity.strDisplayName + "." + selectedField.ID;
        //}
        var cellVM = $scope.model;
        newScope.objCalendar.dictAttributes.ID = $scope.selectedControl.ID
        newScope.objCalendar.dictAttributes.sfwEntityField = $scope.fieldName;
        newScope.FormModel = $scope.formmodel;
        newScope.ParentEntityName = $scope.dragdropdata.Entity;;
        var lstTable = $scope.formmodel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (lstTable && lstTable.length > 0) {
            newScope.SfxMainTable = lstTable[0];
        }
        newScope.title = "Create New Calendar";
        if (isAddScheduler) {
            newScope.title = "Create New Scheduler";
            newScope.lstRelatedDialog = PopulateRelatedDialogList(newScope.SfxMainTable);
        }
        //#region Validate Calendar Control

        newScope.ValidateCalendar = function () {
            newScope.ErrorMessageForDisplay = "";
            if (!newScope.objCalendar.dictAttributes.ID) {
                newScope.ErrorMessageForDisplay = "Enter ID.";
                return true;
            }
            else if (newScope.objCalendar.dictAttributes.ID && !isValidIdentifier(newScope.objCalendar.dictAttributes.ID, false, false)) {
                newScope.ErrorMessageForDisplay = "Invalid ID.";
                return true;
            } else {
                var lstIds = [];
                CheckforDuplicateID($scope.formmodel, newScope.objCalendar.dictAttributes.ID, lstIds);
                if (lstIds.length > 0) {
                    newScope.ErrorMessageForDisplay = "Duplicate ID.";
                    return true;
                }
            }
            if (newScope.objCalendar.dictAttributes.sfwEventId == undefined || newScope.objCalendar.dictAttributes.sfwEventId == "") {
                newScope.ErrorMessageForDisplay = "Enter Event Id.";
                return true;
            }
            if (newScope.objCalendar.dictAttributes.sfwEventName == undefined || newScope.objCalendar.dictAttributes.sfwEventName == "") {
                newScope.ErrorMessageForDisplay = "Enter Event Name.";
                return true;
            }
            if (newScope.objCalendar.dictAttributes.sfwEventStartDate == undefined || newScope.objCalendar.dictAttributes.sfwEventStartDate == "") {
                newScope.ErrorMessageForDisplay = "Enter Event Start Date.";
                return true;
            }
            if (newScope.objCalendar.dictAttributes.sfwEventEndDate == undefined || newScope.objCalendar.dictAttributes.sfwEventEndDate == "") {
                newScope.ErrorMessageForDisplay = "Enter Event End Date.";
                return true;
            }
            //else if (isAddScheduler && !newScope.objCalendar.dictAttributes.sfwEventCategory) {
            //    newScope.ErrorMessageForDisplay = "Enter Event Category.";
            //    return true;
            //} else if (isAddScheduler && !newScope.objCalendar.dictAttributes.sfwRelatedDialogPanel) {
            //    newScope.ErrorMessageForDisplay = "Enter Related Dialog Panel.";
            //    return true;
            //}

            if (newScope.objCalendar.errors && $ValidationService.isEmptyObj(newScope.objCalendar.errors)) {
                return true;
            }

            return false;
        };

        //#endregion

        newScope.onOkClick = function () {

            $rootScope.PushItem(newScope.objCalendar, $scope.model.Elements);

            SetFormSelectedControl($scope.formmodel, newScope.objCalendar, event);

            newScope.onCancelClick();
            $scope.closeClick();
        };

        newScope.onCancelClick = function () {
            if (newScope.objCalendar.errors) {
                newScope.objCalendar.errors = {};
                $ValidationService.removeObjInToArray($scope.validationErrorList, newScope.objCalendar);
            }
            if (newScope.objCalendarDialog) {
                newScope.objCalendarDialog.close();
            }
        };

        newScope.objCalendarDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/AddCalendarControl.html", { width: 700, height: 700 });
        ComponentsPickers.init();
    };
    //#endregion

    //#region  Add Repeater Control
    $scope.addRepeaterControl = function () {
        var strID = CreateControlID($scope.formmodel, "RepeaterViewPanel", "sfwListView");
        var prefix = "swc";
        var objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
        var DisplayedEntity = getDisplayedEntity($scope.LstDisplayedEntities);
        var itempath = $scope.dragdropdata.ID;
        if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
            itempath = DisplayedEntity.strDisplayName + "." + $scope.dragdropdata.ID;
        }
        strControlID = CreateControlID($scope.formmodel, $scope.dragdropdata.ID, 'sfwListView');
        objRepeaterControl.dictAttributes.ID = strControlID;
        objRepeaterControl.ParentVM = $scope.model;
        objRepeaterControl.dictAttributes.sfwEntityField = itempath;

        var tableVM = GetVM("sfwTable", $scope.model);
        if (tableVM) {

            if (objRepeaterControl) {
                objRepeaterControl.dictAttributes.sfwSelection = "Many";
                objRepeaterControl.dictAttributes.sfwCaption = "List View";
                objRepeaterControl.dictAttributes.AllowPaging = "True";
                objRepeaterControl.dictAttributes.PageSize = "1";


                var parentenetityname = $scope.dragdropdata.Entity;
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                objRepeaterControl.dictAttributes.sfwDataKeyNames = GetTableKeyFields(parentenetityname, entityIntellisenseList);

                var prefix = "swc";


                var objListTableModel = AddListViewTable($scope.formmodel, objRepeaterControl);
                objRepeaterControl.Elements.push(objListTableModel);
                objRepeaterControl.initialvisibilty = true;
                objRepeaterControl.isLoaded = true;

                $rootScope.PushItem(objRepeaterControl, $scope.model.Elements);

                if ($scope.selectControl) {
                    $scope.selectControl(objRepeaterControl);
                }
            }
        }
    }
    //#endregion


    $scope.okClick = function () {

        if ($scope.selectedControl) {
            if ($scope.selectedControl.Index == "New Control") {
                if ($scope.selectedControl.Controltype == "sfwGridView") {

                    $scope.addGridControl();
                }
                else if ($scope.selectedControl.Controltype == "UserControl") {
                    $scope.addUserControl();
                }

                else if ($scope.selectedControl.Controltype == "sfwChart") {
                    $scope.addChartControl();
                }
                else if ($scope.selectedControl.Controltype == "sfwCalendar") {
                    $scope.addCalendarControl(false);
                }
                else if ($scope.selectedControl.Controltype == "sfwScheduler") {
                    $scope.addCalendarControl(true);
                }
                else if ($scope.selectedControl.Controltype == "sfwListView") {
                    $scope.addRepeaterControl();
                    $scope.closeClick();
                }
                else if ($scope.selectedControl.Controltype == "sfwListView") {
                    $scope.addRepeaterControl();
                    $scope.closeClick();
                }
                else {
                    $scope.currentControl.Name = $scope.selectedControl.Controltype;
                    $scope.currentControl.dictAttributes.ID = $scope.selectedControl.ID;
                    var dropControl = $scope.model;
                    if ($scope.model.Name == "TemplateField") {
                        dropControl = $scope.model.Elements.filter(function (x) { return x.Name == "ItemTemplate" })[0];
                    }
                    $rootScope.PushItem($scope.currentControl, dropControl.Elements);
                    SetFormSelectedControl($scope.formmodel, $scope.currentControl);
                    $scope.closeClick();
                }
            }
            else {
                if ($scope.currentControl.dictAttributes.sfwDataField) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue($scope.model.Elements[$scope.selectedControl.Index].dictAttributes.sfwDataField, $scope.model.Elements[$scope.selectedControl.Index].dictAttributes, "sfwDataField", $scope.currentControl.dictAttributes.sfwDataField);

                    if ($scope.$parent.formmodel.IsLookupCriteriaEnabled) {
                        var formScope = getCurrentFileScope();
                        if (formScope && formScope.lookupTreeObject) {
                            var subqueryentityname = formScope.lookupTreeObject.EntityName;
                            if (subqueryentityname) {
                                $rootScope.EditPropertyValue($scope.model.Elements[$scope.selectedControl.Index].dictAttributes.sfwQueryID, $scope.model.Elements[$scope.selectedControl.Index].dictAttributes, "sfwQueryID", subqueryentityname);
                            }
                        }
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    $rootScope.EditPropertyValue($scope.model.Elements[$scope.selectedControl.Index].dictAttributes.sfwEntityField, $scope.model.Elements[$scope.selectedControl.Index].dictAttributes, "sfwEntityField", $scope.currentControl.dictAttributes.sfwEntityField);
                }
                $scope.closeClick();
            }
        }
        else {
            $SgMessagesService.Message('Message', "Select a control from the list.");
        }

    };

    $scope.closeClick = function () {
        //ngDialog.close($scope.objectTreeDragDropDialog.ID);
        $scope.objectTreeDragDropDialog.close();
    };

}]);