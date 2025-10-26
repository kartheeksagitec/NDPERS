app.controller("RetrievalParametersController", ["$scope", "$rootScope", "$filter", function ($scope, $rootScope, $filter) {
    $scope.ShowAll = {};
    $scope.ShowAll.isShowAll = false;
    $scope.lstParameters = [];
    var panelObject;
    $scope.Init = function () {
        if ($scope.formobject) {
            var isWizard = $scope.formobject.dictAttributes.sfwType == "Wizard";
            if (isWizard) {
                panelObject = GetVM("sfwWizardStep", $scope.model);
            } else if ($scope.formobject.dictAttributes.sfwType == "FormLinkLookup" || $scope.formobject.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.formobject.dictAttributes.sfwType == "FormLinkWizard") {
                if ($scope.formobject && $scope.formobject.Elements) {
                    for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                        if ($scope.formobject.Elements[i].Name == "items") {
                            $scope.PopulateRetrievalParametersForHtml($scope.formobject.Elements[i], $scope.lstParameters, $scope.formobject.dictAttributes.sfwType);
                            break;
                        }
                    }
                }
            }
            else if ($scope.formobject.dictAttributes.sfwType == "Correspondence" || $scope.formobject.dictAttributes.sfwType == "UserControl") {
                panelObject = GetVM("sfwTable", $scope.model);
            }
            else {
                panelObject = GetVM("sfwPanel", $scope.model); // For Forms
            }
            if (panelObject) {
                PopulateRetrievalOrAutoCompleteParameters(panelObject, $scope.lstParameters, $scope.formobject.dictAttributes.sfwType, true);
            }
        }
        $.connection.hubForm.server.getGlobleParameters().done(function (data) {
            $scope.$apply(function () {
                $scope.objGlobleParameters = data;
                $scope.PopulateGlobalParameters();
            });
        });
    };

    $scope.ShowAllControls = function () {
        $scope.lstParameters = [];
        if ($scope.ShowAll.isShowAll) {
            if ($scope.formobject && $scope.formobject.Elements) {
                for (var i = 0; i < $scope.formobject.Elements.length; i++) {
                    if ($scope.formobject.dictAttributes.sfwType == "Correspondence") {
                        if ($scope.formobject.Elements[i].Name == "sfwQueryForm") {
                            if ($scope.formobject.Elements[i].Elements.length > 0 && $scope.formobject.Elements[i].Elements[0].Name == "sfwTable") {
                                PopulateRetrievalOrAutoCompleteParameters($scope.formobject.Elements[i].Elements[0], $scope.lstParameters, $scope.formobject.dictAttributes.sfwType, true);
                                break;
                            }
                        }
                    }

                    if ($scope.formobject.Elements[i].Name == "sfwTable") {
                        PopulateRetrievalOrAutoCompleteParameters($scope.formobject.Elements[i], $scope.lstParameters, $scope.formobject.dictAttributes.sfwType, true);
                        break;
        }
                }
            }
           
        }
        else {
            PopulateRetrievalOrAutoCompleteParameters(panelObject, $scope.lstParameters, $scope.formobject.dictAttributes.sfwType, true);
        }
        $scope.PopulateGlobalParameters();

        $scope.bindParameterValue();
    };

    $scope.PopulateGlobalParameters = function () {
        function iterator(itm) {
            if (itm.dictAttributes && itm.dictAttributes.ID) {
                var strFieldName = itm.dictAttributes.ID;
                if (!globalParameters.filter(function (itm) { return itm == strFieldName.trim(); })) {
                    globalParameters.push(strFieldName.trim());
                }
                var mainItem = { ID: "~" + strFieldName, IsExpanded: false, IsSelected: false, Elements: [] };
                objGlobalParam.Elements.push(mainItem);
            }
        }
        if ($scope.objGlobleParameters) {
            var strFormType = $scope.formobject.dictAttributes.sfwType;
            if (($scope.model.Name == "sfwTextBox" || $scope.model.Name == "sfwButton"|| $scope.model.Name == "sfwMultiSelectDropDownList"|| 
                    $scope.model.Name == "sfwCascadingDropDownList" || $scope.model.Name == "sfwDropDownList" || $scope.model.Name == "sfwCheckBoxList" || $scope.model.Name == "sfwRadioButtonList")
                 ) {
                if ($scope.model.Name == "sfwButton") {
                    if ($scope.model.dictAttributes.sfwMethodName != "btnOpen_Click" && $scope.model.dictAttributes.sfwMethodName != "btnNew_Click")
                        return;
                }
                var globalParameters = [];

                if ($scope.objGlobleParameters.Elements.length > 0) {
                    var objGlobalParam = { ID: "Global Parameters", IsExpanded: false, IsSelected: false, Elements: [] };

                    angular.forEach($scope.objGlobleParameters.Elements, iterator);

                    if (objGlobalParam.Elements.length > 0) {
                        $scope.lstParameters.push(objGlobalParam);
                    }
                }


            }
        }
    };


    $scope.ExpandCollapsedControl = function (lstparameter, event) {
        lstparameter.IsExpanded = !lstparameter.IsExpanded;
    };
    $scope.selectParameter = function (parameter, event) {
        $scope.selectedParameter = parameter;
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.bindParameterValue = function () {
        if ($scope.selectedCurrentQuery != undefined && $scope.selectedCurrentQuery != "") {
            $scope.ParameterCollection = [];
            angular.forEach($scope.selectedCurrentQuery.Parameters, function (strParam) {
                ParameterField = strParam.ID;
                if (ParameterField.contains("@")) {
                    strParamField = ParameterField.substring(ParameterField.indexOf('@') + 1, ParameterField.length);
                }
                else {
                    strParamField = ParameterField;
                }

                var objParameter = { ParameterField: strParamField, ParameterValue: "" };
                $scope.ParameterCollection.push(objParameter);
            });
        }
        if ($scope.ParameterCollection != undefined && $scope.ParameterCollection.length > 0 && $scope.strSelectedParameters != undefined) {
            var lst = $scope.strSelectedParameters.split(";");
            for (var i = 0; i < lst.length; i++) {
                var lstControlsID = lst[i].split("=");
                for (var j = 0; j < $scope.ParameterCollection.length; j++) {
                    if ($scope.ParameterCollection[j].ParameterField == lstControlsID[0]) {
                        $scope.ParameterCollection[j].ParameterValue = lstControlsID[1];
                    }
                }
            }
        }
    };
    $scope.bindParameterValue();

    $scope.okClick = function () {
        $scope.strSelectedParameters = "";
        if ($scope.ParameterCollection != undefined) {
            for (var i = 0; i < $scope.ParameterCollection.length; i++) {
                if ($scope.ParameterCollection[i].ParameterValue != "") {
                    if ($scope.strSelectedParameters != undefined && $scope.strSelectedParameters != "") {
                        $scope.strSelectedParameters += ";" + $scope.ParameterCollection[i].ParameterField + "=" + $scope.ParameterCollection[i].ParameterValue;
                    }
                    else if ($scope.strSelectedParameters == undefined || $scope.strSelectedParameters == "") {
                        $scope.strSelectedParameters = $scope.ParameterCollection[i].ParameterField + "=" + $scope.ParameterCollection[i].ParameterValue;
                    }
                }
            }
        }
        //if ($scope.IsAutoComplete) {
        //    $scope.$emit("onAutoCompleteParameterClick", $scope.strSelectedParameters);
        //}
        //else {
        $scope.$emit("onRetrievalParameterClick", $scope.strSelectedParameters);
        // }
        $scope.closeClick();
    };
    $scope.closeClick = function () {
        $scope.objNewDialog.close();
    };

    $scope.SelectFieldClick = function (field, event) {
        //$scope.SelectedField = field;
        if (event) {
            event.stopPropagation();
        }
    };


    //#region Html Methods
    $scope.PopulateRetrievalParametersForHtml = function (panel, lstParameters, formType) {
        if (panel) {
            angular.forEach(panel.Elements, function (ctrl) {
                if (ctrl.Name != "sfwPanel" && ctrl.Name != "sfwWizardStep" && ctrl.Name != "sfwGridView") {
                    if (formType == "FormLinkLookup" && ctrl.dictAttributes.sfwDataField && ctrl.dictAttributes.ID) {
                        var obj = { ID: ctrl.dictAttributes.ID, ControlID: "", Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                    }
                    else if ((formType == "FormLinkMaintenance" || formType == "FormLinkWizard") && ctrl.dictAttributes.sfwEntityField && ctrl.dictAttributes.ID) {
                        var obj = { ID: ctrl.dictAttributes.ID, ControlID: "", Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                    }
                }
                if (ctrl.Elements.length > 0) {
                    var lsttempParameters = lstParameters;
                    if (ctrl.Name == "sfwPanel" || ctrl.Name == "sfwDialogPanel") {
                        var Id = ctrl.dictAttributes.sfwCaption;
                        if (Id == undefined || Id == "") {
                            Id = ctrl.dictAttributes.ID;
                        }
                        var obj = { ID: Id, Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                        lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    }
                    if (ctrl.Name == "sfwWizardStep") {
                        var Id = ctrl.dictAttributes.Title;
                        if (Id == undefined || Id == "") {
                            Id = ctrl.dictAttributes.ID;
                        }
                        var obj = { ID: Id, Elements: [], IsExpanded: false };
                        lstParameters.push(obj);
                        lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    }
                    //if (ctrl.Name == "sfwGridView") {
                    //    var obj = { ID: ctrl.dictAttributes.ID + "(Data Keys)", Elements: [], IsExpanded: false }
                    //    lstParameters.push(obj);
                    //    lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                    //    PopulateGridEntityField(ctrl, lsttempParameters);
                    //}
                    $scope.PopulateRetrievalParametersForHtml(ctrl, lsttempParameters, formType);
                }
            });
        }
    };

    //#endregion

    $scope.Init();
}]);

app.directive("retrievalparameterdraggable",[ function () {
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
                e.stopPropagation();
                if (scope.dragdata != undefined && scope.dragdata != "") {
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                }
            }
        }
    };
}]);

app.directive("retrievalparameterdroppable",[ function () {
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
                        scope.dropdata = data;
                    });
                }
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
});
        }
    };
}]);