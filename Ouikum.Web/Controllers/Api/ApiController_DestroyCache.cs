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
using System.Runtime.Caching;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 
        public bool IsResult { get; set; }

        public ActionResult DestroyCategory()
        {
            var cate1 = 0;
            var cate2 = 0;
            var cate3 = 0;
            var cate4 = 0;
            cate1 = 3068; cate2 = 2598; cate3 = 3494; cate4 = 1189;
            IsResult = DestroyCategoryShowCase(1, cate1);
            IsResult = DestroyCategoryShowCase(2, cate2);
            IsResult = DestroyCategoryShowCase(3, cate3);
            IsResult = DestroyCategoryShowCase(4, cate4); 

            return Json(new { isResult = IsResult }, JsonRequestBehavior.AllowGet);
        }
        public bool DestroyCategoryShowCase(int ArrivalNo, int Cate)
        {
            var isresult = false; 
            var name = "showcaseno" + ArrivalNo + "cate" + Cate;
            try
            {
                if (MemoryCache.Default[name] != null)
                {
                    MemoryCache.Default.Add(name, null, DateTime.Now.AddDays(-2));
                    isresult = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false; 
            }
            return isresult;
        }

    }
}
