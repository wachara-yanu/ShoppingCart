using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Message;
using Ouikum.Web.Models;
using res = Prosoft.Resource.Web.Ouikum;
namespace Ouikum.Web.Controllers
{
    public class ManualController : BaseController
    {
        CommonService svCommon = new Common.CommonService();
        // GET: /Manual/
        // GET: /CustomerGuide/Introduction
        #region Introduction
        public ActionResult Guide()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

        // GET: /CustomerGuide/Guide/Content/_Member - Partial view
        public ActionResult _Introduction()
        {
            return View();
        }
        #endregion

        // GET: /CustomerGuide/Guide
        #region Member
        public ActionResult Member()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

        // GET: /CustomerGuide/Guide/Content/_Member - Partial view
        public ActionResult _Member()
        {
            return View();
        }
        #endregion

        // GET: /CustomerGuide/Company
        #region Company
        public ActionResult Company()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        // GET: /CustomerGuide/Guide/Content/_CompanyWeb - Partial view
        public ActionResult _Company()
        {
            return View();
        }
        #endregion

        // GET: /CustomerGuide/Supplier
        #region Supplier
        public ActionResult Supplier()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        // GET: /CustomerGuide/Guide/Content/_Supplier - Partial view
        public ActionResult _Supplier()
        {
            return View();
        }
        #endregion

        // GET: /CustomerGuide/Buyer
        #region Buyer
        public ActionResult Buyer()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        // GET: /CustomerGuide/Guide/Content/_Buyer - Partial view
        public ActionResult _Buyer()
        {
            return View();
        }
        #endregion

        // Get: /CustomerGuide/FAQ
        #region FAQ
        public ActionResult FAQ()
        {
            GetStatusUser();
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        // GET: /CustomerGuide/Guide/Content/_Buyer - Partial view
        public ActionResult _FAQ()
        {
            return View();
        }
        #endregion
    }
}
