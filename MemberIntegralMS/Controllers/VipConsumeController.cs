using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberIntegralMS.Controllers
{
    public class VipConsumeController : Controller
    {
        // GET: FastConsumePage页面
        public ActionResult FastConsumePage()
        {
            return View();
        }

        // GET: SubtractPointPage页面
        public ActionResult SubtractPointPage()
        {
            return View();
        }

        // GET: ConsumeHistoryPage页面
        public ActionResult ConsumeHistoryPage()
        {
            return View();
        }

        MPMSEntities mpms = new MPMSEntities();

        // 查询卡号或电话号是否存在并查询会员信息
        public ActionResult QueryVipIdOrTel(string MC_CardID, string MC_Mobile)
        {

            var list = from m in mpms.MemCards
                       join c in mpms.CardLevels 
                       on m.CL_ID equals c.CL_ID
                       where m.MC_CardID == MC_CardID || m.MC_Mobile == MC_Mobile
                       select new
                       {
                           MC_ID = m.MC_ID,
                           MC_CardID=m.MC_CardID,
                           CL_ID = m.CL_ID,
                           MC_Name = m.MC_Name,
                           MC_Point = m.MC_Point,
                           MC_TotalMoney = m.MC_TotalMoney,
                           CL_LevelName = c.CL_LevelName,
                           CL_Point=c.CL_Point,
                           CL_Percent=c.CL_Percent

                       };
            if (list.Count() > 1)
            {
                return Json(new { pd = -1, mc = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (list.Count()==1)
            {
                return Json(new { pd = 1, mc = list.ToList() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { pd = 0, mc = 0 }, JsonRequestBehavior.AllowGet);
        }

        //快速消费
        public ActionResult FastConsume(int MC_ID,string MC_CardID, float CO_DiscountMoney,int PointNum)
        {
            //修改消费的会员信息
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
            mc.MC_Point += PointNum;
            
            mc.MC_TotalMoney += CO_DiscountMoney;
            mc.MC_TotalCount += 1;

            mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;


            //在转账记录表插入一条记录
            ConsumeOrders co = new ConsumeOrders
            {
                S_ID = (Session["Users"] as Users).S_ID,
                U_ID = (Session["Users"] as Users).U_ID,
                CO_OrderCode = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CO_OrderType = 5,
                MC_ID = MC_ID,
                MC_CardID = MC_CardID,
                EG_ID = 0,
                CO_TotalMoney = CO_DiscountMoney,
                CO_DiscountMoney = CO_DiscountMoney,
                CO_GavePoint = 0,
                CO_Recash = 0,
                CO_Remark="快速消费",
                CO_CreateTime = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"))


            };
            mpms.ConsumeOrders.Add(co);


            return Json(mpms.SaveChanges() > 1, JsonRequestBehavior.AllowGet);
        }


        //会员升级
        public ActionResult VipUpgrade(int MC_ID, int CL_ID, int MC_Point)
        {
            var cl = from c in mpms.CardLevels
                       select new
                       {
                           CL_ID = c.CL_ID,
                           CL_LevelName = c.CL_LevelName,
                           CL_NeedPoint = c.CL_NeedPoint,
                       };
            var r = cl.ToList();
            var isUpgrade = false;
            var UpgradeCL_ID = 0;
            for (int i = 0; i < r.Count; i++)
            {
                if (CL_ID< Convert.ToInt32(r[i].CL_ID) && MC_Point >= Convert.ToInt32( r[i].CL_NeedPoint))
                {
                    UpgradeCL_ID = Convert.ToInt32(r[i].CL_ID);
                    isUpgrade = true;
                }
            }
           
            if (isUpgrade)
            {
                MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
                mc.CL_ID = UpgradeCL_ID;
                mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;
                return Json(mpms.SaveChanges()>0, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);

        }

        //减积分
        public ActionResult SubtractPoint(int MC_ID, string MC_CardID, int CO_GavePoint, string CO_Remark)
        {
            //修改消费的会员信息
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
            mc.MC_Point -= CO_GavePoint;

            mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;


            //在转账记录表插入一条记录
            ConsumeOrders co = new ConsumeOrders
            {
                S_ID = (Session["Users"] as Users).S_ID,
                U_ID = (Session["Users"] as Users).U_ID,
                CO_OrderCode = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CO_OrderType = 3,
                MC_ID = MC_ID,
                MC_CardID = MC_CardID,
                EG_ID = 0,
                CO_TotalMoney = 0,
                CO_DiscountMoney = 0,
                CO_GavePoint = CO_GavePoint,
                CO_Recash = 0,
                CO_Remark = CO_Remark,
                CO_CreateTime = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"))


            };
            mpms.ConsumeOrders.Add(co);


            return Json(mpms.SaveChanges() > 1, JsonRequestBehavior.AllowGet);
        }


        // 查询消费记录
        public ActionResult QueryConsumeHistory(int MC_ID)
        { 

            var list = from m in mpms.MemCards
                       join c in mpms.ConsumeOrders
                       on m.MC_ID equals c.MC_ID
                       join i in
                      (from ct in mpms.CategoryItems where ct.C_Category == "CO_OrderType" select new { CI_ID = ct.CI_ID, CI_Name = ct.CI_Name })
                      on c.CO_OrderType equals i.CI_ID
                       where c.MC_ID == MC_ID 
                       select new
                       {
                           CO_OrderCode = c.CO_OrderCode,
                           MC_CardID = c.MC_CardID,
                           MC_Name = m.MC_Name,
                           CO_GavePoint = c.CO_GavePoint,
                           CO_TotalMoney = c.CO_TotalMoney,
                           CO_DiscountMoney = c.CO_DiscountMoney,
                           CI_Name = i.CI_Name,
                           CO_Recash = c.CO_Recash,
                           CO_CreateTime = c.CO_CreateTime

                       };
            int pageCount = list.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            Session["rows"] = pagesize;
            Session["page"] = page;
            list = list.OrderBy(x => x.CO_OrderCode).Take(pagesize * page).Skip(pagesize * (page - 1));

            var pages = new { total = pageCount, rows = list };
            return Json(pages, JsonRequestBehavior.AllowGet);
        }
    }
}