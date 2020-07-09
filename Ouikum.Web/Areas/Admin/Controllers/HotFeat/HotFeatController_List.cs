using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Product;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Company;
using Prosoft.Service;
using System.Collections;
using System.Transactions;

namespace Ouikum.Web.Admin
{
    public partial class HotFeatController : BaseSecurityAdminController
    {
        #region Get: List
        public ActionResult Index()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsAdmin(13))
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            ViewBag.PStatus = "";
            ViewBag.EnumSearchByHotFeat = svCommon.SelectEnum(CommonService.EnumType.SearchByHotFeat);
            ViewBag.EnumHotFeatStatus = svCommon.SelectEnum(CommonService.EnumType.HotFeatStatus);
            ViewBag.EnumSearchByHotFeatStatus = svCommon.SelectEnum(CommonService.EnumType.SearchByHotFeatStatus);
            DoloadExpireHotFeat();
            #endregion

            return View();
        }
        #endregion

        #region Post: List
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            SetHotFeatPager(form);
            DoLoadComboBoxHotFeatStatus();
            List_DoloadData();
            return PartialView("UC/GridHotFeat");
        }
        #endregion

        #region Post: ProductList
        [HttpPost]
        public ActionResult ProductList(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            SetProductPager(form);
            List_DoloadDataProduct(ProductAction.FrontEnd);
            return PartialView("UC/GridProductList");
        }
        #endregion

        #region Post: ProductList
        [HttpPost]
        public JsonResult AddProductHotList(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);
            var svProduct = new ProductService();
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = "ProductID,ProductName,RowFlag,CompID,CompName";
            sqlWhere = "( IsDelete = 0 AND RowFlag IN (4) AND CompRowFlag IN (2,4) AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0 ) AND CompID = " + ViewBag.CompID;//svProduct.CreateWhereAction(action) + 

            sqlOrderBy = " CreatedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(0, "", (int)ViewBag.PStatus);
            sqlWhere += svProduct.CreateWhereSearchBy(ViewBag.TextSearch, ViewBag.SearchType);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            #endregion

            var data = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

            return Json(new { Suppliers = data, PageIndex = (int)ViewBag.PageIndex, TotalRow = svProduct.TotalRow, TotalPage = svProduct.TotalPage, PageSize = (int)ViewBag.PageSize, });
        }
        #endregion


        #region DeleteHotFeat
        public ActionResult DeleteHotFeat(int HotFeaProductID)
        {
            var svHotFeat = new HotFeaProductService();
            try
            {
                svHotFeat.DeleteHotFeat(HotFeaProductID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, MsgError = "" });
        }
        #endregion

        #region EditExpiredDate
        public ActionResult EditExpiredDate(List<int> HotFeaProductID, int NumMonth, int SendMailExpire,string EditStatus,string EditHotprice)
        {
            var svHotFeat = new HotFeaProductService();
            try
            {
                foreach (var item in HotFeaProductID)
                {
                    if (NumMonth != 0)
                    {
                        svHotFeat.EditExpiredDate(item, NumMonth);
                    }
                    svHotFeat.EditStatusHotprice(item ,EditStatus, EditHotprice);
                    // แจ้ง HotFeat ต่ออายุแล้วการใช้งาน
                    if (svHotFeat.IsResult && SendMailExpire == 1)
                    {
                        SendMailAlerts(item, 3, NumMonth);
                        //return Json(new { Result = true });
                    }
                }

                DoloadExpireHotFeat();
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, MsgError = "", Count_to_Exp = ViewBag.Count_to_Exp, Count_Exp_today = ViewBag.Count_Exp_today, Count_Exp = ViewBag.Count_Exp });
        }
         
        #endregion

        #region DeleteAll
        [HttpPost]
        public ActionResult DeleteAll(List<int> ID)
        {
            var svHotFeat = new HotFeaProductService();
            try
            {
                svHotFeat.DeleteHotFeatAll(ID, LogonCompCode);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, MsgError = "" });
        }
        #endregion

        #region SaveHotFeat
        [HttpPost]
        public ActionResult SaveHotFeat(List<int> CompID, List<int> Expire, List<int> ProductID, List<string> Type, List<string> HotPrice)
        {
            var svHotFeat = new HotFeaProductService();
            var Msg = string.Empty;
            try
            {
                svHotFeat.SaveHotFeat(CompID, Expire, ProductID, Type,HotPrice,LogonCompCode); 

                if (!string.IsNullOrEmpty(svHotFeat.Msg)) 
                    svHotFeat.IsResult = false;

                //อีเมล์แจ้งเพิ่ม HotFeat
                var data = svHotFeat.SelectData<b2bHotFeaProduct>("HotFeaProductID", "Isdelete = 0", "CreatedDate DESC", 1, CompID.Count());
                for (int i = 0; i < data.Count(); i++)
                {
                    SendMailAddHotFeat(data[i].HotFeaProductID, Expire[i]);
                }
            }
            catch (Exception ex)
            {
                Msg = res.Admin.lblCannot_save;
                svHotFeat.IsResult = false;
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, CountSuccess = svHotFeat.CountSuccess, CountProductExist = svHotFeat.Msg });
        }
        #endregion

        #region SaveProductHotFeat
        [HttpPost]
        public ActionResult SaveProductHotFeat(string ProductID, string HotFeaProductID)
        {
            var svHotFeat = new HotFeaProductService();
            try
            {
                svHotFeat.UpdateProductHotFeat(ProductID, HotFeaProductID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, MsgError = "" });
        }
        #endregion


        #region UpdateStatusHotFeat
        public ActionResult UpdateStatusHotFeat(int HotFeaProductID,string Status)
        {
            var svHotFeat = new HotFeaProductService();
            try
            {
                svHotFeat.UpdateStatusHotFeat(HotFeaProductID, Status);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svHotFeat.IsResult, MsgError = "" });
        }
        #endregion

        #region ExtendLifetime
        public ActionResult ExtendLifetime(List<bool> Check, int Value, List<int> ID, List<short> RowVersion)
        {
            var svHotFeat = new HotFeaProductService();

            var listID = new List<int>();

            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    listID.Add(ID[i]);
                }
            }
            svHotFeat.ExtendLifeTime(listID, Value);

            if (svHotFeat.IsResult)
            {
                #region Send Mail
                //for (int i = 0; i < Company.Count(); i++)
                //{
                //  //  SendMailAlerts(Company[i].MemberID, Company[i].Email, 3, Value);
                //} 
                #endregion
            }

                return Json(new { Result = svHotFeat.IsResult }); 
        }
        #endregion

        #region SendMailStatus
        public ActionResult SendMailStatus(List<bool> Check, int Status, List<int> HotFeaProductID, List<short> RowVersion)
        {
            CompanyService svCompany = new CompanyService();
            var HotFeaID = "";
            for (int i = 0; i < HotFeaProductID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    HotFeaID = HotFeaID + "HotFeaProductID = " + HotFeaProductID[i].ToString() + " or ";
                }
            }
            if (HotFeaID != "")
            {
                HotFeaID = HotFeaID.Substring(0, HotFeaID.Length - 4);
            }
            var Company = svCompany.SelectData<view_CompMemberHotFeat>("*", "IsDelete = 0 and (" + HotFeaID + ")", null, 0, 0);
            //sendmail
            if (Status == 1)//เมล์แจ้งเตือน HotFeat ใกล้หมดอายุ
            {
                for (int i = 0; i < Company.Count(); i++)
                {
                    SendMailAlerts(Company[i].HotFeaProductID, Status);
                }
                return Json(new { Result = true });
            }
            else if (Status == 2)//เมล์แจ้งเตือน HotFeat หมดอายุ
            {
                for (int i = 0; i < Company.Count(); i++)
                {
                    SendMailAlerts(Company[i].HotFeaProductID, Status);
                }
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }

        }
        #endregion

        #region SendMailAlerts
        [HttpPost]
        public JsonResult SendMailAlerts(int HotFeaProductID, int status, int LifeTime = 0)
        {
            MemberService svMember = new MemberService();
            var UserData = svMember.SelectData<view_CompMemberHotFeat>("HotFeaProductID,ExpiredDate,Status,CompID,CompName,MemberID,FirstName,Email,ProductID,ProductCode,ProductName", "Isdelete = 0 AND HotFeaProductID = " + HotFeaProductID).First();

            #region variable
            bool IsSend = true;
            var Detail = "";
            string Subject = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            var mailFrom = res.Config.EmailNoReply;
            Hashtable EmailDetail = new Hashtable();

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathGold = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/icon_Gold130.png";
            string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
            string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
            string pathGE = b2bthai_url + "/Content/Default/images/icon_goldExpiresmall.png";

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["CompName"] = UserData.CompName;
            EmailDetail["FirstName"] = UserData.FirstName;
            if (UserData.Status == "P")
            {
                EmailDetail["Status"] = "Premium Product";
            }
            else if (UserData.Status == "F")
            {
                EmailDetail["Status"] = "Feature Product";
            }
            else if (UserData.Status == "H")
            {
                EmailDetail["Status"] = "Hot Product";
            }
            
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathGold"] = pathGold;
            EmailDetail["pathF"] = pathF;
            EmailDetail["pathG"] = pathG;
            EmailDetail["pathGE"] = pathGE;
            #endregion

            if (status == 1)//NearExpireDate ใกล้หมดอายุ
            {
                #region Value For Send Email
                Subject = res.Email.SubjectUpgradeGoldNearExpire;
                TimeSpan time = Convert.ToDateTime(UserData.ExpiredDate) - Convert.ToDateTime(DateTime.Now);
                if (UserData.ExpiredDate != null)
                {
                    EmailDetail["ExpireDate"] = UserData.ExpiredDate.Value.ToString("dd/MM/yyyy");
                }

                // data for set msg detail
                EmailDetail["Time"] = " (" + time.Days.ToString() + " วัน)";
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendHotFeatNearExpire");

                mailTo.Add(UserData.Email);
                #endregion
            }
            else if (status == 2)//ExpireDate หมดอายุวันนี้
            {
                #region Value For Send Email
                Subject = res.Email.SubjectUpgradeGoldExpire;
                if (UserData.ExpiredDate != null)
                {
                    EmailDetail["ExpireDate"] = UserData.ExpiredDate.Value.ToString("dd/MM/yyyy");
                }
                // data for set msg detail
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendHotFeatExpire");

                mailTo.Add(UserData.Email);
                #endregion
            }
            else if (status == 3)//ExpireDate ต่ออายุการใช้งาน
            {
                #region Value For Send Email
                Subject = res.Email.SubjectUpgradePackage;
                EmailDetail["ProductCode"] = UserData.ProductCode;
                EmailDetail["ProductName"] = UserData.ProductName;
                EmailDetail["LifeTime"] = LifeTime + " เดือน";
                if (UserData.ExpiredDate != null)
                {
                    EmailDetail["ExpireDate"] = UserData.ExpiredDate.Value.ToString("dd/MM/yyyy");
                }
                // data for set msg detail
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendHotFeatRenew");

                mailTo.Add(UserData.Email);
                #endregion
            }

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            #endregion

            return Json(new { IsSend = IsSend });
        }
        #endregion

        #region SendMailAddHotFeat
        [HttpPost]
        public JsonResult SendMailAddHotFeat(int HotFeaProductID, int LifeTime = 0)
        {
            MemberService svMember = new MemberService();
            var UserData = svMember.SelectData<view_CompMemberHotFeat>("HotFeaProductID,ExpiredDate,Status,CompID,CompName,MemberID,FirstName,Email,ProductID,ProductCode,ProductName", "Isdelete = 0 AND HotFeaProductID = " + HotFeaProductID).First();

            #region variable
            bool IsSend = true;
            var Detail = "";
            string Subject = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            var mailFrom = res.Config.EmailNoReply;
            Hashtable EmailDetail = new Hashtable();

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathGold = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/icon_Gold130.png";
            string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
            string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
            string pathGE = b2bthai_url + "/Content/Default/images/icon_goldExpiresmall.png";

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["CompName"] = UserData.CompName;
            EmailDetail["FirstName"] = UserData.FirstName;
            if (UserData.Status == "P")
            {
                EmailDetail["Status"] = "Premium Product";
            }
            else if (UserData.Status == "F")
            {
                EmailDetail["Status"] = "Feature Product";
            }
            else if (UserData.Status == "H")
            {
                EmailDetail["Status"] = "Hot Product";
            }

            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathGold"] = pathGold;
            EmailDetail["pathF"] = pathF;
            EmailDetail["pathG"] = pathG;
            EmailDetail["pathGE"] = pathGE;
            #endregion
           
            #region Value For Send Email
            Subject = "สินค้าของคุณ ได้รับการโฆษณาผ่าน B2BThai.com";
            EmailDetail["ProductCode"] = UserData.ProductCode;
            EmailDetail["ProductName"] = UserData.ProductName;
            EmailDetail["LifeTime"] = LifeTime + " เดือน";
            if (UserData.ExpiredDate != null)
            {
                EmailDetail["ExpireDate"] = UserData.ExpiredDate.Value.ToString("dd/MM/yyyy");
            }
            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendHotFeat");

            mailTo.Add(UserData.Email);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            #endregion

            return Json(new { IsSend = IsSend });
        }
        #endregion

    }
}
