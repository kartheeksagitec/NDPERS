app.controller("SearchActiveFormController", ["$scope", "$rootScope", "$filter", "ngDialog", "$timeout", function ($scope, $rootScope, $filter, ngDialog, $timeout) {
    $scope.ID = "";
    $scope.Title = "";
    $scope.Location = "";
    $scope.IsShowSearch = false;
    $scope.ActiveFormCount = 50;

    $.connection.hubForm.server.getFormList().done(function (data) {
        $scope.filelist = JSON.parse(data);
        $scope.lstActiveForms = [];
        function iterator(itm) {
            $scope.AddInList(itm);
        }
        angular.forEach($scope.filelist, iterator);
    });

    $scope.filterForms = function (list) {
        return list.FileType == "Lookup" || list.FileType == "Maintenance" || list.FileType == "Wizard";
    };
    $scope.lstActiveForms = $filter('filter')($scope.filelist, $scope.filterForms, true);


    $scope.filterActiveForm = function (lstActiveForm) {
        return lstActiveForm.FileName.contains($scope.ID);
    };
    $scope.selectActiveFormRow = function (form) {
        $scope.selectedForm = form;
    };
    $scope.OkClick = function () {
        if ($scope.selectedForm && $scope.selectedForm != "") {
            if ($scope.isupdatefromexternalprop) {
                $scope.$emit("onSearchActiveFormOkClick", $scope.selectedForm.FileName);
            }
            else {
                if ($scope.model) {
                    if ($scope.model.dictAttributes) {
                        $scope.model.dictAttributes.sfwActiveForm = $scope.selectedForm.FileName;
                    }
                    else {
                        $scope.model.ActiveForm = $scope.selectedForm.FileName;
                        $scope.model.StrActiveForm = $scope.selectedForm.FileName;
                    }
                }
            }
            //$scope.setActiveFormValue=$scope.selectedForm.FileName;
        }
        else {
            if (!$scope.isupdatefromexternalprop) {
                if ($scope.model) {
                    if ($scope.model.dictAttributes) {
                        $scope.model.dictAttributes.sfwActiveForm = undefined;
                    }
                    else {
                        $scope.model.ActiveForm = undefined;
                        $scope.model.StrActiveForm = undefined;
                    }
                }
            }
        }
        $timeout(function () {
            if ($scope.validateActiveForm) $scope.validateActiveForm();
        });

        $scope.onCancelClick();
    };
    $scope.onCancelClick = function () {
        //ngDialog.close($scope.SearchActiveForm.id);
        $scope.SearchActiveFormDialog.close();
    };

    $scope.onSearch = function (event) {

        if (!$scope.IsShowSearch) {
            $scope.IsShowSearch = true;
        }

    };

    $scope.AddInList = function (obj) {
        var objForm = { FileName: obj.FileID, Location: obj.Location };
        if ($scope.selectFormType && $scope.selectFormType.length > 0 && $scope.selectFormType.indexOf(obj.FileType) > -1) {
            $scope.lstActiveForms.push(objForm);
        }
    };

    $scope.OnActiveFormScroll = function () {
        if ($scope.ActiveFormCount < $scope.lstActiveForms.length) {
            if ($scope.ActiveFormCount + 10 < $scope.lstActiveForms.length) {
                $scope.$evalAsync(function () {
                    $scope.ActiveFormCount = $scope.ActiveFormCount + 10;
                });
            }
            else {
                $scope.$evalAsync(function () {
                    $scope.ActiveFormCount = $scope.lstActiveForms.length;
                });
            }
        }
    };
}]);