using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
using Ouikum.Company;
using Ouikum.Common;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Product;
using System.Collections;

namespace Ouikum.Web.Admin
{
    public partial class ApproveBuyleadController : BaseSecurityAdminController
    {
        CommonService svCommon = new CommonService();
        #region Get: Index
        public ActionResult Index()
        {    
            RememberURL();
            if (!CheckIsAdmin(12))
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            ViewBag.EnumBuylead = svCommon.SelectEnum(CommonService.EnumType.SearchByBuylaed);
            ViewBag.EnumProductStatus = svCommon.SelectEnum(CommonService.EnumType.ProductStatus); //ใช้อันเดียวกับ product
            #endregion

            return View();
        }
        #endregion

        #region Post: Index
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form); 
            SetBuyleadPager(form);
            List_DoloadData(BuyleadAction.Admin); 
            return PartialView("UC/GridApprove");
        }
        #endregion

        #region Approve
        [HttpPost]
        public ActionResult Approve(List<int> ID,List<int>CateLV1,List<int>CateLV2,List<int>CateLV3)
        {
            var svBuylead = new BuyleadService();
            try
            {
                svBuylead.ApproveBuylead(ID, CateLV1, CateLV2, CateLV3, LogonCompCode);
                if (svBuylead.IsResult)
                {
                    for (var i = 0; i < ID.Count(); i++)
                    {
                        var Buylead = svBuylead.SelectData<b2bBuylead>("BuyleadID, BuyleadCode, BuyleadName, BuyleadCompanyName, BuyleadContactPerson, BuyleadEmail, RowFlag, Remark", "BuyleadID = N'" + ID[i] + "'");
                        if (Buylead.Count > 0)
                        {
                            var model = Buylead.First();
                            SendEmailApproveBuylead(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = "" });
        }
        #endregion

        #region Reject
        [HttpPost]
        public ActionResult Reject(List<int> ID,List<int>CateLV1,List<int>CateLV2,List<int>CateLV3 , string Remark)
        {
            var svBuylead = new BuyleadService();
            try
            {
                svBuylead.RejectBuylead(ID, CateLV1, CateLV2, CateLV3, Remark, LogonCompCode);
                if (svBuylead.IsResult)
                {
                    for (var i = 0; i < ID.Count(); i++)
                    {
                        var Buylead = svBuylead.SelectData<b2bBuylead>("BuyleadID, BuyleadCode, BuyleadName, BuyleadCompanyName, BuyleadContactPerson, BuyleadEmail, RowFlag, Remark", "BuyleadID = N'" + ID[i] + "'");
                        if (Buylead.Count > 0)
                        {
                            var model = Buylead.First();
                            SendEmailApproveBuylead(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = "" });
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, List<int> CompID)
        {
            var svBuylead = new BuyleadService();
            var svCompany = new CompanyService();
            var compid = CompID;
            var products = svBuylead.SelectData<b2bBuylead>("BuyleadID,CompID", svBuylead.SQLWhereListInt(ID, "BuyleadID"));
            try
            {
                
                foreach (var item in products)
                {

                    svBuylead.Delete(ID, CateLV1, CateLV2, CateLV3, (int)item.CompID);
                    var imgManager = new FileHelper();
                    svCompany.UpdateBuyleadCount((int)item.CompID);
                    imgManager.DeleteFilesInDir("Buylead/" + item.CompID + "/" + item.BuyleadID);
                }
            }
            catch (Exception ex)
            {
                svBuylead.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError) });
        }
        #endregion

        #region SendEmailApproveBuylead
        public bool SendEmailApproveBuylead(b2bBuylead model)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var urlBuylead = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();

            var svHotFeat = new HotFeaProductService();
            var SQLSelect_Feat = "";

            SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
            var HotProduct = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 4);
            #endregion

            #region Set Content & Value For Send Email
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/MyB2B/buylead";
            urlBuylead = urlb2bthai + "/Purchase/Search/Detail/" + model.BuyleadID + "?name=" + model.BuyleadName;

            //test path logo
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            Hashtable EmailDetail = new Hashtable();
            EmailDetail["Name"] = model.BuyleadContactPerson;
            EmailDetail["CompName"] = model.BuyleadCompanyName;
            EmailDetail["BuyleadCode"] = model.BuyleadCode;
            EmailDetail["BuyleadName"] = model.BuyleadName;
            EmailDetail["Remark"] = model.Remark;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["url"] = url;
            EmailDetail["urlBuylead"] = urlBuylead;

            if (model.RowFlag == 4)
            {
                EmailDetail["Title"] = "แจ้งผลอนุมัติประกาศซื้อสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblApprove;
            }
            else if (model.RowFlag == 3)
            {
                EmailDetail["Title"] = "แจ้งผลไม่อนุมัติประกาศซื้อสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblReject;
            }

            ViewBag.Data = EmailDetail;
            ViewBag.HotProduct = HotProduct;

            string Subject = "แจ้งผลการตรวจสอบประกาศซื้อสินค้า B2BThai.com";

            Detail = PartialViewToString("UC/Email/SendApproveBuylead");

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(model.BuyleadEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            return IsSend;
        }
        #endregion
    }
}
