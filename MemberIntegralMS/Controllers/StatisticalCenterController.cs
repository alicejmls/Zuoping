using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberIntegralMS.Controllers
{
    
        public class StatisticalCenterController : Controller
        {
            MPMSEntities mpmsent = new MPMSEntities();
            // GET: StatisticalCenter
            public ActionResult QuickConsumeSC()
            {
                mpmsent.Configuration.LazyLoadingEnabled = false;
                List<CardLevels> memberCard = mpmsent.CardLevels.ToList();
                ViewBag.LevelName = memberCard;
                return View();
            }

            public ActionResult text()
            {
                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             where c.CO_OrderType == 5
                             select new
                             {
                                 MC_Name = m.MC_Name,
                                 CO_DiscountMoney = c.CO_DiscountMoney,

                             };
                return Json(colist, JsonRequestBehavior.AllowGet);
            }
            public ActionResult text1()
            {
                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             where c.CO_OrderType == 3
                             select new
                             {
                                 MC_Name = m.MC_Name,
                                 CO_GavePoint = c.CO_GavePoint,
                             };
                return Json(colist, JsonRequestBehavior.AllowGet);
            }
            public ActionResult text2()
            {
            var cilist = mpmsent.CategoryItems.Where(x => x.C_Category == "CO_OrderType");

            var colist = from c in mpmsent.ConsumeOrders
                         join i in cilist
                         on c.CO_OrderType equals i.CI_ID
                         select new
                         {
                             CO_DiscountMoney = c.CO_DiscountMoney,
                             CI_Name = i.CI_Name
                         };
            return Json(colist, JsonRequestBehavior.AllowGet);
            }
            public ActionResult text3()
            {
                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             where c.CO_OrderType == 2
                             select new
                             {
                                 MC_Name = m.MC_Name,
                                 CO_GavePoint = c.CO_GavePoint,
                                 CO_Recash = c.CO_Recash

                             };
                return Json(colist, JsonRequestBehavior.AllowGet);
            }

            public ActionResult GiftsExchangeSC()
            {
                return View();
            }
            public ActionResult GetMemberTab()
            {
                mpmsent.Configuration.LazyLoadingEnabled = false;
                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var mlist = mpmsent.MemCards.ToList();

                var date = new
                {
                    total = mlist.Count(),
                    rows = mlist.OrderBy(x => x.MC_ID).Take(endIndex).Skip(startIndex)
                };
                return Json(date, JsonRequestBehavior.AllowGet);


            }

            public ActionResult GetMemberConsumeTab(string sTime, string eTime, string cardId, int? xtype)
            {
                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var cilist = mpmsent.CategoryItems.Where(x => x.C_Category == "CO_OrderType");

                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             join i in cilist
                             on c.CO_OrderType equals i.CI_ID
                             select new
                             {
                                 CO_ID = c.CO_ID,
                                 CO_OrderCode = c.CO_OrderCode,
                                 CO_OrderType = c.CO_OrderType,
                                 MC_CardID = c.MC_CardID,
                                 MC_Name = m.MC_Name,
                                 CO_DiscountMoney = c.CO_DiscountMoney,
                                 CO_GavePoint = c.CO_GavePoint,
                                 CO_CreateTime = c.CO_CreateTime,
                                 CL_ID = m.CL_ID,
                                 CI_Name = i.CI_Name


                             };
                var count = colist;



                if (!string.IsNullOrEmpty(sTime))
                {
                    var stime = Convert.ToDateTime(sTime);
                    count = colist.Where(x => x.CO_CreateTime >= stime);
                    if (!string.IsNullOrEmpty(eTime))
                    {
                        var etime = Convert.ToDateTime(eTime);
                        count = colist.Where(x => x.CO_CreateTime >= stime && x.CO_CreateTime <= etime);
                    }
                }
                else if (!string.IsNullOrEmpty(eTime))
                {
                    var etime = Convert.ToDateTime(eTime);
                    count = colist.Where(x => x.CO_CreateTime <= etime);
                }
                else if (!string.IsNullOrEmpty(cardId))
                {

                    count = colist.Where(x => x.MC_CardID == cardId);
                }
                else if (xtype != null)
                {

                    count = colist.Where(x => x.CO_OrderType == xtype);
                }

                var list = count.OrderBy(x => x.CO_ID).Take(endIndex).Skip(startIndex);
                var date = new
                {
                    total = count.Count(),
                    rows = list
                };
                return Json(date, JsonRequestBehavior.AllowGet);

            }

            public ActionResult GetQuickConsumeSCTab(string sTime, string eTime, string cardId, int? mDj, string oCode, int? xMoney, int? fh)
            {
                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             join l in mpmsent.CardLevels
                             on m.CL_ID equals l.CL_ID
                             where c.CO_OrderType == 5
                             select new
                             {
                                 CO_ID = c.CO_ID,
                                 CO_OrderCode = c.CO_OrderCode,
                                 MC_CardID = c.MC_CardID,
                                 MC_Name = m.MC_Name,
                                 CO_DiscountMoney = c.CO_DiscountMoney,
                                 CO_GavePoint = c.CO_GavePoint,
                                 CO_CreateTime = c.CO_CreateTime,
                                 CL_ID = m.CL_ID,
                                 CL_LevelName = l.CL_LevelName

                             };


                var qcount = colist;

                if (!string.IsNullOrEmpty(sTime))
                {
                    var stime = Convert.ToDateTime(sTime);
                    qcount = colist.Where(x => x.CO_CreateTime >= stime);
                    if (!string.IsNullOrEmpty(eTime))
                    {
                        var etime = Convert.ToDateTime(eTime);
                        qcount = colist.Where(x => x.CO_CreateTime >= stime && x.CO_CreateTime <= etime);
                    }
                }
                else if (!string.IsNullOrEmpty(eTime))
                {
                    var etime = Convert.ToDateTime(eTime);
                    qcount = colist.Where(x => x.CO_CreateTime <= etime);
                }
                else if (!string.IsNullOrEmpty(cardId))
                {

                    qcount = colist.Where(x => x.MC_CardID == cardId);
                }
                else if (mDj != null)
                {

                    qcount = colist.Where(x => x.CL_ID == mDj);
                }
                else if (!string.IsNullOrEmpty(oCode))
                {

                    qcount = colist.Where(x => x.CO_OrderCode == oCode);
                }
                else if (xMoney != null)
                {

                    qcount = colist.Where(x => x.CO_DiscountMoney >= xMoney);
                    if (fh == 1)
                    {
                        qcount = colist.Where(x => x.CO_DiscountMoney > xMoney);
                    }
                    else if (fh == 2)
                    {
                        qcount = colist.Where(x => x.CO_DiscountMoney == xMoney);
                    }
                }


                var qlist = qcount.OrderBy(x => x.CO_ID).Take(endIndex).Skip(startIndex);


                var date = new
                {
                    total = qcount.Count(),
                    rows = qlist
                };
                return Json(date, JsonRequestBehavior.AllowGet);
            }

            public ActionResult GetSintegralTab(string sTime, string eTime, string cardId, int? mDj, string oCode, int? xMoney, int? fh)
            {
                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             join l in mpmsent.CardLevels
                             on m.CL_ID equals l.CL_ID
                             where c.CO_OrderType == 3
                             select new
                             {
                                 CO_ID = c.CO_ID,
                                 CO_OrderCode = c.CO_OrderCode,
                                 MC_CardID = c.MC_CardID,
                                 MC_Name = m.MC_Name,
                                 CO_DiscountMoney = c.CO_DiscountMoney,
                                 CO_GavePoint = c.CO_GavePoint,
                                 CO_CreateTime = c.CO_CreateTime,
                                 CL_ID = m.CL_ID,
                                 CL_LevelName = l.CL_LevelName

                             };


                var qcount = colist;

                if (!string.IsNullOrEmpty(sTime))
                {
                    var stime = Convert.ToDateTime(sTime);
                    qcount = colist.Where(x => x.CO_CreateTime >= stime);
                    if (!string.IsNullOrEmpty(eTime))
                    {
                        var etime = Convert.ToDateTime(eTime);
                        qcount = colist.Where(x => x.CO_CreateTime >= stime && x.CO_CreateTime <= etime);
                    }
                }
                else if (!string.IsNullOrEmpty(eTime))
                {
                    var etime = Convert.ToDateTime(eTime);
                    qcount = colist.Where(x => x.CO_CreateTime <= etime);
                }
                else if (!string.IsNullOrEmpty(cardId))
                {

                    qcount = colist.Where(x => x.MC_CardID == cardId);
                }
                else if (mDj != null)
                {

                    qcount = colist.Where(x => x.CL_ID == mDj);
                }
                else if (!string.IsNullOrEmpty(oCode))
                {

                    qcount = colist.Where(x => x.CO_OrderCode == oCode);
                }
                else if (xMoney != null)
                {

                    qcount = colist.Where(x => x.CO_GavePoint >= xMoney);
                    if (fh == 1)
                    {
                        qcount = colist.Where(x => x.CO_GavePoint > xMoney);
                    }
                    else if (fh == 2)
                    {
                        qcount = colist.Where(x => x.CO_GavePoint == xMoney);
                    }
                }


                var qlist = qcount.OrderBy(x => x.CO_ID).Take(endIndex).Skip(startIndex);


                var date = new
                {
                    total = qcount.Count(),
                    rows = qlist
                };
                return Json(date, JsonRequestBehavior.AllowGet);
            }


            public ActionResult GetIntegralReturnTab(string sTime, string eTime, string cardId, int? mDj, string oCode, int? xMoney, int? fh)
            {
                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var colist = from c in mpmsent.ConsumeOrders
                             join m in mpmsent.MemCards
                             on c.MC_ID equals m.MC_ID
                             join l in mpmsent.CardLevels
                             on m.CL_ID equals l.CL_ID
                             where c.CO_OrderType == 2
                             select new
                             {
                                 CO_ID = c.CO_ID,
                                 CO_OrderCode = c.CO_OrderCode,
                                 MC_CardID = c.MC_CardID,
                                 MC_Name = m.MC_Name,
                                 CO_DiscountMoney = c.CO_DiscountMoney,
                                 CO_GavePoint = c.CO_GavePoint,
                                 CO_CreateTime = c.CO_CreateTime,
                                 CL_ID = m.CL_ID,
                                 CL_LevelName = l.CL_LevelName,
                                 CO_Recash = c.CO_Recash

                             };


                var qcount = colist;

                if (!string.IsNullOrEmpty(sTime))
                {
                    var stime = Convert.ToDateTime(sTime);
                    qcount = colist.Where(x => x.CO_CreateTime >= stime);
                    if (!string.IsNullOrEmpty(eTime))
                    {
                        var etime = Convert.ToDateTime(eTime);
                        qcount = colist.Where(x => x.CO_CreateTime >= stime && x.CO_CreateTime <= etime);
                    }
                }
                else if (!string.IsNullOrEmpty(eTime))
                {
                    var etime = Convert.ToDateTime(eTime);
                    qcount = colist.Where(x => x.CO_CreateTime <= etime);
                }
                else if (!string.IsNullOrEmpty(cardId))
                {

                    qcount = colist.Where(x => x.MC_CardID == cardId);
                }
                else if (mDj != null)
                {

                    qcount = colist.Where(x => x.CL_ID == mDj);
                }
                else if (!string.IsNullOrEmpty(oCode))
                {

                    qcount = colist.Where(x => x.CO_OrderCode == oCode);
                }
                else if (xMoney != null)
                {

                    qcount = colist.Where(x => x.CO_DiscountMoney >= xMoney);
                    if (fh == 1)
                    {
                        qcount = colist.Where(x => x.CO_DiscountMoney > xMoney);
                    }
                    else if (fh == 2)
                    {
                        qcount = colist.Where(x => x.CO_DiscountMoney == xMoney);
                    }
                }


                var qlist = qcount.OrderBy(x => x.CO_ID).Take(endIndex).Skip(startIndex);


                var date = new
                {
                    total = qcount.Count(),
                    rows = qlist
                };
                return Json(date, JsonRequestBehavior.AllowGet);
            }


            public ActionResult GetgiftsExchangeTab(string sTime, string eTime, string cardId)
            {

                int pageIndex = Convert.ToInt32(Request["page"]);  //当前页  
                int pageSize = Convert.ToInt32(Request["rows"]);  //页面行数  
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize;

                var glist = from g in mpmsent.ExchangLogs
                            select new
                            {
                                EL_ID = g.EL_ID,
                                MC_CardID = g.MC_CardID,
                                MC_Name = g.MC_Name,
                                EG_ID = g.EG_ID,
                                EG_GiftName = g.EG_GiftName,
                                EL_Point = g.EL_Point,
                                EL_Number = g.EL_Number,
                                EL_CreateTime = g.EL_CreateTime
                            };

                var gcount = glist;

                if (!string.IsNullOrEmpty(sTime))
                {
                    var stime = Convert.ToDateTime(sTime);
                    gcount = glist.Where(x => x.EL_CreateTime >= stime);
                    if (!string.IsNullOrEmpty(eTime))
                    {
                        var etime = Convert.ToDateTime(eTime);
                        gcount = glist.Where(x => x.EL_CreateTime >= stime && x.EL_CreateTime <= etime);
                    }
                }
                else if (!string.IsNullOrEmpty(eTime))
                {
                    var etime = Convert.ToDateTime(eTime);
                    gcount = glist.Where(x => x.EL_CreateTime <= etime);
                }
                else if (!string.IsNullOrEmpty(cardId))
                {

                    gcount = glist.Where(x => x.MC_CardID == cardId);
                }


                var list = gcount.OrderBy(x => x.EL_ID).Take(endIndex).Skip(startIndex);

                var date = new
                {
                    total = gcount.Count(),
                    rows = list
                };
                return Json(date, JsonRequestBehavior.AllowGet);
            }

            public ActionResult IntegralReturnSC()
            {
                mpmsent.Configuration.LazyLoadingEnabled = false;
                List<CardLevels> memberCard = mpmsent.CardLevels.ToList();
                ViewBag.LevelName = memberCard;
                return View();
            }

            public ActionResult MemberConsumeSC()
            {
                mpmsent.Configuration.LazyLoadingEnabled = false;
                var cilist = mpmsent.CategoryItems.Where(x => x.C_Category == "CO_OrderType");
                ViewBag.LevelName = cilist;
                return View();
            }

            public ActionResult MemberSIntegralSC()
            {
                mpmsent.Configuration.LazyLoadingEnabled = false;
                List<CardLevels> memberCard = mpmsent.CardLevels.ToList();
                ViewBag.LevelName = memberCard;
                return View();
            }
        }
    }
