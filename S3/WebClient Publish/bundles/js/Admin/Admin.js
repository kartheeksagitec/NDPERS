app.controller("AdminController", ["$scope", "$http", "$rootScope", "ConfigurationFactory", "$cookies", "$timeout", "$SgMessagesService", function ($scope, $http, $rootScope, ConfigurationFactory, $cookies, $timeout, $SgMessagesService) {
    //#region Variable Declarations
    $scope.ilstClients = [];
    $scope.iobjSelectedClient = null;
    $scope.ilstUsers = [];
    $scope.iobjSelecteduser = null;

    $scope.ilstAllUsers = [];
    $scope.iobjUserTabSelectedUser = null;
    $scope.iobjUserTabSelectedClient = null;

    $scope.isRefreshRequired = true;
    var amonth = new Date().getUTCMonth() + 1;
    $scope.iobjNewClient = {
        ClientID: -1,
        ClientName: "",
        SDSLicenseType: "S",
        SASLicenseType: "S",
        SMSLicenseType: "S",
        ActiveUserLimit: -1,
        Status: "active",
        StartDate: amonth  + "/" + new Date().getUTCDate() + "/" + new Date().getUTCFullYear(),
        EndDate: null,
        Domains: [],
        UserTypes: []
    };
    $scope.iobjNewDomain = {
        DomainID: -1,
        DomainName: "",
        LDAPPath: "",
    }
    $scope.ilstLicenseTypes = [{ ID: "S", Description: "Standard" }, { ID: "A", Description: "Advanced" }, { ID: "E", Description: "Enterprise" }, { ID: "N", Description: "None" }];
    //endregion

    //#region Initialization
    $scope.init = function () {
        //get user details from cookie and set login name to display on header.
        var userCookie = $cookies.getObject("UserDetails");
        $scope.LoginName = JSON.parse(userCookie).UserName;

        $scope.getClients();
        $scope.getUserInfo();
    }

    //#endregion

    //#region Client Management
    $scope.getClients = function () {
        $http({
            method: 'POST',
            data: {},
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Admin/GetClients"
        }).then(function successCallback(response) {
            $rootScope.IsProjectLoaded = false;
            if (response.data && response.data.length) {
                $scope.ilstClients = response.data;
                //$scope.ilstUsers = response.data[0];
            }
            //if (response.data[1].length > 0) {
            //    $scope.$evalAsync(function () {
            //        $scope.ilstClients = response.data[1];
            //    });
            //}

        }, function errorCallback(exceptionData) {
            $rootScope.IsProjectLoaded = false;
            $rootScope.showExceptionDetails(exceptionData.data);
        });
    };
    $scope.saveClient = function (aobjClient, stId, IsUpdate) {
        $.ajax({
            url: "api/Admin/SaveClient",
            type: 'POST',
            async: true,
            dataType: 'json',
            data: aobjClient,
            success: function (response) {
                if (response && aobjClient.ClientID === -1) {
                    aobjClient.ClientID = response;
                }
                if (!IsUpdate) {
                    $scope.$broadcast('stAddRowBroadcast', { objAdded: aobjClient, stid: stId });
                }
                else {
                    $scope.$broadcast('stUpdateRowBroadcast', { objUpdated: aobjClient, stid: stId });
                }
            },
            error: function (exceptionData) {
                $rootScope.showExceptionDetails(exceptionData);
            }
        });
    }
    $scope.deleteClient = function (updatedobject) {
        if (updatedobject) {
            $SgMessagesService.Message('Delete', "ClientID :" + updatedobject.ClientName + " will be deleted, Do you want to continue?", true, function (result) {
                if (result) {
                    if (updatedobject && (!updatedobject.Domains || updatedobject.Domains.length == 0) && (!updatedobject.UserTypes || updatedobject.UserTypes == 0)) {
                        $.ajax({
                            url: "api/Login/DeleteClient",
                            type: 'POST',
                            async: true,
                            dataType: 'json',
                            data: updatedobject,
                            success: function (response) {
                                $scope.$broadcast('stDeleteRowBroadcast', { objDeleted: updatedobject });
                            },
                            error: function (exceptionData) {
                                $rootScope.showExceptionDetails(exceptionData);
                            }
                        });
                    } else {
                        $SgMessagesService.Message("Alert", "Please un-map users and domains for this client");
                    }
                }
            });
        }
    };
    $scope.selectClient = function (aobjClient) {
        $scope.iobjSelectedClient = aobjClient;
        $scope.iobjNewDomain.ClientID = aobjClient.ClientID;
    };

    $scope.initializeComponentPicker = function () {
        ComponentsPickers.init();
    };
    $scope.validateClient = function (aobjClient) {

        $scope.iobjClientValidation = "";

        if (aobjClient) {
            if (aobjClient.ClientName) {
                for (var i = 0; i < $scope.ilstClients.length; i++) {
                    if ($scope.ilstClients[i].ClientName && aobjClient.ClientName.toLowerCase() == $scope.ilstClients[i].ClientName.toLowerCase() && aobjClient.ClientID != $scope.ilstClients[i].ClientID) {
                        $scope.iobjClientValidation = "Client Name cannot be duplicate."
                        break;
                    }
                }
            }
            if (!(aobjClient.ClientName && aobjClient.ClientName.trim().length > 0)) {
                $scope.iobjClientValidation = "Client Name cannot be empty."
            }
            else if (!(aobjClient.SDSLicenseType && aobjClient.SDSLicenseType.trim().length > 0)) {
                $scope.iobjClientValidation = "SDS License Type cannot be empty."
            }
            else if (!(aobjClient.SASLicenseType && aobjClient.SASLicenseType.trim().length > 0)) {
                $scope.iobjClientValidation = "SAS License Type cannot be empty."
            }
            else if (!(aobjClient.SMSLicenseType && aobjClient.SMSLicenseType.trim().length > 0)) {
                $scope.iobjClientValidation = "SMS License Type cannot be empty."
            }
            else if (!aobjClient.ActiveUserLimit) {
                $scope.iobjClientValidation = "Active User Limit cannot be empty or 0. Set -1 for unlimited."
            }
            else if (!(aobjClient.Status && aobjClient.Status.trim().length > 0)) {
                $scope.iobjClientValidation = "Status cannot be empty."
            }
            else if (!(aobjClient.StartDate && aobjClient.StartDate.trim().length > 0)) {
                $scope.iobjClientValidation = "Start Date cannot be empty."
            }
        }
    }
    //#endregion

    //#region Domain Management
    $scope.saveDomain = function (aobjDomain) {
        $.ajax({
            url: "api/Admin/SaveDomainClientMapping",
            type: 'POST',
            async: true,
            dataType: 'json',
            data: aobjDomain,
            success: function (response) {
                if (response) {
                    if (response === "DUPLICATE_DOMAIN") {
                        toastr.error("Domain Name with a different LDAP Path already exists.");
                    }
                    else {
                        if (aobjDomain.DomainID === -1) {
                            $scope.$evalAsync(function () {
                                aobjDomain.DomainID = response;
                                $scope.selectDomain(aobjDomain);
                            });
                        }
                    }
                }
            },
            error: function (exceptionData) {
                $rootScope.showExceptionDetails(exceptionData);
            }
        });
    }
    $scope.deleteDomain = function () {
        //Make ajax call and delete domain, if done, then update the domain list
        if ($scope.iobjSelectedDomain) {
            $SgMessagesService.Message('Delete', "Domain Name :" + $scope.iobjSelectedDomain.DomainName + " will be deleted, Do you want to continue?", true, function (result) {
                if (result) {
                    $rootScope.IsLoading = true;
                    $http({
                        method: 'POST',
                        data: { clientID: $scope.iobjSelectedClient.ClientID, domainID: $scope.iobjSelectedDomain.DomainID },
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        async: false,
                        url: "api/Admin/DeleteDomain"
                    }).then(function successCallback(response) {
                        if (response.data && response.data == "SUCCESS") {
                            $scope.$evalAsync(function () {
                                var index = $scope.iobjSelectedClient.Domains.indexOf($scope.iobjSelectedDomain);
                                if (index > -1) {
                                    $scope.iobjSelectedClient.Domains.splice(index, 1);
                                }
                                $scope.iobjSelectedDomain = null;
                            });
                        }
                        $rootScope.IsLoading = false;
                    }, function errorCallback(exceptionData) {
                        $rootScope.IsLoading = false;
                        $rootScope.showExceptionDetails(exceptionData.data);
                    });
                }
            });
        }
    };

    $scope.selectDomain = function (aobjDomain) {
        $scope.iobjSelectedDomain = aobjDomain;
    }
    $scope.validateDomain = function (aobjDomain) {
        $scope.domainValidationMessage = ""
        if (aobjDomain) {
            if (!(aobjDomain.DomainName && aobjDomain.DomainName.trim().length > 0)) {
                $scope.domainValidationMessage = "Domain Name cannot be empty."
            }
            else if (!(aobjDomain.LDAPPath && aobjDomain.LDAPPath.trim().length > 0)) {
                $scope.domainValidationMessage = "LDAP Path cannot be empty."
            }
        }
    }
    //#endregion


    //#region User Management

    //$scope.unmapUser = function () {

    //    $rootScope.IsLoading = true;
    //    $http({
    //        method: 'POST',
    //        data: { clientID: $scope.iobjSelectedClient.ClientID, userID: $scope.iobjSelectedUser.User.UserID },
    //        headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //        async: false,
    //        url: "api/Admin/UnMapUserFromClient"
    //    }).then(function successCallback(response) {
    //        if (response.data && response.data == "SUCCESS") {
    //            var index = $scope.iobjSelectedClient.UserTypes.indexOf($scope.iobjSelectedUser);
    //            if (index > -1) {
    //                $scope.iobjSelectedClient.UserTypes.splice(index, 1);
    //            }
    //        }
    //        $rootScope.IsLoading = false;
    //    }, function errorCallback(exceptionData) {
    //        $rootScope.IsLoading = false;
    //        $rootScope.showExceptionDetails(exceptionData.data);
    //    });
    //}
    //$scope.selectUser = function (aobjUser) {
    //    $scope.iobjSelectedUser = aobjUser;
    //}
    //$scope.mapUsers = function () {
    //    if ($scope.iobjSelectedClient) {
    //        var newMapScope = $scope.$new();
    //        newMapScope.lstUnMappedUsers = [];
    //        newMapScope.llstSelectedUsers = [];
    //        $http({
    //            method: 'POST',
    //            data: {},
    //            headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //            async: false,
    //            url: "api/Admin/GetUsersForOwnerMapping?aintClientID=" + $scope.iobjSelectedClient.ClientID
    //        }).then(
    //            function successCallback(response) {
    //                if (response && response.data) {
    //                    newMapScope.lstUnMappedUsers = response.data;
    //                }
    //            },
    //            function errorCallback(exceptionData) {
    //                $rootScope.showExceptionDetails(exceptionData.data);
    //            });
    //        newMapScope.selectAllUsers = function () {
    //            for (var i = 0, len = newMapScope.lstUnMappedUsers.length; i < len; i++) {
    //                newMapScope.llstSelectedUsers.push(newMapScope.lstUnMappedUsers[i]);
    //                newMapScope.lstUnMappedUsers = [];
    //            }
    //        };
    //        newMapScope.selectUser = function (aobjUser) {
    //            var index = newMapScope.lstUnMappedUsers.indexOf(aobjUser);
    //            newMapScope.lstUnMappedUsers.splice(index, 1);
    //            newMapScope.llstSelectedUsers.push(aobjUser);
    //        }
    //        newMapScope.unSelectAllUsers = function () {
    //            for (var i = 0, len = newMapScope.llstSelectedUsers.length; i < len; i++) {
    //                newMapScope.lstUnMappedUsers.push(newMapScope.llstSelectedUsers[i]);
    //                newMapScope.llstSelectedUsers = [];
    //            }
    //        }
    //        newMapScope.unSelectUser = function (aobjUser) {
    //            var index = newMapScope.llstSelectedUsers.indexOf(aobjUser);
    //            newMapScope.llstSelectedUsers.splice(index, 1);
    //            newMapScope.lstUnMappedUsers.push(aobjUser);
    //        }
    //        dialogMapuser = $rootScope.showDialog(newMapScope, "Assign User", "StartUp/views/ClientUserAssociationDialog.html", { minHeight: 600, minWidth: 1100 });
    //        newMapScope.OkClick = function () {
    //            //Update user mapping for current client.
    //            //Make ajax call to save mappings in db.

    //            $scope.saveUserClientMapping(newMapScope.llstSelectedUsers.map(function (item) {
    //                return {
    //                    User: item,
    //                    UserType: 'owner',
    //                    StartDate: new Date().getUTCMonth() + "/" + new Date().getUTCDate() + "/" + new Date().getUTCFullYear(),
    //                    EndDate: null,
    //                    ClientID: $scope.iobjSelectedClient.ClientID,
    //                };
    //            }));
    //            newMapScope.closeDialog();
    //        };
    //        newMapScope.closeDialog = function () {
    //            dialogMapuser.close();
    //        };
    //    }
    //}
    //$scope.mapADUsers = function () {
    //    var newMapScope = $scope.$new();
    //    newMapScope.searchADUser = function () {
    //        $rootScope.IsLoading = true;
    //        $http({
    //            method: 'POST',
    //            data: { clientID: $scope.iobjSelectedClient.ClientID, samAccounName: newMapScope.samAccountName, domainName: newMapScope.domainName, name: newMapScope.name, email: newMapScope.email },
    //            headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //            async: false,
    //            url: "api/Admin/GetADUsersForOwnerMapping"
    //        }).then(function successCallback(response) {
    //            $rootScope.IsLoading = false;
    //            newMapScope.llstUnMappedADUsers = response.data;
    //        }, function errorCallback(exceptionData) {
    //            $rootScope.IsLoading = false;
    //            $rootScope.showExceptionDetails(exceptionData.data);
    //        });
    //    }
    //    newMapScope.OK = function () {
    //        newMapScope.beforeOK();

    //        $scope.saveUserClientMapping(newMapScope.selectedADUsers.map(function (item) {
    //            return {
    //                User: item,
    //                UserType: 'owner',
    //                StartDate: new Date().getUTCMonth() + "/" + new Date().getUTCDate() + "/" + new Date().getUTCFullYear(),
    //                EndDate: null,
    //                ClientID: $scope.iobjSelectedClient.ClientID,
    //            };
    //        }));

    //        //Make ajax call to save mappings in db.

    //        newMapScope.closeDialog();
    //    }
    //    newMapScope.dialogMapuser = $rootScope.showDialog(newMapScope, "Assign Active Directory User", "StartUp/views/GetActiveDirectoryUsers.html", { width: 1200, height: 600 });

    //}
    //$scope.saveUserClientMapping = function (alstUserClientMapping) {
    //    $.ajax({
    //        url: "api/Admin/SaveUserClientMapping",
    //        type: 'POST',
    //        async: true,
    //        data: JSON.stringify(alstUserClientMapping),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        success: function (response) {
    //            $scope.$evalAsync(function () {
    //                if (response && response.length > 0) {
    //                    for (var i = 0, len = response.length; i < len; i++) {
    //                        $scope.iobjSelectedClient.UserTypes.push(response[i]);
    //                    }
    //                }
    //            });
    //        },
    //        error: function (exceptionData) {
    //            $rootScope.showExceptionDetails(exceptionData);
    //        }
    //    });
    //}
    //#endregion

    //#User Tab
    $scope.getUserInfo = function () {
        if ($scope.isRefreshRequired) {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: {},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Admin/GetUserInfo"
            }).then(function successCallback(response) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading == false;
                    if (response.data && response.data.length > 0) {
                        $scope.ilstAllUsers = response.data;
                        $scope.isRefreshRequired = false;
                        //$scope.selectUserTabUser($scope.ilstAllUsers[0]);
                    }
                });
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
    };

    $scope.addUserTabNewUser = function () {
        var newUserScope = $scope.$new(true);
        newUserScope.selectedUser = { UserID: -1, UserName: "", FirstName: "", LastName: "", EmailID: "" };
        newUserScope.createUser = function () {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: {
                    UserID: newUserScope.selectedUser.UserID, UserName: newUserScope.selectedUser.UserName, FirstName: newUserScope.selectedUser.FirstName, LastName: newUserScope.selectedUser.LastName, EmailId: newUserScope.selectedUser.EmailId
                },
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Admin/CreateNewUser"
            }).then(function successCallback(response) {
                $rootScope.IsLoading = false;
                if (response.data == -1) {
                    toastr.error("User already exist.");
                }
                else {
                    newUserScope.closeAddUserDialog();
                    toastr.success("User created sucessfully.");
                    if (response.data && response.data.length > 0) {
                        $scope.ilstAllUsers = response.data;
                        $scope.selectUserTabUser($scope.ilstAllUsers[0]);
                    }
                }

            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        };
        newUserScope.checkValidUser = function () {
            if (!newUserScope.selectedUser.UserName) {
                newUserScope.ErrorMessage = "User Name Required";
                newUserScope.IsError = true;
            }
            else if (!newUserScope.selectedUser.FirstName) {
                newUserScope.ErrorMessage = "First Name Required";
                newUserScope.IsError = true;
            }
            else if (!newUserScope.selectedUser.LastName) {
                newUserScope.ErrorMessage = "Last Name Required";
                newUserScope.IsError = true;
            }
            else if (!newUserScope.selectedUser.EmailId) {
                newUserScope.ErrorMessage = "EmailId Required";
                newUserScope.IsError = true;
            }
            else if (!validateEmail(newUserScope.selectedUser.EmailId)) {
                newUserScope.ErrorMessage = "Invalid emailid";
                newUserScope.IsError = true;
            }
            else {
                newUserScope.ErrorMessage = "";
                newUserScope.IsError = false;
            }
        };
        newUserScope.closeAddUserDialog = function () {
            newUserScope.dialog.close();
        };
        newUserScope.dialog = $rootScope.showDialog(newUserScope, "Create User", "Admin/views/AddNewUser.html", { width: 650, height: 350 });
        newUserScope.checkValidUser();
    };

    $scope.selectUserTabUser = function (aobjUser) {
        $scope.iobjUserTabSelectedUser = aobjUser;
        $scope.iobjUserTabSelectedClient = null;
        if (aobjUser.Clients && aobjUser.Clients.length > 0) {
            $scope.selectUserTablClient(aobjUser.Clients[0]);
        }
    };

    $scope.selectUserTablClient = function (aobjClient) {
        $scope.iobjUserTabSelectedClient = aobjClient;
    };

    $scope.reSendActivationMailForUser = function (user) {        
        if (user && user.UserID) {            
            $http({
                method: 'POST',
                data: {UserID: user.UserID},
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Admin/ReSendActivationMail"
            }).then(function successCallback(response) {
                if (response.data && response.data.length) {

                }
               
            }, function errorCallback(exceptionData) {                
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
        else {
            $rootScope.showExceptionDetails('User ID not found.');
        }
    }
    //#endregion
    $scope.init();
}]);