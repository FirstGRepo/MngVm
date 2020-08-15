using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Monitor.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MngVm.Constant;
using MngVm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace MngVm.BAL
{
    public class AzurePortalService
    {
        private AzureCredentials credentials;
        private AzureCredentialProperties azureCredentialProperties;
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

            azureCredentialProperties = GetCredentialFromFile(CommonConstant.AzureAuthFilePath);
        }

        public IVirtualMachine GetVM(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }

        public IVirtualMachine GetVMDetail(string groupName, string vmName)
        {
            return service.VirtualMachines.GetByResourceGroup(groupName, vmName);
        }
        //public void Start(string groupName, string vmName)
        //{
        //    IVirtualMachine machine = GetVM(groupName, vmName);
        //    machine.Start();
        //}

        public bool Start(string groupName, string vmName)
        {
            bool _return = false;
            if (groupName.IsNotNullOrEmpty() && vmName.IsNotNullOrEmpty())
            {
                service.VirtualMachines.GetByResourceGroup(groupName, vmName).StartAsync();
                _return = true;
            }

            return _return;
        }

        public bool Stop(string groupName, string vmName)
        {
            bool _return = false;
            if (groupName.IsNotNullOrEmpty() && vmName.IsNotNullOrEmpty())
            {
                service.VirtualMachines.GetByResourceGroup(groupName, vmName).DeallocateAsync();
                _return = true;
            }

            return _return;
        }
        public PowerState GetVMStatus(string groupName, string vmName)
        {
            IVirtualMachine machine = GetVM(groupName, vmName);
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

        public IMonitorManager GetMonitorManager(string subscriptionId)
        {
            return MonitorManager
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials, subscriptionId);
        }

        public AzureCredentialProperties GetCredentialFromFile(string path)
        {
            AzureCredentialProperties _return = new AzureCredentialProperties();
            try
            {
                Type objType = typeof(AzureCredentialProperties);
                foreach (var row in System.IO.File.ReadAllLines(path))
                {
                    var keyValue = row.Split('=');
                    var type = objType.GetProperties().Where(x => x.CustomAttributes.Any(i => i.NamedArguments[0].TypedValue.Value.Equals(keyValue[0]))).FirstOrDefault();

                    if (type.IsNotNull())
                    {
                        type.SetValue(_return, string.Join("=", keyValue.Skip(1).ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
                _return = null;
            }

            return _return;
        }

        public string GetBearerAuthToken()
        {
            string _token = string.Empty;

            if (azureCredentialProperties.IsNotNull())
            {
                var context = new AuthenticationContext("https://login.microsoftonline.com/" + azureCredentialProperties.TenantID, false);
                var result = context.AcquireTokenAsync("https://management.core.windows.net/", new ClientCredential(azureCredentialProperties.ClientID, azureCredentialProperties.SecretKey)).Result;

                //_token = string.Concat("Bearer ", result.AccessToken); 
                _token = result.AccessToken;
            }

            return _token;
        }

        public AzureUsageDetail GetAzureUsageDetail(string apiVersion, string expand = null, string filter = null)
        {
            AzureUsageDetail _return = new AzureUsageDetail();
            HttpClient httpClient = new HttpClient();

            string requestUri = $"https://management.azure.com/subscriptions/{azureCredentialProperties.SubscriptionID}/providers/Microsoft.Consumption/usageDetails?api-version={apiVersion}";

            if (expand.IsNotNullOrEmpty())
            {
                requestUri = requestUri.Concat("&$expand=", expand);
            }

            if (filter.IsNotNullOrEmpty())
            {
                requestUri = requestUri.Concat("&$filter=", filter);
            }

            try
            {
                string _token = GetBearerAuthToken();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = httpClient.GetAsync(requestUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;

                    _return = JsonConvert.DeserializeObject<AzureUsageDetail>(content);

                }



            }
            catch (Exception ex)
            {

            }

            return _return;
        }

        public AzureUsageApiResponse GetAzureUsageDetail(string apiVersion, AzureUsageApiRequest requestBody)
        {
            AzureUsageApiResponse _return = new AzureUsageApiResponse();
            HttpClient httpClient = new HttpClient();

            string requestUri = $"https://management.azure.com/subscriptions/{azureCredentialProperties.SubscriptionID}/providers/Microsoft.CostManagement/query?api-version={apiVersion}";

            try
            {
                string _token = GetBearerAuthToken();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = httpClient.PostAsJsonAsync(requestUri, requestBody).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;

                    _return = JsonConvert.DeserializeObject<AzureUsageApiResponse>(content);

                }



            }
            catch (Exception ex)
            {

            }

            return _return;
        }

        public AzureUsageApiRequest GetAzureLastMonthRequestBody()
        {

            return new AzureUsageApiRequest()
            {
                type = "ActualCost",
                timeframe = "TheLastMonth",
                dataset = new Dataset()
                {
                    granularity = "None",
                    aggregation = new Aggregation() { totalCost = new TotalCost() { name = "PreTaxCost", function = "Sum" } },
                    grouping = new List<Grouping>() { new Grouping() { type = "Dimension", name = "ResourceGroup" }, new Grouping() { type = "Dimension", name = "ResourceId" } },
                    filter = new Filter()
                    {
                        and = new List<And>()
                            {
                                new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } },
                                new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } },
                            new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } }
                            }
                    }
                }


            };
        }

        public AzureUsageApiRequest GetAzureCurrentMonthRequestBody()
        {

            return new AzureUsageApiRequest()
            {
                type = "ActualCost",
                timeframe = "MonthToDate",
                dataset = new Dataset()
                {
                    granularity = "None",
                    aggregation = new Aggregation() { totalCost = new TotalCost() { name = "PreTaxCost", function = "Sum" } },
                    grouping = new List<Grouping>() { new Grouping() { type = "Dimension", name = "ResourceGroup" }, new Grouping() { type = "Dimension", name = "ResourceId" } },
                    filter = new Filter()
                    {
                        and = new List<And>()
                            {
                                new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } },
                                new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } },
                            new And() { dimensions = new Dimensions() { name = "ResourceType", Operator = "In", values = new List<string>() { "microsoft.compute/virtualmachines" } } }
                            }
                    }
                }


            };
        }

        public AzureVMCPU GetVMCPUAverage(string resourceGroup, string vmName, DateTime date, string apiVersion)
        {
            AzureVMCPU _return = new AzureVMCPU();

            List<string> cpuDate = new List<string>() { "P1D", "PT6H", "PT1H", "PT5M" };

            HttpClient httpClient = new HttpClient();
            string timespanA = string.Empty;
            string timespanB = JsonConvert.SerializeObject(date).Replace(@"""", @"");
            string requestUri = string.Empty;

            try
            {
                string _token = GetBearerAuthToken();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                foreach (var interval in cpuDate)
                {
                    switch (interval)
                    {
                        case "P1D":
                            timespanA = JsonConvert.SerializeObject(date.AddDays(-1)).Replace(@"""", @"");
                            break;
                        case "PT6H":
                            timespanA = JsonConvert.SerializeObject(date.AddHours(-6)).Replace(@"""", @"");
                            break;
                        case "PT1H":
                            timespanA = JsonConvert.SerializeObject(date.AddHours(-1)).Replace(@"""", @"");
                            break;
                        case "PT5M":
                            timespanA = JsonConvert.SerializeObject(date.AddMinutes(-5)).Replace(@"""", @"");
                            break;
                    }

                    //requestUri = $"https://management.azure.com/subscriptions/{azureCredentialProperties.SubscriptionID}/resourceGroups/{resourceGroup}/providers/Microsoft.Compute/virtualMachines/{vmName}/providers/microsoft.insights/metrics?timespan={timespanA}/{timespanB}&interval={interval}&api-version={apiVersion}&metricnames=Percentage CPU"; 
                    requestUri = $"https://management.azure.com/subscriptions/{azureCredentialProperties.SubscriptionID}/resourceGroups/{resourceGroup}/providers/Microsoft.Compute/virtualMachines/{vmName}/providers/microsoft.insights/metrics?timespan={interval}&interval={interval}&api-version={apiVersion}&metricnames=Percentage CPU";


                    var response = httpClient.GetAsync(requestUri).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;

                        var azureCPUUsage = JsonConvert.DeserializeObject<AzureCPUUsage>(content);

                        if (azureCPUUsage.IsNotNull() && azureCPUUsage.value.IsNotNull() && azureCPUUsage.value.Count > 0)
                        {
                            var _value = azureCPUUsage.value[0];

                            if (_value.timeseries.IsNotNull() && _value.timeseries.Count > 0)
                            {
                                var _timeseriesData = _value.timeseries[0].data;
                                if (_timeseriesData.IsNotNull() && _timeseriesData.Count > 0)
                                {
                                    var _lastRecord = _timeseriesData[_timeseriesData.Count - 1];

                                    switch (interval)
                                    {
                                        case "P1D":
                                            _return.Average24Hr = _lastRecord.average;
                                            break;
                                        case "PT6H":
                                            _return.Average6Hr = _lastRecord.average;
                                            break;
                                        case "PT1H":
                                            _return.Average1Hr = _lastRecord.average;
                                            break;
                                        case "PT5M":
                                            _return.Average5min = _lastRecord.average;
                                            break;
                                    }

                                }

                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {

            }

            return _return;
        }

    }
}