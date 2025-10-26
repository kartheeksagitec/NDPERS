app.controller('LoginController', ["$scope", "$http", "$rootScope", "ConfigurationFactory", "hubcontext", "$cookies", function ($scope, $http, $rootScope, ConfigurationFactory, hubcontext, $cookies) {
    $scope.Submit = function () {
        if ($scope.UserName != undefined && $scope.Password != undefined && $scope.UserName != "" && $scope.Password != "") {
            $rootScope.IsProjectLoaded = true;
            $http({
                method: 'POST',
                data: {
                    UserName: $scope.UserName, Password: $scope.Password
                },
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                url: "api/Login/LoginUser"
            }).then(function successCallback(response) {
                $rootScope.IsProjectLoaded = false;
                $scope.CheckUserValidation(response);
            }, function errorCallback(exceptionData) {
                $rootScope.IsProjectLoaded = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
        else if (!$scope.UserName || $scope.UserName == "") {
            toastr.error("User Name required.");
        }
        else if (!$scope.Password || $scope.Password == "") {
            toastr.error("Password required.");
        }
    };

    $scope.LoginWithADUser = function () {
        $rootScope.IsProjectLoaded = true;
        $http({
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/LoginWithADUser"
        }).then(function successCallback(response) {
            $rootScope.IsProjectLoaded = false;
            $scope.CheckUserValidation(response, true);
        }, function errorCallback(exceptionData) {
            $rootScope.IsProjectLoaded = false;
            $scope.showExceptionDetails(exceptionData.data);
        });
    }
    $scope.CheckUserValidation = function (response, isWindowsLogin) {
        $('.login-content input[id=user]').focus();
        if (response.data == "ActiveUserLimit") {
            toastr.error("Number of users logged in reached to its maximum count.");
        }
        else if (response.data == "UserNotExist") {
            toastr.error("User not exist.");
        }
        else if (response.data == "AccessError") {
            toastr.error("Sorry, You have No Access.");
        }
        else if (response.data == "ActiveError") {
            toastr.error("Account is not activated.");
        }
        else if (response.data == null || response.data === 'UsernamePasswordNotMatched') {
            // toastr.options.positionClass = "toast-center";
            toastr.error("UserName/Password does not match.");
        }
        else if (response.data == "PasswordResetOrNotActive") {
            var errorstr = "Your account is not activated or you need to reset your password.";
            if (!isWindowsLogin) {
                errorstr += " Please click on set new password link.";
            }
            toastr.error(errorstr);
        }
        else if (response.data === "AllowADLogin") {
            $rootScope.allowADLogin = true;
        }
        else if (response.data === "None") {

        }
        else {
            $rootScope.IsLogOut = false;
            //$scope.getSessionTime();
            if ($rootScope.allowADLogin || response.data.DomainName) {
                response.data["allowADLogin"] = true;
            }
            $cookies.putObject("UserDetails", JSON.stringify(response.data));
            var userCookie = $cookies.getObject("UserDetails");
            //sessionStorage.UserDetails = JSON.stringify(response.data);
            if (response.data) {
                if (response.data.DomainName) {
                    $rootScope.IsActiveDirectoryUser = true;
                    $rootScope.allowADLogin = true;
                }
                else {
                    $rootScope.IsActiveDirectoryUser = false;
                }

                if (JSON.parse(userCookie).UserType != "") {
                    if (response.data.NewUser == "Y") {
                        $scope.ChangePasswordFunction(response.data);
                    }
                    else if (response.data.UserName == "admin") {
                        $rootScope.IsAdmin = true;
                        $rootScope.IsLogin = true;
                        $rootScope.setCurrentUserName();
                    }
                    else {
                        $scope.GetPojectNamesAndProjectDetail();
                    }
                }
                else {
                    toastr.error("Sorry, You have No Access.");
                }
            }
        }
    };
    $scope.ChangePasswordFunction = function (userData) {
        var userCookie = $cookies.getObject("UserDetails");
        var newScope = $scope.$new();
        newScope.userDetails = userData;
        newScope.changePasswordDialog = $rootScope.showDialog(newScope, "Change Password", "StartUp/views/ChangePasswordDialog.html", { height: 460, width: 500 });

        newScope.changePasswordCallBack = function (response) {
            if (response.data == "AccessError") {
                toastr.error("Sorry, You have No Access");
            }
            else if (response.data == "passwordMatch") {
                toastr.error("Password has been used in past.");
            }
            else {
                var tempUserDetails = JSON.parse(userCookie);
                tempUserDetails.NewUser = "N";
                $cookies.putObject("UserDetails", JSON.stringify(tempUserDetails));
                //sessionStorage.UserDetails = JSON.stringify(tempUserDetails);
                toastr.success("Password changed successfully.");
                newScope.closeChangePasswordDialog();

                var scope = getScopeByFileName("MainPage");
                if (scope) {
                    scope.GetAfterLoginData();
                }

                $rootScope.IsAdmin = false;
                if (response.data[0].length > 0 && response.data[1].ProjectName != null) {
                    ConfigurationFactory.setConfigurationObject(response.data[0], response.data[1]);
                    $rootScope.IsProjectLoaded = true;
                    $rootScope.loadProjectData(response.data[1], false);
                }
                else {
                    ConfigurationFactory.setConfigurationObject(response.data[0], null);
                    $rootScope.isConfigureSettingsVisible = true;
                }
                $rootScope.IsLogin = true;
            }
        };
        newScope.closeChangePasswordDialog = function () {
            newScope.changePasswordDialog.close();
        };
    };
    $scope.GetPojectNamesAndProjectDetail = function () {
        $rootScope.IsProjectLoaded = true;
        var userCookie = $cookies.getObject("UserDetails");
        $http({
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/GetPojectNamesAndProjectDetail?UserID=" + JSON.parse(userCookie).UserID
        }).then(function successCallback(response) {
            $rootScope.IsProjectLoaded = false;
            if (response.data == "AccessError") {
                toastr.error("Sorry, You have No Access.");
            }
            else if (response.data == "InvalidRequest") {
                toastr.error("Invalid Request.");
            }
            else {
                var scope = getScopeByFileName("MainPage");
                if (scope) {
                    scope.GetAfterLoginData();
                }
                $rootScope.IsAdmin = false;
                if (response.data[0].length > 0) {
                    if (response.data[1].ProjectName != null) {
                        $rootScope.IsProjectLoaded = true;
                        ConfigurationFactory.setConfigurationObject(response.data[0], response.data[1]);
                        $rootScope.loadProjectData(response.data[1], false);
                    }
                    else {
                        ConfigurationFactory.setConfigurationObject(response.data[0], null);
                        $rootScope.isConfigureSettingsVisible = true;
                    }
                }
                else {
                    $rootScope.isConfigureSettingsVisible = true;
                }
                $rootScope.IsLogin = true;
            }

        }, function errorCallback(exceptionData) {
            $rootScope.IsProjectLoaded = false;
            $rootScope.showExceptionDetails(exceptionData.data);
        });
    };
    $scope.getSessionTime = function () {
        $.ajax({
            url: "api/Login/getSessionTime",
            type: 'GET',
            async: false,
            success: successCallback,
        });
        function successCallback(response) {
            if (response) {
                SessionEvents.InitSessionTimeout(response);
            }
            else {
                SessionEvents.InitSessionTimeout(20);
            }
        }
    };
    var userCookie = $cookies.getObject("UserDetails");
    function templateSuccess() {
        var temp = JSON.parse(userCookie);
        if (temp.DomainName) {
            $rootScope.IsActiveDirectoryUser = true;
        }
        if (temp.allowADLogin) {
            $rootScope.allowADLogin = true;
        }
        if (temp.NewUser == "Y") {
            $rootScope.IsProjectLoaded = false;
            $scope.ChangePasswordFunction(temp);
        }
        else if (temp.UserName == "admin") {
            $rootScope.IsAdmin = true;
            $rootScope.IsLogin = true;
            $rootScope.IsProjectLoaded = false;
            $rootScope.setCurrentUserName();
        }
        else {
            $scope.GetPojectNamesAndProjectDetail();
        }
    }
    if (userCookie && userCookie != null) {
        templateSuccess();
    }
    else if (!$rootScope.IsLogOut) {
        $rootScope.IsProjectLoaded = true;
        $http({
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/CheckDomainUserAndLogin"
        }).then(function successCallback(response) {
            $rootScope.IsProjectLoaded = false;
            $scope.CheckUserValidation(response, true);
        }, function errorCallback(exceptionData) {
            $rootScope.IsProjectLoaded = false;
            $scope.showExceptionDetails(exceptionData.data);
        });
    }
    $scope.ForgotPasswordDialog = function () {
        var newScope = $scope.$new(true);
        newScope.UserName = $scope.UserName;
        var passDialog = $rootScope.showDialog(newScope, "Forgot Password", "StartUp/views/ForgotPasswordDialog.html", { height: 210, width: 500 });

        newScope.CheckValidUser = function () {
            newScope.ErrorMessage = "";
            if (!newScope.UserName || newScope.UserName == "") {
                newScope.ErrorMessage = "User Name Required.";
                return true;
            }
            else if (!validateEmail(newScope.UserName)) {
                newScope.ErrorMessage = "User Name not valid.";
                return true;
            }
            else {
                return false;
            }
        };
        newScope.GetPasswordClick = function () {
            //$rootScope.IsLogin = true;
            $http({
                method: 'POST',
                data: {
                    UserName: newScope.UserName
                },
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                url: "api/Login/ForgotPassword"
            }).then(function successCallback(response) {
                // $rootScope.IsLogin = false;
                if (response.data == "InvalidUser") {
                    toastr.error("Invalid user name.");
                }
                else if (response.data == "ActiveError") {
                    toastr.error("Your account is not activated.");
                }
                else {
                    toastr.success("Password reset successfully.");
                    passDialog.close();
                }
            }, function errorCallback(exceptionData) {
                $rootScope.IsLogin = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        };
        newScope.closeDialog = function () {
            passDialog.close();
        };
    };
    $('input[type=password]').bind('copy paste', function (e) {
        e.preventDefault();
    });

    $scope.ShowHidePassword = function (ablnShowHidePassword) {
        if (ablnShowHidePassword) {
            document.getElementById('pass').type = "text";
        }
        else {
            document.getElementById('pass').type = "password";
        }
    };

    $rootScope.loadProjectData = function (projectDetails, isreload) {
        if (projectDetails) {
            if (projectDetails.htxPortals) {
                var cloneProjectDetails = JSON.parse(JSON.stringify(projectDetails));
                cloneProjectDetails.htxPortals = null;
                hubcontext.hubMain.server.loadProjectData(cloneProjectDetails, isreload);
            }
            else {
                hubcontext.hubMain.server.loadProjectData(projectDetails, isreload);
            }
        }
    }
}]);
app.factory('ConfigurationFactory', [function () {
    var lstProjectName = [];
    var objLastProject = {};
    return {
        getLastProjectDetails: function () {
            return objLastProject;
        },
        getProjectList: function () {
            return lstProjectName;
        },
        setConfigurationObject: function (lstprojectName, lastProjectDetails) {
            lstProjectName = lstprojectName;
            objLastProject = lastProjectDetails;
        },
        setProjectNames: function (lstprojectName) {
            lstProjectName = lstprojectName;
        }
    };
}]);
