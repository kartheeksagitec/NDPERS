app.directive('excelmatrixscrolldirective', ["$interval", function ($interval) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            scope.promise = null;
            scope.LoadExcelMatrix = function () {
                scope.LoadSomeRowsinExcelMatrix();
                if ((element.hasScrollBar() || scope.objSelectedLogicalRule.objColumnValues.length == scope.objSelectedLogicalRule.objExcelColumnValues.length) && scope.promise) {
                    $interval.cancel(scope.promise);
                }
            }
            setTimeout(function () {
                scope.promise = $interval(function () {
                    scope.LoadExcelMatrix();
                }, 500);
            }, 1000);
            $(element).scroll(function () {
                if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        scope.LoadSomeRowsinExcelMatrix();
                    });
                }
            });
        }
    };
}]);

app.directive('decisiontablescrolldirective', ["$interval", function ($interval) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            scope.promise = null;
            scope.LoadDecisionTable = function () {
                if ((element.hasScrollBar() || scope.objSelectedLogicalRule.Rows.length <= scope.objSelectedLogicalRule.noConditionsDisplayed) && scope.promise) {
                    $interval.cancel(scope.promise);
                } else {
                    if (scope.objSelectedLogicalRule.Rows.length > scope.objSelectedLogicalRule.noConditionsDisplayed) {
                        scope.objSelectedLogicalRule.noConditionsDisplayed = scope.objSelectedLogicalRule.noConditionsDisplayed + 10;
                    }
                }
            }
            setTimeout(function () {
                scope.promise = $interval(function () {
                    scope.LoadDecisionTable();
                }, 500);
            }, 1000);
            $(element).scroll(function () {
                if (raw.scrollTop + raw.offsetHeight >= (raw.scrollHeight - 5)) { //at the bottom
                    raw.scrollHeight = raw.scrollTop + raw.scrollHeight;
                    scope.$apply(function () {
                        if (scope.objSelectedLogicalRule.Rows.length > scope.objSelectedLogicalRule.noConditionsDisplayed) {
                            scope.objSelectedLogicalRule.noConditionsDisplayed = scope.objSelectedLogicalRule.noConditionsDisplayed + 10;
                        }
                    });
                }
            });
        }
    };
}]);
