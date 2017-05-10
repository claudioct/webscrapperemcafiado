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