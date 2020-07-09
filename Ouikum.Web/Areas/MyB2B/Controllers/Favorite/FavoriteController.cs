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
using Ouikum.BizType;
using Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class FavoriteController : BaseSecurityController
    {
        #region Set DefaultInfo Favorite Service
        string sqlSelect, sqlWhere, sqlOrderBy = "";
        FavProductService svFavProduct;
        FavBuyleadService svFavBuylead;
        FavCompanyService svFavCompany;
        CompanyService svCompany;
        BizTypeService svBizType;
        AddressService svAddress;

        public FavoriteController()
        {
            svFavProduct = new FavProductService();
            svFavBuylead = new FavBuyleadService();
            svFavCompany = new FavCompanyService();
            svBizType = new BizTypeService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
        }
        #endregion

        #region GetProductName From AutoComplete
        [HttpPost]
        public ActionResult GetProductName(string query)
        {
            svFavProduct = new FavProductService();

            sqlWhere = svFavProduct.CreateWhereAction(FavAction.IsFav, 0);
            sqlWhere += svFavProduct.CreateWhereCause(0, query, 0, 0, 0, 0, 0, 0, 0);

            var FavProducts = svFavProduct.SelectData<view_FavProduct>("ProductName", sqlWhere, "ProductName");
            var ProductName = FavProducts.Select(it => it.ProductName).ToList();

            return Json(ProductName);

        }
        #endregion

        #region GetCompName From AutoComplete
        [HttpPost]
        public ActionResult GetCompName(string query)
        {
            svFavCompany = new FavCompanyService();

            sqlWhere = svFavCompany.CreateWhereAction(FavAction.IsFav, 0);
            sqlWhere += svFavCompany.CreateWhereCause(0, query, 0, 0, 0, 0, 0, 0, 0);

            var FavComp = svFavCompany.SelectData<view_FavCompany>("CompName", sqlWhere, "CompName");
            var CompName = FavComp.Select(it => it.CompName).ToList();

            return Json(CompName);

        }
        #endregion

        #region GetBuyleadName From AutoComplete
        [HttpPost]
        public ActionResult GetBuyleadName(string query)
        {
            svFavBuylead = new FavBuyleadService();

            sqlWhere = svFavBuylead.CreateWhereAction(FavAction.IsFav, 0);
            sqlWhere += svFavBuylead.CreateWhereCause(0, query, 0, 0, 0, 0, 0, 0, 0);

            var FavBuyleads = svFavBuylead.SelectData<view_FavBuyLead>("BuyleadName", sqlWhere, "BuyleadName");
            var BuyleadName = FavBuyleads.Select(it => it.BuyleadName).ToList();

            return Json(BuyleadName);

        }
        #endregion

        #region PostAddFavProduct
        [HttpPost]
        public ActionResult AddFavProduct(List<int> ProId)
        {
            var svFavProduct = new FavProductService();
            b2bFavProduct model = new b2bFavProduct();

            try
            {

                #region Set FavProduct Model
                foreach (var Pro in ProId)
                {
                    var IsResult = svFavProduct.InsertFavProduct(Pro, LogonCompID);
                    if (ProId.Count() == 1)
                    {
                        return Json(IsResult);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }


            return Json(new { IsResult = svFavProduct.IsResult, MsgError = GenerateMsgError(svFavProduct.MsgError), ID = model.FavProductID });
        }

        private bool ValidateFav(int Pro)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PostAddFavSupplier
        [HttpPost]
        public ActionResult AddFavSupplier(List<int> SupID)
        {
            var svFavCompany = new FavCompanyService();
            b2bFavCompany model = new b2bFavCompany();

            try
            {
                #region Set svFavcompany Model
                foreach (var Sup in SupID)
                {
                    var IsResult = svFavCompany.InsertFavCompany(Sup, LogonCompID);
                    if (SupID.Count() == 1)
                    {
                        return Json(IsResult);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }


            return Json(new { IsResult = svFavCompany.IsResult, MsgError = GenerateMsgError(svFavCompany.MsgError), ID = model.FavCompID });
        }
        #endregion

        #region PostAddFavBuylead
        [HttpPost]
        public ActionResult AddFavBuylead(List<int> BuyId)
        {
             var svFavBuylead = new FavBuyleadService();
            b2bFavBuylead model = new b2bFavBuylead();

            try
            {
                #region Set svFavBuylead Model
                foreach (var Buy in BuyId)
                {
                    var IsResult = svFavBuylead.InsertFavBuylead(Buy, LogonCompID);
                    if(BuyId.Count() == 1)
                    {
                        return Json(IsResult);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }


            return Json(new { IsResult = svFavBuylead.IsResult, MsgError = GenerateMsgError(svFavBuylead.MsgError), ID = model.FavoriteBuyleadID });
        }
        #endregion


        #region PostDeleteFavProduct
        [HttpPost]
        public ActionResult DeleteFavProduct(List<int> ProductID)
        {
            var svFavProduct = new FavProductService();
            try
            {
                svFavProduct.Delete(ProductID,LogonCompID);
            }
            catch (Exception ex)
            {
                svFavProduct.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svFavProduct.IsResult, MsgError = GenerateMsgError(svFavProduct.MsgError) });
        }
        #endregion

        #region PostDeleteFavComp
        [HttpPost]
        public ActionResult DeleteFavComp(List<int> FavSupplierID)
        {
            var svFavCompany = new FavCompanyService();
            try
            {
                svFavCompany.Delete(FavSupplierID, LogonCompID);
            }
            catch (Exception ex)
            {
                svFavCompany.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svFavCompany.IsResult, MsgError = GenerateMsgError(svFavCompany.MsgError) });
        }
        #endregion

        #region PostDeleteFavBuylead
        [HttpPost]
        public ActionResult DeleteFavBuylead(List<int> BuyleadID)
        {
            var svFavBuylead = new FavBuyleadService();
            try
            {
                svFavBuylead.Delete(BuyleadID, LogonCompID);
            }
            catch (Exception ex)
            {
                svFavBuylead.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svFavBuylead.IsResult, MsgError = GenerateMsgError(svFavBuylead.MsgError) });
        }
        #endregion

        

    }
}

