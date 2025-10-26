app.controller("CenterLeftController", ["$scope", "$filter", "$rootScope", "$ValidationService", "$SgMessagesService", function ($scope, $filter, $rootScope, $ValidationService, $SgMessagesService) {


    $scope.Init = function () {
        if ($scope.CenterLeft) {
            for (var i = 0; i < $scope.CenterLeft.Elements.length; i++) {
                if ($scope.CenterLeft.Elements[i].Name == "sfwTreeView") {
                    $scope.objTreeView = $scope.CenterLeft.Elements[i];
                    $scope.objTreeView.objNodes = $scope.CheckAndCreateModel("Nodes", $scope.objTreeView);
                }
            }
        }
        if ($scope.objTreeView) {
            $scope.objTreeView.IsExpanded = false;
        }
        if ($scope.objTreeView && $scope.objTreeView.objNodes && $scope.objTreeView.objNodes.Elements.length > 0) {
            $scope.objTreeView.IsExpanded = true;
            $scope.SelectNodeClick($scope.objTreeView.objNodes.Elements[0]);
        }
        $scope.IsExpanded = true;
    };
    //#region Context Menu
    var copyItem = {};
    $scope.menuOptionsForCenterLeft = [
        ['Add Control', [
            ['sfwTreeNode', function ($itemScope) {
                $scope.AddControlCommand("TreeNode");

            }, function ($itemScope) {

                return true;
            }],
            ['sfwXmlPanelNode', function ($itemScope) {
                $scope.AddControlCommand("XmlPanelNode");
            }, function ($itemScope) {

                return true;
            }],
        ]], null,
        ['Delete', function ($itemScope) {

            if ($itemScope.nodes && $itemScope.nodes.Name === "sfwTreeNode") {
                $SgMessagesService.Message('Delete', 'Do you want to delete?', true, function (result) {
                    if (result) {
                        var index = $scope.objTreeView.objNodes.Elements.indexOf($itemScope.nodes);
                        if (index > -1) {
                            $scope.objTreeView.objNodes.Elements.splice(index, 1);


                            if (index < $scope.objTreeView.objNodes.Elements.length) {

                                $scope.SelectNodeClick($scope.objTreeView.objNodes.Elements[index], null);
                            }
                            else if ($scope.objTreeView.objNodes.Elements.length > 0) {

                                $scope.SelectNodeClick($scope.objTreeView.objNodes.Elements[index - 1], null);
                            }
                            else {
                                $scope.SelectNodeClick($scope.objTreeView, null);
                            }
                        }
                    }
                });
            }
            else if ($itemScope.itm && $itemScope.itm.Name === "sfwXMLPanel") {
                $SgMessagesService.Message('Delete', 'Do you want to delete?', true, function (result) {
                    if (result) {
                        var index = $scope.CenterLeft.Elements.indexOf($itemScope.itm);
                        if (index > -1) {
                            $scope.CenterLeft.Elements.splice(index, 1);
                            $scope.SelectNodeClick($scope.CenterLeft.Elements[0], null);
                        }
                    }
                });
            }


        },
            function ($itemScope) {
                if ($itemScope.nodes) {
                    $scope.SelectedNode = $itemScope.nodes;

                } else if ($itemScope.itm) {
                    $scope.SelectedNode = $itemScope.itm;
                }
                if (($itemScope.nodes && $itemScope.nodes.Name === "sfwTreeNode") || ($itemScope.itm && $itemScope.itm.Name === "sfwXMLPanel")) {
                    return true;
                }
                else {
                    return false;
                }
            }], null,

        ['Copy', function ($itemScope) {
            copyItem = angular.copy($itemScope.nodes, copyItem);
        },
            function ($itemScope) {
                if ($itemScope.nodes && $itemScope.nodes.Name === "sfwTreeNode") {
                    return true;
                }
                else {
                    return false;
                }
            }], null,
        ['Paste', function ($itemScope) {
            $scope.objTreeView.objNodes.Elements.push(copyItem);
            copyItem = null;
        },
            function ($itemScope) {
                if (copyItem && copyItem.dictAttributes) {
                    return true;
                }

            }], null,
        ['Rename', function ($itemScope) {
            var newScope = $scope.$new();
            if ($itemScope.itm) {
                newScope.newRuleID = $itemScope.itm.dictAttributes.ID;
                var dialog = $rootScope.showDialog(newScope, "Edit ID", "Entity/views/AddRule.html", {
                    width: 500, height: 180
                });

                newScope.addNewRule = function () {
                    $scope.SelectedNode.dictAttributes.ID = newScope.newRuleID;
                    newScope.closeNewRule();
                }
                newScope.closeNewRule = function () {

                    dialog.close();
                }

                newScope.validateNewGroup = function ($itemScope) {
                    newScope.groupErrorMessageForDisplay = "";
                    if (!newScope.newRuleID) {
                        newScope.groupErrorMessageForDisplay = "Error: ID cannot be empty.";
                        return true;
                    }
                    else {
                        var flag = $ValidationService.findDuplicateId($scope.CenterLeft, newScope.newRuleID, ["sfwTreeView", "sfwXMLPanel"], "", null, "dictAttributes.ID");
                        if (flag) {
                            newScope.groupErrorMessageForDisplay = "Error:Duplicate ID present. Please enter another ID.";
                            return true;
                        }
                    }
                }
            }
        },
            function ($itemScope) {
                if ($itemScope.nodes && $itemScope.nodes.Name === "sfwTreeNode") {
                    return false;
                }
                else if (!$itemScope.nodes && !$itemScope.itm) {
                    return false;
                }
                else {
                    return true;
                }
            }], null,
    ];
    //#endregion

    //#region Add Control

    $scope.AddControlCommand = function (cntrlType) {
        if (cntrlType) {
            if (cntrlType == "TreeNode") {
                $scope.AddTreeNode();
            }
            else if (cntrlType == "XmlPanelNode") {
                $scope.AddXmlPanelNode();
            }
        }
    };
    //#endregion

    //#region Add Tree Node
    $scope.AddTreeNode = function () {

        if ($scope.CenterLeft) {
            if ($scope.objTreeView && $scope.objTreeView.objNodes) {
                var objNodechild = { Name: 'sfwTreeNode', Value: '', dictAttributes: {}, Elements: [] };
                $scope.objTreeView.objNodes.Elements.push(objNodechild);
            }
            else {
                $scope.objTreeView = { Name: 'sfwTreeView', Value: '', dictAttributes: {}, Elements: [] };
                $scope.objTreeView.dictAttributes.ID = "trvCenterLeft";
                $scope.objTreeView.IsExpanded = true;
                $scope.objTreeView.objNodes = { Name: 'Nodes', Value: '', dictAttributes: {}, Elements: [] };
                $scope.objTreeView.Elements.push($scope.objTreeView.objNodes);

                var objNodechild = { Name: 'sfwTreeNode', Value: '', dictAttributes: {}, Elements: [] };
                $scope.objTreeView.objNodes.Elements.push(objNodechild);
                $scope.CenterLeft.Elements.push($scope.objTreeView);

            }

            $scope.SelectNodeClick(objNodechild, null);
        }
    };

    $scope.CheckAndCreateModel = function (modelName, Parent) {
        var objModel = undefined;
        var lstModel = Parent.Elements.filter(function (x) { return x.Name === modelName });
        if (lstModel && lstModel.length > 0) {
            objModel = lstModel[0];
        }
        else {
            objModel = { Name: modelName, Value: '', dictAttributes: {}, Elements: [] };
            Parent.Elements.push(objModel);
        }
        return objModel;
    };
    //#endregion


    //#region Add Xml Panel Node
    $scope.AddXmlPanelNode = function () {
        var xmlPanelNodeModel = null;
        xmlPanelNodeModel = { Name: 'sfwXMLPanel', Value: '', dictAttributes: {}, Elements: [] };
        xmlPanelNodeModel.dictAttributes.ID = GetNewStepName("XmlPanelNode", $scope.CenterLeft, 1);
        $scope.CenterLeft.Elements.push(xmlPanelNodeModel);
        $scope.SelectNodeClick(xmlPanelNodeModel, null);

    };
    //#endregion

    $scope.togglebuttonForTreeView = function (item) {
        item.IsExpanded = !item.IsExpanded;
    };

    $scope.toggleRootNode = function () {
        $scope.togglebuttonForTreeView($scope);
    }

    $scope.SelectNodeClick = function (obj, event) {

        $scope.SelectedNode = obj;
        if (event != null) {
            event.stopPropagation();
        }
    };

    $scope.onNavigationParamsClick = function () {
        var newScope = $scope.$new();
        if ($scope.SelectedNode) {
            newScope.SelectedObject = $scope.SelectedNode;
            newScope.IsForm = true;
            newScope.IsMultiActiveForm = false;
            newScope.formobject = $scope.formmodel;
            newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
        }
    };
    $scope.Init();
}]);