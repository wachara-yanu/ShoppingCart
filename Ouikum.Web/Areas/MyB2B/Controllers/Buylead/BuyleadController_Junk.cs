using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using Prosoft.Base;
using Ouikum;
using Ouikum.Buylead;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class BuyleadController : BaseSecurityController
    {
        // GET: /MyB2B/Buylead/


        #region GetManageJunk
        [HttpGet]
        public ActionResult Junk()
        {
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svCategory = new CategoryService();
            GetStatusUser();


            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            ViewBag.TotalRow = 0;

            SelectList_PageSize();
            DoLoadComboBoxProductStatus();
            DoLoadQtyUnits();
            CountMessage();
            CountQuotation();
            ViewBag.CateLevel1 = LogonCompLevel;
            ViewBag.MemberType = LogonMemberType;
            //select Category
            ViewBag.IndrustryCateLV1 = svCategory.ListMenuJunkCategory(LogonCompID, "B2BBuylead");
            //ViewBag.WholesaleCateLV1 = svCategory.ListWholesaleCategory(LogonCompID,"B2BBuylead");
            ViewBag.PageType = "Buylead";
            ViewBag.MenuName = "Junk";
            return View();
        }
        #endregion

        #region GetManageJunk
        [HttpPost]
        public ActionResult Junk(FormCollection form)
        {
            SetPager(form);
            SetBuyleadPager(form);
            List_DoloadData(BuyleadAction.Junk);
            ViewBag.PageType = "Buylead";
            return PartialView("UC/Junk/Content");
        }
        #endregion


        #region PostMoveToJunk
        [HttpPost]
        public ActionResult MoveToJunk(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svBuylead = new BuyleadService();

            try
            {
                svBuylead.MoveToJunk(BuyleadID, CateLV1, CateLV2, CateLV3, LogonCompID);
            }
            catch (Exception ex)
            {
                svBuylead.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError) });
        }
        #endregion

        #region PostDeleteBuylead
        [HttpPost]
        public ActionResult DeleteBuylead(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svBuylead = new BuyleadService();

            try
            {
                svBuylead.Delete(BuyleadID, CateLV1, CateLV2, CateLV3, LogonCompID);
            }
            catch (Exception ex)
            {
                svBuylead.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError) });
        }
        #endregion

        #region PostRestore
        [HttpPost]
        public ActionResult Restore(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svBuylead = new BuyleadService();

            try
            {
                svBuylead.ReStore(BuyleadID, CateLV1, CateLV2, CateLV3, LogonCompID);
            }
            catch (Exception ex)
            {
                svBuylead.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError) });
        }
        #endregion

    }
}
