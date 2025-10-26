app.directive('commonformlinkpropertiesdirective', ["$compile", "$rootScope", "ngDialog", "$filter", "share", "$EntityIntellisenseFactory", "$NavigateToFileService", "hubcontext", "$GetEntityFieldObjectService", "$Entityintellisenseservice", "$getModelList", "CONSTANTS", function ($compile, $rootScope, ngDialog, $filter, share, $EntityIntellisenseFactory, $NavigateToFileService, hubcontext, $GetEntityFieldObjectService, $Entityintellisenseservice, $getModelList, CONST) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            parent: '=',
            formobject: '=',
            currentPanel: '=',
            sharedata: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: "="
        },
        link: function (scope, element, attributes) {
            var unwatch = scope.$watch('formobject.dictAttributes.sfwEntity', function (newVal, oldVal) {
                if (scope.formobject) {
                    scope.setEntityName();
                }
            });
            //#region Init Section
            scope.Init = function () {
                scope.IsLookup = scope.formobject.dictAttributes.sfwType == "FormLinkLookup" ? true : false;
                scope.LoadModes();
                scope.LoadDataFormats();
                scope.LoadTextMode();
                scope.LoadDefaultTypes();
                scope.LoadOperators();
                scope.LoadValidatorOperators();
                scope.LoadSelectionMode();
                scope.LoadRepeatDirection();
                scope.LoadAlignment();
                scope.LoadGridSelection();
                scope.getGroupList();

                scope.LoadDataType();
                scope.LoadType();
                scope.LoadAxisFormatsForChart();
                scope.lstMethodType = [];
                scope.lstMethodType.push("ObjectMethod");
                scope.lstMethodType.push("XmlMethod");
                scope.lstMethodType.push("Rule");
            };

            scope.setEntityNameBasedonControl = function () {
                var entityforCell = "";
                scope.entityName = "";
                var ObjGrid = FindParent(scope.model, "sfwGridView");
                if (scope.model) {
                    //For template field we need from main entity

                    if (scope.model.Name == "sfwGridView") {
                        scope.entityName = "";
                        if (scope.model.dictAttributes.sfwParentGrid) {
                            var objParentGridModel = FindControlByID(scope.formobject, scope.model.dictAttributes.sfwParentGrid);
                            if (objParentGridModel && objParentGridModel.dictAttributes.sfwEntityField) {
                                var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objParentGridModel.dictAttributes.sfwEntityField);
                                if (object) {
                                    scope.entityName = object.Entity;
                                }
                            }
                        }
                        else {
                            scope.entityName = scope.formobject.dictAttributes.sfwEntity;
                        }
                    }
                    else if (ObjGrid && scope.model.Name != "TemplateField") {
                        if (scope.formobject && scope.formobject.dictAttributes.sfwType == "FormLinkLookup") {
                            scope.entityName = scope.formobject.dictAttributes.sfwEntity;
                        }
                        else {
                            entityforCell = getEntityBasedOnControl(ObjGrid);
                        }
                    }
                    else if (scope.model.Name == "TemplateField") {
                        scope.entityName = "";
                        var ObjGrid = FindParent(scope.model, "sfwGridView");
                        if (ObjGrid && ObjGrid.dictAttributes.sfwParentGrid) {
                            var objParentGrid = FindControlByID(scope.formobject, ObjGrid.dictAttributes.sfwParentGrid);
                            if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                                if (entObject) {
                                    scope.entityName = entObject.Entity;
                                }
                            }
                        }
                        else if (ObjGrid) {
                            scope.entityName = scope.formobject.dictAttributes.sfwEntity;
                        }
                    }
                    else {
                        var Objdialog = FindParent(scope.model, "sfwDialogPanel");
                        var Objlist = FindParent(scope.model, "sfwListView");
                        if (Objdialog) {
                            entityforCell = getEntityBasedOnControl(Objdialog);
                        }
                        else if (Objlist) {
                            entityforCell = getEntityBasedOnControl(Objlist);
                        }
                        else {
                            scope.entityName = scope.formobject.dictAttributes.sfwEntity;
                        }
                    }


                    if (entityforCell) {
                        var objAttribute;
                        var field = entityforCell;
                        var objGrid = FindParent(scope.model, "sfwGridView");
                        if (objGrid && objGrid.dictAttributes.sfwParentGrid && objGrid.dictAttributes.sfwEntityField) {
                            var objParentGrid = FindControlByID(scope.formobject, objGrid.dictAttributes.sfwParentGrid);
                            if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                                var entObject = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                                if (entObject) {
                                    entityfieldname = entObject.Entity;
                                    if (FindParent(scope.model, "ItemTemplate")) {
                                        var entObj = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(entityfieldname, objGrid.dictAttributes.sfwEntityField);
                                        if (entObj) {
                                            scope.entityName = entObj.Entity;
                                        }
                                    }
                                }
                            }
                        } else if (objGrid && objGrid.dictAttributes.sfwEntityField) {
                            if (scope.model.Name != "TemplateField") {
                                var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objGrid.dictAttributes.sfwEntityField);
                                if (objField) {
                                    scope.entityName = objField.Entity;
                                }
                            }
                        }
                        else {

                            objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, field);
                            if (objAttribute && objAttribute.Entity) {
                                scope.$evalAsync(function () {
                                    scope.entityName = objAttribute.Entity;
                                });
                            }
                        }
                    }
                    else if (scope.model && FindParent(scope.model, "ItemTemplate") && scope.formobject.dictAttributes.sfwType != "FormLinkLookup") {
                        scope.entityName = "";
                    }
                }
            };
            scope.ObjgridBoundedQuery = {};
            scope.setQueryFieldsIfGridisBoundedToQuery = function (objChild) {
                if ((objChild && objChild.dictAttributes.sfwBoundToQuery && objChild.dictAttributes.sfwBoundToQuery.toLowerCase() == "true")) {
                    var QueryID = objChild.dictAttributes.sfwBaseQuery;
                    $.connection.hubForm.server.getEntityQueryColumns(QueryID, "").done(function (data) {
                        scope.$evalAsync(function () {
                            if (data && data.length > 0) {
                                var lstDataFields = data;
                                sortListBasedOnproperty(lstDataFields, "", "CodeID");
                                scope.ObjgridBoundedQuery.lstselectedobjecttreefields = [];
                                scope.ObjgridBoundedQuery.SortedColumns = [];
                                scope.ObjgridBoundedQuery.lstselectedobjecttreefields = $getModelList.getModelListFromQueryFieldlist(lstDataFields);
                                scope.ObjgridBoundedQuery.SortedColumns = scope.ObjgridBoundedQuery.lstselectedobjecttreefields;
                            }
                        });
                    });
                    scope.IsGridSeleected = true;
                } else {
                    scope.ObjgridBoundedQuery.IsQuery = false;
                }
            }

            scope.InitByControl = function () {
                scope.RelatedControls = [];
                scope.RelatedControls.push("");
                scope.lstRelatedGrid = [];
                scope.lstRelatedGrid.push("");
                scope.lstRelatedDialog = [];
                scope.lstServerMethod = [];
                scope.lstBooleanValues = [];
                scope.lstExecuteAfterSuccessButton = [];
                scope.lstCascadingDropDwonParent = [];
                scope.lstCascadingDropDwonParent.push("");

                scope.SelectedTab = "General";
                $(element).find("[header-properties-row][category!='Behaviour'] td").addClass("prop-close");

                scope.MainTable = undefined;
                lstTable = scope.formobject.Elements.filter(function (item) {
                    return item.Name == "items";
                });
                if (lstTable && lstTable.length > 0) {
                    scope.MainTable = lstTable[0];
                }

                // scope.setEntityName();
                if (scope.model.Name == "sfwGridView" && scope.model.dictAttributes.sfwBoundToQuery && scope.model.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                    scope.setQueryFieldsIfGridisBoundedToQuery(scope.model);
                    scope.ObjgridBoundedQuery.IsQuery = true;
                } else {
                    scope.setEntityName();
                    scope.ObjgridBoundedQuery.IsQuery = false;
                }

                if (scope.model && scope.model.Name == "udc") {
                    if (!scope.model.dictAttributes.Visible) {
                        scope.model.dictAttributes.Visible = "True";
                    }
                }

                scope.IsSearchCriteriaSelected = false;
                if (scope.IsLookup) {
                    scope.IsSearchCriteriaSelected = IsCriteriaField(scope.model);
                }
                else {
                    scope.isFilterCriteria = false;
                    var objPanel = FindParent(scope.model, "sfwPanel");
                    if (objPanel) {
                        var lstButtons = [];
                        FindControlListByName(objPanel, "sfwButton", lstButtons);
                        var lstFilterButtons = lstButtons.filter(function (itm) { return itm.dictAttributes.sfwMethodName === "btnGridSearch_Click" });
                        if (lstFilterButtons.some(function (itm) {return !itm.dictAttributes.sfwNavigationParameter || (itm.dictAttributes.sfwNavigationParameter && itm.dictAttributes.sfwNavigationParameter.contains(scope.model.dictAttributes.ID)) })) {
                            scope.isFilterCriteria = true;
                        }
                        else {
                            var lstButtons = getDescendents(scope.formobject, "sfwButton");
                            var lstFilterButtons = lstButtons.filter(function (itm) { return itm.sfwMethodName === "btnGridSearch_Click" });
                            if (lstFilterButtons.some(function (itm) {return itm.dictAttributes.sfwNavigationParameter && itm.dictAttributes.sfwNavigationParameter.contains(scope.model.dictAttributes.ID) })) {
                                scope.isFilterCriteria = true;
                            }
                        }
                    }
                }

                scope.model.IsChildOfGrid = false;
                var objGridView = FindParent(scope.model, "sfwGridView");
                if (objGridView) {
                    scope.objGridView = objGridView;
                    if (objGridView.dictAttributes.sfwBoundToQuery && objGridView.dictAttributes.sfwBoundToQuery.toLowerCase() == "true") {
                        scope.setQueryFieldsIfGridisBoundedToQuery(objGridView);
                        scope.ObjgridBoundedQuery.IsQuery = true;
                    } else {
                        scope.setEntityName();
                        scope.ObjgridBoundedQuery.IsQuery = false;
                    }
                    scope.model.IsChildOfGrid = true;
                }
                function iAddInlstButtonMethod(item) {
                    scope.lstButtonMethod.push(item);
                }


                if (scope.model.Name == "sfwButton" || scope.model.Name == "sfwImageButton"
                    || scope.model.Name == "sfwLinkButton") {
                    scope.lstButtonMethod = [];

                    if (scope.model.dictAttributes.sfwMethodName == "btnNew_Click") {
                        scope.PoulateRelatedControlForNew();
                    }

                    if (scope.IsLookup) {

                        angular.forEach($rootScope.LstButtonMethodLookup, iAddInlstButtonMethod);
                    }
                    else if (scope.formobject.dictAttributes.sfwType == "FormLinkMaintenance") {

                        angular.forEach($rootScope.LstButtonMethodMaintenance, iAddInlstButtonMethod);
                    }
                    else if (scope.formobject.dictAttributes.sfwType == "FormLinkWizard") {

                        angular.forEach($rootScope.LstButtonMethodWizard, iAddInlstButtonMethod);
                    }
                    //on Button type change , image button method is added
                    if (scope.model.Name == "sfwImageButton") {
                        var item = { Code: "btnRetrieve_Click", Description: "Retrieve" };
                        scope.lstButtonMethod.push(item);
                    }

                    scope.lstEntityFields = [];
                    if (scope.model.dictAttributes.sfwMethodName == "btnOpen_Click") {
                        scope.PopulateEntityFieldsForOpenButton();
                    }
                }

                scope.LoadControlToValidate();
                scope.LoadControlToCompare();
                //lstButtonMethod


                scope.LoadWebControlProperties();
                scope.findCascadingParentControl();
                scope.CurrentTable = FindParent(scope.model, "sfwTable");
                scope.LoadRelatedControlTextbox();

                if (scope.model) {
                    if (scope.model.Name == "sfwImageButton") {
                        scope.RelatedControls = PopulateAssociatedControlsForRetriveButton(scope.model);
                    }
                    else {
                        scope.RelatedControls = PopulateAssociatedControls(scope.model);
                    }

                    if (scope.model.IsChildOfGrid) {
                        var currentGridTable = FindParent(scope.model, "items");
                        FindControlListByName(currentGridTable, "sfwButton", scope.lstRelatedGrid);
                        if (scope.lstRelatedGrid.length > 0) {
                            var lstRelatedButton = [];
                            for (var i = 0; i < scope.lstRelatedGrid.length; i++) {
                                lstRelatedButton.push(scope.lstRelatedGrid[i]);
                            }
                            scope.lstRelatedGrid = [];
                            lstRelatedButton.filter(function (x) {
                                if (x.dictAttributes && x.dictAttributes.ID) {
                                    scope.lstRelatedGrid.push(x.dictAttributes.ID);
                                }
                            });
                        }
                    }
                    else if ((scope.model.dictAttributes.sfwMethodName) && scope.model.dictAttributes.sfwMethodName == "btnResetFilterGrid_Click") {
                        PopulateRelatedGrid(scope.formobject, scope.lstRelatedGrid, true);
                    }
                    else {
                        PopulateRelatedGrid(scope.formobject, scope.lstRelatedGrid, false);
                    }
                }
                scope.InitOnDblClick();
                if (scope.model && scope.model.Name == "sfwWizardStep") {
                    scope.PopulateRelatedButton();
                }

                if (scope.model && (scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwChart")) {
                    scope.PopulateBooleanValues();
                }

                if (scope.model && (scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton" || scope.model.Name == "sfwImageButton")) {
                    scope.MessageIDText = "Message ID:";
                    scope.LoadButtonMethodDescription();
                    scope.PopulateOnClientClick();
                    scope.PopulateSecurityLevel();
                    scope.PopulateSelection();
                    scope.PopulateExecuteAfterSuccessButton();

                    if (scope.model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click") {
                        scope.lstRelatedDialog = PopulateFormLinkRelatedDialogList(scope.MainTable);
                    }

                    var RemoteObjectName = "srvCommon";
                    if (scope.formobject && scope.formobject.dictAttributes.sfwRemoteObject) {
                        RemoteObjectName = scope.formobject.dictAttributes.sfwRemoteObject;
                    }

                    var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                    var lstObjectMethods = GetObjectMethods($EntityIntellisenseFactory.getEntityIntellisense(), scope.formobject.dictAttributes.sfwEntity);
                    scope.lstServerMethod = PopulateServerMethod(lstObjectMethods, scope.model, objServerObject);
                    scope.model.ObjectMethodText = "Server Method :";
                    if (scope.model.dictAttributes.sfwMethodName == "btnCopyRecord_Click" ||
                        scope.model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnValidateExecuteBusinessMethod_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethod_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnBack_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardCancel_Click" || scope.model.dictAttributes.sfwMethodName == "btnCompleteWorkflowActivities_Click") {
                        scope.model.ObjectMethodText = "Object Method :";
                    }
                    scope.PopulateBooleanValues();
                    if (scope.model.dictAttributes.sfwMethodName == "btnPdfCorrespondence_Click") {
                        scope.InitPDFCorrespondenceNavigationParam();
                    }
                    if (scope.model.dictAttributes.sfwMethodName == "btnExportToPDF_Click") {
                        scope.lstPageType = CONST.FORM.LIST_OF_PAGETYPE;
                    }
                    scope.PopulateWizardSteps();
                }
                if (scope.model && scope.model.Name == "sfwImageButton") {
                    scope.PopulateTextRelatedControl();
                    scope.MessageIDText = "Lookup Messages :";
                    scope.OnActiveFormValueChange();
                    scope.PopulateImageAlign();
                }

                if (scope.model.Name == "TemplateField") {
                    scope.lstSortExpressions = [];
                    scope.lstSortExpressions.push("");
                    var objGridView = FindParent(scope.model, "sfwGridView");
                    if (objGridView) {
                        PopulateSortExpressions(objGridView, scope.lstSortExpressions);
                    }
                    scope.HeaderTemplateLabel = undefined;
                    for (var i = 0; i < scope.model.Elements.length; i++) {
                        if (scope.model.Elements[i].Name == "HeaderTemplate") {
                            scope.HeaderTemplateLabel = scope.model.Elements[i].Elements[0];
                        }
                    }
                    if (scope.HeaderTemplateLabel == undefined) {
                        var HeaderTemplate = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: {
                                    sfwEntityField: ""
                                }, Elements: [], Children: [], Name: "sfwLabel", Value: "", prefix: "swc"
                            }], Children: [], Name: "HeaderTemplate", Value: "", prefix: "asp"
                        };
                        scope.HeaderTemplateLabel = HeaderTemplate.Elements[0];
                        //scope.model.Elements.push(HeaderTemplate);
                    }
                }

                scope.addHeaderTemplate = function () {
                    if (scope.model && scope.model.Elements.length > 0) {
                        var obj = scope.model.Elements.filter(function (x) { return x.Name == "HeaderTemplate"; });
                        var index = scope.model.Elements.indexOf(obj[0]);
                        if (index > -1) {
                            scope.model.Elements.splice(index, 1);
                        }
                    }
                    if (scope.HeaderTemplateLabel && scope.HeaderTemplateLabel.dictAttributes.sfwEntityField) {
                        var HeaderTemplate = {
                            dictAttributes: {}, Elements: [scope.HeaderTemplateLabel], Name: "HeaderTemplate", Value: "", prefix: "asp"
                        };

                        scope.model.Elements.push(HeaderTemplate);

                    }

                };

                scope.model.SelectActiveFormType = 'Single';
                scope.IsMultiActiveForm = false;
                if (scope.model.dictAttributes.sfwActiveForm) {
                    if (scope.model.dictAttributes.sfwActiveForm.contains("=")) {
                        scope.model.SelectActiveFormType = 'Multiple';
                        scope.IsMultiActiveForm = true;
                    }
                }

                scope.model.strMethodName = '';
                if (scope.model.dictAttributes.sfwMethodName) {
                    scope.model.strMethodName = scope.model.dictAttributes.sfwMethodName;
                }

                //#region ActiveFormtype
                scope.ActiveFormType = "Lookup,Maintenance,Wizard,FormLinkLookup,FormLinkMaintenance,FormLinkWizard";

                if (scope.model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") {
                    scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                }
                else if (scope.model.dictAttributes.sfwMethodName == "btnOpenLookup_Click") {
                    scope.ActiveFormType = "Lookup,FormLinkLookup,Maintenance,FormLinkMaintenance";
                }
                else if ((scope.model.dictAttributes.sfwMethodName == "btnNew_Click") ||
                    (scope.model.dictAttributes.sfwMethodName == "btnOpen_Click")) {
                    scope.ActiveFormType = "Wizard,Maintenance,FormLinkMaintenance,FormLinkWizard";
                }

                if ((scope.model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") ||
                    (scope.model.dictAttributes.sfwMethodName == "btnPrototypeSearch_Click")) {
                    scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                }

                if (scope.model.dictAttributes.sfwMethodName === "btnRetrieve_Click") {
                    scope.ActiveFormType = "Lookup,FormLinkLookup";
                }

                if (scope.model.Name == "sfwOpenDetail" ||
                    scope.model.Name == "sfwMultiCorrespondence") {
                    scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                }
                if (scope.model.Name == "sfwSeries") {
                    scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                }

                if (scope.model.Name == "sfwOpenDetail" || scope.model.Name == "sfwMultiCorrespondence"
                    || scope.model.dictAttributes.sfwMethodName == "btnWizardAddNewChild_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardFinish_Click" || scope.model.dictAttributes.sfwMethodName == "btnRetrieve_Click"
                    || scope.model.dictAttributes.sfwMethodName == "btnWizardCancel_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardDeleteChild_Click"
                    || scope.model.dictAttributes.sfwMethodName == "btnWizardUpdateChild_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click"
                    || scope.model.dictAttributes.sfwMethodName == "btnWizardNext_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardPrevious_Click"
                    || scope.model.dictAttributes.sfwMethodName == "btnWizardSaveAndNext_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardSaveAndPrevious_Click"
                ) {
                    scope.IsMultiActiveForm = false;
                }

                if (scope.model.Name == "sfwGridView") {
                    scope.lstParentGrid = [];
                    scope.lstParentGrid.push("");
                    PopulateGridID(scope.formobject, scope.model.dictAttributes.ID, scope.lstParentGrid);
                }

                //#region Single, Multiple Type Active Form Selection
                scope.IsActiveFormRadioDisabled = function () {
                    if (scope.model.Name == "sfwOpenDetail" || scope.model.Name == "sfwMultiCorrespondence"  //Removed some condition according to Bug No. 4289,4291,4299
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardFinish_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardCancel_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardNext_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardPrevious_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnWizardSaveAndNext_Click" || scope.model.dictAttributes.sfwMethodName == "btnWizardSaveAndPrevious_Click"
                    ) {
                        return true;
                    }
                    return false;
                };
                //#endregion

                //#endregion
                scope.ShowPropertyBasedOnCondition();

                if (scope.model.Name == "sfwChart") {
                    var ChartAreas;
                    var Series;
                    for (var i = 0; i < scope.model.Elements.length; i++) {
                        if (scope.model.Elements[i].Name == "ChartAreas") {
                            ChartAreas = scope.model.Elements[i];
                            scope.chartArea = scope.model.Elements[i].Elements[0];
                            if (scope.chartArea && scope.chartArea.Elements.length > 0) {
                                scope.Area3D = scope.chartArea.Elements[0];
                            }
                        }
                        else if (scope.model.Elements[i].Name == "Series") {
                            Series = scope.model.Elements[i];
                        }
                    }
                    if (!ChartAreas) {
                        ChartAreas = { Name: 'ChartAreas', Value: '', Elements: [], Children: [], dictAttributes: {} };
                        scope.model.Elements.push(ChartAreas);
                    }
                    if (!scope.chartArea) {
                        scope.chartArea = { Name: 'sfwChartArea', prefix: 'swc', Value: '', Elements: [], Children: [], dictAttributes: {} };
                        ChartAreas.Elements.push(scope.chartArea);
                    }

                    if (!scope.Area3D) {
                        scope.Area3D = { Name: 'Area3DStyle', Value: '', Elements: [], Children: [], dictAttributes: {} };
                        scope.chartArea.Elements.push(scope.Area3D);
                    }

                    if (!Series) {
                        Series = { Name: 'Series', Value: '', Elements: [], Children: [], dictAttributes: {} };
                        scope.model.Elements.push(Series);
                    }
                }

                if (scope.model && scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwCalender" || scope.model.Name == "sfwScheduler") {
                    scope.LoadCollectionFieldEntity();
                }

                if (scope.model) {
                    scope.model.retrievalType = 'Query';
                    if ("sfwRetrievalMethod" in scope.model.dictAttributes) {
                        if (scope.model.dictAttributes.sfwRetrievalMethod != undefined && scope.model.dictAttributes.sfwRetrievalMethod != "") {
                            scope.model.retrievalType = 'Method';
                        }
                    }
                }
                if (scope.model) {
                    scope.model.autocompleteType = 'Query';
                    if (scope.model.dictAttributes.sfwAutoMethod) {
                        scope.model.autocompleteType = 'Method';
                    }
                }

                scope.PopulateKnobStyle = function () {
                    scope.lstKnobStyle = ["butt", "round"];
                    if (!scope.model.dictAttributes.sfwKnobLineStyle) {
                        scope.model.dictAttributes.sfwKnobLineStyle = "butt";
                    }
                };
                scope.PopulateKnobRotation = function () {
                    scope.lstKnobRotation = ["clockwise", "anticlockwise"];
                    if (!scope.model.dictAttributes.sfwKnobRotation) {
                        scope.model.dictAttributes.sfwKnobRotation = "clockwise";
                    }
                };
                scope.PopulateKnobThickness = function () {
                    scope.lstKnobThickness = ["0.1", "0.2", "0.3", "0.4", "0.5"];
                    if (!scope.model.dictAttributes.sfwThickness) {
                        scope.model.dictAttributes.sfwThickness = "0.1";
                    }
                };

                if (scope.model && scope.model.Name == "sfwKnob") {
                    scope.PopulateKnobStyle();
                    scope.PopulateKnobRotation();
                    scope.PopulateKnobThickness();
                }

                if (scope.model.Name == "sfwRuleViewer") {
                    scope.PopulateRulesBasedOnEntity(scope.model.dictAttributes.sfwEntityName);
                }
                if (scope.model && scope.model.Name == "sfwDateTimePicker") scope.LoadDataFormatsForDateTimePicker();

                scope.CheckForFilterGridControl();

                scope.LoadClientVisibilitySource();

                if (scope.model && scope.model.dictAttributes.sfwRetrievalQuery) {
                    scope.RetrievalQueryInputChange(true);
                }
                scope.$evalAsync(function () {
                    scope.setDefaultCodeGroup();
                });
                if (scope.model && scope.model.Name === "sfwLabel" && scope.model.ParentVM) {
                    if (scope.model.ParentVM.Name === "HeaderTemplate" || scope.model.ParentVM.Name === "FooterTemplate") {
                        scope.model.IsHeaderOrFooterField = true;
                    }
                }
            };

            scope.ShowPropertyBasedOnCondition = function () {
                scope.IsShowModePropertyVisible();
                scope.IsShowVisibleRulePropertyVisible();
                scope.IsShowResourcePropertyVisible();
                scope.IsShowCssClassPropertyVisible();
                scope.IsShowCustomAttributePropertyVisible();
            };
            //#endregion

            scope.renderTypes = ["None", "RichText", "Rating", "NumSpinner"];
            scope.PoulateRelatedControlForFinishScheduler = function () {
                scope.lstSchedulerControl = [];
                FindControlListByName(scope.MainTable, 'sfwScheduler', scope.lstSchedulerControl);
                var obj = { dictAttributes: { ID: "" } };
                scope.lstSchedulerControl.splice(0, 0, obj);
            };

            scope.LoadControlToValidate = function () {
                scope.lstControlToValidate = [];
                scope.lstControlToValidate.push("");
                if (scope.model.ParentVM != undefined && scope.model.ParentVM.Elements.length > 0) {
                    for (var i = 0; i < scope.model.ParentVM.Elements.length; i++) {
                        if (scope.model.ParentVM.Elements[i].Name == "sfwDropDownList" || scope.model.ParentVM.Elements[i].Name == "sfwTextBox") {
                            scope.lstControlToValidate.push(scope.model.ParentVM.Elements[i].dictAttributes.ID);
                        }
                    }
                }
            };

            scope.LoadControlToCompare = function () {
                scope.lstControlToCompare = [];
                var obj = {
                    dictAttributes: {
                        ID: ""
                    }
                };
                scope.lstControlToCompare.push(obj);
                if (scope.MainTable) {
                    FindControlListByName(scope.MainTable, "sfwDropDownList", scope.lstControlToCompare);
                    FindControlListByName(scope.MainTable, "sfwTextBox", scope.lstControlToCompare);
                }
            };

            scope.LoadType = function () {
                scope.lstType = [];
                scope.lstType.push("String");
                scope.lstType.push("Integer");
                scope.lstType.push("Double");
                scope.lstType.push("Date");
                scope.lstType.push("Currency");
            };
            scope.LoadDataType = function () {
                scope.lstDataType = [];
                scope.lstDataType.push("String");
                scope.lstDataType.push("Numeric");
                scope.lstDataType.push("Decimal");
                scope.lstDataType.push("DateTime");
                scope.lstDataType.push("Long");
            };
            scope.LoadModes = function () {
                scope.lstModes = [];
                scope.lstModes.push("All");
                scope.lstModes.push("New");
                scope.lstModes.push("Update");
            };
            scope.LoadDataFormats = function () {
                scope.lstDataFormats = [];
                scope.lstDataFormats.push("");
                scope.lstDataFormats.push("{0:d}");
                scope.lstDataFormats.push("{0:C}");
                scope.lstDataFormats.push("{0:G}");
                scope.lstDataFormats.push("{0:#0.00'%}");
                scope.lstDataFormats.push("{0:#0.000'%}");
                scope.lstDataFormats.push("{0:#0.0000'%}");
                scope.lstDataFormats.push("{0:#0.00000'%}");
                scope.lstDataFormats.push("{0:000-##-####}");
                scope.lstDataFormats.push("{0:(###)###-####}");
            };
            scope.LoadDataFormatsForDateTimePicker = function () {
                scope.lstDataFormatsForDateTime = [];
                scope.lstDataFormatsForDateTime.push("");
                scope.lstDataFormatsForDateTime.push("{0:d}");
                scope.lstDataFormatsForDateTime.push("{0:G}");

            };
            scope.LoadTextMode = function () {
                scope.lstTextModes = [];
                scope.lstTextModes.push("");
                scope.lstTextModes.push("SingleLine");
                scope.lstTextModes.push("MultiLine");
                scope.lstTextModes.push("Password");
            };
            scope.LoadDefaultTypes = function () {
                scope.lstDefaultType = [];
                scope.lstDefaultType.push("None");
                scope.lstDefaultType.push("TextValue");
                scope.lstDefaultType.push("TodaysDate");
                scope.lstDefaultType.push("TodaysDateTime");
                scope.lstDefaultType.push("MethodOnServer");
                scope.lstDefaultType.push("MethodOnClient");
                scope.lstDefaultType.push("SystemConstant");
                scope.lstDefaultType.push("ScalarQuery");
            };
            scope.LoadOperators = function () {
                scope.lstOperators = [];
                scope.lstOperators.push("=");
                scope.lstOperators.push("!=");
                scope.lstOperators.push("<");
                scope.lstOperators.push("<=");
                scope.lstOperators.push(">");
                scope.lstOperators.push(">=");
                scope.lstOperators.push("in");
                scope.lstOperators.push("not in");
                scope.lstOperators.push("like");
                scope.lstOperators.push("exists");
                scope.lstOperators.push("between");
                scope.lstOperators.push("is null");
                scope.lstOperators.push("is not null");
            };
            scope.LoadValidatorOperators = function () {
                scope.lstValidatorOperators = [];
                scope.lstValidatorOperators.push("Equal");
                scope.lstValidatorOperators.push("NotEqual");
                scope.lstValidatorOperators.push("GreaterThan");
                scope.lstValidatorOperators.push("GreaterThanEqual");
                scope.lstValidatorOperators.push("LessThan");
                scope.lstValidatorOperators.push("LessThanEqual");
                scope.lstValidatorOperators.push("DataTypeCheck");
            };
            scope.LoadSelectionMode = function () {
                scope.lstSelectionMode = [];
                scope.lstSelectionMode.push("");
                scope.lstSelectionMode.push("Single");
                scope.lstSelectionMode.push("Multiple");
            };
            scope.LoadRepeatDirection = function () {
                scope.lstRepeatDirection = [];
                scope.lstRepeatDirection.push("Horizontal");
                scope.lstRepeatDirection.push("Vertical");
            };
            scope.LoadAlignment = function () {
                scope.lstAlignment = [];
                scope.lstAlignment.push("Left");
                scope.lstAlignment.push("Center");
                scope.lstAlignment.push("Right");
                scope.lstAlignment.push("Justify");
                scope.lstAlignment.push("NotSet");
            };
            scope.LoadGridSelection = function () {
                scope.lstGridSelection = [];
                scope.lstGridSelection.push("None");
                scope.lstGridSelection.push("One");
                scope.lstGridSelection.push("Many");
            };
            scope.LoadAxisFormatsForChart = function () {
                scope.lstAxisFormat = [];
                scope.lstAxisFormat.push("");
                scope.lstAxisFormat.push("{0:C}");
                scope.lstAxisFormat.push("{0:c}");
                scope.lstAxisFormat.push("{0:d}");
                scope.lstAxisFormat.push("{0:%}");
                scope.lstAxisFormat.push("{0:#0.00'%}");
                scope.lstAxisFormat.push("{0:#0.000'%}");
                scope.lstAxisFormat.push("{0:#0.0000'%}");
                scope.lstAxisFormat.push("{0:#0.00000'%}");
            };
            scope.setClientVisibilityClick = function () {

                //scope.template = 'Form/views/ClientVisibility.html';
                //scope.strAttributeName = "sfwClientVisibility";
                //scope.Title = "Set Client Visibility";
                //scope.IsForm = true;

                var newScope = scope.$new();
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.strAttributeName = "sfwClientVisibility";
                newScope.Title = "Set Client Visibility";
                newScope.IsForm = false;
                newScope.objNewDialog = $rootScope.showDialog(newScope, "Set Client Visibility", "Form/views/ClientVisibility.html", { width: 900, height: 650 });
            };

            scope.setClientEnableClick = function () {

                var newScope = scope.$new();
                newScope.strAttributeName = "sfwClientEnable";
                newScope.Title = "Set Client Enability";
                newScope.IsForm = false;

                newScope.objNewDialog = $rootScope.showDialog(newScope, "Set Client Enability", "Form/views/ClientVisibility.html", { width: 900, height: 650 });
            };

            scope.onCustomAttributeClick = function () {

                var newScope = scope.$new();
                newScope.propertyName = "sfwCustomAttributes";
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = true;
                newScope.UserLogParaDialog = $rootScope.showDialog(newScope, "Set Custom Attributes", "Form/views/CustomAttributes.html", { width: 900, height: 650 });
            };
            scope.onSetParameterClick = function () {
                if (scope.model.CommonProp == 'CodeGroup') {
                    scope.onCodeGroupParameterClick();
                }
                else {
                    scope.onQueryParameterClick();
                }
            };
            scope.onCodeGroupParameterClick = function () {
                var newScope = scope.$new(true);
                newScope.SelectedObject = scope.model;
                newScope.formobject = scope.formobject;
                newScope.IsFormCodeGroup = true;
                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Set Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
            };
            scope.onQueryParameterClick = function () {
                var newScope = scope.$new(true);
                newScope.SelectedObject = scope.model;
                newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = true;

                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
            };

            scope.openImageConditionClick = function (item) {
                var newScope = scope.$new(true);
                newScope.oldConditions = item;
                newScope.formobject = scope.formobject;
                newScope.model = scope.model;
                newScope.ImageConditionDialog = $rootScope.showDialog(newScope, "Image Condition", "Form/views/ImageCondition.html", { width: 600, height: 505 });
            };

            scope.openBindtoQueryClick = function (item) {
                var newScope = scope.$new(true);
                newScope.gridmodel = item;
                newScope.formobject = scope.formobject;
                newScope.model = scope.model;
                newScope.BindToQueryDialog = $rootScope.showDialog(newScope, "Bind to Query", "Form/views/BindToQuery.html", { width: 600, height: 500 });
            };

            scope.openCellFormatClick = function (model, IsCellOrRow) {
                //scope.template = 'Form/views/CellFormate.html';
                //scope.TemplateFieldModel = model;
                //scope.formEntity = scope.formobject.dictAttributes.sfwEntity;
                //scope.isLookup = scope.IsLookup;
                //scope.IsCellOrRow = IsCellOrRow;
                var dialogScope = scope.$new(true);

                dialogScope.TemplateFieldModel = model;
                dialogScope.formEntity = scope.formobject.dictAttributes.sfwEntity;
                dialogScope.isLookup = scope.IsLookup;
                dialogScope.IsCellOrRow = IsCellOrRow;

                dialogScope.CellFormatDialog = $rootScope.showDialog(dialogScope, " Cell Format", "Form/views/CellFormate.html", { width: 600, height: 565 });
            };
            scope.openSetFooterClick = function (model) {
                var dialogScope = scope.$new(true);
                // scope.TemplateFieldModel = model;
                // scope.formEntity = scope.formobject.dictAttributes.sfwEntity;
                //scope.template = 'Form/views/SetFooter.html';
                var objGrid = FindParent(model, "sfwGridView");

                if (model && objGrid && objGrid.dictAttributes.sfwParentGrid) {
                    var objParentGrid = FindControlByID(scope.formobject, objGrid.dictAttributes.sfwParentGrid);
                    if (objParentGrid && objParentGrid.dictAttributes.sfwEntityField) {
                        var entityName = null;
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objParentGrid.dictAttributes.sfwEntityField);
                        if (object) {
                            dialogScope.formEntity = object.Entity;
                        }
                    }
                } else {
                    dialogScope.formEntity = scope.formobject.dictAttributes.sfwEntity;
                }
                dialogScope.TemplateFieldModel = model;
                dialogScope.SetFoterDialog = $rootScope.showDialog(dialogScope, "Footer Details", "Form/views/SetFooter.html", { width: 600 });
            };
            scope.openDataKeyAndSortExxpressionClick = function (model) {
                var dialogScope = scope.$new(true);
                dialogScope.model = model;

                dialogScope.DataKeyAndSortExpressionDialog = $rootScope.showDialog(dialogScope, "DataKey And Sort Expression", "Form/views/DataKeysAndSortExpression.html", { width: 800 });
            };

            scope.openGroupExpression = function () {
                var dialogScope = scope.$new(true);
                dialogScope.model = scope.model;

                dialogScope.GroupExpressionDialog = $rootScope.showDialog(dialogScope, "Group Expression", "Form/views/ControlProperties/GroupExpression.html", { width: 800 });

            };

            scope.onValidationRulesClick = function (entityID) {
                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.entityID = entityID;
                newScope.dialogValidation = $rootScope.showDialog(newScope, "Hard Errors", "Form/views/SetValidationRules.html", { width: 700, height: 460 });
            };

            scope.OpenImageUrl = function (openFrom) {
                if (scope.formobject.dictAttributes.WebSite) {
                    $.connection.hubForm.server.openImageUrlClick(scope.formobject.dictAttributes.WebSite, openFrom).done(function (data) {
                        if (data && data.length == 2) {
                            scope.receiveImageFileName(data[0], data[1]);
                        }
                    });
                }
                else {
                    alert("Please select the appropriate 'Website' from the form details page.");
                }
            };
            scope.receiveImageFileName = function (fileName, errorMessage) {
                if (fileName != "") {
                    scope.$apply(function () {
                        scope.model.dictAttributes.ImageUrl = fileName;
                    });
                }
                if (errorMessage != "" && errorMessage != undefined) {
                    alert(errorMessage);
                }

            };

            scope.openRetrievalTemplateProp = function (model) {
                scope.template = 'Form/views/Retrieval.html';
                scope.model = model;
                scope.formobject = scope.formobject;

            };
            scope.openAutoCompleteTemplateProp = function (model) {
                scope.template = 'Form/views/AutoComplete.html';
                scope.model = model;
                scope.formobject = scope.formobject;

            };
            scope.getQuery_dialog = function () {
                var dialogScope = scope.$new(true);
                if (scope.model.dictAttributes.sfwQueryID != undefined) {
                    dialogScope.strSelectedQuery = scope.model.dictAttributes.sfwQueryID;
                }
                dialogScope.IsForm = true;

                dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });
            };
            scope.$on('onFormQueryClick', function (event, data) {
                scope.model.dictAttributes.sfwQueryID = data;
            });
            scope.onAddItem = function () {
                var obj = { dictAttributes: { Text: "", Value: "" }, Elements: [], Children: [], Name: "ListItem", Value: "", prefix: 'asp' };
                scope.model.Elements.push(obj);
            };
            scope.SelectItem = function (item) {
                scope.selectedItem = item;
            };
            scope.onDeleteItem = function () {
                if (scope.selectedItem != undefined) {
                    var index = scope.model.Elements.indexOf(scope.selectedItem);
                    if (index > -1) {
                        scope.model.Elements.splice(index, 1);
                    }
                    else {
                        scope.selectedItem == undefined;
                    }

                }
                //else if(scope.model.Elements.length>0){
                //    scope.model.Elements.splice(scope.model.Elements.length-1,1);
                //}
            };
            scope.commonTemplateSelectionChange = function (para) {
                scope.model.Elements = [];
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);

                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                if (scope.model.Name == "sfwCascadingDropDownList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.DataTextField, scope.model.dictAttributes, "DataTextField", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.DataValueField, scope.model.dictAttributes, "DataValueField", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwLoadSource, scope.model.dictAttributes, "sfwLoadSource", "");
                if (scope.model && scope.model.Name == "sfwCheckBoxList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwDefaultValue, scope.model.dictAttributes, "sfwDefaultValue", "");
                }
                if (scope.model && (scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCascadingDropDownList")) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwImageField, scope.model.dictAttributes, "sfwImageField", "");
                }
                $rootScope.UndRedoBulkOp("End");
                scope.InitializeDropDownItems();

            };

            scope.onEntityFieldTextChange = function () {
                scope.model.CollectionFieldEntity = "";
                if (scope.model.Name == "sfwCheckBoxList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCheckBoxField, scope.model.dictAttributes, "sfwCheckBoxField", "");
                }
                else if (scope.model.Name == "sfwCalendar" || scope.model.Name == "sfwScheduler") {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEventId, scope.model.dictAttributes, "sfwEventId", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEventName, scope.model.dictAttributes, "sfwEventName", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEventStartDate, scope.model.dictAttributes, "sfwEventStartDate", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEventEndDate, scope.model.dictAttributes, "sfwEventEndDate", "");
                    if (scope.model.Name == "sfwScheduler") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEventCategory, scope.model.dictAttributes, "sfwEventCategory", "");
                    }
                    $rootScope.UndRedoBulkOp("Start");
                }
            };

            scope.LoadCheckBoxFieldEntity = function () {
                if (scope.model.dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
                    var strEntityName = scope.formobject.dictAttributes.sfwEntity;
                    if (scope.entityName) {
                        strEntityName = scope.entityName;
                    }
                    objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strEntityName, scope.model.dictAttributes.sfwEntityField);
                    if (objAttribute && objAttribute.Entity) {
                        scope.$evalAsync(function () {
                            scope.model.CheckBoxFieldEntity = objAttribute.Entity;
                        });
                    }
                }
            };

            scope.LoadCollectionFieldEntity = function () {
                if (scope.model.dictAttributes.sfwEntityField != undefined && scope.model.dictAttributes.sfwEntityField != "") {
                    var strEntityName = scope.formobject.dictAttributes.sfwEntity;
                    if (scope.entityName) {
                        strEntityName = scope.entityName;
                    }
                    var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strEntityName, scope.model.dictAttributes.sfwEntityField);
                    if (objAttribute) {
                        scope.$evalAsync(function () {
                            scope.model.CollectionFieldEntity = objAttribute.Entity;
                        });
                    }
                }
            };

            scope.getEntityObject = function (entityName) {
                var objEntity;
                var lstentitylist = $EntityIntellisenseFactory.getEntityIntellisense();
                function filter(itm) {
                    return itm.ID == entityName;
                }
                var lstEntity = lstentitylist.filter(filter);
                if (lstEntity && lstEntity.length > 0) {
                    objEntity = lstEntity[0];
                }
                return objEntity;
            };

            scope.findCascadingParentControl = function () {
                var lstDropdownLists = [];
                if (scope.model.Name == "sfwCascadingDropDownList") {

                    var ojParentGrid = FindParent(scope.model, "sfwGridView");
                    if (ojParentGrid && ojParentGrid.Elements.length > 0) {
                        FindControlListByName(ojParentGrid, scope.model.Name, lstDropdownLists);
                    }
                    else {
                        FindControlListByName(scope.MainTable, scope.model.Name, lstDropdownLists);
                    }
                    if (lstDropdownLists) {
                        lstDropdownLists.filter(function (x) {
                            if (x.dictAttributes.ID != scope.model.dictAttributes.ID) {
                                scope.lstCascadingDropDwonParent.push(x.dictAttributes.ID);
                            }
                        });
                    }
                }
            };

            scope.LoadRelatedControlTextbox = function () {
                scope.lstRelatedControlTextbox = [];
                var obj = {
                    dictAttributes: {
                        ID: ""
                    }
                };
                scope.lstRelatedControlTextbox.push(obj);
                if (scope.MainTable) {
                    FindControlListByNameForTextBox(scope.MainTable, "sfwTextBox", scope.lstRelatedControlTextbox, scope.formobject);
                }
            };
            scope.showQueryIDIntellisenseList = function (event) {
                var inputElement;
                inputElement = $(event.target).prevAll("input[type='text']");
                inputElement.focus();
                if (inputElement) {
                    createQueryIDList(inputElement);
                    if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                }
                if (event) {
                    event.stopPropagation();
                }
            };
            scope.onQueryIDChange = function (event) {
                if (event.key != "Down" && event.key != "Up" && event.key != "Left" && event.key != "Right") {
                    var input = $(event.target);
                    if (event && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                        $(input).autocomplete("search", $(input).val());
                        event.preventDefault();
                    }
                    else {
                        createQueryIDList(input);
                    }
                }
            };
            var createQueryIDList = function (input) {
                var formScope = getCurrentFileScope();
                scope.queryIdList = [];
                if (formScope && formScope.lstQueryID) {
                    var lst = [];
                    function iterator(itm) {
                        if (itm.Name == "query" && itm.dictAttributes.ID) {
                            lst.push(itm.dictAttributes.ID);
                        }
                    }
                    angular.forEach(formScope.lstQueryID, iterator);
                    scope.queryIdList = lst;
                    setSingleLevelAutoComplete(input, lst, scope);
                }

            };

            scope.PopulateRulesBasedOnEntity = function (entityName) {
                scope.Rules = [];
                function filter(itm) {
                    return itm.ID == entityName;
                }
                var lstEntity = $EntityIntellisenseFactory.getEntityIntellisense().filter(filter);
                if (lstEntity && lstEntity.length > 0) {
                    if (lstEntity[0].Rules.length > 0) {
                        scope.Rules = lstEntity[0].Rules;
                    }
                }
            };

            scope.CheckForFilterGridControl = function () {
                var lst = [];
                //scope.model.IsShowDataField = false;
                var objFilterGridButton;
                scope.lstBaseQueryColumns = [];
                if (scope.model && scope.MainTable) {
                    FindControlListByNames(scope.MainTable, ['sfwButton', 'sfwLinkButton', 'sfwImageButton'], lst);
                    if (lst && lst.length > 0) {
                        lst = lst.filter(function (itm) { return itm.dictAttributes.sfwMethodName == "btnGridSearch_Click"; });
                        if (lst && lst.length > 0) {
                            for (var i = 0; i < lst.length; i++) {
                                if (lst[i].dictAttributes.sfwNavigationParameter) {
                                    var lstParam = lst[i].dictAttributes.sfwNavigationParameter.split(';');
                                    if (lstParam && lstParam.length > 0) {
                                        if (lstParam.some(function (param) { return param == scope.model.dictAttributes.ID; })) {
                                            scope.model.IsShowDataField = true;
                                            objFilterGridButton = lst[i];
                                            break;
                                        }
                                    }
                                }
                            }
                            if (objFilterGridButton) {
                                if (objFilterGridButton.dictAttributes.sfwRelatedControl) {
                                    var objGrid = FindControlByID(scope.MainTable, objFilterGridButton.dictAttributes.sfwRelatedControl);
                                    if (objGrid && objGrid.dictAttributes.sfwBaseQuery) {
                                        $.connection.hubForm.server.getEntityQueryColumns(objGrid.dictAttributes.sfwBaseQuery, "ScopeId_" + scope.$id).done(function (data) {
                                            if (data) {
                                                for (var j = 0; j < data.length; j++) {
                                                    scope.lstBaseQueryColumns.push(data[j].CodeID);
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            };
            //#endregion

            //#region Init Common Template Methods
            scope.InitOnDblClick = function () {
                if (scope.model.Name == "sfwListPicker" || scope.model.Name == "sfwSourceList" || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwListBox") {
                    scope.PopulateLoadTyeps();
                    scope.InitializeDropDownItems();
                }
            };

            scope.PopulateLoadTyeps = function () {
                scope.lstLoadTypes = [];
                scope.lstLoadTypes.push("CodeGroup");
                scope.lstLoadTypes.push("Query");
                if (!scope.IsLookup || (scope.model && (scope.model.Name != "sfwDropDownList" && scope.model.Name != "sfwMultiSelectDropDownList"))) {
                    scope.lstLoadTypes.push("Method");
                    var objListView = FindParent(scope.model, "sfwListView");
                    if (scope.model && (scope.model.IsChildOfGrid || objListView)) {
                        scope.lstLoadTypes.push("ChildMethod");
                    }
                }
                scope.lstLoadTypes.push("ServerMethod");
                scope.lstLoadTypes.push("Items");
            };

            scope.InitializeDropDownItems = function () {
                if (scope.model) {
                    scope.model.CommonProp = scope.model.dictAttributes.sfwLoadType;
                    scope.lstCodeValues = [];
                    scope.lstQryField = [];
                    scope.GetCodeValueFields();

                    if (scope.model.CommonProp == undefined || scope.model.CommonProp == "") {
                        scope.model.CommonProp = "CodeGroup";
                        scope.model.dictAttributes.sfwLoadType = "CodeGroup";
                    } else if (scope.model.CommonProp == "Query") {
                        scope.LoadQueryColumns(scope.model.dictAttributes.sfwLoadSource);
                    }
                    else if (scope.model.CommonProp == "ServerMethod") {
                        scope.onRemoteObjectChanged(true);
                    }
                }
            };

            scope.GetCodeValueFields = function () {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lst = entityIntellisenseList.filter(function (x) {
                    return x.ID == "entCodeValue";
                });
                function iterator(objColumn) {
                    var strColumn = objColumn.ID;
                    var strKeyNo = objColumn.KeyNo;
                    var strValue = objColumn.Value;
                    if (!strValue) {
                        strValue = "";
                    }
                    if (strKeyNo == "0" && !IsAuditField(strColumn)) {
                        scope.lstCodeValues.push({ CodeID: strValue, Description: strColumn });
                    }
                }

                if (lst && lst.length > 0) {
                    var objEntity = lst[0];
                    angular.forEach(objEntity.Attributes, iterator);
                }
            };

            //TODO:Test Query search
            scope.onQuerySearchClick = function (param) {

                scope.attributeName = "sfwLoadSource";
                if (param == "DefaultValue") {
                    scope.attributeName = "sfwDefaultValue";
                }

                scope.strSelectedQuery = scope.model.dictAttributes[scope.attributeName];

                var dialogScope = scope.$new(true);
                //dialogScope = scope;
                dialogScope.attributeName = "sfwLoadSource";
                if (param == "DefaultValue") {
                    dialogScope.attributeName = "sfwDefaultValue";
                }
                dialogScope.strSelectedQuery = scope.model.dictAttributes[scope.attributeName];
                dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });
            };

            //TODO: Test 
            scope.$on('onQueryClick', function (event, data) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes[scope.attributeName], scope.model.dictAttributes, scope.attributeName, data);

                scope.LoadQueryColumns(scope.model.dictAttributes[scope.attributeName]);
            });
            scope.LoadQueryColumnsOnChange = function (queryId) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                scope.LoadQueryColumns(queryId);
            };
            scope.LoadQueryColumns = function (queryId) {

                if (queryId) {
                    var lst = queryId.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) {
                            return x.ID == entityName;
                        });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            var lstQuery = objEntity.Queries.filter(function (x) {
                                return x.ID == strQueryID;
                            });
                            if (lstQuery && lstQuery.length > 0) {
                                var objQuery = lstQuery[0];
                                scope.selectedCurrentQuery = objQuery;
                                $.connection.hubForm.server.getEntityQueryColumns(queryId, "MainParentPanelCommonProp" + scope.formobject.dictAttributes.ID).done(function (data) {
                                    scope.$evalAsync(function () {
                                        scope.lstQryField = [];
                                        var obj = { CodeID: '', Description: '' };
                                        //data.splice(0, 0, obj);
                                        scope.lstQryField = data;
                                    });
                                });
                            }
                            else {
                                scope.lstQryField = [];
                            }
                        }
                    }
                }
            };


            //TODO: Test GridView Common Property-> Base Query
            scope.onBaseQuerySearchClick = function () {
                var dialogScope = scope.$new(true);
                dialogScope.strSelectedQuery = scope.model.dictAttributes.sfwBaseQuery;
                dialogScope.IsBaseQuery = true;

                //dialogScope.QueryDialog = ngDialog.open({
                //    template: "Form/views/BrowseForQuery.html",
                //    scope: dialogScope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });
            };
            scope.$on('onBaseQueryClick', function (event, data) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwBaseQuery, scope.model.dictAttributes, "sfwBaseQuery", data);
                scope.onBaseQueryChange(data);
            });

            scope.onBaseQueryChange = function (QueryID, isIgnoreForUndoRedo) {
                scope.model.dictAttributes.sfwBaseQuery = QueryID;
                var DummyParams = [];
                function addParam(itm) {
                    var param = { Name: "parameter", Value: '', dictAttributes: { ID: itm.ID }, Elements: [], Children: [] };
                    var lstPara = DummyParams.filter(function (x) { return x.dictAttributes.ID == param.dictAttributes.ID });
                    if (lstPara && lstPara.length > 0) {
                        param.dictAttributes.sfwEntityField = lstPara[0].dictAttributes.sfwEntityField;
                    }
                    if (isIgnoreForUndoRedo) {
                        objParameters.Elements.push(param);
                    }
                    else {
                        $rootScope.PushItem(param, objParameters.Elements);
                    }
                }

                if (scope.model.dictAttributes.sfwBaseQuery && scope.model.dictAttributes.sfwBaseQuery != "") {
                    var lst = scope.model.dictAttributes.sfwBaseQuery.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        if (!isIgnoreForUndoRedo) {
                            $rootScope.UndRedoBulkOp("Start");
                        }
                        var lstParams = scope.model.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                        var objParameters;
                        if (!lstParams || (lstParams && lstParams.length == 0)) {
                            objParameters = { Name: "Parameters", Value: '', dictAttributes: {}, Elements: [], Children: [] };
                            if (isIgnoreForUndoRedo) {
                                scope.model.Elements.push(objParameters);
                            }
                            else {
                                $rootScope.PushItem(objParameters, scope.model.Elements);
                            }
                        }
                        else {
                            objParameters = lstParams[0];
                        }
                        if (objParameters) {
                            if (objParameters.Elements.length > 0) {
                                for (var i = 0; i < objParameters.Elements.length; i++) {
                                    DummyParams.push(objParameters.Elements[i]);
                                }
                            }
                            objParameters.Elements = [];
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                            if (lstEntity && lstEntity.length > 0) {
                                var objEntity = lstEntity[0];
                                var lstQuery = objEntity.Queries.filter(function (x) { return x.ID == strQueryID; });
                                if (lstQuery && lstQuery.length > 0) {
                                    var objQuery = lstQuery[0];
                                    if (objQuery.Parameters.length > 0) {
                                        angular.forEach(objQuery.Parameters, addParam);
                                    }
                                }
                            }
                        }
                        if (!isIgnoreForUndoRedo) {
                            $rootScope.UndRedoBulkOp("Start");
                        }
                    }
                }
                else {
                    var lstParams = scope.model.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                    var objParameters;
                    if (lstParams && lstParams.length > 0) {
                        objParameters = lstParams[0];
                        if (objParameters) {
                            objParameters.Elements = [];
                        }
                    }
                }
            };

            scope.openBaseQueryParamClick = function () {
                scope.onBaseQueryChange(scope.model.dictAttributes.sfwBaseQuery, true);
                var newScope = scope.$new();
                newScope.Elements = [];
                var lstParams = scope.model.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                if (lstParams && lstParams.length > 0) {
                    if (lstParams[0] && lstParams[0].Elements && lstParams[0].Elements.length > 0) {
                        for (var i = 0; i < lstParams[0].Elements.length; i++) {
                            var param = {};
                            angular.copy(lstParams[0].Elements[i], param);
                            newScope.Elements.push(param);
                        }
                    }
                }
                newScope.entityTreeBinding = { selectedobject: null, lstselectedobjects: [], lstentities: [] };
                newScope.FormEntity = scope.formobject.dictAttributes.sfwEntity;
                newScope.onOkClick = function () {
                    if (newScope.Elements && newScope.Elements.length > 0) {
                        $rootScope.UndRedoBulkOp("Start");
                        if (lstParams && lstParams.length > 0) {
                            $rootScope.EditPropertyValue(lstParams[0].Elements, lstParams[0], "Elements", []);
                            for (var i = 0; i < newScope.Elements.length; i++) {
                                $rootScope.PushItem(newScope.Elements[i], lstParams[0].Elements);
                            }
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                    newScope.dialog.close();
                };
                newScope.onParameterSelected = function (objParam) {
                    newScope.SelectedParameter = objParam;
                };
                newScope.dialog = $rootScope.showDialog(newScope, "Base Query Parameters", "Form/views/ControlProperties/GridBaseQueryParameter.html", { width: 800, height: 525 });
            };

            scope.onEditablePropertyChange = function () {
                $rootScope.UndRedoBulkOp("Start");
                if (scope.model) {
                    if (scope.model.dictAttributes.AllowEditing == "True") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwTwoWayBinding, scope.model.dictAttributes, "sfwTwoWayBinding", "True");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCommonFilterBox, scope.model.dictAttributes, "sfwCommonFilterBox", "False");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwFilterOnKeyPress, scope.model.dictAttributes, "sfwFilterOnKeyPress", "False");

                    }
                    else {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwTwoWayBinding, scope.model.dictAttributes, "sfwTwoWayBinding", "False");

                    }
                }
                $rootScope.UndRedoBulkOp("End");
            };

            scope.canDisabledParameters = function () {
                var isDisable = false;
                if (scope.model) {
                    if (scope.model.dictAttributes.sfwLoadType == "CodeGroup") {
                        if (!scope.model.placeHolder && !scope.model.dictAttributes.sfwLoadSource) {
                            isDisable = true;
                        }
                    }
                    else if (!scope.model.dictAttributes.sfwLoadSource) {
                        isDisable = true;
                    }
                }
                return isDisable;
            }
            scope.onMethodQueryParamClick = function () {

                var newScope = scope.$new(true);
                newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                newScope.SelectedObject = scope.model;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = true;
                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
            }
            //#endregion

            //#region Show hide properties for control
            scope.IsShowModePropertyVisible = function () {
                if (scope.model && scope.model.Name != "sfwLiteral" && scope.model.Name != "RequiredFieldValidator" && scope.model.Name != "CompareValidator" && scope.model.Name != "sfwSoftErrors" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwColumn" && scope.model.Name != "sfwPanel" && scope.model.Name != "sfwRuleViewer") {
                    if (scope.model.Name == "sfwTable" || (!scope.IsLookup && scope.model && (scope.model.Name == "sfwLabel" || scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwCheckBox" || scope.model.Name == "sfwRadioButton" || scope.model.Name == "sfwTextBox" || scope.model.Name == "sfwTargetList" ||
                        scope.model.Name == "sfwListPicker" || scope.model.Name == "sfwSourceList" || scope.model.Name == "sfwMultiCorrespondence" || scope.model.Name == "sfwOpenDetail" || scope.model.Name == "sfwDateTimePicker") || scope.model.Name == "RangeValidator" || scope.model.Name == "ValidationSummary" || scope.model.Name == "RegularExpressionValidator" || scope.model.Name == "sfwCheckBoxList"
                        || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton" || scope.model.Name == "sfwCommandButton" || scope.model.Name == "sfwSlider" || scope.model.Name == "sfwKnob" || scope.model.Name == "sfwJSONData" || scope.model.Name == "sfwButtonGroup")) {
                        scope.IsShowModeProperty = true;
                    } else {
                        scope.IsShowModeProperty = false;
                    }
                }
                else {
                    scope.IsShowModeProperty = false;
                }
            };
            scope.IsShowVisibleRulePropertyVisible = function () {
                if (scope.IsLookup) {
                    if (scope.model.IsChildOfGrid && scope.objGridView) {
                        scope.IsShowVisibleRuleProperty = true;
                    }
                    else {
                        scope.IsShowVisibleRuleProperty = false;
                    }
                }

                else {
                    if (scope.model && scope.model.Name != "sfwLiteral" && scope.model.Name != "RequiredFieldValidator" && scope.model.Name != "CompareValidator" && scope.model.Name != "sfwSoftErrors" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwWizard" && scope.model.Name != "udc" && scope.model.Name != "sfwRuleViewer" && scope.model.Name != 'TemplateField') {
                        if (scope.model.Name == "sfwTable" || (!scope.IsLookup && scope.model && scope.model.Name != "sfwColumn" && scope.model.Name != "hr" && scope.model.Name != "br" && scope.model.Name != "sfwCRViewer" && scope.model.Name != "sfwFileLayout" && scope.model.Name != "sfwFileUpload" && scope.model.Name != "sfwTabContainer" && scope.model.Name != "sfwWizardProgress") || scope.model.Name == "RangeValidator" || scope.model.Name == "ValidationSummary" || scope.model.Name == "RegularExpressionValidator" || scope.model.Name == "sfwTabSheet" || scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwHyperLink" || scope.model.Name == "sfwTable" || scope.model.Name == "sfwGridView") {
                            if (scope.model.IsChildOfGrid && scope.objGridView) {
                                scope.IsShowVisibleRuleProperty = true;
                            }
                            else {
                                scope.IsShowVisibleRuleProperty = true;
                            }
                        } else {
                            scope.IsShowVisibleRuleProperty = false;
                        }
                    }
                    else {
                        scope.IsShowVisibleRuleProperty = false;
                    }
                }
            };
            scope.IsShowResourcePropertyVisible = function () {
                scope.IsShowResourceProperty = false;
                if (!scope.IsLookup) {
                    if (scope.model && !scope.model.IsChildOfGrid) {
                        if (scope.model && scope.model.Name != "sfwLiteral" && scope.model.Name != "RequiredFieldValidator" && scope.model.Name != "CompareValidator" && scope.model.Name != "sfwSoftErrors" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwWizard" && scope.model.Name != "sfwColumn" && scope.model.Name != "sfwRuleViewer") {
                            if (scope.model.Name == "sfwTable" || (scope.model && scope.model.Name != "hr" && scope.model.Name != "br" && scope.model.Name != "sfwCRViewer" && scope.model.Name != "sfwFileLayout" && scope.model.Name != "sfwFileUpload" && scope.model.Name != "sfwTabContainer" && scope.model.Name != "sfwWizardProgress") || scope.model.Name == "RangeValidator" || scope.model.Name == "ValidationSummary" || scope.model.Name == "RegularExpressionValidator" || scope.model.Name == "sfwTabSheet" || scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwHyperLink" || scope.model.Name == "sfwPanel" || scope.model.Name == "sfwTable" || scope.model.Name == "sfwGridView" || (!scope.IsLookup && scope.model.Name == "sfwDateTimePicker")) {
                                scope.IsShowResourceProperty = true;
                            }
                        }
                    }
                }
            };

            scope.IsShowCssClassPropertyVisible = function () {
                if (scope.model && scope.model.Name != "hr" && scope.model.Name != "br" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwTable" && scope.model.Name != "sfwGridView" && scope.model.Name != "udc" && scope.model.Name != "sfwRuleViewer" && scope.model.Name != "sfwJSONData") {
                    scope.IsShowCssClassProperty = true;
                }
                else {
                    scope.IsShowCssClassProperty = false;
                }
            };

            scope.IsShowCustomAttributePropertyVisible = function () {
                //if (scope.model && scope.model.Name != "sfwLiteral" && scope.model.Name != "RequiredFieldValidator" && scope.model.Name != "CompareValidator" && scope.model.Name != "sfwSoftErrors" && scope.model.Name != "TemplateField" && scope.model.Name != "udc" && scope.model.Name != "sfwRuleViewer") {
                //    if (scope.model && scope.model.Name != "sfwColumn" && scope.model.Name != "sfwCRViewer" && scope.model.Name != "sfwFileLayout" && scope.model.Name != "sfwFileUpload" && scope.model.Name != "sfwWizardProgress") {
                //        scope.IsShowCustomAttributeProperty = true;
                //    } else {
                //        scope.IsShowCustomAttributeProperty = false;
                //    }
                //}
                //else {
                //    scope.IsShowCustomAttributeProperty = false;
                //}
            };
            //#endregion

            function PopulateFormLinkRelatedDialogList(sfxTable) {
                var lstRelatedDialog = [];
                lstRelatedDialog.push("");
                if (sfxTable != null && sfxTable != undefined) {
                    PopulateFormLinkRelatedDialogs(sfxTable.Elements, lstRelatedDialog);
                }
                return lstRelatedDialog;
            }

            function PopulateFormLinkRelatedDialogs(elements, lstRelatedDialog) {
                if (elements) {
                    if (elements.Name == "items")
                        elements = elements.Elements;
                    angular.forEach(elements, function (item) {
                        if (item) {
                            if (item.Name == "sfwDialogPanel") {
                                var strCtrlID = item.dictAttributes.ID;
                                if (strCtrlID.length > 0) {
                                    lstRelatedDialog.push(strCtrlID);
                                }
                            }
                            else if (item.Name == "sfwPanel") {
                                if (item.Elements.length > 0 && item.Elements[0]) {
                                    PopulateFormLinkRelatedDialogs(item.Elements[0].Elements, lstRelatedDialog);
                                }
                            }
                            else if (item.Name == "sfwTabContainer") {
                                if (item.Elements.length > 0) {
                                    var sfxTabs = item.Elements[0];
                                    angular.forEach(sfxTabs.Elements, function (sfxTabSheet) {
                                        PopulateFormLinkRelatedDialogs(sfxTabSheet, lstRelatedDialog);
                                    });
                                }
                            }
                            else if (item.Name == "sfwWizard") {
                                if (item.Elements.length > 0 && item.Elements[0].Name == "items") {
                                    var sfxWizard = item.Elements[0];
                                    if (sfxWizard.Elements.length > 0) {
                                        angular.forEach(sfxWizard.Elements, function (sfxCntrl) {
                                            if (sfxCntrl.Elements.length > 0 && sfxCntrl.Elements[0] != null) {
                                                PopulateFormLinkRelatedDialogs(sfxCntrl.Elements[0], lstRelatedDialog);
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    });
                }
            }

            //#region Web Control Methods

            scope.LoadWebControlProperties = function () {
                if (scope.model && $rootScope.lstWebControls && $rootScope.lstWebControls.length > 0) {
                    var lst = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    lst = JSON.parse(JSON.stringify(lst));
                    if (lst && lst.length > 0) {
                        if (scope.IsAdvancedOpen) {
                            scope.objAdvanceProperties = {};
                            scope.objAdvanceProperties.isAccessibilityPropertiesExist = scope.GetIncludePropertyList(lst[0].lstAccessibility).length > 0 ? true : false;
                            scope.objAdvanceProperties.isAppearancePropertiesExist = scope.GetIncludePropertyList(lst[0].lstAppearance).length > 0 ? true : false;
                            scope.objAdvanceProperties.isBehaviorPropertiesExist = scope.GetIncludePropertyList(lst[0].lstBehavior).length > 0 ? true : false;
                            scope.objAdvanceProperties.isCustomPropertiesExist = scope.GetIncludePropertyList(lst[0].lstCustom).length > 0 ? true : false;
                            scope.objAdvanceProperties.isLayoutPropertiesExist = scope.GetIncludePropertyList(lst[0].lstLayout).length > 0 ? true : false;
                            scope.objAdvanceProperties.isNavigationPropertiesExist = scope.GetIncludePropertyList(lst[0].lstNavigation).length > 0 ? true : false;
                            scope.objAdvanceProperties.isExtraPropertiesExist = scope.GetIncludePropertyList(lst[0].lstExtra).length > 0 ? true : false;
                            scope.objAdvanceProperties.isMiscPropertiesExist = scope.GetIncludePropertyList(lst[0].lstMisc).length > 0 ? true : false;
                            if (scope.objAdvanceProperties.isBehaviorPropertiesExist) {
                                scope.objAdvanceProperties.lstBehavior = scope.GetIncludePropertyList(lst[0].lstBehavior);
                            }
                            else if (scope.objAdvanceProperties.isAccessibilityPropertiesExist) {
                                scope.objAdvanceProperties.lstAccessibility = scope.GetIncludePropertyList(lst[0].lstAccessibility);
                            }
                            else if (scope.objAdvanceProperties.isAppearancePropertiesExist) {
                                scope.objAdvanceProperties.lstAppearance = scope.GetIncludePropertyList(lst[0].lstAppearance);
                            }
                            else if (scope.objAdvanceProperties.isCustomPropertiesExist) {
                                scope.objAdvanceProperties.lstCustom = scope.GetIncludePropertyList(lst[0].lstCustom);
                            }
                            //scope.objAdvanceProperties.isCustomPropertiesExist = lst[0].lstCustom && lst[0].lstCustom.length > 0 ? true : false;
                            //scope.objAdvanceProperties.isMiscPropertiesExist = lst[0].lstMisc && lst[0].lstMisc.length > 0 ? true : false;
                        }
                        scope.SetDefaultValueToModel(lst[0]);
                        if (scope.model.Name == "sfwLabel" && "sfwIsCaption" in scope.model.dictAttributes && scope.model.dictAttributes.sfwIsCaption) {
                            var lst1 = scope.GetPropertyListByName(lst[0], "sfwIsCaption");
                            if (lst1 && lst1.length > 0) {
                                lst1[0].IsVisible = false;
                            }
                        }
                    }
                }
                scope.SetDataToWebControl();
            };

            scope.SetDataToWebControl = function () {
                if (scope.model) {
                    var ilsttempwebcontrol = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    ilsttempwebcontrol = JSON.parse(JSON.stringify(ilsttempwebcontrol));
                    scope.ClearWebControlPropertyValue(ilsttempwebcontrol[0]);
                    function iterator(value, key) {
                        var lst = scope.GetPropertyListByName(scope.objAdvanceProperties, key);

                        if (lst && lst.length > 0) {
                            lst[0].PropertyValue = value;
                        }
                    }
                    if (scope.model) {

                        angular.forEach(scope.model.dictAttributes, iterator);
                    }
                }
            };

            scope.ClearWebControlPropertyValue = function (objWebControl) {
                function iterator(x) {
                    if (x.PropertyValue != x.DefaultValue) {
                        x.PropertyValue = "";
                    }
                }
                if (objWebControl) {

                    if (objWebControl.lstAccessibility) {
                        angular.forEach(objWebControl.lstAccessibility, iterator);
                    }
                    if (objWebControl.lstAppearance) {
                        angular.forEach(objWebControl.lstAppearance, iterator);
                    }
                    if (objWebControl.lstBehavior) {
                        angular.forEach(objWebControl.lstBehavior, iterator);
                    }
                    if (objWebControl.lstCustom) {
                        angular.forEach(objWebControl.lstCustom, iterator);
                    }
                    if (objWebControl.lstLayout) {
                        angular.forEach(objWebControl.lstLayout, iterator);
                    }
                    if (objWebControl.lstNavigation) {
                        angular.forEach(objWebControl.lstNavigation, iterator);
                    }
                    if (objWebControl.lstExtra) {
                        angular.forEach(objWebControl.lstExtra, iterator);
                    }
                    if (objWebControl.lstAutoComplete) {
                        angular.forEach(objWebControl.lstAutoComplete, iterator);
                    }
                    if (objWebControl.lstRetrieval) {
                        angular.forEach(objWebControl.lstRetrieval, iterator);
                    }
                    if (objWebControl.lstMisc) {
                        angular.forEach(objWebControl.lstMisc, iterator);
                    }
                }
            };


            scope.GetPropertyListByName = function (objWebControl, propName) {
                function filterProp(x) {
                    return x.PropertyName == propName;
                }
                var lstProperties = [];
                function AddInlstProperties(x) {
                    lstProperties.push(x);
                }

                if (objWebControl) {


                    if (objWebControl.lstAccessibility) {
                        var lst = objWebControl.lstAccessibility.filter(filterProp);
                        angular.forEach(lst, AddInlstProperties);
                    }
                    if (objWebControl.lstAppearance) {
                        var lst1 = objWebControl.lstAppearance.filter(filterProp);
                        angular.forEach(lst1, AddInlstProperties);
                    }
                    if (objWebControl.lstBehavior) {
                        var lst2 = objWebControl.lstBehavior.filter(filterProp);
                        angular.forEach(lst2, AddInlstProperties);
                    }
                    if (objWebControl.lstCustom) {
                        var lst3 = objWebControl.lstCustom.filter(filterProp);
                        angular.forEach(lst3, AddInlstProperties);
                    }
                    if (objWebControl.lstLayout) {
                        var lst4 = objWebControl.lstLayout.filter(filterProp);
                        angular.forEach(lst4, AddInlstProperties);
                    }
                    if (objWebControl.lstNavigation) {
                        var lst5 = objWebControl.lstNavigation.filter(filterProp);
                        angular.forEach(lst5, AddInlstProperties);
                    }
                    if (objWebControl.lstExtra) {
                        var lst6 = objWebControl.lstExtra.filter(filterProp);
                        angular.forEach(lst6, AddInlstProperties);
                    }

                    if (objWebControl.lstAutoComplete) {
                        var lst7 = objWebControl.lstAutoComplete.filter(filterProp);
                        angular.forEach(lst7, AddInlstProperties);
                    }
                    if (objWebControl.lstRetrieval) {
                        var lst8 = objWebControl.lstRetrieval.filter(filterProp);
                        angular.forEach(lst8, AddInlstProperties);
                    }
                    if (objWebControl.lstMisc) {
                        var lst9 = objWebControl.lstMisc.filter(filterProp);
                        angular.forEach(lst9, AddInlstProperties);
                    }
                }
                return lstProperties;
            };

            scope.SetDefaultValueToModel = function (objWebControl) {
                if (scope.model) {
                    var lst = scope.GetPropertyList(objWebControl, "Common");
                    function iterator(itm) {
                        if (itm.DefaultValue != undefined && itm.DefaultValue != "") {
                            if (scope.model.dictAttributes[itm.PropertyName] == undefined || scope.model.dictAttributes[itm.PropertyName] == "") {
                                scope.model.dictAttributes[itm.PropertyName] = itm.DefaultValue;
                            }
                        }
                    }

                    if (lst && lst.length > 0) {
                        angular.forEach(lst, iterator);
                    }
                }
            };

            scope.GetPropertyList = function (objWebControl, propertyType) {
                function filterProp(x) {
                    return x.PropertyType == propertyType;
                }
                var lstProperties = [];
                function iterator(x) {
                    lstProperties.push(x);
                }
                if (objWebControl) {
                    //if (objWebControl.lstAccessibility && objWebControl.lstAccessibility.length > 0)
                    //    var lst = objWebControl.lstAccessibility.filter(filterProp);
                    //var lst1 = objWebControl.lstAppearance.filter(filterProp);
                    //var lst2 = objWebControl.lstBehavior.filter(filterProp);
                    //var lst3 = objWebControl.lstCustom.filter(filterProp);
                    //var lst4 = objWebControl.lstLayout.filter(filterProp);
                    //var lst5 = objWebControl.lstNavigation.filter(filterProp);
                    //var lst6 = objWebControl.lstExtra.filter(filterProp);

                    if (objWebControl.lstAccessibility) {
                        angular.forEach(objWebControl.lstAccessibility, iterator);
                    }
                    if (objWebControl.lstAppearance) {
                        angular.forEach(objWebControl.lstAppearance, iterator);
                    }
                    if (objWebControl.lstBehavior) {
                        angular.forEach(objWebControl.lstBehavior, iterator);
                    }

                    if (objWebControl.lstCustom) {
                        angular.forEach(objWebControl.lstCustom, iterator);
                    }

                    if (objWebControl.lstLayout) {
                        angular.forEach(objWebControl.lstLayout, iterator);
                    }
                    if (objWebControl.lstNavigation) {
                        angular.forEach(objWebControl.lstNavigation, iterator);
                    }
                    if (objWebControl.lstExtra) {
                        angular.forEach(objWebControl.lstExtra, iterator);
                    }
                    if (objWebControl.lstAutoComplete) {
                        angular.forEach(objWebControl.lstAutoComplete, iterator);
                    }
                    if (objWebControl.lstRetrieval) {
                        angular.forEach(objWebControl.lstRetrieval, iterator);
                    }
                }
                return lstProperties;
            };

            scope.onWebControlPropdbClick = function (prop) {
                if (prop.lstValues && prop.lstValues.length > 0) {
                    var selectedPropertyIndex = prop.lstValues.indexOf(prop.PropertyValue);
                    if (selectedPropertyIndex == (prop.lstValues.length - 1)) {
                        selectedPropertyIndex = -1;
                    }
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(prop.PropertyValue, prop, "PropertyValue", prop.lstValues[selectedPropertyIndex + 1]);
                    scope.onWebControlPropValueChange(prop);
                    $rootScope.UndRedoBulkOp("End");
                }
                if (event) {
                    event.stopPropagation();
                }
            }

            scope.onWebControlPropValueChange = function (prop) {
                if (prop) {
                    if (prop.DefaultValue) {
                        scope.model.dictAttributes[prop.PropertyName] = prop.DefaultValue;
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes[prop.PropertyName], scope.model.dictAttributes, prop.PropertyName, prop.PropertyValue);
                }
            };

            // for configure tab in property window(undo redo)
            scope.onWebControlValueChange = function (prop) {
                if (prop) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes[prop.key], scope.model.dictAttributes, prop.key, prop.val);
                }
            };

            scope.DeletePropertyFromModel = function (prop) {
                if (scope.model && prop) {
                    $rootScope.UndRedoBulkOp("Start");
                    var defaultValue = "";
                    var ilsttempwebcontrol = $rootScope.lstWebControls.filter(function (x) { return x.ControlName == scope.model.Name; });
                    ilsttempwebcontrol = JSON.parse(JSON.stringify(ilsttempwebcontrol));
                    if (ilsttempwebcontrol && ilsttempwebcontrol.length > 0) {
                        var lstProp = scope.GetPropertyListByName(ilsttempwebcontrol[0], prop.PropertyName);
                        if (lstProp && lstProp.length > 0) {
                            defaultValue = lstProp[0].DefaultValue;
                        }
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes[prop.PropertyName], scope.model.dictAttributes, prop.PropertyName, defaultValue);
                    $rootScope.DeleteItem(prop, scope.dictAttributesArray);
                    $rootScope.UndRedoBulkOp("End");
                }
            }
            //#endregion

            //#region Visible Rule Intellisence
            scope.visibleRuleTextChanged = function (event) {
                var data = [];
                var input = $(event.target);
                if (scope.formobject && scope.formobject.objExtraData) {
                    var iswizard = scope.formobject.dictAttributes.sfwType == "Wizard" ? true : false;
                    data = PopulateEntityRules(scope.formobject.objExtraData, iswizard, scope.formobject.dictAttributes.sfwInitialLoadGroup);
                    setSingleLevelAutoComplete(input, data);
                }
            };
            //#endregion 

            //#region event and methods for Button

            scope.PopulateEntityFieldsForOpenButton = function (IschangeEvent) {
                var alAvlFlds = [];
                PopulateControlsForActiveForm(alAvlFlds, scope.formobject, scope.model, scope.IsLookup);
                if (IschangeEvent) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEntityField, scope.model.dictAttributes, "sfwEntityField", "");
                }
                scope.model.IsEntityFieldReset = true;
                scope.lstEntityFields = [];
                if (scope.model.dictAttributes.sfwRelatedControl || scope.model.IsChildOfGrid) {
                    if (alAvlFlds.length > 0) {
                        for (var i = 0; i < alAvlFlds.length; i++) {
                            var s = alAvlFlds[i];
                            var strParamValue = "";
                            if (s.indexOf("~") > -1)
                                strParamValue = s.substring(0, s.indexOf("~"));
                            else
                                strParamValue = s;
                            if (!scope.lstEntityFields.some(function (itm) { return itm === strParamValue; })) {
                                scope.lstEntityFields.push(strParamValue);
                            }
                        }
                    }
                }
            };

            scope.PopulateBooleanValues = function () {
                scope.lstBooleanValues.push("True");
                scope.lstBooleanValues.push("False");
            };


            scope.PopulateRelatedButton = function () {
                scope.lstExecuteAfterSuccessButton = [];
                scope.lstExecuteAfterSuccessButton.push("");
                var lst = [];
                FindControlListByNames(scope.model, ['sfwButton', 'sfwLinkButton', 'sfwImageButton'], lst);
                function iteration(obj) {
                    if (obj.dictAttributes.ID !== scope.model.dictAttributes.ID) {
                        scope.lstExecuteAfterSuccessButton.push(obj.dictAttributes.ID);
                    }
                }
                angular.forEach(lst, iteration);
            };

            scope.PopulateExecuteAfterSuccessButton = function () {
                scope.lstExecuteAfterSuccessButton = [];
                scope.lstExecuteAfterSuccessButton.push("");
                var lst = [];
                var lst1 = [];
                var lst2 = [];
                FindControlListByName(scope.MainTable, 'sfwButton', lst);
                FindControlListByName(scope.MainTable, 'sfwLinkButton', lst1);
                FindControlListByName(scope.MainTable, 'sfwImageButton', lst2);
                function iteration(obj) {
                    if (obj.dictAttributes.ID !== scope.model.dictAttributes.ID) {
                        scope.lstExecuteAfterSuccessButton.push(obj.dictAttributes.ID);
                    }
                }
                angular.forEach(lst, iteration);
                angular.forEach(lst1, iteration);
                angular.forEach(lst2, iteration);
            };

            scope.LoadButtonMethodDescription = function () {
                scope.model.MethodDescription = "";
                if (scope.model.dictAttributes.sfwMethodName != undefined && scope.model.dictAttributes.sfwMethodName != "") {
                    var lst = $rootScope.LstButtonMethodLookup.filter(function (x) {
                        return x.Code == scope.model.dictAttributes.sfwMethodName;
                    });
                    if (lst && lst.length > 0) {
                        scope.model.MethodDescription = lst[0].Description;
                    }
                    else {
                        lst = $rootScope.LstButtonMethodMaintenance.filter(function (x) {
                            return x.Code == scope.model.dictAttributes.sfwMethodName;
                        });
                        if (lst && lst.length > 0) {
                            scope.model.MethodDescription = lst[0].Description;
                        }
                        else {
                            lst = $rootScope.LstButtonMethodWizard.filter(function (x) {
                                return x.Code == scope.model.dictAttributes.sfwMethodName;
                            });
                            if (lst && lst.length > 0) {
                                scope.model.MethodDescription = lst[0].Description;
                            }
                        }
                    }
                }
            };

            scope.PopulateOnClientClick = function () {
                scope.lstOnClientClick = [];
                scope.lstOnClientClick.push("");
                scope.lstOnClientClick.push("return confirm('Type your message here.')");
            };

            scope.PopulateSecurityLevel = function () {
                scope.lstSecurityLevel = [];
                scope.lstSecurityLevel.push({
                    Code: "0", Description: " None"
                });
                scope.lstSecurityLevel.push({ Code: "1", Description: " Read" });
                scope.lstSecurityLevel.push({ Code: "2", Description: " Modify" });
                scope.lstSecurityLevel.push({ Code: "3", Description: " New" });
                scope.lstSecurityLevel.push({ Code: "4", Description: " Delete" });
                scope.lstSecurityLevel.push({ Code: "5", Description: " Execute" });
            };

            scope.PopulateSelection = function () {
                scope.lstSelection = [];
                scope.lstSelection.push("None");
                scope.lstSelection.push("One");
                scope.lstSelection.push("Many");
            };

            scope.PoulateRelatedControlForNew = function () {
                scope.RelatedControlsForNew = [];
                if (scope.IsLookup)
                    GetAllControls(scope.formobject, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton", "", "", true, scope.RelatedControlsForNew, false);
                else {

                    GetAllControls(scope.formobject, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton,sfwScheduler", "", "", true, scope.RelatedControlsForNew, true);

                }
            };

            scope.onRelatedControlandEntityFieldChange = function () {
                var strActiveForm = scope.model.dictAttributes.sfwActiveForm;
                if (strActiveForm != undefined && strActiveForm != "") {
                    var alForms = strActiveForm.split(';');
                    if (alForms.length > 0) {
                        strActiveForm = alForms[0];
                        var target = "";
                        for (var i = 0; i < alForms.length; i++) {
                            var form = alForms[i];
                            if (form.contains("="))
                                form = form.substring(form.indexOf('=') + 1);
                            if (target == "") {
                                target += form;
                            } else {
                                target += ";" + form;
                            }
                        }
                    }

                    if (strActiveForm.contains("="))
                        strActiveForm = strActiveForm.substring(strActiveForm.indexOf('=') + 1);
                    var Params = [];
                    $.connection.hubMain.server.getSingleFileDetail(strActiveForm).done(function (data) {
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

                        GetNavParamCollection(data, scope.model, Params, entityIntellisenseList, scope.formobject);
                        PopulateParamValues(Params, scope.model.dictAttributes.sfwNavigationParameter);
                        var istrParameters = GetNavigationParameterValue(Params);
                        if (scope.model.dictAttributes.sfwMethodName == "btnNew_Click") {

                            var relatedControl = scope.model.dictAttributes.sfwActiveForm.contains("=") ? scope.model.dictAttributes.sfwRelatedControl : "";
                            scope.$evalAsync(function () {
                                if (istrParameters == "") {
                                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", relatedControl);
                                }
                                else {
                                    if (relatedControl != undefined && relatedControl != "") {
                                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", istrParameters + ";" + relatedControl);
                                    }
                                    else {
                                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", istrParameters);
                                    }
                                }
                            });
                        }
                        else if (scope.model.dictAttributes.sfwMethodName == "btnOpen_Click") {

                            var entityField = scope.model.dictAttributes.sfwActiveForm.contains("=") ? scope.model.dictAttributes.sfwEntityField : "";
                            scope.$evalAsync(function () {

                                if (scope.model.dictAttributes.sfwActiveForm.contains('=')) {
                                    if (istrParameters == "") {
                                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", entityField);
                                    }
                                    else {
                                        if (entityField != undefined && entityField != "") {
                                            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", istrParameters + ";" + entityField);
                                        }
                                        else {
                                            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", istrParameters);
                                        }
                                    }
                                }
                                else {
                                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", istrParameters);
                                }
                            });
                        }
                    });
                }
            };

            scope.PopulateMessageDescription = function () {
                scope.model.MessageDescription = "";
                if (scope.model.dictAttributes.sfwMessageID != undefined && scope.model.dictAttributes.sfwMessageID != "") {
                    hubMain.server.populateMessageList().done(function (lstMessages) {
                        if (lstMessages && lstMessages.length > 0) {
                            scope.$evalAsync(function () {
                                var lst = lstMessages.filter(function (x) {
                                    return x.MessageID == scope.model.dictAttributes.sfwMessageID;
                                });
                                if (lst && lst.length > 0) {
                                    scope.model.MessageDescription = lst[0].DisplayMessage;
                                }
                            });
                        }
                    });
                }
            };

            scope.PopulateWizardSteps = function () {
                scope.WizardStepNames = [];
                var lst = [];
                FindControlListByName(scope.MainTable, "sfwWizardStep", lst);
                function iterator(ctrl) {
                    var title = ctrl.dictAttributes.Title;
                    if (ctrl.dictAttributes.Title == undefined || ctrl.dictAttributes.Title == "") {
                        title = ctrl.dictAttributes.ID;
                    }
                    scope.WizardStepNames.push({
                        Code: ctrl.dictAttributes.ID, Description: title
                    });
                }

                if (lst.length > 0) {
                    var objWizardSteps = lst;
                    if (objWizardSteps) {

                        angular.forEach(objWizardSteps, iterator);
                    }
                }
            };

            scope.onRemoteObjectChanged = function (isLoadRemoteObjectMethod) {
                var RemoteObjectName = "srvCommon";
                if (scope.formobject && scope.formobject.dictAttributes.sfwRemoteObject) {
                    RemoteObjectName = scope.formobject.dictAttributes.sfwRemoteObject;
                }
                var objServerObject = GetServerMethodObject(RemoteObjectName, scope.formobject.RemoteObjectCollection);
                var lstObjectMethods = GetObjectMethods($EntityIntellisenseFactory.getEntityIntellisense(), scope.formobject.dictAttributes.sfwEntity);
                scope.lstServerMethod = PopulateServerMethod(lstObjectMethods, scope.model, objServerObject, isLoadRemoteObjectMethod);
            };

            scope.InitPDFCorrespondenceNavigationParam = function () {

                var str = '"True"';
                if (scope.model.dictAttributes.sfwNavigationParameter != undefined && scope.model.dictAttributes.sfwNavigationParameter != "") {
                    if (scope.model.dictAttributes.sfwNavigationParameter.contains("sfwShowDownloadDialog=" + str)) {
                        scope.model.IsShowDownloadDialog = true;
                    }
                    else {
                        scope.model.IsShowDownloadDialog = false;
                    }

                    if (scope.model.dictAttributes.sfwNavigationParameter.contains("sfwLaunchNewWindow=" + str)) {
                        scope.model.IsLaunchNewWindow = true;
                    }
                    else {
                        scope.model.IsLaunchNewWindow = false;
                    }
                }
            };

            scope.onUserLogParametersClick = function () {
                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.propertyName = "sfwUserLogParameters";
                newScope.isFormLink = true;
                newScope.UserLogParaDialog = $rootScope.showDialog(newScope, "Set User Log Parameters", "Form/views/CustomAttributes.html", {
                    width: 800, height: 525
                });
            };

            scope.onSearchActiveFormClick = function () {

            };

            scope.onAddMultipleActiveFormClick = function () {

            };

            scope.onNavigationParamsClick = function () {
                //Implement Navigation Parameter here for button
                //scope.SelectedObject = scope.model;
                //scope.selectedCurrentQuery = scope.selectedCurrentQuery;
                //scope.template= "Views/Form/ParameterNavigation.html";
                /*newScope.NavigationParameterDialog = ngDialog.open({
                    template: "Views/Form/ParameterNavigation.html",
                    scope: newScope,
                    closeByDocument: false,
                    className: 'ngdialog-theme-default ngdialog-theme-custom'
                    
                });*/

                if (scope.model.dictAttributes.sfwMethodName == 'btnOpenDoc_Click') {
                    var newScope = scope.$new(true);
                    newScope.formobject = scope.formobject;
                    newScope.model = scope.model;
                    newScope.NavigationParameterOpenWordDialog = $rootScope.showDialog(newScope, "Navigation Parameter", "Form/views/ParameterNavigationOpenWord.html", {
                        width: 800, height: 500
                    });
                }
                else if (scope.model.dictAttributes.sfwMethodName == "btnGridSearch_Click") {
                    var newScope = scope.$new(true);
                    newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                    newScope.SelectedObject = scope.model;
                    newScope.currentTable = scope.CurrentTable;
                    newScope.IsForm = true;
                    newScope.IsMultiActiveForm = false;
                    newScope.formobject = scope.formobject;
                    newScope.isFormLink = scope.isformlink;
                    newScope.dialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/NavParamGridSearch.html", {
                        width: 1000, height: 520
                    });
                }
                else {
                    var newScope = scope.$new(true);
                    newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                    newScope.SelectedObject = scope.model;
                    newScope.IsForm = true;
                    newScope.IsMultiActiveForm = false;
                    newScope.formobject = scope.formobject;
                    newScope.isFormLink = scope.isformlink;
                    newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", {
                        width: 1000, height: 520
                    });
                }
            };

            scope.onMessageIdClick = function () {

            };

            scope.onWorkflowActivitesClick = function () {

            };

            scope.onCheckedChanged = function () {
                if (scope.model.dictAttributes.sfwMethodName == "btnPdfCorrespondence_Click") {
                    var str = '"True"';
                    var strNavigationParameter = "";

                    if (scope.model.IsLaunchNewWindow && scope.model.IsShowDownloadDialog) {
                        strNavigationParameter = "sfwLaunchNewWindow=" + str + ";" + "sfwShowDownloadDialog=" + str;
                    }
                    else if (scope.model.IsLaunchNewWindow && !scope.model.IsShowDownloadDialog) {
                        strNavigationParameter = "sfwLaunchNewWindow=" + str;
                    }
                    else if (!scope.model.IsLaunchNewWindow && scope.model.IsShowDownloadDialog) {
                        strNavigationParameter = "sfwShowDownloadDialog=" + str;
                    }
                    else if (!scope.model.IsLaunchNewWindow && !scope.model.IsShowDownloadDialog) {
                        strNavigationParameter = "";
                    }

                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", strNavigationParameter);
                }
            };

            scope.onMethodTypeChange = function () {
                $rootScope.UndRedoBulkOp("Start");
                var stroldId = scope.model.dictAttributes.sfwMethodName;
                $rootScope.EditPropertyValue(stroldId, scope.model.dictAttributes, "sfwMethodName", scope.model.strMethodName);
                $rootScope.EditPropertyValue(stroldId, scope.model, "strMethodName", scope.model.strMethodName);

                scope.InitByControl();
                for (var key in scope.model.dictAttributes) {
                    if (key != "ID" && key != "Visible" && key != "sfwResource" && key != "sfwVisibleRule" && key != "sfwEnableRule" && key != "CssClass" && key != "sfwCustomAttributes" && key != "sfwMode"
                        && key != "sfwMethodName") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes[key], scope.model.dictAttributes, key, "");
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            };

            scope.onActiveFormTypeChange = function (blnVal) {

                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(scope.IsMultiActiveForm, scope, "IsMultiActiveForm", blnVal);
                if (scope.model.dictAttributes.sfwMethodName == 'btnOpen_Click' && !blnVal) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwEntityField, scope.model.dictAttributes, "sfwEntityField", "");
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwActiveForm, scope.model.dictAttributes, "sfwActiveForm", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                if (scope.model && scope.model.Name == "sfwLinkButton" && scope.model.dictAttributes.sfwMethodName == 'btnOpen_Click' && !blnVal) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwActiveFormField, scope.model.dictAttributes, "sfwActiveFormField", "");
                }
                $rootScope.UndRedoBulkOp("End");

            };

            scope.onTooltipNavigationParamsClick = function () {
                if (scope.model.dictAttributes && scope.model.dictAttributes.sfwActiveForm) {
                    var newScope = scope.$new(true);
                    newScope.formobject = scope.formobject;
                    newScope.SelectedObject = scope.model;
                    newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameter", "Form/views/ParameterNavigationForTooltipForm.html", {
                        width: 800, height: 520
                    });
                } else {
                    alert("Please select active tooltip form");
                }
            };
            //#endregion

            //#region Method For Image Button
            scope.PopulateImageAlign = function () {
                scope.lstImageAlign = [];
                scope.lstImageAlign.push("NotSet");
                scope.lstImageAlign.push("Left");
                scope.lstImageAlign.push("Right");
                scope.lstImageAlign.push("Baseline");
                scope.lstImageAlign.push("Top");
                scope.lstImageAlign.push("Middle");
                scope.lstImageAlign.push("Bottom");
                scope.lstImageAlign.push("AbsBottom");
                scope.lstImageAlign.push("AbsMiddle");
                scope.lstImageAlign.push("TextTop");
            };

            scope.OnActiveFormValueChange = function () {
                if (scope.model.Name == "sfwImageButton" && (scope.model.dictAttributes.sfwActiveForm != undefined && scope.model.dictAttributes.sfwActiveForm != "")) {
                    scope.lstSelectedIndex = [];
                    scope.lstSelectedIndex.push("");

                    $.connection.hubForm.server.getNewFormModel(scope.model.dictAttributes.sfwActiveForm).done(function (data) {
                        scope.receivenewformmodel(data);
                    });

                }
            };

            scope.receivenewformmodel = function (data) {
                scope.$apply(function () {
                    scope.objActiveFormModel = data;
                    if (scope.objActiveFormModel) {
                        if (scope.objActiveFormModel.dictAttributes.sfwType === "Lookup") {
                            var table = scope.objActiveFormModel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
                            if (table && table.length > 0) {
                                GetControlNames(table[0], scope.lstSelectedIndex);
                            }
                        }
                        else {
                            var table = scope.objActiveFormModel.Elements.filter(function (itm) {
                                return itm.Name == "items";
                            });
                            if (table && table.length > 0) {
                                GetControlNames(table[0], scope.lstSelectedIndex);
                            }
                        }
                    }
                });
            };

            scope.PopulateTextRelatedControl = function () {
                scope.TextRelatedControls = [];
                PopulateTextRelatedControls(scope.formobject, scope.TextRelatedControls);
            };

            scope.onRetrievalParametersClick = function (model) {

                if (model.dictAttributes.sfwActiveForm) {
                    var newScope = scope.$new(true);
                    newScope.model = model;
                    newScope.formmodel = scope.formobject;
                    newScope.targetFormModel = scope.objActiveFormModel;
                    newScope.isFormLink = true;
                    newScope.RetrievalButtonParaDialog = $rootScope.showDialog(newScope, "Retrieval Parameters", "Form/views/RetrievalButtonParameters.html", {
                        width: 800, height: 600
                    });
                } else {
                    alert("Target Form not present.");
                }

            };
            //#endregion

            //#region Load Common Properties Template (Revised)
            scope.getGroupList = function (event) {
                //scope.lstGroupList = [];
                //if (scope.formobject.objEntityModel) {
                //    function filterElement(x) {
                //        return x.Name == "groupslist";
                //    }
                //    var lstGroupList = scope.formobject.objEntityModel.Elements.filter(filterElement);
                //    if (lstGroupList && lstGroupList.length > 0) {
                //        scope.lstGroupList = lstGroupList[0].Elements;
                //    }
                //}
                scope.lstGroupList = [];
                if (scope.formobject.objExtraData && scope.formobject.objExtraData.lstGroupsList[0]) {
                    scope.lstGroupList = scope.formobject.objExtraData.lstGroupsList[0].Elements;
                }
                scope.lstGroupList.splice(0, 0, {
                    dictAttributes: { ID: "" }
                });
            };

            var isLoad = true;
            function watchOnModel(newVal, oldVal) {
                if (newVal) {

                    //#region Call Init Method
                    if (isLoad) {
                        scope.Init();
                        isLoad = false;
                    }
                    //#endregion
                    //#region Call Init Method for control
                    scope.InitByControl();
                    //#endregion

                }
            }

            scope.setEntityName = function () {
                if (scope.model && scope.model.dictAttributes.sfwRelatedGrid) {
                    var objmodel = FindControlByID(scope.formobject, scope.model.dictAttributes.sfwRelatedGrid);
                    if (objmodel && objmodel.dictAttributes.sfwEntityField) {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objmodel.dictAttributes.sfwEntityField);
                        if (object) {
                            scope.entityName = object.Entity;
                        }
                    } else {
                        scope.entityName = "";
                    }

                } else {
                    scope.setEntityNameBasedonControl();
                }
            };
            //#endregion

            //#region Retrieval functionality as per new structure

            scope.LoadClientVisibilitySource = function () {
                if (scope.model && scope.model.retrievalType && scope.model.retrievalType == "Method") {
                    scope.lstColumns = [];
                    scope.lstColumns.push({ CodeID: "", Description: "" });
                    function iterator(itm) {
                        if (itm.indexOf("=") > 0) {
                            var param = itm.substring(itm.indexOf("=") + 1);
                            scope.lstColumns.push({ CodeID: param, Description: param });
                        }
                    }
                    if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwRetrievalControls) {
                        var lst = scope.model.dictAttributes.sfwRetrievalControls.split(";");
                        if (lst && lst.length > 0) {

                            angular.forEach(lst, iterator);
                        }
                    }
                }
            };

            scope.RetrievalQueryInputChange = function (isLoad) {
                scope.lstColumns = [];
                if (scope.model.dictAttributes.sfwRetrievalQuery) {
                    var lst = scope.model.dictAttributes.sfwRetrievalQuery.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = $filter('filter')(entityIntellisenseList, {
                            ID: entityName
                        }, true);
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            var lstQuery = objEntity.Queries.filter(function (x) {
                                return x.ID == strQueryID;
                            });
                            if (lstQuery && lstQuery.length > 0) {
                                var objQuery = lstQuery[0];
                                $.connection.hubForm.server.getEntityQueryColumns(scope.model.dictAttributes.sfwRetrievalQuery, "ScopeId_" + scope.$id).done(function (data) {
                                    scope.$evalAsync(function () {
                                        var obj = { CodeID: '', Description: '' };
                                        data.splice(0, 0, obj);
                                        scope.lstColumns = data;
                                        //scope.receiveQueryColumns(data);
                                    });
                                });
                            }
                        }
                    }
                }

                if (!isLoad) {
                    $rootScope.UndRedoBulkOp("Start");

                    if (scope.model && scope.model.Name == "sfwCascadingDropDownList") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", "");
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalControls, scope.model.dictAttributes, "sfwRetrievalControls", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwClientVisibilitySource, scope.model.dictAttributes, "sfwClientVisibilitySource", "");
                    $rootScope.UndRedoBulkOp("End");
                }
            };

            scope.RetrievalMethodInputChange = function () {
                scope.lstColumns = [];
                $rootScope.UndRedoBulkOp("Start");

                if (scope.model && scope.model.Name == "sfwCascadingDropDownList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", "");
                }
                else {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalControls, scope.model.dictAttributes, "sfwRetrievalControls", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwClientVisibilitySource, scope.model.dictAttributes, "sfwClientVisibilitySource", "");
                $rootScope.UndRedoBulkOp("End");
            };

            //scope.receiveQueryColumns = function (data) {
            //    scope.$apply(function () {
            //        scope.lstColumns = data;
            //        scope.lstQryField = data;
            //    });
            //}

            scope.RetrievalMethodChange = function (flag) {
                scope.lstMethods = [];
                scope.lstColumns = [];
                $rootScope.UndRedoBulkOp("Start");

                if (scope.model.retrievalType == "Method") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalQuery, scope.model.dictAttributes, "sfwRetrievalQuery", "");

                    if (flag == true) {

                        if (scope.model && scope.model.Name == "sfwCascadingDropDownList") {
                            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", "");
                        }
                        else {
                            $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                        }
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalControls, scope.model.dictAttributes, "sfwRetrievalControls", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwClientVisibilitySource, scope.model.dictAttributes, "sfwClientVisibilitySource", "");
                    }
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var lst = $filter('filter')(entityIntellisenseList, {
                        ID: scope.formobject.dictAttributes.sfwEntity
                    }, true);
                    if (lst && lst.length > 0) {
                        if (lst[0].ObjectMethods) {
                            scope.lstMethods = lst[0].ObjectMethods;
                        }
                    }
                    $rootScope.UndRedoBulkOp("End");

                }
            };

            if (scope.model && scope.model.dictAttributes.sfwRetrievalMethod) {
                scope.model.retrievalType = "Method";
                scope.RetrievalMethodChange(false);
            }
            else {
                if (scope.model) {
                    scope.model.retrievalType = "Query";
                }
            }

            scope.getParameterRet_dialog = function () {
                var dialogScope = scope.$new(true);
                dialogScope.model = {};
                angular.copy(scope.model, dialogScope.model);
                dialogScope.formobject = {};
                angular.copy(scope.formobject, dialogScope.formobject);
                if (scope.model.Name == "sfwCascadingDropDownList") {
                    if (scope.model.dictAttributes.sfwCascadingRetrievalParameters != undefined) {
                        dialogScope.strSelectedParameters = scope.model.dictAttributes.sfwCascadingRetrievalParameters;
                    }
                }
                else {
                    if (scope.model.dictAttributes.sfwParameters != undefined) {
                        dialogScope.strSelectedParameters = scope.model.dictAttributes.sfwParameters;
                    }
                }
                if (scope.model.retrievalType == "Query" && scope.model.dictAttributes.sfwRetrievalQuery) {
                    var lst = scope.model.dictAttributes.sfwRetrievalQuery.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) {
                            return x.ID == entityName;
                        });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            var lstQuery = objEntity.Queries.filter(function (x) {
                                return x.ID == strQueryID;
                            });
                            if (lstQuery && lstQuery.length > 0) {
                                var objQuery = lstQuery[0];
                                dialogScope.selectedCurrentQuery = objQuery;
                            }
                        }
                    }
                }
                if (scope.model.retrievalType == "Method") {
                    scope.lstMethods = [];

                    var lstData = $Entityintellisenseservice.GetIntellisenseData(scope.formobject.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                    var lsttempData = [];
                    var objMethod;
                    if (lstData) {

                        if (scope.model.dictAttributes.sfwRetrievalMethod) {
                            for (var i = 0; i < lstData.length; i++) {
                                if (lstData[i].ID == scope.model.dictAttributes.sfwRetrievalMethod) {
                                    dialogScope.selectedCurrentQuery = lstData[i];
                                }
                            }
                        }
                    }
                }

                dialogScope.objNewDialog = $rootScope.showDialog(dialogScope, "Retrieval Parameters", "Form/views/RetrievalParameters.html", {
                    width: 700, height: 500
                });

                dialogScope.$on("onRetrievalParameterClick", function (event, data) {
                    if (scope.model.Name == "sfwCascadingDropDownList") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", data);
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", data);
                    }
                    event.stopPropagation();
                });
            };

            scope.$on("onRetrievalParameterClick", function (event, data) {
                if (scope.model.Name == "sfwCascadingDropDownList") {
                    scope.model.dictAttributes.sfwCascadingRetrievalParameters = data;

                }
                else {
                    scope.model.dictAttributes.sfwParameters = data;
                }
                event.stopPropagation();
            });


            var browseRetrievalControls = (function () {
                return {
                    setnewScope: function () {
                        dialogScope = scope.$new(true);
                    },
                    loadDialog: function () {
                        var dialogScope = scope.$new(true);
                        if (scope.model.dictAttributes.sfwRetrievalControls != undefined) {
                            //angular.copy($scope.model.dictAttributes.sfwRetrievalControls, dialogScope.strSelectedRetrievalControls);
                            dialogScope.strSelectedRetrievalControls = scope.model.dictAttributes.sfwRetrievalControls;
                        }
                        dialogScope.model = {};
                        angular.copy(scope.model, dialogScope.model);
                        dialogScope.formobject = {};
                        angular.copy(scope.formobject, dialogScope.formobject);
                        dialogScope.IsRetrievalQuery = true;

                        dialogScope.OnOkClick = function () {
                            scope.LoadClientVisibilitySource();
                        };

                        dialogScope.RetrievalControlsDialog = $rootScope.showDialog(dialogScope, "Retrieval Controls", "Form/views/RetrievalControls.html", {
                            width: 700, height: 500
                        });

                    },
                    afterDialog: function (data) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalControls, scope.model.dictAttributes, "sfwRetrievalControls", data);
                        dialogScope.$destroy();
                    }
                };
            }());

            scope.getRetrievalControl_dialog = function () {
                browseRetrievalControls.setnewScope();
                browseRetrievalControls.loadDialog();
            };
            scope.$on('onRetrievalControlClick', function (event, data) {
                browseRetrievalControls.afterDialog(data);
            });
            scope.RetrievalQueryChange = function () {
                if (scope.model.retrievalType == "Query") {
                    scope.lstColumns = [];
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalMethod, scope.model.dictAttributes, "sfwRetrievalMethod", "");
                    if (scope.model && scope.model.Name == "sfwCascadingDropDownList") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", "");
                    }
                    else {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalControls, scope.model.dictAttributes, "sfwRetrievalControls", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwClientVisibilitySource, scope.model.dictAttributes, "sfwClientVisibilitySource", "");
                    $rootScope.UndRedoBulkOp("End");
                }
            };

            //#endregion

            //#region AutoComplete functionality as per new structure
            if (scope.model && scope.model.Name == "sfwTextBox" && (scope.model.dictAttributes.sfwAutoMinLength == undefined || scope.model.dictAttributes.sfwAutoMinLength == "")) {
                scope.model.dictAttributes.sfwAutoMinLength = "3";
            }
            if (scope.model && scope.model.Name == "sfwTextBox" && (scope.model.dictAttributes.sfwDelay == undefined || scope.model.dictAttributes.sfwDelay == "")) {
                scope.model.dictAttributes.sfwDelay = "1000";
            }
            var browseQueryAutoComplete = (function () {
                return {
                    setnewScope: function () {
                        dialogScope = scope.$new(true);
                    },
                    loadDialog: function () {
                        if (scope.model.dictAttributes.sfwAutoQuery != undefined) {
                            dialogScope.strSelectedQuery = scope.model.dictAttributes.sfwAutoQuery;
                        }
                        dialogScope.IsAutoComplete = true;


                        dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse For Query", "Form/views/BrowseForQuery.html", {
                            width: 1000, height: 700
                        });
                    },
                    afterDialog: function (data) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoQuery, scope.model.dictAttributes, "sfwAutoQuery", data);
                        dialogScope.$destroy();
                    }
                };
            }()); // IFEE 

            scope.AutoCompleteQueryChange = function (data) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoParameters, scope.model.dictAttributes, "sfwAutoParameters", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoColumns, scope.model.dictAttributes, "sfwAutoColumns", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoFillMapping, scope.model.dictAttributes, "sfwAutoFillMapping", "");
                $rootScope.UndRedoBulkOp("End");
            };

            scope.getQueryParameter_dialog = function () {
                browseQueryAutoComplete.setnewScope();
                browseQueryAutoComplete.loadDialog();
            };
            scope.$on('onAutoCompleteClick', function (event, data) {
                browseQueryAutoComplete.afterDialog(data);
            });

            scope.getParameter_dialog = function () {
                var dialogScope = scope.$new(true);
                dialogScope.model = {};
                angular.copy(scope.model, dialogScope.model);
                dialogScope.formobject = {};
                angular.copy(scope.formobject, dialogScope.formobject);
                dialogScope.IsAutoComplete = true;
                dialogScope.FormType = scope.formobject.dictAttributes.sfwType;
                if (scope.model && (scope.model.dictAttributes.sfwAutoParameters != undefined)) {
                    dialogScope.strSelectedParameters = scope.model.dictAttributes.sfwAutoParameters;
                }
                if (scope.model.autocompleteType === "Query" && scope.model.dictAttributes.sfwAutoQuery) {
                    dialogScope.selectedCurrentQuery = $EntityIntellisenseFactory.getQueryByQueryName(scope.model.dictAttributes.sfwAutoQuery);
                }
                else if (scope.model.autocompleteType === "Method" && scope.model.dictAttributes.sfwAutoMethod) {
                    var llstObjectMethods = $Entityintellisenseservice.GetIntellisenseData(scope.entityName, null, null, true, false, true, false, false, false);
                    llstObjectMethods = llstObjectMethods.filter(function (aobjMethod) { return aobjMethod.ID.toLowerCase() === scope.model.dictAttributes.sfwAutoMethod.toLowerCase(); });
                    if (llstObjectMethods && llstObjectMethods.length) {
                        dialogScope.selectedMethod = llstObjectMethods[0];
                    }
                }
                dialogScope.objNewDialog = $rootScope.showDialog(dialogScope, "AutoComplete Parameter", "Form/views/AutoCompleteParameter.html", {
                    width: 800, height: 500
                });


                //dialogScope.RetrievalParameterDialog = ngDialog.open({
                //    template: "Form/views/AutoCompleteParameter.html",
                //    scope: dialogScope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom'
                //});
            };
            scope.$on("onAutoCompleteParameterClick", function (event, data) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoParameters, scope.model.dictAttributes, "sfwAutoParameters", data);
            });


            var browseRetrievalControlsMethod = (function () {
                return {
                    setnewScope: function () {
                        dialogScope = scope.$new(true);
                    },
                    loadDialog: function () {
                        var dialogScope = scope.$new(true);
                        if (scope.model.dictAttributes.sfwAutoFillMapping != undefined) {
                            dialogScope.strSelectedRetrievalControls = scope.model.dictAttributes.sfwAutoFillMapping;
                        }
                        dialogScope.model = {};
                        angular.copy(scope.model, dialogScope.model);
                        dialogScope.formobject = {};
                        angular.copy(scope.formobject, dialogScope.formobject);
                        dialogScope.IsAutoComplete = true;

                        dialogScope.RetrievalControlsDialog = $rootScope.showDialog(dialogScope, "Auto Complete Control Mapping", "Form/views/RetrievalControls.html", {
                            width: 700, height: 500
                        });
                    },
                    afterDialog: function (data) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoFillMapping, scope.model.dictAttributes, "sfwAutoFillMapping", data);
                        dialogScope.$destroy();
                    }
                };
            }()); // IFEE 
            scope.getRetrievalControlMethod_dialog = function () {
                browseRetrievalControlsMethod.setnewScope();
                browseRetrievalControlsMethod.loadDialog();
            };
            scope.$on('onAutoCompletelControlClick', function (event, data) {
                browseRetrievalControlsMethod.afterDialog(data);
            });

            var browseAutoCompleteColumns = (function () {
                return {
                    setnewScope: function () {
                        dialogScope = scope.$new(true);
                    },
                    loadDialog: function () {
                        var dialogScope = scope.$new(true);
                        if (scope.model && (scope.model.dictAttributes.sfwAutoColumns != undefined)) {
                            dialogScope.strSelectedAutoColumns = scope.model.dictAttributes.sfwAutoColumns;
                        }
                        dialogScope.model = {};
                        angular.copy(scope.model, dialogScope.model);
                        dialogScope.formobject = {};
                        angular.copy(scope.formobject, dialogScope.formobject);
                        dialogScope.IsAutoComplete = true;
                        dialogScope.entityName = scope.entityName;

                        dialogScope.AutoCompleteColumnDialog = $rootScope.showDialog(dialogScope, "AutoComplete Columns", "Form/views/AutoCompleteColumns.html", {
                            width: 1000, height: 500
                        });
                    },
                    afterDialog: function (data) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoColumns, scope.model.dictAttributes, "sfwAutoColumns", data);
                        dialogScope.$destroy();
                    }
                };
            }()); // IFEE 
            scope.getColumns_dialog = function () {
                browseAutoCompleteColumns.setnewScope();
                browseAutoCompleteColumns.loadDialog();
            };
            scope.$on('onAutoCompletelColumnsClick', function (event, data) {
                browseAutoCompleteColumns.afterDialog(data);
            });
            scope.okClick = function () {
                if (scope.model && scope.model.dictAttributes.sfwAutoMinLength == "3") {
                    scope.model.dictAttributes.sfwAutoMinLength = "";
                }
                if (scope.model && scope.model.dictAttributes.sfwDelay == "1000") {
                    scope.model.dictAttributes.sfwDelay = "";
                }
                scope.onCancelClick();
            };

            scope.autocompleteMethodInputChange = function () {
                scope.lstColumns = [];
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoParameters, scope.model.dictAttributes, "sfwAutoParameters", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoColumns, scope.model.dictAttributes, "sfwAutoColumns", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoFillMapping, scope.model.dictAttributes, "sfwAutoFillMapping", "");
                $rootScope.UndRedoBulkOp("End");
            };
            scope.onAutocompleteMethodChange = function (clearParams) {
                scope.lstColumns = [];
                if (scope.model.autocompleteType == "Method") {
                    if (clearParams) {
                        scope.onAutocompleteChange(scope.model.autocompleteType);
                    }
                    scope.model.dictAttributes.sfwAutoQuery = "";
                }
            };
            scope.onAutocompleteQueryChange = function (clearParams) {
                scope.lstColumns = [];
                if (scope.model.autocompleteType == "Query") {
                    if (clearParams) {
                        scope.onAutocompleteChange(scope.model.autocompleteType);
                    }
                    scope.model.dictAttributes.sfwAutoMethod = "";
                }
            };
            scope.onAutocompleteChange = function (autocompleteType, enableUndoRedo) {
                //Setting the default value to true;
                if (enableUndoRedo === undefined) {
                    enableUndoRedo = true;
                }

                if (enableUndoRedo) {
                    $rootScope.UndRedoBulkOp("Start");
                    if (autocompleteType === "Query") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalMethod, scope.model.dictAttributes, "sfwAutoMethod", "");
                    }
                    else if (autocompleteType === "Method") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRetrievalQuery, scope.model.dictAttributes, "sfwAutoQuery", "");
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoParameters, scope.model.dictAttributes, "sfwAutoParameters", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoColumns, scope.model.dictAttributes, "sfwAutoColumns", "");
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwAutoFillMapping, scope.model.dictAttributes, "sfwAutoFillMapping", "");
                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    if (autocompleteType === "Query") {
                        scope.model.dictAttributes.sfwRetrievalMethod = "";
                    }
                    else if (autocompleteType === "Method") {
                        scope.model.dictAttributes.sfwRetrievalQuery = "";
                    }
                    scope.model.dictAttributes.sfwAutoParameters = "";
                    scope.model.dictAttributes.sfwAutoColumns = "";
                    scope.model.dictAttributes.sfwAutoFillMapping = "";
                }
            }

            //#endregion

            //#region Method For Chart

            scope.lstChartType = $rootScope.ChartTypes;
            scope.Enable3DChnage = function (IsEnable3D) {
                if (!scope.Area3D && (IsEnable3D || IsEnable3D == "True")) {
                    scope.Area3D = { dictAttributes: { IsEnable3D: "True", LightStyle: "", Inclination: "" }, Elements: [], Children: [], Name: "Area3DStyle", Value: "" };
                }
                if (!IsEnable3D || IsEnable3D == "False") {
                    scope.Area3D.dictAttributes.LightStyle = "";
                    scope.Area3D.dictAttributes.Inclination = "";
                }
            };

            scope.removeSeriescolumnValues = function (chartModel) {
                if (scope.model.dictAttributes.sfwIsDynamicSeries == "False") {
                    for (var i = 0; i < chartModel.Elements.length; i++) {
                        if (chartModel.Elements[i].Name == "Series") {
                            var seriesModel = chartModel.Elements[i];
                            if (seriesModel && seriesModel.Elements.length > 0) {
                                $rootScope.UndRedoBulkOp("Start");
                                for (var j = 0; j < seriesModel.Elements.length; j++) {
                                    $rootScope.EditPropertyValue(seriesModel.Elements[j].dictAttributes.sfwSeriesColumnName, seriesModel.Elements[j].dictAttributes, "sfwSeriesColumnName", "");
                                }
                                $rootScope.UndRedoBulkOp("End");
                            }
                            break;
                        }
                    }
                }
            };

            scope.onChartTypeChanged = function () {
                if (scope.model.dictAttributes.ChartType && (scope.model.dictAttributes.ChartType == "Donut" || scope.model.dictAttributes.ChartType == "Pie")) {
                    scope.model.dictAttributes.sfwIsDynamicSeries = "False";
                    scope.removeSeriescolumnValues(scope.model);
                }
            };
            scope.setSeriesClick = function () {
                var newScope = scope.$new();
                if (scope.model.dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
                    var temp = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, scope.model.dictAttributes.sfwEntityField);
                    if (temp) {
                        newScope.ParentEntityName = temp.Entity;
                    }
                }
                newScope.ChartModel = scope.model;
                newScope.lstToolTipTypes = ['None', 'Chart', 'Table', 'Both'];
                newScope.DataFormats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];
                for (var i = 0; i < scope.model.Elements.length; i++) {
                    if (scope.model.Elements[i].Name == "Series") {
                        newScope.chartSeries = scope.model.Elements[i];
                        if (newScope.chartSeries.Elements.length > 0) {
                            newScope.ObjSeriesModel = newScope.chartSeries.Elements[0];
                            newScope.ObjSeriesModel.dictAttributes.ChartType = scope.model.dictAttributes.ChartType;
                        }
                        else {
                            // newScope.ObjSeriesModel = { dictAttributes: {}, Elements: [], Children: [], Name: "sfwSeries", Value: "" };
                        }

                    }
                }
                var dialog = $rootScope.showDialog(newScope, "Series Properties", "Form/views/SeriesProperties.html", {
                    width: 1000, height: 720
                });
                newScope.onOkClick = function () {
                    dialog.close();
                };
                newScope.selectSeries = function (obj) {
                    newScope.ObjSeriesModel = obj;
                    newScope.ObjSeriesModel.dictAttributes.ChartType = scope.model.dictAttributes.ChartType;
                };
                newScope.addSeries = function () {
                    //var name = GetNewSeriesName("series", newScope.chartSeries.Elements, 1);
                    if (!newScope.chartSeries) {
                        newScope.chartSeries = { dictAttributes: {}, Elements: [], Children: [], Name: "Series", Value: "" };
                    }

                    var name = GetNewSeriesName("Series", newScope.chartSeries, 1);
                    var obj = { dictAttributes: { Name: name, ChartType: scope.model.dictAttributes.ChartType }, Elements: [], Children: [], Name: "sfwSeries", Value: "", prefix: "swc" };

                    $rootScope.PushItem(obj, newScope.chartSeries.Elements);
                    newScope.selectSeries(obj);
                    //newScope.chartSeries.Elements.push(obj);
                };
                var count = 0;

                newScope.deleteSeries = function () {
                    var index = -1;
                    if (newScope.ObjSeriesModel) {
                        for (var i = 0; i < newScope.chartSeries.Elements.length; i++) {
                            if (newScope.chartSeries.Elements[i] == newScope.ObjSeriesModel) {
                                index = i;
                                break;
                            }
                        }
                    }
                    if (index > -1) {
                        $rootScope.DeleteItem(newScope.chartSeries.Elements[index], newScope.chartSeries.Elements);
                        //newScope.chartSeries.Elements.splice(index, 1);
                        if (newScope.chartSeries.Elements.length > 0) {
                            if (index > 0) {
                                newScope.ObjSeriesModel = newScope.chartSeries.Elements[index - 1];
                            }
                            else {
                                newScope.ObjSeriesModel = newScope.chartSeries.Elements[newScope.chartSeries.Elements.length - 1];
                            }
                        }
                        else {
                            newScope.ObjSeriesModel = undefined;
                        }
                    }
                };
                newScope.onNavigationParamsClick = function () {
                    var newNavigationScope = newScope.$new();
                    newNavigationScope.SelectedObject = newScope.ObjSeriesModel;
                    newNavigationScope.IsForm = true;
                    newNavigationScope.IsMultiActiveForm = false;
                    newNavigationScope.formobject = scope.formobject;

                    newNavigationScope.NavigationParameterDialog = $rootScope.showDialog(newNavigationScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", {
                        width: 1000, height: 520
                    });
                };
                newScope.onAdditionalChartColumnClick = function () {
                    var newColumnScope = newScope.$new();
                    newColumnScope.sfwAddtionalChartColumns = [];
                    newColumnScope.SelectedObject = newScope.ObjSeriesModel;
                    var objEntity = "";
                    if (scope.model.dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
                        objEntity = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, scope.model.dictAttributes.sfwEntityField);
                    }
                    if (objEntity) {
                        newColumnScope.ParentEntityName = objEntity.Entity;
                    }
                    if (newScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns) {
                        var temp = newScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns.split(",");
                        for (var i = 0; i < temp.length; i++) {
                            newColumnScope.sfwAddtionalChartColumns.push({ Property: temp[i] });
                        }
                    }
                    var AdditionalChartdialog = $rootScope.showDialog(newColumnScope, "Multiple Objects Selection", "Form/views/AdditionalChartColumnsDialog.html", {
                        width: 1000, height: 490
                    });
                    newColumnScope.onCancelClick = function () {
                        AdditionalChartdialog.close();
                    };
                    newColumnScope.onOkClick = function () {
                        var AddtionalChartColumns = "";
                        for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                            if (newColumnScope.sfwAddtionalChartColumns[i].Property != "") {
                                if (AddtionalChartColumns == "") {
                                    AddtionalChartColumns = newColumnScope.sfwAddtionalChartColumns[i].Property;
                                }
                                else {
                                    AddtionalChartColumns += "," + newColumnScope.sfwAddtionalChartColumns[i].Property;
                                }
                            }
                        }
                        $rootScope.EditPropertyValue(newScope.ObjSeriesModel.dictAttributes.swfAddtionalChartColumns, newScope.ObjSeriesModel.dictAttributes, "swfAddtionalChartColumns", AddtionalChartColumns);
                        AdditionalChartdialog.close();
                    };
                    newColumnScope.selectRow = function (row) {
                        newColumnScope.selectedRow = row;
                    };
                    newColumnScope.addProperty = function () {
                        newColumnScope.sfwAddtionalChartColumns.push({ Property: "" });
                    };
                    newColumnScope.deleteProperty = function () {
                        var index = -1;
                        if (newColumnScope.selectedRow) {
                            for (var i = 0; i < newColumnScope.sfwAddtionalChartColumns.length; i++) {
                                if (newColumnScope.selectedRow == newColumnScope.sfwAddtionalChartColumns[i]) {
                                    index = i;
                                    break;
                                }
                            }
                        }
                        if (index > -1) {
                            newColumnScope.sfwAddtionalChartColumns.splice(index, 1);
                            if (newColumnScope.sfwAddtionalChartColumns.length > 0) {
                                if (index > 0) {
                                    newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[index - 1];
                                }
                                else {
                                    newColumnScope.selectedRow = newColumnScope.sfwAddtionalChartColumns[newColumnScope.sfwAddtionalChartColumns.length - 1];
                                }
                            }
                            else {
                                newColumnScope.selectedRow = undefined;
                            }
                        }
                    };
                };
                newScope.ClearParameter = function () {
                    if (newScope.ObjSeriesModel && newScope.ObjSeriesModel.dictAttributes) {
                        newScope.ObjSeriesModel.dictAttributes.sfwNavigationParameter = "";
                    }
                };
                newScope.NavigateToActiveForm = function (aActiveFromID) {
                    if (aActiveFromID && aActiveFromID != "") {
                        newScope.onOkClick();
                        hubcontext.hubMain.server.navigateToFile(aActiveFromID, "").done(function (objfile) {
                            $rootScope.openFile(objfile, undefined);
                        });
                    }
                };
                newScope.OpenTooltipParams = function () {
                    var newParamScope = newScope.$new();
                    newScope.objTooltipParamsVM = $rootScope.showDialog(newParamScope, "Set Tooltip Parameters", "Form/views/SetToolTipParameters.html", {
                        width: 500, height: 420
                    });

                    newParamScope.onSfxChartTooltipTableCancelClick = function () {
                        if (newScope.objTooltipParamsVM) {
                            newScope.objTooltipParamsVM.close();
                        }
                        //ngDialog.close(newScope.objTooltipParamsVM.id);
                    };

                    newParamScope.onSfxChartTooltipTableOKClick = function () {
                        var lstselectedfields = [];
                        lstselectedfields = GetSelectedFieldList(newScope.ObjSeriesModel.lstselectedobjecttreefields, lstselectedfields);
                        var DisplayedEntity = getDisplayedEntity(newScope.ObjSeriesModel.LstDisplayedEntities);
                        var itempath = "";
                        if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                            itempath = DisplayedEntity.strDisplayName;
                        }

                        function iterator(itm) {
                            var ID = itm.ID;
                            if (itempath) {
                                ID = itempath + "." + ID;
                            }
                            if (tooltipparam) {
                                tooltipparam = tooltipparam + ";" + ID;
                            }
                            else {
                                tooltipparam = ID;
                            }
                        }
                        if (lstselectedfields.length > 0) {
                            var tooltipparam;

                            angular.forEach(lstselectedfields, iterator);

                            $rootScope.EditPropertyValue(newScope.ObjSeriesModel.dictAttributes.sfwTooltipTableParams, newScope.ObjSeriesModel.dictAttributes, "sfwTooltipTableParams", tooltipparam);
                        }
                        newParamScope.onSfxChartTooltipTableCancelClick();
                        //ngDialog.close(newScope.objTooltipParamsVM.id);
                    };
                };

            };


            //#endregion   


            scope.ClickCommonPropsPanels = function (opt) {
                scope.SelectedTab = opt;
                if (opt == 'General') {
                    if (!scope.IsGeneralOpen) {
                        scope.IsGeneralOpen = true;
                    }
                    if (scope.setEntityName) {
                        scope.setEntityName();
                    }
                }
                else if (opt == 'Advanced') {
                    if (!scope.IsAdvancedOpen) {
                        scope.IsAdvancedOpen = true;
                        scope.LoadWebControlProperties();
                    }
                    else {
                        scope.SetDataToWebControl();
                    }
                }
                else if (opt == 'Configured') {
                    scope.dictAttributesArray = [];
                    scope.ConvertTodictAttributesArray();
                    if (!scope.IsConfiguredOpen) {
                        scope.IsConfiguredOpen = true;
                    }
                }
            };

            scope.ConvertTodictAttributesArray = function () {
                var ary = [];
                if (scope.model) {
                    var lstWebControls = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    lstWebControls = JSON.parse(JSON.stringify(lstWebControls));
                    angular.forEach(scope.model.dictAttributes, function (val, key) {
                        if (val) {
                            var lst = [];
                            if (lstWebControls && lstWebControls.length > 0) {
                                lst = scope.GetPropertyListByName(lstWebControls[0], key);
                            }
                            if (!lst || (lst && lst.length == 0) || (lst && lst.length > 0 && lst[0].DefaultValue != val)) {
                                var obj = { PropertyName: key, PropertyValue: val };
                                if (lst && lst.length > 0) {
                                    obj.lstValues = lst[0].lstValues; // added this code to make Configure tab editable ,accordingly Textbox n dropdown ll come.
                                }
                                if (obj.PropertyName != 'runat') {
                                    ary.push(obj);
                                }
                            }
                        }
                    });
                    scope.dictAttributesArray = ary;
                }
            };
            var unwatch = scope.$watch('model', watchOnModel);

            scope.NavigateToEntityQuery = function (aQueryID) {
                if (aQueryID && aQueryID.contains(".")) {
                    var query = aQueryID.split(".");
                    $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
                }
            };
            scope.NavigateToEntityQueryFromDetailQueryID = function (aQueryID) {
                if (aQueryID && scope.IsLookup && scope.IsSearchCriteriaSelected && scope.formobject && scope.formobject.Elements) {
                    var objInitialLoad = scope.formobject.Elements.filter(function (x) {
                        return x.Name && x.Name == "initialload";
                    });
                    if (objInitialLoad && objInitialLoad.length > 0 && objInitialLoad[0].Elements) {
                        var lstQuery = objInitialLoad[0].Elements.filter(function (x) {
                            return x.dictAttributes && x.dictAttributes.ID == aQueryID;
                        });
                        if (lstQuery && lstQuery.length > 0 && lstQuery[0].dictAttributes.sfwQueryRef && lstQuery[0].dictAttributes.sfwQueryRef.contains(".")) {
                            var query = lstQuery[0].dictAttributes.sfwQueryRef.split(".");
                            $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
                        }
                    }
                }
            };
            scope.NavigateToEntityMethod = function (aMethodID) {
                if (aMethodID && aMethodID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "objectmethods", aMethodID);
                }
            };
            scope.NavigateToEntityExpression = function (aExpressionID) {
                if (aExpressionID && aExpressionID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "attributes", aExpressionID);
                }
            };
            scope.NavigateToEntityGroup = function (aGroupID) {
                if (aGroupID && aGroupID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "groupslist", aGroupID);
                }
            };
            scope.NavigateToActiveForm = function (aActiveFromID) {
                if (aActiveFromID && aActiveFromID != "") {
                    if (aActiveFromID.contains("=")) {
                        var temp = aActiveFromID.split(";");
                        var templstActiveForm = temp[0].split("=");
                        if (templstActiveForm && templstActiveForm.length > 1) {
                            aActiveFromID = templstActiveForm[1];
                        }
                    }
                    hubcontext.hubMain.server.navigateToFile(aActiveFromID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, undefined);
                    });
                }
            };
            scope.NavigateToRuleClick = function (aRuleID) {
                if (aRuleID && aRuleID != "") {
                    hubcontext.hubMain.server.navigateToFile(aRuleID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, false);
                    });
                }
            };

            scope.ClickCommonAdvPropsPanels = function (opt) {
                var lst = $rootScope.lstWebControls.filter(function (x) {
                    return x.ControlName == scope.model.Name;
                });
                lst = JSON.parse(JSON.stringify(lst));
                if (lst && lst.length > 0) {
                    //toggle visibilty for all related tr
                    var HTMLelementAdvancePropertiesTable = $(element).find("[advance-property-table]");
                    if (opt == 'Accessibility') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Accessibility'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Accessibility ']:not([header-properties-row])").toggle();
                    }
                    else if (opt == 'Appearance') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Appearance'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Appearance ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstAppearance == undefined) {
                            scope.objAdvanceProperties.lstAppearance = scope.GetIncludePropertyList(lst[0].lstAppearance);
                        }
                    }
                    else if (opt == 'Behaviour') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Behaviour'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Behaviour ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstBehavior == undefined) {
                            scope.objAdvanceProperties.lstBehavior = scope.GetIncludePropertyList(lst[0].lstBehavior);
                        }
                    }
                    else if (opt == 'Custom') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Custom'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Custom ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstCustom == undefined) {
                            scope.objAdvanceProperties.lstCustom = lst[0].lstCustom;
                        }
                    }
                    else if (opt == 'Layout') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Layout'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Layout ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstLayout == undefined) {
                            scope.objAdvanceProperties.lstLayout = scope.GetIncludePropertyList(lst[0].lstLayout);
                        }
                    }
                    else if (opt == 'Navigation') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Navigation'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Navigation ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstExtra == undefined) {
                            scope.objAdvanceProperties.lstExtra = scope.GetIncludePropertyList(lst[0].lstExtra);
                        }
                    }
                    else if (opt == 'Extra') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Extra'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Extra ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstNavigation == undefined) {
                            scope.objAdvanceProperties.lstNavigation = scope.GetIncludePropertyList(lst[0].lstNavigation);
                        }
                    }
                    else if (opt == 'Misc') {
                        $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Misc'] td").toggleClass("prop-close"); // handling collapsing icons                        
                        $(HTMLelementAdvancePropertiesTable).find("tr[category='Misc ']:not([header-properties-row])").toggle();
                        if (scope.objAdvanceProperties.lstMisc == undefined) {
                            scope.objAdvanceProperties.lstMisc = scope.GetIncludePropertyList(lst[0].lstMisc);
                        }
                    }
                    scope.SetDataToWebControl();
                }
            };
            scope.NavigateToEntityField = function (astrEntityField, astrEntityName, propertyName) {
                if (astrEntityField && scope.model) {
                    var arrText = astrEntityField.split('.');
                    var data = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = "";
                    var gridObj = null;
                    if (scope.model.Name == "TemplateField") {
                        gridObj = FindParent(scope.model, "sfwGridView");
                    }
                    if (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwListView" || scope.model.Name == "sfwChart" || (gridObj && gridObj.Name == "sfwGridView")) {
                        var parent = FindParent(scope.model, "sfwListView");
                        var gridModel = gridObj ? gridObj : scope.model;
                        if (parent) {
                            var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parent.dictAttributes.sfwEntityField);
                            if (objParentField) {
                                var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, gridModel.dictAttributes.sfwEntityField);
                                if (objField) {
                                    parententityName = objParentField.Entity;
                                }
                            }
                        }
                        else if (gridModel.dictAttributes.sfwParentGrid && scope.model.Name == "TemplateField") {
                            parententityName = scope.entityName;
                        }
                        else if (gridModel.dictAttributes.sfwParentGrid && gridModel.dictAttributes.sfwEntityField) {
                            var parentGrid = FindControlByID(scope.formobject, gridModel.dictAttributes.sfwParentGrid);
                            var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
                            if (objParentField) {
                                parententityName = objParentField.Entity;
                            }
                        }
                        else {
                            parententityName = scope.formobject.dictAttributes.sfwEntity;
                        }

                    }
                    else if (astrEntityName) {
                        parententityName = astrEntityName;
                    }
                    else if ((scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton") && scope.model.dictAttributes.sfwMethodName && scope.model.dictAttributes.sfwMethodName == "btnOpen_Click" && scope.model.dictAttributes.sfwRelatedControl) {
                        var grid = GetFieldFromFormObject(scope.formobject, 'sfwGridView', 'ID', scope.model.dictAttributes.sfwRelatedControl);
                        if (grid && grid.length > 0 && grid[0].dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
                            var tempparententityName = scope.formobject.dictAttributes.sfwEntity;
                            var tempData = [];
                            while (tempparententityName) {
                                var entity = entities.filter(function (x) {
                                    return x.ID == tempparententityName;
                                });
                                if (entity.length > 0) {
                                    var attributes = entity[0].Attributes;
                                    tempData = tempData.concat(attributes);
                                    tempparententityName = entity[0].ParentId;
                                }
                                else {
                                    tempparententityName = "";
                                }
                            }
                            if (grid[0].dictAttributes.sfwEntityField && (grid[0].dictAttributes.sfwEntityField == "InternalErrors" || grid[0].dictAttributes.sfwEntityField == "ExternalErrors")) {
                                parententityName = "entError";
                                astrEntityName = parententityName;
                            }
                            else {
                                var item = tempData.filter(function (x) { return x.ID == grid[0].dictAttributes.sfwEntityField; });
                                if (item && item.length > 0 && item[0].Entity) {
                                    parententityName = item[0].Entity;
                                    astrEntityName = parententityName;
                                }
                            }

                        }
                        else if (scope.formobject.dictAttributes.sfwEntity) {
                            parententityName = scope.formobject.dictAttributes.sfwEntity;
                        }
                    }
                    else {
                        parententityName = scope.entityName;
                    }
                    while (parententityName) {
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            var attributes = entity[0].Attributes;
                            var tempAttributes = getFilteredAttribute(scope.model.Name, attributes, arrText.length > 1, propertyName);
                            data = data.concat(tempAttributes);
                            parententityName = entity[0].ParentId;
                        }
                        else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        for (var index = 0; index < arrText.length - 1; index++) {
                            var item = data.filter(function (x) { return x.ID == arrText[index]; });
                            if (item.length > 0) {
                                if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
                                    var parententityName = item[0].Entity;
                                    if (index < arrText.length - 2) {
                                        data = [];
                                    }
                                    while (parententityName) {
                                        var entity = entities.filter(function (x) {
                                            return x.ID == parententityName;
                                        });
                                        if (entity.length > 0) {
                                            var attributes = entity[0].Attributes;
                                            var tempAttributes = getFilteredAttribute(scope.model.Name, attributes, index < arrText.length - 2, propertyName);
                                            data = data.concat(tempAttributes);
                                            parententityName = entity[0].ParentId;
                                        }
                                        else {
                                            parententityName = "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (data && data.length > 0) {
                        var tempField = data.filter(function (x) { return x.ID == arrText[arrText.length - 1]; });
                        if (tempField && tempField.length > 0) {
                            var attributeID = arrText[arrText.length - 1];
                            if (tempField[0].Type && tempField[0].Type == "Description" && arrText[arrText.length - 1] && arrText[arrText.length - 1].toLowerCase().endsWith("description")) {
                                attributeID = arrText[arrText.length - 1].substr(0, (arrText[arrText.length - 1].length - 11)) + "Value";
                            }
                            if (arrText.length == 1) {
                                var EntityName = "";
                                var gridObj = null;
                                if (scope.model.Name == "TemplateField") {
                                    gridObj = FindParent(scope.model, "sfwGridView");
                                }
                                if (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwListView" || scope.model.Name == "sfwChart" || (gridObj && gridObj.Name == "sfwGridView")) {
                                    var parent = FindParent(scope.model, "sfwListView");
                                    var gridModel = gridObj ? gridObj : scope.model;
                                    if (parent) {
                                        var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parent.dictAttributes.sfwEntityField);
                                        if (objParentField) {
                                            var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, gridModel.dictAttributes.sfwEntityField);
                                            if (objField) {
                                                EntityName = objParentField.Entity;
                                            }
                                        }
                                    }
                                    else if (gridModel.dictAttributes.sfwParentGrid && scope.model.Name == "TemplateField") {
                                        EntityName = scope.entityName;
                                    }
                                    else if (gridModel.dictAttributes.sfwParentGrid && gridModel.dictAttributes.sfwEntityField) {
                                        var parentGrid = FindControlByID(scope.formobject, gridModel.dictAttributes.sfwParentGrid);
                                        var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
                                        if (objParentField) {
                                            EntityName = objParentField.Entity;
                                        }
                                    }

                                    else {
                                        EntityName = scope.formobject.dictAttributes.sfwEntity;
                                    }
                                }
                                else if (astrEntityName) {
                                    EntityName = astrEntityName;
                                }
                                else {
                                    EntityName = scope.entityName;
                                }
                                $rootScope.IsLoading = true;
                                if (EntityName) {
                                    $NavigateToFileService.NavigateToFile(EntityName, "attributes", attributeID);
                                }
                            }
                            else if (arrText.length > 1) {
                                var item = data.filter(function (x) { return x.ID == arrText[arrText.length - 2]; });
                                if (item && item != "" && item[0].Entity) {
                                    $rootScope.IsLoading = true;
                                    $NavigateToFileService.NavigateToFile(item[0].Entity, "attributes", attributeID);
                                }
                            }
                        }

                    }
                }
            }
            //scope.NavigateToEntityField = function (astrEntityField, astrEntityName, propertyName) {
            //    if (astrEntityField && scope.model) {

            //        var arrText = astrEntityField.split('.');
            //        var data = [];
            //        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            //        var entities = entityIntellisenseList;
            //        var parententityName = "";
            //        var gridObj = null;
            //        if (scope.model.Name == "TemplateField") {
            //            gridObj = FindParent(scope.model, "sfwGridView");
            //        }
            //        if (scope.model.Name == "sfwGridView" || scope.model.Name == "sfwListView" || scope.model.Name == "sfwChart" || (gridObj && gridObj.Name == "sfwGridView")) {
            //            var parent = FindParent(scope.model, "sfwListView");
            //            var gridModel = gridObj ? gridObj : scope.model;
            //            if (parent) {
            //                var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parent.dictAttributes.sfwEntityField);
            //                if (objParentField) {

            //                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(objParentField.Entity, gridModel.dictAttributes.sfwEntityField);
            //                    if (objField) {
            //                        parententityName = objParentField.Entity;
            //                    }
            //                }
            //            }
            //            else if (gridModel.dictAttributes.sfwParentGrid && gridModel.dictAttributes.sfwEntityField) {
            //                var parentGrid = FindControlByID(scope.formobject, gridModel.dictAttributes.sfwParentGrid);
            //                var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, parentGrid.dictAttributes.sfwEntityField);
            //                if (objParentField) {
            //                    parententityName = objParentField.Entity;
            //                }
            //            }
            //            else {
            //                parententityName = scope.formobject.dictAttributes.sfwEntity;
            //            }

            //        }
            //        else if (astrEntityName) {
            //            parententityName = astrEntityName;
            //        }
            //        else if ((scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton") && scope.model.dictAttributes.sfwMethodName && scope.model.dictAttributes.sfwMethodName == "btnOpen_Click" && scope.model.dictAttributes.sfwRelatedControl) {
            //            var grid = GetFieldFromFormObject(scope.formobject, 'sfwGridView', 'ID', scope.model.dictAttributes.sfwRelatedControl);
            //            if (grid && grid.length > 0 && grid[0].dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
            //                var tempparententityName = scope.formobject.dictAttributes.sfwEntity;
            //                var tempData = [];
            //                while (tempparententityName) {
            //                    var entity = entities.filter(function (x) {
            //                        return x.ID == tempparententityName;
            //                    });
            //                    if (entity.length > 0) {
            //                        var attributes = entity[0].Attributes;
            //                        tempData = tempData.concat(attributes);
            //                        tempparententityName = entity[0].ParentId;
            //                    }
            //                    else {
            //                        tempparententityName = "";
            //                    }
            //                }
            //                if (grid[0].dictAttributes.sfwEntityField && (grid[0].dictAttributes.sfwEntityField == "InternalErrors" || grid[0].dictAttributes.sfwEntityField == "ExternalErrors")) {
            //                    parententityName = "entError";
            //                    astrEntityName = parententityName;
            //                }
            //                else {
            //                    var item = tempData.filter(function (x) {
            //                        return x.ID == grid[0].dictAttributes.sfwEntityField;
            //                    });
            //                    if (item && item.length > 0 && item[0].Entity) {
            //                        parententityName = item[0].Entity;
            //                        astrEntityName = parententityName;
            //                    }
            //                }
            //            }
            //            else if (scope.formobject.dictAttributes.sfwEntity) {
            //                parententityName = scope.formobject.dictAttributes.sfwEntity;
            //            }
            //        }
            //        else {
            //            parententityName = scope.entityName;
            //        }
            //        while (parententityName) {
            //            var entity = entities.filter(function (x) {
            //                return x.ID == parententityName;
            //            });
            //            if (entity.length > 0) {
            //                var attributes = entity[0].Attributes;
            //                var tempAttributes = getFilteredAttribute(scope.model.Name, attributes, arrText.length > 1, propertyName);
            //                data = data.concat(tempAttributes);
            //                parententityName = entity[0].ParentId;
            //            }
            //            else {
            //                parententityName = "";
            //            }
            //        }
            //        if (arrText.length > 1) {
            //            scope.isMultilevelActive = true;
            //            for (var index = 0; index < arrText.length - 1; index++) {
            //                var item = data.filter(function (x) {
            //                    return x.ID == arrText[index];
            //                });
            //                if (item.length > 0) {
            //                    if (typeof item[0].DataType != "undefined" && (item[0].DataType == "Object" || item[0].DataType == "Collection" || item[0].DataType == "CDOCollection" || item[0].DataType == "List") && typeof item[0].Entity != "undefined") {
            //                        var parententityName = item[0].Entity;
            //                        if (index < arrText.length - 2) {
            //                            data = [];
            //                        }
            //                        while (parententityName) {
            //                            var entity = entities.filter(function (x) {
            //                                return x.ID == parententityName;
            //                            });
            //                            if (entity.length > 0) {
            //                                var attributes = entity[0].Attributes;
            //                                var tempAttributes = getFilteredAttribute(scope.model.Name, attributes, index < arrText.length - 2, propertyName);
            //                                data = data.concat(tempAttributes);
            //                                parententityName = entity[0].ParentId;
            //                            }
            //                            else {
            //                                parententityName = "";
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (data && data.length > 0) {
            //            var tempField = data.filter(function (x) {
            //                return x.ID == arrText[arrText.length - 1];
            //            });
            //            if (tempField && tempField.length > 0) {
            //                var attributeID = arrText[arrText.length - 1];
            //                if (tempField[0].Type && tempField[0].Type == "Description" && arrText[arrText.length - 1] && arrText[arrText.length - 1].toLowerCase().endsWith("description")) {
            //                    attributeID = arrText[arrText.length - 1].substr(0, (arrText[arrText.length - 1].length - 11)) + "Value";
            //                }
            //                if (arrText.length == 1) {
            //                    var EntityName = "";
            //                    if (astrEntityName) {
            //                        EntityName = astrEntityName;
            //                    }
            //                    else {
            //                        EntityName = scope.entityName;
            //                    }
            //                    if (EntityName) {
            //                        $rootScope.IsLoading = true;
            //                        $NavigateToFileService.NavigateToFile(EntityName, "attributes", attributeID);
            //                    }
            //                }
            //                else if (arrText.length > 1) {
            //                    var item = data.filter(function (x) {
            //                        return x.ID == arrText[arrText.length - 2];
            //                    });
            //                    if (item && item != "" && item[0].Entity) {
            //                        $rootScope.IsLoading = true;
            //                        $NavigateToFileService.NavigateToFile(item[0].Entity, "attributes", attributeID);
            //                    }
            //                }
            //            }

            //        }
            //    }
            //};
            scope.setDefaultCodeGroup = function () {

                if (scope.model && scope.entityName && (scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwListPicker" || scope.model.Name == "sfwListBox" || scope.model.Name == "sfwRadioButtonList")) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var strCodeGroup = "";
                    if ((scope.IsSearchCriteriaSelected || scope.isFilterCriteria) && scope.model.dictAttributes.sfwDataField) {
                        strCodeGroup = GetCodeIDForLookup(scope.entityName, scope.model.dictAttributes.sfwDataField, entityIntellisenseList);
                    }
                    else if (scope.model.dictAttributes.sfwEntityField) {
                        strCodeGroup = GetCodeID(scope.entityName, scope.model.dictAttributes.sfwEntityField, entityIntellisenseList);
                    }
                    if (strCodeGroup) {
                        scope.model.placeHolder = strCodeGroup;
                    }
                    else {
                        scope.model.placeHolder = "";
                    }
                    var prop = "";
                    if ((scope.IsSearchCriteriaSelected || scope.isFilterCriteria) && scope.model.dictAttributes.sfwDataField) {
                        prop = "sfwDataField";
                    } else if (scope.model.dictAttributes.sfwEntityField) {
                        prop = "sfwEntityField";
                    }
                }
                else if (scope.model && scope.model.Name == "sfwCheckBoxList" && scope.model.CheckBoxFieldEntity) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var strCodeGroup = "";
                    if (scope.model.dictAttributes.sfwCheckBoxField) {
                        strCodeGroup = GetCodeID(scope.model.CheckBoxFieldEntity, scope.model.dictAttributes.sfwCheckBoxField, entityIntellisenseList);
                    }
                    if (strCodeGroup) {
                        scope.model.placeHolder = strCodeGroup;
                    }
                    else {
                        scope.model.placeHolder = "";
                    }
                }

            };

            scope.setDefaultValueForCheckBoxList = function () {
                var DialogScope = scope.$new();
                DialogScope.selectAll = false;
                DialogScope.lstValues = [];
                DialogScope.checkBoxListDefaultValueSelectAllChange = function (ablnCheckAll) {
                    if (DialogScope.lstValues.length > 0) {
                        angular.forEach(DialogScope.lstValues, function (aDefaultValue) {
                            aDefaultValue.selected = ablnCheckAll;
                        });
                    }
                };
                DialogScope.selectDefaultValueCheckBox = function () {
                    if (scope.model.dictAttributes.sfwDefaultValue && scope.model.dictAttributes.sfwDefaultValue == "FW_CHECKALL") {
                        DialogScope.selectAll = true;
                        DialogScope.checkBoxListDefaultValueSelectAllChange(true);
                    }
                    else if (scope.model.dictAttributes.sfwDefaultValue && DialogScope.lstValues && DialogScope.lstValues.length > 0) {
                        var arrDefaultValues = scope.model.dictAttributes.sfwDefaultValue.split(";");
                        angular.forEach(arrDefaultValues, function (aDefaultValue) {
                            for (var i = 0; i < DialogScope.lstValues.length; i++) {
                                if (DialogScope.lstValues[i].CodeValue === aDefaultValue) {
                                    DialogScope.lstValues[i].selected = true;
                                }
                            }
                        });
                    }
                };
                if (scope.model.dictAttributes.sfwLoadType == 'CodeGroup') {
                    var codeID = "";
                    if (scope.model.dictAttributes.sfwLoadSource && scope.model.dictAttributes.sfwLoadType == "CodeGroup") {
                        codeID = scope.model.dictAttributes.sfwLoadSource;
                    }
                    else if (scope.model.placeHolder) {
                        codeID = scope.model.placeHolder;
                    }
                    if (codeID) {
                        $rootScope.IsLoading = true;
                        $.connection.hubMain.server.getCodeValuesForDropDown(codeID).done(function (data) {
                            scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                                if (data && data.length > 0) {
                                    DialogScope.lstValues = data;
                                    DialogScope.selectDefaultValueCheckBox();
                                }
                            });
                        });
                    }
                }
                else if (scope.model.dictAttributes.sfwLoadType == 'Items') {
                    if (scope.model.Elements.length > 0) {
                        angular.forEach(scope.model.Elements, function (aitems) {
                            var tempItemObject = { CodeValue: "", Description: "" };
                            if (aitems.dictAttributes.Value) {
                                tempItemObject.CodeValue = aitems.dictAttributes.Value;
                            }
                            if (aitems.dictAttributes.Text) {
                                tempItemObject.Description = aitems.dictAttributes.Text;
                            }
                            DialogScope.lstValues.push(tempItemObject);
                        });
                        DialogScope.selectDefaultValueCheckBox();
                    }
                }


                dialog = $rootScope.showDialog(DialogScope, "CheckBoxList Default Values", "Form/views/SetDefaultValueForCheckBoxList.html");
                DialogScope.checkBoxListDefaultValueOkClick = function () {
                    $rootScope.UndRedoBulkOp("Start");
                    var strDefaultValue = "";
                    scope.model.dictAttributes.sfwDefaultValue = "";
                    if (DialogScope.selectAll) {
                        strDefaultValue = "FW_CHECKALL";
                    }
                    else if (DialogScope.lstValues && DialogScope.lstValues.length > 0) {
                        angular.forEach(DialogScope.lstValues, function (aDefaultValues) {
                            if (aDefaultValues.selected && aDefaultValues.CodeValue) {
                                if (strDefaultValue) {
                                    strDefaultValue += ";" + aDefaultValues.CodeValue;
                                }
                                else {
                                    strDefaultValue = aDefaultValues.CodeValue;
                                }
                            }
                        });
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwDefaultValue, scope.model.dictAttributes, "sfwDefaultValue", strDefaultValue);
                    $rootScope.UndRedoBulkOp("End");
                    DialogScope.closeDialog();
                };

                DialogScope.closeDialog = function () {
                    dialog.close();
                };

            };

            scope.GetIncludePropertyList = function (lstProperty) {
                var IncludedProps = [];
                if (scope.model) {
                    for (var i = 0; i < lstProperty.length > 0; i++) {
                        var lstProp = GetVisiblePropertyForHtmlControls(scope.model.Name);
                        if (lstProperty[i].PropertyName == "Visible" || lstProperty[i].PropertyName == "CssClass" || lstProperty[i].PropertyName.startsWith("sfw") || (lstProp && lstProp.some(function (x) { return x == lstProperty[i].PropertyName }))) {
                            IncludedProps.push(lstProperty[i]);
                        }
                    }
                }
                return IncludedProps;
            };

            scope.ClearSelectColVisibleRule = function () {
                if (scope.model.dictAttributes.sfwSelectColVisibleRule && (scope.model.dictAttributes.sfwSelection != "Many" && scope.model.dictAttributes.sfwSelection != "One")) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwSelectColVisibleRule, scope.model.dictAttributes, "sfwSelectColVisibleRule", "");
                }
            };

            //#region on Change of parent Grid
            scope.onChangeOfParentGrid = function () {
                scope.setEntityNameBasedonControl();


            };


            scope.onChangeOfMethodType = function (methodType) {
                $rootScope.UndRedoBulkOp("Start");
                scope.showRules = false;
                scope.showObjectMethod = false;
                scope.showXmlMethod = false;
                if (methodType == "ObjectMethod") {
                    scope.showObjectMethod = true;
                    scope.Title = "Object Method :";
                }
                else if (methodType == "XmlMethod") {
                    scope.showXmlMethod = true;
                    scope.Title = "Xml Method :";
                }
                else if (methodType == "Rule") {
                    scope.showRules = true;
                    scope.Title = "Rule :";
                }
                scope.model.dictAttributes.sfwObjectMethod = '';
                scope.model.dictAttributes.sfwNavigationParameter = '';

                $rootScope.UndRedoBulkOp("End");
            };
            scope.setQueryParametersForOpenDetail = function () {

                var newScope = scope.$new();
                newScope.Elements = [];
                var lstParams = scope.model.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                if (lstParams && lstParams.length > 0) {
                    if (lstParams[0] && lstParams[0].Elements && lstParams[0].Elements.length > 0) {
                        for (var i = 0; i < lstParams[0].Elements.length; i++) {
                            var param = {};
                            angular.copy(lstParams[0].Elements[i], param);
                            newScope.Elements.push(param);
                        }
                    }
                }
                newScope.entityTreeBinding = { selectedobject: null, lstselectedobjects: [], lstentities: [] };
                if (scope.model.dictAttributes.sfwRelatedControl) {
                    var lobjControlModel = FindControlByID(scope.formobject, scope.model.dictAttributes.sfwRelatedControl);
                    if (lobjControlModel) {
                        var entityAttr = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, lobjControlModel.dictAttributes.sfwEntityField);
                        if (entityAttr) {
                            newScope.FormEntity = entityAttr.Entity;
                        }
                    }
                }
                if (!newScope.FormEntity) {
                    newScope.FormEntity = scope.formobject.dictAttributes.sfwEntity;
                }
                newScope.onOkClick = function () {
                    if (newScope.Elements && newScope.Elements.length > 0) {
                        $rootScope.UndRedoBulkOp("Start");
                        if (lstParams && lstParams.length > 0) {
                            $rootScope.EditPropertyValue(lstParams[0].Elements, lstParams[0], "Elements", []);
                            for (var i = 0; i < newScope.Elements.length; i++) {
                                $rootScope.PushItem(newScope.Elements[i], lstParams[0].Elements);
                            }
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                    newScope.dialog.close();
                };
                newScope.onParameterSelected = function (objParam) {
                    newScope.SelectedParameter = objParam;
                };
                newScope.dialog = $rootScope.showDialog(newScope, "Base Query Parameters", "Form/views/ControlProperties/GridBaseQueryParameter.html", { width: 800, height: 525 });
            };
            scope.updateOpenDetailQueryParameters = function () {
                $rootScope.UndRedoBulkOp("Start");
                var parametersModel = scope.model.Elements.filter(function (itm) { return itm.Name === "Parameters" })[0];
                if (scope.model.dictAttributes.sfwQueryID) {
                    if (!parametersModel) {
                        parametersModel = {
                            Name: "Parameters", dictAttributes: {}, Elements: []
                        };

                        $rootScope.PushItem(parametersModel, scope.model.Elements);
                    }
                    $rootScope.EditPropertyValue(parametersModel.Elements, parametersModel, "Elements", []);
                    //Bring parameters and update
                    var queryModel = $EntityIntellisenseFactory.getQueryByQueryName(scope.model.dictAttributes.sfwQueryID);
                    if (queryModel) {
                        for (var i = 0, len = queryModel.Parameters.length; i < len; i++) {
                            var lcurr = queryModel.Parameters[i];
                            var newModel = {
                                Name: "Parameter", dictAttributes: {
                                    ID: lcurr.ID,
                                    sfwDataType: lcurr.DataType,
                                }, Elements: []
                            };
                            $rootScope.PushItem(newModel, parametersModel.Elements);
                        }
                        //for (param of queryModel.Parameters) {
                        //    var newModel = {
                        //        Name: "Parameter", dictAttributes: {
                        //            ID: param.ID,
                        //            sfwDataType: param.DataType,
                        //        }, Elements: []
                        //    };
                        //    $rootScope.PushItem(newModel, parametersModel.Elements);
                        //}
                    }
                }
                else {
                    if (parametersModel) {
                        var index = scope.model.Elements.indexOf(parametersModel);
                        if (index > -1) {
                            $rootScope.DeleteItem(parametersModel, scope.model.Elements)
                        }
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }
            scope.onTextModeChanged = function () {
                if (scope.model.dictAttributes.TextMode === "MultiLine") {
                    var updatedRenderTypes = ["None", "RichText"];
                    if (updatedRenderTypes.indexOf(scope.model.dictAttributes.sfwRenderType) === -1) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRenderType, scope.model.dictAttributes, "sfwRenderType", "None");
                    }
                    $rootScope.EditPropertyValue(scope.renderTypes, scope, "renderTypes", updatedRenderTypes);
                }
                else if (scope.model.dictAttributes.TextMode === "SingleLine") {
                    $rootScope.EditPropertyValue(scope.renderTypes, scope, "renderTypes", ["None", "RichText", "Rating", "NumSpinner"]);
                }
                else {
                    if (scope.model.dictAttributes.sfwRenderType !== "None") {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwRenderType, scope.model.dictAttributes, "sfwRenderType", "None");
                    }
                }
            };
            scope.onRenderTypeChange = function (astrRenderType) {
                $rootScope.UndRedoBulkOp("Start");
                switch (astrRenderType) {
                    case "None":
                    case "RichText":
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingFullStar, scope.model.dictAttributes, "RatingFullStar", "True");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingNormalFill, scope.model.dictAttributes, "RatingNormalFill", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingRatedFill, scope.model.dictAttributes, "RatingRatedFill", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingStarWidth, scope.model.dictAttributes, "RatingStarWidth", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwMinValue, scope.model.dictAttributes, "sfwMinValue", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwMaxValue, scope.model.dictAttributes, "sfwMaxValue", "");
                        break;
                    case "Rating":
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwMinValue, scope.model.dictAttributes, "sfwMinValue", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwMaxValue, scope.model.dictAttributes, "sfwMaxValue", "");
                        break;
                    case "NumSpinner":
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingFullStar, scope.model.dictAttributes, "RatingFullStar", "True");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingNormalFill, scope.model.dictAttributes, "RatingNormalFill", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingRatedFill, scope.model.dictAttributes, "RatingRatedFill", "");
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.RatingStarWidth, scope.model.dictAttributes, "RatingStarWidth", "");
                        break;
                }
                $rootScope.UndRedoBulkOp("End");
            };
        },
        templateUrl: 'FormLink/views/ControlProperties/HtmlControlCommonProperties.html'
    };
}]);