using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Message;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum;
using Ouikum.Quotation;
using Prosoft.Service;
//using Prosoft.Base;

using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers
{


    public class ReportController : BaseSecurityController
    {
        //
        // GET: /Trade/
        string SQLWhere;
        string SqlSelect;
        CommonService svCommon = new CommonService();
        #region GET : Trade
        public ActionResult Trade(int compid, string Period = "")
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            GetStatusUser();
            var svComp = new CompanyService();
            //if (!string.IsNullOrEmpty(date))
            //    SQLWhere += SQLWhereDateTimeFromPeriod(date, "SendDate");

            var data = svComp.SelectData<b2bCompany>(" * ", " IsDelete = 0 AND CompID = " + compid);
            ViewBag.Company = data.First();
            ViewBag.FindDatePeriod = Period;
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            return View();
        }
        #endregion
         
        #region Post Supplier
        [HttpPost]
        public JsonResult Supplier(FormCollection form)
        {
            var svComp = new CompanyService();
            SetPager(form);
            var sqlWhere = svComp.CreateWhereAction(CompStatus.All, 0);
            sqlWhere += svComp.CreateWhereCause(0, "", ViewBag.TextSearch, 0, 0, 0, 0);
            var data = svComp.SelectData<view_Company>(" CompID , CompName , ContactPhone , ContactEmail ", sqlWhere, "", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);


            return Json(new { Suppliers = data, PageIndex = (int)ViewBag.PageIndex, TotalRow = svComp.TotalRow, TotalPage = svComp.TotalPage, PageSize = (int)ViewBag.PageSize, });
        }

        #endregion

        #region Post List
        [HttpPost]
        public ActionResult MsgTrade(FormCollection form)
        {
            MessageService svMessage = new Message.MessageService();
            SelectList_PageSize();
            SetPager(form);
            List<view_Message> Messages;
            int CompID = int.Parse(form["CompID"]);
            var SQLWhere = "(IsSend = 0 AND MsgFolderID = 1) AND (ToCompID = " + CompID + ") ";//svMessage.CreateWhereAction(MessageStatus.Inbox, CompID)

            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");
            }
            Messages = svMessage.SelectData<view_Message>("*", SQLWhere, " CreatedDate DESC ", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

            return Json(new { Messages = Messages, PageIndex = (int)ViewBag.PageIndex, TotalRow = svMessage.TotalRow, TotalPage = svMessage.TotalPage, PageSize = (int)ViewBag.PageSize, }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult QuotationTrade(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);

            var svQuotation = new QuotationService();

            SqlSelect = "QuotationID,QuotationCode,FromCompID,ToCompID,CompanyName,ReqFirstName,ReqLastName,ReqEmail,SendDate,RowFlag,RowVersion,IsDelete,IsRead,IsReject,IsImportance,ToCompName";

            int CompID = int.Parse(form["CompID"]);
            SQLWhere = svQuotation.CreateWhereAction(QuotationAction.Admin, CompID);

            if (!string.IsNullOrEmpty(form["Period"]))
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");

            var Quotations = svQuotation.SelectData<view_Quotation>(SqlSelect, SQLWhere, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.TotalRow = svQuotation.TotalRow;

            return Json(new { Quotations = Quotations, PageIndex = (int)ViewBag.PageIndex, TotalRow = svQuotation.TotalRow, TotalPage = svQuotation.TotalPage, PageSize = (int)ViewBag.PageSize, }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Quotation Detail

        public ActionResult QuotationDetail(int QuotationID = 0, string QuotationCode = "")
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }
            else
            { 
                GetStatusUser();
                if (QuotationID > 0)
                {
                    Quotation.QuotationService svQuotation = new Quotation.QuotationService();
                    var Quotation = svQuotation.SelectData<b2bQuotation>("*", "IsDelete = 0 AND QuotationID =" + QuotationID).First();

                    if (Quotation.ToCompID == LogonCompID || CheckIsAdmin(9))
                    {
                        ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
                        ViewBag.Quotation = Quotation;

                        return View();
                    }
                    else
                    {
                        return Redirect(res.Pageviews.PvAccessDenied);
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvNotFound);
                }
               
            }
        }
        #endregion

        #region Message Detail
        public ActionResult MessageDetail(int MessageID = 0, string MessageCode = "")
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            RememberURL();

            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }


            GetStatusUser();
            if (MessageID > 0)
            {
                Message.MessageService svMessage = new Message.MessageService();
                var Message = svMessage.SelectData<view_Message>("*", "IsDelete = 0 AND MessageID =" + MessageID).First();

                ViewBag.Message = Message;
                ViewBag.Title = Message.Subject; 

                if (Message.ToCompID == LogonCompID || CheckIsAdmin(9))
                {

                    #region Get FromComp Detail
                    if (Message.FromCompID > 0)
                    {
                        GetCompany((int)Message.FromCompID);
                    }
                    #endregion

                    #region Get ToComp Detail
                    if (Message.ToCompID > 0)
                    {
                        GetToCompany((int)Message.ToCompID);
                    }
                    #endregion
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
            ViewBag.EnumServiceType = new List<view_EnumData>();
            return View();

        }
        #endregion

        #region GetCompany
        public void GetToCompany(int CompID)
        {
            CompanyService svCompany = new CompanyService();
            string sqlSelect = "CompID,CompLevel,CompName,LogoImgPath,ContactImgPath,CompAddrLine1,CompSubDistrict,CompPostalCode,CompPhone,ContactFirstName,ContactLastName,ContactEmail,ContactAddrLine1,ContactSubDistrict,ContactPostalCode,ContactPhone";
            sqlSelect += ",CompDistrictName,CompProvinceName,ContDistrictName,ContProvinceName";
            var Company = svCompany.SelectData<view_Company>(sqlSelect, " CompID = " + CompID).First();
            ViewBag.CompanyDetail = Company;
        }
        #endregion

    }
}
