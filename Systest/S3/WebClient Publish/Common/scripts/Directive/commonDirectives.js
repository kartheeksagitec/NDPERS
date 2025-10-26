
app.directive("openfiledirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: "E",
        scope: {
            curobj: '=',
        },
        replace: true,
        link: function (scope, element, attrs) {
            scope.setclassforopenfile = function (obj) {
                if (obj != undefined) {
                    if (obj.isvisible != undefined) {
                        if (obj.isvisible == true) {
                            return "file-wrapper";
                        }
                        else {
                            return "displaynone";
                        }
                    }
                    else {
                        return "file-wrapper";
                    }
                }
            };
            if (scope.curobj.file != undefined) {
                if (scope.curobj.file.FileType != undefined) {

                    if (scope.curobj.file.FileType == "LogicalRule" || scope.curobj.file.FileType == "DecisionTable" || scope.curobj.file.FileType == "ExcelMatrix") {
                        scope.contentUrl = "Rule/views/RuleCommon.html";
                    }
                    else if (scope.curobj.file.FileType == "ParameterScenario" || scope.curobj.file.FileType == "ObjectScenario" || scope.curobj.file.FileType == "ExcelScenario") {
                        scope.contentUrl = "Scenario/views/Scenario.html";
                    }
                    else if (scope.curobj.file.FileType == "Entity") {
                        scope.contentUrl = "Entity/views/Entity.html";
                    }
                    else if (scope.curobj.file.FileType == "CustomSettings") {
                        scope.contentUrl = "Tools/views/CustomSettings.html";
                    }
                    else if (scope.curobj.file.FileType == "ExecuteQuery") {
                        scope.contentUrl = "Tools/views/ExecuteQuery.html";
                    }
                    else if (scope.curobj.file.FileType == "RuleConstants") {
                        scope.contentUrl = "Constants/Views/Constants.html";
                    }
                    else if (scope.curobj.file.FileType == "ProjectConfiguration") {
                        scope.contentUrl = "Tools/views/ProjectConfiguration.html";
                    }
                    else if (scope.curobj.file.FileName == "RunResults") {
                        scope.contentUrl = "Views/Tools/RunResults.html";
                    }
                    else if (scope.curobj.file.FileType == "AuditLog") {
                        scope.contentUrl = "Tools/views/AuditLog.html";
                    }
                    else if (scope.curobj.file.FileType == "NeoRuleSettings") {
                        scope.contentUrl = "Views/Tools/ConfigureSettings.html";
                    }
                    else if (scope.curobj.file.FileType == "InboundFile" || scope.curobj.file.FileType == "OutboundFile") {
                        scope.contentUrl = "File/views/File.html";
                    }
                    else if (scope.curobj.file.FileType == "Maintenance" || scope.curobj.file.FileType == "Lookup" || scope.curobj.file.FileType == "Wizard" || scope.curobj.file.FileType == "UserControl" || scope.curobj.file.FileType == "Tooltip") {
                        scope.contentUrl = "Form/views/Form.html";

                    }
                    else if (scope.curobj.file.FileType == "FormLinkMaintenance" || scope.curobj.file.FileType == "FormLinkLookup" || scope.curobj.file.FileType == "FormLinkWizard") {

                        scope.contentUrl = "FormLink/views/FormLink.html";

                    }
                    else if (scope.curobj.file.FileType == "Correspondence" || scope.curobj.file.FileType == "PdfCorrespondence") {
                        scope.contentUrl = "Correspondence/views/Correspondence.html";
                    }
                    else if (scope.curobj.file.FileType == "BPMN" || scope.curobj.file.FileType == "BPMTemplate") {
                        scope.contentUrl = "BPM/views/BPMN.html";
                    }
                    else if (scope.curobj.file.FileType == "Report") {
                        scope.contentUrl = "Report/views/Report.html";
                    }
                    else if (scope.curobj.file.FileType == "SearchFiles") {
                        scope.contentUrl = "StartUp/views/SearchFiles.html";
                    }
                    else if (scope.curobj.file.FileType == "WorkflowMap") {
                        scope.contentUrl = "WorkFlow/views/WorkFlowMap.html";
                    }
                    else if (scope.curobj.file.FileType == "VersionBPM") {
                        scope.contentUrl = "BPM/views/BPMVersion.html";
                    }
                    else if (scope.curobj.file.FileType == "ERDiagram") {
                        scope.contentUrl = "Tools/views/NewERDiagram.html";
                    }
                }

            }


        },
        template: '<div id="{{curobj.file.FileName}}" ng-include="contentUrl" ng-class="setclassforopenfile(curobj)" ></div>',
    };
}]);

app.directive('contextMenu', ["$parse", "$q", function ($parse, $q) {

    var contextMenus = [];

    var removeContextMenus = function (level) {
        while (contextMenus.length && (!level || contextMenus.length > level)) {
            contextMenus.pop().remove();
        }
        if (contextMenus.length == 0 && $currentContextMenu) {
            $currentContextMenu.remove();
        }
    };

    var $currentContextMenu = null;

    var renderContextMenu = function ($scope, event, options, model, level) {
        if (!level) {
            level = 0;
        }
        if (!$) {
            var $ = angular.element;
        }
        $(event.currentTarget).addClass('context');
        var $contextMenu = $('<div>');
        if ($currentContextMenu) {
            $contextMenu = $currentContextMenu;
        } else {
            $currentContextMenu = $contextMenu;
        }
        $contextMenu.addClass('dropdown clearfix');
        var $ul = $('<ul class="contex-menu">');
        $ul.addClass('dropdown-menu');
        $ul.attr({
            'role': 'menu'
        });
        $ul.css({
            display: 'block',
            position: 'absolute',
            left: event.pageX + 'px',
            top: event.pageY + 'px',
            "z-index": 10000
        });
        var $promises = [];
        var count = 0;
        var enabledCount = 0;
        angular.forEach(options, function (item, i) {
            var $li = $('<li>');
            if (item === null) {
                $li.addClass('divider');
                count++;
            } else {
                var nestedMenu = angular.isArray(item[1])
                    ? item[1] : angular.isArray(item[2])
                        ? item[2] : angular.isArray(item[3])
                            ? item[3] : null;
                var $a = $('<a>');
                // $a.css("padding-right", "8px");
                $a.attr({
                    tabindex: '-1', href: '#'
                });
                var text = typeof item[0] == 'string' ? item[0] : item[0].call($scope, $scope, event, model);
                $promise = $q.when(text);
                $promises.push($promise);
                $promise.then(function (text) {
                    $a.text(text);
                    if (nestedMenu) {
                        $a.css("cursor", "default");
                        $a.append($('<strong style="font-family:monospace;font-weight:bold;float:right; position:absolute; margin-left:5px;">&gt;</strong>'));
                    }
                    if (item[4] && typeof item[4] === 'string') {
                        $span = $("<span>");
                        $span.css("float", "right");
                        $span.css("margin-left", "10px");
                        $span.text(item[4]);
                        $a.append($span);
                    }
                });
                $li.append($a);

                var enabled = angular.isFunction(item[2]) ? item[2].call($scope, $scope, event, model, text) : true;
                if (enabled) {
                    enabledCount++;
                    var openNestedMenu = function ($event) {
                        removeContextMenus(level + 1);
                        var a = $($ul[0]).outerWidth();

                        var childPopupX = $ul[0].offsetLeft;
                        var ev = {
                            pageX: childPopupX,
                            pageY: $ul[0].offsetTop + $li[0].offsetTop - 3,
                            currentMenuWidth: $ul[0].offsetWidth,
                        };
                        renderContextMenu($scope, ev, nestedMenu, model, level + 1);
                    };
                    $li.on('click', function ($event) {
                        $event.preventDefault();
                        $scope.$apply(function () {
                            if (nestedMenu) {
                                openNestedMenu($event);
                            } else {
                                $(event.currentTarget).removeClass('context');
                                removeContextMenus();
                                item[1].call($scope, $scope, event, model, text);
                            }
                        });
                    });
                    var tempmenu = "";
                    $li.on('mouseover', function ($event, $index) {
                        $scope.$evalAsync(function () {
                            if (nestedMenu && tempmenu != nestedMenu) {
                                tempmenu = nestedMenu;
                                openNestedMenu($event);
                            }
                            else if (!nestedMenu) {
                                while ((contextMenus.length - 1) > level) {
                                    contextMenus.pop().remove();
                                }
                            }
                        });
                    });
                    $li.on('mouseleave', function ($event, $index) {
                        tempmenu = "";
                    });
                } else {
                    $li.on('click', function ($event) {
                        $event.preventDefault();
                    });
                    $li.addClass('disabled');
                    $li.css({
                        display: 'none'
                    });

                    var div = $($ul[0].children);
                    if (count > 0) {
                        div[div.length - 1].className = "";
                    }
                }
            }
            $ul.append($li);
        });
        if (enabledCount > 0) {
            $contextMenu.append($ul);
        }
        //var height = Math.max(
        //    document.body.scrollHeight, document.documentElement.scrollHeight,
        //    document.body.offsetHeight, document.documentElement.offsetHeight,
        //    document.body.clientHeight, document.documentElement.clientHeight
        //);
        var height = $("#main-wrapper").height();
        $contextMenu.css({
            width: '100%',
            height: height + 'px',
            position: 'absolute',
            top: 0,
            left: 0,
            zIndex: 99999999
        });
        $(document).find('body').append($contextMenu);

        //calculate if drop down menu would go out of screen at left or bottom
        // calculation need to be done after element has been added (and all texts are set; thus thepromises)
        // to the DOM the get the actual height
        var levelWidth = 0;
        $q.all($promises).then(function () {
            if (level === 0) {
                var topCoordinate = event.pageY;
                var menuHeight = angular.element($ul[0]).prop('offsetHeight');
                var winHeight = $("#main-wrapper").height();
                if (topCoordinate > menuHeight && winHeight - topCoordinate < menuHeight) {
                    topCoordinate = event.pageY - menuHeight;
                }

                var leftCoordinate = event.pageX;
                var menuWidth = angular.element($ul[0]).prop('offsetWidth');
                var winWidth = event.view.innerWidth;
                if ((leftCoordinate > menuWidth && winWidth - leftCoordinate < menuWidth) || ((leftCoordinate + menuWidth) >= $(window).width())) {
                    leftCoordinate = event.pageX - menuWidth - 20;
                    if ((leftCoordinate + menuWidth) > ($(window).width() - 50)) {
                        leftCoordinate -= 20;
                    }
                }
                var maxHeight = 150;
                if ((winHeight - topCoordinate - 10) > 150) {
                    maxHeight = (winHeight - topCoordinate - 10);
                }
                $ul.css({
                    display: 'block',
                    position: 'absolute',
                    left: leftCoordinate + 'px',
                    top: topCoordinate + 'px',
                    "max-height": maxHeight + 'px',
                    overflow: 'auto'
                });
            }
            else {
                levelWidth = angular.element($ul[0]).prop('offsetWidth');
                var leftWidthNew = "0px";
                if (level > 0) {
                    if ((event.pageX + event.currentMenuWidth + levelWidth) > $(window).width()) {
                        $ul.css({
                            display: 'block',
                            position: 'absolute',
                            left: (event.pageX - levelWidth) + 'px'
                        });
                        leftWidthNew = event.pageX - levelWidth;
                    }
                    else {
                        $ul.css({
                            display: 'block',
                            position: 'absolute',
                            left: (event.pageX + event.currentMenuWidth) + 'px'
                        });
                        leftWidthNew = event.pageX + event.currentMenuWidth;
                    }
                    var topCoordinate = event.pageY;
                    var menuHeight = angular.element($ul[0]).prop('offsetHeight');
                    var winHeight = $("#main-wrapper").height();
                    if (menuHeight > winHeight || topCoordinate + menuHeight > winHeight) {
                        var height = winHeight - topCoordinate;
                        if (height > menuHeight) {
                            $ul.css({
                                "max-height": height + 'px',
                                overflow: 'auto'
                            });
                        }
                        else {
                            if (menuHeight + 50 > winHeight) {
                                $ul.css({
                                    top: '130px',
                                    left: leftWidthNew + 'px',
                                    "max-height": winHeight - 140 + 'px',
                                    overflow: 'auto'
                                });
                            }
                            else {
                                topCoordinate = event.pageY - menuHeight;
                                if (topCoordinate < 150) {
                                    topCoordinate = 150;
                                }
                                $ul.css({
                                    top: topCoordinate + 'px',
                                    "max-height": winHeight - 140 + 'px',
                                    overflow: 'auto'
                                });
                            }
                        }
                    }
                }
            }
        });

        $contextMenu.on("mousedown", function (e) {
            if ($(e.target).hasClass('dropdown')) {
                $(event.currentTarget).removeClass('context');
                removeContextMenus();
            }
        }).on('contextmenu', function (event) {
            $(event.currentTarget).removeClass('context');
            event.preventDefault();
            removeContextMenus(level);
        });
        $scope.$on("$destroy", function () {
            removeContextMenus();
        });

        contextMenus.push($ul);
    };
    return function ($scope, element, attrs) {
        element.on('contextmenu', function (event) {
            event.stopPropagation();
            $scope.$apply(function () {
                event.preventDefault();
                var options = $scope.$eval(attrs.contextMenu);
                var model = $scope.$eval(attrs.model);
                if (options instanceof Array) {
                    if (options.length === 0) {
                        return;
                    }
                    renderContextMenu($scope, event, options, model);
                } else {
                    throw '"' + attrs.contextMenu + '" not an array';
                }
            });
        });
    };
}]);

app.directive("newentityobjecttreetemplate", ["$compile", "$rootScope", "$EntityIntellisenseFactory", function ($compile, $rootScope, $EntityIntellisenseFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            entityname: '=',
            selectedfield: '=',
            lstentity: '=',
            ischeckboxvisible: '=',
            selectedentityobjecttreefields: '=',
            iscollectionnextlevelload: '=',
            lstdisplayentity: '=',
            showonlycollection: '=',
            lstselectedmultiplelevelfield: '=',
            showonlyobject: '=',
            lstpreselectedfields: '=',
            lstpredisabledfields: '=',
            formid: '=',
            showonlycolumns: '=',
            showcolumndescription: '=',
            showvalueofcolumn: '=',
            ismainentity: '=',
            isallownavigation: '='
        },
        link: function (scope, element, attrs) {

            var unwatch = scope.$watch('entityname', function (newVal, oldVal) {
                scope.Init();
            });

            var unwatch2 = scope.$watch("lstpreselectedfields", function (newVal, oldVal) {
                if (newVal && newVal.length > 0) {
                    if (scope.lstpreselectedfields) {
                        angular.forEach(scope.lstpreselectedfields, function (itm) {
                            if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length) {
                                var lst = scope.selectedentityobjecttreefields.filter(function (x) { return x.ID == itm.ID; });
                                if (lst && lst.length > 0) {
                                    lst[0].IsSelected = 'True';
                                    lst[0].IsRecordSelected = true;
                                    lst[0].IsReadOnly = true;
                                    if (scope.lstselectedmultiplelevelfield) {
                                        var obj = { EntityField: lst[0].ID, FieldObject: lst[0], Entity: lst[0].EntityName };
                                        var isFieldFound = scope.isFieldFoundInSelectedList(scope.lstselectedmultiplelevelfield, lst[0].ID);
                                        if (!isFieldFound) {
                                            scope.lstselectedmultiplelevelfield.push(obj);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });

            scope.isFieldFoundInSelectedList = function (lstfields, fieldID) {
                var isFieldFound = false;
                for (var i = 0; i < lstfields.length; i++) {
                    if (lstfields[i].EntityField == fieldID) {
                        isFieldFound = true;
                        break;
                    }
                }
                return isFieldFound;
            };

            scope.CheckForPreDisableAttr = function (objEntity, path) {
                if (scope.lstpredisabledfields) {
                    angular.forEach(scope.lstpredisabledfields, function (itm) {
                        var lst = objEntity.SortedAttributes.filter(function (x) { return x.ID == itm.Value.ID; });
                        if (lst && lst.length > 0) {
                            if (path == "" || path == undefined) {
                                if (lst[0].ID == itm.Key) {
                                    lst[0].IsDisabled = true;
                                }
                            } else if ((path + "." + lst[0].ID) == itm.Key) {
                                lst[0].IsDisabled = true;
                            }
                        }
                    });
                }
            };

            scope.AddClsEntity = function (strEntityname, strdisplayName, strParentEntity, strDataTypeOfField) {
                if (strDataTypeOfField == 'Collection' || strDataTypeOfField == 'List') {
                    scope.isParentFieldCollection = true;
                }
                var objclsEntity = {};
                var Entity = scope.checkEntityIsPresent(strEntityname);
                if (Entity == "") {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var parentID = strEntityname;
                    var entattributes = [];
                    while (parentID) {
                        if (entityIntellisenseList && entityIntellisenseList.length) {
                            var lst = entityIntellisenseList.filter(function (x) { return x.ID == parentID; });
                            if (lst && lst.length > 0) {
                                if (scope.showonlycollection != undefined && !scope.showonlycollection && lst[0].Attributes) {
                                    var tempAttributes = [];
                                    tempAttributes = lst[0].Attributes.filter(function (x) { return (x.DataType && ((x.DataType != 'Collection') && (x.DataType != 'List'))) || !x.DataType; });
                                    entattributes = entattributes.concat(tempAttributes);
                                }
                                else {
                                    entattributes = entattributes.concat(lst[0].Attributes);
                                }
                                parentID = lst[0].ParentId;
                            } else {
                                parentID = "";
                            }
                        }
                    }
                    var EntityAttributes = [];
                    var strattr = JSON.stringify(entattributes.filter(function (attr) { return !(attr.Value && attr.Value.toLowerCase() === "app_json_data"); }));
                    entattributes = JSON.parse(strattr);
                    sortListBasedOnproperty(entattributes, "", "ID");
                    EntityAttributes = entattributes;

                    objclsEntity = {
                        IsVisible: true, strEntityId: strEntityname, Attributes: EntityAttributes, SortedAttributes: EntityAttributes
                    };

                    scope.lstEntities.push(objclsEntity);
                    if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                        ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                    }
                    scope.selectedentityobjecttreefields = EntityAttributes;
                    if (scope.ismainentity && strEntityname && entityIntellisenseList) {
                        var tempEntity = entityIntellisenseList.filter(function (x) {
                            return x.ID == strEntityname;
                        });
                        if (tempEntity.length > 0 && tempEntity[0].ErrorTableName) {
                            var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                            var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                            scope.selectedentityobjecttreefields.push(objInternalError);
                            scope.selectedentityobjecttreefields.push(objExternalError);
                        }
                    }

                } else {
                    objclsEntity = Entity;
                    if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                        ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                    }
                    scope.selectedentityobjecttreefields = Entity.Attributes;
                }
                scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstEntities, strEntityname, "Entity");
                scope.AddClsDisplayEntity(objclsEntity, strdisplayName, strParentEntity, strDataTypeOfField);
                scope.CheckForPreDisableAttr(objclsEntity, scope.lstdisplayentity[scope.lstdisplayentity.length - 1].strDisplayName);
            };

            scope.AddClsDisplayEntity = function (clsEntity, strdisplayName, strParentEntity, strDataTypeOfField) {
                var objclsDisplayEntity = {
                    IsVisible: true, strDisplayName: strdisplayName, strID: clsEntity.strEntityId, entity: clsEntity, strParentID: strParentEntity, DataType: strDataTypeOfField, isParentFieldCollection: scope.isParentFieldCollection
                };
                scope.lstDisplayEntity.push(objclsDisplayEntity);
                scope.lstdisplayentity = scope.lstDisplayEntity;
                scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstDisplayEntity, strdisplayName, "DisplayEntity");
            };

            scope.setVisibilityForEntitiesOrDisplayEntities = function (obj, strname, param) {

                scope.objEntityFilter.EntityTreeFilter = "";

                for (var i = 0; i < obj.length; i++) {
                    if (param == "Entity") {
                        if (obj[i].strEntityId == strname) {
                            obj[i].IsVisible = true;
                            scope.currentEntity = obj[i];
                            obj[i].SortedAttributes = obj[i].Attributes;
                            if (scope.selectedentityobjecttreefields && scope.selectedentityobjecttreefields.length > 0) {
                                ClearSelectedFieldList(scope.selectedentityobjecttreefields);
                            }

                            scope.selectedentityobjecttreefields = obj[i].SortedAttributes;

                        } else {
                            obj[i].IsVisible = false;
                        }
                    } else {
                        if (obj[i].strDisplayName == strname) {
                            obj[i].IsVisible = true;
                            scope.objEntityFilter.DisplayPath = obj[i].strDisplayName;
                        } else {
                            obj[i].IsVisible = false;
                        }
                    }
                }

                if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                    scope.objEntityFilter.LimitCount = 50;
                } else {
                    scope.objEntityFilter.LimitCount = scope.selectedentityobjecttreefields.length;
                }

                if (scope.lstselectedmultiplelevelfield && param == "DisplayEntity") {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    if (displayEntity) {
                        for (var i = 0; i < scope.selectedentityobjecttreefields.length; i++) {
                            scope.setValueToSelectedField(scope.selectedentityobjecttreefields[i], displayEntity);
                        }
                    }
                }
            };

            scope.setValueToSelectedField = function (field, displayEntity) {
                var displayName = displayEntity.strDisplayName;
                fieldName = field.ID;
                if (displayName != "") {
                    fieldName = displayName + "." + field.ID;
                }
                if (field.DataType != "Object" && field.DataType != "Collection" && field.DataType != "List" && field.DataType != "CDOCollection") {
                    for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                        if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                            field.IsSelected = "True";
                            break;
                        }
                    }
                }
            };

            scope.checkEntityIsPresent = function (strentityID) {
                var Entity = "";
                for (var i = 0; i < scope.lstEntities.length; i++) {
                    if (scope.lstEntities[i].strEntityId == strentityID) {
                        Entity = scope.lstEntities[i];
                        break;
                    }
                }

                return Entity;
            };

            scope.getParentEntity = function () {
                var entity = "";
                for (var i = 0; i < scope.lstEntities.length; i++) {
                    if (scope.lstEntities[i].IsVisible) {
                        entity = scope.lstEntities[i].strEntityId;
                        break;
                    }
                }
                return entity;
            };

            scope.getPathFromDisplayedEntities = function () {
                var path = "";
                for (var i = 0; i < scope.lstDisplayEntity.length; i++) {
                    if (scope.lstDisplayEntity[i].IsVisible) {
                        path = scope.lstDisplayEntity[i].strDisplayName;
                        break;
                    }
                }
                return path;
            };
            scope.Init = function () {
                scope.lstEntities = [];
                scope.lstDisplayEntity = [];
                scope.objEntityFilter = {};
                scope.objEntityFilter.EntityTreeFilter = "";
                scope.objEntityFilter.LimitCount = 50;
                scope.objEntityFilter.DisplayPath = "";
                scope.currentEntity = '';
                scope.otherSelectedField = [];
                scope.selectedentityobjecttreefields = [];
                scope.isParentFieldCollection = false;

                var strdisplayName = "";
                var strParentEntity = null;
                scope.AddClsEntity(scope.entityname, strdisplayName, strParentEntity, "");
            };

            if (scope.entityname) {
                scope.Init();
            }

            scope.LoadNextLevelEntityField = function (field, $event) {
                if (field.DataType == 'Collection' || field.DataType == 'List' || field.DataType == 'Object') {
                    if ((field.DataType == 'Collection' || field.DataType == 'List') && !scope.iscollectionnextlevelload) {
                        //alert("Collection Fields Cannot be added..");
                    } else {
                        var objectpath = scope.getPathFromDisplayedEntities();
                        var strEntityname = field.Entity;
                        var strDataTypeOfField = field.DataType;
                        var strdisplayName = "";
                        if (objectpath != "") {
                            strdisplayName = objectpath + "." + field.DisplayName;
                        } else {
                            strdisplayName = field.DisplayName;
                        }
                        var strParentEntity = scope.getParentEntity();
                        scope.AddClsEntity(strEntityname, strdisplayName, strParentEntity, strDataTypeOfField);
                    }
                }
            };

            scope.Navigatetoprevlist = function () {
                if (scope.lstDisplayEntity.length > 1) {
                    scope.lstDisplayEntity.splice(scope.lstDisplayEntity.length - 1, 1);
                    scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].IsVisible = true;
                    scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstEntities, scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].strID, "Entity");
                    scope.setVisibilityForEntitiesOrDisplayEntities(scope.lstdisplayentity, scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].strDisplayName, "DisplayEntity");
                    scope.CheckForPreDisableAttr(scope.lstDisplayEntity[scope.lstDisplayEntity.length - 1].entity, scope.lstdisplayentity[scope.lstdisplayentity.length - 1].DisplayName);
                }
            };

            scope.onRefreshAttributes = function () {
                if (scope.lstEntities && scope.lstEntities.length > 0) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < scope.lstEntities.length; i++) {
                        var entityId = scope.lstEntities[i].strEntityId;
                        var parentID = entityId;
                        var entattributes = [];
                        while (parentID) {
                            if (entityIntellisenseList && entityIntellisenseList.length) {
                                var lst = entityIntellisenseList.filter(function (x) { return x.ID == parentID; });
                                if (lst && lst.length > 0) {
                                    if (scope.showonlycollection != undefined && !scope.showonlycollection && lst[0].Attributes) {
                                        var tempAttributes = [];
                                        tempAttributes = lst[0].Attributes.filter(function (x) { return x.DataType && ((x.DataType != 'Collection') && (x.DataType != 'List')); });
                                        entattributes = entattributes.concat(tempAttributes);
                                    }
                                    else {
                                        entattributes = entattributes.concat(lst[0].Attributes);
                                    }
                                    //entattributes = entattributes.concat(lst[0].Attributes);
                                    parentID = lst[0].ParentId;
                                } else {
                                    parentID = "";
                                }
                            }
                        }
                        var EntityAttributes = [];
                        var strattr = JSON.stringify(entattributes.filter(function (attr) { return !(attr.Value && attr.Value.toLowerCase() === "app_json_data"); }));
                        entattributes = JSON.parse(strattr);

                        if (scope.ismainentity && scope.entityname && entityIntellisenseList) {
                            var tempEntity = entityIntellisenseList.filter(function (x) {
                                return x.ID == scope.entityname;
                            });
                            if (tempEntity.length > 0 && tempEntity[0].ErrorTableName) {
                                var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                                var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                                entattributes.push(objInternalError);
                                entattributes.push(objExternalError);
                            }
                        }

                        sortListBasedOnproperty(entattributes, "", "ID");
                        EntityAttributes = entattributes;
                        scope.lstEntities[i].Attributes = entattributes;
                        scope.lstEntities[i].SortedAttributes = EntityAttributes;

                        var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                        if (displayEntity && displayEntity.strID == entityId) {
                            scope.selectedentityobjecttreefields = EntityAttributes;
                        }
                    }
                    var newScope = scope.$new();
                    newScope.strMessage = "Refresh Completed.";
                    if (scope.objEntityFilter && scope.objEntityFilter.EntityTreeFilter) {
                        scope.objEntityFilter.EntityTreeFilter = "";
                    }
                    newScope.isError = true;
                    var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html");
                    newScope.OkClick = function () {
                        dialog.close();
                        scope.selectedfield = undefined;
                    };
                }
            };

            scope.selectField = function (field, event) {
                if (scope.selectedfield && !event.ctrlKey) {
                    scope.selectedfield.IsRecordSelected = false;
                    if (!scope.ischeckboxvisible) {
                        scope.selectedfield.IsSelected = "False";
                    }
                } else {
                    if (scope.selectedfield) {
                        var isFound = scope.RemoveOrIsFieldPresentFromOtherSelectedField(scope.selectedfield, "bool");
                        if (!isFound) {
                            scope.otherSelectedField.push(scope.selectedfield);
                        }
                        scope.selectedfield = undefined;
                    }
                }

                if (!event.ctrlKey) {
                    scope.selectedfield = field;
                    scope.selectedfield.IsRecordSelected = true;
                }
                if (event) {
                    event.stopPropagation();
                    event.stopImmediatePropagation();
                }

                if (!scope.ischeckboxvisible) {
                    if (event.ctrlKey) {
                        if (field.IsRecordSelected) {
                            field.IsRecordSelected = false;
                            field.IsSelected = "False";
                            scope.RemoveOrIsFieldPresentFromOtherSelectedField(field, "Delete");
                        }
                        else {
                            field.IsRecordSelected = true;
                            field.IsSelected = "True";
                            scope.otherSelectedField.push(field);
                        }
                    }
                    else {
                        for (var i = 0; i < scope.otherSelectedField.length; i++) {
                            scope.otherSelectedField[i].IsRecordSelected = false;
                            scope.otherSelectedField[i].IsSelected = "False";
                        }
                        scope.otherSelectedField = [];
                        field.IsRecordSelected = true;
                        field.IsSelected = "True";
                    }
                }
            };

            scope.RemoveOrIsFieldPresentFromOtherSelectedField = function (objField, param) {
                var isFound = false;
                for (var i = 0; i < scope.otherSelectedField.length; i++) {
                    if (scope.otherSelectedField[i].ID == objField.ID) {
                        if (param == "Delete") {
                            scope.otherSelectedField.splice(i, 1);
                        } else if (param == "bool") {
                            isFound = true;
                        }
                        break;
                    }
                }

                return isFound;
            };

            scope.sortList = function (clsEntity, strText) {
                if (strText != "") {
                    var lstExactMatchCaseSensitive = [];
                    var lstExactMatchCaseInSensitive = [];
                    var lstCaseSenesitive = [];
                    var lstCaseInsensitive = [];
                    var lstContainsCaseSensitive = [];
                    var lstContainsCaseInSensitive = [];
                    var attributeName = "ID";
                    if (scope.showvalueofcolumn) {
                        attributeName = "Value";
                    }
                    for (var i = 0; i < clsEntity.Attributes.length; i++) {
                        if (clsEntity.Attributes[i][attributeName] == strText) {
                            lstExactMatchCaseSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase() == strText.toLowerCase()) {
                            lstExactMatchCaseInSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && clsEntity.Attributes[i][attributeName].indexOf(strText) == 0) {
                            lstCaseSenesitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase().indexOf(strText.toLowerCase()) == 0) {
                            lstCaseInsensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && clsEntity.Attributes[i][attributeName].contains(strText)) {
                            lstContainsCaseSensitive.push(clsEntity.Attributes[i]);
                        } else if (clsEntity.Attributes[i][attributeName] && strText && clsEntity.Attributes[i][attributeName].toLowerCase().contains(strText.toLowerCase())) {
                            lstContainsCaseInSensitive.push(clsEntity.Attributes[i]);
                        }
                    }
                    var lst = lstExactMatchCaseSensitive.concat(lstExactMatchCaseInSensitive).concat(lstCaseSenesitive).concat(lstCaseInsensitive).concat(lstContainsCaseSensitive).concat(lstContainsCaseInSensitive);
                    clsEntity.SortedAttributes = lst;
                    if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                        scope.objEntityFilter.LimitCount = 50;
                    } else {
                        scope.objEntityFilter.LimitCount = clsEntity.Attributes.length;
                    }

                } else {
                    clsEntity.SortedAttributes = clsEntity.Attributes;
                    if (!scope.showonlycollection && !scope.showonlyobject && !scope.showonlycolumns && !scope.showcolumndescription) {
                        scope.objEntityFilter.LimitCount = 50;
                    } else {
                        scope.objEntityFilter.LimitCount = clsEntity.Attributes.length;
                    }
                }
            };

            scope.AddIntoSelectedList = function (field) {
                if (scope.ischeckboxvisible) {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    var displayName = displayEntity.strDisplayName;
                    fieldName = field.ID;
                    if (displayName != "") {
                        fieldName = displayName + "." + field.ID;
                    }
                    if (scope.lstselectedmultiplelevelfield) {
                        if (field.IsSelected == "True") {
                            var obj = { EntityField: fieldName, FieldObject: field, Entity: displayEntity.strID };
                            scope.lstselectedmultiplelevelfield.push(obj);
                        } else {
                            for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                                if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                                    scope.lstselectedmultiplelevelfield.splice(i, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            };

            scope.NavigateToEntity = function (entityID) {
                if (entityID && (scope.isallownavigation === undefined || scope.isallownavigation === true)) {
                    hubMain.server.navigateToFile(entityID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, undefined);
                    });
                }
            };
        }, templateUrl: "Common/views/EntityTreeListTemplate.html"
    };
}]);

app.directive('checkboxdirective', [function () {
    return {
        restrict: 'A',
        scope: {
            attributefield: '=',
            lstdisplayentity: '=',
            lstselectedmultiplelevelfield: '='
        },
        link: function (scope, element, attrs) {
            var ele = element[0];

            scope.InitSelectedField = function (field, element) {
                if (scope.lstselectedmultiplelevelfield) {
                    var displayEntity = getDisplayedEntity(scope.lstdisplayentity);
                    var displayName = displayEntity.strDisplayName;
                    fieldName = field.ID;
                    if (displayName != "") {
                        fieldName = displayName + "." + field.ID;
                    }
                    if (field.DataType != "Object" && field.DataType != "Collection" && field.DataType != "CDOCollection" && field.DataType != "List") {
                        for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                            if (scope.lstselectedmultiplelevelfield[i].EntityField == fieldName) {
                                field.IsSelected = "True";
                                break;
                            }
                        }
                    } else {
                        if (field.DataType == "Object") {
                            scope.setValueToLoadedField(fieldName, element);
                        }
                    }
                }
            };
            //checking object fields are selected or not
            scope.setValueToLoadedField = function (fieldName, element) {
                for (var i = 0; i < scope.lstselectedmultiplelevelfield.length; i++) {
                    if (scope.lstselectedmultiplelevelfield[i].EntityField.contains(fieldName)) {
                        element.indeterminate = true;
                        break;
                    }
                }
            };

            if (scope.lstselectedmultiplelevelfield) {
                scope.InitSelectedField(scope.attributefield, ele);
            }
        }
    };
}]);


app.directive('entitytreescroll', [function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            var scrollTopValue = 0;
            element.bind('scroll', function () {
                var diff = (raw.scrollTop + raw.offsetHeight) - raw.scrollHeight;
                if ((raw.scrollTop + raw.offsetHeight == raw.scrollHeight) || (diff > -5 && diff < 5)) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        if (scope.FormModel && scope.FormModel.IsLookupCriteriaEnabled) {
                            scope.ColumnsLimitCount += 10;
                        } else if (scope.objEntityFilter) {
                            scope.objEntityFilter.LimitCount += 10;
                        } else if (scope.objBusinessObjectFilter) {
                            scope.objBusinessObjectFilter.LimitCount += 10;
                        }
                    });
                }
            });
        }

    };
}]);


app.directive("entityobjectchildtreetemplate", ["$compile", function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Common/views/EntityTreeTemplate.html',
    };
}]);

app.directive('entitytreefielddraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
            dragobject: '='
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
                    dragDropData = scope.dragdata;
                    dragDropDataObject = scope.dragobject;
                }
            }
        },
    };
}]);

app.directive('entitytreefielddroppable', ["$rootScope", function ($rootScope) {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            fieldname: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.addEventListener('dragover', handleDragOver, false);
            el.addEventListener('drop', handleDrop, false);
            el.addEventListener('dragleave', handleDragLeave, false);

            function handleDragOver(e) {
                if (e.preventDefault) {
                    e.preventDefault(); // Necessary. Allows us to drop.
                }

                e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                e.dataTransfer.setData("Text", "");
                e.preventDefault();
            }

            function handleDrop(e) {
                var strData = e.dataTransfer.getData("Text");
                // Stops some browsers from redirecting.
                if (e.stopPropagation) e.stopPropagation();

                if (strData == "" && lstEntityTreeFieldData != null) {
                    e.preventDefault();
                    var obj = lstEntityTreeFieldData;//JSON.parse(strData);
                    var Id = obj[0];
                    var displayName = obj[1];
                    if (displayName != "") {
                        Id = displayName + "." + Id;
                    }
                    dragDropData = Id;
                    // Stops some browsers from redirecting.
                    //if (e.stopPropagation) e.stopPropagation();
                    var ID = dragDropData;
                    if (ID != undefined && ID != null && ID != "") {
                        if (scope.dropdata) {
                            scope.$apply(function () {
                                scope.dropdata[scope.fieldname] = ID;
                            });
                        }
                    }

                    //$(e.target).blur();
                    lstEntityTreeFieldData = null;
                    dragDropData = null;
                } else {
                    if (dragDropData) {

                        var data = dragDropData;
                        if (data != undefined && data != null && data != "") {
                            if (scope.dropdata) {
                                scope.$apply(function () {
                                    scope.dropdata[scope.fieldname] = data;
                                });
                            }
                        }

                        dragDropData = null;
                    }
                    lstEntityTreeFieldData = null;
                    if (e && e.preventDefault) {
                        e.preventDefault();
                    }
                }
            }

            function handleDragLeave(e) {

            }
        }
    };
}]);

app.directive("contentEditable", ["$parse", "$compile", "$rootScope", function ($parse, $compile, $rootScope) {
    return {
        restrict: "A",
        scope: {
            model: "=",
            propertyname: "=",
        },
        require: "ngModel",
        link: function (scope, element, attrs, ctrlr) {
            var ngmodelname = attrs.ngModel;
            var modelname = attrs.model;
            var oldvalue;
            var propname = scope.propertyname;
            if (ngmodelname && modelname && ngmodelname.match("^" + modelname)) {
                propname = ngmodelname.substring(modelname.length + 1, ngmodelname.length);
            }
            var curscope;
            var curscopeDivId;

            var unwatch = scope.$watch('model', function (newVal, oldVal) {
                if (newVal) {
                    oldvalue = getPropertyValue(scope.model, propname);
                    if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileName) {
                        curscopeDivId = $rootScope.currentopenfile.file.FileName;
                        curscope = getScopeByFileName(curscopeDivId);
                    }
                    unwatch();
                }
            });


            ctrlr.$render = function () {
                element.text(ctrlr.$viewValue || "");
            };

            function read() {
                ctrlr.$setViewValue(element.text());
            }


            scope.$watch(attrs.ngModel, function () {
                if (ctrlr.$viewValue) {
                    ctrlr.$viewValue = ctrlr.$viewValue.trim();
                }
                //if (ctrlr.$viewValue != "" && ctrlr.$viewValue != undefined) {
                //    element.removeAttr('placeholder')
                //}
                //else if (ctrlr.$viewValue == "" || ctrlr.$viewValue == undefined) {
                //    if (!element.attr('placeholder')) {
                //        if (attrs.ngModel == "items.dictAttributes.Text") {
                //            element.attr("placeholder", "Description");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwExpression" || attrs.ngModel == "x.dictAttributes.sfwExpression" || attrs.ngModel == "parameter.dictAttributes.sfwExpression") {
                //            element.attr("placeholder", "Expression");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwReturnField") {
                //            element.attr("placeholder", "ReturnField");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwRuleID") {
                //            element.attr("placeholder", "RuleID");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwEffectiveDate") {
                //            element.attr("placeholder", "EffectiveDate");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwMethodName") {
                //            element.attr("placeholder", "MethodName");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwQueryID") {
                //            element.attr("placeholder", "QueryID");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwItemName") {
                //            element.attr("placeholder", "ItemName");
                //        }
                //        else if (attrs.ngModel == "items.dictAttributes.sfwObjectID") {
                //            element.attr("placeholder", "Collection");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwEntityField") {
                //            element.attr("placeholder", "EntityField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwDataField") {
                //            element.attr("placeholder", "DataField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.sfwObjectField") {
                //            element.attr("placeholder", "ObjectField");
                //        }
                //        else if (attrs.ngModel == "model.dictAttributes.Text") {
                //            element.attr("placeholder", "Text");
                //        }
                //        //$compile(element[0])(scope);
                //    }
                //}
            });
            scope.$on("UpdateOnClick", function (e, data) {
                if (angular.equals(data, element[0])) {
                    scope.valueChange(element.text());
                }
            });


            scope.valueChange = function (text) {
                ctrlr.$setViewValue(text);
            };
            element.bind("keyup", function () {
                scope.$parent.$apply(read);
                //scope.$apply(attrs.textAction); 
                if (attrs.descriptionKeydown) {
                    scope.$parent.$apply(attrs.descriptionKeydown);
                }
                if (attrs.loopCollectionKeydown) {
                    var vrloopCollectionKeydown = $parse(attrs.loopCollectionKeydown);
                    vrloopCollectionKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.actionKeydown) {
                    var vractionKeydown = $parse(attrs.actionKeydown);
                    vractionKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.queryidKeydown) {
                    var vractionQueryIdKeydown = $parse(attrs.queryidKeydown);
                    vractionQueryIdKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.methodnameKeydown) {
                    var vractionMethodNameKeydown = $parse(attrs.methodnameKeydown);
                    vractionMethodNameKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.ruleidKeydown) {
                    var vractionRuleIdKeydown = $parse(attrs.ruleidKeydown);
                    vractionRuleIdKeydown(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.spanClick) {
                    var vrclickonspan = $parse(attrs.spanClick);
                    vrclickonspan(scope.$parent, { $event: event });
                    scope.$parent.$apply();
                }
                if (attrs.loopCollectionKeydown) {
                    scope.$apply(attrs.loopCollectionKeydown);
                }

                var newvalue = getPropertyValue(scope.model, propname);
                //newvalue = getRemovedStringStartingSpace(newvalue);
                //setPropertyValue(scope.model, propname, newvalue);
                if (oldvalue != newvalue) {

                    if (curscopeDivId != undefined) {
                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {
                            };
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;

                            sessionstorageitem.UndoList.push(undoitem);

                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                }

            });
            element.bind("keydown", function (eargs) {
                if (eargs.keyCode == $.ui.keyCode.ENTER) {
                    eargs.preventDefault();
                }
            });
        }
    };
}]);

app.directive("commonTreeDirective", [function () {
    return {
        restrict: "E",
        replace: true,
        scope: {
            collection: "=",
            childCollectionProperty: "=",
            textProperty: "=",
            expandedProperty: "=",
            onitemdblclickCallback: "=",
            onitemclickCallback: "=",
        },
        link: function (scope, element, attrs) {
            scope.getPropertyValue = getPropertyValue;
            scope.onItemDoubleClick = function (event, element) {
                if (scope.onitemdblclickCallback) {
                    scope.onitemdblclickCallback(event, element);
                }
            };
            scope.onItemClick = function (event, element) {
                scope.selectedItem = element;
                if (scope.onitemclickCallback) {
                    scope.onitemclickCallback(element, event);
                }
            };
            scope.toggleExpandCollapse = function (event, element) {
                setPropertyValue(element, scope.expandedProperty, !scope.getPropertyValue(element, scope.expandedProperty));
            };
            scope.getPropertyValue = getPropertyValue;
        },
        templateUrl: "Common/views/CommonTreeTemplate.html"
    };
}]);

app.directive("commonTreeChildDirective", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Common/views/CommonTreeItemsTemplate.html',
    };
}]);

app.directive("ruleitemcommontemplate", ["$compile", "$http", "$rootScope", function ($compile, $http, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            iscontroldisable: '=',
        },
        link: function (scope, element, attrs) {

        },
        templateUrl: function (elem, attrs) {
            return attrs.templateUrl || 'Form/views/RuleItemCommonTemplate.html';
        }
    };
}]);

app.directive("undoredodirective", ["$parse", "$compile", "$rootScope", function ($parse, $compile, $rootScope) {
    return {
        restrict: "A",
        scope: {
            model: "=",
            propertyname: "=",
            undorelatedfunction: "=",
        },
        link: function (scope, element, attrs) {
            var propname = "";
            var oldvalue = "";
            var curscopeDivId;
            var curscope;

            var unwatch = scope.$watch('model', function (newVal, oldVal) {
                if (newVal) {
                    var ngmodelname = attrs.ngModel;
                    var modelname = attrs.model;

                    propname = scope.propertyname;

                    if (!propname && ngmodelname && modelname && ngmodelname.match("^" + modelname)) {
                        propname = ngmodelname.substring(modelname.length + 1, ngmodelname.length);
                    }

                    if (scope.model) {
                        oldvalue = getPropertyValue(scope.model, propname);
                    }
                    if ($rootScope.isConfigureSettingsVisible) {
                        curscopeDivId = "SettingsPage";
                    }
                    else {
                        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
                            curscopeDivId = $rootScope.currentopenfile.file.FileName;
                        }
                    }
                    curscope = getCurrentFileScope();

                    unwatch();

                }
            });

            element.bind("focus", function () {
                if (scope.model) {
                    oldvalue = getPropertyValue(scope.model, propname);
                }
            });

            if (attrs.type == "checkbox" || attrs.type == "radio") {
                element.bind("click", function () {
                    var newvalue = getPropertyValue(scope.model, propname);

                    if (oldvalue != newvalue) {

                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {};
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;

                            $rootScope.$evalAsync(function () {
                                sessionstorageitem.UndoList.push(undoitem);
                            });

                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                });
            }
            else {

                element.bind("change", function () {
                    var newvalue = getPropertyValue(scope.model, propname);

                    if (oldvalue != newvalue) {

                        var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                        if (sessionstorageitem) {
                            sessionstorageitem.RedoList = [];
                            var undoitem = {};
                            undoitem.operation = 'Edit';
                            undoitem.value = oldvalue;
                            undoitem.model = scope.model;
                            undoitem.propname = propname;
                            undoitem.undorelatedfunction = scope.undorelatedfunction;
                            $rootScope.$evalAsync(function () {
                                sessionstorageitem.UndoList.push(undoitem);
                            });
                            oldvalue = newvalue;
                            if (curscope) {
                                if (!curscope.isDirty) {
                                    curscope.isDirty = true;
                                }
                            }
                        }
                    }
                });
            }
        }
    };
}]);

app.directive("datepicker", [function () {
    return {
        restrict: "A",
        link: function (scope, el, attr) {
            el.datepicker({
                dateFormat: 'm/d/yy',
                onClose: function (dateText, inst) {
                    $("#tbDescription").focus();
                }, autoclose: true
            });
        }
    };
}]);

app.directive('editableRow', ["$compile", function ($compile) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            if (attributes && attributes.editTemplateUrl && attributes.readOnlyTemplateUrl) {
                scope.isEditable = false;

                var readOnlyTemplateHtml = getHtmlFromServer(attributes.readOnlyTemplateUrl);
                if (readOnlyTemplateHtml) {
                    var editTemplateHtml = getHtmlFromServer(attributes.editTemplateUrl);
                    if (editTemplateHtml) {

                        $(element).keyup(function (e) {
                            if (e.keyCode == 27 && scope.isEditable && !(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                scope.setRowContentReadOnly();
                            }
                        });
                        $(element).dblclick(function () {
                            if (!scope.isEditable && !(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                scope.setRowContentEditable();
                            }
                        });
                        $(element).click(function () {
                            scope.resetAllButThis();
                        });
                        scope.addActionButton = function (actionType, replaceExisting) {
                            var anchor;
                            if (!(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                var anchor = $("<i style='cursor:pointer'></i>");
                                if (actionType == "edit") {
                                    anchor.addClass("fa fa-edit");
                                    anchor.click(function () {
                                        scope.setRowContentEditable();
                                    });
                                }
                                else {
                                    anchor.addClass("fa fa-close");
                                    anchor.click(function () {
                                        scope.setRowContentReadOnly();
                                    });
                                }
                            }
                            var td = $("<td></td>");
                            if (!(attributes.alwaysReadOnly === true || attributes.alwaysReadOnly === "true")) {
                                td.append(anchor);
                            }
                            $(element).append(td);
                        };
                        scope.resetAllButThis = function () {
                            //Set rest of the rows as read-only, when this row is getting edited.
                            var rows = $(element).siblings();
                            for (var index = 0; index < rows.length; index++) {
                                var rowScope = angular.element($(rows[index])).scope();
                                if (rowScope && (rowScope.isEditable === true || rowScope.isEditable === "true") && rowScope.setRowContentReadOnly) {
                                    rowScope.setRowContentReadOnly();
                                }
                            }
                        };

                        scope.setRowContentEditable = function (onLoad) {
                            scope.isEditable = true;
                            scope.setRowContent(editTemplateHtml);
                            if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                                scope.addActionButton("cancel", !onLoad);
                            }
                            scope.compileContent();
                        };
                        scope.setRowContentReadOnly = function (onLoad) {
                            scope.isEditable = false;
                            scope.setRowContent(readOnlyTemplateHtml);
                            if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                                scope.addActionButton("edit", !onLoad);
                            }
                            scope.compileContent();
                        };
                        scope.setRowContent = function (content) {
                            $(element).html(content);
                        };
                        scope.compileContent = function () {
                            $compile($(element).contents())(scope);
                        };

                        if (attributes && (attributes.isNew === true || attributes.isNew === "true")) {
                            scope.setRowContentEditable(true);
                        }
                        else {
                            scope.setRowContentReadOnly(true);
                        }

                        if (attributes && (attributes.showEditButton === true || attributes.showEditButton === "true")) {
                            //Add extra th if not exists.
                            if ($(element).closest("table").find("thead tr").length > 1) {
                                var colHeaderRow = $(element).closest("table").find("thead tr[role='columnheader']");
                                if (colHeaderRow.find("th").length < $(element).find("td").length) {
                                    //Increase heading colspan.
                                    var heading = $(element).closest("table").find("thead tr[role!='columnheader'] th:last-child");
                                    if (heading && heading.length > 0) {
                                        var colspan = parseInt(heading.attr("colspan")) + 1;
                                        heading.attr("colspan", colspan.toString());
                                    }

                                    colHeaderRow.append("<th style='width:35px'>");
                                }
                            }
                            else {
                                var colHeaderRow = $(element).closest("table").find("thead tr");
                                if (colHeaderRow.find("th").length < $(element).find("td").length) {
                                    colHeaderRow.append("<th style='width:35px'>");
                                }
                            }
                        }
                    }
                    else {
                        console.log("invalid edit template url for the directive.");
                    }
                }
                else {
                    console.log("invalid read-only template url for the directive.");
                }
            }
            else {
                console.log("Please set edit/read-only template url for the directive.");
            }
        }
    };
}]);

app.directive('editablelabletotext', ["$compile", function ($compile) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            var ele;
            scope.onChangeHandler = function () {
                var text = ele.val();
                growtextbox(ele, true, text);
            };
            $(element).dblclick(function () {
                ele = $("<input type='text' ng-model='model.dictAttributes.sfwEntityField' ng-change='onChangeHandler()' ng-keydown='onActionKeyDown($event)' class='temptext' undoredodirective model='model' />");
                element.after(ele);
                $compile(ele)(scope);
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwEntityField) {
                    growtextbox(ele, true, scope.model.dictAttributes.sfwEntityField);
                }
                element.hide();
                ele.blur(function () {
                    element.show();
                    $(this).remove();
                });
                ele.focus();
            });
        }
    };
}]);

app.directive('wizardTitleUpdate', ["WizardHandler", function (WizardHandler) {
    return {
        restrict: "A",
        link: function (scope, element, attributes) {
            scope.wizardHandler = WizardHandler;
            if (!scope.onNext) {
                scope.onNext = function () {
                    if (scope.setTitle) {
                        var steps = scope.wizardHandler.wizard().getEnabledSteps();
                        var nextStepIndex = steps.indexOf(scope.wizardHandler.wizard().currentStep()) + 1;
                        if (nextStepIndex > -1 && nextStepIndex < steps.length) {
                            var title = steps[nextStepIndex].wzTitle;
                            scope.setTitle(title);
                        }
                    }
                };
            }
            if (!scope.onPrevious) {
                scope.onPrevious = function () {
                    if (scope.setTitle) {
                        var steps = scope.wizardHandler.wizard().getEnabledSteps();
                        var previousStepIndex = steps.indexOf(scope.wizardHandler.wizard().currentStep()) - 1;
                        if (previousStepIndex > -1 && previousStepIndex < steps.length) {
                            var title = steps[previousStepIndex].wzTitle;
                            scope.setTitle(title);
                        }
                    }
                };
            }

            var wizardUnwatch = scope.$watchCollection(function () { return scope.wizardHandler.wizard().getEnabledSteps(); }, function (newval, oldval) {
                if (newval && newval.length > 0) {
                    if (scope.setTitle) {
                        var title = scope.wizardHandler.wizard().currentStep().wzTitle;
                        scope.setTitle(title);
                    }
                    wizardUnwatch();
                }
            });
        }
    };
}]);

app.directive("loadingBox", [function () {
    return {
        restrict: "E",
        scope: {
            lazymodel: "=",
            size: "@"
        },
        link: function (scope, element, attribute) {
            var loadwatch = scope.$watch('lazymodel', function (newval, oldvalue) {
                var elhover = '<div class="cs-loader-inner"><div><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label></div></div>';
                if (scope.size && scope.size == 'xs') {
                    elhover = '<div class="cs-loader-inner cs-loader-xs"><div><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label><label>	●</label></div></div>';
                }
                if (newval == undefined) {
                    element.append(elhover);
                }
                else {
                    element.find(".cs-loader-inner").remove();
                    loadwatch();
                }
            });
        }
    };
}]);

app.directive("searchSource", ["$rootScope", "$SearchSource", function ($rootScope, $SearchSource) {
    return {
        restrict: "E",
        scope: {
            SearchSource: "=searchSource"
        },
        templateUrl: "Common/views/SearchSourceWrapper.html",
        link: function (scope, element, attribute) {
            scope.onsearchtextchange = function () {
                if (scope.searchtext == "") {
                    scope.SearchSource.searchFindarray = [];
                    scope.SearchSource.searchSourceText = "";
                }
            };
            scope.SearchCriteriaChange = function (criteria) {
                criteria = criteria ? criteria : "";
                scope.searchfocus = true;
                switch (criteria.toLowerCase()) {
                    case "case": scope.SearchSource.IsCaseSensitive = !scope.SearchSource.IsCaseSensitive; break;
                    case "word": scope.SearchSource.IsWholeMatchWord = !scope.SearchSource.IsWholeMatchWord; break;
                    case "regex": scope.SearchSource.IsSearchRegex = !scope.SearchSource.IsSearchRegex; break;
                    default: break;
                }
                if (criteria) {
                    scope.SearchSource.IsSearchCriteriaChange = true;
                    scope.SearchSourceFind();
                }
            };
            scope.SearchSourceFind = function () {
                // new search 
                // blur search div when user clicks 
                scope.searchfocus = false;
                if (scope.searchtext) {
                    // fresh search - when search text is changed / criteria is changed
                    if (scope.SearchSource.searchSourceText != scope.searchtext || (scope.SearchSource.IsSearchCriteriaChange)) {
                        scope.SearchSource.IsSearchCriteriaChange = false;
                        scope.SearchSource.searchSourceText = scope.searchtext;
                        scope.SearchSource.searchFindarray = $SearchSource.getIndicesOfSearchStr(scope.searchtext, scope.SearchSource.txtarea[0].value, scope.SearchSource.IsCaseSensitive, scope.SearchSource.IsWholeMatchWord, scope.SearchSource.IsSearchRegex);
                        if (scope.SearchSource.searchFindarray.length > 0) {
                            scope.SearchSourceMatch(true);
                        }
                    }
                    // next search - on search button
                    else if ((scope.SearchSource.searchFindarray.length - scope.SearchSource.searchfindindex) > 0) {
                        scope.SearchSourceMatch(false, true);
                    }
                }
            };
            scope.SearchSourceMatch = function (isNew, isNext, isPrevious) {
                // handle previous and next 
                var currentIndex = 0;
                if (isNew) scope.SearchSource.searchfindindex = 1; // when search text changes -- index becomes 1  
                else if (!isNew && isNext) scope.SearchSource.searchfindindex += 1;
                else if (!isNew && isPrevious) scope.SearchSource.searchfindindex -= 1;
                currentIndex = scope.SearchSource.searchfindindex - 1;
                if (currentIndex >= 0) {
                    scope.SearchSource.sourceSearchlineno = scope.SearchSource.txtarea[0].value.substr(0, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchFindarray[currentIndex] : scope.SearchSource.searchFindarray[currentIndex].startPos).split("\n").length;
                    $SearchSource.selectword(scope.SearchSource.txtarea, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchFindarray[currentIndex] : scope.SearchSource.searchFindarray[currentIndex].startPos, !scope.SearchSource.IsSearchRegex ? scope.SearchSource.searchSourceText : scope.SearchSource.txtarea[0].value.substring(scope.SearchSource.searchFindarray[currentIndex].startPos, scope.SearchSource.searchFindarray[currentIndex].endPos), scope.SearchSource.sourceSearchlineno);
                    scrollTextArea(scope.SearchSource.txtarea, scope.SearchSource.sourceSearchlineno, scope.SearchSource.searchSourceText);
                }
            };
            scope.CloseSearchSource = function () {
                scope.SearchSource.IsCaseSensitive = false;
                scope.SearchSource.IsWholeMatchWord = false;
                scope.SearchSource.IsSearchRegex = false;
                scope.SearchSource.searchFindarray = [];
                scope.SearchSource.searchSourceText = "";
                scope.SearchSource.IsEnable = false;
                scope.SearchSource.txtarea.focus();
            };
            scope.SearchSource.txtarea[0].onkeydown = function (event) {
                if (event.ctrlKey && event.keyCode == 70) {
                    scope.$evalAsync(function () {
                        scope.SearchSource.IsEnable = true;
                    });
                    scope.SearchSourceFindBySelection();
                    event.preventDefault();
                }
                if (event.shiftKey && event.keyCode == 114) {
                    if (scope.SearchSource.searchfindindex - 1 > 0) scope.SearchSourceMatch(false, false, true);
                    event.preventDefault();
                }
                else if (event.keyCode == 114) {
                    // for next
                    if ((scope.SearchSource.searchFindarray.length - scope.SearchSource.searchfindindex) > 0) scope.SearchSourceMatch(false, true);
                    event.preventDefault();
                }
            };
            scope.inputKeydownCallback = function (e) {
                // for enter search
                var charCode = (e.which) ? e.which : e.keyCode;
                if (charCode == 13) {
                    scope.SearchSourceFind();
                    e.preventDefault();
                }
            };
            scope.inputpasteCallback = function (e) {
                setTimeout(function () {
                    scope.searchtext = (e.target) ? e.target.value : "";
                    if (scope.searchtext) {
                        scope.SearchSourceFind();
                    }
                }, 0); //or 4
            };
            scope.SearchSourceFindBySelection = function () {
                // if anything selected from textarea it should be searched by default on load of this directive
                if (typeof scope.SearchSource.txtarea[0].selectionStart == "number" && typeof scope.SearchSource.txtarea[0].selectionEnd == "number") {
                    if (scope.SearchSource.txtarea[0].selectionStart < scope.SearchSource.txtarea[0].selectionEnd) {
                        scope.searchtext = scope.SearchSource.txtarea[0].value.substring(scope.SearchSource.txtarea[0].selectionStart, scope.SearchSource.txtarea[0].selectionEnd);
                        scope.SearchSourceFind();
                    }
                }
            };
            scope.init = function () {
                // focus should be on input
                scope.inputelement = $("#" + $rootScope.currentopenfile.file.FileName + ' .searchSource').find("input[type='text']").focus();
                scope.inputelement.focus();
                scope.SearchSourceFindBySelection();
            };
            scope.init();
        }
    };
}]);

app.directive("extraFieldDirective", ["$rootScope", "$filter", function ($rootScope, $filter) {
    return {
        restrict: "E",
        scope: {
            extraFieldModel: "=",
            showExtraFieldsTab: "=isTab",
            objFn: "=",
            formName: "=",
            isCretaeNewObjectSteps: "=isSteps",
            closeWizardFn: "=",
            validateFn: "=",
            isFileCreation: "="
        },
        templateUrl: "Common/views/ExtraFieldsOnDetails.html",
        link: function (scope, elem, attr) {
            // # region init section
            scope.objExtraFields = [];
            scope.ID = undefined;

            //hub call for get extra fields 

            scope.onFormChange = function () {
                scope.extraFields = [];
                scope.objExtraFields = [];
                scope.IsBaseAppFile = false;
                if (!scope.isFileCreation && $rootScope.currentfile && $rootScope.currentopenfile.file) {
                    scope.IsBaseAppFile = $rootScope.currentopenfile.file.IsBaseAppFile;
                }
                hubMain.server.getExtraSettingsModel().done(function (data) {
                    scope.$apply(function () {
                        if (data) {
                            var objCustomSettings = data;
                            if (objCustomSettings && objCustomSettings.Elements) {
                                scope.extraFields = $filter('filter')(objCustomSettings.Elements, { Name: scope.formName })[0];
                                if (scope.extraFields && scope.extraFields.Elements && scope.extraFields.Elements.length > 0) {
                                    scope.showExtraFieldsTab = true;
                                    if (scope.objFn && scope.objFn.showExtraFields) scope.objFn.showExtraFields(true);
                                    scope.init();
                                } else {
                                    scope.showExtraFieldsTab = false;
                                    if (scope.objFn && scope.objFn.showExtraFields) scope.objFn.showExtraFields(false);
                                }
                            }
                        }
                    });
                });
            };
            scope.init = function () {
                scope.createExtraFieldList();

            };

            scope.createExtraFieldList = function (model) {
                if (model) {
                    scope.extraFieldModel = model;
                }
                if (scope.extraFields != undefined && scope.extraFields.Elements) {
                    scope.objExtraFields = [];
                    for (var i = 0; i < scope.extraFields.Elements.length; i++) {
                        if (scope.extraFields.Elements[i].dictAttributes.value) {
                            var dummyobj = {
                                ID: scope.extraFields.Elements[i].dictAttributes.value, Description: scope.extraFields.Elements[i].dictAttributes.Description, ControlType: scope.extraFields.Elements[i].dictAttributes.ControlType, Children: [], IsRequired: scope.extraFields.Elements[i].dictAttributes.IsRequired
                            };

                            // if (scope.isCretaeNewObjectSteps == "false" || scope.isCretaeNewObjectSteps == false) {
                            dummyobj.Value = GetExtraFieldValue(dummyobj.ID);
                            //   }

                            if (dummyobj.ControlType == "HyperLink") {
                                dummyobj.URL = undefined;
                                if (scope.extraFieldModel) {
                                    for (var m = 0; m < scope.extraFieldModel.Elements.length; m++) {
                                        if (scope.extraFieldModel.Elements[m].dictAttributes.ID == dummyobj.ID) {
                                            dummyobj.URL = scope.extraFieldModel.Elements[m].dictAttributes.URL;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (scope.extraFields.Elements[i].Elements.length > 0) {
                                for (var j = 0; j < scope.extraFields.Elements[i].Elements.length; j++) {
                                    var dummyobjchild = {
                                        ID: scope.extraFields.Elements[i].Elements[j].dictAttributes.Text, Description: scope.extraFields.Elements[i].Elements[j].dictAttributes.Description
                                    };
                                    if (dummyobj.ControlType == "CheckBoxList") {
                                        if (dummyobj.Value != undefined) {
                                            var val = dummyobj.Value.split(',');
                                            for (var k = 0; k < val.length; k++) {
                                                if (dummyobjchild.ID == val[k]) {
                                                    dummyobjchild.Value = "True";
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        if (dummyobjchild.ID == dummyobj.Value) {
                                            dummyobjchild.Value = true;
                                            scope.ID = dummyobj.Value;
                                        }
                                        else {
                                            dummyobjchild.Value = false;
                                        }
                                    }

                                    dummyobj.Children.push(dummyobjchild);
                                }
                            }
                            scope.objExtraFields.push(dummyobj);
                        }
                    }
                    if (scope.objExtraFields && scope.objExtraFields.length <= 0) {
                        scope.showExtraFieldsTab = false;
                    }
                }

            };


            //#endregion init

            var GetExtraFieldValue = function (ID) {
                if (scope.extraFieldModel) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i].dictAttributes.ID == ID) {
                            return scope.extraFieldModel.Elements[i].dictAttributes.Value;
                            break;
                        }
                    }
                }
            };

            // #region common function for extra fields

            scope.selectExtraFieldRow = function (obj) {
                scope.SelectExtraFieldRow = obj;
            };

            scope.AddHyperLinkUrl = function (obj) {
                var newHyperlinkScope = scope.$new();
                newHyperlinkScope.oldData = {};
                newHyperlinkScope.currentobjHyperLink = obj;
                var temp = JSON.stringify(obj);
                newHyperlinkScope.objHyperLink = JSON.parse(temp);

                newHyperlinkScope.oldData.url = newHyperlinkScope.objHyperLink.URL;
                newHyperlinkScope.oldData.value = newHyperlinkScope.objHyperLink.Value;

                newHyperlinkScope.validateHyperLink = function () {
                    newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = undefined;
                    if ((newHyperlinkScope.objHyperLink.Value == undefined || newHyperlinkScope.objHyperLink.Value == "")) {
                        newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Enter the Description.";
                        return true;
                    } else if (newHyperlinkScope.objHyperLink.URL == undefined) {
                        newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                        if (newHyperlinkScope.currentobjHyperLink.IsRequired == "True" || newHyperlinkScope.currentobjHyperLink.IsRequired == true) {
                            newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                        }
                        return true;
                    } else {
                        var match = newHyperlinkScope.objHyperLink.URL.match(new RegExp(/^http(s)?:\/\/(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?/));
                        if (match == null) {
                            newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                            if (newHyperlinkScope.currentobjHyperLink.IsRequired == "True" || newHyperlinkScope.currentobjHyperLink.IsRequired == true) {
                                newHyperlinkScope.EntityDetailsHyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                            }
                            return true;
                        }
                    }
                };

                newHyperlinkScope.closeEntityDetailsHyperlink = function () {
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].ID == newHyperlinkScope.currentobjHyperLink.ID) {
                            newHyperlinkScope.currentobjHyperLink.URL = newHyperlinkScope.objHyperLink.URL;
                            newHyperlinkScope.currentobjHyperLink.Value = newHyperlinkScope.objHyperLink.Value;
                        }
                    }
                    newHyperlinkScope.dialog.close();
                };

                newHyperlinkScope.cancelHyperlinkDialog = function () {
                    newHyperlinkScope.objHyperLink.URL = newHyperlinkScope.oldData.url;
                    newHyperlinkScope.objHyperLink.Value = newHyperlinkScope.oldData.value;
                    newHyperlinkScope.dialog.close();
                };

                newHyperlinkScope.dialog = $rootScope.showDialog(newHyperlinkScope, "Add Hyperlink Url", "Entity/views/AddHyperLinkUrl.html", { width: 400, height: 500 });

            };

            if (scope.objFn) {
                scope.objFn.getExtraFieldData = function () {
                    return scope.objExtraFields;
                };
                scope.objFn.prepareExtraFieldData = function () {
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].ControlType == "TextBox" || scope.objExtraFields[i].ControlType == "CheckBox" || scope.objExtraFields[i].ControlType == "ComboBox") {
                            scope.updateExtraFieldValue(scope.objExtraFields[i]);
                        }
                        if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                            scope.updateExtraFieldValue(scope.objExtraFields[i]);
                        }
                        if (scope.objExtraFields[i].ControlType == "HyperLink") {
                            scope.updateExtraFieldHyperLinkValue(scope.objExtraFields[i]);
                        }
                    }

                    if (scope.extraFieldModel && scope.extraFieldModel.Elements && scope.extraFieldModel.Elements.length > 0 /* && $rootScope.currentopenfile
                        && ($rootScope.currentopenfile.file.FileType == "DecisionTable" || $rootScope.currentopenfile.file.FileType == "BPMN" || $rootScope.currentopenfile.file.FileType == "BPMTemplate")
                       && (scope.isCretaeNewObjectSteps == "true" || scope.isCretaeNewObjectSteps == true)*/) {
                        var obj;
                        var arrObj = [];
                        for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                            obj = {};
                            obj.ID = scope.extraFieldModel.Elements[i].dictAttributes.ID;
                            obj.Value = scope.extraFieldModel.Elements[i].dictAttributes.Value;
                            obj.URL = scope.extraFieldModel.Elements[i].dictAttributes.URL;
                            arrObj.push(obj);
                        }
                        scope.newExtraFieldModel = arrObj;
                    }
                };

                scope.objFn.getNewExtraFieldData = function () {
                    return scope.newExtraFieldModel;
                };

                scope.objFn.validateExtraFieldsData = function () {
                    scope.extraFieldsErrorMessage = undefined;
                    var flag = false;
                    for (var i = 0; i < scope.objExtraFields.length; i++) {
                        if (scope.objExtraFields[i].IsRequired != undefined && (scope.objExtraFields[i].IsRequired == "True" || scope.objExtraFields[i].IsRequired == true) && (!scope.objExtraFields[i].Value)) {
                            scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                            flag = false;
                            if (scope.objExtraFields[i].ControlType == "ComboBox") {
                                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                                    if (scope.objExtraFields[i].Children[j].Value != undefined && scope.objExtraFields[i].Children[j].Value != "") {
                                        flag = true;
                                        scope.extraFieldsErrorMessage = undefined;
                                    }
                                }
                                if (flag == false) {
                                    scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                                    return true;
                                }
                            }
                            if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                                var flagChk = false;
                                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                                    if (scope.objExtraFields[i].Children[j].Value != undefined && scope.objExtraFields[i].Children[j].Value != "") {
                                        flagChk = true;
                                        flag = true;
                                        scope.extraFieldsErrorMessage = undefined;
                                    }
                                }
                                if (flagChk == false) {
                                    scope.extraFieldsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                                    return true;
                                }
                            }

                            if (flag == false) {
                                return true;
                            }
                        }

                    }
                };

                scope.objFn.checkVisibility = function () {
                    return scope.showExtraFieldsTab;
                };
                scope.objFn.resetExtraField = function (model) {
                    scope.createExtraFieldList(model);
                };
            }

            scope.updateExtraFieldValue = function (textObject) {
                var flag = false;
                if (scope.extraFieldModel && scope.extraFieldModel.Elements && scope.extraFieldModel.Elements.length > 0) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i] && scope.extraFieldModel.Elements[i].dictAttributes.ID == textObject.ID) {
                            var temp;
                            if (textObject.ControlType == "CheckBoxList") {
                                temp = CreateNewObjectForExtraFieldlist(textObject);
                            } else {
                                temp = CreateNewObjectForExtraField(textObject);
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.Value = temp.dictAttributes.Value;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.Value, scope.extraFieldModel.Elements[i].dictAttributes, "Value", temp.dictAttributes.Value);
                            }

                            flag = true;
                        }
                    }
                }
                if (flag == false) {
                    var obj;
                    if (textObject.ControlType == "CheckBoxList") {
                        obj = CreateNewObjectForExtraFieldlist(textObject);
                        if (scope.isFileCreation) {
                            scope.extraFieldModel.Elements.push(obj);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.extraFieldModel.Elements);
                        }
                    } else if (textObject.Value != undefined) {
                        obj = CreateNewObjectForExtraField(textObject);
                        if (scope.isFileCreation) {
                            scope.extraFieldModel.Elements.push(obj);
                        }
                        else {
                            $rootScope.PushItem(obj, scope.extraFieldModel.Elements);
                        }
                    }
                }
            };

            scope.updateExtraFieldHyperLinkValue = function (textObject) {
                var flag = false;
                if (scope.extraFieldModel && scope.extraFieldModel.Elements) {
                    for (var i = 0; i < scope.extraFieldModel.Elements.length; i++) {
                        if (scope.extraFieldModel.Elements[i].dictAttributes.ID == textObject.ID) {
                            var objItem = {
                                Name: "ExtraField", dictAttributes: {}, Elements: []
                            };
                            objItem.dictAttributes.ID = textObject.ID;
                            if (textObject.Value != undefined) {
                                objItem.dictAttributes.Value = textObject.Value;
                            }
                            if (textObject.URL != undefined) {
                                objItem.dictAttributes.URL = textObject.URL;
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.Value = objItem.dictAttributes.Value;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.Value, scope.extraFieldModel.Elements[i].dictAttributes, "Value", objItem.dictAttributes.Value);
                            }
                            if (scope.isFileCreation) {
                                scope.extraFieldModel.Elements[i].dictAttributes.URL = objItem.dictAttributes.URL;
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.extraFieldModel.Elements[i].dictAttributes.URL, scope.extraFieldModel.Elements[i].dictAttributes, "URL", objItem.dictAttributes.URL);
                            }

                            flag = true;
                        }
                    }
                }
                if (flag == false) {
                    var objItem = {
                        Name: "ExtraField", dictAttributes: {}, Elements: []
                    };
                    objItem.dictAttributes.ID = textObject.ID;
                    if (textObject.Value != undefined) {
                        objItem.dictAttributes.Value = textObject.Value;
                    }
                    if (textObject.URL != undefined) {
                        objItem.dictAttributes.URL = textObject.URL;
                    }
                    if (scope.isFileCreation) {
                        scope.extraFieldModel.Elements.push(objItem);
                    }
                    else {
                        $rootScope.PushItem(objItem, scope.extraFieldModel.Elements);
                    }
                }
            };

            var CreateNewObjectForExtraField = function (textObject) {
                var objItem = {
                    Name: "ExtraField", dictAttributes: {}, Elements: []
                };

                objItem.dictAttributes.ID = textObject.ID;
                if (textObject.Value) {
                    objItem.dictAttributes.Value = textObject.Value;
                }
                return objItem;
            };

            var CreateNewObjectForExtraFieldlist = function (textObject) {
                var val = "";
                var count = 0;
                for (var j = 0; j < textObject.Children.length; j++) {
                    if (textObject.Children[j].Value == "True") {
                        if (count == 0) {
                            val += textObject.Children[j].ID;
                        }
                        else {
                            val += "," + textObject.Children[j].ID;
                        }
                        count++;
                    }
                }

                var objItem = {
                    Name: "ExtraField", dictAttributes: {}, Elements: []
                };
                objItem.dictAttributes.ID = textObject.ID;
                if (val != "") {
                    objItem.dictAttributes.Value = val;
                }
                return objItem;
            };

            scope.CloseWizard = function () {
                scope.closeWizardFn();
            };

            scope.$watch("formName", function (newVal, oldVal) {
                if (newVal) {
                    scope.onFormChange();
                }
            });

            // #endregion 
        }
    };
}]);

app.directive("searchDesign", ["hubcontext", "$timeout", "$searchQuerybuilder", "$rootScope", "$searchFindReferences", "$interval", function (hubcontext, $timeout, $searchQuerybuilder, $rootScope, $searchFindReferences, $interval) {
    return {
        restrict: "E",
        scope: {
            toggleDesign: "&",
            traverseModel: "&",
            currentfile: "=currentFile",
            selectElementCallback: "&selectElement",
            mainModel: "="
        },
        templateUrl: "Common/views/SearchDesignWrapper.html",
        link: function (scope, element, attribute) {
            scope.deHighlightDesign = function (HighLightList) {
                if (HighLightList && HighLightList.length > 0) {
                    HighLightList.forEach(function (HighLightObj) {
                        if (HighLightObj.Objhighlight && HighLightObj.Objhighlight.length > 0) {
                            scope.toggleDesign({ items: HighLightObj.Objhighlight, IsAction: false });
                        }
                    });
                }
            };
            scope.highlightDesign = function (File) {
                var items = [];
                if (File.FileInfo.SelectNodePath != null && File.FileInfo.SelectNodePath.length > 1) {
                    if (scope.currentfile.FileType == "DecisionTable") {
                        items = scope.traverseModel({ path: File.FileInfo.SelectNodePath, strPath: File.ParentPath });
                    }
                    else {
                        items = scope.traverseModel({ path: File.FileInfo.SelectNodePath });
                    }
                    if (items.length > 0) {
                        scope.toggleDesign({ items: items, IsAction: true });
                    }
                }
                return items;
            };
            scope.SearchInDesign = function () {
                $rootScope.IsLoading = true;
                hubcontext.hubSearch.server.getFindAdvanceDesign(scope.DesignSearch.SearchObj, scope.currentfile).done(function (DesignNodes) {
                    scope.deHighlightDesign(scope.DesignSearch.HighLightList);
                    scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                        scope.DesignSearch.findSearchList = DesignNodes;
                        // grey effect comes here                
                        // iterate on each type
                        if (scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch) {
                            angular.forEach(scope.DesignSearch.findSearchList, function (item) {
                                // save a dictionary with path and object to traverse to de-highlight properties
                                var HighlightListItem = new scope.HighLightObj();
                                HighlightListItem.path = item.FileInfo.SelectNodePath;
                                HighlightListItem.Objhighlight = scope.highlightDesign(item);
                                scope.DesignSearch.HighLightList.push(HighlightListItem);
                            });
                        }
                        // if result is only one node - select it directly
                        if (DesignNodes.length > 0) {
                            scope.selectElement(DesignNodes[0]);
                        }
                        scope.queryBuilder.setQueryBuilderIsAdvanceOption(true);
                    });
                });
            };
            scope.selectElement = function (File) {
                scope.DesignSearch.SelectNodePath = File.FileInfo.SelectNodePath;
                if (scope.currentfile.FileType == "DecisionTable") {
                    scope.selectElementCallback({ path: File.ParentPath, strPath: File.FileInfo.SelectNodePath });
                }
                else {
                    scope.selectElementCallback({ path: File.FileInfo.SelectNodePath });
                }
            };
            scope.ClearSearchDesign = function () {
                scope.deHighlightDesign(scope.DesignSearch.HighLightList);
                scope.init();
            };
            // key defines when the operator is active - mode A for attribute, mode N for Node. "A" has precedency           
            scope.HighLightObj = function () {
                return { path: "", Objhighlight: {} };
            };
            scope.initAdvance = function () {
                // when file is opened
                // this handles scenario where you want to search with a query and get the references from the hub (ex - advance search, build rule navigation)
                if (scope.queryBuilder.getGlobalQueryBuilder.lstFilter.length > 0) {
                    if (!scope.DesignSearch) {
                        scope.DesignSearch = {};
                        scope.DesignSearch.HighLightList = [];
                        scope.DesignSearch.SearchObj = [];
                    }
                    $rootScope.IsLoading = true;
                    scope.DesignSearch.IsOpen = scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch ? true : false;
                    angular.copy(scope.queryBuilder.getGlobalQueryBuilder.lstFilter, scope.DesignSearch.SearchObj);
                    scope.queryBuilder.setGlobalQueryBuilderFilters([]);
                    scope.SearchInDesign();
                }
            };
            scope.init = function () {
                scope.queryBuilder = $searchQuerybuilder;
                scope.searchFindReferences = $searchFindReferences.getData;
                var deregisterActiveReferenceWatcher;
                // if file is open from advance search - trigger search on init
                if (scope.queryBuilder.getGlobalQueryBuilder.lstFilter.length <= 0) {
                    var isOpen = scope.DesignSearch && scope.DesignSearch.IsOpen && scope.queryBuilder.getGlobalQueryBuilder.isAdvanceSearch ? true : false;
                    scope.DesignSearch = {};
                    scope.DesignSearch.IsOpen = isOpen ? true : false;
                    scope.DesignSearch.SearchObj = [];
                    scope.DesignSearch.HighLightList = [];
                    $timeout(scope.queryBuilder.AddsearchQuerybuilderObj(scope.DesignSearch.SearchObj), 1000); // add one query by default
                }

                // If we add a left container after render, we need to watch and react
                deregisterActiveReferenceWatcher = scope.$watch(function () { return scope.searchFindReferences.activeReference; }, function (newValue, oldValue) {
                    if (!newValue || (($searchFindReferences.getData.activeReference && $searchFindReferences.getData.activeReference.FileInfo.FileName != scope.currentfile.FileName))) {
                        return;
                    }
                    var promise;
                    if ($searchFindReferences.getData.lstReferences.length > 0 && $searchFindReferences.getData.activeReference && ($searchFindReferences.getData.activeReference.FileInfo.FileName == scope.currentfile.FileName)) {
                        function checkBaseModelIsLoaded() {
                            if (scope.mainModel) {
                                $interval.cancel(promise);
                                var activeRef = angular.copy($searchFindReferences.getData.activeReference);
                                $searchFindReferences.setData(null, null, null);
                                scope.selectElement(activeRef);
                            }
                        }
                        promise = $interval(checkBaseModelIsLoaded, 1000);
                    }
                }, true);

                // Unbind when the file is closed
                element.on('$destroy', function () {
                    deregisterActiveReferenceWatcher();
                });

            };
            scope.init();
        }
    };
}]);

// now this directive we are not using - we are using title for display error
app.directive("displayErrors", ["$compile", "$ValidationService", "$timeout", function ($compile, $ValidationService, $timeout) {
    return {
        restrict: 'A',
        scope: {
            currObj: '='
        },
        link: function (scope, elem, attr) {

            scope.showErrors = function (event) {
                $(".duplicate-id-tooltip").remove();
                var htmlText = ['<div ng-if="currObj.errors" class="duplicate-id-tooltip">',
                    '<ul class="list-unstyled" margin-bottom: 0px!important;>',
                    '<li ng-repeat="(property, value) in currObj.errors">',
                    '<span error="{{value}}"></span>',
                    '</li>',
                    '</ul>',
                    '</div>'].join(' ');
                $('body').append($compile(htmlText)(scope));
                $timeout(function () {
                    var selector = $(".duplicate-id-tooltip");
                    if (selector.length > 0) selector.show();

                    var xVal = event.pageX;
                    var yVal = event.pageY;
                    var windowWidth = $(window).width(); //retrieve current window width
                    var windowHeight = $(window).height(); //retrieve current window height
                    //var documentWidth = $(document).width(); //retrieve current document width
                    //var documentHeight = $(document).height(); //retrieve current document height

                    var divSelector = selector[0].getBoundingClientRect();

                    if ((xVal + divSelector.width) >= windowWidth) {
                        xVal -= divSelector.width;
                    }
                    if ((yVal + divSelector.height) >= windowHeight) {
                        yVal -= divSelector.height;
                    }

                    selector.css({
                        display: "block",
                        left: xVal + 'px',
                        top: yVal + 'px',
                        "z-index": 10000
                    });
                });

            };
            scope.isEmpty = function (obj) {
                var flag = false;
                flag = $ValidationService.isEmptyObj(obj);
                return flag;
            };
            // scope.$apply();
        },
        template: function (elem, attr) {
            var htmlText = ['<span class="info-tooltip error-mark" ng-click="showErrors($event)" ng-if="isEmpty(currObj.errors)" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>'];
            return htmlText;
        }
        //template: function (elem, attr) {
        //    var htmlText = ['<div ng-if="currObj.errors" class="duplicate-id-tooltip">',
        //                        '<ul class="list-unstyled" margin-bottom: 0px!important;>',
        //                            '<li ng-repeat="(property, value) in currObj.errors">',
        //                               '<span> {{ value }}  </span>',
        //                            '</li>',
        //                        '</ul>',
        //                     '</div>'].join(' ');
        //  //  $(document).find('body').append($compile(htmlText));
        //    return htmlText;
        //}
    };
}]);

app.directive("showErrors", ["$ValidationService", function ($ValidationService) {
    return {
        restrict: 'A',
        scope: {
            model: '='
        },
        link: function (scope, elem, attr) {
            scope.getErrorMessages = function (obj) {
                var text = "";
                for (var key in obj) {
                    var value = obj[key];
                    if (value && angular.isObject(value)) {
                        value = scope.getErrorMessages(value);
                    }
                    text += value + "\n";
                }
                return text;
            };
            scope.isEmpty = function (obj) {
                var flag = false;
                flag = $ValidationService.isEmptyObj(obj);
                return flag;
            };
        },
        template: function (elem, attr) {
            var htmlText = ["<span class='error-mark' ng-if='isEmpty(model.errors)' ng-attr-title='{{ getErrorMessages(model.errors) }}' style='color:red!important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span>"];
            return htmlText;
        }
    };
}]);
app.directive("showErrorMessage", ["$ValidationService", function ($ValidationService) {
    return {
        restrict: 'A',
        scope: {
            model: '=',
            prefix: '='
        },
        link: function (scope, elem, attr) {
            scope.getErrorMessages = function (obj) {
                var text = "";
                for (var key in obj) {

                    var value = obj[key];
                    if (value && angular.isObject(value)) {
                        value = scope.getErrorMessages(value);
                    }
                    if (key && key.startsWith(scope.prefix)) {
                        text += value + "\n";
                    }
                }
                return text;
            };
            scope.isEmpty = function (obj) {
                var flag = false;
                for (var key in obj) {
                    if (key && key.startsWith(scope.prefix)) {
                        flag = true;
                    }
                }
                return flag;
            };
        },
        template: function (elem, attr) {
            var htmlText = ["<span class='info-tooltip' ng-if='isEmpty(model.errors)' ng-attr-title='{{ getErrorMessages(model.errors) }}' style='color:red!important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span>"];
            return htmlText;
        }
    };
}]);
/*************
  USE Directive 
  1. with px: Ex- <input type="text" class="form-control input-sm" title="{{model.dictAttributes.Width}}" ng-model='model.dictAttributes.Width' postfix="px" numbers-only undoredodirective model='model'>
  2. without px: Ex- <input type="text" class="form-control input-sm" title="{{model.dictAttributes.Width}}" ng-model='model.dictAttributes.Width' numbers-only undoredodirective model='model'>
****************/
app.directive('numbersOnly', function () {
    return {
        link: function (scope, element, attr, ngModelCtrl) {
            var units = ["em", "ex", "%", "px", "cm", "mm", "in", "pt", "pc", "ch", "rem", "vh", "vw", "vmin", "vmax"];
            element.on('keydown keyup', function (event) {
                var input = $(event.target);
                var text;
                text = input.val();
                var key = event.keyCode ? event.keyCode : event.which;
                //if (key == $.ui.keyCode.RIGHT || key == $.ui.keyCode.END) { //if press right arrow button and End button
                //    var startPos = input[0].selectionStart;
                //    var char = text.charAt(startPos);
                //    if (attr.postfix && attr.postfix.indexOf(char) > -1) return false;
                //}

                if (key == $.ui.keyCode.SPACE) return false; // space not allowed
                //if (key == $.ui.keyCode.DELETE) { // when Press delete button px should not be deleted
                //    if ((input[0].selectionStart == (text.length - 2)) && attr.postfix) {
                //        return false;
                //    }
                //}
                if (attr.postfix && text == attr.postfix) {
                    text = "";
                    input.val(text);
                }
                if (text) {
                    //if (attr.postfix && text.indexOf(attr.postfix) <= -1) {
                    //    input[0].value += attr.postfix;
                    //    text += attr.postfix;
                    //    var len = text.length;
                    //    input[0].setSelectionRange(input[0].selectionStart, len - 2);
                    //}

                    //input.val(text);
                    //if (attr.postfix) {
                    //    var len = text.length;
                    //    if (input[0].selectionStart > (len - 2)) {
                    //        input[0].setSelectionRange(input[0].selectionStart, len - 2);
                    //    }
                    //    input.focus();
                    //}
                }
            });
            element.on('blur', function (event) {
                var input = $(event.target);
                var text = input.val();
                if (attr.postfix) {
                    var inputText = scope.filterInput(input);
                    var result = units.some(function (ch) { if (inputText.unit && inputText.unit.toLowerCase() == ch.toLowerCase()) { return true; } });
                    if (!result && input[0].value && inputText.number && !isNaN(inputText.number)) {
                        input[0].value = inputText.number + "px";
                    } else if (!inputText.number) {
                        input[0].value = "";
                    }
                    element.trigger("change");
                }
            });
            element.on("paste", function (e) {
                e.preventDefault();
            });
            scope.filterInput = function ($input) {
                var newInput = "";
                var objInputText = { number: '', unit: '' };
                if ($input && $input[0].value) {
                    var value = $input[0].value;
                    value = value.split("");
                    var index = 0;
                    var valid = true;
                    if (angular.isArray(value)) {
                        while (index < value.length) {
                            if (valid && value[index] == ".") {
                                valid = false;
                                index++;
                                continue;
                            }
                            if (isNaN(value[index])) {
                                break;
                            }
                            index++;
                        }
                        objInputText.number = $input[0].value.substring(0, index);
                        objInputText.unit = $input[0].value.substring(index, $input[0].value.length);
                    }
                }
                return objInputText;
            }
        }
    };
});

app.directive("findReferencebox", [function () {
    return {
        restrict: 'E',
        templateUrl: 'Common/views/FindReferenceBox.html',
    };
}]);

// Use this directive where we used ng-repeat on li or tr and want to navigate list using up and down key
app.directive("keyUpDownWithElement", [function () {
    return {
        scope: {
            callbackfn: '=',
            objproperty: "=",
            property: "=",
            propvalue: "=",
            parentelem: "=",
            isscroll: "=",
            requireEvent: "="
        },
        link: function (scope, element, attr) {
            element.on("keydown", function (e) {
                scope.currentEvent = e;
                scope.nextElement = null;
                var skip = false;
                $(".ui-autocomplete").each(function () { // if intellisense list is open then skip navigation
                    var display = $(this).css("display");
                    if (display == "block") {
                        skip = true;
                    }
                });
                if (skip) {
                    return;
                }

                var elem = element;
                if (scope.parentelem) { // if used this directive inside li or tr item then pass the value of parentelem e.g 'li'
                    elem = $(elem).parents(scope.parentelem).first();
                }
                if (e.keyCode == $.ui.keyCode.UP) {
                    scope.findNextItem(elem, 'UP');
                    e.preventDefault();
                } else if (e.keyCode == $.ui.keyCode.DOWN) {
                    scope.findNextItem(elem, 'DOWN');
                    e.preventDefault();
                }
            });
            scope.findNextItem = function (elem, navigation) {
                var nextElement = null;
                var elemScope = null;
                var currentElement = elem;
                $(currentElement).find("input:focus").blur();

                if (navigation == "UP") {
                    nextElement = $(currentElement).prevAll(':visible:first');
                    if (nextElement.length <= 0) {
                        nextElement = $(currentElement).prev();
                    }
                } else if (navigation == "DOWN") {
                    nextElement = $(currentElement).nextAll(':visible:first');
                    if (nextElement.length <= 0) {
                        nextElement = $(currentElement).next();
                    }
                }
                if (nextElement.length <= 0) {
                    return;
                }

                if (scope.parentelem) { // if used this directive inside li or tr item then focus on it
                    var elemtemp = nextElement.find("[key-up-down-with-element]");
                    elemtemp.focus();
                } else {
                    nextElement.focus();
                }

                elemScope = angular.element(nextElement).scope();
                if (nextElement.is("[ng-if]") && elemScope) {
                    elemScope = elemScope.$parent;
                }

                if (elemScope && elemScope.hasOwnProperty(scope.objproperty)) {
                    var obj = elemScope[scope.objproperty];
                    //if li or tr items having hidden element and if you want to navigate then pass below two property with value
                    if (obj && obj.dictAttributes && obj.dictAttributes.hasOwnProperty(scope.property) && obj.dictAttributes[scope.property] == scope.propvalue) {
                        scope.findNextItem(nextElement, navigation);
                    } else {
                        scope.nextElement = obj != undefined ? obj : null;
                        if (scope.nextElement != null && angular.isFunction(scope.callbackfn)) {
                            scope.$evalAsync(function () {
                                if (scope.requireEvent) {
                                    scope.callbackfn(scope.nextElement, scope.currentEvent);
                                } else {
                                    scope.callbackfn(scope.nextElement);
                                }
                                var parents = nextElement.parents();
                                if (!scope.isscroll) {
                                    $.each(parents, function (key, value) {
                                        if ($(this).hasScrollBar()) {
                                            $(this).scrollTo(nextElement, { offsetTop: 170, offsetLeft: 0 }, null);
                                            return false;
                                        }
                                    });
                                }
                            });
                        }
                    }
                }
            };
        }
    }
}]);


app.directive("basemodelPropertiesrow", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: {
            prop: "=",
            clickCallback: "&",
            changeCallback: "&",
            category: "@",
            dbclickCallback: "&",
            showDeleteButton: "=",
            deletePropCallback: "&",
        },
        templateUrl: 'Form/views/ControlProperties/advancePropertiesCatergory.html',
        link: function (scope, element, attributes) {
            scope.disabledProplist = attributes.disabledProplist ? scope.$eval(attributes.disabledProplist) : [];
            if (scope.prop.PropertyName == "sfwIsCaption" && scope.prop.PropertyValue == "True") {
                scope.disabledProplist.push("sfwIsCaption");
            }
            $(element).find("td:first-child").resizable({
                handles: "e", minWidth: 15,
                resize: function (event, ui) {
                    var lstTD = $(ui.element).closest("tbody").find("td:not(.advanced-prop-head):first-child");
                    $(lstTD).css("width", ui.size.width);
                    $(lstTD).find("div[advance-prop-caption-wrapper]").css("width", ui.size.width);
                }
            });
        }
    };
}]);
app.directive("workflowDrop", ["$rootScope", function ($rootScope) {
    return {
        restrict: 'A',
        scope:
        {
            dropData: "=",
            allowdrop: "="

        },

        link: function (scope, element, attrs) {
            var el = element[0];
            el.addEventListener('drop', dropHander, false);
            el.addEventListener("dragover", function (event) {
                event.preventDefault();
            });
            function dropHander(e) {
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                if (scope.allowdrop) {
                    // Stops some browsers from redirecting.
                    $(e.currentTarget).trigger("click");
                    var dataobject = dragDropDataObject;
                    var data = dragDropData;
                    scope.$apply(function () {
                        if (dataobject && (dataobject.DataType == "ValueType" || dataobject.DataType == "BusObjectType")) {
                            if (scope.dropData) {

                                $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwObjectField, scope.dropData.dictAttributes, 'sfwObjectField', data);
                                if (dataobject.ItemPath) {
                                    var objectID = dataobject.ItemPath.substr(0, dataobject.ItemPath.indexOf('.'));
                                    $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwObjectID, scope.dropData.dictAttributes, 'sfwObjectID', objectID);

                                }


                                var dataType = getDataType(dataobject.DataType, dataobject.DataTypeName);
                                $rootScope.EditPropertyValue(scope.dropData.dictAttributes.sfwDataType, scope.dropData.dictAttributes, 'sfwDataType', dataType);


                            }

                        }
                    });
                    dragDropData = null;
                    dragDropDataObject = null;

                }
                else {
                    return false;
                }
            }
        }

    }

}]);

app.directive('multiValueinput', ["$compile", "$rootScope", "CONSTANTS", function ($compile, $rootScope, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            propertyvalue: "=",
            propertyname: "=",
            separator: "@"
        },
        templateUrl: "Common/views/Directives/multivalueinput.html",
        link: function (scope, element, attrs) {
            scope.onAddMultipleValueClick = function () {
                var newScope = scope.$new(true);
                newScope.lstPropertyValues = [];
                var lstPropertyValues = scope.propertyvalue ? scope.propertyvalue.split(scope.separator) : [];
                if (lstPropertyValues.length > 0) {
                    newScope.lstPropertyValues = $.map(lstPropertyValues, function (obj) {
                        return {
                            value: obj
                        }
                    });
                }
                var objDialogMultiValueinput = $rootScope.showDialog(newScope, "Set Value(s)", "Common/views/AddMultipleInputValue.html", { width: 400, height: 300 });
                newScope.AddValue = function () {
                    newScope.lstPropertyValues.push({ value: "" });
                };
                newScope.deletevalue = function (index) {
                    if (index >= 0) {
                        newScope.lstPropertyValues.splice(index, 1);
                    }
                };
                newScope.setValueClick = function () {
                    if (newScope.lstPropertyValues.length > 0) {
                        var arrvalues = $.map(newScope.lstPropertyValues, function (obj) {
                            if (obj.value) {
                                return obj.value;
                            }
                        });
                        scope.propertyvalue = arrvalues.join(scope.separator);
                    }
                    objDialogMultiValueinput.close();
                };
                newScope.cancelClick = function () {
                    objDialogMultiValueinput.close();
                }
            };
            function init() {
                // initial config will come here
                scope.separator = scope.separator ? scope.separator : ",";

            }
            init();
        }
    };
}]);

app.directive("xmlTree", function () {
    return {
        scope: {
            data: '=',
            selectfunction: '='
        },
        template: '<span ng-repeat="node in data.Elements">' +
            '<span xml-node-tree-directive data="node" selectfunction="selectfunction"></span>' +
            '</span>',
        link: function (scope) {

        }
    }
});

app.directive("xmlNodeTreeDirective", ["$timeout", "$rootScope", function ($timeout, $rootScope) {
    return {
        scope: {
            data: '=',
            selectfunction: '='
        },
        template: '<ul class="formtree">' +
            '<li ng-repeat="item in getColumns(data)" style="cursor: pointer;" data-ng-click="selectElement(item,$event)">' +
            '<span mainspan><i ng-show="item.Elements.length>0 && item.Elements[0].Name != \'ListItem\' && item.Elements[0].Name != \'Columns\'" ng-class="item.isExpand?\'fa fa-caret-down\':\'fa fa-caret-right\'" ng-click="toggle(item,$event)" aria-hidden="true"></i>' +
            '<span ng-bind="item.Name"></span> <span> - </span> <span>{{ item.Name == "sfwPanel" && item.dictAttributes.sfwCaption?item.dictAttributes.sfwCaption: item.dictAttributes.ID }}</span></span>' +
            '<span ng-show="item.isExpand" ng-if="item.Elements.length > 0" xml-tree data="item" selectfunction="selectfunction"></span>' +
            '</li>' +
            '</ul>',
        link: function (scope, ele, attr) {
            scope.toggle = function (item, event) {
                item.isExpand = !item.isExpand;
                //  scope.toggleExpander(event.currentTarget);
            };
            scope.toggleExpander = function (element) {
                var elem = $(element).parents("li").first().find("ul").first();
                $timeout(function () {
                    //$(elem).find("first:li").children('ul.formtree').slideToggle(300);
                    if ($(element).hasClass("fa fa-caret-down")) {
                        $(elem).slideDown("normal");
                    }
                    else {
                        // $(elem).find(".selected-tree-item").not().parent('ul .selected-tree-item').removeClass("selected-tree-item");
                        $(elem).slideUp("normal");
                    }
                });
            }
            scope.selectElement = function (item, event, elem) {
                //$(".formtree [mainspan]").find(".selected-tree-element").removeClass("selected-tree-element");
                var domLi = null;
                if (event) {
                    domLi = $(event.currentTarget);
                    event.stopPropagation();
                } else if (elem) {
                    domLi = elem;
                }
                $(".selected-tree-item").removeClass("selected-tree-item");
                $(domLi).find("[mainspan]:first").addClass("selected-tree-item");
                if (item) {
                    scope.selectedItem = item;
                    $timeout(function () {
                        scope.selectfunction(scope.selectedItem);
                    });
                }

            };
            scope.getColumns = function (model) {
                var list = [];
                if (model.Elements.length > 0) {
                    for (var i = 0; i < model.Elements.length; i++) {
                        var obj = model.Elements[i];
                        //  console.log(obj.Name);
                        if (obj.Name == "sfwTabSheet") {
                            list = list.concat(obj);
                        } else if (obj.Name == "sfwColumn") {
                            list = list.concat(obj.Elements);
                            continue;
                        }
                        if (obj && obj.Name != "sfwColumn" && obj.Name != "sfwTabSheet" && obj.Name != "ListItem" && obj.Name != "Columns") {
                            list = list.concat(scope.getColumns(obj));
                        }
                    }
                }
                return list;
            };
            //scope.$watch("inputid", function () {
            //    scope.searchById(scope.parent, scope.inputid);
            //});
            if (!$rootScope.$$listenerCount['searchById']) {
                $rootScope.$on('searchById', function (event, data) {
                    console.log("called function");
                    scope.searchById(data.model, data.input);
                });
            }
            scope.searchById = function (items, id) {
                var idFound = false;
                var recursiveFilter = function (items, id) {
                    angular.forEach(items.Elements, function (item) {
                        if (item && item.dictAttributes.ID && id && item.dictAttributes.ID.toLowerCase() == id.toLowerCase()) {
                            idFound = true;
                            scope.$evalAsync(function () {
                                var elem = getScope(item);
                                scope.selectElement(item, null, elem);
                                $timeout(function () {
                                    expandParent(item);
                                });
                            });
                        }
                        if (item && angular.isArray(item.Elements) && item.Elements.length > 0 && !idFound) {
                            // item.isExpand = true;
                            recursiveFilter(item, id);
                        }
                    });
                };


                var expandParent = function (item) {
                    var parent = item;
                    while (parent) {
                        parent.isExpand = true;
                        if (parent.ParentVM) {
                            parent = parent.ParentVM;
                        } else {
                            parent = null;
                        }
                    }
                };

                var getScope = function (item) {
                    var elem;
                    $('.formtree li').each(function () {
                        var elemScope = angular.element(this).scope();
                        if (elemScope && elemScope.item == item) {
                            elem = $(this);
                            return false; // stop looking at the rest
                        }
                    });
                    return elem;
                }

                recursiveFilter(items, id);
            };

        }
    }
}]);

app.directive("keyUpDownForTree", ["$timeout", function ($timeout) {
    return {
        scope: {
            callbackfn: '=',
            objproperty: "=",
            property: "=",
            propvalue: "=",
            parentelem: "=",
            objparent: "=",
            isscroll: "="
        },
        link: function (scope, element, attr) {
            element.on("keydown", function (e) {

                scope.nextElement = null;
                var skip = false;
                $(".ui-autocomplete").each(function () { // if intellisense list is open then skip navigation
                    var display = $(this).css("display");
                    if (display == "block") {
                        skip = true;
                    }
                });
                if (skip) {
                    return;
                }

                var elem = element;
                if (scope.parentelem) { // if used this directive inside li or tr item then pass the value of parentelem e.g 'li'
                    elem = $(elem).parents(scope.parentelem).first();
                }
                if (e.keyCode == $.ui.keyCode.UP) {
                    scope.findNextItem(elem, 'UP');
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.DOWN) {
                    scope.findNextItem(elem, 'DOWN');
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.RIGHT) {
                    scope.findNextItem(elem, "RIGHT");
                    e.preventDefault();
                    e.stopPropagation();
                } else if (e.keyCode == $.ui.keyCode.LEFT) {
                    scope.findNextItem(elem, "LEFT");
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
            scope.findNextItem = function (elem, navigation) {
                var nextElement = null;
                var elemScope = null;
                var currentElement = elem;
                $(currentElement).find("input:focus").blur();

                if (navigation == "UP") {
                    nextElement = $(currentElement).prevAll(':visible:first');  // finding in current li previous siblings 
                    if (nextElement && nextElement.length > 0) {
                        var lastElem = $(nextElement).find("li:visible:last"); // if li previous having another children list then move to childrens last li
                        if (lastElem && lastElem.length > 0) {
                            nextElement = lastElem;
                        }
                    }
                    if (nextElement && nextElement.length <= 0) {
                        nextElement = $(currentElement).parents("li:visible:first"); // move to li first parent level li
                    }
                    if (nextElement.length <= 0) {  // move to li next sibling li
                        nextElement = $(currentElement).prev();
                    }
                    scope.goToNextElement(nextElement, currentElement, navigation);
                } else if (navigation == "DOWN") {
                    nextElement = $(currentElement).find("li:visible:first");  // find first decendent of current li next element
                    if (nextElement && nextElement.length <= 0) {
                        nextElement = $(currentElement).nextAll(':visible:first');  // move next li siblings next element
                    }
                    if (nextElement.length <= 0) {  //move to next element
                        nextElement = $(currentElement).next();
                    }
                    if (nextElement && nextElement.length <= 0) {    //  find next li if having siblings then go to parents first li next siblings li
                        nextElement = $(currentElement).parents("li:visible:first").next();
                        var elementParents = $(currentElement).parents("li:visible");
                        var index = 0;
                        while (elementParents && elementParents.length > index) {
                            var parentElem = elementParents[index];
                            if (parentElem) {
                                var siblings = $(parentElem).next().siblings('li');
                                if (siblings.length) {
                                    nextElement = $(parentElem).next();
                                    break;
                                }
                            }
                            index++;
                        }
                        $(currentElement).parents("li:visible").next().siblings('li');
                    }
                    scope.goToNextElement(nextElement, currentElement, navigation);
                } else if (navigation == "RIGHT") {
                    var currentElemScope = angular.element(currentElement).scope();
                    if (currentElemScope && currentElemScope.hasOwnProperty(scope.objproperty)) {
                        var currentElemScope = currentElemScope[scope.objproperty];
                        if (currentElemScope && currentElemScope.Elements.length > 0) {
                            currentElemScope.IsExpanded = true;
                        }
                        $timeout(function () {
                            //nextElement = $(currentElement).find("ul li:first");
                            scope.goToNextElement(currentElement, currentElement, navigation);
                        });
                    }

                } else if (navigation == "LEFT") {
                    var currentElemScope = angular.element(currentElement).scope();
                    if (currentElemScope && currentElemScope.hasOwnProperty(scope.objproperty)) {
                        var currentElemScope = currentElemScope[scope.objproperty];
                        if (currentElemScope && currentElemScope.Elements.length > 0) {
                            currentElemScope.IsExpanded = false;
                        }
                        $timeout(function () {
                            //nextElement = $(currentElement).parents("li:visible:first").next();
                            scope.goToNextElement(currentElement, currentElement, navigation);
                        });
                    }
                }

            };
            scope.goToNextElement = function (nextElem, currentElement, navigation) {
                var nextElement = nextElem;
                if (nextElement && nextElement.length <= 0) {
                    if (navigation == "UP") {
                        nextElement = $(currentElement).parents("li:visible:first");
                    } else if (navigation == "DOWN") {
                        nextElement = $(currentElement).find("ul").find("li:first");
                    }
                    //  return;
                }

                if (scope.parentelem) { // if used this directive inside li or tr item then focus on it
                    var elemtemp = nextElement.find("[key-up-down-with-element]");
                    elemtemp.focus();
                } else {
                    nextElement.focus();
                }

                elemScope = angular.element(nextElement).scope();
                if (nextElement.is("[ng-if]") && elemScope) {
                    elemScope = elemScope.$parent;
                }

                if (elemScope && elemScope.hasOwnProperty(scope.objproperty)) {
                    var obj = elemScope[scope.objproperty];
                    scope.nextElement = obj != undefined ? obj : null;
                    if (scope.nextElement != null && angular.isFunction(scope.callbackfn)) {
                        scope.$evalAsync(function () {
                            var parentObj = null;
                            if (scope.objparent) {
                                parentObj = scope.objparent;
                            }
                            scope.callbackfn(scope.nextElement, parentObj);
                            var parents = nextElement.parents();
                            if (!scope.isscroll) {
                                $.each(parents, function (key, value) {
                                    if ($(this).hasScrollBar()) {
                                        $(this).scrollTo(nextElement, { offsetTop: 170, offsetLeft: 0 }, null);
                                        return false;
                                    }
                                });
                            }
                        });
                    }
                }
            }
        }
    }
}]);

//app.directive("knowtionId", ["hubcontext", "$timeout", "$searchQuerybuilder", "$rootScope", "$searchFindReferences", "$interval", function (hubcontext, $timeout, $searchQuerybuilder, $rootScope, $searchFindReferences, $interval) {
//    return {
//        restrict: "A",
//        link: function (scope, element, attribute) {
//            $(element).attr("tabindex", "0");
//            $(element).focus();
//            $(element).on("keydown", function () {
//                if (event.keyCode == 112 && event.key == "F1") // while pressing F1 key
//                {
//                    scope.showKnowtionHelp();
//                    event.stopPropagation();
//                }
//            });

//            scope.showKnowtionHelp = function () {
//                $rootScope.showHelp(attribute.knowtionId)
//            }
//        }
//    };
//}]);

