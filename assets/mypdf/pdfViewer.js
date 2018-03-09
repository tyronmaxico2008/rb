
myApp.directive("pdfViewer", function () {
    debugger;
    return {
        restrict: "E"
        , scope: { barCode: "=barCode", currentPage: "=currentPage", totalPages: "=totalPages" }
        , replace: false
        , templateUrl: '../assets/mypdf/pdfViewer.html?v=1' //appConfig.appResourceLink + "/dms/controls/pdfViewer/pdfviewer.html"
        , controller: function ($scope) {

            //variable declaration 
            $scope.currentPage = 1;
            $scope.totalPages = 0
            $scope.error = false;

           
            var _pdf = null;

            var _loadPDF = function () {
                if (!$scope.barCode) return;
                var sLink = "../Service/fetchPdf?fileBarCode=" + $scope.barCode;
                PDFJS.getDocument(sLink).then(function (pdf) {
                    // you can now use *pdf* here
                    _pdf = pdf;
                    $scope.totalPages = _pdf.numPages;
                    _viewPage();
                    $scope.$apply();
                });
            }

            $scope.$watch('barCode', function () {
                var barcode = ($scope.barCode || 0);
                if ($.trim(barcode) != "") {
                    $scope.currentPage = 1;
                    _loadPDF();
                }
            });

            $scope.$watch('currentPage', function () {
                debugger;
                if ($scope.currentPage > 0) {
                    _viewPage();
                }
            });

            //$scope.$watch('currentPage', function () {
            //    //alert("Hi");
            //});



            var _viewPage = function () {
                _pdf.getPage($scope.currentPage).then(function (page) {
                    // you can now use *page* here
                    var scale = 1.5;
                    var viewport = page.getViewport(scale);

                    var canvas = document.getElementById('the-canvas');
                    var context = canvas.getContext('2d');
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;


                    var renderContext = {
                        canvasContext: context,
                        viewport: viewport
                    };
                    page.render(renderContext);

                });
            }



            $scope.viewPage = function () {
                _viewPage();
            }


            $scope.reset = function () {
                $scope.currentPage = 1;
                _viewPage();
            }

            $scope.next = function () {
                if ($scope.currentPage == $scope.totalPages) {
                    alert("You are on the last page");
                    return;
                }
                $scope.currentPage += 1;
                _viewPage();
            }

            $scope.previous = function () {
                if ($scope.currentPage == 1) {
                    alert("You are on the first page !")
                    return;
                }

                $scope.currentPage -= 1;
                _viewPage();
            }

           
        }
        , link: function (element) {

        }
    }
});

