using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using Ouikum.Common;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public class MainController : BaseSecurityAdminController
    {
        //
        // GET: /Admin/AdminHome/

        public ActionResult Index()
        {

            RememberURL();
            if (!CheckIsAdmin())
                return Redirect(res.Pageviews.PvMemberSignIn);


            GetStatusUser();
            return View();
        }

    }
}
