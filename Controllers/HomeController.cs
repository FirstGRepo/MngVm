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
                if (user.IsNotNull() && user.machineInfo.Count > 0)
                {
                    foreach (var vm in user.machineInfo)
                    {
                        var powerState = _azService.GetVMStatus(vm.Resource_Group, vm.MachineName);
                        if (powerState != null && powerState.Value.Contains("deallocat"))
                            _azService.Start(vm.Resource_Group, vm.MachineName);
                    }
                    return View(user);
                }
                else
                    return Content(MessageConstant.NoMachineFound);
            }
            else
                return Content(MessageConstant.LoginUrl);
        }

        public ActionResult GetStatus(string resourceGroupName, string machineName)
        {
            var retVal = _azService.GetVMStatus(resourceGroupName, machineName);
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Start(string machineName,string resourceGrp)
        {
            string username = Convert.ToString(Session["username"]);
            UserProfile user = _gService.GetUserVMDetail(username);
            _azService.Start(resourceGrp, machineName);
            var retVal = _azService.GetVMStatus(resourceGrp, machineName);
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