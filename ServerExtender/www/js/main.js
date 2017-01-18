require.config({
    baseUrl: '/',
    urlArgs: 'v=1.0'
});

loadRequiredFiles();

var angularPluginRoutes = [];

// Hardcode your site url if you are using the WebInterface solution
//var siteUrl = 'http://localhost:9000';
var siteUrl = '';

function loadRequiredFiles() {

    var requiredFiles = [
        'angular/application.js',
        'angular/route.js',
        'angular/services/consoleHubService.js',
        'angular/services/serverHubService.js',
        'angular/services/serverConfigHubService.js',
        'angular/controllers/error/errorController.js',
        'angular/controllers/server/serverController.js',
        'angular/controllers/serverConfig/serverConfigController.js'
    ]

    //TODO: Loading page while angular stuff kicks in
    //TODO: Error handling if one of those request fail
    $.ajax({
        url: siteUrl + '/api/plugins/GetRequiredWebResources',
    }).done(function (data) {
        for (var i = 0; i < data.length; i++) {
            requiredFiles.push(data[i].Path);
        }

        var requiredJavascriptFiles = [];

        for (var i = 0; i < requiredFiles.length; i++) {
            var pathSlugs = requiredFiles[i].split('.');
            var extension = pathSlugs[pathSlugs.length - 1].toLowerCase();

            switch (extension) {
                case 'js':
                    requiredJavascriptFiles.push(requiredFiles[i]);
                    break;
                case 'css':
                    loadCss(requiredFiles[i]);
                    break;
                default:
                    console.warn('Unknown extension for path ' + requiredFiles[i]);
            }
        }

        $.ajax({
            url: siteUrl + '/api/plugins/GetAngularRoutes',
        }).done(function (data) {
            angularPluginRoutes = data;

            require(requiredJavascriptFiles, function () {
                angular.bootstrap(document, ['ServerExtenderApp']);
            });
        });
    });
}

function loadCss(url) {
    var link = document.createElement("link");
    link.type = "text/css";
    link.rel = "stylesheet";
    link.href = url;
    document.getElementsByTagName("head")[0].appendChild(link);
}