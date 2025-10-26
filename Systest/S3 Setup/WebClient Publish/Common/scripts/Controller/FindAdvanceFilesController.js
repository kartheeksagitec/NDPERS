app.controller("FindAdvanceFilesController", ['$scope', '$rootScope', 'hubcontext', '$resourceFactory', function ($scope, $rootScope, hubcontext, $resourceFactory) {

    $scope.Init = function () {
        $scope.AdvanceSearch = {};      
        $scope.AdvanceSearch.filetypes = $resourceFactory.getFileTypeList();
        $scope.AdvanceSearch.searchFileType = "All";
        $scope.AdvanceSearch.PageIndex = 0;
        $scope.totalDisplayed = 20;
    };

    $scope.searchTrigger = function () {
        $scope.ngpausescroll = true;
        $scope.before = performance.now();
        $rootScope.IsLoading = true;
        hubcontext.hubSearch.server.getFileFindAdvance($scope.getSearchObj());
    };
    $scope.searchFileSelect = function (file) {
        $scope.FindDialog.close();
        $rootScope.openFile(file.FileInfo);
    };
    $scope.resetFind = function () {
        $scope.AdvanceSearch.PageIndex = 0;
    };
    $("#findFilesDiv").scroll(function () {
        if ($scope.totalDisplayed >= $scope.AdvanceSearch.findSearchList.length && !$scope.ngpausescroll) {
            $scope.AdvanceSearch.PageIndex = $scope.AdvanceSearch.PageIndex + 1;
            $scope.searchTrigger();
        }
        else if ($scope.totalDisplayed < $scope.AdvanceSearch.findSearchList.length && !$scope.ngpausescroll){
            if ($(this)[0].scrollTop + $(this)[0].offsetHeight == $(this)[0].scrollHeight)
                if ($scope.$$phase) {
                    $scope.$evalAsync(function () {
                        $scope.totalDisplayed += 20;
                    });
                } else {
                    $scope.$apply(function () {
                        $scope.totalDisplayed += 20;
                    });
                }                         
        }
    });
    $scope.getSearchObj = function () {
        return {
            SearchText: $scope.AdvanceSearch.searchText,
            MatchCase: $scope.AdvanceSearch.matchCase,
            MatchWholeWord: $scope.AdvanceSearch.matchWholeWord,
            SearchPattern: $scope.AdvanceSearch.searchFileType,
            PageIndex: $scope.AdvanceSearch.PageIndex
        };
    };
    $scope.setFileFindAdvance = function (findObj, data) {
        $scope.ngpausescroll = false;
        //console.log("search text : " + $scope.AdvanceSearch.searchText + " with match case : " + ($scope.AdvanceSearch.matchCase ? true : false) + " with matchwholeword : " + ($scope.AdvanceSearch.matchWholeWord ? true : false) + " took => " + (performance.now() - $scope.before) / 1000);
        $scope.$evalAsync(function () {
            $rootScope.IsLoading = false;
            $scope.AdvanceSearch.CountFilesParsed = findObj.CountFilesParsed;
            if (findObj.PageIndex == 1) {
                $scope.totalDisplayed = 20;
                if (data != null) $scope.AdvanceSearch.findSearchList = data;
            }
            else {
                if (data != null) $scope.AdvanceSearch.findSearchList = $scope.AdvanceSearch.findSearchList.concat(data);
            }
            // if result is less than the item displayed and all files have not been traversed yet
            if ($scope.AdvanceSearch.findSearchList.length <= $scope.totalDisplayed && findObj.ScrollLoadEnable) {
                $scope.AdvanceSearch.PageIndex = $scope.AdvanceSearch.PageIndex + 1;
                $scope.searchTrigger();
            }
        });
    };
    $scope.Init();
}]);

