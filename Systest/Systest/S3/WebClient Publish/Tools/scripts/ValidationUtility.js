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