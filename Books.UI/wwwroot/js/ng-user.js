(function () {

    var app = angular.module("app", ["ngMaterial", "ngMdIcons"])
        .config(function ($mdThemingProvider) {
            $mdThemingProvider.theme('dark-grey').backgroundPalette('grey').dark();
            $mdThemingProvider.theme('dark-orange').backgroundPalette('orange').dark();
            $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();
            $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();
        })
        .filter('reverse', function () {
            return function (items) {
                return items.slice().reverse();
            };
        });
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
        $scope.imagePath = 'img/washedout.png';

        self.simulateQuery = false;
        self.isDisabled = false;

        this.querySearch = function querySearch(query) {
            var deferred = $q.defer();
            $http.get("/books", { params: { d: query } }).then(function(response) {
                $log.debug(response.data);
                deferred.resolve(response.data);
                },
                function(msg, code) {
                    deferred.reject(msg);
                    $log.error(msg, code);
                });

            return deferred.promise;
        }

        self.selectedItemChange = function selectedItemChange(item) {
            $log.debug(item);
            $http.get("/books", { params: { nome: item.nome, autor: item.autor } }).then(function (response) {
                $log.debug(response);
                    $scope.books.push(response.data);
                },
                function (msg, code) {
                    $log.error(msg, code);
                });
        }
         

    }];


    /**
     * Create filter function for a query string
     */

    app.controller("mainController", mainController);

})();