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
    public partial class WebsiteController : BaseController
    {
        public string PathWebsiteHome { get; set; }

        #region Member
        CategoryService svCategory;
        ProductGroupService svProductGroup;
        ProductService svProduct; 
        CompanyService svCompany;
        ProductImageService svProductImage;
        #endregion

        #region DefaultWebsite
        public int DefaultWebsite(int id, string page)
        {
            svCompany = new CompanyService();
            string sqlWhere = string.Empty;
            string sqlSelect = "CompID,CompName,CompCode,CompLevel,emCompID,LogoImgPath,CompWebsiteTemplate,ViewCount,ProductCount,CompHistory,IsOnline,CompAddrLine1,CompPhone,ProvinceName,CreatedDate";
            if(LogonRowFlag == 3){                
                sqlWhere = svCompany.CreateWhereAction(CompStatus.All,id);
            }
            else
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.Online, id);
            }
            var countcompany = svCompany.CountData<view_Company>(sqlSelect, sqlWhere);

            if (countcompany > 0)
            {
                AddViewCount(id, "Supplier");
                ViewBag.LogonCompID = LogonCompID;
                var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                ViewBag.WebCompID = company.CompID;
                ViewBag.WebCompName = company.CompName;
                ViewBag.CompCode = company.CompCode;
                ViewBag.WebCompLevel = company.CompLevel;
                ViewBag.WebLogoImgPath = company.LogoImgPath;
                ViewBag.Template = company.CompWebsiteTemplate;
                ViewBag.WebEmCompID = company.emCompID;
                ViewBag.PageAction = page;
                ViewBag.title = company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                ViewBag.ViewCount = company.ViewCount;
                ViewBag.WebCompHistory = company.CompHistory;
                ViewBag.IsOnline = company.IsOnline;
                ViewBag.MetaDescription = company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
                ViewBag.MetaKeyword = ViewBag.title;
                ProductCount(ViewBag.WebCompID);
                BlogCount(ViewBag.WebCompID);
                if (ViewBag.WebCompLevel == 3)
                {
                    OrderCount(ViewBag.WebCompID);
                    CertifyCount(ViewBag.WebCompID);
                    JobCount(ViewBag.WebEmCompID);
                }

                SetStatusWebsite((int)company.CompID, company.CompName);

                var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0  and CompID = " + id, "ListNo ASC");
                if (SettingMenu.Count == 0)
                {
                    SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1 and IsShow = 1", "ListNo ASC");
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
        public void ProductCount(int? id){
            svProduct = new ProductService();
            var products = 0;
            string sqlSelect = "CompID,ProductID,ProductName";
            string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, id) ;
            sqlWhere += svProduct.CreateWhereCause((int)id);
            products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
            if (products > 0)
            {
                ViewBag.ProductCount = products;
                ViewBag.ProductType = 1;
            }
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

        #endregion

        #region SelectCompanyContactInfo
        public void SelectCompanyContactInfo(int compid, string select)
        {
            string sqlSelect = "CompID,CompCode,CompName,CompLevel,CompPhone,LogoImgPath,BizTypeID,BizTypeName,BizTypeOther,MainCustomer,CompProduct,CompAddrLine1,CompAddrLine2,CompSubDistrict,CompDistrictName,CompProvinceName,CompPostalCode,CompHistory,EmployeeCount,ServiceType,CompImgPath,CompShortDes,LineID,FacebookUrl,CreatedDate,CompWebsiteCss,ProvinceName";
            if (!string.IsNullOrEmpty(select))
            {
                sqlSelect += ","+select;
            }
            string sqlWhere = "IsDelete = 0 AND CompID =" + compid;
            var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
            if (company.ProvinceName == "กรุงเทพมหานคร")
            {
                company.ProvinceName = "กรุงเทพ";
            }
            ViewBag.title = res.Common.lblContac_us + " " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.titleAbout = "เกี่ยวกับเรา " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.titleOrder = "การชำระเงิน/การจัดส่ง " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.titleCertify = "ใบรับรอง/คุณภาพ " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.titleBlog = "บทความ " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.MetaDescription = res.Common.lblContac_us + " " + company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
            ViewBag.MetaKeyword = ViewBag.title;
            ViewBag.Company = company;
            if (company.CompWebsiteCss == null)
                company.CompWebsiteCss = 0;

            ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;
        }
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index(int? id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            RememberURL();

            int PageCheck = 1;
            CheckPage(id, PageCheck);

            string page = "Home";
            int compid = DataManager.ConvertToInteger(id);
            if (id > 0)
            {
                int countcompany = DefaultWebsite(compid, page);

                if (countcompany > 0)
                {
                    ViewBag.PageIndex = 1;
                    ViewBag.PageSize = 20;

                    int CompID = compid;

                    svProduct = new ProductService();

                    #region Select Recommand Product

                    string sqlSelect = "CompID,CompName,ProductID,ProductName,ProductImgPath,RowFlag,ListNo,Price";

                    #region DoWhereCause
                    string sqlWhere = svProduct.CreateWhereAction(ProductAction.Recommend, CompID);
                    #endregion

                    var countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                    if (countproducts != 0)
                    {
                        var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, " ListNo ASC ", ViewBag.PageIndex, 10);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = svProduct.TotalRow;
                    }
                    else
                    {
                        sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, CompID) ;
                        countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                        var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "ModifiedDate DESC", ViewBag.PageIndex, 10);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = countproducts;
                    }
                    #endregion

                    #region Select Company Info
                    sqlSelect = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,LineID,FacebookUrl,CreatedDate,CompWebsiteCss,ContactEmail";
                    sqlWhere = "CompID =" + id;

                    var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                    ViewBag.Company = company;
                    
                    if (company.CompWebsiteCss == null)
                        company.CompWebsiteCss = 0;

                    ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;

                    if (company.CompWebsiteCss == 1)
                    {
                        ViewBag.TotalRow = 0;
                        ViewBag.GroupID = 0;
                        ViewBag.CateID = 0;
                        ViewBag.CateLevel = 0;
                        ViewBag.CompID = id;

                        if (LogonCompID == ViewBag.CompID)
                        {
                            List_DoloadData(ProductAction.WebSite);
                        }
                        else
                        {
                            List_DoloadData(ProductAction.FrontEnd);
                        }

                        ViewBag.PageType = "Index";

                    }
                    #endregion

                    GetStatusUser();
                    return View();
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

        #region List_DoloadData
        public void List_DoloadData(ProductAction action)
        {
            svProduct = new ProductService();
           
            string sqlSelect = "CompID,CompName,ProductID,ProductName,Price,Qty,QtyUnit,ProductImgPath";
            string sqlWhere = sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, (int)ViewBag.CompID);
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
                    ViewBag.WebCompName = products[0].CompName;
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

        #region List_DoloadBlogData
        public void List_DoloadBlogData(ProductAction action)
        {
            svArticle = new ArticleService();
            string sqlSelect = "ArticleID, ArticleName, ImgPath, ShortDescription, CompID, ViewCount";
            string sqlWhere = "IsDelete = 0 ";
            if ((int)ViewBag.CompID > 0)
            {
                sqlWhere += "AND (CompID = " + (int)ViewBag.CompID + ")";
            }
            string sqlOrderBy = " ListNo ASC ";

            var emArticles = svArticle.SelectData<b2bArticle>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            if (emArticles != null)
            {
                if (emArticles.Count() > 0)
                {
                    ViewBag.emArticles = emArticles;
                    ViewBag.TotalRow = svArticle.TotalRow;
                    ViewBag.TotalPage = svArticle.TotalPage;
                    var comp = svArticle.SelectData<b2bCompany>("CompID,CompName", "CompID = " + (int)ViewBag.CompID);
                    ViewBag.WebCompName = comp[0].CompName;
                }
                else
                {
                    ViewBag.emArticles = null;
                    ViewBag.Message = res.Message_Center.lblNoresult;
                }
            }
            else
            {
                ViewBag.emArticles = null;
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
            PathWebsiteHome = "~/CompanyWebsite/" + replaceUrlCompname + "/Main/Index/" + CompID;
        }
        #endregion

        #region SelectCompany
        public view_Company SelectCompany()
        {
            var company = new view_Company();
            if (CheckIsLogin())
            {

                var Companies = svCompany.SelectData<view_Company>("*", "IsDelete = 0 AND emCompID =" + LogonEMCompID, null, 0, 0, false);
                if (Companies.Count() > 0)
                {
                    company = Companies.First();

                }

            }
            return company;
        }
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
    }
}
