/// <reference path="ngInit.js" />

function user($scope, $http) {
    var _grd = new ngCRUD($http, "../Service2/user_list?path=", "../Service2/save_user?path=", "../Service2/user_delete?path=", "id");
    _grd.edit2 = function (r) {
        _grd.edit(r);
        $("#divEntry").modal("show");
    }
    _grd.addNew = function () {
        _grd.row = { id: 0 };
        $("#divEntry").modal("show");
    }
    _grd.addAfterSave(function () {
        //$("#divEntry").modal("hide");
        alert("Operation Done successfully");
    });

    $scope.grd = _grd;



    //$scope.save = function () {
    //    //debugger;

    //    jnPost = {  }
    //    jnPost['name'] = $scope.savuname;
    //    jnPost["userid"] = $scope.savuserid;
    //    jnPost["pwd"] = $scope.savpwd;
    //    jnPost["email"] = $scope.savemail;
    //    jnPost['mob'] = $scope.savmob;
    //    jnPost['remark'] = $scope.savremark;

    //    ng_post($http, "../Service/create_user", jnPost, function (res) {
    //        if (res.result == true) {
    //            alert("Operation done successfully.");
    //        }
    //        else {
    //            alert(res.msg);
    //        }


    //    });

    //}
    _grd.load();
}

myApp.controller("user", user);