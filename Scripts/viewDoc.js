/// <reference path="ngInit.js" />
/// <reference path="angular.min.1.6.js" />


//myApp.directive("docFilter", function ($http) {
//    return {
//        templateUrl: "../Scripts/views/docFilter.html"
//        , scope: { rowFilter: "=row_filter", indexField: "=indexfield" }
//        , controller: function ($scope) {

//            debugger;
//            //$scope.indexfield = [];



//            //$scope.busy = false;



//        }

//    }
//});

myApp.controller("searchDoc", function ($scope, $http) {


    $scope.row_filter = { mainTag_id: null, subTag_id: null, indexfield: null };
    $scope.indexfield = [];
    var _grdFile = new ngGrid($http, "../Service2/getData_filelist?path=0");

    _grdFile.addPostJson(function (d) {
        d['maintagid'] = $scope.row_filter.mainTag_id;
        d['subtagid'] = $scope.row_filter.subTag_id;
        d['indexFieldData'] = JSON.stringify($scope.indexfield);
    });

    $scope.grdFile = _grdFile;
    $scope.current_row = {};
    $scope.datares = [];

    
    ///////////////
    $scope.drp_maintag = [];
    $scope.drp_subtag = [];

    ng_post($http, "../service/getMaintagList", {}, function (data) {
        $scope.drp_maintag = data;
    });



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

    /////////////



    //$scope.load = function () {
    //    debugger;
    //    //var _grd = new ngGrid($http, "../Service2/sysobjects?path=0");

    //    //$scope.grd = _grd;

    //    //_grd.load();

    //    var jnPost = { maintagid: $scope.row_filter.mainTag_id, subtagid: $scope.row_filter.subTag_id };
    //    jnPost['indexFieldData'] = JSON.stringify($scope.indexfield);

    //    ng_post($http, "../Service/getData_filelist", jnPost, function (_rows) {
    //        $scope.rows = _rows;

           
    //    });

        
    //}

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