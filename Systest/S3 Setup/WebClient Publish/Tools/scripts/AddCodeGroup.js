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