using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Rest;
using MngVm.BAL;
using MngVm.Constant;
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
        public ActionResult RedirectToGoogle()
        {
            {
                return Redirect(GoogleContants.redirectToGoogleLogin());
            }
        }
    }
}