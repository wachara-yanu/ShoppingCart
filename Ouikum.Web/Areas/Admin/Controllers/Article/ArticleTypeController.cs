using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Telerik.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Common;
using Ouikum;
using Ouikum.Article;
using Ouikum.Company;
using res = Prosoft.Resource.Web.Ouikum;


namespace Ouikum.Web.Admin
{
    public class ArticleTypeController : BaseController
    {
        //
        // GET: /Admin/Article/
        ArticleService svArticle;
        CompanyService svCompany;

        #region Constructors
        public ArticleTypeController()
        {
            svArticle = new ArticleService();
        }
        #endregion

        #region List Type
        [HttpGet]
        public ActionResult ListType()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(11))
                {
                    GetStatusUser();
                    SetPager();
                    UserStatus();
                    ViewBag.Active = "Article Type";
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost]
        public ActionResult ListType(FormCollection form)
        {
            SelectList_PageSize();
            List<b2bArticleType> ArticlesType;
            SetPager(form);
            if (!string.IsNullOrEmpty(form["SearchArticleType"]))
            {
                var ArticleTypeID = "";
                if (form["SearchType"] != "0")
                {
                    ArticlesType = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 and ArticleTypeName LIKE N'%" + form["SearchArticleType"] + "%' AND ArticleTypeID = " + form["SearchType"], "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }

                else
                {
                    ArticlesType = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 and ArticleTypeName LIKE N'%" + form["SearchArticleType"] + "%'", "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
            }
            else
            {
                if (form["SearchType"] != "0")
                {
                    ArticlesType = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 AND ArticleTypeID = " + form["SearchType"], "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
                else
                {
                    ArticlesType = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0", "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
            }

            string sqlSelect = "CompID,CompName";
            string sqlWhere = "CompID = " + LogonCompID;
            var company = svArticle.SelectData<view_Company>(sqlSelect, sqlWhere).First();
            ViewBag.WebCompName = company.CompName;

            ViewBag.ArticlesType = ArticlesType;
            ViewBag.TotalPage = svArticle.TotalPage;
            ViewBag.TotalRow = svArticle.TotalRow;
            return PartialView("UC/ArticleTypeGrid");
        }
        #endregion

        #region New Type
        [HttpGet]
        public ActionResult NewType()
        {

            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(11))
                {
                    GetStatusUser();
                    UserStatus();

                    /*------Article-------*/
                    ViewBag.Active = "Article";
                    var ArticleTypes = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 AND ArticleTypeID IN(5,7)", "ArticleTypeID ASC");
                    ViewBag.ArticleTypes = ArticleTypes;

                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewType(FormCollection form)
        {
            var ArticlesType = new b2bArticleType();
            var emArticlesType = new emArticleType();
            int ArticleTypeID = DataManager.ConvertToInteger(form["ArticleTypeID"]);

            #region set ค่า b2bArticleType
            //Set Default
            ArticlesType.RowFlag = 1;
            ArticlesType.RowVersion = 1;
            ArticlesType.CreatedBy = "sa";
            ArticlesType.ModifiedBy = "sa";
            ArticlesType.ModifiedDate = DateTimeNow;
            ArticlesType.CreatedDate = DateTimeNow;
            // set Value
            ArticlesType.ImgPath = form["ImgPath"];
            ArticlesType.ArticleTypeName = form["ArticleTypeName"];
            ArticlesType.ArticleTypeID = ArticleTypeID;

            // new set
            ArticlesType.ListNo = DataManager.ConvertToInteger(form["ListNo"]);
            ArticlesType.IsShow = DataManager.ConvertToBool(form["IsShowArticle"]);
            #endregion

            #region Save b2bArticle
            ArticlesType = svArticle.SaveData<b2bArticleType>(ArticlesType, "ArticleTypeID");
            if (svArticle.IsResult)
            {
                emArticlesType.ArticleTypeName = ArticlesType.ArticleTypeName;
                emArticlesType.ArticleTypeID = ArticlesType.ArticleTypeID;
                emArticlesType.ImgPath = ArticlesType.ImgPath;

                // new save
                emArticlesType.ListNo = ArticlesType.ListNo;
                emArticlesType.IsShow = ArticlesType.IsShow;

                #region Save emArticle
                emArticlesType = svArticle.SaveData<emArticleType>(emArticlesType, "ArticleID");
                #endregion
            }
            #endregion

            return Redirect("~/Admin/Article/ListType");
        }
        #endregion

        #region CompNameList
        [HttpPost]
        public ActionResult CompanyList(string CompName)
        {
            var svCompany = new CompanyService();
            var Company = svCompany.SelectData<b2bCompany>("CompID,CompName,IsDelete,CompLevel", "IsDelete = 0 AND CompLevel = 3 AND CompName LIKE N'%" + CompName + "%'", "CompName");
            ViewBag.CompanyList = Company;
            return PartialView("UC/Article/ComNameListUC");
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult EditType(int ArticleTypeID)
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(11))
                {
                    if (ArticleTypeID > 0)
                    {
                        GetStatusUser();
                        /*------Article-------*/
                        ViewBag.Active = "Article";
                        ArticleService svArticle = new ArticleService();
                        b2bArticleType ArticleType = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 AND ArticleTypeID = " + ArticleTypeID, null, 0, 0, false).First();
                        List<b2bArticleType> ArticleTypes = svArticle.SelectData<b2bArticleType>("*", "IsDelete = 0 AND ArticleTypeID IN(5,7)", "ArticleTypeName ASC");
                        ViewBag.ArticleType = ArticleType;
                        ViewBag.ArticleTypes = ArticleTypes;

                        /*------Company-List------*/
                        var svCompany = new CompanyService();
                        var Company = svCompany.SelectData<b2bCompany>("CompID,CompName,IsDelete,CompLevel", "IsDelete = 0 AND CompLevel = 3", "CompName");
                        ViewBag.CompanyList = Company;

                        /*------Company-Name------*/
                        //var CompName = svCompany.SelectData<b2bCompany>("CompID,CompName", "CompID = " + Articles.CompID).First();
                        //ViewBag.CompName = CompName.CompName;

                        CompLevels();
                        UserStatus();

                        return View();
                    }
                    else
                    {
                        return Redirect("~/Admin/ArticleType/List");
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditType(FormCollection form)
        {
            var Articles = new b2bArticle();
            var emArticles = new emArticle();
            int ArticleTypeID = DataManager.ConvertToInteger(form["ArticleTypeID"]);
            int CompID = DataManager.ConvertToInteger(form["hideCompID"]);
            int OldCompID = DataManager.ConvertToInteger(form["OldCompID"]);

            Articles = svArticle.SelectData<b2bArticle>("*", " ArticleID = " + form["ArticleID"] + " AND RowVersion = " + form["RowVersion"]).First();
            
            #region set ค่า b2bArticle
            // set Value
            var ArticleImgPath = Articles.ImgPath;
            var a = form["ImgPath"];
            Articles.ArticleName = form["ArticleName"];
            Articles.ArticleTypeID = ArticleTypeID;
            Articles.Description = ReplaceText(form["Description"]);
            Articles.ShortDescription = ReplaceText(form["ShortDescription"]);
            if (!string.IsNullOrEmpty(form["ImgPath"]) && form["ImgPath"] != ArticleImgPath)
            {
                Articles.ImgPath = form["ImgPath"];
            }
            else
            {
                Articles.ImgPath = Articles.ImgPath;
            }
            Articles.PageTitle = form["PageTitle"];
            Articles.Owner = form["Owner"];
            Articles.Position = form["Position"];
            Articles.IsHot = DataManager.ConvertToBool(form["IsHot"]);
            if (ArticleTypeID == 7)
            {
                Articles.CompID = CompID;
            }
            else
            {
                Articles.CompID = DataManager.ConvertToInteger(Articles.CompID);
            }
            #endregion
            Articles.ModifiedDate = DateTime.Now;
            Articles.RowVersion = DataManager.ConvertToShort(Articles.RowVersion + 1);
            emArticles.RowVersion = DataManager.ConvertToShort(emArticles.RowVersion + 1);

            #region Save b2bArticle
            Articles = svArticle.SaveData<b2bArticle>(Articles, "ArticleID");
            if (svArticle.IsResult)
            {
                emArticles.ModifiedDate = DateTime.Now;
                emArticles.RowVersion = Articles.RowVersion;
    
                #region Save emArticle
                emArticles = svArticle.SaveData<emArticle>(emArticles, "ArticleID");
                #endregion

                if (svArticle.IsResult && svArticle.IsResult)
                {
                    #region SaveArticleImg
                    if (!string.IsNullOrEmpty(form["ImgPath"]))
                    {
                        /* Check New Company */
                        if (OldCompID != CompID)
                        {
                            /*  New Img */
                            if (Articles.ImgPath != ArticleImgPath)
                            {
                                imgManager = new FileHelper();
                                imgManager.DirPath = "Article/" + Articles.CompID + "/" + Articles.ArticleID;
                                imgManager.DirTempPath = "Temp/Article/" + LogonCompID;
                                imgManager.ImageName = form["ImgPath"];
                                imgManager.FullHeight = 0;
                                imgManager.FullWidth = 0;
                                imgManager.ThumbHeight = 150;
                                imgManager.ThumbWidth = 150;

                                imgManager.SaveImageFromTemp();
                            }
                            /*  Old Img */
                            else
                            {
                                var svBlobStorage = new BlobStorageService();
                                string fromFilePath = "Article/" + OldCompID + "/" + Articles.ArticleID + "/" + form["ImgPath"];
                                string descFilePath = "Article/" + Articles.CompID + "/" + Articles.ArticleID + "/" + form["ImgPath"];
                                svBlobStorage.CopyBlob(fromFilePath, descFilePath);
                            }
                        }
                        /* Check Old Company */
                        else
                        {
                            if (Articles.ImgPath != ArticleImgPath)
                            {
                                imgManager = new FileHelper();
                                imgManager.DirPath = "Article/" + Articles.CompID + "/" + Articles.ArticleID;
                                imgManager.DirTempPath = "Temp/Article/" + LogonCompID;
                                imgManager.ImageName = form["ImgPath"];
                                imgManager.FullHeight = 0;
                                imgManager.FullWidth = 0;
                                imgManager.ThumbHeight = 150;
                                imgManager.ThumbWidth = 150;

                                imgManager.SaveImageFromTemp();
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return  Redirect("~/Admin/Article/list");
        }
        #endregion

        #region Delete
        public ActionResult Delete()
        {
            return View();
        }
        #endregion

        /*--------------------BlogImg---------------------*/
        #region SaveBlogImg
        [HttpPost]
        public ActionResult SaveBlogImg(HttpPostedFileBase FileImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Article/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/Article/" + LogonCompID, FileImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveBlogImg
        public ActionResult RemoveBlogImg(string filenames)
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Article/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*------------------------DeleteData-----------------------------*/
        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ArticleID, List<short> RowVersion, string PrimaryKeyName)
        {
            if (PrimaryKeyName == "ArticleID")
            {
                svArticle.DelData<b2bArticle>(Check, ArticleID, RowVersion, PrimaryKeyName);
                if (svArticle.IsResult)
                {
                    svArticle.DeleteData<emArticle>(ArticleID, RowVersion, PrimaryKeyName);
                    svArticle.IsResult = svArticle.IsResult;
                }
            }
            if (svArticle.IsResult)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion
    }
}
