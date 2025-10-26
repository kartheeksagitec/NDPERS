app.controller('NewERDiagram', ['$scope', '$rootScope', '$Chart', '$timeout', function ($scope, $rootScope, $Chart, $timeout) {
    $scope.currentOpenFileName = angular.copy($rootScope.currentopenfile.file.FileName);
    $scope.init = function () {
        $timeout(function () {
            $Chart.drawErDiagram($scope.currentOpenFileName);
        }); 
    };
    $scope.init();
}]);