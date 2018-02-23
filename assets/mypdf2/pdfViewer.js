

myApp.directive("pdfViewer", function () {
    debugger;
    return {
        restrict: "E"
        , scope: { barCode: "=barCode", currentPage: "=currentPage", totalPages: "=totalPages" }
        , replace: false
        , templateUrl: '../assets/mypdf2/pdfViewer2.html' //appConfig.appResourceLink + "/dms/controls/pdfViewer/pdfviewer.html"
        , controller: function ($scope) {

            //variable declaration 
            $scope.currentPage = 1;
            $scope.totalPages = 0
            $scope.error = false;

            //End


            var _pdf = null;


            var _loadPDF = function () {
                var sLink = "../Service/fetchPdf?fileBarCode=" + $scope.barCode;
                PDFJS.getDocument(sLink).then(function (pdf) {
                    // you can now use *pdf* here
                    _pdf = pdf;
                    $scope.totalPages = _pdf.numPages;
                    _viewPage();
                    $scope.$apply();
                });
            }

            $scope.$watch('fileId', function () {
                var iFileID = ($scope.fileId || 0);

                if (iFileID > 0) {
                    $scope.currentPage = 1;
                    _loadPDF();
                }
            });

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

