using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Company;
using Ouikum.Article;
//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Product;

namespace Ouikum.Controllers
{
    public partial class WebsiteController : BaseController
    {
        #region Member
        ArticleService svArticle;
        #endregion

        #region Blog List
        [HttpGet]
        public ActionResult Blog(int? id, int? GroupID, int? CateID, int? CateLevel, string SearchBlog)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Blog";
            int countcompany = DefaultWebsite((int)id, page);

            if (countcompany > 0)
            {
                ViewBag.PageIndex = 1;
                ViewBag.PageSize = 20;
                ViewBag.TotalRow = 0;
                ViewBag.GroupID = GroupID != null ? GroupID : 0;
                ViewBag.CateID = CateID != null ? CateID : 0;
                ViewBag.CateLevel = CateLevel != null ? CateLevel : 0;
                ViewBag.CompID = id;
                ViewBag.TextSearchBlog = SearchBlog;
                string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType,CompWebsiteCss,ProvinceName";
                string sqlWhere_comp = "CompID =" + id;

                var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                ViewBag.Company = company;
                ViewBag.titleBlog = "บทความ " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                ViewBag.PageType = "Blog";

                if (company.CompWebsiteCss == null)
                    company.CompWebsiteCss = 0;

                ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;
                if (company.CompWebsiteCss == 1)
                {
                    if (LogonCompID == ViewBag.CompID)
                    {
                        List_DoloadBlog(BlogAction.WebSite, (int)id);
                    }
                    else
                    {
                        List_DoloadBlog(BlogAction.FrontEnd, (int)id);
                    }
                }
                else
                {
                    if (LogonCompID == ViewBag.CompID)
                    {
                        List_DoloadBlogData(ProductAction.WebSite);
                    }
                    else
                    {
                        List_DoloadBlogData(ProductAction.FrontEnd);
                    }
                }

                svArticle = new ArticleService();
                GetStatusUser();

                string sqlSelect = "ArticleID, ArticleName, ImgPath, ShortDescription, CompID, ViewCount";
                string sqlWhere = "CompID =" + id + " AND IsDelete = 0";
                var emArticles = svArticle.SelectData<b2bArticle>(sqlSelect, sqlWhere, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                ViewBag.emArticles = emArticles;
                ViewBag.TotalRow = svArticle.TotalRow;
                ViewBag.TotalPage = svArticle.TotalPage;

                return View();
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
            
        }
        #endregion

        #region Post Blog
        [HttpPost]
        public ActionResult Blog(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);
            BlogCount(ViewBag.CompID);
            if (LogonCompID == ViewBag.CompID)
            {
                List_DoloadBlogData(ProductAction.WebSite);
            }
            else
            {
                List_DoloadBlogData(ProductAction.FrontEnd);
            }



            return PartialView("UCStyle1/BlogUC");
        }

        #endregion 

        #region Post : Blog Style2
        [HttpPost]
        public ActionResult BlogStyle2(FormCollection form)
        {
            svCompany = new CompanyService();
            SelectList_PageSize();
            List<view_b2bArticle> Blogs;
            SetPager(form);
            if (!string.IsNullOrEmpty(form["SearchBlog"]))
            {
                Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleName LIKE N'%" + form["SearchBlog"] + "%' and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else
            {
                Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }

            if (LogonCompID == ViewBag.CompID)
            {
                List_DoloadBlog(BlogAction.WebSite, Convert.ToInt32(form["hidCompID"]));
            }
            else
            {
                List_DoloadBlog(BlogAction.FrontEnd, Convert.ToInt32(form["hidCompID"]));
            }

            return PartialView("UCStyle2/BlogGalleryUC");

        }
        #endregion


        #region Blog Detail
        public ActionResult BlogDetail(int id, int BlogID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Blog";
            int countcompany = DefaultWebsite(id, page);
            if (countcompany > 0)
            {

                svArticle = new ArticleService();
                svCompany = new CompanyService();

                string sqlSelect = "ArticleID, ArticleName, ImgPath, Description, CompID, ViewCount";
                string sqlWhere = "IsDelete = 0 AND ArticleID = " + BlogID;
                var count = svArticle.CountData<b2bArticle>(sqlSelect, sqlWhere);

                if (count > 0)
                {
                    AddViewCount(BlogID, "Blog");

                    GetStatusUser();

                    var article = svArticle.SelectData<b2bArticle>(sqlSelect, sqlWhere).First();
                    ViewBag.Article = article;

                    sqlSelect = "ArticleID, ArticleName ,CompID";
                    sqlWhere = "IsDelete = 0 AND CompID = " + id;
                    var emArticles = svArticle.SelectData<b2bArticle>(sqlSelect, sqlWhere);
                    ViewBag.emArticles = emArticles;

                    ViewBag.blogtitle = article.ArticleName + " | " + ViewBag.WebCompName + " | " + "-B2BThai.com";

                    var select = "";
                    SelectCompanyContactInfo(id, select);
                    string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType,CompWebsiteCss";
                    string sqlWhere_comp = "CompID =" + id;

                    var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                    ViewBag.Company = company;
                    ViewBag.PageType = "Blog";

                    if (company.CompWebsiteCss == null)
                        company.CompWebsiteCss = 0;

                    ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;

                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvNotFound);
                }
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }

        }
        #endregion

    }
}
