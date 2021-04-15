using MngVm.BAL;
using MngVm.Constant;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MngVm.Controllers
{
    public class HomeController : BaseController
    {
        GoogleSheetService _gService = null;
        AzurePortalService _azService = null;

        public HomeController()
        {
            _gService = new GoogleSheetService();
            _azService = new AzurePortalService();
        }

        [Authorize]
        [NoCache]
        public ActionResult Index()
        {
            string username = Convert.ToString(Session["username"]);
            if (!string.IsNullOrEmpty(username))
            {
                UserProfile user = _gService.GetUserVMDetail(username);
                if (user.IsNotNull() && user.machineInfo.Count > 0)
                {
                    //foreach (var vm in user.machineInfo)
                    //{
                    //    var powerState = _azService.GetVMStatus(vm.Resource_Group, vm.MachineName);
                    //    if (powerState != null && powerState.Value.Contains("deallocat"))
                    //        _azService.Start(vm.Resource_Group, vm.MachineName);
                    //}
                    return View(user);
                }
                else
                {
                    Session.Abandon();
                    return Redirect(GoogleContants.ExternalRedirection);
                    // return Content(MessageConstant.NoMachineFound);
                }
            }
            else
                return Content(MessageConstant.LoginUrl);
        }

        public ActionResult GetStatus(string resourceGroupName, string machineName)
        {
            var retVal = _azService.GetVMStatus(resourceGroupName, machineName);
            return Json(Convert.ToString(retVal), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Start(string machineName, string resourceGrp)
        //{
        //    _azService.Start(resourceGrp, machineName);
        //    var retVal = _azService.GetVMStatus(resourceGrp, machineName);
        //    return Json(Convert.ToString(retVal), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Start(string machineName, string resourceGrp)
        {
            var powerState = _azService.GetVMStatus(resourceGrp, machineName);
            if (powerState != null && powerState.Value.Contains("deallocat"))
                _azService.Start(resourceGrp, machineName);
            return Json("Starting", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Stop(string machineName, string resourceGrp)
        {
            _azService.Stop(resourceGrp, machineName);
            //var retVal = _azService.GetVMStatus(resourceGrp, machineName);
            return Json("Deallocating", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Success()
        {
            try
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
                    FormsAuthentication.SetAuthCookie(userinfo.email, false);
                    if (Constant.Constant.IsAdmin(userinfo.email))
                    {
                        Session["isAdmin"] = true;
                        return RedirectToAction("index", "Admin");
                    }
                    else if (Constant.Constant.IsSubAdmin(userinfo.email))
                    {
                        Session["isSubAdmin"] = true;
                        return RedirectToAction("index", "Admin");
                    }
                    else
                    {
                        //if (Constant.Constant.IsProduction)
                        //{
                        //if (userinfo.email.Contains("@organizedgains.com"))
                        return RedirectToAction("index");
                        //else
                        //    return Redirect(GoogleContants.ExternalRedirection);
                        //}
                        //else
                        //{
                        //    return RedirectToAction("index");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }

        [Authorize]
        public ActionResult SignOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "GoogleControl");
            //return RedirectPermanent("https://www.google.com/accounts/Logout");
        }

        public ActionResult Unauthorize()
        {
            return View();
        }
    }
}