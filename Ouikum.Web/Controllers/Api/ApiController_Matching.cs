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

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 
        #region Get Request Price Inbox
        public string Getsubject(view_Quotation model, bool isMatching)
        {
            var subject = "";
            if (isMatching)
            {
                if ((bool)model.IsOutbox)
                {
                    subject = "คุณได้เปิดการเสนอราคา " + model.ProductName + " กับ " + model.ToCompName + " จำนวน " + model.Qty + " " + model.QtyUnit;
                }
                else
                {
                    subject = model.FromCompName + " สนใจ  " + model.ProductName + "  จำนวน " + model.Qty + " " + model.QtyUnit;
                }
            }
            else
            {
                if ((bool)model.IsOutbox)
                {
                    subject = "คุณได้ขอราคา " + model.ProductName + " กับ " + model.ToCompName + " จำนวน " + model.Qty + " " + model.QtyUnit;
                }
                else
                {
                    subject = model.FromCompName + " ได้ขอราคา " + model.ProductName + " จำนวน " + model.Qty + " " + model.QtyUnit;
                }
            }
            return subject;
        }
        public List<QuotationModel> SetMappingQuotation(List<view_Quotation> quotations, bool isMatching = false)
        {
            var quotation = new List<QuotationModel>();
            var svQuota = new QuotationService();

            #region Check AND Set Model
            if (quotations != null && quotations.Count > 0)
            {
                var qid = quotations.Select(m => m.QuotationCode).ToList();
                var wheresubmsg = CreateWhereINString(qid, "RootQuotationCode");
                // wheresubmsg += " and tocompid = 

                var submsg = svQuota.SelectData<view_Quotation>(" * ", " IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in quotations)
                {
                    var m = new QuotationModel();

                    if ((bool)item.IsOutbox)
                    {
                        var detail = "คุณ ติดต่อเพื่อขอราคา " + item.ProductName + "จำนวน" + item.Qty + "  " + item.QtyUnit + " ติดต่อกลับได้ที่" + item.ReqEmail + "หรือ โทร " + item.ReqPhone;
                        m.msgdetail = (!string.IsNullOrEmpty(item.ReqRemark)) ? item.ReqRemark : detail;
                    }
                    else
                    {
                        m.msgdetail = (!string.IsNullOrEmpty(item.ReqRemark)) ? item.ReqRemark : m.subject;
                    }

                    m.quotaionid = item.QuotationID;
                    m.quotaioncode = item.QuotationCode;
                    m.subject = Getsubject(item, true);

                    m.rootquotationcode = item.RootQuotationCode;
                    m.fromcompid = (int)item.FromCompID;
                    m.fromname = item.CompanyName;
                    m.productid = (int)item.ProductID;
                    m.productname = item.ProductName;
                    m.image = item.ProductImgPath;
                    m.fromcontactphone = item.ReqPhone;
                    m.fromemail = item.ReqEmail;
                    m.tocompid = (int)item.ToCompID;
                    m.toname = item.ToCompName;
                    m.isfavorite = (bool)item.IsImportance;
                    m.isread = (bool)item.IsRead;
                    m.isreply = (bool)item.IsReply;
                    m.isoutbox = (bool)item.IsOutbox;
                    m.createdate = (DateTime)item.CreatedDate;
                    m.modifieddate = (DateTime)item.ModifiedDate;
                    m.rootquotationcode = item.RootQuotationCode;
                    m.status = item.QuotationStatus;
                    m.subquotation = new List<QuotationModel>();
                    m.qty = (short)item.Qty;
                    m.qtyunit = item.QtyUnit;

                    #region sub quotaion
                    //if (submsg != null && submsg.Count > 0)
                    //{
                    //    foreach (var sub in submsg.Where(it => it.RootQuotationCode == item.QuotationCode ))
                    //    {
                    //        #region QuotationModel
                    //        var s = new QuotationModel();
                    //        s.quotaionid = sub.QuotationID;
                    //        s.quotaioncode = sub.QuotationCode;
                    //        s.subject = Getsubject(sub, true); 
                    //        s.rootquotationcode = sub.RootQuotationCode;
                    //        s.fromcompid = (int)sub.FromCompID;
                    //        s.fromname = sub.FromCompName;
                    //        s.tocompid = (int)sub.ToCompID;
                    //        s.toname = sub.ToCompName;
                    //        s.msgdetail = (!string.IsNullOrEmpty(sub.ReqRemark)) ? sub.ReqRemark : m.subject; ;
                    //        s.isfavorite = (bool)sub.IsImportance;
                    //        s.isread = (bool)sub.IsRead;
                    //        s.isreply = (bool)sub.IsReply;
                    //        s.isoutbox = (bool)sub.IsOutbox;
                    //        s.createdate = (DateTime)sub.CreatedDate;
                    //        s.modifieddate = (DateTime)sub.ModifiedDate;
                    //        s.productid = (int)sub.ProductID;
                    //        s.productname = sub.ProductName;
                    //        s.fromcontactphone = sub.ReqPhone;
                    //        s.image = sub.ProductImgPath;
                    //        s.rootquotationcode = sub.RootQuotationCode;
                    //        s.subquotation = new List<QuotationModel>();
                    //        m.subquotation.Add(s);

                    //        #endregion
                    //    }
                    //}
                    #endregion

                    quotation.Add(m);
                }
                #endregion
            }
            #endregion
            return quotation;
        }
        public ActionResult inboxmatching(int compid)
        {
            var svQuota = new QuotationService();
            var sqlwhere = svQuota.CreateWhereAction(QuotationAction.myRequest, compid);
            sqlwhere += " AND IsOutbox = 0 AND IsMatching = 1";
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", sqlwhere);
            quotation = SetMappingQuotation(quotations);
            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Request Price Sent
        public ActionResult sentboxmatching(int compid)
        {
            var svQuota = new QuotationService();
            var sqlwhere = svQuota.CreateWhereAction(QuotationAction.Sentbox, compid);
            sqlwhere += " AND IsMatching = 1";
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", sqlwhere);

            quotation = SetMappingQuotation(quotations);
            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Post SendMatchingTest
        public ActionResult SendMatchingTest(
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
            string message,
            string status
            )
        {
            var svQuotation = new QuotationService();
            svQuotation.SendMatching(rootquotationcode, productid, qty, qtyunit, tocompid, compname, fromcompid, ispublic, phone, email, message, status, true);

            return Json(new { status = svQuotation.IsResult });
        }
        #endregion


        #region Post SendMatching
        [HttpPost]
        public ActionResult SendMatching(
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
            string message,
            string status
            )
        {
            var svQuotation = new QuotationService();
            svQuotation.SendMatching(rootquotationcode, productid, qty, qtyunit, tocompid, compname, fromcompid, ispublic, phone, email, message, status, true);
            return Json(new { status = svQuotation.IsResult });
        }
        #endregion

        #region Open  matching Price
        public ActionResult openmatching(int messageid)
        {
            var svQuota = new QuotationService();
            var quotation = new List<QuotationModel>();
            var quotations = svQuota.SelectData<view_Quotation>(" * ", " QuotationID = " + messageid);

            #region update is read
            if (svQuota.TotalRow > 0)
            {
                svQuota.UpdateByCondition<view_Quotation>("IsRead = 1", " QuotationID = " + messageid + "  ");
            }
            #endregion

            #region quotations
            if (quotations != null && quotations.Count > 0)
            {
                var qid = quotations.Select(m => m.QuotationCode).ToList();
                var wheresubmsg = CreateWhereINString(qid, "RootQuotationCode");
                var submsg = svQuota.SelectData<view_Quotation>(" * ", "IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in quotations)
                {
                    var m = new QuotationModel();
                    m.quotaionid = item.QuotationID;
                    m.quotaioncode = item.QuotationCode;
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
                    #region sub quotaion
                    if (submsg != null && submsg.Count > 0)
                    {
                        foreach (var sub in submsg.Where(it => it.QuotationCode == item.QuotationCode && it.QuotationID != item.QuotationID))
                        {
                            var s = new QuotationModel();
                            s.quotaionid = sub.QuotationID;
                            s.quotaioncode = sub.QuotationCode;
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
                        }
                    }
                    #endregion
                    quotation.Add(m);
                }
                #endregion
            }
            #endregion

            return Json(new { quotation = quotation }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region delete matching
        public ActionResult deletematching(int[] messageid)
        {
            var svQuota = new QuotationService();

            var sqlwhere = CreateWhereIN(messageid, "QuotationID");
            var sql = " IsDelete =  1";
            svQuota.UpdateByCondition<b2bQuotation>(sql, sqlwhere);
            return Json(new { status = svQuota.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Favorite  matching
        public ActionResult favmatching(int messageid, bool fav)
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
