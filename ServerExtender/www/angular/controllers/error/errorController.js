define(['angular/application'], function () {

    angular
    .module('ServerExtenderApp')
    .controller('errorController', ['$scope', '$rootScope', errorController]);

    function errorController($scope, $rootScope) {
    }
});