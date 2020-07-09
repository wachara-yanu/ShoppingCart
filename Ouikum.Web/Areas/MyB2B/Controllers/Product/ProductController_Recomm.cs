using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Web.Models;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        // GET: /MyB2B/Product/

        public ActionResult test()
        {
            return View();
        }

        #region GetManageRecommend
        public ActionResult Recommend()
        {
            
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            if (LogonServiceType != 2 && LogonServiceType != 3)
                return Redirect(res.Pageviews.PvAccessDenied); 


            var svCategory = new CategoryService();
            var svProductGroup = new ProductGroupService();

            GetStatusUser();

            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = 0;

            ViewBag.PageType = "Product";
            ViewBag.MenuName = "Recommend";
            SelectList_PageSize();
            DoLoadComboBoxProductStatus();
            CountMessage();
            CountQuotation();
            //select Category
            ViewBag.IndrustryCateLV1 = svCategory.ListMenuRecommendCategory(LogonCompID);
            //ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory(LogonCompID);
            ViewBag.CateLevel1 = LogonCompLevel;

            return View();
        }
        #endregion

        #region PostManageRecommend
        [HttpPost]
        public ActionResult Recommend(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);
            List_DoloadDataRecommend(ProductAction.Recommend);
            return PartialView("UC/Recommend/Content");
        }

        #endregion

        #region List_DoloadDataRecommend
        public void List_DoloadDataRecommend(ProductAction action)
        {
            var svProduct = new ProductService();
            sqlSelect = "ProductID,ProductName,ProductImgPath,RowFlag,CompID,ListNo";
            sqlWhere = svProduct.CreateWhereAction(action, LogonCompID);

            sqlOrderBy = " ListNo ASC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(LogonCompID);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "ModifiedDate");
            #endregion

            var products = svProduct.SelectData<b2bProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Products = products;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
        }
        #endregion

        #region MoveToStore
        [HttpPost]
        public ActionResult MoveToStore(int ProductID)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.MoveToStore(ProductID, LogonCompID);

                //var mgKeyword = new KeywordMongo();
                //mgKeyword.UpdateMongoProduct(ProductID); 
            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }


        #endregion

        #region MoveToRecommend
        [HttpPost]
        public ActionResult MoveToRecommend(int ProductID, int CateLV1, int CateLV2, int CateLV3)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.MoveToRecommend(ProductID, CateLV1, CateLV2, CateLV3, LogonCompID);

                //var mgKeyword = new KeywordMongo();
                //mgKeyword.UpdateMongoProduct(ProductID); 
            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

        #region ValidateIsMaxRecommend
        [HttpPost]
        public ActionResult ValidateIsMaxRecommend()
        {
            var svProduct = new ProductService();
            svProduct.ValidateFullRecommend(LogonCompID);
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError), Count = svProduct.RecommendCount });
        }

        #endregion

        #region AddRecommend

        [HttpPost]
        public ActionResult AddRecommend(List<int> ProductID)
        {
            var svProduct = new ProductService();
            try
            {
                svProduct.MoveToRecommend(ProductID, LogonCompID);
            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }


        #endregion

        #region SaveRecommend

        [HttpPost]
        public ActionResult SaveChangeListNo(List<int> ProductID)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.SaveChangeListNo(ProductID, LogonCompID);

            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }


        #endregion

        #region FormAddRecommend
        [HttpPost]
        public ActionResult FormAddRecommend(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);

            List_DoloadDataFormAddRecommend(ProductAction.BackEnd);
            return PartialView("UC/Recommend/FormAdd");
        }
        #endregion

        #region List_DoloadDataFormAddRecommend
        public void List_DoloadDataFormAddRecommend(ProductAction action)
        {
            var svProduct = new ProductService();
            sqlSelect = "ProductID,ProductName,ProductImgPath,CompID";
            sqlWhere = svProduct.CreateWhereAction(action, LogonCompID) + " AND ListNo = 0 AND RowFlag >=4 ";

            sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(0, ViewBag.TextSearch);
            #endregion

            var products = svProduct.SelectData<b2bProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, 30);
            ViewBag.Products = products;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
        }
        #endregion

        #region PostProductDetailGetByID
        [HttpPost]
        public ActionResult RecommendDetail(int ID)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProduct = new ProductService();
            sqlSelect = "ProductID,ProductCode,ProductName,ProductImgPath,ShortDescription ,ModifiedDate, CreatedDate,RowFlag,ListNo,";
            sqlSelect += "CateLV1,CateLV2,CateLV3,CompID,ProductGroupName,CategoryName,ViewCount";
            sqlWhere = "ProductID=" + ID;
            var Product = svProduct.SelectData<view_Product>(sqlSelect, sqlWhere, null, 0, 0).First();

            Product.ModifiedDate = DataManager.ConvertToDateTime(Product.ModifiedDate);
            Product.CreatedDate = DataManager.ConvertToDateTime(Product.CreatedDate);

            ViewBag.ProductGetByID = Product;
            var svProductGroup = new ProductGroupService();
            var ProductGroups = svProductGroup.GetProductGroup(LogonCompID);
            if (ProductGroups.Count() > 0)
            {
                ViewBag.ProductGroups = ProductGroups;
            }
            else
            {
                ViewBag.ProductGroups = null;
            }
            return PartialView("UC/Recommend/ProductDetail");
        }
        #endregion

    }
}
