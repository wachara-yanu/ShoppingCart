using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//usin other
//using Prosoft.Base;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Category;
using Ouikum.Product;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Web.Models;

namespace Ouikum.Controllers
{
    public partial class StoreController : BaseController
    {
        // 
        // GET: /ProductController_List/

        string SqlSelect, SqlWhere, SqlOrderBy = "";

        #region GetList
        [HttpGet]
        public ActionResult index(int? Category, int? CateLevel, string CategoryName, int? ProductID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            if (res.Common.lblWebsite == "AppstoreThai" && Category == null && CateLevel == null)
            {
                Category = 7102;
                CateLevel = 1;
            }
            GetStatusUser();
            ProductCount();

            int CateID = (Category != null || Category == 0) ? (int)Category : 0;
            int CateLV = (CateLevel != null) ? (int)CateLevel : 0;

            #region Set Value
            string PAction = string.Empty;

            if (Request.Cookies["PAction"] != null)
            {
                if (Request.Cookies["PAction"].Value != "")
                {
                    PAction = Request.Cookies["PAction"].Value;
                    ViewBag.PageAction = PAction;
                }
                else
                {
                    PAction = "Gallery";
                    ViewBag.PageAction = "Gallery";
                }
            }
            else
            {
                PAction = "Gallery";
                ViewBag.PageAction = "Gallery";
            }
            #endregion

            #region query Product
            if (ViewBag.PageAction == "List")
            {
                SqlSelect = "ProductID,ProductName,ProductCount,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3,CategoryName";
            }
            else
            {
                SqlSelect = "ProductID,ProductName,ProductImgPath,ProductCount,CompID,CompName,CompLevel,CateLV3,CategoryName";
            }

            SqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            SqlWhere += svProduct.CreateWhereCause(0, "", 0, 0, CateLV, CateID, 0, 0, 0);

            var Products = svProduct.SelectData<view_SearchProduct>(SqlSelect, SqlWhere, "Complevel DESC, ModifiedDate DESC", 1, 50);
            ViewBag.Products = Products;

            //var mgKeyword = new KeywordMongo();
            //var mongoProducts = mgKeyword.SearchProductMongo(1, 1, 50, "", 0, CateLV, CateID, 0, 0);
            //var products = MappingProductMongo(mongoProducts);
            //ViewBag.Products = products;
            #endregion

            #region query Biztype
            var Biztype = svBizType.GetBiztypeAll();
            ViewBag.Biztype = Biztype;
            #endregion

            #region Category


            var ParentCate = svCategory.GetCategoryByLevel(1);
            ViewBag.Category = ParentCate;

            //Category Search
            ViewBag.IndrustryCateLV1 = svCategory.ListIndrustryCategory();
            ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory();
            #endregion

            SelectList_PageSize();
            if (CateID != 0)
            {
                SelectCateLV(CateID, CateLV);
                FindCateName(CateID, CateLV);
            }


            #region Set ViewBag
            if (PAction != "List")
            {
                ViewBag.PageAction = "Gallery";
            }
            if (CategoryName != null)
            {
                if (ProductID != null)
                {
                    var Title = svProduct.SelectData<view_Product>("*", "ProductID = " + ProductID, null, 1, 0, false).First();
                    if (Title.ProvinceName == "กรุงเทพมหานคร")
                    {
                        Title.ProvinceName = "กรุงเทพ";
                    }
                    ViewBag.Title = Title.ProductName + " | " + Title.CategoryName + " | " + Title.ProvinceName + " | " + res.Common.lblWebsiteStore;
                    if (!string.IsNullOrEmpty(Title.ProductKeyword))
                    {
                        ViewBag.MetaKeyword = Title.ProductKeyword.Replace('~', ',').Substring(0, Title.ProductKeyword.Length - 1) + Title.CategoryName + " | " + Title.ProvinceName + " | " + res.Common.lblWebsiteStore;
                    }
                    else
                    {
                        ViewBag.MetaKeyword = ViewBag.Title;
                    }
                    if (!string.IsNullOrEmpty(Title.ShortDescription))
                    {
                        ViewBag.MetaDescription = Title.ProductName + " | " + Title.CategoryName + " | " + Title.ShortDescription.Replace('~', ',').Substring(0, Title.ShortDescription.Length - 1) + " | " + Title.ProvinceName + " | " + res.Common.lblWebsiteStore;
                    }
                    else
                    {
                        ViewBag.MetaDescription = ViewBag.Title;
                    }
                }
                else
                {
                    ViewBag.Title = CategoryName + " | " + res.Common.lblWebsiteStore;
                    ViewBag.MetaKeyword = ViewBag.Title;
                    ViewBag.MetaDescription = ViewBag.Title;
                }
            }
            else
            {
                ViewBag.Title = res.Common.lblSearchProduct + " | " + res.Common.lblWebsiteStore;
                ViewBag.MetaKeyword = ViewBag.Title;
                ViewBag.MetaDescription = ViewBag.Title;
            }
            ViewBag.TextSearch = res.Product.lblProduct;
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 50;

            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
            //ViewBag.TotalRow = mgKeyword.TotalRow;
            //ViewBag.TotalPage = mgKeyword.TotalPage; 
            
            ViewBag.ProductID = ProductID;
            #endregion

            return View();

        }
        #endregion

        #region ProductCount
        public void ProductCount() {
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd) + " AND CategoryType IN (1,2)";
            ViewBag.ProductCount = svProduct.CountData<view_SearchProduct>(" * ", sqlWhere).ToString("#,##0");
        }
        #endregion

        #region PostList
        [HttpPost]
        public ActionResult List(string Layout, int? Sort, int PIndex, int PSize, string txtSearch, int? BizTypeID, int CateLevel, int CategoryID, int? CompLevelID)
        {

            #region Set Variable
            if (Layout == "List")
            {
                Response.Cookies["PAction"].Value = ViewBag.PageAction = "List";
            }
            else
            {
                Response.Cookies["PAction"].Value = ViewBag.PageAction = "Gallery";
            }
            #endregion

            //#region Select
            //if (Layout == "Gallery")
            //{
            //    SqlSelect = "ProductID,ProductName,ProductImgPath,ProductCount,CompID,CompName,CompLevel,CateLV3,CategoryName";
            //}
            //else
            //{
            //    SqlSelect = "ProductID,ProductName,ProductCount,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3";
            //}
            //#endregion

            //#region DoWhereCause
            //SqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            //SqlWhere += svProduct.CreateWhereCause(0, txtSearch, 0, 0, (int)CateLevel, (int)CategoryID, (int)CategoryID, (int)CompLevelID, 0);

            //#endregion

            //#region Sort By
            //switch (Sort)
            //{
            //    case 1:
            //        SqlOrderBy = svProduct.CreateOrderby(OrderBy.ModifiedDateDESC);
            //        break;
            //    case 2:
            //        SqlOrderBy = svProduct.CreateOrderby(OrderBy.CreatedDateDESC);
            //        break;
            //    case 3:
            //        SqlOrderBy = svProduct.CreateOrderby(OrderBy.ViewCountDESC);
            //        break;
            //}
            //#endregion

            //ViewBag.Products = svProduct.SelectData<view_SearchProduct>(SqlSelect, SqlWhere, "Complevel DESC," + SqlOrderBy, PIndex, PSize);
            var products = svProduct.SelectData<view_SearchProduct>(SqlSelect, SqlWhere, "Complevel DESC," + SqlOrderBy, PIndex, PSize);
            FindCateName(CategoryID, CateLevel);

            //var mgKeyword = new KeywordMongo();
            //var mongoProducts = mgKeyword.SearchProductMongo(Sort, PIndex, PSize, txtSearch, BizTypeID, CateLevel, CategoryID, CompLevelID, 0);
            //var products = MappingProductMongo(mongoProducts);
            ViewBag.Products = products;

            if (txtSearch != null)
            {
                if ((txtSearch != "") && (Convert.ToInt32(svProduct.TotalRow) > 0))
                {
                    ViewBag.Title = txtSearch;
                }
            }

            #region Set ViewBag
            ViewBag.TextSearch = txtSearch != "" ? txtSearch : res.Product.lblProduct;
            ViewBag.Title = txtSearch;
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.Status = svProduct.CodeError;

            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
            //ViewBag.TotalRow = mgKeyword.TotalRow;
            //ViewBag.TotalPage = mgKeyword.TotalPage; 
            
            #endregion

            #region Layout [List / Gallery]
            if (Layout == "Gallery")
                return PartialView("ProductGalleryUC");
            else
                return PartialView("ProductListUC");
            #endregion
        }
        #endregion

        #region Category Name
        private void FindCateName(int CategoryID, int CateLevel)
        {
            if (CategoryID != 0)
            {

                switch (CateLevel)
                {
                    #region case 0
                    case 0:
                        ViewBag.CateNameLV1 = "";
                        ViewBag.CateNameLV2 = "";
                        ViewBag.CateNameLV3 = "";
                        ViewBag.CateLV = 0;
                        break;
                    #endregion

                    #region case 1
                    case 1:
                        var CateNameLV1 = svCategory.SearchCategoryByID((int)CategoryID).First().CategoryName;
                        ViewBag.CateNameLV1 = CateNameLV1;
                        if (CateNameLV1.Length > 25)
                            ViewBag.btnCateName1 = CateNameLV1.Substring(0, 25) + "..";
                        else
                            ViewBag.btnCateName1 = CateNameLV1;
                        ViewBag.CateLV1 = CategoryID;
                        ViewBag.CateLV = 1;
                        break;
                    #endregion

                    #region case 2
                    case 2:

                        var CateNameLV2 = svCategory.SearchCategoryByID((int)CategoryID).First().CategoryName;
                        ViewBag.CateNameLV2 = CateNameLV2;
                        if (CateNameLV2.Length > 25)
                            ViewBag.btnCateName2 = CateNameLV2.Substring(0, 25) + "..";
                        else
                            ViewBag.btnCateName2 = CateNameLV2;
                        ViewBag.CateLV2 = CategoryID;
                        ViewBag.CateLV = 2;

                        var ParentCategoryID = svCategory.SearchCategoryByID((int)CategoryID).First().ParentCategoryID;
                        ViewBag.CateNameLV1 = svCategory.SearchCategoryByID((int)ParentCategoryID).First().CategoryName;
                        ViewBag.CateLV1 = ParentCategoryID;
                        break;
                    #endregion

                    #region case 3
                    case 3:

                        ViewBag.CateNameLV3 = svCategory.SearchCategoryByID((int)CategoryID).First().CategoryName;
                        ViewBag.CateLV3 = CategoryID;
                        ViewBag.CateLV = 3;

                        var ParentCategoryIDLV2 = svCategory.SearchCategoryByID((int)CategoryID).First().ParentCategoryID;
                        var CateName_LV2 = svCategory.SearchCategoryByID((int)ParentCategoryIDLV2).First().CategoryName;
                        ViewBag.CateNameLV2 = CateName_LV2;
                        if (CateName_LV2.Length > 25)
                            ViewBag.btnCateName2 = CateName_LV2.Substring(0, 25) + "..";
                        else
                            ViewBag.btnCateName2 = CateName_LV2;
                        ViewBag.CateLV2 = ParentCategoryIDLV2;

                        var ParentCategoryIDLV1 = svCategory.SearchCategoryByID((int)ParentCategoryIDLV2).First().ParentCategoryID;
                        ViewBag.CateNameLV1 = svCategory.SearchCategoryByID((int)ParentCategoryIDLV1).First().CategoryName;
                        ViewBag.CateLV1 = ParentCategoryIDLV1;
                        break;
                    #endregion
                }
            }
            else
            {
                ViewBag.CateName = "All";
                ViewBag.CateLV = 0;
            }
            ViewBag.CateID = CategoryID;
        }
        #endregion

        #region PostDetail
        [HttpPost]
        public ActionResult Detail(int? ProductID)
        { 
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            var Products = svProduct.SelectData<view_Product>("*", "ProductID = " + ProductID, null, 1, 0,false);
            ViewBag.ProductDetail = Products.First();

            int CompID = (int)Products.First().CompID;

            var ProductImages = svProductImage.SelectData<view_ProductImage>("*", "ProductID = " + ProductID, null, 1, 0, false);
            ViewBag.ProductImage = ProductImages;

            var Companies = svCompany.SelectData<view_Company>("*", "CompID = " + CompID, null, 1, 0, false);
            ViewBag.Company = Companies.First();

            var Shipments = svCompany.SelectData<b2bCompanyShipment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
            if (Shipments.Count > 0)
            ViewBag.CompanyShipment = Shipments;

            var Payments = svCompany.SelectData<view_CompanyPayment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
            if (Payments.Count > 0)
            ViewBag.CompanyPayments = Payments;


            // เพิ่ม ViewCount 
            AddViewCount((int)ProductID,"Product");

            return PartialView("DetailUC");
        }
        #endregion

        /* Category Menu*/
        #region SelectCateLV2-3
        [HttpPost]
        public ActionResult SelectCateLV(int? ID, int? CateLV)
        {
            SqlSelect = "CategoryID,CategoryName, ParentCategoryPath,CategoryLevel,ParentCategoryID,CategoryType";
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (2,3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (2,3)"; break;
            //    default: sqlwherein = ""; break;
            //}
            switch (CateLV)
            {
                case 0:
                    SqlWhere = "CategoryLevel = 1 AND ProductCount > 0 AND RowFlag = 1 AND IsDelete = 0";
                    break;

                case 1:
                    if (ID != 0) { SqlWhere = " ParentCategoryID = " + ID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0"; }
                    break;

                case 2:
                    if (ID != 0) { SqlWhere = " ParentCategoryID = " + ID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0"; }
                    break;

                case 3:
                    var ParentCategoryID = svCategory.SearchCategoryByID((int)ID).First().ParentCategoryID;
                    SqlWhere = "ParentCategoryID = " + ParentCategoryID + " AND ProductCount > 0 AND RowFlag = 1 AND IsDelete = 0";
                    break;
            }
            //SqlWhere += sqlwherein;
            #region query categorty
            var SqlSelect_List = "";
            //if (Base.AppLang == "en-US")
            //    SqlSelect_List = "CategoryID,CategoryNameEng AS CategoryName, ParentCategoryPath,CategoryLevel,CategoryType";
            //else

            SqlSelect_List = "CategoryID,CategoryName, ParentCategoryPath,CategoryLevel,CategoryType";
            SqlWhere = "CategoryLevel = 1 AND RowFlag  = 1 AND IsDelete = 0 AND ProductCount > 0 ";
            ViewBag.Category = svCategory.SelectData<b2bCategory>(SqlSelect_List, SqlWhere, "CategoryID");
            return PartialView("CategoryUC");
            #endregion
        }
        #endregion

        #region GetProductName
        [HttpPost]
        public ActionResult GetProductName(string query)
        {
            svProduct = new ProductService();

            SqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            SqlWhere += svProduct.CreateWhereCause(0, query, 0, 0, 0, 0, 0, 0, 0);

            var b2bProducts = svProduct.SelectData<view_Product>("ProductName", SqlWhere, "ProductName");
            var ProductName = b2bProducts.Select(it => it.ProductName).ToList();

            return Json(ProductName);

        }
        #endregion
    }
}

        