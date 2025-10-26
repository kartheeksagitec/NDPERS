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
