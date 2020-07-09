using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Quotation;
using res = Prosoft.Resource.Web.Ouikum;
using System.Collections;
using Ouikum.Product;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 

        #region Get Request Price Inbox
        public ActionResult inboxrequest(int compid)
        {
            var svQuota = new QuotationService();
            var sqlwhere = svQuota.CreateWhereAction(QuotationAction.myRequest, compid);
            sqlwhere += " AND IsOutbox = 0 AND IsMatching = 0";
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", sqlwhere);

            quotation = SetMappingQuotation(quotations);
            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Request Price Sent
        public ActionResult sentboxrequest(int compid)
        {
            var svQuota = new QuotationService();
            var sqlwhere = svQuota.CreateWhereAction(QuotationAction.Sentbox, compid);
            sqlwhere += " AND IsMatching = 0";
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", sqlwhere);

            quotation = SetMappingQuotation(quotations);
            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Send Request Price

        #region Post SendRequestPriceTest
        public ActionResult SendRequestPriceTest(
            string rootquotationcode,
            int productid,
            int qty,
            string qtyunit,
            int tocompid,
            string compname,
            int fromcompid,
            bool ispublic,
            string phone,
            string email,
            string detail,
            string status
            )
        {
            var svQuotation = new QuotationService();
            svQuotation.SendMatching(rootquotationcode, productid, qty, qtyunit, tocompid, compname, fromcompid, ispublic, phone, email, detail, status);

            return Json(new { status = svQuotation.IsResult }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Post RequestPrice
        [HttpPost]
        public ActionResult SendRequestPrice(
            string rootquotationcode,
            int productid,
            int qty,
            string qtyunit,
            int tocompid,
            string compname,
            int fromcompid,
            bool ispublic,
            string phone,
            string email
            )
        {

            var svQuotation = new QuotationService();
            svQuotation.SendMatching(rootquotationcode, productid, qty, qtyunit, tocompid, compname, fromcompid, ispublic, phone, email);

            return Json(new { status = svQuotation.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Mathing Company
        public List<b2bCompany> GetMatchingCompany(int productid)
        {
            var svProduct = new ProductService();
            var svCompany = new CompanyService();
            var sqlwhere = @"compid in (  select  DISTINCT compid from b2bproduct " +
                "where CateLV3 = (select CateLV3 from b2bproduct where productid = " + productid + "))";
            var companies = svProduct.SelectData<b2bCompany>(" * ", sqlwhere, "CompID desc", 1, 10);

            return companies;
        }
        #endregion

        public ActionResult GetMatching(int productid)
        {
            return Json(new { comp = GetMatchingCompany(productid) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Open  request Price
        public ActionResult openrequest(int quotationid)
        {
            var svQuota = new QuotationService();
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", " QuotationID = " + quotationid);

            #region update is read
            if (svQuota.TotalRow > 0)
            {
                svQuota.UpdateByCondition<view_Quotation>("IsRead = 1", " QuotationID = " + quotationid + "  ");
            }
            #endregion

            if (quotations != null && quotations.Count > 0)
            {
                var qid = quotations.Select(m => m.QuotationCode).ToList();
                var wheresubmsg = CreateWhereINString(qid, "RootQuotationCode");
                var submsg = svQuota.SelectData<view_Quotation>(" * ", "IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in quotations)
                {
                    #region  Set Model
                    var m = new QuotationModel();
                    m.quotaionid = item.QuotationID;
                    m.quotaioncode = item.QuotationCode;
                    m.rootquotationcode = item.RootQuotationCode;
                    m.productid = (int)item.ProductID;
                    m.productname = item.ProductName;
                    m.qty = (int)item.Qty;
                    m.qtyunit = item.QtyUnit;
                    m.rootquotationcode = item.RootQuotationCode;
                    m.fromcompid = (int)item.FromCompID;
                    m.fromname = item.FromCompName;
                    m.tocompid = (int)item.ToCompID;
                    m.toname = item.ToCompName;
                    m.msgdetail = item.Remark;
                    m.isfavorite = (bool)item.IsImportance;
                    m.isread = (bool)item.IsRead;
                    m.isreply = (bool)item.IsReply;
                    m.isoutbox = (bool)item.IsOutbox;
                    m.createdate = (DateTime)item.CreatedDate;
                    m.modifieddate = (DateTime)item.ModifiedDate;
                    m.subquotation = new List<QuotationModel>();
                    #endregion

                    #region sub quotaion
                    if (submsg != null && submsg.Count > 0)
                    {
                        foreach (var sub in submsg.Where(it => it.QuotationCode == item.QuotationCode && it.QuotationID != item.QuotationID))
                        {
                            #region QuotationModel
                            var s = new QuotationModel();
                            s.quotaionid = sub.QuotationID;
                            s.quotaioncode = sub.QuotationCode;
                            s.rootquotaionid = item.QuotationID;
                            s.rootquotationcode = sub.RootQuotationCode;
                            s.fromcompid = (int)sub.FromCompID;
                            s.fromname = sub.FromCompName;
                            s.tocompid = (int)sub.ToCompID;
                            s.toname = sub.ToCompName;
                            s.msgdetail = sub.Remark;
                            s.isfavorite = (bool)sub.IsImportance;
                            s.isread = (bool)sub.IsRead;
                            s.isreply = (bool)sub.IsReply;
                            s.isoutbox = (bool)sub.IsOutbox;
                            s.createdate = (DateTime)sub.CreatedDate;
                            s.modifieddate = (DateTime)sub.ModifiedDate;
                            s.subquotation = new List<QuotationModel>();
                            m.subquotation.Add(s);
                            #endregion
                        }
                    }
                    #endregion
                    quotation.Add(m);
                }
                #endregion

            }
            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Request Price
        public ActionResult deleterequest(int[] messageid)
        {
            var svQuota = new QuotationService();
            var sqlwhere = CreateWhereIN(messageid, "QuotationID");
            var sql = " IsDelete =  1";
            svQuota.UpdateByCondition<b2bQuotation>(sql, sqlwhere);
            return Json(new { status = svQuota.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Favorite Request Price
        public ActionResult favrequest(int messageid, bool fav)
        {
            var svQuota = new QuotationService();
            var sql = " IsImportance =  0";
            if (fav)
                sql = " IsImportance =  1";

            var quotations = svQuota.UpdateByCondition<b2bQuotation>(sql, "QuotationID = " + messageid + " "); //ใส่ '' ครอบ เนื่องจาก Quotationcode เป็น String
            return Json(new { status = svQuota.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
