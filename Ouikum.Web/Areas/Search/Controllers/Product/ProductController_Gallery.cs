using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Search
{
    public partial class ProductController : BaseController
    {
        #region Gallery
        [HttpGet]
        public ActionResult Gallery(int? id)
        {

            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            RememberURL();
            
            var sqlWhere = string.Empty;             
            
            if (LogonServiceType == 12 || LogonServiceType == 9) 
                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.Admin); 
            else
                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.All); 

            sqlWhere += " AND ProductID = " + id;
            var Products = svProduct.SelectData<view_Product>("*", sqlWhere, null, 1, 0, false);

            if (svProduct.TotalRow == 0)
            {
              return  Redirect("~/Default/NotFound");
            }

            var product = Products.First();

            if (Products != null)
            {
                ViewBag.ProductCount = Products;
                if (product.ProvinceName == res.Product.lblBangkok)
                {
                    product.ProvinceName = res.Product.lblBkk;
                }
                ViewBag.Title = res.Product.lblImage + " : " + product.ProductName + "-" + product.ProvinceName + " | " + res.Common.lblDomainShortName;
                ViewBag.ProductName = product.ProductName;
                ViewBag.ProductCode = product.ProductCode;
                ViewBag.ProductDetail = product;
                int CompID = (int)product.CompID;
                //var sqlwherein = "";
                //switch (res.Common.lblWebsite)
                //{
                //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
                //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
                //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
                //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
                //    default: sqlwherein = ""; break;
                //}
                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd, product.CompID);
                //sqlWhere += sqlwherein;
                //var ImgProducts = svProduct.SelectData<view_SearchProduct>("ProductID,ProductImgPath,ProductName,Price,MinOrderQty,QtyUnit,BizTypeName,ViewCount,Qty", sqlWhere, null, 1, 0, false);
                //ViewBag.ProductImageAll = ImgProducts;
                //ViewBag.ImageCount = ImgProducts.Count;
                var PremiumProducts = new List<view_HotFeaProduct>();
                var SQLSelect_Feat = "";
                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,RowFlag,CompRowFlag,Price,ViewCount";
                PremiumProducts = svProduct.SelectData<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'P' AND ProductID > 0 AND ProRowFlag in(4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID() , Price DESC", 1, 9);//""
                ViewBag.PremiumProducts = PremiumProducts;
                ViewBag.ImageCount = PremiumProducts.Count;

                var ProductImages = svProductImage.SelectData<view_ProductImage>("*", "ProductID = " + id, null, 1, 0, false);
                if (ProductImages.Count == 0)
                {
                    var ProductImage = svProduct.SelectData<view_Product>("ProductImgPath", "ProductID = " + id, null, 1, 0, false);
                    ViewBag.ProductImages = ProductImage;
                    ViewBag.ProductImage = ProductImages;
                }
                else
                {
                    ViewBag.ProductImage = ProductImages;
                }

                var Companies = svCompany.SelectData<view_Company>("*", "CompID = " + CompID, null, 1, 0, false);
                ViewBag.Company = Companies.First();

                var Shipments = svCompany.SelectData<b2bCompanyShipment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
                if (Shipments.Count > 0)
                ViewBag.CompanyShipment = Shipments;

                var Payments = svCompany.SelectData<view_CompanyPayment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
                if (Payments.Count > 0)
                ViewBag.CompanyPayments = Payments;

                ViewBag.CompID = CompID;

                if (!string.IsNullOrEmpty(product.ProductKeyword))
                {
                    ViewBag.MetaKeyword = res.Product.lblImage + " : " + product.ProductKeyword.Replace('~', ' ').Substring(0, product.ProductKeyword.Length - 1) + "-" + product.ProvinceName + " | " + res.Common.lblDomainShortName;
                }
                else
                {
                    ViewBag.MetaKeyword = ViewBag.Title;
                }
                if (!string.IsNullOrEmpty(product.ShortDescription))
                {
                    ViewBag.MetaDescription = res.Product.lblImage + " : " + product.ProductName + " " + product.ProvinceName + " " + product.ShortDescription.Replace('~', ' ').Substring(0, product.ShortDescription.Length - 1) + " | " + res.Common.lblDomainShortName;
                }
                else
                {
                    ViewBag.MetaDescription = ViewBag.Title;
                }
            }
            ViewBag.ProID = id;

            #region Send Message
            GetStatusUser();
            var user = (Ouikum.Company.UserStatusModel)ViewBag.UserStatus;
            if(user.CompID != 0)
                ViewBag.CompanyView = svCompany.SelectData<b2bCompany>("CompID,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone", "CompID = " + user.CompID, null, 1, 0, false).First();
            
            #endregion

            return View();
        }
        #endregion
    }
}
