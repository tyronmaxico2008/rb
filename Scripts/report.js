/// <reference path="ngInit.js" />

function report($scope, $http) {
    //$scope.rows = [];

    $scope.row_filter = { mainTag_id: null, docLocation: "Mumbai", uid: null }

    $scope.row = {};

    

    $scope.drp_maintag = [];
    $scope.drp_subtag = [];

    ng_post($http, "../service/getMaintagList", {}, function (data) {
        $scope.drp_maintag = data;
    });

    ng_post($http, "../service/getUserList", {}, function (data) {
        $scope.drp_users = data;
    });
    //$scope.busy = false;


    //$scope.loadSubTag = function () {
    //    var jnPost = { maintagid: $scope.row_filter.mainTag_id };

    //    ng_post($http, "../service/getSubTagList", jnPost, function (data) {
    //        $scope.drp_subtag = data;
    //    });
    //}

    $scope.busy = false;

    $scope.fetchReport = function () {


        var jnPost = { maintagid: $scope.row_filter.mainTag_id, docLocation: $scope.row_filter.docLocation };
        $scope.busy = true;
        ng_post($http, "../service/setViewData", jnPost, function (data) {
            if (data.result == true) {
                window.location = "../service/downloadReport";
            }
            else {
                alert("No data found");
            }
            $scope.busy = false;
        });
    }

    $scope.fetchReportUserWise = function () {
        var jnPost = { maintagid: $scope.row_filter.mainTag_id, uid: $scope.row_filter.uid };

        ng_post($http, "../service/setViewDataUserWise", jnPost, function (data) {
            if (data.result == true) {
                window.location = "../service/downloadReport";
            }
            else {
                alert("No data found");
            }

        });
    }


}

function bulkupload($scope, $http) {
    $scope.row_csv = { file: null }
    $scope.bulkUpload = function () {
        //var jnPost = { filepath: $scope.filepath };
        ng_post($http, "../Service/bulkUpload", $scope.row_csv, function (data) {
            if (data.result == true)
                alert("Data Uploaded Successfully");
            else
                alert("Data Not Uploaded");

        });
    }
}

function userReport($scope, $http) {
    $scope.usr = {frmdatepicker: null, todatepicker: null};
    $scope.tabshow = true;

    $scope.fetchReportDocumentWise = function () {
        var jnPost = { frmdatepicker: $scope.frmdatepicker, todatepicker: $scope.todatepicker };

        ng_post($http, "../service/setViewDataDocumentWise", jnPost, function (data) {
            if (data.length > 0) {
                $scope.usr = data;
                $scope.tabshow = false;
            }
            else
                alert("No record found");

        });
    }
}

myApp.controller("report", report);

myApp.controller("bulkupload", bulkupload);

myApp.controller("userReport", userReport);