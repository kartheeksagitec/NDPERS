app.directive("baseControlView", ["$compile", "CONSTANTS", "$rootScope", "$EntityIntellisenseFactory", "$GetEntityFieldObjectService", "$ValidationService", "$SgMessagesService", function ($compile, CONST, $rootScope, $EntityIntellisenseFactory, $GetEntityFieldObjectService, $ValidationService, $SgMessagesService) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',
            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        // templateUrl: 'Form/views/Controls/BaseControlView.html',
        template: "<div context-menu='controlmenuOptions' class='form-control-wrapper' ng-class='{\"form-invisible-control\" : model.dictAttributes.Visible === \"False\"}'>  <span show-errors model='model'></span> </div>",

        link:
        {
            pre: function (scope, element, attributes) {

                if (scope.model.Name == 'sfwTextBox' || scope.model.Name == 'sfwLabel' || scope.model.Name == 'sfwDropDownList' || scope.model.Name == 'sfwMultiSelectDropDownList'
                    || scope.model.Name == 'sfwCascadingDropDownList') {
                    element.append("<editable-control-directive model='model' formodel='formodel' lstloadedentitycolumnstree='lstloadedentitycolumnstree' />");
                }
                else if (scope.model.Name == 'sfwGridView') {
                    element.append("<grid-control-directive model='model' formodel='formodel' objparent='objparent' lstloadedentitytrees='lstloadedentitytrees' lstloadedentitycolumnstree='lstloadedentitycolumnstree' lstdisplayentities='lstdisplayentities' entitytreename='entitytreename'  buttons-collection='buttonsCollection'/>");
                }
                else if (scope.model.Name == 'sfwTable') {
                    element.append("<table-control-directive model='model' formodel='formodel' objparent='objparent' lstloadedentitytrees='lstloadedentitytrees' lstloadedentitycolumnstree='lstloadedentitycolumnstree' lstdisplayentities='lstdisplayentities' entitytreename='entitytreename'  buttons-collection='buttonsCollection'/>");
                }
                else if (scope.model.Name == 'sfwTabContainer') {
                    element.append("<tabcontainer-control-directive model='model' formodel='formodel' objparent='objparent' lstloadedentitytrees='lstloadedentitytrees' lstloadedentitycolumnstree='lstloadedentitycolumnstree' lstdisplayentities='lstdisplayentities' entitytreename='entitytreename'  buttons-collection='buttonsCollection'/>");
                }
                else if (scope.model.Name == 'udc') {
                    element.append("<usercontrol-control-directive model='model' formodel='formodel' objparent='objparent' lstloadedentitytrees='lstloadedentitytrees' lstloadedentitycolumnstree='lstloadedentitycolumnstree' lstdisplayentities='lstdisplayentities' entitytreename='entitytreename' buttons-collection='buttonsCollection'/>");
                }
                else if (scope.model.Name == 'sfwPanel' || scope.model.Name == 'sfwDialogPanel' || scope.model.Name == 'sfwListView') {
                    element.append("<childpanel-control-directive model='model' formodel='formodel' objparent='objparent' lstloadedentitytrees='lstloadedentitytrees' lstloadedentitycolumnstree='lstloadedentitycolumnstree' lstdisplayentities='lstdisplayentities' entitytreename='entitytreename'  buttons-collection='buttonsCollection'/>");
                }
                else if (scope.model.Name == 'sfwButtonGroup') {
                    element.append("<emptyparent-control-directive></emptyparent-control-directive>");
                }
                else {
                    element.append("<common-control-directive model='model' formodel='formodel'/>");
                }
                //     if (scope.model.Name == 'sfwCheckBoxList' || scope.model.Name == 'sfwRadioButtonList' || scope.model.Name == 'sfwChart' || scope.model.Name == 'sfwCalendar' || scope.model.Name == 'sfwScheduler' |
                //     scope.model.Name=='sfwCheckBox' || scope.model.Name=='CompareValidator' || scope.model.Name=='sfwFileLayout' || scope.model.Name=='sfwSourceList' || scope.model.Name=='sfwRadioButton' || scope.model.Name=='sfwSlider'
                //|| scope.model.Name=='sfwImage' || scope.model.Name=='sfwCRViewer' || scope.model.Name=='sfwFileUpload' || scope.model.Name=='sfwMultiCorrespondence' || scope.model.Name=='sfwOpenDetail' || scope.model.Name=='sfwQuestionnairePanel'
                //|| scope.model.Name=='RequiredFieldValidator' || scope.model.Name=='sfwSoftErrors' || scope.model.Name=='sfwListPicker' || scope.model.Name=='sfwTargetList' || scope.model.Name=='sfwRuleViewer' || scope.model.Name=='sfwListBox'
                //|| scope.model.Name == 'sfwJSONData' || scope.model.Name == 'br' || scope.model.Name == 'hr' || scope.model.Name == 'sfwKnob' || scope.model.Name == 'sfwDateTimePicker' || scope.model.Name == 'sfwLiteral'
                //   || scope.model.Name == 'sfwButton' || scope.model.Name == 'sfwLinkButton' || scope.model.Name == 'sfwImageButton' || scope.model.Name == 'sfwHyperLink')

                $compile(element.contents())(scope);
            },
            post: function (scope, element, attributes) {
                if (scope.objparent) {
                    scope.model.ParentVM = scope.objparent;
                }

                scope.controlmenuOptions = [
                    ['Create Compatible Caption', function ($itemScope) {
                        if (scope.model) {
                            scope.OnCreateCompatibleLabelClick(scope.model, scope.formodel, $EntityIntellisenseFactory, $rootScope);
                        }
                    }, function ($itemScope, event) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if ($itemScope.tabs) {
                            scope.selectControl($itemScope.tabs, event, scope.formodel);
                        }
                        else {
                            selectControl(scope.model, event, scope.formodel);
                        }
                        if ((scope.model && scope.model.dictAttributes.sfwIsCaption) || scope.IsDataView) {
                            return false;
                        }
                        else if (scope.model && (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwChart" || scope.model.Name == "sfwPanel" ||
                            scope.model.Name == "sfwDialogPanel" || scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton" || scope.model.Name == "sfwImageButton" || scope.model.Name == "sfwTabContainer" || scope.model.Name == "sfwTabSheet"
                            || scope.model.Name == "udc" || scope.model.Name == "sfwListViewer" || scope.model.Name == "sfwButtonGroup")) {
                            return false;
                        }
                        else {
                            if (FindParent(scope.model, "sfwGridView") || FindParent(scope.model, "sfwButtonGroup")) {
                                return false;
                            }
                            else if (hasCaption(scope.model)) {
                                return false;
                            }
                            return true;
                        }
                    }],

                    ['Cut Control', function ($itemScope) {
                        if (scope.model) {
                            scope.OnCutControlClick(scope.model);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if ((scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model && (scope.model.Name == "sfwTabContainer" || scope.model.Name == "sfwGridView")) || scope.IsDataView || scope.model.Name == "sfwToolTipButton") {
                            return false;
                        }
                        return true;

                    }, null, "Ctrl+X"],
                    ['Copy Control', function ($itemScope) {
                        if (scope.model) {
                            scope.OnCopyControlClick(scope.model);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if ((scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model && (scope.model.Name == "sfwTabContainer" || scope.model.Name == "sfwGridView")) || scope.IsDataView || scope.model.Name == "sfwToolTipButton") {
                            return false;
                        }
                        return true;
                    }, null, "Ctrl+C"],
                    ['Delete Control', function ($itemScope) {
                        if (scope.model) {
                            scope.OnDeleteControlClick(scope.model);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if ((scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model.Name == "sfwTabContainer") || scope.IsDataView) {
                            return false;
                        }
                        if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model.Name == "sfwGridView" && !scope.formodel.IsLookupCriteriaEnabled) {
                            return false;
                        }
                        return true;

                    }],
                    ['Add Control', function ($itemScope) {
                        if ($itemScope.model) {
                            scope.openChangeControlDialog("addcontrol", $itemScope.model);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwButtonGroup') {
                            return true;
                        } else {
                            return false;
                        }
                    }],

                    ['Change Control', function () {
                        if (scope.model) {
                            scope.openChangeControlDialog("changecontrol", null);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.model && (scope.model.Name == "sfwTabContainer" || scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwChart" ||
                            scope.model.Name == "sfwGridView" || scope.model.Name == "sfwPanel" || scope.model.Name == "sfwDialogPanel" ||
                            scope.model.Name == "sfwListView" || scope.model.Name == "sfwScheduler" || scope.model.Name == "sfwCalendar"
                            || scope.model.Name == "sfwSoftErrors") || scope.IsDataView) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }],
                    ['Add Columns', function () {
                        if (scope.model) {
                            scope.OnAddGridColumns();
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.model && scope.model.Name == "sfwGridView" && !scope.IsDataView) {
                            return true;
                        }
                        return false;
                    }],
                    ['Paste Control', function ($itemScope) {
                        if (scope.model) {
                            scope.OnPasteControl(scope.model);
                        }
                    }, function ($itemScope) {
                        if (FindParent(scope.model, "sfwGridView") || scope.model.Name == "sfwButtonGroup") {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }, null, "Ctrl+V"],
                    ['Clear Cell', function ($itemScope) {
                        var cellVM = $itemScope.model;
                        while (cellVM.Name != "sfwColumn") {
                            if (cellVM.ParentVM == null) {
                                break;
                            }
                            cellVM = cellVM.ParentVM;
                        }
                        if (cellVM.Name == "sfwColumn") {
                            ClearCell(cellVM, $rootScope, true);

                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model.Name == "sfwGridView" && !scope.formodel.IsLookupCriteriaEnabled) {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }],
                    ['Clear Panel', function ($itemScope) {
                        if ($itemScope.model) {
                            var cellVM = FindParent($itemScope.model, "sfwColumn", true);
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnClearGridClick(tableVM);
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && !scope.formodel.IsLookupCriteriaEnabled) {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }, null, "Shift+Del"],
                    ['Delete Row', function ($itemScope) {
                        if ($itemScope.model) {
                            var cellVM = $itemScope.model;
                            while (cellVM.Name != "sfwColumn") {
                                if (cellVM.ParentVM == null) {
                                    break;
                                }
                                cellVM = cellVM.ParentVM;
                            }
                            if (cellVM.Name == "sfwColumn") {
                                var tableVM = cellVM.ParentVM.ParentVM;
                                var rowVM = cellVM.ParentVM;
                                scope.OnDeleteRowClick(rowVM, tableVM);
                            }
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model.Name == "sfwGridView" && !scope.formodel.IsLookupCriteriaEnabled) {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }, null, null, "Ctrl+Del"],
                    ['Delete Column', function ($itemScope) {
                        if ($itemScope.model) {
                            var cellVM = $itemScope.model;
                            while (cellVM.Name != "sfwColumn") {
                                if (cellVM.ParentVM == null) {
                                    break;
                                }
                                cellVM = cellVM.ParentVM;
                            }
                            if (cellVM.Name == "sfwColumn") {
                                var tableVM = cellVM.ParentVM.ParentVM;
                                scope.OnDeleteColumnClick(cellVM, tableVM);
                            }
                        }
                    }, function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.model.Name == "sfwGridView" && !scope.formodel.IsLookupCriteriaEnabled) {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }, null, null, "Ctrl+Shift+Del"],
                    ['Insert', [
                        ['Row Above ', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnInsertRowAboveClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+Shift+A"],
                        ['Row Below', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnInsertRowBelowClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+Shift+B"],
                        ['Column Left', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnInsertColumnLeftClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+Shift+L"],
                        ['Column Right', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnInsertColumnRightClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+Shift+R"],
                    ], function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }],
                    ['Move', [
                        ['Row Up', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnMoveRowUpClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+U"],
                        ['Row Down', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnMoveRowDownClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+D"],
                        ['Column Left', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnMoveColumnLeftClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+L"],
                        ['Column Right', function ($itemScope) {
                            if ($itemScope.model) {
                                var cellVM = $itemScope.model;
                                while (cellVM.Name != "sfwColumn") {
                                    if (cellVM.ParentVM == null) {
                                        break;
                                    }
                                    cellVM = cellVM.ParentVM;
                                }
                                if (cellVM.Name == "sfwColumn") {
                                    var tableVM = cellVM.ParentVM.ParentVM;
                                    scope.OnMoveColumnRightClick(cellVM, tableVM);
                                }
                            }
                        }, null, null, "Ctrl+R"],
                    ], function ($itemScope) {
                        if (scope.model.Name == 'sfwTable') {
                            return false;
                        }
                        return scope.CanShowContextMenu($itemScope.model, $itemScope.formodel);
                    }],
                    ['Constraint', function ($itemScope) {
                        //Check if entity is set.
                        if (scope.entitytreename) {
                            var entityName = scope.entitytreename;

                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityName, scope.model.dictAttributes.sfwEntityField);
                            if (object) {
                                if (object.DeclaringEntity) {
                                    entityName = object.DeclaringEntity;
                                }
                            }

                            //Check if entity file is already open, then show a message that first close file 'entity name'
                            var entityAlreadyOpened = $rootScope.lstopenedfiles.some(function (file) {
                                if (file.file.FileName == entityName) {
                                    return true;
                                }
                            });

                            if (entityAlreadyOpened) {
                                $SgMessagesService.Message('Close Entity', 'Entity file is currently open. Please close the entity file and try again.');
                            }
                            else {
                                //Check if entity is valid.
                                var entity = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (x) { return x.ID === entityName })[0];
                                if (entity) {
                                    var entityFieldsSplit = scope.model.dictAttributes.sfwEntityField.split(".");
                                    //Check if attribute is belongs to the entity.
                                    var attribute = entity.Attributes.filter(function (x) { return x.ID === entityFieldsSplit[entityFieldsSplit.length - 1]; })[0];
                                    if (attribute) {
                                        if (["column", "property"].indexOf(attribute.Type.toLowerCase()) === -1) {
                                            $SgMessagesService.Message('Invalid Attribute', 'Constraint can be added only for properties and columns.');
                                        }
                                        else {
                                            addEditAttributeConstraint(scope, attribute.ID, null, entityName, true);
                                        }
                                    }
                                    else {
                                        $SgMessagesService.Message('Invalid Attribute', 'Please set the valid attribute for the control.');
                                    }

                                }
                                else {
                                    $SgMessagesService.Message('Invalid Entity', 'Please set the valid entity.');
                                }
                            }
                        }
                        else {
                            $SgMessagesService.Message('Entity Not Set', 'Please set the entity for form.');
                        }
                    }, function ($itemScope) {
                        var retValue = false;
                        if (scope.model.dictAttributes.sfwEntityField && ["sfwGridView", "sfwChart", "sfwCalendar", "sfwScheduler", "sfwListView","sfwCheckBoxList"].indexOf(scope.model.Name) === -1) {
                            retValue = true;
                        }
                        return retValue;
                    }],
                ];

                //#region create compatible label
                scope.OnCreateCompatibleLabelClick = function (obj) {
                    if (obj) {
                        var objCtrl = obj;
                        CreateCompatibleLabel(objCtrl, scope.formodel, $EntityIntellisenseFactory, $rootScope);
                    }
                };

                //#endregion

                scope.OnAddControlToCellClick = function (cellVM, aParam, blnIsGrid) {
                    if (aParam) {
                        var strControlName = aParam;
                        if (strControlName == "sfwButton") {
                            var newScope = scope.$new(true);
                            newScope.item = cellVM;
                            newScope.formodel = scope.formodel;

                            newScope.CreateButtonDialog = $rootScope.showDialog(newScope, "Button Details", "Form/views/CreateButtonWizard/CreateButtonControl.html", { width: 660, height: 550 });

                        }
                        else if (strControlName == "sfwListView") {
                            scope.ErrorMessageForDisplay = "";
                            var strID = CreateControlID(scope.formodel, "RepeaterViewPanel", "sfwListView");
                            var prefix = "swc";
                            scope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
                            scope.ParentEntityName = formodel.dictAttributes.sfwEntity;
                            scope.objRepeaterControl.selectedobjecttreefield;
                            scope.objRepeaterControl.lstselectedobjecttreefields = [];

                            var dialogScope = scope.$new(true);
                            dialogScope.objRepeaterControl = scope.objRepeaterControl;
                            dialogScope.objRepeaterControl.LstDisplayedEntities = [];
                            dialogScope.ParentEntityName = scope.ParentEntityName;
                            dialogScope.RepeaterControldialog = $rootScope.showDialog(dialogScope, "Repeater Control", "Form/views/RepeaterControlTemplate.html", { width: 500, height: 600 });
                        }
                        else {
                            var newScope = scope.$new();
                            AddControlToCell(cellVM, strControlName, undefined, blnIsGrid, scope.formodel, scope.model, $rootScope, newScope, $ValidationService, $GetEntityFieldObjectService);
                        }
                    }
                };
                //#region Change Control
                scope.openChangeControlDialog = function (actionType, cellVM, isGrid) {
                    var dialogScope = scope.$new(true);
                    var model = scope.model;
                    if (cellVM) {
                        model = cellVM;
                    }
                    dialogScope.errorMessage = "";
                    dialogScope.formType = scope.formodel.dictAttributes.sfwType;
                    dialogScope.controlName = "";
                    dialogScope.config = { formModel: scope.formodel, actionType: actionType };
                    dialogScope.lstControls = CONST.FORM.CONTROL_TYPES;
                    dialogScope.lstButtons = angular.copy(scope.buttonsCollection);
                    var dialogTitle = "Add Control";
                    if (actionType == "changecontrol") {
                        var list = dialogScope.lstControls.filter(function (control) {
                            if (control && scope.model && control.method != scope.model.Name) {
                                return control;
                            }
                        });
                        dialogScope.lstControls = list;
                        dialogTitle = "Change Control";
                    }

                    if ((model.IsChildOfGrid || isGrid) && dialogScope.lstControls && dialogScope.lstControls.length > 0) { // alllow controls inside grid
                        var list = dialogScope.lstControls.filter(function (ctrl) { if (ctrl && (ctrl.allowInParent.indexOf("sfwGrid") > -1)) { return ctrl; } });
                        dialogScope.lstControls = list;
                        var objGrid = undefined;
                        if (scope.model && scope.model.Name == "sfwGridView") {
                            objGrid = scope.model;
                        }
                        else {
                            objGrid = FindParent(model, "sfwGridView");
                        }


                        if (dialogScope.formType.toLowerCase() == "lookup" || (objGrid && objGrid.dictAttributes.sfwBoundToQuery && objGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true")) {

                            /* control which are excluding from Lookup Grid/Bind to Query in maintenance .only Non-editable controls are allowed.*/
                            var excludeControlFromLookupGrid = ["sfwCascadingDropDownList", "sfwTextBox", "sfwSwitchCheckBox", "sfwRadioButtonList", "sfwRadioButton", "sfwMultiSelectDropDownList", "sfwDropDownList", "sfwCheckBoxList", "sfwCheckBox", "sfwDateTimePicker"];
                            var list = dialogScope.lstControls.filter(function (ctrl) { if (ctrl && (excludeControlFromLookupGrid.indexOf(ctrl.method) <= -1)) { return ctrl; } });
                            dialogScope.lstControls = list;
                        }
                    } else if (!model.IsChildOfGrid && dialogScope.lstControls && dialogScope.lstControls.length > 0) { // allowed all controls
                        var list = dialogScope.lstControls.filter(function (ctrl) { if (ctrl && (ctrl.allowInParent.indexOf("all") > -1)) { return ctrl; } });
                        dialogScope.lstControls = list;
                    }

                    if (scope.formodel.IsLookupCriteriaEnabled) {
                        if (dialogScope.lstControls && dialogScope.lstControls.length > 0) {
                            var list = dialogScope.lstControls.filter(function (ctrl) { if (ctrl && (ctrl.method != "sfwLabel" && ctrl.method != "sfwToolTipButton")) { return ctrl; } });
                            dialogScope.lstControls = list;
                        }
                    }
                    dialogScope.selectedControl = {};
                    dialogScope.selectedControl.control = null;
                    dialogScope.controltype = "basic";


                    dialogScope.customButton = {};
                    dialogScope.customButton.methodName = "";
                    dialogScope.disableOkBtn = function () {
                        dialogScope.errorMessage = "";
                        if (!dialogScope.selectedControl.control && dialogScope.controltype != "customButton") {
                            return true;
                        } else if (!dialogScope.customButton.methodName && dialogScope.controltype == "customButton") {
                            dialogScope.errorMessage = "Enter method name";
                            return true;
                        } else if (!isValidIdentifier(dialogScope.customButton.methodName, false, false) && dialogScope.controltype == "customButton") {
                            dialogScope.errorMessage = "Invalid method name";
                            return true;
                        }
                        return false;
                    };

                    var getButtonList = function () {
                        dialogScope.lstButtons = angular.copy(scope.buttonsCollection);
                        var btnList = [];
                        if (scope.model.Name == "sfwButton") {
                            btnList = dialogScope.lstButtons.filter(function (btn) {
                                if (btn && scope.model && scope.model.Name == "sfwButton" && btn.Method != scope.model.dictAttributes.sfwMethodName) {
                                    return btn;
                                }
                            });
                        } else {
                            btnList = dialogScope.lstButtons;
                        }
                        return btnList;
                    };
                    dialogScope.$watch('controltype', function (newValue, oldValue) {
                        dialogScope.customButton.methodName = '';
                        dialogScope.selectedControl.control = null;
                        if (newValue != oldValue && newValue == "sfwButton" || newValue == "sfwLinkButton") {
                            dialogScope.lstButtons = getButtonList();
                        } else if (newValue == "sfwImageButton") {
                            dialogScope.lstButtons = getButtonList();
                            var lstRetrieveBtn = dialogScope.lstButtons.filter(function (x) {
                                return x.Method == "btnRetrieve_Click";
                            });
                            if (lstRetrieveBtn && lstRetrieveBtn.length == 0) {
                                dialogScope.lstButtons.push({ "Description": "Retrieve Button", "Method": "btnRetrieve_Click" });
                            }

                        }
                    });
                    dialogScope.selectControlClick = function (objCtrl) {
                        dialogScope.selectedControl.control = objCtrl;
                    };

                    dialogScope.onOkButton = function (control) {
                        if (dialogScope.controltype == "basic") {
                            if (actionType == "addcontrol") {
                                scope.OnAddControlToCellClick(cellVM, control.method, isGrid);
                            } else if (actionType == "changecontrol") {
                                scope.OnChangeControlClick(control.method);
                            }
                        } else if (dialogScope.controltype == "customButton") {
                            if (actionType == "addcontrol") {
                                scope.OnAddControlToCellClick(cellVM, "sfwButton." + dialogScope.customButton.methodName, isGrid);
                            } else if (actionType == "changecontrol") {
                                scope.OnChangeControlClick("sfwButton." + dialogScope.customButton.methodName);
                            }
                        } else if (dialogScope.controltype == "sfwButton" || dialogScope.controltype == "sfwLinkButton" || dialogScope.controltype == "sfwImageButton") {
                            if (actionType == "addcontrol") {
                                scope.OnAddControlToCellClick(cellVM, dialogScope.controltype + "." + control.Method, isGrid);
                            } else if (actionType == "changecontrol") {
                                scope.OnChangeControlClick(dialogScope.controltype + "." + control.Method);
                            }

                        }
                        dialogScope.changeControlDialog.close();
                    };

                    dialogScope.closeDialog = function () {
                        dialogScope.changeControlDialog.close();
                    };

                    dialogScope.changeControlDialog = $rootScope.showDialog(dialogScope, dialogTitle, "Form/views/change_control.html", { width: 500, height: 600 });
                };

                scope.OnChangeControlClick = function (aParam) {
                    if (aParam) {
                        var strOldId = scope.model.dictAttributes.ID;

                        var newCntrl = CreateControl(scope.formodel, scope.model, aParam);

                        persistAttributes(scope.model, newCntrl);

                        if (scope.formodel && scope.formodel.dictAttributes.sfwType === "Lookup") {
                            if (scope.model.Name === "sfwLabel" && scope.model.IsChildOfGrid) {
                                if (scope.model.dictAttributes.sfwRelatedControl) {
                                    scope.model.dictAttributes.sfwRelatedControl = "";
                                }
                            }
                        }

                        if ($rootScope.lstWebControls && $rootScope.lstWebControls.length > 0 && newCntrl.Name && scope.model.dictAttributes && (scope.model.Name != "sfwButton" && scope.model.Name != "sfwLinkButton" && scope.model.Name != "sfwImageButton" && newCntrl.Name != "sfwButton" && newCntrl.Name != "sfwLinkButton" && newCntrl.Name != "sfwImageButton")) {
                            var lstAllPropety = $rootScope.lstWebControls.filter(function (x) {
                                return x.ControlName == newCntrl.Name;
                            });

                            lstAllPropety = JSON.parse(JSON.stringify(lstAllPropety));

                            if (lstAllPropety && lstAllPropety.length > 0 && lstAllPropety[0].lstCustom) {
                                angular.forEach(scope.model.dictAttributes, function (val, key) {

                                    //If an existing checkbox is converted to other control, don't need
                                    //to persist value of sfwIsSwitch attribute.
                                    if (key == "sfwIsSwitch") {
                                        return;
                                    }
                                    var lstCustomPropertyPresent = lstAllPropety[0].lstCustom.filter(function (x) {
                                        return x.PropertyName == key;
                                    });
                                    if (lstCustomPropertyPresent && lstCustomPropertyPresent.length > 0) {
                                        newCntrl.dictAttributes[key] = val;
                                    }
                                    else if (lstAllPropety[0].lstExtra) {
                                        var lstExtraPropertyPresent = lstAllPropety[0].lstExtra.filter(function (x) {
                                            return x.PropertyName == key;
                                        });
                                        if (lstExtraPropertyPresent && lstExtraPropertyPresent.length > 0) {
                                            newCntrl.dictAttributes[key] = val;
                                        }
                                    }
                                });
                                if (newCntrl && (newCntrl.Name == "sfwDropDownList" || newCntrl.Name == "sfwCascadingDropDownList" || newCntrl.Name == "sfwMultiSelectDropDownList" || newCntrl.Name == "sfwListPicker" || newCntrl.Name == "sfwListBox" || newCntrl.Name == "sfwRadioButtonList" || newCntrl.Name == "sfwCheckBoxList" || newCntrl.Name == "sfwSourceList")) {

                                    if (scope.model.Elements && scope.model.Elements.length > 0) {
                                        var lstListItem = scope.model.Elements.filter(function (x) { return x.Name && x.Name == "ListItem"; });
                                        if (lstListItem && lstListItem.length > 0) {
                                            angular.forEach(lstListItem, function (aObjListItem) {
                                                newCntrl.Elements.push(aObjListItem);
                                            });
                                        }
                                    }
                                }
                                if (newCntrl && newCntrl.Name == "sfwCascadingDropDownList") {
                                    if (scope.model.dictAttributes && scope.model.dictAttributes.sfwNavigationParameter) {
                                        newCntrl.dictAttributes.sfwParameters = scope.model.dictAttributes.sfwNavigationParameter;
                                    }
                                    else {
                                        newCntrl.dictAttributes.sfwParameters = "";
                                    }
                                    if (scope.model.dictAttributes && scope.model.dictAttributes.sfwParameters) {
                                        newCntrl.dictAttributes.sfwCascadingRetrievalParameters = scope.model.dictAttributes.sfwParameters;
                                    }
                                    else {
                                        newCntrl.dictAttributes.sfwCascadingRetrievalParameters = "";
                                    }
                                }
                                else if (newCntrl && scope.model.Name == "sfwCascadingDropDownList") {
                                    if (scope.model.dictAttributes) {
                                        var issfwNavigationParameterPresent = lstAllPropety[0].lstCustom.filter(function (x) {
                                            return x.PropertyName == "sfwNavigationParameter";
                                        });
                                        if (issfwNavigationParameterPresent && issfwNavigationParameterPresent.length > 0 && scope.model.dictAttributes.sfwParameters) {
                                            newCntrl.dictAttributes.sfwNavigationParameter = scope.model.dictAttributes.sfwParameters;
                                        }
                                        else {
                                            newCntrl.dictAttributes.sfwNavigationParameter = "";
                                        }
                                    }
                                    if (scope.model.dictAttributes) {
                                        var issfwParametersPresent = lstAllPropety[0].lstCustom.filter(function (x) {
                                            return x.PropertyName == "sfwParameters";
                                        });
                                        if (issfwParametersPresent && issfwParametersPresent.length > 0 && scope.model.dictAttributes.sfwCascadingRetrievalParameters) {
                                            newCntrl.dictAttributes.sfwParameters = scope.model.dictAttributes.sfwCascadingRetrievalParameters;
                                        }
                                        else {
                                            newCntrl.dictAttributes.sfwParameters = "";
                                        }

                                    }
                                }
                                else if (newCntrl && newCntrl.Name == "sfwSoftErrors") {
                                    newCntrl.dictAttributes.sfwEntityField = "InternalErrors";
                                }
                                else if (newCntrl && newCntrl.Name == "sfwEmployerSoftErrors") {
                                    newCntrl.dictAttributes.sfwEntityField = "ExternalErrors";
                                }
                            }

                        }
                        if (!newCntrl) {
                            return;
                        }

                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.DeleteItem(scope.model, scope.model.ParentVM.Elements);
                        scope.ChangeControl(newCntrl);


                        if (scope.model.ParentVM != null) {
                            var objParentVM = scope.model.ParentVM;
                            while (objParentVM.Name != "sfwTable") {
                                if (objParentVM.ParentVM == null) {
                                    break;
                                }
                                objParentVM = objParentVM.ParentVM;
                            }

                            if (objParentVM != null && objParentVM.Name == "sfwTable") {
                                var objAssociatedControl = scope.GetControl(objParentVM, strOldId);
                                if (objAssociatedControl != null) {
                                    objAssociatedControl.dictAttributes.AssociatedControlID = newCntrl.dictAttributes.ID;
                                }
                            }
                        }

                        var objGridView = FindParent(scope.model, "sfwGridView");
                        if (objGridView) {
                            if (scope.formodel && scope.formodel.dictAttributes.sfwType != "Lookup") {

                                if (newCntrl.Name != "sfwLabel" && newCntrl.Name != "sfwButton" && newCntrl.Name != "sfwLinkButton" && newCntrl.Name != "sfwImageButton" && newCntrl.Name !== "sfwButtonGroup") {
                                    $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwTwoWayBinding, objGridView.dictAttributes, "sfwTwoWayBinding", "True");
                                    $rootScope.EditPropertyValue(objGridView.dictAttributes.AllowEditing, objGridView.dictAttributes, "AllowEditing", "True");
                                    $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwCommonFilterBox, objGridView.dictAttributes, "sfwCommonFilterBox", "False");
                                    $rootScope.EditPropertyValue(objGridView.dictAttributes.sfwFilterOnKeyPress, objGridView.dictAttributes, "sfwFilterOnKeyPress", "False");


                                }
                            }
                        }

                        SetFormSelectedControl(scope.formodel, newCntrl);
                        $rootScope.UndRedoBulkOp("End");
                    }
                };

                scope.OnCustomBtnChangeControlClick = function () {
                    var dialogScope = scope.$new(true);
                    dialogScope.button = {};
                    dialogScope.button.methodName = "";
                    dialogScope.errorMessage = "";
                    dialogScope.onBtnOkClick = function () {
                        scope.OnChangeControlClick("sfwButton" + "." + dialogScope.button.methodName);
                        dialogScope.addBtnDialog.close();
                    };
                    dialogScope.validateBtnMethod = function () {
                        dialogScope.errorMessage = "";
                        if (!dialogScope.button.methodName) {
                            dialogScope.errorMessage = "Enter method name";
                            return true;
                        } else if (!isValidIdentifier(dialogScope.button.methodName, false, false)) {
                            dialogScope.errorMessage = "Invalid method name";
                            return true;
                        }
                        return false;
                    };
                    dialogScope.addBtnDialog = $rootScope.showDialog(dialogScope, "Custom Button", "Form/views/AddCustomMethod.html", { width: 300, height: 250 });
                };
                scope.GetControl = function (objTable, aStrId) {
                    var retSfxControl = null;
                    angular.forEach(objTable.Elements, function (objRows) {

                        angular.forEach(objRows.Elements, function (objCell) {

                            angular.forEach(objCell.Elements, function (objsfxControl) {


                                if (objsfxControl.Name == "sfwTable") {
                                    scope.GetControl(objsfxControl, aStrId);
                                }
                                else if (objsfxControl.Name == "sfwPanel") {
                                    angular.forEach(objsfxControl.Elements, function (objPanelTable) {
                                        if (objPanelTable.Name == "sfwTable") {
                                            scope.GetControl(objPanelTable, aStrId);
                                        }
                                    });
                                }
                                else if (objsfxControl.dictAttributes.AssociatedControlID && aStrId &&
                                    objsfxControl.dictAttributes.AssociatedControlID.toLowerCase() == aStrId.toLowerCase()) {
                                    retSfxControl = objsfxControl;
                                }
                            });
                        });
                    });
                    return retSfxControl;
                };


                scope.ChangeControl = function (newCntrl) {

                    if (newCntrl.Name == "sfwDialogPanel" || newCntrl.Name == "sfwPanel" || newCntrl.Name == "sfwListView") {
                        newCntrl.initialvisibilty = true;
                        newCntrl.isLoaded = true;
                    }

                    $rootScope.PushItem(newCntrl, scope.model.ParentVM.Elements);

                };

                //#endregion



                //#region Cut/Copy/Delete/Change Control

                scope.OnPasteControl = function (currmodel) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.OnPasteControl) {
                        filescope.OnPasteControl(currmodel);
                    }

                };
                scope.OnDeleteControlClick = function (aParam) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.OnDeleteControlClick) {
                        filescope.OnDeleteControlClick(aParam);
                    }

                };

                scope.OnCutControlClick = function (currmodel) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.OnCutControlClick) {
                        filescope.OnCutControlClick(currmodel);
                    }

                };

                scope.OnCopyControlClick = function (currmodel) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.OnCopyControlClick) {
                        filescope.OnCopyControlClick(currmodel);
                    }

                };
                //#endregion



                //#region Clear Cell / Grid



                scope.OnClearGridClick = function (tableVM) {
                    OnClearGridClick(tableVM, $rootScope);
                };

                // #endregion

                //#region Insert  Row/Column
                scope.OnInsertRowAboveClick = function (cellVM, tableVM) {
                    OnInsertRowAboveClick(cellVM, tableVM, $rootScope)
                };

                scope.OnInsertRowBelowClick = function (cellVM, tableVM) {
                    OnInsertRowBelowClick(cellVM, tableVM, $rootScope);
                };

                scope.OnInsertColumnLeftClick = function (cellVM, tableVM) {
                    OnInsertColumnLeftClick(cellVM, tableVM);
                };

                scope.OnInsertColumnRightClick = function (cellVM, tableVM) {
                    OnInsertColumnRightClick(cellVM, tableVM);
                };

                //#endregion

                //#region Move Row/Column

                scope.OnMoveRowUpClick = function (aParm, tableVM) {
                    if (null != tableVM) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.MoveRowUp) {
                            filescope.MoveRowUp(aParm, tableVM);
                        }
                    }
                };

                scope.OnMoveRowDownClick = function (aParm, tableVM) {
                    if (tableVM) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.MoveRowDown) {
                            filescope.MoveRowDown(aParm, tableVM);
                        }
                    }
                };


                scope.OnMoveColumnLeftClick = function (aParm, tableVM) {
                    if (tableVM) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.MoveColumnLeft) {
                            filescope.MoveColumnLeft(aParm, tableVM);
                        }
                    }
                };

                scope.OnMoveColumnRightClick = function (aParm, tableVM) {
                    if (tableVM) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.MoveColumnRight) {
                            filescope.MoveColumnRight(aParm, tableVM);
                        }
                    }

                };

                //#endregion

                //#region Delecte Row/Column

                scope.OnDeleteRowClick = function (rowVM, tableVM) {
                    OnDeleteRowClick(rowVM, tableVM, $rootScope, $SgMessagesService);
                };

                scope.OnDeleteColumnClick = function (aParam, tableVM) {
                    OnDeleteColumnClick(aParam, tableVM, $rootScope, scope.formodel, $SgMessagesService);
                };


                //#endregion

                //#region Add Grid Columns
                scope.OnAddGridColumns = function () {
                    var newScope = scope.$new();
                    newScope.objGridView = scope.model;
                    newScope.FormModel = scope.formodel;
                    newScope.IsAddColumnSelected = true;
                    var objAttr;
                    if (newScope.FormModel.dictAttributes.sfwType == "Lookup") {
                        newScope.ParentEntityName = newScope.FormModel.dictAttributes.sfwEntity;
                    }
                    //commented below code, because we don't need to check all these conditions as scope.entitytreename is always updated to GridView collection entity.
                    //So only checking if GridView sfwEntityField is set or not and scope.entitytreename is set or not.
                    else if (scope.model.dictAttributes.sfwEntityField && scope.entitytreename) {
                        newScope.ParentEntityName = scope.entitytreename;

                        //if (scope.model.Name == "sfwGridView" && scope.model.dictAttributes.sfwParentGrid && scope.entitytreename) {
                        //    objAttr = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.entitytreename, scope.model.dictAttributes.sfwEntityField);
                        //}
                        //else {
                        //    var parentListView = FindParent(scope.model, "sfwListView");
                        //    if (parentListView) {
                        //        objAttr = scope.entitytreename;
                        //    }
                        //    else if (scope.model.dictAttributes.sfwEntityField && scope.formodel.dictAttributes.sfwEntity) {
                        //        objAttr = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formodel.dictAttributes.sfwEntity, scope.model.dictAttributes.sfwEntityField);
                        //    }
                        //}
                    }
                    //if (objAttr) {
                    //    if (typeof objAttr === "string") {
                    //        newScope.ParentEntityName = objAttr;
                    //    }
                    //    else if (typeof objAttr === "object" && objAttr.hasOwnProperty("Entity")) {
                    //        newScope.ParentEntityName = objAttr.Entity;
                    //    }
                    //}
                    newScope.objGridView.LstDisplayedEntities = [];
                    newScope.objGridView.lstselectedmultiplelevelfield = [];
                    newScope.objGridView.selectedentityobjecttreefields = [];
                    newScope.title = "Create New Grid";
                    newScope.LstDisplayedEntities = scope.lstdisplayentities;
                    if (newScope.FormModel && newScope.FormModel.dictAttributes.ID.startsWith("wfp")) {
                        newScope.IsPrototype = true;
                    }
                    else {
                        newScope.IsPrototype = false;
                    }
                    newScope.objGridDialog = $rootScope.showDialog(newScope, newScope.title, "Form/views/CreateGridViewControl.html", { width: 1000, height: 700 });
                    newScope.setTitle = function (title) {
                        if (newScope.title) {
                            newScope.title = title;
                            newScope.objGridDialog.updateTitle(newScope.title);
                        }
                    };
                };
                //#endregion

                scope.CanShowContextMenu = function (model, formodel) {
                    return CanShowContextMenu(model, formodel);
                }

            }
        }
    }
}]);

app.directive("tableControlDirective", ["$compile", "$rootScope", "$EntityIntellisenseFactory", "$ValidationService", "$GetEntityFieldObjectService", "$SgMessagesService", function ($compile, $rootScope, $EntityIntellisenseFactory, $ValidationService, $GetEntityFieldObjectService, $SgMessagesService) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',

            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/TableControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {

            scope.blnIsTableSelected = false;
            if (scope.formodel && !scope.formodel.SelectedControl) {
                for (var i = 0; i < scope.formodel.Elements.length; i++) {
                    if (scope.formodel.Elements[i].Name == "sfwTable") {
                        for (var j = 0; j < scope.formodel.Elements[i].Elements.length; j++) {
                            if (scope.formodel.Elements[i].Elements[j].Name == "sfwRow") {
                                if (scope.formodel.Elements[i].Elements[j].Elements.length > 0) {
                                    selectControl(scope.formodel.Elements[i].Elements[j].Elements[0], null, scope.formodel);
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            scope.SetParentVM = function (parent, model) {
                model.ParentVM = parent;

            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                if (objChild && objChild.Name != "sfwColumn" && objChild.Name != "sfwButtonGroup") {
                    scope.blnIsTableSelected = !scope.blnIsTableSelected;
                }
                selectControl(objChild, event, scope.formodel);
            };

            scope.AddRowsAndColumns = function (objchild, event) {
                scope.blnIsTableSelected = true;
                event.stopPropagation();
                var filescope = getCurrentFileScope();
                if (filescope && filescope.AddRowsAndColumns) {
                    filescope.AddRowsAndColumns(objchild, event);
                }
            }

            scope.tdMouseEnterEvent = function (e) {
                scope.startEvent = e;
                $(".row-col").remove();
                var tdScope = angular.element(scope.startEvent.currentTarget).scope();
                var colHeight = scope.startEvent.currentTarget[0] && scope.startEvent.currentTarget[0].offsetHeight;
                var divs = "";
                var position = $(scope.startEvent.currentTarget).attr('cellposition');
                if (position == "top") {
                    divs = $compile("<div class='row-col left-top-col-div' ng-mouseover='addColLeft()'></div><div class='row-col right-top-col-div' ng-mouseover='addColRight()'></div>")(scope);
                }
                if (position == "left") {
                    divs = $compile("<div class='row-col left-top-row-div' ng-mouseover='addRowTop()'></div><div class='row-col left-bottom-row-div' ng-mouseover='addRowBottom()'></div>")(scope);
                }

                $(scope.startEvent.currentTarget).append(divs);
                scope.cellObj = null;
                if (position == "left") {
                    if (scope.startEvent.target.nodeName.toLowerCase() != "td") {
                        scope.cellObj = $(scope.startEvent.target).parents('td').first().siblings().first();
                    } else {
                        scope.cellObj = $(scope.startEvent.target).siblings().first();
                    }
                } else {
                    if (scope.startEvent.target.nodeName.toLowerCase() != "td") {
                        var td = $(scope.startEvent.target).closest('td');
                        var index = $(td).index();
                        scope.cellObj = $(td).closest('tr').siblings().first().children().eq(index);
                    } else {
                        var td = $(scope.startEvent.target);
                        var index = $(td).index();
                        scope.cellObj = $(td).closest('tr').siblings().first().children().eq(index);
                    }
                }
            };

            //#region add row and column 

            scope.addRowTop = function () {
                addCell("addRowTop");
            };
            scope.addRowBottom = function () {
                addCell("addRowBottom");
            };
            scope.addColLeft = function () {
                addCell("addColumnLeft");
            };
            scope.addColRight = function () {
                addCell("addColumnRight");
            };

            // #region add row or column on td hovering 
            var addCell = function (action) {
                if (scope.startEvent) {
                    clearTimeout($rootScope.userIdleTimeout);


                    if ($('.sgt-wrapper').length > 0) {
                        $('#temprow').remove();
                        $('.sgt-wrapper').remove();
                    }

                    var parentNode = $(scope.startEvent.target).closest('tr')[0];

                    var rowCount = 0, columnCount = 0;
                    if (parentNode && (parentNode.nodeName == 'TR' || parentNode.nodeName == 'tr')) {
                        rowCount = parentNode.cells.length;
                    }

                    var row_index = $(scope.startEvent.currentTarget).parent().index();
                    var col_index = $(scope.startEvent.currentTarget).index();


                    var tbody = $(parentNode).closest('tbody');

                    if (tbody && tbody[0]) {
                        columnCount = tbody[0].rows.length;
                    }

                    var newRow = '<tr id="temprow">';
                    for (var i = 0; i < rowCount; i++) {
                        if (i == 0) {
                            newRow += '<td class="sgt-wrapper row-wrap" style="padding:0px;text-align: center;height:20px!important;border:0!important;" title=" Click here to add row "><span style="position:relative;mouse:pointer;height:100%;width:100%" class="sgt-wrapper glyphicon glyphicon-plus-sign"></span>  </td>';
                        } else {
                            newRow += '<td class="sgt-wrapper row-wrap" style="padding:0px;text-align: center;height:20px!important;" title=" Click here to add row "> </td>';
                        }
                    }
                    newRow += '</tr>';

                    var newCol = "";

                    var newCol1 = "<td class='sgt-wrapper col-wrap'  style='width:20px;border:0!important;padding:0' title=' Click here to add Column '><span style='position:relative;mouse:pointer;height:100%;width:100%;margin-left:2px;padding:0' class='sgt-wrapper glyphicon glyphicon-plus-sign'></span>   </td>";
                    newCol += "<td class='sgt-wrapper col-wrap'  style='width:20px;' title=' Click here to add Column '>  </td>";


                    if (action == "addRowTop") {
                        $(parentNode).closest('table > tbody > tr:nth-child(' + (row_index + 1) + ')').before(newRow);
                        $('.sgt-wrapper').addClass("td-fade-in");

                        $(".row-wrap").on("click", function () {
                            $('.sgt-wrapper.row-wrap').parent("tr").remove();
                            $('.sgt-wrapper').remove();
                            if (scope.cellObj) var currScope = angular.element(scope.cellObj).scope();

                            var cellVM = currScope && currScope.col ? currScope.col : null;
                            var tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;

                            if (cellVM && tableVM) {
                                scope.OnInsertRowAboveClick(cellVM, tableVM);
                            }

                        });
                    }
                    if (action == "addRowBottom") {
                        $(parentNode).closest('table > tbody > tr:nth-child(' + (row_index + 1) + ')').after(newRow);
                        $('.sgt-wrapper').addClass("td-fade-in");

                        $(".row-wrap").on("click", function () {
                            $('.sgt-wrapper.row-wrap').parent("tr").remove();
                            $('.sgt-wrapper').remove();
                            if (scope.cellObj) var currScope = angular.element(scope.cellObj).scope();

                            var cellVM = currScope && currScope.col ? currScope.col : null;
                            var tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;

                            if (cellVM && tableVM) {
                                scope.OnInsertRowBelowClick(cellVM, tableVM);
                            }

                        });
                    }
                    if (action == "addColumnRight") {
                        $(parentNode).children('td:eq(' + (col_index) + ')').after(newCol1);
                        $(parentNode).siblings('tr').each(function () {
                            $(this).children('td:eq(' + (col_index) + ')').after(newCol);

                        });
                        $('.sgt-wrapper').addClass("td-fade-in");
                        $(".col-wrap").on("click", function () {
                            $('.sgt-wrapper.row-wrap').parent("tr").remove();
                            $('.sgt-wrapper').remove();

                            if (scope.cellObj) var currScope = angular.element(scope.cellObj).scope();

                            var cellVM = currScope && currScope.col ? currScope.col : null;
                            var tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;


                            if (cellVM && tableVM) {
                                scope.OnInsertColumnRightClick(cellVM, tableVM);
                            }

                        });
                    }

                    if (action == "addColumnLeft") {


                        $(parentNode).children('td:eq(' + (col_index) + ')').before(newCol1);
                        $(parentNode).siblings('tr').each(function () {
                            $(this).children('td:eq(' + (col_index) + ')').before(newCol);
                        });
                        $('.sgt-wrapper').addClass("td-fade-in");
                        $(".col-wrap").on("click", function () {
                            $('.sgt-wrapper.row-wrap').parent("tr").remove();
                            $('.sgt-wrapper').remove();


                            if (scope.cellObj) var currScope = angular.element(scope.cellObj).scope();

                            var cellVM = currScope && currScope.col ? currScope.col : null;
                            var tableVM = cellVM && cellVM.ParentVM ? cellVM.ParentVM.ParentVM : null;

                            if (cellVM && tableVM) {
                                scope.OnInsertColumnLeftClick(cellVM, tableVM);
                            }
                        });
                    }


                }
                // clearTimeout($rootScope.userIdleTimeout);
            };

            scope.$on('$destroy', function () {
                clearTimeout($rootScope.userIdleTimeout);
            });

            //#endregion

            scope.tdKeyDown = function (aobjSelectedCell, event) {
                if (aobjSelectedCell && !event.ctrlKey && !event.shiftKey && !event.altKey && event.which == $.ui.keyCode.DELETE && scope.formodel.SelectedControl && scope.formodel.SelectedControl.Name == "sfwColumn" && scope.formodel.SelectedControl.Elements.length > 0) {
                    $rootScope.UndRedoBulkOp("Start");
                    for (i = (scope.formodel.SelectedControl.Elements.length - 1); i > -1; i--) {
                        $rootScope.DeleteItem(scope.formodel.SelectedControl.Elements[i], scope.formodel.SelectedControl.Elements);
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
            }

            scope.SetColSpan = function (aobjcol) {
                return aobjcol.dictAttributes.ColumnSpan;
            };

            scope.SetRowSpan = function (aobjcol) {
                return aobjcol.dictAttributes.RowSpan;
            };

            //#region Insert  Row/Column
            scope.OnInsertRowAboveClick = function (cellVM, tableVM) {
                OnInsertRowAboveClick(cellVM, tableVM, $rootScope)
            };

            scope.OnInsertRowBelowClick = function (cellVM, tableVM) {
                OnInsertRowBelowClick(cellVM, tableVM, $rootScope);
            };

            scope.OnInsertColumnLeftClick = function (cellVM, tableVM) {
                OnInsertColumnLeftClick(cellVM, tableVM);
            };

            scope.OnInsertColumnRightClick = function (cellVM, tableVM) {
                OnInsertColumnRightClick(cellVM, tableVM);
            };

            //#endregion

            //#region Move Row/Column

            scope.OnMoveRowUpClick = function (aParm, tableVM) {
                if (null != tableVM) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.MoveRowUp) {
                        filescope.MoveRowUp(aParm, tableVM);
                    }
                }
            };

            scope.OnMoveRowDownClick = function (aParm, tableVM) {
                if (tableVM) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.MoveRowDown) {
                        filescope.MoveRowDown(aParm, tableVM);
                    }
                }
            };

            scope.OnMoveColumnLeftClick = function (aParm, tableVM) {
                if (tableVM) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.MoveColumnLeft) {
                        filescope.MoveColumnLeft(aParm, tableVM);
                    }
                }
            };

            scope.OnMoveColumnRightClick = function (aParm, tableVM) {
                if (tableVM) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.MoveColumnRight) {
                        filescope.MoveColumnRight(aParm, tableVM);
                    }
                }

            };

            //#endregion

            //#region Cut/Copy/Paste Cell

            scope.OnCutCell = function (cellVM) {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.OnCutCell) {
                    filescope.OnCutCell(cellVM);
                }
            };

            scope.OnCopyCell = function (cellVM) {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.OnCopyCell) {
                    filescope.OnCopyCell(cellVM);
                }
            };

            scope.OnPasteControl = function (cellVM) {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.OnPasteControl) {
                    filescope.OnPasteControl(cellVM);
                }

            };

            scope.OnPasteCell = function (cellVM) {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.OnPasteCell) {
                    filescope.OnPasteCell(cellVM);
                }

            };

            //#endregion

            //#region Add New Control
            var RepeaterControldialog;
            scope.objRepeaterControl;
            scope.ErrorMessageForDisplay = "";
            scope.onRepeaterControlOkClick = function () {
                if (scope.objRepeaterControl) {
                    scope.objRepeaterControl.dictAttributes.sfwSelection = "Many";
                    scope.objRepeaterControl.dictAttributes.sfwCaption = "List View";
                    scope.objRepeaterControl.dictAttributes.AllowPaging = "True";
                    scope.objRepeaterControl.dictAttributes.PageSize = "1";

                    var selectedField = scope.objRepeaterControl.selectedobjecttreefield;

                    if (selectedField) {
                        var displayEntity = getDisplayedEntity(scope.objRepeaterControl.LstDisplayedEntities);
                        var displayName = displayEntity.strDisplayName;
                        fieldName = selectedField.ID;
                        if (displayName != "") {
                            fieldName = displayName + "." + selectedField.ID;
                        }
                        var entitycollname = fieldName; //GetItemPathForEntityObject(selectedField);
                        scope.objRepeaterControl.dictAttributes.sfwEntityField = entitycollname;
                        var parentenetityname = selectedField.Entity;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        scope.objRepeaterControl.dictAttributes.sfwDataKeyNames = GetTableKeyFields(parentenetityname, entityIntellisenseList);
                    }

                    var prefix = "swc";

                    var objListTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    objListTableModel.ParentVM = scope.objRepeaterControl;
                    var strCtrlId = CreateControlID(scope.formodel, "NewPage", "sfwTable");
                    objListTableModel.dictAttributes.ID = strCtrlId;

                    var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxRowModel.ParentVM = objListTableModel;

                    var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    objListTableModel.Elements.push(sfxRowModel);
                    scope.objRepeaterControl.Elements.push(objListTableModel);
                    scope.objRepeaterControl.initialvisibilty = true;
                    scope.objRepeaterControl.isLoaded = true;
                    $rootScope.PushItem(scope.objRepeaterControl, scope.dropdata.Elements);
                }
                scope.onRepeaterControlCancelClick();
            };
            scope.onRepeaterControlCancelClick = function () {
                RepeaterControldialog.close();
            };

            //#endregion

            //#region Delecte Row/Column

            scope.OnDeleteRowClick = function (rowVM, tableVM) {
                OnDeleteRowClick(rowVM, tableVM, $rootScope, $SgMessagesService);
            };

            scope.OnDeleteColumnClick = function (aParam, tableVM) {
                OnDeleteColumnClick(aParam, tableVM, $rootScope, scope.formodel, $SgMessagesService);
            };

            //#endregion

            //#region Clear Cell / Grid

            scope.OnClearGridClick = function (tableVM) {
                OnClearGridClick(tableVM, $rootScope);
            };
            //#endregion

            scope.CanShowContextMenu = function (model, formodel) {
                return CanShowContextMenu(model, formodel);
            }
            scope.menuOptions = [
                ['Insert', [
                    ['Row Above', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnInsertRowAboveClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+Shift+A"],
                    ['Row Below', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnInsertRowBelowClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+Shift+B"],
                    ['Column Left', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnInsertColumnLeftClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+Shift+L"],
                    ['Column Right', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnInsertColumnRightClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+Shift+R"],
                    ['Cell Right', function ($itemScope) {
                        if ($itemScope.col) {
                            var rowVM = $itemScope.col.ParentVM;
                            var filescope = getCurrentFileScope();
                            if (filescope && filescope.InsertCell) {
                                filescope.InsertCell(rowVM, $itemScope.col, true);
                            }
                        }
                    }, null, null, "Ctrl+Alt+R"],
                    ['Cell Left', function ($itemScope) {
                        if ($itemScope.col) {

                            var rowVM = $itemScope.col.ParentVM;
                            var filescope = getCurrentFileScope();
                            if (filescope && filescope.InsertCell) {
                                filescope.InsertCell(rowVM, $itemScope.col, false);
                            }


                        }
                    }, null, null, "Ctrl+Alt+L"],
                ], function ($itemScope) {
                    if ($itemScope.col && $itemScope.formodel && scope.selectControl) {
                        scope.selectControl($itemScope.col, event, $itemScope.formodel);
                    }
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }], null,
                ['Move', [
                    ['Row Up', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnMoveRowUpClick(cellVM, tableVM);
                        }

                    }, null, null, "Ctrl+U"],
                    ['Row Down', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnMoveRowDownClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+D"],
                    ['Column Left', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnMoveColumnLeftClick(cellVM, tableVM);
                        }
                    }, null, null, "Ctrl+L"],
                    ['Column Right', function ($itemScope) {
                        if ($itemScope.col) {
                            var cellVM = $itemScope.col;
                            var tableVM = cellVM.ParentVM.ParentVM;
                            scope.OnMoveColumnRightClick(cellVM, tableVM);
                        }

                    }, null, null, "Ctrl+R"],
                ], function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }], null,

                ['Add Control', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        scope.$parent.openChangeControlDialog("addcontrol", cellVM);
                    }

                    scope.selectControl($itemScope.col, event, $itemScope.formodel);
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }], null,
                ['Cut Cell', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        scope.OnCutCell(cellVM);
                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, "Ctrl+X"], null,
                ['Copy Cell', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        scope.OnCopyCell(cellVM);
                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, "Ctrl+C"], null,
                ['Paste Cell', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        scope.OnPasteCell(cellVM);
                    }
                }, function ($itemScope) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.ClipboardData && filescope.ClipboardDataOpeType == 'Cell') {
                        return true;
                    }
                }, null, "Ctrl+V"], null,
                ['Delete Cell', function ($itemScope) {
                    if ($itemScope.col) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.MoveRowUp) {
                            filescope.deleteCell($itemScope.col);
                        }
                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, null, "Alt+Del"], null,
                ['Paste Control', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        scope.OnPasteControl(cellVM);
                    }
                }, function ($itemScope) {
                    var filescope = getCurrentFileScope();
                    if (filescope && filescope.ClipboardData && filescope.ClipboardDataOpeType == "Control") {
                        return true;
                    }
                }, null, "Ctrl+V"], null,
                ['Clear Cell', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        ClearCell(cellVM, $rootScope, true);

                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }], null,
                ['Clear Panel', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        var tableVM = cellVM.ParentVM.ParentVM;
                        scope.OnClearGridClick(tableVM);
                    }
                }, function ($itemScope) {
                    if ($itemScope.formodel && $itemScope.formodel.dictAttributes.sfwType == "Lookup" && !$itemScope.formodel.IsLookupCriteriaEnabled) {
                        return false;
                    }
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, "Shift+Del"], null,
                ['Delete Row', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        var tableVM = cellVM.ParentVM.ParentVM;
                        var rowVM = cellVM.ParentVM;
                        scope.OnDeleteRowClick(rowVM, tableVM);
                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, "Ctrl+Del"], null,
                ['Delete Column', function ($itemScope) {
                    if ($itemScope.col) {
                        var cellVM = $itemScope.col;
                        var tableVM = cellVM.ParentVM.ParentVM;
                        scope.OnDeleteColumnClick(cellVM, tableVM);
                    }
                }, function ($itemScope) {
                    return scope.CanShowContextMenu($itemScope.col, $itemScope.formodel);
                }, null, "Ctrl+Shift+Del"], null

            ];

        }
    }
}]);

app.directive("tabcontainerControlDirective", ["$compile", "$rootScope", "$SgMessagesService", function ($compile, $rootScope, $SgMessagesService) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',

            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/TabcontainerControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {
            if (scope.model.Name == "sfwTabContainer") {
                if (scope.model.Elements[0] && scope.model.Elements[0].Elements.length > 0 && !$rootScope.isFromSource) {
                    scope.model.Elements[0].ParentVM = scope.model;
                    if (!scope.model.Elements[0].SelectedTabSheet) {
                        scope.model.Elements[0].SelectedTabSheet = scope.model.Elements[0].Elements[0];
                        scope.model.Elements[0].SelectedTabSheet.IsSelected = true;
                        scope.model.Elements[0].Elements.every(function (item) {
                            item.isLoaded = false;
                        });
                        scope.model.Elements[0].Elements[0].isLoaded = true;
                    }
                }
            }

            scope.SetParentVM = function (parent, model) {
                model.ParentVM = parent;
            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
            };

            //#region Methods for Add/Remove Tabs From Tab Container 
            scope.AddNewTabClick = function (objTabs) {

                if (objTabs) {
                    $rootScope.UndRedoBulkOp("Start");
                    var tabsheetId = CreateControlID(scope.formodel, "NewPage", "sfwTabSheet");
                    var newTabSheetModel = { Name: 'sfwTabSheet', Value: '', prefix: 'swc', dictAttributes: { ID: tabsheetId, HeaderText: "New Page" }, Elements: [], Children: [] };
                    newTabSheetModel.ParentVM = objTabs;
                    newTabSheetModel.isLoaded = true;
                    var newSfxTableModel = { Name: 'sfwTable', Value: '', prefix: 'swc', dictAttributes: { ID: '' }, Elements: [], Children: [] };
                    newSfxTableModel.dictAttributes.ID = CreateControlID(scope.formodel, "Table", "sfwTable", false);
                    newSfxTableModel.ParentVM = newTabSheetModel;

                    $rootScope.PushItem(newSfxTableModel, newTabSheetModel.Elements);

                    var newSfxRowModel = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxRowModel.ParentVM = newSfxTableModel;

                    var newSfxCellModel = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = newSfxRowModel;
                    $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

                    newSfxCellModel = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = newSfxRowModel;
                    $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);


                    newSfxCellModel = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = newSfxRowModel;
                    $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

                    newSfxCellModel = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = newSfxRowModel;
                    $rootScope.PushItem(newSfxCellModel, newSfxRowModel.Elements);

                    $rootScope.PushItem(newSfxRowModel, newSfxTableModel.Elements);

                    $rootScope.PushItem(newTabSheetModel, objTabs.Elements);
                    $rootScope.UndRedoBulkOp("End");
                    SetFormSelectedControl(scope.formodel, newTabSheetModel);
                    objTabs.SelectedTabSheet = newTabSheetModel;
                    objTabs.SelectedTabSheet.IsSelected = true;
                    //tabobj = newTabSheetModel;
                }
            };

            scope.DeleteNewTabClick = function (objTabs, objTabSheet) {
                if (objTabSheet) {
                    if (objTabs) {
                        $SgMessagesService.Message('Delete Tab Sheet', 'Do you want to Delete Selected Tab Sheet ?', true, function (action) {
                            if (action) {
                                $rootScope.UndRedoBulkOp("Start");
                                var index = objTabs.Elements.indexOf(objTabSheet);
                                $rootScope.DeleteItem(objTabSheet, objTabs.Elements);
                                if (objTabs.Elements.length > 0) {
                                    index--;
                                    if (index < 0) {
                                        index = 0;
                                    }

                                    SetFormSelectedControl(scope.formodel, objTabs.Elements[index]);
                                    objTabs.SelectedTabSheet = objTabs.Elements[index];
                                    objTabs.SelectedTabSheet.IsSelected = true;
                                    //tabobj = objTabs.Elements[index];
                                }
                                else {
                                    var objTabContainer = FindParent(scope.formodel.SelectedControl, "sfwTabContainer", true);
                                    if (objTabContainer && objTabContainer.ParentVM) {
                                        $rootScope.DeleteItem(objTabContainer, objTabContainer.ParentVM.Elements);
                                    }
                                }
                                $rootScope.UndRedoBulkOp("End");
                            }
                        });

                    }
                }
            };

            //#endregion

            scope.getTabID = function (item, ishash) {
                if (ishash) {
                    return "#" + item.dictAttributes.ID;
                }
                return item.dictAttributes.ID;
            };

            //#region tabsheet menu Option

            scope.tabsheetmenuOptions = [
                ['Move', [

                    ['Move Tabsheet Left', function ($itemScope) {

                        if ($itemScope.tabs && $itemScope.tabs.Name == "sfwTabSheet" && $itemScope.tabs.ParentVM.Elements.length > 0) {
                            var ColIndex = $itemScope.tabs.ParentVM.Elements.indexOf($itemScope.tabs);
                            if (ColIndex > 0) {
                                var model = $itemScope.tabs.ParentVM.Elements[ColIndex];
                                if (model) {
                                    $rootScope.UndRedoBulkOp("Start");
                                    //Removing
                                    $rootScope.DeleteItem($itemScope.tabs.ParentVM.Elements[ColIndex], $itemScope.tabs.ParentVM.Elements);
                                    //Adding
                                    $rootScope.InsertItem(model, $itemScope.tabs.ParentVM.Elements, ColIndex - 1);
                                    $rootScope.UndRedoBulkOp("End");
                                }
                            }
                        }

                    }, function ($itemScope) {
                        return true;
                    }, null],
                    ['Move Tabsheet Right', function ($itemScope) {
                        if ($itemScope.tabs && $itemScope.tabs.Name == "sfwTabSheet" && $itemScope.tabs.Elements.length > 0) {
                            var ColCount = $itemScope.tabs.ParentVM.Elements.length;
                            var ColIndex = $itemScope.tabs.ParentVM.Elements.indexOf($itemScope.tabs);
                            if (ColIndex < ColCount - 1) {

                                var model = $itemScope.tabs.ParentVM.Elements[ColIndex];
                                if (model) {
                                    $rootScope.UndRedoBulkOp("Start");
                                    //Removing
                                    $rootScope.DeleteItem($itemScope.tabs.ParentVM.Elements[ColIndex], $itemScope.tabs.ParentVM.Elements);
                                    //Adding
                                    $rootScope.InsertItem(model, $itemScope.tabs.ParentVM.Elements, ColIndex + 1);
                                    $rootScope.UndRedoBulkOp("End");
                                }
                            }
                        }

                    }, function ($itemScope) {
                        scope.selectControl($itemScope.tabs, event, scope.formodel);
                        return true;
                    }, null],
                ], function ($itemScope) {
                    return true;
                }],
            ];
            //#endregion

        }
    }
}]);

app.directive("gridControlDirective", ["$compile", "$rootScope", "$ValidationService", "$GetEntityFieldObjectService", "$SgMessagesService", function ($compile, $rootScope, $ValidationService, $GetEntityFieldObjectService, $SgMessagesService) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',

            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/GridControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
            };

            scope.getHeaderColspan = function (item) {
                if (item.Elements.length > 0) {
                    var columns = item.Elements[0];
                    return columns.Elements.length;
                }
                else {
                    return 1;
                }
            };

            scope.getValue = function (item) {
                return getDisplayValue(item);
            };

            //#region Add New Control
            var RepeaterControldialog;
            scope.objRepeaterControl;
            scope.ErrorMessageForDisplay = "";

            scope.OnAddControlToCellClick = function (cellVM, aParam, blnIsGrid) {
                if (aParam) {
                    var strControlName = aParam;
                    if (strControlName == "sfwButton") {
                        var newScope = scope.$new(true);
                        newScope.item = cellVM;
                        newScope.formodel = scope.formodel;

                        newScope.CreateButtonDialog = $rootScope.showDialog(newScope, "Button Details", "Form/views/CreateButtonWizard/CreateButtonControl.html", { width: 660, height: 550 });

                    }
                    else if (strControlName == "sfwListView") {
                        scope.ErrorMessageForDisplay = "";
                        var strID = CreateControlID(scope.formodel, "RepeaterViewPanel", "sfwListView");
                        var prefix = "swc";
                        scope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
                        scope.ParentEntityName = formodel.dictAttributes.sfwEntity;
                        scope.objRepeaterControl.selectedobjecttreefield;
                        scope.objRepeaterControl.lstselectedobjecttreefields = [];

                        var dialogScope = scope.$new(true);
                        dialogScope.objRepeaterControl = scope.objRepeaterControl;
                        dialogScope.objRepeaterControl.LstDisplayedEntities = [];
                        dialogScope.ParentEntityName = scope.ParentEntityName;
                        dialogScope.RepeaterControldialog = $rootScope.showDialog(dialogScope, "Repeater Control", "Form/views/RepeaterControlTemplate.html", { width: 500, height: 600 });
                    }
                    else {
                        var newScope = scope.$new();
                        AddControlToCell(cellVM, strControlName, undefined, blnIsGrid, scope.formodel, scope.model, $rootScope, newScope, $ValidationService, $GetEntityFieldObjectService);
                    }
                }
            };

            scope.onRepeaterControlOkClick = function () {
                if (scope.objRepeaterControl) {
                    scope.objRepeaterControl.dictAttributes.sfwSelection = "Many";
                    scope.objRepeaterControl.dictAttributes.sfwCaption = "List View";
                    scope.objRepeaterControl.dictAttributes.AllowPaging = "True";
                    scope.objRepeaterControl.dictAttributes.PageSize = "1";

                    var selectedField = scope.objRepeaterControl.selectedobjecttreefield;

                    if (selectedField) {
                        var displayEntity = getDisplayedEntity(scope.objRepeaterControl.LstDisplayedEntities);
                        var displayName = displayEntity.strDisplayName;
                        fieldName = selectedField.ID;
                        if (displayName != "") {
                            fieldName = displayName + "." + selectedField.ID;
                        }
                        var entitycollname = fieldName; //GetItemPathForEntityObject(selectedField);
                        scope.objRepeaterControl.dictAttributes.sfwEntityField = entitycollname;
                        var parentenetityname = selectedField.Entity;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        scope.objRepeaterControl.dictAttributes.sfwDataKeyNames = GetTableKeyFields(parentenetityname, entityIntellisenseList);
                    }

                    var prefix = "swc";

                    var objListTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    objListTableModel.ParentVM = scope.objRepeaterControl;
                    var strCtrlId = CreateControlID(scope.formodel, "NewPage", "sfwTable");
                    objListTableModel.dictAttributes.ID = strCtrlId;

                    var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxRowModel.ParentVM = objListTableModel;

                    var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    objListTableModel.Elements.push(sfxRowModel);
                    scope.objRepeaterControl.Elements.push(objListTableModel);
                    scope.objRepeaterControl.initialvisibilty = true;
                    scope.objRepeaterControl.isLoaded = true;
                    $rootScope.PushItem(scope.objRepeaterControl, scope.dropdata.Elements);
                }
                scope.onRepeaterControlCancelClick();
            };

            scope.onRepeaterControlCancelClick = function () {
                RepeaterControldialog.close();
            };


            //#endregion

            scope.templatefieldmenuOptions = [
                ['Delete Column', function ($itemScope) {
                    var col = $itemScope.ele;
                    if (col) {
                        if (col.ParentVM.Elements.length <= 1) {
                            $SgMessagesService.Message('Message', 'The column cannot be deleted. Atleast one column should be present in the grid.');
                        }
                        else {
                            var iColumnIndex = col.ParentVM.Elements.indexOf(col);
                            $rootScope.DeleteItem(col, col.ParentVM.Elements);
                            if (iColumnIndex >= col.ParentVM.Elements.length - 1) {
                                iColumnIndex -= 1;
                            }
                            if (iColumnIndex > -1 && iColumnIndex < col.ParentVM.Elements.length && col.ParentVM.Elements[iColumnIndex]) {
                                SetFormSelectedControl(scope.formodel, col.ParentVM.Elements[iColumnIndex], null);
                            }
                            if (scope.formodel.dictAttributes.ID.startsWith("wfp") && col && col.ParentVM.ParentVM && col.ParentVM.ParentVM.Name == "sfwGridView") {
                                scope.AddShowDataRow(col.ParentVM.ParentVM, false, "");
                            }
                        }
                    }
                }, function ($itemScope, event) {
                    scope.selectControl($itemScope.ele, event, scope.formodel);
                    if (scope.IsDataView) {
                        return false;
                    }
                    return true;

                }], null,
                ['Remove Header', function ($itemScope) {
                    var col = $itemScope.ele;
                    if (col && col.Elements.length > 0) {
                        var obj = col.Elements.filter(function (x) { return x.Name == "HeaderTemplate"; });
                        if (obj && obj.length > 0) {
                            var index = col.Elements.indexOf(obj[0]);
                            if (index > -1) {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.DeleteItem(obj[0], col.Elements);
                                if (col.HeaderTemplateLabel) {
                                    $rootScope.EditPropertyValue(col.HeaderTemplateLabel.dictAttributes.sfwEntityField, col.HeaderTemplateLabel.dictAttributes, "sfwEntityField", "");
                                }
                                $rootScope.UndRedoBulkOp("End");
                            }
                        }
                    }
                }, function ($itemScope) {
                    if ($itemScope.formodel && $itemScope.formodel.dictAttributes.sfwType == "Lookup") {
                        return false;
                    }
                    else {

                        if (scope.showHeaderTemplate($itemScope.ele)) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }

                }], null,
                ['Remove Footer', function ($itemScope) {
                    var col = $itemScope.ele;
                    var col = $itemScope.ele;
                    if (col && col.Elements.length > 0) {
                        var obj = col.Elements.filter(function (x) { return x.Name == "FooterTemplate"; });
                        if (obj && obj.length > 0) {
                            var index = col.Elements.indexOf(obj[0]);
                            if (index > -1) {
                                //  col.Elements.splice(index, 1);
                                $rootScope.DeleteItem(obj[0], col.Elements);

                            }
                        }
                    }
                }, function ($itemScope) {
                    if ($itemScope.formodel && $itemScope.formodel.dictAttributes.sfwType == "Lookup") {
                        return false;
                    }
                    else {

                        if (scope.showFooterTemplate($itemScope.ele)) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }

                }], null,
                ['Move Column Left', function ($itemScope) {
                    var col = $itemScope.ele;
                    if (col) {
                        var ColIndex = col.ParentVM.Elements.indexOf(col);
                        if (ColIndex > 0) {
                            var model = col.ParentVM.Elements[ColIndex];
                            if (model) {
                                $rootScope.UndRedoBulkOp("Start");

                                //Removing
                                $rootScope.DeleteItem(col.ParentVM.Elements[ColIndex], col.ParentVM.Elements);

                                //Adding
                                $rootScope.InsertItem(model, col.ParentVM.Elements, ColIndex - 1);

                                $rootScope.UndRedoBulkOp("End");
                            }

                            // this.SelectControlAfterMoveOpe(selectedControlVM, curRowVM.RowIndex, cellVM.ColIndex - 1);
                        }
                    }
                }, function ($itemScope) {
                    if (scope.IsDataView) {
                        return false;
                    }
                    return true;

                }, null, "Ctrl+L"], null,
                ['Move Column Right', function ($itemScope) {
                    var col = $itemScope.ele;
                    if (col) {
                        var ColCount = col.ParentVM.Elements.length;
                        var ColIndex = col.ParentVM.Elements.indexOf(col);
                        if (ColIndex < ColCount - 1) {

                            var model = col.ParentVM.Elements[ColIndex];
                            if (model) {
                                $rootScope.UndRedoBulkOp("Start");
                                //Removing
                                $rootScope.DeleteItem(col.ParentVM.Elements[ColIndex], col.ParentVM.Elements);

                                //Adding
                                $rootScope.InsertItem(model, col.ParentVM.Elements, ColIndex + 1);

                                $rootScope.UndRedoBulkOp("End");
                            }

                        }
                    }
                }, function ($itemScope) {
                    if (scope.IsDataView) {
                        return false;
                    }
                    return true;

                }, null, "Ctrl+R"], null,
                ['Add Control', function ($itemScope) {
                    var itmtemplate = $itemScope.ele.Elements[0];
                    if (itmtemplate) {
                        scope.$parent.openChangeControlDialog("addcontrol", itmtemplate, true);

                    }
                }, function ($itemScope) {
                    if (scope.IsDataView) {
                        return false;
                    }
                    return true;
                }], null
            ];

            scope.itemtemplatemenuOptions = [
                ['Add Control', function ($itemScope) {
                    var itmtemplate = $itemScope.ele.Elements[0];
                    if (itmtemplate) {
                        scope.$parent.openChangeControlDialog("addcontrol", itmtemplate, true);
                    }
                }, null],
                ['Paste Control', function ($itemScope) {
                    if ($itemScope.ele.Elements[0]) {
                        scope.OnPasteControl($itemScope.ele.Elements[0]);
                    }
                }, null, "Ctrl+V"]
            ];

            scope.OnPasteControl = function (currmodel) {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.OnPasteControl) {
                    filescope.OnPasteControl(currmodel);
                }

            };

            scope.setValueToEntityField = function (dataColumn, dataRowModel) {
                angular.forEach(dataRowModel.Elements, function (item) {
                    if (item.dictAttributes.sfwEntityField === dataColumn.dictAttributes.sfwEntityField) {
                        item.dictAttributes.Value = dataColumn.dictAttributes.Value;
                    }
                    else {
                        scope.setValueToEntityField(dataColumn, item);
                    }
                });
            };

            scope.AddShowDataRow = function (model, flagAddNewRow, event) {
                if (model) {
                    var objGridColumnModel;
                    var objDataRowsModel;
                    for (var i = 0; i < model.Elements.length; i++) {
                        if (model.Elements[i].Name === "Columns") {
                            objGridColumnModel = model.Elements[i].Elements;
                        }
                        else if (model.Elements[i].Name === "Data") {
                            objDataRowsModel = model.Elements[i].Elements;
                        }
                    }

                    if ((objDataRowsModel && objDataRowsModel.length > 0) || (scope.model.prototypemodel && scope.model.prototypemodel.Elements && scope.model.prototypemodel.Elements.length > 0)) {
                        var objDataRow = { dictAttributes: { sfwEntityField: "", Value: "" }, Elements: [], Name: "DataRow", Value: "", prefix: "" };
                        for (var i = 0; i < objGridColumnModel.length; i++) {
                            var templateField = objGridColumnModel[i];
                            var objDataItemRow = { dictAttributes: {}, Elements: [], Name: "DataItem", Value: "", prefix: "" };
                            for (var j = 0; j < templateField.Elements[0].Elements.length; j++) {
                                if (templateField.Elements[0].Elements[j].Name != "cellformat") {
                                    var objDataColumn = { dictAttributes: { sfwEntityField: "", Value: "" }, Elements: [], Name: "DataColumn", Value: "", prefix: "" };
                                    if (templateField.Elements[0].Elements[j].dictAttributes.sfwEntityField) {
                                        objDataColumn.dictAttributes.sfwEntityField = templateField.Elements[0].Elements[j].dictAttributes.sfwEntityField;
                                    }
                                    objDataItemRow.Elements.push(objDataColumn);
                                }
                            }
                            objDataRow.Elements.push(objDataItemRow);
                        }

                        var objData = { dictAttributes: {}, Elements: [], Name: "Data", Value: "", prefix: "" };
                        //objData.Elements.push(objDataRow);
                        //model.Elements.push(objData);
                        //scope.prototypemodel = objData;
                        if (flagAddNewRow) {
                            objDataRow.isEditable = true;
                            scope.selectedDataRow = objDataRow;
                            scope.model.prototypemodel.Elements.push(objDataRow);
                        }
                        else if (objDataRowsModel) {
                            for (var i = 0; i < objDataRowsModel.length; i++) {
                                var objNewDataRow = {};
                                angular.copy(objDataRow, objNewDataRow);
                                for (var j = 0; j < objDataRowsModel[i].Elements.length; j++) {
                                    scope.setValueToEntityField(objDataRowsModel[i].Elements[j], objNewDataRow);
                                }
                                objData.Elements.push(objNewDataRow);
                            }
                            scope.model.prototypemodel = objData;
                        }
                    }
                    else if (flagAddNewRow) {
                        var objDataRow = { dictAttributes: { sfwEntityField: "", Value: "" }, Elements: [], Name: "DataRow", Value: "", prefix: "" };
                        for (var i = 0; i < objGridColumnModel.length; i++) {
                            var templateField = objGridColumnModel[i];
                            var objDataItemRow = { dictAttributes: {}, Elements: [], Name: "DataItem", Value: "", prefix: "" };
                            for (var j = 0; j < templateField.Elements[0].Elements.length; j++) {
                                if (templateField.Elements[0].Elements[j].Name != "cellformat") {
                                    var objDataColumn = { dictAttributes: { sfwEntityField: "", Value: "" }, Elements: [], Name: "DataColumn", Value: "", prefix: "" };
                                    if (templateField.Elements[0].Elements[j].dictAttributes.sfwEntityField) {
                                        objDataColumn.dictAttributes.sfwEntityField = templateField.Elements[0].Elements[j].dictAttributes.sfwEntityField;
                                    }
                                    objDataItemRow.Elements.push(objDataColumn);
                                }
                            }
                            objDataRow.Elements.push(objDataItemRow);
                        }

                        var objData = { dictAttributes: {}, Elements: [], Name: "Data", Value: "", prefix: "" };
                        objData.Elements.push(objDataRow);
                        objDataRow.isEditable = true;
                        scope.selectedDataRow = objDataRow;
                        scope.model.prototypemodel = objData;
                    }

                }
                if (event) {
                    event.stopPropagation();
                }
            };

            scope.ShowGridDesingView = function (model) {
                scope.IsDataView = false;
                var IsDataElement = false;
                if (model && model.prototypemodel) {
                    var objDataModel = GetPrototypeDataModel(model.prototypemodel);
                    if (model.Elements.length > 1) {
                        for (var i = 0; i < model.Elements.length; i++) {
                            if (model.Elements[i].Name == "Data") {
                                IsDataElement = true;
                                model.Elements[i].Elements = objDataModel;
                                break;
                            }
                        }
                    }
                    if (!IsDataElement) {
                        model.Elements.push({ dictAttributes: {}, Elements: objDataModel, Name: "Data", Value: "", prefix: "" });
                    }
                }
            };

            scope.DeleteDataRow = function (model) {
                if (model.prototypemodel && scope.selectedDataRow) {
                    for (var i = 0; i < model.prototypemodel.Elements.length; i++) {
                        if (model.prototypemodel.Elements[i] == scope.selectedDataRow) {
                            model.prototypemodel.Elements.splice(i, 1);
                            if (model.prototypemodel.Elements.length > i) {
                                scope.selectedDataRow = model.prototypemodel.Elements[i];
                            }
                            else if (model.prototypemodel.Elements.length > 0) {
                                scope.selectedDataRow = model.prototypemodel.Elements[model.prototypemodel.Elements.length - 1];
                            }
                            break;
                        }
                    }
                }
            };

            scope.moveDown = function () {
                if (scope.selectedDataRow && scope.model.prototypemodel && scope.model.prototypemodel.Elements && scope.model.prototypemodel.Elements.length > 1) {
                    var index = scope.model.prototypemodel.Elements.indexOf(scope.selectedDataRow);
                    if (index < scope.model.prototypemodel.Elements.length - 1) {
                        scope.model.prototypemodel.Elements.splice(index, 1);
                        scope.model.prototypemodel.Elements.splice(index + 1, 0, scope.selectedDataRow);
                    }
                }
            };

            scope.moveUp = function () {
                if (scope.selectedDataRow && scope.model.prototypemodel && scope.model.prototypemodel.Elements && scope.model.prototypemodel.Elements.length > 1) {
                    var index = scope.model.prototypemodel.Elements.indexOf(scope.selectedDataRow);
                    if (index > 0) {
                        scope.model.prototypemodel.Elements.splice(index, 1);
                        scope.model.prototypemodel.Elements.splice(index - 1, 0, scope.selectedDataRow);
                    }
                }
            };

            scope.SetParentVM = function (parent, model) {
                model.ParentVM = parent;

            };

            //#region Grid Header, Footer Template Methods
            scope.showHeaderTemplate = function (templateField) {
                var isHeaderTemplatePresent = false;
                if (templateField) {
                    var lst = templateField.Elements.filter(function (ele) { return ele.Name == "HeaderTemplate"; });
                    if (lst && lst.length > 0) {
                        if (lst[0].Elements && lst[0].Elements.some(function (ele) { if (ele.dictAttributes.sfwEntityField) return ele.Name == "sfwLabel"; })) {
                            isHeaderTemplatePresent = true;
                        }
                    }
                }
                return isHeaderTemplatePresent;
            };

            scope.openHeaderLabelProperty = function (templateField, event) {
                if (templateField) {
                    var lst = templateField.Elements.filter(function (ele) { return ele.Name == "HeaderTemplate"; });
                    if (lst && lst.length > 0) {
                        if (lst[0].Elements) {
                            lst[0].ParentVM = templateField;
                            var elements = lst[0].Elements.filter(function (ele) { return ele.Name == "sfwLabel"; });
                            if (elements && elements.length > 0) {
                                elements[0].ParentVM = lst[0];
                                scope.selectControl(elements[0], event, scope.formodel);
                            }
                        }
                    }
                }
            };

            scope.showFooterTemplate = function (templateField) {
                var isFooterTemplatePresent = false;
                if (templateField) {
                    var lst = templateField.Elements.filter(function (ele) { return ele.Name == "FooterTemplate"; });
                    if (lst && lst.length > 0) {
                        if (lst[0].Elements && lst[0].Elements.some(function (ele) { return ele.Name == "sfwLabel"; })) {
                            isFooterTemplatePresent = true;
                        }
                    }
                }
                return isFooterTemplatePresent;
            };

            scope.openFooterLabelProperty = function (templateField, event) {
                if (templateField) {
                    var lst = templateField.Elements.filter(function (ele) { return ele.Name == "FooterTemplate"; });
                    if (lst && lst.length > 0) {
                        if (lst[0].Elements) {
                            lst[0].ParentVM = templateField;
                            var elements = lst[0].Elements.filter(function (ele) { return ele.Name == "sfwLabel"; });
                            if (elements && elements.length > 0) {
                                elements[0].ParentVM = lst[0];
                                scope.selectControl(elements[0], event, scope.formodel);
                            }
                        }
                    }
                }
            };

            //#endregion

            scope.selectDataRowInPrototypeGrid = function (ele, event) {
                scope.selectedDataRow = ele;
                event.stopPropagation();
            };

        }
    }
}]);

app.directive("usercontrolControlDirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',

            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/UsercontrolControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {

            scope.getValue = function (item) {
                return getDisplayValue(item);
            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
            };


        }
    }
}]);

app.directive("commonControlDirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    var SetImage = function (item, IsSelected) {
        switch (item.Name) {
            case 'sfwRadioButton':
                if (IsSelected) {
                    return "images/Form/icon-Radio-Button-Selected.svg";
                }
                else {
                    return "images/Form/icon-radio.svg";
                }
                break;
            case 'sfwCheckBox':
                if (item.dictAttributes.sfwIsSwitch && item.dictAttributes.sfwIsSwitch == "True") {
                    if (IsSelected) {
                        return "images/Form/icon-switch-CheckBox-Selected.svg";
                    }
                    else {
                        return "images/Form/icon-switch-CheckBox.svg";
                    }
                }
                else {
                    if (IsSelected) {
                        return "images/Form/icon-Checkbox-Selected.svg";
                    }
                    else {
                        return "images/Form/icon-checkbox.svg";
                    }
                }
                break;
            case 'sfwImage':
                if (IsSelected) {
                    return "images/Form/icon-Image-Selected.svg";
                }
                else {
                    return "images/Form/icon-image.svg";
                }
                break;
            case 'hr':
                if (IsSelected) {
                    return "images/Form/horizontal_rule - selected.svg";
                }
                else {
                    return "images/Form/horizontal_rule.svg";
                }
                break;
            case 'br':
                if (IsSelected) {
                    return "images/Form/break - selected.svg";
                }
                else {
                    return "images/Form/break.svg";
                }
                break;
            case 'RequiredFieldValidator':
                if (IsSelected) {
                    return "images/Form/icon-Required-Field-Validator-Selected.svg";
                }
                else {
                    return "images/Form/icon-required-field-validator.svg";
                }
                break;
            case 'CompareValidator':
                if (IsSelected) {
                    return "images/Form/icon-Compare-Validator-Selected.svg";
                }
                else {
                    return "images/Form/icon-compare-validator.svg";
                }
                break;
            case 'sfwCRViewer':
                if (IsSelected) {
                    return "images/Form/icon-CR-Viewer-Selected.svg";
                }
                else {
                    return "images/Form/CR_Viewer.svg";
                }
                break;
            case 'sfwSoftErrors':
                if (IsSelected) {
                    return "images/Form/icon-Soft-Errors-Selected.svg";
                }
                else {
                    return "images/Form/Soft_Errors.svg";
                }
                break;
            case 'sfwFileLayout':
                if (IsSelected) {
                    return "images/Form/icon-File-Layout-Selected.svg";
                }
                else {
                    return "images/Form/File_Layout.svg";
                }
                break;
            case 'sfwFileUpload':
                if (IsSelected) {
                    return "images/Form/icon-File-Upload-Selected.svg";
                }
                else {
                    return "images/Form/File_Upload.svg";
                }
                break;
            case 'sfwListPicker':
                if (IsSelected) {
                    return "images/Form/icon-Listpicker-Selected.svg";
                }
                else {
                    return "images/Form/icon-mant4.svg";
                }
                break;
            case 'sfwSourceList':
                if (IsSelected) {
                    return "images/Form/icon-Sourcelist-Selected.svg";
                }
                else {
                    return "images/Form/icon-mant5.svg";
                }
                break;
            case 'sfwMultiCorrespondence':
                if (IsSelected) {
                    return "images/Form/icon-Required-Field-Validator-Selected.svg";
                }
                else {
                    return "images/Form/icon-required-field-validator.svg";
                }
                break;
            case 'sfwTargetList':
                if (IsSelected) {
                    return "images/Form/icon-TargetList-Selected.svg";
                }
                else {
                    return "images/Form/icon-mant6.svg";
                }
                break;

            case 'sfwOpenDetail':
                if (IsSelected) {
                    return "images/Form/icon-Open-Detail-Selected.svg";
                }
                else {
                    return "images/Form/icon-open-detail.svg";
                }
                break;

            case 'sfwRuleViewer':
                if (IsSelected) {
                    return "images/Form/icon-Rule-Viewer-Selected.svg";
                }
                else {
                    return "images/Form/icon-Rule-Viewer.svg";
                }
                break;
            case 'sfwSlider':
                if (IsSelected) {
                    return "images/Form/icon-slider-Selected.svg";
                }
                else {
                    return "images/Form/icon-slider.svg";
                }
                break;
            case 'sfwQuestionnairePanel':
                if (IsSelected) {
                    return "images/Form/icon-QuestionnairePanel-selected.svg";
                }
                else {
                    return "images/Form/icon-QuestionnairePanel.svg";
                }
                break;
            case 'sfwDateTimePicker':
                if (IsSelected) {
                    return "images/Form/icon-DateTimePicker-selected.png";
                }
                else {
                    return "images/Form/icon-DateTimepicker.png";
                }
                break;
            case 'sfwKnob':
                if (IsSelected) {
                    return "images/Form/icon-knob-selected.svg";
                }
                else {
                    return "images/Form/icon-knob.svg";
                }
                break;
            case 'sfwCheckBoxList':
                if (IsSelected) {
                    return "images/Form/icon-checkboxlist.svg";
                }
                else {
                    return "images/Form/icon-checkboxlist.svg";
                }
                break;
            case 'sfwRadioButtonList':
                if (IsSelected) {
                    return "images/Form/icon-radiobuttonlist.svg";
                }
                else {
                    return "images/Form/icon-radiobuttonlist.svg";
                }
                break;
            case 'sfwChart':
                if (IsSelected) {
                    return "images/Form/icon-chart.svg";
                }
                else {
                    return "images/Form/icon-chart.svg";
                }
                break;
            case 'sfwCalendar':
                if (IsSelected) {
                    return "images/Form/icon-calendar-selected.png";
                }
                else {
                    return "images/Form/icon-calendar.png";
                }
                break;
            case 'sfwScheduler':
                if (IsSelected) {
                    return "images/Form/icon-scheduler-selected.svg";
                }
                else {
                    return "images/Form/icon-scheduler.svg";
                }
                break;
            case 'sfwListBox':
                if (IsSelected) {
                    return "images/Form/icon-ListBox-Selected.svg";
                }
                else {
                    return "images/Form/icon-ListBox.svg";
                }
                break;
            case 'sfwJSONData':
                if (IsSelected) {
                    return "images/Form/JSON_Data_selected.svg";
                }
                else {
                    return "images/Form/JSON_Data.svg";
                }
                break;
            case 'sfwCaptcha':
                if (IsSelected) {
                    return "images/Form/captcha_control_blue.svg";
                }
                else {
                    return "images/Form/captcha_control_normal.svg";
                }
                break;

        }

    };

    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/CommonControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {
            scope.getDisplayClass = function (item) {
                return getDisplayClass(item);
            };

            scope.strImageSource = SetImage(scope.model, false);

            scope.getValue = function (item) {
                return getDisplayValue(item);
            };

            scope.getCodeValue = function (model, event) {

                selectControl(model, event, scope.formodel);
                if (model.Name == "sfwListPicker" || model.Name == "sfwSourceList" || model.Name == "sfwRadioButtonList" || model.Name == "sfwCascadingDropDownList" || model.Name == "sfwDropDownList" || model.Name == "sfwCheckBoxList" || model.Name == "sfwMultiSelectDropDownList" || model.Name == "sfwListBox") {
                    if ((!model.dictAttributes.sfwLoadType || model.dictAttributes.sfwLoadType == "" || model.dictAttributes.sfwLoadType == "CodeGroup" || (model.dictAttributes.sfwLoadType == "Items" && model.Elements.length == 0)) && (model.dictAttributes.sfwLoadSource || model.placeHolder)) {
                        $rootScope.IsLoading = true;

                        //#region Code Value receive
                        // clear selection if the dialog opens for code value 
                        if (event) {
                            $(event.target).blur();
                            window.getSelection().removeAllRanges();
                        }
                        var codeID = "";
                        if (model.dictAttributes.sfwLoadSource && model.dictAttributes.sfwLoadType == "CodeGroup") {
                            codeID = model.dictAttributes.sfwLoadSource;
                        }
                        else {
                            codeID = model.placeHolder;
                        }
                        $.connection.hubMain.server.getCodeValuesForDropDown(codeID).done(function (data) {
                            scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;

                                var DialogScope = scope.$new(true);
                                DialogScope.ErrorMessage = "";
                                if (data && data.length > 0) {
                                    DialogScope.lstCodeValues = data;
                                }
                                else {
                                    DialogScope.ErrorMessage = "No code value exist.";
                                }
                                dialog = $rootScope.showDialog(DialogScope, "Code Values", "Form/views/CodeValueListDialog.html");
                                DialogScope.closeDialog = function () {
                                    dialog.close();
                                };

                            });
                        });
                    }
                    //#endregion
                }
            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
                scope.strImageSource = SetImage(scope.model, true);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
                scope.strImageSource = SetImage(scope.model, true);
            };

        }
    }
}]);

app.directive("childpanelControlDirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',

            objparent: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/ChildPanelControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {
            scope.initialvisibilty = true;
            scope.panelId = "";
            if (scope.model.dictAttributes.ID) {
                scope.panelId = "Panel" + scope.model.dictAttributes.ID;
            }
            scope.setInitialVisibility = function () {
                scope.initialvisibilty = !scope.initialvisibilty;
            };
            scope.setVisibility = function (model) {
                if (model.ParentControlName == 'udc' && (model.Name == "sfwPanel" || model.Name == "sfwDialogPanel" || model.Name == "sfwListView")) {
                    scope.initialvisibilty = !scope.initialvisibilty;
                    scope.model.isLoaded = true;
                }
            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
            };

            scope.panelMenuOption = [
                ['Create Compatible Caption', function ($itemScope) {
                    var filescope = getCurrentFileScope();
                    if (scope.model) {
                        if (filescope && filescope.OnCreateCompatibleLabelClickForPanel) {
                            filescope.OnCreateCompatibleLabelClickForPanel(scope.model);
                        }
                    }
                }],
                ['Delete Control', function ($itemScope) {
                    var filescope = getCurrentFileScope();
                    if (scope.model) {
                        if (filescope && filescope.OnDeleteControlClick) {
                            filescope.OnDeleteControlClick(scope.model);
                        }
                    }
                }, function ($itemScope) {
                    if (scope.model) {
                        var objPanel = FindParent(scope.model, "sfwPanel");
                        if (!objPanel) {
                            objPanel = FindParent(scope.model, "sfwWizardStep");
                            if (!objPanel) {
                                objPanel = FindParent(scope.model, "sfwDialogPanel", true);
                                if (!objPanel) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }],
                ['Cut Control (Ctrl+X)', function ($itemScope) {
                    if (scope.model) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.OnCutControlClick) {
                            filescope.OnCutControlClick(scope.model);
                        }
                    }
                }, function ($itemScope) {
                    if (scope.model) {
                        var objPanel = FindParent(scope.model, "sfwPanel");
                        if (!objPanel) {
                            objPanel = FindParent(scope.model, "sfwWizardStep");
                            if (!objPanel) {
                                objPanel = FindParent(scope.model, "sfwDialogPanel", true);
                                if (!objPanel) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }],
                ['Copy Control (Ctrl+C)', function ($itemScope) {
                    if (scope.model) {
                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.OnCopyControlClick) {
                            filescope.OnCopyControlClick(scope.model);
                        }
                    }
                }, function ($itemScope) {
                    if (scope.model) {
                        var objPanel = FindParent(scope.model, "sfwPanel");
                        if (!objPanel) {
                            objPanel = FindParent(scope.model, "sfwWizardStep");
                            if (!objPanel) {
                                objPanel = FindParent(scope.model, "sfwDialogPanel", true);
                                if (!objPanel) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }],
            ];

        }
    }
}]);

app.directive("emptyparentControlDirective", ["$compile", "$rootScope", function ($compile, $rootScope) {
    return {
        restrict: "E",
        replace: true,
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/EmptyParentControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {

        },
        controller: ['$scope', function ($scope) {
            $scope.selectControl = function (objChild, event, formModel) {
                selectControl(objChild, event, formModel);
            };
            $scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };
        }]
    }
}]);


app.directive("editableControlDirective", ["$compile", "$rootScope", "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", function ($compile, $rootScope, $EntityIntellisenseFactory, $ValidationService, CONST) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            formodel: '=',
            lstloadedentitycolumnstree: '='
        },
        template: function () {
            var strTemplate = getHtmlFromServer('Form/views/Controls/EditableControlView.html');
            return strTemplate;
        },
        link: function (scope, element, attributes) {
            scope.getCodeValue = function (model, event) {

                selectControl(model, event, scope.formodel);
                if (model.Name == "sfwListPicker" || model.Name == "sfwSourceList" || model.Name == "sfwRadioButtonList" || model.Name == "sfwCascadingDropDownList" || model.Name == "sfwDropDownList" || model.Name == "sfwCheckBoxList" || model.Name == "sfwMultiSelectDropDownList" || model.Name == "sfwListBox") {
                    if ((!model.dictAttributes.sfwLoadType || model.dictAttributes.sfwLoadType == "" || model.dictAttributes.sfwLoadType == "CodeGroup" || (model.dictAttributes.sfwLoadType == "Items" && model.Elements.length == 0)) && (model.dictAttributes.sfwLoadSource || model.placeHolder)) {
                        $rootScope.IsLoading = true;

                        //#region Code Value receive
                        // clear selection if the dialog opens for code value 
                        if (event) {
                            $(event.target).blur();
                            window.getSelection().removeAllRanges();
                        }
                        var codeID = "";
                        if (model.dictAttributes.sfwLoadSource && model.dictAttributes.sfwLoadType == "CodeGroup") {
                            codeID = model.dictAttributes.sfwLoadSource;
                        }
                        else {
                            codeID = model.placeHolder;
                        }
                        $.connection.hubMain.server.getCodeValuesForDropDown(codeID).done(function (data) {
                            $rootScope.IsLoading = false;

                            scope.$evalAsync(function () {
                                var DialogScope = scope.$new(true);
                                DialogScope.ErrorMessage = "";
                                if (data && data.length > 0) {
                                    DialogScope.lstCodeValues = data;
                                }
                                else {
                                    DialogScope.ErrorMessage = "No code value exist.";
                                }
                                dialog = $rootScope.showDialog(DialogScope, "Code Values", "Form/views/CodeValueListDialog.html");
                                DialogScope.closeDialog = function () {
                                    dialog.close();
                                };

                            });
                        });
                    }
                    //#endregion
                }
            };

            scope.getDisplayClass = function (item) {
                return getDisplayClass(item);
            };

            scope.selectControlOnDoubleClick = function (objChild, event) {
                selectControlOnDoubleClick(objChild, event);
            };

            scope.selectControl = function (objChild, event) {
                selectControl(objChild, event, scope.formodel);
            };

            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope && controllerScope.onActionKeyDown) {
                    //controllerScope.SelectedNode = SelectedNode;

                    controllerScope.onActionKeyDown(eargs);
                }
            };

            scope.checkEntityField = function (obj) {
                if (scope.fileScope.lstLoadedEntityTrees) {
                    var entitytree = scope.fileScope.lstLoadedEntityTrees.filter(function (x) { return x.IsVisible; });
                    var entityName;
                    if (entitytree && entitytree.length > 0) {
                        entityName = entitytree[0].EntityName;
                    } else {
                        entityName = scope.formodel.dictAttributes.sfwEntity;
                    }
                    if (obj.dictAttributes.sfwRelatedGrid) {
                        entityName = scope.fileScope.FindEntityName(obj, scope.formodel.dictAttributes.sfwEntity);
                    }
                    if (obj.IsChildOfGrid) {
                        var objGrid = FindParent(obj, "sfwGridView");
                        if (objGrid && objGrid.dictAttributes.sfwParentGrid && objGrid.dictAttributes.sfwEntityField) {
                            entityName = scope.fileScope.FindEntityName(objGrid, scope.formodel.dictAttributes.sfwEntity, true);
                        }
                    }
                    $ValidationService.checkValidListValueForMultilevel([], obj, obj.dictAttributes.sfwEntityField, entityName, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, false, '');
                }
            }
            scope.validateEntityField = function (obj) {
                var listOfColumns = [];
                if (!scope.fileScope) scope.fileScope = getCurrentFileScope();
                if (obj.IsChildOfGrid) {
                    var objGrid = FindParent(obj, "sfwGridView");
                    if (objGrid.Name == "sfwGridView" && objGrid.dictAttributes.sfwBoundToQuery && objGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                        var baseQuery = objGrid.dictAttributes.sfwBaseQuery;
                        if (angular.isArray(scope.columnList) && scope.columnList.length == 0) {
                            $.connection.hubForm.server.getEntityQueryColumns(baseQuery, '').done(function (data) {
                                scope.$evalAsync(function () {
                                    scope.columnList = $ValidationService.getListByPropertyName(data, "CodeID", false);
                                    $ValidationService.checkValidListValue(scope.columnList, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, null);
                                });
                            });
                        } else {
                            $ValidationService.checkValidListValue(scope.columnList, obj, obj.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, null);
                        }
                    } else {
                        scope.checkEntityField(obj);
                    }
                } else {
                    scope.checkEntityField(obj);
                }
                if (scope.model && (scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name == "sfwMultiSelectDropDownList")) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entityName = "";
                    if (scope.lstloadedentitytrees) {
                        var entitytree = scope.lstloadedentitytrees.filter(function (x) { return x.IsVisible; });
                    }
                    if (entitytree && entitytree.length > 0) {
                        entityName = entitytree[0].EntityName;
                    } else {
                        if (scope.formodel) {
                            entityName = scope.formodel.dictAttributes.sfwEntity;
                        }
                    }
                    var strCodeGroup = "";
                    if (entityName) {
                        if (scope.IsSearchCriteriaSelected && scope.model.dictAttributes.sfwDataField) {
                            strCodeGroup = GetCodeIDForLookup(entityName, scope.model.dictAttributes.sfwDataField, entityIntellisenseList);
                        }
                        else if (scope.model.dictAttributes.sfwEntityField) {
                            strCodeGroup = GetCodeID(entityName, scope.model.dictAttributes.sfwEntityField, entityIntellisenseList);
                        }
                        if (strCodeGroup) {
                            scope.model.placeHolder = strCodeGroup;
                        }
                        else {
                            scope.model.placeHolder = "";
                        }
                    }
                }
            };

            scope.onDataFieldKeyDown = function (event) {
                var input = $(event.target);
                var blnFound = false;
                var data;
                scope.dataFieldList = undefined;
                scope.attributeName = "ID";
                if (scope.model.IsShowDataField) {
                    var lst = [];
                    FindControlListByName(scope.formodel, 'sfwButton', lst);
                    if (lst && lst.length > 0) {
                        lst = lst.filter(function (itm) { return itm.dictAttributes.sfwMethodName == "btnGridSearch_Click"; });
                        if (lst && lst.length > 0) {
                            for (var i = 0; i < lst.length; i++) {
                                if (lst[i].dictAttributes.sfwNavigationParameter) {
                                    var lstParam = lst[i].dictAttributes.sfwNavigationParameter.split(';');
                                    if (lstParam && lstParam.length > 0) {
                                        if (lstParam.some(function (param) { return param == scope.model.dictAttributes.ID; })) {
                                            objFilterGridButton = lst[i];
                                            break;
                                        }
                                    }
                                }
                            }

                            if (objFilterGridButton) {
                                if (objFilterGridButton.dictAttributes.sfwRelatedControl) {
                                    var objGrid = FindControlByID(scope.formodel, objFilterGridButton.dictAttributes.sfwRelatedControl);
                                    if (objGrid && objGrid.dictAttributes.sfwBaseQuery) {

                                        $.connection.hubForm.server.getEntityQueryColumns(objGrid.dictAttributes.sfwBaseQuery, "ScopeId_" + scope.$id).done(function (data1) {
                                            if (data1) {
                                                scope.lstColumns = data1;
                                                scope.attributeName = "CodeID";
                                                setSingleLevelAutoComplete(input, scope.lstColumns, scope, scope.attributeName);
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if (scope.model.dictAttributes.sfwQueryID) {
                        data = PopulateQueryColumnFromList(scope.model.dictAttributes.sfwQueryID, scope.lstloadedentitycolumnstree, scope.formodel);
                    }
                    else {
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var MainQuery = GetMainQueryFromFormObject(scope.formodel, entityIntellisenseList);
                        if (MainQuery) {
                            data = PopulateQueryColumnFromList(MainQuery.dictAttributes.ID, scope.lstloadedentitycolumnstree, scope.formodel);
                        }
                        else {
                            scope.attributeName = "Value";
                            if (scope.formodel && scope.formodel.dictAttributes.sfwEntity) {
                                data = getEntityAttributeByType(entityIntellisenseList, scope.formodel.dictAttributes.sfwEntity, "Column");

                            }
                        }
                    }
                }
                if (data) {
                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", "");
                        event.preventDefault();
                    }
                    else {
                        var item = [];

                        var arrText = [input[0].innerText];
                        if (arrText.length > 0) {
                            for (var index = 0; index < arrText.length; index++) {
                                item = data.filter(function (x) { if (x.ID) { return x.ID.toLowerCase().contains(arrText[index].toLowerCase()); } });
                            }
                            data = item;
                        }
                        scope.dataFieldList = data;
                        setSingleLevelAutoComplete(input, data, scope, scope.attributeName);

                    }
                }
            };


            scope.checkDataFieldValueAndClearList = function (model) {
                scope.dataFieldList = undefined;
            };

            //#region Correspondence Query Id
            scope.onQueryIDKeyDown = function (event) {
                setSingleLevelAutoComplete($(event.target), getQueryBookMarksID(scope.formodel));


            };
            //#endregion
        }
    }
}]);



function CanShowContextMenu(model, formodel) {
    if (model && formodel && formodel.dictAttributes.sfwType === "Lookup") {
        var cellVM = FindParent(model, "sfwColumn", true);
        if (cellVM && cellVM.Elements && cellVM.Elements.length > 0 && cellVM.Elements[0].Name === "sfwTabContainer") {
            return false;
        }
        return true;
    }
    return true;
}

function getDisplayClass(item) {
    var classname = "";

    if (item.Name == 'sfwTextBox') {
        if (item.IsSelected) {
            classname = "select-textbox-control";
        }
        else {
            classname = "form-control-textbox";
        }
    }
    else if (item.Name == 'sfwLabel') {
        if (item.IsSelected) {
            classname = "select-label-control";
        }
        else {
            classname = "form-fixed-width";
        }
    }
    else if (item.Name == 'sfwDropDownList' || item.Name == "sfwMultiSelectDropDownList") {
        if (item.IsSelected) {
            classname = "select-dropdown-control";
        }
        else {
            classname = "form-control-dropdown";
        }
    }
    else if (item.Name == 'sfwCascadingDropDownList') {
        if (item.IsSelected) {
            classname = "select-cascading-dropdown-control";
        }
        else {
            classname = "form-control-cascading-dropdown";
        }
    }
    else if (item.Name == 'sfwButton') {
        if (item.IsSelected) {
            classname = "select-button-control";
        }
        else {
            classname = "form-control-button";
        }
    }
    else if (item.Name == 'sfwToolTipButton') {
        if (item.IsSelected) {
            classname = "select-tooltip-button-control";
        }
        else {
            classname = "form-control-tooltip-button";
        }
    }
    else if (item.Name == 'sfwLinkButton') {
        if (item.IsSelected) {
            classname = "select-link-control-button";
        }
        else {
            classname = "link-control-button";
        }
    }
    else if (item.Name == 'sfwImageButton') {
        if (item.IsSelected) {
            classname = "select-image-control-button";
        }
        else {
            classname = "image-control-button";
        }
    }
    else if (item.Name == 'sfwHyperLink') {
        if (item.IsSelected) {
            classname = "select-hyperlink-control";
        }
        else {
            classname = "form-control-hyperlink";
        }
    }
    else {
        if (item.IsSelected) {
            classname = "select-label-control";
        }
        else {
            classname = "form-fixed-width";
        }
    }

    if (item.dictAttributes && item.dictAttributes.Text) {
        classname += ' ';
    }
    else if (item.dictAttributes && item.dictAttributes.sfwCaption) {
        classname += ' ';
    }
    else if (item.dictAttributes && item.dictAttributes.sfwEntityField) {
        classname += ' font-entityfield';
    }
    else if (item.dictAttributes && item.dictAttributes.sfwObjectField) {
        classname += ' font-entityfield';
    }
    else if (item.dictAttributes && item.dictAttributes.ID) {
        classname += ' font-controlid';
    }
    else {
        classname += ' font-controlid';
    }
    if (item.isAdvanceSearched) {
        classname += ' bckgGrey';
    }
    if (item.IsSelected && item.isAdvanceSearched) {
        classname += ' bckgGreen';
    }
    return classname;
};

//#region Select Control
function selectControlOnDoubleClick(objChild, event) {
    var filescope = getCurrentFileScope();
    if (filescope && filescope.selectControlOnDoubleClick) {
        filescope.selectControlOnDoubleClick(objChild, event);
    }
};

function selectControl(objChild, event, formodel) {
    //scope.columnList = [];
    var filescope = getCurrentFileScope();
    if (filescope && filescope.selectControl) {
        filescope.selectControl(objChild, event, formodel);
        // for triggering the drag handler on selection
        $("#" + filescope.currentfile.FileName).find("[drag-handler-btn]").remove();
    }

    if (objChild.Name != 'sfwColumn' && objChild.Name != 'TemplateField' && objChild.Name != 'sfwToolTipButton') {
        if (event) {
            funcontrolDragEnter($(event.currentTarget)[0], objChild, formodel);
        }
        // for selecting the element when its dropped - no event is there - access the current element of the directive and the current model
        else if (objChild.IsSelected) {
            funcontrolDragEnter($(element).find("[control-head]")[0], objChild, formodel);
        }
    }
};
//#endregion

//#region Insert  Row/Column
function OnInsertRowAboveClick(cellVM, tableVM, $rootScope) {
    var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

    var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
    var index = GetIndexToInsert(false, iRowIndex);
    $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);
};

function OnInsertRowBelowClick(cellVM, tableVM, $rootScope) {
    var iRowIndex = tableVM.Elements.indexOf(cellVM.ParentVM);

    var sfxRowModel = InsertRow(cellVM, iRowIndex, tableVM);
    var index = GetIndexToInsert(true, iRowIndex);
    $rootScope.InsertItem(sfxRowModel, tableVM.Elements, index);


};

function OnInsertColumnLeftClick(cellVM, tableVM) {
    var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
    var filescope = getCurrentFileScope();
    if (filescope && filescope.InsertColumn) {
        filescope.InsertColumn(cellVM, false, iColumnIndex, tableVM);
    }

};

function OnInsertColumnRightClick(cellVM, tableVM) {
    var iColumnIndex = cellVM.ParentVM.Elements.indexOf(cellVM);
    var filescope = getCurrentFileScope();
    if (filescope && filescope.InsertColumn) {
        filescope.InsertColumn(cellVM, true, iColumnIndex, tableVM);
    }
};
//#endregion

//#region Clear Cell / Grid
function ClearCell(cellVM, $rootScope, blnAddUndoblock) {
    if (blnAddUndoblock) {
        $rootScope.UndRedoBulkOp("Start");
    }
    while (cellVM.Elements.length > 0) {
        $rootScope.DeleteItem(cellVM.Elements[0], cellVM.Elements);

    }
    if (blnAddUndoblock) {
        $rootScope.UndRedoBulkOp("End");
    }
};

function OnClearGridClick(tableVM, $rootScope) {
    $rootScope.UndRedoBulkOp("Start");
    function iteratorTableRows(rowVM) {
        angular.forEach(rowVM.Elements, iteratorColumns);
    }
    function iteratorColumns(cellVM) {
        ClearCell(cellVM, $rootScope, false);
    }

    if (tableVM) {

        angular.forEach(tableVM.Elements, iteratorTableRows);
    }
    $rootScope.UndRedoBulkOp("End");

};
//#endregion

//#region Delecte Row/Column

function OnDeleteRowClick(rowVM, tableVM, $rootScope, $SgMessagesService) {
    var iRowIndex = tableVM.Elements.indexOf(rowVM);


    if (null != tableVM) {
        if (rowVM) {
            if (tableVM.Elements.length > 1) {

                $rootScope.DeleteItem(rowVM, tableVM.Elements);

            }
            else {
                $SgMessagesService.Message('Message', 'Atleast one row should be present.');
            }
        }
        iRowIndex = iRowIndex - 1;
    }


};

function OnDeleteColumnClick(aParam, tableVM, $rootScope, formodel, $SgMessagesService) {

    var iColumnIndex = aParam.ParentVM.Elements.indexOf(aParam);

    function iDeleteRows(rowVM) {

        for (var i = 0; i < rowVM.Elements.length; i++) {
            var cellVM = rowVM.Elements[i];
            if (i == colIndex) {
                $rootScope.DeleteItem(cellVM, rowVM.Elements);

                break;
            }

        }


    }
    if (aParam) {
        var ColCount = GetMaxColCount(aParam.ParentVM, tableVM);
        if (ColCount > 1) {


            var colIndex = aParam.ParentVM.Elements.indexOf(aParam);
            $rootScope.UndRedoBulkOp("Start");

            angular.forEach(tableVM.Elements, iDeleteRows);

            $rootScope.UndRedoBulkOp("End");


        }
        else {
            $SgMessagesService.Message('Message', 'Atleast one column should be present');

        }
    }
    if (iColumnIndex > 0) {
        iColumnIndex = iColumnIndex - 1;
    }
    if (iColumnIndex > -1 && iColumnIndex < aParam.ParentVM.Elements.length && aParam.ParentVM.Elements[iColumnIndex]) {
        SetFormSelectedControl(formodel, aParam.ParentVM.Elements[iColumnIndex], null);
    }

};


//#endregion

function AddControlToCell(cellVM, cntrlName, sfxControlModel, blnIsGrid, formodel, model, $rootScope, newScope, $ValidationService, $GetEntityFieldObjectService) {
    if (!sfxControlModel) {
        sfxControlModel = CreateControl(formodel, cellVM, cntrlName, blnIsGrid);
    }

    if (sfxControlModel != null && cellVM && sfxControlModel.Name != "udc") {
        if (sfxControlModel.Name == "sfwDialogPanel" || sfxControlModel.Name == "sfwPanel") {
            sfxControlModel.initialvisibilty = true;
            sfxControlModel.isLoaded = true;
        }
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(sfxControlModel, cellVM.Elements);

        if (blnIsGrid) {
            if (formodel && formodel.dictAttributes.sfwType != "Lookup") {

                if (sfxControlModel.Name != "sfwLabel" && sfxControlModel.Name != "sfwButton" && sfxControlModel.Name != "sfwLinkButton" && sfxControlModel.Name != "sfwImageButton" && sfxControlModel.Name !== "sfwButtonGroup") {
                    if (model.Name == "sfwGridView") {
                        $rootScope.EditPropertyValue(model.dictAttributes.sfwTwoWayBinding, model.dictAttributes, "sfwTwoWayBinding", "True");
                        $rootScope.EditPropertyValue(model.dictAttributes.AllowEditing, model.dictAttributes, "AllowEditing", "True");
                        $rootScope.EditPropertyValue(model.dictAttributes.sfwCommonFilterBox, model.dictAttributes, "sfwCommonFilterBox", "False");
                        $rootScope.EditPropertyValue(model.dictAttributes.sfwFilterOnKeyPress, model.dictAttributes, "sfwFilterOnKeyPress", "False");


                    }
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
        SetFormSelectedControl(formodel, sfxControlModel);
    }



    //#region Add User Control
    if (sfxControlModel != undefined && sfxControlModel.Name == "udc") {


        newScope.objSetUCProp = { StrId: sfxControlModel.dictAttributes.ID, StrName: sfxControlModel.dictAttributes.Name, StrEntityField: sfxControlModel.dictAttributes.sfwEntityField, StrResource: sfxControlModel.dictAttributes.sfwResource, formObject: formodel };
        newScope.objSetUCProp.IsAddedFromObjectTree = false;
        newScope.onUserControlOkClick = function () {


            sfxControlModel.dictAttributes.ID = newScope.objSetUCProp.StrId;
            sfxControlModel.dictAttributes.Name = newScope.objSetUCProp.StrName;
            if ((formodel.dictAttributes.sfwEntity != undefined && formodel.dictAttributes.sfwEntity != "") && (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "")) {
                if (newScope.objSetUCProp.StrEntityField.match("^" + formodel.dictAttributes.sfwEntity)) {
                    sfxControlModel.dictAttributes.sfwEntityField = formodel.dictAttributes.sfwEntity + "." + newScope.objSetUCProp.StrEntityField;
                }
                else {
                    sfxControlModel.dictAttributes.sfwEntityField = newScope.objSetUCProp.StrEntityField;
                }
            }
            else {
                sfxControlModel.dictAttributes.sfwEntityField = newScope.objSetUCProp.StrEntityField;
            }
            sfxControlModel.dictAttributes.sfwResource = newScope.objSetUCProp.StrResource;

            if (sfxControlModel.dictAttributes.Name != undefined && sfxControlModel.dictAttributes.Name != "") {
                var fileList = [];
                var obj = { FileName: sfxControlModel.dictAttributes.Name, ID: sfxControlModel.dictAttributes.ID };
                fileList.push(obj);
                $.connection.hubForm.server.getUserControlModel(fileList, "").done(function (udcFileList) {
                    var formScope = getCurrentFileScope();
                    formScope.receiveUcMainTable(udcFileList);
                });
            }
            $rootScope.PushItem(sfxControlModel, cellVM.Elements);
            newScope.onUserControlCancelClick();
        };

        newScope.onUserControlCancelClick = function () {
            var formScope = getCurrentFileScope();
            if (formScope && formScope.validationErrorList) {
                $ValidationService.removeObjInToArray(formScope.validationErrorList, newScope.objSetUCProp);
            }
            if (ucPropDialog) {
                ucPropDialog.close();
            }
        };

        newScope.ValidateUserProp = function () {
            var retVal = false;
            newScope.ErrorMessageForDisplay = "";
            if (newScope.objSetUCProp.StrId == undefined || newScope.objSetUCProp.StrId == "") {
                newScope.ErrorMessageForDisplay = "Error: Enter the ID.";
                retVal = true;
            }
            else {
                var lstIds = [];
                CheckforDuplicateID(formodel, newScope.objSetUCProp.StrId, lstIds);
                if (lstIds.length > 0) {
                    newScope.ErrorMessageForDisplay = "Error: Duplicate ID.";
                    retVal = true;
                } else if (!isValidIdentifier(newScope.objSetUCProp.StrId, false, false)) {
                    newScope.ErrorMessageForDisplay = "Error: Invalid ID.";
                    retVal = true;
                }
            }
            if (!newScope.objSetUCProp.StrName || newScope.objSetUCProp.StrName == '') {
                newScope.ErrorMessageForDisplay = "Please Enter Active Form.";
                retVal = true;
            }
            //else if (!newScope.objSetUCProp.StrEntityField || newScope.objSetUCProp.StrEntityField == "") {
            //    newScope.ErrorMessageForDisplay = "Please Enter Entity field.";
            //    retVal = true;
            //}
            else if (!newScope.objSetUCProp.StrResource || newScope.objSetUCProp.StrResource == '') {
                newScope.ErrorMessageForDisplay = "Please Enter Resource.";
                retVal = true;
            }

            if (newScope.ErrorMessageForDisplay == undefined || newScope.ErrorMessageForDisplay == "") {
                if (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "") {
                    var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(newScope.formodel.dictAttributes.sfwEntity, newScope.objSetUCProp.StrEntityField);
                    if (!object || object.Type != "Object") {
                        newScope.ErrorMessageForDisplay = "Entity Field should be Object.";
                        retVal = true;
                    }
                }
            }
            if (newScope.objSetUCProp.errors && $ValidationService.isEmptyObj(newScope.objSetUCProp.errors)) {
                retVal = true;
            }
            return retVal;
        };

        var ucPropDialog = $rootScope.showDialog(newScope, "User Control", "Form/views/SetUserControlProperties.html");
    }

    //#endregion


};