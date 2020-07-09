using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
using Ouikum.Product;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Purchase
{
    public partial class SearchController : BaseController
    {
        //
        // GET: /Purchase/Search/
        string SqlSelect, SqlWhere = "";

        #region Get Index
        public ActionResult Index(string TextSearch)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            GetStatusUser();

            #region Category 
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6) "; break;
            //    default: sqlwherein = ""; break;
            //}
            //SqlWhere += sqlwherein;
            ViewBag.Category = svCategory.GetCategoryByLevelAndBuyleadCount(1);
            #endregion

            #region query Province
            LoadProvinces();
            //ViewBag.Province = svAddress.GetProvinceAll();
            #endregion

            #region Set ViewBag
            ViewBag.txtSearch = TextSearch;
            ViewBag.CateID = 0;
            ViewBag.ProvinceID = 0;
            ViewBag.CreatedDate = 0;
            ViewBag.BuyleadExpDate = 0;
            ViewBag.BuyleadNotExpDate = 0;
            ViewBag.BuyleadType = 0;
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.EnumSearchByPurchase = svCommon.SelectEnum(CommonService.EnumType.SearchByPurchase);
            ViewBag.TextSearch = TextSearch;
            #endregion

            #region query Buylead
            List_DoloadData(BuyleadAction.FrontEnd);
            #endregion


            return View();
        }
        #endregion

        #region Post Index
        [HttpPost]
        public ActionResult Index(FormCollection form, string txtSearch, int? BuyleadType, int? BuyleadNotExpDate, int? BuyleadExpDate, int? CategoryID, int? ProvinceID, int PageTotal, int PIndex)
        {
            //if (BuyleadExpDate == 0) { PIndex = 1;  }
            //if (BuyleadNotExpDate == 0) { PIndex = 1; }

            #region Set ViewBag
            ViewBag.txtSearch = txtSearch;
            ViewBag.BuyleadExpDate = BuyleadExpDate;
            ViewBag.BuyleadNotExpDate = BuyleadNotExpDate;
            ViewBag.CateID = CategoryID;
            ViewBag.ProvinceID = ProvinceID;
            //ViewBag.CreatedDate = CreatedDate; ไม่ตั้งเงื่อนไขแล้ว จะให้เรียงตาม Create อย่างเดียว
            ViewBag.BuyleadType = BuyleadType;
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PageTotal;
            #endregion
            
            List_DoloadData(BuyleadAction.FrontEnd);
            return PartialView("UC/PurchaseListUC");
        }
        #endregion

        #region Get Detail
        [HttpGet]
        public ActionResult Detail(int? ID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            GetStatusUser();
            #region รายละเอียดประกาศซื้อ
            svBuylead = new BuyleadService();
            var Buyleads = svBuylead.SelectData<view_BuyLead>("*", "BuyleadID = " + ID, null, 1, 0);

            if (svBuylead.TotalRow == 0)
            {
                return Redirect("~/Default/NotFound");
            }

            var buylead = Buyleads.First();
            ViewBag.BuyleadDetail = buylead;
            if (buylead.ProvinceName == "กรุงเทพมหานคร") {
                buylead.ProvinceName = "กรุงเทพ";
            }
            ViewBag.Title = buylead.BuyleadName + " | " + buylead.ProvinceName;
            
            if (!string.IsNullOrEmpty(buylead.BuyleadKeyword))
            ViewBag.Title += buylead.BuyleadKeyword.Replace("~", " ");
            ViewBag.Title += " | " + res.Product.lblBuyleadList + " | " + res.Common.lblDomainShortName;
            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription = ViewBag.Title;
            #endregion

            //#region ผู้ขายสินค้าประเภทนี้
            //ViewBag.Buyer = svBuylead.SelectData<view_PurchaseComp>("*", "CateLV3 IN (" + buylead.CateLV3 + ") AND RowFlag IN(2,4) AND ProductCount > 0", "CompID", 1, 7);
            //#endregion

            //#region สินค้าที่เกี่ยวข้องกับประกาศซื้อ
            //var svProduct = new ProductService();
            //SqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            //SqlWhere += " AND (CateLV3  = " + buylead.CateLV3 + ") AND (ProductImgPath <> '') ";
            //ViewBag.ProductOther = svProduct.SelectData<view_Product>("ProductID,ProductImgPath,CompID,ProductName,CateLV3,IsDelete,RowFlag,CompRowFlag,IsShow,IsJunk", SqlWhere, "ProductID", 1, 7);
            //#endregion

            #region ประกาศซื้อใกล้เคียง
            SqlWhere = svBuylead.CreateWhereAction(BuyleadAction.FrontEnd);
            SqlWhere += " AND CateLV3 = " + buylead.CateLV3 + " AND BuyleadID != " + ID;
            ViewBag.BuyerOther = svBuylead.SelectData<view_BuyLead>("*", SqlWhere, "BuyleadID", 1, 5);
            #endregion

            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.EnumSearchByPurchase = svCommon.SelectEnum(CommonService.EnumType.SearchByPurchase);
            AddViewCount((int)ID, "Buylead");
            return View();
        }
        #endregion

        #region GetBuyleadName
        [HttpPost]
        public ActionResult GetBuyleadName(string query)
        {
            svBuylead = new BuyleadService();

            SqlWhere = svBuylead.CreateWhereAction(BuyleadAction.FrontEnd, 0);
            SqlWhere += svBuylead.CreateWhereCause(0, query, 0, 1,0,0, 0,0);
            SqlWhere += " AND (convert(nvarchar(20), BuyleadExpDate,112) > '" + DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US")) + "')";

            var b2bBuyleads = svBuylead.SelectData<view_BuyLead>("BuyleadName", SqlWhere, "BuyleadName");
            var BuyleadName = b2bBuyleads.Select(it => it.BuyleadName).ToList();

            return Json(BuyleadName);

        }
        #endregion
    }
}
