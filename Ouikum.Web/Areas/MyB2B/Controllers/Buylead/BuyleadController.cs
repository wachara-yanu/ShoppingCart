

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using Prosoft.Base;
using Ouikum.Buylead;
using Ouikum;
using Ouikum.Category;
using Ouikum.Message;
using Ouikum.Quotation;

namespace Ouikum.Web.MyB2B
{
    public partial class BuyleadController : BaseSecurityController
    {
        public string sqlSelect, sqlWhere, sqlOrderBy, GenBuyleadCode = "";
        // GET: /MyB2B/Buylead/ 
        public void GetStatusByCompID(int CompID)
        {
            var svBuylead = new BuyleadService();

            string sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.Junk, CompID);
            var CountJunk = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountJunk = CountJunk;

            sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.BackEnd, CompID) + " AND ListNo = 0 ";
            var CountBuylead = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountBuylead = CountBuylead;

            sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.All, CompID);
            var CountAllitem = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            ViewBag.CountAllitem = CountAllitem;

            sqlwhere += svBuylead.CreateWhereCause(0, "", 4);
            var CountBuyleadApprove = svBuylead.CountData<view_BuyLead>("BuyleadID", sqlwhere);
            ViewBag.CountBuyleadApprove = CountBuyleadApprove;
        }

        public ActionResult GetStatus()
        {
            GetStatusByCompID(LogonCompID);
            return Json(new
            {
                CountJunk = (int)ViewBag.CountJunk,
                CountBuylead = (int)ViewBag.CountBuylead,
                CountBuyleadApprove = (int)ViewBag.CountBuyleadApprove,
                CountBuyleadWait = ((int)ViewBag.CountAllitem - (int)ViewBag.CountBuyleadApprove),
                CountAllitem = (int)ViewBag.CountAllitem
            });
        }

        public void List_DoloadData(BuyleadAction action)
        {
            ViewBag.PageSize = 19;
            var svBuylead = new BuyleadService();
            sqlSelect = "BuyleadID,BuyleadName,BuyleadIMGPath,Remark,RowFlag,CompID,CateLV1,CateLV2,CateLV3,RowVersion";
            sqlWhere = svBuylead.CreateWhereAction(action) + " AND ListNo = 0 ";

            sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svBuylead.CreateWhereCause(LogonCompID, ViewBag.TextSearch, (int)ViewBag.PStatus,
                (int)ViewBag.CateLevel, (int)ViewBag.CateID);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "ModifiedDate");
            #endregion

            var Buyleads = svBuylead.SelectData<b2bBuylead>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Buyleads = Buyleads;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svBuylead.TotalRow;
            ViewBag.TotalPage = svBuylead.TotalPage;
        }


        #region PostSearchCategory
        [HttpPost]
        public ActionResult SearchCategory(string CategoryName)
        {
            if (!string.IsNullOrEmpty(CategoryName))
            {
                var svCategory = new CategoryService();
                var data = svCategory.SearchCategoryByName(CategoryName);
                ViewBag.Categories = data;
                ViewBag.Countcategory = data.Count();
            }
            return PartialView("UC/CategoryList");
        }

        #endregion

        #region CountMessage
        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }
        #endregion

        #region CountQuotation
        public void CountQuotation()
        {
            var svQuotation = new QuotationService();
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
        }
        #endregion

    }
}
