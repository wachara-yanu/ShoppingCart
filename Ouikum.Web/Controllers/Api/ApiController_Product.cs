using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common; 
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Product;
using Ouikum.Category;
using System.Runtime.Caching;
using Ouikum.Company;
namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        //  
        #region category

        

        [HttpGet]
        public ActionResult menucategory()
        {
            var svCate = new CategoryService();
            var cate = new List<CateModel>();
             
            if (MemoryCache.Default["menucategory"] != null)
            {
                cate = (List<CateModel>)MemoryCache.Default["menucategory"];
            }
            else
            {
                var sqlWhere = " AND categorylevel = 1 and categoryid in (1189, 3252, 5635,4179,1,1925,1083,2598,156 ,4370," +
                "2064,3068,3156,7102,6336,747 ,3864,2180,4421,1795,6566,1416,3957,552 ,7248,3494,1615,4030,3689) ";

                var cate1 = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,CategoryLevel,ParentCategoryID ", " IsDelete = 0 " + sqlWhere);

                sqlWhere = "AND categorylevel = 2 and parentcategoryid in (1189, 3252, 5635,4179,1,1925,1083,2598,156 ,4370," +
                "2064,3068,3156,7102,6336,747 ,3864,2180,4421,1795,6566,1416,3957,552 ,7248,3494,1615,4030,3689) ";

                var cate2 = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,CategoryLevel,ParentCategoryID ", " IsDelete = 0  " + sqlWhere);
                
                #region set
                foreach (var item in cate1)
                {
                    #region set level1
                    var cc1 = new CateModel();

                    cc1.id = item.CategoryID;
                    cc1.name = item.CategoryName;
                    cc1.level = (int)item.CategoryLevel;
                    cc1.parentid = (int)item.ParentCategoryID;
                  //  cc1.parentpath = item.ParentCategoryPath;
                    cc1.subcategory = new List<CateModel>();
                    var subcate = new CateModel();
                    foreach (var item2 in cate2.Where(m => m.ParentCategoryID == item.CategoryID))
                    {
                        #region set level2
                        var cc2 = new CateModel();
                        cc2.id = item2.CategoryID;
                        cc2.name = item2.CategoryName;
                        cc2.level = (int)item2.CategoryLevel;
                        cc2.parentid = (int)item2.ParentCategoryID;
                       // cc2.parentpath = item2.ParentCategoryPath;
                        cc2.subcategory = new List<CateModel>();
                       
                        #endregion
                        cc1.subcategory.Add(cc2);
                    }

                    cate.Add(cc1);
                    #endregion


                #endregion
                }
                MemoryCache.Default.Add("menucategory", cate, DateTime.Now.AddDays(1));
            }

            return Json(new { category = cate }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult category(int type = 0)
        {
            var svCate = new CategoryService();
            var cate = new List<CateModel>();

            if (MemoryCache.Default["categoryapp-" + type] != null)
            {
                cate = (List<CateModel>)MemoryCache.Default["categoryapp-" + type];
            }
            else
            {
                var sqlwhere = " AND CategoryType IN (2,3)";
                if (type > 0) 
                    sqlwhere = "AND CategoryType = " + type; 

                var cate1 = svCate.SelectData<b2bCategory>(" * ", " IsDelete = 0 AND CategoryLevel = 1 "+sqlwhere);
                var cate2 = svCate.SelectData<b2bCategory>(" * ", " IsDelete = 0 AND CategoryLevel = 2 "+sqlwhere);
                var cate3 = svCate.SelectData<b2bCategory>(" * ", " IsDelete = 0 AND CategoryLevel = 3 "+sqlwhere);
                foreach (var item in cate1)
                {
                    #region set level1
                    var cc1 = new CateModel();

                    cc1.id = item.CategoryID;
                    cc1.name = item.CategoryName;
                    cc1.level = (int)item.CategoryLevel;
                    cc1.parentid = (int)item.ParentCategoryID;
                    cc1.parentpath = item.ParentCategoryPath;
                    cc1.subcategory = new List<CateModel>();
                    var subcate = new CateModel(); 
                    foreach (var item2 in cate2.Where(m => m.ParentCategoryID == item.CategoryID))
                    {
                        #region set level2
                        var cc2 = new CateModel();
                        cc2.id = item2.CategoryID;
                        cc2.name = item2.CategoryName;
                        cc2.level = (int)item2.CategoryLevel;
                        cc2.parentid = (int)item2.ParentCategoryID;
                        cc2.parentpath = item2.ParentCategoryPath;
                        cc2.subcategory = new List<CateModel>();
                        foreach (var item3 in cate3.Where(m => m.ParentCategoryID == item2.CategoryID))
                        {
                            #region set level3
                            var cc3 = new CateModel();
                            cc3.id = item3.CategoryID;
                            cc3.name = item3.CategoryName;
                            cc3.level = (int)item3.CategoryLevel;
                            cc3.parentid = (int)item3.ParentCategoryID;
                            cc3.parentpath = item3.ParentCategoryPath;
                            cc3.subcategory = new List<CateModel>();
                            cc2.subcategory.Add(cc3);
                            #endregion
                        }
                        #endregion
                        cc1.subcategory.Add(cc2);
                    }

                    cate.Add(cc1);
                    #endregion

                }
                MemoryCache.Default.Add("categoryapp-" + type, cate, DateTime.Now.AddDays(1));
            }

            return Json(new { category = cate }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region product list
        public List<ProductModel> SetMappingProduct(List<view_ProductMobileApp> products)
        {
            var svProduct = new ProductService();
            var product = new List<ProductModel>();
            List<int> productid = products.Select(m => m.ProductID).ToList();
            var whereGallery = "IsDelete = 0";
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
                item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription)) ? p.ShortDescription.Replace("~", " ") : "";
                item.gallery = new List<GalleryModel>();
                var primarygallery = new GalleryModel();
                primarygallery.imageid = 0;
                primarygallery.imagepath = p.ProductImgPath;
                item.gallery.Add(primarygallery);
                #endregion

                if (data != null && data.Count > 0)
                {
                    #region First Image
                    var g = new GalleryModel();
                    g.imageid = 0;
                    g.imagepath = p.ProductImgPath;
                    item.gallery.Add(g);
                    #endregion

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

            return product;
        }
        public ActionResult productlist(string search, int cateid=0, int catelv = 0,int pageindex = 1 ,int pagesize = 50,int compid = 0)
        {
            var svProduct = new ProductService();
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            sqlWhere += svProduct.CreateWhereByCategory(catelv, cateid);
            if (!string.IsNullOrEmpty(search))
            {
                sqlWhere += "AND ( ProductName LIKE N'%" + search + "%'  ) ";
            }

            var products = svProduct.SelectData<view_ProductMobileApp>(" * ", sqlWhere,"ModifiedDate DESC",pageindex,pagesize);
            var totalrow = svProduct.TotalRow;
            var totalpage = svProduct.TotalPage;
            var product = new List<ProductModel>();
             
                var whereGallery = "IsDelete = 0";
                #region Model Set
                if (svProduct.TotalRow > 0)
                {
                    List<int> productid = products.Select(m => m.ProductID).ToList();
                    whereGallery += " AND "+ CreateWhereIN(productid, "productid");

                    var data = svProduct.SelectData<b2bProductImage>(" * ",whereGallery);
                    if (compid > 0)
                    {
                        svProduct.TotalRow = 0;
                        var favorites = new List<b2bFavProduct>();
                        favorites = svProduct.SelectData<b2bFavProduct>(" * ", "IsDelete = 0 AND compid = " + compid); 

                            if (svProduct.TotalRow > 0)
                            {
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
                                    item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription)) ? p.ShortDescription.Replace("~", " ") : "";

                                    if (favorites.Where(m => m.ProductID == p.ProductID).Count() > 0) 
                                        item.isfav = true; 
                                    else 
                                        item.isfav = false; 

                                    #endregion

                                    #region First Image
                                    item.gallery = new List<GalleryModel>();
                                    var g = new GalleryModel();
                                    g.imageid = 0;
                                    g.imagepath = p.ProductImgPath;
                                    item.gallery.Add(g);
                                    #endregion

                                    #region gallery
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
                                    #endregion

                                    product.Add(item);
                                }
                            }
                    }
                    else
                    {
                     
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
                            item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription)) ? p.ShortDescription.Replace("~", " ") : "";
                            item.isfav = false;

                            #endregion

                            #region First Image
                            item.gallery = new List<GalleryModel>();
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
                }
                #endregion
            
            
           // var gallery = svProduct.SelectData<b2bProductImage>(" * ", sqlWhere);

                return Json(new { product = product, totalrow = totalrow, totalpage = totalpage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        

        #region follow list
        public ActionResult followlist(int pageindex = 0, int pagesize = 0,int compid = 0)
        {
            var svProduct = new ProductService();
            var product = new List<ProductModel>();
            var gallery = new List<b2bProductImage>();
            var svCompany = new CompanyService();
            var totalrow  = 0;
            var totalpage = 0;

            if (compid > 0)
            {
                var follow = svCompany.SelectData<b2bFollowCategory>(" * ", " IsDelete = 0 AND CompID = " + compid);
                //var cateid 
                var lstCateID = follow.Select(m=>m.CategoryID).ToList();

                if (svCompany.TotalRow > 0)
                {
                    #region MyRegion
                    var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                    sqlWhere += svProduct.CreateWhereByCategory(1, lstCateID); 
                    var products = svProduct.SelectData<view_ProductMobileApp>(" * ", sqlWhere, " newid() ", pageindex, pagesize);
                    totalrow = svProduct.TotalRow;
                    totalpage = svProduct.TotalPage;
                    var whereGallery = "IsDelete = 0";
                    #endregion

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
                            item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription)) ? p.ShortDescription.Replace("~", " ") : "";
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
                            product.Add(item);
                        }
                    }
                    #endregion
                }
            }

            // var gallery = svProduct.SelectData<b2bProductImage>(" * ", sqlWhere);

            return Json(new { product = product, totalrow = totalrow, totalpage = totalpage }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region featureproduct list
        public ActionResult featureproduct(string type = "F", int pagesize = 50)
        {
            var svProduct = new ProductService();
            var sqlWhere = "IsDelete = 0 AND ExpiredDate < GetDate() ";

            if (string.IsNullOrEmpty(type))
            {
                type = type.ToUpper();
                sqlWhere += "AND Status = N'" + type + "'";
            }
            else
                sqlWhere += "AND Status = N'F' ";

            var products = svProduct.SelectData<view_HotFeaProduct>(" * ", sqlWhere,"",1,pagesize);

            var totalrow = svProduct.TotalRow;
            var totalpage = svProduct.TotalPage;
            var product = new List<ProductModel>();
            var gallery = new List<b2bProductImage>();
            var whereGallery = "IsDelete = 0";
            #region product gallery
            if (svProduct.TotalRow > 0)
            {
                List<int> productid = products.Select(m => (int)m.ProductID).ToList();
                whereGallery += " AND " + CreateWhereIN(productid, "productid");

                var data = svProduct.SelectData<b2bProductImage>(" * ", whereGallery);
                foreach (var p in products)
                {
                    var item = new ProductModel();
                    #region set product model
                    item.productid = (int)p.ProductID;
                    item.productname = p.ProductName;
                    item.price = (decimal)p.Price;
                    item.promotionprice = (p.PromotionPrice > 0) ? (decimal)p.PromotionPrice : 0;
                    item.ispromotion = (p.IsPromotion != null) ? (bool)p.IsPromotion : false;
                    item.compname = p.CompName;
                    item.compid = (int)p.CompID;
                    item.complevel = 0;
                    item.createdate = (DateTime)p.CreatedDate;
                    item.modifieddate = (DateTime)p.ModifiedDate;
                    item.imagepath = p.ProductImgPath;
                    item.productdetail = p.ProductDetail;
                    item.shortdescription = (!string.IsNullOrEmpty(p.ShortDescription)) ? p.ShortDescription.Replace("~", " ") : "";
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
                    product.Add(item);
                }
            }
            #endregion

            return Json(new { product = product, totalrow = svProduct.TotalRow, totalpage = svProduct.TotalPage }, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}
