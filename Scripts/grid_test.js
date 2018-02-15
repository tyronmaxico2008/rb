/// <reference path="angular.min.1.6.js" />
/// <reference path="ngCore/ngGRID.js" />



myApp.controller("grid_test", function ($scope, $http) {
    var _grd = new ngGrid($http, "../Service2/sysobjects?path=0");

    $scope.grd = _grd;

    _grd.load();

   
    
    

    
    
});