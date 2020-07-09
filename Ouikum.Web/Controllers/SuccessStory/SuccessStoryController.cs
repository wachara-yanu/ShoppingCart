using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Article;
using Ouikum;
using Ouikum.Common;
//usin other
//using Prosoft.Base;

namespace Ouikum.Controllers
{
    public class SuccessStoryController : BaseController
    {
        //
        // GET: /SuccessStory/
        #region Member
        ArticleService svArticle;
        CommonService svCommon = new Common.CommonService();
        #endregion 

        #region Get List
        [HttpGet]
        public ActionResult List()
        {

            if (RedirectToProduction())
                return Redirect(UrlProduction);

            GetStatusUser();
            List(1,10);
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            var svArticle = new ArticleService();

            /*Success Story*/
            var Articles = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 AND ArticleTypeID = 7", "ModifiedDate DESC", 1, 10);
            if (Articles.Count > 0)
            {
                ViewBag.Articles = Articles;
                ViewBag.PageIndex = 1;
                ViewBag.TotalPage = svArticle.TotalPage;
                ViewBag.TotalRow = svArticle.TotalRow;
                ViewBag.PageSize = 10;
            }
            LoadHotSuccessStory();

            /*Suppliers Success Story*/
            ViewBag.SuppliersStory = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7", "ModifiedDate DESC", 1, 5);

            return View();
        }
        #endregion 

        #region Post List
        [HttpPost]
        public ActionResult List(int? PIndex, int? PSize)
        {
            int Index = (PIndex != null) ? (int)PIndex : 1;
            int Size = (PSize != null) ? (int)PSize : 10;
            var svArticle = new ArticleService();
            var Articles = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 AND ArticleTypeID = 7", "ModifiedDate DESC", (int)Index, Size);
            if (Articles.Count > 0) {
                ViewBag.Articles = Articles;
                ViewBag.PageIndex = Index;
                ViewBag.TotalPage = svArticle.TotalPage;
                ViewBag.TotalRow = svArticle.TotalRow;
                ViewBag.PageSize = Size;
            }
            return PartialView("UC/ArticleListUC");
        }
        #endregion

        #region Get Detail
        public ActionResult Detail(int? ID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            if (Convert.ToInt32(ID) > 0)
            {
                var svArticle = new ArticleService();
                /*Success Story Detail*/

                var StoryDetail = svArticle.SelectData<view_b2bArticle>("*", "ArticleID = " + ID, null, 1, 0, false).First();
                ViewBag.StoryDetail = StoryDetail;

                /*Suppliers Success Story*/
                ViewBag.SuppliersStory = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7", "ModifiedDate DESC", 1, 5);
                
                /*Update ViewCount*/
                AddViewCount(Convert.ToInt32(StoryDetail.ArticleID), "Blog");
            }
            return View();
        }
        #endregion
    }
}
