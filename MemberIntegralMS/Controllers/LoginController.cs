using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MemberIntegralMS.Controllers
{
    public class LoginController : Controller
    {

        //Login页面
        public ActionResult Login()
        {
            
            return View();
        }

        // 登录
        public ActionResult Logins(string userName, string pwd)
        {
            MPMSEntities mpms = new MPMSEntities();
            Users us = mpms.Users.FirstOrDefault(x => x.U_LoginName == userName && x.U_Password == pwd);
            if (us != null)
            {
                Session["Users"] = us;
                FormsAuthentication.SetAuthCookie((Session["Users"] as Users).U_LoginName, false);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }




    }
}