using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
using Ouikum.Company;
using Ouikum.Common;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Product;
using System.Collections;
using Ouikum.Order;
using System.Transactions;

namespace Ouikum.Web.Admin
{
    public partial class ApprovePackageController : BaseSecurityAdminController
    {
        CommonService svCommon = new CommonService();
        #region Get: Index
        public ActionResult Index()
        {    
            RememberURL();
            if (!CheckIsAdmin(12))
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            ViewBag.EnumPackage = svCommon.SelectEnum(CommonService.EnumType.SearchByPackage);
            ViewBag.EnumPackageStatus = svCommon.SelectEnum(CommonService.EnumType.PackageStatus);
            ViewBag.EnumPackageType = svCommon.SelectEnum(CommonService.EnumType.PackageType);
            #endregion

            return View();
        }
        #endregion

        #region Post: Index
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            SetOrderPackagePager(form);
            List_DoloadData(BuyleadAction.Admin); 
            return PartialView("UC/GridApprove");
        }
        #endregion

        #region Detail
        [HttpGet]
        public ActionResult Detail(int OrderID = 0)
        {
            RememberURL();
            if (!CheckIsAdmin(12))
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                if (OrderID > 0)
                {
                    OrderService svOrder = new OrderService();
                    var OrderComp = svOrder.SelectData<view_OrderDetailComp>("OrderID,CompID,OrderCode,TotalPrice,OrderStatus", "IsDelete = 0 AND OrderID = " + OrderID).First();
                    ViewBag.OrderComp = OrderComp;

                    var Company = svOrder.SelectData<view_Company>(" * ", "IsDelete = 0 AND CompID = " + OrderComp.CompID).First();
                    ViewBag.Company = Company;

                    var OrderDetail = svOrder.SelectData<view_OrderDetail>("*", "IsDelete = 0 AND OrderID = " + OrderID);
                    ViewBag.OrderDetail = OrderDetail;

                    return View();

                }
                else
                {
                    return Redirect("~/Admin/ApprovePackage");
                }
            }
        }
        #endregion

    }
}
