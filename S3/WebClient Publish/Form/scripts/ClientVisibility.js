app.controller("ClientVisibilityController", ["$scope", "$rootScope", "$filter", "ngDialog", "$EntityIntellisenseFactory", "$SgMessagesService", function ($scope, $rootScope, $filter, ngDialog, $EntityIntellisenseFactory, $SgMessagesService) {

    //#region Variables
    $scope.FieldCollection = [];
    $scope.ControlCollection = [];
    $scope.SavedControlList = [];
    $scope.arrSavedControlIdList = [];
    $scope.CodeValues = [];
    $scope.lstQryField = [];
    $scope.lstItems = [];
    //#endregion

    //#region Init Methods

    $scope.InitForm = function () {

        $scope.IsWizard = $scope.formobject.dictAttributes.sfwType == 'Wizard' ? true : false;
        $scope.SetClientVisibility();
        $scope.ClientVisibility = $scope.model.dictAttributes[$scope.strAttributeName];

        $scope.LoadSavedData();

        angular.forEach($scope.SavedControlList, function (sfxClntData) {
            angular.forEach(sfxClntData.arrVisibleControl, function (lbItem) {
                if (lbItem.Content) {
                    if (!$scope.ControlCollection.some(function (itm) { return itm.Content == lbItem.Content; })) {
                        $scope.ControlCollection.push({ Content: lbItem.Content });
                    }
                }
            });
        });

        angular.forEach($scope.arrSavedControlIdList, function (str) {
            if (str) {
                if (!$scope.ControlCollection.some(function (itm) { return itm.Content == str; })) {
                    $scope.ControlCollection.push({ Content: str });
                }
            }
        });

        $scope.PopulateAvailableFields();
        $scope.SetRemoveButtonText();
    };

    $scope.InitFormLink = function () {
        $scope.IsWizard = $scope.formobject.dictAttributes.sfwType == 'Wizard' ? true : false;
        $scope.SetClientVisibility();
        $scope.ClientVisibility = $scope.model.dictAttributes[$scope.strAttributeName];
        $scope.LoadSavedData();

        angular.forEach($scope.SavedControlList, function (sfxClntData) {
            angular.forEach(sfxClntData.arrVisibleControl, function (lbItem) {
                if (lbItem.Content) {
                    if (!$scope.ControlCollection.some(function (itm) { return itm.Content == lbItem.Content; })) {
                        $scope.ControlCollection.push({ Content: lbItem.Content });
                    }
                }
            });
        });

        angular.forEach($scope.arrSavedControlIdList, function (str) {
            if (str) {
                if (!$scope.ControlCollection.some(function (itm) { return itm.Content == str; })) {
                    $scope.ControlCollection.push({ Content: str });
                }
            }
        });

        $scope.PopulateHtxAvailableFields();
        $scope.SetRemoveButtonText();

    };
    //#endregion

    //#region Common Methods
    //#region Set client Visibility Implementation

    $scope.SetClientVisibility = function () {
        $scope.CodeValues.push({ CodeValue: "default", CodeValueDescription: "default" });// default value for Code
        $scope.lstQryField.push({ CodeID: "default", CodeValue: "default" });//default value for Query


        if ($scope.model.dictAttributes.sfwLoadType == 'CodeGroup') {
            $scope.CheckAndPopulateCodeIDValues($scope.formobject);
        }
        else if ($scope.model.dictAttributes.sfwLoadType == 'Query') {
            $scope.GetQuerySchema($scope.model.dictAttributes.sfwLoadSource);
        }
        else if ($scope.model.dictAttributes.sfwLoadType == 'Items') {
            if ($scope.model.Elements.length > 0) {
                $scope.GetItemsList($scope.model);
            }
        }
    };
    //#endregion
    $scope.CheckAndPopulateCodeIDValues = function (formObj) {
        if ($scope.model.dictAttributes.sfwLoadSource) {
            $.connection.hubMain.server.getCodeValues("ScopeId_" + $scope.$id, $scope.model.dictAttributes.sfwLoadSource);
        }
        else if ($scope.model.placeHolder) {
            $.connection.hubMain.server.getCodeValues("ScopeId_" + $scope.$id, $scope.model.placeHolder);
        }

    };

    $scope.receiveList = function (data) {
        $scope.$evalAsync(function () {
            if (data && data.length > 0) {
                if ($scope.CodeValues && $scope.CodeValues.length > 0) {
                    $scope.CodeValues = $scope.CodeValues.concat(data);
                } else {
                    $scope.CodeValues = data;
                }
            }
        });
    };

    $scope.LoadSavedData = function () {
        var objSfxClntVisiblityData;

        if ($scope.ClientVisibility != undefined && $scope.ClientVisibility != "") {
            var arrControlList = $scope.ClientVisibility.split("#");

            arrControlList[0] = arrControlList[0].substring(arrControlList[0].indexOf(":") + 1);
            var strTotalControls = arrControlList[0].split(",");
            for (var i = 0; i < strTotalControls.length; i++) {
                $scope.arrSavedControlIdList.push(strTotalControls[i]);
            }
            var iblnIsDefaultRowExists = false;
            if (arrControlList.length > 1) {
                var strValues = arrControlList[1].split(";");

                for (var i = 0; i < strValues.length; i++) {
                    if (strValues[i] != undefined && strValues[i] != "") {
                        objSfxClntVisiblityData = { arrVisibleControl: [] };
                        var strInnerValues = strValues[i].split(":");
                        objSfxClntVisiblityData.Value = strInnerValues[0];
                        if ($scope.lstItems && $scope.lstItems.length > 0) {
                            var items = $scope.lstItems.filter(function (x) { return x.dictAttributes.Value && x.dictAttributes.Value == strInnerValues[0] });
                            if (items && items.length > 0) {
                                objSfxClntVisiblityData.Value = items[0].dictAttributes.Text;
                            }

                        }
                        
                        if (objSfxClntVisiblityData.Value.trim() == "default") {
                            iblnIsDefaultRowExists = true;
                        }
                        if (strInnerValues.length > 1) {
                            var strVisibleControls = strInnerValues[1].split(",");
                            for (var j = 0; j < strVisibleControls.length; j++) {
                                var lbItem = {};

                                lbItem.Content = strVisibleControls[j];
                                objSfxClntVisiblityData.arrVisibleControl.push(lbItem);
                            }
                        }
                        $scope.SavedControlList.push(objSfxClntVisiblityData);
                    }
                }
            }
            if (!iblnIsDefaultRowExists && $scope.SavedControlList.length == 0) {
                objSfxClntVisiblityData = { arrVisibleControl: [] };
                objSfxClntVisiblityData.Value = "default";
                $scope.SavedControlList.push(objSfxClntVisiblityData);
            }
        }
    };

    $scope.PopulateAvailableFields = function () {

        $scope.FieldCollection = [];
        var iblnCtrlFound = false;
        var mainItem = { Text: $scope.formobject.dictAttributes.ID, IsExpanded: true, IsSelected: false, Items: [], IsCheckBoxVisible: false };
        var table;
        if ($scope.IsWizard || $scope.formobject.dictAttributes.sfwType == "Correspondence") {
            if ($scope.CurrentTable) {
                table = $scope.CurrentTable;
            }
        }
        else {
            for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                if ($scope.formobject.Elements[i].Name == "sfwTable") {
                    table = $scope.formobject.Elements[i];
                    break;
                }
            }
        }

        $scope.PopulateControls(table, mainItem, iblnCtrlFound);
        //$scope.PopulateControls($rootScope.MainTable, mainItem, iblnCtrlFound);
        if (mainItem) {
            $scope.FieldCollection.push(mainItem);
        }
    };

    $scope.PopulateControls = function (asfxTable, tnNode, iblnCtrlFound) {
        var strTreeCaption = "";
        if (asfxTable) {
            strTreeCaption = asfxTable.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
            if (strTreeCaption == "" || strTreeCaption == undefined)
                strTreeCaption = asfxTable.Name; //.istrControlClass;
            var tnTable = { Text: strTreeCaption, IsSelected: false, Items: [], IsCheckBoxVisible: true };
            if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                $scope.arrSavedControlIdList.splice(index, 1);
                tnTable.IsSelected = true;
            }
            angular.forEach(asfxTable.Elements, function (sfxRow) {
                for (var iCol = 0; iCol < sfxRow.Elements.length; iCol++) {
                    var sfxCell = sfxRow.Elements[iCol];
                    if (sfxCell) {
                        angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                            if (sfxCtrl == $scope.model) {
                                iblnCtrlFound = true;
                            }
                            if (sfxCtrl.dictAttributes.ID == "" || sfxCtrl.dictAttributes.ID == undefined) {

                            }
                            else {
                                if (sfxCtrl.Name == "sfwPanel" || sfxCtrl.Name == "sfwDialogPanel") {
                                    strTreeCaption = sfxCtrl.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
                                    if (strTreeCaption == "" || strTreeCaption == undefined)
                                        strTreeCaption = sfxCtrl.Name;
                                    var tnChild = { Text: strTreeCaption, IsSelected: false, Items: [], IsCheckBoxVisible: true };
                                    if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                        var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                        $scope.arrSavedControlIdList.splice(index, 1);
                                        tnChild.IsSelected = true;
                                    }
                                    if (!iblnCtrlFound) {
                                        // tnChild.IsExpanded = true;
                                    }
                                    if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "sfwTable") {
                                        $scope.PopulateControls(sfxCtrl.Elements[0], tnChild, iblnCtrlFound);
                                    }
                                    tnTable.Items.push(tnChild);
                                }
                                else if (sfxCtrl.Name == "sfwWizard") {
                                    strTreeCaption = sfxCtrl.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
                                    if (strTreeCaption == "" || strTreeCaption == undefined)
                                        strTreeCaption = sfxCtrl.Name;
                                    var tnWizardCtrl = { Text: strTreeCaption, Items: [], IsCheckBoxVisible: true };
                                    if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                        var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                        $scope.arrSavedControlIdList.splice(index, 1);
                                        //arrSavedControlIdList.Remove(strTreeCaption);
                                        tnWizardCtrl.IsSelected = true;
                                    }

                                    var tnWizardStep = { Text: "WizardStep", Items: [], IsCheckBoxVisible: true };
                                    tnWizardCtrl.Items.push(tnWizardStep);
                                    angular.forEach(sfxCtrl.Elements, function (objWizard) {
                                        if (objWizard.Name == "HeaderTemplate") {
                                            var tnWizardHeader = { Text: "HeaderTemplate", Items: [], IsCheckBoxVisible: true };
                                            if (objWizard.Elements.length > 0 && objWizard.Elements[0].Name == "sfwTable") {
                                                $scope.PopulateControls(objWizard.Elements[0], tnWizardHeader, iblnCtrlFound);
                                            }
                                            tnWizardCtrl.Items.push(tnWizardHeader);

                                        }
                                        else if (objWizard.Name == "WizardSteps") {
                                            angular.forEach(objWizard.Elements, function (SfxWizardStep) {
                                                strTreeCaption = SfxWizardStep.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
                                                if (strTreeCaption == "" || strTreeCaption == undefined)
                                                    strTreeCaption = SfxWizardStep.Name;
                                                var tnChild = { Text: strTreeCaption, Items: [], IsCheckBoxVisible: true };
                                                if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                                    var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                                    $scope.arrSavedControlIdList.splice(index, 1);
                                                    //arrSavedControlIdList.Remove(strTreeCaption);
                                                    tnChild.IsSelected = true;
                                                }
                                                if (SfxWizardStep.Elements.length > 0 && SfxWizardStep.Elements[0].Name == "sfwTable") {
                                                    $scope.PopulateControls(SfxWizardStep.Elements[0], tnChild, iblnCtrlFound);
                                                }
                                                tnWizardStep.Items.push(tnChild);
                                            });

                                        }
                                        else {
                                            var tnWizardSideBar = { Text: "SideBarTemplate", Items: [], IsCheckBoxVisible: true };
                                            if (objWizard.Elements.length > 0 && objWizard.Elements[0].Name == "sfwTable") {
                                                $scope.PopulateControls(objWizard.Elements[0], tnWizardSideBar, iblnCtrlFound);
                                            }
                                            tnWizardCtrl.Items.push(tnWizardSideBar);
                                        }

                                    });
                                    tnTable.Items.push(tnWizardCtrl);
                                }
                                else if (sfxCtrl.Name == "sfwTabContainer") {
                                    strTreeCaption = sfxCtrl.dictAttributes.ID;  //[ApplicationConstants.XMLFacade.ID].Trim();
                                    if (strTreeCaption == "" || strTreeCaption == undefined)
                                        strTreeCaption = sfxCtrl.Name;
                                    var tnTab = { Text: strTreeCaption, Items: [], IsCheckBoxVisible: true };
                                    if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                        var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                        $scope.arrSavedControlIdList.splice(index, 1);
                                        // $scope.arrSavedControlIdList.Remove(strTreeCaption);
                                        tnTab.IsSelected = true;
                                    }
                                    if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Tabs") {
                                        var sfxTabs = sfxCtrl.Elements[0];
                                        angular.forEach(sfxTabs.Elements, function (sfxTabSheet) {
                                            strTreeCaption = sfxTabSheet.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
                                            if (strTreeCaption == "")
                                                strTreeCaption = sfxTabSheet.Name;
                                            var tnChild = { Text: strTreeCaption, Items: [], IsCheckBoxVisible: true };
                                            if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                                var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                                $scope.arrSavedControlIdList.splice(index, 1);
                                                //arrSavedControlIdList.Remove(strTreeCaption);
                                                tnChild.IsSelected = true;
                                            }
                                            if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "sfwTable") {
                                                $scope.PopulateControls(sfxTabSheet.Elements[0], tnChild, iblnCtrlFound);
                                            }
                                            tnTab.Items.push(tnChild);
                                        });
                                    }
                                    tnTable.Items.push(tnTab);
                                }
                                else {
                                    strTreeCaption = sfxCtrl.dictAttributes.ID; //[ApplicationConstants.XMLFacade.ID].Trim();
                                    if (strTreeCaption == "" || strTreeCaption == undefined)
                                        strTreeCaption = sfxCtrl.Name;
                                    var tnControl = { Text: strTreeCaption, Items: [], IsCheckBoxVisible: true };
                                    tnTable.Items.push(tnControl);
                                    if ($scope.arrSavedControlIdList && $scope.arrSavedControlIdList.length > 0 && $scope.arrSavedControlIdList.some(function (x) { return x == strTreeCaption; })) {
                                        var index = $scope.arrSavedControlIdList.indexOf(strTreeCaption);
                                        $scope.arrSavedControlIdList.splice(index, 1);
                                        //arrSavedControlIdList.Remove(strTreeCaption);
                                        tnControl.IsSelected = true;
                                    }
                                }
                            }
                        });
                    }
                }
            });
            tnNode.Items.push(tnTable);
        }
    };

    $scope.SetRemoveButtonText = function () {
        if ($scope.strAttributeName == "sfwClientEnable") {
            $scope.StrRemoveButtonText = "Remove Client Enable ";
            $scope.StrControlHeader = "Enabled Control(s)";
        }
        else if ($scope.strAttributeName == "sfwClientVisibility") {
            $scope.StrRemoveButtonText = "Remove Client Visibility";
            $scope.StrControlHeader = "Visible Control(s)";
        }


    };

    $scope.GetQuerySchema = function (queryId) {
        if ($scope.model.dictAttributes.sfwLoadSource) {

            $.connection.hubForm.server.getQueryColumnValues($scope.model.dictAttributes.sfwLoadSource, $scope.model.dictAttributes.DataValueField).done(function (data) {
                $scope.$evalAsync(function () {
                    if (data && data.length > 0) {
                        {
                            if ($scope.lstQryField && $scope.lstQryField.length > 0) {
                                $scope.lstQryField = $scope.lstQryField.concat(data);
                            } else {
                                $scope.lstQryField = data;
                            }

                        }
                    }
                });
            });
        }
    };

    $scope.GetItemsList = function (model) {
        angular.forEach(model.Elements, function (itm) {
            if (itm.Name == 'ListItem') {
                $scope.lstItems.push(itm);
            }
        });
        $scope.lstItems.splice(0, 0, { dictAttributes: { Text: 'default' } });

    };


    //#endregion

    //#region Update Data Methods
    $scope.GetDataFromGrid = function () {
        var strCellValue = "";
        var strListValues = "";
        angular.forEach($scope.ControlCollection, function (lvControl) {
            if (strListValues == undefined || strListValues == "") {
                strListValues = lvControl.Content;
            }
            else {
                strListValues = strListValues + "," + lvControl.Content;
            }
        });
        strListValues = "controllist:" + strListValues;
        var strcontrollist = strListValues;
        strListValues = "";
        angular.forEach($scope.SavedControlList, function (data) {
            strCellValue = data.Value;
            if ($scope.lstItems && $scope.lstItems.length > 0) {
                var itmValue = $scope.lstItems.filter(function (x) { return x.dictAttributes.Value && x.dictAttributes.Text == data.Value });
                if (itmValue && itmValue.length > 0) {
                    strCellValue = itmValue[0].dictAttributes.Value;
                }
                else {
                    strCellValue = data.Value;
                }
            }
            var strVisibleControls = "";
            angular.forEach(data.arrVisibleControl, function (lvItem) {
                if (strVisibleControls == undefined || strVisibleControls == "") {
                    strVisibleControls = lvItem.Content;
                }
                else {
                    strVisibleControls = strVisibleControls + "," + lvItem.Content;
                }
            });

            strListValues = strListValues + strCellValue + ":" + strVisibleControls + ";";
        });
        if (strListValues != undefined && strListValues != "") {
            strListValues = strcontrollist + "#" + strListValues;
        }
        return strListValues;
    };

    $scope.UpdateClientVisibility = function () {
        $rootScope.EditPropertyValue($scope.model.dictAttributes[$scope.strAttributeName], $scope.model.dictAttributes, $scope.strAttributeName, $scope.ClientVisibility);
    };

    //#endregion 

    //#region Common Events
    $scope.ExpandCollapsedControl = function (field, event) {
        field.IsExpanded = !field.IsExpanded;
    };

    $scope.SelectFieldClick = function (field, event) {
        $scope.SelectedField = field;
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.SetFieldClass = function (obj) {
        if (obj == $scope.SelectedField) {
            return "selected";
        }
    };

    $scope.onRemoveClientVisibilityClick = function () {
        $scope.ClientVisibility = "";
        $scope.UpdateClientVisibility();

        $scope.onCancelClick();
    };

    $scope.onSaveClick = function () {
        $scope.ClientVisibility = $scope.GetDataFromGrid();
        $scope.UpdateClientVisibility();

        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        //ngDialog.close($scope.clientVisibilityDialog.id);

        $scope.objNewDialog.close();
    };

    $scope.SelectControlFromList = function (obj) {
        $scope.SelectedControl = obj;
    };

    $scope.SelectSaveControlClick = function (saveCtrl) {
        $scope.SelectedVisibility = saveCtrl;
    };

    $scope.SelectVisibleControlClick = function (ctrl) {
        $scope.SelectedItem = ctrl;
    };

    $scope.AddToListboxClick = function () {
        if ($scope.FieldCollection.length > 0) {
            $scope.AddFieldsToControlCollection($scope.FieldCollection[0].Items);
        }
    };
    $scope.AddFieldsToControlCollection = function (fieldCollection) {
        angular.forEach(fieldCollection, function (field) {
            if (field.IsSelected && field.Text) {
                var lstItem = { Content: field.Text };
                if (!$scope.ControlCollection.some(function (x) { return x.Content == field.Text; })) {
                    $scope.ControlCollection.push(lstItem);
                }
                else {
                    //MessageBox.Show("The control " + field.Text + " is already added.", "Error");
                }
            }
            if (field.Items.length > 0) {
                $scope.AddFieldsToControlCollection(field.Items);
            }
        });
    };

    $scope.DeleteToListboxClick = function () {
        if ($scope.SelectedControl != null) {
            var index = $scope.ControlCollection.indexOf($scope.SelectedControl);
            $scope.ControlCollection.splice(index, 1);

            angular.forEach($scope.SavedControlList, function (saveditem) {
                angular.forEach(saveditem.arrVisibleControl, function (lvItem) {
                    if (lvItem.Content == $scope.SelectedControl.Content) {
                        var index = saveditem.arrVisibleControl.indexOf(lvItem);
                        saveditem.arrVisibleControl.splice(index, 1);
                    }
                });
            });

            if (index < $scope.ControlCollection.length) {
                $scope.SelectedControl = $scope.ControlCollection[index];
            }
            else if ($scope.ControlCollection.length > 0) {
                $scope.SelectedControl = $scope.ControlCollection[index - 1];
            }
            if ($scope.ControlCollection.length == 0) {
                $scope.SelectedControl = undefined;
            }
        }
        else {
            $SgMessagesService.Message('Message', "Select control to remove.");
        }
    };

    $scope.AddToGridClick = function () {

        if ($scope.ControlCollection.length == 0) {
            $SgMessagesService.Message('Message', "Please add some control in List.");
        }
        else {
            $scope.AddRowToGrid();
        }
    };

    $scope.AddRowToGrid = function () {
        var objClientVisibilityData = { Value: '', arrVisibleControl: [] };
        if ($scope.SavedControlList.length == 0) {
            objClientVisibilityData.Value = "default";
            angular.forEach($scope.ControlCollection, function (lvItem) {
                var lstItem = { Content: lvItem.Content };
                objClientVisibilityData.arrVisibleControl.push(lstItem);
            });
        }

        $scope.SavedControlList.push(objClientVisibilityData);
    };

    $scope.DeleteToGridClick = function () {
        if ($scope.SelectedVisibility != null) {
            var index = $scope.SavedControlList.indexOf($scope.SelectedVisibility);
            $scope.SavedControlList.splice(index, 1);
            if (index < $scope.SavedControlList.length) {
                $scope.SelectedVisibility = $scope.SavedControlList[index];
            }
            else if ($scope.SavedControlList.length > 0) {
                $scope.SelectedVisibility = $scope.SavedControlList[index - 1];
            }
            if ($scope.SavedControlList.length == 0) {
                $scope.SelectedVisibility = undefined;
            }

        }
    };

    $scope.DeleteSaveControlFromList = function () {
        if ($scope.SelectedVisibility && $scope.SelectedItem) {
            var index = $scope.SelectedVisibility.arrVisibleControl.indexOf($scope.SelectedItem);
            $scope.SelectedVisibility.arrVisibleControl.splice(index, 1);
            if (index < $scope.SelectedVisibility.arrVisibleControl.length) {
                $scope.SelectedItem = $scope.SelectedVisibility.arrVisibleControl[index];
            }
            else if ($scope.SelectedVisibility.arrVisibleControl.length > 0) {
                $scope.SelectedItem = $scope.SelectedVisibility.arrVisibleControl[index - 1];
            }
        }
    };

    //#endregion 

    //#region Context Menu for Saved Item
    $scope.SaveControlListMenu = [['Delete', function ($itemScope) {
        $scope.SelectedItem = $itemScope.ctrl;
        if ($itemScope.$parent && $itemScope.$parent.saveCtrl) {
            $scope.SelectedVisibility = $itemScope.$parent.saveCtrl;
        }
        $scope.DeleteSaveControlFromList();
    }, null]
    ];

    //#endregion


    //#region Form Link Controls
    $scope.PopulateHtxAvailableFields = function (isWizard) {
        $scope.FieldCollection = [];
        if (isWizard) {
            var objItems = $scope.formobject.Elements.filter(function (itm) { return itm.Name == "items"; });
            if (objItems && objItems.length > 0) {
                //adding wizard
                var objWizard = objItems[0].Elements.filter(function (ele) { return ele.Name == "sfwWizard"; });
                if (objWizard && objWizard.length > 0) {

                    var objWizardField = { Text: objWizard.dictAttributes.ID, IsSelected: false, Items: [] };
                    if (objWizardField) {
                        //adding selected wizardStep
                        var objWizardStepModel = $scope.GetSelectedWizardStep($scope.model);
                        if (objWizardStepModel) {
                            var objFiled = $scope.CrateHtxAvailableField(objWizardStepModel);
                            if (objFiled) {
                                objFiled.IsCheckBoxVisible = false;
                                objWizardField.Items.push(objFiled);
                            }
                        }
                        objWizardField.IsExpanded = true;
                        $scope.FieldCollection.push(objWizardField);
                    }
                }
            }
        }
        else {
            var objFiled = $scope.CrateHtxAvailableField($scope.formobject);
            if (objFiled) {
                objFiled.IsExpanded = true;
                objFiled.IsCheckBoxVisible = false;
                $scope.FieldCollection.push(objFiled);
            }
        }
    };

    $scope.GetSelectedWizardStep = function (aCntrlModel) {
        var retVal = null;
        var parent = aCntrlModel.ParentVM;
        while (parent) {
            if (parent.Name == "sfwWizardStep") {
                retVal = parent;
                break;
            }
            parent = parent.ParentVM;
        }
        return retVal;
    };

    $scope.CrateHtxAvailableField = function (aModel) {
        function AddInobjFiled(cntrlModel) {
            var objChild = $scope.CrateHtxAvailableField(cntrlModel);
            if (objChild) {
                objFiled.Items.push(objChild);
            }
        }
        var objFiled = null;
        if (aModel.dictAttributes.ID != undefined && aModel.dictAttributes.ID != "") {

            objFiled = { Text: aModel.dictAttributes.ID, IsCheckBoxVisible: true, Items: [], IsSelected: $scope.arrSavedControlIdList.some(function (itm) { return itm == aModel.dictAttributes.ID; }) };
            var objItemsModel = null;
            if (aModel.Name == "sfwTabContainer") {
                objItemsModel = aModel.Elements.filter(function (itm) { return itm.Name == "Tabs"; });
            }
            else {
                objItemsModel = aModel.Elements.filter(function (itm) { return itm.Name == "items"; });
            }
            if (objItemsModel && objItemsModel.length > 0) {

                angular.forEach(objItemsModel[0].Elements, AddInobjFiled);
            }
        }

        return objFiled;
    };

    //#endregion


    //#region Call Init method
    if ($scope.IsForm) {
        $scope.InitForm();
    }
    else {
        $scope.InitFormLink();
    }
    //#endregion
}]);

app.directive("clientvisibilitydraggable", [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.draggable = true;

            el.addEventListener('dragstart', onDragStart, false);

            function onDragStart(e) {
                if (scope.dragdata != undefined && scope.dragdata != "") {
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                }
            }
        }
    };
}]);

app.directive("clientvisibilitydroppable", [function () {
    return {
        restrict: 'A',
        scope: {
            dropdata: '=',
        },
        link: function (scope, element, attributes) {
            var el = element[0];

            el.addEventListener("dragover", function (e) {
                e.dataTransfer.dropEffect = 'copy';
                if (e.preventDefault) {
                    e.preventDefault();
                }
            });

            el.addEventListener("drop", function (e) {
                e.preventDefault();
                var data = JSON.parse(e.dataTransfer.getData("text"));
                if (data != undefined && data != "") {
                    scope.$apply(function () {
                        if (jQuery.type(scope.dropdata) === "array") {
                            if (!scope.dropdata.some(function (x) { return x.Content == data.Content; })) {
                                scope.dropdata.push(data);
                            }
                        }
                    });
                }
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }, false);
        }
    };
}]);