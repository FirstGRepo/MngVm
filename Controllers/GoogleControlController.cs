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

            return View();
        }
    }
}