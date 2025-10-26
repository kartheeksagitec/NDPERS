app.directive('activeformintellisensetemplate', ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            activeformname: "=",
            propertyname: '=',
            formtype: '=',
            multiactiveformflag: '=',
            formmodel: '=',
            islookup: '=',
            onactiveformchange: '&',
            canhidesearchactiveformwindow: "=",
            isupdatefromexternalprop: "=",
            isdisabledinput: '='
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_active_form' ng-attr-title='{{model.errors.invalid_active_form}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i> </span><div class='input-group'><input type='text' class='form-control input-sm' ng-disabled='isdisabledinput' id='ScopeId_{{$id}}' title='{{activeformname?activeformname:model.placeHolder}}' ng-model='activeformname' ng-keydown='onkeydown($event)' ng-change='onactiveformchange();validateActiveForm()' ng-blur='onactiveformchange();validateActiveForm()' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateActiveForm' placeholder='{{model.placeHolder}}'/><span class='input-group-addon button-intellisense' ng-disabled='isdisabledinput' ng-click='showActivefromIntellisenseList($event)' ng-hide='multiactiveformflag'></span><span class='input-group-addon btn-search-popup'  ng-click='onSearchActiveFormClick($event)' ng-hide='canhidesearchactiveformwindow||multiactiveformflag'></span><span class='input-group-addon btn-search-multiple-popup' ng-click='onAddMultipleActiveFormClick($event)' ng-show='multiactiveformflag'></span></div></div>",
        link: function (scope, element, attrs) {

            scope.$watch("formtype", function (newVal, oldVal) {
                if (newVal) {
                    if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwMethodName && scope.model.dictAttributes.sfwMethodName == 'btnOpenReport_Click') {
                        scope.filelist = [];
                        scope.filelist.push("wfmReportClientMVVM");
                        scope.filelist.push("wfmReportClient");
                    }
                    else {
                        $.connection.hubMain.server.getFilesByType(scope.formtype, "ScopeId_" + scope.$id, null);
                    }
                }
            });
            scope.receiveList = function (data) {
                scope.filelist = data;
                scope.showplaceHolder();
            };
            scope.showplaceHolder = function () {
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwMethodName && (scope.model.dictAttributes.sfwMethodName == 'btnOpen_Click' || scope.model.dictAttributes.sfwMethodName == 'btnNew_Click') && scope.formmodel && (scope.formmodel.dictAttributes.sfwType == "Lookup" || scope.formmodel.dictAttributes.sfwType == "FormLinkLookup") && scope.filelist) {
                    var newFormID = scope.formmodel.dictAttributes.ID.replace("Lookup", "Maintenance");
                    for (var i = 0; i < scope.filelist.length; i++) {
                        if (scope.filelist[i] == newFormID) {
                            scope.$evalAsync(function () {
                                scope.model.placeHolder = newFormID;
                                scope.model.isPlaceHolder = true;
                            });
                            break;
                        }
                    }
                }
            };

            scope.onkeydown = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                if (!scope.multiactiveformflag) {

                    //if (input.val() == undefined || input.val().trim() == "") {
                    setSingleLevelAutoComplete(input, scope.filelist);
                    if ($(input).data('ui-autocomplete') != undefined) {
                        $(input).autocomplete("enable");
                    }
                    //}
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.filelist) {
                        if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                }
                else {
                    if ($(input).data('ui-autocomplete') != undefined) {
                        $(input).autocomplete("disable");
                    }
                }
            };
            scope.onSearchActiveFormClick = function () {
                //scope.SearchActiveForm = ngDialog.open({
                //    template: "/Form/views/SearchActiveForms.html",
                //    scope: scope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.filelist = scope.filelist;
                var tempFormType = scope.formtype;
                newScope.validateActiveForm = scope.validateActiveForm;
                newScope.isupdatefromexternalprop = scope.isupdatefromexternalprop;
                newScope.selectFormType = tempFormType.split(',');
                newScope.SearchActiveFormDialog = $rootScope.showDialog(newScope, "Search Active Form", "Form/views/SearchActiveForms.html", { width: 800, height: 530 });


                function onActiveFormClick(event, data) {
                    if (scope.propertyname) {
                        $rootScope.EditPropertyValue(scope.activeformname, scope, "activeformname", data);
                    }
                    else {
                        scope.activeformname = data;
                    }
                }

                scope.$on("onSearchActiveFormOkClick", onActiveFormClick);

            };

            scope.onAddMultipleActiveFormClick = function () {
                //scope.ActiveFormType = scope.formtype;
                //scope.IsMultiActiveForm = false;

                //scope.ActiveForm = ngDialog.open({
                //    template: "Views/Form/ActiveForms.html",
                //    scope: scope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.formmodel = scope.formmodel;
                newScope.lstEntityField = scope.lstEntityField;
                newScope.islookup = scope.islookup;
                newScope.ActiveFormType = scope.formtype;
                newScope.IsMultiActiveForm = false;
                newScope.validateActiveForm = scope.validateActiveForm;

                newScope.ActiveFormDialog = $rootScope.showDialog(newScope, "Active Forms", "Form/views/ActiveForms.html", { width: 800, height: 500 });

            };
            scope.showActivefromIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.filelist && scope.filelist.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.filelist, scope);
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.validateActiveForm = function () {
                //if (scope.activeformname && scope.model) {
                //    scope.model.isPlaceHolder = false;
                //}
                //else 
                //if (scope.model) {
                //    scope.showplaceHolder();
                //}
                if (scope.propertyname) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else if (scope.model && scope.model.dictAttributes) {
                        input = scope.model.dictAttributes[property];
                    }
                    $ValidationService.checkActiveForm(scope.filelist ? scope.filelist : [], scope.model, input, property, 'invalid_active_form', CONST.VALIDATION.INVALID_ACTIVE_FORM, undefined);
                }
            };
        },
    };
}]);

app.directive("fileintellisensetemplate", ['$rootScope', '$ValidationService', 'CONSTANTS', function ($rootScope, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            filename: "=",
            propertyname: '=',
            filetype: '=',
            onfilechange: '&',
            isdisabled: '=',
            onfileblur: '&',
            insidegrid: "=",
            errorprop: "=",
            isvalidate: "=",
            showentbase: "=",
            isinsidedialog: "=",
            tablename: "=",
            isreset: "="
        },
        template: "<div><div class='input-group' ng-show='!isinsidedialog'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input  ng-class=\"insidegrid?'form-control full-width form-filter input-sm':'form-control input-sm full-width'\" type='text' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-keyup='validateFileTemplate();validateEntity();' ng-change='onchangecallback()' ng-blur='onblur();' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateFileTemplate'/><span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-class=\"isdisabled?'disabledlink':''\"></span></div><div class='input-group' ng-show='isinsidedialog'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input  ng-class=\"insidegrid?'form-control full-width form-filter input-sm':'form-control input-sm full-width'\" type='text' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-keyup='validateFileTemplate();validateEntity();' ng-change='onchangecallback()' ng-blur='onblur();'/><span class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)' ng-class=\"isdisabled?'disabledlink':''\"></span></div></div>",
        link: function (scope, element, attrs) {
            scope.prevvalue = scope.filename;
            scope.onchangecallback = function () {
                // this condition is only for first time - when change is called before we receive the list from the hub
                if (scope.propertyname && !scope.tempModel) {
                    scope.tempModel = "model";
                    var properties = scope.propertyname.split(".");
                    for (var i = 0, l = properties.length; i < l - 1; i++) {
                        scope.tempModel = scope.tempModel.concat("." + properties[i]);
                    }
                }
                if (!scope.model) {
                    scope.model = { dictAttributes: {} };
                }
                if (scope.tempModel) {
                    var properties = scope.propertyname.split(".");
                    //var t = eval(scope.tempModel);
                    var t;
                    var larrProps = scope.tempModel.split(".");
                    var ltmpObject = scope;
                    for (var i = 0, len = larrProps.length; i < len; i++) {
                        ltmpObject = ltmpObject[larrProps[i]];
                    }
                    if (ltmpObject !== null) {
                        t = ltmpObject;
                    }
                    t[properties[properties.length - 1]] = scope.filename;
                }
                scope.onfilechange();
                scope.validateFileTemplate();
                scope.validateEntity();
                //if (scope.model.dictAttributes && scope.model.dictAttributes.hasOwnProperty("sfwEntity")) {
                //    $ValidationService.validateEntity(scope.model, undefined);
                //}
            };
            scope.receiveList = function (data) {
                if (scope.propertyname) {
                    scope.tempModel = "model";
                    var properties = scope.propertyname.split(".");
                    for (var i = 0, l = properties.length; i < l - 1; i++) {
                        scope.tempModel = scope.tempModel.concat("." + properties[i]);
                    }
                }
                scope.filelist = data;
                if (scope.filetype && scope.filetype.toLowerCase() == "entity" && scope.showentbase && scope.filelist.indexOf("entBase") == -1) {
                    scope.filelist.splice(0, 0, "entBase");
                }
                scope.inputElement.focus();
                setSingleLevelAutoComplete(scope.inputElement, scope.filelist);
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
                scope.$evalAsync(function () {
                    scope.validateFileTemplate();
                });
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.filelist && !scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                }
                else if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    scope.inputElement.focus();
                    setSingleLevelAutoComplete(scope.inputElement, scope.filelist, scope);
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onkeydown = function (event) {
                if (!scope.filelist) {
                    scope.inputElement = $(event.target);
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.isreset && scope.filelist) {
                    $.connection.hubMain.server.getFilesByType(scope.filetype, "ScopeId_" + scope.$id, scope.tablename);
                    event.preventDefault();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && scope.filelist) {
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    event.preventDefault();
                } else if (!scope.filelist && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    event.preventDefault();
                }
            };
            scope.validateFileTemplate = function () {
                if (scope.propertyname && scope.isvalidate) {
                    var errMessage, errProp;
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else input = scope.model.dictAttributes[property];
                    if (scope.filetype == "UserControl" || scope.filetype == "Tooltip") {
                        errProp = "invalid_active_form";
                        errMessage = CONST.VALIDATION.INVALID_ACTIVE_FORM;
                        scope.onfilechange();
                    }
                    if (scope.filetype == "Entity") errMessage = CONST.VALIDATION.INVALID_ENTITY_NAME;
                    $ValidationService.checkValidListValue(scope.filelist, scope.model, $(scope.inputElement).val(), property, errProp ? errProp : property, errMessage, undefined);
                }
            };
            scope.validateEntity = function () {
                if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.hasOwnProperty("sfwEntity")) {
                    $ValidationService.validateEntity(scope.model, undefined);
                }
            };
            scope.onblur = function () {
                if (scope.prevvalue != scope.filename) {
                    scope.onfileblur();
                    scope.prevvalue = scope.filename;
                }
            };
        }
    };
}]);

app.directive("entityfieldelementintellisense", ['$rootScope', '$filter', '$ValidationService', 'CONSTANTS', '$EntityIntellisenseFactory', function ($rootScope, $filter, $ValidationService, CONST, $EntityIntellisenseFactory) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modebinding: "=",
            entityid: "=",
            propertyname: '=',
            onchangecallback: '&',
            ontextchangecallback: '&',
            isshowonetoone: '=',
            isshowonetomany: '=',
            isshowcolumns: '=',
            isshowcdocollection: '=',
            isshowproperty: '=',
            isshowglobalparams: '=',
            disabled: "=isDisabled",
            isObject: "=",
            onblurcallback: '&',
            candrop: '=',
            validate: '=',
            isshowexpression: '=',
            errorprop: '=',
            isempty: '=',
            setcolumndatatype: '=',
            isaddthis: '='
        },
        template: "<div><div class='input-group full-width'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input ng-disabled='disabled' type='text' ng-change='onchange()' ondrop='onEntityFieldDropInDirective(event)' ng-blur='onBlur()' ondragover='onEntityFieldDragOver(event)' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateEntityField' ng-keyup='onkeyup($event);validateEntityField()' ng-keydown='onkeydown($event)' ng-change='validateEntityField()'/><span ng-show='!disabled' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.setAutocomplete = function (input, event) {
                // this function will only be called when you want to change the data for the intellisense i.e switch between single level to multilevel and vice-versa
                scope.isMultilevelActive = false;
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    var arrText = getSplitArray(input.val(), input[0].selectionStart);
                    var data = [];
                    if (scope.isaddthis) {
                        data.push({ ID: "this", DisplayName: "this" });
                    }
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = scope.entityid;
                    if (scope.model && scope.model.Name == "sfwGridView") {
                        var entity = entities.filter(function (x) {
                            return x.ID == scope.entityid;
                        });
                        if (entity.length > 0 && entity[0].ErrorTableName) {
                            var objInternalError = { ID: "InternalErrors", DisplayName: "InternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbError", Type: "Collection", DataType: "Collection" };
                            var objExternalError = { ID: "ExternalErrors", DisplayName: "ExternalErrors", Tooltip: "InternalErrors", IsPrivate: "False", Entity: "entError", Value: "ibusSoftErrors.iclbEmployerError", DataType: "Collection", Type: "Collection" };
                            data.push(objInternalError);
                            data.push(objExternalError);
                        }
                    }
                    while (parententityName) {
                        data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, true, scope.isshowonetoone, scope.isshowonetomany, scope.isshowcolumns, scope.isshowcdocollection, scope.isshowexpression, scope.isshowproperty));
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            parententityName = entity[0].ParentId;
                        } else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
                        for (var index = 0; index < arrText.length; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    var parententityName = item[0].Entity;
                                    data = [];
                                    while (parententityName) {
                                        //expression should not come for second level
                                        data = data.concat($rootScope.getEntityAttributeIntellisense(parententityName, true, scope.isshowonetoone, scope.isshowonetomany, scope.isshowcolumns, scope.isshowcdocollection, scope.isshowexpression, scope.isshowproperty));
                                        var entity = entities.filter(function (x) {
                                            return x.ID == parententityName;
                                        });
                                        if (entity.length > 0) {
                                            parententityName = entity[0].ParentId;
                                        } else {
                                            parententityName = "";
                                        }
                                    }
                                }
                                else data = [];
                            }
                        }
                    }
                    if (scope.isObject && data && data.length > 0) {
                        data = $filter('filter')(data, { DataType: "Object" });
                    }
                    if (scope.isshowglobalparams) {
                        $.connection.hubForm.server.getGlobleParameters().done(function (globalParams) {
                            scope.$apply(function () {
                                var filterAttributes = [];
                                scope.objGlobleParameters = globalParams;
                                function iterator(itm) {
                                    if (itm.dictAttributes && itm.dictAttributes.ID) {
                                        var mainItem = { ID: "~" + itm.dictAttributes.ID };
                                        filterAttributes.push(mainItem);
                                    }
                                }
                                if (scope.objGlobleParameters) {
                                    if (scope.objGlobleParameters.Elements.length > 0) {
                                        angular.forEach(scope.objGlobleParameters.Elements, iterator);

                                    }
                                }
                                data = data.concat(filterAttributes);

                                setMultilevelAutoCompleteForObjectTreeIntellisense(input, data, "", scope.onchangecallback);
                            });

                        });
                    }
                    else {
                        setMultilevelAutoCompleteForObjectTreeIntellisense(input, data, "", scope.onchangecallback);
                    }
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense(input, [], "", scope.onchangecallback);
                }
            };
            scope.onkeyup = function (event) {
                // this enables the second level autocomplete
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (scope.inputElement) {
                        var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                        if (arrText.length > 1 || arrText.length == 0) scope.setAutocomplete(scope.inputElement, event);
                    }
                }
                scope.ontextchangecallback();
            };
            scope.prevEntityID = undefined;
            scope.onkeydown = function (event) {
                if (scope.entityid && scope.entityid.trim().length > 0) {
                    if (!scope.inputElement || scope.prevEntityID != scope.entityid) {
                        scope.inputElement = $(event.target);
                        scope.setAutocomplete(scope.inputElement, event);
                    }
                    scope.prevEntityID = scope.entityid;
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        }
                        event.preventDefault();
                    }
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    // if user goes back from second level to first level intellisense
                    if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, event);
                }
                else {
                    setMultilevelAutoCompleteForObjectTreeIntellisense($(event.target), [], "", scope.onchangecallback);
                }
                //else if (scope.inputElement) {
                //    scope.setAutocomplete(scope.inputElement, event);
                //}

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
                    var attrType;
                    var one2one = scope.isshowonetoone ? 'Object,' : '';
                    var one2manay = scope.isshowonetomany ? 'Collection,List,' : '';
                    var column = scope.isshowcolumns ? 'Column,' : '';
                    var cdo = scope.isshowcdocollection ? 'CDOCollection,' : '';
                    var exp = scope.isshowexpression ? 'Expression,' : '';

                    attrType = one2one + one2manay + column + cdo + exp;

                    var propertyTemp = scope.propertyname.split('.');
                    property = propertyTemp[propertyTemp.length - 1];
                    var errorMsg = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    if (property != "sfwEntityField") {
                        errorMsg = CONST.VALIDATION.INVALID_FIELD;
                    }
                    var list = [];
                    $ValidationService.checkValidListValueForMultilevel(list, scope.model, $(scope.inputElement).val(), scope.entityid, property, property, errorMsg, undefined, scope.isempty, attrType);
                    var sfwRelFieldVal = $(scope.inputElement).val();
                    if (property == "sfwRelatedField" && (sfwRelFieldVal != "") && !scope.model.dictAttributes.sfwOperator) {
                        if (!scope.model.errors && !angular.isObject(scope.model.errors)) {
                            scope.model.errors = {};
                        }
                        if (!scope.model.errors.sfwOperator) {
                            scope.model.errors.sfwOperator = CONST.VALIDATION.EMPTY_FIELD;
                        }
                    }
                    if (property == "sfwRelatedField" && (sfwRelFieldVal == "") && !scope.model.dictAttributes.sfwOperator) {
                        if (!scope.model.errors && !angular.isObject(scope.model.errors)) {
                            scope.model.errors = {};
                        }
                        if (scope.model.errors.sfwOperator) {
                            delete scope.model.errors.sfwOperator;
                        }
                    }
                    else if (property == "sfwRelatedField" && (sfwRelFieldVal == "") && scope.model.dictAttributes.sfwOperator) {
                        scope.model.dictAttributes.sfwOperator = "";
                    }
                }
            };
            scope.onBlur = function () {
                if (scope.onblurcallback) scope.onblurcallback({ model: scope.model });
                if (scope.ontextchangecallback) scope.ontextchangecallback();
                //  scope.validateEntityField();
            };
            scope.onchange = function () {
                if (scope.onchangecallback) scope.onchangecallback();
                scope.$evalAsync(function () {
                    scope.validateEntityField();
                });
            };
        }
    };
}]);

app.directive("resourceintellisensetemplate", ["$compile", "$rootScope", "$timeout", "$ValidationService", "CONSTANTS", "$filter", function ($compile, $rootScope, $timeout, $ValidationService, CONST, $filter) {

    var getTemplate = function (ablnshowvertical) {
        var template = ""; //<div class='col-xs-6'>
        if (ablnshowvertical) {
            // template += "<div class='form-group'>",
            template += "<div> <label class='control-label' id='ScopeId_{{$id}}' ng-show='showresourcetext'>{{resourcetext}}</label>";
            template += "<div class='input-group' ng-show='!isinsidedialog'>";
            template += "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>";
            template += "<input  type='text' title='{{resourceid}}' ng-model='resourceid' class='form-control input-sm'  ng-change='onchangeresourcecallback();validateResource()' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateResource'/>";
            template += "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span></div>";
            template += "<div class='input-group' ng-show='isinsidedialog'>";
            template += "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>";
            template += "<input type='text' title='{{resourceid}}' ng-model='resourceid' class='form-control input-sm'  ng-change='onchangeresourcecallback();validateResource()' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)'/>";
            template += "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span></div>";
            //template += "</div>",
            template += "</div>";
        }
        else {
            template = [" <label class='{{resourcelabelclass}}' id='ScopeId_{{$id}}' ng-show='showresourcetext'>{{resourcetext}}</label>",
                "<div class='{{resourcetextboxclass}}'>",
                "<div class='input-group'>",
                "<span class='info-tooltip dtc-span' ng-if='model.errors.invalid_resource' ng-attr-title='{{model.errors.invalid_resource}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                "<input type='text' class='form-control input-sm' title='{{resourceid}}' ng-model='resourceid' ng-change='onchangeresourcecallback();validateResource();' ng-keyup='validateResource()' ng-blur='onchangeresourcecallback();' ng-keydown='resourceIDTextChanged($event)' ng-focus='focusIn()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateResource' class='form-control input-sm'/> ",
                "<span class='input-group-addon button-intellisense' ng-click='showResourceIDIntellisenseList($event)'></span>",
                "<span class='input-group-addon btn-search-popup' ng-click='onResouceSearchClick($event)'></span>",
                "</div>",
                "</div>"];
            template = template.join(' ');
        }



        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            resourceid: "=",
            propertyname: "=",
            showresourcetext: '=',
            resourcetext: '=',
            resourcelabelclass: "@",
            resourcetextboxclass: "@",
            onchangeresourcecallback: '&',
            showvertical: "=",
            isinsidedialog: "="
        },
        link: function (scope, element, attrs) {

            if (!scope.showvertical) {
                if (scope.resourcelabelclass == undefined || scope.resourcelabelclass == "") {
                    scope.resourcelabelclass = "col-xs-4 control-label";
                }
                if (scope.resourcetextboxclass == undefined || scope.resourcetextboxclass == "") {
                    scope.resourcetextboxclass = "col-xs-8";
                }
            }
            else {
                scope.resourcelabelclass = "control-label";
            }

            var newScope;
            var parent = $(element).parent();
            parent.html(getTemplate());

            if (scope.showvertical) {
                parent.html(getTemplate(scope.showvertical));
            }
            else {
                parent.html(getTemplate(false));
            }
            $compile(parent.contents())(scope);
            // var newScope = undefined;
            //  $.connection.hubMain.server.getResources("ScopeId_" + scope.$id);
            scope.receiveList = function (data) {
                scope.resourcelist = data;
                if (newScope) {
                    newScope.resourcelist = scope.resourcelist;
                }
                if (scope.resourcelist && scope.inputElement) {
                    var reourceList = [];
                    if (scope.inputElement) {
                        reourceList = scope.resourcelist.sort();
                        setSingleLevelAutoComplete(scope.inputElement, reourceList, scope, "ResourceID", "ResourceIDDescription");
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        }
                    }
                }
            };

            scope.resourceIDTextChanged = function (event) {
                if (event && event.keyCode == $.ui.keyCode.ESCAPE) {
                    return;
                }
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                var value = scope.inputElement.val();

                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }
                if (scope.resourcelist && scope.resourcelist.length > 0) {
                    var data = scope.resourcelist.sort();
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        event.preventDefault();
                    } else {
                        setSingleLevelAutoComplete(scope.inputElement, data, scope, "ResourceID", "ResourceIDDescription");
                    }
                }
            };

            scope.showResourceIDIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }

                if (scope.inputElement && scope.resourcelist) {
                    var resourceList = scope.resourcelist.sort();
                    setSingleLevelAutoComplete(scope.inputElement, resourceList, scope, "ResourceID", "ResourceIDDescription");
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onResouceSearchClick = function (event) {
                scope.strCode = "Resources";

                newScope = scope.$new();
                newScope.strCode = "Resources";
                newScope.validateResource = scope.validateResource;
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                    $timeout(function () {
                        newScope.resourcelist = scope.resourcelist;
                    });
                } else {
                    newScope.resourcelist = scope.resourcelist;
                }

                newScope.SearchIDDescrDialog = $rootScope.showDialog(newScope, "Search ID Description", "Common/views/SearchIDDescription.html", { width: 530, height: 540 });

                newScope.$on('onOKClick', function (event, data) {

                    if (newScope.propertyname) {
                        if (newScope.propertyname.indexOf('.') > -1) {
                            var property = newScope.propertyname.split('.')[1];
                            $rootScope.EditPropertyValue(scope.model.dictAttributes[property], scope.model.dictAttributes, property, data ? data.ID : "");
                        }
                        else {
                            scope.model[newScope.propertyname] = data ? data.ID : "";
                        }
                    }

                    //scope.resourceid = data ? data.ID : "";
                    if (event) {
                        event.stopPropagation();
                    }
                });
                if (event) {
                    event.stopImmediatePropagation();
                }
            };
            scope.focusIn = function () {
                if (!scope.resourcelist) {
                    $.connection.hubMain.server.getResources("ScopeId_" + scope.$id).done(function (data) {
                        scope.receiveList(data);
                    });
                }
            };
            scope.validateResource = function () {
                if (scope.propertyname && scope.model) {
                    var property = scope.propertyname.split('.')[1];
                    var list = ["0"]; //default value
                    if (scope.resourcelist && scope.resourcelist.length > 0) {
                        list = $ValidationService.getListByPropertyName(scope.resourcelist, "ResourceID", true);
                        list.push("0");// default value
                    }
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_resource", CONST.VALIDATION.RESOURCE_NOT_EXISTS, undefined);
                }
            };
        }
    };
}]);

app.directive("visibleruleintellisensetemplate", ["$compile", "$rootScope", "$NavigateToFileService", "$ValidationService", "CONSTANTS", "$GetEntityFieldObjectService", function ($compile, $rootScope, $NavigateToFileService, $ValidationService, CONST, $GetEntityFieldObjectService) {

    var getTemplate = function () {
        var template = "";
        template += " <label class='{{resourcelabelclass}}'><u ng-if='!sfwvisiblerule.trim() || !isnavigationlink'>{{visibleruletext}}</u><a ng-if='sfwvisiblerule.trim() && isnavigationlink' ng-click='selectVisibleRuleClick()'>{{visibleruletext}}</a></label>";
        template += "<div class='{{resourcetextboxclass}}'>";
        template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwVisibleRule\' && model.errors.invalid_visible_rule" ng-attr-title="{{model.errors.invalid_visible_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwEnableRule\' && model.errors.invalid_enable_rule" ng-attr-title="{{model.errors.invalid_enable_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwReadOnlyRule\' && model.errors.invalid_readonly_rule" ng-attr-title="{{model.errors.invalid_readonly_rule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += '<span class="info-tooltip dtc-span" ng-if="propertyname==\'dictAttributes.sfwSelectColVisibleRule\' && model.errors.sfwSelectColVisibleRule" ng-attr-title="{{model.errors.sfwSelectColVisibleRule}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            template += "<div class='input-group'>",
            template += "<input type='text' title='{{sfwvisiblerule}}' ng-model='sfwvisiblerule' ng-keydown='visibleRuleTextChanged($event)' ng-keyup='validateRule()' ng-change='validateRule()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateRule' class='form-control input-sm'/>",
            template += "<span class='input-group-addon button-intellisense' ng-click='showVisibleRuleIntellisense($event)'></span>",
            template += "</div></div>";
        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            sfwvisiblerule: '=',
            propertyname: '=',
            formmodel: '=',
            visibleruletext: '=',
            resourcelabelclass: "@",
            resourcetextboxclass: "@",
            isnavigationlink: '=',
            entityid: '='
        },
        link: function (scope, element, attributes) {

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            scope.PrevEntity = scope.entityid;
            scope.PrevInitGroup = scope.formmodel.dictAttributes.sfwInitialLoadGroup;
            scope.visibleRuleTextChanged = function (event) {

                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.ctrlKey && event.keyCode == 83) { // if user press (ctrl + s) then need to close opened intellisense list
                    event.preventDefault();
                    return;
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    event.preventDefault();
                    //if (scope.formmodel && scope.formmodel.objExtraData) {
                    //    getVisibleRuleData(scope.inputElement);
                    //    event.preventDefault();
                    //}
                } else if (!$(scope.inputElement).val() && event.keyCode != 38 && event.keyCode != 40) {
                    getVisibleRuleData(scope.inputElement, "change");
                }
            };
            scope.validateRule = function () {
                if (scope.propertyname) {
                    var property = scope.propertyname.split('.')[1];
                    var list = scope.data ? scope.data : [];
                    if (property == "sfwVisibleRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_visible_rule", CONST.VALIDATION.VISIBLE_RULE_NOT_EXISTS, undefined);
                    } else if (property == "sfwEnableRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_enable_rule", CONST.VALIDATION.ENABLE_RULE_NOT_EXISTS, undefined);
                    } else if (property == "sfwReadOnlyRule") {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_readonly_rule", CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, undefined);
                    } else {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.READONLY_RULE_NOT_EXISTS, undefined);
                    }
                }
            };
            scope.showVisibleRuleIntellisense = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();

                if (scope.inputElement) {
                    getVisibleRuleData(scope.inputElement, "change");
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.selectVisibleRuleClick = function () {
                getVisibleRuleData(undefined, "navigate");
            };

            var navigateToRule = function () {
                scope.validateRule();
                if (scope.propertyname) var property = scope.propertyname.split('.')[1];
                var errorProp = "";
                if (property == "sfwVisibleRule") {
                    errorProp = "invalid_visible_rule";
                }
                else if (property == "sfwEnableRule") {
                    errorProp = "invalid_enable_rule";
                }
                else {
                    errorProp = "invalid_readonly_rule";
                }
                var IsNavigate = false;
                if (!scope.model.errors) {
                    IsNavigate = true;
                }
                else if (scope.model.errors && !scope.model.errors[errorProp]) {
                    IsNavigate = true;
                }
                if (scope.sfwvisiblerule && scope.sfwvisiblerule.trim() != "" && IsNavigate) {
                    var nodeName = "initialload";
                    //if (scope.formmodel.dictAttributes.sfwType == "Wizard" || scope.formmodel.dictAttributes.sfwType == "FormLinkWizard") nodeName = "groupslist";
                    var entityID = "";
                    if (scope.formmodel && scope.formmodel.SelectedControl && scope.formmodel.SelectedControl.IsGridChildOfListView) {
                        entityID = scope.entityid;
                    }
                    else if (scope.model && (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwChart" || scope.model.Name == "sfwListView")) {
                        if (scope.model.dictAttributes.sfwParentGrid && scope.model.Name != "TemplateField") {
                            var objParentGrid = FindControlByID(scope.formmodel, scope.model.dictAttributes.sfwParentGrid);
                            if (objParentGrid) {
                                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                                if (entObject) {
                                    entityID = entObject.Entity;
                                }
                            }

                            else {
                                entityID = scope.formmodel.dictAttributes.sfwEntity;
                            }
                        }
                        else if (scope.model.dictAttributes.sfwParentGrid && scope.model.Name == "TemplateField") {
                            entityID = scope.entityid;
                        }
                        else {
                            entityID = scope.formmodel.dictAttributes.sfwEntity;
                        }
                    }
                    else if (scope.entityid) {
                        entityID = scope.entityid;
                    }
                    else if (scope.formmodel && scope.formmodel.dictAttributes) {
                        entityID = scope.formmodel.dictAttributes.sfwEntity;
                    }
                    $NavigateToFileService.NavigateToFile(entityID, nodeName, scope.sfwvisiblerule);
                }
            };

            var getVisibleRuleData = function (input, action) {
                var entityID = "";
                var iswizard = scope.formmodel.dictAttributes.sfwType == "Wizard" || scope.formmodel.dictAttributes.sfwType == "FormLinkWizard" ? true : false;
                if (scope.model && (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwChart" || scope.model.Name == "sfwListView")) {
                    if (scope.formmodel && scope.formmodel.SelectedControl && scope.formmodel.SelectedControl.IsGridChildOfListView) {
                        entityID = scope.entityid;
                    }
                    else if (scope.model.dictAttributes.sfwParentGrid) {
                        var objGrid = FindControlByID(scope.formmodel, scope.model.dictAttributes.sfwParentGrid);
                        if (objGrid) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                            if (object) {
                                entityID = object.Entity;
                            }
                        }
                    }
                    else {
                        entityID = scope.formmodel.dictAttributes.sfwEntity;
                    }
                }
                else if (scope.entityid) {
                    entityID = scope.entityid;
                }
                else {
                    var ObjGrid = FindParent(scope.model, "sfwGridView");
                    if (ObjGrid) {
                        if (ObjGrid.dictAttributes.sfwBoundToQuery && ObjGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                            if (scope.formmodel && scope.formmodel.dictAttributes) {
                                entityID = scope.formmodel.dictAttributes.sfwEntity;
                            }
                        }
                        else if (ObjGrid && ObjGrid.dictAttributes.sfwParentGrid && ObjGrid.dictAttributes.sfwEntityField) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formmodel.dictAttributes.sfwEntity, ObjGrid.dictAttributes.sfwEntityField);
                            if (object) {
                                entityID = object.Entity;
                            }
                        }
                    }
                }
                if (entityID) {
                    hubMain.server.getEntityExtraData(entityID).done(function (data) {
                        scope.$evalAsync(function () {
                            var groupName;
                            if (scope.model && scope.model.ParentVM && scope.model.ParentVM.Name != "ItemTemplate" && entityID == scope.formmodel.dictAttributes.sfwEntity && scope.formmodel.dictAttributes.sfwInitialLoadGroup) {
                                groupName = scope.formmodel.dictAttributes.sfwInitialLoadGroup;
                            }
                            scope.data = PopulateEntityRules(data, iswizard, groupName, scope.model.dictAttributes.sfwRulesGroup /* scope.formmodel.dictAttributes.sfwInitialLoadGroup*/);
                            if (action == "change") {
                                if (input) setSingleLevelAutoComplete(input, scope.data);
                                if ($(scope.inputElement).data('ui-autocomplete')) {
                                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                                }
                            } else if (action == "navigate") {
                                navigateToRule();
                            }
                        });
                    });
                } else if (scope.inputElement) { // if entity is empty then clear intellisense
                    setSingleLevelAutoComplete(scope.inputElement, []);
                }

            };
        }
    };
}]);

app.directive('autoCompleteConfigurationSetting', ["$timeout", "hubcontext", function ($timeout, hubcontext) {
    return {
        restrict: "A",
        scope: {
            basepath: '=',
            searchtext: '=',
        },

        link: function (scope, element, attrs) {
            element.keyup(function () {
                if (scope.basepath && scope.basepath != "") {
                    if (scope.searchtext.length > 2) {
                        hubcontext.hubMain.server.getFolderPathIntellisense(attrs.id, scope.searchtext, scope.basepath);
                    }
                }
            });
        },
    };
}]);

app.directive("messageintellisensetemplate", ["$compile", "$rootScope", "ngDialog", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, ngDialog, $ValidationService, CONST) {

    var getTemplate = function (ablnshowvertical) {
        var template = "";

        if (ablnshowvertical) {
            template += "<div>",
                template += "<div class='col-xs-12' ng-show='!isinsidedialog'>",
                template += " <label class='control-label col-xs-12 no-left-padding' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='input-group col-xs-3 fleft'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMessageID'/>",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "<div class='col-xs-9'><span ng-if='messageid' class=\"file-detail-msg msgfont-11\" ng-bind='newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ></span>";
            template += "<span ng-if='!messageid' class=\"file-empty-mesage\">No Message</span></div></div>";
            template += "<div class='col-xs-12' ng-show='isinsidedialog'>",
                template += " <label class='control-label col-xs-12 no-left-padding' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='input-group col-xs-3 fleft'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' />",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "<div class='col-xs-9' ng-if='!hideMessageText'><span ng-if='messageid' class=\"file-detail-msg msgfont-11\" ng-bind='newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ></span>";
            template += "<span ng-if='!messageid' class=\"file-empty-mesage\">No Message</span></div></div>";
            template += "</div>";

        }
        else {
            template += " <label class='{{messagelabelclass}}' id='ScopeId_{{$id}}' ng-show='showmessagetext'>{{messagetext}}</label>";
            template += "<div class='{{messagetextboxclass}}'>";
            template += "<div ng-show='!isinsidedialog'>",
                template += "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' ng-blur='onblurcallback()' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMessageID'/>",
                template += "<span ng-class='isdisabled?\"disabledlink\" : \"\"' class='input-group-addon button-intellisense' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "</div>";
            template += "<div  ng-show='isinsidedialog'>",
                template += "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "<input ng-disabled='isdisabled' type='text' title='{{messageid + newDisplayMessage}}' ng-model='messageid' class='form-control input-sm' ng-keydown='messageIDTextChanged($event)' ng-keyup='validateMessageID()' ng-change='populateMessageForMessage(messageid);validateMessageID()' ng-blur='onblurcallback()' />",
                template += "<span class='input-group-addon button-intellisense' ng-class='isdisabled?\"disabledlink\" : \"\"' ng-click='showMessageIntellisenseList($event)'></span>",
                //template += "<span ng-if='showTooltipIcons && newDisplayMessage' ng-attr-title='{{newDisplayMessage}}' ng-class='newSeverityValue===\"Information\"?\"color-info\":(newSeverityValue===\"Error\"?\"color-error\":\"color-warning\")' style='top: 6px; right: -15px; position: absolute;'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>",
                template += "</div>",
                template += "</div><span ng-if='newDisplayMessage && !hideMessageText'  class=\"help-block msgfont-11\" ng-attr-title='{{newDisplayMessage}}' ng-bind='newDisplayMessage'></span></div>";
            template += "</div>";

        }

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            messageid: "=",
            propertyname: "=",
            showmessagetext: '=',
            messagetext: '=',
            newDisplayMessage: '@',
            newSeverityValue: '@',
            messagelabelclass: "@",
            messagetextboxclass: "@",
            showvertical: "=",
            errorprop: '=',
            isvalidate: '=',
            onchangecallback: '&',
            onblurcallback: '&',
            isinsidedialog: "=",
            isempty: '=',
            isdisabled: '=',
            hideMessageText: "=",
            showTooltipIcons: "="

        },
        link: function (scope, element, attrs) {

            if (scope.messagelabelclass == undefined || scope.messagelabelclass == "") {
                scope.messagelabelclass = "col-xs-4 control-label";
            }
            if (scope.messagetextboxclass == undefined || scope.messagetextboxclass == "") {
                scope.messagetextboxclass = "col-xs-8";
            }
            var parent = $(element).parent();
            if (scope.showvertical) {
                parent.html(getTemplate(scope.showvertical));
            }
            else {
                parent.html(getTemplate(false));
            }
            $compile(parent.contents())(scope);

            hubMain.server.populateMessageList().done(function (lstMessages) {
                scope.$evalAsync(function () {
                    scope.messagelist = lstMessages;
                    if (scope.messageid) {
                        scope.populateMessageForMessage(scope.messageid);
                    }
                });
            });

            scope.$watch("messageid", function (newValue, oldValue) {
                scope.populateMessageForMessage(scope.messageid);
            });
            scope.messageIDTextChanged = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                var value = input.val();
                var data = [];
                if (scope.messagelist) {
                    data = scope.messagelist.sort();
                }
                setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    // setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                // else {
                //    setSingleLevelAutoComplete(input, data, scope, "MessageID", "DisplayMessageID");
                //}
            };
            scope.showMessageIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    setSingleLevelAutoComplete(scope.inputElement, scope.messagelist, scope, "MessageID", "DisplayMessageID");
                }
                scope.inputElement.focus();

                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.populateMessageForMessage = function (messageID) {

                var messageIDFound = false;
                if (messageID && messageID.trim().length > 0) {
                    if (scope.messagelist && scope.messagelist.length > 0) {
                        var messages = scope.messagelist.filter(function (x) {
                            return x.MessageID == messageID;
                        });
                        if (messages && messages.length > 0) {
                            scope.newDisplayMessage = messages[0].DisplayMessage;

                            if (messages[0].SeverityValue == 'I') {
                                scope.newSeverityValue = "Information";
                            }
                            else if (messages[0].SeverityValue == 'E') {
                                scope.newSeverityValue = "Error";
                            }
                            else if (messages[0].SeverityValue == 'W') {
                                scope.newSeverityValue = "Warnings";
                            }
                            scope.newDisplayMessage = String.format("{0}{1}{2}{3}", " ", scope.newSeverityValue, scope.newDisplayMessage && scope.newDisplayMessage.trim().length > 0 ? " : " : "", scope.newDisplayMessage);
                            messageIDFound = true;

                        }
                    }
                }

                if (!messageIDFound) {
                    scope.newDisplayMessage = "";
                    scope.newSeverityValue = "";
                }
                return scope.newDisplayMessage;
            };
            scope.validateMessageID = function () {
                if (scope.propertyname && scope.isvalidate) {
                    var property = scope.propertyname.split('.');
                    property = property[property.length - 1];
                    var list = $ValidationService.getListByPropertyName(scope.messagelist, "MessageID");
                    list.unshift("0"); // deafult message id
                    //$ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "EMPTY_FIELD", CONST.VALIDATION.INVALID_MESSAGE_ID, undefined);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_MESSAGE_ID, undefined, scope.isempty);
                }
                if (scope.onchangecallback) {
                    //message inside model is not updating
                    if (scope.model) {
                        scope.model.messageId = scope.messageid;
                    }
                    scope.onchangecallback();
                }
            };
            //if (scope.disablemessage == undefined || scope.disablemessage == "" || scope.disablemessage == 'False' || scope.disablemessage == 'false') {
            //    scope.validateMessageID();
            //}
        }
    };
}]);

app.directive("tablenameintellisense", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            selecteditem: '=',
            onchangecallback: '&',
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selecteditem}}" ng-model="selecteditem" ng-blur="onchangecallback($event)" ng-keydown="onTableNameKeyDown($event)" /><span class="input-group-addon button-intellisense" ng-click="showTableNameList($event)"></span></div>',
        link: function (scope, element, attributes) {

            scope.tablelist = [];
            hubMain.server.getUnusedTableList().done(function (data) {
                scope.tablelist = data;
            });

            scope.onTableNameKeyDown = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        event.preventDefault();
                    }
                    else if (scope.tablelist) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.tablelist, scope);
                        //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.showTableNameList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.tablelist, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
        },
        //template: 
    };
}]);

app.directive("locationintellisensetemplate", ["$compile", "$rootScope", "ConfigurationFactory", "$ValidationService", function ($compile, $rootScope, ConfigurationFactory, $ValidationService) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            locationname: "=",
            locationchanged: "&",
            islocationdisabled: "=",
            model: "=",
            validate: "=",
            validateLocationCallback: "=",
            errorprop: "=",
            hideCreateButton: "="
        },
        template: "<div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input type='text' id='ScopeId_{{$id}}' title='{{locationname}}' ng-model='locationname' ng-disabled='islocationdisabled' ng-keydown='locationNameTextChanged($event)' ng-keyup='triggerChange($event)' ng-change='locationCallback()' ng-blur='locationCallback();' class='form-control input-sm'/><span class='input-group-addon button-intellisense' ng-disabled='islocationdisabled' ng-click='showlocationNameList($event)'></span><span class='input-group-btn' style='left: 6px;'><img ng-hide='hideCreateButton' ng-disabled='disableCreateNewFolderBtn' src='images/Common/new-folder.svg' style='height: 22px;width: 22px;' ng-click='createNewDirectory()' alt='New Folder' title='New Folder' /></span></div>",
        link: function (scope, element, attributes) {
            scope.folderlist = [];
            scope.disableCreateNewFolderBtn = true;
            scope.receiveList = function (data) {
                scope.$evalAsync(function () {
                    scope.folderlist = data;
                    setSingleLevelAutoComplete(scope.inputElement, scope.folderlist);

                    if (scope.checkValidation) {
                        scope.checkValidation = false;
                        scope.checkAndValidatePath();
                    }
                    else {
                        scope.inputElement.focus();
                        if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        }
                    }
                });
            };
            if (scope.validateLocationCallback) {
                scope.validateLocationCallback = function () {
                    scope.folderlist = [];
                    scope.locationCallback();
                }
            }
            scope.locationCallback = function (isNewFolder) {
                if (isNewFolder != "newFolder") {
                    if (!scope.inputElement || scope.folderlist.length === 0) {
                        scope.inputElement = $(document.getElementById("ScopeId_" + scope.$id));
                        scope.getFolderPath();
                        scope.checkValidation = true;
                    }
                    else {
                        scope.checkAndValidatePath();
                    }
                }
                scope.locationchanged({ locationname: scope.locationname });
            };
            scope.checkAndValidatePath = function () {
                scope.validatePath();
                scope.checkValidPath();
            };
            scope.getFolderPath = function () {
                var basePath = ConfigurationFactory.getLastProjectDetails().BaseDirectory;
                var serachTerm = "";
                if (scope.inputElement) {
                    serachTerm += $(scope.inputElement).val();
                }
                hubMain.server.getFolderList(serachTerm).done(function (data) {
                    scope.$evalAsync(function () {
                        if (data) {
                            scope.receiveList(data);
                        }
                        else {
                            scope.receiveList([]);
                        }
                    });
                });
            };
            scope.locationNameTextChanged = function (event) {
                if (event && (event.keyCode === 38 || event.keyCode === 40)) { // press up or down key    
                    return;
                }
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                    scope.getFolderPath();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.getFolderPath();
                    event.preventDefault();
                }
                else if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && !scope.folderlist) {
                    event.preventDefault();
                }
            };
            scope.triggerChange = function (event) {
                //scope.locationchanged();
                if (scope.inputElement && $(scope.inputElement).val() /* && $(scope.inputElement).val().endsWith("\\") */) {
                    if (event.keyCode !== $.ui.keyCode.SPACE && event.keyCode !== 17 && event.keyCode !== 38 && event.keyCode !== 40) {
                        scope.getFolderPath();
                    }
                }
                scope.locationchanged({ locationname: scope.locationname });
            };
            scope.showlocationNameList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    scope.getFolderPath();
                }
                else if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete') && scope.folderlist && scope.folderlist.length > 0) {
                    scope.inputElement.focus();
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.createNewDirectory = function () {
                var folderName = scope.inputElement && $(scope.inputElement).val();
                $.connection.hubCreateNewObject.server.getNonExistsDirectories(folderName).done(function (data) {
                    if (data) {
                        var yesOrNo = confirm("Do you want to create \n" + data + "\n folders");
                        if (yesOrNo) {
                            $.connection.hubCreateNewObject.server.createNewDirectory(null, folderName).done(function (data) {
                                scope.$evalAsync(function () {
                                    alert("Created " + data.newFolders.toString() + " folders");
                                    if (scope.model && scope.model.errors) {
                                        delete scope.model.errors.invalid_path;
                                    }
                                    scope.locationCallback("newFolder");
                                    scope.disableCreateNewFolderBtn = true;
                                });
                            });
                        }
                    }
                });
            };
            scope.checkValidPath = function () {
                if (scope.validate && scope.model && scope.model.errors && scope.model.errors.invalid_path == "Folder Not Exist" && scope.locationname[scope.locationname.length - 1] != "\\") {
                    scope.disableCreateNewFolderBtn = false;
                }
                else {
                    scope.disableCreateNewFolderBtn = true;
                }
            }

            scope.validatePath = function () {
                if (scope.validate && scope.model) {
                    $ValidationService.checkValidListValue(scope.folderlist, scope.model, $(scope.inputElement).val(), null, "invalid_path", "Folder Not Exist", undefined, false, true, true, true);
                    if (validatePathForSpecialChar(scope.locationname)) {
                        scope.model.errors.invalid_path = 'Invalid Folder Path. It cannot contain any of the following special characters < > / : | * " : and also cannot start or end with \\ ';
                    }
                }
            };
        }
    };
}]);

app.directive("businessobjectintellisensetemplate", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            busobjectname: "=",
            busobjectchange: "&",
            isentitycreation: "=",
            isonlybusobject: "=",
            isbusobjectreadonly: "=",
            onvalidselection: "&",
            model: "=",
            propertyname: "=",
            onblurcallback: "&",
            undorelatedfunction: "=",
        },
        template: "<div class='input-group'><input ng-change='onChange($event)' type='text' id='ScopeId_{{$id}}' ng-blur='onblurcallback($event)' ng-disabled='isbusobjectreadonly' title='{{busobjectname}}' ng-model='busobjectname' ng-keyup='triggerChange($event)' ng-keydown='busObjectNameTextChanged($event)' class='form-control input-sm' undoredodirective model='model' propertyname='propertyname' undorelatedfunction='undorelatedfunction'/><span ng-disabled='isbusobjectreadonly' class='input-group-addon button-intellisense' ng-click='setbusobjectclick($event)'></span></div>",
        link: function (scope, element, attributes) {

            hubMain.server.getLstBusinessObject().done(function (data) {
                scope.busobjectlist = data;
            });


            scope.triggerChange = function (event) {
                scope.busObjectNameTextChanged(event);
            };

            scope.setbusobjectclick = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (!scope.busobjectlist) {
                    hubMain.server.getLstBusinessObject().done(function (data) {
                        scope.busobjectlist = data;
                    });
                }
                if (scope.inputElement && scope.busobjectlist && scope.busobjectlist.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.busObjectNameTextChanged = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    var value = scope.inputElement.val();
                    var businessobjectList = [];
                    if (!scope.busobjectlist) {
                        hubMain.server.getLstBusinessObject().done(function (data) {
                            scope.busobjectlist = data;
                        });
                    }
                    if (scope.busobjectlist && scope.busobjectlist.length > 0) {
                        var data = scope.busobjectlist.sort();
                        //if (scope.isonlybusobject) {
                        //    data = data.filter(function (x) {
                        //        return x.ID.match("^bus");
                        //    });
                        //}
                        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", value);
                            event.preventDefault();
                        } else {
                            if (data && data.length > 100) {
                                for (var i = 0; i < data.length; i++) {
                                    if (businessobjectList.length < 100) {
                                        if (data[i].ID.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                                            businessobjectList.push(data[i]);
                                        }
                                    } else {
                                        break;
                                    }
                                }
                            } else {
                                businessobjectList = data;
                            }
                            setSingleLevelAutoComplete(scope.inputElement, businessobjectList, scope);
                        }
                    }
                }
            };

            scope.onChange = function () {
                if (scope.busobjectlist && scope.busobjectlist.length > 0 && scope.onvalidselection) {
                    var filter = function (item) {
                        return item.ID == scope.busobjectname;
                    };
                    var items = scope.busobjectlist.filter(filter);
                    if (items && items.length > 0) {
                        if (scope.model && scope.propertyname && getPropertyValue(scope.model, scope.propertyname) != scope.busobjectname) {
                            setPropertyValue(scope.model, scope.propertyname, scope.busobjectname);
                        }
                        scope.onvalidselection({ objEntityBusinessObject: scope.busobjectname });
                    }
                }
            };
        },
        //template: 
    };
}]);

app.directive("entityqueryintellisense", ['$compile', '$rootScope', '$getQueryparam', '$EntityIntellisenseFactory', "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $getQueryparam, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            selecteditem: '=',
            propertyname: '=',
            querytype: '=',
            onchangecallback: '&',
            model: '=',
            disableflag: '=',
            parametermodel: '=',
            showsearchbutton: '=',
            isbpm: '=',
            parameterallowedvalues: '=',
            isnavigatetoentity: '=',
            isscalarquery: '=',
            errorprop: '=',
            validate: '=',
            shownonparameterisedquery: '=',
            isinsidedialog: "@",
            blnWithoutParameter: "=",
            isempty: '=',
        },
        template: "<div style='position:relative'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group full-width' ng-show='!isinsidedialog'><input  type='text' title='{{selecteditem}}' ng-model='selecteditem' ng-change='onchange($event)' ng-disabled='disableflag' ng-keyup='onkeyUp($event);validateQuery()' ng-keydown='onKeyDown($event)' class='form-control input-sm' undoredodirective  model='model' propertyname='propertyname' undorelatedfunction ='validateQuery' ng-change='clearParamters()'/><span ng-hide='disableflag' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span><span ng-show='showsearchbutton && !disableflag && !isbpm' class='input-group-addon btn-search-popup' ng-click='onBrowseForQuery()'></span> <span class='input-group-addon' ng-if='!disableflag && isbpm'  title='set Parameters' ng-click='setQueryParameters()'><i class='fa fa-chevron-right'></i></span><button class='input-group-addon' ng-if='model.dictAttributes.sfwLoadType == \"Query\" && isnavigatetoentity'  title={{model.dictAttributes.sfwLoadSource}} ng-click='NavigateToQueryFromBusMethod(model.dictAttributes.sfwLoadSource)' ng-disabled='isDisableNavigateQuery(model.dictAttributes.sfwLoadSource)'><i class='fa fa-chevron-right'></i></button></div><div class='input-group full-width' ng-show='isinsidedialog'><input  type='text' title='{{selecteditem}}' ng-model='selecteditem' ng-change='onchange($event);validateQuery()' ng-disabled='disableflag' ng-keyup='onkeyUp($event);validateQuery()' ng-keydown='onKeyDown($event)' class='form-control input-sm'/><span ng-hide='disableflag' class='input-group-addon button-intellisense' ng-click='showIntellisenseList($event)'></span><span ng-show='showsearchbutton && !disableflag && !isbpm' class='input-group-addon btn-search-popup' ng-click='onBrowseForQuery()'></span> <span class='input-group-addon' ng-if='!disableflag && isbpm'  title='set Parameters' ng-click='setQueryParameters()'><i class='fa fa-chevron-right'></i></span><button class='input-group-addon' ng-if='model.dictAttributes.sfwLoadType == \"Query\" && isnavigatetoentity'  title={{model.dictAttributes.sfwLoadSource}} ng-click='NavigateToQueryFromBusMethod(model.dictAttributes.sfwLoadSource)' ng-disabled='isDisableNavigateQuery(model.dictAttributes.sfwLoadSource)'><i class='fa fa-chevron-right'></i></button></div></div>",
        link: function (scope, element, attributes) {
            if (scope.propertyname) {
                var propertyName = scope.propertyname.split('.');
                scope.property = propertyName[propertyName.length - 1];
            }
            if (scope.querytextboxclass == undefined) scope.querytextboxclass = "col-xs-8";
            scope.setAutocomplete = function (input, data, event) {
                scope.isMultilevelActive = false;
                var propertyName = "ID";
                var arrText = getSplitArray(input.val().trim(), input[0].selectionStart);
                if (arrText[arrText.length - 1] == "") {
                    arrText.pop();
                }
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        var item = data.filter(function (x) {
                            return x.ID == arrText[index];
                        });
                        if (item.length > 0) {
                            if (item[0].Queries) {
                                scope.isMultilevelActive = true;
                                propertyName = "";
                                if (scope.querytype == "SubSelectQuery") {
                                    data = item[0].Queries.filter(function (x) { return x.QueryType == "SubSelectQuery"; });
                                }
                                else if (scope.querytype == "ScalarQuery") {
                                    data = item[0].Queries.filter(function (x) { return x.QueryType == "ScalarQuery"; });
                                }
                                else {
                                    //in normal entity query intellisense sub select query should not be displayed so this check is added
                                    data = item[0].Queries.filter(function (x) { return x.QueryType != "SubSelectQuery"; });
                                    //data = item[0].Queries;
                                }
                                if (scope.shownonparameterisedquery) {
                                    data = data.filter(function (x) { return !x.Parameters || (x.Parameters && x.Parameters.length == 0); });
                                }
                            }
                            else if ($(input).data('ui-autocomplete') != undefined) {
                                $(input).autocomplete("disable");
                            }
                        }
                        // first level value is not valid -- second level is disabled
                        else if ($(input).data('ui-autocomplete') != undefined) {
                            $(input).autocomplete("disable");
                        }
                    }
                }
                setEntityQueryIntellisense(input, data, scope, propertyName);
            };
            scope.onkeyUp = function (event) {
                // this enables the second level autocomplete
                if (scope.inputElement) {
                    var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                    if ((arrText.length > 1 && !scope.isMultilevelActive) || arrText.length == 0) {
                        scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                    } else {
                        if (arrText.length > 2) {
                            if ($(scope.inputElement).data('ui-autocomplete')) {
                                $(scope.inputElement).autocomplete("disable");
                            }
                        }
                    }
                }
            };
            scope.onKeyDown = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    scope.setAutocomplete(scope.inputElement, entityIntellisenseList, event);
                }
                var IsautocompleteSet = scope.inputElement.data('ui-autocomplete') != undefined;
                var Isdisabled = IsautocompleteSet ? $(scope.inputElement).autocomplete("option", "disabled") : false;
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    event.preventDefault();
                } else if (IsautocompleteSet && Isdisabled && !event.ctrlKey) {
                    $(scope.inputElement).autocomplete("enable");
                }

                var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                // if user goes back from second level to first level intellisense
                if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
            };
            scope.showIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                    scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                }
                scope.inputElement.focus();
                var arrText = getSplitArray(scope.inputElement.val(), scope.inputElement[0].selectionStart);
                if (arrText.length > 1) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                // switch back from multilevel to single level  
                else if (arrText.length == 1 && scope.isMultilevelActive) scope.setAutocomplete(scope.inputElement, $EntityIntellisenseFactory.getEntityIntellisense(), event);
                if ($(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                }
            };
            scope.onchange = function () {

                var aQueryParamColl = $getQueryparam.get(scope.selecteditem);
                if (scope.selecteditem != undefined && attributes.parametermodel && aQueryParamColl) {
                    if (scope.isinsidedialog) {
                        scope.parametermodel = aQueryParamColl;
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.parametermodel, scope, 'parametermodel', aQueryParamColl);
                    }
                }
                scope.validateQuery();

                if (scope.onchangecallback) {
                    if (scope.selecteditem != undefined) {
                        if (scope.model) {
                            if (scope.propertyname && !scope.tempModel) {
                                scope.tempModel = "model";
                                var properties = scope.propertyname.split(".");
                                for (var i = 0, l = properties.length; i < l - 1; i++) {
                                    scope.tempModel = scope.tempModel.concat("." + properties[i]);
                                }
                            }
                            if (scope.tempModel) {
                                var properties = scope.propertyname.split(".");
                                //var t = eval(scope.tempModel);
                                var t;
                                var larrProps = scope.tempModel.split(".");
                                var ltmpObject = scope;
                                for (var i = 0, len = larrProps.length; i < len; i++) {
                                    ltmpObject = ltmpObject[larrProps[i]];
                                }
                                if (ltmpObject !== null) {
                                    t = ltmpObject;
                                }
                                t[properties[properties.length - 1]] = scope.selecteditem;
                            }
                            scope.onchangecallback({ QueryID: scope.selecteditem });
                        } else {
                            scope.onchangecallback({ QueryID: scope.selecteditem });
                        }
                    }
                }
            };
            scope.onBrowseForQuery = function () {
                var newScope = scope.$new();
                newScope.$on('onQueryClickBPM', function (event, data) {
                    // pass true for setting query params after getting the response 
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        if (scope.isinsidedialog) {
                            newScope.mapObj.dictAttributes.sfwQueryID = data.Id;
                        }
                        else {
                            $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryID, newScope.mapObj.dictAttributes, "sfwQueryID", data.Id);
                        }
                    }
                    else if (newScope.mapObj) {
                        if (scope.isinsidedialog) {
                            newScope.mapObj.sfwQueryID = data.Id;
                        }
                        else {
                            $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryID, newScope.mapObj, "sfwQueryID", data.Id);
                        }
                    }

                    function iterator(value, key) {
                        queryParameters.push(value);
                        this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                    }
                    // if query parameters need to be set
                    if (data.Parameters.length > 0) {
                        var queryParametersDisplay = [];
                        var queryParameters = [];

                        angular.forEach(data.Parameters, iterator, queryParametersDisplay);
                        if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryParameters, newScope.mapObj.dictAttributes, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        else if (newScope.mapObj) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryParameters, newScope.mapObj, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                    }
                });
                newScope.$on('onQueryClick', function (event, data) {
                    if (scope.isinsidedialog) {
                        scope.selecteditem = data;
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.selecteditem, scope, "selecteditem", data); //Fixed Bug 10157:Dirty mark for unsaved changes is not displayed on adding Autocomplete or retrieval query
                    }
                    //if (scope.propertyname) {
                    //    $rootScope.EditPropertyValue(scope.selecteditem, scope, "selecteditem", data);
                    //}
                    //else {
                    //    scope.selecteditem = data;
                    //}
                    scope.onchange(event);
                    if (event) {
                        event.stopPropagation();
                    }
                });
                newScope.mapObj = scope.model;
                newScope.strSelectedQuery = scope.selecteditem;
                newScope.IsBPM = scope.isbpm;
                newScope.subQueryType = scope.querytype;
                if (scope.isbpm) {
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj.dictAttributes, "sfwQueryParameters", ";");
                    }
                    else if (newScope.mapObj) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj, "sfwQueryParameters", ";");
                    }
                }

                newScope.queryValues = scope.parameterallowedvalues;
                newScope.validateQuery = scope.validateQuery;
                newScope.QueryDialog = $rootScope.showDialog(newScope, "Browse Queries", "Form/views/BrowseForQuery.html", {
                    width: 1000, height: 700
                });
            };

            scope.setQueryParameters = function () {
                if (scope.isbpm) {
                    var newScope = scope.$new();
                    newScope.mapObj = scope.model;
                    newScope.queryValues = scope.parameterallowedvalues;
                    // for bpm for setting query parameter
                    if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj.dictAttributes, "sfwQueryParameters", ";");
                    }
                    else if (newScope.mapObj) {
                        newScope.queryParameters = $getQueryparam.getQueryparamfromString(newScope.mapObj, "sfwQueryParameters", ";");
                    }

                    newScope.onOKClickQueryParameters = function () {
                        var queryParametersDisplay = [];
                        var queryParameters = [];
                        function iterator(value, key) {
                            queryParameters.push(value);
                            this.push(value.ID + '=' + (value.Value ? value.Value : ""));
                        }
                        angular.forEach(newScope.queryParameters, iterator, queryParametersDisplay);
                        if (newScope.mapObj && newScope.mapObj.dictAttributes) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.dictAttributes.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.dictAttributes.sfwQueryParameters, newScope.mapObj.dictAttributes, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        else if (newScope.mapObj) {
                            if (scope.isinsidedialog) {
                                newScope.mapObj.sfwQueryParameters = queryParametersDisplay.join(";") + ";";
                            }
                            else {
                                $rootScope.EditPropertyValue(newScope.mapObj.sfwQueryParameters, newScope.mapObj, "sfwQueryParameters", queryParametersDisplay.join(";") + ";");
                            }
                        }
                        newScope.parametersdialog.close();
                    };
                    newScope.parametersdialog = $rootScope.showDialog(newScope, "Set Parameters", "BPM/views/SetQueryParameters.html", {
                        width: 500, height: 500
                    });
                }
            };


            scope.NavigateToQueryFromBusMethod = function (loadSource) {
                var index = loadSource.indexOf(".");
                if (index > -1) {
                    var entityName = loadSource.substring(0, index);
                    var indexOfEntity = null;
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    for (var i = 0; i < entityIntellisenseList.length; i++) {
                        if (entityIntellisenseList[i].ID == entityName) {
                            indexOfEntity = i;
                            break;
                        }
                    }
                    var queryName = loadSource.substring(index + 1, loadSource.length);
                    $rootScope.queryID = queryName;
                    var entityScope;
                    for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                        if ($rootScope.lstopenedfiles[i].file.FileName == entityName) {
                            entityScope = getScopeByFileName(entityName);
                            if (entityScope != undefined) {
                                $.connection.hubMain.server.navigateToFile(entityName, "").done(function (objfile) {
                                    $rootScope.openFile(objfile, false);
                                });

                                entityScope.openQueryFromLogicalRule();
                                break;
                            }
                        }
                    }
                    if (entityScope == undefined) {
                        $.connection.hubMain.server.navigateToFile(entityName, "").done(function (objfile) {
                            $rootScope.openFile(objfile, false);
                        });

                    }
                }
            };

            scope.isDisableNavigateQuery = function (loadSource) {
                scope.isscalarquery = false;
                if (loadSource == undefined || loadSource == "") {
                    return true;
                }
                if (loadSource != undefined) {
                    scope.isscalarquery = true;
                    var index = loadSource.indexOf(".");
                    var flag = true;
                    if (index > -1) {
                        var entityName = loadSource.substring(0, index);
                        var indexOfEntity = null;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        for (var i = 0; i < entityIntellisenseList.length; i++) {
                            if (entityIntellisenseList[i].ID == entityName) {
                                indexOfEntity = i;
                                break;
                            }
                        }
                        var queryName = loadSource.substring(index + 1, loadSource.length);
                        if (queryName != undefined && queryName != "" && indexOfEntity) {
                            for (var i = 0; i < entityIntellisenseList[indexOfEntity].Queries.length; i++) {
                                if (entityIntellisenseList[indexOfEntity].Queries[i].ID == queryName) {
                                    if (entityIntellisenseList[indexOfEntity].Queries[i].QueryType == "ScalarQuery") {
                                        scope.isscalarquery = false;
                                    }
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                    return flag;
                }
                return false;
            };
            scope.validateQuery = function () {
                if (scope.property && scope.validate) {
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else
                        if (scope.model && scope.model.dictAttributes) {
                            input = scope.model.dictAttributes[scope.property];
                        }
                    var queryPropList = ["sfwQueryID", "sfwLoadSource", "sfwRetrievalQuery", "sfwAutoQuery", "sfwBaseQuery", "sfwQueryRef", "StrBaseQuery", "loadSource"];
                    var errorMessage = "";
                    if (queryPropList.indexOf(scope.property) > -1) {
                        errorMessage = CONST.VALIDATION.INVALID_QUERY;
                    } else if (scope.property == "sfwCodeTable") {
                        errorMessage = CONST.VALIDATION.INVALID_CODE_TABLE;
                    }
                    $ValidationService.checkValidQuery($EntityIntellisenseFactory.getEntityIntellisense(), scope.model, input, scope.querytype, scope.property, scope.property, errorMessage, undefined, scope.blnWithoutParameter, scope.isempty);
                }
            };

        }
    };
}]);

app.directive("codegroupintellisensetemplate", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {

    var getTemplate = function () {
        var template = ""; //<div class='form-group' >

        template = [" <label class='{{codegrouplabelclass}}' id='ScopeId_{{$id}}' ng-show='showcodegrouptext'>{{codegrouptext}}</label>",
            "<div style='position: relative;' class='{{codegrouptextboxclass}}'>",
            '<span class="info-tooltip dtc-span" ng-if="model.errors.invalid_code_group" ng-attr-title="{{model.errors.invalid_code_group}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>',
            "<div class='input-group'>",
            "<input type='text' ng-class=\"insidegrid?'form-control input-sm form-filter input-sm':'form-control input-sm'\" title='{{codegroupid?codegroupid:model.placeHolder}}' ng-model='codegroupid' ng-keydown='codegroupIDTextChanged($event)' ng-keyup='validateCodeGroup($event)' ng-init='validateCodeGroup()' ng-change='validateCodeGroup()' ng-blur='onblurcallback()' ondrop='return false'  undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateCodeGroup' class='form-control input-sm' placeholder='{{model.placeHolder}}'/> ",
            "<span class='input-group-addon button-intellisense' ng-click='showCodeGroupIDList($event)'></span>",
            "</div>",
            "</div>"];
        template = template.join(' ');

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            codegroupid: "=",
            propertyname: "=",
            insidegrid: "=",
            showcodegrouptext: '=',
            codegrouptext: '=',
            codegrouplabelclass: "@",
            codegrouptextboxclass: "@",
            onblurcallback: '&',
            isDialog: '='
        },
        link: function (scope, element, attrs) {

            if (scope.codegrouplabelclass == undefined || scope.codegrouplabelclass == "") {
                scope.codegrouplabelclass = "col-xs-4 control-label";
            }
            if (scope.codegrouptextboxclass == undefined || scope.codegrouptextboxclass == "") {
                scope.codegrouptextboxclass = "col-xs-8";
            }

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            // var newScope;
            scope.codegrouplist = [];
            scope.receiveList = function (data, inputElement) {
                if (data && data.length > 0) {
                    scope.codegrouplist = data;
                    setSingleLevelAutoComplete(inputElement, scope.codegrouplist, scope, "CodeID", "CodeIDDescription");
                    if (inputElement) {
                        $(inputElement).autocomplete("search", $(inputElement).val());
                    }
                }
            };

            scope.getCodeGroups = function (inputEle) {
                var inputElementForCodeGroup = inputEle;
                $.connection.hubMain.server.getCodeGroups().done(function (data) {
                    scope.$evalAsync(function () {
                        scope.receiveList(data, inputElementForCodeGroup);
                    });
                });
            };

            scope.codegroupIDTextChanged = function (event) {
                if (event && event.keyCode == $.ui.keyCode.TAB) { //  when user press tab key
                    return;
                }
                var input = $(event.target);
                scope.inputElement = input;
                var value = input.val();

                if (scope.codegrouplist.length == 0) {
                    scope.getCodeGroups(input);
                }
                else {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                }
            };

            scope.showCodeGroupIDList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                if (scope.codegrouplist.length == 0) {
                    scope.getCodeGroups(scope.inputElement);
                    scope.inputElement.focus();
                }
                else {
                    scope.inputElement.focus();
                    if (scope.inputElement && scope.codegrouplist) {
                        if ($(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        }
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.validateCodeGroup = function () {
                if (event && event.keyCode == $.ui.keyCode.TAB) { //  when user press tab key
                    return;
                }
                if (scope.propertyname && scope.codegrouplist && scope.codegrouplist.length > 0) {
                    var property = scope.propertyname.split('.')[1];
                    var errorMessage = CONST.VALIDATION.CODE_GROUP_NOT_EXISTS;
                    if (property == "sfwChecklistId") errorMessage = CONST.VALIDATION.CHECKLIST_NOT_EXISTS;
                    var list = $ValidationService.getListByPropertyName(scope.codegrouplist, "CodeID");
                    list.push("0");
                    var isEmpty = false;
                    if (["sfwDropDownList", "sfwCascadingDropDownList", "sfwMultiSelectDropDownList", "sfwListPicker", "sfwListBox", "sfwRadioButtonList", "sfwCheckBoxList", "entity"].indexOf(scope.model.Name) > -1) {
                        var prop = "sfwEntityField";
                        if (scope.model.Name == "sfwCheckBoxList") prop = "sfwCheckBoxField";
                        if (scope.model.dictAttributes[prop] && scope.model.errors && !scope.model.errors[prop]) {
                            if (scope.model.dictAttributes.sfwLoadType == "CodeGroup" && !(scope.model.placeHolder || $(scope.inputElement).val())) {
                                isEmpty = true;
                            }
                        }
                    }
                    //if (!$(scope.inputElement).val()){
                    //    isEmpty = true;
                    //}
                    if (isEmpty) {
                        $ValidationService.checkValidListValue([], scope.model, $(scope.inputElement).val(), "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, undefined, isEmpty);
                    } else {
                        $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_code_group", errorMessage, undefined, isEmpty, scope.isDialog);
                    }
                }
            };

        }
    };
}]);

app.directive("codevaluesintellisensetemplate", ["$compile", "$rootScope", "ngDialog", function ($compile, $rootScope, ngDialog) {

    var getTemplate = function () {
        var template = ""; //<div class='form-group' >

        template = ["<label class='{{codevalueslabelclass}}' id='ScopeId_{{$id}}' ng-show='showcodevaluestext'>{{codevaluestext}}</label>",
            "<div class='{{codevaluestextboxclass}}'>",
            "<div class='input-group'>",
            "<input type='text' class='form-control input-sm text-autocomplete' title='{{codevaluesid}}' ng-model='codevaluesid' onkeydown='codevaluesIDTextChanged(event)' ng-blur='codevaluesidchange($event)' ng-change='codevaluesidchange($event)' undoredodirective model='model' propertyname='propertyname' class='form-control input-sm'/>",
            "<span class='button-intellisense input-group-addon' value=''  ng-click='showCheckListIDList($event)'></span>",

            "</div>",
            "</div>"];
        template = template.join(' ');

        return template;
    };

    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: "=",
            codevaluesid: "=",
            propertyname: "=",
            showcodevaluestext: '=',
            codevaluestext: '=',
            codevalueslabelclass: "@",
            codevaluestextboxclass: "@",
            codedescription: '=',
            codeid: "=",
        },
        link: function (scope, element, attrs) {

            if (scope.codevalueslabelclass == undefined || scope.codevalueslabelclass == "") {
                scope.codevalueslabelclass = "col-xs-4 control-label";
            }
            if (scope.codevaluestextboxclass == undefined || scope.codevaluestextboxclass == "") {
                scope.codevaluestextboxclass = "col-xs-8";
            }

            var parent = $(element).parent();
            parent.html(getTemplate());
            $compile(parent.contents())(scope);
            var newScope;
            scope.$watch('codeid', function (newVal, oldVal) {
                scope.codevalueslist = [];
                if (newVal) {
                    $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
                }
            });

            scope.codevaluesidchange = function (event) {
                function iterator(item) {
                    if (item.CodeValue == scope.codevaluesid) {
                        scope.codedescription = item.Description;
                    }
                }
                scope.codedescription = "";
                if (!scope.codeid) {
                    scope.codevalueslist = [];
                }
                if (scope.codevalueslist && scope.codevalueslist.length > 0) {

                    angular.forEach(scope.codevalueslist, iterator);
                }
            };

            scope.receiveList = function (data) {
                scope.codevalueslist = data;
                if (newScope) {
                    newScope.codevalueslist = scope.codevalueslist;
                }
                if (scope.codevaluesid && scope.codevalueslist && scope.codevalueslist.length > 0) {
                    scope.codevaluesidchange();
                }
            };


            scope.showCheckListIDList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                if (!scope.codevalueslist) {
                    $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.codevalueslist) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.codevalueslist, scope, "CodeValue", "CodeValueDescription");
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.$watch("codevaluesid", function (newVal, oldVal) {
                scope.codevaluesidchange();
            });
        }
    };
}]);

app.directive("busobjectintellisensetemplate", ["$compile", function ($compile) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            filename: "=",
            propertyname: '=',
            filetype: '=',
            onfilechange: '&',
            isdisabled: '=',
            onfileblur: '&'
        },
        template: "<div class='input-group'><input class='form-control input-sm' type='text' ng-blur='onblur()' ng-disabled='isdisabled' id='ScopeId_{{$id}}' title='{{filename}}' ng-model='filename' ng-keydown='onkeydown($event)' ng-change='onchangeCallback()' undoredodirective  model='model' propertyname='propertyname'/><span class='input-group-addon button-intellisense' ng-click='showBusObjectIntellisenseList($event)'></span></div>",
        link: function (scope, element, attrs) {


            scope.receiveList = function (data) {
                scope.busobjectlist = data;
                if (scope.busobjectlist) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope, "Description", "Description");
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.onchangeCallback = function () {
                var busobject;
                if (scope.busobjectlist) {
                    busobject = scope.busobjectlist.filter(function (x) { return x.Description == scope.filename; });
                }
                if (busobject && busobject.length > 0) {
                    scope.model.IsChildOfInboundFileBase = busobject[0].IsChildOfInboundFileBase;
                    scope.model.lstObjectMethod = busobject[0].LstObjectMethods;
                    scope.model.lstFileCollection = busobject[0].LstFieldNames;
                    scope.model.BusinessObject = busobject;
                }
                else {
                    scope.model.IsChildOfInboundFileBase = false;
                    scope.model.lstObjectMethod = [];
                    scope.model.lstFileCollection = [];
                    scope.model.BusinessObject = [];
                }
                if (scope.onfilechange) {
                    scope.onfilechange();
                }
            };
            scope.onblur = function () {
                if (scope.onfileblur) {
                    scope.onfileblur();
                }
            };
            scope.showBusObjectIntellisenseList = function (eargs) {
                if (!scope.busobjectlist) {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target).prevAll("input[type='text']");
                    }
                    $.connection.hubMain.server.getBusObjectsByType(scope.filetype).done(function (result) {
                        var result = JSON.parse(result);
                        scope.receiveList(result);
                    });
                }
                else {
                    scope.inputElement.focus();
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.onkeydown = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (!scope.busobjectlist) {
                    $.connection.hubMain.server.getBusObjectsByType(scope.filetype).done(function (result) {
                        var result = JSON.parse(result);
                        if (event && event.keyCode != $.ui.keyCode.ESCAPE) {
                            scope.receiveList(result);
                        }
                    });
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        event.preventDefault();
                    }
                }
                else {
                    //setSingleLevelAutoComplete(scope.inputElement, scope.busobjectlist, scope, "Description", "Description");
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                        event.preventDefault();
                    }
                }
            };
        }
    };
}]);

app.directive("entitycolumnintellisense", ['$rootScope', '$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($rootScope, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            formodel: '=',
            modebinding: "=",
            entityid: "=",
            propertyname: '=',
            queryid: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: "=",
            isshowcolumnvalues: "=",
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_data_field' ng-attr-title='{{model.errors.invalid_data_field}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' ng-keydown='filterIntelliseneList($event)' ng-keyup='validateDataField()' ng-change='validateDataField()'/><span class='input-group-addon button-intellisense' ng-click='showEntityColumnIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.$watch('entityid', function (newVal, oldVal) {
                if (newVal) {
                    scope.lstEntityColumnList = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    scope.lstEntityColumnList = getEntityAttributeByType(entityIntellisenseList, scope.entityid, "Column");
                }
            });

            scope.$watch('queryid', function (newVal, oldVal) {
                scope.PopulateColumnList(newVal);
            });

            scope.PopulateColumnList = function (queryid) {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var result = PopulateColumnList(queryid, scope.formodel, entityIntellisenseList, scope.lstloadedentitycolumnstree);
                if (result) {
                    scope.lstColumnList = result.list;
                    scope.attributeName = result.attribute;
                }
                /*      if (queryid) {
                          scope.PopulateQueryColumnFromList(queryid);
                      }
                      else {
                          var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                          if (scope.formodel) {
                              var MainQuery = GetMainQueryFromFormObject(scope.formodel, entityIntellisenseList);
                              if (MainQuery) {
                                  scope.PopulateQueryColumnFromList(MainQuery.dictAttributes.ID);
                              }
                              else {
                                  scope.attributeName = "Value";
                                  if (scope.formodel.dictAttributes.sfwEntity) {
                                      var entities = entityIntellisenseList;
                                      var entity = entities.filter(function (x) {
                                          return x.ID == scope.formodel.dictAttributes.sfwEntity;
                                      });
                                      if (entity.length > 0) {
                                          var attributes = entity[0].Attributes;
                                          scope.lstColumnList = attributes.filter(function (itm) { return itm.Type == "Column" });
                                      }
                                  }
                              }
                          }
                      }
                      */
            };

            scope.PopulateQueryColumnFromList = function (queryid) {
                var blnFound = false;
                if (scope.lstloadedentitycolumnstree) {

                    var lst = scope.lstloadedentitycolumnstree.filter(function (itm) {
                        return itm.EntityName == queryid;
                    }
                    );
                    if (lst && lst.length > 0) {

                        scope.lstColumnList = JSON.parse(JSON.stringify(lst[0].lstselectedobjecttreefields));
                        blnFound = true;
                    }
                }

                if (!blnFound) {
                    var lstQueryID = GetQueryListFromObject(scope.formodel);
                    if (lstQueryID) {

                        var lst = lstQueryID.filter(function (itm) {
                            return itm.dictAttributes.ID == queryid;
                        });
                        if (lst && lst.length > 0) {
                            var objnew = { EntityName: lst[0].dictAttributes.ID, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: true };
                            if (scope.lstloadedentitycolumnstree) {
                                scope.lstloadedentitycolumnstree.push(objnew);
                                if (lst[0].dictAttributes.sfwQueryRef) {
                                    $.connection.hubForm.server.getEntityQueryColumns(lst[0].dictAttributes.sfwQueryRef, "LoadQueryFieldsForLookup").done(function (data) {
                                        var scope = GetFormFileScope(scope.formodel);
                                        if (scope && scope.receiveQueryFields) {
                                            scope.receiveQueryFields(data, lst[0].dictAttributes.sfwQueryRef);
                                        }
                                    });
                                }
                                blnFound = true;
                            }
                            if (!objnew.lstselectedobjecttreefields) {
                                objnew.lstselectedobjecttreefields = [];
                            }
                            scope.lstColumnList = objnew.lstselectedobjecttreefields;
                        }
                    }
                }
            };

            scope.filterIntelliseneList = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        //setAutoComplete(scope.inputElement);
                        if ($(scope.inputElement).data('ui-autocomplete') && (scope.lstEntityColumnList || scope.lstColumnList)) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        setAutoComplete(scope.inputElement);
                    }

                    //event.preventDefault();
                    //if (event.stopPropagation) {
                    //    event.stopPropagation();
                    //}
                }

            };

            scope.showEntityColumnIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                setAutoComplete(inputElement);
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };

            var setAutoComplete = function (input) {
                if (scope.entityid != undefined && scope.entityid != "") {
                    scope.attributeName = "ID";
                    if (scope.isshowcolumnvalues) {
                        scope.attributeName = "Value";
                    }
                    setSingleLevelAutoComplete(input, scope.lstEntityColumnList, scope, scope.attributeName, scope.attributeName);
                    //$(input).autocomplete("search", $(input).val());
                }
                else if (scope.lstColumnList) {
                    scope.PopulateColumnList(scope.queryid);
                    setSingleLevelAutoComplete(input, scope.lstColumnList, scope, scope.attributeName, scope.attributeName);
                    //$(input).autocomplete("search", $(input).val());

                }
            };

            scope.validateDataField = function () {
                var property = "sfwDataField";
                var list = [];
                /*    if (scope.entityid) {
                        list = scope.lstEntityColumnList;
                    }
                    else if (scope.lstColumnList) {
                        list = scope.lstColumnList;
                    }
                    $ValidationService.checkDataFieldValue(list, scope.model, $(scope.inputElement).val(), scope.attributeName, property, 'invalid_data_field', CONST.VALIDATION.INVALID_DATA_FIELD, undefined);
                */
            };
        }
    };
}]);

app.directive("entityexpressionintellisense", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modelvalue: "=",
            entityid: "=",
            propertyname: "="
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_expression' ng-attr-title='{{model.errors.invalid_expression}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateExpression' ng-keydown='getEntityExpressionList($event)' ng-keyup='validateExpression()' ng-change='validateExpression()'/>	<span class='input-group-addon button-intellisense' ng-click='showExpressionIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {
            scope.createExpressionList = function () {
                scope.lstEntityExpressionList = [];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                scope.lstEntityExpressionList = getEntityAttributeByType(entityIntellisenseList, scope.entityid, "Expression");

            };

            scope.getEntityExpressionList = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                scope.createExpressionList();
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                } else {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(input, scope.lstEntityExpressionList, scope);
                    }
                }
            };

            scope.showExpressionIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                scope.createExpressionList();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.lstEntityExpressionList, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.validateExpression = function () {
                if (scope.propertyname) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstEntityExpressionList, "ID", false);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, "invalid_expression", CONST.VALIDATION.INVALID_EXPRESSION, undefined);
                }
            };
        }
    };
}]);

app.directive("entitymethodsintellisense", ['$rootScope', '$filter', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', '$Entityintellisenseservice', function ($rootScope, $filter, $ValidationService, CONST, $NavigateToFileService, $Entityintellisenseservice) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            entityid: "=",
            modelvalue: "=",
            propertyname: "=",
            showonlycollection: "=",
            showonlyobject: "=",
            showonlyobjectmethods: "=",
            showrules: "=",
            showxmlmethods: "=",
            errorprop: "=",
            validate: "=",
            methodtext: "=",
            onchangecallback: '&',
        },
        template: "<div><label class='control-label col-xs-5' ng-if='methodtext && (!modelvalue || !modelvalue.trim())'>{{methodtext}}</label><label class='control-label col-xs-5' ng-if='methodtext && modelvalue && modelvalue.trim()'><a ng-click='navigateToMethodClick(modelvalue)'>{{methodtext}}</a></label><div ng-class='methodtext?\"col-xs-7\":\"col-xs-12\" '><div class='input-group'><span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ondrop='return false' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateMethod' ng-keydown='getEntityMethodsList($event)' ng-keyup='validateMethod()' ng-change='validateMethod();onChange()'/>	<span class='input-group-addon button-intellisense' ng-click='showEntityMethodsIntellisenseList($event)'></span></div></div></div>",
        link: function (scope, element, attributes) {
            scope.createMethodsList = function () {
                var lst = null;
                scope.lstMethods = [];

                if (scope.model && scope.model.dictAttributes.sfwExecuteMethodType) {
                    scope.showrules = false;
                    scope.showonlyobjectmethods = false;
                    scope.showxmlmethods = false;

                    if (scope.model.dictAttributes.sfwExecuteMethodType == "Rule") {
                        scope.showrules = true;
                    }
                    else if (scope.model.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                        scope.showonlyobjectmethods = true;
                    }
                    else if (scope.model.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                        scope.showxmlmethods = true;
                    }
                }
                if (scope.showrules) {
                    var tempRuleList = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, false, true, false, false);
                    if (tempRuleList && tempRuleList.length > 0) {
                        lst = tempRuleList.filter(function (itm) { return !itm.IsPrivate && !itm.IsStatic && itm.Status == "Active" });
                    }
                }
                else if (scope.showonlyobjectmethods) {
                    var lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, true, false, false, false);

                }
                else if (scope.showxmlmethods) {
                    var lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, false, false, false, true);

                }
                else {
                    lst = $Entityintellisenseservice.GetIntellisenseData(scope.entityid, "", "", true, false, true, false, false, false);
                }

                if (lst && lst.length > 0) {
                    if (scope.showonlycollection) {
                        lst = $filter("filter")(lst, { ReturnType: "Collection" });
                        if (lst && lst.length > 0) {
                            scope.lstMethods = lst;
                        }
                    }
                    else if (scope.showonlyobject) {
                        lst = $filter("filter")(lst, { ReturnType: "Object" });
                        if (lst && lst.length > 0) {
                            scope.lstMethods = lst;
                        }
                    }
                    else {
                        angular.forEach(lst, function (method) {
                            if (method && method.ID) {
                                scope.lstMethods.push(method);
                            }
                        });
                    }
                }
            };

            scope.getEntityMethodsList = function (event) {
                var input = $(event.target);
                scope.inputElement = input;
                scope.createMethodsList();
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    if (scope.entityid && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    }
                    event.preventDefault();
                } else {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(input, scope.lstMethods, scope);
                    }
                }
            };

            scope.showEntityMethodsIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                scope.createMethodsList();
                if (scope.inputElement) {
                    if (scope.entityid) {
                        setSingleLevelAutoComplete(scope.inputElement, scope.lstMethods, scope);
                        if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                    }

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.onChange = function () {
                if (scope.onchangecallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.modelvalue);
                    }
                    scope.onchangecallback();
                }
            };

            scope.validateMethod = function () {
                if (scope.propertyname && scope.validate) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    var input;
                    if (scope.inputElement) input = $(scope.inputElement).val();
                    else input = scope.model.dictAttributes[property];
                    var list = $ValidationService.getListByPropertyName(scope.lstMethods, "ID", false);
                    var errorProp = "";
                    if (property && (property == "sfwXmlMethod" || property == "sfwLoadSource" || property == "sfwObjectMethod")) {
                        errorProp = "invalid_method";
                    } else if (property == "sfwRetrievalMethod") {
                        errorProp = "invalid_retrieval_method";
                    }
                    if (input) {
                        $ValidationService.checkValidListValue(list, scope.model, input, property, errorProp, CONST.VALIDATION.INVALID_METHOD, undefined);
                    } else {
                        $ValidationService.checkValidListValue(["null"], scope.model, "null", property, errorProp, CONST.VALIDATION.INVALID_METHOD, undefined);
                    }
                }
            };

            scope.navigateToMethodClick = function (aMethodID) {
                if (!scope.lstMethods) {
                    scope.createMethodsList();
                }
                scope.validateMethod();
                var errorProp = "";
                if (scope.propertyname) {
                    var propertyName = scope.propertyname.split('.');
                    var property = propertyName[propertyName.length - 1];
                    if (property && (property == "sfwXmlMethod" || property == "sfwLoadSource")) {
                        errorProp = "invalid_method";
                    } else if (property == "sfwRetrievalMethod") {
                        errorProp = "invalid_retrieval_method";
                    }
                }
                var IsNavigate = false;
                if (!scope.model.errors) {
                    IsNavigate = true;
                }
                else if (scope.model.errors && !scope.model.errors[errorProp]) {
                    IsNavigate = true;
                }
                if (aMethodID && scope.entityid && IsNavigate) {
                    if (scope.lstMethods && scope.lstMethods.length > 0) {
                        if (scope.model.dictAttributes && scope.model.dictAttributes.sfwExecuteMethodType) {
                            if (scope.model.dictAttributes.sfwExecuteMethodType == "Rule") {
                                $NavigateToFileService.NavigateToRule(aMethodID);
                            }
                            else if (scope.model.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                                $NavigateToFileService.NavigateToFile(scope.entityid, "objectmethods", aMethodID);
                            }

                            else if (scope.model.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                                $NavigateToFileService.NavigateToFile(scope.entityid, "methods", aMethodID);
                            }
                        }
                        else {
                            $NavigateToFileService.NavigateToFile(scope.entityid, "objectmethods", aMethodID);
                        }
                    }

                }
            };
        }
    };
}]);

app.directive("objectfieldintellisense", [function () {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            modelvalue: "=",
            parameterlist: "=",
            propertyname: "="
        },
        template: "<div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' ng-keydown='getObjectFieldList($event)'/>	<span class='input-group-addon button-intellisense' ng-click='showObjectFieldIntellisenseList($event)'></span></div>",
        link: function (scope, element, attributes) {

            var filterParameterList = function () {
                if (scope.parameterlist && scope.parameterlist.length > 0) {
                    scope.paramNameList = [];
                    angular.forEach(scope.parameterlist, function (value, key) {
                        if ("ParamName" in value) {
                            scope.paramNameList.push(value.ParamName);
                        }
                    });
                }
            };
            scope.getObjectFieldList = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (scope.paramNameList && scope.paramNameList.length > 0 && $(scope.inputElement).data('ui-autocomplete')) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        if (scope.paramNameList && scope.paramNameList.length > 0) {
                            setSingleLevelAutoComplete(scope.inputElement, scope.paramNameList, scope);
                        }
                    }

                    //event.preventDefault();
                    if (!(event.ctrlKey && (event.keyCode === 83 || event.keyCode === 90 || event.keyCode === 89)) && event.stopPropagation) { // added "ctrl+S" condition coz while doing ctrl+s it was opening webpage save dialog rather saving the file .(by shilpi)
                        event.stopPropagation();
                    }
                }
            };

            scope.showObjectFieldIntellisenseList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement && scope.paramNameList && scope.paramNameList.length > 0) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.paramNameList, scope);
                    if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());

                }
                if (event) {
                    event.stopPropagation();
                }
            };
            filterParameterList();
        }
    };
}]);

app.directive("commonIntellisense", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            selectedItem: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            updateDataCallback: '&',
            onblurcallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            isReset: "=",
            errorprop: "=",
            validate: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    if (scope.updateDataCallback) {
                        scope.updateDataCallback();
                    }
                    scope.initializeIntellisense();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedItem);
                    }
                    scope.changeCallback();
                }
                scope.validateInput();
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                    if (property == "sfwEntityField") errorMessage = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    //if (!$(scope.inputElement).val()) {
                    //    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined,true);
                    //}
                    //else {
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined);
                }
                //}
            };
        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedItem}}" ng-model="selectedItem" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)" ng-blur="onblurcallback()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateInput"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);
app.directive("ruleIntellisense", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", "$EntityIntellisenseFactory", function ($compile, $rootScope, $ValidationService, CONST, $EntityIntellisenseFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            entityId: "=",
            nonStatic: "=",
            returnTypes: "=",
            ruleTypes: "=",
            selectedRule: '=',
            changeCallback: '&',
            onblurcallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            vErrorprop: "=",
            vValidate: "=",
            vAllowEmpty: "=",
            vFieldName: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (eargs) {
                var input = eargs.target;

                var arrText = getSplitArray(input.value, input.selectionStart);

                //Get all the entities which have static rule of the specified rule type.
                var requiredEntities = [];
                if (!scope.nonStatic) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    requiredEntities = entities.filter(function (x) { return x.Rules.some(function (element, index, array) { return element.IsStatic && ((scope.entityId && x.ID == scope.entityId) || !element.IsPrivate); }); });
                }

                var rules = [];
                var lstObject = [];

                //If entity id is provided, then load non-static rules of the entity.
                var lstRulesAndObject = [];
                if (scope.entityId) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityname = scope.entityId;
                    while (parententityname != "") {
                        var entity = entities.filter(function (x) { return x.ID == parententityname; });
                        if (entity && entity.length > 0) {
                            rules = entity[0].Rules.filter(function (y) { return !y.IsStatic && (parententityname == scope.entityId || !y.IsPrivate); });
                            lstObject = entity[0].Attributes.filter(function (z) { return z.DataType == "Object"; });
                            if (scope.entityId != parententityname) {
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

                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        var item = data.filter(function (x) { return x.ID == arrText[index]; });
                        if (item.length > 0 && typeof item[0].Rules != "undefined" && index < arrText.length) {
                            if (item[0].ID == arrText[index]) {
                                data = item[0].Rules.filter(function (y) { return y.IsStatic && ((scope.entityId && item[0].ID == scope.entityId) || !y.IsPrivate); });
                            }
                        }
                        else if (item.length > 0 && item[0].DataType != undefined && index < arrText.length) {
                            if (item[0].DataType == "Object" && item[0].ID == arrText[index]) {
                                parententityname = item[0].Entity;
                                data = [];
                                while (parententityname != "") {
                                    var entity = entities.filter(function (x) { return x.ID == parententityname; });
                                    if (entity && entity.length > 0) {
                                        data = data.concat(entity[0].Rules.filter(function (y) { return !y.IsStatic && !y.IsPrivate; }));
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

                // filtering rules
                if (scope.ruleTypes) {
                    for (var index = 0, len = scope.ruleTypes.length; index < len; index++) {
                        scope.ruleTypes[index] = scope.ruleTypes[index].toLowerCase();
                    }
                }
                if (scope.returnTypes) {
                    for (var index = 0, len = scope.returnTypes.length; index < len; index++) {
                        scope.returnTypes[index] = scope.returnTypes[index].toLowerCase();
                    }
                }
                var item = [];
                if (arrText.length > 0) {
                    for (var index = 0; index < arrText.length; index++) {
                        item = data.filter(function (x) {
                            return x.ID.toLowerCase().contains(arrText[index].toLowerCase())
                                &&
                                (!x.Status || x.Status.toLowerCase() === "active")
                                &&
                                (!x.RuleType || !scope.ruleTypes || !scope.ruleTypes.length || scope.ruleTypes.indexOf(x.RuleType.toLowerCase()) > -1)
                                &&
                                (!x.ReturnType || !scope.returnTypes || !scope.returnTypes.length || scope.returnTypes.indexOf(x.ReturnType.toLowerCase()) > -1);
                        });
                    }
                    data = item;
                }


                setRuleIntellisense($(input), data, scope);

                if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    eargs.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                scope.validate();
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedRule);
                    }
                    scope.changeCallback();
                }
            };
            scope.validate = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    $ValidationService.validateRule(scope.model, scope.selectedRule, scope.entityId, scope.nonStatic, scope.returnTypes, scope.ruleTypes, scope.propertyname, scope.vAllowEmpty, scope.vFieldName);
                }
            };
        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="vErrorprop" ng-attr-title="{{vErrorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedRule}}" ng-model="selectedRule" ng-keyup="onKeyDown($event)" ng-change="onChange()" ng-blur="onblurcallback()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateInput"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);

app.directive("commonIntellisenseWithoutUndoRedo", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            selectedItem: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            isReset: "=",
            errorprop: "=",
            validate: "=",
            isinsidedialog: "="
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement || scope.isReset) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
            };
            scope.onChange = function () {
                scope.validateInput();
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.selectedItem);
                    }
                    scope.changeCallback();
                }
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var errorMessage = CONST.VALIDATION.INVALID_FIELD;
                    if (property == "sfwEntityField") errorMessage = CONST.VALIDATION.ENTITY_FIELD_INCORRECT;
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, errorMessage, undefined, false, scope.isinsidedialog);
                }
            };
        },
        template: '<div style="position:relative"><span class="info-tooltip dtc-span" ng-show="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><div class="input-group"><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selectedItem}}" ng-model="selectedItem" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div></div>'
    };
}]);

app.directive("templateNameIntellisense", ["$ValidationService", "CONSTANTS", function ($ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            modelname: '=',
            onchangecallback: '&',
            model: '=',
            propertyname: '=',
            validate: '=',
            errorprop: '='
        },
        link: function (scope, element, attributes) {
            scope.onTemplateNameChange = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    var input = event.target;
                    scope.inputElement = input;
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val().trim());
                        event.preventDefault();
                    } else {
                        populateTemplateNameData(input);
                    }
                }
            };

            scope.showTemplateNameList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                inputElement.focus();
                scope.inputElement = inputElement;
                populateTemplateNameData(inputElement);
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val().trim());
                if (event) {
                    event.stopPropagation();
                }
            };

            var populateTemplateNameData = function (input) {
                if (scope.collection && scope.collection.length > 0) {
                    setSingleLevelAutoComplete($(input), scope.collection);
                }
            };

            scope.$watch('modelname', function (newVal, oldVal) {
                if (scope.onchangecallback) {
                    scope.onchangecallback();
                }
            }, true);
            scope.validateTemplateName = function () {
                if (scope.validate) {
                    var strProp = scope.propertyname.split('.');
                    var property = strProp[strProp.length - 1];
                    $ValidationService.checkValidListValue(scope.collection, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_TEMPLATE, undefined);
                }
            };
        },
        template: '<div><div class="input-group"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input type="text" class="form-control input-sm" title="{{modelname}}" ng-model="modelname" ng-keydown="onTemplateNameChange($event)" ng-keyup="validateTemplateName()" ng-change="validateTemplateName()" undoredodirective model="model" propertyname="propertyname" undorelatedfunction ="validateTemplateName"><span class="input-group-addon button-intellisense" ng-click="showTemplateNameList($event);"></span></div></div>'
    };
}]);

app.directive("constantintellisene", ["$compile", "$rootScope", "$resourceFactory", function ($compile, $rootScope, $resourceFactory) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            constantbinding: '=',
            model: '=',
            propertyname: '=',
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
                else {
                    scope.initializeIntellisense();
                }
            };

            scope.onKeyUp = function (event) {
                scope.initializeIntellisense();
            };

            scope.onIntellisenseButtonClick = function (event) {
                if (!scope.inputElement) {
                    scope.initializeIntellisense();
                }
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement) {
                    if ($(scope.inputElement).data('ui-autocomplete') && scope.collection) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.initializeIntellisense = function () {
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                if (!scope.collection) {
                    scope.collection = [];
                }

                scope.collection = $resourceFactory.getConstantsList();
                if (scope.inputElement && scope.inputElement.length > 0) {
                    var arrText = getSplitArray($(scope.inputElement).val(), scope.inputElement[0].selectionStart);
                    if (arrText.length > 0) {
                        for (var index = 0; index < arrText.length; index++) {
                            var item = scope.collection.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (item[0].Type && item[0].ID == arrText[index] && item[0].Type == "Constant") {
                                    if (item[0].Type == "Constant") {
                                        scope.collection = $resourceFactory.getConstantsList(arrText.join("."));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (scope.inputElement) {
                    setRuleIntellisense($(scope.inputElement), scope.collection);
                }
                //setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope);
            };
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{constantbinding}}" ng-model="constantbinding" ng-keydown="onKeyDown($event)" ng-keyup="onKeyUp($event)" undoredodirective model="model" propertyname="propertyname"/><span class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>',
    };
}]);

app.directive("entityquerycolumnsintellisense", ['$rootScope', '$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', function ($rootScope, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        retrict: 'E',
        replace: true,
        scope: {
            model: "=",
            formodel: '=',
            modebinding: "=",
            propertyname: '=',
            queryid: '=',
        },
        template: "<div><span class='info-tooltip dtc-span' ng-if='model.errors.invalid_data_field' ng-attr-title='{{model.errors.invalid_data_field}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span><div class='input-group'><input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modebinding}}' ng-model='modebinding' undoredodirective model='model' propertyname='propertyname' ng-keydown='filterIntelliseneList($event)'/><span class='input-group-addon button-intellisense' ng-click='showEntityQueryColumnIntellisenseList($event)'></span></div></div>",
        link: function (scope, element, attributes) {

            var setAutoComplete = function (input, ablnShowIntellisense) {
                if (scope.queryid != undefined && scope.queryid != "") {
                    setSingleLevelAutoComplete(input, scope.lstQueryColumnList, scope, "CodeID", "CodeID");
                    if (ablnShowIntellisense && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                    };
                }
            };
            scope.$watch('queryid', function (newVal, oldVal) {
                if (newVal != oldVal) {
                    scope.iblnresetData = true;
                    scope.lstQueryColumnList = [];
                }
            });

            scope.loadQueryColumns = function (ablnShowIntellisense) {
                $.connection.hubForm.server.getEntityQueryColumns(scope.queryid, "").done(function (data) {
                    scope.$evalAsync(function () {
                        scope.lstQueryColumnList = data;
                        if (scope.inputElement) {
                            setAutoComplete(scope.inputElement, ablnShowIntellisense);
                        }
                    });
                });
            }
            scope.filterIntelliseneList = function (event) {
                if (!scope.lstQueryColumnList || scope.iblnresetData) {
                    scope.iblnresetData = false;
                    scope.loadQueryColumns(false);
                }

                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }

                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        //setAutoComplete(scope.inputElement);
                        if ($(scope.inputElement).data('ui-autocomplete') && scope.lstQueryColumnList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    } else {
                        setAutoComplete(scope.inputElement);
                    }

                    //event.preventDefault();
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                }
            };

            scope.showEntityQueryColumnIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                if (!scope.lstQueryColumnList || scope.iblnresetData) {
                    scope.loadQueryColumns(true);
                }
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };

        }
    };
}]);

app.directive("rulegroupintellisense", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', function ($EntityIntellisenseFactor, $ValidationService, CONST, $NavigateToFileService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            modelvalue: '=',
            propertyname: '=',
            entityid: "=",
            labelText: "@",
            labelClass: "@",
            errorprop: "=",
            validate: "=",
            callback: "&"
        },
        template: "<div>" +
            "<label class={{labelClass}} ng-if='!modelvalue || errorprop'>{{labelText}} </label>" +
            "<label class={{labelClass}} ng-if='modelvalue && !errorprop'><a ng-click='selectGroupClick(modelvalue)'>{{labelText}}</a></label>" +
            "<div class='col-xs-7'>" +
            "<span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>" +
            "<div class='input-group'>" +
            "<input type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue' undoredodirective model='model' propertyname='propertyname' undorelatedfunction ='validateRuleGroup' ng-change='validateRuleGroup()' ng-keyup='LoadRuleList($event)' ng-keydown='filterIntelliseneList($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showRuleGroupIntellisenseList($event)'></span>" +
            "</div></div></div>",
        link: function (scope, element, attributes) {
            var setAutoComplete = function (input) {
                setSingleLevelAutoComplete(input, scope.lstRuleGroupList, scope);

            };

            scope.LoadRuleGroupList = function (isShowList) {
                if (scope.entityid) {
                    hubMain.server.getGroupList(scope.entityid).done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.lstRuleGroupList = scope.getFormatedData(data);
                                if (!scope.inputElement) scope.inputElement = $("#ScopeId_" + scope.$id);
                                if (scope.inputElement) {
                                    $(scope.inputElement).focus();
                                    setAutoComplete(scope.inputElement);
                                    if (isShowList && $(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                                }
                            }

                        });
                    });
                }
            };
            scope.getFormatedData = function (data) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].dictAttributes && data[i].dictAttributes.ID) {
                        tempData.push({ ID: data[i].dictAttributes.ID });
                    }
                }
                return tempData;
            };
            scope.filterIntelliseneList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (!scope.modelvalue || scope.modelvalue == "" || !scope.lstRuleGroupList) {
                            scope.LoadRuleGroupList(true);
                            //if ($(scope.inputElement).data('ui-autocomplete')) 
                            //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                        else if ($(scope.inputElement).data('ui-autocomplete') && scope.lstRuleGroupList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    }
                }
            };

            scope.showRuleGroupIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                scope.LoadRuleList();
                setAutoComplete(inputElement);
                inputElement.focus();

                if (event) {
                    event.stopPropagation();
                }
            };
            scope.LoadRuleList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (!scope.modelvalue || !scope.lstRuleGroupList) {
                    scope.LoadRuleGroupList(true);
                }
                scope.validateRuleGroup();
            };
            scope.selectGroupClick = function (aGroupID) {
                scope.validateRuleGroup();
                var property = scope.propertyname.split('.')[1];

                if (scope.validate && aGroupID && aGroupID != "" && scope.model.errors && !scope.model.errors[property]) {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
                if (!scope.validate && aGroupID && aGroupID != "") {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
            };
            scope.validateRuleGroup = function () {
                if (scope.propertyname && scope.validate) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstRuleGroupList, 'ID');
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_GROUP, undefined);
                }
            };
            scope.LoadRuleGroupList(false);
        }
    };
}]);
app.directive("rulegroupintellisensewithoutundoredo", ['$EntityIntellisenseFactory', '$ValidationService', 'CONSTANTS', '$NavigateToFileService', function ($EntityIntellisenseFactor, $ValidationService, CONST, $NavigateToFileService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            modelvalue: '=',
            propertyname: '=',
            entityid: "=",
            labelText: "@",
            labelClass: "@",
            errorprop: "=",
            validate: "=",
            callback: "&"
        },
        template: "<div>" +
            "<label class={{labelClass}} ng-if='!modelvalue || errorprop'>{{labelText}} </label>" +
            "<label class={{labelClass}} ng-if='modelvalue && !errorprop'><a ng-click='selectGroupClick(modelvalue)'>{{labelText}}</a></label>" +
            "<div class='col-xs-7' >" +
            "<span class='info-tooltip dtc-span' ng-if='errorprop' ng-attr-title='{{errorprop}}' style='color:red !important'><i class='fa fa-exclamation-circle' aria-hidden='true'></i></span>" +
            "<div class='input-group'>" +
            "<input  type='text' id='ScopeId_{{$id}}' class='form-control input-sm' title='{{modelvalue}}' ng-model='modelvalue'  ng-change='validateRuleGroup()' ng-keyup='LoadRuleList($event)' ng-keydown='filterIntelliseneList($event)'/>" +
            "<span class='input-group-addon button-intellisense' ng-click='showRuleGroupIntellisenseList($event)'></span></div>" +
            "</div>" +
            "</div>",
        link: function (scope, element, attributes) {
            var setAutoComplete = function (input) {
                setSingleLevelAutoComplete(input, scope.lstRuleGroupList, scope);
            };

            scope.LoadRuleGroupList = function (isShowList) {
                if (scope.entityid) {
                    hubMain.server.getGroupList(scope.entityid).done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.lstRuleGroupList = scope.getFormatedData(data);
                                if (!scope.inputElement) scope.inputElement = $("#ScopeId_" + scope.$id);
                                if (scope.inputElement) {
                                    $(scope.inputElement).focus();
                                    setAutoComplete(scope.inputElement);
                                    if (isShowList && $(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                                }
                            }

                        });
                    });
                }
            };
            scope.getFormatedData = function (data) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].dictAttributes && data[i].dictAttributes.ID) {
                        tempData.push({ ID: data[i].dictAttributes.ID });
                    }
                }
                return tempData;
            };
            scope.filterIntelliseneList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (!scope.modelvalue || scope.modelvalue == "" || !scope.lstRuleGroupList) {
                            scope.LoadRuleGroupList(true);
                            //if ($(scope.inputElement).data('ui-autocomplete')) $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                        else if ($(scope.inputElement).data('ui-autocomplete') && scope.lstRuleGroupList) {
                            $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                            event.preventDefault();
                        }
                    }
                }
            };

            scope.showRuleGroupIntellisenseList = function (event) {
                var inputElement = $(event.target).prevAll("input[type='text']");
                scope.inputElement = inputElement;
                scope.LoadRuleList();
                setAutoComplete(inputElement);
                inputElement.focus();
                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.LoadRuleList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target);
                }
                if (scope.modelvalue == "" || !scope.lstRuleGroupList) {
                    scope.LoadRuleGroupList(true);
                }
                scope.validateRuleGroup();
            };
            scope.selectGroupClick = function (aGroupID) {
                scope.validateRuleGroup();
                var property = scope.propertyname.split('.')[1];

                if (scope.validate && aGroupID && aGroupID != "" && scope.model.errors && !scope.model.errors[property]) {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
                if (!scope.validate && aGroupID && aGroupID != "") {
                    if (scope.callback) scope.callback();
                    $NavigateToFileService.NavigateToFile(scope.entityid, "groupslist", aGroupID);
                }
            };
            scope.validateRuleGroup = function () {
                if (scope.propertyname && scope.validate) {
                    var property = scope.propertyname.split('.')[1];
                    var list = $ValidationService.getListByPropertyName(scope.lstRuleGroupList, 'ID');
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_GROUP, undefined);
                }
            };
            scope.LoadRuleGroupList();
        }
    };
}]);

app.directive("filterList", ["$rootScope", "$filter", function ($rootScope, $filter) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            isChild: "=",
            changeCallback: '&',
            propertyname: "=",
            alternateProperty: "=",
            cssClass: "@",
            slideLeft: "=",
            skipProp: "=",
            skipValue: "="
        },
        link: function (scope, element, attributes) {
            scope.model = "";
            scope.inlineStyle = attributes.Style;
            var prop = scope.propertyname.split(".");
            var prop2;
            if (scope.alternateProperty) {
                prop2 = scope.alternateProperty.split(".");
            }
            scope.$watch('slideLeft', function () {
                if (!scope.slideLeft) {
                    scope.model = "";
                }
            });
            if (scope.cssClass) {
                scope.textBoxClass = scope.cssClass;
            } else {
                scope.textBoxClass = "col-xs-9";
            }
            scope.onKeyDown = function (event) {
                //if (!scope.inputElement) {
                //    scope.initializeIntellisense();
                //}
                scope.initializeIntellisense();

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    if (scope.model) {
                        scope.onChange();
                    }
                    event.preventDefault();
                }
            };

            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                var list = scope.collection;

                if (scope.collection && scope.collection.length > 0 && scope.skipProp && scope.skipValue != undefined) {  // scope.skipValue can be boolean type or string type 
                    list = scope.collection.filter(function (obj) {
                        if (prop.length > 1) {
                            return (obj[prop[0]][scope.skipProp] != scope.skipValue);
                        } else {
                            return (obj[scope.skipProp] != scope.skipValue);
                        }
                    });
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                newList = [];
                scope.maketList(list, prop, prop2);
                //  setSingleLevelAutoComplete($(scope.inputElement), list, scope, scope.selectedPropertyName, scope.displayPropertyName);
                setSingleLevelAutoComplete($(scope.inputElement), newList, scope);
            };
            scope.onChange = function () {
                if (scope.propertyname) {
                    scope.isMatched = false;
                    //scope.selectedItem = scope.collection.filter(function (obj) {
                    //    if (prop.length > 1) {
                    //        return (obj[prop[0]][prop[1]] == scope.model);
                    //    } else {
                    //        return (obj[prop[0]] == scope.model);
                    //    }
                    //});
                    scope.filterByProperty(scope.collection, prop, prop2);
                    //   scope.selectedItem = scope.selectedItem && scope.selectedItem[0];
                    if (scope.selectedItem && scope.model && scope.isMatched) {
                        scope.changeCallback({ obj: scope.selectedItem });
                    }
                }
            };

            var newList = [];
            scope.maketList = function (collection, prop, prop2) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var obj = collection[i];
                    if (prop.length > 1) {
                        if (prop2 && prop2.length > 1 && obj[prop2[0]][prop2[1]]) { // if alternate property and  property like "dictAttributes.ID"
                            newList.push(obj[prop2[0]][prop2[1]]);
                        } else if (obj[prop[0]][prop[1]]) { // if property like "dictAttributes.ID"
                            newList.push(obj[prop[0]][prop[1]]);
                        }
                    } else {
                        if (obj[prop[0]]) {  // if property like "ID"
                            newList.push(obj[prop[0]]);
                        }
                    }

                    if (scope.isChild && obj.hasOwnProperty("Children") && obj.Children.length > 0) {
                        scope.maketList(obj.Children, prop, prop2);
                    }
                    if (scope.isChild && obj.hasOwnProperty("Elements") && obj.Elements.length > 0) {
                        scope.maketList(obj.Elements, prop, prop2);
                    }
                }
            };

            scope.filterByProperty = function (collection, prop, prop2, parentObj) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var obj = collection[i];

                    if (prop2 && prop2.length > 0 && obj[prop2[0]][prop2[1]] == scope.model) { // if property like "dictAttributes.ID"
                        if (parentObj) parentObj.IsExpanded = true;
                        scope.selectedItem = obj;
                        scope.isMatched = true;
                        break;
                    }
                    if (prop.length > 1) {
                        if (obj[prop[0]][prop[1]] == scope.model) { // if property like "dictAttributes.ID"
                            if (parentObj) parentObj.IsExpanded = true;
                            scope.selectedItem = obj;
                            scope.isMatched = true;
                            break;
                        }
                    } else {
                        if (obj[prop[0]] == scope.model) { // if property like "ID"
                            if (parentObj) parentObj.IsExpanded = true;
                            scope.selectedItem = obj;
                            scope.isMatched = true;
                            break;
                        }
                    }

                    if (scope.isChild && obj.hasOwnProperty("Children") && obj.Children.length > 0) {
                        scope.filterByProperty(obj.Children, prop, prop2, obj);
                    }
                    if (scope.isChild && obj.hasOwnProperty("Elements") && obj.Elements.length > 0) {
                        scope.filterByProperty(obj.Elements, prop, prop2, obj);
                    }
                }
            };
        },
        template: "<div style='{{inlineStyle}}' class='{{textBoxClass}}' ng-show='slideLeft'><input type='text' style='float:right;width:0;border-radius:5px!important' ng-class='slideLeft? \"form-control input-sm search-text-visible\" : \"full-width\" ' id='ScopeId_{{$id}}' placeholder=' Search '  title='{{model}}' ng-model='model' ng-change='onChange()' ng-keydown='onKeyDown($event)' /></div>"
    };
}]);
app.directive("filterTextList", [function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: "=",
            model: "=",
            changeCallback: '&',
            cssClass: "@",
            slideLeft: "="
        },
        link: function (scope, element, attributes) {
            if (scope.cssClass) {
                scope.textBoxClass = scope.cssClass;
            } else {
                scope.textBoxClass = "col-xs-9";
            }
            scope.onKeyDown = function (event) {
                scope.initializeIntellisense(event);

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    if (scope.model) {
                        scope.onChange();
                    }
                    event.preventDefault();
                }
                if (event && event.type == "click") {
                    scope.showIntellisenseList();
                }
            };

            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };
            scope.initializeIntellisense = function () {
                if (!scope.collection) {
                    scope.collection = [];
                }
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                setSingleLevelAutoComplete($(scope.inputElement), scope.collection);
            };
            scope.onChange = function () {

                scope.isMatched = false;

                scope.filterText(scope.collection);
                if (scope.selectedItem && scope.model && scope.isMatched) {
                    scope.changeCallback({ obj: scope.selectedItem });
                }

            };


            scope.filterText = function (collection) {
                for (var i = 0, len = collection.length; i < len; i++) {
                    var text = collection[i];
                    if (text == scope.model) {
                        scope.selectedItem = text;
                        scope.isMatched = true;
                        break;
                    }
                }
            };
        },
        template: "<div style='top:-6px' class='{{textBoxClass}}' ng-show='slideLeft'><div class='input-group'><input type='text'  ng-class='slideLeft? \"form-control input-sm search-text-visible\" : \"full-width\" ' id='ScopeId_{{$id}}' placeholder=' Search or (ctrl + space) display suggestion' title='{{model}}' ng-model='model' ng-change='onChange()' ng-keydown='onKeyDown($event)' /><span class='input-group-addon button-intellisense' ng-click='onKeyDown($event)'></span></div></div>"
    };
}]);

app.directive("servermethoddirective", ["$compile", "$rootScope", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $ValidationService, CONST) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            servermethod: '=',
            displayPropertyName: "=",
            selectedPropertyName: "=",
            changeCallback: '&',
            model: "=",
            propertyname: "=",
            isDisable: "=",
            errorprop: "=",
            validate: "=",
            formobject: "=",
            isloadremoteobject: '=',
        },
        link: function (scope, element, attributes) {
            scope.onKeyDown = function (event) {
                scope.initializeIntellisense();

                if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                    scope.showIntellisenseList();
                    event.preventDefault();
                }
            };
            scope.onIntellisenseButtonClick = function (event) {
                scope.initializeIntellisense();
                scope.inputElement.focus();
                scope.showIntellisenseList();
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.showIntellisenseList = function () {
                if (scope.inputElement && $(scope.inputElement).data('ui-autocomplete')) {
                    $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                }
            };

            scope.getServerMethod = function () {
                scope.collection = [];
                var RemoteObjectName = "srvCommon";
                if (scope.formobject && scope.formobject.dictAttributes.sfwType != "Correspondence" && scope.formobject.dictAttributes.sfwType != "Report" && scope.formobject.dictAttributes.sfwRemoteObject) {
                    RemoteObjectName = scope.formobject.dictAttributes.sfwRemoteObject;
                }

                if (scope.inputElement && !$(scope.inputElement).val()) {
                    scope.formobject.RemoteObjectCollection = [];
                    $.connection.hubForm.server.getRemoteObjectList().done(function (data) {
                        scope.$evalAsync(function () {
                            if (data) {
                                scope.formobject.RemoteObjectCollection = data;
                                var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                                scope.collection = PopulateServerMethod([], scope.model, objServerObject, scope.isloadremoteobject);
                                if (scope.collection && scope.collection.length > 0) {
                                    setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                                }
                            }
                        });
                    });
                } else {

                    var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                    scope.collection = PopulateServerMethod([], scope.model, objServerObject, scope.isloadremoteobject);
                    if (scope.collection && scope.collection.length > 0) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                    }
                }

            };

            scope.initializeIntellisense = function () {
                scope.getServerMethod();
                scope.inputElement = $("#ScopeId_" + scope.$id);
                $(scope.inputElement).focus();
                if (scope.collection && scope.collection.length > 0) {
                    setSingleLevelAutoComplete($(scope.inputElement), scope.collection, scope, scope.selectedPropertyName, scope.displayPropertyName);
                }
            };
            scope.onChange = function () {
                if (scope.changeCallback) {
                    if (scope.model && scope.propertyname) {
                        setPropertyValue(scope.model, scope.propertyname, scope.servermethod);
                    }
                    scope.changeCallback();
                }
                scope.validateInput();
            };
            scope.validateInput = function () {
                if (scope.propertyname && scope.validate) {
                    var prop = scope.propertyname.split('.');
                    var property = prop[prop.length - 1];
                    var list = scope.collection;
                    if (scope.selectedPropertyName) list = $ValidationService.getListByPropertyName(scope.collection, scope.selectedPropertyName);
                    $ValidationService.checkValidListValue(list, scope.model, $(scope.inputElement).val(), property, property, CONST.VALIDATION.INVALID_METHOD, undefined);
                }
            };

        },
        template: '<div class="input-group full-width"><span class="info-tooltip dtc-span" ng-if="errorprop" ng-attr-title="{{errorprop}}" style="color:red !important"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span><input ng-disabled="isDisable" type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{servermethod}}" ng-model="servermethod" ng-keyup="validateInput()" ng-change="onChange()" ng-keydown="onKeyDown($event)" undoredodirective model="model" propertyname="propertyname"/><span ng-show="!isDisable" class="input-group-addon button-intellisense" ng-click="onIntellisenseButtonClick($event)"></span></div>'
    };
}]);


app.directive("internalfunctionsintellisense", ["$compile", "$rootScope", function ($compile, $rootScope) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            selecteditem: '=',
            onchangecallback: '&',
        },
        template: '<div class="input-group"><input type=\ "text\" id="ScopeId_{{$id}}" class="form-control input-sm" title="{{selecteditem}}" ng-model="selecteditem" ng-change="onChange()"  ng-keydown="onInternalFunctionKeyDown($event)" /><span class="input-group-addon button-intellisense" ng-click="showInternalFunctionList($event)"></span></div>',
        link: function (scope, element, attributes) {

            scope.lstInternalFunctions = [];
            $.connection.hubInternalFunctions.server.getRuleFunctions().done(function (data) {
                scope.$evalAsync(function () {
                    scope.lstInternalFunctions = data;
                });

            });
            scope.onInternalFunctionKeyDown = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    if (!scope.inputElement) {
                        scope.inputElement = $(event.target);
                    }
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val());
                        event.preventDefault();
                    }
                    else if (scope.lstInternalFunctions && scope.lstInternalFunctions.length > 0) {
                        setSingleLevelAutoComplete($(scope.inputElement), scope.lstInternalFunctions, scope, "Method", "Method", scope.onchangecallback);
                        //$(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
            };
            scope.showInternalFunctionList = function (event) {
                if (!scope.inputElement) {
                    scope.inputElement = $(event.target).prevAll("input[type='text']");
                }
                scope.inputElement.focus();
                if (scope.inputElement) {
                    setSingleLevelAutoComplete(scope.inputElement, scope.lstInternalFunctions, scope, "Method", "Method", scope.onchangecallback);
                    if ($(scope.inputElement).data('ui-autocomplete')) {
                        $(scope.inputElement).autocomplete("search", $(scope.inputElement).val().trim());
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.onChange = function () {
                if (scope.onchangecallback) {
                    var obj;
                    var lst = scope.lstInternalFunctions.filter(function (itm) { return itm.Method == scope.selecteditem; });
                    if (lst && lst.length > 0) {
                        obj = lst[0];
                    }
                    scope.onchangecallback({ obj: obj });
                }

            };
        },
        //template: 
    };
}]);