using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum;
using System.Collections;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers.AboutUs
{
    public class AboutUsController : BaseController
    {
        // GET: AboutUs
        public ActionResult Home()
        {
            GetStatusUser();

            if (CheckIsLogin())
            {

            }
            return View();
        }
    }
}