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
        AzurePortalService azureService;
        GoogleSheetService googleService;
        public AzureControlController()
        {
            azureService = new AzurePortalService();
            googleService = new GoogleSheetService();
        }

        [HttpGet]
        public HttpResponseMessage Index()
        {
            AzurePortalService service = new AzurePortalService();

            service.GetVM("", "");

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage PerformVMScheduler(RunCommandRequest runCmdRequest)
        {

            if (runCmdRequest.IsNotNull())
            {
                AzureVMLogger _prop = new AzureVMLogger();

                var vmSheetDetail = googleService.GetVMLogDetail(_prop.VMLogSpreadSheetID, _prop.VMLogSheetName, runCmdRequest.VmName);

                var vmDetail = azureService.GetVMDetail(runCmdRequest.ResourceName, runCmdRequest.VmName);




                string currentDateTime = DateTime.Now.ToString(CommonConstant.DateTimeFormat);


                googleService.UpdateCellValue(_prop.VMLogSpreadSheetID,
                                              _prop.VMLogSheetName,
                                              _prop.LastSheetUpdated,
                                              currentDateTime);


            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
