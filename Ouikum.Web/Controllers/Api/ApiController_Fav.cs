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
using Ouikum.Quotation;
using res = Prosoft.Resource.Web.Ouikum;
using System.Collections;
using Ouikum.Favorite;
using Ouikum.Product;
namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 

        #region myfav list
        public ActionResult myfav(int compid,int pageindex = 1 ,int pagesize = 50)
        {
            var svFav = new FavProductService();
            var svProduct = new ProductService();
            var fav = svFav.SelectData<view_FavProduct>(" * ", "isdelete = 0 AND compid = " + compid,"",pageindex,pagesize);
            var product = new List<ProductModel>();
            var gallery = new List<b2bProductImage>();
            var whereGallery = "IsDelete = 0";
            #region product gallery
            if (svFav.TotalRow > 0)
            {
                List<int> productid = fav.Select(m => (int)m.ProductID).ToList();
                whereGallery += " AND " + CreateWhereIN(productid, "productid");

                var data = svProduct.SelectData<b2bProductImage>(" * ", whereGallery);
                foreach (var p in fav)
                {
                    var item = new ProductModel();
                    #region set product model
                    item.productid = (int)p.ProductID;
                    item.productname = p.ProductName;
                    item.price = (decimal)p.Price;
                    //item.promotionprice = (p.PromotionPrice > 0) ? (decimal)p.PromotionPrice : 0;
                    //item.ispromotion = (p.IsPromotion != null) ? (bool)p.IsPromotion : false;
                    item.compname = p.CompName;
                    item.compid = (int)p.ProductCompID;
                    item.complevel = (int)p.CompLevel;
                    item.createdate = p.CreatedDate;
                    item.modifieddate = p.ModifiedDate;
                    item.imagepath = p.ProductImgPath;
                    item.productdetail = p.ProductDetail;
                    
                    item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription))?p.ShortDescription.Replace("~"," "):"";
                    item.isfav = true;
                    item.gallery = new List<GalleryModel>();
                    #endregion

                    #region First Image
                    var g = new GalleryModel();
                    g.imageid = 0;
                    g.imagepath = p.ProductImgPath;
                    item.gallery.Add(g);
                    #endregion

                    if (data != null && data.Count > 0)
                    {

                        foreach (var it in data.Where(m => m.ProductID == p.ProductID))
                        {
                            g = new GalleryModel();
                            g.imageid = it.ProductImageID;
                            g.imagepath = it.ImgPath;
                            item.gallery.Add(g);
                        }
                    }
                    product.Add(item);
                }
            }
            #endregion

            return Json(new { favorites = product, totalrow = svFav.TotalRow, totalpage = svFav.TotalPage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region myfav list
        public ActionResult addfav(int compid,int productid)
        {
            var svFav = new FavProductService();
            var fav = new b2bFavProduct(); 
            svFav.InsertFavProduct(productid, compid);

            return Json(new { status = svFav.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult delfav(int productid,int compid)
        {
            var svFav = new FavProductService();
            var fav = new b2bFavProduct();
            var lstProductID = new List<int>();
            if (productid > 0)
            {
                lstProductID.Add(productid);
                svFav.Delete(lstProductID,compid);
            }

            return Json(new { status = svFav.IsResult }, JsonRequestBehavior.AllowGet);
        }

    }
}
