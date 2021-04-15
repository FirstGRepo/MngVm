using MngVm.BAL;
using MngVm.Filters;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MngVm.Constant;
using System.Linq;

namespace MngVm.Controllers
{
    [AdminAuthorize]
    public class AdminController : BaseController
    {
        LDAPService _ldapService;
        public AdminController()
        {
            _ldapService = new LDAPService();
        }

        [Authorize]
        [NoCache]
        public ActionResult Index()
        {
            PowerShellCommand powershell = new PowerShellCommand();
            if (LDAPContants.LDAPOU.Contains(","))
                ViewBag.OuList = new SelectList(LDAPContants.LDAPOU.Split(","));
            else
                ViewBag.OuList = new SelectList(LDAPContants.LDAPOU);

            if (_ldapService.GetUPNSuffixes().IsNotNull() && _ldapService.GetUPNSuffixes().Count > 0)
                ViewBag.Upn = new SelectList(_ldapService.GetUPNSuffixes());
            else
                ViewBag.Upn = new string[] { string.Empty };

            var items = powershell.getHostPools();

            if (items.IsNotNull() && items.Count() > 0)
                ViewBag.HostpoolList = new SelectList(items);
            else
                ViewBag.HostpoolList = new string[] { string.Empty };

            return View();
        }

        public ActionResult List()
        {
            List<User> users = null;

            if (Session["isAdmin"] != null && Convert.ToBoolean(Session["isAdmin"]))
            {
                users = _ldapService.GetAllUsers();
            }
            else if (Session["isSubAdmin"] != null && Convert.ToBoolean(Session["isSubAdmin"]))
            {
                users = _ldapService.GetAllSubAdminUsers();
            }

            return PartialView("_userList", users);
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            var message = _ldapService.CreateNewUser(user, Session["isSubAdmin"] != null && Convert.ToBoolean(Session["isSubAdmin"]));
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(User user)
        {

            var isAdded = _ldapService.CreateNewUser(user, Session["isSubAdmin"] != null && Convert.ToBoolean(Session["isSubAdmin"]));
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string userName, string hostPool)
        {
            var isAdded = _ldapService.DeleteUser(userName, hostPool);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddRemoveSubAdmin(string userName, string hostPool, bool isAdd)
        {
            var isAdded = _ldapService.AddRemoveSubAdmin(userName, isAdd);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword(string username)
        {
            User user = new User { UserName = username };
            return View(user);
        }
        [HttpPost]
        public ActionResult ChangePassword(User user)
        {
            var isAdded = _ldapService.ChangePassword(user);
            //ViewBag.Message = isAdded ? "Password has been changed successfuly" : "Something went wrong";
            //return View();
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVmsByHostpool(string hostPool)
        {
            PowerShellCommand powershell = new PowerShellCommand();
            var items = powershell.getVmList(hostPool);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}