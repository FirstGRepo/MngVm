using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using MngVm.BAL;
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
        [HttpGet]
        public HttpResponseMessage Index()
        {
            AzurePortalService service = new AzurePortalService();

            service.GetVM("","");

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
