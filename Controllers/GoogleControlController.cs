using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Rest;
using MngVm.BAL;
using MngVm.Common;
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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetSheet()
        {
            GoogleSheetService gService = new GoogleSheetService();

            string spreadsheetId = GoogleContants.SheetId;//"1E3PsxeWZT3QjzYC17-MKwLixY1DU-ZaSniTTeSosQN8"

            string sheet = "Sheet1";
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

            //gService.UpdateEntity(spreadsheetId, sheet);

            return View();
        }
        public ActionResult RedirectToGoogle()
        {
            {
                return Redirect(GoogleContants.redirectToGoogleLogin());
            }
        }
    }
}