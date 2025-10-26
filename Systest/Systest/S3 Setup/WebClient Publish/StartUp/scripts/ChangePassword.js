app.controller("ChangePasswordController", ["$scope", "$rootScope", "$cookies", "$http", function ($scope, $rootScope, $cookies, $http) {
    $scope.ChangePasswordOkClick = function (userDetails) {
        $scope.checkPassword();
        if (!$scope.IsError) {
            $http({
                method: 'POST',
                data: { UserID: userDetails.UserID, Password: userDetails.Password.trim(), oldPassword: userDetails.oldPassword?userDetails.oldPassword.trim():"" },
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/ChangePassword"
            }).then(function successCallback(response) {
                $rootScope.IsProjectLoaded = false;
                if (response.data == "passwordMatch") {
                    toastr.error("Password has been used in past.");
                }
                else if (response.data == "oldpasswordNotMatch") {
                    toastr.error("Incorrect old password.");
                }
                else if ($scope.userDetails && $scope.userDetails.UserName == "admin") {
                    $rootScope.IsAdmin = true;
                    $scope.closeDialog();
                    toastr.success("Password changed successfully.");
                }
                else {
                    $scope.changePasswordCallBack(response);
                }

            }, function errorCallback(exceptionData) {
                $rootScope.IsProjectLoaded = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
    };
    $scope.closeDialog = function () {
        $scope.closeChangePasswordDialog();
    };
    $scope.checkPassword = function () {
        var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}/;
        if ($scope.showOldPassword && !$scope.userDetails.oldPassword) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Please enter old password.";
        }
        else if (!$scope.userDetails.Password) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Please enter new password.";
        }
        else if ($scope.userDetails.Password.indexOf(' ') >= 0) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Space not allowed";
        }
        else if (!re.test($scope.userDetails.Password)) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Password must Contain at least one Uppercase Alphabet, one Lowercase Alphabet, one Number, one Special Character and minimum six characters";
        }
        else if (!$scope.confirmPassword || $scope.confirmPassword == "") {
            $scope.IsError = true;
            $scope.ErrorMessage = "Please confirm your password.";
        }
        else if ($scope.confirmPassword != $scope.userDetails.Password) {
            $scope.IsError = true;
            $scope.ErrorMessage = "Password confirmation does't match password.";
        }
        else {
            $scope.IsError = false;
            $scope.ErrorMessage = "";
        }
    };
    $scope.checkPassword();
    $('input[type=password]').bind('copy paste', function (e) {
        e.preventDefault();
    });
    $scope.ShowHidePassword = function (ablnShowHidePassword,astrInputID) {
        if (ablnShowHidePassword) {
            document.getElementById(astrInputID).type = "text";
        }
        else {
            document.getElementById(astrInputID).type = "password";
        }
    };
}]);