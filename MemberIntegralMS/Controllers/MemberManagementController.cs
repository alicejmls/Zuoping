using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.IO;

using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity;

namespace MemberIntegralMS.Controllers
{
    public class MemberManagementController : Controller
    {
        // GET: MemberManagementPage
        public ActionResult MemberManagementPage()
        {
            return View();
        }


        MPMSEntities mpms = new MPMSEntities();

        // 绑定会员等级
        public ActionResult GetVipGrade()
        {
            var r = from c in mpms.CardLevels
                    select new
                    {
                        CL_ID = c.CL_ID,
                        CL_LevelName = c.CL_LevelName
                    };

            return Json(r, JsonRequestBehavior.AllowGet);
        }


        // 绑定会员卡状态
        public ActionResult GetState()
        {
            var r = from c in mpms.CategoryItems
                    where c.C_Category == "MC_State"
                    select new
                    {
                        MC_State = c.CI_ID,
                        CI_Name = c.CI_Name
                    };

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        // 查询会员卡号是否重复
        public ActionResult QueryVipCardNumber(int MC_ID, string cardNumber)
        {
            if (MC_ID == 0)
            {//添加
                if (mpms.MemCards.FirstOrDefault(x => x.MC_CardID == cardNumber) != null)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //修改
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
            if (cardNumber != mc.MC_CardID && mpms.MemCards.FirstOrDefault(x => x.MC_ID != MC_ID && x.MC_CardID == cardNumber) != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        // 查询
        public ActionResult QueryMemberManagement(string MC_CardID, string MC_Name, string MC_Mobile, int CL_ID, int MC_State,bool DcExcel)
        {
            var info = from c in mpms.CardLevels
                       join m in mpms.MemCards on c.CL_ID equals m.CL_ID
                       join i in
                       (from ct in mpms.CategoryItems where ct.C_Category == "MC_State" select new { CI_ID = ct.CI_ID, CI_Name = ct.CI_Name })
                       on m.MC_State equals i.CI_ID
                       select new
                       {
                           MC_ID = m.MC_ID,
                           MC_CardID = m.MC_CardID,
                           MC_Name = m.MC_Name,
                           MC_Mobile = m.MC_Mobile,
                           MC_Password = m.MC_Password,
                           MC_Birthday_Month = m.MC_Birthday_Month,
                           MC_Birthday_Day = m.MC_Birthday_Day,
                           MC_BirthdayType = m.MC_BirthdayType,
                           MC_IsPast = m.MC_IsPast,
                           MC_PastTime = m.MC_PastTime,
                           MC_Point = m.MC_Point,
                           MC_Money = m.MC_Money,
                           MC_State = m.MC_State,
                           MC_RefererCard = m.MC_RefererCard,
                           MC_RefererName = m.MC_RefererName,
                           MC_Sex = m.MC_Sex,
                           MC_IsPointAuto = m.MC_IsPointAuto,
                           CL_ID = m.CL_ID,
                           CL_LevelName = c.CL_LevelName,
                           CI_Name = i.CI_Name,
                           MC_CreateTime = m.MC_CreateTime,
                           MC_TotalMoney = m.MC_TotalMoney,

                       };

            
            
            info = MC_CardID == "" ? info : info.Where(x => x.MC_CardID.Contains(MC_CardID));
            info = MC_Name == "" ? info : info.Where(x => x.MC_Name.Contains(MC_Name));
            info = MC_Mobile == "" ? info : info.Where(x => x.MC_Mobile.Contains(MC_Mobile));
            info = CL_ID == 0 ? info : info.Where(x => x.CL_ID == CL_ID);
            info = MC_State == 0 ? info : info.Where(x => x.MC_State == MC_State);

            if (DcExcel)     //导出Excel
            {
                int r = Convert.ToInt32(Session["rows"]);
                int p = Convert.ToInt32(Session["page"]);
                var list = info.OrderBy(x => x.MC_ID).Take(r * p).Skip(r * (p - 1));
                //表格的顶层															
                Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                //创建Excel工作薄															
                Excel.Workbook workbook = excel.Workbooks.Add(Server.MapPath("~/ExcelTemplate/会员列表模板.xlsx"));
                //引用Excel工作簿															
                excel.Visible = true;
                //创建Excel工作表															
                Excel.Worksheet sheet = workbook.Worksheets[1] as Excel.Worksheet;
                //创建Excel工作表名称															
                sheet.Name = "会员管理";
                DateTime time = DateTime.Now;
                sheet.Cells[1, 2] = time.ToShortDateString() + "_会员列表";
                //标头单位信息															
                sheet.Cells[3, 3] = list.Count();
                sheet.Cells[3, 5] = list.Where(e => e.MC_Sex == 1).Count();
                sheet.Cells[3, 7] = list.Where(e => e.MC_Sex == 0).Count();
                int i = 6;
                //遍历泛型集合中的所有记录，把数据填充到单元格里面去								
                foreach (var item in list)
                {
                    sheet.Cells[i, 2] = item.MC_CardID;
                    sheet.Cells[i, 3] = item.MC_Name;
                    sheet.Cells[i, 4] = (item.MC_Sex == 1) ? "男" : "女";
                    sheet.Cells[i, 5] = item.MC_Mobile;
                    sheet.Cells[i, 6] = item.MC_CreateTime.ToString();
                    sheet.Cells[i, 7] = item.CL_LevelName;
                    i += 1;
                }


                string path = "~/ExcelTemplate/会员列表_" + time.ToString("yyyy-MM-dd_hhmmss") + ".xlsx";
                //先把文件保存到网站的Excel文件夹里面，然后再下载到客户端来															
                workbook.SaveAs(Server.MapPath(path), Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                workbook.Close(Missing.Value, Missing.Value, Missing.Value);

                excel.Quit();
                //保存好后实现文件的下载															
                FileInfo loadFile = new FileInfo(Server.MapPath(path));
                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = false;
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(loadFile.Name, System.Text.Encoding.UTF8));
                Response.AppendHeader("Content-Length", loadFile.Length.ToString());
                Response.WriteFile(loadFile.FullName);
                Response.Flush();

                //下载完文件后将excel文件夹中的临时文件删除						
                System.IO.File.Delete(Server.MapPath(path));
                //释放进程						
                GC.Collect();

                return RedirectToAction("MemberManagementPage", "MemberManagement");
            }

            int pageCount = info.Count();
            int pagesize = Convert.ToInt32(Request["rows"]);
            int page = Convert.ToInt32(Request["page"]);
            Session["rows"] = pagesize;
            Session["page"] = page;
            info = info.OrderBy(x => x.MC_ID).Take(pagesize * page).Skip(pagesize * (page - 1));
           
            var pages = new { total = pageCount, rows = info };
            return Json(pages, JsonRequestBehavior.AllowGet);
        }



        // 会员转账页面
        public ActionResult MemberTransferPage()
        {
            return View();
        }

        // 查询会员卡号是否存在，返回卡号的信息
        public ActionResult QueryVipIsNull(string cardNumber)
        {
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_CardID == cardNumber);
            if (mc != null)
            {
                return Json(new { pd = true, MC_ID = mc.MC_ID, MC_CardID = mc.MC_CardID, MC_Name = mc.MC_Name, MC_Point = mc.MC_Point, MC_TotalMoney = mc.MC_TotalMoney }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { pd = false }, JsonRequestBehavior.AllowGet);

        }


        // 会员转账页面
        public ActionResult MemberTransfer(int RollMC_ID, string RollCardID, float RollMC_TotalMoney, int ShiftMC_ID, string ShiftCardID, decimal TL_TransferMoney, string TL_Remark, DateTime TL_CreateTime)
        {
            var jf = Convert.ToInt32(RollMC_TotalMoney);

            using (TransactionScope transaction = new TransactionScope())//使用事务
            {
                try
                {
                    //修改转出的会员信息
                    MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == RollMC_ID);
                    mc.MC_Point -= jf;
                    if (mc.MC_Point < 0)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    mc.MC_TotalMoney += RollMC_TotalMoney;
                    mc.MC_TotalCount += 1;

                    mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;


                    //在转账记录表插入一条记录
                    TransferLogs tr = new TransferLogs
                    {
                        S_ID = (Session["Users"] as Users).S_ID,
                        U_ID = (Session["Users"] as Users).U_ID,
                        TL_FromMC_ID = RollMC_ID,
                        TL_FromMC_CardID = RollCardID,
                        TL_ToMC_ID = ShiftMC_ID,
                        TL_ToMC_CardID = ShiftCardID,
                        TL_TransferMoney = TL_TransferMoney,
                        TL_Remark = TL_Remark,
                        TL_CreateTime = TL_CreateTime
                    };
                    mpms.TransferLogs.Add(tr);

                    //修改转入的会员信息
                    MemCards zr = mpms.MemCards.FirstOrDefault(x => x.MC_ID == ShiftMC_ID);
                    zr.MC_Point += jf;

                    mpms.Entry<MemCards>(zr).State = System.Data.Entity.EntityState.Modified;

                    transaction.Complete();//放在catch上面，否则不能回滚

                }
                catch
                {

                }
            }

            return Json(mpms.SaveChanges() > 2, JsonRequestBehavior.AllowGet);
        }

        // 会员换卡页面
        public ActionResult ChangCardPage()
        {
            return View();
        }

        // 会员换卡
        public ActionResult ChangCard(int MC_ID, string MC_CardID, string MC_Password)
        { //修改
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);

            mc.MC_CardID = MC_CardID;
            mc.MC_Password = MC_Password;

            mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;

            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }


        // 查询老会员密码是否存在
        public ActionResult QueryVipPwd(string MC_Password)
        {
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_Password == MC_Password);
            if (mc == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // 挂失/锁定页面
        public ActionResult MemberReportPage()
        {
            return View();
        }


        // 挂失/锁定
        public ActionResult MemberReport(int MC_ID, int MC_State)
        {
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
            mc.MC_State = MC_State;
            mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }


        // 修改和添加页面
        public ActionResult MemberAddOrUpdatePage()
        {
            return View(Session["Users"] as Users);
        }

        // 添加修改方法
        public ActionResult MemberAddOrUpdate(MemCards sj)
        {
            if (sj.MC_ID == 0)
            {//添加
                mpms.MemCards.Add(sj);
                return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
            }
            //修改
            MemCards mc = mpms.MemCards.FirstOrDefault(x => x.MC_ID == sj.MC_ID);

            mc.MC_CardID = sj.MC_CardID;
            mc.MC_Name = sj.MC_Name;
            mc.MC_Mobile = sj.MC_Mobile;
            mc.MC_Password = sj.MC_Password;
            mc.MC_Birthday_Month = sj.MC_Birthday_Month;
            mc.MC_Birthday_Day = sj.MC_Birthday_Day;
            mc.MC_BirthdayType = sj.MC_BirthdayType;
            mc.MC_IsPast = sj.MC_IsPast;
            mc.MC_PastTime = sj.MC_PastTime;
            mc.MC_Point = sj.MC_Point;
            mc.MC_Money = sj.MC_Money;
            mc.MC_State = sj.MC_State;
            mc.MC_RefererCard = sj.MC_RefererCard;
            mc.MC_RefererName = sj.MC_RefererName;
            mc.MC_Sex = sj.MC_Sex;
            mc.MC_IsPointAuto = sj.MC_IsPointAuto;
            mc.CL_ID = sj.CL_ID;

            mpms.Entry<MemCards>(mc).State = System.Data.Entity.EntityState.Modified;
            //修改
            return Json(mpms.SaveChanges() > 0, JsonRequestBehavior.AllowGet);
        }

        //删除
        public ActionResult MemberDelete(int MC_ID)
        {
            var r = mpms.MemCards.FirstOrDefault(x => x.MC_ID == MC_ID);
            mpms.MemCards.Remove(r as MemCards);
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