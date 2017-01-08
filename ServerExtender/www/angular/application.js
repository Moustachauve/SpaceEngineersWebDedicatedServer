var app = angular.module('ServerExtender', ['ngRoute', 'SignalR', 'ngAria', 'ngAnimate', 'ngMaterial'])
.controller('applicationController', ['$scope', '$rootScope', '$route', '$window', '$mdSidenav', function ($scope, $rootScope, $route, $window, $mdSidenav) {


    $scope.openSideMenu = function () {
        $mdSidenav('side').toggle();
    };
    $scope.closeSideMenu = function () {
        $mdSidenav('side').toggle();
    };
}]);