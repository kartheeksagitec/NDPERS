app.controller("RetrievalControlsController", ["$scope", "$rootScope", "$filter", "ngDialog", "$EntityIntellisenseFactory", function ($scope, $rootScope, $filter, ngDialog, $EntityIntellisenseFactory) {
    $scope.lstRetrievalControl = [];
    $scope.lstColumns = [];
    $scope.ShowAll = {};
    $scope.ShowAll.isShowAll = false;
    var panelObject;
    $scope.Init = function () {
        function iterateAttrs(attr) {
            if (attr.DataType != "Collection" && attr.DataType != "Object" && attr.DataType != "CDOCollection" && attr.Type != "Expression") {
                $scope.lstColumns.push({ CodeID: attr.ID });
            }
        }
        function iterateMethod(objMethod) {
            if (objMethod && objMethod.ID == strObjectMethod) {
                if (objMethod.Entity != undefined && objMethod.Entity != "") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var lstEntity = $filter('filter')(entityIntellisenseList, { ID: objMethod.Entity }, true);
                    if (lstEntity && lstEntity.length > 0) {
                        var objEntity = lstEntity[0];
                        angular.forEach(objEntity.Attributes, iterateAttrs);
                    }
                }
            }
        }
        if ($scope.model.dictAttributes.sfwRetrievalQuery || ($scope.IsAutoComplete && $scope.model.autocompleteType === "Query" && $scope.model.dictAttributes.sfwAutoQuery)) {
            if ($scope.IsAutoComplete && $scope.model.dictAttributes.sfwAutoQuery) {
                var lst = $scope.model.dictAttributes.sfwAutoQuery.split('.');
            } else if ($scope.IsRetrievalQuery && $scope.model.dictAttributes.sfwRetrievalQuery) {
                var lst = $scope.model.dictAttributes.sfwRetrievalQuery.split('.');
            }
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var strQueryID = lst[1];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lstEntity = $filter('filter')(entityIntellisenseList, { ID: entityName }, true);
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) { return x.ID == strQueryID; });
                    if (lstQuery && lstQuery.length > 0) {
                        var objQuery = lstQuery[0];
                        if ($scope.IsAutoComplete) {
                            $.connection.hubForm.server.getEntityQueryColumns($scope.model.dictAttributes.sfwAutoQuery, "RetrievalControlsController").done(function (data) {
                                $scope.receiveQueryColumns(data);
                            });
                        }
                        else {
                            $.connection.hubForm.server.getEntityQueryColumns($scope.model.dictAttributes.sfwRetrievalQuery, "RetrievalControlsController").done(function (data) {
                                $scope.receiveQueryColumns(data);
                            });
                        }

                    }
                }
            }
        }
        else if ($scope.model.dictAttributes.sfwRetrievalMethod || ($scope.IsAutoComplete && $scope.model.autocompleteType === "Method" && $scope.model.dictAttributes.sfwAutoMethod)) {
            var strObjectMethod = null;
            if ($scope.IsAutoComplete) {
                strObjectMethod = $scope.model.dictAttributes.sfwAutoMethod;
            }
            else {
                strObjectMethod = $scope.model.dictAttributes.sfwRetrievalMethod.trim();
            }

            if (!strObjectMethod)
                return;
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            $scope.lstColumns = [];
            var lstMethod = GetObjectMethods(entityIntellisenseList, $scope.formobject.dictAttributes.sfwEntity, strObjectMethod);

            angular.forEach(lstMethod, iterateMethod);
        }
        $scope.receiveQueryColumns = function (data) {
            $scope.$apply(function () {
                $scope.lstColumns = data;
            });
        };
        var isWizard = $scope.formobject.dictAttributes.sfwType == "Wizard";
        if (isWizard) {
            panelObject = GetVM("sfwWizardStep", $scope.model);
        }
        else if ($scope.formobject.dictAttributes.sfwType == "Correspondence" || $scope.formobject.dictAttributes.sfwType == "UserControl") {
            panelObject = GetVM("sfwTable", $scope.model);
        }
        else {
            panelObject = GetVM("sfwPanel", $scope.model);
        }
        if ($scope.formobject.dictAttributes.sfwType == "FormLinkLookup" || $scope.formobject.dictAttributes.sfwType == "FormLinkMaintenance" || $scope.formobject.dictAttributes.sfwType == "FormLinkWizard") {
            var lst = $scope.formobject.Elements.filter(function (itm) { return itm.Name == "items"; });
            if (lst && lst.length > 0) {
                PopulateRetrievalControls(lst[0], $scope.lstRetrievalControl);
            }
        }
        else {
            PopulateRetrievalControls(panelObject, $scope.lstRetrievalControl);
        }
        $scope.bindControlsID();
    };
    $scope.bindControlsID = function () {
        if ($scope.strSelectedRetrievalControls != undefined) {
            var lst = $scope.strSelectedRetrievalControls.split(";");
            for (var i = 0; i < lst.length; i++) {
                var lstControlsID = lst[i].split("=");
                for (var j = 0; j < $scope.lstRetrievalControl.length; j++) {
                    if ($scope.lstRetrievalControl[j].ID == lstControlsID[0]) {
                        $scope.lstRetrievalControl[j].ControlID = lstControlsID[1];
                    }
                }
            }
        }
    };
    // $scope.bindControlsID();
    $scope.ShowAllControls = function () {

        $scope.GetRetrievalControlValue();

        $scope.lstRetrievalControl = [];
        if ($scope.ShowAll.isShowAll) {
            PopulateRetrievalControls($scope.formobject, $scope.lstRetrievalControl);
        }
        else {
            PopulateRetrievalControls(panelObject, $scope.lstRetrievalControl);
        }
        $scope.bindControlsID();
    };
    $scope.closeClick = function () {
        $scope.RetrievalControlsDialog.close();
    };

    $scope.GetRetrievalControlValue = function () {
        $scope.strSelectedRetrievalControls = "";
        for (var i = 0; i < $scope.lstRetrievalControl.length; i++) {
            if ($scope.lstRetrievalControl[i].ControlID != "") {
                if ($scope.strSelectedRetrievalControls != undefined && $scope.strSelectedRetrievalControls != "") {
                    $scope.strSelectedRetrievalControls += ";" + $scope.lstRetrievalControl[i].ID + "=" + $scope.lstRetrievalControl[i].ControlID;
                }
                else if ($scope.strSelectedRetrievalControls == undefined || $scope.strSelectedRetrievalControls == "") {
                    $scope.strSelectedRetrievalControls = $scope.lstRetrievalControl[i].ID + "=" + $scope.lstRetrievalControl[i].ControlID;
                }
            }
        }

    };

    $scope.okClick = function () {
        $scope.GetRetrievalControlValue();

        if ($scope.IsAutoComplete) {
            $scope.$emit("onAutoCompletelControlClick", $scope.strSelectedRetrievalControls);
        }
        else {
            $scope.$emit("onRetrievalControlClick", $scope.strSelectedRetrievalControls);
        }


        $scope.closeClick();
        if ($scope.OnOkClick) {
            $scope.OnOkClick();
        }
    };

    $scope.Init();
}]);

app.directive("retrievalcontroldraggable", [function () {
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

app.directive("retrievalcontroldroppable", [function () {
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