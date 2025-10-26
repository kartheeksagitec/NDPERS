
function expandCollapseQuery(event) {
    var element = event.target;
    var tables = $(element).parents("table");
    if (tables.length > 0) {
        $(tables[0]).siblings("ul").slideToggle();
        // $(element).toggleClass("file-expanded file-collapsed");
    }
}

function extractLastValue(term, caretIndex) {
    return split(term).pop();
}

function getIntellisenseDataForMsgs(value) {
    var data = [];
    var rootScope = getCurrentFileScope();
    //Get the $rootScope object
    var scope = rootScope;
    //while (scope != null) {
    //    if (scope.$root) {
    //        scope = scope.$root;
    //        break;
    //    }
    //    else {
    //        scope = scope.$parent;
    //    }
    //}

    //Get constants object from $rootScope object.
    if (scope && scope.lstMessages) {
        data = scope.lstMessages;
        data = data.sort();
        var lstFilterData = [];
        for (var i = 0; i < data.length; i++) {
            if (data[i] && data[i].DisplayMessageID.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                lstFilterData.push(data[i]);
            }
        }
        lstTop100Msgs = [];
        if (lstFilterData.length > 100) {
            for (var i = 0; i <= 100; i++) {
                lstTop100Msgs.push(lstFilterData[i]);
            }
        }
        else {
            lstTop100Msgs = lstFilterData;
        }
    }
    return lstTop100Msgs;
}

function onActionKeyDownForlstMsgs(eargs) {
    var input = eargs.target;
    var charCode = (eargs.which) ? eargs.which : eargs.keyCode;
    if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        eargs.preventDefault();
    }
    if (eargs.char == "\b") {
        var inputvalue = eargs.target.value.slice(0, -1);
    }
    else {
        var inputvalue = eargs.target.value + eargs.char;
    }
    var data = getIntellisenseDataForMsgs($(input).val());
    controlSelection = $(input);
    if (data) {
        controlSelection.autocomplete({
            minLength: 0,
            source: function (request, response) {
                var term = request.term,
                    results = [];
                if (term != undefined) {
                    term = extractLastValue(request.term);
                    //if (term.length > 0) {
                    results = $.ui.autocomplete.filter(
                        $.map(data, function (value, key) {
                            if (typeof value.DisplayMessageID == "undefined") {
                                return {
                                    label: value,
                                    value: value
                                };
                            }
                            else {
                                return {
                                    label: value.DisplayMessageID,
                                    value: value,
                                };
                            }
                        }), extractLastValue(request.term, this.element[0].selectionStart));
                    //}
                    //else {
                    //    results = "";
                    //}
                }
                response(results);
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (ui.item.value.DisplayMessageID) {
                    control.value = ui.item.value.MessageID;
                }
                else {
                    control.value = ui.item.value;
                }
                $(this).trigger("change");
                return false;
            }
        });
    }
}

function extractLastValue(term) {
    return split(term).pop();
}

// Intellisense for Checklist(Code Groups) in Business Object
function getIntellisenseDataForChecklistID(value) {
    var data = [];
    var rootScope = getCurrentFileScope();
    //Get the $rootScope object
    var scope = rootScope;
    while (scope != null) {
        if (scope.$root) {
            scope = scope.$root;
            break;
        }
        else {
            scope = scope.$parent;
        }
    }

    //Get codeGroups object from $rootScope object.
    var lstTop100IDs = [];
    if (scope && scope.lstCodeGroups) {
        data = scope.lstCodeGroups;
        data = data.sort();
        var lstFilterData = [];
        for (var i = 0; i < data.length; i++) {
            if (data[i].CodeIDDescription.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                lstFilterData.push(data[i]);
            }
        }
        if (lstFilterData.length > 100) {
            for (var i = 0; i <= 100; i++) {
                lstTop100IDs.push(lstFilterData[i]);
            }
        }
        else {
            lstTop100IDs = lstFilterData;
        }
    }
    return lstTop100IDs;
}

function onActionKeyDownForlstCheckListID(args) {
    var input = args.target;
    var charCode = (args.which) ? args.which : args.keyCode;
    if (args.ctrlKey && args.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        args.preventDefault();
    }
    if (args.char == "\b") {
        var inputvalue = args.target.value.slice(0, -1);
    }
    else {
        var inputvalue = args.target.value + args.char;
    }
    var data = getIntellisenseDataForChecklistID($(input).val());
    controlSelection = $(input);
    if (data) {
        controlSelection.autocomplete({
            minLength: 0,
            source: function (request, response) {
                var term = request.term,
                    results = [];
                if (term != undefined) {
                    term = extractLastValue(request.term);
                    //if (term.length > 0) {
                    results = $.ui.autocomplete.filter(
                        $.map(data, function (value, key) {
                            if (typeof value.CodeIDDescription == "undefined") {
                                return {
                                    label: value,
                                    value: value
                                };
                            }
                            else {
                                return {
                                    label: value.CodeIDDescription,
                                    value: value,
                                };
                            }
                        }), extractLastValue(request.term, this.element[0].selectionStart));
                    //}
                    //else {
                    //    results = "";
                    //}
                }
                response(results);
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (ui.item.value.CodeIDDescription) {
                    control.value = ui.item.value.CodeID;
                }
                else {
                    control.value = ui.item.value;
                }
                $(this).trigger("change");
                return false;
            }
        });
    }
}



// Intellisense for Checklist(Code ID) in Validation rule checklist tab
function getIntellisenseDataForChecklistIDForRules(value) {
    var data = [];
    var rootScope = getCurrentFileScope();
    //Get the $rootScope object
    var scope = rootScope;
    while (scope != null) {
        if (scope.$root) {
            scope = scope.$root;
            break;
        }
        else {
            scope = scope.$parent;
        }
    }

    //Get codeID object from $rootScope object.
    if (scope && scope.lstCodeGroups) {
        data = scope.lstCodeValue;
        data = data.sort();
        var lstFilterData = [];
        for (var i = 0; i < data.length; i++) {
            if (data[i].CodeIDDescription.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                lstFilterData.push(data[i]);
            }
        }

    }
    return lstFilterData;
}


function onActionKeyDownForlstCheckListIDForRules(args) {
    var input = args.target;
    var charCode = (args.which) ? args.which : args.keyCode;
    if (args.ctrlKey && args.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        args.preventDefault();
    }
    if (args.char == "\b") {
        var inputvalue = args.target.value.slice(0, -1);
    }
    else {
        var inputvalue = args.target.value + args.char;
    }
    var data = getIntellisenseDataForChecklistIDForRules(inputvalue);
    controlSelection = $(input);
    if (data) {
        controlSelection.autocomplete({
            minLength: 0,
            source: function (request, response) {
                var term = request.term,
                    results = [];
                if (term != "" && term != undefined) {
                    term = extractLastValue(request.term);
                    if (term.length > 0) {
                        results = $.ui.autocomplete.filter(
                            $.map(data, function (value, key) {
                                if (typeof value.CodeIDDescription == "undefined") {
                                    return {
                                        label: value,
                                        value: value
                                    };
                                }
                                else {
                                    return {
                                        label: value.CodeIDDescription,
                                        value: value,
                                    };
                                }
                            }), extractLastValue(request.term, this.element[0].selectionStart));
                    }
                    else {
                        results = "";
                    }
                }
                response(results);
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (ui.item.value.CodeIDDescription) {
                    control.value = ui.item.value.CodeValue;
                }
                else {
                    control.value = ui.item.value;
                }
                $(this).trigger("change");
                return false;
            }
        });
    }
}



// Intellisense for Error table in BO
function getIntellisenseDataForErrortable(value) {
    var data = [];
    var rootScope = getCurrentFileScope();
    //Get the $rootScope object
    var scope = rootScope;

    //Get codeID object from $rootScope object.
    if (scope && scope.lstErrorTable) {
        data = scope.lstErrorTable;
        data = data.sort();
        var lstFilterData = [];
        for (var i = 0; i < data.length; i++) {
            if (data[i].toLowerCase().indexOf(value.toLowerCase()) > -1) {
                lstFilterData.push(data[i]);
            }
        }

    }
    return lstFilterData;
}


function onActionKeyDownForErrortables(args) {
    var input = args.target;
    var charCode = (args.which) ? args.which : args.keyCode;
    if (args.ctrlKey && args.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        args.preventDefault();
    }
    if (args.char == "\b") {
        var inputvalue = args.target.value.slice(0, -1);
    }
    else {
        var inputvalue = args.target.value + args.char;
    }
    var data = getIntellisenseDataForErrortable($(input).val());
    setSingleLevelAutoComplete($(input), data);
}

function showErrorTableData(event) {
    var inputElement;
    if (!inputElement) {
        inputElement = $(event.target).prevAll("input[type='text']");
    }
    inputElement.focus();
    var data = getIntellisenseDataForErrortable($(inputElement).val());
    if (inputElement && data) {
        setSingleLevelAutoComplete(inputElement, data);
        if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
    }
    if (event) {
        event.stopPropagation();
    }
}

// Intellisense for Error table in BO
function getIntellisenseDataForChecklisttable(value) {
    var data = [];
    var rootScope = getCurrentFileScope();
    //Get the $rootScope object
    var scope = rootScope;

    //Get codeID object from $rootScope object.
    if (scope && scope.lstErrorTable) {
        data = scope.lstChecklistTable;
        data = data.sort();
        var lstFilterData = [];
        for (var i = 0; i < data.length; i++) {
            if (data[i].toLowerCase().indexOf(value.toLowerCase()) > -1) {
                lstFilterData.push(data[i]);
            }
        }

    }
    return lstFilterData;
}


function onActionKeyDownForChecklisttables(args) {
    var input = args.target;
    var charCode = (args.which) ? args.which : args.keyCode;
    if (args.ctrlKey && args.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        args.preventDefault();
    }
    if (args.char == "\b") {
        var inputvalue = args.target.value.slice(0, -1);
    }
    else {
        var inputvalue = args.target.value + args.char;
    }
    var data = getIntellisenseDataForChecklisttable($(input).val());
    controlSelection = $(input);
    if (data) {
        controlSelection.autocomplete({
            minLength: 0,
            source: function (request, response) {
                var term = request.term,
                    results = [];
                if (term != undefined) {
                    term = extractLastValue(request.term);
                    //if (term.length > 0) {
                    results = $.ui.autocomplete.filter(
                        $.map(data, function (value, key) {
                            if (typeof value == "undefined") {
                                return {
                                    label: value,
                                    value: value
                                };
                            }
                            else {
                                return {
                                    label: value,
                                    value: value,
                                };
                            }
                        }), extractLastValue(request.term, this.element[0].selectionStart));
                    //}
                    //else {
                    //    results = "";
                    //}
                }
                response(results);
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (ui.item.value) {
                    control.value = ui.item.value;
                }
                else {
                    control.value = ui.item.value;
                }
                $(this).trigger("change");
                return false;
            }
        });
    }
}

function showCheckListTableData(event) {
    var inputElement;
    if (!inputElement) {
        inputElement = $(event.target).prevAll("input[type='text']");
    }
    inputElement.focus();
    var data = getIntellisenseDataForChecklisttable($(inputElement).val());
    if (inputElement && data) {
        setSingleLevelAutoComplete(inputElement, data);
        if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
    }
    if (event) {
        event.stopPropagation();
    }
}

function canDropRuleinChecklist(dragdata, dropdata) {

    var retValue = true;
    if (dragdata == undefined || dragdata == null || dropdata == null || dropdata == undefined) {
        retValue = false;
    }
    return retValue;
}

function onBusMethodDrop(e) {
    var scp = $(e.currentTarget).scope();
    // changed this because while adding new xml methods it is not added in xmlmethods list-by sai
    //var selectedXmlMethods = scp.objMethods.Elements.filter(function (x) {
    //    return x.selected;
    //});
    var selectedXmlMethods = scp.obj;
    if (selectedXmlMethods) {
        var id = dragDropData;
        if (id && id.trim().length > 0) {
            if (id.indexOf(".") > 0) {
                id = dragDropData.substring(dragDropData.indexOf(".") + 1);
            }
            var method = null;
            if (id.indexOf(".") == -1 && scp.ObjTree.Rules) {
                var rules = scp.ObjTree.Rules.filter(function (x) {
                    return x.ID == id;
                });
                if (rules && rules.length > 0) {
                    method = rules[0];
                }
            }

            if (method == null) {
                method = getBusObjectByPath(id, scp.ObjTree);
            }

            var id = id.substring(id.lastIndexOf(".") + 1);

            if (method == null) {
                method = getBusObjectByPath(id, scp.ObjTree);
            }
            //drop is removed
            //if (method) {
            //    scp.addBusMethod(selectedXmlMethods, method);
            //}
        }
    }
}
function onBusMethodDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
}


function onQueryIDActionKeyDown(eargs) {
    var rootScope = getScopeByFileName("MainPage");
    var input = eargs.target;
    var arrText = getSplitArray($(input).val(), input.selectionStart);
    var data = rootScope.getFirstLevelEntityIntellisense();
    var propName = "EntityName";
    if (arrText.length > 0) {
        for (var index = 0; index < arrText.length; index++) {
            var item = data.filter(function (x) {
                return x.ID == arrText[index].substring(3);
            });
            if (item.length > 0) {
                propName = "";
                if (item[0].DisplayName != undefined && item[0].Queries != undefined && (eargs.char == "." || arrText[index].charAt(arrText[index].length - 1 || arrText[index].contains("."))) && index < arrText.length) {
                    data = getQueryIntellisense(item[0]);
                }
                else if (item[0].QueryType != undefined && index < arrText.length) {
                    data = [];
                }
            }
        }
    }
    setRuleIntellisense($(input), data, rootScope, propName);

    if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
        $(input).autocomplete("search", $(input).val());
        eargs.preventDefault();
    }
}

function getQueryIntellisense(ObjEntity, text) {
    var data = [];
    data = ObjEntity.Queries;
    return data;
}


var undoFormatQuery = function (query) {
    var retVal = '';

    if (query != undefined && query != '') {
        retVal = removeSpecialCharacters(query, true);
        var lst = ['Select', 'From', 'Where', 'Equal', 'Coma', 'And', 'On', 'InnerJoin', 'LeftOuterJoin', 'RightOuterJoin', 'FullOuterJoin', 'CrossJoin',
            'Join', 'OrderBy', 'GroupBy', 'As', 'OpeningBracket', 'ClosingBracket'];
        for (var i = 0; i < lst.length; i++) {
            retVal = CheckAndUndoFormat(retVal, lst[i]);
        }
    }
    return retVal;
};

var CheckAndUndoFormat = function (query, keywordType) {
    switch (keywordType) {
        case 'Select':
            query = query.replace(new RegExp('(SELECT\n\t)', 'gi'), "SELECT");
            break;
        case 'From':
            query = query.replace(new RegExp('(\nFROM\n\t)', 'gi'), "FROM");
            break;
        case 'Where':
            query = query.replace(new RegExp('(\nWHERE\n\t)', 'gi'), "WHERE");
            break;
        case 'Equal':
            query = query.replace(new RegExp('( = )', 'gi'), "=");
            break;
        case 'Coma':
            query = query.replace(new RegExp('(,\n\t)', 'gi'), ",");
            break;
        case 'And':
            query = query.replace(new RegExp('( AND\n\t)', 'gi'), " AND");
            break;
        case 'On':
            query = query.replace(new RegExp('(\n\t ON)', 'gi'), "ON");
            break;
        case 'Join':
            //UtilityFunctions.CheckAndFormatJoinKeyword(ref query);
            break;

        case 'InnerJoin':
            query = query.replace(new RegExp('(\n\tINNER JOIN)', 'gi'), "INNER JOIN");
            break;
        case 'LeftOuterJoin':
            query = query.replace(new RegExp('(\n\t LEFT OUTER JOIN)', 'gi'), "LEFT OUTER JOIN");
            break;
        case 'RightOuterJoin':
            query = query.replace(new RegExp('(\n\t RIGHT OUTER JOIN)', 'gi'), "RIGHT OUTER JOIN");
            break;
        case 'FullOuterJoin':
            query = query.replace(new RegExp('(\n\t FULL OUTER JOIN)', 'gi'), "FULL OUTER JOIN");
            break;


        case 'CrossJoin':
            query = query.replace(new RegExp('(\n\tCROSS JOIN)', 'gi'), "CROSS JOIN");
            break;
        case 'OrderBy':
            query = query.replace(new RegExp('(\nORDER\t BY )', 'gi'), "ORDER BY");
            break;
        case 'GroupBy':
            query = query.replace(new RegExp('(\nGROUP\t BY )', 'gi'), "GROUP BY");
            break;
        case 'As':
            query = query.replace(new RegExp('( \tAS)', 'gi'), " AS");
            break;
    }
    return query;
};


var checkAndFormat = function (query, keywordType) {
    switch (keywordType) {
        case 'Select':
            query = query.replace(new RegExp('(select )', 'gi'), "SELECT\n\t");
            break;
        case 'From':
            query = query.replace(new RegExp('( from )', 'gi'), "\nFROM\n\t");
            break;
        case 'Where':
            query = query.replace(new RegExp('( where )', 'gi'), "\nWHERE\n\t");
            break;
        case 'Equal':
            query = query.replace(new RegExp('( =)', 'gi'), "=");
            query = query.replace(new RegExp('(= )', 'gi'), "=");
            query = query.replace(new RegExp('(=)', 'gi'), " = ");
            break;
        case 'Coma':
            query = query.replace(new RegExp('( ,)', 'gi'), ",");
            query = query.replace(new RegExp('(, )', 'gi'), ",");
            query = query.replace(new RegExp('(,)', 'gi'), ",\n\t");
            break;
        case 'And':
            query = query.replace(new RegExp('( and)', 'gi'), " AND\n\t");
            break;
        case 'On':
            query = query.replace(new RegExp('( on)', 'gi'), "\n\t ON");
            break;
        case 'Join':
            //SASUtilityFunctions.CheckAndFormatJoinKeyword(ref query);
            break;

        case 'InnerJoin':
            query = query.replace(new RegExp('(inner join)', 'gi'), "\n\tINNER JOIN");
            break;
        case 'LeftOuterJoin':
            query = query.replace(new RegExp('(left outer join )', 'gi'), "LEFT OUTER JOIN");
            query = query.replace(new RegExp('(left outer join)', 'gi'), "\n\t LEFT OUTER JOIN ");
            break;
        case 'RightOuterJoin':
            query = query.replace(new RegExp('(right outer join )', 'gi'), "RIGHT OUTER JOIN");
            query = query.replace(new RegExp('(right outer join)', 'gi'), "\n\tRIGHT OUTER JOIN ");
            break;
        case 'FullOuterJoin':
            query = query.replace(new RegExp('(full outer join )', 'gi'), "FULL OUTER JOIN");
            query = query.replace(new RegExp('(full outer join)', 'gi'), "\n\tFULL OUTER JOIN ");
            break;
        case 'CrossJoin':
            query = query.replace(new RegExp('(cross join )', 'gi'), "CROSS JOIN");
            query = query.replace(new RegExp('(cross join)', 'gi'), "\n\tCROSS JOIN ");
            break;
        case 'OrderBy':
            query = query.replace(new RegExp('(order by )', 'gi'), "ORDER BY");
            query = query.replace(new RegExp('(order by)', 'gi'), "\nORDER BY ");
            break;
        case 'GroupBy':
            query = query.replace(new RegExp('(group by )', 'gi'), "GROUP BY");
            query = query.replace(new RegExp('(group by)', 'gi'), "\nGROUP BY ");
            break;
        case 'As':
            query = query.replace(new RegExp('( as)', 'gi'), " AS");
            break;
        case 'OpeningBracket':
            query = query.replace(new RegExp(' \\( ', 'gi'), "(");
            break;
        case 'ClosingBracket':
            query = query.replace(new RegExp(' \\) ', 'gi'), ")");
            break;
    }
    return query;
};
var removeSpecialCharacters = function (query, isUndo) {
    var ind = 0;
    var sb = '';
    for (var i = 0; i < query.length; i++) {
        if (query[i] != '\n' && query[i] != '\r' && query[i] != '\t')
            sb = sb.concat(query[i]);
        else {
            //adding space in place of \n or \t or \r 
            if (sb[sb.length - 1] != ' ' && isUndo) {
                var nextInd = ind + 1;

                if (nextInd < query.length && query[nextInd] != ' ' && query[nextInd] != '\n' && query[nextInd] != '\r' && query[nextInd] != '\t')
                    sb = sb.concat(" ");
            }
        }

        ind++;
    }
    return sb;
};

function getBusObjectByPath(text, objModel) {
    //removing leading and trailing period(.)
    text.replace(/^[.\s]+|[.\s]+$/g, "");

    var busProps = text.split('.');
    if (busProps)
        for (var i = 0; i < busProps.length; i++) {
            if (objModel != null) {
                var models = objModel.ChildProperties.filter(function (x) { return x.ShortName == busProps[i]; });
                if (models != null && models.length > 0) {
                    objModel = models[0];
                    if (objModel.HasChildItems && objModel.ChildProperties.length == 0) {
                        //(this.ParentVM as LoadGroupVM).BusinessObjectVM.LoadSelectedChildProperties(objModel);
                    }
                }
                else {
                    var models = objModel.lstMethods.filter(function (x) { return x.ShortName == busProps[i]; });
                    if (models != null && models.length > 0) {
                        objModel = models[0];
                    }
                    else {
                        objModel = null;
                        break;
                    }
                }
            }
        }
    return objModel;
}

function GetFullItemPathFromBusObjectTree(field) {
    var itempath = field.ShortName;
    var parent = field.objParent;
    while (parent && !parent.IsMainBusObject && parent.ShortName) {
        itempath = parent.ShortName + "." + itempath;
        parent = parent.objParent;
    }
    return itempath;
}

function addEditAttributeConstraint(scope, attributeName, entityModel, entityId, autoSave) {
    var newScope = scope.$new();
    newScope.entityModel = entityModel;
    newScope.attributeName = attributeName;
    newScope.entityId = entityId;
    newScope.autoSave = autoSave;
    newScope.dialog = scope.$root.showDialog(newScope, "Add Constraint", "<add-edit-attribute-constraint auto-save='autoSave' entity-id='entityId' entity-model='entityModel' attribute-name='attributeName'></add-edit-attribute-constraint>", { showclose: true, isInlineHtml: true, height: 500, width: 1000 })
}