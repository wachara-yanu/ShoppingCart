using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Product;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers
{
    public class PageTestController : Controller
    { 
        public ActionResult index()
        {
            return View();
        }
    }
}
