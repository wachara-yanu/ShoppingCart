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

        public ActionResult Certify(int id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Certify";
            int countcompany = DefaultWebsite(id, page);

            if (countcompany > 0)
            {
                string sqlSelect = "CompCertifyID,CompID,CompName,CertifyName,CertifyImgPath";
                string sqlWhere = "IsDelete = 0 AND CompID =" + id;
                var certify = svCompany.SelectData<view_CompanyCertify>(sqlSelect, sqlWhere, "ListNo ASC");
                ViewBag.certify = certify;

                string sqlSelect1 = "CompProfileID,CompID,CompName,BizTypeName,AddrLine1,AddrLine2,DistrictName,ProvinceName,PostalCode,CeoName,ComercialNo,CompRegisDate,EmployeeCount,MainCustomer";
                string sqlWhere1 = "IsDelete = 0 AND CompID =" + id;
                var certifyData = svCompany.SelectData<view_CompanyProfile>(sqlSelect1, sqlWhere1);
                ViewBag.certifyData = certifyData.First();

                string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType,MainCustomer,SecondaryCustomer,EmployeeCount,RESEmployeeCount,QCEmployeeCount,CompOwner,ComercialNo,CompRegisDate,CompWebsiteCss,ProvinceName";
                string sqlWhere_comp = "CompID =" + id;
                var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                ViewBag.Company = company;
                ViewBag.PageType = "Certify";

                if (company.CompWebsiteCss == null)
                    company.CompWebsiteCss = 0;

                ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;
                ViewBag.titleCertify = "ใบรับรอง/คุณภาพ " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                //sqlSelect = "CompOwner,CompRegisDate,ComercialNo";
                //SelectCompanyContactInfo(id, sqlSelect);
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
