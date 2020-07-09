using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Ouikum.Common;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Transport;

namespace Ouikum.Web.Admin
{
    public class TransportController : BaseController
    {
        // GET: Admin/Transport

        TransportService svTransport;

        public ActionResult Index()
        {
            TransportService svTransport = new TransportService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                var Transport = svTransport.SelectData<OuikumTransport>("*", "IsDelete = 0 AND WebID = 1");
                ViewBag.Transport = Transport;
                ViewBag.TotalPage = svTransport.TotalPage;
                ViewBag.TotalRow = svTransport.TotalRow;
                return View();
            }
        }
        [HttpPost]
        public ActionResult TransportList(FormCollection form)
        {
            var svTranport = new TransportService();
            String SQLWhere = "IsDelete = 0 AND IsShow = 1";
            SelectList_PageSize();
            SetPager(form);
            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND BannerTitle LIKE N'%" + form["SearchText"].Trim() + "%'";
            }
            else
            {
                GetStatusUser();
                SetPager(form);
                var Transport = svTranport.SelectData<OuikumTransport>("*", "IsDelete = 0 AND IsShow = 1", "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                ViewBag.Transport = Transport;
                ViewBag.TotalPage = svTranport.TotalPage;
                ViewBag.TotalRow = svTranport.TotalRow;
            }
            var transport = svTranport.SelectData<OuikumTransport>("*", SQLWhere, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svTranport.TotalPage;
            ViewBag.TotalRow = svTranport.TotalRow;
            ViewBag.Transport = transport;
            return PartialView("UC/GridTransport");

        }
    }
}