using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
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


        #region GetManageJunk
        [HttpGet]
        public ActionResult Junk()
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
            ViewBag.MenuName = "Junk";
            SelectList_PageSize();
            DoLoadComboBoxProductStatus();
            DoLoadQtyUnits();
            CountMessage();
            CountQuotation();
            //select Category
            ViewBag.IndrustryCateLV1 = svCategory.ListMenuJunkCategory(LogonCompID);
            //ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory(LogonCompID);
            ViewBag.CateLevel1 = LogonCompLevel;

            return View();
        }
        #endregion

        #region GetManageJunk
        [HttpPost]
        public ActionResult Junk(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);
            List_DoloadData(ProductAction.Junk);
            return PartialView("UC/Junk/Content");
        }
        #endregion
         

        #region PostMoveToJunk
        [HttpPost]
        public ActionResult MoveToJunk(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.MoveToJunk(ProductID, CateLV1, CateLV2, CateLV3, LogonCompID);
                //var mgKeyword = new KeywordMongo();
                //foreach (var item in ProductID)
                //{
                //    mgKeyword.UpdateMongoProduct(item);
                //}
            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

        #region PostDeleteProduct
        [HttpPost]
        public ActionResult DeleteProduct(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.Delete(ProductID, CateLV1, CateLV2, CateLV3, LogonCompID);

                //var mgKeyword = new KeywordMongo();
                //foreach (var item in ProductID)
                //{
                //    mgKeyword.RemoveProductKeywordMongo(item);
                //}

                foreach(var ID in ProductID){
                    imgManager = new FileHelper();
                    imgManager.DeleteFilesInDir("Product/" + LogonCompID + "/" + ID);
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

        #region PostRestore
        [HttpPost]
        public ActionResult Restore(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svProduct = new ProductService();

            try
            {
                svProduct.ReStore(ProductID, CateLV1, CateLV2, CateLV3, LogonCompID);
                //var mgKeyword = new KeywordMongo();
                //foreach (var item in ProductID)
                //{
                //    mgKeyword.UpdateMongoProduct(item);
                //}

            }
            catch (Exception ex)
            {
                svProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError) });
        }
        #endregion

    }
}
