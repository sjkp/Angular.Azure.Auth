module Angular.Azure.Auth{
    export interface ITenantDetail {
        TenantId: string;
        Tenant: string;
    }

    export interface ISubscription {
        DisplayName: string;
        SubscriptionId: string;
    }

    export interface IApplicationResponse {
        value: IApplication[];
    }

    export interface IApplication {
        displayName: string;
        homepage: string;
        identifierUris: string[];
    }

    export interface IHomeControllerScope extends ng.IScope {
        tenants: ITenantDetail[];
        subscriptions: ISubscription[];
        applications: IApplication[]
        subChange: Function;
        tenantChange: Function;

        subscriptionId: string;
        tenant: ITenantDetail;
        
    }

    export class HomeController {
        public static $inject = ['$scope', '$http', 'adalAuthenticationService'];

        constructor(private $scope: IHomeControllerScope, private $http: ng.IHttpService, private authService: any) {
            authService.acquireToken('https://graph.windows.net/').then((token) => {
                console.log(token);
                authService.acquireToken('https://management.core.windows.net/').then((token2) => {
                    $http.defaults.headers.common['graphToken'] = token;
                    $http.defaults.headers.common['managementToken'] = token2;

                    $http.post('/api/Values/', {
                        graphToken: token,
                        managementToken: token2
                    }).then((success) => {
                        console.log(success);
                        });

                    $http.get('/api/tenants/').then((success) => {
                        this.$scope.tenants = <ITenantDetail[]>success.data;
                    });

                    $http.get('/api/subscriptions').then((res) => {
                        this.$scope.subscriptions = <ISubscription[]>res.data;
                    });
                });
            });

            $scope.subChange = () => {
                console.log($scope.subscriptionId);
                $http.get('/api/' + $scope.subscriptionId + '/applications').then((res) => {
                    console.log(res.data);
                });

            }

            $scope.tenantChange = () => {
                console.log($scope.tenant);
                $http.get('/api/' + $scope.tenant.Tenant + '/applications').then((res) => {
                    var value = (<IApplicationResponse>res.data).value;
                    console.log(value);
                    $scope.applications = value;
                });

            }
        }
    }
}