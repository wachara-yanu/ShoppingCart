using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
namespace Ouikum.Web.Admin
{
    public partial class ApproveProductController : BaseSecurityAdminController
    {
        //
        // GET: /Admin/ApproveProduct/ 
        public void List_DoloadData(ProductAction action)
        {
            var svProduct = new ProductService();
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = "TOP 500 ProductID,ProductName,RowFlag,CompID,comprowflag,CompName,Remark,RowVersion,CateLV1,CateLV2,CateLV3,AdminCode,CompCode,CreatedBy,ModifiedBy,Modifieddate,CreatedDate,CategoryType ";
            sqlWhere = svProduct.CreateWhereAction(action) + " ";

            sqlOrderBy = " RowFlag DESC , CreatedDate DESC  ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(0,"", (int)ViewBag.PStatus);
            sqlWhere += svProduct.CreateWhereSearchBy(ViewBag.TextSearch, ViewBag.SearchType);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            if (LogonServiceType == 15)
                sqlWhere += " AND IsSME = 1";
            #endregion
            sqlWhere += " AND YEAR(Modifieddate) = YEAR(GETDATE())";

            //var products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

            var sql = "SELECT " + sqlSelect + " FROM view_SearchProduct WHERE"+ sqlWhere + " ORDER By " + sqlOrderBy;
            var listData = svProduct.qDB.ExecuteQuery<view_SearchProduct>(sql).ToList(); 
            var products = listData.Skip(((int)ViewBag.PageIndex - 1) * (int)ViewBag.PageSize).Take((int)ViewBag.PageSize).ToList();
            var test = (listData.Count() / (int)ViewBag.PageSize);
            ViewBag.Products = products;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = listData.Count();
            ViewBag.TotalPage = (listData.Count()/(int)ViewBag.PageSize);
        }
    }
}
