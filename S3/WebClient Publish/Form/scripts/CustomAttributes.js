app.controller("CustomAttributesController", ["$scope", "$rootScope", "$SgMessagesService", function ($scope, $rootScope, $SgMessagesService) {

    $scope.IsShowAllControl = false;

    //#region Init Methods
    $scope.Init = function () {
        $scope.FieldCollection = [];
        $scope.ParameterCollection = [];
        $scope.IsLookup = $scope.formobject.dictAttributes.sfwType == 'Lookup' || $scope.formobject.dictAttributes.sfwType == 'FormLinkLookup';
        $scope.IsReport = $scope.formobject.dictAttributes.sfwType == 'Report';
        $scope.IsCorrespondence = $scope.formobject.dictAttributes.sfwType == 'Correspondence';
        $scope.LoadAvailableFields();
        $scope.Initialize();
    };

    $scope.Initialize = function () {
        if ($scope.propertyName == undefined) {
            $scope.propertyName = $scope.$parent.propertyName;
        }

        var customAttribute = $scope.model.dictAttributes[$scope.propertyName];
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
                    var objParameter = { ParameterField: strParamField, ParameterValue: strParamValue, Constants: blnConstant };

                    $scope.ParameterCollection.push(objParameter);
                }
            });
        }
    };

    $scope.LoadAvailableFields = function () {
        var strProperty = "";
        if ($scope.IsLookup) {
            strProperty = "sfwDataField";
        }
        else if ($scope.IsReport) {
            strProperty = "sfwObjectField";
            var table;
            for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                if ($scope.formobject.Elements[i].Name == "sfwTable") {
                    $scope.CurrentTable = $scope.formobject.Elements[i];
                    break;
                }
            }
        }
        else {
            strProperty = "sfwEntityField";
        }

        $scope.FieldCollection = [];

        var mainItem = { Text: "Main", Items: [], IsSelected: false, IsCheckBoxVisible: false };
        if (!$scope.isFormLink) {
            if ($scope.IsShowAllControl) {

                var table;
                for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                    if ($scope.formobject.Elements[i].Name == "sfwTable") {
                        table = $scope.formobject.Elements[i];
                        break;
                    }
                }

                //PopulateAvailableFields(strProperty, $rootScope.MainTable, mainItem, true);
                PopulateAvailableFields(strProperty, table, mainItem, true, $scope.IsLookup, false);
            }
            else {
                PopulateAvailableFields(strProperty, $scope.CurrentTable, mainItem, true, $scope.IsLookup, false);
            }

            if (mainItem.Items.length > 0) {
                $scope.$evalAsync(function () {
                    $scope.FieldCollection.push(mainItem);
                });
            }
        }
        else {
            var objItems = GetFormLinkItemsModel($scope.model, $scope.formobject);
            if (null != objItems) {
                PopulateAvailableFieldsForFormLink(strProperty, objItems, mainItem, true);
                if (mainItem.Items.length > 0) {
                    $scope.FieldCollection.push(mainItem);
                }
            }
        }
    };


    //#endregion

    //#region Common Events

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

    $scope.showAllControlChange = function () {
        // $scope.$apply(function() {
        $scope.LoadAvailableFields();
        //});
    };

    $scope.AddToGridClick = function () {
        if ($scope.FieldCollection.length > 0) {
            $scope.TraverseFieldCollection($scope.FieldCollection[0].Items);
            if ($scope.ParameterCollection.length > 0) {
                $scope.SelectedParameter = $scope.ParameterCollection[$scope.ParameterCollection.length - 1];
            }
        }
    };

    $scope.SelectParameter = function (param) {
        $scope.SelectedParameter = param;
    };

    $scope.SetClassForParameter = function (param) {
        if ($scope.SelectedParameter == param) {
            return "selected";
        }
    };

    $scope.onAddParameter = function () {
        $scope.ParameterCollection.push({ ParameterField: "", ParameterValue: "" });
        if ($scope.ParameterCollection.length > 0) {
            $scope.SelectedParameter = $scope.ParameterCollection[$scope.ParameterCollection.length - 1];
        }
    };

    $scope.onDeleteParameter = function () {
        if ($scope.SelectedParameter) {
            var index = $scope.ParameterCollection.indexOf($scope.SelectedParameter);
            $scope.ParameterCollection.splice(index, 1);

            if (index < $scope.ParameterCollection.length) {
                $scope.SelectedParameter = $scope.ParameterCollection[index];
            }
            else if ($scope.ParameterCollection.length > 0) {
                $scope.SelectedParameter = $scope.ParameterCollection[index - 1];
            }
            else if ($scope.ParameterCollection.length == 0) {
                $scope.SelectedParameter = undefined;
            }
        }
    };

    //#endregion 

    //#region Common Methods

    $scope.TraverseFieldCollection = function (fieldCollection) {
        angular.forEach(fieldCollection, function (field) {
            if (field.IsSelected) {
                var strFld = field.Text;
                var blnFound = false;
                var lst = $scope.ParameterCollection.filter(function (itm) { return itm.ParameterValue == strFld; });
                if (lst && lst.length > 0) {
                    blnFound = true;
                }

                if (blnFound) {
                    $SgMessagesService.Message('Message', strFld + " Column is already added in Collection, please check.");
                }
                else {
                    var strValue = $scope.GetCaptionFromFieldName(strFld);
                    strValue = $scope.RemoveInternalSpace(strValue);
                    var objParameters = { ParameterField: strValue, ParameterValue: strFld };
                    $scope.ParameterCollection.push(objParameters);
                }
                field.IsSelected = false;
            }
            if (field.Items.length > 0) {
                $scope.TraverseFieldCollection(field.Items);
            }
        });
    };

    $scope.RemoveInternalSpace = function (astrInput) {
        astrInput = astrInput.trim();
        while (astrInput.indexOf(" ") > 0) {
            astrInput = astrInput.substring(0, astrInput.indexOf(" ")).trim() + astrInput.substring(astrInput.indexOf(" ")).trim();
        }
        return astrInput;
    };

    $scope.GetCaptionFromFieldName = function (str) {
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

    $scope.GetSavedString = function () {
        var strReturn = "";
        angular.forEach($scope.ParameterCollection, function (objParams) {
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

    $scope.onOkClick = function () {
        var strCustomAttribute = $scope.GetSavedString();
        $rootScope.EditPropertyValue($scope.model.dictAttributes[$scope.propertyName], $scope.model.dictAttributes, $scope.propertyName, strCustomAttribute);
        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        // $scope.objNewDialog.close();
        $scope.UserLogParaDialog.close();
    };
    //#endregion

    $scope.Init();
}]);