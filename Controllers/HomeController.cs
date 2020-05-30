using MngVm.BAL;
using MngVm.Constant;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MngVm.Controllers
{
    public class HomeController : Controller
    {
        GoogleSheetService _gService = null;
        AzurePortalService _azService = null;

        public HomeController()
        {
            _gService = new GoogleSheetService();
            _azService = new AzurePortalService();
        }

        public ActionResult Index()
        {
            string username = Convert.ToString(Session["username"]);
            if (!string.IsNullOrEmpty(username))
            {
                UserProfile user = _gService.GetUserVMDetail(username);
                if (user.IsNotNull() && user.machines.Count > 0)
                {
                    foreach (var vmName in user.machines)
                    {
                        var powerState = _azService.GetVMStatus(Constant.Constant.resourceGroupName, vmName);
                        if (powerState != null && powerState.Value.Contains("deallocat"))
                            _azService.Start(Constant.Constant.resourceGroupName, vmName);
                    }
                    return View(user);
                }
                else
                    return Content(MessageConstant.NoMachineFound);
            }
            else
                return Content(MessageConstant.LoginUrl);
        }

        public ActionResult GetStatus(string machineName)
        {
            var retVal = _azService.GetVMStatus(Constant.Constant.resourceGroupName, machineName);
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Start()
        {
            string username = Convert.ToString(Session["username"]);
            UserProfile user = _gService.GetUserVMDetail(username);
            _azService.Start(Constant.Constant.resourceGroupName, user.machines[0]);
            var retVal = _azService.GetVMStatus(Constant.Constant.resourceGroupName, user.machines[0]);
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Stop()
        //{
        //    string username = Convert.ToString(Session["username"]);
        //    UserProfile user = _gService.GetUserVMDetail(username);
        //    _azService.Deallocate(Constant.Constant.resourceGroupName, user.machines[0]);
        //    var retVal = _azService.GetVMStatus(Constant.Constant.resourceGroupName, user.machines[0]);
        //    return Json(retVal, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Success()
        {
            GoogleService google = new GoogleService();
            string code = Convert.ToString(Request.QueryString["code"]);
            string token = google.GetToken(code);
            UserProfile userinfo = google.userProfile(token);
            Session["username"] = userinfo.email;
            if (string.IsNullOrEmpty(userinfo.email))
                return Content("Email not found");
            else
            {
                if (Constant.Constant.IsProduction)
                {
                    if (userinfo.email.Contains("@organizedgains.com"))
                        return RedirectToAction("index");
                    else
                        return Redirect(GoogleContants.ExternalRedirection);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
        }
    }
}