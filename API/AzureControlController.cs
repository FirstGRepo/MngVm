using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using MngVm.BAL;
using MngVm.Constant;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MngVm.API
{
    public class AzureControlController : ApiController
    { // GET: AzureControl
        //AzurePortalService azureService;
        //GoogleSheetService googleService;
        public AzureControlController()
        {
            //azureService = new AzurePortalService();
            //googleService = new GoogleSheetService();
        }

        [HttpGet]
        public HttpResponseMessage Index()
        {
            AzurePortalService service = new AzurePortalService();

            //service.GetVM("", "");
            //var _azureGoogleProcess = new AzureGoogleProcess();
            //_azureGoogleProcess.PerformAzureToGoogleActionCPUUpdate();
            //_azureGoogleProcess.PerformAzureToGoogleActionLastNCurrentMonthCostUpdate();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[HttpPost]
        //public HttpResponseMessage PerformVMScheduler(RunCommandRequest runCmdRequest)
        //{

        //    if (runCmdRequest.IsNotNull())
        //    {
        //        AzureVMLogger _prop = new AzureVMLogger();

        //        TimeSpan excatIdleTime = CommonConstant.GetIdleTime(runCmdRequest.IdleTime);

        //        bool isSessionNameNumeric = int.TryParse(runCmdRequest.SessionName, out int sessionInteger);

        //        bool isUserActive = runCmdRequest.State.Contains("active", StringComparison.OrdinalIgnoreCase);

        //        var vmSheetDetail = googleService.GetVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName, runCmdRequest.VmName);

        //        if (vmSheetDetail.IsNotNull())
        //        {
        //            var autoShutDetail = googleService.GetVMAutoShutDetail();

        //            var vmDetail = azureService.GetVMDetail(runCmdRequest.ResourceName, runCmdRequest.VmName);
        //            if (vmDetail.IsNotNull() && autoShutDetail.IsNotNull())
        //            {

        //                //Machine is Off
        //                if (runCmdRequest.State.IsNull())
        //                {
        //                    if (vmSheetDetail.ServerStatus.Contains(ServerStatus.Running, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        //Update Server status to Stop and change datetime of server
        //                        googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);
        //                    }

        //                    if (vmSheetDetail.UserActiveStatus.Contains(UserStatus.Active, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        //Update user status to no and change datetime of user
        //                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);
        //                    }
        //                }
        //                else if (isSessionNameNumeric) //User is not connected to rdp
        //                {

        //                    if (excatIdleTime >= autoShutDetail.AutoShutTime)
        //                    {

        //                        //Shutdown the machine and update in excel sheet with no user and server stopped
        //                        azureService.StopVMByIDAsync(vmDetail.VMId);
        //                        googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);
        //                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);

        //                    }
        //                    else
        //                    {
        //                        if (vmSheetDetail.UserActiveStatus.Contains(UserStatus.Active, StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Update user status to no and change datetime of user
        //                            googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);
        //                        }
        //                    }

        //                }
        //                else if (!isSessionNameNumeric) //User is connected to rdp
        //                {
        //                    if (excatIdleTime >= autoShutDetail.AutoShutTime)
        //                    {
        //                        //Shutdown the machine and update in excel sheet with no user and server stopped
        //                        azureService.StopVMByIDAsync(vmDetail.VMId);
        //                        googleService.UpdateVmLogServerStatus(vmSheetDetail.rowId, ServerStatus.Stopped, true);
        //                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.InActive, true);

        //                    }
        //                    else if (!vmSheetDetail.UserActiveStatus.Contains(UserStatus.Active, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        //Update user status to yes and change datetime of user
        //                        googleService.UpdateVmLogUserStatus(vmSheetDetail.rowId, UserStatus.Active, true);
        //                    }
        //                }




        //                //azureService.StopVMByIDAsync(vmDetail.VMId);

        //                string currentDateTime = DateTime.Now.ToString(CommonConstant.DateTimeFormat);


        //                googleService.UpdateCellValue(_prop.VMLogSpreadSheetID,
        //                                              _prop.VMLogSheetName,
        //                                              _prop.LastSheetUpdated,
        //                                              currentDateTime);



        //            }
        //        }



        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}


        [HttpPost]
        public HttpResponseMessage ProcessVMScheduler(IList<RunCommandRequest> requestList)
        {

            requestList = new List<RunCommandRequest>();

            GenerateDumbData(ref requestList);


            if (requestList.IsNull())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, MessageConstant.InsufficientRequestParameter);
            }

            var grpRequest = requestList.GroupBy(x => x.SessionHostName);

            if (grpRequest.IsNotNull())
            {
                var _azureGoogleProcess = new AzureGoogleProcess();
                foreach (var host in grpRequest)
                {
                    if (host.IsNotNull() && host.Count() > 0)
                    {
                        _azureGoogleProcess.PerformAzureToGoogleAction(host.ToList(), host.Key);
                    }

                }

                var _listVm = grpRequest.Select(x => x.Key).ToList();

                _azureGoogleProcess.PerformAzureToGoogleActionForUnlistedVM(_listVm);

                //_azureGoogleProcess.PerformAzureToGoogleActionCPUandCostUpdate(); 
                _azureGoogleProcess.PerformAzureToGoogleActionCPUUpdate();

                _azureGoogleProcess.PerformAzureToGoogleActionLastNCurrentMonthCostUpdate();

                _azureGoogleProcess.UpdateSheetDate();
            }




            return Request.CreateResponse(HttpStatusCode.OK, MessageConstant.SchedulerSuccess);
        }

        private void GenerateDumbData(ref IList<RunCommandRequest> requestList)
        {
            requestList.Add(new RunCommandRequest()
            {
                SessionHostName = "WVDHP01VM-0.fws.local",
                AdUserName = @"FWS\test",
                LogonTime = DateTime.Now,
                SessionId = "2",
                State = "Active",
                TenantName = "FWS Tenant"

            });


            //requestList.Add(new RunCommandRequest()
            //{
            //    SessionHostName = "OGUSWHP04VM-0.fws.local",
            //    AdUserName = @"FWS\brandon.us",
            //    LogonTime = DateTime.Now,
            //    SessionId = "3",
            //    State = "Active",
            //    TenantName = "FWS Tenant"

            //});


            //requestList.Add(new RunCommandRequest()
            //{
            //    SessionHostName = "OGUSWHP04VM-0.fws.local",
            //    AdUserName = @"FWS\fwsadadmin",
            //    LogonTime = DateTime.Now,
            //    SessionId = "4",
            //    State = "Disconnected",
            //    TenantName = "FWS Tenant"

            //});


            //requestList.Add(new RunCommandRequest()
            //{
            //    SessionHostName = "OGUSWHP04VM-0.fws.local",
            //    AdUserName = @"FWS\josh",
            //    LogonTime = DateTime.Now,
            //    SessionId = "6",
            //    State = "Disconnected",
            //    TenantName = "FWS Tenant"
            //});
        }
    }
}
