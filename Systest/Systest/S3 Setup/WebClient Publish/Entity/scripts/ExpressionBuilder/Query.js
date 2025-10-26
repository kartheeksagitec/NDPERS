app.controller("QueryExpression", ["$scope", "$http", "$rootScope", "$filter", "$EntityIntellisenseFactory", function ($scope, $http, $rootScope, $filter, $EntityIntellisenseFactory) {


    $scope.lstEntities = [];
    $scope.lstGenericTypes = ['', 'bool', 'datetime', 'decimal', 'double', 'int', 'string', 'float', 'long', 'short'];


    //#region Initilization
    $scope.Init = function () {
        $scope.lstEntities.push($filter('filter')($EntityIntellisenseFactory.getEntityIntellisense()));
        $scope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;
    };

    //#endregion

    //#region Ok click

    $scope.onOkClick = function () {
        var expression = $scope.getExpression();
        if ($scope.afterOkClick && $scope.objQuery.IsParentDialog) {
            $scope.afterOkClick(expression);
        }
        if ($scope.clickOkFromQuery) {
            $scope.clickOkFromQuery(expression);
        }
        $scope.QueryDialog.close();
    };
    //#endregion



    //#region Cancel click

    $scope.onCancelClick = function () {
        $scope.QueryDialog.close();
    };
    //#endregion


    //#region get parameters
    $scope.selectQuery = function (selectQuery) {
        $scope.objQuery.lstParameters = [];
        $scope.objQuery.selectedQuery = selectQuery;
        if (selectQuery) {
            if (selectQuery && selectQuery.trim().length > 0) {
                var index = selectQuery.indexOf(".");
                if (index > -1) {
                    var entityName = selectQuery.substring(0, index);
                    var entities = $filter('filter')($EntityIntellisenseFactory.getEntityIntellisense(), {
                        ID: entityName
                    }, true);
                    if (entities && entities.length > 0) {
                        var entity = entities[0];

                        var queryName = selectQuery.substring(index + 1, selectQuery.length);
                        if (queryName && queryName.trim().length > 0) {
                            var queries = $filter('filter')(entity.Queries, {
                                ID: queryName
                            }, true);
                            if (queries && queries.length > 0) {
                                var query = queries[0];
                                if (query.Parameters.length > 0) {
                                    angular.forEach(query.Parameters, function (x) {
                                        $scope.$evalAsync(function () {
                                            var objParams = { ParameterName: x.ID, DataType: x.DataType, Isconstant: false, Expression: "" };
                                            $scope.objQuery.lstParameters.push(objParams);
                                        });

                                    });

                                }
                            }
                        }
                    }
                }
            }
        }
    };
    //#endregion

    //#region on click on Parameters
    $scope.SelectParam = function (obj) {
        $scope.objQuery.SelectedParameter = obj;
    };

    //#endregion

    //#region get expression
    $scope.getExpression = function () {
        var expression;
        if ($scope.objQuery.selectedQuery) {
            expression = $scope.GetExpressionForSelectedQuery($scope.objQuery.selectedQuery);
        }

        return expression;
    };
    $scope.GetExpressionForSelectedQuery = function (itm) {
        var expression = "RFunc.ExecuteQuery";
        if ($scope.objQuery.genericType) {
            expression = String.format("{0}<{1}>", expression, $scope.objQuery.genericType);
        }

        expression = String.format("{0}(", expression);

        if (itm) {
            if (itm && itm.trim().length > 0) {
                var index = itm.indexOf(".");
                if (index > -1) {
                    var entityName = itm.substring(0, index);
                    var queryName = itm.substring(index + 1, itm.length);
                    expression = String.format("{0}\"{1}.{2}\"", expression, entityName, queryName);
                }
            }
        }

        angular.forEach($scope.objQuery.lstParameters, function (methodPar) {
            if (methodPar.DataType.toLowerCase().indexOf('string') > -1 && methodPar.Isconstant && methodPar.Isconstant == "True") {
                expression = String.format("{0},\"{1}\"", expression, methodPar.Expression);
            }
            else {
                expression = String.format("{0},{1}", expression, methodPar.Expression);
            }

        });


        expression = String.format("{0})", expression);

        return expression;
    };
    //#endregion


    //#region click on Query icon form Dialog window
    $scope.onQueryClick = function () {
        var newScope = $scope.$new();
        newScope.objQuery = {};
        newScope.clickOkFromQuery = function (expression) {
            if ($scope.objQuery.selectedQuery) {
                $scope.$evalAsync(function () {
                    if ($scope.objQuery.SelectedParameter) {
                        $scope.objQuery.SelectedParameter.Expression = expression;
                    }
                });
            }
        };
        newScope.IsObjectBased = $scope.IsObjectBased;
        newScope.QueryDialog = $rootScope.showDialog(newScope, "Query", "Entity/views/ExpressionBuilder/Query.html", {
            width: 1070, height: 450
        });
    };
    //#endregion

    //#region Internal function on Dialog window
    $scope.onInternalFunctionsClick = function () {
        var newScope = $scope.$new();
        newScope.ObjInternalFunctions = {};
        newScope.clickOkFromInternalFunc = function (expression) {
            if ($scope.objQuery.selectedQuery) {
                $scope.$evalAsync(function () {
                    if ($scope.objQuery.SelectedParameter) {
                        $scope.objQuery.SelectedParameter.Expression = expression;
                    }
                });
            }
        };
        newScope.IsObjectBased = $scope.IsObjectBased;
        newScope.InternalFunctionsDialog = $rootScope.showDialog(newScope, "Internal Functions", "Entity/views/ExpressionBuilder/InternalFunctions.html", {
            width: 1070, height: 450
        });
    };
    //#endregion
    //#region Business  object on Dialog window
    $scope.onBusinessMethodCommand = function () {
        var newScope = $scope.$new();
        newScope.objBusinessObject = {};
        newScope.clickOkFromBusinessObject = function (expression) {
            if ($scope.objQuery.selectedQuery) {
                $scope.$evalAsync(function () {
                    if ($scope.objQuery.SelectedParameter) {
                        $scope.objQuery.SelectedParameter.Expression = expression;
                    }
                });
            }
        };
        newScope.IsObjectBased = $scope.IsObjectBased;
        newScope.busObjectName = $scope.busObjectName;
        newScope.BusinessObjectPropertyMethodDialog = $rootScope.showDialog(newScope, "Business Object Property / Method", "Entity/views/ExpressionBuilder/BusinessObjectPropertyMethod.html", {
            width: 1070, height: 450
        });
    };
    //#endregion

    //call init function
    $scope.Init();
}]);