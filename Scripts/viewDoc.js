/// <reference path="ngInit.js" />
/// <reference path="angular.min.1.6.js" />


myApp.directive("docFilter", function ($http) {
    return {
        templateUrl: "../Scripts/views/docFilter.html"
        , controller: function ($scope) {

            $scope.row_filter = { mainTag_id: null, subTag_id: null, indexfield: null, docLocation: "Mumbai" }
            $scope.drp_maintag = [];
            $scope.drp_subtag = [];
            $scope.indexfield = [];

            ng_post($http, "../service/getMaintagList", {}, function (data) {
                $scope.drp_maintag = data;
            });

            //$scope.busy = false;


            $scope.loadSubTag = function () {
                var jnPost = { maintagid: $scope.row_filter.mainTag_id };

                ng_post($http, "../service/getSubTagList", jnPost, function (data) {
                    $scope.drp_subtag = data;
                });
            }



            $scope.loadIndexfield = function () {
                var jnPost = { subtagid: $scope.row_filter.subTag_id };

                ng_post($http, "../service/getIndexFieldList", jnPost, function (data) {
                    $scope.indexfield = data;
                    debugger;
                });
            }


        }

    }
});

myApp.controller("searchDoc", function ($scope, $http) {

    $scope.rows = [];
    $scope.current_row = {};
    $scope.datares = [];

    $scope.load = function () {
        var jnPost = { maintagid: $scope.row_filter.mainTag_id, subtagid: $scope.row_filter.subTag_id };
        ng_post($http, "../Service/getData_filelist", jnPost, function (_rows) {
            $scope.rows = _rows;
        });
    }

    //ronit change

    $scope.showRowData = function (r) {
        $scope.current_row = r;
        jnPost = { id: $scope.current_row.id };
        ng_post($http, "../Service/fetchIndexField", jnPost, function (row) {
            $scope.datares = row;
        });
        $("#divViewPDF").modal("show");
    }

    //change ends
    //alert("Hiii");



});