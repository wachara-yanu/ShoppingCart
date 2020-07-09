using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Company;
using Ouikum.Common;
using System.Transactions;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public partial class ApproveMemberPaidController : BaseSecurityAdminController
    {
        #region Get: List
        public ActionResult Index()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsAdmin(13))
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            ViewBag.PStatus = "N";
            ViewBag.EnumSearchByMemberPaid = svCommon.SelectEnum(CommonService.EnumType.SearchByMemberPaid);
            ViewBag.EnumMemberPaidStatus = svCommon.SelectEnum(CommonService.EnumType.MemberPaidStatus);
            #endregion

            return View();
        }
        #endregion

        #region Post: List
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            SetMemberPaidPager(form);
            List_DoloadData();
            return PartialView("UC/GridApprove");
        }
        #endregion

        #region Approve
        [HttpPost]
        public ActionResult Approve(List<int> ID)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.ApprovePayment(ID, LogonCompCode);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region Reject
        [HttpPost]
        public ActionResult Reject(List<int> ID, string Remark)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.RejectPayment(ID, Remark, LogonCompCode);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region Detail
        public ActionResult Detail(int MemberPaidID)
        {
            var svOrder = new OrderService();
            var svCompany = new CompanyService();

            var MemberPaid = svOrder.SelectData<b2bMemberPaid>(" * ", "IsDelete = 0 AND RowFlag > 0 AND MemberPaidID = " + MemberPaidID).First();
            var Order = svOrder.SelectData<b2bOrder>(" * ", " MemberPaidID = " + MemberPaidID).First();
            var Bank = svOrder.SelectData<emBank>(" * ", "BankID = " + MemberPaid.BankID).First();
            var PaymentAccount = svOrder.SelectData<b2bPaymentAccount>(" * ", "PaymentAccID = " + MemberPaid.PaymentAccID).First();
            var BankAcc = svOrder.SelectData<emBank>(" * ", "BankID = " + PaymentAccount.BankID).First();
            var OrderDetail = svOrder.SelectData<view_OrderDetail>(" * ", "RowFlag > 0 AND OrderID = " + Order.OrderID, "Price desc");
            var Company = svCompany.SelectData<view_Company>(" * ", "CompID = " + MemberPaid.CompID).First();

            ViewBag.MemberPaid = MemberPaid;
            ViewBag.Bank = Bank;
            ViewBag.BankAcc = BankAcc;
            ViewBag.PaymentAccount = PaymentAccount;
            ViewBag.Order = Order;
            ViewBag.Company = Company;
            ViewBag.OrderDetail = OrderDetail;
            GetStatusUser();
            return View();
        }
        #endregion

    }
}
