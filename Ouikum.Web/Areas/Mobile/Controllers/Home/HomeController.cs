using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2B.Web.Areas.Mobile.Controllers.Home
{
    public class HomeController : BaseController
    {
        //
        // GET: /m/Home/

        public ActionResult Index()
        {
            //search
            LoadProvinces();
            LoadBiztype();
            LoadCategory();

            return View();
        }

    }
}
