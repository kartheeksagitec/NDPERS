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
                }
                else {
                    $scope.lstWfmFiles = [];
                }
                $rootScope.IsLoading = false;
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
            else if (data == "ConversionCompleted") {
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
            if (data.File && data.File.FileName && data.Status == "Completed") {
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
            var lstTempFiles = [];
            for (var i = 0; i < lstFileDetails.length; i++) {
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