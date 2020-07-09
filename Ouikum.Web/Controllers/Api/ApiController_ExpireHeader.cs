using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Quotation;
using res = Prosoft.Resource.Web.Ouikum;
using System.Collections;
using Ouikum.Product;
using System.Diagnostics;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 

        #region Get HeaderExpire
        public ActionResult HeaderExpire(string prefix,string folder,string days,string password )
        {
            if (!string.IsNullOrEmpty(prefix)) { prefix = "--prefix=" + prefix; }
            var arr = new string[] { prefix
                , "--folder=" + folder
                , "--days=" + days
                , "--password=" + password };

            const string argsSeparator = " ";
            string args = string.Join(argsSeparator, arr);
            var output = string.Empty;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Server.MapPath("~/Service/ExpiredHeader/SoGoodAPI_ConfigIIS.EXE"),
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            return View();
        }
        #endregion 

         

    }
}
