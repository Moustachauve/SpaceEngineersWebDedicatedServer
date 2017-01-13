angular
    .module('ServerExtender')
    .controller('serverController', ['$scope', '$rootScope', '$timeout', 'serverHubService', 'consoleHubService', errorController]);

function errorController($scope, $rootScope, $timeout, serverHubService, consoleHubService) {

    var commandHistory = [];
    var commandHistoryIndex = 0;
    var commandPreviousCharCount = 0;

    $scope.serverStatus = '';
    $scope.consoleLog = '';
    $scope.command = '';
    $scope.connectionId = '';

    $rootScope.$on('serverHub:updateStatus', function (event, status) {
        $scope.serverStatus = status;
        $scope.$apply();
    });

    $rootScope.$on('consoleHub:consoleReplace', function (event, consoleLog) {
        $scope.consoleLog = consoleLog;
        $scope.connectionId = consoleHubService.getConnectionId();
        $scope.$apply();
        scrollDownConsole();
    });

    $rootScope.$on('consoleHub:consoleWrite', function (event, consoleLog) {
        $scope.consoleLog += consoleLog;
        $scope.$apply();
        scrollDownConsole();
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
        if (!$scope.command.trim()) {
            $scope.consoleLog += ">\n";
            $scope.command = '';
            $timeout(scrollDownConsole, 0, false);
            
            return;
        }
        consoleHubService.executeCommand($scope.command);
        commandHistory.unshift($scope.command)
        commandHistoryIndex = 0;
        $scope.command = '';
        $scope.focusCommand();
    };

    $scope.focusCommand = function () {
        $('#consoleCommand').focus();
    };

    $scope.keyDownConsole = function ($event) {
        if ($event.keyCode == 38) { // up
            if (commandHistoryIndex + 1 <= commandHistory.length) {
                $scope.command = commandHistory[commandHistoryIndex];
                commandHistoryIndex++;
            }
        }
        else if ($event.keyCode == 40) {// down
            if (commandHistoryIndex > 0) {
                $scope.command = commandHistory[commandHistoryIndex];
                commandHistoryIndex--;
            }
            else {
                $scope.command = '';
            }
        }
    }

    var scrollDownConsole = function () {
        $('#console').scrollTop($('#console')[0].scrollHeight);
    }
}