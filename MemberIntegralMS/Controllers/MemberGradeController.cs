using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberIntegralMS.Controllers
{
    public class MemberGradeController : Controller
    {
        // GET: MemberGrade
        public ActionResult PageMemberGrade()
        {
            return View();
        }

        MPMSEntities mpms = new MPMSEntities();

        //查询
        public ActionResult QueryCardLevels(string CL_LevelName)
        {
            
            var info = from c in mpms.CardLevels
                       select new
                       {
                           CL_ID = c.CL_ID,
                           CL_LevelName = c.CL_LevelName,
                           CL_NeedPoint = c.CL_NeedPoint,
                           CL_Point = c.CL_Point,
                           CL_Percent = c.CL_Percent
                       };
            info = CL_LevelName == "" ? info : info.Where(x=>x.CL_LevelName.Contains(CL_LevelName));
            int pageCount = info.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            info = info.OrderBy(x => x.CL_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
            var pages = new { total = pageCount, rows = info };
            return Json(pages, JsonRequestBehavior.AllowGet);
        }

        // 添加修改页面
        public ActionResult PageMgAddOrUpdate()
        {
            return View();
        }
        // 添加修改方法
        public ActionResult MgAddOrUpdate(CardLevels sj)
        {
            if (sj.CL_ID == 0)
            {//添加
                if (mpms.CardLevels.FirstOrDefault(x => x.CL_LevelName == sj.CL_LevelName) != null)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
                mpms.CardLevels.Add(sj);
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }
            //修改
            CardLevels qqq = mpms.CardLevels.FirstOrDefault(x => x.CL_ID == sj.CL_ID);
            if (sj.CL_LevelName != qqq.CL_LevelName && mpms.CardLevels.FirstOrDefault(x => x.CL_ID!=sj.CL_ID && x.CL_LevelName == sj.CL_LevelName) != null)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            qqq.CL_LevelName = sj.CL_LevelName;
            qqq.CL_NeedPoint = sj.CL_NeedPoint;
            qqq.CL_Point = sj.CL_Point;
            qqq.CL_Percent = sj.CL_Percent;
            mpms.Entry<CardLevels>(qqq).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }

        //删除
        public ActionResult MgGlDelete(int CL_ID)
        {
            var r = mpms.CardLevels.FirstOrDefault(x => x.CL_ID == CL_ID);
            mpms.CardLevels.Remove(r as CardLevels);
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