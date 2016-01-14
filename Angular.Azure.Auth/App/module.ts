module Angular.Azure.Auth{
    var app = angular.module('azure-auth', ['AdalAngular', 'ui.router']);
    app.controller('homeController', HomeController);
    app.config(Config);
}