using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
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
            if (Session["username"].IsNull())
                return View("MainSite");
            else
                return RedirectToAction("Index", "Home");
        }
        public ActionResult MainSite()
        {
            return View();
        }
        public ActionResult RedirectToGoogle()
        {
            {
                return Redirect(GoogleContants.redirectToGoogleLogin());
            }
        }

        public ActionResult RunCommand(string username, string group)
        {
            List<string> TestList1 = new List<string>();
            List<string> TestList2 = new List<string>();
            TestList1.Add("1");
            TestList1.Add("2");
            TestList1.Add("3");
            TestList2.Add("3");
            TestList2.Add("4");
            TestList2.Add("5");
            var CommonList = TestList1.Intersect(TestList2);
            PowerShellCommand power = new PowerShellCommand();
            var data = power.getUserVmList();// AssignMachine(username, group);
            return null;
        }
    }
}