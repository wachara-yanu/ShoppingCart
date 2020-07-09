using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Product;
using Ouikum.Common;
namespace Ouikum.Web.Admin
{
    public partial class HotFeatController : BaseSecurityAdminController
    {
        #region List_DoloadData
        public void List_DoloadData()
        {
            var svHotFeat = new HotFeaProductService();
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = @"  HotFeaProductID,ProductID,ProductName,Price_One,Qty,QtyUnit,Price,CompID,"
                + "CompName,ActivatedDate,ExpiredDate,RowFlag,IsDelete,Status,PackageCount,CategoryType,HotPrice,ProductDelete";
            sqlWhere = svHotFeat.InitialWhere();

            sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svHotFeat.CreateWhereCauseHotFeat((string)ViewBag.TextSearch, (string)ViewBag.SearchType, (string)ViewBag.PStatus, int.Parse(ViewBag.ExpireStatus), (string)ViewBag.SearchStatus);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "ModifiedDate");
            #endregion

            var HotFeats = svHotFeat.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.HotFeats = HotFeats;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svHotFeat.TotalRow;
            ViewBag.TotalPage = svHotFeat.TotalPage;
        }
        #endregion

        #region List_DoloadDataProduct
        public void List_DoloadDataProduct(ProductAction action)
        {
            var svProduct = new ProductService();
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = "ProductID,ProductName,Price_One,Qty,QtyUnit,Price,RowFlag,CompID,CompName";
            sqlWhere = "( IsDelete = 0 AND RowFlag IN (4) AND CompRowFlag IN (2,4) AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0 ) ";//svProduct.CreateWhereAction(action) + 

            sqlOrderBy = " CreatedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(0, "", (int)ViewBag.PStatus);
            sqlWhere += svProduct.CreateWhereSearchBy(ViewBag.TextSearch, ViewBag.SearchType);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            #endregion

            var products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Products = products;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;
        }
        #endregion

        #region SetHotFeatPager
        public void SetHotFeatPager(FormCollection form)
        {
            ViewBag.PStatus = (!string.IsNullOrEmpty(form["PStatus"])) ? DataManager.ConvertToString(form["PStatus"]) : "";
            ViewBag.SearchBy = DataManager.ConvertToString(form["SearchBy"]);
            ViewBag.SearchType = !string.IsNullOrEmpty(form["SearchType"]) ? form["SearchType"] : "";
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
            ViewBag.ExpireStatus = !string.IsNullOrEmpty(form["ExpireStatus"]) ? form["ExpireStatus"] : "0";
            ViewBag.SearchStatus = !string.IsNullOrEmpty(form["SearchStatus"]) ? form["SearchStatus"] : "";
        }
        #endregion

        public void DoloadExpireHotFeat()
        {
            var svHotFeat = new HotFeaProductService();

            #region Count_Exp
            ViewBag.Count_to_Exp = svHotFeat.CountData<view_HotFeaProduct>(" * ", @" IsDelete = 0 AND (Status = 'H' OR Status = 'F') AND ExpiredDate Between '" + DateTime.Now.AddDays(1).ToShortDateString()
                + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'");
            #endregion

            #region Count_Exp_today
            ViewBag.Count_Exp_today = svHotFeat.CountData<view_HotFeaProduct>(" * ", @"IsDelete = 0 AND (Status = 'H' OR Status = 'F') AND ExpiredDate Between '" + DateTime.Now.ToShortDateString()
             + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'");
            #endregion

            #region Count_Exp
            ViewBag.Count_Exp = svHotFeat.CountData<view_HotFeaProduct>(" * ", "IsDelete = 0 AND (Status = 'H' OR Status = 'F') AND  ExpiredDate < '" + DateTime.Now.Date.ToShortDateString() + "'");
            #endregion
        }

        public JsonResult GetEditByID(string HotFeaProductID = "")
        {
            var svHotFeat = new HotFeaProductService();
            CommonService svCommon = new CommonService();

            var EnumHotFeatStatus = svCommon.SelectEnum(CommonService.EnumType.HotFeatStatus);
            var data = svHotFeat.SelectData<view_HotFeaProduct>("HotPrice,Status", "IsDelete = 0 AND HotFeaProductID=" + HotFeaProductID);

            return Json(new { IsResult = true, HotPrice = data.First().HotPrice, Status = data.First().Status, EnumHotFeatStatus = EnumHotFeatStatus });
        }
    }
}
