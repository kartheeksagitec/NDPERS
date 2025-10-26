app.directive("rowconditionitemdirective", ["$compile", "ngDialog", "$rootScope", function ($compile, ngDialog, $rootScope) {
    var changedataheaderid = function (elements, items) {
        if (elements.length > 0) {
            var i = 0;
            for (i = 0; i < elements.length; i++) {
                if (elements[i].Item != undefined) {
                    if (elements[i].Item.ItemType == "assignheader") {
                        if (elements[i].Item.DataHeaderID == items.Item.DataHeaderID) {
                            elements[i].Item.Expression = items.Item.Expression;
                        }
                    }
                }
                if (elements[i].Cells != undefined) {
                    changedataheaderid(elements[i].Cells, items);
                }
            }
        }
    };
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '=',
            rciviewmode: '='
        },
        templateUrl: "Rule/views/DecisionTable/rowconditionitem.html",
        link: function (scope, element, attrs) {
            scope.init = function () {
                scope.conditionType = "default";
                if (scope.items.Item.ItemType == 'notesheader' || scope.items.Item.ItemType == 'returnheader') {
                    scope.conditionType = "1";
                }
                else if ((scope.items.Item.ItemType == 'colheader' || scope.items.Item.ItemType == 'rowheader')) {
                    scope.conditionType = "2";
                }
                else if (scope.items.Item.ItemType == 'assignheader') {
                    scope.conditionType = "3";
                }
                else if (scope.items.Item.ItemType != 'notesheader' && scope.items.Item.ItemType != 'returnheader' && scope.items.Item.ItemType != 'colheader' && scope.items.Item.ItemType != 'rowheader' && scope.items.Item.ItemType != 'assignheader' && scope.items.Item.ItemType != 'notes') {
                    scope.conditionType = "4";
                }
            };
            scope.getclass = function (obj) {
                var classname = "";
                if (obj != undefined) {
                    if (obj.Item.IsSelected) {
                        classname = "decision-table-selection";
                    }
                    else if (obj.Item.ItemType == "if") {
                        classname = "dt-if";
                    }
                    else if (obj.Item.ItemType == "colheader" || obj.Item.ItemType == "rowheader") {
                        classname = "dt-row-col-header";
                    }
                    else if (obj.Item.ItemType == "assignheader" || obj.Item.ItemType == 'notesheader' || obj.Item.ItemType == 'returnheader') {
                        classname = "dt-assign-header";
                    }
                    else {
                        classname = "dt-assign";
                    }
                    if (obj.Item.isAdvanceSearched) {
                        classname += ' bckgGrey';
                    }
                    if (obj.Item.IsSelected && obj.Item.isAdvanceSearched) {
                        classname += ' bckgGreen';
                    }
                }

                if (scope.rciviewmode == 'Developer View') {
                    classname += ' inner-div';
                }
                else {
                    classname += ' inner-div-analystview';
                }
                return classname;
            };
            scope.columnnamechange = function () {
                changedataheaderid(scope.logicalrule.Rows, scope.items);
            };
            scope.onActionKeyDown = function (eargs) {
                controllerScope = getCurrentFileScope();
                if (controllerScope) {
                    controllerScope.onActionKeyDown(eargs);
                }
            };
            scope.notesdailog = function (notesobj) {
                if (notesobj.Item.ItemType == "notes") {
                    var newscope = scope.$new();
                    var data = JSON.stringify(notesobj.Item);
                    var objnotes = JSON.parse(data);
                    objnotes.Value = notesobj.Item.NotesValue;
                    newscope.objText = objnotes;
                    newscope.dialog = $rootScope.showDialog(newscope, "Edit Notes", "Rule/views/EditNotesTemplate.html", { width: 400, height: 400 });
                    newscope.SaveChangedTextData = function () {
                        $rootScope.EditPropertyValue(notesobj.Item.NotesValue, notesobj.Item, "NotesValue", newscope.objText.Value);
                        newscope.onCancelClick();
                    };
                    newscope.onCancelClick = function () {
                        if (newscope.dialog) {
                            newscope.dialog.close();
                        }
                    };
                }
            };
            scope.AddContentEditable = function (event) {
                if ($(event.currentTarget).attr('content-editable') == undefined) {
                    var currentElement = $(event.currentTarget);
                    var caretPos = $(event.currentTarget).text().length;
                    var conditionType = currentElement.attr('data-condition-type');
                    // for description we need to compile the current element 
                    var elementTocompile = currentElement;
                    currentElement.attr('ng-model', $(event.currentTarget).attr('bindpropertyname'));
                    currentElement.attr('content-editable', '');
                    if (scope.items.Item.ItemType == "assignheader") {
                        currentElement.attr('ng-change', 'columnnamechange()');
                    }
                    if (conditionType == "Expression") {
                        elementTocompile = currentElement.closest("[expression-wrapper]");
                    }
                    $compile(elementTocompile)(scope);
                    setTimeout(function () {
                        var htmlElement = $("#" + $rootScope.currentopenfile.file.FileName).find("#" + scope.items.Item.NodeID + " span[data-condition-type= '" + conditionType + "']");
                        if (htmlElement && htmlElement.length > 0) {
                            htmlElement.first().focus();
                            setSelectionByCharacterOffsets(htmlElement[0], caretPos, caretPos);
                        }
                    }, 4);
                }
            };
            scope.init();
        }
    };
}]);

app.directive("rowconditiondirective", ["$compile", "$rootScope", function ($compile, $rootScope) {

    var AddItem = function (obj, param, itemscope, logicalrule) {
        var dataheadernodeid = generateUUID();
        $rootScope.UndRedoBulkOp("Start");
        var dataheader = { DataHeaderID: '', Description: '', Expression: '', ItemType: 'dataheader', NodeID: dataheadernodeid };
        for (var i = 0; i < logicalrule.DataHeaders.length; i++) {
            if (logicalrule.DataHeaders[i].NodeID == itemscope.objChild.Item.DataHeaderID) {
                $rootScope.InsertItem(dataheader, logicalrule.DataHeaders, i + 1);
                //logicalrule.DataHeaders.splice(i + 1, 0, dataheader);
                break;
            }
        }

        var indexofobj = logicalrule.Rows.indexOf(obj);
        for (var j = indexofobj; j < logicalrule.Rows.length; j++) {
            for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                if (logicalrule.Rows[j].Cells[k].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                    if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'assignheader' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'notesheader' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'returnheader') {
                        if (param == 'actions') {
                            itemName = 'assignheader';
                        }

                        else if (param == 'notes') {
                            itemName = 'notesheader';
                        }

                        else if (param == 'return') {
                            itemName = 'returnheader';
                        }
                        var cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID() } };
                    }
                    else if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'assign' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'notes' || logicalrule.Rows[j].Cells[k].Item.ItemType == 'return') {
                        if (param == 'actions') {
                            itemName = 'assign';
                        }

                        else if (param == 'notes') {
                            itemName = 'notes';
                        }

                        else if (param == 'return') {
                            itemName = 'return';
                        }
                        var cell;
                        if (param == "notes") {
                            cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID(), NotesValue: '' } };
                        }
                        else {
                            cell = { Colspan: '1', Rowspan: '1', Item: { DataHeaderID: dataheadernodeid, Description: '', Expression: '', ItemType: itemName, NodeID: generateUUID() } };
                        }
                    }
                    $rootScope.InsertItem(cell, logicalrule.Rows[j].Cells, k + 1);
                }
            }
        }

        // setting colspan to 'if' conditions
        if (indexofobj > 0) {
            for (var i = 0; i < obj.Cells.length; i++) {
                if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                    for (var j = indexofobj - 1; j >= 0; j--) {
                        var index = 0;
                        for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                                index = logicalrule.Rows[j].Cells[k].Colspan;
                            }
                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                                var totalcolspan = index + logicalrule.Rows[j].Cells[k].Colspan;
                                if (totalcolspan > i && index <= i) {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan + 1);
                                    break;
                                }
                                else {
                                    index = index + logicalrule.Rows[j].Cells[k].Colspan;
                                }
                            }
                        }
                    }
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteItem = function (obj, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        var count = 0;
        var countofif = 0;
        for (var k = 0; k < obj.Cells.length; k++) {
            if ((obj.Cells[k].Item.ItemType == 'assignheader') || (obj.Cells[k].Item.ItemType == 'notesheader') || (obj.Cells[k].Item.ItemType == 'returnheader')) {
                count++;
            }
        }
        if (indexofobj > 0) {
            for (var l = 0; l < logicalrule.Rows[indexofobj - 1].Cells.length; l++) {
                if (logicalrule.Rows[indexofobj - 1].Cells[l].Item.ItemType == 'if') {
                    countofif++;
                }
            }
        }

        isvalid = true;
        isreturnpresent = false;
        if (itemscope.objChild.Item.ItemType == "assignheader" && indexofobj > -1) {
            var headerid = itemscope.objChild.Item.DataHeaderID;
            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "returnheader") {
                    isreturnpresent = true;
                    break;
                }
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "assignheader") {
                    if (logicalrule.Rows[indexofobj].Cells[i].Item.DataHeaderID == headerid) {
                        isvalid = false;
                    } else {
                        isvalid = true;
                        break;
                    }
                }
            }

            if (isreturnpresent) {
                isvalid = true;
            }
        }

        if (itemscope.objChild.Item.ItemType == "returnheader" && indexofobj > -1) {
            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "assignheader") {
                    isvalid = true;
                    break;
                } else if (logicalrule.Rows[indexofobj].Cells[i].Item.ItemType == "returnheader" && logicalrule.Rows[indexofobj].Cells[i].Item.DataHeaderID != itemscope.objChild.Item.DataHeaderID) {
                    isvalid = true;
                    break;
                }
                else {
                    isvalid = false;
                }
            }
        }

        if (count < 2 || count == countofif || !isvalid) {
            alert("Atleast one action column should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = 0; i < logicalrule.DataHeaders.length; i++) {
                if (logicalrule.DataHeaders[i].NodeID == itemscope.objChild.Item.DataHeaderID) {
                    $rootScope.DeleteItem(logicalrule.DataHeaders[i], logicalrule.DataHeaders);
                    //logicalrule.DataHeaders.splice(i, 1);
                    break;
                }
            }
            var indexofobj = logicalrule.Rows.indexOf(obj);
            // setting colspan to 'if' conditions
            if (indexofobj > 0) {
                for (var i = 0; i < obj.Cells.length; i++) {
                    if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        for (var j = indexofobj - 1; j >= 0; j--) {
                            var index = 0;
                            for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                                if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                                    index = logicalrule.Rows[j].Cells[k].Colspan;
                                }
                                if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                                    var totalcolspan = index + logicalrule.Rows[j].Cells[k].Colspan;
                                    if (totalcolspan > i && index <= i) {
                                        $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan - 1);
                                        break;
                                    }
                                    else {
                                        index = index + logicalrule.Rows[j].Cells[k].Colspan;
                                    }
                                }
                            }
                        }
                        $rootScope.DeleteItem(obj.Cells[i], obj.Cells);
                        //obj.Cells.splice(i, 1);
                    }

                }
            }
            else {
                for (var i = 0; i < obj.Cells.length; i++) {
                    if (obj.Cells[i].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        $rootScope.DeleteItem(obj.Cells[i], obj.Cells);
                        //obj.Cells.splice(i, 1);
                    }
                }
            }

            // Deleting the condition

            for (var j = indexofobj + 1; j < logicalrule.Rows.length; j++) {
                for (var k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[k].Item.DataHeaderID == itemscope.objChild.Item.DataHeaderID) {
                        $rootScope.DeleteItem(logicalrule.Rows[j].Cells[k], logicalrule.Rows[j].Cells);
                        //logicalrule.Rows[j].Cells.splice(k, 1);
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var AddConditionInLeftOrRight = function (obj, param, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        $rootScope.UndRedoBulkOp("Start");
        if (param == 'Right') {
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    var rowheaderid;
                    if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                        if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                            rowheaderid = generateUUID();
                            var description = logicalrule.Rows[i].Cells[j].Item.Description;
                            var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                            var cell = { Colspan: '1', Rowspan: '1', Item: { Description: description, Expression: expression, ItemType: 'rowheader', NodeID: rowheaderid } };
                            $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                            //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                        }
                    }
                    else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                        if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                            var rowspan = logicalrule.Rows[i].Cells[j].Rowspan;
                            var description = logicalrule.Rows[i].Cells[j].Item.Description;
                            var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                            var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: description, Expression: expression, ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                            $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                            //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                        }
                    }
                }
            }
        }
        else if (param == 'Left') {
            var indexofele = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
            if (indexofele == 0) {
                var rowheaderid = generateUUID();
                var rowspan = 0;
                var positionofsplicing;
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        var rowheaderid;
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                                var cell = { Colspan: '1', Rowspan: '1', Item: { Description: '', Expression: '', ItemType: 'rowheader', NodeID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j);
                                //logicalrule.Rows[i].Cells.splice(j, 0, cell);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (itemscope.objChild.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                                rowspan += logicalrule.Rows[i].Cells[j].Rowspan;
                                if (positionofsplicing == undefined) {
                                    positionofsplicing = i;
                                }
                            }
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length > logicalrule.Rows[i].conditionitemcount) {
                        logicalrule.Rows[i].conditionitemcount += 30;
                    }
                }
                var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: '', Expression: '', ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                if (logicalrule.Rows[positionofsplicing] && logicalrule.Rows[positionofsplicing].Cells) {
                    $rootScope.InsertItem(cell, logicalrule.Rows[positionofsplicing].Cells, 0);
                    if (logicalrule.Rows[positionofsplicing].Cells.length > logicalrule.Rows[positionofsplicing].conditionitemcount) {
                        logicalrule.Rows[positionofsplicing].conditionitemcount += 30;
                    }
                }
                //logicalrule.Rows[positionofsplicing].Cells.splice(0, 0, cell);
            }
            else {
                var objofprevele = logicalrule.Rows[indexofobj].Cells[indexofele - 1];

                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        var rowheaderid;
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (objofprevele.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.NodeID) {
                                rowheaderid = generateUUID();
                                var description = logicalrule.Rows[i].Cells[j].Item.Description;
                                var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                                var cell = { Colspan: '1', Rowspan: '1', Item: { Description: description, Expression: expression, ItemType: 'rowheader', NodeID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                                //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (objofprevele.Item.NodeID == logicalrule.Rows[i].Cells[j].Item.HeaderID) {
                                var rowspan = logicalrule.Rows[i].Cells[j].Rowspan;
                                var description = logicalrule.Rows[i].Cells[j].Item.Description;
                                var expression = logicalrule.Rows[i].Cells[j].Item.Expression;
                                var cell = { Colspan: '1', Rowspan: rowspan, Item: { Description: description, Expression: expression, ItemType: 'if', NodeID: generateUUID(), HeaderID: rowheaderid } };
                                $rootScope.InsertItem(cell, logicalrule.Rows[i].Cells, j + 1);
                                //logicalrule.Rows[i].Cells.splice(j + 1, 0, cell);
                                break;
                            }
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length > logicalrule.Rows[i].conditionitemcount) {
                        logicalrule.Rows[i].conditionitemcount += 30;
                    }
                }
            }
        }

        // setting colspan to colheader

        for (var i = 0; i < logicalrule.Rows.length; i++) {
            for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                    $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", logicalrule.Rows[i].Cells[j].Colspan + 1);
                    break;
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var AddConditionInTopOrBottom = function (obj, param, itemscope, logicalrule) {
        var indexofobj = logicalrule.Rows.indexOf(obj);
        $rootScope.UndRedoBulkOp("Start");
        if (param == 'Top') {
            if (indexofobj > 0) {
                var cells = { Cells: [] };
                for (var i = 0; i < logicalrule.Rows[indexofobj - 1].Cells.length; i++) {
                    colspan = logicalrule.Rows[indexofobj - 1].Cells[i].Colspan;
                    rowspan = logicalrule.Rows[indexofobj - 1].Cells[i].Rowspan;
                    itemName = logicalrule.Rows[indexofobj - 1].Cells[i].Item.ItemType;
                    descriprion = logicalrule.Rows[indexofobj - 1].Cells[i].Item.Description;
                    expression = logicalrule.Rows[indexofobj - 1].Cells[i].Item.Expression;
                    var colheaderid;
                    if (itemName == 'colheader') {
                        colheaderid = generateUUID();
                        cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: colheaderid } };
                    }
                    else {
                        cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: generateUUID(), HeaderID: colheaderid } };
                    }
                    cells.Cells.splice(i, 0, cell);
                }
                $rootScope.InsertItem(cells, logicalrule.Rows, indexofobj);
                //logicalrule.Rows.splice(indexofobj, 0, cells);
            }
            else {
                var cells = { Cells: [] };
                var colheaderid = generateUUID();
                var count = 0;
                var length;
                for (var i = 0; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            length = logicalrule.Rows[i].Cells.length;
                            count++;
                        }
                    }
                }
                if (logicalrule.Rows[indexofobj].Cells[0].Item.ItemType == "colheader") {
                    descriprion = logicalrule.Rows[indexofobj].Cells[0].Item.Description;
                    expression = logicalrule.Rows[indexofobj].Cells[0].Item.Expression;
                }
                else {
                    descriprion = '';
                    expression = '';
                }
                cell = { Colspan: count, Rowspan: 1, Item: { Description: descriprion, Expression: expression, ItemType: 'colheader', NodeID: colheaderid } };
                cells.Cells.push(cell);

                cell = { Colspan: length - count, Rowspan: 1, Item: { Description: '', Expression: '', ItemType: 'if', NodeID: generateUUID(), HeaderID: colheaderid } };
                cells.Cells.push(cell);
                $rootScope.InsertItem(cells, logicalrule.Rows, 0);
                //logicalrule.Rows.splice(0, 0, cells);
            }
        }

        else if (param == 'Bottom') {
            var cells = { Cells: [] };

            for (var i = 0; i < logicalrule.Rows[indexofobj].Cells.length; i++) {
                colspan = logicalrule.Rows[indexofobj].Cells[i].Colspan;
                rowspan = logicalrule.Rows[indexofobj].Cells[i].Rowspan;
                itemName = logicalrule.Rows[indexofobj].Cells[i].Item.ItemType;
                descriprion = logicalrule.Rows[indexofobj].Cells[i].Item.Description;
                expression = logicalrule.Rows[indexofobj].Cells[i].Item.Expression;
                var colheaderid;
                if (itemName == 'colheader') {
                    colheaderid = generateUUID();
                    cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: colheaderid } };
                }
                else {
                    cell = { Colspan: colspan, Rowspan: rowspan, Item: { Description: descriprion, Expression: expression, ItemType: itemName, NodeID: generateUUID(), HeaderID: colheaderid } };
                }

                cells.Cells.splice(i, 0, cell);
            }
            $rootScope.InsertItem(cells, logicalrule.Rows, indexofobj + 1);
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteCondition = function (obj, itemscope, logicalrule) {
        $rootScope.UndRedoBulkOp("Start");
        var indexofobj = logicalrule.Rows.indexOf(obj);
        if (itemscope.objChild.Item.ItemType == 'colheader') {
            if (logicalrule.Rows[indexofobj + 1].Cells[0].Item.ItemType != 'rowheader') {
                $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                //logicalrule.Rows.splice(indexofobj, 1);
            }
            else {
                // getting 1st element data
                var objele;
                for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                    if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'notesheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'returnheader') {
                        objele = logicalrule.Rows[indexofobj + 1].Cells[j];
                        break;
                    }
                }

                if (indexofobj == 0 || (indexofobj > 0 && logicalrule.Rows[indexofobj - 1].Cells.length == 2)) {
                    for (var i = indexofobj + 1; i < logicalrule.Rows.length; i++) {
                        count = 0;
                        for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {

                            if (logicalrule.Rows[i].Cells[j].Item.DataHeaderID == objele.Item.DataHeaderID) {
                                count++;
                            }
                            if (count > 1) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                j--;
                            }
                        }
                    }

                    if (indexofobj > 0) {
                        $rootScope.EditPropertyValue(logicalrule.Rows[indexofobj - 1].Cells[1].Colspan, logicalrule.Rows[indexofobj - 1].Cells[1], "Colspan", logicalrule.Rows[indexofobj + 1].Cells.length - logicalrule.Rows[indexofobj - 1].Cells[0].Colspan);
                    }
                    $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                    //logicalrule.Rows.splice(indexofobj, 1);
                }

                else {
                    var objElements;
                    var celllength;
                    var startindex;
                    var endindex;
                    for (var i = 0; i < logicalrule.Rows[indexofobj - 1].Cells.length - 1; i++) {
                        if (logicalrule.Rows[indexofobj - 1].Cells[i].Item.ItemType == 'colheader') {
                            startindex = logicalrule.Rows[indexofobj - 1].Cells[i].Colspan;
                        }
                        if (logicalrule.Rows[indexofobj - 1].Cells[i + 1].Item.ItemType == 'if') {
                            endindex = startindex + logicalrule.Rows[indexofobj - 1].Cells[i + 1].Colspan;
                        }

                        for (var j = 0; j < logicalrule.Rows[indexofobj].Cells.length - 1; j++) {
                            var startindexofcurr;
                            var endindexofcurr;
                            if (logicalrule.Rows[indexofobj].Cells[j].Item.ItemType == 'colheader') {
                                startindexofcurr = logicalrule.Rows[indexofobj].Cells[j].Colspan;
                            }
                            if (logicalrule.Rows[indexofobj].Cells[j + 1].Item.ItemType == 'if') {
                                endindexofcurr = startindexofcurr + logicalrule.Rows[indexofobj].Cells[j + 1].Colspan;
                            }

                            if (startindex == startindexofcurr) {
                                if (objElements == undefined) {
                                    objElements = [];
                                    for (var k = indexofobj + 1; k < logicalrule.Rows.length; k++) {
                                        var objcells = [];
                                        for (var l = 0; l < logicalrule.Rows[k].Cells.length; l++) {
                                            if (celllength == undefined) {
                                                celllength = logicalrule.Rows[k].Cells.length;
                                            }
                                            if (celllength == logicalrule.Rows[k].Cells.length) {
                                                if (endindexofcurr > l && startindexofcurr <= l) {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'rowheader') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                else if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'if') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                            else {
                                                var diff = celllength - logicalrule.Rows[k].Cells.length;

                                                if ((endindexofcurr - diff) > l && (startindexofcurr - diff) <= l) {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'rowheader') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                                else if (logicalrule.Rows[k].Cells[l].Item.ItemType == 'if') {
                                                    objcells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                        }
                                        var cells = { Cells: objcells };
                                        objElements.push(cells);
                                    }
                                    startindexofcurr = endindexofcurr;
                                }
                                else {
                                    var m = 0;
                                    for (var k = indexofobj + 1; k < logicalrule.Rows.length; k++) {

                                        for (var l = 0; l < logicalrule.Rows[k].Cells.length; l++) {
                                            if (celllength == logicalrule.Rows[k].Cells.length) {
                                                if (endindexofcurr > l && startindexofcurr <= l) {
                                                    objElements[m].Cells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                            else {
                                                var diff = celllength - logicalrule.Rows[k].Cells.length;
                                                if ((endindexofcurr - diff) > l && (startindexofcurr - diff) <= l) {
                                                    objElements[m].Cells.push(logicalrule.Rows[k].Cells[l]);
                                                }
                                            }
                                        }
                                        m++;
                                    }
                                    startindexofcurr = endindexofcurr;
                                }
                            }
                            else {
                                startindexofcurr = endindexofcurr;
                            }
                        }
                        startindex = endindex;
                    }

                    var p = 0;
                    for (var i = indexofobj + 1; i < logicalrule.Rows.length; i++) {
                        logicalrule.Rows[i] = objElements[p];
                        p++;
                    }
                    $rootScope.DeleteItem(logicalrule.Rows[indexofobj], logicalrule.Rows);
                    //logicalrule.Rows.splice(indexofobj, 1);

                    //setting column span
                    var objele;
                    for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                        if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'notesheader' || logicalrule.Rows[indexofobj + 1].Cells[j].Item.ItemType == 'assignheader') {
                            objele = logicalrule.Rows[indexofobj + 1].Cells[j];
                            break;
                        }
                    }
                    var span;
                    var elecount = 0;
                    for (var j = 0; j < logicalrule.Rows[indexofobj + 1].Cells.length; j++) {
                        if (logicalrule.Rows[indexofobj + 1].Cells[j].Item.DataHeaderID == objele.Item.DataHeaderID) {

                            if (elecount > 0) {
                                span = j;
                                break;
                            }
                            else {
                                elecount++;
                            }
                        }
                    }
                    var colspan;
                    var totalcolspan = 0;
                    for (var k = 0; k < logicalrule.Rows[indexofobj - 1].Cells.length; k++) {

                        if (logicalrule.Rows[indexofobj - 1].Cells[k].Item.ItemType == 'colheader') {
                            colspan = span - logicalrule.Rows[indexofobj - 1].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[indexofobj - 1].Cells[k].Item.ItemType == 'if') {
                            $rootScope.EditPropertyValue(logicalrule.Rows[indexofobj - 1].Cells[k].Colspan, logicalrule.Rows[indexofobj - 1].Cells[k], "Colspan", colspan);
                            totalcolspan += colspan;
                        }
                    }

                    if ((indexofobj - 1) > 0) {
                        for (var i = indexofobj - 2; i >= 0; i--) {
                            for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                                var colspanofcell = totalcolspan / (logicalrule.Rows[i].Cells.length - 1);
                                if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", colspanofcell);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (itemscope.objChild.Item.ItemType == 'rowheader') {
            var count = 0;
            var ifcount = 0;
            var indexofitem = obj.Cells.indexOf(itemscope.objChild);
            var blntrue = false;
            if (obj.Cells[indexofitem + 1].Item.ItemType == "rowheader") {
                blntrue = true;
            }
            for (var k = 0; k < obj.Cells.length; k++) {
                if (itemscope.objChild.Item.ItemType == obj.Cells[k].Item.ItemType) {
                    count++;
                }
            }

            if (count < 2) {
                alert("Atleast one  Row condition should be present");
            }
            else {
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'rowheader') {
                            if (logicalrule.Rows[i].Cells[j].Item.NodeID == itemscope.objChild.Item.NodeID) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                break;
                            }
                        }
                        else if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            if (logicalrule.Rows[i].Cells[j].Item.HeaderID == itemscope.objChild.Item.NodeID) {
                                $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                //logicalrule.Rows[i].Cells.splice(j, 1);
                                ifcount++;
                                break;
                            }
                        }
                    }
                }
                if (indexofitem > 0) {
                    var previtemnodeid = obj.Cells[indexofitem - 1].Item.NodeID;
                    var itemcount = 0;
                    for (var l = indexofobj + 1; l < logicalrule.Rows.length; l++) {
                        for (m = 0; m < logicalrule.Rows[l].Cells.length; m++) {
                            if (logicalrule.Rows[l].Cells[m].Item.HeaderID == previtemnodeid && logicalrule.Rows[l].Cells[m].Item.ItemType == 'if') {
                                itemcount++;
                                break;
                            }
                        }
                    }

                    var maxlength = 0;
                    var leastrowlength = 0;
                    for (var i = 0; i < logicalrule.Rows.length; i++) {
                        if (maxlength < logicalrule.Rows[i].Cells.length) {
                            maxlength = logicalrule.Rows[i].Cells.length;
                        }

                        if (leastrowlength == 0 || leastrowlength > logicalrule.Rows[i].Cells.length) {
                            leastrowlength = logicalrule.Rows[i].Cells.length;
                        }
                    }

                    if (!blntrue && itemcount != ifcount) {
                        for (var i = logicalrule.Rows.length - 1; i >= 0; i--) {
                            if (logicalrule.Rows[i].Cells.length == leastrowlength && logicalrule.Rows[i].Cells[0].Item.ItemType != 'rowheader' && logicalrule.Rows[i].Cells[0].Item.ItemType != 'colheader') {
                                var cellslength = leastrowlength;
                                var headerid = undefined;
                                for (var j = i - 1; j >= 0; j--) {
                                    if (logicalrule.Rows[j].Cells.length > cellslength && logicalrule.Rows[j].Cells[0].Item.ItemType == 'if') {
                                        for (k = 0; k < logicalrule.Rows[j].Cells.length; k++)
                                            if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if' && (headerid == undefined || logicalrule.Rows[j].Cells[k].Item.HeaderID != headerid)) {
                                                $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Rowspan, logicalrule.Rows[j].Cells[k], "Rowspan", logicalrule.Rows[j].Cells[k].Rowspan - 1);
                                            }
                                            else {
                                                cellslength = logicalrule.Rows[j].Cells.length;
                                                headerid = logicalrule.Rows[j].Cells[0].Item.HeaderID;
                                                break;
                                            }
                                    }
                                }
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                            }
                        }
                    }
                }

                // setting colspan to colheader

                for (var i = 0; i < logicalrule.Rows.length; i++) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Colspan, logicalrule.Rows[i].Cells[j], "Colspan", logicalrule.Rows[i].Cells[j].Colspan - 1);
                            break;
                        }
                    }
                }
            }
        }
        $rootScope.UndRedoBulkOp("End");
    };

    var AddConditionforRow = function (obj, param, itemscope, logicalrule) {
        $rootScope.UndRedoBulkOp("Start");
        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var indexofobj = logicalrule.Rows.indexOf(obj);
        var lengthofobj = obj.Cells.length;
        var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
        var objElements = [];
        if (param == 'RowCondition') {
            //getting the rows
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                if (lengthofobj == maxlength && indexofcell == 0) {
                    if ((logicalrule.Rows[i].Cells.length < lengthofobj) || i == indexofobj) {
                        var cells = { Cells: logicalrule.Rows[i].Cells };
                        var data = JSON.stringify(cells);
                        var clonecells = JSON.parse(data);
                        objElements.push(clonecells);
                    }
                    else {
                        break;
                    }
                }
                else {
                    var objcells = [];
                    if ((logicalrule.Rows[i].Cells.length < lengthofobj - indexofcell) || i == indexofobj) {

                        if (i == indexofobj) {
                            for (var j = indexofcell; j < logicalrule.Rows[i].Cells.length; j++) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        else {
                            for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        var cells = { Cells: objcells };

                        var data = JSON.stringify(cells);
                        var clonecells = JSON.parse(data);
                        objElements.push(clonecells);
                    }
                    else {
                        break;
                    }
                }
            }
            // setting nodeid

            for (var i = 0; i < objElements.length; i++) {
                for (var j = 0; j < objElements[i].Cells.length; j++) {
                    objElements[i].Cells[j].Item.NodeID = generateUUID();
                }
            }

            // adding the rows
            var p = 0;

            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                if (p < objElements.length) {
                    $rootScope.InsertItem(objElements[p], logicalrule.Rows, i + objElements.length);
                    //logicalrule.Rows.splice(i + objElements.length, 0, objElements[p]);
                    p++;
                }
                else {
                    break;
                }
            }

            // setting rowspan
            var objheaderid = itemscope.objChild.Item.HeaderID;

            for (var i = indexofobj; i >= 0; i--) {
                if (logicalrule.Rows[i].Cells.length >= lengthofobj) {
                    for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[j].Item.HeaderID == objheaderid) {
                            objheaderid = logicalrule.Rows[i].Cells[0].Item.HeaderID;
                            lengthofobj = logicalrule.Rows[i].Cells.length;
                            break;
                        }
                        else {
                            $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Rowspan, logicalrule.Rows[i].Cells[j], "Rowspan", logicalrule.Rows[i].Cells[j].Rowspan + objElements.length);
                        }
                    }
                    if (logicalrule.Rows[i].Cells.length == maxlength) {
                        break;
                    }
                }
            }

        }
        $rootScope.UndRedoBulkOp("End");
    };

    var DeleteConditionforRow = function (obj, param, itemscope, logicalrule) {

        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var count = 0;
        for (var j = 0; j < logicalrule.Rows.length; j++) {
            if (logicalrule.Rows[j].Cells.length == maxlength && logicalrule.Rows[j].Cells[0].Item.ItemType == 'if') {
                count++;
            }
        }
        if (count < 2 && obj.Cells.length == maxlength) {
            alert("Atleast one condition should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            var indexofobj = logicalrule.Rows.indexOf(obj);
            var indexofobjele = indexofobj;

            var lengthofobj = obj.Cells.length;
            var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);
            var objElements = [];
            var count = 0;
            var cell;
            if (param == 'RowCondition') {
                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    if (lengthofobj == maxlength && indexofcell == 0) {
                        if ((logicalrule.Rows[i].Cells.length < lengthofobj) || i == indexofobj) {
                            $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                            //logicalrule.Rows.splice(i, 1);
                            indexofobj -= 1;
                            indexofobjele -= 1;
                            i--;
                        }
                        else {
                            break;
                        }
                    }
                    else {

                        if ((logicalrule.Rows[i].Cells.length < lengthofobj - indexofcell) || i == indexofobj) {
                            count++;
                            if (cell == undefined) {
                                for (var m = 0; m < logicalrule.Rows[i].Cells.length; m++) {
                                    if (logicalrule.Rows[i].Cells[m].Rowspan == itemscope.objChild.Rowspan) {
                                        indexofcell = m;
                                        cell = logicalrule.Rows[i].Cells[m];
                                        break;
                                    }
                                }
                            }

                            if (indexofcell == 0 && i == indexofobj) {
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                                indexofobjele -= 1;
                                indexofobj -= 1;
                                i--;
                            }
                            else if (i == indexofobj) {


                                if (itemscope.objChild.Rowspan == logicalrule.Rows[i].Cells[0].Rowspan && logicalrule.Rows[i].Cells.length == maxlength) {
                                    $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                    //logicalrule.Rows.splice(i, 1);
                                    count = 0;
                                }
                                else {
                                    for (var j = indexofcell; j < logicalrule.Rows[i].Cells.length; j++) {
                                        $rootScope.DeleteItem(logicalrule.Rows[i].Cells[j], logicalrule.Rows[i].Cells);
                                        //logicalrule.Rows[i].Cells.splice(j, 1);
                                        //indexofobj -= 1;
                                        //indexofobjele -= 1;
                                        j--;
                                    }

                                    for (l = 1; logicalrule.Rows.length; l++) {
                                        if (logicalrule.Rows[i + l].Cells[0].Item.HeaderID == cell.Item.HeaderID) {
                                            for (var k = 0; k < logicalrule.Rows[i + l].Cells.length; k++) {
                                                $rootScope.PushItem(logicalrule.Rows[i + l].Cells[k], logicalrule.Rows[i].Cells);
                                                //logicalrule.Rows[i].Cells.push(logicalrule.Rows[i + l].Cells[k]);
                                            }
                                            break;
                                        }
                                        else {
                                            $rootScope.DeleteItem(logicalrule.Rows[i + l], logicalrule.Rows);
                                            //logicalrule.Rows.splice(i + l, 1);
                                            l--;
                                        }

                                    }
                                    $rootScope.DeleteItem(logicalrule.Rows[i + l], logicalrule.Rows);
                                    //logicalrule.Rows.splice(i + l, 1);

                                    count = cell.Rowspan;
                                    break;
                                }
                            }
                            else {
                                $rootScope.DeleteItem(logicalrule.Rows[i], logicalrule.Rows);
                                //logicalrule.Rows.splice(i, 1);
                                i--;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }

                //setting rowspan
                if (cell != undefined) {
                    var objheaderid = cell.Item.HeaderID;
                }
                else {
                    var objheaderid = itemscope.objChild.Item.HeaderID;
                }

                for (var i = indexofobjele; i >= 0; i--) {
                    if (logicalrule.Rows[i].Cells.length >= lengthofobj) {
                        for (var j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                            if (logicalrule.Rows[i].Cells[j].Item.HeaderID == objheaderid) {
                                objheaderid = logicalrule.Rows[i].Cells[0].Item.HeaderID;
                                lengthofobj = logicalrule.Rows[i].Cells.length;
                                break;
                            }
                            else {
                                $rootScope.EditPropertyValue(logicalrule.Rows[i].Cells[j].Rowspan, logicalrule.Rows[i].Cells[j], "Rowspan", logicalrule.Rows[i].Cells[j].Rowspan - count);
                            }
                        }
                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            break;
                        }
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var AddConditionforColumn = function (obj, param, itemscope, logicalrule) {

        var maxlength = 0;
        for (var i = 0; i < logicalrule.Rows.length; i++) {
            if (maxlength < logicalrule.Rows[i].Cells.length) {
                maxlength = logicalrule.Rows[i].Cells.length;
            }
        }
        var startindex;
        var endindex;
        var objElements = [];
        for (var j = 0; j < obj.Cells.length; j++) {
            if (obj.Cells[j].Item.ItemType == 'colheader') {
                startindex = obj.Cells[j].Colspan;
            }
            if (obj.Cells[j].Item.ItemType == 'if') {
                endindex = startindex + obj.Cells[j].Colspan;
            }
            if (endindex != undefined) {
                if (itemscope.objChild.Item.NodeID == obj.Cells[j].Item.NodeID) {


                    break;
                }
                else {
                    startindex = endindex;
                }
            }
        }

        var indexofobj = logicalrule.Rows.indexOf(obj);
        var lengthofobj = obj.Cells.length;
        var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);

        if (param == 'ColumnCondition') {
            $rootScope.UndRedoBulkOp("Start");
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                var elestartindex = 0;
                var eleendindex = 0;
                var objcells = [];
                for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (eleendindex != 0) {
                            if (startindex <= elestartindex && endindex >= eleendindex) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                                elestartindex = eleendindex;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }

                    else {

                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            if (endindex > j && startindex <= j) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                        else {
                            var diff = maxlength - logicalrule.Rows[i].Cells.length;
                            if ((endindex - diff) > j && (startindex - diff) <= j) {
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                        }
                    }
                }

                var cells = { Cells: objcells };
                var data = JSON.stringify(cells);
                var clonecells = JSON.parse(data);
                objElements.push(clonecells);
            }

            // setting nodeid

            for (var i = 0; i < objElements.length; i++) {
                for (var j = 0; j < objElements[i].Cells.length; j++) {
                    objElements[i].Cells[j].$$hashKey = generateUUID() + "";
                    objElements[i].Cells[j].Item.NodeID = generateUUID();
                }
            }

            //inserting the elements

            var l = 0;
            for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                var elestartindex = 0;
                var eleendindex = 0;
                var objcells = [];
                for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                    if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                        }
                        if (eleendindex != 0) {
                            if (endindex == eleendindex) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }

                    else {

                        if (logicalrule.Rows[i].Cells.length == maxlength) {
                            if ((endindex - 1) == j) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                        }
                        else {
                            var diff = maxlength - logicalrule.Rows[i].Cells.length;
                            if (((endindex - 1) - diff) == j) {
                                var m = 1;
                                for (var k = 0; k < objElements[l].Cells.length; k++) {
                                    $rootScope.InsertItem(objElements[l].Cells[k], logicalrule.Rows[i].Cells, j + m);
                                    //logicalrule.Rows[i].Cells.splice(j + m, 0, objElements[l].Cells[k]);
                                    m++;
                                }
                                break;
                            }
                        }
                    }
                }
                l++;
            }

            // setting Column span
            var objelemaxlength = 0;
            for (var i = 0; i < objElements.length; i++) {
                if (objelemaxlength < objElements[i].Cells.length) {
                    objelemaxlength = objElements[i].Cells.length;
                }
            }

            for (var j = indexofobj - 1; j >= 0; j--) {
                var elestartindex = 0;
                var eleendindex = 0;
                for (k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (eleendindex != 0) {

                            if (elestartindex <= startindex && eleendindex >= endindex) {
                                $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan + objelemaxlength);
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    var DeleteConditionforColumn = function (obj, param, itemscope, logicalrule) {
        if (obj.Cells.length == 2) {
            alert("Atleast one condition should be present");
        }
        else {
            $rootScope.UndRedoBulkOp("Start");
            var maxlength = 0;
            for (var i = 0; i < logicalrule.Rows.length; i++) {
                if (maxlength < logicalrule.Rows[i].Cells.length) {
                    maxlength = logicalrule.Rows[i].Cells.length;
                }
            }
            var startindex;
            var endindex;
            var objElements = [];
            for (var j = 0; j < obj.Cells.length; j++) {
                if (obj.Cells[j].Item.ItemType == 'colheader') {
                    startindex = obj.Cells[j].Colspan;
                }
                if (obj.Cells[j].Item.ItemType == 'if') {
                    endindex = startindex + obj.Cells[j].Colspan;
                }
                if (endindex != undefined) {
                    if (itemscope.objChild.Item.NodeID == obj.Cells[j].Item.NodeID) {


                        break;
                    }
                    else {
                        startindex = endindex;
                    }
                }
            }

            var indexofobj = logicalrule.Rows.indexOf(obj);
            var lengthofobj = obj.Cells.length;
            var indexofcell = logicalrule.Rows[indexofobj].Cells.indexOf(itemscope.objChild);

            if (param == 'ColumnCondition') {

                for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
                    var elestartindex = 0;
                    var eleendindex = 0;
                    var objcells = [];
                    for (j = 0; j < logicalrule.Rows[i].Cells.length; j++) {
                        if (logicalrule.Rows[i].Cells[0].Item.ItemType == 'colheader') {

                            if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'colheader') {
                                elestartindex = logicalrule.Rows[i].Cells[j].Colspan;
                                objcells.push(logicalrule.Rows[i].Cells[j]);
                            }
                            if (logicalrule.Rows[i].Cells[j].Item.ItemType == 'if') {
                                eleendindex = elestartindex + logicalrule.Rows[i].Cells[j].Colspan;
                            }
                            if (eleendindex != 0) {
                                if (startindex <= elestartindex && endindex >= eleendindex) {
                                    elestartindex = eleendindex;
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                    elestartindex = eleendindex;
                                }
                            }
                        }
                        else {

                            if (logicalrule.Rows[i].Cells.length == maxlength) {
                                if (endindex > j && startindex <= j) {
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                }
                            }
                            else {
                                var diff = maxlength - logicalrule.Rows[i].Cells.length;
                                if ((endindex - diff) > j && (startindex - diff) <= j) {
                                }
                                else {
                                    objcells.push(logicalrule.Rows[i].Cells[j]);
                                }
                            }
                        }
                    }

                    var cells = { Cells: objcells };
                    var data = JSON.stringify(cells);
                    var clonecells = JSON.parse(data);
                    objElements.push(clonecells);
                }
            }

            // setting columnspan

            for (var j = indexofobj - 1; j >= 0; j--) {
                var elestartindex = 0;
                var eleendindex = 0;
                for (k = 0; k < logicalrule.Rows[j].Cells.length; k++) {
                    if (logicalrule.Rows[j].Cells[0].Item.ItemType == 'colheader') {

                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'colheader') {
                            elestartindex = logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (logicalrule.Rows[j].Cells[k].Item.ItemType == 'if') {
                            eleendindex = elestartindex + logicalrule.Rows[j].Cells[k].Colspan;
                        }
                        if (eleendindex != 0) {

                            if (elestartindex <= startindex && eleendindex >= endindex) {
                                if (logicalrule.Rows[j].Cells[k].Colspan != itemscope.objChild.Colspan) {
                                    $rootScope.EditPropertyValue(logicalrule.Rows[j].Cells[k].Colspan, logicalrule.Rows[j].Cells[k], "Colspan", logicalrule.Rows[j].Cells[k].Colspan - itemscope.objChild.Colspan);

                                }
                                else {
                                    $rootScope.DeleteItem(logicalrule.Rows[j].Cells[k], logicalrule.Rows[j].Cells);
                                    //logicalrule.Rows[j].Cells.splice(k, 1);
                                }
                                break;
                            }
                            else {
                                elestartindex = eleendindex;
                            }
                        }
                    }
                }
            }


            //replacing the elements
            var q = indexofobj;
            while (q < logicalrule.Rows.length) {
                $rootScope.DeleteItem(logicalrule.Rows[q], logicalrule.Rows);
            }

            for (var i = 0; i < objElements.length; i++) {
                $rootScope.PushItem(objElements[i], logicalrule.Rows);
            }
            //var q = 0;
            //for (var i = indexofobj; i < logicalrule.Rows.length; i++) {
            //    logicalrule.Rows[i] = objElements[q];
            //    q++;
            //}
            $rootScope.UndRedoBulkOp("End");
        }

    };

    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '=',
            rcviewmode: '=',
            returntype: '='
        },
        template: '<td rowconditionitemdirective ng-repeat="objChild in items.Cells" items="objChild" context-menu="menuOptionsforitems" valign="top" rowspan={{objChild.Rowspan}} colspan={{objChild.Colspan}} class="dt-table-cell"  logicalrule="logicalrule" rciviewmode="rcviewmode" ng-mousedown="objStepSelectionChanged(objChild)"></td>',
        link: function (scope, element, attrs) {
            scope.menuOptionsforitems = [
                ['Add Condition ', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionforRow(obj, 'RowCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'if'));
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteConditionforRow(obj, 'RowCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'if'));
                }], null,

                ['Add Condition ', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionforColumn(obj, 'ColumnCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'colheader'));
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteConditionforColumn(obj, 'ColumnCondition', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    return ($itemScope.objChild.Item.ItemType == 'if' && (obj.Cells[0].Item.ItemType == 'colheader'));
                }], null,

                ['Add Action ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'actions', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null;
                }],
                null,
                ['Add Return ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'return', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return (($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null) && (scope.returntype);
                }],
                null,
                ['Add Notes ', function ($itemScope) {
                    var obj = scope.items;
                    AddItem(obj, 'notes', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/assignheader/) || $itemScope.objChild.Item.ItemType.match(/returnheader/) || $itemScope.objChild.Item.ItemType.match(/notesheader/)) != null;
                }],
                null,
                ['Delete Action ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/assignheader/) != null;
                }],
                null,
                ['Delete Return ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/returnheader/) != null;
                }],
                null,
                ['Delete Notes ', function ($itemScope) {
                    var obj = scope.items;
                    DeleteItem(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/notesheader/) != null;
                }],
                null,
                ['Add Horizantal Condition', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Top', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    var obj = scope.items;
                    var indexofobj = scope.logicalrule.Rows.indexOf(obj);
                    return ($itemScope.objChild.Item.ItemType.match(/rowheader/) && (indexofobj == 0));
                }],
                null,

                ['Add Condition in Right', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInLeftOrRight(obj, 'Right', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/rowheader/) != null;
                }],
                null,
                ['Add Condition in Left', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInLeftOrRight(obj, 'Left', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/rowheader/) != null;
                }], null,

                ['Add Condition in Bottom', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Bottom', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/colheader/) != null;
                }],
                null,
                ['Add Condition in Top', function ($itemScope) {
                    var obj = scope.items;
                    AddConditionInTopOrBottom(obj, 'Top', $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return $itemScope.objChild.Item.ItemType.match(/colheader/) != null;
                }],
                null,
                ['Delete Condition', function ($itemScope) {
                    var obj = scope.items;
                    DeleteCondition(obj, $itemScope, scope.logicalrule);
                }, function ($itemScope) {
                    return ($itemScope.objChild.Item.ItemType.match(/colheader/) || $itemScope.objChild.Item.ItemType.match(/rowheader/)) != null;
                }], null
            ];
            scope.objStepSelectionChanged = function (step) {
                var rootScope = getCurrentFileScope();
                rootScope.onDecisionStepChange(step);
            };
        }
    };
}]);

