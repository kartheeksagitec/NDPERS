app.directive("rowconditionitemdirectiveforscenarioexecution", ["$compile", function ($compile) {
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '='
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
                    if (scope.items.Item.Expression && scope.items.Item.Expression.length > 0) {
                        scope.conditionType = "5";
                    }
                }
            };

            scope.getclass = function (obj) {
                if (obj != undefined) {
                    if (obj.Item.ItemType == "if") {
                        if (obj.Item.IsStepSelected) {
                            return "dt-if-execution";
                        }
                        else if (obj.Item.IsRelatedStepAfterExecution) {
                            return "dt-if-execution-related-step";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-if-step-execution-error-occur";
                        }
                        else {
                            return "dt-if-step-execution";
                        }
                    }
                    else if (obj.Item.ItemType == "colheader" || obj.Item.ItemType == "rowheader") {
                        return "dt-row-col-header-execution";
                    }
                    else if (obj.Item.ItemType == "assignheader") {
                        if (obj.Item.IsStepSelected) {
                            return "dt-assign-header-execution";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-assign-header-execution-related-step";
                        }
                        else {
                            return "dt-row-assign-header-execution";
                        }
                    }
                    else {
                        if (obj.Item.IsStepSelected) {
                            return "dt-assign-execution-decisiontable";
                        }
                        else if (obj.Item.IsRelatedStepAfterExecution) {
                            return "dt-assign-execution-related-step";
                        }
                        else if (obj.Item.IsErrorOccured) {
                            return "dt-assign-execution-error-occur";
                        }
                        else {
                            return "dt-assign-step-execution";
                        }
                    }
                }
            };
            scope.init();
        }
    };
}]);


app.directive("rowconditiondirectiveforscenarioexecution", ["$compile", "$interval", function ($compile, $interval) {
    return {
        restrict: "A",
        scope: {
            items: '=',
            logicalrule: '='
        },
        template: '<td rowconditionitemdirectiveforscenarioexecution ng-repeat="objChild in items.Cells" items="objChild" valign="top" rowspan={{objChild.Rowspan}} colspan={{objChild.Colspan}} class="dt-table-cell"  logicalrule="logicalrule"></td>',
    };
}]);

