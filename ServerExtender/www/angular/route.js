angular
    .module('ServerExtender')
    .config(function ($routeProvider, $locationProvider) {

        $routeProvider

         /*.when('/', {
             title: "Home",
             templateUrl: '/controllers/leaderboard/mainPage.html',
             controller: 'leaderboardController',
         })*/

        // = Errors =

        .otherwise({
            title: "Page not found",
            templateUrl: 'controllers/error/404.html',
            controller: 'Error404Controller'
        });

        $locationProvider.html5Mode(true);
    });
