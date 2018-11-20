using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MemberIntegralMS.Controllers
{
    public class ShopGlController : BaseController
    {
        // GET: ShopGl
        public ActionResult ShopG()
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
        public ActionResult DoTancUpAdmin(string U_LoginName,int S_ID)
        {
            if (mpms.Users.FirstOrDefault(x => x.U_LoginName == U_LoginName)!=null)
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
            return Json(mpms.SaveChanges() > 0?1:0, JsonRequestBehavior.AllowGet);
        }
        // GET: ShopGl
        public ActionResult GetS_Category()
        {
            var r =  mpms.CategoryItems.Where(x => x.C_Category == "S_Category");
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
        public ActionResult getShopslist(string S_Name, string S_ContactName, string S_Address)
        {
            var sj = from s in mpms.Shops join d in
                     mpms.CategoryItems.Where(x=>x.C_Category=="S_Category") on
                     s.S_Category equals d.CI_ID select new {
                         S_ID=s.S_ID,
                         S_Name=s.S_Name,
                         CI_Name=d.CI_Name,
                         S_ContactName=s.S_ContactName,
                         S_ContactTel=s.S_ContactTel,
                         S_Address=s.S_Address,
                         S_Remark= s.S_Remark,
                         S_Category= s.S_Category,
                         S_IsHasSetAdmin=s.S_IsHasSetAdmin
                     } ;
            sj = S_Name == "" ? sj : sj.Where(x => x.S_Name.Contains(S_Name));
            sj = S_ContactName == "" ? sj : sj.Where(x => x.S_ContactName.Contains(S_ContactName));
            sj = S_Address == "" ? sj : sj.Where(x => x.S_Address.Contains(S_Address));
            int zes =sj.ToList().Count;
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            sj = sj.OrderBy(x=>x.S_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
            var info = new { total = zes, rows = sj };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        // 添加修改页面
        public ActionResult AddOrUpdate()
        {
            return View();
        }
        // 添加修改方法
        public ActionResult DoAddOrUpdate(Shops sj)
        {
            if (sj.S_ID==0)
            {//添加
                mpms.Shops.Add(sj);
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }
            Shops qqq = mpms.Shops.FirstOrDefault(x => x.S_ID == sj.S_ID);
            qqq.S_Name = sj.S_Name;
            qqq.S_Remark = sj.S_Remark;
            qqq.S_ContactTel = sj.S_ContactTel;
            qqq.S_ContactName = sj.S_ContactName;
            qqq.S_Category = sj.S_Category;
                qqq.S_Address = sj.S_Address;
            qqq.S_IsHasSetAdmin = false;
            qqq.S_CreateTime = DateTime.Now;
            mpms.Entry<Shops>(qqq).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges()>0,JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult ShopGlDelete(int S_ID)
        {
            var r = mpms.Shops.FirstOrDefault(x => x.S_ID == S_ID);
            mpms.Shops.Remove(r as Shops);
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