//When we're ready we can bootstrap our app and pass in
//data from our server to the config step for our app

var count_getcurrentfile = 0;

var bootstrap = function (app, dataFromServer) {
    app
        .provider('hubcontext', [function HubcontextProvider() {
            this.$get = function () { return $.connection; };
        }])
        .config(["$provide", "$httpProvider", function ($provide, $httpProvider) {
            $provide.decorator('$exceptionHandler', [extendExceptionHandler]);
            $httpProvider.interceptors.push('noCacheInterceptor');
            //$provide.decorator('inputDirective', function ($delegate) {
            //    var directive = $delegate[0],
            //        link = directive.link;
            //    link.post = function (scope, element, attrs) {
            //        attrs.$set('ngTrim', 'false');
            //    };
            //    return $delegate;
            //});
        }])
        .run(["hubcontext", "$rootScope", function (hubcontext, $rootScope) {
            if (hubcontext.hubMain) {
                hubcontext.hub.start().done(successCallbackHubStart).fail(errorCallbackHubStart);
                $.connection.hub.disconnected(disconnectedCallback);
                function successCallbackHubStart() {
                    $rootScope.$evalAsync(function () {
                        $rootScope.IsAppLoaded = true;
                        $rootScope.IsHubWorking = true;
                    });
                }
                function errorCallbackHubStart() {
                    // code for executing if hub fails
                }
                function disconnectedCallback() {
                    $.connection.hub.start().done(successCallbackHubStart).fail(errorCallbackHubStart);
                }
            }
            else {
                $rootScope.IsHubWorking = false;
                $rootScope.IsAppLoaded = true;
            }
        }]);

    var $app = $("[s3-app]");
    angular.bootstrap($app, ["myApp"]);
};

var app = angular.module('myApp', ['ngDialog', 'ngSanitize', 'mgo-angular-wizard', 'ngCookies', 'smart-table']);
var hubMain = null;
var s3Config = null;
URL_HTTP = URL_HUB = "http://localhost:8080",
    URL_HTTPS = "https://localhost:8083";

if (location.protocol === 'https:') {
    URL_HUB = URL_HTTPS;
}

$.ajax({
    url: 'api/Login/GetHostMachinePort',
    type: 'GET',
    async: false,
    dataType: 'json',
    data: {},
    success: function (data) {
        if (data) {
            if (data["HostMachineName"] && data["Port"]) {
                if (location.protocol === 'https:') {
                    URL_HUB = "https://" + data["HostMachineName"] + ":" + data["Port"];
                }
                else {
                    URL_HUB = "http://" + data["HostMachineName"] + ":" + data["Port"];
                }
            }
        }
    },
    error: function (response) {
        var objError = JSON.parse(response.responseText);
        if (objError && objError.Message) {
            var msg = objError.Message;
            if (objError.ExceptionMessage) {
                msg = msg + "\n" + objError.ExceptionMessage;
            }
            console.log(msg);
        }
        else {
            console.log("An error occured while creating a new GUID.");
        }
    }
});


$.ajax({
    async: false,
    url: URL_HUB + "/signalr/hubs",
    type: 'GET',
    success: function () {
        $.connection.hub.url = URL_HUB + "/signalr";
        hubMain = $.connection.hubMain;
        initializeHubreceivemethods();
        //to read the package file 
        $.getJSON("package.json?" + Date.now(), function (data) {
            s3Config = data;
        });

        angular.element(document.body).ready(function () {
            bootstrap(app);
        });
    },
    error: function (a, b, c) {
        window.location.href = "error_page.html?ERROR_CODE=101";
        //alert("HUB could not be reached");
    }
});

app.factory('noCacheInterceptor', [function () {
    return {
        request: function (config) {
            // to break html cache - url is appended with client version to 
            if (s3Config && config.method == 'GET' && config.url.indexOf('.html') > 0 && config.url != 'wizard.html' && config.url != 'step.html') {
                var separator = config.url.indexOf('?') === -1 ? '?' : '&';
                config.url = config.url + separator + 'rev=' + s3Config.versionClient;
            }
            return config;
        }
    };
}]);

function extendExceptionHandler() {
    return function (exception, cause) {
        var rootScope = angular.element(document.body).injector().get("$rootScope");
        if (rootScope) {
            rootScope.IsLoading = false;
            rootScope.IsProjectLoaded = false;
        }
        $(".page-header-fixed").css("pointer-events", "auto");
        //toastr.error("Exception:" + exception+"\nCause:"+cause);
        console.log(String.format("Exception:{0}\nCause:{1}\nStacktrace:{2}", exception, cause, exception.stack));
        alert(String.format("Exception:{0}\nCause:{1}\nStacktrace:{2}", exception, cause, exception.stack));
    };
}

app.filter('trustAsResourceUrl', ['$sce', function ($sce) {
    return function (val) {
        return $sce.trustAsResourceUrl(val);
    };
}]);