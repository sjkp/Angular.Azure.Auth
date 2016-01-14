module angular.ui {
    export interface IState {
        requireADLogin?: boolean;
    }
}

module Angular.Azure.Auth {
    export class Config {
        public static $inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider', 'adalAuthenticationServiceProvider'];

        constructor(private $stateProvider: ng.ui.IStateProvider, private $urlRouterProvider: ng.ui.IUrlRouterProvider, private $httpProvider: ng.IHttpProvider, private adalAuthenticationServiceProvider: any) {
            this.initAdal();
            this.initStates();
        }

        private initAdal = () => {
            this.adalAuthenticationServiceProvider.init(
                {
                    instance: 'https://login.microsoftonline.com/',
                    tenant: 'common',
                    clientId: 'd1b853e2-6e8c-4e9e-869d-60ce913a280c',
                    extraQueryParameter: 'nux=1',
                    endpoints: {
                        //You don't have to use this, as the app used for authentication is protecting the 
                        //same resource as we are accessing, but if you wanted to access, e.g. other resources, like Office 365 API or Exchange API you would specify it here
                        //'<http-address-of-the-resource>': '<azure-app-id/resource id>' 
                    }
                    //cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
                },
                this.$httpProvider
            );
        }

        private initStates = () => {
            this.$urlRouterProvider.when('', '/home');

            this.$stateProvider.state('home', {
                url: '/home',
                templateUrl: '/app/views/home.html',
                controller: 'homeController',
                requireADLogin: true           
            });

            //this.$urlRouterProvider.otherwise('/movies');
        }
    }
}