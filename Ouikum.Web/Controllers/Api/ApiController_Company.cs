using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Product;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {

        #region SetModelCompany
        public List<CompanyAppModel> SetModelCompany(List<b2bCompany> company)
        {
            var comp = new List<CompanyAppModel>();
            foreach (var item in company)
            {
                #region CompanyAppModel
                var c = new CompanyAppModel();
                c.compid = item.CompID;
                c.compname = item.CompName;
                c.compnameeng = item.CompNameEng;
                c.complevel = (int)item.CompLevel;
                c.compaddress = item.CompAddrLine1;
                c.compemail = item.ContactEmail;
                c.comptel = item.CompPhone;
                c.compmobile = item.CompMobile;
                c.image = item.CompImgPath;
                c.logoimgpath = item.LogoImgPath;
                c.compfax = item.CompFax;
                c.compwebsite = item.CompWebsiteUrl;
                c.province = (int)item.CompProvinceID;
                c.district = (int)item.CompDistrictID;
                c.facebookid = (string.IsNullOrEmpty(item.FacebookUrl))? "": item.FacebookUrl;
                c.product = new List<ProductModel>();
                comp.Add(c);
                #endregion
            }

            return comp;
        }

        #endregion

        #region company detail
        public ActionResult companydetail(int compid)
        {
            var svComp = new CompanyService();
            var company = svComp.SelectData<b2bCompany>(" * ", " CompID = " + compid);
            var CompModel = SetModelCompany(company);
            foreach (var c in CompModel)
            {
                c.product = new List<ProductModel>();
                var svProduct = new ProductService();
                var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, compid);

                var products = svProduct.SelectData<view_ProductMobileApp>(" * ", sqlWhere, "ModifiedDate DESC");
                var gallery = new List<b2bProductImage>();
                var whereGallery = "IsDelete = 0";

                #region product gallery
                if (svProduct.TotalRow > 0)
                {
                    List<int> productid = products.Select(m => m.ProductID).ToList();
                    whereGallery += " AND " + CreateWhereIN(productid, "productid");

                    var data = svProduct.SelectData<b2bProductImage>(" * ", whereGallery);
                    foreach (var p in products)
                    {
                        var item = new ProductModel();

                        #region set product model
                        item.productid = p.ProductID;
                        item.productname = p.ProductName;
                        item.price = (decimal)p.Price;
                        item.promotionprice = (p.PromotionPrice > 0) ? (decimal)p.PromotionPrice : 0;
                        item.ispromotion = (p.IsPromotion != null) ? (bool)p.IsPromotion : false;
                        item.compname = p.CompName;
                        item.compid = (int)p.CompID;
                        item.complevel = (int)p.Complevel;
                        item.createdate = p.CreatedDate;
                        item.modifieddate = p.ModifiedDate;
                        item.imagepath = p.ProductImgPath;
                        item.productdetail = p.ProductDetail;
                        item.shortdescription = p.ShortDescription;
                        item.gallery = new List<GalleryModel>();
                        var primarygallery = new GalleryModel();
                        primarygallery.imageid = 0;
                        primarygallery.imagepath = p.ProductImgPath;
                        item.gallery.Add(primarygallery);
                        #endregion

                        if (data != null && data.Count > 0)
                        {
                            foreach (var it in data.Where(m => m.ProductID == p.ProductID))
                            {
                                var g = new GalleryModel();
                                g.imageid = it.ProductImageID;
                                g.imagepath = it.ImgPath;
                                item.gallery.Add(g);
                            }
                        }
                        c.product.Add(item);
                    }
                }
                #endregion
            } 
            return Json(new { company = CompModel }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UploadLogo
        [HttpPost]
        public ActionResult UploadLogo(HttpPostedFileBase logo, int compid)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Companies/Logo/" + compid);
            #endregion
            imgManager.UploadImage("Temp/Companies/Logo/" + compid, logo);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region Edit Company
        public ActionResult editcompany(
            int compid,
            string image,
            string name,
            string nameeng,
            string tel,
            string mobile,
            string fax,
            string email,
            string website,
            string address,
            int province,
            int district
            )
        {

            var svCompany = new CompanyService();
            var svMember = new CompanyService();
            var b2bCompany = new b2bCompany();
            var emCompanies = new emCompany();
            string compimg;
            string complogo;

            var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + compid + "");
            b2bCompany = Company.First();

            #region set ค่า b2bCompany
            compimg = b2bCompany.CompImgPath;
            complogo = b2bCompany.LogoImgPath;
            b2bCompany.LogoImgPath = image;
            b2bCompany.CompName = name;
            b2bCompany.CompNameEng = nameeng;
            b2bCompany.CompAddrLine1 = address;
            b2bCompany.CompDistrictID = district;
            b2bCompany.CompProvinceID = province;
            b2bCompany.CompPhone = tel;
            b2bCompany.CompMobile = mobile;
            b2bCompany.CompWebsiteUrl = website;
            #endregion

            #region Update b2bCompany
            svCompany.SaveData<b2bCompany>(b2bCompany, "CompID");
            #endregion

            if (svCompany.IsResult)
            {
                emCompanies = svMember.SelectData<emCompany>("*", " CompID = " + b2bCompany.emCompID).First();

                #region set ค่า emCompany
                emCompanies.LogoImgPath = b2bCompany.LogoImgPath;
                emCompanies.CompName = b2bCompany.CompName;
                emCompanies.DisplayName = b2bCompany.DisplayName;
                emCompanies.CompNameEng = b2bCompany.CompNameEng;
                emCompanies.CompAddrLine1 = b2bCompany.CompAddrLine1;
                emCompanies.CompDistrictID = b2bCompany.CompDistrictID;
                emCompanies.CompProvinceID = b2bCompany.CompProvinceID;
                emCompanies.CompPostalCode = b2bCompany.CompPostalCode;
                emCompanies.CompPhone = b2bCompany.CompPhone;
                emCompanies.CompMobile = b2bCompany.CompMobile;
                emCompanies.CompFax = b2bCompany.CompFax;
                emCompanies.BizTypeID = b2bCompany.BizTypeID;
                emCompanies.BizTypeOther = b2bCompany.BizTypeOther;
                emCompanies.CompWebsiteUrl = b2bCompany.CompWebsiteUrl;
                emCompanies.RowVersion = b2bCompany.RowVersion;
                #endregion

                #region Update emCompany
                svMember.SaveData<emCompany>(emCompanies, "CompID");
                #endregion

                if (svCompany.IsResult && svMember.IsResult)
                {
                    #region SaveLogo
                    if (image != "")
                    {
                        if (b2bCompany.LogoImgPath != complogo)
                        {
                            imgManager = new FileHelper();

                            //#region Delete Folder
                            //imgManager.DeleteFilesInDir("Companies/Logo/" + b2bCompany.CompID);
                            //#endregion
                            imgManager.DirPath = "Companies/Logo/" + b2bCompany.CompID;
                            imgManager.DirTempPath = "Temp/Companies/Logo/" + b2bCompany.CompID;
                            imgManager.ImageName = image;
                            //imgManager.ImageThumbName = "Thumb_" + form["LogoImgPath"];
                            imgManager.FullHeight = 150;
                            imgManager.FullWidth = 150;
                            //imgManager.ThumbHeight = 150;
                            //imgManager.ThumbWidth = 150;

                            imgManager.SaveImageFromTemp();
                        }
                    }
                    #endregion
                }
            }

            return Json(new { status = svCompany.IsResult }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region companylist
        public ActionResult companylist(string search, int pageindex = 0, int pagesize = 0)
        {
            var svComp = new CompanyService();
            var sqlwhere = svComp.CreateWhereAction(CompStatus.HaveProduct);

            if (!string.IsNullOrEmpty(search))
                sqlwhere += " AND (CompName Like N'%" + search + "%' )";

            var company = svComp.SelectData<b2bCompany>(" * ", sqlwhere, "", pageindex, pagesize);
            var CompModel = SetModelCompany(company);

            return Json(new { company = CompModel, totalrow = svComp.TotalRow, totalpage = svComp.TotalPage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Setting
        #region Notification

        #region Add Notification
        public ActionResult AddNotification(string appkey, bool? status  ,int compid = 0)
        {
            var svComp = new CompanyService();
            svComp.AddNotification(appkey,status,compid);

            return Json(new { status = svComp.IsResult, msg = GenerateMsgError(svComp.MsgError) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Notification
        public ActionResult GetNotification(int compid)
        {
            var svComp = new CompanyService();
            var notice = svComp.SelectData<emNotification>(" * ", " IsDelete = 0 AND CompID = " + compid);
            return Json(new { notification = notice }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Setting Notification
        public ActionResult SettingNotification(bool status, int compid)
        {
            var svComp = new CompanyService();
            svComp.SettingNotification(status, compid);
            return Json(new { status = svComp.IsResult }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete Notification
        public ActionResult DeleteNotification(string appkey, int compid)
        {
            var svComp = new CompanyService();
            svComp.DeleteNotification(appkey, compid);
            return Json(new { status = svComp.IsResult }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #endregion

        #region add followcatgory
        public ActionResult addfollowcategory(List<int> categoryid, List<int> categorylevel, int compid)
        {
            var svComp = new CompanyService();
            svComp.AddFollowCategory(categoryid, categorylevel, compid);
            return Json(new { status = svComp.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region get follow category
        public ActionResult getfollowcategory(int compid)
        {
            var svComp = new CompanyService();
            var follow = svComp.SelectData<view_FollowCatgory>(" * ", "IsDelete = 0 AND CompID = " + compid);
            var model = new List<FollowCateModel>();
            #region

            foreach (var item in follow)
            {
                var f = new FollowCateModel();
                f.id = (int)item.CategoryID;
                f.name = item.CategoryName;
                f.level = (int)item.CategoryLevel;
                f.compid = (int)item.CompID;
                f.parentid = 0;
                f.parentpath = "";
                f.subcategory = new List<CateModel>();
                model.Add(f);
                //    f.level = item.

            }
            #endregion
            return Json(new { follow = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region delete follow category
        public ActionResult deletefollowcategory(List<int> categoryid, int compid)
        {
            var svComp = new CompanyService();
            svComp.DeleteFollowCategory(categoryid, compid);
            return Json(new { status = svComp.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  Status Msg Info
        public ActionResult countinfo(int compid)
        {
            MessageService svMessage = new MessageService();
            QuotationService svQuota = new QuotationService();

            #region count message unread
            var cntMsg = svMessage.CountData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, compid));
            #endregion

            #region count matching unread
            var sqlwhere = svQuota.CreateWhereAction(QuotationAction.CountQuotation, compid);
            sqlwhere += "ANd isMatching = 1 ";
            var cntMatching = svQuota.CountData<b2bQuotation>("*", sqlwhere);
            #endregion

            #region count quotation unread
            sqlwhere = svQuota.CreateWhereAction(QuotationAction.CountQuotation, compid);
            sqlwhere += "ANd isMatching = 0 ";
            #endregion

            var cntQuota = svQuota.CountData<b2bQuotation>("*", sqlwhere);

            return Json(new { message = cntMsg, matching = cntMatching, quotation = cntQuota }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
