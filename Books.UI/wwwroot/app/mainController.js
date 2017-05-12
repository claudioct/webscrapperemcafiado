(function () {

    var app = angular.module("app");

    var mainController = ["$scope", "$timeout", "$q", "$log", "bookService", "$http", "$window", function ($scope, $timeout, $q, $log, bookService, $http, $window) {

        var self = this;
        $scope.books = [];
        $scope.imagePath = "img/washedout.png";
        self.clear = function () {
            self.searchText = '';
        }

        $scope.apagar = function (book) {
            $scope.books.splice($scope.books.indexOf(book), 1);
        };

        $scope.splice = function (item) {
            var index = $scope.books.indexOf(item);
            $scope.books.splice(index, 1);
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
            if (item != null) {
                $log.info(item);
                $http.get("/books", { params: { nome: item.nome, autor: item.autor } }).then(function(response) {
                        $log.info(response);
                        self.clear();
                        $scope.books.push(response.data);
                    },
                    function(msg, code) {
                        $log.info(msg, code);
                    });
            }
        }
    }];


    /**
     * Create filter function for a query string
     */

    app.controller("mainController", mainController);

})();