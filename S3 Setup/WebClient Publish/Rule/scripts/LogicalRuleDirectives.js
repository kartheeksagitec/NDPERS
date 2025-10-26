app.directive("logicalRuleStartNodeList", ["$compile", function ($compile) {
    return {
        restrict: "E",
        scope: {
            model: "=",
            viewMode: "=",
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getStartTemplate = function () {
                return '<ul class="logical-rule-first-ul"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in siblingsCollection" model="item" view-mode="viewMode"></logical-rule-node-wrapper></ul>';
            };
            scope.init = function () {
                scope.siblingsCollection = scope.model.Elements;
            };
            scope.init();

            //var parentNode = element.parent();
            //if (parentNode) {
            //    var ul = $(scope.getStartTemplate());
            //    $compile(ul)(scope);
            //    element.remove();
            //    parentNode.append(ul);
            //}
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        templateUrl: "Rule/views/LogicalRule/LogicalRuleStartTemplate.html"
    };
}]);
app.directive("logicalRuleNodeList", ["$compile", function ($compile) {
    return {
        restrict: "E",
        replace: true,
        //scope: {
        //    viewMode: "=",
        //    model: "="
        //},
        link: function (scope, element, attributes) {
            scope.getElementsTemplate = function () {
                var elementsTemplateTextList = ['<ul ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':\'\'">                                                                                                         '
                    , '    <li ng-if="item.Name != \'variables\'" ng-repeat="item in siblingsCollection">  '
                    , '        <logical-rule-node-wrapper ng-if="item.Name != \'variables\'" model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                            '
                    , '    </li>                                                                                                                                                                            '
                    , '</ul>                                                                                                                                                                                '];

                return elementsTemplateTextList.join("");
            };

            scope.siblingsCollection = (scope.model.Name == "case" || scope.model.Name == "default") ? scope.model.Elements : scope.model.Children;


            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getElementsTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);
            }
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        //templateUrl: "Rule/views/LogicalRule/LogicalRuleStartTemplate.html"
    };
}]);
app.directive("loopSwitchChildren", ["$compile", function ($compile) {
    return {
        restrict: "E",
        replace: true,
        //scope:{
        //    viewMode: "=",
        //    model: "="
        //},
        link: function (scope, element, attributes) {
            scope.getChildrenTemplate = function () {
                var childrenTemplateTextList = ['<ul ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                                                                       '
                    , '    <li ng-repeat="item in model.Elements">  '
                    , '        <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                        '
                    , '    </li>                                                                                                                                     '
                    , '                                                                                                                                              '
                    , '</ul>                                                                                                                                         '];
                return childrenTemplateTextList.join("");
            };
            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getChildrenTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);
            }
        },
        //template: '<ul ng-hide="isExpanded === false" ng-class="model.Name == \'logicalrule\'?\'logical-rule-first-ul\':(isSiblingsList?\'\':(model.Name == \'switch\'? \'ul-switch\':\'ul-children\'))"><logical-rule-node-wrapper  ng-if="item.Name != \'variables\'" ng-repeat="item in collection" model="item" parent="model" view-mode="viewMode"></logical-rule-node-wrapper></ul>'
        //templateUrl: "Rule/views/LogicalRule/LoopSwitchChildrenTemplate.html"
    };
}]);

app.directive('logicalRuleNodeWrapper', ["$compile", "$rootScope", "$timeout", function ($compile, $rootScope, $timeout) {
    var AddItem = function (obj, param) {
        var objItem;
        var newObject = null;
        if (obj.Name == "case" || obj.Name == "default") {
            objItem = obj.Elements;
        }
        else {
            objItem = obj.Children;
        }

        var nodeid = generateUUID();
        newObject = { Name: param, value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] };
        if (param == 'switch') {
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'case', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'default', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
        }
        else if (param == 'actions') {
            newObject.Elements.push({ Name: 'action', value: '', dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: [] });
        }

        newObject.IsSelected = true;
        $rootScope.PushItem(newObject, objItem);

        $timeout(function () {
            if (obj) {
                checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
            }
            enableExpressionEditing(newObject.dictAttributes.sfwNodeID);
        });
    };

    var AddStep = function (obj, param) {
        var nodeid = generateUUID();
        var newObject = { Name: param, value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] };
        var insertionIndex = null;

        if (param == 'switch') {
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'case', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
            nodeid = generateUUID();
            newObject.Elements.push({ Name: 'default', value: '', dictAttributes: { sfwNodeID: nodeid }, Elements: [], Children: [] });
        }
        else if (param == 'case') {
            var isFound = false;
            for (var i = 0; i < obj.Elements.length; i++) {
                if (obj.Elements[i].Name == 'default') {
                    isFound = true;
                    insertionIndex = obj.Elements.indexOf(obj.Elements[i]);
                    break;
                }
            }
        }
        else if (param == 'actions') {
            newObject.Elements.push({ Name: 'action', value: '', dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: [] });
        }

        newObject.IsSelected = true;
        if (insertionIndex) {
            $rootScope.InsertItem(newObject, obj.Elements, insertionIndex);
        }
        else {
            $rootScope.PushItem(newObject, obj.Elements);
        }

        $timeout(function () {
            if (obj) {
                checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
            }
            enableExpressionEditing(newObject.dictAttributes.sfwNodeID);
        });
    };

    var cutorcopyitem;
    var objParent;
    var Param;

    var CutOrCopyItem = function (obj, objofParent, param) {
        cutorcopyitem = obj;
        objParent = objofParent;
        Param = param;
    };

    var PasteItem = function (obj) {
        if (cutorcopyitem != undefined) {
            $rootScope.UndRedoBulkOp("Start");

            var data = JSON.stringify(cutorcopyitem);
            var objCutorCopyItem = JSON.parse(data);

            if (Param == 'cut') {

                if (cutorcopyitem.Name == 'case') {
                    if (obj.Name != 'switch') {
                        alert("If/Else block can be added only inside switch block");
                        objParent = undefined;
                    }
                    else {
                        var count = 0;
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name) {
                                count++;
                            }
                        }

                        if (count <= 1) {

                            alert("Atleast one if block has to be present inside switch block");
                            objParent = undefined;
                        }
                    }
                }

                if (cutorcopyitem.Name == 'default' && obj.Name == 'switch') {
                    for (var i = 0; i < obj.Elements.length; i++) {
                        if (obj.Elements[i].Name == cutorcopyitem.Name) {
                            alert("Only one Else block can be present");
                            objParent = undefined;
                        }
                    }
                }
                else if (cutorcopyitem.Name == 'default' && obj.Name != 'switch') {
                    alert("If/Else block can be added only inside the Condition");
                    objParent = undefined;
                }
                var ischildren = false;
                var iselements = false;
                if (objParent != undefined) {
                    if (objParent.Name == "foreach" || objParent.Name == "while") {
                        for (var i = 0; i < objParent.Children.length; i++) {
                            if (objParent.Children[i].Name == cutorcopyitem.Name && objParent.Children[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Children[i], objParent.Children);
                                ischildren = true;
                                var curIndex = i;
                                break;
                            }
                        }
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name && objParent.Elements[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Elements[i], objParent.Elements);
                                var curIndex = i;
                                iselements = true;
                                break;
                            }
                        }
                    }
                    if (objParent.Name != "logicalrule" && objParent.Name != "case" && objParent.Name != "default") {
                        for (var i = 0; i < objParent.Children.length; i++) {
                            if (objParent.Children[i].Name == cutorcopyitem.Name && objParent.Children[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Children[i], objParent.Children);
                                ischildren = true;
                                var curIndex = i;
                                break;
                            }
                        }
                    }
                    if (objParent.Name == "logicalrule" || objParent.Name == "case" || objParent.Name == "default" || objParent.Name == "switch") {
                        for (var i = 0; i < objParent.Elements.length; i++) {
                            if (objParent.Elements[i].Name == cutorcopyitem.Name && objParent.Elements[i].dictAttributes.sfwNodeID == cutorcopyitem.dictAttributes.sfwNodeID) {
                                $rootScope.DeleteItem(objParent.Elements[i], objParent.Elements);
                                var curIndex = i;
                                iselements = true;
                                break;
                            }
                        }
                    }

                    if ((cutorcopyitem.Name == "case" || cutorcopyitem.Name == "default") && obj.Name == "switch") {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == "default") {
                                isfound = true;
                                $rootScope.InsertItem(objCutorCopyItem, obj.Elements, i);
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                    else {
                        if ((obj.Name == 'case' && cutorcopyitem.Name == 'case') || (obj.Name == 'default' && cutorcopyitem.Name == 'default') || (obj.Name == 'default' && cutorcopyitem.Name == 'case') ||
                            (obj.Name == 'case' && cutorcopyitem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case')) {
                            alert("If/Else block can be added only inside the Condition");
                        }
                        else {



                            var data = JSON.stringify(cutorcopyitem);
                            var objCutorCopyItem = JSON.parse(data);
                            if (obj.Name == 'case' || obj.Name == 'default') {
                                $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                            }
                            else if (cutorcopyitem.Name == 'case' && obj.Name == 'switch') {
                                var casecount = 0;
                                for (var j = 0; j < obj.Elements.length; j++) {
                                    if (obj.Elements[j].Name == 'case') {
                                        casecount++;
                                    }
                                }
                                $rootScope.InsertItem(cutorcopyitem, obj.Elements, casecount);

                            }
                            else {
                                $rootScope.PushItem(objCutorCopyItem, obj.Children);
                            }
                        }
                    }

                }
                cutorcopyitem = undefined;
            }
            else if (Param == 'copy') {
                var nodeid = generateUUID();
                objCutorCopyItem.dictAttributes.sfwNodeID = nodeid;
                objCutorCopyItem.$$hashKey = generateUUID();
                if (objCutorCopyItem.Children != undefined) {
                    for (var i = 0; i < objCutorCopyItem.Children.length; i++) {
                        objCutorCopyItem.Children = [];
                    }
                }
                ChangeNodeidforCopyObject(objCutorCopyItem);

                if ((obj.Name == 'case' && objCutorCopyItem.Name == 'case') || (obj.Name == 'default' && objCutorCopyItem.Name == 'default') || (obj.Name == 'default' && objCutorCopyItem.Name == 'case')
                    || (obj.Name == 'case' && objCutorCopyItem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case') || (obj.Name != 'switch' && cutorcopyitem.Name == 'default')) {
                    alert("If/Else block can be added only inside the Condition");
                }
                else if (obj.Name == 'case' || obj.Name == 'default') {
                    $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                }
                else if ((objCutorCopyItem.Name == 'case' || objCutorCopyItem.Name == 'default') && obj.Name == 'switch') {
                    if (objCutorCopyItem.Name == 'case') {
                        var casecount = 0;
                        for (var j = 0; j < obj.Elements.length; j++) {
                            if (obj.Elements[j].Name == 'case') {
                                casecount++;
                            }
                        }
                        $rootScope.InsertItem(objCutorCopyItem, obj.Elements, casecount);

                    }
                    else if (objCutorCopyItem.Name == 'default') {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == cutorcopyitem.Name) {
                                alert("Only one Else block can be present");
                                isfound = true;
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                }
                else {
                    $rootScope.PushItem(objCutorCopyItem, obj.Children);

                }

            }
            else if (Param == 'copy++') {
                objCutorCopyItem.dictAttributes.sfwNodeID = generateUUID();
                objCutorCopyItem.$$hashKey = generateUUID();
                ChangeNodeidforCopyObject(objCutorCopyItem);
                if ((obj.Name == 'case' && objCutorCopyItem.Name == 'case') || (obj.Name == 'default' && objCutorCopyItem.Name == 'default') || (obj.Name == 'default' && objCutorCopyItem.Name == 'case')
                    || (obj.Name == 'case' && objCutorCopyItem.Name == 'default') || (obj.Name != 'switch' && cutorcopyitem.Name == 'case') || (obj.Name != 'switch' && cutorcopyitem.Name == 'default')) {
                    alert("If/Else block can be added only inside the Condition");
                }
                else if (obj.Name == 'case' || obj.Name == 'default') {
                    $rootScope.PushItem(objCutorCopyItem, obj.Elements);

                }
                else if ((objCutorCopyItem.Name == 'case' || objCutorCopyItem.Name == 'default') && obj.Name == 'switch') {
                    if (objCutorCopyItem.Name == 'case') {
                        var casecount = 0;
                        for (var j = 0; j < obj.Elements.length; j++) {
                            if (obj.Elements[j].Name == 'case') {
                                casecount++;
                            }
                        }
                        $rootScope.InsertItem(objCutorCopyItem, obj.Elements, casecount);

                    }
                    else if (objCutorCopyItem.Name == 'default') {
                        var isfound = false;
                        for (var i = 0; i < obj.Elements.length; i++) {
                            if (obj.Elements[i].Name == cutorcopyitem.Name) {
                                alert("Only one Else block can be present");
                                isfound = true;
                                break;
                            }
                        }
                        if (!isfound) {
                            $rootScope.PushItem(objCutorCopyItem, obj.Elements);
                        }
                    }
                }
                else {
                    $rootScope.PushItem(objCutorCopyItem, obj.Children);
                }
                cutorcopyitem = undefined;
            }
            $rootScope.UndRedoBulkOp("End");

            var rootScope = getCurrentFileScope();
            if (rootScope.objLogicalRule && rootScope.objLogicalRule.dictAttributes && rootScope.objSelectedLogicalRule) {
                rootScope.ResetAllvalues(rootScope.objSelectedLogicalRule);
            }
            objCutorCopyItem.IsSelected = true;

            $timeout(function () {
                if (obj) {
                    checkAndCollapseNode(obj.dictAttributes.sfwNodeID);
                }
            });
        }
    };

    var isChildOfCutItem = function (parentObj, obj) {
        var retVal = false;
        angular.forEach(parentObj.Elements, function (itm) {
            if (!retVal) {
                if (itm.dictAttributes && obj.dictAttributes && itm.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                    retVal = true;
                }
                else {
                    retVal = isChildOfCutItem(itm, obj);
                }
            }
        });
        if (!retVal) {
            angular.forEach(parentObj.Children, function (itm) {
                if (!retVal) {
                    if (itm.dictAttributes && obj.dictAttributes && itm.dictAttributes.sfwNodeID == obj.dictAttributes.sfwNodeID) {
                        retVal = true;
                    }
                    else {
                        retVal = isChildOfCutItem(itm, obj);
                    }
                }
            });
        }
        return retVal;
    };

    var ChangeNodeidforCopyObject = function (obj) {
        if (obj.Children != undefined) {
            for (var i = 0; i < obj.Children.length; i++) {
                obj.Children[i].dictAttributes.sfwNodeID = generateUUID();
                if (obj.Children[i].$$hashKey) {
                    obj.Children[i].$$hashKey = generateUUID();
                }
                if (obj.Children[i].Children != undefined) {
                    if (obj.Children[i].Children.length > 0) {
                        ChangeNodeidforCopyObject(obj.Children[i]);
                    }
                }
                else {
                    if (obj.Children[i].Elements.length > 0) {
                        ChangeNodeidforCopyObject(obj.Children[i]);
                    }
                }
            }
        }
        if (obj.Elements != undefined) {
            for (var i = 0; i < obj.Elements.length; i++) {
                obj.Elements[i].dictAttributes.sfwNodeID = generateUUID();
                if (obj.Elements[i].$$hashKey) {
                    obj.Elements[i].$$hashKey = generateUUID();
                }
                if (obj.Elements[i].Elements.length > 0) {
                    ChangeNodeidforCopyObject(obj.Elements[i]);
                }
                if (obj.Elements[i].Children != undefined) {
                    if (obj.Elements[i].Children.length > 0) {
                        ChangeNodeidforCopyObject(obj.Elements[i]);
                    }
                }
            }
        }
    };

    // Delete Step Methods
    var OnDeleteCurrentStepClick = function (obj, objParent) {
        $rootScope.UndRedoBulkOp("Start");
        var retVal = false;
        if (obj.Name == 'case') {
            retVal = DeleteIf(obj, objParent);
        }
        else if (obj.Name == "action") {
            retVal = DeleteAssignBlock(obj, objParent);
        }
        else {
            if (objParent.Name == "logicalrule" || objParent.Name == "switch" || objParent.Name == "case" || objParent.Name == "default" || objParent.Name == "foreach" || objParent.Name == "while") {
                var curIndex = objParent.Elements.indexOf(obj);
                if (curIndex > -1) {
                    $rootScope.DeleteItem(obj, objParent.Elements);

                    //objParent.Elements.splice(curIndex, 1);
                    if (objParent.Name != "switch") {
                        if (obj.Children != undefined) {
                            angular.forEach(obj.Children, function (item) {
                                $rootScope.InsertItem(item, objParent.Elements, curIndex);

                                // objParent.Elements.splice(curIndex, 0, item);
                                curIndex++;
                            });
                        }
                    }
                }
            }
            if (objParent.Name != "logicalrule" && objParent.Name != "case" && objParent.Name != "default") {
                var curIndex = objParent.Children.indexOf(obj);
                if (curIndex > -1) {
                    $rootScope.DeleteItem(obj, objParent.Children);

                    //objParent.Children.splice(curIndex, 1);

                    if (obj.Children != undefined && obj.Children.length > 0) {
                        angular.forEach(obj.Children, function (item) {
                            $rootScope.InsertItem(item, objParent.Children, curIndex);


                            // objParent.Children.splice(curIndex, 0, item);
                            curIndex++;
                        });
                    }
                }
            }
            retVal = true;
        }
        $rootScope.UndRedoBulkOp("End");

        return retVal;
    };

    var DeleteIf = function (obj, objParent) {
        var retVal = false;
        var iCount = GetIfAndAssignBlockCount(objParent, obj.Name);
        if (iCount == 1) {
            alert("One If Block Should be there");
        }
        else {
            $rootScope.DeleteItem(obj, objParent.Elements);

            // objParent.Elements.splice(objParent.Elements.indexOf(obj), 1);
            retVal = true;
        }
        return retVal;
    };

    var GetIfAndAssignBlockCount = function (objParent, strName) {
        var iCount = 0;
        angular.forEach(objParent.Elements, function (item) {
            if (item.Name == strName) {
                iCount++;
            }
        });
        return iCount;
    };

    var DeleteAssignBlock = function (obj, objParent) {
        var retVal = false;
        var iCount = GetIfAndAssignBlockCount(objParent);
        if (iCount == 1) {
            alert("One Assign Should be there");
        }
        else {
            $rootScope.DeleteItem(obj, objParent.Elements);

            // objParent.Elements.splice(objParent.Elements.indexOf(obj), 1);
            retVal = true;
        }
    };

    var AddAction = function (obj) {
        var nodeid = generateUUID();
        var objaction = { Elements: [], dictAttributes: { sfwNodeID: nodeid, sfwExpression: "" }, Children: [], IsSelected: false, Name: "action", Value: "" };
        $rootScope.PushItem(objaction, obj.Elements);
    };

    var DeleteAction = function (obj) {
        if (obj.Elements.length > 1) {
            $rootScope.DeleteItem(obj.Elements[obj.Elements.length - 1], obj.Elements);

            // obj.Elements.splice(obj.Elements.length - 1, 1);
        }
    };

    return {
        restrict: "E",
        scope: {
            model: "=",
            parentModel: "=",
            viewMode: "=",
        },
        replace: true,
        link: function (scope, element, attributes) {
            scope.getNodeTemplate = function () {
                var templateText = "";
                switch (scope.model.Name) {
                    case "actions":
                        templateText = '<actions-node></actions-node>';
                        break;
                    case "break":
                    case "continue":
                    case "default":
                        templateText = '<break-continue-default-node></break-continue-default-node>';
                        break;
                    case "notes":
                        templateText = '<notes-node></notes-node>';
                        break;
                    case "return":
                    case "switch":
                    case "case":
                    case "while":
                        templateText = '<return-switch-case-while-node></return-switch-case-while-node>';
                        break;
                    case "calllogicalrule":
                    case "calldecisiontable":
                    case "callexcelmatrix":
                        templateText = '<call-rule-node></call-rule-node>';
                        break;
                    case "foreach":
                        templateText = '<for-each-node></for-each-node>';
                        break;
                    case "query":
                        templateText = '<query-node></query-node>';
                        break;
                    case "method":
                        templateText = '<method-node></method-node>';
                        break;

                }
                return templateText;
            };
            scope.getTemplate = function () {
                var templateTextList = ['<div logical-rule-drag-drop node-id="{{::model.dictAttributes.sfwNodeID}}" node-name="{{::model.Name}}" ng-keydown="onkeydownevent($event,model,parentModel)">                                                                                 '
                    , '    <span ng-show="(model.Name === \'foreach\' || model.Name === \'while\' || model.Name === \'switch\') || (model.Name !== \'case\' && model.Name !== \'default\' && model.Children.length > 0) || ((model.Name === \'case\' || model.Name === \'default\') && model.Elements.length > 0)" class="node-expander node-expanded" ng-click="toggleNodeChildren($event)"></span>                                                                                              '
                    , '    <div class="node-wrapper node-wrapper-after">                                                                                                                                        '];
                templateTextList.push(scope.getNodeTemplate());
                templateTextList = templateTextList.concat(['<ul  logical-rule-drag-drop ng-if="model.Name == \'foreach\' || model.Name == \'while\' || model.Name == \'switch\'" ng-class="model.Name == \'switch\'? \'ul-switch\':\'ul-children\'">                 '
                    , '            <li ng-repeat="item in model.Elements">                                                                                                                                      '
                    , '                <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                           '
                    , '            </li>                                                                                                                                                                        '
                    , '                                                                                                                                                                                         '
                    , '        </ul>                                                                                                                                                                            '
                    , '    </div>                                                                                                                                                                               '
                    , '    <ul ng-if="(model.Name == \'case\' || model.Name == \'default\') && model.Elements.length>0">                                                                                        '
                    , '        <li ng-repeat="item in model.Elements">                                                                                                                                          '
                    , '            <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                               '
                    , '        </li>                                                                                                                                                                            '
                    , '    </ul>                                                                                                                                                                                '
                    , '    <ul ng-if="model.Name != \'case\' && model.Name != \'default\' && model.Children.length>0">                                                                                          '
                    , '        <li ng-repeat="item in model.Children">                                                                                                                                          '
                    , '            <logical-rule-node-wrapper model="item" parent-model="model" view-mode="viewMode"></logical-rule-node-wrapper>                                                               '
                    , '        </li>                                                                                                                                                                            '
                    , '    </ul>                                                                                                                                                                                '
                    , '</div>                                                                                                                                                                                   ']);
                return templateTextList.join("");
            };
            scope.toggleParameters = function (event) {
                if (scope.parameters && scope.parameters.Elements.length > 0) {
                    var nodeElement = $(event.target).parents(".node").first();
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.showParameters(nodeElement);
                    }
                    else {
                        scope.hideParameters(nodeElement);
                    }
                }
            };
            scope.showParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideDown("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").addClass("fa-caret-down");
                nodeElement.find(".parameters-expander-refresh.fa-caret-right").removeClass("fa-caret-right");
            }
            scope.hideParameters = function (nodeElement) {
                nodeElement.find(".parameters-wrapper").slideUp("slow");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").addClass("fa-caret-right");
                nodeElement.find(".parameters-expander-refresh.fa-caret-down").removeClass("fa-caret-down");
            }
            scope.syncParameterVisibility = function (nodeElement) {
                $timeout(function () {
                    if (nodeElement.find(".parameters-expander-refresh.fa-caret-right").length > 0) {
                        scope.hideParameters(nodeElement);
                    }
                    else {
                        scope.showParameters(nodeElement);
                    }
                });
            }
            scope.toggleNodeChildren = function (event) {
                if ($(event.target).hasClass("node-expanded")) {
                    collapseNode(event.target, true);
                }
                else {
                    expandNode(event.target, true);
                }
            };
            scope.onStepSelectChange = function (step) {
                var currentFileScope = getCurrentFileScope();
                currentFileScope.onSelectChange(step);
                scope.SelectedNode = step;
            };
            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    if (controllerScope.onActionKeyDown) {
                        controllerScope.SelectedNode = scope.SelectedNode;
                        controllerScope.onActionKeyDown(eargs);
                    }
                }
            };
            scope.onDescriptionKeyDown = function (eargs) {
            }; // Comes from a xml file, which we cannot access from web.
            scope.init = function () {
                if (scope.model.Name === "switch" || scope.model.Name === "foreach" || scope.model.Name === "while") {
                    scope.childrenCollection = scope.model.Elements;
                }

                if (scope.model.Name === "case" || scope.model.Name === "default") {
                    scope.siblingsCollection = scope.model.Elements;
                }
                else {
                    scope.siblingsCollection = scope.model.Children;
                }


                if (scope.model.Elements.length > 0) {
                    var parametersFilter = function (item) {
                        return item.Name == "parameters";
                    };
                    var parametersModel = scope.model.Elements.filter(parametersFilter);
                    if (parametersModel && parametersModel.length > 0) {
                        scope.parameters = parametersModel[0];
                    }

                }

                scope.menuOptions = [
                    ['AddItem', [
                        ['Condition', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'switch');
                        }],
                        ['Action', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'actions');
                        }],
                        ['Method', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'method');
                        }],
                        ['CallLogicalRule', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'calllogicalrule');
                        }],
                        ['CallDecisionTable', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'calldecisiontable');
                        }],
                        ['CallExcelMatrix', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'callexcelmatrix');
                        }],
                        ['Loop', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'foreach');
                        }],
                        ['Return', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'return');
                        }],
                        ['While', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'while');
                        }],
                        ['Break', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'break');
                        }],
                        ['Continue', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'continue');
                        }],
                        ['Query', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'query');
                        }],
                        ['Notes', function ($itemScope) {
                            var obj = scope.model;
                            AddItem(obj, 'notes');
                        }]
                    ]],
                    null, // Dividier
                    ['Add Step', [
                        ['Condition', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'switch');
                        }],
                        ['Action', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'actions');
                        }],
                        ['Method', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'method');
                        }],
                        ['CallLogicalRule', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'calllogicalrule');
                        }],
                        ['CallDecisionTable', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'calldecisiontable');
                        }],
                        ['CallExcelMatrix', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'callexcelmatrix');
                        }],
                        ['Loop', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'foreach');
                        }],
                        ['Return', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'return');
                        }],
                        ['While', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'while');
                        }],
                        ['Break', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'break');
                        }],
                        ['Continue', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'continue');
                        }],
                        ['Query', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'query');
                        }],
                        ['Notes', function ($itemScope) {
                            var obj = scope.model;
                            AddStep(obj, 'notes');
                        }]
                    ], function ($itemScope) {
                        return ($itemScope.model.Name.match(/foreach/) || $itemScope.model.Name.match(/while/)) != null;
                    }],
                    null,
                    ['Add if', function ($itemScope) {
                        var obj = scope.model;
                        AddStep(obj, 'case');
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/switch/) != null;
                    }
                    ],
                    null,
                    ['Add else', function ($itemScope) {
                        var obj = scope.model;
                        AddStep(obj, 'default');
                    }, function ($itemScope) {
                        for (var i = 0; i < $itemScope.model.Elements.length; i++) {
                            if ($itemScope.model.Elements[i].Name == 'default') {
                                return false;
                            }
                        }
                        return $itemScope.model.Name.match(/switch/) != null;
                    }
                    ],
                    null,
                    ['Add Action', function ($itemScope) {
                        var obj = scope.model;
                        AddAction(obj);
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/actions/) != null;
                    }
                    ],
                    null,
                    ['Delete Action', function ($itemScope) {
                        var obj = scope.model;
                        DeleteAction(obj);
                    }, function ($itemScope) {
                        return $itemScope.model.Name.match(/actions/) != null && scope.model.Elements.length > 1;
                    }
                    ],
                    null,
                    ['Delete', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        if (lobjparent != undefined) {
                            OnDeleteCurrentStepClick(obj, lobjparent);
                        }
                    }, function ($itemScope) {
                        if ($itemScope.model.Name == "case") {
                            var count = 0;

                            for (var i = 0; i < $itemScope.parentModel.Elements.length; i++) {
                                if ($itemScope.parentModel.Elements[i].Name == 'case') {
                                    count++;
                                }
                            }
                            if (count <= 1) {
                                return false;
                            }
                            else {
                                return true;
                            }
                        }
                        else {
                            return true;
                        }
                    }],
                    null,
                    ['Cut', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'cut');
                    }],
                    null,
                    ['Copy', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'copy');
                    }],
                    null,
                    ['Copy++', function ($itemScope) {
                        var obj = scope.model;
                        var lobjparent = scope.parentModel;
                        CutOrCopyItem(obj, lobjparent, 'copy++');
                    }],
                    null,
                    ['Paste', function ($itemScope) {
                        var obj = scope.model;
                        PasteItem(obj);
                    }, function ($itemScope) {
                        var obj = scope.model;
                        if (cutorcopyitem) {
                            if (isChildOfCutItem(cutorcopyitem, obj) && Param === 'cut') {
                                return false;
                            }
                            else if (scope.model == cutorcopyitem && Param === 'cut') {
                                return false;
                            }
                            else {
                                return true;
                            }
                        }
                        return true;
                    }],
                    null,
                    ['Toggle All Outlining', function ($itemScope) {
                        if ($itemScope.model) {
                            ToggleAllOutLining($("div[node-id='" + $itemScope.model.dictAttributes.sfwNodeID + "']").parents(".logical-rule-first-ul").first());
                        }
                    }],
                    null,
                    ['Comment', function ($itemScope) {
                        var obj = scope.model;
                        $rootScope.EditPropertyValue(obj.dictAttributes.sfwCommented, obj.dictAttributes, "sfwCommented", "True");
                    }, function ($itemScope) {
                        return $itemScope.model.dictAttributes.sfwCommented != "True" && $itemScope.model.Name != "case" && $itemScope.model.Name != "default";
                    }
                    ],
                    null,

                    ['Uncomment', function ($itemScope) {
                        var obj = scope.model;
                        $rootScope.EditPropertyValue(obj.dictAttributes.sfwCommented, obj.dictAttributes, "sfwCommented", "False");
                    }, function ($itemScope) {
                        return $itemScope.model.dictAttributes.sfwCommented == "True" && $itemScope.model.Name != "case" && $itemScope.model.Name != "default";
                    }],
                    null
                ];

                if (scope.model.IsSelected) {
                    scope.onStepSelectChange(scope.model);
                }
            };
            scope.getCssClass = function (model) {
                var cssClass = "";
                if (model.dictAttributes.sfwCommented === 'True') {
                    cssClass = "commented-node";
                }
                if (model.IsSelected) {
                    cssClass += " selected-node";
                    if (model.isAdvanceSearched) {
                        cssClass += ' bckgGreen';
                    }
                }
                else if (model.isAdvanceSearched) {
                    cssClass += ' bckgGrey';
                }
                return cssClass;
            }
            scope.init();

            var parentNode = element.parent();
            if (parentNode) {
                var ul = $(scope.getTemplate());
                $compile(ul)(scope);
                element.remove();
                parentNode.append(ul);

                if (scope.model.Name === "foreach" || scope.model.Name === "while") {
                    $timeout(function () {
                        var parentLoopsCount = ul.parents(".ul-children-even,.ul-children-odd").length;
                        if (parentLoopsCount > 0 && parentLoopsCount % 2 == 1) {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-even");
                        }
                        else {
                            ul.find("> .node-wrapper > .ul-children").addClass("ul-children-odd");
                        }
                    });
                }

            }
        },
        //templateUrl: "Rule/views/LogicalRule/NodeWrapperTemplate.html"
    };
}]);
function checkAndCollapseNode(nodeId) {
    var node = $("div[node-id='" + nodeId + "']");
    if (node && node.length > 0) {
        if (node.find("> .node-expander").hasClass("node-collapsed")) {
            collapseNode(node, true);
        }
    }
}
function expandNode(element, skipAnimation) {
    $(element).parents("li").first().find("> div > .node-wrapper > ul").show();
    if (skipAnimation) {
        $(element).parents("li").first().find("> div > ul").show();
        $(element).parents("li").first().find("> div > .node-wrapper > ul > li").show();
    }
    else {
        $(element).parents("li").first().find("> div > ul").slideDown("slow");
        $(element).parents("li").first().find("> div > .node-wrapper > ul > li").slideDown("slow");
    }
    $(element).parents("li").first().find("> div > .node-wrapper > ul").css("min-height", "");
    $(element).siblings(".node-wrapper").addClass("node-wrapper-after");
    $(element).removeClass("node-collapsed");
    $(element).addClass("node-expanded");
};
function collapseNode(element, skipAnimation) {
    $(element).parents("li").first().find("> div > .node-wrapper > ul").css("min-height", "0px");
    if (skipAnimation) {
        $(element).parents("li").first().find("> div > ul").hide();
        $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
        $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
    }
    else {
        $(element).parents("li").first().find("> div > ul").slideUp("slow", function () {
            $(element).parents("li").first().find("> div > ul").hide();
            $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
        });
        if ($(element).parents("li").first().find("> div > .node-wrapper > ul > li").length > 0) {
            $(element).parents("li").first().find("> div > .node-wrapper > ul > li").slideUp("slow", function () {
                $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
                $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
            });
        }
        else {
            $(element).parents("li").first().find("> div > .node-wrapper > ul").hide();
            $(element).siblings(".node-wrapper").removeClass("node-wrapper-after");
        }
    }
    $(element).removeClass("node-expanded");
    $(element).addClass("node-collapsed");
};

app.directive('actionsNode', ["$rootScope", "$timeout", function ($rootScope, $timeout) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.actionIndex = null;
            scope.setCurrentAction = function (index) {
                scope.actionIndex = index;
            };

            scope.onkeydownevent = function (event) {
                if (scope.model.Name == "actions") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.ENTER) {

                        var nodeid = generateUUID();
                        var obj = { Elements: [], dictAttributes: { sfwNodeID: nodeid, sfwExpression: "" }, Children: [], IsSelected: false, Name: "action", Value: "" };
                        if (scope.actionIndex || scope.actionIndex === 0) {
                            $rootScope.InsertItem(obj, scope.model.Elements, scope.actionIndex + 1);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.model.Elements);
                        }

                        $timeout(function () {
                            enableExpressionEditing(scope.model.dictAttributes.sfwNodeID, scope.actionIndex + 1);
                        });

                        //stop propagation so that same event won't be called for parent action.
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        }

                        //Due to above statement, it won't call keydown event for body and so in case of Ctrl+S it won't prevent default action.
                        //So explicitely prevent default action so that if user press Ctrl+S on action node, it won't show default save dialog.
                        if (event.preventDefault) {
                            event.preventDefault();
                        }
                    }
                    else if (event.ctrlKey && event.keyCode == $.ui.keyCode.DELETE) {
                        $(".page-header-fixed").css("pointer-events", "auto");
                        if (scope.model.Elements.length > 1) {
                            if (scope.actionIndex || scope.actionIndex === 0) {
                                $rootScope.DeleteItem(scope.model.Elements[scope.actionIndex], scope.model.Elements);
                            }
                            else {
                                $rootScope.DeleteItem(scope.model.Elements[scope.model.Elements.length - 1], scope.model.Elements);
                            }

                            if (scope.actionIndex > scope.model.Elements.length) {
                                scope.actionIndex--;
                            }
                        }

                        //stop propagation so that same event won't be called for parent action.
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        }
                        //Due to above statement, it won't call keydown event for body and so in case of Ctrl+S it won't prevent default action.
                        //So explicitely prevent default action so that if user press Ctrl+S on action node, it won't show default save dialog.
                        if (event.preventDefault) {
                            event.preventDefault();
                        }
                    }
                }
            };

        },
        templateUrl: "Rule/views/LogicalRule/ActionsNodeTemplate.html",
    };
}]);
app.directive('breakContinueDefaultNode', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Rule/views/LogicalRule/BreakContinueDefaultNodeTemplate.html",
    };
}]);
app.directive('callRuleNode', ["$rootScope", "$EntityIntellisenseFactory", function ($rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onRuleIDKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var input = eargs.target;
                //var arrText = getSplitArray($(input).val(), input.selectionStart);
                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));
                //if (arrText[arrText.length - 1] == "") {
                //    arrText.pop();
                //}
                rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);

                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                else {
                    entityID = rootScope.objLogicalRule.Entity;
                }

                var ruleType;
                if (scope.model.Name == "calllogicalrule") {
                    ruleType = "LogicalRule";
                }
                else if (scope.model.Name == "calldecisiontable") {
                    ruleType = "DecisionTable";
                }
                else if (scope.model.Name == "callexcelmatrix") {
                    ruleType = "ExcelMatrix";
                }

                //Get all the entities which have static rule of the specified rule type.
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var requiredEntities = entities.filter(function (x) { return x.Rules.some(function (element, index, array) { return element.IsStatic && (x.ID == entityID || !element.IsPrivate) && element.RuleType == ruleType; }); });

                var rules = [];
                var lstObject = [];
                //If current rule is non-static then load other non-static rules of the current entity.
                var isstatic = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    isstatic = rootScope.objLogicalRule.dictAttributes.sfwStatic;
                }
                else {
                    isstatic = rootScope.objLogicalRule.Static;
                }

                var lstRulesAndObject = [];
                if (isstatic.toLowerCase() != "true") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityname = entityID;
                    while (parententityname != "") {
                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            rules = entity[0].Rules.filter(function (y) { return !y.IsStatic && (parententityname == entityID || !y.IsPrivate) && y.RuleType == ruleType; });
                            lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                            if (entityID != parententityname) {
                                rules = rules.concat(lstObject);
                            }
                            parententityname = entity[0].ParentId;
                            lstRulesAndObject = lstRulesAndObject.concat(rules);
                        } else {
                            parententityname = "";
                        }
                    }
                }
                var data = requiredEntities.concat(lstRulesAndObject);
                data = data.concat(rootScope.lstSelectedAliasobjCollection);
                data = data.concat(rootScope.getVarParamAttributes(["Object"], true, entityID));

                var aliasArray = [];
                var arrayindex = 0;
                var isAlias = false;
                if (arrText.length > 1) {
                    for (var i = 0; i < arrText.length; i++) {
                        for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                            if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                arrayindex = i;
                                isAlias = true;
                                arrText.splice(i, 1);
                                aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                break;
                            }
                        }
                    }
                }

                for (var k = aliasArray.length - 1; k >= 0; k--) {
                    arrText.splice(arrayindex, 0, aliasArray[k]);
                }

                if (!isAlias) {

                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0 && typeof item[0].Rules != "undefined" && index < arrText.length) {
                                if (item[0].ID == arrText[index]) {
                                    data = item[0].Rules.filter(function (y) { return y.IsStatic && (item[0].ID == entityID || !y.IsPrivate) && y.RuleType == ruleType; });
                                }
                            }
                            else if (item.length > 0 && item[0].DataType != undefined && index < arrText.length) {
                                if (item[0].DataType == "Object" && item[0].ID == arrText[index]) {
                                    parententityname = item[0].Entity;
                                    data = [];
                                    while (parententityname != "") {
                                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                                        if (entity && entity.length > 0) {
                                            data = data.concat(entity[0].Rules.filter(function (y) { return !y.IsStatic && !y.IsPrivate && y.RuleType == ruleType; }));
                                            lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                                            data = data.concat(lstObject);
                                            parententityname = entity[0].ParentId;
                                        } else {
                                            parententityname = "";
                                        }
                                    }
                                }
                            }
                            else if (item.length > 0 && item[0].RuleType != undefined && index < arrText.length) {
                                if (item[0].RuleType == "LogicalRule" || item[0].RuleType == "DecisionTable" || item[0].RuleType == "ExcelMatrix") {
                                    data = [];
                                }
                            } else {
                                if (index != arrText.length - 1) {
                                    data = [];
                                }
                            }
                        }
                    }
                }
                else {
                    data = scope.GetAliasData(arrText, rootScope, ruleType);
                }

                // filtering rules 
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }


                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.onCallRuleDoubleClick = function () {
                var RuleID = scope.model.dictAttributes.sfwRuleID;
                if (RuleID != undefined) {
                    var lastdot = RuleID.lastIndexOf(".");
                    RuleID = RuleID.substring(lastdot + 1);
                    $.connection.hubMain.server.navigateToFile(RuleID, "").done(function (objfile) {
                        $rootScope.openFile(objfile);
                    });
                }
            };
            scope.getExpressionValue = function (parameter, oldParams) {
                var Expression = "";
                var lst = oldParams.filter(function (x) { return x.dictAttributes.ID == parameter.dictAttributes.ID && x.dictAttributes.sfwDataType == parameter.dictAttributes.sfwDataType && x.dictAttributes.sfwDirection == parameter.dictAttributes.sfwDirection; });
                if (lst && lst.length > 0) {
                    Expression = lst[0].dictAttributes.sfwExpression;
                }
                return Expression;
            };
            scope.refreshRuleParameters = function (event) {
                var rootScope = getCurrentFileScope();
                if (rootScope && rootScope.currentfile.FileName) {
                    $rootScope.ClearUndoRedoListByFileName(rootScope.currentfile.FileName);
                }
                var RuleName = scope.model.dictAttributes.sfwRuleID;
                var RuleID = "";
                var arrayText = [];
                if (!RuleName) {
                    RuleName = "";
                }
                if (RuleName.contains(".")) {
                    arrayText = RuleName.split(".");
                }
                if (arrayText.length > 0) {
                    RuleID = arrayText[arrayText.length - 1];
                }
                else {
                    RuleID = RuleName;
                }

                var parameters;
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        parameters = item;
                    }
                });
                $rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    scope.parameters = parameters;
                    $rootScope.PushItem(parameters, scope.model.Elements);
                    // scope.model.Elements.push(parameters);
                }
                var oldParams = [];
                angular.forEach(parameters.Elements, function (param) {
                    oldParams.push(param);
                });
                parameters.Elements = [];
                var isFound = false;
                if (RuleID != "") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < entityIntellisenseList.length; i++) {
                        if (entityIntellisenseList[i].Rules.length > 0) {
                            for (var j = 0; j < entityIntellisenseList[i].Rules.length; j++) {
                                if (entityIntellisenseList[i].Rules[j].ID == RuleID) {

                                    intellisenseruleobject = entityIntellisenseList[i].Rules[j];
                                    if (intellisenseruleobject != undefined) {
                                        angular.forEach(intellisenseruleobject.Parameters, function (param) {
                                            var parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType, sfwDirection: param.Direction }, Elements: [], Children: [] };
                                            parameter.dictAttributes.sfwExpression = scope.getExpressionValue(parameter, oldParams);
                                            if (param.DataType === "Collection" || param.DataType === "List" | param.DataType === "Object") {
                                                parameter.dictAttributes.sfwEntity = param.Entity;
                                            }
                                            parameter.dictAttributes.sfwNodeID = generateUUID();

                                            $rootScope.PushItem(parameter, parameters.Elements);
                                            //parameters.Elements.push(parameter);
                                        });
                                    }
                                    isFound = true;
                                    break;
                                }
                            }
                        }
                        if (isFound) {
                            break;
                        }
                    }
                }
                $rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);

            };
        },
        templateUrl: "Rule/views/LogicalRule/CallRuleNodeTemplate.html",
    };
}]);
app.directive('forEachNode', ["$rootScope", "$EntityIntellisenseFactory", function ($rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onLoopCollectionkeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var data = [];
                var input = eargs.target;
                //checking for alias objects
                rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);

                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var entityName = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                else {
                    entityName = rootScope.objLogicalRule.Entity;
                }
                var entity = entities.filter(function (x) { return x.ID == entityName; });
                var currentopenentity = "";
                if (entity && entity.length > 0) {
                    currentopenentity = entity[0];
                }
                data = data.concat(rootScope.getVarParamAttributes(["CDOCollection", "Collection", "List", "Object"], true, entityName));
                if (rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "false") {
                    if (entity && entity.length > 0) {
                        var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                        data = data.concat(queries);
                    }

                    //If current rule is non-static then load other non-static rules of the current entity.
                    var isstatic = "";
                    if (rootScope.objLogicalRule.dictAttributes) {
                        isstatic = rootScope.objLogicalRule.dictAttributes.sfwStatic;
                    }
                    else {
                        isstatic = rootScope.objLogicalRule.Static;
                    }

                    if (entity && entity.length > 0 && entity[0] && entity[0].ParentId != "" && isstatic.toLowerCase() != "true") {
                        parententityname = entity[0].ParentId;
                        while (parententityname != "") {
                            entity = entities.filter(function (x) { return x.ID == parententityname; });
                            if (entity && entity.length > 0) {
                                var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery"; });
                                data = data.concat(queries);
                            }
                            var attributes = $rootScope.getEntityAttributeIntellisense(parententityname, false);

                            attributes = attributes.filter(function (y) { return (y.DataType == "Collection" || y.DataType == "CDOCollection" || y.DataType == "Object" || y.DataType == "List"); });
                            data = data.concat(attributes);
                            if (entity && entity.length > 0) {
                                parententityname = entity[0].ParentId;
                            } else {
                                parententityname = "";
                            }
                        }
                    }
                }

                // Adding alias objects to data
                data = data.concat(rootScope.lstSelectedAliasobjCollection);

                var aliasArray = [];
                var arrayindex = 0;
                var isAlias = false;
                if (arrText.length > 1) {
                    for (var i = 0; i < arrText.length; i++) {
                        for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                            if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                arrayindex = i;
                                isAlias = true;
                                arrText.splice(i, 1);
                                aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                break;
                            }
                        }
                    }
                }

                for (var k = aliasArray.length - 1; k >= 0; k--) {
                    arrText.splice(arrayindex, 0, aliasArray[k]);
                }
                //var datacollection = data;
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        //data = datacollection;
                        //if (arrText[index] != "") {
                        //var item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()) });
                        var item = data.filter(function (x) { return x.ID == arrText[index]; });
                        if (item.length > 0) {
                            if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined" && item[0].ID == arrText[index] && index < arrText.length - 1) {
                                data = [];
                                parententityname = item[0].Entity;
                                while (parententityname != "") {
                                    entity = entities.filter(function (x) { return x.ID == parententityname; });
                                    if (entity && entity.length > 0) {
                                        //var queries = entity[0].Queries.filter(function (x) { return x.QueryType == "SelectQuery" });
                                        //data = data.concat(queries);
                                    }
                                    data = data.concat($rootScope.getEntityAttributeIntellisense(parententityname, false).filter(function (x) { return (x.DataType == "Object" || x.DataType == "Collection" || x.DataType == "CDOCollection" || x.DataType == "List"); }));
                                    if (entity && entity.length > 0) {
                                        parententityname = entity[0].ParentId;
                                    } else {
                                        parententityname = "";
                                    }
                                }
                            }
                            else if (typeof item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && index < arrText.length - 1) {
                                if (item[0].isQueryfieldsLoaded) {
                                    data = item[0].lstQueryFields;
                                } else {
                                    rootScope.getQueryFields(item[0].Entity, item[0].ID);
                                    data = [];
                                }
                            }
                            else {
                                data = item;
                            }
                        } else {
                            if (index != arrText.length - 1) {
                                data = [];
                            }
                        }
                        //}
                    }
                }
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }

                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }

                //else {
                //    //data = [];
                //    setRuleIntellisense($(input), data, scope);
                //    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", null);
                //}
            };
        },
        templateUrl: "Rule/views/LogicalRule/ForEachNodeTemplate.html",
    };
}]);
app.directive('methodNode', ["$rootScope", "$EntityIntellisenseFactory", "$IentBaseMethodsFactory", function ($rootScope, $EntityIntellisenseFactory, $IentBaseMethodsFactory) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onMethodNameKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                var input = eargs.target;
                //var arrText = getSplitArray($(input).val(), input.selectionStart);
                var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));

                if (arrText[arrText.length - 1] == "") {
                    arrText.pop();
                }

                var data = [];
                //rootScope.businessObjectMethods;
                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var parententityname = entityID;
                while (parententityname != "") {
                    var entity = entities.filter(function (x) { return x.ID == parententityname; });
                    if (entity && entity.length > 0 && entity[0].BusinessObjectName) {
                        var lstObjectMethods = [];
                        lstObjectMethods = entity[0].ObjectMethods.filter(function (item) { return item.ID; });
                        data = data.concat(lstObjectMethods);
                        parententityname = entity[0].ParentId;
                    } else {
                        parententityname = "";
                        var ientBaseMethods = $IentBaseMethodsFactory.getIentBaseMethods();
                        if (ientBaseMethods && ientBaseMethods.length) {
                            data = data.concat(ientBaseMethods);
                        }
                    }
                }

                // filtering method 
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                    }
                    data = item;
                }
                if (input.innerText.contains(".")) {
                    data = [];
                }

                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.getExpressionValue = function (parameter, oldParams) {
                var Expression = "";
                var lst = oldParams.filter(function (x) { return x.dictAttributes.ID == parameter.dictAttributes.ID });
                if (lst && lst.length > 0) {
                    Expression = lst[0].dictAttributes.sfwExpression;
                }
                return Expression;
            };
            scope.refreshMethodParameters = function (event) {
                var MethodName = scope.model.dictAttributes.sfwMethodName;
                var parameters;
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        parameters = item;
                    }
                });
                var rootScope = getCurrentFileScope();
                $rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    $rootScope.PushItem(parameters, scope.model.Elements);

                    // scope.model.Elements.push(parameters);
                }
                var oldParams = [];
                angular.forEach(parameters.Elements, function (param) {
                    oldParams.push(param);
                });
                parameters.Elements = [];

                var lstMethod = [];
                var entityID = "";
                if (rootScope.objLogicalRule.dictAttributes) {
                    entityID = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList;
                var entity = entities.filter(function (x) {
                    return x.ID == entityID;
                });

                if (entity && entity.length > 0 && entity[0].BusinessObjectName) {
                    lstMethod = entity[0].ObjectMethods;
                } else {
                    var ientBaseMethods = $IentBaseMethodsFactory.getIentBaseMethods();
                    if (ientBaseMethods && ientBaseMethods.length) {
                        lstMethod = lstMethod.concat(ientBaseMethods);
                    }
                }
                for (var i = 0; i < lstMethod.length; i++) {
                    if (lstMethod[i].ID == MethodName) {
                        var intellisenseruleobject = lstMethod[i];
                        angular.forEach(intellisenseruleobject.Parameters, function (param) {
                            var parameter = null;
                            if (intellisenseruleobject.MethodName) {
                                parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ParameterName, sfwDataType: param.ParameterType.Name, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                            } else {
                                parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType, sfwEntity: param.Entity, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                            }
                            parameter.dictAttributes.sfwExpression = scope.getExpressionValue(parameter, oldParams);
                            $rootScope.PushItem(parameter, parameters.Elements);

                            //parameters.Elements.push(parameter);
                        });
                        break;
                    }
                }
                $rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);
            };
        },
        templateUrl: "Rule/views/LogicalRule/MethodNodeTemplate.html",
    };
}]);
app.directive('notesNode', ["$rootScope", function ($rootScope) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.editNotes = function (notesobj) {
                var objele = null;
                if (notesobj.Elements.length == 0) {
                    objele = { Children: [], Elements: [], Name: "Text", Value: "", dictAttributes: {} };
                    notesobj.Elements.push(objele);
                }
                else {
                    objele = notesobj.Elements[0];
                }
                var newscope = scope.$new();
                var data = JSON.stringify(objele);
                var objnotes = JSON.parse(data);
                newscope.objText = objnotes;
                newscope.dialog = $rootScope.showDialog(newscope, "Edit Notes", "Rule/views/EditNotesTemplate.html", { width: 400, height: 400 });
                newscope.SaveChangedTextData = function () {
                    $rootScope.EditPropertyValue(objele.Value, objele, "Value", newscope.objText.Value);
                    objele.IsValueInCDATAFormat = true;
                    newscope.onCancelClick();
                };

                newscope.onCancelClick = function () {
                    if (newscope.dialog) {
                        newscope.dialog.close();
                    }
                };
            };
        },
        templateUrl: "Rule/views/LogicalRule/NotesNodeTemplate.html",
    };
}]);
app.directive('queryNode', ["$rootScope", "$EntityIntellisenseFactory", "$NavigateToFileService", function ($rootScope, $EntityIntellisenseFactory, $NavigateToFileService) {
    return {
        restrict: "E",
        replace: true,
        link: function (scope, element, attrs) {
            scope.onQueryIDKeyDown = function (eargs) {
                var rootScope = getCurrentFileScope();
                // if (rootScope.objLogicalRule.dictAttributes.sfwStatic.toLowerCase() == "false") 
                {
                    var input = eargs.target;
                    rootScope.GetAliasObjects(rootScope.objSelectedLogicalRule, true);
                    rootScope.CheckForLoopAndGetLoopParameters(scope.SelectedNode, true, rootScope.objSelectedLogicalRule);
                    var arrText = getSplitArray(input.innerText, getCaretCharacterOffsetWithin(input));

                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var entityName = "";
                    if (rootScope.objLogicalRule.dictAttributes) {
                        entityName = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                    }
                    else {
                        entityName = rootScope.objLogicalRule.Entity;
                    }
                    var entity = entities.filter(function (x) { return x.ID == entityName; });
                    var currentopenentity = "";
                    if (entity[0]) {
                        currentopenentity = entity[0];
                    } else {
                        currentopenentity = "";
                    }

                    var parententityname = entityName;
                    var data = [];

                    if (parententityname != "") {
                        entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            data = data.concat(entity[0].Queries);
                        }
                    }

                    //We are showing only current entity query

                    //while (parententityname != "") {
                    //    entity = entities.filter(function (x) { return x.ID == parententityname });
                    //    if (entity.length > 0) {
                    //        data = data.concat(entity[0].Queries);
                    //    }
                    //    var lstobjects = [];
                    //    if (entity[0]) {
                    //        lstobjects = entity[0].Attributes.filter(function (x) { return x.DataType == "Object" });
                    //        data = data.concat(lstobjects);
                    //        parententityname = entity[0].ParentId;
                    //    } else {
                    //        parententityname = "";
                    //    }
                    //}
                    data = data.concat(rootScope.lstSelectedAliasobjCollection);

                    var aliasArray = [];
                    var arrayindex = 0;
                    var isAlias = false;
                    if (arrText.length > 1) {
                        for (var i = 0; i < arrText.length; i++) {
                            for (var j = 0; j < rootScope.lstSelectedAliasobjCollection.length; j++) {
                                if (arrText[i] == rootScope.lstSelectedAliasobjCollection[j].ID) {
                                    arrayindex = i;
                                    isAlias = true;
                                    arrText.splice(i, 1);
                                    aliasArray = rootScope.lstSelectedAliasobjCollection[j].AliasName.split(".");
                                    break;
                                }
                            }
                        }
                    }

                    for (var k = aliasArray.length - 1; k >= 0; k--) {
                        arrText.splice(arrayindex, 0, aliasArray[k]);
                    }
                    if (!isAlias) {
                        if (arrText.length > 0) {
                            for (var index = 0; index < arrText.length; index++) {
                                //data = datacollection;
                                //if (arrText[index] != "") {
                                //var item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()) });
                                var item = data.filter(function (x) { return x.ID == arrText[index]; });
                                if (item.length > 0) {
                                    if (typeof item[0].DataType != "undefined" && item[0].DataType != "Object" && item[0].DataType != "Collection" && item[0].DataType != "CDOCollection" && item[0].DataType != "List" && index < arrText.length - 1) {

                                        if (item[0].isQueryfieldsLoaded) {
                                            data = item[0].lstQueryFields;
                                        } else {
                                            rootScope.getQueryFields(item[0].Entity, item[0].ID);
                                            data = [];
                                        }
                                    }
                                    else if (item[0].DataType != "undefined" && item[0].DataType == "Object") {
                                        var parententityname = item[0].Entity;
                                        var data = [];
                                        while (parententityname != "") {
                                            entity = entities.filter(function (x) { return x.ID == parententityname; });
                                            if (entity && entity.length > 0) {
                                                data = data.concat(entity[0].Queries);
                                            }
                                            var lstobjects = [];
                                            if (entity && entity.length > 0) {
                                                lstobjects = entity[0].Attributes.filter(function (x) { return x.DataType == "Object"; });
                                                data = data.concat(lstobjects);
                                                parententityname = entity[0].ParentId;
                                            } else {
                                                parententityname = "";
                                            }
                                        }
                                        data = data.concat(rootScope.lstSelectedAliasobjCollection);
                                    }
                                    else {
                                        data = item;
                                    }
                                } else {
                                    if (index != arrText.length - 1) {
                                        data = [];
                                    }
                                }
                                //}
                            }
                        }
                    }
                    else {
                        data = scope.GetAliasData(arrText, rootScope);
                    }

                    // filtering query 
                    var item = [];
                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            item = data.filter(function (x) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); });
                        }
                        data = item;
                    }
                    setRuleIntellisense($(input), data, scope);

                    if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        eargs.preventDefault();
                    }
                }
                //else {
                //    data = [];
                //    setRuleIntellisense($(input), data, scope);
                //    if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", null);
                //}
            };
            scope.navigateToQuery = function (item) {
                var currentFileScope = getCurrentFileScope();
                var queryID = item.dictAttributes.sfwQueryID;
                if (queryID && queryID != "") {
                    if (currentFileScope.objLogicalRule.dictAttributes) {
                        $NavigateToFileService.NavigateToFile(currentFileScope.objLogicalRule.dictAttributes.sfwEntity, "queries", queryID);
                    }
                    else {
                        $NavigateToFileService.NavigateToFile(currentFileScope.objLogicalRule.Entity, "queries", queryID);
                    }
                }

            };
            scope.refreshQueryParameters = function (event) {
                var QueryID = scope.model.dictAttributes.sfwQueryID;
                var parameters;
                var tempParametersList = [];
                angular.forEach(scope.model.Elements, function (item) {
                    if (item.Name == "parameters") {
                        var param = { Children: item.Children, dictAttributes: item.dictAttributes, IsSelected: item.IsSelected, Name: item.Name, value: item.value, Elements: item.Elements };
                        parameters = item;
                        tempParametersList = param;
                    }
                });

                var entityid = "";

                var rootScope = getCurrentFileScope();

                if (rootScope.objLogicalRule.dictAttributes) {
                    entityid = rootScope.objLogicalRule.dictAttributes.sfwEntity;
                }
                else {
                    entityid = rootScope.objLogicalRule.Entity;
                }
                //$rootScope.UndRedoBulkOp("Start");
                if (parameters == undefined) {
                    var parameters = { Name: "parameters", value: '', dictAttributes: {}, Elements: [], Children: [] };
                    scope.model.Elements.push(parameters);
                    //$rootScope.PushItem(parameters, scope.model.Elements);
                }

                parameters.Elements = [];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                for (var i = 0; i < entityIntellisenseList.length; i++) {
                    if (entityIntellisenseList[i].ID == entityid) {
                        if (entityIntellisenseList[i].Queries.length > 0) {
                            for (var j = 0; j < entityIntellisenseList[i].Queries.length; j++) {
                                if (entityIntellisenseList[i].Queries[j].ID == QueryID) {

                                    intellisenseruleobject = entityIntellisenseList[i].Queries[j];
                                    if (intellisenseruleobject != undefined) {
                                        angular.forEach(intellisenseruleobject.Parameters, function (param) {

                                            var parameter = { Name: "parameter", value: '', dictAttributes: { ID: param.ID, sfwDataType: param.DataType }, Elements: [], Children: [] };

                                            angular.forEach(tempParametersList.Elements, function (item) {
                                                if (item.dictAttributes.ID == param.ID) {
                                                    parameter.dictAttributes.sfwExpression = item.dictAttributes.sfwExpression;

                                                }

                                            });
                                            //$rootScope.PushItem(parameter, parameters.Elements);
                                            parameters.Elements.push(parameter);
                                            // parameters.Elements.push(parameter);
                                        });
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                //$rootScope.UndRedoBulkOp("End");

                var nodeElement = $(event.target).parents(".node").first();
                scope.syncParameterVisibility(nodeElement);
            };
        },
        templateUrl: "Rule/views/LogicalRule/QueryNodeTemplate.html",
    };
}]);
app.directive('returnSwitchCaseWhileNode', [function () {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "Rule/views/LogicalRule/ReturnSwitchCaseWhileNodeTemplate.html",
    };
}]);

app.directive('logicalRuleEditable', [function () {
    return {
        restrict: "A",
        link: function (scope, element, attrs) {
            element.dblclick(function (event) {
                if ($(event.target).attr("contenteditable") !== "true") {
                    enableEditing(this);
                }
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            });
            element.on("keyup", function (event) {
                if (event.which == 27) {
                    disableEditing(this);
                }
            });
            element.on("blur", function () {
                disableEditing(this);
            });
        },
    };
}]);
function setSelectionByCharacterOffsets(containerEl, start, end) {
    if (window.getSelection && document.createRange) {
        var charIndex = 0, range = document.createRange();
        range.setStart(containerEl, 0);
        range.collapse(true);
        var nodeStack = [containerEl], node, foundStart = false, stop = false;

        while (!stop && (node = nodeStack.pop())) {
            if (node.nodeType == 3) {
                var nextCharIndex = charIndex + node.length;
                if (!foundStart && start >= charIndex && start <= nextCharIndex) {
                    range.setStart(node, start - charIndex);
                    foundStart = true;
                }
                if (foundStart && end >= charIndex && end <= nextCharIndex) {
                    range.setEnd(node, end - charIndex);
                    stop = true;
                }
                charIndex = nextCharIndex;
            }
            else {
                var i = node.childNodes.length;
                while (i--) {
                    nodeStack.push(node.childNodes[i]);
                }
            }
        }

        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);

    }
    else if (document.selection) {
        var textRange = document.body.createTextRange();
        textRange.moveToElementText(containerEl);
        textRange.collapse(true);
        textRange.moveEnd("character", end);
        textRange.moveStart("character", start);
        textRange.select();
    }
}
function disableEditing(element) {
    $(element).parents(".node").first().attr("draggable", "true");
    $(element).attr("contenteditable", "false");

}
function enableEditing(element) {
    $(element).parents(".node").first().removeAttr("draggable");
    $(element).attr("contenteditable", "true");

    setSelectionByCharacterOffsets(element, $(element).text().length, $(element).text().length);

    $(element).focus();
}
function enableExpressionEditing(sfwNodeId, index) {
    if (sfwNodeId) {
        if (!index) {
            index = 0;
        }

        var editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-block[logical-rule-editable]");
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-container > .node-expression-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-expression-container > .node-expression-inline[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            var editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-container > .node-header-block[logical-rule-editable]");
        }
        if (editableBlock.length === 0) {
            editableBlock = $("div[node-id='" + sfwNodeId + "']").find("> .node-wrapper > .node > .node-header-container > .node-header-inline[logical-rule-editable]");
        }

        if (editableBlock.length > index) {
            enableEditing(editableBlock[index]);
        }
    }
}

app.directive('logicalRuleDragDrop', ["$timeout", function ($timeout) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            if ($(element).hasClass("logical-rule-container")) {
                var onMouseUpHandler = function (event) {
                    removeDropArea();
                    dragDropData = null;
                }
                $(element).on("mouseup drop", onMouseUpHandler);
            }
            scope.canDrop = function (mouseOver) {
                var retValue = false;
                if (dragDropData && (mouseOver.isOverTop || mouseOver.isOverLeft || mouseOver.isOverRight || mouseOver.isOverBottom || mouseOver.isOverInside)) {
                    retValue = true;
                    if (scope.model) {
                        var dragNodeId = null;
                        var dragNodeName = null;
                        if (dragDropData.length === 1) {
                            dragNodeId = dragDropData[0].dictAttributes.sfwNodeID;
                            dragNodeName = dragDropData[0].Name;
                        }
                        else if (dragDropData.length > 1) {
                            dragNodeId = dragDropData[1];
                            var dragNode = searchNodeByIdInCurrentRule(dragNodeId);
                            if (dragNode) {
                                dragNodeName = dragNode.Name;
                            }
                        }

                        //Don't allow to drop a node around itself. Also don't allow to drop a node on the node after  it.
                        if (dragNodeId === scope.model.dictAttributes.sfwNodeID || (dragNodeId === scope.parentModel.dictAttributes.sfwNodeID && mouseOver.isOverRight === false)) {
                            retValue = false;
                        }
                        //Don't allow to drop a foreach/while/switch node inside itself at any level.
                        else if ((dragNodeName === "foreach" || dragNodeName === "while" || dragNodeName === "switch") && $("div[node-id='" + dragNodeId + "']").find("> .node-wrapper > ul div[node-id='" + scope.model.dictAttributes.sfwNodeID + "']").length > 0) {
                            retValue = false;
                        }

                        //Don't allow to drop a node after a collapsed node.
                        else if ($("div[node-id='" + scope.model.dictAttributes.sfwNodeID + "'] > .node-expander").hasClass("node-collapsed") && mouseOver.isOverRight) {
                            retValue = false;
                        }

                        else if ((scope.model.Name === "case" || scope.model.Name === "default")) {

                            //Don't allow to drop a node to the left side of a case/default node.
                            if (mouseOver.isOverLeft) {
                                retValue = false;
                            }
                            //Don't allow to drop a node to the bottom of a default node.
                            else if (scope.model.Name === "default" && mouseOver.isOverBottom) {
                                retValue = false;
                            }
                            //Don't allow to drop a case/default node to the right of another case/default node.
                            else if ((dragNodeName === "case" || dragNodeName === "default") && mouseOver.isOverRight) {
                                retValue = false;
                            }
                            //Don't allow to drop a node to the top/bottom of a case/default node, if dragged node is not a case/default.
                            else if ((mouseOver.isOverTop || mouseOver.isOverBottom) && (dragNodeName !== "case" && dragNodeName !== "default")) {
                                retValue = false;
                            }
                            //Don't allow to drop a default node to a switch block if it already have a default node.
                            else if (dragNodeName === "default" && scope.parentModel.Elements.filter(function (item) { return item.Name === "default"; }).length > 0) {
                                retValue = false;
                            }
                            //Don't allow to drop a default node to the top of a case node.
                            else if (dragNodeName === "default" && scope.model.Name === "case" && mouseOver.isOverTop) {
                                retValue = false;
                            }
                        }
                        else {

                            //Don't allow to drop a case/default node around any other node.
                            if (dragNodeName === "case" || dragNodeName === "default") {
                                retValue = false;
                            }
                        }
                    }
                }
                return retValue;
            };
            var setInsideDropArea = function (event) {
                var originalEvent = event.clientX ? event : event.originalEvent;
                var bounds = event.currentTarget.getBoundingClientRect();
                var mouseOver = { isOverInside: (originalEvent.clientX > bounds.left && originalEvent.clientX < bounds.right && originalEvent.clientY > bounds.top && originalEvent.clientY < bounds.bottom) };
                if (scope.canDrop(mouseOver)) {
                    var container = $(event.currentTarget).closest(".logical-rule-container");
                    setDropAreaBounds("inside", bounds, container);
                }
            }
            var onDragOverHandler = function (event) {
                //To allow drop over above calculated area.
                if (event.preventDefault) {
                    event.preventDefault();
                }

                if (event.currentTarget.localName == "ul" && $(event.currentTarget).hasClass("ul-children")) {
                    if ($(event.currentTarget).find("> li").length === 0) {
                        setInsideDropArea(event);
                    }
                }
                else if (event.currentTarget.localName === "div") {
                    if ($(event.currentTarget).hasClass("logical-rule-container")) {
                        if ($(event.currentTarget).find("> ul > li").length === 0) {
                            setInsideDropArea(event);
                        }
                    }
                    else {
                        var bounds = getBounds(event.currentTarget);
                        var originalEvent = event.clientX ? event : event.originalEvent;
                        //Calculate the area for drag over in different directions.
                        //for Top drop

                        var mouseOver = checkMousePosition(originalEvent, bounds);

                        //clearDragOverTimeout();
                        if (scope.canDrop(mouseOver)) {
                            var container = $(event.currentTarget).parents(".logical-rule-container");

                            //setDragOverTimeout(function () {

                            removeDropArea();
                            //for Top drop
                            if (mouseOver.isOverTop) {
                                setDropAreaBounds("top", bounds, container);

                                var relatedLi = $(event.currentTarget).prev();
                                if (relatedLi && relatedLi.length > 0) {
                                    var relatedBounds = getBounds(relatedLi[0]);
                                    setDropAreaBounds("verticalfill", relatedBounds, container);
                                }
                            }
                            //for Left drop
                            else if (mouseOver.isOverLeft) {
                                setDropAreaBounds("left", bounds, container);
                            }
                            //for Right drop
                            else if (mouseOver.isOverRight) {
                                setDropAreaBounds("right", bounds, container);
                                setDropAreaBounds("horizontalfill", bounds, container);
                            }
                            //for Bottom drop
                            else if (mouseOver.isOverBottom) {
                                setDropAreaBounds("bottom", bounds, container);
                                setDropAreaBounds("verticalfill", bounds, container);
                            }
                            //});
                        }
                    }
                }
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            };
            $(element).on("dragover", onDragOverHandler);
            var getBounds = function (li) {
                var bounds = { liBounds: undefined, nodeBounds: undefined, elementsUlBounds: undefined, childrenUlBounds: undefined };
                bounds.liBounds = li.getBoundingClientRect();
                bounds.nodeBounds = $(li).find(" > .node-wrapper > .node")[0].getBoundingClientRect();

                //Required for right drop
                var elementsUlBounds;
                var elementsUl = $(li).find("> ul");
                if (elementsUl && elementsUl.length > 0) {
                    bounds.elementsUlBounds = elementsUl[0].getBoundingClientRect();
                }

                //Required for bottom drop
                var childrenUlBounds;
                var childrenUl = $(li).find(" > .node-wrapper > ul");
                if (childrenUl && childrenUl.length > 0) {
                    bounds.childrenUlBounds = childrenUl[0].getBoundingClientRect();
                }

                return bounds;
            };
            var setDropAreaBounds = function (direction, bounds, container) {
                var dropAreaBounds = { left: 0, right: 0, height: 0, width: 0 };
                switch (direction) {
                    case "top":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        dropAreaBounds.top = bounds.liBounds.top - 20;
                        dropAreaBounds.width = bounds.liBounds.width - 50;
                        dropAreaBounds.height = bounds.nodeBounds.top - bounds.liBounds.top + 20;
                        break;
                    case "left":
                        dropAreaBounds.left = bounds.liBounds.left;
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        dropAreaBounds.width = bounds.nodeBounds.left - bounds.liBounds.left;
                        dropAreaBounds.height = bounds.liBounds.height - 40;
                        break;
                    case "right":
                        if (bounds.elementsUlBounds) {
                            dropAreaBounds.left = bounds.elementsUlBounds.left - 30;
                        }
                        else {
                            dropAreaBounds.left = bounds.liBounds.right - 30;
                        }
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        dropAreaBounds.width = 30;
                        dropAreaBounds.height = bounds.liBounds.height - 40;
                        break;
                    case "bottom":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        dropAreaBounds.top = bounds.liBounds.bottom - 20;
                        dropAreaBounds.width = bounds.liBounds.width - 60;
                        dropAreaBounds.height = 40;
                        break;
                    case "horizontalfill":
                        dropAreaBounds.left = bounds.nodeBounds.right;
                        dropAreaBounds.top = bounds.nodeBounds.top;
                        if (bounds.elementsUlBounds) {
                            dropAreaBounds.width = bounds.elementsUlBounds.left - 30 - dropAreaBounds.left;
                        }
                        else {
                            dropAreaBounds.width = bounds.liBounds.right - 30 - dropAreaBounds.left;
                        }
                        dropAreaBounds.height = bounds.nodeBounds.height;
                        break;
                    case "verticalfill":
                        dropAreaBounds.left = bounds.nodeBounds.left;
                        if (bounds.childrenUlBounds) {
                            dropAreaBounds.top = bounds.childrenUlBounds.bottom;
                            dropAreaBounds.width = bounds.childrenUlBounds.width + (bounds.childrenUlBounds.left - bounds.nodeBounds.left);
                        }
                        else {
                            dropAreaBounds.top = bounds.nodeBounds.bottom;
                            dropAreaBounds.width = bounds.nodeBounds.width;
                        }
                        dropAreaBounds.height = bounds.liBounds.bottom - 20 - dropAreaBounds.top;
                        break;
                    case "inside":
                        dropAreaBounds.left = bounds.left;
                        dropAreaBounds.top = bounds.top;
                        dropAreaBounds.width = bounds.width;
                        dropAreaBounds.height = bounds.height;
                        break;
                }

                //As we are adding drop area in the rule container only, so we will have to adjust the top and left as per container's 
                //offset and scrolloffset
                var containerBounds = $(container)[0].getBoundingClientRect();
                dropAreaBounds.left = dropAreaBounds.left - containerBounds.left + $(container).scrollLeft();
                dropAreaBounds.top = dropAreaBounds.top - containerBounds.top + $(container).scrollTop();

                addDropArea(direction, dropAreaBounds, container);
            };

            var addDropArea = function (direction, dropAreaBounds, container) {
                if (direction === "horizontalfill") {
                    direction = "right";
                }
                else if (direction === "verticalfill") {
                    direction = "bottom";
                }

                var dropAreaDiv = $('<div class="drop-area" ></div>');

                dropAreaDiv.css("left", dropAreaBounds.left);
                dropAreaDiv.css("top", dropAreaBounds.top);
                dropAreaDiv.css("height", dropAreaBounds.height);
                dropAreaDiv.css("width", dropAreaBounds.width);

                dropAreaDiv.on("dragover", function (event) {
                    if (event.preventDefault) {
                        event.preventDefault();
                    }
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                });
                dropAreaDiv.on("dragleave", function (event) {
                    //removeDropArea();
                });
                dropAreaDiv.on("drop", function (event) {
                    if (direction === "inside") {
                        if (attributes.isFirstNode && scope.rule) {
                            addItemOnDrop(dragDropData, [scope.rule.dictAttributes.sfwNodeID, direction], event);
                        }
                        else {
                            addItemOnDrop(dragDropData, [scope.model.dictAttributes.sfwNodeID, direction], event);
                        }
                    }
                    else {
                        addItemOnDrop(dragDropData, [scope.parentModel.dictAttributes.sfwNodeID, scope.model.dictAttributes.sfwNodeID, direction], event);
                    }
                    var dragData = dragDropData;
                    $timeout(function () {
                        if (dragData && dragData.length > 0 && dragData[0].dictAttributes)
                            enableExpressionEditing(dragData[0].dictAttributes.sfwNodeID);
                    });

                    removeDropArea();
                    dragDropData = null;
                });
                $(container).append(dropAreaDiv);
            };

            var removeDropArea = function () {
                $(".drop-area").remove();
            };
            var checkMousePosition = function (event, bounds) {
                var mouseOver = { isOverTop: false, isOverLeft: false, isOverRight: false, isOverBottom: false };
                mouseOver.isOverTop = (event.clientY > bounds.liBounds.top && event.clientY < bounds.nodeBounds.top && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.liBounds.right - 30);
                //for Left drop
                mouseOver.isOverLeft = (event.clientX > bounds.liBounds.left && event.clientX < bounds.nodeBounds.left && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20);

                //for Right drop
                var isOverHorizontalFillArea = ((bounds.elementsUlBounds && (event.clientX < bounds.elementsUlBounds.left - 30 && event.clientX > bounds.nodeBounds.right && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.nodeBounds.bottom)) || (!bounds.elementsUlBounds && (event.clientX < bounds.liBounds.right - 30 && event.clientX > bounds.nodeBounds.right && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.nodeBounds.bottom)));
                var isOverRightEdge = ((bounds.elementsUlBounds && (event.clientX < bounds.elementsUlBounds.left && event.clientX > bounds.elementsUlBounds.left - 30 && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20)) || (!bounds.elementsUlBounds && (event.clientX < bounds.liBounds.right && event.clientX > bounds.liBounds.right - 30 && event.clientY > bounds.nodeBounds.top && event.clientY < bounds.liBounds.bottom - 20)));
                mouseOver.isOverRight = isOverHorizontalFillArea || isOverRightEdge;

                //for Bottom drop
                var isOverVerticalFillArea = (bounds.childrenUlBounds && event.clientY < bounds.liBounds.bottom && event.clientY > bounds.childrenUlBounds.bottom && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.childrenUlBounds.right) || (!bounds.childrenUlBounds && event.clientY < bounds.liBounds.bottom && event.clientY > bounds.nodeBounds.bottom && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.nodeBounds.right);
                var isOverBottomEdge = (event.clientY < bounds.liBounds.bottom && event.clientY > bounds.liBounds.bottom - 20 && event.clientX > bounds.nodeBounds.left && event.clientX < bounds.liBounds.right - 30);
                mouseOver.isOverBottom = isOverVerticalFillArea || isOverBottomEdge;

                return mouseOver;
            };
        }
    };
}]);

app.directive('customdraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
            isruletoolbar: '=',
            isobjectbased: '=',
            displaypath: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                dragDropData = null;
                dragDropDataObject = null;
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData("Text", "");

                if (scope.dragdata) {
                    if (scope.dragdata instanceof Array) {
                        if (canDrag(scope.dragdata)) {
                            dragDropData = scope.dragdata;
                        }
                    } else {
                        dragDropData = scope.dragdata;
                        if (scope.isruletoolbar) {
                            dragDropData = [getNewObject(dragDropData)];
                        } else if (scope.isobjectbased) {
                            dragDropDataObject = scope.dragdata;
                            dragDropData = scope.dragdata.ShortName;
                            if (scope.displaypath) {
                                dragDropData = scope.displaypath + "." + scope.dragdata.ShortName;
                            }
                        }
                    }
                }
            }
        },
    };
}]);

app.directive('customdroppable', ["$rootScope", "$timeout", function ($rootScope, $timeout) {
    return {
        restrict: "A",
        scope: {
            dropdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.addEventListener('dragover', handleDragOver, false);
            el.addEventListener('drop', handleDrop, false);

            function handleDragOver(e) {

                if (canDrop(dragDropData, scope.dropdata)) {
                    if (e.preventDefault) {
                        e.preventDefault(); // Necessary. Allows us to drop.
                    }

                    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                    e.dataTransfer.setData("Text", "");
                    e.preventDefault();
                }
                return false;
            }

            function handleDrop(e) {
                if (e.target.localName == "span" && e.target.contentEditable == "true") {
                    return;
                }
                // Stops some browsers from redirecting.
                //if (e.stopPropagation) e.stopPropagation();
                var data = dragDropData;
                if (data != undefined && data != null && data != "") {
                    addItemOnDrop(data, scope.dropdata, e);
                }

                dragDropData = null;
            }
        }
    };
}]);

function ExpandCollapse(obj) {
    $(obj).parent().siblings().toggle();
    $(obj).siblings("ul").toggle();

    $(obj).toggleClass("logicalrule-expanded logicalrule-collapsed");

}

function searchNodeByIdInCurrentRule(nodeId) {
    var curScope = getCurrentFileScope();
    return searchByNodeId(curScope.objLogicalRule, nodeId);
}

function searchByNodeId(parent, nodeId) {
    var retValue = null;
    if (parent.dictAttributes.sfwNodeID == nodeId) {
        return parent;
    }
    else {
        if (parent.Name == "neorule" || parent.Name == "logicalrule" || parent.Name == "switch" || parent.Name == "foreach" || parent.Name == "while" || parent.Name == "case" || parent.Name == "default") {
            if (parent.Elements.length > 0) {
                for (var index = 0; index < parent.Elements.length; index++) {
                    retValue = searchByNodeId(parent.Elements[index], nodeId);
                    if (retValue != null) {
                        return retValue;
                    }
                }
            }
        }
        if (parent.Name != "parameters" && parent.Name != "ExtraFields" && parent.Name != "variables" && parent.Name != "neorule" && parent.Name != "logicalrule" && parent.Name != "case" && parent.Name != "default" && retValue == null) {
            if (parent.Children.length > 0) {
                for (var index = 0; index < parent.Children.length; index++) {
                    retValue = searchByNodeId(parent.Children[index], nodeId);
                    if (retValue != null) {
                        return retValue;
                    }
                }
            }
        }
    }
    return retValue;
}

function addItemOnDrop(dragdata, dropdata, event) {
    var curScope = getCurrentFileScope();
    if (dragdata == dropdata) {
        return;
    }

    var dragParent = null;
    var dragChild = null;

    if (dragdata.length == 2) {
        dragParent = searchByNodeId(curScope.objLogicalRule, dragdata[0]);
        dragChild = searchByNodeId(curScope.objLogicalRule, dragdata[1]);
    }
    else if (dragdata.length == 1) {
        dragChild = searchByNodeId(curScope.objLogicalRule, dragdata[0]);
        if (dragChild == null) {
            dragChild = dragdata[0];
            if (curScope.objSelectedLogicalRule) {
                curScope.ResetAllvalues(curScope.objSelectedLogicalRule);
            }
            dragChild.IsSelected = true;
        }
    }

    var dropParent = null;
    var dropChild = null;
    var dropDirection = null;

    if (dropdata.length == 3) {
        dropParent = searchByNodeId(curScope.objLogicalRule, dropdata[0]);
        dropChild = searchByNodeId(curScope.objLogicalRule, dropdata[1]);
        dropDirection = dropdata[2];
    }
    else if (dropdata.length == 2) {
        dropChild = searchByNodeId(curScope.objLogicalRule, dropdata[0]);
        dropDirection = dropdata[1];
    }

    curScope.$evalAsync(function () {
        curScope.$root.UndRedoBulkOp("Start");

        if (dropDirection == "top") {
            var dropIndex;
            if (dropParent.Children && dropParent.Children.length > 0) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }
                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild));
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {

                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }
                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild));
                }
            }
        }
        else if (dropDirection == "bottom") {
            var dropIndex;

            if (dropParent.Children != undefined) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }
                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild) + 1);
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {
                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }
                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild) + 1);
                }
            }
        }
        else if (dropDirection == "left") {
            var dropIndex;
            if (dropParent.Children != undefined) {
                dropIndex = dropParent.Children.indexOf(dropChild);
            }
            else {
                dropIndex = -1;
            }
            if (dropIndex > -1) {
                if (dragParent != null) {
                    removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                }

                curScope.$root.InsertItem(dragChild, dropParent.Children, dropParent.Children.indexOf(dropChild));
                curScope.$root.DeleteItem(dropChild, dropParent.Children);
                curScope.$root.PushItem(dropChild, dragChild.Children);
            }
            else {
                var dropIndex = dropParent.Elements.indexOf(dropChild);
                if (dropIndex > -1) {
                    if (dragParent != null) {
                        removeDragItemFromExistingParent(curScope, dragChild, dragParent);
                    }


                    curScope.$root.InsertItem(dragChild, dropParent.Elements, dropParent.Elements.indexOf(dropChild));
                    curScope.$root.DeleteItem(dropChild, dropParent.Elements);
                    curScope.$root.PushItem(dropChild, dragChild.Children);
                }
            }
        }
        else if (dropDirection == "right") {
            if (dragParent != null) {
                removeDragItemFromExistingParent(curScope, dragChild, dragParent);
            }
            if (dropChild.Name == "case" || dropChild.Name == "default") {

                //insert dropChild.Elements to dragChild.Children from top;
                for (var index = dropChild.Elements.length - 1; index >= 0; index--) {
                    var item = dropChild.Elements[index];
                    curScope.$root.DeleteItem(item, dropChild.Elements);
                    curScope.$root.InsertItem(item, dragChild.Children, 0);
                }

                //add dragChild to dropChild.Elements
                curScope.$root.PushItem(dragChild, dropChild.Elements);
            }
            else {

                //insert dropChild.Children to dragChild.Children from top;
                for (var index = dropChild.Children.length - 1; index >= 0; index--) {
                    var item = dropChild.Children[index];
                    curScope.$root.DeleteItem(item, dropChild.Children);
                    curScope.$root.InsertItem(item, dragChild.Children, 0);
                }

                //add dragChild to dropChild.Children
                curScope.$root.PushItem(dragChild, dropChild.Children);
            }
        }
        else if (dropDirection == "inside") {
            if (dragParent != null) {
                removeDragItemFromExistingParent(curScope, dragChild, dragParent);
            }
            curScope.$root.InsertItem(dragChild, dropChild.Elements, 0);
        }
        curScope.$root.UndRedoBulkOp("End");
    });
}

function removeDragItemFromExistingParent(curScope, dragChild, dragParent) {
    var dragParentCollection = getNodeCollection(dragParent, dragChild);
    var dragChildIndex = dragParentCollection.indexOf(dragChild);
    if (dragChildIndex > -1) {
        var dragChildCollection = getNodeCollection(dragChild);
        curScope.$root.DeleteItem(dragChild, dragParentCollection);

        if (dragChild.Name != "case" && dragChild.Name != "default") {
            for (var index = dragChildCollection.length - 1; index >= 0; index--) {
                var item = dragChildCollection[index];
                curScope.$root.DeleteItem(item, dragChildCollection);
                curScope.$root.InsertItem(item, dragParentCollection, dragChildIndex);
            }
        }
    }
}
function getNodeCollection(parentNode, childNode) {
    var retCollection = null;
    if (childNode) {
        if (parentNode.Children && parentNode.Children.indexOf(childNode) > -1) {
            retCollection = parentNode.Children;
        }
        else if (parentNode.Elements && parentNode.Elements.indexOf(childNode) > -1) {
            retCollection = parentNode.Elements;
        }
    }
    else {
        retCollection = parentNode.Name === "case" || parentNode.Name === "default" ? parentNode.Elements : parentNode.Children;
    }
    return retCollection;
}

var dragDropData = null;
var dragDropDataObject = null;
function GetLevel(element, type) {
    var outerClass = "";
    var innerClass = "";
    if (type == "foreach") {
        outerClass = "ul-foreach-outer";
        innerClass = "ul-foreach-inner";
    }
    else if (type == "while") {
        outerClass = "ul-while-outer";
        innerClass = "ul-while-inner";
    }
    else if (type == "condition") {
        outerClass = "ul-condition-outer";
        innerClass = "ul-condition-inner";
    }

    var parents = $(element).parents("." + outerClass + ",." + innerClass);
    if (parents.length > 0) {
        if ($(parents[0]).attr("class").contains(outerClass)) {
            return true;
        }
        else {
            return false;
        }
    }
}

String.prototype.contains = function (str) {
    return this.indexOf(str) > -1;
};


function expandCollapseParameters(e_args) {
    var p = $(e_args.target);
    var ele = p.parents("tr").siblings();
    var table = $(ele[ele.length - 1]).find("table");

    if (p != null) {
        if (p.hasClass("callactivity-arrow-right")) {

            p.removeClass('callactivity-arrow-right');
            p.addClass('callactivity-arrow-down');

            if (table != null) {
                table.removeAttr('style');
            }
        }
        else {

            p.removeClass('callactivity-arrow-down');
            p.addClass('callactivity-arrow-right');

            if (table != null) {
                table.attr('style', 'display:none');
            }
        }
    }
}

function ToggleAllOutLining(element) {
    var expandedNodes = $(element).find(".node-expanded");
    var collapsedNodes = $(element).find(".node-collapsed");

    if (collapsedNodes.length > 0) {
        collapsedNodes.each(function (index, childElement) {
            expandNode(childElement, true);
        });
    }
    else {
        expandedNodes.each(function (index, childElement) {
            collapseNode(childElement, true);
        });
    }
}

function canDrag(dragdata) {
    var retValue = true;
    var rootScope = getCurrentFileScope();
    if (dragdata != undefined && dragdata != null) {
        var dragParent = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
        var dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[1]);


        if (dragParent != null && dragChild != null && dragParent.Name == "switch" && dragChild.Name == "case") {
            var cases = dragParent.Elements.filter(function (x) {
                return x.Name == "case";
            });
            if (cases.length == 1 && cases[0] == dragChild) {
                retValue = false;
            }
        }
    }

    return retValue;
}
function getNewObject(nodeName) {
    var obj = {
        Name: nodeName, Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: []
    };

    if (nodeName == "actions") {
        obj.Elements.push({
            Name: "action", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: [], Children: []
        });
    }
    else if (nodeName == "switch") {
        obj.Elements.push({
            Name: "case", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: []
        });
        obj.Elements.push({
            Name: "default", Value: "", dictAttributes: { sfwNodeID: generateUUID() }, Elements: []
        });
    }

    return obj;
}

function canDrop(dragdata, dropdata) {
    var rootScope = getCurrentFileScope();
    var retValue = true;
    if (dragdata == undefined || dragdata == null || dropdata == undefined || dropdata == null) {
        retValue = false;
    }
    else {
        var dragParent = null;
        var dragChild = null;

        if (dragdata.length == 2) {
            dragParent = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
            dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[1]);
        }
        else if (dragdata.length == 1) {
            dragChild = searchByNodeId(rootScope.objLogicalRule, dragdata[0]);
            if (dragChild == null) {
                dragChild = dragdata[0];
            }
        }

        var dropParent = null;
        var dropChild = null;
        var dropDirection = null;

        if (dropdata.length == 3) {
            dropParent = searchByNodeId(rootScope.objLogicalRule, dropdata[0]);
            dropChild = searchByNodeId(rootScope.objLogicalRule, dropdata[1]);
            dropDirection = dropdata[2];
        }
        else if (dropdata.length == 2) {
            dropChild = searchByNodeId(rootScope.objLogicalRule, dropdata[0]);
            dropDirection = dropdata[1];
        }

        //If an item is dragged and dropped on itself, don't allow.
        if (dragChild == dropChild) {
            return false;
        }

        //If an item is dragged and dropped on any of it's descendents, don't allow.
        var descendents = getDescendents(dragChild);
        if (descendents.indexOf(dropChild) > -1) {
            return false;
        }

        //If item dropped on logical rule canvas area, allow on if canvas is empty.
        if (dropChild.Name == "logicalrule") {
            var elements = dropChild.Elements.filter(function (x) { return x.Name != "variables"; });
            if (elements.length > 0) {
                return false;
            }
        }
        else {

            //if (dropDirection == "top" && (dropParent == null || dropParent.Name != "logicalrule")) {
            //    return false;
            //}

            // Don't allow an item to be dropped above/below a case/default if it is not case/default.
            if ((dropChild.Name == "case" || dropChild.Name == "default") && dropDirection != "right" && !(dragChild.Name == "case" || dragChild.Name == "default")) {
                return false;
            }

            //Don't allow an item to be dropped on switch if it is not case.
            if (dropChild.Name == "switch" && dropDirection == "inside" && dragChild.Name != "case") {
                return false;
            }

            if (dragChild.Name == "case" || dragChild.Name == "default") {

                if (dropParent != null) {

                    //Don't allow to drop a case/default it it is dropped to the right side of an existing case/default or around anyother item inside switch.
                    if (dropParent.Name != "switch" || dropDirection == "right") {
                        return false;
                    }

                    //Don't allow, If a case is dragged and dropped after a default.
                    if (dragChild.Name == "case" && dropChild.Name == "default") {
                        return false;
                    }

                    //Don't allow, if a default is dragged and dropped inside a switch block where already a default exists.
                    if (dragChild.Name == "default") {

                        var defaults = dropParent.Elements.filter(function (x) { return x.Name == "default"; });
                        if (defaults.length > 0) {
                            return false;
                        }
                        else {
                            //Don't allow, if a default is dragged and dropped before a case, it should always be dropped after last case.
                            var cases = dropParent.Elements.filter(function (x) { return x.Name == "case"; });
                            if (cases.indexOf(dropChild) < cases.length - 1) {
                                return false;
                            }
                        }
                    }
                }
                else {
                    //Dont allow to drag and drop a default into switch at top position.
                    if (dropChild.Name == "switch") {
                        if (dragChild.Name == "default") {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
        }
    }

    return retValue;
}

