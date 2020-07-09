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

        #region Contact
        public ActionResult Contact()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(GetAppSetting("LoginPage"));
            }
            else
            {
                if (CheckIsAdmin(9) || CheckIsAdmin(10) || CheckIsAdmin(11) || CheckIsAdmin(12)|| CheckIsAdmin(15))
                {
                    GetStatusUser();
                    ViewBag.EnumSearchByMember = svCommon.SelectEnum(CommonService.EnumType.SearchByMember);
                    return View();
                }
                else
                {
                    return Redirect(GetAppSetting("AcessDenied"));
                }

            }
        }
        #endregion

        #region Post ContactList List
        [HttpPost]
        public ActionResult ContactList(FormCollection form)
        {
            int CompID = LogonCompID;
            var svCompany = new CompanyService();
            SelectList_PageSize();
            SetPager(form);
            SQLWhere = svCompany.CreateWhereAction(CompStatus.HaveProduct, 0);
            if (DataManager.ConvertToInteger(form["PIndex"]) == 1)
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }
            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND ((CompName LIKE N'%" + form["SearchText"].Trim() + "%') OR (CompCode LIKE N'%" + form["SearchText"].Trim() + "%'))";
            }
            switch (DataManager.ConvertToInteger(form["Sort"]))
            {
                case 1:
                    SQlOrderBy = " CompViewCount DESC";
                    break;
                case 2:
                    SQlOrderBy = " ContactCount DESC";
                    break;
                case 3:
                    SQlOrderBy = " ProductCount DESC";
                    break;
                case 4:
                    SQlOrderBy = " ProductViewCount DESC";
                    break;
                case 5:
                    SQlOrderBy = " TelCount DESC";
                    break;
                case 6:
                    SQlOrderBy = " QuoCount DESC";
                    break;
            }
            SQLWhere += " AND WebID = " + res.Config.WebID;
            
            ViewBag.Company = svCompany.SelectData<view_CompanyContact>(" * ", SQLWhere, SQlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;

            return PartialView("UC/ContactGrid");
        }
        #endregion

    }
}