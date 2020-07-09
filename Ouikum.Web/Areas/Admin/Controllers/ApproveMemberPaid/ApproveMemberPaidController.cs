using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ouikum.Order;
using Ouikum.Common;
namespace Ouikum.Web.Admin
{
    public partial class ApproveMemberPaidController : BaseSecurityAdminController
    {
        //
        // GET: /Admin/ApproveMemberPaid/

        #region List_DoloadData
       

        public void List_DoloadData()
        {
            var svOrder = new OrderService();
           
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            sqlSelect = "MemberPaidID,MemberPaidCode,PaymentStatus,PayerName,IsInvoice,CreatedDate,OrderMemberPaidID,WebID,CompID,CompName";
            sqlWhere = "IsDelete = 0 AND OrderMemberPaidID Is not null";
            sqlOrderBy = " CreatedDate DESC ";

            #region DoWhereCause
            sqlWhere += svOrder.CreateWhereCauseMemberPaid(ViewBag.TextSearch,ViewBag.SearchType,ViewBag.PStatus);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            #endregion

            var MemberPaids = svOrder.SelectData<view_MemberPaidApprove>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.MemberPaids = MemberPaids;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svOrder.TotalRow;
            ViewBag.TotalPage = svOrder.TotalPage;
        }
        #endregion

        #region SetMemberPaidPager
        public void SetMemberPaidPager(FormCollection form)
        {
            ViewBag.Period = DataManager.ConvertToString(form["Period"]);
            ViewBag.PStatus = (!string.IsNullOrEmpty(form["PStatus"])) ? DataManager.ConvertToString(form["PStatus"]) : "N";
            ViewBag.SearchType = DataManager.ConvertToString(form["SearchType"]);
        }
        #endregion
    }
}
