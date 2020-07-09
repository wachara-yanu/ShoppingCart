using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Article;
using Ouikum.Company;
using Ouikum;

namespace OuikumThai.Web.Controllers.Article
{
    public class ArticleNewsController : BaseController
    {
        ArticleService svArticle;
        CompanyService svCompany;
        public ArticleNewsController()
        {
            svArticle = new ArticleService();
            svCompany = new CompanyService();
        }
        string sqlWhere = "";
        public ActionResult Index()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            GetEnumServiceType();
            GetStatusUser();
            SelectList_PageSize();
            #region Article
            sqlWhere = "RowFlag > 0 And ArticleTypeID=2";

            var Article = svArticle.SelectData<view_b2bArticle>("ArticleID,ArticleName,ModifiedDate", sqlWhere, "ModifiedDate DESC", 1, 20);
            ViewBag.Article = Article;
            #endregion

            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = svArticle.TotalRow;
            if (svArticle.TotalRow.ToString().Length > 2)
                ViewBag.TotalRow = String.Format("{0:0,0}", svArticle.TotalRow);
            ViewBag.Type = 2;
            ViewBag.TotalPage = svArticle.TotalPage;
            if (svArticle.TotalPage.ToString().Length > 2)
                ViewBag.TotalPage = String.Format("{0:0,0}", svArticle.TotalPage);
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? Type = 0, int? PIndex = 0, int? PSize = 0)
        {
            SelectList_PageSize();
            #region Article
            sqlWhere = "RowFlag > 0";
            if (Type > 0)
            {
                sqlWhere += " And ArticleTypeID=" + Type;
            }
            var Article = svArticle.SelectData<view_b2bArticle>("ArticleID,ArticleName,ModifiedDate", sqlWhere, "ModifiedDate DESC", (int)PIndex, (int)PSize);
            ViewBag.Article = Article;
            #endregion

            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.Type = Type;
            ViewBag.TotalRow = svArticle.TotalRow;
            if (svArticle.TotalRow.ToString().Length > 2)
                ViewBag.TotalRow = String.Format("{0:0,0}", svArticle.TotalRow);

            ViewBag.TotalPage = svArticle.TotalPage;
            if (svArticle.TotalPage.ToString().Length > 2)
                ViewBag.TotalPage = String.Format("{0:0,0}", svArticle.TotalPage);

            return PartialView("UC/ArticleListUC");
        }
        public ActionResult Detail(int? id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            GetEnumServiceType();
            GetStatusUser();
            #region Article
            sqlWhere = "ArticleID=" + id;
            var ArticleDetail = svArticle.SelectData<view_b2bArticle>("*", sqlWhere, "").First();
            ViewBag.ArticleDetail = ArticleDetail;

            var CreatedBy = svCompany.SelectData<view_Company>("CompID,DisplayName,CompName,LogoImgPath", "CompID=" + ArticleDetail.CompID, "").First();
            ViewBag.CreatedBy = CreatedBy;
            #endregion
            return View();
        }

    }
}
