var app = angular.module('ServerExtender', ['ngRoute', 'SignalR'])
.controller('applicationController', ['$scope', '$rootScope', '$route', '$window', function ($scope, $rootScope, $route, $window) {

    /*$rootScope.signalRReady = false;

    $.connection.hub.url = "http://localhost:9000/signalr";
    $.connection.hub.start().done(function () {
        console.log("SignalR Ready!");
        $rootScope.signalRReady = true;
        $rootScope.$apply();
    });*/
}]);