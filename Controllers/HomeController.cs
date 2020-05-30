using MngVm.BAL;
using MngVm.Common;
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
        public ActionResult Index()
        {
            string username = Convert.ToString(Session["username"]);
            // ApiService service = new ApiService();
            GoogleService google = new GoogleService();
            if (!string.IsNullOrEmpty(username))
            {
                UserProfile user = new UserProfile();
                user.email = username;
                user.machines = google.getVM(username);
                if (user.machines.Count > 0)
                {
                    //foreach (var vmName in user.machines)
                    //{
                    //    var retVal = service.getStatus(vmName);
                    //    if (retVal.displayStatus != null && retVal.displayStatus.Contains("deallocat"))
                    //        service.onActionServer(Constant.OperationStart, vmName);
                    //}
                    return View(user);
                }
                else
                    return Content("No macchine found which assigned to your login, Please contact to your team.");
            }
            else
                return Content("Please Login with google, to proceed click <a href='/'>here</a>. ");
        }

        //public ActionResult GetStatus(string machineName)
        //{
        //    //ApiService service = new ApiService();
        //    var retVal = service.getStatus(machineName);
        //    return Json(retVal, JsonRequestBehavior.AllowGet);
        //}

        // public ActionResult Start()
        //{
        //    string username = Convert.ToString(Session["username"]);
        //   // ApiService service = new ApiService();
        //    GoogleService google = new GoogleService();

        //    List<string> VMs = google.getVM(username);
        //    bool retVal = service.onActionServer(Constant.OperationStart, VMs[0]);
        //    return Json(retVal, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Stop()
        //{
        //    string username = Convert.ToString(Session["username"]);
        //    //ApiService service = new ApiService();
        //    GoogleService google = new GoogleService();
        //    List<string> VMs = google.getVM(username);
        //    bool retVal = service.onActionServer(Constant.OperationStop, VMs[0]);
        //    return Json(retVal, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult list()
        //{
        //    string username = Convert.ToString(Session["username"]);
        //    ApiService service = new ApiService();
        //    bool retVal = service.getList();
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
                if (Constant.IsProduction)
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