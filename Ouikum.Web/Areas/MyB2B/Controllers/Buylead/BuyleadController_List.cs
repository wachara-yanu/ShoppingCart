using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
//using Prosoft.Base;
using Ouikum.Buylead;
using Ouikum.Category;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class BuyleadController : BaseSecurityController
    {
        // GET: /MyB2B/Buylead/

        #region GetManageBuylead
        [HttpGet]
        public ActionResult Index(string PStatus, int? CateLevel, int? CateID)
        {
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            CommonService svCommon = new CommonService();
            GetStatusUser();
            ViewBag.EnumBuyleadStatus = svCommon.SelectEnum(CommonService.EnumType.ProductStatus);
            var svCategory = new CategoryService();


            ViewBag.PageIndex = 1;
            ViewBag.TotalRow = 0;
            ViewBag.PStatus = PStatus != null ? PStatus : "0"; 
            ViewBag.CateLevel = CateLevel != null ? CateLevel : 0;
            ViewBag.CateID = CateID != null ? CateID : 0;

            SelectList_PageSize();
            DoLoadComboBoxProductStatus();
            CountMessage();
            CountQuotation();
            ViewBag.CateLevel1 = LogonCompLevel;
            ViewBag.MemberType = LogonMemberType;
            //select Category
            ViewBag.IndrustryCateLV1 = svCategory.ListMenuProductCategory(LogonCompID, "B2BBuylead");
            //ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory(LogonCompID, "B2BBuylead");
            ViewBag.PageType = "Buylead";
            ViewBag.MenuName = "Index";

            return View();
        }
        #endregion

        #region PostManageBuylead

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SetPager(form);
            SetBuyleadPager(form);
            List_DoloadData(BuyleadAction.BackEnd);
            ViewBag.PageType = "Buylead";
            return PartialView("UC/ManageBuyleadContent");
        }

        #endregion

        #region PostBuyleadDetailGetByID
        [HttpPost]
        public ActionResult BuyleadGetByID(int ID)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svBuylead = new BuyleadService();
            sqlSelect = @"BuyleadID,BuyleadCode,BuyleadName,BuyleadIMGPath,ModifiedDate, CreatedDate,RowFlag,ListNo,
           IsShow,CateLV1,CateLV2,CateLV3,CompID,CategoryName,ViewCount,BuyleadDetail";
            sqlWhere = "BuyleadID=" + ID;
            var Buylead = svBuylead.SelectData<view_BuyLead>(sqlSelect, sqlWhere, null, 0, 0).First();

            Buylead.ModifiedDate = DataManager.ConvertToDateTime(Buylead.ModifiedDate);
            Buylead.CreatedDate = DataManager.ConvertToDateTime(Buylead.CreatedDate);

            ViewBag.BuyleadGetByID = Buylead;
            return PartialView("UC/QuickBuyleadDetail");
        }
        #endregion

        #region Post Show/NotShow
        [HttpPost]
        public ActionResult SaveIsShow(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int IsShow)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svBuylead = new BuyleadService();
            svBuylead.SaveIsShow(BuyleadID, CateLV1, CateLV2, CateLV3, LogonCompID, IsShow);
            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError) });
        }
        #endregion

    }
}
