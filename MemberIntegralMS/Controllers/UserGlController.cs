using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberIntegralMS.Controllers
{
    public class UserGlController : BaseController
    {
        // GET: UserGl
        public ActionResult UserG()
        {
            return View();
        }
        

        MPMSEntities mpms = new MPMSEntities();

        //分配页面
        public ActionResult TancUpAdmin()
        {
            return View();
        }
        // 分配页面方法
        public ActionResult DoTancUpAdmin(string U_LoginName, int S_ID)
        {
            if (mpms.Users.FirstOrDefault(x => x.U_LoginName == U_LoginName) != null)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            Users use = new Users
            {
                S_ID = S_ID,
                U_LoginName = U_LoginName,
                U_Password = "123456",
                U_Role = 2
            };
            mpms.Users.Add(use);
            mpms.SaveChanges();
            Shops sh = mpms.Shops.FirstOrDefault(x => x.S_ID == S_ID);
            sh.S_IsHasSetAdmin = true;
            mpms.Entry<Shops>(sh).State = System.Data.Entity.EntityState.Modified;
            return Json(mpms.SaveChanges() > 0 ? 1 : 0, JsonRequestBehavior.AllowGet);
        }
        // GET: U_Role
        public ActionResult GetU_Role()
        {
            var r = mpms.CategoryItems.Where(x => x.C_Category == "U_Role"&&x.CI_ID!=1);
            var sj = from s in r
                     select new
                     {
                         CI_ID = s.CI_ID,
                         CI_Name = s.CI_Name
                     };
            var a = Json(sj, JsonRequestBehavior.AllowGet);
            return a;
        }

        //查询方法
        public ActionResult getUserlist(string U_LoginName, string U_RealName, string U_Telephone)
        {
            var sid = (Session["Users"] as Users).S_ID;
            var sj = from s in mpms.Users
                     join d in
                mpms.CategoryItems.Where(x => x.C_Category == "U_Role") on
                s.U_Role equals d.CI_ID where s.S_ID== sid && s.U_Role!=1
                     select new
                     {
                         U_ID = s.U_ID,
                         U_LoginName = s.U_LoginName,
                         CI_Name = d.CI_Name,
                         U_RealName = s.U_RealName,
                         U_Sex = s.U_Sex,
                         U_Telephone = s.U_Telephone,
                         U_Role = s.U_Role,
                         U_CanDelete = s.U_CanDelete,
                     };
            sj = U_LoginName == "" ? sj : sj.Where(x => x.U_LoginName.Contains(U_LoginName));
            sj = U_RealName == "" ? sj : sj.Where(x => x.U_RealName.Contains(U_RealName));
            sj = U_Telephone == "" ? sj : sj.Where(x => x.U_Telephone.Contains(U_Telephone));
            int zes = sj.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            sj = sj.OrderBy(x => x.U_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
            var info = new { total = zes, rows = sj };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        // 添加修改页面
        public ActionResult AddOrUpdate()
        {
            return View(Session["Users"] as Users);
        }
        // 添加修改方法
        public ActionResult DoAddOrUpdate(Users sj)
        {
            if (sj.U_ID == 0)
            {//添加
                if ( mpms.Users.FirstOrDefault(x => x.U_LoginName == sj.U_LoginName) != null)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
                sj.U_Password = "123456";
                mpms.Users.Add(sj);
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }
      
            Users qqq = mpms.Users.FirstOrDefault(x => x.U_ID == sj.U_ID);
            if (sj.U_LoginName != qqq.U_LoginName && mpms.Users.FirstOrDefault(x => x.U_LoginName == sj.U_LoginName) != null)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
                qqq.U_LoginName = sj.U_LoginName;
            qqq.U_RealName = sj.U_RealName;
            qqq.U_Telephone = sj.U_Telephone;
            qqq.U_Sex = sj.U_Sex;
            qqq.U_CanDelete = sj.U_CanDelete;
            qqq.U_Role = sj.U_Role;
            qqq.S_ID = sj.S_ID;
            mpms.Entry<Users>(qqq).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult UserGlDelete(int U_ID)
        {
            var r = mpms.Users.FirstOrDefault(x => x.U_ID == U_ID);
            mpms.Users.Remove(r as Users);
            try
            {
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}