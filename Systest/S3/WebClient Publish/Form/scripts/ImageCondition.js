app.controller("ImageConditionController", ["$scope", "$rootScope", "ngDialog", "$SgMessagesService", function ($scope, $rootScope, ngDialog, $SgMessagesService) {
    //var tempconditions = JSON.stringify($scope.oldConditions.Elements);
    //$scope.conditions = JSON.parse(tempconditions);
    $scope.conditions = [];
    angular.copy($scope.oldConditions.Elements, $scope.conditions);
    $scope.onImageCondtionSaveClick = function (conditions) {
        $scope.model.Elements = [];
        if (conditions.length > 0) {
            $scope.model.Elements.push(conditions[0]);
        }
        $scope.onCancelClick();
    };
    $scope.AddNewImageCondtion = function () {
        if ($scope.conditions && $scope.conditions.length > 0) {
            var obj = { dictAttributes: { value: "", ImageUrl: "" }, Elements: [], Children: [], Name: "condition", Value: "" };
            $scope.conditions[0].Elements.push(obj);
            $scope.selectedImageCondition = obj;
        }
        else {
            var obj = [{ dictAttributes: {}, Elements: [{ dictAttributes: { value: "", ImageUrl: "" }, Elements: [], Children: [], Name: "condition", Value: "" }], Children: [], Name: "Conditions", Value: "" }];
            $scope.conditions = obj;
            if ($scope.conditions.length > 0 && $scope.conditions[0].Elements && $scope.conditions[0].Elements.length > 0) {
                $scope.selectedImageCondition = $scope.conditions[0].Elements[$scope.conditions[0].Elements.length - 1];
            }
        }
    };
    $scope.deleteImageCondition = function () {
        if ($scope.conditions[0].Elements.length > 1) {
            var index = $scope.conditions[0].Elements.indexOf($scope.selectedImageCondition);
            $scope.conditions[0].Elements.splice(index, 1);
            if (index < $scope.conditions[0].Elements.length) {
                $scope.selectedImageCondition = $scope.conditions[0].Elements[index];
            }
            else if ($scope.conditions[0].Elements.length > 0) {
                $scope.selectedImageCondition = $scope.conditions[0].Elements[index - 1];
            }
        }
        else {
            $scope.conditions = [];
            $scope.selectedImageCondition = undefined;
        }
    };
    $scope.setRowSelection = function (Imagecondition) {
        $scope.selectedImageCondition = Imagecondition;
    };

    $scope.onCancelClick = function () {
        $scope.ImageConditionDialog.close();
    };

    $scope.OpenImageUrl = function (openFrom, index) {
        if ($scope.formobject.dictAttributes.WebSite) {

            $.connection.hubForm.server.openImageUrlClick($scope.formobject.dictAttributes.WebSite, openFrom).done(function (data) {
                if (data && data.length == 2) {
                    $scope.receiveImageFileName(data[0], data[1], index);
                }
            });

        }
        else {
            $SgMessagesService.Message('Message', "Please select the appropriate 'Website' from the form details page.");

        }
    };

    $scope.receiveImageFileName = function (fileName, errorMessage, index) {
        if (fileName != "") {
            $scope.$apply(function () {
                $scope.conditions[0].Elements[index].dictAttributes.ImageUrl = fileName;
            });
        }
        if (errorMessage != "" && errorMessage != undefined) {
            $SgMessagesService.Message('Message', errorMessage);
        }
    };
}]);