#region using System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml.Linq;
using System.Web.UI;
using System.Drawing;                       // Drawing Image
using System.Text;                          // StringBuilder
using System.IO;                            // StringWriter
using System.Text.RegularExpressions;       // Regex
using System.Runtime.Caching;               // CacheFC
using System.Configuration;
using System.Collections;
using System.Security.Cryptography;
using System.Net.NetworkInformation;        // MacAddress
using System.Net;                           // GetIP4Address()
#endregion
using Prosoft.Base;
using Prosoft.Service;
using System.Threading;
using System.Globalization;
using Autofac;
using Ouikum;
using Ouikum.Common;
using Ouikum.Product;
using Ouikum.Buylead; 
using Ouikum.Company;
using Ouikum.Article;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.BizType;
using Ouikum.Category;
using Lionjob_KingI.Service;
using Ouikum.Web.Models;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Cart;

namespace System.Web.Mvc
{

    public partial class BaseController : BaseClassController
    {
        public Prosoft.Service.FileHelper imgManager;


        #region CheckAuthorizeDomain
        public void CheckAuthorizeDomain()
        {
            HttpContext HttpContext;
            string hostHeader = HttpContext.Current.Request.Headers["host"];   // localhost:xxxx       // ถ้าไม่ใช่ localhost จะใช้ DomainName จาก HostHeader
            string HostName = HttpContext.Current.Request.Url.Host.ToString();
            string RawUrl = HttpContext.Current.Request.RawUrl.ToString();
            string AbsoluteUri = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
            if (HostName.ToLower() == "148260edca7e4a9f8e4b4be6012febe2.cloudapp.net")
            {

            }
            else if (HostName.ToLower() == "localhost")
            {

            }
            else
            {
                //if (HostName.ToLower() != res.Common.lblDomainFullName.ToLower())
                //{
                //    var full = "http://" + res.Common.lblDomainFullName + RawUrl;
                //    HttpContext.Current.Response.Redirect("http://" + res.Common.lblDomainFullName+RawUrl);
                //}
                //else if (HostName.ToLower() == res.Common.lblDomainShotName.ToLower())
                //{
                //    HttpContext.Current.Response.Redirect("http://" + res.Common.lblDomainFullName + RawUrl);
                //}
            }
        }

        #endregion

        #region enum Status  
        public string PathHome { get; set; }

        public enum CategoryType
        {
            Indrustry = 1,
            Wholesale = 2
        }

        public enum CompanyStatus
        {
            BlackList = 0,
            NonActivate = 1,
            Activate = 2,
            BlockInfo = 3,
            Expire = 4
        }

        #endregion

        #region Member
        //Ouikum.Common.AddressDTO dtoAddress;
        List<SelectListItem> listItem = new List<SelectListItem>();
        SelectListItem item = new SelectListItem();
        BuyleadService svBuylead;
        ProductService svProduct;
        QuotationService svQuotation;
        CompanyService svCompany;
        ArticleService svArticle;
        public string UrlProduction = res.Common.lblDomainFullName;
        #endregion

        #region Contructor
        string CurrentCultureInfo = "th-TH";
        string DefaultLanguage = "th-TH";
        public BaseController()
        {
            CheckAuthorizeDomain();
            SetResourceCulture();

            #region Login 
            //  LinkPathB2BThai();
            #endregion

        }
        #endregion

        #region LinkPathB2BThai
        //public void LinkPathB2BThai()
        //{ 
        //    ViewBag.PathB2BThai = res.Pageviews.UrlWeb; 
        //}

        #endregion

        #region RedirectToProduction
        public bool RedirectToProduction()
        {
            var isOpen = true;
            if (isOpen)
            {
                var Host = System.Web.HttpContext.Current.Request.Url.Host.ToString().ToLower();
                var query = System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToString().ToLower();

                if (Host == "91e3d2160c56449699e9f814ad7de768.cloudapp.net")
                {
                    UrlProduction = UrlProduction + query;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion

        #region Remember Last URL
        /// <summary>
        /// Remember Last Visited URL
        /// </summary>
        protected void RememberURL()
        {
            LastURL = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            LastURL = LastURL.Replace(" ", "");
        }
        #endregion

        #region LastURL
        public string LastURL
        {
            get
            {
                if (Session["LastURL"] == null)
                    return Url.Action("Index", "Home");
                else
                    return Session["LastURL"].ToString();
            }
            set
            {
                Session["LastURL"] = value;
            }
        }


        public string RedirectLastUrl(string url)
        {
            if (string.IsNullOrEmpty(LastURL))
                return Url.Content(url);
            else
                return LastURL;
        }
        #endregion


        #region DropDownList

        #region DoLoadComboBoxHotFeatStatus:สถานะ
        public void DoLoadComboBoxHotFeatStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.HotFeatStatus);

            ViewBag.ddlHotFeatStatus = listItem;
        }
        #endregion

        #region DoLoadComboBoxMemberPaidStatus:สถานะ
        public void DoLoadComboBoxMemberPaidStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.MemberPaidStatus);

            ViewBag.ddlMemberPaidStatus = listItem;
        }
        #endregion

        #region DoLoadComboBoxStatus:สถานะ
        public void DoLoadComboBoxStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.ShowStatus);

            ViewBag.ddlFindStatus = listItem;
        }
        #endregion

        #region DoLoadComboBoxApproveStatus:สถานะอนุมัติ
        public void DoLoadComboBoxApproveStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.ApproveStatus);

            ViewBag.ddlFindApproveStatus = listItem;
        }
        #endregion

        #region DoLoadComboBoxProductStatus:สถานะสินค้า
        public void DoLoadComboBoxProductStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.ProductStatus);

            ViewBag.ddlFindProductStatus = listItem;
        }
        #endregion

        #region DoLoadQtyUnit:หน่วยสินค้า
        public void DoLoadQtyUnits()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);

            ViewBag.QtyUnits = listItem;
        }
        #endregion

        #region DoLoadComboBoxPage:หน้า
        public void DoloadComboBoxPageSize()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.PageSize);

            ViewBag.PageSizeList = listItem;
        }
        #endregion

        #region DrowdrawList PageSize
        protected void SelectList_PageSize()
        {
            int[] intPageSize = { 10, 20, 30, 40, 50 };
            ViewBag.PageSizeList = intPageSize;

        }
        #endregion

        #region DoLoadComboBoxGender:เพศ
        public void DoLoadComboBoxGender()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.Gender);

            ViewBag.ddlGender = listItem;
        }
        #endregion

        #region DoLoadComboBoxPeriod:ระยะเวลา
        public void DoLoadComboBoxPeriod()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.Period);

            ViewBag.ddlPeriod = listItem;
        }
        #endregion

        #region JobType
        public void ListJobType()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.JobType);

            ViewBag.JobTypes = listItem;
        }
        #endregion

        #region UserStatus
        public void UserStatus()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.UserStatus);

            ViewBag.ddlUserStatus = listItem;
        }
        #endregion

        #region CompLevels
        public void CompLevels() {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.CompLevel);

            ViewBag.CompLevels = listItem;
        }
        #endregion

        #region ServiceTypeMember
        public void ServiceTypeMember() {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.MemberType);

            ViewBag.ServiceTypes = listItem;
        }
        #endregion

        #region ServiceTypeAdmin
        public void ServiceTypeAdmin()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.AdminType);

            ViewBag.ddlAdminType = listItem;
        }
        #endregion

        #region DegreeLevel
        protected void ListDegreeLevel()
        {
            CommonService svCommon = new CommonService();
            var listItem = svCommon.SelectEnum(CommonService.EnumType.AdminType);

            ViewBag.DegreeLevels = listItem;
        }
        #endregion

        #endregion


        #region DoLoadData
        #region GetEnumServiceType
        public void GetEnumServiceType()
        {
            var svCommon = new CommonService();
            var EnumServiceType = new List<view_EnumData>();
            if (MemoryCache.Default["SearchByServiceType"] != null)
            {
                EnumServiceType = (List<view_EnumData>)MemoryCache.Default["SearchByServiceType"];
            }
            else
            {
                EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
                if (EnumServiceType != null && svCommon.TotalRow > 0)
                {
                    MemoryCache.Default.Add("SearchByServiceType", EnumServiceType, DateTime.Now.AddDays(1));
                }
            }

            ViewBag.EnumServiceType = EnumServiceType;
        }
        #endregion
        #region GetCompany
        public void GetCompany(int CompID)
        {
            var svCompany = new CompanyService();
            string sqlSelect = "CompID,CompLevel,CompName,LogoImgPath,ContactImgPath,CompAddrLine1,CompSubDistrict,CompPostalCode,CompPhone,ContactFirstName,ContactLastName,ContactEmail,ContactAddrLine1,ContactSubDistrict,ContactPostalCode,ContactPhone";
            sqlSelect += ",CompDistrictName,CompProvinceName,ContDistrictName,ContProvinceName,CreatedDate,ExpireDate,ProductCount,ViewCount";
            var Company = svCompany.SelectData<view_Company>(sqlSelect, " CompID = " + CompID).First();
            ViewBag.Company = Company;
        }
        #endregion
        #region GetProduct
        public void GetProductMaxView(int CompID)
        {
            var svProduct = new ProductService();
            string sqlSelect = "ProductID,CompID,ProductName,ViewCount,ProductImgPath";
            string where = "IsDelete = 0 AND CompID = " + CompID;
            var ViewCountMax = svProduct.SelectData<b2bProduct>(sqlSelect, where, "ViewCount DESC");
            ViewBag.ViewCountMax = ViewCountMax.Count > 0 ? ViewCountMax.First() : null;
        }
        #endregion

        #region GetQuotation
        public void GetQuotationMaxView(int CompID)
        {
            var svProduct = new ProductService();
            string sqlSelect = "ProductID,CompID,ProductName,QuotationCount,ProductImgPath";
            string where = "IsDelete = 0 AND CompID = " + CompID;
            var QuotationCountMax = svProduct.SelectData<b2bProduct>(sqlSelect, where, "QuotationCount DESC");
            ViewBag.QuotationCountMax = QuotationCountMax.Count > 0 ? QuotationCountMax.First() : null;
        }
        #endregion



        


        #region GetContact
        public void GetContactMaxView(int CompID)
        {
            var svProduct = new ProductService();
            string sqlSelect = "ProductID,CompID,ProductName,ContactCount,ProductImgPath";
            string where = "IsDelete = 0 AND CompID = " + CompID;
            var ContactCountMax = svProduct.SelectData<b2bProduct>(sqlSelect, where, "ContactCount DESC");
            ViewBag.ContactCountMax = ContactCountMax.Count > 0 ? ContactCountMax.First() : null;
        }
        #endregion
        #region GetNewMessage
        public void GetNewMessage(int CompID)
        {
            var svMessage = new MessageService();
            string sqlSelect = "MessageID,MessageCode,SendDate,FromContactPhone,FromEmail,FromName,IsFavorite,IsAttach,Subject";
            string where = "IsDelete = 0 AND IsSend = 0 AND MsgFolderID = 1 AND ToCompID = " + CompID;
            var NewMessage = svMessage.SelectData<emMessage>(sqlSelect, where);
            ViewBag.NewMessage = NewMessage.Count > 0 ? NewMessage.First() : null;
        }
        #endregion
        #region GetNewQuotation
        public void GetNewQuotation(int CompID)
        {
            var svQuotation = new QuotationService();
            string sqlSelect = "QuotationID,ProductID,ProductName,Qty,QtyUnit,ReqFirstName,ReqLastName,ReqPhone,ReqEmail,IsImportance";
            string where = "IsDelete = 0 AND QuotationStatus = N'R' AND ToCompID = " + CompID;
            var NewQuotation = svQuotation.SelectData<view_Quotation>(sqlSelect, where);
            ViewBag.NewQuotation = NewQuotation.Count > 0 ? NewQuotation.First() : null;
        }
        #endregion
        #endregion

        #region ViewBagHiddenDefault
        public void ViewBagHiddenDefault(string hidWhereCause, string hidGroupBy, string hidOrserBy, int? hidPageIndex, int? hidPageSize, string hidClass, string hidDivID)
        {
            ViewBag.hidUrl = this.Request.Url.Authority;
            ViewBag.hidArea = this.RouteData.DataTokens["area"].ToString() == "Administrator" ? res.Config.BackEnd : this.RouteData.DataTokens["area"].ToString();
            ViewBag.hidController = this.RouteData.Values["controller"];
            ViewBag.hidAction = this.RouteData.Values["action"];
            ViewBag.hidClass = hidClass;
            ViewBag.hidPageIndex = hidPageIndex;
            ViewBag.hidPageSize = hidPageSize;
            ViewBag.hidWhereCause = hidWhereCause;
            ViewBag.hidGroupBy = hidGroupBy;
            ViewBag.hidOrderBy = hidOrserBy;
            ViewBag.hidDivID = hidDivID ?? "divGrid";
        }

        public void ViewBagHiddenDefault(string hidWhereCause, string hidGroupBy, string hidOrderBy, int? hidPageIndex, int? hidPageSize, string hidClass)
        {
            ViewBag.hidUrl = this.Request.Url.Authority;
            ViewBag.hidArea = this.RouteData.DataTokens["area"] != null ? (this.RouteData.DataTokens["area"].ToString() == "Administrator" ? res.Config.BackEnd : this.RouteData.DataTokens["area"].ToString()) : null;
            ViewBag.hidController = this.RouteData.Values["controller"];
            ViewBag.hidAction = this.RouteData.Values["action"];
            ViewBag.hidClass = hidClass;
            ViewBag.hidPageIndex = hidPageIndex;
            ViewBag.hidPageSize = hidPageSize;
            ViewBag.hidWhereCause = hidWhereCause;
            ViewBag.hidGroupBy = hidGroupBy;
            ViewBag.hidOrderBy = hidOrderBy;
            ViewBag.hidDivID = "divGrid";
        }

        public void ViewBagHiddenDefault(string hidWhereCause, string hidGroupBy, string hidOrderBy, int? hidPageIndex, int? hidPageSize)
        {
            ViewBag.hidUrl = this.Request.Url.Authority;
            ViewBag.hidArea = this.RouteData.DataTokens["area"] != null ? (this.RouteData.DataTokens["area"].ToString() == "Administrator" ? res.Config.BackEnd : this.RouteData.DataTokens["area"].ToString()) : null;
            ViewBag.hidController = this.RouteData.Values["controller"];
            ViewBag.hidAction = this.RouteData.Values["action"];
            ViewBag.hidPageIndex = hidPageIndex;
            ViewBag.hidPageSize = hidPageSize;
            ViewBag.hidWhereCause = hidWhereCause;
            ViewBag.hidGroupBy = hidGroupBy;
            ViewBag.hidOrderBy = hidOrderBy;
            ViewBag.hidDivID = "divGrid";
        }
        #endregion

        #region Language
        /// <summary>
        /// SetResourceCulture for Control Language
        /// </summary>
        public override void SetResourceCulture()
        {
            //HttpContext HttpContext;
            // ViewBag.PageLanguage = DataManager.ConvertToString(HttpContext.Current.Request.RequestContext.RouteData.Values["lang"], "");

            //string CurrentLanguage = ViewBag.PageLanguage;  // from route {lang}

            //CurrentCultureInfo = CurrentLanguage == "en" ? "en-US" : "th-TH";
            //ViewBag.Lang = CurrentCultureInfo;
            ////Base.AppLang = CurrentCultureInfo;
            //if (ViewBag.PageLanguage.Equals(DefaultLanguage))
            //{
            //    string strRedirect = HttpContext.Current.Request.RawUrl;
            //    strRedirect = strRedirect.Replace("%2f", "/");
            //    strRedirect = strRedirect.Replace("%2f", "/").Replace("/th", "").Replace("/en", "");
            //    HttpContext.Current.Response.Redirect("~/" + strRedirect);
            //}
            //CultureInfo Culture = new CultureInfo(CurrentCultureInfo);
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(CurrentCultureInfo);
            //ApplicationHelper.PrimaryLanguage = Culture.ToString();

            // Code that runs on application startup
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["Language"];
            if (cookie != null && cookie.Value != null)

            {
                CurrentCultureInfo = cookie.Value;
            }

            CultureInfo Culture = new CultureInfo(CurrentCultureInfo);
            res.Admin.Culture = Culture;
            res.Article.Culture = Culture;
            res.Common.Culture = Culture;
            res.Product.Culture = Culture;
            res.Buylead.Culture = Culture;
            res.Company.Culture = Culture;
            res.Member.Culture = Culture;
            res.Message_Center.Culture = Culture;
            res.Quotation.Culture = Culture;
            res.Purchasing.Culture = Culture;
            res.Order.Culture = Culture;
            res.Pageviews.Culture = Culture;
            res.JS.Culture = Culture;
            res.Benefit.Culture = Culture;
            res.Config.Culture = Culture;
            res.Email.Culture = Culture;
            res.Shopping.Culture = Culture;
            res.Cart.Culture = Culture;
            res.ShoppingBasket.Culture = Culture;
            res.EmaiOrderToBuyer.Culture = Culture;
            res.Shopping.Culture = Culture;
            //res.Controller.Culture = Culture;

        }

        #endregion

        #region CreateLogFiles
        public void CreateLogFiles(Exception ex)
        {
            Prosoft.Service.BlobStorageService blob = new BlobStorageService();//test ใช้ cloud
            var url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            if (LogonCompID > 0)
            {
                OnSendErrorMail(ex, url, LogonCompID.ToString());
                blob.BlobCreateLogFiles(ex, url, LogonCompName);
            }
            else
            {
                OnSendErrorMail(ex, url);
                blob.BlobCreateLogFiles(ex, url);
            }

        }
        #endregion

        #region Save File Image
        public void SaveFileImage(string DirTempPath, string DirPath, string NewImgPath, int width = 0, int height = 0)
        {
            imgManager = new FileHelper();
            if (!string.IsNullOrEmpty(NewImgPath))
            {
                if (!imgManager.Exists(DirPath + "/" + NewImgPath))
                {
                    imgManager.CreateObjImage(DirTempPath, DirPath, NewImgPath, NewImgPath, width, height);
                }
            }
        }
        public void SaveFileImage(string DirTempPath, string DirPath, string[] NewImgPath, int sizeThumb, int sizeImg)
        {
            imgManager = new FileHelper();
            for (int i = 0; i < NewImgPath.Length; i++)
            {
                if (!string.IsNullOrEmpty(NewImgPath[i]))
                {
                    if (!imgManager.Exists(DirPath + "/" + NewImgPath[i]))
                    {
                        imgManager.CreateObjImage(DirTempPath, DirPath, NewImgPath[i], NewImgPath[i], 0, 0);
                        imgManager.CreateObjImage(DirTempPath, DirPath, NewImgPath[i], "Thumb_" + NewImgPath[i], sizeThumb, sizeThumb);
                        imgManager.CreateObjImage(DirTempPath, DirPath, NewImgPath[i], "img-" + NewImgPath[i], sizeImg, sizeImg);
                    }
                }
            }
        }
        public void SaveFileImage(string DirTempPath, string DirPath, List<string> NewImgPath, int sizeThumb, int sizeImg)
        {
            imgManager = new FileHelper();
            foreach (var it in NewImgPath)
            {
                if (!string.IsNullOrEmpty(it))
                {
                    if (!imgManager.Exists(DirPath + "/" + it))
                    {
                        if (imgManager.Exists(DirTempPath + "/" + it))
                        {
                            imgManager.CreateObjImage(DirTempPath, DirPath, it, it, sizeImg, sizeImg);
                            imgManager.CreateObjImage(DirTempPath, DirPath, it, "Thumb_" + it, sizeThumb, sizeThumb);
                            imgManager.CreateObjImage(DirTempPath, DirPath, it, "img-" + it, 0, 0);
                        }
                    }
                }
            }

        }
        #endregion

        #region Delete File Image
        public void DeleteFileImage(string DirPath, string[] OldImgPath, string[] NewImgPath)
        {
            imgManager = new FileHelper();
            List<string> DelCompImg = new List<string>();
            imgManager.IsSuccess = false;

            for (int i = 0; i < OldImgPath.Length; i++)
            {
                int cnt = 0;
                if (!string.IsNullOrEmpty(OldImgPath[i]))
                {
                    if (imgManager.Exists(DirPath + "/" + OldImgPath[i]))
                    {
                        for (int j = 0; j < NewImgPath.Length; j++)
                        {
                            var z = OldImgPath[i];
                            var m = NewImgPath[j];
                            int c = NewImgPath.Length;
                            if (OldImgPath[i] != NewImgPath[j])
                            {
                                cnt++;

                            }
                        }
                        if (cnt == NewImgPath.Length)
                        {
                            DelCompImg.Add(OldImgPath[i]);
                        }
                    }
                }
            }
            if (DelCompImg != null)
            {
                foreach (var filename in DelCompImg)
                {
                    imgManager.RemoveImage(DirPath, filename);
                    imgManager.RemoveImage(DirPath, "Thumb_" + filename);
                    imgManager.RemoveImage(DirPath, "img-" + filename);
                }
            }
        }

        public void DeleteFileImage(string DirPath, List<string> OldImgPath, List<string> NewImgPath)
        {
            imgManager = new FileHelper();
            List<string> DelCompImg = new List<string>();
            imgManager.IsSuccess = false;

            for (int i = 0; i < OldImgPath.Count; i++)
            {
                int cnt = 0;
                if (!string.IsNullOrEmpty(OldImgPath[i]))
                {
                    if (imgManager.Exists(DirPath + "/" + OldImgPath[i]))
                    {
                        for (int j = 0; j < NewImgPath.Count; j++)
                        {
                            var z = OldImgPath[i];
                            var m = NewImgPath[j];
                            int c = NewImgPath.Count;
                            if (OldImgPath[i] != NewImgPath[j])
                            {
                                cnt++;

                            }
                        }
                        if (cnt == NewImgPath.Count)
                        {
                            DelCompImg.Add(OldImgPath[i]);
                        }
                    }
                }
            }
            if (DelCompImg != null)
            {
                DeleteImage(DirPath, DelCompImg);
            }
        }
        #endregion

        public void DeleteImage(string DirPath, List<string> DelCompImg)
        {
            foreach (var filename in DelCompImg)
            {
                imgManager.RemoveImage(DirPath, filename);
                imgManager.RemoveImage(DirPath, "Thumb_" + filename);
                imgManager.RemoveImage(DirPath, "img-" + filename);
            }
        }

        public string MsgErrorResult { get; set; }

        public string GenerateMsgError(List<Exception> ex)
        {
            foreach (var it in ex)
            {
                MsgErrorResult += " - " + it.Message + " \n";
            }
            return MsgErrorResult;
        }

        #region Update ViewCount Product
        public bool CheckExistIP(int id, string TypeName)
        {
            if (Request.Cookies["ClientIP" + TypeName + id] != null)
            {
                ViewBag.ClientIP = (string)Request.Cookies["ClientIP" + TypeName + id].Value;
                return false;
            }
            else
            {
                Response.Cookies["ClientIP" + TypeName + id].Value = Request.UserHostAddress;
                Response.Cookies["ClientIP" + TypeName + id].Expires = DateTime.Now.AddMinutes(1);
                ViewBag.ClientIP = (string)Request.UserHostAddress;
                return true;
            }
        }
        public void AddViewCount(int id, string Type)
        {
            try
            {
                if (CheckExistIP(id, Type))
                {
                    switch (Type)
                    {
                        case "Product":
                            svProduct = new ProductService();
                            svProduct.UpdateProductViewCount(id);
                            //var mgKeyword = new KeywordMongo();
                            //mgKeyword.UpdateProductViewCount(id);
                            break;

                        case "Buylead":
                            svBuylead = new BuyleadService();
                            svBuylead.UpdateBuyleadViewCount(id);
                            break;

                        case "Supplier":
                            svCompany = new CompanyService();
                            svCompany.UpdateCompanyViewCount(id);
                            break;

                        case "Blog":
                            svArticle = new ArticleService();
                            svArticle.UpdateArticleViewCount(id);
                            break;
                        case "Tel":
                            svProduct = new ProductService();
                            svProduct.UpdateTelCount(id);
                            break;
                        case "Quotation":
                            svProduct = new ProductService();
                            svProduct.UpdateQuotationCount(id);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

        }

        public void AddCountQuotation(int id, string Type)
        {
            try
            {
                if (CheckExistIP(id, Type))
                {
                    switch (Type)
                    {
                        case "Quotation":
                            svQuotation = new QuotationService();
                            svQuotation.UpdateQuotationCount(id);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

        }

        #endregion

        #region Update ContactCount b2bProduct

        public void AddContactCount(int id)
        {
            try
            {
                svProduct = new ProductService();
                svProduct.AddContactCount(id);
                //var mgKeyword = new KeywordMongo();
                //mgKeyword.UpdateProductViewCount(id);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

        }
        #endregion

        #region AutoGenCode
        public void GetStatusUser()
        {
            //UserStatusModel user = new UserStatusModel();
            //user.CompID = (LogonCompID > 0)? LogonCompID:0; 
            //user.CompLevel = LogonCompLevel;
            //user.DisplayName = LogonDisplayName;
            //user.Email = LogonEmail;
            //user.ServiceType = LogonServiceType;
            //user.CompName = LogonCompName;
            //ViewBag.UserStatus = user;

            var svMember = new MemberService();
            UserStatusModel user = new UserStatusModel();
            var emMembers = new Ouikum.emMember();
            string ID = DataManager.ConvertToString(LogonMemberID);

            if (ID != "0")
            {
                emMembers = svMember.SelectData<emMember>("*", "IsDelete = 0 AND MemberID =" + ID, null, 1, 1).First();
            }

            user.CompID = (LogonCompID > 0) ? LogonCompID : 0;
            user.CompLevel = LogonCompLevel;
            user.DisplayName = LogonDisplayName;
            user.Email = LogonEmail;
            user.ServiceType = LogonServiceType;
            user.CompName = LogonCompName;
            user.MemId = emMembers.MemberID;
            user.MemEmail = emMembers.Email;
            user.MemFirstName = emMembers.FirstName;
            user.MemLastName = emMembers.LastName;
            user.MemImgPath = emMembers.AvatarImgPath;
            user.MemPhone = emMembers.Phone;
            user.MemAddrLine1 = emMembers.AddrLine1;
            user.MemAddrLine2 = emMembers.AddrLine2;
            user.MemProviceID = emMembers.ProvinceID;
            user.MemDirtriceID = emMembers.DistrictID;
            user.MembSubDirticeID = emMembers.SubDistrict;
            //user.TotalTempCart = Convert.ToInt32(countProduct.countProduct);
            ViewBag.UserStatus = user;
        }
        #endregion

        public void SetStatusWebsite(int CompID,string CompName)
        {
            Response.Cookies["WebsiteCompID"].Value = CompID.ToString();
            Response.Cookies["WebsiteCompID"].Expires = DateTime.Now.AddHours(1);

            string UrlCompName = Url.ReplaceUrl(CompName);
            Response.Cookies["WebsiteCompName"].Value = EncryptText("&%#@?,:*", UrlCompName);
            Response.Cookies["WebsiteCompName"].Expires = DateTime.Now.AddHours(1);

        }

        public void GetStatusWebsite()
        {
            WebsiteModel web = new WebsiteModel();
            web.WebsiteCompID = Request.Cookies["WebsiteCompID"].Value;
            web.WebsiteCompName = DecryptText("&%#@?,:*", Request.Cookies["WebsiteCompName"].Value);
            ViewBag.WebStatus = web;
        } 

        #region AutoGenCode
        public string AutoGenCode(string HCode, int Count)
        {
            if (Session["RunningNumb"] == null)
            {
                Session["RunningNumb"] = 100;

            }
            string FullGen = HCode;
            FullGen = FullGen + "-" + RandomCharecter(3) + "-" + DateTime.Now.ToString("yyMMdd") + "-" + (Count + 1);

            return FullGen;
        }

        private string RandomCharecter(int Size)
        {
            Random ran = new Random();
            string chars = "ABCDEFGHIJKLMNOPQESTUVWXYZ";
            char[] buffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                buffer[i] = chars[ran.Next(chars.Length)];
            }
            return new string(buffer);
        }
        private string RandomCharInt(int Size)
        {
            Random ran = new Random();
            string chars = "0123456789";
            char[] buffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                buffer[i] = chars[ran.Next(chars.Length)];
            }
            return new string(buffer);
        }
        #endregion                

        #region Search Supplier
        public List<view_SearchProduct> SearchSupplier(ProductAction action)
        {
            svProduct = new ProductService();
            string sqlSelect = "CompID";
            string sqlWhere = svProduct.CreateWhereAction(action);

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause((int)ViewBag.CompID, ViewBag.TextSearch, (int)ViewBag.PStatus, (int)ViewBag.GroupID,
                (int)ViewBag.CateLevel, (int)ViewBag.CateID, (int)ViewBag.BizTypeID, (int)ViewBag.CompLevel, (int)ViewBag.CompProvinceID);

            #endregion

            var ProductCompID = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere).Distinct();
            return ProductCompID.ToList();
        }
        #endregion

        #region SetProductPager
        public void SetProductPager(FormCollection form)
        {
            ViewBag.CompID = DataManager.ConvertToInteger(form["CompID"], 0);
            ViewBag.PStatus = DataManager.ConvertToInteger(form["PStatus"], 0) ;
            ViewBag.GroupID = DataManager.ConvertToInteger(form["GroupID"], 0);
            ViewBag.CateID = DataManager.ConvertToInteger(form["CateID"], 0) ;
            ViewBag.CateLevel =  DataManager.ConvertToInteger(form["CateLevel"], 0) ;
            ViewBag.BizTypeID =  DataManager.ConvertToInteger(form["BizTypeID"], 0) ;
            ViewBag.CompLevel =  DataManager.ConvertToInteger(form["CompLevel"], 0) ;
            ViewBag.CompProvinceID = DataManager.ConvertToInteger(form["CompProvinceID"], 0) ;
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
            ViewBag.IsRecommend = DataManager.ConvertToBool(form["IsRecommend"], false);
            ViewBag.SearchType =  !string.IsNullOrEmpty(form["SearchType"]) ? form["SearchType"] : "";
        }
        #endregion

        #region SetBuyleadPager
        public void SetBuyleadPager(FormCollection form)
        {
            ViewBag.CompID = DataManager.ConvertToInteger(form["CompID"], 0);
            ViewBag.PStatus = DataManager.ConvertToInteger(form["PStatus"], 0);
            ViewBag.CateID = DataManager.ConvertToInteger(form["CateID"], 0);
            ViewBag.CateLevel = DataManager.ConvertToInteger(form["CateLevel"], 0);
            ViewBag.BizTypeID = DataManager.ConvertToInteger(form["BizTypeID"], 0);
            ViewBag.CompLevel = DataManager.ConvertToInteger(form["CompLevel"], 0);
            ViewBag.CompProvinceID = DataManager.ConvertToInteger(form["CompProvinceID"], 0);
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
            ViewBag.SearchType = !string.IsNullOrEmpty(form["SearchType"]) ? form["SearchType"] : "";
        }
        #endregion

        #region SetOrderPackagePager
        public void SetOrderPackagePager(FormCollection form)
        {
            ViewBag.PStatus = !string.IsNullOrEmpty(form["PStatus"]) ? form["PStatus"] : "";
            ViewBag.PSType = !string.IsNullOrEmpty(form["PSType"]) ? form["PSType"] : "";
            ViewBag.SearchType = !string.IsNullOrEmpty(form["SearchType"]) ? form["SearchType"] : "";
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
        }
        #endregion

        #region GetTimeStamp
        public string GetTimeStamp()
        {
            DateTime value = System.DateTime.Now;
            return value.ToString("yyyyMMddHHmmssffff");
        }
        #endregion

        #region DownLoadImage
        public ActionResult DownloadImage(string fname, string section,string id)//พารามิเตอร์แรก เป็นชื่อไฟล์,พารามิเตอร์ที่ 2
        {
            string[] item = fname.Split('.');
            string fileType = string.Empty;
            if (item.Count() > 1)
            {
                string ext = item[item.Length - 1].ToLower();
                if (item[item.Length - 1] == "jpg")
                    fileType = "image/jpeg";
                else if (item[item.Length - 1] == "jpeg")
                    fileType = "image/jpeg";
                else if (item[item.Length - 1] == "png")
                    fileType = "image/png";
                else if (item[item.Length - 1] == "gif")
                    fileType = "image/gif";
                else
                    fileType = "image/jpeg";
            }
            string filePath = "~/Upload/Prosoft/"+ section+"/" + id + "/";
            var dir = Server.MapPath(filePath);
            var path = Path.Combine(dir, fname);
            Response.AddHeader("content-disposition", "attachment; filename=" + item[0]);
            return base.File(path, fileType);
        }
        #endregion

        #region GetIPAddress
        public static string GetIPAddress(HttpRequestBase request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch { ip = null; }

            return ip;
        }
        #endregion

        #region ReplaceText
        public string ReplaceText(string value)
        {
            value = Regex.Replace(value, "[\n\r\t]", " ");
            return value;
        }
        #endregion

        /*---------Upload File-------------*/
        #region Upload

        #region SetUpload
        #region SetPrefixPath
        /// <summary> 
        /// SetPrefixPath เพื่อแยกระหว่าง CMSMember,WebAdmin กับ SoGoodAdmin
        /// </summary>
        /// <param name="IsSoGoodAdmin">True | False</param>
        /// <returns></returns>
        public string SetPrefixPath(bool IsSoGoodAdmin)
        {
            //แบบใหม่จะเข้ารหัส WebID และใช้ ViewBag.ImgPrefixPath แทน ViewBag.PrefixPath
            //string Prefix = IsSoGoodAdmin ? "Upload/Administrator/" : "Upload/" + Prosoft.Base.BaseClassController.EnCodeID(DataManager.ConvertToInteger(ViewBag.WebID)) + "/";
            string Prefix = IsSoGoodAdmin ? "Administrator/" : "";
            ViewBag.ImgPrefixPath = Prefix; //แบบใหม่ (แบบเก่าจะเป็น ViewBag.PrefixPath คือไม่มี Img นำหน้า)
            return Prefix;
        }
        #endregion

        #region SetfilePath
        /// <summary> 
        /// SetfilePath เป็นการกำหนด folder path ของไฟล์
        /// </summary>
        /// <param name="Prefix">ได้มาจาก SetPrefix(IsSoGoodAdmin)</param>
        /// <param name="filePath">path ที่ต้องการ</param>
        /// <returns></returns>
        public string SetfilePath(string Prefix, string filePath, bool? IsTemp)
        {
            string Subfix = IsTemp.Value ? GetRandomCode(8) + "" : "";
            if (!string.IsNullOrEmpty(Prefix))
            {
                filePath = filePath.Replace(Prefix, "").Replace(Prefix + "", "");
            }
            filePath = (!string.IsNullOrEmpty(filePath)) ? (IsTemp.Value ? "Temp/" + filePath : filePath) : "Temp";
            filePath = filePath + (!filePath.EndsWith("/") ? "" : "");
            return Prefix + filePath + Subfix;
        }
        #endregion

        #region SetNoImgFullPath
        /// <summary>
        /// SetNoImgFullPath สำหรับกำหนดรูป noimage
        /// </summary>
        /// <param name="IsAvatar">True | False</param>
        /// <returns></returns>
        public string SetNoImgFullPath(bool IsAvatar)
        {
            return IsAvatar ? "Content/Default/Image/noavatar.gif" : "Content/Default/Image/icon-nopicture.png";
        }
        #endregion
        #endregion

        #region SetUpload Configuration For Module

        #region DefaultSetting

        protected static Dictionary<string, object> GetNewSetting()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("modulename", "Default");
            dic.Add("primeid", 0);
            dic.Add("videosize", new int[] { 100, 100 });
            dic.Add("thumbsize", new List<int>[] { new List<int> { 100, 100 } });
            dic.Add("previewsize", new int[] { 100, 100 });
            dic.Add("isshowpreview", false);
            dic.Add("issogoodadmin", false);
            dic.Add("descriptionposition", "top");
            dic.Add("description", "");
            dic.Add("path", "Default");
            dic.Add("keyname", "");
            dic.Add("isavatar", false);
            dic.Add("allowformat", "");
            dic.Add("controllername", "~/Base");
            dic.Add("maxfilesize", 0);
            dic.Add("mutiple", false);
            return dic;
        }
        #endregion

        #region GetReturnSettingValue
        /// <summary>
        /// 
        /// </summary>
        ///  modulename
        ///  primeid
        ///  videosize
        ///  thumbsize
        ///  previewsize
        ///  isshowpreview
        ///  issogoodadmin
        ///  prefixupload
        ///  descriptionposition
        ///  description
        ///  path
        ///  keyname
        ///  isavatar
        ///  allowformat
        public T GetReturnSettingValue<T>(string moduleName, string returnKey, object defalutValue = null)
        {
            Dictionary<string, object> dic = GetNewSetting();
            switch (moduleName.ToLower())
            {

                #region Quotation
                case "quotation":
                    dic["isthumbnail"] = false;
                    dic["thumbsize"] = new List<int>[]
                    {
                        new List<int> { 32, 32 }
                    };
                    dic["previewsize"] = new int[] { 32, 32 };
                    dic["path"] = "Quotation";
                    dic["keyname"] = "FileName";
                    dic["modulename"] = "quotation";
                    dic["isshowpreview"] = false;
                    dic["allowformat"] = ".*";
                    dic["isresize"] = false;
                    dic["isshowpreview"] = true;
                    dic["resizesize"] = new int[] { 0, 0 };
                    dic["maxsize"] = new int[] { 0, 0 };
                    dic["description"] = "<div style=\"width: 380px;\">ชนิดของไฟล์ .pdf .xlsx .docx .jpg .jpgs ขนาดไม่เกิน 5 mb.</div>";
                    dic["descriptionposition"] = "top";
                    dic["controllername"] = "../Base";
                    dic["maxfilesize"] = 5;
                    break;
                #endregion

                #region bidproduct
                case "bidproduct":
                    dic["isthumbnail"] = false;
                    dic["thumbsize"] = new List<int>[]
                    {
                        new List<int> { 32, 32 }
                    };
                    dic["previewsize"] = new int[] { 32, 32 };
                    dic["path"] = "Quotation";
                    dic["keyname"] = "FileName";
                    dic["modulename"] = "quotation";
                    dic["isshowpreview"] = false;
                    dic["allowformat"] = ".*";
                    dic["isresize"] = false;
                    dic["isshowpreview"] = true;
                    dic["resizesize"] = new int[] { 0, 0 };
                    dic["maxsize"] = new int[] { 0, 0 };
                    dic["description"] = "<div style=\"width: 380px;\">" + res.Product.lblformatfile +" "+ res.Product.lblAddImg_Guide2 + "</div>";
                    dic["descriptionposition"] = "top";
                    dic["controllername"] = "./Base";
                    dic["maxfilesize"] = 5;
                    break;
                #endregion

                #region Message Center
                case "message":
                    dic["isthumbnail"] = false;
                    dic["thumbsize"] = new List<int>[]
                    {
                        new List<int> { 32, 32 }
                    };
                    dic["previewsize"] = new int[] { 32, 32 };
                    dic["path"] = "Message";
                    dic["keyname"] = "FileName";
                    dic["modulename"] = "message";
                    dic["isshowpreview"] = false;
                    dic["allowformat"] = ".*";
                    dic["isresize"] = false;
                    dic["isshowpreview"] = true;
                    dic["resizesize"] = new int[] { 0, 0 };
                    dic["maxsize"] = new int[] { 0, 0 };
                    dic["description"] = "<div style=\"width: 380px;\">ชนิดของไฟล์ต้องเป็นไฟล์รูปแบบ .doc,.docx,.pdf </div>";
                    dic["descriptionposition"] = "top";
                    dic["controllername"] = "Base";
                    dic["maxfilesize"] = 5;
                    dic["mutiple"] = true;
                    break;
                #endregion

            }

            #region return part
            if (!string.IsNullOrEmpty(returnKey))
            {
                return (T)(dic.Keys.Contains(returnKey) ? dic[returnKey] : defalutValue);
            }
            else
            {
                return (T)defalutValue;
            }
            #endregion
        }
        #endregion


        #region SetUploadVideo
        public void SetUploadVideo(string moduleName, string imgThumbName, int? PrimeID)
        {
            #region Member
            string prefix = GetReturnSettingValue<string>(moduleName, "prefixupload", "Video");

            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(moduleName, "issogoodadmin", false);
            bool IsAvatar = GetReturnSettingValue<bool>(moduleName, "isavatar", false);
            bool IsShowPreview = GetReturnSettingValue<bool>(moduleName, "isshowpreview", false);

            string PrefixPath = SetPrefixPath(IsSoGoodAdmin);
            string DescriptionPosition = GetReturnSettingValue<string>(moduleName, "descriptionposition", "");
            string AllowFormat = GetReturnSettingValue<string>(moduleName, "allowformat", "");

            #endregion

            #region Set Value

            ViewData["PrefixSetVideoUpload"] = prefix;     //ใช้ Get ViewData ในหน้า UC/UploadPictureUC
            ViewData[prefix + "ModuleName"] = GetReturnSettingValue<string>(moduleName, "modulename", "");

            #region Controller name
            ViewData[prefix + "ControllerName"] = GetReturnSettingValue<string>(moduleName, "controllername", "../Base");
            #endregion

            #region Image FileType
            ViewData[prefix + "AllowFormat"] = AllowFormat;
            #endregion

            #region Image Path
            ViewData[prefix + "PrefixPath"] = PrefixPath;
            ViewData[prefix + "UploadPath"] = SetfilePath(PrefixPath, GetReturnSettingValue<string>(moduleName, "path", ""), true);
            ViewData[prefix + "FilePath"] = SetfilePath(PrefixPath, GetReturnSettingValue<string>(moduleName, "path", "") + "/" + PrimeID, false);
            ViewData[prefix + "FileName"] = GetReturnSettingValue<string>(moduleName, "filename", "");
            ViewData[prefix + "FileThumbName"] = GetReturnSettingValue<string>(moduleName, "thumbfilename", "");
            ViewData[prefix + "NoImgFullPath"] = SetNoImgFullPath(GetReturnSettingValue<bool>(moduleName, "isavatar", false));
            #endregion

            #region Image Boolean
            ViewData[prefix + "IsAvatar"] = GetReturnSettingValue<bool>(moduleName, "isavatar", false);
            ViewData[prefix + "IsSoGoodAdmin"] = GetReturnSettingValue<bool>(moduleName, "issogoodadmin", false);
            #endregion

            #region Image Preview
            ViewData[prefix + "IsShowPreview"] = IsShowPreview;
            if (IsShowPreview)
            {
                int[] size = GetReturnSettingValue<int[]>(moduleName, "previewsize", new int[] { 0, 0 });
                ViewData[prefix + "PreviewWidth"] = size[0];
                ViewData[prefix + "PreviewHeight"] = size[1];
            }
            #endregion

            #region Image Description : ตำแหน่งของคำอธิบาย [Top|Right|Bottom]
            ViewData[prefix + "DescriptionPosition"] = DescriptionPosition;
            //คำอธิบาย
            //if (DescriptionPosition == "right" || DescriptionPosition == "top" || DescriptionPosition == "bottom" || DescriptionPosition == "left")
            ViewData[prefix + "DescriptionList"] = GetReturnSettingValue<string>(moduleName, "description", "");
            #endregion

            #region ThumbSize : ขนาดของ thumb
            string Thumbnail = "";
            foreach (var item in GetReturnSettingValue<List<int>[]>(moduleName, "thumbsize", new List<int>[] { new List<int> { 0, 0 } }))
            {
                Thumbnail += item[0] + "x" + item[1] + ".";
            }
            ViewData[prefix + "ThumbnailSize"] = Thumbnail;
            #endregion
            #endregion
        }
        #endregion

        #region SetUploadImage
        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// </summary>
        public void SetUploadImage(string ModuleName)
        {
            SetUploadImage(ModuleName, "", 0);
        }

        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// <param name="imgFileName">ชื่อไฟล์รูป</param>
        /// <param name="PrimeID">Primery Key</param>
        /// </summary>
        public ViewDataDictionary<dynamic> SetUploadImage(string ModuleName, string imgFileName, int? PrimeID, ViewDataDictionary<dynamic> view = null)
        {
            #region Member
            string prefix = ModuleName;

            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            bool IsAvatar = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            bool IsShowPreview = GetReturnSettingValue<bool>(ModuleName, "isshowpreview", false);

            string PrefixPath = SetPrefixPath(IsSoGoodAdmin);
            string DescriptionPosition = GetReturnSettingValue<string>(ModuleName, "descriptionposition", "");
            string AllowFormat = GetReturnSettingValue<string>(ModuleName, "allowformat", "");

            string Href = GetReturnSettingValue<string>(ModuleName, "href", "");

            int[] size = GetReturnSettingValue<int[]>(ModuleName, "maxsize", new int[] { 0, 0 });

            int MaxWidth = size[0];
            int MaxHeight = size[1];
            #endregion


            #region Set Value
            if (view["PrefixSetImageUpload"] == null)
            {
                view.Add("PrefixSetImageUpload", prefix);
            }
            else
            {
                view["PrefixSetImageUpload"] = prefix;
            }
            view.Add(prefix + "MaxFileSize", GetReturnSettingValue<int>(ModuleName, "maxfilesize", 0));
            view.Add(prefix + "ModuleName", GetReturnSettingValue<string>(ModuleName, "modulename", ""));
            view.Add(prefix + "Href", Href);
            view.Add(prefix + "ControllerName", GetReturnSettingValue<string>(ModuleName, "controllername", ""));

            #region Image FileType
            view.Add(prefix + "AllowFormat", AllowFormat);
            #endregion

            #region Image Path
            view.Add(prefix + "PrefixPath", PrefixPath);
            view.Add(prefix + "UploadPath", SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, true));
            view.Add(prefix + "FilePath", SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, false));
            view.Add(prefix + "FileName", imgFileName);
            view.Add(prefix + "NoImgFullPath", SetNoImgFullPath(IsAvatar));
            #endregion

            #region Image Boolean
            view.Add(prefix + "IsAvatar", IsAvatar);
            view.Add(prefix + "IsResize", GetReturnSettingValue<bool>(ModuleName, "isresize", false));
            #endregion

            #region Image Preview
            view.Add(prefix + "IsShowPreview", IsShowPreview);
            if (IsShowPreview)
            {
                int[] previewsize = GetReturnSettingValue<int[]>(ModuleName, "previewsize", new int[] { 0, 0 });
                view.Add(prefix + "PreviewWidth", previewsize[0]);
                view.Add(prefix + "PreviewHeight", previewsize[1]);
            }
            #endregion

            //#region Image Validation
            //ViewData[prefix + "MaxWidth"] = MaxWidth;
            //ViewData[prefix + "MaxHeight"] = MaxHeight;
            //#endregion

            List<int>[] thumbsize = GetReturnSettingValue<List<int>[]>(ModuleName, "thumbsize", null);
            string thumbstr = string.Empty;
            foreach (var item in thumbsize)
            {
                thumbstr += item[0] + "x" + item[1] + ",";
            }

            #region Thumbnail Size
            view.Add(prefix + "ThumbnailSize", thumbstr);
            #endregion

            #region Image Description : ตำแหน่งของคำอธิบาย [Top|Right|Bottom]
            view.Add(prefix + "DescriptionPosition", DescriptionPosition);
            //คำอธิบาย
            //if (DescriptionPosition == "right" || DescriptionPosition == "top" || DescriptionPosition == "bottom" || DescriptionPosition == "left")
            view.Add(prefix + "DescriptionList", GetReturnSettingValue<string>(ModuleName, "description", ""));


            #endregion
            return view;
        }
            #endregion



        #endregion

        #region SetUploadFile
        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// </summary>
        public void SetUploadFile(string ModuleName)
        {
            SetUploadFile(ModuleName, "", 0);
        }

        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// <param name="imgFileName">ชื่อไฟล์รูป</param>
        /// <param name="PrimeID">Primery Key</param>
        /// </summary>
        public ViewDataDictionary<dynamic> SetUploadFile(string ModuleName, string FileName, int? PrimeID, ViewDataDictionary<dynamic> view = null)
        {
            #region Member
            string prefix = ModuleName;

            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            bool IsAvatar = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            bool IsShowPreview = GetReturnSettingValue<bool>(ModuleName, "isshowpreview", false);

            string PrefixPath = SetPrefixPath(IsSoGoodAdmin);
            string DescriptionPosition = GetReturnSettingValue<string>(ModuleName, "descriptionposition", "");
            string AllowFormat = GetReturnSettingValue<string>(ModuleName, "allowformat", "");

            //int[] size = GetReturnSettingValue<int[]>(ModuleName, "maxsize", new int[] { 0, 0 });

            //int MaxWidth = size[0];
            //int MaxHeight = size[1];
            #endregion


            #region Set Value
            if (view["PrefixSetImageUpload"] == null)
            {
                view.Add("PrefixSetImageUpload", prefix);
            }
            else
            {
                view["PrefixSetImageUpload"] = prefix;
            }

            view.Add(prefix + "ModuleName", GetReturnSettingValue<string>(ModuleName, "modulename", ""));

            view.Add(prefix + "Mutiple", GetReturnSettingValue<bool>(ModuleName, "mutiple", false));
            view.Add(prefix + "MaxFileSize", GetReturnSettingValue<int>(ModuleName, "maxfilesize", 0));

            #region Controller name
            view.Add(prefix + "ControllerName", GetReturnSettingValue<string>(ModuleName, "controllername", "../Base"));
            #endregion

            #region Image FileType
            view.Add(prefix + "AllowFormat", AllowFormat);

            #endregion

            #region Image Path
            view.Add(prefix + "PrefixPath", PrefixPath);
            view.Add(prefix + "UploadPath", SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, true));
            view.Add(prefix + "FilePath", SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, false));
            view.Add(prefix + "NoImgFullPath", SetNoImgFullPath(IsAvatar));
            view.Add(prefix + "FileName", FileName);
            #endregion

            #region Image Boolean
            view.Add(prefix + "IsAvatar", IsAvatar);
            view.Add(prefix + "IsResize", GetReturnSettingValue<bool>(ModuleName, "isresize", false));

            #endregion
            view.Add(prefix + "IsShowPreview", IsShowPreview);


            #region Image Description : ตำแหน่งของคำอธิบาย [Top|Right|Bottom]
            view.Add(prefix + "DescriptionPosition", DescriptionPosition);

            //คำอธิบาย
            //if (DescriptionPosition == "right" || DescriptionPosition == "top" || DescriptionPosition == "bottom" || DescriptionPosition == "left")
            view.Add(prefix + "DescriptionList", GetReturnSettingValue<string>(ModuleName, "description", ""));

            #endregion
            return view;

        }
            #endregion

        #endregion

        #region SetUploadFlash
        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// </summary>
        public void SetUploadFlash(string ModuleName)
        {
            SetUploadFlash(ModuleName, "", 0);
        }

        /// <summary>
        /// ใช้สำหรับ Set ค่าก่อน Upload รูป
        /// <param name="ModuleName">ชื่อโมดูล</param>
        /// <param name="imgFileName">ชื่อไฟล์รูป</param>
        /// <param name="PrimeID">Primery Key</param>
        /// </summary>
        public void SetUploadFlash(string ModuleName, string imgFileName, int? PrimeID)
        {
            #region Member
            string prefix = GetReturnSettingValue<string>(ModuleName, "prefixupload", "Flash");

            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            bool IsAvatar = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            bool IsShowPreview = GetReturnSettingValue<bool>(ModuleName, "isshowpreview", false);

            string PrefixPath = SetPrefixPath(IsSoGoodAdmin);
            string DescriptionPosition = GetReturnSettingValue<string>(ModuleName, "descriptionposition", "");
            string AllowFormat = GetReturnSettingValue<string>(ModuleName, "allowformat", "");

            int[] size = GetReturnSettingValue<int[]>(ModuleName, "maxsize", new int[] { 0, 0 });

            int MaxWidth = size[0];
            int MaxHeight = size[1];
            #endregion

            #region Set Value
            ViewData["PrefixSetFlashUpload"] = prefix;
            ViewData[prefix + "ModuleName"] = GetReturnSettingValue<string>(ModuleName, "modulename", "");

            #region Controller name
            ViewData[prefix + "ControllerName"] = GetReturnSettingValue<string>(ModuleName, "controllername", "../Base");
            #endregion

            #region Image FileType
            ViewData[prefix + "AllowFormat"] = AllowFormat;
            #endregion

            #region Image Path
            ViewData[prefix + "PrefixPath"] = PrefixPath;
            ViewData[prefix + "UploadPath"] = SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, true);
            ViewData[prefix + "FilePath"] = SetfilePath(PrefixPath, GetReturnSettingValue<string>(ModuleName, "path", "") + "/" + PrimeID, false);
            ViewData[prefix + "FileName"] = imgFileName;
            ViewData[prefix + "NoImgFullPath"] = SetNoImgFullPath(IsAvatar);
            #endregion

            #region Image Boolean
            ViewData[prefix + "IsAvatar"] = IsAvatar;
            ViewData[prefix + "IsResize"] = GetReturnSettingValue<bool>(ModuleName, "isresize", false);
            ViewData[prefix + "IsSoGoodAdmin"] = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            #endregion

            #region Image Preview
            ViewData[prefix + "IsShowPreview"] = IsShowPreview;
            if (IsShowPreview)
            {
                int[] previewsize = GetReturnSettingValue<int[]>(ModuleName, "previewsize", new int[] { 0, 0 });
                ViewData[prefix + "PreviewWidth"] = previewsize[0];
                ViewData[prefix + "PreviewHeight"] = previewsize[1];
            }
            #endregion


            #region Image Description : ตำแหน่งของคำอธิบาย [Top|Right|Bottom]
            ViewData[prefix + "DescriptionPosition"] = DescriptionPosition;
            //คำอธิบาย
            //if (DescriptionPosition == "right" || DescriptionPosition == "top" || DescriptionPosition == "bottom" || DescriptionPosition == "left")
            ViewData[prefix + "DescriptionList"] = GetReturnSettingValue<string>(ModuleName, "description", "");

            #endregion
        }
            #endregion

        #endregion

        #endregion

        #region UploadFiles
        /// <summary>
        /// UploadImages ทีละ PrimeID
        /// </summary>
        /// <param name="ModuleName"> ชื่อโมดูล </param>
        /// <param name="fileOldPath"> ImgUploadPath </param>
        /// <param name="models">model ที่ set ค่าต่างๆ มาเรียบร้อยแล้ว เช่น dtoCategory.Categories</param>
        /// <param name="PrimeID"> PrimeID ของ Item นั้นๆ </param>
        public void UploadFiles(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID)
        {
            UploadFiles(ModuleName, fileOldPath, models, PrimeID, "");
        }

        /// <summary>
        /// UploadImages ทีละ PrimeID [มี subfolder]
        /// </summary>
        /// <param name="ModuleName"> ชื่อโมดูล </param>
        /// <param name="fileOldPath"> ImgUploadPath </param>
        /// <param name="models">model ที่ set ค่าต่างๆ มาเรียบร้อยแล้ว เช่น dtoCategory.Categories</param>
        /// <param name="PrimeID"> PrimeID ของ Item นั้นๆ </param>
        /// <param name="subFolderPath"> Folder ที่อยู่ต่อจาก PrimeID เช่น ใน Product มีโฟลเดอร์รูปแยกตามสีของสินค้า ในแต่ละ PrimeID </param>
        public void UploadFiles(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID, string subFolderPath)
        {
            bool? avatar = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            bool square = avatar == true ? true : false;
            string root = res.Pageviews.BlobStorageUrl;
            #region filePath & fileOldPath
            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            string Prefix = SetPrefixPath(IsSoGoodAdmin);
            //          string filePath = "~/" + SetfilePath(Prefix, SetImgPath(ModuleName), false) + EnCodeID(PrimeID.Value) + "/";
            string filePath = root + SetfilePath(Prefix, GetReturnSettingValue<string>(ModuleName, "path", ""), false) + PrimeID + (!string.IsNullOrEmpty(subFolderPath) ? "/" + subFolderPath : "") + "/";
            fileOldPath = root + SetfilePath(Prefix, fileOldPath, false);

            string KeyName = GetReturnSettingValue<string>(ModuleName, "keyname", "");
            bool IsResize = GetReturnSettingValue<bool>(ModuleName, "isresize", false);

            #endregion

            if (filePath != fileOldPath)
            {
                DeleteDir(filePath);

                //#region Create Image
                //foreach (object obj in models)
                //{
                //    string fileName = obj.GetType().GetProperty(KeyName).GetValue(obj, null).ToString();
                //    string OldphysicalPath = Path.Combine(Server.MapPath(fileOldPath), fileName);

                //    #region Create Folder
                //    DirectoryInfo DirInfo = new DirectoryInfo(Server.MapPath(filePath));
                //    if (!DirInfo.Exists) { DirInfo.Create(); }
                //    #endregion

                //    var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);

                //    #region ถ้าไม่มีรูปใน โฟลเดอร์ [PrimeID]
                //    if (!System.IO.File.Exists(physicalPath) && System.IO.File.Exists(OldphysicalPath))
                //    {
                //        #region Check Same New File
                //        var i = 0;
                //        while (System.IO.File.Exists(physicalPath))
                //        {
                //            i += 1;
                //            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "-", i, Path.GetExtension(fileName));
                //            physicalPath = Path.Combine(Server.MapPath(filePath), fileName);
                //        };

                //        System.IO.File.Move(OldphysicalPath, physicalPath);
                //        GetExistFile(physicalPath);
                //        DeleteDir(OldphysicalPath);
                //        #endregion
                //    }

                //    #endregion
                //}
                //#endregion
            }
        }
        #endregion

        #region UploadImages
        /// <summary>
        /// UploadImages ทีละ PrimeID
        /// </summary>
        /// <param name="ModuleName"> ชื่อโมดูล </param>
        /// <param name="fileOldPath"> ImgUploadPath </param>
        /// <param name="models">model ที่ set ค่าต่างๆ มาเรียบร้อยแล้ว เช่น dtoCategory.Categories</param>
        /// <param name="PrimeID"> PrimeID ของ Item นั้นๆ </param>
        public void UploadImages(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID)
        {
            UploadImages(ModuleName, fileOldPath, models, PrimeID, "");
        }

        /// <summary>
        /// UploadImages ทีละ PrimeID [มี subfolder]
        /// </summary>
        /// <param name="ModuleName"> ชื่อโมดูล </param>
        /// <param name="fileOldPath"> ImgUploadPath </param>
        /// <param name="models">model ที่ set ค่าต่างๆ มาเรียบร้อยแล้ว เช่น dtoCategory.Categories</param>
        /// <param name="PrimeID"> PrimeID ของ Item นั้นๆ </param>
        /// <param name="subFolderPath"> Folder ที่อยู่ต่อจาก PrimeID เช่น ใน Product มีโฟลเดอร์รูปแยกตามสีของสินค้า ในแต่ละ PrimeID </param>
        public void UploadImages(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID, string subFolderPath)
        {
            bool? avatar = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            bool square = avatar == true ? true : false;

            #region filePath & fileOldPath
            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "isavatar", false);
            string Prefix = SetPrefixPath(IsSoGoodAdmin);
            //          string filePath = "~/" + SetfilePath(Prefix, SetImgPath(ModuleName), false) + EnCodeID(PrimeID.Value) + "/";
            string filePath = "~/" + SetfilePath(Prefix, GetReturnSettingValue<string>(ModuleName, "path", ""), false) + PrimeID + (!string.IsNullOrEmpty(subFolderPath) ? subFolderPath : "") + "/";
            fileOldPath = "~/" + SetfilePath(Prefix, fileOldPath, false);

            string KeyName = GetReturnSettingValue<string>(ModuleName, "keyname", "");
            bool IsResize = GetReturnSettingValue<bool>(ModuleName, "isresize", false);

            #endregion

            if (filePath != fileOldPath)
            {
                DeleteDir(filePath);

                #region Create Image
                foreach (object obj in models)
                {
                    string fileName = obj.GetType().GetProperty(KeyName).GetValue(obj, null).ToString();
                    string OldphysicalPath = Path.Combine(Server.MapPath(fileOldPath), fileName);

                    #region Create Folder
                    DirectoryInfo DirInfo = new DirectoryInfo(Server.MapPath(filePath));
                    if (!DirInfo.Exists) { DirInfo.Create(); }
                    #endregion

                    var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);

                    #region ถ้าไม่มีรูปใน โฟลเดอร์ [PrimeID]
                    if (!System.IO.File.Exists(physicalPath) && System.IO.File.Exists(OldphysicalPath))
                    {
                        #region Check Same New File
                        var i = 0;
                        while (System.IO.File.Exists(physicalPath))
                        {
                            i += 1;
                            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "-", i, Path.GetExtension(fileName));
                            physicalPath = Path.Combine(Server.MapPath(filePath), fileName);
                        };

                        Bitmap objThumbSize;

                        System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(OldphysicalPath);

                        Bitmap objFullSize;
                        #endregion

                        #region Check New File


                        #region IsResize=true [File.Save]
                        if (IsResize == true)
                        {
                            float intWidth = objGraphic.Width;
                            float intHeight = objGraphic.Height;

                            #region Set New Image Size
                            int[] size = GetReturnSettingValue<int[]>(ModuleName, "maxsize", new int[] { 0, 0 });
                            int maxWidth = (objGraphic.Width > objGraphic.Height) ? size[0] : size[1];
                            int maxHeight = (objGraphic.Width > objGraphic.Height) ? size[1] : size[0];

                            maxWidth = maxWidth == 0 ? objGraphic.Width : maxWidth;
                            maxHeight = maxHeight == 0 ? objGraphic.Height : maxHeight;

                            double whRatio = (double)(intWidth / intHeight);

                            //if (square || avatar.Value)
                            //{
                            //    maxWidth = (avatar.Value) ? 100 : 800;
                            //    maxHeight = (avatar.Value) ? 100 : 800;
                            //    thumbSize = (avatar.Value) ? 50 : 100;
                            //}

                            #endregion

                            #region Resize
                            if (intWidth > maxWidth || intHeight > maxHeight)
                            {
                                intHeight = ((float)objGraphic.Height / (float)objGraphic.Width) * (float)maxWidth;
                                if (intHeight > maxHeight)
                                {
                                    intWidth = (maxHeight * maxWidth) / (int)intHeight;
                                    intHeight = maxHeight;
                                }

                                if (intWidth > maxWidth)
                                {
                                    intWidth = maxWidth;
                                    intHeight = (maxHeight * maxWidth) / (int)intWidth;
                                }
                            }
                            #endregion

                            #region Create Picture
                            objFullSize = new Bitmap(objGraphic, (int)intWidth, (int)intHeight);
                            objFullSize.Save(physicalPath, objGraphic.RawFormat);
                            GetExistFile(physicalPath);
                            #endregion

                            #region Create Thumbnail
                            List<int>[] thumbsize = GetReturnSettingValue<List<int>[]>(ModuleName, "thumbsize", new List<int>[] { new List<int> { 0, 0 } });
                            string[] filetype = fileName.Split('.');
                            foreach (var item in thumbsize)
                            {
                                var physicalPathThumb = Path.Combine(Server.MapPath(filePath), "thumb-" + item[0] + "x" + item[1] + "-" + Path.GetFileNameWithoutExtension(fileName) + "." + filetype[1]);
                                //int thumbSize = 100;
                                //int tw = (intWidth >= intHeight) ? thumbSize : (int)(thumbSize * whRatio);
                                //int th = (intWidth >= intHeight) ? (int)(thumbSize / whRatio) : thumbSize;
                                //int posX = (thumbSize - tw) / 2;
                                //int posY = (thumbSize - th) / 2;
                                objThumbSize = new Bitmap(objGraphic.GetThumbnailImage(item[0], item[1], null, IntPtr.Zero));
                                //Graphics graphic = Graphics.FromImage(objThumbSize);
                                //graphic.Clear(Color.White);
                                //graphic.DrawImage(objGraphic, new Rectangle(posX, posY, tw, th), new Rectangle(0, 0, item[0], item[1]), GraphicsUnit.Pixel);
                                objThumbSize.Save(physicalPathThumb, objGraphic.RawFormat);
                                GetExistFile(physicalPathThumb);
                            }
                            #endregion

                            //objThumbSize = new Bitmap(objGraphic.GetThumbnailImage((int)intWidth / 2, (int)intHeight / 2, null, IntPtr.Zero));
                            //Graphics graphic = Graphics.FromImage(objThumbSize);
                            //graphic.Clear(Color.White);
                            //graphic.DrawImage(objGraphic, new Rectangle(posX, posY, tw, th), new Rectangle(0, 0, (int)intWidth, (int)intHeight), GraphicsUnit.Pixel);
                            //objThumbSize.Save(physicalPathThumb, objGraphic.RawFormat);
                            objGraphic.Dispose();
                            objGraphic = null;
                            objFullSize = null;
                            objThumbSize = null;
                            DeleteDir(fileOldPath);
                        }
                        #endregion

                        #region IsResize=false [File.Move]
                        else
                        {
                            #region movefile to real path
                            //save ได้เลย
                            int[] size = GetReturnSettingValue<int[]>(ModuleName, "maxsize", new int[] { 0, 0 });
                            objFullSize = new Bitmap(objGraphic, size[0] == 0 ? objGraphic.Width : size[0], size[1] == 0 ? objGraphic.Height : size[1]);
                            //objFullSize.Save(physicalPathThumb, objGraphic.RawFormat);
                            objGraphic.Dispose();
                            objFullSize = null;
                            objThumbSize = null;
                            System.IO.File.Move(OldphysicalPath, physicalPath);
                            GetExistFile(physicalPath);
                            #endregion

                            #region Create Thumbnail
                            if (GetReturnSettingValue<List<int>[]>(ModuleName, "thumbsize", null) != null)
                            {
                                List<int>[] thumbsize = GetReturnSettingValue<List<int>[]>(ModuleName, "thumbsize", new List<int>[] { new List<int> { 0, 0 } });
                                string[] filetype = fileName.Split('.');
                                System.Drawing.Image newObjGraphic = System.Drawing.Image.FromFile(physicalPath);
                                foreach (var item in thumbsize)
                                {
                                    var physicalPathThumb = Path.Combine(Server.MapPath(filePath), "thumb-" + item[0] + "x" + item[1] + "-" + Path.GetFileNameWithoutExtension(fileName) + "." + filetype[1]);
                                    //int thumbSize = 100;
                                    //int tw = (intWidth >= intHeight) ? thumbSize : (int)(thumbSize * whRatio);
                                    //int th = (intWidth >= intHeight) ? (int)(thumbSize / whRatio) : thumbSize;
                                    //int posX = (thumbSize - tw) / 2;
                                    //int posY = (thumbSize - th) / 2;
                                    objThumbSize = new Bitmap(newObjGraphic.GetThumbnailImage(item[0], item[1], null, IntPtr.Zero));
                                    //Graphics graphic = Graphics.FromImage(objThumbSize);
                                    //graphic.Clear(Color.White);
                                    //graphic.DrawImage(objGraphic, new Rectangle(posX, posY, tw, th), new Rectangle(0, 0, item[0], item[1]), GraphicsUnit.Pixel);
                                    objThumbSize.Save(physicalPathThumb, newObjGraphic.RawFormat);
                                    GetExistFile(physicalPathThumb);
                                }
                            }
                            objGraphic.Dispose();
                            objGraphic = null;
                            objFullSize = null;
                            objThumbSize = null;
                            DeleteDir(OldphysicalPath);


                        }
                            #endregion
                        #endregion

                        #endregion

                    }
                    else
                    {

                    }

                    #endregion

                }
                #endregion
            }
        }
        #endregion

        #region UploadVideos
        public void UploadVideos(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID)
        {
            UploadVideos(ModuleName, fileOldPath, models, PrimeID, "");
        }


        public void UploadVideos(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID, string subFolderPath)
        {
            #region filePath & fileOldPath
            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            string Prefix = SetPrefixPath(IsSoGoodAdmin);
            //          string filePath = "~/" + SetfilePath(Prefix, SetImgPath(ModuleName), false) + EnCodeID(PrimeID.Value) + "/";
            string filePath = "~/" + SetfilePath(Prefix, GetReturnSettingValue<string>(ModuleName, "path", ""), false) + (!string.IsNullOrEmpty(subFolderPath) ? "/" + subFolderPath + "/" : "");
            fileOldPath = "~/" + SetfilePath(Prefix, fileOldPath, false);

            string KeyName = GetReturnSettingValue<string>(ModuleName, "keyname", "");
            string ThumbFileName = GetReturnSettingValue<string>(ModuleName, "thumbsize", "");

            if (filePath != fileOldPath)
            {
                DeleteDir(filePath);

                foreach (object obj in models)
                {
                    string fileName = obj.GetType().GetProperty(KeyName).GetValue(obj, null).ToString();
                    string OldphysicalPath = Path.Combine(Server.MapPath(fileOldPath), fileName);

                    #region Create Folder
                    DirectoryInfo DirInfo = new DirectoryInfo(Server.MapPath(filePath));
                    if (!DirInfo.Exists) { DirInfo.Create(); }
                    #endregion

                    var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);

                    #region ถ้าไม่มีรูปใน โฟลเดอร์ [PrimeID]
                    if (!System.IO.File.Exists(physicalPath) && System.IO.File.Exists(OldphysicalPath))
                    {
                        #region IsResize=false [File.Move]
                        System.IO.File.Move(OldphysicalPath, physicalPath);
                        if (!string.IsNullOrEmpty(ThumbFileName))
                        {
                            string[] size = ThumbFileName.Split('.');
                            foreach (var item in size)
                            {
                                System.IO.File.Move(Path.Combine(Server.MapPath(fileOldPath), item + Path.GetFileNameWithoutExtension(fileName) + ".jpg"), physicalPath);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
            }

            #endregion
        }
        #endregion

        #region UploadFlashs
        public void UploadFlashs(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID)
        {
            UploadFlashs(ModuleName, fileOldPath, models, PrimeID, "");
        }


        public void UploadFlashs(string ModuleName, string fileOldPath, IEnumerable<object> models, int? PrimeID, string subFolderPath)
        {
            #region filePath & fileOldPath
            bool IsSoGoodAdmin = GetReturnSettingValue<bool>(ModuleName, "issogoodadmin", false);
            string Prefix = SetPrefixPath(IsSoGoodAdmin);
            //          string filePath = "~/" + SetfilePath(Prefix, SetImgPath(ModuleName), false) + EnCodeID(PrimeID.Value) + "/";
            string filePath = "~/" + SetfilePath(Prefix, GetReturnSettingValue<string>(ModuleName, "path", ""), false) + (!string.IsNullOrEmpty(subFolderPath) ? "/" + subFolderPath + "/" : "") + PrimeID + "/";
            fileOldPath = "~/" + SetfilePath(Prefix, fileOldPath, false);

            string KeyName = GetReturnSettingValue<string>(ModuleName, "keyname", "");

            if (filePath != fileOldPath)
            {
                DeleteDir(filePath);

                foreach (object obj in models)
                {
                    string fileName = obj.GetType().GetProperty(KeyName).GetValue(obj, null).ToString();
                    string OldphysicalPath = Path.Combine(Server.MapPath(fileOldPath), fileName);

                    #region Create Folder
                    DirectoryInfo DirInfo = new DirectoryInfo(Server.MapPath(filePath));
                    if (!DirInfo.Exists) { DirInfo.Create(); }
                    #endregion

                    var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);

                    #region ถ้าไม่มีรูปใน โฟลเดอร์ [PrimeID]
                    if (!System.IO.File.Exists(physicalPath) && System.IO.File.Exists(OldphysicalPath))
                    {
                        #region IsResize=false [File.Move]
                        System.IO.File.Move(OldphysicalPath, physicalPath);
                        #endregion

                        GetExistFile(physicalPath);
                    }
                    #endregion
                }
            }

            #endregion
        }
        #endregion

        #region DeleteDir
        private bool DeleteDir(string Directory)
        {
            bool IsResult = true;
            try
            {
                DeleteDirectory(Server.MapPath(Directory));
                IsResult = true;
            }
            catch
            {
                IsResult = false;
            }
            return IsResult;
        }
        #endregion

        #region GetExistFile
        public void GetExistFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                while (!System.IO.File.Exists(filePath))
                {
                    Thread.Sleep(100);
                }
            }
        }
        #endregion

        #region DeleteDirectory
        #region DeleteDirectory
        public static void DeleteDirectory(string target_dir)
        {
            DeleteDirectoryFiles(target_dir);
            while (Directory.Exists(target_dir))
            {
                DeleteDirectoryDirs(target_dir);
            }
        }
        #endregion

        #region DeleteDirectoryDirs
        private static void DeleteDirectoryDirs(string target_dir)
        {
            System.Threading.Thread.Sleep(100);

            if (Directory.Exists(target_dir))
            {

                string[] dirs = Directory.GetDirectories(target_dir);

                if (dirs.Length == 0)
                    Directory.Delete(target_dir, false);
                else
                    foreach (string dir in dirs)
                        DeleteDirectoryDirs(dir);
            }
        }
        #endregion

        #region DeleteDirectoryFiles
        private static void DeleteDirectoryFiles(string target_dir)
        {
            if (System.IO.Directory.Exists(System.IO.Path.Combine(target_dir)))
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectoryFiles(dir);
                }
            }
        }
        #endregion
        #endregion

        #region Generate Code
        #region GetTimeSecond
        public string GetTimeSecond()
        {
            double totalSeconds = (double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            return totalSeconds.ToString().Replace(".", "");
        }
        #endregion

        #region GetRandomCode
        /// <summary>GetRandomCode : สุ่มตัวอักษร 4 ตัว หรือตามที่กำหนด
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRandomCode()
        {
            return GetRandomCode(4);
        }
        public string GetRandomCode(int maxLength)
        {
            maxLength = (maxLength <= 0) ? 4 : maxLength;
            string allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
            //string dash = "-";
            char[] chars = new char[maxLength];
            Random rd = new Random();

            for (int i = 0; i < maxLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        #endregion

        #region GenPassword
        /// <summary>RandomPassword : สร้างรหัสผ่านแบบสุ่ม
        /// </summary>
        /// <returns></returns>
        public string GenPassword()
        {
            int maxLength = 8;
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //string dash = "-";
            char[] chars = new char[maxLength];
            Random rd = new Random();

            for (int i = 0; i < maxLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        #endregion


        #region GenFileName
        /// <summary> GenFileName : สร้างชื่อรูปภาพ
        /// </summary>
        /// <param name="maxLength"></param>
        /// <param name="prefix"></param>
        /// <param name="subfix"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string GenFileName(int maxLength = 10, string prefix = "", string subfix = "")
        {
            if (maxLength <= 0)
                return "";

            //if (separator != "-" || separator != "_")
            //    separator = "-";

            // prefix-Gerogkdfoghpfgphldfj-subfix

            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //string dash = "-";
            char[] chars = new char[maxLength];
            Random rd = new Random();

            for (int i = 0; i < maxLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        #endregion


        #endregion

        #region Static SetUpload_Image
        public static ViewDataDictionary<dynamic> SetUpload_Image(string ModuleName, string ImgFilePath, int? PrimeID, ViewDataDictionary<dynamic> view)
        {
            BaseController bc = new BaseController();
            ViewDataDictionary<dynamic> vd = new ViewDataDictionary<dynamic>();
            vd = bc.SetUploadImage(ModuleName, ImgFilePath, PrimeID, view);
            return vd;
        }
        #endregion

        #region Static SetUpload_File
        public static ViewDataDictionary<dynamic> SetUpload_File(string ModuleName, string ImgFilePath, int? PrimeID, ViewDataDictionary<dynamic> view)
        {
            BaseController bc = new BaseController();
            ViewDataDictionary<dynamic> vd = new ViewDataDictionary<dynamic>();
            vd = bc.SetUploadFile(ModuleName, ImgFilePath, PrimeID, view);
            return vd;
        }
        #endregion

        #endregion

        #region SaveImages
        public ActionResult SaveImages(IEnumerable<HttpPostedFileBase> attachments, string filepath, bool? extention, bool? IsAvatar, int? Index, bool? IsValidateImgSize, int? MaxWidth, int? MaxHeight, string UploadName)
        {
            imgManager = new FileHelper();
            string fileName = string.Empty;
            string oldFileName = string.Empty;
            float fileSize = 0;

            #region Check Path 

            #endregion

            foreach (var file in attachments)
            {
                try
                { 
                        imgManager.UploadImage(filepath, file); 
                }
                catch (Exception ex) { }
            }

            return Json(new { fileSize = fileSize, newImgFile = imgManager.ImageName, oldFileName = oldFileName, newImgPath = filepath.Replace("~/", ""), IsAvatar = IsAvatar, Index = Index, UploadName = UploadName }, "text/plain");
        }
        #endregion

        #region RemoveImages
        public ActionResult RemoveImages(string[] fileNames, string filePath, bool? IsAvatar, string thumbnailSize)
        {
            if (filePath.Contains("Temp"))
            {
                #region DeleteRealPicture
                if (!string.IsNullOrEmpty(filePath))
                {
                    foreach (var fullName in fileNames)
                    {
                        var fileName = Path.GetFileName(fullName);
                        var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }

                }
                #endregion

                #region DeleteThumbnail
                if (!string.IsNullOrEmpty(thumbnailSize))
                {
                    string[] thumbsplit = thumbnailSize.Split(',');
                    foreach (var fullName in fileNames)
                    {
                        string[] fileType = fullName.Split('.');
                        foreach (var item in thumbsplit)
                        {
                            var physicalThumbPath = Path.Combine(Server.MapPath(filePath), "thumb-" + Path.GetFileNameWithoutExtension(fullName) + "-" + item + "." + fileType[1]);
                            if (System.IO.File.Exists(physicalThumbPath))
                            {
                                System.IO.File.Delete(physicalThumbPath);
                            }
                        }
                    }
                }
                #endregion

            }

            return Json(new { newImgFile = "", newImgPath = "", IsAvatar = IsAvatar }, "text/plain");
        }
        #endregion

        #region RemoveFiles
        public ActionResult RemoveFiles(string[] fileNames, string filePath, bool? IsAvatar, string thumbnailSize)
        {
            #region DeleteRealPicture
            if (!string.IsNullOrEmpty(filePath))
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    //var physicalPath = Path.Combine(Server.MapPath(filePath), fileName);
                    var physicalPath = Path.Combine(filePath, fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }

            }
            #endregion

            //#region DeleteThumbnail
            //if (!string.IsNullOrEmpty(thumbnailSize))
            //{
            //    string[] thumbsplit = thumbnailSize.Split(',');
            //    foreach (var fullName in fileNames)
            //    {
            //        string[] fileType = fullName.Split('.');
            //        foreach (var item in thumbsplit)
            //        {
            //            var physicalThumbPath = Path.Combine(Server.MapPath(filePath), "thumb-" + Path.GetFileNameWithoutExtension(fullName) + "-" + item + "." + fileType[1]);
            //            if (System.IO.File.Exists(physicalThumbPath))
            //            {
            //                System.IO.File.Delete(physicalThumbPath);
            //            }
            //        }
            //    }
            //}
            //#endregion

            return Json(new { newImgFile = "", newImgPath = "", IsAvatar = IsAvatar }, "text/plain");
        }
        #endregion


        #region LoadHotSuccessStory
        public void LoadHotSuccessStory()
        {
            var svArticle = new ArticleService();
            /*Hot Success Story*/
            var HotStory = new List<view_b2bArticle>();
            HotStory = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7 AND IsHot = 1", "  NEWID()", 1, 1);
            if (HotStory.Count > 0)
            {
                //HotStory = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7 AND IsHot = 1", "ModifiedDate DESC ", 1, 1);
                ViewBag.HotStory = HotStory.First();
            }
            else
            {
                ViewBag.HotStory = null;
            }
        }
        #endregion

        #region Bitly ShortenUrl

        private const string apiKey = "R_ef51b62b84935f8c8f381357c2ca9d80";
        private const string login = "o_15452av11m";

        public static BitlyResults ShortenUrl(string longUrl)
        {
            var url =
                string.Format("http://api.bit.ly/shorten?format=xml&version=2.0.1&longUrl={0}&login={1}&apiKey={2}",
                              HttpUtility.UrlEncode(longUrl), login, apiKey);
            var resultXml = XDocument.Load(url);
            var x = (from result in resultXml.Descendants("nodeKeyVal")
                     select new BitlyResults
                     {
                         UserHash = result.Element("userHash").Value,
                         ShortUrl = result.Element("shortUrl").Value
                     }
                    );
            return x.Single();
        }

        public class BitlyResults
        {
            public string UserHash { get; set; }

            public string ShortUrl { get; set; }
        }
        #endregion

        #region GetFacebookLoginUrl
        public const string FACEBOOK_USER_DENIED = "user_denied";
        public const string FACEBOOK_ERROR_REASON = "error_reason";
       // public const string FACEBOOK_LOGIN_COMPLETE_URL = res.Pageviews.UrlWeb + "/Member/SignUp";

        public static string GetFacebookLoginUrl(string returnUrl)
        {
            var FACEBOOK_APP_ID = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Key"];
            var FACEBOOK_SECRET = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Secret"];
            var FACEBOOK_SCOPE = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Scope"];
            var url = string.Format(@"https://www.facebook.com/dialog/oauth/?client_id={0}&redirect_uri={1}&scope={2}&state={3}",
                      FACEBOOK_APP_ID, res.Pageviews.UrlWeb + "/Member/SignUp", FACEBOOK_SCOPE, (returnUrl ?? "http://www.ouikum.com"));
            return url;
        }
        #endregion

        //search 
        //#region Provinces
        //public void LoadProvinces()
        //{
        //    AddressService svAddress = new AddressService();
        //    var Provinces = new List<view_Province_Region>();
        //    var name = "province-region";
        //    if (MemoryCache.Default[name] != null)
        //    {
        //        Provinces = (List<view_Province_Region>)MemoryCache.Default[name];
        //    }
        //    else
        //    {

        //        Provinces = svAddress.SelectData<view_Province_Region>("*", "IsDelete = 0", "RegionID ASC");
        //        if (Provinces != null && svAddress.TotalRow > 0)
        //        {
        //            MemoryCache.Default.Add(name, Provinces, DateTime.Now.AddDays(1));
        //        }
        //    }


        //    var SQLSelect_Provinces = "";
        //    if (Base.AppLang == "en-US")
        //        SQLSelect_Provinces = "ProvinceID,ProvinceNameEng AS ProvinceName,RegionNameEng AS RegionName";
        //    else
        //        SQLSelect_Provinces = " ProvinceID,ProvinceName,RegionName";
        //     Provinces = svAddress.SelectData<view_Province_Region>(SQLSelect_Provinces, "IsDelete = 0", "RegionID ASC");

        //    //if (MemoryCache.Default["SearchByServiceType"] != null)
        //    //{
        //    //    EnumServiceType = (List<view_EnumData>)MemoryCache.Default["SearchByServiceType"] ;
        //    //}
        //    //else
        //    //{
        //    //    EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
        //    //    if (EnumServiceType != null && svCommon.TotalRow > 0)
        //    //    {
        //    //        MemoryCache.Default.Add("SearchByServiceType", EnumServiceType, DateTime.Now.AddDays(1));
        //    //    }
        //    //}



        //    ViewBag.Provinces = Provinces;
        //}
        //#endregion

        #region Provinces
        public void LoadProvinces()
        {
            AddressService svAddress = new AddressService();
            var Provinces = new List<view_Province_Region>();
            if (MemoryCache.Default["viewProvinceRegion"] != null)
            {
                Provinces = (List<view_Province_Region>)MemoryCache.Default["viewProvinceRegion"];
            }
            else
            {
                Provinces = svAddress.SelectData<view_Province_Region>("*", "IsDelete = 0", "RegionID ASC");
                if (Provinces != null && svAddress.TotalRow > 0)
                {
                    MemoryCache.Default.Add("viewProvinceRegion", Provinces, DateTime.Now.AddDays(1));
                }
            } 

            ViewBag.Provinces = Provinces;
        }
        #endregion

        #region Biztype
        public void LoadBiztype()
        {
            BizTypeService svBizType = new BizTypeService();

            var SQLSelect_BizT = "";
            //if (Base.AppLang == "en-US")
            //    SQLSelect_BizT = "BizTypeID,BizTypeCode AS BizTypeName";
            //else

            SQLSelect_BizT = " BizTypeID,BizTypeName";
            var Biztypes = svBizType.SelectData<b2bBusinessType>(SQLSelect_BizT, "IsDelete = 0", "BizTypeName ASC");

            ViewBag.BizType = Biztypes;
        }
        #endregion

        #region LoadCategory
        public void LoadCategory()
        {
             var svCategory = new CategoryService();
            #region Load Category
            ViewBag.IndrustryCateLV1 = svCategory.ListIndrustryCategory();
            ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory();
            #endregion
        }
        #endregion

        #region aptcha
        public CaptchaResult GetCaptcha(string id)
        {
            string captchaText = Captcha.GenerateRandomCode();
            HttpContext.Session.Add("captcha_"+ id, captchaText);
            return new CaptchaResult(captchaText);
        }
        #endregion

        public List<view_SearchProduct> MappingProductMongo(List<Ouikum.Web.Models.tbProduct> model)
        {
            var data = new List<view_SearchProduct>();
            var svBizType = new BizTypeService();
            var biztype = svBizType.GetBiztypeAll();

            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    var it = new view_SearchProduct();
                    it.ProvinceName = item.ProvinceName;
                    //   it.ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                    if (item.BizTypeID != 13)
                    {
                        it.BizTypeName = biztype.Where(m => m.BizTypeID == item.BizTypeID).FirstOrDefault().BizTypeName;
                    }
                    else
                    {
                        it.BizTypeName = item.BizTypeOther;
                    }
                    it.ProductID = (int)item.ProductID;
                    it.ProductName = item.ProductName;
                    it.ProductImgPath = item.ProductImgPath;
                    it.ProductKeyword = item.ProductKeyword;
                    it.ShortDescription = item.ShortDescription;
                    it.ViewCount = item.ViewCount;
                    it.CompID = item.CompID;
                    it.ContactCount = (short)item.ContactCount;
                    it.comprowflag = 2;
                    it.CompLevel = (byte)item.Complevel;
                    it.CompName = item.CompName;
                    it.ListNo = item.ListNo;
                    it.CreatedDate = item.CreatedDate;
                    it.ModifiedDate = item.ModifiedDate;
                    it.IsSME = false;

                    data.Add(it);
                }
            }

            return data;
        }

        #region Check Cookie Device

        public void CheckPage(int? id, int PageDevice)
        {
            var device = Request.Browser.IsMobileDevice;
            // True = Mobile || False = Desktop

            if (PageDevice == 1)
            {
                BaseSC _bc = new BaseSC();
                var CName = _bc.GetByID<b2bCompany>("CompID", Convert.ToString(id));

                HttpCookie myCookie = new HttpCookie("CheckDevice");
                myCookie["device"] = Convert.ToString(device);
                myCookie["PageCheck"] = Convert.ToString(PageDevice);
                myCookie["idCheck"] = Convert.ToString(id);
                myCookie["CompName"] = Convert.ToString(CName.CompName);
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
            }
            else if (PageDevice == 2)
            {
                BaseSC _bc = new BaseSC();
                var PName = _bc.GetByID<b2bProduct>("ProductID", Convert.ToString(id));

                HttpCookie myCookie = new HttpCookie("CheckDevice");
                myCookie["device"] = Convert.ToString(device);
                myCookie["PageCheck"] = Convert.ToString(PageDevice);
                myCookie["idCheck"] = Convert.ToString(id);
                myCookie["CompID"] = Convert.ToString(PName.CompID);
                myCookie["ProName"] = Convert.ToString(PName.ProductName);
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
            }
        }

        #endregion

        #region Count PageView

        public void countpageview(int compid)
        {
            
        }

        #endregion
    }


}