using Microsoft.Azure.Management.Compute.Fluent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace MngVm.Filters
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (HttpContext.Current.Session["isAdmin"].IsNull())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{{ "controller", "Home" },
                                          { "action", "Unauthorize" }
                                         });
            }
            else
                filterContext.Result = null;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}