app.controller("NewButtonWizardController", ["$scope", "$http", "$rootScope", "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", "$GetEntityFieldObjectService", "$Entityintellisenseservice", "$SgMessagesService", "$timeout", "$GetGridEntity", function ($scope, $http, $rootScope, $EntityIntellisenseFactory, $ValidationService, CONST, $GetEntityFieldObjectService, $Entityintellisenseservice, $SgMessagesService, $timeout, $GetGridEntity) {
    //#region Variables
    $scope.objNewButtonDialogForGrid = {};
    $scope.objCreateNewGrid = {};
    $scope.objGridObjectField = {};
    $scope.objGridObjectField.lstselectedmultiplelevelfield = [];
    $scope.objGridField = {};
    $scope.objNewButtonActiveForm = {};
    $scope.objNewButtonNaviagtionParam = {};
    $scope.objNewButtonGeneralProp = {};
    $scope.objNewButtonCustomAttr = {};
    $scope.objNewButtonFilterGrid = {};
    $scope.objNewButtonFilterGridQueryParam = {};
    $scope.objNewButtonFilterGridCriteria = {};
    $scope.objNewButtonServerMethod = {};
    $scope.objNewButtonOpenWordDoc = {};
    $scope.lstMode = [];
    $scope.IsPopupStepDisable = true;
    $scope.IsNewButtonStepDisable = true;
    $scope.IsNewGridStepDisable = true;
    $scope.IsNewFilterGridSearchCriteriaStepDisable = true;
    $scope.IsNewSaveStepsDisable = true;
    $scope.IsNewExecuteStepsDisable = true;
    $scope.IsOpenWordDocStepsDisable = true;
    $scope.IsNewExecuteBusinessStepsDisable = true;
    $scope.IsGeneralPropStepsDisable = true;
    $scope.objNewButtonCustomAttr.CustomAttributeText = "Custom Attributes";
    $scope.ControlTypes = ['', 'TextBox', 'DropDownList'];
    $scope.lstControlTypes = ['', 'Label'];
    $scope.objFilterGridFieldsDetails = {};
    $scope.objFilterGridFieldsDetails.AvailableFieldColletion = [];
    $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid = [];
    $scope.objFilterGridFieldsDetails.GridNonAvailableControlCollection = [];
    $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection = [];
    $scope.objNewButtonFilterGridCriteria.AvailableFieldColletion = [];
    $scope.IsParentAsCollection = false;
    $scope.entityName = undefined;
    if ($scope.formodel && $scope.formodel.dictAttributes.sfwEntity) {
        $scope.entityName = $scope.formodel.dictAttributes.sfwEntity;
    }

    if ($scope.ParentModel && $scope.ParentModel.dictAttributes.sfwEntityField) {
        $scope.IsParentAsCollection = true;
        var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formodel.dictAttributes.sfwEntity, $scope.ParentModel.dictAttributes.sfwEntityField);
        if (entObject) {
            $scope.entityName = entObject.Entity;
        }

    }


    //#endregion

    //#region Init Methods
    $scope.Init = function () {
        $scope.sfxControl = { Name: $scope.ControlName, Value: "", prefix: 'swc', dictAttributes: { Visible: 'True' }, Elements: [], Children: [] };
        CheckAndSetSfwButtonDefaultValues($scope.formodel, $scope.sfxControl, $scope.SelectedButtonDetails.Method);
        if ($scope.SelectedButtonDetails.Method == "btnOpenPopupDialog_Click" || $scope.SelectedButtonDetails.Method == "btnNewPopupDialog_Click"
            || $scope.SelectedButtonDetails.Method == "btnFinishPopupDialog_Click" || $scope.SelectedButtonDetails.Method == "btnClosePopupDialog_Click") {
            $scope.InitForGrid();
            $scope.IsPopupStepDisable = false;
        }
        else if ($scope.SelectedButtonDetails.Method == "btnNew_Click" || $scope.SelectedButtonDetails.Method == "btnNewUpdate_Click" || $scope.SelectedButtonDetails.Method == "btnOpen_Click") {
            $scope.InitForNewButtonMethod();
            $scope.InitForGridControl();

            $scope.IsNewButtonStepDisable = false;
        }
        else if ($scope.SelectedButtonDetails.Method == "btnGridSearch_Click") {
            $scope.IsNewGridStepDisable = false;
            $scope.objNewButtonFilterGrid.SelectGridOption = 'CreateNewGrid';
            $scope.InitForGridControl();
        }
        else if ($scope.SelectedButtonDetails.Method == "btnSave_Click" || $scope.SelectedButtonDetails.Method == "btnForceSave_Click"
            || $scope.SelectedButtonDetails.Method == "btnSaveNew_Click" || $scope.SelectedButtonDetails.Method == "btnSaveAndNext_Click"
            || $scope.SelectedButtonDetails.Method == "btnSaveIgnoreReadOnly_Click" || $scope.SelectedButtonDetails.Method == "btnNoChangesSave_Click") {
            $scope.IsNewSaveStepsDisable = false;
        }
        else if ($scope.SelectedButtonDetails.Method == "btnOpenDoc_Click") {
            $scope.IsOpenWordDocStepsDisable = false;
            $scope.InitForGridControl();
            $rootScope.IsLoading = true;
            $.connection.hubCreateNewObject.server.loadCorrespondenceTemplate(false).done(function (data) {
                $scope.receiveCorrespondenceTemplateForForm(data);
            });
        }
        else if ($scope.SelectedButtonDetails.Method == "btnExecuteServerMethod_Click" || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethod_Click"
            || $scope.SelectedButtonDetails.Method == "btnValidateExecuteBusinessMethod_Click" || $scope.SelectedButtonDetails.Method == "btnDownload_Click" || $scope.SelectedButtonDetails.Method == "btnCompleteWorkflowActivities_Click") {
            $scope.IsNewExecuteStepsDisable = false;
            $scope.objNewButtonServerMethod.Init();
        }
        else if ($scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethodSelectRows_Click") {
            $scope.IsNewExecuteBusinessStepsDisable = false;
            $scope.objNewButtonServerMethod.Init();
        }
        else {
            $scope.IsGeneralPropStepsDisable = false;
        }

        $scope.objNewButtonGeneralProp.PopulateMode();
        $scope.objNewButtonGeneralProp.IsLookup = $scope.formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";
        $scope.objNewButtonGeneralProp.PopulateSecurityLevel();
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };

    //#endregion

    //#region Common Methods
    $scope.objNewButtonGeneralProp.PopulateMode = function () {
        $scope.objNewButtonGeneralProp.lstMode = [];
        $scope.objNewButtonGeneralProp.lstMode.push("All");
        $scope.objNewButtonGeneralProp.lstMode.push("New");
        $scope.objNewButtonGeneralProp.lstMode.push("Update");
    };

    $scope.objNewButtonGeneralProp.PopulateSecurityLevel = function () {
        $scope.objNewButtonGeneralProp.lstSecurityLevel = [];
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "0", Description: " None" });
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "1", Description: " Read" });
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "2", Description: " Modify" });
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "3", Description: " New" });
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "4", Description: " Delete" });
        $scope.objNewButtonGeneralProp.lstSecurityLevel.push({ Code: "5", Description: " Execute" });
    };

    $scope.CloseWizardClick = function () {
        //ngDialog.close($scope.NewButtonMainWizardVM.id);

        $scope.ButtonWizardDialog.close();
    };
    //#endregion

    //#region Validation Methods
    $scope.objNewButtonDialogForGrid.IsNextDisable = function () {
        var IsValid = true;
        if ($scope.objNewButtonDialogForGrid.SelectGridOption == "CreateNewGrid") {
            $scope.objNewButtonDialogForGrid.ErrorMessageForDisplay = "";
            if (!$scope.objNewButtonDialogForGrid.ValidateDialogPanel()) {
                IsValid = false;
            }
        }
        return IsValid;
    };

    $scope.objNewButtonDialogForGrid.IsFinishDisable = function () {
        var retVal = true;
        if ($scope.objNewButtonDialogForGrid.SelectGridOption == "UseExistingGrid") {
            $scope.objNewButtonDialogForGrid.ErrorMessageForDisplay = "";
            if (!$scope.objNewButtonDialogForGrid.ValidateDialogPanel() && !$scope.objNewButtonDialogForGrid.ValidateGridControl()) {
                retVal = false;
            }
        }
        return retVal;
    };

    $scope.objCreateNewGrid.IsNextDisable = function () {
        var IsValid = true;
        $scope.objCreateNewGrid.ErrorMessageForDisplay = "";
        if ($scope.objCreateNewGrid.SelectedEntityField && ($scope.objCreateNewGrid.SelectedEntityField.DataType == "Collection"
            || $scope.objCreateNewGrid.SelectedEntityField.DataType == "CDOCollection"
            || $scope.objCreateNewGrid.SelectedEntityField.DataType == "List")) {
            IsValid = false;
        }
        else {
            $scope.objCreateNewGrid.ErrorMessageForDisplay = "Error: A valid collection needs to be selected for the grid.";
        }
        return IsValid;
    };

    $scope.objGridObjectField.IsNextDisable = function () {
        var IsValid = true;
        $scope.objGridObjectField.ErrorMessageForDisplay = "";
        if ($scope.objGridObjectField.lstFields && $scope.objGridObjectField.lstFields.length > 0) {
            var lstList = [];
            lstList = $scope.objGridObjectField.lstselectedmultiplelevelfield;//GetSelectedFieldList($scope.objGridObjectField.lstFields, lstList);
            if (lstList.length > 0) {
                IsValid = false;
            }
            else {
                $scope.objGridObjectField.ErrorMessageForDisplay = "Error: Please select atleast one value from the list.";
            }
        } else if ($scope.isAnyErrors()) {
            IsValid = false;
        }
        return IsValid;
    };

    $scope.objNewButtonActiveForm.IsNextDisable = function () {
        var IsValid = false;
        $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "";

        if ($scope.sfxControl.dictAttributes.ID == undefined || $scope.sfxControl.dictAttributes.ID == "") {
            $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Enter the button ID.";
            return true;
        } else if ($scope.isAnyErrors()) {
            return true;
        }
        if ($scope.objNewButtonActiveForm.ActiveFormType == "SingleForm") {
            if ($scope.objNewButtonActiveForm.StrActiveForm == undefined || $scope.objNewButtonActiveForm.StrActiveForm == "") {
                $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Select active form.";
                return true;
            }
            else if ($scope.objNewButtonActiveForm.StrActiveForm == $scope.formodel.dictAttributes.ID) {
                $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Can not select same form as active form.";
                return true;
            }
        }
        else if ($scope.objNewButtonActiveForm.ActiveFormType == "MultipleForm") {
            if ($scope.objNewButtonActiveForm.ActiveFormCollection.length == 0) {
                $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Atleast one active form has to be added.";
                return true;
            }
            var iActvCount = 0;
            for (var i = 0; i < $scope.objNewButtonActiveForm.ActiveFormCollection.length; i++) {
                var objActiveForm = $scope.objNewButtonActiveForm.ActiveFormCollection[i];
                if (objActiveForm.FieldValue == "ACTV") {
                    iActvCount++;
                }
                if (!objActiveForm.FieldValue) {
                    $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Please Select or enter field value of row number " + (i + 1);
                    return true;
                }
                if (!objActiveForm.ActiveForm) {
                    $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Please enter active form value of row number " + (i + 1);
                    return true;
                }
                else if (objActiveForm.ActiveForm == $scope.formodel.dictAttributes.ID) {
                    $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Can not select same form as active form.";
                    return true;
                }
            }
            if (iActvCount > 1) {
                $scope.objNewButtonActiveForm.ErrorMessageForDisplay = "Cannot add multiple active forms with same field value - ACTV";
                return true;
            }
            else {
                return false;
            }
        }
        return IsValid;
    };

    /******************************** First Screen Next Button Functionality*************************************************************/

    $scope.objNewButtonFilterGrid.IsNextDisable = function () {
        $scope.objNewButtonFilterGrid.ErrorMessageForDisplay = "";
        if ($scope.isAnyErrors()) {
            return true;
        }
        if ($scope.objNewButtonFilterGrid.SelectGridOption && $scope.objNewButtonFilterGrid.SelectGridOption == 'UseExistingGrid') {
            if (!$scope.sfxControl.dictAttributes.sfwRelatedControl) {
                $scope.objNewButtonFilterGrid.ErrorMessageForDisplay = "Please Select Grid View";
                return true;
            }
            else {
                var lst = $scope.objNewButtonFilterGrid.lstRelatedGrid.filter(function (x) { return x == $scope.sfxControl.dictAttributes.sfwRelatedControl; });
                if (!lst || (lst && lst.length == 0)) {
                    $scope.objNewButtonFilterGrid.ErrorMessageForDisplay = "Invalid Grid Selected";
                    return true;
                }
            }
        }

        if ($scope.objNewButtonFilterGrid.StrBaseQuery == undefined || $scope.objNewButtonFilterGrid.StrBaseQuery == "") {
            this.ErrorMessageForDisplay = "Please Select Base Query.";
            return true;
        }
        else {
            var strFile = $scope.objNewButtonFilterGrid.StrBaseQuery.split('.');
            if (strFile.length > 0) {

                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var objFile = entityIntellisenseList.filter(function (itm) { return itm.ID == strFile[0]; });
                if (objFile && objFile.length > 0 && strFile.length > 1) {
                    return $scope.objNewButtonFilterGrid.ValidateQuery(objFile[0], strFile[1]);
                }
                else {
                    $scope.objNewButtonFilterGrid.ErrorMessageForDisplay = "Invalid Base Query";
                    return true;
                }
            }
        }
        return false;
    };


    /******************************** First Screen Next Button Functionality*************************************************************/



    /******************************** Second Screen Finish Button Functionality*************************************************************/
    $scope.objNewButtonFilterGridCriteria.IsFinishDisable = function () {
        $scope.objNewButtonFilterGridCriteria.ErrorMessageForDisplay = "";
        if ($scope.objNewButtonFilterGridCriteria.StrNoOfColumn == undefined || $scope.objNewButtonFilterGridCriteria.StrNoOfColumn == "") {
            $scope.objNewButtonFilterGridCriteria.ErrorMessageForDisplay = "Please Select No of column to be Created.";
            return true;
        }
        return false;
    };


    $scope.objNewButtonServerMethod.onMethodTypeChange = function (methodType) {
        $scope.showRules = false;
        $scope.showObjectMethod = false;
        $scope.showXmlMethod = false;
        $scope.objNewButtonServerMethod.StrServerMethod = '';
        if (methodType == "ObjectMethod") {
            $scope.showObjectMethod = true;
            $scope.Title = "Object Method :";
        }
        else if (methodType == "XmlMethod") {
            $scope.showXmlMethod = true;
            $scope.Title = "Xml Method :";
        }
        else if (methodType == "Rule") {
            $scope.showRules = true;
            $scope.Title = "Rule :";
        }
        $scope.objNewButtonServerMethod.IsNextDisable();
    }
    /******************************** Second Screen Finish Button Functionality*************************************************************/

    $scope.objNewButtonServerMethod.IsFinishDisable = function () {

        if ($scope.SelectedButtonDetails.Method == "btnGridSearchCriteriaReq_Click") {
            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "";
            if ($scope.objNewButtonServerMethod.StrServerMethod == undefined || $scope.objNewButtonServerMethod.StrServerMethod == "") {
                $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Object Method";
                return true;
            }
        }
        else {
            return true;
        }
        return false;
    };

    $scope.objNewButtonServerMethod.IsNextDisable = function () {

        if ($scope.SelectedButtonDetails.Method != "btnGridSearchCriteriaReq_Click") {
            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "";
            if ($scope.SelectedButtonDetails.Method == "btnExecuteServerMethod_Click" || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethod_Click" || $scope.SelectedButtonDetails.Method == "btnDownload_Click"
                || $scope.SelectedButtonDetails.Method == "btnValidateExecuteBusinessMethod_Click" || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethodSelectRows_Click" || $scope.SelectedButtonDetails.Method == "btnCompleteWorkflowActivities_Click") {
                if ($scope.SelectedButtonDetails.Method == "btnExecuteServerMethod_Click" || $scope.SelectedButtonDetails.Method == "btnDownload_Click") {
                    if ($scope.objNewButtonServerMethod.StrServerMethod == undefined || $scope.objNewButtonServerMethod.StrServerMethod == "") {
                        $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Server Method.";
                        return true;
                    }
                    else if ($scope.objNewButtonServerMethod.StrServerMethod && !$scope.objNewButtonServerMethod.lstXmlMethods.some(function (x) { return x == $scope.objNewButtonServerMethod.StrServerMethod; })) {
                        $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Invalid Server Method.";
                        return true;
                    }
                }
                else {
                    if (!$scope.objNewButtonServerMethod.StrServerMethod) {
                        if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "ObjectMethod") {
                            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Object Method.";
                        }
                        else if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "XmlMethod") {
                            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Xml Method.";
                        }
                        else if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "Rule") {
                            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Rule.";
                        }
                        else {
                            $scope.objNewButtonServerMethod.ErrorMessageForDisplay = "Select Object Method.";
                        }
                        return true;
                    }
                }
            }
            if ($scope.isAnyErrors()) {
                return true;
            }
            return false;
        }

        return true;
    };

    $scope.objNewButtonOpenWordDoc.IsNextDisable = function () {
        $scope.objNewButtonOpenWordDoc.ErrorMessageForDisplay = "";
        if ($scope.objNewButtonOpenWordDoc.TemplateName && $scope.objNewButtonOpenWordDoc.lstCorrTemplates != undefined) {
            var lst = $scope.objNewButtonOpenWordDoc.lstCorrTemplates.filter(function (itm) { return itm == $scope.objNewButtonOpenWordDoc.TemplateName; });
            if (!lst || (lst && lst.length == 0)) {
                $scope.objNewButtonOpenWordDoc.ErrorMessageForDisplay = "Invalid Template Name.";
                return true;
            }
        }
        return false;
    };

    //#endregion

    //#region For Method Open,Close,Finish and New Dialog Wizard pages Methods

    //#region Variables
    $scope.objNewButtonDialogForGrid.lstDialogPanel = [];
    $scope.objNewButtonDialogForGrid.lstRelatedGrid = [];
    $scope.objCreateNewGrid.lstselectedobjecttreefields = [];
    $scope.objGridObjectField.lstFields = [];

    //#endregion

    //#region Methods

    //#region Method For Select Grid
    $scope.InitForGrid = function () {
        $scope.objNewButtonDialogForGrid.SelectGridOption = "CreateNewGrid";
        $scope.objNewButtonDialogForGrid.SelectDialogPanelOption = "CreateNewDialogPanel";
        $scope.objNewButtonDialogForGrid.lstRelatedGrid.push("");
        PopulateRelatedGrid($scope.formodel, $scope.objNewButtonDialogForGrid.lstRelatedGrid, false);
        $scope.PopulateDialogPanels();

        $scope.objGridField.LoadControlTypes();
        $scope.objGridField.LoadDataFormat();
        $scope.objGridField.LoadDataKey();
        $scope.objGridField.LoadSort();
        $scope.objGridField.LoadOrder();
        $scope.CreateGridView();
    };

    $scope.CreateGridView = function () {
        $scope.objGridView = { Name: "sfwGridView", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [] };

        $scope.objGridView.dictAttributes.ID = "dgrResult";
        $scope.objGridView.dictAttributes.AllowPaging = "True";
        $scope.objGridView.dictAttributes.AllowSorting = "True";
        $scope.objGridView.dictAttributes.sfwSelection = "Many";
    };

    $scope.PopulateDialogPanels = function () {
        var lst = [];
        FindControlListByName($scope.formodel, "sfwDialogPanel", lst);
        $scope.objNewButtonDialogForGrid.lstDialogPanel.push("");
        if (lst && lst.length > 0) {
            angular.forEach(lst, function (itm) {
                $scope.objNewButtonDialogForGrid.lstDialogPanel.push(itm.dictAttributes.ID);
            });
        }
    };

    $scope.objNewButtonDialogForGrid.ValidateDialogPanel = function () {
        var retValue = false;
        if ($scope.objNewButtonDialogForGrid.SelectDialogPanel == undefined || $scope.objNewButtonDialogForGrid.SelectDialogPanel == "") {
            $scope.objNewButtonDialogForGrid.ErrorMessageForDisplay = "Error: Dialog Panel ID can not be empty.";
            retValue = true;
        }
        else if ($scope.objNewButtonDialogForGrid.SelectDialogPanelOption == "CreateNewDialogPanel" && ($scope.objNewButtonDialogForGrid.SelectDialogPanel != undefined && $scope.objNewButtonDialogForGrid.SelectDialogPanel != "")) {
            if ($scope.objNewButtonDialogForGrid.lstDialogPanel.some(function (x) { return x == $scope.objNewButtonDialogForGrid.SelectDialogPanel; })) {
                $scope.objNewButtonDialogForGrid.ErrorMessageForDisplay = "Error: Dialog Panel ID already exist.";
                retValue = true;
            }
        }
        return retValue;
    };

    $scope.objNewButtonDialogForGrid.ValidateGridControl = function () {
        var retValue = false;
        if ($scope.objNewButtonDialogForGrid.SelectGridOption == "UseExistingGrid")
            if ($scope.objNewButtonDialogForGrid.SelectGrid == undefined || $scope.objNewButtonDialogForGrid.SelectGrid == "") {
                $scope.objNewButtonDialogForGrid.ErrorMessageForDisplay = "Error: Related Control can not be empty.";
                retValue = true;
            }
        return retValue;
    };

    //#endregion

    //#region Method For Create New Grid
    $scope.objCreateNewGrid.OnNextClick = function () {
        if ($scope.objGridObjectField.lstFields && $scope.objGridObjectField.lstFields.length > 0) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var primaryKey = getPrimarykey(entityIntellisenseList, $scope.objGridObjectField.lstFields[0].Entity);

            if ($scope.objGridView && $scope.objGridView.dictAttributes && $scope.objCreateNewGrid.SelectedEntityField) {
                $scope.objGridView.dictAttributes.sfwEntityField = $scope.objCreateNewGrid.SelectedEntityField.ID;
            }
        }
        if ($scope.objGridView && $scope.objGridView.dictAttributes && $scope.objCreateNewGrid.SelectedEntityField) {
            $scope.objGridView.dictAttributes.sfwEntityField = $scope.objCreateNewGrid.SelectedEntityField.ID;
        }
    };
    //#endregion

    //#region Method For Grid Obejct Field

    $scope.objGridObjectField.OnNextClick = function () {
        var lstList = [];
        $scope.objGridField.FieldCollection = [];
        lstList = $scope.objGridObjectField.lstselectedmultiplelevelfield;//GetSelectedFieldList($scope.objGridObjectField.lstFields, lstList);
        if (lstList.length > 0) {
            var primaryKeyFileds = [];
            //if (!string.IsNullOrEmpty(this.objGridView.BusObjKeyFields))
            //{
            //    primaryKeyFileds = this.objGridView.BusObjKeyFields.Split(',');
            //}
            var intCountPrimary = 0;
            angular.forEach(lstList, function (obj) {
                var objModel = obj.FieldObject;
                var sfxField = {};

                sfxField.istrFieldName = objModel.ID;
                sfxField.istrObjectID = objModel.Value;
                sfxField.istrDataType = objModel.DataType;
                var entityname = obj.Entity;
                sfxField.istrEntityName = entityname;
                sfxField.istrItemPath = obj.EntityField;//GetItemPathForEntityObject(objModel);
                sfxField.istrControlType = "Label";

                if (objModel) {
                    //var strHeaderText = sfxField.istrFieldName.substring(sfxField.istrFieldName.lastIndexOf('.') + 1);
                    var strHeaderText = GetCaptionFromField(objModel);
                    sfxField.istrHeader = strHeaderText;
                }

                sfxField.istrPropertyName = obj.EntityField;//GetItemPathForEntityObject(objModel);
                sfxField.istrVisible = "True";

                //if (null != primaryKeyFileds)
                //{
                //    if (null != primaryKeyFileds.FirstOrDefault(key => key == sfxField.istrItemPath))
                //    {
                //        intCountPrimary += 1;
                //        sfxField.istrKey = intCountPrimary.ToString();
                //    }
                //}

                $scope.objGridField.FieldCollection.push(sfxField);
            });
        }

        if ($scope.objCreateNewGrid && $scope.objCreateNewGrid.SelectedEntityField) {
            var strControlID = $scope.objCreateNewGrid.SelectedEntityField.ID;
            strControlID = CreateControlID($scope.formodel, $scope.objCreateNewGrid.SelectedEntityField.ID, "sfwGridView");
            $scope.objGridView.dictAttributes.ID = strControlID;
        }
    };

    //#endregion

    //#region Method For Grid Fields

    //#region Variables
    $scope.objGridField.ArrControlTypes = [];
    $scope.objGridField.ArrDataFormat = [];
    $scope.objGridField.ArrDataKey = [];
    $scope.objGridField.ArrSort = [];
    $scope.objGridField.ArrOrder = [];
    $scope.objGridField.ArrRelativeControl = [];
    $scope.objGridField.FieldCollection = [];
    //#endregion

    $scope.objGridField.ChangeSortExpression = function (selectedcolumn) {
        if (selectedcolumn && !selectedcolumn.istrSort) {
            selectedcolumn.istrOrder = "";
        }
    };

    $scope.objGridField.LoadControlTypes = function () {
        $scope.objGridField.ArrControlTypes = [];
        $scope.objGridField.ArrControlTypes.push("");
        $scope.objGridField.ArrControlTypes.push("Label");
        $scope.objGridField.ArrControlTypes.push("HyperLink");
        $scope.objGridField.ArrControlTypes.push("TextBox");
        $scope.objGridField.ArrControlTypes.push("Checkbox");
        $scope.objGridField.ArrControlTypes.push("DropDownList");
    };

    $scope.objGridField.LoadDataFormat = function () {
        $scope.objGridField.ArrDataFormat = [];
        $scope.objGridField.ArrDataFormat.push("");
        $scope.objGridField.ArrDataFormat.push("{0:d}");              // Date
        $scope.objGridField.ArrDataFormat.push("{0:C}");              // Currency
        $scope.objGridField.ArrDataFormat.push("{0:000-##-####}");    // SSN
        $scope.objGridField.ArrDataFormat.push("{0:(###)###-####}");  // Phone/Fax
    };

    $scope.objGridField.LoadDataKey = function () {
        $scope.objGridField.ArrDataKey = [];
        $scope.objGridField.ArrDataKey.push("");
        $scope.objGridField.ArrDataKey.push("1");
        $scope.objGridField.ArrDataKey.push("2");
        $scope.objGridField.ArrDataKey.push("3");
        $scope.objGridField.ArrDataKey.push("4");
        $scope.objGridField.ArrDataKey.push("5");
    };

    $scope.objGridField.LoadSort = function () {
        $scope.objGridField.ArrSort = [];
        $scope.objGridField.ArrSort.push("");
        $scope.objGridField.ArrSort.push("1");
        $scope.objGridField.ArrSort.push("2");
        $scope.objGridField.ArrSort.push("3");
    };

    $scope.objGridField.LoadOrder = function () {
        $scope.objGridField.ArrOrder = [];
        $scope.objGridField.ArrOrder.push("");
        $scope.objGridField.ArrOrder.push("asc");
        $scope.objGridField.ArrOrder.push("desc");
    };

    $scope.objGridField.LoadRelativeControl = function () {
        $scope.objGridField.ArrRelativeControl = [];
        $scope.objGridField.ArrRelativeControl.push("");

        angular.forEach(ialGridButtons, function (theButton) {
            var strButtonID = theButton.dictAttributes.ID;
            if (strButtonID) {
                $scope.objGridField.ArrRelativeControl.push(strButtonID);
            }
        });
    };

    $scope.objGridField.onFieldClick = function (obj) {
        $scope.objGridField.SelectGridField = obj;
    };

    $scope.objGridField.MoveUpField = function () {
        if ($scope.objGridField.SelectGridField) {
            var index = $scope.objGridField.FieldCollection.indexOf($scope.objGridField.SelectGridField);
            var item = $scope.objGridField.FieldCollection[index - 1];
            $scope.objGridField.FieldCollection[index - 1] = $scope.objGridField.SelectGridField;
            $scope.objGridField.FieldCollection[index] = item;
        }
    };

    $scope.objGridField.MoveDownField = function () {
        if ($scope.objGridField.SelectGridField) {
            var index = $scope.objGridField.FieldCollection.indexOf($scope.objGridField.SelectGridField);
            var item = $scope.objGridField.FieldCollection[index + 1];
            $scope.objGridField.FieldCollection[index + 1] = $scope.objGridField.SelectGridField;
            $scope.objGridField.FieldCollection[index] = item;
        }
    };

    $scope.objGridField.CanMoveUp = function () {
        var retVal = false;
        if ($scope.objGridField.SelectGridField) {
            var index = $scope.objGridField.FieldCollection.indexOf($scope.objGridField.SelectGridField);
            if (index == 0) {
                retVal = true;
            }
        }
        else {
            retVal = true;
        }
        return retVal;
    };
    $scope.objGridField.CanMoveDown = function () {
        var retVal = false;
        if ($scope.objGridField.SelectGridField) {
            var index = $scope.objGridField.FieldCollection.indexOf($scope.objGridField.SelectGridField);
            if (index == $scope.objGridField.FieldCollection.length - 1) {
                retVal = true;
            }
        }
        else {
            retVal = true;
        }
        return retVal;
    };

    //#endregion

    //#endregion

    //#endregion

    //#region Method For New Method Wizard Pages

    $scope.InitForNewButtonMethod = function () {
        var objGrid = FindParent($scope.sfxCell, "sfwGridView");
        if (objGrid) {
            $scope.objNewButtonActiveForm.objRelatedGrid = objGrid;
            $scope.alControls = [];
            $scope.PopulateControlsForActiveForm($scope.alControls, iblnIsLookup);
        }
        $scope.objNewButtonActiveForm.ActiveFormCollection = [];
        $scope.objNewButtonActiveForm.lstRelatedControl = [];
        var iblnIsLookup = $scope.formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";
        var strParent = "";
        if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.sfxControl.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
            if (iblnIsLookup)
                strParent = "tblCriteria panel fields";
            else
                strParent = "Form fields";
        }
        else if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
            PopulateRelatedGrid($scope.formodel, $scope.objNewButtonActiveForm.lstRelatedControl, false);
            var strGridID = $scope.sfxControl.dictAttributes.sfwRelatedControl;
            if (strGridID == "")
                strParent = "Grid fields";
            else
                strParent = strGridID + " fields";
        }
        $scope.alControls = [];
        $scope.PopulateControlsForActiveForm($scope.alControls, iblnIsLookup);
        $scope.objNewButtonActiveForm.StrActiveForm = $scope.sfxControl.dictAttributes.sfwActiveForm;

        $scope.objNewButtonActiveForm.StrEntityField = $scope.sfxControl.dictAttributes.sfwEntityField;
        $scope.InitializeActiveForms($scope.objNewButtonActiveForm.StrActiveForm, $scope.sfxControl.dictAttributes.sfwEntityField, $scope.alControls, strParent);

        $scope.objNewButtonActiveForm.ActiveFormType = 'SingleForm';
        function iterator(itm) {
            if (itm) {
                $scope.objNewButtonActiveForm.lstRelatedControl.push(itm);
            }
        }
        if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
            if ($scope.objNewButtonActiveForm.lstFields && $scope.objNewButtonActiveForm.lstFields.length > 0) {
                angular.forEach($scope.objNewButtonActiveForm.lstFields, iterator);
            }
        }
    };

    //#region New Button Active Form  Method

    $scope.objNewButtonActiveForm.onRelatedControlChanged = function () {
        $scope.objNewButtonActiveForm.ActiveFormCollection = [];
        var iblnIsLookup = $scope.formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";
        if ($scope.sfxControl.dictAttributes.sfwRelatedControl != undefined && $scope.sfxControl.dictAttributes.sfwRelatedControl != "") {
            if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
                var strGridID = $scope.sfxControl.dictAttributes.sfwRelatedControl;
                if (strGridID == "")
                    strParent = "Grid fields";
                else
                    strParent = strGridID + " fields";

                $scope.alControls = [];

                $scope.PopulateControlsForActiveForm($scope.alControls, iblnIsLookup);
                $scope.objNewButtonActiveForm.StrActiveForm = $scope.sfxControl.dictAttributes.sfwActiveForm;

                $scope.objNewButtonActiveForm.StrEntityField = $scope.sfxControl.dictAttributes.sfwEntityField;
                $scope.InitializeActiveForms($scope.objNewButtonActiveForm.StrActiveForm, $scope.sfxControl.dictAttributes.sfwEntityField, $scope.alControls, strParent);
            }
            else if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
                $scope.objNewButtonActiveForm.alCodeVal = [];
                if ($scope.sfxControl.dictAttributes.sfwRelatedControl) {
                    var objControl = FindControlByID($scope.formodel, $scope.sfxControl.dictAttributes.sfwRelatedControl);
                    if (objControl && objControl.dictAttributes && objControl.dictAttributes.sfwLoadType == "CodeGroup") {
                        $scope.IsValue = true;
                        if (objControl.dictAttributes.sfwLoadSource) {
                            $.connection.hubMain.server.getCodeValues("ScopeId_" + $scope.$id, objControl.dictAttributes.sfwLoadSource);
                        }
                        else if (objControl.placeHolder) {
                            $.connection.hubMain.server.getCodeValues("ScopeId_" + $scope.$id, objControl.placeHolder);
                        }
                    }
                    else {
                        $scope.IsValue = false;
                    }
                }
            }
        }
    };

    $scope.PopulateControlsForActiveForm = function (alControls, iblnIsLookup) {
        if ($scope.sfxControl) {
            var altmpControls = [];
            if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.sfxControl.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
                if (iblnIsLookup)
                    GetAllControls($scope.formodel, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton", "tblcriteria", "", false, altmpControls, false);
                else
                    GetAllControls($scope.formodel, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton,sfwScheduler", "", "", false, altmpControls, true);
            }
            else if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
                var strGridID = $scope.sfxControl.dictAttributes.sfwRelatedControl;
                if ($scope.objNewButtonActiveForm.objRelatedGrid && $scope.objNewButtonActiveForm.objRelatedGrid.dictAttributes) {
                    strGridID = $scope.objNewButtonActiveForm.objRelatedGrid.dictAttributes.ID;
                }
                if (strGridID != "")
                    GetAllControls($scope.formodel, "", "", strGridID, false, altmpControls);
            }

            angular.forEach(altmpControls, function (objCtrl) {
                var strCodeGroup = "";
                if ("sfwLoadSource" in objCtrl.dictAttributes) {
                    strCodeGroup = objCtrl.dictAttributes.sfwLoadSource;
                }
                if (strCodeGroup == "" || strCodeGroup == "0") {
                    strCodeGroup = "~";
                    if (objCtrl.Elements.length > 0) {
                        // System.Web.UI.WebControls.ListItemCollection listItems = ((System.Web.UI.WebControls.ListControl)objCtrl.webctrl).Items;
                        angular.forEach(objCtrl.Elements, function (item) {
                            if (item.Name == "ListItem") {
                                if (item.dictAttributes.Value) {
                                    strCodeGroup += item.dictAttributes.Value + ",";
                                }
                            }
                        });
                    }
                }
                if (iblnIsLookup) {
                    if ("sfwDataField" in objCtrl.dictAttributes && objCtrl.dictAttributes.sfwDataField) {
                        if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
                            alControls.push(objCtrl.dictAttributes.ID + "~" + strCodeGroup);
                        }
                        else {
                            alControls.push(objCtrl.dictAttributes.sfwDataField + "~" + strCodeGroup);
                        }
                    }
                }
                else {
                    if ("sfwEntityField" in objCtrl.dictAttributes && objCtrl.dictAttributes.sfwEntityField) {
                        alControls.push(objCtrl.dictAttributes.sfwEntityField);
                    }
                }
            });
        }
    };

    $scope.InitializeActiveForms = function (astrActiveForms, astrObjectField, alAvlFlds, astrParentID) {
        $scope.objNewButtonActiveForm.lstFields = [];
        ilAvlFlds = alAvlFlds;

        if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.sfxControl.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
            GetObjectFields($scope.formodel, $scope.objNewButtonActiveForm.lstFields, $scope.sfxControl);
        }
        else if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnDelete_Click") {
        }
        else if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
            var strMessage = "(Object Fields are populated from the " + astrParentID + ".)";

            $scope.objNewButtonActiveForm.lstFields.push("");

            if (alAvlFlds.length > 0) {
                for (var i = 0; i < alAvlFlds.length; i++) {
                    var s = alAvlFlds[i];
                    var strParamValue = "";
                    if (s.indexOf("~") > -1)
                        strParamValue = s.substring(0, s.indexOf("~"));
                    else
                        strParamValue = s;
                    $scope.objNewButtonActiveForm.lstFields.push(strParamValue);
                }
            }
        }
    };

    $scope.objNewButtonActiveForm.OnAddExpressionClick = function () {
        var objActiveForm = { FieldValue: "", ActiveForm: "" };

        $scope.objNewButtonActiveForm.ActiveFormCollection.push(objActiveForm);
        $scope.objNewButtonActiveForm.SelectedExpression = objActiveForm;
    };

    $scope.objNewButtonActiveForm.OnDeleteExpressionClick = function () {
        if ($scope.objNewButtonActiveForm.SelectedExpression != null) {
            var index = $scope.objNewButtonActiveForm.lstFields.indexOf($scope.objNewButtonActiveForm.SelectedExpression);
            $scope.objNewButtonActiveForm.ActiveFormCollection.splice(index, 1);
            if ($scope.objNewButtonActiveForm.ActiveFormCollection.length > 0) {
                $scope.objNewButtonActiveForm.SelectedExpression = $scope.objNewButtonActiveForm.ActiveFormCollection[$scope.objNewButtonActiveForm.ActiveFormCollection.length - 1];
            }
            else {
                $scope.objNewButtonActiveForm.SelectedExpression = $scope.objNewButtonActiveForm.ActiveFormCollection[0];
            }
        }
    };

    $scope.objNewButtonActiveForm.onEntityFieldChange = function () {
        $scope.IsValue = false;
        $scope.objNewButtonActiveForm.alCodeVal = [];
        var fieldName;
        if ($scope.objNewButtonActiveForm.StrEntityField) {
            fieldName = $scope.objNewButtonActiveForm.StrEntityField;
            var strentityName = $scope.formodel.dictAttributes.sfwEntity;
            if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
                objControl = FindControlByID($scope.formodel, $scope.sfxControl.dictAttributes.sfwRelatedControl);
                if ($scope.objNewButtonActiveForm.objRelatedGrid && $scope.objNewButtonActiveForm.objRelatedGrid.dictAttributes) {
                    objControl = $scope.objNewButtonActiveForm.objRelatedGrid;
                }
                if (objControl && objControl.dictAttributes && objControl.dictAttributes.sfwEntityField) {
                    var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strentityName, objControl.dictAttributes.sfwEntityField);
                    if (objAttribute) {
                        strentityName = objAttribute.Entity;
                    }
                }
            }

            var lstEntity = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (itm) {
                return itm.ID == strentityName;
            });
            if (lstEntity && lstEntity.length > 0) {
                var attribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strentityName, fieldName);
                if (attribute) {
                    var value = attribute.Value;
                    if (value.match("_value$")) {
                        var val = value.replace("_value", "_id");
                        var valAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromFieldValue(strentityName, val);
                        if (valAttribute) {
                            $scope.IsValue = true;
                            var strCodeID = valAttribute.CodeID;
                            $.connection.hubMain.server.getCodeValues("ScopeId_" + $scope.$id, strCodeID);
                        }
                    }
                }
            }
        }
    };

    $scope.receiveList = function (data) {
        $scope.$evalAsync(function () {
            var lstCodeVal = data;
            if (lstCodeVal && lstCodeVal.length > 0) {
                $scope.objNewButtonActiveForm.alCodeVal.push("");
                angular.forEach(lstCodeVal, function (itm) {
                    $scope.objNewButtonActiveForm.alCodeVal.push(itm.CodeValue);
                });
            }
        });
    };

    $scope.objNewButtonActiveForm.objActiveFormTypeChange = function () {
        $scope.objNewButtonActiveForm.StrActiveForm = "";
        $scope.objNewButtonActiveForm.StrEntityField = "";
        $scope.objNewButtonActiveForm.ActiveFormCollection = [];
    };

    //#endregion

    //#region New Button Navigation Parameter Method

    $scope.objNewButtonNaviagtionParam.showAllControlChange = function () {
        $scope.$evalAsync(function () {
            $scope.objNewButtonNaviagtionParam.PopulateAvailableFields();
        });
    };

    $scope.objNewButtonNaviagtionParam.Init = function () {
        if ($scope.IsOpenWordDocStepsDisable) {
            $scope.objNewButtonNaviagtionParam.ParameterCollection = [];
            $scope.objNewButtonNaviagtionParam.FieldCollection = [];
            $scope.objNewButtonNaviagtionParam.PopulateAvailableFields();

            var strMethodName = $scope.sfxControl.dictAttributes.sfwMethodName;
            var strParamValue = "";
            if (strMethodName == "btnExecuteServerMethod_Click" || strMethodName == "btnDownload_Click")
                strParamValue = $scope.sfxControl.dictAttributes.sfwObjectMethod;
            else if (strMethodName == "btnExecuteBusinessMethod_Click" || strMethodName == "btnCompleteWorkflowActivities_Click" ||
                strMethodName == "btnValidateExecuteBusinessMethod_Click") {
                if ($scope.sfxControl.dictAttributes.sfwEntityMethod) {
                    strParamValue = $scope.sfxControl.dictAttributes.sfwEntityMethod + '.' + $scope.sfxControl.dictAttributes.sfwObjectMethod;
                }
                else {
                    strParamValue = $scope.sfxControl.dictAttributes.sfwObjectMethod;
                }
            }
            else if (strMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
                var strGridID = $scope.sfxControl.dictAttributes.sfwRelatedControl;
                var strParam = "";
                if (strGridID != undefined && strGridID != "") {
                    var lstTable = $scope.formodel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
                    if (lstTable && lstTable.length > 0) {
                        strParamValue = strGridID + ":" + $scope.PopulateDataKeyForGrid(strGridID, lstTable[0], strParam);
                    }
                }
                else {
                    strParamValue = "";
                }
            }
            else if (strMethodName == "btnWizardFindAndNext_Click") {
                strParamValue = $scope.sfxControl.dictAttributes.sfwObjectMethod;
            }
            else if (strMethodName == "btnSaveNew_Click") {
                strParamValue = $scope.formodel.dictAttributes.ID;
            }
            else {
                if ($scope.sfxControl.dictAttributes.sfwActiveForm != undefined && $scope.sfxControl.dictAttributes.sfwActiveForm != "") {
                    strParamValue = $scope.sfxControl.dictAttributes.sfwActiveForm;
                }
                else
                    strParamValue = $scope.sfxControl.dictAttributes.sfwXmlDocument;
            }

            $scope.objNewButtonNaviagtionParam.PopulateParameters(strMethodName, strParamValue);
        }
    };

    $scope.objNewButtonNaviagtionParam.PopulateAvailableFields = function (strPropID) {
        $scope.objMainTable = undefined;
        var CurrentTable = undefined;
        if ($scope.formodel && $scope.sfxControl && $scope.sfxControl.dictAttributes && $scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
            var larrPanels = getDescendents($scope.formodel, "sfwPanel");
            if (larrPanels && larrPanels.length > 0) {
                var objPanel = larrPanels.filter(function (pnl) { return pnl.dictAttributes.ID === "pnlMain" || pnl.dictAttributes.ID === "pnlCriteria" })[0];
                if (objPanel && objPanel.Elements && objPanel.Elements.length > 0) {
                    CurrentTable = objPanel.Elements[0];
                }
            }
        }

        if (!CurrentTable) {
            CurrentTable = FindParent($scope.sfxCell, "sfwTable");
        }

        var lst = $scope.formodel.Elements.filter(function (x) { return x.Name == "sfwTable"; });
        if (lst && lst.length > 0) {
            $scope.objMainTable = lst[0];
        }

        $scope.objNewButtonNaviagtionParam.FieldCollection = [];
        if ($scope.formodel) {
            var istrValue = $scope.formodel.dictAttributes.sfwType;
            var blnIsLookup = istrValue.toUpperCase().trim() == "LOOKUP";

            var strProperty;
            var isNewButton = false;
            if ($scope.sfxControl && $scope.sfxControl.dictAttributes && $scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
                strProperty = "ID";
                isNewButton = true;
            }
            else if (blnIsLookup)
                strProperty = "sfwDataField";
            else
                strProperty = "sfwEntityField";

            var objControl = { Text: "Main", Items: [], IsSelected: false, IsCheckBoxVisible: false };
            objControl.ControlName = "Main";

            if ($scope.objNewButtonNaviagtionParam.IsShowAllControl) {
                PopulateAvailableFields(strProperty, $scope.objMainTable, objControl, false, blnIsLookup, isNewButton);
            }
            else {
                if (CurrentTable) {
                    PopulateAvailableFields(strProperty, CurrentTable, objControl, false, blnIsLookup, isNewButton);
                }
                else {
                    PopulateAvailableFields(strProperty, $scope.objMainTable, objControl, false, blnIsLookup, isNewButton);
                }
            }
            if (objControl.Items.length > 0)
                $scope.objNewButtonNaviagtionParam.FieldCollection.push(objControl);
            // tvFields.DataContext = list;
        }
    };

    $scope.objNewButtonNaviagtionParam.AddParamGridRow = function (astrParamType, astrParamField, astrParamValue, astrControlID) {
        var objParameter = {};
        objParameter.Type = astrParamType;
        objParameter.ParameterField = astrParamField;
        if (astrParamValue) {
            if (astrParamValue.match("^#")) {
                objParameter.Constants = true;
                objParameter.ParameterValue = astrParamValue.substring(1);
            }
            else {
                objParameter.ParameterValue = astrParamValue;
            }
            objParameter.EntityField = astrParamValue;
            objParameter.IsReadOnly = true;
        }
        objParameter.ControlID = astrControlID;
        $scope.objNewButtonNaviagtionParam.ParameterCollection.push(objParameter);
    };

    $scope.objNewButtonNaviagtionParam.PopulateParameters = function (astrMethodName, astrParamValue) {
        $scope.objNewButtonNaviagtionParam.IsControlIDVisible = false;
        $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Target Form:";
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        if (astrMethodName == "btnSave_Click" ||
            astrMethodName == "btnNoChangesSave_Click" ||
            astrMethodName == "btnForceSave_Click" ||
            astrMethodName == "btnWizardSaveAndNext_Click" ||
            astrMethodName == "btnWizardSaveAndPrevious_Click" ||
            astrMethodName == "btnSaveAndNext_Click" ||
            astrMethodName == "btnSaveIgnoreReadOnly_Click")  // Load parameters from the source form.
        {
            if ($scope.formodel) {
                var InitialLoadVM;
                $scope.objNewButtonNaviagtionParam.TargetForm = $scope.formodel.dictAttributes.ID;
                var lst = $scope.formodel.Elements.filter(function (x) { return x.Name == "initialload"; });
                if (lst && lst.length > 0) {
                    InitialLoadVM = lst[0];
                }

                if (InitialLoadVM) {
                    var strMethod = "";
                    var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || x.dictAttributes.sfwMode == "Update"); });
                    if (lst && lst.length) {
                        strMethod = lst[0].dictAttributes.sfwMethodName;
                    }


                    if (strMethod != undefined && strMethod != "") {
                        if ($scope.formodel.dictAttributes.sfwRemoteObject) {
                            var objServerObject = GetServerMethodObject($scope.formodel.dictAttributes.sfwRemoteObject, $scope.formodel.RemoteObjectCollection);
                            var paramerters = GetSrvMethodParameters(objServerObject, strMethod);
                            if (paramerters) {
                                for (j = 0; j < paramerters.length; j++) {
                                    $scope.objNewButtonNaviagtionParam.AddParamGridRow(paramerters[j].dictAttributes.sfwDataType, paramerters[j].dictAttributes.ID, "");
                                }
                            }
                        }
                        else {
                            var EntityObject = entityIntellisenseList.filter(function (x) { return x.ID == $scope.formodel.dictAttributes.sfwEntity; });
                            if (EntityObject && EntityObject[0].XmlMethods != undefined && EntityObject[0].XmlMethods.length > 0) {
                                for (i = 0; i < EntityObject[0].XmlMethods.length; i++) {
                                    if (EntityObject[0].XmlMethods[i].ID == strMethod) {
                                        for (j = 0; j < EntityObject[0].XmlMethods[i].Parameters.length; j++) {
                                            $scope.objNewButtonNaviagtionParam.AddParamGridRow(EntityObject[0].XmlMethods[i].Parameters[j].DataType, EntityObject[0].XmlMethods[i].Parameters[j].ID, EntityObject[0].XmlMethods[i].Parameters[j].Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (astrMethodName == "btnExecuteServerMethod_Click" ||
            astrMethodName == "btnWizardFindAndNext_Click" || astrMethodName == "btnDownload_Click")  // Load server method parameters.
        {
            $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Server Method:";

            if (astrParamValue == undefined || astrParamValue == "") { return; }

            var strObjectMethod = astrParamValue.trim();
            $scope.objNewButtonNaviagtionParam.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

            var RemoteObjectName = "srvCommon";
            if ($scope.formodel && $scope.formodel.dictAttributes.sfwRemoteObject) {
                RemoteObjectName = $scope.formodel.dictAttributes.sfwRemoteObject;
            }

            var objServerObject = GetServerMethodObject(RemoteObjectName, $scope.formodel.RemoteObjectCollection);
            var paramerters = GetSrvMethodParameters(objServerObject, strObjectMethod);
            angular.forEach(paramerters, function (objParam) {
                $scope.objNewButtonNaviagtionParam.AddParamGridRow(objParam.dictAttributes.sfwDataType, objParam.dictAttributes.ID, "");
            });
        }
        else if (astrMethodName == "btnExecuteBusinessMethod_Click" ||
            astrMethodName == "btnValidateExecuteBusinessMethod_Click" || astrMethodName == "btnCompleteWorkflowActivities_Click")
        // Load business object method parameters.
        {
            var lstData = [];
            if ($scope.objNewButtonServerMethod && $scope.objNewButtonServerMethod.sfwExecuteMethodType) {
                if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "ObjectMethod") {
                    $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Object Method:";

                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, true, false, false, false);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formodel.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                    }
                }
                else if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "XmlMethod") {
                    $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Xml Method:";
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, false, false, false, true);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formodel.dictAttributes.sfwEntity, "", "", true, false, false, false, false, true);
                    }
                }
                else if ($scope.objNewButtonServerMethod.sfwExecuteMethodType == "Rule") {
                    $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Rule:";
                    if ($scope.entityName) {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, false, true, false, false);
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formodel.dictAttributes.sfwEntity, "", "", true, false, false, true, false, false);
                    }
                }
            }
            else {
                $scope.objNewButtonNaviagtionParam.TargetFormCaption = "Object Method:";
                if ($scope.entityName) {
                    lstData = $Entityintellisenseservice.GetIntellisenseData($scope.entityName, "", "", true, false, true, false, false, false);
                }
                else {
                    lstData = $Entityintellisenseservice.GetIntellisenseData($scope.formodel.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                }
            }

            var strObjectMethod = astrParamValue.trim();
            $scope.objNewButtonNaviagtionParam.TargetForm = strObjectMethod;

            if (strObjectMethod == "" || strObjectMethod == undefined)
                return;

            var strObjectID = "";
            if (strObjectMethod.contains(".")) {
                strObjectID = strObjectMethod.substring(0, strObjectMethod.indexOf('.'));
                strObjectMethod = strObjectMethod.substring(strObjectMethod.indexOf('.') + 1);
            }

            var lsttempData = [];
            var objMethod;
            if (lstData) {
                angular.forEach(lstData, function (item) {
                    if (!objMethod) {
                        if (item.ID == strObjectMethod) {
                            objMethod = item;
                        }
                    }
                });
            }

            if (objMethod) {
                var paramerters = objMethod.Parameters;
                //var paramerters = GetObjectMethodParameters(entityIntellisenseList, $scope.formobject.dictAttributes.sfwEntity, strObjectMethod);
                if (paramerters) {
                    angular.forEach(paramerters, function (objParam) {

                        if (objMethod.RuleType && ["LogicalRule", "DecisionTable", "ExcelMatrix"].indexOf(objMethod.RuleType) > -1) {
                            if (objParam.Direction == "In") {
                                if (astrMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
                                    if (["Object", "Collection", "List"].indexOf(objParam.DataType) > -1) {

                                        $scope.objNewButtonNaviagtionParam.AddParamGridRow(objParam.DataType, objParam.ID, null);
                                    }
                                }
                                else {

                                    $scope.objNewButtonNaviagtionParam.AddParamGridRow(objParam.DataType, objParam.ID, null);

                                }
                            }

                        }
                        else {
                            $scope.objNewButtonNaviagtionParam.AddParamGridRow(objParam.DataType, objParam.ID, null);
                        }

                    });
                }
            }

            // var paramerters = GetObjectMethodParameters(entityIntellisenseList, $scope.formodel.dictAttributes.sfwEntity, strObjectMethod);
        }
        //else if (astrMethodName == "btnExecuteBusinessMethodSelectRows_Click" || astrMethodName == "btnCompleteWorkflowActivities_Click")
        //{
        //    this.TargetFormCaption = "Target Grid:";
        //    if (astrParamValue != string.Empty && astrParamValue.IndexOf(":") > 0)
        //    {
        //        this.TargetForm = astrParamValue.Substring(0, astrParamValue.IndexOf(":"));
        //        string istrParamValue = astrParamValue.Substring(astrParamValue.IndexOf(":") + 1).Trim();
        //        string[] strDataKeyNames = istrParamValue.Split(',');
        //        foreach (string strDataKey in strDataKeyNames)
        //        {
        //            AddParamGridRow("Data Key", strDataKey);
        //    }
        //}
        //else
        //{
        //  $scope.objNewButtonNaviagtionParam.TargetForm = "Please provide the related control.";
        //}
        else if (astrMethodName == "btnSaveNew_Click") {
            $scope.objNewButtonNaviagtionParam.TargetForm = $scope.formodel.dictAttributes.ID;
            $scope.receivenewformmodel($scope.formodel);
        }
        else // Load parameters from the target form. If Active Form is a list of forms, take the first one.
        {
            if (astrParamValue == undefined || astrParamValue == "") { return; }

            var strActiveForm = astrParamValue.trim();

            var alForms = strActiveForm.split(';');
            if (alForms.length > 0) {
                strActiveForm = alForms[0];
                var target = "";
                for (var i = 0; i < alForms.length; i++) {
                    var form = alForms[i];
                    if (form.contains("="))
                        form = form.substring(form.indexOf('=') + 1);
                    if (target == "") {
                        target += form;
                    } else {
                        target += ";" + form;
                    }
                }
            }

            if (strActiveForm.contains("="))
                strActiveForm = strActiveForm.substring(strActiveForm.indexOf('=') + 1);

            $scope.objNewButtonNaviagtionParam.TargetForm = target;


            $.connection.hubForm.server.getNewFormModel(strActiveForm).done(function (data) {
                $scope.receivenewformmodel(data);
            });
            $rootScope.IsLoading = true;
        }
    };

    $scope.receivenewformmodel = function (data) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            var objSfxForm = data;
            var astrMethodName = $scope.sfxControl.dictAttributes.sfwMethodName;
            if (objSfxForm) {
                var istrValue = objSfxForm.dictAttributes.sfwType;
                var blnIsLookup = istrValue.toUpperCase().trim() == "LOOKUP";

                if (blnIsLookup) {
                    $scope.objNewButtonNaviagtionParam.IsSaveWithControlIDVisible = true;

                    $scope.objNewButtonNaviagtionParam.IsControlIDVisible = true;
                    var lstTable = objSfxForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
                    if (lstTable && lstTable.length > 0) {
                        var sfxCriteriaPanel = GetCriteriaPanel(lstTable[0]);
                        if (sfxCriteriaPanel) {
                            angular.forEach(sfxCriteriaPanel.Elements, function (sfxRow) {
                                angular.forEach(sfxRow.Elements, function (sfxCell) {
                                    if (sfxCell) {
                                        angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                                            if ("sfwDataField" in sfxCtrl.dictAttributes) {
                                                var strFieldName = sfxCtrl.dictAttributes.sfwDataField;
                                                var strControlID = sfxCtrl.dictAttributes.ID;
                                                if (strFieldName != "" && strFieldName != undefined)
                                                    $scope.objNewButtonNaviagtionParam.AddParamGridRow("Criteria Field", strFieldName, "", strControlID);
                                            }
                                        });
                                    }
                                });
                            });
                        }
                    }
                }
                else {
                    var blnNewButton = astrMethodName == "btnNew_Click"
                        || astrMethodName == "btnUpdate_Click" || astrMethodName == "btnSaveNew_Click";
                    var InitialLoadVM;
                    var lst = objSfxForm.Elements.filter(function (x) { return x.Name == "initialload"; });
                    if (lst && lst.length > 0) {
                        InitialLoadVM = lst[0];
                    }

                    if (InitialLoadVM) {
                        var strMethod = "";
                        var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || x.dictAttributes.sfwMode == "Update"); });
                        if (lst && lst.length) {
                            strMethod = lst[0].dictAttributes.sfwMethodName;
                        }

                        if (blnNewButton) {
                            var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || x.dictAttributes.sfwMode == "New"); });
                            if (lst && lst.length) {
                                strMethod = lst[0].dictAttributes.sfwMethodName;
                            }
                        }

                        if (strMethod != undefined && strMethod != "") {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            if (objSfxForm.dictAttributes.sfwRemoteObject != undefined && objSfxForm.dictAttributes.sfwRemoteObject != "") {
                                var objServerObject = GetServerMethodObject(objSfxForm.dictAttributes.sfwRemoteObject, $scope.formodel.RemoteObjectCollection);
                                var paramerters = GetSrvMethodParameters(objServerObject, strMethod);
                                if (paramerters) {
                                    for (j = 0; j < paramerters.length; j++) {
                                        $scope.objNewButtonNaviagtionParam.AddParamGridRow(paramerters[j].dictAttributes.sfwDataType, paramerters[j].dictAttributes.ID, "");
                                        //var objParameter = { ParameterField: paramerters[j].dictAttributes.ID, ParameterValue: "", Constants: false };
                                        //vrParCollection.push(objParameter);
                                    }
                                }
                            }
                            else {
                                var paramerters = GetEntityXMLMethodParameters(entityIntellisenseList, objSfxForm.dictAttributes.sfwEntity, strMethod);
                                if (paramerters) {
                                    angular.forEach(paramerters, function (objParam) {
                                        $scope.objNewButtonNaviagtionParam.AddParamGridRow(objParam.DataType, objParam.ID, objParam.Value);
                                    });
                                }
                            }
                        }

                        if (blnNewButton) {
                            var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "session"; });
                            if (lst && lst.length > 0) {
                                angular.forEach(lst[0].Elements, function (objSessionField) {
                                    $scope.objNewButtonNaviagtionParam.AddParamGridRow("Session Field", objSessionField.dictAttributes.ID);
                                });
                            }
                        }
                    }
                }
            }
        });
    };

    $scope.ExpandCollapsedCustomAttrField = function (field, event) {
        field.IsExpanded = !field.IsExpanded;
    };

    $scope.SetFieldClass = function (obj) {
        if (obj == $scope.SelectedField) {
            return "selected";
        }
    };

    $scope.SelectFieldClick = function (obj, event) {
        //if (event.ctrlKey) {
        //    if (obj.IsSelected) {
        //        obj.IsSelected = false;
        //    } else {
        //        obj.IsSelected = true;
        //    }
        //} else {
        //    if (obj.IsSelected) {
        //        // do nothing
        //    } else {
        //        if ($scope.objNewButtonCustomAttr.FieldCollection && $scope.objNewButtonCustomAttr.FieldCollection.length > 0 && $scope.objNewButtonCustomAttr.FieldCollection[0]) {
        //            angular.forEach($scope.objNewButtonCustomAttr.FieldCollection[0].Items, function (itemObj, key) {
        //                if (itemObj.Items.length > 0) {
        //                    TraverseFieldCollection(itemObj);
        //                    itemObj.IsSelected = false;
        //                } else {
        //                    itemObj.IsSelected = false;
        //                }
        //            });
        //            obj.IsSelected = true;
        //        }
        //    }
        //}
        $scope.SelectedField = obj;
        if (event) {
            event.stopPropagation();
        }
    };

    $scope.objNewButtonNaviagtionParam.onMessageIdChange = function () {
        $scope.objNewButtonNaviagtionParam.MessageDescription = "";
        if ($scope.sfxControl.dictAttributes.sfwMessageId != undefined && $scope.sfxControl.dictAttributes.sfwMessageId != "") {
            hubMain.server.populateMessageList().done(function (lstMessages) {
                $scope.$evalAsync(function () {
                    if (lstMessages && lstMessages.length > 0) {
                        var lst = lstMessages.filter(function (x) { return x.MessageID == $scope.sfxControl.dictAttributes.sfwMessageId; });
                        if (lst && lst.length > 0) {
                            $scope.objNewButtonNaviagtionParam.MessageDescription = lst[0].DisplayMessage;
                        }
                    }
                });
            });

        }
    };

    $scope.objNewButtonNaviagtionParam.onMessageIdClick = function () {
        $scope.strCode = "Messages";

        $scope.$on('onOKClick', function (event, data) {
            $scope.sfxControl.dictAttributes.sfwMessageId = data.ID;
            $scope.objNewButtonNaviagtionParam.onMessageIdChange();
        });

        //$scope.dialog = ngDialog.open({
        //    template: 'Views/Form/SearchIDDescription.html',
        //    scope: $scope,
        //    className: 'ngdialog-theme-default',
        //    controller: "SearchIDDescriptionController",
        //    closeByDocument: false,
        //});

        var newScope = $scope.$new(true);
        newScope.strCode = "Messages";

        newScope.SearchIDDescrDialog = $rootScope.showDialog(newScope, "Search ID Description", "Common/views/SearchIDDescription.html", { width: 800 });
    };

    //#endregion

    //#region New Button Custom Attributes
    $scope.InitForCustomAttributes = function (strPropertyName) {
        $scope.objNewButtonCustomAttr.IsShowAllControl = false;
        if (strPropertyName == "sfwCustomAttributes") {
            $scope.objNewButtonCustomAttr.CustomAttributeText = "Custom Attributes:";
            if ($scope.objNewButtonCustomAttr.IsShowAllControlForCustAttr !== undefined) {
                $scope.objNewButtonCustomAttr.IsShowAllControl = $scope.objNewButtonCustomAttr.IsShowAllControlForCustAttr;
            }
        }
        else {
            $scope.objNewButtonCustomAttr.CustomAttributeText = "User Log Parameters:";
            if ($scope.objNewButtonCustomAttr.IsShowAllControlForUserParam !== undefined) {
                $scope.objNewButtonCustomAttr.IsShowAllControl = $scope.objNewButtonCustomAttr.IsShowAllControlForUserParam;
            }
        }

        $scope.objNewButtonCustomAttr.FieldCollection = [];
        $scope.objNewButtonCustomAttr.ParameterCollection = [];
        $scope.objNewButtonCustomAttr.SelectedParameter = undefined;
        $scope.objNewButtonCustomAttr.LoadAvailableFields();
        $scope.objNewButtonCustomAttr.Initialize(strPropertyName);
    };
    //#endregion

    //#region Common Method for Custom Attributes and User Log Parameter

    $scope.objNewButtonCustomAttr.SelectParameter = function (param) {
        $scope.objNewButtonCustomAttr.SelectedParameter = param;
    }

    $scope.objNewButtonCustomAttr.Initialize = function (propertyName) {
        var customAttribute = $scope.sfxControl.dictAttributes[propertyName];
        if (customAttribute != undefined && customAttribute != "") {
            var alParams = customAttribute.split(';');
            angular.forEach(alParams, function (strParam) {
                if (strParam == undefined || strParam == "") {
                }
                else {
                    var strParamField = strParam;
                    var strParamValue = strParam;
                    var blnConstant = false;

                    if (strParam.contains("=")) {
                        strParamField = strParam.substring(0, strParam.indexOf('='));
                        strParamValue = strParam.substring(strParam.indexOf('=') + 1);

                        if (strParamValue.match("^#")) {
                            strParamValue = strParamValue.substring(1);
                            blnConstant = true;
                        }
                    }
                    var objParameter = {
                        ParameterField: strParamField, ParameterValue: strParamValue, Constants: blnConstant
                    };

                    $scope.objNewButtonCustomAttr.ParameterCollection.push(objParameter);
                }
            });
        }
    };

    $scope.objNewButtonCustomAttr.LoadAvailableFields = function () {
        var strProperty = "";
        var iblnIsLookup = $scope.formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";

        if (iblnIsLookup) {
            strProperty = "sfwDataField";
        }
        else {
            strProperty = "sfwEntityField";
        }

        $scope.objMainTable = undefined;
        var CurrentTable = FindParent($scope.sfxCell, "sfwTable");
        var lst = $scope.formodel.Elements.filter(function (x) { return x.Name == "sfwTable"; });
        if (lst && lst.length > 0) {
            $scope.objMainTable = lst[0];
        }

        $scope.objNewButtonCustomAttr.FieldCollection = [];

        var mainItem = {
            Text: "Main", Items: [], IsSelected: false, IsCheckBoxVisible: false
        };

        if ($scope.objNewButtonCustomAttr.IsShowAllControl) {
            PopulateAvailableFields(strProperty, $scope.objMainTable, mainItem, true, iblnIsLookup, false);
        }
        else {
            PopulateAvailableFields(strProperty, CurrentTable, mainItem, true, iblnIsLookup, false);
        }

        if (mainItem.Items.length > 0) {
            $scope.objNewButtonCustomAttr.FieldCollection.push(mainItem);
            // console.log($scope.objNewButtonCustomAttr.FieldCollection);
        }
    };

    $scope.objNewButtonCustomAttr.showAllControlChange = function () {
        $scope.objNewButtonCustomAttr.LoadAvailableFields();
    };

    $scope.objNewButtonCustomAttr.AddToGridClick = function () {
        if ($scope.objNewButtonCustomAttr.FieldCollection.length > 0) {
            $scope.objNewButtonCustomAttr.TraverseFieldCollection($scope.objNewButtonCustomAttr.FieldCollection[0].Items);
            if ($scope.objNewButtonCustomAttr.ParameterCollection.length > 0) {
                $scope.objNewButtonCustomAttr.SelectedParameter = $scope.objNewButtonCustomAttr.ParameterCollection[$scope.objNewButtonCustomAttr.ParameterCollection.length - 1];
            }
        }
    };

    $scope.objNewButtonCustomAttr.onAddParameter = function () {
        $scope.objNewButtonCustomAttr.ParameterCollection.push({
            ParameterField: "", ParameterValue: ""
        });
        if ($scope.objNewButtonCustomAttr.ParameterCollection.length > 0) {
            $scope.objNewButtonCustomAttr.SelectedParameter = $scope.objNewButtonCustomAttr.ParameterCollection[$scope.objNewButtonCustomAttr.ParameterCollection.length - 1];
        }
    };

    $scope.objNewButtonCustomAttr.onDeleteParameter = function () {
        if ($scope.objNewButtonCustomAttr.SelectedParameter) {
            var index = $scope.objNewButtonCustomAttr.ParameterCollection.indexOf($scope.objNewButtonCustomAttr.SelectedParameter);
            $scope.objNewButtonCustomAttr.ParameterCollection.splice(index, 1);

            if (index < $scope.objNewButtonCustomAttr.ParameterCollection.length) {
                $scope.SelectedParameter = $scope.objNewButtonCustomAttr.ParameterCollection[index];
            }
            else if ($scope.objNewButtonCustomAttr.ParameterCollection.length > 0) {
                $scope.objNewButtonCustomAttr.SelectedParameter = $scope.objNewButtonCustomAttr.ParameterCollection[index - 1];
            }
            else if ($scope.objNewButtonCustomAttr.ParameterCollection.length == 0) {
                $scope.objNewButtonCustomAttr.SelectedParameter = undefined;
            }
        }
    };

    $scope.objNewButtonCustomAttr.TraverseFieldCollection = function (fieldCollection) {
        angular.forEach(fieldCollection, function (field) {
            if (field.IsSelected) {
                var strFld = field.Text;
                var blnFound = false;
                var lst = $scope.objNewButtonCustomAttr.ParameterCollection.filter(function (itm) {
                    return itm.ParameterValue == strFld;
                });
                if (lst && lst.length > 0) {
                    blnFound = true;
                }

                if (blnFound) {
                    $SgMessagesService.Message('Message', strFld + " Column is already added in Collection, please check.");
                }
                else {
                    var strValue = $scope.objNewButtonCustomAttr.GetCaptionFromFieldName(strFld);
                    strValue = $scope.objNewButtonCustomAttr.RemoveInternalSpace(strValue);
                    var objParameters = { ParameterField: strValue, ParameterValue: strFld };
                    $scope.objNewButtonCustomAttr.ParameterCollection.push(objParameters);
                }
                field.IsSelected = false;
            }
            if (field.Items.length > 0) {
                $scope.objNewButtonCustomAttr.TraverseFieldCollection(field.Items);
            }
        });
    };

    $scope.objNewButtonCustomAttr.RemoveInternalSpace = function (astrInput) {
        astrInput = astrInput.trim();
        while (astrInput.indexOf(" ") > 0) {
            astrInput = astrInput.substring(0, astrInput.indexOf(" ")).trim() + astrInput.substring(astrInput.indexOf(" ")).trim();
        }
        return astrInput;
    };

    $scope.objNewButtonCustomAttr.GetCaptionFromFieldName = function (str) {
        if (str.match("^icdo"))
            str = str.replace("icdo", "");

        var strCaption = "";
        var blnCapsNext = true;

        for (var i = 0; i < str.length; i++) {
            if ("._".contains("" + str[i])) {
                blnCapsNext = true;
                strCaption += " ";
            }
            else {
                strCaption += blnCapsNext ? str.toUpperCase()[i] : str[i];
                blnCapsNext = false;
            }
        }

        if (strCaption.match(" Id$"))
            strCaption = strCaption.replace(" Id", " ID");
        if (strCaption.contains("Ssn"))
            strCaption = strCaption.replace("Ssn", "SSN");

        var intValuePos = strCaption.indexOf(" Value");
        if (intValuePos > 0)
            strCaption = strCaption.substring(0, intValuePos);

        var intDescPos = strCaption.indexOf(" Description");
        if (intDescPos > 0)
            strCaption = strCaption.substring(0, intDescPos);

        return strCaption;
    };

    $scope.objNewButtonCustomAttr.GetSavedString = function () {
        var strReturn = "";
        angular.forEach($scope.objNewButtonCustomAttr.ParameterCollection, function (objParams) {
            var strParamField = objParams.ParameterField;
            var strParamValue = objParams.ParameterValue;
            if ((strParamValue != undefined && strParamValue != "") || (strParamField != undefined && strParamField != "")) {
                var blnConstatnt = objParams.Constants;

                if (blnConstatnt) {
                    strParamValue = "#" + strParamValue;
                }

                var strParam = strParamValue;

                if (strParamValue.toLowerCase() != strParamField.toLowerCase()) {
                    strParam = strParamField + '=' + strParamValue;
                }

                if (strReturn == "") {
                    strReturn = strParam;
                }
                else {
                    strReturn += ';' + strParam;
                }
            }
        });
        return strReturn;
    };

    //#endregion

    //#region New Button User Log Parameters
    $scope.objNewButtonCustomAttr.OnBackClick = function () {
        var strUserLogParameters = $scope.objNewButtonCustomAttr.GetSavedString();
        if (strUserLogParameters != undefined && strUserLogParameters != "") {
            $scope.sfxControl.dictAttributes.sfwUserLogParameters = strUserLogParameters;
        }
        $scope.objNewButtonCustomAttr.IsShowAllControlForUserParam = $scope.objNewButtonCustomAttr.IsShowAllControl;
        $scope.InitForCustomAttributes("sfwCustomAttributes");
    };

    $scope.objNewButtonCustomAttr.OnBackToGeneralPropClick = function () {
        var strUserLogParameters = $scope.objNewButtonCustomAttr.GetSavedString();
        $scope.sfxControl.dictAttributes.sfwCustomAttributes = strUserLogParameters;
        $scope.objNewButtonCustomAttr.IsShowAllControlForCustAttr = $scope.objNewButtonCustomAttr.IsShowAllControl;
    }
    //#endregion

    //#endregion

    //#region Method For Filter Grid Pages 
    $scope.InitForGridControl = function () {
        $scope.objNewButtonFilterGrid.lstRelatedGrid = [];

        PopulateRelatedGrid($scope.formodel, $scope.objNewButtonFilterGrid.lstRelatedGrid, false);
    };

    $scope.objNewButtonFilterGrid.onQuerySearchClick = function (param) {
        $scope.IsFilterGridSearch = true;
        $scope.QueryDialog = ngDialog.open({
            template: "Views/Form/BrowseForQuery.html",
            scope: $scope,
            closeByDocument: false,
            className: 'ngdialog-theme-default ngdialog-theme-custom',
        });
    };

    $scope.$on('onFilterGridSerchClick', function (event, data, QueryID) {
        if (data) {
            $scope.objNewButtonFilterGrid.StrBaseQuery = QueryID;

        }
    });


    $scope.objNewButtonFilterGrid.PopulateQueryParam = function (parameters, isGrid) {
        var objGrid = undefined;
        if (isGrid) {
            objGrid = $scope.GetGridModel();
        }
        angular.forEach(parameters, function (x) {
            $scope.objNewButtonFilterGrid.lstQryField.push({
                CodeID: x.ID
            });
        });
        if (objGrid) {
            var lst = objGrid.Elements.filter(function (x) {
                return x.Name == "Parameters";
            });
            if (lst && lst.length > 0) {
                var gridParams = lst[0];
                if (gridParams.Elements.length > 0) {
                    angular.forEach(gridParams.Elements, function (itm) {
                        var lst1 = $scope.objNewButtonFilterGrid.lstQryField.filter(function (x) {
                            return x.CodeID == itm.dictAttributes.ID;
                        });
                        if (lst1 && lst1.length > 0) {
                            lst1[0].ParamValue = itm.dictAttributes.sfwEntityField;
                        }
                    });
                }
            }
        }
    };

    $scope.objNewButtonFilterGrid.onQueryChange = function () {
        $scope.objNewButtonFilterGrid.lstQryField = [];
        $scope.objNewButtonFilterGridCriteria.AvailableFieldColletion = [];
        $scope.objFilterGridFieldsDetails.AvailableFieldColletion = [];
        $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection = [];
        $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid = [];
        if ($scope.objNewButtonFilterGrid.StrBaseQuery) {

            var queryId = $scope.objNewButtonFilterGrid.StrBaseQuery;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var lst = queryId.split('.');
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var strQueryID = lst[1];
                $scope.entityName = lst[0];
                var lstEntity = entityIntellisenseList.filter(function (x) {
                    return x.ID == entityName;
                });
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) {
                        return x.ID == strQueryID;
                    });
                    if (lstQuery && lstQuery.length > 0) {
                        var objQuery = lstQuery[0];
                        $scope.objNewButtonFilterGrid.PopulateQueryParam(objQuery.Parameters, false);
                    }
                }
            }
        }
    };

    $scope.objNewButtonFilterGrid.ValidateQuery = function (objFile, astrQuery) {
        if (objFile) {
            var objQueries = objFile.Queries.filter(function (itm) {
                return itm.ID == astrQuery;
            });

            // XElement xEleQueries = xDoc.Descendants(ApplicationConstants.XMLFacade.QUERIES).FirstOrDefault();
            if (!objQueries || (objQueries && objQueries.length == 0)) {
                this.ErrorMessageForDisplay = "Invalid Base Query";
                return true;
            }
        }
        return false;
    };


    /************************************ populating Available fields (second screen)*********************************************************/
    $scope.objNewButtonFilterGridCriteria.PopulateQueryFields = function () {
        angular.forEach($scope.objNewButtonFilterGridCriteria.lstColumns, function (x) {
            /*for second screen*/
            if ($scope.objNewButtonFilterGridCriteria.AvailableFieldColletion.length > 0 && !$scope.objNewButtonFilterGridCriteria.AvailableFieldColletion.some(function (field) { return field.ControlName == x.CodeID })) {
                $scope.objNewButtonFilterGridCriteria.AvailableFieldColletion.push({
                    ControlName: x.CodeID, IsSelected: x.IsSelected, DataType: x.DataType
                });
            }
            if ($scope.objNewButtonFilterGridCriteria.AvailableFieldColletion.length == 0) {
                $scope.objNewButtonFilterGridCriteria.AvailableFieldColletion.push({
                    ControlName: x.CodeID, IsSelected: x.IsSelected, DataType: x.DataType
                });
            }
            // /*for third screen for Query Schema*/

            $scope.objFilterGridFieldsDetails.AvailableFieldColletion = angular.copy($scope.objNewButtonFilterGridCriteria.AvailableFieldColletion);

        });
    };
    //$scope.objFilterGridFieldsDetails.PopulateQueryFields = function () {
    //    angular.forEach($scope.objFilterGridFieldsDetails.lstQueryColumn, function (x) {

    //        if ($scope.objFilterGridFieldsDetails.AvailableFieldColletion.length > 0 && !$scope.objFilterGridFieldsDetails.AvailableFieldColletion.some(function (field) { return field.ControlName == x.CodeID })) {
    //            $scope.objFilterGridFieldsDetails.AvailableFieldColletion.push({
    //                ControlName: x.CodeID, IsSelected: x.IsSelected, DataType: x.DataType
    //            });
    //        }
    //        if ($scope.objFilterGridFieldsDetails.AvailableFieldColletion.length == 0) {
    //            $scope.objFilterGridFieldsDetails.AvailableFieldColletion.push({
    //                ControlName: x.CodeID, IsSelected: x.IsSelected, DataType: x.DataType
    //            });
    //        }

    //    });
    //};

    $scope.objNewButtonFilterGrid.onRelatedGridChange = function () {
        var objGrid = $scope.GetGridModel();
        if (objGrid) {
            $scope.objNewButtonFilterGrid.StrBaseQuery = objGrid.dictAttributes.sfwBaseQuery;
            $scope.objNewButtonFilterGrid.onQueryChange();
            $scope.objNewButtonFilterGrid.LoadQueryColumns($scope.objNewButtonFilterGrid.StrBaseQuery, false);
        }
    };
    $scope.objNewButtonFilterGridCriteria.onMoveUpClick = function () {
        if ($scope.objNewButtonFilterGridCriteria.SelectedGridField) {
            var index = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.indexOf($scope.objNewButtonFilterGridCriteria.SelectedGridField);
            var item = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index - 1];
            $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index - 1] = $scope.objNewButtonFilterGridCriteria.SelectedGridField;
            $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index] = item;
            $scope.scrollBySelectedField("#filter-grid-result", ".selected");
        }
    };

    $scope.objNewButtonFilterGridCriteria.CanMoveUp = function () {
        var retVal = false;
        if ($scope.objNewButtonFilterGridCriteria.SelectedGridField) {
            var index = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.indexOf($scope.objNewButtonFilterGridCriteria.SelectedGridField);
            if (index == 0) {
                retVal = true;
            }
        }
        else {
            retVal = true;
        }
        return retVal;
    };

    $scope.objNewButtonFilterGridCriteria.CanMoveDown = function () {
        var retVal = false;
        if ($scope.objNewButtonFilterGridCriteria.SelectedGridField) {
            var index = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.indexOf($scope.objNewButtonFilterGridCriteria.SelectedGridField);
            if (index == $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.length - 1) {
                retVal = true;
            }
        }
        else {
            retVal = true;
        }
        return retVal;
    };

    $scope.objNewButtonFilterGridCriteria.onMoveDownClick = function () {
        if ($scope.objNewButtonFilterGridCriteria.SelectedGridField) {
            var index = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.indexOf($scope.objNewButtonFilterGridCriteria.SelectedGridField);
            var item = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index + 1];
            $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index + 1] = $scope.objNewButtonFilterGridCriteria.SelectedGridField;
            $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index] = item;
            $scope.scrollBySelectedField("#filter-grid-result", ".selected");
        }
    };

    $scope.scrollBySelectedField = function (parentDiv, selectedElement) {
        var $divDom = $(parentDiv);
        if ($divDom && $divDom.hasScrollBar()) {
            $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 300, offsetLeft: 0 }, null);
            return false;
        }
    }

    $scope.objNewButtonFilterGridCriteria.onDeleteControlClick = function () {
        if ($scope.objNewButtonFilterGridCriteria.SelectedGridField) {
            var index = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.indexOf($scope.objNewButtonFilterGridCriteria.SelectedGridField);
            $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.splice(index, 1);

            if (index < $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.length) {
                $scope.objNewButtonFilterGridCriteria.SelectedGridField = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index];
            }
            else if ($scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.length > 0) {
                $scope.objNewButtonFilterGridCriteria.SelectedGridField = $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection[index - 1];
            }
            else {
                $scope.objNewButtonFilterGridCriteria.SelectedGridField = undefined;
            }
        }
    };
    //#endregion

    //#region Method For Filter Grid Criteria Required

    //#region Add Available Fields
    $scope.objNewButtonFilterGridCriteria.addAvailableFields = function () {

        $scope.AddFieldsFromQuery($scope.objNewButtonFilterGridCriteria.AvailableFieldColletion, $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection);
    };

    $scope.AddFieldsFromQuery = function (AvailableFieldColletion, GridNonAvailableControlCollection) {
        function iChecklstQueryColumnSelected(item) {
            if (item.IsSelected && item.IsSelected == true) {
                if (GridNonAvailableControlCollection && !GridNonAvailableControlCollection.some(function (x) { return x.ID == item.ControlName; })) {
                    var Field = {};
                    Field.ID = item.ControlName;
                    if (QueryID && item.ControlName && item.ControlName.indexOf("_value") > -1) {
                        Field.ControlClass = "DropDownList";
                        Field.ControlName = "ddl" + CreateControlIDInCamelCase(item.ControlName);
                        var objControl = FindControlByID($scope.formodel, Field.ControlName);
                        if (objControl) {
                            Field.ControlName = GetNewSeriesName(Field.ControlName, $scope.formodel, 1)
                        }
                        if (item.ControlName.indexOf("_value") > -1) {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            Field.CodeGroup = GetCodeIDByValue(QueryID, item.ControlName, entityIntellisenseList);
                        }
                        else {
                            Field.CodeGroup = "";
                        }
                        Field.Operator = "=";
                    }
                    else {
                        Field.ControlClass = "TextBox";
                        Field.ControlName = "txt" + CreateControlIDInCamelCase(item.ControlName);
                        var objControl = FindControlByID($scope.formodel, Field.ControlName);
                        if (objControl) {
                            Field.ControlName = GetNewSeriesName(Field.ControlName, $scope.formodel, 1)
                        }

                    }
                    Field.strDataType = item.DataType;
                    if (item.DataType) {
                        if (item.DataType.toLowerCase() == "int") {
                            Field.strDataType = "int";
                        }
                        else if (item.DataType.toLowerCase() == "string") {
                            Field.strDataType = "string";
                        }
                    }
                    if (item && item.Value) {
                        Field.strHeaderText = item.Value.substring(item.Value.lastIndexOf('.') + 1);
                    }
                    Field.strHeaderText = GetCaptionFromField(Field);


                    Field.IsVisible = "True";
                    Field.strControlType = 'Label';
                    if (item.ControlName.Caption && item.ControlName.Caption.trim().indexOf(":") == item.ControlName.Caption.trim().length - 1) { // if caption alreday has colon (:)
                        Field.strCaption = GetCaptionFromFieldName(item.ControlName);
                    }
                    else {
                        Field.strCaption = GetCaptionFromFieldName(item.ControlName) + " : ";
                    }

                    GridNonAvailableControlCollection.push(Field);
                }
                item.IsSelected = false;
            }
        }


        var QueryID = $scope.objNewButtonFilterGrid.StrBaseQuery.split('.')[0];

        angular.forEach(AvailableFieldColletion, iChecklstQueryColumnSelected);

    }
    //#endregion
    //#endregion

    //#region Method for Open Word Document Button Method
    $scope.receiveCorrespondenceTemplateForForm = function (data) {
        $scope.$apply(function () {
            $scope.objNewButtonOpenWordDoc.lstCorrTemplates = data;
            $rootScope.IsLoading = false;
        });
    };

    //#endregion

    //#region Method For Execute Button Methods

    $scope.objNewButtonServerMethod.Init = function () {
        $scope.Title = "Object Method :";
        $scope.objNewButtonServerMethod.sfwExecuteMethodType = "ObjectMethod";
        $scope.showObjectMethod = true;
        $scope.objNewButtonServerMethod.PopulateEntityXmlMethod();
        $scope.objNewButtonServerMethod.onRemoteObjectChanged();
    };

    $scope.objNewButtonServerMethod.onEntityChange = function () {
        $scope.objNewButtonServerMethod.lstXmlMethods = [];
        //if ($scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethod_Click"
        //            || $scope.SelectedButtonDetails.Method == "btnValidateExecuteBusinessMethod_Click" || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethodSelectRows_Click") {
        //    var objMethod = { ID: "", SrvName: "" };
        //    $scope.objNewButtonServerMethod.lstXmlMethods.push(objMethod);
        //}
        $scope.objNewButtonServerMethod.PopulateEntityXmlMethod();
    };

    $scope.objNewButtonServerMethod.PopulateEntityXmlMethod = function () {
        var lstObjectMethods = GetObjectMethods($EntityIntellisenseFactory.getEntityIntellisense(), $scope.formodel.dictAttributes.sfwEntity);
        $scope.objNewButtonServerMethod.lstXmlMethods = PopulateServerMethod(lstObjectMethods, $scope.sfxControl, undefined);

    };

    $scope.objNewButtonServerMethod.onRemoteObjectChanged = function () {
        var RemoteObjectName = "srvCommon";
        if ($scope.formodel && $scope.formodel.dictAttributes.sfwRemoteObject) {
            RemoteObjectName = $scope.formodel.dictAttributes.sfwRemoteObject;
        }
        var obj = GetServerMethodObject(RemoteObjectName, $scope.formodel.RemoteObjectCollection);
        var lstObjectMethods = GetObjectMethods($EntityIntellisenseFactory.getEntityIntellisense(), $scope.formodel.dictAttributes.sfwEntity);
        $scope.objNewButtonServerMethod.lstXmlMethods = PopulateServerMethod(lstObjectMethods, $scope.sfxControl, obj);
    };
    //#endregion

    //#region On Next
    $scope.objNewButtonActiveForm.OnNextClick = function () {
        if ($scope.objNewButtonActiveForm.ActiveFormType == "SingleForm") {
            $scope.sfxControl.dictAttributes.sfwActiveForm = $scope.objNewButtonActiveForm.StrActiveForm;
        }
        else {
            if ($scope.objNewButtonActiveForm.StrEntityField != undefined && $scope.objNewButtonActiveForm.StrEntityField != "") {
                $scope.sfxControl.dictAttributes.sfwEntityField = $scope.objNewButtonActiveForm.StrEntityField;
            }
            $scope.objNewButtonActiveForm.UpdateActiveForms();
        }

        $scope.objNewButtonNaviagtionParam.Init();
    };

    $scope.objNewButtonActiveForm.UpdateActiveForms = function () {
        var istrActiveForms = "";
        $scope.objNewButtonNaviagtionParam.TargetForm = "";
        angular.forEach($scope.objNewButtonActiveForm.ActiveFormCollection, function (objActiveForm) {
            var strFieldValue = objActiveForm.FieldValue;
            var strActiveForm = objActiveForm.ActiveForm;

            if ((strFieldValue != undefined && strFieldValue != "") && (strActiveForm != undefined && strActiveForm != "")) {
                var strForm = strFieldValue + "=" + strActiveForm;

                if (istrActiveForms == "")
                    istrActiveForms = strForm;
                else
                    istrActiveForms += ';' + strForm;
            }
        });
        if ($scope.sfxControl) {
            $scope.sfxControl.dictAttributes.sfwActiveForm = istrActiveForms;
        }
    };

    $scope.objNewButtonNaviagtionParam.OnNextClick = function () {
        $scope.objNewButtonNaviagtionParam.UpdateParameters();
        $scope.InitForCustomAttributes("sfwCustomAttributes");
    };

    $scope.objNewButtonNaviagtionParam.UpdateParameters = function () {
        var istrParameters = "";

        angular.forEach($scope.objNewButtonNaviagtionParam.ParameterCollection, function (objParameter) {
            if (objParameter.ParameterValue != undefined && objParameter.ParameterValue != "") {
                var strParamValue = objParameter.ParameterValue;

                if (strParamValue != "") {
                    var blnConstant = (objParameter.Constants != undefined) ? objParameter.Constants : false;
                    if (blnConstant)
                        strParamValue = "#" + strParamValue;

                    var strParam = strParamValue;
                    var strParamField = objParameter.ParameterField;

                    if (strParamValue.toLowerCase() != strParamField.toLowerCase())
                        strParam = strParamField + '=' + strParamValue;

                    if (istrParameters == "")
                        istrParameters = strParam;
                    else
                        istrParameters += ';' + strParam;
                }
            }
        });

        if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {

            /*Related control will come in navigation parameter only for Multiple Active Forms*/
            var relatedControl = $scope.sfxControl.dictAttributes.sfwActiveForm.contains("=") ? $scope.sfxControl.dictAttributes.sfwRelatedControl : "";
            if (istrParameters == "") {
                $scope.sfxControl.dictAttributes.sfwNavigationParameter = relatedControl;
            }
            else {

                if (relatedControl) {
                    $scope.sfxControl.dictAttributes.sfwNavigationParameter = istrParameters + ";" + relatedControl;

                }
                else {
                    $scope.sfxControl.dictAttributes.sfwNavigationParameter = istrParameters;

                }

            }
        }
        else if ($scope.sfxControl.dictAttributes.sfwActiveForm && $scope.sfxControl.dictAttributes.sfwActiveForm.contains("=")) {
            if ($scope.sfxControl.dictAttributes.sfwMethodName == "btnOpen_Click") {
                if (istrParameters == "") {
                    $scope.sfxControl.dictAttributes.sfwNavigationParameter = $scope.sfxControl.dictAttributes.sfwEntityField;
                }
                else {
                    $scope.sfxControl.dictAttributes.sfwNavigationParameter = istrParameters + ";" + $scope.sfxControl.dictAttributes.sfwEntityField;
                }
            }
            else {
                $scope.sfxControl.dictAttributes.sfwNavigationParameter = istrParameters;
            }
        }
        else {
            $scope.sfxControl.dictAttributes.sfwNavigationParameter = istrParameters;
        }
    };

    $scope.objNewButtonGeneralProp.OnNextClick = function () {
        $scope.InitForCustomAttributes("sfwCustomAttributes");
    };

    $scope.objNewButtonCustomAttr.OnNextClick = function () {
        var strCustomAttribute = $scope.objNewButtonCustomAttr.GetSavedString();
        $scope.sfxControl.dictAttributes.sfwCustomAttributes = strCustomAttribute;

        $scope.objNewButtonCustomAttr.IsShowAllControlForCustAttr = $scope.objNewButtonCustomAttr.IsShowAllControl;
        $scope.InitForCustomAttributes("sfwUserLogParameters");
    };

    $scope.objNewButtonFilterGrid.OnNextClick = function () {
        if ($scope.objNewButtonFilterGrid.StrBaseQuery) {
            if ($scope.objNewButtonFilterGrid.SelectGridOption == "UseExistingGrid") {
                var objGrid = $scope.GetGridModel();
                if (objGrid) {
                    if (objGrid.dictAttributes.sfwBaseQuery != $scope.objNewButtonFilterGrid.StrBaseQuery) {
                        objGrid.dictAttributes.sfwBaseQuery = $scope.objNewButtonFilterGrid.StrBaseQuery;
                    }

                }
            }
            else {

            }
            $scope.objNewButtonFilterGrid.LoadQueryColumns($scope.objNewButtonFilterGrid.StrBaseQuery, true);
            if ($scope.objNewButtonFilterGrid.SelectGridOption == "UseExistingGrid") {
                $scope.objNewButtonFilterGrid.UpdateQueryParam();
            }
            $scope.objNewButtonFilterGridCriteria.StrNoOfColumn = "2";
        }
    };

    $scope.GetGridModel = function () {
        $scope.objMainTable = undefined;
        var lst = $scope.formodel.Elements.filter(function (x) {
            return x.Name == "sfwTable";
        });
        if (lst && lst.length > 0) {
            $scope.objMainTable = lst[0];
        }
        var objGrid = FindControlByID($scope.objMainTable, $scope.sfxControl.dictAttributes.sfwRelatedControl);
        return objGrid;
    };

    $scope.objNewButtonFilterGrid.LoadQueryColumns = function (queryId, IsFilterGrid) {
        if (queryId) {
            var lst = queryId.split('.');
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var strQueryID = lst[1];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lstEntity = entityIntellisenseList.filter(function (x) {
                    return x.ID == entityName;
                });
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) {
                        return x.ID == strQueryID;
                    });
                    if (lstQuery && lstQuery.length > 0) {
                        var objQuery = lstQuery[0];
                        $rootScope.IsLoading = true;
                        if (IsFilterGrid) {
                            $.connection.hubForm.server.getEntityQueryColumns(queryId, "").done(function (data) {
                                $scope.receiveGridBaseQueryColumns(data);
                            });
                        }
                        else {
                            $.connection.hubForm.server.getEntityQueryColumns(queryId, "CreateNewButton").done(function (data) {
                                $scope.receiveGridBaseQueryColumns(data);
                            });
                        }
                    }
                }
            }
        }
    };

    /*****************************   getting query Schema for second Screen *******************/
    $scope.receiveGridBaseQueryColumns = function (data) {

        $scope.$apply(function () {
            $scope.objNewButtonFilterGridCriteria.lstColumns = data;
            if ($scope.objNewButtonFilterGridCriteria.lstColumns.length > 0) {
                $scope.objNewButtonFilterGridCriteria.PopulateQueryFields(); // getting available fields(for second screen) once we are getting query Schema from  hub
            }
            $rootScope.IsLoading = false;
        });
    };


    $scope.objNewButtonFilterGrid.UpdateQueryParam = function () {
        if ($scope.objNewButtonFilterGrid.lstQryField.length > 0) {
            var objGrid = $scope.GetGridModel();
            if (objGrid) {
                var objParams = objGrid.Elements.filter(function (itm) {
                    return itm.Name == "Parameters";
                });
                if (objParams && objParams.length > 0) {
                    var index = objGrid.Elements.indexOf(objParams[0]);
                    objGrid.Elements.splice(index, 1);
                }

                objParams = {
                    Name: "Parameters", Value: '', dictAttributes: {
                    }, Elements: [], Children: []
                };
                objParams.ParentVM = objGrid;
                objGrid.Elements.push(objParams);
                angular.forEach($scope.objNewButtonFilterGrid.lstQryField, function (objParam) {
                    var param = {
                        Name: "parameter", Value: '', dictAttributes: {
                            ID: objParam.CodeID, sfwEntityField: objParam.ParamValue
                        }, Elements: [], Children: []
                    };
                    objParams.Elements.push(param);
                });
            }
        }
    };

    $scope.objNewButtonOpenWordDoc.OnNextClick = function () {
        $scope.objNewButtonOpenWordDoc.SetNavigationParam();
    };

    $scope.objNewButtonOpenWordDoc.SetNavigationParam = function () {
        if (($scope.objNewButtonOpenWordDoc.TemplateName != undefined && $scope.objNewButtonOpenWordDoc.TemplateName != "") && ($scope.objNewButtonOpenWordDoc.TrackingID != undefined && $scope.objNewButtonOpenWordDoc.TrackingID != "")) {
            $scope.sfxControl.dictAttributes.sfwNavigationParameter = "TemplateName=" + $scope.objNewButtonOpenWordDoc.TemplateName + ";TrackingID=" + $scope.objNewButtonOpenWordDoc.TrackingID;
        }
        else if ($scope.objNewButtonOpenWordDoc.TemplateName != undefined && $scope.objNewButtonOpenWordDoc.TemplateName != "") {
            $scope.sfxControl.dictAttributes.sfwNavigationParameter = "TemplateName=" + $scope.objNewButtonOpenWordDoc.TemplateName;
        }
        else if ($scope.objNewButtonOpenWordDoc.TrackingID != undefined && $scope.objNewButtonOpenWordDoc.TrackingID != "") {
            $scope.sfxControl.dictAttributes.sfwNavigationParameter = "TrackingID=" + $scope.objNewButtonOpenWordDoc.TrackingID;
        }
    };

    $scope.objNewButtonServerMethod.OnNextClick = function () {
        if ($scope.SelectedButtonDetails.Method == "btnExecuteServerMethod_Click" || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethod_Click"
            || $scope.SelectedButtonDetails.Method == "btnValidateExecuteBusinessMethod_Click" || $scope.SelectedButtonDetails.Method == "btnDownload_Click"
            || $scope.SelectedButtonDetails.Method == "btnExecuteBusinessMethodSelectRows_Click" || $scope.SelectedButtonDetails.Method == "btnCompleteWorkflowActivities_Click") {
            if ($scope.SelectedButtonDetails.Method == "btnExecuteServerMethod_Click" || $scope.SelectedButtonDetails.Method == "btnDownload_Click") {
                //$scope.sfxControl.dictAttributes.sfwRemoteObject = $scope.objNewButtonServerMethod.StrRemoteObject;
            }

            $scope.sfxControl.dictAttributes.sfwObjectMethod = $scope.objNewButtonServerMethod.StrServerMethod;
            if ($scope.objNewButtonServerMethod.sfwExecuteMethodType) {
                $scope.sfxControl.dictAttributes.sfwExecuteMethodType = $scope.objNewButtonServerMethod.sfwExecuteMethodType;
            }
            $scope.objNewButtonNaviagtionParam.Init();
        }
    };

    //#endregion


    //#region On Finish
    $scope.OnFinishClick = function () {
        $rootScope.UndRedoBulkOp("Start");


        /* now filter Grid & "Filter Grid Criteria Required" are merged only in button wizard if "IscriteriaRequired" is checked then "Filter Grid Criteria Required" else "Filter Grid".*/
        if ($scope.objNewButtonFilterGrid.IsCriteriaRequired == "True") {
            $scope.sfxControl.dictAttributes["ID"] = "GridSearchCriteriaReq_Click";
            $scope.sfxControl.dictAttributes["sfwMethodName"] = "btnGridSearchCriteriaReq_Click";
            $scope.sfxControl.dictAttributes["Text"] = "btnGridSearchCriteriaReq_Click";
        }

        if ($scope.SelectedButtonDetails.Method == "btnOpenPopupDialog_Click" || $scope.SelectedButtonDetails.Method == "btnNewPopupDialog_Click"
            || $scope.SelectedButtonDetails.Method == "btnFinishPopupDialog_Click" || $scope.SelectedButtonDetails.Method == "btnClosePopupDialog_Click") {
            $scope.CreatePopupButton();
        }
        else if (!$scope.IsNewButtonStepDisable || !$scope.IsNewSaveStepsDisable || !$scope.IsOpenWordDocStepsDisable || !$scope.IsNewExecuteStepsDisable
            || !$scope.IsNewExecuteBusinessStepsDisable) {
            var strCustomAttribute = $scope.objNewButtonCustomAttr.GetSavedString();
            if (strCustomAttribute) {
                $scope.sfxControl.dictAttributes.sfwUserLogParameters = strCustomAttribute;
            }
            if ($scope.objNewButtonActiveForm.objRelatedGrid) {
                $scope.sfxControl.IsChildOfGrid = true;
            }
            if ($scope.sfxControl) {
                $scope.AddControlToCell("sfwButton", $scope.sfxCell, $scope.sfxControl);
            }
        }
        else if (!$scope.IsNewGridStepDisable) {
            if ($scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection.length > 0) {
                $scope.objNewButtonFilterGridCriteria.AddControls($scope.sfxCell, $scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection);
            }
            else {
                if ($scope.sfxControl) {
                    $scope.AddControlToCell("sfwButton", $scope.sfxCell, $scope.sfxControl);
                }
            }

            $scope.objNewButtonFilterGridCriteria.SetNavigationParameter();
        }
        else if (!$scope.IsNewFilterGridSearchCriteriaStepDisable) {
            $scope.sfxControl.dictAttributes.sfwObjectMethod = $scope.objNewButtonServerMethod.StrServerMethod;
            if ($scope.sfxControl) {
                $scope.AddControlToCell("sfwButton", $scope.sfxCell, $scope.sfxControl);
            }
        }
        else if (!$scope.IsGeneralPropStepsDisable) {
            if ($scope.sfxControl) {
                $scope.AddControlToCell("sfwButton", $scope.sfxCell, $scope.sfxControl);
            }
        }
        $rootScope.UndRedoBulkOp("End");
        $scope.CloseWizardClick();
    };


    /********************************************** Third Screen Start******************************************************/
    $scope.objFilterGridFieldsDetails.Init = function () {
        $scope.objFilterGridFieldsDetails.LoadData();

    }

    //#region  Move up for Fields Properties

    $scope.objFilterGridFieldsDetails.moveSelectedRowUp = function () {
        if ($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
            var index = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.indexOf($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow);
            var item = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index - 1];
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index - 1] = $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow;
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index] = item;
            $scope.scrollBySelectedField(".lookup-result-grid", ".selected");
        }
    };

    // disable the move up button if there is no element to move up
    $scope.objFilterGridFieldsDetails.canmoveSelectedRowUp = function () {
        $scope.Flag = true;
        if ($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
            for (var i = 0; i < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length; i++) {
                if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[i] == $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
                    if (i > 0) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };
    //#endregion

    //#region  Move Down for Fields Properties
    $scope.scrollBySelectedField = function (parentDiv, selectedElement) {
        $timeout(function () {
            var $divDom = $(parentDiv);
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 300, offsetLeft: 0 }, null);
                return false;
            }

        });
    }

    $scope.objFilterGridFieldsDetails.moveSelectedRowDown = function () {
        if ($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
            var index = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.indexOf($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow);
            var item = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index + 1];
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index + 1] = $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow;
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index] = item;
            $scope.scrollBySelectedField(".lookup-result-grid", ".selected");
        }
    };

    $scope.objFilterGridFieldsDetails.canmoveSelectedRowDown = function () {
        $scope.Flag = true;
        if ($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
            for (var i = 0; i < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length; i++) {
                if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[i] == $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
                    if (i < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length - 1) {
                        $scope.Flag = false;
                    }
                }
            }
        }

        return $scope.Flag;
    };
    //#endregion

    $scope.objFilterGridFieldsDetails.deleteSelectedRow = function () {
        var index = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.indexOf($scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow);
        if (index >= 0) {
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.splice(index, 1);
            if (index < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length) {
                $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index];
            }
            else if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length > 0) {
                $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow = $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[index - 1];
            }
        }
        $scope.LoadSort();
    };
    $scope.objFilterGridFieldsDetails.canDeleteSelectedRow = function () {
        var retValue = false;
        if (!$scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow) {
            retValue = true;
        }
        if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length == 0) {
            retValue = true;
        }
        return retValue;
    };
    $scope.objFilterGridFieldsDetails.onFinishButtonWizardClick = function () {

    }

    //#region Result Fields


    $scope.objFilterGridFieldsDetails.LoadData = function () {

        if ($scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection && $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid) {
            angular.forEach($scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection, function (item) {

                if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid && !$scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.some(function (x) { return x.ID == item.ID; })) {
                    var Field = {};
                    Field.ID = item.ID;

                    Field.strDataType = item.strDataType;
                    if (item.DataType) {
                        if (item.strDataType.toLowerCase() == "int") {
                            Field.strDataType = "int";
                        }
                        else if (item.strDataType.toLowerCase() == "string") {
                            Field.strDataType = "string";
                        }
                    }
                    if (item && item.Value) {
                        Field.strHeaderText = item.Value.substring(item.Value.lastIndexOf('.') + 1);
                    }
                    Field.strHeaderText = GetCaptionFromField(Field);


                    Field.IsVisible = "True";
                    Field.strControlType = 'Label';
                    if (item.ControlName.Caption && item.ControlName.Caption.trim().indexOf(":") == item.ControlName.Caption.trim().length - 1) { // if caption alreday has colon (:)
                        Field.strCaption = GetCaptionFromFieldName(item.ControlName);
                    }
                    else {
                        Field.strCaption = GetCaptionFromFieldName(item.ControlName) + " : ";
                    }

                    $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.push(Field);


                }
            });
        }
        $scope.LoadSort();
        $scope.LoadOrder();
    };


    $scope.LoadSort = function () {
        $scope.ArrSort = [];
        $scope.ArrSort.push("");
        if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length > 0) {
            for (var i = 0; i < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length; i++) {
                $scope.ArrSort.push(i + 1);
            }
        }

    };

    $scope.LoadOrder = function () {
        $scope.ArrOrder = [];
        $scope.ArrOrder.push("");
        $scope.ArrOrder.push("asc");
        $scope.ArrOrder.push("desc");
    };

    $scope.objFilterGridFieldsDetails.addResultFields = function () {
        $scope.AddFieldsFromQuery($scope.objFilterGridFieldsDetails.AvailableFieldColletion, $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid);
        $scope.LoadSort();

    };
    $scope.objFilterGridFieldsDetails.SelectedResultGridRow = function (obj) {
        if (obj) {
            $scope.objFilterGridFieldsDetails.SelectedCurrentResultGridRow = obj;
        }
    };

    //#region for Load Grid Data for Result Grid

    $scope.objFilterGridFieldsDetails.LoadGridDataForResultFields = function (ResultFieldCollection, isLoad) {
        if (!$scope.objFilterGridFieldsDetails.lstResultFieldsForGrid) {
            $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid = [];
        }

        var intCountPrimary = 0;

        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var primaryKeyFields = getPrimarykey(entityIntellisenseList, $scope.entityName);

        function iterator(itm) {
            var isFound = false;
            for (var i = 0; i < $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length; i++) {
                if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid[i].strFieldName == itm.EntityField) {
                    isFound = true;
                    break;
                }
            }
            if (!isFound) {
                var item = {
                };
                var ResultField = itm.FieldObject;
                item.strFieldName = itm.EntityField;
                if (isLoad) {
                    ResultField = itm;
                    item.strFieldName = itm.ID;
                }
                else {
                    item.IsAddFromResultFieldScreen = true;
                }
                //ResultField.ID;
                item.strDataType = ResultField.DataType;
                if (ResultField.DataType) {
                    if (ResultField.DataType.toLowerCase() == "int") {
                        item.strDataType = "int";
                    }
                    else if (ResultField.DataType.toLowerCase() == "string") {
                        item.strDataType = "string";
                    }
                }
                if (ResultField && ResultField.Value) {
                    item.strHeaderText = ResultField.Value.substring(ResultField.Value.lastIndexOf('.') + 1);
                }
                item.strHeaderText = GetCaptionFromField(ResultField);


                if (item.strDataType && (item.strDataType.toLowerCase() == "datetime" || item.strDataType.toLowerCase() == "date")) {
                    item.strDataFormat = '{0:d}';
                }
                else if (item.strDataType && item.strDataType.toLowerCase() == "decimal" && (ResultField.Value.indexOf("_amt") > -1 || ResultField.Value.indexOf("_amount") > -1)) {
                    item.strDataFormat = "{0:C}";
                }
                else if (ResultField.Value.contains("ssn")) {
                    item.strDataFormat = '{0:000-##-####}';
                }
                else if (ResultField.Value.contains("phone") || ResultField.Value.contains("fax")) {
                    item.strDataFormat = '{0:(###)###-####}';
                }
                else {
                    item.strDataFormat = '';
                }
                item.IsVisible = "True";
                item.strControlType = 'Label';
                if (primaryKeyFields.length > 0) {
                    var lstKey = primaryKeyFields.filter(function (x) { return x.ID.toLowerCase() == ResultField.ID.toLowerCase(); });
                    if (lstKey && lstKey.length > 0) {
                        item.strDataKey = lstKey[0].KeyNo;
                    }
                }

                if (!$scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.some(function (field) { return field.strFieldName == itm.ID; })) {
                    $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.push(item);
                }
            }
            if (!isLoad) {
                if (!$scope.objFilterGridFieldsDetails.preselectedfields.some(function (field) { return field.ID == itm.EntityField; })) {
                    if (itm.FieldObject) {
                        itm.FieldObject.IsSelected = "False";
                    }
                }
            }
        }

        if (ResultFieldCollection.length > 0) {
            angular.forEach(ResultFieldCollection, iterator);
        }
    };



    //#endregion
    //#endregion
    //#region Update data on Result Grid(sixth Screen)

    $scope.UpdateResultFields = function (objGridView) {
        function iterator(objGrid) {
            var strFieldName = objGrid.ID;
            var strDataType = objGrid.strDataType;
            var strHeaderText = objGrid.strHeaderText;
            var strControlType = objGrid.strControlType;
            var strDataFormat = objGrid.strDataFormat;

            strVisible = strFieldName.substring(strFieldName.lastIndexOf('.') + 1);
            strVisible = IsAuditField(strVisible) ? "False" : "True";


            var lobjTemp = {
                Name: 'TemplateField', prefix: 'asp', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            lobjTemp.dictAttributes.HeaderText = strHeaderText;
            lobjTemp.dictAttributes.Visible = strVisible;


            var objItemTemplate = {
                Name: 'ItemTemplate', prefix: 'asp', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            if (strControlType) {
                objControl = {
                    Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };
            }


            if (objControl.Name != "sfwLabel" && objControl.Name != "sfwButton" && objControl.Name != "sfwLinkButton" && objControl.Name != "sfwImageButton" && objControl.Name !== "sfwButtonGroup") {
                objGridView.dictAttributes.AllowEditing = "True";
                objGridView.dictAttributes.sfwTwoWayBinding = "True";
                objGridView.dictAttributes.sfwCommonFilterBox = "False";
                objGridView.dictAttributes.sfwFilterOnKeyPress = "False";
            }
            objControl.dictAttributes.sfwEntityField = objGrid.ID;
            objControl.dictAttributes.sfwDataFormat = strDataFormat;
            if (objGrid.IsVisible == "False") {
                objControl.dictAttributes.Visible = objGrid.IsVisible;
            }
            objItemTemplate.Elements.push(objControl);
            lobjTemp.Elements.push(objItemTemplate);
            objColumn.Elements.push(lobjTemp);

        }

        if ($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid && $scope.objFilterGridFieldsDetails.lstResultFieldsForGrid.length > 0) {
            for (var i = 0; i < objGridView.Elements.length; i++) {
                if (objGridView.Elements[i].Name == "Columns") {
                    objGridView.Elements.splice(i, 1);
                    break;
                }
            }

            var objColumn = {
                Name: 'Columns', Value: '', dictAttributes: {}, Elements: [], Children: []
            };

            angular.forEach($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid, iterator);

            $scope.UpdateGridProperties($scope.objFilterGridFieldsDetails.lstResultFieldsForGrid, objGridView);

            objGridView.Elements.push(objColumn);
        }
    };


    //#endregion

    /********************************************** Third Screen End******************************************************/

    $scope.objNewButtonFilterGridCriteria.SetNavigationParameter = function () {
        var strNavParam = "";
        angular.forEach($scope.objNewButtonFilterGridCriteria.GridNonAvailableControlCollection, function (tn) {
            strNavParam += tn.ControlName + ";";
        });

        if (strNavParam.length > 0) {
            if (strNavParam[strNavParam.length - 1] == ';')
                strNavParam = strNavParam.substring(0, strNavParam.length - 1);
        }
        $scope.sfxControl.dictAttributes.sfwNavigationParameter = strNavParam;
    };

    //#region For Create filter grid Methods
    $scope.objNewButtonFilterGridCriteria.AddControls = function (selectedCntrlVM, lstGrid) {
        var blnIsLookup = $scope.formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";
        var strTabSheetID = "";

        if (blnIsLookup) {
            if ($sope.sfxCell.ParentVM.Name == "sfwTabSheet") {
                strTabSheetID = $sope.sfxCell.ParentVM.dictAttributes.ID;
                if ((strTabSheetID != undefined && strTabSheetID != "") && (strTabSheetID == "tshAdvSort" || strTabSheetID == "tshSql")) {
                    return;
                }
            }
        }

        var dRowMultiplier = 0;

        var totalControlCount = 1;

        totalControlCount = lstGrid.length * 2;

        var colCount = $scope.objNewButtonFilterGridCriteria.StrNoOfColumn;
        var intRows = (totalControlCount / colCount) + dRowMultiplier;

        intRows = Math.round(intRows);
        if (intRows <= 0)   //atleast one row should be added
            intRows = 1;

        var cellVM = GetVM("sfwColumn", selectedCntrlVM);

        var intCurRowInd;
        if (cellVM) {
            var rownvM = cellVM.ParentVM;
            var rowindex = rownvM.ParentVM.Elements.indexOf(rownvM);
            intCurRowInd = rowindex;
        }
        else {
            var RowCount = tableVM.Elements.length;
            intCurRowInd = RowCount - 1;
        }

        var cellLst = [];

        var cellInd = 0;

        var sfxPanel = {
            Name: "sfwPanel", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: []
        };
        sfxPanel.ParentVM = $scope.sfxCell;
        sfxPanel.dictAttributes.ID = CreateControlID($scope.formodel, "NewPanel", "sfwPanel");
        var sfxTable = { Name: "sfwTable", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [] };
        sfxTable.ParentVM = sfxPanel;
        sfxPanel.IsVisible = true;
        sfxPanel.initialvisibilty = true;
        sfxPanel.isLoaded = true;

        for (var rowInd = 1; rowInd <= intRows; rowInd++) {
            var sfxRowModel = {
                Name: "sfwRow", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: []
            };
            sfxRowModel.ParentVM = sfxTable;

            for (var colInd = 0; colInd < $scope.objNewButtonFilterGridCriteria.StrNoOfColumn; colInd++) {
                var sfxCellModel = {
                    Name: "sfwColumn", Value: '', prefix: "swc", dictAttributes: {
                    }, Elements: [], Children: []
                };
                sfxCellModel.ParentVM = sfxRowModel;
                $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);

            }
            $rootScope.PushItem(sfxRowModel, sfxTable.Elements);

        }
        $rootScope.PushItem(sfxTable, sfxPanel.Elements);

        $rootScope.PushItem(sfxPanel, $scope.sfxCell.Elements);

        angular.forEach(sfxTable.Elements, function (rowVM) {
            if (rowVM.Name == "sfwRow") {
                angular.forEach(rowVM.Elements, function (vm) {
                    var cellitem = {
                    };
                    cellitem.key = cellInd;
                    cellitem.value = vm;
                    cellLst.push(cellitem);
                    cellInd++;
                });
            }
        });


        cellInd = 0;

        angular.forEach(lstGrid, function (objGrid) {
            var strControlType = undefined;
            if (objGrid.ControlClass && objGrid.ControlClass == "TextBox") {
                strControlType = "sfwTextBox";
            }
            else if (objGrid.ControlClass && objGrid.ControlClass == "DropDownList") {
                strControlType = "sfwDropDownList";
            }
            cellInd = $scope.objNewButtonFilterGridCriteria.AddControlsGrid(cellLst, strControlType, objGrid, cellInd);
        });

        var sfxGridRow = {
            Name: "sfwRow", Value: '', prefix: "swc", dictAttributes: {
            }, Elements: [], Children: []
        };
        sfxGridRow.ParentVM = sfxTable;

        for (var colInd = 0; colInd < $scope.objNewButtonFilterGridCriteria.StrNoOfColumn; colInd++) {
            var sfxGridColumn = {
                Name: "sfwColumn", Value: '', prefix: "swc", dictAttributes: {
                }, Elements: [], Children: []
            };
            sfxGridColumn.ParentVM = sfxGridRow;
            $rootScope.PushItem(sfxGridColumn, sfxGridRow.Elements);

        }

        if (sfxGridRow.Elements.length > 0) {
            var index1 = $scope.objNewButtonFilterGridCriteria.StrNoOfColumn - 1; //Convert.ToInt32(index);
            $rootScope.PushItem($scope.sfxControl, sfxGridRow.Elements[index1].Elements);
            $rootScope.PushItem(sfxGridRow, sfxTable.Elements);

        }
        if ($scope.objNewButtonFilterGrid.SelectGridOption == 'CreateNewGrid') {
            var objGridView = { Name: "sfwGridView", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [] };
            var strControlID = CreateControlID($scope.formodel, 'grvGridView', "sfwGridView");
            objGridView.dictAttributes.ID = strControlID;
            objGridView.dictAttributes.AllowPaging = "True";
            objGridView.dictAttributes.AllowSorting = "True";
            objGridView.dictAttributes.sfwSelection = "Many";
            objGridView.dictAttributes.sfwBaseQuery = $scope.objNewButtonFilterGrid.StrBaseQuery;
            objGridView.dictAttributes.sfwBoundToQuery = "True";
            objGridView.dictAttributes.sfwQueryLoadType = "Solution";
            $scope.sfxControl.dictAttributes.sfwRelatedControl = objGridView.dictAttributes.ID;
            $scope.UpdateResultFields(objGridView);
            var row = {
                Name: "sfwRow", Value: '', prefix: "swc", dictAttributes: {
                }, Elements: [], Children: []
            };
            row.ParentVM = sfxTable;
            for (var colInd = 0; colInd < $scope.objNewButtonFilterGridCriteria.StrNoOfColumn; colInd++) {
                var column = {
                    Name: "sfwColumn", Value: '', prefix: "swc", dictAttributes: {
                    }, Elements: [], Children: []
                };
                column.ParentVM = row;
                $rootScope.PushItem(column, row.Elements);

            }
            $rootScope.PushItem(row, sfxTable.Elements);
            if (sfxTable.Elements.length > 0) {
                var lastRow = sfxTable.Elements[sfxTable.Elements.length - 1];
                if (lastRow.Elements.length > 0) {
                    var lstcol = lastRow.Elements.filter(function (x) { return x.Name == "sfwColumn" });
                    if (lstcol && lstcol[0]) {
                        $rootScope.PushItem(objGridView, lstcol[0].Elements);
                        $scope.objNewButtonFilterGrid.UpdateQueryParam();
                    }
                }
            }
        }

    };

    $scope.objNewButtonFilterGridCriteria.AddControlsGrid = function (acellLst, astrControlClass, aobjGrid, aintCellInd) {
        var cellVM;
        var newControl;

        cellVM = acellLst[aintCellInd].value;
        aintCellInd++;

        newControl = {
            Name: "sfwLabel", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: []
        };
        newControl.ParentVM = cellVM;
        var strLableId = aobjGrid.ControlName;
        if (aobjGrid.ControlName.match("^txt") || aobjGrid.ControlName.match("^ddl")) {
            strLableId = aobjGrid.ControlName.substring(3);
        }
        var strLabelID = CreateControlID($scope.formodel, strLableId, "sfwLabel", true);
        newControl.dictAttributes.ID = strLabelID;
        newControl.dictAttributes.Text = GetCaptionFromFieldName(aobjGrid.ID) + " : ";
        newControl.dictAttributes.AssociatedControlID = aobjGrid.ControlName;
        newControl.dictAttributes.sfwIsCaption = "True";
        $rootScope.PushItem(newControl, cellVM.Elements);

        cellVM = acellLst[aintCellInd].value;
        aintCellInd++;
        newControl = {
            Name: astrControlClass, Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: []
        };
        newControl.ParentVM = cellVM;
        newControl.dictAttributes.ID = aobjGrid.ControlName;
        newControl.dictAttributes.sfwDataField = aobjGrid.ID;
        newControl.IsShowDataField = true;
        if (astrControlClass == "sfwDropDownList") {
            if (aobjGrid.ID.match("_value$")) {
                var objGrid = $scope.GetGridModel();
                if (objGrid) {
                    var strCDOName = objGrid.dictAttributes.sfwBaseQuery;
                    if (strCDOName) {
                        strCDOName = strCDOName.substring(0, strCDOName.indexOf('.'));
                    }
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var strCodeGroup = GetCodeID(strCDOName, aobjGrid.strCaption, entityIntellisenseList);
                    //if (strCodeGroup.length == 0) {
                    //    strCodeGroup = "0";
                    //}
                    newControl.dictAttributes.sfwLoadType = "CodeGroup";
                    newControl.dictAttributes.sfwLoadSource = strCodeGroup;
                }
            }
        }
        $rootScope.PushItem(newControl, cellVM.Elements);
        return aintCellInd;
    };

    //#endregion

    //#region Finish Method for Open,Close,Finish and New Dialog Wizard pages
    $scope.CreatePopupButton = function () {
        var strMethodName = $scope.sfxControl.Name + "." + $scope.SelectedButtonDetails.Method;
        if ($scope.objNewButtonDialogForGrid.SelectGridOption == "CreateNewGrid") {
            $scope.UpdateGridPropertiesForPopUp($scope.objGridField.FieldCollection, $scope.objGridView);
            $scope.UpdateGridFields($scope.objGridField.FieldCollection);
            $scope.AddButtonOpenDialog(true, strMethodName);
        }
        else if ($scope.objNewButtonDialogForGrid.SelectGridOption == "UseExistingGrid") {
            $scope.AddButtonOpenDialog(false, strMethodName);
        }
    };

    $scope.AddButtonOpenDialog = function (CreateNewGrid, strMethodName) {
        // First add the button control
        var rowCell;
        var sfwControlVM;
        var tableVM = FindParent($scope.sfxCell, "sfwTable");
        var rowVM;
        var lst = tableVM.Elements.filter(function (x) {
            return x.Name == "sfwRow";
        });
        if (lst && lst.length > 0) {
            rowVM = lst[lst.length - 1];
        }
        if (CreateNewGrid) {
            if (rowVM) {
                rowCell = undefined;
                lst = rowVM.Elements.filter(function (x) {
                    return x.Name == "sfwColumn";
                });
                if (lst && lst.length > 0) {
                    rowCell = lst[0];
                }
                if (rowCell != null) {
                    strRelatedControl = $scope.CrateGridForOpenDialogVM(rowCell);
                    if (strRelatedControl != undefined && strRelatedControl != "") {
                        $scope.AddControlToCell(strMethodName, $scope.sfxCell);
                        sfwControlVM = $scope.sfxCell.Elements[$scope.sfxCell.Elements.length - 1];
                        if (sfwControlVM != null) {
                            sfwControlVM.dictAttributes.sfwRelatedControl = strRelatedControl;
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        return;
                    }
                }
            }
        }
        else {
            $scope.AddControlToCell(strMethodName, $scope.sfxCell);
            sfwControlVM = $scope.sfxCell.Elements[$scope.sfxCell.Elements.length - 1];
            if (sfwControlVM) {
                sfwControlVM.dictAttributes.sfwRelatedControl = $scope.objNewButtonDialogForGrid.SelectGrid;
            }
            else {
                return;
            }
        }

        // After Adding Button add new Row to add Dialog Panel
        var lst = tableVM.Elements.filter(function (x) {
            return x.Name == "sfwRow";
        });
        if (lst && lst.length > 0) {
            rowVM = lst[lst.length - 1];
        }
        if (rowVM != null) {
            lst = rowVM.Elements.filter(function (x) {
                return x.Name == "sfwColumn";
            });
            if (lst && lst.length > 0) {
                rowCell = lst[0];
            }
        }

        $scope.OnInsertRowBelowClick(rowCell, tableVM);
        var rowVM1;
        var lst = tableVM.Elements.filter(function (x) {
            return x.Name == "sfwRow";
        });
        if (lst && lst.length > 0) {
            rowVM1 = lst[lst.length - 1];
        }
        if (rowVM1) {
            lst = rowVM1.Elements.filter(function (x) {
                return x.Name == "sfwColumn";
            });
            if (lst && lst.length > 0) {
                rowCell = lst[0];
            }
            if (rowCell) {
                var strNewDialogID = $scope.objNewButtonDialogForGrid.SelectDialogPanel;
                if ($scope.objNewButtonDialogForGrid.SelectDialogPanelOption == "CreateNewDialogPanel") {
                    strNewDialogID = $scope.AddDialogVM($scope.objNewButtonDialogForGrid.SelectDialogPanel, rowCell);
                }
                if (strNewDialogID != undefined && strNewDialogID != "") {
                    sfwControlVM.dictAttributes.sfwRelatedDialogPanel = strNewDialogID;
                }
            }
        }
        else {
            // Means we are not able to create new row;
        }
    };

    $scope.AddControlToCell = function (cntrlName, cellVM, sfxControlModel) {
        if (!sfxControlModel) {
            sfxControlModel = CreateControl($scope.formodel, cellVM, cntrlName);
        }

        if (sfxControlModel) {
            $rootScope.PushItem(sfxControlModel, cellVM.Elements);
            SetFormSelectedControl($scope.formodel, sfxControlModel);
        }
        if ($scope.objNewButtonFilterGrid.SelectGridOption == 'CreateNewGrid') {

            var objGridView = { Name: "sfwGridView", Value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [] };
            var strControlID = CreateControlID($scope.formodel, 'grvGridView', "sfwGridView");
            objGridView.dictAttributes.ID = strControlID;
            objGridView.dictAttributes.AllowPaging = "True";
            objGridView.dictAttributes.AllowSorting = "True";
            objGridView.dictAttributes.sfwSelection = "Many";
            objGridView.dictAttributes.sfwBoundToQuery = "True";
            objGridView.dictAttributes.sfwBaseQuery = $scope.objNewButtonFilterGrid.StrBaseQuery;
            $scope.sfxControl.dictAttributes.sfwRelatedControl = objGridView.dictAttributes.ID;
            $scope.UpdateResultFields(objGridView);
            $rootScope.PushItem(objGridView, cellVM.Elements);
            $scope.objNewButtonFilterGrid.UpdateQueryParam();

        }
    };

    $scope.CrateGridForOpenDialogVM = function (rowCell) {
        var strGridID = "";
        var selectedField = $scope.objCreateNewGrid.SelectedEntityField;

        if (selectedField) {
            if (selectedField.DataType == "Collection" || selectedField.DataType == "CDOCollection" || selectedField.DataType == "List") {
                {
                    if (rowCell) {
                        {
                            var tableVM = FindParent(rowCell, "sfwTable");
                            if (tableVM) {
                                var newRowModel = {
                                    Name: "sfwRow", Value: '', prefix: 'swc', dictAttributes: {
                                    }, Elements: [], Children: []
                                };
                                newRowModel.ParentVM = tableVM;

                                var newcellModel = {
                                    Name: "sfwColumn", Value: '', prefix: 'swc', dictAttributes: {
                                    }, Elements: [], Children: []
                                };
                                newcellModel.ParentVM = newRowModel;
                                $rootScope.PushItem($scope.objGridView, newcellModel.Elements);
                                $scope.objGridView.ParentVM = newcellModel;
                                strGridID = $scope.objGridView.dictAttributes.ID;
                                $rootScope.PushItem(newcellModel, newRowModel.Elements);
                                var rowVM;
                                var lst = tableVM.Elements.filter(function (x) {
                                    return x.Name == "sfwRow";
                                });
                                if (lst && lst.length > 0) {
                                    rowVM = lst[lst.length - 1];
                                }
                                if (rowVM) {
                                    for (var ind = 1; ind < rowVM.Elements.length; ind++) {
                                        newcellModel = {
                                            Name: "sfwColumn", Value: '', prefix: 'swc', dictAttributes: {
                                            }, Elements: [], Children: []
                                        };
                                        newcellModel.ParentVM = newRowModel;
                                        $rootScope.PushItem(newcellModel, newRowModel.Elements);
                                    }
                                }
                                $rootScope.PushItem(newRowModel, tableVM.Elements);
                            }
                        }
                    }
                }
            }
            else {
                $SgMessagesService.Message('Message', "A valid collection needs to be selected for the grid.");
            }
        }
        return strGridID;
    };

    $scope.OnInsertRowBelowClick = function (aParam, tableVM) {
        // this.ClearSelection();
        var iRowIndex = tableVM.Elements.indexOf(aParam.ParentVM);

        $scope.InsertRow(aParam, true, iRowIndex, tableVM);

    };

    $scope.InsertRow = function (aParam, isBelow, iRowIndex, tableVM) {
        if (aParam) {
            //  BaseUndoRedoVM selectedControlVM = aParam as BaseUndoRedoVM;

            // bool isValid = this.CanValidForInsert();

            // SfxRowVM rowVM = UtilityFunctions.GetVM<SfxRowVM>(selectedControlVM);
            // if (null != rowVM && isValid)
            {
                var prefix = "swc";
                var sfxRowModel = {
                    Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                sfxRowModel.ParentVM = tableVM;

                var ColCount = GetMaxColCount(aParam.ParentVM, tableVM);
                for (var colInd = 0; colInd < ColCount; colInd++) {
                    var sfxCellModel = {
                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {
                        }, Elements: [], Children: []
                    };
                    sfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(sfxCellModel);
                }

                var index;
                if (isBelow) {
                    index = iRowIndex + 1;
                }
                else {
                    index = iRowIndex;
                }

                $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);

                //BaseUndoRedoVM newRowVM = this.MirrorElements[index];
                //if (newRowVM.Elements.Count > 0)
                //{
                //    if (newRowVM.MirrorElements[0] is SfxControlVM)
                //    {
                //        this.ObjVM.DesignVM.CheckAndUpdateSelectedControlStatus(newRowVM.MirrorElements[0] as SfxControlVM, false);
                //    }
                //}
            }
        }
    };

    $scope.AddDialogVM = function (astrDialogPanelID, cellVM) {
        var strNewDialogID = "";

        $scope.AddControlToCell("DialogPanel", cellVM);
        var dialog = cellVM.Elements[cellVM.Elements.length - 1];
        if (astrDialogPanelID != undefined && astrDialogPanelID != "") {
            dialog.dictAttributes.ID = astrDialogPanelID;
        }
        if (dialog)
            strNewDialogID = dialog.dictAttributes.ID;
        return strNewDialogID;
    };
    $scope.UpdateGridPropertiesForPopUp = function (FieldCollection, objGridView) {
        var strKeySeq;
        var strKeyNames = "";

        var strSortSeq;
        var strSortExpression = "";

        if (FieldCollection != null && FieldCollection.length > 0) {
            angular.forEach(FieldCollection, function (objSfxField) {
                //string strPropertyName = objSfxField.istrItemPath.Substring(objSfxField.istrObjectID.Length + 1);
                strKeySeq = objSfxField.istrKey;
                if (strKeySeq != undefined && strKeySeq != "") {
                    if (strKeyNames.length == 0) {
                        strKeyNames = strKeySeq + ";" + objSfxField.istrPropertyName;
                    }
                    else {
                        strKeyNames += "," + strKeySeq + ";" + objSfxField.istrPropertyName;
                    }
                }

                strSortSeq = objSfxField.istrSort;
                if (strSortSeq != undefined && strSortSeq != "") {
                    if (strSortExpression.length == 0) {
                        strSortExpression = strSortSeq + ";" + objSfxField.istrPropertyName + " " + objSfxField.istrOrder;
                    }
                    else {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.istrPropertyName + " " + objSfxField.istrOrder;
                    }
                }
            });
        }
        var slFieldSeq = {};
        var strDataKeyNames = strKeyNames.split(',');

        angular.forEach(strDataKeyNames, function (strKeyName) {
            var strKeyField = strKeyName.substring(strKeyName.indexOf(';') + 1).trim();
            slFieldSeq[strKeyName] = strKeyField;
        });
        strKeyNames = "";

        angular.forEach(slFieldSeq, function (value, key) {
            if (strKeyNames.length == 0) {
                strKeyNames = value;
            }
            else {
                strKeyNames += "," + value;
            }
        });
        objGridView.dictAttributes.sfwDataKeyNames = strKeyNames;

        slFieldSeq = {
        };
        var strSortExpFields = strSortExpression.split(',');

        angular.forEach(strSortExpFields, function (strSortExp) {
            var strSortField = strSortExp.substring(strSortExp.indexOf(';') + 1).trim();
            slFieldSeq[strSortExp] = strSortField;
        });
        strSortExpression = "";

        angular.forEach(slFieldSeq, function (value, key) {
            if (strSortExpression.length == 0)
                strSortExpression = value;
            else
                strSortExpression += "," + value;
        });
        objGridView.dictAttributes.sfwSortExpression = strSortExpression;
    };

    $scope.UpdateGridProperties = function (FieldCollection, objGridView) {
        var strKeySeq;
        var strKeyNames = "";

        var strSortSeq;
        var strSortExpression = "";

        if (FieldCollection != null && FieldCollection.length > 0) {
            angular.forEach(FieldCollection, function (objSfxField) {
                //string strPropertyName = objSfxField.istrItemPath.Substring(objSfxField.istrObjectID.Length + 1);
                strKeySeq = objSfxField.istrKey;
                if (strKeySeq != undefined && strKeySeq != "") {
                    if (strKeyNames.length == 0) {
                        strKeyNames = strKeySeq + ";" + objSfxField.ID;
                    }
                    else {
                        strKeyNames += "," + strKeySeq + ";" + objSfxField.ID;
                    }
                }

                strSortSeq = objSfxField.strSort;
                if (strSortSeq) {
                    if (strSortExpression.length == 0) {
                        strSortExpression = strSortSeq + ";" + objSfxField.ID + " " + objSfxField.strOrder;
                    }
                    else {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.ID + " " + objSfxField.strOrder;
                    }
                }
            });
        }
        var slFieldSeq = {};
        var strDataKeyNames = strKeyNames.split(',');

        angular.forEach(strDataKeyNames, function (strKeyName) {
            var strKeyField = strKeyName.substring(strKeyName.indexOf(';') + 1).trim();
            slFieldSeq[strKeyName] = strKeyField;
        });
        strKeyNames = "";

        angular.forEach(slFieldSeq, function (value, key) {
            if (strKeyNames.length == 0) {
                strKeyNames = value;
            }
            else {
                strKeyNames += "," + value;
            }
        });
        objGridView.dictAttributes.sfwDataKeyNames = strKeyNames;

        slFieldSeq = {
        };
        var strSortExpFields = strSortExpression.split(',');

        angular.forEach(strSortExpFields, function (strSortExp) {
            var strSortField = strSortExp.substring(strSortExp.indexOf(';') + 1).trim();
            slFieldSeq[strSortExp] = strSortField;
        });
        strSortExpression = "";

        angular.forEach(slFieldSeq, function (value, key) {
            if (strSortExpression.length == 0)
                strSortExpression = value;
            else
                strSortExpression += "," + value;
        });
        objGridView.dictAttributes.sfwSortExpression = strSortExpression;
    };

    $scope.UpdateGridFields = function (FieldCollection) {
        $scope.objGridView.Elements = [];

        if (FieldCollection && FieldCollection.length > 0) {
            var sfxControl;
            var objColumn = {
                Name: "Columns", Value: '', dictAttributes: {
                }, Elements: [], Children: []
            };
            objColumn.ParentVM = $scope.objGridView;

            angular.forEach(FieldCollection, function (objSfxField) {
                var strDataType = objSfxField.istrDataType;
                var strHAlign = "";
                if (strDataType == "Decimal")
                    strHAlign = "Right";

                objTemp = { Name: "TemplateField", Value: '', prefix: 'asp', dictAttributes: {}, Elements: [], Children: [] };
                objTemp.ParentVM = objColumn;

                objTemp.dictAttributes.Visible = objSfxField.istrVisible;
                objTemp.dictAttributes.HeaderText = objSfxField.istrHeader;
                objTemp.dictAttributes["ItemStyle.HorizontalAlign"] = strHAlign;

                var objItmTemp = {
                    Name: "ItemTemplate", Value: '', prefix: 'asp', dictAttributes: {
                    }, Elements: [], Children: []
                };
                objItmTemp.ParentVM = objTemp;
                switch (objSfxField.istrControlType) {
                    case "HyperLink":
                        sfxControl = {
                            Name: "sfwLabel", Value: '', prefix: 'swc', dictAttributes: {
                            }, Elements: [], Children: []
                        };
                        sfxControl.ParentVM = objItmTemp;
                        //sfxControl.SetName(ApplicationConstants.XMLFacade.SFWLABEL, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        sfxControl.dictAttributes.sfwLinkable = "True";
                        sfxControl.dictAttributes.sfwRelatedControl = "btnOpen";
                        break;
                    case "TextBox":
                        sfxControl = {
                            Name: "sfwTextBox", Value: '', prefix: 'swc', dictAttributes: {
                            }, Elements: [], Children: []
                        };
                        sfxControl.ParentVM = objItmTemp;
                        // sfxControl.SetName(ApplicationConstants.XMLFacade.SFWTEXTBOX, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        if (strDataType == "DateTime")
                            sfxControl.dictAttributes.sfwDataType = strDataType;
                        break;
                    case "DropDownList":
                        sfxControl = {
                            Name: "sfwDropDownList", Value: '', prefix: 'swc', dictAttributes: {
                            }, Elements: [], Children: []
                        };
                        sfxControl.ParentVM = objItmTemp;
                        // sfxControl.SetName(ApplicationConstants.XMLFacade.SFWDROPDOWNLIST, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        if (endsWith(objSfxField.istrObjectID, "_value")) {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            var codegroup = GetCodeID($scope.objCreateNewGrid.SelectedEntityField.Entity, objSfxField.istrItemPath, entityIntellisenseList);
                            //var codegroup = UtilityFunctions.GetCodeID(objSfxField.istrItemPath);

                            if (codegroup.length == 0) {
                                codegroup = "0";
                            }
                            if (codegroup != undefined && codegroup != "") {
                                sfxControl.dictAttributes.sfwLoadType = "CodeGroup";
                            }
                        }
                        break;
                    case "Checkbox":
                        sfxControl = {
                            Name: "sfwCheckBox", Value: '', prefix: 'swc', dictAttributes: {
                            }, Elements: [], Children: []
                        };
                        sfxControl.ParentVM = objItmTemp;
                        //sfxControl.SetName(ApplicationConstants.XMLFacade.SFWCHECKBOX, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        break;
                    default:
                        sfxControl = {
                            Name: "sfwLabel", Value: '', prefix: 'swc', dictAttributes: {
                            }, Elements: [], Children: []
                        };
                        sfxControl.ParentVM = objItmTemp;
                        //sfxControl.SetName(ApplicationConstants.XMLFacade.SFWLABEL, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        break;
                }

                sfxControl.dictAttributes.sfwEntityField = objSfxField.istrItemPath;

                objTemp.dictAttributes.sfwSortExpression = sfxControl.dictAttributes.sfwEntityField;

                sfxControl.dictAttributes.sfwDataFormat = objSfxField.istrFormat;
                sfxControl.dictAttributes.sfwRelatedControl = objSfxField.istrRelatedControl;

                if (sfxControl.Name != "sfwLabel" && sfxControl.Name != "sfwButton" && sfxControl.Name != "sfwLinkButton" && sfxControl.Name != "sfwImageButton" && sfxControl.Name !== "sfwButtonGroup") {
                    $scope.objGridView.dictAttributes.AllowEditing = "True";
                    $scope.objGridView.dictAttributes.sfwTwoWayBinding = "True";
                    $scope.objGridView.dictAttributes.sfwCommonFilterBox = "False";
                    $scope.objGridView.dictAttributes.sfwFilterOnKeyPress = "False";
                }

                $rootScope.PushItem(sfxControl, objItmTemp.Elements);
                $rootScope.PushItem(objItmTemp, objTemp.Elements);
                $rootScope.PushItem(objTemp, objColumn.Elements);

            });

            $rootScope.PushItem(objColumn, $scope.objGridView.Elements);

        }
    };
    //#endregion


    $scope.onNextForNewGridCreation = function () {
        $scope.objFilterGridFieldsDetails.Init();
    }
    //#endregion


    $scope.onFinishButtonWizardClick = function () {
        $scope.$emit('AddButtonCancel', {
        });
    };

    //#region Call Init Methods
    $scope.Init();
    //#endregion

    $scope.setTitle = function (title) {
        if ($scope.$parent.title) {
            $scope.$parent.title = title;
            $scope.$parent.ButtonWizardDialog.updateTitle($scope.$parent.title);
        }
    };

    $scope.validateId = function (obj) {
        if ($scope.currentFileScope) {
            $ValidationService.validateID(obj, $scope.currentFileScope.validationErrorList, obj.dictAttributes.ID);
        } else {
            $scope.currentFileScope = $scope.getFileScope();
            $ValidationService.validateID(obj, $scope.currentFileScope.validationErrorList, obj.dictAttributes.ID);
        }
    };
    $scope.validateDuplicateId = function (obj) {
        if ($scope.currentFileScope) {
            $ValidationService.checkDuplicateId(obj, $scope.currentFileScope.formTableModel, $scope.currentFileScope.validationErrorList, false, CONST.FORM.NODES);
        } else {
            $scope.currentFileScope = $scope.getFileScope();
            $ValidationService.checkDuplicateId(obj, $scope.currentFileScope.formTableModel, $scope.currentFileScope.validationErrorList, false, CONST.FORM.NODES);
        }
    };
    $scope.isAnyErrors = function () {
        var returnValue = false;
        returnValue = $ValidationService.isEmptyObj($scope.sfxControl.errors);
        return returnValue;
    };
    $scope.getFileScope = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                return scope;
            } else {
                return undefined;
            }
        }
    };


    $scope.onChangeOfSelectGridOption = function () {
        $scope.sfxControl.dictAttributes.sfwRelatedControl = '';
        $scope.objNewButtonFilterGrid.StrBaseQuery = '';
        $scope.objNewButtonFilterGrid.lstQryField = [];
    }
    $scope.OnChangeOfGridOption = function () {
        {
            $scope.objNewButtonDialogForGrid.SelectGrid = '';
        }
    }

}]);

function onTrackingIDDrop(e) {
    var scp = angular.element($(e.target)).scope();
    var parent = scp.$parent;
    while (parent && !parent.objNewButtonOpenWordDoc.SelectedField) {
        parent = parent.$parent;
    }

    if (scp && scp.$parent && parent.objNewButtonOpenWordDoc.SelectedField) {
        var field = parent.objNewButtonOpenWordDoc.SelectedField;
        var value = field.ID;
        while (field.objParent && !field.objParent.IsMainEntity) {
            field = field.objParent;
            value = [field.ID, value].join(".");
        }

        scp.$apply(function () {
            scp.objNewButtonOpenWordDoc.TrackingID = value;
        });
    }
}

function onFieldCollectionDrop(event) {
    event.preventDefault();
    var scope = angular.element(event.target).scope();
    if (scope && scope.objNewButtonCustomAttr && scope.objNewButtonCustomAttr.AddToGridClick) {
        scope.$apply(function () {
            scope.objNewButtonCustomAttr.AddToGridClick();
        });
    }
}

function TraverseFieldCollection(objColl) {
    angular.forEach(objColl.Items, function (itemObj, key) {
        if (itemObj.Items.length > 0) {
            TraverseFieldCollection(itemObj);
            itemObj.IsSelected = false;
        } else {
            itemObj.IsSelected = false;
        }
    });
}