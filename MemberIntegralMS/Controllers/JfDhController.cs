using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace MemberIntegralMS.Controllers
{
    public class JfDhController : Controller
    {
        
        // GET: JfDh
        public ActionResult JfDh()
        {
            return View();
        }
        // GET: JfDhcx
        public ActionResult JfDhcx()
        {
            return View();
        }
        
        // 查询卡号或电话号是否存在并查询会员信息
        public ActionResult DoJfDhcx(string U_Telephone)
        {
            mpms.Configuration.LazyLoadingEnabled = false;
            var list = from m in mpms.MemCards
                       join c in mpms.ExchangLogs
                       on m.MC_CardID equals c.MC_CardID
                       where (U_Telephone == ""?true:( m.MC_CardID == U_Telephone || m.MC_Mobile == U_Telephone))
                       select c;
            int zes = list.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            list = list.OrderBy(x => x.EL_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
            var info = new { total = zes, rows = list };
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        // GET: Jffx
        public ActionResult Jffx()
        {
            return View();
        }
        MPMSEntities mpms = new MPMSEntities();
        // 查询会员卡号是否存在，返回卡号的信息
        public ActionResult QueryVipIsNull(string cardNumber)
        {
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_CardID == cardNumber);
           
            if (mc==null)
            {
                return Json(new { pd = false }, JsonRequestBehavior.AllowGet);  
            }
            if (mc.MC_State != 1)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { pd = true, MC_ID = mc.MC_ID, MC_CardID = mc.MC_CardID, MC_Name = mc.MC_Name, MC_Point = mc.MC_Point, MC_TotalMoney = mc.MC_TotalMoney }, JsonRequestBehavior.AllowGet);


        }
        //快速消费
        public ActionResult DoJffx(int MC_ID, string MC_CardID, float CO_Recash, int CO_GavePoint)
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
                CO_OrderType = 2,
                MC_ID = MC_ID,
                MC_CardID = MC_CardID,
                EG_ID = 0,
                CO_TotalMoney = 0,
                CO_DiscountMoney =0,
                CO_GavePoint = CO_GavePoint,
                CO_Recash = CO_Recash,
                CO_Remark = "积分返现",
                CO_CreateTime = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"))
            };
            mpms.ConsumeOrders.Add(co);
            return Json(mpms.SaveChanges() > 1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DoJfDh(string gwc, int xf)
        {
            bool pd = true;
            using (TransactionScope transaction = new TransactionScope())//使用事务
            {
                try
                {
                    JavaScriptSerializer jaser = new JavaScriptSerializer();
                    List<ExchangLogs> loglist = jaser.Deserialize<List<ExchangLogs>>(gwc);
                    var mcid=Convert.ToInt32(loglist[0].MC_ID);
                    var Xfz = mpms.MemCards.FirstOrDefault(x => x.MC_ID == mcid) ;
                    Xfz.MC_Point -= xf;
                    for (int i = 0; i < loglist.Count; i++)
                    {
                        loglist[i].S_ID = (Session["Users"] as Users).S_ID;
                        loglist[i].U_ID = (Session["Users"] as Users).U_ID;
                        loglist[i].EL_CreateTime = DateTime.Now;
                        var Egid = Convert.ToInt32(loglist[i].EG_ID);
                        var gift = mpms.ExchangGifts.FirstOrDefault(x => x.EG_ID == Egid);
                        gift.EG_ExchangNum += loglist[i].EL_Number;
                        mpms.ExchangLogs.Add(loglist[i]);
                    }
                    mpms.SaveChanges();
                    transaction.Complete();//放在catch上面，否则不能回滚

                }
                catch
                {
                    pd = false;
                }

            }
            return Json(pd, JsonRequestBehavior.AllowGet);
        }
    }
}