app.controller("SearchFilesController", ["$scope", "hubcontext", "$rootScope", "$filter", "$searchQuerybuilder", "$ObjectsSearch", "$resourceFactory", function ($scope, hubcontext, $rootScope, $filter, $searchQuerybuilder, $ObjectsSearch, $resourceFactory) {

    function SearchFilesController() {
        $scope.lstactiveSearchPanelIDs = ['mainSearchCommonCriteriaPanel'];
        $scope.objDirFunctions = {};
        $scope.SelectedFileType = "Entity";
        $scope.InitAdvanceSearch();
        InitCustomSearch();
        hubMain.server.prepareFolderPath("");
        $('[advance-search-wrapper] input').on("keydown", function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                $scope.$evalAsync(function () {
                    $scope.GetAllFilesInfo();
                });
                return false;
            }
        });
    }

    function InitCustomSearch() {
        $scope.objEntityExtraFields = {
            Name: "ExtraFields", Value: '', dictAttributes: {}, Elements: []
        };
    }

    $scope.InitAdvanceSearch = function () {
        // All File type List       
        $scope.lstFileTypes = JSON.parse(JSON.stringify($resourceFactory.getFileTypeList()));
        $scope.lstTfsStatus = JSON.parse(JSON.stringify($resourceFactory.getTfsStatusList()));
        // Search Criteria Object
        $scope.searchCriteria = {
            fileName: "",
            containingText: "",
            location: "",
            isCreatedDate: false,
            isModifiedDate: false,
            fromDate: "",
            toDate: "",
            fileType: [],
            TfsStatus: [],
            QuerySearchList: []
        };
        $scope.selectedDate = {
            typeOfDate: ''
        };
        $scope.queryBuilder = $searchQuerybuilder;
        $scope.ResultantFiles = [];
        $scope.ResultantExtraFieldFiles = [];
        // to make autofocus work in IE - focus should be on filename search
        setTimeout(function () {
            $('[autofocus]:not(:focus)').eq(0).focus();
            $('[advance-search-wrapper] [st-search]:visible').val(null); // reset filter textbox
            var HTMLElementSelectAll = document.querySelectorAll("[advance-search-wrapper] input[select-all-toggle]");
            if (HTMLElementSelectAll.length > 0) {
                for (var i = 0; i < HTMLElementSelectAll.length; i++) {
                    HTMLElementSelectAll[i].checked = false;
                }
            }
        });
    }

    // #region advance search
    $scope.getFolderPathIntellisense = function (event) {
        var HTMLElementLocationSearch = null;
        if (event.type == "click") {
            HTMLElementLocationSearch = $(event.target).prevAll("input[type='text']");
        }
        else {
            HTMLElementLocationSearch = $(event.target);
        }
        if (HTMLElementLocationSearch) {
            $(HTMLElementLocationSearch).autocomplete({
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
                    hubMain.server.getFolderPathIntellisense(request.term.trim(), "", true).done(function (data) {
                        if (data != "") {
                            $scope.responseParsed = parsePathIntellisenseResult(data);
                            response($scope.responseParsed);
                        }
                        else {
                            $scope.responseParsed = [];
                            response($scope.responseParsed);
                        }
                    });
                },
                search: function (event, ui) {
                    $scope.searchCriteria.location = event.target.value;
                },
                select: function (event, ui) {
                    $scope.searchCriteria.location = ui.item.value;
                },
                focus: function (event, ui) {
                    $(".ui-autocomplete > li").attr("title", ui.item.label);
                }
            });
            if (event.type == "click") {
                $(HTMLElementLocationSearch).focus();
                if ($(HTMLElementLocationSearch).data('ui-autocomplete')) {
                    $(HTMLElementLocationSearch).autocomplete("search", $(HTMLElementLocationSearch).val());
                }
            }
            if (event.ctrlKey && event.keyCode == $.ui.keyCode.SPACE && $(HTMLElementLocationSearch).data('ui-autocomplete')) {
                $(HTMLElementLocationSearch).autocomplete("search", $(HTMLElementLocationSearch).val());
                event.preventDefault();
            }
        }
    };

    $scope.SelectDateType = function () {
        ComponentsPickers.init();
        if ($scope.selectedDate.typeOfDate == "create") {
            $scope.searchCriteria.isCreatedDate = true;
            $scope.searchCriteria.isModifiedDate = false;
        } else {
            $scope.searchCriteria.isModifiedDate = true;
            $scope.searchCriteria.isCreatedDate = false;
        }
    };

    function updatecheckboxFilterList(objchanged, lststrFiler) {
        if (objchanged) {
            var intCurrentIndex = lststrFiler.indexOf(objchanged.value);
            // if obj is not in the list add it
            if (objchanged.selected && (intCurrentIndex < 0)) {
                lststrFiler.push(objchanged.value);
            }
            else if (!objchanged.selected && intCurrentIndex >= 0) {
                lststrFiler.splice(intCurrentIndex, 1);
            }
        }
    }

    $scope.onchangecheckboxlst = function (lstChanged, lststrFiler, objchanged) {
        var intLenlstChanged = lstChanged.length;
        for (var i = 0; i < intLenlstChanged; i++) {
            if (objchanged.value === lstChanged[i].value) {
                updatecheckboxFilterList(objchanged, lststrFiler);
                break;
            }
        }
    };

    $scope.SelectAllFiles = function (lstchange, lststrFiler, e) {
        var intLenChangelst = lstchange.length;
        for (var i = 0; i < intLenChangelst; i++) {
            lstchange[i].selected = e.target.checked;
            updatecheckboxFilterList(lstchange[i], lststrFiler);
        }
        if (e) {
            e.stopImmediatePropagation();
        }
    };

    $scope.GetAllFilesInfo = function (exportToExcel) {
        if (!exportToExcel) {
            exportToExcel = false;
        }
        function ValidateSearchCriteria() {
            if ($scope.searchCriteria.fileType.length == 0) {
                toastr.info("Please select at least one file type");
                return false;
            }
            if ($scope.searchCriteria.isCreatedDate && !$scope.searchCriteria.fromDate) {
                toastr.info("Please select From Date");
                return false;
            }
            if ($scope.searchCriteria.isModifiedDate && !$scope.searchCriteria.fromDate) {
                toastr.info("Please select From Date");
                return false;
            }
            else if (new Date() < Date.parse($scope.searchCriteria.fromDate)) {
                toastr.info("From Date should be less than Or equal to current Date");
                return false;
            }
            else if ($scope.searchCriteria.fromDate) {
                if (!$scope.searchCriteria.toDate) {
                    $scope.searchCriteria.toDate = $filter('date')(new Date(), "MM/dd/yyyy");
                }
            }
            else if ($scope.searchCriteria.toDate) {
                if (!$scope.searchCriteria.fromDate) {
                    toastr.info("Please select From Date");
                    return false;
                }
            }
            else if ($scope.searchCriteria.toDate && Date.parse($scope.searchCriteria.fromDate) > Date.parse($scope.searchCriteria.toDate)) {
                toastr.info("From Date should be greater than To Date");
                return false;
            }
            return true;
        };
        if (ValidateSearchCriteria()) {
            $rootScope.IsLoading = true;
            $scope.searchCriteria.fileName = $scope.searchCriteria.fileName.trim();
            // containing text search - smart search (query builder logic)
            var objSearch = angular.copy($scope.searchCriteria);
            if (objSearch.containingText) {
                var ObjQuery = angular.copy($ObjectsSearch.QueryObj);
                ObjQuery.Operator = "Contains";
                ObjQuery.AttributeValue = objSearch.containingText;
                objSearch.QuerySearchList.push(ObjQuery);
            }
            hubcontext.hubSearch.server.getFilesBySearchCriteria(objSearch, exportToExcel).done(function (data) {
                $scope.$evalAsync(function () {
                    $rootScope.IsLoading = false;
                    $scope.ResultantFiles = data;
                });
            });
        }
    };

    $scope.advanceSearchOpenfile = function (file) {
        // #region smart navigation logic
        if (['WorkflowMap', 'ParameterScenario', 'ObjectScenario', 'ExcelScenario', 'BPMN', 'ExcelMatrix', 'FormLinkLookup', 'FormLinkMaintenance', 'FormLinkWizard'].indexOf(file.FileType) < 0) {
            // set containing text filters if applied - for smart navigation
            var lstQueryBuilder = angular.copy($scope.searchCriteria.QuerySearchList);
            if ($scope.searchCriteria.containingText) {
                var ObjQuery = angular.copy($ObjectsSearch.QueryObj);
                ObjQuery.Operator = "Contains";
                ObjQuery.operatorList = $searchQuerybuilder.getOperatorList();
                ObjQuery.AttributeValue = $scope.searchCriteria.containingText;
                lstQueryBuilder.push(ObjQuery);
            }
            $scope.queryBuilder.setGlobalQueryBuilderFilters(lstQueryBuilder);
        }
        // #endregion       
        $rootScope.openFile(file);
    };

    $scope.togglePanel = function (panelID) {
        var indexActive = $scope.lstactiveSearchPanelIDs.indexOf(panelID);
        if (indexActive >= 0) {
            $scope.lstactiveSearchPanelIDs.splice(indexActive, 1);
        }
        else {
            $scope.lstactiveSearchPanelIDs.push(panelID);
        }
    };
    // #endregion

    //#region Extra field search

    $scope.searchExtraFields = function () {
        if ($scope.objDirFunctions.getExtraFieldData) {
            var extraFieldsData = $scope.objDirFunctions.getExtraFieldData();
            if (extraFieldsData && extraFieldsData.length > 0) {
                hubcontext.hubSearch.server.getExtraFieldFilesBySearchCriteria(extraFieldsData, $scope.SelectedFileType).done(function (data) {
                    $scope.$evalAsync(function () {
                        $rootScope.IsLoading = false;
                        $scope.ResultantExtraFieldFiles = data;
                    });
                });
            }
        }
    };

    $scope.onFileTypeChange = function (fileType) {
        if (fileType) {
            $scope.ResultantExtraFieldFiles = [];
        }
    };

    $scope.clearExtraFieldsList = function () {
        if ($scope.objDirFunctions.resetExtraField) {
            InitCustomSearch();
            $scope.objDirFunctions.resetExtraField();
            $scope.ResultantExtraFieldFiles = [];
        }
    };
    //#endregion

    // constructor call
    SearchFilesController();

    $scope.exportToExcel = function (searchResults) {
        $rootScope.IsLoading = true;
        hubcontext.hubSearch.server.exportSearchResultsToExcel(searchResults).done(function (filePath) {
            $rootScope.IsLoading = false;
            if (filePath) {
                MessageBox("Search Exported", "Search results exported to:\n" + filePath, false, null, { width: 550 });
            }
        });
    };
}]);

