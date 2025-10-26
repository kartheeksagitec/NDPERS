app.controller("InternalFunctions", ["$scope", "$http", "$rootScope", "$filter", "hubcontext", function ($scope, $http, $rootScope, $filter, hubcontext) {

    $scope.lstGenericTypes = ['', 'bool', 'datetime', 'decimal', 'double', 'int', 'string', 'float', 'long', 'short'];

    //#region Init
    $scope.Init = function () {
      //  $scope.PopulateInternalFunctions();
        $scope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;
       
    };

    //#endregion   

    //#region Ok click

    $scope.onOkClick = function () {
        var expression = $scope.getExpression();
        if ($scope.afterOkClick && $scope.ObjInternalFunctions.IsParentDialog) {
            $scope.afterOkClick(expression);
        }
        if ($scope.clickOkFromInternalFunc) {
            $scope.clickOkFromInternalFunc(expression);
        }
        $scope.InternalFunctionsDialog.close();
    };
    //#endregion

    //#region click on Internal Function
    $scope.onClickInternalFunction = function (obj) {
        $scope.ObjInternalFunctions.lstParameters = [];
        $scope.ObjInternalFunctions.SelectedInternalFunc = obj;
        if (obj) {
            if (obj.lstParameter.length > 0) {
                angular.forEach(obj.lstParameter, function (x) {
                    $scope.$evalAsync(function () {
                        var objParams = { ParameterName: x.ParameterName, ParameterDataType: x.ParameterDataType, Isconstant: false, Expression: "", IsGeneric: x.IsGeneric };
                        $scope.ObjInternalFunctions.lstParameters.push(objParams);
                    });

                });
            }
        }

    };
    //#endregion

    //#region Cancel click

    $scope.onCancelClick = function () {
        $scope.InternalFunctionsDialog.close();
    };
    //#endregion

    //#region Select params to enable expression builder icon
    $scope.SelectParam = function (obj) {
        $scope.ObjInternalFunctions.SelectedParameter = obj;
    };
    //#endregion

    //#region Get Expression

    $scope.getExpression = function () {
        var expression;
        if ($scope.ObjInternalFunctions.SelectedInternalFunc) {
            expression = $scope.GetExpressionForInternalFunc($scope.ObjInternalFunctions.SelectedInternalFunc.MethodName.Name);
        }

        return expression;
    };
    $scope.GetExpressionForInternalFunc = function (expression) {
        if ($scope.ObjInternalFunctions.SelectedInternalFunc && $scope.ObjInternalFunctions.SelectedInternalFunc.IsGeneric) {
            expression = String.format("{0}<{1}>", expression, $scope.ObjInternalFunctions.genericType);
        }
        expression = String.format("{0}(", expression);
        angular.forEach($scope.ObjInternalFunctions.lstParameters, function (methodPar) {
            if (methodPar.ParameterDataType.toLowerCase() == "string" && methodPar.Isconstant) {
                expression = String.format("{0}\"{1}\",", expression, methodPar.Expression);
            }
            else {
                expression = String.format("{0}{1},", expression, methodPar.Expression);
            }

        });
        expression = expression.trimEnd(',');
        expression = String.format("{0})", expression);
        return expression;
    };
    //#endregion

    //#region Internal function on Dialog window
    $scope.onInternalFunctionsClick = function () {
        var newScope = $scope.$new();
        newScope.ObjInternalFunctions = {};
        newScope.clickOkFromInternalFunc = function (expression) {
            if ($scope.ObjInternalFunctions.SelectedInternalFunc) {
                $scope.$evalAsync(function () {
                    if ($scope.ObjInternalFunctions.SelectedParameter) {
                        $scope.ObjInternalFunctions.SelectedParameter.Expression = expression;
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

    //#region on clicking Query Icon from Dialog window
    $scope.onQueryClick = function () {
        var newScope = $scope.$new();
        newScope.objQuery = {};
        newScope.clickOkFromQuery = function (expression) {
            // setting expression in Internal function Window from Query Window
            if ($scope.ObjInternalFunctions.SelectedInternalFunc) {
                $scope.$evalAsync(function () {
                    if ($scope.ObjInternalFunctions.SelectedParameter) {
                        $scope.ObjInternalFunctions.SelectedParameter.Expression = expression;
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

    //#region Business  object on Dialog window
    $scope.onBusinessMethodCommand = function () {
        var newScope = $scope.$new();
        newScope.objBusinessObject = {};
        newScope.clickOkFromBusinessObject = function (expression) {
            if ($scope.ObjInternalFunctions.SelectedInternalFunc) {
                $scope.$evalAsync(function () {
                    if ($scope.ObjInternalFunctions.SelectedParameter) {
                        $scope.ObjInternalFunctions.SelectedParameter.Expression = expression;
                    }
                });
            }
        };
        newScope.busObjectName = $scope.busObjectName;
        newScope.IsObjectBased = $scope.IsObjectBased;
        newScope.BusinessObjectPropertyMethodDialog = $rootScope.showDialog(newScope, "Business Object Property / Method", "Entity/views/ExpressionBuilder/BusinessObjectPropertyMethod.html", {
            width: 1070, height: 450
        });
    };
    //#endregion

    //#region Validation for generic type if Method is generic then Generic type should be Selected

    $scope.checkGenericMethod=function()
    {
        var retValue = false;
        $scope.InternalFunctionErrorMessage = undefined;
        if ($scope.ObjInternalFunctions.SelectedInternalFunc && $scope.ObjInternalFunctions.SelectedInternalFunc.IsGeneric)
        {
            if (!$scope.ObjInternalFunctions.genericType)
            {
                $scope.InternalFunctionErrorMessage = "Error: Please select generic type.";
                retValue = true;
            }
        }

        return retValue;
    };
    //#endregion
   
    // Call Init function
    $scope.Init();
}]);