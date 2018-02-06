var myApp = angular.module("myApp", []);

function tagging($scope, $http) {
    $scope.rows = [];
    //$scope.busy = false;


    $http.get("../service/getMaintagList").then(function (res) {
        $scope.rows = res.data;
        //$scope.busy = false;
    });

}

myApp.controller("tagging", tagging);