app.controller("createNewObjectController", ["$scope", "$http", "$rootScope", "FormDetailsFactory", "$EntityIntellisenseFactory", "WizardHandler", "$ValidationService", "$getEnitityXmlMethods", "$filter", "$timeout", "ConfigurationFactory", "CONSTANTS", function ($scope, $http, $rootScope, FormDetailsFactory, $EntityIntellisenseFactory, WizardHandler, $ValidationService, $getEnitityXmlMethods, $filter, $timeout, ConfigurationFactory, CONSTANTS) {
    $scope.wizardHandler = WizardHandler;
    $scope.objNewItems = {};
    $scope.objEntityDetail = {};
    $scope.objUserCtrlDetails = {};
    $scope.objTooltipDetails = {};
    $scope.objOneToOne = {};
    $scope.objOneToOne.ParentTableCollection = [];
    $scope.objOneToMany = {};
    $scope.objOneToMany.ParentTableCollection = [];
    $scope.objCodeValues = {};
    $scope.objEntityDetail.CanFinishDisable = false;
    $scope.objRelatedProperties = {};
    $scope.IsCodeValueDisable = true;
    $scope.IsOneToOneOrManyDisable = false;
    $scope.objCorrespondenceDetail = {};
    $scope.objFileDetails = {};
    $scope.objInboundOutboundDetails = {};
    $scope.objNewItems.SelectedOption = $scope.SelectedCreationOption;
    $scope.objEntityDetail.validTableName = null;
    $scope.lstFormsIDList = [];

    $scope.lstFolder = [];
    $scope.lstMainTable = [];
    $scope.lstBusinessObject = [];
    $scope.lstMessages = [];
    $scope.lstAllFiles = [];
    $scope.objDirFunctions = {};
    $scope.IsSameAsNewDisabled = true;
    $scope.IsSameAsNewForWizardDisabled = false;
    $scope.objWorkflowMapDetails = {};
    $scope.disableCreateNewFolderBtn = true;
    $scope.projectType = null;

    //Getting xml file location and keeping in current controller scope.
    var currentProjectSettings = ConfigurationFactory.getLastProjectDetails();
    var appBasePath = ConfigurationFactory.getCurrentProjectAppBaseFolderName();
    var baseAppPath = ConfigurationFactory.getProjectTypeProvider().BaseApp.BasePath;
    if (currentProjectSettings) {
        $scope.projectType = currentProjectSettings.ProjectType;
    }
    if (currentProjectSettings.BaseDirectory && currentProjectSettings.XmlFileLocation) {
        $scope.xmlFileLocation = combineAsPath(currentProjectSettings.BaseDirectory, currentProjectSettings.XmlFileLocation, appBasePath);
        $scope.baseXmlFileLocation = combineAsPath(currentProjectSettings.BaseDirectory, currentProjectSettings.XmlFileLocation, baseAppPath);
    }
    currentProjectSettings = null;

    //$rootScope.IsWorkFlowMap = ConfigurationFactory.getLastProjectDetails().IsWorkFlow;
    $scope.objLocationCallback = {
        locationCallback: {}
    };

    //#region Init Section
    $.connection.hubCreateNewObject.server.getCommonFieldList().done(function (data) {
        if (data.length == 5) {
            $scope.lstMessages = data[0];
            $scope.lstFolder = data[1];
            $scope.lstMainTable = data[2];
            $scope.lstBusinessObject = data[3];
            $scope.lstAllFiles = data[4];

            if ($scope.SelectedEntity) {
                if ($scope.objNewItems.SelectedOption == "Forms") {
                    $scope.objFormsDetails.isNextDisable();
                }
                else if ($scope.objNewItems.SelectedOption == "Wizard") {
                    $scope.objWizardDetails.isFinsihDisable();
                }
                else if ($scope.objNewItems.SelectedOption == "UserControl") {

                    $scope.objUserCtrlDetails.isFinsihDisable();
                }
                else if ($scope.objNewItems.SelectedOption == "Correspondence") {
                    $scope.objCorrespondenceDetail.isFinishDisable();
                }
                else if ($scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
                    $scope.objFormLinkDetails.isFormLinkFinish();
                }
                else if ($scope.objNewItems.SelectedOption == "Tooltip") {
                    $scope.objTooltipDetails.isFinsihDisable();
                }
            }
        }
    });
    $.connection.hubMain.server.getResources("").done(function (data) {
        $scope.resourceList = $ValidationService.getListByPropertyName(data, "ResourceID", false);
    });
    var CreateNewExtraFieldObject = function (strNodeName) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        return objItem;
    };

    var setExtraFieldsVariables = function (objNameParam) {
        // Extra fields varibales
        //  $scope.objDirFunctions = {};
        $scope.objExtraFields = {};
        $scope.showExtraFieldsTab = false;
        if (objNameParam == "Forms" || objNameParam == "Wizard" || objNameParam == "UserControl" || objNameParam == "Tooltip") {
            $scope.objName = "Form";
        }
        else if (objNameParam == "BPM Map" || objNameParam == "BPMN Map Template") {
            $scope.objName = "BPM";
        }
        else if (objNameParam == "Html Lookup" || objNameParam == "Html Maintenance" || objNameParam == "Html Wizard") {
            $scope.objName = "HTML";
        }
        else if (objNameParam) {
            $scope.objName = objNameParam;
        }
        $scope.objExtraFields = CreateNewExtraFieldObject("ExtraFields");
    };

    $scope.OnNextPageLoad = function () {
        //if ($scope.objDirFunctions.checkVisibility) {
        //    $scope.showExtraFieldsTab = $scope.objDirFunctions.checkVisibility();
        //}

        if ($scope.objNewItems.SelectedOption == "Entity") {
            $scope.objEntityDetail.Init();
            $scope.objEntityDetail.isNextDisableForEntity = true;
        }
        else if ($scope.objNewItems.SelectedOption == "Correspondence") {
            $scope.objCorrespondenceDetail.InitCorrespondence();
        }
        else if ($scope.objNewItems.SelectedOption == "File") {
            $scope.objFileDetails.InitFile();
            $scope.PopulateBasePath();
            $scope.populatePathCode();
            $.connection.hubCreateNewObject.server.getIsPathKeyDetails().done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.IsPathKey = data;
                });
            });
            $.connection.hubCreateNewObject.server.getIsFileIdValueValid().done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.objFileDetails.IsFileIdValueValid = data;
                });
            });
        }

        else if ($scope.objNewItems.SelectedOption == "Forms") {
            $scope.objFormsDetails.InitForms();
        }
        else if ($scope.objNewItems.SelectedOption == "Wizard") {
            $scope.objWizardDetails.InitWizard();
        }
        else if ($scope.objNewItems.SelectedOption == "UserControl") {
            $scope.objUserCtrlDetails.InitUserControl();
        }
        else if ($scope.objNewItems.SelectedOption == "Tooltip") {
            $scope.objTooltipDetails.InitTooltip();
        }
        else if ($scope.objNewItems.SelectedOption == "Report") {
            $scope.objReportDetails.InitForms();
        }
        else if ($scope.objNewItems.SelectedOption == "BPMN" || $scope.objNewItems.SelectedOption == "BPMTemplate") {
            $scope.initializeBpmn();
        }
        else if ($scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
            $scope.objFormLinkDetails.InitFormLink();
        }
        else if ($scope.objNewItems.SelectedOption == "PrototypeWizard" || $scope.objNewItems.SelectedOption == "PrototypeLookup" || $scope.objNewItems.SelectedOption == "PrototypeMaintenance") {
            $scope.objPrototypeFormDetails.InitWizard();
        }

        else if ($scope.objNewItems.SelectedOption == "WorkflowMap") {
            $scope.objWorkflowMapDetails.InitWorkflowMap();
        }
    };

    setExtraFieldsVariables("Forms");
    //#endregion

    //#region Forms Creation Variables
    $scope.objFormsDetails = {};
    $scope.objFormsDetails.IsShowMainQuery = false;
    $scope.objLookupFieldsDetails = {};
    $scope.IsMaintenance = false;
    $scope.IsLookup = false;
    $scope.objLookupResultFields = {};
    $scope.objLookupResultFields.lstEntity = [];
    $scope.objLookupResultFields.SelecetdField = [];
    $scope.objLookupResultFields.IsNewButton = 'True';
    $scope.objLookupResultFields.IsOpenButton = 'True';
    $scope.objLookupResultFields.IsDeleteButton = 'True';
    $scope.objLookupResultFields.IsExportExcelButton = 'True';
    $scope.ControlTypes = ['', 'TextBox', 'DropDownList'];
    $scope.lstControlTypes = ['', 'Label'];

    $scope.lstDataFormat = ['', '{0:d}', '{0:C}', '{0:000-##-####}', '{0:(###)###-####}'];
    $scope.lstDataKey = ['', '1', '2', '3', '4', '5'];
    $scope.lstColumns = ['4', '6', '8', '10'];
    $scope.objLookupFieldsDetails.lstFieldsProperties = [];
    $scope.objLookupResultFields.lstResultFieldsForGrid = [];
    $scope.objLookupFieldsDetails.lstselectedmultiplelevelfield = [];
    $scope.objLookupResultFields.lstselectedmultiplelevelfield = [];
    $scope.IsSameAsNewForMethod = false;
    $scope.IsSameAsNewForWizard = false;
    $scope.objLookupFieldsDetails.lstSelectedFields = [];
    //#endregion

    //#region Wizard Creation variable
    $scope.objWizardDetails = {};
    //#endregion

    //#region Prototype Creation variable
    $scope.objPrototypeFormDetails = {};
    $scope.objPrototypeLookupColumns = {};
    $scope.objPrototypeLookupColumns.lstColumns = [];
    $scope.objPrototypeLookupColumns.ControlTypes = ['', 'Label', 'HyperLink', 'CheckBox'];

    //#endregion

    //#region Report Variable
    $scope.objReportDetails = {};
    //#endregion

    //#region FormLink Creation Variables
    $scope.objFormLinkDetails = {};
    $scope.objFormLinkDetails.lstSubqueries = [];
    $scope.objFormLinkDetails.NewMethod = [];
    $scope.objFormLinkDetails.UpdateMethod = [];
    //#endregion

    //#region Finish
    $scope.OnFinishClick = function () {
        $rootScope.IsLoading = true;
        function iterator(obj) {
            obj.dtDOColumnDetail = [];
            obj.ForeigenKeyCollection = [];
            obj.lstTableCodeValue = [];
            obj.SortOrderCollection = [];
        }

        if ($scope.objNewItems.SelectedOption == "Entity") {
            // #region extra field data
            if ($scope.objDirFunctions.prepareExtraFieldData) {
                $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
                if ($scope.objDirFunctions.getNewExtraFieldData) {
                    $scope.objExtraFields = $scope.objDirFunctions.getNewExtraFieldData(); // getting extra field data from extraFieldDirective
                }
            }
            //#endregion
            if (!$scope.objEntityDetail.TableName) {
                $scope.objEntityDetail.dtDOTableColumns = [];
                $scope.objEntityDetail.lstTableCodeValue = [];
                $scope.objEntityDetail.OneToOneSelectedTableCollection = [];
                $scope.objEntityDetail.OneToManySelectedTableCollection = [];
            }
            else {
                angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iterator);
                angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iterator);
            }
            $scope.CreateEntity($scope.objEntityDetail.TableName, $scope.objEntityDetail.FormattedClassName, $scope.objEntityDetail.Path, $scope.objEntityDetail.lstCodeValues, true, $scope.objEntityDetail.EntityID, $scope.objEntityDetail.Description, $scope.objEntityDetail.BusinessObjectName, $scope.objEntityDetail.ParentEntity, $scope.objEntityDetail.OneToOneSelectedTableCollection, $scope.objEntityDetail.OneToManySelectedTableCollection, $scope.blnShowConsolidatedBORelatedProperties);
        }
        else if ($scope.objNewItems.SelectedOption == "Correspondence") {
            $scope.CreateCorrespondense();
        }
        else if ($scope.objNewItems.SelectedOption == "File") {
            $scope.CreateFile();
        }
        else if ($scope.objNewItems.SelectedOption == "Forms") {
            $scope.Count = 0;
            if ($scope.objFormsDetails.IsMaintenence && $scope.objFormsDetails.IsMaintenence.toLowerCase() == "true") {
                // $scope.CreateForms();
                if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() != "true") {
                    $scope.UpdateDataForMaintenance();
                }
                $scope.UpdateCenterMiddleForMaintenance();
                var obj = {};

                var objFileDetails = {
                    FileType: 'Maintenance', FileName: "wfm" + $scope.objFormsDetails.ID + "Maintenance", FilePath: $scope.objFormsDetails.Path, FileID: "wfm" + $scope.objFormsDetails.ID + "Maintenance"
                };
                getExtraFields($scope.objFormsDetails.objSfxMaintenanceForm);
                $rootScope.SaveModelWithPackets($scope.objFormsDetails.objSfxMaintenanceForm, objFileDetails, undefined, true);

                if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
                    $scope.UpdateResultFields();
                    $scope.UpdateResultPanel();
                    getExtraFields($scope.objFormsDetails.objSfxLookupForm);
                    FormDetailsFactory.setFormDetails($scope.objFormsDetails);
                }
            }
            else if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
                $scope.UpdateResultFields();
                $scope.UpdateResultPanel();
                var obj = {};

                var objFileDetails = {
                    FileType: 'Lookup', FileName: "wfm" + $scope.objFormsDetails.ID + "Lookup", FilePath: $scope.objFormsDetails.Path, FileID: "wfm" + $scope.objFormsDetails.ID + "Lookup"
                };
                getExtraFields($scope.objFormsDetails.objSfxLookupForm);
                $rootScope.SaveModelWithPackets($scope.objFormsDetails.objSfxLookupForm, objFileDetails, undefined, true);
            }
        }
        else if ($scope.objNewItems.SelectedOption == "UserControl") {
            $scope.objUserCtrlDetails.UpdateData();
            var objFileDetails = {
                FileType: 'UserControl', FileName: "udc" + $scope.objUserCtrlDetails.ID + "UserControl", FilePath: $scope.objUserCtrlDetails.Path, FileID: "udc" + $scope.objUserCtrlDetails.ID + "UserControl"
            };
            getExtraFields($scope.objUserCtrl);
            $rootScope.SaveModelWithPackets($scope.objUserCtrl, objFileDetails, undefined, true);
        }
        else if ($scope.objNewItems.SelectedOption == "Tooltip") {
            $scope.objTooltipDetails.UpdateData();
            var objFileDetails = { FileType: 'Tooltip', FileName: "wfm" + $scope.objTooltipDetails.ID + "Tooltip", FilePath: $scope.objTooltipDetails.Path, FileID: "wfm" + $scope.objTooltipDetails.ID + "Tooltip" };
            getExtraFields($scope.objTooltipForm);
            $rootScope.SaveModelWithPackets($scope.objTooltipForm, objFileDetails, undefined, true);
        }
        else if ($scope.objNewItems.SelectedOption == "Wizard") {
            $scope.objWizardDetails.UpdateData();

            var objFileDetails = {
                FileType: 'Wizard', FileName: "wfm" + $scope.objWizardDetails.ID + "Wizard", FilePath: $scope.objWizardDetails.Path, FileID: "wfm" + $scope.objWizardDetails.ID + "Wizard"
            };
            getExtraFields($scope.objSfxWizardForm);
            $rootScope.SaveModelWithPackets($scope.objSfxWizardForm, objFileDetails, undefined, true);

        }
        else if ($scope.objNewItems.SelectedOption == "Report") {

            $scope.objReportDetails.UpdateData();

            var objFileDetails = {
                FileType: 'Report', FileName: "rpt" + $scope.objReportDetails.ID, FilePath: $scope.objReportDetails.Path, FileID: "rpt" + $scope.objReportDetails.ID
            };
            getExtraFields($scope.objReport);
            $rootScope.SaveModelWithPackets($scope.objReport, objFileDetails, undefined, true);
        }
        else if ($scope.objNewItems.SelectedOption == "BPMN" || $scope.objNewItems.SelectedOption == "BPMTemplate") {
            // #region extra field data
            if ($scope.objDirFunctions.prepareExtraFieldData) {
                $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for creating extra field data
                if ($scope.objDirFunctions.getNewExtraFieldData) {
                    $scope.bpmDetails.bpmModel.ExtraFields = $scope.objDirFunctions.getNewExtraFieldData(); // getting extra field data from extraFieldDirective
                }
            }
            //#endregion
            $.connection.hubBPMN.server.createNewBPMN($scope.bpmDetails.bpmModel).done(function (data) {
                var file = data;
                if (file) {
                    $rootScope.$apply(function () {
                        $rootScope.openFile(file, true);
                    });
                }


                $scope.dialogForNew.close();
                $rootScope.IsLoading = false;

            });
        }
        else if ($scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
            var fileType = "";
            if ($scope.objNewItems.SelectedOption == "HtxLookup")
                fileType = 'FormLinkLookup';
            else if ($scope.objNewItems.SelectedOption == "HtxMaintenance")
                fileType = 'FormLinkMaintenance';
            else if ($scope.objNewItems.SelectedOption == "HtxWizard")
                fileType = 'FormLinkWizard';

            var objFileDetails = {
                FileType: fileType, FileName: "htx" + $scope.objFormLinkDetails.ID, FilePath: $scope.objFormLinkDetails.Path, FileID: "htx" + $scope.objFormLinkDetails.ID
            };

            var obj = {};

            obj.ID = $scope.objFormLinkDetails.ID;

            obj.Path = $scope.objFormLinkDetails.Path;
            obj.sfwEntity = $scope.objFormLinkDetails.sfwEntity;
            obj.HtmlTemplatePath = $scope.objFormLinkDetails.HtmlTemplatePath;
            obj.MainQueryID = $scope.objFormLinkDetails.MainQueryID;
            obj.FileName = objFileDetails.FileName;
            obj.FileType = objFileDetails.FileType;
            obj.HtmlFolderTemplatePath = $scope.objFormLinkDetails.HtmlFolderTemplatePath;

            // #region extra field data
            if ($scope.objDirFunctions.prepareExtraFieldData) {
                $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for creating extra field data
            }
            obj.objExtraFields = $scope.objExtraFields; // insert extra fields datai in main object
            $scope.objFormLinkDetails.FormInfo = obj;
            $.connection.hubCreateNewObject.server.populateMapUnmapControls(obj).done(function (lstReturn) {
                $scope.receiveMapUnmapControls(lstReturn[0], lstReturn[1]);
            });
        }
        else if ($scope.objNewItems.SelectedOption == "PrototypeLookup" || $scope.objNewItems.SelectedOption == "PrototypeMaintenance" || $scope.objNewItems.SelectedOption == "PrototypeWizard") {
            var fileType = "";
            if ($scope.objNewItems.SelectedOption == "PrototypeLookup") {
                fileType = 'Lookup';
                $scope.objPrototypeFormDetails.UpdateData(fileType);
            }
            else if ($scope.objNewItems.SelectedOption == "PrototypeMaintenance") {
                fileType = 'Maintenance';
                $scope.objPrototypeFormDetails.UpdateData(fileType);
            }
            else if ($scope.objNewItems.SelectedOption == "PrototypeWizard") {
                fileType = 'Wizard';
                $scope.objPrototypeFormDetails.UpdateData(fileType);
            }

            var objFileDetails = {
                FileType: fileType, FileName: "wfp" + $scope.objPrototypeFormDetails.ID + fileType, FileID: "wfp" + $scope.objPrototypeFormDetails.ID + fileType
            };
            getExtraFields($scope.objSfxPrototypeForm);
            $rootScope.SaveModelWithPackets($scope.objSfxPrototypeForm, objFileDetails, undefined, true);
        }


        else if ($scope.objNewItems.SelectedOption == "WorkflowMap") {

            var objWorkflowMapDetails = {
                FileID: $scope.objWorkflowMapDetails.FileID, ProcessType: $scope.objWorkflowMapDetails.ProcessType, Path: $scope.objWorkflowMapDetails.Path
            };
            $.connection.hubCreateNewObject.server.createWorkflowMap(objWorkflowMapDetails).done(function (data) {
                var file = data;
                if (file) {
                    $rootScope.$apply(function () {
                        $rootScope.openFile(file, true);

                    });
                }
                $scope.dialogForNew.close();
                $rootScope.IsLoading = false;
            });
        }
    };
    //#endregion


    //#region Save model for Forms(Lookup)
    $scope.receiveModelForForms = function (data) {
        var lst = data;
        $scope.$evalAsync(function () {
            var entityScope = getScopeByFileName($scope.SelectedEntity);
            if (entityScope != undefined) {
                if (entityScope && entityScope.lstItemsFormsList) {
                    entityScope.lstItemsFormsList.push(lst);
                }
            }
            if ($scope.objFormsDetails && $scope.objFormsDetails.IsMaintenence && $scope.objFormsDetails.IsMaintenence.toLowerCase() == "true" && $scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
                $scope.Count++;
                if ($scope.Count == 1) {
                    $rootScope.openFile(lst, true);
                }
                else if ($scope.Count == 2) {
                    $scope.dialogForNew.close();
                    $rootScope.openFile(lst, true);
                    $rootScope.IsLoading = false;
                }
            } else {
                $scope.dialogForNew.close();
                $rootScope.openFile(lst, true);
                $rootScope.IsLoading = false;
            }
        });
    };
    //#endregion

    //#region BPMN Methods
    $scope.initializeBpmn = function () {
        $scope.bpmDetails = { bpmModel: {}, ErrorMessageForDisplay: "", isValid: false, templates: [], templatePath: "" };
        $scope.bpmDetails.bpmModel = { FileID: "", Title: "", Template: "", Location: "", IsTemplate: $scope.objNewItems.SelectedOption == "BPMTemplate" };
        $.connection.hubBPMN.server.getBpmnTemplatePath().done(function (location) {
            $scope.$apply(function () {
                if ($scope.objNewItems.SelectedOption == "BPMTemplate") {
                    $scope.bpmDetails.bpmModel.Location = location;
                }
                $scope.bpmDetails.templatePath = location;
                if ($scope.bpmDetails.templatePath && $scope.bpmDetails.templatePath.trim().length > 0) {
                    $scope.bpmDetails.templates = $scope.lstAllFiles.filter(function (x) { return x.FilePath.indexOf($scope.bpmDetails.templatePath) == 0; }).map(function (y) { return y.FileName; });
                }
                $scope.bpmDetails.isValidBpmDetails();
            });
        });
        $scope.bpmDetails.onTemplateKeyDown = function (event) {
            var input = event.target;
            if ($scope.bpmDetails.templates && $scope.bpmDetails.templates.length > 0) setSingleLevelAutoComplete($(input), $scope.bpmDetails.templates);

            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE) {
                if ($(input).data('ui-autocomplete')) $(input).autocomplete("search", $(input).val().trim());
                event.preventDefault();
            }
        };
        $scope.bpmDetails.showTemplateIntellisenseList = function (event) {
            $('#creatbpmtemplateinput').focus();
            if ($scope.bpmDetails.templates && $scope.bpmDetails.templates.length > 0) {
                setSingleLevelAutoComplete($('#creatbpmtemplateinput'), $scope.bpmDetails.templates);
                if ($('#creatbpmtemplateinput').data('ui-autocomplete')) $('#creatbpmtemplateinput').autocomplete("search", $('#creatbpmtemplateinput').val());
            }
        };
        $scope.bpmDetails.canFinishBpmStep = function () {
            return $scope.bpmDetails.isValid;
        };
        $scope.bpmDetails.isValidBpmDetails = function () {
            $scope.bpmDetails.isValid = true;
            $scope.bpmDetails.ErrorMessageForDisplay = "";
            if ($scope.objNewItems.SelectedOption == "BPMTemplate" && !($scope.bpmDetails.templatePath && $scope.bpmDetails.templatePath.trim().length > 0)) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: Template Path not set in Project Settings.";
                $scope.bpmDetails.isValid = false;
            }
            else if (!($scope.bpmDetails.bpmModel.FileID && $scope.bpmDetails.bpmModel.FileID.trim().length > 0)) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: Please enter a File Name (ID).";
                $scope.bpmDetails.isValid = false;
            }
            else if (($scope.bpmDetails.bpmModel.FileID != undefined || $scope.bpmDetails.bpmModel.FileID != "") && $scope.bpmDetails.bpmModel.FileID.length <= 3) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
                $scope.bpmDetails.isValid = false;
            }

            else if ($scope.bpmDetails.isExistingBpm()) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: A file with same name already exists. Please give a different file name.";
                $scope.bpmDetails.isValid = false;
            }
            else if (!isValidFileID($scope.bpmDetails.bpmModel.FileID)) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: File Name must not start with digits, contain space(s), special characters(except '-').";
                $scope.bpmDetails.isValid = false;
            }
            else if ($scope.bpmDetails.bpmModel.Template && $scope.bpmDetails.bpmModel.Template.trim().length > 0 && $scope.bpmDetails.templates.indexOf($scope.bpmDetails.bpmModel.Template) == -1) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: Invalid Template.";
                $scope.bpmDetails.isValid = false;
            }
            else if (!$scope.ValidatePath($scope.bpmDetails.bpmModel, $scope.bpmDetails.bpmModel.Location)) {
                $scope.bpmDetails.ErrorMessageForDisplay = "Error: Invalid Path or click to create new button.";
                $scope.bpmDetails.isValid = false;
            }
            $scope.bpmDetails.isBpmDetailsValid = $scope.bpmDetails.isValid;
            return $scope.bpmDetails.isValid;
        };
        $scope.bpmDetails.isValidBpmPath = function () {
            if ($scope.bpmDetails.bpmModel.Location != "" && $scope.bpmDetails.bpmModel.Location != null && $scope.bpmDetails.bpmModel.Location != undefined) {
                return $scope.lstFolder.filter(function (x) { return x.ID == $scope.bpmDetails.bpmModel.Location; }).length > 0 || $scope.bpmDetails.bpmModel.Location == $scope.bpmDetails.templatePath;
            } else {
                return false;
            }
        };
        $scope.bpmDetails.isExistingBpm = function () {
            return $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "sbp" + $scope.bpmDetails.bpmModel.FileID.toLowerCase() }).length > 0;
        };
        $scope.bpmDetails.isValidBpmDetails();
    };
    //#endregion

    //#region Create Entity Methods 

    //#region Init Methods for Entity Details


    $scope.objEntityDetail.CanShowEntitySteps = function () {
        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "Entity") {
            retVal = false;
        }
        return retVal;
    };

    $scope.LoadFolders = function () {
        //Load Folder From Business Object Location
    };

    $scope.objEntityDetail.Init = function () {
        $scope.objEntityDetail.MasterTables = [];
        $scope.objEntityDetail.lstCodeValues = [];
        $scope.objEntityDetail.OneToOneSelectedTableCollection = [];
        $scope.objEntityDetail.OneToManySelectedTableCollection = [];
        $scope.objEntityDetail.lstTableCodeValue = [];
        $scope.objEntityDetail.TableName = "";
        $scope.objEntityDetail.BusinessObjectName = "";
        $scope.objEntityDetail.Path = "";
        $scope.objEntityDetail.EntityID = "";
        $scope.objEntityDetail.dtDOTableColumns = [];
        $scope.objEntityDetail.FormattedClassName = "";
        $scope.validateEntTableSchema();
        $scope.objEntityDetail.isNextDisable();
    };

    //#region Entity Details Validation

    $scope.ValidateTableName = function () {
        var isValid = true;
        if ($scope.objEntityDetail.TableName && $scope.objEntityDetail.TableName.trim().length > 0) {
            if (!$scope.lstMainTable.some(function (x) { return x.ID == $scope.objEntityDetail.TableName; })) {
                $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Table Name.";
                isValid = false;
            }
            else {
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var entities = entityIntellisenseList.filter(function (x) { return x.TableName == $scope.objEntityDetail.TableName; });
                if (entities.length > 0) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Table name is already binded with " + entities[0].ID + ". Select another table.";
                    isValid = false;
                }
            }
        }
        if (!$scope.objEntityDetail.TableName) {
            $scope.objEntityDetail.validTableName = null;
        } else {
            $scope.objEntityDetail.validTableName = $scope.objEntityDetail.TableName;
        }
        return isValid;
    };

    $scope.ValidateBusObject = function (blnCheckIfExists) {
        var isValid = true;
        $scope.objEntityDetail.ErrorMessageForDisplay = "";

        if ($scope.objEntityDetail.BusinessObjectName && $scope.objEntityDetail.BusinessObjectName != "") {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if ($scope.objEntityDetail.IsExistingEntity) {
                if (!entityIntellisenseList.some(function (x) { return x.ID == "ent" + $scope.objEntityDetail.EntityID && x.BusinessObjectName == $scope.objEntityDetail.BusinessObjectName; })) {
                    if (entityIntellisenseList.some(function (x) { return x.BusinessObjectName == $scope.objEntityDetail.BusinessObjectName; })) {
                        $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Business Object.The business object is used in another entity.";
                        isValid = false;
                    }
                }
            }
            else if (entityIntellisenseList.some(function (x) { return x.BusinessObjectName == $scope.objEntityDetail.BusinessObjectName; })) {
                $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Business Object.The business object is used in another entity.";
                isValid = false;
            }

            if (blnCheckIfExists) {
                if (!$scope.lstBusinessObject.some(function (x) { return x.ID == $scope.objEntityDetail.BusinessObjectName; })) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Business Object.";
                    isValid = false;
                }

            }



        }
        return isValid;
    };



    $scope.ValidateID = function () {
        var isValid = true;
        if ($scope.objEntityDetail.EntityID == undefined || $scope.objEntityDetail.EntityID == "") {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: ID cannot be empty.";
            isValid = false;
        }
        else if ($scope.objEntityDetail.EntityID && $scope.objEntityDetail.EntityID.toLowerCase().startsWith("ent")) {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: ID cannot start with ent.";
            isValid = false;
        }
        else if (($scope.objEntityDetail.EntityID != undefined || $scope.objEntityDetail.EntityID != "") && $scope.objEntityDetail.EntityID.length > 90) {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid ID. The length of ID cannot exceed 90 characters.";
            isValid = false;
        }
        else if (($scope.objEntityDetail.EntityID != undefined || $scope.objEntityDetail.EntityID != "") && $scope.objEntityDetail.EntityID.length <= 3) {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            isValid = false;
        }
        else if (($scope.objEntityDetail.EntityID != undefined || $scope.objEntityDetail.EntityID != "") && $scope.objEntityDetail.EntityID.toLowerCase() == "base") {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: entBase can not be created.";
            isValid = false;
        }
        else if (!isValidFileID($scope.objEntityDetail.EntityID)) {
            $scope.objEntityDetail.ErrorMessageForDisplay = "Error: File Name must not start with digits, contain space(s), special characters(except '-').";
            isValid = false;
        }
        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != "") {
                if (entityIntellisenseList.some(function (x) { return x.ID.toLowerCase() == "ent" + $scope.objEntityDetail.EntityID.toLowerCase() && ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != "") && x.TableName; })) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: The entity already exists. Please enter a different Name (ID).";
                    isValid = false;
                }
                else if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.some(function (x) { return x.file && x.file.FileName && x.file.FileName.toLowerCase() == "ent" + $scope.objEntityDetail.EntityID.toLowerCase(); })) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: The entity is already opened. Please close it first and then update.";
                    isValid = false;
                }
            }
            else {
                if (entityIntellisenseList.some(function (x) { return x.ID.toLowerCase() == "ent" + $scope.objEntityDetail.EntityID.toLowerCase(); })) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: The entity already exists. Please enter a different Name (ID).";
                    isValid = false;
                }
                else if ($rootScope.lstopenedfiles && $rootScope.lstopenedfiles.some(function (x) { return x.file && x.file.ID && x.file.ID.toLowerCase() == "ent" + $scope.objEntityDetail.EntityID.toLowerCase(); })) {
                    $scope.objEntityDetail.ErrorMessageForDisplay = "Error: The entity is already opened. Please close it first and then update.";
                    isValid = false;
                }
            }
        }
        return isValid;
    };

    $scope.validateParentEntity = function () {
        var isValid = true;
        if ($scope.objEntityDetail.ParentEntity && $scope.objEntityDetail.ParentEntity != '') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = entityIntellisenseList.filter(function (x) {
                if (x.ID && x.ID.toLowerCase() == $scope.objEntityDetail.ParentEntity.toLowerCase()) {
                    return x;
                }
            });
            if (angular.isArray(entity) && entity.length <= 0) {
                $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Parent Entity.";
                isValid = false;
            } else if (angular.isArray(entity) && entity.length > 0 && $scope.objEntityDetail.TableName && entity[0].TableName) {
                $scope.objEntityDetail.ErrorMessageForDisplay = "Error: Invalid Parent Entity.";
                isValid = false;
            }

        }
        return isValid;
    };

    $scope.ValidatePath = function (details, path) {
        if (!path) {
            details.ErrorMessageForDisplay = "Error: Path / Folder cannot be empty.";
            return false;
        }
        else {
            if (details.errors && details.errors["invalid_path"]) {
                if (details.errors["invalid_path"] == "Folder Not Exist") {
                    details.ErrorMessageForDisplay = "Info: Folder doesn't exist. Please Click Create New Folder Button.";
                }
                else {
                    details.ErrorMessageForDisplay = 'Error: Invalid Folder Path.';
                }
                return false;
            } else {
                details.ErrorMessageForDisplay = "";
                return true;
            }
        }
        return true;
    };
    var strError = "";
    $scope.ValidateBoDoTemplate = function () {
        var isValid = true;
        if ($scope.objEntityDetail.TableName && $scope.objEntityDetail.TableName.trim().length > 0) {
            if (strError == undefined || strError == "") {
                $.connection.hubCreateNewObject.server.checkForBoDoFile().done(function (data) {
                    if (data && data.length > 0) {
                        $scope.objEntityDetail.ErrorMessageForDisplay = strError = "Error: " + data;
                        isValid = false;
                    }
                });
            }
            else {
                $scope.objEntityDetail.ErrorMessageForDisplay = strError;
            }
        }
        return isValid;
    };

    $scope.ValidateEntityDetails = function () {
        var retVal = false;
        $scope.objEntityDetail.ErrorMessageForDisplay = "";
        $scope.objEntityDetail.validTableName = null;
        //entity creation without business object and without table
        if (!$scope.objEntityDetail.TableName && !$scope.objEntityDetail.BusinessObjectName && $scope.ValidateID() && $scope.ValidatePath($scope.objEntityDetail, $scope.objEntityDetail.Path) && $scope.validateParentEntity()) {
            retVal = true;
        }
        //entity creation without business object and with table - this entity will be created for CDOCollection type attributes
        else if ($scope.objEntityDetail.TableName && !$scope.objEntityDetail.BusinessObjectName && $scope.ValidateID() && $scope.ValidateTableName() && $scope.ValidatePath($scope.objEntityDetail, $scope.objEntityDetail.Path) && $scope.ValidateBoDoTemplate() && $scope.validateParentEntity()) {
            retVal = true;
        }
        //entity creation with business object and without table - this entity will be created for custom classes user has added in solution side which donot have any do file associated
        //only existing business object can be used
        else if (!$scope.objEntityDetail.TableName && $scope.objEntityDetail.BusinessObjectName && $scope.ValidateID() && $scope.ValidateBusObject(true) && $scope.ValidatePath($scope.objEntityDetail, $scope.objEntityDetail.Path) && $scope.ValidateBoDoTemplate() && $scope.validateParentEntity()) {
            retVal = true;
        }
        //entity creation with new business object and with table
        else if ($scope.objEntityDetail.TableName && $scope.objEntityDetail.BusinessObjectName && $scope.ValidateTableName() && $scope.ValidateID(false) && $scope.ValidateBusObject(false) && $scope.validateParentEntity() && $scope.ValidatePath($scope.objEntityDetail, $scope.objEntityDetail.Path) && $scope.ValidateBoDoTemplate()) {
            retVal = true;
        }

        return retVal;
    };

    //#endregion

    $scope.TableNameChangedClick = function () {
        $scope.blnShowConsolidatedBORelatedProperties = false;
        $scope.objEntityDetail.CanFinishDisable = false;
        $scope.isLoadOneToManyData = false;
        $scope.objEntityDetail.isBusObjectDisabled = false;
        //if table name is empty then we will not clear the existing entity name and other data
        function iterator(file) {
            if (file.FileName == "ent" + $scope.objEntityDetail.EntityID) {
                entityDetail = file;
            }
        }

        if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != "") {
            $scope.ClearFields();
            $scope.objEntityDetail.CanFinishDisable = true;
            $scope.objEntityDetail.EntityID = GetFormattedTableName($scope.objEntityDetail.TableName.toLowerCase());
            $scope.objEntityDetail.FormattedClassName = $scope.objEntityDetail.EntityID;
            if ($scope.objFormsDetails.IsCreatewithbusinessobject) {
                $scope.objEntityDetail.BusinessObjectName = "bus" + $scope.objEntityDetail.EntityID;
                $scope.blnShowConsolidatedBORelatedProperties = true;
            }
            if ($scope.lstAllFiles != undefined) {
                var entityDetail;

                angular.forEach($scope.lstAllFiles, iterator);

                if (entityDetail != undefined) {
                    $scope.objEntityDetail.Path = entityDetail.Location;
                }
            }

            if ($scope.lstBusinessObject.some(function (x) { return x.ID == $scope.objEntityDetail.BusinessObjectName; })) {
                $scope.IsOneToOneOrManyDisable = true;
            }
            else {
                $scope.IsOneToOneOrManyDisable = false;
            }

            if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != "") {
                $scope.objEntityDetail.dtDOTableColumns = [];
                //$scope.objEntityDetail.dtCDOXmlColumns = [];
            }
            $scope.objEntityDetail.CheckForExistingEntity();

            $.connection.hubCreateNewObject.server.updatedCodeValuesForMainTable($scope.objEntityDetail.TableName).done(function (lstcodevalues) {
                $scope.objEntityDetail.lstTableCodeValue = lstcodevalues;
                $scope.$apply(function () {
                    if ($scope.objEntityDetail.lstTableCodeValue.length > 0) {
                        $scope.IsCodeValueDisable = false;
                        $scope.objEntityDetail.isNextDisable();
                    }
                    else {
                        $scope.IsCodeValueDisable = true;
                    }
                });
            });
        }
        $scope.objEntityDetail.isNextDisable();
    };

    $scope.objEntityDetail.CheckForExistingEntity = function () {
        $scope.objEntityDetail.IsExistingEntity = false;
        $scope.objEntityDetail.isBusObjectDisabled = false;
        if ($scope.objDirFunctions && $scope.objDirFunctions.resetExtraField) {
            $scope.objDirFunctions.resetExtraField($scope.objExtraFields);
        }
        if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != "") {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();

            if (entityIntellisenseList.some(function (x) { return x.ID.toLowerCase() == "ent" + $scope.objEntityDetail.EntityID.toLowerCase() && ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != ""); })) {

                $scope.objEntityDetail.IsExistingEntity = true;
            }
            var lst = entityIntellisenseList.filter(function (x) { return x.ID == "ent" + $scope.objEntityDetail.EntityID && x.BusinessObjectName == $scope.objEntityDetail.BusinessObjectName; });
            if (lst && lst.length > 0) {
                $scope.objEntityDetail.BusinessObjectName = lst[0].BusinessObjectName;
                if ($scope.objEntityDetail.BusinessObjectName) {
                    $scope.objEntityDetail.isBusObjectDisabled = true;
                }
            }
        }
    };

    $scope.onbusObjectChange = function () {
        if ($scope.lstBusinessObject.some(function (x) { return x.ID == $scope.objEntityDetail.BusinessObjectName; })) {
            $scope.IsOneToOneOrManyDisable = true;
        }
        else {
            $scope.IsOneToOneOrManyDisable = false;
        }
    };

    $scope.ClearFields = function () {
        $scope.objEntityDetail.EntityID = "";
        $scope.objEntityDetail.BusinessObjectName = "";
        $scope.objEntityDetail.FormattedClassName = "";
        $scope.objEntityDetail.dtDOTableColumns = [];
    };


    //#region Methods Update Data For Next Page
    $scope.UpdateOnNextFromEntityDetail = function () {
        var isNewTable = false;
        if (!$scope.objEntityDetail.PrevTableName || $scope.objEntityDetail.PrevTableName != $scope.objEntityDetail.TableName) {
            $scope.objEntityDetail.PrevTableName = $scope.objEntityDetail.TableName;
            isNewTable = true;
            $scope.isLoadOneToManyData = true;
        }

        if ($scope.objEntityDetail.TableName && isNewTable) {
            $scope.objEntityDetail.OneToManySelectedTableCollection = [];
            $scope.objEntityDetail.OneToOneSelectedTableCollection = [];

            if ($scope.IsOneToOneOrManyDisable) {
                $scope.objCodeValues.lstTableCodeValue = [];
                $scope.UpdateMainTableCodeValues();
            }

            $.connection.hubCreateNewObject.server.getDoTableColumnDetails($scope.objEntityDetail.TableName).done(function (DoTableColumnDetails) {
                $scope.$apply(function () {
                    $scope.objEntityDetail.dtDOTableColumns = DoTableColumnDetails;
                });
            });
            $scope.LoadDataForOneToOne();

            if ($scope.objEntityDetail.IsExistingEntity) {
                $.connection.hubCreateNewObject.server.getEntityExtraFieldsByName("ent" + $scope.objEntityDetail.EntityID).done(function (data) {
                    $scope.$evalAsync(function () {
                        if (data && data.Elements && data.Elements.length > 0) {
                            $scope.objExtraFields = data;
                            if ($scope.objDirFunctions && $scope.objDirFunctions.resetExtraField) {
                                $scope.objDirFunctions.resetExtraField($scope.objExtraFields);
                            }
                        }
                    });
                });
            }
        }

    };

    //#endregion
    //#endregion 

    //#region Common Method for Creating New Object 


    $scope.CloseWizardClick = function () {
        $scope.dialogForNew.close();
    };

    $scope.CreateEntity = function (tableName, formattedClassName, path, codeValues, isMainEntity, entityID, description, busObjectName, parentEntity, OneToOneSelectedTableCollection, OneToManySelectedTableCollection, ablnShowConsolidatedBORelatedProperties) {
        var obj = {
            TableName: tableName, FormattedClassName: formattedClassName, FolderPath: path, lstCodeValue: codeValues, IsMainEntity: isMainEntity, EntityName: "ent" + entityID,
            Description: description, BusinessObjectName: busObjectName,
            ParentEntity: parentEntity, UserId: $rootScope.UserID, lstOneToOne: $scope.objEntityDetail.OneToOneSelectedTableCollection, lstOneToMany: OneToManySelectedTableCollection,
            ExtraFields: $scope.objExtraFields, isXMLCreated: false,
        };
        if (obj != undefined) {
            if (!ablnShowConsolidatedBORelatedProperties) { ablnShowConsolidatedBORelatedProperties = false; }
            $.connection.hubCreateNewObject.server.createEntity(obj, ablnShowConsolidatedBORelatedProperties).done(function (lstdetails) {
                $rootScope.IsFilesAddedDelete = true;
                $scope.$apply(function () {
                    var data = lstdetails;
                    if (data.length > 0) {
                        $scope.lstEntities = data[0];
                        $scope.lstSummaryReport = data[1];
                        var intellisensedata = data[2];
                        if (intellisensedata.length > 0) {
                            for (var i = 0; i < intellisensedata.length; i++) {

                                var lst = $EntityIntellisenseFactory.getEntityIntellisense().filter(function (x) {
                                    return x.ID == intellisensedata[i].ID;
                                });
                                if (lst && lst.length > 0) {
                                    $EntityIntellisenseFactory.getEntityIntellisense().splice($EntityIntellisenseFactory.getEntityIntellisense().indexOf(lst[0]), 1);
                                }

                                var entityIntellisense = $EntityIntellisenseFactory.getEntityIntellisense().concat(intellisensedata[i]);
                                $EntityIntellisenseFactory.setEntityIntellisense(entityIntellisense);
                            }
                        }

                        if ($scope.objEntityDetail.TableName && $scope.objEntityDetail.TableName != "") {
                            var newScope = $scope.$new();
                            newScope.lstSummaryReport = $scope.lstSummaryReport;
                            newScope.beforeSummaryClose = function () {
                                if ($scope.lstEntities.length > 0) {
                                    $rootScope.openFile($scope.lstEntities[0], true);
                                }
                                $scope.dialogForNew.close();
                            };

                            newScope.openCSFileClick = function (obj) {
                                if (obj && (obj.FileContent && obj.FileContent != "")) {
                                    $scope.strCSFileContent = obj.FileContent;
                                    var newCSScope = newScope.$new();
                                    newCSScope.closeCsDialog = function () {
                                        newCSScope.csDialog.close();
                                    };
                                    newCSScope.csDialog = $rootScope.showDialog(newCSScope, "CS File Content", "CreateNewObject/views/Entity/OpenCSFile.html");
                                }
                            };

                            newScope.closeDialog = function () {
                                newScope.summaryDialog.close();
                            };
                            if (newScope.lstSummaryReport && newScope.lstSummaryReport.length > 0) {
                                newScope.summaryDialog = $rootScope.showDialog(newScope, "Summary Report", "CreateNewObject/views/Entity/FileSummaryReport.html", { beforeClose: newScope.beforeSummaryClose });
                            }
                            else if (newScope.lstSummaryReport && newScope.lstSummaryReport.length == 0) {
                                if ($scope.lstEntities.length > 0) {
                                    $rootScope.openFile($scope.lstEntities[0], true);
                                }

                                $scope.dialogForNew.close();
                            }
                        }
                        else {

                            if ($scope.lstEntities.length > 0) {
                                $rootScope.openFile($scope.lstEntities[0], true);

                            }

                            $scope.dialogForNew.close();
                        }
                    }
                });
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
            });
        }

    };

    $scope.CloseWizardClick = function () {
        $scope.dialogForNew.close();
    };

    $scope.checkAndExecuteLocationCallback = function (aobjCreationObject) {
        if (typeof $scope.objLocationCallback.locationCallback === "function") {
            $timeout(function () {
                if (aobjCreationObject && aobjCreationObject.errors && aobjCreationObject.errors.invalid_path) {
                    aobjCreationObject.errors.invalid_path = "";
                }
                $scope.objLocationCallback.locationCallback();
            });
        }
    }

    //#endregion

    $scope.objEntityDetail.onEntityNameChange = function () {
        $scope.objEntityDetail.CheckForExistingEntity();
    };
    $scope.CreatewithbusinessobjectChange = function (ablnIsCreatewithbusinessobject) {
        if (ablnIsCreatewithbusinessobject && ablnIsCreatewithbusinessobject == "True") {
            if ($scope.objEntityDetail && $scope.objEntityDetail.EntityID) {
                $scope.objEntityDetail.BusinessObjectName = "bus" + $scope.objEntityDetail.EntityID;
                if (!$scope.objEntityDetail.TableName) {
                    $scope.onbusObjectChange();
                    $scope.objEntityDetail.isNextDisable();
                }
            }
            $scope.blnShowConsolidatedBORelatedProperties = true;
        }
        else {
            $scope.objEntityDetail.BusinessObjectName = "";
            $scope.blnShowConsolidatedBORelatedProperties = false;
        }
    };
    $scope.objEntityDetail.isNextDisable = function () {
        $scope.objEntityDetail.isValidEntity = true;
        if (!$scope.ValidateEntityDetails()) {
            $scope.objEntityDetail.isNextDisableForEntity = true;
            $scope.objEntityDetail.isValidEntity = false;
            return true;
        }
        if ($scope.objEntityDetail.TableName) {
            if ($scope.showExtraFieldsTab) {
                $scope.IsOneToOneOrManyDisable = false;
                $scope.objEntityDetail.isNextDisableForEntity = false;
                return false;
            } else {
                if ($scope.blnShowConsolidatedBORelatedProperties && !$scope.objEntityDetail.BusinessObjectName) {
                    $scope.IsOneToOneOrManyDisable = false;
                    $scope.objEntityDetail.isNextDisableForEntity = true;
                }
                else {
                    $scope.IsOneToOneOrManyDisable = false;
                    $scope.objEntityDetail.isNextDisableForEntity = false;
                }
                return true;
            }
        }
        if (!$scope.objEntityDetail.TableName || $scope.objEntityDetail.TableName == '') {
            if ($scope.showExtraFieldsTab) {
                $scope.IsOneToOneOrManyDisable = true;
                $scope.objEntityDetail.isNextDisableForEntity = false;
                return false;
            } else {
                $scope.IsOneToOneOrManyDisable = false;
                $scope.objEntityDetail.isNextDisableForEntity = true;
                return true;
            }
        }
        if ($scope.objEntityDetail.IsExistingEntity) {
            if (!$scope.showExtraFieldsTab && $scope.IsCodeValueDisable) {
                $scope.objEntityDetail.isNextDisableForEntity = true;
                return false;
            }
        }
        $scope.objEntityDetail.isNextDisableForEntity = false;
        return false;
    };

    $scope.objEntityDetail.isFinishDisable = function () {
        if ($scope.objEntityDetail.TableName && $scope.objEntityDetail.TableName != '') {
            return true;
        }

        if (!$scope.ValidateEntityDetails()) {
            return true;
        }
        if ($scope.showExtraFieldsTab) {
            return true;
        }
        return false;
    };

    //#region Methods for One To one 

    $scope.ResetOneToOneData = function () {
        $scope.objOneToOne.ParentTableCollection = [];
    };

    $scope.LoadDataForOneToOne = function () {
        $scope.ResetOneToOneData();


        if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != " ") {
            $rootScope.IsLoading = true;
            $.connection.hubCreateNewObject.server.getParentTables($scope.objEntityDetail.TableName).done(function (parentTableCollection) {
                $scope.$apply(function () {
                    $scope.objOneToOne.ParentTableCollection = parentTableCollection;
                    $.connection.hubCreateNewObject.server.getDoTableColumnDetails($scope.objEntityDetail.TableName).done(function (DoTableColumnDetails) {
                        $scope.$apply(function () {
                            $scope.objEntityDetail.dtDOTableColumns = DoTableColumnDetails;
                            $rootScope.IsLoading = false;
                        });
                    });
                });
            });
        }
    };



    $scope.SelectParentTableClick = function (obj) {
        $scope.objOneToOne.SelectedParentTable = obj;
    };

    $scope.OnAddParentTable = function () {
        function iteratorForeigenKey(row) {
            if (row.key_no == "1" && row.column_name) {
                $scope.objcol.ForeigenKey = row.column_name.toLowerCase();
                return;
            }
        }

        function iteratorOneToOne(item) {
            if ($scope.objOneToOne.SelectedParentTable.TableName == item.TableName) {
                blnFound = true;
                return;
            }
        }

        function iAddForeigenKeyCollection(row) {
            let lstrProp = "column_name";
            if (!row[lstrProp]) {
                lstrProp = "COLUMN_NAME";
            }
            if (row[lstrProp]) {
                $scope.objcol.ForeigenKeyCollection.push(row[lstrProp].toLowerCase());
            }
        }
        if ($scope.objOneToOne.SelectedParentTable != undefined) {
            var blnFound = false;



            angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iteratorOneToOne);

            if (!blnFound) {
                $scope.objcol = {
                    ForeigenKey: '', TableName: $scope.objOneToOne.SelectedParentTable.TableName, FormattedClassName: $scope.objOneToOne.SelectedParentTable.FormattedClassName, EntityName: "ent" + $scope.objOneToOne.SelectedParentTable.FormattedClassName, ObjectName: $scope.objOneToOne.SelectedParentTable.ObjectName, BusinessObjectName: "bus" + $scope.objOneToOne.SelectedParentTable.FormattedClassName, BaseBusinessObjectName: "busBase"
                };
                $scope.objcol.ForeigenKeyCollection = [];
                $scope.objcol.SortOrderCollection = [];
                $scope.objcol.DefaultSortOrderLst = ["asc", "desc"];
                $scope.objcol.lstBusinessObjects = [];
                $scope.objcol.BusinessObjects = [];
                $scope.objcol.lstCodeValues = [];
                if ($scope.CheckEntityIsValidOrNot($scope.objcol.EntityName)) {
                    $scope.objcol.IsExistingEntity = true;
                    $rootScope.IsLoading = true;
                    $.connection.hubCreateNewObject.server.getChildEntityFolderPath("ent" + $scope.objOneToOne.SelectedParentTable.FormattedClassName).done(function (astrFolderName) {
                        $scope.$evalAsync(function () {
                            if (astrFolderName) {
                                $scope.objcol.FolderName = astrFolderName;
                            }
                            else {
                                $scope.objcol.FolderName = $scope.objEntityDetail.Path;
                            }
                            $rootScope.IsLoading = false;
                        });
                    });
                }
                else {// if entity is not already created , then we will bring code id value pair to create that entity
                    $scope.objcol.FolderName = $scope.objEntityDetail.Path;

                    $rootScope.IsLoading = true;
                    $.connection.hubCreateNewObject.server.updatedCodeValuesForMainTable($scope.objOneToOne.SelectedParentTable.TableName).done(function (lstcodevalues) {
                        $scope.$evalAsync(function () {
                            $scope.objcol.lstTableCodeValue = lstcodevalues;
                            if ($scope.objcol.lstTableCodeValue && $scope.objcol.lstTableCodeValue.length > 0) {
                                $scope.IsCodeValueDisable = false;
                            }
                            $rootScope.IsLoading = false;
                        });
                    });
                }


                $scope.objcol.dtDOColumnDetail = undefined;
                $scope.objcol.dtCDOXmlColumns = undefined;
                $scope.objcol.isFromTable = true;
                if ($scope.objEntityDetail.dtDOTableColumns != undefined) {


                    angular.forEach($scope.objEntityDetail.dtDOTableColumns, iAddForeigenKeyCollection);

                }

                $rootScope.IsLoading = true;
                $.connection.hubCreateNewObject.server.getDoTableColumnDetails($scope.objcol.TableName).done(function (data) {
                    $scope.$apply(function () {
                        $scope.objcol.dtDOColumnDetail = data;
                        if ($scope.objcol.dtDOColumnDetail != undefined) {


                            angular.forEach($scope.objcol.dtDOColumnDetail, iteratorForeigenKey);

                        }
                        if ($scope.objEntityDetail.OneToOneSelectedTableCollection.indexOf($scope.objcol) <= -1) {
                            $scope.objEntityDetail.OneToOneSelectedTableCollection.push($scope.objcol);
                        }
                        $rootScope.IsLoading = false;
                    });
                });
            }
        }
    };

    $scope.SelectOneToOneParentTable = function (obj, event) {
        $scope.objOneToOne.SelectedOneToOneTable = obj;
        var tr = event.currentTarget;
        if (tr.localName != 'tr') {
            tr = $(tr).parents('tr');
            tr.addClass('selected-row');
            tr.siblings('tr').removeClass('selected-row');
        }
        else {
            $(tr).addClass('selected-row');
            $(tr).siblings('tr').removeClass('selected-row');
        }
    };

    $scope.DeleteParentTableRowCommand = function () {
        if ($scope.objOneToOne.SelectedOneToOneTable != undefined) {
            var index = $scope.objEntityDetail.OneToOneSelectedTableCollection.indexOf($scope.objOneToOne.SelectedOneToOneTable);
            if (index >= 0) {
                $scope.objEntityDetail.OneToOneSelectedTableCollection.splice(index, 1);
                if (index < $scope.objEntityDetail.OneToOneSelectedTableCollection.length) {
                    $scope.objOneToOne.SelectedOneToOneTable = $scope.objEntityDetail.OneToOneSelectedTableCollection[index];
                }
                else if ($scope.objEntityDetail.OneToOneSelectedTableCollection.length > 0) {
                    $scope.objOneToOne.SelectedOneToOneTable = $scope.objEntityDetail.OneToOneSelectedTableCollection[index - 1];
                }
            }
            if ($scope.objEntityDetail.OneToOneSelectedTableCollection) {
                var count = 0;
                for (var i = 0; i < $scope.objEntityDetail.OneToOneSelectedTableCollection.length; i++) {
                    if ($scope.objEntityDetail.OneToOneSelectedTableCollection[i].lstTableCodeValue) {
                        count += $scope.objEntityDetail.OneToOneSelectedTableCollection[i].lstTableCodeValue.length;
                    }
                }

                if (count > 0 || ($scope.objEntityDetail.lstTableCodeValue && $scope.objEntityDetail.lstTableCodeValue.length > 0)) {
                    $scope.IsCodeValueDisable = false;
                }
                else {
                    $scope.IsCodeValueDisable = true;
                }
            }
        }

    };

    $scope.UpdateOnNextFromOneToOne = function () {
        if ($scope.objEntityDetail.TableName && $scope.isLoadOneToManyData) {
            $scope.LoadDataForOneToMany();
        }
        //if ($scope.IsCodeValueDisable && !$scope.showExtraFieldsTab && !$scope.blnShowConsolidatedBORelatedProperties) {
        //    $scope.objEntityDetail.isNextDisableForEntity = false;
        //}
        //else {
        //    $scope.objEntityDetail.isNextDisableForEntity = true;
        //}
    };


    $scope.SetClassForOneToOne = function (object, step) {
        if (object == step) {
            return "selected";
        }
    };

    //#endregion

    //#region Methods for One To Many 

    $scope.ResetOneToManyData = function () {
        $scope.objOneToMany.ChildTableCollection = [];
    };

    $scope.LoadDataForOneToMany = function () {
        $scope.ResetOneToManyData();
        $scope.isLoadOneToManyData = false;
        if ($scope.objEntityDetail.TableName != undefined && $scope.objEntityDetail.TableName != " ") {
            $rootScope.IsLoading = true;
            $.connection.hubCreateNewObject.server.getChildTableDetails($scope.objEntityDetail.TableName).done(function (data) {
                $scope.$apply(function () {
                    $rootScope.IsLoading = false;
                    $scope.objOneToMany.ChildTableCollection = data;
                });
            }
            );
        }
    };


    $scope.SelectChildTableClick = function (obj) {
        $scope.objOneToMany.SelectedChildTable = obj;
    };

    $scope.OnAddChildTable = function () {
        function iterator(item) {
            if ($scope.objOneToMany.SelectedChildTable.TableName == item.TableName) {
                blnFound = true;
                return;
            }
        }
        if ($scope.objOneToMany.SelectedChildTable != undefined && !$rootScope.IsLoading) {
            var blnFound = false;
            angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iterator);
            if (!blnFound) {
                $scope.objcol = {
                    ForeigenKey: '', TableName: $scope.objOneToMany.SelectedChildTable.TableName, FormattedClassName: $scope.objOneToMany.SelectedChildTable.FormattedClassName, EntityName: "ent" + $scope.objOneToMany.SelectedChildTable.FormattedClassName, ObjectName: $scope.objOneToMany.SelectedChildTable.ObjectName, BusinessObjectName: "bus" + $scope.objOneToMany.SelectedChildTable.FormattedClassName, BaseBusinessObjectName: "busBase"
                };
                $scope.objcol.ForeigenKeyCollection = [];
                $scope.objcol.SortOrderCollection = [];
                $scope.objcol.DefaultSortOrderLst = ["asc", "desc"];
                $scope.objcol.lstBusinessObjects = [];
                $scope.objcol.BusinessObjects = [];
                $scope.objcol.lstCodeValues = [];// $scope.objOneToMany.SelectedChildTable.lstCodeValues;
                if ($scope.CheckEntityIsValidOrNot($scope.objcol.EntityName)) {
                    $scope.objcol.IsExistingEntity = true;
                    $rootScope.IsLoading = true;
                    $.connection.hubCreateNewObject.server.getChildEntityFolderPath("ent" + $scope.objOneToMany.SelectedChildTable.FormattedClassName).done(function (astrFolderName) {
                        $scope.$evalAsync(function () {
                            if (astrFolderName) {
                                $scope.objcol.FolderName = astrFolderName;
                            }
                            else {
                                $scope.objcol.FolderName = $scope.objEntityDetail.Path;
                            }
                            $rootScope.IsLoading = false;
                        });
                    });
                }
                else { // if entity is not already created , then we will bring code id value pair to create that entity
                    $scope.objcol.FolderName = $scope.objEntityDetail.Path;
                    $rootScope.IsLoading = true;
                    $.connection.hubCreateNewObject.server.updatedCodeValuesForMainTable($scope.objOneToMany.SelectedChildTable.TableName).done(function (lstcodevalues) {
                        $scope.$evalAsync(function () {
                            $scope.objcol.lstTableCodeValue = lstcodevalues;
                            if ($scope.objcol.lstTableCodeValue && $scope.objcol.lstTableCodeValue.length > 0) {
                                $scope.IsCodeValueDisable = false;
                            }


                            if ($scope.IsCodeValueDisable && !$scope.showExtraFieldsTab && !$scope.blnShowConsolidatedBORelatedProperties) {
                                $scope.objEntityDetail.isNextDisableForEntity = true;
                            }
                            else {
                                $scope.objEntityDetail.isNextDisableForEntity = false;
                            }
                            $rootScope.IsLoading = false;
                        });
                    });

                }
                if ($scope.IsCodeValueDisable && !$scope.showExtraFieldsTab && !$scope.blnShowConsolidatedBORelatedProperties) {
                    $scope.objEntityDetail.isNextDisableForEntity = true;
                }
                $scope.objcol.dtDOColumnDetail = undefined;
                $scope.objcol.dtCDOXmlColumns = undefined;
                $scope.objcol.isFromTable = true;
                $rootScope.IsLoading = true;

                $.connection.hubCreateNewObject.server.getDoTableColumnDetails($scope.objcol.TableName).done(function (data) {
                    $scope.receiveDoTableColumnDetailsForChildTable(data);
                });
            }
        }
    };

    $scope.receiveDoTableColumnDetailsForChildTable = function (data) {
        function iteratorForeigenKey(row) {
            if (row.key_no + "" == "1" && row.column_name) {
                $scope.objcol.ForeigenKey = row.column_name.toLowerCase();
                return;
            }
        }
        function iteratorSortOrder(row) {
            let lstrProp = "column_name";
            if (!row[lstrProp]) {
                lstrProp = "COLUMN_NAME";
            }
            if (row[lstrProp]) {
                $scope.objcol.SortOrderCollection.push(row[lstrProp].toLowerCase());
                $scope.objcol.ForeigenKeyCollection.push(row[lstrProp].toLowerCase());
                if (row.key_no + "" == "1") {
                    $scope.objcol.SortOrder = row[lstrProp].toLowerCase();
                }
            }
        }
        $scope.$apply(function () {
            $scope.objcol.dtDOColumnDetail = data;
            if ($scope.objcol.dtDOColumnDetail != undefined) {
                angular.forEach($scope.objcol.dtDOColumnDetail, iteratorSortOrder);
            }
            $scope.objcol.DefaultSortOrder = "asc";
            if ($scope.objEntityDetail.dtDOTableColumns != undefined) {
                angular.forEach($scope.objEntityDetail.dtDOTableColumns, iteratorForeigenKey);
            }
            $scope.objEntityDetail.OneToManySelectedTableCollection.push($scope.objcol);
            $rootScope.IsLoading = false;
        });
    };

    $scope.SelectOneToManyChildTable = function (obj, event) {
        $scope.objOneToMany.SelectedOneToManyTable = obj;
        var tr = event.currentTarget;
        if (tr.localName != 'tr') {
            tr = $(tr).parents('tr');
            tr.addClass('selected-row');
            tr.siblings('tr').removeClass('selected-row');
        }
        else {
            $(tr).addClass('selected-row');
            $(tr).siblings('tr').removeClass('selected-row');
        }
    };

    $scope.DeleteChildTableRowClick = function () {
        if ($scope.objOneToMany.SelectedOneToManyTable != undefined) {
            var index = $scope.objEntityDetail.OneToManySelectedTableCollection.indexOf($scope.objOneToMany.SelectedOneToManyTable);
            if (index >= 0) {
                $scope.objEntityDetail.OneToManySelectedTableCollection.splice(index, 1);
                if (index < $scope.objEntityDetail.OneToManySelectedTableCollection.length) {
                    $scope.objOneToMany.SelectedOneToManyTable = $scope.objEntityDetail.OneToManySelectedTableCollection[index];
                }
                else if ($scope.objEntityDetail.OneToManySelectedTableCollection.length > 0) {
                    $scope.objOneToMany.SelectedOneToManyTable = $scope.objEntityDetail.OneToManySelectedTableCollection[index - 1];
                }
            }
            if ($scope.objEntityDetail.OneToManySelectedTableCollection) {
                var count = 0;
                for (var i = 0; i < $scope.objEntityDetail.OneToManySelectedTableCollection.length; i++) {
                    if ($scope.objEntityDetail.OneToManySelectedTableCollection[i].lstTableCodeValue) {
                        count += $scope.objEntityDetail.OneToManySelectedTableCollection[i].lstTableCodeValue.length;
                    }
                }

                if (count > 0 || ($scope.objEntityDetail.lstTableCodeValue && $scope.objEntityDetail.lstTableCodeValue.length > 0)) {
                    $scope.IsCodeValueDisable = false;
                }
                else {
                    $scope.IsCodeValueDisable = true;
                }
            }


        }
    };



    $scope.UpdateOnNextFromOneToMany = function () {
        $scope.PopulateCodeValues();
        $scope.objRelatedProperties.isEntityFinishDisable = false;
        $scope.objRelatedProperties.isFinishDisable();
        //if (!$scope.showExtraFieldsTab && !$scope.blnShowConsolidatedBORelatedProperties) {
        //    $scope.objEntityDetail.isNextDisableForEntity = true;
        //}
        //else {
        //    $scope.objEntityDetail.isNextDisableForEntity = false;
        //}
    };

    $scope.PopulateCodeValues = function () {
        $scope.objCodeValues.lstTableCodeValue = [];
        $scope.UpdateMainTableCodeValues();
        $scope.UpdateCodeValues($scope.objEntityDetail.OneToOneSelectedTableCollection);
        $scope.UpdateCodeValues($scope.objEntityDetail.OneToManySelectedTableCollection);

        $scope.$evalAsync(function () {
            if ($scope.objCodeValues.lstTableCodeValue.length > 0) {
                $scope.IsCodeValueDisable = false;
            }
            else {
                $scope.IsCodeValueDisable = true;
            }
        });

    };

    $scope.UpdateMainTableCodeValues = function () {

        function getCodeValue(objCodeValue) {
            $scope.objCodeValues.lstTableCodeValue.push(objCodeValue);
        }
        angular.forEach($scope.objEntityDetail.lstTableCodeValue, getCodeValue);

    };
    $scope.UpdateCodeValues = function (lst) {
        function getCodeValues(codeValue) {
            $scope.objCodeValues.lstTableCodeValue.push(codeValue);
        }
        function iterator(table) {
            angular.forEach(table.lstTableCodeValue, getCodeValues);
        }

        angular.forEach(lst, iterator);
    };

    $scope.SetClassForOneToMany = function (object, step) {
        if (object == step) {
            return "selected";
        }
    };

    //#endregion

    //#region Methods for Code Values

    $scope.resetLstCodeValues = function () {
        $scope.objEntityDetail.lstCodeValues = [];
        function iterator(table) {
            if (table.lstCodeValues) {
                table.lstCodeValues = [];
            }
        }
        angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iterator);
        angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iterator);
    };
    $scope.UpdateOnNextFromCodeValueClick = function () {
        $scope.resetLstCodeValues();

        angular.forEach($scope.objCodeValues.lstTableCodeValue, function (obj) {
            var objParent;
            function iteratorOneToOne(item) {
                if (item.TableName.toLowerCase() == obj.TableName.toLowerCase()) {
                    objParent = item;
                    return;
                }
            }
            function iteratorOneToMany(item) {
                if (item.TableName.toLowerCase() == obj.TableName.toLowerCase()) {
                    objParent = item;
                    return;
                }
            }
            if (obj.TableName.toLowerCase() == $scope.objEntityDetail.TableName.toLowerCase()) {
                $scope.objEntityDetail.lstCodeValues.push({ FieldName: obj.Description, CodeValue: obj.Code });
            }
            else {

                angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iteratorOneToOne);

                if (objParent != undefined) {
                    objParent.lstCodeValues.push({ FieldName: obj.Description, CodeValue: obj.Code });
                }
                else {
                    angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iteratorOneToMany);


                    if (objParent != undefined) {
                        objParent.lstCodeValues.push({ FieldName: obj.Description, CodeValue: obj.Code });
                    }
                }
            }
        });
    };

    //#endregion

    //#region Methods for Related Properties (End Page)

    $scope.OnAddNonCollectionRowClick = function () {
        var objcol = {
            ForeigenKey: '', TableName: "", FormattedClassName: "", EntityName: "", ObjectName: "", BusinessObjectName: "", BaseBusinessObjectName: "busBase", FolderName: $scope.objEntityDetail.Path
        };
        objcol.ForeigenKeyCollection = [];
        objcol.SortOrderCollection = [];
        objcol.DefaultSortOrderLst = ["asc", "desc"];
        objcol.lstBusinessObjects = [];
        objcol.BusinessObjects = [];
        objcol.lstCodeValues = [];
        objcol.dtDOColumnDetail = undefined;
        objcol.dtCDOXmlColumns = undefined;
        objcol.isFromTable = false;
        function iterator(item) {
            if (item.IsSelected) {
                item.blnDetailsGridOpen = false;
            }
            item.IsSelected = false;
        }
        angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iterator);
        objcol.IsSelected = true;
        objcol.blnDetailsGridOpen = true;
        function iAddForeigenKeyCollection(row) {
            let lstrProp = "column_name";
            if (!row[lstrProp]) {
                lstrProp = "COLUMN_NAME";
            }
            objcol.ForeigenKeyCollection.push(row[lstrProp].toLowerCase());
        }
        if ($scope.objEntityDetail.dtDOTableColumns != undefined) {

            angular.forEach($scope.objEntityDetail.dtDOTableColumns, iAddForeigenKeyCollection);

        }

        $scope.objEntityDetail.OneToOneSelectedTableCollection.push(objcol);
        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.canDeleteOneToOne = function () {
        if ($scope.objEntityDetail.OneToOneSelectedTableCollection) {
            return $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return x.IsSelected; });
        }
        else {
            return false;
        }
    };

    $scope.OnDeleteNonCollectionRowClick = function () {
        function iterator(item) {
            index = $scope.objEntityDetail.OneToOneSelectedTableCollection.indexOf(item);
            $scope.objEntityDetail.OneToOneSelectedTableCollection.splice($scope.objEntityDetail.OneToOneSelectedTableCollection.indexOf(item), 1);
        }

        var index = 0;
        var selectedItems = $scope.objEntityDetail.OneToOneSelectedTableCollection.filter(function (x) { return x.IsSelected; });
        if (selectedItems.length) {
            angular.forEach(selectedItems, iterator);
            if (index < $scope.objEntityDetail.OneToOneSelectedTableCollection.length) {
                $scope.objEntityDetail.OneToOneSelectedTableCollection[index].IsSelected = true;
            }
            else if ($scope.objEntityDetail.OneToOneSelectedTableCollection.length > 0) {
                $scope.objEntityDetail.OneToOneSelectedTableCollection[index - 1].IsSelected = true;
            }
        }
        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.OnSelectNonCollectionRow = function (oneToOne) {
        function iterator(item) {
            item.IsSelected = false;
        }
        angular.forEach($scope.objEntityDetail.OneToOneSelectedTableCollection, iterator);
        oneToOne.IsSelected = true;

    };

    $scope.OnOpenOneToOneDetailClick = function (oneToOne) {
        oneToOne.blnDetailsGridOpen = true;
    };

    $scope.OnCloseOneToOneDetailClick = function (oneToOne) {
        oneToOne.blnDetailsGridOpen = false;
        oneToOne.FormattedClassName = oneToOne.Attribute;
    };

    $scope.OnAddCollectionRowClick = function () {
        var objcol = {
            ForeigenKey: '', TableName: "", FormattedClassName: "", EntityName: "", ObjectName: "", BusinessObjectName: "", BaseBusinessObjectName: "busBase", FolderName: $scope.objEntityDetail.Path
        };
        objcol.ForeigenKeyCollection = [];
        objcol.SortOrderCollection = [];
        objcol.DefaultSortOrderLst = ["asc", "desc"];
        objcol.lstBusinessObjects = [];
        objcol.BusinessObjects = [];
        objcol.lstCodeValues = [];
        objcol.dtDOColumnDetail = undefined;
        objcol.dtCDOXmlColumns = undefined;
        objcol.isFromTable = false;
        function iterator(item) {
            if (item.IsSelected) {
                item.blnDetailsGridOpen = false;
            }
            item.IsSelected = false;
        }
        angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iterator); objcol.blnDetailsGridOpen = true;
        objcol.IsSelected = true;
        $scope.objEntityDetail.OneToManySelectedTableCollection.push(objcol);
        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.canDeleteOneToMany = function () {
        if ($scope.objEntityDetail.OneToManySelectedTableCollection) {
            return $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return x.IsSelected; });
        }
        else {
            return false;
        }
    };

    $scope.OnDeleteCollectionRowClick = function () {
        function iterator(item) {
            index = $scope.objEntityDetail.OneToManySelectedTableCollection.indexOf(item);
            $scope.objEntityDetail.OneToManySelectedTableCollection.splice($scope.objEntityDetail.OneToManySelectedTableCollection.indexOf(item), 1);
        }

        var index = 0;
        var selectedItems = $scope.objEntityDetail.OneToManySelectedTableCollection.filter(function (x) { return x.IsSelected; });
        if (selectedItems.length) {
            angular.forEach(selectedItems, iterator);
            if (index < $scope.objEntityDetail.OneToManySelectedTableCollection.length) {
                $scope.objEntityDetail.OneToManySelectedTableCollection[index].IsSelected = true;
            }
            else if ($scope.objEntityDetail.OneToManySelectedTableCollection.length > 0) {
                $scope.objEntityDetail.OneToManySelectedTableCollection[index - 1].IsSelected = true;
            }
        }
        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.OnSelectCollectionRow = function (oneToMany) {
        function iterator(item) {
            item.IsSelected = false;
        }
        angular.forEach($scope.objEntityDetail.OneToManySelectedTableCollection, iterator);

        oneToMany.IsSelected = true;

    };

    $scope.OnOpenOneToManyDetailClick = function (oneToMany) {
        oneToMany.blnDetailsGridOpen = true;
    };

    $scope.OnCloseOneToManyDetailClick = function (oneToMany) {
        oneToMany.blnDetailsGridOpen = false;
        oneToMany.FormattedClassName = oneToMany.Attribute;
    };

    $scope.onOneToOneBusObjChange = function () {
        var selectedItems = $scope.objEntityDetail.OneToOneSelectedTableCollection.filter(function (x) { return x.IsSelected; });
        if (selectedItems && selectedItems.length > 0) {
            var objTable = $scope.lstMainTable.filter(function (x) { return x.BusinessObjectName == selectedItems[0].BusinessObjectName; });
            if (objTable.length && objTable.length > 0) {
                selectedItems[0].ObjectName = objTable[0].FormattedClassName;
                selectedItems[0].TableName = objTable[0].ID;
                selectedItems[0].EntityName = "ent" + objTable[0].FormattedClassName;
                selectedItems[0].FormattedClassName = objTable[0].FormattedClassName;
            }
        }
        $scope.objRelatedProperties.isFinishDisable();
    };



    $scope.onOneToManyBusObjChange = function () {
        var selectedItems = $scope.objEntityDetail.OneToManySelectedTableCollection.filter(function (x) { return x.IsSelected; });
        if (selectedItems && selectedItems.length > 0) {
            var objTable = $scope.lstMainTable.filter(function (x) { return x.BusinessObjectName == selectedItems[0].BusinessObjectName; });
            if (objTable.length && objTable.length > 0) {
                selectedItems[0].ObjectName = objTable[0].FormattedClassName;
                selectedItems[0].TableName = objTable[0].ID;
                selectedItems[0].EntityName = "ent" + objTable[0].FormattedClassName;
                selectedItems[0].FormattedClassName = objTable[0].FormattedClassName;
                selectedItems[0].SortOrderCollection = [];
                selectedItems[0].SortOrderCollection.push("");
                selectedItems[0].ForeigenKeyCollection = [];
                selectedItems[0].ForeigenKeyCollection.push("");
                selectedItems[0].SortOrder = "";
                selectedItems[0].ForeigenKey = "";

                $.connection.hubCreateNewObject.server.getDoTableColumnDetails(selectedItems[0].TableName).done(function (data) {
                    $scope.receiveDoTableColumnDetailsForChildTableWhileEditing(data);
                });
            }
        }
        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.receiveDoTableColumnDetailsForChildTableWhileEditing = function (data) {


        $scope.$apply(function () {
            function iterator(row) {
                let lstrProp = "column_name";
                if (!row[lstrProp]) {
                    lstrProp = "COLUMN_NAME";
                }
                selectedItems[0].SortOrderCollection.push(row[lstrProp].toLowerCase());
                selectedItems[0].ForeigenKeyCollection.push(row[lstrProp].toLowerCase());
                if (row.key_no + "" == "1") {
                    selectedItems[0].SortOrder = row[lstrProp].toLowerCase();
                }
            }
            var selectedItems = $scope.objEntityDetail.OneToManySelectedTableCollection.filter(function (x) { return x.IsSelected; });
            if (selectedItems && selectedItems.length > 0) {
                selectedItems[0].dtDOColumnDetail = data;
                angular.forEach(selectedItems[0].dtDOColumnDetail, iterator);



                selectedItems[0].DefaultSortOrder = "asc";
            }
        });
    };

    $scope.objRelatedProperties.validateOneToOne = function () {
        var isValid = true;
        $scope.objRelatedProperties.ErrorMessageForDisplay = "";
        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (!x.isFromTable && (!x.Attribute || x.Attribute.trim() == "")); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Attribute Name for one of the One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (!x.ObjectName || x.ObjectName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Object Name for one of the One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (!x.FolderName || x.FolderName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Folder for one of the One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (!x.isFromTable && (!x.EntityName || (x.EntityName.trim() != "" && !$scope.CheckEntityIsValidOrNot(x.EntityName.trim())))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Invalid Entity Name for one of the One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (x.isFromTable && !x.IsExistingEntity && (!x.EntityName || (x.EntityName.trim() != "" && !x.EntityName.startsWith("ent")))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Entity Name should start with 'ent' for One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (x.isFromTable && !x.IsExistingEntity && (!x.EntityName || (x.EntityName.trim() != "" && $scope.CheckEntityIsValidOrNot(x.EntityName.trim())))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Entity is already associated with busisness object/table for one of the One To One objects.";
        }

        if ($scope.objEntityDetail.OneToOneSelectedTableCollection && $scope.objEntityDetail.OneToOneSelectedTableCollection.some(function (x) { return (!x.EntityName || x.EntityName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Entity Name for one of the One To One objects.";
        }

        return isValid;
    };

    $scope.CheckEntityIsValidOrNot = function (entityId) {
        var isValid = false;
        var entityIntellisense = $EntityIntellisenseFactory.getEntityIntellisense();
        var entity = entityIntellisense.filter(function (x) { return x.ID && x.ID == entityId; });
        if (entity && entity.length > 0) {
            isValid = true;
        }
        return isValid;
    };
    $scope.objRelatedProperties.validateOneToMany = function () {
        var isValid = true;
        $scope.objRelatedProperties.ErrorMessageForDisplay = "";
        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (!x.isFromTable && (!x.Attribute || x.Attribute.trim() == "")); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Attribute Name for one of the One To Many objects.";
        }

        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (!x.ObjectName || x.ObjectName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Object Name for one of the One To Many objects.";
        }

        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (!x.FolderName || x.FolderName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Folder for one of the One To Many objects.";
        }

        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (!x.isFromTable && (!x.EntityName || (x.EntityName.trim() != "" && !$scope.CheckEntityIsValidOrNot(x.EntityName.trim())))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Invalid Entity Name for one of the One To Many objects.";
        }


        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (x.isFromTable && !x.IsExistingEntity && (!x.EntityName || (x.EntityName.trim() != "" && !x.EntityName.startsWith("ent")))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Entity Name should start with 'ent' for One To Many objects.";
        }

        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (x.isFromTable && !x.IsExistingEntity && (!x.EntityName || (x.EntityName.trim() != "" && $scope.CheckEntityIsValidOrNot(x.EntityName.trim())))); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Entity is already associated with busisness object/table for one of the One To Many objects.";
        }

        if ($scope.objEntityDetail.OneToManySelectedTableCollection && $scope.objEntityDetail.OneToManySelectedTableCollection.some(function (x) { return (!x.EntityName || x.EntityName.trim() == ""); })) {
            isValid = false;
            $scope.objRelatedProperties.ErrorMessageForDisplay = "Enter the Entity Name for one of the One To Many objects.";
        }

        return isValid;
    };

    $scope.objRelatedProperties.OnEntityFieldBlur = function (obj) {
        if (!obj.isFromTable) {
            var entityId = obj.EntityName;
            var entityIntellisense = $EntityIntellisenseFactory.getEntityIntellisense();
            var entity = entityIntellisense.filter(function (x) { return x.ID == entityId; });
            if (entity && entity.length > 0) {
                obj.BusinessObjectName = entity[0].BusinessObjectName;
            }
            obj.Attribute = entityId.substring(3);
            obj.ObjectName = entityId.substring(3);
        }

        $scope.objRelatedProperties.isFinishDisable();
    };

    $scope.objRelatedProperties.isFinishDisable = function () {
        var retValue = true;
        if ($scope.objRelatedProperties.validateOneToOne() && $scope.objRelatedProperties.validateOneToMany()) {
            retValue = false;
        }
        $scope.objRelatedProperties.isEntityFinishDisable = retValue;
        return retValue;
    };

    //#endregion

    //#region to check entTable Schema is Present or not

    $scope.validateEntTableSchema = function () {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var entTable = [];
        if (entityIntellisenseList && angular.isArray(entityIntellisenseList)) {
            entTable = entityIntellisenseList.filter(function (x) { return x.EntityName == 'entTableSchema'; });
        }
        if (entTable.length > 0) {
            var objEntity = entTable[0];
            var errorMesz = "";
            var alertMesz = "";
            var errMesz = "entTableSchema does not contain Query-";

            if (!objEntity.Queries.some(function (x) { return x.ID == 'GetTableList'; })) {
                errorMesz += " GetTableList ,";
            }
            if (!objEntity.Queries.some(function (x) { return x.ID == 'GetColumnDetailCDO'; })) {
                errorMesz += " GetColumnDetailCDO ,";
            }
            if (!objEntity.Queries.some(function (x) { return x.ID == 'GetPrimaryKeyDetails'; })) {
                errorMesz += " GetPrimaryKeyDetails ,";
            }
            if (!objEntity.Queries.some(function (x) { return x.ID == 'GetChildrenTables'; })) {
                errorMesz += " GetChildrenTables";
            }

            if (errorMesz && errorMesz.length > 0) {
                errorMesz = errorMesz.substring(0, errorMesz.length - 1);
                alertMesz = errMesz + errorMesz + ".";
                alert(alertMesz);
            }

        }
    };
    //#endregion

    //#endregion

    //#region Create Correspondence Methods 

    $scope.objCorrespondenceDetail.CanShowCorrespondenceSteps = function () {
        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "Correspondence") {
            retVal = false;
        }
        return retVal;
    };

    $scope.objCorrespondenceDetail.InitCorrespondence = function () {
        $scope.objCorrespondenceDetail.CorTemplateCollection = [];
        $scope.objCorrespondenceDetail.MasterTemplateCollection = [];
        $scope.objCorrespondenceDetail.ConditionalRuleCollection = [];
        $scope.objCorrespondenceDetail.AssociatedFormCollection = [];
        $scope.objCorrespondenceDetail.SfwTemplate = "";
        $scope.objCorrespondenceDetail.SfwEntity = "";
        $scope.objCorrespondenceDetail.Path = "";
        $scope.objCorrespondenceDetail.SfwResource = "";
        $scope.objCorrespondenceDetail.SfwMasterTemplate = "";
        $scope.objCorrespondenceDetail.SfwConditionalRule = "";
        $scope.objCorrespondenceDetail.IsUpdateMasterTemplateEnable = false;
        $scope.objCorrespondenceDetail.PopulateCorrespondenceTemplate();
        $scope.objCorrespondenceDetail.LoadMasterTemplate();
        $scope.objCorrespondenceDetail.PopulateAllCorrespondenceTemplate();
        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "Correspondence") {
            $scope.objCorrespondenceDetail.SfwEntity = $scope.SelectedEntity;
            $scope.objCorrespondenceDetail.onEntityChange();
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.SelectedEntity });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objCorrespondenceDetail.Path = strPath;
                }
            }

        }

        $scope.objCorrespondenceDetail.isFinishDisable();
    };

    $scope.objCorrespondenceDetail.PopulateCorrespondenceTemplate = function () {
        $rootScope.IsLoading = true;
        $.connection.hubCreateNewObject.server.loadCorrespondenceTemplate(false).done(function (data) {
            $scope.objCorrespondenceDetail.receiveCorrespondenceTemplate(data);
        });
    };

    $scope.objCorrespondenceDetail.receiveCorrespondenceTemplate = function (data) {
        $scope.$apply(function () {
            $scope.objCorrespondenceDetail.CorTemplateCollection = data;
            $rootScope.IsLoading = false;
        });

    };

    $scope.objCorrespondenceDetail.LoadMasterTemplate = function () {
        $rootScope.IsLoading = true;
        $.connection.hubCreateNewObject.server.loadMasterTemplate().done(function (data) {
            $scope.$apply(function () {
                $scope.objCorrespondenceDetail.MasterTemplateCollection = data;
                $rootScope.IsLoading = false;
            });

        });
    };

    $scope.objCorrespondenceDetail.PopulateAllCorrespondenceTemplate = function () {

        $.connection.hubCorrespondence.server.getListOfAllTemplates().done(function (data) {
            $scope.$apply(function () {
                $scope.objCorrespondenceDetail.AllCorTemplateCollection = data;
            });

        });
    };

    $scope.objCorrespondenceDetail.SelectAssociatedForm = function (associatedForm) {
        $scope.objCorrespondenceDetail.SelectedAssociatedForm = associatedForm;
    };

    $scope.objCorrespondenceDetail.OnAssociatedFormClick = function (associatedForm) {
        $scope.objCorrespondenceDetail.SelectedAssociatedForm = associatedForm;
        if ($scope.objCorrespondenceDetail.SelectedAssociatedForm) {
            var newScope = $scope.$new();
            newScope.SelectedAssociatedForm = $scope.objCorrespondenceDetail.SelectedAssociatedForm;
            newScope.selectedfield = {};
            newScope.onCloseClick = function () {
                newScope.entityTreeDialog.close();
                $scope.objCorrespondenceDetail.isFinishDisable();
            };
            newScope.onOKClick = function () {
                if (newScope.SelectedAssociatedForm.SelectedField) {
                    var displayEntity = getDisplayedEntity($scope.objCorrespondenceDetail.LstDisplayedEntities);
                    var displayName = displayEntity.strDisplayName;
                    fieldName = newScope.SelectedAssociatedForm.SelectedField.ID;
                    if (displayName != "") {
                        fieldName = displayName + "." + newScope.SelectedAssociatedForm.SelectedField.ID;
                    }
                    $scope.objCorrespondenceDetail.SelectedAssociatedForm.SfwTargetBusinessObject = fieldName;//GetItemPathForEntityObject(newScope.SelectedAssociatedForm.SelectedField);
                    $scope.objCorrespondenceDetail.SelectedAssociatedForm.SfwSelectedEntity = newScope.SelectedAssociatedForm.SelectedField.Entity;
                }
                newScope.onCloseClick();
            };
            newScope.entityTreeDialog = $rootScope.showDialog(newScope, "Associated Forms", "CreateNewObject/views/Correspondence/AssociatedForms.html");
        }
    };

    $scope.objCorrespondenceDetail.onCorTemplateChange = function () {
        $scope.objCorrespondenceDetail.PopulateFormTemplate();
        $scope.objCorrespondenceDetail.isFinishDisable();
    };

    $scope.objCorrespondenceDetail.PopulateFormTemplate = function () {
        if ($scope.objCorrespondenceDetail.SfwTemplate && $scope.objCorrespondenceDetail.CorTemplateCollection.some(function (x) { return x == $scope.objCorrespondenceDetail.SfwTemplate; })) {
            $rootScope.IsLoading = true;
            $scope.objCorrespondenceDetail.AssociatedFormCollection = [];
            $.connection.hubCreateNewObject.server.loadFormTemplate($scope.objCorrespondenceDetail.SfwTemplate).done(function (data) {
                $scope.$evalAsync(function () {
                    $scope.objCorrespondenceDetail.AssociatedFormCollection = data;
                    $scope.objCorrespondenceDetail.PopulateDetails();
                    $rootScope.IsLoading = false;
                    $scope.objCorrespondenceDetail.isFinishDisable();
                });
            });
        } else {
            $scope.objCorrespondenceDetail.AssociatedFormCollection = [];
        }
    };

    $scope.objCorrespondenceDetail.PopulateDetails = function () {
        var strBusObject = "";
        var strFilePath = "";

        function getbusObj(objAssForm) {

            if (objAssForm.SfwFormBusinessObject != undefined && objAssForm.SfwFormBusinessObject != "") {
                var strBusObj = objAssForm.SfwFormBusinessObject;

                if (strBusObject == "")
                    strBusObject = strBusObj;
                else if (strBusObject != strBusObj) {
                    strBusObject = "";
                    return;
                }
            }
        }
        angular.forEach($scope.objCorrespondenceDetail.AssociatedFormCollection, getbusObj);



        function iterator(objAssForm) {
            strFilePath = objAssForm.FormLocation;
        }
        angular.forEach($scope.objCorrespondenceDetail.AssociatedFormCollection, iterator);

        if (!$scope.SelectedEntity) {
            $scope.objCorrespondenceDetail.SfwEntity = "";
            $scope.objCorrespondenceDetail.SfwRemoteObject = "";
            if (strBusObject != undefined && strBusObject != "") {
                $scope.objCorrespondenceDetail.SfwEntity = strBusObject;
            }
        }
        // Set the Path from the first available form's location
        if (!$scope.objCorrespondenceDetail.Path) {
            $scope.objCorrespondenceDetail.Path = strFilePath;
        }

        if ($scope.objCorrespondenceDetail.SfwTemplate) {
            $.connection.hubCreateNewObject.server.isUpdateMasterTemplateEnable($scope.objCorrespondenceDetail.SfwTemplate).done(function (isUpdateMasterTemplateEnable) {
                $scope.$apply(function () {
                    if (isUpdateMasterTemplateEnable) {
                        $scope.objCorrespondenceDetail.SfwMasterTemplate = "";
                    }
                    $scope.objCorrespondenceDetail.IsUpdateMasterTemplateEnable = isUpdateMasterTemplateEnable;
                });

            });
        }

    };


    $scope.CreateCorrespondense = function () {
        var lstCorrDetail = [];
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwTemplate);
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwRemoteObject);
        function iterator(itm) {
            itm.SelectedField = undefined;
        }
        angular.forEach($scope.objCorrespondenceDetail.AssociatedFormCollection, iterator);
        lstCorrDetail.push($scope.objCorrespondenceDetail.AssociatedFormCollection);
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwConditionalRule);
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwResource);
        lstCorrDetail.push($scope.objCorrespondenceDetail.Path);
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwMasterTemplate);
        lstCorrDetail.push($scope.objCorrespondenceDetail.IsUpdateMasterTemplateEnable);
        lstCorrDetail.push($scope.objCorrespondenceDetail.SfwEntity);
        lstCorrDetail.push($scope.objCorrespondenceDetail.sfwHeaderTemplate);
        lstCorrDetail.push($scope.objCorrespondenceDetail.sfwFooterTemplate);

        // #region extra field data
        if ($scope.objDirFunctions.prepareExtraFieldData) {
            $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for creating extra field data
        }
        lstCorrDetail.push($scope.objExtraFields);
        $.connection.hubCreateNewObject.server.createCorrespondence(lstCorrDetail).done(function (data) {
            var objDetail = data;
            if (objDetail) {
                $rootScope.openFile(objDetail, true);

                var entityScope = getScopeByFileName($scope.SelectedEntity);
                if (entityScope != undefined) {
                    if (entityScope && entityScope.lstItemsCorrList) {
                        entityScope.lstItemsCorrList.push(objDetail);
                    }
                }

                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });
                $scope.dialogForNew.close();
            }
        });
    };

    $scope.objCorrespondenceDetail.onEntityChange = function () {
        $scope.objCorrespondenceDetail.isFinishDisable();
    };

    //#region Validation Methods For Correspondence Details

    $scope.objCorrespondenceDetail.isFinishDisable = function () {
        var isValidDisable = false;
        $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "";
        if (!$scope.objCorrespondenceDetail.ValidateCorrespondenceTemplate()) {
            isValidDisable = true;
        }
        else if (!$scope.objCorrespondenceDetail.ValidateEntityName()) {
            isValidDisable = true;
        }
        else if (!$scope.objCorrespondenceDetail.ValidateTargetBusObject()) {
            isValidDisable = true;
        }
        else if (!$scope.objCorrespondenceDetail.SfwResource || $scope.objCorrespondenceDetail.SfwResource == "") {
            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Please select a resource.";
            isValidDisable = true;
        }
        else if ($scope.resourceList && $scope.resourceList.indexOf($scope.objCorrespondenceDetail.SfwResource.trim()) <= -1) {
            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: invalid Resource id.";
            return false;
        }
        else if (!$scope.ValidatePath($scope.objCorrespondenceDetail, $scope.objCorrespondenceDetail.Path)) {
            isValidDisable = true;
        }
        else if (!$scope.objCorrespondenceDetail.ValidateMasterTemplate()) {
            isValidDisable = true;
        }
        $scope.objCorrespondenceDetail.isCorrespondenceDetailFinishDisable = isValidDisable;
        return isValidDisable;
    };

    $scope.objCorrespondenceDetail.ValidateCorrespondenceTemplate = function () {
        var isValid = true;
        if ($scope.objCorrespondenceDetail.SfwTemplate == undefined || $scope.objCorrespondenceDetail.SfwTemplate == "") {
            isValid = false;
            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Please select a Correspondence Template.";
        }
        else if (!$scope.objCorrespondenceDetail.CorTemplateCollection.some(function (x) { return x == $scope.objCorrespondenceDetail.SfwTemplate; })) {
            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Invalid Correspondence Template.";
            isValid = false;
        }
        return isValid;
    };

    $scope.objCorrespondenceDetail.ValidateEntityName = function () {
        var isValid = true;
        if ($scope.objCorrespondenceDetail.SfwEntity == undefined || $scope.objCorrespondenceDetail.SfwEntity == "") {
            isValid = false;
            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Please select a Entity.";
        }
        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objCorrespondenceDetail.SfwEntity; })) {
                $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Invalid Entity.";
                isValid = false;
            }
        }
        return isValid;
    };

    $scope.objCorrespondenceDetail.ValidateTargetBusObject = function () {
        var isValid = true;
        if ($scope.objCorrespondenceDetail.AssociatedFormCollection.length > 0) {
            angular.forEach($scope.objCorrespondenceDetail.AssociatedFormCollection, function (objForm) {
                if (objForm.SfwFormBusinessObject != undefined && objForm.SfwFormBusinessObject != "") {
                    if (objForm.SfwTargetBusinessObject == undefined || objForm.SfwTargetBusinessObject == "") {
                        var strFormBusObject = objForm.SfwFormBusinessObject;
                        if (strFormBusObject != $scope.objCorrespondenceDetail.SfwEntity) {
                            isValid = false;
                            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Please select a Target Entity for the form.";
                        }
                    }
                    else {
                        var strBusObjectType = objForm.SfwSelectedEntity;
                        if (strBusObjectType != $scope.objCorrespondenceDetail.SfwEntity) {
                            isValid = false;
                            $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "The Target Entity must be of type main Entity.";
                        }
                    }
                }
            });
        }
        return isValid;
    };

    $scope.objCorrespondenceDetail.ValidateMasterTemplate = function () {
        var isValid = true;
        if ($scope.objCorrespondenceDetail.IsUpdateMasterTemplateEnable) {
            if ($scope.objCorrespondenceDetail.SfwMasterTemplate == undefined || $scope.objCorrespondenceDetail.SfwMasterTemplate == "") {
                isValid = false;
                $scope.objCorrespondenceDetail.ErrorMessageForDisplay = "Error: Please select a Master Template for the Correspondence.";
            }
        }
        return isValid;
    };

    //#endregion

    //#endregion

    //#region Create Inbound/Outbound File Methods

    $scope.CreateFile = function () {

        var obj = {};
        // for file Details(First Screen)
        obj.FileName = $scope.objFileDetails.FileName;
        obj.FileID = $scope.objFileDetails.FileID;
        obj.Description = $scope.objFileDetails.Description;
        obj.FileType = $scope.objFileDetails.FileType;
        if ($scope.objFileDetails.FileType == 'Outbound') {
            obj.QueryID = $scope.objFileDetails.QueryID;
            obj.ObjectMethod = $scope.objFileDetails.ObjectMethod;
            obj.FileCollection = $scope.objFileDetails.FileCollection;
            obj.strFileName = $scope.objFileDetails.strFileName;


        }
        obj.Path = $scope.objFileDetails.Path;
        obj.FileObject = $scope.objFileDetails.ObjectID;
        obj.ProgressCounter = $scope.objFileDetails.ProgressCounter;
        obj.RecordType = $scope.objFileDetails.RecordType;
        obj.DelimitedCharacter = $scope.objFileDetails.DelimitedCharacter;

        // for file Details(Second Screen)
        obj.TransactionCodePosition = $scope.objInboundOutboundDetails.TransactionCodePosition;
        obj.TransactionCodeLength = $scope.objInboundOutboundDetails.TransactionCodeLength;
        obj.HeaderGroupPosition = $scope.objInboundOutboundDetails.HeaderGroupPosition;
        obj.HeaderGroupLength = $scope.objInboundOutboundDetails.HeaderGroupLength;
        obj.InboundMask = $scope.objInboundOutboundDetails.InboundMask;
        obj.BasePath = $scope.objInboundOutboundDetails.BasePath;
        obj.MailBoxPathCode = $scope.objInboundOutboundDetails.MailBoxPathCode;
        obj.MailBoxKey = $scope.objInboundOutboundDetails.MailBoxKey;
        obj.MailBoxPath = $scope.objInboundOutboundDetails.MailBoxPath;
        obj.ProcessPathCode = $scope.objInboundOutboundDetails.ProcessPathCode;
        obj.ProcessPathKey = $scope.objInboundOutboundDetails.ProcessPathKey;
        obj.ProcessPath = $scope.objInboundOutboundDetails.ProcessPath;
        obj.StagingPathCode = $scope.objInboundOutboundDetails.StagingPathCode;
        obj.StagingPathKey = $scope.objInboundOutboundDetails.StagingPathKey;
        obj.StagingPath = $scope.objInboundOutboundDetails.StagingPath;
        obj.ErrorPathCode = $scope.objInboundOutboundDetails.ErrorPathCode;
        obj.ErrorPathKey = $scope.objInboundOutboundDetails.ErrorPathKey;
        obj.ErrorPath = $scope.objInboundOutboundDetails.ErrorPath;
        obj.RecordLayout = $scope.objInboundOutboundDetails.RecordLayout;
        obj.sfwOutboundProcessDetail = $scope.objInboundOutboundDetails.SfwProcessDetail;
        obj.SfwProcessDetail = $scope.objFileDetails.ProcessDetail;

        obj.SfwRecieveToFileDetail = $scope.objFileDetails.RecieveToFileDetail;
        obj.SfwUploadInParallel = $scope.objFileDetails.UploadInParallel;
        obj.SfwUploadToFileDetail = $scope.objFileDetails.UploadToFileDetail;
        obj.SfwGetGroupValueFromHeader = $scope.objFileDetails.GetGroupValueFromHeader;
        obj.SfwProcessInParallel = $scope.objFileDetails.ProcessInParallel;
        obj.SfwCommitSingle = $scope.objFileDetails.CommitSingle;

        obj.IsProcessInGroup = $scope.objInboundOutboundDetails.IsProcessInGroup;

        // #region extra field data
        if ($scope.objDirFunctions.prepareExtraFieldData) {
            $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for creating extra field data
        }
        obj.ExtraFields = $scope.objExtraFields;
        //endregion
        // server call for creating call
        $.connection.hubCreateNewObject.server.createFiles(obj).done(function (data) {
            var objDetail = data;
            if (objDetail) {
                $rootScope.openFile(objDetail, true);
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                });

                $scope.dialogForNew.close();
            }
        });


    };

    $scope.objFileDetails.CanShowFileSteps = function () {
        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "File") {
            retVal = false;
        }
        return retVal;
    };

    $scope.objFileDetails.InitFile = function () {
        $scope.objFileDetails.FileType = "Inbound";
        $scope.objFileDetails.RecordType = "FixedLength";
        $scope.objFileDetails.ObjectID = "";
        $scope.objFileDetails.FileName = "";
        $scope.objFileDetails.FileID = "";
        $scope.objFileDetails.Description = "";
        $scope.objFileDetails.Path = "";
        $scope.objFileDetails.ProgressCounter = "";
        $scope.objFileDetails.DelimitedCharacter = "";
        $scope.objFileDetails.FileCollection = "";
        $scope.objFileDetails.strFileName = "";
        $scope.objFileDetails.ObjectMethod = "";
        $scope.objFileDetails.QueryID = "";
        if (!$scope.objInboundOutboundDetails.SfwProcessDetail) {
            $scope.objInboundOutboundDetails.SfwProcessDetail = "True";
        }

        $.connection.hubCreateNewObject.server.getListOfFormName().done(function (data) {
            $scope.lstFormsList = [];
            $scope.$apply(function () {
                $scope.lstFormsList = data;

            });
        });
        $.connection.hubCreateNewObject.server.getListOfFormID().done(function (data) {
            $scope.lstFormsIDList = [];
            $scope.$apply(function () {
                $scope.lstFormsIDList = data;

            });
        });
        $scope.objFileDetails.isNextDisable();
    };
    $scope.objFileDetails.UpdateOnFileDetailsNext = function () {
        var isNextDisable = false;
        $scope.objInboundOutboundDetails.isFinishDisable();
        return isNextDisable;
    };

    $scope.objFileDetails.OnInboundObjectIdChange = function () {
        if ($scope.objFileDetails.IsChildOfInboundFileBase) {
            $scope.objFileDetails.UploadToFileDetail = true;
            $scope.objFileDetails.CommitSingle = true;
            $scope.objFileDetails.ProcessDetail = true;
        }
        else {
            $scope.objFileDetails.RecieveToFileDetail = false;

            $scope.objFileDetails.UploadInParallel = false;
            $scope.objFileDetails.UploadToFileDetail = false;
            $scope.objFileDetails.GetGroupValueFromHeader = false;

            $scope.objFileDetails.ProcessInParallel = false;
            $scope.objFileDetails.CommitSingle = false;
            $scope.objFileDetails.ProcessDetail = false;

        }
        $scope.objFileDetails.isNextDisable();
    };

    $scope.objFileDetails.OnOutboundObjectIdChange = function () {
        $.connection.hubCreateNewObject.server.populateObjectMethod($scope.objFileDetails.ObjectID).done(function (data) {
            $scope.$evalAsync(function () {
                $scope.objFileDetails.lstObjectMethod = data;
            });
        });

        $.connection.hubCreateNewObject.server.populateFileCollection($scope.objFileDetails.ObjectID).done(function (data) {
            $scope.$evalAsync(function () {
                $scope.objFileDetails.lstFileCollection = data;
            });
        });
        $scope.objFileDetails.isNextDisable();
    };

    $scope.populatePathCode = function () {

        $.connection.hubCreateNewObject.server.populatePath().done(function (data) {
            $scope.lstPopulateCode = [];
            $scope.$apply(function () {
                $scope.lstPopulateCode = data;

            });
        });
    };

    $scope.PopulateBasePath = function () {
        $.connection.hubCreateNewObject.server.populateBasePath().done(function (data) {
            $scope.$apply(function () {
                $scope.objInboundOutboundDetails.BasePath = data;
            });
        });
    };

    $scope.SelectFileForPath = function (pathType) {
        if (pathType) {
            if (pathType == "MailBoxPath") {
                $.connection.hubCreateNewObject.server.openDialog($scope.objInboundOutboundDetails.MailBoxPath, pathType);
            }
            else if (pathType == "ProcessPath") {
                $.connection.hubCreateNewObject.server.openDialog($scope.objInboundOutboundDetails.ProcessPath, pathType);
            }
            else if (pathType == "StagingPath") {
                $.connection.hubCreateNewObject.server.openDialog($scope.objInboundOutboundDetails.StagingPath, pathType);
            }
            else if (pathType == "ErrorPath") {
                $.connection.hubCreateNewObject.server.openDialog($scope.objInboundOutboundDetails.ErrorPath, pathType);

            }

        }
    };
    $scope.receiveFolderPathForFile = function (data, source) {
        $scope.FilePath = undefined;
        $scope.$apply(function () {

            if (source == "MailBoxPath") {
                $scope.objInboundOutboundDetails.MailBoxPath = data;
            }
            else if (source == "ProcessPath") {
                $scope.objInboundOutboundDetails.ProcessPath = data;
            }
            else if (source == "StagingPath") {
                $scope.objInboundOutboundDetails.StagingPath = data;
            }
            else if (source == "ErrorPath") {
                $scope.objInboundOutboundDetails.ErrorPath = data;

            }

        });
    };


    $scope.onChangeOfPathCode = function (data, pathCodeType) {
        var codePath = $scope.lstPopulateCode.filter(function (x) { return x.codeValue.toLowerCase() == data.toLowerCase(); });
        var path = codePath && codePath.length > 0 ? codePath[0].codePath : "";

        if (pathCodeType == "MailBoxPath") {
            $scope.objInboundOutboundDetails.MailBoxPath = path;
        }
        else if (pathCodeType == "ProcessPath") {
            $scope.objInboundOutboundDetails.ProcessPath = path;
        }
        else if (pathCodeType == "StagingPath") {
            $scope.objInboundOutboundDetails.StagingPath = path;
        }
        else if (pathCodeType == "ErrorPath") {
            $scope.objInboundOutboundDetails.ErrorPath = path;
        }

        $scope.objInboundOutboundDetails.isFinishDisable();
    };
    $scope.isExistingPath = function (codeValue) {
        if ($scope.lstPopulateCode && $scope.lstPopulateCode.length > 0 && codeValue) {
            var flag = $scope.lstPopulateCode.filter(function (item) { return item.codeValue == codeValue; }).length > 0;
            return flag;
        }
        return false;
    };

    //#endregion

    //#region  Validation for File

    $scope.objFileDetails.isNextDisable = function () {
        var IsValid = true;
        $scope.objFileDetails.ErrorMessageForDisplay = "";
        if (!$scope.objFileDetails.ValidateFormName()) {
            IsValid = false;
        }
        else if (!$scope.objFileDetails.ValidateFileID()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objFileDetails, $scope.objFileDetails.Path)) {
            IsValid = false;
        }
        else if (!$scope.objFileDetails.ValidateEntity()) {
            IsValid = false;
        }
        else if (!$scope.objFileDetails.ValidateDelimitedCharacter()) {
            IsValid = false;
        }
        $scope.objFileDetails.isFileDetailsNextDisable = IsValid;
        return IsValid;
    };

    $scope.objFileDetails.FileTypeChanged = function () {
        $scope.objFileDetails.ObjectID = "";
        $scope.objFileDetails.IsChildOfInboundFileBase = false;
        $scope.objFileDetails.isNextDisable();
    };

    // for File Name Validation
    $scope.objFileDetails.ValidateFormName = function () {
        var retValue = true;
        if (!$scope.objFileDetails.FileName) {
            $scope.objFileDetails.ErrorMessageForDisplay = "Error: Please enter a File Name (ID).";
            retValue = false;
        }
        else if ($scope.objFileDetails.FileName && $scope.objFileDetails.FileName.toLowerCase().startsWith("fle")) {
            $scope.objFileDetails.ErrorMessageForDisplay = "Error: ID can not start with fle.";
            retValue = false;
        }
        else if (($scope.objFileDetails.FileName) && $scope.objFileDetails.FileName.length <= 3) {
            $scope.objFileDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }
        else {
            if (!isValidFileID($scope.objFileDetails.FileName)) {
                $scope.objFileDetails.ErrorMessageForDisplay = "Error: File Name must not start with digits, contain space(s), special characters(except '-').";
                retValue = false;
            }
            if (angular.isArray($scope.lstFormsList) && $scope.lstFormsList.length > 0) {
                var lst = $scope.lstFormsList.filter(function (x) { return x.toLowerCase() == "fle" + $scope.objFileDetails.FileName.toLowerCase(); });
                if (lst.length > 0) {
                    $scope.objFileDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different File Name (ID).";
                    retValue = false;
                }
            }


        }
        return retValue;
    };
    // for File Id Validation
    $scope.objFileDetails.ValidateFileID = function () {
        var retValue = true;
        if (!$scope.objFileDetails.IsFileIdValueValid) {
            if (!$scope.objFileDetails.FileID) {

                this.ErrorMessageForDisplay = "Error: Please enter a File ID.";
                retValue = false;
            }
            else {
                var reg = new RegExp("[^0-9]");

                if (reg.test($scope.objFileDetails.FileID)) {
                    $scope.objFileDetails.ErrorMessageForDisplay = "Error: Invalid File ID.";
                    retValue = false;
                }
                else {
                    var result;
                    if (!parseInt($scope.objFileDetails.FileID)) {
                        $scope.objFileDetails.ErrorMessageForDisplay = "Error: The File ID is too Long.\nPlease enter a different File ID.";
                        retValue = false;

                    }
                    else {
                        var lst = $scope.lstFormsIDList.filter(function (x) { return x == $scope.objFileDetails.FileID; });
                        if (lst.length > 0) {
                            $scope.objFileDetails.ErrorMessageForDisplay = "Error: The File ID already exists.\nPlease enter a different File ID.";
                            retValue = false;
                        }
                    }
                }
            }
        }
        return retValue;
    };

    $scope.objFileDetails.ValidateEntity = function () {
        var isValid = true;
        if (!$scope.objFileDetails.ObjectID) {
            $scope.objFileDetails.ErrorMessageForDisplay = "Error: File Object cannot be empty.";
            isValid = false;
        }

        //else {
        //    if (!$rootScope.entityIntellisenseList.some(function (x) { return x.ID == $scope.objFileDetails.ObjectID })) {
        //        $scope.objFileDetails.ErrorMessageForDisplay = "Error: The entity ent" + $scope.objFileDetails.ObjectID + " is not valid. Please enter a different Name (ID).";
        //        isValid = false;

        //    }
        //}
        return isValid;
    };
    //  to validate Delimited char
    $scope.objFileDetails.ValidateDelimitedCharacter = function () {

        var retValue = true;
        if ($scope.objFileDetails.RecordType == 'Delimited') {
            if ($scope.objFileDetails.DelimitedCharacter == undefined || $scope.objFileDetails.DelimitedCharacter == "") {
                $scope.objFileDetails.ErrorMessageForDisplay = "Error: Please enter a Delimited Character.";
                retValue = false;
            }
        }
        return retValue;
    };

    //#endregion

    //#region for Inbound/Outbound validation

    $scope.objInboundOutboundDetails.isFinishDisable = function () {
        var IsValid = true;
        $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "";

        if (!$scope.objInboundOutboundDetails.ValidateHeaderGroupPosition()) {
            IsValid = false;
        }
        else if (!$scope.objInboundOutboundDetails.ValidateHeaderGroupLength()) {
            IsValid = false;
        }
        else if (!$scope.objInboundOutboundDetails.ValidateTransactionCodePosition()) {
            IsValid = false;
        }
        else if (!$scope.objInboundOutboundDetails.ValidateTransactionCodeLength()) {
            IsValid = false;
        }
        else if (!$scope.objInboundOutboundDetails.ValidateMailBoxPath()) {
            IsValid = false;
        }
        else if (!$scope.objInboundOutboundDetails.ValidateProcessPath()) {
            IsValid = false;
        }
        else if ($scope.objFileDetails.FileType == 'Inbound') {
            if (!$scope.objInboundOutboundDetails.ValidateStagingPath()) {
                IsValid = false;
            }
            else if (!$scope.objInboundOutboundDetails.ValidateErrorPath()) {
                IsValid = false;
            }
        }

        $scope.objInboundOutboundDetails.isFileFinishDisable = IsValid;
        return IsValid;
    };

    //#region to validate Transaction Code Position
    $scope.objInboundOutboundDetails.ValidateTransactionCodePosition = function () {

        var retValue = true;
        if ($scope.objFileDetails.FileType == 'Inbound') {
            if ($scope.objInboundOutboundDetails.TransactionCodePosition) {
                var reg = new RegExp("[^0-9]");
                if (reg.test($scope.objInboundOutboundDetails.TransactionCodePosition)) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Transaction Code Position should be number.";
                    retValue = false;
                }
            }

        }
        return retValue;
    };
    //#endregion
    //#region to validate Transaction Code Length
    $scope.objInboundOutboundDetails.ValidateTransactionCodeLength = function () {

        var retValue = true;
        if ($scope.objFileDetails.FileType == 'Inbound') {
            if ($scope.objInboundOutboundDetails.TransactionCodeLength) {
                var reg = new RegExp("[^0-9]");
                if (reg.test($scope.objInboundOutboundDetails.TransactionCodeLength)) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Transaction Code Length should be number.";
                    retValue = false;
                }
            }

        }
        return retValue;
    };
    //#endregion

    // to validate header Group position
    $scope.objInboundOutboundDetails.ValidateHeaderGroupPosition = function () {

        var retValue = true;
        if ($scope.objFileDetails.FileType == 'Inbound') {
            if (!$scope.objInboundOutboundDetails.HeaderGroupPosition) {
                $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please enter a Header Group Position.";
                retValue = false;
            }
            else {
                var reg = new RegExp("[^0-9]");

                if (reg.test($scope.objInboundOutboundDetails.HeaderGroupPosition)) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Header Group Position should be number.";
                    retValue = false;
                }
            }
        }
        return retValue;
    };

    // validate header group length
    $scope.objInboundOutboundDetails.ValidateHeaderGroupLength = function () {
        var retValue = true;
        if ($scope.objFileDetails.FileType == 'Inbound') {
            if ($scope.objFileDetails.RecordType != 'Delimited') {
                if (!$scope.objInboundOutboundDetails.HeaderGroupLength) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please enter a Header Group Length.";
                    retValue = false;
                }
                else {
                    var reg = new RegExp("[^0-9]");

                    if (reg.test($scope.objInboundOutboundDetails.HeaderGroupLength)) {
                        $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Header Group Length should be number.";
                        retValue = false;
                    }
                }
            }
        }
        return retValue;
    };
    // Validate Mail Box Path
    $scope.objInboundOutboundDetails.ValidateMailBoxPath = function () {
        var retValue = true;
        if ($scope.objInboundOutboundDetails.MailBoxPathCode == undefined || $scope.objInboundOutboundDetails.MailBoxPathCode == "") {
            $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please select or enter a Mail Box Path.";
            retValue = false;
        }
        if (!$scope.IsPathKey) {
            if ($scope.objInboundOutboundDetails.MailBoxKey == undefined || $scope.objInboundOutboundDetails.MailBoxKey == "") {
                $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please enter a MailBox Path Key.";
                retValue = false;
            }
            else {
                var lst = $scope.lstPopulateCode.filter(function (x) { return x.pathID && x.pathID == $scope.objInboundOutboundDetails.MailBoxKey; });

                if (lst.length > 0) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The key '" + $scope.objInboundOutboundDetails.MailBoxKey + "' already exists.\nPlease enter a different Mail Box Path Key.";
                    retValue = false;
                }
                else {
                    if ($scope.objInboundOutboundDetails.MailBoxPathCode === $scope.objInboundOutboundDetails.ProcessPathCode) {
                        $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.MailBoxPathCode + "' is already used for Process Path.\nPlease enter a different  MailBox Path.";
                        retValue = false;
                    }
                    else if ($scope.objFileDetails.FileType == 'Inbound') {
                        {
                            if ($scope.objInboundOutboundDetails.ProcessPathCode === $scope.objInboundOutboundDetails.StagingPathCode) {
                                $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.MailBoxPathCode + "' is already used for Staging Path.\nPlease enter a different  MailBox Path.";
                                retValue = false;
                            }
                            else if ($scope.objInboundOutboundDetails.ProcessPathCode === $scope.objInboundOutboundDetails.ErrorPathCode) {
                                $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.MailBoxPathCode + "' is already used for Error Path.\nPlease enter a different  MailBox Path.";
                                retValue = false;
                            }
                        }
                    }
                }
            }

        }
        return retValue;
    };

    // validate staging Path

    $scope.objInboundOutboundDetails.ValidateStagingPath = function () {
        var retValue = true;
        if ($scope.objInboundOutboundDetails.StagingPathCode == "" || $scope.objInboundOutboundDetails.StagingPathCode == undefined) {
            $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please select or enter a Staging Path.";
            retValue = false;
        }
        else if ($scope.lstPopulateCode.some(function (x) { return x.codeValue && x.codeValue.toLowerCase() == $scope.objInboundOutboundDetails.StagingPathCode.toLowerCase(); })) {
            if ($scope.objInboundOutboundDetails.StagingPathCode) {
                if ($scope.objInboundOutboundDetails.StagingPathCode === $scope.objInboundOutboundDetails.MailBoxPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.StagingPathCode + "' is already used for MailBox Path.\nPlease enter a different Staging Path.";
                    retValue = false;
                }
                else if ($scope.objInboundOutboundDetails.StagingPathCode === $scope.objInboundOutboundDetails.ErrorPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.StagingPathCode + "' is already used for Error Path.\nPlease enter a different Staging Path.";
                    retValue = false;
                }
                else if ($scope.objInboundOutboundDetails.StagingPathCode === $scope.objInboundOutboundDetails.ProcessPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.StagingPathCode + "' is already used for Process Path.\nPlease enter a different Staging Path.";
                    retValue = false;
                }
            }
        }
        return retValue;
    };
    // validate Error  Path
    $scope.objInboundOutboundDetails.ValidateErrorPath = function () {

        var retValue = true;
        if ($scope.objInboundOutboundDetails.ErrorPathCode == undefined || $scope.objInboundOutboundDetails.ErrorPathCode == "") {
            $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please select or enter a Error Path.";
            retValue = false;
        }
        else {
            if ($scope.lstPopulateCode.some(function (x) { return x.codeValue && x.codeValue.toLowerCase() == $scope.objInboundOutboundDetails.ErrorPathCode.toLowerCase(); })) {

                if ($scope.objInboundOutboundDetails.ErrorPathCode === $scope.objInboundOutboundDetails.MailBoxPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ErrorPathCode + "' is already used for MailBox Path.\nPlease enter a different Error Path.";
                    retValue = false;
                }
                else if ($scope.objInboundOutboundDetails.ErrorPathCode === $scope.objInboundOutboundDetails.StagingPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ErrorPathCode + "' is already used for Staging Path.\nPlease enter a different Error Path.";
                    retValue = false;
                }
                else if ($scope.objInboundOutboundDetails.ErrorPathCode === $scope.objInboundOutboundDetails.ProcessPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ErrorPathCode + "' is already used for Process Path.\nPlease enter a different Error Path.";
                    retValue = false;
                }
            }
        }
        return retValue;
    };
    // validate Process Path
    $scope.objInboundOutboundDetails.ValidateProcessPath = function () {
        var retValue = true;
        if ($scope.objInboundOutboundDetails.ProcessPathCode == undefined || $scope.objInboundOutboundDetails.ProcessPathCode == "") {
            $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: Please select or enter a Process Path.";
            retValue = false;
        }
        else {
            if ($scope.lstPopulateCode.some(function (x) { return x.codeValue && x.codeValue.toLowerCase() == $scope.objInboundOutboundDetails.ProcessPathCode.toLowerCase(); })) {
                if ($scope.objInboundOutboundDetails.ProcessPathCode === $scope.objInboundOutboundDetails.MailBoxPathCode) {
                    $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ProcessPathCode + "' is already used for MailBox Path.\nPlease enter a different Process Path.";
                    retValue = false;
                }
                else if ($scope.objInboundOutboundDetails.FileType == "Inbound") {
                    if ($scope.objInboundOutboundDetails.ProcessPathCode === $scope.objInboundOutboundDetails.StagingPathCode) {
                        $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ProcessPathCode + "' is already used for Staging Path.\nPlease enter a different Process Path.";
                        retValue = false;
                    }
                    else if ($scope.objInboundOutboundDetails.ProcessPathCode === $scope.objInboundOutboundDetails.ErrorPathCode) {
                        $scope.objInboundOutboundDetails.ErrorMessageForDisplay = "Error: The path '" + $scope.objInboundOutboundDetails.ProcessPathCode + "' is already used for Error Path.\nPlease enter a different Process Path.";
                        retValue = false;
                    }
                }
            }
        }
        return retValue;
    };
    //#endregion

    //#region for Query Dialog
    $scope.openQueryDialog = function () {
        $scope.strPath = $scope.objFileDetails.Path;
        //$scope.QueryDialog = ngDialog.open({
        //    template: "Views/Form/BrowseForQuery.html",
        //    scope: $scope,
        //    closeByDocument: false,
        //    className: 'ngdialog-theme-default ngdialog-theme-custom',
        //});

    };

    $scope.$on('onQueryClick', function (event, data) {
        $scope.objFileDetails.QueryID = data;
    });
    //#endregion

    //#region Forms Creation Implementation

    //#region Init Form Function
    $scope.objFormsDetails.InitForms = function () {
        $scope.objFormsDetails.lstWebSite = [];
        $scope.objFormsDetails.MethodType = 'XmlMethod';
        $scope.objFormsDetails.ID = "";
        $scope.objFormsDetails.sfwResource = "";
        $scope.objFormsDetails.Path = "";
        $scope.objFormsDetails.sfwEntity = "";
        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "Forms") {
            $scope.objFormsDetails.sfwEntity = $scope.SelectedEntity;
            $scope.objFormsDetails.PopulateFormName();
        }
        $scope.objFormsDetails.IsLookup = "True";
        $scope.objFormsDetails.IsMaintenence = "True";
        $scope.objFormsDetails.PrevEntityName = "";

        $scope.objFormsDetails.SfwGridCollection = "";
        $scope.objLookupFieldsDetails.lstFieldsProperties = [];
        $scope.objLookupResultFields.lstResultFields = [];
        $scope.objLookupFieldsDetails.lstselectedmultiplelevelfield = [];

        $scope.objFormsDetails.isNextDisable();
    };

    $scope.receiveRemoteObjectCollection = function (data) {
        if ($scope.objNewItems.SelectedOption == "HtxWizard" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxLookup") {
            $scope.$evalAsync(function () {
                $scope.objFormLinkDetails.RemoteObjectCollection = data;
                if ($scope.objFormLinkDetails.RemoteObjectCollection && $scope.objFormLinkDetails.RemoteObjectCollection.length > 0) {
                    $scope.objFormLinkDetails.RemoteObjectCollection.splice(0, 0, { dictAttributes: { ID: "" } });
                    $scope.objFormLinkDetails.onRemoteObjectChange();
                    if ($scope.objNewItems.SelectedOption == "HtxWizard" || $scope.objNewItems.SelectedOption == "HtxMaintenance") {
                        $scope.objFormLinkDetails.populateSrvParamtersForNew($scope.objFormLinkDetails.NewMethod.sfwMethodName);
                        $scope.objFormLinkDetails.populateSrvParamtersForUpdate($scope.objFormLinkDetails.UpdateMethod.sfwMethodName);
                    }
                }
            });
        }
    };

    $scope.objFormLinkDetails.populateSrvParamtersForNew = function (srvMethod) {
        $scope.paramtersForNewMethod = [];
        function params(itm) {
            if (itm.Name == "parameter") {
                $scope.paramtersForNewMethod.push(itm);
            }
        }

        if (srvMethod) {
            var lst = $scope.XmlNewMethodsCollection.filter(function (x) {
                return x.dictAttributes.ID == srvMethod && x.dictAttributes.sfwMode != "Update";
            });
            if (lst && lst.length > 0) {
                angular.forEach(lst[0].Elements, params);

            }
        }
    };

    $scope.objFormLinkDetails.populateSrvParamtersForUpdate = function (srvMethod) {
        $scope.paramtersForUpdateMethod = [];
        function iterator(itm) {
            if (itm.Name == "parameter") {
                $scope.paramtersForUpdateMethod.push(itm);
            }
        }

        if (srvMethod) {

            var lst = $scope.XmlUpdateMethodsCollection.filter(function (x) { return x.dictAttributes.ID == srvMethod && x.dictAttributes.sfwMode != "New"; });
            if (lst && lst.length > 0) {


                angular.forEach(lst[0].Elements, iterator);
            }
        }
    };

    $scope.objFormsDetails.onWebSiteChange = function (event) {
        var input = $(event.target);
        setSingleLevelAutoComplete(input, $scope.objFormsDetails.lstWebSite);
    };

    //#endregion

    //#region User Control Implementation
    $scope.objUserCtrlDetails.InitUserControl = function () {
        $scope.objUserCtrlDetails.ID = "";
        $scope.objUserCtrlDetails.Path = "";
        $scope.objUserCtrlDetails.sfwEntity = "";
        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "UserControl") {
            $scope.objUserCtrlDetails.sfwEntity = $scope.SelectedEntity;
            $scope.objUserCtrlDetails.PopulateFormName();
            if ($scope.objUserCtrlDetails.sfwEntity === "entPrototype") {
                $scope.objUserCtrlDetails.isPrototype = true;
            }
        }

        $scope.objUserCtrlDetails.isFinsihDisable();
    };

    $scope.objUserCtrlDetails.ValidateUserCtrl = function () {
        var retValue = true;
        if ($scope.objUserCtrlDetails.ID == undefined || $scope.objUserCtrlDetails.ID == "") {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: Please enter a User Control ID.";
            return false;
        }
        else if (!isValidFileID($scope.objUserCtrlDetails.ID, true)) {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
            return false;
        }
        else if ($scope.objUserCtrlDetails.ID && $scope.objUserCtrlDetails.ID.toLowerCase().startsWith("udc")) {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: ID can not start with udc.";
            return false;
        }
        else if (($scope.objUserCtrlDetails.ID != undefined || $scope.objUserCtrlDetails.ID != "") && $scope.objUserCtrlDetails.ID.length <= 3) {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            return false;
        }
        else if ($scope.objUserCtrlDetails.ID && $scope.objUserCtrlDetails.ID.toLowerCase().endsWith("usercontrol")) {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: ID can not ends with usercontrol.";
            return false;
        } else {
            var udcId = "udc" + $scope.objUserCtrlDetails.ID + "UserControl";
            var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == udcId.toLowerCase(); });
            if (lst.length > 0) {
                $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different User control ID.";
                return false;
            }
        }
        if (!$scope.ValidatePath($scope.objUserCtrlDetails, $scope.objUserCtrlDetails.Path)) {
            return false;
        }
        if (!$scope.objUserCtrlDetails.sfwEntity || $scope.objUserCtrlDetails.sfwEntity == '') {
            $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: Please enter a Entity.";
            return false;
        } else if ($scope.objUserCtrlDetails.sfwEntity && $scope.objUserCtrlDetails.sfwEntity != '') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objUserCtrlDetails.sfwEntity; })) {
                $scope.objUserCtrlDetails.ErrorMessageForDisplay = "Error: Invalid Entity.";
                return false;
            }
        }
        return retValue;
    };
    $scope.objUserCtrlDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objUserCtrlDetails.ErrorMessageForDisplay = "";

        if (!$scope.objUserCtrlDetails.ValidateUserCtrl()) {
            IsValid = false;
        }
        $scope.objUserCtrlDetails.isUserCtrlFinsihDisable = IsValid;
        return IsValid;
    };

    $scope.objUserCtrlDetails.PopulateFormName = function () {
        $scope.objUserCtrlDetails.ID = "";
        if ($scope.objUserCtrlDetails.sfwEntity != undefined && $scope.objUserCtrlDetails.sfwEntity != "") {
            $scope.objUserCtrlDetails.ID = $scope.objUserCtrlDetails.sfwEntity.substring(3);
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.objUserCtrlDetails.sfwEntity; });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objUserCtrlDetails.Path = strPath;
                }

            }
        }
        $scope.checkAndExecuteLocationCallback($scope.objUserCtrlDetails);
    };

    $scope.objUserCtrlDetails.UpdateData = function () {
        $scope.objUserCtrlDetails.updateDetails();
        $scope.objUserCtrlDetails.UpdateUserCtrlParameters();
    };

    $scope.objUserCtrlDetails.updateDetails = function () {
        var strUserCtrlID = 'udc' + $scope.objUserCtrlDetails.ID + "UserControl";
        $scope.objUserCtrl = { Name: 'form', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        $scope.objUserCtrl.dictAttributes.ID = strUserCtrlID;
        // $scope.objUserCtrl.dictAttributes.Path = $scope.objUserCtrlDetails.Path;
        $scope.objUserCtrl.dictAttributes.sfwEntity = $scope.objUserCtrlDetails.sfwEntity;
        $scope.objUserCtrl.dictAttributes.sfwType = 'UserControl';
        $scope.objUserCtrl.dictAttributes.sfwTitle = strUserCtrlID;
    };

    $scope.objUserCtrlDetails.UpdateUserCtrlParameters = function () {
        var objSfxTable = { Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };//.SetName(ApplicationConstants.XMLFacade.SFWTABLE, ApplicationConstants.XMLFacade.PREFIX_SWC);
        objSfxTable.dictAttributes.ID = "Main";

        var newRow = { Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        for (var i = 0; i < 3; i++) {
            var newCell = { Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: [] };
            newRow.Elements.push(newCell);
        }

        objSfxTable.Elements.push(newRow);

        $scope.objUserCtrl.Elements.push(objSfxTable);
    };
    $scope.objUserCtrlDetails.toggleIsPrototype = function () {
        if ($scope.objUserCtrlDetails.isPrototype) {
            $scope.objUserCtrlDetails.sfwEntity = "entPrototype";
        }
        else {
            $scope.objUserCtrlDetails.sfwEntity = "";
        }
    }

    //#endregion End of User control Implementation

    //#region start of Tooltip form implementaion 
    $scope.objTooltipDetails.InitTooltip = function () {
        $scope.objTooltipDetails.ID = "";
        $scope.objTooltipDetails.sfwResource = "";
        $scope.objTooltipDetails.Path = "";
        $scope.objTooltipDetails.sfwEntity = "";
        $scope.objTooltipDetails.sfwMethodName = "";
        $scope.objTooltipDetails.isFinsihDisable();
        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "Tooltip") {
            $scope.objTooltipDetails.sfwEntity = $scope.SelectedEntity;
            $scope.objTooltipDetails.PopulateFormName();
        }

    };

    $scope.objTooltipDetails.PopulateFormName = function () {
        $scope.objTooltipDetails.ID = "";
        if ($scope.objTooltipDetails.sfwEntity != undefined && $scope.objTooltipDetails.sfwEntity != "") {
            $scope.objTooltipDetails.ID = $scope.objTooltipDetails.sfwEntity.substring(3);
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.objTooltipDetails.sfwEntity; });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objTooltipDetails.Path = strPath;
                }

            }
        }
        $scope.checkAndExecuteLocationCallback($scope.objTooltipDetails);
    };



    $scope.objTooltipDetails.ValidateTooltip = function () {
        var retValue = true;
        $scope.objTooltipDetails.ErrorMessageForDisplay = "";
        if ($scope.objTooltipDetails.ID == undefined || $scope.objTooltipDetails.ID == "") {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Please enter a Tooltip form ID.";
            return false;
        }
        else if (!isValidFileID($scope.objTooltipDetails.ID, true)) {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
            return false;
        }
        else if (($scope.objTooltipDetails.ID != undefined || $scope.objTooltipDetails.ID != "") && $scope.objTooltipDetails.ID.length <= 3) {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            return false;
        }

        else if ($scope.objTooltipDetails.ID && $scope.objTooltipDetails.ID.toLowerCase().startsWith("wfm")) {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: ID can not start with wfm.";
            return false;
        } else if ($scope.objTooltipDetails.ID && $scope.objTooltipDetails.ID.toLowerCase().endsWith("tooltip")) {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: ID can not ends with tooltip.";
            return false;
        } else {
            var tooltipId = "wfm" + $scope.objTooltipDetails.ID + "Tooltip";
            var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == tooltipId.toLowerCase(); });
            if (lst.length > 0) {
                $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: The File Name " + tooltipId + " already exists.\nPlease enter a different tooltip form ID.";
                return false;
            }
        }
        //if (!$scope.objTooltipDetails.sfwResource || $scope.objTooltipDetails.sfwResource == '') {
        //    $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Please enter a Resource.";
        //    return false;
        //} else if ($scope.resourceList.indexOf($scope.objTooltipDetails.sfwResource.trim()) <= -1) {
        //    $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: invalid Resource id.";
        //    return false;
        //}
        if (!$scope.ValidatePath($scope.objTooltipDetails, $scope.objTooltipDetails.Path)) {
            return false;
        }
        if (!$scope.objTooltipDetails.sfwEntity || $scope.objTooltipDetails.sfwEntity == '') {
            $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Please enter a Entity.";
            return false;
        } else if ($scope.objTooltipDetails.sfwEntity && $scope.objTooltipDetails.sfwEntity != '') {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objTooltipDetails.sfwEntity; })) {
                $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Invalid Entity.";
                return false;
            }
        }
        //else if ($scope.objTooltipDetails.sfwXmlMethod == undefined || $scope.objTooltipDetails.sfwXmlMethod == '') {
        //    $scope.objTooltipDetails.ErrorMessageForDisplay = "Error: Please select a Xml Method.";
        //    return false;
        //}
        return retValue;
    };
    $scope.objTooltipDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objTooltipDetails.ErrorMessageForDisplay = "";

        if (!$scope.objTooltipDetails.ValidateTooltip()) {
            IsValid = false;
        }
        $scope.objTooltipDetails.isToolTipDetailsFinishDisable = IsValid;
        return IsValid;
    };

    $scope.objTooltipDetails.UpdateData = function () {
        $scope.objTooltipDetails.updateDetails();
        //$scope.objTooltipDetails.UpdateInitialLoad();
        $scope.objTooltipDetails.UpdateCenterMiddle();
    };

    $scope.objTooltipDetails.updateDetails = function () {
        var strTooltipID = 'wfm' + $scope.objTooltipDetails.ID + "Tooltip";
        $scope.objTooltipForm = { Name: 'form', Value: '', dictAttributes: {}, Elements: [], Children: [] };
        $scope.objTooltipForm.dictAttributes.ID = strTooltipID;
        $scope.objTooltipForm.dictAttributes.sfwResource = $scope.objTooltipDetails.sfwResource;
        $scope.objTooltipForm.dictAttributes.sfwEntity = $scope.objTooltipDetails.sfwEntity;
        $scope.objTooltipForm.dictAttributes.sfwActive = 'True';
        $scope.objTooltipForm.dictAttributes.sfwStatus = "Review";
        $scope.objTooltipForm.dictAttributes.sfwType = 'Tooltip';
        $scope.objTooltipForm.dictAttributes.sfwTitle = strTooltipID;
    };

    $scope.objTooltipDetails.UpdateInitialLoad = function () {
        var objInitializeLoad = {
            Name: 'initialload', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        var callMethodsModel = {
            Name: 'callmethods', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        if ($scope.objTooltipDetails.sfwMethodName) {
            callMethodsModel.dictAttributes.sfwMethodName = $scope.objTooltipDetails.sfwMethodName;
        }
        objInitializeLoad.Elements.push(callMethodsModel);
        $scope.objTooltipForm.Elements.push(objInitializeLoad);
    };

    $scope.objTooltipDetails.UpdateCenterMiddle = function () {

        var sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var objMainPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        objMainPanel.dictAttributes.ID = "pnlMainPanel";

        var newRow;
        var newCell;
        var objSfxTable;

        //#region Main Panel

        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };


        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objMainPanel.Elements.push(objSfxTable);

        //#endregion
        MainCell.Elements.push(objMainPanel);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objTooltipForm.Elements.push(sfxMainTable);

    };

    // #endregion End of tooltip form implementation

    //#region Form disable options
    $scope.objFormsDetails.CanShowFormsSteps = function () {

        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "Forms") {
            retVal = false;
        }
        return retVal;
    };


    //#endregion

    //#region Next Click
    $scope.objFormsDetails.UpdateOnFormDetailsNext = function () {
        var isNewEntity = false;
        var isNewMainQuery = false;
        var isNextDisable = false;
        $scope.objFormsDetails.ShowLookupStep();
        //#region creating Basemodel for GridView

        if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
            $scope.UpdateData();
            if (!$scope.objFormsDetails.PrevEntityName || $scope.objFormsDetails.PrevEntityName != $scope.objFormsDetails.sfwEntity) {
                $scope.objFormsDetails.PrevEntityName = $scope.objFormsDetails.sfwEntity;
                isNewEntity = true;
            }

            if ($scope.objFormsDetails.IsShowMainQuery && (!$scope.objFormsDetails.PrevMainQuery || $scope.objFormsDetails.PrevMainQuery != $scope.objFormsDetails.MainQueryID)) {
                $scope.objFormsDetails.PrevMainQuery = $scope.objFormsDetails.MainQueryID;
                isNewMainQuery = true;
            }

            if ($scope.objFormsDetails.IsShowMainQuery && $scope.objFormsDetails.MainQueryID) {
                if ($scope.objFormsDetails.IsShowMainQuery && $scope.objFormsDetails.MainQueryID && isNewMainQuery) {
                    $scope.UpdateInitialLoad();
                    $.connection.hubForm.server.getEntityQueryColumns($scope.objFormsDetails.MainQueryID, "").done(function (data) {
                        $scope.receiveQueryColumnsForLookup(data);
                    });
                }
            }
            $scope.objGridView = {
                Name: "sfwGridView", prefix: "swc", Value: '', dictAttributes: { ID: 'dgrResult', AllowPaging: 'True', AllowSorting: 'True', sfwSelection: 'Many' }, Elements: [], Children: []
            };
        }
        if ($scope.objFormsDetails.IsMaintenence && $scope.objFormsDetails.IsMaintenence.toLowerCase() == "true") {
            $scope.UpdateDataForMaintenance();
        }

        //#endregion
        // call to get entity Model
        if (($scope.objFormsDetails.sfwEntity && isNewEntity) || ($scope.objFormsDetails.IsShowMainQuery && $scope.objFormsDetails.MainQueryID && isNewMainQuery)) {
            $scope.objLookupFieldsDetails.lstFieldsProperties = [];
            $scope.objFormsDetails.SfwGridCollection = "";
            $scope.objLookupResultFields.lstResultFields = [];
            $scope.objLookupFieldsDetails.lstselectedmultiplelevelfield = [];
        }
        return isNextDisable;
    };

    $scope.receiveQueryColumnsForLookup = function (data) {
        $scope.objLookupFieldsDetails.lstQueryColumn = [];
        $scope.$apply(function () {
            $scope.objLookupFieldsDetails.lstQueryColumn = data;
        });
    };
    //#endregion

    //#region Next disable Functionality

    $scope.objFormsDetails.PopulateFormName = function () {
        $scope.objFormsDetails.ID = "";
        $scope.objFormsDetails.IsShowMainQuery = false;
        if ($scope.objFormsDetails.sfwEntity != undefined && $scope.objFormsDetails.sfwEntity != "") {
            $scope.objFormsDetails.ID = $scope.objFormsDetails.sfwEntity.substring(3);
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.objFormsDetails.sfwEntity; });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objFormsDetails.Path = strPath;
                }
                if (!entities[0].TableName) {
                    $scope.checkShowMainQuery($scope.objFormsDetails, $scope.objFormsDetails.sfwEntity);
                    //$scope.objFormsDetails.IsShowMainQuery = true;
                }
            }
        }
        $scope.checkAndExecuteLocationCallback($scope.objFormsDetails);
    };
    $scope.checkShowMainQuery = function (aobjDetails, astrEntityName) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var larrEntity = [];
        var lstrParentEntity = astrEntityName;
        var lblnTableFound = false;
        while (lstrParentEntity) {
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == lstrParentEntity; });
            if (entities && entities.length > 0) {
                if (entities[0].TableName) {
                    lblnTableFound = true;
                    break;
                }
                else if (entities[0].ParentId && larrEntity.indexOf(entities[0].ParentId) === -1) {
                    lstrParentEntity = entities[0].ParentId;
                }
                else {
                    lstrParentEntity = "";
                }
            }
            else {
                lstrParentEntity = 0;
            }
        }
        if (!lblnTableFound) {
            aobjDetails.IsShowMainQuery = true;
        }
    }
    $scope.objFormsDetails.isNextDisable = function (QueryID) {
        $scope.objFormsDetails.isFormDetailsNextDisable = true;
        var IsValid = true;
        $scope.objFormsDetails.ErrorMessageForDisplay = "";
        if (!$scope.objFormsDetails.ValidateEntity()) {
            IsValid = false;
        }
        else if (!$scope.objFormsDetails.ValidateFormName()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objFormsDetails, $scope.objFormsDetails.Path)) {
            IsValid = false;
        }
        else if (!$scope.objFormsDetails.ValidateFormObject()) {
            IsValid = false;
        }
        else if (!$scope.objFormsDetails.ValidateMainQuery(QueryID)) {
            IsValid = false;
        }
        $scope.objFormsDetails.isFormDetailsNextDisable = IsValid;
        $scope.objFormsDetails.ShowLookupStep();
        return IsValid;
    };
    //#endregion

    //#region Validate Data for Common Form Details
    $scope.objFormsDetails.ValidateFormObject = function () {
        var retValue = true;
        if (($scope.objFormsDetails.IsLookup == undefined || $scope.objFormsDetails.IsLookup.toLowerCase() == "false") && ($scope.objFormsDetails.IsMaintenence == undefined || $scope.objFormsDetails.IsMaintenence.toLowerCase() == "false")) {
            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Please Select atleast one Form object.";
            retValue = false;
        }

        return retValue;
    };


    //#endregion

    //#region Validate Path for Common Form Details

    //#endregion

    //#region Validate Entity  for Common Form Details
    $scope.objFormsDetails.ValidateEntity = function () {
        var isValid = true;
        if ($scope.objFormsDetails.sfwEntity == undefined || $scope.objFormsDetails.sfwEntity == "") {
            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Entity cannot be empty.";
            isValid = false;
        }

        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objFormsDetails.sfwEntity; })) {
                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: The entity is not valid. Please enter a different Name (ID).";
                isValid = false;

            }
        }
        return isValid;
    };

    //#endregion

    //#region Form Name Validation
    $scope.objFormsDetails.ValidateFormName = function () {
        var retValue = true;
        if ($scope.objFormsDetails.ID == undefined || $scope.objFormsDetails.ID == "") {
            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Please enter a Form Name (ID).";
            retValue = false;
        }
        else if ($scope.objFormsDetails.ID && $scope.objFormsDetails.ID.toLowerCase().startsWith("wfm")) {
            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: ID can not start with wfm.";
            retValue = false;
        }

        else if (($scope.objFormsDetails.ID != undefined || $scope.objFormsDetails.ID != "") && $scope.objFormsDetails.ID.length <= 3) {
            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }
        else if ($scope.objFormsDetails.ID && ($scope.objFormsDetails.ID.toLowerCase().endsWith("lookup") || $scope.objFormsDetails.ID.toLowerCase().endsWith("maintenance"))) {
            if ($scope.objFormsDetails.ID.toLowerCase().endsWith("maintenance")) {
                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: ID can not ends with maintenance.";
            } else {
                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: ID can not ends with lookup.";
            }
            retValue = false;
        }
        else {
            if (!isValidFileID($scope.objFormsDetails.ID, true)) {
                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
                retValue = false;
            }
            if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {

                var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "lookup" || x.FileID.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "lookup"; });
                if (lst.length > 0) {
                    $scope.objFormsDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                    retValue = false;
                }
                if ($scope.objFormsDetails.IsMaintenence && $scope.objFormsDetails.IsMaintenence.toLowerCase() == "true") {
                    var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "maintenance" || x.FileID.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "maintenance"; });
                    if (lst.length > 0) {
                        $scope.objFormsDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                        retValue = false;
                    }
                }
            }
            else {
                var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "maintenance" || x.FileID.toLowerCase() == "wfm" + $scope.objFormsDetails.ID.toLowerCase() + "maintenance"; });
                if (lst.length > 0) {
                    $scope.objFormsDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                    retValue = false;
                }
            }

        }
        return retValue;
    };

    //#endregion

    //#region Validate Main Query

    $scope.objFormsDetails.ValidateMainQuery = function (QueryID) {
        var retValue = true;
        if (!QueryID) {
            QueryID = $scope.objFormsDetails.MainQueryID;
        }
        if ($scope.objFormsDetails.IsShowMainQuery && $scope.objFormsDetails.IsLookup == "True") {
            if (QueryID == undefined || QueryID == "") {
                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Please select a Lookup Query.";
                retValue = false;
            }
            else if (QueryID) {
                if (!QueryID.contains(".")) {
                    $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                    retValue = false;
                }
                else {
                    var queryId = QueryID;
                    var lst = queryId.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            var lobjquery = objEntity.Queries.filter(function (x) { return x.ID == strQueryID; });
                            if (lobjquery && lobjquery.length > 0) {
                                if (lobjquery[0].Parameters.length > 0) {
                                    $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Invalid Query.Parameterised query is not allowed.";
                                    retValue = false;
                                }
                            }
                            else {
                                $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                                retValue = false;
                            }
                        }
                        else {
                            $scope.objFormsDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                            retValue = false;
                        }
                    }
                }
            }
        }
        return retValue;
    };
    //#endregion

    //#region Lookup

    //#region Update InitialLoad for Next (Lookup)(Second Screen)
    $scope.UpdateInitialLoad = function () {

        if ($scope.objFormsDetails.objSfxLookupForm && !$scope.objFormsDetails.objSfxLookupForm.Elements.some(function (x) { return x.Name == "initialload"; })) {
            $scope.objFormsDetails.objInitializeLoad = {
                Name: 'initialload', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            if ($scope.objFormsDetails.objSfxLookupForm) {
                $scope.objFormsDetails.objSfxLookupForm.Elements.push($scope.objFormsDetails.objInitializeLoad);
            }
        }
        else {
            if ($scope.objFormsDetails.objInitializeLoad) {
                $scope.objFormsDetails.objInitializeLoad.Elements = [];
            }
        }
        if ($scope.objFormsDetails.objInitializeLoad) {
            $scope.objFormsDetails.objInitializeLoad.Elements = [];
        }

        if ($scope.objFormsDetails.MainQueryID) {
            $scope.objFormsDetails.objLink = {
                Name: 'query', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
            };

            if ($scope.objFormsDetails.MainQueryID.length > 0) {

                $scope.objFormsDetails.objLink.dictAttributes.ID = $scope.objFormsDetails.MainQueryID.substring(0, $scope.objFormsDetails.MainQueryID.indexOf("."));
                $scope.objFormsDetails.objLink.dictAttributes.sfwQueryRef = $scope.objFormsDetails.MainQueryID;
            }
            $scope.objFormsDetails.objInitializeLoad.Elements.push($scope.objFormsDetails.objLink);
        }
    };
    //#endregion

    //#region functionality for  Lookup to hide n show depending upon the option
    $scope.objFormsDetails.ShowLookupStep = function () {

        var retValue = true;
        if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
            retValue = false;
        }
        $scope.objFormsDetails.isShowLookupStep = retValue;
        return retValue;
    };

    //#endregion

    //#endregion

    //#region Maintenance


    //#region Maintenance Session Fields section
    $scope.IsLookupSelected = function () {
        var retValue = false;
        if ($scope.objFormsDetails.IsLookup && $scope.objFormsDetails.IsLookup.toLowerCase() == "true") {
            retValue = true;
        }
        if (!retValue) {
            if ($scope.showExtraFieldsTab) {//If Having Extra Fields steps
                retValue = true;
            }
        }
        $scope.isFormDetailsLookUpSelected = retValue;
        return retValue;
    };

    //#endregion


    //#region SetTab(New and Update method)

    $scope.tab = 1;
    $scope.setTab = function (newTab) {

        $scope.tab = newTab;
    };
    $scope.isSet = function (tabNum) {
        return $scope.tab === tabNum;
    };
    //#endregion


    //#endregion    

    //#region Create Form for Maintenance

    //#region for Creation Of Maintenance

    //#region Update data for Next from FormDetails (Maintenance)(First Screen)
    $scope.UpdateDataForMaintenance = function () {

        var strFormName = "wfm" + $scope.objFormsDetails.ID + "Maintenance";
        var title = GetFormTitle('Maintenance', $scope.objFormsDetails.ID);
        $scope.objFormsDetails.objSfxMaintenanceForm = {
            Name: "form", Value: '', dictAttributes: {
                ID: strFormName, sfwType: 'Maintenance',
                sfwEntity: $scope.objFormsDetails.sfwEntity, sfwTitle: title, sfwActive: 'True', sfwStatus: 'Review',
                sfwAspNetCaching: 'False', sfwMaxCount: '', sfwMenuItemID: "wfmDefaultLookup.aspx?FormID=" + strFormName.replace("Maintenance", "Lookup")
            }, Elements: [], Children: []
        };
    };

    //#endregion

    //#region Session Fields (third screen)

    $scope.UpdateCenterMiddleForMaintenance = function () {
        var MainCell;
        var MainRow;
        var objSfxPanel;
        var objSfxTable;
        var newRow;
        var newCell;
        var newControl;

        var sfxMainTable = $scope.objFormsDetails.objSfxMaintenanceForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (sfxMainTable && sfxMainTable.length > 0) {
            var index = $scope.objFormsDetails.objSfxMaintenanceForm.Elements.indexOf(sfxMainTable);
            if (index > -1) {
                $scope.objFormsDetails.objSfxMaintenanceForm.Elements.splice(index, 1);
            }
        }

        sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        // toolbar Panel

        objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel.dictAttributes.ID = "pnltoolbar";
        objSfxTable.dictAttributes.ID = "toolbar";
        objSfxTable.dictAttributes.CellPadding = "3";


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newControl.dictAttributes.ID = "btnSave";
        newControl.dictAttributes.sfwMethodName = "btnSave_Click";
        newControl.dictAttributes.Text = "Save";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnCancel";
        newControl.dictAttributes.sfwMethodName = "btnCancel_Click";
        newControl.dictAttributes.Text = "Refresh";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnPrev";
        newControl.dictAttributes.sfwMethodName = "btnPrev_Click";
        newControl.dictAttributes.Text = "Prev";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnNext";
        newControl.dictAttributes.sfwMethodName = "btnNext_Click";
        newControl.dictAttributes.Text = "Next";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnCorrespondence";
        newControl.dictAttributes.sfwMethodName = "btnCorrespondence_Click";
        newControl.dictAttributes.Text = "Correspondence";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);
        MainCell.Elements.push(objSfxPanel);
        objSfxTable.Elements.push(newRow);
        objSfxPanel.Elements.push(objSfxTable);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);


        // tblMain Panel

        MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        objSfxPanel.dictAttributes.ID = "pnlMain";
        objSfxTable.dictAttributes.ID = "tblMain";
        objSfxTable.dictAttributes.CssClass = "Table";

        var strCaption = $scope.objFormsDetails.objSfxMaintenanceForm.dictAttributes.ID;
        strCaption = strCaption.substring(0, strCaption.indexOf("Maintenance"));
        strCaption = strCaption.substring(3);
        strCaption = strCaption + " Details";

        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var strKeyFields = GetTableKeyFields($scope.objFormsDetails.sfwEntity, entityIntellisenseList);
        var strFieldName = strKeyFields;
        if (strKeyFields && strKeyFields.length > 0 && strKeyFields[0].contains('.')) {
            strFieldName = strFieldName.substring(strFieldName.lastIndexOf('.') + 1);
        }

        var newLabelControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newLabelControl.dictAttributes.ID = CreateControlIDWithPrefix("", strFieldName, "sfwLabel", false);
        newLabelControl.dictAttributes.sfwEntityField = strKeyFields;

        newCell.Elements.push(newLabelControl);

        newRow.Elements.push(newCell);

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);

        objSfxTable.Elements.push(newRow);

        objSfxPanel.Elements.push(objSfxTable);

        MainRow.Elements.push(MainCell);
        MainCell.Elements.push(objSfxPanel);
        sfxMainTable.Elements.push(MainRow);

        // tblAuditInfo Panel


        MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        objSfxPanel.dictAttributes.ID = "pnlAuditInfo";
        objSfxTable.dictAttributes.ID = "tblAuditInfo";
        objSfxTable.dictAttributes.CssClass = 'Table';
        objSfxPanel.dictAttributes.sfwCaption = "Audit Information";

        var strObjectField = null;


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        // Create

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "capCreate";
        newControl.dictAttributes.Text = "Created : ";
        newControl.dictAttributes.sfwIsCaption = "True";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblCreatedBy";
        newControl.dictAttributes.sfwEntityField = "CreatedBy";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        // Update Sequence


        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblUpdateSeq";
        newControl.dictAttributes.sfwEntityField = "UpdateSeq";
        newControl.dictAttributes.sfwTwoWayBinding = "True";
        newControl.dictAttributes.Visible = "False";
        newCell.Elements.push(newControl);



        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblCreatedDate";
        newControl.dictAttributes.sfwEntityField = "CreatedDate";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        // Modify

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "capModify";
        newControl.dictAttributes.Text = "Modified : ";

        newControl.dictAttributes.sfwIsCaption = "True";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblModifiedBy";
        newControl.dictAttributes.sfwEntityField = "ModifiedBy";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblModifiedDate";
        newControl.dictAttributes.sfwEntityField = "ModifiedDate";


        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        objSfxTable.Elements.push(newRow);
        objSfxPanel.Elements.push(objSfxTable);

        MainCell.Elements.push(objSfxPanel);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objFormsDetails.objSfxMaintenanceForm.Elements.push(sfxMainTable);

    };
    //#endregion

    //#endregion


    //#endregion

    //#region Available Fields

    //#region Add Available Fields
    $scope.objLookupFieldsDetails.addAvailableFields = function () {
        function iterator(item) {
            if (item.FieldObject)
                if (item.FieldObject.IsSelected && item.FieldObject.IsSelected == "True") {
                    if (!$scope.objLookupFieldsDetails.lstFieldsProperties.some(function (x) { return x.ID == item.FieldObject.Value; })) {
                        var field = { ID: item.FieldObject.Value };
                        field.IsRangeVisible = false;
                        field.DataType = item.FieldObject.DataType;
                        if (item.FieldObject.Value.indexOf("_value") > -1) {
                            field.ControlClass = "DropDownList";
                            field.ControlName = "ddl" + CreateControlIDInCamelCase(item.FieldObject.Value);
                            if (item.FieldObject.Value.indexOf("_value") > -1) {
                                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                field.CodeGroup = GetCodeIDByValue($scope.objFormsDetails.sfwEntity, item.FieldObject.Value, entityIntellisenseList);
                            }
                            else {
                                field.CodeGroup = "";
                            }
                            field.Operator = "=";
                        }
                        else {
                            field.ControlClass = "TextBox";
                            field.ControlName = "txt" + CreateControlIDInCamelCase(item.FieldObject.Value);
                        }
                        if (item.FieldObject.DataType.toLowerCase() == "date" || item.FieldObject.DataType.toLowerCase() == "datetime" || item.FieldObject.DataType.toLowerCase() == "decimal" ||
                            item.FieldObject.DataType.toLowerCase() == "double" || item.FieldObject.DataType.toLowerCase() == "int") {
                            field.IsRangeVisible = true;
                        }

                        if (item.FieldObject.Caption && item.FieldObject.Caption.trim().indexOf(":") == item.FieldObject.Caption.trim().length - 1) { // if caption alreday has colon (:)
                            field.strCaption = GetCaptionFromField(item.FieldObject);
                        }
                        else {
                            field.strCaption = GetCaptionFromField(item.FieldObject) + " : ";
                        }


                        $scope.objLookupFieldsDetails.lstFieldsProperties.push(field);
                    }
                    item.FieldObject.IsSelected = "False";
                }
        }

        function iChecklstQueryColumnSelected(item) {
            if (item.IsSelected && item.IsSelected == true) {
                if (!$scope.objLookupFieldsDetails.lstFieldsProperties.some(function (x) { return x.CodeID == item.CodeID; })) {
                    item.ID = item.CodeID;
                    if (QueryID && item.CodeID && item.CodeID.indexOf("_value") > -1) {
                        item.ControlClass = "DropDownList";
                        item.ControlName = "ddl" + CreateControlIDInCamelCase(item.CodeID);
                        if (item.CodeID.indexOf("_value") > -1) {
                            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                            item.CodeGroup = GetCodeIDByValue(QueryID, item.CodeID, entityIntellisenseList);
                        }
                        else {
                            item.CodeGroup = "";
                        }
                        item.Operator = "=";
                    }
                    else {
                        item.ControlClass = "TextBox";
                        item.ControlName = "txt" + CreateControlIDInCamelCase(item.CodeID);
                    }
                    if (item.DataType.toLowerCase() == "date" || item.DataType.toLowerCase() == "datetime" || item.DataType.toLowerCase() == "decimal" ||
                        item.DataType.toLowerCase() == "double" || item.DataType.toLowerCase() == "int") {
                        item.IsRangeVisible = true;
                    }
                    if (item.CodeID.Caption && item.CodeID.Caption.trim().indexOf(":") == item.CodeID.Caption.trim().length - 1) { // if caption alreday has colon (:)
                        item.strCaption = GetCaptionFromFieldName(item.CodeID);
                    }
                    else {
                        item.strCaption = GetCaptionFromFieldName(item.CodeID) + " : ";
                    }
                    $scope.objLookupFieldsDetails.lstFieldsProperties.push(item);
                }
                item.IsSelected = false;
            }
        }

        if ($scope.objFormsDetails.IsShowMainQuery) {
            var QueryID = $scope.objFormsDetails.MainQueryID.split('.')[0];

            angular.forEach($scope.objLookupFieldsDetails.lstQueryColumn, iChecklstQueryColumnSelected);
        }
        else {
            angular.forEach($scope.objLookupFieldsDetails.lstselectedmultiplelevelfield, iterator);
            $scope.objLookupFieldsDetails.lstselectedmultiplelevelfield = [];
        }
    };
    //#endregion

    $scope.objLookupFieldsDetails.getAvailableFields = function (obj, event) {
        if (event) {
            event.stopPropagation();
            event.stopImmediatePropagation();
        }
        if (obj) {
            $scope.objLookupFieldsDetails.SelectedAvailableFields = obj;
        }
    };

    //#region Next Button Functionality

    $scope.objLookupFieldsDetails.UpdateOnFormDetailsNext = function () {
        var isNextDisable = false;
        $scope.UpdateCenterMiddle();
        $scope.objLookupResultFields.LoadData();
        //$scope.objLookupFieldsDetails.LoadGridDataForFieldProperties();
        return isNextDisable;
    };
    //#endregion

    //#endregion

    //#region Fields Properties


    $scope.objLookupFieldsDetails.SelectedFieldPropRow = function (obj) {
        if (obj) {
            $scope.objLookupFieldsDetails.SelectedFieldRow = obj;
        }
    };

    //#region  Move up for Fields Properties

    $scope.objLookupFieldsDetails.moveSelectedRowUp = function () {
        if ($scope.objLookupFieldsDetails.SelectedFieldRow) {
            var index = $scope.objLookupFieldsDetails.lstFieldsProperties.indexOf($scope.objLookupFieldsDetails.SelectedFieldRow);
            var item = $scope.objLookupFieldsDetails.lstFieldsProperties[index - 1];
            $scope.objLookupFieldsDetails.lstFieldsProperties[index - 1] = $scope.objLookupFieldsDetails.SelectedFieldRow;
            $scope.objLookupFieldsDetails.lstFieldsProperties[index] = item;
            $scope.scrollBySelectedField(".lookup-field-details-search-table", ".selected");
        }
    };

    // disable the move up button if there is no element to move up
    $scope.objLookupFieldsDetails.canmoveSelectedRowUp = function () {
        $scope.Flag = true;
        if ($scope.objLookupFieldsDetails.SelectedFieldRow) {
            for (var i = 0; i < $scope.objLookupFieldsDetails.lstFieldsProperties.length; i++) {
                if ($scope.objLookupFieldsDetails.lstFieldsProperties[i] == $scope.objLookupFieldsDetails.SelectedFieldRow) {
                    if (i > 0) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };
    //#endregion

    //#region  Move Down for Fields Properties
    $scope.scrollBySelectedField = function (parentDiv, selectedElement) {
        $timeout(function () {
            var $divDom = $(parentDiv);
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 300, offsetLeft: 0 }, null);
                return false;
            }

        });
    }

    $scope.objLookupFieldsDetails.moveSelectedRowDown = function () {
        if ($scope.objLookupFieldsDetails.SelectedFieldRow) {
            var index = $scope.objLookupFieldsDetails.lstFieldsProperties.indexOf($scope.objLookupFieldsDetails.SelectedFieldRow);
            var item = $scope.objLookupFieldsDetails.lstFieldsProperties[index + 1];
            $scope.objLookupFieldsDetails.lstFieldsProperties[index + 1] = $scope.objLookupFieldsDetails.SelectedFieldRow;
            $scope.objLookupFieldsDetails.lstFieldsProperties[index] = item;
            $scope.scrollBySelectedField(".lookup-field-details-search-table", ".selected");
        }
    };

    $scope.objLookupFieldsDetails.canmoveSelectedRowDown = function () {
        $scope.Flag = true;
        if ($scope.objLookupFieldsDetails.SelectedFieldRow) {
            for (var i = 0; i < $scope.objLookupFieldsDetails.lstFieldsProperties.length; i++) {
                if ($scope.objLookupFieldsDetails.lstFieldsProperties[i] == $scope.objLookupFieldsDetails.SelectedFieldRow) {
                    if (i < $scope.objLookupFieldsDetails.lstFieldsProperties.length - 1) {
                        $scope.Flag = false;
                    }
                }
            }
        }

        return $scope.Flag;
    };
    //#endregion

    $scope.objLookupFieldsDetails.deleteSelectedRow = function () {
        var index = $scope.objLookupFieldsDetails.lstFieldsProperties.indexOf($scope.objLookupFieldsDetails.SelectedFieldRow);
        if (index >= 0) {
            $scope.objLookupFieldsDetails.lstFieldsProperties.splice(index, 1);
            if (index < $scope.objLookupFieldsDetails.lstFieldsProperties.length) {
                $scope.objLookupFieldsDetails.SelectedFieldRow = $scope.objLookupFieldsDetails.lstFieldsProperties[index];
            }
            else if ($scope.objLookupFieldsDetails.lstFieldsProperties.length > 0) {
                $scope.objLookupFieldsDetails.SelectedFieldRow = $scope.objLookupFieldsDetails.lstFieldsProperties[index - 1];
            }
        }
    };

    //#endregion

    //#region Result Fields

    $scope.objLookupResultFields.LoadData = function () {
        function iterator(itm) {
            var attributes;
            if ($scope.objFormsDetails.IsShowMainQuery) {
                attributes = lst[0].Attributes.filter(function (x) {
                    return x.ID == itm.ID;
                });
            }
            else {
                attributes = lst[0].Attributes.filter(function (x) {
                    return x.Value == itm.ID;
                });
            }
            if (attributes && attributes.length > 0) {
                if (attributes[0].Value && attributes[0].Value.endsWith("_value")) {
                    var attrs = lst[0].Attributes.filter(function (x) {
                        return x.Value == attributes[0].Value.replace("_value", "_description");
                    });
                    if (attrs && attrs.length > 0) {
                        attributes = attrs;
                    }
                }
                if (!lstSelectedField.some(function (field) { return field.ID == attributes[0].ID; })) {
                    lstSelectedField.push(attributes[0]);
                }
            }
        }
        $scope.objLookupResultFields.preselectedfields = [];
        //$scope.objLookupResultFields.lstResultFieldsForGrid = [];
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var lst = entityIntellisenseList.filter(function (x) {
            return x.ID == $scope.objFormsDetails.sfwEntity;
        });
        if (lst && lst.length > 0) {
            if (lst[0].Attributes.length > 0) {
                var primarykeyattribute = lst[0].Attributes.filter(function (x) {
                    return x.Type == "Column" && x.KeyNo == '1';
                });
                if (primarykeyattribute.length > 0) {
                    primarykeyattribute[0].EntityName = lst[0].ID;
                    $scope.objLookupResultFields.preselectedfields.push(primarykeyattribute[0]);
                    $scope.objLookupResultFields.LoadGridDataForResultFields($scope.objLookupResultFields.preselectedfields, true);
                }
            }

            var lstSelectedField = [];
            if ($scope.objLookupFieldsDetails.lstFieldsProperties && $scope.objLookupFieldsDetails.lstFieldsProperties.length > 0) {
                angular.forEach($scope.objLookupFieldsDetails.lstFieldsProperties, iterator);
                $scope.objLookupResultFields.LoadGridDataForResultFields(lstSelectedField, true);

                if (!$scope.objFormsDetails.IsShowMainQuery) {
                    var dummyList = [];
                    for (var i = 0; i < $scope.objLookupResultFields.lstResultFieldsForGrid.length > 0; i++) {
                        function some(itm) {
                            return itm.ID == $scope.objLookupResultFields.lstResultFieldsForGrid[i].strFieldName;
                        }
                        if (!lstSelectedField.some(some)) {
                            if (!$scope.objLookupResultFields.preselectedfields.some(some)) {
                                if (!$scope.objLookupResultFields.lstResultFieldsForGrid[i].IsAddFromResultFieldScreen) {
                                    $scope.objLookupResultFields.lstResultFieldsForGrid.splice(i, 1);
                                    i--;
                                }
                                //dummyList.push($scope.objLookupResultFields.lstResultFieldsForGrid[i]);
                            }
                        }
                    }
                }
            }
        }

        $scope.LoadSort();
        $scope.LoadOrder();
    };

    /*changed coz of Bug 9282:*/
    $scope.LoadSort = function () {
        $scope.ArrSort = [];
        $scope.ArrSort.push("");
        if ($scope.objLookupResultFields.lstResultFieldsForGrid.length > 0) {
            for (var i = 0; i < $scope.objLookupResultFields.lstResultFieldsForGrid.length; i++) {
                $scope.ArrSort.push(i + 1);
            }
        }

    };

    $scope.LoadOrder = function () {
        $scope.ArrOrder = [];
        $scope.ArrOrder.push("");
        $scope.ArrOrder.push("asc");
        $scope.ArrOrder.push("desc");
    };

    $scope.objLookupResultFields.addResultFields = function () {
        $scope.objLookupResultFields.LoadGridDataForResultFields($scope.objLookupResultFields.lstselectedmultiplelevelfield);
        if ($scope.objLookupResultFields.lstResultFieldsForGrid && $scope.objLookupResultFields.lstResultFieldsForGrid.length > 0) {
            $scope.objLookupResultFields.SelectedResultGridRow($scope.objLookupResultFields.lstResultFieldsForGrid[$scope.objLookupResultFields.lstResultFieldsForGrid.length - 1]);
            $scope.objLookupResultFields.lstselectedmultiplelevelfield = [];
            $scope.LoadSort();
        }
    };

    //#region for Next Button Functionality
    $scope.objLookupResultFields.UpdateOnFormDetailsNext = function () {
        var isNextDisable = false;

        return isNextDisable;
    };
    //#endregion

    //#region for Load Grid Data for Result Grid

    $scope.objLookupResultFields.LoadGridDataForResultFields = function (ResultFieldCollection, isLoad) {
        if (!$scope.objLookupResultFields.lstResultFieldsForGrid) {
            $scope.objLookupResultFields.lstResultFieldsForGrid = [];
        }

        var intCountPrimary = 0;

        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var primaryKeyFields = getPrimarykey(entityIntellisenseList, $scope.objFormsDetails.sfwEntity);

        function iterator(itm) {
            var isFound = false;
            for (var i = 0; i < $scope.objLookupResultFields.lstResultFieldsForGrid.length; i++) {
                if ($scope.objLookupResultFields.lstResultFieldsForGrid[i].strFieldName == itm.EntityField) {
                    isFound = true;
                    break;
                }
            }
            if (!isFound) {
                var item = {
                };
                var ResultField = itm.FieldObject;
                item.strFieldName = itm.EntityField;
                if (isLoad) {
                    ResultField = itm;
                    item.strFieldName = itm.ID;
                }
                else {
                    item.IsAddFromResultFieldScreen = true;
                }
                //ResultField.ID;
                item.strDataType = ResultField.DataType;
                if (ResultField.DataType) {
                    if (ResultField.DataType.toLowerCase() == "int") {
                        item.strDataType = "int";
                    }
                    else if (ResultField.DataType.toLowerCase() == "string") {
                        item.strDataType = "string";
                    }
                }
                item.strHeaderText = ResultField.Value.substring(ResultField.Value.lastIndexOf('.') + 1);
                item.strHeaderText = GetCaptionFromField(ResultField);


                if (item.strDataType && (item.strDataType.toLowerCase() == "datetime" || item.strDataType.toLowerCase() == "date")) {
                    item.strDataFormat = '{0:d}';
                }
                else if (item.strDataType && item.strDataType.toLowerCase() == "decimal" && (ResultField.Value.indexOf("_amt") > -1 || ResultField.Value.indexOf("_amount") > -1)) {
                    item.strDataFormat = "{0:C}";
                }
                else if (ResultField.Value.contains("ssn")) {
                    item.strDataFormat = '{0:000-##-####}';
                }
                else if (ResultField.Value.contains("phone") || ResultField.Value.contains("fax")) {
                    item.strDataFormat = '{0:(###)###-####}';
                }
                else {
                    item.strDataFormat = '';
                }
                item.IsVisible = "True";
                item.strControlType = 'Label';
                if (primaryKeyFields.length > 0) {
                    var lstKey = primaryKeyFields.filter(function (x) { return x.ID.toLowerCase() == ResultField.ID.toLowerCase(); });
                    if (lstKey && lstKey.length > 0) {
                        item.strDataKey = lstKey[0].KeyNo;
                    }
                }

                if (!$scope.objLookupResultFields.lstResultFieldsForGrid.some(function (field) { return field.strFieldName == itm.ID; })) {
                    $scope.objLookupResultFields.lstResultFieldsForGrid.push(item);
                }
            }
            if (!isLoad) {
                if (!$scope.objLookupResultFields.preselectedfields.some(function (field) { return field.ID == itm.EntityField; })) {
                    if (itm.FieldObject) {
                        itm.FieldObject.IsSelected = "False";
                    }
                }
            }
        }

        if (ResultFieldCollection.length > 0) {


            angular.forEach(ResultFieldCollection, iterator);
        }
    };

    $scope.objLookupResultFields.UpdateGridProperties = function () {
        var strKeySeq;
        var strKeyNames = "";

        var strSortSeq;
        var strSortExpression = "";
        function getSortExpression(objSfxField) {
            strKeySeq = objSfxField.istrKey;
            if (strKeySeq) {
                if (strKeyNames.length == 0) {
                    strKeyNames = strKeySeq + ";" + objSfxField.strFieldName;
                }
                else {
                    strKeyNames += "," + strKeySeq + ";" + objSfxField.strFieldName;
                }
            }

            strSortSeq = objSfxField.strSort;
            if (strSortSeq) {
                if (strSortExpression.length == 0) {
                    if (!objSfxField.strOrder) {
                        strSortExpression = strSortSeq + ";" + objSfxField.strFieldName;
                    }
                    else {
                        strSortExpression = strSortSeq + ";" + objSfxField.strFieldName + " " + objSfxField.strOrder;
                    }
                }
                else {
                    if (!objSfxField.strOrder) {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.strFieldName;
                    }
                    else {
                        strSortExpression += "," + strSortSeq + ";" + objSfxField.strFieldName + " " + objSfxField.strOrder;
                    }
                }
            }
        }
        if ($scope.objLookupResultFields.lstResultFieldsForGrid.length > 0) {



            angular.forEach($scope.objLookupResultFields.lstResultFieldsForGrid, getSortExpression);


        }
        var slFieldSeq = [];
        var strDataKeyNames = strKeyNames.split(',');
        function getFieldSequence(strKeyName) {
            var strKeyField = strKeyName.substring(strKeyName.indexOf(';') + 1);
            var obj = {};
            obj.key = strKeyName;
            obj.Value = strKeyField;
            slFieldSeq.push(obj);
        }
        angular.forEach(strDataKeyNames, getFieldSequence);


        strKeyNames = "";

        // IDictionaryEnumerator ide = slFieldSeq.GetEnumerator();
        function getKeyName(key) {
            if (strKeyNames.length == 0) {
                strKeyNames = key.Value;
            }
            else {
                strKeyNames += "," + key.Value;
            }
        }
        angular.forEach(slFieldSeq, getKeyName);

        $scope.objGridView.dictAttributes.sfwDataKeyNames = strKeyNames;

        slFieldSeq = [];
        var strSortExpFields = strSortExpression.split(',');

        function iteratorSort(strSortExp) {
            var strSortField = strSortExp.substring(strSortExp.indexOf(';') + 1);
            var obj = {};
            obj.key = strSortExp;
            obj.Value = strSortField;
            slFieldSeq.push(obj);
        }

        angular.forEach(strSortExpFields, iteratorSort);


        strSortExpression = "";

        //ide = slFieldSeq.GetEnumerator();
        function iterator(key) {
            if (strSortExpression.length == 0)
                strSortExpression = key.Value;
            else
                strSortExpression += "," + key.Value;
        }
        angular.forEach(slFieldSeq, iterator);

        $scope.objGridView.dictAttributes.sfwSortExpression = strSortExpression;
    };


    //#endregion

    //#endregion

    //#region Result Grid

    //#region Click On selected Result Grid

    $scope.objLookupResultFields.SelectedResultGridRow = function (obj) {
        if (obj) {
            $scope.objLookupResultFields.SelectedCurrentResultGridRow = obj;
        }
    };

    $scope.objLookupResultFields.ChangeSortExpression = function (selectedcolumn) {
        if (selectedcolumn && !selectedcolumn.strSort) {
            selectedcolumn.strOrder = "";
        }
    };
    //#endregion


    //#region  Move up for Result Grid Row

    $scope.objLookupResultFields.moveSelectedRowUp = function () {
        if ($scope.objLookupResultFields.SelectedCurrentResultGridRow) {
            var index = $scope.objLookupResultFields.lstResultFieldsForGrid.indexOf($scope.objLookupResultFields.SelectedCurrentResultGridRow);
            var item = $scope.objLookupResultFields.lstResultFieldsForGrid[index - 1];
            $scope.objLookupResultFields.lstResultFieldsForGrid[index - 1] = $scope.objLookupResultFields.SelectedCurrentResultGridRow;
            $scope.objLookupResultFields.lstResultFieldsForGrid[index] = item;
            $scope.scrollBySelectedField("#lookup-result-grid", ".selected");
        }
    };


    $scope.objLookupResultFields.canmoveSelectedRowUp = function () {
        $scope.Flag = true;
        if ($scope.objLookupResultFields.SelectedCurrentResultGridRow) {
            for (var i = 0; i < $scope.objLookupResultFields.lstResultFieldsForGrid.length; i++) {
                if ($scope.objLookupResultFields.lstResultFieldsForGrid[i] == $scope.objLookupResultFields.SelectedCurrentResultGridRow) {
                    if (i > 0) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };
    //#endregion


    //#region Move down or Result Grid Row


    $scope.objLookupResultFields.moveSelectedRowDown = function () {
        if ($scope.objLookupResultFields.SelectedCurrentResultGridRow) {
            var index = $scope.objLookupResultFields.lstResultFieldsForGrid.indexOf($scope.objLookupResultFields.SelectedCurrentResultGridRow);
            var item = $scope.objLookupResultFields.lstResultFieldsForGrid[index + 1];
            $scope.objLookupResultFields.lstResultFieldsForGrid[index + 1] = $scope.objLookupResultFields.SelectedCurrentResultGridRow;
            $scope.objLookupResultFields.lstResultFieldsForGrid[index] = item;
            $scope.scrollBySelectedField("#lookup-result-grid", ".selected");
        }
    };

    $scope.objLookupResultFields.canmoveSelectedRowDown = function () {
        $scope.Flag = true;
        if ($scope.objLookupResultFields.SelectedCurrentResultGridRow) {
            for (var i = 0; i < $scope.objLookupResultFields.lstResultFieldsForGrid.length; i++) {
                if ($scope.objLookupResultFields.lstResultFieldsForGrid[i] == $scope.objLookupResultFields.SelectedCurrentResultGridRow) {
                    if (i < $scope.objLookupResultFields.lstResultFieldsForGrid.length - 1) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };

    $scope.objLookupResultFields.deleteSelectedRow = function () {
        if ($scope.objLookupResultFields.SelectedCurrentResultGridRow) {
            if (!$scope.objLookupResultFields.preselectedfields.some(function (itm) { return itm.ID == $scope.objLookupResultFields.SelectedCurrentResultGridRow.strFieldName; })) {
                var index = $scope.objLookupResultFields.lstResultFieldsForGrid.indexOf($scope.objLookupResultFields.SelectedCurrentResultGridRow);
                if (index >= 0) {
                    $scope.objLookupResultFields.lstResultFieldsForGrid.splice(index, 1);
                    if (index < $scope.objLookupResultFields.lstResultFieldsForGrid.length) {
                        $scope.objLookupResultFields.SelectedCurrentResultGridRow = $scope.objLookupResultFields.lstResultFieldsForGrid[index];
                    }
                    else if ($scope.objLookupResultFields.lstResultFieldsForGrid.length > 0) {
                        $scope.objLookupResultFields.SelectedCurrentResultGridRow = $scope.objLookupResultFields.lstResultFieldsForGrid[index - 1];
                    }
                    $scope.LoadSort();
                }
            }
            else {
                alert("Primary Key Can not  be deleted");
            }
        }
    };

    //#endregion

    //#endregion

    //#region Update data for Next from FormDetails (Lookup)(First Screen)
    $scope.UpdateData = function () {

        var strFormName = "wfm" + $scope.objFormsDetails.ID + "Lookup";
        var title = GetFormTitle('Lookup', $scope.objFormsDetails.ID);
        $scope.objFormsDetails.objSfxLookupForm = {
            Name: "form", Value: '', dictAttributes: {
                ID: strFormName, sfwType: 'Lookup',
                sfwEntity: $scope.objFormsDetails.sfwEntity, sfwTitle: title, sfwActive: 'True', sfwStatus: 'Review',
                sfwAspNetCaching: 'False', sfwMaxCount: '', sfwMenuItemID: "wfmDefaultLookup.aspx?FormID=" + strFormName.replace("Maintenance", "Lookup")
            }, Elements: [], Children: []
        };
    };

    //#endregion

    //#region Update data for next (Lookup Search Grid)(fourth Screen)

    $scope.UpdateCenterMiddle = function () {

        var objOldTable = $scope.objFormsDetails.objSfxLookupForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (objOldTable && objOldTable.length > 0) {
            var index = $scope.objFormsDetails.objSfxLookupForm.Elements.indexOf(objOldTable[0]);
            if (index > -1) {
                $scope.objFormsDetails.objSfxLookupForm.Elements.splice(index, 1);
            }
        }

        var sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel.dictAttributes.ID = "pnlCriteria";
        objSfxPanel.dictAttributes.sfwCaption = "Criteria";
        $scope.UpdateSearchCriteria(objSfxPanel);

        MainCell.Elements.push(objSfxPanel);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objFormsDetails.objSfxLookupForm.Elements.push(sfxMainTable);

    };

    $scope.UpdateSearchCriteria = function (sfxParent) {

        var newRow;
        var newCell;
        var newControl;
        var objSfxPanel = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWTABLE, ApplicationConstants.XMLFacade.PREFIX_SWC);
        objSfxPanel.dictAttributes.ID = "tblCriteria";

        var j = 0;
        var strFieldLabel;
        var strFieldLabelID;

        angular.forEach($scope.objLookupFieldsDetails.lstFieldsProperties, function (objCriteriaField) {
            if (j == 0) {
                // newRow = new BaseModel(objSfxPanel);
                newRow = {
                    Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWROW, ApplicationConstants.XMLFacade.PREFIX_SWC);
                objSfxPanel.Elements.push(newRow);
            }
            if (j < parseInt($scope.objLookupFieldsDetails.noOfColumns)) {
                if (objCriteriaField.strCaption) {
                    strFieldLabel = objCriteriaField.strCaption;
                }
                else {
                    strFieldLabel = GetCaptionFromFieldName(objCriteriaField.ID) + " : ";
                }
                strFieldLabelID = "cap" + CreateControlIDInCamelCase(objCriteriaField.ID);

                if (objCriteriaField.IsRangeCheck && objCriteriaField.IsRangeCheck == true) {
                    var iRemainingCol = parseInt($scope.objLookupFieldsDetails.noOfColumns) - j;
                    if (iRemainingCol < 4) {

                        newRow = {
                            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                        };//.SetName(ApplicationConstants.XMLFacade.SFWROW, ApplicationConstants.XMLFacade.PREFIX_SWC);
                        objSfxPanel.Elements.push(newRow);
                    }

                    j = j + 4;

                    $scope.AddRange(parseInt($scope.objLookupFieldsDetails.noOfColumns), objCriteriaField, newRow);
                }
                else {

                    newCell = {
                        Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

                    newControl = {
                        Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };//.SetName("sfwLabel", ApplicationConstants.XMLFacade.PREFIX_SWC);
                    newControl.dictAttributes.ID = strFieldLabelID;
                    newControl.dictAttributes.Text = strFieldLabel;
                    newControl.dictAttributes.AssociatedControlID = objCriteriaField.ControlName;
                    newControl.dictAttributes.sfwIsCaption = "True";
                    newCell.Elements.push(newControl);
                    newRow.Elements.push(newCell);
                    j++;



                    newCell = {
                        Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);
                    var name = "sfw" + objCriteriaField.ControlClass;
                    newControl = {
                        Name: "sfw" + objCriteriaField.ControlClass, prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };//.SetName(objCriteriaField.ControlClass, ApplicationConstants.XMLFacade.PREFIX_SWC);
                    newControl.dictAttributes.ID = objCriteriaField.ControlName;
                    if ($scope.objFormsDetails.IsShowMainQuery) {
                        newControl.dictAttributes.sfwQueryID = $scope.objFormsDetails.MainQueryID.split('.')[0];
                    }
                    newControl.dictAttributes.sfwDataField = objCriteriaField.ID;
                    newControl.dictAttributes.sfwOperator = objCriteriaField.Operator;
                    newControl.dictAttributes.sfwCarryOver = objCriteriaField.IsCarryOverCheck;

                    if (objCriteriaField.ControlClass == "TextBox") {
                        if (objCriteriaField.DataType.toLowerCase() == "datetime" || objCriteriaField.DataType.toLowerCase() == "date") {
                            newControl.dictAttributes.sfwDataType = "DateTime";
                        }
                        else if (objCriteriaField.DataType.toLowerCase() == "decimal" || objCriteriaField.DataType.toLowerCase().indexOf("int") > -1) {
                            newControl.dictAttributes.sfwDataType = "Numeric";
                        }
                        else {
                            newControl.dictAttributes.sfwDataType = "String";
                        }
                        if (objCriteriaField.DataType.toLowerCase() == "datetime" || objCriteriaField.DataType.toLowerCase() == "date") {
                            newControl.dictAttributes.sfwDataFormat = "{0:d}";
                        }
                        else if (objCriteriaField.DataType.toLowerCase() == "decimal" && (objCriteriaField.ID.indexOf("_amt") > -1 ||
                            objCriteriaField.ID.indexOf("_amount") > -1)) {
                            newControl.dictAttributes.sfwDataFormat = "{0:C}";
                        }
                        else if (objCriteriaField.ID.contains("ssn")) {
                            newControl.dictAttributes.sfwDataFormat = "{0:000-##-####}";
                        }
                        else if (objCriteriaField.ID.contains("phone") || objCriteriaField.ID.contains("fax")) {
                            newControl.dictAttributes.sfwDataFormat = "{0:(###)###-####}";
                        }
                    }
                    else if (objCriteriaField.ControlClass == "DropDownList" ||
                        objCriteriaField.ControlClass == "CheckBoxList") {
                        newControl.dictAttributes.sfwLoadType = "CodeGroup";
                        //newControl.dictAttributes.sfwLoadSource = objCriteriaField.CodeGroup;

                        newControl.dictAttributes.DataTextField = objCriteriaField.DataTextField;
                        newControl.dictAttributes.DataValueField = objCriteriaField.DataValueField;
                    }

                    newCell.Elements.push(newControl);
                    newRow.Elements.push(newCell);
                    j++;
                }
            }

            if (j >= parseInt($scope.objLookupFieldsDetails.noOfColumns)) {
                j = 0;
            }
        });



        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWROW, ApplicationConstants.XMLFacade.PREFIX_SWC);

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWBUTTON, ApplicationConstants.XMLFacade.PREFIX_SWC);
        newControl.dictAttributes.ID = "btnSearch";
        newControl.dictAttributes.sfwMethodName = "btnSearch_Click";
        newControl.dictAttributes.sfwOperateOn = "tblCriteria";
        newControl.dictAttributes.Text = "Search";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWBUTTON, ApplicationConstants.XMLFacade.PREFIX_SWC);
        newControl.dictAttributes.ID = "btnReset";
        newControl.dictAttributes.sfwMethodName = "btnReset_Click";
        newControl.dictAttributes.sfwOperateOn = "tblCriteria";
        newControl.dictAttributes.Text = "Reset";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.SFWBUTTON, ApplicationConstants.XMLFacade.PREFIX_SWC);
        newControl.dictAttributes.ID = "btnStoreSearch";
        newControl.dictAttributes.sfwMethodName = "btnStoreUserDefaults_Click";
        newControl.dictAttributes.sfwOperateOn = "tblCriteria";
        newControl.dictAttributes.Text = "Store Search";
        newCell.Elements.push(newControl);

        newRow.Elements.push(newCell);
        objSfxPanel.Elements.push(newRow);
        var iCellCount = 0;

        function iterator(row) {
            if (row.Elements.length > iCellCount) {
                iCellCount = row.Elements.length;
            }
        }
        angular.forEach(objSfxPanel.Elements, iterator);


        function item(row) {
            while (row.Elements.length < iCellCount) {

                var newRow1 = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);
                row.Elements.push(newRow1);
            }
        }
        angular.forEach(objSfxPanel.Elements, item);

        sfxParent.Elements.push(objSfxPanel);
    };

    $scope.AddRange = function (aCellCount, afield, sfxParent) {
        if (afield.DataType.toLowerCase() == "date" || afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "decimal" ||
            afield.DataType.toLowerCase() == "double" || afield.DataType.toLowerCase().indexOf("int") > -1) {
            var controlID = CreateControlIDInCamelCase(afield.ID);


            var j = 0;

            if (j < aCellCount) {
                j++;

                var newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

                var newCntrl = {
                    Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName("sfwLabel", ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = "cap" + controlID + "From";
                newCntrl.dictAttributes.sfwIsCaption = "True";
                if (afield.strCaption) {
                    if (afield.strCaption.trim().indexOf(":") == afield.strCaption.trim().length - 1) {
                        afield.strCaption = afield.strCaption.trim();
                        newCntrl.dictAttributes.Text = afield.strCaption.slice(0, afield.strCaption.length - 1) + " From : ";
                    }
                } else {
                    newCntrl.dictAttributes.Text = GetCaptionFromFieldName(afield.ID) + " From : ";
                }

                newCell.Elements.push(newCntrl);
                sfxParent.Elements.push(newCell);
            }

            if (j < aCellCount) {
                j++;

                var newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);


                var newCntrl = {
                    Name: 'sfwTextBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWTEXTBOX, ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = "txt" + controlID + "From";
                if ($scope.objFormsDetails.IsShowMainQuery) {
                    newCntrl.dictAttributes.sfwQueryID = $scope.objFormsDetails.MainQueryID.split('.')[0];
                }
                newCntrl.dictAttributes.sfwDataField = afield.ID;
                newCntrl.dictAttributes.sfwOperator = "between";
                newCntrl.dictAttributes.sfwRelatedControl = 'txt' + controlID + "To";

                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date") {
                    newCntrl.dictAttributes.sfwDataType = "datetime";
                    newCntrl.dictAttributes.sfwDataFormat = "{0:d}";
                }
                else
                    newCntrl.dictAttributes.sfwDataType = "numeric";

                newCell.Elements.push(newCntrl);


                var newCntrl = {
                    Name: 'CompareValidator', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.COMPAREVALIDATOR, ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = "val" + controlID + "From";
                newCntrl.dictAttributes.Text = '*';
                newCntrl.dictAttributes.Operator = "DataTypeCheck";
                newCntrl.dictAttributes.ControlToValidate = 'txt' + controlID + "From";

                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date") {
                    newCntrl.dictAttributes.Type = "Date";
                    if (afield.strCaption) {
                        newCntrl.dictAttributes.ErrorMessage = afield.strCaption + " From must be in mm/dd/yyyy format. ";
                    }
                    else {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " From must be in mm/dd/yyyy format.";
                    }
                }
                else {
                    newCntrl.dictAttributes.Type = "Integer";
                    if (afield.strCaption) {
                        newCntrl.dictAttributes.ErrorMessage = afield.strCaption + " From must be an Integer value.";
                    }
                    else {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " From must be an Integer value.";
                    }
                }

                newCell.Elements.push(newCntrl);
                sfxParent.Elements.push(newCell);
            }


            if (j < aCellCount) {
                j++;

                var newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

                var newCntrl = {
                    Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWLABEL, ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = 'cap' + controlID + "To";
                newCntrl.dictAttributes.sfwIsCaption = "True";
                if (afield.strCaption) {
                    if (afield.strCaption.trim().indexOf(":") == afield.strCaption.trim().length - 1) {
                        afield.strCaption = afield.strCaption.trim();
                        newCntrl.dictAttributes.Text = afield.strCaption.slice(0, afield.strCaption.length - 1) + " To : ";
                    }

                }
                else {
                    newCntrl.dictAttributes.Text = GetCaptionFromFieldName(afield.ID) + " To : ";
                }

                newCell.Elements.push(newCntrl);
                sfxParent.Elements.push(newCell);

            }

            if (j < aCellCount) {
                j++;

                var newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWCOLUMN, ApplicationConstants.XMLFacade.PREFIX_SWC);

                var newCntrl = {
                    Name: 'sfwTextBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.SFWTEXTBOX, ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = 'txt' + controlID + "To";
                if ($scope.objFormsDetails.IsShowMainQuery) {
                    newCntrl.dictAttributes.sfwQueryID = $scope.objFormsDetails.MainQueryID.split('.')[0];
                }
                newCntrl.dictAttributes.sfwDataField = afield.ID;
                newCntrl.dictAttributes.sfwOperator = "between";

                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date") {
                    newCntrl.dictAttributes.sfwDataType = "datetime";
                    newCntrl.dictAttributes.sfwDataFormat = "{0:d}";
                }
                else {
                    newCntrl.dictAttributes.sfwDataType = "numeric";
                }

                newCell.Elements.push(newCntrl);


                var newCntrl = {
                    Name: 'CompareValidator', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.COMPAREVALIDATOR, ApplicationConstants.XMLFacade.PREFIX_SWC);

                newCntrl.dictAttributes.ID = "val" + controlID + "To";
                newCntrl.dictAttributes.Text = "*";
                newCntrl.dictAttributes.Operator = "DataTypeCheck";
                newCntrl.dictAttributes.ControlToValidate = 'txt' + controlID + "To";

                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date") {
                    newCntrl.dictAttributes.Type = "Date";
                    if (afield.strCaption) {
                        newCntrl.dictAttributes.ErrorMessage = afield.strCaption + " To must be in mm/dd/yyyy format.";
                    }
                    else {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " To must be in mm/dd/yyyy format.";
                    }
                }
                else {
                    newCntrl.dictAttributes.Type = "Integer";
                    if (afield.strCaption) {
                        newCntrl.dictAttributes.ErrorMessage = afield.strCaption + " From must be an Integer value.";
                    }
                    else {
                        newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " To must be an Integer value.";
                    }
                }

                newCell.Elements.push(newCntrl);

                // Insert second CompareValidator


                var newCntrl = {
                    Name: 'CompareValidator', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };//.SetName(ApplicationConstants.XMLFacade.COMPAREVALIDATOR, ApplicationConstants.XMLFacade.PREFIX_SWC);


                newCntrl.dictAttributes.ID = "val" + controlID + "Range";
                newCntrl.dictAttributes.Text = '*';
                newCntrl.dictAttributes.Operator = "GreaterThanEqual";
                newCntrl.dictAttributes.ControlToValidate = 'txt' + controlID + "To";
                newCntrl.dictAttributes.ControlToCompare = 'txt' + controlID + "From";
                if (afield.strCaption) {
                    newCntrl.dictAttributes.ErrorMessage = afield.strCaption + " To cannot be less than " + afield.strCaption + " From.";
                }
                else {
                    newCntrl.dictAttributes.ErrorMessage = GetCaptionFromFieldName(afield.ID) + " To cannot be less than " + GetCaptionFromFieldName(afield.ID) + " From.";
                }

                if (afield.DataType.toLowerCase() == "datetime" || afield.DataType.toLowerCase() == "date")
                    newCntrl.dictAttributes.Type = "Date";
                else
                    newCntrl.dictAttributes.Type = "Integer";

                newCell.Elements.push(newCntrl);
                sfxParent.Elements.push(newCell);

            }
        }
    };

    //#endregion

    //#region Update data on Result Grid(sixth Screen)

    $scope.UpdateResultFields = function () {
        function iterator(objGrid) {
            var strFieldName = objGrid.strFieldName;
            var strDataType = objGrid.strDataType;
            var strHeaderText = objGrid.strHeaderText;
            var strControlType = objGrid.strControlType;
            var strDataFormat = objGrid.strDataFormat;

            strVisible = strFieldName.substring(strFieldName.lastIndexOf('.') + 1);
            strVisible = IsAuditField(strVisible) ? "False" : "True";


            var lobjTemp = {
                Name: 'TemplateField', prefix: 'asp', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            lobjTemp.dictAttributes.HeaderText = strHeaderText;
            lobjTemp.dictAttributes.Visible = strVisible;


            var objItemTemplate = {
                Name: 'ItemTemplate', prefix: 'asp', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            switch (strControlType) {
                case "HyperLink":

                    var objControl = {
                        Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    objControl.dictAttributes.sfwLinkable = "True";
                    objControl.dictAttributes.sfwRelatedControl = "btnOpen";
                    break;
                case "TextBox":

                    objControl = {
                        Name: 'sfwTextBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    if (strDataType.toLowerCase() == "datetime")
                        objControl.dictAttributes.sfwDataType = strDataType.toLowerCase();
                    break;
                case "DropDownList":

                    var objControl = {
                        Name: 'sfwDropDownList', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };

                    var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                    var codegroup = GetCodeID($scope.objFormsDetails.sfwEntity, strFieldName, entityIntellisenseList);
                    if (codegroup) { /* Bug 9674:In Lookup form-for Drop Down Control(Value Field) in Grid-InCorrect Properties are Displayed. */
                        //   objControl.dictAttributes.sfwLoadSource = codegroup;
                        objControl.dictAttributes.sfwLoadType = "CodeGroup";
                    }
                    break;
                case "Checkbox":

                    objControl = {
                        Name: 'sfwCheckBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    break;
                default:

                    objControl = {
                        Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    break;
            }



            objControl.dictAttributes.sfwEntityField = objGrid.strFieldName;
            objControl.dictAttributes.sfwDataFormat = strDataFormat;
            if (objGrid.IsVisible == "False") {
                objControl.dictAttributes.Visible = objGrid.IsVisible;
            }
            objItemTemplate.Elements.push(objControl);
            lobjTemp.Elements.push(objItemTemplate);
            objColumn.Elements.push(lobjTemp);

        }

        if ($scope.objLookupResultFields.lstResultFieldsForGrid && $scope.objLookupResultFields.lstResultFieldsForGrid.length > 0) {
            for (var i = 0; i < $scope.objGridView.Elements.length; i++) {
                if ($scope.objGridView.Elements[i].Name == "Columns") {
                    $scope.objGridView.Elements.splice(i, 1);
                    break;
                }
            }

            var objColumn = {
                Name: 'Columns', Value: '', dictAttributes: {}, Elements: [], Children: []
            };

            angular.forEach($scope.objLookupResultFields.lstResultFieldsForGrid, iterator);

            $scope.objLookupResultFields.UpdateGridProperties();
            //$scope.objGridView.dictAttributes.sfwDataKeyNames = strDataKeyNames;
            $scope.objGridView.Elements.push(objColumn);
        }
    };


    //#endregion



    //#region Update Data for Grid Button (Seventh Screen)
    $scope.UpdateResultPanel = function () {
        var sfxMainTable;
        var lstTables = $scope.objFormsDetails.objSfxLookupForm.Elements.filter(function (x) { return x.Name == "sfwTable"; });
        if (lstTables && lstTables.length > 0) {
            sfxMainTable = lstTables[0];
        }
        var thisRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        var sfxCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        $scope.DeleteResultPanel();


        var objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel.dictAttributes.ID = "pnlResult";
        objSfxPanel.dictAttributes.sfwCaption = "Search Results";
        objSfxTable.dictAttributes.ID = "tblResult";
        objSfxTable.dictAttributes.CssClass = 'Table';


        var newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        var newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        var strNavParams;

        if ($scope.objLookupResultFields.IsNewButton && $scope.objLookupResultFields.IsNewButton.toLowerCase() == "true") {

            newControl = {
                Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = "btnNew";
            newControl.dictAttributes.ImageUrl = "~/Image/gNew.gif";
            newControl.dictAttributes.sfwMethodName = "btnNew_Click";
            newControl.dictAttributes.sfwOperateOn = "tblCriteria";

            //newControl.dictAttributes.sfwActiveForm = $scope.objLookupGridButton.NewActiveForm;
            newControl.dictAttributes.Text = 'New';

            // newControl.dictAttributes.sfwNavigationParameter = strNavParams;
            newCell.Elements.push(newControl);
        }

        if ($scope.objLookupResultFields.IsOpenButton && $scope.objLookupResultFields.IsOpenButton.toLowerCase() == "true") {

            var newControl = {
                Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = "btnOpen";
            newControl.dictAttributes.ImageUrl = "~/Image/gOpen.gif";
            newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
            newControl.dictAttributes.sfwRelatedControl = "dgrResult";

            //newControl.dictAttributes.sfwActiveForm = $scope.objLookupGridButton.OpenActiveForm;
            newControl.dictAttributes.Text = "Open";

            //newControl.dictAttributes.sfwNavigationParameter = strNavParams;
            newCell.Elements.push(newControl);
        }

        if ($scope.objLookupResultFields.IsDeleteButton && $scope.objLookupResultFields.IsDeleteButton.toLowerCase() == "true") {

            var newControl = {
                Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = "btnDelete";
            newControl.dictAttributes.ImageUrl = "~/Image/gDelete.gif";
            newControl.dictAttributes.sfwMethodName = "btnDelete_Click";
            newControl.dictAttributes.sfwRelatedControl = "dgrResult";
            newControl.dictAttributes.Text = "Delete";
            newCell.Elements.push(newControl);
        }

        if ($scope.objLookupResultFields.IsExportExcelButton && $scope.objLookupResultFields.IsExportExcelButton.toLowerCase() == "true") {

            var newControl = {
                Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newControl.dictAttributes.ID = "btnExportExcel";
            newControl.dictAttributes.ImageUrl = "~/Image/gExport.gif";
            newControl.dictAttributes.sfwMethodName = "btnColumnsToExport_Click";
            newControl.dictAttributes.sfwRelatedControl = "dgrResult";
            newControl.dictAttributes.Text = "Export To Excel";
            newCell.Elements.push(newControl);
        }

        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newCell.Elements.push($scope.objGridView);

        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objSfxPanel.Elements.push(objSfxTable);

        if (sfxMainTable == null) {

            var sfxMainTable = {
                Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            $scope.objFormsDetails.objSfxLookupForm.Elements.push(sfxMainTable);
        }

        sfxCell.Elements.push(objSfxPanel);
        thisRow.Elements.push(sfxCell);
        sfxMainTable.Elements.push(thisRow);
    };

    $scope.DeleteResultPanel = function () {
        function iterator(sfxRow) {

            function getPanelResult(sfxCell) {
                if (sfxCell.Elements.length > 0) {
                    if (sfxCell.Elements[0].Name == 'sfwPanel') {
                        var sfxPanel = sfxCell.Elements[0];
                        if (sfxPanel.dictAttributes.ID == "pnlResult") {
                            arrResult = sfxRow;
                        }
                    }
                }
            }

            angular.forEach(sfxRow.Elements, getPanelResult);
            if (arrResult != null) {
                // break;
            }


        }

        var sfxMainTable;
        var arrResult;
        var lstTable = $scope.objFormsDetails.objSfxLookupForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (lstTable && lstTable.length > 0) {
            sfxMainTable = lstTable[0];


            angular.forEach(sfxMainTable.Elements, iterator);


            if (arrResult != null) {

                var index = sfxMainTable.Elements.indexOf(arrResult);
                if (index > -1)
                    sfxMainTable.Elements.splice(index, 1);
            }
        }
    };

    //#endregion
    //#endregion

    //#region Wizard Creation


    //#region Wizard Init Methods
    $scope.objWizardDetails.InitWizard = function () {
        $scope.objWizardDetails.ID = "";
        $scope.objWizardDetails.sfwTitle = "";
        $scope.objWizardDetails.sfwDescription = "";
        $scope.objWizardDetails.Path = "";
        $scope.objWizardDetails.sfwEntity = "";
        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "Wizard") {
            $scope.objWizardDetails.sfwEntity = $scope.SelectedEntity;
            $scope.objWizardDetails.PopulateFormName();
        }
        $scope.objWizardDetails.isFinsihDisable();
    };

    $scope.objWizardDetails.PopulateFormName = function () {
        $scope.objWizardDetails.ID = "";
        if ($scope.objWizardDetails.sfwEntity != undefined && $scope.objWizardDetails.sfwEntity != "") {
            $scope.objWizardDetails.ID = $scope.objWizardDetails.sfwEntity.substring(3);
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.objWizardDetails.sfwEntity; });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objWizardDetails.Path = strPath;
                }
            }
        }
        $scope.checkAndExecuteLocationCallback($scope.objWizardDetails);
    };

    //#endregion

    //#region Wizard disable options
    $scope.objWizardDetails.CanShowWizardSteps = function () {

        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "Wizard") {
            retVal = false;
        }
        return retVal;
    };

    //#endregion

    //#region Update Data
    $scope.objWizardDetails.UpdateData = function () {
        $scope.objWizardDetails.UpdateFormAttributes();
        $scope.objWizardDetails.UpdateCenterMiddle();
    };

    $scope.objWizardDetails.UpdateFormAttributes = function () {


        $scope.objSfxWizardForm = {
            Name: 'form', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.FORM);
        var strFormName = 'wfm' + $scope.objWizardDetails.ID + "Wizard";
        $scope.objSfxWizardForm.dictAttributes.ID = strFormName;
        if ($scope.objWizardDetails.sfwTitle) {
            $scope.objSfxWizardForm.dictAttributes.sfwTitle = $scope.objWizardDetails.sfwTitle;
        } else {
            $scope.objSfxWizardForm.dictAttributes.sfwTitle = GetFormTitle('Wizard', $scope.objWizardDetails.ID);
        }
        $scope.objSfxWizardForm.dictAttributes.sfwDescription = $scope.objWizardDetails.sfwDescription;
        $scope.objSfxWizardForm.dictAttributes.sfwEntity = $scope.objWizardDetails.sfwEntity;
        $scope.objSfxWizardForm.dictAttributes.sfwType = "Wizard";
        $scope.objSfxWizardForm.dictAttributes.sfwActive = "True";
        $scope.objSfxWizardForm.dictAttributes.sfwMenuItemID = '';
        $scope.objSfxWizardForm.dictAttributes.sfwAspNetCaching = '';
    };

    $scope.objWizardDetails.UpdateCenterMiddle = function () {

        var sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var objSfxWizard = {
            Name: 'sfwWizard', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxWizard.dictAttributes.ID = "wizMain";
        //objSfxWizard.dictAttributes.DisplaySideBar = 'False';
        objSfxWizard.dictAttributes.Width = "100%";


        var objWizardStep = {
            Name: 'WizardSteps', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        }; //.SetName(ApplicationConstants.XMLFacade.WIZARDSTEPS);


        var objSfxWizardStep = {
            Name: 'sfwWizardStep', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        }; //.SetName(ApplicationConstants.XMLFacade.SFWWIZARDSTEP, ApplicationConstants.XMLFacade.PREFIX_SWC);
        objSfxWizardStep.dictAttributes.ID = "wzsStep";
        objSfxWizardStep.dictAttributes.sfwShowInHeader = 'True';
        objSfxWizardStep.dictAttributes.Title = "New Step";


        var newRow;
        var newCell;
        var objSfxTable;

        //#region Add Header Template


        var objHeaderTemplate = {
            Name: 'HeaderTemplate', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };


        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable.dictAttributes.ID = "tblHeaderTemplate";
        objSfxTable.dictAttributes.CssClass = 'Table';


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objHeaderTemplate.Elements.push(objSfxTable);

        //#endregion

        //#region Add Wizard Step


        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable.dictAttributes.ID = "tblStep";
        objSfxTable.dictAttributes.CssClass = 'Table';


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objSfxWizardStep.Elements.push(objSfxTable);
        objWizardStep.Elements.push(objSfxWizardStep);
        objSfxWizard.Elements.push(objWizardStep);
        objSfxWizard.Elements.push(objHeaderTemplate);

        // #endregion

        //#region Add SideBar Template

        var objSideBarTemplate = {
            Name: 'SideBarTemplate', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };     //.SetName(ApplicationConstants.XMLFacade.SIDEBARTEMPLATE);


        var objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSideBarTemplate.Elements.push(objSfxTable);
        objSfxWizard.Elements.push(objSideBarTemplate);
        // #endregion


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell.Elements.push(objSfxWizard);
        MainRow.Elements.push(newCell);
        MainRow.Elements.push(MainCell);
        MainRow.Elements.push(newCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objSfxWizardForm.Elements.push(sfxMainTable);

    };

    //#endregion

    //#region for Finish Disable
    $scope.objWizardDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objWizardDetails.ErrorMessageForDisplay = "";

        if (!$scope.objWizardDetails.ValidateEntity()) {
            IsValid = false;
        }
        else if (!$scope.objWizardDetails.ValidateFormName()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objWizardDetails, $scope.objWizardDetails.Path)) {
            IsValid = false;
        }
        $scope.objWizardDetails.isWizardFinsihDisable = IsValid;
        return IsValid;
    };
    //#endregion

    //#region Form Name Validation for Wizard
    $scope.objWizardDetails.ValidateFormName = function () {
        var retValue = true;
        if ($scope.objWizardDetails.ID == undefined || $scope.objWizardDetails.ID == "") {
            $scope.objWizardDetails.ErrorMessageForDisplay = "Error: Please enter a Wizard Name (ID).";
            retValue = false;
        }
        else if ($scope.objWizardDetails.ID && $scope.objWizardDetails.ID.toLowerCase().startsWith("wfm")) {
            $scope.objWizardDetails.ErrorMessageForDisplay = "Error: ID can not start with wfm.";
            retValue = false;
        }

        else if (($scope.objWizardDetails.ID != undefined || $scope.objWizardDetails.ID != "") && $scope.objWizardDetails.ID.length <= 3) {
            $scope.objWizardDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }
        else if ($scope.objWizardDetails.ID && $scope.objWizardDetails.ID.toLowerCase().endsWith("wizard")) {
            $scope.objWizardDetails.ErrorMessageForDisplay = "Error: ID can not ends with wizard.";
            retValue = false;
        }
        else {
            if (!isValidFileID($scope.objWizardDetails.ID, true)) {
                $scope.objWizardDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
                retValue = false;
            }

            var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "wfm" + $scope.objWizardDetails.ID.toLowerCase() + "wizard" || x.FileID.toLowerCase() == "wfm" + $scope.objWizardDetails.ID.toLowerCase() + "wizard"; });
            if (lst.length > 0) {
                $scope.objWizardDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Wizard Name (ID).";
                retValue = false;
            }


        }
        return retValue;
    };

    //#endregion

    //#region Validate Path for Wizard

    //#endregion

    //#region Validate Entity  for Wizard
    $scope.objWizardDetails.ValidateEntity = function () {
        var isValid = true;
        if ($scope.objWizardDetails.sfwEntity == undefined || $scope.objWizardDetails.sfwEntity == "") {
            $scope.objWizardDetails.ErrorMessageForDisplay = "Error: Entity cannot be empty.";
            isValid = false;
        }

        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objWizardDetails.sfwEntity; })) {
                $scope.objWizardDetails.ErrorMessageForDisplay = "Error: The entity is not valid. Please enter a different Name (ID).";
                isValid = false;

            }
        }
        return isValid;
    };

    //#endregion


    //#endregion

    //#region Report Creation

    //#region Wizard disable options
    $scope.objReportDetails.CanShowReportSteps = function () {

        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "Report") {
            retVal = false;
        }
        return retVal;
    };

    //#endregion

    //#region Finish Disable
    $scope.objReportDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objReportDetails.ErrorMessageForDisplay = "";

        if (!$scope.objReportDetails.ValidateReportID()) {
            IsValid = false;
        }
        else if (!$scope.objReportDetails.ValidateReportName()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objReportDetails, $scope.objReportDetails.Path)) {
            IsValid = false;
        }
        else if (!$scope.objReportDetails.ValidateInputFields()) {
            IsValid = false;
        }
        else if (!$scope.objReportDetails.ValidateQuery()) {
            IsValid = false;
        }
        $scope.objReportDetails.isReportFinsihDisable = IsValid;
        return IsValid;
    };
    //#endregion

    //#region Validate Fields

    //#region Validation for Report ID
    $scope.objReportDetails.ValidateReportID = function () {
        var retValue = true;
        if ($scope.objReportDetails.ID == undefined || $scope.objReportDetails.ID == "") {
            $scope.objReportDetails.ErrorMessageForDisplay = "Error: Please enter a Report ID.";
            retValue = false;
        }
        else if ($scope.objReportDetails.ID && $scope.objReportDetails.ID.toLowerCase().startsWith("rpt")) {
            $scope.objReportDetails.ErrorMessageForDisplay = "Error: ID can not start with rpt.";
            retValue = false;
        }
        else if (($scope.objReportDetails.ID != undefined || $scope.objReportDetails.ID != "") && $scope.objReportDetails.ID.length <= 3) {
            $scope.objReportDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }

        else {
            if (!isValidFileID($scope.objReportDetails.ID)) {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: File Name must not start with digits, contain space(s), special characters(except '-').";
                retValue = false;
            }

            var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "rpt" + $scope.objReportDetails.ID.toLowerCase(); });
            if (lst.length > 0) {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Report ID.";
                retValue = false;

            }


        }
        return retValue;
    };

    //#endregion

    //#region Validation for Report Name
    $scope.objReportDetails.ValidateReportName = function () {
        var retValue = true;
        if ($scope.objReportDetails.sfwReportName == undefined || $scope.objReportDetails.sfwReportName == "") {
            $scope.objReportDetails.ErrorMessageForDisplay = "Error: Please select a Report Name.";
            retValue = false;
        }
        else if ($scope.objReportDetails.lstReportData.length > 0) {
            if (!$scope.objReportDetails.lstReportData.some(function (x) { return x != undefined && x == $scope.objReportDetails.sfwReportName; })) {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: Invalid Report Name.";
                retValue = false;
            }
        }
        return retValue;
    };

    //#endregion



    //#region Validate Resource and Report category validation
    $scope.objReportDetails.ValidateInputFields = function () {
        if (angular.isArray($scope.objReportDetails.lstReportCategory) && $scope.objReportDetails.sfwReportCategory) {
            var list = $ValidationService.getListByPropertyName($scope.objReportDetails.lstReportCategory, "CodeValue", false);
            if (list.indexOf($scope.objReportDetails.sfwReportCategory.trim()) <= -1) {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: invalid Report category.";
                return false;
            }
        }
        if (angular.isArray($scope.resourceList) && $scope.objReportDetails.sfwResource && $scope.resourceList.indexOf($scope.objReportDetails.sfwResource.trim()) <= -1) {
            $scope.objReportDetails.ErrorMessageForDisplay = "Error: invalid Resource id.";
            return false;
        }

        return true;
    };
    //#region End of Resource and Report category validation
    //#region Validate Query
    $scope.objReportDetails.ValidateQuery = function () {
        var isValid = true;
        if ($scope.objReportDetails.sfwType == 'Query') {
            if ($scope.objReportDetails.sfwQueryRef == undefined || $scope.objReportDetails.sfwQueryRef == "") {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: Please select a Query.";
                isValid = false;
            } else if ($scope.objReportDetails.sfwQueryRef) {
                if (!$scope.objReportDetails.sfwQueryRef.contains(".")) {
                    $scope.objReportDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                    isValid = false;
                }
                else {
                    var queryId = $scope.objReportDetails.sfwQueryRef;
                    var lst = queryId.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            if (!objEntity.Queries.some(function (x) { return x.ID == strQueryID; })) {
                                $scope.objReportDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                                isValid = false;
                            }
                        }
                        else {
                            $scope.objReportDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                            isValid = false;
                        }
                    }

                    return isValid;
                }
            }
        }
        else if ($scope.objReportDetails.sfwType == 'Method') {
            if ($scope.objReportDetails.sfwMethodName == undefined || $scope.objReportDetails.sfwMethodName == "") {
                $scope.objReportDetails.ErrorMessageForDisplay = "Error: Please select a Method.";
                isValid = false;
            }
        }

        return isValid;
    };
    //#endregion

    //#endregion

    //#region Init Of Report
    $scope.objReportDetails.InitForms = function () {
        $scope.objReportDetails.ID = "";
        $scope.objReportDetails.sfwReportName = "";
        $scope.objReportDetails.sfwDescription = "";
        $scope.objReportDetails.Path = "";
        $scope.objReportDetails.sfwQueryRef = "";
        $scope.objReportDetails.sfwMethodName = "";
        $scope.objReportDetails.sfwReportCategory = "";
        $scope.objReportDetails.sfwResource = "";
        $scope.objReportDetails.sfwType = 'Query';
        $.connection.hubCreateNewObject.server.loadReportData().done(function (ReportData) {
            $scope.$apply(function () {
                $scope.objReportDetails.lstReportData = ReportData;
            });
        });
        $.connection.hubCreateNewObject.server.loadMethodList().done(function (data) {
            if (data) {
                $scope.objReportDetails.lstReportMethod = JSON.parse(data);
            }
        });

        $.connection.hubCreateNewObject.server.loadReportCategory().done(function (data) {
            $scope.$evalAsync(function () {
                if (data) {
                    $scope.objReportDetails.lstReportCategory = JSON.parse(data);
                }
            });
        });

        $scope.objReportDetails.isFinsihDisable();
    };





    $scope.objReportDetails.onReportNameChange = function (event) {
        var input = $(event.target);
        // $(input).val($(input).val().replace(/^\s+/, ""));
        setSingleLevelAutoComplete(input, $scope.objReportDetails.lstReportData);

        if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(input).data('ui-autocomplete')) {
            $(input).autocomplete("search", $(input).val().trim());
            event.preventDefault();
        }
    };

    $scope.objReportDetails.onReportNameClick = function (event) {
        var inputElement = $(event.target).prevAll("input[type='text']");

        if (inputElement) {
            inputElement.focus();
            setSingleLevelAutoComplete(inputElement, $scope.objReportDetails.lstReportData);
            if ($(inputElement).data('ui-autocomplete')) $(inputElement).autocomplete("search", $(inputElement).val());
        }
        if (event) {
            event.stopPropagation();
        }
    };

    //#endregion

    //#region  Open Query Dialog
    $scope.objReportDetails.openQueryDialog = function () {
        //$scope.QueryDialog = ngDialog.open({
        //    template: "Views/Form/BrowseForQuery.html",
        //    scope: $scope,
        //    closeByDocument: false,
        //    className: 'ngdialog-theme-default ngdialog-theme-custom',
        //});
    };


    $scope.$on('onQueryClick', function (event, data) {
        $scope.objReportDetails.sfwQueryRef = data;
    });

    //#endregion

    //#region Method Parameter
    $scope.objReportDetails.GetMethodParameters = function (methodName) {
        var alParams = [];


        function iterator(itm) {
            var strParamType = itm.ParameterType.FullName;
            strParamType = strParamType.substring(strParamType.lastIndexOf('.') + 1);

            alParams.push({ ID: " " + itm.ParameterName, DataType: strParamType });
        }
        var lstMethod = $scope.objReportDetails.lstReportMethod.filter(function (itm) {
            return itm.ShortName == methodName;
        });
        if (lstMethod && lstMethod.length > 0) {
            var objMethod = lstMethod[0];

            angular.forEach(objMethod.Parameters, iterator);
        }
        return alParams;

    };
    //#endregion
    //#region Update Data
    $scope.objReportDetails.UpdateData = function () {
        $scope.objReportDetails.UpdateReportDetails();
        $scope.objReportDetails.UpdateReportParameters();

    };
    $scope.objReportDetails.UpdateReportDetails = function () {
        var strReportID = 'rpt' + $scope.objReportDetails.ID;

        $scope.objReport = {
            Name: 'form', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        $scope.objReport.dictAttributes.ID = strReportID;
        $scope.objReport.dictAttributes.sfwReportName = $scope.objReportDetails.sfwReportName;
        $scope.objReport.dictAttributes.sfwReportCategory = $scope.objReportDetails.sfwReportCategory;
        $scope.objReport.dictAttributes.sfwResource = $scope.objReportDetails.sfwResource;
        $scope.objReport.dictAttributes.sfwDescription = $scope.objReportDetails.sfwDescription;
        $scope.objReport.dictAttributes.sfwType = 'Report';
        $scope.objReport.dictAttributes.sfwActive = "True";
        $scope.objReport.dictAttributes.sfwStatus = 'Review';
        $scope.objReport.dictAttributes.version = "4.0";
    };
    $scope.objReportDetails.UpdateReportParameters = function () {


        var sfxInitialLoadObject = {
            Name: 'initialload', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        var objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable.dictAttributes.ID = "Main";
        var strArrParams = [];

        if ($scope.objReportDetails.sfwType && $scope.objReportDetails.sfwType == 'Query') {

            var sfxQuery = {
                Name: 'query', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            sfxQuery.dictAttributes.sfwQueryRef = $scope.objReportDetails.sfwQueryRef;
            sfxInitialLoadObject.Elements.push(sfxQuery);
            strArrParams = $scope.objReportDetails.onQueryChange();
        }
        else if ($scope.objReportDetails.sfwType && $scope.objReportDetails.sfwType == 'Method') {

            var sfxMethod = {
                Name: 'custommethod', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            sfxMethod.dictAttributes.sfwMethodName = $scope.objReportDetails.sfwMethodName;
            sfxInitialLoadObject.Elements.push(sfxMethod);
            strArrParams = $scope.objReportDetails.GetMethodParameters($scope.objReportDetails.sfwMethodName);
        }

        if (sfxInitialLoadObject && strArrParams.length > 0) {
            var newRow;
            var newCell;
            var newControl;

            var i = 0;
            var strParameter;
            var strParamLabel;

            while (i < strArrParams.length) {

                newRow = {
                    Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };
                strParameter = strArrParams[i];
                strParameter = strParameter.ID.substr(1);
                strParamLabel = GetCaptionFromFieldName(strParameter) + " : ";


                newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };

                newControl = {
                    Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: { sfwIsCaption: 'True' }, Elements: [], Children: []
                };
                newControl.dictAttributes.Text = strParamLabel;
                newCell.Elements.push(newControl);
                newRow.Elements.push(newCell);


                newCell = {
                    Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };

                newControl = {
                    Name: 'sfwTextBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                };
                newControl.dictAttributes.ID = strParameter;
                newControl.dictAttributes.sfwObjectField = strParameter;
                newCell.Elements.push(newControl);
                newRow.Elements.push(newCell);

                i++;

                if (i < strArrParams.length) {
                    strParameter = strArrParams[i];
                    strParameter = strParameter.ID.substr(1);
                    strParamLabel = GetCaptionFromFieldName(strParameter) + " : ";



                    newCell = {
                        Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };

                    newControl = {
                        Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: { sfwIsCaption: 'True' }, Elements: [], Children: []
                    };
                    newControl.dictAttributes.Text = strParamLabel;
                    newCell.Elements.push(newControl);
                    newRow.Elements.push(newCell);


                    newCell = {
                        Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };

                    newControl = {
                        Name: 'sfwTextBox', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
                    };
                    newControl.dictAttributes.ID = strParameter;
                    newControl.dictAttributes.sfwObjectField = strParameter;
                    newCell.Elements.push(newControl);
                    newRow.Elements.push(newCell);

                    i++;
                }
                objSfxTable.Elements.push(newRow);
            }
        }
        else {

            newRow = {
                Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };


            newCell = {
                Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
            };
            newRow.Elements.push(newCell);

            objSfxTable.Elements.push(newRow);
        }

        $scope.objReport.Elements.push(sfxInitialLoadObject);
        $scope.objReport.Elements.push(objSfxTable);


    };

    //#region Get Query params

    $scope.objReportDetails.onQueryChange = function () {
        $scope.lstQryField = [];
        function iterator(x) {
            $scope.lstQryField.push(x);
        }
        if ($scope.objReportDetails.sfwQueryRef) {

            var queryId = $scope.objReportDetails.sfwQueryRef;
            var lst = queryId.split('.');
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var strQueryID = lst[1];
                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                var lstEntity = entityIntellisenseList.filter(function (x) {
                    return x.ID == entityName;
                });
                if (lstEntity && lstEntity.length > 0) {
                    var objEntity = lstEntity[0];
                    var lstQuery = objEntity.Queries.filter(function (x) {
                        return x.ID == strQueryID;
                    });
                    if (lstQuery && lstQuery.length > 0) {
                        var objQuery = lstQuery[0];


                        angular.forEach(objQuery.Parameters, iterator);
                    }
                }
            }
        }
        return $scope.lstQryField;
    };
    //#endregion
    //#endregion

    //#endregion

    //#region FormLink Creation
    $scope.objFormLinkDetails.InitFormLink = function () {
        $scope.objFormLinkDetails.lstWebSite = [];
        $scope.objFormLinkDetails.ID = "";
        $scope.objFormLinkDetails.HtmlTemplatePath = "";
        $scope.objFormLinkDetails.Path = "";
        $scope.objFormLinkDetails.MainQueryID = "";
        $scope.objFormLinkDetails.sfwEntity = "";

        if ($scope.SelectedEntity && $scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
            $scope.objFormLinkDetails.sfwEntity = $scope.SelectedEntity;
            $scope.objFormLinkDetails.PopulateFormName();
        }

        if ($scope.objFormLinkDetails.sfwEntity) {
            $.connection.hubForm.server.getFormEntityModel($scope.objFormLinkDetails.sfwEntity).done(function (data) {
                $scope.$apply(function () {

                    $scope.objFormLinkDetails.objEntityModel = data;
                    var lstGroupList = $scope.objFormLinkDetails.objEntityModel.Elements.filter(function (x) { return x.Name == 'groupslist'; });
                    if (lstGroupList && lstGroupList.length > 0) {
                        $scope.objFormLinkDetails.GroupList = lstGroupList[0];
                    }
                });
            });
        }
        $scope.objFormLinkDetails.isFormLinkFinish();
    };

    //#region FormLink disable options
    $scope.objFormLinkDetails.CanShowFormLinkStep = function () {

        var retVal = true;
        if ($scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
            retVal = false;
        }
        return retVal;
    };
    //#endregion

    //#region FormLink Finish button disable
    $scope.objFormLinkDetails.isFormLinkFinish = function () {
        //if all fields are entered then set it to true
        var IsValid = true;
        $scope.objFormLinkDetails.ErrorMessageForDisplay = "";

        if (!$scope.objFormLinkDetails.ValidateFormName()) {
            IsValid = false;
        }
        else if (!$scope.objFormLinkDetails.ValidateHtmlTemplatePath()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objFormLinkDetails, $scope.objFormLinkDetails.Path)) {
            IsValid = false;
        }
        else if (!$scope.objFormLinkDetails.ValidateMainQuery()) {
            IsValid = false;
        }
        else if (!$scope.objFormLinkDetails.ValidateEntity()) {
            IsValid = false;
        }
        $scope.objFormLinkDetails.isFormLinkDetailsFinishDisable = IsValid;
        return IsValid;
    };

    $scope.objFormLinkDetails.ValidateFormName = function () {
        var retValue = true;
        if ($scope.objFormLinkDetails.ID == undefined || $scope.objFormLinkDetails.ID == "") {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Please enter a Form Name (ID).";
            retValue = false;
        }
        else if ($scope.objFormLinkDetails.ID && $scope.objFormLinkDetails.ID.toLowerCase().startsWith("htx")) {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: ID can not start with htx.";
            retValue = false;
        }
        else if (($scope.objFormLinkDetails.ID != undefined || $scope.objFormLinkDetails.ID != "") && $scope.objFormLinkDetails.ID.length <= 3) {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }

        else if ($scope.objFormLinkDetails.ID && $scope.objFormLinkDetails.ID.toLowerCase().endsWith("lookup")) {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: ID can not ends with lookup.";
            retValue = false;
        }
        else if ($scope.objFormLinkDetails.ID && $scope.objFormLinkDetails.ID.toLowerCase().endsWith("maintenance")) {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: ID can not ends with maintenance.";
            retValue = false;
        }
        else if ($scope.objFormLinkDetails.ID && $scope.objFormLinkDetails.ID.toLowerCase().endsWith("wizard")) {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: ID can not ends with wizard.";
            retValue = false;
        }
        else {
            if (!isValidFileID($scope.objFormLinkDetails.ID, true)) {
                $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
                retValue = false;
            }
            if ($scope.objNewItems.SelectedOption == "HtxLookup") {

                var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "htx" + $scope.objFormLinkDetails.ID.toLowerCase() + "lookup"; });
                if (lst.length > 0) {
                    $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                    retValue = false;
                }
            }
            else if ($scope.objNewItems.SelectedOption == "HtxMaintenance") {
                var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "htx" + $scope.objFormLinkDetails.ID.toLowerCase() + "maintenance"; });
                if (lst.length > 0) {
                    $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                    retValue = false;
                }
            }
            else if ($scope.objNewItems.SelectedOption == "HtxWizard") {
                var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "htx" + $scope.objFormLinkDetails.ID.toLowerCase() + "wizard"; });
                if (lst.length > 0) {
                    $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Form Name (ID).";
                    retValue = false;
                }
            }

        }
        return retValue;
    };



    $scope.objFormLinkDetails.ValidateEntity = function () {
        var isValid = true;
        if ($scope.objFormLinkDetails.sfwEntity == undefined || $scope.objFormLinkDetails.sfwEntity == "") {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Entity cannot be empty.";
            isValid = false;
        }
        else {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            if (!entityIntellisenseList.some(function (x) { return x.ID == $scope.objFormLinkDetails.sfwEntity; })) {
                $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: The entity is not valid. Please enter a different Name (ID).";
                isValid = false;

            }
        }
        return isValid;
    };

    $scope.objFormLinkDetails.ValidateHtmlTemplatePath = function () {
        var retValue = true;
        if ($scope.objFormLinkDetails.HtmlTemplatePath == undefined || $scope.objFormLinkDetails.HtmlTemplatePath == "") {
            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Please enter a Html Template Path.";
            retValue = false;
        }
        //check if file exists or not
        return retValue;
    };

    $scope.objFormLinkDetails.ValidateMainQuery = function () {
        var retValue = true;
        if ($scope.objNewItems.SelectedOption == 'HtxLookup' && $scope.objFormLinkDetails.IsShowMainQuery) {
            //if ($scope.objFormLinkDetails.MainQueryID == undefined || $scope.objFormLinkDetails.MainQueryID == "") {
            //    $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Please enter a Main Query.";
            //    retValue = false;
            //}

            if ($scope.objFormLinkDetails.MainQueryID == undefined || $scope.objFormLinkDetails.MainQueryID == "") {
                $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Please select a Lookup Query.";
                retValue = false;
            }
            else if ($scope.objFormLinkDetails.MainQueryID) {
                if (!$scope.objFormLinkDetails.MainQueryID.contains(".")) {
                    $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                    retValue = false;
                }
                else {
                    var queryId = $scope.objFormLinkDetails.MainQueryID;
                    var lst = queryId.split('.');
                    if (lst && lst.length == 2) {
                        var entityName = lst[0];
                        var strQueryID = lst[1];
                        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                        var lstEntity = entityIntellisenseList.filter(function (x) { return x.ID == entityName; });
                        if (lstEntity && lstEntity.length > 0) {
                            var objEntity = lstEntity[0];
                            if (!objEntity.Queries.some(function (x) { return x.ID == strQueryID; })) {
                                $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                                retValue = false;
                            }
                        }
                        else {
                            $scope.objFormLinkDetails.ErrorMessageForDisplay = "Error: Invalid Query";
                            retValue = false;
                        }
                    }

                    return retValue;
                }
            }
        }
        //check if file exists or not
        return retValue;
    };
    //#endregion

    $scope.objFormLinkDetails.getSelectedDetails = function (ProjectProperty, selectiontype, noOfFiles, filterType) {
        $scope.selectedProperty = ProjectProperty;
        if (selectiontype == "FilePath") {
            var initialDirectory;
            $scope.IsSingleOrMultiple = noOfFiles;
            $.connection.hubFormLink.server.getHtmlTemplateFilePath().done(function (data) {
                $scope.HtmlTemplatePath = data;
                $scope.objFormLinkDetails.HtmlFolderTemplatePath = data;
                $.connection.hubMain.server.getFilePath(false, "CreateNewObject", $scope.HtmlTemplatePath, "html");
            });
            //hubcontext.hubMain.server.getFilePath(noOfFiles == "M", "ProjectSettings", initialDirectory, filterType);
        }
    };

    $scope.objFormLinkDetails.PopulateFormName = function () {
        $scope.objFormLinkDetails.ID = "";
        $scope.objFormLinkDetails.IsShowMainQuery = false;
        if ($scope.objFormLinkDetails.sfwEntity != undefined && $scope.objFormLinkDetails.sfwEntity != "") {
            $scope.objFormLinkDetails.ID = $scope.objFormLinkDetails.sfwEntity.substring(3);
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            var entities = entityIntellisenseList.filter(function (x) { return x.ID == $scope.objFormLinkDetails.sfwEntity });
            if (entities && entities.length > 0) {
                if (entities[0].FilePath.contains('.')) {
                    var strPath;
                    if ($scope.baseXmlFileLocation && entities[0].FilePath.indexOf($scope.baseXmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.baseXmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else if ($scope.xmlFileLocation && entities[0].FilePath.indexOf($scope.xmlFileLocation) === 0) {
                        strPath = entities[0].FilePath.substring($scope.xmlFileLocation.length + 1, entities[0].FilePath.lastIndexOf('\\'));
                    }
                    else {
                        strPath = entities[0].FilePath.substring(0, entities[0].FilePath.lastIndexOf('\\'));
                        strPath = strPath.substring(strPath.lastIndexOf('\\') + 1);
                    }
                    $scope.objFormLinkDetails.Path = strPath;
                }
                if (!entities[0].TableName) {
                    //$scope.objFormLinkDetails.IsShowMainQuery = true;
                    $scope.checkShowMainQuery($scope.objFormLinkDetails, $scope.objFormLinkDetails.sfwEntity);
                }
            }
        }
        $scope.checkAndExecuteLocationCallback($scope.objFormLinkDetails);
    };

    $scope.receiveNewHtmlFilePath = function (data) {
        $scope.$apply(function () {
            $scope.objFormLinkDetails.HtmlTemplatePath = data[0];
            $scope.objFormLinkDetails.isFormLinkFinish();
        });
    };

    $scope.receiveMapUnmapControls = function (controlLists, controlTypes) {
        function iAddInvalidControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.objFormLinkDetails.InvalidControlList.push(item);
        }

        function iAddIgnoredControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.objFormLinkDetails.IgnoredControlList.push(item);
        }

        function iAddRemovedControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.objFormLinkDetails.RemovedControlList.push(item);
        }

        function iADdMappedControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.objFormLinkDetails.MappedControlList.push(item);
        }

        function iAddMapControlList(cntrl) {
            var item = {
                ControlID: cntrl.ControlID, HtmlControlType: cntrl.HtmlControlType, ControlType: cntrl.ControlType, IsValid: cntrl.IsValid
            };
            $scope.objFormLinkDetails.MapControlList.push(item);
        }
        $rootScope.IsLoading = false;
        var data = controlLists;
        $scope.objFormLinkDetails.MapControlList = [];
        $scope.objFormLinkDetails.MappedControlList = [];
        $scope.objFormLinkDetails.IgnoredControlList = [];
        $scope.objFormLinkDetails.RemovedControlList = [];
        $scope.objFormLinkDetails.InvalidControlList = [];
        if (data.Map && data.Map.length > 0) {

            angular.forEach(data.Map, iAddMapControlList);
        }
        if (data.Mapped && data.Mapped.length > 0) {

            angular.forEach(data.Mapped, iADdMappedControlList);
        }
        if (data.Removed && data.Removed.length > 0) {

            angular.forEach(data.Removed, iAddRemovedControlList);
        }
        if (data.Ignored && data.Ignored.length > 0) {

            angular.forEach(data.Ignored, iAddIgnoredControlList);
        }
        if (data.Invalid && data.Invalid.length > 0) {

            angular.forEach(data.Invalid, iAddInvalidControlList);
        }

        var data = controlTypes;
        $scope.$apply(function () {
            var newScope = $scope.$new();
            $scope.activeTab = "MAP";
            newScope.isMappedControlClick = false;
            newScope.MapControls = $scope.objFormLinkDetails.MapControlList;
            newScope.MappedControls = $scope.objFormLinkDetails.MappedControlList;
            newScope.RemovedControls = $scope.objFormLinkDetails.RemovedControlList;
            newScope.IgnoredControls = $scope.objFormLinkDetails.IgnoredControlList;
            newScope.InvalidControls = $scope.objFormLinkDetails.InvalidControlList;
            newScope.ControlTypes = data;
            newScope.FormObject = $scope.objFormLinkDetails.FormInfo;
            if ($scope.objFormLinkDetails.FormInfo.FileType == "FormLinkLookup") {
                var index = newScope.ControlTypes.indexOf("sfwHyperLink");
                newScope.ControlTypes.splice(index, 1);
                index = newScope.ControlTypes.indexOf("sfwJSONData");
                if (index >= 0) {
                    newScope.ControlTypes.splice(index, 1);
                }
                index = newScope.ControlTypes.indexOf("sfwCalendar");
                if (index >= 0) {
                    newScope.ControlTypes.splice(index, 1);
                }
                index = newScope.ControlTypes.indexOf("sfwScheduler");
                if (index >= 0) {
                    newScope.ControlTypes.splice(index, 1);
                }
            }
            if ($scope.objFormLinkDetails.FormInfo.FileType == "FormLinkMaintenance" || $scope.objFormLinkDetails.FormInfo.FileType == "FormLinkWizard") {
                newScope.ControlTypes.push("sfwListView");
            }
            newScope.onOKClick = function () {
                //Do something on OK click
                //close the dialog
                $scope.objFormLinkDetails.MapControlList = newScope.MapControls;

                newScope.isOkClick = true;
                //newScope.MapControls
                newScope.SaveModelWithPackets(newScope.MapControls, 'Map');

                newScope.closeDialog();
            };
            newScope.closeDialog = function () {
                if (newScope.dialog) {
                    //if (!newScope.isOkClick)
                    //    $.connection.hubCreateNewObject.server.createHtxModel(newScope.isOkClick, newScope.FormObject, newScope.ControlTypes).done(function (data) {
                    //        var lst = data;
                    //        $scope.$apply(function () {
                    //            $rootScope.openFile(lst, true);

                    //            var entityScope = getCurrentFileScope();
                    //            if (entityScope != undefined) {
                    //                if (entityScope && entityScope.lstItemsHtxList) {
                    //                    entityScope.lstItemsHtxList.push(lst);
                    //                }
                    //            }

                    //            $rootScope.IsLoading = false;
                    //            $scope.dialogForNew.close();
                    //        });
                    //    });
                    newScope.isOkClick = false;
                    $rootScope.IsLoading = false;
                    newScope.dialog.close();

                }
            };

            newScope.setActiveTab = function (tab) {
                newScope.activeTab = tab;
                if (tab == "MAPPED")
                    newScope.isMappedControlClick = true;
            };

            newScope.SaveModelWithPackets = function (scopeobject, controlStatus) {

                var strobj = JSON.stringify(scopeobject);
                $rootScope.IsLoading = true;
                if (strobj.length < 32000) {
                    $.connection.hubCreateNewObject.server.populateCreationControls(scopeobject, controlStatus).done(
                        $.connection.hubCreateNewObject.server.createHtxModel(newScope.isOkClick, newScope.FormObject, newScope.ControlTypes).done(function (data) {
                            var lst = data;
                            $scope.$evalAsync(function () {
                                $scope.dialogForNew.close();
                                $rootScope.openFile(lst, true);
                                var entityScope = getScopeByFileName($scope.SelectedEntity);
                                if (entityScope != undefined) {
                                    if (entityScope && entityScope.lstItemsHtxList) {
                                        entityScope.lstItemsHtxList.push(lst);
                                    }
                                }
                                $rootScope.IsLoading = false;
                            });
                        })
                    );
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

                    SendNewHtxDataPacketsToServer(lstDataPackets, controlStatus);
                    $.connection.hubCreateNewObject.server.createHtxModel(newScope.isOkClick, newScope.FormObject, newScope.ControlTypes).done(function (data) {
                        var lst = data;
                        $scope.$evalAsync(function () {
                            $scope.dialogForNew.close();
                            $rootScope.openFile(lst, true);
                            var entityScope = getScopeByFileName($scope.SelectedEntity);
                            if (entityScope != undefined) {
                                if (entityScope && entityScope.lstItemsHtxList) {
                                    entityScope.lstItemsHtxList.push(lst);
                                }
                            }
                            $rootScope.IsLoading = false;
                        });
                    });
                }
            };

            var SendNewHtxDataPacketsToServer = function (lstpackets, controlStatus) {

                for (var i = 0; i < lstpackets.length; i++) {
                    $.connection.hubCreateNewObject.server.receiveNewHtxDataPackets(lstpackets[i], lstpackets.length, i, controlStatus);
                }
            };

            newScope.dialog = $rootScope.showDialog(newScope, "Control Mapping", "CreateNewObject/views/FormLink/FormLinkMapUnmapControls.html", { width: 600, height: 400 });
        });
        $rootScope.IsLoading = false;
    };

    //#endregion

    //#region Prototype Creation

    //#region Prototype Init Methods
    $scope.objPrototypeFormDetails.InitWizard = function () {
        $scope.objPrototypeFormDetails.ID = "";
        $scope.objPrototypeFormDetails.sfwTitle = "";
        $scope.objPrototypeFormDetails.isFinsihDisable();
    };
    //#endregion

    //#region for Detail Next Disable
    $scope.objPrototypeFormDetails.isNextDisable = function () {
        var IsValid = true;
        $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "";

        if (!$scope.objPrototypeFormDetails.ValidateFormName()) {
            IsValid = false;
        }

        if (IsValid && $scope.objNewItems.SelectedOption.toLowerCase() != "prototypelookup") {
            IsValid = false;
        }
        $scope.objPrototypeFormDetails.isPrototypeFormDetailsNextDisable = IsValid;
        $scope.objPrototypeLookupColumns.isFinsihDisable();
    };
    //#endregion

    //#region for Detail Finish Disable
    $scope.objPrototypeFormDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "";

        if (!$scope.objPrototypeFormDetails.ValidateFormName()) {
            IsValid = false;
        }
        $scope.objPrototypeFormDetails.isPrototypeFormDetailsFinishDisable = IsValid;

        return IsValid;
    };
    //#endregion

    //#region for Grid Column Finish Disable
    $scope.objPrototypeLookupColumns.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objPrototypeLookupColumns.ErrorMessageForDisplay = "";

        if (!$scope.objPrototypeLookupColumns.ValidateGridColumn()) {
            IsValid = false;
        }
        $scope.objPrototypeLookupColumns.isPrototypeFormDetailsFinishDisable = IsValid;

        return IsValid;
    };

    $scope.objPrototypeLookupColumns.ValidateGridColumn = function () {
        var retValue = true;
        if ($scope.objPrototypeLookupColumns.lstColumns.length == 0) {
            $scope.objPrototypeLookupColumns.ErrorMessageForDisplay = "Error: Atleast One Column should be there.";
            retValue = false;
        }
        return retValue;
    };

    //#endregion

    //#region Form Name Validation for Wizard
    $scope.objPrototypeFormDetails.ValidateFormName = function () {
        var retValue = true;
        if ($scope.objPrototypeFormDetails.ID == undefined || $scope.objPrototypeFormDetails.ID == "") {
            $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: Please enter a " + $scope.objNewItems.SelectedOption.substring(9) + " Name (ID).";
            retValue = false;
        }
        else if (!isValidFileID($scope.objPrototypeFormDetails.ID, true)) {
            $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: Numeric Values and Special characters are not allowed in Name.";
            retValue = false;
        }
        else if ($scope.objPrototypeFormDetails.ID && $scope.objPrototypeFormDetails.ID.toLowerCase().startsWith("wfp")) {
            $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: ID can not start with wfp.";
            retValue = false;
        }

        else if (($scope.objPrototypeFormDetails.ID != undefined || $scope.objPrototypeFormDetails.ID != "") && $scope.objPrototypeFormDetails.ID.length <= 3) {
            $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: Invalid ID. Minimum 4 characters are required.";
            retValue = false;
        }
        else if ($scope.objPrototypeFormDetails.ID && $scope.objPrototypeFormDetails.ID.toLowerCase().endsWith($scope.objNewItems.SelectedOption.substring(9).toLowerCase())) {
            $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: ID can not end with " + $scope.objNewItems.SelectedOption.substring(9) + " .";
            retValue = false;
        }
        else {
            var lst = $scope.lstAllFiles.filter(function (x) { return x.FileName.toLowerCase() == "wfp" + $scope.objPrototypeFormDetails.ID.toLowerCase() + $scope.objNewItems.SelectedOption.substring(9).toLowerCase(); });
            if (lst.length > 0) {
                $scope.objPrototypeFormDetails.ErrorMessageForDisplay = "Error: The File Name already exists.\nPlease enter a different Wizard Name (ID).";
                retValue = false;
            }
        }
        return retValue;
    };

    //#endregion

    //#region Proptotype Lookup Grid Column Methods

    $scope.objPrototypeLookupColumns.onSelectLookupColumns = function (obj) {
        $scope.objPrototypeLookupColumns.SelectedColumn = obj;
    };

    $scope.objPrototypeLookupColumns.AddColumn = function () {
        var obj = { ColumnName: '', ControlType: 'Label' };
        $scope.objPrototypeLookupColumns.lstColumns.push(obj);
        $scope.objPrototypeLookupColumns.onSelectLookupColumns(obj);
        $scope.objPrototypeLookupColumns.isFinsihDisable();
        $scope.scrollBySelectedField("#prototype-lookup-column-details", ".selected");
    };

    //#region Delete Grid Column

    $scope.objPrototypeLookupColumns.deleteColumn = function () {
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            var index = $scope.objPrototypeLookupColumns.lstColumns.indexOf($scope.objPrototypeLookupColumns.SelectedColumn);
            if (index >= 0) {
                $scope.objPrototypeLookupColumns.lstColumns.splice(index, 1);
                if (index < $scope.objPrototypeLookupColumns.lstColumns.length) {
                    $scope.objPrototypeLookupColumns.SelectedColumn = $scope.objPrototypeLookupColumns.lstColumns[index];
                }
                else if ($scope.objPrototypeLookupColumns.lstColumns.length > 0) {
                    $scope.objPrototypeLookupColumns.SelectedColumn = $scope.objPrototypeLookupColumns.lstColumns[index - 1];
                }
            }
        }
        $scope.objPrototypeLookupColumns.isFinsihDisable();
    };

    $scope.objPrototypeLookupColumns.canDeleteColumn = function () {
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            return true;
        }
        else {
            return false;
        }
    };

    //#endregion

    //#region Move Up Grid Column

    $scope.objPrototypeLookupColumns.moveSelectedGridColumnUp = function () {
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            var index = $scope.objPrototypeLookupColumns.lstColumns.indexOf($scope.objPrototypeLookupColumns.SelectedColumn);
            var item = $scope.objPrototypeLookupColumns.lstColumns[index - 1];
            $scope.objPrototypeLookupColumns.lstColumns[index - 1] = $scope.objPrototypeLookupColumns.SelectedColumn;
            $scope.objPrototypeLookupColumns.lstColumns[index] = item;
            $scope.scrollBySelectedField("#prototype-lookup-column-details", ".selected");
        }
    };


    $scope.objPrototypeLookupColumns.canmoveSelectedGridColumnUp = function () {
        var Flag = true;
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            for (var i = 0; i < $scope.objPrototypeLookupColumns.lstColumns.length; i++) {
                if ($scope.objPrototypeLookupColumns.lstColumns[i] == $scope.objPrototypeLookupColumns.SelectedColumn) {
                    if (i > 0) {
                        Flag = false;
                    }
                }
            }

        }

        return Flag;
    };
    //#endregion

    //#region Move Down Grid Column

    $scope.objPrototypeLookupColumns.moveSelectedGridColumnDown = function () {
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            var index = $scope.objPrototypeLookupColumns.lstColumns.indexOf($scope.objPrototypeLookupColumns.SelectedColumn);
            var item = $scope.objPrototypeLookupColumns.lstColumns[index + 1];
            $scope.objPrototypeLookupColumns.lstColumns[index + 1] = $scope.objPrototypeLookupColumns.SelectedColumn;
            $scope.objPrototypeLookupColumns.lstColumns[index] = item;
            $scope.scrollBySelectedField("#prototype-lookup-column-details", ".selected");
        }
    };

    $scope.objPrototypeLookupColumns.canmoveSelectedGridColumnDown = function () {
        var Flag = true;
        if ($scope.objPrototypeLookupColumns.SelectedColumn) {
            for (var i = 0; i < $scope.objPrototypeLookupColumns.lstColumns.length; i++) {
                if ($scope.objPrototypeLookupColumns.lstColumns[i] == $scope.objPrototypeLookupColumns.SelectedColumn) {
                    if (i < $scope.objPrototypeLookupColumns.lstColumns.length - 1) {
                        Flag = false;
                    }
                }
            }
        }

        return Flag;
    };
    //#endregion

    //#endregion

    //#region Update Data for Prototype
    $scope.objPrototypeFormDetails.UpdateData = function (fileType) {
        $scope.objPrototypeFormDetails.UpdateFormAttributes(fileType);
        if (fileType == "Wizard") {
            $scope.objPrototypeFormDetails.UpdateCenterMiddle();
        }
        else if (fileType == "Lookup") {
            $scope.objPrototypeFormDetails.UpdateCenterMiddleForLookup();
        }
        else if (fileType == "Maintenance") {
            $scope.objPrototypeFormDetails.UpdateCenterMiddleForMaintenance();
        }
    };

    //#region Common Data

    $scope.objPrototypeFormDetails.UpdateFormAttributes = function (fileType) {


        $scope.objSfxPrototypeForm = {
            Name: 'form', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };//.SetName(ApplicationConstants.XMLFacade.FORM);
        var strFormName = 'wfp' + $scope.objPrototypeFormDetails.ID + fileType;
        $scope.objSfxPrototypeForm.dictAttributes.ID = strFormName;
        $scope.objSfxPrototypeForm.dictAttributes.sfwTitle = $scope.objPrototypeFormDetails.sfwTitle;


        $scope.objSfxPrototypeForm.dictAttributes.sfwEntity = "entPrototype";
        $scope.objSfxPrototypeForm.dictAttributes.sfwType = fileType;
        $scope.objSfxPrototypeForm.dictAttributes.sfwActive = "True";

        $scope.objSfxPrototypeForm.dictAttributes.sfwAspNetCaching = 'False';
    };

    //#endregion

    //#region Update Data for Wizard

    $scope.objPrototypeFormDetails.UpdateCenterMiddle = function () {

        var sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        var objSfxWizard = {
            Name: 'sfwWizard', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxWizard.dictAttributes.ID = "wizMain";
        //objSfxWizard.dictAttributes.DisplaySideBar = 'False';
        objSfxWizard.dictAttributes.Width = "100%";


        var objWizardStep = {
            Name: 'WizardSteps', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        }; //.SetName(ApplicationConstants.XMLFacade.WIZARDSTEPS);


        var objSfxWizardStep = {
            Name: 'sfwWizardStep', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        }; //.SetName(ApplicationConstants.XMLFacade.SFWWIZARDSTEP, ApplicationConstants.XMLFacade.PREFIX_SWC);
        objSfxWizardStep.dictAttributes.ID = "wzsStep";
        objSfxWizardStep.dictAttributes.sfwShowInHeader = 'True';
        objSfxWizardStep.dictAttributes.Title = "New Step";


        var newRow;
        var newCell;
        var objSfxTable;

        //#region Add Header Template


        var objHeaderTemplate = {
            Name: 'HeaderTemplate', prefix: '', Value: '', dictAttributes: {}, Elements: [], Children: []
        };


        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable.dictAttributes.ID = "tblHeaderTemplate";
        objSfxTable.dictAttributes.CssClass = 'Table';


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objHeaderTemplate.Elements.push(objSfxTable);

        //#endregion

        //#region Add Wizard Step


        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable.dictAttributes.ID = "tblStep";
        objSfxTable.dictAttributes.CssClass = 'Table';


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objSfxWizardStep.Elements.push(objSfxTable);
        objWizardStep.Elements.push(objSfxWizardStep);
        objSfxWizard.Elements.push(objWizardStep);
        objSfxWizard.Elements.push(objHeaderTemplate);

        // #endregion

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell.Elements.push(objSfxWizard);
        MainRow.Elements.push(newCell);
        MainRow.Elements.push(MainCell);
        MainRow.Elements.push(newCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objSfxPrototypeForm.Elements.push(sfxMainTable);

    };

    //#endregion

    //#region Update Data for Maintenance
    $scope.objPrototypeFormDetails.UpdateCenterMiddleForMaintenance = function () {
        var MainCell;
        var MainRow;
        var objSfxPanel;
        var objSfxTable;
        var newRow;
        var newCell;
        var newControl;

        var sfxMainTable = $scope.objSfxPrototypeForm.Elements.filter(function (itm) { return itm.Name == "sfwTable"; });
        if (sfxMainTable && sfxMainTable.length > 0) {
            var index = $scope.objSfxPrototypeForm.Elements.indexOf(sfxMainTable);
            if (index > -1) {
                $scope.objSfxPrototypeForm.Elements.splice(index, 1);
            }
        }

        sfxMainTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        // toolbar Panel

        objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel.dictAttributes.ID = "pnltoolbar";
        objSfxTable.dictAttributes.ID = "toolbar";
        objSfxTable.dictAttributes.CellPadding = "3";


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        newControl.dictAttributes.ID = "btnSave";
        newControl.dictAttributes.sfwMethodName = "btnSave_Click";
        newControl.dictAttributes.Text = "Save";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnCancel";
        newControl.dictAttributes.sfwMethodName = "btnCancel_Click";
        newControl.dictAttributes.Text = "Refresh";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnPrev";
        newControl.dictAttributes.sfwMethodName = "btnPrev_Click";
        newControl.dictAttributes.Text = "Prev";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnNext";
        newControl.dictAttributes.sfwMethodName = "btnNext_Click";
        newControl.dictAttributes.Text = "Next";
        newCell.Elements.push(newControl);


        newControl = {
            Name: 'sfwButton', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "btnCorrespondence";
        newControl.dictAttributes.sfwMethodName = "btnCorrespondence_Click";
        newControl.dictAttributes.Text = "Correspondence";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);
        MainCell.Elements.push(objSfxPanel);
        objSfxTable.Elements.push(newRow);
        objSfxPanel.Elements.push(objSfxTable);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);

        // tblAuditInfo Panel


        MainRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        MainCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxPanel = {
            Name: 'sfwPanel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        objSfxTable = {
            Name: 'sfwTable', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };

        objSfxPanel.dictAttributes.ID = "pnlAuditInfo";
        objSfxTable.dictAttributes.ID = "tblAuditInfo";
        objSfxTable.dictAttributes.CssClass = 'Table';
        objSfxPanel.dictAttributes.sfwCaption = "Audit Information";

        var strObjectField = null;


        newRow = {
            Name: 'sfwRow', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        // Create

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "capCreate";
        newControl.dictAttributes.Text = "Created : ";
        newControl.dictAttributes.sfwIsCaption = "True";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblCreatedBy";
        newControl.dictAttributes.Text = "Created By";
        newControl.dictAttributes.sfwIsCaption = "True";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblCreatedDate";
        newControl.dictAttributes.Text = "Created Date";
        newControl.dictAttributes.sfwIsCaption = "True";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        // Modify

        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "capModify";
        newControl.dictAttributes.Text = "Modified : ";

        newControl.dictAttributes.sfwIsCaption = "True";

        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblModifiedBy";
        newControl.dictAttributes.Text = "Modified By";
        newControl.dictAttributes.sfwIsCaption = "True";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);


        newCell = {
            Name: 'sfwColumn', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl = {
            Name: 'sfwLabel', prefix: 'swc', Value: '', dictAttributes: {}, Elements: [], Children: []
        };
        newControl.dictAttributes.ID = "lblModifiedDate";
        newControl.dictAttributes.Text = "Modified Date";
        newControl.dictAttributes.sfwIsCaption = "True";


        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        objSfxTable.Elements.push(newRow);
        objSfxPanel.Elements.push(objSfxTable);

        MainCell.Elements.push(objSfxPanel);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objSfxPrototypeForm.Elements.push(sfxMainTable);

    };
    //#endregion

    //#region Update Data for Lookup
    $scope.objPrototypeFormDetails.UpdateCenterMiddleForLookup = function () {
        var sfxMainTable = { Name: 'sfwTable', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        var objSfxPanel = { Name: 'sfwPanel', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        var MainRow = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        var MainCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };

        var objSfxPanel = $scope.objPrototypeFormDetails.UpdateSearchCriteria();

        MainCell.Elements.push(objSfxPanel);
        MainRow.Elements.push(MainCell);
        sfxMainTable.Elements.push(MainRow);
        $scope.objSfxPrototypeForm.Elements.push(sfxMainTable);

        $scope.objPrototypeLookupColumns.UpdateResultGrid();
        $scope.objPrototypeLookupColumns.UpdateResultPanel(sfxMainTable);

    };

    $scope.objPrototypeFormDetails.UpdateSearchCriteria = function () {
        var objSfxPanel;
        var newRow;
        var newCell;
        var newControl;
        objSfxPanel = { Name: 'sfwPanel', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        objSfxPanel.dictAttributes.ID = "pnlCriteria";
        var objSfxTable = { Name: 'sfwTable', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        objSfxTable.dictAttributes.ID = "tblCriteria";

        // Rows & Columns
        var iRows = 3;
        var iColumns = 4;

        for (var i = 0; i < iRows; i++) {
            newRow = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };

            for (var j = 0; j < iColumns; j++) {
                var newColumn = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                newRow.Elements.push(newColumn);
            }

            objSfxTable.Elements.push(newRow);
        }

        // Buttons
        newRow = {
            Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: []
        };

        newCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };

        newControl = {
            Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: []
        };
        newControl.dictAttributes.ID = "btnSearch";
        newControl.dictAttributes.ImageUrl = "~/Image/gSearch.gif";
        newControl.dictAttributes.sfwMethodName = "btnPrototypeSearch_Click";
        newControl.dictAttributes.sfwOperatorOn = "tblCriteria";
        newControl.dictAttributes.Text = "Search";
        newCell.Elements.push(newControl);

        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnReset";
        newControl.dictAttributes.ImageUrl = "~/Image/gReset.gif";
        newControl.dictAttributes.sfwMethodName = "btnReset_Click";
        newControl.dictAttributes.sfwOperatorOn = "tblCriteria";
        newControl.dictAttributes.Text = "Reset";
        newCell.Elements.push(newControl);
        newRow.Elements.push(newCell);

        newCell = {
            Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: []
        };

        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnStoreSearch";
        newControl.dictAttributes.ImageUrl = "~/Image/gSave.gif";
        newControl.dictAttributes.sfwMethodName = "btnStoreUserDefaults_Click";
        newControl.dictAttributes.sfwOperatorOn = "tblCriteria";
        newControl.dictAttributes.Text = "Store Search";
        newCell.Elements.push(newControl);

        newRow.Elements.push(newCell);

        newCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newRow.Elements.push(newCell);

        newCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newRow.Elements.push(newCell);

        objSfxTable.Elements.push(newRow);
        objSfxPanel.Elements.push(objSfxTable);
        return objSfxPanel;
    };

    $scope.objPrototypeLookupColumns.UpdateResultGrid = function () {
        $scope.objPrototypeLookupColumns.objGridView = { Name: "sfwGridView", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        $scope.objPrototypeLookupColumns.objGridView.dictAttributes.ID = "dgrResult";
        $scope.objPrototypeLookupColumns.objGridView.dictAttributes.AllowPaging = "True";
        $scope.objPrototypeLookupColumns.objGridView.dictAttributes.AllowSorting = "True";
        $scope.objPrototypeLookupColumns.objGridView.dictAttributes.sfwSelection = "Many";

        var objItemTemplate;

        var objColumn = { Name: "Columns", Value: '', dictAttributes: {}, Elements: [] };
        for (var i = 0; i < $scope.objPrototypeLookupColumns.lstColumns.length > 0; i++) {
            var objClsControl = $scope.objPrototypeLookupColumns.lstColumns[i];
            var strColumnName = objClsControl.ColumnName;
            var strControlType = objClsControl.ControlType;

            var lobjTemp = {
                Name: "TemplateField", Value: '', prefix: 'asp', dictAttributes: {}, Elements: []
            };
            lobjTemp.dictAttributes.HeaderText = strColumnName;

            objItemTemplate = { Name: "ItemTemplate", Value: '', prefix: 'asp', dictAttributes: {}, Elements: [] };

            var objControl;
            switch (strControlType) {
                case "HyperLink":
                    objControl = { Name: "sfwLabel", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                    objControl.dictAttributes.sfwLinkable = "True";
                    break;
                case "TextBox":
                    objControl = { Name: "sfwTextBox", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                    break;
                case "DropDownList":
                    objControl = { Name: "sfwDropDownList", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                    break;
                case "CheckBox":
                    objControl = { Name: "sfwCheckBox", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                    objControl.dictAttributes.sfwTwoWayBinding = "False";
                    break;
                default:
                    objControl = { Name: "sfwLabel", Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
                    break;
            }

            objControl.dictAttributes.sfwEntityField = "Property" + i;
            objItemTemplate.Elements.push(objControl);
            lobjTemp.Elements.push(objItemTemplate);
            objColumn.Elements.push(lobjTemp);
        }
        $scope.objPrototypeLookupColumns.objGridView.Elements.push(objColumn);
    };

    $scope.objPrototypeLookupColumns.UpdateResultPanel = function (sfxMainTable) {
        var objSfxPanel;
        var objSfxTable;
        var newRow;
        var newCell;
        var newControl;

        var thisRow = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };

        sfxCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };


        $scope.objPrototypeLookupColumns.DeleteResultPanel(sfxMainTable);

        objSfxPanel = { Name: 'sfwPanel', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        objSfxPanel.dictAttributes.ID = "pnlResult";
        objSfxPanel.dictAttributes.sfwCaption = "Search Results";

        objSfxTable = { Name: 'sfwTable', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        objSfxTable.dictAttributes.ID = "tblResult";

        newRow = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };


        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnNew";
        newControl.dictAttributes.ImageUrl = "~/Image/gNew.gif";
        newControl.dictAttributes.sfwMethodName = "btnNew_Click";
        newControl.dictAttributes.sfwOperatorOn = "tblCriteria";

        newControl.dictAttributes.Text = "New";

        newCell.Elements.push(newControl);

        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnOpen";
        newControl.dictAttributes.ImageUrl = "~/Image/gOpen.gif";
        newControl.dictAttributes.sfwMethodName = "btnOpen_Click";
        newControl.dictAttributes.sfwRelatedControl = "dgrResult";
        newControl.dictAttributes.Text = "Open";

        newCell.Elements.push(newControl);


        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnDelete";
        newControl.dictAttributes.ImageUrl = "~/Image/gDelete.gif";
        newControl.dictAttributes.sfwMethodName = "btnDelete_Click";
        newControl.dictAttributes.sfwRelatedControl = "dgrResult";
        newControl.dictAttributes.Text = "Delete";

        newCell.Elements.push(newControl);

        newControl = { Name: 'sfwButton', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newControl.dictAttributes.ID = "btnExportExcel";
        newControl.dictAttributes.ImageUrl = "~/Image/gExport.gif";
        newControl.dictAttributes.sfwMethodName = "btnColumnsToExport_Click";
        newControl.dictAttributes.sfwRelatedControl = "dgrResult";
        newControl.dictAttributes.Text = "Export To Excel";
        newCell.Elements.push(newControl);

        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        newRow = { Name: 'sfwRow', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
        newCell = { Name: 'sfwColumn', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };

        newCell.Elements.push($scope.objPrototypeLookupColumns.objGridView);

        newRow.Elements.push(newCell);
        objSfxTable.Elements.push(newRow);

        objSfxPanel.Elements.push(objSfxTable);

        if (sfxMainTable == null) {
            sfxMainTable = { Name: 'sfwTable', Value: '', prefix: 'swc', dictAttributes: {}, Elements: [] };
            $scope.objSfxPrototypeForm.Elements.push(sfxMainTable);
        }

        sfxCell.Elements.push(objSfxPanel);
        thisRow.Elements.push(sfxCell);
        sfxMainTable.Elements.push(thisRow);

    };

    $scope.objPrototypeLookupColumns.DeleteResultPanel = function (sfxMainTable) {
        var arrResult;
        function iterateTable(sfxRow) {
            function iterateRow(sfxCell) {
                if (sfxCell.Elements.length > 0) {
                    if (sfxCell.Elements[0].Name === "sfwPanel") {
                        var sfxPanel = sfxCell.Elements[0];
                        if (!arrResult) {
                            if (sfxPanel.dictAttributes.ID == "pnlResult") {
                                arrResult = sfxRow;
                            }
                        }
                    }
                }
                angular.forEach(sfxRow.Elements, iterateRow);
            }
        }
        if ($scope.objSfxPrototypeForm.Elements.length > 0) {
            if (sfxMainTable) {

                angular.forEach(sfxMainTable.Elements, iterateTable);

                if (arrResult) {
                    var index = sfwMainTable.Elements.indexOf(arrResult);
                    sfxMainTable.Elements.splice(index, 1);
                }
            }
        }

    };

    //#endregion

    //#endregion

    //#endregion

    $scope.onFileTypeChanged = function () {
        var fileType = "";
        if ($scope.objNewItems.SelectedOption == "BPMN" || $scope.objNewItems.SelectedOption == "BPMTemplate") {
            fileType = $scope.objNewItems.SelectedOption == "BPMN" ? "BPM Map" : "BPMN Map Template";
            setExtraFieldsVariables(fileType);
        }
        else if ($scope.objNewItems.SelectedOption == "HtxLookup" || $scope.objNewItems.SelectedOption == "HtxMaintenance" || $scope.objNewItems.SelectedOption == "HtxWizard") {
            fileType = $scope.objNewItems.SelectedOption == "HtxLookup" ? "Html Lookup" : ($scope.objNewItems.SelectedOption == "HtxMaintenance" ? "Html Maintenance" : "Html Wizard");
            setExtraFieldsVariables(fileType);
        }
        else {
            fileType = $scope.objNewItems.SelectedOption;
            setExtraFieldsVariables(fileType);
        }
    };
    $scope.setTitle = function (title) {
        if ($scope.$parent.title) {
            $scope.$parent.title = title;
            $scope.$parent.dialogForNew.updateTitle($scope.$parent.title);
        }
    };

    $scope.validateExtraFields = function () {
        var isValid = false;
        if ($scope.objDirFunctions.validateExtraFieldsData) {
            isValid = $scope.objDirFunctions.validateExtraFieldsData();
        }
        return isValid;
    };

    var getExtraFields = function (obj) {
        // #region extra field data
        if ($scope.objDirFunctions.prepareExtraFieldData) {
            $scope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
        }
        //#endregion
        obj.Elements.push($scope.objExtraFields);
    };


    $scope.objDirFunctions.showExtraFields = function (isPresent) {
        $scope.showExtraFieldsTab = isPresent;
    };


    $scope.checkCodeValues = function (lstCodeValues) {
        var retValue = false;
        angular.forEach(lstCodeValues.lstTableCodeValue, function (codeValue) {
            if (codeValue.errors) {
                if ($ValidationService.isEmptyObj(codeValue.errors)) {
                    retValue = true;
                }
            }
        });

        if ($scope.IsOneToOneOrManyDisable && !retValue) {
            if ($scope.showExtraFields) {
                retValue = true;
            }
        }
        return retValue;
    };

    //#region WorkflowMap Creation

    $scope.objWorkflowMapDetails.InitWorkflowMap = function () {
        $scope.objWorkflowMapDetails.isFinsihDisable();
        $scope.objWorkflowMapDetails.ID = "";

        $scope.objWorkflowMapDetails.LoadWorkflowMapFile();
    };
    //#region Load Workflow Map file
    $scope.objWorkflowMapDetails.LoadWorkflowMapFile = function () {
        $.connection.hubCreateNewObject.server.loadMapFiles().done(function (WorkflowMapFiles) {
            $scope.$apply(function () {
                $scope.objWorkflowMapDetails.lstWorkflowMapFiles = WorkflowMapFiles;
            });
        });
        $.connection.hubCreateNewObject.server.loadProcessType().done(function (WorkflowMapTypes) {
            $scope.$apply(function () {
                $scope.objWorkflowMapDetails.lstWorkflowMapProcessType = WorkflowMapTypes;
            });
        });

    };

    //#endregion 

    //#region Create Workflow Map

    $scope.CreateWorkflowMap = function () {

        var objCreateWorkflowMap = {};

        objCreateWorkflowMap.FileID = $scope.objWorkflowMapDetails.FileID;
        objCreateWorkflowMap.ProcessType = $scope.objWorkflowMapDetails.ProcessType;
        objCreateWorkflowMap.Path = $scope.objWorkflowMapDetails.Path;

    };

    //#endregion


    //#region Finish Disable

    $scope.objWorkflowMapDetails.isFinsihDisable = function () {
        var IsValid = true;
        $scope.objWorkflowMapDetails.ErrorMessageForDisplay = "";

        if (!$scope.objWorkflowMapDetails.ValidateMapID()) {
            IsValid = false;
        }
        else if (!$scope.objWorkflowMapDetails.ValidateprocessType()) {
            IsValid = false;
        }
        else if (!$scope.ValidatePath($scope.objWorkflowMapDetails, $scope.objWorkflowMapDetails.Path)) {
            IsValid = false;
        }

        $scope.objWorkflowMapDetails.isWorkflowMapFinsihDisable = IsValid;
        return IsValid;
    };
    //#endregion


    //#region Validation

    $scope.objWorkflowMapDetails.ValidateMapID = function () {
        var retValue = true;
        if ($scope.objWorkflowMapDetails.FileID == undefined || $scope.objWorkflowMapDetails.FileID == "") {
            $scope.objWorkflowMapDetails.ErrorMessageForDisplay = "Error: Please select a Map File.";
            retValue = false;
        }

        return retValue;
    };

    $scope.objWorkflowMapDetails.ValidateprocessType = function () {

        var retValue = true;
        if ($scope.objWorkflowMapDetails.ProcessType == undefined || $scope.objWorkflowMapDetails.ProcessType == "") {
            $scope.objWorkflowMapDetails.ErrorMessageForDisplay = "Error: Please select a Workflow Process Type.";
            retValue = false;
        }

        return retValue;

    };


    //#endregion

    //#region Close Click

    $scope.CloseWorkflowMapClick = function () {
        $scope.dialogForNew.close();
    };
    //#endregion

    //#endregion

    // Create New Directory if it is not exists

    $scope.OnNextPageLoad();
    $scope.onFileTypeChanged();
}]);