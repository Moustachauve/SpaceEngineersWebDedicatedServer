angular
    .module('ServerExtender')
    .controller('serverConfigController', ['$scope', '$rootScope', 'serverConfigHubService', 'serverHubService', '$mdToast', serverConfigController]);

function serverConfigController($scope, $rootScope, serverConfigHubService, serverHubService, $mdToast) {

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
        $scope.$apply();
    });
    $rootScope.$on('serverConfigHub:setValue', function (event, key, value) {
        setConfigFromKey(key, value);
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

    $scope.saveValue = function (key) {
        var value = getConfigFromKey(key);
        var currentItem = $scope.serverConfig;
        // TODO: Add a promise or something to confirm that the server did save the value and show a toast to the user
        //       to congratulate him :)
        serverConfigHubService.setValue(key, value);
        $scope.showToast('Field ' + key + ' was saved!');
    }

    var getConfigFromKey = function (key) {
        var keyNodes = key.split('.');
        var currentItem = $scope;
        for (var i = 0; i < keyNodes.length; i++) {
            currentItem = currentItem[keyNodes[i]];
        }

        return currentItem;
    }

    var setConfigFromKey = function (key, value) {
        var keyNodes = key.split('.');
        var currentItem = $scope;
        for (var i = 0; i < keyNodes.length - 1; i++) {
            currentItem = currentItem[keyNodes[i]];
        }

        currentItem[keyNodes[keyNodes.length - 1]] = value;
    }

    $scope.showToast = function (content) {
        $mdToast.show(
          $mdToast.simple()
            .textContent(content)
            .position('top right')
            .hideDelay(2000)
            .parent('#page_container')
        );
    };
}