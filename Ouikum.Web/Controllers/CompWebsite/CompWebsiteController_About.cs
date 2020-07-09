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

        #region SelectCompany
        public view_Company SelectCompany()
        {
            var company = new view_Company();
            if (CheckIsLogin())
            {

                var Companies = svCompany.SelectData<view_Company>("*", "IsDelete = 0 AND emCompID =" + LogonEMCompID, null, 0, 0, false);
                if (Companies.Count() > 0)
                {
                    company = Companies.First();

                }

            }
            return company;
        }
        #endregion

        public ActionResult About(int id)
        {

            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "About";
            int countcompany = DefaultWebsite(id, page);

            if (countcompany > 0)
            {

                GetStatusUser();
                var Company = new view_Company();
                //var Company = SelectCompany();
                var Companies = svCompany.SelectData<view_Company>("*", "IsDelete = 0 AND CompID =" + id, null, 0, 0, false);
                if (Companies.Count() > 0)
                {
                    Company = Companies.First();

                }
                ViewBag.Company = Company;
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
