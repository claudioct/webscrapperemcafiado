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