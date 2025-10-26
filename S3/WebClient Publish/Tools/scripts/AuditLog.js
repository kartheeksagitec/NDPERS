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
