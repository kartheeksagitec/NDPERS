function getDisplayValue(item) {
    if (item != undefined) {
        var strReturn = "";
        if (item.Name == "udc") {
            strReturn = item.dictAttributes.ID;
        }
        else {
            if (item.dictAttributes && item.dictAttributes.Text) {
                strReturn = item.dictAttributes.Text;
            }
            else if (item.dictAttributes && item.dictAttributes.sfwCaption) {
                strReturn = item.dictAttributes.sfwCaption;
            }
            else if (item.dictAttributes && item.dictAttributes.sfwEntityField) {
                var str = item.dictAttributes.sfwEntityField;
                var lastdot = str.lastIndexOf(".");
                strReturn = str.substring(lastdot + 1);
            }
            else if (item.dictAttributes && item.dictAttributes.sfwDataField) {
                var str = item.dictAttributes.sfwDataField;
                var lastdot = str.lastIndexOf(".");
                strReturn = str.substring(lastdot + 1);
            }
            else if (item.dictAttributes && item.dictAttributes.sfwObjectField) {
                strReturn = item.dictAttributes.sfwObjectField;
            }
            else if (item.dictAttributes && item.dictAttributes.Title) {
                strReturn = item.dictAttributes.Title;
            }
            else if (item.dictAttributes && item.dictAttributes.ID) {
                strReturn = item.dictAttributes.ID;
            }
            else {
                strReturn = item.Name;
            }

            if (item.Name == "sfwLiteral") {
                strReturn = "|" + strReturn + "|";
            }
        }
        return strReturn;
    }
    else {
        return "";
    }
}

//#region Object Tree Methods
function GetSelectedFieldList(attributes, selectedfields) {
    angular.forEach(attributes, function (obj) {
        if (obj.IsSelected && obj.IsSelected.toLowerCase() == "true") {
            selectedfields.push(obj);
        }
        if (obj.Attributes && obj.Attributes.length > 0) {
            GetSelectedFieldList(obj.Attributes, selectedfields);
        }
    });
    return selectedfields;

}

function getDisplayedEntity(lstdisplayedentities) {
    var displayedentity;
    if (lstdisplayedentities && lstdisplayedentities.length > 0) {
        for (var i = 0; i < lstdisplayedentities.length; i++) {
            if (lstdisplayedentities[i].IsVisible) {
                displayedentity = lstdisplayedentities[i];
                break;
            }
        }
    }

    return displayedentity;
}

function ClearSelectedFieldList(attributes) {
    angular.forEach(attributes, function (obj) {
        if (obj.IsSelected && obj.IsSelected.toLowerCase() == "true") {
            obj.IsSelected = "false";
            obj.IsRecordSelected = false;
        }
        //if (obj.Attributes && obj.Attributes.length > 0) {
        //    ClearSelectedFieldList(obj.Attributes);
        //}
    });
}

function GetItemPathForEntityObject(field) {
    var itempath = field.ID;
    var parent = field.objParent;
    while (parent && !parent.IsMainEntity && parent.ID) {
        itempath = parent.ID + "." + itempath;
        parent = parent.objParent;
    }
    return itempath;
}

function GetCodeIDByValue(astrEntityName, astrFieldName, entityIntellisenseList) {
    var lstrCodeID = "";

    if (astrEntityName) {
        var lst = entityIntellisenseList.filter(function (x) { return x.ID == astrEntityName; });
        if (lst && lst.length > 0) {
            if (lst[0].Attributes.length > 0) {
                var attribute = lst[0].Attributes.filter(function (x) { return x.Value == astrFieldName; });
                if (attribute.length > 0 && attribute[0].Value) {
                    var attributeidfield = lst[0].Attributes.filter(function (x) { return x.Value == attribute[0].Value.replace("_value", "_id"); });
                    if (attributeidfield && attributeidfield.length > 0) {
                        lstrCodeID = attributeidfield[0].CodeID;
                    }
                }
            }
        }
    }
    return lstrCodeID;
}
function GetCodeID(astrEntityName, astrFieldName, entityIntellisenseList) {
    var lstrCodeID = "";
    var lstFieldNames = [];
    var lstAttributes = [];
    if (astrFieldName) {
        lstFieldNames = astrFieldName.split('.');
    }
    if (astrEntityName) {
        var lst = entityIntellisenseList.filter(function (x) { return x.ID == astrEntityName; });
        for (var i = 0; i < lstFieldNames.length; i++) {
            if (lst && lst.length > 0) {
                lstAttributes = getEntityAttributes(lst[0].EntityName, entityIntellisenseList);
                if (lstAttributes.length > 0) {
                    var attribute = lstAttributes.filter(function (x) { return x.ID == lstFieldNames[i]; });
                    if (attribute.length > 0 && attribute[0].Value) {
                        var attributeidfield = lstAttributes.filter(function (x) { return x.Value == attribute[0].Value.replace("_value", "_id"); });
                        if (attributeidfield && attributeidfield.length > 0) {
                            lstrCodeID = attributeidfield[0].CodeID;
                        }
                    }
                    if (attribute.length > 0 && attribute[0].Entity) {
                        lst = entityIntellisenseList.filter(function (x) { return x.ID == attribute[0].Entity; });
                    }
                }
            }
        }
    }
    return lstrCodeID;
}
function getEntityAttributes(entity, entityIntellisenseList) {
    var data = [];
    var entityId = entity;
    while (entityId) {
        var lst = entityIntellisenseList.filter(function (x) { return x.ID == entityId; });
        if (lst && lst.length > 0 && lst[0].Attributes.length > 0) {
            data = data.concat(lst[0].Attributes);
        }
        if (entityIntellisenseList) {
            var entity = entityIntellisenseList.filter(function (x) {
                return x.ID == entityId;
            });
            if (entity.length > 0) {
                entityId = entity[0].ParentId;
            } else {
                entityId = "";
            }
        }
    }
    return data;
}

function GetCodeIDForLookup(astrEntityName, astrFieldName, entityIntellisenseList) {
    var lstrCodeID = "";
    if (astrEntityName) {
        var lst = entityIntellisenseList.filter(function (x) { return x.ID == astrEntityName; });
        if (lst && lst.length > 0) {
            if (lst[0].Attributes.length > 0) {
                var attribute = lst[0].Attributes.filter(function (x) { return x.Value == astrFieldName; });
                if (attribute.length > 0 && attribute[0].Value) {
                    var attributeidfield = lst[0].Attributes.filter(function (x) { return x.Value == attribute[0].Value.replace("_value", "_id"); });
                    if (attributeidfield && attributeidfield.length > 0) {
                        lstrCodeID = attributeidfield[0].CodeID;
                    }
                }
            }
        }
    }
    return lstrCodeID;
}

function GetFieldFromFormObject(objvm, controltype, attribute, matchingvalue, objret) {

    if (objret && objret.length > 0) {
        return objret;
    }
    else {
        if (objvm) {
            for (var i = 0; i < objvm.Elements.length; i++) {
                var selobj = objvm.Elements[i];
                var field = selobj.Elements.filter(function (x) { return x.Name == controltype && x.dictAttributes[attribute] == matchingvalue; });
                if (field.length > 0) {
                    return field;

                }
                else {
                    objret = GetFieldFromFormObject(selobj, controltype, attribute, matchingvalue, objret);

                }
            }
        }
    }
    return objret;

}



function GetTableKeyFields(astrEntityName, entityIntellisenseList) {
    var lstrKeyFields = "";
    if (astrEntityName) {
        var lst = entityIntellisenseList.filter(function (x) { return x.ID == astrEntityName; });
        if (lst && lst.length > 0) {
            if (lst[0].Attributes.length > 0) {

                angular.forEach(lst[0].Attributes, function (attribute) {
                    var iKeyNo = attribute.KeyNo;

                    if (iKeyNo > 0) {
                        if (lstrKeyFields.length == 0)
                            lstrKeyFields = attribute.ID;
                        else
                            lstrKeyFields += "," + attribute.ID;
                    }
                });
            }
        }
    }
    return lstrKeyFields;
}

//#endregion

//#region Load Entity Fields


function PopulateAvailableFields(strProperty, sfxTable, mainItem, isCheckBoxVisible, isLookup, isNewButton) {
    var strTreeCaption = "";
    if (sfxTable) {
        angular.forEach(sfxTable.Elements, function (sfxRow) {
            angular.forEach(sfxRow.Elements, function (sfxCell) {
                if (sfxCell) {
                    angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                        if (sfxCtrl.Name == "sfwPanel") {
                            strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
                            if (strTreeCaption == "" || strTreeCaption == undefined) {
                                strTreeCaption = sfxCtrl.dictAttributes.ID;
                            }
                            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible };
                            angular.forEach(sfxCtrl.Elements, function (model) {
                                PopulateAvailableFields(strProperty, model, childItem, isCheckBoxVisible, isLookup, isNewButton);
                            });
                            if (childItem.Items.length > 0) {
                                mainItem.Items.push(childItem);
                            }
                        }
                        else if (sfxCtrl.Name == "sfwTabContainer") {
                            strTreeCaption = sfxCtrl.dictAttributes.HeaderText;
                            if (strTreeCaption == "" || strTreeCaption == undefined) {
                                strTreeCaption = sfxCtrl.dictAttributes.ID;
                            }
                            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible };
                            var objTabs = sfxCtrl.Elements[0];
                            if (objTabs) {
                                angular.forEach(objTabs.Elements, function (model) {
                                    if (model.Elements.length > 0 && model.Elements[0].Name == "sfwTable") {
                                        PopulateAvailableFields(strProperty, model.Elements[0], childItem, isCheckBoxVisible, isLookup, isNewButton);
                                    }
                                });
                            }
                            if (childItem.Items.length > 0) {
                                mainItem.Items.push(childItem);
                            }
                        }
                        else if (sfxCtrl.Name == "sfwGridView") {
                            var strGridId = sfxCtrl.dictAttributes.ID;
                            var childItem = { Text: strGridId, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible };

                            //var strDataKeys = sfxCtrl.dictAttributes.sfwDataKeyNames;
                            //if(strDataKeys)
                            //{
                            //var strDataKeyNames = strDataKeys.split(',');
                            //}

                            if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                                for (var j = 0; j < sfxCtrl.Elements[0].Elements.length; j++) {
                                    var objTempField = sfxCtrl.Elements[0].Elements[j];
                                    if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                        var objItemTempField = objTempField.Elements[0];
                                        angular.forEach(objItemTempField.Elements, function (sfxControl) {

                                            var strFieldName = "";

                                            //strFieldName = sfxCtrl.dictAttributes.sfwEntityField.trim();//[ApplicationConstants.XMLFacade.SFWDATAFIELD].Trim();

                                            strFieldName = sfxControl.dictAttributes.sfwEntityField;//[ApplicationConstants.XMLFacade.SFWDATAFIELD];
                                            if (strFieldName != undefined && strFieldName != "") {
                                                //childItem.Items.Add(new clsField(strFieldName) { IsCheckBoxVisible = true });
                                                childItem.Items.push({ Text: strFieldName, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible });
                                            }
                                        });
                                    }
                                }
                                if (childItem.Items.length > 0) {
                                    mainItem.Items.push(childItem);
                                }
                            }
                        }
                        else if (sfxCtrl.Name == "sfwWizard") {
                            var WizardSteps = sfxCtrl.Elements.filter(function (itm) { return itm.Name == "WizardSteps"; });
                            if (WizardSteps && WizardSteps.length > 0) {
                                angular.forEach(WizardSteps[0].Elements, function (sfxWizardStep) {
                                    var sfxHeaderTemplate = sfxWizardStep.Name == "HeaderTemplate" ? sfxWizardStep : undefined;
                                    if (sfxHeaderTemplate) {
                                        strTreeCaption = "HeaderTemplate";
                                        var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: false };

                                        if (sfxHeaderTemplate.Elements.length > 0) {
                                            PopulateAvailableFields(strProperty, sfxHeaderTemplate.Elements[0], childItem, isCheckBoxVisible, isLookup, isNewButton);
                                            if (childItem.Items.length > 0) {
                                                mainItem.Items.push(childItem);
                                            }
                                        }
                                    }
                                    else {
                                        angular.forEach(sfxWizardStep.Elements, function (wizardStep) {
                                            strTreeCaption = wizardStep.dictAttributes.Title;
                                            if (strTreeCaption == "" || strTreeCaption == undefined) {
                                                strTreeCaption = wizardStep.dictAttributes.ID;
                                            }
                                            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: false };
                                            if (wizardStep.Elements.length > 0) {
                                                PopulateAvailableFields(strProperty, wizardStep, childItem, isCheckBoxVisible, isLookup, isNewButton);
                                                if (childItem.Items.length > 0) {
                                                    mainItem.Items.push(childItem);
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                        }
                        else {
                            var strFieldName = "";
                            if (isLookup && !IsCriteriaField(sfxCtrl) && strProperty == "sfwDataField") {
                                strProperty = 'sfwEntityField';
                            }

                            strFieldName = sfxCtrl.dictAttributes[strProperty];
                            if (!isNewButton || (isNewButton && sfxCtrl.Name !== "sfwButton" && sfxCtrl.Name !== "sfwLinkButton" && sfxCtrl.Name !== "sfwImageButton")) {
                                var childItem = { Text: strFieldName, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible };//new clsField(strFieldName) { IsCheckBoxVisible = true };
                                if (strFieldName != undefined && strFieldName != "") {
                                    mainItem.Items.push(childItem);
                                }
                            }
                        }
                    });
                }
            });
        });
    }
}

function PopulateAvailableFieldsForFormLink(strProperty, objItems, mainItem, isCheckBoxVisible) {
    var strTreeCaption = "";
    angular.forEach(objItems.Elements, function (sfxCtrl) {
        if (sfxCtrl.Name == "items") {
            PopulateAvailableFieldsForFormLink(strProperty, sfxCtrl, mainItem);
        }
        else if (sfxCtrl.Name == "sfwPanel") {
            strTreeCaption = sfxCtrl.dictAttributes.sfwCaption;
            if (strTreeCaption == "" || strTreeCaption == undefined) {
                strTreeCaption = sfxCtrl.dictAttributes.ID;
            }
            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false };
            angular.forEach(sfxCtrl.Elements, function (model) {
                PopulateAvailableFieldsForFormLink(strProperty, model, childItem, isCheckBoxVisible);
            });
            if (childItem.Items.length > 0) {
                mainItem.Items.push(childItem);
            }
        }
        else if (sfxCtrl.Name == "sfwTabContainer") {
            strTreeCaption = sfxCtrl.dictAttributes.HeaderText;
            if (strTreeCaption == "" || strTreeCaption == undefined) {
                strTreeCaption = sfxCtrl.dictAttributes.ID;
            }
            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible };
            var objTabs = sfxCtrl.Elements[0];
            if (objTabs) {
                angular.forEach(objTabs.Elements, function (model) {
                    PopulateAvailableFieldsForFormLink(strProperty, model, childItem, isCheckBoxVisible);
                });
            }
            if (childItem.Items.length > 0) {
                mainItem.Items.push(childItem);
            }
        }
        else if (sfxCtrl.Name == "sfwGridView") {
            var strGridId = sfxCtrl.dictAttributes.ID;
            var childItem = { Text: strGridId, Items: [], IsSelected: false };
            if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                for (var j = 0; j < sfxCtrl.Elements[0].Elements.length; j++) {
                    var objTempField = sfxCtrl.Elements[0].Elements[j];
                    if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                        var objItemTempField = objTempField.Elements[0];
                        angular.forEach(objItemTempField.Elements, function (sfxControl) {
                            var strFieldName = "";

                            strFieldName = sfxControl.dictAttributes.sfwEntityField;
                            if (strFieldName != undefined && strFieldName != "") {
                                childItem.Items.push({ Text: strFieldName, Items: [], IsSelected: false, IsCheckBoxVisible: isCheckBoxVisible });
                            }
                        });

                    }
                }
                if (childItem.Items.length > 0) {
                    mainItem.Items.push(childItem);
                }
            }
        }
        else if (sfxCtrl.Name == "sfwWizard") {
            //foreach (BaseModel sfxWizardStep in sfxCtrl.Elements)
            var items = sfxCtrl.Elements.filter(function (itm) { return itm.Name == "items"; });
            if (items && items.length > 0) {
                angular.forEach(items[0].Elements, function (sfxWizardStep) {
                    var sfxHeaderTemplate = sfxWizardStep.Name == "HeaderTemplate" ? sfxWizardStep : undefined;
                    if (sfxHeaderTemplate) {
                        strTreeCaption = "HeaderTemplate";
                        var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: false };
                        if (sfxHeaderTemplate.Elements.length > 0) {
                            PopulateAvailableFieldsForFormLink(strProperty, sfxHeaderTemplate.Elements[0], childItem, isCheckBoxVisible);
                            if (childItem.Items.length > 0) {
                                mainItem.Items.push(childItem);
                            }
                        }
                    }
                    else {
                        angular.forEach(sfxWizardStep.Elements, function (wizardStep) {
                            strTreeCaption = sfxWizardStep.dictAttributes.Title;
                            if (!strTreeCaption || strTreeCaption == "") {
                                strTreeCaption = sfxWizardStep.dictAttributes.ID;
                            }
                            var childItem = { Text: strTreeCaption, Items: [], IsSelected: false, IsCheckBoxVisible: false };
                            if (wizardStep.Elements.length > 0) {
                                PopulateAvailableFieldsForFormLink(strProperty, wizardStep, childItem, isCheckBoxVisible);
                                if (childItem.Items.length > 0) {
                                    mainItem.Items.push(childItem);
                                }
                            }
                        });
                    }
                });
            }
        }
        else {
            var strFieldName = "";
            strFieldName = sfxCtrl.dictAttributes[strProperty];
            var childItem = n = { Text: strFieldName, Items: [], IsSelected: false, IsCheckBoxVisible: true };
            if (strFieldName && strFieldName != "") {
                mainItem.Items.push(childItem);
            }
        }
    });
}

function GetFormLinkItemsModel(model, formObject) {
    var retVal = { Name: "sfwLabel", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [], Children: [] };
    //if (formObject.dictAttributes.sfwType == "FormLinkWizard") {
    //    retVal = formObject.Elements.filter(function (x) { return x.Name.toLowerCase() == "items"; });
    //    if (retVal && retVal.length > 0) {
    //        return retVal[0].Elements;
    //    }
    //}
    //else {
    retVal = formObject.Elements.filter(function (x) { return x.Name.toLowerCase() == "items"; });
    if (retVal && retVal.length > 0) {
        return retVal[0];
    }
    //}
    return retVal;
}
//#endregion

function SetFormSelectedControl(formModel, objControl, event) {
    if (formModel && objControl) {
        if (formModel.SelectedControl) {
            formModel.SelectedControl.IsSelected = false;
            if (formModel.SelectedControl.Name == "sfwPanel" || formModel.SelectedControl.Name == "sfwDialogPanel" || formModel.SelectedControl.Name == "sfwListView") {
                formModel.SelectedControl.IsVisible = false;
            }
        }
        formModel.SelectedControl = objControl;
        formModel.SelectedControl.IsSelected = true;
        if (formModel.SelectedControl.Name == "sfwPanel" || formModel.SelectedControl.Name == "sfwDialogPanel" || formModel.SelectedControl.Name == "sfwListView") {
            formModel.SelectedControl.IsVisible = true;
        }
    }
    if (event) {
        event.stopPropagation();
    }
}

function GetCaptionFromFieldName(str) {
    if (str) {
        if (startsWith(str, "icdo", 0))
            str = str.replace("icdo", "");

        var strCaption = "";
        var blnCapsNext = true;

        for (var i = 0; i < str.length; i++) {
            if ("._".contains("" + str[i])) {
                blnCapsNext = true;
                strCaption += " ";
            }
            else {
                strCaption += blnCapsNext ? str.toUpperCase()[i] : str[i];
                blnCapsNext = false;
            }
        }

        if (endsWith(strCaption, " Id"))
            strCaption = strCaption.replace(" Id", " ID");
        if (strCaption.contains("Ssn"))
            strCaption = strCaption.replace("Ssn", "SSN");

        var intValuePos = strCaption.indexOf(" Value");
        if (intValuePos > 0)
            strCaption = strCaption.substring(0, intValuePos);

        var intDescPos = strCaption.indexOf(" Description");
        if (intDescPos > 0)
            strCaption = strCaption.substring(0, intDescPos);
    }
    return strCaption;
}
function GetCaptionFromField(field) {
    if (field) {
        if (field.Caption && field.Caption.trim().length > 0) {
            return field.Caption;
        }
        else {
            return GetCaptionFromFieldName(field.ID);
        }
    }
}
function CreateControl(formodel, cellVM, cntrlName, blnIsGrid) {
    var lst = cntrlName.split('.');
    var cntrlClass = "";
    var methodName = "";
    if (null != lst) {
        cntrlClass = lst[0];
        if (lst.length == 2) {
            methodName = lst[1];
        }
    }

    var sfxControlModel = CreateControlWithMethod(formodel, cntrlClass, methodName, cellVM, blnIsGrid);

    return sfxControlModel;
}

function CreateControlWithMethod(formodel, astrCntrlName, astrMethodName, cellVM, blnIsGrid) {
    var sfxControl = null;
    switch (astrCntrlName) {
        case "Panel":
            sfxControl = AddNewPanel(formodel, "sfwPanel", astrCntrlName, "NewPanel", cellVM);
            break;

        case "DialogPanel":
            sfxControl = AddNewPanel(formodel, "sfwDialogPanel", astrCntrlName, "DialogPanel", cellVM);
            break;

        //case "GridView":
        //    bool blnIsPrototype = false;
        //    if (aObjVM is FormObjectVM)
        //    {
        //        blnIsPrototype = (aObjVM as FormObjectVM).IsPrototype;
        //    }
        //    sfxControl = UtilityFunctions.CreateGridView(aObjVM, cellVM as SfxCellVM, blnIsPrototype);
        //    break;
        case "TabContainer":
            sfxControl = CreateTabContainer(formodel, cellVM);
            break;
        //case "NewButton":
        //    sfxControl = UtilityFunctions.CreateNewButton(aObjVM, cellVM as SfxCellVM, aModel);
        //    break;
        case "UserControl":
            sfxControl = { Name: "udc", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: [] };
            sfxControl.ParentVM = cellVM;


            sfxControl.dictAttributes.ID = GetControlID(formodel, sfxControl.Name);
            break;
        case "Caption":
            prefix = "swc";
            sfxControl = { Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
            sfxControl.ParentVM = cellVM;
            sfxControl.dictAttributes.ID = GetControlIDForCaption(formodel, sfxControl.Name, true);
            sfxControl.dictAttributes.sfwIsCaption = "True";
            break;
        case "sfwSwitchCheckBox":
            prefix = "swc";
            sfxControl = { Name: "sfwCheckBox", value: '', prefix: prefix, dictAttributes: { sfwIsSwitch: "True" }, Elements: [], Children: [] };
            sfxControl.ParentVM = cellVM;
            sfxControl.dictAttributes.ID = GetControlID(formodel, sfxControl.Name);
            break;
        default:
            var contrlname = astrCntrlName;
            var prefix = "";

            if (astrCntrlName == "sfwCommandButton") {
                contrlname = "sfwCommandButton";
                prefix = "swc";
            }
            else if (astrCntrlName == "sfwEmployerSoftErrors") {
                contrlname = "sfwSoftErrors";
                prefix = "swc";
            }
            else if (astrCntrlName == "br" || astrCntrlName == "hr") {
                contrlname = astrCntrlName;
            }

            else {
                contrlname = astrCntrlName;
                prefix = "swc";
            }
            sfxControl = { Name: contrlname, value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
            sfxControl.ParentVM = cellVM;
            if (!blnIsGrid) {
                sfxControl.dictAttributes.ID = GetControlID(formodel, sfxControl.Name);
            }
            else if (blnIsGrid && contrlname && contrlname == "sfwCascadingDropDownList") {
                sfxControl.dictAttributes.ID = GetControlID(formodel, sfxControl.Name);
            }
            if (contrlname && contrlname === "sfwDateTimePicker") {
                sfxControl.dictAttributes.sfwDataFormat = "{0:d}";
            }
            CheckAndSetControlDefaultValues(formodel, sfxControl, sfxControl.Name, astrMethodName, astrCntrlName);
            break;
    }
    return sfxControl;
}

function CreateTabContainer(formodel, sfxCellVM) {
    var prefix = "swc";
    var newTabContainerModel = { Name: "sfwTabContainer", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newTabContainerModel.ParentVM = sfxCellVM;
    newTabContainerModel.dictAttributes.ID = CreateControlID(formodel, "sfwTabContainer", "sfwTabContainer", false);

    var newtabsModel = { Name: "Tabs", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: [] };
    newtabsModel.ParentVM = newTabContainerModel;
    newTabContainerModel.Elements.push(newtabsModel);

    var newTabSheetModel = { Name: "sfwTabSheet", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newTabSheetModel.ParentVM = newtabsModel;
    newTabSheetModel.dictAttributes.ID = CreateControlID(formodel, "NewPage", "sfwTabSheet", false);
    newTabSheetModel.dictAttributes.HeaderText = "New Page";
    newtabsModel.Elements.push(newTabSheetModel);

    var newSfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxTableModel.ParentVM = newTabSheetModel;
    newSfxTableModel.dictAttributes.ID = CreateControlID(formodel, "Table", "sfwTable", false);
    newTabSheetModel.Elements.push(newSfxTableModel);

    var newSfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxRowModel.ParentVM = newSfxTableModel;


    var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxCellModel.ParentVM = newSfxRowModel;
    newSfxRowModel.Elements.push(newSfxCellModel);

    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxCellModel.ParentVM = newSfxRowModel;
    newSfxRowModel.Elements.push(newSfxCellModel);


    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxCellModel.ParentVM = newSfxRowModel;
    newSfxRowModel.Elements.push(newSfxCellModel);

    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    newSfxCellModel.ParentVM = newSfxRowModel;
    newSfxRowModel.Elements.push(newSfxCellModel);


    newSfxTableModel.Elements.push(newSfxRowModel);



    return newTabContainerModel;


}

function AddNewPanel(formodel, astrCntrlName, astrID, strPanelName, aModel) {
    var prefix = "swc";
    var sfxPanelModel = { Name: astrCntrlName, value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxPanelModel.ParentVM = aModel;

    sfxPanelModel.dictAttributes.ID = CreateControlID(formodel, strPanelName, astrCntrlName, false);
    sfxPanelModel.dictAttributes.sfwCaption = "New Page";

    var strCtrlId = CreateControlID(formodel, "NewPage", "sfwTable", false);

    var sfxTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxTableModel.ParentVM = sfxPanelModel;

    sfxTableModel.dictAttributes.ID = strCtrlId;

    var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = sfxTableModel;

    var newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = sfxRowModel;
    sfxRowModel.Elements.push(newSfxCellModel);

    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = sfxRowModel;
    sfxRowModel.Elements.push(newSfxCellModel);

    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = sfxRowModel;
    sfxRowModel.Elements.push(newSfxCellModel);

    newSfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = sfxRowModel;
    sfxRowModel.Elements.push(newSfxCellModel);

    sfxTableModel.Elements.push(sfxRowModel);

    sfxPanelModel.Elements.push(sfxTableModel);

    return sfxPanelModel;
}

function CheckAndSetControlDefaultValues(formodel, aModel, astrDropOpeName, astrMethodName, astrControlName) {
    //if (aModel.Name === "sfwCaptcha") {
    //    aModel.dictAttributes.sfwAudio = "Y";
    //    aModel.dictAttributes.sfwMode = "text";
    //    aModel.dictAttributes.sfwRefreshImageUrl = "/Icon_captcharefresh.jfif";
    //    aModel.dictAttributes.sfwSpeakerImageUrl = "/Icon_captchspeaker.jfif";
    //}
    if (aModel.Name == "sfwImageButton") {
        aModel.dictAttributes.sfwMethodName = "btnRetrieve_Click";
        aModel.dictAttributes.ImageUrl = "~/Image/Lookup.jpg";
        aModel.dictAttributes.ImageAlign = "AbsMiddle";


    }
    if (aModel.Name == "sfwSoftErrors") {
        if (astrControlName == "sfwSoftErrors") {
            aModel.dictAttributes.sfwEntityField = "InternalErrors";
        }
        else if (astrControlName == "sfwEmployerSoftErrors") {
            aModel.dictAttributes.sfwEntityField = "ExternalErrors";
            //var strBusObject = scope.GetBusinessObjectName(aObjVM);
            //aModel.dictAttributes.sfwObjectID = string.Format("{0}.ibusSoftErrors.iclbEmployerError", strBusObject);
        }
    }
    else if ((astrControlName != "sfwCommandButton") && (aModel.Name == "sfwButton" || aModel.Name == "sfwLinkButton" || aModel.Name == "sfwImageButton")) {
        CheckAndSetSfwButtonDefaultValues(formodel, aModel, astrMethodName);
    }
    else if (astrControlName == "sfwCommandButton") {
        var astrCommandName = "Cancel";
        aModel.dictAttributes.CommandName = astrCommandName;
        var strButtonID = CreateControlID(formodel, astrCommandName, "sfwButton", false);
        aModel.dictAttributes.Text = astrCommandName;
        aModel.dictAttributes.ID = strButtonID;
        aModel.dictAttributes.sfwCheckChanges = "True";
    }
    else if (astrControlName == "sfwCheckBox") {
        aModel.dictAttributes.sfwIsSwitch = "False";
    }
    else if (aModel.Name === "sfwTextBox" || aModel.Name === "sfwLabel") {
        aModel.dictAttributes.sfwRenderType = "None";
    }
    else if (aModel.Name === "sfwCheckBoxList") {
        aModel.dictAttributes.sfwDisplayInDialog = "True";
    }
}

function GetControlIDForCaption(formmodel, astrName, IsCaption) {
    var strID = astrName;
    if (startsWith(strID, "sfw", 0)) {
        strID = strID.substring(3);
    }
    strID = CreateControlID(formmodel, strID, astrName, IsCaption);

    return strID;
}

function GetControlID(formodel, astrName) {
    var strID = astrName;
    if (startsWith(strID, "sfw", 0)) {
        strID = strID.substring(3);
    }

    strID = CreateControlID(formodel, strID, astrName, false);

    return strID;
}

function CheckAndSetSfwButtonDefaultValues(formodel, aModel, astrMethodName) {
    if (astrMethodName) {
        var strButtonText = "";
        var strBtnID = "";
        switch (astrMethodName) {
            // Common Buttons
            case "btnNew_Click":
                strButtonText = "New";
                break;
            case "btnDelete_Click":
                strButtonText = "Delete";
                break;
            case "btnColumnsToExport_Click":
                strButtonText = "Export To Excel";
                break;
            case "btnNewUpdate_Click":
                strButtonText = "New Update";
                break;
            case "btnOpenLookup_Click":
                strButtonText = "Open Lookup";
                break;
            case "btnOpen_Click":
                strButtonText = "Open";
                break;
            case "btnOpenDoc_Click":
                strButtonText = "Open Document";
                break;

            // Lookup Buttons
            case "btnApplySort_Click":
                strButtonText = "Apply Sort";
                break;
            case "btnReset_Click":
                strButtonText = "Reset";
                break;
            case "btnSearch_Click":
                strButtonText = "Search";
                break;
            case "btnSearchCriteriaRequired_Click":
                strButtonText = "Search Criteria";
                break;
            case "btnStoreUserDefaults_Click":
                strButtonText = "Store Defaults";
                break;

            // Maintetance Buttons
            case "btnAddNewChild_Click":
                strButtonText = "Add New Child";
                break;
            case "btnAuditHistory_Click":
                strButtonText = "Audit History";

                var strNavParams = "primary_key=;form_name=#" + formodel.dictAttributes.ID;
                aModel.dictAttributes.sfwActiveForm = "wfmAuditMaintenance";
                aModel.dictAttributes.sfwNavigationParameter = strNavParams;

                break;
            case "btnCorrespondence_Click":
                strButtonText = "Correspondence";
                break;
            case "btnDownload_Click":
                strButtonText = "Download";
                break;
            case "btnExecuteBusinessMethodSelectRows_Click":
                strButtonText = "Execute Pass Selected Rows";
                break;
            case "btnValidateExecuteBusinessMethod_Click":
                strButtonText = "Execute Refresh From Object";
                break;
            case "btnExecuteBusinessMethod_Click":
                strButtonText = "Execute Refresh Data";
                break;
            case "btnExecuteServerMethod_Click":
                strButtonText = "Execute Server Method";
                break;
            case "btnForceSave_Click":
                strButtonText = "Force Save";
                break;
            case "btnNext_Click":
                strButtonText = "Next Record";
                break;
            case "btnPrev_Click":
                strButtonText = "Previous Record";
                break;
            case "btnCancel_Click":
                strButtonText = "Refresh Data";
                break;
            case "btnRefreshServers_Click":
                strButtonText = "Refresh Servers";
                break;
            case "btnSaveAndNext_Click":
                strButtonText = "Save And Next";
                break;
            case "btnSaveNew_Click":
                strButtonText = "Save And New";
                break;
            case "btnSave_Click":
                strButtonText = "Save";
                break;
            case "btnSaveIgnoreReadOnly_Click":
                strButtonText = "Save ReadOnly";
                break;
            case "btnNoChangesSave_Click":
                strButtonText = "Save No Changes";
                break;
            case "btnGridViewAdd_Click":
                strButtonText = "Add";
                break;
            case "btnGridViewUpdate_Click":
                strButtonText = "Update";
                break;
            case "btnGridViewSelect_Click":
                strButtonText = "Select";
                break;
            case "btnGridViewDelete_Click":
                strButtonText = "Delete";
                break;
            case "btnWizardPrevious_Click":
                strButtonText = "Previous";
                break;
            case "btnWizardSaveAndPrevious_Click":
                strButtonText = "Previous";
                strBtnID = "Save And Previous";
                break;
            case "btnWizardNext_Click":
                strButtonText = "Next";
                break;
            case "btnWizardFindAndNext_Click":
                strButtonText = "Next";
                strBtnID = "Find And Next";
                break;
            case "btnWizardSaveAndNext_Click":
                strButtonText = "Next";
                strBtnID = "Save And Next";
                break;
            case "btnWizardFinish_Click":
                strButtonText = "Finish";
                break;
            case "btnPdfCorrespondence_Click":
                strButtonText = "PDF Correspondence";
                aModel.dictAttributes.sfwTriggerPostBack = "True";
                break;

            case "btnBpmApprove_Click":
                strButtonText = "BPM Approve";
                aModel.dictAttributes.sfwMode = "Update";
                aModel.dictAttributes.sfwRefreshUpdatePanels = "uppWorkflowNew;uppCenterMiddle";
                break;
            case "btnBpmReject_Click":
                strButtonText = "BPM Reject";
                aModel.dictAttributes.sfwMode = "Update";
                aModel.dictAttributes.sfwRefreshUpdatePanels = "uppWorkflowNew;uppCenterMiddle";
                break;
            case "btnBpmSubmit_Click":
                strButtonText = "BPM Submit";
                aModel.dictAttributes.sfwMode = "Update";
                aModel.dictAttributes.sfwRefreshUpdatePanels = "uppWorkflowNew;uppCenterMiddle";
                break;
            default:
                strButtonText = astrMethodName.replace("btn", "");
                break;
        }

        aModel.dictAttributes.sfwMethodName = astrMethodName;

        if (strButtonText != undefined && strButtonText != "") {
            if (strBtnID == undefined || strBtnID == "") {
                strBtnID = strButtonText;
            }
            aModel.dictAttributes.Text = strButtonText;

            var strButtonID = "";
            strButtonID = CreateControlID(formodel, strBtnID, aModel.Name);
            aModel.dictAttributes.ID = strButtonID;
        }
    }
}

function GetMaxColCount(arowVM, tableVM) {

    var MaxColCount = arowVM.Elements.length;
    if (tableVM) {
        angular.forEach(tableVM.Elements, function (rowVM) {
            if (rowVM.Elements.length > MaxColCount) {
                MaxColCount = rowVM.Elements.length;
            }
        });
    }

    return MaxColCount;
}

function CreateControlIDInCamelCase(astrFieldName) {
    var strCtrlID = "";
    var strSep = "~`!@#$%^&*_-–+=[{()}]|:;<,.>?/. ";

    var blnCapsNext = true;
    for (i = 0; i < astrFieldName.length; i++) {
        if (strSep.contains("" + astrFieldName[i]))
            blnCapsNext = true;
        else {
            strCtrlID += blnCapsNext ? astrFieldName.toUpperCase()[i] : astrFieldName[i];
            blnCapsNext = false;
        }
    }

    if ((strCtrlID.toLowerCase().indexOf("iclb") > -1 && strCtrlID.toLowerCase().startsWith("iclb")) || (strCtrlID.toLowerCase().indexOf("icol") > -1 && strCtrlID.toLowerCase().startsWith("icol")))
        strCtrlID = strCtrlID.substring(4);

    return strCtrlID;
}

function SetDefultValuesBasedOnDataType(aField, astrClassName, sfxCntrlModel) {
    if (astrClassName == "sfwLabel" || astrClassName == "sfwTextBox" || astrClassName == "sfwDateTimePicker") {
        if (astrClassName == "sfwLabel" && aField.Value && (aField.Value.toLowerCase() == "created_date" || aField.Value.toLowerCase() == "modified_date")) {
            sfxCntrlModel.dictAttributes.sfwDataFormat = "{0:G}";
        }
        else if (aField.DataType && (aField.DataType.toLowerCase() == "datetime" || aField.DataType.toLowerCase() == "date")) {
            sfxCntrlModel.dictAttributes.sfwDataFormat = "{0:d}";
        }
        else if (aField.DataType == "Decimal" && endsWith(aField.Value, "_amt") || endsWith(aField.Value, "_amount")) {
            sfxCntrlModel.dictAttributes.sfwDataFormat = "{0:C}";
        }
        else if (aField.Value.contains("ssn")) {
            sfxCntrlModel.dictAttributes.sfwDataFormat = "{0:000-##-####}";
        }
        else if (aField.Value.contains("phone") || aField.Value.contains("fax")) {
            sfxCntrlModel.dictAttributes.sfwDataFormat = "{0:(###)###-####}";
        }
    }
}

function CreateControlID(formodel, astrFieldName, astrControlClass, ablnIsCaption) {
    var strCtrlID = CreateControlIDWithPrefix(formodel, astrFieldName, astrControlClass, ablnIsCaption);

    if (strCtrlID == "")
        return "";

    if (formodel) {
        var objModel = FindControlByID(formodel, strCtrlID);

        var iNum = 0;
        var strID = strCtrlID;
        while (objModel != null) {
            iNum++;
            strCtrlID = String.format("{0}{1}", strID, iNum);
            objModel = FindControlByID(formodel, strCtrlID);
        }
    }
    return strCtrlID;
}

function CreateControlIDWithPrefix(formodel, astrFieldName, astrControlClass, ablnIsCaption) {
    var strCtrlID = "";
    strCtrlID = getControlIDPrefix(astrControlClass, ablnIsCaption);

    if (startsWith(astrFieldName, strCtrlID, 0)) {
        astrFieldName = astrFieldName.substring(strCtrlID.length);
    }

    if (strCtrlID.length > 0)
        strCtrlID += CreateActualControlID(astrFieldName);

    return strCtrlID;
}

function getControlIDPrefix(controlName, isCaption) {
    var prefix = "";
    switch (controlName) {
        case "sfwTable":
            prefix = "tbl";
            break;
        case "sfwPanel":
            prefix = "pnl";
            break;
        case "sfwTabContainer":
            prefix = "tct";
            break;
        case "sfwTabSheet":
            prefix = "tsh";
            break;
        case "sfwLabel":
            if (isCaption)
                prefix = "cap";
            else
                prefix = "lbl";
            break;
        case "sfwLiteral":
            prefix = "ltr";
            break;
        case "sfwCheckBox":
            prefix = "chk";
            break;
        case "sfwTextBox":
            prefix = "txt";
            break;
        case "sfwDropDownList":
        case "sfwMultiSelectDropDownList":
            prefix = "ddl";
            break;
        case "sfwCheckBoxList":
            prefix = "cbl";
            break;
        case "sfwRadioButtonList":
            prefix = "rbl";
            break;
        case "sfwGridView":
            prefix = "grv";
            break;
        case "sfwButton":
        case "sfwToolTipButton":
            prefix = "btn";
            break;
        case "sfwLinkButton":
            prefix = "btn";
            break;
        case "sfwImageButton":
            prefix = "btn";
            break;
        case "sfwFileLayout":
            prefix = "flo";
            break;
        case "RequiredFieldValidator":
            prefix = "rfv";
            break;
        case "CompareValidator":
            prefix = "cmv";
            break;
        case "RangeValidator":
            prefix = "rnv";
            break;
        case "RegularExpressionValidator":
            prefix = "rev";
            break;
        case "ValidationSummary":
            prefix = "vls";
            break;
        case "sfwRadioButton":
            prefix = "rdb";
            break;
        case "sfwMultiCorrespondence":
            prefix = "mcr";
            break;
        case "sfwCRViewer":
            prefix = "crv";
            break;
        case "sfwHyperLink":
            prefix = "hyp";
            break;
        case "sfwFileUpload":
            prefix = "flu";
            break;
        case "sfwSoftErrors":
            prefix = "egr";
            break;
        case "sfwListBox":
            prefix = "lst";
            break;
        case "sfwImage":
            prefix = "img";
            break;
        case "sfwWizard":
            prefix = "wiz";
            break;
        case "sfwWizardStep":
            prefix = "wzs";
            break;
        case "sfwCascadingDropDownList":
            prefix = "ddl";
            break;
        case "sfwListPicker":
            prefix = "lv";
            break;
        case "sfwTargetList":
            prefix = "lv";
            break;
        case "sfwSourceList":
            prefix = "lv";
            break;

        case "sfwChart":
            prefix = "chrt";
            break;
        case "sfwSeries":
            prefix = "ser";
            break;
        case "sfwChartArea":
            prefix = "chrta";
            break;

        case "sfwDialogPanel":
            prefix = "pnl";
            break;

        case "udc":
        case "UserControl":
            prefix = "UserControl";
            break;

        case "sfwListView":
            prefix = "lst";
            break;
        case "sfwRuleViewer":
            prefix = "rvw";
            break;
        case "sfwSlider":
            prefix = "sld";
            break;
        case "sfwQuestionnairePanel":
            prefix = "qnp";
            break;
        case "sfwDateTimePicker":
            prefix = "dtp";
            break;

        case "sfwKnob":
            prefix = "knb";
            break;
        case "sfwCalendar":
            prefix = 'cal';
            break;
        case "hr":
        case "br":
            prefix = controlName;
            break;
        case "sfwUserDefaults":
            prefix = "uds";
            break;
        case "sfwOpenDetail":
            prefix = 'od';
            break;
        case "sfwWizardProgress":
            prefix = "wp";
            break;
        case "sfwJSONData":
            prefix = "jdata";
            break;
        case "sfwScheduler":
            prefix = "sch";
            break;
        case "sfwButtonGroup":
            prefix = "btngrp";
            break;
        case "sfwCaptcha":
            prefix = "cpt";
            break;
    }
    return prefix;
}

function CreateActualControlID(astrFieldName) {
    var strCtrlID = "";
    var strSep = "~`!@#$%^&*_-–+=[{()}]|:;<,.>?/. ";

    var blnCapsNext = true;
    for (var i = 0; i < astrFieldName.length; i++) {
        if (strSep.indexOf("" + astrFieldName[i]) !== -1) {
            blnCapsNext = true;
        }
        else {
            strCtrlID += blnCapsNext ? astrFieldName.toUpperCase()[i] : astrFieldName[i];
            blnCapsNext = false;
        }
    }

    if (startsWith(strCtrlID.toLowerCase(), "iclb", 0) || startsWith(strCtrlID.toLowerCase(), "icol", 0))
        strCtrlID = strCtrlID.substring(4);

    return strCtrlID;
}

function FindParent(aModel, parentCtrlName, includeSelf) {
    if (aModel) {
        var retValue = null;
        if (aModel.Name === parentCtrlName && includeSelf) {
            retValue = aModel;
        }
        else {
            var parent = aModel.ParentVM;

            while (parent && parent.Name != parentCtrlName) {
                parent = parent.ParentVM;
            }

            retValue = parent;
        }
        return retValue;
    }
}

function PopulateAssociatedControls(aModel) {
    //Populate Associated Control
    var lstAssociatedControls = [];
    var sfxAssociatedCell = [];
    lstAssociatedControls.push("");
    if (aModel.dictAttributes) {
        var strEntityField = aModel.dictAttributes.sfwEntityField;//[ApplicationConstants.XMLFacade.SFWDATAFIELD];
        var strDataField = aModel.dictAttributes.sfwDataField;
        if ((strEntityField == undefined || strEntityField == "") && (strDataField == undefined || strDataField == "")) {

            var sfxRow = aModel;
            var sfxAssociatedCell;
            sfxRow = FindParent(aModel, "sfwRow");
            var blnCellFound = false;

            if (sfxRow) {
                var iRow = -1;
                angular.forEach(sfxRow.Elements, function (sfxCell) {
                    iRow++;
                    if (blnCellFound) {
                        return;
                    }
                    angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                        if (sfxCtrl) {
                            if (sfxCtrl.dictAttributes.ID == aModel.dictAttributes.ID) {
                                blnCellFound = true;
                                sfxAssociatedCell.push(sfxRow.Elements[iRow]);
                                if (sfxRow.Elements.length > iRow + 1 && sfxRow.Elements[iRow + 1] != undefined) {
                                    sfxAssociatedCell.push(sfxRow.Elements[iRow + 1]);
                                }
                                return;
                            }
                        }
                    });
                });
                if (sfxAssociatedCell != undefined) {
                    if (sfxAssociatedCell.length > 0) {
                        angular.forEach(sfxAssociatedCell, function (sfxCell) {
                            if (sfxCell && sfxCell.Elements.length > 0) {
                                angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                                    if (!sfxCtrl.dictAttributes.sfwIsCaption) {
                                        var strID = sfxCtrl.dictAttributes.ID;
                                        if (strID) {
                                            if (sfxCtrl.Name != "sfwGridView" &&
                                                sfxCtrl.Name != "sfwChart" &&
                                                sfxCtrl.Name != "sfwPanel" &&
                                                sfxCtrl.Name != "sfwDialogPanel" &&
                                                sfxCtrl.Name != "sfwButton" &&
                                                sfxCtrl.Name != "sfwTabContainer" &&
                                                sfxCtrl.Name != "sfwTabSheet"
                                                && sfxCtrl.Name != "udc"
                                                && sfxCtrl.Name != "sfwListViewer") {

                                                lstAssociatedControls.push(strID);
                                            }
                                        }
                                    }

                                });
                            }
                        });
                    }
                    else {
                        lstAssociatedControls.push("");
                    }
                }
            }
        }
    }
    return lstAssociatedControls;
}

function PopulateAssociatedControlsForRetriveButton(aModel) {
    //Populate Associated Control
    var lstAssociatedControls = [];
    lstAssociatedControls.push("");
    var strEntityField = aModel.dictAttributes.sfwEntityField;//[ApplicationConstants.XMLFacade.SFWDATAFIELD];
    var strDataField = aModel.dictAttributes.sfwDataField;
    if ((strEntityField == undefined || strEntityField == "") && (strDataField == undefined || strDataField == "")) {
        var sfxRow = aModel;
        var sfxAssociatedCell;
        sfxRow = FindParent(aModel, "sfwRow");
        var blnCellFound = false;

        if (sfxRow != undefined) {
            var iRow = -1;
            angular.forEach(sfxRow.Elements, function (sfxCell) {
                iRow++;
                if (blnCellFound) {
                    return;
                }
                angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                    if (sfxCtrl) {
                        if (sfxCtrl.dictAttributes.ID == aModel.dictAttributes.ID) {
                            blnCellFound = true;
                            if (sfxRow.Elements.length > iRow + 1 && sfxRow.Elements[iRow] != undefined) {
                                sfxAssociatedCell = sfxRow.Elements[iRow];
                            }
                            return;
                        }
                    }
                });
            });
            if (sfxAssociatedCell != undefined) {
                if (sfxAssociatedCell.Elements.length > 0) {
                    angular.forEach(sfxAssociatedCell.Elements, function (sfxCtrl) {
                        var strID = sfxCtrl.dictAttributes.ID;//[ApplicationConstants.XMLFacade.ID];
                        if (strID != undefined && strID != "") {
                            if (sfxCtrl.Name == "sfwLabel" ||
                                sfxCtrl.Name == "sfwTextBox" ||
                                sfxCtrl.Name == "sfwDropDownList" ||
                                sfxCtrl.Name == "sfwMultiSelectDropDownList" ||
                                sfxCtrl.Name == "sfwCheckBoxList" ||
                                sfxCtrl.Name == "sfwRadioButtonList" ||
                                sfxCtrl.Name == "sfwCheckBox" ||
                                sfxCtrl.Name == "sfwRadioButton" ||
                                sfxCtrl.Name == "sfwCascadingDropDownList") {
                                lstAssociatedControls.push(strID);
                            }
                        }
                    });
                }
                else {
                    lstAssociatedControls.push("");
                }
            }
        }
    }
    return lstAssociatedControls;
}

function PopulateRelatedGrid(aModel, lstParentGrid, isFilterReset) {
    if (isFilterReset) {
        if (aModel.Name == "sfwButton" ||
            aModel.Name == "sfwLinkButton" ||
            aModel.Name == "sfwImageButton") {
            if (aModel.dictAttributes.sfwMethodName == "btnGridSearch_Click"
                || aModel.dictAttributes.sfwMethodName == "btnGridSearchCriteriaReq_Click") {
                var strBtnCtrlID = aModel.dictAttributes.ID;//[ApplicationConstants.XMLFacade.ID];
                if (strBtnCtrlID && strBtnCtrlID.length > 0) {
                    lstParentGrid.push(strBtnCtrlID);
                }
            }
        }
        angular.forEach(aModel.Elements, function (itm) {
            PopulateRelatedGrid(itm, lstParentGrid, true);
        });
    }
    else {
        if (aModel.Name == "sfwGridView" || aModel.Name == "sfwScheduler") {
            var strCtrlID = aModel.dictAttributes.ID;
            if (strCtrlID && strCtrlID.length > 0 && !strCtrlID.match("^idsfw")) {
                lstParentGrid.push(strCtrlID);
            }
        }
        angular.forEach(aModel.Elements, function (itm) {
            PopulateRelatedGrid(itm, lstParentGrid);
        });
    }
}

function PopulateEntityRules(entityModel, isWizard, astrInitialLoadGroup, strGroupName) {
    var lstBusinessRules = [];

    if (astrInitialLoadGroup != undefined && astrInitialLoadGroup != "") {
        var groupListModel;
        var lstGrouplist = entityModel.lstGroupsList;
        if (lstGrouplist && lstGrouplist.length > 0) {
            groupListModel = lstGrouplist[0];
        }
        if (groupListModel != null) {
            var groupModel;
            var lstGroups = groupListModel.Elements.filter(function (ele) { return ele.dictAttributes.ID == astrInitialLoadGroup; });
            if (lstGroups && lstGroups.length > 0) {
                groupModel = lstGroups[0];
            }
            if (groupModel) {
                angular.forEach(groupModel.Elements, function (itmModel) {
                    lstBusinessRules.push(itmModel.dictAttributes.ID);
                });
            }
        }
    }
    else {
        var initialLoad;
        if (entityModel) {
            var lstinitialLoad = entityModel.lstInitialLoadList;
            if (lstinitialLoad && lstinitialLoad.length > 0) {
                initialLoad = lstinitialLoad[0];
            }
            if (initialLoad) {
                angular.forEach(initialLoad.Elements, function (lxmlItem) {
                    lstBusinessRules.push(lxmlItem.dictAttributes.ID);
                });
            }
        }
    }

    return lstBusinessRules;
}

function IsAuditField(strField) {
    strField = strField.toLowerCase();

    if (strField == "created_by") return true;
    if (strField == "created_date") return true;
    if (strField == "modified_by") return true;
    if (strField == "modified_date") return true;
    if (strField == "update_seq") return true;

    return false;
}

function FindControlByID(sfxControl, strId) {
    var objControl;
    if (strId && strId != "" && sfxControl.dictAttributes.ID == strId) {
        objControl = sfxControl;
        return objControl;
    }

    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (objControl == undefined) {
            objControl = FindControlByID(ctrl, strId);
        }
    });

    return objControl;
}

function CheckforDuplicateID(model, strID, lstIds) {
    if (model.dictAttributes.ID != undefined && model.dictAttributes.ID != "") {
        if (model.dictAttributes.ID == strID) {
            lstIds.push(strID);
        }
    }

    angular.forEach(model.Elements, function (ctrl) {

        CheckforDuplicateID(ctrl, strID, lstIds);

    });
}

function GetBuisnessRules(entityModel, astrSection, nodeName, sfwRuleGroup, isWizard) {
    var arrResult = [];
    //arrResult.Add(string.Empty);
    if (entityModel != null) {

        if (isWizard && entityModel.lstGroupsList) {
            var groupsModel = entityModel.lstGroupsList[0];
            if (groupsModel && groupsModel.Elements.length > 0) {
                angular.forEach(groupsModel.Elements, function (groupModel) {
                    if (groupModel.dictAttributes.ID == sfwRuleGroup) {
                        angular.forEach(groupModel.Elements, function (itemModel) {
                            arrResult.push(itemModel.dictAttributes.ID);
                        });
                    }
                });
            }
        }
        else {
            if (entityModel.Elements.length > 0) {
                var lstRuleList = entityModel.Elements.filter(function (itm) { return itm.Name == astrSection; });
                if (lstRuleList && lstRuleList.length > 0) {
                    var objRuleList = lstRuleList[0];
                    if (objRuleList != null) {
                        var rules = [];
                        GetRulesList(objRuleList, nodeName, rules);

                        if (null != rules) {
                            angular.forEach(rules, function (objRule) {
                                arrResult.push(objRule.dictAttributes.ID);
                            });
                        }
                    }
                }
            }
        }
    }

    return arrResult;
}

function GetRulesList(objRuleList, nodeName, rules) {
    if (objRuleList.Name == nodeName) {
        rules.push(objRuleList);
    }

    angular.forEach(objRuleList.Elements, function (itm) {
        GetRulesList(itm, nodeName, rules);
    });

    angular.forEach(objRuleList.Children, function (itm) {
        GetRulesList(itm, nodeName, rules);
    });
}

function FindControlListByNameForTextBox(sfxControl, name, list, formobject) {
    if (name && name != "" && sfxControl && sfxControl.Name == name) {
        if (formobject && formobject.SelectedControl && formobject.SelectedControl.Name == name && formobject.SelectedControl.dictAttributes.ID && formobject.SelectedControl.dictAttributes.ID != sfxControl.dictAttributes.ID) {
            list.push(sfxControl);
        }
    }

    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name != "sfwGridView") { // done by neha : we donot want to search for the controls inside grid
            FindControlListByNameForTextBox(ctrl, name, list, formobject);
        }
    });
}
function FindControlListByName(sfxControl, name, list) {
    if (name && name != "" && sfxControl && sfxControl.Name == name) {
        list.push(sfxControl);
    }
    if (sfxControl) {
        angular.forEach(sfxControl.Elements, function (ctrl) {
            if (ctrl.Name != "sfwGridView") { // done by neha : we donot want to search for the controls inside grid
                FindControlListByName(ctrl, name, list);
            }
        });
    }
}

function FindCaptionControlList(sfxControl, list) {
    if (sfxControl && sfxControl.Name == "sfwLabel" && sfxControl.dictAttributes && sfxControl.dictAttributes.sfwIsCaption && sfxControl.dictAttributes.sfwIsCaption.toLowerCase() == "true") {
        list.push(sfxControl);
    }

    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name != "sfwGridView") { // done by neha : we donot want to search for the controls inside grid
            FindCaptionControlList(ctrl, list);
        }
    });
}

function FindControlListByNames(sfxControl, controlNames, list) {
    if (controlNames && controlNames.length > 0 && sfxControl && controlNames.some(function (itm) { return itm == sfxControl.Name; })) {
        list.push(sfxControl);
    }

    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name != "sfwGridView") { // done by neha : we donot want to search for the controls inside grid
            FindControlListByNames(ctrl, controlNames, list);
        }
    });
}

function PopulateRelatedDialogList(sfxTable, model) {
    var lstRelatedDialog = [];
    lstRelatedDialog.push("");
    if (sfxTable != null) {
        PopulateRelatedDialogs(sfxTable, lstRelatedDialog, model);
    }
    return lstRelatedDialog;
}

function PopulateRelatedDialogs(sfxTable, lstRelatedDialog, model) {
    if (sfxTable != null) {
        angular.forEach(sfxTable.Elements, function (sfxRow) {

            if (sfxRow.Name == "sfwDialogPanel") {
                var strCtrlID = sfxRow.dictAttributes.ID;
                if (strCtrlID.length > 0) {
                    lstRelatedDialog.push(strCtrlID);
                }
            }
            else if (sfxRow.Name == "sfwWizardStep") {
                if (model) {
                    var parent = FindParent(model, "sfwWizardStep");
                    if (parent && sfxRow.dictAttributes.ID == parent.dictAttributes.ID) {
                        if (sfxRow.Elements.length > 0) {
                            PopulateRelatedDialogs(sfxRow, lstRelatedDialog, model);
                        }
                    }
                }
            }
            else {
                PopulateRelatedDialogs(sfxRow, lstRelatedDialog, model);
            }

        });
    }
}




function PopulateServerMethod(lstObjectMethods, objControl, objRemoteObject, isLoadRemoteObjectMethod) {
    var lstServerMethod = [];
    lstServerMethod.push("");
    if (objControl.dictAttributes.sfwMethodName == "btnExecuteServerMethod_Click" || objControl.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click"
        || objControl.dictAttributes.sfwMethodName == "btnWorkflowExecuteMethod_Click" || objControl.dictAttributes.sfwMethodName == "btnDownload_Click"
        || objControl.dictAttributes.sfwMethodName == "btnExecuteServerMethodFromLookup_Click" || isLoadRemoteObjectMethod) {
        if (objRemoteObject) {
            angular.forEach(objRemoteObject.Elements, function (method) {
                if (method.dictAttributes.ID) {
                    lstServerMethod.push(method.dictAttributes.ID);
                }
            });
        }
    }
    else if (objControl.dictAttributes.sfwMethodName == "btnExecuteBusinessMethod_Click" || objControl.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click"
        || objControl.dictAttributes.sfwMethodName == "btnValidateExecuteBusinessMethod_Click" || objControl.dictAttributes.sfwMethodName == "btnCopyRecord_Click"
        || objControl.dictAttributes.sfwMethodName == "btnGridSearchCriteriaReq_Click" || objControl.dictAttributes.sfwMethodName == "btnBack_Click"
        || objControl.dictAttributes.sfwMethodName == "btnCompleteWorkflowActivities_Click" || objControl.dictAttributes.sfwMethodName == "btnWizardCancel_Click" || objControl.dictAttributes.sfwMethodName == "btnCompleteWorkflowActivities_Click") {
        angular.forEach(lstObjectMethods, function (method) {
            if (method.ID) {
                lstServerMethod.push(method.ID);
            }
        });

    }
    return lstServerMethod;
}

function GetServerMethodObject(RemoteObject, lstRemoteObject) {
    var objServerObject;
    if (RemoteObject != undefined && RemoteObject != "") {
        if (lstRemoteObject && lstRemoteObject.length > 0) {

            var lst = lstRemoteObject.filter(function (itm) {
                return itm.dictAttributes.ID == RemoteObject;
            });
            if (lst && lst.length > 0) {
                objServerObject = lst[0];
            }
        }
    }
    return objServerObject;
}


function IsCriteriaField(model) {
    var retVal = true;

    var parentObj = model.ParentVM;
    while (null != parentObj) {
        if (parentObj.Name == "sfwGridView") {
            {
                retVal = false;
            }

            break;
        }
        parentObj = parentObj.ParentVM;
    }

    return retVal;
}

function LoadAttributeButton(model, formType) {
    var lstAttributes = [];
    if (formType == "Lookup" || formType == "FormLinkLookup") {
        if ((model.dictAttributes.sfwMethodName == "btnNew_Click") ||
            (model.dictAttributes.sfwMethodName == "btnOpenDoc_Click")) {
            if (model.dictAttributes.sfwMethodName == "btnNew_Click") {
                lstAttributes.push("sfwActiveForm");
            }
            lstAttributes.push("sfwCustomAttributes");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            if ((model.dictAttributes.sfwMethodName == "btnNew_Click")) {
                lstAttributes.push("sfwEntityField");
            }
            if (!(model.dictAttributes.sfwMethodName == "btnNew_Click")) {
                lstAttributes.push("sfwRelatedControl");
            }
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameter");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnOpen_Click") ||
            (model.dictAttributes.sfwMethodName == "btnOpenLookup_Click")) {
            if ((model.dictAttributes.sfwMethodName == "btnOpenLookup_Click")) {
                lstAttributes.push("sfwActiveForm");
            }
            if (model.dictAttributes.sfwMethodName == "btnOpen_Click") {
                lstAttributes.push("sfwActiveForm");
                lstAttributes.push("sfwFormTitleField");
                lstAttributes.push("sfwFormToolTipField");
            }
            lstAttributes.push("sfwCustomAttributes");

            lstAttributes.push("sfwNavigationParameter");

            lstAttributes.push("OnClientClick");
            if (model.dictAttributes.sfwMethodName == "btnOpen_Click") {
                lstAttributes.push("sfwEntityField");
                lstAttributes.push("sfwRelatedControl");
            }
            lstAttributes.push("sfwSecurityLevel");
            if (model.dictAttributes.sfwMethodName == "btnOpen_Click") {
                lstAttributes.push("sfwSelection");
            }
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnDelete_Click") ||
            (model.dictAttributes.sfwMethodName == "btnColumnsToExport_Click") ||
            (model.dictAttributes.sfwMethodName == "btnExportToExcel_Click")) {
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.dictAttributes.sfwMethodName == "btnColumnsToExport_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnExportToExcel_Click")) {
                lstAttributes.push("sfwMessageId");
            }
            lstAttributes.push("OnClientClick");
            if (model.dictAttributes.sfwMethodName != "btnExportToExcel_Click") {
                lstAttributes.push("sfwRelatedControl");
            }
            lstAttributes.push("sfwSecurityLevel");
            if (!(model.dictAttributes.sfwMethodName == "btnColumnsToExport_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnExportToExcel_Click")) {
                lstAttributes.push("sfwSelection");
            }
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
            lstAttributes.push("sfwCustomAttributes");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") ||
            (model.dictAttributes.sfwMethodName == "btnNewUpdate_Click")) {
            if (model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
                lstAttributes.push("sfwActiveForm");
                lstAttributes.push("sfwCustomAttributes");
                lstAttributes.push("sfwMessageId");
            }
            if (model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") {
                lstAttributes.push("sfwActiveForm");
                lstAttributes.push("sfwCustomAttributes");
            }
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnReset_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSearch_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSearchCriteriaRequired_Click") ||
            (model.dictAttributes.sfwMethodName == "btnStoreUserDefaults_Click") ||
            (model.dictAttributes.sfwMethodName == "btnApplySort_Click")) {
            lstAttributes.push("sfwCustomAttributes");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwObjectMethod");
            if (model.dictAttributes.sfwMethodName == "btnApplySort_Click") {
                lstAttributes.push("sfwRelatedControl");
            }
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");
        }
        else {//LookUp
            lstAttributes.push("sfwActiveForm");
            lstAttributes.push("sfwCustomAttributes");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("Visible");

        }
    }
    else if ((formType == "Maintenance" || formType == "FormLinkMaintenance") || (formType == "FormLinkWizard")) {

        if (model.dictAttributes.sfwMethodName == "btnColumnsToExport_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnGridViewAddEmpty_Click") ||
            (model.dictAttributes.sfwMethodName == "btnGridViewDelete_Click") ||
            (model.dictAttributes.sfwMethodName == "btnGridViewDeleteLast_Click") ||
            (model.dictAttributes.sfwMethodName == "btnGridViewSelect_Click")) {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnGridViewAdd_Click") ||
            (model.dictAttributes.sfwMethodName == "btnGridViewUpdate_Click")) {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnNew_Click") {
            lstAttributes.push("sfwActiveForm");
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnDelete_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnExportToExcel_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") ||
            (model.dictAttributes.sfwMethodName == "btnOpenLookup_Click") ||
            (model.dictAttributes.sfwMethodName == "btnAuditHistory_Click")) {
            if ((model.dictAttributes.sfwMethodName == "btnCorrespondenceRows_Click") ||
                (model.dictAttributes.sfwMethodName == "btnOpenLookup_Click")) {
                lstAttributes.push("sfwActiveForm");
            }
            if ((model.dictAttributes.sfwMethodName == "btnAuditHistory_Click")) {
                lstAttributes.push("sfwActiveForm");
            }
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
            lstAttributes.push("sfwActiveForm");
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnOpen_Click") ||
            (model.dictAttributes.sfwMethodName == "btnPrototypeSearch_Click")) {
            lstAttributes.push("sfwActiveForm");
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            if (model.dictAttributes.sfwMethodName == "btnPrototypeSearch_Click") {
                lstAttributes.push("sfwMessageId");
            }
            lstAttributes.push("sfwMode");

            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            if (model.dictAttributes.sfwMethodName == "btnPrototypeSearch_Click") {
                lstAttributes.push("sfwObjectMethod");
            }
            if ((model.dictAttributes.sfwMethodName == "btnOpen_Click")) {
                lstAttributes.push("sfwEntityField");
                lstAttributes.push("sfwFormTitleField");
                lstAttributes.push("sfwFormToolTipField");
            }
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnGridSearch_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnForceSave_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSaveNew_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSaveAndNext_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSave_Click") ||
            (model.dictAttributes.sfwMethodName == "btnSaveIgnoreReadOnly_Click") ||
            (model.dictAttributes.sfwMethodName == "btnNoChangesSave_Click")) {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnOpenDoc_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnAddNewChild_Click") ||
            (model.dictAttributes.sfwMethodName == "btnClearControls_Click") ||
            (model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click") ||
            (model.dictAttributes.sfwMethodName == "btnCorrespondence_Click") ||
            (model.dictAttributes.sfwMethodName == "btnDownload_Click") ||
            (model.dictAttributes.sfwMethodName == "btnGridSearchCriteriaReq_Click") ||
            (model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click") ||
            (model.dictAttributes.sfwMethodName == "btnLoadOnDemand_Click") ||
            (model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click") ||
            (model.dictAttributes.sfwMethodName == "btnNext_Click") ||
            (model.dictAttributes.sfwMethodName == "btnOpenDetail_Click") ||
            (model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click") ||
            (model.dictAttributes.sfwMethodName == "btnPrev_Click") ||
            (model.dictAttributes.sfwMethodName == "btnCancel_Click") ||
            (model.dictAttributes.sfwMethodName == "btnRefreshServers_Click") ||
            (model.dictAttributes.sfwMethodName == "btnReturnMaintenance_Click")) {
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            if (!(model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click")) {
                lstAttributes.push("sfwMessageId");
            }
            lstAttributes.push("sfwMode");
            if (!(model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click") &&
                !(model.dictAttributes.sfwMethodName == "btnCorrespondence_Click")) {
                lstAttributes.push("sfwObjectMethod");
            }

            if (model.dictAttributes.sfwMethodName == "btnCorrespondence_Click") {
                lstAttributes.push("sfwTriggerPostBack");
            }

            if ((model.dictAttributes.sfwMethodName == "btnAddNewChild_Click") ||
                (model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click") ||
                (model.dictAttributes.sfwMethodName == "btnNewPopupDialog_Click") ||
                (model.dictAttributes.sfwMethodName == "btnFinishPopupDialog_Click") ||
                (model.dictAttributes.sfwMethodName == "btnClosePopupDialog_Click")) {
                lstAttributes.push("sfwRelatedControl");
                if (!(model.dictAttributes.sfwMethodName == "btnAddNewChild_Click")) {
                    lstAttributes.push("sfwRelatedDialogPanel");
                }
            }
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            if (model.dictAttributes.sfwMethodName == "btnOpenPopupDialog_Click") {
                lstAttributes.push("sfwSelection");
            }
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click") ||
            (model.dictAttributes.sfwMethodName == "btnValidateExecuteBusinessMethod_Click") ||
            (model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethod_Click") ||
            (model.dictAttributes.sfwMethodName == "btnExecuteServerMethod_Click") || (model.dictAttributes.sfwMethodName == "btnCompleteWorkflowActivities_Click")) {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");

            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");

            if (model.dictAttributes.sfwMethodName != "btnExecuteServerMethod_Click") {
                lstAttributes.push("sfwObjectMethod");
            }
            lstAttributes.push("OnClientClick");
            if (model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
                lstAttributes.push("sfwRelatedControl");
            }
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            if (model.dictAttributes.sfwMethodName == "btnExecuteServerMethod_Click") {
                lstAttributes.push("sfwObjectMethod");
            }
            if (model.dictAttributes.sfwMethodName == "btnExecuteBusinessMethodSelectRows_Click") {
                lstAttributes.push("sfwSelection");
            }
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnWorkflowExecuteMethod_Click")) {

            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            if (model.dictAttributes.sfwMethodName != "btnCompleteWorkflowActivities_Click") {
                lstAttributes.push("sfwNavigationParameter");
            }
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
            lstAttributes.push("sfwWorkflowActivities");
        }
        else if (model.dictAttributes.sfwMethodName == "btnPdfCorrespondence_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }

            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");

            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwDownloadFileName");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwTriggerPostBack");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");

            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("sfwNavigationParameter");
        }
        else if ((model.dictAttributes.sfwMethodName == "btnWizardCancel_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardFindAndNext_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardFinish_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardNext_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardPrevious_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardSaveAndNext_Click") ||
            (model.dictAttributes.sfwMethodName == "btnWizardSaveAndPrevious_Click")) {
            lstAttributes.push("sfwActiveForm");
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");


            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
        }
        else if (model.dictAttributes.sfwMethodName == "btnGenerateReport_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnMasterDetailHeader_Click") {
            lstAttributes.push("sfwCheckChanges");
            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");
        }
        else if (model.dictAttributes.sfwMethodName == "btnCopyRecord_Click") {

            lstAttributes.push("sfwCustomAttributes");
            if (!(model.Name == "sfwLinkButton")) {
                lstAttributes.push("sfwEnableRule");
            }
            lstAttributes.push("sfwExecuteAfterSuccess");

            lstAttributes.push("sfwMode");
            lstAttributes.push("sfwMessageId");
            lstAttributes.push("sfwNavigationParameter");
            lstAttributes.push("sfwObjectMethod");
            lstAttributes.push("OnClientClick");
            lstAttributes.push("sfwResource");
            lstAttributes.push("sfwRelatedControl");
            lstAttributes.push("sfwSelection");
            lstAttributes.push("sfwSetPageMode");
            lstAttributes.push("sfwSecurityLevel");
            lstAttributes.push("sfwTriggerPostBack");
            lstAttributes.push("sfwSaveBeforeAction");
            lstAttributes.push("Text");
            lstAttributes.push("sfwUserLogParameters");
            lstAttributes.push("sfwVisibleRule");
            lstAttributes.push("Visible");

        }
        else {//Wizard--Maintenance
            if (formType == "Wizard" && (model.dictAttributes.CommandName != undefined && model.dictAttributes.CommandName != "")) {
                lstAttributes.push("sfwCheckChanges");
                lstAttributes.push("sfwCustomAttributes");
                lstAttributes.push("CommandName");
                if (!(model.Name == "sfwLinkButton")) {
                    lstAttributes.push("sfwEnableRule");
                }
                lstAttributes.push("sfwMode");
                lstAttributes.push("OnClientClick");
                lstAttributes.push("sfwResource");
                lstAttributes.push("sfwSecurityLevel");
                lstAttributes.push("Text");
                lstAttributes.push("sfwUserLogParameters");
                lstAttributes.push("sfwVisibleRule");
                lstAttributes.push("Visible");
            }
            else {
                lstAttributes.push("sfwActiveForm");
                lstAttributes.push("sfwCheckChanges");
                lstAttributes.push("sfwCustomAttributes");
                if (!(model.Name == "sfwLinkButton")) {
                    lstAttributes.push("sfwEnableRule");
                }
                lstAttributes.push("sfwMode");
                lstAttributes.push("sfwMessageId");
                if (model.dictAttributes.sfwMethodName != "btnBpmApprove_Click" && model.dictAttributes.sfwMethodName != "btnBpmSubmit_Click" && model.dictAttributes.sfwMethodName != "btnBpmReject_Click") {
                    lstAttributes.push("sfwNavigationParameter");
                }
                lstAttributes.push("sfwObjectMethod");
                lstAttributes.push("OnClientClick");
                lstAttributes.push("sfwResource");
                lstAttributes.push("sfwRelatedControl");
                lstAttributes.push("sfwSelection");
                lstAttributes.push("sfwSecurityLevel");
                lstAttributes.push("Text");
                lstAttributes.push("sfwUserLogParameters");
                lstAttributes.push("sfwVisibleRule");
                lstAttributes.push("Visible");
            }
        }
    }

    return lstAttributes;
}

//#region Get Primary Key
function getPrimarykey(EntityList, EntityName) {
    var lstPrimaryKey = [];
    var lst = EntityList.filter(function (x) { return x.ID == EntityName; });
    if (lst && lst.length > 0) {
        if (lst[0].Attributes.length > 0) {
            var primarykeyattribute = lst[0].Attributes.filter(function (x) { return x.Type == "Column" && x.KeyNo == '1'; });
            if (primarykeyattribute.length > 0) {
                lstPrimaryKey.push(primarykeyattribute[0]);
            }
        }
    }
    return lstPrimaryKey;
}

//#endregion

//#region Get Title for Lookup
function GetFormTitle(strType, FormName) {
    var strFormTitle = "";
    var strFormName = FormName + strType;

    for (var i = 0; i < strFormName.length; i++) {
        var ch = strFormName[i];
        var blnCaseChanged = false;

        if (i > 0) {
            if (CharCase(strFormName[i - 1]) < CharCase(strFormName[i])) {
                blnCaseChanged = true;
            }
        }

        if (blnCaseChanged) {
            strFormTitle += " ";
        }
        strFormTitle += strFormName[i];
    }

    return strFormTitle;
}
function CharCase(ch) {
    return (ch >= 'a') && (ch <= 'z') ? 0 : 1;
}
//#endregion

//#region Populate Server Method for Entity


function PopulateServerMethodForEntity(entityModel) {
    var lstServerMethodForEntity = [];
    if (entityModel) {
        var lstMethods = entityModel.Elements.filter(function (x) { return x.Name == "methods"; });
        if (lstMethods && lstMethods.length > 0) {
            var objMethods = lstMethods[0];
            angular.forEach(objMethods.Elements, function (method) {
                //if (method.dictAttributes.sfwRemoteObject != undefined && method.dictAttributes.sfwRemoteObject != "") {
                lstServerMethodForEntity.push(method);
                //}
            });
        }
    }
    return lstServerMethodForEntity;
}
//#endregion

function IsAuditField(strField) {
    strField = strField.toLowerCase();

    if (strField == "created_by") { return true; }
    if (strField == "created_date") { return true; }
    if (strField == "modified_by") { return true; }
    if (strField == "modified_date") { return true; }
    if (strField == "update_seq") { return true; }

    return false;
}

function PopulateGridDataField(sfxControl, lstColumnName) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.dictAttributes && ctrl.dictAttributes.sfwEntityField && ctrl.Name != "rowformat" && ctrl.Name != "cellformat" && ctrl.Name != "HeaderTemplate" && ctrl.Name != "FooterTemplate" && ctrl.Name != "Parameters" && ctrl.Name != "Data") {
            lstColumnName.push(ctrl.dictAttributes.sfwEntityField);
        }
        if (ctrl.Elements.length > 0 && ctrl.Name != "rowformat" && ctrl.Name != "cellformat" && ctrl.Name != "HeaderTemplate" && ctrl.Name != "FooterTemplate" && ctrl.Name != "Parameters" && ctrl.Name != "Data") {
            PopulateGridDataField(ctrl, lstColumnName);
        }
    });
}

function PopulateRetrievalControls(panel, lstRetrievalControl) {
    if (panel) {
        angular.forEach(panel.Elements, function (ctrl) {
            if (ctrl.Name == "sfwLabel" || ctrl.Name == "sfwDropDownList" || ctrl.Name == "sfwTextBox" || ctrl.Name == "sfwCascadingDropDownList" || ctrl.Name == "sfwMultiSelectDropDownList" || ctrl.Name == "sfwCheckBox") {
                var obj = { ID: ctrl.dictAttributes.ID, ControlID: "" };
                lstRetrievalControl.push(obj);
            }
            if (ctrl.Elements.length > 0 && ctrl.Name != "sfwGridView") {
                PopulateRetrievalControls(ctrl, lstRetrievalControl);
            }
        });
    }
}

function PopulateSortExpressions(sfxControl, lstSortExpressions) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.dictAttributes.sfwEntityField && ctrl.dictAttributes.sfwEntityField != "") {
            lstSortExpressions.push(ctrl.dictAttributes.sfwEntityField);
        }
        if (ctrl.Elements.length > 0 && ctrl.Name && (ctrl.Name == "Columns" || ctrl.Name == "ItemTemplate" || ctrl.Name == "TemplateField")) {
            PopulateSortExpressions(ctrl, lstSortExpressions);
        }
    });
}

function PopulateRetrievalOrAutoCompleteParameters(panel, lstParameters, formType, IsRetrieval) {
    if (panel) {
        angular.forEach(panel.Elements, function (ctrl) {

            if (ctrl.Name != "sfwPanel" && ctrl.Name != "sfwWizardStep" && ctrl.Name != "sfwGridView") {
                if (formType == "Lookup" && ctrl.dictAttributes.sfwDataField && ctrl.dictAttributes.ID) {
                    var obj = { ID: ctrl.dictAttributes.ID, Name: ctrl.Name, ControlID: "", Elements: [], IsExpanded: false };
                    lstParameters.push(obj);
                }
                else if ((formType == "Maintenance" || formType == "Wizard" || formType == "UserControl") && ctrl.dictAttributes.sfwEntityField && ctrl.dictAttributes.ID) {
                    var obj = { ID: ctrl.dictAttributes.ID, Name: ctrl.Name, ControlID: "", Elements: [], IsExpanded: false };
                    lstParameters.push(obj);
                }
                else if (formType == "Correspondence" && ctrl.dictAttributes.sfwQueryID && ctrl.dictAttributes.ID) {
                    var obj = { ID: ctrl.dictAttributes.ID, Name: ctrl.Name, ControlID: "", Elements: [], IsExpanded: false };
                    lstParameters.push(obj);
                }
            }

            if (ctrl.Elements.length > 0) {
                var lsttempParameters = lstParameters;
                if (ctrl.Name == "sfwPanel") {
                    var id = ctrl.dictAttributes.sfwCaption;
                    if (id == undefined || id == "") {
                        id = ctrl.dictAttributes.ID;
                    }
                    var obj = { ID: id, Elements: [], Name: ctrl.Name, IsExpanded: false };
                    lstParameters.push(obj);
                    lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                }
                if (ctrl.Name == "sfwWizardStep") {
                    var id = ctrl.dictAttributes.Title;
                    if (id == undefined || id == "") {
                        id = ctrl.dictAttributes.ID;
                    }
                    var obj = { ID: id, Elements: [], Name: ctrl.Name, IsExpanded: false };
                    lstParameters.push(obj);
                    lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                }
                //if (ctrl.Name == "sfwGridView") {
                //    var obj = { ID: ctrl.dictAttributes.ID + "(Data Keys)", Elements: [], IsExpanded: false }
                //    lstParameters.push(obj);
                //    lsttempParameters = lstParameters[lstParameters.length - 1].Elements;
                //    PopulateGridEntityField(ctrl, lsttempParameters);
                //}
                PopulateRetrievalOrAutoCompleteParameters(ctrl, lsttempParameters, formType, IsRetrieval);
            }
        });
    }
}

function PopulateGridEntityField(sfxControl, lstParameters) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        //if (ctrl.Name == "sfwLabel" || ctrl.Name == "sfwDropDownList" || ctrl.Name == "sfwTextBox" || ctrl.Name == "sfwCascadingDropDownList") {
        if (ctrl.dictAttributes.sfwEntityField && ctrl.Name != "rowformat" && ctrl.Name != "cellformat" && ctrl.Name != "HeaderTemplate" && ctrl.Name != "FooterTemplate") {
            var obj = { ID: ctrl.dictAttributes.sfwEntityField, ControlID: "", Elements: [], IsExpanded: false };
            lstParameters.push(obj);
        }

        //}
        if (ctrl.Elements.length > 0 && ctrl.Name != "rowformat" && ctrl.Name != "cellformat" && ctrl.Name != "HeaderTemplate" && ctrl.Name != "FooterTemplate") {
            PopulateGridEntityField(ctrl, lstParameters);
        }
    });
}

function PopulateFormEntityField(sfxControl, lstEntityField, IsLookup) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.dictAttributes.sfwEntityField && !IsLookup) {
            lstEntityField.push(ctrl.dictAttributes.sfwEntityField);
        }
        else if (ctrl.dictAttributes.sfwDataField && IsLookup) {
            lstEntityField.push(ctrl.dictAttributes.sfwDataField);
        }
        if (ctrl.Elements.length > 0) {
            if (ctrl.Name != "sfwGridView" && ctrl.Name != "sfwDialogPanel") {
                PopulateFormEntityField(ctrl, lstEntityField, IsLookup);
            }
        }
    });
}

function PopulateFormLinkEntityField(sfxControl, lstEntityField, IsLookup) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.dictAttributes.sfwEntityField && !IsLookup) {
            lstEntityField.push(ctrl.dictAttributes.sfwEntityField);
        }
        else if (ctrl.dictAttributes.sfwDataField && IsLookup) {
            lstEntityField.push(ctrl.dictAttributes.sfwDataField);
        }
        if (ctrl.Elements.length > 0) {
            PopulateFormLinkEntityField(ctrl, lstEntityField, IsLookup);
        }
    });
}

function PopulateGridID(sfxControl, currentGridID, lstParentGrid) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name == "sfwGridView" && ctrl.dictAttributes.ID && ctrl.dictAttributes.ID != currentGridID) {
            lstParentGrid.push(ctrl.dictAttributes.ID);
        }
        if (ctrl.Elements && ctrl.Elements.length > 0) {
            PopulateGridID(ctrl, currentGridID, lstParentGrid);
        }
    });
}

function GetVMUsingID(ControlName, sfxControl, ctrlID, lstEntityField) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name == ControlName && ctrl.dictAttributes.ID == ctrlID) {
            PopulateEntityField(ctrl, lstEntityField);
        }
        if (ctrl.Elements.length > 0) {
            GetVMUsingID(ControlName, ctrl, ctrlID, lstEntityField);
        }
    });
}
function PopulateEntityField(sfxControl, lstParameters) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.dictAttributes.sfwEntityField) {
            lstParameters.push(ctrl.dictAttributes.sfwEntityField);
        }
        if (ctrl.Elements.length > 0) {
            PopulateEntityField(ctrl, lstParameters);
        }
    });
}

function GetAllControls(objSfxForm, astrControlClass, astrPanelID, astrGridID, IsFormLink, alControls, isAllPanel) {

    var iblnOnlyGrid = (astrGridID != "");
    angular.forEach(objSfxForm.Elements, function (objTable) {

        if (objTable.Name == "items" && IsFormLink) {
            GetAllFormControls(objTable, astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid);

        }
        if (objTable.Name == "sfwTable") {
            GetAllControlsFormMainTable(objTable, astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel);
        }
    });
}

function GetAllControlsFormMainTable(sfxTable, astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel) {
    var strArrControlClass = astrControlClass.split(',');

    if (sfxTable) {
        var strTableID = sfxTable.dictAttributes.ID;
        function iterateWizardSteps(sfxWizardStep) {
            if (!iblnOnlyGrid) {
                if (!astrPanelID || (astrPanelID && strTableID != "" && strTableID != undefined && astrPanelID.toLowerCase() == strTableID.toLowerCase()))  //Apurba 12/09/2011
                {
                    if (strArrControlClass.length > 0) {
                        angular.forEach(strArrControlClass, function (strControlClass) {
                            if (strControlClass == sfxWizardStep.Name) {
                                if (sfxWizardStep.dictAttributes && sfxWizardStep.dictAttributes.ID) {
                                    alControls.push(sfxWizardStep);
                                }
                            }
                        });
                    }
                    else {
                        if (sfxWizardStep.dictAttributes && sfxWizardStep.dictAttributes.ID) {
                            alControls.push(sfxWizardStep);
                        }
                    }
                }
            }
            if (sfxWizardStep.Elements.length > 0 && sfxWizardStep.Elements[0].Name == "sfwTable") {
                GetAllControlsFormMainTable(sfxWizardStep.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel);
            }
        }
        function iterateWizardItems(objWizard) {
            if (objWizard.Name == "HeaderTemplate") {
                strTreeCaption = "HeaderTemplate";
                if (!iblnOnlyGrid) {
                    if (!astrPanelID || (astrPanelID && strTableID != "" && strTableID != undefined && astrPanelID.toLowerCase() == strTableID.toLowerCase()))  //Apurba 12/09/2011
                    {
                        if (strArrControlClass.length > 0) {
                            angular.forEach(strArrControlClass, function (strControlClass) {
                                if (strControlClass == objWizard.Name) {
                                    if (objWizard.dictAttributes && objWizard.dictAttributes.ID) {
                                        alControls.push(objWizard);
                                    }
                                }
                            });
                        }
                        else {
                            if (objWizard.dictAttributes && objWizard.dictAttributes.ID) {
                                alControls.push(objWizard);
                            }
                        }
                    }
                }
                if (objWizard.Elements.length > 0 && objWizard.Elements[0].Name == "sfwTable") {
                    GetAllControlsFormMainTable(objWizard.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel);
                }

            }
            else {

                angular.forEach(objWizard.Elements, iterateWizardSteps);
            }
        }
        angular.forEach(sfxTable.Elements, function (sfxRow) {
            angular.forEach(sfxRow.Elements, function (sfxCell) {
                if (sfxCell) {
                    angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                        if (!iblnOnlyGrid) {
                            if (!astrPanelID || (astrPanelID && strTableID != undefined && strTableID != "" && (astrPanelID.toLowerCase() == strTableID.toLowerCase()) || isAllPanel)) {
                                if (strArrControlClass.length > 0) {
                                    angular.forEach(strArrControlClass, function (strControlClass) {
                                        if (strControlClass == sfxCtrl.Name) {
                                            if (sfxCtrl.dictAttributes && sfxCtrl.dictAttributes.ID) {
                                                alControls.push(sfxCtrl);
                                            }
                                        }
                                    });
                                }
                                else {
                                    if (sfxCtrl.dictAttributes && sfxCtrl.dictAttributes.ID) {
                                        alControls.push(sfxCtrl);
                                    }
                                }
                            }
                        }
                        if (sfxCtrl.Name == "sfwChart") {
                            var objSfxChart = sfxCtrl;
                            angular.forEach(objSfxChart.Elements, function (objChart) {
                                if (objChart.Name == "Series") {
                                    for (var i = 0; i < objChart.Elements.length; i++) {
                                        var objSeries = objChart.Elements[i];
                                        if (objSeries.Name == "sfwSeries") {
                                            if (objSeries.dictAttributes && objSeries.dictAttributes.ID) {
                                                alControls.push(objSeries);
                                            }
                                        }
                                    }
                                }
                                if (objChart.Name == "ChartAreas") {
                                    for (var i = 0; i < objChart.Elements.Count; i++) {
                                        var objChartArea = objChart.Elements[i];
                                        if (objChartArea.Name == "sfwChartArea") {
                                            if (objChartArea.dictAttributes && objChartArea.dictAttributes.ID) {
                                                alControls.push(objChartArea);
                                            }
                                        }
                                    }
                                }
                            });
                        }


                        if (sfxCtrl.Name == "sfwPanel") {
                            var sfxPanel = sfxCtrl;
                            if (sfxPanel.Elements.length > 0 && sfxPanel.Elements[0].Name == "sfwTable") {
                                GetAllControlsFormMainTable(sfxPanel.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel);
                            }
                        }
                        else if (sfxCtrl.Name == "sfwTabContainer") {
                            var sfxTabContainer = sfxCtrl;
                            if (sfxTabContainer.Elements.length > 0 && sfxTabContainer.Elements[0].Name == "Tabs") {
                                var sfxTab = sfxTabContainer.Elements[0];
                                angular.forEach(sfxTab.Elements, function (sfxTabSheet) {
                                    if (!iblnOnlyGrid) {
                                        if (strTableID != undefined && strTableID != "" && astrPanelID.toLowerCase() == strTableID.toLowerCase())  //Apurba 12/09/2011
                                        {
                                            if (strArrControlClass.length > 0) {
                                                angular.forEach(strArrControlClass, function (strControlClass) {
                                                    if (strControlClass == sfxTabSheet.Name)
                                                        if (sfxTabSheet.dictAttributes && sfxTabSheet.dictAttributes.ID) {
                                                            alControls.push(sfxTabSheet);
                                                        }
                                                });
                                            }
                                            else
                                                if (sfxTabSheet.dictAttributes && sfxTabSheet.dictAttributes.ID) {
                                                    alControls.push(sfxTabSheet);
                                                }
                                        }
                                    }
                                    if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "sfwTable") {
                                        GetAllControlsFormMainTable(sfxTabSheet.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid, isAllPanel);
                                    }
                                });
                            }
                        }
                        else if (sfxCtrl.Name == "sfwGridView") {
                            var strGridId = sfxCtrl.dictAttributes.ID;
                            if (iblnOnlyGrid) {
                                if (strGridId == astrGridID) {
                                    if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                                        var objColumn = sfxCtrl.Elements[0];
                                        for (var i = 0; i < objColumn.Elements.length; i++) {
                                            var objTempField = objColumn.Elements[i];
                                            if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                                angular.forEach(objTempField.Elements[0].Elements, function (sfxControl) {
                                                    alControls.push(sfxControl);
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (sfxCtrl.Name == "sfwWizard") {
                            angular.forEach(sfxCtrl.Elements, iterateWizardItems);
                        }
                        else if (sfxCtrl.Name == "sfwButtonGroup") {
                            for (var i = 0; i < sfxCtrl.Elements.length; i++) {
                                if (sfxCtrl.Elements[i].Name == "sfwScheduler") {
                                    if (sfxCtrl.Elements[i].dictAttributes && sfxCtrl.Elements[i].dictAttributes.ID) {
                                        alControls.push(sfxCtrl.Elements[i]);
                                        break;
                                    }
                                }
                            }
                        }
                    });
                }
            });
        });
    }
}
function GetAllFormControls(sfxItems, astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid) {
    var strArrControlClass = astrControlClass.split(',');
    function iterateWizardSteps(sfxWizardStep) {
        if (!iblnOnlyGrid) {
            if (strArrControlClass.length > 0) {
                angular.forEach(strArrControlClass, function (strControlClass) {
                    if (strControlClass == sfxWizardStep.Name) {
                        if (sfxWizardStep.dictAttributes && sfxWizardStep.dictAttributes.ID) {
                            alControls.push(sfxWizardStep);
                        }
                    }
                });
            }
            else {
                if (sfxWizardStep.dictAttributes && sfxWizardStep.dictAttributes.ID) {
                    alControls.push(sfxWizardStep);
                }
            }
        }
        if (sfxWizardStep.Elements.length > 0 && sfxWizardStep.Elements[0].Name == "items") {
            GetAllFormControls(sfxWizardStep.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid);
        }
    }
    function iterateWizardItems(objWizard) {
        if (objWizard.Name == "HeaderTemplate") {
            strTreeCaption = "HeaderTemplate";
            if (!iblnOnlyGrid) {
                if (strArrControlClass.length > 0) {
                    angular.forEach(strArrControlClass, function (strControlClass) {
                        if (strControlClass == objWizard.Name) {
                            if (objWizard.dictAttributes && objWizard.dictAttributes.ID) {
                                alControls.push(objWizard);
                            }
                        }
                    });
                }
                else {
                    if (objWizard.dictAttributes && objWizard.dictAttributes.ID) {
                        alControls.push(objWizard);
                    }
                }
            }
            if (objWizard.Elements.length > 0 && objWizard.Elements[0].Name == "items") {
                GetAllFormControls(objWizard.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid);
            }

        }
        else {

            angular.forEach(objWizard.Elements, iterateWizardSteps);
        }
    }

    if (sfxItems) {
        var strTableID = sfxItems.dictAttributes.ID;
        angular.forEach(sfxItems.Elements, function (sfxCtrl) {
            {
                if (!iblnOnlyGrid) {
                    //if (strTableID != "" && strTableID != undefined && astrPanelID.toLowerCase() == strTableID.toLowerCase())  //Apurba 12/09/2011
                    //{
                    if (strArrControlClass.length > 0) {
                        angular.forEach(strArrControlClass, function (strControlClass) {
                            if (strControlClass == sfxCtrl.Name) {
                                if (sfxCtrl.dictAttributes && sfxCtrl.dictAttributes.ID) {
                                    alControls.push(sfxCtrl);
                                }
                            }
                        });

                    }
                    else {
                        if (sfxCtrl.dictAttributes && sfxCtrl.dictAttributes.ID) {
                            alControls.push(sfxCtrl);
                        }
                    }
                    //}
                }
                //Manish for SfwChart
                if (sfxCtrl.Name == "sfwChart") {
                    var objSfxChart = sfxCtrl;
                    angular.forEach(objSfxChart.Elements, function (objChart) {
                        if (objChart.Name == "Series") {
                            for (var i = 0; i < objChart.Elements.length; i++) {
                                var objSeries = objChart.Elements[i];
                                if (objSeries.Name == "sfwSeries") {
                                    if (objSeries.dictAttributes && objSeries.dictAttributes.ID) {
                                        alControls.push(objSeries);
                                    }
                                }
                            }
                        }
                        if (objChart.Name == "ChartAreas") {
                            for (var i = 0; i < objChart.Elements.Count; i++) {
                                var objChartArea = objSfxChart.Elements[i];
                                if (objChartArea.Name == "sfwChartArea") {
                                    if (objChartArea.dictAttributes && objChartArea.dictAttributes.ID) {
                                        alControls.push(objChartArea);
                                    }
                                }
                            }
                        }
                    });
                }


                if (sfxCtrl.Name == "sfwPanel") {
                    var sfxPanel = sfxCtrl;
                    if (sfxPanel.Elements.length > 0 && sfxPanel.Elements[0].Name == "items") {
                        GetAllFormControls(sfxPanel.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid);
                    }
                }
                else if (sfxCtrl.Name == "sfwTabContainer") {
                    var sfxTabContainer = sfxCtrl;
                    if (sfxTabContainer.Elements.length > 0 && sfxTabContainer.Elements[0].Name == "Tabs") {
                        var sfxTab = sfxTabContainer.Elements[0];
                        angular.forEach(sfxTab.Elements, function (sfxTabSheet) {
                            if (!iblnOnlyGrid) {
                                //if (strTableID != "" && strTableID != undefined && astrPanelID.toLowerCase() == strTableID.toLowerCase())  //Apurba 12/09/2011
                                //{
                                if (strArrControlClass.length > 0) {
                                    angular.forEach(strArrControlClass, function (strControlClass) {
                                        if (strControlClass == sfxTabSheet.Name) {
                                            if (sfxTabSheet.dictAttributes && sfxTabSheet.dictAttributes.ID) {
                                                alControls.push(sfxTabSheet);
                                            }
                                        }
                                    });
                                }
                                else {
                                    if (sfxTabSheet.dictAttributes && sfxTabSheet.dictAttributes.ID) {
                                        alControls.push(sfxTabSheet);
                                    }
                                }
                                //}
                            }
                            if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "items") {
                                GetAllFormControls(sfxTabSheet.Elements[0], astrControlClass, astrPanelID, astrGridID, alControls, iblnOnlyGrid);
                            }
                        });
                    }
                }

                else if (sfxCtrl.Name == "sfwGridView") {
                    var strGridId = sfxCtrl.dictAttributes.ID;
                    if (iblnOnlyGrid) {
                        if (strGridId == astrGridID) {
                            if (sfxCtrl.Elements.length > 0 && sfxCtrl.Elements[0].Name == "Columns") {
                                var objColumn = sfxCtrl.Elements[0];
                                for (var i = 0; i < objColumn.Elements.length; i++) {
                                    var objTempField = objColumn.Elements[i];
                                    if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                        angular.forEach(objTempField.Elements[0].Elements, function (sfxControl) {
                                            alControls.push(sfxControl);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else if (sfxCtrl.Name == "sfwWizard") {
                    angular.forEach(sfxCtrl.Elements, iterateWizardItems);
                }
            }
        });
    }
}

function GetEntityXMLMethodParameters(entityList, entityID, strMethod) {

    var entity = entityList.filter(function (x) { return x.ID == entityID; });
    var objMethod;

    if (entity && entity.length) {
        var objEntity = entity[0];
        angular.forEach(objEntity.XmlMethods, function (item) {
            if (!objMethod) {
                if (item.ID == strMethod) {
                    objMethod = item;
                }
            }
        });

    }
    if (objMethod) {
        return objMethod.Parameters;
    }
    else {
        return objMethod;
    }
}

function GetObjectMethodParameters(entityList, entityID, strMethod) {
    var entity = entityList.filter(function (x) { return x.ID == entityID; });
    var objMethod;

    if (entity && entity.length) {
        var objEntity = entity[0];
        angular.forEach(objEntity.ObjectMethods, function (item) {
            if (!objMethod) {
                if (item.ID == strMethod) {
                    objMethod = item;
                }
            }
        });

    }
    if (objMethod) {
        return objMethod.Parameters;
    }
    else {
        return objMethod;
    }
}

function GetObjectMethods(entityList, entityID) {
    var entity = entityList.filter(function (x) { return x.ID == entityID; });
    var lstMethod = [];

    if (entity && entity.length) {
        var objEntity = entity[0];
        angular.forEach(objEntity.ObjectMethods, function (item) {
            lstMethod.push(item);
        });
    }
    return lstMethod;
}

function GetSrvMethodParameters(objServerObject, strObjectMethod) {
    var objMethod;
    if (objServerObject) {
        angular.forEach(objServerObject.Elements, function (item) {
            if (!objMethod) {
                if (item.dictAttributes.ID == strObjectMethod) {
                    objMethod = item;
                }
            }
        });
    }
    if (objMethod) {
        return objMethod.Elements;
    }
    return objMethod;
}

function GetCriteriaPanel(sfxTable) {
    var sfxCriteriaTable;
    angular.forEach(sfxTable.Elements, function (objModel) {
        if (!sfxCriteriaTable) {
            if (objModel.dictAttributes.ID) {
                var strID = objModel.dictAttributes.ID.toLowerCase().trim();
            }
            if (strID == "tblcriteria")
                sfxCriteriaTable = objModel;
            else
                sfxCriteriaTable = GetCriteriaPanel(objModel);
        }
    });
    return sfxCriteriaTable;
}



function PopulateTextRelatedControls(aModel, lstRelatedControl) {
    if (aModel.Name == "sfwTextBox") {
        if (aModel.dictAttributes.ID) {
            var strCtrlID = aModel.dictAttributes.ID;
            if (strCtrlID.length > 0 && !strCtrlID.match("^idsfw")) {
                lstRelatedControl.push(strCtrlID);
            }
        }
    }
    angular.forEach(aModel.Elements, function (itm) {
        PopulateTextRelatedControls(itm, lstRelatedControl);
    });
}

function GetControlNames(objTable, lstSelectedIndex) {
    angular.forEach(objTable.Elements, function (objRows) {
        angular.forEach(objRows.Elements, function (objCell) {
            angular.forEach(objCell.Elements, function (objsfxControl) {
                if (objsfxControl.Name == "sfwTable") {
                    GetControlNames(objsfxControl, lstSelectedIndex);
                }
                else if (objsfxControl.Name == "sfwPanel") {
                    angular.forEach(objsfxControl.Elements, function (objPanelTable) {
                        if (objPanelTable.Name == "sfwTable") {
                            GetControlNames(objPanelTable, lstSelectedIndex);
                        }
                    });
                }
                else if (objsfxControl.Name == "sfwGridView") {

                    if (objsfxControl.Elements.length > 0 && objsfxControl.Elements[0].Name == "Columns") {
                        for (var j = 0; j < objsfxControl.Elements[0].Elements.length; j++) {
                            var objTempField = objsfxControl.Elements[0].Elements[j];
                            if (objTempField.Elements.length > 0 && objTempField.Elements[0].Name == "ItemTemplate") {
                                var objItemTempField = objTempField.Elements[0];
                                angular.forEach(objItemTempField.Elements, function (sfxControl) {
                                    if ("sfwEntityField" in sfxControl.dictAttributes) {
                                        var strFieldName = "";

                                        strFieldName = sfxControl.dictAttributes.sfwEntityField;

                                        lstSelectedIndex.push(strFieldName);
                                    }
                                });
                            }
                        }
                    }
                }

            });
        });
    });
}

function GetRetrievalTargetControls(panelObj, lstTargetFields) {
    angular.forEach(panelObj.Elements, function (ctrl) {
        if (ctrl.Name == "sfwTextBox" || ctrl.Name == "sfwCheckBox" || ctrl.Name == "sfwDropDownList" || ctrl.Name == "sfwRadioButtonList" || ctrl.Name == "sfwMultiSelectDropDownList" || ctrl.Name == "sfwCascadingDropDownList") {
            if (ctrl.dictAttributes.ID) {
                var obj = { ID: ctrl.dictAttributes.ID, sourceControl: "", isConstant: false };
                lstTargetFields.push(obj);
            }
        }
        if (ctrl.Elements.length > 0 && ctrl.Name != "sfwGridView") {
            GetRetrievalTargetControls(ctrl, lstTargetFields);
        }
    });
}

function GetRetrievalSourceControls(panelObj, lstSourceFields) {
    angular.forEach(panelObj.Elements, function (ctrl) {
        if (ctrl.Name == "sfwTextBox" || ctrl.Name == "sfwCheckBox" || ctrl.Name == "sfwDropDownList" || ctrl.Name == "sfwRadioButtonList" || ctrl.Name == "sfwMultiSelectDropDownList" || ctrl.Name == "sfwCascadingDropDownList") {
            if (ctrl.dictAttributes.ID) {
                var obj = { ID: ctrl.dictAttributes.ID, Elements: [], IsExpanded: false };
                lstSourceFields.push(obj);
            }
        }
        if (ctrl.Elements.length > 0 && ctrl.Name != "sfwGridView" && ctrl.Name != "sfwDialogPanel") {
            var lsttempSourceFields = lstSourceFields;
            if (ctrl.Name == "sfwPanel") {
                var obj = { ID: ctrl.dictAttributes.sfwCaption, Elements: [], IsExpanded: false };
                lstSourceFields.push(obj);
                lsttempSourceFields = lstSourceFields[lstSourceFields.length - 1].Elements;
            }
            else if (ctrl.Name == "sfwWizardStep") {
                var obj = { ID: ctrl.dictAttributes.Title, Elements: [], IsExpanded: false };
                lstSourceFields.push(obj);
                lsttempSourceFields = lstSourceFields[lstSourceFields.length - 1].Elements;
            }
            GetRetrievalSourceControls(ctrl, lsttempSourceFields);
        }
    });
}

//#region Get Query params

function onQueryChange(QueryRef, entityIntellisenseList) {
    var lstQryField = [];
    if (QueryRef) {

        var queryId = QueryRef;
        var lst = queryId.split('.');
        if (lst && lst.length == 2) {
            var entityName = lst[0];
            var strQueryID = lst[1];
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
                    angular.forEach(objQuery.Parameters, function (x) {
                        lstQryField.push(x);
                    });
                }
            }
        }
    }
    return lstQryField;
}
//#endregion

function CheckIfObjectType(StrEntityField, entityName, lstEntity) {
    var retVal = false;
    if (entityName != undefined && entityName != "") {
        var lst = lstEntity.filter(function (itm) {
            return itm.ID == entityName;
        }
        );
        if (lst && lst.length > 0) {
            var objEntity = lst[0];
            if (StrEntityField.contains('.')) {
                var strEntityFields = StrEntityField.split(".");
                for (var i = 0; i < strEntityFields.length; i++) {
                    retVal = false;
                    var objAttribute = SearchAttributeInEntity(objEntity, strEntityFields[i]);
                    if (objAttribute && objAttribute.DataType == "Object") {
                        retVal = true;
                    }
                    if (objAttribute && objAttribute.Entity != undefined && objAttribute.Entity != "") {
                        lst = lstEntity.filter(function (itm) {
                            return itm.ID == objAttribute.Entity;
                        }
                        );
                        if (lst && lst.length > 0) {
                            objEntity = lst[0];
                        }
                    }
                }
            }
            else {
                var objAttribute = SearchAttributeInEntity(objEntity, StrEntityField);
                if (objAttribute && objAttribute.DataType == "Object") {
                    retVal = true;
                }
            }
        }
    }
    return retVal;
}

function SearchAttributeInEntity(objEntity, strAttr) {
    var objAttribute;

    var lstAttr = objEntity.Attributes.filter(function (itm) {
        return itm.ID == strAttr;
    }
    );
    if (lstAttr && lstAttr.length > 0) {
        objAttribute = lstAttr[0];
    }

    return objAttribute;
}

function GetQueryListFromObject(formObject) {
    var lstQueryID = [];
    var initialload = formObject.Elements.filter(function (x) { return x.Name == 'initialload'; });

    if (initialload.length > 0) {
        for (i = 0; i < initialload[0].Elements.length; i++) {
            if (initialload[0].Elements[i].Name == "query") {
                lstQueryID.push(initialload[0].Elements[i]);
            }
        }
    }
    return lstQueryID;
}

function GetMainQueryFromFormObject(formObject, entityIntellisenseList) {
    var initialload = formObject.Elements.filter(function (x) { return x.Name == 'initialload'; });
    var MainQuery;
    if (initialload && initialload.length > 0) {
        for (i = 0; i < initialload[0].Elements.length; i++) {
            var sfwQueryRef = initialload[0].Elements[i].dictAttributes.sfwQueryRef;
            if (!IsSubQuery(sfwQueryRef, entityIntellisenseList)) {
                MainQuery = initialload[0].Elements[i];
            }
        }
    }
    return MainQuery;
}

function IsSubQuery(strQuery, entityIntellisenseList) {
    var retValue = false;
    function iterator(Query) {
        if (!retValue) {
            if (Query.ID == strQueryName && Query.QueryType && Query.QueryType.toLowerCase() == "subselectquery") {
                retValue = true;
            }
        }
    }
    if (strQuery != "" && strQuery != undefined) {
        var strCDOName = strQuery.substring(0, strQuery.indexOf("."));
        var lstObj = entityIntellisenseList.filter(function (x) {
            return x.ID == strCDOName;
        });
        if (lstObj && lstObj.length > 0) {

            var strQueryName = strQuery.substring(strQuery.indexOf(".") + 1);

            angular.forEach(lstObj[0].Queries, iterator);
        }
    }

    return retValue;
}


function getEntityBasedOnControl(item) {
    var entityfieldname = "";
    var curscope = getCurrentFileScope();

    if (item.Name == "sfwGridView") {
        entityfieldname = item.dictAttributes.sfwEntityField;
    }
    else if (item.Name == "sfwDialogPanel") {
        var strdialogpanelid = item.dictAttributes.ID;
        if (strdialogpanelid) {
            var button = GetFieldFromFormObject(curscope.SfxMainTable, 'sfwButton', 'sfwRelatedDialogPanel', strdialogpanelid);
            if (button && button.length > 0 && button[0].dictAttributes.sfwRelatedControl) {
                var gridview = GetFieldFromFormObject(curscope.SfxMainTable, 'sfwGridView', 'ID', button[0].dictAttributes.sfwRelatedControl);
                if (gridview && gridview.length > 0) {
                    entityfieldname = gridview[0].dictAttributes.sfwEntityField;
                }
            }
            else {
                var objScheduler = GetFieldFromFormObject(curscope.SfxMainTable, 'sfwScheduler', 'sfwRelatedDialogPanel', strdialogpanelid);
                if (objScheduler && objScheduler.length > 0) {
                    entityfieldname = objScheduler[0].dictAttributes.sfwEntityField;
                }
            }
        }
    }
    else if (item.Name == "sfwListView") {
        entityfieldname = item.dictAttributes.sfwEntityField;
    }
    return entityfieldname;
}

function GetNewSeriesName(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    while (CheckForDuplicateName(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    }

    return strItemName;
}

function CheckForDuplicateName(strId, objRules) {
    var blnReturn = false;
    if (objRules) {
        if (objRules.dictAttributes.Name) {
            blnReturn = objRules.dictAttributes.Name == strId;
        }
        else if (objRules.dictAttributes.ID) {
            blnReturn = objRules.dictAttributes.ID == strId;
        }
        if (!blnReturn) {
            angular.forEach(objRules.Elements, function (item) {
                if (!blnReturn) {
                    blnReturn = CheckForDuplicateName(strId, item);
                    if (blnReturn) {
                        return;
                    }
                }
            });
        }
    }
    return blnReturn;
}

function GetNavParamCollection(objSfxForm, objControl, ParameterCollection, entityIntellisenseList, formodel) {
    if (objSfxForm) {
        if (objSfxForm.dictAttributes.sfwType == "Lookup") {
            var lstTable = objSfxForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
            if (lstTable && lstTable.length > 0)
                var sfxCriteriaPanel = GetCriteriaPanel(lstTable[0]);

            if (sfxCriteriaPanel) {
                angular.forEach(sfxCriteriaPanel.Elements, function (sfxRow) {
                    angular.forEach(sfxRow.Elements, function (sfxCell) {
                        if (sfxCell) {
                            angular.forEach(sfxCell.Elements, function (sfxCtrl) {
                                if ("sfwDataField" in sfxCtrl.dictAttributes) {
                                    var strFieldName = sfxCtrl.dictAttributes.sfwDataField;
                                    var strControlID = sfxCtrl.dictAttributes.ID;
                                    if (strFieldName != "" && strFieldName != undefined)
                                        AddParamGridRow("Criteria Field", strFieldName, "", strControlID, ParameterCollection);
                                }
                            });
                        }
                    });
                });
            }
        }
        else {
            var astrMethodName = objControl.dictAttributes.sfwMethodName;

            var blnNewButton = astrMethodName == "btnNew_Click"
                || astrMethodName == "btnUpdate_Click" || astrMethodName == "btnSaveNew_Click";

            var InitialLoadVM;
            var lst = objSfxForm.Elements.filter(function (x) { return x.Name == "initialload"; });
            if (lst && lst.length > 0) {
                InitialLoadVM = lst[0];
            }

            if (InitialLoadVM) {
                var strMethod = "";
                var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || x.dictAttributes.sfwMode == "Update"); });
                if (lst && lst.length) {
                    strMethod = lst[0].dictAttributes.sfwMethodName;
                }

                if (blnNewButton) {
                    var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "callmethods" && (x.dictAttributes.sfwMode == "" || x.dictAttributes.sfwMode == undefined || x.dictAttributes.sfwMode == "New"); });
                    if (lst && lst.length) {
                        strMethod = lst[0].dictAttributes.sfwMethodName;
                    }
                }

                if (strMethod != undefined && strMethod != "") {
                    if (objSfxForm.dictAttributes.sfwRemoteObject != undefined && objSfxForm.dictAttributes.sfwRemoteObject != "") {
                        var objServerObject = GetServerMethodObject(objSfxForm.dictAttributes.sfwRemoteObject, formodel.RemoteObjectCollection);
                        var paramerters = GetSrvMethodParameters(objServerObject, strMethod);
                        if (paramerters) {
                            for (j = 0; j < paramerters.length; j++) {
                                AddParamGridRow(paramerters[j].dictAttributes.sfwDataType, paramerters[j].dictAttributes.ID, "", "", ParameterCollection);
                            }
                        }
                    }
                    else {
                        var paramerters = GetEntityXMLMethodParameters(entityIntellisenseList, objSfxForm.dictAttributes.sfwEntity, strMethod);
                        if (paramerters) {
                            angular.forEach(paramerters, function (objParam) {
                                AddParamGridRow(objParam.DataType, objParam.ID, objParam.Value, "", ParameterCollection);
                            });
                        }
                    }

                }
                if (blnNewButton) {
                    var lst = InitialLoadVM.Elements.filter(function (x) { return x.Name == "session"; });
                    if (lst && lst.length > 0) {
                        angular.forEach(lst[0].Elements, function (objSessionField) {
                            AddParamGridRow("Session Field", objSessionField.dictAttributes.ID, "", "", ParameterCollection);
                        });
                    }
                }
            }
        }
    }
}


function AddParamGridRow(astrParamType, astrParamField, astrParamValue, astrControlID, ParameterCollection) {
    var objParameter = {};
    objParameter.Type = astrParamType;
    objParameter.ParameterField = astrParamField;
    if (astrParamValue != undefined && astrParamValue != "") {
        if (astrParamValue.match("^#")) {
            objParameter.Constants = true;
            objParameter.ParameterValue = astrParamValue.substring(1);
        }
        else {
            objParameter.ParameterValue = astrParamValue;
        }
        objParameter.IsReadOnly = true;

    }
    objParameter.ControlID = astrControlID;
    ParameterCollection.push(objParameter);
}

function PopulateParamValues(ParameterCollection, istrParameters) {
    if (istrParameters != undefined && istrParameters != "") {
        if (istrParameters.match("^@")) {
            istrParameters = istrParameters.substring(1);
            var alParams = istrParameters.split(';');
            angular.forEach(alParams, function (strParam) {
                var strControlID = strParam;
                var strParamsValue = strParam;
                var blnConstant = false;

                if (strParam.contains("=")) {
                    strControlID = strParam.substring(0, strParam.indexOf('='));
                    strParamsValue = strParam.substring(strParam.indexOf('=') + 1);

                    if (strParamsValue.match("^#")) {
                        strParamsValue = strParamsValue.substring(1);
                        blnConstant = true;
                    }
                }

                angular.forEach(ParameterCollection, function (objParameter) {
                    if (objParameter.ControlID == strControlID) {
                        objParameter.ParameterValue = strParamsValue;
                        objParameter.Constants = blnConstant;
                    }
                });
            });
        }
        else {
            var alParams = istrParameters.split(';');
            angular.forEach(alParams, function (strParam) {
                var strParamField = strParam;
                var strParamsValue = strParam;
                var blnConstant = false;

                if (strParam.contains("=")) {
                    strParamField = strParam.substring(0, strParam.indexOf('='));
                    strParamsValue = strParam.substring(strParam.indexOf('=') + 1);

                    if (strParamsValue.match("^#")) {
                        strParamsValue = strParamsValue.substring(1);
                        blnConstant = true;
                    }
                    else if (strParamsValue.match("^~")) {
                        strParamsValue = strParamsValue.substring(1);
                    }
                }

                angular.forEach(ParameterCollection, function (objParameter) {
                    if (objParameter.ParameterField.toLowerCase() == strParamField.toLowerCase()) {
                        objParameter.ParameterValue = strParamsValue;
                        objParameter.Constants = blnConstant;
                    }
                });
            });
        }
    }
}

function GetNavigationParameterValue(ParameterCollection) {
    var strReturn = "";
    angular.forEach(ParameterCollection, function (objParams) {
        var strParamField = objParams.ParameterField;
        var strParamValue = objParams.ParameterValue;
        if ((strParamValue != undefined && strParamValue != "") || (strParamField != undefined && strParamField != "")) {
            var blnConstatnt = objParams.Constants;

            if (blnConstatnt) {
                strParamValue = "#" + strParamValue;
            }

            var strParam = strParamValue;

            if (strParamValue != undefined && strParamValue != "" && strParamValue.toLowerCase() != strParamField.toLowerCase()) {
                strParam = strParamField + '=' + strParamValue;
            }
            if (strParam != undefined && strParam != "") {

                if (strReturn == "") {
                    strReturn = strParam;
                }
                else {
                    strReturn += ';' + strParam;
                }
            }
        }
    });
    return strReturn;
}

function GetAttribute(attribute, objEntity) {
    var objAttribute;
    var attrs = objEntity.Attributes.filter(function (attr) {
        return attr.ID == attribute;
    }
    );
    if (attrs && attrs.length > 0) {
        objAttribute = attrs[0];
    }
    return objAttribute;
}

function getEntityObject(entityName, $EntityIntellisenseFactory) {
    var objEntity;
    var lstentitylist = $EntityIntellisenseFactory.getEntityIntellisense();
    var lstEntity = lstentitylist.filter(function (itm) {
        return itm.ID == entityName;
    }
    );
    if (lstEntity && lstEntity.length > 0) {
        objEntity = lstEntity[0];
    }
    return objEntity;
}

function persistAttributes(oldControlModel, newControlModel) {
    if (oldControlModel && newControlModel && oldControlModel.dictAttributes && newControlModel.dictAttributes) {

        //Retain General attributes if any.
        if (oldControlModel.dictAttributes.ID && oldControlModel.dictAttributes.ID.trim().length > 0) {
            var isCaption = oldControlModel.Name == "sfwLabel" && oldControlModel.dictAttributes.sfwIsCaption && oldControlModel.dictAttributes.sfwIsCaption == "True";
            var oldControlPrefix = getControlIDPrefix(oldControlModel.Name, isCaption);
            var id = getControlIDPrefix(newControlModel.Name);
            if (oldControlModel.Name == "sfwLabel" && (oldControlModel.dictAttributes.ID.startsWith("cap") || oldControlModel.dictAttributes.ID.startsWith("lbl"))) {
                id += oldControlModel.dictAttributes.ID.substring(3);
            }
            else if (oldControlModel.dictAttributes.ID.startsWith(oldControlPrefix)) {
                id += oldControlModel.dictAttributes.ID.substring(oldControlPrefix.length);
            }
            else {
                id += oldControlModel.dictAttributes.ID;
            }

            newControlModel.dictAttributes.ID = id;
        }
        if (oldControlModel.dictAttributes.Visible && oldControlModel.dictAttributes.Visible.trim().length > 0) {
            newControlModel.dictAttributes.Visible = oldControlModel.dictAttributes.Visible;
        }
        if (oldControlModel.dictAttributes.sfwResource && oldControlModel.dictAttributes.sfwResource.trim().length > 0) {
            newControlModel.dictAttributes.sfwResource = oldControlModel.dictAttributes.sfwResource;
        }
        if (oldControlModel.dictAttributes.sfwVisibleRule && oldControlModel.dictAttributes.sfwVisibleRule.trim().length > 0) {
            newControlModel.dictAttributes.sfwVisibleRule = oldControlModel.dictAttributes.sfwVisibleRule;
        }
        if (oldControlModel.dictAttributes.sfwEnableRule && oldControlModel.dictAttributes.sfwEnableRule.trim().length > 0 && newControlModel.Name == 'sfwButton') {
            newControlModel.dictAttributes.sfwEnableRule = oldControlModel.dictAttributes.sfwEnableRule;
        }
        if (oldControlModel.dictAttributes.CssClass && oldControlModel.dictAttributes.CssClass.trim().length > 0) {
            newControlModel.dictAttributes.CssClass = oldControlModel.dictAttributes.CssClass;
        }
        if (oldControlModel.dictAttributes.sfwCustomAttributes && oldControlModel.dictAttributes.sfwCustomAttributes.trim().length > 0) {
            newControlModel.dictAttributes.sfwCustomAttributes = oldControlModel.dictAttributes.sfwCustomAttributes;
        }
        if (oldControlModel.dictAttributes.sfwMode && oldControlModel.dictAttributes.sfwMode.trim().length > 0) {
            newControlModel.dictAttributes.sfwMode = oldControlModel.dictAttributes.sfwMode;
        }

        //Retains Entity Field or Data Field attributes if any.
        if (newControlModel.Name != "sfwButton" && newControlModel.Name != "sfwImageButton" && newControlModel.Name != "sfwSoftErrors" && newControlModel.Name != "sfwLinkButton" && newControlModel.Name != "sfwToolTipButton" && newControlModel.Name != "CompareValidator" && newControlModel.Name != "sfwDialogPanel" && newControlModel.Name != "sfwListBox" && newControlModel.Name != "sfwButtonGroup" && !(newControlModel.Name == "sfwLabel" && newControlModel.dictAttributes.sfwIsCaption == "True")) {
            if (oldControlModel.dictAttributes.sfwEntityField && oldControlModel.dictAttributes.sfwEntityField.trim().length > 0) {
                newControlModel.dictAttributes.sfwEntityField = oldControlModel.dictAttributes.sfwEntityField;

            }
            else if (oldControlModel.dictAttributes.sfwDataField && oldControlModel.dictAttributes.sfwDataField.trim().length > 0) {
                newControlModel.dictAttributes.sfwDataField = oldControlModel.dictAttributes.sfwDataField;
            }
            else if (oldControlModel.dictAttributes.sfwObjectField && oldControlModel.dictAttributes.sfwObjectField.trim().length > 0) {
                newControlModel.dictAttributes.sfwObjectField = oldControlModel.dictAttributes.sfwObjectField;
            }
        }

    }
}

function getEntityAttributeByType(entities, entityid, type) {
    var lstEntityColumnList = [];
    var entity = entities.filter(function (x) {
        return x.ID == entityid;
    });

    if (entity.length > 0) {
        var attributes = entity[0].Attributes;
        lstEntityColumnList = attributes.filter(function (itm) { return itm.Type == type; });
    }
    return lstEntityColumnList;
}
function PopulateColumnList(queryid, formodel, entityIntellisenseList, lstloadedentitycolumnstree) {
    var lstColumnList = [];
    var attributeName = "ID";
    var result = {};
    if (queryid) {
        lstColumnList = PopulateQueryColumnFromList(queryid, lstloadedentitycolumnstree, formodel);
    }
    else {
        if (formodel) {
            var MainQuery = GetMainQueryFromFormObject(formodel, entityIntellisenseList);
            if (MainQuery) {
                lstColumnList = PopulateQueryColumnFromList(MainQuery.dictAttributes.ID, lstloadedentitycolumnstree, formodel);
            }
            else {
                attributeName = "Value";
                if (formodel.dictAttributes.sfwEntity) {
                    var entities = entityIntellisenseList;
                    var entity = entities.filter(function (x) {
                        return x.ID == formodel.dictAttributes.sfwEntity;
                    });
                    if (entity.length > 0) {
                        var attributes = entity[0].Attributes;
                        lstColumnList = attributes.filter(function (itm) { return itm.Type == "Column"; });
                    }
                }
            }
        }
    }
    result.list = lstColumnList;
    result.attribute = attributeName;
    return result;
}

function PopulateQueryColumnFromList(queryid, lstloadedentitycolumnstree, formodel) {
    var blnFound = false;
    var lstColumnList = [];
    if (lstloadedentitycolumnstree) {

        var lst = lstloadedentitycolumnstree.filter(function (itm) {
            return itm.EntityName == queryid;
        });
        if (lst && lst.length > 0) {
            lstColumnList = JSON.parse(JSON.stringify(lst[0].lstselectedobjecttreefields));
            blnFound = true;
        }
    }

    if (!blnFound && lstloadedentitycolumnstree) {
        var lstQueryID = GetQueryListFromObject(formodel);
        if (lstQueryID && lstQueryID.length > 0) {
            var lst = lstQueryID.filter(function (itm) {
                return itm.dictAttributes.ID == queryid;
            }
            );
            if (lst && lst.length > 0) {
                var objnew = { EntityName: lst[0].dictAttributes.ID, IsVisible: true, selectedobjecttreefield: undefined, lstselectedobjecttreefields: [], IsQuery: true };
                lstloadedentitycolumnstree.push(objnew);

                if (lst[0].dictAttributes.sfwQueryRef) {
                    $.connection.hubForm.server.getEntityQueryColumns(lst[0].dictAttributes.sfwQueryRef, "LoadQueryFieldsForLookup").done(function (data) {
                        var scope = GetFormFileScope(formodel);
                        if (scope && scope.receiveQueryFields) {
                            scope.receiveQueryFields(data, lst[0].dictAttributes.sfwQueryRef);
                        }
                    });
                }
                blnFound = true;
                if (!objnew.lstselectedobjecttreefields) {
                    objnew.lstselectedobjecttreefields = [];
                }
                lstColumnList = objnew.lstselectedobjecttreefields;
            }
        }
    }
    return lstColumnList;
}
function getQueryBookMarksID(formModel) {
    var lstQueryBookMarksID = [];
    var lstQueryBookMarks = formModel.Elements.filter(function (itm) { return itm.Name == "sfwQueryBookMarks"; });
    if (lstQueryBookMarks && lstQueryBookMarks.length > 0) {

        angular.forEach(lstQueryBookMarks[0].Elements, function (itm) {
            lstQueryBookMarksID.push(itm.dictAttributes.ID);
        });
    }

    return lstQueryBookMarksID;

}


function PopulateControlsForActiveForm(alControls, formmodel, model, iblnIsLookup) {
    var IsHtxForm = false;
    if (formmodel.dictAttributes.sfwType == "FormLinkLookup" ||
        formmodel.dictAttributes.sfwType == "FormLinkMaintenance" ||
        formmodel.dictAttributes.sfwType == "FormLinkWizard") {
        IsHtxForm = true;
    }
    var altmpControls = [];
    if (model.dictAttributes.sfwMethodName == "btnNew_Click" || model.dictAttributes.sfwMethodName == "btnNewUpdate_Click") {
        if (iblnIsLookup)
            GetAllControls(formmodel, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton", "tblcriteria", "", IsHtxForm, altmpControls, false);
        else
            GetAllControls(formmodel, "sfwLabel,sfwTextBox,sfwDropDownList,sfwCheckBox,sfwRadioButtonList,sfwLinkButton,sfwScheduler", "", "", IsHtxForm, altmpControls, true);
    }
    else if (model.dictAttributes.sfwMethodName == "btnOpen_Click") {
        var strGridID = model.dictAttributes.sfwRelatedControl;
        var objGridView = FindParent(model, "sfwGridView");
        if (objGridView && objGridView.dictAttributes) {
            strGridID = objGridView.dictAttributes.ID;
        }
        if (strGridID != "")
            GetAllControls(formmodel, "", "", strGridID, IsHtxForm, altmpControls);
    }

    angular.forEach(altmpControls, function (objCtrl) {
        var strCodeGroup = "";
        if ("sfwLoadSource" in objCtrl.dictAttributes) {
            strCodeGroup = objCtrl.dictAttributes.sfwLoadSource;
        }
        if (strCodeGroup == "" || strCodeGroup == "0") {
            strCodeGroup = "~";
            if (objCtrl.Elements.length > 0) {
                // System.Web.UI.WebControls.ListItemCollection listItems = ((System.Web.UI.WebControls.ListControl)objCtrl.webctrl).Items;
                angular.forEach(objCtrl.Elements, function (item) {
                    if (item.Name == "ListItem") {
                        if (item.dictAttributes.Value != undefined && item.dictAttributes.Value != "") {
                            strCodeGroup += item.dictAttributes.Value + ",";
                        }
                    }
                });
            }
        }
        if ("sfwEntityField" in objCtrl.dictAttributes && (objCtrl.dictAttributes.sfwEntityField != undefined && objCtrl.dictAttributes.sfwEntityField != ""))
            alControls.push(objCtrl.dictAttributes.sfwEntityField + "~" + strCodeGroup);
    });
}
function CheckForFilterGrid(objForm) {
    var lst = [];
    FindControlListByNames(objForm, ['sfwButton', 'sfwLinkButton', 'sfwImageButton'], lst);
    if (lst && lst.length > 0) {
        lst = lst.filter(function (itm) { return itm.dictAttributes.sfwMethodName == "btnGridSearch_Click"; });
        if (lst && lst.length > 0) {
            for (var i = 0; i < lst.length; i++) {
                if (lst[i].dictAttributes.sfwNavigationParameter) {
                    var lstParam = lst[i].dictAttributes.sfwNavigationParameter.split(';');
                    if (lstParam && lstParam.length > 0) {
                        for (j = 0; j < lstParam.length; j++) {
                            var model = FindControlByID(objForm, lstParam[j]);
                            if (model) {
                                model.IsShowDataField = true;
                            }
                        }
                    }
                }
            }
        }
    }
}

function CanAddControlToDropList(objWebControl, formmodel, model) {
    var canAddControl = false;
    if (formmodel && formmodel.dictAttributes && formmodel.dictAttributes.sfwType == "Lookup" && objWebControl.lstCustom && objWebControl.lstCustom.some(function (x) { return x.PropertyName == "sfwDataField"; })) {
        canAddControl = true;
    }
    else if (formmodel && formmodel.dictAttributes && (formmodel.dictAttributes.sfwType == "Maintenance" || formmodel.dictAttributes.sfwType == "Wizard" || formmodel.dictAttributes.sfwType == "UserControl") && objWebControl.lstCustom && objWebControl.lstCustom.some(function (x) { return x.PropertyName == "sfwEntityField"; })) {
        canAddControl = true;
    }
    else if (formmodel && formmodel.dictAttributes && formmodel.dictAttributes.sfwType == "Correspondence" && objWebControl.lstCustom && objWebControl.lstCustom.some(function (x) { return x.PropertyName == "sfwQueryID"; })) {
        canAddControl = true;
    }

    if ((model.Name == "sfwLabel" && model.dictAttributes.sfwIsCaption == "True") || model.Name == "sfwGridView" ||
        model.Name == "sfwChart" || model.Name == "sfwListView" || model.Name == "sfwPanel" || model.Name == "sfwCheckBoxList" || model.Name == "sfwButton" || model.Name == "sfwLinkButton" || model.Name == "sfwImageButton") {
        canAddControl = false;
    }

    return canAddControl;
}


//#region Validate New

function PopulateButtonID(sfxControl, lstButton) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name == "sfwButton" && ctrl.dictAttributes.sfwMethodName == "btnNew_Click" && ctrl.dictAttributes.ID) {
            lstButton.push(ctrl);
        }
        if (ctrl.Elements.length > 0) {
            PopulateButtonID(ctrl, lstButton);
        }
    });
    return lstButton;
}
function PopulateControlID(sfxControl, lstControlID) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if ((ctrl.Name == "sfwTextBox" || ctrl.Name == "sfwDropDownList" || ctrl.Name == "sfwCascadingDropDownList" || ctrl.Name == "sfwMultiSelectDropDownList") && ctrl.dictAttributes.ID && ctrl.dictAttributes.ID != "") {
            lstControlID.push(ctrl.dictAttributes.ID);
        }
        if (ctrl.Elements.length > 0 && ctrl.Name != "sfwGridView") {
            PopulateControlID(ctrl, lstControlID);
        }
    });
    return lstControlID;
}

//#endregion

function CreateCompatibleLabel(objCtrl, formodel, $EntityIntellisenseFactory, $rootScope) {
    var strDisplayName = "";
    var strFieldName = objCtrl.dictAttributes.sfwDataField;
    if (!strFieldName) {
        strFieldName = objCtrl.dictAttributes.sfwEntityField;
        if (!strFieldName) {
            strFieldName = objCtrl.dictAttributes.sfwObjectField;
        }
        if (!strFieldName && formodel.dictAttributes.sfwType == "Correspondence") { // for correspondence
            strFieldName = objCtrl.dictAttributes.sfwQueryID;
        }
    }
    var formScope = getCurrentFileScope();
    var objAttribute;

    objAttribute = GetAttributesFromList(strFieldName, formodel, $EntityIntellisenseFactory);
    if (objAttribute) {
        if (formodel && !formodel.IsLookupCriteriaEnabled) {
            strDisplayName = GetCaptionFromField(objAttribute);
        }
        else if (formScope && formodel.IsLookupCriteriaEnabled == true && (!formScope.MainQuery && (!formScope.SelectedQuery || (formScope.SelectedQuery && !formScope.SelectedQuery.dictAttributes.ID)))) {
            strDisplayName = GetCaptionFromField(objAttribute);
        } else {
            strDisplayName = GetCaptionFromFieldName(strFieldName);
        }
    }

    if (strFieldName) {
        if (!strDisplayName) {
            strDisplayName = GetCaptionFromFieldName(strFieldName);
        }
        if (objCtrl.Name != "sfwGridView")
            if (strDisplayName && strDisplayName.trim().length > 0 && !strDisplayName.contains(":")) {
                strDisplayName += " :";
            }
    }
    else
        strDisplayName = "Error! No field is binded with the control";

    var cellVM = objCtrl.ParentVM;
    if (cellVM) {
        var rowVM = cellVM.ParentVM;
        if (rowVM) {
            var index = rowVM.Elements.indexOf(cellVM);
            if (index > 0) {
                var objcell = rowVM.Elements[index - 1];

                var prefix = "swc";
                var objLabel = {
                    Name: "sfwLabel", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: []
                };
                objLabel.ParentVM = objcell;
                objLabel.dictAttributes.Text = strDisplayName;
                objLabel.dictAttributes.sfwIsCaption = "True";
                var strId = objLabel.Name;
                if (objLabel.Name && objLabel.Name.startsWith("sfw")) {
                    strId = objLabel.Name.substring(3);
                }
                objLabel.dictAttributes.ID = CreateControlID(formodel, strId, objLabel.Name, true);

                var strAssociatedID = objCtrl.dictAttributes.ID;
                if (strAssociatedID) {
                    objLabel.dictAttributes.AssociatedControlID = strAssociatedID;
                }

                $rootScope.PushItem(objLabel, objcell.Elements);
            }
        }
    }
}

function GetAttributesFromList(strFieldName, formodel, $EntityIntellisenseFactory) {
    var objEntity = getEntityObject(formodel.dictAttributes.sfwEntity, $EntityIntellisenseFactory);
    var objAttribute;
    var field = strFieldName;
    if (strFieldName != undefined && strFieldName != "" && strFieldName.contains(".")) {
        var lst = strFieldName.split(".");
        for (var i = 0; i < lst.length; i++) {
            if (objAttribute) {
                if (objAttribute.Entity != undefined && objAttribute.Entity != "") {
                    objEntity = getEntityObject(objAttribute.Entity, $EntityIntellisenseFactory);
                }
            }
            objAttribute = GetAttribute(lst[i], objEntity);
            field = lst[i];
        }
    }

    if (objEntity) {
        objAttribute = GetAttribute(field, objEntity);
    }
    return objAttribute;
}
function getFilteredAttribute(astrControlName, lstAttributes, ablnIsMultiLevel, propertyName) {
    var tempAttributes = [];
    if (astrControlName && lstAttributes && lstAttributes.length > 0) {
        if ("sfwChart" == astrControlName && propertyName && (propertyName == "XAxisTitle" || propertyName == "YAxisTitle" || propertyName == "ChartName")) {
            if (ablnIsMultiLevel) tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type == "Object"; });
            else {
                tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type.toLowerCase() != "expression"; });
            }
        }
        else if (["sfwGridView", "sfwChart", "sfwListView"].indexOf(astrControlName) > -1) {
            if (ablnIsMultiLevel) tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type == "Object"; });
            else {
                tempAttributes = lstAttributes.filter(function (x) { return x.Type && (x.Type.toLowerCase() == "object" || x.Type.toLowerCase() == "collection"); });
            }
        } else if ("sfwCheckBoxList" == astrControlName && propertyName && propertyName == "sfwEntityField") {
            if (ablnIsMultiLevel) tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type == "Object"; });
            else {
                tempAttributes = lstAttributes.filter(function (x) { return x.Type && (x.Type == "CDOCollection"); });
            }
        }
        else if ("sfwCalendar" == astrControlName && propertyName && propertyName == "sfwEntityField") {
            if (ablnIsMultiLevel) tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type == "Object"; });
            else {
                tempAttributes = lstAttributes.filter(function (x) { return x.Type && (x.Type.toLowerCase() == "object" || x.Type.toLowerCase() == "collection"); });
            }
        } else if (astrControlName == "sfwLabel") {
            tempAttributes = lstAttributes;
        } else if (astrControlName == "udc") {
            tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type == "Object"; });
        }
        else {
            tempAttributes = lstAttributes.filter(function (x) { return x.Type && x.Type.toLowerCase() != "expression"; });
        }
    }
    return tempAttributes;
}

function PopulateGridIDForNewButton(sfxControl, lstGrid) {
    angular.forEach(sfxControl.Elements, function (ctrl) {
        if (ctrl.Name == "sfwGridView" && ctrl.dictAttributes.ID) {
            lstGrid.push(ctrl.dictAttributes.ID);
        }
        if (ctrl.Elements && ctrl.Elements.length > 0) {
            PopulateGridIDForNewButton(ctrl, lstGrid);
        }
    });
}

function GetFormFileScope(formodel) {
    var formName = "";
    if (formodel && formodel.dictAttributes) {
        formName = formodel.dictAttributes.ID;
    }
    var mainScope = getScopeByFileName("MainPage");
    if (formName) {
        var scope = getScopeByFileName(formName);
        if (!scope) {
            formName = formName.substring(3);
            formName = "htx" + formName;
            scope = getScopeByFileName(formName);
        }
    }
    if (!scope) {
        if (mainscope) {
            var curfileid = mainscope.$root.currentopenfile.file.FileName;
            scope = getScopeByFileName(curfileid);
        }
    }
    return scope;
}




function GetObjectFields(formodel, lstFields, sfxControl) {
    var iblnIsLookup = formodel.dictAttributes.sfwType.toUpperCase() == "LOOKUP";
    lstFields.push("");


    if (formodel.Elements.length > 0) {
        angular.forEach(formodel.Elements, function (objTableM) {
            if (objTableM.Name == "sfwTable") {
                GetObjectFieldForTable(sfxControl, objTableM, iblnIsLookup, lstFields);
            }
        });
    }
}

function IsSearchCriteria(objControl) {
    var retVal = false;
    if (objControl && objControl.Name == "sfwTable" && (objControl.dictAttributes.ID == "tblCriteria" || objControl.dictAttributes.ID == "tblAdvCriteria" || objControl.dictAttributes.ID == "tblAdvSort" || objControl.dictAttributes.ID == "tblSql")) {
        retVal = true;
    }
    function iterator(itm) {
        if (!retVal) {
            retVal = IsSearchCriteria(itm);
        }
    }
    if (!retVal) {
        if (objControl && objControl.Elements.length > 0) {

            angular.forEach(objControl.Elements, iterator);
        }
    }

    return retVal;
}


function GetObjectFieldForTable(sfxControl, BaseTable, isLookup, lstFields, AddElements, isPanelCriteria) {
    angular.forEach(BaseTable.Elements, function (objRowM) {
        angular.forEach(objRowM.Elements, function (objCellM) {
            angular.forEach(objCellM.Elements, function (objControl) {


                if (isLookup) {
                    if (objControl.Name == "sfwPanel" && IsSearchCriteria(objControl)) {
                        if (objControl.Elements.length > 0 && objControl.Elements[0].Name == "sfwTable") {
                            GetObjectFieldForTable(sfxControl, objControl.Elements[0], isLookup, lstFields, true, true);
                        }
                    }
                    else if (objControl.Name == "sfwTabContainer") {
                        if (objControl.Elements.length > 0 && objControl.Elements[0].Name == "Tabs") {
                            var sfxTabs = objControl.Elements[0];
                            angular.forEach(sfxTabs.Elements, function (sfxTabSheet) {
                                if (sfxTabSheet.Elements.length > 0 && sfxTabSheet.Elements[0].Name == "sfwTable") {
                                    GetObjectFieldForTable(sfxControl, sfxTabSheet.Elements[0], isLookup, lstFields, true, true);
                                }
                            });
                        }
                    }
                }
                else {
                    if (objControl.Name == "sfwPanel") {
                        if (objControl.Elements.length > 0 && objControl.Elements[0].Name == "sfwTable") {
                            GetObjectFieldForTable(sfxControl, objControl.Elements[0], isLookup, lstFields, true, false);
                        }
                    }
                }

                if (isLookup && AddElements && isPanelCriteria) {

                    if (objControl.dictAttributes.sfwDataField != undefined && objControl.dictAttributes.sfwDataField != "") {
                        if (sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
                            if (objControl.Name == "sfwLabel" || objControl.Name == "sfwTextBox" || objControl.Name == "sfwDropDownList" || objControl.Name == "sfwCheckBox" || objControl.Name == "sfwRadioButtonList" || objControl.Name == "sfwLinkButton") {
                                lstFields.push(objControl.dictAttributes.ID);
                            }
                        }
                        else {
                            lstFields.push(objControl.dictAttributes.sfwDataField);
                        }
                    }

                }
                else if (AddElements) {
                    if (objControl.dictAttributes.sfwEntityField != undefined && objControl.dictAttributes.sfwEntityField != "") {
                        if (sfxControl.dictAttributes.sfwMethodName == "btnNew_Click") {
                            if (objControl.Name == "sfwLabel" || objControl.Name == "sfwTextBox" || objControl.Name == "sfwDropDownList" || objControl.Name == "sfwCheckBox" || objControl.Name == "sfwRadioButtonList" || objControl.Name == "sfwLinkButton") {
                                lstFields.push(objControl.dictAttributes.ID);
                            }
                        }
                        else {
                            lstFields.push(objControl.dictAttributes.sfwEntityField);
                        }
                    }
                }
            });
        });
    });
}

//#region Functions For Insert  and Move Row and Column

function InsertRow(aParam, iRowIndex, tableVM) {
    var prefix = "swc";
    var sfxRowModel = { Name: "sfwRow", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    sfxRowModel.ParentVM = tableVM;

    var rowvm;
    if (aParam.Name == "sfwColumn") {
        rowvm = aParam.ParentVM;
    }
    else if (aParam.Name == "sfwRow") {
        rowvm = aParam;
    }
    var ColCount = GetMaxColCount(rowvm, tableVM);
    for (var colInd = 0; colInd < ColCount; colInd++) {
        var sfxCellModel = { Name: "sfwColumn", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
        sfxCellModel.ParentVM = sfxRowModel;
        sfxRowModel.Elements.push(sfxCellModel);
    }

    return sfxRowModel;
}

function GetIndexToInsert(isBelow, iRowIndex) {
    var index;
    if (isBelow) {
        index = iRowIndex + 1;
    }
    else {
        index = iRowIndex;
    }
    return index;
}

//#endregion

function AddListViewTable(FormModel, objRepeaterControl) {
    var prefix = "swc";

    var objListTableModel = { Name: "sfwTable", value: '', prefix: prefix, dictAttributes: {}, Elements: [], Children: [] };
    objListTableModel.ParentVM = objRepeaterControl;
    var strCtrlId = CreateControlID(FormModel, "NewPage", "sfwTable");
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
    return objListTableModel;
}

var setParentControlName = function (control) {
    for (var i = 0; i < control.Elements.length > 0; i++) {
        if (control.Elements[i].Name == "sfwPanel" || control.Elements[i].Name == "sfwDialogPanel" || control.Elements[i].Name == "sfwListView") {
            control.Elements[i].ParentControlName = 'udc';
        }
        if (control.Elements[i].Elements.length > 0) {
            setParentControlName(control.Elements[i]);
        }
    }
};


function GetVisiblePropertyForHtmlControls(ControlName) {
    var lstProperties = [];
    if (ControlName == "sfwGridView") {
        lstProperties = ["Visible", "AllowGrouping", "AllowPaging", "AllowEditing", "AllowFiltering", "AllowSorting", "ShowHeader", "EmptyDataText", "PageSize", "ShowHeaderWhenEmpty"];
    }
    else if (ControlName == "TemplateField") {
        lstProperties = ["Visible", "HeaderText", "SortExpression"];
    }
    else if (ControlName == "sfwTabSheet") {
        lstProperties = ["Visible", "HeaderText", "DefaultButton"];
    }
    else if (ControlName == "sfwDropDownList") {
        lstProperties = ["Visible", "OnClientChange", "DataTextField", "DataValueField", "AutoPostBack"];
    }
    else if (ControlName == "sfwCascadingDropDownList" || ControlName == "sfwListPicker" || ControlName == "sfwSourceList") {
        lstProperties = ["Visible", "DataTextField", "DataValueField"];
    }
    else if (ControlName == "sfwCheckBoxList") {
        lstProperties = ["Visible", "RepeatColumns", "RepeatDirection", "DataTextField", "DataValueField"];
    }
    else if (ControlName == "sfwButton" || ControlName == "sfwLinkButton") {

        lstProperties = ["Visible", "OnClientClick"];
    }
    else if (ControlName == "sfwRadioButton") {
        lstProperties = ["Visible", "GroupName", "AutoPostBack"];
    }
    else if (ControlName == "sfwRadioButtonList") {
        lstProperties = ["Visible", "RepeatColumns", "RepeatDirection", "DataTextField", "DataValueField"];
    }
    else if (ControlName == "sfwPanel") {
        lstProperties = ["Visible", "DefaultButton"];
    }
    else if (ControlName == "sfwFileUpload") {
        lstProperties = ["Visible", "HeaderStatusUpload", "SelectButtonText", "StatusFailedMessage", "StatusUploadedMessage", "UploadButtonText", "UploadClick", "AllowMultiple"];
    }
    else if (ControlName == "sfwIamge") {
        lstProperties = ["Visible", "ImageUrl"];
    }
    else if (ControlName == "sfwImageButton") {
        lstProperties = ["Visible", "ImageUrl", "ImageAlign"];
    }
    else if (ControlName == "sfwLabel") {
        lstProperties = ["Visible", "AssociatedControlID", "ToolTip"];
    }
    else if (ControlName == "sfwTextBox") {
        lstProperties = ["Visible", "TextMode"];
    }
    else if (ControlName == "ListItem") {
        lstProperties = ["Visible", "Text", "Value"];
    }
    else if (ControlName == "sfwMultiSelectDropDownList") {
        lstProperties = ["AutoPostBack"];
    }
    return lstProperties;
};

function GetEntityFieldNameFromControl(SfxMainTable, item) {
    var entityfieldname = "";
    if (item.Name == "sfwGridView") {
        entityfieldname = item.dictAttributes.sfwEntityField;
    }
    else if (item.Name == "sfwDialogPanel") {
        var strdialogpanelid = item.dictAttributes.ID;
        if (strdialogpanelid) {
            var button = GetFieldFromFormObject(SfxMainTable, 'sfwButton', 'sfwRelatedDialogPanel', strdialogpanelid);
            if (button && button.length > 0 && button[0].dictAttributes.sfwRelatedControl) {
                var gridview = GetFieldFromFormObject(SfxMainTable, 'sfwGridView', 'ID', button[0].dictAttributes.sfwRelatedControl);
                if (gridview && gridview.length > 0) {
                    entityfieldname = gridview[0].dictAttributes.sfwEntityField;
                }
            }
            else {
                var objScheduler = GetFieldFromFormObject(SfxMainTable, 'sfwScheduler', 'sfwRelatedDialogPanel', strdialogpanelid);
                if (objScheduler && objScheduler.length > 0) {
                    entityfieldname = objScheduler[0].dictAttributes.sfwEntityField;
                }
            }
        }
    }
    else if (item.Name == "sfwListView") {
        entityfieldname = item.dictAttributes.sfwEntityField;
    }

    return entityfieldname;
}

function hasCaption(objControl) {
    var _hasCaption = false;
    if (objControl) {
        var objParentTable = FindParent(objControl, "sfwTable");
        if (objParentTable) {
            var lstDescendents = getDescendents(objParentTable);
            var lstCaptions = lstDescendents.filter(function (ctrl) { return ctrl.dictAttributes && ctrl.dictAttributes.sfwIsCaption === "True" && objControl.dictAttributes.ID && ctrl.dictAttributes.AssociatedControlID === objControl.dictAttributes.ID });
            if (lstCaptions && lstCaptions.length > 0) {
                _hasCaption = true;
            }
        }
    }
    return _hasCaption;
}

function isGridPresentInsidePanel(obj) {
    var isFound = false;
    if (obj && obj.Elements.length > 0) {
        for (var i = 0; i < obj.Elements.length; i++) {
            if (obj.Elements[i].Name == "sfwGridView") {
                isFound = true;
                break;
            } else if (obj.Elements[i].Elements.length > 0 && !isFound) {
                isFound = isGridPresentInsidePanel(obj.Elements[i]);
            }
        }
    }
    return isFound;
}


//#region Populate Available Control ( for grid, scheduler)

function PopulateAvailableControl(fieldControlCollection, mainTable, isNewParams, IsAddColumnSelected) {
    if (!IsAddColumnSelected) {
        var mainItem = {
            Text: "Main", Items: [], IsSelected: false, IsCheckBoxVisible: false
        };

        var strProperty = "sfwEntityField";
        var isNewButton = false;
        if (isNewParams) {
            strProperty = "ID";
            isNewButton = true;
        }

        PopulateAvailableFields(strProperty, mainTable, mainItem, true, false, isNewButton);

        if (mainItem.Items.length > 0) {
            fieldControlCollection.push(mainItem);
        }
    }
};

//#endregion

//#region Populate Global Params( for grid, scheduler)

function PopulateGlobalParameters(objGlobleParameters, fieldControlCollection) {
    function AddInobjGlobalParam(itm) {
        if (itm.dictAttributes && itm.dictAttributes.ID) {
            var strFieldName = itm.dictAttributes.ID;
            if (!globalParameters.filter(function (itm) { return itm == strFieldName.trim(); })) {
                globalParameters.push(strFieldName.trim());
            }
            var mainItem = { Text: "~" + strFieldName, Items: [], IsSelected: false, IsCheckBoxVisible: true };
            objGlobalParam.Items.push(mainItem);
        }
    }
    if (objGlobleParameters) {

        var globalParameters = [];
        if (objGlobleParameters.Elements.length > 0) {
            var objGlobalParam = { Text: "Global Parameters", Items: [], IsSelected: false, IsCheckBoxVisible: false };
            angular.forEach(objGlobleParameters.Elements, AddInobjGlobalParam);
            if (objGlobalParam.Items.length > 0) {
                fieldControlCollection.push(objGlobalParam);

            }

        }
    }
};

//#endregion

//#region get navigation Params( for grid, scheduler)

function GetNavigationParameters(lstParameters) {
    var strParameters = "";
    if (lstParameters) {
        for (i = 0; i < lstParameters.length; i++) {
            var grdParam = lstParameters[i];
            if (!grdParam) {
                continue;
            }
            var strParamValue = grdParam.ParmeterValue;

            if (strParamValue) {
                var blnConstant = grdParam.ParmeterConstant;
                if (blnConstant)
                    strParamValue = "#" + strParamValue;

                var strParam = strParamValue;
                var strParamField = grdParam.ParmeterField;

                if (strParamValue.toLowerCase() != strParamField.toLowerCase())
                    strParam = strParamField + '=' + strParamValue;

                if (strParameters == "")
                    strParameters = strParam;
                else
                    strParameters += ';' + strParam;
            }
        }
    }
    return strParameters;
};

//#endregion


//#region Load Details (Forms and HTX)

function LoadDetails(formModel, objLoadDetails, ablnAddInUndoRedo, $rootScope, isFormLink) {
    var dummyLstLoadDetails = [];

    //Get the boolean flag for whether any of the new or update method is set at form level. This is as per discussion with Fwk team.
    var loadMethodSet = false;
    for (var frmIndex = 0, frmElementCount = formModel.Elements.length; frmIndex < frmElementCount; frmIndex++) {
        if (formModel.Elements[frmIndex].Name === "initialload") {
            for (var initialLoadIndex = 0, initialLoadElementCount = formModel.Elements[frmIndex].Elements.length; initialLoadIndex < initialLoadElementCount; initialLoadIndex++) {
                if (formModel.Elements[frmIndex].Elements[initialLoadIndex].Name === "callmethods") {
                    if (formModel.Elements[frmIndex].Elements[initialLoadIndex].dictAttributes.sfwMethodName) {
                        loadMethodSet = true;
                        break;
                    }
                }
            }
            break;
        }
    }

    for (var i = 0; i < formModel.Elements.length; i++) {
        if (formModel.Elements[i].Name == "loaddetails") {
            objLoadDetails = formModel.Elements[i];

            break;
        }
    }
    if (ablnAddInUndoRedo) {
        $rootScope.UndRedoBulkOp("Start");
    }
    var lstTable = undefined;
    if (isFormLink) {
        lstTable = formModel.Elements.filter(function (itm) { return itm.Name == "items"; });
    }
    else if (!isFormLink) {
        lstTable = formModel.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
    }
    if (lstTable && lstTable.length > 0) {
        if (!objLoadDetails) {
            objLoadDetails = {
                dictAttributes: {}, Children: [], Elements: [], Name: "loaddetails", Value: ""
            };
            if (ablnAddInUndoRedo) {
                $rootScope.PushItem(objLoadDetails, formModel.Elements);
            }
            else {
                formModel.Elements.push(objLoadDetails);
            }
        }
        UpdateLoadDetailsNodes(lstTable[0], dummyLstLoadDetails, $rootScope, ablnAddInUndoRedo, loadMethodSet);

        angular.forEach(objLoadDetails.Elements, function (itm) {

            var lstitems = dummyLstLoadDetails.filter(function (x) { return x.dictAttributes.ID == itm.dictAttributes.ID; });
            if (lstitems && lstitems.length > 0) {
                if (!lstitems[0].isLoadWhenRequired) {

                    if (ablnAddInUndoRedo) {
                        $rootScope.EditPropertyValue(lstitems[0].dictAttributes.sfwUpdateAutoLoad, lstitems[0].dictAttributes, "sfwUpdateAutoLoad", itm.dictAttributes.sfwUpdateAutoLoad);
                        $rootScope.EditPropertyValue(lstitems[0].dictAttributes.sfwNewAutoLoad, lstitems[0].dictAttributes, "sfwNewAutoLoad", itm.dictAttributes.sfwNewAutoLoad);
                    }

                    else {
                        lstitems[0].dictAttributes.sfwUpdateAutoLoad = itm.dictAttributes.sfwUpdateAutoLoad;
                        lstitems[0].dictAttributes.sfwNewAutoLoad = itm.dictAttributes.sfwNewAutoLoad;
                    }
                }
            }
        });
        if (ablnAddInUndoRedo) {
            $rootScope.EditPropertyValue(objLoadDetails.Elements, objLoadDetails, "Elements", []);
        }
        else {
            objLoadDetails.Elements = [];
        }

        for (var j = 0; j < dummyLstLoadDetails.length; j++) {
            if (ablnAddInUndoRedo) {
                $rootScope.PushItem(dummyLstLoadDetails[j], objLoadDetails.Elements);
            }
            else {
                objLoadDetails.Elements.push(dummyLstLoadDetails[j]);
            }
        }
    }
    if (ablnAddInUndoRedo) {
        $rootScope.UndRedoBulkOp("End");
    }

    return dummyLstLoadDetails;
};

function UpdateLoadDetailsNodes(aobjNode, aobjdummyLstLoadDetails, $rootScope, ablnAddInUndoRedo, loadMethodSet) {
    var strEntityField = aobjNode.dictAttributes.sfwEntityField;
    if ((aobjNode.Name == "sfwGridView" && !aobjNode.dictAttributes.sfwXMLLoadMethod) || aobjNode.Name == "sfwChart" || aobjNode.Name == "sfwListView" || aobjNode.Name == "sfwCheckBoxList" || aobjNode.Name == "sfwCalendar" || aobjNode.Name == "sfwScheduler" || aobjNode.Name == "udc") {
        if (strEntityField) {
            var aobjEntityField = aobjdummyLstLoadDetails.filter(function (itm) { return itm.Name == "entityfield" && itm.dictAttributes.ID == strEntityField; });
            if (aobjEntityField.length == 0) {

                var objEntityFields = {
                    Name: "entityfield", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: []
                };


                objEntityFields.dictAttributes.ID = strEntityField;
                objEntityFields.isLoadWhenRequired = false;
                if (aobjNode.dictAttributes.sfwLoadWhenRequired && aobjNode.dictAttributes.sfwLoadWhenRequired == 'True') {
                    objEntityFields.dictAttributes.sfwUpdateAutoLoad = 'False';
                    objEntityFields.dictAttributes.sfwNewAutoLoad = 'False';
                    objEntityFields.isLoadWhenRequired = true;
                }
                else if (loadMethodSet) {
                    objEntityFields.dictAttributes.sfwNewAutoLoad = 'False';
                    objEntityFields.dictAttributes.sfwUpdateAutoLoad = 'False';
                }
                else {
                    objEntityFields.dictAttributes.sfwUpdateAutoLoad = 'True';
                    objEntityFields.dictAttributes.sfwNewAutoLoad = 'True';
                }
                if (ablnAddInUndoRedo) {
                    $rootScope.PushItem(objEntityFields, aobjdummyLstLoadDetails);
                }
                else {
                    aobjdummyLstLoadDetails.push(objEntityFields);
                }
            }
            else {
                if (aobjNode.dictAttributes.sfwLoadWhenRequired && aobjNode.dictAttributes.sfwLoadWhenRequired == 'True') {

                    if (ablnAddInUndoRedo) {
                        $rootScope.EditPropertyValue(aobjEntityField[0].dictAttributes.sfwUpdateAutoLoad, aobjEntityField[0].dictAttributes, "sfwUpdateAutoLoad", 'False');
                        $rootScope.EditPropertyValue(aobjEntityField[0].dictAttributes.sfwNewAutoLoad, aobjEntityField[0].dictAttributes, "sfwNewAutoLoad", 'False');
                        $rootScope.EditPropertyValue(aobjEntityField[0].isLoadWhenRequired, aobjEntityField[0], "isLoadWhenRequired", true);
                    }
                    else {
                        aobjEntityField[0].dictAttributes.sfwUpdateAutoLoad = 'False';
                        aobjEntityField[0].dictAttributes.sfwNewAutoLoad = 'False';
                        aobjEntityField[0].isLoadWhenRequired = true;
                    }
                }
            }
        }
    }
    else {
        if (strEntityField) {
            if (strEntityField.contains(".")) {
                strEntityField = strEntityField.substring(0, strEntityField.lastIndexOf('.'));
                var aobjEntityField = aobjdummyLstLoadDetails.filter(function (itm) { return itm.Name == "entityfield" && itm.dictAttributes.ID == strEntityField; });
                if (aobjEntityField.length == 0) {

                    var objEntityFields = {
                        Name: "entityfield", value: '', prefix: "", dictAttributes: {}, Elements: [], Children: []
                    };


                    objEntityFields.dictAttributes.ID = strEntityField;
                    if (loadMethodSet) {
                        objEntityFields.dictAttributes.sfwNewAutoLoad = "False";
                        objEntityFields.dictAttributes.sfwUpdateAutoLoad = "False";
                    }
                    else {
                        objEntityFields.dictAttributes.sfwUpdateAutoLoad = "True";
                        objEntityFields.dictAttributes.sfwNewAutoLoad = "True";
                    }

                    if (ablnAddInUndoRedo) {
                        $rootScope.PushItem(objEntityFields, aobjdummyLstLoadDetails);
                    }
                    else {
                        aobjdummyLstLoadDetails.push(objEntityFields);
                    }
                }
            }
        }
        angular.forEach(aobjNode.Elements, function (itm) {
            UpdateLoadDetailsNodes(itm, aobjdummyLstLoadDetails, $rootScope, ablnAddInUndoRedo, loadMethodSet);
        });


    }

    /* added this condition if "ListView" has Grid then we have to show grid view collection also in load details. */
    /* Bug 9971:In forms-if user add Grid inside Repeater control , on Load Details pop- up, collection is not displayed */

    if (aobjNode.Name == "sfwListView") {
        angular.forEach(aobjNode.Elements, function (itm) {
            UpdateLoadDetailsNodes(itm, aobjdummyLstLoadDetails, $rootScope, ablnAddInUndoRedo, loadMethodSet);
        });
    }
};

//#endregion