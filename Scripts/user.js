﻿/// <reference path="ngInit.js" />

function user($scope, $http)
{
    var _grd = new ngCRUD($http, "../Service2/user_list?path=", "../Service2/user_save", "../Service2/user_delete", "id");
    _grd.edit2 = function (r) {
        _grd.edit(r);
        $("#divEntry").modal("show");
    }
    $scope.grd = _grd;

    $scope.save = function () {
        //debugger;

        jnPost = {  }
        jnPost['name'] = $scope.savuname;
        jnPost["userid"] = $scope.savuserid;
        jnPost["pwd"] = $scope.savpwd;
        jnPost["email"] = $scope.savemail;
        jnPost['mob'] = $scope.savmob;
        jnPost['remark'] = $scope.savremark;

        ng_post($http, "../Service/create_user", jnPost, function (res) {
            if (res.result == true) {
                alert("Operation done successfully.");
            }
            else {
                alert(res.msg);
            }


        });

    }
    _grd.load();
}

myApp.controller("user", user);