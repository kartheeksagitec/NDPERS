app.controller("BusinessObjectPropertyMethod", ["$scope", "$http", "$rootScope", function ($scope, $http, $rootScope) {

    //#region New BusinessObject Tree Fields 
    $scope.objBusinessObjectTree = {};
    $scope.objBusinessObjectTree.lstdisplaybusinessobject = [];
    $scope.objBusinessObjectTree.lstmultipleselectedfields = [];
    $scope.objBusinessObjectTree.lstCurrentBusinessObjectProperties = [];
    $scope.objBusinessObjectTree.SelectedField = undefined;
    $scope.objBusinessObject.ObjTree = undefined;
    $scope.objBusinessObject.sfwObjectField = undefined;
    $scope.lstMethodProperties = [];
    $scope.lstGenericTypes = ['', 'bool', 'datetime', 'decimal', 'double', 'int', 'string', 'float', 'long', 'short'];
    //#endRegion

    $scope.objBusinessObjectTree.BusObjectName = $scope.busObjectName; // assigning business Object 
    $scope.IsObjectBased = $scope.SelectedRule.dictAttributes.sfwObjectBased;

    //#region get Parameters

    $scope.onPropertyChange = function (id)
    {
        
        $scope.lstMethodProperties = [];
        if (id)
        {
            if (id.indexOf('.') > 0) {
                var arrText = id.split('.');
                var data = $scope.objBusinessObject.ObjTree;
                for (var index = 0; index < arrText.length; index++) {
                    data = $scope.getData(arrText[index], data.ChildProperties, data.lstMethods);
                }

                if (data) {
                    $scope.objBusinessObjectTree.SelectedField = data;
                    $scope.onGetParameters(data);
                }
            }
            else {
                
                if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
                    var lstMethod = $scope.objBusinessObject.ObjTree.lstMethods.filter(function (itm) {
                        return itm.ShortName == id;
                    });
                    if (lstMethod && lstMethod.length > 0) {
                        $scope.objBusinessObjectTree.SelectedField = lstMethod[0];
                        $scope.onGetParameters(lstMethod[0]);
                    }
                }
            }


        }
    };
    
    $scope.onGetParameters = function (selectedfield) {

        if (selectedfield && selectedfield.Parameters && selectedfield.Parameters.length > 0) {
            angular.forEach(selectedfield.Parameters, function (obj) {
                var objParams = { ParameterName: obj.ParameterName, DataType: obj.ParameterType.FullName, Isconstant: false, Expression: "" };
                $scope.lstMethodProperties.push(objParams);
            });

        }

    };

    $scope.getData = function (data, childProps, lstMethods) {
        var objData;
        angular.forEach(childProps, function (prop) {
            if (!objData) {
                if (prop.ShortName == data) {
                    objData = prop;
                    return objData;
                }

                if (prop.ChildProperties.length > 0) {
                    objData = $scope.getData(data, prop.ChildProperties);
                    if (objData) {
                        return objData;
                    }
                }
            }
        });

        angular.forEach(lstMethods, function (prop) {
            if (!objData) {
                if (prop.ShortName == data) {
                    objData = prop;
                    return objData;
                }
            }
        });
        return objData;
    };

    //#endregion

    //#region Select params to enable expression builder icon
    $scope.SelectParam = function (obj) {
        $scope.objBusinessObject.SelectedParameter = obj;
    };
    //#endregion

    //#region Ok click of Business Object 

    $scope.onOkClick = function () {
        var expression = $scope.GetExpression();
        if ($scope.afterOkClick && $scope.objBusinessObject.IsParentDialog) {
            $scope.afterOkClick(expression);
        }
        if ($scope.clickOkFromBusinessObject) {
            $scope.clickOkFromBusinessObject(expression);
        }
        $scope.BusinessObjectPropertyMethodDialog.close();
    };
    //#endregion


    //#region getExpression
    $scope.GetExpression = function () {
        var expression;

        if ($scope.objBusinessObjectTree.SelectedField) {
            expression = "this.";
            if ($scope.objBusinessObjectTree.SelectedField.ItemPath && $scope.objBusinessObjectTree.BusObjectName) {
                if ($scope.objBusinessObjectTree.SelectedField.ItemPath.startsWith($scope.objBusinessObjectTree.BusObjectName)) {
                    var itemPath = $scope.objBusinessObjectTree.SelectedField.ItemPath.substring($scope.objBusinessObjectTree.BusObjectName.length + 1);
                    expression = String.format("{0}{1}", expression, itemPath);
                }
            }
            else if ($scope.objBusinessObjectTree.SelectedField && $scope.objBusinessObjectTree.SelectedField.Name) {
                expression = String.format("{0}{1}", expression, $scope.objBusinessObjectTree.SelectedField.Name);
            }

            if ($scope.objBusinessObjectTree.SelectedField.DataType.toLowerCase() == "methodtype") {
                expression = $scope.UpdateMethodExpression(expression);
            }
        }

        return expression;
    };

    $scope.UpdateMethodExpression = function (exp) {
        if ($scope.objBusinessObjectTree.SelectedField && $scope.objBusinessObjectTree.SelectedField.lstMethods.length > 0 && $scope.objBusinessObjectTree.SelectedField.lstMethods.IsGeneric) {
            exp = String.format("{0}<{1}>", exp, $scope.objBusinessObjectTree.genericType);
        }
        exp = String.format("{0}(", exp);
        if ($scope.lstMethodProperties.length > 0) {

            angular.forEach($scope.lstMethodProperties, function (itm) {

                if (itm.DataType.toLowerCase().indexOf('string') > -1 && itm.Isconstant) {
                    exp = String.format("{0}\"{1}\",", exp, itm.Expression);
                }
                else {
                    exp = String.format("{0}{1},", exp, itm.Expression);
                }
            });

            exp = exp.trimEnd(',');
        }
        exp = String.format("{0})", exp);

        return exp;
    };


    //#endregion

    //#region Cancel click

    $scope.onCancelClick = function () {
        $scope.BusinessObjectPropertyMethodDialog.close();
    };
    //#endregion


    //#region Business  object on Dialog window
    $scope.onBusinessMethodCommand = function () {
        var newScope = $scope.$new();
        newScope.objBusinessObject = {};
        newScope.clickOkFromBusinessObject = function (expression) {
            if ($scope.objBusinessObjectTree.SelectedField) {
                $scope.$evalAsync(function () {
                    if ($scope.objBusinessObject.SelectedParameter) {
                        $scope.objBusinessObject.SelectedParameter.Expression = expression;
                    }
                });
            }
        };
        newScope.IsObjectBased = $scope.IsObjectBased;
        newScope.BusinessObjectPropertyMethodDialog = $rootScope.showDialog(newScope, "Business Object Property / Method", "Entity/views/ExpressionBuilder/BusinessObjectPropertyMethod.html", {
            width: 1070, height: 450
        });
    };
    //#endregion

    //#region Internal function on Dialog window
    $scope.onInternalFunctionsClick = function () {
        var newScope = $scope.$new();
        newScope.ObjInternalFunctions = {};
        newScope.clickOkFromInternalFunc = function (expression) {
            if ($scope.objBusinessObjectTree.SelectedField) {
                $scope.$evalAsync(function () {
                    if ($scope.objBusinessObject.SelectedParameter) {
                        $scope.objBusinessObject.SelectedParameter.Expression = expression;
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


    //#region click on Query icon form Dialog window
    $scope.onQueryClick = function () {
        var newScope = $scope.$new();
        newScope.objQuery = {};
        newScope.clickOkFromQuery = function (expression) {
            if ($scope.objBusinessObjectTree.SelectedField) {
                $scope.$evalAsync(function () {
                    if ($scope.objBusinessObject.SelectedParameter) {
                        $scope.objBusinessObject.SelectedParameter.Expression = expression;
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



    //#region Validation for generic type if Method is generic then Generic type should be Selected

    $scope.checkGenericMethod = function () {
        var retValue = false;
        $scope.BuisnessObjectErrorMessage = undefined;
        if ($scope.objBusinessObjectTree.SelectedField && $scope.objBusinessObjectTree.SelectedField.lstMethods && $scope.objBusinessObjectTree.SelectedField.lstMethods.length > 0 && $scope.objBusinessObjectTree.SelectedField.lstMethods.IsGeneric) {
            if (!$scope.objBusinessObjectTree.genericType) {
                $scope.BuisnessObjectErrorMessage = "Error: Please select generic type.";
                retValue = true;
            }
        }

        return retValue;
    };
    //#endregion
}]);