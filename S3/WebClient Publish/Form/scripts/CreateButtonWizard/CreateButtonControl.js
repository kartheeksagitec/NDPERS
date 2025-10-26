app.controller("CreateButtonController", ["$scope", "$rootScope", function ($scope, $rootScope) {
    $scope.ErrorMessageForDisplay = "";
    $scope.strButtonType = "sfwButton";
    $scope.strCustomMethod = "";

    //#region Init Methods
    $scope.Init = function () {
        $scope.ButtonsDetailsCollection = [];
        $scope.LoadButtonDetails();
        $scope.LoadNewButtonList();
    };

    $scope.LoadButtonDetails = function () {
        var type = $scope.formodel.dictAttributes.sfwType;
        function AddInButtonsDetailsCollection(objBtnMethod) {
            var buttonDetails = {};
            var attr = null;
            attr = objBtnMethod.Attribute;
            buttonDetails.Method = objBtnMethod.Code;
            buttonDetails.Description = objBtnMethod.Description;
            buttonDetails.Category = objBtnMethod.Category;

            $scope.ButtonsDetailsCollection.push(buttonDetails);
        }
        if (type) {
            var lst = [];
            if (type == "Lookup") {
                lst = $rootScope.LstButtonMethodLookup;
            }
            else if (type == "Wizard") {
                lst = $rootScope.LstButtonMethodWizard;
            }
            else if (type == "Maintenance" || type == "UserControl") {
                lst = $rootScope.LstButtonMethodMaintenance;
            }

            if (lst != null) {
                angular.forEach(lst, AddInButtonsDetailsCollection);


            }
        }
    };

    $scope.LoadNewButtonList = function () {
        lstNewButtons = [];

        var strFormType = $scope.formodel.dictAttributes.sfwType;
        if (strFormType == "Lookup") {
            lstNewButtons.push({ Group: "New", MethodName: "btnNew_Click" });
            lstNewButtons.push({ Group: "New", MethodName: "btnNewUpdate_Click" });
            lstNewButtons.push({ Group: "Open", MethodName: "btnOpenLookup" });
            lstNewButtons.push({ Group: "Open", MethodName: "btnOpenDoc_Click" });
            lstNewButtons.push({ Group: "Delete", MethodName: "btnDelete_Click" });

        }
        if (strFormType == "Maintenance" || strFormType == "Wizard") {
            lstNewButtons.push({ Group: "New", MethodName: "btnNew_Click" });
            lstNewButtons.push({ Group: "New", MethodName: "btnNewUpdate_Click" });
            lstNewButtons.push({ Group: "New", MethodName: "btnNewPopupDialog_Click" });
            lstNewButtons.push({ Group: "Open", MethodName: "btnOpen_Click" });
            lstNewButtons.push({ Group: "Open", MethodName: "btnOpenDoc_Click" });
            lstNewButtons.push({ Group: "Open", MethodName: "btnOpenPopupDialog_Click" });

            //lstNewButtons.push({ Group: "Close", MethodName: "btnClosePopupDialog_Click" });
            lstNewButtons.push({ Group: "Finish", MethodName: "btnFinishPopupDialog_Click" });

            lstNewButtons.push({ Group: "Grid", MethodName: "btnGridSearch_Click" });
            lstNewButtons.push({ Group: "Grid", MethodName: "btnGridSearchCriteriaReq_Click" });

            lstNewButtons.push({ Group: "Save", MethodName: "btnSave_Click" });
            lstNewButtons.push({ Group: "Save", MethodName: "btnSaveIgnoreReadOnly_Click" });
            lstNewButtons.push({ Group: "Save", MethodName: "btnForceSave_Click" });
            lstNewButtons.push({ Group: "Save", MethodName: "btnNoChangesSave_Click" });
            lstNewButtons.push({ Group: "Save", MethodName: "btnSaveNew_Click" });
            lstNewButtons.push({ Group: "Save", MethodName: "btnSaveAndNext_Click" });

            lstNewButtons.push({ Group: "Download", MethodName: "btnDownload_Click" });
            lstNewButtons.push({ Group: "Execute", MethodName: "btnExecuteServerMethod_Click" });
            lstNewButtons.push({ Group: "Execute", MethodName: "btnValidateExecuteBusinessMethod_Click" });
            lstNewButtons.push({ Group: "Execute", MethodName: "btnExecuteBusinessMethodSelectRows_Click" });
            lstNewButtons.push({ Group: "Execute", MethodName: "btnExecuteBusinessMethod_Click" });
            lstNewButtons.push({ Group: "Workflow", MethodName: "btnCompleteWorkflowActivities_Click" });
        }
        if (strFormType == "Wizard") {
            lstNewButtons.push({ Group: "Grid", MethodName: "btnGridSearch_Click" });
            lstNewButtons.push({ Group: "Grid", MethodName: "btnGridSearchCriteriaReq_Click" });

            lstNewButtons.push({ Group: "Execute", MethodName: "btnExecuteServerMethod_Click" });
        }
    };

    //#endregion

    //#region Common Events


    $scope.AddControlToCell = function (cntrlName, sfxControlModel) {
        if (!sfxControlModel) {
            sfxControlModel = CreateControl($scope.formodel, $scope.item, cntrlName);
        }

        if (sfxControlModel != null) {
            if (["btnCompleteWorkflowActivities_Click", "btnExecuteBusinessMethodSelectRows_Click", "btnValidateExecuteBusinessMethod_Click", "btnExecuteBusinessMethod_Click"].indexOf(sfxControlModel.dictAttributes.sfwMethodName) > -1) {
                sfxControlModel.dictAttributes.sfwExecuteMethodType = "ObjectMethod";
            }
            if ($scope.item.Name === "TemplateField") {
                var obj = $scope.item.Elements.filter(function (x) { return x.Name === "ItemTemplate" });
                if (obj && obj.length > 0) {
                    $rootScope.PushItem(sfxControlModel, obj[0].Elements);
                }
            }
            else {
                $rootScope.PushItem(sfxControlModel, $scope.item.Elements);
            }

            SetFormSelectedControl($scope.formodel, sfxControlModel);
            //CheckAndUpdateSelectedControlStatus(this.MirrorElements[this.MirrorElements.Count - 1] as SfxControlVM, false);
            //this.PopulateObjectID(this.ObjVM.Model, sfxControlModel);
        }
    };


    $scope.selectButton = function (param) {
        $scope.SelectedButtonDetail = param;
    };

    $scope.onAddButtonOkClick = function (strControlName) {
        if ($scope.SelectedButtonDetail && $scope.SelectedButtonDetail.Method) {
            var isDialogClose = true;
            var obj;
            for (i = 0; i < lstNewButtons.length; i++) {
                if (lstNewButtons[i].MethodName == $scope.SelectedButtonDetail.Method) {
                    obj = lstNewButtons[i];
                    break;
                }
            }

            if (strControlName && !obj) {
                if ($scope.item.Name == "sfwColumn" || $scope.item.Name == "TemplateField" || $scope.item.Name == "ItemTemplate" || $scope.item.Name == "sfwButtonGroup") {
                    $scope.AddControlToCell(strControlName + "." + $scope.SelectedButtonDetail.Method);
                }
            }

            if (obj) {
                if ($scope.item.Name == "TemplateField" && ($scope.SelectedButtonDetail.Method == "btnGridSearch_Click"
                    || $scope.SelectedButtonDetail.Method == "btnGridSearchCriteriaReq_Click" || "btnOpenPopupDialog_Click")) {
                    $scope.AddControlToCell(strControlName + "." + $scope.SelectedButtonDetail.Method);
                }
                else {

                    isDialogClose = false;
                    var newScopeButtonWizard = $scope.$new();
                    newScopeButtonWizard.formodel = $scope.formodel;
                    newScopeButtonWizard.sfxCell = $scope.item;
                    newScopeButtonWizard.SelectedButtonDetails = $scope.SelectedButtonDetail;
                    newScopeButtonWizard.ControlName = strControlName;

                    $rootScope.IsLoading = true;

                    newScopeButtonWizard.beforeClose = function () {
                        $scope.onAddButtonCancelClick();
                    };
                    newScopeButtonWizard.title = "Create New Button";
                    newScopeButtonWizard.ButtonWizardDialog = $rootScope.showDialog(newScopeButtonWizard, newScopeButtonWizard.title, "Form/views/CreateButtonWizard/NewButtonWizard.html", { width: 1000, height: 660, beforeClose: newScopeButtonWizard.beforeClose });
                }
            }
        }
        else if ($scope.strButtonType == "CustomButton" && $scope.strCustomMethod) {
            var isDialogClose = true;
            $scope.AddControlToCell("sfwButton" + "." + $scope.strCustomMethod);
        }

        if (isDialogClose) {
            $scope.onAddButtonCancelClick();
        }
    };

    $scope.onAddButtonCancelClick = function () {
        //ngDialog.close($scope.CreateButtondialog.id);
        $scope.CreateButtonDialog.close();
    };

    $scope.$on('AddButtonCancel', function () {
        $scope.onAddButtonCancelClick();
    });

    $scope.CreateDummyObject = function (objButtonDetails, group, controlName) {


    };

    $scope.onChangeButtonType = function () {
        $scope.SelectedButtonDetail = {};
        $scope.strCustomMethod = "";
        if ($scope.strButtonType == "sfwImageButton") {
            var buttonDetails = {};
            buttonDetails.Method = "btnRetrieve_Click";
            buttonDetails.Description = "Retrieve Button";
            buttonDetails.Category = "ImageButton";
            $scope.ButtonsDetailsCollection.push(buttonDetails);
            $scope.SelectedButtonDetail = buttonDetails;
        }
        else {
            var lst = $scope.ButtonsDetailsCollection.filter(function (x) { return x.Method == "btnRetrieve_Click"; });
            if (lst && lst.length > 0) {
                var index = $scope.ButtonsDetailsCollection.indexOf(lst[0]);
                $scope.ButtonsDetailsCollection.splice(index, 1);
            }
        }
    };

    //#endregion

    //#region Validation Methods

    $scope.ValidateButton = function () {

        if ($scope.SelectedButtonDetail && $scope.SelectedButtonDetail.Method) {
            $scope.ErrorMessageForDisplay = "";
            return false;
        }
        else if ($scope.strButtonType == "CustomButton" && $scope.strCustomMethod) {
            $scope.ErrorMessageForDisplay = "";
            if (!isValidIdentifier($scope.strCustomMethod, false, false)) {
                $scope.ErrorMessageForDisplay = "Invalid method name";
                return true;
            } else {
                return false;
            }
        }
        $scope.ErrorMessageForDisplay = "Select Button Method";

        return true;
    };

    //#endregion

    //#region Call Init Methods
    $scope.Init();
    //#endregion
}]);