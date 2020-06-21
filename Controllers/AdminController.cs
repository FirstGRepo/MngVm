using MngVm.BAL;
using MngVm.Filters;
using MngVm.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MngVm.Constant;

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

        public ActionResult Index()
        {
            throw new Exception();
            if (LDAPContants.LDAPOU.Contains(","))
                ViewBag.OuList = new SelectList(LDAPContants.LDAPOU.Split(","));
            else
                ViewBag.OuList = new SelectList(LDAPContants.LDAPOU);
            if (_ldapService.GetUPNSuffixes().IsNotNull() && _ldapService.GetUPNSuffixes().Count > 0)
                ViewBag.Upn = new SelectList(_ldapService.GetUPNSuffixes());
            else
                ViewBag.Upn = new string[] { string.Empty };
            return View();
        }

        public ActionResult List()
        {
            List<User> users = _ldapService.GetAllUsers();
            return PartialView("_userList", users);
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            var message = _ldapService.CreateNewUser(user);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(User user)
        {

            var isAdded = _ldapService.CreateNewUser(user);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string userName)
        {

            var isAdded = _ldapService.DeleteUser(userName);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

    }
}