app.controller("FileController", ["$scope", "$http", "$rootScope", "ngDialog", "$filter", "$EntityIntellisenseFactory", "$timeout", function ($scope, $http, $rootScope, ngDialog, $filter, $EntityIntellisenseFactory, $timeout) {

    //#region Variables
    $scope.dataformats = ['', 'MMddyy', 'yyyyMMdd', 'MM/dd/yyyy', 'ImpliedDecimal', 'SignedImpliedDecimal'];
    $scope.datatypes = ['', 'String', 'Numeric', 'Decimal', 'DateTime'];
    $scope.recordType = ['', 'CommaDelimited', 'FixedLength', 'Header', 'Footer', 'Detail', 'Trailer', 'AutoSkip'];
    if ($rootScope.currentopenfile.file.FileType == "InboundFile") $scope.recordType.splice(7, 1);
    else if ($rootScope.currentopenfile.file.FileType == "OutboundFile") {
        $scope.recordType.splice(1, 1);
        $scope.recordType.splice(1, 1);
    }

    $scope.outBoundDataFormats = ['', '{0:0.00}', '{0:000.00}', '{0:00000.00}', '{0:000000.00}', '{0:00000000.00}', '{0:000000000.00}', '{0:00000000000.00}', '{0:000-00-0000}',
        '{0:000000}', '{0:#.00}', '{0:#########.00}', '{0:########0.00}', '{0:###,###,###.00}', '{0:####}', '{0:######}', '{0:########}', '{0:#############}',
        '{0:000-##-####}', '{0:$#,###,###.00}', '{0:d}', '{0:hhmm}', '{0:MMddyyyy}', '{0:MM/dd/yyyy}', '{0:MMyyyy}', '{0:MM/yyyy}', '{0:MMM yyyy}', '{0:yyyyMMdd}'];
    $scope.outboundPaddingchars = ['', '*'];
    $rootScope.IsLoading = true;
    $scope.IsItemsSelected = true;
    $scope.IsOutboundItemsSelected = true;
    $scope.currentfile = $rootScope.currentopenfile.file;
    $scope.objFile = {};
    $scope.objFile.SelectedEntityObjectTreeFields = [];
    $scope.dummyEntityProperties = [];
    $scope.objFile.SelecetdField = undefined;
    $scope.objFile.LstDisplayedEntities = [];
    $scope.lstEntityNames = [];
    $scope.entityNameForEntityTree = "";
    $scope.selectedDesignSource = false;
    $scope.BusObjectName = "";
    $scope.objBusinessObject = {};
    $scope.objBusinessObject.ObjTree = undefined;
    //$scope.IsChildOfInboundFileBase = false;
    //#endregion

    //New BusinessObject Tree Fields 
    $scope.objBusinessObjectTree = {};
    $scope.objBusinessObjectTree.lstdisplaybusinessobject = [];
    $scope.objBusinessObjectTree.lstmultipleselectedfields = [];
    $scope.objBusinessObjectTree.lstCurrentBusinessObjectProperties = [];
    //End Region

    //#region On Load
    hubMain.server.getModel($scope.currentfile.FilePath, $scope.currentfile.FileType).done(function (model) {
        if (model) {
            $scope.receivefilemodel(model);
        }
        else {
            $rootScope.closeFile($scope.currentfile.FileName);
        }

    });

    $scope.receivefilemodel = function (data) {
        $scope.$apply(function () {
            $rootScope.IsLoading = false;
            $scope.objFileExtraFields = [];
            $scope.objFile = data;


            //$scope.IsChildOfInboundFileBase = $scope.objFile.objExtraData.IsChildOfInboundFileBase;
            $scope.lstObjectMethod = $scope.objFile.objExtraData.MethodNames;
            $scope.lstFileCollection = $scope.objFile.objExtraData.FieldNames;


            $scope.objFile.IsChildOfInboundFileBase = $scope.objFile.objExtraData.IsChildOfInboundFileBase;
            $scope.objFile.lstObjectMethod = $scope.objFile.objExtraData.MethodNames;
            $scope.objFile.lstFileCollection = $scope.objFile.objExtraData.FieldNames;

            if (!$scope.objFile.dictAttributes.sfwIsDelimited && $scope.objFile.IsChildOfInboundFileBase) {
                $scope.objFile.dictAttributes.sfwIsDelimited = "False";
            }

            $scope.objFile.lstFileCollection.push("");
            for (var i = 0; i < $scope.objFile.Elements.length > 0; i++) {
                if ($scope.objFile.Elements[i].Name == "record") {
                    $scope.SelectedRecordClick($scope.objFile.Elements[i]);
                    break;
                }
            }


            //Get Extra Field
            $scope.objFileExtraFields = $filter('filter')($scope.objFile.Elements, { Name: 'ExtraFields' });
            if ($scope.objFileExtraFields.length > 0) {
                $scope.objFileExtraFields = $scope.objFileExtraFields[0];
                //$scope.removeExtraFieldsDataInToMainModel();
            }

            if ($scope.objFileExtraFields.length == 0) {
                $scope.objFileExtraFields = {
                    Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
                };
            }

        });
        $scope.SetDummyRecordTypeAndHeaderObjectID();
    };
    //#endregion
    // used for find in design - does not support excel matrix
    $scope.ToggleUIDesginAttribute = function (items, IsAction) {
        if (items.length > 0 && items[items.length - 1].Name != 'ExtraFields') {
            items[items.length - 1].isAdvanceSearched = IsAction;
        }
    };
    $scope.traverseMainModel = function (path) {
        var items = [], objHierarchy;
        if (path.contains("-") || path.contains(",")) {
            objHierarchy = $scope.objFile;
            for (var i = 0; i < path.split(',').length; i++) {
                objHierarchy = $scope.FindNodeHierarchy(objHierarchy, path.split(',')[i].substring(path.split(',')[i].lastIndexOf('-') + 1));
                if (objHierarchy) {
                    items.push(objHierarchy);
                }
            }
        }
        return items;
    };
    $scope.selectElement = function (path) {
        if (path != null && path != "file" && path != "") {
            var items = $scope.traverseMainModel(path);
            var idSelectedDiv = ""; // for scrolling the selected item to view
            if (items.length > 0 && items[0].Name == 'record') {
                $scope.SelectedRecordClick(items[0]);
                $scope.ActiveTabForFile = 'Items';
                idSelectedDiv = "s3-file-record-list";
            }
            if (items.length > 1 && items[items.length - 1] && items[items.length - 1].Name == "sfwField") {
                $scope.selectedRecordRowClick(items[items.length - 1]);
                idSelectedDiv = "s3-file-field-list";
            }
            if (items.length > 0) {
                setTimeout(function () {
                    if (idSelectedDiv) {
                        var elem = $("#" + $scope.currentfile.FileName + " #" + idSelectedDiv).find(".selected");
                        if (elem && path != null && path != "file" && path != "") {
                            if (elem.length > 0) {
                                elem[0].scrollIntoView();
                            }
                        }
                    }
                }, 500);
            }
        }
        $rootScope.IsLoading = false;
    };
    //#region Methods for Design To source and Source To Design 
    $scope.ShowDesign = function () {
        if ($scope.selectedDesignSource == true) {
            $scope.showSource = false;
            var xmlstring = $scope.editor.getValue();

            if (xmlstring != null && xmlstring != "") {
                $rootScope.IsLoading = true;
                var lineno = $scope.editor.selection.getCursor().row;
                lineno = lineno + 1;

                if (xmlstring.length < 32000) {
                    hubMain.server.getDesignXmlString(xmlstring, $scope.currentfile, lineno);
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

                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Source-Design", lineNumber);
                }
                $scope.receivefiledesignxmlobject = function (data, path) {
                    $scope.selectedDesignSource = false;
                    if ($scope.isSourceDirty) {
                        $scope.receivefilemodel(data);
                        $scope.isSourceDirty = false;
                    }
                    //else {
                    //    $scope.removeExtraFieldsDataInToMainModel();
                    //}

                    // navigate and highlight the node (path)
                    $scope.$evalAsync($scope.selectElement(path));
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
    var SendDataPacketsToServer = function (lstpackets, filedetails, operationtoperform, nodeId) {
        for (var i = 0; i < lstpackets.length; i++) {
            hubMain.server.receiveDataPackets(lstpackets[i], lstpackets.length, filedetails, i, operationtoperform, nodeId);
        }
    };
    $scope.ShowSource = function () {
        if ($scope.selectedDesignSource == false) {
            $scope.selectedDesignSource = true;
            var sObj;
            var indexPath = [];
            var pathString;

            if ($scope.objFile) {
                $rootScope.IsLoading = true;
                //$scope.addExtraFieldsDataInToMainModel();
                var objreturn1 = GetBaseModel($scope.objFile);
                var strobj = JSON.stringify(objreturn1);
                var nodeId = [];
                var selectedFileItem;

                if ($scope.selectedCurrentRecord && $scope.selectedCurrentRecordRow == undefined) {
                    selectedFileItem = $scope.selectedCurrentRecord;
                }
                else {
                    selectedFileItem = $scope.selectedCurrentRecordRow;
                }
                if (selectedFileItem) {
                    //sObj = FindDeepNode($scope.objFile, selectedFileItem);
                    //pathString = getPathSource(sObj, indexPath);
                    //angular.copy(pathString.reverse(), nodeId);

                    var pathToObject = [];

                    sObj = $scope.FindDeepNode($scope.objFile, selectedFileItem, pathToObject);
                    pathString = $scope.getPathSource($scope.objFile, pathToObject, indexPath);
                    angular.copy(pathString, nodeId);
                }

                if (strobj.length < 32000) {
                    hubMain.server.getSourceXmlObject(strobj, $scope.currentfile, nodeId);
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
                    SendDataPacketsToServer(lstDataPackets, $scope.currentfile, "Design-Source", nodeId);
                }
                $scope.receivesourcexml = function (xmlstring, lineno) {
                    $scope.$apply(function () {
                        $scope.xmlSource = xmlstring;
                        var ID = $scope.currentfile.FileName;
                        setDataToEditor($scope, xmlstring, lineno, ID);
                        $scope.$evalAsync(function () {
                            $rootScope.IsLoading = false;
                            $scope.selectedCurrentRecordRow = undefined;
                        });
                        if (window.navigator.userAgent.toLowerCase().contains("chrome")) {
                            $rootScope.IsLoading = false;
                        }
                    });
                };
            }
        }
    };

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
    //#endregion

    //#region 
    $scope.SetDummyRecordTypeAndHeaderObjectID = function () {
        if ($scope.objFile.Elements.length > 0) {
            function SetDummyValue(obj) {
                if (obj.Name == 'record') {
                    obj.DummyRecordType = obj.dictAttributes.sfwRecordType;
                    if ($scope.objFile && $scope.objFile.dictAttributes.sfwType == "Inbound") {
                        obj.DummyHeaderEntity = obj.dictAttributes.sfwHeaderEntity;
                    }
                }
            }
            angular.forEach($scope.objFile.Elements, SetDummyValue);
        }
    };
    //#endregion

    // #region Inbound File

    $scope.onDetailClick = function () {
        var newScope = $scope.$new();
        // Extra field variables
        newScope.objExtraFields = [];
        newScope.objDirFunctions = {};
        newScope.showExtraFieldsTab = false;
        newScope.formName = "File";

        if ($scope.objFile.dictAttributes.sfwType == "Inbound") {
            newScope.objInboundFile = $scope.objFile;
            newScope.objInboundDetails = {};
            newScope.objInboundDetails.dictAttributes = {};
            newScope.objInboundDetails.IsChildOfInboundFileBase = newScope.objInboundFile.IsChildOfInboundFileBase;
            newScope.objInboundDetails.dictAttributes.ID = newScope.objInboundFile.dictAttributes.ID;
            newScope.objInboundDetails.dictAttributes.sfwInboundObject = newScope.objInboundFile.dictAttributes.sfwInboundObject;
            newScope.objInboundDetails.dictAttributes.sfwDelimitedBy = newScope.objInboundFile.dictAttributes.sfwDelimitedBy;
            newScope.objInboundDetails.dictAttributes.sfwDegreeOfParallelism = newScope.objInboundFile.dictAttributes.sfwDegreeOfParallelism;
            newScope.objInboundDetails.dictAttributes.sfwThresholdCommit = newScope.objInboundFile.dictAttributes.sfwThresholdCommit;
            newScope.objInboundDetails.dictAttributes.sfwRecieveToFileDetail = newScope.objInboundFile.dictAttributes.sfwRecieveToFileDetail;
            newScope.objInboundDetails.dictAttributes.sfwProcessInParallel = newScope.objInboundFile.dictAttributes.sfwProcessInParallel;
            newScope.objInboundDetails.dictAttributes.sfwCommitSingle = newScope.objInboundFile.dictAttributes.sfwCommitSingle;
            newScope.objInboundDetails.dictAttributes.sfwProcessDetail = newScope.objInboundFile.dictAttributes.sfwProcessDetail;
            newScope.objInboundDetails.dictAttributes.sfwDescription = newScope.objInboundFile.dictAttributes.sfwDescription;
            newScope.objInboundDetails.dictAttributes.sfwTitle = newScope.objInboundFile.dictAttributes.sfwTitle;
            newScope.objInboundDetails.dictAttributes.sfwIsDelimited = newScope.objInboundFile.dictAttributes.sfwIsDelimited;
            newScope.objInboundDetails.dictAttributes.sfwProgressCounter = newScope.objInboundFile.dictAttributes.sfwProgressCounter;
            newScope.objInboundDetails.dictAttributes.sfwActive = newScope.objInboundFile.dictAttributes.sfwActive;
            newScope.objInboundDetails.dictAttributes.sfwUploadInParallel = newScope.objInboundFile.dictAttributes.sfwUploadInParallel;
            newScope.objInboundDetails.dictAttributes.sfwUploadToFileDetail = newScope.objInboundFile.dictAttributes.sfwUploadToFileDetail;
            newScope.objInboundDetails.dictAttributes.sfwGetGroupValueFromHeader = newScope.objInboundFile.dictAttributes.sfwGetGroupValueFromHeader;
            newScope.objInboundDetails.dictAttributes.sfwEncodingType = newScope.objInboundFile.dictAttributes.sfwEncodingType;

            newScope.dialogOptions = { height: 700, width: 700 };
            newScope.templateName = "File/views/InboundFileDetails.html";

        } else {
            newScope.objOutboundFile = $scope.objFile;
            newScope.objOutBound = {};
            newScope.objOutBound.dictAttributes = {};
            newScope.objOutBound.dictAttributes.sfwType = newScope.objOutboundFile.dictAttributes.sfwType;
            newScope.objOutBound.dictAttributes.ID = newScope.objOutboundFile.dictAttributes.ID;
            newScope.objOutBound.dictAttributes.sfwOutboundObject = newScope.objOutboundFile.dictAttributes.sfwOutboundObject;
            newScope.objOutBound.dictAttributes.sfwLoadMethod = newScope.objOutboundFile.dictAttributes.sfwLoadMethod;
            newScope.objOutBound.dictAttributes.sfwFileName = newScope.objOutboundFile.dictAttributes.sfwFileName;
            newScope.objOutBound.dictAttributes.sfwDescription = newScope.objOutboundFile.dictAttributes.sfwDescription;
            newScope.objOutBound.dictAttributes.sfwTitle = newScope.objOutboundFile.dictAttributes.sfwTitle;
            newScope.objOutBound.dictAttributes.sfwQueryID = newScope.objOutboundFile.dictAttributes.sfwQueryID;
            newScope.objOutBound.dictAttributes.sfwProgressCounter = newScope.objOutboundFile.dictAttributes.sfwProgressCounter;
            newScope.objOutBound.lstFileCollection = newScope.objOutboundFile.lstFileCollection;
            newScope.objOutBound.lstObjectMethod = newScope.objOutboundFile.lstObjectMethod;
            newScope.objOutBound.dictAttributes.sfwXMLType = newScope.objOutboundFile.dictAttributes.sfwXMLType;
            newScope.objOutBound.dictAttributes.sfwFileNameMapper = newScope.objOutboundFile.dictAttributes.sfwFileNameMapper;



            newScope.dialogOptions = { height: 700, width: 700 };
            if (newScope.objOutboundFile.dictAttributes.sfwCollection) {
                for (var i = 0; i < newScope.objOutboundFile.lstFileCollection.length; i++) {
                    var arr = newScope.objOutboundFile.lstFileCollection[i].split(":");
                    if (arr[0] == newScope.objOutboundFile.dictAttributes.sfwCollection) {
                        newScope.objOutBound.dictAttributes.sfwCollection = newScope.objOutboundFile.lstFileCollection[i];
                    }
                }
            }
            newScope.templateName = "File/views/OutboundFileDetails.html";
            //newScope.AddOnlyPropertytoObject = function (sfwCollection) {
            //    if (sfwCollection && sfwCollection != null) {
            //        var arr = sfwCollection.split(":");
            //        if (sfwCollection != "") {
            //            newScope.objOutboundFile.dictAttributes.sfwCollection = arr[0];
            //        } else {
            //            newScope.objOutboundFile.dictAttributes.sfwCollection = "";
            //        }
            //    }
            //    else {
            //        newScope.objOutboundFile.dictAttributes.sfwCollection = "";
            //    }
            //};

            newScope.onChangeBusinessObject = function () {
                $scope.lstobjecttree = [];
                $scope.objBusinessObject.ObjTree = undefined;
                if ($scope.selectedCurrentRecord) {
                    $rootScope.EditPropertyValue($scope.selectedCurrentRecord.sfwRecordObject, $scope.selectedCurrentRecord, "sfwRecordObject", "");
                }
                // newScope.objOutBound.dictAttributes.sfwCollection = "";

                if (newScope.objOutboundFile.BusinessObject && newScope.objOutboundFile.BusinessObject.length > 0) {
                    if (newScope.objOutboundFile.dictAttributes && newScope.objOutboundFile.dictAttributes.sfwOutboundObject) {
                        lst = newScope.objOutboundFile.dictAttributes.sfwOutboundObject.split(".");
                        if ($scope.selectedCurrentRecord && $scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "AutoSkip" && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID) {
                            $rootScope.EditPropertyValue($scope.BusObjectName, $scope, "BusObjectName", $scope.selectedCurrentRecord.dictAttributes.sfwObjectID);
                        }
                        else if (lst.length > 0) {
                            $rootScope.EditPropertyValue($scope.BusObjectName, $scope, "BusObjectName", lst[lst.length - 1]);
                        }
                    }
                } else {
                    $rootScope.EditPropertyValue($scope.BusObjectName, $scope, "BusObjectName", "");

                    if ($scope.selectedCurrentRecord && $scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "AutoSkip" && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID) {
                        $rootScope.EditPropertyValue($scope.BusObjectName, $scope, "BusObjectName", $scope.selectedCurrentRecord.dictAttributes.sfwObjectID);

                    }
                }

            };
        }

        if (newScope.templateName && newScope.templateName.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, 'Details', newScope.templateName, newScope.dialogOptions);
        }

        newScope.validateFileDetails = function () {
            newScope.FileDetailsErrorMessage = "";
            if (newScope.objDirFunctions.getExtraFieldData) {
                newScope.objExtraFields = newScope.objDirFunctions.getExtraFieldData(); // getting extra field data from extraFieldDirective
            }

            var flag = validateExtraFields(newScope);
            newScope.FileDetailsErrorMessage = newScope.FormDetailsErrorMessage;
            return flag;
        };
        newScope.onProcessInParallelChanged = function () {
            if (newScope.objInboundDetails.dictAttributes.sfwProcessInParallel == "True") {
                newScope.objInboundDetails.dictAttributes.sfwCommitSingle = "False";
            }
        };

        newScope.OnInboundObjectIdChange = function () {
            if (newScope.objInboundDetails) {
                //$rootScope.UndRedoBulkOp("Start");
                if (newScope.objInboundDetails.IsChildOfInboundFileBase) {
                    newScope.objInboundDetails.dictAttributes.sfwUploadToFileDetail = "True";

                    newScope.objInboundDetails.dictAttributes.sfwCommitSingle = "True";
                    newScope.objInboundDetails.dictAttributes.sfwProcessDetail = "True";
                    if (!newScope.objInboundDetails.dictAttributes.sfwIsDelimited) {
                        newScope.objInboundDetails.dictAttributes.sfwIsDelimited = "False";
                    }
                }
                else {
                    newScope.objInboundDetails.dictAttributes.sfwRecieveToFileDetail = "False";
                    newScope.objInboundDetails.dictAttributes.sfwUploadInParallel = "False";
                    newScope.objInboundDetails.dictAttributes.sfwUploadToFileDetail = "False";
                    newScope.objInboundDetails.dictAttributes.sfwGetGroupValueFromHeader = "False";
                    newScope.objInboundDetails.dictAttributes.sfwProcessInParallel = "False";
                    newScope.objInboundDetails.dictAttributes.sfwCommitSingle = "False";
                }
                //$rootScope.UndRedoBulkOp("End");
            }
        };

        newScope.okClick = function () {
            alert("Updation of file details will update the XML file only. For DB update, user should do it manually.");

            if ($scope.objFile.dictAttributes.sfwType != "Inbound") {
                $scope.objFile.dictAttributes.sfwType = newScope.objOutBound.dictAttributes.sfwType;
                $scope.objFile.dictAttributes.ID = newScope.objOutBound.dictAttributes.ID;
                $rootScope.UndRedoBulkOp("Start");
                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwLoadMethod, $scope.objFile.dictAttributes, "sfwLoadMethod", newScope.objOutBound.dictAttributes.sfwLoadMethod);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwFileName, $scope.objFile.dictAttributes, "sfwFileName", newScope.objOutBound.dictAttributes.sfwFileName);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwDescription, $scope.objFile.dictAttributes, "sfwDescription", newScope.objOutBound.dictAttributes.sfwDescription);
                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwXMLType, $scope.objFile.dictAttributes, "sfwXMLType", newScope.objOutBound.dictAttributes.sfwXMLType);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwTitle, $scope.objFile.dictAttributes, "sfwTitle", newScope.objOutBound.dictAttributes.sfwTitle);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwQueryID, $scope.objFile.dictAttributes, "sfwQueryID", newScope.objOutBound.dictAttributes.sfwQueryID);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwProgressCounter, $scope.objFile.dictAttributes, "sfwProgressCounter", newScope.objOutBound.dictAttributes.sfwProgressCounter);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwFileNameMapper, $scope.objFile.dictAttributes, "sfwFileNameMapper", newScope.objOutBound.dictAttributes.sfwFileNameMapper);

                $rootScope.EditPropertyValue($scope.objFile.lstFileCollection, $scope.objFile, "lstFileCollection", newScope.objOutBound.lstFileCollection);

                $rootScope.EditPropertyValue($scope.objFile.lstObjectMethod, $scope.objFile, "lstObjectMethod", newScope.objOutBound.lstObjectMethod);

                if (newScope.objOutboundFile.dictAttributes.sfwOutboundObject != newScope.objOutBound.dictAttributes.sfwOutboundObject) {
                    $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwOutboundObject, $scope.objFile.dictAttributes, "sfwOutboundObject", newScope.objOutBound.dictAttributes.sfwOutboundObject);

                    $rootScope.EditPropertyValue($scope.objFile.BusinessObject, $scope.objFile, "BusinessObject", newScope.objOutBound.BusinessObject);
                    newScope.onChangeBusinessObject();
                }

                if (newScope.objOutBound.dictAttributes.sfwCollection && newScope.objOutBound.dictAttributes.sfwCollection != null) {
                    var arr = newScope.objOutBound.dictAttributes.sfwCollection.split(":");
                    if (arr.length > 0) {
                        $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwCollection, $scope.objFile.dictAttributes, "sfwCollection", arr[0]);

                    } else {
                        $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwCollection, $scope.objFile.dictAttributes, "sfwCollection", "");
                    }
                }
                else {
                    $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwCollection, $scope.objFile.dictAttributes, "sfwCollection", "");
                }

                $rootScope.UndRedoBulkOp("End");
            } else {
                $scope.objFile.dictAttributes.ID = newScope.objInboundDetails.dictAttributes.ID;
                $rootScope.UndRedoBulkOp("Start");

                $rootScope.EditPropertyValue($scope.objFile.IsChildOfInboundFileBase, $scope.objFile, "IsChildOfInboundFileBase", newScope.objInboundDetails.IsChildOfInboundFileBase);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwInboundObject, $scope.objFile.dictAttributes, "sfwInboundObject", newScope.objInboundDetails.dictAttributes.sfwInboundObject);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwDelimitedBy, $scope.objFile.dictAttributes, "sfwDelimitedBy", newScope.objInboundDetails.dictAttributes.sfwDelimitedBy);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwDegreeOfParallelism, $scope.objFile.dictAttributes, "sfwDegreeOfParallelism", newScope.objInboundDetails.dictAttributes.sfwDegreeOfParallelism);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwThresholdCommit, $scope.objFile.dictAttributes, "sfwThresholdCommit", newScope.objInboundDetails.dictAttributes.sfwThresholdCommit);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwRecieveToFileDetail, $scope.objFile.dictAttributes, "sfwRecieveToFileDetail", newScope.objInboundDetails.dictAttributes.sfwRecieveToFileDetail);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwProcessInParallel, $scope.objFile.dictAttributes, "sfwProcessInParallel", newScope.objInboundDetails.dictAttributes.sfwProcessInParallel);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwCommitSingle, $scope.objFile.dictAttributes, "sfwCommitSingle", newScope.objInboundDetails.dictAttributes.sfwCommitSingle);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwProcessDetail, $scope.objFile.dictAttributes, "sfwProcessDetail", newScope.objInboundDetails.dictAttributes.sfwProcessDetail);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwDescription, $scope.objFile.dictAttributes, "sfwDescription", newScope.objInboundDetails.dictAttributes.sfwDescription);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwTitle, $scope.objFile.dictAttributes, "sfwTitle", newScope.objInboundDetails.dictAttributes.sfwTitle);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwIsDelimited, $scope.objFile.dictAttributes, "sfwIsDelimited", newScope.objInboundDetails.dictAttributes.sfwIsDelimited);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwProgressCounter, $scope.objFile.dictAttributes, "sfwProgressCounter", newScope.objInboundDetails.dictAttributes.sfwProgressCounter);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwActive, $scope.objFile.dictAttributes, "sfwActive", newScope.objInboundDetails.dictAttributes.sfwActive);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwUploadInParallel, $scope.objFile.dictAttributes, "sfwUploadInParallel", newScope.objInboundDetails.dictAttributes.sfwUploadInParallel);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwUploadToFileDetail, $scope.objFile.dictAttributes, "sfwUploadToFileDetail", newScope.objInboundDetails.dictAttributes.sfwUploadToFileDetail);

                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwGetGroupValueFromHeader, $scope.objFile.dictAttributes, "sfwGetGroupValueFromHeader", newScope.objInboundDetails.dictAttributes.sfwGetGroupValueFromHeader);
                $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwEncodingType, $scope.objFile.dictAttributes, "sfwEncodingType", newScope.objInboundDetails.dictAttributes.sfwEncodingType);
                $rootScope.UndRedoBulkOp("End");
            }
            // #region extra field data
            if (newScope.objDirFunctions.prepareExtraFieldData) {
                newScope.objDirFunctions.prepareExtraFieldData();// calling extraFieldDirective function for getting extra field data
            }
            //#endregion
            if (newScope.dialog) {
                newScope.dialog.close();
            }
            $scope.recordQueryChange();
        };
    };
    $scope.onChangeBusinessObject = function () {
        $scope.lstobjecttree = [];
        $scope.objBusinessObject.ObjTree = undefined;
        if ($scope.selectedCurrentRecord) {
            $scope.selectedCurrentRecord.sfwRecordObject = "";
        }
        if ($scope.selectedCurrentRecord && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID != "") {
            $scope.BusObjectName = $scope.selectedCurrentRecord.dictAttributes.sfwObjectID;
        }
        else {
            if ($scope.objFile.dictAttributes && $scope.objFile.dictAttributes.sfwOutboundObject) {
                lst = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                if (lst.length > 0) {
                    $scope.BusObjectName = lst[lst.length - 1];
                }
            }
        }
    };
    $scope.AddonlyObjectIDtoObject = function (sfwRecordObject) {
        if ($scope.selectedCurrentRecord) {
            if (!sfwRecordObject) {
                $scope.selectedCurrentRecord.dictAttributes.sfwRecordObject = "";
            }
            if ($scope.selectedCurrentRecord && sfwRecordObject && sfwRecordObject.contains(":")) {
                var arr = sfwRecordObject.split(":");

                $scope.selectedCurrentRecord.dictAttributes.sfwRecordObject = arr[0];
                var objectID = sfwRecordObject.substring(sfwRecordObject.lastIndexOf("[") + 1, sfwRecordObject.lastIndexOf("]"));
                $scope.BusObjectName = objectID;
            }
            else {
                $scope.lstobjecttree = [];
                $scope.objBusinessObject.ObjTree = undefined;
                if ($scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwOutboundObject != "") {
                    var tempArra = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                    $scope.BusObjectName = tempArra[tempArra.length - 1];
                }
            }
        }
    };

    // click on selected Records
    $scope.SelectedRecordClick = function (obj) {
        //   $scope.selectedRecord = index;
        $scope.selectedCurrentRecordRow = undefined;
        $scope.selectedCurrentRecord = obj;
        $scope.objFile.SelecetdField = undefined;
        $scope.AddEntityIDinList(obj, false);
        $scope.entityNameForEntityTree = "";

        //if detail entity is set then pre select that entity or select header entity if it is selected
        if (obj && obj.dictAttributes.sfwHeaderEntity && obj.dictAttributes.sfwHeaderEntity != "") {
            $scope.entityNameForEntityTree = obj.dictAttributes.sfwHeaderEntity;
        }
        if (obj && obj.dictAttributes.sfwDetailEntity && obj.dictAttributes.sfwDetailEntity != "") {
            $scope.entityNameForEntityTree = obj.dictAttributes.sfwDetailEntity;
        }

        $scope.objBusinessObject.ObjTree = undefined;
        $scope.lstobjecttree = [];
        if ($scope.objFile.dictAttributes.sfwType != "Inbound" && $scope.selectedCurrentRecord) {

            if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID) {
                if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "AutoSkip" && $scope.selectedCurrentRecord.dictAttributes.sfwObjectID != "") {
                    $scope.BusObjectName = $scope.selectedCurrentRecord.dictAttributes.sfwObjectID;
                }
            }
            else if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType && $scope.selectedCurrentRecord.dictAttributes.sfwRecordType != "AutoSkip" && !$scope.selectedCurrentRecord.dictAttributes.sfwRecordObject) {
                if ($scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwOutboundObject != "") {
                    var tempArra = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                    $scope.BusObjectName = tempArra[tempArra.length - 1];
                }
            }
            else if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordObject) {
                for (var i = 0; i < $scope.objFile.lstFileCollection.length; i++) {
                    var arr = $scope.objFile.lstFileCollection[i].split(":");
                    if (arr[0] == $scope.selectedCurrentRecord.dictAttributes.sfwRecordObject) {
                        $scope.selectedCurrentRecord.sfwRecordObject = $scope.objFile.lstFileCollection[i];
                        var objectID = $scope.objFile.lstFileCollection[i].substring($scope.objFile.lstFileCollection[i].lastIndexOf("[") + 1, $scope.objFile.lstFileCollection[i].lastIndexOf("]"));
                        $scope.BusObjectName = objectID;
                        break;
                    }
                }
            } else {
                if ($scope.selectedCurrentRecord) {
                    $scope.selectedCurrentRecord.sfwRecordObject = "";
                    $scope.BusObjectName = "";
                    if ($scope.objFile.dictAttributes.sfwOutboundObject) {
                        lst = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                        if (lst.length > 0) {
                            $scope.BusObjectName = lst[lst.length - 1];
                        }
                    }
                }
            }
        }
        $scope.recordQueryChange();
    };

    $scope.AddEntityIDinList = function (obj, aIsCallHeaderChange) {
        $scope.lstEntityNames = [];
        if (aIsCallHeaderChange) {
            $scope.OnInboundHeaderIdChanged(event);
        }
        if (obj) {
            obj.errors = "";
            if (obj.dictAttributes.sfwHeaderEntity && obj.dictAttributes.sfwHeaderEntity != "") {
                $scope.lstEntityNames.push(obj.dictAttributes.sfwHeaderEntity);
            }
            if (obj.dictAttributes.sfwDetailEntity && obj.dictAttributes.sfwDetailEntity != "") {
                $scope.lstEntityNames.push(obj.dictAttributes.sfwDetailEntity);
            }
            if (obj.dictAttributes.sfwHeaderEntity && obj.dictAttributes.sfwDetailEntity && obj.dictAttributes.sfwHeaderEntity === obj.dictAttributes.sfwDetailEntity) {
                obj.errors = "Header entity and detail entity can not be same";
            }
        }
    };

    // click on Items Tab
    $scope.onItemsClick = function () {
        $scope.IsItemsSelected = true;
        $scope.IsEntityTreeSelected = false;
    };
    // Click on Object Tree Tab
    $scope.onEntityTreeClick = function () {
        $scope.IsItemsSelected = false;
        $scope.IsEntityTreeSelected = true;
    };

    // on click of particular Record Row
    $scope.selectedRecordRowClick = function (obj) {
        if (obj) {
            $scope.selectedCurrentRecordRow = obj;
        }
    };

    // delete selected column details
    $scope.deleteSelectedRow = function () {
        var Fieldindex = -1;
        if ($scope.selectedCurrentRecordRow) {
            Fieldindex = $scope.selectedCurrentRecord.Elements.indexOf($scope.selectedCurrentRecordRow);
            if (Fieldindex > -1) {
                var id = "";
                if ($scope.selectedCurrentRecordRow.dictAttributes.sfwEntityField) {
                    id = $scope.selectedCurrentRecordRow.dictAttributes.sfwEntityField;
                } else if ($scope.objFile.dictAttributes.sfwType == "Outbound") {
                    id = $scope.selectedCurrentRecordRow.dictAttributes.sfwObjectField;
                }
                if (confirm("FieldID : '" + id + "'" + "  " + " will be deleted, Do you wan.t to continue?")) {
                    $rootScope.DeleteItem($scope.selectedCurrentRecord.Elements[Fieldindex], $scope.selectedCurrentRecord.Elements);
                    $scope.selectedCurrentRecordRow = undefined;

                    if (Fieldindex < $scope.selectedCurrentRecord.Elements.length) {

                        $scope.selectedCurrentRecordRow = $scope.selectedCurrentRecord.Elements[Fieldindex];
                    }
                    else if ($scope.selectedCurrentRecord.Elements.length > 0) {
                        $scope.selectedCurrentRecordRow = $scope.selectedCurrentRecord.Elements[Fieldindex - 1];
                    }
                    $scope.isDirty = true;
                }
            }
        }
    };

    // disable if there is no element for SFW row
    $scope.canDeleteRow = function () {
        if ($scope.selectedCurrentRecordRow) {
            return true;
        }
        else {
            return false;
        }
    };
    // Move up functionality for Row from Record Layout
    $scope.moveSelectedRowUp = function () {
        if ($scope.selectedCurrentRecordRow) {

            var index = $scope.selectedCurrentRecord.Elements.indexOf($scope.selectedCurrentRecordRow);
            var dummy = $scope.selectedCurrentRecordRow;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCurrentRecordRow, $scope.selectedCurrentRecord.Elements);
            $rootScope.InsertItem(dummy, $scope.selectedCurrentRecord.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollBySelection(".field-binding-table", ".selected");
        }
    };

    // disable the move up button if there is no element to move up
    $scope.canmoveSelectedRowUp = function () {
        $scope.Flag = true;
        if ($scope.selectedCurrentRecordRow != undefined) {
            for (var i = 0; i < $scope.selectedCurrentRecord.Elements.length; i++) {
                if ($scope.selectedCurrentRecord.Elements[i] == $scope.selectedCurrentRecordRow) {
                    if (i > 0) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };

    // Move Down function for Row from Record Layout

    $scope.moveSelectedRowDown = function () {
        if ($scope.selectedCurrentRecordRow && $scope.selectedCurrentRecord.Elements.length > 0) {
            var index = $scope.selectedCurrentRecord.Elements.indexOf($scope.selectedCurrentRecordRow);
            var dummy = $scope.selectedCurrentRecordRow;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCurrentRecordRow, $scope.selectedCurrentRecord.Elements);
            $rootScope.InsertItem(dummy, $scope.selectedCurrentRecord.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollBySelection(".field-binding-table", ".selected");
        }
    };


    // disable move down when there is no element to move down
    $scope.canmoveSelectedRowDown = function () {
        $scope.Flag = true;
        if ($scope.selectedCurrentRecordRow != undefined && $scope.selectedCurrentRecord.Elements.length > 0) {
            for (var i = 0; i < $scope.selectedCurrentRecord.Elements.length; i++) {
                if ($scope.selectedCurrentRecord.Elements[i] == $scope.selectedCurrentRecordRow) {
                    if (i < $scope.selectedCurrentRecord.Elements.length - 1) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };


    $scope.addNewRecord = function () {
        var obj = { Name: 'record', value: '', dictAttributes: { ID: "New Layout" }, Elements: [] };
        $rootScope.PushItem(obj, $scope.objFile.Elements, "SelectedRecordClick");
        $scope.SelectedRecordClick(obj);

    };

    // deleting record from Recordlist
    $scope.deleteRecord = function () {
        if ($scope.selectedCurrentRecord) {
            var lstrecordElement = $scope.objFile.Elements.filter(function (item) {
                return item.Name == 'record';
            });
            if (lstrecordElement.length > 1) {
                var Fieldindex = -1;
                Fieldindex = $scope.objFile.Elements.indexOf($scope.selectedCurrentRecord);
                if (Fieldindex > -1) {
                    if (confirm("RecordID : '" + $scope.selectedCurrentRecord.dictAttributes.ID + "'" + "  " + " will be deleted, Do you want to continue?")) {
                        $rootScope.DeleteItem($scope.objFile.Elements[Fieldindex], $scope.objFile.Elements, "SelectedRecordClick");
                        $scope.selectedCurrentRecord = undefined;
                        $scope.selectedCurrentRecordRow = undefined;
                        if ($scope.objFile.Elements.length > 0) {
                            for (var i = Fieldindex; ;) {
                                if (Fieldindex == 0) {
                                    if (i < $scope.objFile.Elements.length) {
                                        if ($scope.objFile.Elements[i] && $scope.objFile.Elements[i].Name == 'record') {
                                            //$scope.selectedCurrentRecord = $scope.objFile.Elements[i];
                                            $scope.SelectedRecordClick($scope.objFile.Elements[i]);
                                            break;
                                        }
                                    }
                                    else {
                                        break;
                                    }
                                    i++;
                                }
                                else {
                                    if (i >= 0) {
                                        if ($scope.objFile.Elements[i]) {
                                            if ($scope.objFile.Elements[i].Name == 'record') {
                                                //$scope.selectedCurrentRecord = $scope.objFile.Elements[i];
                                                $scope.SelectedRecordClick($scope.objFile.Elements[i]);
                                                break;
                                            }
                                        }
                                        // if last element is deleted
                                        else {
                                            if ($scope.objFile.Elements[i - 1].Name == 'record') {
                                                //$scope.selectedCurrentRecord = $scope.objFile.Elements[i - 1];
                                                $scope.SelectedRecordClick($scope.objFile.Elements[i - 1]);
                                                break;
                                            }
                                        }
                                    }
                                    else {
                                        break;
                                    }
                                    i--;
                                }
                            }
                        }
                        else {
                            $scope.selectedCurrentRecord = undefined;
                        }
                        $scope.selectedRecord = Fieldindex;
                    }
                }
            }
            else {
                alert("Atleast one record should be present.");
            }
        }
    };

    // Move up functionality for Row from Record Layout
    $scope.moveLayoutSelectedRowUp = function () {
        if ($scope.selectedCurrentRecord) {
            var index = $scope.objFile.Elements.indexOf($scope.selectedCurrentRecord);
            var dummy = $scope.selectedCurrentRecord;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCurrentRecord, $scope.objFile.Elements);
            $rootScope.InsertItem(dummy, $scope.objFile.Elements, index - 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollBySelection(".files-sidebar", ".active");
        }
    };

    // disable the move up button if there is no element to move up
    $scope.canmoveLayoutSelectedRowUp = function () {
        $scope.Flag = true;
        if ($scope.selectedCurrentRecord != undefined) {
            for (var i = 0; i < $scope.objFile.Elements.length; i++) {
                if ($scope.objFile.Elements[i] == $scope.selectedCurrentRecord) {
                    if (i > 0) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };

    // Move Down function for Row from Record Layout
    $scope.scrollBySelection = function (parentDiv, selectedElement) {
        $timeout(function () {
            var $divDom = $("#" + $rootScope.currentopenfile.file.FileName).find(parentDiv);
            if ($divDom && $divDom.hasScrollBar()) {
                $divDom.scrollTo($divDom.find(selectedElement), { offsetTop: 400, offsetLeft: 0 }, null);
                return false;
            }

        });
    }
    $scope.moveLayoutSelectedRowDown = function () {
        if ($scope.selectedCurrentRecord && $scope.objFile.Elements.length > 0) {
            var index = $scope.objFile.Elements.indexOf($scope.selectedCurrentRecord);
            var dummy = $scope.selectedCurrentRecord;

            $rootScope.UndRedoBulkOp("Start");
            $rootScope.DeleteItem($scope.selectedCurrentRecord, $scope.objFile.Elements);
            $rootScope.InsertItem(dummy, $scope.objFile.Elements, index + 1);
            $rootScope.UndRedoBulkOp("End");
            $scope.scrollBySelection(".files-sidebar", ".active");
        }
    };


    // disable move down when there is no element to move down
    $scope.canmoveLayoutSelectedRowDown = function () {
        $scope.Flag = true;
        if ($scope.selectedCurrentRecord != undefined && $scope.objFile.Elements.length > 0) {
            for (var i = 0; i < $scope.objFile.Elements.length; i++) {
                if ($scope.objFile.Elements[i] == $scope.selectedCurrentRecord) {
                    if (i < $scope.objFile.Elements.length - 1) {
                        $scope.Flag = false;
                    }
                }
            }

        }

        return $scope.Flag;
    };
    // add new Filler Functionality

    $scope.addNewFiller = function () {
        var newValueField = {
        };
        newValueField.Value = '';
        newValueField.Name = 'sfwField';
        newValueField.Elements = [];
        if ($scope.objFile.dictAttributes.sfwType == "Inbound") {
            newValueField.dictAttributes = {
                sfwDescription: 'Filler', sfwEntityField: 'Filler', sfwDataFormat: '', sfwBindHeader: '', sfwCodeID: '', sfwRequired: ''
            };
        }
        else {
            newValueField.dictAttributes = {
                sfwDescription: 'Filler', sfwObjectField: 'Filler', sfwDataFormat: '', sfwRemoveDecimal: '', sfwConstant: '', sfwPaddingCharacter: '', sfwEndNode: '', sfwStartNode: ''
            };
        }

        if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType != "CommaDelimited") {
            newValueField.dictAttributes.sfwLength = '0';
        }

        $rootScope.PushItem(newValueField, $scope.selectedCurrentRecord.Elements, "selectedRecordRowClick");
        $scope.selectedRecordRowClick(newValueField);
    };

    $scope.addNewConstant = function () {
        var newValueField = {
        };
        newValueField.Value = '';
        newValueField.Name = 'sfwField';
        newValueField.Elements = [];
        newValueField.dictAttributes = {
            sfwDescription: '', sfwLength: '0', sfwObjectField: '', sfwDataFormat: '', sfwRemoveDecimal: '', sfwConstant: '', sfwPaddingCharacter: '', sfwEndNode: '', sfwStartNode: ''
        };
        $rootScope.PushItem(newValueField, $scope.selectedCurrentRecord.Elements, "selectedRecordRowClick");
        $scope.selectedRecordRowClick(newValueField);
    };

    $scope.addNewFields = function () {
        if ($scope.objFile.dictAttributes.sfwType == "Inbound") {
            if ($scope.objFile.SelectedEntityObjectTreeFields && $scope.objFile.SelectedEntityObjectTreeFields.length > 0) {
                var selectedEntityFields = $scope.objFile.SelectedEntityObjectTreeFields.filter(function (item) { return item.IsSelected && item.IsSelected.toLowerCase() == "true"; });
                if (selectedEntityFields && selectedEntityFields.length > 0) {
                    for (var index = 0; index < selectedEntityFields.length; index++) {
                        if (selectedEntityFields[index].IsMainEntity) {
                            alert(String.format("'{0}' field cannot be added as it is entity.", selectedEntityFields[index].ID));
                        }
                        else if (selectedEntityFields[index].Type && selectedEntityFields[index].Type != "Object" && selectedEntityFields[index].Type != "Collection" && selectedEntityFields[index].Type != "List" && selectedEntityFields[index].Type != "CDOCollection") {
                            var DisplayedEntity = getDisplayedEntity($scope.objFile.LstDisplayedEntities);
                            var itempath = selectedEntityFields[index].ID;
                            if (DisplayedEntity && DisplayedEntity.strDisplayName != "") {
                                itempath = DisplayedEntity.strDisplayName + "." + selectedEntityFields[index].ID;
                            }
                            var EntityField = itempath;//GetItemPathForEntityObject(selectedEntityFields[index]);
                            var newValueField = {
                            };
                            var Description = selectedEntityFields[index].Value;
                            var bindHeader = "";
                            if ($scope.entityNameForEntityTree && $scope.selectedCurrentRecord && $scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity &&
                                $scope.entityNameForEntityTree == $scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity) bindHeader = "True";
                            Description = $scope.CapitalizeFirstLetterInWord(Description);
                            newValueField.Value = '';
                            newValueField.Name = 'sfwField';
                            newValueField.Elements = [];

                            newValueField.dictAttributes = {
                                sfwDescription: Description, sfwEntityField: EntityField, sfwDataFormat: '', sfwBindHeader: bindHeader, sfwCodeID: '', sfwRequired: ''
                            };

                            if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType != "CommaDelimited") {
                                newValueField.dictAttributes.sfwLength = '0';
                            }

                            if (endsWith(selectedEntityFields[index].Value, "_value")) {

                                var entityname = DisplayedEntity.strID;
                                var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
                                var strCodeGroup = GetCodeID(entityname, selectedEntityFields[index].ID, entityIntellisenseList);
                                if (!strCodeGroup) {
                                    strCodeGroup = "0";
                                }
                                newValueField.dictAttributes.sfwCodeID = strCodeGroup;
                            }

                            if (!$scope.selectedCurrentRecord.Elements.some(function (itm) { return itm.dictAttributes && itm.dictAttributes.sfwEntityField == EntityField; })) {
                                $rootScope.PushItem(newValueField, $scope.selectedCurrentRecord.Elements, "selectedRecordRowClick");
                                $scope.selectedRecordRowClick(newValueField);
                            }
                        }
                        else {
                            alert(String.format("'{0}' field cannot be added as it is object/collection.", selectedEntityFields[index].ID));
                        }
                        selectedEntityFields[index].IsSelected = "False";
                        selectedEntityFields[index].IsRecordSelected = false;
                    }
                } else {
                    alert("Please select atleast one field from entity tree.");
                }
            }
        }
        else {
            if ($scope.selectedCurrentRecord) {
                if ($scope.objFile.SelecetdField) {
                    $scope.AddSingleOrMultipleSelectedFields($scope.objFile.SelecetdField);
                    $scope.objFile.SelecetdField = undefined;
                }
                else if ($scope.objBusinessObjectTree.lstmultipleselectedfields.length > 0) {
                    for (var i = 0; i < $scope.objBusinessObjectTree.lstmultipleselectedfields.length; i++) {
                        $scope.AddSingleOrMultipleSelectedFields($scope.objBusinessObjectTree.lstmultipleselectedfields[i]);
                    }
                    $scope.objBusinessObjectTree.lstmultipleselectedfields = [];
                }
                else if ($scope.selectedCurrentRecord.dictAttributes && $scope.selectedCurrentRecord.dictAttributes.sfwQueryID) {
                    var newValueField = {
                        Name: 'sfwField',
                        dictAttributes: { sfwDescription: '', sfwLength: '0', sfwObjectField: '', sfwDataFormat: '', sfwRemoveDecimal: '', sfwConstant: '', sfwPaddingCharacter: '', sfwEndNode: '', sfwStartNode: '' },
                        Elements: []
                    };
                    $rootScope.PushItem(newValueField, $scope.selectedCurrentRecord.Elements, "selectedRecordRowClick");
                    $scope.selectedRecordRowClick(newValueField);
                }
                else {
                    alert("Please select a field from object tree.");
                }
            }
            $scope.ClearSelectedFieldList($scope.objBusinessObjectTree.lstCurrentBusinessObjectProperties);
        }
    };

    $scope.AddSingleOrMultipleSelectedFields = function (SelecetdField) {
        if (SelecetdField.DataType && SelecetdField.DataType == "ValueType") {
            var newValueField = {
            };
            newValueField.Value = '';
            newValueField.Name = 'sfwField';
            newValueField.Elements = [];
            var strFullPath = "";
            var strDisplayObject = getDisplayedEntity($scope.objBusinessObjectTree.lstdisplaybusinessobject);
            var strDisplayName = strDisplayObject.strDisplayName;
            if (strDisplayName) {
                strFullPath = strDisplayName + "." + SelecetdField.ShortName;
            } else {
                strFullPath = SelecetdField.ShortName;
            }
            newValueField.dictAttributes = { sfwDescription: SelecetdField.ShortName, sfwLength: '0', sfwObjectField: strFullPath, sfwDataFormat: '', sfwRemoveDecimal: '', sfwConstant: '', sfwPaddingCharacter: '', sfwEndNode: '', sfwStartNode: '' };
            if (!$scope.selectedCurrentRecord.Elements.some(function (itm) { return itm.dictAttributes && itm.dictAttributes.sfwObjectField && itm.dictAttributes.sfwObjectField == strFullPath; })) {
                $rootScope.PushItem(newValueField, $scope.selectedCurrentRecord.Elements, "selectedRecordRowClick");
                $scope.selectedRecordRowClick(newValueField);
            }
        }
        else {
            alert("Object/Collection Cannot be added");
        }
    };

    $scope.ClearSelectedFieldList = function (Properties) {
        angular.forEach(Properties, function (obj) {
            if (obj.IsSelected && obj.IsSelected.toLowerCase() == "true") {
                obj.IsSelected = "false";
                obj.IsRecordSelected = false;
            }
            //if (obj.ChildProperties && obj.ChildProperties.length > 0) {
            //    $scope.ClearSelectedFieldList(obj);
            //}
        });
    };

    $scope.CapitalizeFirstLetterInWord = function (string) {
        var strArray = string.split('_');
        var description = "";
        for (var i = 0; i < strArray.length; i++) {
            if (i != 0) {
                description += " ";
            }
            description += strArray[i].charAt(0).toUpperCase() + strArray[i].slice(1);
        }
        return description;
    };


    $scope.openCodeIDDialog = function () {
        if ($scope.selectedCurrentRecordRow) {
            var newScope = $scope.$new();
            newScope.selectedCurrentRecordRow = $scope.selectedCurrentRecordRow;
            newScope.selectedCurrentRecordRow.selectedCheckListIDValue = $scope.selectedCurrentRecordRow.selectedCheckListIDValue;
            newScope.SetCheckListValue = function () {

                if (newScope.selectedCurrentRecordRow.selectedCheckListIDValue != undefined) {
                    $scope.selectedCurrentRecordRow.dictAttributes.sfwCodeID = newScope.selectedCurrentRecordRow.selectedCheckListIDValue.CodeID;
                    ngDialog.close(newScope.openDialog.id);

                }
            };
            newScope.CloseCheckList = function () {
                ngDialog.close(newScope.openDialog.id);
            };

            newScope.openDialog = ngDialog.open({
                template: 'SearchCheckListValue',
                scope: newScope,
                closeByDocument: false,
                className: 'ngdialog-theme-default ngdialog-theme-custom',
            });
        }
    };

    // open Required Message Dialog
    $scope.openRequireMessageDialog = function (obj) {
        var newScope = $scope.$new();
        newScope.selectedCurrentRecordRow = JSON.parse(JSON.stringify(obj));
        newScope.displayMessage = { message: "m", severity: "s" };
        newScope.dialogOptions = { height: 500, width: 900 };
        newScope.templateName = "File/views/InboundFileAttributes.html";
        newScope.selectedFieldValueMapper = null;
        if (newScope.templateName && newScope.templateName.trim().length > 0) {
            newScope.dialog = $rootScope.showDialog(newScope, 'Inbound Attributes', newScope.templateName, newScope.dialogOptions);
        }

        newScope.addFieldValueMapper = function () {
            var fieldValueMapper = {
                Name: "sfwFieldValueMapper", dictAttributes: {}, Elements: []
            };
            newScope.selectedCurrentRecordRow.Elements.push(fieldValueMapper);
        }
        newScope.deleteFieldValueMapper = function () {
            if (newScope.selectedFieldValueMapper) {
                var index = newScope.selectedCurrentRecordRow.Elements.indexOf(newScope.selectedFieldValueMapper);
                if (index > -1) {
                    newScope.selectedCurrentRecordRow.Elements.splice(index, 1);
                    newScope.selectedFieldValueMapper = null;
                    if (index === newScope.selectedCurrentRecordRow.Elements.length) {
                        index--;
                    }
                    if (index > -1) {
                        newScope.selectedFieldValueMapper = newScope.selectedCurrentRecordRow.Elements[index];
                    }
                }
            }
        }
        newScope.canDeleteFieldValueMapper = function () {
            return newScope.selectedFieldValueMapper;
        }
        newScope.selectFieldValueMapper = function (fieldValueMapper) {
            newScope.selectedFieldValueMapper = fieldValueMapper;
        }
        newScope.validate = function () {
        }

        newScope.okClick = function () {
            $rootScope.EditPropertyValue(obj.dictAttributes, obj, "dictAttributes", newScope.selectedCurrentRecordRow.dictAttributes);
            $rootScope.EditPropertyValue(obj.Elements, obj, "Elements", newScope.selectedCurrentRecordRow.Elements);
            newScope.closeDialog();
        };
        newScope.closeDialog = function () {
            if (newScope.dialog) {
                newScope.dialog.close();
            }
        }
    };

    $scope.onChangeEntityTree = function (entName) {
        $scope.objFileSelecetdField = undefined;
        if (entName == null) {
            $scope.entityNameForEntityTree = "...";
        } else {
            $scope.entityNameForEntityTree = entName;
        }
    };

    // object tree
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
                $scope.lstobjecttree = [];
                $scope.lstobjecttree.push($scope.objBusinessObject.ObjTree);
                $scope.objBusinessObject.ObjTree.IsMainBusObject = true;
                $scope.objBusinessObject.ObjTree.IsVisible = true;
                SetParentForObjTreeChild($scope.objBusinessObject.ObjTree);
            }
        });
    };


    $scope.onRecordTypeChanged = function () {
        if ($scope.selectedCurrentRecord) {
            if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "CommaDelimited") {
                angular.forEach($scope.selectedCurrentRecord.Elements, function (obj) {
                    if (obj.Name == "sfwField") {
                        obj.PrevLength = obj.dictAttributes.sfwLength;
                        obj.dictAttributes.sfwLength = "";
                    }
                });
            }
            else {
                angular.forEach($scope.selectedCurrentRecord.Elements, function (obj) {
                    if (obj.PrevLength != undefined && obj.PrevLength != "") {
                        {
                            obj.dictAttributes.sfwLength = obj.PrevLength;
                        }
                    }
                });

                if ($scope.objFile.dictAttributes.sfwType == "Inbound") {
                    if ($scope.objFile.IsChildOfInboundFileBase) {
                        if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "Header") {

                            if ($scope.objFile.dictAttributes.sfwProcessDetail != undefined && $scope.objFile.dictAttributes.sfwProcessDetail != "" && $scope.objFile.dictAttributes.sfwProcessDetail.toLowerCase() == "true") {
                                var result = confirm("Do you want to set record type as Header then Process Detail should be false");
                                if (result) {
                                    $scope.objFile.dictAttributes.sfwProcessDetail = "False";
                                }
                                else {
                                    if ($scope.selectedCurrentRecord.DummyRecordType) {
                                        $scope.selectedCurrentRecord.dictAttributes.sfwRecordType = $scope.selectedCurrentRecord.DummyRecordType;
                                    }
                                    else {
                                        $scope.selectedCurrentRecord.dictAttributes.sfwRecordType = "";
                                    }
                                }
                            }

                            if ($scope.objFile.dictAttributes.sfwUploadToFileDetail == undefined || $scope.objFile.dictAttributes.sfwUploadToFileDetail == "" || ($scope.objFile.dictAttributes.sfwUploadToFileDetail != undefined && $scope.objFile.dictAttributes.sfwUploadToFileDetail != "" && $scope.objFile.dictAttributes.sfwUploadToFileDetail.toLowerCase() == "false")) {
                                if ($scope.objFile.dictAttributes.sfwProcessInParallel != undefined && $scope.objFile.dictAttributes.sfwProcessInParallel != "" && $scope.objFile.dictAttributes.sfwProcessInParallel.toLowerCase() == "true") {
                                    var result = confirm("Do you want to set record type as Header then Process in Parallel should be false", "Confirmation");
                                    if (result) {
                                        $scope.objFile.dictAttributes.sfwProcessInParallel = "False";
                                    }
                                    else {
                                        if ($scope.selectedCurrentRecord.DummyRecordType) {
                                            $scope.selectedCurrentRecord.dictAttributes.sfwRecordType = $scope.selectedCurrentRecord.DummyRecordType;
                                        }
                                        else {
                                            $scope.selectedCurrentRecord.dictAttributes.sfwRecordType = "";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // $scope.selectedCurrentRecord.DummyRecordType = $scope.selectedCurrentRecord.dictAttributes.sfwRecordType;
                }

                if ($scope.selectedCurrentRecord.dictAttributes.sfwRecordType == "AutoSkip") {
                    $rootScope.UndRedoBulkOp("Start");
                    $rootScope.EditPropertyValue($scope.selectedCurrentRecord.sfwRecordObject, $scope.selectedCurrentRecord, "sfwRecordObject", "");
                    $scope.lstobjecttree = [];
                    $scope.objBusinessObject.ObjTree = undefined;
                    $rootScope.EditPropertyValue($scope.selectedCurrentRecord.dictAttributes.sfwRecordObject, $scope.selectedCurrentRecord.dictAttributes, "sfwRecordObject", "");
                    if ($scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwOutboundObject != "") {
                        var tempArra = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                        $scope.BusObjectName = tempArra[tempArra.length - 1];
                    }
                    $rootScope.UndRedoBulkOp("End");
                }
                else {
                    if (($scope.selectedCurrentRecord.DummyRecordType == "AutoSkip" || $scope.selectedCurrentRecord.DummyRecordType == undefined) && $scope.selectedCurrentRecord.dictAttributes.sfwRecordType != "AutoSkip") {
                        $rootScope.UndRedoBulkOp("Start");
                        $rootScope.EditPropertyValue($scope.selectedCurrentRecord.dictAttributes.sfwObjectID, $scope.selectedCurrentRecord.dictAttributes, "sfwObjectID", "");
                        $scope.lstobjecttree = [];
                        $scope.objBusinessObject.ObjTree = undefined;
                        if ($scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwOutboundObject != "") {
                            var tempArra = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
                            $scope.BusObjectName = tempArra[tempArra.length - 1];
                        }
                        $rootScope.UndRedoBulkOp("End");
                    }
                }

                $scope.selectedCurrentRecord.DummyRecordType = $scope.selectedCurrentRecord.dictAttributes.sfwRecordType;
            }

        }
    };

    $scope.OnInboundHeaderIdChanged = function (event) {
        if ($scope.selectedCurrentRecord) {
            $rootScope.UndRedoBulkOp("Start");
            if ($scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity != undefined && $scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity != "") {
                if ($scope.objFile.dictAttributes.sfwUploadToFileDetail == undefined || $scope.objFile.dictAttributes.sfwUploadToFileDetail == "" || ($scope.objFile.dictAttributes.sfwUploadToFileDetail != undefined && $scope.objFile.dictAttributes.sfwUploadToFileDetail != "" && $scope.objFile.dictAttributes.sfwUploadToFileDetail.toLowerCase() == "false")) {

                    if ($scope.objFile.dictAttributes.sfwProcessInParallel != undefined && $scope.objFile.dictAttributes.sfwProcessInParallel != "" && $scope.objFile.dictAttributes.sfwProcessInParallel.toLowerCase() == "true") {
                        var result = confirm("Do you want to set Header Object ID then Process in Parallel should be false");
                        if (!result) {
                            $rootScope.EditPropertyValue($scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity, $scope.selectedCurrentRecord.dictAttributes, "sfwHeaderEntity", "");
                            $scope.selectedCurrentRecord.dictAttributes.sfwHeaderEntity = "";
                            //this[ApplicationConstants.XMLFacade.SFWHEADEROBJECTID] = string.Empty;
                            if (event) {
                                event.preventDefault();
                            }
                        }
                        else {
                            $rootScope.EditPropertyValue($scope.objFile.dictAttributes.sfwProcessInParallel, $scope.objFile.dictAttributes, "sfwProcessInParallel", "False");
                            //this.ObjVM.Model["sfwProcessInParallel"] = "False";
                        }
                    }
                }
            }
            $rootScope.UndRedoBulkOp("End");
        }
    };

    $scope.NavigateToEntity = function (aEntityID) {
        if (aEntityID && aEntityID != "") {
            hubMain.server.navigateToFile(aEntityID, "").done(function (objfile) {
                $rootScope.openFile(objfile, undefined);
            });
        }
    };

    //$scope.removeExtraFieldsDataInToMainModel = function () {
    //    if ($scope.objFileExtraFields) {
    //        var index = $scope.objFile.Elements.indexOf($scope.objFileExtraFields);
    //        if (index > -1) {
    //            $scope.objFile.Elements.splice(index, 1);
    //        }
    //    }
    //}

    $scope.addExtraFieldsDataInToMainModel = function () {
        if ($scope.objFileExtraFields) {
            var index = $scope.objFile.Elements.indexOf($scope.objFileExtraFields);
            if (index == -1) {
                $scope.objFile.Elements.push($scope.objFileExtraFields);
            }
        }
    };

    $scope.BeforeSaveToFile = function () {
        $scope.addExtraFieldsDataInToMainModel();
    };

    ////#region key Up & Down


    //$scope.KeyUp = function (ruleType) {
    //    if (ruleType && ruleType == 'File' && $scope.selectedCurrentRecord) {
    //        var tempObj = keyUpAction($scope.selectedCurrentRecord, $scope.objFile);
    //        if (tempObj) {
    //            $scope.SelectedRecordClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }

    //    }


    //}
    //$scope.KeyDown = function (ruleType) {
    //    if (ruleType && ruleType == 'File' && $scope.selectedCurrentRecord) {

    //        var tempObj = keyDownAction($scope.selectedCurrentRecord, $scope.objFile);
    //        if (tempObj) {
    //            $scope.SelectedRecordClick(tempObj);
    //        }
    //        else {

    //            return true;
    //        }
    //    }



    //}
    ////#endregion

    $scope.recordQueryChange = function () {
        $scope.ActiveTabForFile = 'Items';
        $scope.iblnShowQueryIntellisense = false;
        $scope.strQueryID = "";
        if ($scope.selectedCurrentRecord) {
            if ($scope.selectedCurrentRecord.dictAttributes.sfwQueryID || (!$scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwQueryID)) {
                $scope.iblnShowQueryIntellisense = true;
            }
            var lstrQueryId = $scope.selectedCurrentRecord.dictAttributes.sfwQueryID;
            if (!lstrQueryId && !$scope.objFile.dictAttributes.sfwOutboundObject && $scope.objFile.dictAttributes.sfwQueryID) {
                lstrQueryId = $scope.objFile.dictAttributes.sfwQueryID;
            }
            if ($scope.strQueryID != lstrQueryId) {
                $scope.strQueryID = lstrQueryId;
            }
        }
    };
    function watchOnProperty(newValue, OldValue) {
        $scope.recordQueryChange();
    };
    $scope.$watch("objFile.dictAttributes.sfwOutboundObject", watchOnProperty);
    $scope.$watch("objFile.dictAttributes.sfwQueryID", watchOnProperty);
    $scope.$watch("selectedCurrentRecord.dictAttributes.sfwQueryID", watchOnProperty);
    //#region Record level Query parameter

    $scope.setQueryParamter = function (astrQueryId, aobjCurrentRecord) {
        var newQueryparameterScope = $scope.$new(true);
        newQueryparameterScope.lstQuerySchema = [];
        newQueryparameterScope.setBusObjectName = function (astrBusName) {
            newQueryparameterScope.BusObjectName = astrBusName;
            newQueryparameterScope.objBusinessObjectTree = { lstdisplaybusinessobject: [], lstmultipleselectedfields: [], lstCurrentBusinessObjectProperties: [] };
            newQueryparameterScope.SelecetdField = undefined;
            newQueryparameterScope.ObjTree = undefined;
        };
        if ($scope.objFile.dictAttributes && $scope.objFile.dictAttributes.sfwOutboundObject) {
            var lst = $scope.objFile.dictAttributes.sfwOutboundObject.split(".");
            if (lst.length > 0) {
                newQueryparameterScope.setBusObjectName(lst[lst.length - 1]);
            }
        }
        else {
            var lstrQueryId = $scope.objFile.dictAttributes.sfwQueryID;
            if (lstrQueryId && lstrQueryId.contains(".")) {
                $.connection.hubForm.server.getEntityQueryColumns(lstrQueryId, "").done(function (data) {
                    if (data) {
                        for (var i = 0; i < data.length; i++) {
                            newQueryparameterScope.lstQuerySchema.push(data[i].CodeID);
                        }
                    }
                });
            }
        }
        newQueryparameterScope.lstParameter = [];
        newQueryparameterScope.lobjQuery = getQueryFromEntityIntellisense(astrQueryId, $EntityIntellisenseFactory.getEntityIntellisense());
        if (newQueryparameterScope.lobjQuery !== null) {
            newQueryparameterScope.lstParameter = [];
            angular.forEach(newQueryparameterScope.lobjQuery.Parameters, function (astrParam) {
                var lstrParameter = "";
                var lstrField = astrParam.ID;
                if (lstrField.contains("@")) {
                    lstrParameter = lstrField.substring(lstrField.indexOf('@') + 1, lstrField.length);
                }
                else {
                    lstrParameter = lstrField;
                }

                var lobjParameter = { DisplayField: lstrParameter, ParameterField: lstrField, ParameterValue: "" };
                newQueryparameterScope.lstParameter.push(lobjParameter);
            });
        }

        var lstExistingparameter = aobjCurrentRecord.Elements.filter(function (aobjElement) {
            return aobjElement.Name === "parameters";
        })
        if (lstExistingparameter.length > 0) {
            for (var i = 0; i < lstExistingparameter[0].Elements.length; i++) {
                var lobjParameter = lstExistingparameter[0].Elements[i];
                for (var j = 0; j < newQueryparameterScope.lstParameter.length; j++) {
                    if (newQueryparameterScope.lstParameter[j].ParameterField == lobjParameter.dictAttributes.ID) {
                        newQueryparameterScope.lstParameter[j].ParameterValue = lobjParameter.dictAttributes.sfwQueryBindField;
                    }
                }
            }
        }

        newQueryparameterScope.objNewDialog = $rootScope.showDialog(newQueryparameterScope, "Set Query Parameters", "File/views/SetRecordLevelQueryParameter.html", {
            width: 700, height: 500
        });

        newQueryparameterScope.setParameter = function () {
            var lobjparameters = { Name: "parameters", Elements: [] };

            for (var i = 0, len = newQueryparameterScope.lstParameter.length; i < len; i++) {
                var lobjParameter = { Name: "parameter", dictAttributes: { ID: newQueryparameterScope.lstParameter[i].ParameterField, sfwQueryBindField: newQueryparameterScope.lstParameter[i].ParameterValue } };
                lobjparameters.Elements.push(lobjParameter);
            }
            $rootScope.UndRedoBulkOp("Start");
            if (lstExistingparameter.length > 0) {
                var lintIndex = -1;
                for (var i = 0, len = aobjCurrentRecord.Elements.length; i < len; i++) {
                    var lobj = aobjCurrentRecord.Elements[i];
                    if (lobj.Name === "parameters") {
                        lintIndex = i;
                        break;
                    }
                }
                if (lintIndex > -1) {
                    $rootScope.DeleteItem(aobjCurrentRecord.Elements[lintIndex], aobjCurrentRecord.Elements);
                }
            }
            $rootScope.InsertItem(lobjparameters, aobjCurrentRecord.Elements, 0);
            $rootScope.UndRedoBulkOp("End");
            newQueryparameterScope.closeSetParameterDialog();
        };
        newQueryparameterScope.closeSetParameterDialog = function () {
            newQueryparameterScope.objNewDialog.close();
        };


    };

    //#endregion

}]);

