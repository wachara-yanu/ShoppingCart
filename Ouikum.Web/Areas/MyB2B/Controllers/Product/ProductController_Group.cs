using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ouikum;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        [HttpPost]
        public ActionResult SaveGroupProduct(string ProductGroupName, int? GroupID, short? RowVersion)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svProductGroup = new ProductGroupService();
            var model = new b2bProductGroup();
            try
            {
                #region Set Model

                if (GroupID > 0)
                {
                    model.ProductGroupID = (int)GroupID;
                }
                else
                {
                    var sqlWhere = "IsDelete = 0 and CompID = " + LogonCompID;
                    var data = svProductGroup.SelectData<b2bProductGroup>("*", sqlWhere, "ListNo DESC");
                    if (svProductGroup.TotalRow == 0)
                    {
                        model.ListNo = 1;
                    }
                    else
                    {

                        model.ListNo = data.First().ListNo + 1;
                    }
                }
                if (RowVersion > 0)
                    model.RowVersion = (short)RowVersion;

                model.ProductGroupName = ProductGroupName;
                model.CompID = LogonCompID;
                model.IsShow = true;
                #endregion

                svProductGroup.SaveProductGroup(model);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svProductGroup.IsResult, MsgError = GenerateMsgError(svProductGroup.MsgError), ID = model.ProductGroupID });
        }

        [HttpPost]
        public ActionResult GroupProductList()
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProductGroup = new ProductGroupService();
            ViewBag.ProductGroups = svProductGroup.GetProductGroup(LogonCompID);
            return PartialView("UC/GroupProductList");
        }

        [HttpPost]
        public ActionResult DeleteProductGroup(int GroupID,short RowVersion)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            var svProductGroup = new ProductGroupService(); 
            try
            {
                svProductGroup.Delete(GroupID, LogonCompID);
                return Json(new { IsResult = svProductGroup.IsResult, MsgError = GenerateMsgError(svProductGroup.MsgError)});
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svProductGroup.IsResult, MsgError = GenerateMsgError(svProductGroup.MsgError)});
        }

        [HttpPost]
        public ActionResult ChangeListNoGroup(List<int> id, List<int> no)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svProductGroup = new ProductGroupService();
            try
            {
                svProductGroup.UpdateProductGroup(id, no);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svProductGroup.IsResult, MsgError = GenerateMsgError(svProductGroup.MsgError), ID = id });
        }

    }
}
