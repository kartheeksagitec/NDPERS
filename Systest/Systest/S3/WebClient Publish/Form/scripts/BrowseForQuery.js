app.controller("BrowseForQueryController", ["$scope", "$rootScope", "ngDialog", "share", "ParameterFactory", "$EntityIntellisenseFactory", "$filter", "$timeout", function ($scope, $rootScope, ngDialog, share, ParameterFactory, $EntityIntellisenseFactory, $filter, $timeout) {
    $scope.sharedata = share;
    $scope.blnShowAllFilesForBrowse = false;

    $scope.Init = function () {
        
        $scope.lstEntity = [];
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        var scopecreatenewobj = getScopeByFileName('createNewObject');
        if (scopecreatenewobj && scopecreatenewobj.objNewItems && scopecreatenewobj.objNewItems.SelectedOption === "File") {
            $scope.strPath = scopecreatenewobj.objFileDetails.Path;
        }
        else if (scopecreatenewobj && scopecreatenewobj.objFormsDetails && scopecreatenewobj.objFormsDetails.Path) {
            $scope.strPath = scopecreatenewobj.objFormsDetails.Path;
        }
        else if (scopecreatenewobj && scopecreatenewobj.objFormLinkDetails && scopecreatenewobj.objFormLinkDetails.Path) {
            $scope.strPath = scopecreatenewobj.objFormLinkDetails.Path;
        }
        
        else if (scopecreatenewobj && scopecreatenewobj.objReportDetails && scopecreatenewobj.objReportDetails.Path) {
            $scope.strPath = scopecreatenewobj.objReportDetails.Path;
        }
        else if (!scopecreatenewobj && $rootScope.currentopenfile) {
            var currentfile = $rootScope.currentopenfile.file.FilePath.split('\\');
            $scope.strPath = currentfile[currentfile.length - 2];
        }
        function AddInlstEntity(item) {
            $scope.lstEntity.push(item);
        }
        if ($scope.strPath) {
            var lstObj = entityIntellisenseList.filter(function (x) {
                var lstsplit = [];
                if (x.FilePath) {
                    lstsplit = x.FilePath.split('\\');
                    var entityfolder = lstsplit[lstsplit.length - 2];
                    return $scope.strPath == entityfolder;
                }
            });
            angular.forEach(lstObj, AddInlstEntity);

        }
        else {
            $scope.lstEntity = entityIntellisenseList;
        }

        if ($scope.subQueryType) {
            angular.forEach(entityIntellisenseList, function (item) {
                if (item.Queries.length > 0) {
                    angular.forEach(item.Queries, function (itm) {
                        itm.IsVisible = false;
                        if (itm.QueryType == $scope.subQueryType) {
                            itm.IsVisible = true;
                        }

                    });
                }

            });
        }
        else {
            angular.forEach(entityIntellisenseList, function (item) {
                if (item.Queries.length > 0) {
                    angular.forEach(item.Queries, function (itm) {
                        if (itm.QueryType != "SubSelectQuery") {
                            itm.IsVisible = true;
                        }
                    });
                }

            });
        }

        function UpdateParameterValue(value, key) {
            var parameter = $scope.selectedCurrentQuery.Parameters.filter(function (item) {
                return item.ID == value.ID;
            });
            if (parameter.length > 0) {
                parameter[0].Value = value.Value ? value.Value : "";
            }
        }

        if ($scope.strSelectedQuery != undefined && $scope.strSelectedQuery != "") {
            var lst = $scope.strSelectedQuery.split('.');
            if (lst && lst.length == 2) {
                var entityName = lst[0];
                var queryID = lst[1];
                var lst = $scope.lstEntity.filter(function (x) { return x.ID == entityName; });
                if (lst && lst.length > 0) {
                    $scope.selectedCurrentEntity = lst[0];
                    $scope.ScrollToCurrentEntity();
                    $scope.queryCount = $scope.selectedCurrentEntity.Queries.length;
                    var lstQuery = $scope.selectedCurrentEntity.Queries.filter(function (x) { return x.ID == queryID; });
                    if (lstQuery && lstQuery.length > 0) {
                        $scope.selectedCurrentQuery = lstQuery[0];
                        // for setting paramter and values of the already selected query - applicable only in BPM
                        if ($scope.queryParameters && $scope.queryParameters.length > 0) {

                            angular.forEach($scope.queryParameters, UpdateParameterValue);
                                }
                        $scope.ScrollToCurrentQuery();
                        //$scope.sharedata.selectedCurrentQuery = $scope.selectedCurrentQuery;
                    }
                }
            }
        }
    };

    $scope.ScrollToCurrentEntity = function () {
        if ($scope.selectedCurrentEntity) {
            var item = document.getElementById($scope.selectedCurrentEntity.ID);
            if (item) {
                item.scrollIntoView();
            }
            else {
                var selectionInterval = setInterval(function () {
                    var item = document.getElementById($scope.selectedCurrentEntity.ID);
                    if (item) {
                        item.scrollIntoView();
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }
                    setTimeout(function () {
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }, 5000);
                }, 100);
            }
        }
    };

    $scope.ScrollToCurrentQuery = function () {
        if ($scope.selectedCurrentQuery) {
            var item = document.getElementById($scope.selectedCurrentEntity.ID + '_' + $scope.selectedCurrentQuery.ID);
            if (item) {
                item.scrollIntoView();
            }
            else {
                var selectionInterval = setInterval(function () {
                    var item = document.getElementById($scope.selectedCurrentEntity.ID + '_' + $scope.selectedCurrentQuery.ID);
                    if (item) {
                        item.scrollIntoView();
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }
                    setTimeout(function () {
                        if (selectionInterval) {
                            clearInterval(selectionInterval);
                        }
                    }, 5000);
                }, 100);
            }
        }
    };


    $scope.ShowAllFilesForBrowseClick = function (blnShowAllFilesForBrowse) {
        var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
        setTimeout(function () {
            $scope.$evalAsync(function () {
                if (blnShowAllFilesForBrowse) {
                    $scope.lstEntity = entityIntellisenseList;
                }
                else {
                    var scopecreatenewobj = getScopeByFileName('createNewObject');
                    if (scopecreatenewobj && scopecreatenewobj.objNewItems && scopecreatenewobj.objNewItems.SelectedOption === "File") {
                        $scope.strPath = scopecreatenewobj.objFileDetails.Path;
                    }
                    else if (scopecreatenewobj && scopecreatenewobj.objFormsDetails && scopecreatenewobj.objFormsDetails.Path) {
                        $scope.strPath = scopecreatenewobj.objFormsDetails.Path;
                    }
                    else if (scopecreatenewobj && scopecreatenewobj.objReportDetails && scopecreatenewobj.objReportDetails.Path) {
                        $scope.strPath = scopecreatenewobj.objReportDetails.Path;
                    }
                    else if (!scopecreatenewobj && $rootScope.currentopenfile) {
                        var currentfile = $rootScope.currentopenfile.file.FilePath.split('\\');
                        $scope.strPath = currentfile[currentfile.length - 2];
                    }
                    if ($scope.strPath) {
                        $scope.lstEntity = [];
                        $scope.selectedCurrentEntity = {};
                        $scope.selectedCurrentQuery = {
                        };
                        var lstObj = entityIntellisenseList.filter(function (x) {
                            var lstsplit = [];
                            if (x.FilePath) {
                                lstsplit = x.FilePath.split('\\');
                                var entityfolder = lstsplit[lstsplit.length - 2];
                                return $scope.strPath == entityfolder;
                            }
                        });
                        angular.forEach(lstObj, function (item) {
                            $scope.lstEntity.push(item);
                        });
                    }
                }
            });
        }, 5);
    };

    //#region Browse
    $scope.selectedEntityClick = function (obj, isScroll) {
        $scope.queryCount = 0;
        $scope.selectedCurrentEntity = obj;
        if ($scope.subQueryType) {
            var lst = $scope.selectedCurrentEntity.Queries.filter(function (x) { return x.QueryType == $scope.subQueryType; });
            if (lst && lst.length > 0) {
                $scope.queryCount = lst.length;
            }

        }
        else {
            var lst = $scope.selectedCurrentEntity.Queries.filter(function (x) { return x.QueryType != "SubSelectQuery"; });
            if (lst && lst.length > 0) {
                $scope.queryCount = lst.length;
            }
        }
        if (isScroll) {
            $scope.ScrollToCurrentEntity();
        }
    };
    $scope.selectedQueryClick = function (obj, isScroll) {
        $scope.selectedCurrentQuery = obj;
        if (isScroll) {
            $scope.ScrollToCurrentQuery();
        }
    };

    $scope.selectedParameterClick = function (obj) {
        $scope.selectedCurrentParameter = obj;
    };

    var KeyCodes = {
        UPARROW: 38,
        DOWNARROW: 40,
    };

    $scope.$on('arrowpress', function (msg, e) {
        if (e.code == KeyCodes.UPARROW) {
            var index = $scope.lstEntity.indexOf($scope.selectedCurrentEntity);

            if (index > 0) {
                $scope.selectedEntityClick($scope.lstEntity[index - 1]);
            }
        }
        else if (e.code == KeyCodes.DOWNARROW) {
            var index = $scope.lstEntity.indexOf($scope.selectedCurrentEntity);

            if (index < $scope.lstEntity.length - 1) {
                $scope.selectedEntityClick($scope.lstEntity[index + 1]);
            }
        }

        $scope.$apply();

        $timeout(function () {
            var elem;
            if ($scope.isSet(1)) {
                elem = $("#divBrowse").find(".selected");
            }
            else if ($scope.isSet(2)) {
                elem = $("#divTable").find(".selected");
            }

            if ($scope.isSet(1) && elem.length > 0) {
                $('#divBrowse > .col-xs-12 > .browse-for-query-form > .col-xs-6 > .browse-query-panel > .form-group > #divBrowseEntity').scrollTop($(elem[0]).offset().top - $('#divBrowse > .col-xs-12 > .browse-for-query-form > .col-xs-6 > .browse-query-panel > .form-group > #divBrowseEntity').offset().top + $('#divBrowse > .col-xs-12 > .browse-for-query-form > .col-xs-6 > .browse-query-panel > .form-group > #divBrowseEntity').scrollTop() - 10);
            }
        }, 300);
    });

    //#endregion


    //#region Table

    $scope.getTableList = function (obj) {
        $scope.lstEntityQuery = [];
        $scope.selectedTableRow = undefined;
        $scope.selectedCurrentQuery = undefined;
        if (obj) {
            var entityIntellisenseList = $EntityIntellisenseFactory.getEntityIntellisense();
            $scope.lstEntityQuery = $filter('entitybyTable')(entityIntellisenseList, obj, $scope.subQueryType);
        }
    };

    $scope.selectTableClick = function (obj) {
        if (obj) {
            $scope.selectedCurrentQuery = obj;
        }
    };

    $scope.selectedTableParametersClick = function (index, obj) {
        if (obj) {
            $scope.selectedTableParametersRow = index;
            $scope.selectedCurrentTableParametersRow = obj;
        }
    };

    $scope.closeClick = function () {
        //if ($scope.QueryDialog && $scope.QueryDialog.id) {
        //    ngDialog.close($scope.QueryDialog.id);
        //}
        //else if ($scope.queryDialogID && $scope.queryDialogID.id) {
        //    ngDialog.close($scope.queryDialogID.id);
        //}
        $scope.QueryDialog.close();
    };
    $scope.okClick = function () {
        var QueryID;
        if ($scope.tab == 1 && $scope.selectedCurrentEntity && $scope.selectedCurrentEntity.ID) {
            QueryID = $scope.selectedCurrentEntity.ID + "." + $scope.selectedCurrentQuery.ID;
        }
        else if ($scope.selectedCurrentQuery && $scope.selectedCurrentQuery.ID) {
            QueryID = $scope.selectedCurrentQuery.ID;
        }
        if ($scope.IsBPM) {
            var selectedQueryObj = { Id: QueryID, Parameters: $scope.selectedCurrentQuery.Parameters };
            $scope.$emit("onQueryClickBPM", selectedQueryObj);
        }
        else if ($scope.IsRetrieval) {
            $scope.$emit("onRetrievalClick", QueryID);
        }
        else if ($scope.IsAutoComplete) {
            $scope.$emit("onAutoCompleteClick", QueryID);
        }
        else if ($scope.IsForm) {
            $scope.$emit("onFormQueryClick", QueryID);
        }
        else if ($scope.IsFilterGridSearch) {
            $scope.$emit("onFilterGridSerchClick", $scope.selectedCurrentQuery, QueryID);
        }
        else if ($scope.IsBaseQuery) {
            $scope.$emit("onBaseQueryClick", QueryID);
        }
        else {
            $scope.$emit("onQueryClick", QueryID);
            $scope.sharedata.selectedCurrentQuery = $scope.selectedCurrentQuery;
            $scope.sharedata.isNewQuerySelected = true;

            //ParameterFactory.setParameterObject($scope.selectedCurrentQuery, true);

        }
        //$scope.$emit("onQueryClick", QueryID);
        //ngDialog.close($scope.QueryDialog.id);
        $timeout(function () {
            if ($scope.validateQuery) $scope.validateQuery();
        });
        
        $scope.QueryDialog.close();
    };

    $scope.ValidateDataAll = function () {

        $scope.ErrorMessageForDisplay = "";
        if ($scope.tab == 1) //for browse tab
        {
            if ($scope.selectedCurrentQuery == undefined) {
                $scope.ErrorMessageForDisplay = "Error: A appropriate Query needs to be selected.";
                return true;
            }

            else if ($scope.selectedCurrentQuery.SqlQuery == undefined || $scope.selectedCurrentQuery.SqlQuery == "") {
                $scope.ErrorMessageForDisplay = "Error: Selected Query Should Not be blank. ";

                return true;
            }
            return false;
        }
        else {
            if ($scope.selectedCurrentQuery == undefined) {
                $scope.ErrorMessageForDisplay = "Error: A appropriate Query needs to be selected.";

                return true;
            }
            return false;
        }
    };

    //#endregion

    $scope.clearTab = function () {
        $scope.selectedCurrentQuery = undefined;
        $scope.selectedCurrentEntity = undefined;
        $scope.EntityName = "";
        $scope.QueryID = "";
        $scope.TableName = undefined;
        $scope.lstEntityQuery = [];
        $scope.selectedTableRow = undefined;
    };

    //Functionality to implement Tab On the Page
    $scope.tab = 1;
    $scope.setTab = function (newTab) {
        $scope.tab = newTab;
        $scope.clearTab();
    };
    $scope.isSet = function (tabNum) {
        return $scope.tab === tabNum;
    };

    $scope.Init();
}]);

app.factory('ParameterFactory', [function () {
    var sharedata = { selectedCurrentQuery: {}, isNewQuerySelected: false };
    return {

        getParameterObject: function () {
            return sharedata;
        },
        setParameterObject: function (selectedCurrentQuery, isNewQuerySelected) {
            sharedata.selectedCurrentQuery = selectedCurrentQuery;
            sharedata.isNewQuerySelected = isNewQuerySelected;
        },
        clearParameterFactory: function(){
            sharedata.selectedCurrentQuery = {};
            sharedata.isNewQuerySelected = {};
        }
    };
}]);

app.directive('keyScroll', [function () {
    return function (scope, elem) {
        elem.bind('keydown', function (event) {
            if (event.keyCode == 38 || event.keyCode == 40) {
                scope.$emit('arrowpress', { code: event.keyCode });
            }
        });
    };
}]);