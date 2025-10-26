app.controller("CreateClientController", ["$scope", "$http", "$rootScope", "ConfigurationFactory", "$cookies", "$timeout", function ($scope, $http, $rootScope, ConfigurationFactory, $cookies, $timeout) {
    $scope.lstClient = [];
    $scope.lstUser = [];
    $scope.Password = "sagitec";
    $rootScope.IsProjectLoaded = true;
    var userCookie = $cookies.getObject("UserDetails");
    $scope.LoginName = JSON.parse(userCookie).UserName;
    $scope.getUserClientData = function () {
        $http({
            method: 'POST',
            data: {},
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/GetClientUserData"
        }).then(function successCallback(response) {
            $rootScope.IsProjectLoaded = false;
            if (response.data[0].length > 0) {
                $scope.lstUser = response.data[0];
            }
            if (response.data[1].length > 0) {
                $scope.lstClient = response.data[1];
            }

        }, function errorCallback(exceptionData) {
            $rootScope.IsProjectLoaded = false;
            $rootScope.showExceptionDetails(exceptionData.data);
        });
    };
    $scope.getUserClientList = function (recordSelection) {
        $http({
            method: 'POST',
            data: {},
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/GetUserClientProjectList"
        }).then(function successCallback(response) {
            $scope.lstAllUsers = response.data;
            if (angular.isArray($scope.lstAllUsers) && $scope.lstAllUsers.length) {
                if (recordSelection == "first") {
                    $scope.selectUserRow($scope.lstAllUsers[0]);
                } else if (recordSelection == "last"){
                    $scope.selectUserRow($scope.lstAllUsers[$scope.lstAllUsers.length - 1]);
                    $timeout(function () {
                        $("#sg-user-details.userinfo-table").scrollTo($("#sg-user-details.userinfo-table").find("tr.selected"), null, null);
                    });
                }
            }
        }, function errorCallback(exceptionData) {
            console.log(exceptionData);
        });
    };
    $scope.LogoutSession = function () {
        $.ajax({
            url: "api/Login/LogOutUser",
            type: 'GET',
            async: false,
            success: successCallback,
            error: errorCallBack
        });
        function successCallback(response) {
            $cookies.remove("UserDetails");
            $scope.LoginName = "";
            $rootScope.IsLogin = false;
            $rootScope.IsAdmin = false;
            $rootScope.isConfigureSettingsVisible = false;
            $rootScope.isstartpagevisible = false;
            toastr.success("User has logged out successfully");
        }
        function errorCallBack(exceptionData) {
            $rootScope.IsLoading = false;
            $rootScope.showExceptionDetails(exceptionData.data);
        }


    };
   
    $scope.selectClientRow = function (objClient) {
        $scope.selectedClient = objClient;
    };
    $scope.updateUserRole = function (client) {
        if (client && $scope.selectedUser) {
            var adminUsers = client.AdminUser;
            var tempAdmins = adminUsers.split(";");
            var clientIndex = tempAdmins.indexOf($scope.selectedUser.UserID.toString());
            if (client.userRole != "owner" && clientIndex > -1) {
                tempAdmins.splice(clientIndex, 1);
            } else if (client.userRole == "owner" && clientIndex == -1) {
                tempAdmins.push($scope.selectedUser.UserID.toString());
            }
            adminUsers = tempAdmins.join(";");
           
            var requestData = { UserID: $scope.selectedUser.UserID, ClientID: client.clientId, UserRole: client.userRole, AdminUser: adminUsers };
            var boolYesNo = confirm("Are you sure you want to update user role");
            if (boolYesNo) {
                $http({
                    method: 'POST',
                    data: requestData,
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/UpdateUserRole"
                }).then(function successCallback(response) {
                    if (response.data == "success") {
                        $scope.getUserClientData();
                        // $scope.getUserClientList();
                        toastr.success("User role updated sucessfully.");
                    } else {
                        toastr.error("User role updation faild");
                    }
                }, function errorCallback(exceptionData) {
                    toastr.error("User role updation faild");
                });
            }
        }
    };
    $scope.OpenCreateClient = function (flag, objClient) {
        $scope.lstMapUsers = [];
        $scope.mappedADUsers = [];
        $scope.lstDeletedMapUser = [];
        $scope.lstDeletedADMapUser = [];
        $scope.IsUpdate = flag;
        var newScope = $scope.$new();
        if ($scope.IsUpdate) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = true;
            });
            newScope.ClientId = objClient.ClientID;
            newScope.ClientName = objClient.ClientName;
            newScope.SDSLicenseType = objClient.SDSLicenseType;
            newScope.SASLicenseType = objClient.SASLicenseType;
            newScope.SMSLicenseType = objClient.SMSLicenseType;
            newScope.ActiveUserLimit = objClient.ActiveUserLimit;
            newScope.Status = objClient.Status;
            newScope.StartDate = objClient.StartDate;
            newScope.EndDate = objClient.EndDate;
            if (objClient.AdminUser) {
                objClient.AdminUser = objClient.AdminUser.replace(/^;/, ""); // remove first semicolon into user list
            }
            // var lstuserId = objClient.AdminUser.split(";");
            $http({
                method: 'POST',
                data: {},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/GetMappedUserData?ownerUsers=" + objClient.AdminUser
            }).then(function successCallback(response) {
                $scope.lstMapUsers = response.data;
                newScope.checkValidClient();
            }, function errorCallback(exceptionData) {
                $rootScope.showExceptionDetails(exceptionData.data);
            });
            $http({
                method: 'POST',
                data: {},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/GetMappedUserData?ownerUsers=" + objClient.AdminUser + "&isADUser=true"
            }).then(function successCallback(response) {
                $rootScope.IsLoading = false;
                $scope.mappedADUsers = response.data;
                newScope.checkValidClient();
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });

            //if (lstuserId.length > 1) {
            //    for (var i = 0; i < lstuserId.length; i++) {
            //        var index = index = $scope.lstUser.map(function (element) { return element.UserID; }).indexOf(parseInt(lstuserId[i]));
            //        $scope.lstMapUsers.push($scope.lstUser[index]);
            //    }
            //}
            //else if (lstuserId.length > 0) {
            //    var index = index = $scope.lstUser.map(function (element) { return element.UserID; }).indexOf(parseInt(lstuserId[0]));
            //    $scope.lstMapUsers.push($scope.lstUser[index]);
            //}
        }
        else {
            newScope.ClientId = null;
        }
        newScope.lstLicensetype = [{ ID: "S", Description: "Standard" }, { ID: "A", Description: "Advanced" }, { ID: "E", Description: "Enterprise" }, { ID: "N", Description: "None" }];
        var dialog = $rootScope.showDialog(newScope, "Create Client", "StartUp/views/CreateClientDialog.html", { width: 683, height: 590 });
        ComponentsPickers.init();
        newScope.AddClient = function () {
            $scope.AdminUser = "";
            var ObjClientInfo = {};
            for (var i = 0; i < $scope.lstMapUsers.length; i++) {
                if ($scope.AdminUser == "") {
                    $scope.AdminUser = $scope.lstMapUsers[i].UserID;
                }
                else {
                    $scope.AdminUser += ";" + $scope.lstMapUsers[i].UserID;
                }

            }

            newScope.successCallback = function (response) {
                $rootScope.IsProjectLoaded = false;
                $scope.lstClient = response.data;
                dialog.close();
                var newScope = $scope.$new();
                if ($scope.IsUpdate) {
                    $scope.getUserClientList("first");
                    toastr.success("Client updated sucessfully.");
                }
                else {
                    $scope.getUserClientList();
                    toastr.success("Client created sucessfully.");
                }
                $scope.IsUpdate = undefined;

            }
            newScope.errorCallback = function (exceptionData) {
                $rootScope.IsProjectLoaded = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            }

            $http({
                method: 'POST',
                data: {
                    ClientID: newScope.ClientId, ClientName: newScope.ClientName, SDSLicenseType: newScope.SDSLicenseType, SASLicenseType: newScope.SASLicenseType,
                    SMSLicenseType: newScope.SMSLicenseType, ActiveUserLimit: newScope.ActiveUserLimit, Status: newScope.Status,
                    StartDate: newScope.StartDate, EndDate: newScope.EndDate, AdminUser: newScope.AdminUser, IsUpdateFlag: $scope.IsUpdate, ActiveDirectoryUsers: newScope.mappedADUsers
                },
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/CreateUpdateClient"
            }).then(newScope.successCallback, newScope.errorCallback);
        };
        newScope.closeDialog = function () {
            dialog.close();
        };
        newScope.selectAdminUser = function (user) {
            newScope.selectedAdminUser = user;
        };
        newScope.selectADOwner = function (user) {
            newScope.selectedADOwner = user;
        };

        newScope.deleteAdminUser = function () {
            var index = $scope.lstMapUsers.indexOf(newScope.selectedAdminUser);
            if (index > -1) {
                if ($scope.lstDeletedMapUser.length > 0) {
                    var tempindex = $scope.lstDeletedMapUser.map(function (element) { return element.UserName; }).indexOf(newScope.selectedAdminUser.UserName);
                    if (tempindex == -1) {
                        $scope.lstDeletedMapUser.push($scope.lstMapUsers[index]);
                    }
                }
                else {
                    $scope.lstDeletedMapUser.push($scope.lstMapUsers[index]);
                }
                $scope.lstMapUsers.splice(index, 1);
                newScope.selectedAdminUser = undefined;
                newScope.checkValidClient();
            }
        };
        newScope.deleteADUserFromOwners = function () {
            var index = $scope.mappedADUsers.indexOf(newScope.selectedADOwner);
            if (index > -1) {
                if ($scope.lstDeletedADMapUser.length > 0) {
                    var tempindex = $scope.lstDeletedADMapUser.map(function (element) { return element.SAMAccountName; }).indexOf(newScope.selectedADOwner.SAMAccountName);
                    if (tempindex == -1) {
                        $scope.lstDeletedADMapUser.push($scope.mappedADUsers[index]);
                    }
                }
                else {
                    $scope.lstDeletedADMapUser.push($scope.mappedADUsers[index]);
                }
                $scope.mappedADUsers.splice(index, 1);
                newScope.selectedADOwner = undefined;
                newScope.checkValidClient();
            }
        };

        newScope.checkValidClient = function () {
            if (newScope.ClientName) {
                if ($scope.lstClient && $scope.lstClient.length > 0) {
                    var index = $scope.lstClient.map(function (element) { return element.ClientName.toLowerCase(); }).indexOf(newScope.ClientName.toLowerCase());
                    if ((index > -1 && !$scope.IsUpdate) || ($scope.IsUpdate && index > -1 && objClient.ClientName.toLowerCase() != newScope.ClientName.toLowerCase())) {
                        newScope.ClientErrorMessage = "Client Name already exist.";
                        newScope.IsClientError = true;
                        return;
                    }
                }
            }
            if (!newScope.ClientName || newScope.ClientName == "") {
                newScope.ClientErrorMessage = "Client Name Required";
                newScope.IsClientError = true;
            }
            else if (!newScope.SDSLicenseType || newScope.SDSLicenseType == "") {
                newScope.ClientErrorMessage = "SDS LicenseType Required";
                newScope.IsClientError = true;
            }
            else if (!newScope.SASLicenseType || newScope.SASLicenseType == "") {
                newScope.ClientErrorMessage = "SAS LicenseType Required";
                newScope.IsClientError = true;
            }
            else if (!newScope.SMSLicenseType || newScope.SMSLicenseType == "") {
                newScope.ClientErrorMessage = "SMS LicenseType Required";
                newScope.IsClientError = true;
            }
            else if (!newScope.ActiveUserLimit || newScope.ActiveUserLimit == "") {
                newScope.ClientErrorMessage = "Active User Limit Required";
                newScope.IsClientError = true;
            }
            else if (newScope.ActiveUserLimit < -1) {
                newScope.ClientErrorMessage = "Active User Limit should be greater than -1";
                newScope.IsClientError = true;
            }
            else if (!newScope.Status || newScope.Status == "") {
                newScope.ClientErrorMessage = "Status Required";
                newScope.IsClientError = true;
            }
            else if (!newScope.StartDate || newScope.StartDate == "") {
                newScope.ClientErrorMessage = "Start Date Required";
                newScope.IsClientError = true;
            }
            else if (!ValidateDate(newScope.StartDate)) {
                newScope.ClientErrorMessage = "Start Date should be mm/dd/yyyy or mm-dd-yyyy format";
                newScope.IsClientError = true;
            }
            else if (new Date() < Date.parse(newScope.StartDate)) {
                newScope.ClientErrorMessage = "Start Date should be less than Or equal to current Date";
                newScope.IsClientError = true;
            }
            else if (newScope.EndDate && !ValidateDate(newScope.EndDate)) {
                newScope.ClientErrorMessage = "End Date should be mm/dd/yyyy or mm-dd-yyyy";
                newScope.IsClientError = true;
            }
            else if (newScope.EndDate && Date.parse(newScope.StartDate) > Date.parse(newScope.EndDate)) {
                newScope.ClientErrorMessage = "Start Date should be greater than End Date";
                newScope.IsClientError = true;
            }
            else if ((!$scope.lstMapUsers || $scope.lstMapUsers.length == 0) && (!$scope.mappedADUsers || $scope.mappedADUsers.length == 0)) {
                newScope.ClientErrorMessage = "At least one owner required.";
                newScope.IsClientError = true;
            }
            else {
                newScope.ClientErrorMessage = "";
                newScope.IsClientError = false;
            }
        };
        newScope.MapUsers = function () {
            var newMapScope = $scope.$new();
            newMapScope.lstUser = [];
            $http({
                method: 'POST',
                data: {},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                async: false,
                url: "api/Login/GetUnMappedUserData"
            }).then(
                function successCallback(response) {
                    newMapScope.lstUser = [];
                    newMapScope.lstUser = response.data;
                    if ($scope.lstMapUsers.length > 0 && newMapScope.lstUser.length > 0) {
                        for (var i = 0; i < $scope.lstMapUsers.length; i++) {
                            var index = newMapScope.lstUser.map(function (element) { return element.UserName; }).indexOf($scope.lstMapUsers[i].UserName);
                            if (index > -1) {
                                newMapScope.lstUser.splice(index, 1);
                            }
                        }
                    }
                    if ($scope.lstDeletedMapUser.length > 0) {
                        for (var i = 0; i < $scope.lstDeletedMapUser.length; i++) {
                            var index = newMapScope.lstUser.map(function (element) { return element.UserName; }).indexOf($scope.lstDeletedMapUser[i].UserName);
                            if (index == -1) {
                                newMapScope.lstUser.push($scope.lstDeletedMapUser[i]);
                            }
                        }
                    }

                },
                function errorCallback(exceptionData) {
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            //newScope.lstUser = $scope.lstUser;
            dialogMapuser = $rootScope.showDialog(newMapScope, "Assign User", "StartUp/views/ClientUserAssociationDialog.html");
            newMapScope.lstTempMapUser = [];
            newMapScope.OkClick = function () {
                for (var i = 0; i < newMapScope.lstTempMapUser.length; i++) {
                    var index = $scope.lstMapUsers.map(function (element) { return element.UserID; }).indexOf(newMapScope.lstTempMapUser[i].UserID);
                    if (index < 0) {
                        $scope.lstMapUsers.push(newMapScope.lstTempMapUser[i]);
                        if ($scope.lstDeletedMapUser.length > 0) {
                            var index = $scope.lstDeletedMapUser.map(function (element) { return element.UserName; }).indexOf(newMapScope.lstTempMapUser[i].UserName);
                            if (index > -1) {
                                $scope.lstDeletedMapUser.splice(index, 1);
                            }
                        }
                        newScope.checkValidClient();
                    }
                }
                newMapScope.closeDialog();
            };
            newMapScope.closeDialog = function () {
                dialogMapuser.close();
            };
            newMapScope.CheckUserMap = function (isSelected, objUser) {
                if (isSelected) {
                    newMapScope.lstTempMapUser.push({ UserID: objUser.UserID, UserName: objUser.UserName });
                }
                else {
                    var index = newMapScope.lstTempMapUser.map(function (element) { return element.UserID; }).indexOf(objUser.UserID);
                    if (index > -1) {
                        newMapScope.lstTempMapUser.splice(index, 1);
                    }
                }
            };

        };
        newScope.mapADUsers = function () {
            var newMapScope = $scope.$new();
            newMapScope.searchADUser = function () {
                $rootScope.IsLoading = true;
                $http({
                    method: 'POST',
                    data: { samAccounName: newMapScope.samAccountName, domainName: newMapScope.domainName, name: newMapScope.name, email: newMapScope.email },
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    async: false,
                    url: "api/Login/GetUnMappedADUserData"
                }).then(function successCallback(response) {
                    $rootScope.IsLoading = false;
                    newMapScope.filterADUsers = response.data;
                    if ($scope.lstDeletedADMapUser.length > 0) {
                        for (var i = 0; i < $scope.lstDeletedADMapUser.length; i++) {
                            var adSelectedUser = newMapScope.filterADUsers.filter(function (adUser) {
                                return adUser.SAMAccountName.toLowerCase() == $scope.lstDeletedADMapUser[i].SAMAccountName.toLowerCase()
                                    && adUser.Name.toLowerCase() == $scope.lstDeletedADMapUser[i].Name.toLowerCase()
                                    && adUser.DomainName.toLowerCase() == $scope.lstDeletedADMapUser[i].DomainName.toLowerCase()
                            });
                            var adcurrentUser = $scope.lstDeletedADMapUser[i];
                            if (adSelectedUser.length == 0 && adcurrentUser.SAMAccountName.toLowerCase().contains(newMapScope.samAccountName.toLowerCase())
                                && adcurrentUser.Name.toLowerCase().contains(newMapScope.name.toLowerCase()) && adcurrentUser.DomainName.toLowerCase().contains(newMapScope.domainName.toLowerCase())) {
                                newMapScope.filterADUsers.push($scope.lstDeletedADMapUser[i]);
                            }
                        }
                    }
                    newMapScope.removeMappedADUser(newMapScope.selectedADUsers, newMapScope.filterADUsers);
                    if ($scope.mappedADUsers && $scope.mappedADUsers.length > 0) {
                        newMapScope.removeMappedADUser($scope.mappedADUsers, newMapScope.filterADUsers);
                    }

                    newMapScope.selectedUserDisplayCount = newMapScope.filterUserDisplayCount = 30;
                    // newMapScope.samAccountName = newMapScope.name = newMapScope.domainName = "";

                    newMapScope.filterUserDisplayCount = 30;
                }, function errorCallback(exceptionData) {
                    $rootScope.IsLoading = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            }

            newMapScope.OK = function () {
            newMapScope.beforeOK();
                for (var idx = 0; idx < newMapScope.selectedADUsers.length; idx++) {
                    $scope.mappedADUsers.push(newMapScope.selectedADUsers[idx]);
                    if ($scope.lstDeletedMapUser.length > 0) {
                        var index = $scope.lstDeletedMapUser.map(function (element) { return element.SAMAccountName; }).indexOf(newMapScope.selectedADUsers[idx].SAMAccountName);
                        if (index > -1) {
                            $scope.lstDeletedMapUser.splice(index, 1);
                        }
                    }
                }
                newScope.checkValidClient();
                newMapScope.closeDialog();
            }
            newMapScope.dialogMapuser = $rootScope.showDialog(newMapScope, "Assign Active Directory User", "StartUp/views/GetActiveDirectoryUsers.html", { width: 1200, height: 550 });

        }
        newScope.checkValidClient();
    };
    $scope.DeleteClient = function () {
        if ($scope.selectedClient) {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: {},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/DeleteClient?clientID=" + $scope.selectedClient.ClientID
            }).then(function successCallback(response) {
                $scope.lstUser = [];
                $scope.lstClient = [];
                $rootScope.IsLoading = false;
                if (response.data[0].length > 0) {
                    $scope.lstUser = response.data[0];
                }
                if (response.data[1].length > 0) {
                    $scope.lstClient = response.data[1];
                    $scope.selectedClient = $scope.lstClient[$scope.lstClient.length - 1];
                }
                $scope.getUserClientList("first");
                toastr.success("Client Deleted sucessfully.");
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
    };


    $scope.ChangePassword = function () {
        var userCookie = $cookies.getObject("UserDetails");
        if (userCookie && JSON.parse(userCookie).UserName != null) {
            var newScope = $scope.$new(true);
            newScope.showOldPassword = true;
            newScope.userDetails = JSON.parse(userCookie);
            newScope.changePasswordDialog = $rootScope.showDialog(newScope, "Change Password", "StartUp/views/ChangePasswordDialog.html", { height: 440, width: 500 });
            newScope.changePasswordCallBack = function (aObjResponse) {
                toastr.success("Password changed successfully.");
                newScope.closeChangePasswordDialog();
            };
            newScope.closeChangePasswordDialog = function () {
                newScope.changePasswordDialog.close();
            };
        }

    };
    $scope.selectUserRow = function (_userInfo) {
        $scope.selectedUser = _userInfo;
        $scope.selectClientRow($scope.selectedUser.lstClients[0]);
    };
    $scope.selectClientRow = function (_clientInfo) {
        $scope.selectedClient = _clientInfo;
    };
    $scope.openAddUserDialog = function (userOperationType,user) {
        var newScopeUser = $scope.$new(true);
        newScopeUser.userOperationType = userOperationType;
        newScopeUser.selectedUser = { UserName: '', FirstName: '', LastName: '', DomainName: '' };
        if (newScopeUser.userOperationType == "newUser") {
            newScopeUser.saveButtonLabel = "Create User";
        } else if (newScopeUser.userOperationType == "updateUser" && user) {
            $scope.selectUserRow(user);
            angular.copy($scope.selectedUser, newScopeUser.selectedUser);
            newScopeUser.saveButtonLabel = "Update User";
        }
       
        newScopeUser.CreateUserOkClick = function () {
            var index = -1;
            if ($scope.lstUser.length > 0) {
                index = $scope.lstUser.map(function (element) { return element.UserName; }).indexOf(newScopeUser.UserName);
            }
            if (index < 0 || newScopeUser.userOperationType == "updateUser") {
                $http({
                    method: 'POST',
                    data: [{ UserName: newScopeUser.selectedUser.UserName, FirstName: newScopeUser.selectedUser.FirstName, LastName: newScopeUser.selectedUser.LastName, EmailId: newScopeUser.selectedUser.EmailId, saveType: newScopeUser.userOperationType }],
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/CreateUser"
                }).then(function successCallback(response) {
                    $rootScope.IsProjectLoaded = false;
                    if (response.data != "UserExist") {
                        $scope.lstUser = response.data;
                        newScopeUser.dialog.close();
                        if (newScopeUser.userOperationType == "updateUser") {
                            toastr.success("User details updated sucessfully.");
                        } else {
                            $scope.getUserClientList("last");
                            toastr.success("User created sucessfully.");
                        }
                    } else {
                        toastr.error("User already exist.");
                    }

                }, function errorCallback(exceptionData) {
                    $rootScope.IsProjectLoaded = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            }
            else {
                toastr.error("User already exist.");
            }
        };
        newScopeUser.CheckValidUser = function () {
            if (!newScopeUser.selectedUser.UserName && newScopeUser.userOperationType == "newUser") {
                newScopeUser.ErrorMessage = "User Name Required";
                newScopeUser.IsError = true;
            }
            else if (!validateEmail(newScopeUser.selectedUser.UserName) && newScopeUser.userOperationType == "newUser") {
                newScopeUser.ErrorMessage = "Invalid username.";
                newScopeUser.IsError = true;
            }
            else if (!newScopeUser.selectedUser.FirstName) {
                newScopeUser.ErrorMessage = "First Name Required";
                newScopeUser.IsError = true;
            }
            else if (!newScopeUser.selectedUser.LastName) {
                newScopeUser.ErrorMessage = "Last Name Required";
                newScopeUser.IsError = true;
            }
            else if (newScopeUser.userOperationType == "updateUser" && (!newScopeUser.selectedUser.EmailId || !validateEmail(newScopeUser.selectedUser.EmailId))) {
                newScopeUser.ErrorMessage = "Invalid emailid";
                newScopeUser.IsError = true;
            }
            else {
                newScopeUser.ErrorMessage = "";
                newScopeUser.IsError = false;
            }
        };
        newScopeUser.closeAddUserDialog = function () {
            newScopeUser.dialog.close();
        };
        newScopeUser.dialog = $rootScope.showDialog(newScopeUser, newScopeUser.saveButtonLabel, "StartUp/views/add_new_user.html", { width: 650, height: 350 });
        newScopeUser.CheckValidUser();
    };
    $scope.deleteUser = function () {
        
    };

    $scope.showADUsers = function () {
        var newMapScope = $scope.$new();
        newMapScope.IsShowRole = false;
        newMapScope.newAddedADUsers = [];
        newMapScope.OK = function () {
            newMapScope.beforeOK();
            for (var idx = 0; idx < newMapScope.selectedADUsers.length; idx++) {
                var tempAdUserObj = {
                    UserName: newMapScope.selectedADUsers[idx].SAMAccountName,
                    FirstName: newMapScope.selectedADUsers[idx].FirstName,
                    LastName: newMapScope.selectedADUsers[idx].LastName,
                    EmailId: newMapScope.selectedADUsers[idx].EmailId,
                    DomainName: newMapScope.selectedADUsers[idx].DomainName,
                    saveType: "newUser",
                };
               
                newMapScope.newAddedADUsers.push(tempAdUserObj);
            }
            if (newMapScope.selectedADUsers.length && newMapScope.newAddedADUsers.length > 0) {
                $http({
                    method: 'POST',
                    data: newMapScope.newAddedADUsers,
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/CreateUser"
                }).then(function successCallback(response) {
                    $rootScope.IsProjectLoaded = false;
                    if (response.data != "UserExist") {
                        $scope.lstUser = response.data;
                        $scope.getUserClientList("last");
                        newMapScope.dialogMapuser.close();
                        toastr.success("User created sucessfully.");
                    } else {
                        toastr.error("User already exist.");
                    }

                }, function errorCallback(exceptionData) {
                    $rootScope.IsProjectLoaded = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            }
        };
        newMapScope.removeExistingADUser = function (lstAllADUser) {
            if (angular.isArray($scope.lstAllUsers) && $scope.lstAllUsers.length) {
                for (var idx = 0; idx < $scope.lstAllUsers.length; idx++) {
                    var adSelectedUser = lstAllADUser.filter(function (adUser) {
                        return adUser.SAMAccountName.toLowerCase() == $scope.lstAllUsers[idx].UserName.toLowerCase()
                            && adUser.DomainName.toLowerCase() == $scope.lstAllUsers[idx].DomainName.toLowerCase()
                    });
                    if (adSelectedUser.length > 0) {
                        var adSelectedUserIndex = lstAllADUser.indexOf(adSelectedUser[0]);
                        if (adSelectedUserIndex > -1) {
                            lstAllADUser.splice(adSelectedUserIndex, 1);
                        }
                    }
                }
            }
        };
        newMapScope.searchADUser = function () {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: { userID: JSON.parse(userCookie).UserID,samAccounName: newMapScope.samAccountName, domainName: newMapScope.domainName, name: newMapScope.name, email: newMapScope.email },
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                async: false,
                url: "api/ConfigureSetting/GetADUsersForMapping"
            }).then(function successCallback(response) {
                $rootScope.IsLoading = false;
                newMapScope.filterADUsers = response.data;
                newMapScope.removeExistingADUser(newMapScope.filterADUsers);

                newMapScope.selectedUserDisplayCount = newMapScope.filterUserDisplayCount = 30;
                newMapScope.filterUserDisplayCount = 30;
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        };
        newMapScope.dialogMapuser = $rootScope.showDialog(newMapScope, "Add Active Directory User", "StartUp/views/GetActiveDirectoryUsers.html", { width: 1200, height: 550 });
    };
    $scope.getUserClientData();
    $scope.getUserClientList("first");
}]);