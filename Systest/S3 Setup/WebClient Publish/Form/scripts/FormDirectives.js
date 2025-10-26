app.directive('formcontroldraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '=',
            dragfromleft: '='

        },
        link: function (scope, element, attributes) {
            if (scope.dragdata) {
                var el = element[0];
                el.draggable = true;
                el.addEventListener('dragstart', handleDragStart, false);

            }
            function handleDragStart(e) {
                dragDropData = null;
                dragDropDataObject = null;
                dragfromleft = undefined;
                e.dataTransfer.effectAllowed = 'move';
                dragDropData = scope.dragdata;
                draggingDataFromLeft = scope.dragfromleft;
                dragDropDataObject = null;
                e.dataTransfer.setData("Text", "");
            }
        }
    };
}]);

app.directive('formcontroldroppable', ["$rootScope", "ngDialog", "$compile", "$EntityIntellisenseFactory", "$timeout", "$GetEntityFieldObjectService", "$ValidationService", "$SgMessagesService", function ($rootScope, ngDialog, $compile, $EntityIntellisenseFactory, $timeout, $GetEntityFieldObjectService, $ValidationService, $SgMessagesService) {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            formodel: '=',
            lstdisplayentities: '=',
            entitytreename: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];

            el.addEventListener('dragover', handleDragOver, false);
            el.addEventListener('drop', handleDrop, false);
            el.addEventListener('dragleave', handleDragLeave, false);
            var col = $(element);
            var elhover;
            var eldropdown;

            scope.openControlList = function () {
                if (eldropdown) {
                    eldropdown.remove();
                }
                eldropdown = $compile('<div id="controlmenudropdown" class="dropdown-content-common" onclick="stopPropogation(event)" addcontroltemplate item="dropdata" formodel="formodel"></div>')(scope);
                eldropdown.css({
                    left: scope.rect.right - 200,
                    top: scope.rect.top + 8,
                    bottom: scope.rect.bottom
                });
                elhover.append(eldropdown);
            };
            // for adding controls to column - on hover
            $(element).hover(function () {

                if (scope.dropdata && !FindParent(scope.dropdata, "sfwGridView") && scope.dropdata.Name != "sfwTabSheet") {

                    $("[add-control-hover-trigger]").remove();

                    //  $(".row-col").remove();
                    elhover = $compile('<i class="fa fa-plus-circle plusicon" add-control-hover-trigger ng-click="openControlList()" aria-hidden="true" style="z-index:999"></i>')(scope);
                    scope.rect = this.getBoundingClientRect();
                    col.css({
                        'position': 'relative'
                    });
                    col.append(elhover);
                    //scope.colHeight = col[0] && col[0].offsetHeight;
                    //   var divs = $compile("<div class='row-col row-top-div'></div><div class='row-col col-left-div'></div><div class='row-col col-right-div'></div><div class='row-col col-bottom-div'></div>")(scope);
                    //   col.append(divs);
                    //   $timeout(function () {
                    //       $('.col-right-div,.col-left-div').css("height", scope.colHeight + 'px');
                    //   });
                }

            }, function () {
                col.find("[add-control-hover-trigger]").remove();
                col.css({
                    'position': ''
                });
                //if ($(".row-col").length) 
                //     $(".row-col").remove();
            });

            scope.getCellEntityName = function (objcell) {
                var entityforCell = "";
                var ObjGrid = FindParent(objcell, "sfwGridView");
                if (ObjGrid) {
                    entityforCell = getEntityBasedOnControl(ObjGrid);
                }
                else {
                    var Objdialog = FindParent(objcell, "sfwDialogPanel");
                    if (Objdialog) {
                        entityforCell = getEntityBasedOnControl(Objdialog);
                    }
                    else {
                        var Objlist = FindParent(objcell, "sfwListView");
                        if (Objlist) {
                            entityforCell = getEntityBasedOnControl(Objlist);
                        }
                        else {
                            entityforCell = scope.formodel.dictAttributes.sfwEntity;
                        }
                    }
                }

                return entityforCell;
            };

            scope.GetParent = function (itm) {
                var control = FindParent(itm, "sfwGridView", true);
                if (!control) {
                    control = FindParent(itm, "sfwDialogPanel", true);
                    if (!control) {
                        control = FindParent(itm, "sfwListView", true);
                    }
                }
                return control;
            };

            scope.GetEntityNameFromObject = function (control) {
                var strEntity;
                var lst = scope.formodel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
                if (lst && lst.length > 0) {
                    var entityfieldname = GetEntityFieldNameFromControl(lst[0], control);
                    if (entityfieldname) {
                        if (entityfieldname == "InternalErrors" || entityfieldname == "ExternalErrors") {
                            strEntity = "entError";
                        }
                        else {
                            var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formodel.dictAttributes.sfwEntity, entityfieldname);
                            if (objField) {
                                strEntity = objField.Entity;
                            }
                            else {
                                strEntity = '';
                            }
                        }
                    }
                }
                return strEntity;
            };

            scope.CanDropControlFromOneCellToAnother = function () {
                var retVal = true;
                // get parent of drop data to check if parent is grid, dialog panel, listview
                var objParentControl = scope.GetParent(scope.dropdata);
                // get parent of selected control to check if parent is grid, dialog panel, listview
                var selectedControlParent = scope.GetParent(scope.formodel.SelectedControl);
                // sfwScheduler , sfwCalendar,  sfwChart i.e collection type controls can not be dropped inside other parent collection type control i.e grid, dialog panel, listview
                if (scope.formodel.SelectedControl && (scope.formodel.SelectedControl.Name == "sfwScheduler" || scope.formodel.SelectedControl.Name == "sfwCalendar" || scope.formodel.SelectedControl.Name == "sfwChart")) {
                    selectedControlParent = scope.formodel.SelectedControl;
                }
                if (objParentControl != selectedControlParent) {
                    // if drop data collection and selected control collection is same - not supported in s3
                    //var strEntity1 = scope.GetEntityNameFromObject(objParentControl);
                    //var strEntity2 = scope.GetEntityNameFromObject(selectedControlParent);
                    //if (!strEntity1 || !strEntity2 || strEntity1 != strEntity2) {
                    return false;
                    //}
                }
                return retVal;
            }

            scope.CanDropControl = function () {
                var retVal = true;
                var displayEntity = getDisplayedEntity(scope.lstdisplayentities);

                if (displayEntity) {
                    var control = scope.GetParent(scope.dropdata);
                    var selectedControlParent = scope.GetParent(scope.formodel.SelectedControl);
                    if ((control && !selectedControlParent) || (!control && selectedControlParent)) {
                        return false;
                    }
                    else if (control && selectedControlParent) {
                        var strEntity1 = scope.GetEntityNameFromObject(control);
                        var strEntity2 = scope.GetEntityNameFromObject(selectedControlParent);
                        if (!strEntity1 || !strEntity2 || strEntity1 != strEntity2) {
                            return false;
                        }
                    }

                    var strEntity = scope.formodel.dictAttributes.sfwEntity;
                    if (control) {
                        strEntity = scope.GetEntityNameFromObject(control);
                    }

                    if (displayEntity.strDisplayName && displayEntity.strDisplayName.contains(".")) {

                        var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(strEntity, displayEntity.strDisplayName);
                        if (objField) {
                            if (displayEntity.strID !== objField.Entity) {
                                return false;
                            }
                        }
                        else {
                            return false;
                        }
                    }
                    else if (!displayEntity.strParentID && displayEntity.strID !== strEntity) {
                        return false;
                    }
                    else if (displayEntity.strID === strEntity && dragDropDataObject && dragDropDataObject.DataType && (dragDropDataObject.DataType.indexOf(["Collection", "List", "Object", "CDOCollection"]) > -1)) {
                        retVal = false;
                    }
                    else if (displayEntity.strParentID && displayEntity.strParentID !== strEntity) {
                        retVal = false;
                    }
                }

                return retVal;
            }

            scope.isValidField = function (fullpath) {
                var displayEntity = getDisplayedEntity(scope.lstdisplayentities);
                var arrpath = fullpath.split(".");
                var isFound = false;
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                //var entity = scope.formodel.dictAttributes.sfwEntity;
                var entityforcell = scope.getCellEntityName(scope.dropdata);
                var entity = scope.entitytreename;
                var FieldEntity = "";
                if (entity == entityforcell) {
                    for (var i = 0; i < arrpath.length; i++) {
                        isFound = false;
                        var Attributes = [];
                        if (entity != "") {
                            var lst = entityIntellisenseList.filter(function (x) {
                                return x.ID == entity;
                            });
                            if (lst.length > 0) {
                                Attributes = lst[0].Attributes;
                            }

                            if (Attributes.length > 0) {
                                var lstAttr = Attributes.filter(function (x) {
                                    return x.ID == arrpath[i];
                                });
                                if (lstAttr.length > 0) {
                                    FieldEntity = entity;
                                    entity = lstAttr[0].Entity;
                                    isFound = true;
                                } else {
                                    entity = "";
                                }
                            } else {
                                entity = "";
                            }
                        }
                    }

                    if (isFound) {
                        if (FieldEntity != displayEntity.strID) {
                            isFound = false;
                        }
                    }
                }

                return isFound;
            };

            function handleDragOver(e) {

                if (canDropInForm(dragDropData, scope.dropdata)) {
                    if (e && e.preventDefault) {
                        e.preventDefault(); // Necessary. Allows us to drop.
                    }
                    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
                    //if (typeof $(e.srcElement).attr("formcontroldroppable") !== typeof undefined && $(e.srcElement).attr("formcontroldroppable") !== false) {
                    //    $(e.srcElement).attr("style", "border-color:blue;color:blue");
                    //}
                }

                return false;
            }

            function handleDrop(e) {
                var strData = e.dataTransfer.getData("Text");
                if (strData) {
                    if (e) {
                        e.preventDefault();
                    }
                    strData = "";//return false;
                }
                var isValidDrop = false;
                // Stops some browsers from redirecting.
                if (e.stopPropagation) e.stopPropagation();
                var lookupfieldQueryId = "";
                if (strData == "" && lstEntityTreeFieldData != null) {
                    e.preventDefault();
                    var obj = lstEntityTreeFieldData;//JSON.parse(strData);
                    var Id = obj[0];
                    var DisplayName = obj[1];
                    var DataType = obj[2];
                    var data = obj[3];//JSON.parse(obj[3]);
                    var isparentTypeCollection = obj[4];
                    var fieldtype = obj[5];
                    lookupfieldQueryId = obj[6];

                    dragDropData = Id;
                    if (scope.formodel) {
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var MainQuery = GetMainQueryFromFormObject(scope.formodel, entityIntellisenseList);
                        if (!MainQuery && data) {
                            dragDropData = data.Value;
                        }
                    }
                    dragDropDataObject = data;
                    var fullpath = Id;
                    if (DisplayName != "") {
                        fullpath = DisplayName + "." + Id;
                    }
                    if (fieldtype == "Column") {
                        isValidDrop = true;
                    } else {
                        isValidDrop = scope.isValidField(fullpath);
                        isValidDrop = true;
                    }
                    lstEntityTreeFieldData = null;

                }
                else {
                    if (strData == null || strData == "") {
                        isValidDrop = true;
                    }
                    lstEntityTreeFieldData = null;
                    if (e && e.preventDefault) {
                        e.preventDefault();
                    }
                }

                var data = dragDropData;
                var controlName = data;
                if (data && typeof data === "object" && data.Name) {
                    controlName = data.Name;
                }

                if (isValidDrop) {
                    if (scope.dropdata && ["sfwHyperLink", "sfwCascadingDropDownList", "sfwCheckBox", "sfwDropDownList", "sfwMultiSelectDropDownList", "sfwImage", "sfwLabel", "sfwRadioButton", "sfwRadioButtonList", "sfwTextBox", "sfwDateTimePicker", "sfwToolTipButton", "sfwJSONData", "sfwButton", "sfwButtonGroup"].indexOf(controlName) <= -1 && scope.dropdata.Name == "TemplateField") {
                        $SgMessagesService.Message('Message', "Cannot add the control " + controlName + " inside grid");
                    }
                    else if (scope.dropdata.Name == 'sfwButtonGroup' && FindParent(scope.dropdata, "sfwGridView") && ["sfwHyperLink", "sfwCascadingDropDownList", "sfwCheckBox", "sfwDropDownList", "sfwMultiSelectDropDownList", "sfwImage", "sfwLabel", "sfwRadioButton", "sfwRadioButtonList", "sfwTextBox", "sfwDateTimePicker", "sfwToolTipButton", "sfwJSONData", "sfwButton", "sfwButtonGroup"].indexOf(controlName) <= -1) {
                        $SgMessagesService.Message('Message', "Cannot add the control " + controlName + " inside button group present in a grid");
                    }
                    else if (dragDropDataObject) {
                        if (DataType != undefined) {
                            if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.formodel.IsLookupCriteriaEnabled && scope.dropdata && scope.dropdata.Elements.length > 0 && scope.dropdata.Elements[0].Name == "sfwTabContainer") {
                                $SgMessagesService.Message('Message', "Can not add a control outside the criteria tab container");
                            }
                            else if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.formodel.IsPrototypeLookupCriteriaEnabled && scope.dropdata && scope.dropdata.Elements.length > 0 && scope.dropdata.Elements[0].Name == "sfwTabContainer") {
                                $SgMessagesService.Message('Message', "Can not add a control outside the criteria tab container");
                            }
                            else if (scope.CanDropControl()) {
                                var dragDropDataType = DataType;

                                var astrCntrlClass = "";

                                if (isparentTypeCollection == 'true') {
                                    $SgMessagesService.Message('Message', "Can't Drop Collection field.");
                                }
                                else {
                                    if (dragDropDataType == "Object" || dragDropDataType == "List" || dragDropDataType == "Collection" || dragDropDataType == "CDOCollection") {
                                        $SgMessagesService.Message('Message', "You can't drop collection/object.");
                                        dragDropData = null;
                                        dragDropDataType = null;
                                    }
                                    else if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Tooltip") {
                                        astrCntrlClass = "sfwLabel";
                                    }
                                    else if (dragDropDataType == "bool") {
                                        astrCntrlClass = "sfwCheckBox";
                                    }
                                    else if (dragDropDataObject.Value && dragDropDataObject.Value.endsWith("_value")) {
                                        astrCntrlClass = "sfwDropDownList";
                                    }
                                    else if ((dragDropDataObject.Value && dragDropDataObject.Value.endsWith("_description")) || dragDropDataObject.Type == "Expression") {
                                        astrCntrlClass = "sfwLabel";
                                    }
                                    else {
                                        astrCntrlClass = "sfwTextBox";
                                    }
                                }
                                if (astrCntrlClass != undefined && astrCntrlClass != "") {
                                    var sfxControlModel = CreateControl(scope.formodel, scope.dropdata, astrCntrlClass);
                                }

                                if (sfxControlModel) {
                                    var astrFieldName = "";
                                    var rawID = "";
                                    if (dragDropDataObject.Type === "Expression") {
                                        rawID = dragDropDataObject.ID;
                                    }
                                    else {
                                        rawID = dragDropDataObject.Value;
                                    }
                                    var capsNext = false;
                                    for (i = 0; i < rawID.length; i++) {
                                        if (i == 0) {
                                            astrFieldName = rawID[i].toUpperCase();
                                        }
                                        else if (rawID[i] == "_") {
                                            capsNext = true;
                                        }
                                        else {
                                            if (capsNext) {
                                                astrFieldName = astrFieldName + rawID[i].toUpperCase();
                                                capsNext = false;
                                            }
                                            else {
                                                astrFieldName = astrFieldName + rawID[i];
                                            }
                                        }
                                    }
                                    sfxControlModel.dictAttributes.ID = CreateControlID(scope.formodel, astrFieldName, astrCntrlClass, false);
                                    if (scope.formodel.dictAttributes.sfwType == "Lookup" && IsCriteriaField(scope.dropdata)) {
                                        sfxControlModel.dictAttributes.sfwDataField = dragDropData;
                                        sfxControlModel.dictAttributes.sfwQueryID = lookupfieldQueryId;
                                    }
                                    else {
                                        sfxControlModel.dictAttributes.sfwEntityField = fullpath;//GetItemPathForEntityObject(dragDropDataObject);
                                    }

                                    if (sfxControlModel.Name == "sfwTextBox" || sfxControlModel.Name == "sfwLabel") {

                                        if (scope.formodel.dictAttributes.sfwType == "Lookup") {
                                            if (dragDropDataType.toLowerCase() == "datetime" || dragDropDataType.toLowerCase() == "date") {
                                                sfxControlModel.dictAttributes.sfwDataType = "DateTime";
                                            }
                                            else if (dragDropDataType.toLowerCase() == "decimal" || dragDropDataType.toLowerCase().indexOf("int") > -1) {
                                                sfxControlModel.dictAttributes.sfwDataType = "Numeric";
                                            }
                                            else {
                                                sfxControlModel.dictAttributes.sfwDataType = "String";
                                            }
                                        }
                                        if (sfxControlModel.Name == "sfwLabel" && (dragDropDataObject.Value.toLowerCase() == "created_date" || dragDropDataObject.Value.toLowerCase() == "modified_date")) {
                                            sfxControlModel.dictAttributes.sfwDataFormat = "{0:G}";
                                        }
                                        else if (dragDropDataType.toLowerCase() == "datetime" || dragDropDataType.toLowerCase() == "date") {
                                            sfxControlModel.dictAttributes.sfwDataFormat = "{0:d}";
                                        }
                                        else if (dragDropDataType.toLowerCase() == "decimal" && (dragDropDataObject.Value.indexOf("_amt") > -1 ||
                                            dragDropDataObject.Value.indexOf("_amount") > -1)) {
                                            sfxControlModel.dictAttributes.sfwDataFormat = "{0:C}";
                                        }
                                        else if (dragDropDataObject.Value.contains("ssn")) {
                                            sfxControlModel.dictAttributes.sfwDataFormat = "{0:000-##-####}";
                                        }
                                        else if (dragDropDataObject.Value.contains("phone") || dragDropDataObject.Value.contains("fax")) {
                                            sfxControlModel.dictAttributes.sfwDataFormat = "{0:(###)###-####}";
                                        }
                                    }
                                    else if (sfxControlModel.Name == "sfwDropDownList") {
                                        if (endsWith(dragDropDataObject.Value, "_value")) {
                                            var displayEntity = getDisplayedEntity(scope.lstdisplayentities);
                                            var entityname = displayEntity.strID;
                                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                            var strCodeGroup = GetCodeIDByValue(entityname, dragDropDataObject.Value, entityIntellisenseList);
                                            if (!strCodeGroup) {
                                                strCodeGroup = "0";
                                            }
                                            sfxControlModel.dictAttributes.sfwLoadType = "CodeGroup";
                                            // sfxControlModel.dictAttributes.sfwLoadSource = strCodeGroup;
                                            sfxControlModel.placeHolder = strCodeGroup;
                                        }
                                    }
                                }
                                //var lstGrids = [];
                                //FindControlListByName(scope.dropdata, "sfwGridView", lstGrids);
                                //FindControlListByName(scope.dropdata, "sfwChart", lstGrids);
                                //FindCaptionControlList(scope.dropdata, lstGrids)
                                var count = 0;
                                for (var i = 0; i < scope.dropdata.Elements.length; i++) {
                                    var canAddControl = false;
                                    if ($rootScope.lstWebControls && $rootScope.lstWebControls.length > 0) {
                                        var lst = $rootScope.lstWebControls.filter(function (x) {
                                            return x.ControlName == scope.dropdata.Elements[i].Name;
                                        });
                                        lst = JSON.parse(JSON.stringify(lst));
                                        if (lst && lst.length > 0) {
                                            canAddControl = CanAddControlToDropList(lst[0], scope.formodel, scope.dropdata.Elements[i], true);
                                            if (canAddControl) {
                                                count++;
                                            }
                                        }
                                    }
                                }
                                if (scope.formodel && scope.formodel.dictAttributes && scope.formodel.dictAttributes.sfwType == "Tooltip" && (dragDropDataObject.Type == "Object" || ["DateTime", "datetime"].indexOf(dragDropDataObject.DataType) > -1)) {
                                    canAddControl = false;
                                    $SgMessagesService.Message('Message', "drop is not allowed.");
                                    return true;
                                }
                                else if (scope.formodel && scope.formodel.dictAttributes && scope.formodel.dictAttributes.sfwType == "Lookup" && ["Object", "Collection", "CDOCollection", "List"].indexOf(dragDropDataObject.Type) > -1) {
                                    canAddControl = false;
                                    $SgMessagesService.Message('Message', "drop is not allowed.");
                                    return true;
                                }

                                else if (FindParent(scope.dropdata, "sfwDialogPanel") && ["Object", "Collection", "CDOCollection", "List"].indexOf(dragDropDataObject.Type) > -1) {
                                    canAddControl = false;
                                    $SgMessagesService.Message('Message', "drop is not allowed.");
                                    return true;
                                }
                                //else if (sfxControlModel && (count > 0 || ["Object", "Collection", "CDOCollection", "List"].indexOf(dragDropDataObject.Type) > -1)) {
                                //    var newScope = scope.$new(true);
                                //    newScope.model = scope.dropdata;
                                //    newScope.currentControl = sfxControlModel;
                                //    newScope.formmodel = scope.formodel;
                                //    newScope.dragdropdata = dragDropDataObject;
                                //    newScope.fieldName = dragDropDataObject.DisplayName;


                                //    newScope.objectTreeDragDropDialog = $rootScope.showDialog(newScope, "Drag Drop", "Form/views/ObjectTreeDragDropDialog.html", { width: 1000, height: 400 });


                                //}
                                else if (sfxControlModel) {
                                    scope.$apply(function () {
                                        var dropControl = scope.dropdata;
                                        if (scope.dropdata.Name == "TemplateField") {
                                            dropControl = scope.dropdata.Elements.filter(function (x) { return x.Name == "ItemTemplate" })[0];
                                        }
                                        $rootScope.PushItem(sfxControlModel, dropControl.Elements);
                                        SetFormSelectedControl(scope.formodel, sfxControlModel);
                                    });
                                }
                            }
                        }
                    }
                    else if (typeof (dragDropData) == "string") {
                        /* control which are excluding from Lookup/Bind To Query in maintenance Grid only Non editable controls are allowed.*/
                        var excludeControlsFromLookupGrid = ["sfwCascadingDropDownList", "sfwTextBox", "sfwSwitchCheckBox", "sfwRadioButtonList", "sfwRadioButton", "sfwMultiSelectDropDownList", "sfwDropDownList", "sfwCheckBoxList", "sfwCheckBox", "sfwDateTimePicker"];

                        var objGrid = FindParent(scope.dropdata, "sfwGridView");
                        if (data != undefined && data != null && data != "") {
                            if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.formodel.IsLookupCriteriaEnabled && scope.dropdata && scope.dropdata.Elements.length > 0 && scope.dropdata.Elements[0].Name == "sfwTabContainer") {
                                $SgMessagesService.Message('Message', "Can not add a control outside the criteria tab container");
                            }
                            else if (scope.formodel && scope.formodel.dictAttributes.sfwType == "Lookup" && scope.formodel.IsPrototypeLookupCriteriaEnabled && scope.dropdata && scope.dropdata.Elements.length > 0 && scope.dropdata.Elements[0].Name == "sfwTabContainer") {
                                $SgMessagesService.Message('Message', "Can not add a control outside the criteria tab container");
                            }
                            else if (scope.formodel.dictAttributes.sfwType.toLowerCase() == "lookup" && excludeControlsFromLookupGrid.indexOf(dragDropData) > -1 && objGrid) {
                                $SgMessagesService.Message('Message', "Can not add control inside the Grid.");
                            }

                            else if (objGrid && objGrid.dictAttributes.sfwBoundToQuery && objGrid.dictAttributes.sfwBoundToQuery.toLowerCase() == "true" && excludeControlsFromLookupGrid.indexOf(dragDropData) > -1) {
                                $SgMessagesService.Message('Message', "Can not add control inside the Grid.");
                            }
                            else if (objGrid && ["sfwHyperLink", "sfwCascadingDropDownList", "sfwCheckBox", "sfwDropDownList", "sfwMultiSelectDropDownList", "sfwImage", "sfwLabel", "sfwRadioButton", "sfwRadioButtonList", "sfwTextBox", "sfwDateTimePicker", "sfwToolTipButton", "sfwJSONData", "sfwButton", "sfwButtonGroup"].indexOf(controlName) <= -1) {
                                $SgMessagesService.Message('Message', "Can not add a control " + controlName + " inside grid");
                            }
                            else {
                                if (objGrid && scope.dropdata.Name != "sfwLabel" && scope.dropdata.Name != "sfwButton" && scope.dropdata.Name != "sfwLinkButton" && scope.dropdata.Name != "sfwImageButton" && scope.dropdata.Name !== "sfwButtonGroup") {

                                    $rootScope.EditPropertyValue(objGrid.dictAttributes.AllowEditing, objGrid.dictAttributes, "AllowEditing", "True");
                                    $rootScope.EditPropertyValue(objGrid.dictAttributes.sfwTwoWayBinding, objGrid.dictAttributes, "sfwTwoWayBinding", "True");
                                    $rootScope.EditPropertyValue(objGrid.dictAttributes.sfwCommonFilterBox, objGrid.dictAttributes, "sfwCommonFilterBox", "False");
                                    $rootScope.EditPropertyValue(objGrid.dictAttributes.sfwFilterOnKeyPress, objGrid.dictAttributes, "sfwFilterOnKeyPress", "False");
                                }
                                addItemOnDropInForm(data, scope.dropdata, e, scope.formodel);
                            }
                        }

                        //var curscope = getScopeByFileName(scope.formodel.dictAttributes.ID);
                        //if (curscope) {
                        //    curscope.showOtherControl = false;
                        //    curscope.showHTMLControl = false;
                        //}
                    }
                    else {
                        if (data) {
                            var tempObj = {};
                            var aparent = scope.dropdata.ParentVM;
                            var blnissameparent = false;
                            while (aparent) {
                                if (aparent == data) {
                                    blnissameparent = true;
                                    break;
                                }
                                aparent = aparent.ParentVM;
                            }
                            if (!blnissameparent) {
                                if (scope.CanDropControlFromOneCellToAnother()) {
                                    angular.copy(data, tempObj);
                                    $rootScope.UndRedoBulkOp("Start");
                                    scope.$apply(function () {
                                        // new dragged object has to sync with selected form control                                                       
                                        tempObj.isSelected = true;
                                        if (scope.dropdata.Name === "TemplateField") {
                                            if (FindParent(dragDropData, "sfwGridView")) {
                                                var lst = scope.dropdata.Elements.filter(function (x) { return x.Name == "ItemTemplate" });
                                                if (lst && lst.length > 0) {
                                                    $rootScope.PushItem(tempObj, lst[0].Elements);
                                                    tempObj.ParentVM = lst[0];
                                                }
                                                if (data && data.ParentVM) {
                                                    $rootScope.DeleteItem(data, data.ParentVM.Elements);
                                                }
                                            }
                                        }
                                        else if ((dragDropData.Name === "sfwScheduler" && FindParent(scope.dropdata, "sfwDialogPanel")) || dragDropData.Name === "sfwTable") {
                                            $SgMessagesService.Message('Message', "drop is not allowed.");
                                        }
                                        else if (dragDropData.Name === "sfwGridView" && CandropControlIntoCell(dragDropData, scope.dropdata, "sfwListView")) {
                                            $SgMessagesService.Message('Message', "drop is not allowed.");
                                        }
                                        else if (CandropControlIntoCell(dragDropData, scope.dropdata, "sfwDialogPanel")) {
                                            $SgMessagesService.Message('Message', "drop is not allowed.");
                                        }
                                        else if (dragDropData.Name === "sfwTabSheet" && FindParent(scope.dropdata, "sfwTabContainer") && scope.dropdata.Name === "sfwTabSheet") {
                                            if (dragDropData.ParentVM.Name === "Tabs") {
                                                var dragIndex = dragDropData.ParentVM.Elements.indexOf(dragDropData);
                                                var dropIndex = scope.dropdata.ParentVM.Elements.indexOf(scope.dropdata);
                                                if (dragIndex > -1 && dropIndex > -1) {
                                                    $rootScope.UndRedoBulkOp("Start");
                                                    var model = dragDropData.ParentVM.Elements[dragIndex];
                                                    if (dragIndex < dropIndex) {
                                                        //removing
                                                        $rootScope.DeleteItem(dragDropData.ParentVM.Elements[dragIndex], dragDropData.ParentVM.Elements);                                                       //Adding
                                                        //Adding
                                                        $rootScope.InsertItem(model, dragDropData.ParentVM.Elements, dropIndex);
                                                    }
                                                    else {
                                                        //removing
                                                        $rootScope.DeleteItem(dragDropData.ParentVM.Elements[dragIndex], dragDropData.ParentVM.Elements);
                                                        //Adding
                                                        $rootScope.InsertItem(model, dragDropData.ParentVM.Elements, dropIndex);
                                                    }
                                                    $rootScope.UndRedoBulkOp("End");
                                                }
                                            }

                                        }
                                        else {
                                            scope.$evalAsync(function () {
                                                $rootScope.PushItem(tempObj, scope.dropdata.Elements);
                                                tempObj.ParentVM = scope.dropdata;
                                                SetFormSelectedControl(scope.formodel, tempObj, null);
                                                if (data && data.ParentVM) {
                                                    $rootScope.DeleteItem(data, data.ParentVM.Elements);
                                                }
                                            });
                                        }
                                    });
                                    $rootScope.UndRedoBulkOp("End");
                                }
                            }
                            else {
                                toastr.error("Same item cannot be dropped as child.");
                            }

                        }
                    }
                    // $(e.srcElement).removeAttr("style");
                    dragDropData = null;
                    dragDropDataObject = null;
                }
            }

            function handleDragLeave(e) {
                //if (typeof $(e.srcElement).attr("formcontroldroppable") !== typeof undefined && $(e.srcElement).attr("formcontroldroppable") !== false) {
                //    $(e.srcElement).removeAttr("style");
                //}
            }

            function canDropInForm(dragdata, dropdata) {

                if (FindParent(dragdata, "sfwGridView") && (dropdata.Name === "sfwColumn")) {
                    return false;
                }
                else if (!FindParent(dragdata, "sfwGridView") && dropdata.Name === "TemplateField") {
                    return false;
                }
                else if (FindParent(dragdata, "sfwGridView") !== FindParent(dropdata, "sfwGridView")) {
                    return false;
                }
                //removed by neha to fix bug id 12453 - no control other than label is getting dropped inside cell due to this condition. In drop function we are already validating if control is allowed inside grid or not so no need to check before drop
                //else if (scope.formodel.dictAttributes.sfwType.toLowerCase() == "lookup" && "sfwLabel" !== dragdata && isGridPresentInsidePanel(dropdata)) {
                //    return false;
                //}
                else if (scope.formodel.dictAttributes.sfwType.toLowerCase() == "lookup" && "sfwButton" !== dragdata && !isGridPresentInsidePanel(dropdata)) {
                    var objpanel = FindParent(dropdata, "sfwPanel");
                    if (objpanel && dropdata.Name == "sfwColumn" && objpanel.dictAttributes.ID == "pnlResult") {
                        return false;
                    }
                }
                return true;
            }

            function CandropControlIntoCell(dragdata, dropdata, ParentName) {
                var retValue = false;
                var dropDataParent = FindParent(scope.dropdata, ParentName);
                var dragDataParent = FindParent(dragDropData, ParentName);
                if (!dragDataParent && !dropDataParent) {
                    retValue = false;
                }
                else {
                    if (dragDataParent && dropDataParent && dragDataParent.dictAttributes && dropDataParent.dictAttributes) {
                        if (dragDataParent.dictAttributes.ID != dropDataParent.dictAttributes.ID) {
                            retValue = true;
                        }
                    }

                    else if (!dragDataParent || !dropDataParent) {
                        retValue = true;
                    }
                }

                return retValue;
            }

            var RepeaterControldialog;
            scope.objRepeaterControl;


            function addItemOnDropInForm(dragdata, dropdata, event, formodel) {
                if (dragdata) {
                    var strControlName = dragdata;
                    if (formodel.dictAttributes.sfwType === "Report") {
                        var strObjectField = "";
                        if (draggingDataFromLeft) {
                            strControlName = "sfwTextBox";
                            strObjectField = dragdata;
                        }
                        objControl = AddControlToCell(strControlName, undefined, dropdata, formodel);
                        if (objControl && strObjectField) {
                            scope.$evalAsync(function () {
                                objControl.dictAttributes.sfwObjectField = strObjectField;
                            });
                            objControl.dictAttributes.ID = strObjectField;
                        }
                    }
                    else {
                        if (strControlName == "sfwButton") {
                            var newScope = scope.$new(true);
                            newScope.item = dropdata;
                            newScope.formodel = scope.formodel;
                            var objListView = FindParent(dropdata, "sfwListView");
                            newScope.ParentModel = objListView;
                            if (objListView) {
                                newScope.ParentModel = objListView;
                            }



                            newScope.CreateButtonDialog = $rootScope.showDialog(newScope, "Button Details", "Form/views/CreateButtonWizard/CreateButtonControl.html", { width: 660, height: 550 });
                        }
                        else if (strControlName == "sfwListView") {
                            var strID = CreateControlID(scope.formodel, "RepeaterViewPanel", "sfwListView");
                            var prefix = "swc";
                            /*scope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
                            scope.ParentEntityName = formodel.dictAttributes.sfwEntity;
                            scope.objRepeaterControl.selectedobjecttreefield;
                            scope.objRepeaterControl.lstselectedobjecttreefields = [];
                            RepeaterControldialog = ngDialog.open({
                                template: 'RepeaterControlTemplate',
                                scope: scope,
                                closeByDocument: false
                            });*/


                            var newRepeaterControlScope = scope.$new();
                            newRepeaterControlScope.ValidateRepeaterProp = function () {
                                var isValid = false;
                                newRepeaterControlScope.ErrorMessageForDisplay = "";
                                if (!newRepeaterControlScope.objRepeaterControl.dictAttributes.ID) {
                                    isValid = true;
                                    newRepeaterControlScope.ErrorMessageForDisplay = "Error : Please Enter ID.";
                                } else if (newRepeaterControlScope.objRepeaterControl.dictAttributes.ID && !isValidIdentifier(newRepeaterControlScope.objRepeaterControl.dictAttributes.ID, false, false)) {
                                    isValid = true;
                                    newRepeaterControlScope.ErrorMessageForDisplay = "Error : Invalid ID.";
                                } else if (newRepeaterControlScope.objRepeaterControl.dictAttributes.ID) {
                                    var lstIds = [];
                                    CheckforDuplicateID(scope.formodel, newRepeaterControlScope.objRepeaterControl.dictAttributes.ID, lstIds);
                                    if (lstIds.length > 0) {
                                        isValid = true;
                                        newRepeaterControlScope.ErrorMessageForDisplay = "Error : Duplicate ID.";
                                    }
                                }
                                if (!newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield) {
                                    isValid = true;
                                    newRepeaterControlScope.ErrorMessageForDisplay = "Error : Please Select a valid collection.";
                                }
                                else if (newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield && (newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield.DataType != "Collection" && newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield.DataType != "CDOCollection" && newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield.DataType != "List")) {
                                    newRepeaterControlScope.ErrorMessageForDisplay = "Error : Please Select a valid collection.";
                                    isValid = true;
                                }
                                return isValid;
                            };



                            newRepeaterControlScope.onRepeaterControlOkClick = function () {
                                if (newRepeaterControlScope.objRepeaterControl) {
                                    newRepeaterControlScope.objRepeaterControl.dictAttributes.sfwSelection = "Many";
                                    newRepeaterControlScope.objRepeaterControl.dictAttributes.sfwCaption = "List View";
                                    newRepeaterControlScope.objRepeaterControl.dictAttributes.AllowPaging = "True";
                                    newRepeaterControlScope.objRepeaterControl.dictAttributes.PageSize = "1";

                                    var selectedField = newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield;

                                    if (selectedField) {
                                        var displayEntity = getDisplayedEntity(newRepeaterControlScope.objRepeaterControl.LstDisplayedEntities);
                                        var displayName = displayEntity.strDisplayName;
                                        fieldName = selectedField.ID;
                                        if (displayName != "") {
                                            fieldName = displayName + "." + selectedField.ID;
                                        }
                                        var entitycollname = fieldName; //GetItemPathForEntityObject(selectedField);
                                        newRepeaterControlScope.objRepeaterControl.dictAttributes.sfwEntityField = entitycollname;
                                        var parentenetityname = selectedField.Entity;
                                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                        newRepeaterControlScope.objRepeaterControl.dictAttributes.sfwDataKeyNames = GetTableKeyFields(parentenetityname, entityIntellisenseList);
                                    }

                                    var prefix = "swc";

                                    var objListTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
                                    objListTableModel.ParentVM = newRepeaterControlScope.objRepeaterControl;
                                    var strCtrlId = CreateControlID(newRepeaterControlScope.formodel, "NewPage", "sfwTable");
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
                                    newRepeaterControlScope.objRepeaterControl.Elements.push(objListTableModel);
                                    newRepeaterControlScope.objRepeaterControl.initialvisibilty = true;
                                    newRepeaterControlScope.objRepeaterControl.isLoaded = true;
                                    $rootScope.PushItem(newRepeaterControlScope.objRepeaterControl, newRepeaterControlScope.dropdata.Elements);
                                }
                                newRepeaterControlScope.onRepeaterControlCancelClick();
                            };

                            newRepeaterControlScope.onRepeaterControlCancelClick = function () {

                                newRepeaterControlScope.RepeaterControldialog.close();
                            };


                            newRepeaterControlScope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
                            newRepeaterControlScope.ParentEntityName = formodel.dictAttributes.sfwEntity;
                            newRepeaterControlScope.objRepeaterControl.selectedobjecttreefield;
                            newRepeaterControlScope.objRepeaterControl.lstselectedobjecttreefields = [];
                            newRepeaterControlScope.dropdata = scope.dropdata;
                            newRepeaterControlScope.formodel = scope.formodel;

                            newRepeaterControlScope.RepeaterControldialog = $rootScope.showDialog(newRepeaterControlScope, "Repeater Control", "Form/views/RepeaterControlTemplate.html", { width: 500, height: 600 });


                        }
                        else if (strControlName == "sfwGridView") {
                            if (FindParent(dropdata, "sfwDialogPanel")) {
                                $SgMessagesService.Message('Message', "Cannot drop a grid inside dialog panel.");
                            }
                            else {
                                var newScope = scope.$new();
                                newScope.formobject = formodel;
                                newScope.model = undefined;
                                newScope.dropdata = scope.dropdata;
                                newScope.IsAddNewGrid = true;
                                newScope.IsAddFromToolBox = true;
                                if (strControlName == "sfwGridView") {
                                    if (FindParent(dropdata, "sfwListView")) {
                                        newScope.formobject.SelectedControl.IsGridChildOfListView = true;

                                        //Idealy dropdata and newScope.formobject.SelectedControl should be same, but in some cases it's and issue.
                                        //To avoid that issue for now setting property "IsGridChildOfListView" for dropdata, without changing existing code.
                                        dropdata.IsGridChildOfListView = true;
                                    }
                                }
                                newScope.BindToQueryDialog = $rootScope.showDialog(newScope, "Bind to Query", "Form/views/BindToQuery.html", { width: 600, height: 250 });
                            }
                        }
                        else {
                            AddControlToCell(strControlName, undefined, dropdata, formodel);
                        }
                    }
                }
            }


            function AddControlToCell(cntrlName, sfxControlModel, cellVM, formodel) {

                if (!sfxControlModel) {
                    sfxControlModel = CreateControl(formodel, cellVM, cntrlName);
                }

                if (sfxControlModel != null && sfxControlModel.Name != "udc") {
                    scope.$apply(function () {
                        function iGetMainQuery(itm) {
                            if (!MainQuery) {
                                var strQuery = itm.dictAttributes.sfwQueryRef;
                                if (itm.Name == "query" && !scope.IsSubQuery(strQuery)) {
                                    MainQuery = itm;
                                }
                            }
                        }
                        if (sfxControlModel.Name == "sfwDialogPanel" || sfxControlModel.Name == "sfwPanel") {
                            sfxControlModel.initialvisibilty = true;
                            sfxControlModel.isLoaded = true;
                        }
                        if (sfxControlModel.Name != "udc") {
                            if (cellVM.Name == "TemplateField") {
                                var lst = cellVM.Elements.filter(function (x) { return x.Name == "ItemTemplate" });
                                if (lst && lst.length > 0) {
                                    $rootScope.PushItem(sfxControlModel, lst[0].Elements);
                                    sfxControlModel.ParentVM = lst[0];
                                }
                            }
                            else {
                                $rootScope.PushItem(sfxControlModel, cellVM.Elements);
                            }
                        }
                        SetFormSelectedControl(formodel, sfxControlModel);

                        if (formodel && formodel.dictAttributes && formodel.dictAttributes.sfwType == "Lookup") {
                            if (sfxControlModel.Name != "sfwLabel" && sfxControlModel.Name != "sfwLiteral"
                                && sfxControlModel.Name != "RequiredFieldValidator" && sfxControlModel.Name != "CompareValidator"
                                && sfxControlModel.Name != "hr" && sfxControlModel.Name != "br"
                                && sfxControlModel.Name != "sfwUserDefaults" && sfxControlModel.Name != "CompareValidator") {
                                var lst = formodel.Elements.filter(function (itm) { return itm.Name == "initialload"; });
                                if (lst && lst.length > 0) {
                                    var initialLoad = lst[0];
                                    var MainQuery;

                                    angular.forEach(initialLoad.Elements, iGetMainQuery);
                                    if (MainQuery) {
                                        sfxControlModel.dictAttributes.sfwQueryID = MainQuery.dictAttributes.ID;
                                    }
                                }
                            }

                        }

                        var filescope = getCurrentFileScope();
                        if (filescope && filescope.selectControl) {
                            filescope.selectControl(sfxControlModel, event);
                        }
                    });
                    //this.ObjVM.DesignVM.CheckAndUpdateSelectedControlStatus(this.MirrorElements[this.MirrorElements.Count - 1] as SfxControlVM, false);
                    //this.PopulateObjectID(this.ObjVM.Model, sfxControlModel);
                }


                //#region Add User Control
                if (sfxControlModel != undefined && sfxControlModel.Name == "udc") {

                    var newScope = scope.$new();
                    newScope.dropdata = scope.dropdata;
                    newScope.formodel = scope.formodel;
                    newScope.objSetUCProp = { StrId: sfxControlModel.dictAttributes.ID, StrName: '', StrEntityField: '', StrResource: '' };
                    newScope.objSetUCProp.IsAddedFromObjectTree = false;
                    newScope.onUserControlOkClick = function () {
                        sfxControlModel.dictAttributes.ID = newScope.objSetUCProp.StrId;
                        sfxControlModel.dictAttributes.Name = newScope.objSetUCProp.StrName;
                        if ((scope.formodel.dictAttributes.sfwEntity != undefined && scope.formodel.dictAttributes.sfwEntity != "") && (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "")) {
                            if (newScope.objSetUCProp.StrEntityField.match("^" + scope.formodel.dictAttributes.sfwEntity)) {
                                sfxControlModel.dictAttributes.sfwEntityField = scope.formodel.dictAttributes.sfwEntity + "." + newScope.objSetUCProp.StrEntityField;
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
                        SetFormSelectedControl(scope.formodel, sfxControlModel);
                        newScope.onUserControlCancelClick();

                        //#region Receive User Control Table Model

                        //#endregion


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
                            CheckforDuplicateID(scope.formodel, newScope.objSetUCProp.StrId, lstIds);
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
                    newScope.receiveUcMainTable = function (data, strUcId) {
                        var table;
                        for (var i = 0; i < scope.formodel.Elements.length; i++) {
                            if (scope.formodel.Elements[i].Name == "sfwTable") {
                                table = scope.formodel.Elements[i];
                                break;
                            }
                        }
                        var ucControl = FindControlByID(table, strUcId);
                        if (ucControl) {
                            ucControl.UcChild = data;
                        }
                    };
                }

                //#endregion


                return sfxControlModel;
            }

            scope.IsSubQuery = function (strQuery) {
                function iIssubselectquery(Query) {
                    if (!retValue) {
                        if (Query.ID == strQueryName && Query.QueryType.toLowerCase() == "subselectquery") {
                            retValue = true;
                        }
                    }
                }
                var retValue = false;
                if (strQuery != "" && strQuery != undefined) {
                    var strCDOName = strQuery.substring(0, strQuery.indexOf("."));
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var lstObj = entityIntellisenseList.filter(function (x) {
                        return x.ID == strCDOName;
                    });
                    if (lstObj && lstObj.length > 0) {

                        var strQueryName = strQuery.substring(strQuery.indexOf(".") + 1);

                        angular.forEach(lstObj[0].Queries, iIssubselectquery);
                    }
                }

                return retValue;
            };


            scope.receiveUcMainTable = function (data, strUcId) {

                var table;
                for (var i = 0; i < scope.formodel.Elements.length; i++) {
                    if (scope.formodel.Elements[i].Name == "sfwTable") {
                        table = scope.formodel.Elements[i];
                        break;
                    }
                }

                var ucControl = FindControlByID(table, strUcId);
                if (ucControl) {
                    ucControl.UcChild = data;
                }
            };
        }
    };
}]);

app.directive('commonpropertiesdirective', ["$compile", "$rootScope", "ngDialog", "$filter", "$EntityIntellisenseFactory", "$NavigateToFileService", "hubcontext", "$ValidationService", "CONSTANTS", "$ValidateBaseModelStructure", "$Entityintellisenseservice", "$GetEntityFieldObjectService", "$SgMessagesService", "$GetGridEntity", function ($compile, $rootScope, ngDialog, $filter, $EntityIntellisenseFactory, $NavigateToFileService, hubcontext, $ValidationService, CONST, $ValidateBaseModelStructure, $Entityintellisenseservice, $GetEntityFieldObjectService, $SgMessagesService, $GetGridEntity) {

    return {
        restrict: "E",
        replace: true,
        scope: {
            model: '=',
            parent: '=',
            formobject: '=',
            currentpanel: '=',
            isformlink: '=',
            lstloadedentitytrees: '=',
            lstloadedentitycolumnstree: '=',
            entityTreeName: '=',
            objgridboundedquery: '='
        },
        link: function (scope, element, attributes) {
            //#region Init Section
            var unwatch = scope.$watch('entityTreeName', function (newVal, oldVal) {
                scope.entityName = scope.entityTreeName;
                scope.setEntityName();
            });

            scope.InitByControl = function () {
                scope.ResetAdvanceProperties();
                scope.Title = "Object Method";

                if (scope.formobject && scope.formobject.SelectedControl && scope.formobject.SelectedControl.dictAttributes.sfwExecuteMethodType) {
                    if (scope.formobject.SelectedControl.dictAttributes.sfwExecuteMethodType == "ObjectMethod") {
                        scope.Title = "Object Method";
                    }
                    else if (scope.formobject.SelectedControl.dictAttributes.sfwExecuteMethodType == "XmlMethod") {
                        scope.Title = "Xml Method";
                    }
                    else if (scope.formobject.SelectedControl.dictAttributes.sfwExecuteMethodType == "Rule") {
                        scope.Title = "Rule";
                    }
                }
                //scope.entityName = "";
                //if (scope.formobject && scope.formobject.dictAttributes && scope.formobject.dictAttributes.ID) {
                //    var curscope = getScopeByFileName(scope.formobject.dictAttributes.ID);

                //    if (scope.IsCorrespondence) {
                //        scope.entityName = scope.formobject.dictAttributes.sfwEntity;
                //    }
                //    else {
                //        if (curscope && curscope.entityTreeName) {
                //            scope.entityName = curscope.entityTreeName;
                //        }
                //    }
                //}

                if (scope.model && scope.model.Name == "udc") {
                    if (!scope.model.dictAttributes.Visible) {
                        scope.model.dictAttributes.Visible = "True";
                    }
                }

                scope.lstHardErrorData = undefined; // reset hard error data when control change
                scope.$evalAsync(function () {
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
                            if (lstFilterButtons.some(function (itm) { return !itm.dictAttributes.sfwNavigationParameter || (itm.dictAttributes.sfwNavigationParameter && itm.dictAttributes.sfwNavigationParameter.contains(scope.model.dictAttributes.ID)) })) {
                                scope.isFilterCriteria = true;
                            }
                            else {
                                var lstButtons = getDescendents(scope.formobject, "sfwButton");
                                var lstFilterButtons = lstButtons.filter(function (itm) { return itm.sfwMethodName === "btnGridSearch_Click" });
                                if (lstFilterButtons.some(function (itm) { return itm.dictAttributes.sfwNavigationParameter && itm.dictAttributes.sfwNavigationParameter.contains(scope.model.dictAttributes.ID) })) {
                                    scope.isFilterCriteria = true;
                                }
                            }
                        }
                    }
                    if (scope.formobject && scope.formobject.dictAttributes && scope.formobject.dictAttributes.ID.startsWith("wfp")) {
                        scope.IsSearchCriteriaSelected = false;
                    }

                });
                scope.model.IsChildOfGrid = false;
                var objGridView = FindParent(scope.model, "sfwGridView");
                if (objGridView) {
                    scope.objGridView = objGridView;
                    scope.model.IsChildOfGrid = true;
                    scope.setEntityName();
                }
                scope.RelatedControls = [];
                scope.RelatedControls.push("");
                scope.lstRelatedGrid = [];
                scope.lstMethodType = [];
                scope.lstRelatedGrid.push("");
                scope.lstRelatedDialog = [];
                scope.lstServerMethod = [];
                scope.lstBooleanValues = [];
                scope.lstExecuteAfterSuccessButton = [];
                scope.lstCascadingDropDwonParent = [];
                scope.lstCascadingDropDwonParent.push("");
                scope.lstMethodType.push("ObjectMethod");
                scope.lstMethodType.push("XmlMethod");
                scope.lstMethodType.push("Rule");


                scope.LoadWebControlProperties();

                if (scope.isformlink == undefined || !scope.isformlink)
                    scope.LoadControlToValidate();

                scope.findCascadingParentControl();
                if (scope.currentpanel && scope.currentpanel.Elements.length > 0) {
                    scope.CurrentTable = scope.currentpanel.Elements[0];//FindParent(scope.model, "sfwTable");
                }

                if (scope.isformlink == undefined || !scope.isformlink)
                    scope.LoadControlToCompare();

                scope.LoadRelatedControlTextbox();
                if (scope.model) {
                    if (scope.model.Name == "sfwImageButton") {
                        scope.RelatedControls = PopulateAssociatedControlsForRetriveButton(scope.model);
                    }
                    else {
                        scope.RelatedControls = PopulateAssociatedControls(scope.model);
                    }

                    if (scope.model.IsChildOfGrid) {
                        var currentGridTable = FindParent(scope.model, "sfwTable");
                        if (currentGridTable) {
                            FindControlListByName(currentGridTable, "sfwButton", scope.lstRelatedGrid);
                            if (scope.lstRelatedGrid.length > 0) {
                                var lstRelatedButton = [];
                                angular.copy(scope.lstRelatedGrid, lstRelatedButton);
                                scope.lstRelatedGrid = [];
                                lstRelatedButton.filter(function (x) {
                                    if (x.dictAttributes && x.dictAttributes.ID) {
                                        scope.lstRelatedGrid.push(x.dictAttributes.ID);
                                    }
                                });
                            }
                        }
                        scope.lstRelatedGrid.splice(0, 0, '');

                    }
                    else if ((scope.model.dictAttributes.sfwMethodName != undefined && scope.model.dictAttributes.sfwMethodName != "") && scope.model.dictAttributes.sfwMethodName == "btnResetFilterGrid_Click") {
                        PopulateRelatedGrid(scope.formobject, scope.lstRelatedGrid, true);
                    }
                    else {
                        PopulateRelatedGrid(scope.formobject, scope.lstRelatedGrid, false);

                    }
                }

                scope.InitOnDblClick();
                scope.PopulateSelection();
                if (scope.model && (scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwChart")) {
                    scope.PopulateBooleanValues();
                }
                if (scope.model && scope.model.Name == "sfwWizardStep") {
                    scope.PopulateRelatedButton();
                }

                if (scope.model && (scope.model.Name == "sfwButton" || scope.model.Name == "sfwLinkButton" || scope.model.Name == "sfwImageButton")) {
                    scope.MessageIDText = "Message ID:";
                    scope.LoadButtonMethodDescription();
                    scope.PopulateOnClientClick();
                    scope.PopulateSecurityLevel();
                    scope.PopulateExecuteAfterSuccessButton();

                    if (scope.model.dictAttributes.sfwMethodName == "btnNew_Click") {
                        scope.PoulateRelatedControlForNew();
                    }

                    if (scope.model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click"
                        || scope.model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click") {
                        scope.lstRelatedDialog = PopulateRelatedDialogList(scope.MainTable, scope.model);
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
                    scope.lstEntityFields = [];
                    if (scope.model.dictAttributes.sfwMethodName == "btnOpen_Click") {
                        scope.PopulateEntityFieldsForOpenButton();
                    }
                }

                if (scope.model && scope.model.Name == "sfwImageButton") {
                    scope.PopulateTextRelatedControl();
                    scope.lstSelectedIndex = [];
                    scope.MessageIDText = "Lookup Messages :";
                    scope.OnActiveFormValueChange();
                    scope.PopulateImageAlign();
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

                if (scope.model.Name == "TemplateField") {
                    scope.lstSortExpressions = [];
                    scope.lstSortExpressions.push("");
                    var objGridView = FindParent(scope.model, "sfwGridView");
                    if (objGridView) {
                        PopulateSortExpressions(objGridView, scope.lstSortExpressions);
                    }
                    scope.model.HeaderTemplateLabel = undefined;
                    for (var i = 0; i < scope.model.Elements.length; i++) {
                        if (scope.model.Elements[i].Name == "HeaderTemplate") {
                            scope.model.HeaderTemplateLabel = scope.model.Elements[i].Elements[0];
                        }
                    }
                    if (scope.model.HeaderTemplateLabel == undefined) {
                        var HeaderTemplate = {
                            dictAttributes: {}, Elements: [{
                                dictAttributes: {
                                    sfwEntityField: ""
                                }, Elements: [], Children: [], Name: "sfwLabel", Value: "", prefix: "swc"
                            }], Children: [], Name: "HeaderTemplate", Value: "", prefix: "asp"
                        };
                        scope.model.HeaderTemplateLabel = HeaderTemplate.Elements[0];
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
                    if (scope.model.HeaderTemplateLabel && scope.model.HeaderTemplateLabel.dictAttributes.sfwEntityField) {
                        var HeaderTemplate = {
                            dictAttributes: {}, Elements: [scope.model.HeaderTemplateLabel], Name: "HeaderTemplate", Value: "", prefix: "asp"
                        };

                        scope.model.Elements.push(HeaderTemplate);

                    }

                };
                if (scope.model.Name == "sfwCommandButton") {
                    scope.PopulateOnClientClick();
                    scope.PopulateSecurityLevel();
                    scope.PopulateOnCommandName();
                }

                scope.IsMultiActiveForm = false;
                if (!scope.model.SelectActiveFormType) {
                    scope.model.SelectActiveFormType = 'Single';
                }
                else {
                    if (scope.model.SelectActiveFormType === 'Multiple') {
                        scope.IsMultiActiveForm = true;
                    }
                }
                if (scope.model.dictAttributes.sfwActiveForm != undefined && scope.model.dictAttributes.sfwActiveForm != "") {
                    if (scope.model.dictAttributes.sfwActiveForm.contains("=")) {
                        scope.model.SelectActiveFormType = 'Multiple';
                        scope.IsMultiActiveForm = true;
                    }
                }

                //#region ActiveFormtype

                scope.setActiveForm();


                if (scope.model.Name == "sfwGridView") {
                    scope.lstParentGrid = [];
                    PopulateGridID(scope.formobject, scope.model.dictAttributes.ID, scope.lstParentGrid);
                    scope.lstParentGrid.splice(0, 0, "");
                }
                //if (scope.model.dictAttributes.sfwMethodName && scope.model.dictAttributes.sfwMethodName == "btnNew_Click") {
                //    scope.lstGrid = [];
                //    PopulateGridIDForNewButton(scope.formobject, scope.lstGrid);
                //    scope.lstGrid.splice(0, 0, "");
                //}

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

                if (scope.model.Name == "sfwChart") {
                    for (var i = 0; i < scope.model.Elements.length; i++) {
                        if (scope.model.Elements[i].Name == "ChartAreas") {
                            scope.chartArea = scope.model.Elements[i].Elements[0];
                            if (scope.chartArea && scope.chartArea.Elements.length > 0) {
                                scope.Area3D = scope.chartArea.Elements[0];
                            }
                        }
                    }
                }

                if (scope.model.Name == "sfwRuleViewer") {
                    scope.PopulateRulesBasedOnEntity(scope.model.dictAttributes.sfwEntityName);
                }

                scope.ShowPropertyBasedOnCondition();
                if (scope.model && scope.model.Name == "sfwDateTimePicker") scope.LoadDataFormatsForDateTimePicker();

                if (scope.model && (scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwCalendar" || scope.model.Name == "sfwScheduler")) {
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

                if (scope.model && scope.model.dictAttributes.sfwRetrievalQuery) {
                    scope.RetrievalQueryInputChange(true);
                }

                scope.LoadClientVisibilitySource();

                scope.CheckForFilterGridControl();
                scope.$evalAsync(function () {
                    scope.setDefaultCodeGroup();
                });

                if (scope.model && scope.model.Name === "sfwLabel" && scope.model.ParentVM) {
                    if (scope.model.ParentVM.Name === "HeaderTemplate" || scope.model.ParentVM.Name === "FooterTemplate") {
                        scope.model.IsHeaderOrFooterField = true;
                    }
                }
            };

            scope.setActiveForm = function () {
                scope.ActiveFormType = "Lookup,Maintenance,Wizard,FormLinkLookup,FormLinkMaintenance,FormLinkWizard";
                if (scope.model) {
                    if (scope.model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") {
                        scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                    }
                    else if (scope.model.dictAttributes.sfwMethodName == "btnOpenLookup_Click") {
                        scope.ActiveFormType = "Lookup,FormLinkLookup,Maintenance,FormLinkMaintenance";
                    }
                    else if ((scope.model.dictAttributes.sfwMethodName == "btnNew_Click") ||
                        (scope.model.dictAttributes.sfwMethodName == "btnOpen_Click")) {

                        var lblRelatedControlIsScheduler = false;
                        if (scope.model.dictAttributes.sfwRelatedControl) {
                            var lobjModel = FindControlByID(scope.formobject, scope.model.dictAttributes.sfwRelatedControl);
                            if (lobjModel && lobjModel.Name === "sfwScheduler") {
                                lblRelatedControlIsScheduler = true;
                            }
                        }
                        if (lblRelatedControlIsScheduler) {
                            scope.ActiveFormType = "Maintenance,FormLinkMaintenance";
                        }
                        else {
                            scope.ActiveFormType = "Wizard,Maintenance,FormLinkMaintenance,FormLinkWizard";
                        }
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
                }
            }
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
                    scope.validateEmptyCodeId(prop);
                }
                else if (scope.model && scope.model.Name == "sfwCheckBoxList" && scope.model.CollectionFieldEntity) {
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var strCodeGroup = "";
                    if (scope.model.dictAttributes.sfwCheckBoxField) {
                        strCodeGroup = GetCodeID(scope.model.CollectionFieldEntity, scope.model.dictAttributes.sfwCheckBoxField, entityIntellisenseList);
                    }
                    if (strCodeGroup) {
                        scope.model.placeHolder = strCodeGroup;
                    }
                    else {
                        scope.model.placeHolder = "";
                    }
                    scope.validateEmptyCodeId("sfwCheckBoxField");
                }

            };

            scope.validateEmptyCodeId = function (property) {
                if (!scope.model.errors) {
                    scope.model.errors = {};
                }
                if (scope.model.dictAttributes[property] && scope.model.errors && !scope.model.errors[property]) {
                    if (scope.model.dictAttributes.sfwLoadType == "CodeGroup" && scope.model.placeHolder && scope.model.errors.invalid_code_group == CONST.VALIDATION.EMPTY_FIELD) {
                        $ValidationService.checkValidListValue([], scope.model, scope.model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, undefined, true);
                    } else if (scope.model.errors && scope.model.errors.invalid_code_group != CONST.VALIDATION.CODE_GROUP_NOT_EXISTS) {
                        $ValidationService.checkValidListValue([scope.model.dictAttributes.sfwLoadSource], scope.model, scope.model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, undefined);
                    }
                } else if (scope.model.errors && scope.model.errors.invalid_code_group != CONST.VALIDATION.CODE_GROUP_NOT_EXISTS) {
                    $ValidationService.checkValidListValue([scope.model.dictAttributes.sfwLoadSource], scope.model, scope.model.dictAttributes.sfwLoadSource, "sfwLoadSource", "invalid_code_group", CONST.VALIDATION.CODE_GROUP_NOT_EXISTS, undefined);
                }
            };
            scope.Init = function () {
                scope.IsLookup = scope.formobject.dictAttributes.sfwType == "Lookup" ? true : false;
                scope.IsReport = scope.formobject.dictAttributes.sfwType == "Report" ? true : false;
                scope.IsCorrespondence = scope.formobject.dictAttributes.sfwType == "Correspondence" ? true : false;
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
                scope.MainTable = undefined;
                var lstTable;
                if (scope.isformlink != undefined && scope.isformlink == true) {
                    scope.MainTable = scope.formobject.Children;
                }
                else {
                    lstTable = scope.formobject.Elements.filter(function (item) {
                        return item.Name == "sfwTable";
                    });
                    if (lstTable && lstTable.length > 0) {
                        scope.MainTable = lstTable[0];
                    }
                }
                scope.LoadDataType();
                scope.LoadType();
                scope.LoadAxisFormatsForChart();



            };

            scope.ResetAdvanceProperties = function () {
                scope.SelectedTab = "General";
                $(element).find("[header-properties-row][category!='Accessibility'] td").addClass("prop-close");
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

            //#region Init Common Template Methods

            scope.InitOnDblClick = function () {
                if (scope.model.Name == "sfwListPicker" || scope.model.Name == "sfwSourceList" || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name == "sfwMultiSelectDropDownList" || scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwListBox" || scope.model.Name == "sfwCheckBoxList") {
                    scope.PopulateLoadTyeps();
                    scope.InitializeDropDownItems();
                }
            };

            scope.PopulateLoadTyeps = function () {
                scope.lstLoadTypes = [];
                scope.lstLoadTypes.push("CodeGroup");
                scope.lstLoadTypes.push("Query");
                if (!scope.IsLookup || (scope.model && (scope.model.Name != "sfwDropDownList" && scope.model.Name != "sfwMultiSelectDropDownList"))) {
                    if (!scope.IsReport) {
                        scope.lstLoadTypes.push("Method");
                        var objListView = FindParent(scope.model, "sfwListView");
                        if (scope.model && (scope.model.IsChildOfGrid || objListView)) {
                            scope.lstLoadTypes.push("ChildMethod");
                        }
                    }

                }
                scope.lstLoadTypes.push("ServerMethod");
                if (scope.model.Name != "sfwCascadingDropDownList" || (scope.model.Name == "sfwCascadingDropDownList" && !scope.model.dictAttributes.sfwParentControl)) {
                    scope.lstLoadTypes.push("Items");
                }
            };

            scope.GetCodeValueFields = function () {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lst = entityIntellisenseList.filter(function (x) {
                    return x.ID == "entCodeValue";
                });
                function iAddInlstCodeValues(objColumn) {
                    var strColumn = objColumn.ID;
                    var strKeyNo = objColumn.KeyNo;
                    var strValue = objColumn.Value;
                    if (!strValue) {
                        strValue = "";
                    }
                    if (strKeyNo == "0" && !IsAuditField(strColumn)) {
                        scope.lstCodeValues.push({
                            CodeID: strValue, Description: strColumn
                        });
                    }
                }
                if (lst && lst.length > 0) {
                    var objEntity = lst[0];

                    angular.forEach(objEntity.Attributes, iAddInlstCodeValues);

                }

                //scope.lstCodeValues.splice(0, 0, { CodeID: "", Description: "" });
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

            scope.$on('onOKClick', function (event, data) {

                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwLoadSource, scope.model.dictAttributes, "sfwLoadSource", data.ID);

            });

            scope.onQuerySearchClick = function (param) {

                scope.attributeName = "sfwLoadSource";
                if (param == "DefaultValue") {
                    scope.attributeName = "sfwDefaultValue";
                }

                scope.strSelectedQuery = scope.model.dictAttributes[scope.attributeName];
                //scope.QueryDialog = ngDialog.open({
                //    template: "Form/views/BrowseForQuery.html",
                //    scope: scope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

                var dialogScope = scope.$new(true);
                //dialogScope = scope;
                dialogScope.attributeName = "sfwLoadSource";
                if (param == "DefaultValue") {
                    dialogScope.attributeName = "sfwDefaultValue";
                }
                dialogScope.strSelectedQuery = scope.model.dictAttributes[scope.attributeName];
                dialogScope.QueryDialog = $rootScope.showDialog(dialogScope, "Browse For Query", "Form/views/BrowseForQuery.html", { width: 1000, height: 700 });
            };

            scope.$on('onQueryClick', function (event, data) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes[scope.attributeName], scope.model.dictAttributes, scope.attributeName, data);

                scope.LoadQueryColumns(scope.model.dictAttributes[scope.attributeName]);
            });
            scope.LoadQueryColumnsOnChange = function (queryId) {
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                scope.LoadQueryColumns(queryId);
            };
            scope.LoadQueryColumns = function (queryId) {
                if (queryId != undefined && queryId != "") {
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

            scope.onMethodQueryParamClick = function () {

                var newScope = scope.$new(true);
                newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                newScope.SelectedObject = scope.model;
                newScope.IsForm = true;
                newScope.IsMultiActiveForm = false;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = scope.isformlink;
                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
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
            //#endregion

            //#region Common Event for Common Properties

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

            scope.LoadModes = function () {
                scope.lstModes = [];
                scope.lstModes.push("All");
                scope.lstModes.push("New");
                scope.lstModes.push("Update");
            };

            scope.LoadSelectionMode = function () {
                scope.lstSelectionMode = [];
                scope.lstSelectionMode.push("Single");
                scope.lstSelectionMode.push("Multiple");
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
                //  scope.lstTextModes.push("");
                scope.lstTextModes.push("SingleLine");
                scope.lstTextModes.push("MultiLine");
                scope.lstTextModes.push("Password");
                scope.lstTextModes.push("Color");
                scope.lstTextModes.push("Date");
                scope.lstTextModes.push("DateTime");
                scope.lstTextModes.push("DateTimeLocal");
                scope.lstTextModes.push("Email");
                scope.lstTextModes.push("Month");
                scope.lstTextModes.push("Number");
                scope.lstTextModes.push("Range");
                scope.lstTextModes.push("Search");
                scope.lstTextModes.push("Phone");
                scope.lstTextModes.push("Time");
                scope.lstTextModes.push("Url");
                scope.lstTextModes.push("Week");
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

            scope.findCascadingParentControl = function () {
                var lstDropdownLists = [];
                if (scope.model.Name == "sfwCascadingDropDownList" || scope.model.Name === "sfwMultiSelectDropDownList") {
                    var ojParentGrid = FindParent(scope.model, "sfwGridView");
                    if (ojParentGrid && ojParentGrid.Elements.length > 0) {
                        FindControlListByName(ojParentGrid, "sfwCascadingDropDownList", lstDropdownLists);
                    }
                    else {
                        FindControlListByName(scope.MainTable, "sfwCascadingDropDownList", lstDropdownLists);
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
                scope.lstGridSelection.splice(0, 0, "");
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

            scope.setClientVisibilityClick = function () {

                //scope.template = 'Form/views/ClientVisibility.html';
                //scope.strAttributeName = "sfwClientVisibility";
                //scope.Title = "Set Client Visibility";
                //scope.IsForm = true;

                var newScope = scope.$new();
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.strAttributeName = "sfwClientVisibility";
                newScope.CurrentTable = scope.CurrentTable;
                newScope.Title = "Set Client Visibility";
                newScope.IsForm = true;
                if (scope.isformlink) {
                    newScope.IsForm = false;
                }

                newScope.objNewDialog = $rootScope.showDialog(newScope, "Set Client Visibility", "Form/views/ClientVisibility.html", { width: 900, height: 500 });
            };

            scope.setClientEnableClick = function () {

                var newScope = scope.$new();
                newScope.strAttributeName = "sfwClientEnable";
                newScope.Title = "Set Client Enability";
                newScope.CurrentTable = scope.CurrentTable;
                newScope.IsForm = true;
                if (scope.isformlink) {
                    newScope.IsForm = false;
                }

                newScope.objNewDialog = $rootScope.showDialog(newScope, "Set Client Enability", "Form/views/ClientVisibility.html", { width: 900, height: 500 });
            };

            scope.onCustomAttributeClick = function () {
                //scope.IsShowCustomAttributes = !scope.IsShowCustomAttributes;

                var newScope = scope.$new();
                newScope.propertyName = "sfwCustomAttributes";
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = scope.isformlink;
                newScope.CurrentTable = scope.CurrentTable;
                newScope.UserLogParaDialog = $rootScope.showDialog(newScope, "Set Custom Attributes", "Form/views/CustomAttributes.html", { width: 900, height: 500 });

                //scope.template = 'Form/views/CustomAttributes.html';

                //scope.Title = "Set Custom Attributes";
                //scope.propertyName = "sfwCustomAttributes";
            };

            scope.onQueryParameterClick = function () {
                var newScope = scope.$new(true);
                newScope.SelectedObject = scope.model;
                newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                newScope.formobject = scope.formobject;
                newScope.isFormLink = scope.isformlink;
                //newScope = scope;
                //newScope.NavigationParameterDialog = ngDialog.open({
                //    template: "Form/views/ParameterNavigation.html",
                //    scope: newScope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom'
                //});

                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
            };
            scope.onCodeGroupParameterClick = function () {
                var newScope = scope.$new(true);
                newScope.SelectedObject = scope.model;
                newScope.formobject = scope.formobject;
                newScope.IsFormCodeGroup = true;
                newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Set Parameters", "Form/views/ParameterNavigation.html", { width: 1000, height: 520 });
            };
            scope.onSetParameterClick = function () {
                if (scope.model.CommonProp == 'CodeGroup') {
                    scope.onCodeGroupParameterClick();
                }
                else {
                    scope.onQueryParameterClick();
                }
            };
            scope.openImageConditionClick = function (item) {
                //scope.template = 'Form/views/ImageCondition.html';
                //scope.oldConditions = item;


                var newScope = scope.$new(true);
                newScope.oldConditions = item;
                newScope.formobject = scope.formobject;
                newScope.model = scope.model;
                newScope.ImageConditionDialog = $rootScope.showDialog(newScope, "Image Condition", "Form/views/ImageCondition.html", { width: 600, height: 500 });
            };

            scope.openBindtoQueryClick = function (item) {
                var newScope = scope.$new();
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
                var lstRowFormat = model.Elements.filter(function (itm) { return itm.Name == "rowformat"; });
                if (IsCellOrRow != "CellFormat" && (!lstRowFormat || (lstRowFormat && lstRowFormat.length == 0))) {
                    var rowformat = { Name: "rowformat", Value: "", dictAttributes: {}, Elements: [], Children: [] };
                    model.Elements.push(rowformat);
                }
                var FormatType = "Row Format";
                if (dialogScope.IsCellOrRow == "CellFormat") {
                    FormatType = "Cell Format";
                }
                dialogScope.CellFormatDialog = $rootScope.showDialog(dialogScope, FormatType, "Form/views/CellFormate.html", { width: 600, height: 565 });
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
                newScope.validateValidationRules = scope.validateValidationRules;
                newScope.dialogValidation = $rootScope.showDialog(newScope, "Hard Errors", "Form/views/SetValidationRules.html", { width: 700, height: 460 });
            };

            scope.FuncSetColSpanRowSpan = function () {
                var objCurrentParent = scope.model.ParentVM;
                var objCurrentParentTable = objCurrentParent ? objCurrentParent.ParentVM : null;
                var numCurrentrowindex = objCurrentParentTable.Elements.indexOf(objCurrentParent);
                var lstSiblingColumn = objCurrentParent.Elements;
                var numCurrentcolindex = lstSiblingColumn.indexOf(scope.model);
                var objColRowspanscope = scope.$new(true);
                var numOldColspan = parseInt(scope.model.dictAttributes.ColumnSpan) ? parseInt(scope.model.dictAttributes.ColumnSpan) : 1;
                var numOldRowspan = parseInt(scope.model.dictAttributes.RowSpan) ? parseInt(scope.model.dictAttributes.RowSpan) : 1;
                objColRowspanscope.ColumnSpan = scope.model.dictAttributes.ColumnSpan;
                objColRowspanscope.RowSpan = scope.model.dictAttributes.RowSpan;
                objColRowspanscope.objTableModel = angular.copy(objCurrentParentTable, objColRowspanscope.objTableModel);
                objColRowspanscope.formodel = scope.formobject;
                objColRowspanscope.SetColRowSpanDialog = $rootScope.showDialog(objColRowspanscope, "Set Column/Row Span", "Form/views/SetColRowSpan.html", { width: 1000, height: 500, showclose: true });
                setTimeout(function () {
                    var elem = $("#ColSpan_Dialog_" + objColRowspanscope.$id + " #virtual_column_" + numCurrentrowindex + "_" + numCurrentcolindex);
                    if (elem) {
                        if (elem.length > 0) {
                            elem[0].scrollIntoView();
                        }
                    }
                }, 500);
                objColRowspanscope.okClick = function (IsChange) {
                    function ColumnDeletingResetCallback(itemcolumn, index, array) {
                        itemcolumn.isDeleting = false;
                        itemcolumn.isAdding = false;
                        itemcolumn.isEditing = false;
                    }
                    function GetRowColspanCount(ItemName) {
                        var numrowColSpanCount = 0;
                        var arrayValidcolspan = [];
                        return {
                            resetCount: function () {
                                numrowColSpanCount = 0;
                            },
                            setCount: function (itemcolumn, index, array) {
                                // max count of colspan in entire table                                        
                                numrowColSpanCount += parseInt(itemcolumn.dictAttributes[ItemName]) ? parseInt(itemcolumn.dictAttributes[ItemName]) : 1;
                                if (index == array.length - 1) {
                                    arrayValidcolspan.push(numrowColSpanCount);
                                }
                            },
                            getCount: function () {
                                return numrowColSpanCount;
                            },
                            getCounts: function () {
                                return arrayValidcolspan;
                            }
                        };
                    }
                    function itemrowColumnCallback(itemcolumn, index, array) {
                        objRowCount.setCount(itemcolumn, index, array);
                    }
                    function RowCallbackobjCurrentParentTable(itemrow, index, array) {
                        // dont consider the current row for determining the valid colspan
                        if (index != numCurrentrowindex) {
                            objRowCount.resetCount();
                            itemrow.Elements.forEach(itemrowColumnCallback);
                        }
                    }
                    function ColumnCallbacknumCellstoadd(itemcolumn, index, array) {
                        if (index > numCurrentcolindex) {
                            numCellstoadd -= parseInt(itemcolumn.dictAttributes.ColumnSpan) ? parseInt(itemcolumn.dictAttributes.ColumnSpan) : 1;
                        }
                    }
                    function ColumnCallbackitemrow(itemcolumn, index, array) {
                        if (index == numCurrentcolindex) {
                            objRowCount.setCount(itemcolumn, index, array);
                        }
                        else if (index > numCurrentcolindex) {
                            return true;
                        }
                    }
                    function ColumnColSpanSumCallback(itemcolumn, index, array) {
                        if (index < numCurrentcolindex) {
                            numSumColspans += parseInt(itemcolumn.dictAttributes.ColumnSpan) ? parseInt(itemcolumn.dictAttributes.ColumnSpan) : 1;
                        }
                    }
                    function RowCallbackobjCurrentParentTable(itemrow, index, array) {
                        // dont consider the rows prior to the current row for determining the valid rowspan
                        if (index > numCurrentrowindex) {
                            itemrow.Elements.some(ColumnCallbackitemrow);
                        }
                        else if (index == numCurrentrowindex) {
                            itemrow.Elements.some(ColumnColSpanSumCallback);
                        }
                    }
                    function RowSpanColumnCallback(itemcolumn, index, array) {
                        // critical
                        if ($ValidateBaseModelStructure.is_validModelElements(itemcolumn, 'sfwColumn', false) && itemcolumn.dictAttributes.ColumnSpan && (parseInt(itemcolumn.dictAttributes.ColumnSpan) > 1)) {
                            if (IsChange) {
                                itemcolumn.isEditing = true;
                            }
                            $rootScope.EditPropertyValue(itemcolumn.dictAttributes.ColumnSpan, itemcolumn.dictAttributes, "ColumnSpan", itemcolumn.dictAttributes.ColumnSpan - 1);
                        }
                        else {
                            if (IsChange) {
                                itemcolumn.isDeleting = true;
                            }
                            else {
                                $rootScope.DeleteItem(itemcolumn, array);
                            }
                        }
                    }
                    function iColumnCallback(itemcolumn, index, array) {
                        if (index > numCurrentcolindex) {
                            numIndexCol -= parseInt(itemcolumn.dictAttributes.ColumnSpan) ? parseInt(itemcolumn.dictAttributes.ColumnSpan) : 1;
                        }
                    }
                    function iColumnCallbacknumIndexCol(itemcolumn, index, array) {
                        if (index > numCurrentcolindex) {
                            numIndexCol -= parseInt(itemcolumn.dictAttributes.ColumnSpan) ? parseInt(itemcolumn.dictAttributes.ColumnSpan) : 1;
                        }
                    }
                    if ($ValidateBaseModelStructure.is_validModelElements(objCurrentParent, 'sfwRow', true) && $ValidateBaseModelStructure.is_validModelElements(objCurrentParentTable, 'sfwTable', true)) {
                        // if user clicks of reflect the changes in the main table
                        var objCurrentParent = scope.model.ParentVM;
                        var objCurrentParentTable = objCurrentParent ? objCurrentParent.ParentVM : null;
                        var numCurrentrowindex = objCurrentParentTable.Elements.indexOf(objCurrentParent);
                        var lstSiblingColumn = objCurrentParent.Elements;
                        var numCurrentcolindex = lstSiblingColumn.indexOf(scope.model);
                        var numColSibling = lstSiblingColumn.length;
                        if (IsChange) {
                            // all variables are set from the clone object
                            objColRowspanscope.objTableModel = angular.copy(objCurrentParentTable, objColRowspanscope.objTableModel);

                            objCurrentParentTable = objColRowspanscope.objTableModel;
                            objCurrentParent = objCurrentParentTable.Elements[numCurrentrowindex];
                            objCurrentParent.Elements.forEach(ColumnDeletingResetCallback);
                            objColRowspanscope.objCtrlModel = objCurrentParent.Elements[numCurrentcolindex];
                        }

                        if (!IsChange) {
                            $rootScope.UndRedoBulkOp("Start");
                        }
                        // for colspan                        
                        if (parseInt(objColRowspanscope.ColumnSpan) && parseInt(scope.model.dictAttributes.ColumnSpan) !== parseInt(objColRowspanscope.ColumnSpan)) {
                            var numValidcolspan = 0;
                            if (numCurrentcolindex > -1) {
                                var objRowCount = GetRowColspanCount('ColumnSpan');

                                objCurrentParentTable.Elements.forEach(RowCallbackobjCurrentParentTable);
                                numValidcolspan = Math.max.apply(null, objRowCount.getCounts());
                                // minus colspans for columns prior to the current column
                                lstSiblingColumn.some(function (item, index) {
                                    if (index < numCurrentcolindex) {
                                        numValidcolspan -= item.dictAttributes.ColumnSpan ? item.dictAttributes.ColumnSpan : 1;
                                    }
                                    else {
                                        return true;
                                    }
                                });
                                // if invalid then delete all right 
                                if (numValidcolspan < parseInt(objColRowspanscope.ColumnSpan)) {
                                    for (var i = numColSibling - 1; i > numCurrentcolindex; i--) {
                                        if (IsChange) {
                                            objCurrentParent.Elements[i].isDeleting = true;
                                        }
                                        else {
                                            $rootScope.DeleteItem(objCurrentParent.Elements[i], objCurrentParent.Elements);
                                        }
                                    }
                                }
                                // for valid check condition
                                if (numValidcolspan >= parseInt(objColRowspanscope.ColumnSpan)) {
                                    // if new value greater then old value then delete the cells
                                    if (parseInt(objColRowspanscope.ColumnSpan) > numOldColspan) {
                                        var numCelltoDelete = parseInt(objColRowspanscope.ColumnSpan) - numOldColspan;
                                        for (var i = numCurrentcolindex + numCelltoDelete; i > numCurrentcolindex; i--) {
                                            if (IsChange) {
                                                objCurrentParent.Elements[i].isDeleting = true;
                                            }
                                            else {
                                                $rootScope.DeleteItem(objCurrentParent.Elements[i], objCurrentParent.Elements);
                                            }
                                        }
                                    }
                                    // if new value less then old value then add the cells on the right side
                                    else if (parseInt(objColRowspanscope.ColumnSpan) < numOldColspan) {
                                        var sfxCellModel = function () {
                                            var objNewCell = {};
                                            if (!IsChange) {
                                                objNewCell = { Name: "sfwColumn", value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [], ParentVM: objCurrentParent };
                                            }
                                            else {
                                                objNewCell = { Name: "sfwColumn", value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [], ParentVM: objCurrentParent, isAdding: true };
                                            }
                                            return objNewCell;
                                        };
                                        // check for last column
                                        // new value should be less or equal to valid colspan
                                        if (parseInt(objColRowspanscope.ColumnSpan) <= numValidcolspan) {
                                            var numCellstoadd = (numValidcolspan - (parseInt(objColRowspanscope.ColumnSpan) ? parseInt(objColRowspanscope.ColumnSpan) : 1));

                                            objCurrentParent.Elements.forEach(ColumnCallbacknumCellstoadd);
                                            if (numCurrentcolindex < objCurrentParent.Elements.length - 1) {
                                                for (var i = 0; i < numCellstoadd; i++) {
                                                    $rootScope.InsertItem(new sfxCellModel(), objCurrentParent.Elements, numCurrentcolindex + 1);
                                                }
                                            }
                                            else {
                                                for (var i = 0; i < numCellstoadd; i++) {
                                                    $rootScope.PushItem(new sfxCellModel(), objCurrentParent.Elements);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!IsChange) {
                                    $rootScope.EditPropertyValue(scope.model.dictAttributes.ColumnSpan, scope.model.dictAttributes, "ColumnSpan", objColRowspanscope.ColumnSpan);
                                }
                            }
                        }
                        numOldColspan = parseInt(scope.model.dictAttributes.ColumnSpan) ? parseInt(scope.model.dictAttributes.ColumnSpan) : 1;
                        // for rowspan 
                        if (parseInt(objColRowspanscope.RowSpan) && parseInt(scope.model.dictAttributes.RowSpan) !== parseInt(objColRowspanscope.RowSpan)) {
                            var numValidrowspan = 0;
                            // consider col index to be count of the colspan till current column
                            var numSumColspans = 0;
                            if (numCurrentrowindex > -1) {
                                var objRowCount = GetRowColspanCount('RowSpan');

                                objCurrentParentTable.Elements.forEach(RowCallbackobjCurrentParentTable);
                                numValidrowspan = objRowCount.getCount() + 1; // for the rowspan of the current col                                

                                //if invalid then delete all cols at the index (consider colspans of previous columns) in subsquent rows - if cols have colspans do minus 1
                                if (numValidrowspan < parseInt(objColRowspanscope.RowSpan)) {
                                    // dont consider the rows prior to the current row for determining the valid rowspan
                                    for (var i = numCurrentrowindex + 1; i < objCurrentParentTable.Elements.length; i++) {
                                        var numIndexCol = objCurrentParentTable.Elements[i].Elements.length - 1;

                                        objCurrentParentTable.Elements[i].Elements.forEach(iColumnCallback);
                                        while (numIndexCol >= numSumColspans) {
                                            RowSpanColumnCallback(objCurrentParentTable.Elements[i].Elements[numIndexCol], numIndexCol, objCurrentParentTable.Elements[i].Elements);
                                            numIndexCol -= 1;
                                        }
                                    }
                                }
                                // for valid check condition
                                if (numValidrowspan >= parseInt(objColRowspanscope.RowSpan)) {
                                    // if new value greater then old value then update colspan at corresponding index
                                    if (parseInt(objColRowspanscope.RowSpan) > numOldRowspan) {
                                        var numRowtoTraverse = parseInt(objColRowspanscope.RowSpan) - numOldRowspan;
                                        for (var i = numCurrentrowindex + 1, j = 0; j < numRowtoTraverse; i++ , j++) {
                                            var numIndexCol = objCurrentParentTable.Elements[i].Elements.length - 1;

                                            objCurrentParentTable.Elements[i].Elements.forEach(iColumnCallbacknumIndexCol);
                                            while (numIndexCol >= numSumColspans) {
                                                RowSpanColumnCallback(objCurrentParentTable.Elements[i].Elements[numIndexCol], numIndexCol, objCurrentParentTable.Elements[i].Elements);
                                                numIndexCol -= 1;
                                            }
                                        }
                                    }
                                    // if new value less then old value then add the cells in the corresponding index
                                    else {
                                        var numCellstoadd = parseInt(scope.model.dictAttributes.RowSpan) - (parseInt(objColRowspanscope.RowSpan) ? parseInt(objColRowspanscope.RowSpan) : 1);
                                        for (var i = numCurrentrowindex + parseInt(scope.model.dictAttributes.RowSpan) - 1, j = 0; i < objCurrentParentTable.Elements.length && j < numCellstoadd; i++ , j++) {
                                            var sfxCellModel = function () {
                                                var objNewCell = {};
                                                if (!IsChange) {
                                                    objNewCell = { Name: "sfwColumn", value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [], ParentVM: objCurrentParentTable.Elements[i] };
                                                }
                                                else {
                                                    objNewCell = { Name: "sfwColumn", value: '', prefix: "swc", dictAttributes: {}, Elements: [], Children: [], ParentVM: objCurrentParentTable.Elements[i], isAdding: true };
                                                }
                                                return objNewCell;
                                            };
                                            if (numSumColspans < objCurrentParentTable.Elements[i].Elements.length - 1) {
                                                $rootScope.InsertItem(new sfxCellModel(), objCurrentParentTable.Elements[i].Elements, numSumColspans);
                                            }
                                            else {
                                                $rootScope.PushItem(new sfxCellModel(), objCurrentParentTable.Elements[i].Elements);
                                            }
                                        }
                                    }
                                }
                                if (!IsChange) {
                                    $rootScope.EditPropertyValue(scope.model.dictAttributes.RowSpan, scope.model.dictAttributes, "RowSpan", objColRowspanscope.RowSpan);
                                }
                            }
                        }
                        if (!IsChange) {
                            $rootScope.UndRedoBulkOp("End");
                        }
                    }
                    if (!IsChange) {
                        objColRowspanscope.closeClick();
                    }
                };
                objColRowspanscope.closeClick = function () {
                    scope.$evalAsync(function () {
                        objColRowspanscope.SetColRowSpanDialog.close();
                        objColRowspanscope.$destroy();
                    });
                };
            };

            scope.validateValidationRules = function (list) {
                if (scope.model && scope.model.dictAttributes) {
                    var lstHardErrors = [];
                    scope.lstHardErrorData;
                    var input = scope.model.dictAttributes.sfwValidationRules;
                    if (scope.$parent.iswizard) {
                        var objWizardStep = FindParent(scope.model, "sfwWizardStep");
                        if (objWizardStep) {
                            var strRuleGroup = objWizardStep.dictAttributes.sfwRulesGroup;
                            lstHardErrors = scope.$parent.createValidationRuleList(scope.formobject.objExtraData, scope.$parent.iswizard, strRuleGroup);
                        }
                    } else if (scope.entityName && !scope.lstHardErrorData && !list) {
                        scope.lstHardErrorData = [];
                        hubMain.server.getEntityExtraData(scope.entityName).done(function (data) {
                            scope.$evalAsync(function () {
                                if (data && data.lstHardErrorList) {
                                    var dataList = data.lstHardErrorList[0];
                                    if (dataList.Elements.length > 0) {
                                        angular.forEach(dataList.Elements, function (item) {
                                            if (item && item.dictAttributes.ID) {
                                                scope.lstHardErrorData.push(item.dictAttributes.ID);
                                            }
                                        });
                                    }
                                    lstHardErrors = scope.lstHardErrorData;
                                    $ValidationService.checkMultipleValueWithList(scope.lstHardErrorData, scope.model, input, ";", 'sfwValidationRules', "invalid_validation_rule", CONST.VALIDATION.VALIDATION_RULE_NOT_EXISTS, undefined);
                                }
                            });
                        });
                    } else {
                        lstHardErrors = scope.$parent.createValidationRuleList(scope.formobject.objExtraData, false, null);
                    }
                    if (list) {
                        lstHardErrors = list;
                    } else if (scope.lstHardErrorData) {
                        lstHardErrors = scope.lstHardErrorData;
                    }
                    $ValidationService.checkMultipleValueWithList(lstHardErrors, scope.model, input, ";", 'sfwValidationRules', "invalid_validation_rule", CONST.VALIDATION.VALIDATION_RULE_NOT_EXISTS, undefined);
                }
            };
            scope.OpenImageUrl = function (openFrom) {
                if (scope.formobject.dictAttributes.WebSite) {
                    //var lstDialog = ngDialog.getOpenDialogs();
                    //if (lstDialog && lstDialog.length > 0) {
                    //    var dialogId = lstDialog[lstDialog.length - 1];
                    $.connection.hubForm.server.openImageUrlClick(scope.formobject.dictAttributes.WebSite, openFrom).done(function (data) {
                        if (data && data.length == 2) {
                            scope.receiveImageFileName(data[0], data[1]);
                        }
                    });
                    //}
                }
                else {
                    $SgMessagesService.Message('Message', "Please select the appropriate 'Website' from the form details page.");
                }
            };
            scope.receiveImageFileName = function (fileName, errorMessage) {
                if (fileName != "") {
                    scope.$apply(function () {
                        scope.model.dictAttributes.ImageUrl = fileName;
                    });
                }
                if (errorMessage != "" && errorMessage != undefined) {
                    $SgMessagesService.Message('Message', errorMessage);
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
                //dialogScope.QueryDialog = ngDialog.open({
                //    template: "Form/views/BrowseForQuery.html",
                //    scope: dialogScope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                //});

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

                    if (index > 0) {
                        scope.selectedItem = scope.model.Elements[index - 1];
                    }
                    else {
                        scope.selectedItem = scope.model.Elements[index];
                    }
                }
                //else if(scope.model.Elements.length>0){
                //    scope.model.Elements.splice(scope.model.Elements.length-1,1);
                //}
            };
            scope.commonTemplateSelectionChange = function (para) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue(scope.model.Elements, scope.model, "Elements", []);
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                if (scope.model.Name == "sfwCascadingDropDownList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", "");
                }
                $rootScope.EditPropertyValue(scope.model.dictAttributes.DataTextField, scope.model.dictAttributes, "DataTextField", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.DataValueField, scope.model.dictAttributes, "DataValueField", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwLoadSource, scope.model.dictAttributes, "sfwLoadSource", "");
                $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwImageField, scope.model.dictAttributes, "sfwImageField", "");

                if (scope.model && scope.model.Name == "sfwCheckBoxList") {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwDefaultValue, scope.model.dictAttributes, "sfwDefaultValue", "");
                }
                if (scope.model && (scope.model.Name == "sfwDropDownList" || scope.model.Name == "sfwCascadingDropDownList")) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwImageField, scope.model.dictAttributes, "sfwImageField", "");
                }
                $rootScope.UndRedoBulkOp("End");
                //scope.model.dictAttributes.sfwLoadType = para;

                scope.InitializeDropDownItems();
                scope.setDefaultCodeGroup();

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
                    if (scope.model.hasOwnProperty("errors")) {
                        for (var prop in scope.model.errors) {
                            if (["sfwEventId", "sfwEventName", "sfwEventStartDate", "sfwEventEndDate", "sfwEventCategory"].indexOf(prop) > -1) {
                                delete scope.model.errors[prop];
                            }
                        }
                    }
                    $rootScope.UndRedoBulkOp("Start");
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

                var lstEntity = lstentitylist.filter(function (itm) {
                    return itm.ID == entityName;
                });
                if (lstEntity && lstEntity.length > 0) {
                    objEntity = lstEntity[0];
                }
                return objEntity;
            };

            scope.onQueryIDChange = function (event) {
                var input = $(event.target);
                scope.queryIdInput = input;
                if (event && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                else {
                    createQueryIDList(input);
                }
            };

            scope.showQueryIDIntellisenseList = function (event) {
                var inputElement;
                inputElement = $(event.target).prevAll("input[type='text']");
                scope.queryIdInput = inputElement;
                inputElement.focus();
                if (inputElement) {
                    createQueryIDList(inputElement);
                    if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                }
                if (event) {
                    event.stopPropagation();
                }
            };

            var createQueryIDList = function (input) {
                //var formScope = getCurrentFileScope();
                scope.queryIdList = [];
                function iterator(itm) {
                    lst.push(itm.dictAttributes.ID);
                }
                if (scope.formobject) {
                    var lstQueryID = GetQueryListFromObject(scope.formobject);
                    //if (formScope && formScope.lstQueryID) {
                    var lst = [];

                    angular.forEach(lstQueryID, iterator);
                    scope.queryIdList = lst;
                    setSingleLevelAutoComplete(input, lst, scope);
                    //}
                }
            };

            scope.PopulateRulesBasedOnEntity = function (entityName) {
                scope.Rules = [];

                var lstEntity = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (itm) {
                    return itm.ID == entityName;
                });
                if (lstEntity && lstEntity.length > 0) {
                    if (lstEntity[0].Rules.length > 0) {
                        scope.Rules = lstEntity[0].Rules;
                    }
                }
            };

            scope.validateQueryId = function (model) {
                var property = "sfwQueryID";
                $ValidationService.checkValidListValue(scope.queryIdList, model, scope.queryIdInput ? scope.queryIdInput.val() : '', property, "invalid_query_id", CONST.VALIDATION.INVALID_QUERY_ID, undefined);
            };
            scope.onBaseQueryChange = function (QueryId, isIgnoreForUndoRedo) {
                var DummyParams = [];
                var formScope = getCurrentFileScope();
                if (formScope && formScope.selectControl) {
                    formScope.selectControl(scope.model);
                }
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
                scope.model.dictAttributes.sfwBaseQuery = QueryId;
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
                            $rootScope.UndRedoBulkOp("End");
                        }
                    }
                }
                else {
                    var lstParams = scope.model.Elements.filter(function (itm) { return itm.Name == "Parameters"; });
                    var objParameters;
                    if (lstParams && lstParams.length > 0) {
                        objParameters = lstParams[0];
                        objParameters.Elements = [];
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

            scope.onGridDataSourceTypeChange = function () {
                var curscope = getCurrentFileScope();

                if (curscope.loadFormEntityTree) curscope.loadGridEntityTree(scope.model);

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

            //#region Event and Methods For Button

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

            scope.PopulateOnCommandName = function () {
                scope.lstOnCommandName = [];
                scope.lstOnCommandName.push("");
                scope.lstOnCommandName.push("Cancel");
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
                    GetAllControls(scope.formobject, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton", "tblcriteria", "", false, scope.RelatedControlsForNew, false);
                else {

                    GetAllControls(scope.formobject, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton,sfwScheduler", "", "", false, scope.RelatedControlsForNew, true);

                }

                var obj = { dictAttributes: { ID: "" } };
                scope.RelatedControlsForNew.splice(0, 0, obj);

            };

            scope.PoulateRelatedControlForFinishScheduler = function () {
                scope.lstSchedulerControl = [];
                FindControlListByName(scope.MainTable, 'sfwScheduler', scope.lstSchedulerControl);
                var obj = { dictAttributes: { ID: "" } };
                scope.lstSchedulerControl.splice(0, 0, obj);
            };

            scope.CustomMethodNameChange = function () {
                var curscope = getCurrentFileScope();
                if (curscope) {
                    curscope.isDirty = true;
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
                else {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", "");
                }
                scope.setActiveForm();
            };

            scope.PopulateExecuteAfterSuccessButton = function () {
                scope.lstExecuteAfterSuccessButton = [];
                scope.lstExecuteAfterSuccessButton.push("");
                var lst = [];
                FindControlListByName(scope.MainTable, 'sfwButton', lst);
                function iteration(obj) {
                    if (obj.dictAttributes.ID !== scope.model.dictAttributes.ID) {
                        scope.lstExecuteAfterSuccessButton.push(obj.dictAttributes.ID);
                    }
                }
                angular.forEach(lst, iteration);
            };

            scope.PopulateMessageDescription = function () {
                scope.model.MessageDescription = "";
                if (scope.model.dictAttributes.sfwMessageID != undefined && scope.model.dictAttributes.sfwMessageID != "") {
                    hubMain.server.populateMessageList().done(function (lstMessages) {
                        if (lstMessages && lstMessages.length > 0) {
                            scope.$evalAsync(function () {
                                var lst = lstMessages.filter(function (x) { return x.MessageID == scope.model.dictAttributes.sfwMessageID; });
                                if (lst && lst.length > 0) {
                                    scope.model.MessageDescription = lst[0].DisplayMessage;
                                }
                            });
                        }
                    });
                }
            };

            scope.PopulateBooleanValues = function () {
                scope.lstBooleanValues.push("True");
                scope.lstBooleanValues.push("False");
            };

            scope.PopulateWizardSteps = function () {
                scope.WizardStepNames = [];
                var lst = [];
                FindControlListByName(scope.MainTable, "WizardSteps", lst);
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
                    var objWizardSteps = lst[0];
                    if (objWizardSteps) {


                        angular.forEach(objWizardSteps.Elements, iterator);
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
                //scope.template = 'Form/views/CustomAttributes.html';

                //scope.Title = "Set User Log Parameters";
                //scope.propertyName = "sfwUserLogParameters";

                var newScope = scope.$new(true);
                newScope.model = scope.model;
                newScope.formobject = scope.formobject;
                newScope.propertyName = "sfwUserLogParameters";
                newScope.isFormLink = scope.isformlink;
                newScope.CurrentTable = scope.CurrentTable;
                newScope.UserLogParaDialog = $rootScope.showDialog(newScope, "Set User Log Parameters", "Form/views/CustomAttributes.html", { width: 800, height: 525 });
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
                    newScope.NavigationParameterOpenWordDialog = $rootScope.showDialog(newScope, "Navigation Parameter", "Form/views/ParameterNavigationOpenWord.html", { width: 800, height: 500 });
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
                    newScope.dialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/NavParamGridSearch.html", { width: 1000, height: 520 });
                }
                else if (scope.model.dictAttributes.sfwMethodName == 'btnOpenReport_Click') {
                    var newScope = scope.$new(true);
                    newScope.model = scope.model;
                    newScope.istrEntityName = "";
                    if (scope.model.dictAttributes.sfwRelatedControl) {
                        var objmodel = FindControlByID(scope.formobject, scope.model.dictAttributes.sfwRelatedControl);
                        if (objmodel && objmodel.dictAttributes.sfwEntityField) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objmodel.dictAttributes.sfwEntityField);
                            if (object) {
                                newScope.istrEntityName = object.Entity;
                            }
                        }
                    }
                    else if (scope.formobject.dictAttributes.sfwEntity) {
                        newScope.istrEntityName = scope.formobject.dictAttributes.sfwEntity;
                    }
                    newScope.ParameterNavigationOpenReportDialog = $rootScope.showDialog(newScope, "Navigation Parameter", "Form/views/ParameterNavigationOpenReport.html", { width: 700, height: 400 });
                }
                else {
                    var newScope = scope.$new(true);
                    newScope.selectedCurrentQuery = scope.selectedCurrentQuery;
                    newScope.SelectedObject = scope.model;
                    newScope.IsForm = true;
                    newScope.IsMultiActiveForm = false;
                    newScope.formobject = scope.formobject;
                    newScope.isFormLink = scope.isformlink;
                    var objGrid = FindParent(newScope.SelectedObject, "sfwGridView");
                    if (objGrid) {
                        newScope.ParentModel = objGrid;
                    }
                    var objListView = FindParent(newScope.SelectedObject, "sfwListView");
                    if (objListView) {
                        newScope.ParentModel = objListView;
                    }

                    newScope.NavigationParameterDialog = $rootScope.showDialog(newScope, "Navigation Parameters", "Form/views/ParameterNavigation.html", {
                        width: 1000, height: 520
                    });
                }
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
                    $SgMessagesService.Message('Message', "Please select active tooltip form");
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
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", strNavigationParameter, "sfwLaunchNewWindow=" + str + ";" + "sfwShowDownloadDialog=" + str);
                    }
                    else if (scope.model.IsLaunchNewWindow && !scope.model.IsShowDownloadDialog) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", strNavigationParameter, "sfwLaunchNewWindow=" + str);
                    }
                    else if (!scope.model.IsLaunchNewWindow && scope.model.IsShowDownloadDialog) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", strNavigationParameter, "sfwShowDownloadDialog=" + str);
                    }
                    else if (!scope.model.IsLaunchNewWindow && !scope.model.IsShowDownloadDialog) {
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwNavigationParameter, scope.model.dictAttributes, "sfwNavigationParameter", strNavigationParameter, "");
                    }

                }
            };

            //#endregion

            //#region Method For Image Button

            scope.PopulateTextRelatedControl = function () {
                scope.TextRelatedControls = [];
                PopulateTextRelatedControls(scope.formobject, scope.TextRelatedControls);
                scope.TextRelatedControls.splice(0, 0, "");
            };

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

            scope.OnActiveFormValueChange = function (isViewChange) {
                if (scope.model.Name == "sfwImageButton") {
                    if (scope.model.dictAttributes.sfwActiveForm) {
                        if (isViewChange) {
                            scope.lstSelectedIndex = [];
                            scope.lstSelectedIndex.push("");
                            scope.model.dictAttributes.sfwSelectedIndex = "";
                        }
                        // if active form is valid - only then get form model - only appplicable for onchange
                        if ((scope.model.errors && !scope.model.errors.invalid_active_form) || !scope.model.errors) {
                            $.connection.hubForm.server.getNewFormModel(scope.model.dictAttributes.sfwActiveForm).done(function (data) {
                                scope.receivenewformmodel(data);
                            });
                        }
                    }
                    else {
                        // when user deletes active form manually -- clear binded List for selectedIndex and and property sfwSelectedIndex
                        scope.lstSelectedIndex = [];
                        scope.lstSelectedIndex.push("");
                        scope.model.dictAttributes.sfwSelectedIndex = "";
                    }
                }
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

            scope.onRetrievalParametersClick = function (model) {

                if (model.dictAttributes.sfwActiveForm) {
                    var newScope = scope.$new(true);
                    newScope.model = model;
                    newScope.formmodel = scope.formobject;
                    newScope.targetFormModel = scope.objActiveFormModel;

                    newScope.RetrievalButtonParaDialog = $rootScope.showDialog(newScope, "Retrieval Parameters", "Form/views/RetrievalButtonParameters.html", {
                        width: 800, height: 600
                    });
                } else {
                    $SgMessagesService.Message('Message', "Target Form not present.");
                }

            };

            //#endregion 

            //#region Web Control Methods
            scope.LoadWebControlProperties = function () {
                if ($rootScope.lstWebControls && $rootScope.lstWebControls.length > 0 && scope.model) {
                    var lst = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    lst = JSON.parse(JSON.stringify(lst));
                    if (lst && lst.length > 0) {
                        if (scope.IsAdvancedOpen) {
                            scope.objAdvanceProperties = {};
                            scope.objAdvanceProperties.isAccessibilityPropertiesExist = lst[0].lstAccessibility && lst[0].lstAccessibility.length > 0 ? true : false;
                            scope.objAdvanceProperties.isAppearancePropertiesExist = lst[0].lstAppearance && lst[0].lstAppearance.length > 0 ? true : false;
                            scope.objAdvanceProperties.isBehaviorPropertiesExist = lst[0].lstBehavior && lst[0].lstBehavior.length > 0 ? true : false;
                            scope.objAdvanceProperties.isCustomPropertiesExist = lst[0].lstCustom && lst[0].lstCustom.length > 0 ? true : false;
                            scope.objAdvanceProperties.isLayoutPropertiesExist = lst[0].lstLayout && lst[0].lstLayout.length > 0 ? true : false;
                            scope.objAdvanceProperties.isNavigationPropertiesExist = lst[0].lstNavigation && lst[0].lstNavigation.length > 0 ? true : false;
                            scope.objAdvanceProperties.isExtraPropertiesExist = lst[0].lstExtra && lst[0].lstExtra.length > 0 ? true : false;
                            scope.objAdvanceProperties.isMiscPropertiesExist = lst[0].lstMisc && lst[0].lstMisc.length > 0 ? true : false;
                            scope.objAdvanceProperties.lstAccessibility = lst[0].lstAccessibility;
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

            scope.SetDefaultValueToModel = function (objWebControl) {

                var lst = scope.GetPropertyList(objWebControl, "Common");
                function iterator(itm) {
                    if (itm.DefaultValue != undefined && itm.DefaultValue != "") {
                        if ((scope.model.dictAttributes[itm.PropertyName] == undefined || scope.model.dictAttributes[itm.PropertyName] == "") && scope.model.dictAttributes[itm.PropertyName] != itm.DefaultValue) {
                            scope.model.dictAttributes[itm.PropertyName] = itm.DefaultValue;
                        }
                    }
                }
                if (lst && lst.length > 0) {

                    angular.forEach(lst, iterator);
                }
            };

            scope.GetPropertyList = function (objWebControl, propertyType) {
                //function filterProp(x) {
                //    return x.PropertyType == propertyType;
                //}
                //var lst = objWebControl.lstAccessibility.filter(filterProp);
                //var lst1 = objWebControl.lstAppearance.filter(filterProp);
                //var lst2 = objWebControl.lstBehavior.filter(filterProp);
                //var lst3 = objWebControl.lstCustom.filter(filterProp);
                //var lst4 = objWebControl.lstLayout.filter(filterProp);
                //var lst5 = objWebControl.lstNavigation.filter(filterProp);
                //var lst6 = objWebControl.lstExtra.filter(filterProp);

                var lstProperties = [];

                function iterator(x) {
                    lstProperties.push(x);
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
                }
                return lstProperties;
            };

            scope.GetPropertyListByName = function (objWebControl, propName) {
                function filterProp(x) {
                    return x.PropertyName == propName;
                }
                function AddInlstProperties(x) {
                    lstProperties.push(x);
                }
                var lstProperties = [];
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

            scope.SetDataToWebControl = function () {
                if (scope.model) {
                    var ilsttempwebcontrol = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    ilsttempwebcontrol = JSON.parse(JSON.stringify(ilsttempwebcontrol));

                    scope.ClearWebControlPropertyValue(ilsttempwebcontrol[0]);
                    function iterator(value, key) {
                        var lst = scope.GetPropertyListByName(scope.objAdvanceProperties, key);
                        //var lst = scope.lstWebControl.filter(function (x) {
                        //    return x.PropertyName == key
                        //});
                        if (lst && lst.length > 0) {
                            lst[0].PropertyValue = value;
                        }
                    }
                    angular.forEach(scope.model.dictAttributes, iterator);
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
                    var ilsttempwebcontrol = $rootScope.lstWebControls.filter(function (x) {
                        return x.ControlName == scope.model.Name;
                    });
                    ilsttempwebcontrol = JSON.parse(JSON.stringify(ilsttempwebcontrol));

                    var defaultValue = "";
                    if (ilsttempwebcontrol && ilsttempwebcontrol.length > 0) {
                        var lstProp = scope.GetPropertyListByName(ilsttempwebcontrol[0], prop.PropertyName);
                        if (lstProp && lstProp.length > 0) {
                            defaultValue = lstProp[0].DefaultValue;
                        }
                    }
                    $rootScope.EditPropertyValue(scope.model.dictAttributes[prop.PropertyName], scope.model.dictAttributes, prop.PropertyName, defaultValue);
                    $rootScope.DeleteItem(prop, scope.dictAttributesArray);
                    if (scope.model.UcChild) {
                        $rootScope.EditPropertyValue(scope.model.UcChild, scope.model, "UcChild", []);
                    }

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

            //#region Retrieval functionality as per new structure

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

            scope.RetrievalQueryInputChange = function (isLoad) {
                scope.lstColumns = [];
                if (scope.model.dictAttributes.sfwRetrievalQuery) {
                    var lst = scope.model.dictAttributes.sfwRetrievalQuery.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = $filter('filter')(entityIntellisenseList, { ID: entityName }, true);
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

            scope.RetrievalMethodChange = function (flag) {
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
                    var lstData = [];
                    if (scope.model.IsChildOfGrid) {
                        var objParent = FindParent(scope.model, "sfwGridView");
                        if (objParent == null) {

                            objParent = FindParent(scope.model, "sfwListView");
                        }
                        if (objParent) {
                            var objParentField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, objParent.dictAttributes.sfwEntityField);
                            if (objParentField && objParentField.Entity) {
                                lstData = $Entityintellisenseservice.GetIntellisenseData(objParentField.Entity, "", "", true, false, true, false, false, false);
                            }
                        }
                    }
                    else {
                        lstData = $Entityintellisenseservice.GetIntellisenseData(scope.formobject.dictAttributes.sfwEntity, "", "", true, false, true, false, false, false);
                    }
                    var lsttempData = [];
                    var objMethod;
                    if (lstData) {
                        //angular.forEach(lstData, function (item) {
                        //    if (!objMethod) {
                        //        if (item.ID == strObjectMethod) {
                        //            objMethod = item;
                        //        }
                        //    }
                        //});
                        if (scope.model.dictAttributes.sfwRetrievalMethod) {
                            for (var i = 0; i < lstData.length; i++) {
                                if (lstData[i].ID == scope.model.dictAttributes.sfwRetrievalMethod) {
                                    dialogScope.selectedCurrentQuery = lstData[i];
                                }
                            }
                        }
                    }


                    //var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    //var lst = $filter('filter')(entityIntellisenseList, { ID: scope.formobject.dictAttributes.sfwEntity }, true);
                    //if (lst && lst.length > 0) {
                    //    if (lst[0].ObjectMethods) {
                    //        scope.lstMethods = lst[0].ObjectMethods;
                    //    }
                    //}

                }

                dialogScope.objNewDialog = $rootScope.showDialog(dialogScope, "Retrieval Parameters", "Form/views/RetrievalParameters.html", {
                    width: 700, height: 500
                });

                //dialogScope.RetrievalParameterDialog = ngDialog.open({
                //    template: "Form/views/RetrievalParameters.html",
                //    scope: dialogScope,
                //    closeByDocument: false,
                //    className: 'ngdialog-theme-default ngdialog-theme-custom'
                //});
                dialogScope.$on("onRetrievalParameterClick", function (event, data) {
                    if (scope.model.Name == "sfwCascadingDropDownList") {
                        // scope.model.dictAttributes.sfwCascadingRetrievalParameters = data;

                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", data);
                        createGlobalParameterList(scope.model.dictAttributes.sfwCascadingRetrievalParameters, "sfwCascadingRetrievalParameters");
                    }
                    else {
                        //scope.model.dictAttributes.sfwParameters = data;
                        $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", data);
                        createGlobalParameterList(scope.model.dictAttributes.sfwParameters, "sfwParameters");
                    }
                    event.stopPropagation();
                });
            };
            scope.$on("onRetrievalParameterClick", function (event, data) {
                if (scope.model.Name == "sfwCascadingDropDownList") {
                    //  scope.model.dictAttributes.sfwCascadingRetrievalParameters = data;
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwCascadingRetrievalParameters, scope.model.dictAttributes, "sfwCascadingRetrievalParameters", data);
                    createGlobalParameterList(scope.model.dictAttributes.sfwCascadingRetrievalParameters, "sfwCascadingRetrievalParameters");
                }
                else {
                    //  scope.model.dictAttributes.sfwParameters = data;
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwParameters, scope.model.dictAttributes, "sfwParameters", data);
                    createGlobalParameterList(scope.model.dictAttributes.sfwParameters, "sfwParameters");
                }
                event.stopPropagation();
            });

            scope.parameterList = [];
            var createGlobalParameterList = function (vlaue, prop) {
                $.connection.hubForm.server.getGlobleParameters().done(function (data) {
                    scope.$evalAsync(function () {
                        if (data) {
                            angular.forEach(data.Elements, function (paramObj) {
                                if (paramObj && paramObj.dictAttributes.ID) {
                                    var item = "~" + paramObj.dictAttributes.ID;
                                    scope.parameterList.push(item);
                                }
                            });
                            scope.validateParameters(vlaue, prop);
                        } else {
                            scope.validateParameters(vlaue, prop);
                        }
                    });
                });
            };
            var populateAllControlsId = function (mainObj) {
                angular.forEach(mainObj.Elements, function (obj) {
                    if (obj && obj.dictAttributes.ID) {
                        scope.parameterList.push(obj.dictAttributes.ID);
                    }
                    if (obj.Elements.length > 0) {
                        populateAllControlsId(obj);
                    }
                });
            };
            scope.validateParameters = function (params, prop) {
                populateAllControlsId(scope.formobject);
                var prefix = "prop-";
                if (prop == "sfwAutoParameters") {
                    prefix = "autoprop-";
                } else if (prop == "sfwCascadingRetrievalParameters") {
                    prefix = "cprop-";
                }
                if (angular.isObject(scope.model.errors)) {
                    for (var key in scope.model.errors) {
                        if (key && key.startsWith(prefix)) {
                            delete scope.model.errors[key];
                        }
                    }
                }
                if (params) {
                    var param = params.split(";");
                    for (var i = 0; i < param.length; i++) {
                        var str1 = param[i].split("=");
                        var strId = str1[str1.length - 1];
                        $ValidationService.checkValidListValue(scope.parameterList, scope.model, strId, prop, prefix + strId, "parameter value(" + strId + ") does not exists", null);
                    }
                }
            };
            scope.LoadClientVisibilitySource = function () {
                function iterator(itm) {
                    if (itm.indexOf("=") > 0) {
                        var param = itm.substring(itm.indexOf("=") + 1);
                        scope.lstColumns.push({ CodeID: param, Description: param });
                    }
                }
                if (scope.model && scope.model.retrievalType && scope.model.retrievalType == "Method") {
                    scope.lstColumns = [];
                    scope.lstColumns.push({ CodeID: "", Description: "" });
                    if (scope.model && scope.model.dictAttributes && scope.model.dictAttributes.sfwRetrievalControls) {
                        var lst = scope.model.dictAttributes.sfwRetrievalControls.split(";");
                        if (lst && lst.length > 0) {

                            angular.forEach(lst, iterator);
                        }
                    }
                }
            };


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
                        //dialogScope.RetrievalControlsDialog = ngDialog.open({
                        //    template: "Form/views/RetrievalControls.html",
                        //    scope: dialogScope,
                        //    closeByDocument: false,
                        //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                        //});

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
            }()); // IFEE 


            scope.getRetrievalControl_dialog = function () {
                browseRetrievalControls.setnewScope();
                browseRetrievalControls.loadDialog();
            };
            scope.$on('onRetrievalControlClick', function (event, data) {
                browseRetrievalControls.afterDialog(data);
            });
            scope.RetrievalQueryChange = function () {
                scope.lstColumns = [];
                if (scope.model.retrievalType == "Query") {
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
                createGlobalParameterList(scope.model.dictAttributes.sfwAutoParameters, "sfwAutoParameters");
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
                        //dialogScope.RetrievalControlsDialog = ngDialog.open({
                        //    template: "Form/views/RetrievalControls.html",
                        //    scope: dialogScope,
                        //    closeByDocument: false,
                        //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                        //});

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
                        //dialogScope.AutoCompleteColumnsDialog = ngDialog.open({
                        //    template: "Form/views/AutoCompleteColumns.html",
                        //    scope: dialogScope,
                        //    closeByDocument: false,
                        //    className: 'ngdialog-theme-default ngdialog-theme-custom',
                        //});

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

            //#region Load Common Properties Template (Revised)
            var isLoad = true;
            scope.getGroupList = function (event) {
                scope.lstGroupList = [];
                if (scope.formobject.objExtraData && scope.formobject.objExtraData.lstGroupsList[0]) {


                    scope.lstGroupList = scope.formobject.objExtraData.lstGroupsList[0].Elements;

                    //if (lstGroupList && lstGroupList.length > 0) {
                    //    setSingleLevelAutoComplete($(event.currentTarget), lstGroupList[0].Elements, scope, "ID");
                    //}
                }
                scope.lstGroupList.splice(0, 0, { dictAttributes: { ID: "" } });
            };
            //var parent = $(element).parent();
            //parent.html(scope.getTemplate());
            //$compile(parent.contents())(scope);
            scope.setEntityName = function () {
                if (scope.formobject && scope.formobject.SelectedControl && scope.formobject.SelectedControl.IsGridChildOfListView) {
                    if (scope.model && scope.model.Name == "sfwGridView") {
                        var listViewparent = FindParent(scope.model, "sfwListView");
                        if (listViewparent) {
                            var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, listViewparent.dictAttributes.sfwEntityField);
                            if (object) {
                                scope.entityName = object.Entity;
                            }
                        }
                    }
                    else {
                        scope.entityName = scope.entityTreeName;
                    }
                }
                else {
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
                    } else if (scope.formobject && scope.model && scope.model.IsChildOfGrid) {
                        scope.entityName = $GetGridEntity.getEntityName(scope.formobject, scope.model);
                    } else if (scope.formobject && scope.model && scope.model.dictAttributes.sfwEntityField && scope.model.dictAttributes.sfwEntityField.indexOf('.') > -1) {
                        var object = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, scope.model.dictAttributes.sfwEntityField);
                        if (object && object.Entity) {
                            scope.entityName = object.Entity;
                        }
                    }
                    else {
                        scope.entityName = scope.entityTreeName;
                    }
                }

            };

            function watchOnModel(newVal, oldVal) {
                if (newVal) {

                    //#region Call Init Method
                    if (isLoad) {
                        scope.Init();
                        isLoad = false;
                    }
                    //#endregion
                    //#region Call Init Method for control
                    scope.setEntityName();
                    scope.InitByControl();
                    //#endregion

                }
            }

            var unwatch = scope.$watch('model', watchOnModel);

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
                if (scope.IsReport) {
                    scope.IsShowVisibleRuleProperty = false;
                }
                else if (scope.IsCorrespondence && scope.model && scope.model.Name != "sfwColumn") {
                    scope.IsShowVisibleRuleProperty = true;
                }
                else if (scope.IsLookup) {
                    if (scope.model.IsChildOfGrid && scope.objGridView) {
                        scope.IsShowVisibleRuleProperty = true;
                    }
                    else {
                        scope.IsShowVisibleRuleProperty = false;
                    }
                }
                else {
                    if (scope.model && scope.model.Name != "sfwLiteral" && scope.model.Name != "RequiredFieldValidator" && scope.model.Name != "CompareValidator" && scope.model.Name != "sfwSoftErrors" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwWizard" && scope.model.Name != "udc" && scope.model.Name != "sfwRuleViewer" && scope.model.Name != 'TemplateField') {
                        if (scope.model.Name == "sfwTable" || (scope.model && scope.model.Name != "sfwColumn" && scope.model.Name != "hr" && scope.model.Name != "br" && scope.model.Name != "sfwCRViewer" && scope.model.Name != "sfwFileLayout" && scope.model.Name != "sfwFileUpload" && scope.model.Name != "sfwTabContainer" && scope.model.Name != "sfwWizardProgress") || scope.model.Name == "RangeValidator" || scope.model.Name == "ValidationSummary" || scope.model.Name == "RegularExpressionValidator" || scope.model.Name == "sfwTabSheet" || scope.model.Name == "sfwCheckBoxList" || scope.model.Name == "sfwRadioButtonList" || scope.model.Name == "sfwHyperLink" || scope.model.Name == "sfwTable" || scope.model.Name == "sfwGridView") {
                            if (scope.model.IsChildOfGrid && scope.objGridView) {
                                //if (scope.objGridView.dictAttributes.sfwDatasourceType == "SystemQuery" || scope.objGridView.dictAttributes.sfwDatasourceType == "CustomQuery") {
                                //    scope.IsShowVisibleRuleProperty = false;
                                //}
                                //else {
                                scope.IsShowVisibleRuleProperty = true;
                                //}
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
                if (scope.model && scope.model.Name != "hr" && scope.model.Name != "sfwToolTipButton" && scope.model.Name != "br" && scope.model.Name != "TemplateField" && scope.model.Name != "sfwTable" && scope.model.Name != "sfwGridView" && scope.model.Name != "udc" && scope.model.Name != "sfwRuleViewer" && scope.model.Name != "sfwJSONData") {
                    scope.IsShowCssClassProperty = true;
                }
                else {
                    scope.IsShowCssClassProperty = false;
                }
            };

            scope.IsShowCustomAttributePropertyVisible = function () {
                if (scope.IsReport) {
                    scope.IsShowCustomAttributeProperty = (scope.model && ["sfwDropDownList", "sfwImageButton", "sfwCascadingDropDownList", "sfwRadioButtonList"].indexOf(scope.model.Name) > -1) ? true : false;
                }
                else if (scope.IsCorrespondence) {
                    scope.IsShowCustomAttributeProperty = true;
                }
                else {
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
                    scope.IsShowCustomAttributeProperty = true;

                }
            };
            //#endregion

            //#region User Control Methods
            scope.onUserControlTextChange = function (event) {
                if (scope.model.dictAttributes.Name && scope.model.errors && !scope.model.errors.invalid_active_form) {
                    var fileList = [];
                    var obj = { FileName: scope.model.dictAttributes.Name, ID: scope.model.dictAttributes.ID };
                    fileList.push(obj);
                    $.connection.hubForm.server.getUserControlModel(fileList, "").done(function (udcFileList) {
                        var formScope = getCurrentFileScope();
                        formScope.receiveUcMainTable(udcFileList);
                    });
                } else {
                    scope.model.UcChild = [];
                }
            };
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
                    if (!scope.model.hasOwnProperty("errors")) {
                        scope.model.errors = {};
                    }

                    scope.model.errors.series_errors = {};

                    for (var i = 0; i < newScope.chartSeries.Elements.length; i++) {
                        var objSeries = newScope.chartSeries.Elements[i];
                        if (objSeries.errors && !angular.equals({}, objSeries.errors)) {
                            for (var prop in objSeries.errors) {
                                if (!scope.model.errors.series_errors.hasOwnProperty(prop)) {
                                    scope.model.errors.series_errors[prop + "_" + objSeries.dictAttributes.Name] = objSeries.errors[prop];
                                }
                            }
                        }
                    }
                    if (angular.equals({}, scope.model.errors.series_errors)) {
                        var keys = Object.keys(scope.model.errors);
                        var len = keys.length;
                        if (scope.model.errors.hasOwnProperty("series_errors") && len == 1) {
                            $ValidationService.checkValidListValue([], scope.model, "", "series_errors", "series_errors", "", undefined);
                        }
                        delete scope.model.errors.series_errors;
                    }

                    dialog.close();
                };
                newScope.selectSeries = function (obj) {
                    newScope.ObjSeriesModel = obj;
                    newScope.ObjSeriesModel.dictAttributes.ChartType = scope.model.dictAttributes.ChartType;
                };
                newScope.validateSeriesNameNotEmpty = function () {
                    newScope.strError = null;
                    if (newScope.chartSeries && newScope.chartSeries.Elements) {
                        for (var i = 0; i < newScope.chartSeries.Elements.length; i++) {
                            if (!newScope.chartSeries.Elements[i].dictAttributes.Name) {
                                newScope.strError = "Error: Please enter a Series Name.";
                                break;
                            }
                        }
                    }
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
                newScope.validateSeriesNameNotEmpty();
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
                    var temp = "";
                    if (scope.model.dictAttributes.sfwEntityField && scope.formobject.dictAttributes.sfwEntity) {
                        temp = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.formobject.dictAttributes.sfwEntity, scope.model.dictAttributes.sfwEntityField);
                    }
                    if (temp) {
                        newColumnScope.ParentEntityName = temp.Entity;
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
                            $rootScope.openFile(objfile, false);
                        });
                    }
                };

                newScope.OpenTooltipParams = function () {
                    var newParamScope = newScope.$new();
                    newScope.objTooltipParamsVM = $rootScope.showDialog(newParamScope, "Set Tooltip Parameters", "Form/views/SetToolTipParameters.html", { width: 500, height: 420 });

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

            scope.selectQueryClick = function (aQueryID, apropertyName) {
                if (aQueryID && aQueryID != "" && aQueryID.contains(".")) {
                    var query = aQueryID.split(".");
                    $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
                }

            };
            scope.NavigateToEntityQueryFromDetailQueryID = function (aQueryID) {
                if (aQueryID && scope.IsLookup && scope.IsSearchCriteriaSelected && scope.formobject && scope.formobject.Elements) {
                    var objInitialLoad = scope.formobject.Elements.filter(function (x) { return x.Name && x.Name == "initialload"; });
                    if (objInitialLoad && objInitialLoad.length > 0 && objInitialLoad[0].Elements) {
                        var lstQuery = objInitialLoad[0].Elements.filter(function (x) { return x.dictAttributes && x.dictAttributes.ID == aQueryID; });
                        if (lstQuery && lstQuery.length > 0 && lstQuery[0].dictAttributes.sfwQueryRef && lstQuery[0].dictAttributes.sfwQueryRef.contains(".")) {
                            var query = lstQuery[0].dictAttributes.sfwQueryRef.split(".");
                            $NavigateToFileService.NavigateToFile(query[0], "queries", query[1]);
                        }
                    }
                }
            };
            scope.selectMethodClick = function (aMethodID) {
                if (aMethodID && aMethodID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "objectmethods", aMethodID);
                }
            };
            scope.selectExpressionClick = function (aExpressionID) {
                if (aExpressionID && aExpressionID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "attributes", aExpressionID);
                }
            };
            scope.selectGroupClick = function (aGroupID) {
                if (aGroupID && aGroupID != "") {
                    $NavigateToFileService.NavigateToFile(scope.formobject.dictAttributes.sfwEntity, "groupslist", aGroupID);
                }
            };
            scope.NavigateToRuleClick = function (aRuleID) {
                if (aRuleID && aRuleID != "") {
                    hubcontext.hubMain.server.navigateToFile(aRuleID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, false);
                    });
                }
            };
            scope.NavigateToActiveForm = function (aActiveFromID, aDefaultActiveForm) {
                if (aActiveFromID && aActiveFromID != "") {
                    if (aActiveFromID.contains("=")) {
                        var temp = aActiveFromID.split(";");
                        var templstActiveForm = temp[0].split("=");
                        if (templstActiveForm && templstActiveForm.length > 1) {
                            aActiveFromID = templstActiveForm[1];
                        }
                    }
                    $rootScope.IsLoading = true;
                    hubcontext.hubMain.server.navigateToFile(aActiveFromID, "").done(function (objfile) {
                        scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $rootScope.openFile(objfile, undefined);
                        });
                    });

                } else if (aDefaultActiveForm && aDefaultActiveForm != "") {
                    $rootScope.IsLoading = true;
                    hubcontext.hubMain.server.navigateToFile(aDefaultActiveForm, "").done(function (objfile) {
                        scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $rootScope.openFile(objfile, false);
                        });
                    });
                }
            };
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

            scope.ClickCommonAdvPropsPanels = function (opt, $event) {
                if (scope.model) {
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
                                scope.objAdvanceProperties.lstAppearance = lst[0].lstAppearance;
                            }
                        }
                        else if (opt == 'Behaviour') {
                            $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Behaviour'] td").toggleClass("prop-close"); // handling collapsing icons                        
                            $(HTMLelementAdvancePropertiesTable).find("tr[category='Behaviour ']:not([header-properties-row])").toggle();
                            if (scope.objAdvanceProperties.lstBehavior == undefined) {
                                scope.objAdvanceProperties.lstBehavior = lst[0].lstBehavior;
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
                                scope.objAdvanceProperties.lstLayout = lst[0].lstLayout;
                            }
                        }
                        else if (opt == 'Navigation') {
                            $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Navigation'] td").toggleClass("prop-close"); // handling collapsing icons                        
                            $(HTMLelementAdvancePropertiesTable).find("tr[category='Navigation ']:not([header-properties-row])").toggle();
                            if (scope.objAdvanceProperties.lstNavigation == undefined) {
                                scope.objAdvanceProperties.lstNavigation = lst[0].lstNavigation;
                            }
                        }
                        else if (opt == 'Extra') {
                            $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Extra'] td").toggleClass("prop-close"); // handling collapsing icons                        
                            $(HTMLelementAdvancePropertiesTable).find("tr[category='Extra ']:not([header-properties-row])").toggle();
                            if (scope.objAdvanceProperties.lstExtra == undefined) {
                                scope.objAdvanceProperties.lstExtra = lst[0].lstExtra;
                            }
                        }
                        else if (opt == 'Misc') {
                            $(HTMLelementAdvancePropertiesTable).find("[header-properties-row][category='Misc'] td").toggleClass("prop-close"); // handling collapsing icons                        
                            $(HTMLelementAdvancePropertiesTable).find("tr[category='Misc ']:not([header-properties-row])").toggle();
                            if (scope.objAdvanceProperties.lstMisc == undefined) {
                                scope.objAdvanceProperties.lstMisc = lst[0].lstMisc;
                            }
                        }
                        scope.SetDataToWebControl();
                    }
                }
            };

            scope.selectActiveFormClick = function (activeFromID) {
                if (activeFromID) {
                    hubcontext.hubMain.server.navigateToFile(activeFromID, "").done(function (objfile) {
                        $rootScope.openFile(objfile, false);
                    });
                }
            };

            //#region Correspondence Query ID Section



            scope.onCorrespondenceQueryIDKeyDown = function (event) {
                var input = $(event.target);
                scope.queryIdInput = input;
                if (event && event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                    $(input).autocomplete("search", $(input).val());
                    event.preventDefault();
                }
                else {
                    createCorrespondenceQueryIDList(input);
                }
            };

            scope.showBookMarksQueryIDIntellisenseList = function (event) {

                var inputElement;
                inputElement = $(event.target).prevAll("input[type='text']");
                scope.queryIdInput = inputElement;
                inputElement.focus();
                if (inputElement) {
                    createCorrespondenceQueryIDList(inputElement);
                    if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                }
                if (event) {
                    event.stopPropagation();
                }
            };


            var createCorrespondenceQueryIDList = function (input) {
                var lst = getQueryBookMarksID(scope.formobject);
                scope.queryIdList = lst;
                if (lst && lst.length > 0) {
                    setSingleLevelAutoComplete(input, lst);
                }

            };

            //#endregion

            // #region validation
            scope.validateId = function (model) {
                if (scope.$parent && scope.$parent.validationErrorList) {
                    if (model.hasOwnProperty("IsChildOfGrid") && model.IsChildOfGrid) {
                        $ValidationService.validateID(model, scope.$parent.validationErrorList, model.dictAttributes.ID, true);
                    } else if (model.Name != "sfwTable") {
                        $ValidationService.validateID(model, scope.$parent.validationErrorList, model.dictAttributes.ID);
                    }
                }
            };
            scope.validateDuplicateId = function () {
                if (scope.$parent && scope.$parent.iterateModel) {
                    var tableData;
                    if (scope.$parent.formTableModel) tableData = scope.$parent.formTableModel;
                    else if (scope.$parent.objQueryForm) tableData = scope.$parent.objQueryForm;
                    scope.$parent.iterateModel(tableData);
                }
            };

            // #endregion
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


            scope.onchangeOfEntityField = function () {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.selectControl) {
                    filescope.selectControl(scope.model, event);
                }
            }

            scope.ClearSelectColVisibleRule = function () {
                if (scope.model.dictAttributes.sfwSelectColVisibleRule && (scope.model.dictAttributes.sfwSelection != "Many" && scope.model.dictAttributes.sfwSelection != "One")) {
                    $rootScope.EditPropertyValue(scope.model.dictAttributes.sfwSelectColVisibleRule, scope.model.dictAttributes, "sfwSelectColVisibleRule", "");
                }
            };

            //#region on Change of parent Grid
            scope.onChangeOfParentGrid = function () {
                var filescope = getCurrentFileScope();
                if (filescope && filescope.selectControl) {
                    if (scope.model && scope.model.dictAttributes.sfwEntityField) {
                        scope.model.dictAttributes.sfwEntityField = null;
                    }
                    filescope.selectControl(scope.model, event);
                }

            };

            scope.onChangeOfMethodType = function (methodType) {
                $rootScope.UndRedoBulkOp("Start");
                scope.showRules = false;
                scope.showObjectMethod = false;
                scope.showXmlMethod = false;
                if (methodType == "ObjectMethod") {
                    scope.showObjectMethod = true;
                    scope.Title = "Object Method";
                }
                else if (methodType == "XmlMethod") {
                    scope.showXmlMethod = true;
                    scope.Title = "Xml Method";
                }
                else if (methodType == "Rule") {
                    scope.showRules = true;
                    scope.Title = "Rule";
                }

                scope.model.dictAttributes.sfwObjectMethod = '';
                scope.model.dictAttributes.sfwNavigationParameter = '';

                $rootScope.UndRedoBulkOp("End");
            };

            scope.onChangeOfMethodType = function (methodType) {
                $rootScope.UndRedoBulkOp("Start");
                scope.showRules = false;
                scope.showObjectMethod = false;
                scope.showXmlMethod = false;
                if (methodType == "ObjectMethod") {
                    scope.showObjectMethod = true;
                    scope.Title = "Object Method";
                }
                else if (methodType == "XmlMethod") {
                    scope.showXmlMethod = true;
                    scope.Title = "Xml Method";
                }
                else if (methodType == "Rule") {
                    scope.showRules = true;
                    scope.Title = "Rule";
                }

                scope.model.dictAttributes.sfwObjectMethod = '';
                scope.model.dictAttributes.sfwNavigationParameter = '';

                $rootScope.UndRedoBulkOp("End");
            };

            ////#endregion

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
        templateUrl: 'Form/views/ControlProperties/ControlCommonProperties.html'
    };
}]);

app.service('share', [function () {
    return {};
}]);

app.directive('addcontroltemplate', ['$compile', '$rootScope', 'ngDialog', '$EntityIntellisenseFactory', '$GetEntityFieldObjectService', '$ValidationService', function ($compile, $rootScope, ngDialog, $EntityIntellisenseFactory, $GetEntityFieldObjectService, $ValidationService) {

    var getmenuoptionsTemplate = function (formtype, IsLookupCriteriaEnabled) {
        var template = "";
        if ((formtype == 'Lookup' && IsLookupCriteriaEnabled) || formtype == "Correspondence") { }
        else {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwLabel\")'>Label</span>";
        }
        if (formtype == "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"Panel\")'>Panel</span>";
        }

        template += "<span ng-click='OnAddControlToCellClick(\"Caption\")'>Caption</span>";

        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip" && formtype != "Lookup") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwHyperLink\")'>HyperLink</span>";
        }
        if (formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwLiteral\")'>Literal</span>";
        }
        if (formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwTextBox\")'>TextBox</span>";

            template += "<span ng-click='OnAddControlToCellClick(\"sfwDropDownList\")'>DropDownList</span>";

            if (formtype != "Report" && formtype != "Correspondence") {
                template += "<span ng-click='OnAddControlToCellClick(\"sfwMultiSelectDropDownList\")'>MultiSelectDropDownList</span>";
            }

            template += "<span ng-click='OnAddControlToCellClick(\"sfwCheckBox\")'>CheckBox</span>";
            if (formtype != "Lookup") {
                template += "<span ng-click='OnAddControlToCellClick(\"sfwCheckBoxList\")'>CheckBoxList</span>";

                template += "<span ng-click='OnAddControlToCellClick(\"sfwRadioButtonList\")'>RadioButton List</span>";
            }
        }
        if (formtype == "Report") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwCascadingDropDownList\")'>CascadingDropDownList</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwRadioButton\")'>Radio Button</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwImageButton\")'>Retrieve Button</span>";
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwButton\")'>Button</span>";
        }

        if (formtype == "Correspondence") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwRadioButton\")'>Radio Button</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"Panel\")'>Panel</span>";
        }

        if (formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"RequiredFieldValidator\")'>RequiredFieldValidator</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"CompareValidator\")'>CompareValidator</span>";
        }

        if (formtype == "Lookup") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwUserDefaults\")'>User Defaults</span>";

        }
        else {
            if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip") {
                template += "<span ng-click='OnAddControlToCellClick(\"sfwCRViewer\")'>CR Viwer</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwFileLayout\")'>File Layout</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwFileUpload\")'>File Upload</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwSoftErrors\")'>Soft Errors</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwEmployerSoftErrors\")'>Employer Soft Errors</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwRadioButton\")'>Radio Button</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"Panel\")'>Panel</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwListPicker\")'>List Picker</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwSourceList\")'>Source List</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwTargetList\")'>Target List</span>";
            }
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"hr\")'>Horizontal Rule</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"br\")'>Break</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwCascadingDropDownList\")'>CascadingDropDownList</span>";
            if (formtype != "Lookup") {
                template += "<span ng-click='OnAddControlToCellClick(\"sfwMultiCorrespondence\")'>Multi Correspondence</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"DialogPanel\")'>Dialog Panel</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwImage\")'>Image</span>";
                template += "<span ng-click='OnAddControlToCellClick(\"sfwOpenDetail\")'>Open Detail</span>";
            }
        }

        if (formtype == "Wizard") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwCommandButton\")'>Command Button</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwWizardProgress\")'>Wizard Progress</span>";
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip" && formtype != "Lookup") {
            template += "<span ng-click='OnAddControlToCellClick(\"TabContainer\")'>Tab Container</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwListBox\")'>ListBox</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"RangeValidator\")'>Range Validator</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"RegularExpressionValidator\")'>Regular Expression Validator</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"ValidationSummary\")'>Validation Summary</span>";
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip") {
            if (formtype != "Lookup") {
                template += formtype != 'UserControl' ? "<span ng-click='OnAddControlToCellClick(\"UserControl\")'>UserControl</span>" : '';
                template += "<span ng-click='OnAddControlToCellClick(\"sfwRuleViewer\")'>RuleViewer</span>";
            }
            template += "<span ng-click='OnAddControlToCellClick(\"sfwSwitchCheckBox\")'>SwitchCheckBox</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwSlider\")'>Slider</span>";
            template += "<span ng-click='OnAddControlToCellClick(\"sfwKnob\")'>Knob</span>";
        }
        if (formtype != "Report" && formtype != "Lookup" && formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwQuestionnairePanel\")'>Questionnaire Panel</span>";
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwDateTimePicker\")'>DateTime Picker</span>";
        }
        if (formtype != "Report" && formtype != "Correspondence" && formtype != "Lookup" && formtype != "Tooltip") {
            template += "<span ng-click='OnAddControlToCellClick(\"sfwJSONData\")'>JSON Data</span>";
        }

        template += "<span ng-click='OnAddControlToCellClick(\"sfwButtonGroup\")'>Button Group</span>";
        return template;
    };

    return {
        restrict: "A",
        replace: true,
        scope: {
            item: '=',
            formodel: '='
        },
        link: function (scope, element, attrs) {

            if (angular.isArray(scope.item.Elements)) {
                var type = scope.formodel.dictAttributes.sfwType;
                var IsLookupCriteriaEnabled = scope.formodel.IsLookupCriteriaEnabled;
                element.html(getmenuoptionsTemplate(type, IsLookupCriteriaEnabled));

                $compile(element.contents())(scope);
            }

            //#region Add New Control
            var RepeaterControldialog;
            scope.objRepeaterControl;

            scope.OnAddControlToCellClick = function (aParam) {
                if (aParam) {
                    $(".plusicon").remove();
                    var strControlName = aParam;
                    if (strControlName == "sfwButton") {

                        var newScope = scope.$new(true);
                        newScope.item = scope.item;
                        newScope.formodel = scope.formodel;

                        newScope.CreateButtonDialog = $rootScope.showDialog(newScope, "Button Details", "Form/views/CreateButtonWizard/CreateButtonControl.html", {
                            width: 660, height: 550
                        });


                    }
                    else if (strControlName == "sfwListView") {
                        var strID = CreateControlID(scope.formodel, "RepeaterViewPanel", "sfwListView");
                        var prefix = "swc";
                        scope.objRepeaterControl = { Name: "sfwListView", value: '', prefix: prefix, dictAttributes: { ID: strID }, Elements: [], Children: [] };
                        scope.ParentEntityName = formodel.dictAttributes.sfwEntity;
                        scope.objRepeaterControl.selectedobjecttreefield;
                        scope.objRepeaterControl.lstselectedobjecttreefields = [];
                        //RepeaterControldialog = ngDialog.open({
                        //    template: 'RepeaterControlTemplate',
                        //    scope: scope,
                        //    closeByDocument: false
                        //});
                        var dialogScope = scope.$new(true);
                        //dialogScope = scope;
                        dialogScope.objRepeaterControl = scope.objRepeaterControl;
                        dialogScope.objRepeaterControl.LstDisplayedEntities = [];
                        dialogScope.ParentEntityName = scope.ParentEntityName;
                        dialogScope.RepeaterControldialog = $rootScope.showDialog(dialogScope, "Repeater Control", "Form/views/RepeaterControlTemplate.html", {
                            width: 500, height: 600
                        });
                    }
                    else {
                        scope.AddControlToCell(strControlName);
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

                    var objListTableModel = {
                        Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    objListTableModel.ParentVM = scope.objRepeaterControl;
                    var strCtrlId = CreateControlID(scope.formodel, "NewPage", "sfwTable");
                    objListTableModel.dictAttributes.ID = strCtrlId;

                    var sfxRowModel = {
                        Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    sfxRowModel.ParentVM = objListTableModel;

                    var newSfxCellModel = {
                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = {
                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = {
                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
                    newSfxCellModel.ParentVM = sfxRowModel;
                    sfxRowModel.Elements.push(newSfxCellModel);

                    newSfxCellModel = {
                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                    };
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
            scope.AddControlToCell = function (cntrlName, sfxControlModel) {
                if (!sfxControlModel) {
                    sfxControlModel = CreateControl(scope.formodel, scope.item, cntrlName);
                }

                if (sfxControlModel != null && sfxControlModel.Name != "udc") {
                    if (sfxControlModel.Name == "sfwDialogPanel" || sfxControlModel.Name == "sfwPanel") {
                        sfxControlModel.initialvisibilty = true;
                        sfxControlModel.isLoaded = true;
                    }
                    $rootScope.PushItem(sfxControlModel, scope.item.Elements);
                    SetFormSelectedControl(scope.formodel, sfxControlModel);
                    //this.ObjVM.DesignVM.CheckAndUpdateSelectedControlStatus(this.MirrorElements[this.MirrorElements.Count - 1] as SfxControlVM, false);
                    //this.PopulateObjectID(this.ObjVM.Model, sfxControlModel);

                }

                //#region Add User Control
                if (sfxControlModel != undefined && sfxControlModel.Name == "udc") {

                    var newScope = scope.$new();
                    newScope.formodel = scope.formodel;
                    newScope.objSetUCProp = {
                        StrId: sfxControlModel.dictAttributes.ID, StrName: '', StrEntityField: '', StrResource: ''
                    };
                    newScope.objSetUCProp.IsAddedFromObjectTree = false;

                    newScope.onUserControlOkClick = function () {
                        sfxControlModel.dictAttributes.ID = newScope.objSetUCProp.StrId;
                        sfxControlModel.dictAttributes.Name = newScope.objSetUCProp.StrName;
                        if ((scope.formodel.dictAttributes.sfwEntity != undefined && scope.formodel.dictAttributes.sfwEntity != "") && (newScope.objSetUCProp.StrEntityField != undefined && newScope.objSetUCProp.StrEntityField != "")) {
                            if (newScope.objSetUCProp.StrEntityField.match("^" + scope.formodel.dictAttributes.sfwEntity)) {
                                sfxControlModel.dictAttributes.sfwEntityField = scope.formodel.dictAttributes.sfwEntity + "." + newScope.objSetUCProp.StrEntityField;
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
                        $rootScope.PushItem(sfxControlModel, scope.item.Elements);
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
                            CheckforDuplicateID(scope.formodel, newScope.objSetUCProp.StrId, lstIds);
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
            //#endregion
        },
    };
}]);

app.directive("newbuttoncommonproptemplate", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/CreateButtonWizard/NewButtonCommonProperties.html',
    };
}]);

app.directive("navigationparamtreetemplatefornewbutton", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/CreateButtonWizard/NavigationParamTreeForNewButton.html',
    };
}]);

app.directive("controlistchildtreetemplate", ["$compile", function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/ClientVisibilityChildTree.html',
    };
}]);

app.directive("customattributetreetemplate", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Common/views/CustomAttributeTreeTemplate.html',
    };
}]);

app.directive("commonparametertree", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/CommonParameterTree.html',
    };
}]);

app.directive("clientvisibilitychildtree", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        template: function () {
            var temp = '<div>'
            temp += '<ul ng-if="field.Items.length > 0" ng-show="field.IsExpanded" role="group">';
            temp += '<li ng-repeat="field in field.Items" ng-click="SelectFieldClick(field, $event)" ng-mousedown="SelectFieldClick(field, $event)" class="common-tree-list client-visibility-tree-option">';
            temp += '<i ng-class="field.IsExpanded?\'fa fa-minus\':\'fa fa-plus\'" ng-click="ExpandCollapsedControl(field,$event)" ng-show="field.Items.length>0" class="client-visibility-icon"></i>';
            temp += '<input type="checkbox" ng-model="field.IsSelected" ng-show="field.IsCheckBoxVisible" />';
            temp += '<span ng-bind="field.Text" class="client-visibility-tree-option-text"></span>';
            temp += '<div clientvisibilitychildtree ng-if="field.Items.length > 0"></div>';
            temp += '</li>';
            temp += '</ul>';
            temp += '</div>';

            return temp;
        },
    };
}]);

app.directive("customattributechildtree", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/CustomAttributeTree.html',
    };
}]);

app.directive("chartcolumndroppable", [function () {
    return {
        restrict: 'A',
        scope: {
            dropdata: '=',
            lstcolumns: '=',
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            el.addEventListener('drop', function (e) {
                var strData = e.dataTransfer.getData("Text");
                if (strData == "" && lstEntityTreeFieldData != null) {
                    var obj = lstEntityTreeFieldData;
                    var Id = obj[0];
                    var DisplayName = obj[1];
                    var DataType = obj[2];
                    var data = obj[3];//JSON.parse(obj[3]);
                    var isparentTypeCollection = obj[4];
                    var fullpath = Id;
                    if (DisplayName != "") {
                        fullpath = DisplayName + "." + Id;
                    }
                    dragDropData = fullpath;
                } else {
                    lstEntityTreeFieldData = null;
                }
                if (dragDropData) {
                    scope.$apply(function () {
                        if (DataType != "Collection" && DataType != "CDOCollection" && DataType != "Object" && DataType != "List") {
                            var flag = true;
                            if (scope.lstcolumns.length > 0) {
                                for (var i = 0; i < scope.lstcolumns.length; i++) {
                                    if (scope.lstcolumns[i].Property == dragDropData) {
                                        flag = false;
                                    }
                                }
                            }
                            if (flag) {
                                scope.lstcolumns.push({ Property: dragDropData });
                            }
                        }
                        dragDropData = null;
                    });
                }
                if (e.preventDefault) {
                    e.preventDefault();
                }
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            });
            $(el).on('dragenter', function (event) {
                event.preventDefault();
                //  $(this).html('drop now').css('background','blue');
            });
            $(el).on('dragleave', function () {
                // $(this).html('drop here').css('background','red');
            });
            $(el).on('dragover', function (event) {
                event.preventDefault();
            });
        }
    };
}]);

app.directive("basequeryparamdroppable", ["$rootScope", function ($rootScope) {
    return {
        restrict: 'A',
        scope: {
            dropdata: '=',
            dragdata: '=',
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
                var strData = e.dataTransfer.getData("Text");
                if (strData == "" && lstEntityTreeFieldData != null) {
                    var obj = lstEntityTreeFieldData;
                    var Id = obj[0];
                    var DisplayName = obj[1];
                    var DataType = obj[2];
                    var data = obj[3];//JSON.parse(obj[3]);
                    var isparentTypeCollection = obj[4];
                    var fullpath = Id;
                    if (DisplayName != "") {
                        fullpath = DisplayName + "." + Id;
                    }
                } else {
                    lstEntityTreeFieldData = null;
                }
                if (scope.dragdata && strData == "" && lstEntityTreeFieldData != null) {
                    scope.$evalAsync(function () {
                        //$rootScope.UndRedoBulkOp("Start");
                        // $rootScope.EditPropertyValue(scope.dropdata.dictAttributes.sfwDataType, scope.dropdata.dictAttributes, "sfwDataType", DataType);
                        scope.dropdata.dictAttributes.sfwDataType = DataType;
                        // $rootScope.EditPropertyValue(scope.dropdata.dictAttributes.sfwEntityField, scope.dropdata.dictAttributes, "sfwEntityField", fullpath);
                        scope.dropdata.dictAttributes.sfwEntityField = fullpath;
                        // $rootScope.UndRedoBulkOp("End");
                    });
                    lstEntityTreeFieldData = null;
                }

                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }, false);
        }

    };
}]);

app.directive("navigationparamchildtemplate", [function () {
    return {
        restrict: 'A',
        replace: true,
        scope: false,
        templateUrl: 'Form/views/NavigationParamChildTree.html',
    };
}]);


app.directive('addrowsandcolumnstotable', ["$rootScope", function ($rootScope) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: "Form/views/AddOrDeleteCellsToTable.html",
        link: function (scope, element, attributes) {
            scope.getArrayCount = function (count, param) {
                var lst = [];
                if (count && (count + 5) > 10) {
                    count = count + 5;
                } else {
                    count = 10;
                }
                if (param && param == "Delete") {
                    count = count + 1;
                }
                for (var i = 1; i <= count; i++) {
                    lst.push(i);
                }

                return lst;
            };

            scope.AddRowsAndColumns = function (objChild, event) {
                scope.objRowAndColumnIndex = {};
                scope.objRowAndColumnIndex.RowIndex = 0;
                scope.objRowAndColumnIndex.lstDeleteRowIndex = [];
                scope.objRowAndColumnIndex.lstDeleteColumnIndex = [];
                scope.objRowAndColumnIndex.ColumnIndex = 0;
                scope.objRowAndColumnIndex.CurrentRowIndex = 0;
                scope.objRowAndColumnIndex.CurrentColumnIndex = 0;
                scope.objFileModelForTable = {};
                if (scope.currentfile && scope.currentfile.FileType == "Correspondence") {
                    scope.objFileModelForTable = scope.objCorrespondenceDetails;
                } else if (scope.currentfile && scope.currentfile.FileType == "Report") {
                    scope.objFileModelForTable = scope.ReportModel;
                } else {
                    scope.objFileModelForTable = scope.FormModel;
                }
                scope.setMaxRowAndColumnIndex(objChild);
                if (objChild.Name == "sfwTable" && event) {
                    var divid = "tablediv_" + scope.currentfile.FileName;
                    var element = document.getElementById(divid);
                    $(element).css({
                        left: event.clientX - 50,
                        top: event.clientY,
                    });
                    element.style.display = "block";
                }
            };

            scope.setMaxRowAndColumnIndex = function (objTable) {
                if (objTable) {
                    scope.objRowAndColumnIndex.CurrentRowIndex = objTable.Elements.length;
                    for (var i = 0; i < objTable.Elements.length; i++) {
                        if (scope.objRowAndColumnIndex.CurrentColumnIndex < objTable.Elements[i].Elements.length) {
                            scope.objRowAndColumnIndex.CurrentColumnIndex = objTable.Elements[i].Elements.length;
                        }
                    }
                }
            };

            scope.onDeleteColumnOrRowCellClick = function (event, index, param) {
                if (event) {
                    if (index != 0) {
                        if (param == 'Row') {
                            if (index <= scope.objRowAndColumnIndex.CurrentRowIndex) {
                                var rowindex = scope.objRowAndColumnIndex.lstDeleteRowIndex.indexOf(index);
                                if (rowindex == -1) {
                                    scope.objRowAndColumnIndex.lstDeleteRowIndex.push(index);
                                } else {
                                    scope.objRowAndColumnIndex.lstDeleteRowIndex.splice(rowindex, 1);
                                }
                            }
                        } else if (param == 'Column') {
                            if (index <= scope.objRowAndColumnIndex.CurrentColumnIndex) {
                                var colindex = scope.objRowAndColumnIndex.lstDeleteColumnIndex.indexOf(index);
                                if (colindex == -1) {
                                    scope.objRowAndColumnIndex.lstDeleteColumnIndex.push(index);
                                } else {
                                    scope.objRowAndColumnIndex.lstDeleteColumnIndex.splice(colindex, 1);
                                }
                            }
                        }
                    }
                    event.stopPropagation();
                }
            };

            scope.isCellDelete = function (row, column) {
                var isFound = false;
                if (scope.objRowAndColumnIndex && scope.objRowAndColumnIndex.lstDeleteRowIndex.indexOf(row) > -1) {
                    isFound = true;
                }

                if (scope.objRowAndColumnIndex && scope.objRowAndColumnIndex.lstDeleteColumnIndex.indexOf(column) > -1) {
                    isFound = true;
                }

                if (isFound) {
                    if (!(scope.objRowAndColumnIndex.CurrentRowIndex >= row && scope.objRowAndColumnIndex.CurrentColumnIndex >= column)) {
                        isFound = false;
                    }
                }
                return isFound;
            };

            scope.AddCellsFromTable1 = function (event, row, column, isDummyCell) {
                scope.clearCellsFromtable();
                scope.AddCellsFromTable(event, row, column, isDummyCell);
                if (event && (scope.objRowAndColumnIndex && (scope.objRowAndColumnIndex.lstDeleteRowIndex.length != 0 || scope.objRowAndColumnIndex.lstDeleteColumnIndex.length != 0))) {
                    event.stopPropagation();
                } else {
                    scope.setDisplayNoneToTable();
                }
            };

            scope.setDisplayNoneToTable = function () {
                var divid = "tablediv_" + scope.currentfile.FileName;
                var element = document.getElementById(divid);
                if (element && element.style.display == "block") {
                    element.style.display = "none";
                }
                if (scope.objRowAndColumnIndex) {
                    scope.objRowAndColumnIndex.lstDeleteRowIndex = [];
                    scope.objRowAndColumnIndex.lstDeleteColumnIndex = [];
                    scope.objRowAndColumnIndex.ColumnIndex = 0;
                    scope.objRowAndColumnIndex.CurrentRowIndex = 0;
                    scope.objRowAndColumnIndex.CurrentColumnIndex = 0;
                }
            };

            scope.AddCellsFromTable = function (event, row, column, isDummyCell) {
                if (scope.objRowAndColumnIndex && scope.objRowAndColumnIndex.lstDeleteRowIndex.length == 0 && scope.objRowAndColumnIndex.lstDeleteColumnIndex.length == 0) {
                    if (scope.objFileModelForTable && scope.objFileModelForTable.SelectedControl.Name == "sfwTable") {
                        scope.objRowAndColumnIndex.RowIndex = row;
                        scope.objRowAndColumnIndex.ColumnIndex = column;
                        if (!isDummyCell) {
                            $rootScope.UndRedoBulkOp("Start");
                        }
                        if (row > scope.objFileModelForTable.SelectedControl.Elements.length) {
                            for (var i = scope.objFileModelForTable.SelectedControl.Elements.length; i < row; i++) {
                                var prefix = "swc";
                                var sfxRowModel = {
                                    Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                                };
                                sfxRowModel.ParentVM = scope.objFileModelForTable.SelectedControl;
                                if (isDummyCell) {
                                    sfxRowModel.isDummy = true;
                                    scope.objFileModelForTable.SelectedControl.Elements.push(sfxRowModel);
                                } else {
                                    $rootScope.PushItem(sfxRowModel, scope.objFileModelForTable.SelectedControl.Elements);
                                }
                            }
                        } else {
                            while (row < scope.objFileModelForTable.SelectedControl.Elements.length) {
                                if (scope.objFileModelForTable.SelectedControl.Elements[scope.objFileModelForTable.SelectedControl.Elements.length - 1].isDummy) {
                                    scope.objFileModelForTable.SelectedControl.Elements.splice(scope.objFileModelForTable.SelectedControl.Elements.length - 1, 1);
                                } else {
                                    break;
                                }
                            }
                        }

                        for (var j = 0; j < scope.objFileModelForTable.SelectedControl.Elements.length; j++) {
                            if (column > scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length) {
                                for (k = scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length; k < column; k++) {
                                    var prefix = "swc";
                                    var sfxCellModel = {
                                        Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                                    };
                                    sfxCellModel.ParentVM = scope.objFileModelForTable.SelectedControl.Elements[j];
                                    if (isDummyCell) {
                                        sfxCellModel.isDummy = true;
                                        scope.objFileModelForTable.SelectedControl.Elements[j].Elements.push(sfxCellModel);
                                    } else {
                                        $rootScope.PushItem(sfxCellModel, scope.objFileModelForTable.SelectedControl.Elements[j].Elements);
                                    }
                                }
                            } else {
                                while (column < scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length) {
                                    if (scope.objFileModelForTable.SelectedControl.Elements[j].Elements[scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length - 1].isDummy) {
                                        scope.objFileModelForTable.SelectedControl.Elements[j].Elements.splice(scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length - 1, 1);
                                    } else {
                                        break;
                                    }
                                }
                            }
                        }
                        if (!isDummyCell) {
                            $rootScope.UndRedoBulkOp("End");
                        }
                    }
                }
            };

            scope.DeleteSelectedRowsAndColumns = function () {
                scope.$evalAsync(function () {
                    if (scope.objFileModelForTable && scope.objFileModelForTable.SelectedControl && scope.objFileModelForTable.SelectedControl.Name == "sfwTable" && scope.objRowAndColumnIndex && (scope.objRowAndColumnIndex.lstDeleteColumnIndex.length > 0 || scope.objRowAndColumnIndex.lstDeleteRowIndex.length > 0)) {
                        $rootScope.UndRedoBulkOp("Start");
                        if (scope.objRowAndColumnIndex.lstDeleteRowIndex && scope.objRowAndColumnIndex.lstDeleteRowIndex.length > 0) {
                            scope.objRowAndColumnIndex.lstDeleteRowIndex.sort(function (a, b) { return b - a });
                            for (var i = 0; i < scope.objRowAndColumnIndex.lstDeleteRowIndex.length; i++) {
                                var rowIndex = scope.objRowAndColumnIndex.lstDeleteRowIndex[i] - 1;
                                if (rowIndex > -1 && scope.objFileModelForTable.SelectedControl.Elements[rowIndex]) {
                                    $rootScope.DeleteItem(scope.objFileModelForTable.SelectedControl.Elements[rowIndex], scope.objFileModelForTable.SelectedControl.Elements);
                                }
                            }
                        }

                        if (scope.objRowAndColumnIndex.lstDeleteColumnIndex && scope.objRowAndColumnIndex.lstDeleteColumnIndex.length > 0) {
                            scope.objRowAndColumnIndex.lstDeleteColumnIndex.sort(function (a, b) { return b - a });
                            for (var j = scope.objFileModelForTable.SelectedControl.Elements.length - 1; j >= 0; j--) {
                                for (var k = 0; k < scope.objRowAndColumnIndex.lstDeleteColumnIndex.length; k++) {
                                    var columnIndex = scope.objRowAndColumnIndex.lstDeleteColumnIndex[k] - 1;
                                    if (columnIndex > -1 && scope.objFileModelForTable.SelectedControl.Elements[j].Elements[columnIndex]) {
                                        $rootScope.DeleteItem(scope.objFileModelForTable.SelectedControl.Elements[j].Elements[columnIndex], scope.objFileModelForTable.SelectedControl.Elements[j].Elements);
                                    }
                                }
                                if (scope.objFileModelForTable.SelectedControl.Elements[j].Elements.length == 0) {
                                    $rootScope.DeleteItem(scope.objFileModelForTable.SelectedControl.Elements[j], scope.objFileModelForTable.SelectedControl.Elements);
                                }
                            }
                        }

                        if (scope.objFileModelForTable.SelectedControl.Elements.length == 0) {
                            var prefix = "swc";
                            var sfxRowModel = {
                                Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                            };
                            sfxRowModel.ParentVM = scope.objFileModelForTable.SelectedControl;
                            $rootScope.PushItem(sfxRowModel, scope.objFileModelForTable.SelectedControl.Elements);
                            var sfxCellModel = {
                                Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                            };
                            sfxCellModel.ParentVM = sfxRowModel;
                            $rootScope.PushItem(sfxCellModel, sfxRowModel.Elements);
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                    scope.setDisplayNoneToTable();
                });
            };

            scope.clearCellsFromtable = function (event) {
                scope.objRowAndColumnIndex.ColumnIndex = 0;
                scope.objRowAndColumnIndex.RowIndex = 0;
                if (scope.objFileModelForTable && scope.objFileModelForTable.SelectedControl.Name == "sfwTable") {
                    while (scope.objFileModelForTable.SelectedControl.Elements.length > 0 && scope.objFileModelForTable.SelectedControl.Elements[scope.objFileModelForTable.SelectedControl.Elements.length - 1].isDummy) {
                        scope.objFileModelForTable.SelectedControl.Elements.splice(scope.objFileModelForTable.SelectedControl.Elements.length - 1, 1);
                    }

                    for (var i = 0; i < scope.objFileModelForTable.SelectedControl.Elements.length; i++) {
                        while (scope.objFileModelForTable.SelectedControl.Elements[i].Elements.length > 0 && scope.objFileModelForTable.SelectedControl.Elements[i].Elements[scope.objFileModelForTable.SelectedControl.Elements[i].Elements.length - 1].isDummy) {
                            scope.objFileModelForTable.SelectedControl.Elements[i].Elements.splice(scope.objFileModelForTable.SelectedControl.Elements[i].Elements.length - 1);
                        }
                    }
                }
                if (event) {
                    event.stopPropagation();
                }
            };
        }
    };
}]);