angular
    .module('ServerExtender')
    .controller('serverConfigController', ['$scope', '$rootScope', 'serverConfigHubService', 'serverHubService', serverConfigController]);

function serverConfigController($scope, $rootScope, serverConfigHubService, serverHubService) {

    $scope.formDisabled = true;
    $scope.serverConfig = '';
    $scope.gameModes = [
        { value: 0, name: 'Creative' },
        { value: 1, name: 'Survival' },
    ];
    $scope.onlineModes = [
        { value: 0, name: 'Offline' },
        { value: 3, name: 'Private' },
        { value: 2, name: 'Friends' },
        { value: 1, name: 'Public' },
    ];
    $scope.environmentHostilityValues = [
        { value: 0, name: 'Safe' },
        { value: 1, name: 'Normal' },
        { value: 2, name: 'Cataclysm' },
        { value: 3, name: 'Cataclysm Unreal' },
    ];

    $rootScope.$on('serverConfigHub:replaceConfig', function (event, serverConfig) {
        $scope.serverConfig = serverConfig;
        console.log(serverConfig);
        $scope.$apply();
    });
    $rootScope.$on('serverHub:updateStatus', function (event, status) {
        $scope.formDisabled = status != 'Stopped';
        $scope.$apply();
    });


    if (serverConfigHubService.isConnected()) {
        serverConfigHubService.reloadConfig();
    }
    if (serverHubService.isConnected()) {
        serverHubService.updateStatus();
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