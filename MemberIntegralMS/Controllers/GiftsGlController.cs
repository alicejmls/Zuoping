using MemberIntegralMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MemberIntegralMS.Controllers
{
    public class GiftsGlController : Controller
    {
        // GET: GiftsGl
        public ActionResult GiftsGl()
        {
            return View();
        }
        MPMSEntities mpms = new MPMSEntities();


        //查询方法
        public ActionResult getUserlist()
        {
            var sid = 2;//组装前测试用
            //var sid = (Session["Users"] as Users).S_ID;
            var sj = from s in mpms.ExchangGifts

                     where s.S_ID == sid 
                     select new
                     {
                         EG_ExchangNum = s.EG_ExchangNum,
                         EG_GiftCode = s.EG_GiftCode,
                         EG_GiftName = s.EG_GiftName,
                         EG_ID = s.EG_ID,
                         EG_Number = s.EG_Number-s.EG_ExchangNum,
                         EG_Photo = s.EG_Photo,
                         EG_Point = s.EG_Point,
                         EG_Remark = s.EG_Remark,
                         S_ID=s.S_ID
                     };
            int zes = sj.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            sj = sj.OrderBy(x => x.EG_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
            var info = new { total = zes, rows = sj };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        // 添加修改页面
        public ActionResult AddOrUpdate()
        {
            return View(Session["Users"] as Users);
        }
        
            public ActionResult UPAddphoto(HttpPostedFileBase photo)
          {
            try
            {
                string path = Server.MapPath("/upLoadImage/");
                string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") +"_"+ photo.FileName;
                photo.SaveAs(path + fileName);
                return Content(("/upLoadImage/" + fileName));
            }
            catch 
            {
                return Content("失败");
            }
          

        }
        // 添加修改方法
        public ActionResult DoAddOrUpdate(ExchangGifts sj)
            {
                if (sj.EG_ID == 0)
            {//添加
                if (mpms.ExchangGifts.FirstOrDefault(x => x.EG_GiftName == sj.EG_GiftName) != null ||
                   mpms.ExchangGifts.FirstOrDefault(x => x.EG_GiftCode == sj.EG_GiftCode) != null)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
                mpms.ExchangGifts.Add(sj);
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }

            ExchangGifts qqq = mpms.ExchangGifts.FirstOrDefault(x => x.EG_ID == sj.EG_ID);
            if ((sj.EG_GiftName != qqq.EG_GiftName && mpms.ExchangGifts.FirstOrDefault(x => x.EG_GiftName == sj.EG_GiftName) != null)||
                (sj.EG_GiftCode != qqq.EG_GiftCode && mpms.ExchangGifts.FirstOrDefault(x => x.EG_GiftCode == sj.EG_GiftCode) != null))
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }

            qqq.EG_GiftCode = sj.EG_GiftCode;
            qqq.EG_GiftName = sj.EG_GiftName;
            qqq.EG_Number = sj.EG_Number;
            qqq.EG_Point = sj.EG_Point;
            qqq.EG_Remark = sj.EG_Remark;
            qqq.EG_Photo = sj.EG_Photo;
            mpms.Entry<ExchangGifts>(qqq).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }
        //删除
        public ActionResult UserGlDelete(int EG_ID)
        {
            var r = mpms.ExchangGifts.FirstOrDefault(x => x.EG_ID == EG_ID);
            string filelj = r.EG_Photo;
            mpms.ExchangGifts.Remove(r as ExchangGifts);
            try
            {
                if (filelj.Length>0)
                {
                    filelj = Server.MapPath(filelj);
                    System.IO.File.Delete(filelj);
                }
               
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}