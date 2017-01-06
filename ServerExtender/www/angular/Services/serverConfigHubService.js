angular
    .module('ServerExtender')
    .factory('serverConfigHubService', ['$rootScope', 'Hub', function ($rootScope, Hub) {

        var service = this;
        var isConnected = false;

        var hub = new Hub('serverConfigHub', {
            rootPath: 'http://localhost:9000/signalr',

            listeners: {
                'replaceConfig': function (serverConfig) {
                    $rootScope.$emit('serverConfigHub:replaceConfig', serverConfig);
                }
            },

            methods: ['getConfig', 'reloadConfig'],

            errorHandler: function (error) {
                console.error(error);
            },

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        console.log('---connecting---');
                        isConnected = false;
                        break;
                    case $.signalR.connectionState.connected:
                        isConnected = true;
                        hub.getConfig();
                        break;
                    case $.signalR.connectionState.reconnecting:
                        console.log('---reconnecting---');
                        isConnected = false;
                        break;
                    case $.signalR.connectionState.disconnected:
                        console.log('---disconnected---');
                        isConnected = false;
                        break;
                }
            },

            useSharedConnection: false
        });

        service.getConfig = function () {
            hub.getConfig();
        };

        service.reloadConfig = function () {
            hub.reloadConfig();
        };

        service.isConnected = function () {
            return isConnected;
        };

        return service;
    }]);