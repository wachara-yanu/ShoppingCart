using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Category;
using Ouikum.Product;
using Ouikum.Buylead; 
using Ouikum.Company;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Article;
using Ouikum.OrderPuchase;
//using Prosoft.Base;
using res = Prosoft.Resource.Web.Ouikum;
using System.Runtime.Caching;
using Ouikum.Web.Models;

namespace Ouikum.Web.Controllers
{
    public partial class HomeController : BaseController
    {
        public ActionResult Index2()
        {
            var p = new ProductService();
            var sc = new CategoryService(p.qDB);

            return Json(new { isres = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            //if (RedirectToProduction())
            //    return Redirect(UrlProduction);

            LoadProductShowcase();
            LoadHotProduct();
            LoadRecently();
            LoadCategory();
            LoadFeatureProduct();
            LoadRandomCompany();
            LoadRandomBanner();
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
                OrderPurchaseService svOrderPur = new OrderPurchaseService();
                string SQLWhere = "";

                string sqlwhere = svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID);
                int count = svMessage.CountData<emMessage>("MessageID", sqlwhere);
                ViewBag.UnRead = count;

                SQLWhere = svQuotation.CreateWhereAction(QuotationAction.CountQuotation, LogonCompID);
                ViewBag.CountQuo = svQuotation.SelectData<b2bQuotation>("QuotationID", SQLWhere, "QuotationID", 1, 0).Count;

                SQLWhere = svOrderPur.CreateWhereAction(OrderPurchaseAction.CountOrderPurchase, LogonCompID);
                ViewBag.CountOrP = svOrderPur.SelectData<OuikumOrderDetail>("OrDetailID", SQLWhere, "OrDetailID", 1, 0).Count;
            }
            #endregion

            ViewBag.FBAccessToken = GetAppSetting("FBPageAccessToken");
            ViewBag.Page = "Home";

            return View();
        }

        #region DoLoad HomePage

        #region CachingRecentSupplier
        public void CachingRecentSupplier(string sqlWhereRecently)
        {

            var svCompany = new CompanyService();
            /*Hot Success Story*/

            var name = "recentcomp";
            var data = new List<view_CompMember> ();
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
                //data = (List<view_SearchProduct>)MemoryCache.Default[name + Base.AppLang];
            }
            else
            {
                var SQLSelect_list = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_list = "ProductID,CompID,ProductImgPath,ProductNameEng AS ProductName,CreatedDate,Modifieddate,ProvinceName,Price,Ispromotion,PromotionPrice";
                //else

                SQLSelect_list = "SELECT Top 10 dbo.b2bProduct.ProductID,dbo.b2bCompany.CompID,dbo.b2bProduct.ProductImgPath,dbo.b2bProduct.ProductName,";
                SQLSelect_list += " dbo.b2bProduct.CreatedDate,dbo.b2bProduct.Modifieddate,dbo.b2bProduct.Price,dbo.b2bProduct.Ispromotion,dbo.b2bProduct.PromotionPrice,dbo.b2bProduct.Price_One,dbo.b2bProduct.RowFlag";
                SQLSelect_list += " FROM dbo.b2bProduct LEFT OUTER JOIN dbo.b2bCompany ON dbo.b2bCompany.CompID = dbo.b2bProduct.CompID ";
                SQLSelect_list += " WHERE ( dbo.b2bProduct.IsDelete = 0 AND dbo.b2bProduct.RowFlag IN (4,5,6) AND dbo.b2bCompany.RowFlag IN (2,4) AND dbo.b2bCompany.IsDelete = 0) ";
                SQLSelect_list += " AND ( dbo.b2bProduct.IsShow = 1 AND dbo.b2bProduct.IsJunk = 0 ) And dbo.b2bProduct.RowFlag = 4 ";
                SQLSelect_list += " ORDER BY dbo.b2bProduct.CreatedDate DESC";
                //data = svProduct.SelectDataProduct<view_SearchProduct>(SQLSelect_list, sqlWhereRecently, "CreatedDate DESC", 1, 10);

                data = svProduct.SelectDataCachingRecentProduct<view_SearchProduct>(SQLSelect_list);
                
                //data = data.OrderBy(m => m.CreatedDate).ToList();

                if (data != null && data.Count > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
                    //MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddHours(4));
                }


                //var i = 0;
                //var listID = new List<long>();
                //do
                //{
                //    var sql = "select ....";
                //    var ProductID = 100;
                //    listID.Add(ProductID);
                //    i++;

                //} while (listID.Count() < 10);

                

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
                feature = feature.OrderByDescending(m => m.Price).ToList();
            }
            else
            {
                var svHotFeat = new HotFeaProductService();

                var SQLSelect_Feat = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_Feat = "ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice";
                //else

                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
                feature = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'F' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 20);//""
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

        #region Load Random Banner
        public void LoadRandomBanner()
        {
            var svArticle = new ArticleService();
            var sqlWhere = "IsDelete = 0 AND IsShow = 1 AND WebID = 1";

            var data = new List<b2bBanner>();
            var Banner = new b2bBanner();
            if (MemoryCache.Default["BannerHome"] != null)
            {
                data = (List<b2bBanner>)MemoryCache.Default["BannerHome"];
                var rand = new Random();
                Banner = data[rand.Next(data.Count)];
            }
            else
            {
                data = svArticle.SelectData<b2bBanner>("*", sqlWhere, "ListNo ASC", 1, 10);

                if (data != null && svArticle.TotalRow > 0)
                {
                    MemoryCache.Default.Add("BannerHome", data, DateTime.Now.AddMinutes(10));
                    var rand = new Random();
                    Banner = data[rand.Next(data.Count)];
                };
            }

            ViewBag.Banner = data;


        }
        #endregion

        #region Load B2BToday
        public ActionResult B2BToday()
        {
            var svProduct = new ProductService();
            var svBuylead = new BuyleadService();
            var svCompany = new CompanyService();

            string CountProduct, Countsoftware, CountCentral, CountWest, CountNorth, CountNortheast, CountSouth, CountEast, CountSupplier = "";

            var HomeCountProduct = "HomeCountProduct";
            var HomeCountsoftware = "HomeCountsoftware";
            var HomeCountCentral = "HomeCountCentral";
            var HomeCountWest = "HomeCountWest";
            var HomeCountNorth = "HomeCountNorth";
            var HomeCountNortheast = "HomeCountNortheast";
            var HomeCountSouth = "HomeCountSouth";
            var HomeCountEast = "HomeCountEast";
            var HomeCountSupplier = "HomeCountSupplier";

            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);

            if (MemoryCache.Default[HomeCountProduct] != null)
            {
                CountProduct = (string)MemoryCache.Default[HomeCountProduct];
            }
            else
            {
                CountProduct = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountProduct != "" || CountProduct != null)
                {
                    MemoryCache.Default.Add(HomeCountProduct, CountProduct, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountsoftware] != null)
            {
                Countsoftware = (string)MemoryCache.Default[HomeCountsoftware];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And CateLV1=7102";
                Countsoftware = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (Countsoftware != "" || Countsoftware != null)
                {
                    MemoryCache.Default.Add(HomeCountsoftware, Countsoftware, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountCentral] != null)
            {
                CountCentral = (string)MemoryCache.Default[HomeCountCentral];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=1";
                CountCentral = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountCentral != "" || CountCentral != null)
                {
                    MemoryCache.Default.Add(HomeCountCentral, CountCentral, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountWest] != null)
            {
                CountWest = (string)MemoryCache.Default[HomeCountWest];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=2";
                CountWest = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountWest != "" || CountWest != null)
                {
                    MemoryCache.Default.Add(HomeCountWest, CountWest, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountNorth] != null)
            {
                CountNorth = (string)MemoryCache.Default[HomeCountNorth];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=3";
                CountNorth = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountNorth != "" || CountNorth != null)
                {
                    MemoryCache.Default.Add(HomeCountNorth, CountNorth, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountNortheast] != null)
            {
                CountNortheast = (string)MemoryCache.Default[HomeCountNortheast];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=4";
                CountNortheast = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountNortheast != "" || CountNortheast != null)
                {
                    MemoryCache.Default.Add(HomeCountNortheast, CountNortheast, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountSouth] != null)
            {
                CountSouth = (string)MemoryCache.Default[HomeCountSouth];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=5";
                CountSouth = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountSouth != "" || CountSouth != null)
                {
                    MemoryCache.Default.Add(HomeCountSouth, CountSouth, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountEast] != null)
            {
                CountEast = (string)MemoryCache.Default[HomeCountEast];
            }
            else
            {
                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
                sqlWhere += " And RegionID=6";
                CountEast = svProduct.CountData<view_SearchProduct>("ProductID", sqlWhere).ToString("#,##0");
                if (CountEast != "" || CountEast != null)
                {
                    MemoryCache.Default.Add(HomeCountEast, CountEast, DateTime.Now.AddHours(2));
                }
            }

            if (MemoryCache.Default[HomeCountSupplier] != null)
            {
                CountSupplier = (string)MemoryCache.Default[HomeCountSupplier];
            }
            else
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.HaveProduct);
                sqlWhere += svCompany.CreateWhereServiceType(3);
                CountSupplier = svCompany.CountData<b2bCompany>("CompID", sqlWhere).ToString("#,##0");
                if (CountSupplier != "" || CountSupplier != null)
                {
                    MemoryCache.Default.Add(HomeCountSupplier, CountSupplier, DateTime.Now.AddHours(2));
                }
            }
            
            return Json(new { ProductAll = CountProduct, SupplierAll = CountSupplier, SoftwareAll = Countsoftware, Central = CountCentral, West = CountWest, North = CountNorth, Northeast = CountNortheast, South = CountSouth, East = CountEast });
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
            LoadRandomBanner();

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
                quotations = svQuotation.SelectData<view_QuotationPublic>("*", SQLWhere, "CreatedDate DESC", 1, 10);

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
            //var svArticle = new ArticleService();
            ///*Hot Success Story*/
          
            //var name = "successstory";
            //var data = new List<view_b2bArticle>();

            //if (MemoryCache.Default[name] != null)
            //{
            //    data = (List<view_b2bArticle>)MemoryCache.Default[name];
            //    var rand = new Random();
            //    ViewBag.HotStory  = data[rand.Next(data.Count)];
            //}
            //else
            //{
            //    data = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 7", "  NEWID()", 1, 10);
            //    if (data != null && svArticle.TotalRow > 0)
            //    {
            //        MemoryCache.Default.Add(name, data, DateTime.Now.AddHours(4));
            //        ViewBag.HotStory = data.First();
            //    }

            //}

            var svArticle = new ArticleService();
            /*Hot Success Story*/

            var name = "Article by Customer";
            var data = new List<view_b2bArticle>();

            if (MemoryCache.Default[name] != null)
            {
                data = (List<view_b2bArticle>)MemoryCache.Default[name];
                var rand = new Random();
                ViewBag.HotStory = data[rand.Next(data.Count)];
            }
            else
            {
                data = svArticle.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleTypeID = 5 and CompID != 0", "  NEWID()", 1, 10);
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
        public List<view_SearchProduct> CachingNewProductArrival(string sqlSelect,string sqlWhere,int ArrivalNo,int Cate)
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
                data = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, " NEWID()", 1, 6);
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
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.Home);
            var sqlSelect = " ProductID,ProductName,Price_One,CompID,CompName,CompLevel,ProductImgPath,ShortDescription,ProvinceName,Price,Ispromotion,PromotionPrice,QtyUnit";
            var cate1 = 0;
            var cate2 = 0;
            var cate3 = 0;
            var cate4 = 0;
       

            //3068
            cate1 = 1; cate2 = 7; cate3 = 6; cate4 = 10; 
            sqlWhere += " And CateLV1 = ";

            var NewArrival1 = CachingNewProductArrival(sqlSelect, sqlWhere + cate1, 1, cate1);
            ViewBag.NewArrival1 = NewArrival1;

            var NewArrival2 = CachingNewProductArrival(sqlSelect, sqlWhere + cate2, 7, cate2);
            ViewBag.NewArrival2 = NewArrival2;


            var NewArrival3 = CachingNewProductArrival(sqlSelect, sqlWhere + cate3, 6, cate3);
            ViewBag.NewArrival3 = NewArrival3;


            var NewArrival4 = CachingNewProductArrival(sqlSelect, sqlWhere + cate4, 10, cate4);
            ViewBag.NewArrival4 = NewArrival4;


            return PartialView("UC/CategoryShowCase");
        }


        [HttpPost]
        public ActionResult BindLoadProductShowcase()
        {
            var svProduct = new ProductService();
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            var sqlSelect = " ProductID,ProductName,CompID,CompName,CompLevel,ProductImgPath,ShortDescription,ProvinceName,Price,Price_One,Ispromotion,PromotionPrice,QtyUnit";
            var cate1 = 0;
            var cate2 = 0;
            var cate3 = 0;
            var cate4 = 0;
        
            switch (res.Common.lblWebsite)
            {
                case "Ouikum.com": cate1 = 1; cate2 = 7; cate3 = 6; cate4 = 10;  sqlWhere += " And CateLV1=";    break;
                case "AntCart": cate1 = 6566; cate2 = 6277; cate3 = 6103; cate4 = 5888;     sqlWhere += " And CateLV1=";    break;
                case "myOtopThai": cate1 = 7377; cate2 = 7341; cate3 = 7508; cate4 = 7426;  sqlWhere += " And CateLV2=";    break;
                case "AppstoreThai": cate1 = 7611; cate2 = 7571; cate3 = 7596; cate4 = 7175;sqlWhere += " And CateLV2=";    break;
                case "Myanmar": cate1 = 3068; cate2 = 2598; cate3 = 3494; cate4 = 1189;     sqlWhere += " And CateLV1="; break; 
                default: break;
            }
            var NewArrival1 = CachingNewProductArrival(sqlSelect, sqlWhere + cate1, 1, cate1);
            var NewArrival2 = CachingNewProductArrival(sqlSelect, sqlWhere + cate2, 7, cate2);
            var NewArrival3 = CachingNewProductArrival(sqlSelect, sqlWhere + cate3, 6, cate3);
            var NewArrival4 = CachingNewProductArrival(sqlSelect, sqlWhere + cate4, 10, cate4);

            return Json(new
            {
                Arrival1 = NewArrival1,
                Arrival2 = NewArrival2,
                Arrival3 = NewArrival3,
                Arrival4 = NewArrival4,
            
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
        public ActionResult LoadSoftwareCategory(int CateID,int Type)
        {
            var svCategory = new CategoryService();
            var cates = new List<b2bCategory>();
            var name = "SubCategory-"+CateID+"-"+Type;
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
    }
}
