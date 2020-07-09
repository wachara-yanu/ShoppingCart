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
using Ouikum.Quotation;
namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 
        public ActionResult CloseQuotation(string code, string password)
        {
            var svQuo = new QuotationService();
            if (CheckIsAdmin())
            {
                if (password == "12344")
                {
                    svQuo.CloseBidProductByCode(code);
                }
            } 
            return Json(new { result = svQuo.IsResult }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenQuotation(string code, string password)
        {
            var svQuo = new QuotationService();
            if (CheckIsAdmin())
            {
                if (password == "12344")
                {
                    svQuo.CloseBidProductByCode(code, false);
                }
            }
            return Json(new { result = svQuo.IsResult }, JsonRequestBehavior.AllowGet);
        }

    }
}
