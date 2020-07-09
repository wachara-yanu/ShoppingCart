using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ouikum.Web.Admin
{
    public class PackageController : Controller
    {
        //
        // GET: /Admin/AdminPackage/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult New()
        {
            return View();
        }
        public ActionResult NewPackageGroup()
        {
            return View();
        }

    }
}
