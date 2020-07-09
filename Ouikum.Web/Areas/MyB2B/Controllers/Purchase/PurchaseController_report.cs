using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Data.SqlClient;
using Ouikum.Purchase;
using System.Data;
using Prosoft.Service;
using Ouikum;
using Ouikum.Company;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class PurchaseController : BaseController
    {

        public ActionResult Report(string id)
        {
            CommonService svCommon = new Common.CommonService();
            PurchaseService svPurchase = new PurchaseService();
            var svCompany = new CompanyService();
            //int decrypt_id = DeCodeID(id);

            string sqlSelect = "AssignLeadID,AssignLeadName,ToContactName,IsDelete";
            string sqlWhere = "AssignLeadID = " + Convert.ToInt32(id); //+ " AND IsDelete = 0"
            var AssignLeads = svPurchase.SelectData<b2bAssignLead>(sqlSelect, sqlWhere).First();
            if (AssignLeads.IsDelete == true)
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
            else
            {

                sqlSelect = "CompID,CompName,ContactEmail,CompPhone,CompAddrLine1,CompAddrLine2,CompSubDistrict,DistrictName,ProvinceName,CompPostalCode";
                sqlWhere = "AssignLeadID = " + Convert.ToInt32(id);
                var Companies = svPurchase.SelectData<view_AssignLeadForReport>(sqlSelect, sqlWhere, "CompName ASC");

                ViewBag.AssignLeads = AssignLeads;
                ViewBag.CompanyLead = Companies;

                GetStatusUser();
                if (CheckIsLogin())
                {
                    ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
                }
                ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

                ViewBag.PageIndex = 1;
                ViewBag.PageSize = 10;
                ViewBag.TotalPage = svPurchase.TotalPage;
                ViewBag.TotalRow = svPurchase.TotalRow;
            }
            return View();
        }

    }
}
