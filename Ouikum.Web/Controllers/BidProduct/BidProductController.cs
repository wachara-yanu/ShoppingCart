 using System;
using Ouikum.Category;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Product;
using Ouikum.Quotation;
using Ouikum.QuotationAttach;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
//using Prosoft.Base;
using System.Text.RegularExpressions;
namespace Ouikum.Web.Controllers
{
    public partial class BidProductController : BaseController
    {
        #region Member
        QuotationService svQuotation;
        CompanyService svCompany;
        ProductService svProduct;
        CategoryService svCategory;
        QuotationAttachService svQuotationAttach;
        CommonService svCommon = new Common.CommonService();
        #endregion

        string SQLWhere = "";

        #region Get BidProduct
        [HttpGet]
        public ActionResult List(string Code,string TextSearch)
        {
            GetStatusUser();
            RememberURL();
            if (!string.IsNullOrEmpty(Code))
            {
                ViewBag.QuotationCode = Code;
            }

            List_DoloadBidProduct(TextSearch, "Product",0, 1, 20);
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.EnumSortByProduct = svCommon.SelectEnum(CommonService.EnumType.SortByProduct);
            return View();
        }
        #endregion

        #region Post BidProduct
        [HttpPost]
        public ActionResult BidProduct(string TextSearch, string SearchType, int? CateLV, int? PIndex, int? PSize)
        {
            List_DoloadBidProduct(TextSearch, "Product", CateLV, PIndex, PSize);
            return PartialView("UC/QuotationUC/BidProductListUC");
        }
        #endregion

        #region List_DoloadBidProduct
        public void List_DoloadBidProduct(string TextSearch, string SearchType, int? CateLV, int? PIndex, int? PSize)
        {
            int PageIndex = (PIndex != null) ? (int)PIndex : 1;
            int PageSize = (PSize != null) ? (int)PSize : 20;
            string txtSearch = (TextSearch != null) ? TextSearch : "";
            string Type = "";
            string Cate = "";
            switch (SearchType)
            {
                case "Product":
                    Type = " AND ProductName LIKE N'" + txtSearch + "%'";
                    break;

                case "Buyer":
                    Type = " AND CompanyName LIKE N'" + txtSearch + "%'";
                    break;
            }
            var svQuotation = new QuotationService();
            var svCategory = new CategoryService();
            SQLWhere = svQuotation.CreateWhereAction(QuotationAction.FrontEnd, 0);
            SQLWhere += Type;
            if (CateLV > 0)
            {
                SQLWhere += " AND CateLV1 =" + CateLV;
            }
            //var sqlwherein="";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;

            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            var SqlSelect = "";

            //if (Base.AppLang == "en-US")
            //    SqlSelect = "CategoryID,CategoryNameEng AS CategoryName,ParentCategoryPath,CategoryLevel";

            //else

            SqlSelect = "CategoryID,CategoryName,ParentCategoryPath,CategoryLevel";
            var SqlWhereCate = "CategoryLevel = 1 AND RowFlag = 1 AND IsDelete = 0 ";
            ViewBag.Category = svCategory.SelectData<b2bCategory>(SqlSelect,SqlWhereCate,"CategoryName", 0,0);
           
            var QuotationList = svQuotation.SelectData<view_QuotationPublic>("*",SQLWhere,"CreatedDate DESC",PageIndex,PageSize);

            if (QuotationList.Count > 0)
            {
                ViewBag.Quotation = QuotationList;
            }
            ViewBag.TextSearch = TextSearch;
            ViewBag.PageIndex = PageIndex;
            ViewBag.TotalRow = svQuotation.TotalRow;
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.PageSize = PSize;

        }
        #endregion

        #region Detail
        [HttpGet]
        public ActionResult Detail(string Code)
        {

            GetStatusUser();

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            #region QuotDetail
            int CompID = 0;
            int ProductID = 0;
            var svQuotation = new QuotationService();
            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "IsDelete = 0 AND QuotationCode = '" + Code + "'", "CreatedDate", 1, 0);
            if (Quotation.Count > 0)
            {
                AddCountQuotation(Quotation[0].QuotationID, "Quotation");
                ViewBag.QuotDetail = Quotation.First();
                if (Quotation.First().FromCompID != 0)
                    CompID = Convert.ToInt32(Quotation.First().FromCompID);
                if (Quotation.First().ProductID != 0)
                    ProductID = Convert.ToInt32(Quotation.First().ProductID);

                #region ProductName
                svProduct = new ProductService();
                var Product = svProduct.SelectData<b2bProduct>("ProductID,ProductName,ProductKeyword,ProductImgPath,CompID", "ProductID = " + ProductID, "ProductName", 1, 0, false).First();
                if (Product.ProductName.Length >= 30)
                {
                    ViewBag.ProNameShort = Product.ProductName.Substring(0, 30) + "...";
                }
                else
                {
                    ViewBag.ProNameShort = Product.ProductName;
                }
                ViewBag.ProductName = Product.ProductName;
                ViewBag.KeyWord = " | " + Product.ProductKeyword + " | ";
                ViewBag.Product = Product;
                #endregion

                #region Company
                svCompany = new CompanyService();
                if (CompID != 0)
                {
                    ViewBag.Company = svCompany.SelectData<view_Company>("CompID,CompName,CompImgPath,BizTypeName,ProvinceName,ContactEmail", "CompID = " + CompID, null, 1, 0, false).First();
                }
                #endregion

                #region IsSend
                var IsSend = svQuotation.SelectData<b2bQuotation>("*", "QuotationCode = '" + Code + "' AND FromCompID = " + LogonCompID + " ");
                if (IsSend.Count > 0)
                {
                    ViewBag.IsSend = "1";
                }
                else
                {
                    ViewBag.IsSend = "0";
                }
                #endregion

                return View();
            }
            else {
                string url = res.Pageviews.PvNotFound;
                //if (Base.AppLang == "en-US")
                //{
                //    url = Regex.Replace(url, "~/", "~/en/");
                //}
                return Redirect(url);
            }
            #endregion

            
        }
        #endregion

        #region Update IsPublic
        public bool Update_IsPublic(string QuotationCode)
        {
            var svQuotation = new QuotationService();
            svQuotation.UpdateByCondition<b2bQuotation>("IsClosed = 1", "QuotationCode = '" + QuotationCode + "'");
            if (svQuotation.IsResult)
            {
                return svQuotation.IsResult;
            }
            else
            {
                return svQuotation.IsResult;
            }
        }
        #endregion

        /*---------User-Other-View-Quotation---------*/
        #region Get Reply
        [HttpGet]
        public ActionResult Reply(int? ID)
        {
            GetStatusUser();

            int CompID = 0;
            int ProductID = 0;
            int QuotationID = 0;

            /*------Quotation------*/
            #region QuotationReply
            var svQuotation = new QuotationService();
            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + ID, "CreatedDate", 1, 1).First();
            if (Quotation.FromCompID != 0)
                CompID = Convert.ToInt32(Quotation.FromCompID);
            if (Quotation.ProductID != 0)
                ProductID = Convert.ToInt32(Quotation.ProductID);
            QuotationID = DataManager.ConvertToInteger(Quotation.QuotationID);
            ViewBag.QuoDetail = Quotation;
            ViewBag.QouStatus = "Quotation";
            #endregion

            #region File Attach
            var svQuotationAttach = new QuotationAttachService();
            var AttachFile = svQuotationAttach.SelectData<b2bQuotationAttach>("*", "QuotationID = " + QuotationID, null, 0, 0, false);
            if (AttachFile.Count() > 0)
            {
                ViewBag.AttachFile = AttachFile.First();
                ViewBag.AttachRemark = AttachFile.First().Remark;
            }
            #endregion

            #region ProductName
            svProduct = new ProductService();
            var ProductName = svProduct.SelectData<b2bProduct>("ProductID,ProductName", "ProductID = " + ProductID, "ProductName", 1, 0, false).First().ProductName;
            if (ProductName.Length >= 35)
            {
                ViewBag.ProNameShort = ProductName.Substring(0, 35) + "...";
            }
            else
            {
                ViewBag.ProNameShort = ProductName;
            }
            ViewBag.ProductName = ProductName;
            #endregion

            #region Company
            /*----FromCompany----*/
            if (CompID != 0)
            {
                svCompany = new CompanyService();
                var Company = svCompany.SelectData<view_Company>("CompID,CompName,CompLevel,CompImgPath,BizTypeName,ProvinceName,CompPhone,ContactEmail", "CompID = " + CompID, null, 1, 0, false).First();
                ViewBag.Company = Company;
                ViewBag.CompName = Company.CompName;
                ViewBag.CompPhone = Company.CompPhone;
                ViewBag.CompEmail = Company.ContactEmail;
                if (!string.IsNullOrEmpty(Company.ContactFirstName))
                    ViewBag.ContactName = Company.ContactFirstName + " " + Company.ContactLastName;
                else
                    ViewBag.ContactName = "-";
            }
            else
            {
                ViewBag.CompName = Quotation.SaleCompany;
                if (!string.IsNullOrEmpty(Quotation.SalePhone))
                    ViewBag.CompPhone = Quotation.SalePhone;
                else
                    ViewBag.CompPhone = "-";
                ViewBag.CompEmail = Quotation.SaleEmail;
                ViewBag.ContactName = Quotation.SaleName;
            }
            /*ToCompany*/
            if (Quotation.ToCompID == 0)
            {
                svCompany = new CompanyService();
                int Quo_ID = Convert.ToInt16(ID) - 1;
                var ToComp = svQuotation.SelectData<b2bQuotation>("QuotationID,CompanyName", "QuotationID = " + ID, "CreatedDate", 1, 1).First();
                ViewBag.ToCompName = ToComp.CompanyName;
            }
            #endregion

            return View();
        }
        #endregion

    }
}
