using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Category;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.Common;
using System.Collections;
//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Article;
using System.Web.Configuration;
using Ouikum.Buylead;

namespace Ouikum.Controllers
{
    public partial class CompWebsiteController : BaseController
    {
        public string PathWebsiteHome { get; set; }

        #region Member
        CategoryService svCategory;
        ProductGroupService svProductGroup;
        ProductService svProduct;
        CompanyService svCompany;
        ProductImageService svProductImage;
        //AddressService svAddress;
        #endregion

        #region Menu
        [HttpPost]
        public ActionResult Menu(int? CompID, int? CateID, int? GroupID)
        {
            if (CompID > 0)
            {
                svProductGroup = new ProductGroupService();
                svCategory = new CategoryService();
                GetStatusWebsite();
                string sqlWhere = "IsDelete = 0 AND ProductCount>0 AND CompID =" + CompID;
                var ProductGroup = svProductGroup.SelectData<view_ProductGroup>(" * ", sqlWhere, " ListNo ASC, ProductGroupName ASC");
                ViewBag.GroupID = GroupID != null ? GroupID : 0;
                ViewBag.CateID = CateID != null ? CateID : 0;
                if (svProductGroup.TotalRow > 0)
                {
                    ViewBag.ProductGroup = ProductGroup;

                    return PartialView("UC/ProductGroupMenu");
                }
                else
                {

                    ViewBag.IndrustryCategories = svCategory.ListIndrustryCategory((int)CompID);
                    //ViewBag.WholesaleCategory = svCategory.ListWholesaleCategory((int)CompID); 

                    return PartialView("UC/CategoriesMenu");
                }

            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
        }
        #endregion

        #region DefaultWebsite
        public int DefaultWebsite(int id, string page)
        {
            svCompany = new CompanyService();
            string sqlWhere = string.Empty;
            string sqlSelect = "CompID,CompName,CompCode,CompLevel,emCompID,LogoImgPath,CompWebsiteTemplate,ViewCount,ProductCount,CompHistory,IsOnline,CompAddrLine1,CompPhone,ProvinceName";
            if (LogonRowFlag == 3)
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.All, id);
            }
            else
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.Online, id);
            }
            //var sqlwhereinweb = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwhereinweb = " AND WebID = 1 "; break;
            //    case "AntCart": sqlwhereinweb = " AND WebID = 3 "; break;
            //    case "myOtopThai": sqlwhereinweb = " AND WebID = 5 "; break;
            //    case "AppstoreThai": sqlwhereinweb = " AND WebID = 6 "; break;
            //    default: sqlwhereinweb = ""; break;
            //}
            //sqlWhere += sqlwhereinweb;

            var countcompany = svCompany.CountData<view_Company>(sqlSelect, sqlWhere);


            if (countcompany > 0)
            {
                ViewBag.LogonCompID = LogonCompID;
                AddViewCount(id, "Supplier");
                var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                ViewBag.WebCompID = company.CompID;
                ViewBag.WebCompName = company.CompName;
                ViewBag.CompCode = company.CompCode;
                ViewBag.WebCompLevel = company.CompLevel;
                ViewBag.WebLogoImgPath = company.LogoImgPath;
                ViewBag.ContactPhone = company.ContactPhone;
                ViewBag.Template = company.CompWebsiteTemplate;
                ViewBag.WebEmCompID = company.emCompID;
                ViewBag.PageAction = page;
                //if (company.ProvinceName == "กรุงเทพมหานคร")
                //{
                //    company.ProvinceName = "กรุงเทพ";
                //}
                ViewBag.title = company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                ViewBag.ViewCount = company.ViewCount;
                ViewBag.WebCompHistory = company.CompHistory;
                ViewBag.IsOnline = company.IsOnline;
                ViewBag.MetaDescription = company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
                ViewBag.MetaKeyword = ViewBag.title;
                //ViewBag.ProductCount = company.ProductCount;
                ProductCount(ViewBag.WebCompID);
                BlogCount(ViewBag.WebCompID);
                if (ViewBag.WebCompLevel == 3)
                {
                    OrderCount(ViewBag.WebCompID);
                    CertifyCount(ViewBag.WebCompID);
                    JobCount(ViewBag.WebEmCompID);
                }

                SetStatusWebsite((int)company.CompID, company.CompName);

                //var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 ", "ListNo ASC");
                var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0  and CompID = " + id, "ListNo ASC");
                if (SettingMenu.Count == 0)
                {
                    SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC");
                }
                else
                {
                    var where = "IsDelete = 0 and ([IsDefaultMenu] = 1 ";
                    foreach (var list in SettingMenu)
                    {
                        where += " or (FromMenuID = " + list.FromMenuID + " AND CompID = " + id + " AND IsShow = 1)";
                    }
                    where += ")";
                    foreach (var list in SettingMenu)
                    {
                        where += "AND CompMenuID != " + list.FromMenuID;
                    }
                    SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", where, "ListNo ASC");
                }

                ViewBag.SettingMenu = SettingMenu;

            }
            return countcompany;
        }
        #endregion

        #region Check Count Data

        #region Product
        public void ProductCount(int? id)
        {
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            svProduct = new ProductService();
            var products = 0;
            //if (LogonCompID == id)
            //{
            //    string sqlSelect = "CompID,ProductID,ProductName";
            //    string sqlWhere = svProduct.CreateWhereAction(ProductAction.WebSite, id);
            //    products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
            //    if (products > 0)
            //    {
            //        ViewBag.ProductCount = products;
            //        ViewBag.ProductType = 1;
            //    }
            //}
            //else
            //{
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, id);
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
            //}

        }
        #endregion

        #region Order
        public void OrderCount(int? id)
        {
            svCompany = new CompanyService();

            #region Order
            string sqlSelectOrder = "CompPaymentID,AccNo,AccName,AccType,BankName,BranchName";
            string sqlWhereOrder = "CompID =" + id + " AND IsDelete = 0";
            var payment = svCompany.SelectData<view_CompanyPayment>(sqlSelectOrder, sqlWhereOrder).Count();
            ViewBag.PaymentCount = payment;

            sqlSelectOrder = "CompShipmentID,ShipmentName,ShipmentDuration,PackingName,Remark";
            sqlWhereOrder = "CompID =" + id + " AND IsDelete = 0";
            var shipment = svCompany.SelectData<b2bCompanyShipment>(sqlSelectOrder, sqlWhereOrder).Count();
            ViewBag.ShipmentCount = shipment;
            #endregion

        }
        #endregion

        #region Certify
        public void CertifyCount(int? id)
        {
            #region Certify
            svCompany = new CompanyService();
            string sqlSelectCertify = "CompCertifyID,CompID,CompName,CertifyName,CertifyImgPath";
            string sqlWhereCertify = "IsDelete = 0 AND CompID =" + id;
            var certify = svCompany.CountData<view_CompanyCertify>(sqlSelectCertify, sqlWhereCertify);
            ViewBag.CertifyCount = certify;
            #endregion
        }
        #endregion

        #region Job
        public void JobCount(int? emCompID)
        {
            svJob = new JobService();
            string sqlSelect = "JobID, JobName";
            string sqlWhere = "CompID =" + emCompID + " AND IsDelete = 0";
            var emJobs = svJob.CountData<view_emJob>(sqlSelect, sqlWhere);
            ViewBag.CountJob = emJobs;

        }
        #endregion

        #region Blog
        public void BlogCount(int? CompID)
        {
            var svArticle = new emArticleService();
            string sqlSelect = "ArticleID, ArticleName";
            string sqlWhere = "CompID =" + CompID + " AND IsDelete = 0";
            var emArticles = svArticle.CountData<b2bArticle>(sqlSelect, sqlWhere);
            ViewBag.CountArticle = emArticles;
        }
        #endregion

        #region SelectCompanyContactInfo
        public void SelectCompanyContactInfo(int compid, string select)
        {
            string sqlSelect = "CompID,CompName,CompLevel,CompPhone,LogoImgPath,BizTypeID,BizTypeName,BizTypeOther,MainCustomer,CompProduct,CompAddrLine1,CompAddrLine2,CompSubDistrict,CompDistrictName,CompProvinceName,CompPostalCode,CompHistory,EmployeeCount,ServiceType";
            if (!string.IsNullOrEmpty(select))
            {
                sqlSelect += "," + select;
            }
            string sqlWhere = "IsDelete = 0 AND CompID =" + compid;
            //var sqlwhereinweb = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwhereinweb = " AND WebID = 1 "; break;
            //    case "AntCart": sqlwhereinweb = " AND WebID = 3 "; break;
            //    case "myOtopThai": sqlwhereinweb = " AND WebID = 5 "; break;
            //    case "AppstoreThai": sqlwhereinweb = " AND WebID = 6 "; break;
            //    default: sqlwhereinweb = ""; break;
            //}
            //sqlWhere += sqlwhereinweb;
            var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
            //if (company.ProvinceName == "กรุงเทพมหานคร")
            //{
            //    company.ProvinceName = "กรุงเทพ";
            //}
            GetStatusByCompID(LogonCompID);
            ViewBag.title = res.Common.lblContac_us + " " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.MetaDescription = res.Common.lblContac_us + " " + company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
            ViewBag.MetaKeyword = ViewBag.title;
            ViewBag.Company = company;
        }
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index(string CompanyName, int? id, int? GroupID, int? CateID, int? CateLevel, string textsearch)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            RememberURL();

            string page = "Home";
            int compid = DataManager.ConvertToInteger(id);
            if (id > 0)
            {
                int countcompany = DefaultWebsite(compid, page);

                if (countcompany > 0)
                {
                    SetPager();
                    SelectList_PageSize();
                    ViewBag.PageIndex = 1;
                    ViewBag.PageSize = 20;
                    ViewBag.TotalRow = 0;
                    ViewBag.GroupID = GroupID != null ? GroupID : 0;
                    ViewBag.CateID = CateID != null ? CateID : 0;
                    ViewBag.CateLevel = CateLevel != null ? CateLevel : 0;
                    ViewBag.CompID = id;
                    ViewBag.TextSearch = textsearch;

                    if (LogonCompID == ViewBag.CompID)
                    {
                        List_DoloadData(ProductAction.WebSite);
                    }
                    else
                    {
                        List_DoloadData(ProductAction.FrontEnd);
                    }

                    int CompID = compid;

                    svProduct = new ProductService();
                    GetStatusUser();

                    #region Select Recommand Product
                    //,LogoImgPath,ContactEmail
                    string sqlSelect = "CompID,CompName,ProductID,ProductName,ProductImgPath,RowFlag,ListNo,ViewCount,Price,Qty";

                    //var sqlwherein = "";
                    //switch (res.Common.lblWebsite)
                    //{
                    //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
                    //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
                    //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
                    //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
                    //    default: sqlwherein = ""; break;
                    //}

                    #region DoWhereCause
                    string sqlWhere = svProduct.CreateWhereAction(ProductAction.Recommend, CompID);

                    #endregion

                    var countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                    if (countproducts != 0)
                    {
                        var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, " ListNo ASC ", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = svProduct.TotalRow;
                    }
                    else
                    {
                        sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, CompID);
                        countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                        var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = countproducts;
                    }
                    #endregion

                    #region Select Company Info
                    sqlSelect = "CompID,ServiceType,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate";
                    sqlWhere = "CompID =" + id;
                    //var sqlwhereinweb = "";
                    //switch (res.Common.lblWebsite)
                    //{
                    //    case "B2BThai": sqlwhereinweb = " AND WebID = 1 "; break;
                    //    case "AntCart": sqlwhereinweb = " AND WebID = 3 "; break;
                    //    case "myOtopThai": sqlwhereinweb = " AND WebID = 5 "; break;
                    //    case "AppstoreThai": sqlwhereinweb = " AND WebID = 6 "; break;
                    //    default: sqlwhereinweb = ""; break;
                    //}
                    //sqlWhere += sqlwhereinweb;
                    var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                    ViewBag.Company = company;
                    ViewBag.CompID = id;
                    ViewBag.PageType = "Index";
                    return View();
                    #endregion
                   
                }
                else
                {
                    return Redirect(res.Pageviews.PvNotFound);
                }
            }
            else
            {
                return Redirect(res.Pageviews.PvErrorPages);
            }

        }
        #endregion

        #region Post Index
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);

            svProduct = new ProductService();
            var products = 0;
            if (LogonCompID == Convert.ToInt32(form["CompID"]))
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.WebSite, Convert.ToInt32(form["CompID"]));
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
            }
            else
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, Convert.ToInt32(form["CompID"]));
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
            }

            if (LogonCompID == ViewBag.CompID)
            {
                List_DoloadData(ProductAction.WebSite);
            }
            else
            {
                List_DoloadData(ProductAction.FrontEnd);
            }

            return PartialView("UC/IndexGalleryUC");
        }

        #endregion

        #region List_DoloadData
        public void List_DoloadData(ProductAction action)
        {
            svProduct = new ProductService();
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            string sqlSelect = "CompID,CompName,ProductID,ProductName,Price,Qty,ProductImgPath,ViewCount";
            string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, (int)ViewBag.CompID);

            string sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause((int)ViewBag.CompID, ViewBag.TextSearch, 0, (int)ViewBag.GroupID,
                (int)ViewBag.CateLevel, (int)ViewBag.CateID);

            #endregion

            var products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            if (products != null)
            {
                if (products.Count() > 0)
                {
                    ViewBag.Products = products;
                    ViewBag.TotalRow = svProduct.TotalRow;
                    ViewBag.TotalPage = svProduct.TotalPage;
                }
                else
                {
                    ViewBag.Products = null;
                    ViewBag.Message = "ไม่มีสินค้า";
                }
            }
            else
            {
                ViewBag.Products = null;
                ViewBag.Message = res.Message_Center.lblNoresult;
            }
            int size = 1;
            if (products.Count() <= 6)
            {
                size = 2;
            }
            else if (products.Count() <= 9)
            {
                size = 3;
            }
            else if (products.Count() <= 12)
            {
                size = 4;
            }
            else if (products.Count() <= 15)
            {
                size = 5;
            }
            else
            {
                size = 7;
            }
            var slideproduct = svProduct.SelectData<view_SearchProduct>("CompID,CompName,ProductID,ProductName,ProductImgPath", sqlWhere, "ProductName,ProductImgPath", 1, size, true);
            if (slideproduct != null)
            {
                if (slideproduct.Count() > 0)
                {
                    ViewBag.SlideProduct = slideproduct;
                }
                else
                {
                    ViewBag.SlideProduct = null;
                    ViewBag.Message = "ไม่มีสินค้า";
                }
            }
            else
            {
                ViewBag.SlideProduct = null;
                ViewBag.Message = res.Message_Center.lblNoresult;
            }
        }
        #endregion

        #region List_DoloadBlog
        public void List_DoloadBlog(BlogAction action, int id)
        {
            svProduct = new ProductService();

            string sqlOrderBy = "ModifiedDate DESC";

            string sqlSelect = "ArticleID, ArticleName, ImgPath, ShortDescription, CompID, ViewCount";
            string sqlWhere = "CompID =" + id + " AND IsDelete = 0";
            var a = ViewBag.TextSearchBlog;
            if (!string.IsNullOrEmpty(ViewBag.TextSearchBlog))
            {
                sqlWhere += "AND ArticleName LIKE N'%" + ViewBag.TextSearchBlog + "%'";
            }

            var Articles = svProduct.SelectData<b2bArticle>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            string sqlSelect_comp = "CompName";
            string sqlWhere_comp = "CompID =" + id;
            var ID = ViewBag.CompID; //เอามารับค่า id
            var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
            ViewBag.WebCompName = (string)company.CompName;
            ViewBag.PageType = "Blog";
            if (Articles != null)
            {
                if (Articles.Count() > 0)
                {
                    ViewBag.Articles = Articles;
                    ViewBag.TotalRow = svProduct.TotalRow;
                    ViewBag.TotalPage = svProduct.TotalPage;
                }
                else
                {
                    ViewBag.Products = null;
                    ViewBag.Message = res.Message_Center.lblNoresult;
                }
            }
            else
            {
                ViewBag.Products = null;
                ViewBag.Message = res.Message_Center.lblNoresult;
            }
        }
        #endregion

        #region Path Website
        public void LinkPathCompanyWebsite(string CompName, int CompID)
        {
            string replaceUrlCompname = Url.ReplaceUrl(CompName);
            PathWebsiteHome = "~/CompWebsite/" + replaceUrlCompname + "/Main/Index/" + CompID;
        }
        #endregion

        #region SettingMenu

        public void CompanySetting(int? id)
        {
            var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 ", "ListNo ASC");
            var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0  and CompID = " + LogonCompID, "ListNo ASC");
            if (SetMenu.Count == 0)
            {
                SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC");
            }
            else
            {
                var where = "IsDelete = 0 ";
                foreach (var list in SettingMenu)
                {
                    where += "and CompMenuID != " + list.FromMenuID;
                }
                SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", where, "ListNo ASC");
            }

            ViewBag.SettingMenu = SettingMenu;
        }
        #region Get: Setting

        public ActionResult CompanySetting()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CompLevel = LogonCompLevel;
                GetStatusUser();
                SetPager();
                if (LogonCompLevel == 3)
                {
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvMemberSignIn);
                }


            }

        }

        #endregion

        //#region Post: Setting
        //[HttpPost, ValidateInput(false)]
        //public ActionResult CompanySetting(FormCollection form)
        //{
        //    SelectList_PageSize();
        //    SetPager(form);
        //    var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
        //    var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and MenuName LIKE N'%" + form["MenuName"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
        //    if (SetMenu.Count == 0)
        //    {
        //        SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
        //    }
        //    else
        //    {
        //        var where = "IsDelete = 0 ";
        //        foreach (var list in SettingMenu)
        //        {
        //            where += "and CompMenuID != " + list.FromMenuID;
        //        }
        //        SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", where, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
        //    }
        //    ViewBag.SettingMenu = SettingMenu;
        //    ViewBag.TotalPage = svCompany.TotalPage;
        //    ViewBag.TotalRow = svCompany.TotalRow;
        //    return PartialView("UC/NavBar/Website");
        //}

        //#endregion

        #endregion

        public void GetStatusByCompID(int CompID)
        {
            var svBuylead = new BuyleadService();

            string sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.Junk, CompID);
            var CountJunk = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountJunk = CountJunk;

            sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.BackEnd, CompID) + " AND ListNo = 0 ";
            var CountBuylead = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountBuylead = CountBuylead;

            sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.All, CompID);
            var CountAllitem = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountAllitem = CountAllitem;

            sqlwhere += svBuylead.CreateWhereCause(0, "", 4);
            var CountBuyleadApprove = svBuylead.CountData<view_BuyLead>("BuyleadID", sqlwhere);
            ViewBag.CountBuyleadApprove = CountBuyleadApprove;
        }

        public ActionResult GetStatus()
        {
            GetStatusByCompID(LogonCompID);
            return Json(new
            {
                CountJunk = (int)ViewBag.CountJunk,
                CountBuylead = (int)ViewBag.CountBuylead,
                CountBuyleadApprove = (int)ViewBag.CountBuyleadApprove,
                CountBuyleadWait = ((int)ViewBag.CountAllitem - (int)ViewBag.CountBuyleadApprove),
                CountAllitem = (int)ViewBag.CountAllitem
            });
        }
        #endregion

        public string ProductID { get; set; }
    }
}

