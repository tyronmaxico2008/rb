var myApp = angular.module("myApp", []);

//commit test

function getFormData(fields)
{
    var fd = new FormData();

    for (var f in fields) {
        if (fields[f] != null && fields[f] != undefined)
            fd.append(f, fields[f]);
    }

    return fd;
}

function tagging($scope, $http) {
    //$scope.rows = [];
    $scope.row = { mainTag_id :null, subTag_id :null }
    $scope.drp_maintag = [];
    $scope.drp_subtag = [];
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


}

myApp.controller("tagging", tagging);