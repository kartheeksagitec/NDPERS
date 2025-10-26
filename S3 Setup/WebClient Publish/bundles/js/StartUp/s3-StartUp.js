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

app.controller('MainController', ["$scope", "$http", "$rootScope", "ngDialog", "$window", "$compile", "$interval", "hubcontext", "ConfigurationFactory", "cutFilter", "$DashboardFactory", "$FormatQueryFactory", "$resourceFactory", "$EntityIntellisenseFactory", "$cookies", "$filter", "$Errors", "$timeout", "$searchQuerybuilder", "$NavigateToFileService", "$searchFindReferences", "ParameterFactory", "$SgMessagesService", "$IentBaseMethodsFactory", function ($scope, $http, $rootScope, ngDialog, $window, $compile, $interval, hubcontext, ConfigurationFactory, cutFilter, $DashboardFactory, $FormatQueryFactory, $resourceFactory, $EntityIntellisenseFactory, $cookies, $filter, $Errors, $timeout, $searchQuerybuilder, $NavigateToFileService, $searchFindReferences, ParameterFactory, $SgMessagesService, $IentBaseMethodsFactory) {
    //#region Common Methods
    $rootScope.userIdleTimeout = 0;
    $rootScope.UndoRedoList = {};
    $rootScope.IsTFSConfigured = false;
    $rootScope.lstopenedfiles = [];
    $rootScope.lstPrevious = [];
    $rootScope.lstNext = [];
    $scope.SideBarSearch = {};
    $scope.SideBarSearch.listFile = [];
    $scope.SideBarSearch.totalRecords = 0;
    $scope.SideBarSearch.FilefilterText = "";
    $scope.iblnChatWindowVisible = false;
    $scope.iblnChatHelpVisible = true;
    var userCookie = $cookies.getObject("UserDetails");
    if (userCookie && JSON.parse(userCookie).UserName != null) {
        $rootScope.IsLogin = false;
    }
    $http({
        method: 'GET',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        url: "api/Login/GetEntityValidationFlag"
    }).then(function successCallback(response) {
        $rootScope.iblnShowEntityDuplicateIdAsWarning = response.data == "true" ? true : false;
    }, function errorCallback(exceptionData) {
        $rootScope.showExceptionDetails(exceptionData.data);
    });
    $scope.showError = function () {
        var newScope = $scope.$new();
        newScope.IsSignalRError = true;
        var dialog = $rootScope.showDialog(newScope, "Error Information", "Tools/views/LoginPageError.html");
        newScope.closeDialog = function () {
            dialog.close();
        };
    };


    $scope.GetAfterLoginData = function () {
        hubMain.server.getCommonData().done(function (objcommondata) {
            $scope.strFwkVersion = objcommondata.fwkVersion;
            $scope.strApplicationVersion = objcommondata.strApplicationVersion;
            $scope.IncorrectHubHandler();
            globalKeywords = objcommondata.lstKeyWords;
            $rootScope.LstButtonMethodLookup = objcommondata.LstButtonMethodLookup;
            $rootScope.LstButtonMethodMaintenance = objcommondata.LstButtonMethodMaintenance;
            $rootScope.LstButtonMethodWizard = objcommondata.LstButtonMethodWizard;
            //$rootScope.ChartTypes = objcommondata.lstChartTypes;
            $rootScope.lstWebControls = objcommondata.lstWebControl;
            $rootScope.lstActions = objcommondata.lstActions;
            $IentBaseMethodsFactory.setIentBaseMethods(objcommondata.LientBaseMethods);
            $rootScope.ChartTypes = ["Bar", "Column", "Donut", "Line", "Pie", "Radar", "StackedBar", "StackedColumn"]; // we are refering now this chart list insted of getting from FW
        });


        $scope.$evalAsync(function () {
            $rootScope.setCurrentUserName();
            $scope.CurrentProjectName = undefined;
        });
        $rootScope.updateProjectList();
    };

    $rootScope.updateProjectList = function () {
        $scope.$evalAsync(function () {
            $scope.lstprojects = ConfigurationFactory.getProjectList();
            var temObj = ConfigurationFactory.getLastProjectDetails();
            if (temObj && temObj != null) {
                $scope.selectedProject = ConfigurationFactory.getLastProjectDetails().ProjectName;
            }
        });
    };
    $rootScope.setCurrentUserName = function () {
        $rootScope.LoginName = "";
        var userCookie = $cookies.getObject("UserDetails");
        if (userCookie) {
            var lobjUser = JSON.parse(userCookie);
            if (lobjUser) {
                $rootScope.LoginName = lobjUser.FirstName;
            }
        }
    }
    $scope.IncorrectHubHandler = function () {
        if (s3Config && (s3Config.versionClient !== $scope.strApplicationVersion)) {
            toastr.warning("You are using a wrong version of S3 Hub, Please take the latest version (" + s3Config.versionClient + ") to have a smooth experience.", "Hub Version", { closeButton: true, timeOut: 0, extendedTimeOut: 0 });
        }
    }

    $scope.loadSelectedProject = function (currProject) {
        var scope = getScopeByFileName("SettingsPage");
        if (scope != undefined && scope.isDirty && $rootScope.isConfigureSettingsVisible) {
            scope.$evalAsync(function () {
                scope.closeFile(currProject.ProjectID);
            });
        }
        else {
            var newScope = $scope.$new();
            newScope.lstDirtyFiles = [];
            if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
                newScope.lstDirtyFiles = $scope.getDirtyFiles();
            }

            if (newScope.lstDirtyFiles.length > 0) {
                var dialog = $rootScope.showDialog(newScope, "UnSaved File List", "Common/views/UnsavedFileListDialog.html");
                newScope.closeSaveListDialog = function (buttontext) {
                    var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                    if (buttontext == "Yes") {
                        for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                            scope.onsavefileclick(newScope.lstDirtyFiles[i]);
                        }
                        $scope.ChangeProject(currProject);
                    }
                    else if (buttontext == "No") {
                        for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                            $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                        }
                        $scope.ChangeProject(currProject);
                    }
                    dialog.close();
                };
            }
            else {
                $scope.ChangeProject(currProject);
            }
        }
    };

    $scope.ChangeProject = function (currProject) {
        $rootScope.$evalAsync(function () {
            $rootScope.ClearUndoRedoList();
            $rootScope.IsProjectLoaded = true;
            $rootScope.projectLoadError = false;
            $rootScope.isConfigureSettingsVisible = false;
            $rootScope.isstartpagevisible = false;
            var lobjLastProjectDetail = ConfigurationFactory.getLastProjectDetails();
            if (!$scope.CurrentProjectName || (lobjLastProjectDetail && lobjLastProjectDetail != null && currProject.ProjectID != lobjLastProjectDetail.ProjectId)) {
                var userCookie = $cookies.getObject("UserDetails");
                var temUser = JSON.parse(userCookie);
                $http({
                    method: 'POST',
                    data: { UserID: temUser.UserID, ProjectId: currProject.ProjectID },
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/LoadSelectedProject"
                }).then(function successCallback(response) {

                    if (response.data == "ActiveUserLimit") {
                        $rootScope.IsProjectLoaded = false;
                        $rootScope.isConfigureSettingsVisible = true;
                        toastr.error("Number of users logged in reached to its maximum count.");
                    }
                    else if (response.data == "DateError") {
                        $rootScope.IsProjectLoaded = false;
                        $rootScope.isConfigureSettingsVisible = true;
                        toastr.error("Check status or end date is expired.");
                    }
                    else {
                        if (response.data[0].length > 0 && response.data[1].ProjectName != null) {
                            ConfigurationFactory.setConfigurationObject(response.data[0], response.data[1]);
                        }
                        else {
                            ConfigurationFactory.setConfigurationObject(response.data[0], null);
                        }
                        $rootScope.resetRootScopeVarible();
                        $scope.$evalAsync(function () {
                            $rootScope.IsProjectLoaded = true;
                        });
                        $rootScope.loadProjectData(response.data[1], true);
                    }

                }, function errorCallback(exceptionData) {
                    $rootScope.IsProjectLoaded = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });


            }
            else if (lobjLastProjectDetail) {
                $scope.$evalAsync(function () {
                    $rootScope.IsProjectLoaded = true;
                });
                $rootScope.resetRootScopeVarible();
                $rootScope.loadProjectData(lobjLastProjectDetail, true);
            }
        });

    };

    $scope.handleFileOperationsAdvanceSearch = function ($itemScope, objFile) {
        var filesToClose = $rootScope.lstopenedfiles.filter(function (item) { return item.file.FileName == objFile.FileName; });
        if (filesToClose && filesToClose.length > 0) {
            var lstAdvancesearch = [];
            // if open file is advance search - update the result list
            if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'SearchFiles') {
                lstAdvancesearch = $itemScope.$parent ? $itemScope.$parent.ResultantFiles : [];
            }
            // get search file scope and update the result list
            else {
                var searchfilescope = getScopeByFileName('SearchFiles');
                lstAdvancesearch = searchfilescope ? searchfilescope.ResultantFiles : [];
            }
            lstAdvancesearch.some(function (data, index, list) {
                if (data.FileName == objFile.FileName) {
                    list[index] = objFile;
                }
            });
        }
    };

    $rootScope.menuOptionForFiles = [
        ["Open Containing Folder", function ($itemScope) {
            hubMain.server.openContainingFolder($rootScope.currentopenfile.file.FilePath);
        }, function ($itemScope) {
            if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && !$rootScope.currentopenfile.file.FilePath) {
                return false;
            }
            else {
                return true;
            }
        }],
        ["Open in Notepad++", function ($itemScope) {
            hubMain.server.openInNotepadPlus($rootScope.currentopenfile.file.FilePath);
        }, function ($itemScope) {
            if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && !$rootScope.currentopenfile.file.FilePath) {
                return false;
            }
            else {
                return true;
            }
        }],

        ["Open HTML in Notepad++", function ($itemScope) {
            hubMain.server.openHTMLInNotepadPlus($rootScope.currentopenfile.file.FileName);
        }, function ($itemScope) {
            if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && !$rootScope.currentopenfile.file.FileName) {
                return false;
            }
            else if (["FormLinkLookup", "FormLinkMaintenance", "FormLinkWizard"].indexOf($rootScope.currentopenfile.file.FileType) < 0) {
                return false;
            }

            else {
                return true;
            }
        }],
        ["Pin File", function ($itemScope) {
            $scope.UserFileAction("pin", $rootScope.currentopenfile.file);
        }, function ($itemScope) {
            if ($rootScope.currentopenfile.isPinned || ($rootScope.currentopenfile.file.FileType && $rootScope.currentopenfile.file.FileType == "ERDiagram")) {
                return false;
            }
            else {
                return true;
            }
        }],
        ["UnPin File", function ($itemScope) {
            $scope.UserFileAction("pin", $rootScope.currentopenfile.file);
        }, function ($itemScope) {
            if ($rootScope.currentopenfile.isPinned) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Add File', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to Add ";
            newScope.FileName = $rootScope.currentopenfile.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", { width: 500, height: 170 });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.addFileInTFS($rootScope.currentopenfile.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveFileAddInTFSStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };

        }, function ($itemScope) {
            $scope.IsTFS = false;
            var lastProjectData = ConfigurationFactory.getLastProjectDetails();
            if (lastProjectData && lastProjectData.IsTFS) {
                $scope.IsTFS = true;
            }
            if ($rootScope.currentopenfile.file.TFSState == 'OnlyLocal' && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Check In', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to check in the pending changes in ";
            newScope.FileName = $rootScope.currentopenfile.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", { width: 500, height: 170 });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.checkInTFS($rootScope.currentopenfile.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveCheckInTfsStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };

        }, function ($itemScope) {
            if (($rootScope.currentopenfile.file.TFSState == 'InAdd' || $rootScope.currentopenfile.file.TFSState == 'InEdit') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Check Out', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to check out the ";
            newScope.FileName = $rootScope.currentopenfile.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", { width: 500, height: 170 });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.checkOut($rootScope.currentopenfile.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveCheckOutTfsStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if ($rootScope.currentopenfile.file.TFSState == 'InEdit' || $rootScope.currentopenfile.file.TFSState == 'InAdd' || $rootScope.currentopenfile.file.TFSState == 'OnlyLocal') {
                return false;
            }
            else if ($scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Undo', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to undo changes ";
            newScope.FileName = "";
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", { width: 500, height: 170 });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.undoCheckOut($rootScope.currentopenfile.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveUndoModel(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if (($rootScope.currentopenfile.file.TFSState == 'InAdd' || $rootScope.currentopenfile.file.TFSState == 'InEdit') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Get Latest Version', function ($itemScope) {
            $rootScope.IsLoading = true;
            hubMain.server.getLatest($rootScope.currentopenfile.file).done(function (objFile) {
                if (objFile) {
                    $scope.receiveGetLatest(objFile);
                    // handle advance search
                    $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                }
            });
        }, function ($itemScope) {
            if (($rootScope.currentopenfile.file.TFSState == 'InAdd' || $rootScope.currentopenfile.file.TFSState == 'InEdit' || $rootScope.currentopenfile.file.TFSState == 'LocalWithoutChange') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ["Close All But this", function ($itemScope) {
            $rootScope.checkAndCloseFiles();
        }, function ($itemScope) {
            return true;
        }]];

    $scope.receiveProjectafterLoad = function (dbConnection, ProjectName, errorList, IsOracleDB) {
        $scope.$evalAsync(function () {
            //console.log(performance.now() - $rootScope.timerbeforeloadProject);
            $scope.CurrentProjectName = ProjectName;
            $scope.selectedProject = ProjectName;
            $scope.strDBConnection = dbConnection;
            $scope.fileErrorList = errorList;
            $rootScope.IsProjectInitialLoad = true;
            $rootScope.IsOracleDB = IsOracleDB;
            $rootScope.isstartpagevisible = true;
            $rootScope.lstPrevious.push(undefined);
            $rootScope.IsProjectLoaded = false;
        });
    };
    hubcontext.hubMain.client.setToolsFileDetails = function (arrClsFileInfo) {
        if (arrClsFileInfo) {
            var data = JSON.parse(arrClsFileInfo);
            if (data && data.length > 0) {
                var files = $filter("filter")(data, { FileName: 'AuditLog' });
                if (files && files.length > 0) {
                    $scope.auditLogFileInfo = files[0];
                }

                var files = $filter("filter")(data, { FileName: 'CustomSettings' });
                if (files && files.length > 0) {
                    $scope.customSettingsFileInfo = files[0];
                }

                var files = $filter("filter")(data, { FileName: 'RuleConstants' });
                if (files && files.length > 0) {
                    $scope.ruleConstantsFileInfo = files[0];
                }

                var files = $filter("filter")(data, { FileName: 'entTableSchema' });
                if (files && files.length > 0) {
                    $scope.entTableSchemaInfo = files[0];
                }

                var files = $filter("filter")(data, { FileName: 'ProjectConfiguration' });
                if (files && files.length > 0) {
                    $scope.projectConfigurationFileInfo = files[0];
                }
            }
        }

    };

    $scope.redirectToProjectSettings = function () {
        $scope.$evalAsync(function () {
            $rootScope.isstartpagevisible = false;
            $rootScope.IsProjectLoaded = false;
            $rootScope.isConfigureSettingsVisible = true;
            $rootScope.currentopenfile = undefined;
        });
    };

    $scope.openConfigureSettings = function (flag) {

        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
            newScope.lstDirtyFiles = $scope.getDirtyFiles();
        }
        else {
            $rootScope.isstartpagevisible = false;
            $scope.closeLeftBar();
            $rootScope.isConfigureSettingsVisible = true;
        }
        if (newScope.lstDirtyFiles.length > 0) {
            var dialog = $rootScope.showDialog(newScope, "UnSaved File List", "Common/views/UnsavedFileListDialog.html");
            newScope.closeSaveListDialog = function (buttontext) {
                var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                if (buttontext == "Yes") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        scope.onsavefileclick(newScope.lstDirtyFiles[i]);
                        //$rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    $rootScope.$evalAsync(function () {
                        $rootScope.isstartpagevisible = false;
                        $rootScope.isConfigureSettingsVisible = true;
                        $scope.closeLeftBar();
                    });
                }
                else if (buttontext == "No") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    $rootScope.$evalAsync(function () {
                        $rootScope.isstartpagevisible = false;
                        $rootScope.isConfigureSettingsVisible = true;
                        $scope.closeLeftBar();
                    });
                }
                dialog.close();
            };
        }
        else {
            $rootScope.$evalAsync(function () {
                $rootScope.isstartpagevisible = false;
                $rootScope.isConfigureSettingsVisible = true;
                $rootScope.currentopenfile = undefined;
                $scope.closeLeftBar();
            });
        }
    };

    $scope.getDirtyFiles = function () {
        var lstDirtyFiles = [];
        for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
            var scope = getScopeByFileName($rootScope.lstopenedfiles[i].file.FileName);
            if (scope) {
                if (scope.isDirty) {
                    lstDirtyFiles.push($rootScope.lstopenedfiles[i]);
                }
            }
        }
        return lstDirtyFiles;
    };
    //=========================================== Left Slide Bar implementation starts
    function setFileTypeWise(data) {
        $scope.$evalAsync(function () {
            if (data && data.searchFiles) {
                $scope.SideBarSearch.listFile = data.searchFiles.length > 0 ? $filter('orderBy')(data.searchFiles, 'FileName') : [];
                $scope.SideBarSearch.totalRecords = data.totalRecords;
            }
            $scope.SideBarSearch.IsProcessing = false;
        });
    }
    $scope.slideOutLeftBar = function (category, $event) {
        var activeli = $event.currentTarget;
        if ($scope.SideBarSearch.category != category || $rootScope.IsTfsUpdated || $rootScope.IsFilesAddedDelete || $rootScope.isFilesListUpdated) {
            $rootScope.IsTfsUpdated = false;
            $scope.SideBarSearch.FilefilterText = "";
            $scope.SideBarSearch.category = category;
            $scope.SideBarSearch.PageIndex = 1;
            var sideBarSearchPromise = $scope.GetSideBarSearchList();
            sideBarSearchPromise.done(setFileTypeWise);
            var sliderleft = activeli.getBoundingClientRect();
            $(".left-slider").css({ left: (sliderleft.left - 115) + 'px', });
            $(".left-slider").show(500);
        }
        else {
            $(activeli).toggleClass("borderBWhite");
            $(".left-slider").toggle();
        }
        // tweak for IE
        window.setTimeout(function () {
            $("[ui-search-wrapper] input[id='searchfiletype']").focus();
        }, 50);
    };
    $scope.closeLeftBar = function () {
        $('.global-menu li a').removeClass("borderBWhite");
        $(".left-slider").slideUp("fast");
    };

    $scope.GetSideBarSearchList = function () {
        return hubcontext.hubSearch.server.getFileListTypewise($scope.SideBarSearch.PageIndex, $scope.SideBarSearch.category, $scope.SideBarSearch.FilefilterText ? $scope.SideBarSearch.FilefilterText.trim() : "");
    };
    $scope.SidebarScrollDown = function () {
        $scope.SideBarSearch.IsProcessing = true;
        $scope.SideBarSearch.PageIndex = $scope.SideBarSearch.PageIndex + 1;
        $scope.SideBarSearch.SelectedFile = $("[ui-search-wrapper]").find("li.selected").length > 0 ? $("[ui-search-wrapper]").find("li.selected")[0] : null;
        if ($scope.SideBarSearch.listFile && $scope.SideBarSearch.listFile.length < $scope.SideBarSearch.totalRecords) {
            var scrollPromise = $scope.GetSideBarSearchList();
            scrollPromise.done(function (data) {
                setFileTypeWise(data);
                if ($scope.SideBarSearch.SelectedFile) {
                    setTimeout(function () {
                        var idSelectedFile = $($scope.SideBarSearch.SelectedFile).attr("id");
                        var elem = $("[ui-search-wrapper] #" + idSelectedFile);
                        if (elem.length > 0) {
                            elem.addClass('selected');
                            $("[ui-search-wrapper] .list-group").scrollTo(elem, null, null);
                        }
                    }, 500);
                }
            });
        }
    };
    $scope.SideBarFileSelect = function (file) {
        $(".left-slider").hide();
        $rootScope.openFile(file);
    };
    $scope.homepageclick = function () {
        $scope.$evalAsync(function () {
            if (!$rootScope.isConfigureSettingsVisible && !$rootScope.isstartpagevisible) {
                $rootScope.isstartpagevisible = true;
                $scope.closeLeftBar();
                $rootScope.isConfigureSettingsVisible = false;
                $rootScope.lstPrevious.push(undefined);
                // flush the current file variable
                $rootScope.currentopenfile = undefined;
                $scope.ClassForHeaderPanel = "";
            }
        });
    };
    $scope.SearchFilterFileTypeWise = function () {
        $scope.SideBarSearch.PageIndex = 1;
        var sideBarSearchPromise = $scope.GetSideBarSearchList();
        sideBarSearchPromise.done(setFileTypeWise);
    };
    $scope.SideBarSearchKeyDown = function (e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        var objParent = $(e.currentTarget).parents("[ui-search-wrapper]");
        switch (charCode) {
            // up handlers 
            case 38:
                if ($(objParent).find("li.selected").length > 0) {
                    $(objParent).find('li:not(:first-child).selected').removeClass('selected').prev().addClass('selected');
                }
                if ($(objParent).find("li.selected").length > 0) {
                    $("[ui-search-wrapper] .list-group").scrollTo($(objParent).find("li.selected"), null, null);
                }
                break;
            // down handlers
            case 40:
                if ($(objParent).find("li.selected").length > 0) {
                    $(objParent).find('li:not(:last-child).selected').removeClass('selected').next().addClass('selected');
                }
                else {
                    $(objParent).find('li:first-child').addClass('selected');
                }
                if ($(objParent).find("li.selected").length > 0) {
                    $("[ui-search-wrapper] .list-group").scrollTo($(objParent).find("li.selected"), null, null);
                }
                break;
            // for enter search
            case 13:
                var listItems = $(objParent).find("li");
                var numIndexSelected = $(listItems).index($(listItems).filter(".selected"));
                if (numIndexSelected > -1 && $scope.SideBarSearch.listFile.length > 0 && $scope.SideBarSearch.listFile[numIndexSelected]) {
                    $scope.SideBarFileSelect($scope.SideBarSearch.listFile[numIndexSelected]);
                }
                event.preventDefault();
                break;
            default:
        }
    };
    //=========================================== Left Slide Bar implementation ends

    //=========================================== global search implementation starts
    $scope.getGlobalSearch = function () {
        $('#globalSearch').autocomplete({
            minLength: 0,
            appendTo: "#dvIntellisense",
            open: function (event, ui) {
                $("#dvIntellisense > ul").css({
                    width: 'auto',
                    height: 'auto',
                    overflow: 'auto',
                    maxWidth: '300px',
                    maxHeight: "700px"
                });
                var $input = $(event.target),
                    $results = $input.autocomplete("widget");
                //top = $results.position().top,
                left = $results.position().left + 10,
                    windowWidth = $(window).width(),
                    height = $results.height(),
                    inputHeight = $input.height(),
                    windowsHeight = $(window).height();
                if (windowsHeight < $results.position().top + height + inputHeight) {
                    newTop = $results.position().top - height - inputHeight - 10;
                    $results.css("top", newTop + "px");
                }
                if (left + $results.width() > windowWidth) {
                    newleft = left - $results.width() + 110;
                    $results.css("left", newleft + "px");
                }
            },
            source: function (request, response) {
                if (request.term && request.term.trim() != "") {
                    hubcontext.hubSearch.server.getsearchGlobal(request.term.trim()).done(function (data) {
                        if (data) {
                            response(parseGlobalSearchResult(data));
                        }
                    });
                }
            },
            search: function (event, ui) {
                if (event.target.value == "") {
                    $(this).autocomplete("destroy");
                }
            },
            focus: function (event, ui) {
                $(".ui-autocomplete > li").attr("title", ui.item.value);
            },
            select: function (event, ui) {
                onGlobalSearchSelect(ui);
                this.value = "";
                return false;
            }
        });
    };
    $scope.getGlobalSearchPaste = function (e) {
        if ($(e.currentTarget).data("ui-autocomplete")) {
            setTimeout(function () {
                $(e.currentTarget).autocomplete("search", $(e.currentTarget).val());
            }, 0);
        }
    };
    function parseGlobalSearchResult(data) {
        return $.map(data, function (value, key) {
            return {
                label: value.FileName,
                value: value.FileName,
                FileName: value.FileName,
                FilePath: value.FilePath,
                FileType: value.FileType,
                TFSState: value.TFSState
            };
        });
    }
    function onGlobalSearchSelect(ui) {
        $rootScope.isstartpagevisible = false;
        $rootScope.openFile(ui.item);
    }
    //=========================================== global search implementation starts

    document.body.onkeydown = function (event) {

        if (event.ctrlKey && event.shiftKey && event.keyCode == 70) {
            // advance search can be triggered by this
            $scope.openSearchFile();
            //var dialogScope = $scope.$new(true);
            //dialogScope.FindDialog = $rootScope.showDialog(dialogScope, "Find in Files", "Common/views/FindAdvanceFiles.html", { width: 1100, height: 500, showclose: true });
            //dialogScope.close = function () {
            //    $scope.$evalAsync(function () {
            //        dialogScope.FindDialog.close();
            //    });
            //}
        }

        if (event.keyCode == 27) {
            $(".page-header-fixed").css("pointer-events", "auto");
        }
        if ($('.sgt-wrapper').length > 0) {
            $("#temprow").remove();
            $('.sgt-wrapper.row-wrap').parent("tr").remove();
            $('.sgt-wrapper').remove();
        }
        if ($rootScope.userIdleTimeout) clearTimeout($rootScope.userIdleTimeout);
    };
    document.onmousemove = function () {
        $scope.IsUserWorking = true;
        if ($('.sgt-wrapper').length > 0 && (!$(event.target).hasClass('sgt-wrapper') && !$(event.target).hasClass('row-col'))) {
            $('.sgt-wrapper.row-wrap').parent("tr").remove();
            $('.sgt-wrapper.col-wrap').remove();
        }
        if ($("#temprow").length && (!$(event.target).hasClass('sgt-wrapper') && !$(event.target).hasClass('row-col'))) $("#temprow").remove();
    };
    document.onmousedown = function (event) {
        $scope.IsUserWorking = true;
        if ($(event.target).parents('.left-slider').length <= 0) {
            // if top file bar is visible then slide it up
            if ($(".left-slider").is(":visible")) {
                $(".left-slider").slideUp("fast");
            }
        }
        var errPopup = $('#validation-error-list');
        if (!errPopup.is(event.target) && errPopup.has(event.target).length == 0) {
            errPopup.fadeOut();
        }
        var treePopup = $('.xml-control-tree');
        if (!treePopup.is(event.target) && treePopup.has(event.target).length == 0 && $(event.target).prop("id") != "quick-navigator") {
            treePopup.slideUp();
            $scope.showFormTreeMap("close");
        }

        var warningPopup = $('#validation-warning-list');
        if (!warningPopup.is(event.target) && warningPopup.has(event.target).length == 0) {
            warningPopup.fadeOut();
        }
        var dupIdPopup = $('.duplicate-id-tooltip');
        if (!dupIdPopup.is(event.target) && dupIdPopup.has(event.target).length == 0) {
            dupIdPopup.fadeOut();
        }
        if ($('.sgt-wrapper').length > 0 && !$(event.target).hasClass('sgt-wrapper')) {
            $('#temprow').remove();
            $('.sgt-wrapper.row-wrap').parent("tr").remove();
            $('.sgt-wrapper.col-wrap').remove();
        }

        clearTimeout($rootScope.userIdleTimeout);
    };
    document.ontouchstart = function () {
        $scope.IsUserWorking = true;
    };
    document.onclick = function () {
        $scope.IsUserWorking = true;
    };
    //This works well for F5, Ctr+F5 and Ctrl+R
    document.onkeydown = function (e) {
        if (e.keyCode == 116 || (e.ctrlKey && e.keyCode == 82)) {
            $rootScope.IsLogOut = true;
        }
        $scope.IsUserWorking = true;
    };


    //setInterval(checkUserIdle, 1000);
    //function checkUserIdle() {
    //    if ($scope.IsUserWorking && $rootScope.IsLogin) {
    //        SessionEvents.ResetTimer();
    //        $scope.IsUserWorking = false;
    //    }
    //}

    //#region Open File
    $rootScope.openFile = function (fileDetails, isNewFile) {
        function iterator(value, key) {
            value.isvisible = false;
            if (value.file.FileName == fileDetails.FileName && fileDetails.FileType && value.file.FileType == fileDetails.FileType) {
                blnisadded = true;
                value.isvisible = true;
                $rootScope.currentopenfile = value;

            }
        }
        if (fileDetails != undefined) {
            // if you type something in global search and open the file from somewhere else then search text will be cleared
            var dateTime = new Date();
            fileDetails.TimeModified = dateTime.toISOString(); // using for active file list(Shift + Tab) sorting 
            $('#globalSearch').val("");
            $('.d3-tip').hide();
            $scope.closeLeftBar();
            if (isNewFile) {
                $rootScope.IsFilesAddedDelete = true;
                $rootScope.IsTfsUpdated = true;
            }
            //if (fileDetails.FilePath || (fileDetails.FileName && fileDetails.FileName == "SearchFiles")) {
            if (fileDetails.FileName) {
                // for handling previous and next
                var index = $rootScope.lstPrevious.length - 1;
                if (index > -1 && ($rootScope.lstPrevious[index] == undefined || $rootScope.lstPrevious[index].FileName != fileDetails.FileName)) {
                    if (!$rootScope.currentopenfile || ($rootScope.currentopenfile.file.FileName != fileDetails.FileName && (isNewFile || isNewFile == undefined)))
                        $rootScope.lstPrevious.push(fileDetails);
                }
            }
            // actual push in the open file list - will do everytime irrespective of filepath exist 
            var blnisadded = false;

            angular.forEach($rootScope.lstopenedfiles, iterator);
            if (blnisadded == false) {
                var objopenfile = {
                    file: fileDetails, isvisible: true, IsSavePending: false
                };
                $rootScope.currentopenfile = objopenfile;
                $rootScope.lstopenedfiles.push(objopenfile);
            }
            $scope.SetClassForHeaderPanel();
            if (fileDetails.FilePath) {
                // for checking if the opened file is already pinned - to show the icon
                var pinnedfiles = $DashboardFactory.getPinnedFilesData();
                var pinfile = pinnedfiles.filter(function (item) {
                    return item.FileName == $rootScope.currentopenfile.file.FileName;
                });
                $rootScope.currentopenfile.isPinned = pinfile.length > 0 ? true : false;
                if ($rootScope.currentopenfile) {
                    $scope.UserFileAction("Recent", $rootScope.currentopenfile.file);
                }
            }
            $rootScope.$evalAsync(function () {
                $rootScope.isConfigureSettingsVisible = false;
                $rootScope.isstartpagevisible = false;
            });
        }
    };


    //#endregion

    //#region Previous/Next file
    $scope.PreviousClick = function () {
        var tempFile = "";
        if ($rootScope.lstPrevious.length > 1) {
            tempFile = $rootScope.lstPrevious[$rootScope.lstPrevious.length - 2];

            if ($rootScope.lstNext.length > 0) {
                if ($rootScope.lstNext[$rootScope.lstNext.length - 1] != $rootScope.lstPrevious[$rootScope.lstPrevious.length - 1]) {
                    $rootScope.lstNext.push($rootScope.lstPrevious[$rootScope.lstPrevious.length - 1]);
                }
            }
            else {
                $rootScope.lstNext.push($rootScope.lstPrevious[$rootScope.lstPrevious.length - 1]);
            }

            $rootScope.lstPrevious.splice($rootScope.lstPrevious.length - 1, 1);
            if (tempFile == undefined) {
                $scope.$evalAsync(function () {
                    $rootScope.isstartpagevisible = true;
                    $rootScope.isConfigureSettingsVisible = false;
                    $scope.closeLeftBar();
                    $rootScope.currentopenfile = undefined;
                });
            }
            else {
                $rootScope.openFile(tempFile, false);
            }
        }



    };
    $scope.NextClick = function () {
        if ($rootScope.lstNext.length > 0) {
            var tempFile = $rootScope.lstNext[$rootScope.lstNext.length - 1];
            if (tempFile == undefined) {
                $scope.$evalAsync(function () {
                    $rootScope.currentopenfile = undefined;  //Bug 9979:On File Menues-Selection is Displayed -If File is not Opened.
                    $scope.closeLeftBar();
                    $rootScope.isstartpagevisible = true;
                    $rootScope.isConfigureSettingsVisible = false;
                });
            }
            else {
                $rootScope.openFile(tempFile, false);
            }
            $rootScope.lstNext.splice($rootScope.lstNext.length - 1, 1);
            if ($rootScope.lstPrevious.length > 0) {
                if ($rootScope.lstPrevious[$rootScope.lstPrevious.length - 1] != tempFile) {
                    $rootScope.lstPrevious.push(tempFile);
                }
            }
            else {
                $rootScope.lstPrevious.push(tempFile);
            }

        }
    };
    //#endregion

    //#region Close File
    $rootScope.checkAndCloseFiles = function () {
        //Need to write logic for checking dirty files and show a confirmation dialog
        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        newScope.lstNotDirtyFiles = [];
        var temp = JSON.stringify($rootScope.lstopenedfiles);
        var tempOpenFile = JSON.parse(temp);
        if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
            for (var i = 0; i < tempOpenFile.length; i++) {
                if (tempOpenFile[i].file.FileName != $rootScope.currentopenfile.file.FileName) {
                    var scope = getScopeByFileName(tempOpenFile[i].file.FileName);
                    if (scope) {
                        if (scope.isDirty) {
                            newScope.lstDirtyFiles.push(tempOpenFile[i]);
                        }
                        else {
                            newScope.lstNotDirtyFiles.push(tempOpenFile[i]);
                            //$rootScope.closeFile(tempOpenFile[i].file.FileName);
                        }
                    }
                }
            }
        }
        if (newScope.lstDirtyFiles.length > 0) {
            var dialog = $rootScope.showDialog(newScope, "UnSaved File List", "Common/views/UnsavedFileListDialog.html");
            newScope.closeSaveListDialog = function (buttontext) {
                var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                if (buttontext == "Yes") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        scope.onsavefileclick(newScope.lstDirtyFiles[i]);
                        $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                }
                else if (buttontext == "No") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                }
                dialog.close();
            };
        }
        else {
            for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
            }
        }
    };
    $rootScope.CloseCurrentFiles = function (files) {
        var file = { file: files };
        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        var scope = getScopeByFileName(files.FileName);
        if (scope) {
            if (scope.isDirty) {
                newScope.lstDirtyFiles.push(file);
            }
            else {
                $rootScope.closeFile(files.FileName);
            }
        }
        if (newScope.lstDirtyFiles.length > 0) {
            var dialog = $rootScope.showDialog(newScope, "Save Dialog", "Common/views/UnsavedFileListDialog.html");
            newScope.closeSaveListDialog = function (buttontext) {
                var pageLayoutScope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                if (buttontext == "Yes") {
                    if (!scope.canSaveFile || (scope.canSaveFile && scope.canSaveFile())) {
                        pageLayoutScope.onsavefileclick(newScope.lstDirtyFiles[0]);
                        $rootScope.closeFile(newScope.lstDirtyFiles[0].file.FileName);
                    }
                }
                else if (buttontext == "No") {
                    $rootScope.closeFile(newScope.lstDirtyFiles[0].file.FileName);
                }
                dialog.close();
            };
        }
    };
    $rootScope.closeFile = function (fileName) {
        var filesToClose = $rootScope.lstopenedfiles.filter(function (item) { return item.file.FileName == fileName; });
        if (filesToClose && filesToClose.length > 0) {
            hubcontext.hubMain.server.clearSelectedPath(fileName);
            var index = $rootScope.lstopenedfiles.indexOf(filesToClose[0]);
            $rootScope.lstopenedfiles.splice(index, 1);
            filesToClose[0].isvisible = false;
            // fix for htx iframe focus issue
            if (filesToClose[0].file.FileType == "FormLinkLookup" || filesToClose[0].file.FileType == "FormLinkMaintenance" || filesToClose[0].file.FileType == "FormLinkWizard") {
                $('#globalSearch').focus();
            }
            for (var i = $rootScope.lstPrevious.length; i--;) {
                if ($rootScope.lstPrevious[i] != undefined && $rootScope.lstPrevious[i].FileName == fileName) $rootScope.lstPrevious.splice(i, 1);
            }
            for (var i = $rootScope.lstNext.length; i--;) {
                if ($rootScope.lstNext[i] != undefined && $rootScope.lstNext[i].FileName == fileName) $rootScope.lstNext.splice(i, 1);
            }
            $rootScope.ClearUndoRedoListByFileName(filesToClose[0].file.FileName);

            var deletePosition = -1;
            for (var i = 0; i < $Errors.validationListObj.length; i++) {
                if ($Errors.validationListObj[i].FileName == fileName) {
                    deletePosition = i;
                }
            }
            if (deletePosition != -1) { // delete error list of current file
                $Errors.validationListObj.splice(deletePosition, 1);
            }

            if ($rootScope.lstopenedfiles.length > 0) {
                //$rootScope.currentopenfile = $rootScope.lstopenedfiles[0];
                //$rootScope.currentopenfile.isvisible = true;
                if ($rootScope.lstPrevious.length > 1) {
                    if ($rootScope.lstNext.length > 0 && $rootScope.lstNext[$rootScope.lstNext.length - 1] == $rootScope.lstopenedfiles[0].file) {
                        $rootScope.lstNext.splice($rootScope.lstNext.length - 1, 1);
                    }
                    if ($rootScope.lstPrevious[$rootScope.lstPrevious.length - 1] == undefined) {
                        $rootScope.isstartpagevisible = true;
                        $rootScope.currentopenfile = undefined;
                    }
                    else {
                        $rootScope.openFile($rootScope.lstPrevious[$rootScope.lstPrevious.length - 1]);
                    }

                }
                else {
                    if ($rootScope.lstNext.length > 0 && $rootScope.lstNext[$rootScope.lstNext.length - 1] == $rootScope.lstopenedfiles[0].file) {
                        $rootScope.lstNext.splice($rootScope.lstNext.length - 1, 1);
                    }
                    $rootScope.openFile($rootScope.lstopenedfiles[0].file);
                }

            }
            else {
                $rootScope.lstNext = [];
                $rootScope.lstPrevious = [];
                $rootScope.lstPrevious.push(undefined);
                $scope.ClassForHeaderPanel = "";
                $rootScope.currentopenfile = undefined;
                $rootScope.$evalAsync(function () {
                    $rootScope.isstartpagevisible = true;
                });
            }
            $scope.SetClassForHeaderPanel();
            // check for floating button - find in reference 
            if (filesToClose[0].file.FileType && filesToClose[0].file.FileType == 'Entity') {
                var eleFloatingReferences = $('body').find(".findReferences-floating-btn[filename='" + filesToClose[0].file.FileName + "']");
                if (eleFloatingReferences && eleFloatingReferences.length > 0) {
                    $searchFindReferences.resetData();
                    eleFloatingReferences.remove();
                }
            }
        }
    };
    //#endregion
    $scope.addanddeleteuserfile = function (action, objfile) {
        if (action.toLowerCase() == "pin" || action.toLowerCase() == "unpin") {
            var pinnedfiles = $DashboardFactory.getPinnedFilesData();
            var pinfile = pinnedfiles.filter(function (item) {
                return item.FileName == objfile.FileName;
            });
            // if file is already there
            if (pinfile.length > 0) {
                var pinindex = pinnedfiles.indexOf(pinfile[0]);
                pinnedfiles.splice(pinindex, 1);
            }
            else {
                if (pinnedfiles.length < 20 && action.toLowerCase() == "pin") {
                    pinnedfiles.unshift(objfile);
                }
                else if (pinnedfiles.length >= 20 && action.toLowerCase() == "pin") {
                    pinnedfiles.splice(-1, 1);
                    pinnedfiles.unshift(objfile);
                }
            }
            $DashboardFactory.setPinnedFilesData(pinnedfiles);
        }
        else if (action.toLowerCase() == "recent" || action.toLowerCase() == "unrecent") {
            var recentfiles = $DashboardFactory.getRecentFilesData();
            var recentfile = recentfiles.filter(function (item) {
                return item.FileName == objfile.FileName;
            });
            // if file is already there
            if (recentfile.length > 0) {
                var recentindex = recentfiles.indexOf(recentfile[0]);
                if (action.toLowerCase() == "unrecent") {
                    if (recentindex > -1) recentfiles.splice(recentindex, 1);
                }
                else if (action.toLowerCase() == "recent") {
                    recentfiles.splice(recentindex, 1);
                    recentfiles.unshift(objfile);
                }
            }
            else {
                if (action.toLowerCase() == "recent" && recentfiles.length < 20) {
                    recentfiles.unshift(objfile);
                }
                else if (recentfiles.length >= 20 && action.toLowerCase() == "recent") {
                    recentfiles.splice(-1, 1);
                    recentfiles.unshift(objfile);
                }
            }
            $DashboardFactory.setRecentFilesData(recentfiles);
        }
    };

    $scope.UserFileAction = function (action, objfile) {
        // every user action in the factory
        // do validation for file
        if (action != "") {
            var userCookie = $cookies.getObject("UserDetails");
            if (userCookie && JSON.parse(userCookie).UserID != null) {
                var filesdataPromise = $DashboardFactory.postUserFiles(objfile, JSON.parse(userCookie).UserID, action);
                filesdataPromise.then(function (result) {
                    if (result) {
                        // if dashboard is visible update user files
                        $scope.$broadcast('updateUserFiles');
                        $scope.addanddeleteuserfile(action, objfile);
                        if (action.toLowerCase() == "pin" && $rootScope.currentopenfile && objfile.FileName == $rootScope.currentopenfile.file.FileName) {
                            $rootScope.currentopenfile.isPinned = !$rootScope.currentopenfile.isPinned;
                            if ($rootScope.currentopenfile.isPinned) toastr.success("File pinned successfully");
                            else toastr.success("File unpinned successfully");
                        }
                    }
                });
            }
        }
    };
    //#region Undo Redo functionality

    $rootScope.ClearUndoRedoList = function () {
        $rootScope.UndoRedoList = {};
    };

    $rootScope.ClearUndoRedoListByFileName = function (fileName) {
        if ($rootScope.UndoRedoList.hasOwnProperty(fileName)) {
            var files = $rootScope.lstopenedfiles.filter(function (item) { return item.file.FileName == fileName; });
            if (files && files.length > 0) {

                $rootScope.UndoRedoList[fileName].UndoList = [];
                $rootScope.UndoRedoList[fileName].RedoList = [];
            }
            else {
                delete $rootScope.UndoRedoList[fileName];
            }
        }
    };

    $scope.UndoClick = function () {
        if ($scope.canUndo()) {
            var curscopeDivId;
            if ($rootScope.isConfigureSettingsVisible) {
                curscopeDivId = "SettingsPage";
            }
            else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
                curscopeDivId = $rootScope.currentopenfile.file.FileName;
            }

            var scope = getScopeByFileName(curscopeDivId);
            if (curscopeDivId != undefined) {
                var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                if (sessionstorageitem) {
                    if (sessionstorageitem.UndoList.length > 0) {
                        var undoitem = sessionstorageitem.UndoList.pop();
                        $scope.AddToRedoList(sessionstorageitem, undoitem);

                        if (undoitem.position == "End") {
                            while (undoitem.position != "Start") {
                                undoitem = sessionstorageitem.UndoList.pop();
                                $scope.AddToRedoList(sessionstorageitem, undoitem);
                            }
                        }

                    }
                }
                if (scope && scope.validateFileData && scope.currentfile && scope.currentfile.FileType == "Entity") {
                    scope.validateFileData();
                }
                if (scope) {
                    if (!scope.isDirty) {
                        scope.isDirty = true;
                    }
                }
            }
        }
    };

    $scope.AddToRedoList = function (curscope, undoitem) {
        var redoitem = {
        };

        if (undoitem.operation == "Edit") {
            if (undoitem.model) {
                redoitem.value = getPropertyValue(undoitem.model, undoitem.propname);
                redoitem.operation = undoitem.operation;
                redoitem.model = undoitem.model;
                redoitem.propname = undoitem.propname;
                redoitem.undorelatedfunction = undoitem.undorelatedfunction;

                if (undoitem.model) {
                    setPropertyValue(undoitem.model, undoitem.propname, undoitem.value);
                }
                if (undoitem.undorelatedfunction) {
                    if (typeof undoitem.undorelatedfunction === "function") {
                        var model = undoitem.model;
                        $timeout(function () {
                            if (undoitem.undorelatedfunction.length > 0) {
                                undoitem.undorelatedfunction(model);
                            } else {
                                undoitem.undorelatedfunction();
                            }
                        });
                    } else {
                        var curelescope = getCurrentFileScope();
                        if (curelescope != undefined) {
                            curelescope[undoitem.undorelatedfunction](undoitem.value);
                        }
                    }
                }
                curscope.RedoList.push(redoitem);
            }
        }
        else if (undoitem.operation == "Add") {
            if (undoitem.collection) {

                redoitem.operation = undoitem.operation;
                redoitem.collection = undoitem.collection;
                redoitem.item = undoitem.item;
                redoitem.undorelatedfunction = undoitem.undorelatedfunction;
                var curindex = undoitem.collection.indexOf(undoitem.item);

                if (undoitem.index != undefined && undoitem.index > -1) {
                    redoitem.index = curindex;
                }

                curscope.RedoList.push(redoitem);
                if (undoitem.collection) {
                    undoitem.collection.splice(undoitem.index, 1);
                }

                if (undoitem.undorelatedfunction) {
                    var curelescope = getCurrentFileScope();
                    if (curelescope != undefined) {
                        // if collection length is zero - selection should be removed i.e selected variable should be set to undefined (reviewed) - shashank was here
                        if (undoitem.collection.length > 0) {
                            if (curindex > 0) {
                                var indexSelectItem = null;
                                for (var i = curindex - 1; i >= 0; i--) {
                                    if (undoitem.collection[i].Name == undoitem.item.Name) {
                                        indexSelectItem = i;
                                        break;
                                    }
                                }
                                curelescope[undoitem.undorelatedfunction](indexSelectItem != null ? undoitem.collection[indexSelectItem] : undefined);
                            }
                            else {
                                curelescope[undoitem.undorelatedfunction](undoitem.collection[0].Name == undoitem.item.Name ? undoitem.collection[0] : undefined);
                            }
                        }
                        else {
                            curelescope[undoitem.undorelatedfunction]();
                        }
                    }
                }
            }
        }
        else if (undoitem.operation == "Insert") {
            if (undoitem.collection) {

                redoitem.operation = undoitem.operation;
                redoitem.collection = undoitem.collection;
                redoitem.item = undoitem.item;
                redoitem.index = undoitem.index;

                curscope.RedoList.push(redoitem);
                if (undoitem.collection) {
                    undoitem.collection.splice(undoitem.index, 1);
                }
            }
        }
        else if (undoitem.operation == "Delete") {
            if (undoitem.collection) {

                redoitem.operation = undoitem.operation;
                redoitem.collection = undoitem.collection;
                redoitem.item = undoitem.item;
                redoitem.index = undoitem.index;
                redoitem.undorelatedfunction = undoitem.undorelatedfunction;

                curscope.RedoList.push(redoitem);

                if (undoitem.collection) {
                    undoitem.collection.splice(undoitem.index, 0, undoitem.item);
                }

                if (undoitem.undorelatedfunction) {
                    var curelescope = getCurrentFileScope();
                    if (curelescope != undefined) {
                        if (undoitem.collection.length > 0) {
                            curelescope[undoitem.undorelatedfunction](undoitem.collection[undoitem.index]);
                        }
                    }
                }
            }
        }
        else {
            redoitem.position = undoitem.position;
            curscope.RedoList.push(redoitem);
        }
    };

    $scope.RedoClick = function () {
        if ($scope.canRedo()) {
            var curscopeDivId;
            if ($rootScope.isConfigureSettingsVisible) {
                curscopeDivId = "SettingsPage";
            }
            else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
                curscopeDivId = $rootScope.currentopenfile.file.FileName;
            }

            //if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            //    curscopeDivId = $rootScope.currentopenfile.file.FileName;
            //}
            //else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            //    curscopeDivId = $rootScope.currentopenfile.file.FileName;
            //}
            var scope = getScopeByFileName(curscopeDivId);

            if (curscopeDivId != undefined) {
                var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                if (sessionstorageitem) {
                    if (sessionstorageitem.RedoList.length > 0) {
                        var redoitem = sessionstorageitem.RedoList.pop();
                        $scope.AddToUndoList(redoitem, sessionstorageitem);

                        if (redoitem.position == "Start") {
                            while (redoitem.position != "End") {
                                redoitem = sessionstorageitem.RedoList.pop();
                                $scope.AddToUndoList(redoitem, sessionstorageitem);
                            }
                        }

                    }
                }

                if (scope && scope.validateFileData && scope.currentfile && scope.currentfile.FileType == "Entity") {
                    scope.validateFileData();
                }
                if (scope) {
                    if (!scope.isDirty) {
                        scope.isDirty = true;
                    }
                }
            }
        }
    };

    $scope.AddToUndoList = function (redoitem, curscope) {
        var undoitem = {
        };


        if (redoitem.operation == "Edit") {
            if (redoitem.model) {
                undoitem.value = getPropertyValue(redoitem.model, redoitem.propname);
                undoitem.operation = redoitem.operation;
                undoitem.model = redoitem.model;
                undoitem.propname = redoitem.propname;
                undoitem.undorelatedfunction = redoitem.undorelatedfunction;
                if (redoitem.model) {
                    setPropertyValue(redoitem.model, redoitem.propname, redoitem.value);
                }


                if (redoitem.undorelatedfunction) {
                    if (typeof redoitem.undorelatedfunction === "function") {
                        var model = redoitem.model;
                        $timeout(function () {
                            if (redoitem.undorelatedfunction.length > 0) {
                                redoitem.undorelatedfunction(model);
                            } else {
                                redoitem.undorelatedfunction();
                            }
                        });
                    } else {
                        var curelescope = getCurrentFileScope();
                        if (curelescope != undefined) {
                            curelescope[redoitem.undorelatedfunction](redoitem.value);
                        }
                    }
                }
                curscope.UndoList.push(undoitem);
            }
        }
        else if (redoitem.operation == "Add") {
            if (redoitem.collection) {
                $rootScope.PushItem(redoitem.item, redoitem.collection, redoitem.undorelatedfunction, true);
            }
        }
        else if (redoitem.operation == "Insert") {
            if (redoitem.collection) {
                $rootScope.InsertItem(redoitem.item, redoitem.collection, redoitem.index, true);
            }
        }
        else if (redoitem.operation == "Delete") {
            if (redoitem.collection) {
                $rootScope.DeleteItem(redoitem.item, redoitem.collection, redoitem.undorelatedfunction, true);
            }
        }
        else {
            undoitem.position = redoitem.position;
            curscope.UndoList.push(undoitem);
        }

    };

    $rootScope.GetSessionStorageObject = function (fileid) {
        var sessionstorageitem;
        if ($rootScope.UndoRedoList.hasOwnProperty(fileid)) {
            sessionstorageitem = $rootScope.UndoRedoList[fileid];
        }
        else {
            var newobj = {
                FileName: fileid, UndoList: [], RedoList: []
            };
            $rootScope.UndoRedoList[fileid] = newobj;
            sessionstorageitem = newobj;
        }
        return sessionstorageitem;
    };

    $rootScope.UndRedoBulkOp = function (optype) {

        //var curscopeDivId = $rootScope.currentopenfile.file.FileName;

        var curscopeDivId;
        if ($rootScope.isConfigureSettingsVisible) {
            curscopeDivId = "SettingsPage";
        }
        else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            curscopeDivId = $rootScope.currentopenfile.file.FileName;
        }
        if (curscopeDivId != undefined) {
            var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
            if (sessionstorageitem) {

                sessionstorageitem.RedoList = [];

                if (sessionstorageitem.UndoList.length > 0) {
                    var lastitem = sessionstorageitem.UndoList[sessionstorageitem.UndoList.length - 1];
                    if (lastitem.position == "Start") {
                        sessionstorageitem.UndoList.pop();
                    } else {
                        var undoitem = {
                        };
                        undoitem.position = optype;
                        sessionstorageitem.UndoList.push(undoitem);
                    }
                }
                else {
                    var undoitem = {
                    };
                    undoitem.position = optype;
                    sessionstorageitem.UndoList.push(undoitem);
                }
            }
        }
    };


    $rootScope.EditPropertyValue = function (oldvalue, model, propname, newvalue, undorelatedfunction) {
        /// <summary>Updates the given property of a given object and sets the old value of the property of a given object, in the undo stack.</summary>
        /// <param name="oldvalue" type="any">old value of the property.</param>
        /// <param name="model" type="any">object whose property is to be changed.</param>
        /// <param name="propname" type="string">name of the property whose value needs to be changed.</param>
        /// <param name="newvalue" type="any">name of the property whose value needs to be changed.</param>
        if (typeof oldvalue === "undefined") {
            oldvalue = "";
        }
        else if (angular.isString(oldvalue)) {
            oldvalue = oldvalue.trim();
        }

        if (typeof newvalue === "undefined") {
            newvalue = "";
        }
        else if (angular.isString(newvalue)) {
            newvalue = newvalue.trim();
        }

        if (oldvalue != newvalue) {
            var curscopeDivId;
            if ($rootScope.isConfigureSettingsVisible) {
                curscopeDivId = "SettingsPage";
            }
            else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
                curscopeDivId = $rootScope.currentopenfile.file.FileName;
            }
            if (curscopeDivId != undefined) {
                var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
                if (sessionstorageitem) {
                    sessionstorageitem.RedoList = [];
                    var undoitem = {
                    };
                    undoitem.operation = 'Edit';
                    undoitem.value = oldvalue;
                    undoitem.model = model;
                    undoitem.propname = propname;
                    undoitem.undorelatedfunction = undorelatedfunction;
                    sessionstorageitem.UndoList.push(undoitem);
                }

                var scope = getScopeByFileName(curscopeDivId);
                if (scope) {
                    if (!scope.isDirty) {
                        scope.isDirty = true;
                    }
                }
            }

            model[propname] = newvalue;

        }
    };

    $rootScope.PushItem = function (item, collection, undorelatedfunction, isfromredo) {
        var index;
        if (collection) {
            index = collection.push(item);
            index = index - 1;
        }
        var curscopeDivId;
        if ($rootScope.isConfigureSettingsVisible) {
            curscopeDivId = "SettingsPage";
        }
        else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            curscopeDivId = $rootScope.currentopenfile.file.FileName;
        }
        // var curscopeDivId = $rootScope.currentopenfile.file.FileName;
        if (curscopeDivId != undefined) {
            var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);


            if (!isfromredo) {
                sessionstorageitem.RedoList = [];

            }
            else {
                if (undorelatedfunction) {
                    var curelescope = getCurrentFileScope();
                    if (curelescope != undefined) {
                        if (collection.length > 0) {
                            curelescope[undorelatedfunction](collection[index]);
                        }
                    }
                }
            }
            var undoitem = {
            };
            undoitem.operation = 'Add';
            undoitem.collection = collection;
            undoitem.item = item;
            undoitem.index = index;
            undoitem.undorelatedfunction = undorelatedfunction;

            sessionstorageitem.UndoList.push(undoitem);

            var scope = getScopeByFileName(curscopeDivId);
            if (scope) {
                if (!scope.isDirty) {
                    scope.isDirty = true;
                }
            }
        }

    };

    $rootScope.InsertItem = function (item, collection, index, isfromredo) {

        if (collection) {
            collection.splice(index, 0, item);
        }
        var curscopeDivId;
        if ($rootScope.isConfigureSettingsVisible) {
            curscopeDivId = "SettingsPage";
        }
        else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            curscopeDivId = $rootScope.currentopenfile.file.FileName;
        }
        if (curscopeDivId != undefined) {
            var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
            if (!isfromredo) {
                sessionstorageitem.RedoList = [];
            }
            var undoitem = {
            };
            undoitem.operation = 'Insert';
            undoitem.collection = collection;
            undoitem.item = item;
            undoitem.index = index;

            sessionstorageitem.UndoList.push(undoitem);
            var scope = getScopeByFileName(curscopeDivId);
            if (scope) {
                if (!scope.isDirty) {
                    scope.isDirty = true;
                }
            }
        }


    };

    $rootScope.DeleteItem = function (item, collection, undorelatedfunction, isfromredo) {

        var index;
        if (collection) {
            index = collection.indexOf(item);
            collection.splice(index, 1);
        }
        var curscopeDivId;
        if ($rootScope.isConfigureSettingsVisible) {
            curscopeDivId = "SettingsPage";
        }
        else if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileName) {
            curscopeDivId = $rootScope.currentopenfile.file.FileName;
        }

        if (curscopeDivId != undefined) {
            var sessionstorageitem = $rootScope.GetSessionStorageObject(curscopeDivId);
            if (!isfromredo) {
                sessionstorageitem.RedoList = [];
            }
            else {
                if (undorelatedfunction) {
                    var curelescope = getCurrentFileScope();
                    if (curelescope != undefined) {
                        if (collection.length > 0) {
                            if (index > 0) {
                                curelescope[undorelatedfunction](collection[index - 1]);
                            }
                            else {
                                curelescope[undorelatedfunction](collection[0]);
                            }
                        }
                        else {
                            curelescope[undorelatedfunction](undefined);
                        }
                    }
                }
            }
            var undoitem = {
            };
            undoitem.operation = 'Delete';
            undoitem.collection = collection;
            undoitem.item = item;
            undoitem.index = index;
            undoitem.undorelatedfunction = undorelatedfunction;

            sessionstorageitem.UndoList.push(undoitem);
            var scope = getScopeByFileName(curscopeDivId);
            if (scope) {
                if (!scope.isDirty) {
                    scope.isDirty = true;
                }
            }
        }

    };


    $scope.canUndo = function () {
        if ($rootScope.currentopenfile) {
            return $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName] && $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName].UndoList && $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName].UndoList.length > 0;
        }
        else if ($rootScope.isConfigureSettingsVisible) {
            return $rootScope.UndoRedoList.SettingsPage && $rootScope.UndoRedoList.SettingsPage.UndoList && $rootScope.UndoRedoList.SettingsPage.UndoList.length > 0;
        } else {
            return false;
        }
    };

    $scope.canRedo = function () {
        if ($rootScope.currentopenfile) {
            return $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName] && $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName].RedoList && $rootScope.UndoRedoList[$rootScope.currentopenfile.file.FileName].RedoList.length > 0;
        }
        else if ($rootScope.isConfigureSettingsVisible) {
            return $rootScope.UndoRedoList.SettingsPage && $rootScope.UndoRedoList.SettingsPage.RedoList && $rootScope.UndoRedoList.SettingsPage.RedoList.length > 0;
        } else {
            return false;
        }
    };

    //#endregion
    $scope.receiveProjectLoadError = function (error) {
        $rootScope.$evalAsync(function () {

            $rootScope.projectLoadError = true;
            $rootScope.projectLoadErrormsg = error;
        });
    };

    $rootScope.showExceptionDetails = function (exception) {
        $rootScope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            $rootScope.IsProjectLoaded = false;
        });
        var newScope = $rootScope.$new();
        var messages = [];
        if (exception) {
            messages.push(exception.Message);
            if (exception.InnerException) {
                messages.push(exception.InnerException.Message);
            }
            newScope.errorMessage = messages.join("\n");
            newScope.stackTrace = exception.StackTraceString;
        }
        newScope.sendErrorReport = function () {
            if (exception) {
                hubMain.server.sendErrorReport(exception);
            }
            dialog.close();
        };
        newScope.closeDialog = function () {
            dialog.close();
        };
        var dialog = $rootScope.showDialog(newScope, "Exception Details", "Common/views/ExceptionDetails.html");
    };

    $rootScope.showDialog = function (dialogScope, title, template, options) {
        var div = $("<div id='dialog' tabindex='0'></div>");
        var prevDialogBounds = {};
        function templateSuccess(response) {
            var defaultOptions = {
                position: {
                    my: "center", at: "center", of: window
                },
                minWidth: 50,
                minHeight: 50,
                width: 500,
                height: 400,
                modal: true,
                //show: { effect: "scale", duration: 500 },
                //hide: { effect: "scale", duration: 500 }
            };
            if (options) {
                if (options.beforeClose) {
                    defaultOptions.beforeClose = options.beforeClose;
                }
                if (options.height) {
                    defaultOptions.height = options.height;
                }
                if (options.width) {
                    defaultOptions.width = options.width;
                }
                if (options.minHeight) {
                    defaultOptions.minHeight = options.minHeight;
                    if (defaultOptions.height < defaultOptions.minHeight) {
                        defaultOptions.height = defaultOptions.minHeight;
                    }
                }
                if (options.minWidth) {
                    defaultOptions.minWidth = options.minWidth;
                    if (defaultOptions.width < defaultOptions.minWidth) {
                        defaultOptions.width = defaultOptions.minWidth;
                    }
                }

                if (options.modal != undefined && options.modal == false) {
                    defaultOptions.modal = false;
                }
            }
            function onOpen() {
                if (options && options.open) {
                    options.open();
                }
                $(".ui-dialog-titlebar-close").hide();
                if (options && options.showclose) {
                    var closebutton = $('<span style="cursor:pointer;color:black;float:right">X</span>');
                    closebutton.click(function (event) {
                        div.dialog("close");
                    });
                    div.closest(".ui-dialog").find(".ui-dialog-titlebar").append(closebutton);
                }
                if (options && options.isInfoDialog) {
                    $('.ui-dialog-content').addClass('info-dialog');
                }

                if (options && options.isAlert) {
                    $(".ui-widget-header").last().addClass("sg-alert-header");
                }
                div.closest(".ui-dialog").find(".ui-dialog-titlebar").on("dblclick", onDoubleClickTitleBar);
                updateTitle(title);

                setDialogSizeAndPosition(defaultOptions);

                div.on("keydown", function (event) {
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                    preventDefaultSaveUndoRedo(event);
                });
                div.on("keypress", function (event) {
                    if (event.which == $.ui.keyCode.ENTER || event.keyCode == $.ui.keyCode.ENTER) {
                        div.find(".dialog-btn-default:visible:enabled:first").trigger("click");
                    }
                    else if (event.which == $.ui.keyCode.ESCAPE || event.keyCode === $.ui.keyCode.ESCAPE) {
                        if (options && options.closeOnEscape) {
                            div.dialog("close");
                        }
                    }
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                });

            }

            function onDoubleClickTitleBar() {
                var dialogOuterDiv = div.closest(".ui-dialog");
                if (dialogOuterDiv && dialogOuterDiv.length > 0) {
                    if (dialogOuterDiv.height() === defaultOptions.height) {
                        prevDialogBounds = { height: dialogOuterDiv.height(), width: dialogOuterDiv.width(), top: dialogOuterDiv.position().top, left: dialogOuterDiv.position().left };
                        var windowHeight = window.outerHeight - 100;
                        var windowWidth = window.outerWidth - 50;
                        setDialogSizeAndPosition({ height: windowHeight, width: windowWidth, top: 0, left: 12.5 });
                    }
                    else {
                        if (prevDialogBounds) {
                            setDialogSizeAndPosition(prevDialogBounds);
                            prevDialogBounds = null;
                        }
                    }
                }
            }
            function afterClose() {
                if (options && options.close) {
                    options.close();
                }
                div.dialog("destroy");
                dialogScope.$destroy();
                div.remove();
            }
            function onFocus() {
                if (options && options.focus) {
                    options.focus();
                }
            }
            function setDialogSizeAndPosition(bounds) {
                var dialogDiv = div.closest(".ui-dialog");
                if (bounds && dialogDiv && dialogDiv.length > 0) {
                    if (bounds.height) {
                        dialogDiv.css("height", bounds.height + "px");
                    }
                    if (bounds.width) {
                        dialogDiv.css("width", bounds.width + "px");
                    }
                    if (bounds.top) {
                        dialogDiv.css("top", bounds.top + "px");
                    }
                    else {
                        dialogDiv.css("top", "calc((100% - " + bounds.height + "px)/2)");
                    }

                    if (bounds.left) {
                        dialogDiv.css("left", bounds.left + "px");
                    }
                    else {
                        dialogDiv.css("left", "calc((100% - " + bounds.width + "px)/2)");
                    }
                }
            }
            defaultOptions.open = onOpen;
            defaultOptions.close = afterClose;
            defaultOptions.focus = onFocus;
            var innerDiv = $("<div id='ScopeId_{{$id}}' class='full-height'></div>");

            innerDiv.html(response);

            div.append(innerDiv);

            $("#main-wrapper").append(div);
            $compile(div)(dialogScope);
            div.dialog(defaultOptions);
        }
        function templateError(response) {
            $SgMessagesService.Message('Message', "An error occured while getting the temeplate html.");
        }
        function closeDialog() {
            div.dialog("close");
        }
        function updateTitle(newTitle) {
            div.siblings(".ui-dialog-titlebar").find("span[class='ui-dialog-title']").text(newTitle).attr("title", newTitle);
        }

        if (options && options.isInlineHtml) {
            templateSuccess(template);
        }
        else {
            var path = [window.location.protocol, "//", window.location.host, "/", getWebApplicationName(), "/", template].join("");
            $.ajax({
                url: path,
                type: 'GET',
                async: false,
                success: templateSuccess,
                error: templateError
            });
        }

        return {
            close: closeDialog,
            updateTitle: updateTitle
        };
    };
    //$scope.toggleProjectsList = function (event, isfromnopen) {
    //    if ($('#projectsList').is(':visible')) {
    //        $('#projectsList').slideUp();
    //        $scope.showProjectList = false;
    //    }
    //    else if (!isfromnopen) {
    //        $scope.showProjectList = true;
    //        $('#projectsList').slideDown();
    //    }
    //    stopPropogation(event);
    //}



    //#region Common Methods

    $rootScope.getEntityAttributeIntellisense = function (entityName, includePrivate, isOnlyOneToOne, isOnlyOneToMany, isshowcolumns, isshowcdocollection, isshowexpression, isShowProperty) {
        var data = [];
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entities = entityIntellisenseList;
        if (entities && entities.length > 0) {
            var entity = entities.filter(function (x) {
                return x.ID == entityName;
            });
            if (entity.length > 0) {
                var attributes = entity[0].Attributes;
                if (!includePrivate) {
                    //attributes = attributes.filter(function (x) {
                    //    return (!x.IsPrivate|| x.IsPrivate == undefined)
                    //});
                    var filterAttributes = [];
                    for (var i = 0; i < attributes.length; i++) {
                        if ((attributes[i].IsPrivate + "").toLowerCase() == "false" || attributes[i].IsPrivate == undefined) {
                            $scope.filterEntityAttributeBasedOnCriteria(attributes[i], isOnlyOneToOne, isOnlyOneToMany, filterAttributes, isshowcolumns, isshowcdocollection, isshowexpression, isShowProperty);
                        }
                    }
                    attributes = filterAttributes;
                }
                else {
                    var filterAttributes = [];
                    for (var i = 0; i < attributes.length; i++) {
                        $scope.filterEntityAttributeBasedOnCriteria(attributes[i], isOnlyOneToOne, isOnlyOneToMany, filterAttributes, isshowcolumns, isshowcdocollection, isshowexpression, isShowProperty);

                    }
                    attributes = filterAttributes;
                }
                data = data.concat(attributes);
            }
        }
        return data;
    };

    $scope.filterEntityAttributeBasedOnCriteria = function (attribute, isOnlyOneToOne, isOnlyOneToMany, filterAttributes, isshowcolumns, isshowcdocollection, isshowexpression, isShowProperty) {
        if (isshowcolumns) {
            if (attribute.Type && attribute.Type == "Column") {
                filterAttributes.push(attribute);
            }
        }
        if (isOnlyOneToOne) {
            if (attribute.DataType == "Object") {
                filterAttributes.push(attribute);
            }
        }
        if (isOnlyOneToMany) {
            if (attribute.DataType == "Collection" || attribute.DataType == "List") {
                filterAttributes.push(attribute);
            }
        }
        if (isshowcdocollection) {
            if (attribute.DataType == "CDOCollection") {
                filterAttributes.push(attribute);
            }
        }
        if (isshowexpression) {
            if (attribute.Type == "Expression") {
                filterAttributes.push(attribute);
            }
        }
        if (isShowProperty && attribute.Type && attribute.Type.toLowerCase() == "property") {
            filterAttributes.push(attribute);
        }
        if (!isOnlyOneToOne && !isOnlyOneToMany && !isshowcdocollection && !isshowcolumns && !isShowProperty) {
            if (attribute.Type != "Expression") {
                filterAttributes.push(attribute);
            }
        }

    };

    $scope.onnewfileclick = function (itm) {
        if ($scope.CurrentProjectName) {
            var newScope = $scope.$new();
            newScope.SelectedCreationOption = itm;
            newScope.title = "Create New Entity";
            newScope.dialogForNew = $rootScope.showDialog(newScope, newScope.title, "CreateNewObject/views/CreateNewObject.html", {
                width: 830, height: 700
            });
        }
    };

    $scope.SetClassForHeaderPanel = function () {
        if ($rootScope.currentopenfile) {
            if ($rootScope.currentopenfile.file.FileType == 'Maintenance' || $rootScope.currentopenfile.file.FileType == 'Wizard' || $rootScope.currentopenfile.file.FileType == 'Lookup' || $rootScope.currentopenfile.file.FileType == 'UserControl' || $rootScope.currentopenfile.file.FileType == 'Tooltip') {
                $scope.ClassForHeaderPanel = "form-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'Entity') {
                $scope.ClassForHeaderPanel = "entity-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'FormLinkLookup' || $rootScope.currentopenfile.file.FileType == 'FormLinkMaintenance' || $rootScope.currentopenfile.file.FileType == 'FormLinkWizard') {
                $scope.ClassForHeaderPanel = "htmlform-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'Correspondence' || $rootScope.currentopenfile.file.FileType == 'PDFCorrespondence') {
                $scope.ClassForHeaderPanel = "correspondence-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'LogicalRule' || $rootScope.currentopenfile.file.FileType == 'DecisionTable' || $rootScope.currentopenfile.file.FileType == 'ExcelMatrix') {
                $scope.ClassForHeaderPanel = "rule-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'ExcelScenario' || $rootScope.currentopenfile.file.FileType == 'ObjectScenario' || $rootScope.currentopenfile.file.FileType == 'ParameterScenario') {
                $scope.ClassForHeaderPanel = "scenario-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'BPMN') {
                $scope.ClassForHeaderPanel = "bpm-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'InboundFile' || $rootScope.currentopenfile.file.FileType == 'OutboundFile') {
                $scope.ClassForHeaderPanel = "file-header";
            }
            else if ($rootScope.currentopenfile.file.FileType == 'Report') {
                $scope.ClassForHeaderPanel = "report-header";
            }
            else {
                $scope.ClassForHeaderPanel = "otherfile-header";
            }
        }
    };

    //#endregion

    $scope.LogoutSession = function () {
        var userCookie = $cookies.getObject("UserDetails");

        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        newScope.lstNotDirtyFiles = [];
        var temp = JSON.stringify($rootScope.lstopenedfiles);
        var tempOpenFile = JSON.parse(temp);
        newScope.logout = function () {
            if (userCookie && JSON.parse(userCookie).UserName != null) {
                $rootScope.IsLoading = true;
                $.ajax({
                    url: "api/Login/LogOutUser",
                    type: 'GET',
                    async: false,
                    success: successCallback,
                    error: errorCallBack
                });
                function successCallback(response) {
                    $cookies.remove("UserDetails");
                    $rootScope.resetRootScopeVarible();
                    toastr.success("User has logged out successfully");
                    $rootScope.IsLogin = false;
                    $rootScope.IsAdmin = false;
                    $rootScope.IsLogOut = true;
                    $rootScope.IsLoading = false;
                    $rootScope.setCurrentUserName();
                    $rootScope.isConfigureSettingsVisible = false;
                    $rootScope.isstartpagevisible = false;
                }
                function errorCallBack(exceptionData) {
                    $rootScope.IsLoading = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                }
            }
        };
        if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
            for (var i = 0; i < tempOpenFile.length; i++) {
                var scope = getScopeByFileName(tempOpenFile[i].file.FileName);
                if (scope) {
                    if (scope.isDirty) {
                        newScope.lstDirtyFiles.push(tempOpenFile[i]);
                    }
                    else {
                        newScope.lstNotDirtyFiles.push(tempOpenFile[i]);
                    }
                }
            }
        }

        if (newScope.lstDirtyFiles.length > 0) {
            var dialog = $rootScope.showDialog(newScope, "UnSaved File List", "Common/views/UnsavedFileListDialog.html");
            newScope.closeSaveListDialog = function (buttontext) {
                var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                if (buttontext == "Yes") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        scope.onsavefileclick(newScope.lstDirtyFiles[i]);
                        $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                    newScope.logout();
                }
                else if (buttontext == "No") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                    newScope.logout();
                }
                dialog.close();
            };
        }
        else {
            for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                $rootScope.closeFile(newScope.lstNotDirtyFiles[i].file.FileName);
            }
            newScope.logout();
        }

    };
    $scope.ChangePassword = function () {
        if ($rootScope.IsActiveDirectoryUser) {
            toastr.error("Active Directory User cannot change password.");
            return;
        }

        var userCookie = $cookies.getObject("UserDetails");
        if (userCookie && JSON.parse(userCookie).UserName != null) {
            var newScope = $scope.$new(true);
            newScope.showOldPassword = true;
            newScope.userDetails = JSON.parse(userCookie);
            newScope.changePasswordDialog = $rootScope.showDialog(newScope, "Change Password", "StartUp/views/ChangePasswordDialog.html", { height: 460, width: 500 });
            newScope.changePasswordCallBack = function (aObjResponse) {
                toastr.success("Password changed successfully.");
                newScope.closeChangePasswordDialog();
            };
            newScope.closeChangePasswordDialog = function () {
                newScope.changePasswordDialog.close();
            };
        }

    };
    //#region Toolbar  options

    $scope.lstTools = ['Run Results', 'Rule Errors', 'Refresh Master Data', 'Get Rule Configuration Results', 'Custom Settings', 'Project Configuration',
        'View DataBase Object File', 'Execute Query', 'Audit Log', 'Build Entity Rule', 'Build Object Rule', 'Build Rules Assembly - Debug Mode',
        'Build Rules Assembly - Release Mode', 'Build Project', 'Add Resources', 'Constant', 'Advance Search (Ctrl+Shift+F)', 'Convert To HTX', 'New Entity Diagram',
        'Set Code Group', 'Keyboard Shortcuts', 'Table/do Report', 'Validation Utility'];

    var count = 1;
    $scope.ShowSelectedTool = function (toolname) {
        //toggleToolsList();
        if ($scope.CurrentProjectName) {
            $scope.RuleNameReadOnly = false;
            $scope.RuleName = undefined;
            $scope.EffectiveDate = undefined;
            $scope.ToDate = undefined;
            $scope.StrExecutionId = undefined;
            $scope.ErrorStatus = undefined;
            $scope.BusinessObjectName = undefined;

            $scope.lstRunresults = null;
            $scope.lstRuleErrors = null;
            $scope.lstGetRuleConfigResults = null;

            if (toolname == 'Run Results') {
                $scope.OpenRunResultClick(true, "");
            }
            else if (toolname == 'Rule Errors') {
                $scope.OpenRuleErrorsClick(true, "");
            }
            else if (toolname == 'Get Rule Configuration Results') {
                $scope.OpenGetRuleConfigurationClick();
                //var dialog = ngDialog.open({
                //    template: 'getRules', className: 'ngdialog-theme-default dialogwidth', closeByDocument: false, scope: $scope
                //});
                //GetRuleConfigDialog = dialog.Id;
            }
            else if (toolname == "Custom Settings") {
                if ($scope.customSettingsFileInfo) {
                    $rootScope.openFile($scope.customSettingsFileInfo);
                }
            }
            else if (toolname == "Check Queries") {
                $scope.OpenCheckQueriesClick();
                //var dialog = ngDialog.open({
                //    template: 'checkqueries', className: 'ngdialog-theme-default dialogwidth', closeByDocument: false, scope: $scope
                //});
                //CheckQueriesDialog = dialog.Id;
            }
            else if (toolname == "Project Configuration") {
                if ($scope.projectConfigurationFileInfo) {
                    $rootScope.openFile($scope.projectConfigurationFileInfo);
                }
            }
            else if (toolname == "View DataBase Object File") {
                $rootScope.openFile($scope.entTableSchemaInfo);
            }
            else if (toolname == "Audit Log") {
                if ($scope.auditLogFileInfo) {
                    $rootScope.openFile($scope.auditLogFileInfo);
                }
            }
            else if (toolname == "Execute Query") {
                var Id = count++;
                var obj = {
                    FileName: 'ExecuteQuery' + '(' + Id + ')', FileType: 'ExecuteQuery', FileID: Id, FilePath: ''
                };
                $rootScope.openFile(obj);
            }

            else if (toolname == "Build Entity Rule") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.onBuildRules().done(function (data) {
                    $rootScope.receiveRuleBuildResult(data);
                });
            } else if (toolname == "Build Object Rule") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.onBuildObjectRules().done(function (data) {
                    $rootScope.receiveRuleBuildResult(data);
                });
            } else if (toolname == "Build Rules Assembly - Debug Mode") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.onConvertDLL("Debug").done(function (data) {
                    $rootScope.receiveRuleBuildResult(data);
                });
            }
            else if (toolname == "Build Rules Assembly - Release Mode") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.onConvertDLL("Release").done(function (data) {
                    $rootScope.receiveRuleBuildResult(data);
                });
            }
            else if (toolname == "Build Project") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.onBuildProjectClick();
            }
            else if (toolname == "Refresh Master Data") {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = true;
                });
                hubMain.server.refreshMasterData().done(function () {
                    //$rootScope.IsLoading = true;
                    hubMain.server.getCommonIntellisenseData();
                    toastr.success("Master Data is Refreshed");
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                });
            }
            else if (toolname == "Add Resources") {
                var newScope = $scope.$new(true);

                hubMain.server.getResourcesFormList().done(function (data) {
                    newScope.filelist = JSON.parse(data);
                    newScope.$evalAsync();
                });
                newScope.onSearch = function (folderName) {
                    newScope.lstResourceFormList = [];
                    newScope.IsSelectAll = false;
                    angular.forEach(newScope.filelist, function (item) {
                        if (item.Location == folderName) {
                            newScope.lstResourceFormList.push(item);
                        }
                    });
                    newScope.resetModelData();
                };

                newScope.onToggleSelectionClick = function (isSelectAll) {
                    var res = $filter('filter');
                    newScope.queryData = $filter('filter')(newScope.lstResourceFormList, { Location: newScope.folderName });
                    if (newScope.queryData && newScope.queryData.length > 0) {
                        angular.forEach(newScope.queryData, function (obj) {
                            obj.IsChecked = isSelectAll;
                        });
                    }
                };
                newScope.preserveCheckedFiles = [];

                newScope.selectFormId = function (objform) {
                    newScope.selectedFormId = objform;
                }
                newScope.onResourceNotSetRowClick = function (objResource) {
                    newScope.selectedResourceNotSet = objResource;
                }
                newScope.onAddResource = function () {
                    var objFormDataList = [];
                    if (newScope.lstResourceFormList) {
                        var checkedList = newScope.lstResourceFormList.filter(function (item) {
                            return item.IsChecked;
                        });
                        angular.copy(checkedList, objFormDataList);
                        newScope.resetModelData();
                    }

                    newScope.IsProcessing = true;
                    if (objFormDataList.length > 0) {
                        hubMain.server.getFileAddResources(objFormDataList).done(function (data) {
                            newScope.resourceSet = data.ResourceSetDetails;
                            newScope.resourceNotSet = data.ResourceNotSetDetails;
                            newScope.formResource = data.FormResourceDetails;
                            //newScope.readOnlyFiles = data.ReadOnlyFiles;
                            newScope.IsProcessing = false;
                            newScope.$evalAsync();
                        });
                    }
                };
                newScope.displayedRecordsNotSet = 25;
                newScope.OnScrollNotSet = function () {
                    newScope.displayedRecordsNotSet += 25;
                    newScope.$evalAsync();
                };

                newScope.displayedRecordsSet = 25;
                newScope.OnScrollSet = function () {
                    newScope.displayedRecordsSet += 25;
                    newScope.$evalAsync();
                };

                newScope.displayedRecordsForm = 25;
                newScope.OnScrollForm = function () {
                    newScope.displayedRecordsForm += 25;
                    newScope.$evalAsync();
                };

                newScope.resetModelData = function () {
                    newScope.resourceSet = undefined;
                    newScope.resourceNotSet = undefined;
                    newScope.formResource = undefined;
                    //newScope.readOnlyFiles = undefined;
                };

                newScope.onExport = function () {
                    $rootScope.IsLoading = true;
                    hubMain.server.exportResources().done(function () {
                        $rootScope.$apply(function () {
                            $rootScope.IsLoading = false;
                        });
                    });
                };

                var strobj;
                newScope.lstAllFormModel = [];
                newScope.onSaveResources = function () {
                    //saveResourceValue(newScope.formResource);
                    //saveResourceValue(newScope.resourceNotSet);
                    //saveResourceValue(newScope.resourceSet);

                    newScope.selectedFiles = [];
                    if (newScope.formResource) {
                        $rootScope.IsLoading = true;
                        for (var i = 0; i < newScope.formResource.length; i++) {
                            for (var j = 0; j < newScope.lstResourceFormList.length; j++) {
                                if (newScope.formResource[i].strFormId == newScope.lstResourceFormList[j].FileName) {
                                    newScope.selectedFiles.push(newScope.lstResourceFormList[j]);
                                }
                            }
                        }

                        var resourceDetails = { FormResourceDetails: newScope.formResource, ResourceNotSetDetails: newScope.resourceNotSet, ResourceSetDetails: newScope.resourceSet };
                        strobj = JSON.stringify(resourceDetails);
                        if (strobj.length < 32000) {
                            hubMain.server.getSaveResources(resourceDetails, newScope.selectedFiles, "ScopeId_" + newScope.$id).done(function () {
                                //$rootScope.$apply(function () {
                                //    $rootScope.IsLoading = false;
                                //    toastr.success("Resources Saved successfully");
                                //    newScope.CloseAddResourcesDialog();
                                //});
                            });
                        }
                        else {
                            var strpacket = "";
                            var lstDataPackets = [];
                            var count = 0;
                            for (var i = 0; i < strobj.length; i++) {
                                count++;
                                strpacket = strpacket + strobj[i];
                                if (count == 32000) {
                                    count = 0;
                                    lstDataPackets.push(strpacket);
                                    strpacket = "";
                                }
                            }
                            if (count != 0) {
                                lstDataPackets.push(strpacket);
                            }
                            newScope.saveResourcePacketsToServer(lstDataPackets, newScope.selectedFiles, "ScopeId_" + newScope.$id);
                        }
                    }
                };

                var saveResourceValue = function (allResources) {
                    for (var i = 0; i < allResources.length; i++) {
                        if (allResources[i].strControlId == null && allResources[i].strFormId != null) {
                            if (allResources[i].formModel != null) {
                                allResources[i].formModel.dictAttributes.sfwResource = allResources[i].strResource;
                            }
                        }
                        else if (allResources[i].strControlId != null && allResources[i].strFormId != null) {
                            allResources[i].controlModel.dictAttributes.sfwResource = allResources[i].strResource;
                        }

                        if (newScope.lstAllFormModel.indexOf(allResources[i].formModel) == -1) {
                            newScope.lstAllFormModel.push(allResources[i].formModel);
                        }
                    }
                };

                newScope.saveResourcePacketsToServer = function (lstpackets, lstResourceFormList, scopeId) {
                    for (var i = 0; i < lstpackets.length; i++) {
                        hubMain.server.packetSaveResource(lstpackets[i], lstpackets.length, lstResourceFormList, i, scopeId).done(function () {
                            //$rootScope.IsLoading = false;
                            //newScope.$evalAsync();
                        });
                    }
                };

                newScope.receiveSaveResourceException = function (lstResourceSaveException) {
                    if (lstResourceSaveException.length > 0) {
                        //Show Exception Message
                        var newErrorScope = newScope.$new(true);
                        newErrorScope.lstErrorMessage = lstResourceSaveException;
                        $rootScope.IsLoading = false;
                        newErrorScope.dialogError = $rootScope.showDialog(newErrorScope, "Exception Details", "Tools/views/ResourceException.html", { width: 500, height: 500 });

                        newErrorScope.closeDialog = function () {
                            newErrorScope.dialogError.close();
                        };
                        $rootScope.$apply(function () { });
                    }
                    else {
                        $rootScope.$apply(function () {
                            $rootScope.IsLoading = false;
                            toastr.success("Resources Saved successfully");
                            newScope.CloseAddResourcesDialog();
                        });
                    }
                };

                var addResourcesDialog = $rootScope.showDialog(newScope, "Update Resources", "Tools/views/AddResources.html", {
                    width: 1000, height: 650
                });

                newScope.CloseAddResourcesDialog = function () {
                    addResourcesDialog.close();
                };

                //Functionality to implement Tab On the Page
                newScope.tab = 1;
                newScope.setTab = function (newTab) {
                    newScope.tab = newTab;
                };
                newScope.isSet = function (tabNum) {
                    return newScope.tab === tabNum;
                };
            }
            else if (toolname == "Convert To HTX") {
                var temObj = ConfigurationFactory.getLastProjectDetails();
                if (temObj && temObj != null && ConfigurationFactory.getLastProjectDetails().HtmlTemplatesLocation) {
                    var newScope = $scope.$new();
                    var convertToHtxDialog = $rootScope.showDialog(newScope, "Convert To HTX", "Tools/views/ConvertToHtx.html", { width: 1100, height: 510 });
                    newScope.CloseDialog = function () {
                        convertToHtxDialog.close();
                    };
                    newScope.CancelConvertion = function () {
                        // CancelConverSion
                        hubcontext.hubConvertToHtx.server.stopConverSion();
                    };
                }
                else {
                    toastr.error("Set HTML Template Location");
                }

            } else if (toolname == "Constant") {
                $scope.constantclick();
            }
            else if (toolname == "Advance Search (Ctrl+Shift+F)") {
                $scope.openSearchFile();
            }
            else if (toolname === "New Entity Diagram") {
                var newEntityDiagramScope = $scope.$new();
                var newEntityDiagramDialog = $rootScope.showDialog(newEntityDiagramScope, "Add Entity", "Tools/views/SelectEntitiesDiagram.html", { width: 650, height: 450 });
                newEntityDiagramScope.onEntityDiagramCancelClick = function () {
                    newEntityDiagramDialog.close();
                };
            }
            else if (toolname === "Set Code Group") {
                var AddCodeGroup = $scope.$new();
                AddCodeGroup.CodeGroupDialog = $rootScope.showDialog(AddCodeGroup, "Set Code Group", "Tools/views/AddCodeGroup.html", { width: 1000, height: 470 });
            }
            else if (toolname === 'Keyboard Shortcuts') {
                var lobjscopeShortcut = $scope.$new();
                lobjscopeShortcut.KeyboardShortcutDialog = $rootScope.showDialog(lobjscopeShortcut, "Keyboard Shortcuts", "Tools/views/KeyboardShortcuts.html", { width: 700, height: 600, showclose: true });
                lobjscopeShortcut.close = function () {
                    lobjscopeShortcut.KeyboardShortcutDialog.close();
                }
            }
            else if (toolname === "Table/do Report") {
                var lobjEntityListScope = $scope.$new();
                hubcontext.hubMain.server.loadTableDoReport().done(function (data) {
                    lobjEntityListScope.$evalAsync(function () {
                        lobjEntityListScope.entityReport = data;
                    });
                });
                lobjEntityListScope.EntityListDialog = $rootScope.showDialog(lobjEntityListScope, "Table/do Report", "Tools/views/EntityReport.html", { width: 700, height: 600, showclose: true });
                lobjEntityListScope.close = function () {
                    lobjEntityListScope.EntityListDialog.close();
                };
                lobjEntityListScope.MergeSelectedFile = function () {
                    var lobjSelectedTabledoReport = { lstDuplicateTableNameEntities: [] };
                    if (lobjEntityListScope.entityReport && lobjEntityListScope.entityReport.lstDuplicateTableNameEntities && lobjEntityListScope.entityReport.lstDuplicateTableNameEntities.length > 0) {
                        for (var i = 0; i < lobjEntityListScope.entityReport.lstDuplicateTableNameEntities.length; i++) {
                            var lcurrTable = lobjEntityListScope.entityReport.lstDuplicateTableNameEntities[i];
                            if (lcurrTable && lcurrTable.IsSelected && lcurrTable.istrMergeEntityName) {
                                delete lcurrTable.IsSelected;
                                lobjSelectedTabledoReport.lstDuplicateTableNameEntities.push(lcurrTable);
                            }
                        }
                        hubcontext.hubMain.server.mergeEntity(lobjSelectedTabledoReport).done(function (data) {
                            var test = "";
                        });
                    }
                };
                lobjEntityListScope.setNewEntity = function (aobjItem, astrEntityName) {
                    if (aobjItem.IsSelected && aobjItem.lstEntityName && aobjItem.lstEntityName.length > 0) {
                        if (astrEntityName) {
                            aobjItem.istrMergeEntityName = astrEntityName;
                        }
                        else {
                            aobjItem.istrMergeEntityName = aobjItem.lstEntityName[0];
                        }
                    }
                    else {
                        aobjItem.istrMergeEntityName = "";
                    }
                };
            } else if (toolname == "Validation Utility") {
                var lobjValidationUtility = $scope.$new(true);
                lobjValidationUtility.ValidationUtilityDialog = $rootScope.showDialog(lobjValidationUtility, "Validation Utility", "Tools/views/ValidationUtility.html", { width: 800, height: 600, showclose: true });
                lobjValidationUtility.close = function () {
                    lobjValidationUtility.ValidationUtilityDialog.close();
                };
            }
        }
    };

    $scope.CloseAddResourcesDialog = function () {
        addResourcesDialog.close();
    };

    //#region Run Result
    $scope.OpenRunResultClick = function (isfromTools, ruleName) {
        var newScope = $scope.$new(true);
        newScope.isValid = true;
        newScope.errorMessage;
        if (!isfromTools) {
            newScope.RuleNameReadOnly = true;
            newScope.RuleName = ruleName;
        }
        else {
            newScope.RuleNameReadOnly = false;
        }

        var addRunResultDialog = $rootScope.showDialog(newScope, "Run Results", "Tools/views/RunResult.html", {
            width: 1100, height: 600
        });
        ComponentsPickers.init();
        newScope.CloseRunResultsDialog = function () {
            addRunResultDialog.close();
        };

        newScope.OnSearchClick = function (Toolname) {
            var dtResults = null;
            var strQuery;
            var lstQuery = [];
            newScope.Toolname = Toolname;
            newScope.lstdata = undefined;
            newScope.runResult.PageIndex = 1;
            newScope.ErrorMessage = "";
            var obj = { "StrExecutionId": newScope.StrExecutionId, "RuleName": newScope.RuleName, "ToDate": newScope.ToDate, "EffectiveDate": newScope.EffectiveDate, "ErrorStatus": newScope.ErrorStatus, "UserName": newScope.UserName, "PrimaryKey": newScope.PrimaryKey, "BusinessObjectField": newScope.BusinessObjectField };

            hubMain.server.executeQueryInTool(obj, Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.ErrorMessage = "";
                if (!data || (data && JSON.parse(data).length == 0)) {
                    newScope.ErrorMessage = "No Records Found.";
                }
                newScope.receieveExecuteQuery(data);
            });
        };
        newScope.OnClearSearchClick = function (Toolname) {
            newScope.ErrorMessage = "";
            newScope.StrExecutionId = "";
            if (!newScope.RuleNameReadOnly) {
                newScope.RuleName = "";
            }
            newScope.ToDate = "";
            newScope.EffectiveDate = "";
            newScope.ErrorStatus = "";
            newScope.UserName = "";
            newScope.PrimaryKey = "";
            newScope.BusinessObjectField = "";
            newScope.lstdata = null;
            newScope.runResult.lstRunresults = null;
            newScope.runResult.IsProcessing = false;
            newScope.ValidateExecutionIdForRunResult();
            newScope.validationForDate();
        };

        newScope.runResult = {
        };
        newScope.viewDiagram = function (data) {
            $rootScope.IsLoading = true;
            $.connection.hubTools.server.loadRunResultDiagram(data.EXECUTION_ID).done(function (result) {
                newScope.viewDiagramSuccessCallback(result);
            }).fail(function (data) {
                //alert(data.Message);
            });
        };
        newScope.viewDiagramSuccessCallback = function (data) {
            if (data.ruleModel) {
                var ruletype = "";
                var strRuleID = "";
                if (data.ruleModel.dictAttributes) {
                    ruletype = data.ruleModel.dictAttributes.sfwRuleType;
                    strRuleID = data.ruleModel.dictAttributes.ID;
                }
                else {
                    ruletype = data.ruleModel.RuleType;
                    strRuleID = data.ruleModel.RuleID;
                }
                closestRule = GetClosestLogicalRule(data.ruleModel, data.effectiveDate, ruletype);
                if (closestRule != undefined) {
                    UpdateTestExecutionFlow(data.scenarioRuleSteps, ruletype, closestRule, "AllRuleSteps");
                    var viewDiagramScope = $scope.$new();
                    viewDiagramScope.IsRunResults = true;
                    viewDiagramScope.objLogicalRule = data.ruleModel;
                    viewDiagramScope.objSelectedLogicalRule = closestRule;
                    //viewDiagramScope.testExecutionResults = $scope.objEntityBasedScenario.SelectedStep.TestExecutionResults;
                    viewDiagramScope.objEntity = data.entityModel;
                    viewDiagramScope.Type = "RunResults";
                    viewDiagramScope.IsDiagramDivExpanded = true;
                    viewDiagramScope.scenarioruntype = "AllRuleSteps"; //$scope.objEntityBasedScenario.SelectedStep.strRunType;
                    viewDiagramScope.viewDiagramDialog = $rootScope.showDialog(viewDiagramScope, "Rule : " + strRuleID + " - Effective Date : " + data.effectiveDate + "[ Elapsed Time:" + data.elapsedTime + "]", "Scenario/views/ViewRuleDiagram.html", { height: 700, width: 1400 });

                    viewDiagramScope.ShowOnlyExecutedPath = function (ablnShowOnlyExecutedPath) {
                        ShowOnlyExecutedPath(ablnShowOnlyExecutedPath, viewDiagramScope.objSelectedLogicalRule);
                    };
                    $rootScope.IsLoading = false;
                }
            }
        };
        newScope.runResult.IsProcessing = false;
        newScope.runResult.PageIndex = 1;

        newScope.OnSearchScrollDown = function () {
            newScope.runResult.IsProcessing = true;
            newScope.runResult.PageIndex = newScope.runResult.PageIndex + 1;

            var obj = { "StrExecutionId": newScope.StrExecutionId, "RuleName": newScope.RuleName, "ToDate": newScope.ToDate, "EffectiveDate": newScope.EffectiveDate, "ErrorStatus": newScope.ErrorStatus, "UserName": newScope.UserName, "PrimaryKey": newScope.PrimaryKey, "BusinessObjectField": newScope.BusinessObjectField };

            hubMain.server.executeQueryInTool(obj, newScope.Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.receieveExecuteQuery(data);
            });
        };

        newScope.receieveExecuteQuery = function (data) {
            newScope.$apply(function () {

                newScope.lstdata = JSON.parse(data);
                newScope.runResult.lstRunresults = newScope.lstdata;
                newScope.runResult.IsProcessing = false;
            });
        };
        newScope.receiveErrorMessageQuery = function (data) {
            newScope.$apply(function () {
                newScope.errorMessage = data;
            });
        };

        newScope.ValidateExecutionIdForRunResult = function (executionId) {
            newScope.isValid = true;
            if (executionId && !$scope.ValidateExecutionId(executionId)) {
                newScope.isValid = false;
            }
        };

        newScope.validationForDate = function (testDate) {
            newScope.isValid = true;
            if (testDate && !$scope.ValidateDate(testDate)) {
                newScope.isValid = false;
            }
        };
    };
    //#endregion

    //#region Rule Errors
    $scope.OpenRuleErrorsClick = function (isfromTools, ruleName) {
        var newScope = $scope.$new(true);
        newScope.isValid = true;
        newScope.errorMessage;
        if (!isfromTools) {
            newScope.RuleNameReadOnly = true;
            newScope.RuleName = ruleName;
        }
        else {
            newScope.RuleNameReadOnly = false;
        }
        var addRuleErrorsDialog = $rootScope.showDialog(newScope, "Rule Errors", "Tools/views/RuleErrors.html", {
            width: 1100, height: 600
        });
        ComponentsPickers.init();
        newScope.CloseRuleErrorsDialog = function () {
            addRuleErrorsDialog.close();
        };

        newScope.OnSearchClick = function (Toolname) {
            var dtResults = null;
            var strQuery;
            var lstQuery = [];
            newScope.Toolname = Toolname;
            newScope.lstdata = undefined;
            newScope.runResult.PageIndex = 1;

            var obj = { "RuleName": newScope.RuleName, "ToDate": newScope.ToDate, "EffectiveDate": newScope.EffectiveDate };

            hubMain.server.executeQueryInTool(obj, Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.ErrorMessage = "";
                if (!data || (data && JSON.parse(data).length == 0)) {
                    newScope.ErrorMessage = "No Result Found.";
                }
                newScope.receieveExecuteQuery(data);
            });
        };
        newScope.runResult = {
        };
        newScope.runResult.IsProcessing = false;
        newScope.runResult.PageIndex = 1;

        newScope.OnSearchScrollDown = function () {
            newScope.runResult.IsProcessing = true;
            newScope.runResult.PageIndex = newScope.runResult.PageIndex + 1;

            var obj = { "RuleName": newScope.RuleName, "ToDate": newScope.ToDate, "EffectiveDate": newScope.EffectiveDate };

            hubMain.server.executeQueryInTool(obj, newScope.Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.receieveExecuteQuery(data);
            });
        };

        newScope.receieveExecuteQuery = function (data) {
            newScope.$apply(function () {
                newScope.lstdata = JSON.parse(data);
                if (newScope.Toolname == 'Rule Errors') {
                    newScope.runResult.lstRuleErrors = newScope.lstdata;
                    if (!(newScope.lstdata && newScope.lstdata.length > 0)) {
                        newScope.ErrorMessage = "No Records Found.";
                    }
                    else {
                        newScope.ErrorMessage = "";
                    }
                }
                newScope.runResult.IsProcessing = false;
            });
        };
        newScope.receiveErrorMessageQuery = function (data) {
            newScope.$apply(function () {
                newScope.errorMessage = data;
            });
        };
        newScope.validationForDate = function (testDate) {
            newScope.isValid = true;
            if (testDate && !$scope.ValidateDate(testDate)) {
                newScope.isValid = false;
            }
        };
    };
    //#endregion


    $scope.ValidateDate = function (testDate) {
        var isValid = true;
        var date_regex = /^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$/;
        if (!(date_regex.test(testDate))) {
            isValid = false;
        }
        return isValid;
    };

    $scope.ValidateExecutionId = function (executionId) {
        var isValid = true;
        var date_regex = /^\d+$/;
        if (!(date_regex.test(executionId))) {
            isValid = false;
        }
        return isValid;
    };
    //#region Get Rule Configuration Results
    $scope.OpenGetRuleConfigurationClick = function (isfromTools, ruleName) {
        var newScope = $scope.$new(true);
        newScope.errorMessage;
        var addRuleConfigurationDialog = $rootScope.showDialog(newScope, "Get Rule Configuration Result", "Tools/views/GetRulesConfiguration.html", {
            width: 1100, height: 600
        });
        ComponentsPickers.init();
        newScope.CloseGetRuleConfigDialog = function () {
            addRuleConfigurationDialog.close();
        };

        newScope.OnSearchClick = function (Toolname) {
            var dtResults = null;
            var strQuery;
            var lstQuery = [];
            newScope.Toolname = Toolname;
            newScope.lstdata = undefined;
            newScope.runResult.PageIndex = 1;

            newScope.strQuery = strQuery;

            var obj = { "StrExecutionId": newScope.StrExecutionId, "RuleName": newScope.RuleName, "BusinessObjectName": newScope.BusinessObjectName, "EffectiveDate": newScope.EffectiveDate };

            hubMain.server.executeQueryInTool(obj, Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.receieveExecuteQuery(data);
            });
        };
        newScope.runResult = {
        };
        newScope.runResult.IsProcessing = false;
        newScope.runResult.PageIndex = 1;

        newScope.OnSearchScrollDown = function () {
            newScope.runResult.IsProcessing = true;
            newScope.runResult.PageIndex = newScope.runResult.PageIndex + 1;

            var obj = { "StrExecutionId": newScope.StrExecutionId, "RuleName": newScope.RuleName, "BusinessObjectName": newScope.BusinessObjectName, "EffectiveDate": newScope.EffectiveDate };
            hubMain.server.executeQueryInTool(obj, newScope.Toolname, newScope.runResult.PageIndex * 15, newScope.$id).done(function (data) {
                newScope.receieveExecuteQuery(data);
            });
        };

        newScope.receieveExecuteQuery = function (data) {
            newScope.$apply(function () {

                newScope.lstdata = JSON.parse(data);
                if (newScope.Toolname == 'Get Rule Configuration Results') {
                    newScope.runResult.lstGetRuleConfigResults = newScope.lstdata;
                    if (!(newScope.lstData && newScope.lstData.length > 0)) {
                        newScope.errorMessage = "No Records Found.";
                    }
                }
                newScope.runResult.IsProcessing = false;
            });
        };
        newScope.receiveErrorMessageQuery = function (data) {
            newScope.$apply(function () {
                newScope.errorMessage = data;
            });
        };
    };
    //#endregion

    $scope.showBuildErrorNode = function (error) {
        if (error && error.istrRuleID && error.istrNodeID) {
            //$NavigateToFileService.NavigateToFile(error.istrRuleID, error.istrNodeName, error.istrNodeID);
            var nodename = "";
            if (error.istrNodeName) {
                nodename = error.istrNodeName;
            }
            var obj = {
                NodeName: nodename, Attribute: "sfwNodeID", Operator: "EqualTo", AttributeValue: error.istrNodeID, isFromBuildErrors: true
            };
            var lst = [];
            lst.push(obj);
            hubMain.server.getFileInfoByID(error.istrRuleID).done(function (result) {
                var fileInfo = result;
                if (fileInfo) {
                    // if rule not already open set factory data and open
                    var IsFileOpen = $rootScope.lstopenedfiles.some(function (file) {
                        if (file.file.FileName == fileInfo.FileName) {
                            return true;
                        }
                    });
                    if (!IsFileOpen) {
                        $searchQuerybuilder.setGlobalQueryBuilderFilters(lst);
                        $searchQuerybuilder.setQueryBuilderIsAdvanceOption(false);
                        $rootScope.openFile(fileInfo);
                    }
                    // if already open - navigate to that node
                    else {
                        $rootScope.openFile(fileInfo);
                        $rootScope.IsLoading = true;
                        hubcontext.hubSearch.server.getFindAdvanceDesign(lst, fileInfo).done(function (DesignNodes) {
                            var filescope = getScopeByFileName(fileInfo.FileName);
                            if (filescope && filescope.selectElement) {
                                filescope.$evalAsync(function () {
                                    $rootScope.IsLoading = false;
                                    // if result is only one node - select it directly
                                    if (DesignNodes.length == 1) {
                                        if (fileInfo.FileType == "DecisionTable") {
                                            filescope.selectElement(DesignNodes[0].ParentPath, DesignNodes[0].FileInfo.SelectNodePath);
                                        } else {
                                            filescope.selectElement(DesignNodes[0].FileInfo.SelectNodePath);
                                        }
                                    }
                                });
                            }
                        });
                    }
                }
            });
        }
    };

    $rootScope.receiveRuleBuildResult = function (data) {
        if (data) {
            $scope.$apply(function () {
                $scope.ruleBuildResult = data;
                $scope.errorCount = null;
                $scope.warningCount = null;
                $scope.messageCount = null;
                if ($scope.ruleBuildResult.ilstErrors.length > 0) {
                    $scope.errorCount = $scope.ruleBuildResult.ilstErrors.length;
                }
                if ($scope.ruleBuildResult.ilstWarnings.length > 0) {
                    $scope.warningCount = $scope.ruleBuildResult.ilstWarnings.length;
                }
                if ($scope.ruleBuildResult.ilstMessages.length > 0) {
                    $scope.messageCount = $scope.ruleBuildResult.ilstMessages.length;
                }
                //Show footer with Build Results visible and selected
                showBuildResults();

                //Select Build Results and show the content
                toggleBuildResults(null, true);

                //Select appropriate sub tab on the Build Results content.
                if ($scope.ruleBuildResult.ilstErrors.length > 0) {
                    selectBuildResultErrors();
                }
                else if ($scope.ruleBuildResult.ilstWarnings.length > 0) {
                    selectBuildResultWarnings();
                }
                else {

                    selectBuildResultMessages();
                }
            });
        }
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };

    $scope.selectedTab = "Errors";
    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };

    $scope.IsPhysicalFileFilter = function (value, index, array) {
        return value.file.FilePath ? true : false;
    };

    $scope.getactualFileCount = function () {
        return $rootScope.lstopenedfiles ? $rootScope.lstopenedfiles.length : 0;
    };


    //#region RuleConstants
    $scope.constantclick = function () {
        $rootScope.openFile($scope.ruleConstantsFileInfo);
    };
    //#endregion

    $scope.openSearchFile = function () {
        $scope.searchFiles = {
            Data: null,
            FileName: "SearchFiles",
            FilePath: "",
            FileType: "SearchFiles",
            TFSState: "None",
            TimeModified: null
        };
        $rootScope.openFile($scope.searchFiles);
    };
    //#endregion

    $scope.CheckForDirtyFiles = function () {
        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
            for (var i = $rootScope.lstopenedfiles.length; i--;) {
                //for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                var scope = getScopeByFileName($rootScope.lstopenedfiles[i].file.FileName);
                if (scope) {
                    if (scope.isDirty) {
                        newScope.lstDirtyFiles.push($rootScope.lstopenedfiles[i]);
                    }
                    else {
                        $rootScope.closeFile($rootScope.lstopenedfiles[i].file.FileName);
                    }
                }
            }
        }
        return newScope.lstDirtyFiles;
    };

    //#endregion
    $window.onbeforeunload = function (e) {
        if ($rootScope.IsLogOut == undefined) {
            var tempFiles = $scope.CheckForDirtyFiles();
            if (tempFiles.length > 0) {
                var files = "";
                for (var i = 0; i < tempFiles.length; i++) {
                    files += "\r\n" + tempFiles[i].file.FilePath;
                }
                var flag = window.confirm("You are closing the window. do you want to Save following Files?" + files + "\r\n\r\n Click 'Ok' to Save or click 'Cancel' to stay back");
                if (flag) {
                    var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                    for (var i = 0; i < tempFiles.length; i++) {
                        scope.onsavefileclick(tempFiles[i]);
                        $rootScope.closeFile(tempFiles[i].file.FileName);
                    }
                    //$.ajax({
                    //    url: "api/Login/LogOutUser",
                    //    type: 'GET',
                    //    async: false
                    //});
                }
                else {
                    return false;
                }
            }
            //else {
            //    $.ajax({
            //        url: "api/Login/LogOutUser",
            //        type: 'GET',
            //        async: false
            //    });
            //}
        }
        else {
            $rootScope.IsLogOut = undefined;
        }
    };
    //#region TFS
    $rootScope.menuOptionsForFileList = [
        ['Delete', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to delete this file ";
            newScope.FileName = $itemScope.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                width: 500, height: 170
            });
            newScope.OkClick = function () {
                var promiseDeleteFile = $rootScope.DeleteFile($itemScope.file);
                promiseDeleteFile.done(function (isDelete) {
                    $rootScope.deleteFileSucessCallback($itemScope.file, isDelete);
                    // handle advance search - result file should be removed if file is deleted
                    if (isDelete && $rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'SearchFiles') {
                        if ($itemScope.$parent.ResultantFiles) {
                            $itemScope.$parent.ResultantFiles.some(function (data, index) {
                                if (data.FileName == $itemScope.file.FileName) {
                                    $itemScope.$parent.ResultantFiles.splice(index, 1);
                                    return true;
                                }
                            });
                        }
                        else if ($itemScope.$parent.ResultantExtraFieldFiles) {
                            $itemScope.$parent.ResultantExtraFieldFiles.some(function (data, index) {
                                if (data.FileName == $itemScope.file.FileName) {
                                    $itemScope.$parent.ResultantExtraFieldFiles.splice(index, 1);
                                    return true;
                                }
                            });
                        }
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if ($itemScope.file.FileType.toLowerCase() == "auditlog" || $itemScope.file.FileType.toLowerCase() == "ruleconstants" || $itemScope.file.FileType.toLowerCase() == "customsettings" || $itemScope.file.FileType.toLowerCase() == "projectconfiguration" || $itemScope.file.FileName == "entTableSchema") {
                return false;
            }
            return true;
        }],
        ['Add File', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to Add ";
            newScope.FileName = $itemScope.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                width: 500, height: 170
            });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.addFileInTFS($itemScope.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveFileAddInTFSStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };

        },
            function ($itemScope) {
                $scope.IsTFS = false;
                var lastProjectData = ConfigurationFactory.getLastProjectDetails();
                if (lastProjectData && lastProjectData.IsTFS) {
                    $scope.IsTFS = true;
                }
                if ($itemScope.file.TFSState == 'OnlyLocal' && $scope.IsTFS) {
                    return true;
                }
                else {
                    return false;
                }
            }],
        ['Check In', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to check in the pending changes in ";
            newScope.FileName = $itemScope.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                width: 500, height: 170
            });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.checkInTFS($itemScope.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveCheckInTfsStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if (($itemScope.file.TFSState == 'InAdd' || $itemScope.file.TFSState == 'InEdit') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Check Out', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to check out the ";
            newScope.FileName = $itemScope.file.FileName;
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                width: 500, height: 170
            });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.checkOut($itemScope.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveCheckOutTfsStatus(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if ($itemScope.file.TFSState == 'InEdit' || $itemScope.file.TFSState == 'InAdd' || $itemScope.file.TFSState == 'OnlyLocal') {
                return false;
            }
            else if ($scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Undo', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.strMessage = "Are you sure you want to undo changes ";
            newScope.FileName = "";
            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html", {
                width: 500, height: 170
            });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                hubMain.server.undoCheckOut($itemScope.file).done(function (objFile) {
                    if (objFile) {
                        $scope.receiveUndoModel(objFile);
                        // handle advance search
                        $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                    }
                });
                dialog.close();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }, function ($itemScope) {
            if (($itemScope.file.TFSState == 'InAdd' || $itemScope.file.TFSState == 'InEdit') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Get Latest Version', function ($itemScope) {
            $rootScope.IsLoading = true;
            hubMain.server.getLatest($itemScope.file).done(function (objFile) {
                if (objFile) {
                    $scope.receiveGetLatest(objFile);
                    // handle advance search
                    $scope.handleFileOperationsAdvanceSearch($itemScope, objFile);
                }
            });
        }, function ($itemScope) {
            if (($itemScope.file.TFSState == 'InAdd' || $itemScope.file.TFSState == 'InEdit' || $itemScope.file.TFSState == 'LocalWithoutChange') && $scope.IsTFS) {
                return true;
            }
            else {
                return false;
            }
        }],
        ['Save As', function ($itemScope) {
            var newScope = $scope.$new();
            hubMain.server.getAllFileList().done(function (data) {
                newScope.$apply(function () {
                    newScope.lstAllfiles = data;
                });
            });

            var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/SaveAs.html", {
                width: 500, height: 200
            });
            newScope.OkClick = function () {
                if ($itemScope.file.FileType.toLowerCase() == "lookup") {
                    SaveAs($itemScope.file, newScope.newFileName + "Lookup");
                }
                else if ($itemScope.file.FileType.toLowerCase() == "maintenance") {
                    SaveAs($itemScope.file, newScope.newFileName + "Maintenance");
                }
                else if ($itemScope.file.FileType.toLowerCase() == "wizard") {
                    SaveAs($itemScope.file, newScope.newFileName + "Wizard");
                }
                else if ($itemScope.file.FileType.toLowerCase() == "tooltip") {
                    SaveAs($itemScope.file, newScope.newFileName + "Tooltip");
                }
                else if ($itemScope.file.FileType.toLowerCase() == "usercontrol") {
                    SaveAs($itemScope.file, newScope.newFileName + "UserControl");
                }
                else {
                    SaveAs($itemScope.file, newScope.newFileName);
                }
                dialog.close();

            };
            newScope.closeDialog = function () {
                dialog.close();
            };
            newScope.ValidateFileName = function () {

                if ($itemScope.file.FileType.toLowerCase() == "lookup" || $itemScope.file.FileType.toLowerCase() == "maintenance" || $itemScope.file.FileType.toLowerCase() == "wizard") {
                    newScope.ValidateFormName($itemScope.file);

                }
                else if ($itemScope.file.FileType.toLowerCase() == "logicalrule" || $itemScope.file.FileType.toLowerCase() == "decisiontable" || $itemScope.file.FileType.toLowerCase() == "excelmatrix") {
                    newScope.validateNewRule();
                }
                else if ($itemScope.file.FileType.toLowerCase() == "bpmn" || $itemScope.file.FileType.toLowerCase() == "bpmtemplate") {
                    newScope.ValidateBPMN();
                }
                else if ($itemScope.file.FileType.toLowerCase() == "usercontrol") {
                    newScope.ValidateUserControl();
                }
                else if ($itemScope.file.FileType.toLowerCase() == "tooltip") {
                    newScope.ValidateTooltip();
                }
            };
            //#region Validation for Save As Pop up

            //#region Validation Form
            newScope.ValidateFormName = function (objFile) {
                newScope.SaveAsError = undefined;
                var retValue = true;
                if (!newScope.newFileName) {
                    newScope.SaveAsError = "Error: Please enter a Form Name (ID).";
                    retValue = false;
                }

                else {
                    if (!isValidFileID(newScope.newFileName, true)) {
                        newScope.SaveAsError = "Error: Numeric Values and Special characters are not allowed in Name.";
                        retValue = false;
                    }
                    else if (newScope.newFileName && newScope.newFileName.toLowerCase().startsWith("wfm")) {
                        newScope.SaveAsError = "Error: ID can not start with wfm.";
                        retValue = false;
                    }
                    else if (newScope.newFileName && newScope.newFileName.trim().length <= 3) {
                        newScope.SaveAsError = "Error: Invalid ID. Minimum 4 characters are required.";
                        retValue = false;
                    }
                    else if (newScope.newFileName && (newScope.newFileName.toLowerCase().endsWith("lookup") || newScope.newFileName.toLowerCase().endsWith("maintenance") || newScope.newFileName.toLowerCase().endsWith("wizard"))) {
                        if (newScope.newFileName.toLowerCase().endsWith("maintenance")) {
                            newScope.SaveAsError = "Error: ID can not ends with maintenance.";
                        } else if (newScope.newFileName.toLowerCase().endsWith("lookup")) {
                            newScope.SaveAsError = "Error: ID can not ends with lookup.";
                        }
                        else {
                            newScope.SaveAsError = "Error: ID can not ends with wizard.";
                        }
                        retValue = false;
                    }
                    else if (objFile.FileType && objFile.FileType.toLowerCase() == "lookup") {

                        var lst = newScope.lstAllfiles.filter(function (x) { if (x.FileName) { return x.FileName.toLowerCase() == "wfm" + newScope.newFileName.toLowerCase() + "lookup"; } });
                        if (lst.length > 0) {
                            newScope.SaveAsError = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                            retValue = false;
                        }

                    }
                    else if (objFile.FileType && objFile.FileType.toLowerCase() == "maintenance") {
                        var lst = newScope.lstAllfiles.filter(function (x) { if (x.FileName) { return x.FileName.toLowerCase() == "wfm" + newScope.newFileName.toLowerCase() + "maintenance"; } });
                        if (lst.length > 0) {
                            newScope.SaveAsError = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                            retValue = false;
                        }

                    }
                    else if (objFile.FileType && objFile.FileType.toLowerCase() == "wizard") {
                        var lst = newScope.lstAllfiles.filter(function (x) {
                            if (x.FileName) {
                                return x.FileName.toLowerCase() == "wfm" + newScope.newFileName.toLowerCase() + "wizard";
                            }
                        });
                        if (lst.length > 0) {
                            newScope.SaveAsError = "Error: The File Name already exists.\nPlease enter a different Wizard Name (ID).";
                            retValue = false;
                        }
                    }

                    return retValue;

                }
            };

            //#endregion

            //#region Validate Rule
            newScope.validateNewRule = function () {
                var retValue = true;
                newScope.SaveAsError = undefined;

                if (!newScope.newFileName) {
                    newScope.SaveAsError = "Error: ID cannot be empty.";
                    retValue = false;
                }
                else if (!isValidIdentifier(newScope.newFileName)) {
                    newScope.SaveAsError = "Error: Invalid ID.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.trim().length <= 3) {
                    newScope.SaveAsError = "Error: Invalid ID. Minimum 4 characters are required.";
                    retValue = false;
                }
                else if (newScope.lstAllfiles && newScope.lstAllfiles.length > 0) {
                    var lst = newScope.lstAllfiles.filter(function (x) {
                        return x.FileName.toLowerCase() == "rul" + newScope.newFileName.toLowerCase();
                    });
                    if (lst && lst.length > 0) {
                        newScope.SaveAsError = "Error: The rule name \"rul" + newScope.newFileName + "\"  already exists. Please enter a different Name (ID).";
                        retValue = false;
                    }
                }

                return retValue;
            };

            //#endregion

            //#region Validate BPMN
            newScope.ValidateBPMN = function () {
                newScope.SaveAsError = undefined;
                var retValue = true;
                if (!(newScope.newFileName && newScope.newFileName.trim().length > 0)) {
                    newScope.SaveAsError = "Error: Please enter a File Name (ID).";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.trim().length <= 3) {
                    newScope.SaveAsError = "Error: Invalid ID. Minimum 4 characters are required.";
                    retValue = false;
                }
                else if (newScope.isExistingBpm()) {
                    newScope.SaveAsError = "Error: A file with same name already exists. Please give a different file name.";
                    retValue = false;
                }
                else if (!isValidFileID(newScope.newFileName)) {
                    newScope.SaveAsError = "Error: File Name must not start with digits, contain space(s), special characters(except '-').";
                    retValue = false;
                }

                return retValue;
            };
            newScope.isExistingBpm = function () {
                return newScope.lstAllfiles.filter(function (x) { return x.FileName.toLowerCase() == "sbp" + newScope.newFileName.toLowerCase(); }).length > 0;
            };
            //#endregion

            //#region Validate User Control
            newScope.ValidateUserControl = function () {
                newScope.SaveAsError = undefined;

                var retValue = true;
                if (!newScope.newFileName) {
                    newScope.SaveAsError = "Error: Please enter a User Control ID.";
                    retValue = false;
                }
                else if (!isValidFileID(newScope.newFileName, true)) {
                    newScope.SaveAsError = "Error: Numeric Values and Special characters are not allowed in Name.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.trim().length <= 3) {
                    newScope.SaveAsError = "Error: Invalid ID. Minimum 4 characters are required.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.toLowerCase().startsWith("udc")) {
                    newScope.SaveAsError = "Error: ID can not start with udc.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.toLowerCase().endsWith("usercontrol")) {
                    newScope.SaveAsError = "Error: ID can not ends with usercontrol.";
                    retValue = false;
                }
                else {
                    var udcId = "udc" + newScope.newFileName + "UserControl";
                    var lst = newScope.lstAllfiles.filter(function (x) { return x.FileName.toLowerCase() == udcId.toLowerCase(); });
                    if (lst.length > 0) {
                        newScope.SaveAsError = "Error: The File Name already exists.\nPlease enter a different User control ID.";
                        retValue = false;
                    }
                }
            };
            //#endregion

            //#region Validate Tooltip

            newScope.ValidateTooltip = function () {
                newScope.SaveAsError = undefined;
                var retValue = true;

                if (!newScope.newFileName) {
                    newScope.SaveAsError = "Error: Please enter a Tooltip form ID.";
                    retValue = false;
                }
                else if (!isValidFileID(newScope.newFileName, true)) {
                    newScope.SaveAsError = "Error: Numeric Values and Special characters are not allowed in Name.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.trim().length <= 3) {
                    newScope.SaveAsError = "Error: Invalid ID. Minimum 4 characters are required.";
                    retValue = false;
                }
                else if (newScope.newFileName && newScope.newFileName.toLowerCase().startsWith("wfm")) {
                    newScope.SaveAsError = "Error: ID can not start with wfm.";
                    retValue = false;
                } else if (newScope.newFileName && newScope.newFileName.toLowerCase().endsWith("tooltip")) {
                    newScope.SaveAsError = "Error: ID can not ends with tooltip.";
                    retValue = false;
                } else {
                    var tooltipId = "wfm" + newScope.newFileName + "Tooltip";
                    var lst = newScope.lstAllfiles.filter(function (x) { return x.FileName.toLowerCase() == tooltipId.toLowerCase(); });
                    if (lst.length > 0) {
                        newScope.SaveAsError = "Error: The File Name " + tooltipId + " already exists.\nPlease enter a different tooltip form ID.";
                        retValue = false;
                    }
                }
            };
            //#endregion

        },
            function ($itemScope) {
                if ($itemScope.file.FileType == "Report" || $itemScope.file.FileType == "Entity" || $itemScope.file.FileType == "Correspondence" || $itemScope.file.FileType == "InboundFile" || $itemScope.file.FileType == "OutboundFile" || $itemScope.file.FileType == "ParameterScenario" || $itemScope.file.FileType == "ObjectScenario" || $itemScope.file.FileType == "ExcelScenario" || $itemScope.file.FileType == "FormLinkMaintenance" || $itemScope.file.FileType == "FormLinkLookup" || $itemScope.file.FileType == "FormLinkWizard" || $itemScope.file.FileType == "WorkflowMap" || $itemScope.file.FileType.toLowerCase() == "auditlog" || $itemScope.file.FileType.toLowerCase() == "ruleconstants" || $itemScope.file.FileType.toLowerCase() == "customsettings" || $itemScope.file.FileType.toLowerCase() == "projectconfiguration") {
                    return false;
                }
                else {
                    return true;
                }
            }],
        ['Convert to wfm', function ($itemScope) {
            var newScope = $scope.$new();
            newScope.objFile = $itemScope.file;
            var scope = getScopeByFileName($itemScope.file.FileName);
            if (scope && scope.isDirty) {
                $SgMessagesService.Message('Alert', "Save the file before converting.");
            }
            else {
                newScope.objFormsDetails = {};
                newScope.objFormsDetails.FormName = newScope.objFile.FileName.replace('wfp', '');
                newScope.objFormsDetails.FormName = newScope.objFormsDetails.FormName.replace(newScope.objFile.FileType, '');
                newScope.convertToWfmErrors = null;
                $.connection.hubCreateNewObject.server.getCommonFieldList().done(function (data) {
                    newScope.$evalAsync(function () {
                        if (data.length == 5) {
                            //newScope.lstFolder = data[1];
                            newScope.lstAllFiles = data[4];
                        }
                    });
                });

                var dialog = $rootScope.showDialog(newScope, "Convert to WFM", "StartUp/views/converttowfm.html", {
                    width: 500, height: 400
                });
                newScope.OkClick = function () {
                    switch (newScope.objFile.FileType.toLowerCase()) {
                        case "lookup":
                            newScope.convertToWfm(newScope.objFile, newScope.objFormsDetails, "Lookup");
                            break;
                        case "maintenance":
                            newScope.convertToWfm(newScope.objFile, newScope.objFormsDetails, "Maintenance");
                            break;
                        case "wizard":
                            newScope.convertToWfm(newScope.objFile, newScope.objFormsDetails, "Wizard");
                            break;
                        default:
                            toastr.error("Required data is not sufficient to convert to wfm");
                    }
                    dialog.close();
                };
                newScope.closeConvertToWfmDialog = function () {
                    dialog.close();
                };

                newScope.convertToWfm = function (objFile, objNewFile) {
                    objNewFile.FormType = objFile.FileType;
                    $rootScope.IsLoading = true;
                    hubcontext.hubMain.server.convertToWFM(objFile, objNewFile).done(function (data) {
                        if (data && data == "Success") {
                            toastr.success("File converted successfully");
                            $scope.$evalAsync(function () {
                                $rootScope.IsFilesAddedDelete = true;
                                $rootScope.IsTfsUpdated = true;
                                $DashboardFactory.getFileTypeData();
                                $DashboardFactory.getTfsStatusData();
                            });
                        }
                        $rootScope.IsLoading = false;
                    });
                }
                //#region Validation Form
                newScope.ValidateConvertToWfm = function () {
                    var retValue = false;
                    if (!newScope.objFormsDetails.FormName) {
                        newScope.convertToWfmErrors = "Error: Please enter a Form Name (ID).";
                        retValue = true;
                    } else if (!isValidFileID(newScope.objFormsDetails.FormName, true)) {
                        newScope.convertToWfmErrors = "Error: Numeric Values and Special characters are not allowed in Name.";
                        retValue = true;
                    }
                    else if (newScope.objFormsDetails.FormName && newScope.objFormsDetails.FormName.toLowerCase().startsWith("wfm")) {
                        newScope.convertToWfmErrors = "Error: ID can not start with wfm.";
                        retValue = true;
                    } else if (newScope.objFormsDetails.FormName && newScope.objFormsDetails.FormName.trim().length <= 3) {
                        newScope.convertToWfmErrors = "Error: Invalid ID. Minimum 4 characters are required.";
                        retValue = true;
                    }
                    else if (newScope.objFormsDetails.FormName && (newScope.objFormsDetails.FormName.toLowerCase().endsWith("lookup") || newScope.objFormsDetails.FormName.toLowerCase().endsWith("maintenance") || newScope.objFormsDetails.FormName.toLowerCase().endsWith("wizard"))) {
                        var postfixName = newScope.objFormsDetails.FormName.toLowerCase().match(/lookup/g) || newScope.objFormsDetails.FormName.toLowerCase().match(/maintenance/g) || newScope.objFormsDetails.FormName.toLowerCase().match(/wizard/g);
                        newScope.convertToWfmErrors = "Error: ID can not ends with " + postfixName[0] + ".";
                        retValue = true;
                    }

                    if (!retValue && newScope.objFile.FileType && newScope.objFile.FileType.toLowerCase() == "lookup") {
                        var fileEndsWithStr = "lookup";
                        retValue = newScope.checkFileExists(fileEndsWithStr);
                    }
                    else if (!retValue && newScope.objFile.FileType && newScope.objFile.FileType.toLowerCase() == "maintenance") {
                        var fileEndsWithStr = "maintenance";
                        retValue = newScope.checkFileExists(fileEndsWithStr);
                    }
                    else if (!retValue && newScope.objFile.FileType && newScope.objFile.FileType.toLowerCase() == "wizard") {
                        var fileEndsWithStr = "wizard";
                        retValue = newScope.checkFileExists(fileEndsWithStr);
                    }

                    if (!retValue && newScope.objFormsDetails.Entity) {
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        if (!entityIntellisenseList.some(function (x) { return x.ID == newScope.objFormsDetails.Entity; })) {
                            newScope.convertToWfmErrors = "Error: Invalid Entity.";
                            retValue = true;
                        }
                    } else if (!(retValue && newScope.objFormsDetails.Entity)) {
                        retValue = true;
                        newScope.convertToWfmErrors = "Error: Enter Entity name.";
                    }

                    if (!retValue && !newScope.objFormsDetails.Path) {
                        retValue = true;
                        newScope.convertToWfmErrors = "Error: Enter file path.";
                    }
                    if (newScope.objFormsDetails.errors && newScope.objFormsDetails.errors["invalid_path"]) {
                        if (newScope.objFormsDetails.errors["invalid_path"] == "Folder Not Exist") {
                            retValue = true;
                            newScope.convertToWfmErrors = "Info: Folder doesn't exist. Please Click Create New Folder Button.";
                        }
                    }
                    if (!retValue) {
                        newScope.convertToWfmErrors = "";
                    }
                    return retValue;
                };
                newScope.checkFileExists = function (endStr) {
                    var isExists = false;
                    var formName = "wfm" + newScope.objFormsDetails.FormName.toLowerCase() + endStr;
                    var lst = newScope.lstAllFiles && newScope.lstAllFiles.filter(function (x) { if (x.FileName) { return x.FileName.toLowerCase() == formName.toLowerCase() || x.FileID.toLowerCase() == formName.toLowerCase(); } });
                    if (lst && lst.length > 0) {
                        newScope.convertToWfmErrors = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                        isExists = true;
                    }
                    return isExists;
                };
            }
            //#endregion
        },
            function ($itemScope) {
                if ($itemScope.file.FileName.startsWith('wfp')) {
                    return true;
                }
                else {
                    return false;
                }
            }],
        ['Convert to wfp', function ($itemScope) {
            var objFile = $itemScope.file;
            var scope = getScopeByFileName($itemScope.file.FileName);
            if (scope && scope.isDirty) {
                $SgMessagesService.Message('Alert', "Save the file before converting.");
            }
            else {
                $SgMessagesService.Message('Convert File', "Do you want to convert file from wfm to wfp?", true, function (action) {
                    if (action) {
                        convertToWFP(objFile);
                    }
                });
            }
        },
            function ($itemScope) {
                if ($itemScope.file.FileName.startsWith('wfm') && $itemScope.file.FileType != "Tooltip") {
                    return true;
                } else {
                    return false;
                }
            }],
    ];

    $rootScope.deleteFileSucessCallback = function (filedetails, isDelete) {
        if (isDelete) {
            $scope.$evalAsync(function () {
                if ($rootScope.lstopenedfiles.length > 0) {
                    for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                        if ($rootScope.lstopenedfiles[i].file.FileName == filedetails.FileName) {
                            $rootScope.closeFile(filedetails.FileName);
                        }
                    }
                }
            });
            $rootScope.IsTfsUpdated = true;
            $rootScope.isFilesListUpdated = true;
            $rootScope.IsFilesAddedDelete = true;
            // if this file was pinned then this will unpin it -- call deleteRecentUserFiles,deletePinUserFiles

            $scope.UserFileAction("Unpin", filedetails);
            $scope.UserFileAction("Unrecent", filedetails);


            //#region If deleted file is an entity then remove it from entity intellisense list
            if (filedetails.FileType == "Entity") {
                var allEntities = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = allEntities.filter(function (x) { return x.ID == filedetails.FileName; });
                if (entities && entities.length > 0) {
                    var index = allEntities.indexOf(entities[0]);
                    if (index > -1) {
                        allEntities.splice(index, 1);
                    }
                }
            }
            //#endregion

            //#region If deleted file is a rule, then remove it from entity intellisense list.
            var entity = null;
            var rule = null;

            var someFilter = function (item) {
                //If rule found, set it for further use.
                return item.ID == filedetails.FileName ? ((rule = item), true) : false;
            };
            var filter = function (item, index) {
                //if entity found, set it for further use.
                return item.Rules.some(someFilter) ? ((entity = item), true) : false;
            };

            //Find entity and rule
            var findEntity = function () {
                $EntityIntellisenseFactory.getEntityIntellisense().some(filter);
            };
            findEntity();

            //check if entity and rule is found, then remove rule from entity rules list.
            if (entity && rule) {
                var index = entity.Rules.indexOf(rule);
                entity.Rules.splice(index, 1);
            }
            //#endregion

            if ($rootScope.isstartpagevisible) {
                $DashboardFactory.getFileTypeData();
                $DashboardFactory.getTfsStatusData();
                $scope.addanddeleteuserfile("Unpin", filedetails);
                $scope.addanddeleteuserfile("Unrecent", filedetails);
                $scope.$broadcast('updateUserFiles');
            }

            toastr.success("File deleted successfully.");
        }
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
    };

    $rootScope.DeleteFile = function (filedetails) {
        $rootScope.IsLoading = true;
        $scope.closeLeftBar();
        return hubMain.server.deleteFile(filedetails);
    };
    $rootScope.updateUserFileDetails = function (lstFileDetails) {
        var pinnedfiles = $DashboardFactory.getPinnedFilesData();
        var recentfiles = $DashboardFactory.getRecentFilesData();
        var pinindex = pinnedfiles.filter(function (item) {
            return item.FileName == lstFileDetails.FileName;
        });
        var recentindex = recentfiles.filter(function (item) {
            return item.FileName == lstFileDetails.FileName;
        });
        if (pinindex.length == 1) {
            var index = pinnedfiles.indexOf(pinindex[0]);
            pinnedfiles.splice(index, 1);
            pinnedfiles.unshift(lstFileDetails);
        }
        if (recentindex.length == 1) {
            var index = recentfiles.indexOf(recentindex[0]);
            recentfiles.splice(index, 1);
            recentfiles.unshift(lstFileDetails);
        }
        $DashboardFactory.setPinnedFilesData(pinnedfiles);
        $DashboardFactory.setRecentFilesData(recentfiles);
        if ($rootScope.isstartpagevisible) $scope.$broadcast('updateUserFiles');
    };
    $scope.receiveFileAddInTFSStatus = function (fileInfo) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            if (fileInfo && $rootScope.currentopenfile && $rootScope.currentopenfile.file && fileInfo.FileName == $rootScope.currentopenfile.file.FileName) {
                $rootScope.currentopenfile.file.TFSState = fileInfo.TFSState;
            }
            if ($rootScope.lstopenedfiles.length > 0) {
                $scope.changeOpenFileTfsStatus(fileInfo);
            }
        });
        $rootScope.isFilesListUpdated = true;
        $rootScope.IsTfsUpdated = true;
        if ($rootScope.isstartpagevisible) $DashboardFactory.getTfsStatusData();
        $rootScope.updateUserFileDetails(fileInfo);
        toastr.success("File added successfully");
    };
    $scope.receiveCheckInTfsStatus = function (fileInfo) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            if (fileInfo && $rootScope.currentopenfile && $rootScope.currentopenfile.file && fileInfo.FileName == $rootScope.currentopenfile.file.FileName) {
                $rootScope.currentopenfile.file.TFSState = fileInfo.TFSState;
            }
            if ($rootScope.lstopenedfiles.length > 0) {
                $scope.changeOpenFileTfsStatus(fileInfo);
            }
        });
        $rootScope.IsTfsUpdated = true;
        $rootScope.isFilesListUpdated = true;
        if ($rootScope.isstartpagevisible) $DashboardFactory.getTfsStatusData();
        $rootScope.updateUserFileDetails(fileInfo);
        toastr.success("File checked in successfully");
    };
    $scope.receiveCheckOutTfsStatus = function (fileInfo) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            if (fileInfo && $rootScope.currentopenfile && $rootScope.currentopenfile.file && fileInfo.FileName == $rootScope.currentopenfile.file.FileName) {
                $rootScope.currentopenfile.file.TFSState = fileInfo.TFSState;
            }
            if ($rootScope.lstopenedfiles.length > 0) {
                $scope.changeOpenFileTfsStatus(fileInfo);
            }
        });
        $rootScope.IsTfsUpdated = true;
        $rootScope.isFilesListUpdated = true;
        if ($rootScope.isstartpagevisible) $DashboardFactory.getTfsStatusData();
        $rootScope.updateUserFileDetails(fileInfo);
        toastr.success("File checked out successfully");
    };
    $scope.receiveUndoModel = function (fileInfo) {
        $scope.$evalAsync(function () {
            if ($rootScope.lstopenedfiles.length > 0) {
                $scope.checkFileIsOpenAndReloadModel(fileInfo);
            }
            $rootScope.IsLoading = false;
        });
        $rootScope.IsTfsUpdated = true;
        $rootScope.isFilesListUpdated = true;
        $rootScope.updateUserFileDetails(fileInfo);
        if ($rootScope.isstartpagevisible) $DashboardFactory.getTfsStatusData();
        toastr.success("Undo successfull");
    };
    $scope.receiveGetLatest = function (fileInfo) {
        $scope.$evalAsync(function () {
            if ($rootScope.lstopenedfiles.length > 0) {
                $scope.checkFileIsOpenAndReloadModel(fileInfo);
            }
            $rootScope.IsLoading = false;
        });
        $rootScope.IsTfsUpdated = true;
        $rootScope.isFilesListUpdated = true;
        toastr.success("No conflict found.");
    };

    $scope.receiveErrorMessage = function (errorMessage) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
        });
        var newScope = $scope.$new();
        newScope.strMessage = errorMessage;
        newScope.isError = true;
        var dialog = $rootScope.showDialog(newScope, "", "StartUp/views/TFSMessageDialog.html");
        newScope.OkClick = function () {
            dialog.close();
        };
        //alert(errorMessage);
    };
    $scope.changeOpenFileTfsStatus = function (fileInfo) {
        for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
            if ($rootScope.lstopenedfiles[i].file.FileName == fileInfo.FileName) {
                $rootScope.lstopenedfiles[i].file.TFSState = fileInfo.TFSState;

            }
        }
    };
    $scope.checkFileIsOpenAndReloadModel = function (fileInfo) {
        for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
            if ($rootScope.lstopenedfiles[i].file.FileName == fileInfo.FileName) {
                var fileDetails = $rootScope.lstopenedfiles[i];
                $rootScope.closeFile(fileInfo.FileName);
                $rootScope.openFile($rootScope.lstopenedfiles[i]);
            }
        }
    };

    var SaveAs = function (oldFileName, newFileName) {

        if (oldFileName && newFileName) {
            $rootScope.IsLoading = true;

            hubcontext.hubMain.server.saveAsFile(oldFileName, newFileName).done(function (data) {
                if (data && data == "Success") {
                    $scope.$evalAsync(function () {
                        $rootScope.IsFilesAddedDelete = true;
                        $rootScope.IsTfsUpdated = true;
                        $DashboardFactory.getFileTypeData();
                        $DashboardFactory.getTfsStatusData();
                    });
                }
            });
            $rootScope.IsLoading = false;
        }

    };

    var convertToWFP = function (aObjFile) {
        var newFileObj = {};
        if (aObjFile) {
            newFileObj.FormName = aObjFile.FileName.replace('wfm', '');
            newFileObj.FormName = newFileObj.FormName.replace(aObjFile.FileType, '');
            newFileObj.Entity = "entPrototype";
            newFileObj.Path = "Prototype" + "\\" + aObjFile.FileType;
            $rootScope.IsLoading = true;
            hubcontext.hubMain.server.convertToWFP(aObjFile, newFileObj).done(function (data) {
                if (data && data == "Success") {
                    toastr.success("File converted successfully");
                    $scope.$evalAsync(function () {
                        $rootScope.IsFilesAddedDelete = true;
                        $rootScope.IsTfsUpdated = true;
                        $DashboardFactory.getFileTypeData();
                        $DashboardFactory.getTfsStatusData();
                    });
                }
                $rootScope.IsLoading = false;
            });
        }
    }
    //#endregion

    //#region Creation Of Excel Matrix
    $scope.objExcelMatrixDetails = {
    };
    $scope.objExcelMatrixDetails.SelectedMatrix = "ImportExcelMatrix";
    $scope.objExcelMatrixDetails.SelectedFile = "ExcelFile";
    $scope.objExcelMatrixDetails.SelectedTable = "DecisionTable";
    $scope.objExcelMatrixDetails.ExcelFilePath = "";
    $scope.objExcelMatrixDetails.ExcelSelectedQuery = "";
    $scope.objExcelMatrixDetails.Precision = "6";
    $scope.objExcelMatrixDetails.Delimiter = ",";
    $scope.objExcelMatrixDetails.ColumnIndex = -1;
    $scope.objExcelMatrixDetails.RowIndex = -1;
    $scope.objExcelMatrixDetails.ExcelNeoTrackID = "";



    $scope.addRules = function (ruleType) {
        $scope.currentEntityExcelMatrix = null;
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var currentEntity = entityIntellisenseList.filter(function (x) {
            return x.ID == $scope.currentFileName;
        });
        if (currentEntity && currentEntity.length > 0) {
            $scope.currentEntityExcelMatrix = currentEntity[0].Rules.filter(function (x) {
                return x.RuleType == "ExcelMatrix" || x.RuleType == "DecisionTable";
            });
            if ($scope.currentEntityExcelMatrix && $scope.currentEntityExcelMatrix.length > 0) {
                for (var i = 0; i < $scope.currentEntityExcelMatrix.length; i++) {
                    if ($scope.currentEntityExcelMatrix[i].ID) {
                        $scope.currentEntityExcelMatrix[i].RuleID = $scope.currentEntityExcelMatrix[i].ID.substring(3);
                    } else {
                        $scope.currentEntityExcelMatrix[i].RuleID = "";
                    }
                }
            }
        }

        if (ruleType == "ExcelMatrix") {
            $scope.ResetAllPropertiesOfExcelMatrixTemplate();
            var newScope = $scope.$new();
            newScope.objExcelMatrixDetails = $scope.objExcelMatrixDetails;
            newScope.lstLoadedExcelData = $scope.lstLoadedExcelData;
            newScope.lstInputColumns = $scope.lstInputColumns;
            newScope.lstOutputColumns = $scope.lstOutputColumns;
            newScope.lstSetValuesData = $scope.lstSetValuesData;
            newScope.lstReturnParameters = $scope.lstReturnParameters;
            newScope.lstimportExcelFileData = $scope.lstimportExcelFileData;
            newScope.ReturnTypes = ['', 'bool', 'DateTime', 'decimal', 'double', 'float', 'int', 'long', 'short', 'string'];
            newScope.dialogOptions = {
                height: 700, width: 1100
            };
            newScope.templateName = "Rule/views/CreateNewExcelMatrix.html";
            if (newScope.templateName && newScope.templateName.trim().length > 0) {
                newScope.dialog = $rootScope.showDialog(newScope, 'New Rule', newScope.templateName, newScope.dialogOptions);
            }

            newScope.isCodeQuery = false;
            var isexcelfile = false;
            var isimportexcelmatrix = false;
            var isdecisiontable = true;
            var isstatic = false;
            newScope.objLoadFileData;
            newScope.OnKeyDowninExcelMatrixFileName = function (event) {
                if ($scope.currentEntityExcelMatrix && $scope.currentEntityExcelMatrix.length > 0) {

                    var selectedRuleType;
                    if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcelMatrix') {
                        selectedRuleType = "ExcelMatrix";
                    }
                    else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcel') {
                        if (newScope.objExcelMatrixDetails.SelectedTable.toLowerCase() == "excelmatrix") {
                            selectedRuleType = "excelmatrix";
                        }
                        else {
                            selectedRuleType = "decisiontable";
                        }
                    }

                    var rules = $scope.currentEntityExcelMatrix.filter(function (x) {
                        return x.RuleType.toLowerCase() == selectedRuleType.toLowerCase();
                    });

                    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                        if (rules && rules.length > 0) {
                            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
                        }
                        event.preventDefault();
                    } else {
                        if (rules && rules.length > 0) {
                            setSingleLevelAutoComplete($(event.currentTarget), rules, "", "RuleID", "ID");
                        }
                        event.stopPropagation();
                    }
                }
            };

            newScope.onExcelMatrixNameChanged = function (data) {
                newScope.validateExcelMatrixName(data);
                var selectedRuleType;
                //Validate duplicate ids.
                if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcel') {
                    if (newScope.objExcelMatrixDetails.SelectedTable.toLowerCase() == "excelmatrix") {
                        selectedRuleType = "excelmatrix";
                    }
                    else {
                        selectedRuleType = "decisiontable";
                    }
                    var items = newScope.lstimportExcelFileData.filter(function (x) {
                        return !angular.equals(data, x);
                    });
                    angular.forEach(items, function (item) {
                        newScope.validateExcelMatrixName(item);
                    });
                }
                else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcelMatrix') {
                    selectedRuleType = "ExcelMatrix";
                    var items = newScope.lstLoadedExcelData.filter(function (x) {
                        return !angular.equals(data, x);
                    });
                    angular.forEach(items, function (item) {
                        newScope.validateExcelMatrixName(item);
                    });
                }

                var rules = [];
                if ($scope.currentEntityExcelMatrix && $scope.currentEntityExcelMatrix.length > 0) {
                    rules = $scope.currentEntityExcelMatrix.filter(function (x) {
                        return data.ExcelMatrixName && x.ID && x.ID.toLowerCase().substring(3) == data.ExcelMatrixName.toLowerCase() && x.RuleType.toLowerCase() == selectedRuleType.toLowerCase();
                    });
                }
                if (rules && rules.length > 0) {
                    //Reset the values
                    data.ReturnType = "";
                    data.InputColumns = [];
                    data.OutputColumns = [];
                    data.ReturnParameter = [];
                    data.IsStatic = false;

                    data.ReturnType = rules[0].ReturnType;
                    data.isReturnTypeDisable = true;
                    if (selectedRuleType == "decisiontable") {
                        data.ReturnType = rules[0].ReturnType;
                        data.IsStatic = rules[0].IsStatic;
                        if (data.InputColumns && data.InputColumns.length == 0) {
                            if (rules[0].Parameters) {
                                for (var i = 0; i < rules[0].Parameters.length; i++) {
                                    var obj = {};
                                    obj.DataType = rules[0].Parameters[i].DataType;
                                    obj.Name = rules[0].Parameters[i].ID;
                                    if (rules[0].Parameters[i].Direction == "In") {
                                        data.InputColumns.push(obj);
                                    } else if (rules[0].Parameters[i].Direction == "Out") {
                                        data.OutputColumns.push(obj);
                                    }
                                }
                            }

                            if (rules[0].ReturnType) {
                                var obj = {};
                                obj.DataType = rules[0].ReturnType;
                                obj.Name = "Return";
                                data.ReturnParameter.push(obj);
                            }
                        }
                    } else {
                        if (rules[0].Parameters && rules[0].Parameters.length > 0) {
                            data.RowParameter.Name = rules[0].Parameters[0].ID;
                            data.RowParameter.DataType = rules[0].Parameters[0].DataType;
                            data.DefaultOutputValue = rules[0].DefaultOutputValue;
                            if (rules[0].Parameters.length > 1) {
                                data.ColumnParameter.Name = rules[0].Parameters[1].ID;
                                data.ColumnParameter.DataType = rules[0].Parameters[1].DataType;
                            }
                        }
                    }
                }
                else {
                    data.isReturnTypeDisable = false;

                    if (selectedRuleType == "decisiontable") {
                        if (data.isExistingRule) {
                            data.ReturnType = "";
                            data.InputColumns = [];
                            data.OutputColumns = [];
                            data.ReturnParameter = [];
                            data.IsStatic = false;
                        }
                        data.isExistingRule = false;
                        //data.ReturnType = "";
                        //data.InputColumns = [];
                        //data.OutputColumns = [];
                        //data.ReturnParameter = [];
                        //data.IsStatic = false;
                    } else {
                        data.EffectiveDate = "";
                        //if (data.RowParameter) {
                        //    data.RowParameter.Name = "";
                        //    data.RowParameter.DataType = "";
                        //}
                        //if (data.ColumnParameter) {
                        //    data.ColumnParameter.Name = "";
                        //    data.ColumnParameter.DataType = "";
                        //}
                    }
                }
            };

            newScope.onExcelMatrixEffectiveDateChanged = function (data) {
                newScope.validateExcelMatrixEffectiveDate(data);
            };

            newScope.LoadDataFromtheExcelFileAndQuery = function () {
                $rootScope.IsLoading = true;
                isexcelfile = false;
                isimportexcelmatrix = false;
                isdecisiontable = true;
                isstatic = false;
                if (newScope.objExcelMatrixDetails.SelectedFile == "ExcelFile") {
                    isexcelfile = true;
                    //$scope.SelectedFile = "ExcelFile";
                }
                if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcelMatrix") {
                    isimportexcelmatrix = true;
                    //$scope.SelectedMatrix = "ImportExcelMatrix";
                }
                else {
                    newScope.objExcelMatrixDetails.SelectedMatrix = "ImportExcel";
                }
                if (newScope.objExcelMatrixDetails.SelectedTable == "DecisionTable") {
                    isdecisiontable = true;
                }
                else {
                    isdecisiontable = false;
                }
                if (isimportexcelmatrix) {
                    isdecisiontable = false;
                }
                newScope.Parameters = [];
                if (newScope.objExcelMatrixDetails.QueryParameters && newScope.objExcelMatrixDetails.QueryParameters.length > 0) {
                    for (var i = 0; i < newScope.objExcelMatrixDetails.QueryParameters.length; i++) {
                        var obj = {
                            ID: newScope.objExcelMatrixDetails.QueryParameters[i].dictAttributes.ID, DataType: newScope.objExcelMatrixDetails.QueryParameters[i].dictAttributes.sfwDataType, Value: newScope.objExcelMatrixDetails.QueryParameters[i].Value
                        };
                        newScope.Parameters.push(obj);
                    }
                }

                newScope.objExcelFileDetails = {
                    ExcelFilePath: newScope.objExcelMatrixDetails.ExcelFilePath, ExcelQuery: newScope.objExcelMatrixDetails.ExcelSelectedQuery, IsExcelFile: isexcelfile, IsImportExcelMatrix: isimportexcelmatrix, IsDecisionTable: isdecisiontable, IsCdoQuery: newScope.objExcelMatrixDetails.isCodeQuery,
                    QueryID: newScope.objExcelMatrixDetails.QueryID, DataType: newScope.objExcelMatrixDetails.QueryDataType, Parameters: newScope.Parameters
                };
                $.connection.hubEntityModel.server.loadDataFromtheExcelFile(newScope.objExcelFileDetails).done(function (data) {
                    newScope.$evalAsync(function () {
                        if (data && data.length > 0 && data[0].Data.length > 0) {
                            if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcelMatrix") {
                                newScope.lstLoadedExcelData = data;
                            } else {
                                newScope.lstimportExcelFileData = data;
                            }
                        }
                        else {
                            newScope.lstLoadedExcelData = [];
                            newScope.lstimportExcelFileData = [];
                        }

                        //Validating the data onload.
                        if (newScope.lstLoadedExcelData && newScope.lstLoadedExcelData.length > 0) {
                            angular.forEach(newScope.lstLoadedExcelData, function (data) {
                                newScope.validateExcelData(data);
                            });
                        }
                        if (newScope.lstimportExcelFileData && newScope.lstimportExcelFileData.length > 0) {
                            angular.forEach(newScope.lstimportExcelFileData, function (data) {
                                newScope.validateExcelData(data);
                            });
                        }
                        $rootScope.IsLoading = false;
                    });
                });
            };

            newScope.onSelectedTableChanged = function (selectedTable) {
                newScope.objExcelMatrixDetails.SelectedTable = selectedTable;
                angular.forEach(newScope.lstimportExcelFileData, function (item) {
                    item.ExcelMatrixName = item.EffectiveDate = "";
                    newScope.validateExcelData(item);
                });
            };

            newScope.selectFile = function (NoOfFile) {

                newScope.lstLoadedExcelData = [];
                hubcontext.hubMain.server.getFilePath(NoOfFile == "M", "NewExcelMatrix", null, "Excel");
            };

            newScope.getRowParameter = function (obj) {
                if (obj.RowParameter.Name != null && obj.RowParameter.Name != "" && obj.RowParameter.DataType != null && obj.RowParameter.DataType != "") {
                    var parameter = obj.RowParameter.Name + "[" + obj.RowParameter.DataType + "]";
                    return parameter;
                }
                else {
                    return "";
                }
            };

            newScope.getColumnParameter = function (obj) {
                if (obj.ColumnParameter.Name != null && obj.ColumnParameter.Name != "" && obj.ColumnParameter.DataType != null && obj.ColumnParameter.DataType != "") {
                    var parameter = obj.ColumnParameter.Name + "[" + obj.ColumnParameter.DataType + "]";
                    return parameter;
                }
                else {
                    return "";
                }
            };

            newScope.validateExcelData = function (data) {
                newScope.validateExcelMatrixName(data);
                newScope.validateExcelMatrixEffectiveDate(data);
            };

            newScope.validateExcelMatrixName = function (data) {
                var selectedRuleType;
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcelMatrix') {
                    selectedRuleType = "ExcelMatrix";
                }
                else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcel') {
                    if (newScope.objExcelMatrixDetails.SelectedTable.toLowerCase() == "excelmatrix") {
                        selectedRuleType = "excelmatrix";
                    }
                    else {
                        selectedRuleType = "decisiontable";
                    }
                }

                data.ExcelMatrixNameError = "";
                if (!(data.ExcelMatrixName && data.ExcelMatrixName.trim().length > 0)) {
                    data.ExcelMatrixNameError = "File Name cannot be empty.";
                }
                else {
                    if ($scope.currentEntityExcelMatrix.some(function (itm) { return itm.ID.toLowerCase() === data.ExcelMatrixName.toLowerCase() })) {

                        data.ExcelMatrixNameError = "Rule name '" + data.ExcelMatrixName + "' already exists.\nPlease enter a different ID (Name).";

                    }
                    else if (!isValidIdentifier(data.ExcelMatrixName)) {
                        data.ExcelMatrixNameError = "Invalid File Name.";
                    }
                    else if (entityIntellisenseList.some(function (y) { return y.Rules.some(function (z) { return (z.ID && z.ID.toLowerCase().substring(3) == data.ExcelMatrixName.toLowerCase()) && (y.ID != $scope.currentFileName || z.RuleType.toLowerCase() != selectedRuleType.toLowerCase()); }); })) {
                        data.ExcelMatrixNameError = "Rule name '" + data.ExcelMatrixName + "' already exists.\nPlease enter a different ID (Name).";
                    }
                    else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcel' && newScope.lstimportExcelFileData && newScope.lstimportExcelFileData.filter(function (x) { return data.ExcelMatrixName && x.ExcelMatrixName && x.ExcelMatrixName.toLowerCase() == data.ExcelMatrixName.toLowerCase(); }).length > 1) {
                        data.ExcelMatrixNameError = "Duplicate File Name.";
                    }
                    else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcelMatrix' && newScope.lstLoadedExcelData && newScope.lstLoadedExcelData.filter(function (x) { return data.ExcelMatrixName && x.ExcelMatrixName && x.ExcelMatrixName.toLowerCase() == data.ExcelMatrixName.toLowerCase(); }).length > 1) {
                        data.ExcelMatrixNameError = "Duplicate File Name.";
                    }
                }
                newScope.validateExcelMatrixEffectiveDate(data);
                if (data.ExcelMatrixNameError == "Invalid File Name." && data.isExistingRule) {
                    data.ExcelMatrixNameError = "";
                }
            };

            newScope.validateExcelMatrixEffectiveDate = function (data) {
                data.isExistingRule = false;
                data.ExcelMatrixEffectiveDateError = "";
                var selectedRuleType;
                if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcelMatrix') {
                    selectedRuleType = "ExcelMatrix";
                }
                else if (newScope.objExcelMatrixDetails.SelectedMatrix == 'ImportExcel') {
                    if (newScope.objExcelMatrixDetails.SelectedTable.toLowerCase() == "excelmatrix") {
                        selectedRuleType = "excelmatrix";
                    }
                    else {
                        selectedRuleType = "decisiontable";
                    }
                }

                var rules = [];
                if ($scope.currentEntityExcelMatrix && $scope.currentEntityExcelMatrix.length > 0) {
                    rules = $scope.currentEntityExcelMatrix.filter(function (x) {
                        return data.ExcelMatrixName && data.ExcelMatrixName.trim().length > 0 && x.ID && x.ID.toLowerCase().substring(3) == data.ExcelMatrixName.toLowerCase() && x.RuleType.toLowerCase() == selectedRuleType.toLowerCase();
                    });
                }
                if (rules && rules.length > 0) {
                    if (!(data.EffectiveDate && data.EffectiveDate.trim().length > 0)) {
                        data.ExcelMatrixEffectiveDateError = "Default rule already exists.\nPlease select an effective date.";
                    }
                    else if (rules[0].EffectiveDates.indexOf(data.EffectiveDate) > -1) {
                        data.ExcelMatrixEffectiveDateError = "A Rule version with effective date already exists.\nPlease select different effective date.";
                    }
                    data.isExistingRule = true;
                }
            };

            newScope.ResetData = function (objdata) {
                objdata.ExcelMatrixName = "";
                objdata.EffectiveDate = "";
                objdata.DataStartCellVal = "";
                objdata.ReturnType = "";
                objdata.DefaultOutputValue = "";
                objdata.ColumnParameter.Name = "";
                objdata.ColumnParameter.DataType = "";
                objdata.ColumnParameter.Description = "";

                objdata.RowParameter.Name = "";
                objdata.RowParameter.DataType = "";
                objdata.RowParameter.Description = "";
                objdata.isReturnTypeDisable = false;
                newScope.validateExcelMatrixName(objdata);

            };

            newScope.ResetDataForSetValues = function (objdata) {
                objdata.ExcelMatrixName = "";
                objdata.EffectiveDate = "";
                objdata.InputColumns = [];
                objdata.OutputColumns = [];
                objdata.ReturnParameter = [];
                objdata.IsStatic = false;
                objdata.IsDescriptionSameAsExpression = false; /*Bug 8314:1. On reset, same as expression checkbox is not getting cleared while creation decision table using excel matrix*/
                if (objdata.RowParameter) {
                    objdata.RowParameter.Name = "";
                    objdata.RowParameter.DataType = "";
                    objdata.RowParameter.Description = "";
                }

                if (objdata.ColumnParameter) {
                    objdata.ColumnParameter.Name = "";
                    objdata.ColumnParameter.DataType = "";
                    objdata.ColumnParameter.Description = "";
                }

                objdata.XAxisCellValue = "";
                objdata.YAxisCellValue = "";
                objdata.Value = "";
                objdata.DistinctFileValue = "";
                objdata.ReturnType = "";

            };

            // create ExcelMatrix
            newScope.createNewExcelMatrix = function () {
                var lstExcelData = [];
                $rootScope.IsLoading = true;
                if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcelMatrix") {
                    var obj;
                    for (var i = 0; i < newScope.lstLoadedExcelData.length; i++) {
                        //if ($scope.lstLoadedExcelData[i].ExcelMatrixName != null && $scope.lstLoadedExcelData[i].ExcelMatrixName != "" && $scope.lstLoadedExcelData[i].ReturnType != null) {
                        if (newScope.isValidImportData(newScope.lstLoadedExcelData[i])) {
                            newScope.lstLoadedExcelData[i].IsValid = true;
                            newScope.objExcelData = newScope.lstLoadedExcelData[i];
                            obj = {
                                DefaultOutputValue: newScope.objExcelData.DefaultOutputValue,
                                EffectiveDate: newScope.objExcelData.EffectiveDate,
                                ReturnType: newScope.objExcelData.ReturnType,
                                DataStartCellVal: newScope.objExcelData.DataStartCellVal,
                                SheetName: newScope.objExcelData.SheetName,
                                ExcelMatrixName: newScope.objExcelData.ExcelMatrixName,
                                IsValid: newScope.objExcelData.IsValid,
                                IsStatic: newScope.objExcelData.IsStatic,
                                RowParameter: {
                                    Name: newScope.objExcelData.RowParameter.Name,
                                    DataType: newScope.objExcelData.RowParameter.DataType,
                                    Description: newScope.objExcelData.RowParameter.Description
                                },
                                ColumnParameter: {
                                    Name: newScope.objExcelData.ColumnParameter.Name,
                                    DataType: newScope.objExcelData.ColumnParameter.DataType,
                                    Description: newScope.objExcelData.ColumnParameter.Description
                                },
                                DataColumnIndex: newScope.objExcelData.ColumnIndex,
                                DataRowIndex: newScope.objExcelData.RowIndex,
                                Precision: newScope.objExcelMatrixDetails.Precision,
                                Delimiter: newScope.objExcelMatrixDetails.Delimiter,
                                blnIsNewExcelFormat: newScope.objExcelData.blnIsNewExcelFormat,
                                ExcelFilePath: newScope.objExcelMatrixDetails.ExcelFilePath,
                                IsExcelFile: isexcelfile,
                                //IsDecisionTable:this.isDecisionTable,
                                LoadExcelFileDetails: newScope.objExcelFileDetails,
                                NeoTrackID: newScope.objExcelMatrixDetails.ExcelNeoTrackID
                            };
                            lstExcelData.push(obj);

                        }
                        else {
                            newScope.lstLoadedExcelData[i].IsValid = false;
                        }
                    }
                }
                else {
                    var obj;
                    for (var i = 0; i < newScope.lstimportExcelFileData.length; i++) {
                        //if ($scope.lstimportExcelFileData[i].ExcelMatrixName != null && $scope.lstimportExcelFileData[i].ExcelMatrixName != "" && (($scope.lstimportExcelFileData[i].InputColumns.length > 0 && $scope.lstimportExcelFileData[i].OutputColumns.length > 0) || $scope.SelectedFile == "ExcelFile")) {
                        if (newScope.isValidImportData(newScope.lstimportExcelFileData[i])) {
                            newScope.lstimportExcelFileData[i].IsValid = true;
                            newScope.objExcelData = newScope.lstimportExcelFileData[i];
                            obj = {
                                DefaultOutputValue: newScope.objExcelData.DefaultOutputValue,
                                EffectiveDate: newScope.objExcelData.EffectiveDate,
                                ReturnType: newScope.objExcelData.ReturnType,
                                DataStartCellVal: newScope.objExcelData.DataStartCellVal,
                                SheetName: newScope.objExcelData.SheetName,
                                ExcelMatrixName: newScope.objExcelData.ExcelMatrixName,
                                IsValid: newScope.objExcelData.IsValid,
                                IsStatic: newScope.objExcelData.IsStatic,
                                RowParameter: {
                                    Name: newScope.objExcelData.RowParameter.Name,
                                    DataType: newScope.objExcelData.RowParameter.DataType,
                                    Description: newScope.objExcelData.RowParameter.Description
                                },
                                ColumnParameter: {
                                    Name: newScope.objExcelData.ColumnParameter.Name,
                                    DataType: newScope.objExcelData.ColumnParameter.DataType,
                                    Description: newScope.objExcelData.ColumnParameter.Description
                                },
                                InputColumns: newScope.objExcelData.InputColumns,
                                OutputColumns: newScope.objExcelData.OutputColumns,
                                ReturnParameter: newScope.objExcelData.ReturnParameter,
                                DataColumnIndex: newScope.ColumnIndex,
                                DataRowIndex: newScope.RowIndex,
                                Precision: newScope.objExcelMatrixDetails.Precision,
                                Delimiter: newScope.objExcelMatrixDetails.Delimiter,
                                blnIsNewExcelFormat: newScope.objExcelData.blnIsNewExcelFormat,
                                ExcelFilePath: newScope.objExcelMatrixDetails.ExcelFilePath,
                                IsExcelFile: isexcelfile,
                                DistinctFileValue: newScope.objExcelData.DistinctFileValue,
                                XAxisCellValue: newScope.objExcelData.XAxisCellValue,
                                YAxisCellValue: newScope.objExcelData.YAxisCellValue,
                                Value: newScope.objExcelData.Value,
                                //IsDecisionTable:this.isDecisionTable,
                                LoadExcelFileDetails: newScope.objExcelFileDetails,
                                NeoTrackID: newScope.objExcelMatrixDetails.ExcelNeoTrackID,
                                DescriptionIsSameAsExpression: newScope.objExcelData.IsDescriptionSameAsExpression ? true : false
                            };
                            lstExcelData.push(obj);

                        }
                        else {
                            newScope.lstimportExcelFileData[i].IsValid = false;
                        }
                    }
                }
                if (lstExcelData.length > 0) {
                    var filedetails;
                    if ($scope.currentFileName != undefined && $scope.currentFilePath != undefined) {
                        filedetails = $rootScope.currentopenfile.file;
                    }

                    $scope.objRule = {
                    };
                    $scope.objRule.RuleType = ruleType;
                    $scope.objRule.IsDefaultVersion = true;

                    if ($scope.currentFileName) {
                        $scope.objRule.EntityName = $scope.currentFileName;
                    }
                    else {

                        var scope = getCurrentControllerScope();
                        $scope.objRule.EntityName = scope.objEntity.dictAttributes.ID;
                    }
                    $scope.objRule.RuleID = "";
                    $scope.objRule.Description = "";
                    $scope.objRule.EffectiveDate = "";
                    $scope.objRule.NeoTrackID = "";
                    $scope.objRule.EntityReturnType = "";
                    $scope.objRule.ReturnType = "";
                    $scope.objRule.IsStatic = false;
                    $scope.objRule.IsMatchAllCondition = false;
                    $scope.objRule.IsThrowError = false;
                    $scope.objRule.IsPrivate = false;
                    $scope.objRule.Trace = false;
                    $scope.objRule.CacheResult = false;

                    $rootScope.IsLoading = true;
                    $.connection.hubEntityModel.server.createRule(filedetails, lstExcelData).done(function (data) {
                        $scope.$apply(function () {
                            var entityid = $scope.objRule.EntityName;
                            var entities = $EntityIntellisenseFactory.getEntityIntellisense();
                            entities = entities.filter(function (x) {
                                return x.ID == entityid;
                            });
                            $.connection.hubEntityModel.server.getEntityRulesIntellisenseData(entityid).done(function (data) {
                                lstRuleIntellisenseData = [];
                                if (data) {
                                    lstRuleIntellisenseData = data;
                                }
                                entities[0].Rules = lstRuleIntellisenseData;
                            });
                            if (data != undefined && data != null && data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    if ($scope.objRule) {
                                        //Adding rule in the intellisense list. 
                                        //Note that before adding this rule to entity rules list, we will have to add it to intellisense coz that entity rule search is dependent on intellisense

                                        if (entities.length > 0) {
                                            //entities[0].Rules.push(data[i][1]);
                                            var entityScope = getScopeByFileName(newScope.objRule.EntityName);

                                            if (entityScope && entityScope.lstItemsRulesList) {
                                                entityScope.lstItemsRulesList.push(data[i][0]);
                                            }
                                            if (entityScope.objEntity && entityScope.objEntity.dictAttributes.ID == newScope.objRule.EntityName) {
                                                var objEntityRuleDetails = {
                                                    DataSourceBasedScenario: null, EntityScenario: null, ExcelBasedScenario: null, ID: data[i][1].ID, IsVisible: false, ObjectBasedScenario: null, RuleFileDetails: obj, Type: obj.FileType
                                                };
                                                entityScope.lstEntityRules.push(objEntityRuleDetails);
                                                entityScope.SearchRuleCommand();
                                            }
                                        }
                                    }
                                }
                            }
                            $rootScope.IsLoading = false;
                            $SgMessagesService.Message('Message', "Complete");
                        });
                    });
                }
                else {
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                }
                newScope.closeNewRuleWindow();
            };

            newScope.isValidImportData = function (obj) {
                if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcelMatrix") {
                    var selectedType = "excelmatrix";
                }
                else if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcel") {
                    if (newScope.objExcelMatrixDetails.SelectedTable.toLowerCase() == "decisiontable") {
                        var selectedType = "decisiontable";
                    }
                    else {
                        var selectedType = "excelmatrix";
                    }
                }

                if (selectedType) {
                    if (obj.ExcelMatrixNameError && obj.ExcelMatrixNameError.trim().length > 0) {
                        return false;
                    }
                    if (obj.ExcelMatrixEffectiveDateError && obj.ExcelMatrixEffectiveDateError.trim().length > 0) {
                        return false;
                    }
                    if (selectedType == "excelmatrix") {
                        if (!(obj.ReturnType && obj.ReturnType.trim().length > 0)) {
                            return false;
                        }
                        if (!(obj.RowParameter.Name && obj.RowParameter.Name.trim().length > 0)) {
                            return false;
                        }
                        if (!(obj.RowParameter.DataType && obj.RowParameter.DataType.trim().length > 0)) {
                            return false;
                        }
                        if (!(obj.ColumnParameter.Name && obj.ColumnParameter.Name.trim().length > 0)) {
                            return false;
                        }
                        if (!(obj.ColumnParameter.DataType && obj.ColumnParameter.DataType.trim().length > 0)) {
                            return false;
                        }
                        if (newScope.objExcelMatrixDetails.SelectedMatrix == "ImportExcelMatrix") {
                            if (!(obj.DataStartCellVal !== undefined && obj.DataStartCellVal.toString().trim().length > 0)) {
                                return false;
                            }
                            if (obj.ColumnIndex == undefined) {
                                return false;
                            }
                            if (obj.RowIndex == undefined) {
                                return false;
                            }
                        }
                        else {
                            if (!(obj.XAxisCellValue && obj.XAxisCellValue.trim().length > 0)) {
                                return false;
                            }
                            if (!(obj.YAxisCellValue && obj.YAxisCellValue.trim().length > 0)) {
                                return false;
                            }
                            if (!(obj.Value && obj.Value.trim().length > 0)) {
                                return false;
                            }
                        }
                    }
                    else {
                        if (obj.InputColumns && obj.InputColumns.length > 0) {
                            for (var i = 0; i < obj.InputColumns.length; i++) {
                                if (!(obj.InputColumns[i].Name && obj.InputColumns[i].Name.trim().length > 0)) {
                                    return false;
                                }
                                if (!(obj.InputColumns[i].DataType && obj.InputColumns[i].DataType.trim().length > 0)) {
                                    return false;
                                }
                            }
                        }
                        else {
                            return false;
                        }
                        if (obj.OutputColumns && obj.OutputColumns.length > 0) {
                            for (var i = 0; i < obj.OutputColumns.length; i++) {
                                if (!(obj.OutputColumns[i].Name && obj.OutputColumns[i].Name.trim().length > 0)) {
                                    return false;
                                }
                                if (!(obj.OutputColumns[i].DataType && obj.OutputColumns[i].DataType.trim().length > 0)) {
                                    return false;
                                }
                            }
                        }
                        else {
                            if (!(obj.ReturnParameter && obj.ReturnParameter.length > 0)) {
                                return false;
                            }
                        }
                    }
                }
                else {
                    return false;
                }
                return true;
            };

            // Add or Edit Parameter 
            newScope.OnAddOrEditParameter = function (parameter, obj) {
                var newParameterScope = $scope.$new();
                newParameterScope.ReturnTypes = newScope.ReturnTypes;
                newParameterScope.SelectedDataObj = obj;
                newParameterScope.SelectedParameter = parameter;
                if (parameter == "RowParameter") {
                    newParameterScope.ParameterName = obj.RowParameter.Name;
                    newParameterScope.ParameterDataType = obj.RowParameter.DataType;
                    newParameterScope.ParameterDescription = obj.RowParameter.Description;

                }
                else if (parameter == "ColumnParameter") {
                    newParameterScope.ParameterName = obj.ColumnParameter.Name;
                    newParameterScope.ParameterDataType = obj.ColumnParameter.DataType;
                    newParameterScope.ParameterDescription = obj.ColumnParameter.Description;
                }

                newParameterScope.validateImportExcelMatrixParameter = function () {
                    newParameterScope.ImportExcelMatrixParameterError = "";
                    if (!(newParameterScope.ParameterName && newParameterScope.ParameterName.trim().length > 0)) {
                        newParameterScope.ImportExcelMatrixParameterError = "Parameter Name cannot be empty.";
                    } else if (!isValidIdentifier(newParameterScope.ParameterName)) {
                        newParameterScope.ImportExcelMatrixParameterError = "Invalid Parameter Name.";
                    } else if (!(newParameterScope.ParameterDataType && newParameterScope.ParameterDataType.trim().length > 0)) {
                        newParameterScope.ImportExcelMatrixParameterError = "Parameter Datatype cannot be empty.";
                    }
                };
                newParameterScope.validateImportExcelMatrixParameter();
                newParameterScope.dialogOptions = {
                    height: 250, width: 500
                };
                newParameterScope.templateName = "Rule/views/AddOrEditParameterTemplate.html";
                if (newParameterScope.templateName && newParameterScope.templateName.trim().length > 0) {
                    newParameterScope.dialog = $rootScope.showDialog(newParameterScope, 'Add/Edit Parameter', newParameterScope.templateName, newParameterScope.dialogOptions);
                }

                newParameterScope.onOkAddOrEditClick = function () {
                    if (newParameterScope.SelectedParameter == "RowParameter") {
                        newParameterScope.SelectedDataObj.RowParameter.Name = newParameterScope.ParameterName;
                        newParameterScope.SelectedDataObj.RowParameter.DataType = newParameterScope.ParameterDataType;
                        newParameterScope.SelectedDataObj.RowParameter.Description = newParameterScope.ParameterDescription;

                    }
                    else if (newParameterScope.SelectedParameter == "ColumnParameter") {
                        newParameterScope.SelectedDataObj.ColumnParameter.Name = newParameterScope.ParameterName;
                        newParameterScope.SelectedDataObj.ColumnParameter.DataType = newParameterScope.ParameterDataType;
                        newParameterScope.SelectedDataObj.ColumnParameter.Description = newParameterScope.ParameterDescription;
                    }
                    newParameterScope.onCloseAddOrEditClick();
                };
                newParameterScope.onCloseAddOrEditClick = function () {
                    if (newParameterScope.dialog) {
                        newParameterScope.dialog.close();
                    }
                    newParameterScope.SelectedDataObj = null;
                };

            };

            // Data Starting cell dailog  related Data

            newScope.onDataStartingCellClick = function (obj) {
                var newExcelScope = $scope.$new();
                newExcelScope.SelectedDataStartingCell;
                newExcelScope.LoadSomeData = function () {
                    var count = 0;
                    for (var i = newExcelScope.lstData.length; i < newExcelScope.lstobjExceldata.length; i++) {
                        count++;
                        if (count == 10) {
                            break;
                        }
                        else {
                            newExcelScope.lstData.push(newExcelScope.lstobjExceldata[i]);
                        }
                    }
                };
                newExcelScope.lstData = [];
                newExcelScope.SelectedDataObj = obj;
                newExcelScope.lstobjExceldata = obj.Data;
                newExcelScope.LoadSomeData();
                //$scope.lstData = obj.Data;
                newExcelScope.dialogOptions = {
                    height: 500, width: 600
                };
                newExcelScope.templateName = "Rule/views/ExcelDataTemplate.html";
                if (newExcelScope.templateName && newExcelScope.templateName.trim().length > 0) {
                    newExcelScope.dialog = $rootScope.showDialog(newExcelScope, '', newExcelScope.templateName, newExcelScope.dialogOptions);
                }
                newExcelScope.onOkClickExcelDailog = function () {
                    newExcelScope.SelectedDataObj.DataStartCellVal = newExcelScope.SelectedDataStartingCell;
                    newExcelScope.SelectedDataObj.RowIndex = newExcelScope.RowIndex;
                    newExcelScope.SelectedDataObj.ColumnIndex = newExcelScope.ColumnIndex;
                    newExcelScope.closeExcelDailog();
                };

                newExcelScope.closeExcelDailog = function () {
                    if (newExcelScope.dialog) {
                        newExcelScope.dialog.close();
                    }
                    newExcelScope.SelectedDataObj = null;
                };

                newExcelScope.setSelectedCellData = function (celldata) {
                    newExcelScope.SelectedDataStartingCell = celldata;
                };

                newExcelScope.setRowIndex = function (rowindex) {
                    newExcelScope.RowIndex = rowindex;
                };
                newExcelScope.setColumnIndex = function (columnindex) {
                    newExcelScope.ColumnIndex = columnindex;
                };
            };

            //Close Dialog
            newScope.closeNewRuleWindow = function () {
                if (newScope.dialog) {
                    newScope.dialog.close();
                }
            };
            newScope.isSetDetailsLoaded = false;
            // import excel set values
            newScope.onSetValueClick = function (obj) {
                var newSetValuesScope = $scope.$new();
                var newSetDetailsScope;
                if (newScope.isSetDetailsLoaded) {
                    newSetDetailsScope = newScope.setDetailsScope;
                }
                newSetValuesScope.scopeId = newSetValuesScope.$id;
                newSetValuesScope.objExcelMatrixDetails = $scope.objExcelMatrixDetails;
                newSetValuesScope.lstSetValuesData = [];
                newSetValuesScope.ObjSelectedlstValuesData = obj;
                /* changes made by Sai Kumar coz of this issue(On Cancel, changes done on return type dropdown and same as expression checkbox should get undo)*/
                newSetValuesScope.setValues = {};
                newSetValuesScope.setValues.ReturnType = newSetValuesScope.ObjSelectedlstValuesData.ReturnType;
                newSetValuesScope.setValues.IsStatic = newSetValuesScope.ObjSelectedlstValuesData.IsStatic;
                newSetValuesScope.setValues.isExistingRule = newSetValuesScope.ObjSelectedlstValuesData.isExistingRule;
                newSetValuesScope.setValues.IsDescriptionSameAsExpression = newSetValuesScope.ObjSelectedlstValuesData.IsDescriptionSameAsExpression;
                newSetValuesScope.lstColumns = [];
                newSetValuesScope.lstDummyColumns = [];

                newSetValuesScope.lstKeys = [];
                if (newSetValuesScope.ObjSelectedlstValuesData.Data.length > 0) {
                    angular.forEach(newSetValuesScope.ObjSelectedlstValuesData.Data[0], function (value, key) {
                        if (key != "$$hashKey") {
                            if (key != null) {
                                newSetValuesScope.lstKeys.push(key);
                            }
                            else {
                                newSetValuesScope.lstKeys.push("");
                            }
                        }
                    });
                }

                if (newSetDetailsScope) {
                    newSetDetailsScope.ObjSelectedlstValuesData = newSetValuesScope.ObjSelectedlstValuesData;
                    newSetDetailsScope.ResetSetDetailsData();
                }

                newSetValuesScope.setColumnNamesinLst = function (lstcol) {
                    newSetValuesScope.lstColumns = [];
                    newSetValuesScope.lstDummyColumns = [];
                    angular.forEach(lstcol, function (value, key) {
                        if (key != "$$hashKey") {
                            if (value != null) {
                                pushColumn(value);
                            }
                            else {
                                pushColumn("");
                            }
                        }
                    });

                    function pushColumn(value) {
                        value = value.toString();
                        newSetValuesScope.lstColumns.push(value.trim());
                        newSetValuesScope.lstDummyColumns.push(value.trim());
                    }
                };
                newSetValuesScope.lstInputColumns = [];
                newSetValuesScope.lstOutputColumns = [];
                newSetValuesScope.lstReturnParameters = [];

                newSetValuesScope.InputCanMoveErrorUp = function () {
                    newSetValuesScope.Flag = true;
                    if (newSetValuesScope.objselectedInputValue != undefined) {
                        for (var i = 0; i < newSetValuesScope.lstInputColumns.length; i++) {
                            if (newSetValuesScope.lstInputColumns[i] == newSetValuesScope.objselectedInputValue) {
                                if (i > 0) {
                                    newSetValuesScope.Flag = false;
                                }
                            }
                        }

                    }
                    return newSetValuesScope.Flag;
                };

                newSetValuesScope.InputCanMoveErrorDown = function () {
                    newSetValuesScope.Flag = true;
                    if (newSetValuesScope.objselectedInputValue != undefined) {
                        for (var i = 0; i < newSetValuesScope.lstInputColumns.length; i++) {
                            if (newSetValuesScope.lstInputColumns[i] == newSetValuesScope.objselectedInputValue) {
                                if (i < newSetValuesScope.lstInputColumns.length - 1) {
                                    newSetValuesScope.Flag = false;
                                }
                            }
                        }
                    }
                    return newSetValuesScope.Flag;
                };

                newSetValuesScope.InputmoveErrorUp = function () {
                    var index = newSetValuesScope.lstInputColumns.indexOf(newSetValuesScope.objselectedInputValue);
                    var item = newSetValuesScope.lstInputColumns[index - 1];
                    newSetValuesScope.lstInputColumns[index - 1] = newSetValuesScope.objselectedInputValue;
                    newSetValuesScope.lstInputColumns[index] = item;
                };

                newSetValuesScope.InputmoveErrorDown = function () {
                    var index = newSetValuesScope.lstInputColumns.indexOf(newSetValuesScope.objselectedInputValue);
                    var item = newSetValuesScope.lstInputColumns[index + 1];
                    newSetValuesScope.lstInputColumns[index + 1] = newSetValuesScope.objselectedInputValue;
                    newSetValuesScope.lstInputColumns[index] = item;
                };

                newSetValuesScope.DeleteInputParameter = function () {
                    var index = newSetValuesScope.lstInputColumns.indexOf(newSetValuesScope.objselectedInputValue);
                    newSetValuesScope.lstInputColumns.splice(index, 1);
                    if (index != 0) {
                        newSetValuesScope.objselectedInputValue = newSetValuesScope.lstInputColumns[index - 1];
                    }
                    else if (newSetValuesScope.lstInputColumns.length > 0) {
                        newSetValuesScope.objselectedInputValue = newSetValuesScope.lstInputColumns[index];
                    }
                    else {
                        newSetValuesScope.objselectedInputValue = undefined;
                    }
                };

                newSetValuesScope.DeleteOutputParameter = function () {
                    var index = newSetValuesScope.lstOutputColumns.indexOf(newSetValuesScope.objselectedOutputValue);
                    newSetValuesScope.lstOutputColumns.splice(index, 1);
                    if (index != 0) {
                        newSetValuesScope.objselectedOutputValue = newSetValuesScope.lstOutputColumns[index - 1];
                    }
                    else if (newSetValuesScope.lstOutputColumns.length > 0) {
                        newSetValuesScope.objselectedOutputValue = newSetValuesScope.lstOutputColumns[index];
                    }
                    else {
                        newSetValuesScope.objselectedOutputValue = undefined;
                    }
                };
                //For Return Parameters
                newSetValuesScope.DeleteReturnParameter = function () {
                    if (newSetValuesScope.lstReturnParameters.length > 0) {
                        newSetValuesScope.lstReturnParameters.splice(newSetValuesScope.lstReturnParameters.length - 1, 1);
                    }
                };

                newSetValuesScope.InputDeleteDisable = function () {
                    if (newSetValuesScope.objselectedInputValue != undefined) {
                        return false;
                    }
                    else {
                        return true;
                    }
                };

                newSetValuesScope.OutputDeleteDisable = function () {
                    if (newSetValuesScope.objselectedOutputValue != undefined) {
                        return false;
                    }
                    else {
                        return true;
                    }
                };

                newSetValuesScope.validateImportExcelMatrixParameter = function () {
                    newSetValuesScope.ImportExcelMatrixParameterError = "";
                    if (newSetValuesScope.SelectedDataObj) {
                        var obj = newSetValuesScope.SelectedDataObj;
                    }
                    else if (newSetValuesScope.ObjSelectedlstValuesData) {
                        var obj = newSetValuesScope.ObjSelectedlstValuesData;
                    }

                    if (obj) {
                        if (!(newSetValuesScope.ParameterName && newSetValuesScope.ParameterName.trim().length > 0)) {
                            newSetValuesScope.ImportExcelMatrixParameterError = "Parameter Name cannot be empty.";
                        }
                        else if (!isValidIdentifier(newSetValuesScope.ParameterName)) {
                            newSetValuesScope.ImportExcelMatrixParameterError = "Invalid Parameter Name.";
                        }
                        else if (newSetValuesScope.existingParameter(newSetValuesScope.ParameterName, obj)) {
                            newSetValuesScope.ImportExcelMatrixParameterError = "A Parameter with same name already existing in the selected rule.";
                        }
                        else if (!(newSetValuesScope.ParameterDataType && newSetValuesScope.ParameterDataType.trim().length > 0)) {
                            newSetValuesScope.ImportExcelMatrixParameterError = "Parameter Datatype cannot be empty.";
                        }
                    }
                };

                newSetValuesScope.existingParameter = function (parameterName, obj) {
                    if (obj.ExcelMatrixName && obj.ExcelMatrixName.trim().length > 0) {
                        var rules = $scope.currentEntityExcelMatrix.filter(function (x) {
                            return x.ID == obj.ExcelMatrixName;
                        });
                        if (rules && rules.length > 0) {
                            if (rules[0].Parameters.some(function (x) {
                                return x.ID == parameterName;
                            })) {
                                return true;
                            }
                        }
                    }
                    return false;
                };

                newSetValuesScope.SelectInputParameter = function (obj) {
                    newSetValuesScope.objselectedInputValue = obj;
                };

                newSetValuesScope.OutputmoveErrorUp = function () {
                    var index = newSetValuesScope.lstOutputColumns.indexOf(newSetValuesScope.objselectedOutputValue);
                    var item = newSetValuesScope.lstOutputColumns[index - 1];
                    newSetValuesScope.lstOutputColumns[index - 1] = newSetValuesScope.objselectedOutputValue;
                    newSetValuesScope.lstOutputColumns[index] = item;
                };

                newSetValuesScope.SelectOutputParameter = function (obj) {
                    newSetValuesScope.objselectedOutputValue = obj;
                };

                newSetValuesScope.OutputCanMoveErrorUp = function () {
                    newSetValuesScope.Flag = true;
                    if (newSetValuesScope.objselectedOutputValue != undefined) {
                        for (var i = 0; i < newSetValuesScope.lstOutputColumns.length; i++) {
                            if (newSetValuesScope.lstOutputColumns[i] == newSetValuesScope.objselectedOutputValue) {
                                if (i > 0) {
                                    newSetValuesScope.Flag = false;
                                }
                            }
                        }

                    }

                    return newSetValuesScope.Flag;
                };

                newSetValuesScope.OutputmoveErrorDown = function () {
                    var index = newSetValuesScope.lstOutputColumns.indexOf(newSetValuesScope.objselectedOutputValue);
                    var item = newSetValuesScope.lstOutputColumns[index + 1];
                    newSetValuesScope.lstOutputColumns[index + 1] = newSetValuesScope.objselectedOutputValue;
                    newSetValuesScope.lstOutputColumns[index] = item;
                };

                newSetValuesScope.OutputCanMoveErrorDown = function () {
                    newSetValuesScope.Flag = true;
                    if (newSetValuesScope.objselectedOutputValue != undefined) {
                        for (var i = 0; i < newSetValuesScope.lstOutputColumns.length; i++) {
                            if (newSetValuesScope.lstOutputColumns[i] == newSetValuesScope.objselectedOutputValue) {
                                if (i < newSetValuesScope.lstOutputColumns.length - 1) {
                                    newSetValuesScope.Flag = false;
                                }
                            }
                        }

                    }
                    return newSetValuesScope.Flag;
                };

                //add or edit parameter for excel
                newSetValuesScope.OnAddOrEditParameterForImportExcel = function (obj, param) {
                    var newParameterScope = $scope.$new();
                    newParameterScope.isExistingRule = newSetValuesScope.ObjSelectedlstValuesData.isExistingRule;
                    newParameterScope.ReturnTypes = newScope.ReturnTypes;
                    newParameterScope.objselectedSetValue = obj;
                    newParameterScope.ParameterName = obj.Name;
                    newParameterScope.ParameterDataType = obj.DataType;
                    newParameterScope.ParameterOperator = "";
                    if (obj.Operator) {
                        newParameterScope.ParameterOperator = obj.Operator;
                    }
                    newParameterScope.ParameterDescription = obj.Description;
                    newSetValuesScope.validateImportExcelMatrixParameter();
                    if (param == "Return") {
                        newParameterScope.isReturnParameter = true;
                    }

                    newParameterScope.dialogOptions = {
                        height: 250, width: 500
                    };
                    newParameterScope.validateImportExcelMatrixParameter = function () {
                        newParameterScope.ImportExcelMatrixParameterError = "";
                        if (!(newParameterScope.ParameterName && newParameterScope.ParameterName.trim().length > 0)) {
                            newParameterScope.ImportExcelMatrixParameterError = "Parameter Name cannot be empty.";
                        } else if (!isValidIdentifier(newParameterScope.ParameterName)) {
                            newParameterScope.ImportExcelMatrixParameterError = "Invalid Parameter Name.";
                        } else if ((param && param != "Return") && !(newParameterScope.ParameterDataType && newParameterScope.ParameterDataType.trim().length > 0)) {
                            newParameterScope.ImportExcelMatrixParameterError = "Parameter Datatype cannot be empty.";
                        }
                    };

                    newParameterScope.validateImportExcelMatrixParameter();
                    newParameterScope.Operators = ["", ">", "<", "==", ">=", "<=", "!="];
                    newParameterScope.ParameterType = param;
                    newParameterScope.templateName = "Rule/views/AddOrEditParameterTemplate.html";
                    if (newParameterScope.templateName && newParameterScope.templateName.trim().length > 0) {
                        newParameterScope.dialog = $rootScope.showDialog(newParameterScope, 'Add/Edit Parameter', newParameterScope.templateName, newParameterScope.dialogOptions);
                    }

                    newParameterScope.onOkAddOrEditClick = function () {
                        newParameterScope.objselectedSetValue.Name = newParameterScope.ParameterName;
                        newParameterScope.objselectedSetValue.DataType = newParameterScope.ParameterDataType;
                        newParameterScope.objselectedSetValue.Description = newParameterScope.ParameterDescription;
                        newParameterScope.objselectedSetValue.Operator = newParameterScope.ParameterOperator;
                        newParameterScope.onCloseAddOrEditClick();
                    };

                    newParameterScope.onCloseAddOrEditClick = function () {
                        if (newParameterScope.dialog) {
                            newParameterScope.dialog.close();
                        }
                        newParameterScope.objselectedSetValue = null;
                    };

                };

                newSetValuesScope.setInputValues = function (index) {
                    var value = newSetValuesScope.lstColumns[index - 1];
                    if (value && value.trim().length > 0) {
                        var isfound = false;
                        for (var i = 0; i < newSetValuesScope.lstInputColumns.length; i++) {
                            if (value == newSetValuesScope.lstInputColumns[i].ColumnName) {
                                var isfound = true;
                                break;
                            }
                        }
                        for (var j = 0; j < newSetValuesScope.lstOutputColumns.length; j++) {
                            if (value == newSetValuesScope.lstOutputColumns[j].ColumnName) {
                                newSetValuesScope.lstOutputColumns.splice(j, 1);
                            }
                        }
                        if (!isfound) {
                            var param = value;
                            param = param.replace(new RegExp(" ", 'gi'), "");
                            var obj = {
                                ColumnName: value, Name: param, DataType: "", Description: value
                            };
                            newSetValuesScope.lstInputColumns.push(obj);
                            newSetValuesScope.objselectedInputValue = obj;
                            newSetValuesScope.OnAddOrEditParameterForImportExcel(newSetValuesScope.lstInputColumns[newSetValuesScope.lstInputColumns.length - 1], "Input");
                        }
                    }
                };

                newSetValuesScope.setValuesForExistingParameter = function (objData, index, param) {
                    var value = newSetValuesScope.lstColumns[index - 1];
                    var key = newSetValuesScope.lstKeys[index - 1];
                    if (value) {
                        objData.ColumnName = value;
                        objData.Description = value;
                    } else {
                        objData.ColumnName = "";
                    }

                    if (key) {
                        objData.key = "[" + key + "]";
                    } else {
                        objData.key = "";
                    }
                    newSetValuesScope.OnAddOrEditParameterForImportExcel(objData, param);
                    newSetValuesScope.ValidateExistingParameterValuesForDecVersion();
                };

                newSetValuesScope.setOutputValues = function (index) {
                    var value = newSetValuesScope.lstColumns[index - 1];
                    if (value && value.trim().length > 0) {
                        var isfound = false;
                        for (var i = 0; i < newSetValuesScope.lstOutputColumns.length; i++) {
                            if (value == newSetValuesScope.lstOutputColumns[i].ColumnName) {
                                var isfound = true;
                                break;
                            }
                        }
                        for (var j = 0; j < newSetValuesScope.lstInputColumns.length; j++) {
                            if (value == newSetValuesScope.lstInputColumns[j].ColumnName) {
                                newSetValuesScope.lstInputColumns.splice(j, 1);
                            }
                        }
                        if (!isfound) {
                            var param = value;
                            param = param.replace(new RegExp(" ", 'gi'), "");
                            var obj = {
                                ColumnName: value, Name: param, DataType: "", Description: value
                            };
                            newSetValuesScope.lstOutputColumns.push(obj);
                            newSetValuesScope.objselectedOutputValue = obj;
                            newSetValuesScope.OnAddOrEditParameterForImportExcel(newSetValuesScope.lstOutputColumns[newSetValuesScope.lstOutputColumns.length - 1], "Output");
                        }
                    }
                };

                //For Return Parameters
                newSetValuesScope.setReturnParameter = function (index) {
                    if (newSetValuesScope.setValues.ReturnType) {
                        var value = newSetValuesScope.lstColumns[index - 1];
                        if (value && value.trim().length > 0) {
                            if (newSetValuesScope.lstReturnParameters.length == 0) {
                                var param = value;
                                param = param.replace(new RegExp(" ", 'gi'), "");
                                var obj = {
                                    ColumnName: value, Name: param, DataType: "", Description: value
                                };
                                newSetValuesScope.lstReturnParameters.push(obj);
                                newSetValuesScope.objselectedreturnparameter = obj;
                                newSetValuesScope.OnAddOrEditParameterForImportExcel(newSetValuesScope.lstReturnParameters[newSetValuesScope.lstReturnParameters.length - 1], "Return");
                            }
                        }
                    } else {
                        $SgMessagesService.Message('Message', "please select return type to add return parameter");
                    }
                };

                newSetValuesScope.SelectReturnParameter = function (obj) {
                    newSetValuesScope.objselectedreturnparameter = obj;
                };

                newSetValuesScope.onSetValuesOkClick = function () {
                    newSetValuesScope.ObjSelectedlstValuesData.OutputColumns = [];
                    newSetValuesScope.ObjSelectedlstValuesData.InputColumns = [];
                    newSetValuesScope.ObjSelectedlstValuesData.ReturnParameter = [];
                    angular.forEach(newSetValuesScope.lstOutputColumns, function (value, key) {
                        newSetValuesScope.ObjSelectedlstValuesData.OutputColumns.push(value);
                    });

                    angular.forEach(newSetValuesScope.lstInputColumns, function (value, key) {
                        newSetValuesScope.ObjSelectedlstValuesData.InputColumns.push(value);
                    });
                    //For Return Parameters
                    if (newSetValuesScope.lstReturnParameters && newSetValuesScope.lstReturnParameters.length > 0) {
                        newSetValuesScope.ObjSelectedlstValuesData.ReturnParameter.push(newSetValuesScope.lstReturnParameters[0]);
                    }
                    newSetValuesScope.ObjSelectedlstValuesData.DistinctFileValue = newSetValuesScope.DistinctFileValue;
                    newSetValuesScope.ObjSelectedlstValuesData.XAxisCellValue = newSetValuesScope.XAxisCellValue;
                    newSetValuesScope.ObjSelectedlstValuesData.YAxisCellValue = newSetValuesScope.YAxisCellValue;
                    newSetValuesScope.ObjSelectedlstValuesData.Value = newSetValuesScope.Value;
                    /* changes made by Sai Kumar coz of this issue(On Cancel, changes done on return type dropdown and same as expression checkbox should get undo)*/
                    newSetValuesScope.ObjSelectedlstValuesData.ReturnType = newSetValuesScope.setValues.ReturnType;
                    newSetValuesScope.ObjSelectedlstValuesData.isExistingRule = newSetValuesScope.setValues.isExistingRule;
                    newSetValuesScope.ObjSelectedlstValuesData.IsDescriptionSameAsExpression = newSetValuesScope.setValues.IsDescriptionSameAsExpression;
                    newSetValuesScope.ObjSelectedlstValuesData.IsStatic = newSetValuesScope.setValues.IsStatic;
                    newSetValuesScope.onSetValuescloseClick();
                };

                newSetValuesScope.onSetValuescloseClick = function () {
                    if (newSetValuesScope.dialog) {
                        newSetValuesScope.dialog.close();
                        newSetValuesScope.ObjSelectedlstValuesData = null;

                    }
                };

                for (var i = 0; i < obj.Data.length; i++) {
                    if (i <= 20) {
                        newSetValuesScope.lstSetValuesData.push(obj.Data[i]);
                    }
                    else {
                        break;
                    }
                }

                newSetValuesScope.setColumnNamesinLst(newSetValuesScope.lstSetValuesData[0]);
                newSetValuesScope.ValidateExistingParameterValuesForDecVersion = function () {
                    var isErrorFound = false;
                    newSetValuesScope.ErrorMessage = "";
                    if (!isErrorFound) {
                        for (var i = 0; i < newSetValuesScope.lstInputColumns.length; i++) {
                            if (!newSetValuesScope.lstInputColumns[i].ColumnName) {
                                isErrorFound = true;
                                newSetValuesScope.ErrorMessage = "Error : Map Input column for  " + newSetValuesScope.lstInputColumns[i].Name;
                                break;
                            }
                        }
                    }
                    if (!isErrorFound) {
                        for (var i = 0; i < newSetValuesScope.lstOutputColumns.length; i++) {
                            if (!newSetValuesScope.lstOutputColumns[i].ColumnName) {
                                isErrorFound = true;
                                newSetValuesScope.ErrorMessage = "Error : Map Output column for  " + newSetValuesScope.lstOutputColumns[i].Name;
                                break;
                            }
                        }
                    }
                    if (!isErrorFound) {
                        for (var i = 0; i < newSetValuesScope.lstReturnParameters.length; i++) {
                            if (!newSetValuesScope.lstReturnParameters[i].ColumnName) {
                                isErrorFound = true;
                                newSetValuesScope.ErrorMessage = "Error : Map Return column for  " + newSetValuesScope.lstReturnParameters[i].Name;
                                break;
                            }
                        }
                    }
                };
                if (newSetValuesScope.objExcelMatrixDetails.SelectedTable == "DecisionTable") {
                    newSetValuesScope.lstInputColumns = [];
                    newSetValuesScope.lstOutputColumns = [];
                    newSetValuesScope.lstReturnParameters = [];
                    angular.forEach(newSetValuesScope.ObjSelectedlstValuesData.InputColumns, function (value, key) {
                        var strValue = JSON.stringify(value);
                        value = JSON.parse(strValue);
                        newSetValuesScope.lstInputColumns.push(value);
                    });
                    angular.forEach(newSetValuesScope.ObjSelectedlstValuesData.OutputColumns, function (value, key) {
                        var strValue = JSON.stringify(value);
                        value = JSON.parse(strValue);
                        newSetValuesScope.lstOutputColumns.push(value);
                    });
                    //For Return Parameters
                    if (newSetValuesScope.ObjSelectedlstValuesData && newSetValuesScope.ObjSelectedlstValuesData.ReturnParameter.length > 0) {
                        var strValue = JSON.stringify(newSetValuesScope.ObjSelectedlstValuesData.ReturnParameter[0]);
                        value = JSON.parse(strValue);
                        newSetValuesScope.lstReturnParameters.push(value);
                    }

                    if (newSetValuesScope.ObjSelectedlstValuesData.isExistingRule) {
                        newSetValuesScope.ValidateExistingParameterValuesForDecVersion();
                    }
                    //$scope.lstobjExceldata = obj.Data;
                    //$scope.LoadSomeData();
                    //$scope.lstSetValuesData = obj.Data;
                    newSetValuesScope.dialogOptions = {
                        height: 700, width: 900
                    };
                    newSetValuesScope.onDecisionTableDataTypeChanged = function () {
                        if (newSetValuesScope.ObjSelectedlstValuesData.ReturnType == "") {
                            newSetValuesScope.lstReturnParameters = [];
                        }
                    };
                    newSetValuesScope.DataTypes = newScope.ReturnTypes;
                    newSetValuesScope.templateName = "Rule/views/SetValuesForImportExcel.html";
                    if (newSetValuesScope.templateName && newSetValuesScope.templateName.trim().length > 0) {
                        newSetValuesScope.dialog = $rootScope.showDialog(newSetValuesScope, 'Set Values', newSetValuesScope.templateName, newSetValuesScope.dialogOptions);
                    }

                }
                else {
                    newSetValuesScope.DistinctFileValue = newSetValuesScope.ObjSelectedlstValuesData.DistinctFileValue;
                    newSetValuesScope.XAxisCellValue = newSetValuesScope.ObjSelectedlstValuesData.XAxisCellValue;
                    newSetValuesScope.YAxisCellValue = newSetValuesScope.ObjSelectedlstValuesData.YAxisCellValue;
                    newSetValuesScope.Value = newSetValuesScope.ObjSelectedlstValuesData.Value;
                    newSetValuesScope.dialogOptions = {
                        height: 700, width: 900
                    };
                    newSetValuesScope.templateName = "Rule/views/SetValuesForExcelMatrix.html";
                    if (newSetValuesScope.templateName && newSetValuesScope.templateName.trim().length > 0) {
                        newSetValuesScope.dialog = $rootScope.showDialog(newSetValuesScope, 'Set Values', newSetValuesScope.templateName, newSetValuesScope.dialogOptions);
                    }

                    newSetValuesScope.onImoprtExcelValueKeyDown = function (eargs) {
                        var input = eargs.target;
                        var charCode = (eargs.which) ? eargs.which : eargs.keyCode;
                        var data = newSetValuesScope.lstColumns;
                        if (eargs.ctrlKey && eargs.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
                            if (data && data.length > 0) {
                                $(input).autocomplete("search", $(input).val());
                            }
                            eargs.preventDefault();
                        }
                        controlSelection = $(input);
                        if (data && data.length > 0) {
                            setSingleLevelAutoComplete(controlSelection, data);
                            eargs.stopPropagation();
                        }
                    };

                    newSetValuesScope.SetValuesIntellisenseButtonClick = function (event) {
                        var inputElement;
                        inputElement = $(event.target).prevAll("input[type='text']");
                        inputElement.focus();
                        if (inputElement) {
                            var data = newSetValuesScope.lstColumns;
                            if (data && data.length > 0) {
                                setSingleLevelAutoComplete(inputElement, data);
                                if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
                            }
                        }
                        if (event) {
                            event.stopPropagation();
                        }
                    };

                    newSetValuesScope.SetValuesYAxisCellValueChange = function (val) {
                        newSetValuesScope.YAxisCellValue = val;
                    };
                    newSetValuesScope.SetValuesXAxisCellValueChange = function (val) {
                        newSetValuesScope.XAxisCellValue = val;
                    };
                    newSetValuesScope.SetValuesValueChange = function (val) {
                        newSetValuesScope.Value = val;
                    };

                    newSetValuesScope.validatesetValues = function () {
                        if (newSetValuesScope.YAxisCellValue == undefined || newSetValuesScope.YAxisCellValue == "") {
                            newSetValuesScope.setvaluesError = "Error: Enter Y Axis Value";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.lstColumns.indexOf(newSetValuesScope.YAxisCellValue.trim()) == -1) {
                            newSetValuesScope.setvaluesError = "Error: Invalid Y Axis Value.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.XAxisCellValue == undefined || newSetValuesScope.XAxisCellValue == "") {
                            newSetValuesScope.setvaluesError = "Error: Enter X Axis Value";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.lstColumns.indexOf(newSetValuesScope.XAxisCellValue.trim()) == -1) {
                            newSetValuesScope.setvaluesError = "Error: Invalid X Axis Value.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.Value == undefined || newSetValuesScope.Value == "") {
                            newSetValuesScope.setvaluesError = "Error: Enter Value";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.lstColumns.indexOf(newSetValuesScope.Value.trim()) == -1) {
                            newSetValuesScope.setvaluesError = "Error: Invalid Value.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.DistinctFileValue && newSetValuesScope.lstColumns.indexOf(newSetValuesScope.DistinctFileValue.trim()) == -1) {
                            newSetValuesScope.setvaluesError = "Error: Invalid Distinct File Value.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if ((newSetValuesScope.XAxisCellValue.trim().toLowerCase() === newSetValuesScope.YAxisCellValue.trim().toLowerCase()) ||
                            (newSetValuesScope.XAxisCellValue.trim().toLowerCase() === newSetValuesScope.Value.trim().toLowerCase()) ||
                            (newSetValuesScope.XAxisCellValue.trim().toLowerCase() === (newSetValuesScope.DistinctFileValue && newSetValuesScope.DistinctFileValue.trim().toLowerCase())) ||
                            (newSetValuesScope.YAxisCellValue.trim().toLowerCase() === newSetValuesScope.Value.trim().toLowerCase()) ||
                            (newSetValuesScope.YAxisCellValue.trim().toLowerCase() === (newSetValuesScope.DistinctFileValue && newSetValuesScope.DistinctFileValue.trim().toLowerCase())) ||
                            (newSetValuesScope.Value.trim().toLowerCase() === (newSetValuesScope.DistinctFileValue && newSetValuesScope.DistinctFileValue.trim().toLowerCase()))) {
                            newSetValuesScope.setvaluesError = "Error: X Axis, Y Axis, Value and Distinct File fields should have different values.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (newSetValuesScope.SetDetailsErrorMessage && newSetValuesScope.SetDetailsErrorMessage.trim().length > 0) {
                            newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }
                        else if (!newSetDetailsScope) {
                            newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                            newSetValuesScope.setValueShowError = true;
                            return true;
                        }

                        else if (newSetDetailsScope) {
                            if (newSetDetailsScope.ObjSelectedlstValuesData) {
                                newSetDetailsScope.ShowError = true;
                                if (!(newSetDetailsScope.setDetailsReturnType && newSetDetailsScope.setDetailsReturnType.trim().length > 0)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (!(newSetDetailsScope.setDetailsRowParamName && newSetDetailsScope.setDetailsRowParamName.trim().length > 0)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (!isValidIdentifier(newSetDetailsScope.setDetailsRowParamName)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (newSetDetailsScope.existingParameter(newSetDetailsScope.setDetailsRowParamName, newSetDetailsScope.ObjSelectedlstValuesData)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;

                                }
                                else if (!(newSetDetailsScope.setDetailsRowReturnType && newSetDetailsScope.setDetailsRowReturnType.trim().length > 0)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (!(newSetDetailsScope.setDetailsColumnParamName && newSetDetailsScope.setDetailsColumnParamName.trim().length > 0)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (!isValidIdentifier(newSetDetailsScope.setDetailsColumnParamName)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else if (newSetDetailsScope.existingParameter(newSetDetailsScope.setDetailsColumnParamName, newSetDetailsScope.ObjSelectedlstValuesData)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;

                                }
                                else if (!(newSetDetailsScope.setDetailsColumnReturnType && newSetDetailsScope.setDetailsColumnReturnType.trim().length > 0)) {
                                    newSetValuesScope.setvaluesError = "Error: Enter Correct Values in details section.";
                                    newSetValuesScope.setValueShowError = true;
                                    return true;
                                }
                                else {
                                    newSetValuesScope.setValueShowError = false;
                                    return false;
                                }
                            }
                        }
                        else {
                            newSetValuesScope.setValueShowError = false;
                            return false;
                        }
                    };

                    // set details for excel martrix
                    newSetValuesScope.OnSetDetailsClick = function () {
                        newSetDetailsScope = $scope.$new();
                        newScope.isSetDetailsLoaded = true;
                        newScope.setDetailsScope = newSetDetailsScope;
                        newSetDetailsScope.ObjSelectedlstValuesData = newSetValuesScope.ObjSelectedlstValuesData;
                        newSetDetailsScope.existingParameter = function (parameterName, obj) {
                            if (obj.ExcelMatrixName && obj.ExcelMatrixName.trim().length > 0) {
                                if (!$scope.currentEntityExcelMatrix) {
                                    $scope.currentEntityExcelMatrix = [];
                                }
                                var rules = $scope.currentEntityExcelMatrix.filter(function (x) {
                                    return x.ID == obj.ExcelMatrixName;
                                });
                                if (rules && rules.length > 0) {
                                    if (rules[0].Parameters.some(function (x) {
                                        return x.ID == parameterName;
                                    })) {
                                        return true;
                                    }
                                }
                            }
                            return false;
                        };
                        newSetDetailsScope.ResetSetDetailsData = function () {
                            newSetDetailsScope.setDetailsReturnType = newSetDetailsScope.ObjSelectedlstValuesData.ReturnType;
                            newSetDetailsScope.isSetDetailsReturnTypeDisable = newSetDetailsScope.ObjSelectedlstValuesData.isReturnTypeDisable;
                            newSetDetailsScope.setDetailsReturn = newSetDetailsScope.ObjSelectedlstValuesData.ReturnType;
                            newSetDetailsScope.setDetailsColumnReturnType = newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.DataType;
                            newSetDetailsScope.setDetailsColumnReturn = newSetDetailsScope.setDetailsColumnReturnType;
                            newSetDetailsScope.setDetailsRowReturnType = newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.DataType;
                            newSetDetailsScope.setDetailsRowReturn = newSetDetailsScope.setDetailsRowReturnType;
                            newSetDetailsScope.setDetailsRowParamName = "";
                            newSetDetailsScope.setDetailsColumnParamName = "";
                            newSetDetailsScope.setDetailsRowDescription = "";
                            newSetDetailsScope.setDetailsColumnDescription = "";
                            if (newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Name && newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Name.trim().length > 0) {
                                newSetDetailsScope.setDetailsRowParamName = newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Name;
                            }
                            else {
                                if (newSetValuesScope.XAxisCellValue) {
                                    newSetDetailsScope.setDetailsRowParamName = newSetValuesScope.XAxisCellValue.replace(" ", "");
                                }

                            }
                            if (newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Name && newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Name.trim().length > 0) {
                                newSetDetailsScope.setDetailsColumnParamName = newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Name;
                            }
                            else {
                                if (newSetValuesScope.YAxisCellValue) {
                                    newSetDetailsScope.setDetailsColumnParamName = newSetValuesScope.YAxisCellValue.replace(" ", "");
                                }
                            }

                            if (newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Description && newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Description.trim().length > 0) {
                                newSetDetailsScope.setDetailsRowDescription = newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Description;
                            }
                            else {
                                newSetDetailsScope.setDetailsRowDescription = newSetValuesScope.XAxisCellValue;
                            }
                            if (newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Description && newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Description.trim().length > 0) {
                                newSetDetailsScope.setDetailsColumnDescription = newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Description;
                            }
                            else {
                                newSetDetailsScope.setDetailsColumnDescription = newSetValuesScope.YAxisCellValue;
                            }
                        };

                        newSetDetailsScope.ValidateSetDetails = function () {
                            newSetDetailsScope.SetDetailsErrorMessage = "";
                            newSetDetailsScope.ShowError = true;
                            if (!(newSetDetailsScope.setDetailsReturnType && newSetDetailsScope.setDetailsReturnType.trim().length > 0)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Return type cannot be empty.";
                                return true;
                            }
                            else if (!(newSetDetailsScope.setDetailsRowParamName && newSetDetailsScope.setDetailsRowParamName.trim().length > 0)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Row Parameter Name cannot be empty.";
                                return true;
                            }
                            else if (!isValidIdentifier(newSetDetailsScope.setDetailsRowParamName)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Invalid Row Parameter Name.";
                                return true;
                            }
                            else if (newSetDetailsScope.existingParameter(newSetDetailsScope.setDetailsRowParamName, newSetDetailsScope.ObjSelectedlstValuesData)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "A Parameter with same name already existing in the selected rule.";

                            }
                            else if (!(newSetDetailsScope.setDetailsRowReturnType && newSetDetailsScope.setDetailsRowReturnType.trim().length > 0)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Row Parameter Data Type cannot be empty.";
                                return true;
                            }
                            else if (!(newSetDetailsScope.setDetailsColumnParamName && newSetDetailsScope.setDetailsColumnParamName.trim().length > 0)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Column Parameter Name cannot be empty.";
                                newSetDetailsScope.ShowError = true;
                                return true;
                            }
                            else if (!isValidIdentifier(newSetDetailsScope.setDetailsColumnParamName)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Invalid Column Parameter Name.";
                                newSetDetailsScope.ShowError = true;
                                return true;
                            }
                            else if (newSetDetailsScope.existingParameter(newSetDetailsScope.setDetailsColumnParamName, newSetDetailsScope.ObjSelectedlstValuesData)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "A Parameter with same name already existing in the selected rule.";

                            }
                            else if (!(newSetDetailsScope.setDetailsColumnReturnType && newSetDetailsScope.setDetailsColumnReturnType.trim().length > 0)) {
                                newSetDetailsScope.SetDetailsErrorMessage = "Column Parameter Data Type cannot be empty.";
                                newSetDetailsScope.ShowError = true;
                                return true;
                            }
                            else {
                                newSetDetailsScope.ShowError = false;
                                return false;
                            }
                        };
                        newSetDetailsScope.ResetSetDetailsData();
                        newSetDetailsScope.ValidateSetDetails();
                        newSetDetailsScope.ReturnTypes = newScope.ReturnTypes;
                        newSetDetailsScope.dialogOptions = {
                            height: 600, width: 750
                        };
                        newSetDetailsScope.templateName = "Rule/views/SetDetailsForExcelMatrix.html";
                        if (newSetDetailsScope.templateName && newSetDetailsScope.templateName.trim().length > 0) {
                            newSetDetailsScope.dialog = $rootScope.showDialog(newSetDetailsScope, 'Set Details', newSetDetailsScope.templateName, newSetDetailsScope.dialogOptions);
                        }
                        newSetDetailsScope.onSetDetailsCancelClick = function () {
                            if (newSetDetailsScope.dialog) {
                                newSetDetailsScope.dialog.close();
                            }
                            newSetDetailsScope.ResetSetDetailsData();
                        };

                        newSetDetailsScope.SetDetailsReturnTypeChange = function (returnType) {
                            newSetDetailsScope.setDetailsReturnType = returnType;
                        };

                        newSetDetailsScope.SetDetailsRowParameterNameChange = function (para) {
                            newSetDetailsScope.setDetailsRowParamName = para;
                        };
                        newSetDetailsScope.SetDetailsRowReturnTypeChange = function (returnType) {
                            newSetDetailsScope.setDetailsRowReturnType = returnType;
                        };

                        newSetDetailsScope.SetDetailsColumnParameterNameChange = function (para) {
                            newSetDetailsScope.setDetailsColumnParamName = para;
                        };
                        newSetDetailsScope.SetDetailsColumnReturnTypeChange = function (returnType) {
                            newSetDetailsScope.setDetailsColumnReturnType = returnType;
                        };


                        newSetDetailsScope.onSetDetailsOkClick = function () {
                            if (!newSetDetailsScope.ObjSelectedlstValuesData.isReturnTypeDisable) {
                                newSetDetailsScope.ObjSelectedlstValuesData.ReturnType = newSetDetailsScope.setDetailsReturnType;
                                newSetValuesScope.setValues.ReturnType = newSetDetailsScope.setDetailsReturnType;
                            }
                            newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.DataType = newSetDetailsScope.setDetailsColumnReturnType;
                            newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.DataType = newSetDetailsScope.setDetailsRowReturnType;
                            newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Name = newSetDetailsScope.setDetailsRowParamName;
                            newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Name = newSetDetailsScope.setDetailsColumnParamName;
                            newSetDetailsScope.ObjSelectedlstValuesData.RowParameter.Description = newSetDetailsScope.setDetailsRowDescription;
                            newSetDetailsScope.ObjSelectedlstValuesData.ColumnParameter.Description = newSetDetailsScope.setDetailsColumnDescription;
                            newSetDetailsScope.onSetDetailsCancelClick();
                        };

                    };
                }

            };

            //import Excel matrix for Query
            newScope.OnSearchClickInCreateExcelMatrix = function () {
                var newExcelQueryScope = $scope.$new();
                newExcelQueryScope.objExcelMatrixDetails = newScope.objExcelMatrixDetails;
                newExcelQueryScope.objExcelMatrixDetails.ExcelQuery = newExcelQueryScope.objExcelMatrixDetails.ExcelSelectedQuery;
                newExcelQueryScope.QueryID = newExcelQueryScope.objExcelMatrixDetails.QueryID;
                newExcelQueryScope.QueryDataType = newExcelQueryScope.objExcelMatrixDetails.QueryDataType;
                newExcelQueryScope.isCodeQuery = newExcelQueryScope.objExcelMatrixDetails.isCodeQuery;
                newExcelQueryScope.QueryParameters = newExcelQueryScope.objExcelMatrixDetails.QueryParameters;
                newExcelQueryScope.parameterTypes = ['', 'int', 'long', 'string', 'decimal', 'DateTime'];

                newExcelQueryScope.QueryDataTypes = ['', 'bool', 'DateTime', 'decimal', 'double', 'float', 'int', 'long', 'short', 'string', 'EntityTable'];

                if (newScope.objExcelMatrixDetails.SelectedFile == 'Query') {
                    newExcelQueryScope.templateName = "Rule/Views/ExcelMatrixQueryTemplate.html";
                    newExcelQueryScope.dialog = $rootScope.showDialog(newExcelQueryScope, "Select Query", newExcelQueryScope.templateName, {
                        height: 600, width: 1000
                    });
                }

                newExcelQueryScope.onCloseQueryTemplateClick = function () {
                    if (newExcelQueryScope.dialog) {
                        newExcelQueryScope.dialog.close();
                    }
                };

                newExcelQueryScope.setQueryBasedOnEntityandQueryID = function (QueryID) {
                    if (newExcelQueryScope.isCodeQuery) {
                        var lst = QueryID.split(".");
                        newExcelQueryScope.objExcelMatrixDetails.ExcelQuery = "";
                        if (QueryID != "" && lst.length > 1) {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            var Entity = entityIntellisenseList.filter(function (x) {
                                return x.ID == lst[0];
                            });
                            if (lst[1] != "" && lst[0] != "") {
                                var Query = Entity[0].Queries.filter(function (y) {
                                    return y.ID == lst[1];
                                });
                                if (Query.length > 0) {
                                    newExcelQueryScope.objExcelMatrixDetails.ExcelQuery = Query[0].SqlQuery;
                                    newExcelQueryScope.getParametersBasedOnQuery(Query[0].SqlQuery);
                                }
                            }
                        }
                    }
                };

                newExcelQueryScope.onOkQueryTemplateClick = function () {
                    newExcelQueryScope.objExcelMatrixDetails.ExcelSelectedQuery = newExcelQueryScope.objExcelMatrixDetails.ExcelQuery;
                    newExcelQueryScope.objExcelMatrixDetails.QueryID = newExcelQueryScope.QueryID;
                    newExcelQueryScope.objExcelMatrixDetails.QueryDataType = newExcelQueryScope.QueryDataType;
                    newExcelQueryScope.objExcelMatrixDetails.isCodeQuery = newExcelQueryScope.isCodeQuery;
                    newExcelQueryScope.objExcelMatrixDetails.QueryParameters = newExcelQueryScope.QueryParameters;
                    newExcelQueryScope.onCloseQueryTemplateClick();
                };

                newExcelQueryScope.getParametersBasedOnQuery = function (strQuery) {
                    if (strQuery != undefined && strQuery != '') {
                        var query = strQuery;
                        var larrSql = query.split(/[\s,()=\r\n]+/);
                        var larrParameter = [];
                        var lstrParameterName;
                        var queryParameter;
                        var parameterElements = [];

                        //adds all parameters of query to array and whichever param is not in Elements is added to Elements
                        for (var i = 0; i < larrSql.length; i++) {
                            var lblnFound = false;
                            if (larrSql[i].indexOf("@") > -1) {
                                queryParameter = larrSql[i];

                                if (larrParameter.indexOf(queryParameter) == -1) {
                                    var newQueryParameter = {
                                    };
                                    newQueryParameter.Name = "parameter";
                                    newQueryParameter.dictAttributes = {
                                    };
                                    newQueryParameter.dictAttributes.ID = queryParameter;
                                    newQueryParameter.dictAttributes.sfwDataType = "";
                                    newQueryParameter.Value = "";
                                    parameterElements.push(newQueryParameter);
                                    larrParameter.push(queryParameter);
                                }
                                //var objparameterdetails = { Parameter: queryParameter, Type: newQueryParameter.dictAttributes['sfwDataType'] }

                            }
                        }
                        //checks for updated query parameters and removes params from elements which are not in current query
                        for (var i = 0; i < parameterElements.length; i++) {
                            var parameterName = parameterElements[i].dictAttributes.ID;
                            if (larrParameter.indexOf(parameterName) == -1) {
                                parameterElements.splice(i, 1);
                            }
                        }
                    }
                    newExcelQueryScope.QueryParameters = parameterElements;
                };



                newExcelQueryScope.AddNodeLockOperation = function () {
                    if (newExcelQueryScope.objExcelMatrixDetails.ExcelQuery) {
                        var callQueryWithNoLock = $FormatQueryFactory.createQueryWithNoLock(newExcelQueryScope.objExcelMatrixDetails.ExcelQuery);
                        callQueryWithNoLock.then(function (astrQuery) {
                            if (astrQuery) {
                                $scope.$evalAsync(function () {
                                    newExcelQueryScope.objExcelMatrixDetails.ExcelQuery = astrQuery;
                                });
                            }
                        });
                    }
                };

                newExcelQueryScope.formatQuery = function () {
                    if (newExcelQueryScope.objExcelMatrixDetails.ExcelQuery) {
                        var callformattedQuery = $FormatQueryFactory.formatQuery(newExcelQueryScope.objExcelMatrixDetails.ExcelQuery);
                        callformattedQuery.then(function (astrQuery) {
                            if (astrQuery) {
                                $scope.$evalAsync(function () {
                                    newExcelQueryScope.objExcelMatrixDetails.ExcelQuery = astrQuery;
                                });
                            }
                        });
                    }
                };

            };
        }
        else {
            $scope.LoadRuleReturnTypes(ruleType);

            var newScope = $scope.$new();
            newScope.showExtraFieldsTab = false;
            newScope.objRuleExtraFields = {
                Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
            };
            newScope.formName = "Rule";
            newScope.objDirFunctions = {};
            newScope.SelectedNewRuleDetailsDialogTab = 'Details';
            newScope.ruleErrorMessageForDisplay = undefined;
            $scope.objRule = {
            };
            newScope.objRule.RuleType = ruleType;
            newScope.objRule.IsDefaultVersion = true;

            if ($scope.currentFileName) {
                newScope.objRule.EntityName = $scope.currentFileName;
            }
            else {

                var scope = getCurrentControllerScope();
                newScope.objRule.EntityName = scope.objEntity.dictAttributes.ID;
            }


            newScope.objRule.RuleID = "";
            newScope.objRule.Description = "";
            newScope.objRule.EffectiveDate = "";
            newScope.objRule.NeoTrackID = "";
            newScope.objRule.EntityReturnType = "";
            newScope.objRule.ReturnType = "";
            newScope.objRule.IsStatic = false;
            newScope.objRule.IsMatchAllCondition = false;
            $scope.objRule.IsThrowError = false;
            newScope.objRule.IsPrivate = false;
            newScope.objRule.Trace = false;
            newScope.objRule.CacheResult = false;



            newScope.newRuleDialog = $rootScope.showDialog(newScope, "New Rule", "Rule/views/NewRuleTemplate.html", {
                height: 600, width: 1100
            });
            ComponentsPickers.init();

            hubMain.server.getAllRuleFiles("ScopeId_" + newScope.$id);

            newScope.receiveAllRuleFiles = function (data) {
                newScope.$apply(function () {
                    newScope.files = data;
                });
            };

            newScope.selectNewDetailsDialogTab = function () {
                newScope.SelectedNewRuleDetailsDialogTab = 'Details';
            };
            newScope.selectNewExtraFieldsDialogTab = function () {
                newScope.SelectedNewRuleDetailsDialogTab = 'ExtraFields';
            };

            newScope.CreateNewRuleDetails = function () {
                newScope.SelectedNewRuleDetailsDialogTab = 'Details';
                newScope.ID = undefined;
                newScope.objExtraFields = [];

            };
            newScope.CreateNewRuleDetails();

            newScope.onSelectedTableChanged = function (selectedTable) {
                newScope.SelectedTable = selectedTable;
                angular.forEach(newScope.lstimportExcelFileData, function (item) {
                    item.ExcelMatrixName = item.EffectiveDate = "";
                    newScope.validateExcelData(item);
                });
            };


            newScope.validateNewRule = function () {
                newScope.ruleErrorMessageForDisplay = undefined;
                if (!newScope.objRule.RuleID) {
                    newScope.ruleErrorMessageForDisplay = "Error: ID cannot be empty.";
                    return true;
                }
                else if (!isValidIdentifier(newScope.objRule.RuleID)) {
                    newScope.ruleErrorMessageForDisplay = "Error: Invalid ID.";
                    return true;
                }
                else if ((newScope.objRule.ReturnType == 'Collection' || newScope.objRule.ReturnType == 'List' || newScope.objRule.ReturnType == 'Object') && !newScope.objRule.EntityReturnType && newScope.objRule.EntityReturnType.trim().length == 0) {
                    newScope.ruleErrorMessageForDisplay = "Enter Return Entity Type.";
                    return true;
                }
                else if (!newScope.objRule.IsDefaultVersion && (!newScope.objRule.EffectiveDate || newScope.objRule.EffectiveDate == "")) {
                    newScope.ruleErrorMessageForDisplay = "Error: Effective Date Required.";
                    return true;
                }
                else if (newScope.files && newScope.files.length > 0) {
                    var lst = newScope.files.filter(function (x) {
                        return x.FileName.toLowerCase() == "rul" + newScope.objRule.RuleID.toLowerCase();
                    });
                    if (lst && lst.length > 0) {
                        newScope.ruleErrorMessageForDisplay = "Error: The rule name \"rul" + newScope.objRule.RuleID + "\"  already exists. Please enter a different Name (ID).";
                        return true;
                    }
                }


                if (newScope.objDirFunctions.getExtraFieldData) {
                    newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
                }

                var extraValueFlag = validateExtraFields(newScope);
                if (extraValueFlag) {
                    newScope.ruleErrorMessageForDisplay = newScope.FormDetailsErrorMessage;
                    return true;
                }

                if (newScope.ruleErrorMessageForDisplay != "" && newScope.ruleErrorMessageForDisplay != undefined) {
                    return true;
                }
                return false;
            };
            newScope.DefaultVersionCheckChange = function () {
                if (newScope.objRule.IsDefaultVersion) {
                    newScope.objRule.EffectiveDate = "";
                }
                newScope.validateNewRule();
            };

            newScope.createNewRule = function () {

                if (newScope.objDirFunctions.prepareExtraFieldData) {
                    newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                }

                if (newScope.objRule != undefined) {
                    if ($scope.currentFilePath) {
                        $.connection.hubEntityModel.server.createNewRule("rul" + newScope.objRule.RuleID, newScope.objRule.Description, newScope.objRule.RuleType, newScope.objRule.ReturnType, newScope.objRule.EffectiveDate, newScope.objRule.NeoTrackID, newScope.objRule.EntityName, newScope.objRule.EntityReturnType, newScope.objRule.IsStatic, newScope.objRule.IsMatchAllCondition, newScope.objRule.IsThrowError, newScope.objRule.IsDefaultVersion, newScope.objRule.IsPrivate, $scope.currentFilePath, newScope.objRule.Trace, newScope.objRule.CacheResult, newScope.objRuleExtraFields).done(function (data) {
                            newScope.receieveNewRule(data);
                        });
                    }
                    else {
                        $.connection.hubEntityModel.server.createNewRule("rul" + newScope.objRule.RuleID, newScope.objRule.Description, newScope.objRule.RuleType, newScope.objRule.ReturnType, newScope.objRule.EffectiveDate, newScope.objRule.NeoTrackID, newScope.objRule.EntityName, newScope.objRule.EntityReturnType, newScope.objRule.IsStatic, newScope.objRule.IsMatchAllCondition, newScope.objRule.IsThrowError, newScope.objRule.IsDefaultVersion, newScope.objRule.IsPrivate, $rootScope.currentopenfile.file.FilePath, newScope.objRule.Trace, newScope.objRule.CacheResult, newScope.objRuleExtraFields).done(function (data) {
                            newScope.receieveNewRule(data);
                        });
                    }
                }
            };

            newScope.receieveNewRule = function (data) {
                var lst = data;
                newScope.$apply(function () {

                    var obj = lst[0];
                    if (newScope.files != null) {
                        newScope.files.push(obj);
                        $rootScope.openFile(obj);
                    }
                    newScope.closeNewRuleWindow();

                    if (newScope.objRule) {
                        //Adding rule in the intellisense list. 
                        //Note that before adding this rule to entity rules list, we will have to add it to intellisense coz that entity rule search is dependent on intellisense
                        var entityid = newScope.objRule.EntityName;
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var entities = entityIntellisenseList.filter(function (x) {
                            return x.ID == entityid;
                        });
                        if (entities.length > 0) {
                            entities[0].Rules = entities[0].Rules.concat(lst[1]);
                        }

                        var entityScope = getScopeByFileName(entityid);
                        if (entityScope != undefined) {
                            if (entityScope && entityScope.lstItemsRulesList) {
                                entityScope.lstItemsRulesList.push(lst[0]);
                            }
                            if (entityScope.objEntity && entityScope.objEntity.dictAttributes.ID == newScope.objRule.EntityName) {
                                var objEntityRuleDetails = {
                                    DataSourceBasedScenario: null, EntityScenario: null, ExcelBasedScenario: null, ID: obj.FileName, IsVisible: false, ObjectBasedScenario: null, RuleFileDetails: obj, Type: obj.FileType
                                };
                                entityScope.lstEntityRules.push(objEntityRuleDetails);
                                entityScope.SearchRuleCommand();

                            }
                        }


                    }
                });
            };

            newScope.closeNewRuleWindow = function () {
                newScope.newRuleDialog.close();
            };
        }
    };

    $scope.ResetAllPropertiesOfExcelMatrixTemplate = function () {
        $scope.lstLoadedExcelData = [];
        $scope.lstInputColumns = [];
        $scope.lstOutputColumns = [];
        $scope.lstSetValuesData = [];
        //$scope.lstReturnParameters = [];
        $scope.lstimportExcelFileData = [];

        $scope.objExcelMatrixDetails.SelectedMatrix = "ImportExcelMatrix";
        $scope.objExcelMatrixDetails.SelectedFile = "ExcelFile";
        $scope.objExcelMatrixDetails.SelectedTable = "DecisionTable";
        $scope.objExcelMatrixDetails.ExcelFilePath = "";
        $scope.objExcelMatrixDetails.ExcelNeoTrackID = "";
        $scope.objExcelMatrixDetails.Precision = "6";
        $scope.objExcelMatrixDetails.Delimiter = ",";
        $scope.objExcelMatrixDetails.ColumnIndex = -1;
        $scope.objExcelMatrixDetails.RowIndex = -1;
        $scope.objExcelMatrixDetails.ExcelSelectedQuery = "";

        //For Query Template
        $scope.objExcelMatrixDetails.ExcelQuery = "";
        $scope.objExcelMatrixDetails.QueryDataType = "";
        $scope.objExcelMatrixDetails.isCodeQuery = false;
        $scope.objExcelMatrixDetails.QueryID = "";
        $scope.objExcelMatrixDetails.QueryParameters = [];
    };

    $scope.receiveFilePath = function (filepath) {
        $scope.$apply(function () {
            if (filepath.length > 0) {
                if (filepath[0].toLowerCase().endsWith(".xls") || filepath[0].toLowerCase().endsWith(".xlsx")) {
                    $scope.objExcelMatrixDetails.ExcelFilePath = filepath[0];
                } else {
                    $SgMessagesService.Message('Message', "Please select valid Excel File");
                }
            }
        });
    };

    $scope.selectNewDetailsDialogTab = function () {
        $scope.SelectedNewRuleDetailsDialogTab = 'Details';
    };

    $scope.selectNewExtraFieldsDialogTab = function () {
        $scope.SelectedNewRuleDetailsDialogTab = 'ExtraFields';
    };

    /*  newScope.CreateNewRuleDetails = function () {
          newScope.SelectedNewRuleDetailsDialogTab = 'Details';
          newScope.objExtraFields = [];
          newScope.ID = undefined;
          for (var i = 0; i < newScope.objCustomSettings.Elements.length; i++) {
              if (newScope.objCustomSettings.Elements[i].Name == "Rule") {
                  newScope.temp = newScope.objCustomSettings.Elements[i];
                  break;
              }
          }
          if (newScope.temp != undefined) {
              for (var i = 0; i < newScope.temp.Elements.length; i++) {
                  var dummyobj = { ID: newScope.temp.Elements[i].dictAttributes.value, Description: newScope.temp.Elements[i].dictAttributes.Description, ControlType: newScope.temp.Elements[i].dictAttributes.ControlType, Children: [], IsRequired: newScope.temp.Elements[i].dictAttributes.IsRequired };
                  dummyobj.Value = "";
                  if (dummyobj.ControlType == "HyperLink") {
                      dummyobj.URL = "";
                  }
                  if (newScope.temp.Elements[i].Elements.length > 0) {
                      for (var j = 0; j < newScope.temp.Elements[i].Elements.length; j++) {
                          var dummyobjchild = { ID: newScope.temp.Elements[i].Elements[j].dictAttributes.Text, Description: newScope.temp.Elements[i].Elements[j].dictAttributes.Description, Value: "" };
                          dummyobj.Children.push(dummyobjchild);
                      }
                  }
                  newScope.objExtraFields.push(dummyobj);
              }
          }
          if (newScope.objExtraFields.length > 0) {
              newScope.showExtraFieldsTab = true;
          }
          else {
              newScope.showExtraFieldsTab = false;
          }
          
      }
      */

    $scope.AddHyperLinkUrl = function (obj) {

        $scope.currentobjHyperLink = obj;
        var temp = JSON.stringify(obj);
        $scope.objHyperLink = JSON.parse(temp);
        $scope.Hyperlinkdialog = ngDialog.open({
            template: 'AddHyperLinkUrlForNewRule',
            scope: $scope,
            closeByDocument: false
        });
    };

    $scope.closeNewRuleDetailsHyperlink = function () {
        for (var i = 0; i < $scope.objExtraFields.length; i++) {
            if ($scope.objExtraFields[i].ID == $scope.currentobjHyperLink.ID) {
                $scope.currentobjHyperLink.URL = $scope.objHyperLink.URL;
                $scope.currentobjHyperLink.Value = $scope.objHyperLink.Value;
            }
        }
        if ($scope.Hyperlinkdialog != undefined) {
            ngDialog.close($scope.Hyperlinkdialog.id);
        }
    };

    $scope.onSelectedTableChanged = function (selectedTable) {
        $scope.SelectedTable = selectedTable;
        angular.forEach($scope.lstimportExcelFileData, function (item) {
            item.ExcelMatrixName = item.EffectiveDate = "";
            $scope.validateExcelData(item);
        });
    };

    $scope.validateHyperLink = function () {
        $scope.HyperlinkErrorMessage = "";
        if ((this.objHyperLink.Value == undefined || this.objHyperLink.Value == "")) {
            $scope.HyperlinkErrorMessage = "Error: Enter the Description.";
            return true;
        }
        else if (this.objHyperLink.URL == undefined) {
            $scope.HyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
            if ($scope.currentobjHyperLink.IsRequired == "True" || $scope.currentobjHyperLink.IsRequired == true) {
                $scope.ruleErrorMessageForDisplay = "Error: Invalid Hyperlink URL.";
            }
            return true;
        }
        else {
            var match = $scope.objHyperLink.URL.match(new RegExp(/^http(s)?:\/\/(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?/));
            if (match == null) {
                $scope.HyperlinkErrorMessage = "Error: Invalid Hyperlink URL.";
                if ($scope.currentobjHyperLink.IsRequired == "True" || $scope.currentobjHyperLink.IsRequired == true) {
                    $scope.ruleErrorMessageForDisplay = "Error: Invalid Hyperlink URL.";
                }
                return true;
            }
        }
    };

    $scope.selectionChange = function (obj, optID) {
        var index = $scope.objExtraFields.indexOf(obj);
        for (var i = 0; i < $scope.objExtraFields[index].Children.length; i++) {
            if ($scope.objExtraFields[index].Children[i].ID == optID) {
                $scope.ID = optID;
                $scope.objExtraFields[index].Children[i].Value = true;
                $scope.objExtraFields[index].Value = optID;
            }
            else {
                $scope.objExtraFields[index].Children[i].Value = false;
            }
        }
        $scope.validateNewRule();
    };


    $scope.updateExtraFieldTextBoxValue = function (textObject) {
        var obj = $scope.CreateNewObjectForExtraField(textObject);
        $scope.objRuleExtraFields.Elements.push(obj);
    };

    $scope.CreateNewObjectForExtraField = function (textObject) {
        var objItem = {
            Name: "ExtraField", dictAttributes: {}, Elements: []
        };
        objItem.dictAttributes.ID = textObject.ID;
        if (textObject.Value != undefined) {
            objItem.dictAttributes.Value = textObject.Value;
        }
        return objItem;
    };

    $scope.updateExtraFieldCheckBoxListValue = function (textObject) {
        var obj = $scope.CreateNewObjectForExtraFieldlist(textObject);
        if (obj.Value != undefined || obj.dictAttributes.Value != undefined) {
            $scope.objRuleExtraFields.Elements.push(obj);
        }
    };

    $scope.CreateNewObjectForExtraFieldlist = function (textObject) {
        var val = "";
        var count = 0;
        for (var j = 0; j < textObject.Children.length; j++) {
            if (textObject.Children[j].Value == "True") {
                if (count == 0) {
                    val += textObject.Children[j].ID;
                }
                else {
                    val += "," + textObject.Children[j].ID;
                }
                count++;
            }
        }
        var objItem = {
            Name: "ExtraField", dictAttributes: {
            }, Elements: []
        };
        objItem.dictAttributes.ID = textObject.ID;
        if (val != "") {
            objItem.dictAttributes.Value = val;
        }
        return objItem;
    };

    $scope.updateExtraFieldHyperLinkValue = function (textObject) {
        var objItem = {
            Name: "ExtraField", dictAttributes: {}, Elements: []
        };
        objItem.dictAttributes.ID = textObject.ID;
        if (textObject.Value != undefined) {
            objItem.dictAttributes.Value = textObject.Value;
        }
        if (textObject.URL != undefined) {
            objItem.dictAttributes.URL = textObject.URL;
        }
        $scope.objRuleExtraFields.Elements.push(objItem);
    };

    $scope.fileNameChanged = function (oFileInput) {
        $scope.$apply(function () {
            $scope.ExcelFilePath = oFileInput.value;
            $scope.$$childTail.ExcelFilePath = oFileInput.value;
            oFileInput.value = "";
        });
    };


    var selectedExcelMatrix = "";
    $scope.onRadioButtonClickInExcelMatrix = function (selectedparam) {
        if (this.SelectedMatrix != selectedExcelMatrix && selectedExcelMatrix != "") {
            $scope.ExcelFilePath = "";
            $scope.ExcelSelectedQuery = "";
            $scope.lstLoadedExcelData = [];
            $scope.lstimportExcelFileData = [];
        }
        selectedExcelMatrix = this.SelectedMatrix;
    };


    $scope.QueryDataType = "";

    $rootScope.LoadRuleReturnTypes = function (ruleType) {
        if (ruleType == "DecisionTable") {
            $rootScope.ReturnValues = ['', 'bool', 'datetime', 'decimal', 'float', 'double', 'int', 'long', 'short', 'string'];
        }
        else {
            $rootScope.ReturnValues = ['', 'bool', 'datetime', 'decimal', 'float', 'double', 'int', 'long', 'short', 'string', 'Collection', 'Object', 'List'];
        }
    };

    $scope.showErrorPage = function () {
        if ($("#invalidFiles").length > 0) {
            return;
        }
        if ($(".blink-err-btn").length > 0) {
            $(".blink-err-btn").removeClass("blink-err-btn");
        }
        var newScope = $scope.$new();
        if ($scope.fileErrorList && $scope.fileErrorList.length > 0) {
            newScope.fileErrorList = $scope.fileErrorList;
        } else {
            newScope.fileErrorList = [];
        }
        newScope.close = function () {
            dialog.close();
        };
        var dialog = $rootScope.showDialog(newScope, "Errors: Invalid Files", "Common/views/ErrorPage.html", { modal: false, showclose: true, isInfoDialog: true, width: 1000, height: 500 });
    };

    $scope.checkIfObject = function (input) {
        if (angular.isObject(input)) return true;
        else return false;
    };
    //#endregion
    if ($.connection && $.connection.hubMain && $.connection.hubMain.client) {
        $.connection.hubMain.client.refreshBrowser = function () {
            alert("Signalr Hub is shut down. Application will be reloaded.");
            window.location.href = "error_page.html?ERROR_CODE=101";
        };
    }

    $scope.showFormTreeMap = function (opt) {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (opt && scope) {
                scope.viewTreeMap = false;
            } else if (scope && scope.showFormTreeMap) {
                scope.showFormTreeMap();
            }
        }
    };
    $scope.activeFileListKeyDown = function (e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        var objParent = $(e.currentTarget).parents("[ui-active-file-wrapper]");
        switch (charCode) {
            // tab handlers
            case 9:
                if ($(objParent).find("li:last").hasClass("selected")) {
                    $(objParent).find("li.selected").removeClass("selected");
                    $(objParent).find("li:first").addClass("selected");
                    moveActiveFileUp(objParent);
                } else {
                    moveActiveFileDown(objParent);
                }
                break;
            // up handlers 
            case 38:
                moveActiveFileUp(objParent);
                break;
            // down handlers
            case 40:
                moveActiveFileDown(objParent);
                break;
            default:
        }
        if ($(objParent).find("li.selected").length) {
            var elemScope = angular.element($(objParent).find("li.selected")).scope();
            if (elemScope && elemScope.fileObj)
                setActiveFileInfo(elemScope.fileObj);
        }
    };
    var moveActiveFileDown = function (objParent) {
        if ($(objParent).find("li.selected").length > 0) {
            $(objParent).find('li:not(:last-child).selected').removeClass('selected').next().addClass('selected');
        }
        else {
            $(objParent).find('li:first-child').addClass('selected');
        }
        if ($(objParent).find("li.selected").length > 0) {
            $("[ui-active-file-wrapper] #active-files-list").scrollTo($(objParent).find("li.selected"), { offsetTop: 200, offsetLeft: 0 }, null);
        }
    };
    var moveActiveFileUp = function (objParent) {
        if ($(objParent).find("li.selected").length > 0) {
            $(objParent).find('li:not(:first-child).selected').removeClass('selected').prev().addClass('selected');
        }
        if ($(objParent).find("li.selected").length > 0) {
            $("[ui-active-file-wrapper] #active-files-list").scrollTo($(objParent).find("li.selected"), { offsetTop: 200, offsetLeft: 0 }, null);
        }
    };
    $scope.selectActiveFile = function (_file) {
        if (_file) {
            $rootScope.openFile(_file.file);
            $("#active-files").removeClass("show");
            $("#active-files").addClass("hide");
        }
    };
    var setActiveFileInfo = function (_fileInfo) {
        if (_fileInfo) {
            $scope.activeFileInfo = _fileInfo;
        }
    };

    $rootScope.showHelp = function (knowtionId) {
        var helpUrl = null;
        if (knowtionId) {
            helpUrl = getKnowtionHelpUrl(knowtionId);
        }
        else {
            helpUrl = "Technical Documentation/html/index.html";
        }

        if (helpUrl) {
            var dialogScope = $rootScope.$new();
            dialogScope.helpUrl = helpUrl;
            dialogScope.dialog = $rootScope.showDialog(dialogScope, "Help", "Common/views/HelpDialog.html", { width: 900, height: 600, showclose: true, closeOnFocusOut: true });
            dialogScope.close = function () {
                if (dialogScope.dialog && dialogScope.dialog.close) {
                    dialogScope.dialog.close();
                }
            }

            dialogScope.isUrlLoaded = false;

            function checkIframeLoaded(scope) {
                // Get a handle to the iframe element
                var iframe = document.getElementById('help-iframe');
                try {
                    var iframeDoc = iframe.contentDocument || iframe.contentWindow.document;

                    // Check if loading is complete
                    if (iframeDoc.readyState == 'complete') {
                        scope.$evalAsync(function () {
                            scope.isUrlLoaded = true;
                        });
                    }
                }
                catch (e) {
                    scope.$evalAsync(function () {
                        scope.isUrlLoaded = true;
                    });
                }

                if (!scope.isUrlLoaded) {
                    window.setTimeout(checkIframeLoaded, 100, scope);
                }
            }
            checkIframeLoaded(dialogScope);
        }
    }
    $rootScope.resetRootScopeVarible = function () {
        $scope.SideBarSearch = {};

        $scope.SideBarSearch.listFile = [];
        $scope.SideBarSearch.totalRecords = 0;
        $scope.SideBarSearch.FilefilterText = "";
        $scope.SideBarSearch.category = "";
        $scope.CurrentProjectName = undefined;
        $rootScope.IsLoading = false;
        $rootScope.currentopenfile = null;
        $rootScope.lstopenedfiles = [];
        $rootScope.UndoRedoList = []
        $rootScope.lstPrevious = [];
        $rootScope.lstNext = [];
        $rootScope.IsOracleDB = false;
        $rootScope.projectLoadError = false;
        $rootScope.isstartpagevisible = false;
        $rootScope.globalSearchText = "";
        delete $rootScope.openFromHtx;
        $scope.ruleBuildResult = null;

        $rootScope.userIdleTimeout = 0;
        $rootScope.rFuncMethods = [];
        $rootScope.isFilesListUpdated = undefined;
        $rootScope.IsProjectInitialLoad = true;
        $rootScope.IsWorkFlowMap = undefined;
        $rootScope.queryID = undefined;
        $resourceFactory.clearResourceFactory();
        $searchQuerybuilder.clearSearchQueryBuilder();
        $EntityIntellisenseFactory.clearEntityIntellisenseFactory();
        $searchFindReferences.resetData();
        ParameterFactory.clearParameterFactory();
        $('#globalSearch').val("");
        $rootScope.IsTfsUpdated = true;
        $rootScope.IsTFSConfigured = false;
        $rootScope.IsTfsUpdated = true;
        $rootScope.IsFilesAddedDelete = true;
    };

    //#region ChatBot 
    $scope.showAndHideChatWindow = function (ablnChatWindowVisible) {
        $scope.iblnChatWindowVisible = ablnChatWindowVisible;
    };
    $scope.showAndHideHelpWindow = function (ablnHelpWindow) {
        if (ablnHelpWindow) {
            $(".chat-help-parent").animate({ right: "0px" }, 1000);
        }
        else {
            $(".chat-help-parent").animate({ right: "-130px" }, 1000);
        }
        $scope.iblnChatHelpVisible = ablnHelpWindow;
    };
    //#endregion
}]);




app.controller('PageLayoutController', ["$scope", "$http", "$rootScope", "$Errors", "$DashboardFactory", function ($scope, $http, $rootScope, $Errors, $DashboardFactory) {
    $scope.isstartpagevisible = false;
    $scope.isConfigureSettingsVisible = false;

    //#region Page Header Actions

    $scope.closeFile = function (file) {
        $rootScope.CloseCurrentFiles(file);
    };

    $scope.toggleView = function (view) {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.ShowDesign) {
                if (view == "Design") {
                    if (scope.isSourceDirty) {
                        $rootScope.ClearUndoRedoListByFileName(scope.currentfile.FileName);
                    }
                    scope.ShowDesign();
                }
                else if (view == "Source") {
                    scope.ShowSource();
                }
            }
        }
    };
    $scope.selectDesignSource = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileName) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            $scope.isHideUndoredobuttons = false;
            if (scope) {
                $scope.isHideUndoredobuttons = scope.selectedDesignSource;
                return scope.selectedDesignSource;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    };

    $scope.getFileTitle = function () {
        $scope.currentEntityName = "";
        $scope.isBpmMapLocked = false;
        var title = "";
        var scope = {};
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            if ($rootScope.currentopenfile.file.FileType) {
                scope = getCurrentFileScope();
                if (scope) {
                    switch ($rootScope.currentopenfile.file.FileType) {
                        case "BPMN":
                        case "BPMTemplate":
                            if (scope.BPMNModel && scope.BPMNModel.Title && scope.BPMNModel.Title.trim().length > 0) title = scope.BPMNModel.Title;
                            if ($rootScope.currentopenfile.file.FileType == "BPMN") {
                                if (!scope.IsMapEditable) {
                                    $scope.isBpmMapLocked = true;
                                }
                            }
                            break;
                        case "Lookup":
                        case "Maintenance":
                        case "Wizard":
                        case "UserControl":
                        case "Tooltip":
                            if (scope.FormModel && scope.FormModel.dictAttributes) {
                                $scope.currentEntityName = scope.FormModel.dictAttributes.sfwEntity;
                                if (scope.FormModel.dictAttributes.sfwTitle && scope.FormModel.dictAttributes.sfwTitle.trim().length > 0) title = scope.FormModel.dictAttributes.sfwTitle;
                            }
                            break;
                        case "LogicalRule":
                        case "ExcelMatrix":
                            if (scope.objLogicalRule && scope.objLogicalRule.dictAttributes)
                                $scope.currentEntityName = scope.objLogicalRule.dictAttributes.sfwEntity;
                            break;
                        case "DecisionTable":
                            if (scope.objLogicalRule)
                                $scope.currentEntityName = scope.objLogicalRule.Entity;
                            break;
                        case "Correspondence":
                        case "PDFCorrespondence":
                            if (scope.objCorrespondenceDetails && scope.objCorrespondenceDetails.dictAttributes)
                                $scope.currentEntityName = scope.objCorrespondenceDetails.dictAttributes.sfwEntity;
                            break;
                        case "FormLinkMaintenance":
                        case "FormLinkLookup":
                        case "FormLinkWizard":
                            if (scope.FormLinkModel && scope.FormLinkModel.dictAttributes)
                                $scope.currentEntityName = scope.FormLinkModel.dictAttributes.sfwEntity;
                            break;
                        case "InboundFile":
                        case "OutboundFile":
                            if (scope.objFile && scope.objFile.dictAttributes) {
                                if (scope.objFile.dictAttributes.sfwTitle) {
                                    title = scope.objFile.dictAttributes.sfwTitle;
                                }
                            }
                            break;
                        case "Entity":
                            if (scope.objEntity && scope.objEntity.dictAttributes.sfwParentEntity) {
                                $scope.currentEntityName = scope.objEntity.dictAttributes.sfwParentEntity;
                            }
                            break;
                        default:
                    }
                }
            }
            if (title == "" && $rootScope.currentopenfile.file.FileName) {
                title = $rootScope.currentopenfile.file.FileName;
            }
            return title;
        }
    };

    $scope.getTfsIcon = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType) {
            switch ($rootScope.currentopenfile.file.FileType) {
                case "BPMN":
                case "BPMTemplate":
                    "";
                    break;
            }
            return $rootScope.currentopenfile.file.FileName;
        }
    };

    $scope.getFileTypeIcon = function () {
    };

    $scope.isCurrentFileDirty = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                return scope.isDirty;
            }
        }
    };

    $scope.onDetailClick = function () {
        var curfileid = $rootScope.currentopenfile.file;
        var scope;
        scope = getScopeByFileName(curfileid.FileName);
        if (curfileid.FileType == "UserControl" || curfileid.FileType == "Tooltip") {
            if (scope && scope.onUserControlDetailsClick) {
                scope.onUserControlDetailsClick();
            }
        } else {
            if (scope && scope.onDetailClick) {
                scope.onDetailClick();
            }
        }
    };

    $scope.showWizardProperty = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.showWizardProperty) {
            scope.showWizardProperty();
        }
    };

    $scope.refreshConstants = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.onRefreshAllGroups) {
            scope.onRefreshAllGroups();
        }
    };

    $scope.openValidateNew = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenValidateNewDialog();
            }
        }
    };

    //#region Rule
    $scope.lstViewModel = ['Developer View', 'Analyst View'];
    $scope.ViewMode = "Developer View";
    $scope.setViewMode = function (viewmode) {
        $scope.ViewMode = viewmode;
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.setViewMode(viewmode);
        }
    };
    $scope.versionWiseDisplay = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.versionWiseDisplay();
        }
    };
    $scope.OpenRunResult = function () {
        var RuleName = $scope.getRuleName(scope);
        var scope = getScopeByFileName("MainPage");
        if (scope) {
            scope.OpenRunResultClick(false, RuleName);
        }
    };
    $scope.OpenRuleErrors = function () {
        var RuleName = $scope.getRuleName(scope);
        var scope = getScopeByFileName("MainPage");
        if (scope) {
            scope.OpenRuleErrorsClick(false, RuleName);
        }
    };
    $scope.getRuleName = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        var RuleName = "";
        if (scope) {
            if (scope.objLogicalRule.dictAttributes != undefined) {
                RuleName = scope.objLogicalRule.dictAttributes.ID;
            }
            else {
                RuleName = scope.objLogicalRule.RuleID;
            }
        }
        return RuleName;
    };
    $scope.buildRule = function (isObjectRule) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.buildRule(isObjectRule);
        }
    };
    $scope.openRuleInExpression = function (isObjectRule) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.openRuleInExpression(isObjectRule);
        }
    };
    $scope.SelectVersion = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            $scope.objEffectiveDate = scope.objEffectiveDate;
        }
    };
    $scope.UpdateRule = function (effectiveDate) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.UpdateRule(effectiveDate);
        }
    };

    $scope.buildRuleClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            $scope.CanCreateObjectBasedScenario = scope.CanCreateObjectBasedScenario;
        }
    }

    $scope.ScenariosClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            $scope.HasRelatedParameterBasedScenario = scope.HasRelatedParameterBasedScenario;
            $scope.CanCreateObjectBasedScenario = scope.CanCreateObjectBasedScenario;
            $scope.HasRelatedXLBasedScenario = scope.HasRelatedXLBasedScenario;
            $scope.HasRelatedObjectBasedScenario = scope.HasRelatedObjectBasedScenario;
        }
    };
    $scope.AddNewScenario = function (scenarioType, flag) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.AddNewScenario(scenarioType, flag);
        }
    };
    $scope.NavigateToScenario = function (scenarioType) {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.NavigateToScenario(scenarioType);
        }
    };
    //#endregion


    //#region Fix all Node ids
    $scope.FixAllNodeIds = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.FixAllNodeIds();
        }
    }
    //#endregion
    //#region Scenario
    $scope.ScenarioRunAllTests = function (runType) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.RunAllTests) {
            scope.RunAllTests(runType);
        }
    };

    $scope.canCreateShowParameterScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            if ($rootScope.currentopenfile.file.FileType == 'ParameterScenario' || $rootScope.currentopenfile.file.FileType == 'ObjectScenario' || $rootScope.currentopenfile.file.FileType == 'ExcelScenario') {
                if (fileScope.IsObjectBasedScenario && fileScope.objEntityBasedScenario.SelectedStep.IsRunSelected == true) {
                    return true;
                }
            }
        }
    };

    $scope.NavigateToRule = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.NavigateToRule();
        }
    };

    $scope.RefreshScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.RefreshScenario();
        }
    };

    $scope.canShowNavigationImage = function (type) {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            if ($rootScope.currentopenfile.file.FileType == 'ParameterScenario' || $rootScope.currentopenfile.file.FileType == 'ObjectScenario' || $rootScope.currentopenfile.file.FileType == 'ExcelScenario') {
                if (fileScope.RuleType == type) {
                    return true;
                }
            }
        }
    };

    $scope.canShowAddParameterBasedScenario = function () {
        if ($rootScope.currentopenfile.file.FileType == 'ObjectScenario') {
            var fileScope = getCurrentFileScope();
            if (fileScope) {
                if (fileScope.objEntityBasedScenario && fileScope.objEntityBasedScenario.SelectedStep && fileScope.objEntityBasedScenario.SelectedStep.IsRunSelected) {
                    return true;
                }
            }
        }
        return false;
    };

    $scope.onCreateParameterScenario = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.onCreateParameterScenario();
        }
    };
    //#endregion

    //#region Forms
    $scope.OnOpenValidationRuleDetailsClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);

        if (scope && scope.OnOpenValidationRuleDetailsClick) {
            scope.OnOpenValidationRuleDetailsClick();
        }
    };

    $scope.OnAddPanelClick = function (action) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope) {
            if (action == "Panel") {
                scope.OnAddPanelClick(-1, false);
            }
            if (action == "Wizard") {
                scope.OnAddWizardStepClick(-1, false);
            }
        }
    };

    $scope.getHightlightAttribute = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && !scope.selectedDesignSource) {
            scope.getHightlightAttribute();
        }
    };

    $scope.OnSwitchView = function (action) {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope) {
            scope.OnSwitchView();
        }
    };

    $scope.isDeveloperView = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope) {
                return scope.isDeveloperView;
            }
        }
        else {
            return false;
        }
    };
    //#endregion

    //#region BPM
    $scope.deployBpmnMap = function (redeploy) {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.deployMap) {
                scope.deployMap(redeploy);
            }
        }
    };
    $scope.loadVersions = function () {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.loadVersions) {
                scope.loadVersions();
            }
        }
    };

    $scope.saveAsPng = function () {
        if ($rootScope.currentopenfile.file && $rootScope.currentopenfile.file.FileType == 'BPMN') {
            var curfileid = $rootScope.currentopenfile.file.FileName;
            var scope = getScopeByFileName(curfileid);
            if (scope && scope.deployMap) {
                scope.saveBPMNAsPng();
            }
        }
    }
    //#endregion

    //#region Correspondence
    $scope.openWordTemplateClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.openWordTemplateClick) {
            scope.openWordTemplateClick();
        }
    };

    $scope.onSynchroniseClick = function () {
        var curfileid = $rootScope.currentopenfile.file.FileName;
        var scope = getScopeByFileName(curfileid);
        if (scope && scope.onSynchroniseClick) {
            scope.onSynchroniseClick();
        }
    };

    //#endregion

    //#region Html Forms
    $scope.onRefreshClick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            if (!scope.isDirty) {
                scope.OnRefreshClick();
            } else {
                alert("Please save the file before refresh.");
            }
        }
    };

    $scope.onDisplayUnmapColumnclick = function () {
        var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
        if (scope) {
            scope.OnDisplayUnmapColumnclick();
        }
    };
    //#endregion


    //#region Save logic
    $scope.onsavefileclick = function (obj) {
        if (!obj.IsSavePending) {
            if ($scope.selectDesignSource() == true) {
                $scope.onsavefilexmlclick(obj);
                return;
            }
            var filetype = "";
            var filedetails;
            if (obj) {
                filetype = obj.file.FileType;
                filedetails = obj.file;
            }

            if (filetype == "ExcelScenario") {
                alert('Excel Scenario cannot be saved.');
            }
            else if (obj) {
                var curscope = getScopeByFileName(obj.file.FileName);
                if (curscope && (!curscope.canSaveFile || (curscope.canSaveFile && curscope.canSaveFile()))) {
                    if (curscope && curscope.BeforeSaveToFile) {
                        curscope.BeforeSaveToFile();
                    }

                    if (filetype == "NeoRuleSettings") {
                        var scopeobject = $scope.GetCurrentScopeObjectModel(filetype, "SettingsPage");
                    }
                    else {
                        var scopeobject = $scope.GetCurrentScopeObjectModel(filetype, obj.file.FileName);
                    }

                    if (scopeobject) {
                        $rootScope.SaveModelWithPackets(scopeobject, filedetails, filetype, false);
                    }
                }
            }
        }
    };

    $scope.GetCurrentScopeObjectModel = function (FileType, FileName) {
        var scope = getScopeByFileName(FileName);
        if (FileType == "NeoRuleSettings") {
            return scope.configureSettingsDetails;
        }
        else if (FileType == "LogicalRule" || FileType == "DecisionTable" || FileType == "ExcelMatrix") {
            return scope.objLogicalRule;
        }
        else if (FileType == "ParameterScenario" || FileType == "ObjectScenario" || FileType == "ExcelScenario" || FileType == "DatasourceScenario") {
            return scope.objEntityBasedScenario;
        }
        else if (FileType == "Entity") {
            return scope.objEntity;
        }
        else if (FileType == "RuleConstants") {
            return scope.objDummyConstants;
        }
        else if (FileType == "AuditLog") {
            return scope.AuditlogFiledata;
        }
        else if (FileType == "ProjectConfiguration") {
            return scope.projectconfigurationfiledata;
        }
        else if (FileType == "CustomSettings") {
            return scope.customsettingsfiledata;
        }
        else if (FileType == "Correspondence") {
            return scope.objCorrespondenceDetails;
        }
        else if (FileType == "Maintenance" || FileType == "Lookup" || FileType == "Wizard" || FileType == "UserControl" || FileType == "Tooltip") {
            var model = GetBaseModel(scope.FormModel);
            return model;
        }
        else if (FileType == "Report") {
            return scope.ReportModel;
        }
        else if (FileType == "InboundFile" || FileType == "OutboundFile") {
            return scope.objFile;
        }
        else if (FileType == "BPMN" || FileType == "BPMTemplate") {
            return scope.BPMNModel;
        }
        else if (FileType == "FormLinkMaintenance" || FileType == "FormLinkLookup" || FileType == "FormLinkWizard") {
            return scope.FormLinkModel;
        }
        else if (FileType == "WorkflowMap") {
            return scope.WorkflowMapModel;
        }
    };



    $rootScope.SaveModelWithPackets = function (scopeobject, filedetails, filetype, iscreatenew) {
        var objreturn1;
        if (typeof (scopeobject) == "object") {
            if (filetype == "DecisionTable") {
                objreturn1 = scopeobject;
            }
            else if (filetype == "BPMN" || filetype == "BPMTemplate") {
                objreturn1 = scopeobject;
            }
            else {
                objreturn1 = GetBaseModel(scopeobject);
            }
        }
        if (objreturn1 && !(objreturn1.Name || objreturn1.ID || objreturn1.RuleID)) { // Fixed Bug 10330:while opening any correspondence and hitting on save button , stack trace is displayed 
            return;
        }
        var strobj;
        var saveType;
        if (typeof (scopeobject) == "object") {
            strobj = JSON.stringify(objreturn1);
            saveType = "SaveModel";
        }
        else {
            strobj = scopeobject;
            saveType = "SaveXMLSource";
        }
        for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
            if ($rootScope.lstopenedfiles[i].file.FileName === filedetails.FileName) {
                $rootScope.lstopenedfiles[i].IsSavePending = true;
                break;
            }
        }
        $rootScope.IsLoading = true;
        //if (strobj.length < 32000) {
        if (typeof (scopeobject) == "object") {
            hubMain.server.saveModel(strobj, filedetails, true, iscreatenew);
        }
        else {
            hubMain.server.saveXMLString(strobj, filedetails, true);
        }
        //}
        //else {
        //    var strpacket = "";
        //    var lstDataPackets = [];
        //    var count = 0;
        //    for (var i = 0; i < strobj.length; i++) {
        //        count++;
        //        strpacket = strpacket + strobj[i];
        //        if (count == 32000) {
        //            count = 0;
        //            lstDataPackets.push(strpacket);
        //            strpacket = "";
        //        }
        //    }
        //    if (count != 0) {
        //        lstDataPackets.push(strpacket);
        //    }
        //    //SendChunkByIndex(0, lstDataPackets, filedetails, saveType);
        //    SendDataPacketsToServer(lstDataPackets, filedetails, saveType);
        //}
    };

    //var SendChunkByIndex = function (i, Base64Chunks, filedetails, saveType, aNodeID) {
    //    if (i < Base64Chunks.length) {
    //        var EOF = false;
    //        if (i == Base64Chunks.length - 1) {
    //            EOF = true;
    //        }
    //        hubMain.server.sendDataPackets(Base64Chunks[i], EOF, i, filedetails, saveType, aNodeID).done(function () {
    //            SendChunkByIndex(i + 1, Base64Chunks, filedetails, saveType, aNodeID);
    //        }).fail(function () {
    //            //error message
    //        });
    //    }
    //}

    var SendDataPacketsToServer = function (lstpackets, filedetails, saveType) {
       
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, saveType, null);
        }
    };

    $scope.receiveSavedModel = function (ErrorObject, strFileName) {
        if ($rootScope.currentopenfile) {
            var FileName = strFileName;
            var curscope = getScopeByFileName(FileName);
            if (curscope) {
                curscope.$evalAsync(function () {
                    curscope.isDirty = false;
                    if (curscope && curscope.AfterSaveToFile) {
                        curscope.AfterSaveToFile();
                    }
                    $rootScope.isFilesListUpdated = true;
                });
                //if (ErrorObject) {
                //    if (ErrorObject.currentfile) {
                //        if (curscope.currentfile.FileName.match("^rul")) {
                //            curscope.displayValidation(ErrorObject);
                //        }
                //    }
                //}

                if (curscope.currentfile && curscope.currentfile.FileName.match("^ent")) {
                    curscope.ValidateObject();
                }
            }
        }

        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file.FileType == "RuleConstants") {
            hubMain.server.reloadConstantFroValidateExpression().done(function (strerror) {
                $scope.UpdateUI(curscope);
                if (strerror) {
                    alert(strerror);
                }
            });
        }
        else {
            $scope.UpdateUI(curscope);
        }
    };

    $scope.receivePendingSaveModel = function (strFileName) {
        if (strFileName) {
            for (var i = 0; i < $rootScope.lstopenedfiles.length; i++) {
                if ($rootScope.lstopenedfiles[i].file.FileName === strFileName) {
                    $rootScope.lstopenedfiles[i].IsSavePending = false;
                    break;
                }
            }
        }
    }

    $scope.UpdateUI = function (curscope) {
        $rootScope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            if ($rootScope.IsTFSConfigured) { // changes made coz after saving file,tfs state was not coming properly.
                if ($rootScope.currentopenfile) {
                    $rootScope.currentopenfile.file.TFSState = "InEdit";
                    $rootScope.updateUserFileDetails($rootScope.currentopenfile.file);
                }

                // for updating the tfs chart
                $rootScope.IsTfsUpdated = true;
                $DashboardFactory.getTfsStatusData();
            }

            if (curscope && curscope.removeExtraFieldsDataInToMainModel) {
                curscope.removeExtraFieldsDataInToMainModel();
            }
            toastr.success("File saved sucessfully.");
        });
    }
    //#endregion

    //#region Save XML Source
    $scope.onsavefilexmlclick = function (obj) {
        if (obj) {
            var filetype = obj.file.FileType;
            var filedetails = obj.file;
            var curscope = getScopeByFileName(obj.file.FileName);
            if (filetype == "ExcelScenario") {
                alert('Excel Scenario cannot be saved.');
            }
            else {
                var scopexmlstring;
                if (curscope && curscope.editor) {
                    if (curscope.isSourceDirty) {
                        $rootScope.ClearUndoRedoListByFileName(curscope.currentfile.FileName);
                    }
                    scopexmlstring = curscope.editor.getValue();
                }
                if (scopexmlstring && scopexmlstring != "") {
                    $rootScope.SaveModelWithPackets(scopexmlstring, filedetails, filetype, false);
                }
            }
        }
    };
    //#endregion

    $scope.openCurrentEntity = function (entName) {
        var entFileName = entName;

        $.connection.hubMain.server.navigateToFile(entFileName, "").done(function (objfile) {
            $rootScope.openFile(objfile, undefined);
        });
    };

    $scope.validateFile = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.validateFileData) {
                $("#validation-btn").prop("disabled", true);
                scope.validateFileData(true);
            }
        }
    };

    $scope.addNoLockToQueries = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.addNoLockToQueries) {
                scope.addNoLockToQueries();
            }
        }
    };
    

    $scope.checkValidation = function () {
        var retVal = false;
        $scope.errorListObj = [];
        if ($Errors.validationListObj.length > 0 && $rootScope.currentopenfile) {
            angular.forEach($Errors.validationListObj, function (obj) {
                if (obj.FileName == $rootScope.currentopenfile.file.FileName) {
                    $scope.errorListObj = obj.errorList;
                    retVal = $scope.errorListObj.length > 0 ? true : false;
                }
            });
        }
        return retVal;
    };
    $scope.checkWarning = function () {
        var retVal = false;
        $scope.warningList = [];
        if ($Errors.validationListObj.length > 0 && $rootScope.currentopenfile) {
            angular.forEach($Errors.validationListObj, function (obj) {
                if (obj.FileName == $rootScope.currentopenfile.file.FileName) {
                    if (obj.warningList) {
                        $scope.warningList = obj.warningList;
                        retVal = $scope.warningList.length > 0 ? true : false;
                    }
                }
            });
        }
        return retVal;
    }
    $scope.showErrorList = function () {
        if ($Errors.validationListObj.length > 0) {
            if ($('#validation-error-list').length > 0) {
                $('#validation-error-list').toggle();
            }
        }
    };
    $scope.showWarnings = function () {
        if ($Errors.validationListObj.length > 0) {
            if ($('#validation-warning-list').length > 0) {
                $('#validation-warning-list').toggle();
            }
        }
    };
    $scope.navigateToError = function (errObj) {
        $('#validation-error-list').fadeOut();
        $('#validation-warning-list').fadeOut();
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope && scope.findParentAndChildObject) {
                scope.findParentAndChildObject(errObj);
            }
        }
    };
    $scope.toggleErrorList = function (obj) {
        if (obj.isExpand) {
            obj.isExpand = false;
        } else {
            obj.isExpand = true;
        }
    };

    //#region Load Details

    $scope.openLoadDetails = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenLoadDetailsDialog();
            }
        }
    };
    //#endregion


    //#region Workflow Refresh Map


    $scope.RefreshWorkflowMap = function () {
        var fileScope = getCurrentFileScope();
        if (fileScope) {
            fileScope.RefreshWorkflowMap();
        }
    };
    //#endregion

    $scope.CutCopyPasteClick = function (operation) {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (operation == "cut" || operation == "copy" || operation == "paste") {
                if (scope && scope.OnCopyCutPasteControlClick && !scope.selectedDesignSource) {
                    scope.OnCopyCutPasteControlClick(operation);
                }
            }
            else if ($rootScope.currentopenfile.file.FileType == "Lookup" || $rootScope.currentopenfile.file.FileType == "Maintenance" || $rootScope.currentopenfile.file.FileType == "Wizard" || $rootScope.currentopenfile.file.FileType == "Tooltip" || $rootScope.currentopenfile.file.FileType == "UserControl" || $rootScope.currentopenfile.file.FileType == "Report" || $rootScope.currentopenfile.file.FileType == "Correspondence") {
                if (operation == "InsertRowAbove" || operation == "InsertRowBelow" || operation == "InsertColumnLeft" || operation == "InsertColumnRight"
                    || operation == "MoveRowUp" || operation == "MoveRowDown" || operation == "MoveColumnLeft" || operation == "MoveColumnRight") {
                    if (scope && scope.OnRowColumnInsertMoveClick && !scope.selectedDesignSource) {
                        scope.OnRowColumnInsertMoveClick(operation);
                    }
                }
                else if (operation == "InsertCellLeft" || operation == "InsertCellRight") {
                    if (scope && scope.OnInsertCellClick && !scope.selectedDesignSource) {
                        scope.OnInsertCellClick(operation);
                    }
                }
            }
        }
    };



    //#region Centre Left

    $scope.openCentreLeft = function () {
        if ($rootScope.currentopenfile && $rootScope.currentopenfile.file) {
            var scope = getScopeByFileName($rootScope.currentopenfile.file.FileName);
            if (scope) {
                scope.OpenCentreLeftDialog();
            }
        }
    };

    //#endregion
}]);

$(document).ready(function () {
    $(document).on("keydown", function (e) {
        if (e.shiftKey && e.keyCode == $.ui.keyCode.TAB) {
            e.preventDefault();
            e.stopPropagation();
            if (e.shiftKey && $("#active-files").hasClass("hide")) {
                if (document.getElementsByClassName("ui-dialog").length == 0) {
                    openActiveFileDialog();
                }
            }
        }
        if (e.ctrlKey && e.keyCode == 90) {
            if (!(e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $("#btnUndo:not([disabled]):visible").click();
            }
            e.preventDefault();

        }
        else if (e.ctrlKey && e.keyCode == 89) {
            if (!(e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $("#btnRedo:not([disabled]):visible").click();
            }
            e.preventDefault();

        }
        else if (e.ctrlKey && e.keyCode == 83) {
            if ((e.target.localName == "input" && $(e.target).attr("type") == "text")) {
                $(e.target).blur();
            }
            $("#btnSave:not([disabled]):visible").click();
            return false;
        }
        //cut
        else if (e.ctrlKey && e.keyCode == 88) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("cut");
            }
        }
        //copy
        else if (e.ctrlKey && e.keyCode == 67) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("copy");
            }
            // e.preventDefault();
        }
        //paste
        else if (e.ctrlKey && e.keyCode == 86) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("paste");
            }
            //e.preventDefault();
            //return;
        }
        //Insert Row Above
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 65) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertRowAbove");
            }
            e.preventDefault();
        }
        //Insert Row Below
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 66) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertRowBelow");
            }
            e.preventDefault();
        }
        //Insert Column Left
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertColumnLeft");
            }
            e.preventDefault();
        }
        //Insert Column Right
        else if (e.ctrlKey && e.shiftKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertColumnRight");
            }
            e.preventDefault();
        }
        //Insert Cell Left
        else if (e.ctrlKey && e.altKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertCellLeft");
            }
            e.preventDefault();
        }
        //Insert Cell Right
        else if (e.ctrlKey && e.altKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("InsertCellRight");
            }
            e.preventDefault();
        }
        //Delete Cell
        else if (e.altKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteCellClick && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteCellClick();
                });
                e.preventDefault();
            }
        }
        //Move Row Up
        else if (e.ctrlKey && e.keyCode == 85) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveRowUp");
            }
            e.preventDefault();
        }
        //Move Row Down
        else if (e.ctrlKey && e.keyCode == 68) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveRowDown");
            }
            e.preventDefault();
        }
        //Move Column Left
        else if (e.ctrlKey && e.keyCode == 76) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveColumnLeft");
            }
            e.preventDefault();
        }
        //Move Column Right
        else if (e.ctrlKey && e.keyCode == 82) {
            var scope = getScopeByFileName("PageLayout");
            if (scope && scope.CutCopyPasteClick) {
                scope.CutCopyPasteClick("MoveColumnRight");
            }
            e.preventDefault();
        }
        //Delete Column
        else if (e.ctrlKey && e.shiftKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteColumn && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteColumn();
                });
                e.preventDefault();
            }
        }
        //Delete Row
        else if (e.ctrlKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.deleteRow && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.deleteRow();
                });
                e.preventDefault();
            }
        }
        //Clear Panel
        else if (e.shiftKey && e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.clearPanel && !scope.selectedDesignSource) {
                scope.$evalAsync(function () {
                    scope.clearPanel();
                });
                e.preventDefault();
            }
        }
        else if (e.keyCode == $.ui.keyCode.DELETE) {
            var scope = getCurrentFileScope();
            if (scope && scope.DeleteSelectedRowsAndColumns) {
                scope.DeleteSelectedRowsAndColumns();
            }
        }

    });
    $(document).on("keyup", function (e) {
        //    if (e.ctrlKey && e.keyCode == 83) {
        //        $("#btnSave").click();
        //    }
        if (!e.shiftKey && $("#active-files").hasClass("show")) {
            if ($("[ui-active-file-wrapper]").find("li.selected").length) {
                $("[ui-active-file-wrapper]").find("li.selected").trigger("click");
            } else {
                $("#active-files").removeClass("show");
                $("#active-files").addClass("hide");
            }
        }
    });
    //Prevent browser back button
    history.pushState(null, null, document.URL);
    window.addEventListener('popstate', function () {
        history.pushState(null, null, document.URL);
    });

    var openActiveFileDialog = function () {
        var element = $('body[ng-controller="MainController"]');
        if (element) {
            rootScope = angular.element(element).scope().$root;
            if (rootScope.IsLoading) {
                return;
            }
        }

        var domActiveFiles = $("#active-files");
        domActiveFiles.removeClass("hide");
        domActiveFiles.addClass("show");
        domActiveFiles.find("input").focus();
        domActiveFiles.find('li.selected').removeClass('selected');
        if (domActiveFiles.find("li:nth-child(2)").length) {
            domActiveFiles.find("li:nth-child(2)").addClass('selected');
        } else {
            domActiveFiles.find("li:nth-child(1)").addClass('selected');
        }
        domActiveFiles.find("input").trigger("keydown");
    };
});

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
app.controller("SearchFilesController", ["$scope", "hubcontext", "$rootScope", "$filter", "$searchQuerybuilder", "$ObjectsSearch", "$resourceFactory", function ($scope, hubcontext, $rootScope, $filter, $searchQuerybuilder, $ObjectsSearch, $resourceFactory) {

    function SearchFilesController() {
        $scope.lstactiveSearchPanelIDs = ['mainSearchCommonCriteriaPanel'];
        $scope.objDirFunctions = {};
        $scope.SelectedFileType = "Entity";
        $scope.InitAdvanceSearch();
        InitCustomSearch();
        hubMain.server.prepareFolderPath("");
        $('[advance-search-wrapper] input').on("keydown", function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                $scope.$evalAsync(function () {
                    $scope.GetAllFilesInfo();
                });
                return false;
            }
        });
    }

    function InitCustomSearch() {
        $scope.objEntityExtraFields = {
            Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
        };
    }

    $scope.InitAdvanceSearch = function () {
        // All File type List       
        $scope.lstFileTypes = JSON.parse(JSON.stringify($resourceFactory.getFileTypeList()));
        $scope.lstTfsStatus = JSON.parse(JSON.stringify($resourceFactory.getTfsStatusList()));
        // Search Criteria Object
        $scope.searchCriteria = {
            fileName: "",
            containingText: "",
            location: "",
            isCreatedDate: false,
            isModifiedDate: false,
            fromDate: "",
            toDate: "",
            fileType: [],
            TfsStatus: [],
            QuerySearchList: []
        };
        $scope.selectedDate = {
            typeOfDate: ''
        };
        $scope.queryBuilder = $searchQuerybuilder;
        $scope.ResultantFiles = [];
        $scope.ResultantExtraFieldFiles = [];
        // to make autofocus work in IE - focus should be on filename search
        setTimeout(function () {
            $('[autofocus]:not(:focus)').eq(0).focus();
            $('[advance-search-wrapper] [st-search]:visible').val(null); // reset filter textbox
            var HTMLElementSelectAll = document.querySelectorAll("[advance-search-wrapper] input[select-all-toggle]");
            if (HTMLElementSelectAll.length > 0) {
                for (var i = 0; i < HTMLElementSelectAll.length; i++) {
                    HTMLElementSelectAll[i].checked = false;
                }
            }
        });
    }

    // #region advance search
    $scope.getFolderPathIntellisense = function (event) {
        var HTMLElementLocationSearch = null;
        if (event.type == "click") {
            HTMLElementLocationSearch = $(event.target).prevAll("input[type='text']");
        }
        else {
            HTMLElementLocationSearch = $(event.target);
        }
        if (HTMLElementLocationSearch) {
            $(HTMLElementLocationSearch).autocomplete({
                minLength: 0,
                appendTo: "#dvIntellisense",
                open: function (event1, ui) {
                    $("#dvIntellisense > ul").css({
                        width: 'auto',
                        height: 'auto',
                        overflow: 'auto',
                        maxWidth: '300px',
                        maxHeight: "300px"
                    });
                    setIntellisensePosition(event1);
                    $(".tab-content").find(".settings-tab-content").css("pointer-events", "none");
                },
                //delay: 100,
                source: function (request, response) {
                    hubMain.server.getFolderPathIntellisense(request.term.trim(), "", true).done(function (data) {
                        if (data != "") {
                            $scope.responseParsed = parsePathIntellisenseResult(data);
                            response($scope.responseParsed);
                        }
                        else {
                            $scope.responseParsed = [];
                            response($scope.responseParsed);
                        }
                    });
                },
                search: function (event, ui) {
                    $scope.searchCriteria.location = event.target.value;
                },
                select: function (event, ui) {
                    $scope.searchCriteria.location = ui.item.value;
                },
                focus: function (event, ui) {
                    $(".ui-autocomplete > li").attr("title", ui.item.label);
                }
            });
            if (event.type == "click") {
                $(HTMLElementLocationSearch).focus();
                if ($(HTMLElementLocationSearch).data('ui-autocomplete')) {
                    $(HTMLElementLocationSearch).autocomplete("search", $(HTMLElementLocationSearch).val());
                }
            }
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(HTMLElementLocationSearch).data('ui-autocomplete')) {
                $(HTMLElementLocationSearch).autocomplete("search", $(HTMLElementLocationSearch).val());
                event.preventDefault();
            }
        }
    };

    $scope.SelectDateType = function () {
        ComponentsPickers.init();
        if ($scope.selectedDate.typeOfDate == "create") {
            $scope.searchCriteria.isCreatedDate = true;
            $scope.searchCriteria.isModifiedDate = false;
        } else {
            $scope.searchCriteria.isModifiedDate = true;
            $scope.searchCriteria.isCreatedDate = false;
        }
    };

    function updatecheckboxFilterList(objchanged, lststrFiler) {
        if (objchanged) {
            var intCurrentIndex = lststrFiler.indexOf(objchanged.value);
            // if obj is not in the list add it
            if (objchanged.selected && (intCurrentIndex < 0)) {
                lststrFiler.push(objchanged.value);
            }
            else if (!objchanged.selected && intCurrentIndex >= 0) {
                lststrFiler.splice(intCurrentIndex, 1);
            }
        }
    }

    $scope.onchangecheckboxlst = function (lstChanged, lststrFiler, objchanged) {
        var intLenlstChanged = lstChanged.length;
        for (var i = 0; i < intLenlstChanged; i++) {
            if (objchanged.value === lstChanged[i].value) {
                updatecheckboxFilterList(objchanged, lststrFiler);
                break;
            }
        }
    };

    $scope.SelectAllFiles = function (lstchange, lststrFiler, e) {
        var intLenChangelst = lstchange.length;
        for (var i = 0; i < intLenChangelst; i++) {
            lstchange[i].selected = e.target.checked;
            updatecheckboxFilterList(lstchange[i], lststrFiler);
        }
        if (e) {
            e.stopImmediatePropagation();
        }
    };

    $scope.GetAllFilesInfo = function (exportToExcel) {
        if (!exportToExcel) {
            exportToExcel = false;
        }
        function ValidateSearchCriteria() {
            if ($scope.searchCriteria.fileType.length == 0) {
                toastr.info("Please select at least one file type");
                return false;
            }
            if ($scope.searchCriteria.isCreatedDate && !$scope.searchCriteria.fromDate) {
                toastr.info("Please select From Date");
                return false;
            }
            if ($scope.searchCriteria.isModifiedDate && !$scope.searchCriteria.fromDate) {
                toastr.info("Please select From Date");
                return false;
            }
            else if (new Date() < Date.parse($scope.searchCriteria.fromDate)) {
                toastr.info("From Date should be less than Or equal to current Date");
                return false;
            }
            else if ($scope.searchCriteria.fromDate) {
                if (!$scope.searchCriteria.toDate) {
                    $scope.searchCriteria.toDate = $filter('date')(new Date(), "MM/dd/yyyy");
                }
            }
            else if ($scope.searchCriteria.toDate) {
                if (!$scope.searchCriteria.fromDate) {
                    toastr.info("Please select From Date");
                    return false;
                }
            }
            else if ($scope.searchCriteria.toDate && Date.parse($scope.searchCriteria.fromDate) > Date.parse($scope.searchCriteria.toDate)) {
                toastr.info("From Date should be greater than To Date");
                return false;
            }
            return true;
        };
        if (ValidateSearchCriteria()) {
            $rootScope.IsLoading = true;
            $scope.searchCriteria.fileName = $scope.searchCriteria.fileName.trim();
            // containing text search - smart search (query builder logic)
            var objSearch = angular.copy($scope.searchCriteria);
            if (objSearch.containingText) {
                var ObjQuery = angular.copy($ObjectsSearch.QueryObj);
                ObjQuery.Operator = "Contains";
                ObjQuery.AttributeValue = objSearch.containingText;
                objSearch.QuerySearchList.push(ObjQuery);
            }
            hubcontext.hubSearch.server.getFilesBySearchCriteria(objSearch, exportToExcel).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                    $scope.ResultantFiles = data;
                });
            });
        }
    };

    $scope.advanceSearchOpenfile = function (file) {
        // #region smart navigation logic
        if (['WorkflowMap', 'ParameterScenario', 'ObjectScenario', 'ExcelScenario', 'BPMN', 'ExcelMatrix', 'FormLinkLookup', 'FormLinkMaintenance', 'FormLinkWizard'].indexOf(file.FileType) < 0) {
            // set containing text filters if applied - for smart navigation
            var lstQueryBuilder = angular.copy($scope.searchCriteria.QuerySearchList);
            if ($scope.searchCriteria.containingText) {
                var ObjQuery = angular.copy($ObjectsSearch.QueryObj);
                ObjQuery.Operator = "Contains";
                ObjQuery.operatorList = $searchQuerybuilder.getOperatorList();
                ObjQuery.AttributeValue = $scope.searchCriteria.containingText;
                lstQueryBuilder.push(ObjQuery);
            }
            $scope.queryBuilder.setGlobalQueryBuilderFilters(lstQueryBuilder);
        }
        // #endregion       
        $rootScope.openFile(file);
    };

    $scope.togglePanel = function (panelID) {
        var indexActive = $scope.lstactiveSearchPanelIDs.indexOf(panelID);
        if (indexActive >= 0) {
            $scope.lstactiveSearchPanelIDs.splice(indexActive, 1);
        }
        else {
            $scope.lstactiveSearchPanelIDs.push(panelID);
        }
    };
    // #endregion

    //#region Extra field search

    $scope.searchExtraFields = function () {
        if ($scope.objDirFunctions.getExtraFieldData) {
            var extraFieldsData = $scope.objDirFunctions.getExtraFieldData();
            if (extraFieldsData && extraFieldsData.length > 0) {
                hubcontext.hubSearch.server.getExtraFieldFilesBySearchCriteria(extraFieldsData, $scope.SelectedFileType).done(function (data) {
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                        $scope.ResultantExtraFieldFiles = data;
                    });
                });
            }
        }
    };

    $scope.onFileTypeChange = function (fileType) {
        if (fileType) {
            $scope.ResultantExtraFieldFiles = [];
        }
    };

    $scope.clearExtraFieldsList = function () {
        if ($scope.objDirFunctions.resetExtraField) {
            InitCustomSearch();
            $scope.objDirFunctions.resetExtraField();
            $scope.ResultantExtraFieldFiles = [];
        }
    };
    //#endregion

    // constructor call
    SearchFilesController();

    $scope.exportToExcel = function (searchResults) {
        $rootScope.IsLoading = true;
        hubcontext.hubSearch.server.exportSearchResultsToExcel(searchResults).done(function (filePath) {
            $rootScope.IsLoading = false;
            if (filePath) {
                MessageBox("Search Exported", "Search results exported to:\n" + filePath, false, null, { width: 550 });
            }
        });
    };
}]);


app.controller('DashboardController', ['$scope', '$rootScope', 'hubcontext', '$DashboardFactory', '$Chart', '$cookies', 'ConfigurationFactory', '$interval', '$http', function ($scope, $rootScope, hubcontext, $DashboardFactory, $Chart, $cookies, ConfigurationFactory, $interval, $http) {
    var userCookie = $cookies.getObject("UserDetails");
    $scope.init = function () {
        var filedata = $DashboardFactory.getFileTypeData();
        $scope.dashboardData = $DashboardFactory.getData;
        $rootScope.IsTFSConfigured = false;
        $rootScope.isConfigureSettingsVisible == false;
        $rootScope.currentopenfile = undefined;
        //Below code is commented for demo purpose as we are not showing workflow maps in that. 
        //Also we don't have any visible setting for the same in project settings.
        var lobjLastProjectDetail = ConfigurationFactory.getLastProjectDetails();
        if (lobjLastProjectDetail && lobjLastProjectDetail.IsWorkFlow) {
            $rootScope.IsWorkFlowMap = lobjLastProjectDetail.IsWorkFlow;
        }
        if (lobjLastProjectDetail && lobjLastProjectDetail.IsTFS) {
            $rootScope.IsTFSConfigured = true;
            var tfsdata = $DashboardFactory.getTfsStatusData();
            // initailly tfsdata will be null in factory
            if (tfsdata.length > 0 && !$rootScope.IsTfsUpdated) {
                $Chart.drawPieChart(tfsdata, "TfsChart", "Name", "Count");
                $(window).resize(function () {
                    $Chart.drawPieChart(tfsdata, "TfsChart", "Name", "Count");
                });
            }
            else {
                var promise = $interval(checkTfsData, 100);
                function checkTfsData() {
                    // normal flow when no new tfs action is performed just draw from the data in the factory - ex for the first time
                    if (!$rootScope.IsTfsUpdated) {
                        tfsdata = $DashboardFactory.getTfsStatusData();
                        if (tfsdata.length > 0) {
                            $interval.cancel(promise);
                            if ($('#TfsChart')[0] && $('#TfsChart').html().trim().length == 0) {
                                $Chart.drawPieChart(tfsdata, "TfsChart", "Name", "Count");
                                $(window).resize(function () {
                                    $Chart.drawPieChart(tfsdata, "TfsChart", "Name", "Count");
                                });
                            }
                        }
                    }
                    else if ($rootScope.IsTfsUpdated) {
                        $interval.cancel(promise);
                    }
                }
            }
        }
        if (filedata.length > 0 && !$rootScope.IsFilesAddedDelete) {
            $Chart.drawStackedBarChart(filedata, "mainChart", "ParentType", "ChildType");
            $(window).resize(function () {
                $Chart.drawStackedBarChart(filedata, "mainChart", "ParentType", "ChildType");
            });
        }
        else {
            // normal flow when no files are updated and factory is null - just draw from the data in the factory - ex for the first time
            var promise1 = $interval(checkTypeChartData, 100);
            function checkTypeChartData() {
                if (!$rootScope.IsFilesAddedDelete) {
                    filedata = $DashboardFactory.getFileTypeData();
                    if (filedata.length > 0) {
                        $interval.cancel(promise1);
                        if ($('#mainChart')[0] && $('#mainChart').html().trim().length == 0) {
                            $Chart.drawStackedBarChart(filedata, "mainChart", "ParentType", "ChildType");
                            $(window).resize(function () {
                                $Chart.drawStackedBarChart(filedata, "mainChart", "ParentType", "ChildType");
                            });
                        } 
                    }
                }
                else {
                    $interval.cancel(promise1);
                }
            }
        }
        $scope.updateUserfiles();
    };

    $scope.$on("updateUserFiles", function () {
        $scope.updateUserfiles();
    });

    $scope.updateUserfiles = function () {
        // fetch user files from db for the first time only othewise go to factory directly 
        if ($rootScope.IsProjectInitialLoad) {
            // for checking if cookie is present
            if (userCookie && JSON.parse(userCookie).UserID != null) {
                var filesdataPromise = $DashboardFactory.getUserFiles(JSON.parse(userCookie).UserID, ConfigurationFactory.getLastProjectDetails().ProjectId);
                filesdataPromise.then(function (result) {
                    if (result && result.files) {
                        // this checks for user files are at valid path -- if not remove from the list
                        hubcontext.hubMain.server.getFilePathValid(result.files).done(function (data) {
                            $scope.objFiles = {};
                            $scope.$evalAsync(function () {
                                $scope.objFiles.pinnedFiles = data[0];
                                $scope.objFiles.recentFiles = data[1];
                            });
                            $DashboardFactory.setPinnedFilesData(data[0]);
                            $DashboardFactory.setRecentFilesData(data[1]);
                            // if some invalid file is found call server and delete invalid files - update factory list
                            if (result.files.pinnedFiles.length != data[0].length || result.files.recentFiles.length != data[1].length) {
                                var deletepinned = result.files.pinnedFiles.difference(data[0], "FileName");
                                var deleterecent = result.files.recentFiles.difference(data[1], "FileName");
                                var invalidFiles = $DashboardFactory.deleteUserFiles({ pinnedFiles: deletepinned, recentFiles: deleterecent });
                                invalidFiles.then(function (result) {
                                    if (result) {
                                        toastr.info((deletepinned.length + deleterecent.length) + " invalid file(s) removed from recent and pinned files list", { timeOut: 3500 });
                                    }
                                });
                            }
                        });
                    }
                    else {
                        $scope.$evalAsync(function () {
                            $scope.objFiles = {}; // make it dirty for loading box
                        });
                    }
                });
            }
            $rootScope.IsProjectInitialLoad = false;
        }
            // fetch data from factory 
        else {
            $scope.$evalAsync(function () {
                $scope.objFiles = $DashboardFactory.getUserFilesData();
            });

        }
    };

    $scope.init();

    $scope.unPinUserFile = function (File) {
        if (userCookie) {
            var filesdataPromise = $DashboardFactory.postUserFiles(File, JSON.parse(userCookie).UserID, "Pin");
            filesdataPromise.then(function (result) {
                if (result) {
                    var deleteIndex = $scope.objFiles.pinnedFiles.indexOf(File);
                    if (deleteIndex > -1) {
                        $scope.objFiles.pinnedFiles.splice(deleteIndex, 1);
                        $DashboardFactory.setPinnedFilesData($scope.objFiles.pinnedFiles);
                        toastr.success("File unpinned successfully");
                    }
                }
            });
        }
    };
    // Redraw based on the new size whenever the browser window is resized.
    //window.addEventListener("resize", mainDashboard.drawVChart);
}]);
app.factory('$DashboardFactory', ['hubcontext', '$http', '$rootScope', '$Chart', 'ConfigurationFactory', '$timeout', function (hubcontext, $http, $rootScope, $Chart, ConfigurationFactory, $timeout) {
    var item = { fileTypeChart: [], TfsStatusChart: [], pinnedFiles: [], recentFiles: [], LegendData: undefined };
    if (hubcontext.hubMain) {
        hubcontext.hubMain.client.setDashboardChartData = function (data, datalegend) {
            if (data.length > 0 && datalegend.length > 0) {
                // IE is not reflecting model in the controller if change is done in factory
                $timeout(function () {

                    item.fileTypeChart = JSON.parse(data);


                    item.LegendData = JSON.parse(datalegend);
                    if ($('#mainChart').length == 1 && $rootScope.IsFilesAddedDelete) {
                        $rootScope.IsFilesAddedDelete = false;
                        $Chart.drawStackedBarChart(item.fileTypeChart, "mainChart", "ParentType", "ChildType");
                        $(window).resize(function () {
                            $Chart.drawStackedBarChart(item.fileTypeChart, "mainChart", "ParentType", "ChildType");
                        });
                    }
                }, 0);
            }
        };
        hubcontext.hubMain.client.setDashboardTfsChartData = function (data) {
            if (data.length > 0) {
                item.TfsStatusChart = JSON.parse(data);
                if ($('#TfsChart').length == 1 && $rootScope.IsTfsUpdated) {
                    $rootScope.IsTfsUpdated = false;
                    $Chart.drawPieChart(item.TfsStatusChart, "TfsChart", "Name", "Count");
                    $(window).resize(function () {
                        $Chart.drawPieChart(item.TfsStatusChart, "TfsChart", "Name", "Count");
                    });
                }
            }
        };
    }
    return {
        getFileTypeData: function () {
            if ($rootScope.IsFilesAddedDelete) hubcontext.hubMain.server.getDashboardChartData();
            return item.fileTypeChart;
        },
        getData: item,
        getTfsStatusData: function () {
            if ($rootScope.IsTfsUpdated) hubcontext.hubMain.server.getDashboardTfsChartData();
            return item.TfsStatusChart;
        },
        getUserFilesData: function () {
            return { pinnedFiles: item.pinnedFiles, recentFiles: item.recentFiles };
        },
        getPinnedFilesData: function () {
            return item.pinnedFiles;
        },
        getRecentFilesData: function () {
            return item.recentFiles;
        },
        setPinnedFilesData: function (pinnedFiles) {
            item.pinnedFiles = pinnedFiles;
        },
        setRecentFilesData: function (recentFiles) {
            item.recentFiles = recentFiles;
        },
        getUserFiles: function (userId, project_id) {
            return $http({
                method: 'GET',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/DashBoard/getUserFiles?userId=" + userId + "&project_id=" + project_id
            }).then(function successCallback(response) {
                if (!response.data.IsException) {
                    item.pinnedFiles = response.data.pinnedFiles ? response.data.pinnedFiles : [];
                    item.recentFiles = response.data.recentFiles ? response.data.recentFiles : [];
                    return { files: { pinnedFiles: item.pinnedFiles, recentFiles: item.recentFiles } };
                }
            }, function errorCallback(response) {
                $rootScope.showExceptionDetails(response.data);
            });
        },
        postUserFiles: function (File, userId, action) {
            // remove userid from here - will take it from session in the controller
            var obj = {
                UserId: userId,
                ProjectId: ConfigurationFactory.getLastProjectDetails().ProjectId,
                FileName: File.FileName,
                FilePath: File.FilePath,
                FileType: File.FileType,
                UserTypeFile: action
            };
            return $http({
                method: 'POST',
                data: obj,
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/UserAction/PostUserFiles"
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
                $rootScope.showExceptionDetails(response.data);
            });
        },
        updateUserFiles: function (FileList) {
            // remove userid from here - will take it from session in the controller           
            return $http({
                method: 'POST',
                data: FileList,
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/UserAction/UpdateUserFiles"
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
                $rootScope.showExceptionDetails(response.data);
            });
        },
        deleteUserFiles: function (FileList) {
            // remove userid from here - will take it from session in the controller           
            return $http({
                method: 'POST',
                data: FileList,
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/UserAction/DeleteUserFiles"
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
                $rootScope.showExceptionDetails(response.data);
            });
        }
    };
}]);

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