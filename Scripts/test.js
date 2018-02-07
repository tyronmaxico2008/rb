var myApp = angular.module("myApp", []);

//commit test

function getFormData(fields) {
    var fd = new FormData();

    for (var f in fields) {
        if (fields[f] != null && fields[f] != undefined)
            fd.append(f, fields[f]);
    }

    return fd;
}

function tagging($scope, $http) {
    //$scope.rows = [];
    $scope.row = { mainTag_id: null, subTag_id: null, indexfield: null }
    $scope.drp_maintag = [];
    $scope.drp_subtag = [];
    $scope.indexfield = [];
    $scope.showsave = false;
    //$scope.busy = false;

    $http.post("../service/getMaintagList").then(function (res) {
        $scope.drp_maintag = res.data;
        //$scope.busy = false;
    });

    $scope.loadSubTag = function () {

        var jnPost = { maintagid: $scope.row.mainTag_id };

        $http.post("../service/getSubTagList", getFormData(jnPost), {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.drp_subtag = res.data;
            
        });
    }

    $scope.loadIndexfield = function () {
        var jnPost = { subtagid: $scope.row.subTag_id };

        $http.post("../service/getIndexFieldList", getFormData(jnPost), {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.indexfield = res.data;
            $scope.showsave = true;
            
        });
    }


}

function login($scope, $http) {
    $scope.signin = function () {
        $scope.chk="";
        var jnPost = { username: $scope.username, password: $scope.password };
        
        $http.post("../service/userlogin", getFormData(jnPost), {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            if (res.data.result == true)
            {
                window.location.href = '../Home/Index';


            }
           
            else {

                alert("Wrong userid pass");

            }

        }).error(function () {

            alert('failed');

        })

    };
}

myApp.controller("tagging", tagging);
myApp.controller("login", login);