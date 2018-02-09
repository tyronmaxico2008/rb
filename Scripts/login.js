/// <reference path="angular.min.1.6.js" />
/// <reference path="ngInit.js" />

function login($scope, $http) {
    $scope.signin = function () {
        $scope.chk = "";

        var jnPost = { username: $scope.username, password: $scope.password };
        
        ng_post($http, "../service/userlogin", jnPost, function (data, status) {
            if (data.result == true) {
                window.location.href = '../Home/Index';
            }
            else {
                alert("Wrong userid pass");
            }
        });
    };
}


myApp.controller("login", login);



