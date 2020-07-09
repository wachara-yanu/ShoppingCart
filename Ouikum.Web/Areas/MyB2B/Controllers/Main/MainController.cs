using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Common;
using Ouikum;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.Buylead;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Cart;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.OrderPuchase;

namespace Ouikum.Web.MyB2B
{
    public partial class MainController : BaseSecurityController
    {  
        #region Get Index
        public ActionResult Index()
        {
             
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            List_DoloadData();
            CountQuotation();
            GetCompStatus();
            CountMessage();
            CountQuotations();
            ViewBag.CateLevel1 = LogonCompLevel;
            ViewBag.MemberType = LogonMemberType;
            ViewBag.PageType = "MainIndex";
            return View();
        }
        #endregion

        #region List_DoloadData
        public void List_DoloadData()
        {
            GetCompany(LogonCompID);
            GetProductMaxView(LogonCompID);
            GetQuotationMaxView(LogonCompID);
            GetContactMaxView(LogonCompID);
            GetNewMessage(LogonCompID);
            GetNewQuotation(LogonCompID);
            GetMaxProduct(LogonCompLevel);
            GetStatusUser();
        }
        #endregion

        #region GetMaxProduct
        public void GetMaxProduct(int CompLevel){

            var svPackage = new Order.PackageService();
            var MaxProduct = 999999;
            if (CompLevel == 1)
            {
                MaxProduct = svPackage.GetMaxProduct(CompLevel);
            }
            ViewBag.MaxProduct = MaxProduct;

        }
        #endregion

        #region GetCompStatus
        public ActionResult GetCompStatus()
        {
            MyB2BStatus(LogonCompID);

            return Json(new
            {
                CountProduct = (int)ViewBag.CountProduct,
                CountProductApprove = (int)ViewBag.CountProductApprove,
                CountProductWait = (int)ViewBag.CountProductWait,
                CountProductReject = (int)ViewBag.CountProductReject,
                //CountBuylead = (int)ViewBag.CountBuylead,
                //CountBuyleadReject = (int)ViewBag.CountBuyleadReject,
                UnRead = (int)ViewBag.UnRead
                //Important = (int)ViewBag.Important,
                
                //Inbox = (int)ViewBag.Inbox
            });
        }
        #endregion

        #region MyB2BStatus
        public void MyB2BStatus(int CompID)
        {
            var svProduct = new ProductService();
            var svBuylead = new BuyleadService();
            var svMessage = new MessageService();
            var svQuotation = new QuotationService();
            var svOrderPur = new OrderPurchaseService();
            string sqlwhere = string.Empty;
            //string sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID);
            var count = 0;
            //var sqlwherein = "";
            //    switch (res.Common.lblWebsite)
            //    {
            //        case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //        case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //        case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //        case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //        default: sqlwherein = ""; break;
            //    }sqlwhere += sqlwherein;
            sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID) ;
            
            count = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProduct = count;

            sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID) + " AND RowFlag = 3";
            count = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProductReject = count;

            sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID) + " AND RowFlag IN (2)";
                count = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
                ViewBag.CountProductWait = count;

                sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID) + " AND RowFlag IN (4,5)";
            count = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProductApprove = count;
            //sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.All, CompID);
            //count = svProduct.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            //ViewBag.CountBuylead = count;

            //sqlwhere = svBuylead.CreateWhereAction(BuyleadAction.All, CompID) + " AND RowFlag = 3 ";
            //count = svBuylead.CountData<b2bBuylead>("BuyleadID", sqlwhere);
            //ViewBag.CountBuyleadReject = count;

            //sqlwhere = svMessage.CreateWhereAction(MessageStatus.Inbox, CompID);
            //count = svMessage.CountData<emMessage>("MessageID", sqlwhere);
            //ViewBag.Inbox = count;

            sqlwhere = svMessage.CreateWhereAction(MessageStatus.UnRead, CompID);
            count = svMessage.CountData<emMessage>("MessageID", sqlwhere);
            ViewBag.UnRead = count;

        }
        #endregion

        #region CountQuotation
        public void CountQuotation()
        {
            string SQLWhere = "";
            var svQuotation = new QuotationService();
            var svOrderPur = new OrderPurchaseService();
            SQLWhere = svQuotation.CreateWhereAction(QuotationAction.CountQuotation, LogonCompID);
            SQLWhere += " AND (IsRead = 0)";
            ViewBag.QuoCount = svQuotation.SelectData<b2bQuotation>("QuotationID", SQLWhere, "QuotationID", 1, 0).Count;

            var SQLWhereList = svQuotation.CreateWhereAction(QuotationAction.BackEnd, LogonCompID);
            ViewBag.QuoCountList = svQuotation.SelectData<b2bQuotation>("QuotationID", SQLWhereList, "QuotationID", 1, 0).Count;

            SQLWhere = svOrderPur.CreateWhereAction(OrderPurchaseAction.CountOrderPurchase, LogonCompID);
            ViewBag.CountOrP = svOrderPur.SelectData<OuikumOrderDetail>("OrDetailID", SQLWhere, "OrDetailID", 1, 0).Count;

        }
        #endregion

        #region CountMessage
        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInboxList = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Inbox, LogonCompID), null, 0, 0).Count();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }
        #endregion
        
        #region CountQuotations
        public void CountQuotations()
        {
            var svQuotation = new QuotationService();
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
        }
        #endregion
    }
}
