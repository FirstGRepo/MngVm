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
                                    googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);

                                }

                                if (_onSheetServerActive)
                                {

                                    DateTime? latestTime = vmSheetDetail.ServerDateTime > vmSheetDetail.UserActiveDateTime ? vmSheetDetail.ServerDateTime : vmSheetDetail.UserActiveDateTime;

                                    //if (autoShutDetail.LastUpdated.Value.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime)
                                    if (DateTime.Now.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime)
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
                                                googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);

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
                                                if (DateTime.Now.Subtract(latestTime.Value) >= autoShutDetail.AutoShutTime)
                                                {
                                                    //shutdown system with server status update to Stopped
                                                    azureService.StopVMByVmNameAsync(vmSheetDetail.ResourceGroupName, vmSheetDetail.ServerName);
                                                    googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);

                                                    if (_onSheetUserActive)
                                                    {
                                                        //Update User status to No and change datetime of user
                                                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);

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