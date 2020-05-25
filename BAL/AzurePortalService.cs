using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.BAL
{
    public class AzurePortalService
    {

        private AzureCredentials credentials;
        public IAzure service;
        public AzurePortalService()
        {
            credentials = SdkContext.AzureCredentialsFactory
                            .FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Credentials/azureauth.properties"));

            service = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

        }

        public IVirtualMachine GetVM(string groupName, string vmName)
        {
            groupName = "ADrG";
            vmName = "HP01VM-0";
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }

    }
}