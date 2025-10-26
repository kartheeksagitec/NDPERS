app.directive('scrolldirective', [function () {
    return {
        restrict: 'A',
        scope: {
            scopeid: "="
        },
        link: function (scope, element, attrs) {
            var raw = element[0];
            console.log('loading directive');
            var scrollTopValue = 0;
            element.bind('scroll', function () {
                if (raw.scrollTop + raw.offsetHeight > raw.scrollHeight && raw.scrollTop > scrollTopValue) { //at the bottom
                    scrollTopValue = raw.scrollTop - 10;
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    var scope = GetCurrentScopeObject("Main");
                    scope.$apply(function () {
                        scope.LoadSomeData();
                    });
                }
            });
        }

    };
}]);

app.directive('customvaluedraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            if (el.localName == "li") {
                //alert(el.localName);
                //alert(scope.dragdata);"Age Years"
            }
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                if (scope.dragdata != undefined && scope.dragdata != '') {
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                    dragDropData = scope.dragdata;
                }
            }
        },
    };
}]);


app.directive("dropvalue", [function () {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            scopeid: '=',
            parameter: '='
            //  selectedRuleFromGroup: '='
        },
        link: function (scope, element, attributes) {

            var el = element[0];
            el.addEventListener("dragover", DragOver, false);
            el.addEventListener("drop", Drop, false);
            //el.addEventListener('dragleave', DragLeave, false);


            function DragOver(e) {
                e.dataTransfer.dropEffect = 'copy';

                if (scope.dropdata != undefined && scope.dropdata != '') {
                    if (e.preventDefault) {
                        e.preventDefault();
                    }
                }

                return false;

            }
            function Drop(e) {
                //var Scope = GetCurrentScopeObject("Main");
                var Scope = angular.element(document.getElementById("ScopeId_" + scope.scopeid)).scope();
                e.preventDefault();
                var data = JSON.parse(e.dataTransfer.getData("text"));
                Scope.$apply(function () {
                    if (data != undefined && data != '') {
                        if (scope.dropdata == 'input') {
                            if (!Scope.ObjSelectedlstValuesData.isExistingRule) {
                                Scope.setInputValues(data);
                            }
                        }
                        else if (scope.dropdata == 'output') {
                            if (!Scope.ObjSelectedlstValuesData.isExistingRule) {
                                Scope.setOutputValues(data);
                            }
                        }
                        else if (scope.dropdata == 'return') {
                            if (!Scope.ObjSelectedlstValuesData.isExistingRule) {
                                Scope.setReturnParameter(data);
                            }
                        }
                        else if (scope.dropdata == 'distinctfile') {
                            Scope.DistinctFileValue = Scope.lstDummyColumns[data - 1];
                        }
                        else if (scope.dropdata == 'xvalue') {
                            Scope.XAxisCellValue = Scope.lstDummyColumns[data - 1];
                        }
                        else if (scope.dropdata == 'yvalue') {
                            Scope.YAxisCellValue = Scope.lstDummyColumns[data - 1];
                        }
                        else if (scope.dropdata == 'value') {
                            Scope.Value = Scope.lstDummyColumns[data - 1];
                        } else if (scope.dropdata) {
                            Scope.setValuesForExistingParameter(scope.dropdata, data, scope.parameter);
                        }
                    }
                });

                if (e.stopPropagation) e.stopPropagation();
            }

            function DragLeave(e) {

            }

        }
    };
}]);

app.directive("validationruleintellisense", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modebinding: "=",
            cssclass: '=',
            propertyname: '=',
        },
        link: function (scope, element, attrs) {
            // var parent = $(element).parent();
            // element.html("<input type='text' ng-class='cssclass' title='{{modebinding}}' ng-model='modebinding' ng-keydown='onkeydown($event)'  undoredodirective  model='model' propertyname='propertyname' />");
            element.html("<div class='input-group'><input type='text' ng-class='cssclass' title='{{modebinding}}' ng-model='modebinding' ng-keydown='onkeydown($event)'  undoredodirective  model='model' propertyname='propertyname' /><span class='input-group-addon button-intellisense' ng-click='showValidationRules($event)'></span></div>");
            $compile(element.contents())(scope);
            scope.onkeydown = function (event) {
                var input = $(event.target);
                if (input.val() == undefined || input.val().trim() == "") {
                    var data = [];
                    controllerScope = getCurrentFileScope();
                    if (controllerScope) {
                        var rules = controllerScope.objEntity.Elements.filter(function (x) {
                            return x.Name == "rules";
                        });
                        if (rules && rules.length > 0) {
                            var validationRules = getDescendents(rules[0]);
                            if (validationRules && validationRules.length > 0) {
                                validationRules = validationRules.filter(function (x) {
                                    return x.Name == "rule";
                                });
                                angular.forEach(validationRules, function (rule) {
                                    data.push({ ID: rule.dictAttributes.ID });
                                });
                                if (controllerScope.objInheritedRules && controllerScope.objInheritedRules.Elements && controllerScope.objInheritedRules.Elements.length > 0) {
                                    angular.forEach(controllerScope.objInheritedRules.Elements, function (rule) {
                                        data.push({ ID: rule.dictAttributes.ID });
                                    });
                                }
                            }
                        }
                    }
                    setSingleLevelAutoComplete(input, data);
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
            };

            scope.showValidationRules = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prev();
                }
                scope.inputElement.focus();
                var data = [];
                var controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    var rules = controllerScope.objEntity.Elements.filter(function (x) {
                        return x.Name == "rules";
                    });
                    if (rules && rules.length > 0) {
                        var validationRules = getDescendents(rules[0]);
                        if (validationRules && validationRules.length > 0) {
                            validationRules = validationRules.filter(function (x) {
                                return x.Name == "rule";
                            });
                            angular.forEach(validationRules, function (rule) {
                                data.push({ ID: rule.dictAttributes.ID });
                            });
                        }
                    }
                }
                if (scope.inputElement && data) {
                    setSingleLevelAutoComplete(scope.inputElement, data, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
        },
        //template: "<input type='text' ng-class='cssclass' ng-model='modebinding' ng-keydown='onkeydown($event)'    />"
    };
}]);

app.directive("objecttreeintellisensetemplate", ["$compile", "$rootScope", "$EntityIntellisenseFactory", 'CONSTANTS', '$ValidationService', function ($compile, $rootScope, $EntityIntellisenseFactory, CONST, $ValidationService) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            busobject: '=',
            model: "=",
            objtree: "=",
            objectfield: "=",
            isloadmethod: "=",
            allowdrop: "=",
            isxmlmethod: "=",
            isdisabled: '=',
            onselectioncallback: '&',
            propertyname: "=",
            insidegrid: "=",
            entityname: '=',
            isloadonlymethod: '=',
            parentmodel: '=',
            errorprop: '=',
            isshowcollection: '=',
            validate: '='
        },
        template: "<div class='input-group'><span class='info-tooltip' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input ng-class=\"insidegrid?'form-control form-filter input-sm':'form-control input-sm'\" type='text' ng-change='validateEntityField()' ng-disabled='isdisabled' title='{{objectfield}}' ng-model='objectfield' ng-keydown='onObjectFieldKeyDown($event)'  undoredodirective model='model' propertyname='propertyname' ng-blur='onTextBoxblur()' /><span class='input-group-addon button-intellisense' ng-disabled='isdisabled' ng-click='showObjectFieldList($event)'></span></div>",
        link: function (scope, element, attrs) {
            scope._objectField = scope.objectfield;
            Object.defineProperty(scope, "objectfield", {
                get: function () {
                    return scope._objectField;
                },
                set: function (value) {
                    var oldval = scope._objectField;
                    scope._objectField = value;
                    setPropertyValue(scope.model, scope.propertyname, value);
                    if (scope.inputElement) {
                        scope.objectFieldTextChanged();
                        scope.selectionCallback();
                    }
                },
            });

            var unwatch = scope.$watch('busobject', function (newVal, oldVal) {
                if (newVal !== oldVal) {
                    scope.objtree = undefined;
                }
            });

            scope.onTextBoxblur = function () {
                scope.inputElement = undefined;
            };
            scope.LoadObjectTree = function () {
                if (scope.busobject) {
                    $.connection.hubEntityModel.server.loadObjectTree(scope.busobject, "", true).done(function (data) {
                        scope.$evalAsync(function () {
                            scope.receiveObjectTree(data);
                            scope.Init();
                            scope.objectFieldTextChanged();
                            if ($(scope.inputElement).data('ui-autocomplete') !== undefined) {
                                $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            }
                        });
                    });
                }
            };

            scope.receiveObjectTree = function (data, path) {
                data = JSON.parse(data);
                var obj = data;
                if (path != undefined && path != "") {
                    var busObject = getBusObjectByPath(path, scope.objtree);
                    if (busObject && busObject.ItemType.Name == obj.ObjName) {
                        busObject.ChildProperties = obj.ChildProperties;
                        busObject.lstMethods = obj.lstMethods;
                        busObject.HasLoadedProp = true;
                    }
                    SetParentForObjTreeChild(busObject);
                }
                else {
                    scope.objtree = obj;
                    //Instead of re-initializing the array, clearing it so that it won't 
                    //loose the reference which we are already using while creating new xml method.

                    scope.objtree.IsMainBusObject = true;
                    scope.objtree.IsVisible = true;
                    SetParentForObjTreeChild(scope.objtree);


                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var objEntityIntellisense = entityIntellisenseList.filter(function (entity) {
                        return entity.ID == scope.entityname;
                    });

                    if (objEntityIntellisense != undefined && objEntityIntellisense.length > 0 && objEntityIntellisense[0].Rules.length > 0) {
                        scope.objtree.Rules = objEntityIntellisense[0].Rules;
                    }
                }
            };


            scope.getData = function (data, childProps) {
                var objData;
                angular.forEach(childProps, function (prop) {
                    if (objData == undefined) {
                        if (prop.ShortName == data) {
                            objData = prop;
                            return objData;
                        }

                        if (prop.ChildProperties.length > 0) {
                            objData = scope.getData(data, prop.ChildProperties);
                            if (objData != undefined) {
                                return objData;
                            }
                        }
                    }
                });
                return objData;
            };

            scope.SetIntellisenseData = function (data, objModel, text) {
                if (text) {
                    var busObject = getBusObjectByPath(text, objModel);
                    if (busObject && busObject.ItemType.Name == data.ObjName) {
                        busObject.ChildProperties = data.ChildProperties;
                        busObject.lstMethods = data.lstMethods;
                        busObject.HasLoadedProp = true;
                    }
                }
            };

            scope.Init = function () {
                if (scope.objectfield != undefined && scope.objectfield != "") {

                    var arrText = scope.objectfield.split('.');
                    var text = "";
                    if (arrText.length > 0 && scope.objtree) {
                        var prop = scope.objtree;
                        if (prop && prop.ChildProperties) {
                            for (var index = 0; index < arrText.length; index++) {
                                if (prop && prop.ChildProperties.length > 0) {
                                    prop = scope.getData(arrText[index], prop.ChildProperties);
                                    if (prop && (prop.HasLoadedProp == undefined || !prop.HasLoadedProp)) {
                                        var lst = arrText.filter(function (value, key) {
                                            return key > index;
                                        });
                                        if (prop.DataType != "ValueType") {
                                            text += arrText[index];
                                            $.connection.hubEntityModel.server.loadObjectTreeIntellisenseForProp(prop.ItemType.Name, lst, text).done(function (data) {
                                                scope.$evalAsync(function () {
                                                    if (scope.objtree != undefined) {
                                                        var obj = data;
                                                        scope.SetIntellisenseData(obj, scope.objtree, text);

                                                        if ($(document.activeElement).length > 0) {
                                                            if (document.activeElement.localName == "input") {
                                                                if ($(document.activeElement).val().contains(".") && $(document.activeElement).val().lastIndexOf(".") == $(document.activeElement).val().length - 1) {
                                                                    setMultilevelAutoCompleteForObjectTreeIntellisense($(document.activeElement), obj.ChildProperties, "ShortName", undefined, scope);
                                                                }
                                                            }
                                                            else if (document.activeElement.localName == "textarea") {
                                                                var autoCompleteData = obj.ChildProperties;
                                                                autoCompleteData = autoCompleteData.concat(obj.lstMethods);
                                                                setMultilevelAutoCompleteForObjectTreeIntellisense($(document.activeElement), obj.ChildProperties, "ShortName", undefined, scope);
                                                            }
                                                        }
                                                    }
                                                });
                                            });
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            scope.selectionCallback = function () {
                scope.$evalAsync(function () {
                    if (scope.inputElement) {
                        var prop;
                        if (scope.inputElement.val().contains('.')) {
                            var text = scope.inputElement.val();
                            text = text.substring(0, text.lastIndexOf('.'));
                            if (text) {
                                prop = getBusObjectByPath(text, scope.objtree);
                            }
                        }
                        else {
                            prop = scope.objtree;
                        }

                        if (prop && prop.ChildProperties && prop.ChildProperties.length > 0 && scope.inputElement.val().length > 0) {
                            var item = prop.ChildProperties.filter(function (x) { return x.FullPath == scope.inputElement.val().trim(); });
                            if (scope.model) {
                                if (item.length > 0) {
                                    scope.model.selecteditembusinessobjectname = item[0].ItemType.Name;
                                }
                                else {
                                    scope.model.selecteditembusinessobjectname = "";
                                }
                            }
                        }

                        if (scope.onselectioncallback) {
                            var value = scope.inputElement ? scope.inputElement.val() : "";
                            scope.onselectioncallback({ id: value, model: scope.model });
                        }
                    }
                });
            };
            scope.objectFieldTextChanged = function () {
                if (!scope.objtree) {
                    if (scope.busobject) {
                        scope.LoadObjectTree();
                    }
                }
                else {
                    // if (scope.onselectioncallback) {
                    //    if (scope.model && scope.propertyname) {
                    //        setPropertyValue(scope.model, scope.propertyname, scope.objectfield);
                    //    }
                    //     scope.onselectioncallback();
                    //}


                    if (scope.objtree != undefined && scope.inputElement) {
                        if (scope.inputElement.val() == undefined || scope.inputElement.val().trim() == "") {
                            if (scope.isxmlmethod) {
                                data = [];
                                if (scope.objtree.ChildProperties && !scope.isloadonlymethod) {
                                    data = data.concat(scope.objtree.ChildProperties);
                                }
                                if (scope.objtree.lstMethods) {
                                    data = data.concat(scope.objtree.lstMethods);
                                }
                                if (scope.objtree.Rules && !scope.isloadonlymethod) {
                                    data = data.concat(scope.objtree.Rules);
                                }
                                if (scope.isloadonlymethod) {
                                    setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description", scope.selectionCallback);
                                }
                                else {
                                    setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, data, "ShortName", scope.onselectioncallback, scope);
                                }
                            }
                            else {
                                if (scope.isloadonlymethod) {
                                    setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description", scope.selectionCallback);
                                }
                                else {
                                    var intellisensedata = [];
                                    if (scope.isshowcollection == false) {
                                        intellisensedata = scope.getFilteredProperties(scope.objtree.ChildProperties);
                                    } else {
                                        intellisensedata = scope.objtree.ChildProperties;
                                    }
                                    setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, intellisensedata, "ShortName", scope.onselectioncallback, scope);
                                }
                            }
                        }
                        else {
                            if (scope.inputElement.val().contains('.')) {
                                var text = scope.inputElement.val();
                                var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                                text = arrText.join(".");
                                text = text.substring(0, text.lastIndexOf('.'));
                                var prop = getBusObjectByPath(text, scope.objtree);
                                if (!prop && scope.objtree) {
                                    prop = scope.objtree;
                                }
                                if (prop) {
                                    if (scope.isloadonlymethod) {
                                        setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description");
                                    }
                                    else {
                                        var intellisensedata = [];
                                        if (scope.isshowcollection == false) {
                                            intellisensedata = scope.getFilteredProperties(prop.ChildProperties);
                                        } else {
                                            intellisensedata = prop.ChildProperties;
                                            if (prop.lstMethods && prop.lstMethods.length > 0) {
                                                intellisensedata = intellisensedata.concat(prop.lstMethods);
                                            }
                                        }
                                        setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, intellisensedata, "ShortName", scope.onselectioncallback, scope);

                                    }
                                }
                            }
                            else {
                                if (scope.objtree != undefined) {
                                    if (scope.isxmlmethod) {
                                        data = [];
                                        if (scope.objtree.ChildProperties && !scope.isloadonlymethod) {
                                            data = data.concat(scope.objtree.ChildProperties);
                                        }
                                        if (scope.objtree.lstMethods) {
                                            data = data.concat(scope.objtree.lstMethods);
                                        }
                                        if (scope.objtree.Rules && !scope.isloadonlymethod) {
                                            data = data.concat(scope.objtree.Rules);
                                        }
                                        if (scope.isloadonlymethod) {
                                            setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description");
                                        }
                                        else {
                                            setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, data, "ShortName", scope.onselectioncallback, scope);
                                        }
                                    }
                                    else {
                                        if (scope.isloadonlymethod) {
                                            setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description");
                                        }
                                        else {
                                            var intellisensedata = [];
                                            if (scope.isshowcollection == false) {
                                                intellisensedata = scope.getFilteredProperties(scope.objtree.ChildProperties);
                                            } else {
                                                intellisensedata = scope.objtree.ChildProperties;
                                            }
                                            setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, intellisensedata, "ShortName", scope.onselectioncallback, scope);
                                        }
                                    }
                                }
                            }

                        }
                    }

                }
            };

            scope.getFilteredProperties = function (attributes) {
                var lstProperties = [];
                for (var i = 0; i < attributes.length > 0; i++) {
                    if (attributes[i].DataType == "ValueType" || attributes[i].DataType == "BusObjectType" || attributes[i].DataType == "TableObjectType" || attributes[i].DataType == "OtherReferenceType") {
                        lstProperties.push(attributes[i]);
                    }
                }
                return lstProperties;
            };
            scope.showObjectFieldList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                var data = [];
                if (!scope.objtree) {
                    if (scope.busobject) {
                        scope.LoadObjectTree();
                    }
                }
                if (scope.isxmlmethod) {

                    if (scope.objtree && scope.objtree.ChildProperties && !scope.isloadonlymethod) {
                        data = data.concat(scope.objtree.ChildProperties);
                    }
                    if (scope.objtree && scope.objtree.lstMethods) {
                        data = data.concat(scope.objtree.lstMethods);
                    }
                    if (scope.objtree && scope.objtree.Rules && !scope.isloadonlymethod) {
                        data = data.concat(scope.objtree.Rules);
                    }
                    if (scope.isloadonlymethod) {
                        setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description", scope.selectionCallback);
                    }
                    else {
                        //setSingleLevelAutoComplete(scope.inputElement, data, scope);
                        setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, data, "ShortName", scope.onselectioncallback, scope);
                    }
                }
                else if (scope.objtree) {
                    if (scope.isloadonlymethod) {
                        setSingleLevelAutoComplete(scope.inputElement, data, scope, "ShortName", "Description", scope.selectionCallback);
                    }
                    else {
                        setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, scope.objtree.ChildProperties, "ShortName", scope.onselectioncallback, scope);
                    }
                }

                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onObjectFieldKeyDown = function (event) {
                scope.inputElement = $(event.target);

                if ((event.ctrlKey && event.keyCode === $.ui.keyCode.SPACE) || event.keyCode === $.ui.keyCode.PERIOD) {
                    if (!scope.objtree) {
                        if (scope.busobject) {
                            scope.LoadObjectTree();
                        }
                    }
                    else {
                        scope.objectFieldTextChanged(event);
                        if ($(scope.inputElement).data('ui-autocomplete') !== undefined) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        }
                    }

                    if (event.ctrlKey && event.keyCode === $.ui.keyCode.SPACE) {
                        event.preventDefault();
                    }
                }
            };

            var el = element[0];
            el.addEventListener('dragover', dragOverHandler, false);
            el.addEventListener('drop', dropHander, false);

            function dragOverHandler(e) {
                if (scope.allowdrop) {
                    if (e.preventDefault) {
                        e.preventDefault(); // Necessary. Allows us to drop.
                    }
                    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                }
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                return false;
            }

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
                            if (scope.parentmodel && scope.parentmodel.Elements) {
                                if (!scope.parentmodel.Elements.some(function (itm) { return itm.dictAttributes && itm.dictAttributes.sfwObjectField == data; })) {
                                    $rootScope.EditPropertyValue(scope.objectfield, scope, 'objectfield', data);
                                }
                            }
                            else {
                                $rootScope.EditPropertyValue(scope.objectfield, scope, 'objectfield', data);
                            }
                        } else {
                            alert("Collection cannot be added.");
                        }
                    });
                    dragDropData = null;
                    scope.onselectioncallback();
                }
                else {
                    return false;
                }
            }

            scope.validateEntityField = function () {
                if (scope.propertyname && scope.validate) {
                    var propertyTemp = scope.propertyname.split('.');
                    property = propertyTemp[propertyTemp.length - 1];
                    var errorMsg = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    if (property != "sfwEntityField") {
                        errorMsg = CONST.VALIDATION.INVALID_FIELD;
                    }
                    var list = [];

                    if (scope.objtree && scope.objtree.ChildProperties) {
                        list = $ValidationService.getListByPropertyName(scope.objtree.ChildProperties, "Name", false);
                    }
                    for (var i = 0; i < list.length; i++) {
                        list[i] = list[i].split(":")[0];
                    }
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_FIELD, undefined);
                }
            };
        }
    };
}]);

app.directive('customruledraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            if (el.localName == "li") {
                //alert(el.localName);
                //alert(scope.dragdata);
            }
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                if (scope.dragdata.Name == 'rule') {
                    //e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                    dragDropData = null;
                    dragDropData = scope.dragdata;
                }

            }
        },
    };
}]);

app.directive("droprule", ["$rootScope", function ($rootScope) {

    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            selectedsection: '='
            //  selectedRuleFromGroup: '='
        },
        link: function (scope, element, attributes) {

            var el = element[0];
            el.addEventListener("dragover", DragOver, false);
            el.addEventListener("drop", Drop, false);
            //el.addEventListener('dragleave', DragLeave, false);


            function DragOver(e) {
                e.dataTransfer.dropEffect = 'copy';

                if (scope.dropdata && scope.dropdata.Name && (scope.dropdata.Name == "group" || scope.dropdata.Name == "item" || scope.dropdata.Name == "harderror" || scope.dropdata.Name == "softerror")) {
                    if (e.preventDefault) {
                        e.preventDefault();
                    }

                }

                return false;

            }
            function Drop(e) {

                var entityScope = getCurrentFileScope();
                //alert('drop');
                e.preventDefault();

                var data = dragDropData;
                if (data != null) {
                    var objRule = entityScope.GetRuleByName(data.dictAttributes.ID);
                    if (scope.dropdata.Name == "group" && data.Name == 'rule') {
                        var objItem = {
                            Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: "All", sfwStatus: 'Active' }, Elements: []
                        };
                        if (scope.dropdata.dictAttributes) {
                            objItem.ParentID = scope.dropdata.dictAttributes.ID;
                        }
                        objItem.objRule = objRule;
                        var isFound = false;
                        angular.forEach(scope.dropdata.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                isFound = true;
                            }
                        });
                        scope.$apply(function () {

                            if (!isFound) {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.PushItem(objItem, scope.dropdata.Elements, "SelectGroupClick");
                                entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "Group");
                                //scope.dropdata.Elements.push(objItem);
                                entityScope.SelectGroupClick(objItem);
                                $rootScope.UndRedoBulkOp("End");
                            }

                            else {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                            }

                        });
                    }

                    else if (data.Name == 'rule' && scope.selectedsection == 'HardError') {
                        if (scope.dropdata.Name == "harderror") {

                            var objItem = {
                                Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: "All", sfwStatus: 'Active' }, Elements: [], Children: []
                            };
                            objItem.objRule = objRule;
                            var isFound = false;
                            angular.forEach(scope.dropdata.Elements, function (item) {
                                if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                    isFound = true;
                                }
                            });
                            scope.$apply(function () {

                                if (!isFound) {
                                    if ((data.dictAttributes.sfwMessageId == undefined || data.dictAttributes.sfwMessageId == "") && (data.dictAttributes.sfwMessage == undefined || data.dictAttributes.sfwMessage == "")) {
                                        alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                                        return;
                                    }
                                    else {
                                        $rootScope.UndRedoBulkOp("Start");
                                        $rootScope.PushItem(objItem, scope.dropdata.Elements, "SelectHardError");

                                        //scope.dropdata.Elements.push(objItem);
                                        entityScope.SelectHardError(objItem);
                                        entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "HardError");
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                }

                                else {
                                    alert('Cannot add the selected rule to validation. Same rule already exists as a Parent.');
                                }

                            });
                        }

                        else if (scope.dropdata.Name == "item") {

                            var objItem = {
                                Name: 'rule', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwStatus: 'Active' }, Elements: []
                            };
                            objItem.objRule = objRule;
                            var isFound = false;

                            if (scope.dropdata.dictAttributes.ID == data.dictAttributes.ID) {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                                if (e.stopPropagation) e.stopPropagation();
                                return;
                            }
                            if (scope.dropdata != undefined && scope.dropdata.Children != undefined && scope.dropdata.Children != null) {
                                angular.forEach(scope.dropdata.Children, function (item) {
                                    if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                        isFound = true;
                                    }
                                });
                            }
                            scope.$apply(function () {

                                if (!isFound) {
                                    if ((data.dictAttributes.sfwMessageId == undefined || data.dictAttributes.sfwMessageId == "") && (data.dictAttributes.sfwMessage == undefined || data.dictAttributes.sfwMessage == "")) {
                                        alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                                        return;
                                    }
                                    else if (scope.dropdata != undefined && scope.dropdata.Children != undefined && scope.dropdata != null && scope.dropdata.Children != null) {
                                        $rootScope.UndRedoBulkOp("Start");
                                        $rootScope.PushItem(objItem, scope.dropdata.Children, "SelectHardError");

                                        //scope.dropdata.Children.push(objItem);
                                        entityScope.SelectHardError(objItem);
                                        entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "HardErrorAsChild");
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                }

                                else {
                                    alert('Cannot add the selected rule to validation. Same rule already exists.');
                                }

                            });

                        }
                    }

                    else if (data.Name == 'rule' && scope.selectedsection == 'SoftError') {
                        if (scope.dropdata.Name == "softerror") {
                            //data.dictAttributes.sfwMode = 'All';
                            //data.dictAttributes.sfwStatus = 'Active';
                            var objItem = {
                                Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: "All", sfwStatus: 'Active' }, Elements: [], Children: []
                            };
                            objItem.objRule = objRule;
                            var isFound = false;
                            angular.forEach(scope.dropdata.Elements, function (item) {
                                if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                    isFound = true;
                                }
                            });
                            scope.$apply(function () {

                                if (!isFound) {
                                    if ((data.dictAttributes.sfwMessageId == undefined || data.dictAttributes.sfwMessageId == "") && (data.dictAttributes.sfwMessage == undefined || data.dictAttributes.sfwMessage == "")) {
                                        alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                                        return;
                                    }
                                    else {
                                        $rootScope.UndRedoBulkOp("Start");
                                        $rootScope.PushItem(objItem, scope.dropdata.Elements, "SelectSoftError");

                                        //scope.dropdata.Elements.push(objItem);
                                        entityScope.SelectSoftError(objItem);
                                        entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "SoftError");
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                }

                                else {
                                    alert('Cannot add the selected rule to validation. Same rule already exists as a Parent.');
                                }

                            });
                        }

                        else if (scope.dropdata.Name == "item") {

                            var objItem = {
                                Name: 'rule', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwStatus: 'Active' }, Elements: []
                            };
                            objItem.objRule = objRule;
                            var isFound = false;

                            if (scope.dropdata.dictAttributes.ID == data.dictAttributes.ID) {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                                if (e.stopPropagation) e.stopPropagation();
                                return;
                            }
                            angular.forEach(scope.dropdata.Children, function (item) {
                                if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                    isFound = true;
                                }
                            });
                            scope.$apply(function () {

                                if (!isFound) {
                                    if ((data.dictAttributes.sfwMessageId == undefined || data.dictAttributes.sfwMessageId == "") && (data.dictAttributes.sfwMessage == undefined || data.dictAttributes.sfwMessage == "")) {
                                        alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                                        return;
                                    }
                                    else {
                                        $rootScope.UndRedoBulkOp("Start");
                                        $rootScope.PushItem(objItem, scope.dropdata.Children, "SelectSoftError");

                                        //scope.dropdata.Children.push(objItem);
                                        entityScope.SelectSoftError(objItem);
                                        entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "SoftErrorAsChild");
                                        $rootScope.UndRedoBulkOp("End");
                                    }
                                }

                                else {
                                    alert('Cannot add the selected rule to validation. Same rule already exists.');
                                }

                            });
                        }

                    }

                    if (e.stopPropagation) e.stopPropagation();
                    dragDropData = null;
                }

                // entityScope = undefined;
            }

            function DragLeave(e) {

            }

        }
    };
}]);

app.directive("dropruleinchecklist", ["$rootScope", function ($rootScope) {
    return {
        restrict: "A",
        scope: {
            dropruledata: '=',

        },
        link: function (scope, element, attributes) {

            var el = element[0];
            el.addEventListener("dragover", DragOver, false);
            el.addEventListener("drop", Drop, false);
            //el.addEventListener('dragleave', DragLeave, false);

            function DragOver(e) {
                e.dataTransfer.dropEffect = 'copy';
                if (canDropRuleinChecklist(dragDropData, scope.dropruledata)) {
                    if (scope.dropruledata.Name == "checklist" || scope.dropruledata.Name == "initialload" || scope.dropruledata.Name == "validatedelete") {
                        if (e.preventDefault) {
                            e.preventDefault();
                        }
                    }
                }

                return false;

            }
            function Drop(e) {
                //alert('drop');
                var entityScope = getCurrentFileScope();
                e.preventDefault();
                var data = dragDropData;

                if (data != null) {
                    var objRule = entityScope.GetRuleByName(data.dictAttributes.ID);
                    if (scope.dropruledata.Name == "checklist") {
                        var objItem = {
                            Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: 'All', sfwStatus: 'Active' }, Elements: []
                        };
                        objItem.objRule = objRule;
                        var isFound = false;
                        angular.forEach(scope.dropruledata.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                isFound = true;
                            }
                        });
                        scope.$apply(function () {

                            if (!isFound) {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.PushItem(objItem, scope.dropruledata.Elements, "SelectCheckListClick");
                                entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "CheckList");
                                //scope.dropruledata.Elements.push(objItem);
                                entityScope.SelectedCheckList = objItem;
                                $rootScope.UndRedoBulkOp("End");
                            }

                            else {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                            }

                        });
                    }
                    else if (scope.dropruledata.Name == "initialload") {
                        var objItem = {
                            Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: 'All', sfwStatus: 'Active' }, Elements: []
                        };
                        objItem.objRule = objRule;
                        var isFound = false;
                        angular.forEach(scope.dropruledata.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                isFound = true;
                            }
                        });
                        scope.$apply(function () {

                            if (!isFound) {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.PushItem(objItem, scope.dropruledata.Elements, "SelectedInitialLoadClick");

                                //scope.dropruledata.Elements.push(objItem);
                                entityScope.SelectedInitialLoadClick(objItem);
                                entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "InitialLoad");
                                $rootScope.UndRedoBulkOp("End");
                            }

                            else {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                            }

                        });
                    }
                    else if (scope.dropruledata.Name == "validatedelete") {

                        var objItem = {
                            Name: 'item', value: '', dictAttributes: { ID: data.dictAttributes.ID, sfwMode: 'All', sfwStatus: 'Active' }, Elements: []
                        };
                        objItem.objRule = objRule;
                        var isFound = false;
                        angular.forEach(scope.dropruledata.Elements, function (item) {
                            if (item.dictAttributes.ID == objItem.dictAttributes.ID) {
                                isFound = true;
                            }
                        });
                        scope.$apply(function () {

                            if (!isFound) {
                                if ((data.dictAttributes.sfwMessageId == undefined || data.dictAttributes.sfwMessageId == "") && (data.dictAttributes.sfwMessage == undefined || data.dictAttributes.sfwMessage == "")) {
                                    alert('Cannot add the selected rule to validation. Rule must contain MessageID/Description.');
                                    return;
                                }
                                else {
                                    $rootScope.UndRedoBulkOp("Start");
                                    $rootScope.PushItem(objItem, scope.dropruledata.Elements, "SelectedValidateDeleteClick");

                                    //scope.dropruledata.Elements.push(objItem);
                                    entityScope.SelectedValidateDeleteClick(objItem);
                                    entityScope.setValidationRuleCheckboxValue(data.dictAttributes.ID, true, "ValidateDelete");
                                    $rootScope.UndRedoBulkOp("End");
                                }
                            }

                            else {
                                alert('Cannot add the selected rule to validation. Same rule already exists.');
                            }

                        });
                    }

                    dragDropData = null;
                }
            }

            function DragLeave(e) {

            }

        }
    };
}]);

app.directive("checklistitemcommontemplate", ["$compile", "$http", "$rootScope", "ngDialog", function ($compile, $http, $rootScope, ngDialog) {
    //var getTemplate = function () {
    //    var template = '<div><div class="col-1"><div class="row"><div class="initial-load-subhead"><span>{{model.dictAttributes.ID}}</span> </div><div class="row"><div class="load-caption"><label>Mode :</label>';
    //    template += '</div><select class="mode-select" ng-model="model.dictAttributes.sfwMode" undoredodirective model="model" ng-disabled="iscontroldisable"><option value=""></option><option value="All">All</option><option value="New">New</option><option value="Update">Update</option>';
    //    template += '</select></div><div class="row"><div class="load-caption"><label>Status :</label></div><select ng-model="model.dictAttributes.sfwStatus" undoredodirective model="model" class="mode-select" ng-disabled="iscontroldisable"><option value=""></option><option value="Active">Active</option><option value="InActive">InActive</option></select>';
    //    template += '</div><div class="row"><div class="load-caption"><label>CheckList Value :</label></div><input type="text" ondrop="return false;" ng-model="model.dictAttributes.sfwChecklistValue" undoredodirective model="model" ng-change="GetCheckListDescription()" class="initial-load-textbox" ng-disabled="iscontroldisable"/><button value="Search"  ng-click="OpenCheckListDialog()" ng-disabled="iscontroldisable">Search</button> ';
    //    template += '</div><div class="row"><div class="load-caption"><label>CheckList Description :</label></div><span>{{codeDescription}}</span></div></div>';

    //    return template;
    //}

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            codevalues: '=',
            iscontroldisable: '=',
            entitymodel: "="
        },
        link: function (scope, element, attrs) {
            //element.html(getTemplate())
            //$compile(element.contents())(scope)
            scope.GetCheckListDescription = function (codeDescription) {
                scope.codeDescription = codeDescription;
            };

            scope.SetCheckList = function () {
                if (scope.objCheckListValue != undefined) {
                    scope.model.dictAttributes.sfwChecklistValue = scope.objCheckListValue.CodeID;
                    scope.codeDescription = scope.objCheckListValue.Description;
                    ngDialog.close(openedDialogId);
                }
            };
            scope.OnSelectChecklist = function (obj) {
                scope.objCheckListValue = obj;
            };
            scope.CloseCheckList = function () {
                ngDialog.close(openedDialogId);
            };
            scope.GetCheckListDescription();
        },
        templateUrl: function (elem, attrs) {
            return attrs.templateUrl || 'Entity/views/CheckItemTemplate.html';
        }
    };
}]);

app.directive('colResizeableJquery', [function () {
    return {
        restrict: 'A',
        link: function (scope, elem) {
            elem.on('mouseenter', function (e) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                var selectedTable = "#" + elem[0].id;
                if (elem[0].id != undefined && elem[0].id != "") {
                    var selectedelement = selectedTable + " tr th";
                    //if ((selectedTable != "#tblAttribute") && (selectedTable != "#tblBusinessObjectMethod")) {
                    if ((selectedTable != "#tblBusinessObjectMethod")) {
                        scope.$evalAsync(function () {
                            $(selectedelement).resizable({
                                handles: 'e',
                                minWidth: 10,
                                grid: 50,
                                alsoResize: selectedTable,
                            });
                        });
                    }
                    else {
                        scope.$apply(function () {
                            $(selectedelement).resizable({
                                handles: 'e',
                                minWidth: 10,
                            });
                        });
                    }
                }
            });
        }
    };
}]);
app.directive('colResizeableExcelMatrics', [function () {
    return {
        restrict: 'A',
        link: function (scope, elem) {
            elem.on('mouseenter', function (e) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                var selectedTable = "#" + elem[0].id;
                var selectedelement = selectedTable + " tr td";

                scope.$apply(function () {
                    $(selectedelement).resizable({
                        handles: 'e',
                        minWidth: 10,
                        grid: 50,
                        alsoResize: selectedTable
                    });
                });
            });
        }
    };
}]);

app.directive("commonEntityTreeDirective", ["$compile", function ($compile) {

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
            parentTextProperty: "=",
            contextmenuproperty: '=',
            filtertext: "=",
            selecteditem: '=',
            validationruletype: '=',
            selectedsection: '=',
            validationerrorobject: '='
        },
        link: function (scope, element, attrs) {

            scope.onItemDoubleClick = function (event, element) {
                if (scope.onitemdblclickCallback) {
                    scope.onitemdblclickCallback(event, element);
                }
            };
            scope.onItemClick = function (element, event) {
                scope.selecteditem = element;
                if (scope.onitemclickCallback) {
                    scope.onitemclickCallback(element, event, scope.selectedsection);
                }
            };
            scope.toggleExpandCollapse = function (event, element) {
                element.IsExpanded = !element.IsExpanded;
                //setPropertyValue(element, scope.expandedProperty, !scope.getPropertyValue(element, scope.expandedProperty));
            };
            scope.getPropertyValue = getPropertyValue;
        },
        templateUrl: "Entity/views/TreeTemplateForEntity.html"
    };
}]);

app.directive("commonEntityTreeChildDirective", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Entity/views/TreeItemsTemplateForEntity.html',
    };
}]);

app.directive("newbusobjecttreedirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            busobjectname: '=',
            isshowproperties: '=',
            isshowmethods: '=',
            isshowrules: '=',
            selectedfield: '=',
            lstdisplaybusinessobject: '=',
            lstmultipleselectedfields: '=',
            lstcurrentbusinessobjectproperties: '=',
            ischeckboxvisible: '=',
            selectedfieldclick: "&"

        },
        link: function (scope, element, attr) {

            var unwatch = scope.$watch('busobjectname', function (newVal, oldVal) {
                scope.Init();
            });

            scope.AddClsBusinessObject = function (strBusobjectname, strdisplayName, strParentBusinessObject, strDataTypeOfField) {
                if (strDataTypeOfField == 'Collection' || strDataTypeOfField == 'List') {
                    scope.isParentFieldCollection = true;
                } else {
                    scope.isParentFieldCollection = false;
                }
                var objclsBusinessObject = {};
                var BusObject = "";
                BusObject = scope.checkObjectIsPresent(strBusobjectname);
                if (BusObject == "") {
                    var blnLoadMethod = !scope.isshowmethods ? false : true;
                    $.connection.hubEntityModel.server.loadObjectTree(strBusobjectname, "", blnLoadMethod).done(function (result) {
                        var data = result;
                        data = JSON.parse(data);
                        scope.$evalAsync(function () {
                            scope.ChildProperties = [];
                            scope.lstMethods = [];
                            $rootScope.IsLoading = false;
                            scope.lstChildPropertiesandMethods = [];
                            if (data.ChildProperties) {
                                scope.ChildProperties = data.ChildProperties;
                            }
                            if (data.lstMethods) {
                                scope.lstMethods = data.lstMethods;
                            }
                            if (scope.isshowproperties) {
                                scope.lstChildPropertiesandMethods = scope.ChildProperties;
                            }
                            if (scope.isshowmethods) {
                                scope.lstChildPropertiesandMethods = scope.lstChildPropertiesandMethods.concat(scope.lstMethods);
                            }
                            sortListBasedOnproperty(scope.lstChildPropertiesandMethods, "", "Name");

                            objclsBusinessObject = {
                                IsVisible: true, strBusinessobjectId: strBusobjectname, Properties: scope.lstChildPropertiesandMethods, SortedProperties: scope.lstChildPropertiesandMethods
                            };
                            scope.lstcurrentbusinessobjectproperties = objclsBusinessObject.Properties;
                            scope.lstBusinessObjects.push(objclsBusinessObject);
                            scope.AddClsDisplayBusinessObject(objclsBusinessObject, strdisplayName, strParentBusinessObject, strDataTypeOfField);
                            scope.setVisibilityForBusinessObjectOrDisplayBusinessObject(scope.lstBusinessObjects, strBusobjectname, "BusinessObject");
                        });
                    });
                } else {
                    objclsBusinessObject = BusObject;
                    scope.lstcurrentbusinessobjectproperties = objclsBusinessObject.Properties;
                    scope.AddClsDisplayBusinessObject(objclsBusinessObject, strdisplayName, strParentBusinessObject, strDataTypeOfField);
                    scope.setVisibilityForBusinessObjectOrDisplayBusinessObject(scope.lstBusinessObjects, strBusobjectname, "BusinessObject");
                }
            };

            scope.AddClsDisplayBusinessObject = function (clsBusinessobject, strdisplayName, strParentBusinessObject, strDataTypeOfField) {
                var objclsDisplayBusinessObject = {
                    IsVisible: true, strDisplayName: strdisplayName, strID: clsBusinessobject.strBusinessobjectId, ClsBusinessObject: clsBusinessobject, strParentID: strParentBusinessObject, DataType: strDataTypeOfField, isParentFieldCollection: scope.isParentFieldCollection
                };
                scope.lstDisplayBusinessObject.push(objclsDisplayBusinessObject);
                scope.lstdisplaybusinessobject = scope.lstDisplayBusinessObject;
                scope.setVisibilityForBusinessObjectOrDisplayBusinessObject(scope.lstDisplayBusinessObject, strdisplayName, "DisplayBusinessObject");
            };

            scope.setVisibilityForBusinessObjectOrDisplayBusinessObject = function (obj, strname, param) {
                for (var i = 0; i < obj.length; i++) {
                    if (param == "BusinessObject") {
                        if (obj[i].strBusinessobjectId == strname) {
                            obj[i].IsVisible = true;
                            scope.currentBusinessObject = obj[i];
                            obj[i].SortedProperties = obj[i].Properties;
                            scope.lstcurrentbusinessobjectproperties = obj[i].Properties;
                        } else {
                            obj[i].IsVisible = false;
                        }
                    } else {
                        if (obj[i].strDisplayName == strname) {
                            obj[i].IsVisible = true;
                            scope.objBusinessObjectFilter.DisplayPath = obj[i].strDisplayName;
                        } else {
                            obj[i].IsVisible = false;
                        }
                    }
                }
                scope.objBusinessObjectFilter.LimitCount = 35;
            };

            scope.Init = function () {
                scope.lstBusinessObjects = [];
                scope.lstDisplayBusinessObject = [];
                scope.objBusinessObjectFilter = {};
                scope.objBusinessObjectFilter.ObjectTreeFilter = "";
                scope.objBusinessObjectFilter.LimitCount = 35;
                scope.objBusinessObjectFilter.DisplayPath = "";
                scope.currentBusinessObject = '';
                var strdisplayName = "";
                var strParentBusinessObject = null;
                if (scope.busobjectname) {
                    scope.AddClsBusinessObject(scope.busobjectname, strdisplayName, strParentBusinessObject, "");
                }
            };

            scope.Navigatetoprevlist = function () {
                if (scope.lstDisplayBusinessObject.length > 1) {
                    ClearSelectedFieldList(scope.lstChildPropertiesandMethods);
                    scope.lstmultipleselectedfields = [];
                    scope.IsSelectAll = false;
                    scope.lstDisplayBusinessObject.splice(scope.lstDisplayBusinessObject.length - 1, 1);
                    scope.lstDisplayBusinessObject[scope.lstDisplayBusinessObject.length - 1].IsVisible = true;
                    scope.setVisibilityForBusinessObjectOrDisplayBusinessObject(scope.lstBusinessObjects, scope.lstDisplayBusinessObject[scope.lstDisplayBusinessObject.length - 1].strID, "BusinessObject");
                    scope.setVisibilityForBusinessObjectOrDisplayBusinessObject(scope.lstDisplayBusinessObject, scope.lstDisplayBusinessObject[scope.lstDisplayBusinessObject.length - 1].strDisplayName, "DisplayBusinessObject");
                }
            };

            scope.getPathFromDisplayedBusinessObjects = function () {
                var path = "";
                for (var i = 0; i < scope.lstDisplayBusinessObject.length; i++) {
                    if (scope.lstDisplayBusinessObject[i].IsVisible) {
                        path = scope.lstDisplayBusinessObject[i].strDisplayName;
                        break;
                    }
                }
                return path;
            };

            scope.isValidField = function (field) {
                var isvalid = false;
                if (field.DataType == "CDOCollection" || field.DataType == "List" || field.DataType == "CollctionType" || field.DataType == "BusObjectType" || field.DataType == "TableObjectType" || field.DataType == "OtherReferenceType" ||
                    (field.DataTypeName == "CDOCollection" || field.DataTypeName == "List" || field.DataTypeName == "CollctionType" || field.DataTypeName == "BusObjectType" || field.DataTypeName == "TableObjectType" || field.DataTypeName == "OtherReferenceType")) {
                    isvalid = true;
                }
                return isvalid;
            };

            scope.LoadNextLevelObjectField = function (field, event) {
                var path = GetFullItemPathFromBusObjectTree(field);
                var isValid = scope.isValidField(field);
                if (field.ItemType && isValid) {
                    var objectpath = scope.getPathFromDisplayedBusinessObjects();
                    var strDataTypeOfField = field.DataType;
                    var strdisplayName = "";
                    if (objectpath != "") {
                        strdisplayName = objectpath + "." + field.ShortName;
                    } else {
                        strdisplayName = field.ShortName;
                    }
                    var strParentBusinessObject = scope.getParentBusinessObject();
                    ClearSelectedFieldList(scope.lstChildPropertiesandMethods);
                    scope.lstmultipleselectedfields = [];
                    scope.IsSelectAll = false;
                    scope.AddClsBusinessObject(field.ItemType.Name, strdisplayName, strParentBusinessObject, strDataTypeOfField);
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.getParentBusinessObject = function () {
                var objectID = "";
                for (var i = 0; i < scope.lstBusinessObjects.length; i++) {
                    if (scope.lstBusinessObjects[i].IsVisible) {
                        objectID = scope.lstBusinessObjects[i].strBusinessobjectId;
                        break;
                    }
                }
                return objectID;
            };

            scope.checkObjectIsPresent = function (strObjectID) {
                var Object = "";
                for (var i = 0; i < scope.lstBusinessObjects.length; i++) {
                    if (scope.lstBusinessObjects[i].strBusinessobjectId == strObjectID) {
                        Object = scope.lstBusinessObjects[i];
                        break;
                    }
                }

                return Object;
            };

            scope.sortList = function (clsObject, strText) {
                if (strText != "") {
                    var lstExactMatchCaseSensitive = [];
                    var lstExactMatchCaseInSensitive = [];
                    var lstCaseSenesitive = [];
                    var lstCaseInsensitive = [];
                    var lstContainsCaseSensitive = [];
                    var lstContainsCaseInSensitive = [];
                    var attributeName = "Name";

                    for (var i = 0; i < clsObject.Properties.length; i++) {
                        if (clsObject.Properties[i][attributeName] == strText) {
                            lstExactMatchCaseSensitive.push(clsObject.Properties[i]);
                        } else if (clsObject.Properties[i][attributeName].toLowerCase() == strText.toLowerCase()) {
                            lstExactMatchCaseInSensitive.push(clsObject.Properties[i]);
                        } else if (clsObject.Properties[i][attributeName].indexOf(strText) == 0) {
                            lstCaseSenesitive.push(clsObject.Properties[i]);
                        } else if (clsObject.Properties[i][attributeName].toLowerCase().indexOf(strText.toLowerCase()) == 0) {
                            lstCaseInsensitive.push(clsObject.Properties[i]);
                        } else if (clsObject.Properties[i][attributeName].contains(strText)) {
                            lstContainsCaseSensitive.push(clsObject.Properties[i]);
                        } else if (clsObject.Properties[i][attributeName].toLowerCase().contains(strText.toLowerCase())) {
                            lstContainsCaseInSensitive.push(clsObject.Properties[i]);
                        }
                    }
                    var lst = lstExactMatchCaseSensitive.concat(lstExactMatchCaseInSensitive).concat(lstCaseSenesitive).concat(lstCaseInsensitive).concat(lstContainsCaseSensitive).concat(lstContainsCaseInSensitive);
                    clsObject.SortedProperties = lst;
                } else {
                    clsObject.SortedProperties = clsObject.Properties;
                }
                scope.objBusinessObjectFilter.LimitCount = 35;
            };

            scope.lstmultipleselectedfields = [];
            scope.IsSelectAll = false;
            scope.RemoveOrIsFieldPresentFromMultipleSelectedField = function (objField, param) {
                var isFound = false;
                for (var i = 0; i < scope.lstmultipleselectedfields.length; i++) {
                    if (scope.lstmultipleselectedfields[i].Name == objField.Name) {
                        if (param == "Delete") {
                            scope.lstmultipleselectedfields.splice(i, 1);
                        } else if (param == "bool") {
                            isFound = true;
                        }
                        break;
                    }
                }

                return isFound;
            };
            scope.selectField = function (field, event) {
                if (scope.selectedfield && !event.ctrlKey) {
                    scope.selectedfield.IsRecordSelected = false;
                    if (!scope.ischeckboxvisible) {
                        scope.selectedfield.IsSelected = "False";
                    }
                } else {
                    if (scope.selectedfield) {
                        var isFound = scope.RemoveOrIsFieldPresentFromMultipleSelectedField(scope.selectedfield, "bool");
                        if (!isFound) {
                            scope.lstmultipleselectedfields.push(scope.selectedfield);
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
                            scope.RemoveOrIsFieldPresentFromMultipleSelectedField(field, "Delete");
                        }
                        else {
                            field.IsRecordSelected = true;
                            field.IsSelected = "True";
                            scope.lstmultipleselectedfields.push(field);
                        }
                    }
                    else {
                        for (var i = 0; i < scope.lstmultipleselectedfields.length; i++) {
                            scope.lstmultipleselectedfields[i].IsRecordSelected = false;
                            scope.lstmultipleselectedfields[i].IsSelected = "False";
                        }
                        scope.lstmultipleselectedfields = [];
                        field.IsRecordSelected = true;
                        field.IsSelected = "True";
                    }
                }
                if (scope.selectedfieldclick) {
                    scope.selectedfieldclick({ selectedfield: scope.selectedfield });
                }
            };

            scope.AddIntoSelectedList = function (field) {
                if (scope.ischeckboxvisible) {
                    if (scope.lstmultipleselectedfields) {
                        if (field.IsSelected == "True") {
                            scope.lstmultipleselectedfields.push(field);
                        } else {
                            for (var i = 0; i < scope.lstmultipleselectedfields.length; i++) {
                                if (scope.lstmultipleselectedfields[i].ShortName == field.ShortName) {
                                    scope.lstmultipleselectedfields.splice(i, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            };
            scope.OnChangeSelectAll = function (bool) {
                scope.lstmultipleselectedfields = [];
                for (var i = 0; i < scope.lstcurrentbusinessobjectproperties.length; i++) {
                    if (bool) {
                        scope.lstcurrentbusinessobjectproperties[i].IsSelected = 'True';
                        scope.lstmultipleselectedfields.push(scope.lstcurrentbusinessobjectproperties[i]);
                    } else {
                        scope.lstcurrentbusinessobjectproperties[i].IsSelected = 'False';
                    }
                }
            };
        },
        templateUrl: 'Entity/views/ObjTree/BusObjectTreeTemplate.html',
    };
}]);

app.directive("busobjecttreechildtemplate", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Entity/views/ObjTree/BusObjectTreeChildWrapper.html',
    };
}]);

app.directive("entityquerytemplate", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Entity/views/EntityQueryTree.html',
    };
}]);

app.directive('validationrulestreeview', ['$compile', function ($compile) {
    var getTreeViewForRules = function (item, element) {
        var template = ["<div>",
            "<ul>",
            "<li ng-repeat='element in lstrules.Elements |filter:{dictAttributes:{[filterproperty]:filtertext}} track by $index' aria-selected='false' customruledraggable dragdata='element' ng-class=\"classforchild?'marg-left-20':''\">",
            "<div context-menu='menuoptionforrules' ng-class=\"element==selecteditem?'selected color-light2':''\" tabindex='{{$index}}' key-up-down-with-element callbackfn='onItemClick' objproperty=\"'element'\" parentelem=\"'li'\">",
            "<i style='float:left;margin-top:6px' ng-if='element.Elements.length>0' ng-click='toggleExpandCollapse($event,element)' ng-class=\"element.IsExpanded?'fa fa-minus':'fa fa-plus'\"></i>",
            "<span class='info-tooltip dtc-span' style='position:relative;color:red!important;' ng-if='element.errors.empty_id || element.errors.inValid_id || element.errors.duplicate_id' ng-attr-title='{{element.errors.empty_id || element.errors.inValid_id || element.errors.duplicate_id}}'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
            "<span ng-dblclick='onItemDoubleClik($event,element)' ng-click='onItemClick(element)' ng-class='{bckgGrey: element.isAdvanceSearched, bckgGreen: (selecteditem == element && element.isAdvanceSearched)}' ng-bind='element.dictAttributes.ID' droprule selectedsection=\"'Item'\" dropdata='element' ondragenter='return false;' title='{{element.dictAttributes.ID}}'></span>",
            "</div>",
            "<span validationrulestreeview lstrules='element' filtertext='filtertext' filterproperty='filterproperty' selecteditem='selecteditem' onitemclickcallback='onitemclickcallback(selecteditem,selectedsection)' ng-if='element.Elements.length > 0' filtertext='filtertext' classforchild='true' menuoptionforrules='menuoptionforrules' ng-show='element.IsExpanded'></span>",
            "</li>",
            "</ul>",
            "</div>"].join(' ');
        return template;
    };
    return {
        restrict: "EA",
        scope: {
            lstrules: '=',
            menuoptionforrules: "=",
            selecteditem: "=",
            filtertext: "=",
            classforchild: "=",
            findParentAndChildObject: '=',
            filterproperty: "="
        },
        link: function (scope, element, attributes) {
            scope.onItemDoubleClick = function (event, element) {
                if (scope.onitemdblclickCallback) {
                    scope.onitemdblclickCallback(event, element);
                }
            };
            scope.onItemClick = function (element) {
                if (element) {
                    var controllerScope = getCurrentFileScope();
                    if (controllerScope) {
                        controllerScope.SelectedRule = element;
                        if (controllerScope.selectValidationRuleInAllTypes) {
                            controllerScope.selectValidationRuleInAllTypes(element, true, "ValidationRule");
                        }
                        //else {
                        //    controllerScope = getScopeByFileName("EntityValidationRulesController");
                        //    if (controllerScope) {
                        //        controllerScope.SelectedRule = element;
                        //        if (controllerScope.selectedRules) {
                        //            controllerScope.selectedRules(element, event);
                        //        }
                        //    }
                        //}
                    }
                }
            };
            scope.toggleExpandCollapse = function (event, element) {
                element.IsExpanded = !element.IsExpanded;
            };
            scope.$watch("lstrules", function (newVal, OldVal) {
                if (scope.lstrules && angular.isArray(scope.lstrules.Elements)) {
                    element.html(getTreeViewForRules(scope.lstrules, element));
                    $compile(element.contents())(scope);
                }
            });
        }
    };
}]);

app.directive("duplicateIdObj", [function () {
    return {
        restrict: 'A',
        scope: {
            currObj: '=',
            errorMessageClickCallback: '&callbackFn'
        },
        link: function (scope, elem, attr) {

            scope.findNextObject = function (event, obj) {
                event.stopImmediatePropagation();
                event.stopPropagation();
                $('.duplicate-id-tooltip').hide();
                scope.errorMessageClickCallback({ paramObj: obj });
            };
        },
        template: function (elem, attr) {
            var htmlText = ['<div ng-if="currObj.otherDuplicateObj.length >0" class="duplicate-id-tooltip">',
                '<ul class="list-unstyled" margin-bottom: 0px!important;>',
                '<li ng-repeat="obj in currObj.otherDuplicateObj track by $index">',
                '<span ng-click="findNextObject($event,obj);"> Duplicate ID ( {{obj.dictAttributes.ID}} ) present in - {{obj.Name}} </span>',
                '</li>',
                '</ul>',
                '</div>'].join(' ');
            return htmlText;
        }
    };
}]);

app.directive("entityattributeintellisense", ['$rootScope', '$filter', '$Entityintellisenseservice', 'CONSTANTS', '$ValidationService', function ($rootScope, $filter, $Entityintellisenseservice, CONST, $ValidationService) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            type: "=",
            datatype: "=",
            entityid: "=",
            modebinding: "=",
            onchangecallback: '&',
            ontextchangecallback: '&',
            isdisabled: "=",
            onblurcallback: '&',
            validate: '=',
            errorprop: '=',
            isempty: '=',
            setcolumndatatype: '=',
            isjson: '=',
            isloadinheritedobjects: '=',
            propertyname: '='
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input ng-disabled='isdisabled' type='text' ng-change='onchange();validateEntityField()' ondrop='onEntityFieldDropInDirective(event)' ng-blur='onBlur()' ondragover='onEntityFieldDragOver(event)' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' ng-keyup='onkeyup($event)' ng-keydown='onkeydown($event)' /><span ng-disabled='isdisabled' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.prevEntityID = undefined;
            scope.onkeyup = function (event) {
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (scope.inputElement) {
                        var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                        if (arrText.length > 1 || arrText.length == 0) scope.setAutocomplete(scope.inputElement, event);
                    }
                }
                scope.ontextchangecallback();
            };
            scope.onkeydown = function (event) {
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (!scope.inputElement || scope.prevEntityID != scope.entityid) {
                        scope.inputElement = $(event.target);
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.prevEntityID = scope.entityid;
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        event.preventDefault();
                    }
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    // if user goes back from second level to first level intellisense
                    if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense($(event.target), [], "", scope.onchangecallback);
                }
            };
            scope.getIntellisenseData = function (entityID) {
                var filteredData = [];
                filteredData = $Entityintellisenseservice.GetIntellisenseData(entityID, scope.type, scope.datatype, scope.isloadinheritedobjects, true, false, false, false, false);
                if (scope.isjson) {
                    var lstJsonFields = [];
                    for (var i = 0; i < filteredData.length; i++) {
                        if (filteredData[i].Value && filteredData[i].Value.toLowerCase().startsWith("json")) {
                            lstJsonFields.push(filteredData[i]);
                        }
                    }
                    return lstJsonFields;
                }
                return filteredData;
            };

            scope.setAutocomplete = function (input, event) {
                // this function will only be called when you want to change the data for the intellisense i.e switch between single level to multilevel and vice-versa
                scope.isMultilevelActive = false;
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    var arrText = getSplitArray(input.val(), input[0].selectionStart);
                    var data = [];
                    data = scope.getIntellisenseData(scope.entityid);
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    data = scope.getIntellisenseData(item[0].Entity);
                                }
                                else {
                                    data = [];
                                }
                            }
                        }
                    }
                    //if (scope.isObject && data && data.length > 0) {
                    //    data = $filter('filter')(data, { DataType: "Object" });
                    //}
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, data, "", scope.onchangecallback);
                } else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, [], "", scope.onchangecallback);
                }
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }

                if (scope.entityid && scope.entityid.trim().length > 0) {

                    if (scope.prevEntityID != scope.entityid) {
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.inputElement.focus();
                    scope.prevEntityID = scope.entityid;
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    if (arrText.length > 1) scope.setAutocomplete(scope.inputElement, event);
                    // switch back from multilevel to single level  
                    else if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    }
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, [], "", scope.onchangecallback);
                }
            };
            scope.validateEntityField = function () {
                if (scope.propertyname && scope.validate) {
                    var propertyTemp = scope.propertyname.split('.');
                    property = propertyTemp[propertyTemp.length - 1];
                    var errorMsg = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    if (property != "sfwEntityField") {
                        errorMsg = CONST.VALIDATION.INVALID_FIELD;
                    }
                    var list = [];

                    if (scope.isjson) {
                        list = $ValidationService.getListByPropertyName(scope.getIntellisenseData(scope.entityid), "ID", false);
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_FIELD, undefined);
                    } else {
                        $ValidationService.checkValidListValueForMultilevel(list, scope.model, $(scope.inputElement).val(), scope.entityid, property, property, errorMsg, undefined, scope.isempty);
                    }

                }
            };
            scope.onBlur = function () {
                if (scope.onblurcallback) scope.onblurcallback();
                // scope.validateEntityField();
            };
            scope.onchange = function () {
                if (scope.onchangecallback) scope.onchangecallback();
                scope.validateEntityField();
            };
        }
    };
}]);

app.directive("xmlmethodintellisensetemplate", ["$compile", "$rootScope", "$Entityintellisenseservice", "$filter", "$ValidationService", "CONSTANTS", "$GetEntityFieldObjectService", function ($compile, $rootScope, $Entityintellisenseservice, $filter, $ValidationService, CONST, $GetEntityFieldObjectService) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            objectfield: "=",
            propertyname: "=",
            entityid: '=',
            type: '=',
            onchangecallback: '&',
            isxmlmethod: '=',
            iscorrespondancexmlmethod: '@',
            onmethodorrulechange: '&',
            isdelete: '=',
            lstbusbasemethods: '=',
            onpropertychanged: '&'
        },
        template: "<div class='input-group'><span class='info-tooltip' ng-if='model.errors.invalid_method_name && !isxmlmethod' ng-attr-title='{{model.errors.invalid_method_name}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input ng-class=\"insidegrid?'form-control form-filter input-sm':'form-control input-sm'\" type='text' ng-disabled='isdisabled' title='{{objectfield}}' ng-model='objectfield' ng-keyup='onkeyup($event);validateMethodName()' ng-keydown='onkeydown($event)' ng-change='onchange();validateMethodName()' ng-blur='onBlur()' undoredodirective model='model' propertyname='propertyname'/><span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div>",
        link: function (scope, element, attrs) {
            scope.prevEntityID = undefined;
            scope.selectedobject = "";
            scope.onkeyup = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (scope.entityid && scope.entityid.trim().length > 0) {
                        if (scope.inputElement) {
                            var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                            if (arrText.length > 1 || arrText.length == 0) scope.setAutocomplete(scope.inputElement, event);
                        }
                    }
                }
            };
            scope.onkeydown = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    scope.selectedobject = "";
                    if (scope.entityid && scope.entityid.trim().length > 0) {
                        if (!scope.inputElement || scope.prevEntityID != scope.entityid || !$(event.target).val()) {
                            scope.inputElement = $(event.target);
                            scope.setAutocomplete(scope.inputElement, event);
                        }
                        scope.prevEntityID = scope.entityid;
                        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                        var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                        // if user goes back from second level to first level intellisense
                        if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                    }
                    else {
                        setMultilevelAutoCompleteForObjectTreeIntellisense($(event.target), [], "", scope);
                    }
                }
            };
            scope.getIntellisenseData = function (entityID) {
                var filteredData = [];
                if (scope.isdelete) {
                    var lstData = $Entityintellisenseservice.GetIntellisenseData(entityID, scope.type, scope.datatype, true, true, true, false, false, false);
                    var lsttempData = [];
                    if (lstData) {
                        for (var i = 0; i < lstData.length; i++) {
                            if (lstData[i].Parameters && lstData[i].Type == "Method" && lstData[i].Parameters.length == 0) {
                                if (lstData[i].ID) {
                                    lsttempData.push(lstData[i]);
                                }
                            }
                            if (lstData[i].Type != "Method") {
                                if (lstData[i].ID) {
                                    lsttempData.push(lstData[i]);
                                }
                            }
                        }
                    }
                    filteredData = lsttempData;
                }
                else if (scope.iscorrespondancexmlmethod) {
                    filteredData = $Entityintellisenseservice.GetIntellisenseData(entityID, scope.type, scope.datatype, true, false, false, false, false, true);
                }
                else {
                    filteredData = $Entityintellisenseservice.GetIntellisenseData(entityID, scope.type, scope.datatype, true, true, true, true, false, false);
                }
                return filteredData;
            };

            scope.setAutocomplete = function (input, event) {
                // this function will only be called when you want to change the data for the intellisense i.e switch between single level to multilevel and vice-versa
                scope.isMultilevelActive = false;
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    var arrText = getSplitArray(input.val(), input[0].selectionStart);
                    scope.data = [];
                    scope.data = scope.getIntellisenseData(scope.entityid);
                    if (scope.lstbusbasemethods && scope.lstbusbasemethods.length) {
                        scope.data = scope.data.concat(scope.lstbusbasemethods);
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length; index++) {
                            var item = scope.data.filter(function (x) { return x.ID && x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    scope.data = scope.getIntellisenseData(item[0].Entity);
                                }
                                else {
                                    scope.data = [];
                                }
                            }
                        }
                    }
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, scope.data, "", null, scope);
                } else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, [], "", null, scope);
                }
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }

                if (scope.entityid && scope.entityid.trim().length > 0) {

                    if (scope.prevEntityID != scope.entityid || !$(scope.inputElement).val()) {
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.inputElement.focus();
                    scope.prevEntityID = scope.entityid;
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    if (arrText.length > 1) scope.setAutocomplete(scope.inputElement, event);
                    // switch back from multilevel to single level  
                    else if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    }
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(scope.inputElement, [], "", scope.onchangecallback);
                }
            };
            scope.onBlur = function () {
                if (scope.onchangecallback) scope.onchangecallback();
                // scope.validateEntityField();
            };

            scope.getObjectFromField = function (entityID, entityField) {
                var objField = "";
                objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityID, entityField);
                if (!objField && scope.data && scope.data.length > 0) {
                    var lstText = entityField.split(".");
                    var objItem = $filter('filter')(scope.data, { ID: lstText[lstText.length - 1] }, true);
                    if (objItem && objItem.length > 0) {
                        objField = objItem[0];
                    }
                }
                return objField;
            };
            scope.onchange = function () {
                if (scope.isxmlmethod) {
                    scope.model.dictAttributes.ID = scope.objectfield;
                    if (scope.objectfield) {
                        if (scope.objectfield && scope.entityid) {
                            var obj = scope.getObjectFromField(scope.entityid, scope.objectfield);
                            if (obj) {
                                var Selectedobj = "";
                                if (scope.selectedobject) {
                                    Selectedobj = scope.selectedobject;
                                } else {
                                    Selectedobj = obj;
                                }
                                if (Selectedobj.Type) {
                                    scope.model.dictAttributes.sfwItemType = Selectedobj.Type;
                                }

                                if (Selectedobj.Type == "Method" || Selectedobj.Type == "Rule") {
                                    scope.model.dictAttributes.sfwDataType = Selectedobj.ReturnType;
                                    if (scope.onmethodorrulechange) {
                                        scope.onmethodorrulechange({ objobjectmethodorrule: Selectedobj, objitem: scope.model });
                                    }
                                }
                                if (Selectedobj.Type == "Property" || Selectedobj.Type == "Column") {
                                    if (scope.model.dictAttributes.ID) {
                                        var lstParameter = scope.model.dictAttributes.ID.split(".");
                                        var parameter = lstParameter.join("_");
                                        scope.model.dictAttributes.sfwParameter = parameter;
                                        scope.model.dictAttributes.sfwItemType = "Property";
                                        scope.model.dictAttributes.sfwDataType = Selectedobj.DataType;
                                    } else {
                                        scope.model.dictAttributes.sfwItemType = "";
                                        scope.model.dictAttributes.sfwParameter = "";
                                        scope.model.dictAttributes.sfwDataType = "";
                                    }
                                    if (scope.onpropertychanged) {
                                        scope.onpropertychanged({ objitem: scope.model });
                                    }
                                }
                            } else {
                                scope.clearFields();
                            }
                        } else {
                            scope.clearFields();
                        }
                    } else {
                        scope.clearFields();
                    }
                }
            };



            scope.clearFields = function () {
                scope.model.dictAttributes.sfwParameter = "";
                scope.model.dictAttributes.sfwItemType = "";
                scope.model.dictAttributes.sfwLoadType = "";
                scope.model.dictAttributes.sfwLoadSource = "";
                scope.model.dictAttributes.sfwDataType = "";
            };
            scope.validateMethodName = function () {
                if (scope.propertyname && scope.model) {
                    var property = scope.propertyname.split('.')[1];
                    var list = [];
                    if (scope.lstbusbasemethods && scope.lstbusbasemethods.length > 0) {
                        list = list.concat(scope.lstbusbasemethods);
                    }
                    $ValidationService.checkValidListValueForXMLMethod(list, scope.model, $(scope.inputElement).val(), scope.entityid, property, "invalid_method_name", CONST.VALIDATION.INVALID_NAME, undefined, scope.type, scope.datatype, scope.isdelete, scope.iscorrespondancexmlmethod);
                }
            };
        }
    };
}]);

app.directive("addEditAttributeConstraint", ["$compile", "$rootScope", "$Entityintellisenseservice", "$EntityIntellisenseFactory", function ($compile, $rootScope, $Entityintellisenseservice, $EntityIntellisenseFactory) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            autoSave: "=",
            attributeName: "=",
            entityModel: "=",
            entityId: "=",
        },
        templateUrl: "Entity/views/Constraint/AddFieldConstraint.html",
        link: function (scope, element, attrs) {
            scope.entityIntellisenseService = $Entityintellisenseservice;
            scope.entityIntellisenseFactory = $EntityIntellisenseFactory;
            scope.ctrl = new AddEditConstraintsController(scope.autoSave, scope.attributeName, scope.entityModel, scope.entityId, scope);
            scope.onCancelClick = function () {
                if (scope.$parent.dialog && scope.$parent.dialog.close) {
                    scope.$parent.dialog.close();
                }
            }
        }
    };
}]);

function AddEditConstraintsController(autoSave, attributeName, entityModel, entityId, scope) {
    var ctrl = this;
    ctrl.autoSave = autoSave;
    ctrl.attributeName = attributeName;
    ctrl.entityModel = entityModel;
    ctrl.entityId = entityId;
    ctrl.applicationPortals = [];

    $.connection.hubMain.server.populateMessageList().done(function (lstMessages) {
        ctrl.lstMessages = lstMessages;
    });
    $.connection.hubMain.server.getPortalsModel().done(function (portalsModel) {
        if (portalsModel && portalsModel.Elements.length) {
            for (var index = 0, len = portalsModel.Elements.length; index < len; index++) {
                var portal = portalsModel.Elements[index];
                if (portal && portal.dictAttributes && portal.dictAttributes.ID && portal.dictAttributes.sfwPortalName &&
                    !ctrl.applicationPortals.some(function (item) { return item.id === portal.dictAttributes.ID; })) {
                    ctrl.applicationPortals.push({ id: portal.dictAttributes.ID, name: portal.dictAttributes.sfwPortalName });
                }
            }
        }
    });
    ctrl.getErrorObject = function (obj) {
        if (!obj.errors) {
            obj.errors = {};
        }
        return obj.errors;
    }
    ctrl.initilize = function () {
        ctrl.isNewMode = true;
        ctrl.attributeConstraintModel = null;
        ctrl.lstConstraints = [];
        ctrl.errorMessage = null;
        ctrl.operators = ['Equal', 'NotEqual', 'LessThan', 'LessThanEqual', 'GreaterThan', 'GreaterThanEqual'];
        //How to identify that attribute name needs to be disabled.

        if (ctrl.entityModel) {
            if (!ctrl.entityId) {
                ctrl.entityId = ctrl.entityModel.dictAttributes.ID;
            }
            ctrl.setAttributeConstraintModel();
        }
        else if (ctrl.entityId) {
            //Get entity file info and model by entity id.
            $.connection.hubEntityModel.server.getEntityModelById(ctrl.entityId).done(function (data) {
                if (data) {
                    ctrl.entityModel = data;
                    ctrl.setAttributeConstraintModel();
                }
                else {
                    ctrl.errorMessage = String.format("entity '{0}' not found.", ctrl.entityId);
                }
            });
        }
        else {
            ctrl.errorMessage = "entity id is not specified.";
        }
    };
    ctrl.setAttributeConstraintModel = function () {

        if (ctrl.attributeName) {
            //Find for constraint model associated with the attribute name.
            ctrl.entityConstraintModel = ctrl.entityModel.Elements.filter(function (x) { return x.Name === "constraint"; })[0];
            if (ctrl.entityConstraintModel) {
                ctrl.attributeConstraintModel = ctrl.entityConstraintModel.Elements.filter(function (itm) { return itm.Name === "item" && itm.dictAttributes.sfwFieldName === ctrl.attributeName; })[0];
            }

            //If found, that means edit mode.
            //Else new mode.
            if (ctrl.attributeConstraintModel) {
                ctrl.isNewMode = false;
            }
            else {
                ctrl.isNewMode = true;
                ctrl.attributeConstraintModel = new BaseModel("item");
            }
        }
        else {
            //ctrl will always be new mode.
            ctrl.isNewMode = true;
            ctrl.attributeConstraintModel = new BaseModel("item");
        }
        if (ctrl.attributeConstraintModel && !ctrl.isNewMode) {
            //Update root properties.
            ctrl.datatype = ctrl.attributeConstraintModel.dictAttributes.sfwDataType;
            ctrl.displayName = ctrl.attributeConstraintModel.dictAttributes.sfwDisplayName;

            // take portal message for require, min, max, length and compare
            ctrl.attributePortalMessages = ctrl.attributeConstraintModel.Elements.filter(function (itm) { return itm.Name === "portalmessages"; })[0];

            //Add required constraint
            if (ctrl.attributeConstraintModel.dictAttributes.sfwRequired && ctrl.attributeConstraintModel.dictAttributes.sfwRequired.toLowerCase() === "true") {
                var requiredConstraint = new BaseConstraint("Required", ctrl.attributeConstraintModel.dictAttributes.sfwReqMessageId);
                // update portal messages.
                if (ctrl.attributePortalMessages) {
                    var portals = ctrl.attributePortalMessages.Elements.filter(function (itm) { return itm.Name === "portal" && itm.dictAttributes.sfwReqMessageId; });
                    for (var i = 0, len = portals.length; i < len; i++) {
                        var portalMessage = new PortalMessage(portals[i].dictAttributes.sfwPortalName, portals[i].dictAttributes.sfwReqMessageId);
                        requiredConstraint.portalMessages.push(portalMessage);
                    }
                }

                ctrl.lstConstraints.push(requiredConstraint);
            }

            //Add min constraint
            if (ctrl.attributeConstraintModel.dictAttributes.sfwMinValue) {
                var minConstraint = new ValueConstraint("Min", ctrl.attributeConstraintModel.dictAttributes.sfwMinMessageId, ctrl.attributeConstraintModel.dictAttributes.sfwMinValue);

                // update portal messages.
                if (ctrl.attributePortalMessages) {
                    var portals = ctrl.attributePortalMessages.Elements.filter(function (itm) { return itm.Name === "portal" && itm.dictAttributes.sfwMinMessageId; });
                    for (var i = 0, len = portals.length; i < len; i++) {
                        var portalMessage = new PortalMessage(portals[i].dictAttributes.sfwPortalName, portals[i].dictAttributes.sfwMinMessageId);
                        minConstraint.portalMessages.push(portalMessage);
                    }
                }

                ctrl.lstConstraints.push(minConstraint);
            }

            //Add max constraint
            if (ctrl.attributeConstraintModel.dictAttributes.sfwMaxValue) {
                var maxConstraint = new ValueConstraint("Max", ctrl.attributeConstraintModel.dictAttributes.sfwMaxMessageId, ctrl.attributeConstraintModel.dictAttributes.sfwMaxValue);

                // update portal messages.
                if (ctrl.attributePortalMessages) {
                    var portals = ctrl.attributePortalMessages.Elements.filter(function (itm) { return itm.Name === "portal" && itm.dictAttributes.sfwMaxMessageId; });
                    for (var i = 0, len = portals.lenght; i < len; i++) {
                        var portalMessage = new PortalMessage(portals[i].sfwPortalName, portals[i].sfwMaxMessageId);
                        maxConstraint.portalMessages.push(portalMessage);
                    }
                }

                ctrl.lstConstraints.push(maxConstraint);
            }

            //Add length constraint
            if (ctrl.attributeConstraintModel.dictAttributes.sfwLength) {
                var lengthConstraint = new ValueConstraint("Length", ctrl.attributeConstraintModel.dictAttributes.sfwLengthMessageId, ctrl.attributeConstraintModel.dictAttributes.sfwLength);
                ctrl.lengthConstraintExists = true;

                // update portal messages.
                if (ctrl.attributePortalMessages) {
                    var portals = ctrl.attributePortalMessages.Elements.filter(function (itm) { return itm.Name === "portal" && itm.dictAttributes.sfwLengthMessageId; });
                    for (var i = 0, len = portals.length; i < len; i++) {
                        var portalMessage = new PortalMessage(portals[i].dictAttributes.sfwPortalName, portals[i].dictAttributes.sfwLengthMessageId);
                        lengthConstraint.portalMessages.push(portalMessage);
                    }
                }

                ctrl.lstConstraints.push(lengthConstraint);
            }

            //Add compare constraint
            if (ctrl.attributeConstraintModel.dictAttributes.sfwRelatedField) {
                var compareConstraint = new CompareConstraint("Compare", ctrl.attributeConstraintModel.dictAttributes.sfwCompMessageId, ctrl.attributeConstraintModel.dictAttributes.sfwRelatedField, ctrl.attributeConstraintModel.dictAttributes.sfwOperator);
                compareConstraint.relatedDisplayName = ctrl.attributeConstraintModel.dictAttributes.sfwRelDisplayName;
                // update portal messages.
                if (ctrl.attributePortalMessages) {
                    var portals = ctrl.attributePortalMessages.Elements.filter(function (itm) { return itm.Name === "portal" && itm.dictAttributes.sfwCompMessageId; });
                    for (var i = 0, len = portals.length; i < len; i++) {
                        var portalMessage = new PortalMessage(portals[i].dictAttributes.sfwPortalName, portals[i].dictAttributes.sfwCompMessageId);
                        compareConstraint.portalMessages.push(portalMessage);
                    }
                }

                ctrl.lstConstraints.push(compareConstraint);
            }

            //Add query/rule constraints
            ctrl.ruleQueryConstraints = ctrl.attributeConstraintModel.Elements.filter(function (itm) { return itm.Name === "rulequeryconstraints"; })[0];
            if (ctrl.ruleQueryConstraints) {
                for (var index = 0, len = ctrl.ruleQueryConstraints.Elements.length; index < len; index++) {
                    var value = ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwCompValue;
                    var isConstant = false;
                    if (value && value.indexOf("#") === 0) {
                        value = value.substring(1);
                        isConstant = true;
                    }

                    var ruleQueryConstraint = new RuleQueryConstraint(ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwLoadType,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwMessageId,
                        value,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwOperator,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwLoadSource,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwMode,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwRelatedFieldConstraints,
                        isConstant,
                        ctrl.ruleQueryConstraints.Elements[index].dictAttributes.sfwExecuteOnChange
                    );
                    var parameters = ctrl.ruleQueryConstraints.Elements[index].Elements.filter(function (itm) { return itm.Name === "parameters"; })[0];
                    if (parameters) {
                        for (var i = 0, parLength = parameters.Elements.length; i < parLength; i++) {
                            var expression = parameters.Elements[i].dictAttributes.sfwExpression;
                            var isConstant = false;
                            if (parameters.Elements[i].dictAttributes.sfwExpression && parameters.Elements[i].dictAttributes.sfwExpression.indexOf("#") === 0) {
                                expression = parameters.Elements[i].dictAttributes.sfwExpression.substring(1);
                                isConstant = true;
                            }
                            var param = new Parameter(parameters.Elements[i].dictAttributes.ID, parameters.Elements[i].dictAttributes.sfwDataType, expression, isConstant);
                            ruleQueryConstraint.parameters.push(param);
                        }
                    }

                    //Portal messages needs to be updated.
                    var ruleQueryPortals = ctrl.ruleQueryConstraints.Elements[index].Elements.filter(function (itm) { return itm.Name === "portalmessages"; })[0];
                    if (ruleQueryPortals) {
                        for (var i = 0, portalLength = ruleQueryPortals.Elements.length; i < portalLength; i++) {
                            var ruleQueryPortal = new PortalMessage(ruleQueryPortals.Elements[i].dictAttributes.sfwPortalName, ruleQueryPortals.Elements[i].dictAttributes.sfwMessageId);
                            ruleQueryConstraint.portalMessages.push(ruleQueryPortal);
                        }
                    }
                    ctrl.lstConstraints.push(ruleQueryConstraint);
                }
            }
        }
        if (ctrl.attributeName) {
            ctrl.onAttributeChanged();
        }
        else {
            ctrl.allowConstraint();
            ctrl.getErrorObject(ctrl).attributeName = "The field can not be empty.";
        }
        if (!ctrl.isNewMode && scope.$parent.dialog && scope.$parent.dialog.updateTitle) {
            scope.$parent.dialog.updateTitle("Edit Constraint");
        }

        //Validate on load
        ctrl.validate();
    };
    ctrl.onOKClick = function () {
        //Clear existing constraints from the attribute constraint model
        ctrl.attributeConstraintModel.dictAttributes = {};
        if (ctrl.attributePortalMessages) {
            while (ctrl.attributePortalMessages.Elements.length) {
                scope.$root.DeleteItem(ctrl.attributePortalMessages.Elements[0], ctrl.attributePortalMessages.Elements);
            }
        }
        else {
            ctrl.attributePortalMessages = new BaseModel("portalmessages");
            ctrl.attributeConstraintModel.Elements.push(ctrl.attributePortalMessages);
        }


        if (ctrl.ruleQueryConstraints) {
            while (ctrl.ruleQueryConstraints.Elements.length) {
                scope.$root.DeleteItem(ctrl.ruleQueryConstraints.Elements[0], ctrl.ruleQueryConstraints.Elements);
            }
        }
        else {
            ctrl.ruleQueryConstraints = new BaseModel("rulequeryconstraints");
            ctrl.attributeConstraintModel.Elements.push(ctrl.ruleQueryConstraints);
        }

        ctrl.attributeConstraintModel.dictAttributes.sfwFieldName = ctrl.attributeName;
        ctrl.attributeConstraintModel.dictAttributes.sfwDataType = ctrl.datatype;
        ctrl.attributeConstraintModel.dictAttributes.sfwDisplayName = ctrl.displayName;

        //compile ctrl.lstConstraints list and update values in ctrl.attributeConstraintModel
        for (var index = 0, len = ctrl.lstConstraints.length; index < len; index++) {
            var constraint = ctrl.lstConstraints[index];

            //Update Required Constraint
            if (constraint.type === "Required") {
                ctrl.attributeConstraintModel.dictAttributes.sfwRequired = "True";
                ctrl.attributeConstraintModel.dictAttributes.sfwReqMessageId = constraint.messageId;
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        var portalModel = ctrl.attributePortalMessages.Elements.filter(function (x) { return x.dictAttributes.sfwPortalName === constraint.portalMessages[i].portalName; })[0];
                        if (portalModel) {
                            portalModel.dictAttributes.sfwReqMessageId = constraint.portalMessages[i].messageId;
                        }
                        else {
                            ctrl.attributePortalMessages.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwReqMessageId: constraint.portalMessages[i].messageId }));
                        }
                    }
                }
            }
            //Update Min Constraint
            else if (constraint.type === "Min") {
                ctrl.attributeConstraintModel.dictAttributes.sfwMinValue = constraint.value;
                ctrl.attributeConstraintModel.dictAttributes.sfwMinMessageId = constraint.messageId;
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        var portalModel = ctrl.attributePortalMessages.Elements.filter(function (x) { return x.dictAttributes.sfwPortalName === constraint.portalMessages[i].portalName; })[0];
                        if (portalModel) {
                            portalModel.dictAttributes.sfwMinMessageId = constraint.portalMessages[i].messageId;
                        }
                        else {
                            ctrl.attributePortalMessages.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwMinMessageId: constraint.portalMessages[i].messageId }));
                        }
                    }
                }
            }
            //Update Max Constraint
            else if (constraint.type === "Max") {
                ctrl.attributeConstraintModel.dictAttributes.sfwMaxValue = constraint.value;
                ctrl.attributeConstraintModel.dictAttributes.sfwMaxMessageId = constraint.messageId;
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        var portalModel = ctrl.attributePortalMessages.Elements.filter(function (x) { return x.dictAttributes.sfwPortalName === constraint.portalMessages[i].portalName; })[0];
                        if (portalModel) {
                            portalModel.dictAttributes.sfwMaxMessageId = constraint.portalMessages[i].messageId;
                        }
                        else {
                            ctrl.attributePortalMessages.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwMaxMessageId: constraint.portalMessages[i].messageId }));
                        }
                    }
                }
            }
            //Update Length Constraint
            else if (constraint.type === "Length") {
                ctrl.attributeConstraintModel.dictAttributes.sfwLength = constraint.value;
                ctrl.attributeConstraintModel.dictAttributes.sfwLengthMessageId = constraint.messageId;
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        var portalModel = ctrl.attributePortalMessages.Elements.filter(function (x) { return x.dictAttributes.sfwPortalName === constraint.portalMessages[i].portalName; })[0];
                        if (portalModel) {
                            portalModel.dictAttributes.sfwLengthMessageId = constraint.portalMessages[i].messageId;
                        }
                        else {
                            ctrl.attributePortalMessages.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwLengthMessageId: constraint.portalMessages[i].messageId }));
                        }
                    }
                }
            }
            //Update Compare Constraint
            else if (constraint.type === "Compare") {
                ctrl.attributeConstraintModel.dictAttributes.sfwRelatedField = constraint.value;
                ctrl.attributeConstraintModel.dictAttributes.sfwRelDisplayName = constraint.relatedDisplayName;
                ctrl.attributeConstraintModel.dictAttributes.sfwCompMessageId = constraint.messageId;
                ctrl.attributeConstraintModel.dictAttributes.sfwOperator = constraint.operator;
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        var portalModel = ctrl.attributePortalMessages.Elements.filter(function (x) { return x.dictAttributes.sfwPortalName === constraint.portalMessages[i].portalName; })[0];
                        if (portalModel) {
                            portalModel.dictAttributes.sfwCompMessageId = constraint.portalMessages[i].messageId;
                        }
                        else {
                            ctrl.attributePortalMessages.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwCompMessageId: constraint.portalMessages[i].messageId }));
                        }
                    }
                }
            }
            //Update Rule/Query Constraint
            else {
                var ruleQueryConstraintModel = new BaseModel("rulequery", {
                    sfwLoadType: constraint.type,
                    sfwLoadSource: constraint.loadSource,
                    sfwOperator: constraint.operator,
                    sfwCompValue: constraint.isConstantValue ? "#" + constraint.value : constraint.value,
                    sfwRelatedFieldConstraints: constraint.dependentAttributes,
                    sfwMessageId: constraint.messageId,
                    sfwMode: constraint.mode,
                    sfwExecuteOnChange: constraint.executeOnChange
                });
                if (constraint.parameters.length) {
                    //Populate parameters
                    var parametersModel = new BaseModel("parameters");
                    ruleQueryConstraintModel.Elements.push(parametersModel);
                    for (var i = 0, paramLength = constraint.parameters.length; i < paramLength; i++) {
                        parametersModel.Elements.push(new BaseModel("parameter", { ID: constraint.parameters[i].id, sfwDataType: constraint.parameters[i].datatype, sfwExpression: constraint.parameters[i].isConstant ? "#" + constraint.parameters[i].expression : constraint.parameters[i].expression }));
                    }
                }
                if (constraint.portalMessages.length) {
                    //Populate portal message
                    var portalMessagesModel = new BaseModel("portalmessages");
                    ruleQueryConstraintModel.Elements.push(portalMessagesModel);
                    for (var i = 0, portalLength = constraint.portalMessages.length; i < portalLength; i++) {
                        portalMessagesModel.Elements.push(new BaseModel("portal", { sfwPortalName: constraint.portalMessages[i].portalName, sfwMessageId: constraint.portalMessages[i].messageId }));
                    }
                }
                ctrl.ruleQueryConstraints.Elements.push(ruleQueryConstraintModel);
            }
        }

        if (ctrl.autoSave) {
            $.connection.hubEntityModel.server.addAttributeConstraintAndSave(ctrl.attributeConstraintModel, ctrl.entityId).done(function (data) {
            });
        }
        else {
            //if open in new mode, then add attribute constraint model in entity model.
            if (ctrl.isNewMode) {
                var entityConstraintModel = ctrl.entityModel.Elements.filter(function (itm) { return itm.Name === "constraint"; })[0];
                if (!entityConstraintModel) {
                    entityConstraintModel = new BaseModel("constraint");
                    scope.$root.PushItem(entityConstraintModel, ctrl.entityModel.Elements);
                }
                scope.$root.PushItem(ctrl.attributeConstraintModel, entityConstraintModel.Elements);
            }
        }

        //close dialog
        if (scope.onCancelClick) {
            scope.onCancelClick();
        }
    };
    ctrl.onAttributeChanged = function () {
        //Set the datatype and display name
        var attributes = scope.entityIntellisenseFactory.getAttributes(ctrl.entityId, ["column", "property"], null, true);
        var attribute = attributes.filter(function (x) { return x.ID === ctrl.attributeName; })[0];
        if (attribute) {
            scope.$evalAsync(function () {
                ctrl.datatype = attribute.DataType;
                if (!ctrl.displayName) {
                    ctrl.displayName = attribute.Caption;
                }
            });
        }
        ctrl.allowConstraint();
        ctrl.validate();
    };
    ctrl.allowConstraint = function () {
        scope.$evalAsync(function () {
            //reset all values
            ctrl.allowMinConstraint = false;
            ctrl.allowMaxConstraint = false;
            ctrl.allowLengthConstraint = false;
            ctrl.allowCompareConstraint = false;


            //Depending on datatype of the attribute, remove the constraints which are not compatible.

            //Min and Max constraint is only for number datatypes
            if (ctrl.datatype && ["decimal", "double", "float", "long", "short", "int"].indexOf(ctrl.datatype) > -1) {
                ctrl.allowMinConstraint = true;
                ctrl.allowMaxConstraint = true;
            }
            else {
                //remove min max constraints
                ctrl.deleteConstraint('Min');
                ctrl.deleteConstraint('Max');
            }

            //Length constraint is only for string
            if (ctrl.datatype && ctrl.datatype === "string") {
                ctrl.allowLengthConstraint = true;
            }
            else {
                //remove Length constraint
                ctrl.deleteConstraint('Length');
            }

            //Compare is only for number and date datatypes.    
            if (ctrl.datatype && ["decimal", "double", "float", "long", "short", "int", "datetime"].indexOf(ctrl.datatype) > -1) {
                ctrl.allowCompareConstraint = true;
            }
            else {
                //remove Compare constraint
                ctrl.deleteConstraint('Compare');
            }

            ctrl.updateHasConstraintFlags();
        });
    };
    ctrl.updateHasConstraintFlags = function () {
        ctrl.hasRequiredConstraint = ctrl.lstConstraints.some(function (x) { return x.type === "Required"; });
        ctrl.hasMinConstraint = ctrl.lstConstraints.some(function (x) { return x.type === "Min"; });
        ctrl.hasMaxConstraint = ctrl.lstConstraints.some(function (x) { return x.type === "Max"; });
        ctrl.hasLengthConstraint = ctrl.lstConstraints.some(function (x) { return x.type === "Length"; });
        ctrl.hasCompareConstraint = ctrl.lstConstraints.some(function (x) { return x.type === "Compare"; });
    }
    ctrl.selectConstraint = function (constraint) {
        ctrl.selectedConstraint = constraint;
        if (ctrl.selectedConstraint.messageId && !ctrl.selectedConstraint.message) {
            ctrl.updateMessageText(ctrl.selectedConstraint);
        }
    };
    ctrl.onLoadSourceChanged = function (constraint) {
        ctrl.populateParameters(constraint);
        ctrl.validate();
    }
    ctrl.populateParameters = function (constraint) {
        //Populate Parameters
        if (constraint) {
            var existingParameters = null;
            if (constraint.parameters.length) {
                existingParameters = JSON.parse(JSON.stringify(constraint.parameters));
            }
            constraint.parameters.length = 0;
            if (constraint.loadSource) {
                var obj = null;
                if (constraint.type === "Rule") {
                    var entityID = ctrl.entityId;
                    var ruleID = null;
                    if (constraint.loadSource.indexOf(".") > -1) {
                        var entityRule = constraint.loadSource.split(".");
                        if (entityRule.length > 1) {
                            entityID = entityRule[0];
                            ruleID = entityRule[1];
                        }
                    }
                    else {
                        ruleID = constraint.loadSource;
                    }
                    if (entityID && ruleID) {
                        var lstRules = scope.entityIntellisenseService.GetIntellisenseData(entityID, "", "", true, false, false, true, false, false);
                        obj = lstRules.filter(function (x) { return x.ID === ruleID; })[0];
                    }
                }
                else if (constraint.type === "Query") {
                    var obj = scope.entityIntellisenseFactory.getQueryByQueryName(constraint.loadSource);
                }
                if (obj) {
                    for (var index = 0, len = obj.Parameters.length; index < len; index++) {
                        var param = new Parameter(obj.Parameters[index].ID, obj.Parameters[index].DataType);
                        constraint.parameters.push(param);
                        if (existingParameters) {
                            var existingParam = existingParameters.filter(function (x) { return x.id === param.id && x.datatype === param.datatype; })[0];
                            if (existingParam) {
                                param.expression = existingParam.expression;
                                param.isConstant = existingParam.isConstant;
                            }
                        }
                    }
                }
            }
        }
    };
    ctrl.setParameters = function (constraint) {
        var newScope = scope.$new();
        newScope.ctrl = ctrl;
        newScope.constraint = JSON.parse(JSON.stringify(constraint));
        newScope.okClick = function () {
            ctrl.selectedConstraint.parameters = newScope.constraint.parameters;
            if (newScope.dialog && newScope.dialog.close) {
                newScope.dialog.close();
            }
        }

        newScope.dialog = scope.$root.showDialog(newScope, "Set Parameters", "Entity/views/Constraint/SetConstraintParametersDialog.html", { showclose: true })
    };
    ctrl.onRelatedFieldChanged = function (constraint) {
        if (constraint.type === "Compare") {
            //Populate related field display name.
            var attributesModel = ctrl.entityModel.Elements.filter(function (x) { return x.Name.toLowerCase() === "attributes"; })[0];
            if (attributesModel) {
                var attributeModel = attributesModel.Elements.filter(function (x) { return x.dictAttributes.ID === constraint.value; })[0];
                if (attributeModel) {
                    scope.$evalAsync(function () {
                        constraint.relatedDisplayName = attributeModel.dictAttributes.sfwCaption;
                    });
                }
            }
        }
        ctrl.validate();
    };
    ctrl.addConstraint = function (type) {
        var constraint = null;
        switch (type) {
            case "Required":
                if (!ctrl.lstConstraints.some(function (itm) { return itm.type === type; })) {
                    constraint = new BaseConstraint(type);
                }
                break;
            case "Min":
            case "Max":
            case "Length":
                if (!ctrl.lstConstraints.some(function (itm) { return itm.type === type; })) {
                    constraint = new ValueConstraint(type);
                }
                break;
            case "Compare":
                if (!ctrl.lstConstraints.some(function (itm) { return itm.type === type; })) {
                    constraint = new CompareConstraint(type);
                }
                break;
            case "Rule":
            case "Query":
                constraint = new RuleQueryConstraint(type);
                break;
        }
        if (constraint) {
            ctrl.lstConstraints.push(constraint);
            ctrl.selectConstraint(constraint);
            ctrl.validate();
        }
    };
    ctrl.updateConstraints = function () {
        if (ctrl.hasRequiredConstraint) {
            ctrl.addConstraint('Required');
        }
        else {
            ctrl.deleteConstraint('Required');
        }
        if (ctrl.hasLengthConstraint) {
            ctrl.addConstraint('Length');
        }
        else {
            ctrl.deleteConstraint('Length');
        }
        if (ctrl.hasMinConstraint) {
            ctrl.addConstraint('Min');
        }
        else {
            ctrl.deleteConstraint('Min');
        }
        if (ctrl.hasMaxConstraint) {
            ctrl.addConstraint('Max');
        }
        else {
            ctrl.deleteConstraint('Max');
        }
        if (ctrl.hasCompareConstraint) {
            ctrl.addConstraint('Compare');
        }
        else {
            ctrl.deleteConstraint('Compare');
        }
    }
    ctrl.deleteConstraint = function (type) {
        var constraint = null;
        if (type) {
            constraint = ctrl.lstConstraints.filter(function (itm) { return itm.type === type; })[0];
        }
        else {
            constraint = ctrl.selectedConstraint;
        }
        if (constraint) {
            var index = ctrl.lstConstraints.indexOf(constraint);
            if (index > -1) {
                ctrl.lstConstraints.splice(index, 1);

                if (constraint === ctrl.selectedConstraint) {
                    if (index === ctrl.lstConstraints.length) {
                        index--;
                    }
                    if (index > -1) {
                        ctrl.selectedConstraint = ctrl.lstConstraints[index];
                    }
                    else {
                        ctrl.selectedConstraint = null;
                    }
                }

                ctrl.validate();
            }
        }
        if (!type) {
            ctrl.updateHasConstraintFlags();
        }
    };
    ctrl.onMessageIdChanged = function (constraint) {
        ctrl.updateMessageText(constraint);
        ctrl.validate();
    }
    ctrl.updateMessageText = function (item) {
        if (item) {
            item.message = "";
            if (item.messageId && ctrl.lstMessages) {
                var msg = ctrl.lstMessages.filter(function (x) { return x.MessageID == item.messageId; })[0];
                if (msg) {
                    item.message = msg.DisplayMessage;
                }
            }
        }
    };
    ctrl.validateEntityAttribute = function (obj, property) {
        if (obj && property && obj.hasOwnProperty(property)) {
            var attributes = scope.entityIntellisenseFactory.getAttributes(ctrl.entityId, ["column", "property"], null, true)
            if (!attributes.some(function (attr) { return attr.ID === obj[property]; })) {
                ctrl.getErrorObject(obj)[property] = "Invalid Attribute. Please select a valid column/property type attribute.";
            }
        }
    }
    ctrl.validateAttributeName = function () {
        ctrl.errors = null;
        if (ctrl.attributeName) {
            if (ctrl.constraintAlreadyPresent()) {
                ctrl.getErrorObject(ctrl).attributeName = "A constraint on this attribute already present.";
            }
            else {
                ctrl.validateEntityAttribute(ctrl, 'attributeName');
            }
        }
        else {
            ctrl.getErrorObject(ctrl).attributeName = "Attribute Name cannot be empty.";
        }
    };
    ctrl.validateConstraint = function (constraint) {
        if (constraint.type === "Rule" || constraint.type === "Query") {
            if (!constraint.loadSource) {
                ctrl.getErrorObject(constraint).loadSource = constraint.type + "ID cannot be empty.";
            }
        }

        if (constraint.type !== "Required") {
            ctrl.validateValue(constraint);
        }

        if (constraint.errors && !constraint.errors.loadSource && !constraint.errors.value && !constraint.errors.messageId) {
            constraint.errors = null;
        }
    };
    ctrl.validateValue = function (constraint) {
        //Value cannot be empty.
        var objErrors = ctrl.getErrorObject(constraint);
        objErrors.value = null;
        if (!constraint.value) {
            objErrors.value = "Value cannot be empty.";
        }
        else if (["Min", "Max", "Length"].indexOf(constraint.type) > -1) {
            //Value should be valid number.
            if (isNaN(constraint.value)) {
                objErrors.value = "Value should be numeric.";
            }
        }
        else if (["Compare", "Rule", "Query"].indexOf(constraint.type) > -1) {
            //Value should not be same as the base Attribute.
            if (constraint.value === ctrl.attributeName) {
                objErrors.value = "Value(Related Attribute Name) cannot be same as the Attribute Name.";
            }

            if (!constraint.isConstantValue) {
                //Check if value is a valid attribute
                ctrl.validateEntityAttribute(constraint, 'value');
            }
        }
    };
    ctrl.validate = function () {
        ctrl.errorMessage = "";

        //Attribute Name should not be empty and invalid.
        ctrl.validateAttributeName();

        if (ctrl.errors && ctrl.errors.attributeName) {
            ctrl.errorMessage = ctrl.errors.attributeName;
        }
        else if (!ctrl.lstConstraints.length) {
            ctrl.errors = null;
            ctrl.errorMessage = "Please add at least one type of constraint.";
        }
        else {
            ctrl.errors = null;
            //Validate constraints as per datatype and prompt to remove which are not valid.

            //Validate individual constraints constraints
            for (var index = 0, len = ctrl.lstConstraints.length; index < len; index++) {
                ctrl.validateConstraint(ctrl.lstConstraints[index]);
            }

            if (ctrl.lstConstraints.some(function (item) { return item.errors; })) {
                ctrl.errorMessage = "Please resolve the validation error(s) for one or more constraints.";
            }
        }
    };
    ctrl.constraintAlreadyPresent = function () {
        var retValue = false;
        var entityConstraintModel = ctrl.entityModel.Elements.filter(function (itm) { return itm.Name === "constraint"; })[0];
        if (entityConstraintModel) {
            retValue = entityConstraintModel.Elements.some(function (item) { return item.Name === "item" && item.dictAttributes.sfwFieldName === ctrl.attributeName && item !== ctrl.attributeConstraintModel; });
        }
        return retValue;
    }
    ctrl.setPortalMessages = function (constraint) {
        var newScope = scope.$new();
        newScope.ctrl = ctrl;
        newScope.constraint = JSON.parse(JSON.stringify(constraint));
        newScope.okClick = function () {
            ctrl.selectedConstraint.portalMessages = newScope.constraint.portalMessages;
            if (newScope.dialog && newScope.dialog.close) {
                newScope.dialog.close();
            }
        }
        //#region Portals
        newScope.selectPortal = function (portal) {
            newScope.selectedPortal = portal;
        };
        newScope.addPortal = function () {
            var item = new PortalMessage();
            newScope.constraint.portalMessages.push(item);
            newScope.selectPortal(item);
            newScope.validatePortals();
        };
        newScope.canDeletePortal = function () {
            return newScope.selectedPortal;
        };
        newScope.deletePortal = function () {
            var index = newScope.constraint.portalMessages.indexOf(newScope.selectedPortal);
            if (index > -1) {
                newScope.constraint.portalMessages.splice(index, 1);
                //Select next item
                if (newScope.constraint.portalMessages.length > 0) {
                    if (index == newScope.constraint.portalMessages.length) {
                        index--;
                    }
                    newScope.selectPortal(newScope.constraint.portalMessages[index]);
                }
            }
            newScope.validatePortals();
        };
        newScope.validatePortal = function (portal) {
            if (portal.portalName) {
                if (newScope.constraint.portalMessages.some(function (itm) { return itm.portalName === portal.portalName && itm !== portal; })) {
                    newScope.ctrl.getErrorObject(portal).portalName = "Duplicate Portal Name. Please select different portal name.";
                }
                else if (portal.errors) {
                    newScope.ctrl.getErrorObject(portal).portalName = null;
                }
            }
            else {
                newScope.ctrl.getErrorObject(portal).portalName = "Portal Name cannot be empty.";
            }

            if (!portal.messageId) {
                newScope.ctrl.getErrorObject(portal).messageId = "Message ID cannot be empty.";
            }
            else if (portal.errors) {
                newScope.ctrl.getErrorObject(portal).portalName = null;
            }
        }
        newScope.validatePortals = function () {
            newScope.portalError = "";

            for (var index = 0, len = newScope.constraint.portalMessages.length; index < len; index++) {
                newScope.validatePortal(newScope.constraint.portalMessages[index]);
            }

            if (newScope.constraint.portalMessages.some(function (itm) { return itm.errors && (itm.errors.portalName || itm.errors.messageId); })) {
                newScope.portalError = "Please resolve validation errors.";
            }
        }
        newScope.onPortalNameChanged = function () {
            newScope.validatePortals();
        }
        newScope.onPortalMessageChanged = function (portal) {
            newScope.validatePortals();
            ctrl.updateMessageText(portal)
        }
        //#endregion

        newScope.validatePortals();
        newScope.dialog = scope.$root.showDialog(newScope, "Set Portal Messages", "Entity/views/Constraint/SetPortalMessagesDialog.html", { showclose: true })
    };
    ctrl.initilize();
}
function BaseConstraint(type, messageId) {
    this.type = type;
    this.messageId = messageId ? messageId : "";
    this.message = "";
    this.portalMessages = [];
}
function ValueConstraint(type, messageId, value) {
    BaseConstraint.call(this, type, messageId);
    this.value = value ? value : "";
}
function CompareConstraint(type, messageId, value, operator) {
    ValueConstraint.call(this, type, messageId, value);
    this.operator = operator ? operator : "Equal";
    this.relatedDisplayName = "";
}
function RuleQueryConstraint(type, messageId, value, operator, loadSource, mode, dependentAttributes, isConstantValue, executeOnChange) {
    CompareConstraint.call(this, type, messageId, value, operator);
    this.loadSource = loadSource ? loadSource : "";
    this.mode = mode ? mode : "All";
    this.dependentAttributes = dependentAttributes ? dependentAttributes : "";
    this.parameters = [];
    this.isConstantValue = isConstantValue ? isConstantValue : false;
    this.executeOnChange = executeOnChange ? executeOnChange : "True";
}
function PortalMessage(portalName, messageId) {
    this.portalName = portalName;
    this.messageId = messageId ? messageId : "";
    this.message = "";
}
function Parameter(id, datatype, expression, isConstant) {
    this.id = id;
    this.datatype = datatype ? datatype : "";
    this.expression = expression ? expression : "";
    this.isConstant = isConstant;
}
function BaseModel(name, attributes) {
    this.Name = name;
    this.dictAttributes = attributes ? attributes : {};
    this.Elements = [];
    this.Children = [];
}