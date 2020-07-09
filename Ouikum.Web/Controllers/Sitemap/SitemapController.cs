using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;

namespace Ouikum.Web.Controllers.Sitemap
{
    public class SitemapController : BaseController
    {
        //
        // GET: /Sidemap/

        public ActionResult Index()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            GetStatusUser();
            return View();
        }

    }
}
