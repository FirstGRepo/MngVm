using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class AzureCredentialProperties
    {
        [Display(Name = "subscription")]
        public string SubscriptionID { get; set; }
        [Display(Name = "client")]
        public string ClientID { get; set; }
        [Display(Name = "key")]
        public string SecretKey { get; set; }
        [Display(Name = "tenant")]
        public string TenantID { get; set; }
        [Display(Name = "managementURI")]
        public string ManagementURI { get; set; }
        [Display(Name = "baseURL")]
        public string BaseURL { get; set; }
        [Display(Name = "authURL")]
        public string AuthURL { get; set; }
        [Display(Name = "graphURL")]
        public string GraphURL { get; set; }

    }
}