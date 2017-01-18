define(['angular/application'], function () {

    angular
        .module('ServerExtenderApp')
        .config(function ($routeProvider, $locationProvider) {

            $routeProvider
                // === Base routes ===
                .when('/', {
                    title: "Server Status",
                    templateUrl: 'angular/controllers/server/status.html',
                    controller: 'serverController'
                })
                .when('/server/config', {
                    title: "Server Configuration",
                    templateUrl: 'angular/controllers/serverConfig/serverConfigEdit.html',
                    controller: 'serverConfigController'
                })

                // === Errors routes ===

                .otherwise({
                    title: "Page not found",
                    templateUrl: 'angular/controllers/error/404.html',
                    controller: 'errorController'
                });

            // === Plugins ===
            for (var i = 0; i < angularPluginRoutes.length; i++) {
                console.log(angularPluginRoutes[i]);
                var currentRoute = angularPluginRoutes[i];
                $routeProvider.when(currentRoute.Route, {
                    title: "Temp plugin title",
                    templateUrl: currentRoute.ViewPath,
                    controller: currentRoute.ControllerName
                })
            }



            $locationProvider.html5Mode(true);
        });
});