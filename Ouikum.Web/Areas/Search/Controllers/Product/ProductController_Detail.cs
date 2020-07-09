using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using System.Runtime.Caching;
using Ouikum.Product;
namespace Ouikum.Web.Search
{
    public partial class ProductController : BaseController
    {
        #region Detail
        [HttpGet]
        public ActionResult Detail(int? id, string name = "")
        {

            if (RedirectToProduction())
                return Redirect(UrlProduction); 

            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            RememberURL();

            int PageCheck = 2;
            CheckPage(id, PageCheck);
            
            var product =  new  view_Product();
            var sqlWhere = string.Empty;

            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}

            if (LogonServiceType == 12 || LogonServiceType == 9)
            {
                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.Admin);
                sqlWhere += " AND ProductID = " + id;
                var data = svProduct.SelectData<view_Product>(" * ", sqlWhere, null, 1, 0, false);

                if (svProduct.TotalRow > 0)
                    product = data.First();
            }
            else
            {

                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.All);

                sqlWhere += " AND ProductID = " + id   ;
                var data = svProduct.SelectData<view_Product>(" * ", sqlWhere, null, 1, 0, false);

                if (svProduct.TotalRow > 0)
                    product = data.First();
                else
                {
                    sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd);
                    sqlWhere += " AND ProductID = "+id;
                    data = svProduct.SelectData<view_Product>(" * ", sqlWhere, null, 1, 0, false);
                    if (svProduct.TotalRow > 0)
                        product = data.First();
                } 
            }


            if (svProduct.TotalRow > 0)
            {
                #region Set Product Detail
                ViewBag.ProductCount = svProduct.TotalRow;
                ViewBag.ProductName = product.ProductName;
                ViewBag.ProductCode = product.ProductCode;
                ViewBag.ProductDetail = product;
                int CompID = (int)product.CompID;
                sqlWhere = svProduct.CreateWhereAction(Product.ProductAction.FrontEnd,product.CompID);
                sqlWhere += " AND ProductID = " + id;
                //sqlWhere += sqlwherein;
                svProduct.TotalRow = 0;
                //var ImgProducts = svProduct.SelectData<view_SearchProduct>("ProductID,ProductImgPath,ProductName,ProductNameEng,Price,MinOrderQty, ViewCount,Qty", sqlWhere, null, 1, 20, false);

                //string ItemName = "";
                //if(ImgProducts != null && ImgProducts.Count() > 0){
                //    ItemName = ViewBag.CurrentLanguage == "en" ? ImgProducts.FirstOrDefault().ProductNameEng : ImgProducts.FirstOrDefault().ProductName;
                //}
                //if (Url.FriendlyEncode(name) != Url.FriendlyEncode(ItemName))
                //    return RedirectToAction("Detail", new { id = id, name = Url.FriendlyEncode(ItemName) });

                //ViewBag.ProductImageAll = ImgProducts;
                //ViewBag.ImageCount = svProduct.TotalRow;

                var ProductImages = svProductImage.SelectData<view_ProductImage>("*", "ProductID = " + id + " And Isdelete=0", null, 1, 0, false);
                ViewBag.ProductImage = ProductImages;

                var Companies = svCompany.SelectData<view_Company>("*", "CompID = " + CompID, null, 1, 0, false);
                ViewBag.Company = Companies.First();
                ViewBag.ProvincceID = Companies.First().CompProvinceID;
                if (product.ProvinceName == res.Product.lblBangkok) {
                    product.ProvinceName = res.Product.lblBkk;
                }

                ViewBag.Title = product.ProductName + " | " + product.CategoryName + " | " + product.ShortDescription.Replace('~', ' ').Substring(0, product.ShortDescription.Length - 1) + " | " + product.CompName + " | " + product.ProvinceName + " | " + res.Common.lblDomainShortName;

                var Shipments = svCompany.SelectData<b2bCompanyShipment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
                if (Shipments.Count > 0)
                    ViewBag.CompanyShipment = Shipments;

                var Payments = svCompany.SelectData<view_CompanyPayment>("*", " IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
                if (Payments.Count > 0)
                    ViewBag.CompanyPayments = Payments;

                var IsOnline = svCompany.SelectData<b2bLogOn>("*", "IsDelete = 0 AND CompID = " + CompID, null, 1, 0, false);
                if (IsOnline.Count > 0)
                    ViewBag.IsOnline = IsOnline.First().IsOnline;

                ViewBag.CompID = CompID;
                if (!string.IsNullOrEmpty(product.ProductKeyword))
                {
                    ViewBag.MetaKeyword = product.ProductName + " | " + product.ProductKeyword.Replace('~', ' ').Substring(0, product.ProductKeyword.Length - 1) + " | " + product.CategoryName  + " | " + "สินค้า, ซื้อ, รายละเอียด, ราคา"+ " | " + product.CompName + " | " + product.ProvinceName + " | B2BThai.com";
                }
                else {
                    ViewBag.MetaKeyword = ViewBag.Title;
                }
                if (!string.IsNullOrEmpty(product.ShortDescription))
                {
                    ViewBag.MetaDescription = product.ProductName + " | " + product.CategoryName + " | " + product.ProvinceName + " " + product.ShortDescription.Replace('~', ' ').Substring(0, product.ShortDescription.Length - 1) + " | " + product.CompName + " | " + product.ProvinceName + " | B2BThai.Com";
                }
                else
                {
                    ViewBag.MetaDescription = ViewBag.Title;
                }
                #endregion

                ViewBag.ProID = id;

                #region Send Message
                GetStatusUser();
                var user = (Ouikum.Company.UserStatusModel)ViewBag.UserStatus;
                if (user.CompID != 0)
                    ViewBag.CompanyView = svCompany.SelectData<b2bCompany>("CompID,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone", "CompID = " + user.CompID, null, 1, 0, false).First();

                #endregion

                var productToCompId = new List<view_SearchProduct>();
                var featureHot = new List<view_HotFeaProduct>();
                var PremiumProducts = new List<view_HotFeaProduct>();
                var SQLSelect_Feat = "";
                var SQLSelect_SearchProduct = "";

                SQLSelect_SearchProduct = " ProductID,ProductName,CompID,ProductImgPath,RowFlag,CompRowFlag,Price,ViewCount,MinOrderQty";
                var where = "AND CompID = " + product.CompID;
                productToCompId = svProduct.SelectHotProduct<view_SearchProduct>(SQLSelect_SearchProduct, "ProductID > 0 AND Rowflag in(2,4) AND CompRowFlag in(2,4) AND IsDelete = 0 AND IsJunk = 0" + where, "NEWID(),Price DESC", 1, 20);//""

                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,RowFlag,CompRowFlag,Price,Price_One,ViewCount,MinOrderQty,HotPrice";
                var num = productToCompId.Count;
                if (num < 6)
                {
                    var count = 20 - num;
                    featureHot = svProduct.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "HotFeaProductID > 0 AND Rowflag in(3) AND CompRowFlag in(2,4) AND IsDelete = 0 AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, count);//""
                }
                ViewBag.FeatProductsHot = featureHot;
                ViewBag.FeatProducts = productToCompId;
                if (MemoryCache.Default["LoadPremiumProducts"] != null)
                {
                    PremiumProducts = (List<view_HotFeaProduct>)MemoryCache.Default["LoadPremiumProducts"];
                    PremiumProducts = PremiumProducts.OrderBy(x => Guid.NewGuid()).ToList();
                    PremiumProducts = PremiumProducts.OrderByDescending(m => m.HotPrice).ToList();
                }
                else
                {
                    PremiumProducts = svProduct.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'P' AND ProductID > 0 AND ProRowFlag in(4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 9);//""
                    if (svProduct.TotalRow > 0)
                    {
                        MemoryCache.Default.Add("LoadPremiumProducts", PremiumProducts, DateTime.Now.AddMinutes(10));
                    }
                }

                ViewBag.PremiumProducts = PremiumProducts;
                ViewBag.ImageCount = PremiumProducts.Count;

                AddViewCount((int)id, "Product");

                //var productname = product.ProductName.Split(' ');
                //var ProductKeywordByName = svCompany.SelectDataProduct<b2bProduct>("ProductID,ProductName,ProductImgPath,CompID", " IsDelete = 0 AND CompID != 0 AND ProductID NOT IN(" + product.ProductID + ") AND ProductKeyword like N'%" + productname[0] + "%'", null, 1, 3);
                //if (ProductKeywordByName.Count > 0)
                //{
                //    if (ProductKeywordByName.Count == 3)
                //    {
                //        ViewBag.ProductKeyword = ProductKeywordByName;
                //    }
                //    else
                //    {
                //        var proCount = 3 - ProductKeywordByName.Count;
                //        var proID = product.ProductID + ",";
                //        foreach (var pro in (List<b2bProduct>)ProductKeywordByName)
                //        {
                //            proID += pro.ProductID + ",";
                //        }
                //        proID = proID.Substring(0, proID.Length - 1);
                //        var ProductKeywordByCate = svCompany.SelectDataProduct<b2bProduct>("ProductID,ProductName,ProductImgPath,CompID", " IsDelete = 0 AND CompID != 0 AND ProductID NOT IN(" + proID + ") AND CateLV1 =" + product.CateLV1, null, 1, proCount);
                //        ViewBag.ProductKeywordByCate = ProductKeywordByCate;
                //        ViewBag.ProductKeyword = ProductKeywordByName;
                //    }
                //}
                //else
                //{
                //    var ProductKeywordByCate = svCompany.SelectDataProduct<b2bProduct>("ProductID,ProductName,ProductImgPath,CompID", " IsDelete = 0 AND CompID != 0 AND ProductID NOT IN(" + product.ProductID + ") AND CateLV1 =" + product.CateLV1, null, 1, 3);
                //    ViewBag.ProductKeyword = ProductKeywordByCate;
                //}
            }
            else
            {
                return Redirect("~/Default/NotFound");
            }

            return View();
        }
        #endregion
    }
}
