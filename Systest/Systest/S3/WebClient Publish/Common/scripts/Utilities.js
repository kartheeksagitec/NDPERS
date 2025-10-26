String.prototype.contains = function (str, ignoreCase) {
    if (ignoreCase) {
        return this.toLowerCase().indexOf(str.toLowerCase()) > -1;
    }
    else {
        return this.indexOf(str) > -1;
    }
};
String.prototype.startsWith = function (str, ignoreCase) {
    if (ignoreCase) {
        return this.toLowerCase().indexOf(str.toLowerCase()) === 0;
    }
    else {
        return this.indexOf(str) === 0;
    }
};
String.prototype.endsWith = function (str, ignoreCase) {
    if (ignoreCase) {
        if (this.toLowerCase().indexOf(str.toLowerCase()) > -1) {
            return this.toLowerCase().lastIndexOf(str.toLowerCase()) == this.length - str.length;
        } else {
            return false;
        }
    }
    else {
        if (this.indexOf(str) > -1) {
            return this.lastIndexOf(str) == this.length - str.length;
        } else {
            return false;
        }
    }
};
String.prototype.capitalizeFirstLetter = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
};
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
};

String.prototype.trimStart = function (trimString, ignoreCase) {
    var targetString = this;
    if (!trimString) {
        trimString = " ";
    }
    if (targetString.startsWith(trimString, ignoreCase)) {
        targetString = targetString.substring(trimString.length);
        targetString = targetString.trimStart(trimString, ignoreCase);
    }
    return targetString;
};
String.prototype.trimEnd = function (trimString, ignoreCase) {
    var targetString = this;
    if (!trimString) {
        trimString = " ";
    }
    if (targetString.endsWith(trimString, ignoreCase)) {
        targetString = targetString.substring(0, targetString.length - trimString.length);
        targetString = targetString.trimEnd(trimString, ignoreCase);
    }
    return targetString;
};
String.prototype.trimFull = function (trimString, ignoreCase) {
    return this.trimStart(trimString, ignoreCase).trimEnd(trimString, ignoreCase);
};

Array.prototype.unique = function () {
    var list = this;
    return list.filter(function (item, pos) {
        return list.indexOf(item) == pos;
    });
};

Array.prototype.difference = function (a, propertyname) {
    if (!propertyname) return this.filter(function (i) { return a.indexOf(i) < 0; });
    else return this.filter(function (i) {
        return a.map(function (element) { if (element) { return element[propertyname]; } }).indexOf(i[propertyname]) < 0;
    });
};

function getCurrentFileScope() {
    element = $('body[ng-controller="MainController"]');
    if (element) {
        rootScope = angular.element(element).scope().$root;
        if (rootScope.isConfigureSettingsVisible) {
            return getScopeByFileName("SettingsPage");
        }
        else if (rootScope && rootScope.currentopenfile && rootScope.currentopenfile.file) {
            return getScopeByFileName(rootScope.currentopenfile.file.FileName);
        }
    }
}

function getScopeByFileName(fileName) {
    /// <summary>Get the scope of a div which is created for the specified filename. For Main page pass fileName as 'MainPage'. For Start page pass fileName as 'StartPage'. For Settings page pass fileName as 'SettingsPage'. </summary>
    /// <param name="fileName" type="string">filename for which scope is required.</param>
    /// <returns type="object">scope of a div which is created for the specified filename.</returns>
    var element;
    var scope;
    if (fileName == "MainPage") {
        element = $('body[ng-controller="MainController"]');
    }

    else if (fileName == "StartPage") {
        element = $('div[ng-controller="StartController"]');
    }
    else if (fileName == "SettingsPage") {
        element = $('div[ng-controller="configureSettingsController"]');
    }
    else if (fileName == "PageLayout") {
        element = $('div[ng-controller="PageLayoutController"]');
    }
    else if (fileName == "ParameterNavigation") {
        element = $('div[ng-controller="NavigationParameterController"]');
    }
    else if (fileName == "ImageCondition") {
        element = $('div[ng-controller="ImageConditionController"]');
    }
    else if (fileName == "createNewObject") {
        element = $('div[ng-controller="createNewObjectController"]');
    }
    else if (fileName == "NewButtonWizard") {
        element = $('div[ng-controller="NewButtonWizardController"]');
    }
    else if (fileName == "ExecuteQuery") {
        element = $('div[ng-controller="executequerycontroller"]');
    }
    else if (fileName == "CustomSettings") {
        element = $('div[ng-controller="customSettingsController"]');
    }
    else if (fileName == "Form") {
        element = $('div[ng-controller="FormController"]');
    }
    else if (fileName == "SearchFiles") {
        element = $('div[ng-controller="SearchFilesController"]');
    }
    else if (fileName == "AutoCompleteColumnsController") {
        element = $('div[ng-controller="AutoCompleteColumnsController"]');
    }
    else if (fileName == "NavigationParameterController") {
        element = $('div[ng-controller="NavigationParameterController"]');
    }
    else if (fileName == "RetrievalControlsController") {
        element = $('div[ng-controller="RetrievalControlsController"]');
    }
    else if (fileName == "EntityValidationRulesController") {
        element = $('div[ng-controller="EntityValidationRulesController"]');
    }
    else if (fileName == "ConvertToHtx") {
        element = $('div[ng-controller="ConvertToHtxController"]');
    }
    else if (fileName == "ValidationUtilityControl") {
        element = $('div[ng-controller="ValidationUtilityControl"]');
    }
    else {
        element = $("div[id='" + fileName + "']").find("div[ng-controller]");
    }

    if (element) {
        scope = angular.element(element).scope();
    }
    return scope;
}

function GetVM(ControlName, acntrlVM) {
    var parentVM = acntrlVM;
    while (parentVM) {
        if (parentVM.Name == ControlName) {
            return parentVM;
        }
        parentVM = parentVM.ParentVM;
    }

}

function startsWith(actulalString, searchString, position) {
    return actulalString.indexOf(searchString, position) === position;
    //lowercase removed bcozit matches wrong in case of form
    // return actulalString.toLowerCase().indexOf(searchString.toLowerCase(), position) === position;
}

function endsWith(str, suffix) {
    if (str) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }
}

function setSingleLevelAutoComplete(controlSelection, data, scope, propertyName, displaypropertyname, callBackFunction) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // press up or down key    
        return;
    }
    // this code should be at server side
    if (data) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (a && b) {
                if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname) && a[displaypropertyname]) {
                    nameA = a[displaypropertyname].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName) && a[propertyName]) {
                    nameA = a[propertyName].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName) && a.dictAttributes[propertyName]) {
                    nameA = a.dictAttributes[propertyName].toLowerCase();
                }
                else if (a.hasOwnProperty("ID") && a.ID) {
                    nameA = a.ID.toLowerCase();
                }

                else if (a && a.toLowerCase) {
                    nameA = a.toLowerCase();
                }

                if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname) && b[displaypropertyname]) {
                    nameB = b[displaypropertyname].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName) && b[propertyName]) {
                    nameB = b[propertyName].toLowerCase();
                }
                else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName) && b.dictAttributes[propertyName]) {
                    nameB = b.dictAttributes[propertyName].toLowerCase();
                }
                else if (b.hasOwnProperty("ID") && b.ID) {
                    nameB = b.ID.toLowerCase();
                }
                else if (b && b.toLowerCase) {
                    nameB = b.toLowerCase();
                }

                if (nameA && nameB) {
                    if (nameA < nameB) //sort string ascending
                        return -1;
                    if (nameA > nameB)
                        return 1;
                }
            }
            return 0; //default return value (no sorting)
        });

        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "span") {
                    var doc = controlSelection[0].ownerDocument || controlSelection[0].document;
                    var win = doc.defaultView || doc.parentWindow;
                    var Position;
                    var sel;
                    if (win && typeof win.getSelection != "undefined") {
                        sel = win.getSelection();
                        if (sel.rangeCount > 0) {
                            var range = win.getSelection().getRangeAt(0);
                            Position = range.getBoundingClientRect();
                        }
                    }
                    var options;
                    if (!Position || (Position.left == 0 && Position.top == 0)) {
                        options = {
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px",
                            "z-index": "999999999",
                        };
                    } else {
                        options = {
                            left: (Position.left) + "px",
                            top: (Position.top + Position.height) + "px",
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px",
                            "z-index": "999999999",
                        };
                    }
                    $("#dvIntellisense > ul").css(options);
                    setIntellisensePosition(event);
                }
                else if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type"))) {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        options.left = (pos.left) + "px";
                        // options.top = (pos.top) + "px";

                    }
                    $("#dvIntellisense > ul").css(options);
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    if ($(controlSelection).data('ui-autocomplete')) {
                        var $results = $(controlSelection).autocomplete("widget");
                        windowWidth = $(window).width(),
                            height = $results.height(),
                            inputHeight = $(controlSelection).height(),
                            windowsHeight = $(window).height();
                        if (windowsHeight < $results.position().top + height + inputHeight) {
                            newTop = $results.position().top - height - inputHeight - 10;
                            $results.css("top", newTop + "px");
                        }
                    }
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    $("#dvIntellisense > ul").css(options);
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            source: function (request, response) {
                // single level autocomplete will not filter with any special characters except alphabets
                //var isAlphabets = request.term.match(/^[a-zA-Z0-9 -_]*$/);
                // if (isAlphabets) {
                var result = $.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        var templogo = "";
                        var templogo = "";
                        if (!value) {
                            return;
                        }
                        if (value.Type && value.Type == "Method") {
                            templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/method.png'/>";
                        }
                        else if (value.Type && value.Type == "Rule") {
                            if (value.RuleType && value.RuleType == "LogicalRule") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/logical_rule.png'/>";

                            }
                            else if (value.RuleType && value.RuleType == "DecisionTable") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/decision_table..png'/>";
                            }
                            else if (value.RuleType && value.RuleType == "ExcelMatrix") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Excel_matrix.png'/>";
                            }

                        }
                        else if (value.Type && value.Type == "Property") {
                            if (value.DataType && value.DataType.toLowerCase() == "int") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                            }
                            else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                        }
                        else if (value.Type && value.Type == "Column") {
                            if (value.DataType && value.DataType.toLowerCase() == "int") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                            }
                            else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                            }
                            else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                        }
                        else if (value.Type && value.Type == "Collection") {
                            templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                        }
                        else if (value.Type && value.Type == "Object") {
                            templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                        }
                        if (typeof displaypropertyname != "undefined" && value.hasOwnProperty(displaypropertyname)) {
                            return {
                                label: value[displaypropertyname],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                            return {
                                label: value[propertyName],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.dictAttributes && value.dictAttributes.hasOwnProperty(propertyName)) {
                            return {
                                label: value.dictAttributes[propertyName],
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (value && typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value,
                                logo: templogo
                            };
                        }
                        else if (value) {
                            return {
                                label: value.ID,
                                value: value.ID,
                                logo: templogo
                            };
                        }


                    }), request.term/* extractLast(request.term, this.element[0].selectionStart)*/);
                response(result.slice(0, 50));
                // }
            },
            select: function (event, ui) {
                var control = this;
                var value = "";
                if (event.target.localName == "span") {
                    //this.innerText = this.innerText + ui.item.value.ID;
                    //this.innerText = [this.innerText.slice(0, caretindex), ui.item.value.ID, this.innerText.slice(caretindex)].join('');
                    if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                        value = ui.item.value[propertyName];
                    }
                    else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                        value = ui.item.value.dictAttributes[propertyName];
                    }
                    else if (ui.item.value.ID) {
                        value = ui.item.value.ID;
                    }
                    else {
                        value = ui.item.value;
                    }
                    this.innerText = value;
                    var scope1 = angular.element($(this)).scope();
                    if (scope1) {
                        scope1.$broadcast("UpdateOnClick", controlSelection[0]);
                    }
                }
                else {
                    if (typeof propertyName != "undefined" && ui.item.value.hasOwnProperty(propertyName)) {
                        value = ui.item.value[propertyName];
                    }
                    else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                        value = ui.item.value.dictAttributes[propertyName];
                    }
                    else if (ui.item.value.ID) {
                        value = ui.item.value.ID;
                    }
                    else {
                        value = ui.item.value;
                    }
                    control.value = value;
                }
                $(this).trigger("change");
                if (callBackFunction && typeof callBackFunction === "function") {
                    callBackFunction({ obj: ui.item.value });
                }
                return false;
            },
            close: function () {
                $(".page-header-fixed").css("pointer-events", "auto");
            }
        });
        //data = getIntellisenseRecord($(controlSelection).val(), data, propertyName);
    }
    //else return false;

}
$.ui.autocomplete.prototype._renderItem = function (ul, item) {
    var temp = "";
    if (item.logo && item.logo != "") {
        temp = item.logo + item.label;
    }
    else {
        temp = item.label;
    }

    return $("<li></li>")
        .data("item.autocomplete", item)
        .append("<a>" + temp + "</a>")
        .appendTo(ul);
};
function setSingleLevelAutoCompleteForCodeValues(controlSelection, data, scope, propertyName, displaypropertyname, model, type) {
    if (data && data.length > 0) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName].toLowerCase();
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID.toLowerCase();
            }
            else if (a.toLowerCase) {
                nameA = a.toLowerCase();
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName].toLowerCase();
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID.toLowerCase();
            }
            else if (b.toLowerCase) {
                nameB = b.toLowerCase();
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
            return 0; //default return value (no sorting)
        });
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type"))) {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        options.left = (pos.left) + "px";
                        options.top = (pos.top) + "px";

                    }
                    $("#dvIntellisense > ul").css(options);
                }
                setIntellisensePosition(event);

            },
            source: function (request, response) {

                //response();
                response($.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        if (typeof displaypropertyname != "undefined" && value.hasOwnProperty(displaypropertyname)) {
                            return {
                                label: value[displaypropertyname],
                                value: value,
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                            return {
                                label: value[propertyName],
                                value: value,
                            };
                        }
                        else if (typeof propertyName != "undefined" && value.dictAttributes && value.dictAttributes.hasOwnProperty(propertyName)) {
                            return {
                                label: value.dictAttributes[propertyName],
                                value: value,
                            };
                        }
                        else if (typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value
                            };
                        }
                        else {

                            return {
                                label: value.ID,
                                value: value.ID,
                            };
                        }
                    }), extractLast(request.term, this.element[0].selectionStart)));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                $(".ui-autocomplete > li").attr("title", ui.item.label);
                return false;
            },
            select: function (event, ui) {
                var control = this;
                if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                    this.value = ui.item.value[propertyName];
                    if (type == "role") {
                        model.dictAttributes.role = ui.item.value.CodeID;
                    } else if (type == "skill") {
                        model.dictAttributes.skill = ui.item.value.CodeID;
                    }
                    else if (type == "location") {
                        model.dictAttributes.location = ui.item.value.CodeID;
                    } else if (type == "position") {
                        model.dictAttributes.position = ui.item.value.CodeID;
                    } else if (type == "authoritylevel") {
                        model.dictAttributes.authorityLevel = ui.item.value.CodeID;
                    } else if (type == "user") {
                        model.dictAttributes.user = ui.item.value.CodeID;
                    }
                }
                else if (typeof propertyName != "undefined" && ui.item.value.dictAttributes && ui.item.value.dictAttributes.hasOwnProperty(propertyName)) {
                    this.value = ui.item.value.dictAttributes[propertyName];
                }
                else if (ui.item.value.ID) {
                    control.value = ui.item.value.ID;
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

function containsMathOpe(text) {
    if (text.contains("+") || text.contains("-") || text.contains("*") || text.contains("/") || text.contains(" ") ||
        text.contains(">") || text.contains("<") || text.contains("%") || text.contains("==") || text.contains("!") ||
        text.contains("=") || text.contains("(")) {
        return true;
    }

    return false;
}
function isMathope(char) {
    if (char == '+' || char == '-' || char == '*' || char == '/' || char == ' ' || char == '\n' || char == '>' || char == '<' ||
        char == '%' || char == '=' || char == '"' || char == '(' || char == ')' || char == ',' || char == '?' || char == ':') {
        return true;
    }

    return false;
}


function getSplitArray(text, caretIndex) {
    var retVal = [];
    //if (containsMathOpe(text)) {
    var txtArr = text.split('');
    var res = [];
    for (var i = caretIndex - 1; i >= 0; i--) {
        if (txtArr[i] != '"' && isMathope(txtArr[i]))
            break;
        res.push(txtArr[i]);
    }

    if (res.length > 0) {
        text = res.reverse().join('');
        retVal = text.split('.');
    }
    //}
    //else {
    //    retVal = text.split('.');
    //}
    return retVal;
}

function split(val) {
    return val.split(".");
}
function extractLast(term, caretIndex) {
    var arr = getSplitArray(term, caretIndex);
    if (arr.length > 0) {
        return arr.pop();
    }
    else {
        return "";
    }
}


function caretPosition(element) {
    var caretOffset = 0;
    var doc = element.ownerDocument || element.document;
    var win = doc.defaultView || doc.parentWindow;
    var sel;
    if (typeof win.getSelection != "undefined") {
        sel = win.getSelection();
        if (sel.rangeCount > 0) {
            var range = win.getSelection().getRangeAt(0);
            var preCaretRange = range.cloneRange();
            preCaretRange.selectNodeContents(element);
            preCaretRange.setEnd(range.endContainer, range.endOffset);
            caretOffset = preCaretRange.toString().length;
        }
    } else if ((sel = doc.selection) && sel.type != "Control") {
        var textRange = sel.createRange();
        var preCaretTextRange = doc.body.createTextRange();
        preCaretTextRange.moveToElementText(element);
        preCaretTextRange.setEndPoint("EndToEnd", textRange);
        caretOffset = preCaretTextRange.text.length;
    }
    return caretOffset;
}

function setMultilevelAutoCompleteForObjectTreeIntellisense(controlSelection, data, propertyName, onSelectionCallback, scope) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // press up or down key    
        return;
    }
    if (data && data.length > 0) {
        data = data.sort(function (a, b) {
            var nameA;
            var nameB;
            if (a.hasOwnProperty(propertyName)) {
                if (a[propertyName] && a[propertyName].toLowerCase) nameA = a[propertyName].toLowerCase();
                else nameA = a[propertyName];
            }
            else if (a.hasOwnProperty("ID")) {
                if (a.ID && a.ID.toLowerCase) nameA = a.ID.toLowerCase();
                else nameA = a.ID;
            }
            else if (a && a.toLowerCase) nameA = a.toLowerCase();

            if (b.hasOwnProperty(propertyName)) {
                if (b[propertyName] && b[propertyName].toLowerCase) nameB = b[propertyName].toLowerCase();
                else nameB = b[propertyName];
            }
            else if (b.hasOwnProperty("ID")) {
                if (b.ID && b.ID.toLowerCase) nameB = b.ID.toLowerCase();
                else nameB = b.ID;
            }
            else if (b && b.toLowerCase) nameB = b.toLowerCase();
            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
            return 0; //default return value (no sorting)
        });
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var options = {
                        width: "200px",
                        height: 'auto',
                        overflow: 'auto',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    };
                    if (controlSelection[0].localName == "textarea") {
                        var pos = controlSelection.textareaHelper('caretPosAbs');
                        var windowWidth = $(window).width();
                        if (pos.left + 220 > windowWidth) {
                            var diffinpos = (pos.left + 220) - windowWidth;
                            options.left = (pos.left - (diffinpos)) + "px";
                        } else {
                            options.left = (pos.left) + "px";
                        }
                        options.top = (pos.top) + "px";

                        $("#dvIntellisense > ul").css(options);
                    }
                    else {
                        $("#dvIntellisense > ul").css(options);
                        setIntellisensePosition(event);
                    }
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            source: function (request, response) {

                //response();
                if (data && data.length > 0)
                    response($.ui.autocomplete.filter(
                        $.map(data, function (value, key) {

                            var templogo = "";
                            if (value.Type && value.Type == "Method") {
                                templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/method.png'/>";
                            }
                            else if (value.Type && value.Type == "Rule") {
                                if (value.RuleType && value.RuleType == "LogicalRule") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/logical_rule.png'/>";

                                }
                                else if (value.RuleType && value.RuleType == "DecisionTable") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/decision_table..png'/>";
                                }
                                else if (value.RuleType && value.RuleType == "ExcelMatrix") {
                                    templogo = "<img style='margin-right:6px' src='images/Home/small_filetype/Excel_matrix.png'/>";
                                }

                            }
                            else if (value.Type && value.Type == "Property") {
                                if (value.DataType && value.DataType.toLowerCase() == "int") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                                }
                                else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                                }
                            }
                            else if (value.Type && value.Type == "Column") {
                                if (value.DataType && value.DataType.toLowerCase() == "int") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-integer.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "bool") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-boolean.png'/>";
                                }
                                else if (value.DataType && (value.DataType.toLowerCase() == "collection" || value.DataType.toLowerCase() == "cdocollection" || value.DataType.toLowerCase() == "list")) {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "datetime") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-datetime.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "decimal") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-decimal.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "double") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-double.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "float") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-float.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "string") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-string.png'/>";
                                }
                                else if (value.DataType && value.DataType.toLowerCase() == "object") {
                                    templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                                }
                            }
                            else if (value.Type && value.Type == "Collection") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-collection.png'/>";
                            }
                            else if (value.Type && value.Type == "Object") {
                                templogo = "<img style='margin-right:6px' src='images/Form/icon-object.png'/>";
                            }
                            if (value.hasOwnProperty(propertyName) && value[propertyName]) {
                                return {
                                    label: value[propertyName],
                                    value: value,
                                    logo: templogo
                                };
                            }
                            else if (value.hasOwnProperty("ID") && value.ID) {
                                return {
                                    label: value.ID,
                                    value: value,
                                    logo: templogo
                                };
                            }
                            else {
                                return {
                                    label: value,
                                    value: value,
                                    logo: templogo

                                };
                            }
                            if (templogo) {
                                return {
                                    label: value,
                                    value: value,
                                    logo: templogo

                                };
                            }
                        }), extractLast(request.term, this.element[0].selectionStart)));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                if (ui.item && ui.item.value && "Description" in ui.item.value && ui.item.value.Description) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Description);
                }
                else if (ui.item && ui.item.value && "Tooltip" in ui.item.value && ui.item.value.Tooltip) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Tooltip);
                }
                else {
                    /* code changes made coz in case of overloaded method , parameters needs to show to differentiate. (Bug 7598)*/
                    var titlevalue = ui.item.label;
                    if (ui.item.value && ui.item.value.Parameters && ui.item.value.Parameters.length > 0) {
                        titlevalue += "("
                        for (var i = 0; i < ui.item.value.Parameters.length; i++) {

                            titlevalue += ui.item.value.Parameters[i].ID;
                            if (ui.item.value.Parameters.length > 1) {
                                titlevalue += ",";
                            }

                        }
                        if (titlevalue.lastIndexOf(",") > -1) {
                            titlevalue = titlevalue.replace(/,\s*$/, "");
                        }
                        titlevalue += ")"
                    }
                    $(".ui-autocomplete > li").attr("title", titlevalue);
                }
                return false;
            },
            select: function (event, ui) {
                var arr = getSplitArray(this.value, this.selectionStart);
                var startPosition = this.selectionStart;
                var arrLen = 0;
                if (arr.length > 0) {
                    // this.value = this.value.substr(0, this.value.lastIndexOf(arr[arr.length - 1]));
                    arrLen = arr[arr.length - 1].length;
                }
                if (this.value == "") {
                    startPosition = 0;
                }
                var selectedtextlength = 0;

                if (ui.item.value[propertyName]) {
                    if (startPosition == 0) {
                        this.value = ui.item.value[propertyName];
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value[propertyName], this.value.slice(startPosition)].join('');
                        // this.value = this.value.substr(0, startPosition) + ui.item.value[propertyName] + this.value.substr(startPosition);
                        //this.value = this.value + ui.item.value[propertyName];
                    }
                    selectedtextlength = ui.item.value[propertyName].length;
                }
                else if (ui.item.value.ID) {
                    if (startPosition == 0) {
                        this.value = ui.item.value.ID;
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value.ID, this.value.slice(startPosition)].join('');
                        //this.value = this.value.substr(0, startPosition) + ui.item.value.ID + this.value.substr(startPosition);
                        //this.value = this.value + ui.item.value.ID;
                    }
                    selectedtextlength = ui.item.value.ID.length;
                }
                else {
                    this.value = this.value + ui.item.value;
                    selectedtextlength = ui.item.value.length;
                }
                this.value = this.value.trim();
                var prop = ui.item.value;
                if (prop.DataType == "TableObjectType" || prop.DataType == "CollctionType" || prop.DataType == "BusObjectType" || prop.DataType == "OtherReferenceType") {
                    if (!prop.HasLoadedProp || prop.HasLoadedProp == undefined) {
                        var txt = arr.join(".");
                        if (txt.indexOf("this.") == 0) {
                            txt = txt.substring(5);
                        }
                        $.connection.hubEntityModel.server.loadObjectTree(prop.ItemType.Name, prop.FullPath, true).done(function (data) {
                            scope.receiveObjectTree(data, prop.FullPath);
                        });
                    }
                }
                if (scope && scope.selectedobject != undefined) {
                    scope.selectedobject = prop;
                }
                $(this).trigger("change");
                //if (onSelectionCallback) {
                //    onSelectionCallback({ prop: prop });
                //}
                if (this.selectionEnd) {
                    this.selectionEnd = startPosition + selectedtextlength;
                }
                return false;
            },
            close: function () {
                controlSelection[0].focus();
                $(".page-header-fixed").css("pointer-events", "auto");
                //$(controlSelection).autocomplete("destroy");
            }
        });
    }
    else {
        if ($(controlSelection).data('ui-autocomplete')) {
            $(controlSelection).autocomplete("destroy");
            $(".page-header-fixed").css("pointer-events", "auto");
        }
    }
}

function generateUUID() {
    var guid;
    $.ajax({
        url: 'api/Login/GetGUID',
        type: 'GET',
        async: false,
        dataType: 'json',
        data: {},
        success: function (data) {
            guid = data;
        },
        error: function (response) {
            var objError = JSON.parse(response.responseText);
            if (objError && objError.Message) {
                var msg = objError.Message;
                if (objError.ExceptionMessage) {
                    msg = msg + "\n" + objError.ExceptionMessage;
                }
                MessageBox("Message", msg);
            }
            else {
                MessageBox("Message", "An error occured while creating a new GUID.");
            }
        }
    });

    return guid;
}

function onBodyClick(event) {
}

//#region Function for Creating New Objects
function GetFormattedTableName(astrTableName) {
    if (astrTableName.length > 4 &&
        astrTableName.toLowerCase().match("^sg") && astrTableName.substring(4, 3) == "_") {
        astrTableName = astrTableName.substring(4);
    }
    var strFinal = "";
    if (astrTableName == undefined || astrTableName == "") {
        return astrTableName;
    }
    var strSplit = astrTableName.split('_');
    angular.forEach(strSplit, function (strInclExcl) {
        if (strInclExcl == undefined || strInclExcl == "") {
            strFinal += strInclExcl;
        }
        else {
            strFinal += strInclExcl.substring(0, 1).toUpperCase();
            strFinal += strInclExcl.substring(1, strInclExcl.length).toLowerCase();
        }
    });

    return strFinal;
}


//#endregion

function onDragOverCommon(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Necessary. Allows us to drop.
    }

    e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
}
function isValidIdentifier(identifier, excludeNumber, includeKeywords, includeHyphen) {
    if (!identifier)
        return false;
    if (identifier.trim() == "") {
        return false;
    }


    if (!includeKeywords && globalKeywords && globalKeywords.length > 0 && globalKeywords.indexOf(identifier.toLowerCase()) > -1) {
        return false;
    }
    var regex;
    if (excludeNumber) {
        regex = new RegExp("^[_a-zA-Z][_a-zA-Z]*$");
    } else if (includeHyphen) {
        regex = new RegExp("^[-_a-zA-Z][-_a-zA-Z0-9]+$");
    } else {
        regex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
    }
    if (excludeNumber) {
        var alphaNumeric = new RegExp("^[a-zA-Z]*$");
    }
    else if (includeHyphen) {
        var alphaNumeric = new RegExp("^[-a-zA-Z0-9]*$");
    } else {
        var alphaNumeric = new RegExp("^[a-zA-Z0-9]*$");
    }

    if (regex.test(identifier)) {
        var tempString = identifier.replace(/_/g, "");
        if (tempString.trim() != "" && alphaNumeric.test(tempString)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}

function isValidFileID(identifier, IsForm) {
    if (!identifier)
        return false;
    if (identifier.trim() == "") {
        return false;
    }
    var regex = new RegExp("^[a-zA-Z][a-zA-Z0-9-]*$");
    if (IsForm) {
        regex = new RegExp("^[a-zA-Z][a-zA-Z]*$");
    }
    if (regex.test(identifier)) {
        return true;
    }
    else {
        return false;
    }
}


function getWebApplicationName() {
    var webAppName = window.location.pathname.substring(1);
    var index = webAppName.indexOf("/");
    if (index > -1) {
        webAppName = webAppName.substring(0, index);
    }
    return webAppName;
}

//#region getDescendents
function getDescendents(obj, name) {
    var elements = [];
    if (obj.Children != undefined) {
        if (obj.Children.length > 0) {
            for (var index = 0; index < obj.Children.length; index++) {
                if (!name || obj.Children[index].Name === name) {
                    elements.push(obj.Children[index]);
                }
                var childElements = getDescendents(obj.Children[index]);
                for (var i = 0; i < childElements.length; i++) {
                    if (!name || childElements[i].Name === name) {
                        elements.push(childElements[i]);
                    }
                }
            }
        }
    }

    if (obj.Elements != undefined) {
        if (obj.Elements.length > 0) {
            for (var index = 0; index < obj.Elements.length; index++) {
                if (!name || obj.Elements[index].Name === name) {
                    elements.push(obj.Elements[index]);
                }
                var childElements = getDescendents(obj.Elements[index]);
                for (var i = 0; i < childElements.length; i++) {
                    if (!name || childElements[i].Name === name) {
                        elements.push(childElements[i]);
                    }
                }
            }
        }
    }
    return elements;
}
//#endregion

//#region Function for New Rule, New Group
function GetNewStepName(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    while (CheckForDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, "(" + iItemNum.toString() + ")");
    }

    return strItemName;
}

function CheckForDuplicateID(strId, objRules) {
    var blnReturn = false;
    if (objRules) {
        blnReturn = objRules.dictAttributes.ID == strId;
        if (!blnReturn) {
            angular.forEach(objRules.Elements, function (item) {
                if (!blnReturn) {
                    blnReturn = CheckForDuplicateID(strId, item);
                    if (blnReturn) {
                        return;
                    }
                }
            });
        }
    }
    return blnReturn;
}

function GetNewQueryName(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    while (CheckForDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    }

    return strItemName;


}
function CheckInitialLoadQueryDuplicateID(strId, objRules) {
    var blnReturn = false;
    if (objRules) {
        if (objRules.dictAttributes && objRules.dictAttributes.ID) {
            blnReturn = objRules.dictAttributes.ID == strId;
        }

        if (!blnReturn && objRules && objRules.length > 0) {
            angular.forEach(objRules, function (item) {
                if (!blnReturn) {
                    blnReturn = CheckInitialLoadQueryDuplicateID(strId, item);
                    if (blnReturn) {
                        return;
                    }
                }
            });
        }
    }
    return blnReturn;
}

function GetInitialLoadQueryID(strItemKey, objRules, iItemNum) {

    var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    while (CheckInitialLoadQueryDuplicateID(strItemName, objRules)) {
        iItemNum++;
        strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
    }

    return strItemName;


}
//#endregion

function getHtmlFromServer(url) {
    var htmlText;
    var path = [window.location.protocol, "//", window.location.host, "/", getWebApplicationName(), "/", url].join("");

    var successCallBack = function (response) {
        htmlText = response;
    };
    var errorCallBack = function (response) {
        console.log(String.format("Invalid Url: {0}", path));
    };

    $.ajax({
        url: path,
        type: 'GET',
        async: false,
        success: successCallBack,
        error: errorCallBack
    });

    return htmlText;
}

function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}


function getEntityObject(entitylist, entityname, isfirstlevelexpanded) {
    var obj;
    var lst = entitylist.filter(function (x) { return x.ID == entityname; });
    if (lst && lst.length > 0) {
        obj = { ID: lst[0].ID, TableName: lst[0].TableName, BusinessObjectName: lst[0].BusinessObjectName, Attributes: [], IsExpanded: isfirstlevelexpanded, XmlMethods: [] };
        if (lst[0].Attributes && lst[0].Attributes.length > 0) {
            if (isfirstlevelexpanded) {
                createEntityAttributeList(lst[0].Attributes, obj);
                //createEntityXmlMethodList(lst[0].XmlMethods, obj);
            }
        }
    }

    return obj;
}

function createEntityAttributeList(attributes, parent) {
    attributes.sort(sort_by('ID', false, function (a) { return a.toUpperCase(); }));


    angular.forEach(attributes, function (field) {
        function iterateAttr(attr) {
            var val = field.Value.replace('_id', '_value');
            if (attr.Value == val) {
                blnFound = true;
            }
        }
        //in entity tree we do not want to show the attributes ending with _id so the check is added by neha
        if (endsWith(field.Value, "_id")) {
            var blnFound = false;
            angular.forEach(attributes, iterateAttr);
            if (!blnFound) {
                var objField = { ID: field.ID, Value: field.Value, DataType: field.DataType, Entity: field.Entity, Type: field.Type, Attributes: [], XmlMethods: [], objParent: parent };
                parent.Attributes.push(objField);
            }
        }
        else {
            var objField = { ID: field.ID, Value: field.Value, DataType: field.DataType, Entity: field.Entity, Type: field.Type, Attributes: [], XmlMethods: [], objParent: parent };
            parent.Attributes.push(objField);
        }
    });
    //if (parent.Attributes) {
    //    var count = 0;
    //    while (count != 2) {
    //        count++
    //        var tempattr = JSON.stringify(parent.Attributes);
    //        var data = JSON.parse(tempattr);
    //        parent.Attributes = parent.Attributes.concat(data);
    //    }
    //}

}

/*Use of sort_by
For Int field => arrayList.sort(sort_by('price', true, parseInt));
For String field => arrayList.sort(sort_by('ID', function (a) { return a.toUpperCase() }));
*/

function sort_by(field, reverse, primer) {
    var key = primer ?
        function (x) { return primer(x[field]); } :
        function (x) { return x[field]; };

    reverse = !reverse ? 1 : -1;

    return function (a, b) {
        return a = key(a), b = key(b), reverse * ((a > b) - (b > a));
    };
}

function createEntityXmlMethodItemList(items, parent) {
    angular.forEach(items, function (item) {
        var objItem = { ID: item.ID, ItemType: item.ItemType, LoadType: item.LoadType, LoadSource: item.LoadSource, SfwParameters: item.SfwParameters, objParent: parent };
        parent.Items.push(objItem);
    });
}

function getIntellisenseRecord(value, genericlist, propertyName) {
    //Get First 100 records from the list.
    if (genericlist) {
        data = genericlist;
        data = data.sort();
        var lstFilterData = [];
        if (propertyName != "ActiveForm" && propertyName != "VisibleRule" && propertyName != "FileIntellisense") {
            for (var i = 0; i < data.length; i++) {
                //if (data[i].DisplayMessageID.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                if (data[i][propertyName].toLowerCase().indexOf(value.toLowerCase()) > -1) {
                    lstFilterData.push(data[i]);
                }
            }
        }
        else {
            lstFilterData = data;
        }
        lstTop100Msgs = [];
        if (lstFilterData.length > 100) {
            for (var i = 0; i <= 99; i++) {
                lstTop100Msgs.push(lstFilterData[i]);
            }
        }
        else {
            lstTop100Msgs = lstFilterData;
        }
    }
    return lstTop100Msgs;
}

function SetParentForObjTreeChild(objtree) {
    function iterator(item) {
        item.objParent = objtree;
        item.FullPath = GetFullItemPathFromBusObjectTree(item);
        if (item.ChildProperties && item.ChildProperties.length > 0) {
            SetParentForObjTreeChild(item);
        }
    }

    if (objtree) {
        angular.forEach(objtree.ChildProperties, iterator);
        angular.forEach(objtree.lstMethods, iterator);
    }
}

function stopPropogation(event) {
    event.stopPropagation();
}

function GetBaseModel(data) {
    if (data) {
        var objNewObject = {
            prefix: data.prefix, Name: data.Name, Value: data.Value, dictAttributes: {}, Elements: [], Children: [], IsValueInCDATAFormat: data.IsValueInCDATAFormat, IsBaseAppModel: data.IsBaseAppModel, BaseAppModel: GetBaseModel(data.BaseAppModel)
        };


        for (var key in data.dictAttributes) {
            if (data.dictAttributes.hasOwnProperty(key)) {
                objNewObject.dictAttributes[key] = data.dictAttributes[key];
            }
        }


        angular.forEach(data.Elements, function (step) {
            if (step.Name && step.Name === "sfwGridView" && step.prototypemodel) {
                var objDataModel = GetPrototypeDataModel(step.prototypemodel);
                var IsDataElement = false;
                if (step.Elements.length > 1) {

                    for (var i = 0; i < step.Elements.length; i++) {
                        if (step.Elements[i].Name == "Data") {
                            IsDataElement = true;
                            step.Elements[i].Elements = objDataModel;
                            break;
                        }
                    }

                }
                if (!IsDataElement && objDataModel && objDataModel.length > 0) {
                    var objNewData = { dictAttributes: {}, Elements: [], Name: "Data", Value: "", prefix: "" };
                    objNewData.Elements = objDataModel;
                    step.Elements.push(objNewData);
                }
            }
            var model = GetBaseModel(step);
            objNewObject.Elements.push(model);
        });

        angular.forEach(data.Children, function (step) {
            var model = GetBaseModel(step);
            objNewObject.Children.push(model);
        });
    }


    return objNewObject;
}
function GetPrototypeDataModel(data) {
    if (data) {
        var objdataRowsModel = [];
        angular.forEach(data.Elements, function (dataRows) {
            if (dataRows.Elements) {
                var objRows = { dictAttributes: {}, Elements: [], Name: "DataRow", Value: "", prefix: "" };
                angular.forEach(dataRows.Elements, function (dataItem) {
                    if (dataItem.Elements) {
                        angular.forEach(dataItem.Elements, function (dataCol) {
                            objRows.Elements.push(dataCol);
                        });
                    }
                });
                objdataRowsModel.push(objRows);
            }
        });
        return objdataRowsModel;
    }
}
function getPropertyValue(obj, propname) {
    if (propname) {
        if (propname.contains('.')) {
            while (propname.contains('.')) {
                var strProp = propname.substring(0, propname.indexOf('.'));
                propname = propname.substring(propname.indexOf('.') + 1);
                if (obj && obj.hasOwnProperty(strProp)) {
                    obj = obj[strProp];
                }
            }
        }

        if (obj && obj.hasOwnProperty(propname)) {
            return obj[propname];
        }
    }
    return "";
}


function setPropertyValue(obj, propname, value) {
    if (propname && obj) {
        if (propname.contains('.')) {
            while (propname.contains('.')) {
                var strProp = propname.substring(0, propname.indexOf('.'));
                propname = propname.substring(propname.indexOf('.') + 1);
                if (obj.hasOwnProperty(strProp) && obj[strProp] !== null && typeof obj[strProp] === "object") {
                    obj = obj[strProp];
                }
            }
        }
        obj[propname] = value;
    }
}

function selectBuildResultErrors() {

    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    var lobjSpliter = $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[id="divbottompanel"]').show();
    if (lobjSpliter && lobjSpliter.refresh) {
        lobjSpliter.refresh();
    }
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").addClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").removeClass("selected");
}
function selectBuildResultWarnings() {
    $('div[id="divbottompanel"]').show();
    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").addClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").removeClass("selected");
    showFooter();
}
function selectBuildResultMessages() {
    $('div[id="divbottompanel"]').show();
    $("#main-wrapper").addClass("splitter_panel");
    $("#main-wrapper").addClass("spliter");
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});
    $('.spliter').width('100%').height('calc(100% - 80px)').split({
        orientation: 'horizontal', limit: 20
    });
    $('div[class="footer-slide-up"][id="dvBuildResults"] #errorList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #warningList').hide();
    $('div[class="footer-slide-up"][id="dvBuildResults"] #messageList').show();
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnError").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnWarning").removeClass("selected");
    $('div[class="footer-slide-up"][id="dvBuildResults"]').find("#btnMessage").addClass("selected");
}
function onClickBuildResultsDiv() {
    showBuildResults();
    toggleBuildResults(null, true);
}
function showBuildResults() {
    showFooter();
    $("div[class='footer-main-links'] #liBuildResults").show();
    selectBuildResultErrors();
    $('div[id="divBuildResultsbutton"]').hide();
}

function CloseBuildResultsButton(event) {
    $('div[id="divBuildResultsbutton"]').hide();
    if (event) {
        event.stopPropagation();
    }
}

function showFooter() {
    $(".footer").show();
}

function toggleBuildResults(event, showOnly) {
    onBodyClick();
    if ($('div[class="footer-slide-up"][id="dvBuildResults"]').is(':visible')) {
        if (!showOnly) {
            $(".footer-main-links #liBuildResults a").removeClass("selected");
            $('div[class="footer-slide-up"][id="dvBuildResults"]').hide("slide", {
                direction: "down"
            });
        }
    }
    else {
        $(".footer-main-links #liBuildResults a").addClass("selected");
        $('div[class="footer-slide-up"][id="dvBuildResults"]').show("slide", {
            direction: "down"
        });
        hideOtherFooters('BuildResults');
    }
    if (event) {
        event.stopPropagation();
    }
}

function hideBuildResults() {
    closeBuildResults();
    $("div[class='footer-main-links'] #liBuildResults").hide();
    if ($("div[class='footer-main-links'] ul").find("li:visible").length == 0) {
        hideFooter();
    }
    $('div[id="divBuildResultsbutton"]').show();
}

function closeBuildResults() {
    if ($('div[class="footer-slide-up"][id="dvBuildResults"]').is(':visible')) {
        $(".footer-main-links #liBuildResults a").removeClass("selected");
        $('div[class="footer-slide-up"][id="dvBuildResults"]').hide("slide", {
            direction: "down"
        });
    }
    $("#main-wrapper").removeClass("splitter_panel");
    $("#main-wrapper").removeClass("spliter");
    $('div[id="divbottompanel"]').hide();
    $('#main-wrapper .a').height('100%');
    //$('.spliter').width('100%').height('100vh').split({
    //    orientation: 'horizontal', limit: 20
    //});

}

function hideFooter() {
    $(".footer").hide();
}

function hideOtherFooters(currentTab) {
    if (currentTab != "BuildResults") {
        closeBuildResults();
    }
    if (currentTab != "FindResults") {
        closeFindResults();
    }
}

function closeFindResults() {
    if ($('div[class="footer-slide-up"][id="dvFindResults"]').is(':visible')) {
        $(".footer-main-links #liFindResults a").removeClass("selected");
        $('div[class="footer-slide-up"][id="dvFindResults"]').hide("slide", {
            direction: "down"
        });
    }
}


(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof module === 'object' && module.exports) {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function ($) {
    'use strict';
    var caretClass = 'textarea-helper-caret'
        , dataKey = 'textarea-helper'

        // Styles that could influence size of the mirrored element.
        , mirrorStyles = [
            // Box Styles.
            'box-sizing', 'height', 'width', 'padding-bottom'
            , 'padding-left', 'padding-right', 'padding-top'

            // Font stuff.
            , 'font-family', 'font-size', 'font-style'
            , 'font-variant', 'font-weight'

            // Spacing etc.
            , 'word-spacing', 'letter-spacing', 'line-height'
            , 'text-decoration', 'text-indent', 'text-transform'

            // The direction.
            , 'direction'
        ];

    var TextareaHelper = function (elem) {
        if (elem.nodeName.toLowerCase() !== 'textarea' && elem.nodeName.toLowerCase() !== 'input') return;
        this.$text = $(elem);
        this.$mirror = $('<div/>').css({
            'position': 'absolute'
            , 'overflow': 'auto'
            , 'white-space': 'pre-wrap'
            , 'word-wrap': 'break-word'
            , 'top': 0
            , 'left': -9999
        }).insertAfter(this.$text);
    };

    (function () {
        this.update = function () {

            // Copy styles.
            var styles = {};
            for (var i = 0, style; style = mirrorStyles[i]; i++) {
                styles[style] = this.$text.css(style);
            }
            this.$mirror.css(styles).empty();

            // Update content and insert caret.
            var caretPos = this.getOriginalCaretPos()
                , str = this.$text.val()
                , pre = document.createTextNode(str.substring(0, caretPos))
                , post = document.createTextNode(str.substring(caretPos))
                , $car = $('<span/>').addClass(caretClass).css('position', 'absolute').html('&nbsp;');
            this.$mirror.append(pre, $car, post)
                .scrollTop(this.$text.scrollTop());
        };

        this.destroy = function () {
            this.$mirror.remove();
            this.$text.removeData(dataKey);
            return null;
        };

        this.caretPos = function () {
            this.update();
            var $caret = this.$mirror.find('.' + caretClass)
                , pos = $caret.position();
            if (this.$text.css('direction') === 'rtl') {
                pos.right = this.$mirror.innerWidth() - pos.left - $caret.width();
                pos.left = 'auto';
            }

            return pos;
        };
        this.caretPosAbs = function () {
            var absPos = this.$text[0].getBoundingClientRect();
            var leftBorderWidth = this.$text.css("border-left-width");
            var topBorderWidth = this.$text.css("border-top-width");
            var leftPadding = this.$text.css("padding-left");
            var topPadding = this.$text.css("padding-top");
            var carPos = this.caretPos();

            if (!absPos) {
                absPos = { left: 0, top: 0 };
            }
            else {
                if (!absPos.left) {
                    absPos.left = 0;
                }
                if (!absPos.top) {
                    absPos.top = 0;
                }
            }

            if (!leftBorderWidth) {
                leftBorderWidth = 0;
            }
            if (!topBorderWidth) {
                topBorderWidth = 0;
            }
            if (!leftPadding) {
                leftPadding = 0;
            }
            if (!topPadding) {
                topPadding = 0;
            }

            var actPos = {
                left: parseFloat(absPos.left) + parseFloat(leftBorderWidth) + parseFloat(leftPadding) + parseFloat(carPos.left),
                top: parseFloat(absPos.top) + parseFloat(topBorderWidth) + parseFloat(topPadding) + parseFloat(carPos.top) + 15
            };
            return actPos;
        };
        this.height = function () {
            this.update();
            this.$mirror.css('height', '');
            return this.$mirror.height();
        };
        // XBrowser caret position
        // Adapted from http://stackoverflow.com/questions/263743/how-to-get-caret-position-in-textarea
        this.getOriginalCaretPos = function () {
            var text = this.$text[0];
            if (text.selectionStart) {
                return text.selectionStart;
            } else if (document.selection) {
                text.focus();
                var r = document.selection.createRange();
                if (r == null) {
                    return 0;
                }
                var re = text.createTextRange()
                    , rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);
                return rc.text.length;
            }
            return 0;
        };

    }).call(TextareaHelper.prototype);

    $.fn.textareaHelper = function (method) {
        this.each(function () {
            var $this = $(this)
                , instance = $this.data(dataKey);
            if (!instance) {
                instance = new TextareaHelper(this);
                $this.data(dataKey, instance);
            }
        });
        if (method) {
            var instance = this.first().data(dataKey);
            return instance[method]();
        } else {
            return this;
        }
    };

}));

$.fn.scrollTo = function (target, options, callback) {
    if (typeof options == 'function' && arguments.length == 2) { callback = options; options = target; }
    var settings = $.extend({
        scrollTarget: target,
        offsetTop: 100,
        offsetLeft: 100,
        duration: 300,
        easing: 'swing'
    }, options);
    return this.each(function () {
        var scrollPane = $(this);
        var scrollTarget = (typeof settings.scrollTarget == "number") ? settings.scrollTarget : $(settings.scrollTarget);
        var scrollY = 0;
        if ((typeof scrollTarget == "number")) {
            scrollY = scrollTarget;
        }
        else {
            if (scrollTarget.offset()) {
                scrollY = scrollTarget.offset().top + scrollPane.scrollTop() - parseInt(settings.offsetTop);
            }
            else {
                scrollY = scrollPane.scrollTop() - parseInt(settings.offsetTop);
            }
        }
        if (scrollY < scrollPane[0].scrollHeight) {
            scrollY -= 150;
        }

        scrollPane.animate({ scrollTop: scrollY }, parseInt(settings.duration), settings.easing, function () {
            if (typeof callback == 'function') { callback.call(this); }
        });
        var scrollX = 0;
        if (settings.offsetLeft > 0) {
            if ((typeof scrollTarget == "number")) {
                scrollX = scrollTarget;
            }
            else {
                if (scrollTarget.offset()) {
                    scrollX = scrollTarget.offset().left + scrollPane.scrollLeft() - parseInt(settings.offsetLeft);
                }
                else {
                    scrollX = scrollPane.scrollLeft() - parseInt(settings.offsetLeft);
                }
            }
        }

        if (scrollX < scrollPane[0].scrollWidth) {
            scrollX -= 150;
        }

        scrollPane.animate({ scrollLeft: scrollX }, parseInt(settings.duration), settings.easing, function () {
            if (typeof callback == 'function') { callback.call(this); }
        });

    });
};

function getArgumentDataTypePrefix(datatype) {
    var prefiex = "";
    if (datatype && datatype.trim() != "") {
        switch (datatype) {
            case "bool":
            case "boolean":
            case "Boolean":
                prefiex = "abln";
                break;
            case "datetime":
            case "DateTime":
                prefiex = "adt";
                break;
            case "decimal":
            case "Decimal":
                prefiex = "adcml";
                break;
            case "double":
            case "Double":
                prefiex = "adbl";
                break;
            case "float":
            case "Float":
                prefiex = "aflt";
                break;
            case "int":
            case "Int32":
            case "long":
            case "Int64":
            case "short":
            case "Int16":
                prefiex = "aint";
                break;
            case "string":
            case "String":
                prefiex = "astr";
                break;
            case "Collection":
                prefiex = "aclb";
                break;
            case "CDOCollection":
                prefiex = "aclb";
                break;
            case "Object":
                prefiex = "aobj";
                break;
            case "List":
                prefiex = "alst";
                break;
        }
    }
    return prefiex;
}
function GetDataTypePrefix(datatype) {
    var prefiex="";
    if (datatype) {
        switch (datatype) {
            case "bool":
                prefiex = "ibln";
                break;
            case "datetime":
                prefiex = "idt";
                break;
            case "decimal":
                prefiex = "idec";
                break;
            case "double":
                prefiex = "idbl";
                break;
            case "float":
                prefiex = "iflt";
                break;
            case "int":
                prefiex = "iint";
                break;
            case "long":
                prefiex = "ilong";
                break;
            case "short":
                prefiex = "ishrt";
                break;
            case "string":
                prefiex = "istr";
                break;
            case "Collection":
                prefiex = "iclb";
                break;
            case "CDOCollection":
                prefiex = "iclb";
                break;
            case "Object":
                prefiex = "obj";
                break;
            case "List":
                prefiex = "lst";
                break;
        }
    }
    return prefiex;
}

function makeStringToCamelCase(astrFieldName) {

    var strCtrlID = "";
    var strSep = "~`!@#$%^&*_-+=[{()}]|:;<,.>?/.";

    var blnCapsNext = true;
    for (var i = 0; i < astrFieldName.length; i++) {
        if (strSep.contains("" + astrFieldName[i]))
            blnCapsNext = true;
        else {
            strCtrlID += blnCapsNext ? astrFieldName.toUpperCase()[i] : astrFieldName[i];
            if ("" + astrFieldName[i] == " ") {
                blnCapsNext = true;
            }
            else {
                blnCapsNext = false;
            }
        }
    }

    if (strCtrlID.toLowerCase().match("^iclb") || strCtrlID.toLowerCase().match("^icol") || strCtrlID.toLowerCase().match("^ibus"))
        strCtrlID = strCtrlID.substring(4);

    return strCtrlID;
}




function codevaluesIDTextChanged(event) {
    var input = $(event.target);
    var scope = angular.element(event.target).scope();
    if (!scope.codeid) {
        scope.codevalueslist = [];
    }
    if (!scope.codevalueslist) {
        $.connection.hubMain.server.getCodeValues("ScopeId_" + scope.$id, scope.codeid);
    }
    if (scope.codevalueslist && scope.codevalueslist.length > 0) {
        var codeValuesList = [];
        var value = input.val();
        var data = scope.codevalueslist.sort();
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val());
            event.preventDefault();
        } else {
            if (data && data.length > 100) {
                for (var i = 0; i < data.length; i++) {
                    if (codeValuesList.length < 100) {
                        if (data[i].CodeValueDescription.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                            codeValuesList.push(data[i]);
                        }
                    } else {
                        break;
                    }
                }
            } else {
                codeValuesList = data;
            }
            setSingleLevelAutoComplete(input, codeValuesList, scope, "CodeValue", "CodeValueDescription");
        }
    }
    else {
        setSingleLevelAutoComplete(input, [], scope, "CodeValue", "CodeValueDescription");
    }
}

//Design-Source Common Function
function getLineNumber(txtarea) {
    var LineNumber; //Current Line number
    var lines; //Total number of lines
    var line;  //Selected Line text content
    var lineno = 1;

    LineNumber = txtarea.val().substr(0, txtarea[0].selectionStart).split("\n").length;
    lines = txtarea.val().split('\n');
    line = lines[LineNumber - 1];

    if (line && ((line.trim().contains("<") && !line.trim().startsWith("<") && !line.trim().contains("</")) || (line.trim().contains("</") && line.trim().endsWith(">")) || (line.trim().contains("></") && line.trim().contains("<") && line[line.trim().indexOf("<") + 1] != "/"))) {
        lineno = LineNumber;
    }
    else if (line && ((!line.trim().startsWith("<") || line.trim().startsWith("</") || !line.trim().startsWith("/") || !line.trim().contains("<")))) {
        while (line != undefined) {
            line = lines[LineNumber - 1];
            LineNumber--;
            if (line && line.trim().startsWith("<")) {
                LineNumber++;
                lineno = LineNumber;
                break;
            }
        }
    }

    return lineno;
}

function scrollTextArea(txtArea, line, text) {
    var noOfRows = txtArea[0].value.split("\n");
    var lineHeight = txtArea[0].scrollHeight / noOfRows.length;
    txtArea[0].scrollTop = (line - 1) * lineHeight;

    // for scroll left
    if (line && text) {
        var lineWidth = txtArea[0].scrollWidth / 3;
        var noCharParsed = noOfRows[line - 1].indexOf(text);
        txtArea[0].scrollLeft = (noCharParsed * 4.4) + text.length;
    }
}

function selectTextareaLine(txtArea, lineNum) {
    lineNum--; // array starts at 0
    var lines = txtArea[0].value.split("\n");

    // calculate start/end
    var startPos = 0, endPos = txtArea[0].value.length;
    for (var x = 0; x < lines.length; x++) {
        if (x == lineNum) {
            break;
        }
        startPos += (lines[x].length + 1);
    }

    var endPos = lines[lineNum].length + startPos;

    if (typeof (txtArea[0].selectionStart) != undefined) {
        txtArea[0].selectionStart = startPos;
        txtArea.focus();
        txtArea[0].selectionEnd = endPos;
        //return true;
    }
    // return false;
}

var FindDeepNode = function (objParentElements, selectedItem) {
    if (objParentElements) {
        angular.forEach(objParentElements.Elements, function (item) {
            item.ParentVM = objParentElements;
            if (item == selectedItem) {
                return selectedItem;
            }
            else if (item.Elements && item.Elements.length > 0) {
                selectedItem = FindDeepNode(item, selectedItem);
                return selectedItem;
            }
            else if (item.Children && item.Children.length > 0) {
                selectedChildItem = FindDeepNodeChildren(item, selectedItem);
                if (selectedChildItem == selectedItem) {
                    return selectedItem;
                }
            }
        });
    }
    return selectedItem;
};

var FindDeepNodeChildren = function (item, selectedItem) {
    angular.forEach(item.Children, function (obj) {
        obj.ParentVM = item;
        if (obj == selectedItem) {
            return obj;
        }
    });
};

var getPathSource = function (sObj, indexPath) {
    while (sObj.ParentVM) {
        if (sObj.ParentVM.Elements.length > 0) {
            indexPath.push(sObj.ParentVM.Elements.indexOf(sObj));
        }
        else if (sObj.ParentVM.Children.length > 0) {
            indexPath.push(sObj.ParentVM.Children.indexOf(sObj));
        }
        sObj = sObj.ParentVM;
    }
    return indexPath;
};

var FindNodeHierarchy = function (objParentElements, index) {
    if (objParentElements && objParentElements.Elements) {
        var newObj = objParentElements.Elements[index];
        if (newObj == undefined) {
            newObj = objParentElements.Elements[index - 1];
        }
        if (newObj) {
            newObj.ParentVM = objParentElements;
        }
        return newObj;
    }
};

var FindChidrenHierarchy = function (objParentElements, index) {
    return objParentElements.Children[index];
};

var sourceChanged = function () {
    var scope = getCurrentFileScope();
    if (scope && scope.sourceChanged) {
        scope.sourceChanged();
    }
};

function setEntityQueryIntellisense(controlSelection, data, scope, propertyName) {
    if (window.event && (window.event.keyCode === 38 || window.event.keyCode === 40)) { // when pressed up or down key    
        return;
    }
    if (propertyName == "") propertyName = undefined;
    if (data && data.length > 0) {
        if (typeof data[0].ID == "undefined") data = data.sort();
        else {
            if (typeof propertyName != "undefined") {
                data = data.sort(function (a, b) {
                    var nameA = a[propertyName].toLowerCase(), nameB = b[propertyName].toLowerCase();
                    //sort string ascending
                    if (nameA < nameB) return -1;
                    if (nameA > nameB) return 1;
                    return 0; //default return value (no sorting)
                });
            }
            else {
                data = data.sort(function (a, b) {
                    if (a.ID && b.ID) {
                        var nameA = a.ID.toLowerCase(), nameB = b.ID.toLowerCase();
                        //sort string ascending
                        if (nameA < nameB) return -1;
                        if (nameA > nameB) return 1;
                    }
                    return 0; //default return value (no sorting)
                });
            }
        }
        controlSelection.autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event) {
                if (controlSelection[0].localName == "textarea" || (controlSelection[0].localName == "input" && controlSelection.attr("type") == "text")) {
                    var pos = controlSelection.textareaHelper('caretPosAbs');
                    $("#dvIntellisense > ul").css({
                        left: (pos.left) + "px",
                        //top: (pos.top) + "px",
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px",
                        "z-index": "999999999",
                    });
                    setIntellisensePosition(event);
                }
                else if (controlSelection[0].localName == "span") {
                    var distanceToTop = this.getBoundingClientRect().top;
                    var distanceToleft = this.getBoundingClientRect().left;
                    var currentSpanWidth = this.getBoundingClientRect().width;
                    var currentSpanHeight = this.getBoundingClientRect().height;
                    var windowsHeight = $(window).height();
                    if (distanceToTop + 300 > windowsHeight) {
                        distanceToTop -= 300;
                    }
                    else {
                        distanceToTop += currentSpanHeight;
                    }
                    $("#dvIntellisense > ul").css({
                        left: (distanceToleft) + "px",
                        top: (distanceToTop) + "px",
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px"
                    });
                    var vrCurrentPopupHeight;
                    for (var i = 0; i < $("#dvIntellisense > ul").length; i++) {
                        if ($("#dvIntellisense > ul")[i].offsetHeight != 0) {
                            var vrCurrentPopupHeight = $("#dvIntellisense > ul")[i].offsetHeight;
                        }
                    }
                    if ((vrCurrentPopupHeight < 300) && (distanceToTop < this.getBoundingClientRect().top)) {
                        if ((this.getBoundingClientRect().top + this.getBoundingClientRect().height + vrCurrentPopupHeight) > windowsHeight) {
                            distanceToTop = this.getBoundingClientRect().top - vrCurrentPopupHeight;
                        }
                        else {
                            distanceToTop = this.getBoundingClientRect().top + this.getBoundingClientRect().height;
                        }
                        $("#dvIntellisense > ul").css({
                            left: (distanceToleft) + "px",
                            top: (distanceToTop) + "px",
                            width: 'auto',
                            height: 'auto',
                            overflow: 'auto',
                            maxWidth: '300px',
                            maxHeight: "300px"
                        });
                    }
                }
                if ($(controlSelection).data('ui-autocomplete')) {
                    $(".page-header-fixed").css("pointer-events", "none");
                    $("#dvIntellisense").css("pointer-events", "auto");
                }
            },
            source: function (request, response) {
                var result = $.ui.autocomplete.filter(
                    $.map(data, function (value, key) {
                        if (typeof value.ID == "undefined") {
                            return {
                                label: value,
                                value: value
                            };
                        }
                        else {
                            if (typeof propertyName != "undefined" && value.hasOwnProperty(propertyName)) {
                                return {
                                    label: value[propertyName],
                                    value: value,
                                };
                            }
                            else {
                                return {
                                    label: value.ID,
                                    value: value,
                                };
                            }
                        }
                    }), extractLast(request.term, this.element[0].selectionStart));
                response(result.slice(0, 100));
            },
            focus: function (event, ui) {
                // prevent value inserted on focus
                if (ui.item.value && ui.item.value.hasOwnProperty("Tooltip") && ui.item.value.Tooltip) {
                    $(".ui-autocomplete > li").attr("title", ui.item.value.Tooltip);
                } else {
                    $(".ui-autocomplete > li").attr("title", ui.item.label);
                }
                return false;
            },
            select: function (event, ui) {
                var arr;
                var caretindex = 0;
                var startPosition = 0;
                var arrLen = 0;
                if (event.target.localName == "input") {
                    arr = getSplitArray(this.value, this.selectionStart);
                    startPosition = this.selectionStart;
                }
                else if (event.target.localName == "span") {
                    caretindex = caretPosition(this);
                    arr = getSplitArray(this.innerText, caretindex);
                    startPosition = caretindex;
                }

                if (arr.length > 0) {
                    if (event.target.localName == "input") {
                        //this.value = this.value.substr(0, this.value.lastIndexOf(arr[arr.length - 1]));
                        arrLen = arr[arr.length - 1].length;
                    }
                    else if (event.target.localName == "span") {
                        var index = caretindex - arr[arr.length - 1].length;
                        if (arr[arr.length - 1].length > 0) {
                            caretindex = index;
                        }
                        var splittedarray = arr[arr.length - 1].split('');
                        var totallength = index + splittedarray.length;
                        for (var i = index; i < totallength; i++) {
                            this.innerText = this.innerText.slice(0, index) + this.innerText.slice(index + 1, this.innerText.length);
                        }
                        //this.innerText = this.innerText.substr(0, this.innerText.lastIndexOf(arr[arr.length - 1]))
                    }
                }
                if (!(this.value)) {
                    startPosition = 0;
                }
                if (typeof propertyName != "undefined" && ui.item.value[propertyName]) {
                    //this.value = this.value + ui.item.value[propertyName];
                    if (startPosition == 0) {
                        this.value = ui.item.value[propertyName];
                    } else {
                        this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value[propertyName], this.value.slice(startPosition)].join('');
                    }
                }
                else if (ui.item.value.ID) {
                    if (event.target.localName == "input") {
                        // this.value = this.value + ui.item.value.ID;
                        if (startPosition == 0) {
                            this.value = ui.item.value.ID;
                        } else {
                            this.value = [this.value.slice(0, (startPosition - arrLen)), ui.item.value.ID, this.value.slice(startPosition)].join('');
                        }
                    }
                    else if (event.target.localName == "span") {
                        this.innerText = [this.innerText.slice(0, caretindex), ui.item.value.ID, this.innerText.slice(caretindex)].join('');
                    }
                    if (scope && scope.items && scope.items.Name && (scope.items.Name == "calllogicalrule" || scope.items.Name == "calldecisiontable" || scope.items.Name == "callexcelmatrix" || scope.items.Name == "method" || scope.items.Name == "query")) {
                        var parameters = scope.items.Elements.filter(function (x) { return x.Name == "parameters"; });
                        if (ui.item.value.RuleType || ui.item.value.QueryType || ui.item.value.ReturnType) {
                            if (ui.item.value.Parameters) {
                                var parametersModel = null;
                                if (parameters.length > 0) {
                                    parametersModel = parameters[0];
                                }
                                else {
                                    parametersModel = {
                                        Name: "parameters", Value: "", dictAttributes: {}, Elements: [], Children: []
                                    };
                                }
                                scope.$root.UndRedoBulkOp("Start");
                                //Removing unwanted parameters.
                                if (parametersModel.Elements.length > 0) {
                                    scope.$apply(function () {
                                        parametersModel.Elements = parametersModel.Elements.filter(function (x) { return ui.item.value.Parameters.some(function (element) { return element.ID == x.dictAttributes.ID; }); });
                                    });
                                }
                                //Adding or updating existing parameters
                                for (var index in ui.item.value.Parameters) {
                                    var parameterModel = parametersModel.Elements.filter(function (element) { return element.dictAttributes.ID == ui.item.value.Parameters[index].ID; });
                                    if (parameterModel.length > 0) {

                                        scope.$apply(function () {
                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwDataType, parameterModel[0].dictAttributes, "sfwDataType", ui.item.value.Parameters[index].DataType);

                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwDirection, parameterModel[0].dictAttributes, "sfwDirection", ui.item.value.Parameters[index].Direction);

                                            scope.$root.EditPropertyValue(parameterModel[0].dictAttributes.sfwEntity, parameterModel[0].dictAttributes, "sfwEntity", ui.item.value.Parameters[index].Entity);
                                        });
                                    }
                                    else {
                                        scope.$apply(function () {
                                            var newparam = { Name: "parameter", Value: "", dictAttributes: { ID: ui.item.value.Parameters[index].ID, sfwDataType: ui.item.value.Parameters[index].DataType, sfwDirection: ui.item.value.Parameters[index].Direction, sfwEntity: ui.item.value.Parameters[index].Entity, sfwNodeID: generateUUID() }, Elements: [], Children: [] };
                                            scope.$root.PushItem(newparam, parametersModel.Elements);
                                        });
                                    }
                                }

                                if (parameters.length == 0) {
                                    scope.$apply(function () {
                                        scope.$root.PushItem(parametersModel, scope.items.Elements);
                                    });
                                }
                                scope.$root.UndRedoBulkOp("End");
                            }
                        }
                        else {
                            if (parameters.length > 0) {
                                scope.$apply(function () {
                                    scope.$root.DeleteItem(scope.items.Elements[scope.items.Elements.indexOf(parameters[0])], scope.items.Elements);
                                });
                            }
                        }
                    }
                } else {
                    this.value = this.value + ui.item.value;
                    //this.innerText = this.value + ui.item.value;
                }
                var scope1 = angular.element($(this)).scope();
                if (scope1) scope1.$broadcast("UpdateOnClick", controlSelection[0]);
                if (this.childNodes[0] != undefined) {
                    var doc = this.ownerDocument || this.document;
                    var win = doc.defaultView || doc.parentWindow;
                    var sel;

                    var position = caretindex + ui.item.value.ID.length;
                    var range = document.createRange();
                    range.setStart(this.childNodes[0], position);
                    range.collapse(true);
                    if (typeof win.getSelection != "undefined") {
                        sel = win.getSelection();
                        sel.removeAllRanges();
                        sel.addRange(range);
                    } else if ((sel = doc.selection) && sel.type != "Control") {
                        sel.removeAllRanges();
                        sel.addRange(range);
                    }
                }
                else $(this).trigger("change");
                return false;
            },
            close: function () {
                $(".page-header-fixed").css("pointer-events", "auto");
            }
        });
    }
    else {
        if ($(controlSelection).data('ui-autocomplete')) {
            $(controlSelection).autocomplete("destroy");
        }
    }
}

function preventDefaultSaveUndoRedo(e) {
    if (e.ctrlKey && (e.keyCode == 90 || e.keyCode == 89 || e.keyCode == 83)) {
        e.preventDefault();
    }
}

function getDataType(objectDataType, dataTypeName) {
    var retVal = "";
    if (objectDataType == "CollctionType") {
        retVal = "Collection";
    }
    else if (objectDataType == "List") {
        retVal = "List";
    }
    else if (objectDataType == "BusObjectType" || objectDataType == "OtherReferenceType" || objectDataType == "TableObjectType") {
        retVal = "Object";
    }
    else
        if (dataTypeName != undefined && dataTypeName != "" && dataTypeName != null) {
            if (dataTypeName.toLowerCase() == "int32") {
                retVal = "int";
            }
            else if (dataTypeName.toLowerCase() == "int16") {
                retVal = "short";
            }
            else if (dataTypeName.toLowerCase() == "int64") {
                retVal = "long";
            }
            else if (dataTypeName.toLowerCase() == "single") {
                retVal = "float";
            }
            else if (dataTypeName.toLowerCase() == "boolean") {
                retVal = "bool";
            }
            else {
                retVal = dataTypeName.toLowerCase();
            }
        }
    return retVal;
}

function setIntellisensePosition(event) {
    var $input = $(event.target);
    if ($input.data('ui-autocomplete')) {
        var $results = $input.autocomplete("widget");
        //top = $results.position().top,
        left = $results.position().left,
            windowWidth = $(window).width(),
            height = $results.height(),
            inputHeight = $input.height(),
            windowsHeight = $(window).height();
        if (windowsHeight < $results.position().top + height + inputHeight) {
            newTop = $results.position().top - height - inputHeight - 10;
            $results.css("top", newTop + "px");
        }
        var width = $results.width();
        if (left + width > windowWidth) {
            var diff = (left + width) - windowWidth;
            newleft = left - (diff + 15);
            $results.css("left", newleft + "px");
        }
    }
    else {
        event.stopPropagation();
    }
}


function sortNumbersBasedOnproperty(lstobj, text, displaypropertyname) {
    lstobj = lstobj.sort(function (a, b) {
        var nameA;
        var nameB;
        if (a && b) {
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname];
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName];
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName];
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID;
            }
            else if (a) {
                nameA = a;
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname];
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName];
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName];
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID;
            }
            else if (b) {
                nameB = b;
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
        }
        return 0; //default return value (no sorting)
    });

}

function sortListBasedOnproperty(lstobj, text, displaypropertyname) {
    lstobj = lstobj.sort(function (a, b) {
        var nameA;
        var nameB;
        if (a && b) {
            if (typeof displaypropertyname != "undefined" && a.hasOwnProperty(displaypropertyname)) {
                nameA = a[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.hasOwnProperty(propertyName)) {
                nameA = a[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && a.dictAttributes && a.dictAttributes.hasOwnProperty(propertyName)) {
                nameA = a.dictAttributes[propertyName].toLowerCase();
            }
            else if (a.hasOwnProperty("ID")) {
                nameA = a.ID.toLowerCase();
            }
            else if (a.toLowerCase) {
                nameA = a.toLowerCase();
            }

            if (typeof displaypropertyname != "undefined" && b.hasOwnProperty(displaypropertyname)) {
                nameB = b[displaypropertyname].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.hasOwnProperty(propertyName)) {
                nameB = b[propertyName].toLowerCase();
            }
            else if (typeof propertyName != "undefined" && b.dictAttributes && b.dictAttributes.hasOwnProperty(propertyName)) {
                nameB = b.dictAttributes[propertyName].toLowerCase();
            }
            else if (b.hasOwnProperty("ID")) {
                nameB = b.ID.toLowerCase();
            }
            else if (b.toLowerCase) {
                nameB = b.toLowerCase();
            }

            if (nameA && nameB) {
                if (nameA < nameB) //sort string ascending
                    return -1;
                if (nameA > nameB)
                    return 1;
            }
        }
        return 0; //default return value (no sorting)
    });

}

function getRemovedStringStartingSpace(strValue) {
    var strNewValue = "";
    var arrValue = strValue.split(" ");
    for (var i = 0; i < arrValue.length; i++) {
        if (arrValue[i].trim() == "") {
            arrValue.splice(i, 1);
            i--;
        } else {
            break;
        }
    }
    var strNewValue = arrValue.join('');
    return strNewValue;
}

var lstEntityTreeFieldData = null;
function onEntityTreeDragStart(e) {
    dragDropData = null;
    dragDropDataObject = null;
    //e.dataTransfer.effectAllowed = 'move';
    var ID = $(e.currentTarget).attr("drag-id");
    var DisplayName = $(e.currentTarget).attr("drag-display-name");
    var Datatype = $(e.currentTarget).attr("drag-datatype");
    var dragObject = $(e.currentTarget).attr("drag-object");
    var dragfieldtype = $(e.currentTarget).attr("drag-fieldtype");
    dragObject = JSON.parse(dragObject);
    var isparentTypeCollection = $(e.currentTarget).attr("drag-boolparenttype");
    var lookupfieldQueryId = $(e.currentTarget).attr("drag-fieldquery");
    var obj = [ID, DisplayName, Datatype, dragObject, isparentTypeCollection, dragfieldtype, lookupfieldQueryId];
    lstEntityTreeFieldData = obj;
    var strobj = JSON.stringify(obj);
    e.dataTransfer.setData("Text", "");
}

function onEntityFieldDragOver(e) {
    if (e) {
        var scp = angular.element($(e.target)).scope();
        if (e.preventDefault) {
            e.preventDefault();
        }
        if (e.stopPropagation) {
            e.stopPropagation();
        }
        if (scp && scp.candrop != undefined && scp.candrop == false) {
            return false;
        }
    }
}

function SetCodeIDDescriptionToList(lst) {
    if (lst && lst.length > 0) {
        for (var i = 0; i < lst.length; i++) {
            lst[i].CodeIDDescription = lst[i].CodeID + " - " + lst[i].Description;
        }
    }
}

function onEntityFieldDropInDirective(e) {
    var strData = e.dataTransfer.getData("Text");
    if (e.stopPropagation) {
        e.stopPropagation();
    }
    e.preventDefault();
    if (strData == "" && lstEntityTreeFieldData != null) {
        var obj = lstEntityTreeFieldData;//JSON.parse(strData);
        var Id = obj[0];
        var DisplayName = obj[1];
        var DataType = obj[2];
        var scp = angular.element($(e.target)).scope();
        if (scp && (scp.candrop == undefined || scp.candrop)) {
            var entityFullPath = "";
            if (DisplayName != "") {
                entityFullPath = DisplayName + "." + Id;
            } else {
                entityFullPath = Id;
            }
            if (obj[3] && obj[3].Type == "Description") {
                DataType = "Description";
            }
            if (DataType && DataType != "Object" && DataType != "Collection" && DataType != "List" && DataType != "CDOCollection" && scp.model && scp.model.Name != "sfwTableBookMark" && scp.model.Name != "sfwChildTemplateBookmark") {
                if (scp.model) {
                    scp.$evalAsync(function () {
                        if (scp.model.dictAttributes) {
                            if (DataType != "Description") {
                                if (scp.setcolumndatatype) {

                                    scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwColumnDataType, scp.model.dictAttributes, "sfwColumnDataType", DataType);
                                }
                                else {
                                    scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwDataType, scp.model.dictAttributes, "sfwDataType", DataType);
                                }
                            }
                            scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                        } else {
                            scp.$root.EditPropertyValue(scp.model.TrackingID, scp.model, "TrackingID", entityFullPath);
                        }

                    });
                }
            }
            else if (scp.model && scp.model.Name == "sfwChildTemplateBookmark") {
                if (scp.model.dictAttributes && scp.model.dictAttributes.sfwChildTemplateType == "Collection") {
                    if (DataType == "Collection" || DataType == "CDOCollection" || DataType == "List") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    }
                    else {
                        MessageBox("Message", "Field Type has to be 'Collection' or 'List' ");
                    }
                } else if (scp.model.dictAttributes && scp.model.dictAttributes.sfwChildTemplateType == "Entity") {
                    if (DataType == "Object") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    }
                    else {
                        MessageBox("Message", "Field Type has to be 'Object'");
                    }
                }
                else {
                    MessageBox("Message", "Select Object Type for template.");
                }
            }
            else {
                if (scp.model && scp.model.Name == "sfwTableBookMark") {
                    if (DataType == "Collection") {
                        scp.$evalAsync(function () {
                            if (scp.model.dictAttributes) {
                                scp.$root.EditPropertyValue(scp.model.dictAttributes.sfwEntityField, scp.model.dictAttributes, "sfwEntityField", entityFullPath);
                                scp.onBlur();
                            }
                        });
                    } else {
                        MessageBox("Message", "Object/Field Cannot be added");
                    }
                } else {
                    MessageBox("Message", "Object/Collection Cannot be added");
                }
            }
            if (scp.validateEntityField && scp.validate) {
                scp.$evalAsync(function () {
                    scp.inputElement = undefined;
                    scp.validateEntityField();
                });
            }
        }
        e.dataTransfer.clearData();
        lstEntityTreeFieldData = null;
    } else {
        lstEntityTreeFieldData = null;
        if (e && e.preventDefault) {
            e.preventDefault();
        }
    }
}



window.onerror = function (msg, url, linenumber) {
    var rootScope = angular.element(document.body).injector().get("$rootScope");
    if (rootScope) {
        rootScope.IsLoading = false;
        rootScope.IsProjectLoaded = false;
    }
    console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
    //alert(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
    //ns.displayActivity(false);
    return true;
};

//Extra Fields Validation
function validateExtraFields(scope) {
    var flag = false;
    scope.FormDetailsErrorMessage = "";
    for (var i = 0; i < scope.objExtraFields.length; i++) {
        if (scope.objExtraFields[i].IsRequired != undefined && (scope.objExtraFields[i].IsRequired == "True" || scope.objExtraFields[i].IsRequired == true) && (!scope.objExtraFields[i].Value)) {
            scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
            flag = false;
            if (scope.objExtraFields[i].ControlType == "ComboBox") {
                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                    if (scope.objExtraFields[i].Children[j].Value) {
                        flag = true;
                        scope.FormDetailsErrorMessage = undefined;
                    }
                }
                if (flag == false) {
                    scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                    return true;
                }
            }
            if (scope.objExtraFields[i].ControlType == "CheckBoxList") {
                var flagChk = false;
                for (var j = 0; j < scope.objExtraFields[i].Children.length; j++) {
                    if (scope.objExtraFields[i].Children[j].Value) {
                        flagChk = true;
                        flag = true;
                        scope.FormDetailsErrorMessage = undefined;
                    }
                }
                if (flagChk == false) {
                    scope.FormDetailsErrorMessage = "Error: Enter the Extra Field Value for the Field " + scope.objExtraFields[i].ID;
                    return true;
                }
            }

            if (flag == false) {
                return true;
            }
        }

    }
    return false;
}

if (!String.prototype.includes) {
    String.prototype.includes = function () {
        'use strict';
        return String.prototype.indexOf.apply(this, arguments) !== -1;
    };
}

//Entity Message Description
var populateMessageByMessageID = function (messageID, lstMessages, objRule) {
    var messageIDFound = false;
    if (messageID && messageID.trim().length > 0) {
        var messages = lstMessages.filter(function (x) { return x.MessageID == messageID; });
        if (messages && messages.length > 0) {
            objRule.displayMessage = messages[0].DisplayMessage;

            if (messages[0].SeverityValue == 'I') {
                objRule.severityValue = "Information";
            }
            else if (messages[0].SeverityValue == 'E') {
                objRule.severityValue = "Error";
            }
            else if (messages[0].SeverityValue == 'W') {
                objRule.severityValue = "Warnings";
            }

            messageIDFound = true;

        }
    }

    if (!messageIDFound) {
        objRule.displayMessage = "";
        objRule.severityValue = "";
    }
};

// find in source - whole word search - remove special charaters before match
function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function toggleDuplicateIdErrList(event) {
    var selector = $(".duplicate-id-tooltip");
    if (selector.length > 0) {
        $(event.target).parent().siblings().find('.duplicate-id-tooltip').toggle();
        // $(event.target + ".duplicate-id-tooltip").fadeToggle();
    }
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function parsePathIntellisenseResult(data) {
    return $.map(JSON.parse(data), function (value, key) {
        return {
            label: value,
            value: value
        };
    });
}

function keyUpAction(selectedItem, ruleList) {

    var tempObj;
    var index = ruleList.Elements.indexOf(selectedItem);
    if (index > 0) {
        tempObj = ruleList.Elements[index - 1];
    }
    return tempObj;
}

function getNewBaseModel(name, dictAttributes, setNodeID) {
    if (name === undefined) {
        name = "";
    }
    var baseModel = { Name: name, Value: "", dictAttributes: {}, Elements: [], Children: [] };
    if (dictAttributes) {
        baseModel.dictAttributes = dictAttributes;
    }
    if (setNodeID) {
        baseModel.dictAttributes.sfwNodeID = generateUUID();
    }
    return baseModel;
}
/**
 * detect IE
 * returns version of IE or false, if browser is not Internet Explorer
 */
function detectIE() {
    var ua = window.navigator.userAgent;

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}

function stopEventPropagationConditional($event) {
    if ($event) {
        $event.stopPropagation();
    }
}


//#region set caret index & getting updated expression value
function addTextAtCaret(textAreaId, text) {
    var cursorPosition;
    var updatedExpression;
    var textArea = document.getElementById(textAreaId);
    var cursorPosition = textArea.selectionStart;
    cursorPosition = cursorPosition;

    updatedExpression = addTextAtCursorPosition(textArea, cursorPosition, text);
    updateCursorPosition(cursorPosition, text, textArea);
    return updatedExpression;
}
function addTextAtCursorPosition(textArea, cursorPosition, text) {
    var front = (textArea.value).substring(0, cursorPosition);
    var back = (textArea.value).substring(cursorPosition, textArea.value.length);
    textArea.value = front + text + back;
    return textArea.value;
}
function updateCursorPosition(cursorPosition, text, textArea) {
    if (text == undefined) {
        text.length = 0;
    }
    cursorPosition = cursorPosition + text.length;
    //   textArea.selectionStart = cursorPosition;
    textArea.focus();
    //  textArea.selectionEnd = cursorPosition;

}
(function ($) {
    $.fn.hasScrollBar = function () {
        if (this.get(0) && this.get(0).scrollHeight)
            return this.get(0).scrollHeight > this.height();
    }
})(jQuery);
//#endregion

function ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, objSelectedLogicalRule) {
    //angular.forEach(objSelectedLogicalRule.Rows, function (row) {
    //    if (row.Cells.length > 0 && row.Cells[0].Item && row.Cells[0].Item.IsStepSelected == true) {
    //        angular.forEach(row.Cells, function (step) {
    //            step.IsVisiblefterExecution = true;
    //        });
    //    }
    //});

    if (ablnShowOnlyExecutedPath) {
        for (var i = 0; i < objSelectedLogicalRule.Rows.length; i++) {
            var row = objSelectedLogicalRule.Rows[i];
            row.IsVisiblefterExecution = false;
            if (row.Cells.length > 0 && row.Cells[0].Item && row.Cells[0].Item.IsStepSelected == true) {

                row.IsVisiblefterExecution = true;
                var rowspan = row.Cells[0].Rowspan;
                if (rowspan > 1) {
                    for (var j = i + 1; j < i + rowspan; j++) {
                        objSelectedLogicalRule.Rows[j].IsVisiblefterExecution = true;
                    }
                    i = i + rowspan - 1;
                }
            }
        }
    }
    else {
        for (var i = 0; i < objSelectedLogicalRule.Rows.length; i++) {
            var row = objSelectedLogicalRule.Rows[i];
            row.IsVisiblefterExecution = true;
        }
    }

};

function setDataToEditor(scope, xmlstring, lineno, ID) {
    if (!lineno) {
        lineno = 1;
    }
    if (!xmlstring) {
        xmlstring = "";
    }
    if (scope && !scope.editor) {
        var divId = "editor_" + ID;
        scope.editor = ace.edit(divId);
        scope.editor.getSession().setMode("ace/mode/xml");
        scope.editor.resize(true);
    }
    if (scope && scope.editor) {
        scope.$evalAsync(function () {
            scope.editor.getSession().setValue(xmlstring);
            scope.isSourceDirty = false;
            scope.editor.setFontSize(14);
            scope.editor.getSession().on('change', function (e) {
                if (scope.editor.curOp && scope.editor.curOp.command.name) {
                    scope.$evalAsync(function () {
                        scope.isSourceDirty = true;
                        scope.isDirty = true;
                    });
                }
            });
            scope.editor.gotoLine(lineno);
            scope.editor.renderer.scrollCursorIntoView({ row: lineno, column: 1 }, 0.5);
        });
    }
};

//#region Knowtion Related Functions

//disable F1 (default help  Functionality)
function preventDefaultBrowserHelp() {
    $(function () {
        var cancelKeypress = false;
        // Need to cancel event (only applies to IE)
        if ("onhelp" in window) {
            // (jQuery cannot bind "onhelp" event)
            window.onhelp = function () {
                return false;
            };
        }
        $(document).keydown(function (evt) {
            // F1 pressed
            if (evt.keyCode === 112) {
                if (window.event) {
                    // Write back to IE's event object
                    window.event.keyCode = 0;
                }
                cancelKeypress = true;
                return false;
            }
        });
        // Needed for Opera (as in Andy E's answer)
        $(document).keypress(function (evt) {
            if (cancelKeypress) {
                cancelKeypress = false; // Only this keypress
                return false;
            }
        });
    });
}
//preventDefaultBrowserHelp();
function getKnowtionHelpUrl(knowtionId) {
    var knowtionUrl;

    $.ajax({
        url: "api/Login/GetKnowtionUrl?astrKnowtionID=" + knowtionId,
        type: 'POST',
        async: false,
        success: function (data) {
            knowtionUrl = data;
        },
        error: function (response) {
            var objError = JSON.parse(response.responseText);
            if (objError && objError.Message) {
                var msg = objError.Message;
                if (objError.ExceptionMessage) {
                    msg = msg + "\n" + objError.ExceptionMessage;
                }
                MessageBox("Message", msg);
            }
            else {
                MessageBox("Message", "An error occured while getting knowtion help url.");
            }
        }
    });

    return knowtionUrl;
}

//#endregion


function closest(el, predicate) {
    return predicate(el) ? el : (
        el && (el.parentNode instanceof Element) && closest(el.parentNode, predicate)
    );
}

// allowed numbers only in input 
function onlyAllowedNumbers(event) {
    if (event && !(event.charCode >= 48 && event.charCode <= 57)) {
        event.preventDefault();
    }
};

function validatePathForSpecialChar(path) {
    var regPatt = /^(\\|\s)|([\/:*?"<>|])|(\\\s*\\)|(\\$)/;
    return regPatt.test(path);
}

//get DBTypes 
function getlstQueryTypes(strQuertTypes) {
    var lstDB = strQuertTypes.split(';');
    var lstDBTypes = [];
    for (var i = 0; i < lstDB.length; i++) {
        var objDBType = {};
        var lstType = lstDB[i].split(':');
        objDBType.ID = lstType[0];
        objDBType.Attribute = lstType[1];
        lstDBTypes.push(objDBType);
    }
    return lstDBTypes;
};

//get Query
function getQuery(queryTypes, selectedQuery) {
    var query = undefined;
    for (var i = 0; i < queryTypes.length; i++) {
        if (queryTypes[i] && queryTypes[i].Attribute) {
            if (selectedQuery.dictAttributes[queryTypes[i].Attribute]) {
                query = selectedQuery.dictAttributes[queryTypes[i].Attribute];
                break;
            }
        }

    };
    return query;
};

function MessageBox(aTitle, aMessage, isConfirm, aCallback, aSize) {
    var MessagesService = angular.element(document.body).injector().get("$SgMessagesService");
    if (MessagesService && MessagesService.Message) {
        MessagesService.Message(aTitle, aMessage, isConfirm, aCallback, aSize);
    }
};

function getQueryFromEntityIntellisense(astrQueryId, alstEntity) {
    var lobjQuery = null;
    var lst = astrQueryId != undefined && astrQueryId != null ? astrQueryId.split('.') : [];
    if (lst && lst.length == 2) {
        var entityName = lst[0];
        var strQueryID = lst[1];
        var lstEntity = alstEntity.filter(function (x) {
            return x.ID == entityName;
        });
        if (lstEntity && lstEntity.length > 0) {
            var objEntity = lstEntity[0];
            var lstQuery = objEntity.Queries.filter(function (x) {
                return x.ID == strQueryID;
            });
            if (lstQuery && lstQuery.length > 0) {
                var objQuery = lstQuery[0];
                lobjQuery = objQuery;
            }
        }
    }
    return lobjQuery;
};
function combineAsPath() {
    var pathSeparator = "\\";
    var path = "";
    if (arguments && arguments.length) {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i].trim()) {
                path += arguments[i].trimFull(pathSeparator) + pathSeparator;
            }
        }
    }
    return path.trimFull(pathSeparator);
}

var GetTokens = function (str) {
    var count = 0;
    var regex = /{(.+?)}/g;
    var match, results = [];
    while (match = regex.exec(str)) {
        results.push(match[1]);
        count++;
    }

    return count;
};
function updateModelForBaseAppModel(mergedModel, baseAppModel, updateParentName, appParentModel, baseParentModel) {
    var updateParent = false;
    for (var baseIndex = 0; baseIndex < baseAppModel.Elements.length; baseIndex++) {
        var bmodel = baseAppModel.Elements[baseIndex];
        for (var appIndex = 0; appIndex < mergedModel.Elements.length; appIndex++) {
            var amodel = mergedModel.Elements[appIndex];
            if (bmodel.Name === amodel.Name) {
                if (baseAppModel.Name === "constant" && bmodel.Name === "item" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.sfwEffectiveDate === amodel.dictAttributes.sfwEffectiveDate) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (baseAppModel.Name === "constraint" && bmodel.Name === "item" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.sfwFieldName === amodel.dictAttributes.sfwFieldName) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (baseAppModel.Name === "ColumnName" && bmodel.Name === "Value" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.Text === amodel.dictAttributes.Text) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (bmodel.Name === "ColumnName" &&
                    !bmodel.dictAttributes.ID && !amodel.dictAttributes.ID) {
                    if (bmodel.dictAttributes.value === amodel.dictAttributes.value) {
                        updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                    }
                }
                else if (bmodel.dictAttributes.ID === amodel.dictAttributes.ID) {
                    updateModelForBaseAppModel(amodel, bmodel, updateParentName, mergedModel, baseAppModel);
                }
            }
        }
    }
    if (updateParentName && mergedModel.Name === updateParentName) {
        updateParent = getDescendents(mergedModel).some(function (itm) { return itm.BaseAppModel; });
    }

    var duplicateMergedModel = {};
    var duplicateBaseAppModel = {};

    for (var prop in mergedModel.dictAttributes) {
        if (mergedModel.dictAttributes[prop]) {
            duplicateMergedModel[prop] = mergedModel.dictAttributes[prop];
        }
    }
    for (var prop in baseAppModel.dictAttributes) {
        if (baseAppModel.dictAttributes[prop]) {
            duplicateBaseAppModel[prop] = baseAppModel.dictAttributes[prop];
        }
    }

    var isSame = JSON.stringify(duplicateMergedModel) === JSON.stringify(duplicateBaseAppModel);
    if (!isSame || updateParent) {
        mergedModel.BaseAppModel = baseAppModel;
        mergedModel.IsBaseAppModel = false;
    }
}

function revertToBaseAppModel(model) {
    if (model && model.BaseAppModel) {
        model.Elements = JSON.parse(JSON.stringify(model.BaseAppModel.Elements));
        model.Children = JSON.parse(JSON.stringify(model.BaseAppModel.Children));
        model.dictAttributes = JSON.parse(JSON.stringify(model.BaseAppModel.dictAttributes));
        model.IsBaseAppModel = true;
        model.BaseAppModel = null;
    }
}
