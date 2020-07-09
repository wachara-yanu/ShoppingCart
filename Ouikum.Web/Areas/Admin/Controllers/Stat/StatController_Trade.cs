using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Message;
using Ouikum.Company;
using Ouikum;
using Ouikum.Quotation;
using Ouikum.Common;
using System.Collections;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{

    public partial class StatController : BaseSecurityAdminController
    {

        #region GET : Trade
        public ActionResult Trade()
        {
            if (!CheckIsAdmin())
            {
                return Redirect(res.Pageviews.PvAccessDenied);
            }
            GetStatusUser();
            return View();
        }
        #endregion

        #region GET : Post
        [HttpPost]
        public ActionResult Trade(FormCollection form)
        {
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
            var data = svComp.SelectData<view_Company>(" CompID , CompName , ContactPhone , ContactEmail ","Webid = 1 AND "+ sqlWhere, "", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

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
            var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Inbox, CompID);

            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");
            }
            Messages = svMessage.SelectData<view_Message>("* ", SQLWhere, " CreatedDate DESC ", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

            return Json(new { Messages = Messages, PageIndex = (int)ViewBag.PageIndex, TotalRow = svMessage.TotalRow, TotalPage = svMessage.TotalPage, PageSize = (int)ViewBag.PageSize, }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult QuotationTrade(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);

            var svQuotation = new QuotationService();

            SqlSelect = " * ";

            int CompID = int.Parse(form["CompID"]);
            SQLWhere = svQuotation.CreateWhereAction(QuotationAction.Admin, CompID);

            var a = form["Period"];
            if (!string.IsNullOrEmpty(form["Period"]))
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");

            var Quotations = svQuotation.SelectData<view_Quotation>(SqlSelect, SQLWhere, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.TotalRow = svQuotation.TotalRow;

            return Json(new { Quotations = Quotations, PageIndex = (int)ViewBag.PageIndex, TotalRow = svQuotation.TotalRow, TotalPage = svQuotation.TotalPage, PageSize = (int)ViewBag.PageSize, }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Post : Email
        [HttpPost, ValidateInput(false)]
        public JsonResult SendMail(int CompID, string Email, int WithUser)
        {
            var svMember = new MemberService();
            var data = svMember.SelectData<view_CompMember>(" * ", "IsDelete = 0 AND CompID = " + CompID);
            var IsResult = false;
            if(svMember.TotalRow > 0 ){
                var UserData = data.First();
            #region SendEmail

            #region variable
            //bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            var encrypt = new EncryptManager();
            #endregion

            #region Set Content & Value For Send Email
            string Subject = res.Admin.lblReportcontact_requestPrice +" "+ res.Common.lblDomainShortName;
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["CompName"] = UserData.CompName;
            EmailDetail["FirstName"] = UserData.FirstName;
            EmailDetail["pathLogo"] = pathlogo;
            if (WithUser == 1)
            {
                EmailDetail["Username"] = UserData.UserName;
                EmailDetail["Password"] = encrypt.DecryptData(UserData.Password);
            }
            EmailDetail["CompID"] = CompID;
            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendUserInformationTrade");
            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(Email);
            //mailTo.Add(UserData.Email);
            #endregion

            IsResult = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            #endregion

            }else{
                IsResult = false;
            }
            return Json(new { IsResult = IsResult, ErrorMsg = GenerateMsgError(svMember.MsgError) });
        }
        #endregion
    }
}