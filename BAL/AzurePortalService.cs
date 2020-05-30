using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using MngVm.Constant;
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
                            .FromFile(CommonConstant.AzureAuthFilePath);

            service = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

        }

        public IVirtualMachine GetVM(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }

        public IVirtualMachine GetVMDetail(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }
    }
}