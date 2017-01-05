angular
    .module('ServerExtender')
    .factory('serverHubService', ['$rootScope', 'Hub', function ($rootScope, Hub) {

        var service = this;
        
        var hub = new Hub('serverHub', {
            rootPath: 'http://localhost:9000/signalr',

            listeners: {
                'updateStatus': function (status) {
                    $rootScope.$emit('serverHub:updateStatus', status);
                }
            },

            methods: ['start', 'stop', 'updateStatus'],

            errorHandler: function (error) {
                console.error(error);
            },

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        console.log('---connecting---');

                        break;
                    case $.signalR.connectionState.connected:
                        console.log('---connected---');

                        hub.updateStatus();
                        break;
                    case $.signalR.connectionState.reconnecting:
                        console.log('---reconnecting---');

                        break;
                    case $.signalR.connectionState.disconnected:
                        console.log('---disconnected---');
                        break;
                }
            },

            useSharedConnection: false
        });

        service.updateStatus = function () {
            hub.updateStatus();
        };

        service.startServer = function () {
            hub.start();
        };

        service.stopServer = function () {
            hub.stop();
        };
        
        return service;
    }]);