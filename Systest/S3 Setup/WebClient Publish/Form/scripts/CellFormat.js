app.controller("CellFormatController", ["$scope", "$rootScope", "$filter", "$EntityIntellisenseFactory", "$GetEntityFieldObjectService", function ($scope, $rootScope, $filter, $EntityIntellisenseFactory, $GetEntityFieldObjectService) {
    $scope.lstColumnName = [];
    if ($scope.IsCellOrRow == "CellFormat") {
        PopulateGridDataField($scope.TemplateFieldModel.ParentVM, $scope.lstColumnName);
    }
    else {
        PopulateGridDataField($scope.TemplateFieldModel, $scope.lstColumnName);
    }

    if ($scope.lstColumnName && $scope.lstColumnName.length > 0) {
        $scope.lstColumnName.splice(0, 0, "");
    }

    $scope.Itemtemplate = {};
    $scope.isDropDown = false;
    $scope.checkAndGetCodeValues = function (sfwEntityField) {
        var columnName = "";
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        if ($scope.isLookup) {
            var lst = $filter('filter')(entityIntellisenseList, { ID: $scope.formEntity }, true);
            if (lst && lst.length > 0) {
                if (lst[0].Attributes.length > 0) {
                    var lstAttribute = $filter('filter')(lst[0].Attributes, { ID: sfwEntityField }, true);
                    if (lstAttribute.length > 0)
                        columnName = lstAttribute[0].Value;
                }
            }
            if (columnName.lastIndexOf('_value') == columnName.length - '_value'.length) {
                $scope.isDropDown = true;
                $scope.CurrentCodeID = GetCodeID($scope.formEntity, sfwEntityField, entityIntellisenseList);
            } else {
                $scope.isDropDown = false;
            }
        }
        else {

            var lstFormEntity = $filter('filter')(entityIntellisenseList, { ID: $scope.formEntity }, true);
            if (lstFormEntity) {
                var gridObject = GetVM("sfwGridView", $scope.TemplateFieldModel);
                var EntityField;
                if (gridObject) {
                    EntityField = gridObject.dictAttributes.sfwEntityField;
                } else {
                    var listObject = GetVM("sfwListView", $scope.TemplateFieldModel);
                    EntityField = listObject.dictAttributes.sfwEntityField;
                }

                if (EntityField && $scope.formEntity) {
                    // Getting Grid EntityName
                    var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField($scope.formEntity, EntityField);
                    var entityname = "";
                    if (object) {
                        entityname = object.Entity;
                    }
                    if (entityname) {
                        //Getting Grid Entity Object
                        var objectField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityname, sfwEntityField);
                        var columnName = "";
                        if (objectField) {
                            columnName = objectField.Value;
                        }
                        if (columnName && columnName.lastIndexOf('_value') == columnName.length - '_value'.length) {
                            $scope.isDropDown = true;
                            $scope.CurrentCodeID = GetCodeID(entityname, sfwEntityField, entityIntellisenseList);
                        } else {
                            $scope.isDropDown = false;
                        }
                    }
                }
            }
        }
    };

    if ($scope.IsCellOrRow == "CellFormat") {
        for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
            if ($scope.TemplateFieldModel.Elements[i].Name == "ItemTemplate") {
                angular.copy($scope.TemplateFieldModel.Elements[i], $scope.Itemtemplate);
                break;
            }
        }

        for (var i = 0; i < $scope.Itemtemplate.Elements.length; i++) {
            if ($scope.Itemtemplate.Elements[i].Name == "cellformat") {
                $scope.cellFormat = $scope.Itemtemplate.Elements[i];
                if ($scope.cellFormat.dictAttributes.sfwEntityField) {
                    $scope.checkAndGetCodeValues($scope.cellFormat.dictAttributes.sfwEntityField);
                }
                break;
            }
        }
    }
    else {
        for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
            if ($scope.TemplateFieldModel.Elements[i].Name == "rowformat") {
                // $scope.cellFormat = $scope.TemplateFieldModel.Elements[i];
                $scope.cellFormat = {};
                angular.copy($scope.TemplateFieldModel.Elements[i], $scope.cellFormat);
                $scope.tempRowFormat = {};
                angular.copy($scope.TemplateFieldModel.Elements[i], $scope.tempRowFormat);
                if ($scope.cellFormat.dictAttributes.sfwEntityField) {
                    $scope.checkAndGetCodeValues($scope.cellFormat.dictAttributes.sfwEntityField);
                }
                break;
            }
        }
    }

    $scope.setRowSelection = function (condition) {
        $scope.selectedCssClassCondition = condition;
    };
    $scope.AddNewCssCondition = function (columnName) {
        if ($scope.cellFormat && $scope.cellFormat.Elements != undefined) {
            var obj = { dictAttributes: { value: "", cssclass: "" }, Elements: [], Children: [], Name: "condition", Value: "", };
            $rootScope.PushItem(obj, $scope.cellFormat.Elements[0].Elements, null);
        }
        $scope.checkAndGetCodeValues(columnName);
    };

    $scope.onCondtionSaveClick = function (conditions) {
        if (conditions != undefined && conditions != "" && conditions.Elements && conditions.Elements.length > 0) {
            if (conditions.Elements[0].Elements && conditions.Elements[0].Elements.length == 0) {
                conditions.dictAttributes.sfwEntityField = "";
            }
            var flag = false;
            if ($scope.IsCellOrRow == "CellFormat") {
                for (var i = 0; i < $scope.Itemtemplate.Elements.length; i++) {
                    if ($scope.Itemtemplate.Elements[i].Name == "cellformat") {
                        if ($scope.cellFormat && $scope.cellFormat.dictAttributes.sfwEntityField) {
                            $scope.Itemtemplate.Elements[i] = $scope.cellFormat;
                        }
                        else {
                            $scope.Itemtemplate.Elements.splice(i, 1);
                        }
                        flag = true;
                        break;
                    }
                }
            }
            else {
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "rowformat") {
                        if ($scope.cellFormat && $scope.cellFormat.dictAttributes.sfwEntityField) {
                            $scope.TemplateFieldModel.Elements[i] = $scope.cellFormat;
                        }
                        else {
                            $scope.TemplateFieldModel.Elements.splice(i, 1);
                        }
                        flag = true;
                        break;
                    }
                }
            }
            if ($scope.IsCellOrRow == "CellFormat") {
                if (flag == false) {
                    $scope.Itemtemplate.Elements.push(conditions);
                }
                //$scope.TemplateFieldModel.Elements = [];
                //$scope.TemplateFieldModel.Elements.push($scope.Itemtemplate);
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "ItemTemplate") {
                        $scope.TemplateFieldModel.Elements.splice(i, 1);
                        $scope.TemplateFieldModel.Elements.splice(i, 0, $scope.Itemtemplate);
                        break;
                    }
                }
            }
            else if (flag == false) {
                $scope.TemplateFieldModel.Elements.push($scope.cellFormat);
            }
        }
        else {
            if ($scope.TemplateFieldModel && $scope.TemplateFieldModel.Elements) {
                var index = -1;
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == $scope.IsCellOrRow.toLowerCase()) {
                        index = i;
                    }
                }
                if (index > -1) {
                    $scope.TemplateFieldModel.Elements.splice(index, 1);
                }
            }
        }
        $scope.CellFormatDialog.close();
    };
    $scope.onCancelClick = function () {
        if ($scope.IsCellOrRow != "CellFormat") {
            for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                if ($scope.TemplateFieldModel.Elements[i].Name == "rowformat") {
                    if ($scope.cellFormat && $scope.cellFormat.dictAttributes.sfwEntityField && $scope.tempRowFormat) {
                        $scope.TemplateFieldModel.Elements[i] = $scope.tempRowFormat;
                    }
                    else {
                        $scope.TemplateFieldModel.Elements.splice(i, 1);
                    }
                    flag = true;
                    break;
                }
            }
        }
        $scope.CellFormatDialog.close();
    };
    $scope.deleteCssCondition = function () {
        if ($scope.cellFormat.Elements[0].Elements.length > 0) {
            if ($scope.selectedCssClassCondition != undefined) {
                var index = $scope.cellFormat.Elements[0].Elements.indexOf($scope.selectedCssClassCondition);
                //$scope.cellFormat.Elements[0].Elements.splice(index, 1);
                if (index > -1) {
                    $rootScope.DeleteItem($scope.selectedCssClassCondition, $scope.cellFormat.Elements[0].Elements);
                }
            }
            $scope.selectedCssClassCondition = undefined;
        }
        else {
            //$scope.cellFormat.Elements[0].Elements = [];
            $scope.selectedCssClassCondition = undefined;
        }
    };
    $scope.clearCellFormat = function () {
        if ($scope.cellFormat != undefined && $scope.cellFormat.Elements && $scope.cellFormat.Elements.length > 0) {
            $scope.cellFormat.Elements[0].Elements = [];
        }
        else {
            $scope.cellFormat = undefined;
        }
    };

    $scope.clearRoworCellFormat = function () {
        var obj = {
            dictAttributes: { sfwEntityField: $scope.cellFormat.dictAttributes.sfwEntityField },
            Elements: [{ dictAttributes: {}, Elements: [], Children: [], Name: "Conditions", Value: "" }], Children: [], Name: "rowformat", Value: ""
        };

        if ($scope.IsCellOrRow == "CellFormat") {
            obj.Name = "cellformat";
        }

        if ($scope.cellFormat && $scope.cellFormat.Elements) {
            if ($scope.IsCellOrRow == "CellFormat") {
                for (var i = 0; i < $scope.Itemtemplate.Elements.length; i++) {
                    if ($scope.Itemtemplate.Elements[i].Name == "cellformat") {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem($scope.cellFormat, $scope.Itemtemplate.Elements);
                        $scope.cellFormat = obj;
                        $rootScope.PushItem($scope.cellFormat, $scope.Itemtemplate.Elements, null);
                        $rootScope.UndRedoBulkOp("End");
                        break;
                    }
                }
            }
            else {
                for (var i = 0; i < $scope.TemplateFieldModel.Elements.length; i++) {
                    if ($scope.TemplateFieldModel.Elements[i].Name == "rowformat") {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem($scope.cellFormat, $scope.TemplateFieldModel.Elements);
                        $scope.cellFormat = obj;
                        $rootScope.PushItem($scope.cellFormat, $scope.TemplateFieldModel.Elements, null);
                        $rootScope.UndRedoBulkOp("End");
                        break;
                    }
                }
            }
        } else {
            $scope.cellFormat = obj;
            if ($scope.IsCellOrRow == "CellFormat") {
                $rootScope.PushItem($scope.cellFormat, $scope.Itemtemplate.Elements, null);
            } else {
                $rootScope.PushItem($scope.cellFormat, $scope.TemplateFieldModel.Elements, null);
            }
        }
        $scope.checkAndGetCodeValues($scope.cellFormat.dictAttributes.sfwEntityField);
        //$scope.cellFormat = obj;
    };
}]);