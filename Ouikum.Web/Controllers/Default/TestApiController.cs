using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using PonyTaleMarket.Common;
using Prosoft.Service;
using PonyTaleMarket.Product;
using PonyTaleMarket.Category;
using res = Prosoft.Resource.Web.PonyTaleMarket;
using PonyTaleMarket.Buylead;
using PonyTaleMarket.Company;
using System.Runtime.Caching;
using PonyTaleMarket.Quotation;
using PonyTaleMarket.Article;
using PonyTaleMarket.Web.Models;
using PonyTaleMarket.Message;
using AutoMapper;

namespace PonyTaleMarket.Web.Controllers
{
    public class TestApiController : BaseController
    {

        #region DoLoad HomePage

        #region CachingRecentSupplier
        public void CachingRecentSupplier(string sqlWhereRecently)
        {

            var svCompany = new CompanyService();
            /*Hot Success Story*/

            var name = "recentcomp";
            var data = new List<view_CompMember>();
            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_CompMember>)MemoryCache.Default[name];
            }
            else
            {
                data = svCompany.SelectData<view_CompMember>("CompID,CompName,ProvinceName,CreatedDate,CompLevel", sqlWhereRecently, "CreatedDate DESC", 1, 10);

                if (data != null && data.Count > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                }
            }
            ViewBag.NewSuppliers = data;

        }

        #endregion

        #region CachingRecentProduct
        public void CachingRecentProduct(string sqlWhereRecently)
        {

            var svProduct = new ProductService();
            /*Hot Success Story*/

            var name = "recentproduct";
            List<view_SearchProduct> data;
            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_SearchProduct>)MemoryCache.Default[name];
            }
            else
            {
                data = svProduct.SelectData<view_SearchProduct>("ProductID,CompID,ProductImgPath,ProductName,CreatedDate,Modifieddate,ProvinceName,Price,Ispromotion,PromotionPrice", sqlWhereRecently, "CreatedDate DESC", 1, 10);

                if (data != null && data.Count > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                }
            }
            ViewBag.NewProducts = data;

        }

        #endregion

        #region LoadRecently
        public void LoadRecently()
        {
            var svProduct = new ProductService();
            var svCompany = new CompanyService();

            #region Load NewLy
            var sqlWhere = "IsDelete = 0 AND (( CompRowFlag IN (2,4) ) AND ProductCount > 0 ) ";
            sqlWhere += svCompany.CreateWhereServiceType(3);
            sqlWhere += svCompany.CreateWhereCause(0, "", "", 3);

            CachingRecentSupplier(sqlWhere);

            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            sqlWhere += svProduct.CreateWhereCause(0, "", 4);

            CachingRecentProduct(sqlWhere);

            #endregion
        }
        #endregion

        #region LoadFeatureProduct
        public void LoadFeatureProduct()
        {
            #region Load Feature
            var feature = new List<view_HotFeaProduct>();
            if (MemoryCache.Default["LoadFeature"] != null)
            {
                feature = (List<view_HotFeaProduct>)MemoryCache.Default["LoadFeature"];
                feature = feature.OrderBy(x => Guid.NewGuid()).ToList();
            }
            else
            {
                var svHotFeat = new HotFeaProductService();
                feature = svHotFeat.SelectData<view_HotFeaProduct>(" ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice", "Rowflag = 3 AND Status = 'F' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4)", " NEWID()", 1, 20);
                if (svHotFeat.TotalRow > 0)
                {
                    MemoryCache.Default.Add("LoadFeature", feature, DateTime.Now.AddMinutes(10));
                }
            }
            ViewBag.FeatProducts = feature;
            #endregion
        }
        #endregion

        #region Load Random Company
        public void LoadRandomCompany()
        {

            var svCompany = new CompanyService();
            var sqlWhere = "IsDelete = 0 AND (( CompRowFlag IN (2,4) ) AND ProductCount > 0 ) AND LogoImgPath != '' AND CompLevel = 3 AND CompImgPath != ''";

            var data = new List<view_CompMember>();
            var comp = new view_CompMember();
            if (MemoryCache.Default["CompMember"] != null)
            {
                data = (List<view_CompMember>)MemoryCache.Default["CompMember"];
                var rand = new Random();
                comp = data[rand.Next(data.Count)];
            }
            else
            {
                data = svCompany.SelectData<view_CompMember>(
                     @" CompID,CompName,LogoImgPath,CompLevel,CompPhone,ProductCount,
                ContactFirstName,ContactLastName,BizTypeID,BizTypeName,BizTypeOther,ProvinceName",
                     sqlWhere,
                     "  NEWID()", 1, 10);

                if (data != null && svCompany.TotalRow > 0)
                {
                    MemoryCache.Default.Add("CompMember", data, DateTime.Now.AddHours(4));
                    var rand = new Random();
                    comp = data[rand.Next(data.Count)];
                };
            }

            ViewBag.Company = comp;


        }
        #endregion

        #region Load B2BToday
        public ActionResult B2BToday()
        {
            var svProduct = new ProductService();
            var svBuylead = new BuyleadService();
            var svCompany = new CompanyService();

            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);

            string CountProduct = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");

            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            sqlWhere += " And CateLV1=7102";
            string Countsoftware = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");

            sqlWhere = svCompany.CreateWhereAction(CompStatus.HaveProduct);
            sqlWhere += svCompany.CreateWhereServiceType(3);
            string CountSupplier = svCompany.CountData<b2bCompany>("CompID", sqlWhere).ToString("#,##0");

            return Json(new { ProductAll = CountProduct, SupplierAll = CountSupplier, SoftwareAll = Countsoftware });
        }
        #endregion
        #endregion

        public ActionResult defaut()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            LoadRecently();
            LoadCategory();
            LoadFeatureProduct();
            LoadRandomCompany();

            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }




        #region LoadBidProduct
        public void LoadDataBidProduct()
        {
            string SQLWhere = "";
            var svQuotation = new QuotationService();
            var quotations = new List<view_QuotationPublic>();
            if (MemoryCache.Default["loadbidproduct"] != null)
            {
                quotations = (List<view_QuotationPublic>)MemoryCache.Default["loadbidproduct"];
            }
            else
            {
                SQLWhere = svQuotation.CreateWhereAction(QuotationAction.FrontEnd, 0);
                quotations = svQuotation.SelectData<view_QuotationPublic>("*", SQLWhere, "ModifiedDate DESC", 1, 10);

                if (quotations != null && svQuotation.TotalRow > 0)
                {
                    MemoryCache.Default.Add("loadbidproduct", quotations, DateTime.Now.AddMinutes(5));
                };
            }

            ViewBag.Quotation = quotations;


        }
        [HttpPost]
        public ActionResult LoadBidProduct()
        {
            LoadDataBidProduct();
            return PartialView("UC/BidProductListUC");
        }
        #endregion

        #region LoadSuccessStory
        public void LoadSuccessStory()
        {
            var svArticle = new ArticleService();
            /*Hot Success Story*/

            var name = "successstory";
            var data = new List<view_b2bArticle>();

            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_b2bArticle>)MemoryCache.Default[name];
                var rand = new Random();
                ViewBag.HotStory = data[rand.Next(data.Count)];
            }
            else
            {
                data = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7", "  NEWID()", 1, 10);
                if (data != null && svArticle.TotalRow > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                    ViewBag.HotStory = data.First();
                }

            }
        }
        #endregion

        #region LoadProductShowcase
        #region CachingNewProductArrival
        public List<view_SearchProduct> CachingNewProductArrival(string sqlSelect, string sqlWhere, int ArrivalNo, int Cate)
        {
            var svProduct = new ProductService();
            var data = new List<view_SearchProduct>();
            var name = "showcaseno" + ArrivalNo + "cate" + Cate;

            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_SearchProduct>)MemoryCache.Default[name];
            }
            else
            {
                data = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, " NEWID()", 1, 4);
                if (data != null && data.Count > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                }
            }
            return data;
        }
        #endregion

        [HttpPost]
        public ActionResult LoadProductShowcase()
        {
            var svProduct = new ProductService();
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            var sqlSelect = " ProductID,ProductName,CompID,CompName,CompLevel,ProductImgPath,ShortDescription,ProvinceName,Price,Ispromotion,PromotionPrice,QtyUnit";
            var cate1 = 0;
            var cate2 = 0;
            var cate3 = 0;
            var cate4 = 0;


            cate1 = 3068; cate2 = 2598; cate3 = 3494; cate4 = 1189;
            sqlWhere += " And CateLV1 = ";


            var NewArrival1 = CachingNewProductArrival(sqlSelect, sqlWhere + cate1, 1, cate1);
            ViewBag.NewArrival1 = NewArrival1;

            var NewArrival2 = CachingNewProductArrival(sqlSelect, sqlWhere + cate2, 2, cate2);
            ViewBag.NewArrival2 = NewArrival2;


            var NewArrival3 = CachingNewProductArrival(sqlSelect, sqlWhere + cate3, 3, cate3);
            ViewBag.NewArrival3 = NewArrival3;


            var NewArrival4 = CachingNewProductArrival(sqlSelect, sqlWhere + cate4, 4, cate4);
            ViewBag.NewArrival4 = NewArrival4;

            return PartialView("UC/CategoryShowCase");
        }


        [HttpPost]
        public ActionResult BindLoadProductShowcase()
        {
            var svProduct = new ProductService();
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            var sqlSelect = " ProductID,ProductName,CompID,CompName,CompLevel,ProductImgPath,ShortDescription,ProvinceName,Price,Ispromotion,PromotionPrice,QtyUnit";
            var cate1 = 0;
            var cate2 = 0;
            var cate3 = 0;
            var cate4 = 0;
            switch (res.Common.lblWebsite)
            {
                case "B2BThai": cate1 = 3068; cate2 = 2598; cate3 = 3494; cate4 = 1189; sqlWhere += " And CateLV1="; break;
                case "AntCart": cate1 = 6566; cate2 = 6277; cate3 = 6103; cate4 = 5888; sqlWhere += " And CateLV1="; break;
                case "myOtopThai": cate1 = 7377; cate2 = 7341; cate3 = 7508; cate4 = 7426; sqlWhere += " And CateLV2="; break;
                case "AppstoreThai": cate1 = 7611; cate2 = 7571; cate3 = 7596; cate4 = 7175; sqlWhere += " And CateLV2="; break;
                case "Myanmar": cate1 = 3068; cate2 = 2598; cate3 = 3494; cate4 = 1189; sqlWhere += " And CateLV1="; break;
                default: break;
            }
            var NewArrival1 = CachingNewProductArrival(sqlSelect, sqlWhere + cate1, 1, cate1);
            var NewArrival2 = CachingNewProductArrival(sqlSelect, sqlWhere + cate2, 2, cate2);
            var NewArrival3 = CachingNewProductArrival(sqlSelect, sqlWhere + cate3, 3, cate3);
            var NewArrival4 = CachingNewProductArrival(sqlSelect, sqlWhere + cate4, 4, cate4);

            return Json(new
            {
                Arrival1 = NewArrival1,
                Arrival2 = NewArrival2,
                Arrival3 = NewArrival3,
                Arrival4 = NewArrival4
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region LoadHotProduct
        public void LoadHotProduct()
        {
            var svHotFeat = new HotFeaProductService();
            var name = "hotfeat";

            List<view_HotFeaProduct> data;

            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_HotFeaProduct>)MemoryCache.Default[name];
            }
            else
            {
                data = svHotFeat.SelectData<view_HotFeaProduct>(" ProductID,ProductName,CompID,ProductImgPath", "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND IsDelete=0", "  CreatedDate DESC ", 1, 20);

                if (data != null && data.Count > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                }
            }
            ViewBag.HotProducts = data;
            ViewBag.HotProductsCount = svHotFeat.TotalRow;
        }
        #endregion
        //AppstoreThai
        #region LoadCateLV2

        [HttpPost]
        public ActionResult LoadCateLV2(List<int> cateid)
        {
            var svCategory = new CategoryService();
            var cates = new List<b2bCategory>();
            if (MemoryCache.Default["whereInCateLV2"] != null)
            {
                var sqlWhere = CreateWhereIN(cateid, "ParentCategoryID");
                var where = (string)MemoryCache.Default["whereInCateLV2"];
                if (sqlWhere == where)
                {
                    cates = (List<b2bCategory>)MemoryCache.Default["CateLV2"];
                }
                else
                {
                    cates = svCategory.SelectData<b2bCategory>("CategoryID,CategoryName,CategoryLevel,ParentCategoryID",
                        " IsDelete = 0 AND " + sqlWhere);

                    if (cates != null && svCategory.TotalRow > 0)
                    {
                        #region Set Model
                        var model = new List<CateModel>();
                        foreach (var item in cateid)
                        {
                            foreach (var sub in cates.Where(m => m.ParentCategoryID == item).OrderBy(m => m.CategoryName).Take(15))
                            {
                                var m = new CateModel();
                                m.id = sub.CategoryID;
                                m.level = (int)sub.CategoryLevel;
                                m.name = sub.CategoryName;
                                m.parentid = (int)sub.ParentCategoryID;
                                m.subcategory = new List<CateModel>();
                                model.Add(m);
                            }
                        }
                        #endregion

                        MemoryCache.Default.Add("CateLV2", model, DateTime.Now.AddHours(4));
                        MemoryCache.Default.Add("whereInCateLV2", where, DateTime.Now.AddHours(4));
                    }
                }
            }
            else
            {
                var sqlWhere = CreateWhereIN(cateid, "ParentCategoryID");
                cates = svCategory.SelectData<b2bCategory>("CategoryID,CategoryName,CategoryLevel,ParentCategoryID",
                    " IsDelete = 0 AND " + sqlWhere);
                if (cates != null && svCategory.TotalRow > 0)
                {
                    #region Set Model
                    var model = new List<CateModel>();
                    foreach (var item in cateid)
                    {
                        foreach (var sub in cates.Where(m => m.ParentCategoryID == item).OrderBy(m => m.CategoryName).Take(15))
                        {
                            var m = new CateModel();
                            m.id = sub.CategoryID;
                            m.level = (int)sub.CategoryLevel;
                            m.name = sub.CategoryName;
                            m.parentid = (int)sub.ParentCategoryID;
                            m.subcategory = new List<CateModel>();
                            model.Add(m);
                        }
                    }
                    #endregion

                    MemoryCache.Default.Add("CateLV2", model, DateTime.Now.AddHours(4));
                    MemoryCache.Default.Add("whereInCateLV2", sqlWhere, DateTime.Now.AddHours(4));
                }
            }

            return Json(new { CateLV = cates });
        }



        #endregion
        #region LoadSoftwareCategory
        [HttpPost]
        public ActionResult LoadSoftwareCategory(int CateID, int Type)
        {
            var svCategory = new CategoryService();
            var cates = new List<b2bCategory>();
            var name = "SubCategory-" + CateID + "-" + Type;
            if (MemoryCache.Default[name] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default[name];
            }
            else
            {
                #region Load Category
                if (Type == 1)
                {
                    cates = svCategory.ListWholesaleConsumerCategoryLv2(CateID, 15);
                }
                if (Type == 2)
                {
                    cates = svCategory.LoadSubCategory(CateID, 10, 3);
                }
                if (Type == 3)
                {
                    cates = svCategory.LoadSubCategory(CateID, 20, 2);
                }
                #endregion

                if (cates != null && svCategory.TotalRow > 0)
                {
                    MemoryCache.Default.Add(name, cates, DateTime.Now.AddDays(4));
                }
            }

            ViewBag.CateLV3 = cates;

            return Json(new { CateLV3 = cates });
        }
        #endregion

        #region GetCategoryFooter
        public void GetCategoryFooter()
        {
            var cates = new List<b2bCategory>();
            var svCategory = new CategoryService();
            cates = svCategory.GetCategoryFooter();

            ViewBag.CategoryFooter = cates;
        }

        #endregion

        [HttpPost]
        public ActionResult GetCateFooter()
        {
            var cates = new List<b2bCategory>();
            var svCategory = new CategoryService();
            cates = svCategory.GetCategoryFooter();
            return Json(new { cates = cates }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            LoadProvinces();
            LoadBiztype();
            LoadCategory();
            return View();
        }

        public ActionResult testmongo()
        { 
            var mg = new KeywordMongo();


            return Json(new
            {
                result = true// mg.Result.Where(m=>m.IsResult == true && m.Comment == "save").ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveProductMongo(int id)
        {
            var mg = new KeywordMongo();
            var result = mg.UpdateMongoProduct(id);

            return Json(new
            {
                result = result// mg.Result.Where(m=>m.IsResult == true && m.Comment == "save").ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateProductMongo(int startId, int endId)
        {
            var mg = new KeywordMongo();
            var result = mg.UpdateMongoProduct(startId,endId);  

            return Json(new
            {
                result = result// mg.Result.Where(m=>m.IsResult == true && m.Comment == "save").ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestSpeed()
        {
            GetStatusUser();
            LoadProductShowcase();
            LoadHotProduct();
            LoadRecently();
            LoadCategory();
            LoadFeatureProduct();
            LoadRandomCompany();
            GetStatusUser();
            LoadDataBidProduct();
            LoadSuccessStory();
            GetEnumServiceType();
            //search
            LoadProvinces();
            LoadBiztype();
            #region CheckIsLogin
            if (CheckIsLogin())
            {
                MessageService svMessage = new MessageService();
                QuotationService svQuotation = new QuotationService();
                string SQLWhere = "";

                string sqlwhere = svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID);
                int count = svMessage.CountData<emMessage>("MessageID", sqlwhere);
                ViewBag.UnRead = count;

                SQLWhere = svQuotation.CreateWhereAction(QuotationAction.CountQuotation, LogonCompID);
                ViewBag.CountQuo = svQuotation.SelectData<b2bQuotation>("QuotationID", SQLWhere, "QuotationID", 1, 0).Count;
            }
            #endregion

            ViewBag.FBAccessToken = GetAppSetting("FBPageAccessToken");
            ViewBag.Page = "Home";
           
            return View();
        }

        public ActionResult BlobDownLoads(string dir, string name)
        {
            dir = dir.Replace("-", "/");
            BlobStorageService svBlob = new BlobStorageService();
            var container = svBlob.GetCloudBlobContainer();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name); // force download

            container.GetBlockBlobReference(dir + "/" + name).DownloadToStream(Response.OutputStream);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetFBGroup()
        {
            var fbHelper = new FacebookHelper();
            var data = fbHelper.FeedB2BThaiGroup();

            return Json(new
            {
                FeedGroup = data
            }, JsonRequestBehavior.AllowGet);
        }


        #region ListBlob
        public ActionResult testCopy()
        {

            BlobStorageService svBlob = new BlobStorageService();
             svBlob.CopyBlob("Manual/mail-month.txt", "Manual/mail-month-copy.txt");
            return Json(new {IsSuccess   = svBlob.IsSuccess }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBlob()
        {

            BlobStorageService svBlob = new BlobStorageService();
            var get = svBlob.GetBlobFile("Manual/mail-month.txt");
            return Json(new { url = get }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListBlob()
        { 
            BlobStorageService svBlob = new BlobStorageService();
            var blob = svBlob.ListBlobFile("Manual");
            var IsResult = false; 
            if (blob != null && blob.Count > 0)
            {
                IsResult = true;
            }
            return Json(new { success = IsResult, blob = blob }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CheckExist
        public ActionResult CheckExist()
        { 
            BlobStorageService svBlob = new BlobStorageService();
            var exist = svBlob.Exists("Manual/mail-month.txt");
            var notexist = svBlob.Exists("Manual/mail-month2.txt");
            return Json(new { exist = exist, notexist = notexist }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult TestUrlAuthorize()
        {
            CheckAuthorizeDomain();
            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestErrorPage()
        {
            var isresult = false;
            try
            {
                int a = Int16.Parse("aaa");
                isresult = true;
            }
            catch (Exception ex)
            {
                isresult = false;
                CreateLogFiles(ex);
            }
            return Json(isresult, JsonRequestBehavior.AllowGet);
        }

        #region Mongo

        public ActionResult GetProductMongo(int startId, int endId)
        {
            var mgKeyword = new KeywordMongo();
            var sv = new ProductService();
            var sql = @" Select * from view_ProductMobileApp
       where (IsDelete = 0 AND RowFlag IN (4,5,6) AND CompRowFlag IN (2,4)
      AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0 ) AND ( productid >= " + startId + " AND productid <= " + endId + " )";
            var data = new List<view_ProductMobileApp>();
            data = sv.qDB.ExecuteQuery<view_ProductMobileApp>(sql).ToList();
            var keywords = mgKeyword.SaveKeywordMongo(data);

            return Json(new
            {
                keywords = keywords
            }, JsonRequestBehavior.AllowGet);
        } 

        [HttpPost]
        public ActionResult SearchProduct(    
            int? Sort = 1,
            int PIndex = 1, int PSize = 20,
            string searchKey = "", int? BizTypeID = 0,
            int CateLevel = 0, int CategoryID = 0,
            int? Complevel = 0, int ProvinceID = 0)
        {
            var mgKeyword = new KeywordMongo();
            var keywords = mgKeyword.SearchProductMongo(Sort, PIndex, PSize, searchKey, BizTypeID, CateLevel, CategoryID, Complevel, ProvinceID);
            var products = new List<view_ProductMobileApp>();
            foreach (var item in keywords)
            {
                Mapper.CreateMap<Models.tbProduct, view_ProductMobileApp>();
                view_ProductMobileApp model = Mapper.Map<Models.tbProduct, view_ProductMobileApp>(item);
                products.Add(model);
            }

            return Json(new { totalrow = mgKeyword.TotalRow, products = products }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductById(int ProductID)
        {
            var mgKeyword = new KeywordMongo();
            var product = mgKeyword.GetProductMongoById(ProductID);

            return Json(new { product = product }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveProductKeywordMongo(int ProductID)
        {
            var mgKeyword = new KeywordMongo();
            var result = false; 
            try
            {
                result = mgKeyword.RemoveProductKeywordMongo(ProductID);

            }
            catch (Exception ex)
            {
                result = false;
                throw;
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult productviewcount(int ProductID)
        {
            var isresult = false;
            try
            {
                var mg = new KeywordMongo();
                isresult = mg.UpdateProductViewCount(ProductID);
            }
            catch (Exception ex)
            {
                isresult = false;
                CreateLogFiles(ex);
            }
            return Json(isresult, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ReplaceSearchKey(string search)
        { 
                var mg = new KeywordMongo();
             
            return Json( mg.ReplaceSearchKey(search), JsonRequestBehavior.AllowGet);
        }
         
        //public ActionResult UpdateEmployee(string name)
        //{ 
        //        var mg = new EmployeeMongo();
                
             
        //    return Json( mg.UpdateEmployee(name), JsonRequestBehavior.AllowGet);
        //} 

        [HttpPost]
        public ActionResult DecodPass(string pass)
        {
            var encryp = new EncryptManager();
            pass = encryp.DecryptData(pass);
            return Json(pass, JsonRequestBehavior.AllowGet);
        }


        
    }
}
