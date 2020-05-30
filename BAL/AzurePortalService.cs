using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using MngVm.Constant;
using System;
using MngVm.Common;

namespace MngVm.BAL
{
    public class AzurePortalService
    {
        private AzureCredentials credentials;
        public IAzure service;
        public string ResourceGroupName = Constant.resourceGroupName;
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

        public IVirtualMachine GetVM(string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }

        public IVirtualMachine GetVMDetail(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }
        public void Start(string vmName)
        {
            IVirtualMachine machine = GetVM(vmName);
            machine.Start();
        }

        public PowerState GetVMStatus(string vmName)
        {
            IVirtualMachine machine = GetVM(vmName);
            return machine.PowerState;
        }
    }
}