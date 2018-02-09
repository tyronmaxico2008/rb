
var myApp = angular.module("myApp", []);

function getFormData(fields) {
    var fd = new FormData();

    for (var f in fields) {
        if (fields[f] != null && fields[f] != undefined)
            fd.append(f, fields[f]);
    }

    return fd;
}


function ng_post($http, slink, jn, callBack) {

    $http.post(slink, getFormData(jn), {
        transformRequest: angular.identity,
        headers: { 'Content-Type': undefined }
    }).then(function (res) {

        callBack(res.data);
    });
}


myApp.directive("pdfView2", function () {
    return {
        restrict: "E"
        , replace: true
        , scope: { barCode: "=" }
        , link: function (scope, element) {
            scope.$watch("barCode", function (newValue) {
                element.html("");
                if (newValue) {
                    var sTemplate = '<embed width="100%" height="800px"  runat="server"  name="embedPDF" id="embedPDF" src="/service/fetchpdf?fileBarCode=' + newValue + '&scrollbar=0" type="application/pdf" internalinstanceid="8" />'
                    var oHtml = angular.element(sTemplate);
                    element.append(oHtml);
                }
            });
        }
    };
});

myApp.directive("pdfView", function () {
    return {
        restrict: "E"
        , replace: true
        , scope: { barCode: "=" }
        , link: function (scope, element) {
            scope.$watch("barCode", function (newValue) {
                element.html("");
                if (newValue) {
                    var sTemplate = '<iframe width="100%" height="800px"  runat="server" seamless="seamless" scrolling="no" frameborder="0" allowtransparency="true"  id="embedPDF" src="/service/fetchpdf?fileBarCode=' + newValue + '#page=3"></iframe>'
                    var oHtml = angular.element(sTemplate);
                    element.append(oHtml);
                }
            });
        }
    };
});

