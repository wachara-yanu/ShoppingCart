using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Message;
using Ouikum.Company;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public partial class StatController : BaseSecurityAdminController
    {

        #region Message

        #region Message
        [HttpGet]
        public ActionResult Message()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(9) || CheckIsAdmin(15))
                {
                    GetStatusUser();
                    ViewBag.EnumMessage = svCommon.SelectEnum(CommonService.EnumType.SearchByMessage);
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }

            }
        }
        #endregion

        #region Post List
        public string SqlWhereSearchBy(int SearchBy,string SearchText)
        {
            string sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(SearchText))
            {
                if (SearchBy == 1)
                {
                    sqlWhere += "AND ToCompName Like N'%" + SearchText + "%' ";
                }
                else if (SearchBy == 2)
                {
                    sqlWhere += "AND ( FromCompName Like N'%" + SearchText + "%' OR FromName Like N'%" + SearchText + "%' )";
                }
                else if (SearchBy == 3)
                {
                    sqlWhere += "AND Subject Like N'%" + SearchText + "%' ";
                }
                else
                {
                    sqlWhere = "AND (Subject Like N'%" + SearchText + "%' OR FromCompName Like N'%" + SearchText + "%' OR FromName Like N'%" + SearchText + "%' OR ToCompName Like N'%" + SearchText + "%')";
                }
            }
            return sqlWhere;
        }
        [HttpPost]
        public ActionResult MessageList(FormCollection form)
        {
            MessageService svMessage = new MessageService();
            SelectList_PageSize();
            SetPager(form);
            List<view_Message> Messages;

            var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Report, LogonCompID);
            SQLWhere += SqlWhereSearchBy(int.Parse(form["SearchBy"]), form["SearchText"]);

            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");
            }
            Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);

            ViewBag.Messages = Messages;
            ViewBag.TotalPage = svMessage.TotalPage;
            ViewBag.TotalRow = svMessage.TotalRow;
            return PartialView("UC/MessageGrid");
        }
        #endregion

        #region Detail
        public ActionResult MessageDetail(int MessageID = 0, string MessageCode = "")
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {

                if (CheckIsAdmin(9))
                {
                    GetStatusUser();
                    if (MessageID > 0)
                    {
                        Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
                        var Message = svMessage.SelectData<view_Message>("*", "IsDelete = 0 AND MessageID =" + MessageID).First();

                        ViewBag.Message = Message;
                        ViewBag.msgtitle = Message.Subject;

                        #region Get FromComp Detail
                        if (Message.FromCompID > 0)
                        {
                            GetCompany((int)Message.FromCompID);
                        }
                        else
                        {
                            ViewBag.Company = null;
                        }
                        #endregion

                        #region Get ToComp Detail
                        if (Message.ToCompID > 0)
                        {
                            GetToCompany((int)Message.ToCompID);
                        }
                        else
                        {
                            ViewBag.CompanyDetail = null;
                        }
                        #endregion

                        return View();

                    }
                    else
                    {
                        return Redirect("~/Report/List");
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
               
            }
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

        #endregion

    }
}
