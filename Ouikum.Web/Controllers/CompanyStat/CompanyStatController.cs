using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using Ouikum.Message;
using Ouikum.Company;
using Prosoft.Service;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Quotation;
using System.Web.Mail;
using Ouikum.Product;

namespace Ouikum.Controllers
{
    public partial class CompanyStatController : BaseController
    {
        //
        // GET: /Message/
        string SqlSelect, SQLWhere, SQlOrderBy;

        public ActionResult Index()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(GetAppSetting("LoginPage"));
            }
            else
            {
                GetStatusUser();
                ViewBag.SearchProductByComp = svCommon.SelectEnum(CommonService.EnumType.SearchProductByComp);
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.PageType = "CompanyStat";
                return View();
            }
        }

        #region Post ContactList List
        [HttpPost]
        public ActionResult IndexList(FormCollection form)
        {
            int CompID = LogonCompID;
            var svProduct = new ProductService();
            SelectList_PageSize();
            SetPager(form);
            SQLWhere = "IsDelete = 0 AND CompID = " + CompID;
            if (DataManager.ConvertToInteger(form["PIndex"]) == 1)
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }
            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND ((ProductName LIKE N'%" + form["SearchText"].Trim() + "%') OR (ProductCode LIKE N'%" + form["SearchText"].Trim() + "%'))";
            }
            switch (DataManager.ConvertToInteger(form["Sort"]))
            {
                case 1:
                    SQlOrderBy = " ViewCount DESC";
                    break;
                case 2:
                    SQlOrderBy = " QuotationCount DESC";
                    break;
                case 3:
                    SQlOrderBy = " TelCount DESC";
                    break;
            }

            ViewBag.Product = svProduct.SelectData<b2bProduct>(" * ", SQLWhere, SQlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svProduct.TotalPage;
            ViewBag.TotalRow = svProduct.TotalRow;

            return PartialView("UC/CompanyStatGrid");
        }
        #endregion

    }
}
