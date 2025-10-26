
//#region Drop for EntityField for Data Fields

function onEntityFieldDropForDataFields(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
        var dataType = scp.$parent.objCorrespondenceDetails.SelecetdField.DataType;
        var strDataFormat = null;

        while (field.objParent && !field.objParent.IsMainEntity) {
            field = field.objParent;
            value = [field.ID, value].join(".");
        }
        if (dataType && dataType.toLowerCase() == "datetime") {
            strDataFormat = "{0:d}";
        }
        else if (dataType && dataType.toLowerCase() == "decimal" && (field.ID.toLowerCase().indexOf("_amt") > -1 || field.ID.toLowerCase().indexOf("_amount") > -1)) {
            strDataFormat = "{0:C}";
        }
        else if (dataType && dataType.toLowerCase().contains("ssn")) {
            strDataFormat = "{0:000-##-####}";
        }
        else if (field.ID.toLowerCase().contains("phone") || field.ID.toLowerCase().contains("fax")) {
            strDataFormat = "{0:(###)###-####}";
        }
        scp.$parent.$apply(function () {
            scp.$parent.selectedFieldBookMark.dictAttributes.sfwEntityField = value;
            if (dataType) {
                scp.$parent.selectedFieldBookMark.dictAttributes.sfwDataType = dataType.toLowerCase();
            }
            if (strDataFormat) {
                scp.$parent.selectedFieldBookMark.dictAttributes.sfwDataFormat = strDataFormat;
            }
        });
    }
}
//#endregion

//#region Drop for EntityField for Table

function onEntityFieldDropForTable(e) {
    var scp = angular.element($(e.target)).scope();
    if (scp && scp.objCorrespondenceDetails.SelecetdField) {
        var field = scp.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Collection') {
            var value = scp.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Collection') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$apply(function () {
                scp.selectedTableBookMark.dictAttributes.sfwEntityField = value;
            });
        }
        else {
            alert('Only attribute of type Collection/List can be dropped.');
        }
    }
    lstEntityTreeFieldData = null;
}
//#endregion

//#region drop for Field in Table Fields

function onEntityFieldDropForTableFields(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp && scp.objCorrespondenceDetails.SelecetdField) {
        var field = scp.objCorrespondenceDetails.SelecetdField;
        var strDataFormat = "";
        if (scp.selectedTableBookMark.dictAttributes.sfwEntityField) {
            var value = scp.objCorrespondenceDetails.SelecetdField.ID;
            var DisplayedEntity = getDisplayedEntity(scp.objCorrespondenceDetails.LstDisplayedEntities);
            var itempath = field.ID;
            if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                itempath = DisplayedEntity.strDisplayName + "." + field.ID;
            }
            var entityFullPath = itempath; //GetItemPathForEntityObject(field);
            if (field && entityFullPath && entityFullPath.contains(scp.selectedTableBookMark.dictAttributes.sfwEntityField)) {

                if (field.DataType && field.DataType.toLowerCase() == "datetime") {
                    strDataFormat = "{0:d}";
                }
                else if ((field.DataType && field.DataType.toLowerCase() == "decimal") && (field.Value && (field.Value.toLowerCase().indexOf("_amt") > -1 || field.Value.toLowerCase().indexOf("_amount") > -1))) {
                    strDataFormat = "{0:C}";
                }
                else if (field.Value && field.Value.toLowerCase().contains("ssn")) {
                    strDataFormat = "{0:000-##-####}";
                }
                else if (field.Value && (field.Value.toLowerCase().contains("phone") || field.Value.toLowerCase().contains("fax"))) {
                    strDataFormat = "{0:(###)###-####}";
                }

                if (strDataFormat != "") {
                    strDataFormat = strDataFormat;
                }
                scp.$apply(function () {
                    scp.objTbl.dictAttributes.sfwDataField = entityFullPath.substring(scp.selectedTableBookMark.dictAttributes.sfwEntityField.length + 1, entityFullPath.length);
                    scp.objTbl.dictAttributes.sfwDataFormat = strDataFormat;


                });

            }
            else {
                alert('The Field Name should belong to the collection object associated with the Table.');
            }
        }
    }
    lstEntityTreeFieldData = null;
}
//#endregion


//#region drop for Field in Template Object Fields
function onEntityFieldDropForTemplateObjectBased(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Object') {
            var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Object') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$parent.$apply(function () {
                scp.$parent.selectedTemplateBookMark.dictAttributes.sfwEntityField = value;


            });

            scp.$parent.getTemplateList();
        }
        else {
            alert("Field Type has to be 'Object'");
        }
    }
}
//#endregion



//#region  drop for Field in Template Collection Fields
function onEntityFieldDropForTemplateForCollection(e) {

    var scp = angular.element($(e.target)).scope();
    if (scp && scp.$parent && scp.$parent.objCorrespondenceDetails.SelecetdField) {
        var field = scp.$parent.objCorrespondenceDetails.SelecetdField;
        if (field.Type == 'Collection') {
            var value = scp.$parent.objCorrespondenceDetails.SelecetdField.ID;
            while (field.objParent && !field.objParent.IsMainEntity && field.objParent.Type == 'Collection') {
                field = field.objParent;
                value = [field.ID, value].join(".");
            }

            scp.$parent.$apply(function () {
                scp.$parent.selectedTemplateBookMark.dictAttributes.sfwEntityField = value;

            });
            scp.$parent.getTemplateList();
        }
        else {
            alert("Field Type has to be 'Collection' or 'List'");
        }
    }
}
//#endregion

//#region on Add Item 

GetMatchTemplate = function (objChildTemplate, objField) {
    var matchTemplates = [];
    var Parent = objField.ParentVM;
    while (Parent && Parent.Name !== "sfwCorrespondenceTestField" && Parent.Name !== "sfwCorrespondenceTest") {
        Parent = Parent.ParentVM;
    }

    if (Parent) {
        matchTemplates = objChildTemplate.Elements.filter(function (x) { return x.dictAttributes.ID == Parent.dictAttributes.ID });
    }

    return matchTemplates;
}
OpenPopup = function (objField, currentScope, mainmodel) {
    //if BookmarkType is Table then new BaseDialog is created
    if (objField.dictAttributes.sfwBookmarkType == "Table" || objField.dictAttributes.sfwBookmarkType == "sfwRepeater") {
        var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
        if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
            var matchedTemplate = GetMatchTemplate(objChildTemplateBookmarks[0], objField);
            if (!matchedTemplate || (matchedTemplate && matchedTemplate.length == 0)) {
                var parentVM = objField.ParentVM;
                while (parentVM != null) {
                    if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                        var templateBookamrks = getDescendents(objChildTemplateBookmarks);
                        //if template id is there in ChildTemplateBookmarks then only break;
                        if (templateBookamrks && templateBookamrks.length > 0) {
                            var templateBookamrk = templateBookamrks[0].filter(function (x) { return x.dictAttributes.ID == ParentVM.dictAttributes.ID });
                            if (templateBookamrk && templateBookamrk.length > 0)
                                break;
                        }
                    }
                    parentVM = parentVM.ParentVM;
                }

                if (parentVM) {
                    var lstAllBookMarks = [];
                    var fileModel = null;
                    var tempDescendent = getDescendents(mainmodel);
                    if (tempDescendent && tempDescendent.length > 0) {
                        var collectionTemplate = tempDescendent.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });//x => x[ApplicationConstants.XMLFacade.ID] == parentVM[ApplicationConstants.XMLFacade.ID]);
                        if (collectionTemplate && collectionTemplate.length > 0) {
                            if (collectionTemplate[0].dictAttributes.sfwTemplateName) {
                                $.connection.hubCorrespondence.server.getAllBookMarks(collectionTemplate[0].dictAttributes.sfwTemplateName).done(function (data) {
                                    if (data.length == 2) {
                                        lstAllBookMarks = data[0];
                                        fileModel = data[1];
                                        if (fileModel) {
                                            if (objField.ParentVM.dictAttributes.ID) {
                                                var objDescendent = getDescendents(fileModel);
                                                if (objDescendent && objDescendent.length > 0) {
                                                    var childTemplateModel = objDescendent.filter(function (x) { return x.dictAttributes.ID == objField.ParentVM.dictAttributes.ID });//.Descendants().FirstOrDefault(x => x[ApplicationConstants.XMLFacade.ID] == this.ParentVM[ApplicationConstants.XMLFacade.ID]);

                                                    if (childTemplateModel && childTemplateModel.length > 0) {

                                                        OpenChildTemplateWindow(lstAllBookMarks, childTemplateModel[0], objField, currentScope, mainmodel);
                                                    }
                                                }
                                            }
                                            else {

                                                var parent = objField.ParentVM;
                                                var collectionTemplateID = undefined;
                                                var templateBookmark = null;
                                                while (parentVM != null) {
                                                    if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                                                        var templateBookamrks = getDescendents(objChildTemplateBookmarks);
                                                        //if template id is there in ChildTemplateBookmarks then only break;
                                                        if (templateBookamrks && templateBookamrks.length > 0) {
                                                            var templateBookamrk = templateBookamrks[0].filter(function (x) { return x.dictAttributes.ID == ParentVM.dictAttributes.ID });
                                                            if (templateBookamrk && templateBookamrk.length > 0)
                                                                break;
                                                        }

                                                    }
                                                    parentVM = parentVM.ParentVM;
                                                }

                                                if (fileModel) {
                                                    OpenChildTemplateWindow(lstAllBookMarks, fileModel, objField, currentScope, mainmodel);
                                                }
                                            }


                                        }
                                    }
                                });
                            }

                        }
                        else {
                            GetAllBookmarksAndFileModelForTable(objField, mainmodel.dictAttributes.ID, currentScope, mainmodel);

                        }
                    }
                }
                else {

                    GetAllBookmarksAndFileModelForTable(objField, mainmodel.dictAttributes.ID, currentScope, mainmodel);
                }
            }
            else {
                // if child template is Object type which has table then below code is executed
                if (matchedTemplate) {

                    GetAllBookmarksAndFileModelForTable(objField, matchedTemplate[0].dictAttributes.sfwTemplateName, currentScope, mainmodel);

                }
            }

        }

    }

    // BookmarkType is Template then new BaseDialog is created
    else if (objField.dictAttributes.sfwBookmarkType == "Template") {
        var lstAllBookMarks = [];
        var fileModel = null;
        var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
        if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
            var objtemplate = objChildTemplateBookmarks[0].Elements.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });
            if (objtemplate && objtemplate.length > 0 && objtemplate[0].dictAttributes.sfwTemplateName) {
                $.connection.hubCorrespondence.server.getAllBookMarks(objtemplate[0].dictAttributes.sfwTemplateName).done(function (data) {
                    if (data.length == 2) {
                        lstAllBookMarks = data[0];
                        fileModel = data[1];
                        if (fileModel) {
                            OpenChildTemplateWindow(lstAllBookMarks, fileModel, objField, currentScope, mainmodel);
                        }
                        else {
                            // if child template has child template again then following code is executed.
                            var parentVM = objField.ParentVM;
                            while (parentVM) {
                                if (parentVM.dictAttributes.sfwBookmarkType == "Template" && parentVM.dictAttributes.sfwDataType) {
                                    //if template id is there in ChildTemplateBookmarks then only break;
                                    var objChildTemplateBookmarks = mainmodel.Elements.filter(function (x) { return x.Name == "sfwChildTemplateBookmarks" });
                                    if (objChildTemplateBookmarks && objChildTemplateBookmarks.length > 0) {
                                        var templateBookamrks = getDescendents(objChildTemplateBookmarks[0]);
                                        if (templateBookamrks && templateBookamrks.length > 0) {
                                            var templateBookamrk = templateBookamrks.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });
                                            if (templateBookamrk && templateBookamrk.length > 0)
                                                break;
                                        }
                                    }

                                }
                                parentVM = parentVM.ParentVM;
                            }

                            if (parentVM) {
                                var lstAllBookMarks = [];
                                var fileModel = null;
                                var tempDescendent = getDescendents(mainmodel);
                                if (tempDescendent && tempDescendent.length > 0) {
                                    var nestedTemplate = tempDescendent.filter(function (x) { return x.dictAttributes.ID == parentVM.dictAttributes.ID });// (x => x[ApplicationConstants.XMLFacade.ID] == parentVM[ApplicationConstants.XMLFacade.ID]);
                                    if (nestedTemplate && nestedTemplate.length > 0 && nestedTemplate[0].dictAttributes.sfwTemplateName) {
                                        $.connection.hubCorrespondence.server.getAllBookMarks(nestedTemplate[0].dictAttributes.sfwTemplateName).done(function (data) {

                                            if (data.length == 2) {
                                                lstAllBookMarks = data[0];
                                                fileModel = data[1];

                                                var testmodel = getDescendents(fileModel);
                                                if (testmodel && testmodel.length > 0) {
                                                    var childTemplateElementInModel = testmodel.filter(function (x) { return x.dictAttributes.ID == objtemplate[0].dictAttributes.ID });  //x => x[ApplicationConstants.XMLFacade.ID] == templateControl[ApplicationConstants.XMLFacade.ID]);

                                                    if (childTemplateElementInModel && childTemplateElementInModel.length > 0) {
                                                        var objchildTemplateElementInModel = getDescendents(childTemplateElementInModel);
                                                        if (objchildTemplateElementInModel && objchildTemplateElementInModel.length > 0) {
                                                            var childTemplateElementInModel = objchildTemplateElementInModel.filter(function (x) { return x.dictAttributes.ID == objtemplate[0].dictAttributes.ID });//x => x[ApplicationConstants.XMLFacade.ID] == templateControl[ApplicationConstants.XMLFacade.ID]);

                                                            if (childTemplateElementInModel && childTemplateElementInModel.length > 0) {
                                                                if (childTemplateElementInModel[0].dictAttributes.sfwChildTemplateType == "Entity" || childTemplateElementInModel[0].dictAttributes == "Collection") {
                                                                    var lstAllBookMarksForChild = [];
                                                                    var fileModelForChild = null;
                                                                    if (childTemplateElementInModel[0].dictAttributes.sfwTemplateName) {
                                                                        $.connection.hubCorrespondence.server.getAllBookMarks(childTemplateElementInModel[0].dictAttributes.sfwTemplateName).done(function (data) {
                                                                            var lstAllBookMarksForChild = data[0];
                                                                            var fileModelForChild = data[1];
                                                                            if (fileModelForChild) {

                                                                                OpenChildTemplateWindow(lstAllBookMarksForChild, fileModelForChild, childTemplateElementInModel[0], currentScope, mainmodel);
                                                                            }
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        });
                                    }
                                }

                            }
                        }
                    }
                });
            }


        };
    }
}
//#endregion

//#region Get Bookmarks and Model for Collection
GetAllBookmarksAndFileModelForTable = function (objField, templateName, currentScope, mainmodel) {
    var newScope = currentScope.$new(true);
    newScope.lstData = [];
    var lstAllBookMarks = [];
    var fileModel = null;
    newScope.testfieldmodel = objField;
    newScope.correspondencemainmodel = mainmodel;
    if (templateName) {
        $.connection.hubCorrespondence.server.getAllBookMarks(templateName).done(function (data) {

            if (data.length == 2) {
                lstAllBookMarks = data[0];
                fileModel = data[1];
            }

            if (objField.dictAttributes.sfwBookmarkType && objField.dictAttributes.sfwBookmarkType.indexOf("Template") > -1) {
                newScope.$evalAsync(function () {

                    newScope.lstData = CreateColumnsForChildTemplate(lstAllBookMarks);
                });
            }
            else {
                //for collection
                //if table belongs to Child Template
                if (fileModel) {
                    var objDescendent = getDescendents(fileModel);
                    if (objDescendent && objDescendent.length > 0) {
                        var table = objDescendent.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });// fileModel.Descendants().FirstOrDefault(x => x[ApplicationConstants.XMLFacade.ID] == FieldVM[ApplicationConstants.XMLFacade.ID]);
                        if (table && table.length > 0) {
                            angular.forEach(table[0].Elements, function (item) {
                                if (table[0].Name == "sfwRepeater") {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["ID"]);
                                    });
                                }
                                else {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["sfwColumnHeader"]);
                                    });
                                }
                            });

                        }
                    }

                }
                else {

                    var objTableBookMarks = mainmodel.Elements.filter(function (x) { x.Name == "sfwTableBookMarks" });
                    if (objTableBookMarks && objTableBookMarks.length > 0) {
                        var table = objTableBookMarks[0].Elements.filter(function (x) { return x.dictAttributes.ID == objField.dictAttributes.ID });//=> x["ID"] == FieldVM["ID"]);
                        if (table && table.length > 0) {
                            angular.forEach(table[0].Elements, function (item) {
                                if (table[0].Name == "sfwRepeater") {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["ID"]);
                                    });
                                }
                                else {
                                    newScope.$evalAsync(function () {
                                        newScope.lstData.push(item.dictAttributes["sfwColumnHeader"]);
                                    });
                                }
                            });
                        }
                    }

                }
            }
        });

        newScope.openAddItemToListForCollection = currentScope.$root.showDialog(newScope, "Add Items", "Correspondence/views/AddItemToListForCollection.html");

        newScope.AddCollectionRecord = function () {
            var count = 0;
            var tempModel = undefined;
            var fields = {
                Name: "sfwCorrespondenceTestFields", Value: '', dictAttributes: {}, Elements: []
            };
            if (newScope.model) {
                count = newScope.model.Elements.length;
                tempModel = newScope.model;
            }
            else if (newScope.testfieldmodel) {
                count = newScope.testfieldmodel.Elements.length;
                tempModel = newScope.testfieldmodel;
            }
            fields.dictAttributes.indexNumber = count;
            fields.ParentVM = newScope.testfieldmodel;

            currentScope.$root.PushItem(fields, newScope.testfieldmodel.Elements);

            if (newScope.lstData && newScope.lstData.length > 0) {
                angular.forEach(newScope.lstData, function (item) {

                    var field = {
                        Name: "sfwCorrespondenceTestField", Value: '', dictAttributes: {}, Elements: []
                    };
                    field.ParentVM = fields;
                    field.dictAttributes.ID = item;
                    if (lstAllBookMarks && lstAllBookMarks.length > 0) {
                        if (tempModel && tempModel.dictAttributes.sfwBookmarkType != "Table") {
                            var corBasicBkmrk = lstAllBookMarks.filter(function (x) { return x.Name == item });
                            if (corBasicBkmrk && corBasicBkmrk.length > 0) {
                                if (corBasicBkmrk.Type == BookmarkType.QueryUser ||
                                    corBasicBkmrk.Type == BookmarkType.QueryConditionalBlock) {

                                }
                                SetTemplateBookmarkTypeInModel(corBasicBkmrk, field, tempModel);
                            }
                        }
                    }

                    currentScope.$root.PushItem(field, fields.Elements);
                });
                newScope.selectedCollectionRow = newScope.testfieldmodel.Elements[newScope.testfieldmodel.Elements.length - 1];
            }
        }

        newScope.SelectedCollectionField = function (obj) {
            newScope.selectedCollectionRow = obj;
        }

        newScope.DeleteCollectionRecord = function () {
            var index = newScope.testfieldmodel.Elements.indexOf(newScope.selectedCollectionRow);
            if (index > -1) {
                currentScope.$root.DeleteItem(newScope.selectedCollectionRow, newScope.testfieldmodel.Elements, "");
                if (index < newScope.testfieldmodel.Elements.length) {
                    newScope.SelectedCollectionField(newScope.testfieldmodel.Elements[index]);
                }
                else if (newScope.testfieldmodel.Elements.length > 0) {
                    newScope.SelectedCollectionField(newScope.testfieldmodel.Elements[index - 1]);
                }
                else {
                    newScope.SelectedCollectionField(newScope.testfieldmodel);
                }

            }
            if (newScope.testfieldmodel.Elements.length == 0) {
                newScope.selectedCollectionRow = undefined;
            }
        }

    }
}


SetTemplateBookmarkTypeInModel = function (item, field, fileModel) {
    var objDescendent = getDescendents(fileModel);
    if (objDescendent && objDescendent.length > 0) {
        var fieldModel = objDescendent.filter(function (x) { return x.dictAttributes.ID == field.dictAttributes.ID });//(x => x[ApplicationConstants.XMLFacade.ID] == field[ApplicationConstants.XMLFacade.ID]);
        if (fieldModel && fieldModel.length > 0) {
            field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwDataType;
            switch (item[0].Type) {
                case BookmarkType.ConditionalBlock:
                case BookmarkType.ElseBlock:
                case BookmarkType.EndBlock:
                case BookmarkType.Simple:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
                case BookmarkType.NormalTable:
                case BookmarkType.StaticTable:
                    field.dictAttributes.sfwBookmarkType = "Table";
                    break;
                case BookmarkType.Template:
                    field.dictAttributes.sfwBookmarkType = "Template";
                    field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwChildTemplateType;
                    break;
                case BookmarkType.RuleConditionalBlock:
                    field.dictAttributes.sfwBookmarkType = "Rule";

                    break;
                default:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
            }
        }
    }
}
CreateColumnsForChildTemplate = function (bookmarks) {
    var lstData = [];
    if (bookmarks && bookmarks.length > 0) {
        //find all bookmarks and add it as column header
        angular.forEach(bookmarks, function (item) {
            lstData.push(item.Name);
        });

    }
    return lstData;
}

//#endregion

//#region open child window for Template Bookmark(for Entity & Collection Datatype)
OpenChildTemplateWindow = function (allBookmarks, fileModel, objField, currentScope, mainmodel) {
    //if DataType is Object
    if (objField.dictAttributes.sfwDataType == "Entity") {

        GetAllBookmarksAndFileModelForEntity(objField, fileModel.dictAttributes.ID, currentScope, mainmodel);

    }
    //if DataType is Collection
    else if (objField.dictAttributes.sfwDataType == "Collection") {
        GetAllBookmarksAndFileModelForTable(objField, fileModel.dictAttributes.ID, currentScope, mainmodel);

    }
    //}

}
//#endregion

//#region Get All Bookmarks And FileModel For Entity DataType
GetAllBookmarksAndFileModelForEntity = function (objField, templateName, currentScope, mainmodel) {

    var newScope = currentScope.$new();
    newScope.lstData = [];
    var lstAllBookMarks = [];
    var fileModel = null;
    if (templateName) {
        $.connection.hubCorrespondence.server.getAllBookMarks(templateName).done(function (data) {

            if (data.length == 2) {
                lstAllBookMarks = data[0];
                fileModel = data[1];
            }

            if (objField.Elements.length > 0) {

                //removing old bookmarks which is not present in new bookmarks list
                for (var j = objField.Elements.length - 1; j >= 0; j--) {
                    var bookmarkFound = lstAllBookMarks.filter(function (x) { return x.Name == objField.Elements[j].dictAttributes.ID });
                    if (!bookmarkFound || (bookmarkFound && bookmarkFound.length == 0)) {
                        objField.Elements.splice(j, 1);

                    };
                }


                angular.forEach(lstAllBookMarks, function (item) {
                    var bookmarkFound = objField.Elements.filter(function (x) { return x.dictAttributes.ID == item.Name }); //x => x[ApplicationConstants.XMLFacade.ID] == item.Name);
                    if (!bookmarkFound || (bookmarkFound && bookmarkFound.length == 0)) {
                        AddTemplateBookmarkInModel(item, fileModel, objField, currentScope);
                    }

                });

            }
            else {
                angular.forEach(lstAllBookMarks, function (item) {
                    AddTemplateBookmarkInModel(item, fileModel, objField, currentScope);
                });
            }
            newScope.testfieldmodel = objField;
            newScope.correspondencemainmodel = mainmodel;
            newScope.openAddItemToListForEntity = currentScope.$root.showDialog(newScope, "Add Items", "Correspondence/views/AddItemToListForEntity.html");
            newScope.OnItemClick = function (obj) {
                newScope.selectedItem = obj;
            }

            newScope.$evalAsync(function () {
                newScope.Elements = objField.Elements;
            });
            newScope.OpenPopup = function (objField) {
                OpenPopup(objField, newScope, fileModel);
            }

        });

    }

}

AddTemplateBookmarkInModel = function (item, fileModel, objField, currentScope) {
    if (item.Type == BookmarkType.QueryUser ||
        item.Type == BookmarkType.QueryConditionalBlock) {
        return;
    }

    var field = {
        Name: "sfwCorrespondenceTestField", Value: '', dictAttributes: {}, Elements: []
    };


    field.dictAttributes.ID = item.Name;
    var objDescendent = getDescendents(fileModel);
    if (objDescendent && objDescendent.length > 0) {

        var fieldModel = objDescendent.filter(function (x) { return x.dictAttributes.ID && x.dictAttributes.ID.toLowerCase() == item.Name.toLowerCase() });
        if (fieldModel && fieldModel.length > 0) {
            field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwDataType;
            switch (item.Type) {
                case BookmarkType.ConditionalBlock:
                case BookmarkType.ElseBlock:
                case BookmarkType.EndBlock:
                case BookmarkType.Simple:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
                case BookmarkType.NormalTable:
                case BookmarkType.StaticTable:
                    field.dictAttributes.sfwBookmarkType = "Table";
                    break;
                case BookmarkType.Template:
                    field.dictAttributes.sfwBookmarkType = "Template";
                    field.dictAttributes.sfwDataType = fieldModel[0].dictAttributes.sfwChildTemplateType;
                    break;
                case BookmarkType.RuleConditionalBlock:
                    field.dictAttributes.sfwBookmarkType = "Rule";

                    break;
                default:
                    field.dictAttributes.sfwBookmarkType = "Field";
                    break;
            }
        }
    }
    currentScope.$root.PushItem(field, objField.Elements);
}


//#endregion


//#region enum Bookmarks Types
var BookmarkType =
    {
        Undefined: 0,
        Simple: 1,
        Constant: 2,
        ConditionalBlock: 3,
        QueryConditionalBlock: 4,
        EndBlock: 5,
        Barcode: 6,
        QueryUser: 7,
        NormalTable: 8,
        StaticTable: 9,
        InsertFile: 10,
        ElseBlock: 11,
        Template: 12,
        RuleConditionalBlock: 13

    };

//#endregion
app.directive('correspondencetabletemplate', ['$compile', "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", "$NavigateToFileService", "$rootScope", "$GetEntityFieldObjectService", function ($compile, $EntityIntellisenseFactory, $ValidationService, CONST, $NavigateToFileService, $rootScope, $GetEntityFieldObjectService) {
    var getTemplate = function (content1) {

        var template = '';
        template += '<div class="panel" ng-click="loadEntityTree(true, $event)">',
            template += '<div class="panel-heading"  ng-click="items.IsExpanded=!items.IsExpanded">',
            template += '<span ng-bind="items.dictAttributes.ID"></span>',
            template += '<div class="correspondence-panel-arrow">',
            template += '<i ng-class="!items.IsExpanded?\'fa fa-chevron-down\':\'fa fa-chevron-up\'">',
            template += '</i>',
            template += '</div>',
            template += '</div>',
            template += '<div class="panel-body" ng-if="items.IsExpanded">',
            template += '<div class="form-group no-bottom-margin">',
            template += '<label class="control-label corr-tables-caption" ng-if="!items.dictAttributes.sfwEntityField.trim()">Entity Field</label><label class="control-label corr-tables-caption" ng-if="items.dictAttributes.sfwEntityField.trim()"><a ng-click="NavigateToEntityField(items.dictAttributes.sfwEntityField,objcorrespondence.dictAttributes.sfwEntity)">Entity Field</a></label>',
            template += '<entityfieldelementintellisense onchangecallback="clearEntityFieldforTable()" modebinding="items.dictAttributes.sfwEntityField"  class="corr-tables-input" model="items" onblurcallback="setEntityIDForSelectedTable(items)" isshowonetoone=true isshowonetomany=true entityid="objcorrespondence.dictAttributes.sfwEntity" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="items.errors.sfwEntityField" validate="true"/>',
            template += '</div>',
            template += '<div class="contact-table-manage">',
            template += '<table class="sws-table sws-bordered-table auto-width" ng-click="loadEntityTree(false, $event)">',
            template += '<thead>',
            template += '<tr>',
            template += '<th></th>',
            template += '<th ng-repeat="objTbl in items.Elements"  ng-class="{\'selected\':objTbl==selectedSfwColumn, \'bckgGrey\' : objTbl.isAdvanceSearched, \'bckgGreen\' : (objTbl.isAdvanceSearched && objTbl==selectedSfwColumn)}" >{{objTbl.dictAttributes.sfwColumnHeader}}</th>',
            template += '</tr>',
            template += '</thead>',
            template += '<tbody>',
            template += '<tr>',
            template += '<td class="sws-bordered-table-left-td">Field Name</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            template += '<entityfieldelementintellisense setcolumndatatype="true" candrop="true" modebinding="objTbl.dictAttributes.sfwEntityField" model="objTbl" entityid="SelectedTableEntityID" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="objTbl.errors.sfwEntityField" validate="true" isempty="true"/>',
            template += '</td>',
            template += '</tr>',
            template += '<tr >',
            template += '<td class="sws-bordered-table-left-td">Data Format</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            //  template += '<select class="form-control form-filter input-sm" ng-options="x for x in dataformats" title="{{objTbl.dictAttributes.sfwDataFormat}}" ng-model="objTbl.dictAttributes.sfwDataFormat"></select>',
            template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            template += '</td>',
            template += '</tr>',
            template += '<tr>',
            template += '<td class="sws-bordered-table-left-td">Data Type</td>',
            template += '<td ng-repeat="objTbl in items.Elements">',
            template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwColumnDataType}}" ng-model="objTbl.dictAttributes.sfwColumnDataType"></select>',
            template += '</td>',
            template += '</tr>',
            template += '</tbody>',
            template += '</table>';
        template += '</div>',
            template += '</div>',
            template += '</div>';
        return template;
    };

    return {
        restrict: "E",
        replace: true,
        scope: {
            items: '=',
            objcorrespondence: '=',
            entitytreename: '=entityTreeName',
            changeCallback: '&',
            selectedSfwColumn: '='
        },
        link: function (scope, element, attrs) {


            scope.datatypes = CONST.CORRESPONDENCEDATATYPES;
            scope.dataformats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];

            scope.loadEntityTree = function (isParent, e) {
                if (!isParent) {
                    if (scope.items.dictAttributes.sfwEntityField && scope.objcorrespondence.dictAttributes.sfwEntity) {
                        var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.objcorrespondence.dictAttributes.sfwEntity, scope.items.dictAttributes.sfwEntityField);
                        if (objAttribute && (objAttribute.DataType == 'Collection' || objAttribute.DataType == 'List')) {
                            if (scope.changeCallback) {
                                scope.changeCallback({ entity: objAttribute.Entity });
                            }
                        }
                    }
                }
                else {
                    if (scope.changeCallback) {
                        scope.changeCallback();
                    }
                }
                if (e) {
                    e.stopPropagation();
                }
            };

            scope.clearEntityFieldforTable = function () {
                scope.items.Elements.forEach(function (objTL) {
                    objTL.dictAttributes.sfwEntityField = "";
                    $ValidationService.checkValidListValue([], objTL, objTL.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                    //  $ValidationService.checkValidListValueForMultilevel([], objTL, objTL.dictAttributes.sfwEntityField, scope.items.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                });
            };

            scope.setEntityIDForSelectedTable = function (selectedTableBookMark) {
                scope.SelectedTableEntityID = undefined;
                entityField = selectedTableBookMark.dictAttributes.sfwEntityField;
                var EntityID = scope.objcorrespondence.dictAttributes.sfwEntity;
                if (entityField && EntityID) {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(EntityID, entityField);
                    if (objField) {
                        scope.SelectedTableEntityID = objField.Entity;
                    }
                }
            };

            if (scope.items) {
                element.html(getTemplate(scope.items));
                $compile(element.contents())(scope);
                scope.setEntityIDForSelectedTable(scope.items);
            }
            scope.NavigateToEntityField = function (astrEntityField, astrEntityName) {
                if (astrEntityField && astrEntityName) {
                    var arrText = astrEntityField.split('.');
                    var data = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = "";

                    if (astrEntityName) {
                        parententityName = astrEntityName;
                    }
                    while (parententityName) {
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            var attributes = entity[0].Attributes;
                            data = data.concat(attributes);
                            parententityName = entity[0].ParentId;
                        }
                        else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
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
                                            data = data.concat(attributes);
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
                                if (astrEntityName) {
                                    EntityName = astrEntityName;
                                }
                                if (EntityName) {
                                    $rootScope.IsLoading = true;
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
            };
        }
    };
}]);

app.directive('correspondencerepeatercontroltemplate', ['$compile', "$EntityIntellisenseFactory", "$ValidationService", "CONSTANTS", "$NavigateToFileService", "$rootScope", "$GetEntityFieldObjectService", function ($compile, $EntityIntellisenseFactory, $ValidationService, CONST, $NavigateToFileService, $rootScope, $GetEntityFieldObjectService) {
    var getTemplate = function (content1) {

        var template = '';
        template += '<div class="panel" ng-click="loadEntityTree(true, $event)">',
            template += '<div class="panel-heading"  ng-click="items.IsExpanded=!items.IsExpanded">',
            template += '<span ng-bind="items.dictAttributes.ID"></span>',
            template += '<div class="correspondence-panel-arrow">',
            template += '<i ng-class="!items.IsExpanded?\'fa fa-chevron-down\':\'fa fa-chevron-up\'">',
            template += '</i>',
            template += '</div>',
            template += '</div>',
            template += '<div class="panel-body" ng-if="items.IsExpanded">',
            template += '<Grid>',
            template += '    <Grid.ColumnDefinitions>',
            template += '        <ColumnDefinition Width="auto" />',
            template += '        <ColumnDefinition Width="*" />',
            template += '        <ColumnDefinition Width="auto" />',
            template += '        <ColumnDefinition Width="*" />',
            template += '  </Grid.ColumnDefinitions>',
            template += '    <Grid.RowDefinitions>',
            template += '        <RowDefinition Height="*" />',
            template += '</Grid.RowDefinitions>',
            template += '<label Grid.Column="0" class="control-label " style="vertical-align:10px" ng-if="!items.dictAttributes.sfwEntityField.trim()">Entity Field</label><label class="control-label corr-tables-caption" ng-if="items.dictAttributes.sfwEntityField.trim()"><a ng-click="NavigateToEntityField(items.dictAttributes.sfwEntityField,objcorrespondence.dictAttributes.sfwEntity)">Entity Field</a></label>',
            template += '<entityfieldelementintellisense Grid.Column="1" onchangecallback="clearEntityFieldforTable()" modebinding="items.dictAttributes.sfwEntityField" class="corr-tables-input" model="items" onblurcallback="setEntityIDForSelectedTable(items)" isshowonetoone=true isshowonetomany=true entityid="objcorrespondence.dictAttributes.sfwEntity" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="items.errors.sfwEntityField" validate="true" />',

            template += '<label Grid.Column="2" style="vertical-align:10px" class="control-label marg-left-10">Show First Line Header</label>',
            template += '<input class="marg-left-10" type="checkbox" style="vertical-align:10px" ng-model="items.dictAttributes.sfwFirstLineHeader" ng-checked="items.dictAttributes.sfwFirstLineHeader" undoredodirective model="items" ng-true-value="\'True\'" ng-false-value="\'False\'" />',

            template += '</Grid>',
            template += '<div class="contact-table-manage">',
            template += '<table class="sws-table sws-bordered-table auto-width" ng-click="loadEntityTree(false, $event)">',
            template += '<thead>',
            template += '<tr>',
            template += '<th>ID</th>',
            template += '<th>Data Type</th>',
            template += '<th>Data Format</th>',
            template += '<th>Entity Field</th>',
            template += '</tr>',
            template += '</thead>',
            template += '<tbody>',
            template += '<tr ng-repeat="objTbl in items.Elements">',
            template += '<td>',
            template += '<input type="text" class="form-control input-sm" ng-model="objTbl.dictAttributes.ID"  undoredodirective model="objTbl" />',
            template += '</td>',
            template += '<td>',
            template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwDataType}}" ng-model="objTbl.dictAttributes.sfwDataType"></select>',
            template += '</td>',
            template += '<td>',
            template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            template += '</td>',
            template += '<td>',
            template += '<entityfieldelementintellisense setcolumndatatype="true" candrop="true" modebinding="objTbl.dictAttributes.sfwEntityField" model="objTbl" entityid="SelectedTableEntityID" propertyname="\'dictAttributes.sfwEntityField\'" errorprop="objTbl.errors.sfwEntityField" validate="true" isempty="true"/>',
            template += '</td>',
            template += '</tr>',
            //template += '<tr >',
            //template += '<td class="sws-bordered-table-left-td">Data Format</td>',
            //template += '<td ng-repeat="objTbl in items.Elements">',
            ////  template += '<select class="form-control form-filter input-sm" ng-options="x for x in dataformats" title="{{objTbl.dictAttributes.sfwDataFormat}}" ng-model="objTbl.dictAttributes.sfwDataFormat"></select>',
            //template += '<common-intellisense collection="dataformats" selected-item="objTbl.dictAttributes.sfwDataFormat" model="objTbl" propertyname="\'dictAttributes.sfwDataFormat\'"></common-intellisense>',
            //template += '</td>',
            //template += '</tr>',
            //template += '<tr>',
            //template += '<td class="sws-bordered-table-left-td">Data Type</td>',
            //template += '<td ng-repeat="objTbl in items.Elements">',
            //template += '<select class="form-control form-filter input-sm" ng-options="datatype.CodeID as datatype.CodeValue for datatype in datatypes" title="{{objTbl.dictAttributes.sfwColumnDataType}}" ng-model="objTbl.dictAttributes.sfwColumnDataType"></select>',
            //template += '</td>',
            //template += '</tr>',
            template += '</tbody>',
            template += '</table>';
        template += '</div>',
            template += '</div>',
            template += '</div>';
        return template;
    };

    return {
        restrict: "E",
        replace: true,
        scope: {
            items: '=',
            objcorrespondence: '=',
            entitytreename: '=entityTreeName',
            changeCallback: '&',
            selectedSfwColumn: '='
        },
        link: function (scope, element, attrs) {


            scope.datatypes = CONST.CORRESPONDENCEDATATYPES;
            scope.dataformats = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];

            scope.loadEntityTree = function (isParent, e) {
                if (!isParent) {
                    if (scope.items.dictAttributes.sfwEntityField && scope.objcorrespondence.dictAttributes.sfwEntity) {
                        var objAttribute = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(scope.objcorrespondence.dictAttributes.sfwEntity, scope.items.dictAttributes.sfwEntityField);
                        if (objAttribute && (objAttribute.DataType == 'Collection' || objAttribute.DataType == 'List')) {
                            if (scope.changeCallback) {
                                scope.changeCallback({ entity: objAttribute.Entity });
                            }
                        }
                    }
                }
                else {
                    if (scope.changeCallback) {
                        scope.changeCallback();
                    }
                }
                if (e) {
                    e.stopPropagation();
                }
            };

            scope.clearEntityFieldforTable = function () {
                scope.items.Elements.forEach(function (objTL) {
                    objTL.dictAttributes.sfwEntityField = "";
                    $ValidationService.checkValidListValue([], objTL, objTL.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                    //  $ValidationService.checkValidListValueForMultilevel([], objTL, objTL.dictAttributes.sfwEntityField, scope.items.dictAttributes.sfwEntityField, "sfwEntityField", "sfwEntityField", CONST.VALIDATION.ENTITY_FIELD_INCORRECT, undefined, true);
                });
            };

            scope.setEntityIDForSelectedTable = function (selectedTableBookMark) {
                scope.SelectedTableEntityID = undefined;
                entityField = selectedTableBookMark.dictAttributes.sfwEntityField;
                var EntityID = scope.objcorrespondence.dictAttributes.sfwEntity;
                if (entityField && EntityID) {
                    var objField = $GetEntityFieldObjectService.GetEntityFieldObjectFromEntityField(EntityID, entityField);
                    if (objField) {
                        scope.SelectedTableEntityID = objField.Entity;
                    }
                }
            };

            if (scope.items) {
                element.html(getTemplate(scope.items));
                $compile(element.contents())(scope);
                scope.setEntityIDForSelectedTable(scope.items);
            }
            scope.NavigateToEntityField = function (astrEntityField, astrEntityName) {
                if (astrEntityField && astrEntityName) {
                    var arrText = astrEntityField.split('.');
                    var data = [];
                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var entities = entityIntellisenseList;
                    var parententityName = "";

                    if (astrEntityName) {
                        parententityName = astrEntityName;
                    }
                    while (parententityName) {
                        var entity = entities.filter(function (x) {
                            return x.ID == parententityName;
                        });
                        if (entity.length > 0) {
                            var attributes = entity[0].Attributes;
                            data = data.concat(attributes);
                            parententityName = entity[0].ParentId;
                        }
                        else {
                            parententityName = "";
                        }
                    }
                    if (arrText.length > 1) {
                        scope.isMultilevelActive = true;
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
                                            data = data.concat(attributes);
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
                                if (astrEntityName) {
                                    EntityName = astrEntityName;
                                }
                                if (EntityName) {
                                    $rootScope.IsLoading = true;
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
            };
        }
    };
}]);