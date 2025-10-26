app.directive("userTypeList", ["$compile", "$rootScope", "$http", "$SgMessagesService", "$timeout", function ($compile, $rootScope, $http, $SgMessagesService, $timeout) {
    return {
        restrict: "E",
        scope: {
            clientId: "=",
            userTypes: "=",
            isAdminMapping: "=",
            callBackFunction: "&",
            isRefreshRequired: '='
        },
        replace: true,
        link: function (scope, element, attrs) {
            //#region User Management

            scope.iobjSelectedUser = null;
            scope.lstUserType = [{ ID: "owner", Description: "Owner" }, { ID: "developer", Description: "Developer" }, { ID: "ba", Description: "BA" }, { ID: "viewer", Description: "Viewer" }];
            scope.lstStatus = [{ ID: "Active", Value: 'active' }, { ID: "InActive", Value: 'inactive' }];
            scope.unmapUser = function () {
                if (scope.iobjSelectedUser) {
                    $SgMessagesService.Message('Delete', "User Name :" + scope.iobjSelectedUser.User.UserName + " will be deleted, Do you want to continue?", true, function (result) {
                        if (result) {
                            $rootScope.IsLoading = true;
                            $http({
                                method: 'POST',
                                data: { clientID: scope.clientId, userID: scope.iobjSelectedUser.User.UserID },
                                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                                async: false,
                                url: "api/Admin/UnMapUserFromClient"
                            }).then(function successCallback(response) {
                                if (response.data && response.data == "SUCCESS") {
                                    var cindex = scope.userTypes.indexOf(scope.iobjSelectedUser);
                                    scope.isRefreshRequired = true;
                                    scope.$broadcast('stDeleteRowBroadcast', { objDeleted: scope.iobjSelectedUser });
                                    if (cindex > scope.userTypes.length - 1) {
                                        cindex -= 1;
                                    }
                                    if (scope.userTypes && scope.userTypes.length) {
                                        scope.selectUser(scope.userTypes[cindex]);
                                    }
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

            scope.selectUser = function (aobjUser) {
                scope.iobjSelectedUser = aobjUser;
            }
            scope.mapUsers = function () {
                if (scope.clientId) {
                    var newMapScope = scope.$new();
                    newMapScope.IsShowRole = !scope.isAdminMapping;
                    newMapScope.lstUnMappedUsers = [];
                    newMapScope.llstSelectedUsers = [];
                    $http({
                        method: 'POST',
                        data: { clientId: scope.clientId, adminMapping: scope.isAdminMapping },
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        async: false,
                        url: "api/Admin/GetUsersForOwnerMapping"
                    }).then(
                        function successCallback(response) {
                            if (response && response.data) {
                                newMapScope.lstUnMappedUsers = response.data;
                            }
                        },
                        function errorCallback(exceptionData) {
                            $rootScope.showExceptionDetails(exceptionData.data);
                        });
                    newMapScope.selectAllUsers = function () {
                        for (var i = 0, len = newMapScope.lstUnMappedUsers.length; i < len; i++) {
                            newMapScope.llstSelectedUsers.push(newMapScope.lstUnMappedUsers[i]);
                        }
                        newMapScope.lstUnMappedUsers = [];
                    };
                    newMapScope.selectUser = function (aobjUser) {
                        var index = newMapScope.lstUnMappedUsers.indexOf(aobjUser);
                        newMapScope.lstUnMappedUsers.splice(index, 1);
                        newMapScope.llstSelectedUsers.push(aobjUser);
                    }
                    newMapScope.unSelectAllUsers = function () {
                        for (var i = 0, len = newMapScope.llstSelectedUsers.length; i < len; i++) {
                            newMapScope.lstUnMappedUsers.push(newMapScope.llstSelectedUsers[i]);
                        }
                        newMapScope.llstSelectedUsers = [];
                    }
                    newMapScope.unSelectUser = function (aobjUser) {
                        var index = newMapScope.llstSelectedUsers.indexOf(aobjUser);
                        newMapScope.llstSelectedUsers.splice(index, 1);
                        newMapScope.lstUnMappedUsers.push(aobjUser);
                    }
                    dialogMapuser = $rootScope.showDialog(newMapScope, "Assign User", "StartUp/views/ClientUserAssociationDialog.html", { minHeight: 600, minWidth: 1300 });
                    newMapScope.OkClick = function () {
                        //Update user mapping for current client.
                        //Make ajax call to save mappings in db.
                        scope.isRefreshRequired = true;
                        var amonth = new Date().getUTCMonth() + 1;
                        scope.saveUserClientMapping(newMapScope.llstSelectedUsers.map(function (item) {
                            return {
                                User: item,
                                UserType: scope.isAdminMapping ? 'owner' : 'developer',
                                StartDate: amonth + "/" + new Date().getUTCDate() + "/" + new Date().getUTCFullYear(),
                                EndDate: null,
                                ClientID: scope.clientId,
                                Status: 'active'
                            };
                        }));
                        newMapScope.closeDialog();
                    };
                    newMapScope.closeDialog = function () {
                        dialogMapuser.close();
                    };
                }
            }
            scope.mapADUsers = function () {
                var newMapScope = scope.$new();
                newMapScope.IsShowRole = !scope.isAdminMapping;
                newMapScope.searchADUser = function () {
                    $rootScope.IsLoading = true;
                    $http({
                        method: 'POST',
                        data: { clientID: scope.clientId, samAccounName: newMapScope.samAccountName, domainName: newMapScope.domainName, name: newMapScope.name, email: newMapScope.email, adminMapping: scope.isAdminMapping },
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        async: false,
                        url: "api/Admin/GetADUsersForOwnerMapping"
                    }).then(function successCallback(response) {
                        $rootScope.IsLoading = false;
                        newMapScope.llstUnMappedADUsers = response.data;
                        scope.isRefreshRequired = true;
                    }, function errorCallback(exceptionData) {
                        $rootScope.IsLoading = false;
                        $rootScope.showExceptionDetails(exceptionData.data);
                    });
                }
                newMapScope.OK = function () {
                    newMapScope.beforeOK();
                    var amonth = new Date().getUTCMonth() + 1;

                    scope.saveUserClientMapping(newMapScope.selectedADUsers.map(function (item) {
                        return {
                            User: item,
                            UserType: scope.isAdminMapping ? 'owner' : 'developer',
                            StartDate: amonth + "/" + new Date().getUTCDate() + "/" + new Date().getUTCFullYear(),
                            EndDate: null,
                            ClientID: scope.clientId,
                            Status: 'active'
                        };
                    }));

                    //Make ajax call to save mappings in db.

                    newMapScope.closeDialog();
                }
                newMapScope.dialogMapuser = $rootScope.showDialog(newMapScope, "Assign Active Directory User", "StartUp/views/GetActiveDirectoryUsers.html", { width: 1400, height: 600 });

            }
            scope.saveUserClientMapping = function (alstUserClientMapping) {
                if (alstUserClientMapping) {
                    $.ajax({
                        url: "api/Admin/SaveUserClientMapping",
                        type: 'POST',
                        async: true,
                        data: JSON.stringify(alstUserClientMapping),
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            scope.$evalAsync(function () {
                                if (response && response.length > 0) {
                                    for (var i = 0, len = response.length; i < len; i++) {
                                        scope.userTypes.push(response[i]);
                                    }
                                }
                            });
                        },
                        error: function (exceptionData) {
                            $rootScope.showExceptionDetails(exceptionData);
                        }
                    });
                }
            }

            scope.createUser = function () {
                if (scope.callBackFunction) {
                    scope.callBackFunction();
                }
            }

            scope.onUserUpdateClick = function (objUserType, stId) {
                $http({
                    method: 'POST',
                    data: objUserType,
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/UpdateClientUserTypeDetails"
                }).then(function successCallback(response) {
                    if (response && response.data == "Cannot map user as owner for more than one client.") {
                        toastr.error(response.data);
                    } else {
                        toastr.success("User Updated sucessfully.");
                        scope.$broadcast('stUpdateRowBroadcast', { objUpdated: objUserType, stid: stId });
                    }
                }, function errorCallback(exceptionData) {
                    $rootScope.IsProjectLoaded = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            }

            scope.reSendActivationMailForUser = function (userType) {
                if (userType && userType.User && userType.User.UserID) {
                    $http({
                        method: 'POST',
                        data: { UserID: userType.User.UserID },
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
            scope.initializeComponentPicker = function () {
                ComponentsPickers.init();
            };

            $timeout(function () {
                if (scope.userTypes && scope.userTypes.length) {
                    scope.iobjSelectedUser = scope.userTypes[0]; // select first record
                }
            });
        },
        templateUrl: "User/views/userTypeList.html"
    };
}]);
