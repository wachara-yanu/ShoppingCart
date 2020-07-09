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

        public ActionResult About(int id)
        {

            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "About";
            int countcompany = DefaultWebsite(id, page);
            if (countcompany > 0)
            {
                SelectCompanyContactInfo(id, "CompWebsiteUrl");
                GetStatusUser();
                string sqlSelect = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,LineID,FacebookUrl,CreatedDate,ContactEmail,CompWebsiteCss,CompHistory";
                sqlSelect += ",FactoryRemark,FactoryFax,FactoryMobile,FactoryPhone,FactorySize,YearEstablished,CompFounder,BizTypeName,BizTypeOther";
                string sqlWhere = "CompID =" + id;

                var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                ViewBag.Company = company;

                if (company.CompWebsiteCss == null)
                    company.CompWebsiteCss = 0;

                ViewBag.CompanyWebsiteCss = company.CompWebsiteCss;
                ViewBag.PageType = "About";
                return View();
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
            
        }

    }
}
