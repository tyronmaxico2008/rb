/// <reference path="angular.min.1.6.js" />


var myApp = angular.module("myApp", []);


function ng_post($http, slink, jn, callBack) {

    $http.post(slink, getFormData(jn), {
        transformRequest: angular.identity,
        headers: { 'Content-Type': undefined }
    }).then(function (res) {

        callBack(res.data);
    });
}



myApp.directive("pager", function () {
    return {
        restrict: "E",
        replace: true,
        scope: { grd: "=grd" },
        templateUrl: "../Scripts/ngCore/pager.html",
        link: function (scope, element, attrs) {

        }
    }
});

myApp.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            var iSize = (attrs.size || 2);


            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);


function ngGrid($http, sGetPath) {

    var grid = this;

    grid.getPath = sGetPath;

    //Ajax loader
    grid.beforeLoad = [];
    grid.afterLoad = [];
    grid.postJson = [];
    grid.busy = false;
    grid.selectAll = false;

    grid.isError = false;
    grid.errorMessage = "";

    grid.setSubModule = function (val) {
        subModuleName = val;
    }

    grid.addBeforeLoad = function (fn) {
        grid.beforeLoad.push(fn);
    }

    grid.addAfterLoad = function (fn) {
        grid.afterLoad.push(fn);
    }


    grid.addPostJson = function (fn) {
        grid.postJson.push(fn);
    }

    var _beforeLoad = function () {
        $.each(grid.beforeLoad, function () {
            if ($.isFunction(this)) this();
        });
    }

    var _afterLoad = function () {
        $.each(grid.afterLoad, function () {
            if ($.isFunction(this)) this();
        });
    }

    var fnPostJson = function (d) {
        $.each(grid.postJson, function () {
            if ($.isFunction(this)) this(d);
        });
    };


    this.row = {};


    this.edit = function (r) {
        this.row = r;
    }


    this.onError = function (sMsg) {
        alert(sMsg);
    }

    this.loadOnEnter = function (e) {
        if (e.which == 13)
            this.load();
    }


    this.getCheckedValues = function (Field_value, checkField, seprator) {

        var seprator1 = seprator == undefined ? "," : seprator;
        var lst = [];

        for (var i = 0; i < this.rows.length; i++)
            if (this.rows[i][checkField])
                lst.push(this.rows[i][Field_value]);

        return lst.join(seprator1);
    }



    this.rows = [];
    this.count = 0;
    this.pageIndex = 0;
    this.pageSize = "20";
    this.pageButtons = [0, 1, 2, 3];



    this.searchOnEnter = function (e) {

        if (e.which == 13) {
            this.pageIndex = 0;
            this.load();
        }

    }

    this.search = function (e) {
        this.pageIndex = 0;
        this.load();
    }


    this.ActiveClass = function (r) {
        return r == this.pageIndex ? 'active1' : '';
    }

    this.getSortClass = function (sField) {

        if (sField == this.sort_field) {
            switch (this.sort_type) {
                case "asc":
                    return "fa-sort-alpha-asc";
                case "desc":
                    return "fa-sort-alpha-desc";
                default:
                    return "fa-sort"
            }
        }
        else {
            return "fa-sort"
        }
    }


    //Sorting setting 
    this.sort_type = ""
    this.sort_field = ""

    this.sort = function (sField, e) {
        this.sort_field = sField;
        this.sort_type = this.sort_type == "asc" ? "desc" : "asc";
        this.load(null, e);
    }
    /////////////////


    this.getPageCount = function () {
        return Math.ceil(this.count / this.pageSize);
    }

    this.changePage = function (iPageIndex, e) {
        this.pageIndex = iPageIndex;
        this.load(null, e);
    }

    this.MoveToFirstPage = function () {
        this.pageButtons[0] = 0
        this.pageButtons[1] = 1
        this.pageButtons[2] = 2
        this.pageButtons[3] = 3


        this.pageIndex = this.pageButtons[0];
        this.load();


    }

    this.MoveToLastPage = function () {

        while (this.pageButtons[3] <= this.getPageCount() - 1) {
            this.pageButtons[0] += 4
            this.pageButtons[1] += 4
            this.pageButtons[2] += 4
            this.pageButtons[3] += 4
        }

        this.pageIndex = this.pageButtons[0];
        this.load();
    }




    this.MoveNext = function () {

        if (this.pageButtons[3] >= this.getPageCount()) {
            //alert(this.pageButtons[3] + ": " + this.getPageCount());
            return;
        }




        this.pageButtons[0] += 4
        this.pageButtons[1] += 4
        this.pageButtons[2] += 4
        this.pageButtons[3] += 4


        this.pageIndex = this.pageButtons[0];
        this.load();

    }




    this.MovePrevious = function () {
        if (this.pageButtons[0] <= 0) return;

        this.pageButtons[0] -= 4
        this.pageButtons[1] -= 4
        this.pageButtons[2] -= 4
        this.pageButtons[3] -= 4
        this.pageIndex = this.pageButtons[0];
        this.load();
    }

    this.setButtons = function () {
    }


    this.load = function (callBack, e) {


        var jnPost = {};


        if (this.sort_type != "" && this.sort_field != "") {
            jnPost["$sort"] = this.sort_field + " " + this.sort_type;
        }


        if (fnPostJson != undefined) fnPostJson(jnPost);

        _beforeLoad();
        grid.busy = true;

        ng_post($http, sGetPath + "&start=" + this.pageIndex + "&length=" + this.pageSize, jnPost, function (data) {

            if (data.error == true) {
                grid.isError = true;
                grid.errorMessage = data.error_msg;
            }
            else {
                grid.isError = false;
                grid.errorMessage = "";

                grid.rows = data.data;
                grid.count = data.recordsTotal;
            }


            if ($.isFunction(callBack)) callBack();

            _afterLoad();
            grid.busy = false;
            //this.setButtons();

        }, e);
    }


    this.loadAll = function (dPostData, callBack, e) {
        /*

        var jnPost = null;


        if ($.isPlainObject(dPostData))
            jnPost = dPostData;
        else
            if ($.isFunction(fnPostJson)) {
                var jnPost = {};
                fnPostJson(jnPost);
            }


        _beforeLoad();

        bll.execJson(sGetPath, jnPost, function (data) {
            if ($.isArray(data)) {
                grid.rows = data;
                grid.count = data.length;

                if ($.isFunction(callBack)) callBack();

                _afterLoad();
            }
        }, e);
        */
        alert("Not required");
    }

    this.selectById = function (iId, callback, e) {


        if ($.isNumeric(iId) == false || parseInt(iId) == 0) {
            this.row = {};
            return;
        }

        var jnPost = {};
        jnPost[this.PrimaryKeyField] = iId;

        _beforeLoad();
        bll.execJson(sGetPath, jnPost, function (data) {
            if (data.length > 0) {
                grid.row = data[0];
                if ($.isFunction(callback)) callback();
            }
            _afterLoad();
        }, e);
    }


    this.selectByFilter = function (filterData, callback, e) {
        _beforeLoad();
        grid.busy = true;
        bll.execJson(sGetPath, filterData, function (data) {
            if (data.length > 0) {
                grid.row = data[0];
                if ($.isFunction(callback)) callback();
            }
            _afterLoad();
            grid.busy = false;
        }, e);
    }

    //Alter

    this.downloadSQLReport = function (sReportName, sType, e) {
        var jnPost = {};
        fnPostJson(jnPost);
        bll.downloadSQLReport(sReportName, sType, jnPost, e);

        //bll.downloadSQLReport("ap_mlm:customer_list", "Excel", $scope.row_filter, e);
    }

    ///designed for VUE

    this.fill = function () {
        self = this;
        self.busy = true;

        bll.execGrid(this.getPath, this.pageIndex, this.pageIndex * this.pageSize, this.pageSize, this.filterData, function (res) {

            if (res.error == true) {
                self.isError = true;
                self.errorMessage = res.error_msg;
            }
            else {
                self.isError = false;
                self.errorMessage = "";

                self.rows = res.data;
                self.count = res.recordsTotal;
            }
        });

        /*
        bll.getDataPaging(this.getPath,this.filterData,this.pageIndex,this.pageSize,function(res){

            if(res.error == true)
            {
                self.isError = true;
                self.errorMessage = res.error_msg;
            }
            else
            {
                self.isError = false;
                self.errorMessage = "";

                self.rows = res.data;
                self.count = res.recordsTotal;
            }

            self.busy = false;

        });
        */
    }

    this.setPage = function (iPageIndex) {
        debugger;

        //Control start 
        if ((iPageIndex < 0)) {
            alert("You are on the first page !");
            return;
        }

        if (iPageIndex >= this.getPageCount()) {
            alert("You are on the last page !");
            return;
        }

        //Control end

        this.pageIndex = iPageIndex;
        this.fill();
    }

    this.setPageSize = function () {
        this.pageIndex = 0;
        this.fill();
    }

}


function ngCRUD(bll, sGetPath, sSavePath, sDeletePath, PrimaryKeyField) {

    var grd = new ngGrid(bll, sGetPath);

    grd.PrimaryKeyField = PrimaryKeyField;

    grd.row_copy = null;

    grd.formClear = function () {
        grd.row = { id: 0 };
    }

    grd.downloadFile = function (r, sField) {

        /*
        var iID = 0;
        iID = r[grd.PrimaryKeyField];
        var sPath = ng.getlinkDownloadFile(grd.ModuleName, sField, iID);
        document.location.href = sPath;
        */
    }

    grd.exec = function (row, sPath, e, callback) {

        if (grd.beforeSave != undefined) {
            if (!grd.beforeSave()) return false;
        }

        r = row == undefined || row == null ? grd.row : row;

        bll.UpdateModule(sPath, r, function (status, data) {
            if (status == "success") {

                //grd.formClear();

                if (grd.afterSave != undefined && $.isFunction(grd.afterSave)) {
                    //grd.afterSave(data);
                }

                if ($.isFunction(callback)) callback();
            }
        }, e);
    }

    grd.beforeSave = null;

    grd.addBeforeSave = function (fn) {
        grd.beforeSave = fn;
    }

    grd.afterSave = null;

    grd.addAfterSave = function (fn) {
        grd.afterSave = fn;
    }

    grd.save = function (callback, e) {

        if (grd.beforeSave != undefined) {
            if (!grd.beforeSave()) return false;
        }

        bll.UpdateModule(sSavePath, grd.row, function (status, data, info) {

            if (status == "success") {

                grd.formClear();

                if (grd.afterSave != undefined && $.isFunction(grd.afterSave)) {
                    grd.afterSave(data, info);
                }

                if ($.isFunction(callback)) callback(data, info);

            }
            else if (status == "error") {
                //grd.onError(data);
            }
        }, e, false);
    }


    //grd.save_others = function (callback, e) {
    //    grd.exec(ActionName, e, callback);
    //}

    //grd.edit = function (r) {
    //    grd.row = clone(r);
    //}

    grd.copy = function (r) {
        grd.row_copy = r == undefined ? clone(grd.row) : clone(r);
    }

    grd.paste = function () {
        grd.row = clone(grd.row_copy);
        grd.row.id = 0;
    }


    grd.del = function (r, callBack, e) {
        if (!confirm("Are you sure want to delete selected record ?")) return;
        bll.UpdateModule(sDeletePath, { id: r[PrimaryKeyField] }, function (status) {

            if (status == "success") {
                //if (callBack != undefined) grid.load();
                grd.formClear();

                if ($.isFunction(callBack)) {
                    callBack();
                }
                else if (callBack == undefined) {
                    grd.load();
                }
            }
        }, e);
    }

    return grd;
}



function ngCRUD($http, sGetPath, sSavePath, sDeletePath, PrimaryKeyField) {

    var grd = new ngGrid($http, sGetPath);

    grd.PrimaryKeyField = PrimaryKeyField;

    grd.row_copy = null;

    grd.formClear = function () {
        grd.row = { id: 0 };
    }

    grd.beforeSave = null;

    grd.addBeforeSave = function (fn) {
        grd.beforeSave = fn;
    }

    grd.afterSave = null;

    grd.addAfterSave = function (fn) {
        grd.afterSave = fn;
    }

    grd.save = function (callback, e) {

        if (grd.beforeSave != undefined) {
            if (!grd.beforeSave()) return false;
        }

        ng_post($http,sSavePath, grd.row, function (status, data, info) {

            if (status == "success") {

                grd.formClear();

                if (grd.afterSave != undefined && $.isFunction(grd.afterSave)) {
                    grd.afterSave(data, info);
                }

                if ($.isFunction(callback)) callback(data, info);

            }
            else if (status == "error") {
                //grd.onError(data);
            }
        }, e, false);
    }


    //grd.save_others = function (callback, e) {
    //    grd.exec(ActionName, e, callback);
    //}

    grd.edit = function (r) {
        grd.row = r;
    }

    grd.copy = function (r) {
        grd.row_copy = r == undefined ? clone(grd.row) : clone(r);
    }

    grd.paste = function () {
        grd.row = clone(grd.row_copy);
        grd.row.id = 0;
    }


    grd.del = function (r, callBack, e) {
        if (!confirm("Are you sure want to delete selected record ?")) return;
        ng_post($http,sDeletePath, { id: r[PrimaryKeyField] }, function (status) {

            if (status == "success") {
                //if (callBack != undefined) grid.load();
                grd.formClear();

                if ($.isFunction(callBack)) {
                    callBack();
                }
                else if (callBack == undefined) {
                    grd.load();
                }
            }
        }, e);
    }

    return grd;
}

