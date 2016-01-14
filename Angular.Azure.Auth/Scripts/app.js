var Angular;
(function (Angular) {
    var Azure;
    (function (Azure) {
        var Auth;
        (function (Auth) {
            var Config = (function () {
                function Config($stateProvider, $urlRouterProvider, $httpProvider, adalAuthenticationServiceProvider) {
                    var _this = this;
                    this.$stateProvider = $stateProvider;
                    this.$urlRouterProvider = $urlRouterProvider;
                    this.$httpProvider = $httpProvider;
                    this.adalAuthenticationServiceProvider = adalAuthenticationServiceProvider;
                    this.initAdal = function () {
                        _this.adalAuthenticationServiceProvider.init({
                            instance: 'https://login.microsoftonline.com/',
                            tenant: 'common',
                            clientId: 'd1b853e2-6e8c-4e9e-869d-60ce913a280c',
                            extraQueryParameter: 'nux=1',
                            endpoints: {}
                        }, _this.$httpProvider);
                    };
                    this.initStates = function () {
                        _this.$urlRouterProvider.when('', '/home');
                        _this.$stateProvider.state('home', {
                            url: '/home',
                            templateUrl: '/app/views/home.html',
                            controller: 'homeController',
                            requireADLogin: true
                        });
                    };
                    this.initAdal();
                    this.initStates();
                }
                Config.$inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider', 'adalAuthenticationServiceProvider'];
                return Config;
            })();
            Auth.Config = Config;
        })(Auth = Azure.Auth || (Azure.Auth = {}));
    })(Azure = Angular.Azure || (Angular.Azure = {}));
})(Angular || (Angular = {}));
var Angular;
(function (Angular) {
    var Azure;
    (function (Azure) {
        var Auth;
        (function (Auth) {
            var HomeController = (function () {
                function HomeController($scope, $http, authService) {
                    var _this = this;
                    this.$scope = $scope;
                    this.$http = $http;
                    this.authService = authService;
                    authService.acquireToken('https://graph.windows.net/').then(function (token) {
                        console.log(token);
                        authService.acquireToken('https://management.core.windows.net/').then(function (token2) {
                            $http.defaults.headers.common['graphToken'] = token;
                            $http.defaults.headers.common['managementToken'] = token2;
                            $http.post('/api/Values/', {
                                graphToken: token,
                                managementToken: token2
                            }).then(function (success) {
                                console.log(success);
                            });
                            $http.get('/api/tenants/').then(function (success) {
                                _this.$scope.tenants = success.data;
                            });
                            $http.get('/api/subscriptions').then(function (res) {
                                _this.$scope.subscriptions = res.data;
                            });
                        });
                    });
                    $scope.subChange = function () {
                        console.log($scope.subscriptionId);
                        $http.get('/api/' + $scope.subscriptionId + '/applications').then(function (res) {
                            console.log(res.data);
                        });
                    };
                    $scope.tenantChange = function () {
                        console.log($scope.tenant);
                        $http.get('/api/' + $scope.tenant.Tenant + '/applications').then(function (res) {
                            var value = res.data.value;
                            console.log(value);
                            $scope.applications = value;
                        });
                    };
                }
                HomeController.$inject = ['$scope', '$http', 'adalAuthenticationService'];
                return HomeController;
            })();
            Auth.HomeController = HomeController;
        })(Auth = Azure.Auth || (Azure.Auth = {}));
    })(Azure = Angular.Azure || (Angular.Azure = {}));
})(Angular || (Angular = {}));
var Angular;
(function (Angular) {
    var Azure;
    (function (Azure) {
        var Auth;
        (function (Auth) {
            var app = angular.module('azure-auth', ['AdalAngular', 'ui.router']);
            app.controller('homeController', Auth.HomeController);
            app.config(Auth.Config);
        })(Auth = Azure.Auth || (Azure.Auth = {}));
    })(Azure = Angular.Azure || (Angular.Azure = {}));
})(Angular || (Angular = {}));
//# sourceMappingURL=app.js.map