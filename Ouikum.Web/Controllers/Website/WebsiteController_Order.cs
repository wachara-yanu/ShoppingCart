using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Company;

//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Controllers
{
    public partial class WebsiteController : BaseController
    {

        public ActionResult Order(int id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Order";
            int countcompany = DefaultWebsite(id, page);

            if (countcompany > 0)
            {
                #region Payment
                string sqlSelect = "CompPaymentID,AccNo,AccName,AccType,BankName,BranchName";
                string sqlWhere = "IsDelete = 0 AND CompID =" + id;
                var payment = svCompany.SelectData<view_CompanyPayment>(sqlSelect, sqlWhere, "ListNo ASC");

                ViewBag.CompanyPayment = payment;

                #endregion

                #region Shipment
                sqlSelect = "CompShipmentID,ShipmentName,ShipmentDuration,PackingName,Remark";
                sqlWhere = "IsDelete = 0 AND CompID =" + id;
                var shipment = svCompany.SelectData<b2bCompanyShipment>(sqlSelect, sqlWhere, "ListNo ASC");

                ViewBag.CompanyShipment = shipment;
                var select = "";
                SelectCompanyContactInfo(id, select);
                string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType,CompWebsiteCss";
                string sqlWhere_comp = "CompID =" + id;

                var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                ViewBag.Company = company;
                ViewBag.PageType = "Order";

                if (company.CompWebsiteCss == null)
                    company.CompWebsiteCss = 0;

                ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;

                #endregion
                GetStatusUser();
                return View();
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
        }

    }
}
