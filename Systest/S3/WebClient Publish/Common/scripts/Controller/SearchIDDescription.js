app.controller("SearchIDDescriptionController", ["$scope", "$rootScope", "$timeout", function ($scope, $rootScope, $timeout) {

    //#region Init Methods
    $scope.Search = { ID: "", Description: "" };
    $scope.Init = function () {
        $scope.SearchResultCollection = [];
    };
    //#endregion 

    //#region Common Methods
    $scope.PopulateSearchResult = function () {
        $scope.SearchResultCollection = [];
        function iterator(obj) {
            var objSearchResult = {};
            if ($scope.SearchResultCollection.length < 100) {
                if ($scope.strCode == "Codegroups") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.CodeID && obj.CodeID.contains($scope.Search.ID) && obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.CodeID && obj.CodeID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeID, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else if ($scope.strCode == "Codeiddescription") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.CodeValue && obj.CodeValue.toLowerCase().contains($scope.Search.ID.toLowerCase()) && obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.CodeValue && obj.CodeValue.toLowerCase().contains($scope.Search.ID.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.Description && obj.Description.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.CodeValue, Description: obj.Description };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else if ($scope.strCode == "Resources") {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.ResourceID && obj.ResourceID.contains($scope.Search.ID) && obj.ResourceDescription && obj.ResourceDescription.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.ResourceID && obj.ResourceID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.ResourceDescription && obj.ResourceDescription.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.ResourceID, Description: obj.ResourceDescription };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
                else {
                    if ($scope.Search.ID != undefined && $scope.Search.ID != "" && $scope.Search.Description != undefined && $scope.Search.Description != "") {
                        if (obj.MessageID && obj.MessageID.contains($scope.Search.ID) && obj.DisplayMessage && obj.DisplayMessage.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID != undefined || $scope.Search.ID != "") && ($scope.Search.Description == undefined || $scope.Search.Description == "")) {
                        if (obj.MessageID && obj.MessageID.contains($scope.Search.ID)) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                    else if (($scope.Search.ID == undefined || $scope.Search.ID == "") && ($scope.Search.Description != undefined || $scope.Search.Description != "")) {
                        if (obj.DisplayMessage && obj.DisplayMessage.toLowerCase().contains($scope.Search.Description.toLowerCase())) {
                            objSearchResult = { ID: obj.MessageID, Description: obj.DisplayMessage, SeverityValue: obj.SeverityValue };
                            $scope.SearchResultCollection.push(objSearchResult);
                        }
                    }
                }
            }
        }

        if ($scope.strCode == "Codegroups") {
            angular.forEach($scope.codegrouplist, iterator);
        }
        else if ($scope.strCode == "Codeiddescription") {
            angular.forEach($scope.codevalueslist, iterator);
        }
        else if ($scope.strCode == "Resources") {
            angular.forEach($scope.resourcelist, iterator);
        }
        else if ($scope.strCode == "Messages") {
            angular.forEach($scope.lstMessages, iterator);
        }
    };
    //#endregion

    //#region Common Events
    $scope.SelectSearchObject = function (obj) {
        $scope.SelectedSearchResult = obj;
    };

    $scope.onOkClick = function () {
        $scope.$emit("onOKClick", $scope.SelectedSearchResult);
        $timeout(function () {
            if ($scope.validateResource) $scope.validateResource(); // validate resource
            if ($scope.validateMessageID) $scope.validateMessageID(); // validate message id
        });
        $scope.onCancelClick();
    };

    $scope.onCancelClick = function () {
        //ngDialog.close($scope.dialog.id);
        $scope.SearchIDDescrDialog.close();
    };
    //#endregion

    $scope.Init();
}]);

