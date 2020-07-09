using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
namespace Ouikum.Web.Admin
{
    public partial class ApproveBuyleadController : BaseSecurityAdminController
    {
        //
        // GET: /Admin/ApproveBuylead/ 
        public void List_DoloadData(BuyleadAction action)
        {
            var svBuylead = new BuyleadService();
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = "BuyleadID,BuyleadName,BuyleadEmail,RowFlag,CompID,CompName,RowVersion,Remark,CateLV1,CateLV2,CateLV3,Modifieddate,CreatedDate,AdminCode,CompCode,CreatedBy,ModifiedBy,CategoryType";
            sqlWhere = svBuylead.CreateWhereAction(action) + " ";

            sqlOrderBy = " CreatedDate DESC ";

            #region DoWhereCause
            sqlWhere += svBuylead.CreateWhereCause(0,"", (int)ViewBag.PStatus);
            sqlWhere += svBuylead.CreateWhereSearchBy(ViewBag.TextSearch, ViewBag.SearchType);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            #endregion

            var Buyleads = svBuylead.SelectData<view_SearchBuylead>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize); 
            ViewBag.Buyleads = Buyleads;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svBuylead.TotalRow;
            ViewBag.TotalPage = svBuylead.TotalPage;
        }
    }
}
