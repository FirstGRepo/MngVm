using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using MngVm.Constant;
using System;

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

        public IVirtualMachine GetVM(string groupName,string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }

        public IVirtualMachine GetVMDetail(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }
        public void Start(string groupName, string vmName)
        {
            IVirtualMachine machine = GetVM(groupName,vmName);
            machine.Start();
        }

        public PowerState GetVMStatus(string groupName, string vmName)
        {
            IVirtualMachine machine = GetVM(groupName,vmName);
            return machine.PowerState;
        }

        public bool StopVMByVmNameAsync(string groupName, string vmName)
        {
            bool _return = false;
            if (groupName.IsNotNullOrEmpty() && vmName.IsNotNullOrEmpty())
            {
                service.VirtualMachines.GetByResourceGroup(groupName, vmName).DeallocateAsync();
                _return = true;
            }

            return _return;
        }

    }
}