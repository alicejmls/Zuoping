using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MemberIntegralMS.Controllers
{
    public class IndexController : BaseController
    {
        MPMSEntities mpms = new MPMSEntities();
        // Index页面
        public ActionResult Index()
        {
            Users user = Session["Users"] as Users;
            return View(user);
        }
        // Index页面
        public ActionResult Indextext()
        {
           
            return View();
        }
        

        // PageUpdateUser页面
        public ActionResult PageUpdateUser()
        {
            return View(Session["Users"] as Users);
        }

        // PageUpdatePwd页面
        public ActionResult PageUpdatePwd()
        {
            return View(Session["Users"] as Users);
        }


        //退出登录
        public ActionResult LogOut()
        {
            Session["Users"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //修改用户信息
        public ActionResult UpdateUser(Users us)
        {
            var uid= (Session["Users"] as Users).U_ID;
            Users user = mpms.Users.FirstOrDefault(x=>x.U_ID==uid);
            if ( us.U_LoginName!= user.U_LoginName && mpms.Users.FirstOrDefault(x=>x.U_LoginName== us.U_LoginName)!=null)
            {

                return Json(new { pd = -1, U_RealNames = user.U_RealName }, JsonRequestBehavior.AllowGet);
            }
            user.U_LoginName = us.U_LoginName;
            user.U_RealName = us.U_RealName;
            user.U_Sex = us.U_Sex;
            user.U_Telephone = us.U_Telephone;

            mpms.Entry<Users>(user).State = System.Data.Entity.EntityState.Modified;
             
            if (mpms.SaveChanges() > 0)
            {
                Session["Users"] = user;
                return Json(new { pd = true, U_RealNames = user.U_RealName }, JsonRequestBehavior.AllowGet);
            }
           return Json(new { pd = false, U_RealNames ="" }, JsonRequestBehavior.AllowGet);


        }

        //修改密码
        public ActionResult UpdatePwd(int uid,string oldPwd,string okPwd)
        {
            Users user = mpms.Users.FirstOrDefault(x => x.U_ID == uid);
            if (user.U_Password!= oldPwd)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            user.U_Password = okPwd;
            mpms.Entry<Users>(user).State = System.Data.Entity.EntityState.Modified;
            Session["Users"] = user;
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }
        
    }
}