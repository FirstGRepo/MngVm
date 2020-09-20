using Microsoft.Azure.Management.Compute.Fluent;
using MngVm.Constant;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.BAL
{
    public class AzureGoogleProcess
    {
        AzurePortalService azureService;
        GoogleSheetService googleService;
        public AzureGoogleProcess()
        {
            azureService = new AzurePortalService();
            googleService = new GoogleSheetService();

        }
        public bool PerformAzureToGoogleAction(IList<RunCommandRequest> requestList, string SessionHostName)
        {

            bool _return = false;
            try
            {
                if (requestList.IsNotNull())
                {
                    AzureVMLogger _prop = new AzureVMLogger();
                    bool isUserActive = requestList.Any(x => x.State.Contains("Active", StringComparison.OrdinalIgnoreCase));
                    string azureVMName = CommonConstant.GetActualVMName(SessionHostName);

                    var vmSheetDetail = googleService.GetVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName, azureVMName);

                    if (vmSheetDetail.IsNotNull())
                    {
                        var autoShutDetail = googleService.GetVMAutoShutDetail();

                        var vmDetail = azureService.GetVMDetail(vmSheetDetail.ResourceGroupName, azureVMName);
                        if (vmDetail.IsNotNull() && autoShutDetail.IsNotNull())
                        {
                            bool _onSheetUserActive = vmSheetDetail.UserActiveStatus.Contains(UserStatus.Active, StringComparison.OrdinalIgnoreCase);
                            bool _onSheetServerActive = vmSheetDetail.ServerStatus.Contains(ServerStatus.Running, StringComparison.OrdinalIgnoreCase);

                            if (isUserActive)
                            {
                                if (!_onSheetUserActive)
                                {
                                    //Update User status to Yes and change datetime of user
                                    googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.Active, true);

                                }
                                if (!_onSheetServerActive)
                                {
                                    //Update Server status to Run and change datetime of server
                                    googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Running, true);

                                }
                            }
                            else
                            {
                                if (_onSheetUserActive)
                                {
                                    //Update User status to No and change datetime of user
                                    googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, false);

                                }

                                if (_onSheetServerActive)
                                {

                                    DateTime? latestTime = vmSheetDetail.ServerDateTime > vmSheetDetail.UserActiveDateTime ? vmSheetDetail.ServerDateTime : vmSheetDetail.UserActiveDateTime;

                                    //if (autoShutDetail.LastUpdated.Value.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime)
                                    if (DateTime.Now.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime && autoShutDetail.AutoShutTime != new TimeSpan(0, 0, 0, 0))
                                    {
                                        //shutdown system with server status update to Stopped
                                        azureService.StopVMByVmNameAsync(vmSheetDetail.ResourceGroupName, azureVMName);
                                        googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);

                                    }

                                }
                            }

                        }
                    }


                    _return = true;
                }

            }
            catch (Exception ex)
            {
                _return = false;
            }

            return _return;
        }

        public bool PerformAzureToGoogleActionForUnlistedVM(IList<string> vmNameList)
        {

            bool _return = false;
            try
            {
                if (vmNameList.IsNotNull())
                {
                    AzureVMLogger _prop = new AzureVMLogger();

                    var _listAllVM = googleService.GetAllVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName);

                    if (_listAllVM.IsNotNull())
                    {
                        var autoShutDetail = googleService.GetVMAutoShutDetail();

                        var _filteredVMList = _listAllVM.Where(x => !vmNameList.Any(i => CommonConstant.GetActualVMName(i).Equals(x.ServerName, StringComparison.OrdinalIgnoreCase)));

                        if (_filteredVMList.IsNotNull())
                        {
                            foreach (VMLogSheetDetail vmSheetDetail in _filteredVMList)
                            {
                                if (vmSheetDetail.IsNotNull())
                                {
                                    var vmDetail = azureService.GetVMDetail(vmSheetDetail.ResourceGroupName, vmSheetDetail.ServerName);
                                    if (vmDetail.IsNotNull() && autoShutDetail.IsNotNull())
                                    {
                                        bool _onSheetUserActive = vmSheetDetail.UserActiveStatus.Contains(UserStatus.Active, StringComparison.OrdinalIgnoreCase);
                                        bool _onSheetServerActive = vmSheetDetail.ServerStatus.Contains(ServerStatus.Running, StringComparison.OrdinalIgnoreCase);


                                        if (vmDetail.PowerState == PowerState.Deallocating
                                            || vmDetail.PowerState == PowerState.Deallocated
                                            || vmDetail.PowerState == PowerState.Stopping
                                            || vmDetail.PowerState == PowerState.Stopped)
                                        {
                                            if (_onSheetUserActive)
                                            {
                                                //Update User status to No and change datetime of user
                                                googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, false);

                                            }

                                            if (_onSheetServerActive)
                                            {
                                                //Update Server status to Stopped and change datetime of server
                                                googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);

                                            }
                                        }
                                        else if (vmDetail.PowerState == PowerState.Running
                                                || vmDetail.PowerState == PowerState.Starting)
                                        {
                                            if (!_onSheetServerActive)
                                            {
                                                googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Running, true);
                                            }
                                            else
                                            {
                                                DateTime? latestTime = vmSheetDetail.ServerDateTime > vmSheetDetail.UserActiveDateTime ? vmSheetDetail.ServerDateTime : vmSheetDetail.UserActiveDateTime;

                                                //if (autoShutDetail.LastUpdated.Value.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime)
                                                if (DateTime.Now.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime && autoShutDetail.AutoShutTime != new TimeSpan(0, 0, 0, 0))
                                                {
                                                    //shutdown system with server status update to Stopped
                                                    azureService.StopVMByVmNameAsync(vmSheetDetail.ResourceGroupName, vmSheetDetail.ServerName);
                                                    googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);

                                                    if (_onSheetUserActive)
                                                    {
                                                        //Update User status to No and change datetime of user
                                                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, false);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _return = true;
                }

            }
            catch (Exception ex)
            {
                _return = false;
            }

            return _return;
        }

        public bool PerformAzureToGoogleActionLastNCurrentMonthCostUpdate()
        {

            bool _return = false;
            try
            {
                AzureVMLogger _prop = new AzureVMLogger();
                AzureUsageApiRequest _request = new AzureUsageApiRequest();

                var _listAllVM = googleService.GetAllVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName);

                if (_listAllVM.IsNotNull())
                {
                    //var autoShutDetail = googleService.GetVMAutoShutDetail(); 

                    //// Current Month Cost 
                    _request = azureService.GetAzureCurrentMonthRequestBody();

                    var _currentMonthCost = azureService.GetAzureUsageDetail(_prop.ApiVersion, _request);

                    //// Last Month Cost 
                    _request = azureService.GetAzureLastMonthRequestBody();

                    var _lastMonthCost = azureService.GetAzureUsageDetail(_prop.ApiVersion, _request);



                    int _idxThisMonthPreTaxCost = _currentMonthCost?.properties.columns.FindIndex(x => x.name.Equals("PreTaxCost")) ?? 0;
                    int _idxThisMonthResourceGroup = _currentMonthCost?.properties.columns.FindIndex(x => x.name.Equals("ResourceGroup")) ?? 0;
                    int _idxThisMonthResourceId = _currentMonthCost?.properties.columns.FindIndex(x => x.name.Equals("ResourceId")) ?? 0;
                    int _idxThisMonthCurrency = _currentMonthCost?.properties.columns.FindIndex(x => x.name.Equals("Currency")) ?? 0;


                    int _idxLastMonthPreTaxCost = _lastMonthCost?.properties.columns.FindIndex(x => x.name.Equals("PreTaxCost")) ?? 0;
                    int _idxLastMonthResourceGroup = _lastMonthCost?.properties.columns.FindIndex(x => x.name.Equals("ResourceGroup")) ?? 0;
                    int _idxLastMonthResourceId = _lastMonthCost?.properties.columns.FindIndex(x => x.name.Equals("ResourceId")) ?? 0;
                    int _idxLastMonthCurrency = _lastMonthCost?.properties.columns.FindIndex(x => x.name.Equals("Currency")) ?? 0;

                    foreach (VMLogSheetDetail vmSheetDetail in _listAllVM)
                    {
                        decimal thisMonthCost = 0;
                        decimal lastMonthCost = 0;

                        if (vmSheetDetail.IsNotNull())
                        {
                            var _thisMonthCostUpdate = _currentMonthCost?.properties.rows.Where(x => CommonConstant.GetVMNameFromUrl(Convert.ToString(x[_idxThisMonthResourceId])).Contains(vmSheetDetail.ServerName, StringComparison.OrdinalIgnoreCase)
                                                                                                && Convert.ToString(x[_idxThisMonthResourceGroup]).Contains(vmSheetDetail.ResourceGroupName, StringComparison.OrdinalIgnoreCase))
                                                                                        .FirstOrDefault();

                            var _lastMonthCostUpdate = _lastMonthCost?.properties.rows.Where(x => CommonConstant.GetVMNameFromUrl(Convert.ToString(x[_idxLastMonthResourceId])).Contains(vmSheetDetail.ServerName, StringComparison.OrdinalIgnoreCase)
                                                                                                && Convert.ToString(x[_idxLastMonthResourceGroup]).Contains(vmSheetDetail.ResourceGroupName, StringComparison.OrdinalIgnoreCase))
                                                                                       .FirstOrDefault();

                            var vmDetail = azureService.GetVMDetail(vmSheetDetail.ResourceGroupName, vmSheetDetail.ServerName);
                            if (vmDetail.IsNotNull())
                            {
                                if (_thisMonthCostUpdate.IsNotNull())
                                {
                                    decimal.TryParse(Convert.ToString(_thisMonthCostUpdate[_idxThisMonthPreTaxCost]), out thisMonthCost);
                                    thisMonthCost = decimal.Round(thisMonthCost, 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    thisMonthCost = 0;
                                }


                                googleService.UpdateVmLogLastMonthCost(vmSheetDetail.rowId, string.Format("{0:0.00}", thisMonthCost), 1);

                                if (_lastMonthCostUpdate.IsNotNull())
                                {
                                    decimal.TryParse(Convert.ToString(_lastMonthCostUpdate[_idxLastMonthPreTaxCost]), out lastMonthCost);
                                    lastMonthCost = decimal.Round(lastMonthCost, 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    lastMonthCost = 0;
                                }

                                googleService.UpdateVmLogLastMonthCost(vmSheetDetail.rowId, string.Format("{0:0.00}", lastMonthCost), 0);


                                //calculate difference 

                                var _currenMonthtDate = DateTime.Now;
                                var _lastMonthDate = DateTime.Now.AddMonths(-1);
                                int todayDay = _currenMonthtDate.Day;

                                int thisMonthTotalDays = DateTime.DaysInMonth(_currenMonthtDate.Year, _currenMonthtDate.Month);
                                int lastMonthTotalDays = DateTime.DaysInMonth(_lastMonthDate.Year, _lastMonthDate.Month);


                                decimal _avgThisMonthCost = decimal.Round(((thisMonthCost / todayDay) * thisMonthTotalDays), 2, MidpointRounding.AwayFromZero);

                                decimal costDiff = 0;
                                bool isRedColor = false;
                                if (lastMonthCost >= _avgThisMonthCost)
                                {
                                    costDiff = _avgThisMonthCost <= 0 ? 0 : decimal.Round((_avgThisMonthCost * 100) / lastMonthCost, 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    isRedColor = true;
                                    costDiff = _avgThisMonthCost <= 0 ? 0 : decimal.Round(((_avgThisMonthCost - lastMonthCost) * 100) / _avgThisMonthCost, 2, MidpointRounding.AwayFromZero);
                                }

                                googleService.UpdateVmLogDiffCost(vmSheetDetail.rowId, string.Format("{0:0.00}%", Math.Floor(costDiff)), isRedColor);


                            }
                        }



                    }


                }

                _return = true;


            }
            catch (Exception ex)
            {
                _return = false;
            }

            return _return;

        }

        public bool PerformAzureToGoogleActionCPUUpdate()
        {
            bool _return = false;
            try
            {
                AzureVMLogger _prop = new AzureVMLogger();

                var _listAllVM = googleService.GetAllVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName);

                if (_listAllVM.IsNotNull())
                {
                    var _utcDate = DateTime.UtcNow;
                    var _paramDate = new DateTime(_utcDate.Year, _utcDate.Month, _utcDate.Day, _utcDate.Hour, _utcDate.Minute, 0);

                    foreach (VMLogSheetDetail vmSheetDetail in _listAllVM)
                    {
                        if (vmSheetDetail.IsNotNull())
                        {
                            var _cpuUpdate = azureService.GetVMCPUAverage(vmSheetDetail.ResourceGroupName, vmSheetDetail.ServerName, _paramDate, "2019-07-01");

                            if (_cpuUpdate.IsNotNull())
                            {
                                googleService.UpdateVmLogCPUAverage(vmSheetDetail.rowId, string.Format("{0:0.00}", _cpuUpdate.Average5min), 1);
                                googleService.UpdateVmLogCPUAverage(vmSheetDetail.rowId, string.Format("{0:0.00}", _cpuUpdate.Average1Hr), 2);
                                googleService.UpdateVmLogCPUAverage(vmSheetDetail.rowId, string.Format("{0:0.00}", _cpuUpdate.Average6Hr), 3);
                                googleService.UpdateVmLogCPUAverage(vmSheetDetail.rowId, string.Format("{0:0.00}", _cpuUpdate.Average24Hr), 4);
                                googleService.UpdateVmLogCPUAverage(vmSheetDetail.rowId, string.Format("{0:0.00}", _cpuUpdate.Peak24Hr), 5);
                            }
                        }
                    }
                }

                _return = true;


            }
            catch (Exception ex)
            {
                _return = false;
            }

            return _return;
        }

        public bool UpdateSheetDate()
        {
            bool _return = false;
            try
            {
                AzureVMLogger _prop = new AzureVMLogger();

                string currentDateTime = DateTime.Now.ToString(CommonConstant.DateTimeFormat);


                _return = googleService.UpdateCellValue(_prop.VMLogSpreadSheetID,
                                              _prop.VMLogSheetName,
                                              _prop.LastSheetUpdated,
                                              currentDateTime);


            }
            catch (Exception ex)
            {
                _return = false;
            }

            return _return;
        }
    }
}