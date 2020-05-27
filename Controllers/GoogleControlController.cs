using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Rest;
using MngVm.BAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MngVm.Controllers
{
    public class GoogleControlController : Controller
    {
        // GET: GoogleControl
        public ActionResult Index()
        {
            GoogleSheetService gService = new GoogleSheetService();

            string spreadsheetId = "1Doq86OH2r5IfNl06LsrpBG8uEfIDuPSTaCUArtKo5LU";

            string sheet = "Workspace Log";
            var range = $"{sheet}!A4:E5";
            var request = gService.service.Spreadsheets.Values.Get(spreadsheetId, range);


            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var item in values)
                {

                }

            }

            gService.UpdateEntity(spreadsheetId, sheet);




            return View();
        }
    }
}