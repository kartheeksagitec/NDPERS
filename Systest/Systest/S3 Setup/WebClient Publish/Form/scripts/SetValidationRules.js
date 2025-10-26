app.controller("ValidationRulesController", ["$scope", "$rootScope", "$timeout", "$ValidationService", function ($scope, $rootScope, $timeout, $ValidationService) {

    //#region Variables
    $scope.HardErrorList = [];
    $scope.SoftErrorList = [];
    $scope.IsChildOfWizardStep = false;

    //#endregion    
    //#region Init Methods
    $scope.Init = function () {
        var strRuleGroup = "";
        var objWizardStep = FindParent($scope.model, "sfwWizardStep");
        if (objWizardStep) {
            $scope.IsChildOfWizardStep = true;
            strRuleGroup = objWizardStep.dictAttributes.sfwRulesGroup;
        }
        var exisistingRules = $scope.model.dictAttributes.sfwValidationRules;
        $scope.LoadRules(strRuleGroup, exisistingRules);
    };

    $scope.LoadRules = function (strRuleGroup, exisistingRules) {
        var lstHardErrorsTemp;
        var lstSoftErrorsTemp;
        if ($scope.IsChildOfWizardStep && $scope.entityID) {
            hubMain.server.getEntityExtraData($scope.entityID).done(function (data) {
                if (data) {
                    $scope.$evalAsync(function () {
                        lstHardErrorsTemp = GetBuisnessRules(data, "", "", strRuleGroup, $scope.IsChildOfWizardStep);
                        angular.forEach(lstHardErrorsTemp, function (item) {
                            if (item != undefined && item != "") {
                                $scope.HardErrorList.push({ Name: item, IsSelected: false });
                }
            });
                        $scope.selectExistingRules(exisistingRules);
                    });  
        }
        else {
                    $scope.HardErrorList = [];
                }
            });
                    
        }
        else if ($scope.entityID) {
            hubMain.server.getEntityExtraData($scope.entityID).done(function (data) {
                if (data && data.lstHardErrorList) {
                    $scope.$evalAsync(function () {
                        var hardErrorModel = data.lstHardErrorList[0];
                if (hardErrorModel && hardErrorModel.Elements.length > 0) {
                    angular.forEach(hardErrorModel.Elements, function (item) {
                        if (item != undefined && item.dictAttributes.ID != "") {
                            $scope.HardErrorList.push({ Name: item.dictAttributes.ID, IsSelected: false });
                        }
                    });
                            $scope.selectExistingRules(exisistingRules);
                }
                    });
                }
                else {
                    $scope.HardErrorList = [];
                }
            });
                
            }
           
           //lstSoftErrorsTemp = GetBuisnessRules($scope.formobject.objExtraData, "softerror", "item", strRuleGroup, $scope.IsChildOfWizardStep);
        };
   
    $scope.selectExistingRules = function (exisistingRules) {
        if (exisistingRules != undefined && exisistingRules != "") {
            var strRules = exisistingRules.split(';');
            for (var i = 0; i < strRules.length; i++) {
                var hardError = $scope.HardErrorList.filter(function (x) { return x.Name == strRules[i]; });
                if (hardError && hardError.length > 0) {
                    hardError[0].IsSelected = true;
                }
            }

        }
    };
    //#endregion
    var strErrors = "";
    function iterator(error) {
        if (error.IsSelected) {
            strErrors += error.Name + ";";
        }
    }

    function FindParent(aModel, parentCtrlName) {
        var parent = aModel.ParentVM;

        while (parent && parent.Name != parentCtrlName) {
            parent = parent.ParentVM;
        }

        return parent;
    }

    //#region Common Events
    
    $scope.onSaveClick = function () {
        angular.forEach($scope.HardErrorList,iterator);
        angular.forEach($scope.SoftErrorList, iterator); 
        if (strErrors != undefined && strErrors != "") {
            strErrors = strErrors.substring(0, strErrors.lastIndexOf(';'));
        }

        $scope.model.dictAttributes.sfwValidationRules = strErrors;
        $timeout(function () {
            var list = [];
            if ($scope.HardErrorList) {
              list = $ValidationService.getListByPropertyName($scope.HardErrorList, 'Name');
            }
        if ($scope.validateValidationRules) $scope.validateValidationRules(list);
        });
        
        $scope.onCancelClick();
    };
    $scope.onCancelClick = function () {
        $scope.dialogValidation.close();
    };

    //#endegion

    //#region Call Init Method

    $scope.Init();

    //#endregion
}]);