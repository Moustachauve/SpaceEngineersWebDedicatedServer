define(['angular/application'], function () {

    angular
        .module('ServerExtenderApp')
        .factory('consoleHubService', ['$rootScope', 'Hub', function ($rootScope, Hub) {

            var service = this;

            var hub = new Hub('consoleHub', {
                rootPath: 'http://localhost:9000/signalr',

                listeners: {
                    'consoleReplace': function (consoleLog) {
                        $rootScope.$emit('consoleHub:consoleReplace', consoleLog);
                    },
                    'consoleWrite': function (consoleLog) {
                        $rootScope.$emit('consoleHub:consoleWrite', consoleLog);
                    }
                },

                methods: ['getConsoleText', 'executeCommand'],

                errorHandler: function (error) {
                    console.error(error);
                },

                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            break;
                        case $.signalR.connectionState.connected:
                            hub.getConsoleText();
                            break;
                        case $.signalR.connectionState.reconnecting:
                            break;
                        case $.signalR.connectionState.disconnected:
                            break;
                    }
                },

                useSharedConnection: false
            });

            service.getConsoleText = function () {
                hub.getConsoleText();
            };

            service.executeCommand = function (command) {
                hub.executeCommand(command);
            };

            service.isConnected = function () {
                return hub.connection.state === $.signalR.connectionState.connected;
            };

            service.getConnectionId = function () {
                return hub.connection.id;
            };

            return service;
        }]);
});