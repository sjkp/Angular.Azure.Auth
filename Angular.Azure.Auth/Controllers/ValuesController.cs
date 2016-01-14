namespace Angular.Azure.Auth.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Azure.Graph.RBAC.Models;
    using Microsoft.Azure.Management.Authorization;
    using Microsoft.Azure.Management.Authorization.Models;
    using Microsoft.Azure.Management.Resources;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Newtonsoft.Json;

    public class ValuesController : ApiController
    {
        public async Task<IHttpActionResult> GetValues()
        {
            return Ok(new[] { "a", "b", "c" });
        }

        [Route("api/tenants")]
        [ResponseType(typeof(IEnumerable<TenantDetail>))]
        public async Task<IHttpActionResult> GetTenants()
        {
            var ret = new List<Models.TenantDetail>();
            using (var subClient = GetSubscriptionClient(Request))
            using (var client = GetGraphClient(Request))
            {
                var tenants = await subClient.Tenants.ListAsync();
                foreach(var tenant in tenants)
                {
                    var res = string.Empty;
                    
                    var model = new Models.TenantDetail
                    {
                        TenantId = tenant.TenantId,
                        
                    };
                    ret.Add(model);
                    try {
                        res = await client.GetStringAsync(tenant.TenantId + "/tenantDetails?api-version=1.6");
                    }
                     catch(Exception ex)
                    {
                        continue;
                    }
                    var tenantDetails = JsonConvert.DeserializeObject<Models.GraphTenantDetails>(res);
                    model.Tenant = tenantDetails.value.FirstOrDefault()?.verifiedDomains.FirstOrDefault()?.name;
                }
                
            }
            return Ok(ret);
        }

        [Route("api/subscriptions")]
        [ResponseType(typeof(IEnumerable<Microsoft.Azure.Management.Resources.Models.Subscription>))]
        public async Task<IHttpActionResult> GetSubscriptions()
        {
            using (var client = GetSubscriptionClient(Request))
            {
                var res = await client.Subscriptions.ListAsync();
                return Ok(res.ToList());
            }
        }

        [Route("api/{tenant}/applications")]
        public async Task<IHttpActionResult> GetApplications(string tenant)
        {
            using (var client = GetGraphClient(Request))
            {
                var res = await client.GetStringAsync(tenant + "/applications?api-version=1.6");
                return Ok(JsonConvert.DeserializeObject(res));
            }                
        }

        private HttpClient GetGraphClient(HttpRequestMessage request)
        {
            var httpclient = new HttpClient()
            {
                BaseAddress = new Uri("https://graph.windows.net/")
            };
            var token = request.Headers.GetValues("graphToken").First();
            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
            return httpclient;
        }

        private SubscriptionClient GetSubscriptionClient(HttpRequestMessage request)
        {
            var creds = new TokenCredentials(request.Headers.GetValues("managementToken").First());
            var subscriptionClient = new SubscriptionClient(creds);
            subscriptionClient.SubscriptionId = Guid.Empty.ToString(); //must be set or validation error.
            return subscriptionClient;
        }


        [HttpPost]
        public async Task<IHttpActionResult> Send(CreateRequest token)
        {
            var creds = new TokenCredentials(token.managementToken);
            var subscriptionClient = new SubscriptionClient(creds);
            subscriptionClient.SubscriptionId = Guid.Empty.ToString();
            var tenants = await subscriptionClient.Tenants.ListAsync();
            var subscriptions = await subscriptionClient.Subscriptions.ListAsync();

            var client = new Microsoft.Azure.Graph.RBAC.GraphRbacManagementClient(new TokenCredentials(token.graphToken));
            var subscriptionId = subscriptions.First().SubscriptionId;
            client.SubscriptionId = subscriptionId;
            client.TenantID = tenants.First().TenantId;

            var httpclient = new HttpClient();

            httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token.graphToken);
            httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
            var tenantdetails = await httpclient.GetStringAsync("https://graph.windows.net/" + client.TenantID + "/tenantDetails?api-version=1.6");

            var res = await httpclient.GetStringAsync("https://graph.windows.net/sjkpdevs.onmicrosoft.com/applications?api-version=1.6"); //1.42-previewInternal

            var graphClient = new ActiveDirectoryClient(new Uri("https://graph.windows.net/sjkpdevs.onmicrosoft.com"), () =>
            {
                return Task.FromResult(token.graphToken);
            });

            try
            {                
                await graphClient.Applications.AddApplicationAsync(new Microsoft.Azure.ActiveDirectory.GraphClient.Application()
                {
                    DisplayName = "Test 12345",
                    //AppId = "https://ssltest.sjkp.dk",
                    IdentifierUris = new[] { "https://ssltest.sjkp.dk" },
                    RequiredResourceAccess = new []
                    {
                        new RequiredResourceAccess()
                        {
                            ResourceAppId = "797f4846-ba00-4fd7-ba43-dac1f8f63013",
                            ResourceAccess = new []
                            {                               
                                new ResourceAccess()
                                {
                                    Id = new Guid("41094075-9dad-400e-a0bd-54e686782033"),
                                    Type = "Scope"
                                }
                            }
                        }
                    },
                    PasswordCredentials = {new Microsoft.Azure.ActiveDirectory.GraphClient.PasswordCredential()
                {
                    KeyId = Guid.NewGuid(),
                    Value = "simon123",
                    EndDate = DateTime.UtcNow.AddYears(2),
                    StartDate = DateTime.UtcNow,
                } },
                    Homepage = "https://ssltest.sjkp.dk",
                });
            }
            catch (Exception ex)
            {
                
            }

            var appQuery = await graphClient.Applications.Where(s => s.DisplayName == "Test 12345").ExecuteAsync();

            var app = appQuery.CurrentPage.First();


            try
            {
                await graphClient.ServicePrincipals.AddServicePrincipalAsync(new Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal()
                {
                    AppId = app.AppId,
                });
            } catch(Exception ex)
            {

            }
           


            var servicePrincipals = await graphClient.ServicePrincipals.ExecuteAsync();

            var sp = servicePrincipals.CurrentPage.Where(s => s.AppDisplayName == "Test 12345");

            var objectId = sp.First().ObjectId;

            var resourceGroup = new ResourceManagementClient(creds);
            resourceGroup.SubscriptionId = subscriptionId;
            var resourceGroupList = resourceGroup.ResourceGroups.List();

            var armClient = new AuthorizationManagementClient(creds);
            armClient.SubscriptionId = subscriptionId;
            
            //https://msdn.microsoft.com/en-us/library/azure/dn906887.aspx 
            var rolesAssignments = armClient.RoleAssignments.ListForResourceGroup(resourceGroupList.First().Name);
            var scope = string.Format("/subscriptions/{0}/resourceGroups/{1}", subscriptionId, resourceGroupList.First().Name);
            var roleDefinitions = armClient.RoleDefinitions.List(scope);
            armClient.RoleAssignments.Create(scope, Guid.NewGuid().ToString(), new RoleAssignmentProperties()
            {
                
                PrincipalId = objectId,
                RoleDefinitionId = roleDefinitions.First().Id
            });
          

            return Ok();
        }

    }
}
