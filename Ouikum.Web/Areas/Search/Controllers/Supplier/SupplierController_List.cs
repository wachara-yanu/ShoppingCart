using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Company;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;
//using Prosoft.Base;
namespace Ouikum.Web.Search
{
    public partial class SupplierController : BaseController
    {
        string sqlSelect, sqlWhere, sqlOrderBy = "";
        #region Index
        [HttpGet]
        public ActionResult Index(int? Category, int? CateLevel, string CategoryName, string TextSearch)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            if (res.Common.lblWebsite == "AppstoreThai" && Category == null && CateLevel == null)
            {
                Category = 7102;
                CateLevel = 1;
            }
            CommonService svCommon = new CommonService();
            GetStatusUser();

            int cateid = (Category != null || Category == 0) ? (int)Category : 0;
            int catelv = (CateLevel != null) ? (int)CateLevel : 0;
            string txtSearch = (TextSearch != null) ? TextSearch : "";
            if (txtSearch == "")
            {
                ViewBag.TextSearch = "";
                if (cateid == 0)
                {
                    ViewBag.Title = MvcHtmlString.Create(res.Common.lblSearchSupplier + " | " + res.Common.lblDomainShortName);
                }
            }
            else
            {
                ViewBag.TextSearch = txtSearch;
                ViewBag.Title = MvcHtmlString.Create(ViewBag.TextSearch + " | " + res.Common.lblDomainShortName);
            }
            DoloadSearchSupplier(1, 1, 20, txtSearch, 0, catelv, cateid, 0, 0);

            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}

            #region Category
            //if (Base.AppLang == "en-US")
            //    sqlSelect = "CategoryID,CategoryNameEng As CategoryName, ParentCategoryPath,CategoryLevel,CategoryType";
            //else

            sqlSelect = "CategoryID,CategoryName, ParentCategoryPath,CategoryLevel,CategoryType";
            sqlWhere = "CategoryLevel = 1 AND RowFlag  = 1 AND IsDelete = 0 AND ProductCount > 0 ";
            var ParentCate = svCategory.SelectData<b2bCategory>(sqlSelect, sqlWhere, "CategoryName");
            ViewBag.Category = ParentCate;
            #endregion

            #region query Biztype
            var SQlSelect_Biz = "";
            //if (Base.AppLang == "en-US")
            //    SQlSelect_Biz = "BizTypeID,BizTypeCode As BizTypeName";
            //else

            SQlSelect_Biz = "BizTypeID,BizTypeName,BizTypeCode";
            var Biztype = svBizType.SelectData<b2bBusinessType>(SQlSelect_Biz,"Rowflag > 0","BizTypeID");
            ViewBag.Biztype = Biztype;
            #endregion

            #region query Province
            LoadProvinces();
            //ViewBag.Province = svAddress.SelectData<emProvince>("*", "Rowflag = 1 AND IsDelete = 0", "RegionID");
            #endregion

            SelectList_PageSize();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.EnumSortBySupplier = svCommon.SelectEnum(CommonService.EnumType.SortBySupplier);
            FindCateName((int)ViewBag.CateID, (int)ViewBag.CateLevel);
            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription = ViewBag.Title;
            return View();
        }
        #endregion

        #region DoloadSearchSupplier

        public void DoloadSearchSupplier(int? Sort, int? PIndex, int? PSize, string txtSearch, int? BizTypeID, int? CateLevel, int? CategoryID, int? CompLevelID, int? ProvinceID)
        {
            if (CategoryID != null)

            #region set ViewBag For Search SupplierCompID
            ViewBag.CompID = 0;
            ViewBag.PStatus = 0;
            ViewBag.GroupID = 0;

            var cateid = (int)CategoryID;
            var catelevel = (int)CateLevel;
            ViewBag.CateID = cateid > 0 ? cateid : 0; 
            ViewBag.CateLevel = catelevel > 0 ? catelevel : 0;
            ViewBag.BizTypeID = 0;
            ViewBag.CompLevel = 0;
            ViewBag.CompProvinceID = 0;
            #endregion
            sqlSelect = "CompID,CompName,CompCode,CompShortDes,CompLevel,BizTypeName,BizTypeOther,CompImgPath,LogoImgPath,ProvinceName,CompPhone,ContactPhone,FactoryPhone,ContactEmail,ProductCount,ViewCount,IsOnline,ModifiedDate,CreatedDate";
            #region DoWhereCause
            sqlWhere = "Webid = 1 AND ";
            sqlWhere += svCompany.CreateWhereAction(CompStatus.HaveProduct, 0);
            sqlWhere += svCompany.CreateWhereCause(0, "", null, (int)CompLevelID, (int)BizTypeID, 0, (int)ProvinceID);
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND WebID = 1 "; break;
            //    case "AntCart": sqlwherein = " AND WebID = 3 "; break;
            //    case "myOtopThai": sqlwherein = " AND WebID = 5 "; break;
            //    case "AppstoreThai": sqlwherein = " AND WebID = 6 "; break;
            //    default: sqlwherein = ""; break;
            //}
            //sqlWhere += sqlwherein;
            #endregion

            #region Sort By
            if (Sort == null || Sort == 0)
                Sort = 3;
            switch (Sort)
            {
                case 1:
                    sqlOrderBy = svCompany.CreateOrderby(OrderBy.ModifiedDateDESC);
                    break;
                case 2:
                    sqlOrderBy = svCompany.CreateOrderby(OrderBy.CreatedDateDESC);
                    break;
                case 3:
                    sqlOrderBy = svCompany.CreateOrderby(OrderBy.ViewCountDESC);
                    break;
            }
            ViewBag.Sort = Convert.ToString(Sort);
            #endregion

            if (!string.IsNullOrEmpty(txtSearch))
            {
                #region Search By Text Search
                sqlWhere += " AND ( CompName LIKE N'%" + txtSearch + "%'  ";
                var sqlWhereProMatch = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd);
                sqlWhereProMatch += svProduct.CreateWhereCause(0, txtSearch, 0, 0, catelevel, cateid, (int)BizTypeID, (int)CompLevelID, (int)ProvinceID);
                sqlWhereProMatch += " Group By CompID  ";


                var ProMatch = svProduct.SelectData<view_SearchProduct>(" CompID ", sqlWhereProMatch, "", 0, 0, false);
                ViewBag.ProMatch = ProMatch;

                #region SuppliersProductMatching
                string CompPro = string.Empty;
                if (ViewBag.ProMatch != null)
                {
                    if (ProMatch.Count > 0)
                    {
                        for (int x = 0; x < ProMatch.Count; x++)
                        {
                            CompPro += ProMatch[x].CompID + ",";
                        }
                        CompPro = CompPro.Substring(0, CompPro.Length - 1);
                        sqlWhere += " Or CompID IN (" + CompPro + ") ";
                    }

                }

                #endregion

                sqlWhere += " ) ";
                #endregion

            }
            //else
            //{
            //    #region No Text Search
            //    var sqlWhereProMatch = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd);

                //sqlWhereProMatch += svProduct.CreateWhereCause(0, txtSearch, 0, 0, catelevel, cateid, (int)BizTypeID, (int)CompLevelID, (int)ProvinceID);
                //sqlWhereProMatch += " AND ProductCount > 0 ";
                //sqlWhereProMatch += " Group By CompID  ";


                //var ProMatch = svProduct.SelectData<view_SearchProduct>(" CompID ", sqlWhereProMatch, "", 0, 0, false);
                //ViewBag.ProMatch = ProMatch;

                //#region SuppliersProductMatching
                //string CompPro = string.Empty;
                //if (ViewBag.ProMatch != null)
                //{
                //    if (ProMatch.Count > 0)
                //    {
                //        for (int x = 0; x < ProMatch.Count; x++)
                //        {
                //            CompPro += ProMatch[x].CompID + ",";
                //        }
                //        CompPro = CompPro.Substring(0, CompPro.Length - 1);
                //        sqlWhere += " AND CompID IN (" + CompPro + ") ";
                //    }

                //}
                //#endregion
            //    #endregion
            //}


            var Suppliers = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere, "CompLevel DESC," + sqlOrderBy, (int)PIndex, (int)PSize);
            ViewBag.Suppliers = Suppliers;
            
            #region ProductMatching
            string CompID = string.Empty;
            if (ViewBag.Suppliers != null)
            {
                //if (Suppliers.Count > 0)
                //{
                //    for (int x = 0; x < Suppliers.Count; x++)
                //    {
                //        CompID += Suppliers[x].CompID + ",";
                //    }
                //    CompID = CompID.Substring(0, CompID.Length - 1);
                //    sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd);
                //    sqlWhere += svProduct.CreateWhereCause(0, txtSearch);
                //    sqlWhere += " AND CompID IN (" + CompID + ") ";
                //    var ProductMatching = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName,ProductImgPath,CompID", sqlWhere, "ModifiedDate DESC", 1,20);
                //    ViewBag.ProductMatching = ProductMatching;
                //}
            }
            #endregion

            LoadInterestSupplier();

            #region Set ViewBag
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalRow = svCompany.TotalRow;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.CateID = CategoryID;
            #endregion

            if (CategoryID == 0)
            {
                ViewBag.CateParentLevel = 0;
            }
            else
            {
                ViewBag.CateParentLevel = CateLevel;
            }

        }

        #endregion

        #region PostIndex
        [HttpPost]
        public ActionResult List(int? Sort = 1, int PIndex = 1, int PSize = 20, string txtSearch = "", int? BizTypeID = 0, int CateLevel = 0, int CategoryID = 0, int? CompLevelID = 0, int ProvinceID = 0)
        {
            CommonService svCommon = new CommonService();
            ViewBag.TextSearch = txtSearch;
            GetStatusUser();
            FindCateName(CategoryID, CateLevel);
            ViewBag.EnumSortBySupplier = svCommon.SelectEnum(CommonService.EnumType.SortBySupplier);
            DoloadSearchSupplier(Sort, PIndex, PSize, txtSearch, BizTypeID, CateLevel, CategoryID, CompLevelID, ProvinceID);

            //if (Request.Browser.IsMobileDevice)
            //{
            //    return PartialView("UC_mobile/SupplierListUC");
            //}
            //else
            //{
                return PartialView("Search/SupplierListUC");
            //}

        }
        #endregion
 
        #region PostIndex
        [HttpPost]
        public ActionResult GetRelateSupplier(List<int> compid,string textsearch)
        {
            var svProduct = new Product.ProductService();
            var products = new List<view_SearchProduct>();
            foreach(var item in compid)
            {
                var where = string.Empty;                    
                    where = " IsDelete = 0 AND RowFlag IN (4,5,6) ";
                    where = where + svProduct.CreateWhereCause(item, textsearch);

                    var data = svProduct.SelectData<view_SearchProduct>(" ProductID,CompID,CompName,ProductName,ProductImgPath ", where, null, 1, 4);
                foreach (var p in data)
                {
                    products.Add(p);
                }

            }

            return Json(products);
        }
        #endregion

        #region Detail

        [HttpGet]
        public ActionResult Detail(int? id)
        {
            CommonService svCommon = new CommonService();
            CompanyService svCompany = new CompanyService();
            Ouikum.Product.ProductService svProduct = new Ouikum.Product.ProductService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            RememberURL();

            GetStatusUser();
            var user = (Company.UserStatusModel)ViewBag.UserStatus;

            var sqlWhere = string.Empty;
            #region Set Supplier Detail

            var Companies = svCompany.SelectData<view_Company>("*", "CompID = " + id, null, 1, 0, false);
            ViewBag.Company = Companies.First();
            ViewBag.Province = Companies.First().CompProvinceID;

            var Shipments = svCompany.SelectData<b2bCompanyShipment>("*", " IsDelete = 0 AND CompID = " + id, null, 1, 0, false);
            if (Shipments.Count > 0)
                ViewBag.CompanyShipment = Shipments;

            var Payments = svCompany.SelectData<view_CompanyPayment>("*", " IsDelete = 0 AND CompID = " + id, null, 1, 0, false);
            if (Payments.Count > 0)
                ViewBag.CompanyPayments = Payments;

            #region companyProduct
            sqlSelect = @"ProductID,ProductName,ProductCount,CompID,CompName,CompLevel,BizTypeName,CompProvinceID," +
                    "BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3,ViewCount,ModifiedDate,CreatedDate,ContactCount,Price,IsPromotion,PromotionPrice";
            sqlWhere = svProduct.CreateWhereAction(Ouikum.Product.ProductAction.FrontEnd, 0);
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            sqlWhere += " AND CompID =" +id;
            var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "Complevel DESC, ModifiedDate DESC", 1, 0, false);
            if (Products.Count > 0)
                ViewBag.CompanyPorducts = Products;
            #endregion
            ViewBag.CompID = id;
            ViewBag.MetaKeyword = ViewBag.Title;
            #endregion

            ViewBag.ProID = id;

            #region Send Message
            if (user.CompID != 0)
                ViewBag.CompanyView = svCompany.SelectData<b2bCompany>("id,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone", "id = " + user.CompID, null, 1, 0, false).First();
            #endregion

            AddViewCount((int)id, "Supplier");

            return View();
        }
        #endregion

        #region Category Name
        private void FindCateName(int? CategoryID, int? CateLevel)
        {
            if (CategoryID > 0)
            {

                switch (CateLevel)
                {
                    case 0:
                        ViewBag.CateNameLV1 = "";
                        ViewBag.CateNameLV2 = "";
                        ViewBag.CateNameLV3 = "";
                        ViewBag.CateLV = 0;
                        break;

                    case 1:
                        var case1 = svCategory.SearchCategoryByID((int)CategoryID).First();
                        ViewBag.CateLV1 = CategoryID;
                        ViewBag.CateLV = 1;

                        ViewBag.CateNameLV1 = case1.CategoryName;
                        ViewBag.ReCateNameLV1 = @Url.ReplaceUrl(case1.CategoryName);
                        ViewBag.catetype = case1.CategoryType;
                        ViewBag.Title = MvcHtmlString.Create(ViewBag.CateNameLV1 + " | " + res.Common.lblDomainShortName);
                        break;

                    case 2:
                        var case2 = svCategory.SearchCategoryByID((int)CategoryID).First();
                        ViewBag.CateNameLV2 = case2.CategoryName;
                        ViewBag.ReCateNameLV2 = @Url.ReplaceUrl(case2.CategoryName);
                        ViewBag.CateLV2 = CategoryID;
                        ViewBag.CateLV = 2;
                        ViewBag.Title = MvcHtmlString.Create(ViewBag.CateNameLV2 + " | " + res.Common.lblDomainShortName);

                        var ParentCategoryID = case2.ParentCategoryID;
                        var datacase2 = svCategory.SearchCategoryByID((int)ParentCategoryID).First();
                        ViewBag.CateNameLV1 = datacase2.CategoryName;
                        ViewBag.ReCateNameLV1 = @Url.ReplaceUrl(datacase2.CategoryName);
                        ViewBag.catetype = datacase2.CategoryType;
                        ViewBag.CateLV1 = ParentCategoryID;
                        break;

                    case 3:
                        var case3 = svCategory.SearchCategoryByID((int)CategoryID).First();
                        ViewBag.CateNameLV3 = case3.CategoryName;
                        ViewBag.ReCateNameLV3 = @Url.ReplaceUrl(case3.CategoryName);
                        ViewBag.CateLV3 = CategoryID;
                        ViewBag.CateLV = 3;
                        ViewBag.Title = MvcHtmlString.Create(ViewBag.CateNameLV3 + " | " + res.Common.lblDomainShortName);


                        var ParentCategoryIDLV2 = case3.ParentCategoryID;
                        var parentLV2 = svCategory.SearchCategoryByID((int)ParentCategoryIDLV2).First();
                        ViewBag.CateNameLV2 = parentLV2.CategoryName;
                        ViewBag.ReCateNameLV2 = @Url.ReplaceUrl(parentLV2.CategoryName);
                        ViewBag.CateLV2 = ParentCategoryIDLV2;

                        var ParentCategoryIDLV1 = parentLV2.ParentCategoryID;
                        var parentLV1 = svCategory.SearchCategoryByID((int)ParentCategoryIDLV1).First();
                        ViewBag.CateNameLV1 = parentLV1.CategoryName;
                        ViewBag.ReCateNameLV1 = @Url.ReplaceUrl(parentLV1.CategoryName);
                        ViewBag.catetype = parentLV1.CategoryType;
                        ViewBag.CateLV1 = ParentCategoryIDLV1;
                        break;
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

        #region ChildCateMenu
        [HttpPost]
        public ActionResult ChildCateMenu(int CategoryID, int CateLevel)
        {
            string sqlWhere = "";
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            switch (CateLevel)
            {
                case 0:
                    sqlWhere = "CategoryLevel = 1 AND ProductCount > 0 AND RowFlag = 1 AND IsDelete = 0";
                    break;

                case 1:
                    if (CategoryID != 0) { sqlWhere = " ParentCategoryID = " + CategoryID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0"; }
                    break;

                case 2:
                    if (CategoryID != 0) { sqlWhere = " ParentCategoryID = " + CategoryID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0"; }
                    break;

                case 3:
                    var ParentCategoryID = svCategory.SearchCategoryByID((int)CategoryID).First().ParentCategoryID;
                    sqlWhere = "ParentCategoryID = " + ParentCategoryID + " AND ProductCount > 0 AND RowFlag = 1 AND IsDelete = 0";
                    break;
            }
            //sqlWhere += sqlwherein;
            var b2bCategorys = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,ParentCategoryID,CategoryName", sqlWhere, "CategoryID", 1, 0);
            b2bCategorys = b2bCategorys.OrderBy(m => m.CategoryLevel).ThenBy(m => m.CategoryName).ToList();
            var Obj = b2bCategorys.Select(it => new
            {
                CategoryID = it.CategoryID,
                ParentCategoryID = it.ParentCategoryID,
                CategoryLevel = it.CategoryLevel,
                CategoryName = it.CategoryName
            });
            return Json(b2bCategorys);

        }
        #endregion

        #region GetCompName
        [HttpPost]
        public ActionResult GetCompName(string query)
        {
            svCompany = new CompanyService();

            sqlWhere = svCompany.CreateWhereAction(CompStatus.HaveProduct, 0);
            sqlWhere += svCompany.CreateWhereCause(0, "", query, 0, 0, 0);
            var b2bSuppliers = svCompany.SelectData<view_Company>("CompName", sqlWhere, "CompName");
            var CompName = b2bSuppliers.Select(it => it.CompName).ToList();

            return Json(CompName);

        }
        #endregion
    }
}
