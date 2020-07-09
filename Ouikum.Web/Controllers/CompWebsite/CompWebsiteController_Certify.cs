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
    public partial class CompWebsiteController : BaseController
    {

        public ActionResult Certify(int id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Certify";
            int countcompany = DefaultWebsite(id, page);

            if (countcompany > 0)
            {

                //if (ViewBag.WebCompLevel == 3 && ViewBag.CertifyCount > 0)
                //{
                    string sqlSelect = "CompCertifyID,CompID,CompName,CertifyName,CertifyImgPath";
                    string sqlWhere = "IsDelete = 0 AND CompID =" + id;
                    var certify = svCompany.SelectData<view_CompanyCertify>(sqlSelect, sqlWhere, "ListNo ASC");
                    ViewBag.certify = certify;

                    sqlSelect = "CompOwner,CompRegisDate,ComercialNo";
                    SelectCompanyContactInfo(id, sqlSelect);
                    GetStatusUser();
                    //var Company = SelectCompany();
                    string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType,MainCustomer,SecondaryCustomer,EmployeeCount,RESEmployeeCount,QCEmployeeCount,CompOwner,ComercialNo,CompRegisDate";
                    string sqlWhere_comp = "CompID =" + id;
                    var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                    ViewBag.Company = company;
                    ViewBag.PageType = "Certify";

                    return View();
                //}
                //else
                //{
                //    GetStatusUser();
                //    LinkPathCompanyWebsite((string)ViewBag.WebCompName, id);
                //    return Redirect(PathWebsiteHome);
                //}
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }

        }

    }
}
