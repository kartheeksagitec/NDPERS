app.controller('customSettingsController', ["$scope", "$rootScope", "$timeout", function ($scope, $rootScope, $timeout) {
    //#region On Load
    $scope.currentfile = $rootScope.currentopenfile.file;

    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (data) {
        if (data) {
            $scope.receiveCustomSettingsmodel(data);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receiveCustomSettingsmodel = function (data) {
        $scope.$apply(function () {

            $scope.customsettingsfiledata = data;
            $scope.selectedTab = "entity";
            $scope.SelectedCustomTab = "Entity";

            /*** Get extra field model of file and stored in scope **/
            for (var i = 0; i < $scope.customsettingsfiledata.Elements.length; i++) {
                if ($scope.customsettingsfiledata.Elements[i].Name == 'Entity') {
                    $scope.objEntity = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Rule') {
                    $scope.objRule = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Form') {
                    $scope.objForm = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'HTML') {
                    $scope.objFormHtml = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Correspondence') {
                    $scope.objCorrespondence = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Scenario') {
                    $scope.objScenario = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'BPM') {
                    $scope.objBPM = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Workflow') {
                    $scope.objWorkflow = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'File') {
                    $scope.objFile = $scope.customsettingsfiledata.Elements[i];
                }
                else if ($scope.customsettingsfiledata.Elements[i].Name == 'Report') {
                    $scope.objReport = $scope.customsettingsfiledata.Elements[i];
                }
            }

            //initially select first row of every object
            if ($scope.objEntity && $scope.objEntity.Elements.length > 0) {
                $scope.objEntity.SelectedField = $scope.objEntity.Elements[0];
            }
            if ($scope.objForm && $scope.objForm.Elements.length > 0) {
                $scope.objForm.SelectedField = $scope.objForm.Elements[0];
            }
            if ($scope.objFormHtml && $scope.objFormHtml.Elements.length > 0) {
                $scope.objFormHtml.SelectedField = $scope.objFormHtml.Elements[0];
            }
            if ($scope.objCorrespondence && $scope.objCorrespondence.Elements.length > 0) {
                $scope.objCorrespondence.SelectedField = $scope.objCorrespondence.Elements[0];
            }
            if ($scope.objScenario && $scope.objScenario.Elements.length > 0) {
                $scope.objScenario.SelectedField = $scope.objScenario.Elements[0];
            }
            if ($scope.objBPM && $scope.objBPM.Elements.length > 0) {
                $scope.objBPM.SelectedField = $scope.objBPM.Elements[0];
            }
            if ($scope.objWorkflow && $scope.objWorkflow.Elements.length > 0) {
                $scope.objWorkflow.SelectedField = $scope.objWorkflow.Elements[0];
            }
            if ($scope.objFile && $scope.objFile.Elements.length > 0) {
                $scope.objFile.SelectedField = $scope.objFile.Elements[0];
            }
            if ($scope.objReport && $scope.objReport.Elements.length > 0) {
                $scope.objReport.SelectedField = $scope.objReport.Elements[0];
            }
            if ($scope.objRule && $scope.objRule.Elements.length > 0) {
                $scope.objRule.SelectedField = $scope.objRule.Elements[0];
            }

            //  Initially if object is empty then create new object schema
            if (!$scope.objEntity) {
                $scope.objEntity = $scope.CreateNewObject("Entity", $scope.customsettingsfiledata);
            }
            if (!$scope.objRule) {
                $scope.objRule = $scope.CreateNewObject("Rule", $scope.customsettingsfiledata);
            }
            if (!$scope.objForm) {
                $scope.objForm = $scope.CreateNewObject("Form", $scope.customsettingsfiledata);
            }
            if (!$scope.objFormHtml) {
                $scope.objFormHtml = $scope.CreateNewObject("HTML", $scope.customsettingsfiledata);
            }
            if (!$scope.objCorrespondence) {
                $scope.objCorrespondence = $scope.CreateNewObject("Correspondence", $scope.customsettingsfiledata);
            }
            if (!$scope.objScenario) {
                $scope.objScenario = $scope.CreateNewObject("Scenario", $scope.customsettingsfiledata);
            }
            if (!$scope.objBPM) {
                $scope.objBPM = $scope.CreateNewObject("BPM", $scope.customsettingsfiledata);
            }
            if (!$scope.objWorkflow) {
                $scope.objWorkflow = $scope.CreateNewObject("Workflow", $scope.customsettingsfiledata);
            }
            if (!$scope.objFile) {
                $scope.objFile = $scope.CreateNewObject("File", $scope.customsettingsfiledata);
            }
            if (!$scope.objReport) {
                $scope.objReport = $scope.CreateNewObject("Report", $scope.customsettingsfiledata);
            }
        });
    };

    //#endregion

    //#region Xml source
    $scope.selectedDesignSource = false;
    var editor_html;
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {

            var xmlstring = $scope.editor.getValue();
            if (xmlstring != null && xmlstring != "") {
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
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receiveCustomSettingsmodel(data);
                        $scope.isSourceDirty = false;
                    }
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                    });
                };
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
            if ($scope.customsettingsfiledata) {
                $rootScope.IsLoading = true;
                var objreturn1 = GetBaseModel($scope.customsettingsfiledata);

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
                            var ID = "customsettings";
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

    //#region Common Methods


    $scope.selectTab = function (tabName) {
        $scope.selectedTab = tabName;
    };

    $scope.CreateNewObject = function (strNodeName, objParent) {
        var objItem = {
            Name: strNodeName, Value: '', dictAttributes: {}, Elements: []
        };
        objParent.Elements.push(objItem);
        return objItem;
    };

    $scope.AddExtraFields = function (fieldname) {
        var fieldobj = {
            dictAttributes: {
                value: "",
                Description: "",
                ControlType: "TextBox",
                IsRequired: false
            },
            Elements: [],
            Children: [],
            Name: "ColumnName",
            Value: ""
        };
        if (fieldname == 'Entity') {
            $rootScope.PushItem(fieldobj, $scope.objEntity.Elements);
            $scope.selectedField($scope.objEntity, fieldobj);
        }
        else if (fieldname == 'Form') {
            $rootScope.PushItem(fieldobj, $scope.objForm.Elements);
            $scope.selectedField($scope.objForm, fieldobj);
        }
        else if (fieldname == 'HTML') {
            $rootScope.PushItem(fieldobj, $scope.objFormHtml.Elements);
            $scope.selectedField($scope.objFormHtml, fieldobj);
        }
        else if (fieldname == 'Correspondence') {
            $rootScope.PushItem(fieldobj, $scope.objCorrespondence.Elements);
            $scope.selectedField($scope.objCorrespondence, fieldobj);
        }
        else if (fieldname == 'Scenario') {
            $rootScope.PushItem(fieldobj, $scope.objScenario.Elements);
            $scope.selectedField($scope.objScenario, fieldobj);
        }
        else if (fieldname == 'BPM') {
            $rootScope.PushItem(fieldobj, $scope.objBPM.Elements);
            $scope.selectedField($scope.objBPM, fieldobj);
        }
        else if (fieldname == 'Workflow') {
            $rootScope.PushItem(fieldobj, $scope.objWorkflow.Elements);
            $scope.selectedField($scope.objWorkflow, fieldobj);
        }
        else if (fieldname == 'File') {
            $rootScope.PushItem(fieldobj, $scope.objFile.Elements);
            $scope.selectedField($scope.objFile, fieldobj);
        }
        else if (fieldname == 'Report') {
            $rootScope.PushItem(fieldobj, $scope.objReport.Elements);
            $scope.selectedField($scope.objReport, fieldobj);
        }
        else if (fieldname == 'Rule') {
            $rootScope.PushItem(fieldobj, $scope.objRule.Elements);
            $scope.selectedField($scope.objRule, fieldobj);
        }

        fieldobj.IsFieldVisibility = true;
    };

    $scope.selectedField = function (selectedField, obj) {
        if (selectedField.SelectedField && selectedField.SelectedField != obj) {
            selectedField.SelectedField.IsFieldVisibility = false;
        }
        selectedField.SelectedField = obj;
    };

    $scope.DeleteExtraFields = function (fieldname) {
        if (fieldname == 'Entity') {
            if ($scope.objEntity.SelectedField) {
                deleteRow($scope.objEntity);
            }
        }
        else if (fieldname == "Form") {
            if ($scope.objForm.SelectedField) {
                deleteRow($scope.objForm);
            }
        }
        else if (fieldname == "HTML") {
            if ($scope.objFormHtml.SelectedField) {
                deleteRow($scope.objFormHtml);
            }
        }
        else if (fieldname == "Correspondence") {
            if ($scope.objCorrespondence.SelectedField) {
                deleteRow($scope.objCorrespondence);
            }
        }
        else if (fieldname == "Scenario") {
            if ($scope.objScenario.SelectedField) {
                deleteRow($scope.objScenario);
            }
        }
        else if (fieldname == "BPM") {
            if ($scope.objBPM.SelectedField) {
                deleteRow($scope.objBPM);
            }
        }
        else if (fieldname == "Workflow") {
            if ($scope.objWorkflow.SelectedField) {
                deleteRow($scope.objWorkflow);
            }
        }
        else if (fieldname == "File") {
            if ($scope.objFile.SelectedField) {
                deleteRow($scope.objFile);
            }
        }
        else if (fieldname == "Report") {
            if ($scope.objReport.SelectedField) {
                deleteRow($scope.objReport);
            }
        }
        else {
            if ($scope.objRule.SelectedField) {
                deleteRow($scope.objRule);
            }
        }
    };

    var deleteRow = function (objModel) {
        var index = objModel.Elements.indexOf(objModel.SelectedField);
        $rootScope.DeleteItem(objModel.SelectedField, objModel.Elements);
        if (index < objModel.Elements.length) {
            objModel.SelectedField = objModel.Elements[index];
        }
        else if (objModel.Elements.length > 0) {
            objModel.SelectedField = objModel.Elements[index - 1];
        }
    };
    //#endregion
}]);

app.directive("customSetttingsDirective", ["$rootScope", "$timeout", function ($rootScope, $timeout) {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
            currentFileObject: '='
        },
        link: function (scope, element, attributes) {
            scope.objSettings = scope.currentFileObject;
            scope.ControlTypes = ["TextBox", "ComboBox", "CheckBox", "CheckBoxList", "HyperLink"];

            scope.selectedObjectfield = function (obj) {
                if (scope.objSettings.SelectedField && scope.objSettings.SelectedField != obj) {
                    scope.objSettings.SelectedField.IsFieldVisibility = false;
                }
                scope.objSettings.SelectedField = obj;
            };

            scope.getBoolValueForEnablingOrDisabling = function (obj) {
                if (obj.dictAttributes.ControlType == "TextBox" || obj.dictAttributes.ControlType == "CheckBox" || obj.dictAttributes.ControlType == "HyperLink") {
                    obj.Elements = [];
                    if (obj.dictAttributes.ControlType == "CheckBox") delete obj.dictAttributes.IsRequired;
                    return true;
                }
                else {
                    return false;
                }
            };

            scope.slectFirstObjectValue = function (obj) {
                obj.SelectedValue = obj.Elements[0];
            };

            scope.selectedObjectValueIndex = function (index, obj, objvalues) {
                var selectedentityvalueindex = index;
                obj.SelectedValue = objvalues;
            };

            scope.AddValues = function (obj) {
                var objvalues = {
                    dictAttributes:
                        {
                            Text: "",
                            Description: "",
                        },
                    Elements: [],
                    Children: [],
                    Name: "Value",
                    Value: ""
                };
                $rootScope.PushItem(objvalues, obj.Elements);
                obj.SelectedValue = obj.Elements[obj.Elements.length - 1];
                $timeout(function () {
                    var elem = $('.cust-setting-panel-body table tr:last');
                    if (elem) {
                        $('.cust-setting-panel-body table > tbody').scrollTo(elem, null, null);
                    }
                });
            };

            scope.DeleteValues = function (obj, fieldname) {
                var index = obj.Elements.indexOf(obj.SelectedValue);
                $rootScope.DeleteItem(obj.Elements[index], obj.Elements);

                if (index > 0) {
                    obj.SelectedValue = obj.Elements[index - 1];
                }
                else {
                    obj.SelectedValue = obj.Elements[index];
                }
            };
        },
        templateUrl: 'Tools/views/CustomSettingsExtraField.html'
    };
}]);