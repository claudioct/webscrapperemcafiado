(function () {

    var app = angular.module("app", ["ngMaterial", "ngTouch","ngMdIcons"]);
    app.config(["$mdThemingProvider", function ($mdThemingProvider) {
        $mdThemingProvider.theme("dark-grey").backgroundPalette("grey").dark();
        $mdThemingProvider.theme("dark-orange").backgroundPalette("orange").dark();
        $mdThemingProvider.theme("dark-purple").backgroundPalette("deep-purple").dark();
        $mdThemingProvider.theme("dark-blue").backgroundPalette("blue").dark();
    }]);
})();
(function() {

    var app = angular.module("app");
    var bookService = [
        "$http", "$log", function($http, $log) {

            var getBooks = function(search) {
                return $http.get("/books?d=" + search)
                    // success callback
                    .then(function(response) {
                            $log.debug(response);
                            return response.data;
                        },
                        // error callback
                        function(data, status, headers, config) {
                            $log.debug(data);
                            $log.debug(status);
                            $log.debug(headers);
                            $log.debug(config);
                        });
            };

            return {
                getBooks: getBooks
            };
        }
    ];
    // the service that retrieves some movie title from an url
    app.factory("bookService", bookService);
})();

(function () {

    var app = angular.module("app");

    var mainController = ["$scope", "$timeout", "$q", "$log", "bookService", "$http", function ($scope, $timeout, $q, $log, bookService, $http) {

        var self = this;
        $scope.books = [];
        $scope.imagePath = "img/washedout.png";
        self.clear = function () {
            self.searchText = '';
        }

        $scope.apagar = function (id) {
            $scope.books = $scope.books.filter(function (obj) {
                return obj.id !== id;
            });
        };

        self.simulateQuery = false;
        self.isDisabled = false;

        this.querySearch = function querySearch(query) {
            var deferred = $q.defer();
            $http.get("/books", { params: { d: query } }).then(function(response) {
                $log.info(response.data);
                deferred.resolve(response.data);
                },
                function(msg, code) {
                    deferred.reject(msg);
                    $log.info(msg, code);
                });

            return deferred.promise;
        }

        self.selectedItemChange = function selectedItemChange(item) {
            $log.debug(item);
            $http.get("/books", { params: { nome: item.nome, autor: item.autor } }).then(function (response) {
                    $log.info(response);
                    self.clear();
                    $scope.books.push(response.data);
                },
                function (msg, code) {
                    $log.info(msg, code);
                });
        }
         

    }];


    /**
     * Create filter function for a query string
     */

    app.controller("mainController", mainController);

})();