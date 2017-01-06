angular
    .module('ServerExtender')
    .controller('serverConfigController', ['$scope', '$rootScope', 'serverConfigHubService', serverConfigController]);

function serverConfigController($scope, $rootScope, serverConfigHubService) {

    $scope.serverConfig = '';

    $rootScope.$on('serverConfigHub:replaceConfig', function (event, serverConfig) {
        $scope.serverConfig = serverConfig;
        console.log(serverConfig);
        $scope.$apply();
    });

    if (serverConfigHubService.isConnected()) {
        serverConfigHubService.reloadConfig();
    }

    $scope.getType = function (variable) {
        var type = typeof variable;
        if (type === 'object') {
            if (Array.isArray(variable))
                return 'array';
        }

        return type;
    };
}