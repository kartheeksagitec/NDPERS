app.controller('GetActiveDirectoryUsersController', ['$scope', function ($scope) {

    $scope.$parent.dialogMapuser = null;
    $scope.$parent.selectedADUsers = [];
    $scope.$parent.samAccountName = "";
    $scope.$parent.name = "";
    $scope.$parent.domainName = "";
    $scope.$parent.email = "";

    $scope.selectADUser = function (user, index) {
        var filterIndex = $scope.llstUnMappedADUsers.indexOf(user);
        var selectedIndex = $scope.selectedADUsers.indexOf(user);

        if (filterIndex > -1 && selectedIndex == -1) {
            $scope.llstUnMappedADUsers.splice(filterIndex, 1);
            var tempUser = {};
            angular.copy(user, tempUser);
            if ($scope.$parent.IsShowRole) {
                tempUser.UserType = "developer";
            }
            tempUser.editableEmailId = user.EmailID;
            if (index || index === 0) {
                $scope.selectedADUsers.splice(0, 0, tempUser);
            }
            else {
                $scope.selectedADUsers.push(tempUser);
            }
        }
        $scope.$parent.validateEmail();
    };
    $scope.deSelectADUser = function (user, index) {
        var filterIndex = $scope.llstUnMappedADUsers.indexOf(user);
        var selectedIndex = $scope.selectedADUsers.indexOf(user);

        if (selectedIndex > -1 && filterIndex == -1) {
            $scope.selectedADUsers.splice(selectedIndex, 1);

            if (index || index === 0) {
                $scope.llstUnMappedADUsers.splice(0, 0, user);
            }
            else {
                delete user.editableEmailId;
                $scope.llstUnMappedADUsers.push(user);
            }
        }
        $scope.$parent.validateEmail();
    };
    $scope.selectAllADUsers = function () {
        if ($scope.llstUnMappedADUsers && $scope.llstUnMappedADUsers.length > 0) {
            for (var idx = $scope.llstUnMappedADUsers.length - 1; idx >= 0; idx--) {
                $scope.selectADUser($scope.llstUnMappedADUsers[idx], 0);
            }
            $scope.filterUserDisplayCount = $scope.selectedUserDisplayCount = 30;
        }
    };
    $scope.deSelectAllADUsers = function () {
        for (var idx = $scope.selectedADUsers.length - 1; idx >= 0; idx--) {
            $scope.deSelectADUser($scope.selectedADUsers[idx], 0);
        }
        $scope.filterUserDisplayCount = $scope.selectedUserDisplayCount = 30;
    };
    $scope.clearSearchCriteria = function () {
        $scope.$parent.samAccountName = "";
        $scope.$parent.name = "";
        $scope.$parent.domainName = "";
        $scope.$parent.email = "";
    };
    $scope.$parent.closeDialog = function () {
        if ($scope.$parent.dialogMapuser && $scope.$parent.dialogMapuser.close) {
            $scope.$parent.dialogMapuser.close();
        }
    };
    $scope.$parent.beforeOK = function () {
        $scope.validateEmail();
        for (var idx = 0; idx < $scope.$parent.selectedADUsers.length; idx++) {
            $scope.$parent.selectedADUsers[idx].EmailId = $scope.$parent.selectedADUsers[idx].editableEmailId;
        }
    }
    $scope.$parent.validateEmail = function () {
        if ($scope.$parent.selectedADUsers.some(function (user) { return !user.editableEmailId; })) {
            $scope.$parent.errorMessage = "One or more user doesn't have email id.";
        }
        else if ($scope.$parent.selectedADUsers.some(function (user) { return !validateEmail(user.editableEmailId); })) {
            $scope.$parent.errorMessage = "One or more user doesn't have valid email id.";
        }
        else {
            $scope.$parent.errorMessage = "";
        }
    }
}]);
