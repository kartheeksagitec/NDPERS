app.controller("ActiveFormsController", ["$scope", "$rootScope", "$filter", "ngDialog", "$EntityIntellisenseFactory", "$timeout", "$GetEntityFieldObjectService", function ($scope, $rootScope, $filter, ngDialog, $EntityIntellisenseFactory, $timeout, $GetEntityFieldObjectService) {
    $scope.lstEntityField = [];
    $scope.lstrEntityField = "";
    $scope.isDropDown = false;
    $scope.IsHtxForm = false;
    var iblnIsLookup = $scope.formmodel.dictAttributes.sfwType == "Lookup";
    var iblnIsFormLinkLookup = $scope.formmodel.dictAttributes.sfwType == "FormLinkLookup";
    $scope.strParent = "";
    if ($scope.model.dictAttributes && $scope.model.dictAttributes.sfwEntityField) {
        $scope.lstrEntityField = $scope.model.dictAttributes.sfwEntityField;
    }
    if ($scope.model.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
        if (iblnIsLookup || iblnIsFormLinkLookup)
            $scope.strParent = "tblCriteria panel fields";
        else
            $scope.strParent = "Form fields";
    }
    else if ($scope.model.dictAttributes.sfwMethodName == "btnOpen_Click") {
        var strGridID = $scope.model.dictAttributes.sfwRelatedControl;
        var objGrid = FindParent($scope.model, "sfwGridView");
        if (objGrid && objGrid.dictAttributes) {
            strGridID = objGrid.dictAttributes.ID;
        }
        if (strGridID == "" || strGridID == undefined)
            $scope.strParent = "Grid fields";
        else
            $scope.strParent = strGridID + " fields";
    }

    $scope.InitializeActiveForms = function (astrActiveForms, astrObjectField, alAvlFlds, astrParentID) {
        $scope.lstEntityField = [];
        ilAvlFlds = alAvlFlds;

        if ($scope.model.dictAttributes.sfwMethodName == "btnNew_Click" || $scope.model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
            GetObjectFields($scope.formmodel, $scope.lstEntityField, $scope.model);
        }
        else if ($scope.model.dictAttributes.sfwMethodName == "btnDelete_Click") {

        }
        else if ($scope.model.dictAttributes.sfwMethodName == "btnOpen_Click") {
            var strMessage = "(Object Fields are populated from the " + astrParentID + ".)";

            $scope.lstEntityField.push("");

            if (alAvlFlds.length > 0) {
                for (var i = 0; i < alAvlFlds.length; i++) {
                    var s = alAvlFlds[i];
                    var strParamValue = "";
                    if (s.indexOf("~") > -1)
                        strParamValue = s.substring(0, s.indexOf("~"));
                    else
                        strParamValue = s;
                    if (!$scope.lstEntityField.some(function (itm) { return itm == strParamValue; })) {
                        $scope.lstEntityField.push(strParamValue);
                    }
                }
            }
        }
    };

    $scope.alControls = [];
    PopulateControlsForActiveForm($scope.alControls, $scope.formmodel, $scope.model, iblnIsLookup || iblnIsFormLinkLookup);
    $scope.InitializeActiveForms($scope.model.dictAttributes.sfwActiveForm, $scope.lstrEntityField, $scope.alControls, $scope.strParent);

    //if ($scope.model.dictAttributes.sfwRelatedControl && $scope.model.dictAttributes.sfwRelatedControl != "") {
    //    var gridObj = {};
    //    GetVMUsingID("sfwGridView", $scope.formmodel, $scope.model.dictAttributes.sfwRelatedControl, $scope.lstEntityField);
    //}
    //else {
    //    
    //    else
    //        PopulateFormEntityField($scope.formmodel, $scope.lstEntityField, iblnIsLookup);
    //}
    $scope.checkAndGetCodeValues = function () {
        var columnName = "";
        $scope.isDropDown = false;
        if ($scope.model.dictAttributes.sfwMethodName == "btnNew_Click") {
            if ($scope.model.dictAttributes.sfwRelatedControl != undefined && $scope.model.dictAttributes.sfwRelatedControl != "") {
                var objControl = FindControlByID($scope.formmodel, $scope.model.dictAttributes.sfwRelatedControl);
                if (objControl && objControl.dictAttributes && objControl.dictAttributes.sfwLoadType == "CodeGroup") {
                    if (objControl.dictAttributes.sfwLoadSource != undefined && objControl.dictAttributes.sfwLoadSource != "") {
                        $scope.isDropDown = true;
                        $scope.CurrentCodeID = objControl.dictAttributes.sfwLoadSource;
                    }
                    else if (objControl.placeHolder) {
                        $scope.isDropDown = true;
                        $scope.CurrentCodeID = objControl.placeHolder;
                    }
                }
            }
        }
        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (($scope.model.dictAttributes.sfwRelatedControl || $scope.model.IsChildOfGrid) && !iblnIsLookup && !iblnIsFormLinkLookup) {
                var lstFormEntity = $filter('filter')(entityIntellisenseList, { ID: $scope.formmodel.dictAttributes.sfwEntity }, true);
                if (lstFormEntity) {
                    var gridObject = [];
                    var strRelatedControl = $scope.model.dictAttributes.sfwRelatedControl;
                    var objGrid = FindParent($scope.model, "sfwGridView");
                    if (objGrid && objGrid.dictAttributes) {
                        strRelatedControl = objGrid.dictAttributes.ID;
                    }
                    GetGridObject("sfwGridView", $scope.formmodel, strRelatedControl, gridObject);
                    if (gridObject.length > 0 && gridObject[0].dictAttributes.sfwEntityField) {
                        var gridEntityField = gridObject[0].dictAttributes.sfwEntityField;
                        // Getting Grid EntityName
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formmodel.dictAttributes.sfwEntity, gridEntityField);
                        var strEntityName = "";
                        if (object) {
                            strEntityName = object.Entity;
                        }
                        if (strEntityName) {
                            //Getting Grid Entity Object
                            if ($scope.lstrEntityField) {
                                var objectField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strEntityName, $scope.lstrEntityField);
                                var columnName = "";
                                if (objectField) {
                                    columnName = objectField.Value;
                                }
                                if (columnName && columnName.lastIndexOf('_value') == columnName.length - '_value'.length) {
                                    $scope.isDropDown = true;
                                    $scope.CurrentCodeID = GetCodeID(strEntityName, $scope.lstrEntityField, entityIntellisenseList);
                                } else {
                                    $scope.isDropDown = false;
                                }
                            }
                        }
                    }
                }
            }
            else {
                var lst = $filter('filter')(entityIntellisenseList, { ID: $scope.formmodel.dictAttributes.sfwEntity }, true);
                if (lst && lst.length > 0) {
                    if (lst[0].Attributes.length > 0 && $scope.lstrEntityField) {
                        var lstAttribute = $filter('filter')(lst[0].Attributes, { ID: $scope.lstrEntityField }, true);
                        if (lstAttribute.length > 0)
                            columnName = lstAttribute[0].Value;
                    }
                }
                if (columnName.lastIndexOf('_value') == columnName.length - '_value'.length) {
                    $scope.isDropDown = true;
                    $scope.CurrentCodeID = GetCodeID($scope.formmodel.dictAttributes.sfwEntity, $scope.lstrEntityField, entityIntellisenseList);
                } else {
                    $scope.isDropDown = false;
                }
            }
        }
    };
    $scope.checkAndGetCodeValues();
    $scope.lstActiveForm = [];
    if ($scope.model.dictAttributes.sfwActiveForm && $scope.model.dictAttributes.sfwActiveForm.contains("=")) {
        var tempActiveForm = $scope.model.dictAttributes.sfwActiveForm.split(";");
        if (tempActiveForm) {
            for (var i = 0; i < tempActiveForm.length; i++) {
                var tempObj = tempActiveForm[i].split("=");
                $scope.lstActiveForm.push({ FieldValue: tempObj[0], ActiveForm: tempObj[1] });
            }
        }
    }
    $scope.AddActiveForm = function () {
        $scope.lstActiveForm.push({ FieldValue: "", ActiveForm: "" });

        $scope.checkAndGetCodeValues();
    };
    $scope.DeleteActiveForm = function () {
        if ($scope.selectedActiveForm && $scope.selectedActiveForm != "") {
            var index = $scope.lstActiveForm.indexOf($scope.selectedActiveForm);
            if (index > -1) {
                $scope.lstActiveForm.splice(index, 1);
            }
            else {
                $scope.selectedActiveForm = undefined;
            }
        }
    };
    $scope.selectActiveFormRow = function (form) {
        $scope.selectedActiveForm = form;
    };
    $scope.OkClick = function () {
        var activeFormString = "";
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwEntityField, $scope.model.dictAttributes, "sfwEntityField", $scope.lstrEntityField);
        if ($scope.lstActiveForm.length > 0) {
            for (var i = 0; i < $scope.lstActiveForm.length; i++) {
                if ($scope.lstActiveForm[i].FieldValue != "" && $scope.lstActiveForm[i].ActiveForm != "") {
                    if (activeFormString == "") {
                        activeFormString = $scope.lstActiveForm[i].FieldValue + "=" + $scope.lstActiveForm[i].ActiveForm;
                    }
                    else {
                        activeFormString += ";" + $scope.lstActiveForm[i].FieldValue + "=" + $scope.lstActiveForm[i].ActiveForm;
                    }
                }
            }
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwActiveForm, $scope.model.dictAttributes, "sfwActiveForm", activeFormString);
        }
        else {
            $rootScope.EditPropertyValue($scope.model.dictAttributes.sfwActiveForm, $scope.model.dictAttributes, "sfwActiveForm", "");
        }
        $rootScope.UndRedoBulkOp("End");
        //ngDialog.close($scope.ActiveForm.id);
        $timeout(function () {
            if ($scope.validateActiveForm) $scope.validateActiveForm();
        });
        $scope.ActiveFormDialog.close();
    };

    $scope.CancelClick = function () {
        $scope.ActiveFormDialog.close();
    };

    $scope.validateActiveForms = function () {
        var IsValid = false;
        $scope.ErrorMessageForDisplay = "";
        if ($scope.lstActiveForm.length == 0) {
            $scope.ErrorMessageForDisplay = "Atleast one active form has to be added.";
            return true;
        }
        if ($scope.lstActiveForm.length > 0) {
            var iActvCount = 0;
            for (var i = 0; i < $scope.lstActiveForm.length; i++) {
                var objActiveForm = $scope.lstActiveForm[i];
                if (objActiveForm.FieldValue == "ACTV") {
                    iActvCount++;
                }
                if (!objActiveForm.FieldValue) {
                    $scope.ErrorMessageForDisplay = "Please enter field value of row number " + (i + 1);
                    return true;
                }
                if (!objActiveForm.ActiveForm) {
                    $scope.ErrorMessageForDisplay = "Please enter active form value of row number " + (i + 1);
                    return true;
                }

            }
            if (iActvCount > 1) {
                $scope.ErrorMessageForDisplay = "Cannot add multiple active forms with same field value - ACTV";
                return true;
            }
            else {
                return false;
            }
        }
        return IsValid;
    };

    $scope.clearActiveFormList = function () {
        $scope.lstActiveForm = [];
    };
}]);


function GetGridObject(ControlName, sfxControl, ctrlID, lstgridObj) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name == ControlName && ctrl.dictAttributes.ID == ctrlID) {
            lstgridObj.push(ctrl);
        }
        if (ctrl.Elements.length > 0) {
            GetGridObject(ControlName, ctrl, ctrlID, lstgridObj);
        }
    });
}