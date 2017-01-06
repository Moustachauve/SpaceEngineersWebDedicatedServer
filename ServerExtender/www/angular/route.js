angular
    .module('ServerExtender')
    .config(function ($routeProvider, $locationProvider) {

        $routeProvider

         .when('/', {
             title: "Server Info",
             templateUrl: 'angular/controllers/server/status.html',
             controller: 'serverController',
         })
         .when('/entities', {
             title: "Entities",
             templateUrl: 'angular/controllers/entities/entitiesList.html',
             controller: 'entitiesController',
         })

        // = Errors =

        .otherwise({
            title: "Page not found",
            templateUrl: 'angular/controllers/error/404.html',
            controller: 'errorController'
        });

        $locationProvider.html5Mode(true);
    });
