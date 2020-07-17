using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;

namespace MngVm.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base

        protected override void OnException(ExceptionContext filterContext)
        {
            //if (filterContext.ExceptionHandled)
            //    return;
            LogError(filterContext.Exception);
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
        }

        protected void LogError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine + "Time : " + DateTime.Now + Environment.NewLine);
            sb.Append("Method : " + ex.TargetSite + Environment.NewLine);
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine + "--------------------------------------------------------------------------" + Environment.NewLine);

            if (!System.IO.Directory.Exists(Server.MapPath("~/Logs")))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath("~/Logs"));
            }
            System.IO.File.AppendAllText(Server.MapPath("~/Logs/") + DateTime.Now.ToString("dd-MM-yyyy") + "_log.txt", sb.ToString());
            sb.Clear();
        }
    }
}