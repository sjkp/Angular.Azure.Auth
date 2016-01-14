using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace Angular.Azure.Auth.Controllers
{
    public class AADApplicationCreateModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string[] IdentifierUris { get; set; }

        [Required]
        public string Homepage { get; set; }

        public IList<RequiredResourceAccess> RequiredResourceAccess { get; set; }

        [Required]
        public string Password { get; set; }
    }
}