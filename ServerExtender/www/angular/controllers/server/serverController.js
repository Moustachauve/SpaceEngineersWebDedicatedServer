angular
    .module('ServerExtender')
    .controller('serverController', ['$scope', '$rootScope', 'serverHubService', 'consoleHubService', errorController]);

function errorController($scope, $rootScope, serverHubService, consoleHubService) {

    $scope.serverStatus = '';
    $scope.consoleLog = '';

    $rootScope.$on('serverHub:updateStatus', function (event, status) {
        $scope.serverStatus = status;
        $scope.$apply();
    });

    $rootScope.$on('consoleHub:consoleReplace', function (event, consoleLog) {
        $scope.consoleLog = consoleLog;
        $scope.$apply();
        $('#console').scrollTop($('#console')[0].scrollHeight);
    });

    $rootScope.$on('consoleHub:consoleWrite', function (event, consoleLog) {
        $scope.consoleLog += consoleLog;
        $scope.$apply();
        $('#console').scrollTop($('#console')[0].scrollHeight);
    });

    if (serverHubService.isConnected()) {
        serverHubService.updateStatus();
    }
    if (consoleHubService.isConnected()) {
        consoleHubService.getConsoleText();
    }


    $scope.refreshStatus = function () {
        serverHubService.updateStatus();
    };

    $scope.startServer = function () {
        serverHubService.startServer();
    };

    $scope.stopServer = function () {
        serverHubService.stopServer();
    };

    $scope.executeCommand = function () {
        console.log("executeCommand: " + $scope.command);
        consoleHubService.executeCommand($scope.command);
        $scope.command = '';
        $scope.focusCommand();
    };

    $scope.focusCommand = function () {
        $('#consoleCommand').focus();
    };
}