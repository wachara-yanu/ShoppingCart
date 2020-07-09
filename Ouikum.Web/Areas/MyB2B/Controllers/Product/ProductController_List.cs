using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum;
using System.Text.RegularExpressions;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        // GET: /MyB2B/Product/

        #region GetManageProduct
        [HttpGet]
        public ActionResult Index(string PStatus, int? GroupID, int? CateLevel, int? CateID)
        { 
            RememberURL();

            string urlSingIn = res.Pageviews.PvMemberSignIn; 
            //if (Prosoft.Base.Base.AppLang == "en-US") { urlSingIn = Regex.Replace(urlSingIn, "~/", "~/en/"); }
            //else { urlSingIn = res.Pageviews.PvMemberSignIn; }
            string urlDenied = res.Pageviews.PvAccessDenied; 
            //if (Prosoft.Base.Base.AppLang == "en-US") { urlDenied = Regex.Replace(urlDenied, "~/", "~/en/"); }
            //else { urlDenied = res.Pageviews.PvAccessDenied; }

            if (!CheckIsLogin())
                return Redirect(urlSingIn);

            if (LogonServiceType != 2 && LogonServiceType != 3)
                return Redirect(urlDenied);


            CommonService svCommon = new CommonService();
            GetStatusUser();
            ViewBag.EnumProductStatus = svCommon.SelectEnum(CommonService.EnumType.ProductStatus);
            var svCategory = new CategoryService();
            var svProductGroup = new ProductGroupService();


            ViewBag.PageIndex = 1;
            ViewBag.TotalRow = 0;
            ViewBag.PStatus = PStatus != null ? PStatus : "0"; 
            ViewBag.GroupID = GroupID != null ? GroupID : 0;
            ViewBag.CateLevel = CateLevel != null ? CateLevel : 0;
            ViewBag.CateID = CateID != null ? CateID : 0;
            
            SelectList_PageSize();
            DoLoadComboBoxProductStatus();
            CountMessage();
            CountQuotation();
            //select Category
            ViewBag.IndrustryCateLV1 = svCategory.ListMenuProductCategory(LogonCompID);
            //ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory(LogonCompID);
            ViewBag.CateLevel1 = LogonCompLevel;
            ViewBag.PageType = "Product";
            ViewBag.MenuName = "Index";
            return View();
        }
        #endregion

        #region PostManageProduct
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);
            List_DoloadData(ProductAction.BackEnd);
            return PartialView("UC/ManageProductContent");
        }

        #endregion
        
        #region PostProductDetailGetByID
        [HttpPost]
        public ActionResult ProductGetByID(int ID)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProduct = new ProductService();
            sqlSelect = @"ProductID,ProductCode,ProductName,ProductImgPath,ShortDescription ,ModifiedDate, CreatedDate,RowFlag,ListNo,
           IsShow,CateLV1,CateLV2,CateLV3,CompID,ProductGroupID,ProductGroupName,CategoryName,ViewCount";
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
            return PartialView("UC/QuickProductDetail");
        }
        #endregion

        #region Post Changegroup
        [HttpPost]
        public ActionResult ChangeGroup(int ProductID, int GroupID)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProduct = new ProductService();
            svProduct.ChangeGroup(ProductID, GroupID);
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

        #region Post Show/NotShow
        [HttpPost]
        public ActionResult SaveIsShow(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3,int IsShow)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProduct = new ProductService();
            svProduct.SaveIsShow(ProductID, CateLV1, CateLV2, CateLV3,LogonCompID, IsShow);
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

    }
}
