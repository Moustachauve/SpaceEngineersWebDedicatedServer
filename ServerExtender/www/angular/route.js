angular
    .module('ServerExtender')
    .config(function ($routeProvider, $locationProvider) {

        $routeProvider

         .when('/', {
             title: "Server Status",
             templateUrl: 'angular/controllers/server/status.html',
             controller: 'serverController'
         })
         .when('/entities', {
             title: "Entities",
             templateUrl: 'angular/controllers/entities/entitiesList.html',
             controller: 'entitiesController'
         })
         .when('/server/config', {
             title: "Server Configuration",
             templateUrl: 'angular/controllers/serverConfig/serverConfigEdit.html',
             controller: 'serverConfigController'
         })

        // = Errors =

        .otherwise({
            title: "Page not found",
            templateUrl: 'angular/controllers/error/404.html',
            controller: 'errorController'
        });

        $locationProvider.html5Mode(true);
    });
