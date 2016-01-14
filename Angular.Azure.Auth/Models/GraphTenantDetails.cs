using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angular.Azure.Auth.Models
{

    public class AssignedPlan
    {
        public string assignedTimestamp { get; set; }
        public string capabilityStatus { get; set; }
        public string service { get; set; }
        public string servicePlanId { get; set; }
    }

    public class ProvisionedPlan
    {
        public string capabilityStatus { get; set; }
        public string provisioningStatus { get; set; }
        public string service { get; set; }
    }

    public class VerifiedDomain
    {
        public string capabilities { get; set; }
        public bool @default { get; set; }
        public string id { get; set; }
        public bool initial { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Value
    {
        public string objectType { get; set; }
        public string objectId { get; set; }
        public object deletionTimestamp { get; set; }
        public List<AssignedPlan> assignedPlans { get; set; }
        public object city { get; set; }
        public object companyLastDirSyncTime { get; set; }
        public object country { get; set; }
        public string countryLetterCode { get; set; }
        public object dirSyncEnabled { get; set; }
        public string displayName { get; set; }
        public List<object> marketingNotificationEmails { get; set; }
        public object postalCode { get; set; }
        public string preferredLanguage { get; set; }
        public List<ProvisionedPlan> provisionedPlans { get; set; }
        public List<object> provisioningErrors { get; set; }
        public List<object> securityComplianceNotificationMails { get; set; }
        public List<object> securityComplianceNotificationPhones { get; set; }
        public object state { get; set; }
        public object street { get; set; }
        public List<string> technicalNotificationMails { get; set; }
        public object telephoneNumber { get; set; }
        public List<VerifiedDomain> verifiedDomains { get; set; }
    }

    public class GraphTenantDetails
    { 
        public List<Value> value { get; set; }
    }

}
