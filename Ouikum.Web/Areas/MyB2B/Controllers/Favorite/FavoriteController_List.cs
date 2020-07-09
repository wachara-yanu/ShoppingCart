using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using other
//using Prosoft.Base;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum.Favorite;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class FavoriteController : BaseSecurityController
    {
        public string sqlwherein;
        #region Index(Product-Defalut)
        [HttpGet]
        public ActionResult Index(string TextSearch)
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            GetStatusUser();
            #region query Product
            sqlSelect = "ProductID,ProductName,CompProductCount,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,ProductCompID,ModifiedDate,CreatedDate";
            sqlWhere = svFavProduct.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavProduct.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0);
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            sqlWhere += sqlwherein;
            var Products = svFavProduct.SelectData<view_FavProduct>(sqlSelect, sqlWhere, "CreatedDate DESC", 1, 20);
            ViewBag.Products = Products;
            #endregion


            #region query Biztype
            var Biztype = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName", "Rowflag > 0", "BizTypeID");
            ViewBag.Biztype = Biztype;
            #endregion

            #region query Province
            ViewBag.Province = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "Rowflag = 1 AND IsDelete = 0", "RegionID");
            #endregion

            SelectList_PageSize();

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = svFavProduct.TotalRow;// String.Format("{0:0,0}", svFavProduct.TotalRow);
            ViewBag.TotalPage = svFavProduct.TotalPage;//String.Format("{0:0,0}", svFavProduct.TotalPage);
            #endregion

            return View();

        }
        #endregion

        #region PostProduct
        [HttpPost]
        public ActionResult Product(int PIndex, int PSize, string TextSearch)
        {

            sqlSelect = "ProductID,ProductName,CompProductCount,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,ProductCompID";

            #region DoWhereCause
            sqlWhere = svFavProduct.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavProduct.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0, 0);
            #endregion


            var b2bProducts = svFavProduct.SelectData<view_FavProduct>(sqlSelect, sqlWhere, sqlOrderBy, PIndex, PSize);
            ViewBag.Products = b2bProducts;

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalRow = svFavProduct.TotalRow;// String.Format("{0:0,0}", svProduct.TotalRow);
            ViewBag.TotalPage = svFavProduct.TotalPage;// String.Format("{0:0,0}", svProduct.TotalPage);
            #endregion

            return PartialView("UC/ProductListUC");

        }
        #endregion

        #region GetSuppliers
        [HttpGet]
        public ActionResult Suppliers(string TextSearch)
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            GetStatusUser();
            #region query Supplier
            sqlSelect = "FavSupplierID,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ProductCount,LogoImgPath,ProvinceName";
            sqlWhere = svFavCompany.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavCompany.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0);
            var Supplier = svFavCompany.SelectData<view_FavCompany>(sqlSelect, sqlWhere, "CreatedDate DESC", 1, 20);
            ViewBag.Supplier = Supplier;
            #endregion

            #region query Biztype
            var Biztype = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName", "Rowflag > 0", "BizTypeID");
            ViewBag.Biztype = Biztype;
            #endregion

            #region query Province
            ViewBag.Province = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "Rowflag = 1 AND IsDelete = 0", "RegionID");
            #endregion

            SelectList_PageSize();

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = svFavCompany.TotalRow;// String.Format("{0:0,0}", svFavProduct.TotalRow);
            ViewBag.TotalPage = svFavCompany.TotalPage;//String.Format("{0:0,0}", svFavProduct.TotalPage);
            #endregion

            return View();

        }
        #endregion

        #region PostSupplier
        [HttpPost]
        public ActionResult Suppliers(int PIndex, int PSize, string TextSearch)
        {

            sqlSelect = "FavSupplierID,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,ProductCount,LogoImgPath,ProvinceName";

            #region DoWhereCause
            sqlWhere = svFavCompany.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavCompany.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0, 0);
            #endregion


            var Supplier = svFavCompany.SelectData<view_FavCompany>(sqlSelect, sqlWhere, sqlOrderBy, PIndex, PSize);
            ViewBag.Supplier = Supplier;

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalRow = svFavCompany.TotalRow;// String.Format("{0:0,0}", svProduct.TotalRow);
            ViewBag.TotalPage = svFavCompany.TotalPage;// String.Format("{0:0,0}", svProduct.TotalPage);
            #endregion

            return PartialView("UC/SupplierListUC");

        }
        #endregion

        #region GetBuyers
        [HttpGet]
        public ActionResult Buyers(string TextSearch)
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            GetStatusUser();
            #region query Buylead
            sqlSelect = "BuyleadID,BuyleadName,CompID,BuyleadCompanyName,CompLevel,BizTypeName,BizTypeOther,BuyleadCount,BuyleadIMGPath,BuyleadDetail,ProvinceName,BLCompID,BuyleadContactPerson,BuyleadTel,BuyleadEmail";
            sqlWhere = svFavBuylead.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavBuylead.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0);
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            //sqlWhere += sqlwherein;
            var Buyers = svFavBuylead.SelectData<view_FavBuyLead>(sqlSelect, sqlWhere, "CreatedDate DESC", 1, 20);
            ViewBag.Buyers = Buyers;
            #endregion

            #region query Biztype
            var Biztype = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName", "Rowflag > 0", "BizTypeID");
            ViewBag.Biztype = Biztype;
            #endregion

            #region query Province
            ViewBag.Province = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "Rowflag = 1 AND IsDelete = 0", "RegionID");
            #endregion

            SelectList_PageSize();

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = svFavBuylead.TotalRow;// String.Format("{0:0,0}", svFavProduct.TotalRow);
            ViewBag.TotalPage = svFavBuylead.TotalPage;//String.Format("{0:0,0}", svFavProduct.TotalPage);
            #endregion

            return View();

        }
        #endregion

        #region PostBuyers
        [HttpPost]
        public ActionResult Buyers(int PIndex, int PSize, string TextSearch)
        {

            sqlSelect = "BuyleadID,BuyleadName,CompID,BuyleadCompanyName,CompLevel,BizTypeName,BizTypeOther,BuyleadCount,BuyleadIMGPath,BuyleadDetail,ProvinceName,BLCompID,BuyleadContactPerson,BuyleadTel,BuyleadEmail";

            #region DoWhereCause
            sqlWhere = svFavBuylead.CreateWhereAction(FavAction.IsFav, LogonCompID);
            sqlWhere += svFavBuylead.CreateWhereCause(0, TextSearch, 0, 0, 0, 0, 0, 0, 0);
            #endregion


            var Buyers = svFavBuylead.SelectData<view_FavBuyLead>(sqlSelect, sqlWhere, sqlOrderBy, PIndex, PSize);
            ViewBag.Buyers = Buyers;

            #region Set ViewBag
            ViewBag.TextSearch = TextSearch != "" ? TextSearch : "";
            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalRow = svFavBuylead.TotalRow;// String.Format("{0:0,0}", svProduct.TotalRow);
            ViewBag.TotalPage = svFavBuylead.TotalPage;// String.Format("{0:0,0}", svProduct.TotalPage);
            #endregion

            return PartialView("UC/BuyerListUC");

        }
        #endregion

    }
}

