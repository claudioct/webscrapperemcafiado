(function () {

    var app = angular.module("app", ["ngMaterial", "ngAnimate", "ngTouch", "ngMdIcons"]);
    app.config(["$mdThemingProvider", function ($mdThemingProvider) {
        $mdThemingProvider.theme("dark-grey").backgroundPalette("grey").dark();
        $mdThemingProvider.theme("dark-orange").backgroundPalette("orange").dark();
        $mdThemingProvider.theme("dark-purple").backgroundPalette("deep-purple").dark();
        $mdThemingProvider.theme("dark-blue").backgroundPalette("blue").dark();
    }]);
})();