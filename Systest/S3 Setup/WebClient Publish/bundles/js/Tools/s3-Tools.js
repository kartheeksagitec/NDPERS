app.controller("configureSettingsController", ['$scope', '$http', 'ngDialog', 'ConfigurationFactory', 'hubcontext', '$rootScope', '$cookies', '$EntityIntellisenseFactory', '$timeout', function ($scope, $http, ngDialog, ConfigurationFactory, hubcontext, $rootScope, $cookies, $EntityIntellisenseFactory, $timeout) {
    $scope.showTfsDetails = false;
    $scope.showAddPortalDetailsTab = false;
    $scope.showPoratlDetailsTab = false;
    $scope.lstTempMappedUser = [];
    $scope.lstMappedUser = [];
    $scope.isSavePending = false;
    $scope.selectedConfigureTab = "ProjectSettings";
    var userCookie = $cookies.getObject("UserDetails");
    var currentUser = JSON.parse(userCookie);
    if (currentUser.UserType == "owner") {
        $scope.ShowUserTab = true;
    }
    else {
        $scope.ShowUserTab = false;
    }

    if (!$scope.configureSettingsDetails) {
        $rootScope.IsLoading = true;
        $http({
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/ConfigureSetting/GetConfigurationDetails?userId=" + currentUser.UserID
        }).then(function successCallback(response) {
            $rootScope.IsLoading = false;
            $scope.configureSettingsDetails = response.data[0];
            if ($scope.configureSettingsDetails && $scope.configureSettingsDetails.length > 0) {
                $scope.configureSettingsDetails.ClientID = $scope.configureSettingsDetails[0].ClientId;
            }
            if (response.data[1] && response.data[1].length > 0) {
                $scope.lstMappedUser = response.data[1];
            } else {
                $scope.lstMappedUser = [];
            }
            $scope.lstUsers = response.data[2];
            $scope.objClientInfo = response.data[3];
            var lastProject = ConfigurationFactory.getLastProjectDetails();
            if (lastProject && lastProject.ProjectName && lastProject.ProjectName != "") {
                for (var i = 0; i < $scope.configureSettingsDetails.length; i++) {
                    if (lastProject.ProjectId == $scope.configureSettingsDetails[i].ProjectId && lastProject.ProjectName == $scope.configureSettingsDetails[i].ProjectName) {
                        $scope.showProjectDetails($scope.configureSettingsDetails[i]);
                    }
                }
            }
            $scope.getActiveDirectoryMappedUser(currentUser.UserID)
        },
            function errorCallback(response) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(response.data);
            });
    }

    $scope.getActiveDirectoryMappedUser = function (aintUserID) {
        $http({
            method: 'POST',
            data: { userID: currentUser.UserID },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/ConfigureSetting/GetMappedADUser"
        }).then(function successCallback(response) {
            $scope.lstAdMappedUser = response.data;
        },
            function errorCallback(response) {
                $rootScope.showExceptionDetails(response.data);
            });
    }


    $scope.AddDBType = function () {
        var newQueryTypeScope = $scope.$new();
        newQueryTypeScope.lstDBTypes = [];
        var objOracle = { ID: "Oracle", IsSelected: "False", Attribute: "sfwOracle" };
        newQueryTypeScope.isValidQueryType = function (strDbType) {
            var isValid = true;
            for (var j = 0; j < $scope.selectedProject.lstQueryTypes.length; j++) {
                if ($scope.selectedProject.lstQueryTypes[j].ID == strDbType) {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
        newQueryTypeScope.lstDBTypes.push(objOracle);
        newQueryTypeScope.onAddQueryTypeOkClick = function () {
            for (var i = 0; i < newQueryTypeScope.lstDBTypes.length; i++) {
                if (newQueryTypeScope.lstDBTypes[i].IsSelected == "True") {
                    var isValidType = newQueryTypeScope.isValidQueryType(newQueryTypeScope.lstDBTypes[i].ID);
                    if (isValidType && $scope.selectedProject && $scope.selectedProject.lstQueryTypes) {
                        var objDBType = { ID: newQueryTypeScope.lstDBTypes[i].ID, Attribute: newQueryTypeScope.lstDBTypes[i].Attribute };
                        $rootScope.PushItem(objDBType, $scope.selectedProject.lstQueryTypes, "");
                    }
                }
            }
            newQueryTypeScope.onCancelClick();
        }
        newQueryTypeScope.dialogOptions = { height: 300, width: 350 };
        newQueryTypeScope.addQuerydialog = $rootScope.showDialog(newQueryTypeScope, "Add Query Type", "Tools/views/AddQueryType.html", newQueryTypeScope.dialogOptions);
        newQueryTypeScope.onCancelClick = function () {
            newQueryTypeScope.addQuerydialog.close();
        }
    };

    $scope.selectDBType = function (objDBType) {
        $scope.selectedProject.selectedDBType = objDBType;
    };

    $scope.DeleteDBType = function () {
        if ($scope.selectedProject.selectedDBType) {
            for (var i = 1; i < $scope.selectedProject.lstQueryTypes.length; i++) {
                if ($scope.selectedProject.lstQueryTypes[i] == $scope.selectedProject.selectedDBType) {
                    $rootScope.DeleteItem($scope.selectedProject.lstQueryTypes[i], $scope.selectedProject.lstQueryTypes, "");
                    $scope.selectedProject.selectedDBType = "";
                    break;
                }
            }
        }
    };

    $scope.concatDBTypesforAllProjects = function (lstProjectConfigDetails) {
        if (lstProjectConfigDetails) {
            for (var i = 0; i < lstProjectConfigDetails.length; i++) {
                if (lstProjectConfigDetails[i].lstQueryTypes && lstProjectConfigDetails[i].lstQueryTypes.length > 0) {
                    var strQueryTypes = $scope.getStrQueryTypes(lstProjectConfigDetails[i].lstQueryTypes);
                    lstProjectConfigDetails[i].QueryTypes = strQueryTypes;
                }
            }
        }
    }

    $scope.getStrQueryTypes = function (lstQueryTypes) {
        if (lstQueryTypes && lstQueryTypes.length > 0) {
            var lstDBTypes = [];
            var strQueryTypes = "";
            for (var i = 0; i < lstQueryTypes.length; i++) {
                if (lstQueryTypes[i].ID && lstQueryTypes[i].ID.trim() != "" && lstQueryTypes[i].Attribute && lstQueryTypes[i].Attribute.trim() != "") {
                    var dbType = lstQueryTypes[i].ID.trim() + ":" + lstQueryTypes[i].Attribute.trim();
                    lstDBTypes.push(dbType);
                }
            }
        }
        strQueryTypes = lstDBTypes.join(';');
        return strQueryTypes;
    };

    $scope.showProjectDetails = function (project) {
        if (!$scope.selectedProject || !$scope.selectedProject.ProjectId || $scope.selectedProject.ProjectId != project.ProjectId) {
            if ($scope.selectedProject) {
                for (var i = 0; i < $scope.configureSettingsDetails.length; i++) {
                    if ($scope.selectedProject.ProjectId == $scope.configureSettingsDetails[i].ProjectId && $scope.selectedProject.ProjectName == $scope.configureSettingsDetails[i].ProjectName) {
                        $scope.configureSettingsDetails[i] = $scope.selectedProject;
                    }
                }
            }
            $scope.selectedProject = project;
            $scope.CheckValidParentEntity();
            $scope.lstAssemblyNames = [];

            $scope.selectedProject.selectedDBType = "";
            if (!$scope.selectedProject.QueryTypes) {
                $scope.selectedProject.QueryTypes = "SQL:sfwSql";
            }

            if (!$scope.selectedProject.lstQueryTypes) {
                $scope.selectedProject.lstQueryTypes = [];
                $scope.selectedProject.lstQueryTypes = getlstQueryTypes($scope.selectedProject.QueryTypes);
            }

            if (project.AssemblyNames != "") {
                var lst = project.AssemblyNames.split(";");
                for (var i = 0; i < lst.length; i++) {
                    if (lst[i] != "" && i != lst.length - 1) {
                        $scope.lstAssemblyNames.push({ isCheck: true, Name: lst[i] });
                    }
                }
            }
            if (project.DataObjectProjectLocation && project.DataObjectProjectLocation == project.BusinessObjectProjectLocation) {
                $scope.sameAsBussinesObj = true;
            }
            if (project.BaseDirectory && project.BaseDirectory != "") {
                hubcontext.hubMain.server.prepareFolderPath(project.BaseDirectory);
            }

            if ($scope.selectedProject.IsHtxPortalTableExists) {
                $scope.showPoratlDetailsTab = $scope.selectedProject.IsHtxPortalTableExists;
                if (angular.isArray($scope.selectedProject.htxPortals) && $scope.selectedProject.htxPortals.length > 0) {
                    $scope.showPoratlDetails($scope.selectedProject.htxPortals[0]);
                    $scope.showAddPortalDetailsTab = true;
                } else {
                    $scope.showPoratlDetails(null);
                    $scope.showAddPortalDetailsTab = false;
                }
            } else {
                $scope.showPoratlDetailsTab = false;
            }
            if ($scope.showPoratlDetailsTab && $scope.ShowUserTab && $rootScope.openFromHtx) {
                $scope.selectedConfigureTab = 'HTXSettings'
            }
            delete $rootScope.openFromHtx;
        }
    };
    $scope.checkedUnchecked = function (project) {
        if (project && project.DataObjectProjectLocation && project.DataObjectProjectLocation.trim() == project.BusinessObjectProjectLocation.trim()) {
            $scope.sameAsBussinesObj = true;
        }
        else {
            $scope.sameAsBussinesObj = false;
        }
        return $scope.IsDisable;
    };
    $scope.IsEnableParentEntity = function () {
        if ($scope.selectedProject) {
            var lastProject = ConfigurationFactory.getLastProjectDetails();
            if (lastProject && lastProject.ProjectId && lastProject.ProjectId != "" && $scope.selectedProject.ProjectId == lastProject.ProjectId) {
                return true;
            }
        }
        else {
            return false;
        }
    };
    //$scope.setNewProjectName = function () {
    //    var count = 0;
    //    if ($scope.configureSettingsDetails) {
    //        for (i = 0; i < $scope.configureSettingsDetails.length; i++) {
    //            if ("test" == $scope.configureSettingsDetails[i].ProjectName.substring(0, 4).toLowerCase()) {
    //                count++;
    //            }
    //        }
    //        if (count == 0) {
    //            return "Test";
    //        }
    //        else {
    //            return "Test" + count;
    //        }
    //    }
    //    else {
    //        return "Test";
    //    }
    //}
    $scope.addProject = function () {
        var newScope = $scope.$new();
        //newScope.newprojectName = $scope.setNewProjectName();
        var dialog = $rootScope.showDialog(newScope, "Add New Project", "Tools/views/AddNewProject.html");
        newScope.addProjectOkClick = function (newprojectName) {
            if (newprojectName) {
                $scope.lstAssemblyNames = [];
                var keyNames = ["ProjectID", "BaseDirectory", "XmlFileLocation", "ScenarioXmlFileLocation", "ApplicationExecutablesLocation", "Solution",
                    "BusinessObjectLocation", , "DataObjectLocation", "AssemblyNames", "BusinessObjectProjectLocation", "DataObjectProjectLocation",
                    "DefaultParentEntity", "CorrespondenceTemplatesLocation", "ReportsLocation", "HtmlTemplatesLocation", "BPMTemplatesLocation",
                    "IsTFS", "IsRemoteLogin", "TFSUrl", "TFSSourceLocation", "TFSScenarioSourceLocation", "TFPath",
                    "TfsLocalPath", "tfptPath", "CheckPendingChanges", "Website", "IsWorkFlow", "CorrespondenceTestsLocation"];
                var objNew = {};
                objNew.ProjectName = newprojectName;
                for (var i = 0; i < keyNames.length; i++) {
                    if (keyNames[i] == "XmlFileLocation") {
                        objNew[keyNames[i]] = "XML";
                    }
                    else if (keyNames[i] == "ScenarioXmlFileLocation") {
                        objNew[keyNames[i]] = "XMLScenario";
                    }
                    else if (keyNames[i] == "ApplicationExecutablesLocation") {
                        objNew[keyNames[i]] = "Bin";
                    }
                    else if (keyNames[i] == "Solution") {
                        objNew[keyNames[i]] = "slnNeoSpin";
                    }
                    else if (keyNames[i] == "BusinessObjectLocation") {
                        objNew[keyNames[i]] = "NeoSpinBusinessObjects";
                    }
                    else if (keyNames[i] == "ProjectID") {
                        objNew[keyNames[i]] = -1;
                    }
                    else if (keyNames[i] == "IsTFS" || keyNames[i] == "IsRemoteLogin" || keyNames[i] == "CheckPendingChanges") {
                        objNew[keyNames[i]] = false;
                    }
                    else {
                        objNew[keyNames[i]] = "";
                    }
                }
                objNew.htxPortals = [];
                objNew.IsHtxPortalTableExists = $scope.showPoratlDetailsTab;
                objNew.UserType = "owner";
                newScope.closeDialog();
                if (!$scope.configureSettingsDetails) {
                    $scope.configureSettingsDetails = [];
                }
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(objNew, $scope.configureSettingsDetails, "showProjectDetails");
                $scope.showProjectDetails(objNew);
                $rootScope.UndRedoBulkOp("End");
                //$scope.configureSettingsDetails.push(objNew);
                //$scope.selectedProject = objNew;
            }
        };
        newScope.checkProjectNameNew = function () {
            newScope.ErrorMessage = "";
            if (!newScope.newprojectName || newScope.newprojectName == "") {
                newScope.ErrorMessage = "Error:Project Name can not be empty.";
                return true;
            }
            else if ($scope.configureSettingsDetails && $scope.configureSettingsDetails.length > 0) {
                for (i = 0; i < $scope.configureSettingsDetails.length; i++) {
                    if (newScope.newprojectName.toLowerCase() == $scope.configureSettingsDetails[i].ProjectName.toLowerCase()) {
                        newScope.ErrorMessage = "Error:Duplicate Project Name.";
                        return true;
                    }
                    //else {
                    //    return false;
                    //}
                }
            }
        };
        newScope.closeDialog = function () {
            dialog.close();
        };
    };
    $scope.deleteProject = function () {
        var newScope = $scope.$new();
        newScope.IsDelete = true;
        var dialog = $rootScope.showDialog(newScope, "Delete Project", "Tools/views/DeleteProject.html", { width: 500, height: 160 });
        newScope.OkClick = function () {
            var index = $scope.configureSettingsDetails.indexOf($scope.selectedProject);
            if (index > -1) {
                $rootScope.DeleteItem($scope.configureSettingsDetails[index], $scope.configureSettingsDetails, "showProjectDetails");
                //$scope.configureSettingsDetails.splice(index, 1);
                if ($scope.configureSettingsDetails.length > 0 && index > 0) {
                    $scope.showProjectDetails($scope.configureSettingsDetails[index - 1]);
                }
                else if ($scope.configureSettingsDetails.length > 0) {
                    $scope.showProjectDetails($scope.configureSettingsDetails[$scope.configureSettingsDetails.length - 1]);
                }

                //$rootScope.IsProjectLoaded = true;
                //$http({
                //    method: 'POST',
                //    data: JSON.stringify({
                //        lstProjectDetails: $scope.configureSettingsDetails, userID: currentUser.UserID
                //    }),
                //    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                //    url: "api/Login/SaveProjectDetails"
                //}).then(function successCallback(response) {
                //    toastr.success("Project Deleted sucessfully.");                   
                //    $scope.saveAllprojectDetailsSettings(response.data);
                //}, function errorCallback(exceptionData) {
                //    $rootScope.IsProjectLoaded = false;
                //    $rootScope.showExceptionDetails(exceptionData.data);
                //});
                //$scope.selectedProject = undefined;
            }
            newScope.closeDialog();
        };
        newScope.closeDialog = function () {
            dialog.close();
        };
    };
    $scope.getFolderPathList = function (baseDirectory) {
        if (baseDirectory && baseDirectory != "") {
            hubcontext.hubMain.server.prepareFolderPath(baseDirectory);
        }
    };
    $scope.editProjectName = function (project) {
        var aintselectedIndex = $scope.configureSettingsDetails.indexOf(project);
        var newScope = $scope.$new();
        newScope.editprojectName = project.ProjectName;
        if (project && project.UserType == "owner") {
            var dialog = $rootScope.showDialog(newScope, "Edit Project Name", "Tools/views/EditProjectName.html");
            newScope.checkProjectName = function () {
                newScope.errorMessage = "";
                if (!newScope.editprojectName || newScope.editprojectName == "") {
                    newScope.errorMessage = "Error:Project Name can not be empty.";
                    return true;
                }
                else if (newScope.editprojectName.toLowerCase() == $scope.configureSettingsDetails[aintselectedIndex].ProjectName.toLowerCase()) {
                    return false;
                }
                else {
                    for (i = 0; i < $scope.configureSettingsDetails.length; i++) {
                        if (i != aintselectedIndex) {
                            if (newScope.editprojectName.toLowerCase() == $scope.configureSettingsDetails[i].ProjectName.toLowerCase()) {
                                //Same project name as other project name
                                newScope.errorMessage = "Error:Duplicate Project Name.";
                                return true;
                            }
                            //else {
                            //    return false;
                            //}
                        }
                    }
                }
            };
            newScope.saveEditProject = function (editprojectName) {
                if (editprojectName != "") {
                    $rootScope.EditPropertyValue($scope.configureSettingsDetails[aintselectedIndex].ProjectName, $scope.configureSettingsDetails[aintselectedIndex], "ProjectName", editprojectName);
                    newScope.closeDialog();
                }
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }
    };
    $scope.getSelectedDetails = function (ProjectProperty, selectiontype, noOfFiles, filterType, objHtxPortal) {
        $scope.selectedProperty = ProjectProperty;
        if (selectiontype == "FilePath") {
            var initialDirectory;
            if ($scope.selectedProject.BaseDirectory && $scope.selectedProject.ApplicationExecutablesLocation) {
                if (/\\+$/g.test($scope.selectedProject.BaseDirectory)) { // if path ends with '\\' then no need to append '\\' 
                    initialDirectory = $scope.selectedProject.BaseDirectory + $scope.selectedProject.ApplicationExecutablesLocation;
                } else {
                    initialDirectory = $scope.selectedProject.BaseDirectory + "\\" + $scope.selectedProject.ApplicationExecutablesLocation;
                }
            }
            $scope.IsSingleOrMultiple = noOfFiles;
            hubcontext.hubMain.server.getFilePath(noOfFiles == "M", "ProjectSettings", initialDirectory, filterType);
        }
        else if (selectiontype == "FolderPath") {
            hubcontext.hubMain.server.getFolderPath(ProjectProperty.BaseDirectory).done(function (folderpath) {
                $scope.$apply(function () {
                    if (folderpath != undefined && folderpath != "") {
                        var tempfolderPath = folderpath;

                        if ($scope.selectedProperty == "Website") {

                            if ($scope.selectedProject.Website && $scope.selectedProject.Website != "") {
                                tempfolderPath = $scope.selectedProject.Website + ";" + folderpath.substring(folderpath.lastIndexOf("\\") + 1, folderpath.length);
                            }
                            else {
                                tempfolderPath = folderpath.substring(folderpath.lastIndexOf("\\") + 1, folderpath.length);
                            }
                        }
                        if (objHtxPortal) {
                            $rootScope.EditPropertyValue(objHtxPortal[$scope.selectedProperty], objHtxPortal, $scope.selectedProperty, tempfolderPath);
                        } else {
                            $rootScope.EditPropertyValue($scope.selectedProject[$scope.selectedProperty], $scope.selectedProject, $scope.selectedProperty, tempfolderPath);
                        }
                    }
                });
            });
        }
    };

    $scope.sameAsBussinessObjChange = function (flag, project) {
        if (flag) {
            if (project.BusinessObjectProjectLocation != "") {
                $rootScope.EditPropertyValue($scope.selectedProject.DataObjectProjectLocation, $scope.selectedProject, "DataObjectProjectLocation", project.BusinessObjectProjectLocation);
                $scope.selectedProject.DataObjectLocation = "";
            }
        }
        else {
            $rootScope.EditPropertyValue($scope.selectedProject.DataObjectProjectLocation, $scope.selectedProject, "DataObjectProjectLocation", "");
            $scope.selectedProject.DataObjectLocation = "";
        }
    };
    $scope.setCheckBoxVariable = function () {
        if (!$scope.selectedProject.DataObjectProjectLocation || $scope.selectedProject.DataObjectProjectLocation == "") {
            $scope.sameAsBussinesObj = false;
        }
    };
    $scope.receiveProjectSettingsFilePath = function (filepath) {
        $scope.$evalAsync(function () {

            if (filepath != undefined && filepath.length != 0) {
                $rootScope.UndRedoBulkOp("Start");
                if ($scope.IsSingleOrMultiple == "M") {
                    for (var i = 0; i < filepath.length; i++) {
                        var assemblyName = filepath[i].substring(filepath[i].lastIndexOf("\\") + 1, filepath[i].length);
                        var flag = true;
                        if ($scope.lstAssemblyNames && $scope.lstAssemblyNames.length > 0) {
                            var index = $scope.lstAssemblyNames.map(function (element) { return element.Name; }).indexOf(assemblyName);
                            if (index > -1) {
                                flag = false;
                            }
                        }
                        if (flag) {
                            if ($scope.selectedProject[$scope.selectedProperty] && $scope.selectedProject[$scope.selectedProperty] != "") {
                                $rootScope.EditPropertyValue($scope.selectedProject[$scope.selectedProperty], $scope.selectedProject, "" + $scope.selectedProperty + "", $scope.selectedProject[$scope.selectedProperty] + assemblyName + ";");
                            }
                            else {
                                $rootScope.EditPropertyValue($scope.selectedProject[$scope.selectedProperty], $scope.selectedProject, "" + $scope.selectedProperty + "", assemblyName + ";");
                            }
                            if ($scope.selectedProperty == "AssemblyNames") {
                                $rootScope.PushItem({ Name: assemblyName }, $scope.lstAssemblyNames);
                                //$scope.lstAssemblyNames.push({ Name: assemblyName });
                            }
                        }
                    }
                }
                else {
                    if ($scope.selectedProperty === "TFPath" || $scope.selectedProperty === "tfptPath") {
                        $rootScope.EditPropertyValue($scope.selectedProject[$scope.selectedProperty], $scope.selectedProject, "" + $scope.selectedProperty + "", filepath[0]);
                    }
                    else {
                        var assemblyName = filepath[0].substring(filepath[0].lastIndexOf("\\") + 1, filepath[0].length);
                        var flag = true;
                        if ($scope.lstAssemblyNames && $scope.lstAssemblyNames.length > 0) {
                            var index = $scope.lstAssemblyNames.map(function (element) { return element.Name; }).indexOf(assemblyName);
                            if (index > -1) {
                                flag = false;
                            }
                        }
                        if (flag) {
                            $rootScope.EditPropertyValue($scope.selectedProject[$scope.selectedProperty], $scope.selectedProject, "" + $scope.selectedProperty + "", assemblyName);
                            if ($scope.selectedProperty == "AssemblyName") {
                                $rootScope.PushItem({ Name: assemblyName }, $scope.lstAssemblyNames);
                                // $scope.lstAssemblyNames.push({ Name: assemblyName });
                            }
                        }
                    }
                }
                $rootScope.UndRedoBulkOp("End");
            }

        });
    };
    $scope.selectAssemblyName = function (assembly) {
        $scope.selectedAssembly = assembly;
    };
    $scope.deleteAssemblyNames = function () {
        $rootScope.UndRedoBulkOp("Start");
        for (i = 0; i < $scope.lstAssemblyNames.length; i++) {
            if ($scope.lstAssemblyNames[i] == $scope.selectedAssembly) {
                //$scope.lstAssemblyNames.splice(i, 1);
                $rootScope.DeleteItem($scope.selectedAssembly, $scope.lstAssemblyNames);
                $scope.selectedAssembly = undefined;
            }
        }
        var strtemp = "";
        for (i = 0; i < $scope.lstAssemblyNames.length; i++) {
            strtemp += $scope.lstAssemblyNames[i].Name + ";";
        }
        $rootScope.EditPropertyValue($scope.selectedProject.AssemblyNames, $scope.selectedProject, "AssemblyNames", strtemp);
        $rootScope.UndRedoBulkOp("End");
    };
    //=========================================== Folder Path implementation starts
    $scope.getFolderPathIntellisense = function (controlID, basePath, event) {

        $('#' + controlID).autocomplete({
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

                if (basePath && basePath != "") {

                    if ((controlID == "BusinessObjectLocation" || controlID == "DataObjectLocation") && $scope.selectedProject.Solution && $scope.selectedProject.Solution != "") {
                        basePath = $scope.selectedProject.Solution;
                    }
                    else {
                        basePath = "";
                    }
                    hubcontext.hubMain.server.getFolderPathIntellisense(request.term.trim(), basePath, false).done(function (data) {
                        if (data != "") {
                            $scope.responseParsed = parsePathIntellisenseResult(data);
                            response($scope.responseParsed);
                        }
                        else {
                            $scope.responseParsed = [];
                            response($scope.responseParsed);
                        }
                    });
                }

            },
            focus: function (event, ui) {
                $(".ui-autocomplete > li").attr("title", ui.item.label);
            },
            search: function (event, ui) {

            },
            select: function (event, ui) {
                $scope.selectedProject["" + $(this)[0].id + ""] = ui.item.value;
                $(this).trigger("change");
                $('#' + $(this)[0].id).click();
            },
            close: function () {
                $(".tab-content").find(".settings-tab-content").css("pointer-events", "auto");
                $("#dvIntellisense")[0].innerHTML = "";
            }
        });

        if (event.type == "click") {
            $('#' + controlID).focus();
            if ($('#' + controlID).data('ui-autocomplete')) $('#' + controlID).autocomplete("search", $('#' + controlID).val());
        }

        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $('#' + controlID).data('ui-autocomplete')) {
            $('#' + controlID).autocomplete("search", $('#' + controlID).val());
            event.preventDefault();
        }
    };



    var element = $('body[ng-controller="MainController"]');
    Mainscope = angular.element(element).scope();
    $scope.IsCloseEnable = function () {
        var tempData = ConfigurationFactory.getLastProjectDetails();
        if (Mainscope.CurrentProjectName == undefined || Mainscope.CurrentProjectName == "") {
            return true;
        }
        else {
            return false;
        }
    };
    $scope.closeFile = function (astrProjectID) {
        if (!$scope.isDirty) {
            $scope.closeFileFunction();
        }
        else {
            if (!$scope.isSavePending && $scope.configureSettingsDetails.length > 0 && !$scope.IsDisable) {
                var newScope = $scope.$new();
                newScope.IsSave = true;
                var dialog = $rootScope.showDialog(newScope, "UnSave Project", "Tools/views/DeleteProject.html", { width: 500, height: 160 });
                newScope.OkClick = function () {
                    $rootScope.IsLoading = true;
                    $scope.isSavePending = true;

                    for (var i = 0; i < $scope.configureSettingsDetails.length; i++) {
                        if ($scope.selectedProject.ProjectName == $scope.configureSettingsDetails[i].ProjectName) {
                            $scope.configureSettingsDetails[i] = $scope.selectedProject;
                        }
                    }
                    var lintprojectId = -1;
                    if (astrProjectID) {
                        lintprojectId = astrProjectID;
                    }

                    //$rootScope.IsProjectLoaded = true;
                    $http({
                        method: 'POST',
                        data: JSON.stringify({
                            lstProjectDetails: $scope.configureSettingsDetails, userID: currentUser.UserID, projectID: lintprojectId
                        }),
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        url: "api/Login/SaveProjectDetails"
                    }).then(function successCallback(response) {
                        dialog.close();
                        $rootScope.IsLoading = false;
                        toastr.success("Project Detail Saved sucessfully.");
                        $scope.saveAllprojectDetailsSettings(response.data);
                    }, function errorCallback(exceptionData) {
                        $rootScope.IsLoading = false;
                        $scope.isSavePending = false;
                        $rootScope.IsProjectLoaded = false;
                        $rootScope.showExceptionDetails(exceptionData.data);
                    });
                };
                newScope.closeDialog = function () {
                    dialog.close();
                    $scope.closeFileFunction();
                };
            }
        }
    };
    $scope.closeFileFunction = function () {
        if (!$scope.IsCloseEnable() && ConfigurationFactory.getLastProjectDetails() != null && ConfigurationFactory.getLastProjectDetails().ProjectName && $rootScope.lstopenedfiles.length == 0) {
            $rootScope.isConfigureSettingsVisible = false;
            $rootScope.isstartpagevisible = true;
            $rootScope.currentopenfile = undefined;
        }
        else if (!$scope.IsCloseEnable() && ConfigurationFactory.getLastProjectDetails() != null && ConfigurationFactory.getLastProjectDetails().ProjectName) {
            $rootScope.isConfigureSettingsVisible = false;
            if ($rootScope.currentopenfile === undefined) {
                $rootScope.isstartpagevisible = true;
            }
        }
    };
    //$scope.OpenMapUserDialog = function (IsUpdate, objUser) {
    //    var newScope = $scope.$new();
    //    $scope.IsUpdate = IsUpdate;
    //    newScope.lstTempMapUser = [];
    //    newScope.lstUserType = [{ ID: "owner", Description: "Owner" }, { ID: "developer", Description: "Developer" }, { ID: "ba", Description: "BA" }, { ID: "viewer", Description: "Viewer" }];
    //    var dialog;
    //    newScope.setStausAndDate = function (objUser) {
    //        var index = newScope.lstTempMapUser.map(function (element) { return element.UserID; }).indexOf(objUser.UserID);
    //        if (index > -1) {
    //            var newScope1 = $scope.$new();
    //            newScope1.IsOwner = true;
    //            var temp = JSON.stringify(newScope.lstUserType);
    //            newScope1.lstUserType = JSON.parse(temp);
    //            $http({
    //                method: 'POST',
    //                data: { UserID: objUser.UserID },
    //                headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //                url: "api/Login/CheckUserIsOwner"
    //            }).then(function successCallback(response) {
    //                if (response.data == "owner" && !IsUpdate) {
    //                    newScope1.lstUserType.splice(0, 1);
    //                }
    //                var dialog = $rootScope.showDialog(newScope1, "Set User Data", "StartUp/views/UserAssociationDialog.html");
    //                ComponentsPickers.init();
    //                newScope1.Status = newScope.lstTempMapUser[index].Status;
    //                newScope1.StartDate = newScope.lstTempMapUser[index].StartDate;
    //                newScope1.EndDate = newScope.lstTempMapUser[index].EndDate;
    //                newScope1.UserType = newScope.lstTempMapUser[index].UserType;
    //                newScope1.OkClick = function () {
    //                    newScope.lstTempMapUser[index].Status = newScope1.Status;
    //                    newScope.lstTempMapUser[index].StartDate = newScope1.StartDate;
    //                    if (newScope1.EndDate) {
    //                        newScope.lstTempMapUser[index].EndDate = newScope1.EndDate;
    //                    }
    //                    else {
    //                        newScope.lstTempMapUser[index].EndDate = null;
    //                    }

    //                    newScope.lstTempMapUser[index].UserType = newScope1.UserType;
    //                    if (IsUpdate) {
    //                        $scope.lstTempMappedUser.push(newScope.lstTempMapUser[index]);
    //                        $scope.SaveAndUpdateMapUser(IsUpdate);
    //                    }
    //                    dialog.close();
    //                };
    //                newScope1.closeDialog = function () {
    //                    dialog.close();
    //                };
    //                newScope1.checkValidValue = function () {
    //                    newScope1.UserAssociationError = "";
    //                    if (newScope1.Status == "") {
    //                        newScope1.UserAssociationError = "Status required";
    //                        return true;
    //                    }
    //                    else if (newScope1.StartDate == null) {
    //                        newScope1.UserAssociationError = "Start Date required";
    //                        return true;
    //                    }
    //                    else if (!ValidateDate(newScope1.StartDate)) {
    //                        newScope1.UserAssociationError = "Start Date should be mm/dd/yyyy or mm-dd-yyyy format";
    //                        return true;
    //                    }
    //                    else if (new Date() < Date.parse(newScope1.StartDate)) {
    //                        newScope1.UserAssociationError = "Start Date should be less than Or equal to current Date";
    //                        return true;
    //                    }
    //                    else if (newScope1.EndDate && !ValidateDate(newScope1.EndDate)) {
    //                        newScope1.UserAssociationError = "End Date should be mm/dd/yyyy or mm-dd-yyyy format";
    //                        return true;
    //                    }
    //                    else if (newScope1.EndDate && Date.parse(newScope1.StartDate) > Date.parse(newScope1.EndDate)) {
    //                        newScope1.UserAssociationError = "Start Date should be greater than End Date";
    //                        return true;
    //                    }
    //                    else if ($scope.objClientInfo && $scope.objClientInfo.EndDate != "" && Date.parse($scope.objClientInfo.EndDate) < Date.parse(newScope1.EndDate)) {
    //                        newScope1.UserAssociationError = "End Date should be less than client End Date. i.e " + $scope.objClientInfo.EndDate;
    //                        return true;
    //                    }
    //                    else if (newScope1.UserType == "") {
    //                        newScope1.UserAssociationError = "User type required";
    //                        return true;
    //                    }
    //                    return false;
    //                };
    //            }, function errorCallback(exceptionData) {
    //                $rootScope.showExceptionDetails(exceptionData.data);
    //            });
    //        }
    //    };
    //    if (IsUpdate) {
    //        newScope.IsSelected = true;
    //        newScope.lstUser = [];
    //        newScope.lstUser.push(objUser);
    //        newScope.lstTempMapUser.push(objUser);
    //        newScope.setStausAndDate(objUser);
    //    }
    //    else {
    //        $http({
    //            method: 'POST',
    //            headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //            url: "api/Login/GetUserForMapping?UserID=" + currentUser.UserID
    //        }).then(function successCallback(response) {
    //            newScope.lstUnMappedUsers = response.data;
    //            newScope.showEdit = true;
    //            dialog = $rootScope.showDialog(newScope, "Assign User", "StartUp/views/ClientUserAssociationDialog.html", { width: 690, height: 430 });
    //        }, function errorCallback(exceptionData) {
    //            $rootScope.showExceptionDetails(exceptionData.data);
    //        });

    //    }

    //    newScope.OkClick = function () {
    //        for (var i = 0; i < newScope.lstTempMapUser.length; i++) {
    //            var index = $scope.lstMappedUser.map(function (element) { return element.UserID; }).indexOf(newScope.lstTempMapUser[i].UserID);
    //            if (index < 0) {
    //                //var obj = { ClientID: null, UserID: newScope.lstTempMapUser[i].UserID, Status: "active", StartDate: null, EndDate: null, UserType: "ba" }
    //                $scope.lstTempMappedUser.push(newScope.lstTempMapUser[i]);
    //            }
    //            if (IsUpdate && index > -1) {
    //                $scope.lstTempMappedUser.splice(index, 1);
    //                $scope.lstTempMappedUser.push(newScope.lstTempMapUser[i]);
    //            }
    //        }
    //        $scope.SaveAndUpdateMapUser(IsUpdate);
    //        newScope.closeDialog();
    //    };
    //    newScope.closeDialog = function () {
    //        dialog.close();
    //    };
    //    newScope.CheckUserMap = function (isSelected, objUser) {
    //        if (isSelected) {
    //            var obj = { ClientID: null, UserID: objUser.UserID, Status: "", StartDate: null, EndDate: null, UserType: "" };
    //            newScope.lstTempMapUser.push(obj);
    //        }
    //        else {
    //            var index = newScope.lstTempMapUser.map(function (element) { return element.UserID; }).indexOf(objUser.UserID);
    //            if (index > -1) {
    //                newScope.lstTempMapUser.splice(index, 1);
    //            }
    //        }
    //    };
    //    newScope.IsDisable = function () {
    //        newScope.IsError = false;
    //        newScope.ErrorMessage = "";
    //        if (newScope.lstTempMapUser.length == 0) {
    //            return true;
    //        }
    //        for (var i = 0; i < newScope.lstTempMapUser.length > 0; i++) {
    //            var obj = newScope.lstTempMapUser[i];
    //            if (obj.UserID == "" || obj.Status == "" || obj.StartDate == null || obj.UserType == "") {
    //                newScope.ErrorMessage = "Set status,startDate,EndDate and User Type.";
    //                return true;
    //            }
    //        }
    //        return false;
    //    };

    //};
    $scope.MapADUsers = function () {
        var newMapScope = $scope.$new();
        newMapScope.IsShowRole = true;
        newMapScope.searchADUser = function () {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: { userID: currentUser.UserID, samAccounName: newMapScope.samAccountName, domainName: newMapScope.domainName, name: newMapScope.name, email: newMapScope.email },
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                async: false,
                url: "api/ConfigureSetting/GetADUsersForMapping"
            }).then(function successCallback(response) {
                $rootScope.IsLoading = false;
                newMapScope.filterADUsers = response.data;

                newMapScope.removeMappedADUser(newMapScope.selectedADUsers, newMapScope.filterADUsers);

                newMapScope.selectedUserDisplayCount = newMapScope.filterUserDisplayCount = 30;
                // newMapScope.samAccountName = newMapScope.name = newMapScope.domainName = "";

                newMapScope.filterUserDisplayCount = 30;
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        };

        newMapScope.OK = function () {
            newMapScope.beforeOK();
            $scope.mappedADUsers = [];
            for (var idx = 0; idx < newMapScope.selectedADUsers.length; idx++) {
                var tempAdUserObj = {
                    UserID: newMapScope.selectedADUsers[idx].UserID,
                    SAMAccountName: newMapScope.selectedADUsers[idx].SAMAccountName,
                    Name: newMapScope.selectedADUsers[idx].Name,
                    DomainName: newMapScope.selectedADUsers[idx].DomainName,
                    UserClientAssociation: { UserType: newMapScope.selectedADUsers[idx].UserType },
                    EmailId: newMapScope.selectedADUsers[idx].EmailId,
                };
                //$scope.mappedADUsers.push(newMapScope.selectedADUsers[idx]);
                $scope.mappedADUsers.push(tempAdUserObj);
            }
            if ($scope.mappedADUsers && $scope.mappedADUsers.length > 0 && currentUser && currentUser.UserID) {
                $rootScope.IsLoading = true;
                $http({
                    method: 'POST',
                    data: { ActiveDirectoryUsers: $scope.mappedADUsers, UserID: currentUser.UserID },
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    async: false,
                    url: "api/ConfigureSetting/AddADUserMapping"
                }).then(function successCallback(response) {
                    $rootScope.IsLoading = false;
                    $scope.$evalAsync(function () {
                        $scope.lstAdMappedUser = response.data;
                    });
                }, function errorCallback(exceptionData) {
                    $rootScope.IsLoading = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
            }
            newMapScope.closeDialog();
        };
        newMapScope.dialogMapuser = $rootScope.showDialog(newMapScope, "Assign Active Directory User", "StartUp/views/GetActiveDirectoryUsers.html", { width: 1200, height: 550 });

    }
    $scope.DeleteMappedUsed = function (IsFromADMapping) {
        if ($scope.selectedUser) {
            $rootScope.IsLoading = true;
            $http({
                method: 'POST',
                data: JSON.stringify({
                    ClientID: $scope.selectedUser.ClientID, UserID: $scope.selectedUser.UserID, OwnerUserID: currentUser.UserID, IsFromADUser: IsFromADMapping
                }),
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                url: "api/Login/DeleteUserMapping"
            }).then(function successCallback(response) {
                $rootScope.IsLoading = false;
                if (IsFromADMapping) {
                    if (response.data && response.data.length > 0) {
                        $scope.lstAdMappedUser = response.data;
                    } else {
                        $scope.lstAdMappedUser = [];
                    }
                }
                else {
                    if (response.data && response.data.length > 0) {
                        $scope.lstMappedUser = response.data;
                    } else {
                        $scope.lstMappedUser = [];
                    }
                }
                $scope.lstTempMappedUser = [];
                toastr.success("User Deleted sucessfully.");
            }, function errorCallback(exceptionData) {
                $rootScope.IsLoading = false;
                $rootScope.showExceptionDetails(exceptionData.data);
            });
        }
    };

    //$scope.SaveAndUpdateMapUser = function (IsUpdate) {
    //    if ($scope.lstTempMappedUser.length > 0) {
    //        $http({
    //            method: 'POST',
    //            data: JSON.stringify({
    //                UserClientDetails: $scope.lstTempMappedUser, IsUpdateFlag: IsUpdate, OwnerUserID: currentUser.UserID
    //            }),
    //            headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //            url: "api/Login/CreateUpdateUserMapping"
    //        }).then(function successCallback(response) {
    //            //$rootScope.IsProjectLoaded = false;
    //            $scope.lstMappedUser = response.data;
    //            $scope.lstTempMappedUser = [];
    //            var newScope = $scope.$new();
    //            if ($scope.IsUpdate) {
    //                toastr.success("User Updated sucessfully.");
    //            }
    //            else {
    //                toastr.success("User created sucessfully.");
    //            }
    //            $scope.IsUpdate = undefined;
    //        }, function errorCallback(exceptionData) {
    //            $rootScope.IsProjectLoaded = false;
    //            $rootScope.showExceptionDetails(exceptionData.data);
    //        });
    //    }
    //};

    $scope.OpenAddUserDialog = function () {
        var newScope11 = $scope.$new();
        var dialog = $rootScope.showDialog(newScope11, "Add User", "StartUp/views/CreateUserDialog.html", { width: 750, height: 500 });
        ComponentsPickers.init();
        newScope11.CreateUserOkClick = function () {
            if ($scope.lstUsers.length > 0) {
                index = $scope.lstUsers.map(function (element) { return element.UserName; }).indexOf(this.UserName);
            }
            if ($scope.lstUsers.length == 0 || index < 0) {
                $http({
                    method: 'POST',
                    data: {
                        UserID: currentUser.UserID, UserName: this.UserName, FirstName: this.FirstName, LastName: this.LastName,
                        Status: this.Status, StartDate: this.StartDate, EndDate: this.EndDate, UserType: this.UserType, EmailId: this.EmailID
                    },
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/CreateAndMapUser"
                }).then(function successCallback(response) {
                    $rootScope.IsProjectLoaded = false;
                    if (response.data != "UserExist") {
                        dialog.close();
                        $scope.lstUsers = response.data[0];
                        if (response.data[1] && response.data[1].length) {
                            $scope.lstMappedUser = response.data[1];
                        } else {
                            $scope.lstMappedUser = [];
                        }
                        toastr.success("User Mapped sucessfully.");
                    }
                    else {
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
        newScope11.closeDialog = function () {
            dialog.close();
        };
        newScope11.CheckValidUser = function () {
            if (!newScope11.UserName || newScope11.UserName == "") {
                newScope11.ErrorMessage = "User Name Required";
                newScope11.IsError = true;
            }
            //else if (!validateEmail(newScope11.UserName)) {
            //    newScope11.ErrorMessage = "User Name not valid.";
            //    newScope11.IsError = true;
            //}
            else if (!newScope11.FirstName || newScope11.FirstName == "") {
                newScope11.ErrorMessage = "First Name Required";
                newScope11.IsError = true;
            }
            else if (!newScope11.LastName || newScope11.LastName == "") {
                newScope11.ErrorMessage = "Last Name Required";
                newScope11.IsError = true;
            }
            else if (!newScope11.Status || newScope11.Status == "") {
                newScope11.ErrorMessage = "Status Required";
                newScope11.IsError = true;
            }
            else if (!newScope11.StartDate || newScope11.StartDate == null) {
                newScope11.ErrorMessage = "Start Date Required";
                newScope11.IsError = true;
            }
            else if (!ValidateDate(newScope11.StartDate)) {
                newScope11.ErrorMessage = "Start Date should be mm/dd/yyyy or mm-dd-yyyy";
                newScope11.IsError = true;
            }
            else if (new Date() < Date.parse(newScope11.StartDate)) {
                newScope11.ErrorMessage = "Start Date should be less than Or equal to current Date";
                newScope11.IsError = true;
            }
            else if (newScope11.EndDate && !ValidateDate(newScope11.EndDate)) {
                newScope11.ErrorMessage = "End Date should be mm/dd/yyyy or mm-dd-yyyy";
                newScope11.IsError = true;
            }
            else if (newScope11.EndDate && Date.parse(newScope11.StartDate) > Date.parse(newScope11.EndDate)) {
                newScope11.ErrorMessage = "Start Date greater than End Date";
                newScope11.IsError = true;
            }
            else if ($scope.objClientInfo && $scope.objClientInfo.EndDate != "" && Date.parse($scope.objClientInfo.EndDate) < Date.parse(newScope11.EndDate)) {
                newScope11.ErrorMessage = "End Date should be less than client End Date. i.e " + $scope.objClientInfo.EndDate;
                return true;
            }
            else if (!newScope11.UserType || newScope11.UserType == "") {
                newScope11.ErrorMessage = "UserType Required";
                newScope11.IsError = true;
            }
            else if (!newScope11.EmailID || newScope11.EmailID == "") {
                newScope11.ErrorMessage = "Email-id Required";
                newScope11.IsError = true;
            }
            else if (!validateEmail(newScope11.EmailID)) {
                newScope11.ErrorMessage = "Email-id not valid";
                newScope11.IsError = true;
            }
            else {
                newScope11.ErrorMessage = "";
                newScope11.IsError = false;
            }
            return newScope11.IsError;
        };
    };

    $scope.onsavefileclick = function () {
        if (!$scope.isSavePending && $scope.configureSettingsDetails && $scope.configureSettingsDetails.length > 0 && !$scope.IsDisable) {
            var newScope = $scope.$new();
            var lastProject = ConfigurationFactory.getLastProjectDetails();
            if (lastProject && lastProject.ProjectName && lastProject.ProjectId) {
                newScope.IsSaveAndReLoad = true;
            }
            else {
                newScope.IsSave = true;
            }
            var dialog = $rootScope.showDialog(newScope, "Save Project", "Tools/views/DeleteProject.html", { width: 500, height: 160 });
            newScope.OkClick = function () {
                $rootScope.IsLoading = true;
                $scope.isSavePending = true;

                //setDBTypestoAllProject
                $scope.concatDBTypesforAllProjects($scope.configureSettingsDetails);

                // only owner is allowed to do it 
                if ($scope.selectedProject) {
                    for (var i = 0; i < $scope.configureSettingsDetails.length; i++) {
                        if ($scope.selectedProject.ProjectId == $scope.configureSettingsDetails[i].ProjectId && $scope.selectedProject.ProjectName == $scope.configureSettingsDetails[i].ProjectName) {
                            $scope.configureSettingsDetails[i] = $scope.selectedProject;
                        }
                    }
                }
                $http({
                    method: 'POST',
                    data: JSON.stringify({
                        lstProjectDetails: $scope.configureSettingsDetails, userID: currentUser.UserID
                    }),
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    url: "api/Login/SaveProjectDetails"
                }).then(function successCallback(response) {
                    $scope.isDirty = false;
                    toastr.success("Project Detail Saved sucessfully.");
                    $scope.saveAllprojectDetailsSettings(response.data);
                    $rootScope.IsLoading = false;
                }, function errorCallback(exceptionData) {
                    $rootScope.IsProjectLoaded = false;
                    $rootScope.IsLoading = false;
                    $scope.isSavePending = false;
                    $rootScope.showExceptionDetails(exceptionData.data);
                });
                newScope.closeDialog();
            };
            newScope.closeDialog = function () {
                dialog.close();
            };
        }
    };
    $scope.selectUserRow = function (userObj) {
        $scope.selectedUser = userObj;
    };

    $scope.saveAllprojectDetailsSettings = function (lstprojects) {
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = true;
        });
        $scope.configureSettingsDetails = lstprojects;
        var userCookie = $cookies.getObject("UserDetails");
        $http({
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            url: "api/Login/GetPojectNamesAndProjectDetail?UserID=" + JSON.parse(userCookie).UserID
        }).then(function successCallback(response) {
            if (response.data == "AccessError") {
                toastr.error("Sorry, You have No Access.");
            }
            else if (response.data == "InvalidRequest") {
                toastr.error("Invalid Request.");
            }
            else {
                if (response.data[0].length > 0) {
                    if (response.data[1].ProjectName != null) {
                        ConfigurationFactory.setConfigurationObject(response.data[0], response.data[1]);
                    }
                    else {
                        ConfigurationFactory.setConfigurationObject(response.data[0], null);
                    }
                }
                else {
                    $rootScope.isConfigureSettingsVisible = true;
                }
                var lstrProjects = JSON.stringify(lstprojects);
                if (lstrProjects.length < 32000) {
                    hubcontext.hubMain.server.updateAllProjectLoadDetails(lstprojects);
                }
                else {
                    $scope.sendDataInPackets(lstrProjects, "UpdateAllProjectLoadDetails")
                }
            }

        }, function errorCallback(exceptionData) {
            $rootScope.IsLoading = false;
            $rootScope.showExceptionDetails(exceptionData.data);
        });
    };

    $scope.sendDataInPackets = function (data, operation) {
        var strpacket = "";
        var lstDataPackets = [];
        var count = 0;
        for (var i = 0, len = data.length; i < len; i++) {
            count++;
            strpacket = strpacket + data[i];
            if (count == 32000) {
                count = 0;
                lstDataPackets.push(strpacket);
                strpacket = "";
            }
        }
        if (count != 0) {
            lstDataPackets.push(strpacket);
        }

        for (var i = 0, len = lstDataPackets.length; i < len; i++) {
            hubMain.server.receiveDataPackets(lstDataPackets[i], len, $scope.currentopenfile, i, operation, []);
        }
    }

    $scope.receiveAfterUpdateAllProjectLoadDetails = function (data) {
        $rootScope.IsLogOut = true;
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            $scope.isSavePending = false;
        });
        var lastProject = ConfigurationFactory.getLastProjectDetails();
        if (lastProject && lastProject.ProjectName && lastProject.ProjectId) {
            var IslastProjectDelete = false;
            for (var i = 0; i < $scope.configureSettingsDetails.length; i++) {
                if (lastProject.ProjectId == $scope.configureSettingsDetails[i].ProjectId) {
                    IslastProjectDelete = true;
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = true;
                    });
                    if (!angular.equals(lastProject, $scope.configureSettingsDetails[i])) {
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $rootScope.IsProjectLoaded = true;
                        });
                        $rootScope.resetRootScopeVarible();
                        $rootScope.isConfigureSettingsVisible = false;
                        $rootScope.loadProjectData($scope.configureSettingsDetails[i], true);
                    }
                    else {
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                        });
                        $scope.selectedProject = undefined;
                        $rootScope.resetRootScopeVarible();
                    }
                    break;
                }
            }
            if (!IslastProjectDelete) {
                $scope.selectedProject = undefined;
                if ($scope.configureSettingsDetails.length > 0) {
                    $scope.selectedProject = $scope.configureSettingsDetails[0];
                }
                $rootScope.resetRootScopeVarible();
            }
        }
        else {
            $scope.selectedProject = undefined;
            if ($scope.configureSettingsDetails.length > 0) {
                $scope.selectedProject = $scope.configureSettingsDetails[0];
            }
            $rootScope.resetRootScopeVarible();
        }
        $rootScope.updateProjectList();
    }

    $scope.isCurrentFileDirty = function () {
        var scope = getScopeByFileName("SettingsPage");
        if (scope) {
            return scope.isDirty;
        }
    };
    $scope.CheckValidParentEntity = function () {
        if ($EntityIntellisenseFactory.getEntityIntellisense() && $EntityIntellisenseFactory.getEntityIntellisense().length > 0) {
            var tempEntity = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (x) { return x.DisplayName == $scope.selectedProject.DefaultParentEntity; });
            if (tempEntity && tempEntity.length > 0 && tempEntity[0].ParentId) {
                $scope.ErrorMessage = "Can not set " + $scope.selectedProject.DefaultParentEntity + " as Default parent Entity.";
            }
            else {
                $scope.ErrorMessage = undefined;
            }
        }
    };
    $scope.UndoClick = function () {
        if (Mainscope && Mainscope.UndoClick) {
            Mainscope.UndoClick();
        }
    };
    $scope.RedoClick = function () {
        if (Mainscope && Mainscope.RedoClick) {
            Mainscope.RedoClick();
        }
    };

    ////#region key Move up n move down 

    //$scope.KeyUp = function (ruleType) {
    //    if (ruleType && ruleType == 'Configuration' && $scope.selectedProject) {
    //        var lstProject = { Name: '', Value: '', dictAttributes: {}, Elements: [] };
    //        angular.forEach($scope.configureSettingsDetails, function (itm) {
    //            lstProject.Elements.push(itm);
    //        });
    //        var currentUser = keyUpAction($scope.selectedProject, lstProject);
    //        if (currentUser) {
    //            $scope.showProjectDetails(currentUser);
    //        }
    //        else {
    //            return true;
    //        }
    //    }
    //}

    //$scope.KeyDown = function (ruleType) {
    //    if (ruleType && ruleType == 'Configuration' && $scope.selectedProject) {
    //        var lstProject = { Name: '', Value: '', dictAttributes: {}, Elements: [] };
    //        angular.forEach($scope.configureSettingsDetails, function (itm) {
    //            lstProject.Elements.push(itm);
    //        });
    //        var currentUser = keyDownAction($scope.selectedProject, lstProject);
    //        if (currentUser) {
    //            $scope.showProjectDetails(currentUser);
    //        }
    //        else {
    //            return true;
    //        }
    //    }
    //}

    ////#endregion
    // #region portal settings section


    $scope.showPoratlDetails = function (portal) {
        $scope.selectedPortalDetails = portal;
        $scope.selectedCssFile = null;
        $scope.selectedJsFile = null;
        $scope.lstPortalCss = [];
        $scope.lstPortalJs = [];
        if ($scope.selectedPortalDetails && $scope.selectedPortalDetails.CSSFiles) {
            $scope.lstPortalCss = $scope.selectedPortalDetails.CSSFiles.split(",");
            $scope.lstPortalCss = $scope.lstPortalCss.unique();
        }
        if ($scope.selectedPortalDetails && $scope.selectedPortalDetails.JSFiles) {
            $scope.lstPortalJs = $scope.selectedPortalDetails.JSFiles.split(",");
            $scope.lstPortalJs = $scope.lstPortalJs.unique();
        }
    };
    $scope.addNewPortal = function (type) {
        var newScope = $scope.$new();
        newScope.title = "Add New Portal";
        newScope.ErrorMessage = "";
        newScope.portalName = null;
        var tempPortalName = null;
        if (type == "edit") {
            newScope.title = "Edit Portal";
            newScope.portalName = $scope.selectedPortalDetails.PortalName;
            tempPortalName = newScope.portalName;
        }
        newScope.onPortalDialogOkClick = function () {
            if (type == "new") {
                var newPortalDetails = { CSSFiles: "", PortalPath: "", JSFiles: "", IsDefault: "false" };
                newPortalDetails.PortalName = newScope.portalName;
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.PushItem(newPortalDetails, $scope.selectedProject.htxPortals, "showPoratlDetails");
                $scope.showPoratlDetails(newPortalDetails);
                $rootScope.UndRedoBulkOp("End");
                //$scope.selectedProject.htxPortals.push(newPortalDetails);
            } else if (type == "edit") {
                $scope.selectedPortalDetails.PortalName = newScope.portalName;
            }
            if (angular.isArray($scope.selectedProject.htxPortals) && $scope.selectedProject.htxPortals.length > 0) {
                $scope.showAddPortalDetailsTab = true;
            }
            dialog.close();
        };
        newScope.checkPortalName = function () {
            newScope.ErrorMessage = "";
            if (!newScope.portalName) {
                newScope.ErrorMessage = "Error:Portal Name can not be empty.";
                return true;
            }
            else if (angular.isArray($scope.selectedProject.htxPortals) && $scope.selectedProject.htxPortals.length > 0) {
                if (type == "edit" && newScope.portalName && tempPortalName.toLowerCase() == newScope.portalName.toLowerCase()) {
                    return false;
                }
                var objPortal = $scope.selectedProject.htxPortals.filter(function (portal) {
                    return (portal.PortalName && portal.PortalName.toLowerCase() == newScope.portalName.toLowerCase());
                });
                if (angular.isArray(objPortal) && objPortal.length > 0) {
                    newScope.ErrorMessage = "Error:Duplicate Portal Name.";
                    return true;
                } else {
                    return false;
                }
            }
        };
        newScope.closeDialog = function () {
            if (type == "edit") {
                $scope.selectedPortalDetails.PortalName = tempPortalName;
            }
            dialog.close();
        };
        var dialog = $rootScope.showDialog(newScope, newScope.title, "StartUp/views/add_new_portal_dialog.html", { height: 300, width: 450 });
    };

    $scope.deletePortal = function () {
        if ($scope.selectedPortalDetails && confirm("Are you sure you want to delete portal?")) {
            var index = $scope.selectedProject.htxPortals.indexOf($scope.selectedPortalDetails);
            if (index > -1) {
                $rootScope.DeleteItem($scope.selectedPortalDetails, $scope.selectedProject.htxPortals, "showPoratlDetails");
            }
            if (index < $scope.selectedProject.htxPortals.length) {
                $scope.showPoratlDetails($scope.selectedProject.htxPortals[index]);
            } else {
                $scope.showPoratlDetails($scope.selectedProject.htxPortals[$scope.selectedProject.htxPortals.length - 1]);
            }
        }
        if (angular.isArray($scope.selectedProject.htxPortals) && $scope.selectedProject.htxPortals.length <= 0) {
            $scope.showAddPortalDetailsTab = false;
        }
    };

    $scope.addFiles = function () {
        var newScope = $scope.$new(true);
        newScope.title = "Add New CSS";
        newScope.ErrorMessage = "";
        newScope.selectedAddedfile = null;
        newScope.lstCssFiles = [];
        newScope.cssFileList = [];
        newScope.selectedCssFileList = [];

        newScope.selectedAddedJsFiles = null;
        newScope.lstJsFiles = [];
        newScope.jsFileList = [];
        newScope.selectedJsFilesList = [];

        var dialog = null;
        if ($scope.selectedPortalDetails.CSSFiles) {
            var lstCss = $scope.selectedPortalDetails.CSSFiles.split(",");
            newScope.selectedCssFileList = lstCss;
        }
        if ($scope.selectedPortalDetails.JSFiles) {
            var lstJS = $scope.selectedPortalDetails.JSFiles.split(",");
            newScope.selectedJsFilesList = lstJS;
        }

        newScope.getFileList = function (fileExtention, selectedFileList, lstFiles) {
            if ($scope.selectedPortalDetails.PortalPath) {
                $rootScope.IsLoading = true;
                hubcontext.hubMain.server.getCssFiles($scope.selectedPortalDetails.PortalPath, fileExtention).done(function (data) {
                    newScope.$evalAsync(function () {
                        if (angular.isArray(data) && data.length == 1 && data[0] == "failure") {
                            toastr.warning("Select valid website location");
                        } else if (angular.isArray(data)) {
                            newScope.filterCssFiles(data, selectedFileList, lstFiles);
                        }
                        $rootScope.IsLoading = false;
                    });
                });
            }
        };
        newScope.getFileList('*.css', newScope.selectedCssFileList, newScope.lstCssFiles); // get all css files

        newScope.selectFileTab = function (fileTab) {
            newScope.selectedFilesTab = fileTab;
            newScope.selectedAddedfile = null;
            $("#selectedCssFiles").find("tr.selected").removeClass("selected");
            $("#selectedJsFiles").find("tr.selected").removeClass("selected");
            if ("JS" == fileTab && newScope.lstJsFiles.length == 0) {
                newScope.getFileList('*.js', newScope.selectedJsFilesList, newScope.lstJsFiles); // get all css files
            }
        };

        newScope.filterCssFiles = function (sourceList, targetList, mainList) {
            var tempCssFileList = sourceList.filter(function (val) {
                return targetList.indexOf(val) == -1;
            });

            angular.copy(tempCssFileList, mainList);

        };

        newScope.addCssFromWebConfig = function (fileList, selectedFileList) {
            if ($scope.selectedPortalDetails.PortalPath) {
                $rootScope.IsLoading = true;
                hubcontext.hubMain.server.getCssFileNameFromWebConfig($scope.selectedPortalDetails.PortalPath).done(function (data) {
                    newScope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                        if (angular.isArray(data) && data.length == 1 && data[0] == "failure") {
                            toastr.warning("Select valid website location");
                        } else if (angular.isArray(data)) {
                            selectedFileList.length = 0;
                            fileList.push.apply(selectedFileList, data);
                            selectedFileList = selectedFileList.unique();
                            newScope.selectedAddedfile = null;
                        }
                    });
                });
            }
        };
        newScope.selectCssFile = function (event, fileName, aCssFileList) {
            var element = null;
            if (event) {
                element = $(event.currentTarget);
                if (element && element.hasClass("selected")) {
                    element.removeClass("selected");
                    var index = aCssFileList.indexOf(fileName);
                    if (index > -1) {
                        aCssFileList.splice(index, 1);
                    }
                } else if (element && !element.hasClass("selected")) {
                    element.addClass("selected");
                    aCssFileList.push(fileName);
                }
            }
        };
        newScope.selectAllCssFile = function (option, aLstCssFiles, aCssFileList) {
            if (option) {
                $("#cssFileList").find("tr").addClass("selected");
                angular.copy(aLstCssFiles, aCssFileList);
            } else {
                $("#cssFileList").find("tr.selected").removeClass("selected");
                aCssFileList = [];
            }
        };
        newScope.addCssFiles = function (aSelectAllCssFiles, aSelectedCssFileList, aLstCssFiles, aFileType) {
            aSelectAllCssFiles = false;
            if (aFileType == "css") {
                aSelectedCssFileList.push.apply(aSelectedCssFileList, newScope.cssFileList);
            } else if (aFileType == "js") {
                aSelectedCssFileList.push.apply(aSelectedCssFileList, newScope.jsFileList);
            }

            var tempCssFileList = [];
            $.each(aSelectedCssFileList, function (i, el) {
                if ($.inArray(el, tempCssFileList) === -1) tempCssFileList.push(el);
            });
            angular.copy(tempCssFileList, aSelectedCssFileList);

            newScope.$evalAsync(function () {
                newScope.filterCssFiles(aLstCssFiles, aSelectedCssFileList, aLstCssFiles, "css");
                var domSelectedCssFiles = null;
                if (aFileType == "css") {
                    $("#cssFileList").find("tr.selected").removeClass("selected");
                    domSelectedCssFiles = $("#selectedCssFiles");
                } else if (aFileType == "js") {
                    $("#jsFileList").find("tr.selected").removeClass("selected");
                    domSelectedCssFiles = $("#selectedJsFiles");
                }
                domSelectedCssFiles.closest("div").scrollTop(domSelectedCssFiles.height());
                domSelectedCssFiles.find("tr.selected").removeClass("selected");
                $timeout(function () {
                    domSelectedCssFiles.find("tr:last").addClass("selected");
                    newScope.selectedAddedfile = aSelectedCssFileList[aSelectedCssFileList.length - 1];
                });
                newScope.cssFileList = [];
                newScope.jsFileList = [];
            });
        };
        newScope.selectAddedCssFile = function (event, fileName, aSelectedDomID) {
            var element = null;
            if (event) {
                newScope.selectedAddedfile = fileName;
                element = $(event.currentTarget);
                $(aSelectedDomID).find("tr.selected").removeClass("selected");
                if (element && element.hasClass("selected")) {
                    element.removeClass("selected");
                } else if (element && !element.hasClass("selected")) {
                    element.addClass("selected");
                }
            }
        };
        newScope.deleteCssFile = function (aSelectedCssFileList, aLstCssFiles, aListDomId) {
            if (newScope.selectedAddedfile) {
                var index = aSelectedCssFileList.indexOf(newScope.selectedAddedfile);
                if (index > -1) {
                    if (aLstCssFiles.indexOf(newScope.selectedAddedfile) < 0) {
                        aLstCssFiles.push(newScope.selectedAddedfile);
                    }
                    aSelectedCssFileList.splice(index, 1);
                    newScope.selectedAddedfile = null;
                    $timeout(function () {
                        if (index > aSelectedCssFileList.length - 1) {
                            index = index - 1;
                        }
                        var domRow = $(aListDomId).find("tr").eq(index);
                        if (domRow && !domRow.hasClass("selected")) {
                            domRow.addClass("selected");
                            newScope.selectedAddedfile = domRow.find("span").text();
                        }
                    });
                }
            }
        };
        newScope.canDeleteCssFile = function () {
            if (newScope.selectedAddedfile) {
                return true;
            } else {
                return false;
            }
        };
        newScope.canDeleteJsFile = function () {
            if (newScope.selectedAddedfile) {
                return true;
            } else {
                return false;
            }
        };

        newScope.onCssDialogOkClick = function () {
            if (angular.isArray(newScope.selectedCssFileList) && newScope.selectedCssFileList.length > 0) {
                $scope.selectedPortalDetails.CSSFiles = newScope.selectedCssFileList.join();
            } else {
                $scope.selectedPortalDetails.CSSFiles = "";
            }
            if (angular.isArray(newScope.selectedJsFilesList) && newScope.selectedJsFilesList.length > 0) {
                $scope.selectedPortalDetails.JSFiles = newScope.selectedJsFilesList.join();
            } else {
                $scope.selectedPortalDetails.JSFiles = "";
            }
            $scope.showPoratlDetails($scope.selectedPortalDetails);
            dialog.close();
        };
        newScope.closeDialog = function () {
            dialog.close();
        };
        dialog = $rootScope.showDialog(newScope, newScope.title, "StartUp/views/AddCssFiles.html", { height: 500, width: 750 });
    };

    $scope.portalMenuOptions = [
        ['Rename', function ($itemScope) {
            $scope.addNewPortal("edit");
        }, function ($itemScope) {
            if ($itemScope && $itemScope.selectedProject.UserType != "owner") {
                return false;
            } else {
                return true;
            }
        }],
        ["Set As Default", function ($itemScope) {
            $scope.setAsDefaultPortal();

            if ($itemScope.portal) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue($itemScope.portal.IsDefault, $itemScope.portal, "IsDefault", "true");
                $rootScope.UndRedoBulkOp("End");

            }
        }, function ($itemScope) {
            if ($itemScope && $itemScope.selectedProject.UserType != "owner") {
                return false;
            } else if ($itemScope.portal && $itemScope.portal.IsDefault == "true") {
                return false;
            } else {
                return true;
            }
        }],
        ["Unset Default", function ($itemScope) {
            if ($itemScope.portal && $itemScope.portal.IsDefault) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue($itemScope.portal.IsDefault, $itemScope.portal, "IsDefault", "false");
                $rootScope.UndRedoBulkOp("End");

            }
        }, function ($itemScope) {
            if ($itemScope && $itemScope.selectedProject.UserType != "owner") {
                return false;
            } else if ($itemScope.portal && $itemScope.portal.IsDefault == "true") {
                return true;
            }
        }]
    ];
    $scope.setAsDefaultPortal = function () {
        if ($scope.selectedProject && $scope.selectedProject.htxPortals.length) {
            for (var i = 0, len = $scope.selectedProject.htxPortals.length; i < len; i++) {
                var portal = $scope.selectedProject.htxPortals[i];
                if (portal && portal.IsDefault) {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue(portal.IsDefault, portal, "IsDefault", "false");
                    $rootScope.UndRedoBulkOp("End");
                }
            }

        }
    };
    $scope.selectedCssFile = null;
    $scope.selectedJsFile = null;
    $scope.selectPortalCssFile = function (event, file) {
        var element = null;
        if (event) {
            $scope.selectedCssFile = file;
            element = $(event.currentTarget);
            $("#cssPortalFileList").find("tr.selected").removeClass("selected");
            if (element && element.hasClass("selected")) {
                element.removeClass("selected");
            } else if (element && !element.hasClass("selected")) {
                element.addClass("selected");
            }
        }
    };
    $scope.canDeleteCssFile = function () {
        if ($scope.selectedCssFile) {
            return true;
        } else {
            return false;
        }
    };
    $scope.canMoveCssFileUp = function () {
        if ($scope.canDeleteCssFile() && $scope.selectedCssFile) {
            var index = $scope.lstPortalCss.indexOf($scope.selectedCssFile);
            if (index > 0) {
                return true;
            }
        } else {
            return false;
        }
    };
    $scope.canMoveCssFileDown = function () {
        if ($scope.canDeleteCssFile() && $scope.selectedCssFile) {
            var index = $scope.lstPortalCss.indexOf($scope.selectedCssFile);
            if (index < $scope.lstPortalCss.length - 1) {
                return true;
            }
        } else {
            return false;
        }
    };
    $scope.moveCssFileNameUpOrDown = function (direction) {
        if ($scope.selectedCssFile) {
            $rootScope.UndRedoBulkOp("Start");
            var index = $scope.lstPortalCss.indexOf($scope.selectedCssFile);
            $rootScope.DeleteItem($scope.selectedCssFile, $scope.lstPortalCss);
            if (direction == "up") {
                index -= 1;
            } else {
                index += 1;
            }
            $rootScope.InsertItem($scope.selectedCssFile, $scope.lstPortalCss, index);
            $scope.selectedPortalDetails.CSSFiles = $scope.lstPortalCss.join();
            $rootScope.UndRedoBulkOp("End");
            $timeout(function () {
                var parentObj = $("#cssPortalFileList").closest(".entity-dashboard-inner-section-container");
                parentObj.scrollTo(parentObj.find("tr.selected"), { offsetTop: 130, offsetLeft: 0 }, null);
            });
        }
    };

    $scope.deleteCssFile = function () {
        if ($scope.selectedCssFile) {
            var index = $scope.lstPortalCss.indexOf($scope.selectedCssFile);
            if (index > -1) {
                $rootScope.DeleteItem($scope.selectedCssFile, $scope.lstPortalCss);
                $scope.selectedPortalDetails.CSSFiles = $scope.lstPortalCss.join();
                $scope.selectedCssFile = null;
                $timeout(function () {
                    if (index > $scope.lstPortalCss.length - 1) {
                        index = index - 1;
                    }
                    var domRow = $("#cssPortalFileList").find("tr").eq(index);
                    if (domRow && !domRow.hasClass("selected")) {
                        domRow.addClass("selected");
                        $scope.selectedCssFile = domRow.find("span").text();
                    }
                });
            }
        }
    };

    $scope.selectPortalJsFile = function (event, file) {
        var element = null;
        if (event) {
            $scope.selectedJsFile = file;
            element = $(event.currentTarget);
            $("#jsPortalFileList").find("tr.selected").removeClass("selected");
            if (element && element.hasClass("selected")) {
                element.removeClass("selected");
            } else if (element && !element.hasClass("selected")) {
                element.addClass("selected");
            }
        }
    };
    $scope.canDeleteJsFile = function () {
        if ($scope.selectedJsFile) {
            return true;
        } else {
            return false;
        }
    };
    $scope.canMoveJsFileUp = function () {
        if ($scope.canDeleteJsFile() && $scope.selectedJsFile) {
            var index = $scope.lstPortalJs.indexOf($scope.selectedJsFile);
            if (index > 0) {
                return true;
            }
        } else {
            return false;
        }
    };
    $scope.canMoveJsFileDown = function () {
        if ($scope.canDeleteJsFile() && $scope.selectedJsFile) {
            var index = $scope.lstPortalJs.indexOf($scope.selectedJsFile);
            if (index < $scope.lstPortalJs.length - 1) {
                return true;
            }
        } else {
            return false;
        }
    };
    $scope.moveJsFileNameUpOrDown = function (direction) {
        if ($scope.selectedJsFile) {
            $rootScope.UndRedoBulkOp("Start");
            var index = $scope.lstPortalJs.indexOf($scope.selectedJsFile);
            $rootScope.DeleteItem($scope.selectedJsFile, $scope.lstPortalJs);
            if (direction == "up") {
                index -= 1;
            } else {
                index += 1;
            }
            $rootScope.InsertItem($scope.selectedJsFile, $scope.lstPortalJs, index);
            $scope.selectedPortalDetails.JSFiles = $scope.lstPortalJs.join();
            $rootScope.UndRedoBulkOp("End");
            $timeout(function () {
                var parentObj = $("#jsPortalFileList").closest(".entity-dashboard-inner-section-container");
                parentObj.scrollTo(parentObj.find("tr.selected"), { offsetTop: 130, offsetLeft: 0 }, null);
            });
        }
    };

    $scope.deleteJsFile = function () {
        if ($scope.selectedJsFile) {
            var index = $scope.lstPortalJs.indexOf($scope.selectedJsFile);
            if (index > -1) {
                $rootScope.DeleteItem($scope.selectedJsFile, $scope.lstPortalJs);
                $scope.selectedPortalDetails.JSFiles = $scope.lstPortalJs.join();
                $scope.selectedJsFile = null;
                $timeout(function () {
                    if (index > $scope.lstPortalJs.length - 1) {
                        index = index - 1;
                    }
                    var domRow = $("#jsPortalFileList").find("tr").eq(index);
                    if (domRow && !domRow.hasClass("selected")) {
                        domRow.addClass("selected");
                        $scope.selectedJsFile = domRow.find("span").text();
                    }
                });
            }
        }
    };


    // #endregion
}]);


var ExecuteQueryCount = 1;
app.controller('executequerycontroller', ["$scope", "$rootScope", "$interval", function ($scope, $rootScope, $interval) {

    hubMain.server.getExecuteQueryDetails().done(function (data) {

        $scope.$apply(function () {
            $scope.QueryHeader = "Execute Query" + "(" + ExecuteQueryCount++ + ")";
            $scope.QueryDivID = "ExecuteQuery" + ExecuteQueryCount;
            //if ($rootScope.IsOracleDB) {
            //    $scope.lstExecuteQueryTables = [];
            //    if (data && data.length > 0) {
            //        for (var i = 0, len = data.length; i < len; i++) {
            //            $scope.lstExecuteQueryTables.push({ name: data[i].NAME });
            //        }
            //    }
            //}
            //else {
            $scope.lstExecuteQueryTables = data;
            //}
            $scope.Query = "";
            $scope.TableName = "";
            $scope.chkbxFetchRows = true;
            $scope.setValueToQueryEditor();
        });
    });

    $scope.setValueToQueryEditor = function () {
        if (!$scope.QueryEditor) {
            var divId = "Queryeditor_" + $scope.QueryDivID;
            var ExecuteQuerypromise = $interval(function () {
                if ($("#" + divId).length > 0) {
                    $scope.QueryEditor = ace.edit(divId);
                    $scope.QueryEditor.getSession().setMode("ace/mode/sql");
                    $scope.QueryEditor.resize(true);
                    $scope.QueryEditor.setFontSize(13);
                    $scope.QueryEditor.renderer.setShowGutter(false);
                    $scope.QueryEditor.getSession().setValue($scope.Query);
                    $scope.QueryEditor.getSession().on('change', function (e) {
                        var strQuery = $scope.QueryEditor.getValue();
                        if ($scope.QueryEditor.curOp && $scope.QueryEditor.curOp.command.name) {
                            $scope.Query = strQuery;
                        } else if (strQuery && strQuery != $scope.Query) {
                            //In ace editor if we set value using setValue function change event will fire for that we are checking editor data and current query data
                            $scope.setValueToQueryEditor();
                        }
                    });
                    $interval.cancel(ExecuteQuerypromise);
                }
            }, 500);
        }
        else if ($scope.QueryEditor) {
            $scope.QueryEditor.getSession().setValue($scope.Query);
        }

    };

    $scope.selectedTab = "data";
    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };

    $scope.selectedTable = undefined;
    $scope.selectTable = function (table) {
        $scope.selectedTable = table;
    };
    $scope.onChange = function (val) {
        $scope.chkbxFetchRows = val;
    }
    $scope.ExecutetableQuery = function (btn) {
        var strQuery;
        $scope.btn = btn;

        var returnResult;
        if ($scope.QueryEditor) {
            returnResult = $scope.QueryEditor.getSelectedText();
            if (!returnResult) {
                returnResult = $scope.QueryEditor.getValue();
            }
        }
        if (returnResult) {
            strQuery = returnResult;
        }

        if ($scope.chkbxFetchRows && returnResult && !$rootScope.IsOracleDB) {

            if (returnResult.indexOf("select") > -1) {
                strQuery = [returnResult.slice(0, returnResult.toLowerCase().indexOf("select") + 7), " Top 50 ", returnResult.slice(returnResult.toLowerCase().indexOf("select") + 7)].join('');

            }

        }

        if (strQuery) {
            $rootScope.IsLoading = true;
            hubMain.server.executeQueryTool(strQuery, true, "");
        }
        else {
            $scope.lstExecuteQuery1 = undefined;
        }
    };

    $scope.getQuery = function (caretPositionStart, caretPositionEnd) {
        var line;
        var txtarea;
        var txtQuery = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#taQuery");
        txtarea = txtQuery[0];

        if (caretPositionStart == caretPositionEnd) {
            //Text Not selected , Only click
            var LineNumber = txtarea.value.substr(0, txtarea.selectionStart).split("\n").length;
            var lines = $("div[id='" + $rootScope.currentopenfile.file.FileName + "']").find("#taQuery").val().split('\n');
            line = lines[LineNumber - 1];
            return line;
        }
        else {
            //Text selected
            line = txtarea.value.substring(caretPositionStart, caretPositionEnd);
            return line.replace("\n", "").replace("\r", "");
        }
    };

    $scope.receieveExecuteQuery = function (data) {
        $scope.$evalAsync(function () {
            $scope.lstdata = JSON.parse(data);
            $rootScope.IsLoading = false;
            if ($scope.btn == 'btn1') {
                $scope.lstExecuteQuery1 = $scope.lstdata;
            }
            else {
                $scope.lstExecuteQuery2 = $scope.lstdata;
            }
        });
    };


    $scope.RunAll = function () {
        if ($scope.Query != undefined && $scope.Query != "") {
            var strQuery = $scope.Query;

            hubMain.server.runAllQueries(strQuery).done(function (data) {
                $scope.$apply(function () {
                    $scope.strErrorMsgdata = data;
                    alert("Complete");
                });
            });
        }
    };



}]);


app.directive('customtabledraggable', [function () {
    return {
        restrict: 'A',
        scope: {
            dragdata: '='
        },
        link: function (scope, element, attributes) {
            var el = element[0];
            if (el.localName == "li") {
                //alert(el.localName);
                //alert(scope.dragdata);
            }
            el.draggable = true;

            el.addEventListener('dragstart', handleDragStart, false);
            function handleDragStart(e) {
                if (scope.dragdata != undefined && scope.dragdata != '') {
                    e.dataTransfer.setData("text", JSON.stringify(scope.dragdata));
                    dragDropData = scope.dragdata;
                }
            }
        },
    };
}]);


app.directive("droptable", [function () {
    return {
        restrict: "A",
        scope: {
            dropdata: '=',
            //  selectedRuleFromGroup: '='
        },
        link: function (scope, element, attributes) {

            var el = element[0];
            el.addEventListener("dragover", DragOver, false);
            el.addEventListener("drop", Drop, false);
            //el.addEventListener('dragleave', DragLeave, false);


            function DragOver(e) {
                e.dataTransfer.dropEffect = 'copy';

                if (scope.dropdata != undefined && scope.dropdata != '') {
                    if (e.preventDefault) {
                        e.preventDefault();
                    }
                }

                return false;

            }
            function Drop(e) {
                //var Scope; 
                //var currentelement = $('div[ng-controller="executequerycontroller"]');
                //if (currentelement && currentelement.length > 0) {
                //    Scope=angular.element(currentelement).scope();
                //}
                var Scope = getCurrentFileScope();
                //var Scope = getCurrentControllerScope();
                e.preventDefault();
                var data = dragDropData;
                Scope.$apply(function () {
                    if (data) {
                        Scope.TableName = data.name;
                        if (Scope.Query == "") {
                            Scope.Query += "select * from" + " " + data.name + ";";
                        }
                        else {
                            Scope.Query += "\r\n" + "select * from" + " " + data.name + ";";
                        }
                        if (Scope.setValueToQueryEditor) {
                            Scope.setValueToQueryEditor();
                        }
                        dragDropData = null;
                    }
                });

                if (e.stopPropagation) e.stopPropagation();
            }

            function DragLeave(e) {

            }

        }
    };
}]);
app.controller('auditlogcontroller', ["$scope", "$http", "$rootScope", "$timeout", function ($scope, $http, $rootScope, $timeout) {
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.selectedDesignSource = false;
    $rootScope.IsLoading = true;
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveauditlogmodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receiveauditlogmodel = function (data) {
        $scope.AuditlogFiledata = data;
        $scope.tableCount = 30;
        $scope.$evalAsync(function () {
            $scope.Auditlogtableslist = data.objExtraData;
            $rootScope.IsLoading = false;
            $scope.Getlistoftable();
        });
    };

    //#region Design-Source Xml
    $scope.selectedDesignSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            $scope.selectedDesignSource = false;
            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
                if ($scope.isSourceDirty) {
                    $rootScope.IsLoading = true;
                    if (xmlstring.length < 32000) {
                        hubMain.server.getXmlString(xmlstring, $scope.currentfile);
                    }
                    else {
                        var strpacket = "";
                        var lstDataPackets = [];
                        var count = 0;
                        for (var i = 0; i < xmlstring.length; i++) {
                            count++;
                            strpacket = strpacket + xmlstring[i];
                            if (count == 32000) {
                                count = 0;
                                lstDataPackets.push(strpacket);
                                strpacket = "";
                            }
                        }
                        if (count != 0) {
                            lstDataPackets.push(strpacket);
                        }

                        SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "SourceToDesignCommon");
                    }
                    $scope.receivexmlobject = function (data) {
                        if ($scope.isSourceDirty) {
                            $scope.receiveauditlogmodel(data);
                            $scope.isSourceDirty = false;
                        }
                    };
                }
            }
        }
    };
    $scope.isSourceDirty;
    $scope.sourceChanged = function () {
        $scope.isSourceDirty = true;
        $scope.isDirty = true;
        $scope.SearchSource.IsSearchCriteriaChange = true;
    };
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, null);
        }
    };
    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            $scope.updateAuditLogFiledata();
            if ($scope.AuditlogFiledata) {
                $rootScope.IsLoading = true;

                var objreturn1 = GetBaseModel($scope.AuditlogFiledata);

                if (objreturn1 != "") {

                    var strobj = JSON.stringify(objreturn1);
                    if (strobj.length < 32000) {
                        hubMain.server.getXmlObject(strobj, $scope.currentfile);
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
                        SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "DesignToSourceCommon");
                    }
                    $scope.receivexml = function (xmlstring) {
                        $scope.$apply(function () {
                            $scope.xmlSource = xmlstring;
                            var ID = "auditlog";
                            setDataToEditor($scope, xmlstring, 1, ID);
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });
                            if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                                $scope.$evalAsync(function () {
                                    $rootScope.IsLoading = false;
                                });
                            }
                        });
                    };
                }
            }
        }
    };
    //#endregion

    $scope.BeforeSaveToFile = function () {
        $scope.updateAuditLogFiledata();
    };
    $scope.updateAuditLogFiledata = function () {
        $scope.AuditlogFiledata.Elements = [];
        var users = {
            Name: 'users', value: '', dictAttributes: { Include: "*" }, Elements: [], Children: []
        };
        $scope.AuditlogFiledata.Elements.push(users);

        var itemsToBeAdded = $scope.Auditlogtableslist.filter(function (item) { return item.IsExcluded || item.OnInsert || item.OnUpdate || item.OnDelete; });

        var excludeModel;
        if (itemsToBeAdded.some(function (item) { return item.IsExcluded; })) {
            var fullauditlog = {
                Name: 'fullauditlog', value: '', dictAttributes: {}, Elements: [], Children: []
            };
            excludeModel = {
                Name: 'Exclude', value: '', dictAttributes: {}, Elements: [], Children: []
            };
            fullauditlog.Elements.push(excludeModel);
            $scope.AuditlogFiledata.Elements.push(fullauditlog);
        }

        for (var i = 0; i < itemsToBeAdded.length; i++) {
            var obj = itemsToBeAdded[i];
            if (obj.OnInsert || obj.OnUpdate || obj.OnDelete) {
                var columnNames = obj.lstAuditTableDetails.filter(function (item) {
                    if (obj.IncludeAll) {
                        return !item.IsAudit;
                    }
                    else {
                        return item.IsAudit;
                    }
                }).map(function (x) { return x.ColumnName; });
                var table = {
                    Name: 'table', value: '', dictAttributes: { name: obj.TableName, OnInsert: obj.OnInsert, OnUpdate: obj.OnUpdate, OnDelete: obj.OnDelete, Exclude: obj.IncludeAll ? columnNames.join(";") : "*", Include: obj.IncludeAll ? "*" : columnNames.join(";") }, Elements: [], Children: []
                };
                $scope.AuditlogFiledata.Elements.push(table);
            }
            if (obj.IsExcluded && excludeModel) {
                var table = {
                    Name: 'table', value: '', dictAttributes: { name: obj.TableName }, Elements: [], Children: []
                };
                excludeModel.Elements.push(table);
            }
        }
    };

    //$scope.receiveAuditLogDetails = function (data) {
    //    $scope.$evalAsync(function () {
    //        $scope.Auditlogtableslist = JSON.parse(data);
    //        $rootScope.IsLoading = false;
    //        $scope.Getlistoftable();
    //    });
    //}

    $scope.selectTable = function (table) {
        $scope.selectedTable = table;
        $scope.selectedTable.columnCount = 30;
    };

    $scope.btns = [{
        label: "On Insert",
        state: false,
    }, {
        label: "On Update",
        state: false,
    }, {
        label: "On Delete",
        state: false,
    }, {
        label: "On Excluded",
        state: false,
    }];
    $scope.toggle = function (button) {
        button.state = !button.state;
        $scope.Getlistoftable();
    };

    $scope.Getlistoftable = function () {
        $rootScope.IsLoading = true;
        $scope.tableCount = 30;
        $scope.lstTables = [];
        if ($scope.btns[0].state || $scope.btns[1].state || $scope.btns[2].state || $scope.btns[3].state) {
            for (var i = 0; i < $scope.Auditlogtableslist.length; i++) {
                if ($scope.lstTables.indexOf($scope.Auditlogtableslist[i]) == -1) {
                    if ($scope.Auditlogtableslist[i].OnInsert == $scope.btns[0].state && $scope.btns[0].state == true) {
                        $scope.lstTables.push($scope.Auditlogtableslist[i]);
                    }
                    else if ($scope.Auditlogtableslist[i].OnUpdate == $scope.btns[1].state && $scope.btns[1].state == true) {
                        $scope.lstTables.push($scope.Auditlogtableslist[i]);
                    }
                    else if ($scope.Auditlogtableslist[i].OnDelete == $scope.btns[2].state && $scope.btns[2].state == true) {
                        $scope.lstTables.push($scope.Auditlogtableslist[i]);
                    }
                    else if ($scope.Auditlogtableslist[i].IsExcluded == $scope.btns[3].state && $scope.btns[3].state == true) {
                        $scope.lstTables.push($scope.Auditlogtableslist[i]);
                    }
                }
            }
        }
        else {
            $scope.lstTables = $scope.Auditlogtableslist;
        }
        $rootScope.IsLoading = false;
    };
    $scope.toggleSelectAll = function () {
        if ($scope.selectedTable && $scope.selectedTable.lstAuditTableDetails) {
            for (var i = 0; i < $scope.selectedTable.lstAuditTableDetails.length; i++) {
                $scope.selectedTable.lstAuditTableDetails[i].IsAudit = $scope.selectedTable.IncludeAll;
            }
        }
    };
    $scope.onTableScroll = function () {
        if ($scope.tableCount < $scope.Auditlogtableslist.length) {
            if ($scope.tableCount + 20 < $scope.Auditlogtableslist.length) {
                $scope.$apply(function () {
                    $scope.tableCount = $scope.tableCount + 20;
                });
            }
            else {
                $scope.$apply(function () {
                    $scope.tableCount = $scope.Auditlogtableslist.length;
                });
            }
        }
    };
    $scope.onColumnScroll = function () {
        if ($scope.selectedTable.columnCount < $scope.selectedTable.lstAuditTableDetails.length) {
            if ($scope.selectedTable.columnCount + 10 < $scope.selectedTable.lstAuditTableDetails.length) {
                $scope.$evalAsync(function () {
                    $scope.selectedTable.columnCount = $scope.selectedTable.columnCount + 10;
                });
            }
            else {
                $scope.$evalAsync(function () {
                    $scope.selectedTable.columnCount = $scope.selectedTable.lstAuditTableDetails.length;
                });
            }
        }
    };
}]);

app.controller("projectconfigurationcontroller", ["$scope", "$rootScope", "$EntityIntellisenseFactory", "$getEnitityXmlMethods", "$filter", "$timeout", function ($scope, $rootScope, $EntityIntellisenseFactory, $getEnitityXmlMethods, $filter, $timeout) {
    //#region Varaibles
    $scope.objCommonproperties = {};
    $scope.stdBookmarksModel = {};
    $scope.objServerMethods = {};
    $scope.remoteObjCollection = {};
    $scope.objBusinessObject = {};
    $scope.objBusinessObject.ObjTree = undefined;
    $scope.objBusinessObject.BusObjectName = "";
    $scope.isSourceDirty = false;
    $scope.serverMethodFilters = { isServiceFilterVisible: false, isServerMethodFilterVisible: false };
    //Added a collection of string, to maintain the names of loaded tab contents.
    $scope.loadedTabs = [];

    //#endregion

    //#region On Load
    $scope.initialize = function () {
        $rootScope.IsLoading = true;
        $scope.currentfile = $rootScope.currentopenfile.file;
        hubMain.server.getModel($rootScope.currentopenfile.file.FilePath, $rootScope.currentopenfile.file.FileType).done(function (data) {
            if (data) {
                $scope.receiveProjectConfigurationmodel(data);
            }
            else {
                $rootScope.closeFile($scope.currentfile.FileName);
            }

        });
        function setRemoteObjectCollection(data) {
            if (data) {
                $scope.remoteObjCollection = data;
            }
        }
        hubMain.server.getRemoteObjectCollection().done(setRemoteObjectCollection);
    };

    $scope.receiveProjectConfigurationmodel = function (data) {
        $scope.$evalAsync(function () {
            $scope.projectconfigurationfiledata = data;

            //Check and create commonproperties model
            var items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "commonproperties" }, true);
            if (items && items.length > 0) {
                $scope.objCommonproperties = items[0];
            }
            else {
                $scope.objCommonproperties = $scope.CreateNewObject("commonproperties", $scope.projectconfigurationfiledata);
            }

            //Check and create properties model
            items = $filter("filter")($scope.objCommonproperties.Elements, { Name: "properties" }, true);
            if (items && items.length > 0) {
                $scope.propertiesModel = items[0];
            }
            else {
                $scope.propertiesModel = $scope.CreateNewObject("properties", $scope.objCommonproperties);
            }

            //Check and create correspondence model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "correspondence" }, true);
            if (items && items.length > 0) {
                $scope.correspondenceModel = items[0];
            }
            else {
                $scope.correspondenceModel = $scope.CreateNewObject("correspondence", $scope.projectconfigurationfiledata);
            }

            //Check and create stdbookmarks model
            items = $filter("filter")($scope.correspondenceModel.Elements, { Name: "stdbookmarks" }, true);
            if (items && items.length > 0) {
                $scope.stdBookmarksModel = items[0];
            }
            else {
                $scope.stdBookmarksModel = $scope.CreateNewObject("stdbookmarks", $scope.correspondenceModel);
            }

            //Check and create servermethods model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "servermethods" }, true);
            if (items && items.length > 0) {
                $scope.objServerMethods = items[0];
            }
            else {
                $scope.objServerMethods = $scope.CreateNewObject("servermethods", $scope.projectconfigurationfiledata);
            }

            //Check and create globalparameters model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "globalparameters" }, true);
            if (items && items.length > 0) {
                $scope.globalParametersModel = items[0];
            }
            else {
                $scope.globalParametersModel = $scope.CreateNewObject("globalparameters", $scope.projectconfigurationfiledata);
            }

            //Check and create portals model
            items = $filter("filter")($scope.projectconfigurationfiledata.Elements, { Name: "applicationportals" }, true);
            if (items && items.length > 0) {
                $scope.portalsModel = items[0];
            }
            else {
                $scope.portalsModel = $scope.CreateNewObject("applicationportals", $scope.projectconfigurationfiledata);
            }


            for (var index = 0; index < $scope.stdBookmarksModel.Elements.length; index++) {
                //$scope.loadXmlMethods($scope.stdBookmarksModel.Elements[index]);
                $scope.onCorrespondanceEntityChanged($scope.stdBookmarksModel.Elements[index]);
            }

            $scope.selectCommonPropertiesTab();
            // if server method groups are present - select the first method group
            if ($scope.objServerMethods.Elements && $scope.objServerMethods.Elements.length > 0) {
                $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[0]);
            }

            $scope.validatePortals();

            $rootScope.IsLoading = false;
        });
    };

    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };


    //#endregion

    //#region Xml source 
    $scope.showSource = false;
    $scope.selectedDesignSource = false;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            $scope.selectedDesignSource = false;
            var xmlstring = $scope.editor.getValue();
            var lineno = $scope.editor.selection.getCursor().row;
            lineno = lineno + 1;
            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $rootScope.currentopenfile.file, lineno);
                }
                else {
                    var lineNumber = [];
                    if (lineno > 0) {
                        lineNumber[0] = lineno;
                    }
                    else {
                        lineNumber[0] = 1;
                    }
                    var strpacket = "";
                    var lstDataPackets = [];
                    var count = 0;
                    for (var i = 0; i < xmlstring.length; i++) {
                        count++;
                        strpacket = strpacket + xmlstring[i];
                        if (count == 32000) {
                            count = 0;
                            lstDataPackets.push(strpacket);
                            strpacket = "";
                        }
                    }
                    if (count != 0) {
                        lstDataPackets.push(strpacket);
                    }

                    //  SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "SourceToDesignCommon");
                    SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Source-Design", lineNumber);
                }
                $scope.receivedesignxmlobject = function (data, path) {
                    if ($scope.isSourceDirty) {
                        $scope.receiveProjectConfigurationmodel(data);
                        $scope.$evalAsync(function () {
                            $scope.selectItemByPath(path);
                        });
                        $scope.isSourceDirty = false;
                    } else {
                        $scope.selectItemByPath(path);
                    }
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                };
            }
        }
    };

    $scope.sourceChanged = function () {
        $scope.isSourceDirty = true;
        $scope.isDirty = true;
        $scope.SearchSource.IsSearchCriteriaChange = true;
    };
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };

    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            if ($scope.projectconfigurationfiledata != null && $scope.projectconfigurationfiledata != undefined) {
                $rootScope.IsLoading = true;
                var objreturn1 = GetBaseModel($scope.projectconfigurationfiledata);

                if (objreturn1 != "") {
                    var nodeId = [];
                    var selectedItem;
                    $rootScope.IsLoading = true;
                    var sObj;
                    var indexPath = [];
                    var pathString;
                    var selectedItem = $scope.getSelectedItem();

                    if (selectedItem) {
                        var pathToObject = [];
                        sObj = $scope.FindDeepNode($scope.projectconfigurationfiledata, selectedItem, pathToObject);
                        pathString = $scope.getPathSource($scope.projectconfigurationfiledata, pathToObject, indexPath);
                        angular.copy(pathString, nodeId);
                    }

                    var strobj = JSON.stringify(objreturn1);
                    if (strobj.length < 32000) {
                        //hubMain.server.getXmlObject(strobj, $rootScope.currentopenfile.file);
                        hubMain.server.getSourceXmlObject(strobj, $rootScope.currentopenfile.file, nodeId);
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
                        SendDataPacketsToServer(lstDataPackets, $rootScope.currentopenfile.file, "Design-Source", nodeId);

                    }
                    $scope.receivesourcexml = function (xmlstring, lineno) {
                        $scope.$apply(function () {
                            $scope.xmlSource = xmlstring;
                            var ID = "projectconfiguration";
                            setDataToEditor($scope, xmlstring, lineno, ID);
                            $scope.$evalAsync(function () {
                                $rootScope.IsLoading = false;
                            });

                            if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                                $scope.$evalAsync(function () {
                                    $rootScope.IsLoading = false;
                                });
                            }
                        });
                    };
                }
            }
        }
    };

    $scope.getSelectedItem = function () {
        var item = null;
        //select common property
        switch ($scope.SelectedTab) {
            case "CommonProperties":
                if ($scope.selectedCommonProperty) {
                    item = $scope.selectedCommonProperty;
                }
                if ($scope.selectedObject) {
                    if ($scope.selectedObjectItem) {
                        item = $scope.selectedObjectItem;
                    } else {
                        item = $scope.selectedObject;
                    }
                }
                break;
            case "Correspondance":
                if ($scope.selectedStandardBookmark) {
                    if ($scope.selectedStandardBookmarkItem) {
                        item = $scope.selectedStandardBookmarkItem;
                    } else {
                        item = $scope.selectedStandardBookmark;
                    }
                }
                break;
            case "ServerMethods":
                if ($scope.currentSrvMethod) {
                    if ($scope.currentMethod) {
                        item = $scope.currentMethod;
                    } else {
                        item = $scope.currentSrvMethod;
                    }
                }
                break;
            case "GlobalParameters":
                if ($scope.selectedGlobalParameter) {
                    item = $scope.selectedGlobalParameter;
                }
                break;
            default:
                item = $scope.selectedCommonProperty;
        }
        return item;
    };

    $scope.selectItemByPath = function (path) {
        console.log(path);
        if (path != null && path != "") {
            var items = [];
            var objHierarchy;
            if (path.contains("-") || path.contains(",")) {
                objHierarchy = $scope.projectconfigurationfiledata;
                for (var i = 0; i < path.split(',').length; i++) {
                    objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                    items.push(objHierarchy);
                }
            }

            if (items.length > 1) {
                for (var i = 0; i < items.length; i++) {
                    // commonproperties-0,object-2,item-1
                    if (items[i].Name == "commonproperties") {
                        $scope.selectCommonPropertiesTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && i > 0 && items[i - 1].Name == "properties" && items[i].Name == "item") {
                        $scope.selectCommonProperty(items[i]);
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && items[i].Name == "object") {
                        $scope.selectObject(items[i]);
                        $scope.toggleObjectExpander(items[i], true);
                        continue;
                    }
                    if ($scope.SelectedTab == "CommonProperties" && i > 0 && items[i - 1].Name == "object" && items[i].Name == "item") {
                        $scope.selectObjectItem(items[i - 1], items[i]);
                        continue;
                    }
                    // correspondence - 2, stdbookmarks - 0, object - 0, item - 1
                    if (items[i].Name == "correspondence") {
                        $scope.selectCorrespondanceTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && items[i].Name == "stdbookmarks") {
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && i > 0 && items[i - 1].Name == "stdbookmarks" && items[i].Name == "object") {
                        $scope.selectStandardBookmark(items[i]);
                        $scope.toggleStandardBookmarkExpander(items[i], true);
                        continue;
                    }
                    if ($scope.SelectedTab == "Correspondance" && i > 1 && items[i - 1].Name == "object" && items[i].Name == "item") {
                        $scope.selectStandardBookmarkItem(items[i - 1], items[i]);
                        continue;
                    }
                    //servermethods-1,methodgroup-7,method-1,parameter-0
                    if (items[i].Name == "servermethods") {
                        $scope.selectServerMethods();
                        continue;
                    }
                    if ($scope.SelectedTab == "ServerMethods" && items[i].Name == "methodgroup") {
                        $scope.getCurrentSrvMethod(items[i]);
                        continue;
                    }
                    if ($scope.SelectedTab == "ServerMethods" && i > 0 && items[i - 1].Name == "methodgroup" && items[i].Name == "method") {
                        $scope.getCurrentMethod(items[i]);
                        continue;
                    }
                    //globalparameters-3,globlarparameter-1
                    if (items[i].Name == "globalparameters") {
                        $scope.selectGlobalParametersTab();
                        continue;
                    }
                    if ($scope.SelectedTab == "GlobalParameters" && items[i].Name == "globlarparameter") {
                        $scope.selectGlobalParameter(items[i]);
                        continue;
                    }
                }
            }
            else if (items.length == 1) {
                $scope.selectCommonPropertiesTab();
                $scope.selectCommonProperty(items[0]);
            }
        }
    }

    $scope.FindNodeHierarchy = function (objParentElements, index) {
        if (objParentElements && objParentElements.Elements) {
            var newObj = objParentElements.Elements[index];
            if (newObj == undefined) {
                newObj = objParentElements.Elements[index - 1];
            }
            return newObj;
        }
    };
    $scope.FindDeepNode = function (objParentElements, selectedItem, pathToObject) {
        if (objParentElements) {
            angular.forEach(objParentElements.Elements, function (item) {
                //item.ParentVM = objParentElements
                var isNodeInPath = $scope.isValidObject(item, selectedItem);
                if (isNodeInPath) {
                    pathToObject.push(item);
                }
                if (item == selectedItem) {
                    return selectedItem;
                }
                else if (item.Elements && item.Elements.length > 0) {
                    selectedItem = $scope.FindDeepNode(item, selectedItem, pathToObject);
                    return selectedItem;
                }
            });
        }
        return selectedItem;
    };

    $scope.isValidObject = function (objParentElements, selectedItem) {
        var result;
        if (objParentElements == selectedItem) {
            result = true;
            return result;
        }

        for (var ele in objParentElements.Elements) {
            if (objParentElements.Elements[ele] == selectedItem) {
                result = true;
                return result;
            }
            if (objParentElements.Elements[ele].Elements && objParentElements.Elements[ele].Elements.length > 0) {
                for (iele in objParentElements.Elements[ele].Elements) {
                    result = $scope.isValidObject(objParentElements.Elements[ele].Elements[iele], selectedItem);
                    if (result == true) {
                        return result;
                    }
                }
            }
        }
        return result;
    };

    $scope.getPathSource = function (objModel, pathToObject, indexPath) {
        for (var i = 0; i < pathToObject.length; i++) {
            if (i == 0) {
                var indx = objModel.Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
            else {
                var indx = pathToObject[i - 1].Elements.indexOf(pathToObject[i]);
                indexPath.push(indx);
            }
        }
        return indexPath;
    };
    //#endregion

    //#region Common Methods

    $scope.selectCommonPropertiesTab = function () {
        $scope.SelectedTab = "CommonProperties";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only.
        if ($scope.loadedTabs.indexOf("CommonProperties") == -1) {
            $scope.loadedTabs.push("CommonProperties");
        }
        //if ($scope.objCommonproperties && $scope.objCommonproperties.Elements) {
        //    $scope.loadObjectTreeForExpandedObject($scope.objCommonproperties.Elements);
        //}
    };
    $scope.selectCorrespondanceTab = function () {
        $scope.SelectedTab = "Correspondance";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("Correspondance") == -1) {
            $scope.loadedTabs.push("Correspondance");
        }
        //if ($scope.stdBookmarksModel && $scope.stdBookmarksModel.Elements) {
        //    $scope.loadObjectTreeForExpandedObject($scope.stdBookmarksModel.Elements);
        //}
    };
    $scope.selectServerMethods = function () {
        $scope.SelectedTab = "ServerMethods";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("ServerMethods") == -1) {
            $scope.loadedTabs.push("ServerMethods");
        }
    };
    $scope.selectGlobalParametersTab = function () {
        $scope.SelectedTab = "GlobalParameters";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("GlobalParameters") == -1) {
            $scope.loadedTabs.push("GlobalParameters");
        }
    };
    $scope.selectPortalsTab = function () {
        $scope.SelectedTab = "Portals";
        //When the tab is first time selected tab name will be added to loadedTabs list and content will be loaded.
        //Next time onwards it won't load the content again, but will show the loaded content only. 
        if ($scope.loadedTabs.indexOf("Portals") == -1) {
            $scope.loadedTabs.push("Portals");
        }
    };

    $scope.onBusObjectChanged = function (object, loadXmlMethods) {
        if (object && object.IsObjectVisibility) {
            $scope.$evalAsync(function () {
                $scope.objBusinessObject.BusObjectName = object.dictAttributes.ID;
                $scope.objBusinessObject.ObjTree = undefined;
            });
        }
        if (loadXmlMethods) {
            $scope.loadXmlMethods(object);
        }
    };

    //#endregion

    //#region Object Tree Methods

    //This method loads the object tree of the expanded object (object/standard bookmark) for the selected tab.
    $scope.loadObjectTreeForExpandedObject = function (collection) {
        if (collection && collection.length > 0) {
            var items = $filter('filter')(collection, { IsObjectVisibility: true }, true);
            if (items && items.length > 0) {
                if (items[0].dictAttributes && items[0].dictAttributes.ID && items[0].dictAttributes.ID.trim().length > 0
                    && $scope.objBusinessObject.ObjTree.ObjName != items[0].dictAttributes.ID) {
                    $scope.$evalAsync(function () {
                        $scope.objBusinessObject.BusObjectName = items[0].dictAttributes.ID;
                        $scope.objBusinessObject.ObjTree = undefined;
                    });
                }
            }
        }
    };

    $scope.receiveObjectTree = function (data, path) {
        $scope.$evalAsync(function () {
            var obj = JSON.parse(data);
            if (path != undefined && path != "") {
                var busObject = getBusObjectByPath(path, $scope.objBusinessObject.ObjTree);
                if (busObject && busObject.ItemType.Name == obj.ObjName) {
                    busObject.ChildProperties = obj.ChildProperties;
                    busObject.lstMethods = obj.lstMethods;
                    busObject.HasLoadedProp = true;
                }
                SetParentForObjTreeChild(busObject);
            }
            else {
                $scope.objBusinessObject.ObjTree = obj;
                $scope.objBusinessObject.ObjTree.IsMainBusObject = true;
                $scope.objBusinessObject.ObjTree.IsVisible = true;
                SetParentForObjTreeChild($scope.objBusinessObject.ObjTree);
            }
        });
    };

    //#endregion

    //#region Common Properties

    $scope.selectCommonProperty = function (property, fromUndoRedoBlock) {
        if ($scope.selectedObject) {
            $scope.selectedObject.IsObjectVisibility = false;
            $scope.selectedObject = null;
        }
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedCommonProperty, $scope, "selectedCommonProperty", property);
        }
        else {
            $scope.selectedCommonProperty = property;
        }
    };
    $scope.addEditCommonProperty = function (commonProperty) {
        var newScope = $scope.$new();
        if (commonProperty) {
            newScope.id = commonProperty.dictAttributes.ID;
            newScope.description = commonProperty.dictAttributes.sfwDescription;
        }
        else {
            newScope.id = "";
            newScope.description = "";
        }
        newScope.validateID = function () {
            newScope.errorMessage = "";
            if (!(newScope.id && newScope.id.trim().length > 0)) {
                newScope.errorMessage = "ID cannot be blank or whitespace(s).";
            }
            else if ($scope.propertiesModel.Elements.some(function (x) { return commonProperty ? (x.dictAttributes.ID == newScope.id && x != commonProperty) : x.dictAttributes.ID == newScope.id; })) {
                newScope.errorMessage = "Duplicate ID.";
            }
        };
        newScope.updateCommonProperty = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (commonProperty) {
                $rootScope.EditPropertyValue(commonProperty.dictAttributes.ID, commonProperty.dictAttributes, "ID", newScope.id);
                $rootScope.EditPropertyValue(commonProperty.dictAttributes.sfwDescription, commonProperty.dictAttributes, "sfwDescription", newScope.description);
            }
            else {
                var property = { Name: "item", Value: "", dictAttributes: { ID: newScope.id, sfwDescription: newScope.description }, Elements: [] };
                $rootScope.PushItem(property, $scope.propertiesModel.Elements);
                $scope.selectCommonProperty(property, true);
            }
            $rootScope.UndRedoBulkOp("End");
            newScope.cancel();
        };
        newScope.cancel = function () {
            if (newScope.dialog && newScope.dialog.close) {
                newScope.dialog.close();
            }
        };
        newScope.dialog = $rootScope.showDialog(newScope, commonProperty ? "Edit Common Property" : "Add Common Property", "Tools/views/AddEditCommonProperty.html", { width: 500, height: 300 });
        newScope.validateID();
    };
    $scope.canDeleteCommonProperty = function () {
        return $scope.selectedCommonProperty;
    };
    $scope.deleteCommonProperty = function () {
        var index = $scope.propertiesModel.Elements.indexOf($scope.selectedCommonProperty);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCommonProperty, $scope.propertiesModel.Elements);

            //Select next item
            if ($scope.propertiesModel.Elements.length > 0) {
                if (index == $scope.propertiesModel.Elements.length) {
                    index--;
                }
                $scope.selectCommonProperty($scope.propertiesModel.Elements[index], true);
            }
            else {
                $scope.selectCommonProperty(null, true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    //#region Common Properties - Objects

    $scope.selectObject = function (object, fromUndoRedoBlock) {
        $scope.selectedCommonProperty = null;
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedObject, $scope, "selectedObject", object);
        }
        else {
            $scope.selectedObject = object;
        }
    };
    $scope.addObject = function () {
        var objofObject = {
            dictAttributes: {},
            Elements: [],
            Children: [],
            Name: "object",
            Value: ""
        };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(objofObject, $scope.objCommonproperties.Elements);
        //objofObject.IsObjectVisibility = true;
        $scope.selectObject(objofObject, true);
        $scope.toggleObjectExpander(objofObject);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteObject = function () {
        return $scope.selectedObject;
    };
    $scope.deleteObject = function () {
        var objects = $scope.objCommonproperties.Elements.filter(function (item) { return item.Name == "object"; });
        if (objects && objects.length > 0) {
            var index = $scope.objCommonproperties.Elements.indexOf($scope.selectedObject);
            var objIndex = objects.indexOf($scope.selectedObject);
            if (index > -1 && objIndex > -1) {
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.DeleteItem($scope.selectedObject, $scope.objCommonproperties.Elements);
                objects = $scope.objCommonproperties.Elements.filter(function (item) { return item.Name == "object"; });

                //Select next item
                if (objects && objects.length > 0) {
                    if (objIndex == objects.length) {
                        objIndex--;
                    }
                    $scope.selectObject(objects[objIndex], true);
                }
                else {
                    $scope.selectObject(null, true);
                }
                $rootScope.UndRedoBulkOp("End");
            }
        }
    };
    $scope.toggleObjectExpander = function (object, isExpand) {
        if (object) {
            for (var i = 0; i < $scope.objCommonproperties.Elements.length; i++) {
                if ($scope.objCommonproperties.Elements[i].Name == "object") {
                    if ($scope.objCommonproperties.Elements[i] == object) {
                        if (isExpand) {
                            $scope.objCommonproperties.Elements[i].IsObjectVisibility = true;
                        } else {
                            $scope.objCommonproperties.Elements[i].IsObjectVisibility = !$scope.objCommonproperties.Elements[i].IsObjectVisibility;
                        }
                    }
                    else {
                        $scope.objCommonproperties.Elements[i].IsObjectVisibility = false;
                    }
                }
                $scope.onEntityChanged(object.dictAttributes.ID);
            }

            //if ((!$scope.objBusinessObject.ObjTree) ||
            //    (object.IsObjectVisibility &&
            //    object.dictAttributes && object.dictAttributes.ID && object.dictAttributes.ID.trim().length > 0 &&
            //    $scope.objBusinessObject.ObjTree.ObjName != object.dictAttributes.ID)) {
            //    $scope.$evalAsync(function () {
            //        $scope.objBusinessObject.BusObjectName = object.dictAttributes.ID;
            //        $scope.objBusinessObject.ObjTree = undefined;
            //    });
            //}
        }
    };

    //#region Common Properties - Objects - Items
    $scope.selectObjectItem = function (object, item, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue(object.selectedObjectItem, object, "selectedObjectItem", item);
        }
        else {
            object.selectedObjectItem = item;
            $scope.selectedObjectItem = item;
        }
    };
    $scope.addObjectItem = function (object) {
        var item = {
            dictAttributes: {},
            Elements: [],
            Children: [],
            Name: "item",
            Value: ""
        };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, object.Elements);
        $scope.selectObjectItem(object, item, true);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteObjectItem = function (object) {
        return object.selectedObjectItem;
    };
    $scope.deleteObjectItem = function (object) {
        var index = object.Elements.indexOf(object.selectedObjectItem);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem(object.selectedObjectItem, object.Elements);

            //Select next item
            if (object.Elements.length > 0) {
                if (index == object.Elements.length) {
                    index--;
                }
                $scope.selectObjectItem(object, object.Elements[index], true);
            }
            else {
                $scope.selectObjectItem(object, null, true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    $scope.onObjectFieldChanged = function (item) {
        if (item.dictAttributes.sfwEntityField && item.dictAttributes.sfwEntityField.trim().length > 0) {
            item.dictAttributes.sfwObjectMethod = '';
        }
    };
    $scope.onObjectMethodChanged = function (item) {
        if (item.dictAttributes.sfwObjectMethod && item.dictAttributes.sfwObjectMethod.trim().length > 0) {
            item.dictAttributes.sfwEntityField = '';
        }
    };
    $scope.getObjectMethodList = function (entityID, event) {
        //if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
        //    if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(event.currentTarget).data('ui-autocomplete')) {
        //        $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
        //        event.preventDefault();
        //    } else {
        //        setSingleLevelAutoComplete($(event.currentTarget), $scope.objBusinessObject.ObjTree.lstMethods, $scope, "ShortName", "Description");
        //    }
        //}
        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            event.preventDefault();
        }
        else if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
            $scope.onEntityChanged(entityID);
            setSingleLevelAutoComplete($(event.currentTarget), $scope.lstObjectMethod, $scope, "ID", "ID");
            if ($(event.currentTarget).data('ui-autocomplete')) {
                $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
            }
            event.preventDefault();
        }
        else if ($(event.currentTarget).data('ui-autocomplete') && $(event.currentTarget).val() != "") {
            $(event.currentTarget).autocomplete("search", $(event.currentTarget).val());
        }

    };
    $scope.showObjectMethodList = function (entityID, event) {
        $scope.inputElement = $(event.target).prevAll("input[type='text']");
        if ($scope.inputElement) {
            $($scope.inputElement).focus();
            //if ($scope.objBusinessObject.ObjTree && $scope.objBusinessObject.ObjTree.lstMethods) {
            //    setSingleLevelAutoComplete($scope.inputElement, $scope.objBusinessObject.ObjTree.lstMethods, $scope, "ShortName", "Description");
            //    if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            //}
            if ($($scope.inputElement).val() == "") {
                $scope.onEntityChanged(entityID);
            }
            if ($scope.lstObjectMethod && $scope.lstObjectMethod.length > 0) {
                setSingleLevelAutoComplete($scope.inputElement, $scope.lstObjectMethod, $scope, "ID", "ID");
                if ($($scope.inputElement).data('ui-autocomplete')) $($scope.inputElement).autocomplete("search", $($scope.inputElement).val());
            }
            else {
                $scope.onEntityChanged(entityID);
            }
        }
        if (event) {
            event.stopPropagation();
        }
    };
    $scope.onEntityChanged = function (entityID) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        parententityName = entityID;
        $scope.lstObjectMethod = [];
        while (parententityName) {
            var entity = entityIntellisenseList.filter(function (x) {
                return x.ID == parententityName;
            });
            if (entity.length > 0 && entity[0].ObjectMethods && entity[0].ObjectMethods.length > 0) {
                var tempdata = [];
                for (var i = 0; i < entity[0].ObjectMethods.length; i++) {
                    if (entity[0].ObjectMethods[i].Parameters && entity[0].ObjectMethods[i].Parameters.length == 0 && entity[0].ObjectMethods[i].ReturnType != "Void") {
                        if (entity[0].ObjectMethods[i].ID) {
                            tempdata.push(entity[0].ObjectMethods[i]);
                        }
                    }
                }
                $scope.lstObjectMethod = $scope.lstObjectMethod.concat(tempdata);
            }
            if (entity.length > 0) {
                parententityName = entity[0].ParentId;
            } else {
                parententityName = "";
            }
        }
    };
    //#endregion

    //#endregion

    //#endregion

    //#region Correspondence - Standard Bookmarks

    $scope.selectStandardBookmark = function (stdBookmark, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedStandardBookmark, $scope, "selectedStandardBookmark", stdBookmark);
        }
        else {
            $scope.selectedStandardBookmark = stdBookmark;
        }
    };
    $scope.addStandardBookmark = function () {
        var object = { Name: "object", Value: "", dictAttributes: {}, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(object, $scope.stdBookmarksModel.Elements);
        $scope.selectStandardBookmark(object, true);
        $rootScope.UndRedoBulkOp("End");
        $scope.toggleStandardBookmarkExpander(object);
    };
    $scope.canDeleteStandardBookmark = function () {
        return $scope.selectedStandardBookmark;
    };
    $scope.deleteStandardBookmark = function () {
        var index = $scope.stdBookmarksModel.Elements.indexOf($scope.selectedStandardBookmark);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedStandardBookmark, $scope.stdBookmarksModel.Elements);

            //Select next item
            if ($scope.stdBookmarksModel.Elements.length > 0) {
                if (index == $scope.stdBookmarksModel.Elements.length) {
                    index--;
                }
                $scope.selectStandardBookmark($scope.stdBookmarksModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };
    $scope.toggleStandardBookmarkExpander = function (stdBookmark, isExpand) {
        if (stdBookmark) {
            for (var i = 0; i < $scope.stdBookmarksModel.Elements.length; i++) {
                if ($scope.stdBookmarksModel.Elements[i] == stdBookmark) {
                    if (isExpand) {
                        $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = true;
                    } else {
                        $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = !$scope.stdBookmarksModel.Elements[i].IsObjectVisibility;
                    }
                }
                else {
                    $scope.stdBookmarksModel.Elements[i].IsObjectVisibility = false;
                }
            }
            $scope.onCorrespondanceEntityChanged(stdBookmark);
            //if ((!$scope.objBusinessObject.ObjTree) ||
            //    (stdBookmark.IsObjectVisibility &&
            //    stdBookmark.dictAttributes && stdBookmark.dictAttributes.ID && stdBookmark.dictAttributes.ID.trim().length > 0 &&
            //    $scope.objBusinessObject.ObjTree.ObjName != stdBookmark.dictAttributes.ID)) {
            //    $scope.$evalAsync(function () {
            //        $scope.objBusinessObject.ObjTree = undefined;
            //        $scope.objBusinessObject.BusObjectName = stdBookmark.dictAttributes.ID;
            //    });
            //}
        }
    };
    $scope.loadXmlMethods = function (object) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entities = $filter('filter')(entityIntellisenseList, { BusinessObjectName: object.dictAttributes.ID }, true);
        if (entities && entities.length > 0)
            object.xmlMethods = $getEnitityXmlMethods.get(entities[0].ID);
    };
    $scope.onCorrespondanceEntityChanged = function (object) {
        if (object) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            parententityName = object.dictAttributes.ID;
            object.xmlMethods = [];
            while (parententityName) {
                var entity = entityIntellisenseList.filter(function (x) {
                    return x.ID == parententityName;
                });
                if (entity.length > 0 && entity[0].XmlMethods && entity[0].XmlMethods.length > 0) {
                    object.xmlMethods = object.xmlMethods.concat(entity[0].XmlMethods);
                }
                if (entity.length > 0) {
                    parententityName = entity[0].ParentId;
                } else {
                    parententityName = "";
                }
            }
        }
        else {
            object.xmlMethods = [];
        }
    };
    //#region Standard Bookmarks - Items
    $scope.selectStandardBookmarkItem = function (parentBookmark, stdBookmarkItem, fromUndoRedoBlock) {
        if (parentBookmark) {
            if (fromUndoRedoBlock) {
                $rootScope.EditPropertyValue(parentBookmark.selectedStandardBookmarkItem, parentBookmark, "selectedStandardBookmarkItem", stdBookmarkItem);
                parentBookmark.selectedStandardBookmarkItem = stdBookmarkItem;
            }
            else {
                parentBookmark.selectedStandardBookmarkItem = stdBookmarkItem;
            }
            $scope.selectedStandardBookmarkItem = stdBookmarkItem;
        }
    };
    $scope.addStandardBookmarkItem = function (parentBookmark) {
        if (parentBookmark) {
            var item = { Name: "item", Value: "", dictAttributes: {}, Elements: [] };
            $scope.AddOrEditStandardBookMark(parentBookmark, item, "Add");
        }
    };
    $scope.canDeleteStandardBookmarkItem = function (parentBookmark) {
        return parentBookmark.selectedStandardBookmarkItem;
    };
    $scope.deleteStandardBookmarkItem = function (parentBookmark) {
        var index = parentBookmark.Elements.indexOf(parentBookmark.selectedStandardBookmarkItem);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem(parentBookmark.selectedStandardBookmarkItem, parentBookmark.Elements);

            //Select next item
            if (parentBookmark.Elements.length > 0) {
                if (index == parentBookmark.Elements.length) {
                    index--;
                }
                $scope.selectStandardBookmarkItem(parentBookmark.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.AddOrEditStandardBookMark = function (objBookMarkParent, objStdBookMark, param) {
        var newStdBookMarkScope = $scope.$new();
        newStdBookMarkScope.objBookMark = {};
        newStdBookMarkScope.objBookMark.Entity = objBookMarkParent.dictAttributes.ID;
        if (objStdBookMark) {
            newStdBookMarkScope.objBookMark.id = objStdBookMark.dictAttributes.ID;
            newStdBookMarkScope.objBookMark.EntityField = objStdBookMark.dictAttributes.sfwEntityField;
        }
        else {
            newStdBookMarkScope.objBookMark.id = "";
            newStdBookMarkScope.objBookMark.EntityField = "";
        }
        newStdBookMarkScope.errorMessage = "";

        newStdBookMarkScope.validateID = function () {
            newStdBookMarkScope.errorMessage = "";
            if (!(newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.trim().length > 0)) {
                newStdBookMarkScope.errorMessage = "ID cannot be blank or whitespace(s).";
            }
            else if (newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.substring(0, 3) != "std") {
                newStdBookMarkScope.errorMessage = "ID should start with 'std' prefix .";
            }
            else if (newStdBookMarkScope.objBookMark.id && newStdBookMarkScope.objBookMark.id.substring(0, 3) == "std" && newStdBookMarkScope.objBookMark.id.length == 3) {
                newStdBookMarkScope.errorMessage = "'std' is prefix , Enter valid ID.";
            }
            else if ($scope.stdBookmarksModel && $scope.stdBookmarksModel.Elements.length) {
                for (var i = 0; i < $scope.stdBookmarksModel.Elements.length; i++) {
                    for (var j = 0; j < $scope.stdBookmarksModel.Elements[i].Elements.length; j++) {
                        if ($scope.stdBookmarksModel.Elements[i].Elements[j] != objStdBookMark && newStdBookMarkScope.objBookMark.id == $scope.stdBookmarksModel.Elements[i].Elements[j].dictAttributes.ID) {
                            newStdBookMarkScope.errorMessage = "Duplicate ID.";
                            break;
                        }
                    }
                    if (newStdBookMarkScope.errorMessage) {
                        break;
                    }
                }
            }
        };
        newStdBookMarkScope.cancel = function () {
            if (newStdBookMarkScope.dialog && newStdBookMarkScope.dialog.close) {
                newStdBookMarkScope.dialog.close();
            }
        };

        newStdBookMarkScope.updateStandardBookMark = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (param == "Add") {
                objStdBookMark.dictAttributes.ID = newStdBookMarkScope.objBookMark.id;
                objStdBookMark.dictAttributes.sfwEntityField = newStdBookMarkScope.objBookMark.EntityField;
                $rootScope.PushItem(objStdBookMark, objBookMarkParent.Elements);
                $scope.selectStandardBookmarkItem(objBookMarkParent, objStdBookMark, true);
            } else if (param == "Edit") {
                $rootScope.EditPropertyValue(objStdBookMark.dictAttributes.ID, objStdBookMark.dictAttributes, 'ID', newStdBookMarkScope.objBookMark.id);
                $rootScope.EditPropertyValue(objStdBookMark.dictAttributes.sfwEntityField, objStdBookMark.dictAttributes, 'sfwEntityField', newStdBookMarkScope.objBookMark.EntityField);
            }
            $rootScope.UndRedoBulkOp("End");
            newStdBookMarkScope.cancel();
        };
        var Title = "";
        if (param == "Edit") {
            Title = "Edit Standard Book Mark";
        } else {
            Title = "Add Standard Book Mark";
        }
        newStdBookMarkScope.dialog = $rootScope.showDialog(newStdBookMarkScope, Title, "Tools/views/AddEditStandardBookMark.html", { width: 500, height: 300 });
        newStdBookMarkScope.validateID();
    }
    //#endregion

    //#endregion

    //#region Server Methods
    $scope.clearServerMethodFilter = function () {
        $scope.serverMethodFilters.isServerMethodFilterVisible = false;
    };
    $scope.selectAndScrollSrvMethod = function (methodObj, fromUndoRedoBlock) {
        $scope.getCurrentSrvMethod(methodObj, fromUndoRedoBlock);
        $timeout(function () {
            var selectedItem = $("#server-method-list").find(".searchrowSelected");
            if (selectedItem && selectedItem.length) {
                selectedItem[0].scrollIntoView();
            }
        });
    };
    $scope.getCurrentSrvMethod = function (methodObj, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.currentSrvMethod, $scope, "currentSrvMethod", methodObj);
            $rootScope.EditPropertyValue($scope.currentMethod, $scope, "currentMethod", null);
        }
        else {
            $scope.currentSrvMethod = methodObj;
            $scope.currentMethod = null;
        }
        $scope.clearServerMethodFilter();
    };

    $scope.selectAndScrollRemoteMethod = function (methodObj, fromUndoRedoBlock) {
        $scope.getCurrentMethod(methodObj, fromUndoRedoBlock);
        $timeout(function () {
            var selectedItem = $("#remote-method-list").find(".searchrowSelected");
            if (selectedItem && selectedItem.length) {
                selectedItem[0].scrollIntoView();
            }
        });
    };
    $scope.getCurrentMethod = function (methods, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.currentMethod, $scope, "currentMethod", methods);
        }
        else {
            $scope.currentMethod = methods;
        }
    };

    // ======================== referesh server methods start ==============================================

    $scope.refreshAllRemoteObjects = function () {
        /// <summary>test</summary>
        // check for classes - if not present delete        
        if ($scope.objServerMethods.Elements.length > 0) {
            $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
            var indexofSelectedMethodGroup = $scope.objServerMethods.Elements.indexOf($scope.currentSrvMethod);
            for (var i = $scope.objServerMethods.Elements.length - 1; i >= 0; i--) {
                if ($scope.remoteObjCollection.indexOf($scope.objServerMethods.Elements[i].dictAttributes.ID) < 0) {
                    if (indexofSelectedMethodGroup >= i) {
                        indexofSelectedMethodGroup -= 1;
                    }
                    $scope.isDirty = true;
                    $scope.objServerMethods.Elements.splice(i, 1);
                }
            }
            if ($scope.objServerMethods.Elements.length > 0) {
                var isSelectedGroupExist = false;
                // loop on classes and check methods are present - if not present delete
                $scope.objServerMethods.Elements.forEach(function (remoteObject) {
                    if ($scope.currentSrvMethod && ($scope.currentSrvMethod.dictAttributes.ID == remoteObject.dictAttributes.ID)) {
                        isSelectedGroupExist = true;
                    }
                    $scope.refreshRemoteObject(remoteObject, true);
                });
                // if selected object does not exist - select the one at its original index or the first one from the remaining list
                if (!isSelectedGroupExist) {
                    $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[indexofSelectedMethodGroup] ? $scope.objServerMethods.Elements[indexofSelectedMethodGroup] : $scope.objServerMethods.Elements[0]);
                }
            }
            else {
                $scope.getCurrentSrvMethod(undefined);
            }
        }
    };

    $scope.refreshRemoteObject = function (remoteObject, isGlobalRefresh) {
        $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
        var indexofSelectedMethod = remoteObject.Elements.indexOf($scope.currentMethod);
        $rootScope.IsLoading = true;
        $.connection.hubMain.server.getRemoteObjectMethods(remoteObject.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    if (data.length > 0) {
                        for (var i = remoteObject.Elements.length - 1; i >= 0; i--) {
                            function filterRemotemethods(Rmethods, index, remoteMethods) {
                                if (Rmethods.MethodName == remoteObject.Elements[i].dictAttributes.ID) {
                                    return true;
                                }
                            }
                            if (!data.some(filterRemotemethods)) {
                                if (indexofSelectedMethod >= i) {
                                    indexofSelectedMethod -= 1;
                                }
                                $scope.isDirty = true;
                                remoteObject.Elements.splice(i, 1);
                            }
                        }
                        if (remoteObject.Elements.length > 0) {
                            var isSelectedMethodExist = false;
                            // if method present check for parameters - and replace
                            remoteObject.Elements.forEach(function (localRemoteMethod) {
                                // if any method is selected - preserve the selection
                                if ($scope.currentMethod && (localRemoteMethod.dictAttributes.ID == $scope.currentMethod.dictAttributes.ID)) {
                                    isSelectedMethodExist = true;
                                }
                                $scope.refreshRemoteMethod(remoteObject.dictAttributes.ID, localRemoteMethod);
                            });
                            if (isGlobalRefresh) {
                                $scope.getCurrentMethod(undefined);
                            }
                            else if (!isSelectedMethodExist) {
                                $scope.getCurrentMethod(remoteObject.Elements[indexofSelectedMethod] ? remoteObject.Elements[indexofSelectedMethod] : remoteObject.Elements[0]);
                            }

                        }
                        else {
                            $scope.getCurrentMethod(undefined);
                        }
                    }
                    // remoteobject does not have any method
                    else {
                        $scope.isDirty = true;
                        for (var i = remoteObject.Elements.length - 1; i >= 0; i--) {
                            remoteObject.Elements.splice(i, 1);
                        }
                        $scope.getCurrentMethod(undefined);
                    }
                }
                else {
                    toastr.warning(remoteObject.dictAttributes.ID + " is not a valid method group");
                }
                $rootScope.IsLoading = false;
            });
        });
    };

    $scope.refreshRemoteMethod = function (remoteObjectID, localRemoteMethod) {
        if ($rootScope.currentopenfile) {
            $rootScope.ClearUndoRedoListByFileName($rootScope.currentopenfile.file.FileName);
        }
        $rootScope.IsLoading = true;
        $.connection.hubMain.server.getRemoteMethod(remoteObjectID, localRemoteMethod.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.isDirty = true;
                    localRemoteMethod.Elements = [];
                    for (var i = 0; i < data.Parameters.length; i++) {
                        var newParameters = { Name: 'parameter', Value: '', dictAttributes: { ID: data.Parameters[i].ParameterName, sfwDataType: data.Parameters[i].ParameterType.FullName }, Elements: [] };
                        localRemoteMethod.Elements.push(newParameters);
                    }
                }
                else {
                    toastr.warning(localRemoteMethod.dictAttributes.ID + " is not a valid method");
                }
                $rootScope.IsLoading = false;
            });
        });
    };

    // ======================== referesh server methods ends ===========================================

    // Add method for selected server method
    $scope.OpenAddMethodWindow = function (methodGrp) {
        var newScope = $scope.$new();
        $.connection.hubMain.server.getRemoteObjectMethods($scope.currentSrvMethod.dictAttributes.ID).done(function (data) {
            $scope.$evalAsync(function () {
                newScope.lstRemoteObjMethod = data;
            });
        });

        newScope.templateUrl = "Tools/views/AddServerMethod.html";
        newScope.methodMessage = "Please select method.";

        newScope.OnOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            var newMethod = {
                Name: 'method', Value: '', dictAttributes: { ID: newScope.selectedMethodName.MethodName, sfwMode: newScope.sfwMode }, Elements: []
            };
            angular.forEach(newScope.lstParameters, function (param, key) {
                var newParameters = { Name: 'parameter', Value: '', dictAttributes: { ID: param.ParameterName, sfwDataType: param.ParameterType.FullName }, Elements: [] };
                $rootScope.PushItem(newParameters, newMethod.Elements);
                newParameters = {};
            });
            if (newScope.selectedMethodName && newScope.sfwMode) {
                angular.forEach($scope.objServerMethods.Elements, function (method, key) {
                    if (method.dictAttributes.ID == $scope.currentSrvMethod.dictAttributes.ID) {
                        $rootScope.PushItem(newMethod, method.Elements);
                        newScope.OnCancelClick();
                        //console.log("Before add method", $scope.objServerMethods);
                    }
                });
            }
            $scope.getCurrentMethod(newMethod, true);
            $rootScope.UndRedoBulkOp("End");
        };

        newScope.OnCancelClick = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };


        newScope.getParameters = function () {
            newScope.lstParameters = newScope.selectedMethodName.Parameters;
        };

        newScope.validateServerMethod = function () {
            newScope.methodMessage = "";
            if (!newScope.selectedMethodName) {
                newScope.methodMessage = "Please select method.";
                return true;
            }
            else if (!newScope.sfwMode) {
                newScope.methodMessage = "Please select Mode.";
                return true;
            }
            else if (newScope.currentSrvMethod) {
                var flag = false;
                if ($scope.currentSrvMethod.Elements.length == 0) {
                    return false;
                }
                angular.forEach($scope.currentSrvMethod.Elements, function (method, key) {
                    if (method.dictAttributes.ID == newScope.selectedMethodName.MethodName) {
                        newScope.methodMessage = "This method already exists.";
                        flag = true;
                    }
                });
                return flag;
            }
            else {
                return false;
            }
        };

        if (newScope.templateUrl && newScope.templateUrl.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, "Add Method", newScope.templateUrl, { width: 500, height: 300 });
        }
    };

    // Add server methods
    $scope.OpenAddMethodGroup = function () {
        var newScope = $scope.$new();
        newScope.templateUrl = "Tools/views/AddServerMethodGroup.html";
        newScope.remoteObjCollection = angular.copy($scope.remoteObjCollection);
        // method group which are not already present can be added
        function filterRemoteMethodGroup(methodGroup) {
            var numIndexRemoteObject = newScope.remoteObjCollection.indexOf(methodGroup.dictAttributes.ID);
            if (numIndexRemoteObject >= 0) {
                newScope.remoteObjCollection.splice(numIndexRemoteObject, 1);
            }
        }
        $scope.objServerMethods.Elements.forEach(filterRemoteMethodGroup);
        newScope.srvMethodMessage = "Please select method.";
        newScope.validData = true;
        newScope.OnOkClick = function () {
            $rootScope.UndRedoBulkOp("Start");
            if (newScope.selectedMethodGroup) {
                var newSrvMethod = { Name: "methodgroup", Value: '', dictAttributes: { ID: newScope.selectedMethodGroup }, Elements: [] };
                $rootScope.PushItem(newSrvMethod, $scope.objServerMethods.Elements);
                newScope.OnCancelClick();
            }
            $scope.getCurrentSrvMethod(newSrvMethod, true);
            $rootScope.UndRedoBulkOp("End");
        };

        newScope.OnCancelClick = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        };

        newScope.checkMethodGroupValidity = function (obj) {
            newScope.validData = false;
            angular.forEach($scope.objServerMethods.Elements, function (method, key) {
                if (method.dictAttributes.ID == newScope.selectedMethodGroup) {
                    newScope.srvMethodMessage = "This server method already exists.";
                    newScope.validData = true;
                    return false;
                }
            });
        };

        if (newScope.templateUrl && newScope.templateUrl.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, "Add Method Group", newScope.templateUrl, { width: 500, height: 300 });
        }
    };

    // Delete server method group
    $scope.DeleteMethodGroup = function () {
        var index = $scope.objServerMethods.Elements.indexOf($scope.currentSrvMethod);
        if (index > -1) {
            toastr.success("Server group deleted Successfully");
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.currentSrvMethod, $scope.objServerMethods.Elements);
            //Select next item
            if ($scope.objServerMethods.Elements.length > 0) {
                if (index == $scope.objServerMethods.Elements.length) {
                    index--;
                }
                $scope.getCurrentSrvMethod($scope.objServerMethods.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    // Delete Single method 
    $scope.DeleteMethod = function () {
        var index = $scope.currentSrvMethod.Elements.indexOf($scope.currentMethod);
        if (index > -1) {
            toastr.success("Method deleted Successfully");
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.currentMethod, $scope.currentSrvMethod.Elements);
            //Select next item
            if ($scope.currentSrvMethod.Elements.length > 0) {
                if (index == $scope.currentSrvMethod.Elements.length) {
                    index--;
                }
                $scope.getCurrentMethod($scope.currentSrvMethod.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.removeOldID = function (parameter) {
        $rootScope.EditPropertyValue(parameter.dictAttributes.OldID, parameter.dictAttributes, 'OldID', "");
    };

    //#endregion

    //#region Global Parameters

    $scope.selectGlobalParameter = function (globalParameter, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedGlobalParameter, $scope, "selectedGlobalParameter", globalParameter);
        }
        else {
            $scope.selectedGlobalParameter = globalParameter;
        }
    };
    $scope.addGlobalParameter = function () {
        var item = { Name: "globlarparameter", Value: "", dictAttributes: {}, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, $scope.globalParametersModel.Elements);
        $scope.selectGlobalParameter(item, true);
        $rootScope.UndRedoBulkOp("End");
    };
    $scope.canDeleteGlobalParameter = function () {
        return $scope.selectedGlobalParameter;
    };
    $scope.deleteGlobalParameter = function () {
        var index = $scope.globalParametersModel.Elements.indexOf($scope.selectedGlobalParameter);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedGlobalParameter, $scope.globalParametersModel.Elements);
            //Select next item
            if ($scope.globalParametersModel.Elements.length > 0) {
                if (index == $scope.globalParametersModel.Elements.length) {
                    index--;
                }
                $scope.selectGlobalParameter($scope.globalParametersModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    //#endregion

    //#region Portals

    $scope.selectPortal = function (portal, fromUndoRedoBlock) {
        if (fromUndoRedoBlock) {
            $rootScope.EditPropertyValue($scope.selectedPortal, $scope, "selectedPortal", portal);
        }
        else {
            $scope.selectedPortal = portal;
        }
    };
    $scope.addPortal = function () {
        var item = { Name: "portal", Value: "", dictAttributes: { sfwPortalType: "MVVM" }, Elements: [] };
        $rootScope.UndRedoBulkOp("Start");
        $rootScope.PushItem(item, $scope.portalsModel.Elements);
        $scope.selectPortal(item, true);
        $rootScope.UndRedoBulkOp("End");
        $scope.validatePortals();
    };
    $scope.canDeletePortal = function () {
        return $scope.selectedPortal;
    };
    $scope.deletePortal = function () {
        var index = $scope.portalsModel.Elements.indexOf($scope.selectedPortal);
        if (index > -1) {
            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedPortal, $scope.portalsModel.Elements);
            //Select next item
            if ($scope.portalsModel.Elements.length > 0) {
                if (index == $scope.portalsModel.Elements.length) {
                    index--;
                }
                $scope.selectPortal($scope.portalsModel.Elements[index], true);
            }
            $rootScope.UndRedoBulkOp("End");
        }
        $scope.validatePortals();
    };
    $scope.validatePortal = function (portal) {
        if (portal.dictAttributes.ID) {
            if ($scope.portalsModel.Elements.some(function (itm) { return itm.dictAttributes.ID === portal.dictAttributes.ID && itm !== portal; })) {
                if (!portal.errors) {
                    portal.errors = {};
                }
                portal.errors.ID = "Duplicate Portal ID. Please provide a different Portal ID.";
            }
            else if (portal.errors) {
                portal.errors.ID = null;
            }
        }
        else {
            if (!portal.errors) {
                portal.errors = {};
            }
            portal.errors.ID = "Portal ID cannot be empty.";
        }

        if (portal.dictAttributes.sfwPortalName) {
            if ($scope.portalsModel.Elements.some(function (itm) { return itm.dictAttributes.sfwPortalName === portal.dictAttributes.sfwPortalName && itm !== portal; })) {
                if (!portal.errors) {
                    portal.errors = {};
                }
                portal.errors.sfwPortalName = "Duplicate Portal Name. Please provide a different Portal Name.";
            }
            else if (portal.errors) {
                portal.errors.sfwPortalName = null;
            }
        }
        else {
            if (!portal.errors) {
                portal.errors = {};
            }
            portal.errors.sfwPortalName = "Portal Name cannot be empty.";
        }

    }
    $scope.validatePortals = function () {
        for (var index = 0, len = $scope.portalsModel.Elements.length; index < len; index++) {
            $scope.validatePortal($scope.portalsModel.Elements[index]);
        }
    }
    //#endregion

    $scope.initialize();
}]);
app.controller("ConvertToHtxController", ["$scope", "hubcontext", "$rootScope", "$cookies", "$DashboardFactory", function ($scope, hubcontext, $rootScope, $cookies, $DashboardFactory) {
    $rootScope.IsLoading = true;
    $scope.SearchText = "";
    $scope.location = "";
    $scope.IsMaintenance = true;
    $scope.IsLookup = true;
    $scope.IsWizard = true;
    $scope.IsProcessing = true;
    $scope.IsConverting = false;
    $scope.isCheckAllFiles = false;
    $scope.lstFileDetails = [];
    $scope.pageIndex = 1;
    $scope.lstSelectedFiles = [];
    $scope.changeFileList = function () {
        $scope.pageIndex = 1;
        $scope.filetype = "";
        if ($scope.IsMaintenance) {
            $scope.filetype = "Maintenance";
        }
        else {
            $scope.RemoveSelectedFiles("Maintenance");
        }
        if ($scope.IsLookup) {
            if ($scope.filetype != "") {
                $scope.filetype += ",Lookup";
            }
            else {
                $scope.filetype = "Lookup";
            }
        }
        else {
            $scope.RemoveSelectedFiles("Lookup");
        }
        if ($scope.IsWizard) {
            if ($scope.filetype != "") {
                $scope.filetype += ",Wizard";
            }
            else {
                $scope.filetype = "Wizard";
            }
        }
        else {
            $scope.RemoveSelectedFiles("Wizard");
        }

        $scope.getFilterList();
    };
    $scope.RemoveSelectedFiles = function (fileType) {
        if ($scope.lstSelectedFiles.length > 0) {
            for (var i = $scope.lstSelectedFiles.length; i--;) {
                if ($scope.lstSelectedFiles[i].File && $scope.lstSelectedFiles[i].File.FileType && $scope.lstSelectedFiles[i].File.FileType == fileType) $scope.lstSelectedFiles.splice(i, 1);
            }
        }
    };
    $scope.getFilterList = function () {
        if ($scope.lstWfmFiles && $scope.lstWfmFiles.length > 0) {
            angular.forEach($scope.lstWfmFiles, function (item) {
                if (item.IsSelected) {
                    if (!$scope.lstSelectedFiles.some(function (x) { return x.FileName == item.File.FileName; })) {
                        $scope.lstSelectedFiles.push(item);
                    }
                }
            });
        }
        hubcontext.hubConvertToHtx.server.getWfmFormList($scope.pageIndex, $scope.filetype, $scope.SearchText, $scope.location).done(function (data) {
            $scope.$evalAsync(function () {
                $scope.IsProcessing = false;
                if (data) {
                    $scope.lstWfmFiles = data;

                    if ($scope.lstSelectedFiles.length > 0 && $scope.lstWfmFiles && $scope.lstWfmFiles.length > 0) {
                        angular.forEach($scope.lstSelectedFiles, function (item) {
                            if (item && item.File && item.File.FileName) {
                                var objfile = $scope.lstWfmFiles.filter(function (x) { return x.File && x.File.FileName == item.File.FileName; });
                                if (objfile && objfile.length > 0) {
                                    objfile[0].IsSelected = true;
                                    objfile[0].Status = item.Status;
                                }
                            }
                        });
                    }
                    if ($scope.isCheckAllFiles) {
                        $scope.checkAllFiles(true);
                    }
                    $rootScope.IsLoading = false;
                }
                else {
                    $scope.lstWfmFiles = [];
                }
            });
        });
    };
    $scope.changeFileList();
    $scope.checkAllFiles = function (isCheckAllFiles) {
        $scope.isCheckAllFiles = isCheckAllFiles;
        if ($scope.lstWfmFiles && $scope.lstWfmFiles.length > 0) {
            angular.forEach($scope.lstWfmFiles, function (file) {
                if (!(file.Status && file.Status == "Completed")) {
                    file.IsSelected = isCheckAllFiles;
                }
            });
        }
    };
    $scope.convertToHtxClick = function () {
        var lstFiles = [];
        if ($scope.lstWfmFiles && $scope.lstWfmFiles.length > 0) {
            angular.forEach($scope.lstWfmFiles, function (item) {
                if (item.IsSelected) {
                    item.Status = "Pending";
                    lstFiles.push(item.File.FileName);
                }
            });
        }
        if (lstFiles.length > 0) {
            var newScope = $scope.$new();
            newScope.lstDirtyFiles = [];
            var temp = JSON.stringify($rootScope.lstopenedfiles);
            var tempOpenFile = JSON.parse(temp);
            if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.length > 0) {
                for (var i = 0; i < tempOpenFile.length; i++) {
                    for (var j = 0; j < lstFiles.length; j++) {
                        if (tempOpenFile[i].file.FileName === lstFiles[j]) {
                            var scope = getScopeByFileName(tempOpenFile[i].file.FileName);
                            if (scope) {
                                if (scope.isDirty) {
                                    newScope.lstDirtyFiles.push(tempOpenFile[i]);
                                }
                                else {
                                    $rootScope.closeFile(tempOpenFile[i].file.FileName);
                                }
                            }
                        }
                    }
                }
            }
            //else {
            //    $scope.IsProcessing = true;
            //    hubcontext.hubConvertToHtx.server.convertToHtx(lstFiles, $scope.isCheckAllFiles, $scope.filetype, $scope.SearchText, $scope.location);
            //}
            if (newScope.lstDirtyFiles.length > 0) {
                var dialog = $rootScope.showDialog(newScope, "UnSaved File List", "Common/views/UnsavedFileListDialog.html");
                newScope.closeSaveListDialog = function (buttontext) {
                    var scope = angular.element($('div[ng-controller="PageLayoutController"]')).scope();
                    if (buttontext == "Yes") {
                        for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                            scope.onsavefileclick(newScope.lstDirtyFiles[i]);
                            $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                        }
                    }
                    else if (buttontext == "No") {
                        for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                            $rootScope.closeFile(newScope.lstDirtyFiles[i].file.FileName);
                        }
                    }
                    $scope.IsProcessing = true;
                    $scope.IsConverting = true;
                    $scope.lstSelectedFiles = [];
                    hubcontext.hubConvertToHtx.server.convertToHtx(lstFiles, $scope.isCheckAllFiles, $scope.filetype, $scope.SearchText, $scope.location);
                    dialog.close();
                };
            }
            else if (lstFiles && lstFiles.length > 0) {
                $scope.IsProcessing = true;
                $scope.IsConverting = true;
                $scope.lstSelectedFiles = [];
                hubcontext.hubConvertToHtx.server.convertToHtx(lstFiles, $scope.isCheckAllFiles, $scope.filetype, $scope.SearchText, $scope.location);
            }
        }
    };
    $scope.receiveFileStatus = function (data) {
        $scope.$evalAsync(function () {
            if (data != "ConversionCompleted" && data != "ConversionCanceled") {
                var isFileFound = false;
                if ($scope.lstWfmFiles && $scope.lstWfmFiles.length > 0) {
                    for (var i = 0; i < $scope.lstWfmFiles.length; i++) {
                        if (data.File && data.File.FileName && $scope.lstWfmFiles[i].File.FileName === data.File.FileName) {
                            $scope.lstWfmFiles[i].IsSelected = false;
                            $scope.lstWfmFiles[i].Status = data.Status;
                            isFileFound = true;
                            break;
                        }
                    }
                }
                if (!isFileFound && data.File && data.File.FileName) {
                    $scope.lstWfmFiles.push(data);
                }
            }
            else if (data == "ConversionCompleted" ) {
                if (data.Status && data.Status == "Completed") {
                    toastr.success("Selected Files are succesfully converted.");
                }
                $scope.IsConverting = false;
                $scope.updateUserFiles($scope.lstSelectedFiles);
            }
            else if (data == "ConversionCanceled") {
                $scope.IsConverting = false;
                $scope.updateUserFiles($scope.lstSelectedFiles);
            }
            if(data.File && data.File.FileName && data.Status=="Completed"){
                $scope.lstSelectedFiles.push(data);
            }
        });
    };
    $scope.OnFormScroll = function () {
        if (!$scope.IsProcessing) {
            $scope.IsProcessing = true;
            $scope.pageIndex = $scope.pageIndex + 1;
            $scope.getFilterList();
        }
    };
    $scope.SearchFormList = function () {
        //if (!$scope.IsProcessing) {
        //$scope.pageIndex = 1;
        //$scope.IsProcessing = true;
        //$scope.getFilterList();
        // }
        $scope.lstSelectedFiles = [];
        if (!$scope.IsConverting) {
            $scope.pageIndex = 1;
            $scope.IsProcessing = true;
            $scope.getFilterList();
        }
    };

    $scope.updateUserFiles = function (lstFileDetails) {
        if (lstFileDetails.length > 0) {
            var lstTempFiles=[];
            for(var i=0;i<lstFileDetails.length;i++){
                lstTempFiles.push(lstFileDetails[i].File);
            }
            var userCookie = $cookies.getObject("UserDetails");
            for (var i = 0; i < lstTempFiles.length; i++) {
                lstTempFiles[i].UserId = JSON.parse(userCookie).UserID;
                lstTempFiles[i].Data = null;
            }
            var filesdataPromise = $DashboardFactory.deleteUserFiles({ pinnedFiles: lstTempFiles, recentFiles: lstTempFiles });
            filesdataPromise.then(function (result) {
                if (result) {
                    $scope.$evalAsync(function () {
                        $rootScope.IsFilesAddedDelete = true;
                        $rootScope.IsTfsUpdated = true;
                        for (var i = 0; i < lstTempFiles.length; i++) {
                            $scope.$parent.addanddeleteuserfile("Unpin", lstTempFiles[i]);
                            $scope.$parent.addanddeleteuserfile("Unrecent", lstTempFiles[i]);
                        }
                        if ($rootScope.isstartpagevisible) {
                            $DashboardFactory.getFileTypeData();
                            $DashboardFactory.getTfsStatusData();                           
                            $scope.$broadcast('updateUserFiles');
                        }
                    });
                }
            });
        }
    };
    $('div[ng-controller="ConvertToHtxController"]').on("keydown", function (e) {
        if (e.keyCode == 13) {
                $scope.SearchFormList();
        }
    });

    $scope.selectCurrentRow = function (aobjItem) {
        $scope.selectedCurrentRow = aobjItem;
    };
    $scope.getClass = function (aobjItem) {
        if (aobjItem && aobjItem.IsSelected) {
            return "selected";
        }
        else if (aobjItem && aobjItem == $scope.selectedCurrentRow) {
            return "select-cell-control";
        }
        else {
            return "";
        }
    };
}]);
app.controller("SelectEntitiesDiagram", ["$scope", "$rootScope", "$EntityIntellisenseFactory", "$Chart", function ($scope, $rootScope, $EntityIntellisenseFactory, $Chart) {
    $rootScope.IsLoading = true;
    var lstTempEntity = $EntityIntellisenseFactory.getEntityIntellisense();
    var strString = JSON.stringify(lstTempEntity);
    $scope.lstEntity = JSON.parse(strString);
    $rootScope.IsLoading = false;
   
    $scope.onShowEntityDiagramClick = function () {
        $scope.objEntityDiagram = [];
        $scope.lstCreatedEntity = [];
        $scope.exampleNodes = [];
        $scope.exampleLinks = [];
        var lstSelectedEntity = $scope.lstEntity.filter(function (aobjEntity) { return aobjEntity.IsSelected == true; });
        if (lstSelectedEntity.length > 0) {
            angular.forEach(lstSelectedEntity, function (aobjEntity) {
                $scope.CreateEntityDiagram(aobjEntity, $scope.objEntityDiagram);
            });
        }
        else {
            toastr.warning("Please select entity.");
        }
        angular.forEach($scope.objEntityDiagram, function (aobjEntity) {
            $scope.CreateEntityDiagramLinks(aobjEntity);
        });
        
        //var exampleNodes = JSON.stringify($scope.exampleNodes);
        //var exampleLinks = JSON.stringify($scope.exampleLinks);
        var strFileName = GetNewERDiagramName("ERDiagram", $rootScope.lstopenedfiles, 0);
        $Chart.setData($scope.exampleNodes, $scope.exampleLinks);
        if (lstSelectedEntity.length > 0) {
            var searchFiles = {
                Data: null,
                FileName: strFileName,
                FilePath: "",
                FileType: "ERDiagram",
                TFSState: "None",
                TimeModified: null
            };
            $rootScope.openFile(searchFiles);
            $scope.onEntityDiagramCancelClick();
        }
    };
    function CheckForDuplicateID(strId, alstOpenFile) {
        var blnReturn = false;
        if (alstOpenFile.length > 0) {
            for (var i = 0; i < alstOpenFile.length; i++) {
                if (alstOpenFile[i].file.FileName == strId) {
                    blnReturn = true;
                    break;
                }
            }
        }
        return blnReturn;
    };

    function GetNewERDiagramName(strItemKey, lstOpenFile, iItemNum) {
        var strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
        while (CheckForDuplicateID(strItemName, lstOpenFile)) {
            iItemNum++;
            strItemName = String.format("{0}{1}", strItemKey, iItemNum.toString());
        }
        return strItemName;
    };
    $scope.CreateEntityDiagram = function (aobjCurrentEntity, aobjEntityDiagram) {
        var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjCurrentEntity.EntityName.toLowerCase() });
        if (lstIsEntityPresent.length == 0) {
            var objResult = { lstOneToOne: [], lstOneToMany: [], entityName: aobjCurrentEntity.EntityName, lstActiveAttributes: [] };
            aobjEntityDiagram.push(objResult);
            var lstAttributes = [];
            angular.forEach(aobjCurrentEntity.Attributes, function (aobjAttribute) {
                if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                    lstAttributes.push(aobjAttribute.ID);
                }
            });
            $scope.exampleNodes.push({ "type": "Entity", id: aobjCurrentEntity.EntityName, name: aobjCurrentEntity.EntityName, attributes: lstAttributes });
            $scope.lstCreatedEntity.push(aobjCurrentEntity.EntityName);
            if (aobjCurrentEntity.Attributes) {
                var lstOneToOneAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && aobjAttribute.Type.toLowerCase() == "object" });
                if (lstOneToOneAttributes.length > 0) {
                    angular.forEach(lstOneToOneAttributes, function (aobjOneToOneAttribute) {
                        if (aobjOneToOneAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName && aobjEntityItem.EntityName == aobjOneToOneAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                objResult.lstActiveAttributes.push(aobjOneToOneAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToOneAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToOne = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    objResult.lstOneToOne.push(objResultOneToOne);
                                    var lstOneToOneAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List")) {
                                            lstOneToOneAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToOneAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);

                                    //$scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToOne);
                                }
                            }
                        }
                    });
                }
                var lstOneToManyAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && (aobjAttribute.Type.toLowerCase() == "collection" || aobjAttribute.Type.toLowerCase() == "list" || aobjAttribute.Type.toLowerCase() == "cdocollection") });
                if (lstOneToManyAttributes.length > 0) {
                    angular.forEach(lstOneToManyAttributes, function (aobjOneToManyAttribute) {
                        if (aobjOneToManyAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName == aobjOneToManyAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                objResult.lstActiveAttributes.push(aobjOneToManyAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToManyAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToMany = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    objResult.lstOneToMany.push(objResultOneToMany);
                                    var lstOneToManyAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (lstOneToManyAttributes.length < 10 && aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToManyAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToManyAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);
                                    // $scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToMany);
                                }
                            }
                        }
                    });
                }
            }
        }
        else {
            var createdEntityObject = $scope.GetCreatedEntityObject(aobjCurrentEntity);
            if (aobjCurrentEntity.Attributes && createdEntityObject) {

                var lstOneToOneAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && aobjAttribute.Type.toLowerCase() == "object" });
                if (lstOneToOneAttributes.length > 0) {
                    angular.forEach(lstOneToOneAttributes, function (aobjOneToOneAttribute) {
                        if (aobjOneToOneAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName && aobjEntityItem.EntityName == aobjOneToOneAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                createdEntityObject.lstActiveAttributes.push(aobjOneToOneAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToOneAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToOne = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    createdEntityObject.lstOneToOne.push(objResultOneToOne);
                                    var lstOneToOneAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (lstOneToOneAttributes.length < 5 && aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToOneAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToOneAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);

                                    //$scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToOne);
                                }
                            }
                        }
                    });
                }
                var lstOneToManyAttributes = aobjCurrentEntity.Attributes.filter(function (aobjAttribute) { return aobjAttribute.Type && (aobjAttribute.Type.toLowerCase() == "collection" || aobjAttribute.Type.toLowerCase() == "list" || aobjAttribute.Type.toLowerCase() == "cdocollection") });
                if (lstOneToManyAttributes.length > 0) {
                    angular.forEach(lstOneToManyAttributes, function (aobjOneToManyAttribute) {
                        if (aobjOneToManyAttribute.Entity) {
                            var lstTempEntity = $scope.lstEntity.filter(function (aobjEntityItem) { return aobjEntityItem.EntityName == aobjOneToManyAttribute.Entity });
                            if (lstTempEntity.length > 0) {
                                createdEntityObject.lstActiveAttributes.push(aobjOneToManyAttribute);
                                var lstIsEntityPresent = $scope.lstCreatedEntity.filter(function (astrEntity) { return astrEntity && astrEntity.toLowerCase() == aobjOneToManyAttribute.Entity.toLowerCase() });
                                if (lstIsEntityPresent.length == 0) {

                                    var objResultOneToMany = { lstOneToOne: [], lstOneToMany: [], entityName: lstTempEntity[0].EntityName, lstActiveAttributes: [] };
                                    createdEntityObject.lstOneToMany.push(objResultOneToMany);
                                    var lstOneToManyAttributes = [];
                                    angular.forEach(lstTempEntity[0].Attributes, function (aobjAttribute) {
                                        if (aobjAttribute.ID && aobjAttribute.Type && (aobjAttribute.Type == "Object" || aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                                            lstOneToManyAttributes.push(aobjAttribute.ID);
                                        }
                                    });
                                    $scope.exampleNodes.push({ "type": "Entity", id: lstTempEntity[0].EntityName, name: lstTempEntity[0].EntityName, attributes: lstOneToManyAttributes });
                                    $scope.lstCreatedEntity.push(lstTempEntity[0].EntityName);
                                    // $scope.CreateEntityDiagram(lstTempEntity[0], objResult.lstOneToMany);
                                }
                            }
                        }
                    });
                }
            }
        }
    }

    $scope.CreateEntityDiagramLinks = function (aobjEntity) {
        if (aobjEntity.lstActiveAttributes && aobjEntity.lstActiveAttributes.length > 0) {
            angular.forEach(aobjEntity.lstActiveAttributes, function (aobjAttribute) {
                var objAttribute = { source: aobjEntity.entityName, target: aobjAttribute.Entity, value: 5, type: "" };
                if (aobjAttribute.Type && aobjAttribute.Type == "Object") {
                    objAttribute.type = "onetoone";
                }
                else if (aobjAttribute.Type && (aobjAttribute.Type == "Collection" || aobjAttribute.Type == "List" || aobjAttribute.Type == "CDOCollection")) {
                    objAttribute.type = "onetomany";
                }
                var IsLinkAlreadyPresent = $scope.exampleLinks.filter(function (aobjLink) { return aobjLink.source == objAttribute.source && aobjLink.target == objAttribute.target && aobjLink.type == objAttribute.type });
                if (IsLinkAlreadyPresent.length == 0) {
                    $scope.exampleLinks.push(objAttribute);
                }
            });
            if (aobjEntity.lstOneToOne && aobjEntity.lstOneToOne.length > 0) {
                angular.forEach(aobjEntity.lstOneToOne, function (aobjOneToOneEntity) {
                    $scope.CreateEntityDiagramLinks(aobjOneToOneEntity);
                });
            }
            if (aobjEntity.lstOneToMany && aobjEntity.lstOneToMany.length > 0) {
                angular.forEach(aobjEntity.lstOneToMany, function (aobjOneToManyEntity) {
                    $scope.CreateEntityDiagramLinks(aobjOneToManyEntity);
                });
            }
        }
    }

    $scope.GetCreatedEntityObject = function (aobjEntity) {
        var result=null;
        for (var i = 0; i < $scope.objEntityDiagram.length; i++) {
            if ($scope.objEntityDiagram[i].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                result = $scope.objEntityDiagram[i];
            }
            if (!result && $scope.objEntityDiagram[i].lstOneToOne && $scope.objEntityDiagram[i].lstOneToOne.length > 0) {
                var lstOneToOneCreatedEntity = $scope.objEntityDiagram[i].lstOneToOne;
                for (var j = 0; j < lstOneToOneCreatedEntity.length; j++) {
                    if (lstOneToOneCreatedEntity[j].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                        result = lstOneToOneCreatedEntity[j];
                        break;
                    }
                }
            }
            if (!result && $scope.objEntityDiagram[i].lstOneToMany && $scope.objEntityDiagram[i].lstOneToMany.length > 0) {
                var lstOneToManyCreatedEntity = $scope.objEntityDiagram[i].lstOneToMany;
                for (var k = 0; k < lstOneToManyCreatedEntity.length; k++) {
                    if (lstOneToManyCreatedEntity[k].entityName.toLowerCase() == aobjEntity.EntityName.toLowerCase()) {
                        result = lstOneToManyCreatedEntity[j];
                        break;
                    }
                }
            }
            
        }
        return result;
    };
   
}]);
app.controller('NewERDiagram', ['$scope', '$rootScope', '$Chart', '$timeout', function ($scope, $rootScope, $Chart, $timeout) {
    $scope.currentOpenFileName = angular.copy($rootScope.currentopenfile.file.FileName);
    $scope.init = function () {
        $timeout(function () {
            $Chart.drawErDiagram($scope.currentOpenFileName);
        }); 
    };
    $scope.init();
}]);
app.controller("AddCodeGroupController", ["$scope", "hubcontext", "$rootScope", "$cookies", "$DashboardFactory", function ($scope, hubcontext, $rootScope, $cookies, $DashboardFactory) {

    $scope.SelectAllCheckBoxChange = function () {
        $scope.IsEntity = $scope.IsForm = $scope.IsHtmlForm = $scope.IsCorrespondence = $scope.IsReport = $scope.IsFile = $scope.IsSelectAll;
    };
    $scope.FileTypeCheckBoxChange = function () {
        if ($scope.IsEntity && $scope.IsForm && $scope.IsHtmlForm && $scope.IsCorrespondence && $scope.IsReport && $scope.IsFile) {
            $scope.IsSelectAll = true;
        }
        else {
            $scope.IsSelectAll = false;
        }
    };

    $scope.GetInvalidCodeGroup = function () {
        var lstFileTypes = [];
        if ($scope.IsEntity) {
            lstFileTypes.push('Entity');
        }
        if ($scope.IsForm) {
            lstFileTypes.push('Lookup');
            lstFileTypes.push('Maintenance');
            lstFileTypes.push('Wizard');
            lstFileTypes.push('UserControl');
            lstFileTypes.push('Tooltip');
        }
        if ($scope.IsHtmlForm) {
            lstFileTypes.push('FormLinkLookup');
            lstFileTypes.push('FormLinkMaintenance');
            lstFileTypes.push('FormLinkWizard');
        }
        if ($scope.IsCorrespondence) {
            lstFileTypes.push('Correspondence');
        }
        if ($scope.IsReport) {
            lstFileTypes.push('Report');
        }
        if ($scope.IsFile) {
            lstFileTypes.push('InboundFile');
        }
        if (lstFileTypes.length > 0) {
            $rootScope.IsLoading = true;
            hubcontext.hubTools.server.getInvalidCodeGroup(lstFileTypes.join(",")).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                    $scope.lstInvalidCodeGroups = data;
                });
            });
        }
        else {
            toastr.warning("Select at least one file type!");
        }
    };

    $scope.CloseAddCodeGroupDialog = function () {
        $scope.CodeGroupDialog.close();
    };
    $scope.onSaveCodeGroups = function () {
        if ($scope.lstInvalidCodeGroups && $scope.lstInvalidCodeGroups.length > 0) {
                $rootScope.IsLoading = true;
                hubcontext.hubMain.server.saveCodeGroups($scope.lstInvalidCodeGroups).done(function (lstUpdateFiles) {
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                    $scope.CloseAddCodeGroupDialog();
                    if (lstUpdateFiles.length) {
                        $rootScope.IsTfsUpdated = true;
                        if ($rootScope.isstartpagevisible) {
                            $DashboardFactory.getTfsStatusData();                            
                        }
                        for (var i = 0; i < lstUpdateFiles.length; i++) {
                            $rootScope.updateUserFileDetails(lstUpdateFiles[i]);
                        }
                        toastr.success(lstUpdateFiles.length + " file(s) saved successfully.");
                    }
                });
        }
    };

    $rootScope.checkAndCloseFilesAndSaveCodeGroups = function () {
        //Need to write logic for checking dirty files and show a confirmation dialog
        var newScope = $scope.$new();
        newScope.lstDirtyFiles = [];
        newScope.lstNotDirtyFiles = [];
        var temp = JSON.stringify($rootScope.lstopenedfiles);
        var tempOpenFile = JSON.parse(temp);
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
                        $scope.CheckInvalidCodeGroupFileAndClose(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $scope.CheckInvalidCodeGroupFileAndClose(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                    $scope.onSaveCodeGroups();
                }
                else if (buttontext == "No") {
                    for (var i = 0; i < newScope.lstDirtyFiles.length; i++) {
                        $scope.CheckInvalidCodeGroupFileAndClose(newScope.lstDirtyFiles[i].file.FileName);
                    }
                    for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                        $scope.CheckInvalidCodeGroupFileAndClose(newScope.lstNotDirtyFiles[i].file.FileName);
                    }
                    $scope.onSaveCodeGroups();
                }
                dialog.close();
            };
        }
        else {
            for (var i = 0; i < newScope.lstNotDirtyFiles.length; i++) {
                $scope.CheckInvalidCodeGroupFileAndClose(newScope.lstNotDirtyFiles[i].file.FileName);
            }
            $scope.onSaveCodeGroups();
        }
    };

    $scope.CheckInvalidCodeGroupFileAndClose=function(astrFileName){
        if($scope.lstInvalidCodeGroups && $scope.lstInvalidCodeGroups.length > 0 && astrFileName){
            var invalidCodeGroupFileExist=$scope.lstInvalidCodeGroups.filter(function(aObjfile){ return aObjfile.FileName && aObjfile.FileName.toLowerCase()== astrFileName.toLowerCase()});
            if(invalidCodeGroupFileExist.length>0){
                $rootScope.closeFile(astrFileName);
            }
        }
    }; 

}]);
app.controller("ValidationUtilityControl", ["$scope", "hubcontext", "$Errors", "$ModuleValidationService", "$rootScope", function ($scope, hubcontext, $Errors, $ModuleValidationService, $rootScope) {
    $scope.moduleType = "Form";
    $scope.istrStatusMessage = "";
    $scope.iblnFailed = false;
    $scope.lstValidatedFiles = [];
    $scope.istrFilevalidatedMsg = "";
    $scope.totalFileCount;
    $scope.fileValidatedCount;
    $scope.iobjModel;
    $scope.lobjResponseData = [];
    $scope.iblnProcessing = false;
    $scope.onModuleTypeChange = function (astrModuleType) {
        if (!$scope.iblnProcessing) {
            $scope.moduleType = astrModuleType;
        }
    };
    $scope.Validate = function () {
        $rootScope.IsLoading = true;
        $scope.lstValidatedFiles = [];
        $scope.istrStatusMessage = "Getting the files for Validation....";
        $scope.istrFilevalidatedMsg = "";
        $scope.totalFileCount = 0;
        $scope.fileValidatedCount = 0;
        $scope.lobjResponseData = [];
        hubcontext.hubValidationUtility.server.getFileList(1, $scope.moduleType).done(function (aobjResponseData) {
            $scope.$evalAsync(function () {
                $rootScope.IsLoading = false;
                if (aobjResponseData && aobjResponseData.length > 0) {

                    $scope.iblnProcessing = true;
                    $scope.lobjResponseData = aobjResponseData;
                    $scope.totalFileCount = aobjResponseData.length;
                    $scope.lstValidatedFiles = [];
                    $scope.istrStatusMessage = "Validation started....";
                    $scope.iobjModel = aobjResponseData[0];
                    aobjResponseData.splice(0, 1);
                    $scope.fileValidatedCount = $scope.totalFileCount - aobjResponseData.length;

                    $scope.iobjModel.Model.istrStatus = "Pending";
                    $scope.istrStatusMessage = "Validating " + $scope.iobjModel.FileName + " File....";
                    // $FormValidationService.validateFileData($scope.iobjModel.Model, $scope);
                    $ModuleValidationService.validate($scope.iobjModel.Model, $scope, $scope.moduleType);
                }
                else {
                    $scope.istrStatusMessage = "No files for validation....";
                }
            });
        });
    };
    $scope.validateFile = function (astrStatus) {
        if (astrStatus === "Failed") {
            $scope.iblnFailed = true;
            $scope.iblnProcessing = false;
            $scope.istrStatusMessage = "Cannot validate further due to error.";
        }
        else if (astrStatus === "Completed") {
            $scope.istrFilevalidatedMsg = $scope.fileValidatedCount + " out of " + $scope.totalFileCount;
            if ($Errors.validationListObj.length > 0 && $Errors.validationListObj[0].FileName === $scope.iobjModel.FileName) {
                var lobjValidatedFiles = { FileName: $scope.iobjModel.FileName, status: astrStatus, errorList: [], warningList: [] };

                var lobjErrorList = $Errors.validationListObj[0].errorList;
                var llstWarningList = $Errors.validationListObj[0].warningList;
                if (lobjErrorList && lobjErrorList.length) {
                for (var i = 0; i < lobjErrorList.length; i++) {
                    var lobjError = lobjErrorList[i];
                    var lstrControlId = lobjError.dictAttributes.ID ? lobjError.dictAttributes.ID : "";
                    lobjValidatedFiles.errorList.push({ istrControl: lobjError.Name, istrControlID: lstrControlId, errors: lobjError.errors });
                }
                }
                if (llstWarningList && llstWarningList.length) {
                    for (var i = 0; i < llstWarningList.length; i++) {
                        var lobjWarnning = llstWarningList[i];
                        var lstrControlId = lobjWarnning.dictAttributes.ID ? lobjWarnning.dictAttributes.ID : "";
                        lobjValidatedFiles.warningList.push({ istrControl: lobjWarnning.Name, istrControlID: lstrControlId, errors: lobjWarnning.warnings });
                    }
                }
                if (lobjValidatedFiles.errorList.length > 0 || lobjValidatedFiles.warningList.length > 0) {
                    $scope.lstValidatedFiles.push(lobjValidatedFiles);
                }
            }
            $Errors.validationListObj = [];
            if ($scope.lobjResponseData.length > 0) {
                $scope.iobjModel = $scope.lobjResponseData[0];
                $scope.lobjResponseData.splice(0, 1);
                $scope.fileValidatedCount = $scope.totalFileCount - $scope.lobjResponseData.length;
                $scope.iobjModel.Model.istrStatus = "Pending";
                $scope.istrStatusMessage = "Validating " + $scope.iobjModel.FileName + " File....";
                //  $FormValidationService.validateFileData($scope.iobjModel.Model, $scope);
                $ModuleValidationService.validate($scope.iobjModel.Model, $scope, $scope.moduleType);

            }
            else if ($scope.lobjResponseData.length == 0) {
                $scope.iblnProcessing = false;
                $scope.istrStatusMessage = "All files are validated.";
            }
        }
    };
    $scope.sendDataInPackets = function (data, operation) {
        var strpacket = "";
        var lstDataPackets = [];
        var count = 0;
        for (var i = 0, len = data.length; i < len; i++) {
            count++;
            strpacket = strpacket + data[i];
            if (count == 32000) {
                count = 0;
                lstDataPackets.push(strpacket);
                strpacket = "";
            }
        }
        if (count != 0) {
            lstDataPackets.push(strpacket);
        }

        for (var i = 0, len = lstDataPackets.length; i < len; i++) {
            hubMain.server.receiveDataPackets(lstDataPackets[i], len, $scope.currentopenfile, i, operation, []);
        }
    }
    $scope.ExportvalidationReport = function () {
        var lstrValidatedFiles = JSON.stringify($scope.lstValidatedFiles);
        if (lstrValidatedFiles.length < 32000) {
            hubcontext.hubMain.server.exportValidationReport($scope.lstValidatedFiles);
        }
        else {
            $scope.sendDataInPackets(lstrValidatedFiles, "GeneratevalidationReport")
        }

    };

    $scope.receiveExportvalidationReport = function (astrpath) {
        if (astrpath) {
            alert("Validation report generated at " + astrpath + " path.");
        }
    };
}]);