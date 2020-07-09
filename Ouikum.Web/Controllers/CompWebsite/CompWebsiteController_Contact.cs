using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using Ouikum.Company;
using Ouikum.Common;
//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Controllers
{
    public partial class CompWebsiteController : BaseController
    {
        #region Contact
        public ActionResult Contact(int id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Contact";
            int countcompany = DefaultWebsite(id, page);


            if (countcompany > 0)
            {

                string select = "ContactFirstName,ContactLastName,ContactPositionName,ContactPhone,ContactMobile,ContactFax";
                select += ",ContactAddrLine1,ContactAddrLine2,ContactImgPath,ContactDistrictID,ContactProvinceID,ContDistrictName,ContProvinceName,ContactPostalCode,MapImgPath,CompMapDetail,GMapLatitude,GMapLongtitude";
                select += ",GPinLatitude,GPinLongtitude,GZoom,ContactEmail,CompWebsiteUrl,CompProvinceID,LineID,FacebookUrl,CreatedDate,CompCode";
                SelectCompanyContactInfo(id, select);

                ViewBag.PageType = "Contact";
                GetStatusUser();

                return View();

                string sqlSelect_comp = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate,ServiceType";
                string sqlWhere_comp = "CompID =" + id;
                var company = svCompany.SelectData<view_Company>(sqlSelect_comp, sqlWhere_comp).First();
                ViewBag.Company = company;

            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }

        }
        #endregion

        #region if Company Google Map is null
        public ActionResult setDefaultGmap(int ProvinceID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            AddressService svAddress = new AddressService();
            var Provinces = svAddress.ListProvince().ToList();
            Hashtable Gmap = new Hashtable();

            if (ProvinceID > 0)
            {
                foreach (var it in Provinces.Where(m => m.ProvinceID == ProvinceID))
                {
                    Gmap.Add("GMapLatitude", it.Latitude);
                    Gmap.Add("GPinLatitude", it.Latitude);
                    Gmap.Add("GMapLongtitude", it.Longitude);
                    Gmap.Add("GPinLongtitude", it.Longitude);
                    Gmap.Add("GZoom", 10);
                }
            }
            else
            {
                ProvinceID = 1;
                foreach (var it in Provinces.Where(m => m.ProvinceID == ProvinceID))
                {
                    Gmap.Add("GMapLatitude", it.Latitude);
                    Gmap.Add("GPinLatitude", it.Latitude);
                    Gmap.Add("GMapLongtitude", it.Longitude);
                    Gmap.Add("GPinLongtitude", it.Longitude);
                    Gmap.Add("GZoom", 10);
                }
            }

            return Json(Gmap);
        }
        #endregion

    }
}
