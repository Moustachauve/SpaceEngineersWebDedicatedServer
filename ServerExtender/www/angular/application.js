var app = angular.module('ServerExtender', ['ngRoute', 'SignalR', 'ngAria', 'ngAnimate', 'ngMaterial'])

// Set the page title according to the current route
.run(['$rootScope', '$route', function ($rootScope, $route) {
    $rootScope.$on('$routeChangeSuccess', function () {
        $rootScope.pageTitle = $route.current.title + ' - Space Engineers Dedicated Server';
    });
}])

.controller('applicationController', ['$scope', '$rootScope', '$route', '$window', '$mdSidenav', function ($scope, $rootScope, $route, $window, $mdSidenav) {


    $scope.openSideMenu = function () {
        $mdSidenav('side').toggle();
    };
    $scope.closeSideMenu = function () {
        if (!$mdSidenav('side').isLockedOpen()) {
            $mdSidenav('side').toggle();
        }
    };
}])

.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('dark-grey').backgroundPalette('grey').dark();
});