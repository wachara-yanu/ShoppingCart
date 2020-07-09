using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum;
using Ouikum.Article;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
namespace Ouikum.Web.Admin
{
    public class BlogController : BaseController
    {
        //
        // GET: /Admin/Blog/ 

        #region Blog
        

        #region Get : Blog
        public ActionResult Blog()
        {
            var svArticle = new ArticleService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                var ArticleTypes = svArticle.SelectData<b2bArticleType>("ArticleTypeID,ArticleTypeName", "IsDelete = 0");
                ViewBag.ArticleTypes = ArticleTypes;
                ViewBag.CompLevel = LogonCompLevel;
                return View();
            }
        }
        #endregion

        #region Post : Blog
        [HttpPost]
        public ActionResult Blog(FormCollection form)
        {
            var svCompany = new Company.CompanyService();
            var svArticle = new ArticleService();
            SelectList_PageSize();
            List<view_b2bArticle> Blogs;
            SetPager(form);
            if (!string.IsNullOrEmpty(form["SearchBlog"]))
            {
                var ArticleTypeID = "";
                if (form["SearchType"] == "ArticleName")
                {
                    Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleName LIKE N'%" + form["SearchBlog"] + "%' and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
                else
                {
                    var articletype = svArticle.SelectData<b2bArticleType>("ArticleTypeID,ArticleTypeName", "ArticleTypeName LIKE N'%" + form["SearchBlog"] + "%'");
                    foreach (var it in (List<b2bArticleType>)articletype)
                    {
                        ArticleTypeID = ArticleTypeID + "ArticleTypeID = " + it.ArticleTypeID.ToString() + " or ";
                    }
                    if (ArticleTypeID != "")
                    {
                        ArticleTypeID = ArticleTypeID.Substring(0, ArticleTypeID.Length - 4);
                        Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and (" + ArticleTypeID + ") and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                    }
                    else
                    {
                        Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleType = 0 and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                    }
                }
            }
            else
            {
                Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and CompID = " + LogonCompID, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }


            ViewBag.Blogs = Blogs;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/BlogGrid");
        }
        #endregion

        #region SaveBlog
        [HttpPost, ValidateInput(false)]
        public bool SaveBlog(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var Articles = new b2bArticle();
            var emArticles = new emArticle();
            var svMember = new Common.MemberService();
            var svCompany = new Company.CompanyService();
            if (objState == 2)// update
            {
                Articles = svCompany.SelectData<b2bArticle>("*", " ArticleID = " + form["ArticleID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bArticle
            var ArticleImgPath = Articles.ImgPath;
            Articles.CompID = DataManager.ConvertToInteger(LogonCompID);
            Articles.ArticleName = form["ArticleName"];
            Articles.ArticleTypeID = DataManager.ConvertToInteger(form["ArticleTypeID"]);
            Articles.Description = ReplaceText(form["Description"]);
            Articles.ShortDescription = ReplaceText(form["ShortDescription"]);
            Articles.ImgPath = form["ImgPath"];
            Articles.PageTitle = form["PageTitle"];
            if (objState == 2)// update
            {
                Articles.RowVersion = DataManager.ConvertToShort(Articles.RowVersion + 1);
                emArticles.RowVersion = DataManager.ConvertToShort(emArticles.RowVersion + 1);
            }
            else
            {
                Articles.ViewCount = 0;
                Articles.RowFlag = 1;
                Articles.RowVersion = 1;
                Articles.CreatedBy = "sa";
                Articles.ModifiedBy = "sa";
                Articles.ModifiedDate = DateTime.Now;
                Articles.CreatedDate = DateTime.Now;
            }
            #endregion

            #region Save b2bArticle
            Articles = svCompany.SaveData<b2bArticle>(Articles, "ArticleID");
            if (svCompany.IsResult)
            {
                if (objState == 2)// update
                {
                    emArticles.RowVersion = Articles.RowVersion;
                }
                else
                {
                    emArticles.CompID = DataManager.ConvertToInteger(Request.Cookies[res.Common.lblWebsite].Values["emCompID"]);
                    emArticles.ArticleName = Articles.ArticleName;
                    emArticles.ArticleTypeID = Articles.ArticleTypeID;
                    emArticles.Description = Articles.Description;
                    emArticles.ShortDescription = Articles.ShortDescription;
                    emArticles.ImgPath = Articles.ImgPath;
                    emArticles.PageTitle = Articles.PageTitle;
                    emArticles.ViewCount = Articles.ViewCount;
                }

                #region Save emArticle
                emArticles = svMember.SaveData<emArticle>(emArticles, "ArticleID");
                #endregion

                if (svCompany.IsResult && svMember.IsResult)
                {
                    #region SaveArticleImg
                    if (!string.IsNullOrEmpty(form["ImgPath"]))
                    {
                        if (Articles.ImgPath != ArticleImgPath)
                        {
                            imgManager = new FileHelper();
                            imgManager.DirPath = "Article/" + LogonCompID + "/" + Articles.ArticleID;
                            imgManager.DirTempPath = "Temp/Article/" + LogonCompID;
                            imgManager.ImageName = form["ImgPath"];
                            //imgManager.ImageThumbName = "Thumb_" + form["ImgPath"];
                            imgManager.FullHeight = 0;
                            imgManager.FullWidth = 0;
                            imgManager.ThumbHeight = 150;
                            imgManager.ThumbWidth = 150;

                            imgManager.SaveImageFromTemp();
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return svCompany.IsResult;
        }
        #endregion

        #region EditBlog
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBlog(FormCollection form)
        {
            var Articles = new b2bArticle();
            var svCompany = new Company.CompanyService();
            if (!string.IsNullOrEmpty(form["ArticleID"]))
            {
                Articles = svCompany.SelectData<b2bArticle>("*", " IsDelete = 0 AND ArticleID =" + form["ArticleID"]).First();
            }
            return Json(new { ArticleID = Articles.ArticleID, CompID = Articles.CompID, RowVersion = Articles.RowVersion, ArticleTypeID = Articles.ArticleTypeID, ArticleName = Articles.ArticleName, pageTitle = Articles.PageTitle, ImgPath = Articles.ImgPath, Description = Articles.Description, ShortDescription = Articles.ShortDescription });
        }
        #endregion

        #endregion

    }
}
