using System;
using System.Web.Mvc;

namespace MngVm.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            if (Session["isAdmin"].IsNull())
                return Content(Constant.MessageConstant.NotAuthorize);
            else
                return View();
        }
    }
}