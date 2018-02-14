/// <reference path="ngInit.js" />

function report($scope, $http) {
    //$scope.rows = [];

    $scope.row_filter = { mainTag_id: null, subTag_id: null }

    $scope.row = {};

    $scope.drp_maintag = [];
    $scope.drp_subtag = [];

    ng_post($http, "../service/getMaintagList", {}, function (data) {
        $scope.drp_maintag = data;
    });

    //$scope.busy = false;


    //$scope.loadSubTag = function () {
    //    var jnPost = { maintagid: $scope.row_filter.mainTag_id };

    //    ng_post($http, "../service/getSubTagList", jnPost, function (data) {
    //        $scope.drp_subtag = data;
    //    });
    //}

    $scope.fetchReport = function () {
        var jnPost = { maintagid: $scope.row_filter.mainTag_id };

        ng_post($http, "../service/setViewData", jnPost, function (data) {
            if (data.result == true) {
                window.location = "../service/downloadReport";
            }
            else {
                alert("No data found");
            }
           
        });
    }
}

myApp.controller("report", report);