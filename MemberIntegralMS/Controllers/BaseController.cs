using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MemberIntegralMS.Controllers
{
    public class BaseController:Controller
    {
       protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Login/Login");
            }
        }


    }
}