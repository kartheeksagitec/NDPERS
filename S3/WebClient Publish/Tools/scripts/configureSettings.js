
app.controller("configureSettingsController", ['$scope', '$http', 'ngDialog', 'ConfigurationFactory', 'hubcontext', '$rootScope', '$cookies', '$EntityIntellisenseFactory', '$timeout', "CONSTANTS", function ($scope, $http, ngDialog, ConfigurationFactory, hubcontext, $rootScope, $cookies, $EntityIntellisenseFactory, $timeout, CONST) {
    $scope.showTfsDetails = false;
    $scope.showAddPortalDetailsTab = false;
    $scope.showPoratlDetailsTab = false;
    $scope.lstTempMappedUser = [];
    $scope.lstMappedUser = [];
    $scope.isSavePending = false;
    $scope.selectedConfigureTab = "ProjectSettings";
    $scope.projectTypes = [];
    var userCookie = $cookies.getObject("UserDetails");
    var currentUser = JSON.parse(userCookie);
    if (currentUser.UserType == "owner") {
        $scope.ShowUserTab = true;
    }
    else {
        $scope.ShowUserTab = false;
    }
    $scope.projectTypes = ConfigurationFactory.getProjectTypeProvider().ProjectTypeList;

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
        var objPostgreSQL = { ID: "PostgreSQL", IsSelected: "False", Attribute: "sfwPostgre" };
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
        newQueryTypeScope.lstDBTypes.push(objPostgreSQL);
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
                    "BusinessObjectLocation", "DataObjectLocation", "AssemblyNames", "BusinessObjectProjectLocation", "DataObjectProjectLocation",
                    "DefaultParentEntity", "CorrespondenceTemplatesLocation", "ReportsLocation", "HtmlTemplatesLocation", "BPMTemplatesLocation",
                    "IsTFS", "IsRemoteLogin", "TFSUrl", "TFSSourceLocation", "TFSScenarioSourceLocation", "TFPath",
                    "TfsLocalPath", "tfptPath", "CheckPendingChanges", "Website", "IsWorkFlow", "CorrespondenceTestsLocation", "ProjectType"];
                var objNew = {};
                objNew.ProjectName = newprojectName;
                for (var i = 0; i < keyNames.length; i++) {
                    if (keyNames[i] === "ProjectType") {
                        objNew[keyNames[i]] = 0;
                    }
                    else if (keyNames[i] == "XmlFileLocation") {
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

