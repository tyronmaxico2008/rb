/// <reference path="ngInit.js" />

function tagging($scope, $http) {
    //$scope.rows = [];

    $scope.row_filter = { mainTag_id: null, subTag_id: null, indexfield: null, docLocation: "Mumbai" }

    $scope.row = {};

    $scope.drp_maintag = [];
    $scope.drp_subtag = [];
    $scope.indexfield = [];
    $scope.showsave = false;
    $scope.currentPage = 0;
    $scope.totalPages = 0;

    $scope.inProcess = false;

    //$scope.busy = false;

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

            for (var i = 0; i < $scope.indexfield.length; i++) {
                var r = $scope.indexfield[i];

                if (r.indexval == "Box Barcode No") {
                    r.val = $scope.row.BoxBarcode;
                }
                if (r.indexval == "File Barcode No") {
                    r.val = $scope.row.FileBarcode;
                }
                if (r.indexval == "Invoice No") {
                    r.val = $scope.row.InvoiceNo;
                }
                if (r.indexval == "Invoice Date") {
                    r.val = $scope.row.InvoiceDate;
                }
                if (r.indexval == "Sales Return No") {
                    r.val = $scope.row.SRNNo;
                }
                if (r.indexval == "Sales Return Date") {
                    r.val = $scope.row.SRNDate;
                }
                if (r.indexval == "ARA Doc No") {
                    r.val = $scope.row.ARADocNo;
                }
                if (r.indexval == "ARA City") {
                    r.val = $scope.row.ARACity;
                }
                if (r.indexval == "STN No") {
                    r.val = $scope.row.STNNo;
                }
                if (r.indexval == "STNDate") {
                    r.val = $scope.row.STNDate;
                }
                if (r.indexval == "From Location") {
                    r.val = $scope.row.STNFrom;
                }
                if (r.indexval == "To Location") {
                    r.val = $scope.row.STNTo;
                }
                if (r.indexval == "LR No") {
                    r.val = $scope.row.LRNo;
                }
                if (r.indexval == "Comment") {
                    r.val = $scope.row.Comment;
                }
                if (r.indexval == "BillingLocation") {
                    r.val = $scope.row.BillingLocation;
                }
                if (r.indexval == "SRN Location") {
                    r.val = $scope.row.BillingLocation;
                }
                if (r.indexval == "Customer Name") {
                    r.val = $scope.row.CustomerName;
                }


            }

            $scope.showsave = true;
        });
    }

    $scope.getFileBarCodeData = function () {
        debugger;
        var jnPost = { getbarcode: $scope.getbarcode, docLocation: $scope.row_filter.docLocation };
        $scope.inProcess = true;
        ng_post($http, "../service/getFileBarCodeData", jnPost, function (rows) {
            if (rows.length > 0) {
                $scope.row = rows[0];
                $scope.getTotalDoc();
                $scope.indexfield = [];
                $scope.row_filter.mainTag_id = "";
                $scope.row_filter.subTag_id = "";
            }
            else {
                $scope.inProcess = false;
                alert("No record found !");

            }
        });
    }

    $scope.getTotalDoc = function () {
        var jnPost = { docLocation: $scope.row_filter.docLocation };

        ng_post($http, "../service/getTotalDoc", jnPost, function (rows) {
            debugger;
            $scope.totalDocleft = rows[0]["rec_count"];
        });
    }

    $scope.releaseDoc = function () {
        var jnPost = { filebarcode: $scope.row.FileBarcode };

        ng_post($http, "../service/releaseDoc", jnPost, function (rows) {
            $scope.inProcess = false;
        });
    }

    $scope.moveNextPage = function () {
        debugger;
        if ($scope.totalPages >= $scope.currentPage) {
            $scope.currentPage += 1;
            $scope.row_filter = { mainTag_id: $scope.row_filter.mainTag_id, subTag_id: $scope.row_filter.subTag_id, docLocation: $scope.row_filter.docLocation };
        }
        else {
            $scope.getFileBarCodeData();
        }
    }

    $scope.save_indexfield = function () {
        //debugger;

        jnPost = { dumpid: $scope.row.dumpid, fileBarCode: $scope.row.FileBarcode }
        jnPost['currentPage'] = $scope.currentPage;
        jnPost["maintagid"] = $scope.row_filter.mainTag_id;
        jnPost["subtagid"] = $scope.row_filter.subTag_id;
        jnPost['indexField'] = JSON.stringify($scope.indexfield);
        jnPost['docLocation'] = $scope.row_filter.docLocation;

        ///////validation code
        for (var i = 0; i < $scope.indexfield.length; i++) {
            var r = $scope.indexfield[i];
            //alert(r.indexval);
            //alert(r.val)
            //r['invoice']
            if (r.indexval == "Invoice No" && r.val == "") {
                alert("Please enter the invoice number");
                return;
            }
            if (r.indexval == "Invoice Date" || r.indexval.trim() == "SRNDate" || r.indexval.trim() == "STNDate") {
                var input = r.val;
                var pattern = /^([0-9]{2})[- /.]([0-9]{2})[- /.]([0-9]{4})$/;
                if (pattern.test(input) == false) {
                    alert("Date is not in correct format");
                    return;
                }

            }

        }
        //for (var i = 0; i < $scope.indexfield.length; i++) {
        //    var r = $scope.indexfield[i];

        //    //var input = '01.01.1997';
        //    //
        //    //var pattern = /^([0-9]{2})[- /.]([0-9]{2})[- /.]([0-9]{4})$/;
        //    //
        //    //alert(pattern.test(input));
        //    if (r.indexval == "Invoice Date") {
        //        var input = r.val;
        //        var pattern = /^([0-9]{2})[- /.]([0-9]{2})[- /.]([0-9]{4})$/;
        //        if (pattern.test(input) == false) {
        //            alert("Date is not in correct format");
        //            return;
        //        }

        //    }
        //}

        //////validation code ends here

        ng_post($http, "../Service/save_indexField", jnPost, function (res) {
            if (res.result == true) {
                alert("Operation done successfully.");
                if ($scope.totalPages > $scope.currentPage) {
                    $scope.moveNextPage();
                }
                else {
                    $scope.inProcess = false;
                    $scope.getbarcode = "";
                    $scope.getFileBarCodeData();
                }
            }
            else {
                alert(res.msg);
            }


        });

    }

    $scope.delete = function () {
        //debugger;

        jnPost = { dumpid: $scope.row.dumpid, fileBarCode: $scope.row.FileBarcode }
        jnPost['currentPage'] = $scope.currentPage;
        //jnPost["maintagid"] = $scope.row_filter.mainTag_id;
        //jnPost["subtagid"] = $scope.row_filter.subTag_id;
        //jnPost['indexField'] = JSON.stringify($scope.indexfield);
        jnPost['docLocation'] = $scope.row_filter.docLocation;

        ng_post($http, "../Service/delete", jnPost, function (res) {
            if (res.result == true) {
                alert("Operation done successfully.");
                if ($scope.totalPages > $scope.currentPage) {
                    $scope.moveNextPage();
                }
                else {
                    $scope.inProcess = false;
                    $scope.getbarcode = "";
                    $scope.getFileBarCodeData();
                }
            }
            else {
                alert(res.msg);
            }


        });

    }




}


myApp.controller("tagging", tagging);
