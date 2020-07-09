using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using System.Runtime.Caching;
using Ouikum.Web.Models;
namespace Ouikum.Web.Search
{
    public partial class ProductController : BaseController
    {
        string sqlSelect, sqlWhere, sqlOrderBy = "";
        #region Index
        [HttpGet]
        public ActionResult Index(int? Category, int? CateLevel, string CategoryName, string TextSearch, int? Province)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            GetStatusUser();
            int ProvinceID = (Province != null || Province == 0) ? (int)Province : 0;
            int CateID = (Category != null || Category == 0) ? (int)Category : 0;
            int CateLV = (CateLevel != null) ? (int)CateLevel : 0;
            string txtSearch = (TextSearch != null) ? TextSearch.Trim() : "";
            if (txtSearch == "")
            {
                ViewBag.TextSearch = "";
                if (CateID == 0)
                {
                    ViewBag.Title = MvcHtmlString.Create(res.Common.lblSearchProduct + " | " + res.Common.lblDomainShortName);
                }
            }
            else
            {
                ViewBag.TextSearch = txtSearch;
                ViewBag.Relate = txtSearch;
                ViewBag.Title = MvcHtmlString.Create(ViewBag.TextSearch + " | " + res.Common.lblDomainShortName);
            }

            //#region query Product
            var data = svProduct.SearchProduct(ProductAction.FrontEnd, txtSearch, CateLV, CateID, 0, 0, 0, 1, 20);
            ViewBag.Status = svProduct.CodeError;
            var products = MappingProduct(data);
            ViewBag.Products = products;
            //#endregion

            #region query Product Mongo
            //var mgKeywod = new KeywordMongo();
            //var mongoProducts = mgKeywod.SearchProductMongo(1, 1, 20, txtSearch, 0, CateLV, CateID, 0, ProvinceID);
            //var products = MappingProductMongo(mongoProducts);
            ViewBag.Status = svProduct.CodeError;
            ViewBag.Products = products;
            #endregion


            #region Category

            var ParentCate = svCategory.GetCategoryByLevel(1);

            ViewBag.Category = ParentCate;
            #endregion

            #region query Biztype
            var Biztype = svBizType.GetBiztypeAll();
            ViewBag.Biztype = Biztype;
            #endregion

            #region query Province
            //ViewBag.Province = svAddress.GetProvinceAll();
            LoadProvinces();
            #endregion

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.EnumSortByProduct = svCommon.SelectEnum(CommonService.EnumType.SortByProduct);
            if (TextSearch != "")
            {
                //  LoadFeatureProduct(TextSearch, Category, CateLevel);
                LoadHotProduct(TextSearch, Category, CateLevel);
            }
            FindCateName(CateID, CateLV);

            if ((Category == 1083 || Category == 1165))
            {
                ViewBag.ShowBranner = "Show";
                ViewBag.CateBanner = Category;
            }


            #region Set ViewBag
            ViewBag.CateLV = CateLV;
            ViewBag.CateID = CateID;
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;

            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;

            //ViewBag.TotalRow = mgKeywod.TotalRow;
            //ViewBag.TotalPage = mgKeywod.TotalPage;


            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription = ViewBag.Title;
            #endregion
            //  GetStatusUser();

            #region Load Feature
            var feature = new List<view_HotFeaProduct>();
            if (MemoryCache.Default["LoadFeatureSearch"] != null)
            {
                feature = (List<view_HotFeaProduct>)MemoryCache.Default["LoadFeatureSearch"];
                feature = feature.OrderBy(x => Guid.NewGuid()).ToList();
                feature = feature.OrderByDescending(m => m.Price).ToList();
            }
            else
            {
                var svHotFeat = new HotFeaProductService();

                var SQLSelect_Feat = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_Feat = "ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice";
                //else

                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
                feature = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 10);//""
                if (svHotFeat.TotalRow > 0)
                {
                    MemoryCache.Default.Add("LoadFeatureSearch", feature, DateTime.Now.AddMinutes(10));
                }
            }
            ViewBag.FeatProducts = feature;
            #endregion

            return View();
        }
        #endregion

        #region PostIndex
        [HttpPost]
        public ActionResult List(int? Sort = 1, int PIndex = 1, int PSize = 20, string txtSearch = "", int? BizTypeID = 0, int CateLevel = 0, int CategoryID = 0, int? CompLevelID = 0, int ProvinceID = 0)
        {
            ViewBag.Title = res.Common.lblSearchProduct;

            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.Trim();
                LoadHotProduct(txtSearch, CategoryID, CateLevel);
            }

            GetStatusUser();
            CommonService svCommon = new CommonService();
            ViewBag.EnumSortByProduct = svCommon.SelectEnum(CommonService.EnumType.SortByProduct);
            sqlSelect = @"ProductID,ProductName,ProductNameEng,ProductCount,CompID,CompName,CompLevel,BizTypeName,CompProvinceID,ListNo," +
                    "BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3,ViewCount,ModifiedDate,CreatedDate,ContactCount,Price,IsPromotion,PromotionPrice";

            #region DoWhereCause
            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            sqlWhere += svProduct.CreateWhereCause(0, txtSearch, 0, 0, (int)CateLevel, (int)CategoryID, (int)BizTypeID, (int)CompLevelID, (int)ProvinceID);

            #endregion

            #region Sort By
            sqlOrderBy += " ListNo DESC ";
            if (CompLevelID == 3)
            {
                sqlOrderBy += " ,Complevel DESC ";
            }

            switch (Sort)
            {
                case 1:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ComplevelDESC);
                    break;
                case 2:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.CreatedDateDESC);
                    break;
                case 3:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ViewCountDESC);
                    break;
                case 4:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ContactCountDESC);
                    break;
            }
            #endregion
            sqlOrderBy += " , ModifiedDate DESC  ";

            #region Get b2bProduct
            FindCateName(CategoryID, CateLevel);
            ViewBag.Sort = Convert.ToString(Sort);

            var data = svProduct.SearchProduct(ProductAction.FrontEnd, txtSearch, (int)CateLevel, (int)CategoryID, (int)BizTypeID, (int)ProvinceID, 0, PIndex, PSize, sqlOrderBy);
            var products = MappingProduct(data);
            //var mgKeyword = new KeywordMongo();
            //  var products = new List<Models.tbProduct>();
            //var mongoProducts = mgKeyword.SearchProductMongo(Sort, PIndex, PSize, txtSearch, BizTypeID, CateLevel, CategoryID, CompLevelID, ProvinceID);
            //var products = MappingProductMongo(mongoProducts);
            ViewBag.Products = products;
            if (svProduct.TotalRow > 0)
            {
                var product = products.First();
                if (BizTypeID > 0)
                {
                    ViewBag.Title += " " + product.BizTypeName;
                }
                if (ProvinceID > 0)
                {
                    ViewBag.Title += " " + product.ProvinceName;
                }
            }
            ViewBag.Title += " | " + res.Common.lblDomainShortName;
            #endregion

            #region Set ViewBag
            ViewBag.TextSearch = txtSearch;
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.CateID = CategoryID;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
            //ViewBag.TotalRow = mgKeyword.TotalRow;
            //ViewBag.TotalPage = mgKeyword.TotalPage;

            if (CategoryID == 0)
            {
                ViewBag.CateParentLevel = 0;
            }
            else
            {
                ViewBag.CateParentLevel = CateLevel;
            }
            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription = ViewBag.Title;
            #endregion

            #region Load Feature
            var feature = new List<view_HotFeaProduct>();
            if (MemoryCache.Default["LoadFeatureSearch"] != null)
            {
                feature = (List<view_HotFeaProduct>)MemoryCache.Default["LoadFeatureSearch"];
                feature = feature.OrderBy(x => Guid.NewGuid()).ToList();
                feature = feature.OrderByDescending(m => m.Price).ToList();
            }
            else
            {
                var svHotFeat = new HotFeaProductService();
                var SQLSelect_Feat = "";

                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
                feature = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 10);//""
                if (svHotFeat.TotalRow > 0)
                {
                    MemoryCache.Default.Add("LoadFeatureSearch", feature, DateTime.Now.AddMinutes(10));
                }
            }
            ViewBag.FeatProducts = feature;
            #endregion

            //stopwatch.Stop();
            //if (Request.Browser.IsMobileDevice)
            //{
            //    return PartialView("UC_mobile/ProductListUC");
            //}else{
            return PartialView("Search/ProductListUC");
            //}

        }
        #endregion


        #region BindList
        public ActionResult BindList(int? Sort = 1, int PIndex = 1, int PSize = 20, string txtSearch = "", int? BizTypeID = 0, int CateLevel = 0, int CategoryID = 0, int? CompLevelID = 0, int ProvinceID = 0)
        {
            if (!string.IsNullOrEmpty(txtSearch))
            {
                txtSearch = txtSearch.Trim();
            }
            CommonService svCommon = new CommonService();
            sqlSelect = @"ProductID,ProductName,ProductCount,CompID,CompName,CompLevel,BizTypeName,CompProvinceID," +
                    "BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3,ViewCount,ModifiedDate,CreatedDate,ContactCount";

            #region DoWhereCause
            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            sqlWhere += svProduct.CreateWhereCause(0, txtSearch, 0, 0, (int)CateLevel, (int)CategoryID, (int)BizTypeID, (int)CompLevelID, (int)ProvinceID);

            #endregion

            #region Sort By
            switch (Sort)
            {
                case 1:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ModifiedDateDESC);
                    break;
                case 2:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.CreatedDateDESC);
                    break;
                case 3:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ViewCountDESC);
                    break;
                case 4:
                    sqlOrderBy += svProduct.CreateOrderby(OrderBy.ContactCountDESC);
                    break;
            }
            #endregion

            #region Get b2bProduct
           var b2bProducts = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "Complevel DESC," + sqlOrderBy, PIndex, PSize);
            #endregion

            #region Mongo Get Product
           //var mgKeyword = new KeywordMongo();
           //var data = mgKeyword.SearchProductMongo(Sort, PIndex, PSize, txtSearch, BizTypeID, CateLevel, CategoryID, CompLevelID, ProvinceID);
           //var mongoProducts = MappingProductMongo(data);
            #endregion

            return Json(new
            {
                Products = b2bProducts,
                //Products = mongoProducts, // mongo data
                CateID = CategoryID,
                TotalRow = svProduct.TotalRow,
                TotalPage = svProduct.TotalPage                
                //TotalRow = mgKeyword.TotalRow,
                //TotalPage = mgKeyword.TotalPage
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Category Name
        private void FindCateName(int CategoryID, int CateLevel)
        {
            if (CategoryID != 0)
            {

                switch (CateLevel)
                {
                    case 0:
                        ViewBag.CateNameLV1 = "";
                        ViewBag.CateNameLV2 = "";
                        ViewBag.CateNameLV3 = "";
                        ViewBag.CateNameLV1Eng = "";
                        ViewBag.CateNameLV2Eng = "";
                        ViewBag.CateNameLV3Eng = "";
                        ViewBag.CateLV = 0;
                        break;

                    case 1:
                        var case1 = svCategory.SearchCategoryByID((int)CategoryID).First();
                        ViewBag.CateLV1 = CategoryID;
                        ViewBag.CateLV = 1;
                        ViewBag.CateNameLV1 = case1.CategoryName;
                        ViewBag.CateNameLV1Eng = case1.CategoryNameEng;
                        ViewBag.Relate = ViewBag.CateNameLV1;
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
            //string sqlWhere = "";
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
                    if (CategoryID != 0) { sqlWhere = " ParentCategoryID = " + CategoryID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0 "; }
                    break;

                case 2:
                    if (CategoryID != 0) { sqlWhere = " ParentCategoryID = " + CategoryID + " AND ProductCount > 0 AND RowFlag  = 1 AND IsDelete = 0 "; }
                    break;

                case 3:
                    var ParentCategoryID = svCategory.SearchCategoryByID((int)CategoryID).First().ParentCategoryID;
                    sqlWhere = "ParentCategoryID = " + ParentCategoryID + " AND ProductCount > 0 AND RowFlag = 1 AND IsDelete = 0 ";
                    break;
            }
            //sqlWhere += sqlwherein;
            var sqlSelect = "";
            //if (Prosoft.Base.Base.AppLang == "en-US")
            //    sqlSelect = "CategoryID,CategoryLevel,ParentCategoryID,CategoryNameEng AS CategoryName";
            //else
            sqlSelect = "CategoryID,CategoryLevel,ParentCategoryID,CategoryName,CategoryNameEng";
            var b2bCategorys = svCategory.SelectData<b2bCategory>(sqlSelect, sqlWhere, "CategoryName,CategoryNameEng", 1, 0);
            b2bCategorys = b2bCategorys.OrderBy(m => m.CategoryLevel).ThenBy(m => m.CategoryName).ToList();
            var Obj = b2bCategorys.Select(it => new
            {
                CategoryID = it.CategoryID,
                ParentCategoryID = it.ParentCategoryID,
                CategoryLevel = it.CategoryLevel,
                CategoryName = it.CategoryName,
                CategoryNameEng = it.CategoryNameEng
            });
            return Json(b2bCategorys);

        }
        #endregion

        #region GetProductName
        [HttpPost]
        public ActionResult GetProductName(string query)
        {
            svProduct = new ProductService();

            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            sqlWhere += svProduct.CreateWhereCause(0, query, 0, 0, 0, 0, 0, 0, 0);

            var b2bProducts = svProduct.SelectData<view_Product>("ProductName", sqlWhere, "ProductName");
            var ProductName = b2bProducts.Select(it => it.ProductName).ToList();

            return Json(ProductName);

        }
        #endregion

        public List<view_SearchProduct> MappingProduct(List<view_ProductMobileApp> model)
        {
            var data = new List<view_SearchProduct>();
            var biztype = svBizType.GetBiztypeAll();
            var provinces = svAddress.GetProvinceAll();
            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    var it = new view_SearchProduct();
                    it.ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                    if (item.BizTypeID != 14)
                    {
                        it.BizTypeName = biztype.Where(m => m.BizTypeID == item.BizTypeID).FirstOrDefault().BizTypeName;
                    }
                    else
                    {
                        it.BizTypeName = item.BizTypeOther;
                    }
                    it.ProductID = item.ProductID;
                    it.ProductName = item.ProductName;
                    it.ProductImgPath = item.ProductImgPath;
                    it.ProductKeyword = item.ProductKeyword;
                    it.ShortDescription = item.ShortDescription;
                    it.ViewCount = item.ViewCount;
                    it.CompID = item.CompID;
                    it.ContactCount = item.ContactCount;
                    it.comprowflag = item.CompRowFlag;
                    it.CompLevel = (byte)item.Complevel;
                    it.CompName = item.CompName;
                    it.ListNo = item.ListNo;
                    it.CreatedDate = item.CreatedDate;
                    it.ModifiedDate = item.ModifiedDate;

                    data.Add(it);
                }
            }

            return data;
        }


        public List<view_SearchProduct> MappingProductMongo(List<Models.tbProduct> model)
        {
            var data = new List<view_SearchProduct>();
            var biztype = svBizType.GetBiztypeAll();

            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    var it = new view_SearchProduct();
                    it.ProvinceName = item.ProvinceName;
                    //   it.ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                    if (item.BizTypeID != 13)
                    {
                        it.BizTypeName = biztype.Where(m => m.BizTypeID == item.BizTypeID).FirstOrDefault().BizTypeName;
                    }
                    else
                    {
                        it.BizTypeName = item.BizTypeOther;
                    }
                    it.ProductID = (int)item.ProductID;
                    it.ProductName = item.ProductName;
                    it.ProductImgPath = item.ProductImgPath;
                    it.ProductKeyword = item.ProductKeyword;
                    it.ShortDescription = item.ShortDescription;
                    it.ViewCount = item.ViewCount;
                    it.CompID = item.CompID;
                    it.ContactCount = (short)item.ContactCount;
                    it.comprowflag = 2;
                    it.CompLevel = getCompLevelByCompID(item.CompID);
                    it.CompName = item.CompName;
                    it.ListNo = item.ListNo;
                    it.CreatedDate = item.CreatedDate;
                    it.ModifiedDate = item.ModifiedDate;
                    it.IsSME = false;

                    data.Add(it);
                }
            }

            return data;
        }
        private byte getCompLevelByCompID(int CompID = 0)
        {
            byte CompLevel = 0;
            try
            {
                if (CompID > 0)
                {
                    CompLevel = svProduct.qDB.ExecuteQuery<byte>("SELECT CompLevel from b2bCompany Where CompID = " + CompID).ToList().FirstOrDefault();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return CompLevel;
        }
    }

}
