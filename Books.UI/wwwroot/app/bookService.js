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
