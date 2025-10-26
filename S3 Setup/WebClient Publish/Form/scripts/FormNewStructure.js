app.directive("panelaccordiantemplate", ["$compile", "$rootScope", "$EntityIntellisenseFactory", "$timeout", "$SgMessagesService", function ($compile, $rootScope, $EntityIntellisenseFactory, $timeout, $SgMessagesService) {
    return {
        restrict: "E",
        scope: {
            model: '=',
            formodel: '=',
            objparent: '=',
            parentaccordianid: '=',
            initialvisibilty: '=',
            panelindex: '@',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            lstdisplayentities: '=',
            entitytreename: '=',
            buttonsCollection: '='
        },
        replace: true,
        templateUrl: "Form/views/Controls/PanelControlView.html",
        link: function (scope, element, attributes) {
            var curscope = getCurrentFileScope();
            scope.ShowPanelButtons = function () {
                if (curscope && curscope.CurrPanel && curscope.CurrPanel.Name != "sfwDialogPanel") {
                    if (curscope.CurrPanel.dictAttributes.ID && curscope.CurrPanel.dictAttributes.ID == scope.model.dictAttributes.ID) {
                        return true;
                    }
                }
                return false;
            };
            scope.setInitialVisibility = function () {
                scope.initialvisibilty = !scope.initialvisibilty;
            };
            scope.objSelectionClassAttributes = {};
            scope.ChangeSelection = function (value, param) {
                if (curscope) {
                    if (param == "LookupAdvCriteria") {
                        if (value == 'panel-adv-criteria-icon active') {
                            curscope.IsLookupAdvCriteriaSelected = true;
                        }
                        else {
                            curscope.IsLookupAdvCriteriaSelected = false;
                        }
                    }
                    if (param == "LookupAdvSort") {
                        if (value == 'panel-adv-sort-icon active') {
                            curscope.IsLookupAdvSortSelected = true;
                        }
                        else {
                            curscope.IsLookupAdvSortSelected = false;
                        }
                    }

                    if (param == "LookupQuery") {
                        if (value == 'panel-query-icon active') {
                            curscope.IsLookupQuerySelected = true;
                        }
                        else {
                            curscope.IsLookupQuerySelected = false;
                        }
                    }
                }
            };
            Object.defineProperty(scope.objSelectionClassAttributes, "LookupAdvCriteria", {
                get: function () {
                    if (curscope && curscope.IsLookupAdvCriteriaSelected) {
                        return 'panel-adv-criteria-icon active';
                    }
                    else {
                        return 'panel-adv-criteria-icon';
                    }
                },
                set: function (value) {
                    scope.ChangeSelection(value, "LookupAdvCriteria");
                },
            });
            Object.defineProperty(scope.objSelectionClassAttributes, "LookupAdvSort", {
                get: function () {
                    if (curscope && curscope.IsLookupAdvSortSelected) {
                        return 'panel-adv-sort-icon active';
                    }
                    else {
                        return 'panel-adv-sort-icon';
                    }
                },
                set: function (value) {
                    scope.ChangeSelection(value, "LookupAdvSort");
                },
            });
            Object.defineProperty(scope.objSelectionClassAttributes, "LookupQuery", {
                get: function () {
                    if (curscope && curscope.IsLookupQuerySelected) {
                        return 'panel-query-icon active';
                    }
                    else {
                        return 'panel-query-icon';
                    }
                },
                set: function (value) {
                    scope.ChangeSelection(value, "LookupQuery");
                },
            });
            //scope.setSelection();
            scope.SetParentVM = function (parent, model) {
                model.ParentVM = parent;
            };

            if (scope.objparent) {
                scope.model.ParentVM = scope.objparent;
            }

            scope.getValue = function (item) {
                return getDisplayValue(item);
            };

            scope.getColspan = function (item) {
                if (item && item.dictAttributes.ColumnSpan != undefined && item.dictAttributes.ColumnSpan != "") {
                    return item.dictAttributes.ColumnSpan;
                }
                else {
                    return 1;
                }
            };


            scope.loadFormEntityTree = function (item) {
                if (scope.FormModel.IsLookupCriteriaEnabled == false) {
                    curscope.IsGridSeleected = false;

                    var blnFound = false;
                    for (var i = 0; i < curscope.lstLoadedEntityTrees.length; i++) {
                        if (curscope.lstLoadedEntityTrees[i].EntityName == scope.formodel.dictAttributes.sfwEntity && !curscope.lstLoadedEntityTrees[i].IsQuery) {
                            blnFound = true;
                            curscope.lstLoadedEntityTrees[i].IsVisible = true;
                            curscope.entityTreeName = curscope.lstLoadedEntityTrees[i].EntityName;
                            curscope.currentEntiyTreeObject = curscope.lstLoadedEntityTrees[i];
                        }
                        else {
                            curscope.lstLoadedEntityTrees[i].IsVisible = false;

                        }

                    }
                    if (!blnFound) {
                        var objNew = { EntityName: scope.formodel.dictAttributes.sfwEntity, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: false };
                        curscope.lstLoadedEntityTrees.push(objNew);
                        curscope.entityTreeName = objNew.EntityName;
                        curscope.currentEntiyTreeObject = objNew;
                    }
                }
            };

            scope.selectControl = function (objChild, event) {
                SetFormSelectedControl(scope.formodel, objChild, event);

                scope.loadFormEntityTree(objChild);

            };


            //#region lookup functionality for criteria tabs


            scope.OnAdvCriteriaClick = function (event) {
                scope.CheckAndUpdateCriteria("AdvCriteria", event);
            };

            scope.OnAdvSortClick = function (event) {
                scope.CheckAndUpdateCriteria("AdvSort", event);
            };

            scope.OnQueryClick = function (event) {
                scope.CheckAndUpdateCriteria("Query", event);
            };

            scope.ChangeSelectionProperty = function (criteriaName) {
                if (curscope && criteriaName == "Query") {
                    if (curscope.IsLookupQuerySelected) {
                        $rootScope.EditPropertyValue('panel-query-icon', scope.objSelectionClassAttributes, "LookupQuery", 'panel-query-icon active');
                    }
                    else {
                        $rootScope.EditPropertyValue('panel-query-icon active', scope.objSelectionClassAttributes, "LookupQuery", 'panel-query-icon');
                    }
                }
                if (curscope && criteriaName == "AdvSort") {
                    if (curscope.IsLookupAdvSortSelected) {
                        $rootScope.EditPropertyValue('panel-adv-sort-icon', scope.objSelectionClassAttributes, "LookupAdvSort", 'panel-adv-sort-icon active');
                    }
                    else {
                        $rootScope.EditPropertyValue('panel-adv-sort-icon active', scope.objSelectionClassAttributes, "LookupAdvSort", 'panel-adv-sort-icon');
                    }
                }
                if (curscope && criteriaName == "AdvCriteria") {
                    if (curscope.IsLookupAdvCriteriaSelected) {
                        $rootScope.EditPropertyValue('panel-adv-criteria-icon', scope.objSelectionClassAttributes, "LookupAdvCriteria", 'panel-adv-criteria-icon active');
                    }
                    else {
                        $rootScope.EditPropertyValue('panel-adv-criteria-icon active', scope.objSelectionClassAttributes, "LookupAdvCriteria", 'panel-adv-criteria-icon');
                    }
                }

            };
            scope.CheckAndUpdateCriteria = function (criteriaName, event) {
                $rootScope.UndRedoBulkOp("Start");

                var sfxTabContainerModel = null;
                var TableVM = curscope.CurrPanel.Elements[0];


                if (TableVM) {
                    var RowCount = TableVM.Elements.length;
                    var rowVM = TableVM.Elements[0];
                    var ColCount = rowVM.Elements.length;
                    if (RowCount == 1 && ColCount == 1) {
                        var sfxCellVM = rowVM.Elements[0];
                        if (sfxCellVM.Elements.length > 0 && sfxCellVM.Elements[0].Name == "sfwTabContainer") {
                            sfxTabContainerModel = sfxCellVM.Elements[0];
                        }
                    }
                }

                if (!sfxTabContainerModel) {
                    var strPanelID = curscope.CurrPanel.dictAttributes.ID;
                    var strMainTableID = "";
                    var strTabContainerID = "";

                    if (startsWith(strPanelID, "pnl", 0)) {
                        strMainTableID = strPanelID.substring(3);
                        strTabContainerID = strPanelID.substring(3);
                    }
                    else {
                        strMainTableID = strPanelID;
                        strTabContainerID = strPanelID;
                    }

                    var prefix = "swc";
                    var sfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxTableModel.ParentVM = curscope.CurrPanel;
                    sfxTableModel.dictAttributes.ID = strMainTableID;

                    sfxTabContainerModel = { Name: "sfwTabContainer", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxTabContainerModel.ParentVM = sfxTableModel;
                    strTabContainerID = CreateControlID(scope.formodel, strTabContainerID, "sfwTabContainer", false);
                    sfxTabContainerModel.dictAttributes.ID = strTabContainerID;

                    var tabsModel = { Name: "Tabs", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: [] };
                    tabsModel.ParentVM = sfxTabContainerModel;

                    var sfxTabSheetModel = { Name: "sfwTabSheet", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxTabSheetModel.ParentVM = tabsModel;

                    var strTabSheetID = "";

                    var strTableID = TableVM.dictAttributes.ID;

                    if (startsWith(strTableID, "tbl", 0)) {
                        strTabSheetID = strTableID.substring(3);
                    }
                    else {
                        strTabSheetID = strTableID;
                    }

                    strTabSheetID = CreateControlID(scope.formodel, strTabSheetID, "sfwTabSheet", false);
                    sfxTabSheetModel.dictAttributes.ID = strTabSheetID;
                    sfxTabSheetModel.dictAttributes.HeaderText = "Criteria";

                    sfxTabSheetModel.Elements.push(TableVM);
                    tabsModel.Elements.push(sfxTabSheetModel);
                    sfxTabContainerModel.Elements.push(tabsModel);


                    var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxRowModel.ParentVM = sfxTableModel;

                    var sfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                    sfxCellModel.ParentVM = sfxRowModel;


                    sfxCellModel.Elements.push(sfxTabContainerModel);

                    sfxRowModel.Elements.push(sfxCellModel);
                    sfxTableModel.Elements.push(sfxRowModel);

                    while (curscope.CurrPanel.Elements.length > 0) {
                        $rootScope.DeleteItem(curscope.CurrPanel.Elements[0], curscope.CurrPanel.Elements);
                    }

                    $rootScope.PushItem(sfxTableModel, curscope.CurrPanel.Elements);

                }

                function iUpdateCriteriaState(sfxtabSheet) {
                    var strTabName = sfxtabSheet.dictAttributes.ID;
                    if (criteriaName == "AdvCriteria" && strTabName.contains("tshAdvCriteria")) {
                        tabSheetVMToRemove = sfxtabSheet;
                        isTabContain = true;
                        curscope.IsLookupAdvCriteriaSelected = false;

                    }
                    else if (criteriaName == "AdvSort" && strTabName.contains("tshAdvSort")) {
                        tabSheetVMToRemove = sfxtabSheet;
                        isTabContain = true;
                        curscope.IsLookupAdvSortSelected = false;

                    }
                    else if (criteriaName == "Query" && strTabName.contains("tshSql")) {
                        tabSheetVMToRemove = sfxtabSheet;
                        isTabContain = true;
                        curscope.IsLookupQuerySelected = false;
                    }
                }
                if (sfxTabContainerModel) {
                    var tabsVM = sfxTabContainerModel.Elements[0];
                    if (tabsVM) {
                        tabsVM.LookupCriPanelVM = curscope.CurrPanel;
                        var tabSheetVMToRemove;
                        var isTabContain = false;

                        angular.forEach(tabsVM.Elements, iUpdateCriteriaState);

                        if (isTabContain) {
                            $rootScope.DeleteItem(tabSheetVMToRemove, tabsVM.Elements);

                            if (tabsVM.Elements.length == 1) {
                                if (tabsVM.Elements[0].Elements[0].Name == "sfwTable") {
                                    var tableModel = tabsVM.Elements[0].Elements[0];

                                    while (curscope.CurrPanel.Elements.length > 0) {
                                        $rootScope.DeleteItem(curscope.CurrPanel.Elements[0], curscope.CurrPanel.Elements);
                                    }

                                    $rootScope.PushItem(tableModel, curscope.CurrPanel.Elements);
                                }
                            }
                        }
                        else {
                            var sfxTabSheetModel;
                            if (criteriaName == "AdvCriteria") {
                                sfxTabSheetModel = scope.AddAdvCriteriaPanel(tabsVM);
                                curscope.IsLookupAdvCriteriaSelected = true;

                            }
                            else if (criteriaName == "AdvSort") {
                                sfxTabSheetModel = scope.AddAdvSortPanel(tabsVM);
                                curscope.IsLookupAdvSortSelected = true;

                            }
                            else if (criteriaName == "Query") {
                                sfxTabSheetModel = scope.AddQueryPanel(tabsVM);
                                curscope.IsLookupQuerySelected = true;

                            }

                            if (tabsVM) {
                                $rootScope.PushItem(sfxTabSheetModel, tabsVM.Elements);
                                if (!sfxTabSheetModel.isLoaded) {
                                    sfxTabSheetModel.isLoaded = true;
                                }
                                tabsVM.SelectedTabSheet = sfxTabSheetModel;
                                tabsVM.SelectedTabSheet.IsSelected = true;
                                SetFormSelectedControl(scope.formodel, sfxTabSheetModel, undefined);
                            }
                        }

                    }
                }
                scope.ChangeSelectionProperty(criteriaName);
                $rootScope.UndRedoBulkOp("End");
                if (event) {
                    event.stopPropagation();
                }
            };



            scope.AddAdvCriteriaPanel = function (aParent) {
                var prefix = "swc";
                var newSfxTabSheetModel = { Name: "sfwTabSheet", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTabSheetModel.ParentVM = aParent;
                newSfxTabSheetModel.dictAttributes.ID = "tshAdvCriteria";
                newSfxTabSheetModel.dictAttributes.HeaderText = "Adv. Criteria";

                var newSfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTableModel.ParentVM = newSfxTabSheetModel;
                newSfxTableModel.dictAttributes.ID = "tblAdvCriteria";

                var newSfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxRowModel.ParentVM = newSfxTableModel;

                var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                var newControl = { Name: "sfwButton", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "btnAdvSearch";
                newControl.dictAttributes.sfwMethodName = "btnSearch_Click";
                newControl.dictAttributes.sfwOperateOn = "tblAdvCriteria";
                newControl.dictAttributes.Text = "Adv Search";
                newSfxCellModel.Elements.push(newControl);

                newControl = {
                    Name: "sfwButton", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "btnAdvReset";
                newControl.dictAttributes.sfwMethodName = "btnReset_Click";
                newControl.dictAttributes.sfwOperateOn = "tblAdvCriteria";
                newControl.dictAttributes.Text = "Adv Reset";
                newSfxCellModel.Elements.push(newControl);

                newSfxRowModel.Elements.push(newSfxCellModel);

                newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                newControl = {
                    Name: "sfwButton", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "btnAdvStoreSearch";
                newControl.dictAttributes.sfwMethodName = "btnStoreUserDefaults_Click";
                newControl.dictAttributes.sfwOperateOn = "tblAdvCriteria";
                newControl.dictAttributes.Text = "Adv Store Search";
                newSfxCellModel.Elements.push(newControl);

                newSfxRowModel.Elements.push(newSfxCellModel);


                newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                newSfxRowModel.Elements.push(newSfxCellModel);

                newSfxTableModel.Elements.push(newSfxRowModel);
                newSfxTabSheetModel.Elements.push(newSfxTableModel);

                return newSfxTabSheetModel;
            };

            scope.AddAdvSortPanel = function (aParent) {
                var prefix = "swc";
                var newSfxTabSheetModel = { Name: "sfwTabSheet", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTabSheetModel.ParentVM = aParent;
                newSfxTabSheetModel.dictAttributes.ID = "tshAdvSort";
                newSfxTabSheetModel.dictAttributes.HeaderText = "Adv. Sort";

                var newSfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTableModel.ParentVM = newSfxTabSheetModel;
                newSfxTableModel.dictAttributes.ID = "tblAdvSort";

                var newSfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxRowModel.ParentVM = newSfxTableModel;

                var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                var newControl = { Name: "sfwAdvSort", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "AdvSort";
                newControl.dictAttributes.sfwRelatedControl = "dgrResult";

                newSfxCellModel.Elements.push(newControl);
                newSfxRowModel.Elements.push(newSfxCellModel);
                newSfxTableModel.Elements.push(newSfxRowModel);

                newSfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxRowModel.ParentVM = newSfxTableModel;

                newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                var newControl = { Name: "sfwButton", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "btnApplySort";
                newControl.dictAttributes.sfwMethodName = "btnApplySort_Click";
                newControl.dictAttributes.sfwOperateOn = "tblAdvSort";
                newControl.dictAttributes.sfwRelatedControl = "dgrResult";
                newControl.dictAttributes.Text = "Apply Sort";

                newSfxCellModel.Elements.push(newControl);
                newSfxRowModel.Elements.push(newSfxCellModel);
                newSfxTableModel.Elements.push(newSfxRowModel);

                newSfxTabSheetModel.Elements.push(newSfxTableModel);

                return newSfxTabSheetModel;




            };

            scope.AddQueryPanel = function (aParent) {
                var prefix = "swc";
                var newSfxTabSheetModel = { Name: "sfwTabSheet", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTabSheetModel.ParentVM = aParent;
                newSfxTabSheetModel.dictAttributes.ID = "tshSql";
                newSfxTabSheetModel.dictAttributes.HeaderText = "Query";

                var newSfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxTableModel.ParentVM = newSfxTabSheetModel;
                newSfxTableModel.dictAttributes.ID = "tblSql";

                var newSfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxRowModel.ParentVM = newSfxTableModel;

                var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newSfxCellModel.ParentVM = newSfxRowModel;

                var newControl = { Name: "sfwTextBox", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                newControl.ParentVM = newSfxCellModel;
                newControl.dictAttributes.ID = "txbSql";
                newControl.dictAttributes.TextMode = "MultiLine";
                newControl.dictAttributes.Rows = "5";

                newSfxCellModel.Elements.push(newControl);
                newSfxRowModel.Elements.push(newSfxCellModel);
                newSfxTableModel.Elements.push(newSfxRowModel);
                newSfxTabSheetModel.Elements.push(newSfxTableModel);

                return newSfxTabSheetModel;
            };

            //#endregion

            //#region Wizard Functionality(Add, Delete, Move Up , Move Down)


            //#region Delete Step
            scope.DeleteWizardStepClick = function () {
                if (curscope.CurrPanel) {
                    if (curscope.CurrPanel.Name == "HeaderTemplate") {
                        $SgMessagesService.Message('Message', 'HeaderTemplate can not be deleted.');
                    }
                    else {
                        $SgMessagesService.Message('Delete', "The selected wizard " + curscope.CurrPanel.dictAttributes.Title + " will be deleted.\nAre you sure you want to delete it?", true, function (result) {
                            if (result) {
                                $rootScope.UndRedoBulkOp("Start");
                                var sfxRow = curscope.CurrPanel.ParentVM;
                                var index = curscope.MainPanels.indexOf(curscope.CurrPanel);
                                $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels, "selectPanelControl");

                                if (sfxRow) {
                                    $rootScope.DeleteItem(curscope.CurrPanel, sfxRow.Elements);
                                }

                                if (index < curscope.MainPanels.length) {
                                    curscope.selectPanelControl(curscope.MainPanels[index]);
                                }
                                else if (curscope.MainPanels.length > 0) {
                                    curscope.selectPanelControl(curscope.MainPanels[index - 1]);
                                }
                                $rootScope.UndRedoBulkOp("End");
                            }
                        });
                    }
                }
            };

            //#endregion

            //#region Move Up
            scope.scrollPanel = function () {
                $timeout(function () {
                    var $divDom = $("#" + scope.formodel.dictAttributes.ID).find(".form-area-editable");
                    if ($divDom && $divDom.hasScrollBar()) {
                        $divDom.scrollTo($divDom.find(".active.form-panel-wrapper"), { offsetTop: 0, offsetLeft: 0 }, null);
                        return false;
                    }

                });
            }

            scope.MoveUpWizardClick = function () {
                if (curscope.CurrPanel) {
                    var sfxRow = curscope.CurrPanel.ParentVM;
                    if (sfxRow) {
                        var index = sfxRow.Elements.indexOf(curscope.CurrPanel);
                        if (index > -1) {
                            if (index == 0) {
                                $SgMessagesService.Message('Message', "Wizard step cannot be arranged before header template.");
                            }
                            else {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.DeleteItem(curscope.CurrPanel, sfxRow.Elements);
                                $rootScope.InsertItem(curscope.CurrPanel, sfxRow.Elements, index - 1);

                                var panelIndex = curscope.MainPanels.indexOf(curscope.CurrPanel);
                                $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels);
                                $rootScope.InsertItem(curscope.CurrPanel, curscope.MainPanels, panelIndex - 1);
                                $rootScope.UndRedoBulkOp("End");
                                scope.scrollPanel();
                            }
                        }
                    }
                }
            };

            scope.canMoveUpWizardDisable = function () {
                var retVal = false;
                if (curscope.CurrPanel) {
                    var sfxRow = curscope.CurrPanel.ParentVM;
                    if (sfxRow) {
                        var index = sfxRow.Elements.indexOf(curscope.CurrPanel);
                        if (index == 0) {
                            retVal = true;
                        }
                    }
                }
                else {
                    retVal = true;
                }
                return retVal;
            };


            //#endregion

            //#region Move Down


            scope.MoveDownWizardClick = function () {
                if (curscope.CurrPanel) {
                    var sfxRow = curscope.CurrPanel.ParentVM;
                    if (sfxRow) {
                        var index = sfxRow.Elements.indexOf(curscope.CurrPanel);
                        if (index > -1) {
                            if (curscope.MainPanels.length > index + 1) {
                                $rootScope.UndRedoBulkOp("Start");

                                $rootScope.DeleteItem(curscope.CurrPanel, sfxRow.Elements);
                                $rootScope.InsertItem(curscope.CurrPanel, sfxRow.Elements, index + 1);

                                var panelIndex = curscope.MainPanels.indexOf(curscope.CurrPanel);
                                $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels);
                                $rootScope.InsertItem(curscope.CurrPanel, curscope.MainPanels, panelIndex + 1);
                                $rootScope.UndRedoBulkOp("End");
                                scope.scrollPanel();
                            }
                        }
                    }
                }
            };


            scope.canMoveDownWizardDisable = function () {
                var retVal = false;
                if (curscope.CurrPanel) {
                    var sfxRow = curscope.CurrPanel.ParentVM;
                    if (sfxRow) {
                        var index = sfxRow.Elements.indexOf(curscope.CurrPanel);
                        if (sfxRow.Elements.length == index + 1) {
                            retVal = true;
                        }
                    }
                }
                else {
                    retVal = true;
                }
                return retVal;
            };

            //#endregion
            //#endregion

            //#region Panel Functions
            scope.addPanel = function (type, index) {
                event.stopPropagation();
                switch (type) {
                    case "wizard":
                        curscope.OnAddWizardStepClick(index, true);
                        break;
                    case "maintenance":
                    case "tooltip":
                        curscope.OnAddPanelClick(index, true);
                        break;
                }
            };


            scope.DeleteMainPanelClick = function () {
                if (curscope.CurrPanel) {
                    $SgMessagesService.Message('Delete', "The selected panel " + curscope.CurrPanel.dictAttributes.ID + " will be deleted.\nAre you sure you want to delete it?", true, function (result) {
                        if (result) {
                            $rootScope.UndRedoBulkOp("Start");
                            var sfxRow = FindParent(curscope.CurrPanel, "sfwRow");
                            var index = curscope.MainPanels.indexOf(curscope.CurrPanel);
                            $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels, "selectPanelControl");

                            if (index < curscope.MainPanels.length) {
                                curscope.selectPanelControl(curscope.MainPanels[index]);
                            }
                            else if (curscope.MainPanels.length > 0) {
                                curscope.selectPanelControl(curscope.MainPanels[index - 1]);
                            }

                            if (sfxRow) {
                                $rootScope.DeleteItem(sfxRow, curscope.SfxMainTable.Elements);
                            }

                            $rootScope.UndRedoBulkOp("End");
                        }
                    });
                }
            };

            scope.MoveUpMainPanelClick = function () {
                if (curscope.CurrPanel) {
                    var sfxRow = FindParent(curscope.CurrPanel, "sfwRow");
                    if (sfxRow) {
                        var index = curscope.SfxMainTable.Elements.indexOf(sfxRow);
                        if (index > -1) {
                            if (index > 0) {
                                $rootScope.UndRedoBulkOp("Start");
                                $rootScope.DeleteItem(sfxRow, curscope.SfxMainTable.Elements);
                                $rootScope.InsertItem(sfxRow, curscope.SfxMainTable.Elements, index - 1);

                                var panelIndex = curscope.MainPanels.indexOf(curscope.CurrPanel);
                                $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels);
                                $rootScope.InsertItem(curscope.CurrPanel, curscope.MainPanels, panelIndex - 1);
                                $rootScope.UndRedoBulkOp("End");
                                scope.scrollPanel();
                            }
                        }
                    }
                }
            };

            scope.MoveDownMainPanelClick = function () {
                if (curscope.CurrPanel) {
                    var sfxRow = FindParent(curscope.CurrPanel, "sfwRow");
                    if (sfxRow) {
                        var index = curscope.SfxMainTable.Elements.indexOf(sfxRow);
                        if (index > -1) {
                            if (curscope.MainPanels.length > index + 1) {
                                $rootScope.UndRedoBulkOp("Start");

                                $rootScope.DeleteItem(sfxRow, curscope.SfxMainTable.Elements);
                                $rootScope.InsertItem(sfxRow, curscope.SfxMainTable.Elements, index + 1);

                                var panelIndex = curscope.MainPanels.indexOf(curscope.CurrPanel);
                                $rootScope.DeleteItem(curscope.CurrPanel, curscope.MainPanels);
                                $rootScope.InsertItem(curscope.CurrPanel, curscope.MainPanels, panelIndex + 1);
                                $rootScope.UndRedoBulkOp("End");
                                scope.scrollPanel();
                            }
                        }
                    }
                }
            };

            scope.canMoveUpDisable = function () {
                var retVal = false;
                if (curscope.CurrPanel) {
                    var sfxRow = FindParent(curscope.CurrPanel, "sfwRow");
                    if (sfxRow) {
                        var index = curscope.SfxMainTable.Elements.indexOf(sfxRow);
                        if (index == 0) {
                            retVal = true;
                        }
                    }
                }
                else {
                    retVal = true;
                }
                return retVal;
            };

            scope.canMoveDownDisable = function () {
                var retVal = false;
                if (curscope.CurrPanel) {
                    var sfxRow = FindParent(curscope.CurrPanel, "sfwRow");
                    if (sfxRow) {
                        var index = curscope.SfxMainTable.Elements.indexOf(sfxRow);
                        if (curscope.SfxMainTable.Elements.length == index + 1) {
                            retVal = true;
                        }
                    }
                }
                else {
                    retVal = true;
                }
                return retVal;
            };

            scope.getAddpanelTrigger = function (event, sfwType, index) {
                sfwType = sfwType ? sfwType.trim().toLowerCase() : "";
                var insertindex = index != undefined ? parseInt(index) + 1 : 0;
                var title = "Add Panel";
                if (scope.formodel.dictAttributes.sfwType == "Wizard") {
                    title = "Add Wizard Step";
                }
                if (scope.formodel.dictAttributes.sfwType != "Lookup") {
                    elhover = $compile('<button class="addPanelDivTrigger" ng-click="addPanel(' + '\'' + sfwType + '\'' + ',' + insertindex + ')">' + title + '</button>')(scope);
                    elhover.css({
                        left: event.clientX - 50
                    });
                    if (curscope && !curscope.IsToolsDivCollapsed) {
                        var toolboxwrapper = $("div[id='" + curscope.currentfile.FileName + "']").find(".toolbox-wrapper");
                        elhover.css({
                            left: event.clientX - 20 - toolboxwrapper.width()
                        });
                    }
                    event.currentTarget.appendChild(elhover[0]);
                }
            };

            scope.deleteAddpanelTrigger = function () {
                $(".addPanelDivTrigger").remove();
            };

            scope.panelMenuOption = [
                ['Create Compatible Caption', function ($itemScope) {
                    var filescope = getCurrentFileScope();
                    if (scope.model) {
                        if (filescope && filescope.OnCreateCompatibleLabelClickForPanel) {
                            filescope.OnCreateCompatibleLabelClickForPanel(scope.model);
                        }
                    }
                }]
            ];

            //#endregion
        }
    };
}]);

function growtextbox(event, isCurrentElemet, text) {
    var comfortZone = 10;
    var newWidth = 0;
    var maxWidth = 1000;
    var minWidth = 100;
    var input = "";
    var val = "";
    if (!isCurrentElemet) {
        input = $(event.currentTarget);
        val = input.val();
    } else {
        input = event;
        val = text;
    }
    var orginalwidth = input.width();
    input.width("auto");
    var defaultwidth = input.width();
    var testSubject = $('<tester/>').css({
        position: 'absolute',
        top: -9999,
        left: -9999,
        width: 'auto',
        fontSize: input.css('fontSize'),
        fontFamily: input.css('fontFamily'),
        fontWeight: input.css('fontWeight'),
        letterSpacing: input.css('letterSpacing'),
        whiteSpace: 'nowrap',
        display: 'block',
        visibility: 'hidden'
    });
    var escaped = val.replace(/&/g, '&amp;').replace(/\s/g, '&nbsp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    testSubject.html(escaped);
    $("body").append(testSubject);
    var width = testSubject.width();
    // Calculate new width + whether to change
    var testerWidth = testSubject.width();
    testSubject.remove();
    if (testerWidth < minWidth) {
        newWidth = minWidth;
    } else if (testerWidth > minWidth) {
        newWidth = testerWidth;
    }
    input.width(newWidth + comfortZone);
}

function funcontrolDragEnter(element, model, formodel) {
    if (model && model.IsSelected && element && ((!(element.draggable) && ($(element).children("[content-editable]").length <= 0)) || (($(element).children("[content-editable]").length > 0) && $(element).children("[drag-handler-btn]").length <= 0))) {
        if (formodel.dictAttributes.sfwType == "Lookup" && formodel.SelectedControl.Name == "sfwGridView") {
            return;
        }
        //var htmldragTrigger = $('<i class="fa fa-circle select-label-control" drag-handler-btn aria-hidden="true" style="z-index:999;position:absolute;left:-2px;top:-5px;font-size:8px;"></i><i class="fa fa-circle select-label-control" drag-handler-btn aria-hidden="true" style="z-index:999;position:absolute;right:-2px;top:-5px;font-size:8px;"></i><i class="fa fa-circle select-label-control" drag-handler-btn aria-hidden="true" style="z-index:999;position:absolute;left:-2px;bottom:-5px;font-size:8px;"></i><i class="fa fa-circle select-label-control" drag-handler-btn aria-hidden="true" style="z-index:999;position:absolute;right:-2px;bottom:-5px;font-size:8px;"></i><div drag-handler-btn style="clear: both"></div>');
        //var htmlcontroldragwrapper = $("<div drag-handler-wrapper class='control-drag-wrapper' style='position:relative;border:1px solid #e77b21;box-shadow:0 0 8px #e77b21;'></div>");
        //// check if any parent has been activated for drag - drop
        //var lstDragParents = $(e.currentTarget).parents("[drag-handler-wrapper]");
        //if (lstDragParents.length > 0) {
        //    lstDragParents.find("[drag-handler-btn]").remove();
        //    lstDragParents.children().first().unwrap();
        //}
        //$(e.currentTarget).wrap(htmlcontroldragwrapper);
        //$(e.currentTarget).parent("[drag-handler-wrapper]").append(htmldragTrigger);
        //var el = $(e.currentTarget).parent("[drag-handler-wrapper]")[0];
        var el = element;
        if ($(element).children("[content-editable]").length > 0) {
            $(element).prepend("<span drag-handler-btn  class='drag-text-control'></span>");
            el = $(element).children("[drag-handler-btn]")[0];
        }
        el.draggable = true;
        el.addEventListener('dragstart', handleDragStart, false);
        function handleDragStart(e) {
            $(e.currentTarget).closest("[formcontroldroppable]").find("[add-control-hover-trigger]").remove();
            dragDropData = null;
            dragDropDataObject = null;
            dragfromleft = undefined;
            var imgObj = $(e.currentTarget)[0];
            if ($(e.currentTarget).siblings("[content-editable]").length > 0) {
                imgObj = $(e.currentTarget).parent()[0];
            }
            if (!detectIE()) {
                e.dataTransfer.setDragImage(imgObj, 5, 5);
            }
            dragDropData = model;
            draggingDataFromLeft = null;
            dragDropDataObject = null;
            e.stopPropagation();
        }
    }
}