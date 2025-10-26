/** 
* @version 2.1.9
* @license MIT
*/
(function (ng, undefined) {
    'use strict';

    ng.module('smart-table', []).run(['$templateCache', function ($templateCache) {
        $templateCache.put('template/smart-table/pagination.html',
            '<nav ng-if="numPages && pages.length >= 2"><ul class="pagination">' +
            '<li ng-repeat="page in pages" ng-class="{active: page==currentPage}"><a href="#" ng-click="selectPage(page); $event.preventDefault(); $event.stopPropagation();">{{page}}</a></li>' +
            '</ul></nav>');
    }]);

    ng.module('smart-table')
        .constant('stConfig', {
            pagination: {
                template: 'template/smart-table/pagination.html',
                itemsByPage: 10,
                displayedPages: 5
            },
            search: {
                delay: 400, // ms
                inputEvent: 'input'
            },
            select: {
                mode: 'single',
                selectedClass: 'st-selected',
                nextPageDelay: 300
            },
            sort: {
                ascentClass: 'st-sort-ascent',
                descentClass: 'st-sort-descent',
                descendingFirst: false,
                skipNatural: false,
                delay: 300
            },
            pipe: {
                delay: 100 //ms
            },
            delete: {
                trigger: 'auto'
            },
            add: {
                trigger: 'auto'
            },
            update: {
                trigger: 'auto'
            }
        });
    ng.module('smart-table').controller('stTableController', [
        '$scope',
        '$parse',
        '$filter',
        '$timeout',
        '$attrs',
        '$rootScope',
        'CONSTANTS',
        'stConfig',
        function StTableController($scope, $parse, $filter, $timeout, $attrs, $rootScope, CONST, stConfig) {
            var propertyName = $attrs.stTable;
            var tablerowcategory = $attrs.stTableItemRow;
            var displayGetter = $parse(propertyName);
            var displaySetter = displayGetter.assign;
            var safeGetter;
            var orderBy = $filter('orderBy');
            var filter = $filter('filter');
            var safeCopy = copyRefs(displayGetter($scope));
            var tableState = {
                Id: tablerowcategory + Date.now(),
                sort: {},
                search: {},
                pagination: { start: 0, totalItemCount: 0 },
                // flag for checking if the list binded to the table has been loaded at least
                // once with more than one record (helps in default selection of the first row at initial stage)
                IsListInitialized: false, 
                Operation: {
                    allowedoperations: [],
                    addrowtemplate: "",
                    onselectcallback: null,
                    // this is a dummy object which will be added to the collection
                    addrowobj: {},
                    beforedeletecallback: null,
                    afterdeletecallback: null,
                    // for giving prompt to user before delete - read value of this property of the record to be deleted to show in the prompt
                    // multiple values supported (comma separated) (check if multiple properties needed to be check - ex. constraint - sfwDisplayName, sfwFieldName)
                    deleteitempropertyname: "",
                    beforeaddcallback: null,
                    afteraddcallback: null,
                    additemtrigger: stConfig.add.trigger,
                    // setting validation messages before adding any record - multiple values supported (comma separated)
                    addvalidationErrors: [],
                    dblclickcallback: null,
                    enterkeycallback: null,
                    // not yet implemented
                    deletekeycallback: null,
                    deleteitemtrigger: stConfig.delete.trigger,
                    updateitemtrigger: stConfig.update.trigger,
                    beforeupdatecallback: null,
                    afterupdatecallback: null
                }
            };
            var filtered;
            var pipeAfterSafeCopy = true;
            var ctrl = this;
            var lastSelected;

            function copyRefs(src) {
                return src ? [].concat(src) : [];
            }

            function updateSafeCopy() {
                safeCopy = copyRefs(safeGetter($scope));
                if (pipeAfterSafeCopy === true) {
                    ctrl.pipe();
                }
            }

            function deepDelete(object, path) {
                if (path.indexOf('.') != -1) {
                    var partials = path.split('.');
                    var key = partials.pop();
                    var parentPath = partials.join('.');
                    var parentObject = $parse(parentPath)(object);
                    delete parentObject[key];
                    if (Object.keys(parentObject).length == 0) {
                        deepDelete(object, parentPath);
                    }
                } else {
                    delete object[path];
                }
            }

            if ($attrs.stSafeSrc) {
                safeGetter = $parse($attrs.stSafeSrc);
                var safeSrc = safeGetter($scope);
                $scope.$watch(
                    function () {
                        var safeSrc = safeGetter($scope);
                        return safeSrc && safeSrc.length ? safeSrc[0] : undefined;
                    },
                    function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            updateSafeCopy();
                        }
                    }
                );
                $scope.$watch(
                    function () {
                        var safeSrc = safeGetter($scope);
                        return safeSrc ? safeSrc.length : 0;
                    },
                    function (newValue, oldValue) {
                        if (newValue !== safeCopy.length) {
                            updateSafeCopy();
                        }
                    }
                );
                $scope.$watch(
                    function () {
                        return safeGetter($scope);
                    },
                    function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            tableState.pagination.start = 0;
                            updateSafeCopy();
                        }
                    }
                );
            }

            /**
             * sort the rows
             * @param {Function | String} predicate - function or string which will be used as predicate for the sorting
             * @param [reverse] - if you want to reverse the order
             */
            this.sortBy = function sortBy(predicate, reverse) {
                tableState.sort.predicate = predicate;
                tableState.sort.reverse = reverse === true;

                if (ng.isFunction(predicate)) {
                    tableState.sort.functionName = predicate.name;
                } else {
                    delete tableState.sort.functionName;
                }

                tableState.pagination.start = 0;
                return this.pipe();
            };

            /**
             * search matching rows
             * @param {String} input - the input string
             * @param {String} [predicate] - the property name against you want to check the match, otherwise it will search on all properties
             * @param {String | Function } [comparator] - a comparator to pass to the filter for the (pass true for stric mode)
             */
            this.search = function search(input, predicate, comparator) {
                var predicateObject = tableState.search.predicateObject || {};
                var prop = predicate ? predicate : '$';

                input = ng.isString(input) ? input.trim() : input;
                $parse(prop).assign(predicateObject, input);
                // to avoid to filter out null value
                if (!input) {
                    deepDelete(predicateObject, prop);
                }
                tableState.search.predicateObject = predicateObject;
                tableState.pagination.start = 0;
                // clear selection when list is filtered - to avoid unwanted delete
                if (lastSelected) {
                    lastSelected.isSelected = false;
                    lastSelected = null;
                }
                return this.pipe();
            };

            /**
             * this will chain the operations of sorting and filtering based on the current table state (sort options, filtering, ect)
             */
            this.pipe = function pipe() {
                var pagination = tableState.pagination;
                var output;
                if ($attrs.stGroupPropValue && $attrs.stGroupFilter) {
                    var arrGroupProp = $attrs.stGroupPropValue.split(",");
                    filtered = $filter($attrs.stGroupFilter)(safeCopy, arrGroupProp);
                    if (tableState.search.predicateObject) {
                        filtered = filter(filtered, tableState.search.predicateObject);
                    }
                }
                else {
                    filtered = tableState.search.predicateObject
                        ? filter(safeCopy, tableState.search.predicateObject)
                        : safeCopy;
                }
                if (tableState.sort.predicate) {                    
                    filtered = orderBy(
                        filtered,
                        tableState.sort.predicate,
                        tableState.sort.reverse
                    );
                }
                pagination.totalItemCount = filtered.length;
                if (pagination.number !== undefined) {
                    pagination.numberOfPages = filtered.length > 0
                        ? Math.ceil(filtered.length / pagination.number)
                        : 1;
                    pagination.start = pagination.start >= filtered.length
                        ? (pagination.numberOfPages - 1) * pagination.number
                        : pagination.start;
                    output = filtered.slice(
                        pagination.start,
                        pagination.start + parseInt(pagination.number)
                    );
                }
                displaySetter($scope, output || filtered);
            };

            /**
             * select a dataRow (it will add the attribute isSelected to the row object)
             * @param {Object} row - the row to select
             * @param {String} [mode] - "single" or "multiple" (multiple by default)
             */
            this.select = function select(row, mode, lastRow) {
                var rows = copyRefs(displayGetter($scope));
                var index = rows.indexOf(row);
                if (index !== -1) {
                    if (mode === 'single')
                    {
                        // if same row is selected - Don't un-select it
                        if (row !== lastSelected) {
                            row.isSelected = row.isSelected !== true;
                            if (lastSelected) {
                                lastSelected.isSelected = false;
                            }
                            if (lastRow) {
                                lastRow.isSelected = false;
                            }
                            lastSelected = row.isSelected === true ? row : undefined;
                        }                        
                    }
                    else {
                        rows[index].isSelected = !rows[index].isSelected;
                    }
                }
            };

            /**
             * take a slice of the current sorted/filtered collection (pagination)
             *
             * @param {Number} start - start index of the slice
             * @param {Number} number - the number of item in the slice
             */
            this.slice = function splice(start, number) {
                tableState.pagination.start = start;
                tableState.pagination.number = number;
                // clear selection when list is filtered - to avoid unwanted delete
                // if commented selection persists on changing page
                //if (lastSelected) {
                //    lastSelected.isSelected = false;
                //    lastSelected = null;
                //}
                return this.pipe();
            };

            /**
             * return the current state of the table
             * @returns {{sort: {}, search: {}, pagination: {start: number}}}
             */
            this.tableState = function getTableState() {
                return tableState;
            };

            this.getFilteredCollection = function getFilteredCollection() {
                return filtered || safeCopy;
            };

            /**
             * Use a different filter function than the angular FilterFilter
             * @param filterName the name under which the custom filter is registered
             */
            this.setFilterFunction = function setFilterFunction(filterName) {
                filter = $filter(filterName);
            };

            this.TableOperationState = function getTableOperationState() {
                return tableState.Operation;
            };

            this.setTableOperationState = function setTableOperationState(OperationState) {
                tableState.OperationState = OperationState;
            };

            /**
             * add , update and delete operations on rows
             * @param operation the name of the operation to be performed
             */
            this.crud = function crud(operation) {
                var tableOperationState = tableState.Operation;
                if (tableOperationState.allowedoperations.indexOf(operation) > -1) {
                    switch (operation) {
                        // #region delete
                        case "delete":
                            var rows = safeGetter($scope),
                                displayrows = displayGetter($scope),
                                cindex = displayrows.indexOf(lastSelected);
                            if (cindex > -1) {
                                var rowDelete = displayrows[cindex], rowproperty = "";
                                for (var i = 0; i < tableOperationState.deleteitempropertyname.length; i++) {
                                    var temp = eval("rowDelete." + tableOperationState.deleteitempropertyname[i]);
                                    if (temp) {
                                        rowproperty = temp;
                                        break;
                                    }
                                }
                                // if delete mode is auto
                                if (tableOperationState.deleteitemtrigger == stConfig.delete.trigger) {
                                    if (confirm(tablerowcategory + " : '" + rowproperty + "'" + " will be deleted, Do you want to continue?")) {
                                        this.deleteItemAutoMode(tableOperationState, rows, displayrows, rowDelete, cindex);
                                    }
                                }
                                // for manual mode - call the before callback - user will decide the if item will be deleted or not
                                else {
                                    this.deleteItemManualModeBeforeDelete(tableOperationState, rowDelete);
                                }
                            }
                            else {
                                toastr.warning("Please select the " + tablerowcategory + " which you want to delete.", "", { timeOut: 2000 });
                            }
                            break;
                        // #endregion                        
                        // #region add
                        case "add":
                            if (tableOperationState.addrowtemplate) {
                                var newobj = $parse(tableOperationState.addrowobj)($scope);
                                if (newobj) {
                                    var ctrl = this,
                                        newScope = $scope.$new();
                                    newScope.tableId = tableState.Id;
                                    var saferows = safeGetter($scope),
                                        addvalidationErrLen = tableOperationState.addvalidationErrors.length;
                                    newScope.model = JSON.parse(JSON.stringify(newobj));
                                    // #region validation
                                    newScope.model.errors = {};
                                    newScope.model.warnings = {};
                                    if (addvalidationErrLen > 0) {
                                        tableOperationState.addvalidationErrors.forEach(function (error) {
                                            var ObjError = CONST.VALIDATIONERROR.filter(function (iError) {
                                                return iError.ERROR_KEY === error;
                                            })
                                            if (ObjError.length > 0) {
                                                newScope.model.errors[error] = CONST.VALIDATION[ObjError[0].ERROR_VALUE];
                                            }
                                        });
                                    }
                                    // #endregion                                    
                                    var dialog = $rootScope.showDialog(newScope, "Add " + tablerowcategory, tableOperationState.addrowtemplate, {
                                        width: 665, height: 670
                                    });
                                    newScope.onCancelClick = function () {
                                        dialog.close();
                                    };
                                    newScope.OkClick = function () {
                                        delete newScope.model.errors;
                                        // if add mode is auto - bind ok click
                                        if (tableOperationState.additemtrigger == stConfig.add.trigger) {
                                            ctrl.addItemAutoMode(tableOperationState, saferows, newScope.model);
                                        }
                                        // for manual mode - user will decide the if item will be add or not (using a broadcast in before add callback)
                                        else {
                                            ctrl.addItemManualModeBeforeAdd(tableOperationState, newScope.model, newScope.tableId);
                                        }
                                        newScope.onCancelClick();
                                    };
                                }
                            }
                            else {
                                toastr.warning("Please give the template to add the required " + tablerowcategory);
                            }
                            break;
                        // #endregion                        
                        // #region update
                        case "update":
                            if (lastSelected) {
                                if (tableOperationState.addrowtemplate) {
                                    var ctrl = this,
                                        newScope = $scope.$new();
                                    newScope.model = JSON.parse(JSON.stringify(lastSelected)),
                                        newScope.tableId = tableState.Id,
                                        newScope.oldModel = lastSelected,
                                        newScope.IsUpdate = true;
                                    var dialog = $rootScope.showDialog(newScope, "Edit " + tablerowcategory, tableOperationState.addrowtemplate, {
                                        width: 665, height: 670
                                    });
                                    newScope.onCancelClick = function () {
                                        dialog.close();
                                    };
                                    newScope.OkClick = function () {
                                        delete newScope.model.errors;
                                        // if add mode is auto - bind ok click
                                        if (tableOperationState.updateitemtrigger == stConfig.update.trigger) {
                                            ctrl.updateItemAutoMode(tableOperationState, newScope.model);
                                        }
                                        // for manual mode - user will decide the if item will be add or not (using a broadcast in before add callback)
                                        else {
                                            ctrl.updateItemManualModeBeforeUpdate(tableOperationState, newScope.model, newScope.tableId);
                                        }
                                        newScope.onCancelClick();
                                    };
                                }
                                else {
                                    toastr.warning("Please give the template to edit " + tablerowcategory);
                                }
                            }
                            break;
                        // #endregion                        
                        default: break;
                    }
                }
                else {
                    toastr.warning("This operation is not supported for the current record.", "", { timeOut: 3000 });
                }
            };

            this.addItemAutoMode = function (tableOperationState, saferows, newObject) {
                if (tableOperationState.beforeaddcallback) {
                    $parse(tableOperationState.beforeaddcallback)($scope, { newobject: newObject });
                }
                $rootScope.PushItem(newObject, saferows, null);
                if (tableOperationState.afteraddcallback) {
                    $parse(tableOperationState.afteraddcallback)($scope, { newobject: newObject });
                }
                updateSafeCopy();
                $scope.$evalAsync(function () {
                    var pagination = ctrl.tableState().pagination;
                    var rows = ctrl.getFilteredCollection(),
                        indexOfRow = rows.indexOf(newObject),
                        finalPage = Math.floor(indexOfRow / pagination.number);
                    if (indexOfRow > -1 && (finalPage * pagination.number !== pagination.start)) {
                        ctrl.slice(finalPage * pagination.number, pagination.number);
                    }
                    ctrl.select(newObject, "single");
                });
            };

            this.addItemManualModeBeforeAdd = function (tableOperationState, newObject, tableId) {
                if (tableOperationState.beforeaddcallback) {
                    $parse(tableOperationState.beforeaddcallback)($scope, { newobject: newObject, stId: tableId });
                }
            };

            this.addItemManualModeAfterAdd = function (rowAdded, stid) {
                if (stid == tableState.Id) {
                    var tableOperationState = tableState.Operation,
                        saferows = safeGetter($scope);
                    $rootScope.PushItem(rowAdded, saferows, null);
                    if (tableOperationState.afteraddcallback) {
                        $parse(tableOperationState.afteraddcallback)($scope, { newobject: rowAdded });
                    }
                    updateSafeCopy();
                    $scope.$evalAsync(function () {
                        var pagination = ctrl.tableState().pagination;
                        var rows = ctrl.getFilteredCollection(),
                            indexOfRow = rows.indexOf(rowAdded),
                            finalPage = Math.floor(indexOfRow / pagination.number);
                        if (indexOfRow > -1 && (finalPage * pagination.number !== pagination.start)) {
                            ctrl.slice(finalPage * pagination.number, pagination.number);
                        }
                        ctrl.select(rowAdded, "single");
                    });
                }
            };

            this.deleteItemAutoMode = function (tableOperationState, rows, displayrows, rowDelete, cindex) {
                if (tableOperationState.beforedeletecallback) {
                    $parse(tableOperationState.beforedeletecallback)($scope, { updatedobject: rowDelete });
                }
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem(rowDelete, rows);
                $timeout(function () {
                    var displayrows = displayGetter($scope);
                    if (cindex > displayrows.length - 1) {
                        cindex -= 1;
                    }
                    if (displayrows && displayrows.length) {
                        ctrl.select(displayrows[cindex], "single");
                    }
                });
                $rootScope.UndRedoBulkOp("End");
                if (tableOperationState.afterdeletecallback) {
                    $parse(tableOperationState.afterdeletecallback)($scope, { updatedobject: rowDelete });
                }
            };

            this.deleteItemManualModeBeforeDelete = function (tableOperationState, rowDelete) {
                if (tableOperationState.beforedeletecallback) {
                    $parse(tableOperationState.beforedeletecallback)($scope, { updatedobject: rowDelete });
                }
            };

            this.deleteItemManualModeAfterDelete = function (rowDelete) {
                var tableOperationState = tableState.Operation,
                    rows = safeGetter($scope),
                    displayrows = displayGetter($scope),
                    cindex = displayrows.indexOf(rowDelete);
                // this will check if the row deleted belongs to the current grid
                if (cindex > -1) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.DeleteItem(rowDelete, rows);
                    $timeout(function () {
                        displayrows = displayGetter($scope);
                        if (cindex > displayrows.length - 1) {
                            cindex -= 1;
                        }
                        if (displayrows && displayrows.length) {
                            ctrl.select(displayrows[cindex], "single");
                        }
                    },100);
                    $rootScope.UndRedoBulkOp("End");
                    if (tableOperationState.afterdeletecallback) {
                        $parse(tableOperationState.afterdeletecallback)($scope, { updatedobject: rowDelete });
                    }
                }
            };

            this.updateItemAutoMode = function (tableOperationState, rowUpdate) {
                if (tableOperationState.beforeupdatecallback) {
                    $parse(tableOperationState.beforeupdatecallback)($scope, { updatedobject: rowUpdate });
                }
                $rootScope.UndRedoBulkOp("Start");
                Object.keys(rowUpdate).forEach(function (key, index) {
                    if (key !== "$$hashKey") {
                        $rootScope.EditPropertyValue(lastSelected[key], lastSelected, key, rowUpdate[key]);
                    }
                });
                $rootScope.UndRedoBulkOp("End");
                if (tableOperationState.afterupdatecallback) {
                    $parse(tableOperationState.afterupdatecallback)($scope, { updatedobject: rowUpdate });
                }
            };

            this.updateItemManualModeBeforeUpdate = function (tableOperationState, updateObject, tableId) {
                if (tableOperationState.beforeupdatecallback) {
                    $parse(tableOperationState.beforeupdatecallback)($scope, { updatedobject: updateObject, stId: tableId, IsUpdate: true });
                }
            };

            this.updateItemManualModeAfterUpdate = function (rowUpdate, stid) {
                if (stid == tableState.Id) {
                    var tableOperationState = tableState.Operation;
                    $scope.$evalAsync(function () {
                        $rootScope.UndRedoBulkOp("Start");
                        Object.keys(rowUpdate).forEach(function (key, index) {
                            if (key !== "$$hashKey") {
                                $rootScope.EditPropertyValue(lastSelected[key], lastSelected, key, rowUpdate[key]);
                            }
                        });
                        $rootScope.UndRedoBulkOp("End");
                        if (tableOperationState.afterupdatecallback) {
                            $parse(tableOperationState.afterupdatecallback)($scope, { updatedobject: rowUpdate });
                        }
                    });
                }
            }
            /**
            * add , update and delete operations on rows
            * @param operation the name of the operation to be performed
            */
            this.eventOperation = function eventOperation(event, currentRecord, selectionmode) {
                var tableoperationstate = ctrl.TableOperationState();
                switch (event) {  
                    case "click":
                        this.select(currentRecord, selectionmode);
                        if (tableoperationstate.onselectcallback) {
                            $parse(tableoperationstate.onselectcallback)($scope, { record: currentRecord });
                        }
                        break;
                    case "dblclick":
                        if (tableoperationstate.dblclickcallback) {
                            $parse(tableoperationstate.dblclickcallback)($scope, { record: lastSelected });
                        }
                        else {
                            this.crud("update");
                        }                        
                        break;
                    case "enterclick":
                        $parse(tableoperationstate.enterkeycallback)($scope, { record: lastSelected });
                        break;
                    default:
                }
            }

            /**
             * Use a different function than the angular orderBy
             * @param sortFunctionName the name under which the custom order function is registered
             */
            this.setSortFunction = function setSortFunction(sortFunctionName) {
                orderBy = $filter(sortFunctionName);
            };

            /**
             * Usually when the safe copy is updated the pipe function is called.
             * Calling this method will prevent it, which is something required when using a custom pipe function
             */
            this.preventPipeOnWatch = function preventPipe() {
                pipeAfterSafeCopy = false;
            };

        }
    ]).directive('stTable', ['$timeout', 'stConfig', function ($timeout, stConfig) {
        return {
            restrict: 'A',
            controller: 'stTableController',
            link: function (scope, element, attr, ctrl) {
                var mode = attr.stSelectMode || stConfig.select.mode,
                    pagination = ctrl.tableState().pagination,
                    currentTbody = $(element).children('tbody');

                if (attr.stSetFilter) {
                    ctrl.setFilterFunction(attr.stSetFilter);
                }

                if (attr.stSetSort) {
                    ctrl.setSortFunction(attr.stSetSort);
                }

                // Optimize this code
                $(element).find("tr[role='heading'] th").resizable({
                    handles: "e", minWidth: 15,
                    resize: function (event, ui) {
                        var lstTD = $(ui.element).closest("tbody").find("td");
                        $(lstTD).css("width", ui.size.width);
                    }
                });

                function changeFocustoFirstRow() {
                    ctrl.eventOperation("click", ctrl.getFilteredCollection()[pagination.start], mode);
                    currentTbody[0].querySelectorAll('[st-select-row]')[0].focus();
                }

                function keyBoardhandler(e) {
                    var lstFilteredCollection = ctrl.getFilteredCollection();
                    switch (e.which) {
                        case $.ui.keyCode.PAGE_UP:
                            if (pagination.start >= pagination.number) {
                                ctrl.slice(pagination.start - pagination.number, pagination.number);
                                $timeout(changeFocustoFirstRow);
                            }
                            break
                        case $.ui.keyCode.PAGE_DOWN:
                            if (pagination.start < (pagination.number * (pagination.numberOfPages - 1))) {
                                ctrl.slice(pagination.start + pagination.number, pagination.number);
                                $timeout(changeFocustoFirstRow);
                            }
                            break
                        default: break;
                    }
                }

                element.bind("keydown", function (e) {
                    scope.$evalAsync(keyBoardhandler(e));                    
                });
            }
        };
    }]);

    ng.module('smart-table')
        .directive('stSearch', ['stConfig', '$timeout', '$parse', function (stConfig, $timeout, $parse) {
            return {
                require: '^stTable',
                link: function (scope, element, attr, ctrl) {
                    var tableCtrl = ctrl;
                    var promise = null;
                    var throttle = attr.stDelay || stConfig.search.delay;
                    var event = attr.stInputEvent || stConfig.search.inputEvent;

                    attr.$observe('stSearch', function (newValue, oldValue) {
                        var input = element[0].value;
                        if (newValue !== oldValue && input) {
                            ctrl.tableState().search = {};
                            tableCtrl.search(input, newValue);
                        }
                    });

                    //table state -> view
                    scope.$watch(function () {
                        var input = element[0].value;
                        if (!input && ctrl.tableState().search) {
                            delete ctrl.tableState().search.predicateObject;
                        }
                        return ctrl.tableState().search;
                    }, function (newValue, oldValue) {
                        var predicateExpression = attr.stSearch || '$';
                        if (newValue.predicateObject && $parse(predicateExpression)(newValue.predicateObject) !== element[0].value) {
                            element[0].value = $parse(predicateExpression)(newValue.predicateObject) || '';
                        }
                    }, true);

                    // view -> table state
                    element.bind(event, function (evt) {
                        evt = evt.originalEvent || evt;
                        if (promise !== null) {
                            $timeout.cancel(promise);
                        }

                        promise = $timeout(function () {
                            tableCtrl.search(evt.target.value, attr.stSearch || '');
                            promise = null;
                        }, throttle);
                    });
                }
            };
        }]);

    ng.module('smart-table')
        .directive('stSelectRow', ['stConfig', '$timeout', function (stConfig, $timeout) {
            return {
                restrict: 'A',
                require: '^stTable',
                scope: {
                    row: '=stSelectRow'
                },
                link: function (scope, element, attr, ctrl) {
                    var mode = attr.stSelectMode || stConfig.select.mode,
                        throttle = attr.stSelectDelay || stConfig.select.nextPageDelay,                        
                        currentTableBody = $(element).closest('tbody');
                    
                    element.bind('click', function (e) {
                        scope.$evalAsync(function () {
                            ctrl.eventOperation("click", scope.row, mode);
                        });
                    });

                    element.bind('dblclick', function () {
                        scope.$evalAsync(function () {
                            ctrl.eventOperation("dblclick");
                        });
                    });

                    function keyBoardhandler(e) {
                        var pagination = ctrl.tableState().pagination,
                            lstFilteredCollection = ctrl.getFilteredCollection(),
                            indexCurrentRow = lstFilteredCollection.indexOf(scope.row),
                            finalPage = Math.ceil(indexCurrentRow / pagination.number),                           
                            tableoperationstate = ctrl.TableOperationState();
                        // up down, delete, page up, page down
                        switch (e.which) {
                            // #region Enter Key
                            case $.ui.keyCode.ENTER:
                                if (tableoperationstate.enterkeycallback) {
                                    ctrl.eventOperation("enterclick");
                                }
                                else {
                                    ctrl.crud("update");
                                }
                                e.preventDefault();
                                break;
                            // #endregion
                            // #region Up Arrow Key
                            case $.ui.keyCode.UP:
                                // handle pagination - previous page -> select first row       
                                if (indexCurrentRow > 0) {
                                    if (pagination.start > 0 && indexCurrentRow == pagination.start) {
                                        ctrl.slice((finalPage - 1) * pagination.number, pagination.number);
                                        $timeout(function () {
                                            ctrl.eventOperation("click", lstFilteredCollection[indexCurrentRow - 1], mode);
                                            var lstAllrows = currentTableBody[0].querySelectorAll('[st-select-row]');
                                            lstAllrows[lstAllrows.length - 1].focus();
                                        }, throttle);
                                    }
                                    else {
                                        ctrl.eventOperation("click", lstFilteredCollection[indexCurrentRow - 1], mode);
                                        if (element[0].previousElementSibling) {
                                            element[0].previousElementSibling.focus();
                                        }                                        
                                    }
                                }
                                break;
                            // #endregion
                            // #region Down Arrow Key/ Tab key
                            case $.ui.keyCode.TAB:
                                e.preventDefault();
                            case $.ui.keyCode.DOWN:
                                if (indexCurrentRow < pagination.totalItemCount - 1) {
                                    // handle pagination - next page -> select first row
                                    if (indexCurrentRow < (pagination.number + pagination.start - 1)) {
                                        ctrl.eventOperation("click", lstFilteredCollection[indexCurrentRow + 1], mode);
                                        if (element[0].nextElementSibling) {
                                            element[0].nextElementSibling.focus();
                                        }
                                    }
                                    else {
                                        ctrl.slice(finalPage * pagination.number, pagination.number);
                                        $timeout(function () {
                                            ctrl.eventOperation("click", lstFilteredCollection[indexCurrentRow + 1], mode);
                                            currentTableBody[0].querySelectorAll('[st-select-row]')[0].focus();
                                        }, throttle);
                                    }
                                }
                                break;
                            // #endregion
                            // #region Delete Key
                            case $.ui.keyCode.DELETE:
                                ctrl.crud("delete");
                                break;
                            // #endregion                           
                            default: break;
                        }
                    }

                    element.bind("keydown", function (e) {
                        scope.$evalAsync(keyBoardhandler(e));
                    });

                    scope.$watch('row.isSelected', function (newValue) {
                        if (newValue === true) {  
                            element.addClass(stConfig.select.selectedClass);
                        }
                        else {
                            element.removeClass(stConfig.select.selectedClass);
                        }
                    });

                    // when adding a new record - focus should be new row added
                    if (scope.row.isSelected) {
                        if (element[0] !== document.activeElement) {
                            $timeout(function () {
                                ctrl.eventOperation("click", scope.row, mode);
                                element[0].focus();                                
                            });
                        }
                    }

                    // optimize this code
                    element.find("[st-operation]").on('click', function (e) {
                        var HTMLElestOperation = e.currentTarget;
                        var strAttr = HTMLElestOperation.getAttribute("st-operation");
                        e.stopImmediatePropagation();
                        if (!scope.row.isSelected) {
                            ctrl.select(scope.row, mode);
                        }
                        $(element).focus();
                        ctrl.crud(strAttr);
                    });

                    // by default - select the first row of the table
                    if (!ctrl.tableState().IsListInitialized && scope.$parent.$first) {
                        // list has more than once record - determines the initial stage of table
                        ctrl.tableState().IsListInitialized = true;
                        ctrl.eventOperation("click", scope.row, mode);
                    }
                }
            };
        }]);

    ng.module('smart-table')
        .directive('stSort', ['stConfig', '$parse', '$timeout', function (stConfig, $parse, $timeout) {
            return {
                restrict: 'A',
                require: '^stTable',
                link: function (scope, element, attr, ctrl) {

                    var predicate = attr.stSort;
                    var getter = $parse(predicate);
                    var index = 0;
                    var classAscent = attr.stClassAscent || stConfig.sort.ascentClass;
                    var classDescent = attr.stClassDescent || stConfig.sort.descentClass;
                    var stateClasses = [classAscent, classDescent];
                    var sortDefault;
                    var skipNatural = attr.stSkipNatural !== undefined ? attr.stSkipNatural : stConfig.sort.skipNatural;
                    var descendingFirst = attr.stDescendingFirst !== undefined ? attr.stDescendingFirst : stConfig.sort.descendingFirst;
                    var promise = null;
                    var throttle = attr.stDelay || stConfig.sort.delay;

                    // set aria attributes
                    var ariaSort = 'aria-sort';
                    var ariaSortNone = 'none';
                    var ariaSortAscending = 'ascending';
                    var ariaSortDescending = 'descending';
                    element
                        .attr('role', 'columnheader')
                        .attr(ariaSort, ariaSortNone);

                    if (attr.stSortDefault) {
                        sortDefault = scope.$eval(attr.stSortDefault) !== undefined ? scope.$eval(attr.stSortDefault) : attr.stSortDefault;
                    }

                    //view --> table state
                    function sort(isInitialSort) {
                        if (descendingFirst) {
                            index = index === 0 ? 2 : index - 1;
                        } else {
                            index++;
                        }

                        var func;
                        predicate = ng.isFunction(getter(scope)) || ng.isArray(getter(scope)) ? getter(scope) : attr.stSort;
                        if (index % 3 === 0 && !!skipNatural !== true) {
                            //manual reset
                            index = 0;
                            ctrl.tableState().sort = {};
                            ctrl.tableState().pagination.start = 0;
                            func = ctrl.pipe.bind(ctrl);
                        } else {
                            func = ctrl.sortBy.bind(ctrl, predicate, index % 2 === 0);
                        }
                        if (promise !== null) {
                            $timeout.cancel(promise);
                        }
                        if (throttle < 0 || isInitialSort) {
                            func();
                        } else {
                            promise = $timeout(function () {
                                func();
                            }, throttle);
                        }
                    }

                    element.bind('click', function sortClick() {
                        if (predicate) {
                            scope.$apply(sort);
                        }
                    });

                    if (sortDefault) {
                        index = sortDefault === 'reverse' ? 1 : 0;
                        sort(true);
                    }

                    //table state --> view
                    scope.$watch(function () {
                        return ctrl.tableState().sort;
                    }, function (newValue) {
                        if (newValue.predicate !== predicate) {
                            index = 0;
                            element
                                .removeClass(classAscent)
                                .removeClass(classDescent)
                                .attr(ariaSort, ariaSortNone);
                        } else {
                            index = newValue.reverse === true ? 2 : 1;
                            element
                                .removeClass(stateClasses[index % 2])
                                .addClass(stateClasses[index - 1])
                                .attr(ariaSort, newValue.reverse ? ariaSortDescending : ariaSortAscending);
                        }
                    }, true);
                }
            };
        }]);

    ng.module('smart-table')
        .directive('stPagination', ['stConfig', '$timeout', function (stConfig, $timeout) {
            return {
                restrict: 'EA',
                require: '^stTable',
                scope: {
                    stItemsByPage: '=?',
                    stDisplayedPages: '=?',
                    stPageChange: '&',
                    selection: '=stNavigationSelect'
                },
                templateUrl: function (element, attrs) {
                    if (attrs.stTemplate) {
                        return attrs.stTemplate;
                    }
                    return stConfig.pagination.template;
                },
                link: function (scope, element, attrs, ctrl) {

                    scope.stItemsByPage = scope.stItemsByPage ? +(scope.stItemsByPage) : stConfig.pagination.itemsByPage;
                    scope.stDisplayedPages = scope.stDisplayedPages ? +(scope.stDisplayedPages) : stConfig.pagination.displayedPages;

                    scope.currentPage = 1;
                    scope.pages = [];

                    function checkSmartTable(htmlelement) {
                        if (htmlelement && htmlelement.hasAttribute("st-table")) {
                            return true;
                        }
                    }

                    var currenttable = closest(element[0], checkSmartTable),
                        firstvisibleParent = currenttable.parentNode,
                        currenttablebody = $(currenttable).find('tbody')[0];

                    function getVisibleParent(htmlparent) {
                        if (htmlparent.offsetHeight > 0) {
                            firstvisibleParent = htmlparent;
                            return
                        }
                        else {
                            getVisibleParent(htmlparent.parentNode);
                        }
                    }

                    function setDynamicTableHeight() {
                        //if parent is not visible - traverse till the parent is visible 
                        if (firstvisibleParent.offsetHeight == 0) {
                            getVisibleParent(firstvisibleParent);
                        }

                        //parent height = thead(29 + 26 + 39) + tbody (tr*38) + tfoot(77)
                        //get difference
                        //no of row - difference - height/ tbody tr

                        //no of visible rows + column header is mandatory
                        // temporary - update pagination on list change
                        var numVisibleRow = $(currenttablebody).children("[st-select-row]").length > 0 ? $(currenttablebody).children("[st-select-row]").length : scope.stItemsByPage,
                            tableheight = numVisibleRow * 38 + 26;
                        if ($(currenttable).find("tr[st-operation-row]").length > 0) {
                            tableheight += 29;
                        }
                        if ($(currenttable).find("tr[role='search']").length > 0) {
                            tableheight += 39;
                        }
                        if ($(currenttable).find("tfoot [st-pagination]").length > 0 && numVisibleRow >= scope.stItemsByPage) {
                            tableheight += 64;
                        }

                        var tablestyle = window.getComputedStyle ? window.getComputedStyle(currenttable) : currenttable.currentStyle;

                        var parentstyle = window.getComputedStyle ? window.getComputedStyle(firstvisibleParent) : firstvisibleParent.currentStyle;

                        var parentHeight = firstvisibleParent.clientHeight;  // height with padding

                        parentHeight -= (parentstyle.paddingTop ? parseFloat(parentstyle.paddingTop) : 0) + (parentstyle.paddingBottom ? parseFloat(parentstyle.paddingBottom) : 0);

                        var tablemarginBottom = tablestyle.marginBottom,
                            tablemarginTop = tablestyle.marginTop,
                            tablepaddingBottom = tablestyle.paddingBottom,
                            tablepaddingTop = tablestyle.paddingTop,
                            tableborderBottom = tablestyle.borderBottomWidth,
                            tableborderTop = tablestyle.borderTopWidth;

                        var tablemargin = (tablemarginTop ? parseFloat(tablemarginTop) : 0) + (tablemarginBottom ? parseFloat(tablemarginBottom) : 0);
                        var tablepadding = (tablepaddingTop ? parseFloat(tablepaddingTop) : 0) + (tablepaddingBottom ? parseFloat(tablepaddingBottom) : 0);
                        var tableborder = (tableborderTop ? parseFloat(tableborderTop) : 0) + (tableborderBottom ? parseFloat(tableborderBottom) : 0);

                        var differenceFactor = tableheight + tablemargin + tablepadding + tableborder - parentHeight;
                        //console.log("table height -> " + (tableheight + tablemargin + tablepadding + tableborder));
                        //console.log("parent height -> " + parentHeight);

                        var NumExtraRows = Math.ceil(differenceFactor / 38);
                        //console.log(NumExtraRows);

                        if (NumExtraRows > 0) {
                            scope.stItemsByPage -= (NumExtraRows + (scope.stItemsByPage - numVisibleRow));
                        }
                    }

                    $timeout(function () {
                        setDynamicTableHeight();                        
                    });

                    function redraw() {
                        var paginationState = ctrl.tableState().pagination;
                        var start = 1;
                        var end;
                        var i;
                        var prevPage = scope.currentPage;
                        scope.totalItemCount = paginationState.totalItemCount;
                        scope.currentPage = Math.floor(paginationState.start / paginationState.number) + 1;

                        start = Math.max(start, scope.currentPage - Math.abs(Math.floor(scope.stDisplayedPages / 2)));
                        end = start + scope.stDisplayedPages;

                        if (end > paginationState.numberOfPages) {
                            end = paginationState.numberOfPages + 1;
                            start = Math.max(1, end - scope.stDisplayedPages);
                        }

                        scope.pages = [];
                        scope.numPages = paginationState.numberOfPages;

                        for (i = start; i < end; i++) {
                            scope.pages.push(i);
                        }

                        if (prevPage !== scope.currentPage) {
                            scope.stPageChange({ newPage: scope.currentPage });
                        }

                    }

                    //table state --> view
                    scope.$watch(function () {
                        return ctrl.tableState().pagination;
                    }, redraw, true);

                    //scope --> table state  (--> view)
                    scope.$watch('stItemsByPage', function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            scope.selectPage(1);
                        }
                    });

                    scope.$watch('stDisplayedPages', redraw);

                    //view -> table state
                    scope.selectPage = function (page) {
                        if (page > 0 && page <= scope.numPages) {
                            ctrl.slice((page - 1) * scope.stItemsByPage, scope.stItemsByPage);
                        }
                    };

                    scope.$watch('selection', function (newValue, oldValue) {
                        if (newValue) {
                            $timeout(function () {
                                var pagination = ctrl.tableState().pagination;
                                var rows = ctrl.getFilteredCollection(),
                                    indexOfRow = rows.indexOf(newValue),
                                    finalPage = Math.floor(indexOfRow / pagination.number);
                                if (indexOfRow > -1) {
                                    if (finalPage * pagination.number !== pagination.start) {
                                        ctrl.slice(finalPage * pagination.number, pagination.number);
                                    }
                                    if (!newValue.isSelected) {
                                        ctrl.select(newValue, "single");
                                    }
                                    setTimeout(function () {
                                        var selectedRow = currenttable.querySelector("tbody tr.st-selected");
                                        if (selectedRow) {
                                            selectedRow.focus();
                                        }
                                    }, 500);
                                }
                            });
                        }
                    });

                    if (!ctrl.tableState().pagination.number) {
                        ctrl.slice(0, scope.stItemsByPage);
                    }
                }
            };
        }]);

    ng.module('smart-table')
        .directive('stPipe', ['stConfig', '$timeout', function (config, $timeout) {
            return {
                require: 'stTable',
                scope: {
                    stPipe: '='
                },
                link: {

                    pre: function (scope, element, attrs, ctrl) {

                        var pipePromise = null;

                        if (ng.isFunction(scope.stPipe)) {
                            ctrl.preventPipeOnWatch();
                            ctrl.pipe = function () {

                                if (pipePromise !== null) {
                                    $timeout.cancel(pipePromise)
                                }

                                pipePromise = $timeout(function () {
                                    scope.stPipe(ctrl.tableState(), ctrl);
                                }, config.pipe.delay);

                                return pipePromise;
                            }
                        }
                    },

                    post: function (scope, element, attrs, ctrl) {
                        ctrl.pipe();
                    }
                }
            };
        }]);

    ng.module('smart-table')
        .directive('stOperationRow', ['$parse', 'stConfig', function ($parse, stConfig) {
            return {
                require: '^stTable',
                restrict: 'A',
                link: function link(scope, element, attrs, ctrl) {

                    var tableoperationstate = ctrl.TableOperationState();
                    tableoperationstate.allowedoperations = attrs.stOperationAllowed ? attrs.stOperationAllowed.split(",") : [];

                    var SelectionFunctionName = attrs.stOperationOnSelectCallback ? attrs.stOperationOnSelectCallback.substring(0, attrs.stOperationOnSelectCallback.indexOf('(')) : undefined,
                        isSelectionFunctionValid = SelectionFunctionName ? $parse(SelectionFunctionName)(scope) : false;

                    if (isSelectionFunctionValid) {
                        tableoperationstate.onselectcallback = attrs.stOperationOnSelectCallback;
                    }

                    // #region delete
                    var BeforeFunctionName = attrs.stOperationBeforeDeleteCallback ? attrs.stOperationBeforeDeleteCallback.substring(0, attrs.stOperationBeforeDeleteCallback.indexOf('(')) : undefined,
                        AfterFunctionName = attrs.stOperationAfterDeleteCallback ? attrs.stOperationAfterDeleteCallback.substring(0, attrs.stOperationAfterDeleteCallback.indexOf('(')) : undefined,
                        isBeforeFunctionValid = BeforeFunctionName ? $parse(BeforeFunctionName)(scope) : false,
                        isAfterFunctionValid = AfterFunctionName ? $parse(AfterFunctionName)(scope) : false;

                    tableoperationstate.deleteitempropertyname = attrs.stOperationDeleteItemProperty ? attrs.stOperationDeleteItemProperty.split(",") : [];

                    tableoperationstate.deleteitemtrigger = attrs.stOperationDeleteTrigger ? attrs.stOperationDeleteTrigger : stConfig.delete.trigger;

                    if (tableoperationstate.deleteitemtrigger == 'manual') {
                        scope.$on('stDeleteRowBroadcast', function (event, data) {
                            scope.$evalAsync(ctrl.deleteItemManualModeAfterDelete(data.objDeleted));
                        });
                    }

                    if (isBeforeFunctionValid) {
                        tableoperationstate.beforedeletecallback = attrs.stOperationBeforeDeleteCallback;
                    }
                    if (isAfterFunctionValid) {
                        tableoperationstate.afterdeletecallback = attrs.stOperationAfterDeleteCallback;
                    }
                    // #endregion

                    // #region Add
                    var BeforeAddFunctionName = attrs.stOperationBeforeAddCallback ? attrs.stOperationBeforeAddCallback.substring(0, attrs.stOperationBeforeAddCallback.indexOf('(')) : undefined,
                        AfterAddFunctionName = attrs.stOperationAfterAddCallback ? attrs.stOperationAfterAddCallback.substring(0, attrs.stOperationAfterAddCallback.indexOf('(')) : undefined,
                        isBeforeAddFunctionValid = BeforeAddFunctionName ? $parse(BeforeAddFunctionName)(scope) : false,
                        isAfterAddFunctionValid = AfterAddFunctionName ? $parse(AfterAddFunctionName)(scope) : false;

                    tableoperationstate.additemtrigger = attrs.stOperationAddTrigger ? attrs.stOperationAddTrigger : stConfig.add.trigger;

                    if (tableoperationstate.additemtrigger == 'manual') {
                        scope.$on('stAddRowBroadcast', function (event, data) {
                            scope.$evalAsync(ctrl.addItemManualModeAfterAdd(data.objAdded, data.stid));
                        });
                    }

                    if (isBeforeAddFunctionValid) {
                        tableoperationstate.beforeaddcallback = attrs.stOperationBeforeAddCallback;
                    }
                    if (isAfterAddFunctionValid) {
                        tableoperationstate.afteraddcallback = attrs.stOperationAfterAddCallback;
                    }

                    if (attrs.stOperationAddObject) {
                        tableoperationstate.addrowobj = attrs.stOperationAddObject;
                    }
                    if (attrs.stOperationAddTemplate) {
                        tableoperationstate.addrowtemplate = attrs.stOperationAddTemplate;
                    }
                    tableoperationstate.addvalidationErrors = attrs.stOperationAddValidationErrors ? attrs.stOperationAddValidationErrors.split(",") : [];

                    // #endregion

                    // #region update
                    // #region override double click
                    if (attrs.stSelectRowDblclick) {
                        var dblclickFunctionName = attrs.stSelectRowDblclick ? attrs.stSelectRowDblclick.substring(0, attrs.stSelectRowDblclick.indexOf('(')) : undefined,
                            isdblclickFunctionValid = dblclickFunctionName ? $parse(dblclickFunctionName)(scope) : false;
                        if (ng.isFunction(isdblclickFunctionValid)) {
                            tableoperationstate.dblclickcallback = attrs.stSelectRowDblclick;
                        }
                    }
                    // #endregion
                    // #region override enter key
                    if (attrs.stSelectRowEnterKeyclick) {
                        var enterkeyclickFunctionName = attrs.stSelectRowEnterKeyclick ? attrs.stSelectRowEnterKeyclick.substring(0, attrs.stSelectRowEnterKeyclick.indexOf('(')) : undefined,
                            isenterkeyclickFunctionValid = enterkeyclickFunctionName ? $parse(enterkeyclickFunctionName)(scope) : false;
                        if (ng.isFunction(isenterkeyclickFunctionValid)) {
                            tableoperationstate.enterkeycallback = attrs.stSelectRowEnterKeyclick;
                        }
                    }
                    // #endregion
                    var BeforeUpdateFunctionName = attrs.stOperationBeforeUpdateCallback ? attrs.stOperationBeforeUpdateCallback.substring(0, attrs.stOperationBeforeUpdateCallback.indexOf('(')) : undefined,
                        AfterUpdateFunctionName = attrs.stOperationAfterUpdateCallback ? attrs.stOperationAfterUpdateCallback.substring(0, attrs.stOperationAfterUpdateCallback.indexOf('(')) : undefined,
                        isBeforeUpdateFunctionValid = BeforeUpdateFunctionName ? $parse(BeforeUpdateFunctionName)(scope) : false,
                        isAfterUpdateFunctionValid = AfterUpdateFunctionName ? $parse(AfterUpdateFunctionName)(scope) : false;

                    tableoperationstate.updateitemtrigger = attrs.stOperationUpdateTrigger ? attrs.stOperationUpdateTrigger : stConfig.update.trigger;

                    if (tableoperationstate.updateitemtrigger == 'manual') {
                        scope.$on('stUpdateRowBroadcast', function (event, data) {
                            scope.$evalAsync(ctrl.updateItemManualModeAfterUpdate(data.objUpdated, data.stid));
                        });
                    }

                    if (isBeforeUpdateFunctionValid) {
                        tableoperationstate.beforeupdatecallback = attrs.stOperationBeforeUpdateCallback;
                    }
                    if (isAfterUpdateFunctionValid) {
                        tableoperationstate.afterupdatecallback = attrs.stOperationAfterUpdateCallback;
                    }
                    // #endregion

                    ctrl.setTableOperationState(tableoperationstate);

                }
            };
        }]);

    ng.module('smart-table')
        .directive('stOperation', ['$parse', function ($parse) {
            return {
                require: '^stTable',
                restrict: 'A',
                link: function link(scope, element, attrs, ctrl) {
                    function checkSmartTableRow(htmlelement) {
                        if (htmlelement && htmlelement.hasAttribute("st-select-row")) {
                            return true;
                        }
                    }
                    var HTMLCurrentRow = closest(element[0], checkSmartTableRow);
                    if (!HTMLCurrentRow) {
                        element.bind('click', function () {
                            scope.$evalAsync(function () {
                                ctrl.crud(attrs.stOperation);
                            });
                        });
                    }
                }
            };
        }]);

})(angular);