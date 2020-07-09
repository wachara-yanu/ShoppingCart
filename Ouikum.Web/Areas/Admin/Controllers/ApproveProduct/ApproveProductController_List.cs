using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.Common;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using System.Collections;

namespace Ouikum.Web.Admin
{
    public partial class ApproveProductController : BaseSecurityAdminController
    {

        #region Get: Index
        public ActionResult Index()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsAdmin(12))
                if(!CheckIsAdmin(15))
                    return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            ViewBag.EnumSearchByProduct = svCommon.SelectEnum(CommonService.EnumType.SearchByProduct);
            ViewBag.EnumProductStatus = svCommon.SelectEnum(CommonService.EnumType.ProductStatus);
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
            SetProductPager(form);
            List_DoloadData(ProductAction.Admin);
            DoLoadComboBoxProductStatus();
            return PartialView("UC/GridApprove");
        }
        #endregion

        #region Approve
        [HttpPost]
        public ActionResult Approve(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, List<int> CompID)
        {
            var svProduct = new ProductService();
            try
            {
                svProduct.ApproveProduct(ID, CateLV1, CateLV2, CateLV3, LogonCompCode, CompID);
                if (svProduct.IsResult)
                {
                    for (var i = 0; i < ID.Count(); i++)
                    {
                        var Product = svProduct.SelectData<view_Product>("ProductID, ProductCode, ProductName, CompName, ContactFirstName, ContactLastName, ContactEmail, RowFlag, Remark", "ProductID = N'" + ID[i] + "'");
                        if (Product.Count > 0)
                        {
                            var model = Product.First();
                            SendEmailApproveProduct(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = "" });
        }
        #endregion

        #region Reject
        [HttpPost]
        public ActionResult Reject(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, List<int> CompID, string Remark)
        {
            var svProduct = new ProductService();
            try
            {
                svProduct.RejectProduct(ID, CateLV1, CateLV2, CateLV3, Remark, LogonCompCode, CompID);
                if (svProduct.IsResult)
                {
                    for (var i = 0; i < ID.Count(); i++)
                    {
                        var Product = svProduct.SelectData<view_Product>("ProductID, ProductCode, ProductName, CompName, ContactFirstName, ContactLastName, ContactEmail, RowFlag, Remark", "ProductID = N'" + ID[i] + "'");
                        if (Product.Count > 0)
                        {
                            var model = Product.First();
                            SendEmailApproveProduct(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = "" });
        }
        #endregion

        #region Post: PreviewAdmin
        [HttpPost]
        public ActionResult PreviewAdmin(string Code)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                var svCompany = new CompanyService();
                var Company = svCompany.SelectData<view_Company>(" * ", " CompCode = N'" + Code + "'");
                if (Company != null)
                {
                    ViewBag.Company = (view_Company)Company.First();
                    ServiceTypeAdmin();
                }
            }

            return PartialView("UC/PreviewAdmin");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, List<int> CompID)
        {
            var svProduct = new ProductService();
            var products = svProduct.SelectData<b2bProduct>("ProductID,CompID", svProduct.SQLWhereListInt(ID, "ProductID"));
            try
            {
                var i = 0;
                foreach (var item in products)
                {
                    svProduct.Delete((int)item.ProductID, CateLV1[i], CateLV2[i], CateLV3[i], (int)item.CompID);

                    var imgManager = new FileHelper();
                    imgManager.DeleteFilesInDir("Product/" + item.CompID + "/" + item.ProductID);
                    i++;
                }
            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

        #region SendEmailApproveProduct
        public bool SendEmailApproveProduct(view_Product model)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var urlProduct = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            #endregion

            #region Set Content & Value For Send Email
            var Buylead = new List<view_BuyLead>();
            var svProduct = new ProductService();
            Buylead = svProduct.SelectData<view_BuyLead>("TOP 3 BuyleadID, BuyleadName, CompID, BuyleadIMGPath, Qty, QtyUnit", " (IsDelete = 0 AND RowFlag IN (4)) AND ( IsShow = 1 AND IsJunk = 0 ) ORDER BY NEWID()", "", 1, 0, false);
            ViewBag.Buyleads = Buylead;
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/MyB2B/product/Index";
            urlProduct = urlb2bthai + "/Search/Product/Detail/" + model.ProductID + "?name=" + model.ProductName;

            //test path logo
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            Hashtable EmailDetail = new Hashtable();
            EmailDetail["Name"] = model.ContactFirstName + " " + model.ContactLastName;
            EmailDetail["CompName"] = model.CompName;
            EmailDetail["ProductCode"] = model.ProductCode;
            EmailDetail["ProductName"] = model.ProductName;
            EmailDetail["Remark"] = model.Remark;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["url"] = url;
            EmailDetail["urlProduct"] = urlProduct;

            if (model.RowFlag == 4)
            {
                EmailDetail["Title"] = "แจ้งผลอนุมัติประกาศขายสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblApprove;
            }
            else if (model.RowFlag == 3)
            {
                EmailDetail["Title"] = "แจ้งผลไม่อนุมัติประกาศขายสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblReject;
            }

            ViewBag.Data = EmailDetail;

            string Subject = "แจ้งผลการตรวจสอบประกาศขายสินค้า B2BThai.com";

            Detail = PartialViewToString("UC/Email/SendApproveProduct");

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(model.ContactEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            return IsSend;
        }
        #endregion
    }
}
